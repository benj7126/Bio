using Bio.Purchasables.Plasmid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Bio.Purchasables.PlayerUpgrades
{
    public abstract class MaximumPlasmids : PurchasableUpgrade
    {
        public override string InternalName => "Plasmid Capacity";
        public override string Texture => "";
        public override string Description => "+1 Max Plasmid";
        public override void OnPurchase(Player player)
        {
            CorePlayer corePlayer = player.GetModPlayer<CorePlayer>();
            corePlayer.maxPlasmids++;
        }
    }

    public class BuyMaximumPlasmids1 : MaximumPlasmids
    {
        public override int Stage => 1;
        public override int Cost => 100;
        public override bool ProgressionLock => NPC.downedBoss3; // Skeletron
        public override string UnlockCondition => "Defeat Skeletron";
    }
    public class BuyMaximumPlasmids2 : MaximumPlasmids
    {
        public override int Stage => 2;
        public override int Cost => 200;
        public override bool ProgressionLock => Main.hardMode; // Skeletron
        public override string UnlockCondition => "Enter Hardmode";
    }
    public class BuyMaximumPlasmids3 : MaximumPlasmids
    {
        public override int Stage => 3;
        public override int Cost => 400;
        public override bool ProgressionLock => NPC.downedMoonlord;
        public override string UnlockCondition => "Defeat Moonlord";
    }
}
