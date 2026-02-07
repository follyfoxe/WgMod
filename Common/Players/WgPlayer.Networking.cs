using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WgMod.Common.Players;

public partial class WgPlayer
{
    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)WgMod.MessageType.WgPlayerSync);
        packet.Write((byte)Player.whoAmI);
        packet.Write(Weight.Mass);
        packet.Send(toWho, fromWho);
    }

    public void ReceivePlayerSync(BinaryReader reader)
    {
        SetWeight(new Weight(reader.ReadSingle()));
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        WgPlayer clone = (WgPlayer)targetCopy;
        clone.SetWeight(Weight, false);
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        WgPlayer clone = (WgPlayer)clientPlayer;
        if (Weight != clone.Weight)
            SyncPlayer(-1, Main.myPlayer, false);
    }

    // Saving
    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet("Weight", out float w))
            SetWeight(new Weight(w), false);
        else
            SetWeight(Weight.Base, false);
    }

    public override void SaveData(TagCompound tag)
    {
        tag["Weight"] = Weight.Mass;
    }
}
