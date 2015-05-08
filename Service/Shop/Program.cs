using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packet;
using EasyNetQ;

namespace Shop
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus(RabbitUtil.ConnectionString()))
            {
                var message = new TestMessage { Text = "Hello Rabbit" };
                bus.Publish(message);

                bus.RespondAsync<BuyCommand, BuyResult>(async request =>
                {
                    Console.WriteLine("Request received. - " + request.GetType().ToString());
                    Console.WriteLine(" Param " + request.shopId);

                    await Task.Delay(2000);

                    return new BuyResult { Result = "Shop item " + request.shopId + " bought." };
                });

                Console.ReadLine();
            }
        }
    }
}
