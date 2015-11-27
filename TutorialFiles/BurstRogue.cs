using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartBot.Database;
using SmartBot.Mulligan;
using SmartBot.Plugins.API;

namespace SmartBotUI.MulliganRogue
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


        /*
         * Dictionary of cards you would like to keep
         * First Input <String>: Card name
         * Second Input <bool>: allow more than one? 
         */
        private readonly Dictionary<string, bool> _whiteList;
        /*
         * _cardsToKeep;
         * You will fill up list of cards that your mulligan will tell SmartBot to keep
         */
        private readonly List<Card.Cards> _cardsToKeep;

        private const string Coin = "GAME_005"; //Coin Declaration
        private static bool _hasCoin;
        // static boolean check to be accesable by any method in case you decide to use helper methods

        #region allplayablecards

        /*
         * Below you have list of all playable and collectable cards taht might appear in your mulligan
         * scenario. I found it tideous to always look them up so I generated a list from CardTemplates 
         * of every possible card and post them here for convinience. It will also save you time and reduce
         * the cnaces of messing up when you _whiteList cards
         *      Example:
         *              __whiteList.AddOrUpdate("GVG"_002, true);
         *              and
         *              __whiteList.AddOrUpdate(Snowchugger, true);
         *      Those 2 lines will produce the same outcome, but second one requires less work. given that
         *      you don't need to look up the code for snowchugger :) 
         *      
         * 
         * FULL LIST BELOW IN THE FOLLOWING ORDER
         * 1) Class Specific Minions
         * 2) Neutral Minions
         * 2) Class Specific Spells
         */
        //=============================Mage
        private const string SootSpewer = "GVG_123";
        private const string FlameLeviathan = "GVG_007";
        private const string ArchmageAntonidas = "EX1_559";
        private const string ColdarraDrake = "AT_008";
        private const string Rhonin = "AT_009";
        private const string ManaWyrm = "NEW1_012";
        private const string SorcerersApprentice = "EX1_608";
        private const string Snowchugger = "GVG_002";
        private const string FallenHero = "AT_003";
        private const string EtherealArcanist = "EX1_274";
        private const string WeeSpellstopper = "GVG_122";
        private const string DalaranAspirant = "AT_006";
        private const string WaterElemental = "CS2_033";
        private const string KirinTorMage = "EX1_612";
        private const string GoblinBlastmage = "GVG_004";
        private const string Spellslinger = "AT_007";
        //=============================Paladin
        private const string SwordofJustice = "EX1_366";
        private const string Coghammer = "GVG_059";
        private const string Quartermaster = "GVG_060";
        private const string CobaltGuardian = "GVG_062";
        private const string BolvarFordragon = "GVG_063";
        private const string MysteriousChallenger = "AT_079";
        private const string EadricthePure = "AT_081";
        private const string TuskarrJouster = "AT_104";
        private const string GuardianofKings = "CS2_088";
        private const string TirionFordring = "EX1_383";
        private const string AldorPeacekeeper = "EX1_382";
        private const string LightsJustice = "CS2_091";
        private const string ArgentLance = "AT_077";
        private const string ArgentProtector = "EX1_362";
        private const string TruesilverChampion = "CS2_097";
        private const string WarhorseTrainer = "AT_075";
        private const string MurlocKnight = "AT_076";
        private const string ShieldedMinibot = "GVG_058";
        private const string ScarletPurifier = "GVG_101";
        //=============================Druid
        private const string DruidoftheSaber = "AT_042";
        private const string Malorne = "GVG_035";
        private const string MechBearCat = "GVG_034";
        private const string Cenarius = "EX1_573";
        private const string AncientofLore = "NEW1_008";
        private const string DruidoftheFang = "GVG_080";
        private const string Aviana = "AT_045";
        private const string KnightoftheWild = "AT_041";
        private const string DruidoftheClaw = "EX1_165";
        private const string AncientofWar = "EX1_178";
        private const string IronbarkProtector = "CS2_232";
        private const string AnodizedRoboCub = "GVG_030";
        private const string KeeperoftheGrove = "EX1_166";
        private const string DarnassusAspirant = "AT_038";
        private const string GroveTender = "GVG_032";
        private const string Wildwalker = "AT_040";
        private const string SavageCombatant = "AT_039";
        //=============================Hunter
        private const string TimberWolf = "DS1_175";
        private const string BraveArcher = "AT_059";
        private const string KingofBeasts = "GVG_046";
        private const string MetaltoothLeaper = "GVG_048";
        private const string Gahzrilla = "GVG_049";
        private const string Stablemaster = "AT_057";
        private const string Acidmaw = "AT_063";
        private const string KingKrush = "EX1_543";
        private const string RamWrangler = "AT_010";
        private const string SavannahHighmane = "EX1_534";
        private const string TundraRhino = "DS1_178";
        private const string StarvingBuzzard = "CS2_237";
        private const string GladiatorsLongbow = "DS1_188";
        private const string Webspinner = "FP1_011";
        private const string ScavengingHyena = "EX1_531";
        private const string Glaivezooka = "GVG_043";
        private const string SteamwheedleSniper = "GVG_087";
        private const string KingsElekk = "AT_058";
        private const string EaglehornBow = "EX1_536";
        private const string Huffer = "NEW1_034";
        private const string Leokk = "NEW1_033";
        private const string Houndmaster = "DS1_070";
        private const string Misha = "NEW1_032";
        //=============================Priest
        private const string Lightwell = "EX1_341";
        private const string Shadowfiend = "AT_014";
        private const string Lightspawn = "EX1_335";
        private const string Shadowbomber = "GVG_009";
        private const string Voljin = "GVG_014";
        private const string TempleEnforcer = "EX1_623";
        private const string UpgradedRepairBot = "GVG_083";
        private const string ConfessorPaletress = "AT_018";
        private const string CabalShadowPriest = "EX1_091";
        private const string ProphetVelen = "EX1_350";
        private const string WyrmrestAgent = "AT_116";
        private const string NorthshireCleric = "CS2_235";
        private const string Shadowboxer = "GVG_072";
        private const string Shrinkmeister = "GVG_011";
        private const string AuchenaiSoulpriest = "EX1_591";
        private const string HolyChampion = "AT_011";
        private const string SpawnofShadows = "AT_012";
        private const string DarkCultist = "FP1_023";
        //=============================Rogue
        private const string PatientAssassin = "EX1_522";
        private const string IronSensei = "GVG_027";
        private const string OneeyedCheat = "GVG_025";
        private const string TradePrinceGallywix = "GVG_028";
        private const string Kidnapper = "NEW1_005";
        private const string OgreNinja = "GVG_088";
        private const string AssassinsBlade = "CS2_080";
        private const string ShadoPanRider = "AT_028";
        private const string Anubarak = "AT_036";
        private const string WickedKnife = "CS2_082";
        private const string Buccaneer = "AT_029";
        private const string CogmastersWrench = "GVG_024";
        private const string DefiasRingleader = "EX1_131";
        private const string PoisonedBlade = "AT_034";
        private const string Cutpurse = "AT_031";
        private const string SI7Agent = "EX1_134";
        private const string UndercityValiant = "AT_030";
        private const string GoblinAutoBarber = "GVG_023";
        private const string EdwinVanCleef = "EX1_613";
        private const string PerditionsBlade = "EX1_133";
        private const string MasterofDisguise = "NEW1_014";
        private const string AnubarAmbusher = "FP1_026";
        private const string ShadyDealer = "AT_032";
        //=============================Shaman
        private const string StoneclawTotem = "CS2_051";
        private const string FlametongueTotem = "EX1_565";
        private const string VitalityTotem = "GVG_039";
        private const string HealingTotem = "NEW1_009";
        private const string WrathofAirTotem = "CS2_052";
        private const string DustDevil = "EX1_243";
        private const string ManaTideTotem = "EX1_575";
        private const string SearingTotem = "CS2_050";
        private const string Neptulon = "GVG_042";
        private const string Doomhammer = "EX1_567";
        private const string AlAkirtheWindlord = "NEW1_010";
        private const string FireElemental = "CS2_042";
        private const string TheMistcaller = "AT_054";
        private const string ThunderBluffValiant = "AT_049";
        private const string EarthElemental = "EX1_250";
        private const string StormforgedAxe = "EX1_247";
        private const string WhirlingZapomatic = "GVG_037";
        private const string Windspeaker = "EX1_587";
        private const string TuskarrTotemic = "AT_046";
        private const string ChargedHammer = "AT_050";
        private const string UnboundElemental = "EX1_258";
        private const string SiltfinSpiritwalker = "GVG_040";
        private const string Powermace = "GVG_036";
        private const string DraeneiTotemcarver = "AT_047";
        private const string DunemaulShaman = "GVG_066";
        private const string TotemGolem = "AT_052";
        //=============================Warlock
        private const string BloodImp = "CS2_059";
        private const string SummoningPortal = "EX1_315";
        private const string MalGanis = "GVG_021";
        private const string MistressofPain = "GVG_018";
        private const string AnimaGolem = "GVG_077";
        private const string DreadInfernal = "CS2_064";
        private const string FloatingWatcher = "GVG_100";
        private const string FearsomeDoomguard = "AT_020";
        private const string VoidCrusher = "AT_023";
        private const string WilfredFizzlebang = "AT_027";
        private const string LordJaraxxus = "EX1_323";
        private const string Doomguard = "EX1_310";
        private const string Dreadsteed = "AT_019";
        private const string VoidTerror = "EX1_304";
        private const string Voidwalker = "CS2_065";
        private const string Succubus = "EX1_306";
        private const string TinyKnightofEvil = "AT_021";
        private const string FelCannon = "GVG_020";
        private const string Voidcaller = "FP1_022";
        private const string Wrathguard = "AT_026";
        private const string FlameImp = "EX1_319";
        private const string Felguard = "EX1_301";
        private const string BloodFury = "EX1_323w";
        private const string PitLord = "EX1_313";
        //=============================Warrior
        private const string ArcaniteReaper = "CS2_112";
        private const string Shieldmaiden = "GVG_053";
        private const string IronJuggernaut = "GVG_056";
        private const string SiegeEngine = "GVG_086";
        private const string VarianWrynn = "AT_072";
        private const string SeaReaver = "AT_130";
        private const string GrommashHellscream = "EX1_414";
        private const string Gorehowl = "EX1_411";
        private const string Armorsmith = "EX1_402";
        private const string Warbot = "GVG_051";
        private const string CruelTaskmaster = "EX1_603";
        private const string SparringPartner = "AT_069";
        private const string FieryWarAxe = "CS2_106";
        private const string AlexstraszasChampion = "AT_071";
        private const string WarsongCommander = "EX1_084";
        private const string DeathsBite = "FP1_021";
        private const string ArathiWeaponsmith = "EX1_398";
        private const string KingsDefender = "AT_065";
        private const string OrgrimmarAspirant = "AT_066";
        private const string KorkronElite = "NEW1_011";
        private const string FrothingBerserker = "EX1_604";
        private const string OgreWarmaul = "GVG_054";
        private const string MagnataurAlpha = "AT_067";
        private const string ScrewjankClunker = "GVG_055";
        //=============================Neutral
        private const string ShadeofNaxxramas = "FP1_005";
        private const string NerubianEgg = "FP1_007";
        private const string LorewalkerCho = "EX1_100";
        private const string NatPagle = "EX1_557";
        private const string Frog = "hexfrog";
        private const string Shieldbearer = "EX1_405";
        private const string TargetDummy = "GVG_093";
        private const string ArgentWatchman = "AT_109";
        private const string NoviceEngineer = "EX1_015";
        private const string BloodmageThalnos = "EX1_012";
        private const string Doomsayer = "NEW1_021";
        private const string CaptainsParrot = "NEW1_016";
        private const string ExplosiveSheep = "GVG_076";
        private const string BluegillWarrior = "CS2_173";
        private const string MurlocTidehunter = "EX1_506";
        private const string GrimscaleOracle = "EX1_508";
        private const string IronbeakOwl = "CS2_203";
        private const string AlarmoBot = "EX1_006";
        private const string Hobgoblin = "GVG_104";
        private const string VoodooDoctor = "EX1_011";
        private const string TournamentAttendee = "AT_097";
        private const string Skeleton = "skele11";
        private const string YoungDragonhawk = "CS2_169";
        private const string SouthseaCaptain = "NEW1_027";
        private const string AngryChicken = "EX1_009";
        private const string Imp = "EX1_598";
        private const string QuestingAdventurer = "EX1_044";
        private const string ManaAddict = "EX1_055";
        private const string MurlocWarleader = "EX1_507";
        private const string GoldshireFootman = "CS1_042";
        private const string MasterSwordsmith = "NEW1_037";
        private const string StonetuskBoar = "CS2_171";
        private const string RaidLeader = "CS2_122";
        private const string Wolfrider = "CS2_124";
        private const string DarkscaleHealer = "DS1_055";
        private const string FelReaver = "GVG_016";
        private const string Loatheb = "FP1_030";
        private const string UnstableGhoul = "FP1_024";
        private const string Feugen = "FP1_015";
        private const string Stalagg = "FP1_014";
        private const string KelThuzad = "FP1_013";
        private const string AntiqueHealbot = "GVG_069";
        private const string SaltyDog = "GVG_070";
        private const string SludgeBelcher = "FP1_012";
        private const string SpectralKnight = "FP1_008";
        private const string ForceTankMAX = "GVG_079";
        private const string HarrisonJones = "EX1_558";
        private const string Nozdormu = "EX1_560";
        private const string Alexstrasza = "EX1_561";
        private const string Onyxia = "EX1_562";
        private const string Malygos = "EX1_563";
        private const string FacelessManipulator = "EX1_564";
        private const string Ysera = "EX1_572";
        private const string TheBeast = "EX1_577";
        private const string PriestessofElune = "EX1_583";
        private const string SeaGiant = "EX1_586";
        private const string Maexxna = "FP1_010";
        private const string BloodKnight = "EX1_590";
        private const string MoltenGiant = "EX1_620";
        private const string CaptainGreenskin = "NEW1_024";
        private const string Deathwing = "NEW1_030";
        private const string JusticarTrueheart = "AT_132";
        private const string Gruul = "NEW1_038";
        private const string Hogger = "NEW1_040";
        private const string StampedingKodo = "NEW1_041";
        private const string GelbinMekkatorque = "EX1_112";
        private const string EliteTaurenChieftain = "PRO_001";
        private const string IllidanStormrage = "EX1_614";
        private const string SkycapnKragg = "AT_070";
        private const string MogorsChampion = "AT_088";
        private const string MuklasChampion = "AT_090";
        private const string ClockworkKnight = "AT_096";
        private const string SideshowSpelleater = "AT_098";
        private const string Kodorider = "AT_099";
        private const string PitFighter = "AT_101";
        private const string CapturedJormungar = "AT_102";
        private const string NorthSeaKraken = "AT_103";
        private const string MasterJouster = "AT_112";
        private const string Recruiter = "AT_113";
        private const string FencingCoach = "AT_115";
        private const string MasterofCeremonies = "AT_117";
        private const string GrandCrusader = "AT_118";
        private const string KvaldirRaider = "AT_119";
        private const string FrostGiant = "AT_120";
        private const string Chillmaw = "AT_123";
        private const string BolfRamshield = "AT_124";
        private const string Icehowl = "AT_125";
        private const string NexusChampionSaraad = "AT_127";
        private const string TheSkeletonKnight = "AT_128";
        private const string MadderBomber = "GVG_090";
        private const string BombLobber = "GVG_099";
        private const string PilotedSkyGolem = "GVG_105";
        private const string Junkbot = "GVG_106";
        private const string DrBoom = "GVG_110";
        private const string MimironsHead = "GVG_111";
        private const string MogortheOgre = "GVG_112";
        private const string FoeReaper4000 = "GVG_113";
        private const string SneedsOldShredder = "GVG_114";
        private const string Toshley = "GVG_115";
        private const string MekgineerThermaplugg = "GVG_116";
        private const string Gazlowe = "GVG_117";
        private const string Blingtron3000 = "GVG_119";
        private const string HemetNesingwary = "GVG_120";
        private const string ClockworkGiant = "GVG_121";
        private const string TroggzortheEarthinator = "GVG_118";
        private const string NerubarWeblord = "FP1_017";
        private const string RavenholdtAssassin = "CS2_161";
        private const string StormpikeCommando = "CS2_150";
        private const string Archmage = "CS2_155";
        private const string RecklessRocketeer = "CS2_213";
        private const string CoreHound = "CS2_201";
        private const string TheBlackKnight = "EX1_002";
        private const string BaronGeddon = "EX1_249";
        private const string StranglethornTiger = "EX1_028";
        private const string MindControlTech = "EX1_085";
        private const string SylvanasWindrunner = "EX1_016";
        private const string LordoftheArena = "CS2_162";
        private const string BoulderfistOgre = "CS2_200";
        private const string WarGolem = "CS2_186";
        private const string SilverHandKnight = "CS2_151";
        private const string YoungPriestess = "EX1_004";
        private const string FenCreeper = "CS1_069";
        private const string RagnarostheFirelord = "EX1_298";
        private const string AzureDrake = "EX1_284";
        private const string BigGameHunter = "EX1_005";
        private const string GadgetzanAuctioneer = "EX1_095";
        private const string BootyBayBodyguard = "CS2_187";
        private const string FrostElemental = "EX1_283";
        private const string Sunwalker = "EX1_032";
        private const string Nightblade = "EX1_593";
        private const string MurlocTidecaller = "EX1_509";
        private const string IronforgeRifleman = "CS2_141";
        private const string ColdlightOracle = "EX1_050";
        private const string LeeroyJenkins = "EX1_116";
        private const string MurlocRaider = "CS2_168";
        private const string CairneBloodhoof = "EX1_110";
        private const string GurubashiBerserker = "EX1_399";
        private const string Abomination = "EX1_097";
        private const string ArgentCommander = "EX1_067";
        private const string MountainGiant = "EX1_105";
        private const string SpitefulSmith = "CS2_221";
        private const string StormwindChampion = "CS2_222";
        private const string WindfuryHarpy = "EX1_033";
        private const string FrostwolfWarlord = "CS2_226";
        private const string VentureCoMercenary = "CS2_227";
        private const string AbusiveSergeant = "CS2_188";
        private const string SouthseaDeckhand = "CS2_146";
        private const string Lightwarden = "EX1_001";
        private const string Undertaker = "FP1_028";
        private const string MagmaRager = "CS2_118";
        private const string ArgentHorserider = "AT_087";
        private const string Cogmaster = "GVG_013";
        private const string DamagedGolem = "skele21";
        private const string LowlySquire = "AT_082";
        private const string Secretkeeper = "EX1_080";
        private const string HungryCrab = "NEW1_017";
        private const string InjuredKvaldir = "AT_105";
        private const string MicroMachine = "GVG_103";
        private const string Wisp = "CS2_231";
        private const string ClockworkGnome = "GVG_082";
        private const string ElvenArcher = "CS2_189";
        private const string LootHoarder = "EX1_096";
        private const string AvataroftheCoin = "GAME_002";
        private const string SilentKnight = "AT_095";
        private const string LeperGnome = "EX1_029";
        private const string FrostwolfGrunt = "CS2_121";
        private const string CrazedAlchemist = "EX1_059";
        private const string PintSizedSummoner = "EX1_076";
        private const string WorgenInfiltrator = "EX1_010";
        private const string AnnoyoTron = "GVG_085";
        private const string GadgetzanJouster = "AT_133";
        private const string DireWolfAlpha = "EX1_162";
        private const string BloodsailCorsair = "NEW1_025";
        private const string LanceCarrier = "AT_084";
        private const string ManaWraith = "EX1_616";
        private const string HauntedCreeper = "FP1_002";
        private const string EchoingOoze = "FP1_003";
        private const string MadScientist = "FP1_004";
        private const string KoboldGeomancer = "CS2_142";
        private const string EnhanceoMechano = "GVG_107";
        private const string Puddlestomper = "GVG_064";
        private const string YouthfulBrewmaster = "EX1_049";
        private const string StonesplinterTrogg = "GVG_067";
        private const string AcidicSwampOoze = "EX1_066";
        private const string Recombobulator = "GVG_108";
        private const string AmaniBerserker = "EX1_393";
        private const string ShipsCannon = "GVG_075";
        private const string BloodfenRaptor = "CS2_172";
        private const string BoneguardLieutenant = "AT_089";
        private const string TwilightDrake = "EX1_043";
        private const string MadBomber = "EX1_082";
        private const string KnifeJuggler = "NEW1_019";
        private const string Mechwarper = "GVG_006";
        private const string Jeeves = "GVG_094";
        private const string RiverCrocolisk = "CS2_120";
        private const string WildPyromancer = "NEW1_020";
        private const string FaerieDragon = "NEW1_023";
        private const string GarrisonCommander = "AT_080";
        private const string AcolyteofPain = "EX1_007";
        private const string Pirate = "TB_015";
        private const string AncientWatcher = "EX1_045";
        private const string FlameJuggler = "AT_094";
        private const string ArgentSquire = "EX1_008";
        private const string GnomereganInfantry = "GVG_098";
        private const string DalaranMage = "EX1_582";
        private const string MiniMage = "GVG_109";
        private const string ThrallmarFarseer = "EX1_021";
        private const string Squire = "CS2_152";
        private const string BloodsailRaider = "NEW1_018";
        private const string OldMurkEye = "EX1_062";
        private const string DreadCorsair = "NEW1_022";
        private const string Demolisher = "EX1_102";
        private const string DefenderofArgus = "EX1_093";
        private const string CultMaster = "EX1_595";
        private const string FlyingMachine = "GVG_084";
        private const string SunfuryProtector = "EX1_058";
        private const string TaurenWarrior = "EX1_390";
        private const string EmperorCobra = "EX1_170";
        private const string StoneskinGargoyle = "FP1_027";
        private const string SilverbackPatriarch = "CS2_127";
        private const string GnomishExperimenter = "GVG_092";
        private const string DragonhawkRider = "AT_083";
        private const string SilverHandRegent = "AT_100";
        private const string ImpMaster = "EX1_597";
        private const string GnomishInventor = "CS2_147";
        private const string IronfurGrizzly = "CS2_125";
        private const string RagingWorgen = "EX1_412";
        private const string DragonlingMechanic = "EX1_025";
        private const string ShatteredSunCleric = "EX1_019";
        private const string StormwindKnight = "CS2_131";
        private const string KezanMystic = "GVG_074";
        private const string ScarletCrusader = "EX1_020";
        private const string ArcaneNullifierX21 = "GVG_091";
        private const string Illuminator = "GVG_089";
        private const string Spellbreaker = "EX1_048";
        private const string GilblinStalker = "GVG_081";
        private const string PilotedShredder = "GVG_096";
        private const string GoblinSapper = "GVG_095";
        private const string EarthenRingFarseer = "CS2_117";
        private const string ArcaneGolem = "EX1_089";
        private const string ColiseumManager = "AT_110";
        private const string OgreMagi = "CS2_197";
        private const string ArmoredWarhorse = "AT_108";
        private const string ColdlightSeer = "EX1_103";
        private const string VioletTeacher = "NEW1_026";
        private const string MaidenoftheLake = "AT_085";
        private const string SilvermoonGuardian = "EX1_023";
        private const string IceRager = "AT_092";
        private const string FrigidSnobold = "AT_093";
        private const string FinkleEinhorn = "EX1_finkle";
        private const string JunglePanther = "EX1_017";
        private const string BurlyRockjawTrogg = "GVG_068";
        private const string HarvestGolem = "EX1_556";
        private const string BaronRivendare = "FP1_031";
        private const string LilExorcist = "GVG_097";
        private const string GormoktheImpaler = "AT_122";
        private const string CrowdFavorite = "AT_121";
        private const string SenjinShieldmasta = "CS2_179";
        private const string MogushanWarden = "EX1_396";
        private const string TinkertownTechnician = "GVG_102";
        private const string AncientMage = "EX1_584";
        private const string RazorfenHunter = "CS2_196";
        private const string DarkIronDwarf = "EX1_046";
        private const string LightsChampion = "AT_106";
        private const string LostTallstrider = "GVG_071";
        private const string RefreshmentVendor = "AT_111";
        private const string EvilHeckler = "AT_114";
        private const string Saboteur = "AT_086";
        private const string TwilightGuardian = "AT_017";
        private const string WailingSoul = "FP1_016";
        private const string OasisSnapjaw = "CS2_119";
        private const string ChillwindYeti = "CS2_182";
        private const string AncientBrewmaster = "EX1_057";
        private const string TournamentMedic = "AT_091";
        private const string TinkmasterOverspark = "EX1_083";
        private const string ZombieChow = "FP1_001";
        private const string MillhouseManastorm = "NEW1_029";
        private const string Deathcharger = "FP1_006";
        private const string MechanicalYeti = "GVG_078";
        private const string SpiderTank = "GVG_044";
        private const string EydisDarkbane = "AT_131";
        private const string OgreBrute = "GVG_065";
        private const string FjolaLightbane = "AT_129";
        private const string DancingSwords = "FP1_029";
        private const string Deathlord = "FP1_009";
        private const string KingMukla = "EX1_014";
        private const string InjuredBlademaster = "CS2_181";

        #endregion

        #region allSpells

        //=============================Mage
        private const string Spellbender = "tt_010";
        private const string Duplicate = "FP1_018";
        private const string Flamecannon = "GVG_001";
        private const string UnstablePortal = "GVG_003";
        private const string Vaporize = "EX1_594";
        private const string EchoofMedivh = "GVG_005";
        private const string PolymorphBoar = "AT_005";
        private const string ArcaneBlast = "AT_004";
        private const string Effigy = "AT_002";
        private const string FlameLance = "AT_001";
        private const string Polymorph = "CS2_022";
        private const string ArcaneIntellect = "CS2_023";
        private const string Frostbolt = "CS2_024";
        private const string ArcaneExplosion = "CS2_025";
        private const string FrostNova = "CS2_026";
        private const string MirrorImage = "CS2_027";
        private const string Fireball = "CS2_029";
        private const string Flamestrike = "CS2_032";
        private const string ConeofCold = "EX1_275";
        private const string Pyroblast = "EX1_279";
        private const string Counterspell = "EX1_287";
        private const string IceBarrier = "EX1_289";
        private const string MirrorEntity = "EX1_294";
        private const string IceBlock = "EX1_295";
        private const string Blizzard = "CS2_028";
        private const string IceLance = "CS2_031";
        private const string ArcaneMissiles = "EX1_277";
        //=============================Paladin
        private const string Equality = "EX1_619";
        private const string Avenge = "FP1_020";
        private const string DivineFavor = "EX1_349";
        private const string LayonHands = "EX1_354";
        private const string BlessedChampion = "EX1_355";
        private const string BlessingofWisdom = "EX1_363";
        private const string HolyWrath = "EX1_365";
        private const string Repentance = "EX1_379";
        private const string AvengingWrath = "EX1_384";
        private const string CompetitiveSpirit = "AT_073";
        private const string SealofLight = "GVG_057";
        private const string MusterforBattle = "GVG_061";
        private const string SealofChampions = "AT_074";
        private const string BlessingofMight = "CS2_087";
        private const string HolyLight = "CS2_089";
        private const string BlessingofKings = "CS2_092";
        private const string Consecration = "CS2_093";
        private const string HammerofWrath = "CS2_094";
        private const string Redemption = "EX1_136";
        private const string EyeforanEye = "EX1_132";
        private const string NobleSacrifice = "EX1_130";
        private const string Humility = "EX1_360";
        private const string HandofProtection = "EX1_371";
        private const string EntertheColiseum = "AT_078";
        //=============================Druid
        private const string Starfall = "NEW1_007";
        private const string PoisonSeeds = "FP1_019";
        private const string Bite = "EX1_570";
        private const string ForceofNature = "EX1_571";
        private const string Savagery = "EX1_578";
        private const string LivingRoots = "AT_037";
        private const string AstralCommunion = "AT_043";
        private const string Mulch = "AT_044";
        private const string Recycle = "GVG_031";
        private const string TreeofLife = "GVG_033";
        private const string DarkWispers = "GVG_041";
        private const string Innervate = "EX1_169";
        private const string Starfire = "EX1_173";
        private const string Claw = "CS2_005";
        private const string HealingTouch = "CS2_007";
        private const string Moonfire = "CS2_008";
        private const string MarkoftheWild = "CS2_009";
        private const string SavageRoar = "CS2_011";
        private const string Swipe = "CS2_012";
        private const string WildGrowth = "CS2_013";
        private const string ExcessMana = "CS2_013t";
        private const string Wrath = "EX1_154";
        private const string MarkofNature = "EX1_155";
        private const string SouloftheForest = "EX1_158";
        private const string PoweroftheWild = "EX1_160";
        private const string Naturalize = "EX1_161";
        private const string Nourish = "EX1_164";
        //=============================Hunter
        private const string ExplosiveTrap = "EX1_610";
        private const string FreezingTrap = "EX1_611";
        private const string DeadlyShot = "EX1_617";
        private const string Snipe = "EX1_609";
        private const string Misdirection = "EX1_533";
        private const string ExplosiveShot = "EX1_537";
        private const string UnleashtheHounds = "EX1_538";
        private const string Flare = "EX1_544";
        private const string BestialWrath = "EX1_549";
        private const string SnakeTrap = "EX1_554";
        private const string Powershot = "AT_056";
        private const string BearTrap = "AT_060";
        private const string LockandLoad = "AT_061";
        private const string BallofSpiders = "AT_062";
        private const string CallPet = "GVG_017";
        private const string FeignDeath = "GVG_026";
        private const string CobraShot = "GVG_073";
        private const string HuntersMark = "CS2_084";
        private const string MultiShot = "DS1_183";
        private const string Tracking = "DS1_184";
        private const string ArcaneShot = "DS1_185";
        private const string KillCommand = "EX1_539";
        private const string AnimalCompanion = "NEW1_031";
        //=============================Priest
        private const string HolyNova = "CS1_112";
        private const string CircleofHealing = "EX1_621";
        private const string HolyFire = "EX1_624";
        private const string Shadowform = "EX1_625";
        private const string MassDispel = "EX1_626";
        private const string ShadowMadness = "EX1_334";
        private const string Thoughtsteal = "EX1_339";
        private const string Mindgames = "EX1_345";
        private const string Silence = "EX1_332";
        private const string Lightbomb = "GVG_008";
        private const string LightoftheNaaru = "GVG_012";
        private const string PowerWordGlory = "AT_013";
        private const string Convert = "AT_015";
        private const string Confuse = "AT_016";
        private const string FlashHeal = "AT_055";
        private const string VelensChosen = "GVG_010";
        private const string ShadowWordPain = "CS2_234";
        private const string DivineSpirit = "CS2_236";
        private const string MindBlast = "DS1_233";
        private const string MindControl = "CS1_113";
        private const string HolySmite = "CS1_130";
        private const string MindVision = "CS2_003";
        private const string PowerWordShield = "CS2_004";
        private const string ShadowWordDeath = "EX1_622";
        private const string InnerFire = "CS1_129";
        //=============================Rogue
        private const string Burgle = "AT_033";
        private const string BeneaththeGrounds = "AT_035";
        private const string TinkersSharpswordOil = "GVG_022";
        private const string Sabotage = "GVG_047";
        private const string Backstab = "CS2_072";
        private const string DeadlyPoison = "CS2_074";
        private const string SinisterStrike = "CS2_075";
        private const string Assassinate = "CS2_076";
        private const string Sprint = "CS2_077";
        private const string FanofKnives = "EX1_129";
        private const string Shiv = "EX1_278";
        private const string Headcrack = "EX1_137";
        private const string Shadowstep = "EX1_144";
        private const string Preparation = "EX1_145";
        private const string Conceal = "EX1_128";
        private const string Sap = "EX1_581";
        private const string Vanish = "NEW1_004";
        private const string ColdBlood = "CS2_073";
        private const string BladeFlurry = "CS2_233";
        private const string Eviscerate = "EX1_124";
        private const string Betrayal = "EX1_126";
        //=============================Shaman
        private const string Reincarnate = "FP1_025";
        private const string HealingWave = "AT_048";
        private const string ElementalDestruction = "AT_051";
        private const string AncestralKnowledge = "AT_053";
        private const string AncestorsCall = "GVG_029";
        private const string Crackle = "GVG_038";
        private const string FrostShock = "CS2_037";
        private const string Windfury = "CS2_039";
        private const string AncestralHealing = "CS2_041";
        private const string RockbiterWeapon = "CS2_045";
        private const string Bloodlust = "CS2_046";
        private const string TotemicMight = "EX1_244";
        private const string Hex = "EX1_246";
        private const string LightningBolt = "EX1_238";
        private const string LavaBurst = "EX1_241";
        private const string EarthShock = "EX1_245";
        private const string ForkedLightning = "EX1_251";
        private const string LightningStorm = "EX1_259";
        private const string FeralSpirit = "EX1_248";
        private const string AncestralSpirit = "CS2_038";
        private const string FarSight = "CS2_053";
        //=============================Warlock
        private const string Demonfire = "EX1_596";
        private const string FistofJaraxxus = "AT_022";
        private const string Demonfuse = "AT_024";
        private const string DarkBargain = "AT_025";
        private const string Darkbomb = "GVG_015";
        private const string Demonheart = "GVG_019";
        private const string Implosion = "GVG_045";
        private const string BaneofDoom = "EX1_320";
        private const string PowerOverwhelming = "EX1_316";
        private const string Corruption = "CS2_063";
        private const string Hellfire = "CS2_062";
        private const string DrainLife = "CS2_061";
        private const string ShadowBolt = "CS2_057";
        private const string SenseDemons = "EX1_317";
        private const string Shadowflame = "EX1_303";
        private const string SiphonSoul = "EX1_309";
        private const string TwistingNether = "EX1_312";
        private const string MortalCoil = "EX1_302";
        private const string Soulfire = "EX1_308";
        private const string SacrificialPact = "NEW1_003";
        //=============================Warrior
        private const string CommandingShout = "NEW1_036";
        private const string InnerRage = "EX1_607";
        private const string Slam = "EX1_391";
        private const string BattleRage = "EX1_392";
        private const string Brawl = "EX1_407";
        private const string MortalStrike = "EX1_408";
        private const string Upgrade = "EX1_409";
        private const string ShieldSlam = "EX1_410";
        private const string Bash = "AT_064";
        private const string Bolster = "AT_068";
        private const string BouncingBlade = "GVG_050";
        private const string Crush = "GVG_052";
        private const string BurrowingMine = "GVG_056t";
        private const string Charge = "CS2_103";
        private const string Execute = "CS2_108";
        private const string Cleave = "CS2_114";
        private const string HeroicStrike = "CS2_105";
        private const string Whirlwind = "EX1_400";
        private const string ShieldBlock = "EX1_606";
        private const string Rampage = "CS2_104";

        #endregion

        /*In case you decide to keep track of number of 1, 2 or 3 drops in your mulligan*/
        public static int Num1Drops { get; private set; }
        public static int Num2Drops { get; private set; }
        public static int Num3Drops { get; private set; }
        /*
         * In case you decide to check if you have a particular drop
         * For example
         * if(_has2Drop)
         *     __whiteList.AddOrUpdate(<AnyGood3Drop>, false);
         */
        private static bool _has1Drop;
        private static bool _has2Drop;
        private static bool _has3Drop;
        private static bool _has4Drop;

        /*========================END OF DEFINITION=======================*/




        public bMulliganProfile()
        {
            _whiteList = new Dictionary<string, bool>();
            _cardsToKeep = new List<Card.Cards>();
        }



        public List<Card.Cards> HandleMulligan(List<Card.Cards> Choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            _hasCoin = Choices.Count > 3;
            List<string> alwaysTwo = new List<string> { LeperGnome, SouthseaDeckhand, LootHoarder };
            foreach (var q in alwaysTwo)
                _whiteList.AddOrUpdate(q, true);
            List<string> atLeastOne = new List<string> { DefiasRingleader, SI7Agent, ArgentHorserider };
            foreach (var q in atLeastOne)
                _whiteList.AddOrUpdate(q, false);
            switch (opponentClass)
            {
                case Card.CClass.SHAMAN:
                    {
                        _whiteList.AddOrUpdate(DefiasRingleader, true);
                        _whiteList.AddOrUpdate(DeadlyPoison, false);
                        _whiteList.AddOrUpdate(IronbeakOwl, false);
                        break;
                    }
                case Card.CClass.PRIEST:
                    {
                        _whiteList.AddOrUpdate(DefiasRingleader, true);
                        _whiteList.AddOrUpdate(DeadlyPoison, false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);
                        break;
                    }
                case Card.CClass.MAGE:
                    {
                        _whiteList.AddOrUpdate(DeadlyPoison, false);
                        _whiteList.AddOrUpdate(IronbeakOwl, false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);
                        break;
                    }
                case Card.CClass.PALADIN:
                    {
                        _whiteList.AddOrUpdate(DeadlyPoison, false);
                        _whiteList.AddOrUpdate(IronbeakOwl, false);
                        _whiteList.AddOrUpdate(BladeFlurry, false);
                        break;
                    }
                case Card.CClass.WARRIOR:
                    {
                        _whiteList.AddOrUpdate(PilotedShredder, false);
                        _whiteList.AddOrUpdate(DeadlyPoison, false);
                        break;
                    }
                case Card.CClass.WARLOCK:
                    {
                        _whiteList.AddOrUpdate(DefiasRingleader, true);
                        _whiteList.AddOrUpdate(DeadlyPoison, false);
                        _whiteList.AddOrUpdate(IronbeakOwl, true);
                        _whiteList.AddOrUpdate(PilotedShredder, false);
                        break;
                    }
                case Card.CClass.HUNTER:
                    {
                        _whiteList.AddOrUpdate(IronbeakOwl, false);
                        _whiteList.AddOrUpdate(BladeFlurry, false);
                        break;
                    }
                case Card.CClass.ROGUE:
                    {
                        _whiteList.AddOrUpdate(DefiasRingleader, true);
                        _whiteList.AddOrUpdate(DeadlyPoison, false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);
                        /*If we have a coin, we will save assasin blade in our hand*/
                        _whiteList.AddOrUpdate(_hasCoin ? AssassinsBlade : "", false);
                        /*Since Argent Horserider is always whitelisted with at least 1 copy, we don't need to whitelist it again
                         since we know that if it's our choices, it will always be kept. Hence the cold blood will be kept
                         Alternative way of coding it 
                         if(Choices.Any(c => c.ToString() == ArgentHorserider))
                           _whiteList.AddOrUpdate(ColdBlood, false);
                         */
                        _whiteList.AddOrUpdate(Choices.Any(c => c.ToString() == ArgentHorserider) ? ColdBlood : "", false);
                        break;
                    }
                case Card.CClass.DRUID:
                    {
                        _whiteList.AddOrUpdate(IronbeakOwl, false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);
                        break;
                    }
            }

            /*
             * Code below is final stage of your mulligan which will go through your _whiteList
             * and your choices to see which cards are good to keep
             * It checks if you want to keep the card, do you want more than 1 copy of it
             * Non LINQ version of 6 lines below is this:
             * --------------------------------
             foreach (var s in Choices)
             {
                bool keptOneAlready = false;
                foreach (var c in _cardsToKeep)
                {
                    if (c.ToString() == s.ToString())
                    {
                        keptOneAlready = true;
                        break;
                    }
                }
                if (__whiteList.ContainsKey(s.ToString()))
                {
                    if (!keptOneAlready | __whiteList[s.ToString()]) 
                        _cardsToKeep.Add(s);
                }
             }
             * --------------------------------
             * You will not need to change anything with code below, unless you really want to. 
             */
            foreach (var s in from s in Choices let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString()) where _whiteList.ContainsKey(s.ToString()) where !keptOneAlready | _whiteList[s.ToString()] select s)
            {
                _cardsToKeep.Add(s);
            }
            return _cardsToKeep;
        }
    }
}
