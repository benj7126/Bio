using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bio.Purchasables.Plasmid
{
    public abstract class BaseBuyTest : PurchasablePlasmid
    {
        public override string InternalName => "Test";
    }

    public class BuyTest1 : BaseBuyTest
    {
        public override string AddPlasmid => "Test";
        public override int Stage => 1;
        public override int Cost => 20;
    }
}
