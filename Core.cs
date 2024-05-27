using Microsoft.Build.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.UI.Chat;
using static Terraria.GameContent.Animations.IL_Actions.NPCs;

namespace Bio
{
    public class Core
    {
        static public void LoadAll()
        {
            LoadPlasmids();
            LoadPurchables();
        }

        static public Dictionary<string, Plasmid> Plasmids = new Dictionary<string, Plasmid>();
        static public void LoadPlasmids()
        {
            Plasmids.Clear();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.BaseType == typeof(Plasmid))
                {
                    Plasmid plasmid = (Plasmid)Activator.CreateInstance(type);
                    Plasmids.Add(plasmid.Name, plasmid);
                }
            }
        }

        static public Dictionary<KeyValuePair<string, int>, Purchasable> Purchables = new Dictionary<KeyValuePair<string, int>, Purchasable>();
        // <Name, Stage>
        static public List<string> AllPurchables = new List<string>();
        static public Dictionary<string, int> AllMaxStage = new Dictionary<string, int>();

        static public void LoadPurchables()
        {
            Purchables.Clear();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(Purchasable)) && !type.IsAbstract)
                {
                    Purchasable purchasable = (Purchasable)Activator.CreateInstance(type);
                    Purchables.Add(new (purchasable.InternalName, purchasable.Stage), purchasable);

                    if (!AllPurchables.Contains(purchasable.InternalName))
                        AllPurchables.Add(purchasable.InternalName);

                    if (!AllMaxStage.ContainsKey(purchasable.InternalName))
                        AllMaxStage.Add(purchasable.InternalName, 0);

                    AllMaxStage[purchasable.InternalName] = Math.Max(purchasable.Stage, AllMaxStage[purchasable.InternalName]);
                }
            }
        }

    }
    public class CorePlayer : ModPlayer
    {

        public float eve = 0;
        public float maxEve = 100f;
        public float maxEveMulti = 1f;
        public float eveRegen = 0.2f;
        public float eveRegenMulti = 1f;

        public int adam = 1000;

        private bool active = false;

        private bool rightState = false;

        public List<string> PlasmidInventory = new List<string>() { };
        public int[] equippedPlasmids = new int[6] { 0, 1, 2, -1, -1, -1 }; // idk max amount...
        public int maxPlasmids = 3;
        private int activePlasmidIdx = 0;
        public Plasmid ActivePlasmid { get { return equippedPlasmids[activePlasmidIdx] < 0 ? null : equippedPlasmids[activePlasmidIdx] >= PlasmidInventory.Count ? null : Core.Plasmids[PlasmidInventory[equippedPlasmids[activePlasmidIdx]]]; } }

        public Dictionary<string, int> PurchasedFromShop = new Dictionary<string, int>();

        public List<Purchasable> Shop = new List<Purchasable>();
        public bool UIOpen = true;
        public override void SetControls()
        {
            if (active)
            {
                // stop swapping item while plasmid is active
                PlayerInput.Triggers.Current.Hotbar1 = false;
                PlayerInput.Triggers.Current.Hotbar2 = false;
                PlayerInput.Triggers.Current.Hotbar3 = false;
                PlayerInput.Triggers.Current.Hotbar4 = false;
                PlayerInput.Triggers.Current.Hotbar5 = false;
                PlayerInput.Triggers.Current.Hotbar6 = false;
                PlayerInput.Triggers.Current.Hotbar7 = false;
                PlayerInput.Triggers.Current.Hotbar8 = false;
                PlayerInput.Triggers.Current.Hotbar9 = false;
                PlayerInput.Triggers.Current.Hotbar10 = false;
            }
        }
        public override void OnEnterWorld()
        {
            GenerateShop();
        }
        public void GenerateShop()
        {
            // when locking by content is added, add a condition that placs them at the bottom of the shop.

            Shop.Clear();

            List<KeyValuePair<Purchasable, int>> ShopItems = new List<KeyValuePair<Purchasable, int>>();
            List<KeyValuePair<Purchasable, int>> LockedShopItems = new List<KeyValuePair<Purchasable, int>>();
            foreach (string pString in Core.AllPurchables)
            {
                if (PurchasedFromShop.ContainsKey(pString) && PurchasedFromShop[pString] >= Core.AllMaxStage[pString])
                    continue; // already has it maxed

                if (PurchasedFromShop.ContainsKey(pString)) // has one of the upgrades
                {
                    Purchasable p = Core.Purchables[new (pString, PurchasedFromShop[pString] + 1)];

                    if (p.ProgressionLock)
                        ShopItems.Add(new(p, p.Cost));
                    else
                        LockedShopItems.Add(new(p, p.Cost));
                }
                else
                {
                    Purchasable p = Core.Purchables[new(pString, 1)];

                    if (p.ProgressionLock)
                        ShopItems.Add(new(p, p.Cost));
                    else
                        LockedShopItems.Add(new(p, p.Cost));
                }
            }

            ShopItems.Sort((a, b) => a.Value - b.Value);
            LockedShopItems.Sort((a, b) => a.Value - b.Value);

            ShopItems.AddRange(LockedShopItems);

            foreach (KeyValuePair<Purchasable, int> ShopItem in ShopItems)
            {
                Shop.Add(ShopItem.Key);
            }
        }

        private bool HoldingWeapon()
        {
            Item i = Player.HeldItem;
            return i.damage > 0 &&
                i.pick <= 0 &&
                i.axe <= 0 &&
                i.hammer <= 0 &&
                i.ammo == Terraria.ID.AmmoID.None &&
                i.createTile == -1 && 
                i.createWall == -1 &&
                Player.selectedItem != 58;
        }
        private bool Within(Rectangle r, Vector2 point)
        {
            return point.X > r.Left && point.Y > r.Top && point.X < r.Right && point.Y < r.Bottom;
        }

        int cycle = 0;
        public override void PreUpdate()
        {
            if (UIOpen && Within(new Rectangle(400, Main.screenHeight - 400, 200, 10000), new Vector2(Main.mouseX, Main.mouseY)))
            {
                Player.mouseInterface = true;
            }

            if (!active)
                cycle++;
            if (cycle == 20){
                cycle = 0;
                if (PlasmidInventory.Count > 0)
                {
                    do
                    {
                        activePlasmidIdx++;
                        if (activePlasmidIdx == equippedPlasmids.Length)
                            activePlasmidIdx = 0;
                    } while (ActivePlasmid == null);
                }
            }
            /*
            GenerateShop();
            Console.WriteLine(":---- Begin ----:");
            Console.WriteLine("Shop: ");
            foreach (Purchasable p in Shop)
            {
                Console.WriteLine(p.Name);
            }
            Console.WriteLine("PlasmidInventory: ");
            foreach (string p in PlasmidInventory)
            {
                Console.WriteLine(p);
            }
            Console.WriteLine(":---- End ----:");

            if (stage != 2)
            {
                stage++;
                Shop[0].Purchase(Player);
            }
            */

            float newEve = eve + eveRegen * eveRegenMulti;
            eve = newEve > maxEve * maxEveMulti ? maxEve * maxEveMulti : newEve;

            if (ActivePlasmid != null)
            {
                if (HoldingWeapon() && rightState == false && Main.mouseRight && eve >= ActivePlasmid.InitialCost && !Player.mouseInterface)
                {
                    eve -= ActivePlasmid.InitialCost;
                    ActivePlasmid.Activate(Player);
                    active = ActivePlasmid.HasContinuousActivation;
                }
                else if (active && Main.mouseRightRelease || Player.mouseInterface)
                {
                    ActivePlasmid.Deactivate();
                    active = false;
                }

                if (active)
                {
                    if (eve >= ActivePlasmid.ConstantCost)
                    {
                        eve -= ActivePlasmid.ConstantCost;
                        ActivePlasmid.Update(Player);
                    }
                    else
                    {
                        active = false;
                        ActivePlasmid.Deactivate();
                    }
                }
            }

            rightState = Main.mouseRight;
        }

        public override void SaveData(TagCompound tag)
        {
            foreach (KeyValuePair<string, int> ShopItem in PurchasedFromShop)
            {
                tag.Add(ShopItem.Key, ShopItem.Value);
            }

            TagCompound Inventory = new TagCompound();
            Inventory.Add("Size", PlasmidInventory.Count);
            for (int i = 0; i < PlasmidInventory.Count; i++)
            {
                string plasmid = PlasmidInventory[i];
                Inventory.Add(i.ToString(), plasmid);
            }
            tag.Add("Inventory", Inventory);
            
            for (int i = 0; i < equippedPlasmids.Length; i++)
            {
                tag.Add("Equip" + i, equippedPlasmids[i]);
            }
        }
        public override void LoadData(TagCompound tag)
        {
            foreach (string ShopItem in Core.AllPurchables)
            {
                if (tag.ContainsKey(ShopItem))
                {
                    int stage = tag.Get<int>(ShopItem);
                    
                    for (int i = 1; i < stage+1; i++)
                    {
                        if (Core.Purchables[new (ShopItem, i)].ReapplyOnLoad)
                        {
                            Core.Purchables[new(ShopItem, i)].Purchase(Player, true);
                        }
                    }
                }
            }

            if (tag.ContainsKey("Inventory"))
            {
                TagCompound Inventory = tag.Get<TagCompound>("Inventory");
                int size = Inventory.Get<int>("Size");
                for (int i = 0; i < size; i++)
                {
                    PlasmidInventory.Add(Inventory.Get<string>(i.ToString()));
                }
            }

            for (int i = 0; i < equippedPlasmids.Length; i++)
            {
                if (tag.ContainsKey("Equip" + i))
                    equippedPlasmids[i] = tag.Get<int>("Equip" + i);
            }
        }
    }


    public class CoreSystem : ModSystem
    {
        private bool Within(Rectangle r, Vector2 point)
        {
            return point.X > r.Left && point.Y > r.Top && point.X < r.Right && point.Y < r.Bottom;
        }

        private bool leftState = false;
        public override void Load()
        {
            Core.LoadAll();
        }

        public override void PreUpdatePlayers()
        {            
            Player player = Main.LocalPlayer;
            CorePlayer cPlayer = player.GetModPlayer<CorePlayer>();

            if (leftState == false && Main.mouseLeft)
            {
                for (int ip = 0; ip < cPlayer.Shop.Count; ip++)
                {
                    if (Within(new Rectangle(400, Main.screenHeight - 400 + 24 * ip, 200, 20), new Vector2(Main.mouseX, Main.mouseY)))
                    {
                        Purchasable p = cPlayer.Shop[ip];
                        if (p.Cost <= cPlayer.adam && p.ProgressionLock)
                        {
                            cPlayer.adam -= p.Cost;
                            p.Purchase(player);
                        }
                        cPlayer.GenerateShop();
                    }
                }

                leftState = true;
                return;
            }

            leftState = Main.mouseLeft;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "Bio: Interface",
                    delegate
                    {
                        DrawInterface();
                        return true;
                    },
                    InterfaceScaleType.UI)
                );

                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "Bio: Dispenser",
                    delegate
                    {
                        DrawDispenser();
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        //private static Texture2D eve = ModContent.Request<Texture2D>("Bio/eve", AssetRequestMode.ImmediateLoad).Value;
        //private static Effect HPCuter = ModContent.Request<Effect>("Bio/Effects/HPCutter", AssetRequestMode.ImmediateLoad).Value;
        private static Texture2D Pixel = ModContent.Request<Texture2D>("Bio/WhitePixel", AssetRequestMode.ImmediateLoad).Value;

        private void DrawInterface()
        {
            Player player = Main.LocalPlayer;
            CorePlayer cPlayer = player.GetModPlayer<CorePlayer>();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, DepthStencilState.None, null, null, Main.UIScaleMatrix);
            // start shader for hp cut-off ^^

            // Draw hp...

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, DepthStencilState.None, null, null, Main.UIScaleMatrix);

            // draw ui thingys

            Main.spriteBatch.Draw(Pixel, new Vector2(100, Main.screenHeight - 120), new Rectangle(0, 0, 100, 20), Color.Gray);
            Main.spriteBatch.Draw(Pixel, new Vector2(102, Main.screenHeight - 118), new Rectangle(0, 0, (int)(96f * cPlayer.eve / (cPlayer.maxEve * cPlayer.maxEveMulti)), 16), Color.White);

            if (cPlayer.ActivePlasmid != null)
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, cPlayer.ActivePlasmid.Name, new Vector2(100, Main.screenHeight - 150), Color.White, 0f, Vector2.Zero, Vector2.One);

        }
        private void DrawDispenser()
        {
            Player player = Main.LocalPlayer;
            CorePlayer cPlayer = player.GetModPlayer<CorePlayer>();

            if (!cPlayer.UIOpen)
                return;

            Vector2 mousePosition = new Vector2(Main.mouseX, Main.mouseY);

            Purchasable drawDesc = null;

            for (int ip = 0; ip < cPlayer.Shop.Count; ip++)
            {
                Purchasable p = cPlayer.Shop[ip];
                Color c = Color.Gray;
                if (Within(new Rectangle(400, Main.screenHeight - 400 + 24 * ip, 200, 20), mousePosition))
                {
                    c = Color.Yellow;
                    drawDesc = p;
                }
                Main.spriteBatch.Draw(Pixel, new Vector2(400, Main.screenHeight - 400 + 24 * ip), new Rectangle(0, 0, 200, 20), c);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, p.Name + " - " + p.Cost, new Vector2(400, Main.screenHeight - 400 + 24 * ip), Color.White, 0f, Vector2.Zero, Vector2.One);
            }

            if (drawDesc != null)
                drawDesc.DrawDesc(Main.spriteBatch, mousePosition);
        }
    }
}