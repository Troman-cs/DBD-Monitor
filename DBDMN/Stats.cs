using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DBDMN.ScreenParser;

namespace DBDMN
{
    public static class Stats
    {

        /// <summary>
        /// Solo, SWF or Custom game
        /// </summary>
        public enum GameType
        {
            Error,

            /// <summary>
            /// Ranked Solo Survivor (not SWF) or ANY ranked killer game. Not a custom game, not survivor SWF game.
            /// </summary>
            Solo,

            /// <summary>
            /// Survivor SWF game. This can't be any ranked killer game, because SWF=more than 1 player in pre-lobby
            /// </summary>
            SWF,
            CustomGame
        }

        /// <summary>
        /// results of the last game
        /// </summary>
        private static GameResult lastGameResult = null;

        private static List<GameResult> games = new List<GameResult>();

        public const int invalidIntValue = int.MinValue;
        public const int invalidDoubleValue = int.MinValue;

        /// <summary>
        /// Add results of the current game
        /// </summary>
        public static void addCurGameResult( GameResult curGameResult )
        {
            Dbg.assert( curGameResult != null );

            // Exit if we are not allowed to add new results
            if ( !Form1.getInstance().isAddNewGameResultsEnabled() )
                return;

            // Set date of this game to today
            curGameResult.setDate( DateTime.Now );

            lastGameResult = curGameResult;

            Log.log( "Cur game stats stored: " + curGameResult.ToString() );

            addGameToStats( curGameResult );

            Actions.onAddCurGameToResults();
        }

        /// <summary>
        /// "SWF" to GameType.SWF
        /// </summary>
        public static GameType getGameTypeEnumFromString(string sGameType)
        {
            sGameType = sGameType.Trim();

            var values = Enum.GetValues( typeof( GameType ) ).Cast<GameType>();

            foreach(var v in values)
            {
                if ( v.ToString().ToLower() == sGameType.ToLower() )
                    return v;
            }

            return GameType.Error;
        }

        /// <summary>
        /// If we leave DBD endscores screen and restart this program, it will recognize
        /// the shown game results as a new game again and again. Prevent it.
        /// </summary>
        public static bool isGameResultsSameAsLastGameResults(GameResult newGame)
        {
            var lastGame = getLastGameInStats();

            // No games stored yet?
            if ( lastGame == null )
                return false;

            // Check survivor results
            for(int index = (int)PlayerIndex.Survivor1; index <= (int)PlayerIndex.Survivor4; index++ )
            {
                // Check results for survivors
                if ( newGame.getSurvivorResultIcon( index ) != lastGame.getSurvivorResultIcon( index ) )
                    return false;
            }

            // Check killer result
            if ( newGame.getKillerResultIcon() != lastGame.getKillerResultIcon() )
                return false;

            // Check BP
            for ( int player = ( int )PlayerIndex.Survivor1; player <= ( int )PlayerIndex.Killer; player++ )
            {
                if ( newGame.getBpAmount( ( PlayerIndex )player ) != lastGame.getBpAmount( ( PlayerIndex )player ) )
                    return false;
            }

            // Check my role
            if ( newGame.getMyGameRole() != lastGame.getMyGameRole() )
                return false;

            return true;
        }

        private static GameResult getLastGameInStats()
        {
            if ( games.Count == 0 )
                return null;

            return games[ games.Count - 1 ];
        }

        /// <summary>
        /// Add game to stats
        /// </summary>
        public static void addGameToStats( GameResult game, bool bSaveToDisk = true )
        {
            Dbg.assert( game != null );

            games.Add( game );

            // Save immediately if needed
            if( bSaveToDisk )
                StatSaver.save();
        }

        public static void setGames(List<GameResult> newGames)
        {
            games = newGames;
        }

        public static List<GameResult> getGames() => games;

        public static int getNumGamesAsSurvivor()
        {
            int numGamesAsSurvivor = 0;
            foreach(var game in games)
            {
                if ( game.isMyGameRoleIsSurvivor() && !game.isThisGameCanceledBecauseOfDCWhileLoading() )
                    numGamesAsSurvivor++;
            }

            return numGamesAsSurvivor;
        }

