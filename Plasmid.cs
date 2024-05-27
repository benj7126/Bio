using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.CompilerServices.SymbolWriter;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Bio
{
    public abstract class Plasmid
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Texture { get; }
        public virtual float InitialCost => 0f;
        public virtual float ConstantCost => 0f;
        public virtual bool HasContinuousActivation => true;
        public virtual void Activate(Player player) { }
        public virtual void Deactivate() { }
        public virtual void Update(Player player) { }
    }
}