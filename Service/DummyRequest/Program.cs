using EasyNetQ;
using Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyRequest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus(RabbitUtil.ConnectionString()))
            {
                var request = new TestRequestMessage { Text = "Hello from the client! " };

                bus.RequestAsync<LoginCommand, LoginResult>(new LoginCommand { id = "gb", pw = "pw" }).ContinueWith(e =>
                {
                    Console.WriteLine("Login result: " + e.Result.Result);


                    bus.RequestAsync<BattleCommand, BattleResult>(new BattleCommand()).ContinueWith(e2 =>
                    {
                        Console.WriteLine("Got response!", e2.Result.Result);
                        Console.WriteLine(" ==> '{0}'", e2.Result.Result);

                        bus.RequestAsync<BuyCommand, BuyResult>(new BuyCommand { shopId = 123 }).ContinueWith(e3 =>
                        {
                            Console.WriteLine("Got response!", e3.Result.Result);
                            Console.WriteLine(" ==> '{0}'", e3.Result.Result);

                        });
                    });
                });

                Console.WriteLine("Request finished.");
                Console.ReadLine();
            }
        }
    }
}
