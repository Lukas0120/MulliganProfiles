using System;
using System.Collections.Generic;
using System.Linq;
using SmartBot.Database;
using SmartBot.Mulligan;
using SmartBot.Plugins.API;

namespace SmartBotUI.Mulligan
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


        public bMulliganProfile()
        {
            _whiteList = new Dictionary<string, bool>();
            _cardsToKeep = new List<Card.Cards>();
        }

        public List<Card.Cards> HandleMulligan(List<Card.Cards> Choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            var hand = HandleMinions(Choices, _whiteList);
            _whiteList.AddOrUpdate(Coin, true);

            _whiteList.AddOrUpdate(HandleWeapons(Choices, hand), false);// only 1 weapon is allowed
            _whiteList.AddOrUpdate(HandleSpells(Choices, hand), false); // only 1 spell is allowed
            Bot.Log("Your Card options are");
           
            foreach (var q in Choices)
                Bot.Log(CardTemplate.LoadFromId(q.ToString()).ToString());
            Bot.Log("");
            Bot.Log("Mulligan pick the following cards to keep ");
            foreach (var s in from s in Choices
                              let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString())
                              where _whiteList.ContainsKey(s.ToString())
                              where !keptOneAlready | _whiteList[s.ToString()]
                              select s)
            {
                Bot.Log(CardTemplate.LoadFromId(s.ToString()).ToString());
                _cardsToKeep.Add(s);
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
                "EX1_277", "GVG_001","CS2_024","CS2_027","GVG_003",     
                "CS2_045","EX1_248",                                        
                "CS1_130","GVG_010","EX1_339",
                "EX1_130","FP1_020","AT_074","GVG_061",
                "AT_064","EX1_391","EX1_606",
                "EX1_302","GVG_015","GVG_045",
                "DS1_184","NEW1_031","BRM_013","EX1_538",
                "CS2_074","AT_033","AT_035","CS2_072",
                "EX1_169","CS2_013","AT_037","EX1_160","EX1_154"
            };
            var badSecrets = new List<string>
                /*Bad secrets
             *Hunter: Snipe, Misdirection
             *Mage: Spellbender, Counterspell, Vaporize
             *Paladin: Eye for an Eye, Competetive Spirit, Redemption, Repentance
             */
            {
                "EX1_609", "EX1_533",
                "tt_010", "EX1_287", "EX1_594",
                "EX1_132", "AT_073", "EX1_136","EX1_379"
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
                if (spells.Cost == 2 && spells.IsSecret && !hand.Item2)
                    return c.ToString();
                if (spells.Cost == 3 && spells.IsSecret && !hand.Item3 && coin)
                    return c.ToString();
            }

            return "";
        }

        /// <summary>
        /// 
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
        private static Tuple<bool, bool, bool, bool> HandleMinions(List<Card.Cards> choices, IDictionary<string, bool> whiteList)
        { 
            
            var one = Bot.CurrentDeck().Cards.Count(c => CardTemplate.LoadFromId(c).Cost == 1);
            var two = Bot.CurrentDeck().Cards.Count(c => CardTemplate.LoadFromId(c).Cost == 2);
            var three = Bot.CurrentDeck().Cards.Count(c => CardTemplate.LoadFromId(c).Cost == 3);
            var four = Bot.CurrentDeck().Cards.Count(c => CardTemplate.LoadFromId(c).Cost == 4);
            var five = Bot.CurrentDeck().Cards.Count(c => CardTemplate.LoadFromId(c).Cost == 5);
            var allOneDrops = (from q in Bot.CurrentDeck().Cards select CardTemplate.LoadFromId(q.ToString()) into qq where qq.Cost == 1 select qq.Name).ToList();
            var allTwoDrops = (from q in Bot.CurrentDeck().Cards select CardTemplate.LoadFromId(q.ToString()) into qq where qq.Cost == 2 select qq.Name).ToList();
            var allThreeDrops = (from q in Bot.CurrentDeck().Cards select CardTemplate.LoadFromId(q.ToString()) into qq where qq.Cost == 3 select qq.Name).ToList();
            var allFourDrops = (from q in Bot.CurrentDeck().Cards select CardTemplate.LoadFromId(q.ToString()) into qq where qq.Cost == 4 select qq.Name).ToList();
           
            var divineShield = Bot.CurrentDeck().Cards.Count(c => CardTemplate.LoadFromId(c).Divineshield);
            var spells =
                Bot.CurrentDeck()
                    .Cards.Count(c => CardTemplate.LoadFromId(c).Type == Card.CType.SPELL);
            var weapons =
                Bot.CurrentDeck()
                    .Cards.Count(c => CardTemplate.LoadFromId(c).Type == Card.CType.WEAPON);
            var mechs =
                Bot.CurrentDeck()
                    .Cards.Count(c => CardTemplate.LoadFromId(c).Race == Card.CRace.MECH);
            var beast =
                Bot.CurrentDeck()
                    .Cards.Count(c => CardTemplate.LoadFromId(c).Race == Card.CRace.BEAST);
            var demon =
                Bot.CurrentDeck()
                    .Cards.Count(c => CardTemplate.LoadFromId(c).Race == Card.CRace.DEMON);
            
            Bot.Log(string.Format("\n============================" +
                                  "\nMulligan adjusted with the following knowlege. \n\n" +
                                  " Your current arena deck contains:" +
                                  " \n Mechs {0} \n Beasts {1} \n Demons {2} \n Divine Shield Minions {3}\n" +
                                  " ============================\n" +
                                  "Minion Curve Breakdown \n" +
                                  "# 1 Mana Drops {4}\n" +
                                  "# 2 Mana Drops {5}\n" +
                                  "# 3 Mana Drops {6}\n" +
                                  "# 4 Mana Drops {7}\n" +
                                  "# 5 Mana Drops {8}\n\n" +
                                  "Weapon Count {9}\n" +
                                  "Spell Count {10}\n", mechs, beast, demon, divineShield, one, two, three, four, five, weapons, spells));

            // var has1Drop = false;
            var has2Drop = false;
            var has3Drop = false;
            var has4Drop = false;
            var num2Drops = 0;
            var num3Drops = 0;
            var hasCoin = choices.Count > 3;
            var badMinions = new List<string>
            {
                "CS2_173", "CS2_203", "FP1_017", "EX1_045", "NEW1_037", "EX1_055", "EX1_058", "NEW1_021",
                "GVG_025", "GVG_039", "EX1_306", "EX1_084", "EX1_582", "GVG_084", "CS2_118", "CS2_122", 
                "CS2_124", "EX1_089", "EX1_050", "GVG_089", "EX1_005", "EX1_595", "EX1_396", "EX1_048", "AT_091",
                "EX1_584", "EX1_093","GVG_094", "GVG_109","GVG_107", "DS1_175", "EX1_362", "GVG_122","EX1_011", "AT_075" ,"EX1_085"
            };
            GetOneDrops(choices, badMinions, whiteList);


            //checkTwoDrop()
            foreach (var c in choices)
            {
                if (badMinions.Contains(c.ToString()) || CardTemplate.LoadFromId(c.ToString()).Cost != 2) continue;
                var minion = CardTemplate.LoadFromId(c.ToString());
                if (minion.Type == Card.CType.MINION)
                {
                    whiteList.AddOrUpdate(GetBestOne(choices, 2, badMinions), false);
                    num2Drops++;
                    has2Drop = true;
                }
                if (minion.Type == Card.CType.MINION && hasCoin)
                {
                    whiteList.AddOrUpdate(c.ToString(), true);
                    num2Drops++;
                }
                if (num2Drops >= 2) break;
            }
            //CheckThreeDrops();
            foreach (var c in choices)
            {

                if (badMinions.Contains(c.ToString()) || CardTemplate.LoadFromId(c.ToString()).Cost != 3) continue;

                var minion = CardTemplate.LoadFromId(c.ToString());
                if (minion.Type == Card.CType.MINION)
                {
                    whiteList.AddOrUpdate(GetBestOne(choices, 3, badMinions), hasCoin);
                    num3Drops++;
                    has3Drop = true;
                }
                if (has2Drop || num3Drops > 1 || minion.Health <= 2 || !hasCoin || badMinions.Contains(c.ToString()))
                    continue;
                whiteList.AddOrUpdate(c.ToString(), false);
                num3Drops++;
                has3Drop = true;
            }
            //checkFourDrops();
            foreach (var minion in choices.Where(c => !badMinions.Contains(c.ToString()) && has3Drop).Where(c => CardTemplate.LoadFromId(c.ToString()).Cost == 4 && has3Drop || hasCoin).Select(c => CardTemplate.LoadFromId(c.ToString())).Where(minion => minion.Type == Card.CType.MINION))
            {
                whiteList.AddOrUpdate(GetBestOne(choices, 4, badMinions), false);
                has4Drop = true;
            }

            return new Tuple<bool, bool, bool, bool>(true, has2Drop, has3Drop, has4Drop);
        }

        private static void GetOneDrops(List<Card.Cards> choices, List<string> badMinions, IDictionary<string, bool> whiteList, bool allowTwo = false)
        {
            var gotOne = false;
            foreach (var c in choices)
            {
                if (gotOne) break;
                if (badMinions.Contains(c.ToString()) || CardTemplate.LoadFromId(c.ToString()).Cost != 1) continue;
                CardTemplate minion = CardTemplate.LoadFromId(c.ToString());
                if (!badMinions.Contains(c.ToString()) && minion.Type == Card.CType.MINION && minion.Atk >= 1)
                {
                    gotOne = true;
                    whiteList.AddOrUpdate(GetBestOne(choices, 1, badMinions), false);
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
            return choices.Where(c => !badMinions.Contains(c.ToString())).Where(c => CardTemplate.LoadFromId(c.ToString()).Cost == cost).Aggregate("CS2_118", (current, c) => WhichIsStronger(current, c.ToString()));
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