        /// <summary>
        /// In how many games we were kileld when playing as survivor
        /// </summary>
        public static int getNumGamesKilledAsSurvivor()
        {
            int numKilled = 0;
            foreach ( var game in games )
            {
                if ( game.isMyGameRoleIsSurvivor() )
                {
                    if ( game.isMeKilledThisGame() && !game.isThisGameCanceledBecauseOfDCWhileLoading() )
                        numKilled++;
                }
            }

            return numKilled;
        }

        public static int getNumGamesEscapedAsSurvivor()
        {
            int numEscaped = 0;
            foreach ( var game in games )
            {
                if ( game.isMyGameRoleIsSurvivor() )
                {
                    if ( game.isMeEscapedThisGame() && !game.isThisGameCanceledBecauseOfDCWhileLoading() )
                        numEscaped++;
                }
            }

            return numEscaped;
        }





        public static int getNumGamesAsKiller()
        {
            int numGamesAsKiller = 0;
            foreach ( var game in games )
            {
                if ( game.isMyGameRoleIsKiller() )
                    numGamesAsKiller++;
            }

            return numGamesAsKiller;
        }

        /// <summary>
        /// get total number of survivors killed
        /// </summary>
        public static int getNumTotalSurvivorsKilledAsKiller()
        {
            int numTotalSurvivorsKilledAsKiller = 0;
            foreach ( var game in games )
            {
                if ( game.isMyGameRoleIsKiller() && !game.isThisGameCanceledBecauseOfDCWhileLoading() )
                    numTotalSurvivorsKilledAsKiller = numTotalSurvivorsKilledAsKiller + game.getNumKilledInGame();
            }

            return numTotalSurvivorsKilledAsKiller;
        }



        public static (int killrate, double killrateXk, int numGames) getStatsMyKillRateAsKillerAsPercentage()
        {
            // Only ranked games (=solo games for killer), no custom games
            var myRankedKillerGames = getGames().Where( g => g.isMyGameRoleIsKiller() && g.isAnyRankedKillerGame() &&
                !g.isThisGameCanceledBecauseOfDCWhileLoading() );

            // get num games
            int numOfMyGamesAsKiller = myRankedKillerGames.Count();
            if ( numOfMyGamesAsKiller == 0 )       // Avoid division by 0
                return (invalidIntValue, invalidDoubleValue, 0);

            // get num kills
            int numKills = 0;
            foreach(var g in myRankedKillerGames )
                numKills = numKills + g.getNumKilledInGame();

            // in %
            int killrateInPercentage = ( int )Math.Round( ( float )numKills * 100f / (( float )numOfMyGamesAsKiller * 4f), 0 );

            // 0k-4k
            double killrateXk = Math.Round( 4f / 100f * ( float )killrateInPercentage, 1 );

            return (killrateInPercentage, killrateXk, numOfMyGamesAsKiller);
        }

        public static (int escapeRate, int numGames) getStatsEscapeRateAsSwfSurvivorAsPercentage( )
        {
            // SWF games
            var swfSurvivorGames = getGames().Where( g => g.isMyGameRoleIsSurvivor() && g.isSwfGame() &&
                !g.isThisGameCanceledBecauseOfDCWhileLoading() );
            
            // Custom games
            IEnumerable<GameResult> customSurvivorGames = new List<GameResult>();
            //if( bIncludeCustomGames )
            //    customSurvivorGames = getGames().Where( g => g.isMyGameRoleIsSurvivor() && g.isSwfGame() );

            // Combine SWF + custom games
            var allGames = swfSurvivorGames.Union( customSurvivorGames );

            // get num of games
            int numAllGames = allGames.Count();
            if ( numAllGames == 0 )
                return (invalidIntValue, 0);

            // get num escapes
            int numMySwfEscapes = allGames.Count( g => g.isMeEscapedThisGame() );

            var escapeRate = ( int )Math.Round( ( float )numMySwfEscapes * ( float )100 / 
                ( float )numAllGames, 0 );

            return ( escapeRate, numAllGames );
        }

