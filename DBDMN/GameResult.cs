using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DBDMN.ScreenParser;

namespace DBDMN
{
    /// <summary>
    /// A class to hold all info about game results and icons in 1 place
    /// </summary>
    public class GameResult
    {

        //private int myBpAmount = EndscoreBpDigitGfx.INVALID_BP_AMOUNT;

        /// <summary>
        /// Amount of Bloodpoints we earned this game. From the endgame scoreboard
        /// </summary>
        private int[] playerBpAmount = { EndscoreBpDigitGfx.INVALID_BP_AMOUNT, EndscoreBpDigitGfx.INVALID_BP_AMOUNT,
            EndscoreBpDigitGfx.INVALID_BP_AMOUNT, EndscoreBpDigitGfx.INVALID_BP_AMOUNT, EndscoreBpDigitGfx.INVALID_BP_AMOUNT };

        /// <summary>
        /// Did we play as killer or survivor?
        /// </summary>
        public enum GameRole { Killer, Survivor, Error };

        public enum Pip { Error, Depipped, SafetyPipped, Pipped, DoublePipped };

        /// <summary> My player index in this game </summary>
        private PlayerIndex myPlayerIndex = PlayerIndex.Error;

        /// <summary> Solo (survivor or killer), SWF or custom game? </summary>
        private Stats.GameType _gameType = Stats.GameType.Error;

        //private List<EndgameSurvivorIcon> survIconList = new List<EndgameSurvivorIcon>();

        private DateTime gameDate = DateTime.MinValue;

        /// <summary>
        /// 0-Based. Survivor or killer
        /// </summary>
        private List<EndgameSurvivorIcon> playerIconList = new List<EndgameSurvivorIcon>{ EndgameSurvivorIcon.Error,
            EndgameSurvivorIcon.Error, EndgameSurvivorIcon.Error, EndgameSurvivorIcon.Error,
            EndgameSurvivorIcon.DefaultKillerIcon   // Set a default icon for now, since we have no recognition
        };

        //public GameResult(): this( PlayerIndex.Error, EndgameSurvivorIcon.Error, EndgameSurvivorIcon.Error,
        //        EndgameSurvivorIcon.Error, EndgameSurvivorIcon.Error )
        //{ 
        //}

        public static GameResult initWithErrors()
        {
            return new GameResult( PlayerIndex.Error, EndgameSurvivorIcon.Error, EndgameSurvivorIcon.Error,
                 EndgameSurvivorIcon.Error, EndgameSurvivorIcon.Error, Stats.GameType.Error);
        }

        public GameResult( PlayerIndex selectedPlayerIndex, EndgameSurvivorIcon s1, EndgameSurvivorIcon s2,
            EndgameSurvivorIcon s3, EndgameSurvivorIcon s4, Stats.GameType gameType )
        {
            setSelectedPlayerIndex( selectedPlayerIndex );

            // Store icons
            setSurvivorIcon( 0, s1 );
            setSurvivorIcon( 1, s2 );
            setSurvivorIcon( 2, s3 );
            setSurvivorIcon( 3, s4 );

            // Store my bloodpoints
            //setMyBpAmount( myBloodpoints );

            // Store game type
            setGameType( gameType );
        }

        public bool isAnySurvivorStillPlaying() => getNumStillPlaying() > 0;

        /// <summary>
        /// Returns true if all 4 survivor endresult icons and selected player index
        /// were correctly recognized
        /// </summary>
        public bool isAllIconsCorrectlyRecognized() => myPlayerIndex != PlayerIndex.Error && getNumErrorRecognizing() == 0;

        /// <summary>
        /// Was I playing as survivor and not as killer?
        /// </summary>
        public bool isMyGameRoleIsSurvivor()
        {
            return myPlayerIndex == PlayerIndex.Survivor1 || myPlayerIndex == PlayerIndex.Survivor2 ||
                myPlayerIndex == PlayerIndex.Survivor3 || myPlayerIndex == PlayerIndex.Survivor4;
        }

        /// <summary>
        /// Was I playing as killer and not as survivor?
        /// </summary>
        public bool isMyGameRoleIsKiller()
        {
            return myPlayerIndex == PlayerIndex.Killer;
        }

        public GameRole getMyGameRole()
        {
            // Get player role in this game
            switch ( myPlayerIndex )
            {
                case PlayerIndex.Killer:
                    return GameRole.Killer;
                case PlayerIndex.Survivor1:
                case PlayerIndex.Survivor2:
                case PlayerIndex.Survivor3:
                case PlayerIndex.Survivor4:
                    return GameRole.Survivor;
                case PlayerIndex.Error:
                default:
                    Dbg.onDebugError( "Invalid selected player index" );
                    break;
            }

            return GameRole.Error;
        }

