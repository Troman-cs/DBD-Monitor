using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DBDMN.ScreenParser;
using static DBDMN.Stats;

namespace DBDMN
{
    public static class StatSaver
    {
        private static string filePath = Utils.getAppPath() + "\\stats.st";

        public static string lineNameSeparator = "=";
        public static string gameFieldsSeparator = "/";
        public static string gameFieldValueSeparator = ":";

        /// <summary>
        /// Game stat line begins with this
        /// </summary>
        public static string lineGameStat = "g";

        public static string gameLinePlayerIndexField = "i";
        public static string gameLineSurvivor1ResultField = "s1";
        public static string gameLineSurvivor2ResultField = "s2";
        public static string gameLineSurvivor3ResultField = "s3";
        public static string gameLineSurvivor4ResultField = "s4";
        public static string gameLineKillerField = "k";

        public static string gameLineSurvivorResultEscaped = "e";
        public static string gameLineSurvivorResultSacrificed = "s";
        public static string gameLineSurvivorResultMoried = "m";
        public static string gameLineSurvivorResultStillPlaying = "p";
        public static string gameLineSurvivorResultDCed = "d";
        public static string gameLineSurvivorResultNoPlayer = "np"; // in a custom game - this player slot was empty

        /// <summary> Date when this game was played </summary>
        public static string gameLineDate = "dt";
        public static string gameLineMatchDuration = "dr";

        // Bloodpoints
        public static string gameLineSurvivor1BP = "s1bp";
        public static string gameLineSurvivor2BP = "s2bp";
        public static string gameLineSurvivor3BP = "s3bp";
        public static string gameLineSurvivor4BP = "s4bp";
        public static string gameLineKillerBP = "kbp";

        //public static string gameLineBloodpoints = "bp";
        public static string gameLineGameType = "gType";

        private static bool bStatsLoaded = false;

        public static void load()
        {
            // Exit if no stats file
            if (!File.Exists(filePath))
            {
                bStatsLoaded = true;
                return;
            }

            Dbg.assert( Stats.getGames().Count == 0, "Trying to load stats while they are already loaded, " +
                "might lose loaded stats");

            if ( Stats.getGames().Count > 0 )
                return;

            List<GameResult> games = new List<GameResult>();

            GameResult game = null;

            using ( StreamReader file = new StreamReader( filePath ) )
            {
                while ( !file.EndOfStream )
                {
                    var line = file.ReadLine();

                    game = null;

                    try
                    {
                        game = parseGameLine( line );
                    }
                    catch ( Exception e )
                    {
                        Dbg.onDebugError( "Error loading stats line: " + e.ToString() );
                    }

                    if ( game != null )
                        games.Add( game );
                }
            }

            Stats.setGames( games );

            bStatsLoaded = true;
        }

        private static GameResult parseGameLine(string line)
        {
            if ( line.Trim() == "" )
                return null;

            // "g=..."
            var v = line.Split( new string[] { lineNameSeparator }, 2, StringSplitOptions.RemoveEmptyEntries );

            if ( v.Length != 2 )
                return null;

            var lineType = v[ 0 ].Trim();

            // Not a game stat line?
            if ( lineType != lineGameStat )
                return null;

            var lineFields = v[ 1 ].Trim();
            var gameLineFields = lineFields.Split( new string[] { gameFieldsSeparator }, StringSplitOptions.RemoveEmptyEntries );


            PlayerIndex playerIndex = PlayerIndex.Error;
            EndgameSurvivorIcon surv1Result = EndgameSurvivorIcon.Error;
            EndgameSurvivorIcon surv2Result = EndgameSurvivorIcon.Error;
            EndgameSurvivorIcon surv3Result = EndgameSurvivorIcon.Error;
            EndgameSurvivorIcon surv4Result = EndgameSurvivorIcon.Error;

            DateTime gameDate = DateTime.MinValue;
            GameType gameType = GameType.Error;

            int[] bloodpoints = new int[ 5 ]{
                EndscoreBpDigitGfx.INVALID_BP_AMOUNT,
                EndscoreBpDigitGfx.INVALID_BP_AMOUNT,
                EndscoreBpDigitGfx.INVALID_BP_AMOUNT,
                EndscoreBpDigitGfx.INVALID_BP_AMOUNT,
                EndscoreBpDigitGfx.INVALID_BP_AMOUNT };

            GameResult game = null;

            foreach (var gameField in gameLineFields)
            {
                var fieldNameAndValue = gameField.Split( new string[] { gameFieldValueSeparator }, 2, StringSplitOptions.None );

                Dbg.assert( fieldNameAndValue.Length == 2, "Wrong game value format" );

                var fieldName = fieldNameAndValue[0].Trim();
                var fieldValue = fieldNameAndValue[1].Trim();

                // Player index
                if ( fieldName.Trim() == gameLinePlayerIndexField )
                    playerIndex = parsePlayerIndexField( fieldValue );
                else if ( fieldName.Trim() == gameLineSurvivor1ResultField )
                    surv1Result = parsePlayerResultField( fieldValue );
                else if ( fieldName.Trim() == gameLineSurvivor2ResultField )
                    surv2Result = parsePlayerResultField( fieldValue );
                else if ( fieldName.Trim() == gameLineSurvivor3ResultField )
                    surv3Result = parsePlayerResultField( fieldValue );
                else if ( fieldName.Trim() == gameLineSurvivor4ResultField )
                    surv4Result = parsePlayerResultField( fieldValue );
                else if ( fieldName.Trim() == gameLineDate )
                    gameDate = Utils.parseDateFromDdMmYyyyString( fieldValue );

                // Bloodpoints
                else if ( fieldName.Trim() == gameLineSurvivor1BP )
                    rememberBP( PlayerIndex.Survivor1, fieldValue );
                else if ( fieldName.Trim() == gameLineSurvivor2BP )
                    rememberBP( PlayerIndex.Survivor2, fieldValue );
                else if ( fieldName.Trim() == gameLineSurvivor3BP )
                    rememberBP( PlayerIndex.Survivor3, fieldValue );
                else if ( fieldName.Trim() == gameLineSurvivor4BP )
                    rememberBP( PlayerIndex.Survivor4, fieldValue );
                else if ( fieldName.Trim() == gameLineKillerBP )
                    rememberBP( PlayerIndex.Killer, fieldValue );

                else if( fieldName.ToLower().Trim() == gameLineGameType.ToLower() )
                {
                    gameType = Stats.getGameTypeEnumFromString( fieldValue );

                    Dbg.assert( gameType != GameType.Error, "Loaded wrong game type" );
                }

            }

            // Create game object
            game = new GameResult( playerIndex, surv1Result, surv2Result, surv3Result, surv4Result,
                gameType );

            // Set date
            if ( gameDate != DateTime.MinValue )
                game.setDate( gameDate );

            // Set BP
            for(int player=(int)PlayerIndex.Survivor1; player <= (int)PlayerIndex.Killer; player++ )
            {
                game.setBpAmount( ( PlayerIndex )player, bloodpoints[ player ] );
            }

            void rememberBP(PlayerIndex player, string fieldValue )
            {
                bloodpoints[ ( int )player ] = EndscoreBpDigitGfx.INVALID_BP_AMOUNT;

                fieldValue = fieldValue.Trim();
                if ( fieldValue != "" )
                    bloodpoints[ (int)player ] = int.Parse( fieldValue );
            }

            return game;
        }

