using Bio.Purchasables.Plasmid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Bio.Purchasables.PlayerUpgrades
{
    public abstract class EveRegen : PurchasableUpgrade
    {
        public override string InternalName => "Eve Regen";
        public override string Texture => "";
    }

    public class BuyEveRegen1 : EveRegen
    {
        public override int Stage => 1;
        public override int Cost => 10;
        public override string Description => "Increase regen by 4 / second";
        public override void OnPurchase(Player player)
        {
            CorePlayer corePlayer = player.GetModPlayer<CorePlayer>();
            corePlayer.eveRegen += 4 / 60; // 4 / 60 frams - 60 frames = 1 sec
        }
    }
    public class BuyEveRegen2 : EveRegen
    {
        public override int Stage => 2;
        public override int Cost => 40;
        public override string Description => "Increase regen by 12 / second";
        public override void OnPurchase(Player player)
        {
            CorePlayer corePlayer = player.GetModPlayer<CorePlayer>();
            corePlayer.eveRegen += 12 / 60; // 4 / 60 frams - 60 frames = 1 sec
        }
    }
    public class BuyEveRegen3 : EveRegen
    {
        public override int Stage => 3;
        public override int Cost => 200;
        public override string Description => "Double regen.";
        public override void OnPurchase(Player player)
        {
            CorePlayer corePlayer = player.GetModPlayer<CorePlayer>();
            corePlayer.eveRegenMulti *= 2f;
        }
    }
}
