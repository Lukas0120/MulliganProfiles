using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartBot.Database;
using SmartBot.Mulligan;
using SmartBot.Plugins.API;

namespace SmartBotUI.Mulligan.Priority
{
    public static class Extension
    {
        public static void AddOrUpdate<TKey, TValue>(
            this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }
    }

    [Serializable]


    public class bMulliganProfile : MulliganProfile
    {
        private static bool hasCoin;
        //=================ADJUSTABLE MINION CAP===================//
        public const int Allowed1Drops = 1;
        public static readonly int Allowed2Drops = hasCoin ? 3 : 2;      //allow 3 on coin, 1 wihtout
        public static readonly int Allowed3Drops = hasCoin ? 2 : 1;      //allows 2 on coin, 1 without
        public const int Allowed4Drops = 1;


        //=========================================================//




        private Dictionary<string, bool> _whiteList; // CardName, KeepDouble
        private static Dictionary<string, int> _priorityList;
        private readonly List<Card.Cards> _cardsToKeep;
        private const string Coin = "GAME_005";
        #region allplayablecards
        private const string StoneclawTotem = "CS2_051";
        private const string Wolfrider = "CS2_124";
        private const string ShadowofNothing = "EX1_345t";
        private const string Frog = "hexfrog";
        private const string ArgentHorserider = "AT_087";
        private const string HomingChicken = "Mekka1";
        private const string MirrorImage = "CS2_mirror";
        private const string Shieldbearer = "EX1_405";
        private const string WrathofAirTotem = "CS2_052";
        private const string HealingTotem = "NEW1_009";
        private const string TargetDummy = "GVG_093";
        private const string BloodImp = "CS2_059";
        private const string MagmaRager = "CS2_118";
        private const string ScarletCrusader = "EX1_020";
        private const string Poultryizer = "Mekka4";
        private const string RepairBot = "Mekka2";
        private const string Emboldener3000 = "Mekka3";
        private const string NerubianEgg = "FP1_007";
        private const string FlametongueTotem = "EX1_565";
        private const string VitalityTotem = "GVG_039";
        private const string AlarmoBot = "EX1_006";
        private const string ManaTideTotem = "EX1_575";
        private const string LorewalkerCho = "EX1_100";
        private const string DustDevil = "EX1_243";
        private const string NatPagle = "EX1_557";
        private const string SummoningPortal = "EX1_315";
        private const string CaptainsParrot = "NEW1_016";
        private const string NoviceEngineer = "EX1_015";
        private const string GrimscaleOracle = "EX1_508";
        private const string Lightwell = "EX1_341";
        private const string BloodmageThalnos = "EX1_012";
        private const string Slime = "FP1_012t";
        private const string TimberWolf = "DS1_175";
        private const string Hound = "EX1_538t";
        private const string GoldshireFootman = "CS1_042";
        private const string ExplosiveSheep = "GVG_076";
        private const string Lightspawn = "EX1_335";
        private const string KvaldirRaider = "AT_119";
        private const string AncientofLore = "NEW1_008";
        private const string AlAkirtheWindlord = "NEW1_010";
        private const string CaptainGreenskin = "NEW1_024";
        private const string Deathwing = "NEW1_030";
        private const string Gruul = "NEW1_038";
        private const string JusticarTrueheart = "AT_132";
        private const string StampedingKodo = "NEW1_041";
        private const string GrandCrusader = "AT_118";
        private const string GelbinMekkatorque = "EX1_112";
        private const string Recruiter = "AT_113";
        private const string MasterJouster = "AT_112";
        private const string ArgentWatchman = "AT_109";
        private const string TuskarrJouster = "AT_104";
        private const string EliteTaurenChieftain = "PRO_001";
        private const string SpectralSpider = "FP1_002t";
        private const string NorthSeaKraken = "AT_103";
        private const string SpectralKnight = "FP1_008";
        private const string Maexxna = "FP1_010";
        private const string SludgeBelcher = "FP1_012";
        private const string CapturedJormungar = "AT_102";
        private const string KelThuzad = "FP1_013";
        private const string Stalagg = "FP1_014";
        private const string Thaddius = "FP1_014t";
        private const string Feugen = "FP1_015";
        private const string Loatheb = "FP1_030";
        private const string FlameLeviathan = "GVG_007";
        private const string Shadowbomber = "GVG_009";
        private const string Voljin = "GVG_014";
        private const string Kidnapper = "NEW1_005";
        private const string Infernal = "EX1_tk34";
        private const string Devilsaur = "EX1_tk29";
        private const string Squirrel = "EX1_tk28";
        private const string SeaReaver = "AT_130";
        private const string TheSkeletonKnight = "AT_128";
        private const string ProphetVelen = "EX1_350";
        private const string TirionFordring = "EX1_383";
        private const string NexusChampionSaraad = "AT_127";
        private const string GrommashHellscream = "EX1_414";
        private const string MurlocTidecaller = "EX1_509";
        private const string PatientAssassin = "EX1_522";
        private const string SavannahHighmane = "EX1_534";
        private const string Icehowl = "AT_125";
        private const string KingKrush = "EX1_543";
        private const string BolfRamshield = "AT_124";
        private const string HarrisonJones = "EX1_558";
        private const string Chillmaw = "AT_123";
        private const string FelReaver = "GVG_016";
        private const string ArchmageAntonidas = "EX1_559";
        private const string Alexstrasza = "EX1_561";
        private const string Onyxia = "EX1_562";
        private const string Malygos = "EX1_563";
        private const string FacelessManipulator = "EX1_564";
        private const string Ysera = "EX1_572";
        private const string Cenarius = "EX1_573";
        private const string FrostGiant = "AT_120";
        private const string TheBeast = "EX1_577";
        private const string PriestessofElune = "EX1_583";
        private const string SeaGiant = "EX1_586";
        private const string IllidanStormrage = "EX1_614";
        private const string MoltenGiant = "EX1_620";
        private const string TempleEnforcer = "EX1_623";
        private const string Nozdormu = "EX1_560";
        private const string PitFighter = "AT_101";
        private const string MalGanis = "GVG_021";
        private const string TradePrinceGallywix = "GVG_028";
        private const string DrBoom = "GVG_110";
        private const string MimironsHead = "GVG_111";
        private const string V07TR0N = "GVG_111t";
        private const string MysteriousChallenger = "AT_079";
        private const string VarianWrynn = "AT_072";
        private const string MogortheOgre = "GVG_112";
        private const string FoeReaper4000 = "GVG_113";
        private const string SneedsOldShredder = "GVG_114";
        private const string Toshley = "GVG_115";
        private const string MekgineerThermaplugg = "GVG_116";
        private const string Gazlowe = "GVG_117";
        private const string TroggzortheEarthinator = "GVG_118";
        private const string Blingtron3000 = "GVG_119";
        private const string HemetNesingwary = "GVG_120";
        private const string Junkbot = "GVG_106";
        private const string SkycapnKragg = "AT_070";
        private const string ColdarraDrake = "AT_008";
        private const string Acidmaw = "AT_063";
        private const string Rhonin = "AT_009";
        private const string RamWrangler = "AT_010";
        private const string ConfessorPaletress = "AT_018";
        private const string BraveArcher = "AT_059";
        private const string FearsomeDoomguard = "AT_020";
        private const string VoidCrusher = "AT_023";
        private const string WilfredFizzlebang = "AT_027";
        private const string ShadoPanRider = "AT_028";
        private const string Anubarak = "AT_036";
        private const string Sapling = "AT_037t";
        private const string KnightoftheWild = "AT_041";
        private const string Aviana = "AT_045";
        private const string ClockworkGiant = "GVG_121";
        private const string TheMistcaller = "AT_054";
        private const string EadricthePure = "AT_081";
        private const string PilotedSkyGolem = "GVG_105";
        private const string MechBearCat = "GVG_034";
        private const string WarKodo = "AT_099t";
        private const string Malorne = "GVG_035";
        private const string Kodorider = "AT_099";
        private const string Neptulon = "GVG_042";
        private const string Imp = "GVG_045t";
        private const string SideshowSpelleater = "AT_098";
        private const string KingofBeasts = "GVG_046";
        private const string Gahzrilla = "GVG_049";
        private const string Shieldmaiden = "GVG_053";
        private const string IronJuggernaut = "GVG_056";
        private const string Quartermaster = "GVG_060";
        private const string CobaltGuardian = "GVG_062";
        private const string BolvarFordragon = "GVG_063";
        private const string MogorsChampion = "AT_088";
        private const string AntiqueHealbot = "GVG_069";
        private const string TournamentAttendee = "AT_097";
        private const string AnimaGolem = "GVG_077";
        private const string ForceTankMAX = "GVG_079";
        private const string DruidoftheFang = "GVG_080";
        private const string UpgradedRepairBot = "GVG_083";
        private const string SiegeEngine = "GVG_086";
        private const string OgreNinja = "GVG_088";
        private const string ClockworkKnight = "AT_096";
        private const string MadderBomber = "GVG_090";
        private const string MuklasChampion = "AT_090";
        private const string BombLobber = "GVG_099";
        private const string FloatingWatcher = "GVG_100";
        private const string SaltyDog = "GVG_070";
        private const string ThunderBluffValiant = "AT_049";
        private const string Hogger = "NEW1_040";
        private const string DruidoftheClaw = "EX1_165";
        private const string BoulderfistOgre = "CS2_200";
        private const string GadgetzanAuctioneer = "EX1_095";
        private const string SpitefulSmith = "CS2_221";
        private const string StarvingBuzzard = "CS2_237";
        private const string LordJaraxxus = "EX1_323";
        private const string VentureCoMercenary = "CS2_227";
        private const string WarGolem = "CS2_186";
        private const string LordoftheArena = "CS2_162";
        private const string WindfuryHarpy = "EX1_033";
        private const string IronbarkProtector = "CS2_232";
        private const string BootyBayBodyguard = "CS2_187";
        private const string AngryChicken = "EX1_009";
        private const string FrostwolfWarlord = "CS2_226";
        private const string Whelp = "EX1_116t";
        private const string StormwindChampion = "CS2_222";
        private const string SylvanasWindrunner = "EX1_016";
        private const string LeeroyJenkins = "EX1_116";
        private const string RecklessRocketeer = "CS2_213";
        private const string CoreHound = "CS2_201";
        private const string CairneBloodhoof = "EX1_110";
        private const string MountainGiant = "EX1_105";
        private const string Sunwalker = "EX1_032";
        private const string StranglethornTiger = "EX1_028";
        private const string TheBlackKnight = "EX1_002";
        private const string Abomination = "EX1_097";
        private const string StormpikeCommando = "CS2_150";
        private const string Nightblade = "EX1_593";
        private const string DreadInfernal = "CS2_064";
        private const string StonetuskBoar = "CS2_171";
        private const string GurubashiBerserker = "EX1_399";
        private const string Archmage = "CS2_155";
        private const string GuardianofKings = "CS2_088";
        private const string Skeleton = "skele11";
        private const string CabalShadowPriest = "EX1_091";
        private const string DarkscaleHealer = "DS1_055";
        private const string SilverHandRecruit = "CS2_101t";
        private const string FenCreeper = "CS1_069";
        private const string Doomguard = "EX1_310";
        private const string RagnarostheFirelord = "EX1_298";
        private const string FireElemental = "CS2_042";
        private const string FrostElemental = "EX1_283";
        private const string ArgentCommander = "EX1_067";
        private const string SilverHandKnight = "CS2_151";
        private const string RavenholdtAssassin = "CS2_161";
        private const string YoungDragonhawk = "CS2_169";
        private const string TundraRhino = "DS1_178";
        private const string WorthlessImp = "EX1_317t";
        private const string EarthElemental = "EX1_250";
        private const string BaronGeddon = "EX1_249";
        private const string AncientofWar = "EX1_178";
        private const string AzureDrake = "EX1_284";
        private const string SearingTotem = "CS2_050";
        private const string HungryCrab = "NEW1_017";
        private const string Undertaker = "FP1_028";
        private const string Secretkeeper = "EX1_080";
        private const string Doomsayer = "NEW1_021";
        private const string ElvenArcher = "CS2_189";
        private const string LowlySquire = "AT_082";
        private const string MurlocRaider = "CS2_168";
        private const string InjuredKvaldir = "AT_105";
        private const string YoungPriestess = "EX1_004";
        private const string BoomBot = "GVG_110t";
        private const string Defender = "EX1_130a";
        private const string Lightwarden = "EX1_001";
        private const string Dreadsteed = "AT_019";
        private const string DamagedGolem = "skele21";
        private const string AvataroftheCoin = "GAME_002";
        private const string MechanicalDragonling = "EX1_025t";
        private const string VoodooDoctor = "EX1_011";
        private const string SouthseaDeckhand = "CS2_146";
        private const string Sheep = "CS2_tk1";
        private const string Chicken = "Mekka4t";
        private const string Cogmaster = "GVG_013";
        private const string Murloc = "PRO_001at";
        private const string Webspinner = "FP1_011";
        private const string Wisp = "CS2_231";
        private const string Warbot = "GVG_051";
        private const string AbusiveSergeant = "CS2_188";
        private const string NorthshireCleric = "CS2_235";
        private const string MurlocScout = "EX1_506a";
        private const string Voidwalker = "CS2_065";
        private const string ManaWyrm = "NEW1_012";
        private const string VioletApprentice = "NEW1_026t";
        private const string FlameofAzzinoth = "EX1_614t";
        private const string MasterSwordsmith = "NEW1_037";
        private const string QuestingAdventurer = "EX1_044";
        private const string LeperGnome = "EX1_029";
        private const string SabertoothLion = "AT_042t";
        private const string IronSensei = "GVG_027";
        private const string AcolyteofPain = "EX1_007";
        private const string Buccaneer = "AT_029";
        private const string DefiasBandit = "EX1_131t";
        private const string ClockworkGnome = "GVG_082";
        private const string Hobgoblin = "GVG_104";
        private const string ManaAddict = "EX1_055";
        private const string ShadeofNaxxramas = "FP1_005";
        private const string RaidLeader = "CS2_122";
        private const string GadgetzanJouster = "AT_133";
        private const string BluegillWarrior = "CS2_173";
        private const string BloodsailCorsair = "NEW1_025";
        private const string LootHoarder = "EX1_096";
        private const string Armorsmith = "EX1_402";
        private const string EmperorCobra = "EX1_170";
        private const string Jeeves = "GVG_094";
        private const string Demolisher = "EX1_102";
        private const string SouthseaCaptain = "NEW1_027";
        private const string SilverbackPatriarch = "CS2_127";
        private const string WorgenInfiltrator = "EX1_010";
        private const string ThrallmarFarseer = "EX1_021";
        private const string MistressofPain = "GVG_018";
        private const string DruidoftheSaber = "AT_042";
        private const string MurlocWarleader = "EX1_507";
        private const string OneeyedCheat = "GVG_025";
        private const string FlyingMachine = "GVG_084";
        private const string StoneskinGargoyle = "FP1_027";
        private const string GnomereganInfantry = "GVG_098";
        private const string UnstableGhoul = "FP1_024";
        private const string FencingCoach = "AT_115";
        private const string ColdlightOracle = "EX1_050";
        private const string DalaranMage = "EX1_582";
        private const string TaurenWarrior = "EX1_390";
        private const string IronbeakOwl = "CS2_203";
        private const string MurlocTidehunter = "EX1_506";
        private const string WarsongCommander = "EX1_084";
        private const string EnhanceoMechano = "GVG_107";
        private const string FlesheatingGhoul = "tt_004";
        private const string EdwinVanCleef = "EX1_613";
        private const string TwilightDrake = "EX1_043";
        private const string Spellbender = "tt_010a";
        private const string IronforgeRifleman = "CS2_141";
        private const string NerubarWeblord = "FP1_017";
        private const string MicroMachine = "GVG_103";
        private const string Shadowfiend = "AT_014";
        private const string OldMurkEye = "EX1_062";
        private const string KeeperoftheGrove = "EX1_166";
        private const string UnboundElemental = "EX1_258";
        private const string Squire = "CS2_152";
        private const string EtherealArcanist = "EX1_274";
        private const string MasterofCeremonies = "AT_117";
        private const string DireWolfAlpha = "EX1_162";
        private const string Boar = "AT_005t";
        private const string RagingWorgen = "EX1_412";
        private const string Hyena = "EX1_534t";
        private const string ScavengingHyena = "EX1_531";
        private const string Illuminator = "GVG_089";
        private const string ShatteredSunCleric = "EX1_019";
        private const string GnomishExperimenter = "GVG_092";
        private const string Huffer = "NEW1_034";
        private const string GoblinSapper = "GVG_095";
        private const string Leokk = "NEW1_033";
        private const string Succubus = "EX1_306";
        private const string ArathiWeaponsmith = "EX1_398";
        private const string MiniMage = "GVG_109";
        private const string ArgentProtector = "EX1_362";
        private const string Windspeaker = "EX1_587";
        private const string SootSpewer = "GVG_123";
        private const string SI7Agent = "EX1_134";
        private const string ImpMaster = "EX1_597";
        private const string WyrmrestAgent = "AT_116";
        private const string GroveTender = "GVG_032";
        private const string AnodizedRoboCub = "GVG_030";
        private const string ManaWraith = "EX1_616";
        private const string DefenderofArgus = "EX1_093";
        private const string IronfurGrizzly = "CS2_125";
        private const string TuskarrTotemic = "AT_046";
        private const string Stablemaster = "AT_057";
        private const string OrgrimmarAspirant = "AT_066";
        private const string KoboldGeomancer = "CS2_142";
        private const string HauntedCreeper = "FP1_002";
        private const string SilverHandRegent = "AT_100";
        private const string WarhorseTrainer = "AT_075";
        private const string DreadCorsair = "NEW1_022";
        private const string DragonhawkRider = "AT_083";
        private const string LanceCarrier = "AT_084";
        private const string CrazedAlchemist = "EX1_059";
        private const string Gnoll = "NEW1_040t";
        private const string EchoingOoze = "FP1_003";
        private const string Cutpurse = "AT_031";
        private const string ArgentSquire = "EX1_008";
        private const string DefiasRingleader = "EX1_131";
        private const string PintSizedSummoner = "EX1_076";
        private const string BigGameHunter = "EX1_005";
        private const string CruelTaskmaster = "EX1_603";
        private const string FrothingBerserker = "EX1_604";
        private const string FrostwolfGrunt = "CS2_121";
        private const string BloodKnight = "EX1_590";
        private const string CultMaster = "EX1_595";
        private const string MadScientist = "FP1_004";
        private const string SilentKnight = "AT_095";
        private const string SparringPartner = "AT_069";
        private const string MurlocKnight = "AT_076";
        private const string GarrisonCommander = "AT_080";
        private const string TinyKnightofEvil = "AT_021";
        private const string IceRager = "AT_092";
        private const string Dreadscale = "AT_063t";
        private const string BloodfenRaptor = "CS2_172";
        private const string StormwindKnight = "CS2_131";
        private const string RazorfenHunter = "CS2_196";
        private const string DragonlingMechanic = "EX1_025";
        private const string KingsElekk = "AT_058";
        private const string Houndmaster = "DS1_070";
        private const string GnomishInventor = "CS2_147";
        private const string RiverCrocolisk = "CS2_120";
        private const string BoneguardLieutenant = "AT_089";
        private const string UndercityValiant = "AT_030";
        private const string WeeSpellstopper = "GVG_122";
        private const string FallenHero = "AT_003";
        private const string AcidicSwampOoze = "EX1_066";
        private const string ColiseumManager = "AT_110";
        private const string Puddlestomper = "GVG_064";
        private const string KorkronElite = "NEW1_011";
        private const string SiltfinSpiritwalker = "GVG_040";
        private const string WhirlingZapomatic = "GVG_037";
        private const string GoblinAutoBarber = "GVG_023";
        private const string JunglePanther = "EX1_017";
        private const string Shrinkmeister = "GVG_011";
        private const string Snowchugger = "GVG_002";
        private const string Recombobulator = "GVG_108";
        private const string Spellbreaker = "EX1_048";
        private const string YouthfulBrewmaster = "EX1_049";
        private const string MadBomber = "EX1_082";
        private const string Pirate = "TB_015";
        private const string MindControlTech = "EX1_085";
        private const string ArcaneGolem = "EX1_089";
        private const string FaerieDragon = "NEW1_023";
        private const string WildPyromancer = "NEW1_020";
        private const string KnifeJuggler = "NEW1_019";
        private const string ColdlightSeer = "EX1_103";
        private const string SpiritWolf = "EX1_tk11";
        private const string SorcerersApprentice = "EX1_608";
        private const string Panther = "EX1_160t";
        private const string HarvestGolem = "EX1_556";
        private const string AmaniBerserker = "EX1_393";
        private const string AldorPeacekeeper = "EX1_382";
        private const string SpiderTank = "GVG_044";
        private const string MetaltoothLeaper = "GVG_048";
        private const string FlameImp = "EX1_319";
        private const string TinkertownTechnician = "GVG_102";
        private const string PilotedShredder = "GVG_096";
        private const string ArcaneNullifierX21 = "GVG_091";
        private const string ShipsCannon = "GVG_075";
        private const string EarthenRingFarseer = "CS2_117";
        private const string LilExorcist = "GVG_097";
        private const string KezanMystic = "GVG_074";
        private const string Shadowboxer = "GVG_072";
        private const string StonesplinterTrogg = "GVG_067";
        private const string SteamwheedleSniper = "GVG_087";
        private const string AnnoyoTron = "GVG_085";
        private const string ArmoredWarhorse = "AT_108";
        private const string AncientWatcher = "EX1_045";
        private const string KirinTorMage = "EX1_612";
        private const string AlexstraszasChampion = "AT_071";
        private const string SenjinShieldmasta = "CS2_179";
        private const string DarkIronDwarf = "EX1_046";
        private const string MagnataurAlpha = "AT_067";
        private const string LightsChampion = "AT_106";
        private const string Saboteur = "AT_086";
        private const string SunfuryProtector = "EX1_058";
        private const string MasterofDisguise = "NEW1_014";
        private const string TinkmasterOverspark = "EX1_083";
        private const string FrigidSnobold = "AT_093";
        private const string FlameJuggler = "AT_094";
        private const string BloodsailRaider = "NEW1_018";
        private const string DalaranAspirant = "AT_006";
        private const string MaidenoftheLake = "AT_085";
        private const string HolyChampion = "AT_011";
        private const string GilblinStalker = "GVG_081";
        private const string VioletTeacher = "NEW1_026";
        private const string ScrewjankClunker = "GVG_055";
        private const string EydisDarkbane = "AT_131";
        private const string ShadyDealer = "AT_032";
        private const string BurlyRockjawTrogg = "GVG_068";
        private const string FjolaLightbane = "AT_129";
        private const string ScarletPurifier = "GVG_101";
        private const string MogushanWarden = "EX1_396";
        private const string Wildwalker = "AT_040";
        private const string VoidTerror = "EX1_304";
        private const string GormoktheImpaler = "AT_122";
        private const string Voidcaller = "FP1_022";
        private const string FelCannon = "GVG_020";
        private const string SilvermoonGuardian = "EX1_023";
        private const string DraeneiTotemcarver = "AT_047";
        private const string CrowdFavorite = "AT_121";
        private const string OgreMagi = "CS2_197";
        private const string AncientMage = "EX1_584";
        private const string BaronRivendare = "FP1_031";
        private const string AuchenaiSoulpriest = "EX1_591";
        private const string SabertoothPanther = "AT_042t2";
        private const string BaineBloodhoof = "EX1_110t";
        private const string RefreshmentVendor = "AT_111";
        private const string OasisSnapjaw = "CS2_119";
        private const string WaterElemental = "CS2_033";
        private const string EvilHeckler = "AT_114";
        private const string ZombieChow = "FP1_001";
        private const string WailingSoul = "FP1_016";
        private const string GoblinBlastmage = "GVG_004";
        private const string Spellslinger = "AT_007";
        private const string TournamentMedic = "AT_091";
        private const string DunemaulShaman = "GVG_066";
        private const string AncientBrewmaster = "EX1_057";
        private const string Deathcharger = "FP1_006";
        private const string DarnassusAspirant = "AT_038";
        private const string SavageCombatant = "AT_039";
        private const string ChillwindYeti = "CS2_182";
        private const string DarkCultist = "FP1_023";
        private const string LostTallstrider = "GVG_071";
        private const string TwilightGuardian = "AT_017";
        private const string SpawnofShadows = "AT_012";
        private const string Mechwarper = "GVG_006";
        private const string OgreBrute = "GVG_065";
        private const string MechanicalYeti = "GVG_078";
        private const string FinkleEinhorn = "EX1_finkle";
        private const string Misha = "NEW1_032";
        private const string ShieldedMinibot = "GVG_058";
        private const string TotemGolem = "AT_052";
        private const string Felguard = "EX1_301";
        private const string DancingSwords = "FP1_029";
        private const string Wrathguard = "AT_026";
        private const string MillhouseManastorm = "NEW1_029";
        private const string AnubarAmbusher = "FP1_026";
        private const string Deathlord = "FP1_009";
        private const string KingMukla = "EX1_014";
        private const string PitLord = "EX1_313";
        private const string InjuredBlademaster = "CS2_181";
        #endregion
        /*Very experimental, and will not work until I feel comfortable with them*/
        public bool _arenaMode = true; //do not touch
        private static double _averageAggro = 3.5;
        private const double swapChance = 20.00; //Minimum draw chance of a better card 


