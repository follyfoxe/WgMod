using Terraria;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs.Consumables;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class GnomeLuck : ModBuff
{
    WgStat _luck = new(0.2f, 1f);
    private string _tooltip;

    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        if (!player.TryGetModPlayer(out GnomeLuckPlayer gl))
            return;
        float immobility = wg.Weight.ClampedImmobility;
        _luck.Lerp(immobility);

        gl._active = true;
        gl._luckModifier = _luck;

        if (_luck < 0.4f)
            _tooltip = "Slightly increases luck"; // Don't work
        if (_luck > 0.8f)
            _tooltip = "Greatly increases luck";
        else
            _tooltip = "Increases luck";
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        tip = base.Description.Format(_tooltip);
    }
}

public class GnomeLuckPlayer : ModPlayer
{
    public bool _active;
    public int _luckModifier;

    public override void ResetEffects()
    {
        _active = false;
    }

    public override void ModifyLuck(ref float luck) // Unsure of if this works, hard to test
    {
        if (!_active)
            return;

        luck += _luckModifier;
    }
}
