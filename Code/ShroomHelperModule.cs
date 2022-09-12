using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System.Collections.Generic;
using Celeste.Mod.ShroomHelper.Effects;
using Celeste.Mod.ShroomHelper.Entities;
using System;
using System.Reflection;


namespace Celeste.Mod.ShroomHelper
{
    public class ShroomHelperModule : EverestModule
    {

        // Only one alive module instance can exist at any given time.
        public static ShroomHelperModule Instance;

        public ShroomHelperModule()
        {
            Instance = this;
        }

        // Set up any hooks, event handlers and your mod in general here.
        // Load runs before Celeste itself has initialized properly.
        public override void Load()
        {
            Everest.Events.Level.OnLoadBackdrop += Level_OnLoadBackdrop;
            DoubleRefillBooster.Load();

            // single dash golden
            Everest.Events.Level.OnLoadEntity += Level_OnLoadEntity;
            Everest.Events.Level.OnTransitionTo += Level_TransitionTo;
            On.Celeste.Player.StartDash += Player_StartDash;
            On.Celeste.Player.RefillDash += Player_RefillDash;
            On.Celeste.Player.UseRefill += Player_UseRefill;
            Everest.Events.Player.OnDie += Player_OnDie;
        }

        // Unload the entirety of your mod's content. Free up any native resources.
        public override void Unload()
        {
            Everest.Events.Level.OnLoadBackdrop -= Level_OnLoadBackdrop;
            DoubleRefillBooster.Unload();

            // single dash golden
            Everest.Events.Level.OnLoadEntity -= Level_OnLoadEntity;
            Everest.Events.Level.OnTransitionTo -= Level_TransitionTo;
            On.Celeste.Player.StartDash -= Player_StartDash;
            On.Celeste.Player.RefillDash -= Player_RefillDash;
            Everest.Events.Player.OnDie -= Player_OnDie;
        }

        // sprite bank loading
        public static SpriteBank spriteBank;
        public override void LoadContent(bool firstLoad)
        {
            base.LoadContent(firstLoad);
            spriteBank = new SpriteBank(GFX.Game, "Graphics/ShroomHelper/Sprites.xml");
        }

        private Backdrop Level_OnLoadBackdrop(MapData map, BinaryPacker.Element child, BinaryPacker.Element above)
        {
            if (child.Name.Equals("ShroomHelper/ShroomPetals", StringComparison.OrdinalIgnoreCase))
            {
                ShroomPetals shroomPetals = new ShroomPetals();
                if (child.HasAttr("color"))
                    shroomPetals.shroomColor = child.Attr("color");
                return shroomPetals;
            }

            return null;
        }

        // One Dash Golden Session Logic
        public override Type SessionType => typeof(ShroomHelperSession);
        public static ShroomHelperSession ShroomSession => (ShroomHelperSession)Instance._Session;

        private bool Level_OnLoadEntity(Level level, LevelData levelData, Vector2 offset, EntityData entityData)
        {
            if (entityData.Name == "ShroomHelper/OneDashWingedStrawberry")
            {
                if (level.Session.StartedFromBeginning && !ShroomSession.dashedTwice)
                {
                    return false;
                } else
                {
                    return true;
                }
            }

            return false; // false loads the entity, true de-spawns it
        }

        // only set session dashedTwice data on room transitions
        public void Level_TransitionTo(Level level, LevelData next, Vector2 direction)
        {
            if (ShroomSession.brokeDashLimitInRoom)
            {
                RegisterDashedTwice();
                ResetBrokeDashLimitInRoom();
            }
        }

        private void Player_OnDie(Player player)
        {
            ResetDashCounter();
            ResetBrokeDashLimitInRoom();
        }

        private int Player_StartDash(On.Celeste.Player.orig_StartDash orig, Player self)
        {
            orig(self);
            IncrementDashCounter();
            if (ShroomSession.beforeRefillDashCount > 1)
            {
                RegisterBrokeDashLimitInRoom();
            }
            return 2; // this method wants an int for some reason and it needs to be 2
        }

        private bool Player_RefillDash(On.Celeste.Player.orig_RefillDash orig, Player self)
        {
            ResetDashCounter();
            return orig(self);
        }

        private bool Player_UseRefill(On.Celeste.Player.orig_UseRefill orig, Player self, bool twoDashes)
        {
            ResetDashCounter();
            return orig(self, twoDashes);
        }

        private void RegisterDashedTwice() { ShroomSession.dashedTwice = true; }
        private void ResetDashCheck() { ShroomSession.dashedTwice = false; }

        private void ResetDashCounter() { ShroomSession.beforeRefillDashCount = 0; }
        private void IncrementDashCounter() { ShroomSession.beforeRefillDashCount += 1; }

        private void RegisterBrokeDashLimitInRoom() { ShroomSession.brokeDashLimitInRoom = true; }
        private void ResetBrokeDashLimitInRoom() { ShroomSession.brokeDashLimitInRoom = false; }
    }
}