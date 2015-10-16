using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartBot.Mulligan;
using SmartBot.Plugins.API;
using SmartBotUI;


namespace SmartBotUI.Mulligan.MC
{
    public static class Extension
    {
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            map[key] = value;
        }
    }

    [Serializable]
    public class bMulliganProfile : MulliganProfile
    {
        #region Data
        /*==================Custom Behavior Logic==================
         *                                                        *
         *  Warning: custom behavior doesn't apply to all classes *
         *           refer to the forum thread for more details   *
         *                                                        *
         *  If you do not agree with some of the custom           *  
         *  mulligan scenarios you can set their respective       *
         *  boolean value to false                                *
         *                                                        * 
         *      True = this behavior will occur                   *
         *      False = it will avoid that particular             *
         *              mulligan logc                             * 
         *========================================================*/
        private bool _midrangeSecretPaladin = false;                   //Midrange Secret Paladin based mulligan. 
        //                Keep rush and anti control on false.

        private const bool CompetitiveMustard = true; //CM     //Keeps Competitive Spirit with Muster for Battle on coin
        private const bool VengefulSecretKeeper = false;         //VSK    //Keeps noble sacrifice with avenge when you have a secretkeeper
        private bool _nobleJuggler = true;                  //NKJ    //Keeps noble sacrifice with knife juggler
        private const bool Redeeming2Drops = true;               //R2D    //Keep  redemption with Shielded Minibot, or Harvest Golem
        private const bool KeepBloodKnightOnCurve = false;      //KBKoC  //Keeps Blood Knight on divine shield curve
        private const bool MysteriousChallengerCoin = true;    //MCC    //Keep  Mysterious Challenger on coin
        private const bool MysteriousChallengerForever = true; //MCF    //Always keeps Mysterious Challenger even without coin
        private const bool CoghammerLogic = true;                //Cog    //Keeps coghammer on curve, but never against warriors


        /* Values for reference:***********************************************************************/
        /***********    CM       VSK       NKJ       R2D       KBKoC       MCF       MCC       Cog    *
        /* v2.1.2:      false    false     true      true      false      false     true       false  *
         * v2.2.1:      true     true      true      true      true       false     true       false  *
         * Dr. 6:       false    false     false     false     false      true      false      false  *
         * Wbulot dr6:  false    false     false     false     false      false     false      false  *
         * ThyFate:     true     false     true      true      false      false     true       true   
         * ********************************************************************************************
         */


        /**********************************************************/
        /*==============End of Custom Behavior Logic==============*/
        /*********Do not change anything below this line***********/
        /**********************************************************/

        private const string AbusiveSergeant = "CS2_188";
        private const string Annoyatron = "GVG_085";
        private const string ArgentSquire = "EX1_008";
        private const string Avenge = "FP1_020";
        private const string BloodKnight = "EX1_590";
        private const string Coin = "GAME_005";
        private const string Consecration = "CS2_093";
        private const string CompetitiveSpirit = "AT_073";
        private const string HarvestGolem = "EX1_556";
        private const string HauntedCreeper = "FP1_002";
        private const string IronbeakOwl = "CS2_203";
        private const string KnifeJuggler = "NEW1_019";
        private const string LeperGnome = "EX1_029";
        private const string MadScientist = "FP1_004";
        private const string MusterForBattle = "GVG_061";
        private const string MysteriousChallenger = "AT_079";
        private const string NobleSacrifice = "EX1_130";
        private const string PilotedShredder = "GVG_096";
        private const string Redemption = "EX1_136";
        private const string Repentance = "EX1_379";
        private const string Secretkeeper = "EX1_080";
        private const string ShieldedMinibot = "GVG_058";
        private const string TruesilverChamption = "CS2_097";
        private const string Coghammer = "GVG_059";
        private const string ZombieChow = "FP1_001";


        private Dictionary<string, bool> _whiteList; // CardName, KeepDouble
        private List<Card.Cards> _cardsToKeep;

        #endregion Data

        #region Constructor

        public bMulliganProfile()
            : base()
        {
            _whiteList = new Dictionary<string, bool>();
            _cardsToKeep = new List<Card.Cards>();
        }

        #endregion Constructor

        #region Methods

        public List<Card.Cards> HandleMulligan(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            bool hasCoin = choices.Count > 3;
            bool has2Drop = (choices.Any(c => c.ToString() == HarvestGolem) ||choices.Any(c => c.ToString() == ShieldedMinibot) ||choices.Any(c => c.ToString() == MadScientist) ||choices.Any(c => c.ToString() == KnifeJuggler));
            bool lazyFlag = false;
            #region Default Mulligan

            _whiteList.AddOrUpdate(AbusiveSergeant, false);
            _whiteList.AddOrUpdate(ArgentSquire, true);
            _whiteList.AddOrUpdate(Coin, true); // Would be nice to keep double
            if (opponentClass != Card.CClass.WARRIOR)
                _whiteList.AddOrUpdate(HauntedCreeper, true);
            _whiteList.AddOrUpdate(KnifeJuggler, false);
            _whiteList.AddOrUpdate(LeperGnome, true);
            _whiteList.AddOrUpdate(MadScientist, true);
            _whiteList.AddOrUpdate(Secretkeeper, true);
            _whiteList.AddOrUpdate(ShieldedMinibot, true);
            _whiteList.AddOrUpdate(ZombieChow, true);



            #endregion Default Mulligan

           if (_midrangeSecretPaladin && hasCoin && has2Drop)
                _whiteList.AddOrUpdate(PilotedShredder, false);

            if (_midrangeSecretPaladin)
                _whiteList.AddOrUpdate(Avenge, false);

            switch (opponentClass)
            {
                case Card.CClass.DRUID:
                    {
                        if (hasCoin)
                            lazyFlag = true;
                        _whiteList.AddOrUpdate(MusterForBattle, false);
                        if (!_midrangeSecretPaladin)
                            _whiteList.AddOrUpdate(PilotedShredder, false);

                        break;
                    }
                case Card.CClass.HUNTER:
                    {
                        if (CoghammerLogic && has2Drop)
                            _whiteList.AddOrUpdate(Coghammer, false);
                        _whiteList.AddOrUpdate(Annoyatron, false);
                        if (_midrangeSecretPaladin && hasCoin)
                            _whiteList.AddOrUpdate(Consecration, false);
                        else if (!_midrangeSecretPaladin)
                            _whiteList.AddOrUpdate(Consecration, false);
                        break;
                    }
                case Card.CClass.MAGE:
                    {
                        if (hasCoin)
                            lazyFlag = true;
                        _whiteList.AddOrUpdate(MusterForBattle, false);
                        if (!_midrangeSecretPaladin)
                            _whiteList.AddOrUpdate(MysteriousChallenger, false);
                        else
                            _whiteList.AddOrUpdate(NobleSacrifice, false);

                        if (_midrangeSecretPaladin && hasCoin)
                            _whiteList.AddOrUpdate(Consecration, false);
                        else if (!_midrangeSecretPaladin)
                            _whiteList.AddOrUpdate(Consecration, false);
                        break;
                    }
                case Card.CClass.PALADIN:
                    {
                        if (hasCoin || has2Drop)
                        {
                            _whiteList.AddOrUpdate(BloodKnight, false);
                            _whiteList.AddOrUpdate(IronbeakOwl, false);
                        }
                        _whiteList.AddOrUpdate(Annoyatron, false);
                        _whiteList.AddOrUpdate(Consecration, false);

                        if (_midrangeSecretPaladin && hasCoin)
                            _whiteList.AddOrUpdate(Consecration, false);
                        else if (!_midrangeSecretPaladin)
                            _whiteList.AddOrUpdate(Consecration, false);

                        _whiteList.AddOrUpdate(MusterForBattle, false);
                        if (!_midrangeSecretPaladin)
                            _whiteList.AddOrUpdate(MysteriousChallenger, false);
                        break;
                    }
                case Card.CClass.PRIEST:
                    {
                        if (!_midrangeSecretPaladin)
                            _whiteList.AddOrUpdate(MysteriousChallenger, false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);

                        break;
                    }
                case Card.CClass.ROGUE:
                    {
                        if (hasCoin)
                            lazyFlag = true;
                        _whiteList.AddOrUpdate(MusterForBattle, false);
                        if (!_midrangeSecretPaladin)
                            _whiteList.AddOrUpdate(MysteriousChallenger, false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);

                        break;
                    }
                case Card.CClass.SHAMAN:
                    {
                        _whiteList.AddOrUpdate(MusterForBattle, false);
                        if (!_midrangeSecretPaladin)
                            _whiteList.AddOrUpdate(MysteriousChallenger, false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);
                        break;
                    }
                case Card.CClass.WARLOCK:
                    {
                        _whiteList.AddOrUpdate(Consecration, false);
                        _whiteList.AddOrUpdate(MusterForBattle, false);
                        break;
                    }
                case Card.CClass.WARRIOR:
                    {
                        _whiteList.AddOrUpdate(Annoyatron, false);
                        _whiteList.AddOrUpdate(MusterForBattle, false);
                        if (!_midrangeSecretPaladin && hasCoin)
                            _whiteList.AddOrUpdate(MysteriousChallenger, false);

                        _whiteList.AddOrUpdate(PilotedShredder, hasCoin);

                        break;
                    }
            }
            if (MysteriousChallengerForever)
                _whiteList.AddOrUpdate(MysteriousChallenger, false);

            if ((choices.Any(c => c.ToString() == Coghammer) && (CoghammerLogic && has2Drop) && (opponentClass != Card.CClass.WARRIOR)))
                _whiteList.AddOrUpdate(Coghammer, false);
            else if ((opponentClass == Card.CClass.WARRIOR) || (opponentClass == Card.CClass.PRIEST) || (opponentClass == Card.CClass.ROGUE) || (opponentClass == Card.CClass.DRUID))
                _whiteList.AddOrUpdate(TruesilverChamption, false);

            if ((CompetitiveMustard && lazyFlag) && (choices.Any(c => c.ToString() == CompetitiveSpirit) && choices.Any(c => c.ToString() == MusterForBattle)))
            {
                _whiteList.AddOrUpdate(MusterForBattle, false);
                _whiteList.AddOrUpdate(CompetitiveSpirit, false);
            }

            //Keep Mysterious Challenger on coin
            if (hasCoin && !_midrangeSecretPaladin && MysteriousChallengerCoin)
            {
                _whiteList.AddOrUpdate(Annoyatron, false);
                _whiteList.AddOrUpdate(MysteriousChallenger, false);
            }

            // Redemption and Harvest Golem are kept if you have both.
            if (Redeeming2Drops &&
               (choices.Any(c => c.ToString() == HarvestGolem) ||
                choices.Any(c => c.ToString() == ShieldedMinibot)
                ))
            {
                _whiteList.AddOrUpdate(HarvestGolem, false);
                _whiteList.AddOrUpdate(Redemption, false);
                // has2drop = true;
            }

            if ((opponentClass == Card.CClass.ROGUE) || (opponentClass == Card.CClass.DRUID)) //These classes can kill Defender if it's played on turn 1. 
                _nobleJuggler = false;

            // Noble Sacrifice is kept if you have Knife Juggler.
            if (_nobleJuggler && (choices.Any(c => c.ToString() == KnifeJuggler)))
                _whiteList.AddOrUpdate(NobleSacrifice, false);


            // Tech choice with blood knight
            if (KeepBloodKnightOnCurve &&
                (choices.Any(c => c.ToString() == ArgentSquire) && (choices.Any(c => c.ToString() == ShieldedMinibot) || choices.Any(c => c.ToString() == Annoyatron))))
            {
                _whiteList.AddOrUpdate(Annoyatron, false);
                _whiteList.AddOrUpdate(BloodKnight, false);
            }
            //Experimental segment that keeps noble sac and avenge with secret keeper. 
            if (VengefulSecretKeeper &&
                (choices.Any(c => c.ToString() == Avenge) &&
                (choices.Any(c => c.ToString() == Secretkeeper) &&
                 choices.Any(c => c.ToString() == NobleSacrifice))))
            {
                _whiteList.AddOrUpdate(NobleSacrifice, false);
                _whiteList.AddOrUpdate(Avenge, false);
            }


            #endregion

            foreach (Card.Cards s in from s in choices
                                     let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString())
                                     where _whiteList.ContainsKey(s.ToString())
                                     where !keptOneAlready | _whiteList[s.ToString()]
                                     select s)
                _cardsToKeep.Add(s);
            return _cardsToKeep;
        }
    }
}