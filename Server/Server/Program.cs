using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Data;
using Server.Game;
using ServerCore;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();

        static void TickRoom(Scenes scene, int tick  = 100)
        {
            var timer = new System.Timers.Timer();
            timer.Interval = tick;
            timer.Elapsed += ((s, e) => { scene.Update(); });
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        static List<Scenes> SceneAdd()
        {
            List<Scenes> scenes = new List<Scenes>();

            scenes.Add(SceneManager.Instance.Add(() => { return new Lobby(); }));
            scenes.Add(SceneManager.Instance.Add(() => { return new Huntingground(); }));

            return scenes;
        }

        static void Main(string[] args)
        {
            ConfigManager.LoadConfig();
            DataManager.Init();
            List<Scenes> scenes = SceneAdd();
            foreach(Scenes scene in scenes)
            {
                TickRoom(scene, 50);
            }

            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("연결 대기중...");

            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
