﻿using Microsoft.Xna.Framework;
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
    public class BeeSwarm2 : Plasmid
    {
        public override string Name => "BeeSwarm 2.0";
        public override string Description => "Spawns two bees every frame";
        public override string Texture => "Bio/eve";
        public override float ConstantCost => 0.5f;

        public override void Update(Player player)
        {
            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center + new Vector2(0, -60) + Main.rand.NextVector2Circular(40, 40), new Vector2(1 * player.direction, 0), 181, 10, 0f, player.whoAmI);
            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center + new Vector2(0, -60) + Main.rand.NextVector2Circular(40, 40), new Vector2(1 * player.direction, 0), 181, 10, 0f, player.whoAmI);
        }

        public override void Activate(Player player)
        {
            Console.WriteLine("Activate");
        }

        public override void Deactivate()
        {
            Console.WriteLine("Deactivate");
        }
    }
}
