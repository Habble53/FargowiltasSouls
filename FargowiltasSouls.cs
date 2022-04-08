﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
//using FargowiltasSouls.ModCompatibilities;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using FargowiltasSouls.EternityMode;
//using FargowiltasSouls.EternityMode.Content.Boss.HM;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
//using FargowiltasSouls.NPCs.AbomBoss;
//using FargowiltasSouls.NPCs.Champions;
//using FargowiltasSouls.NPCs.DeviBoss;
//using FargowiltasSouls.NPCs.MutantBoss;
using FargowiltasSouls.Sky;
//using Fargowiltas.Items.Summons.Deviantt;
//using Fargowiltas.Items.Misc;
//using Fargowiltas.Items.Explosives;
//using FargowiltasSouls.Items.Dyes;
using FargowiltasSouls.Toggler;
using System.Linq;
using Terraria.Chat;
using FargowiltasSouls.NPCs.EternityMode;
using ReLogic.Content;
using Terraria.GameContent;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Souls;
using FargowiltasSouls.EternityMode.Content.Boss.HM;
using FargowiltasSouls.Items;
using FargowiltasSouls.Items.Placeables;
using FargowiltasSouls.Items.Accessories.Enchantments;
using FargowiltasSouls.Items.Accessories.Forces;
using FargowiltasSouls.Items.Dyes;
using FargowiltasSouls.Items.Accessories.Souls;
using Terraria.GameContent.ItemDropRules;
using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Items.Consumables;

namespace FargowiltasSouls
{
    public partial class FargowiltasSouls : Mod
    {
        internal static ModKeybind FreezeKey;
        internal static ModKeybind GoldKey;
        internal static ModKeybind SmokeBombKey;
        internal static ModKeybind BetsyDashKey;
        internal static ModKeybind MutantBombKey;
        internal static ModKeybind SoulToggleKey;

        internal static List<int> DebuffIDs;

        internal static FargowiltasSouls Instance;

        internal bool LoadedNewSprites;

        internal static float OldMusicFade;

        public UserInterface CustomResources;

        internal static readonly Dictionary<int, int> ModProjDict = new Dictionary<int, int>();

        internal struct TextureBuffer
        {
            public static readonly Dictionary<int, Asset<Texture2D>> NPC = new Dictionary<int, Asset<Texture2D>>();
            public static readonly Dictionary<int, Asset<Texture2D>> NPCHeadBoss = new Dictionary<int, Asset<Texture2D>>();
            public static readonly Dictionary<int, Asset<Texture2D>> Gore = new Dictionary<int, Asset<Texture2D>>();
            public static readonly Dictionary<int, Asset<Texture2D>> Golem = new Dictionary<int, Asset<Texture2D>>();
            public static readonly Dictionary<int, Asset<Texture2D>> Extra = new Dictionary<int, Asset<Texture2D>>();
            public static Asset<Texture2D> Ninja = null;
            public static Asset<Texture2D> BoneArm = null;
            public static Asset<Texture2D> BoneArm2 = null;
            public static Asset<Texture2D> Chain12 = null;
            public static Asset<Texture2D> Chain26 = null;
            public static Asset<Texture2D> Chain27 = null;
            public static Asset<Texture2D> Wof = null;
        }

        public static UIManager UserInterfaceManager => Instance._userInterfaceManager;
        private UIManager _userInterfaceManager;

        //        #region Compatibilities

        //        public CalamityCompatibility CalamityCompatibility { get; private set; }
        //        public bool CalamityLoaded => CalamityCompatibility != null;

        //        public ThoriumCompatibility ThoriumCompatibility { get; private set; }
        //        public bool ThoriumLoaded => ThoriumCompatibility != null;

        //        public SoACompatibility SoACompatibility { get; private set; }
        //        public bool SoALoaded => SoACompatibility != null;

        //        public MasomodeEXCompatibility MasomodeEXCompatibility { get; private set; }
        //        public bool MasomodeEXLoaded => MasomodeEXCompatibility != null;

        //        public BossChecklistCompatibility BossChecklistCompatibility { get; private set; }
        //        public bool BossChecklistLoaded => BossChecklistCompatibility != null;

        //        #endregion Compatibilities

        //public Fargowiltas()
        //{
        //    Properties = new ModProperties
        //    {
        //        Autoload = true,
        //        AutoloadGores = true,
        //        AutoloadSounds = true
        //    };
        //}

        public void AddToggle(string toggle, string name, int item, string color = "ffffff")
        {
            ModTranslation text = LocalizationLoader.CreateTranslation(Instance, toggle);
            text.SetDefault($"[i:{item}] [c/{color}:{name}]");
            LocalizationLoader.AddTranslation(text);
        }

