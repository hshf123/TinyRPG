using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;

		Console.WriteLine($"Player : {clientSession.MyPlayer.Info.PlayerId} Move({movePacket.PosInfo.PosX}, {movePacket.PosInfo.PosY})");

		if (clientSession.MyPlayer == null)
			return;
		if (clientSession.MyPlayer.Scene == null)
			return;

		// TODO : 검증 (클라가 해킹되어서 잘못된 정보로 들어올 수도 있으므로)

		// 일단 서버에서 좌표이동(아직 클라에서는 이동X)
		PlayerInfo info = clientSession.MyPlayer.Info;
		info.PosInfo = movePacket.PosInfo;

		// 플레이어가 속한 씬에있는 모든 유저에게 브로드캐스트
		S_Move resMovePacket = new S_Move();
		resMovePacket.PlayerId = info.PlayerId;
		resMovePacket.PosInfo = info.PosInfo;
		clientSession.MyPlayer.Scene.Broadcast(resMovePacket);
	}
}
