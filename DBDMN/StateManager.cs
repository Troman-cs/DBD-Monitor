using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DBDMN.ScreenParser;
using static DBDMN.Stats;

namespace DBDMN
{
    public class StateManager
    {
        /// <summary>
        /// Major game states
        /// </summary>
        public enum State
        {
            [Description( "Unknown" )]
            Unknown,

            [Description( "None" )]
            None,

            [Description( "PreLobbyLookingForMatch" )]
            PreLobby_LookingForLobby,  // survivor or killer

            [Description( "PreLobbyIdle" )]
            PreLobby_Idling_SurvivorOrKiller,   // Solo survivor or killer

            [Description( "InLobby" )]
            Lobby_SurvivorOrKiller,     // In the actual lobby

            [Description( "AllPlayersInLobby" )]
            Lobby_AllPlayersInLobby,      // In the lobby - all 5 players are in the lobby

            [Description( "AllPlayersReady" )]
            Lobby_AllPlayersReady,      // In the lobby - all players clicked ready

            [Description( "LoadingAlmostDone" )]
            LoadingMatch_AlmostDone,    // Almost finished loading the match (the actual game)

            [Description( "EndgameScoreboard" )]
            Endgame_ScoreBoard,

            [Description( "EndgameObserving" )]
            Endgame_ObservingSomeone
        }


        /// <summary>
        /// Solo is default. If we didn't see pre-lobby, we can't exactly know what type of game it is,
        /// so leave default at solo.
        /// </summary>
        private static GameType _gameType = GameType.Solo;

        /// <summary>
        /// Do we currently have any pverlay message displayed in the game?
        /// It can be an error message or a lobby-leaving
        /// confirmation message
        /// </summary>
        private static bool bHaveErrorMessageDisplayed = false;
        private static bool bHaveLeaveLobbyConfirmationMessageDisplayed = false;

        // In-Lobby stuff
        private static bool bAllPlayersAreInTheLobby = false;
        private static bool bAllPlayersClickedReady = false;

        // Endgame-Soreboard stuff: my game role, survivor escaped/killed icons
        private static GameResult scoreboardGameResult =  GameResult.initWithErrors();

        // Did we save the results of the current game in the stats already?
        // Prevents from saving the same stats twice.
        private static bool bSubmittedScoreboardResultsToStats = false;


        private static State state = State.Unknown;
        private static State stateSetDuringCurrentUpdate = State.None;

        /// <summary>
        /// Given "Lobby_AllPlayersInLobby" return State.Lobby_AllPlayersInLobby
        /// </summary>
        public static State? getStateEnumFromStateEnumString( string sStateName )
        {
            sStateName = sStateName.Trim();

            var stateEnumValues = Enum.GetValues( typeof( State ) ).Cast<State>();
            foreach (var state in stateEnumValues )
            {
                if ( state.ToString() == sStateName )
                    return state;
            }

            return null;
        }

        /// <summary>
        /// [Description( "Lobby_AllPlayersInLobby" )] => State.Lobby_AllPlayersInLobby
        /// </summary>
        public static State? getStateEnumFromStateEnumDescription( string sStateDescription )
        {
            sStateDescription = sStateDescription.Trim();

            var stateEnumValues = Enum.GetValues( typeof( State ) ).Cast<State>();
            foreach ( var state in stateEnumValues )
            {
                if ( state.getEnumValueName().ToLower() == sStateDescription.ToLower() )
                    return state;
            }

            return null;
        }

        public static State setState(State newState)
        {
            // Did we already recognize some state during the current update?
            // Avoid recognizing more than one state from the same image
            if ( stateSetDuringCurrentUpdate != State.None )
            {
                Debug.Assert( false, "More than one state recognized: " +
                    stateSetDuringCurrentUpdate.ToString() + " vs " + newState.ToString() );
            }

            State oldState = state;

            state = newState;

            // Remember that we already set a new state during the current update
            // This way we can see if we recognize more than 1 state from the same
            // image
            stateSetDuringCurrentUpdate = newState;

            // Do logic on leaving a state, if required
            if ( oldState != newState )
                onLeavingState( oldState, newState );

            return oldState;
        }