        /*========================END OF DEFINITION=======================*/

        private static int _num2Drops;
        private static int _num3Drops;
        private static bool has1Drop;
        private static bool has2Drop;
        private static bool has3Drop;
        private static bool has4Drop;


        public bMulliganProfile()
        {
            _whiteList = new Dictionary<string, bool>();
            _priorityList = new Dictionary<string, int>();
            _cardsToKeep = new List<Card.Cards>();
        }

        public static double AverageAggro
        {
            get { return _averageAggro; }
            set { _averageAggro = value; }
        }

        public List<Card.Cards> HandleMulligan(List<Card.Cards> Choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            hasCoin = Choices.Count > 3;
            var hand = HandleMinions(Choices, _whiteList, opponentClass, ownClass);
            _whiteList.AddOrUpdate(Coin, true);

            _whiteList.AddOrUpdate(HandleWeapons(Choices, hand), false); // only 1 weapon is allowed
            _whiteList.AddOrUpdate(HandleSpells(Choices, hand), false); // only 1 spell is allowed

            using (
                StreamWriter file =
                    new StreamWriter(
                        AppDomain.CurrentDomain.BaseDirectory + "\\MulliganProfiles\\MulliganChoicesArchive.txt", true))
            {
                file.WriteLine("Your Card options against " + opponentClass + " as a " + ownClass +" were:");
                foreach (var q in Choices)
                    file.WriteLine(CardTemplate.LoadFromId(q.ToString()).Cost + " mana card: " +
                                   CardTemplate.LoadFromId(q.ToString()).Name);
                file.WriteLine("");
                file.WriteLine("Mulligan pick the following cards to keep ");
                foreach (var s in from s in Choices
                                  let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString())
                                  where _whiteList.ContainsKey(s.ToString())
                                  where !keptOneAlready | _whiteList[s.ToString()]
                                  select s)
                {
                    file.WriteLine(CardTemplate.LoadFromId(s.ToString()).Cost + " mana card: " +
                                   CardTemplate.LoadFromId(s.ToString()).Name);
                    _cardsToKeep.Add(s);
                }
                file.WriteLine(
                    "|");
                file.WriteLine(
                    "=====COMMENT SECTION[Agree? No? Why?]:");
                file.WriteLine("=====:");
                file.WriteLine(
                    "========================================------------");

                file.WriteLine("--------------------------------------");
                file.WriteLine("Your average deck mana cost is " +
                               Bot.CurrentDeck().Cards.Average(c => CardTemplate.LoadFromId(c).Cost));
                file.WriteLine("--------------------------------------");
                file.WriteLine("----Your alternative options in your deck were: ");
                var allOneDrops = (from q in Bot.CurrentDeck().Cards
                                   select CardTemplate.LoadFromId(q)
                                       into qq
                                       where qq.Cost == 1 && !qq.IsSecret
                                       select qq).ToList();
                var allTwoDrops = (from q in Bot.CurrentDeck().Cards
                                   select CardTemplate.LoadFromId(q)
                                       into qq
                                       where qq.Cost == 2 && !qq.IsSecret
                                       select qq).ToList();
                var allThreeDrops = (from q in Bot.CurrentDeck().Cards
                                     select CardTemplate.LoadFromId(q)
                                         into qq
                                         where qq.Cost == 3 && !qq.IsSecret
                                         select qq).ToList();
                var allFourDrops = (from q in Bot.CurrentDeck().Cards
                                    select CardTemplate.LoadFromId(q)
                                        into qq
                                        where qq.Cost == 4 && !qq.IsSecret
                                        select qq).ToList();
                file.WriteLine("\n------------One Drops:--");
                file.WriteLine(" ");
                var count = 0;
                foreach (var c in allOneDrops)
                {
                    if (count == 3)
                    {
                        file.WriteLine(" ");
                        count = 0;
                    }
                    file.Write("\"{0}[{1}/{2}]\", ", c.Name, c.Atk, c.Health);
                    count++;
                }
                count = 0;
                file.WriteLine("__");
                file.WriteLine("\n------------Two Drops:--");
                
                foreach (var c in allTwoDrops)
                {
                    if (count == 3)
                    {
                        file.WriteLine(" ");
                        count = 0;
                    }
                    file.Write("\"{0}[{1}/{2}]\", ", c.Name, c.Atk, c.Health);
                    count++;
                }
                count = 0;
                file.WriteLine("__");
                file.WriteLine("\n------------Three Drops:--");
                
                foreach (var c in allThreeDrops)
                {
                    if (count == 3)
                    {
                        file.WriteLine(" ");
                        count = 0;
                    }
                    file.Write("\"{0}[{1}/{2}]\", ", c.Name, c.Atk, c.Health);
                    count++;
                }
                count = 0;
                file.WriteLine("__");
                file.WriteLine("------------Four Drops:--");
                
                foreach (var c in allFourDrops)
                {
                    if (count == 3)
                    {
                        file.WriteLine(" ");
                        count = 0;
                    }
                    file.Write("\"{0}[{1}/{2}]\", ", c.Name, c.Atk, c.Health);
                    count++;
                }
                var yourDeck = Bot.CurrentDeck().Cards.ToList();
                file.WriteLine("\n\n================YOUR DECK Strings================\n");
                file.WriteLine("Import mulligan debug ");
                foreach (var w in yourDeck)
                {
                    var t = CardTemplate.LoadFromId(w);
                    file.Write("\"{0}\", ", t.Id);
                }
                file.WriteLine("\n Import mulligan tester ");
                foreach (var w in yourDeck)
                {
                    var t = CardTemplate.LoadFromId(w);
                    file.Write("{0};", t.Id);
                }
                file.WriteLine("");
                file.WriteLine(
                    "|");
                file.WriteLine(
                    "|");
                file.WriteLine("Bot finished a game against " + opponentClass + " as a " + ownClass);
                file.WriteLine(
                    "============================================================================================");
                file.WriteLine(
                    "=======================================NEW RUN==============================================");
                file.Close();

            }

