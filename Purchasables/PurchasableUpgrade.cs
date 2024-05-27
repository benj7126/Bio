using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Bio.Purchasables.Plasmid
{
    public abstract class PurchasableUpgrade : Purchasable
    {
        public override string Name
        {
            get
            {
                string stage = " ";
                for (int i = 0; i < Stage; i++)
                    stage += "I";

                return InternalName + (Stage == 1 ? "" : stage);
            }
        }
        override public bool ReapplyOnLoad => true;
    }
}