        /// <summary>
        /// Do whatever is required when leaving a certain state
        /// </summary>
        private static void onLeavingState(State oldState, State newState)
        {
            // If completely leaving the game after showing the Endgame screen,
            // store the game results, even if we didn't get them all and someone
            // is still playing
            if ( isAnyEndgameState( oldState ) && !isAnyEndgameState( newState ) )
            {
                Actions.onLeavingEndgameScreen();

                // Save game results to the stats if still needed
                saveScoreboardResultsToStats();

                // Reset Swf/Custom game to solo
                resetGameType();
            }
        }

        /// <summary>
        /// Save scoreboard results of the current game to the stats
        /// </summary>
        private static void saveScoreboardResultsToStats()
        {
            // Leave if already saved this game, don't save the same game twice
            if ( bSubmittedScoreboardResultsToStats )
                return;

            bSubmittedScoreboardResultsToStats = true;

            // We could have restarted this program while DBD's scoreboard stays up.
            // We can keep adding the same game results over and over, make sure
            // we are doing doing so by checking if last game is the same
            if ( !Stats.isGameResultsSameAsLastGameResults( scoreboardGameResult ) )
                Stats.addCurGameResult( scoreboardGameResult );
            else
                Log.log( "Last game result were the same. Not storing this game." );
        }

        public static void beforeAnyStateUpdates()
        {
            resetStateThatWasRecognizedDuringCurrentUpdate();
        }

        private static void resetStateThatWasRecognizedDuringCurrentUpdate() => stateSetDuringCurrentUpdate = State.None;
           
        public static State getState() => state;

        public static bool setHaveErrorMessageDisplayed(bool bNewMessageState )
        {
            bool bOldMessageState = haveErrorMessageDisplayed();

            bHaveErrorMessageDisplayed = bNewMessageState;

            // Message state change
            if(!bOldMessageState && bNewMessageState )
                Actions.onUnknownErrorOccuredMessage();

            return bOldMessageState;
        }

        public static bool haveErrorMessageDisplayed()
        {
            return bHaveErrorMessageDisplayed;
        }

        public static bool setHaveLeaveLobbyConfirmationMessageDisplayed( bool bNewMessageState )
        {
            bool bOldMessageState = bHaveLeaveLobbyConfirmationMessageDisplayed;

            bHaveLeaveLobbyConfirmationMessageDisplayed = bNewMessageState;

            // Message state change
            if ( !bOldMessageState && bNewMessageState )
                Actions.onLeaveLobbyConfirmationMessage();

            return bOldMessageState;
        }

        public static bool haveLeaveLobbyConfirmationMessageDisplayed()
        {
            return bHaveLeaveLobbyConfirmationMessageDisplayed;
        }

        public static bool haveAnyMessageDisplayed()
        {
            return haveErrorMessageDisplayed() || haveLeaveLobbyConfirmationMessageDisplayed();
        }

        public static void setSurvivorOrKillerIdlingInPreLobbyState( GameType gameType )
        {
            var newState = State.PreLobby_Idling_SurvivorOrKiller;

            var oldState = setState( newState );

            if ( oldState != newState )
                Actions.onIdlingInPreLobby();

            setGameType( gameType );
        }

        public static void setLookingForLobbyState(GameType gameType)
        {
            var newState = State.PreLobby_LookingForLobby;

            var oldState = setState( newState );

            if( oldState != newState )
                Actions.onStartedLookingForLobby();

            setGameType( gameType );
        }

        private static void setGameType( GameType gameType )
        {
            StateManager._gameType = gameType;
        }

        private static void resetGameType()
        {
            setGameType( GameType.Solo );
        }

        public static GameType getGameType() => StateManager._gameType;