            return _cardsToKeep;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="hand">
        /// Sends a tuple with the following information:
        /// Whitelisted minions, hasOneManaDrop, hasTwoManaDrop, hasThreeManaDrop and hasFourManaDrop
        /// </param>
        /// <returns></returns>
        private static string HandleSpells(List<Card.Cards> choices, Tuple<bool, bool, bool, bool> hand)
        {
            var allowedSpells = new List<string>
            {
                /*
                 * Mage: Frostbolt, Flamecannon, Unstable Portal, Arcane Missiles, Mirror Image
                 * Shaman: Rockbiter Weapon, Feral Spirit
                 * Priest: Holy Smite, Velens Chosen, Thoughtsteal
                 * 
                 * Paladin: Noble Sac, Avenge, Seal of the Champion, Mustard for Butter
                 * Warrior: Bash, Slam, Shield Block
                 * Warlock: Mortal Coin, Darkbomb, ImpLosion
                 * 
                 * Hunter: Tracking, Animal Companion, Secrets, Quick Shot, unleash
                 * Rogue: Deadly Poison, Burgle, Beneath the Grounds, Backstab
                 * Druid: Innervate, Wild Growth, Living Roots, Power of the Wild, Wrath
                 */
                "EX1_277",
                "GVG_001",
                "CS2_024",
                "CS2_027",
                "GVG_003",
                "CS2_045",
                "EX1_248",
                "CS1_130",
                "GVG_010",
                "EX1_339",
                "EX1_130",
                "FP1_020",
                "AT_074",
                "GVG_061",
                "AT_064",
                "EX1_391",
                "EX1_606",
                "EX1_302",
                "GVG_015",
                "GVG_045",
                "DS1_184",
                "NEW1_031",
                "BRM_013",
                "EX1_538",
                "CS2_074",
                "AT_033",
                "AT_035",
                "CS2_072",
                "EX1_169",
                "CS2_013",
                "AT_037",
                "EX1_160",
                "EX1_154"
            };
            var badSecrets = new List<string>
                /*Bad secrets
             *Hunter: Snipe, Misdirection
             *Mage: Spellbender, Counterspell, Vaporize
             *Paladin: Eye for an Eye, Competetive Spirit, Redemption, Repentance
             */
            {
                "EX1_609",
                "EX1_533",
                "tt_010",
                "EX1_287",
                "EX1_594",
                "EX1_132",
                "AT_073",
                "EX1_136",
                "EX1_379"
            };
            foreach (var c in choices)
            {
                var spells = CardTemplate.LoadFromId(c.ToString());

                if (spells.Type == Card.CType.SPELL && allowedSpells.Contains(c.ToString()))
                {
                    if (spells.Cost == 0 && allowedSpells.Contains(c.ToString()))
                        return c.ToString();
                    if (!spells.IsSecret && spells.Cost == 1 && !hand.Item1)
                        return c.ToString();
                    if (!spells.IsSecret && spells.Cost == 2 && !hand.Item2 || hasCoin)
                        return c.ToString();
                    if (!spells.IsSecret && spells.Cost == 3 && !hand.Item3)
                        return c.ToString();
                    if (!spells.IsSecret && spells.Cost == 4 && !hand.Item4)
                        return c.ToString();


                }
                if (badSecrets.Contains(c.ToString())) continue;
                if (spells.Cost == 1 && spells.IsSecret && !hand.Item1)
                    return c.ToString();
                if (spells.Cost == 2 && spells.IsSecret && !hand.Item2 &&
                    !choices.Any(q => q.ToString().Equals("FP1_004"))) // toss away any secrets if I have mad scientist
                    return c.ToString();
                if (spells.Cost == 3 && spells.IsSecret && !hand.Item3 && hasCoin &&
                    !choices.Any(q => q.ToString().Equals("FP1_004"))) //toss away any secret if I have mad scientist
                    return c.ToString();
            }

            return "";
        }

