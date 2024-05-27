using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Bio.Purchasables
{
    public abstract class PurchasablePlasmid : Purchasable
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
        public override string Description
        {
            get
            {
                if (RemovePlasmid == "")
                {
                    return "Purchase " + Core.Plasmids[AddPlasmid].Name + ":\n" +
                        Core.Plasmids[AddPlasmid].Description;
                }
                else
                {
                    return "Upgrades " + Core.Plasmids[RemovePlasmid].Name + " to " + Core.Plasmids[AddPlasmid].Name + ":\n" +
                        Core.Plasmids[AddPlasmid].Description;
                }
            }
        }
        public override string Texture => Core.Plasmids[AddPlasmid].Texture; // use the texture of the plasmid you are buying.
        public virtual string RemovePlasmid => "";
        public abstract string AddPlasmid { get; }
        override public bool ReapplyOnLoad => false; // this is saved sepperaetly
        override public void OnPurchase(Player player)
        {
            CorePlayer corePlayer = player.GetModPlayer<CorePlayer>();

            int at = corePlayer.PlasmidInventory.Count;

            if (RemovePlasmid != "")
            {
                at = corePlayer.PlasmidInventory.IndexOf(RemovePlasmid);
                corePlayer.PlasmidInventory.Remove(RemovePlasmid);
            }

            corePlayer.PlasmidInventory.Insert(at, AddPlasmid);
        }
    }
}
