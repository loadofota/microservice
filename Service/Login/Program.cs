using EasyNetQ;
using Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus(RabbitUtil.ConnectionString()))
            {
                bus.Respond<LoginCommand, LoginResult>(request =>
                {
                    Console.WriteLine("Request received. - " + request.GetType().ToString());
                    Console.WriteLine(" ID = " + request.id);
                    Console.WriteLine(" PW = " + request.pw);

                    Task.Delay(1000).Wait();

                    return new LoginResult { Result = string.Format("Login id={0} pw={1} success.", request.id, request.pw) };
                });

                Console.ReadLine();
            }
        }
    }
}