        /// <summary>
        /// Takes a better weapon with lower cost. 
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="hand"></param>
        /// <returns></returns>
        private static string HandleWeapons(List<Card.Cards> choices, Tuple<bool, bool, bool, bool> hand)
        {
            var hasCoin = choices.Count() > 3;
            var gotWeapon = false;
            var bestWeapon = "";
            var bestWeaponCheck = CardTemplate.LoadFromId("EX1_409t"); //checks against heavy axe
            foreach (var c in from c in choices
                              let weap = CardTemplate.LoadFromId(c.ToString())
                              where !hand.Item1 && weap.Cost == 2 && weap.Type == Card.CType.WEAPON &&
                                    bestWeaponCheck.Atk <= weap.Atk
                              select c)
            {
                bestWeaponCheck = CardTemplate.LoadFromId(c.ToString());
                bestWeapon = c.ToString();
                gotWeapon = true;
            }
            foreach (var c in from c in choices
                              let weap = CardTemplate.LoadFromId(c.ToString())
                              where weap.Cost == 3 && weap.Type == Card.CType.WEAPON &&
                                    bestWeaponCheck.Atk <= weap.Atk
                              select c)
            {
                bestWeaponCheck = CardTemplate.LoadFromId(c.ToString());
                bestWeapon = c.ToString();
                gotWeapon = true;
            }
            foreach (var c in from c in choices
                              let weap = CardTemplate.LoadFromId(c.ToString())
                              where !hand.Item4 && hasCoin && !gotWeapon && weap.Cost == 4 && weap.Type == Card.CType.WEAPON &&
                                    bestWeaponCheck.Atk >= weap.Atk
                              select c)
            {
                bestWeaponCheck = CardTemplate.LoadFromId(c.ToString());
                bestWeapon = c.ToString();
                gotWeapon = true;
            }
            return bestWeapon;
        }

