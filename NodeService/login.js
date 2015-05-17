var q = 'Packet.Login2Command:Packet';

function bail(err) {
	console.error(err);
	process.exit(1);
}

// Publisher
function publisher(conn) {
	conn.createChannel(on_open);
	function on_open(err, ch) {
		if (err != null) bail(err);
		ch.assertQueue(q);
		ch.sendToQueue(q, new Buffer('something to do'));
	}
}

// Consumer
function consumer(conn) {
	var ok = conn.createChannel(on_open);
	function on_open(err, ch) {
    console.log('Consumer opened');

		if (err != null) bail(err);
		ch.assertQueue(q);
		ch.consume(q, function(msg) {
      console.log('Consuming a message...');
			if (msg !== null) {
				console.log(msg.content.toString());
				ch.ack(msg);
			}
		});
	}
}

require('amqplib/callback_api')
	.connect('amqp://gb:kimgoyub@gbjp.cloudapp.net/ttl', function(err, conn) {
		if (err != null) bail(err);
		consumer(conn);
		//publisher(conn);
	});