        public static (int escapeRate, int numGames) getStatsEscapeRateAsSoloSurvivorAsPercentage()
        {
            // Get my solo games, exclude games with DC during game load
            var soloSurvivorGames = getGames().Where( g => g.isMyGameRoleIsSurvivor() 
                && g.isSoloGame() && !g.isThisGameCanceledBecauseOfDCWhileLoading() );

            // get num games
            int numMySoloSurvivorGames = soloSurvivorGames.Count();
            if( numMySoloSurvivorGames == 0)
                return (invalidIntValue, 0);

            // get num escapes
            int numMySoloEscapes = soloSurvivorGames.Count( g => g.isMeEscapedThisGame() );

            // calc escape rate %
            var esapeRate = ( int )Math.Round( ( float )numMySoloEscapes * ( float )100 / 
                ( float )numMySoloSurvivorGames, 0 );

            return ( esapeRate, numMySoloSurvivorGames );
        }


        /// <summary>
        /// Escape rate of other survivors (not me) in my solo games
        /// </summary>
        public static int getStatsEscapeRateOfOtherSurvivorsInMySoloGames()
        {
            // Get all games where I played as a solo survivor
            var mySurvivorSoloGames = getGames().Where( g => g.isMyGameRoleIsSurvivor() && g.isSoloGame() &&
                !g.isThisGameCanceledBecauseOfDCWhileLoading() );

            // Get num games
            int numSoloSurvivorGames = mySurvivorSoloGames.Count();
            if ( numSoloSurvivorGames == 0 )
                return invalidIntValue;

            // get num other survivors escaped
            int numOtherSurvivorEscaped = 0;
            foreach ( var g in mySurvivorSoloGames )
                numOtherSurvivorEscaped = numOtherSurvivorEscaped + g.getNumOtherSurvivorsEscaped();

            return ( int )Math.Round( ( float )numOtherSurvivorEscaped * ( float )100 / 
                (( float )numSoloSurvivorGames * 3f), 0 );
        }

        /// <summary>
        /// Get num of other survivors (not me) escaped in my SWF games
        /// </summary>
        public static int getStatsEscapeRateOfOtherSurvivorsDuringMySwfGamesAsPercentage()
        {
            // Get all my SWF games
            var mySwfSurvivorGames = getGames().Where( g => g.isMyGameRoleIsSurvivor() && g.isSwfGame() &&
                !g.isThisGameCanceledBecauseOfDCWhileLoading() );

            // get num games
            int numSwfSurvivorGames = mySwfSurvivorGames.Count();
            if ( numSwfSurvivorGames == 0 )
                return invalidIntValue;

            // get num other survivors escaped
            int numOtherSurvivorEscaped = 0;
            foreach ( var g in mySwfSurvivorGames )
                numOtherSurvivorEscaped = numOtherSurvivorEscaped + g.getNumOtherSurvivorsEscaped();

            return ( int )Math.Round( ( float )numOtherSurvivorEscaped * ( float )100 / 
                ( ( float )numSwfSurvivorGames * 3f ), 0 );
        }

        public static int getStatsSurvivorAverageBP()
        {
            // Get my survivor games (solo and SWF), exclude games with DC during game load
            var survivorGames = getGames().Where( g => g.isMyGameRoleIsSurvivor() &&
                g.isSoloOrSwfSurvivorGame() && !g.isThisGameCanceledBecauseOfDCWhileLoading() );

            // get num games
            int numMySurvivorGames = survivorGames.Count();
            if ( numMySurvivorGames == 0 )
                return invalidIntValue;

            // get total BP, only in games for which we have BP stored
            int totalBP = survivorGames.Where( g => g.isMyBpAmountSet() ).Sum( g => g.getMyBpAmount() );

            // calc escape rate %
            int result = totalBP / numMySurvivorGames;

            return result;
        }

        public static int getStatsKillerAverageBP()
        {
            // Get all my non-custom killer games, exclude games with DC during game load
            var rankedKillerGames = getGames().Where( g => g.isMyGameRoleIsKiller() && g.isAnyRankedKillerGame() &&
                !g.isThisGameCanceledBecauseOfDCWhileLoading() );

            // get num games
            int numMyRankedKillerGames = rankedKillerGames.Count();
            if ( numMyRankedKillerGames == 0 )
                return invalidIntValue;

            // get total BP
            int totalBP = rankedKillerGames.Where( g => g.isMyBpAmountSet() ).Sum( g => g.getMyBpAmount() );

            // calc escape rate %
            int result = totalBP / numMyRankedKillerGames;

            return result;
        }

