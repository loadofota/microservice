import tornado.httpserver
import tornado.websocket
import tornado.ioloop
import tornado.web
import pika
from pika.adapters.tornado_connection import TornadoConnection
import uuid
import json

class TornadoQueueConnection(object):
 
    def __init__(self, host, user='guest', password='guest', vhost='/'):
        if tornado is None:
            raise Exception('You must add tornado to your requirements!')
        if pika is None:
            raise Exception('You must add pika to your requirements!')
        self._parameters = pika.ConnectionParameters(
            host=host,
            credentials=pika.PlainCredentials(user, password),
            virtual_host=vhost
        )
        self._connection = None
        self._channel = None
 
        self.ioloop = tornado.ioloop.IOLoop.instance()
        self.ioloop.add_timeout(0, self._connect)
        self._delivery_tag = 0
        self._confirmation_callbacks = {}
        
        self.client_map = {}
 
    def publish(self, exchange, routing_key, headers, body, callback):
        properties = pika.BasicProperties(content_type='text/plain')
 
        if self._connection is None or self._connection.is_closed:
            self._connect()
            callback(False)
        if self._channel is None or self._channel.is_closed:
            self._open_channel()
            callback(False)
 
        self._channel.basic_publish(exchange, routing_key, body, properties)
 
        self._delivery_tag += 1
        self._confirmation_callbacks[self._delivery_tag] = callback
 
    def publish_json(self, exchange, routing_key, headers, body, callback):
        data = ujson.dumps(body)
        self.publish(exchange, routing_key, headers, data, callback)
 
    def _on_delivery_confirmation(self, method_frame):
        confirmation_type = method_frame.method.NAME.split('.')[1].lower()
        tag = method_frame.method.delivery_tag
        if confirmation_type == 'ack':
            success = True
        else:
            success = False
 
        callback = self._confirmation_callbacks[tag]
        del self._confirmation_callbacks[tag]
        callback(success)
 
    def close(self):
        self._connection.close()
 
    def _connect(self):
        self.connection = TornadoConnection(
            self._parameters,
            on_open_callback=self._on_connected,
            stop_ioloop_on_close=False,
        )
 
    def _on_connected(self, connection):
        self._connection = connection
        self._connection.add_on_close_callback(self._on_connection_closed)
        self._open_channel()
        
    def _on_connection_closed(self, method_frame):
        self._connection = None
        self._connect()
 
    def _open_channel(self):
        self.connection.channel(self._on_channel_open)
 
    def _on_channel_open(self, channel):
        self._channel = channel
        #self._channel.confirm_delivery(self._on_delivery_confirmation)
        
        self._channel.queue_declare(self.callback_queue_declare, exclusive=True)
 

    def on_response(self, ch, method, props, body):
        print 'on_response called'
        
        client = self.client_map[props.correlation_id]
        
        if client != None:
            
            print 'on_response ' + props.type + ' ' + body
            
            client.write_message(body)
            
            del self.client_map[props.correlation_id]

    def call(self, client, cmd_type, cmd_body):
        
        corr_id = str(uuid.uuid4())
        
        self.client_map[corr_id] = client
        
        self._channel.basic_publish(exchange='easy_net_q_rpc',
                                   routing_key=cmd_type, #'Packet.LoginCommand:Packet',
                                   properties=pika.BasicProperties(
                                         type = cmd_type, #'Packet.LoginCommand:Packet',
                                         reply_to = self.callback_queue,
                                         correlation_id = corr_id,
                                         ),
                                   body=cmd_body) #'{"id":"gb", "pw":"pw"}')
                                   
        

    def callback_queue_declare(self, result):
        
        print 'callback_queue_declare called'
    
        self.callback_queue = result.method.queue
        
        self._channel.basic_consume(self.on_response, no_ack=True,
                                   queue=self.callback_queue)
    
    
class WSHandler(tornado.websocket.WebSocketHandler):
    def open(self):
        print 'new connection'
        self.write_message("Welcome (new connection)")
      
    def on_message(self, message):
        print 'on_message called'
        
        print 'callback_queue_declare %s' % message
        json_message = json.loads(message)
        rmq.call(self, json_message['$type'].replace(', ', ':'), message)
        
    def on_close(self):
      print 'connection closed'
 
 
application = tornado.web.Application([
    (r'/ws', WSHandler),
])


rmq = TornadoQueueConnection('localhost', 'gb', 'kimgoyub', 'ttl')

if __name__ == "__main__":
    http_server = tornado.httpserver.HTTPServer(application)
    http_server.listen(19191)
    
    tornado.ioloop.IOLoop.instance().start()
