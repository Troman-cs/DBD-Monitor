using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace DBDMN
{
    /// <summary>
    /// Actions as response to game events and state changes
    /// </summary>
    public static class Actions
    {
        /// <summary>
        /// Are actions enabled in general?
        /// </summary>
        public static bool Enabled = true;

        private static Dictionary<StateManager.State, Action> actions = new Dictionary<StateManager.State, Action>();

        public static Action addAction( StateManager.State actionForState, string actionDescription )
        {
            Action action = new Action( actionDescription );
            action.forState = actionForState;

            Dbg.assert( !actions.ContainsKey( actionForState ), "Action already registered: " + actionForState.ToString() );

            // Save action
            actions[ actionForState ] = action;

            return action;
        }

        public static Dictionary<StateManager.State, Action> getActions()
        {
            return actions;
        }

        public static void save()
        {

        }

        public static void onIdlingInPreLobby()
        {
            Log.log("onIdlingInPreLobby");
            Form1.stopStopwatch();
        }

        public static void onSwfLookingForLobby()
        {
            Log.log( "onSwfLookingForLobby" );
        }

        public static void onEnteredLobby()
        {
            Log.log("onEnteredLobby");

            Dbg.saveErrorImageToFile();

            if ( Actions.actions.ContainsKey( StateManager.State.Lobby_SurvivorOrKiller ) )
                Actions.actions[ StateManager.State.Lobby_SurvivorOrKiller ].activate();
        }

        /// <summary>
        /// Gers activated on state change
        /// </summary>
        public static void onAllPlayersEnteredGameLobby()
        {
            Log.log("onAllPlayersEnteredGameLobby()");

            if ( Actions.actions.ContainsKey( StateManager.State.Lobby_AllPlayersInLobby ) )
                Actions.actions[ StateManager.State.Lobby_AllPlayersInLobby ].activate();

            // All players idling in game lobby?
            //if (isGameLobbyShown())
            //{
            //if (!ScreenCapture.isDBDWindowFocused())    // Don't activate if active already
            //    ScreenCapture.activateGame();
            //}
        }

        /// <summary>
        /// All players clicked Ready for the first time
        /// </summary>
        public static void onAllPlayersClickedReady()
        {
            Log.log("onAllPlayersClickedReady()");

            //if (!ScreenCapture.isDBDWindowFocused())    // Don't activate if active already
            //    ScreenCapture.activateGame();

            // Activate action
            if ( Actions.actions.ContainsKey( StateManager.State.Lobby_AllPlayersReady ) )
                Actions.actions[ StateManager.State.Lobby_AllPlayersReady ].activate();

            Form1.stopStopwatch();
        }

        public static void onAlmostFinishedLoadingMatch()
        {
            Log.log( "onAlmostFinishedLoadingMatch" );

            if ( Actions.actions.ContainsKey( StateManager.State.LoadingMatch_AlmostDone ) )
                Actions.actions[ StateManager.State.LoadingMatch_AlmostDone ].activate();
        }

        public static void onUnknownErrorOccuredMessage()
        {
            Log.log("Unknown error occured message");
        }

        public static void onLeaveLobbyConfirmationMessage()
        {
            Log.log("onLeaveLobbyConfirmationMessage");
        }

        public static void onStartedLookingForLobby()
        {
            Log.log("onStartedLookingForLobby");
            Form1.startStopwatch();
        }

        public static void onEndgameScreen()
        {
            Log.log( "onEndgame (any entry state)" );
        }

        public static void onLeavingEndgameScreen()
        {
            Log.log( "onLeavingEndgameScreen" );
        }

        public static void onEndgame_Scoreboard()
        {
            Log.log( "onEndgameScoreboard" );
        }

        public static void onEndgame_ObservingSomeone()
        {
            Log.log( "onEndgameObservingSomeone" );
        }

        ///// <summary>
        ///// Are we in the game lobby and not on some other game screen?
        ///// Also not in the SWF lobby.
        ///// </summary>
        //private static bool isGameLobbyShown()
        //{
        //    bool hasShop = ScreenParser.hasShopIcon();
        //    bool hasReady = ScreenParser.hasReadyButton();
        //    bool hasUnready = ScreenParser.hasUnreadyButton();

        //    // Make sure we are really in game lobby
        //    return !hasShop && !ScreenParser.hasSurvivorLookingForMatchText() &&
        //        (hasReady || hasUnready);
        //}



        public static void playSound( Sound.SoundsEnum sound, bool bLooped = false)
        {
            // All sound muted? - don't play
            if ( Form1.getInstance().isSoundMuted() )
                return;

            Sound.playSound( sound, bLooped );
        }

        public static void stopSound()
        {
            Sound.stopSound();
        }

        public static void onAddCurGameToResults()
        {
            // Sound disabled?
            if ( !Config.getConfigValueAsBool( Config.keyGeneralPlaySoundOnSavingCurGameStats ) )
                return;

            Actions.playSound( Sound.SoundsEnum.Shutter );

            // Recalc stats
            Form1.getInstance().recalcStats();
        }

        //public static void playCustomSound( string path )
        //{
        //    try
        //    {
        //        Player.SoundLocation = path;
        //        Player.Play();
        //    }
        //    catch ( Exception )
        //    {
        //    }
        //}



    }
}
