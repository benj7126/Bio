using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Bio
{
    public abstract class Purchasable
    {
        public abstract string InternalName { get; }
        public abstract string Texture { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract int Cost { get; }
        public abstract int Stage { get; }
        public abstract bool ReapplyOnLoad { get; }
        public abstract void OnPurchase(Player player);
        public void Purchase(Player player, bool isLoad = false)
        {
            CorePlayer corePlayer = player.GetModPlayer<CorePlayer>();

            if (!corePlayer.PurchasedFromShop.ContainsKey(InternalName))
                corePlayer.PurchasedFromShop.Add(InternalName, 0);

            if (corePlayer.PurchasedFromShop[InternalName] > Stage)
                Console.Error.WriteLine("There already exists an instance of " + InternalName + " at " + corePlayer.PurchasedFromShop[InternalName] + ", higher than " + Stage);

            corePlayer.PurchasedFromShop[InternalName] = Stage;

            if (!ReapplyOnLoad && isLoad)
                return;

            OnPurchase(player);
        }
        public virtual bool ProgressionLock => true;
        public virtual string UnlockCondition => "";

        private static Texture2D Pixel = ModContent.Request<Texture2D>("Bio/WhitePixel", AssetRequestMode.ImmediateLoad).Value;
        public void DrawDesc(SpriteBatch spriteBatch, Vector2 mousePos)
        {
            mousePos += new Vector2(20f, 20f);
            Vector2 DescSize = Vector2.Zero;

            Vector2 Border = new Vector2(6f, 6f);

            float extraLineWidth = 0;
            Color baseColor = Color.White;

            int X = (int)MathF.Ceiling(mousePos.X + Border.X);
            int Y = (int)MathF.Ceiling(mousePos.Y + Border.Y);

            Vector2 NameDimensions = ChatManager.GetStringSize(FontAssets.MouseText.Value, Name, Vector2.One, -1f);
            float NameHeight = NameDimensions.Y * 1.2f;

            string description = Description;

            if (!ProgressionLock)
                description += "\nLocked: [" + UnlockCondition + "]";

            string[] DescLines = description.Split("\n");

            foreach (string line in DescLines)
            {
                Vector2 stringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, line, Vector2.One, -1f);
                if (stringSize.X > DescSize.X)
                {
                    DescSize.X = stringSize.X;
                }
                DescSize.Y += stringSize.Y + extraLineWidth;
            }

            if (DescSize.X < NameDimensions.X) DescSize.X = NameDimensions.X;
            DescSize.Y += NameHeight;
            DescSize += Border * 2f;

            Rectangle DescBG = new Rectangle((int)MathF.Ceiling(mousePos.X), (int)MathF.Ceiling(mousePos.Y), (int)MathF.Ceiling(DescSize.X), (int)MathF.Ceiling(DescSize.Y));

            spriteBatch.Draw(Pixel, DescBG, Color.Orange);

            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, Name, new Vector2(X, Y), baseColor, 0f, Vector2.Zero, Vector2.One, -1f, 2f);

            Y += (int)MathF.Ceiling(NameHeight);

            foreach (string line in DescLines)
            {
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, line, new Vector2(X, Y), baseColor, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
                Y += (int)(FontAssets.MouseText.Value.MeasureString(line).Y + extraLineWidth);
            }
        }
    }
}
