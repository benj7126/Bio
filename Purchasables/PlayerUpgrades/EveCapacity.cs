using Bio.Purchasables.Plasmid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Bio.Purchasables.PlayerUpgrades
{
    public abstract class EveCapacity : PurchasableUpgrade
    {
        public override string InternalName => "Eve Capacity";
        public override string Texture => "";
    }

    public class BuyEveCapacity1 : EveCapacity
    {
        public override int Stage => 1;
        public override int Cost => 10;
        public override string Description => "Add 50 addition eve capacity";
        public override void OnPurchase(Player player)
        {
            CorePlayer corePlayer = player.GetModPlayer<CorePlayer>();
            corePlayer.maxEve += 50; // 4 / 60 frams - 60 frames = 1 sec
        }
    }
    public class BuyEveCapacity2 : EveCapacity
    {
        public override int Stage => 2;
        public override int Cost => 40;
        public override string Description => "Add 100 addition eve capacity";
        public override void OnPurchase(Player player)
        {
            CorePlayer corePlayer = player.GetModPlayer<CorePlayer>();
            corePlayer.maxEve += 50; // 4 / 60 frams - 60 frames = 1 sec
        }
    }
    public class BuyEveCapacity3 : EveCapacity
    {
        public override int Stage => 3;
        public override int Cost => 200;
        public override string Description => "Double eve capacity";
        public override void OnPurchase(Player player)
        {
            CorePlayer corePlayer = player.GetModPlayer<CorePlayer>();
            corePlayer.maxEveMulti *= 2f;
        }
    }
}
