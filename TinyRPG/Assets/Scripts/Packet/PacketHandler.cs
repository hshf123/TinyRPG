using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class PacketHandler
{
	public static void S_ChatHandler(PacketSession session, IMessage packet)
	{
		S_Chat chatPacket = packet as S_Chat;
		ServerSession serverSession = session as ServerSession;

		Debug.Log(chatPacket.Context);
	}
}