        public static void setSurvivorOrKillerEnteredLobbyState()
        {
            var newState = State.Lobby_SurvivorOrKiller;

            var oldState = setState( newState );

            if ( oldState != newState )
            {
                // Re-init some variables related to this state,
                // otherwise their old states can carry over from a previous game
                bAllPlayersAreInTheLobby = false;
                bAllPlayersClickedReady = false;

                Actions.onEnteredLobby();
            }

            // Only check player ready status if we are in the lobby!
            // Otherwise we might recognize ready states on an image
            // where ready-states are not even shown
            updateSurvivorOrKillerEnteredLobbyState();
        }

        private static void updateSurvivorOrKillerEnteredLobbyState()
        {
            // Only makes sense in this state
            Debug.Assert( getState() == State.Lobby_SurvivorOrKiller );

            // All players have just entered lobby?
            bool bNewAllPlayersInLobby = ScreenParser.hasAllPlayersEnteredLobby();
            if ( !bAllPlayersAreInTheLobby && bNewAllPlayersInLobby )
                Actions.onAllPlayersEnteredGameLobby();
            bAllPlayersAreInTheLobby = bNewAllPlayersInLobby;

            // All players clicked "Ready" and have red checkmarks?
            bool bNewAllPlayersReady = ScreenParser.hasAllPlayersClickedReady();
            if ( !bAllPlayersClickedReady && bNewAllPlayersReady )
                Actions.onAllPlayersClickedReady();
            bAllPlayersClickedReady = bNewAllPlayersReady;

        }

        public static void setAlmostFinishedLoadingMatchState()
        {
            var newState = State.LoadingMatch_AlmostDone;

            var oldState = setState( newState );

            if ( oldState != newState )
            {
                Actions.onAlmostFinishedLoadingMatch();
            }
        }

        /// <summary>
        /// We finished the game: we are at Scoreboard, Observing etc
        /// </summary>
        private static bool isAnyEndgameState(State state)
        {
            switch ( state )
            {
                case State.Endgame_ScoreBoard:
                case State.Endgame_ObservingSomeone:
                    return true;
            }

            return false;
        }

        private static void initEndgamePlayerResultInfo()
        {
            scoreboardGameResult = GameResult.initWithErrors();

            // We usually know game type at this point already and it won't change later
            scoreboardGameResult.setGameType( getGameType() );

            // Stats not saved yet
            bSubmittedScoreboardResultsToStats = false;
        }

        public static void setEndgameObservingSomeoneState()
        {
            var newState = State.Endgame_ObservingSomeone;

            var oldState = setState( newState );

            // We have just entered Endgame screen, any of its pages or observing
            if ( !isAnyEndgameState( oldState ) && isAnyEndgameState( newState ) )
            {
                // Re-init some variables related to this state,
                // otherwise their old states can carry over from a previous game
                initEndgamePlayerResultInfo();

                Actions.onEndgameScreen();
            }

            // Entered Endgame Observing for the first time
            if ( oldState != newState )
            {
                Actions.onEndgame_ObservingSomeone();
            }
        }

        public static void setEndgameScoreboardState()
        {
            var newState = State.Endgame_ScoreBoard;

            var oldState = setState( newState );

            // We have just entered Endgame screen, any of its pages or observing
            if ( !isAnyEndgameState( oldState ) && isAnyEndgameState( newState ) )
            {
                // We just entered the endgame screen. Re-init some variables related to this state,
                // otherwise their old states can carry over from a previous game
                initEndgamePlayerResultInfo();

                Actions.onEndgameScreen();
            }

            // Entered Scoreboard for the first time
            if ( oldState != newState )
                Actions.onEndgame_Scoreboard();

            // Update players game result info, if still need to
            if ( mustStillUpdateScoreboardDetails() )
            {
                updateEndgameScoreboardState();

                // We got all info we can get? Save to stats then
                if ( !mustStillUpdateScoreboardDetails() )
                    saveScoreboardResultsToStats();
            }
        }

