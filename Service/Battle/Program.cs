using EasyNetQ;
using Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus(RabbitUtil.ConnectionString()))
            {
                var message = new TestMessage { Text = "Hello Rabbit" };
                bus.Publish(message);

                bus.Respond<TestRequestMessage, TestResponseMessage>(request => {

                    Console.WriteLine("Request received. - " + request.Text);

                    Task.Delay(5000).Wait();

                    return new TestResponseMessage { Text = request.Text + " all done!" };
                });

                bus.Respond<BattleCommand, BattleResult>(request =>
                {

                    Console.WriteLine("Request received. - " + request.GetType().ToString());

                    Task.Delay(3000).Wait();

                    return new BattleResult { Result = "Win" };
                });

                Console.ReadLine();
            }
        }
    }
}