        /// <summary>
        /// How many survivors escaped in this game
        /// </summary>
        public int getNumEscaped() => playerIconList.Count( survIcon => survIcon == EndgameSurvivorIcon.Escaped );

        /// <summary>
        /// How many other survivors escaped (excluding me)
        /// </summary>
        public int getNumOtherSurvivorsEscaped()
        {
            int numOtherSurvivorsEscaped = 0;
            
            // For all 4 survivors
            for(int i = (int)PlayerIndex.Survivor1; i <= (int)PlayerIndex.Survivor4; i++ )
            {
                // Not me
                if ( this.getSelectedPlayerIndex() != ( PlayerIndex )i )
                {
                    if ( isSurvivorEscaped( i ) )
                        numOtherSurvivorsEscaped++;
                }
            }

            return numOtherSurvivorsEscaped;
        }

        public int getNumSacrificed() => playerIconList.Count( survIcon => survIcon == EndgameSurvivorIcon.KilledSacrificed );
        public int getNumMoried() => playerIconList.Count( survIcon => survIcon == EndgameSurvivorIcon.KilledBleedoutOrMoried );

        /// <summary>
        /// Sacrificed or moried or bleedout
        /// </summary>
        public int getNumKilledInGame() => getNumSacrificed() + getNumMoried();
        public int getNumDCed() => playerIconList.Count( survIcon => survIcon == EndgameSurvivorIcon.DCed );

        /// <summary>
        /// Number of survivors that are still in the game (ignoring killer)
        /// </summary>
        public int getNumStillPlaying() => playerIconList.Count( survIcon => survIcon == EndgameSurvivorIcon.SurvIconStillPlaying );
        public int getNumErrorRecognizing() => playerIconList.Count( survIcon => survIcon == EndgameSurvivorIcon.Error );

        /// <summary>
        /// Who did I play as in this game. Killer or one of 4 survivors
        /// </summary>
        public PlayerIndex getSelectedPlayerIndex() => myPlayerIndex;

        public void setSelectedPlayerIndex(PlayerIndex playerIndex)
        {
            this.myPlayerIndex = playerIndex;
        }

        /// <summary>
        /// Index is 0-3 (not 1-4).
        /// </summary>
        public EndgameSurvivorIcon getSurvivorResultIcon( int survivorIndex )
        {
            Dbg.assertPlayerIndexIsInRangeAndNotInvalid( ( PlayerIndex )survivorIndex );

            return this.playerIconList[ survivorIndex ];
        }

        public EndgameSurvivorIcon getKillerResultIcon()
        {
            // Error for now, because not implemented
            return EndgameSurvivorIcon.Error;
        }

        public void setSurvivorIcon( int survivorIndex, EndgameSurvivorIcon survIcon )
        {
            Dbg.assert( playerIconList.Count > survivorIndex );
            Dbg.assert( survivorIndex >= 0, "Survivor index too low" );
            Dbg.assert( survivorIndex <= 3, "Survivor index too high" );

            this.playerIconList[ survivorIndex ] = survIcon;
        }

        /// <summary>
        /// For debugging icons
        /// </summary>
        public string iconsToString()
        {
            return "Selected player: " + getSelectedPlayerIndex().ToString() +
                ", Surv1: " + getSurvivorResultIcon( 0 ).ToString() +
                " / Surv2: " + getSurvivorResultIcon( 1 ).ToString() +
                " / Surv3: " + getSurvivorResultIcon( 2 ).ToString() +
                " / Surv4: " + getSurvivorResultIcon( 3 ).ToString();
        }



