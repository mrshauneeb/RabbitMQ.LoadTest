﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using RabbitMQ.LoadTest.Messages;

namespace RabbitMQ.LoadTest.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            string defaulthostport = ConfigurationManager.AppSettings["Host"] + ":" + ConfigurationManager.AppSettings["Port"];
            string[] hostport = args.Length > 0 ? args[0].Split(':') : defaulthostport.Split(':');

            string host = hostport[0];
            ushort port = hostport.Length > 1 ? Convert.ToUInt16(hostport[1]) : Convert.ToUInt16(ConfigurationManager.AppSettings["Port"]);

            string vhost = args.Length > 1 ? args[1] : ConfigurationManager.AppSettings["VHost"];
            string username = args.Length > 2 ? args[2] : ConfigurationManager.AppSettings["RabbitMQUser"];
            string password = args.Length > 3 ? args[3] : ConfigurationManager.AppSettings["RabbitMQPassword"];

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            int threads = args.Length > 4 ? Convert.ToInt16(args[4]) : Convert.ToInt16(ConfigurationManager.AppSettings["Threads"]);

            Task[] tasks = new Task[threads];

            for (int i = 0; i < tasks.Length; i++)
            {
                string threadno = i.ToString();
                tasks[i] = Task.Factory.StartNew(() => Subscribe(host, port, vhost, username, password, threadno, token), token);
            }

            Console.WriteLine("Subscription Started. Hit enter to quit");
            Console.ReadLine();
            tokenSource.Cancel();

        }

        protected static void Subscribe(string host, ushort port, string vhost, string username, string password, string threadno, CancellationToken token)
        {
            var messages = new List<object>(10);
            messages.Add(new XMLMessage0());
            messages.Add(new XMLMessage1());
            messages.Add(new XMLMessage2());
            messages.Add(new XMLMessage3());
            messages.Add(new XMLMessage4());
            messages.Add(new XMLMessage5());
            messages.Add(new XMLMessage6());
            messages.Add(new XMLMessage7());
            messages.Add(new XMLMessage8());
            messages.Add(new XMLMessage9());

            using (var bus = RabbitHutch.CreateBus(host, port, vhost, username, password, 3, serviceRegister => serviceRegister.Register<IEasyNetQLogger>(serviceProvider => new NullLogger())))
            {
                int counter = 0;

                switch (Convert.ToInt32(threadno) % 10)
                {
                    case 0:
                        bus.Subscribe<XMLMessage0>("XML_subscriber0", message => outputtoconsole(message.XMLString, counter++, threadno));
                        break;
                    case 1:
                        bus.Subscribe<XMLMessage1>("XML_subscriber1", message => outputtoconsole(message.XMLString, counter++, threadno));
                        break;
                    case 2:
                        bus.Subscribe<XMLMessage2>("XML_subscriber2", message => outputtoconsole(message.XMLString, counter++, threadno));
                        break;
                    case 3:
                        bus.Subscribe<XMLMessage3>("XML_subscriber3", message => outputtoconsole(message.XMLString, counter++, threadno));
                        break;
                    case 4:
                        bus.Subscribe<XMLMessage4>("XML_subscriber4", message => outputtoconsole(message.XMLString, counter++, threadno));
                        break;
                    case 5:
                        bus.Subscribe<XMLMessage5>("XML_subscriber5", message => outputtoconsole(message.XMLString, counter++, threadno));
                        break;
                    case 6:
                        bus.Subscribe<XMLMessage6>("XML_subscriber6", message => outputtoconsole(message.XMLString, counter++, threadno));
                        break;
                    case 7:
                        bus.Subscribe<XMLMessage7>("XML_subscriber7", message => outputtoconsole(message.XMLString, counter++, threadno));
                        break;
                    case 8:
                        bus.Subscribe<XMLMessage8>("XML_subscriber8", message => outputtoconsole(message.XMLString, counter++, threadno));
                        break;
                    case 9:
                        bus.Subscribe<XMLMessage9>("XML_subscriber9", message => outputtoconsole(message.XMLString, counter++, threadno));
                        break;
                }

                while (!token.IsCancellationRequested)
                {
                    //Wait until cancel signal is sent.
                    Thread.Sleep(500);
                }
                Console.WriteLine("Thread {0} stopped", threadno);
            }
        }

        static void outputtoconsole(string message, int counter,string threadno)
        {
            if (counter % 100 == 0) 
                Console.WriteLine(threadno + "-" + counter.ToString());

        }
    }
}
