using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S_EnterGame enterPacket = packet as S_EnterGame;
        Managers.Object.Add(enterPacket.Player, myPlayer: true);
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leavePacket = packet as S_LeaveGame;
        Managers.Object.Clear();
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;

        foreach (ObjectInfo obj in spawnPacket.Objects)
        {
            Managers.Object.Add(obj, myPlayer: false);
        }
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;

        foreach (int id in despawnPacket.ObjectIds)
        {
            Managers.Object.Remove(id);
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.Find(movePacket.ObjectId);
        if (go == null)
            return;

        if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
            return;

        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return;

        bc.PosInfo = movePacket.PosInfo;
    }

    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
        S_Skill skillPacket = packet as S_Skill;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.Find(skillPacket.ObjectId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.UseSkill(skillPacket.Info.SkillId);
        }
    }

    public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeHp hpPacket = packet as S_ChangeHp;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.Find(hpPacket.ObjectId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            cc.Hp = hpPacket.Hp; // Stat.Hp가 아니라 Hp를 건드리면서 UpdateHp호출
        }
    }

    public static void S_DieHandler(PacketSession session, IMessage packet)
    {
        S_Die hpPacket = packet as S_Die;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.Find(hpPacket.ObjectId);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc != null)
        {
            // TODO : 나를 죽인사람 정보에 대한 처리를 할까말까

            cc.OnDead();
        }
    }

    public static void S_PortalLoadHandler(PacketSession session, IMessage packet)
    {
        S_PortalLoad loadPacket = packet as S_PortalLoad;
        ServerSession serverSession = session as ServerSession;

        GameObject go = Managers.Object.Find(loadPacket.PlayerId);
        if (go == null)
            return;

        MyPlayerController mc = go.GetComponent<MyPlayerController>();
        if (mc != null)
        {
            mc.PortalLoad(loadPacket.HopeScene);
        }
    }
}