        /// <summary>
        /// Method is designed to look through your hand and whitelist best possible minions to fit your curve well
        /// </summary>
        /// <param name="choices">List of 3 to 4 card choices that are analyzed</param>
        /// <param name="whiteList">Dictionary list which contains cards that are kept</param>
        /// <returns></returns>
        private static Tuple<bool, bool, bool, bool> HandleMinions(List<Card.Cards> choices,
            IDictionary<string, bool> whiteList, Card.CClass opponentClass, Card.CClass ownClass)
        {


            var aggro = Bot.CurrentDeck().Cards.Average(c => CardTemplate.LoadFromId(c).Cost) < _averageAggro;

            var badMinions = new List<string>
            {
                "CS2_173",
                "CS2_203",
                "FP1_017",
                "EX1_045",
                "NEW1_037",
                "EX1_055",
                "EX1_058",
                "NEW1_021",
                "GVG_025",
                "GVG_039",
                "EX1_306",
                "EX1_084",
                "EX1_582",
                "GVG_084",
                "CS2_118",
                "CS2_122",
                "CS2_124",
                "EX1_089",
                "EX1_050",
                "GVG_089",
                "EX1_005",
                "EX1_595",
                "EX1_396",
                "EX1_048",
                "AT_091",
                "EX1_584",
                "EX1_093",
                "GVG_094",
                "GVG_109",
                "GVG_107",
                "DS1_175",
                "EX1_362",
                "GVG_122",
                "EX1_011",
                "AT_075",
                "EX1_085"
            };

            var myChoices = new Dictionary<string, int>();
            var myChoices2 = new Dictionary<string, int>();
            var myChoices3 = new Dictionary<string, int>();

            foreach (var c in choices)
            {
                var minion = CardTemplate.LoadFromId(c.ToString());
                var prioCheck = GetPriority(minion, opponentClass, ownClass);

                switch (minion.Cost)
                {
                    case 1:
                        {
                            myChoices.AddOrUpdate(minion.Id.ToString(), prioCheck);
                            break;
                        }
                    case 2:
                        {
                            myChoices2.AddOrUpdate(minion.Id.ToString(), prioCheck);
                            break;
                        }
                    case 3:
                        {
                            myChoices3.AddOrUpdate(minion.Id.ToString(), prioCheck);
                            break;
                        }
                }
            }
            var num1Drops = 0;
            var sortedDict = (myChoices.OrderByDescending(entry => entry.Value))
                 .Take(4)
                 .ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (var c in sortedDict)
            {
                if (c.Value < 1) continue;
                if (c.Value <= 1 && hasCoin) continue;
                whiteList.AddOrUpdate(c.Key, c.Value > 6);
                num1Drops++;
                if (num1Drops >= Allowed1Drops) break;
            }
            var sortedDict2 = (myChoices2.OrderByDescending(entry => entry.Value))
                 .Take(4)
                 .ToDictionary(pair => pair.Key, pair => pair.Value);
            var num2Drops = 0;
            foreach (var c in sortedDict2.Where(c => c.Value >= 2))
            {
                num2Drops++;
                whiteList.AddOrUpdate(c.Key, c.Value > 4);
                if (num2Drops == Allowed2Drops) break;
            }
            var sortedDict3 = (myChoices3.OrderByDescending(entry => entry.Value))
                 .Take(4) //at most 2 3 drops
                 .ToDictionary(pair => pair.Key, pair => pair.Value);
            var num3Drops = 0;
            foreach (var c in sortedDict3)
            {
                //if (num2Drops == 0) break;
                if (c.Value < 1) continue;
                num3Drops++;
                whiteList.AddOrUpdate(c.Key, false);
                if (num3Drops == Allowed3Drops) break;
            }

            GetFourDrops(choices, badMinions, whiteList, aggro);


            return new Tuple<bool, bool, bool, bool>(has1Drop, has2Drop, has3Drop, has4Drop);
        }
        private static int GetPriority(CardTemplate c, Card.CClass opponentClass, Card.CClass ownClass, int modifier = 0)
        {
            if (c.IsSecret) return -50;
            if (c.Type == Card.CType.SPELL || c.Type == Card.CType.HERO || c.Type == Card.CType.HEROPOWER)
                return -50;
            if (c.Cost > 4)
                return 0;

            /*
             * 
             */
            var myDeck = Bot.CurrentDeck().Cards.ToList();
            var numMechs = myDeck.Count(q => CardTemplate.LoadFromId(q).Race == Card.CRace.MECH);
            var containsWarper = myDeck.Any(q => q.ToString() == Mechwarper);
            var numDrag = myDeck.Count(q => CardTemplate.LoadFromId(q).Race == Card.CRace.DRAGON);
            var numSecret = myDeck.Count(q => CardTemplate.LoadFromId(q).IsSecret && CardTemplate.LoadFromId(q).Cost == 1);



            var value = 0;
            var vanila = c.Health == 3 && c.Atk == 2;
            var vanila3 = c.Health == 4 && c.Atk == 3;

            switch (c.Cost)
            {
                case 1:
                    {
                        if (opponentClass == Card.CClass.MAGE || opponentClass == Card.CClass.ROGUE ||
                            opponentClass == Card.CClass.DRUID && c.Health == 1 && !c.Divineshield)
                            value--;
                        if (c.Name.Contains("Cogmaster") && numMechs > 3)
                            value++;
                        if (c.Name.Contains("Injured") && ownClass != Card.CClass.PRIEST)
                            value -= 7;
                        if (c.Name.Contains("secret") && numSecret > 2)
                            value++;
                        if (c.Name.Contains("Voodoo"))
                            value -= 2;
                        if (c.Overload > 0)
                            value -= 4;
                        if (c.Health == 3)
                            value++;
                        if (c.Health == c.Atk && c.HasBattlecry)
                            value += 2;
                        if (c.Quality == Card.CQuality.Epic)
                            value -= 2;
                        if (c.Quality == Card.CQuality.Rare && c.Atk > c.Health)
                            value--;
                        if (c.Atk > c.Health && c.Class == Card.CClass.ROGUE)
                            value += 2;
                        if (c.Stealth && c.Atk == 2)
                            value++;
                        if (c.Divineshield)
                            value++;
                        if (c.HasDeathrattle)
                            value++;
                        if (c.Race == Card.CRace.MURLOC)
                            value--;
                        if (c.Class == Card.CClass.NONE && c.Atk == 2 || c.HasBattlecry)
                            value++;
                        if (c.Taunt && c.Class != Card.CClass.WARLOCK)
                            value -= 2;
                        if (c.Class == Card.CClass.HUNTER && !c.HasDeathrattle)
                            value--;
                        if (c.Class == Card.CClass.HUNTER && c.HasDeathrattle)
                            value++;
                        break;
                    }
                case 2:
                    {
                        value--;
                        if ((c.Health == 1 && !c.HasDeathrattle) || (c.Atk > c.Health + 1) || (c.Health > c.Atk + 1) || (c.Atk == 1 && c.Health == 1))
                        {
                            value -= 2;
                            break;
                        }

                        if (c.Charge)
                            value--;
                        if (c.Taunt)
                            value--;
                        value++;

                        if (c.Name.Equals("Sunfury Protector") && numMechs > 3)
                            value -= 4;
                        if (c.Name.Equals("Mechwarper") && numMechs > 3)
                            value += 2;
                        if (c.Stealth)
                            value++;
                        if (c.Race == Card.CRace.DEMON && c.HasBattlecry)
                            value -= 5;
                        value++;
                        if (vanila && opponentClass == Card.CClass.PALADIN)
                            value++;


                        break;
                    }
                case 3:
                    {
                        value += 2;
                        if (opponentClass == Card.CClass.PALADIN && c.Health == 2)
                            value--;
                        if (c.Charge && c.Divineshield)
                            value++;
                        if (c.Quality == Card.CQuality.Legendary)
                            value++;
                        if (c.Race == Card.CRace.DEMON)
                            value++;
                        if (c.Quality == Card.CQuality.Epic && opponentClass != Card.CClass.PALADIN)
                            value -= 7;
                        if (c.Health == 1 && !c.Divineshield && !c.HasDeathrattle)
                            value -= 5;
                        if (vanila3)
                            value += 2;
                        if (c.Health == c.Cost && c.Atk == c.Cost && c.Quality == Card.CQuality.Rare && !c.Inspire)
                            value -= 7;
                        if (c.Health < c.Cost && c.Atk < c.Cost)
                            value -= 5;
                        if (c.Class == Card.CClass.ROGUE && c.Race != Card.CRace.MECH)
                            value += 5;
                        if (c.HasBattlecry)
                            value++;
                        if (c.HasDeathrattle)
                            value++;
                        break;
                    }
                case 4:
                    {
                        if (numDrag > 2 && c.Quality == Card.CQuality.Epic)
                            value += 5;

                        value++;
                        break;
                    }
            }
            if (c.Health >= c.Atk + 1)
            {
                if (c.HasBattlecry) value++;
                if (c.HasDeathrattle) value++;
                if (c.Enrage) value++;
            }
            if (c.Cantattack || c.Atk == 0)
                value -= 5;
            if (c.Health > c.Cost && c.Atk > c.Cost)
                value += 2;
            if (c.Divineshield) value += 2;
            if (c.Divineshield && c.Health == c.Cost && c.Atk == c.Cost)
                value += 2;
            if (c.Stealth) value++;
            value += c.Health - 1 + c.Atk - 1;


            return value + modifier;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="badMinions"></param>
        /// <param name="whiteList"></param>
        /// <param name="aggro"></param>
        private static void GetFourDrops(List<Card.Cards> choices, List<string> badMinions,
            IDictionary<string, bool> whiteList, bool aggro)
        {
            if (!has3Drop) return; //if no 3 drop, return
            foreach (var c in choices)
            {
                if (badMinions.Contains(c.ToString()) || !has3Drop || (choices.Count < 4)) continue;

                if (CardTemplate.LoadFromId(c.ToString()).Cost == 4)
                {
                    var minion = CardTemplate.LoadFromId(c.ToString());
                    if (minion.Type == Card.CType.MINION)
                    {
                        whiteList.AddOrUpdate(GetBestOne(choices, 4, badMinions), false);
                        has4Drop = true;
                    }
                }

            }
        }

        /// <summary>
        /// its redudndant method. I will remove it after we get out of beta
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="cost"></param>
        /// <param name="badMinions"></param>
        /// <returns></returns>
        private static string GetBestOne(List<Card.Cards> choices, int cost, List<string> badMinions)
        {
            return
                choices.Where(c => !badMinions.Contains(c.ToString()))
                    .Where(c => CardTemplate.LoadFromId(c.ToString()).Cost == cost)
                    .Aggregate("CS2_118", (current, c) => WhichIsStronger(current, c.ToString()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curBest"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        private static string WhichIsStronger(string curBest, string comparison)
        {
            var curBestCheck = CardTemplate.LoadFromId(curBest);
            var comparisonCheck = CardTemplate.LoadFromId(comparison);
            if (curBestCheck.Type == Card.CType.MINION)
                return curBestCheck.Health > comparisonCheck.Health ? curBest : comparison; // handles minions
            return curBest;
        }
    }
}