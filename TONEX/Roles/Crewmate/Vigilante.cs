﻿using AmongUs.GameOptions;
using Hazel;
using TONEX.Roles.Core;
using TONEX.Roles.Core.Interfaces.GroupAndRole;
using TONEX.Roles.Neutral;
using UnityEngine;

namespace TONEX.Roles.Crewmate;
public sealed class Vigilante : RoleBase, IKiller, ISchrodingerCatOwner
{
    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Vigilante),
            player => new Vigilante(player),
            CustomRoles.Vigilante,
            () => RoleTypes.Impostor,
            CustomRoleTypes.Crewmate,
            21400,
            null,
            "vi|俠客",
            "#C90000",
            true,
            introSound: () => GetIntroSound(RoleTypes.Crewmate)
        );
    public Vigilante(PlayerControl player)
    : base(
        RoleInfo,
        player,
        () => HasTask.False
    )
    { }
    public SchrodingerCat.TeamType SchrodingerCatChangeTo => SchrodingerCat.TeamType.Crew;

    private bool IsKilled;
    public override void Add()
    {
        var playerId = Player.PlayerId;
        IsKilled = false;
    }
    private void SendRPC()
    {
        using var sender = CreateSender();
        sender.Writer.Write(IsKilled);
    }
    public override void ReceiveRPC(MessageReader reader)
    {
        
        IsKilled = reader.ReadBoolean();
    }
    public float CalculateKillCooldown() => CanUseKillButton() ? 0f : 255f;
    public bool CanUseKillButton() => Player.IsAlive() && !IsKilled;
    public bool CanUseSabotageButton() => false;
    public bool CanUseImpostorVentButton() => false;
    public override void ApplyGameOptions(IGameOptions opt) => opt.SetVision(false);
    public bool OnCheckMurderAsKiller(MurderInfo info)
    {
        if (Is(info.AttemptKiller) && !info.IsSuicide)
        {
            if (IsKilled) return false;
            IsKilled = true;
            SendRPC();
            Player.ResetKillCooldown();
        }
        return true;
    }
    public override string GetProgressText(bool comms = false) => Utils.ColorString(CanUseKillButton() ? Utils.GetRoleColor(CustomRoles.Vigilante) : Color.gray, $"({(CanUseKillButton() ? 1 : 0)})");
}