        public override void Load()
        {
            Instance = this;

            // Load EModeNPCMods
            foreach (Type type in Code.GetTypes().OrderBy(type => type.FullName, StringComparer.InvariantCulture))
            {
                if (type.IsSubclassOf(typeof(EModeNPCBehaviour)) && !type.IsAbstract)
                {
                    EModeNPCBehaviour mod = (EModeNPCBehaviour)Activator.CreateInstance(type);
                    mod.Load();
                }
            }

            // Just to make sure they're always in the same order
            EModeNPCBehaviour.AllEModeNpcBehaviours.OrderBy(m => m.GetType().FullName, StringComparer.InvariantCulture);

            SkyManager.Instance["FargowiltasSouls:AbomBoss"] = new AbomSky();
            SkyManager.Instance["FargowiltasSouls:MutantBoss"] = new MutantSky();
            SkyManager.Instance["FargowiltasSouls:MutantBoss2"] = new MutantSky2();

            //            if (Language.ActiveCulture == (int)GameCulture.CultureName.Chinese)
            //            {
            //                FreezeKey = RegisterHotKey("冻结时间", "P");
            //                GoldKey = RegisterHotKey("金身", "O");
            //                SmokeBombKey = RegisterHotKey("Throw Smoke Bomb", "I");
            //                BetsyDashKey = RegisterHotKey("Betsy Dash", "C");
            //                MutantBombKey = RegisterHotKey("Mutant Bomb", "Z");
            //                SoulToggleKey = RegisterHotKey("Open Soul Toggler", ".");
            //            }
            //            else
            //            {
            FreezeKey = KeybindLoader.RegisterKeybind(this, "Freeze Time", "P");
            GoldKey = KeybindLoader.RegisterKeybind(this, "Turn Gold", "O");
            SmokeBombKey = KeybindLoader.RegisterKeybind(this, "Throw Smoke Bomb", "I");
            BetsyDashKey = KeybindLoader.RegisterKeybind(this, "Fireball Dash", "C");
            MutantBombKey = KeybindLoader.RegisterKeybind(this, "Mutant Bomb", "Z");
            SoulToggleKey = KeybindLoader.RegisterKeybind(this, "Open Soul Toggler", ".");
            //            }

            ToggleLoader.Load();

            _userInterfaceManager = new UIManager();
            _userInterfaceManager.LoadUI();

            #region Toggles

            //AddToggle("PresetHeader", "Preset Configurations", ModContent.ItemType<Masochist>());

            #region enchants

            AddToggle("WoodHeader", "Force of Timber", ModContent.ItemType<TimberForce>());
            AddToggle("BorealConfig", "Boreal Snowballs", ModContent.ItemType<BorealWoodEnchant>(), "8B7464");
            AddToggle("MahoganyConfig", "Mahogany Hook Speed", ModContent.ItemType<RichMahoganyEnchant>(), "b56c64");
            AddToggle("EbonConfig", "Ebonwood Shadowflame", ModContent.ItemType<EbonwoodEnchant>(), "645a8d");
            AddToggle("ShadeConfig", "Blood Geyser On Hit", ModContent.ItemType<ShadewoodEnchant>(), "586876");
            AddToggle("ShadeOnHitConfig", "Proximity Triggers On Hit Effects", ModContent.ItemType<ShadewoodEnchant>(), "586876");
            AddToggle("PalmConfig", "Palmwood Sentry", ModContent.ItemType<PalmWoodEnchant>(), "b78d56");
            AddToggle("PearlConfig", "Pearlwood Rain", ModContent.ItemType<PearlwoodEnchant>(), "ad9a5f");

            AddToggle("EarthHeader", "Force of Earth", ModContent.ItemType<EarthForce>());
            AddToggle("AdamantiteConfig", "Adamantite Projectile Splitting", ModContent.ItemType<AdamantiteEnchant>(), "dd557d");
            AddToggle("CobaltConfig", "Cobalt Shards", ModContent.ItemType<CobaltEnchant>(), "3da4c4");
            AddToggle("AncientCobaltConfig", "Ancient Cobalt Stingers", ModContent.ItemType<AncientCobaltEnchant>(), "354c74");
            AddToggle("MythrilConfig", "Mythril Weapon Speed", ModContent.ItemType<MythrilEnchant>(), "9dd290");
            AddToggle("OrichalcumConfig", "Orichalcum Petals", ModContent.ItemType<OrichalcumEnchant>(), "eb3291");
            AddToggle("PalladiumConfig", "Palladium Healing", ModContent.ItemType<PalladiumEnchant>(), "f5ac28");
            AddToggle("PalladiumOrbConfig", "Palladium Orbs", ModContent.ItemType<PalladiumEnchant>(), "f5ac28");
            AddToggle("TitaniumConfig", "Titanium Shadow Dodge", ModContent.ItemType<TitaniumEnchant>(), "828c88");

            AddToggle("TerraHeader", "Terra Force", ModContent.ItemType<TerraForce>());
            AddToggle("CopperConfig", "Copper Lightning", ModContent.ItemType<CopperEnchant>(), "d56617");
            AddToggle("IronMConfig", "Iron Magnet", ModContent.ItemType<IronEnchant>(), "988e83");
            AddToggle("IronSConfig", "Iron Shield", ModContent.ItemType<IronEnchant>(), "988e83");
            AddToggle("TinConfig", "Tin Crits", ModContent.ItemType<TinEnchant>(), "a28b4e");
            AddToggle("TungstenConfig", "Tungsten Item Effect", ModContent.ItemType<TungstenEnchant>(), "b0d2b2");
            AddToggle("TungstenProjConfig", "Tungsten Projectile Effect", ModContent.ItemType<TungstenEnchant>(), "b0d2b2");
            AddToggle("ObsidianConfig", "Obsidian Explosions", ModContent.ItemType<ObsidianEnchant>(), "453e73");

            AddToggle("WillHeader", "Force of Will", ModContent.ItemType<WillForce>());
            AddToggle("GladiatorConfig", "Gladiator Rain", ModContent.ItemType<GladiatorEnchant>(), "9c924e");
            AddToggle("GoldConfig", "Gold Lucky Coin", ModContent.ItemType<GoldEnchant>(), "e7b21c");
            AddToggle("HuntressConfig", "Huntress Ability", ModContent.ItemType<HuntressEnchant>(), "7ac04c");
            AddToggle("ValhallaConfig", "Squire/Valhalla Healing", ModContent.ItemType<ValhallaKnightEnchant>(), "93651e");
            AddToggle("SquirePanicConfig", "Ballista Panic On Hit", ModContent.ItemType<SquireEnchant>(), "948f8c");

            AddToggle("LifeHeader", "Force of Life", ModContent.ItemType<LifeForce>());
            AddToggle("BeeConfig", "Bees", ModContent.ItemType<BeeEnchant>(), "FEF625");
            AddToggle("BeetleConfig", "Beetles", ModContent.ItemType<BeetleEnchant>(), "6D5C85");
            AddToggle("CactusConfig", "Cactus Needles", ModContent.ItemType<CactusEnchant>(), "799e1d");
            AddToggle("PumpkinConfig", "Grow Pumpkins", ModContent.ItemType<PumpkinEnchant>(), "e3651c");
            AddToggle("SpiderConfig", "Spider Crits", ModContent.ItemType<SpiderEnchant>(), "6d4e45");
            AddToggle("TurtleConfig", "Turtle Shell Buff", ModContent.ItemType<TurtleEnchant>(), "f89c5c");

            AddToggle("NatureHeader", "Force of Nature", ModContent.ItemType<NatureForce>());
            AddToggle("ChlorophyteConfig", "Chlorophyte Leaf Crystal", ModContent.ItemType<ChlorophyteEnchant>(), "248900");
            AddToggle("CrimsonConfig", "Crimson Regen", ModContent.ItemType<CrimsonEnchant>(), "C8364B");
            AddToggle("RainConfig", "Rain Clouds", ModContent.ItemType<RainEnchant>(), "ffec00");
            AddToggle("FrostConfig", "Frost Icicles", ModContent.ItemType<FrostEnchant>(), "7abdb9");
            AddToggle("SnowConfig", "Snowstorm", ModContent.ItemType<SnowEnchant>(), "25c3f2");
            AddToggle("JungleConfig", "Jungle Spores", ModContent.ItemType<JungleEnchant>(), "71971f");
            AddToggle("JungleDashConfig", "Jungle Dash", ModContent.ItemType<JungleEnchant>(), "71971f");
            AddToggle("MoltenConfig", "Molten Inferno Buff", ModContent.ItemType<MoltenEnchant>(), "c12b2b");
            AddToggle("MoltenEConfig", "Molten Explosion On Hit", ModContent.ItemType<MoltenEnchant>(), "c12b2b");
            AddToggle("ShroomiteConfig", "Shroomite Stealth", ModContent.ItemType<ShroomiteEnchant>(), "008cf4");
            AddToggle("ShroomiteShroomConfig", "Shroomite Mushrooms", ModContent.ItemType<ShroomiteEnchant>(), "008cf4");

            AddToggle("ShadowHeader", "Shadow Force", ModContent.ItemType<ShadowForce>());
            AddToggle("DarkArtConfig", "Flameburst Minion", ModContent.ItemType<DarkArtistEnchant>(), "9b5cb0");
            AddToggle("ApprenticeConfig", "Apprentice Effect", ModContent.ItemType<ApprenticeEnchant>(), "5d86a6");
            AddToggle("NecroConfig", "Necro Graves", ModContent.ItemType<NecroEnchant>(), "565643");
            AddToggle("NecroGloveConfig", "Necro Bone Glove", ModContent.ItemType<NecroEnchant>(), "565643");
            AddToggle("ShadowConfig", "Shadow Orbs", ModContent.ItemType<ShadowEnchant>(), "42356f");
            AddToggle("AncientShadowConfig", "Ancient Shadow Darkness", ModContent.ItemType<AncientShadowEnchant>(), "42356f");
            AddToggle("MonkConfig", "Monk Dash", ModContent.ItemType<MonkEnchant>(), "920520");
            AddToggle("ShinobiDashConfig", "Shinobi Teleport Dash", ModContent.ItemType<ShinobiEnchant>(), "935b18");
            AddToggle("ShinobiConfig", "Shinobi Through Walls", ModContent.ItemType<ShinobiEnchant>(), "935b18");
            AddToggle("SpookyConfig", "Spooky Scythes", ModContent.ItemType<SpookyEnchant>(), "644e74");

            AddToggle("SpiritHeader", "Force of Spirit", ModContent.ItemType<SpiritForce>());
            AddToggle("FossilConfig", "Fossil Bones On Hit", ModContent.ItemType<FossilEnchant>(), "8c5c3b");
            AddToggle("ForbiddenConfig", "Forbidden Storm", ModContent.ItemType<ForbiddenEnchant>(), "e7b21c");
            AddToggle("HallowedConfig", "Hallowed Enchanted Sword Familiar", ModContent.ItemType<HallowEnchant>(), "968564");
            AddToggle("HallowSConfig", "Hallowed Shield", ModContent.ItemType<HallowEnchant>(), "968564");
            AddToggle("SilverConfig", "Silver Sword Familiar", ModContent.ItemType<SilverEnchant>(), "b4b4cc");
            AddToggle("SilverSpeedConfig", "Silver Minion Speed", ModContent.ItemType<SilverEnchant>(), "b4b4cc");
            AddToggle("SpectreConfig", "Spectre Orbs", ModContent.ItemType<SpectreEnchant>(), "accdfc");
            AddToggle("TikiConfig", "Tiki Minions", ModContent.ItemType<TikiEnchant>(), "56A52B");

            AddToggle("CosmoHeader", "Force of Cosmos", ModContent.ItemType<CosmoForce>());
            AddToggle("MeteorConfig", "Meteor Shower", ModContent.ItemType<MeteorEnchant>(), "5f4752");
            AddToggle("NebulaConfig", "Nebula Boosters", ModContent.ItemType<NebulaEnchant>(), "fe7ee5");
            AddToggle("SolarConfig", "Solar Shield", ModContent.ItemType<SolarEnchant>(), "fe9e23");
            AddToggle("SolarFlareConfig", "Inflict Solar Flare", ModContent.ItemType<SolarEnchant>(), "fe9e23");
            AddToggle("StardustConfig", "Stardust Guardian", ModContent.ItemType<StardustEnchant>(), "00aeee");
            AddToggle("VortexSConfig", "Vortex Stealth", ModContent.ItemType<VortexEnchant>(), "00f2aa");
            AddToggle("VortexVConfig", "Vortex Voids", ModContent.ItemType<VortexEnchant>(), "00f2aa");

            #endregion enchants

            #region masomode toggles

            //Masomode Header
            AddToggle("MasoHeader", "Eternity Mode", ModContent.ItemType<MutantStatue>());
            AddToggle("MasoBossRecolors", "Boss Recolors", ModContent.ItemType<Masochist>());
            AddToggle("MasoCanPlay", "Masochist Mode Available", ModContent.ItemType<Masochist>(), "ff0000");
            
            AddToggle("MasoHeader2", "Eternity Mode Accessories", ModContent.ItemType<DeviatingEnergy>());
            AddToggle("MasoAeolusConfig", "Aeolus Jump", ModContent.ItemType<AeolusBoots>());
            AddToggle("MasoAeolusFlowerConfig", "Flower Boots", ModContent.ItemType<AeolusBoots>());
            AddToggle("MasoIconConfig", "Sinister Icon Spawn Rates", ModContent.ItemType<SinisterIcon>());
            AddToggle("MasoIconDropsConfig", "Sinister Icon Drops", ModContent.ItemType<SinisterIcon>());
            AddToggle("MasoGrazeConfig", "Graze", ModContent.ItemType<SparklingAdoration>());
            AddToggle("MasoGrazeRingConfig", "Graze Radius Visual", ModContent.ItemType<SparklingAdoration>());
            AddToggle("MasoDevianttHeartsConfig", "Homing Hearts On Hit", ModContent.ItemType<SparklingAdoration>());

            //supreme death fairy header
            AddToggle("SupremeFairyHeader", "Supreme Deathbringer Fairy", ModContent.ItemType<SupremeDeathbringerFairy>());
            AddToggle("MasoSlimeConfig", "Slimy Balls", ModContent.ItemType<SlimyShield>());
            AddToggle("SlimeFallingConfig", "Increased Fall Speed", ModContent.ItemType<SlimyShield>());
            AddToggle("MasoEyeConfig", "Scythes When Dashing", ModContent.ItemType<AgitatingLens>());
            AddToggle("MasoHoneyConfig", "Honey When Hitting Enemies", ModContent.ItemType<QueenStinger>());
            AddToggle("MasoSkeleConfig", "Skeletron Arms Minion", ModContent.ItemType<NecromanticBrew>());

            //bionomic
            AddToggle("BionomicHeader", "Bionomic Cluster", ModContent.ItemType<BionomicCluster>());
            AddToggle("MasoConcoctionConfig", "Tim's Concoction", ModContent.ItemType<TimsConcoction>());
            AddToggle("MasoCarrotConfig", "Carrot View", ModContent.ItemType<OrdinaryCarrot>());
            AddToggle("MasoRainbowConfig", "Rainbow Slime Minion", ModContent.ItemType<ConcentratedRainbowMatter>());
            AddToggle("MasoFrigidConfig", "Frostfireballs", ModContent.ItemType<FrigidGemstone>());
            AddToggle("MasoNymphConfig", "Attacks Spawn Hearts", ModContent.ItemType<NymphsPerfume>());
            AddToggle("MasoSqueakConfig", "Squeaky Toy On Hit", ModContent.ItemType<Items.Accessories.Masomode.SqueakyToy>());
            AddToggle("MasoPouchConfig", "Shadowflame Tentacles", ModContent.ItemType<WretchedPouch>());
            AddToggle("MasoClippedConfig", "Inflict Clipped Wings", ModContent.ItemType<WyvernFeather>());
            AddToggle("TribalCharmConfig", "Tribal Charm Auto Swing", ModContent.ItemType<TribalCharm>());
            //AddToggle("WalletHeader", "Security Wallet", ModContent.ItemType<SecurityWallet>());

            //dubious
            AddToggle("DubiousHeader", "Dubious Circuitry", ModContent.ItemType<DubiousCircuitry>());
            AddToggle("MasoLightningConfig", "Inflict Lightning Rod", ModContent.ItemType<GroundStick>());
            AddToggle("MasoProbeConfig", "Probes Minion", ModContent.ItemType<GroundStick>());

            //pure heart
            AddToggle("PureHeartHeader", "Pure Heart", ModContent.ItemType<PureHeart>());
            AddToggle("MasoEaterConfig", "Tiny Eaters", ModContent.ItemType<CorruptHeart>());
            AddToggle("MasoBrainConfig", "Creeper Shield", ModContent.ItemType<GuttedHeart>());

            //lump of flesh
            AddToggle("LumpofFleshHeader", "Lump of Flesh", ModContent.ItemType<LumpOfFlesh>());
            AddToggle("MasoPugentConfig", "Pungent Eye Minion", ModContent.ItemType<LumpOfFlesh>());

            //chalice
            AddToggle("ChaliceHeader", "Chalice of the Moon", ModContent.ItemType<ChaliceoftheMoon>());
            AddToggle("MasoCultistConfig", "Cultist Minion", ModContent.ItemType<ChaliceoftheMoon>());
            AddToggle("MasoPlantConfig", "Plantera Minion", ModContent.ItemType<MagicalBulb>());
            AddToggle("MasoGolemConfig", "Lihzahrd Ground Pound", ModContent.ItemType<LihzahrdTreasureBox>());
            AddToggle("MasoBoulderConfig", "Boulder Spray", ModContent.ItemType<LihzahrdTreasureBox>());
            AddToggle("MasoCelestConfig", "Celestial Rune Support", ModContent.ItemType<CelestialRune>());
            AddToggle("MasoVisionConfig", "Ancient Visions On Hit", ModContent.ItemType<CelestialRune>());

            //heart of the masochist
            AddToggle("HeartHeader", "Heart of the Eternal", ModContent.ItemType<HeartoftheMasochist>());
            AddToggle("MasoPumpConfig", "Pumpking's Cape Support", ModContent.ItemType<PumpkingsCape>());
            AddToggle("IceQueensCrownConfig", "Freeze On Hit", ModContent.ItemType<IceQueensCrown>());
            AddToggle("MasoFlockoConfig", "Flocko Minion", ModContent.ItemType<IceQueensCrown>());
            AddToggle("MasoUfoConfig", "Saucer Minion", ModContent.ItemType<SaucerControlConsole>());
            AddToggle("MasoGravConfig", "Gravity Control", ModContent.ItemType<GalacticGlobe>());
            AddToggle("MasoGrav2Config", "Stabilized Gravity", ModContent.ItemType<GalacticGlobe>());
            AddToggle("MasoTrueEyeConfig", "True Eyes Minion", ModContent.ItemType<GalacticGlobe>());

            //cyclonic fin
            AddToggle("CyclonicHeader", "Abominable Wand", ModContent.ItemType<AbominableWand>());
            AddToggle("MasoFishronConfig", "Spectral Abominationn", ModContent.ItemType<AbominableWand>());

            //            //mutant armor
            //            AddToggle("MutantArmorHeader", "True Mutant Armor", ModContent.ItemType<HeartoftheMasochist>());
            //            AddToggle("MasoAbomConfig", "Abominationn Minion", ModContent.ItemType<MutantMask>());
            //            AddToggle("MasoRingConfig", "Phantasmal Ring Minion", ModContent.ItemType<MutantMask>());
            //            AddToggle("MasoReviveDeathrayConfig", "Deathray When Revived", ModContent.ItemType<MutantMask>());

            #endregion masomode toggles

            #region soul toggles

            AddToggle("SoulHeader", "Souls", ModContent.ItemType<UniverseSoul>());
            AddToggle("MeleeConfig", "Melee Speed", ModContent.ItemType<BerserkerSoul>());
            AddToggle("MagmaStoneConfig", "Magma Stone", ModContent.ItemType<BerserkerSoul>());
            AddToggle("YoyoBagConfig", "Yoyo Bag", ModContent.ItemType<BerserkerSoul>());
            AddToggle("MoonCharmConfig", "Moon Charm", ModContent.ItemType<BerserkerSoul>());
            AddToggle("NeptuneShellConfig", "Neptune's Shell", ModContent.ItemType<BerserkerSoul>());
            AddToggle("SniperConfig", "Sniper Scope", ModContent.ItemType<SnipersSoul>());
            AddToggle("UniverseConfig", "Universe Attack Speed", ModContent.ItemType<UniverseSoul>());
            AddToggle("MiningHuntConfig", "Mining Hunter Buff", ModContent.ItemType<MinerEnchant>());
            AddToggle("MiningDangerConfig", "Mining Dangersense Buff", ModContent.ItemType<MinerEnchant>());
            AddToggle("MiningSpelunkConfig", "Mining Spelunker Buff", ModContent.ItemType<MinerEnchant>());
            AddToggle("MiningShineConfig", "Mining Shine Buff", ModContent.ItemType<MinerEnchant>());
            AddToggle("BuilderConfig", "Builder Mode", ModContent.ItemType<WorldShaperSoul>());
            AddToggle("TrawlerSporeConfig", "Spore Sac", ModContent.ItemType<TrawlerSoul>());
            AddToggle("DefenseStarConfig", "Stars On Hit", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefenseBeeConfig", "Bees On Hit", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefensePanicConfig", "Panic On Hit", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefenseFleshKnuckleConfig", "Flesh Knuckles Aggro", ModContent.ItemType<ColossusSoul>());
            AddToggle("DefensePaladinConfig", "Paladin's Shield", ModContent.ItemType<ColossusSoul>());
            AddToggle("FlightMasteryInsigniaConfig", "Soaring Insignia Acceleration", ModContent.ItemType<FlightMasterySoul>());
            AddToggle("FlightMasteryGravityConfig", "Amplified Gravity", ModContent.ItemType<FlightMasterySoul>());
            AddToggle("RunSpeedConfig", "Higher Base Run Speed", ModContent.ItemType<SupersonicSoul>());
            AddToggle("MomentumConfig", "No Momentum", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicTabiConfig", "Tabi Dash", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicClimbingConfig", "Tiger Climbing Gear", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicConfig", "Supersonic Speed Boosts", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicJumpsConfig", "Supersonic Jumps", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicRocketBootsConfig", "Supersonic Rocket Boots", ModContent.ItemType<SupersonicSoul>());
            AddToggle("SupersonicCarpetConfig", "Supersonic Carpet", ModContent.ItemType<SupersonicSoul>());
            //AddToggle("SupersonicFlowerConfig", "Flower Boots", ModContent.ItemType<SupersonicSoul>(), "248900");
            AddToggle("CthulhuShieldConfig", "Shield of Cthulhu", ModContent.ItemType<SupersonicSoul>());
            AddToggle("BlackBeltConfig", "Black Belt", ModContent.ItemType<SupersonicSoul>());
            AddToggle("TrawlerConfig", "Trawler Extra Lures", ModContent.ItemType<TrawlerSoul>());
            AddToggle("TrawlerJumpConfig", "Trawler Jump", ModContent.ItemType<TrawlerSoul>());
            //            AddToggle("EternityConfig", "Eternity Stacking", ModContent.ItemType<EternitySoul>());

            //            #endregion soul toggles

            //            #region pet toggles

            //            AddToggle("PetHeader", "Pets", ItemID.ZephyrFish);
            //            AddToggle("PetBlackCatConfig", "Black Cat Pet", 1810);
            //            AddToggle("PetCompanionCubeConfig", "Companion Cube Pet", 3628);
            //            AddToggle("PetCursedSaplingConfig", "Cursed Sapling Pet", 1837);
            //            AddToggle("PetDinoConfig", "Dino Pet", 1242);
            //            AddToggle("PetDragonConfig", "Dragon Pet", 3857);
            //            AddToggle("PetEaterConfig", "Eater Pet", 994);
            //            AddToggle("PetEyeSpringConfig", "Eye Spring Pet", 1311);
            //            AddToggle("PetFaceMonsterConfig", "Face Monster Pet", 3060);
            //            AddToggle("PetGatoConfig", "Gato Pet", 3855);
            //            AddToggle("PetHornetConfig", "Hornet Pet", 1170);
            //            AddToggle("PetLizardConfig", "Lizard Pet", 1172);
            //            AddToggle("PetMinitaurConfig", "Mini Minotaur Pet", 2587);
            //            AddToggle("PetParrotConfig", "Parrot Pet", 1180);
            //            AddToggle("PetPenguinConfig", "Penguin Pet", 669);
            //            AddToggle("PetPupConfig", "Puppy Pet", 1927);
            //            AddToggle("PetSeedConfig", "Seedling Pet", 1182);
            //            AddToggle("PetDGConfig", "Skeletron Pet", 1169);
            //            AddToggle("PetSnowmanConfig", "Snowman Pet", 1312);
            //            AddToggle("PetGrinchConfig", "Grinch Pet", ItemID.BabyGrinchMischiefWhistle);
            //            AddToggle("PetSpiderConfig", "Spider Pet", 1798);
            //            AddToggle("PetSquashConfig", "Squashling Pet", 1799);
            //            AddToggle("PetTikiConfig", "Tiki Pet", 1171);
            //            AddToggle("PetShroomConfig", "Truffle Pet", 1181);
            //            AddToggle("PetTurtleConfig", "Turtle Pet", 753);
            //            AddToggle("PetZephyrConfig", "Zephyr Fish Pet", 2420);
            //            AddToggle("PetHeartConfig", "Crimson Heart Pet", 3062);
            //            AddToggle("PetNaviConfig", "Fairy Pet", 425);
            //            AddToggle("PetFlickerConfig", "Flickerwick Pet", 3856);
            //            AddToggle("PetLanternConfig", "Magic Lantern Pet", 3043);
            //            AddToggle("PetOrbConfig", "Shadow Orb Pet", 115);
            //            AddToggle("PetSuspEyeConfig", "Suspicious Eye Pet", 3577);
            //            AddToggle("PetWispConfig", "Wisp Pet", 1183);

            #endregion pet toggles

            #region patreon toggles
            AddToggle("PatreonHeader", "Patreon Items", ModContent.ItemType<Patreon.Gittle.RoombaPet>());
            AddToggle("PatreonRoomba", "Roomba", ModContent.ItemType<Patreon.Gittle.RoombaPet>());
            AddToggle("PatreonOrb", "Computation Orb", ModContent.ItemType<Patreon.Daawnz.ComputationOrb>());
            AddToggle("PatreonFishingRod", "Miss Drakovi's Fishing Pole", ModContent.ItemType<Patreon.Sasha.MissDrakovisFishingPole>());
            AddToggle("PatreonDoor", "Squidward Door", ModContent.ItemType<Patreon.Sam.SquidwardDoor>());
            AddToggle("PatreonWolf", "Paradox Wolf Soul", ModContent.ItemType<Patreon.ParadoxWolf.ParadoxWolfSoul>());
            AddToggle("PatreonDove", "Fig Branch", ModContent.ItemType<Patreon.ManliestDove.FigBranch>());
            AddToggle("PatreonKingSlime", "Medallion of the Fallen King", ModContent.ItemType<Patreon.Catsounds.MedallionoftheFallenKing>());
            AddToggle("PatreonFishron", "Staff Of Unleashed Ocean", ModContent.ItemType<Patreon.DemonKing.StaffOfUnleashedOcean>());
            AddToggle("PatreonPlant", "Piranha Plant Voodoo Doll", ModContent.ItemType<Patreon.LaBonez.PiranhaPlantVoodooDoll>());
            AddToggle("PatreonDevious", "Devious Aestheticus", ModContent.ItemType<Patreon.DevAesthetic.DeviousAestheticus>());
            AddToggle("PatreonVortex", "Vortex Ritual", ModContent.ItemType<Patreon.GreatestKraken.VortexMagnetRitual>());
            AddToggle("PatreonPrime", "Prime Staff", ModContent.ItemType<Patreon.Purified.PrimeStaff>());
            AddToggle("PatreonCrimetroid", "Crimetroid", ModContent.ItemType<Patreon.Shucks.CrimetroidEgg>());
            #endregion patreon toggles
            
            #endregion Toggles

            if (Main.netMode != NetmodeID.Server)
            {
                #region shaders

                //loading refs for shaders
                Ref<Effect> lcRef = new Ref<Effect>(Assets.Request<Effect>("Effects/LifeChampionShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> wcRef = new Ref<Effect>(Assets.Request<Effect>("Effects/WillChampionShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> gaiaRef = new Ref<Effect>(Assets.Request<Effect>("Effects/GaiaShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> textRef = new Ref<Effect>(Assets.Request<Effect>("Effects/TextShader", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> invertRef = new Ref<Effect>(Assets.Request<Effect>("Effects/Invert", AssetRequestMode.ImmediateLoad).Value);
                Ref<Effect> finalSparkRef = new Ref<Effect>(Assets.Request<Effect>("Effects/FinalSpark", AssetRequestMode.ImmediateLoad).Value);
                //Ref<Effect> shockwaveRef = new Ref<Effect>(Assets.Request<Effect>("Effects/ShockwaveEffect", AssetRequestMode.ImmediateLoad).Value); // The path to the compiled shader file.

                //loading shaders from refs
                GameShaders.Misc["LCWingShader"] = new MiscShaderData(lcRef, "LCWings");
                GameShaders.Armor.BindShader(ModContent.ItemType<LifeDye>(), new ArmorShaderData(lcRef, "LCArmor").UseColor(new Color(1f, 0.647f, 0.839f)).UseSecondaryColor(Color.Goldenrod));

                GameShaders.Misc["WCWingShader"] = new MiscShaderData(wcRef, "WCWings");
                GameShaders.Armor.BindShader(ModContent.ItemType<WillDye>(), new ArmorShaderData(wcRef, "WCArmor").UseColor(Color.DarkOrchid).UseSecondaryColor(Color.LightPink).UseImage("Images/Misc/noise"));

                GameShaders.Misc["GaiaShader"] = new MiscShaderData(gaiaRef, "GaiaGlow");
                GameShaders.Armor.BindShader(ModContent.ItemType<GaiaDye>(), new ArmorShaderData(gaiaRef, "GaiaArmor").UseColor(new Color(0.44f, 1, 0.09f)).UseSecondaryColor(new Color(0.5f, 1f, 0.9f)));

                GameShaders.Misc["PulseUpwards"] = new MiscShaderData(textRef, "PulseUpwards");
                GameShaders.Misc["PulseDiagonal"] = new MiscShaderData(textRef, "PulseDiagonal");
                GameShaders.Misc["PulseCircle"] = new MiscShaderData(textRef, "PulseCircle");

                Filters.Scene["FargowiltasSouls:Invert"] = new Filter(new TimeStopShader(invertRef, "Main"), EffectPriority.VeryHigh);
                Filters.Scene["FargowiltasSouls:FinalSpark"] = new Filter(new FinalSparkShader(finalSparkRef, "FinalSpark"), EffectPriority.VeryHigh);

                //Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(shockwaveRef, "Shockwave"), EffectPriority.VeryHigh);
                //Filters.Scene["Shockwave"].Load();

                #endregion shaders
            }

            //            PatreonMiscMethods.Load(this);

            On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.CanDrop += IsMasterModeOrEMode_CanDrop;
            On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.CanShowItemDropInUI += IsMasterModeOrEMode_CanShowItemDropInUI;
            On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.CanDrop += DropBasedOnMasterOrEMode_CanDrop;
            On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction += DropBasedOnMasterOrEMode_TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction;
        }

        private static bool IsMasterModeOrEMode_CanDrop(
            On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.orig_CanDrop orig,
            Conditions.IsMasterMode self, DropAttemptInfo info)
        {
            // Use | instead of || so orig runs no matter what.
            return FargoSoulsWorld.EternityMode | orig(self, info);
        }

        private static bool IsMasterModeOrEMode_CanShowItemDropInUI(
            On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.orig_CanShowItemDropInUI orig,
            Conditions.IsMasterMode self)
        {
            // Use | instead of || so orig runs no matter what.
            return FargoSoulsWorld.EternityMode | orig(self);
        }

        private static bool DropBasedOnMasterOrEMode_CanDrop(
            On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.orig_CanDrop orig,
            DropBasedOnMasterMode self, DropAttemptInfo info)
        {
            // Use | instead of || so orig runs no matter what.
            return (FargoSoulsWorld.EternityMode && self.ruleForMasterMode.CanDrop(info)) | orig(self, info);
        }

        private static ItemDropAttemptResult DropBasedOnMasterOrEMode_TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction(
            On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.orig_TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction orig,
            DropBasedOnMasterMode self, DropAttemptInfo info, ItemDropRuleResolveAction resolveAction)
        {
            ItemDropAttemptResult itemDropAttemptResult = orig(self, info, resolveAction);
            return FargoSoulsWorld.EternityMode ? resolveAction(self.ruleForMasterMode, info) : itemDropAttemptResult;
        }

        public override void Unload()
        {
            On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.CanDrop -= IsMasterModeOrEMode_CanDrop;
            On.Terraria.GameContent.ItemDropRules.Conditions.IsMasterMode.CanShowItemDropInUI -= IsMasterModeOrEMode_CanShowItemDropInUI;
            On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.CanDrop -= DropBasedOnMasterOrEMode_CanDrop;
            On.Terraria.GameContent.ItemDropRules.DropBasedOnMasterMode.TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction -= DropBasedOnMasterOrEMode_TryDroppingItem_DropAttemptInfo_ItemDropRuleResolveAction;

            NPC.LunarShieldPowerExpert = 150;

            void RestoreSprites(Dictionary<int, Asset<Texture2D>> buffer, Asset<Texture2D>[] original)
            {
                foreach (KeyValuePair<int, Asset<Texture2D>> pair in buffer)
                    original[pair.Key] = pair.Value;

                buffer.Clear();
            }

            RestoreSprites(TextureBuffer.NPC, TextureAssets.Npc);
            RestoreSprites(TextureBuffer.NPCHeadBoss, TextureAssets.NpcHeadBoss);
            RestoreSprites(TextureBuffer.Gore, TextureAssets.Gore);
            RestoreSprites(TextureBuffer.Golem, TextureAssets.Golem);
            RestoreSprites(TextureBuffer.Extra, TextureAssets.Extra);

            if (TextureBuffer.Ninja != null)
                TextureAssets.Ninja = TextureBuffer.Ninja;
            if (TextureBuffer.BoneArm != null)
                TextureAssets.BoneArm = TextureBuffer.BoneArm;
            if (TextureBuffer.BoneArm2 != null)
                TextureAssets.BoneArm2 = TextureBuffer.BoneArm2;
            if (TextureBuffer.Chain12 != null)
                TextureAssets.Chain12 = TextureBuffer.Chain12;
            if (TextureBuffer.Chain26 != null)
                TextureAssets.Chain26 = TextureBuffer.Chain26;
            if (TextureBuffer.Chain27 != null)
                TextureAssets.Chain27 = TextureBuffer.Chain27;
            if (TextureBuffer.Wof != null)
                TextureAssets.Wof = TextureBuffer.Wof;

            if (DebuffIDs != null)
                DebuffIDs.Clear();

            //            OldMusicFade = 0;

            //            //game will reload golem textures, this helps prevent the crash on reload
            //            Main.NPCLoaded[NPCID.Golem] = false;
            //            Main.NPCLoaded[NPCID.GolemFistLeft] = false;
            //            Main.NPCLoaded[NPCID.GolemFistRight] = false;
            //            Main.NPCLoaded[NPCID.GolemHead] = false;
            //            Main.NPCLoaded[NPCID.GolemHeadFree] = false;

            EModeNPCBehaviour.AllEModeNpcBehaviours.Clear();

            ToggleLoader.Unload();
        }

        public override object Call(params object[] args)
        {
            try
            {
                string code = args[0].ToString();

                switch (code)
                {
                    case "Emode":
                    case "EMode":
                    case "EternityMode":
                        return FargoSoulsWorld.EternityMode;

                    case "Masomode":
                    case "MasoMode":
                    case "MasochistMode":
                    case "ForgottenMode":
                    case "Forgor":
                    case "ForgorMode":
                    case "MasomodeReal":
                    case "MasoModeReal":
                    case "MasochistModeReal":
                    case "RealMode":
                    case "GetReal":
                        return FargoSoulsWorld.MasochistModeReal;

                    case "DownedMutant":
                        return FargoSoulsWorld.downedMutant;

                    case "DownedAbom":
                    case "DownedAbominationn":
                        return FargoSoulsWorld.downedAbom;

                    case "DownedChamp":
                    case "DownedChampion":
                        return FargoSoulsWorld.downedChampions[(int)args[1]];

                    case "DownedEri":
                    case "DownedEridanus":
                    case "DownedCosmos":
                    case "DownedCosmosChamp":
                    case "DownedCosmosChampion":
                        return FargoSoulsWorld.downedChampions[8];

                    case "DownedDevi":
                    case "DownedDeviantt":
                        return FargoSoulsWorld.downedDevi;

                    case "DownedFishronEX":
                        return FargoSoulsWorld.downedFishronEX;

                    case "PureHeart":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().PureHeart;

                    case "MutantAntibodies":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().MutantAntibodies;

                    case "SinisterIcon":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().SinisterIcon;

                    case "AbomAlive":
                        return false; //FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.abomBoss, ModContent.NPCType<AbomBoss>());

                    case "MutantAlive":
                        return false; //FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>());

                    case "DeviAlive":
                    case "DeviBossAlive":
                    case "DevianttAlive":
                    case "DevianttBossAlive":
                        return FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.deviBoss, ModContent.NPCType<NPCs.DeviBoss.DeviBoss>());

                    case "MutantPact":
                    case "MutantsPact":
                    case "MutantCreditCard":
                    case "MutantsCreditCard":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().MutantsCreditCard;

                    case "MutantDiscountCard":
                    case "MutantsDiscountCard":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().MutantsDiscountCard;

                    case "EridanusArmor":
                    case "EridanusArmour":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().EridanusEmpower;

                    /*case "DevianttGifts":

                        Player player = Main.LocalPlayer;
                        FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();

                        if (!fargoPlayer.ReceivedMasoGift)
                        {
                            fargoPlayer.ReceivedMasoGift = true;
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                DropDevianttsGift(player);
                            }
                            else if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                var netMessage = GetPacket(); // Broadcast item request to server
                                netMessage.Write((byte)14);
                                netMessage.Write((byte)player.whoAmI);
                                netMessage.Send();
                            }

                            return true;
                        }

                        break;*/

                    case "GiftsReceived":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().ReceivedMasoGift;

                    case "GiveDevianttGifts":
                        Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().ReceivedMasoGift = true;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            DropDevianttsGift(Main.LocalPlayer);
                        }
                        else if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            var netMessage = GetPacket(); // Broadcast item request to server
                            netMessage.Write((byte)14);
                            netMessage.Write((byte)Main.LocalPlayer.whoAmI);
                            netMessage.Send();
                        }

                        Main.npcChatText = "This world looks tougher than usual, so you can have these on the house just this once! Talk to me if you need any tips, yeah?";
                        break;

                    case "SummonCrit":
                    case "SummonCritChance":
                    case "GetSummonCrit":
                    case "GetSummonCritChance":
                    case "SummonerCrit":
                    case "SummonerCritChance":
                    case "GetSummonerCrit":
                    case "GetSummonerCritChance":
                    case "MinionCrit":
                    case "MinionCritChance":
                    case "GetMinionCrit":
                    case "GetMinionCritChance":
                        return Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().SummonCrit;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Call Error: " + e.StackTrace + e.Message);
            }

            return base.Call(args);
        }

        public static void DropDevianttsGift(Player player)
        {
            Item.NewItem(null, player.Center, ItemID.SilverPickaxe);
            Item.NewItem(null, player.Center, ItemID.SilverAxe);
            Item.NewItem(null, player.Center, ItemID.SilverHammer);

            Item.NewItem(null, player.Center, ItemID.Torch, 100);
            Item.NewItem(null, player.Center, ItemID.LifeCrystal, 4);
            Item.NewItem(null, player.Center, ItemID.ManaCrystal, 4);
            Item.NewItem(null, player.Center, ItemID.RecallPotion, 15);
            if (Main.netMode != NetmodeID.SinglePlayer)
                Item.NewItem(null, player.Center, ItemID.WormholePotion, 15);

            //Item.NewItem(null, player.Center, ModContent.ItemType<DevianttsSundial>());
            //Item.NewItem(null, player.Center, ModContent.ItemType<EternityAdvisor>());

            void GiveItem(string modName, string itemName, int amount = 1)
            {
                if (ModContent.TryFind(modName, itemName, out ModItem modItem))
                    Item.NewItem(null, player.Center, modItem.Type, amount);
            }

            GiveItem("Fargowiltas", "AutoHouse", 5);
            GiveItem("Fargowiltas", "MiniInstabridge", 5);
            GiveItem("Fargowiltas", "HalfInstavator");

            Item.NewItem(null, player.Center, ModContent.ItemType<EurusSock>());
            Item.NewItem(null, player.Center, ModContent.ItemType<PuffInABottle>());
            Item.NewItem(null, player.Center, ItemID.BugNet);
            Item.NewItem(null, player.Center, ItemID.GrapplingHook);

            //only give once per world
            if (!FargoSoulsWorld.ReceivedTerraStorage)
            {
                if (ModLoader.TryGetMod("MagicStorage", out Mod magicStorage))
                {
                    GiveItem("MagicStorage", "StorageHeart");
                    GiveItem("MagicStorage", "CraftingAccess");
                    GiveItem("MagicStorage", "StorageUnit", 16);

                    FargoSoulsWorld.ReceivedTerraStorage = true;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.WorldData); //sync world in mp
                }
                else if (ModLoader.TryGetMod("MagicStorageExtra", out Mod magicStorageExtra))
                {
                    GiveItem("MagicStorageExtra", "StorageHeart");
                    GiveItem("MagicStorageExtra", "CraftingAccess");
                    GiveItem("MagicStorageExtra", "StorageUnit", 16);

                    FargoSoulsWorld.ReceivedTerraStorage = true;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.WorldData); //sync world in mp
                }
            }
        }

        //bool sheet
        public override void PostSetupContent()
        {
            try
            {
                //CalamityCompatibility = new CalamityCompatibility(this).TryLoad() as CalamityCompatibility;
                //ThoriumCompatibility = new ThoriumCompatibility(this).TryLoad() as ThoriumCompatibility;
                //SoACompatibility = new SoACompatibility(this).TryLoad() as SoACompatibility;
                //MasomodeEXCompatibility = new MasomodeEXCompatibility(this).TryLoad() as MasomodeEXCompatibility;
                //BossChecklistCompatibility = (BossChecklistCompatibility)new BossChecklistCompatibility(this).TryLoad();

                //if (BossChecklistCompatibility != null)
                //    BossChecklistCompatibility.Initialize();

                DebuffIDs = new List<int> { BuffID.Bleeding, BuffID.OnFire, BuffID.Rabies, BuffID.Confused, BuffID.Weak, BuffID.BrokenArmor, BuffID.Darkness, BuffID.Slow, BuffID.Cursed, BuffID.Poisoned, BuffID.Silenced, 39, 44, 46, 47, 67, 68, 69, 70, 80,
                            88, 94, 103, 137, 144, 145, 149, 156, 160, 163, 164, 195, 196, 197, 199 };
                DebuffIDs.Add(ModContent.BuffType<Antisocial>());
                DebuffIDs.Add(ModContent.BuffType<Atrophied>());
                DebuffIDs.Add(ModContent.BuffType<Berserked>());
                DebuffIDs.Add(ModContent.BuffType<Bloodthirsty>());
                DebuffIDs.Add(ModContent.BuffType<ClippedWings>());
                DebuffIDs.Add(ModContent.BuffType<Crippled>());
                DebuffIDs.Add(ModContent.BuffType<CurseoftheMoon>());
                DebuffIDs.Add(ModContent.BuffType<Defenseless>());
                DebuffIDs.Add(ModContent.BuffType<FlamesoftheUniverse>());
                DebuffIDs.Add(ModContent.BuffType<Flipped>());
                DebuffIDs.Add(ModContent.BuffType<FlippedHallow>());
                DebuffIDs.Add(ModContent.BuffType<Fused>());
                DebuffIDs.Add(ModContent.BuffType<GodEater>());
                DebuffIDs.Add(ModContent.BuffType<Guilty>());
                DebuffIDs.Add(ModContent.BuffType<Hexed>());
                DebuffIDs.Add(ModContent.BuffType<HolyPrice>());
                DebuffIDs.Add(ModContent.BuffType<Hypothermia>());
                DebuffIDs.Add(ModContent.BuffType<Infested>());
                DebuffIDs.Add(ModContent.BuffType<InfestedEX>());
                DebuffIDs.Add(ModContent.BuffType<IvyVenom>());
                DebuffIDs.Add(ModContent.BuffType<Jammed>());
                DebuffIDs.Add(ModContent.BuffType<Lethargic>());
                DebuffIDs.Add(ModContent.BuffType<LightningRod>());
                DebuffIDs.Add(ModContent.BuffType<LihzahrdCurse>());
                DebuffIDs.Add(ModContent.BuffType<LivingWasteland>());
                DebuffIDs.Add(ModContent.BuffType<Lovestruck>());
                DebuffIDs.Add(ModContent.BuffType<LowGround>());
                DebuffIDs.Add(ModContent.BuffType<MarkedforDeath>());
                DebuffIDs.Add(ModContent.BuffType<Midas>());
                DebuffIDs.Add(ModContent.BuffType<MutantNibble>());
                DebuffIDs.Add(ModContent.BuffType<NanoInjection>());
                DebuffIDs.Add(ModContent.BuffType<NullificationCurse>());
                DebuffIDs.Add(ModContent.BuffType<OceanicMaul>());
                DebuffIDs.Add(ModContent.BuffType<OceanicSeal>());
                DebuffIDs.Add(ModContent.BuffType<Oiled>());
                DebuffIDs.Add(ModContent.BuffType<Purified>());
                DebuffIDs.Add(ModContent.BuffType<Recovering>());
                DebuffIDs.Add(ModContent.BuffType<ReverseManaFlow>());
                DebuffIDs.Add(ModContent.BuffType<Rotting>());
                DebuffIDs.Add(ModContent.BuffType<Shadowflame>());
                DebuffIDs.Add(ModContent.BuffType<Buffs.Masomode.SqueakyToy>());
                DebuffIDs.Add(ModContent.BuffType<Stunned>());
                DebuffIDs.Add(ModContent.BuffType<Swarming>());
                DebuffIDs.Add(ModContent.BuffType<Unstable>());

                DebuffIDs.Add(ModContent.BuffType<AbomFang>());
                DebuffIDs.Add(ModContent.BuffType<AbomPresence>());
                DebuffIDs.Add(ModContent.BuffType<MutantFang>());
                DebuffIDs.Add(ModContent.BuffType<MutantPresence>());

                DebuffIDs.Add(ModContent.BuffType<AbomRebirth>());

                DebuffIDs.Add(ModContent.BuffType<TimeFrozen>());

                //Mod bossHealthBar = ModLoader.GetMod("FKBossHealthBar");
                //if (bossHealthBar != null)
                //{
                //    //bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<BabyGuardian>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<TimberChampion>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<TimberChampionHead>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<EarthChampion>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<LifeChampion>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<WillChampion>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<ShadowChampion>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<SpiritChampion>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<TerraChampion>());
                //    bossHealthBar.Call("RegisterHealthBarMini", ModContent.NPCType<NatureChampion>());

                //    bossHealthBar.Call("hbStart");
                //    bossHealthBar.Call("hbSetColours", new Color(1f, 1f, 1f), new Color(1f, 1f, 0.5f), new Color(1f, 0f, 0f));
                //    bossHealthBar.Call("hbFinishSingle", ModContent.NPCType<CosmosChampion>());

                //    bossHealthBar.Call("hbStart");
                //    bossHealthBar.Call("hbSetColours", new Color(1f, 0f, 1f), new Color(1f, 0.2f, 0.6f), new Color(1f, 0f, 0f));
                //    bossHealthBar.Call("hbFinishSingle", ModContent.NPCType<DeviBoss>());

                //    bossHealthBar.Call("RegisterDD2HealthBar", ModContent.NPCType<AbomBoss>());

                //    bossHealthBar.Call("hbStart");
                //    bossHealthBar.Call("hbSetColours", new Color(55, 255, 191), new Color(0f, 1f, 0f), new Color(0f, 0.5f, 1f));
                //    //bossHealthBar.Call("hbSetBossHeadTexture", GetTexture("NPCs/MutantBoss/MutantBoss_Head_Boss"));
                //    bossHealthBar.Call("hbSetTexture",
                //        bossHealthBar.GetTexture("UI/MoonLordBarStart"), null,
                //        bossHealthBar.GetTexture("UI/MoonLordBarEnd"), null);
                //    bossHealthBar.Call("hbSetTextureExpert",
                //        bossHealthBar.GetTexture("UI/MoonLordBarStart_Exp"), null,
                //        bossHealthBar.GetTexture("UI/MoonLordBarEnd_Exp"), null);
                //    bossHealthBar.Call("hbFinishSingle", ModContent.NPCType<MutantBoss>());
                //}

                //mutant shop
                Mod fargos = ModLoader.GetMod("Fargowiltas");
                fargos.Call("AddSummon", 5.01f, "FargowiltasSouls", "DevisCurse", () => FargoSoulsWorld.downedDevi, Item.buyPrice(0, 17, 50));
                fargos.Call("AddSummon", 14.01f, "FargowiltasSouls", "AbomsCurse", () => FargoSoulsWorld.downedAbom, 10000000);
                fargos.Call("AddSummon", 14.02f, "FargowiltasSouls", "TruffleWormEX", () => FargoSoulsWorld.downedFishronEX, 10000000);
                fargos.Call("AddSummon", 14.03f, "FargowiltasSouls", "MutantsCurse", () => FargoSoulsWorld.downedMutant, 20000000);
            }
            catch (Exception e)
            {
                Logger.Warn("FargowiltasSouls PostSetupContent Error: " + e.StackTrace + e.Message);
            }
        }

        public void ManageMusicTimestop(bool playMusicAgain)
        {
            if (Main.dedServ)
                return;

            if (playMusicAgain)
            {
                if (OldMusicFade > 0)
                {
                    Main.musicFade[Main.curMusic] = OldMusicFade;
                    OldMusicFade = 0;
                }
            }
            else
            {
                if (OldMusicFade == 0)
                {
                    OldMusicFade = Main.musicFade[Main.curMusic];
                }
                else
                {
                    for (int i = 0; i < Main.musicFade.Length; i++)
                        Main.musicFade[i] = 0f;
                }
            }
        }

        static float ColorTimer;
        public static Color EModeColor()
        {
            Color mutantColor = new Color(28, 222, 152);
            Color abomColor = new Color(255, 224, 53);
            Color deviColor = new Color(255, 51, 153);
            ColorTimer += 0.5f;
            if (ColorTimer >= 300)
            {
                ColorTimer = 0;
            }

            if (ColorTimer < 100)
                return Color.Lerp(mutantColor, abomColor, ColorTimer / 100);
            else if (ColorTimer < 200)
                return Color.Lerp(abomColor, deviColor, (ColorTimer - 100) / 100);
            else
                return Color.Lerp(deviColor, mutantColor, (ColorTimer - 200) / 100);
        }

        //        /*public void AddPartialRecipe(ModItem modItem, ModRecipe recipe, int tileType, int replacementItem)
        //        {
        //            RecipeGroup group = new RecipeGroup(() => $"{Lang.misc[37]} {modItem.DisplayName.GetDefault()} Material");
        //            foreach (Item i in recipe.requiredItem)
        //            {
        //                if (i == null || i.type == ItemID.None)
        //                    continue;
        //                group.ValidItems.Add(i.type);
        //            }
        //            string groupName = $"FargowiltasSouls:Any{modItem.Name}Material";
        //            RecipeGroup.RegisterGroup(groupName, group);

        //            ModRecipe partialRecipe = new ModRecipe(this);
        //            int originalItemsNeeded = group.ValidItems.Count / 2;
        //            partialRecipe.AddRecipeGroup(groupName, originalItemsNeeded);
        //            partialRecipe.AddIngredient(replacementItem, group.ValidItems.Count - originalItemsNeeded);
        //            partial.AddTile(tileType);
        //            partialRecipe.SetResult(modItem);
        //            partialRecipe.AddRecipe();
        //        }*/

        //        public override void AddRecipes()
        //        {
        //            ModRecipe recipe = new ModRecipe(this);
        //            .AddIngredient(ItemID.SoulofLight, 7)
        //            .AddIngredient(ItemID.SoulofNight, 7)
        //            .AddIngredient(ModContent.ItemType<Items.Misc.DeviatingEnergy>(), 5)
        //            .AddTile(TileID.MythrilAnvil)
        //            recipe.SetResult(ModContent.ItemType<JungleChest>());
        //            .Register();

        //            recipe = new ModRecipe(this);
        //            .AddIngredient(ItemID.WizardHat)
        //            .AddIngredient(ModContent.ItemType<Items.Misc.DeviatingEnergy>(), 5)
        //            .AddTile(TileID.MythrilAnvil)
        //            recipe.SetResult(ModContent.ItemType<RuneOrb>());
        //            .Register();

        //            recipe = new ModRecipe(this);
        //            .AddIngredient(ItemID.LifeCrystal)
        //            .AddTile(TileID.CookingPots)
        //            recipe.SetResult(ModContent.ItemType<HeartChocolate>());
        //            .Register();

        //            /*recipe = new ModRecipe(this);
        //            recipe.AddRecipeGroup("FargowiltasSouls:AnyBonesBanner", 2);
        //            .AddIngredient(ModContent.ItemType<Items.Misc.DeviatingEnergy>(), 5)
        //            .AddTile(TileID.Anvils)
        //            recipe.SetResult(ModContent.ItemType<InnocuousSkull>());
        //            .Register();*/
        //        }

        public override void AddRecipeGroups()
        {
            RecipeGroup group;

            //drax
            group = new RecipeGroup(() => Lang.misc[37] + " Drax", ItemID.Drax, ItemID.PickaxeAxe);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyDrax", group);

            //dungeon enemies
            group = new RecipeGroup(() => Lang.misc[37] + " Angry or Armored Bones Banner", ItemID.AngryBonesBanner, ItemID.BlueArmoredBonesBanner, ItemID.HellArmoredBonesBanner, ItemID.RustyArmoredBonesBanner);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyBonesBanner", group);

            //cobalt
            group = new RecipeGroup(() => Lang.misc[37] + " Cobalt Repeater", ItemID.CobaltRepeater, ItemID.PalladiumRepeater);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyCobaltRepeater", group);

            //mythril
            group = new RecipeGroup(() => Lang.misc[37] + " Mythril Repeater", ItemID.MythrilRepeater, ItemID.OrichalcumRepeater);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyMythrilRepeater", group);

            //adamantite
            group = new RecipeGroup(() => Lang.misc[37] + " Adamantite Repeater", ItemID.AdamantiteRepeater, ItemID.TitaniumRepeater);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAdamantiteRepeater", group);

            //evil wood
            group = new RecipeGroup(() => Lang.misc[37] + " Evil Wood", ItemID.Ebonwood, ItemID.Shadewood);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyEvilWood", group);

            //any adamantite
            group = new RecipeGroup(() => Lang.misc[37] + " Adamantite Bar", ItemID.AdamantiteBar, ItemID.TitaniumBar);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAdamantite", group);

            //shroomite head
            group = new RecipeGroup(() => Lang.misc[37] + " Shroomite Head Piece", ItemID.ShroomiteHeadgear, ItemID.ShroomiteMask, ItemID.ShroomiteHelmet);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyShroomHead", group);

            //orichalcum head
            group = new RecipeGroup(() => Lang.misc[37] + " Orichalcum Head Piece", ItemID.OrichalcumHeadgear, ItemID.OrichalcumMask, ItemID.OrichalcumHelmet);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyOriHead", group);

            //palladium head
            group = new RecipeGroup(() => Lang.misc[37] + " Palladium Head Piece", ItemID.PalladiumHeadgear, ItemID.PalladiumMask, ItemID.PalladiumHelmet);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyPallaHead", group);

            //cobalt head
            group = new RecipeGroup(() => Lang.misc[37] + " Cobalt Head Piece", ItemID.CobaltHelmet, ItemID.CobaltHat, ItemID.CobaltMask);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyCobaltHead", group);

            //mythril head
            group = new RecipeGroup(() => Lang.misc[37] + " Mythril Head Piece", ItemID.MythrilHat, ItemID.MythrilHelmet, ItemID.MythrilHood);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyMythrilHead", group);

            //titanium head
            group = new RecipeGroup(() => Lang.misc[37] + " Titanium Head Piece", ItemID.TitaniumHeadgear, ItemID.TitaniumMask, ItemID.TitaniumHelmet);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyTitaHead", group);

            //hallowed head
            group = new RecipeGroup(() => Lang.misc[37] + " Hallowed Head Piece", ItemID.HallowedMask, ItemID.HallowedHeadgear, ItemID.HallowedHelmet, ItemID.HallowedHood);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyHallowHead", group);

            //ancient hallow
            group = new RecipeGroup(() => Lang.misc[37] + " Ancient Hallowed Head Piece", ItemID.AncientHallowedHeadgear, ItemID.AncientHallowedHelmet, ItemID.AncientHallowedHood, ItemID.AncientHallowedMask);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAncientHallowHead", group);

            //adamantite head
            group = new RecipeGroup(() => Lang.misc[37] + " Adamantite Head Piece", ItemID.AdamantiteHelmet, ItemID.AdamantiteMask, ItemID.AdamantiteHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyAdamHead", group);

            //chloro head
            group = new RecipeGroup(() => Lang.misc[37] + " Chlorophyte Head Piece", ItemID.ChlorophyteMask, ItemID.ChlorophyteHelmet, ItemID.ChlorophyteHeadgear);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyChloroHead", group);

            //spectre head
            group = new RecipeGroup(() => Lang.misc[37] + " Spectre Head Piece", ItemID.SpectreHood, ItemID.SpectreMask);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnySpectreHead", group);

            //beetle body
            group = new RecipeGroup(() => Lang.misc[37] + " Beetle Body", ItemID.BeetleShell, ItemID.BeetleScaleMail);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyBeetle", group);

            //            //phasesabers
            //            group = new RecipeGroup(() => Lang.misc[37] + " Phasesaber", ItemID.RedPhasesaber, ItemID.BluePhasesaber, ItemID.GreenPhasesaber, ItemID.PurplePhasesaber, ItemID.WhitePhasesaber,
            //                ItemID.YellowPhasesaber);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyPhasesaber", group);

            //            //vanilla butterflies
            //            group = new RecipeGroup(() => Lang.misc[37] + " Butterfly", ItemID.JuliaButterfly, ItemID.MonarchButterfly, ItemID.PurpleEmperorButterfly,
            //                ItemID.RedAdmiralButterfly, ItemID.SulphurButterfly, ItemID.TreeNymphButterfly, ItemID.UlyssesButterfly, ItemID.ZebraSwallowtailButterfly);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyButterfly", group);

            //vanilla squirrels
            group = new RecipeGroup(() => Lang.misc[37] + " Squirrel", ItemID.Squirrel, ItemID.SquirrelRed);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnySquirrel", group);

            //            //vanilla fish
            //            group = new RecipeGroup(() => Lang.misc[37] + " Common Fish", ItemID.AtlanticCod, ItemID.Bass, ItemID.Trout, ItemID.RedSnapper, ItemID.Salmon, ItemID.Tuna);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyCommonFish", group);

            //vanilla birds
            group = new RecipeGroup(() => Lang.misc[37] + " Bird", ItemID.Bird, ItemID.BlueJay, ItemID.Cardinal, ItemID.GoldBird, ItemID.Duck, ItemID.MallardDuck, ItemID.Grebe, ItemID.Penguin, ItemID.Seagull);
            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyBird", group);

            //            //vanilla scorpions
            //            group = new RecipeGroup(() => Lang.misc[37] + " Scorpion", ItemID.Scorpion, ItemID.BlackScorpion);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyScorpion", group);

            //            //gold pick
            //            group = new RecipeGroup(() => Lang.misc[37] + " Gold Pickaxe", ItemID.GoldPickaxe, ItemID.PlatinumPickaxe);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyGoldPickaxe", group);

            //            //fish trash
            //            group = new RecipeGroup(() => Lang.misc[37] + " Fishing Trash", ItemID.OldShoe, ItemID.TinCan, ItemID.FishingSeaweed);
            //            RecipeGroup.RegisterGroup("FargowiltasSouls:AnyFishingTrash", group);


        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            switch (reader.ReadByte())
            {
                case 0: //server side spawning creepers
                    if (Main.netMode == NetmodeID.Server)
                    {
                        byte p = reader.ReadByte();
                        int multiplier = reader.ReadByte();
                        int n = NPC.NewNPC(NPC.GetSpawnSource_NPCRelease(p), (int)Main.player[p].Center.X, (int)Main.player[p].Center.Y, ModContent.NPCType<CreeperGutted>(), 0,
                            p, 0f, multiplier, 0);
                        if (n != Main.maxNPCs)
                        {
                            Main.npc[n].velocity = Vector2.UnitX.RotatedByRandom(2 * Math.PI) * 8;
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                        }
                    }
                    break;

                //case 1: //server side synchronize pillar data request
                //    if (Main.netMode == NetmodeID.Server)
                //    {
                //        byte pillar = reader.ReadByte();
                //        if (!Main.npc[pillar].GetGlobalNPC<EModeGlobalNPC>().masoBool[1])
                //        {
                //            Main.npc[pillar].GetGlobalNPC<EModeGlobalNPC>().masoBool[1] = true;
                //            Main.npc[pillar].GetGlobalNPC<EModeGlobalNPC>().SetDefaults(Main.npc[pillar]);
                //            Main.npc[pillar].life = Main.npc[pillar].lifeMax;
                //        }
                //    }
                //    break;

                //case 2: //net updating maso
                //    EModeGlobalNPC fargoNPC = Main.npc[reader.ReadByte()].GetGlobalNPC<EModeGlobalNPC>();
                //    fargoNPC.masoBool[0] = reader.ReadBoolean();
                //    fargoNPC.masoBool[1] = reader.ReadBoolean();
                //    fargoNPC.masoBool[2] = reader.ReadBoolean();
                //    fargoNPC.masoBool[3] = reader.ReadBoolean();
                //    fargoNPC.Counter[0] = reader.ReadInt32();
                //    fargoNPC.Counter[1] = reader.ReadInt32();
                //    fargoNPC.Counter[2] = reader.ReadInt32();
                //    fargoNPC.Counter[3] = reader.ReadInt32();
                //    break;

                //case 3: //rainbow slime/paladin, MP clients syncing to server
                //    if (Main.netMode == NetmodeID.MultiplayerClient)
                //    {
                //        byte npc = reader.ReadByte();
                //        Main.npc[npc].lifeMax = reader.ReadInt32();
                //        float newScale = reader.ReadSingle();
                //        Main.npc[npc].position = Main.npc[npc].Center;
                //        Main.npc[npc].width = (int)(Main.npc[npc].width / Main.npc[npc].scale * newScale);
                //        Main.npc[npc].height = (int)(Main.npc[npc].height / Main.npc[npc].scale * newScale);
                //        Main.npc[npc].scale = newScale;
                //        Main.npc[npc].Center = Main.npc[npc].position;
                //    }
                //    break;

                ///*case 4: //moon lord vulnerability synchronization
                //    if (Main.netMode == NetmodeID.MultiplayerClient)
                //    {
                //        int ML = reader.ReadByte();
                //        Main.npc[ML].GetGlobalNPC<EModeGlobalNPC>().Counter[0] = reader.ReadInt32();
                //        EModeGlobalNPC.masoStateML = reader.ReadByte();
                //    }
                //    break;*/

                //case 5: //retinazer laser MP sync
                //    if (Main.netMode == NetmodeID.MultiplayerClient)
                //    {
                //        int reti = reader.ReadByte();
                //        Main.npc[reti].GetGlobalNPC<EModeGlobalNPC>().masoBool[2] = reader.ReadBoolean();
                //        Main.npc[reti].GetGlobalNPC<EModeGlobalNPC>().Counter[0] = reader.ReadInt32();
                //    }
                //    break;

                //case 6: //shark MP sync
                //    if (Main.netMode == NetmodeID.MultiplayerClient)
                //    {
                //        int shark = reader.ReadByte();
                //        Main.npc[shark].GetGlobalNPC<EModeGlobalNPC>().SharkCount = reader.ReadByte();
                //    }
                //    break;

                //case 7: //client to server activate dark caster family
                //    if (Main.netMode == NetmodeID.Server)
                //    {
                //        int caster = reader.ReadByte();
                //        if (Main.npc[caster].GetGlobalNPC<EModeGlobalNPC>().Counter[1] == 0)
                //            Main.npc[caster].GetGlobalNPC<EModeGlobalNPC>().Counter[1] = reader.ReadInt32();
                //    }
                //    break;

                //case 8: //server to clients reset counter
                //    if (Main.netMode == NetmodeID.MultiplayerClient)
                //    {
                //        int caster = reader.ReadByte();
                //        Main.npc[caster].GetGlobalNPC<EModeGlobalNPC>().Counter[1] = 0;
                //    }
                //    break;

                case 9: //client to server, request heart spawn
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int p = reader.ReadByte();
                        int n = reader.ReadByte();
                        Item.NewItem(Main.player[p].GetItemSource_OnHit(Main.npc[n], ItemSourceID.None), Main.npc[n].Hitbox, ItemID.Heart);
                    }
                    break;

                case 10: //client to server, sync cultist data
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int cult = reader.ReadByte();

                        LunaticCultist cultist = Main.npc[cult].GetEModeNPCMod<LunaticCultist>();
                        cultist.MeleeDamageCounter += reader.ReadInt32();
                        cultist.RangedDamageCounter += reader.ReadInt32();
                        cultist.MagicDamageCounter += reader.ReadInt32();
                        cultist.MinionDamageCounter += reader.ReadInt32();
                    }
                    break;

                case 11: //refresh creeper
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        byte player = reader.ReadByte();
                        NPC creeper = Main.npc[reader.ReadByte()];
                        if (creeper.active && creeper.type == ModContent.NPCType<CreeperGutted>() && creeper.ai[0] == player)
                        {
                            int damage = creeper.lifeMax - creeper.life;
                            creeper.life = creeper.lifeMax;
                            if (damage > 0)
                                CombatText.NewText(creeper.Hitbox, CombatText.HealLife, damage);
                            if (Main.netMode == NetmodeID.Server)
                                creeper.netUpdate = true;
                        }
                    }
                    break;

                //case 12: //prime limbs spin
                //    if (Main.netMode == NetmodeID.MultiplayerClient)
                //    {
                //        int n = reader.ReadByte();
                //        EModeGlobalNPC limb = Main.npc[n].GetGlobalNPC<EModeGlobalNPC>();
                //        limb.masoBool[2] = reader.ReadBoolean();
                //        limb.Counter[0] = reader.ReadInt32();
                //        Main.npc[n].localAI[3] = reader.ReadSingle();
                //    }
                //    break;

                //case 13: //prime limbs swipe
                //    if (Main.netMode == NetmodeID.MultiplayerClient)
                //    {
                //        int n = reader.ReadByte();
                //        EModeGlobalNPC limb = Main.npc[n].GetGlobalNPC<EModeGlobalNPC>();
                //        limb.Counter[0] = reader.ReadInt32();
                //        limb.Counter[1] = reader.ReadInt32();
                //    }
                //    break;

                case 14: //devi gifts
                    if (Main.netMode == NetmodeID.Server)
                    {
                        Player player = Main.player[reader.ReadByte()];
                        DropDevianttsGift(player);
                    }
                    break;

                //case 15: //sync npc counter array
                //    if (Main.netMode == NetmodeID.MultiplayerClient)
                //    {
                //        int n = reader.ReadByte();
                //        EModeGlobalNPC eNPC = Main.npc[n].GetGlobalNPC<EModeGlobalNPC>();
                //        for (int i = 0; i < eNPC.Counter.Length; i++)
                //            eNPC.Counter[i] = reader.ReadInt32();
                //    }
                //    break;

                case 16: //client requesting a client side item from server
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int p = reader.ReadInt32();
                        int type = reader.ReadInt32();
                        int netID = reader.ReadInt32();
                        byte prefix = reader.ReadByte();
                        int stack = reader.ReadInt32();

                        int i = Item.NewItem(Main.player[p].GetItemSource_Misc(ItemSourceID.PlayerDropItemCheck), Main.player[p].Hitbox, type, stack, true, prefix);
                        Main.timeItemSlotCannotBeReusedFor[i] = 54000;

                        var netMessage = GetPacket();
                        netMessage.Write((byte)17);
                        netMessage.Write(p);
                        netMessage.Write(type);
                        netMessage.Write(netID);
                        netMessage.Write(prefix);
                        netMessage.Write(stack);
                        netMessage.Write(i);
                        netMessage.Send();

                        Main.item[i].active = false;
                    }
                    break;

                case 17: //client-server handshake, spawn client-side item
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        int p = reader.ReadInt32();
                        int type = reader.ReadInt32();
                        int netID = reader.ReadInt32();
                        byte prefix = reader.ReadByte();
                        int stack = reader.ReadInt32();
                        int i = reader.ReadInt32();

                        if (Main.myPlayer == p)
                        {
                            Main.item[i].netDefaults(netID);

                            Main.item[i].active = true;
                            //Main.item[i].spawnTime = 0;
                            Main.item[i].playerIndexTheItemIsReservedFor = p;

                            Main.item[i].Prefix(prefix);
                            Main.item[i].stack = stack;
                            Main.item[i].velocity.X = Main.rand.Next(-20, 21) * 0.2f;
                            Main.item[i].velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
                            Main.item[i].noGrabDelay = 100;
                            Main.item[i].newAndShiny = false;

                            Main.item[i].position = Main.player[p].position;
                            Main.item[i].position.X += Main.rand.NextFloat(Main.player[p].Hitbox.Width);
                            Main.item[i].position.Y += Main.rand.NextFloat(Main.player[p].Hitbox.Height);
                        }
                    }
                    break;