        public static void updateEndgameScoreboardState(bool bSuppressStateDebugCheck = false)
        {
            // Only makes sense in this state
            if( !bSuppressStateDebugCheck )
                Debug.Assert( getState() == State.Endgame_ScoreBoard );

            // Player we are playing as (killer or survivor index)
            var selectedPlayer = ScreenParser.recognizeScoreboardSelectedPlayer( bSuppressStateDebugCheck );

            // Failed to recognize selected player this time? Leave the old value, maybe it succeeded previously
            if ( selectedPlayer != PlayerIndex.Error )
                scoreboardGameResult.setSelectedPlayerIndex( selectedPlayer );

            // Make sure game type is save correctly, it shouldn't change here
            Dbg.assert( scoreboardGameResult.getGameType() == getGameType() );


            // Recognize game result icons for all 4 survivors
            EndgameSurvivorIcon[] survivorGameResults = new EndgameSurvivorIcon[ 4 ];

            for (int surv = 0; surv <= 3; surv++ )
            {
                survivorGameResults[ surv ] = ScreenParser.recognizeEndgameScoreboardSurvIcon( (PlayerIndex)surv );

                // When we mouse-over perk description, result icons gets covered and can't be recornized
                // So, if we have an error while recognizing it, leave the last value
                if ( survivorGameResults[ surv ] != EndgameSurvivorIcon.Error )
                    scoreboardGameResult.setSurvivorIcon( surv, survivorGameResults[ surv ] );
            }

            // Get Bloodpoints amounts
            for ( int player = ( int )PlayerIndex.Survivor1; player <= ( int )PlayerIndex.Killer; player++ )
            {
                int bpAmount = EndscoreBpDigitGfx.recognizePlayerBPNumber( ( PlayerIndex )player );
                if ( bpAmount != EndscoreBpDigitGfx.INVALID_BP_AMOUNT )
                    scoreboardGameResult.setBpAmount( ( PlayerIndex )player, bpAmount );
            }

            // Debug
            Log.log( scoreboardGameResult.iconsToString() );
        }

        /// <summary>
        /// We either failed to get all the necesssary details and must re-try. Or I'm playing
        /// survivor and some other survivors are still playing, keep updating their results 
        /// until they stop playing and we can get their results (when playing killer and looking
        /// at the scoreboard all survivors have already stopped playing).
        /// </summary>
        private static bool mustStillUpdateScoreboardDetails()
        {
            // if playing as killer and have "still playing" ssurvivor icon,
            // it could be due to a DC
            return !isAllScoreboardPlayerStatesCorrectlyRecognized() ||
                ( isMePlayingAsSurvivor() && isScoreboardAnySurvivorStillPlaying() );
        }

        /// <summary>
        /// I'm playing as survivor and not as killer?
        /// </summary>
        private static bool isMePlayingAsSurvivor()
        {
            return scoreboardGameResult.isMyGameRoleIsSurvivor();
        }

        private static bool isScoreboardAnySurvivorStillPlaying()
        {
            return scoreboardGameResult.isAnySurvivorStillPlaying();
        }

        private static bool isAllScoreboardPlayerStatesCorrectlyRecognized()
        {
            return scoreboardGameResult.isAllIconsCorrectlyRecognized() &&
                scoreboardGameResult.isBpAmountSet(PlayerIndex.Survivor1) &&
                scoreboardGameResult.isBpAmountSet( PlayerIndex.Survivor2 ) &&
                scoreboardGameResult.isBpAmountSet( PlayerIndex.Survivor3 ) &&
                scoreboardGameResult.isBpAmountSet( PlayerIndex.Survivor4 ) &&
                scoreboardGameResult.isBpAmountSet( PlayerIndex.Killer );
        }

        ///// <summary>
        ///// Update details of the state after setting it.
        ///// For lobby - see who is ready
        ///// For Endgame Scoreboard get who was killed and who escaped etc
        ///// </summary>
        //public static void updateStateDetails()
        //{
        //    switch ( getState() )
        //    {
        //        case State.Lobby_SurvivorOrKiller:

        //            break;
        //        case State.Endgame_ScoreBoard:

        //            break;
        //        default:
        //            break;
        //    }
        //}

    }
}
