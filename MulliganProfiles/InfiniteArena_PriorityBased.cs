using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        private Dictionary<string, bool> _whiteList; // CardName, KeepDouble
        private readonly List<Card.Cards> _cardsToKeep;
        private const string Coin = "GAME_005";

        /*Very experimental, and will not work until I feel comfortable with them*/
        public bool _arenaMode = true; //do not touch
        private static double _averageAggro = 3.5;
        //Experimental value that tells the mulligan which decks are considered aggro based on average mana cost of your deck. 

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
            _cardsToKeep = new List<Card.Cards>();
        }

        public static double AverageAggro
        {
            get { return _averageAggro; }
            set { _averageAggro = value; }
        }

        public List<Card.Cards> HandleMulligan(List<Card.Cards> Choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            var hand = HandleMinions(Choices, _whiteList, opponentClass);
            _whiteList.AddOrUpdate(Coin, true);

            _whiteList.AddOrUpdate(HandleWeapons(Choices, hand), false); // only 1 weapon is allowed
            _whiteList.AddOrUpdate(HandleSpells(Choices, hand), false); // only 1 spell is allowed

            using (
                System.IO.StreamWriter file =
                    new System.IO.StreamWriter(
                        AppDomain.CurrentDomain.BaseDirectory + "\\MulliganProfiles\\MulliganChoicesArchive.txt", true))
            {
                file.WriteLine("Your Card options were:");
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
                    "========================================------------");
                file.WriteLine(
                    "=====COMMENT SECTION[Agree? No? Why?]:");
                file.WriteLine(
                    "========================================------------");

                file.WriteLine("--------------------------------------");
                file.WriteLine("Your average deck mana cost is " +
                               Bot.CurrentDeck().Cards.Average(c => CardTemplate.LoadFromId(c).Cost));
                file.WriteLine("--------------------------------------");
                file.WriteLine("----Your alternative options in your deck were: ");
                var allOneDrops = (from q in Bot.CurrentDeck().Cards
                                   select CardTemplate.LoadFromId(q.ToString())
                                       into qq
                                       where qq.Cost == 1 && !qq.IsSecret
                                       select qq.Name).ToList();
                var allTwoDrops = (from q in Bot.CurrentDeck().Cards
                                   select CardTemplate.LoadFromId(q.ToString())
                                       into qq
                                       where qq.Cost == 2 && !qq.IsSecret
                                       select qq.Name).ToList();
                var allThreeDrops = (from q in Bot.CurrentDeck().Cards
                                     select CardTemplate.LoadFromId(q.ToString())
                                         into qq
                                         where qq.Cost == 3 && !qq.IsSecret
                                         select qq.Name).ToList();
                var allFourDrops = (from q in Bot.CurrentDeck().Cards
                                    select CardTemplate.LoadFromId(q.ToString())
                                        into qq
                                        where qq.Cost == 4 && !qq.IsSecret
                                        select qq.Name).ToList();
                file.WriteLine("------------One Drops:--");
                foreach (var c in allOneDrops)
                    file.WriteLine(c);
                file.WriteLine("------------Two Drops:--");
                foreach (var c in allTwoDrops)
                    file.WriteLine(c);
                file.WriteLine("------------Three Drops:--");
                foreach (var c in allThreeDrops)
                    file.WriteLine(c);
                file.WriteLine("------------Four Drops:--");
                foreach (var c in allFourDrops)
                    file.WriteLine(c);
                var yourDeck = Bot.CurrentDeck().Cards.ToList();
                file.WriteLine("\n\nYOUR DECK string:\n");
                file.WriteLine("Import mulligan debug\n ");
                foreach (var w in yourDeck)
                {
                    var t = CardTemplate.LoadFromId(w);
                    file.Write("\"{0}\", ", t.Id);
                }
                file.WriteLine("\n import mulligan tester\n ");
                foreach (var w in yourDeck)
                {
                    var t = CardTemplate.LoadFromId(w);
                    file.Write("{0};", t.Id);
                }
                file.WriteLine("");
                file.WriteLine(
                    "============================================================================================");
                file.WriteLine(
                    "============================================================================================");
                file.WriteLine("END OF MULLIGAN AGAINST " + opponentClass + " As a " + ownClass);
                file.WriteLine(
                    "============================================================================================");
                file.WriteLine(
                    "============================================================================================");
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
            var coin = choices.Count() > 4;
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
                    if (!spells.IsSecret && spells.Cost == 2 && !hand.Item2 || coin)
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
                if (spells.Cost == 3 && spells.IsSecret && !hand.Item3 && coin &&
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
            IDictionary<string, bool> whiteList, Card.CClass opponentClass)
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

            foreach (var c in choices)
            {
                var minion = CardTemplate.LoadFromId(c.ToString());
                var prioCheck = GetPriority(minion, opponentClass);
                if (prioCheck <= 1 || minion.Cost >= 4) continue;
                if (!aggro &&_num2Drops == 2) continue;
                    
                if (minion.Cost == 2)
                {
                    has2Drop = true;
                    _num2Drops++;
                }
                if (minion.Cost == 3)
                {
                    has3Drop = true;
                    _num3Drops++;
                }
                whiteList.AddOrUpdate(minion.Id.ToString(), prioCheck > 4);
            }

            GetFourDrops(choices, badMinions, whiteList, aggro);


            return new Tuple<bool, bool, bool, bool>(has1Drop, has2Drop, has3Drop, has4Drop);
        }
        private static int GetPriority(CardTemplate c, Card.CClass opponentClass)
        {
            #region decks

            var myDeck = Bot.CurrentDeck().Cards.ToList();
            var numMechs = myDeck.Count(q => CardTemplate.LoadFromId(q).Race == Card.CRace.MECH);

            #endregion

            var value = 0;
            var vanila = c.Health == 3 && c.Atk == 2;
            var vanila3 = c.Health == 4 && c.Atk == 3;

            switch (c.Cost)
            {
                case 1:
                    {
                        if (c.Health == 3 && c.Atk == 2)
                            value += 5;
                        break;
                    }
                case 2:
                    {
                        value += 2;
                        if (c.Charge)
                            value--;
                        value++;
                        if (c.Atk == 1 && c.Health == 1)
                        {
                            value -= 2;
                            break;
                        }
                        value++;
                        if (c.Atk > c.Health + 1)
                        {
                            value--;
                            break;
                        }
                        if (c.Health > c.Atk + 1)
                        {
                            value--;
                            break;
                        }

                        if (c.Name.Equals("Mechwarper") && numMechs > 3)
                            value += 2;

                        if (c.Race == Card.CRace.MECH) //and choices have mechwarper
                            value++;
                        if (c.Race == Card.CRace.DEMON && c.HasBattlecry)
                            value -= -5;
                        value++;
                        if (vanila && opponentClass == Card.CClass.PALADIN)
                            value++;


                        break;
                    }
                case 3:
                    {
                        if (vanila3)
                            value++;
                        if (c.HasBattlecry)
                            value++;
                        value++;
                        break;
                    }
                case 4:
                    {
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
                value--;
            if (c.Health > c.Cost && c.Atk > c.Cost)
                value += 2;
            if (c.Divineshield) value += 2;
            if (c.Divineshield && c.Health == c.Cost && c.Atk == c.Cost)
                value += 2;
            if (c.Stealth) value++;
            value += c.Health - 1 + c.Atk - 1;
            value -= 3;

            return value;
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