                //case 18: //client to server, requesting pillar sync
                //    if (Main.netMode == NetmodeID.Server)
                //    {
                //        int n = reader.ReadByte();
                //        int type = reader.ReadInt32();
                //        if (Main.npc[n].active && Main.npc[n].type == type)
                //        {
                //            Main.npc[n].GetGlobalNPC<EModeGlobalNPC>().NetUpdateMaso(n);
                //        }
                //    }
                //    break;

                /*case 19: //client to all others, synchronize extra updates
                    {
                        int p = reader.ReadInt32();
                        int type = reader.ReadInt32();
                        int extraUpdates = reader.ReadInt32();
                        if (Main.projectile[p].active && Main.projectile[p].type == type)
                            Main.projectile[p].extraUpdates = extraUpdates;
                    }
                    break;*/

                case 22: // New maso sync
                    {
                        int npcToSync = reader.ReadInt32();
                        Main.npc[npcToSync].GetGlobalNPC<NewEModeGlobalNPC>().NetRecieve(reader);
                    }
                    break;

                case 23: //sync friendly proj turned hostile
                    {
                        int owner = reader.ReadInt32();
                        int uuid = reader.ReadInt32();
                        int projType = reader.ReadInt32();

                        int byUUID = FargoSoulsUtil.GetByUUIDReal(owner, uuid, projType);
                        if (byUUID != -1)
                        {
                            Main.projectile[byUUID].hostile = true;
                            Main.projectile[byUUID].friendly = false;
                        }
                    }
                    break;