        private static EndgameSurvivorIcon parsePlayerResultField( string sPlayerResultValue )
        {
            sPlayerResultValue = sPlayerResultValue.Trim();

            if ( sPlayerResultValue == gameLineSurvivorResultEscaped )
                return EndgameSurvivorIcon.Escaped;
            else if ( sPlayerResultValue == gameLineSurvivorResultSacrificed )
                return EndgameSurvivorIcon.KilledSacrificed;
            else if ( sPlayerResultValue == gameLineSurvivorResultMoried )
                return EndgameSurvivorIcon.KilledBleedoutOrMoried;
            else if ( sPlayerResultValue == gameLineSurvivorResultStillPlaying )
                return EndgameSurvivorIcon.SurvIconStillPlaying;
            else if ( sPlayerResultValue == gameLineSurvivorResultDCed )
                return EndgameSurvivorIcon.DCed;
            else if ( sPlayerResultValue == gameLineSurvivorResultNoPlayer )
                return EndgameSurvivorIcon.NoPlayer;
            else
            {
                Dbg.onDebugError( "Wrong surv result loaded: " + sPlayerResultValue );
            }

            return EndgameSurvivorIcon.Error;
        }

        private static PlayerIndex parsePlayerIndexField(string sPlayerIndexValue)
        {
            sPlayerIndexValue = sPlayerIndexValue.Trim();

            if ( sPlayerIndexValue == "0" )
                return PlayerIndex.Survivor1;
            else if ( sPlayerIndexValue == "1" )
                return PlayerIndex.Survivor2;
            else if ( sPlayerIndexValue == "2" )
                return PlayerIndex.Survivor3;
            else if ( sPlayerIndexValue == "3" )
                return PlayerIndex.Survivor4;
            else if ( sPlayerIndexValue == "4" )
                return PlayerIndex.Killer;
            else
            {
                Dbg.onDebugError( "Loaded invalid player index: " + sPlayerIndexValue );
            }

            return PlayerIndex.Error;
        }

        /// <summary>
        /// Save to disk
        /// </summary>
        public static void save()
        {
            Dbg.assert( bStatsLoaded, "Trying to save stats without loading them first," +
                " might delete all stats this way" );

            if ( !bStatsLoaded )
                return;


            //var comment = ";playerIndex(0-4)/s0_res/s1_res/s2_res/s3_res/k_res/pip?/duration/date/BP/killer_surv_name/";
            var ver = "ver=1.0";

            var result = ver + "\n\n";

            using ( StreamWriter file = new StreamWriter( filePath ) )
            {
                foreach ( var game in Stats.getGames() )
                {
                    var gameResult = "";

                    try
                    {
                        gameResult = game.getStringForSaving();

                    }
                    catch ( Exception e )
                    {
                        Dbg.onDebugError( "Error saving stats line: " + e.ToString() );
                    }

                    // Make more readable
                    gameResult = gameResult.Replace( gameFieldsSeparator, " " + gameFieldsSeparator + " " );

                    result = result + "\n" + gameResult;

                    // write line by line, in case we get an error and lose some data
                    file.WriteLine( gameResult );
                    file.Flush();
                }

                //file.Write( result );
            }
        }
    }
}
