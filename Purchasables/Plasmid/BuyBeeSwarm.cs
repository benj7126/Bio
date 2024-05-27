using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bio.Purchasables.Plasmid
{
    public abstract class BaseBuyBeeSwarm : PurchasablePlasmid
    {
        public override string InternalName => "BeeSwarm";
    }

    public class BuyBeeSwarm1 : BaseBuyBeeSwarm
    {
        public override string AddPlasmid => "BeeSwarm";
        public override int Stage => 1;
        public override int Cost => 10;
    }
    public class BuyBeeSwarm2 : BaseBuyBeeSwarm
    {
        public override string RemovePlasmid => "BeeSwarm";
        public override string AddPlasmid => "BeeSwarm 2.0";
        public override int Stage => 2;
        public override int Cost => 100;
    }
}