                case 77: //server side spawning fishron EX
                    if (Main.netMode == NetmodeID.Server)
                    {
                        byte target = reader.ReadByte();
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();
                        EModeGlobalNPC.spawnFishronEX = true;
                        NPC.NewNPC(NPC.GetBossSpawnSource(target), x, y, NPCID.DukeFishron, 0, 0f, 0f, 0f, 0f, target);
                        EModeGlobalNPC.spawnFishronEX = false;
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Duke Fishron EX has awoken!"), new Color(50, 100, 255));
                    }
                    break;

                case 78: //confirming fish EX max life
                    {
                        int f = reader.ReadInt32();
                        Main.npc[f].lifeMax = reader.ReadInt32();
                    }
                    break;

                case 79: //sync toggles on join
                    {
                        Player player = Main.player[reader.ReadByte()];
                        FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
                        byte count = reader.ReadByte();
                        List<string> keys = ToggleLoader.LoadedToggles.Keys.ToList();

                        for (int i = 0; i < count; i++)
                        {
                            modPlayer.Toggler.Toggles[keys[i]].ToggleBool = reader.ReadBoolean();
                        }
                    }
                    break;

                case 80: //sync single toggle
                    {
                        Player player = Main.player[reader.ReadByte()];
                        player.SetToggleValue(reader.ReadString(), reader.ReadBoolean());
                    }
                    break;

