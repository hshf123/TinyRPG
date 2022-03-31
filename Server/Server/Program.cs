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

        static void FlushRoom()
        {
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        static void SceneAdd()
        {
            SceneManager.Instance.Add(()=> { return new Lobby(); });
            SceneManager.Instance.Add(()=> { return new Huntingground(); });
        }

        static void SceneUpdate(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                SceneManager.Instance.Find(i).Update();
            }
        }

        static void Main(string[] args)
        {
            ConfigManager.LoadConfig();
            DataManager.Init();
            SceneAdd();

			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine("Listening...");

			//FlushRoom();
			JobTimer.Instance.Push(FlushRoom);

			while (true)
			{
				// JobTimer.Instance.Flush();
				SceneUpdate(2);
			}
		}
	}
}
