using AmongUs.GameOptions;
using System.Linq;
using TONEX.Roles.Core;
using TONEX.Roles.Core.Interfaces;
using TONEX.Roles.Core.Interfaces.GroupAndRole;

namespace TONEX.Roles.Neutral;

public sealed class Hater : RoleBase, IAdditionalWinner, INeutralKiller
{
    public static readonly SimpleRoleInfo RoleInfo =
       SimpleRoleInfo.Create(
            typeof(Hater),
            player => new Hater(player),
            CustomRoles.Hater,
            () => RoleTypes.Impostor,
            CustomRoleTypes.Neutral,
            51700,
            null,
            "ht|fff團|fff|fff团",
            "#414b66",
            true
        );
    public Hater(PlayerControl player)
    : base(
        RoleInfo,
        player,
        () => HasTask.False
    )
    { }

    public float CalculateKillCooldown() => 0f; 
    public bool IsNE { get; private set; } = false;
    public bool IsNK { get; private set; } = true;
    public bool CanUseSabotageButton() => false;
    public bool CanUseImpostorVentButton() => false;
    public override void ApplyGameOptions(IGameOptions opt) => opt.SetVision(true);

    public SchrodingerCat.TeamType SchrodingerCatChangeTo => SchrodingerCat.TeamType.Hater;

    public bool OverrideKillButtonText(out string text)
    {
        text = Translator.GetString("HaterButtonText");
        return true;
    }
    public void OnMurderPlayerAsKiller(MurderInfo info)
    {
        (var killer, var target) = info.AttemptTuple;
        if (Is(killer) && !info.IsSuicide && !target.Is(CustomRoles.Lovers) && !target.Is(CustomRoles.Neptune) && !target.Is(CustomRoles.Admirer) && !target.Is(CustomRoles.AdmirerLovers) && !target.Is(CustomRoles.Akujo) && !target.Is(CustomRoles.AkujoLovers) && !target.Is(CustomRoles.Cupid)&& !target.Is(CustomRoles.CupidLovers) && !target.Is(CustomRoles.Yandere))
        {
            killer.RpcMurderPlayer(killer);
            PlayerState.GetByPlayerId(killer.PlayerId).DeathReason = CustomDeathReason.Sacrifice;
            Logger.Info($"{killer.GetRealName()} 击杀了非目标玩家，壮烈牺牲了（bushi）", "FFF");
            return;
        }
    }
    public bool CheckWin(ref CustomRoles winnerRole , ref CountTypes winnerCountType)
    {
        return CustomWinnerHolder.WinnerTeam != CustomWinner.Lovers
            && !CustomWinnerHolder.AdditionalWinnerRoles.Contains(CustomRoles.Lovers)
            && !CustomRoles.Lovers.IsExist()
            && !CustomRoles.AdmirerLovers.IsExist()
            && !CustomRoles.AkujoLovers.IsExist()
            && !CustomRoles.CupidLovers.IsExist()
            && !CustomRoles.Admirer.IsExist()
            && !CustomRoles.Akujo.IsExist()
            && !CustomRoles.Cupid.IsExist()
            && !CustomRoles.Yandere.IsExist()
            && !CustomRoles.Neptune.IsExist()
            && Main.AllPlayerControls.Any(p => (p.GetCustomRole() is CustomRoles.Akujo or CustomRoles.Admirer or CustomRoles.Cupid or CustomRoles.Yandere 
            || p.GetCustomSubRoles().Contains(CustomRoles.Lovers) || p.GetCustomSubRoles().Contains(CustomRoles.AdmirerLovers) || p.GetCustomSubRoles().Contains(CustomRoles.AkujoLovers) 
            || p.GetCustomSubRoles().Contains(CustomRoles.CupidLovers) || p.GetCustomSubRoles().Contains(CustomRoles.Neptune)) && Is(p.GetRealKiller()));
    }
}
