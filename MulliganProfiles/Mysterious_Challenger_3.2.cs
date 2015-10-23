using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartBot.Mulligan;
using SmartBot.Plugins.API;
using SmartBotUI;


namespace SmartBotUI.Mulligan.MCUpdate
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

        private bool _midrangeSecretPaladin = false;
        //If above is true, Mysterious Challenger logic should be set to false on all 3. 
        private bool _neverMysteriousChallenger = false; 
        private bool _mysteriousChallengerCoin = true;
        private bool _mysteriousChallengerForever = false;
        

        //SECRET PALADIN SPECIFIC
        private const bool CompetitiveMustard = true;
        private bool _nobleJuggler = true;
        private const bool Redeeming2Drops = true;
        private const bool CoghammerLogic = true;

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
        private const string Secretkeeper = "EX1_080";
        private const string ShieldedMinibot = "GVG_058";
        private const string TruesilverChamption = "CS2_097";
        private const string Coghammer = "GVG_059";
        private const string ZombieChow = "FP1_001";


        private readonly Dictionary<string, bool> _whiteList; // CardName, KeepDouble
        private readonly List<Card.Cards> _cardsToKeep;

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
            var hasCoin = choices.Count > 3;
            var has2Drop = (choices.Any(c => c.ToString() == ShieldedMinibot) || choices.Any(c => c.ToString() == MadScientist) || choices.Any(c => c.ToString() == KnifeJuggler));
            var lazyFlag = false;
            #region Default Mulligan
            /*Prevent any sort of reduntant custom logics*/
            FixRedundancy();
            _whiteList.AddOrUpdate(AbusiveSergeant, false);
            _whiteList.AddOrUpdate(ArgentSquire, true);
            _whiteList.AddOrUpdate(Coin, true); // Would be nice to keep double
            if (opponentClass != Card.CClass.WARRIOR)
                _whiteList.AddOrUpdate(HauntedCreeper, false);
            _whiteList.AddOrUpdate(KnifeJuggler, false);
            _whiteList.AddOrUpdate(LeperGnome, true);
            _whiteList.AddOrUpdate(MadScientist, true);
            _whiteList.AddOrUpdate(Secretkeeper, false);
            _whiteList.AddOrUpdate(ShieldedMinibot, true);
            _whiteList.AddOrUpdate(ZombieChow, true);
            _whiteList.AddOrUpdate(has2Drop ? HarvestGolem:"", false);
            _whiteList.AddOrUpdate(choices.Any(c=>c.ToString() == KnifeJuggler) ? MusterForBattle: "", false);
            #endregion Default Mulligan

            _whiteList.AddOrUpdate(_midrangeSecretPaladin ? Avenge : "", false);
            _whiteList.AddOrUpdate(_mysteriousChallengerForever ? MysteriousChallenger : "", false);
            _whiteList.AddOrUpdate(_mysteriousChallengerCoin && hasCoin ? MysteriousChallenger : "", false);
            _whiteList.AddOrUpdate((_nobleJuggler && (choices.Any(c => c.ToString() == KnifeJuggler))) ? NobleSacrifice : "", false);
            _whiteList.AddOrUpdate(choices.Any(c => c.ToString() == ShieldedMinibot) && Redeeming2Drops ? Redemption : "", false);
            switch (opponentClass)
            {
                case Card.CClass.DRUID:
                    {

                        lazyFlag = hasCoin;
                        _whiteList.AddOrUpdate(MusterForBattle, false);
                        _whiteList.AddOrUpdate(!_midrangeSecretPaladin ? PilotedShredder : "", false);

                        break;
                    }
                case Card.CClass.HUNTER:
                    {
                        _whiteList.AddOrUpdate((CoghammerLogic && has2Drop) ? Coghammer : "", false);
                        _whiteList.AddOrUpdate(Annoyatron, false);
                        _whiteList.AddOrUpdate((!_midrangeSecretPaladin && hasCoin ) ? Consecration : "", false);
                        break;
                    }
                case Card.CClass.MAGE:
                    {
                        lazyFlag = hasCoin;
                        _whiteList.AddOrUpdate(MusterForBattle, false);
                        _whiteList.AddOrUpdate(!_midrangeSecretPaladin && !_neverMysteriousChallenger ? MysteriousChallenger : NobleSacrifice, false);
                        _whiteList.AddOrUpdate(_midrangeSecretPaladin && hasCoin ? Consecration : "", false);
                        break;
                    }
                case Card.CClass.PALADIN:
                    {
                        _whiteList.AddOrUpdate(hasCoin || has2Drop ? BloodKnight : "", false);
                        _whiteList.AddOrUpdate(hasCoin || has2Drop ? IronbeakOwl : "", false);
                        _whiteList.AddOrUpdate(Annoyatron, false);
                        _whiteList.AddOrUpdate(Consecration, false);

                        if (_midrangeSecretPaladin && hasCoin)
                            _whiteList.AddOrUpdate(Consecration, false);
                        else if (!_midrangeSecretPaladin)
                            _whiteList.AddOrUpdate(Consecration, false);

                        _whiteList.AddOrUpdate(MusterForBattle, false);
                        if (!_midrangeSecretPaladin && !_neverMysteriousChallenger)
                            _whiteList.AddOrUpdate(MysteriousChallenger, false);
                        break;
                    }
                case Card.CClass.PRIEST:
                    {
                        _whiteList.AddOrUpdate(!_midrangeSecretPaladin && !_neverMysteriousChallenger ? MysteriousChallenger : "", false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);

                        break;
                    }
                case Card.CClass.ROGUE:
                    {
                        if (hasCoin)
                            lazyFlag = true;
                        _whiteList.AddOrUpdate(MusterForBattle, false);
                        if (!_midrangeSecretPaladin && !_neverMysteriousChallenger)
                            _whiteList.AddOrUpdate(MysteriousChallenger, false);
                        _whiteList.AddOrUpdate(PilotedShredder, false);

                        break;
                    }
                case Card.CClass.SHAMAN:
                    {
                        _whiteList.AddOrUpdate(MusterForBattle, false);
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
                        if (!_midrangeSecretPaladin && hasCoin && !_neverMysteriousChallenger)
                            _whiteList.AddOrUpdate(MysteriousChallenger, false);

                        _whiteList.AddOrUpdate(PilotedShredder, hasCoin);

                        break;
                    }
            }
            if ((choices.Any(c => c.ToString() == Coghammer) && (CoghammerLogic && has2Drop) && (opponentClass != Card.CClass.WARRIOR)))
                _whiteList.AddOrUpdate(Coghammer, false);
            else if ((opponentClass == Card.CClass.WARRIOR) || (opponentClass == Card.CClass.PRIEST) || (opponentClass == Card.CClass.ROGUE) || (opponentClass == Card.CClass.DRUID))
                _whiteList.AddOrUpdate(TruesilverChamption, false);

            if ((CompetitiveMustard && lazyFlag) && (choices.Any(c => c.ToString() == CompetitiveSpirit) && choices.Any(c => c.ToString() == MusterForBattle)))
            {
                _whiteList.AddOrUpdate(MusterForBattle, false);
                _whiteList.AddOrUpdate(CompetitiveSpirit, false);
            }

            if (hasCoin && !_midrangeSecretPaladin && _mysteriousChallengerCoin)
            {
                _whiteList.AddOrUpdate(Annoyatron, false);
                _whiteList.AddOrUpdate(MysteriousChallenger, false);
            }
            
            if ((opponentClass == Card.CClass.ROGUE) || (opponentClass == Card.CClass.DRUID)) //These classes can kill Defender if it's played on turn 1. 
                _nobleJuggler = false;


        #endregion

            foreach (var s in from s in choices
                                     let keptOneAlready = _cardsToKeep.Any(c => c.ToString() == s.ToString())
                                     where _whiteList.ContainsKey(s.ToString())
                                     where !keptOneAlready | _whiteList[s.ToString()]
                                     select s)
                _cardsToKeep.Add(s);
            return _cardsToKeep;
        }

        private void FixRedundancy()
        {

            if (_midrangeSecretPaladin)
            {
                _mysteriousChallengerCoin = false;
                _mysteriousChallengerForever = false;
                _neverMysteriousChallenger = false;
            }
            if (_neverMysteriousChallenger)
            {
                _mysteriousChallengerForever = false;
                _mysteriousChallengerCoin = false;
            }
            if (_mysteriousChallengerCoin)
            {
                _mysteriousChallengerForever = false;
                _neverMysteriousChallenger = false;
            }
            if (!_mysteriousChallengerForever) return;
            _neverMysteriousChallenger = false;
            _mysteriousChallengerCoin = false;
        }
    }
}