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

        Console.WriteLine($"Player : {clientSession.MyPlayer.Info.ObjectId} Move({movePacket.PosInfo.PosX}, {movePacket.PosInfo.PosY})");

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        Scenes scene = player.Scene;
        if (scene == null)
            return;

        scene.HandleMove(player, movePacket);
    }

    public static void C_SkillHandler(PacketSession session, IMessage packet)
    {
        C_Skill skillPacket = packet as C_Skill;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        Scenes scene = player.Scene;
        if (scene == null)
            return;

        scene.HandleSkill(player, skillPacket);
    }

    public static void C_PortalLoadHandler(PacketSession session, IMessage packet)
    {
        C_PortalLoad loadPacket = packet as C_PortalLoad;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        Scenes scene = player.Scene;
        if (scene == null)
            return;

        scene.LoadScene(player, loadPacket);
    }

    public static void C_PortalHandler(PacketSession session, IMessage packet)
    {
        C_Portal portalPacket = packet as C_Portal;
        ClientSession clientSession = session as ClientSession;

        Player player = clientSession.MyPlayer;
        if (player == null)
            return;

        Scenes scene = player.Scene;
        if (scene == null)
            return;

        scene.PortalScene(player, portalPacket);
    }
}
