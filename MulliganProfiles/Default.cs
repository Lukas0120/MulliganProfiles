using System;
using System.Collections.Generic;
using System.Linq;
using SmartBot.Database;
using SmartBot.Plugins.API;

namespace SmartBot.Mulligan
{
    [Serializable]
    public class DefaultMulliganProfile : MulliganProfile
    {
        private readonly List<Card.Cards> OneManaMinionsToAlwaysKeep = new List<Card.Cards>
        {
            /* --Neutral-- */
            Card.Cards.EX1_080, //Secretkeeper
            Card.Cards.GVG_082, //Clockwork Gnome
            Card.Cards.EX1_008, //Argent Squire
            Card.Cards.EX1_029, //Leper Gnome
            Card.Cards.FP1_001, //Zombie Chow
            Card.Cards.EX1_010, //Worgen Infiltrator

            /* --Priest-- */
            Card.Cards.BRM_004, //Twilight Whelp
            Card.Cards.CS2_235, //Northshire Cleric

            /* --Hunter-- */
            Card.Cards.FP1_011, //Webspinner

            /* --Mage-- */
            Card.Cards.NEW1_012, //Mana Wyrm

            /* --Warlock-- */
            Card.Cards.EX1_319, //Flame Imp
            Card.Cards.CS2_065, //Voidwalker

            /* --Rogue-- */
            Card.Cards.AT_029 //Buccaneer
        };

        private readonly List<Card.Cards> SpellsAndWeaponsToAlwaysKeep = new List<Card.Cards>
        {
            /* --Warrior-- */
            Card.Cards.CS2_106, //Fiery War Axe

            /* --Priest-- */
            Card.Cards.CS2_234, //Shadow Word: Pain

            /* --Mage-- */
            Card.Cards.CS2_024, //Frostbolt
            Card.Cards.GVG_001, //Flamecannon
            Card.Cards.GVG_003, //Unstable Portal

            /* --Warlock-- */
            Card.Cards.GVG_015, //Darkbomb

            /* --Rogue-- */
            Card.Cards.CS2_072, //Backstab

            /* --Shaman-- */
            Card.Cards.EX1_245, //Earth Shock
            Card.Cards.CS2_045, //Rockbiter Weapon

            /* --Druid-- */
            Card.Cards.AT_037, //Living Roots
            Card.Cards.EX1_169, //Innervate
            Card.Cards.EX1_160, //Power of the Wild
            Card.Cards.EX1_154, //Wrath
            Card.Cards.CS2_013 //Wild Growth
        };

        public List<Card.Cards> HandleMulligan(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            var CardsToKeep = new List<Card.Cards>();
            var HasCoin = choices.Count == 4;

            var WorthyOneDrop = choices.Count(x => IsWorthyOneDrop(x));
            var WorthyTwoDrop = choices.Count(x => IsWorthyTwoDrop(x));
            var WorthyThreeDrop = choices.Count(x => IsWorthyThreeDrop(x));
            var WorthyFourDrop = choices.Count(x => IsWorthyFourDrop(x));

            var MaxOneDrop = HasCoin && WorthyOneDrop == 2 && WorthyTwoDrop >= 1 && WorthyThreeDrop >= 1 ? 2 : 1;

            var MaxTwoDrop = WorthyOneDrop >= 1 && (WorthyThreeDrop >= 1 || (WorthyFourDrop > 1 && HasCoin)) ? 1 : 3;

            var MaxThreeDrop = WorthyTwoDrop > 0 ? 1 : 0;

            var MaxFourDrop = HasCoin && WorthyTwoDrop == 1 ? 1 : 0;

            choices.ForEach(delegate(Card.Cards cardid)
            {
                if (IsWorthyOneDrop(cardid) && CardsToKeep.Count(x => IsWorthyOneDrop(x)) < MaxOneDrop)
                    CardsToKeep.Add(cardid);
                if (IsWorthyTwoDrop(cardid) && CardsToKeep.Count(x => IsWorthyTwoDrop(x)) < MaxTwoDrop)
                    CardsToKeep.Add(cardid);
                if (IsWorthyThreeDrop(cardid) && CardsToKeep.Count(x => IsWorthyThreeDrop(x)) < MaxThreeDrop)
                    CardsToKeep.Add(cardid);
                if (IsWorthyFourDrop(cardid) && CardsToKeep.Count(x => IsWorthyFourDrop(x)) < MaxFourDrop)
                    CardsToKeep.Add(cardid);
            });

            return CardsToKeep;
        }

        private bool IsWorthyOneDrop(Card.Cards id)
        {
            return (OneManaMinionsToAlwaysKeep.Contains(id));
        }

        private bool IsWorthyTwoDrop(Card.Cards id)
        {
            if (SpellsAndWeaponsToAlwaysKeep.Contains(id)) return true;
            return CardTemplate.LoadFromId(id).Cost == 2 && CardTemplate.LoadFromId(id).Atk >= 2 &&
                   CardTemplate.LoadFromId(id).Health >= 2 && CardTemplate.LoadFromId(id).Type == Card.CType.MINION;
        }

        private bool IsWorthyThreeDrop(Card.Cards id)
        {
            return CardTemplate.LoadFromId(id).Cost == 3 && CardTemplate.LoadFromId(id).Atk >= 3 &&
                   CardTemplate.LoadFromId(id).Health >= 3 && CardTemplate.LoadFromId(id).Type == Card.CType.MINION;
        }

        private bool IsWorthyFourDrop(Card.Cards id)
        {
            return CardTemplate.LoadFromId(id).Cost == 4 && CardTemplate.LoadFromId(id).Type == Card.CType.MINION;
        }
    }
}