        /// <summary>
        /// Get string for saving to disk
        /// </summary>
        public string getStringForSaving()
        {
            // Player index
            var playerIndexEnum = getSelectedPlayerIndex();
            int playerIndex = ( int )playerIndexEnum;

            Dbg.assertPlayerIndexIsInRangeAndNotInvalid( playerIndexEnum );

            string surv1Result = _resultIconToString( getSurvivorResultIcon( 0 ) );
            string surv2Result = _resultIconToString( getSurvivorResultIcon( 1 ) );
            string surv3Result = _resultIconToString( getSurvivorResultIcon( 2 ) );
            string surv4Result = _resultIconToString( getSurvivorResultIcon( 3 ) );

            string killerResult = "";

            string pipResult = "";

            string gameDuration = "";

            string myCharacterName = "";


            return StatSaver.lineGameStat + StatSaver.lineNameSeparator + StatSaver.gameLinePlayerIndexField +
                StatSaver.gameFieldValueSeparator + playerIndex + StatSaver.gameFieldsSeparator +
                StatSaver.gameLineSurvivor1ResultField + StatSaver.gameFieldValueSeparator +
                surv1Result + StatSaver.gameFieldsSeparator + StatSaver.gameLineSurvivor2ResultField +
                StatSaver.gameFieldValueSeparator + surv2Result + StatSaver.gameFieldsSeparator +
                StatSaver.gameLineSurvivor3ResultField + StatSaver.gameFieldValueSeparator + surv3Result +
                StatSaver.gameFieldsSeparator + StatSaver.gameLineSurvivor4ResultField +
                StatSaver.gameFieldValueSeparator + surv4Result + StatSaver.gameFieldsSeparator +

                // Killer result
                StatSaver.gameLineKillerField + StatSaver.gameFieldValueSeparator + killerResult + StatSaver.gameFieldsSeparator +

                // Pipped?
                "p" + StatSaver.gameFieldValueSeparator + pipResult + StatSaver.gameFieldsSeparator +

                // Game type
                getKeyValuePair( StatSaver.gameLineGameType, this.getGameType().ToString() ) +

                // Bloodpoints
                getKeyValuePair(StatSaver.gameLineSurvivor1BP, getBpAmount(PlayerIndex.Survivor1).ToString() ) +
                getKeyValuePair(StatSaver.gameLineSurvivor2BP, getBpAmount(PlayerIndex.Survivor2).ToString() ) +
                getKeyValuePair(StatSaver.gameLineSurvivor3BP, getBpAmount(PlayerIndex.Survivor3).ToString() ) +
                getKeyValuePair(StatSaver.gameLineSurvivor4BP, getBpAmount(PlayerIndex.Survivor4).ToString() ) +
                getKeyValuePair(StatSaver.gameLineKillerBP, getBpAmount(PlayerIndex.Killer ).ToString() ) +

                //StatSaver.gameLineBloodpoints + StatSaver.gameFieldValueSeparator + getMyBpAmount() + StatSaver.gameFieldsSeparator +
                
                // Match duration
                StatSaver.gameLineMatchDuration + StatSaver.gameFieldValueSeparator + gameDuration + StatSaver.gameFieldsSeparator +
                
                // Date
                StatSaver.gameLineDate + StatSaver.gameFieldValueSeparator + Utils.getCurrentDateAsString(this.getDate()) + StatSaver.gameFieldsSeparator +
                
                // Name of Killer/Survivor played this game
                "n" + StatSaver.gameFieldValueSeparator + myCharacterName;

            string getKeyValuePair(string keyOfValue, string value)
            {
                return keyOfValue + StatSaver.gameFieldValueSeparator + value + StatSaver.gameFieldsSeparator;
            }

            string _resultIconToString( EndgameSurvivorIcon icon)
            {
                string result = "error";
                if ( icon == EndgameSurvivorIcon.Escaped )
                    result = StatSaver.gameLineSurvivorResultEscaped;
                else if ( icon == EndgameSurvivorIcon.KilledSacrificed )
                    result = StatSaver.gameLineSurvivorResultSacrificed;
                else if ( icon == EndgameSurvivorIcon.KilledBleedoutOrMoried )
                    result = StatSaver.gameLineSurvivorResultMoried;
                else if ( icon == EndgameSurvivorIcon.DCed )
                    result = StatSaver.gameLineSurvivorResultDCed;
                else if ( icon == EndgameSurvivorIcon.SurvIconStillPlaying )
                    result = StatSaver.gameLineSurvivorResultStillPlaying;
                else
                {
                    // in a custom game this player spot might not be filled, if not
                    // all 4 survivors are playing
                    if ( this.getGameType() == Stats.GameType.CustomGame )
                        result = StatSaver.gameLineSurvivorResultNoPlayer;
                    else
                        Dbg.onDebugError( "Wrong result: " + icon.ToString() );
                }

                return result;
            }
        }

        /// <summary> Solo, SWF, custom game </summary>
        public Stats.GameType getGameType() => this._gameType;

        public void setGameType( Stats.GameType gameType )
        {
            this._gameType = gameType;
        }

        public void setDate( DateTime date )
        {
            this.gameDate = date;
        }

        public DateTime getDate() => this.gameDate;

        public override string ToString() => iconsToString() + ", BP S1: " + getBpAmount(PlayerIndex.Survivor1) +
            ", BP S2: " + getBpAmount( PlayerIndex.Survivor2 ) + ", BP S3: " + getBpAmount( PlayerIndex.Survivor3)
             + ", BP S4: " + getBpAmount( PlayerIndex.Survivor4 ) + ", BP K: " + getBpAmount( PlayerIndex.Killer );