        /// <summary>
        /// Get killrate of killers when I play survivor
        /// </summary>
        public static (int killrate, double killrateXk) getStatsOtherKillRateAsKillerAsPercentage()
        {
            // get my solo and SWF games, no custom games
            var mySoloOrSwfSurvivorGames = getGames().Where( g => g.isMyGameRoleIsSurvivor() && 
                g.isSoloOrSwfSurvivorGame() && !g.isThisGameCanceledBecauseOfDCWhileLoading() );

            // get num games
            int numGames = mySoloOrSwfSurvivorGames.Count();
            if ( numGames == 0 )       // Avoid division by 0
                return (invalidIntValue, invalidDoubleValue);

            // get num kills
            int numKills = 0;
            foreach ( var g in mySoloOrSwfSurvivorGames )
                numKills = numKills + g.getNumKilledInGame();

            // in %
            int killrateInPercentage = ( int )Math.Round( ( float )numKills * 100f / ( ( float )numGames * 4f ), 0 );

            // 0k-4k
            double killrateXk = Math.Round( 4f / 100f * ( float )killrateInPercentage, 1 );

            return (killrateInPercentage, killrateXk);
        }

        public static int getStatsOtherSurvivorAverageBP()
        {
            // Get my survivor games (solo and SWF), exclude games with DC during game load
            var myRankedSurvivorGames = getGames().Where( g => g.isMyGameRoleIsSurvivor() &&
                g.isSoloOrSwfSurvivorGame() && !g.isThisGameCanceledBecauseOfDCWhileLoading() );

            int numPlayersWithValidBP = 0;
            int totalBP = 0;
            foreach(var g in myRankedSurvivorGames )
            {
                Dbg.assert( g.isMyGameRoleIsSurvivor() );

                for(int player = (int)PlayerIndex.Survivor1; player <= (int)PlayerIndex.Survivor4; player++ )
                {
                    // make sure it's not me. but some other survivor
                    if ( ( PlayerIndex )player != g.getSelectedPlayerIndex() )
                    {
                        // make sure this survivor didn't DC, otherwise his BP will be 0
                        // Also make sure he is not still playing, otherwise his BP is not final
                        if ( g.isSurvivorKilledOrEscaped( player ) )
                        {
                            // Only if have BP stored
                            if ( g.isBpAmountSet( ( PlayerIndex )player ) )
                            {
                                totalBP = totalBP + g.getBpAmount( ( PlayerIndex )player );
                                numPlayersWithValidBP++;
                            }
                        }
                    }
                }
            }

            // Avoid division by 0
            if ( numPlayersWithValidBP == 0 )
                return invalidIntValue;

            // calc escape rate %
            int result = totalBP / numPlayersWithValidBP;

            return result;
        }

        /// <summary>
        /// In the games where I play as survivor, get killer's averga BP
        /// </summary>
        public static int getStatsOtherKillersAverageBP()
        {
            // Get my survivor games (solo and SWF), exclude games with DC during game load
            var myRankedSurvivorGames = getGames().Where( g => g.isMyGameRoleIsSurvivor() &&
                g.isSoloOrSwfSurvivorGame() && !g.isThisGameCanceledBecauseOfDCWhileLoading() );

            int numPlayersWithValidBP = 0;
            int totalBP = 0;
            foreach ( var g in myRankedSurvivorGames )
            {
                Dbg.assert( g.isMyGameRoleIsSurvivor() );

                // sure this survivor didn't DC, otherwise his BP will be 0
                // Also make sure he is not still playing, otherwise his BP is not final
                if ( !g.isPlayerDCed( (int)PlayerIndex.Killer ) )
                {
                    // Only if have BP stored
                    if ( g.isBpAmountSet( PlayerIndex.Killer ) )
                    {
                        totalBP = totalBP + g.getBpAmount( PlayerIndex.Killer );
                        numPlayersWithValidBP++;
                    }
                }
            }

            // Avoid division by 0
            if ( numPlayersWithValidBP == 0 )
                return invalidIntValue;

            // calc escape rate %
            int result = totalBP / numPlayersWithValidBP;

            return result;
        }
    }
}
