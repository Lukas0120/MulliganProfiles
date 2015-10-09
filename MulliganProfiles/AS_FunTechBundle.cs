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
        /// <summary>
        ///     Boolean Logic Place
        /// </summary>
        /// ////////////////////////////////// //
        private const bool MMFelRever = true; // [Mech Mage] Keeps felReaver on a curve


        /// ////////////////////////////////// //
        private const string Coin = "GAME_005";

        private readonly List<string> _removalSpells;
        private readonly Dictionary<string, bool> _whiteList; // CardName, KeepDouble
        private readonly List<Card.Cards> _cardsToKeep;


        public bMulliganProfile()
        {
            _removalSpells = new List<string> { "EX1_246", "CS2_022", "CS2_076", "AT_048" };
            _whiteList = new Dictionary<string, bool>();
            _cardsToKeep = new List<Card.Cards>();
        }


        public List<Card.Cards> HandleMulligan(List<Card.Cards> Choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            var hasCoin = Choices.Count > 3;
            _whiteList.AddOrUpdate(Coin, true);

            #region Class Specific Mulligan

            switch (ownClass)
            {
                case Card.CClass.DRUID:
                    {
                        break;
                    }
                case Card.CClass.HUNTER:
                    {
                        break;
                    }
                case Card.CClass.MAGE:
                    {
                        MulliganMage(Choices, _whiteList, opponentClass, hasCoin);
                        break;
                    }
                case Card.CClass.PALADIN:
                    {
                        break;
                    }

                case Card.CClass.PRIEST:
                    {
                        break;
                    }
                case Card.CClass.ROGUE:
                    {
                        break;
                    }
                case Card.CClass.SHAMAN:
                    {
                        MulliganShaman(Choices, _whiteList, opponentClass, hasCoin, _removalSpells);
                        break;
                    }
                case Card.CClass.WARLOCK:
                    {
                        break;
                    }
                case Card.CClass.WARRIOR:
                    {
                        MulliganWarrior(Choices, _whiteList, opponentClass, hasCoin);
                        break;
                    }
            }

            #endregion

            foreach (var s in from s in Choices
                              let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString())
                              where _whiteList.ContainsKey(s.ToString())
                              where !keptOneAlready | _whiteList[s.ToString()]
                              select s)
                _cardsToKeep.Add(s);


            return _cardsToKeep;
        }

        /// <summary>
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="whiteList"></param>
        /// <param name="opponentClass"></param>
        /// <param name="hasCoin"></param>
        /// <param name="removalSpells"></param>
        private static void MulliganShaman(List<Card.Cards> choices, Dictionary<string, bool> whiteList, Card.CClass opponentClass,
            bool hasCoin, List<string> removalSpells)
        {
            //var gotWeapon = false;
            var control = false;
            var oneDrop = false;
            var twoDrop = false;
            var threeDrop = false;

            foreach (var c in choices)
            {
                var temp = CardTemplate.LoadFromId(c.ToString());
                if (temp.Race == SmartBot.Plugins.API.Card.CRace.TOTEM && temp.Cost == 2 && temp.Atk != 0)
                {
                    twoDrop = true;
                    whiteList.AddOrUpdate(c.ToString(), hasCoin);
                }

                switch (opponentClass)
                {
                    case Card.CClass.SHAMAN:
                        control = true;
                        break;
                    case Card.CClass.PRIEST:
                        control = true;
                        break;
                    case Card.CClass.MAGE:
                        //control = true;
                        break;
                    case Card.CClass.PALADIN:
                        break;
                    case Card.CClass.WARRIOR:
                        control = true;
                        break;
                    case Card.CClass.WARLOCK:
                        {
                            whiteList.AddOrUpdate("EX1_245", false); //earth shock because I assume it's a handlock
                            control = true;
                        }
                        break;
                    case Card.CClass.HUNTER:
                        break;
                    case Card.CClass.ROGUE:
                        control = true;
                        break;
                    case Card.CClass.DRUID: // I assume that druids on the ladder are aggro
                        //control = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("opponentClass", opponentClass, null);
                }
                if (temp.Type == SmartBot.Plugins.API.Card.CType.MINION &&
                    temp.Quality != SmartBot.Plugins.API.Card.CQuality.Epic)
                {
                    if (temp.Cost == 1)
                    {
                        oneDrop = true;
                        whiteList.AddOrUpdate(c.ToString(), !control);
                    }
                    if (temp.Cost == 2 && temp.Atk > 0)
                    {
                        twoDrop = true;
                        whiteList.AddOrUpdate(c.ToString(), false);
                    }
                    if (!control && temp.Cost == 3 && temp.Atk > 1)
                    {
                        threeDrop = true;
                        whiteList.AddOrUpdate(c.ToString(), hasCoin);
                    }
                }
                var curve = twoDrop || threeDrop && hasCoin;
                if (curve && temp.Cost == 4 && temp.Atk > 3)
                {
                    whiteList.AddOrUpdate(c.ToString(), false);
                }
            }
            foreach (var c in choices)
            {
                var temp = CardTemplate.LoadFromId(c.ToString());
                if (temp.Type != SmartBot.Plugins.API.Card.CType.SPELL) continue;
                if (control && temp.Cost == 1) whiteList.AddOrUpdate(c.ToString(), false);
                if (!control && temp.Cost == 3 && !removalSpells.Contains(c.ToString())) whiteList.AddOrUpdate(c.ToString(), false);
            }
        }

        /// <summary>
        ///     Mech Mages
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="whiteList"></param>
        /// <param name="opponentClass"></param>
        /// <param name="hasCoin"></param>
        private static void MulliganMage(List<Card.Cards> choices, Dictionary<string, bool> whiteList, Card.CClass opponentClass,
            bool hasCoin)
        {
            var control = false;
            var oneDrop = false;
            var twoDrop = false;
            var threeDrop = false;
            var curve = false;
            CardTemplate oneManaMinion = null;
            foreach (var c in choices)
            {
                var temp = CardTemplate.LoadFromId(c.ToString());
                if (temp.Race == SmartBot.Plugins.API.Card.CRace.MECH)
                {
                    if (temp.Cost == 1)
                        oneManaMinion = CardTemplate.LoadFromId(c.ToString());
                    if (temp.Cost == 2)
                    {
                        twoDrop = true;
                        whiteList.AddOrUpdate(c.ToString(), true);
                    }

                }
                if (temp.Race != SmartBot.Plugins.API.Card.CRace.MECH &&
                    temp.Quality != SmartBot.Plugins.API.Card.CQuality.Epic &&
                    temp.Type != SmartBot.Plugins.API.Card.CType.SPELL) //minions handler
                {
                    if (temp.Cost == 1 && temp.Race != SmartBot.Plugins.API.Card.CRace.MECH)
                    {
                        oneDrop = true;
                        whiteList.AddOrUpdate(c.ToString(), false);
                    }
                    if (temp.Cost == 2)
                    {
                        twoDrop = true;
                        whiteList.AddOrUpdate(c.ToString(), true);
                    }

                }

                switch (opponentClass)
                {
                    case Card.CClass.SHAMAN:
                        control = true;
                        break;
                    case Card.CClass.PRIEST:
                        control = true;
                        break;
                    case Card.CClass.MAGE:
                        break;
                    case Card.CClass.PALADIN:
                        {
                            if (temp.Cost == 3 && temp.Quality == SmartBot.Plugins.API.Card.CQuality.Epic)
                                whiteList.AddOrUpdate(c.ToString(), false);
                            break;
                        }
                    case Card.CClass.WARRIOR:
                        control = true;
                        break;
                    case Card.CClass.WARLOCK:
                        control = true;
                        break;
                    case Card.CClass.HUNTER:
                        break;
                    case Card.CClass.ROGUE:
                        control = true;
                        break;
                    case Card.CClass.DRUID:
                        control = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("opponentClass", opponentClass, null);
                }

                if (temp.Type == SmartBot.Plugins.API.Card.CType.SPELL && temp.Cost == 2)
                    whiteList.AddOrUpdate(c.ToString(), false);

                if (!control)
                {
                    if (temp.Type == SmartBot.Plugins.API.Card.CType.SPELL && temp.Cost == 1)
                        whiteList.AddOrUpdate(c.ToString(), false);
                    continue;
                }
                if (temp.Cost == 4 && temp.Race == SmartBot.Plugins.API.Card.CRace.MECH && temp.Atk > 3)
                    whiteList.AddOrUpdate(c.ToString(), false);

            }
            foreach (var c in from c in choices let temp = CardTemplate.LoadFromId(c.ToString()) where twoDrop && temp.Cost == 3 && temp.Type == SmartBot.Plugins.API.Card.CType.MINION select c)
            {
                threeDrop = true;
                whiteList.AddOrUpdate(c.ToString(), hasCoin);
            }
            curve = twoDrop && threeDrop;
            if (oneManaMinion != null && (!oneDrop && oneManaMinion.Cost == 1))
                whiteList.AddOrUpdate("GVG_082", false);
            if (!threeDrop && !hasCoin) return;
            if (MMFelRever && threeDrop && hasCoin && choices.Any(c => c.ToString() == "GVG_016"))
                whiteList.AddOrUpdate("GVG_016", false);
            if (choices.Any(c => c.ToString() == "GVG_004") && threeDrop || curve)
                whiteList.AddOrUpdate("GVG_004", false);
        }

        /// <summary>
        ///     Supported deck arthetypes:
        ///     Mech Warrior
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="whiteList"></param>
        /// <param name="opponentClass"></param>
        /// <param name="coin"></param>
        private static void MulliganWarrior(List<Card.Cards> choices, IDictionary<string, bool> whiteList,
            Card.CClass opponentClass, bool coin)
        {
            var gotWeapon = false;
            var control = false;
            var oneDrop = false;
            var twoDrop = false;
            var threeDrop = false;
            var weapons = new List<string>();

            foreach (var c in choices)
            {
                var temp = CardTemplate.LoadFromId(c.ToString());
                if (!gotWeapon && temp.Type == SmartBot.Plugins.API.Card.CType.WEAPON && temp.Cost == 2)
                {
                    twoDrop = true;
                    whiteList.AddOrUpdate(c.ToString(), false);
                    gotWeapon = true;
                }
                if (temp.Race == SmartBot.Plugins.API.Card.CRace.MECH)
                {
                    if (temp.Cost == 1)
                    {
                        oneDrop = true;
                        whiteList.AddOrUpdate(c.ToString(), false);
                    }
                    if (temp.Cost == 2)
                    {
                        twoDrop = true;
                        whiteList.AddOrUpdate(c.ToString(), false);
                    }
                    if (temp.Cost == 3)
                    {
                        threeDrop = true;
                        whiteList.AddOrUpdate(c.ToString(), false);
                    }
                }
                if (temp.Type != SmartBot.Plugins.API.Card.CType.WEAPON && temp.Cost <= 3 &&
                    temp.Type != SmartBot.Plugins.API.Card.CType.SPELL &&
                    temp.Race != SmartBot.Plugins.API.Card.CRace.MECH)
                {
                    whiteList.AddOrUpdate(c.ToString(), false);
                }
                switch (opponentClass)
                {
                    case Card.CClass.SHAMAN:
                        control = true;
                        break;
                    case Card.CClass.PRIEST:
                        control = true;
                        break;
                    case Card.CClass.MAGE:
                        //control = true;
                        break;
                    case Card.CClass.PALADIN:
                        break;
                    case Card.CClass.WARRIOR:
                        control = true;
                        break;
                    case Card.CClass.WARLOCK:
                        control = true;
                        break;
                    case Card.CClass.HUNTER:
                        break;
                    case Card.CClass.ROGUE:
                        control = true;
                        break;
                    case Card.CClass.DRUID:
                        control = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("opponentClass", opponentClass, null);
                }
                if (!control) continue;
                //var curve = twoDrop && threeDrop;
                if (temp.Type != SmartBot.Plugins.API.Card.CType.WEAPON && twoDrop && temp.Cost == 4 &&
                    temp.Race == SmartBot.Plugins.API.Card.CRace.MECH && temp.Atk > 3)
                    whiteList.AddOrUpdate(c.ToString(), false);
                //if (coin && temp.Quality == SmartBot.Plugins.API.Card.CQuality.Epic)
                //    _whiteList.AddOrUpdate(c.ToString(),false);
            }
            foreach (var c in from c in choices let temp = CardTemplate.LoadFromId(c.ToString()) where !gotWeapon && temp.Type == SmartBot.Plugins.API.Card.CType.WEAPON && temp.Cost > 2 && temp.Cost < 5 select c)
            {
                gotWeapon = true;
                whiteList.AddOrUpdate(c.ToString(), false);
                break;
            }
        }
    }
}