        public bool isSurvivorEscaped(int survivorIndex)
        {
            Dbg.assertIsValidSurvivorIndex( survivorIndex );

            return getSurvivorResultIcon( survivorIndex ) == EndgameSurvivorIcon.Escaped;
        }

        /// <summary>
        /// Survivor or Killer DCed
        /// </summary>
        public bool isPlayerDCed( int survivorIndex )
        {
            Dbg.assertPlayerIndexIsInRangeAndNotInvalid( (PlayerIndex)survivorIndex );

            return getSurvivorResultIcon( survivorIndex ) == EndgameSurvivorIcon.DCed;
        }

        /// <summary>
        /// Was killed or escaped (not still playing and not DCed etc)
        /// </summary>
        public bool isSurvivorKilledOrEscaped( int survivorIndex )
        {
            Dbg.assertIsValidSurvivorIndex( survivorIndex );

            return isSurvivorEscaped( survivorIndex ) || isSurvivorKilled( survivorIndex );
        }

        /// <summary>
        /// Killed or moried or bleedout
        /// </summary>
        public bool isSurvivorKilled( int survivorIndex )
        {
            Dbg.assertIsValidSurvivorIndex( survivorIndex );

            return getSurvivorResultIcon( survivorIndex ) == EndgameSurvivorIcon.KilledSacrificed ||
                getSurvivorResultIcon( survivorIndex ) == EndgameSurvivorIcon.KilledBleedoutOrMoried;
        }

        public bool isSurvivorStillPlaying( int survivorIndex )
        {
            Dbg.assertIsValidSurvivorIndex( survivorIndex );

            return getSurvivorResultIcon( survivorIndex ) == EndgameSurvivorIcon.SurvIconStillPlaying;
        }

        /// <summary>
        /// Did I escape this game?
        /// </summary>
        public bool isMeEscapedThisGame()
        {
            if ( !isMyGameRoleIsSurvivor() )
                return false;

            var myPlayerIndex = getSelectedPlayerIndex();

            return isSurvivorEscaped( (int)myPlayerIndex );
        }

        public bool isMeKilledThisGame()
        {
            if ( !isMyGameRoleIsSurvivor() )
                return false;

            var myPlayerIndex = getSelectedPlayerIndex();

            return isSurvivorKilled( ( int )myPlayerIndex );
        }

        /// <summary>
        /// In this game someone DCed while the game was loading, so no game was played
        /// </summary>
        public bool isThisGameCanceledBecauseOfDCWhileLoading()
        {
            // If all survivors have "still playing" icon = someone DCed while loading
            return isSurvivorStillPlaying( 0 ) && isSurvivorStillPlaying( 1 ) &&
                isSurvivorStillPlaying( 2 ) && isSurvivorStillPlaying( 3 );
        }

        /// <summary>
        /// Set BP amount for survivor or killer
        /// </summary>
        public void setBpAmount(PlayerIndex playerIndex, int myBpAmount)
        {
            Dbg.assertPlayerIndexIsInRangeAndNotInvalid( playerIndex );

            this.playerBpAmount[ ( int )playerIndex ] = myBpAmount;
        }

        public int getBpAmount( PlayerIndex playerIndex )
        {
            Dbg.assertPlayerIndexIsInRangeAndNotInvalid( playerIndex );

            return this.playerBpAmount[ (int)playerIndex ];
        }

        /// <summary>
        /// Get my bloodpoints
        /// </summary>
        public int getMyBpAmount()
        {
            var me = getSelectedPlayerIndex();

            return this.playerBpAmount[ (int)me ];
        }

        public bool isBpAmountSet( PlayerIndex playerIndex )
        {
            Dbg.assertPlayerIndexIsInRangeAndNotInvalid( playerIndex );

            return this.playerBpAmount[ ( int )playerIndex ] != EndscoreBpDigitGfx.INVALID_BP_AMOUNT;
        }

        public bool isMyBpAmountSet()
        {
            return getMyBpAmount() != EndscoreBpDigitGfx.INVALID_BP_AMOUNT;
        }


        public bool isSoloGame() => getGameType() == Stats.GameType.Solo;
        public bool isAnyRankedKillerGame() => getGameType() == Stats.GameType.Solo;
        public bool isSoloOrSwfSurvivorGame() => isSoloGame() || isSwfGame();
        public bool isSwfGame() => getGameType() == Stats.GameType.SWF;
        public bool isCustomGame() => getGameType() == Stats.GameType.CustomGame;
    }
}