                case 81: //server acknowledges a CanPlayMaso player
                    if (Main.netMode == NetmodeID.Server)
                    {
                        FargoSoulsWorld.CanPlayMaso = reader.ReadBoolean();
                    }
                    break;

                default:
                    break;
            }

            //BaseMod Stuff
            /*MsgType msg = (MsgType)reader.ReadByte();
            if (msg == MsgType.ProjectileHostility) //projectile hostility and ownership
            {
                int owner = reader.ReadInt32();
                int projID = reader.ReadInt32();
                bool friendly = reader.ReadBoolean();
                bool hostile = reader.ReadBoolean();
                if (Main.projectile[projID] != null)
                {
                    Main.projectile[projID].owner = owner;
                    Main.projectile[projID].friendly = friendly;
                    Main.projectile[projID].hostile = hostile;
                }
                if (Main.netMode == NetmodeID.Server) MNet.SendBaseNetMessage(0, owner, projID, friendly, hostile);
            }
            else
            if (msg == MsgType.SyncAI) //sync AI array
            {
                int classID = reader.ReadByte();
                int id = reader.ReadInt16();
                int aitype = reader.ReadByte();
                int arrayLength = reader.ReadByte();
                float[] newAI = new float[arrayLength];
                for (int m = 0; m < arrayLength; m++)
                {
                    newAI[m] = reader.ReadSingle();
                }
                if (classID == 0 && Main.npc[id] != null && Main.npc[id].active && Main.npc[id].modNPC != null && Main.npc[id].modNPC is ParentNPC)
                {
                    ((ParentNPC)Main.npc[id].modNPC).SetAI(newAI, aitype);
                }
                else
                if (classID == 1 && Main.projectile[id] != null && Main.projectile[id].active && Main.projectile[id].modProjectile != null && Main.projectile[id].modProjectile is ParentProjectile)
                {
                    ((ParentProjectile)Main.projectile[id].modProjectile).SetAI(newAI, aitype);
                }
                if (Main.netMode == NetmodeID.Server) BaseNet.SyncAI(classID, id, newAI, aitype);
            }*/
        }

        //        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        //        {
        //            if (Main.musicVolume != 0 && Main.myPlayer != -1 && !Main.gameMenu && Main.LocalPlayer.active)
        //            {
        //                if (MMWorld.MMArmy && priority <= MusicPriority.Environment)
        //                {
        //                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/MonsterMadhouse");
        //                    priority = MusicPriority.Event;
        //                }
        //                /*if (FargoSoulsGlobalNPC.FargoSoulsUtil.BossIsAlive(ref FargoSoulsGlobalNPC.mutantBoss, ModContent.NPCType<NPCs.MutantBoss.MutantBoss>())
        //                    && Main.player[Main.myPlayer].Distance(Main.npc[FargoSoulsGlobalNPC.mutantBoss].Center) < 3000)
        //                {
        //                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/SteelRed");
        //                    priority = (MusicPriority)12;
        //                }*/
        //            }
        //        }

        public static bool NoInvasion(NPCSpawnInfo spawnInfo)
        {
            return !spawnInfo.invasion && (!Main.pumpkinMoon && !Main.snowMoon || spawnInfo.spawnTileY > Main.worldSurface || Main.dayTime) &&
                   (!Main.eclipse || spawnInfo.spawnTileY > Main.worldSurface || !Main.dayTime);
        }

        public static bool NoBiome(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.player;
            return !player.ZoneJungle && !player.ZoneDungeon && !player.ZoneCorrupt && !player.ZoneCrimson && !player.ZoneHallow && !player.ZoneSnow && !player.ZoneUndergroundDesert;
        }

        public static bool NoZoneAllowWater(NPCSpawnInfo spawnInfo)
        {
            return !spawnInfo.sky && !spawnInfo.player.ZoneMeteor && !spawnInfo.spiderCave;
        }

        public static bool NoZone(NPCSpawnInfo spawnInfo)
        {
            return NoZoneAllowWater(spawnInfo) && !spawnInfo.water;
        }

        public static bool NormalSpawn(NPCSpawnInfo spawnInfo)
        {
            return !spawnInfo.playerInTown && NoInvasion(spawnInfo);
        }

        public static bool NoZoneNormalSpawn(NPCSpawnInfo spawnInfo)
        {
            return NormalSpawn(spawnInfo) && NoZone(spawnInfo);
        }

        public static bool NoZoneNormalSpawnAllowWater(NPCSpawnInfo spawnInfo)
        {
            return NormalSpawn(spawnInfo) && NoZoneAllowWater(spawnInfo);
        }

        public static bool NoBiomeNormalSpawn(NPCSpawnInfo spawnInfo)
        {
            return NormalSpawn(spawnInfo) && NoBiome(spawnInfo) && NoZone(spawnInfo);
        }

        //        public override void UpdateUI(GameTime gameTime)
        //        {
        //            base.UpdateUI(gameTime);
        //            UserInterfaceManager.UpdateUI(gameTime);
        //        }

        //        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        //        {
        //            base.ModifyInterfaceLayers(layers);
        //            UserInterfaceManager.ModifyInterfaceLayers(layers);
        //        }
    }

    //    internal enum MsgType : byte
    //    {
    //        ProjectileHostility,
    //        SyncAI
    //    }
}