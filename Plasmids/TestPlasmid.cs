using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.CompilerServices.SymbolWriter;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Bio.Plasmids
{
    public class TestPlasmid : Plasmid
    {
        public override string Name => "Test";
        public override string Description => "Test";
        public override string Texture => "Bio/eve";
        public override float InitialCost => 200f;
        public override void Activate(Player player)
        {
            Console.WriteLine("Activated...");
        }
        public override void Deactivate()
        {

        }
    }
}
