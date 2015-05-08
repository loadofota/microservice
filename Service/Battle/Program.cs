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

                bus.RespondAsync<BattleCommand, BattleResult>(async request =>
                {

                    Console.WriteLine("Request received. - " + request.GetType().ToString());

                    await Task.Delay(3000);
                    
                    
                    return new BattleResult { Result = "Win" };
                });

                Console.ReadLine();
            }
        }
    }
}
