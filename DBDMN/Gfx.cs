using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DBDMN.GamePoint;
using static DBDMN.ScreenParser;

namespace DBDMN
{
    public class Gfx
    {
        public static string _1080p = "1920x1080";
        public static string _720p = "1280x720";

        /// <summary>
        /// Game resolutions that we support
        /// </summary>
        public static List<string> supportedResolutions = new List<string> { _1080p, _720p };

        private string name = null;
        private bool bDebug = false;
        private List<GamePoint> gamePoints;



        /// <summary>
        /// Width of the crossplay icon. It moved "Ready" button by this amount to the left
        /// </summary>
        public static Dictionary<string, int> crossplayIconWidth = new Dictionary<string, int>();   // 1827 - 1781;    // 46

        /// <summary>
        /// Vertical offset betweenn player icons on the Endscreen Scoreboard (killer/survivor)
        /// </summary>
        public static Dictionary<string, int[]> verticalOffsetBetweenEndscorePlayerIcons =
            new Dictionary<string, int[]>();



        #region gfx elements

        // Red/Grey icons
        public static Gfx[] playerReadyIcon = new Gfx[ 5 ];

        public static Gfx endgameScoreSelectedPlayer = null;

        /// <summary>
        /// Observing how someone is playing after ending our game
        /// </summary>
        public static Gfx endgameObservingScreen = null;

        /// <summary> Crossplay icon in the lobby, when crossplay is off </summary>
        public static Gfx crossPlayIcon = null;

        /// <summary> Ready button killer/survivor. All coords are with "crossplay" icon shown. </summary>
        public static Gfx readyIcon = null;

        /// <summary>
        /// "UN" from "Unready". For both killer and survivor.
        /// All coords are with "crossplay" icon shown.
        /// </summary>
        public static Gfx unreadyIcon = null;

        /// <summary> "Cancel" text when searching for lobby as killer </summary>
        public static Gfx killerCancelIcon = null;

        /// <summary> "Cancel" text when searching for lobby as survivor. This is with "Crossplay" icon shown </summary>
        public static Gfx survivorCancelIcon = null;

        /// <summary> "looking for match..." text for survivors when searching for lobby </summary>
        public static Gfx survivorLookingForMatchText = null;

        /// <summary> Shop icon on the left of the screen, in the pre-lobby </summary>
        public static Gfx shopIcon = null;

        /// <summary> Any overlay dark-blue messages </summary>
        public static Gfx overlayDarkBlueMsg = null;

        /// <summary> "An unknown error occured" on the main menu or lobby, which brings back to main screen  </summary>
        public static Gfx unknownErrorMsg = null;

        /// <summary> "Are you sure you want to leave the lobby?" msg </summary>
        public static Gfx leaveLobbyConfirmationMsg = null;

        /// <summary> Endgame - "Scoreboard" </summary>
        public static Gfx endgameScoreboard = null;

        /// <summary> survivor edgame escape icon </summary>
        public static Gfx scoreboardSurvivorEscapeIcon = null;

        /// <summary> endscore survivor endgame DC icon </summary>
        public static Gfx scoreboardSurvivorDCIcon = null;

        /// <summary> Endscore player head icon (when someone is still playing or DC on load) </summary>
        public static Gfx scoreboardSurvivorPlayerHeadIcon = null;

        /// <summary> Endscore killed icon (thin skull) </summary>
        public static Gfx scoreboardSurvivorSacrificedIcon = null;

        /// <summary> Endscore moried or bleedout icon (thick skull) </summary>
        public static Gfx scoreboardSurvivorMoriedIcon = null;


        /// <summary> Black game loading screen with DBD logo </summary>
        public static Gfx anyBlackLoadingScreenWithDbdLogo = null;

        /// <summary> Loading bar on bottom, when loading into the game, when it's almost done </summary>
        public static Gfx loadingBarAlmostFinished = null;
        #endregion


        public static void initialize()
        {
            // Init digits recognition
            EndscoreBpDigitGfx.intitialize();

            crossplayIconWidth[_1080p] = 1827 - 1781;    // 46
            crossplayIconWidth[_720p] = 31;

            verticalOffsetBetweenEndscorePlayerIcons[ _1080p ] = new int[ 5 ];
            verticalOffsetBetweenEndscorePlayerIcons[ _1080p ][ ( int )PlayerIndex.Survivor1 ] = 0;
            verticalOffsetBetweenEndscorePlayerIcons[ _1080p ][ ( int )PlayerIndex.Survivor2 ] = 112;
            verticalOffsetBetweenEndscorePlayerIcons[ _1080p ][ ( int )PlayerIndex.Survivor3 ] = 112 * 2;
            verticalOffsetBetweenEndscorePlayerIcons[ _1080p ][ ( int )PlayerIndex.Survivor4 ] = 112 * 3 - 3;
            verticalOffsetBetweenEndscorePlayerIcons[ _1080p ][ ( int )PlayerIndex.Killer ] = 112 * 4 - 5;

            verticalOffsetBetweenEndscorePlayerIcons[ _720p ] = new int[ 5 ];
            verticalOffsetBetweenEndscorePlayerIcons[ _720p ][ ( int )PlayerIndex.Survivor1 ] = 0;
            verticalOffsetBetweenEndscorePlayerIcons[ _720p ][ ( int )PlayerIndex.Survivor2 ] = 75;
            verticalOffsetBetweenEndscorePlayerIcons[ _720p ][ ( int )PlayerIndex.Survivor3 ] = 75 * 2 - 1;
            verticalOffsetBetweenEndscorePlayerIcons[ _720p ][ ( int )PlayerIndex.Survivor4 ] = 75 * 3 - 3;
            verticalOffsetBetweenEndscorePlayerIcons[ _720p ][ ( int )PlayerIndex.Killer ] = 75 * 4 - 4;

            // Coords for players check icons
            playerReadyIcon[ ( int )PlayerIndex.Survivor1 ] = new Gfx( "Survivor 1 ready icon (left-most survivor line)",
            false, new List<GamePoint>
            { new GamePoint("Top", PointColor.WillProvidLater,
                    new Dictionary<string, string> { [ _1080p ] = "1773-919", [_720p] = "1182-612"}),
                new GamePoint("Bottom, under killer line", PointColor.WillProvidLater,
                    new Dictionary<string, string> { [_1080p] = "1773-941", [_720p] = "1181-626"}) } );
            playerReadyIcon[ ( int )PlayerIndex.Survivor2 ] = new Gfx( "Survivor 2 ready icon",
            false, new List<GamePoint>
            { new GamePoint("Top", PointColor.WillProvidLater,
                    new Dictionary<string, string> { [_1080p] = "1787-919", [_720p] = "1191-609"}),
                new GamePoint("Bottom, under killer line", PointColor.WillProvidLater,
                    new Dictionary<string, string> { [_1080p] = "1787-941", [_720p] = "1191-616"}) } );
            playerReadyIcon[ ( int )PlayerIndex.Survivor3 ] = new Gfx( "Survivor 3 ready icon",
            false, new List<GamePoint>
            { new GamePoint("Top", PointColor.WillProvidLater,
                    new Dictionary<string, string> { [_1080p] = "1800-919", [_720p] = "1200-609"}),
                new GamePoint("Center, above killer line", PointColor.WillProvidLater,
                    new Dictionary<string, string> { [_1080p] = "1800-929", [_720p] = "1200-618"}) } );
            playerReadyIcon[ ( int )PlayerIndex.Survivor4 ] = new Gfx( "Survivor 4 ready icon (right-most line)",
            false, new List<GamePoint>
            { new GamePoint("Top", PointColor.WillProvidLater,
                    new Dictionary<string, string> { [_1080p] = "1813-920", [_720p] = "1208-613"}),
                new GamePoint("Center, above killer line", PointColor.WillProvidLater,
                    new Dictionary<string, string> { [_1080p] = "1813-930", [_720p] = "1208-619"}) } );
            playerReadyIcon[ ( int )PlayerIndex.Killer ] = new Gfx( "Killer ready icon",
            false, new List<GamePoint>
            { new GamePoint("Left", PointColor.WillProvidLater,
                    new Dictionary<string, string> { [_1080p] = "1761-928", [_720p] = "1171-618"}),
                new GamePoint("Center, between 2nd and 3rd line", PointColor.WillProvidLater,
                    new Dictionary<string, string> { [_1080p] = "1793-936", [_720p] = "1195-625"}) } );

            loadingBarAlmostFinished = new Gfx( "Loading bar on bottom, when loading into the game, when it's almost done",
            false, new List<GamePoint>
            {
                new GamePoint("Left-most part of the loading bar: filled", PointColor.DarkGrey,
                    new Dictionary<string, string> { [_1080p] = "590-993", [_720p] = "396-662"}),
                new GamePoint("At about 50%: filled", PointColor.DarkGrey,
                    new Dictionary<string, string> { [_1080p] = "900-993", [_720p] = "600-662"}),
                new GamePoint("At about 80%: filled", PointColor.DarkGrey,
                    new Dictionary<string, string> { [_1080p] = "1090-993", [_720p] = "800-662"}),
                new GamePoint("At about 95%: either filled or not filled yet", PointColor.DarkOrVeryDarkGrey,
                    new Dictionary<string, string> { [_1080p] = "1217-993", [_720p] = "858-663"}),
                new GamePoint("To the left of the load bar", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "459-996", [_720p] = "367-663"}),
                new GamePoint("To the right of the load bar", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "1426-996", [_720p] = "909-663"}),
                new GamePoint("Undet the load bar", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "950-1030", [_720p] = "650-690"}),
                new GamePoint("Very top left part of the black screen", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "40-40", [_720p] = "30-30"}),
                new GamePoint("Very top right part of the black screen", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "1870-40", [_720p] = "1250-30"}),
                new GamePoint("Very bottom right part of the black screen", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "1870-1030", [_720p] = "1250-690"}),
                new GamePoint("Very bottom left part of the black screen", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "40-1030", [_720p] = "30-690"}),

            } );

            anyBlackLoadingScreenWithDbdLogo = new Gfx( "Black game loading screen with DBD logo",
            false, new List<GamePoint>
            {
                new GamePoint("DBD Logo: Horizontal killer line: Left-most part", PointColor.DbdLogoColor,
                    new Dictionary<string, string> { [_1080p] = "745-434", [_720p] = "469-281"}),
                new GamePoint("DBD Logo: Horizontal killer line: Right-most part", PointColor.DbdLogoColor,
                    new Dictionary<string, string> { [_1080p] = "1120-521", [_720p] = "739-347"}),
                new GamePoint("DBD Logo: First vertical line: Top part", PointColor.DbdLogoColor,
                    new Dictionary<string, string> { [_1080p] = "820-406", [_720p] = "547-269"}),
                new GamePoint("DBD Logo: Second vertical line: Top part", PointColor.DbdLogoColor,
                    new Dictionary<string, string> { [_1080p] = "892-365", [_720p] = "590-227"}),
                new GamePoint("DBD Logo: Third vertical line: Top part", PointColor.DbdLogoColor,
                    new Dictionary<string, string> { [_1080p] = "986-407", [_720p] = "660-252" } ),
                new GamePoint("DBD Logo: Fourth vertical line: Top part", PointColor.DbdLogoColor,
                    new Dictionary<string, string> { [_1080p] = "1083-429", [_720p] = "709-267" } ),
                new GamePoint("To the left of the logo", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "584-423", [_720p] = "411-273" } ),
                new GamePoint("To the right of the logo", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "1340-568", [_720p] = "863-377" } ),
                new GamePoint("Between 1st and 2nd vertical line: top part", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "861-381", [_720p] = "573-252" } ),
                new GamePoint("Between 2st and 3rd vertical line: top part", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "951-385", [_720p] = "634-273" } ),
                new GamePoint("Between 3rd and 4th vertical line: top part", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "1037-405", [_720p] = "691-293" } )
            } );

            scoreboardSurvivorMoriedIcon = new Gfx( "Endscore moried or bleedout icon (thick skull)",
            false, new List<GamePoint>
            {
                new GamePoint("Top of the head", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "837-307", [_720p] = "558-205"}),
                new GamePoint("Left part of the head", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "832-323", [_720p] = "553-206"}),
                new GamePoint("Right part of the head", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "842-323", [_720p] = "564-206"}),
                new GamePoint("Left eye", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "830-318", [_720p] = "553-212"}),
                new GamePoint("Right eye", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "842-318", [_720p] = "561-212"}),
                new GamePoint("Nose", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "837-324", [_720p] = "558-215"})
            } );

            scoreboardSurvivorSacrificedIcon = new Gfx( "Endscore sacrificed icon (thin skull)",
            false, new List<GamePoint>
            {
                new GamePoint("Top of the head", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "837-307", [_720p] = "558-205"}),
                new GamePoint("Left part of the head", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "832-320", [_720p] = "553-206"}),
                new GamePoint("Right part of the head", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "842-321", [_720p] = "564-207"}),
                new GamePoint("Left eye", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "832-314", [_720p] = "554-210"}),
                new GamePoint("Right eye", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "843-315", [_720p] = "561-210"}),
                new GamePoint("Mouth", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "838-329", [_720p] = "559-220"})
            } );

            scoreboardSurvivorPlayerHeadIcon = new Gfx( "Endscore player head icon",
            false, new List<GamePoint>
            {
                new GamePoint("Top of the head", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "836-306", [_720p] = "558-206"}),
                new GamePoint("Bottom left", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "822-331", [_720p] = "550-220"}),
                new GamePoint("Bottom right", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "850-333", [_720p] = "565-221"}),
                new GamePoint("To the left of the neck", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "830-322", [_720p] = "552-215"}),
                new GamePoint("To the right of the neck", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "843-321", [_720p] = "564-213"})
            } );

            scoreboardSurvivorDCIcon = new Gfx( "endscore survivor DC icon",
            false, new List<GamePoint>
            {
                new GamePoint("Red X mark in DC icon", PointColor.Red,
                    new Dictionary<string, string> { [_1080p] = "851-327", [_720p] = "566-217"}),
                new GamePoint("Left part of the plug", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "834-320", [_720p] = "555-213"}),
                new GamePoint("Right part of the plug", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "849-317", [_720p] = "566-211"})
            } );

            scoreboardSurvivorEscapeIcon = new Gfx( "survivor edgame escape icon",
            false, new List<GamePoint>
            {
                new GamePoint("Inside the head", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "834-310", [_720p] = "556-207"}),
                new GamePoint("To the right of the head", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "841-308", [_720p] = "560-206"}),
                new GamePoint("Inside the body", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "830-322", [_720p] = "553-215"}),
                new GamePoint("To the right of the body", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "840-332", [_720p] = "559-221"})
            } );

            endgameScoreboard = new Gfx( "Endgame - 'Scoreboard'",
            false, new List<GamePoint>
            {
                new GamePoint("'S' char in red 'scoreboard' text at the top, center of char", PointColor.Red,
                    new Dictionary<string, string> { [_1080p] = "104-150", [_720p] = "70-92"}),
                new GamePoint("Inside 'S', max ~70 brightness", PointColor.DarkAndNotRed,
                    new Dictionary<string, string> { [_1080p] = "104-156", [_720p] = "69-103"}),
                new GamePoint("'D' char in 'scoreboard'", PointColor.Red,
                    new Dictionary<string, string> { [_1080p] = "318-150", [_720p] = "221-96"}),
                new GamePoint("Inside 'D' char, max ~70 brightness", PointColor.DarkAndNotRed,
                    new Dictionary<string, string> { [_1080p] = "326-150", [_720p] = "217-99"}),

                new GamePoint("'E' char, top horizontal line in 'Continue'", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "1802-999", [_720p] = "1200-666"}),

                new GamePoint("Inside 'E' char in 'Continue'", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "1802-1002", [_720p] = "1201-673"}),

                new GamePoint("'E' char, middle horizontal line in 'Continue'", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "1802-1006", [_720p] = "1200-670"}),

                new GamePoint("Vertical red line to the right of 'Continue'", PointColor.Red,
                    new Dictionary<string, string> { [_1080p] = "1821-1005", [_720p] = "1214-667"}),
                new GamePoint("Top horizontal black like. It fades-in, needs 2 secs to get fully shown", PointColor.BrightnessMax15,
                    new Dictionary<string, string> { [_1080p] = "189-235", [_720p] = "166-155"}),
                new GamePoint("Red Killer chart line. Need this one, it gets shown only 2 secs after the chart gfx", PointColor.Red,
                    new Dictionary<string, string> { [_1080p] = "402-725", [_720p] = "223-478"})
            } );

            leaveLobbyConfirmationMsg = new Gfx( "'Are you sure you want to leave the lobby ?' msg",
            false, new List<GamePoint>
            {
                new GamePoint("First 'L' char", PointColor.White,
                    new Dictionary<string, string> { [_1080p] = "856-464", [_720p] = "569-317"}),
                new GamePoint("Inside 'L' char", PointColor.NotWhite,
                    new Dictionary<string, string> { [_1080p] = "865-464", [_720p] = "574-309"}),
                new GamePoint("'Y' char", PointColor.NotWhite,
                    new Dictionary<string, string> { [_1080p] = "1070-467", [_720p] = "714-303"})
            } );

            unknownErrorMsg = new Gfx( "'An unknown error occured' on the main menu or lobby",
            true, new List<GamePoint>
            {
                new GamePoint("'E' char in 'Error' msg caption", PointColor.White,
                    new Dictionary<string, string> { [_1080p] = "921-466", [_720p] = "610-310"}),
                new GamePoint("Inside 'E' char", PointColor.NotWhite,
                    new Dictionary<string, string> { [_1080p] = "921-471", [_720p] = "614-307"}),
                new GamePoint("Before 'E' char", PointColor.NotWhite,
                    new Dictionary<string, string> { [_1080p] = "903-466", [_720p] = "601-310"})
            } );


            overlayDarkBlueMsg = new Gfx( "Any overlay dark-blue messages",
            false, new List<GamePoint>
            {
                new GamePoint("Top dark-blue part left", PointColor.DarkBlue,
                    new Dictionary<string, string> { [_1080p] = "85-465", [_720p] = "64-309"}),
                new GamePoint("Top dark-blue part right", PointColor.DarkBlue,
                    new Dictionary<string, string> { [_1080p] = "1820-465", [_720p] = "1184-308"}),
                new GamePoint("Bottom black part left", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "85-585", [_720p] = "64-387"}),
                new GamePoint("Bottom black part right", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "1820-585", [_720p] = "1184-405"})
            } );

            shopIcon = new Gfx( "'Shop icon on the left of the screen, in the pre-lobby",
            false, new List<GamePoint>
            {
                new GamePoint("Very top left corner (otherwise no pitch-black on mouse-over)", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "88-794", [_720p] = "52-530"}),
                //new GamePoint("Black top right corner", PointColor.Black,     // it's just black, not PitchBlack
                //    new Dictionary<string, string> { [_1080p] = "184-795"}),
                new GamePoint("Very bottom right corner (otherwise no pitch-black on mouse-over)", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "185-882", [_720p] = "123-588"}),
                new GamePoint("Very bottom left corner (otherwise no pitch-black on mouse-over)", PointColor.PitchBlack,
                    new Dictionary<string, string> { [_1080p] = "82-880", [_720p] = "54-586"}),
                new GamePoint("Inside white shop icon1, can be yellow or mouse-over 1", PointColor.BrighterThanDark,
                    new Dictionary<string, string> { [_1080p] = "136-844", [_720p] = "90-563"}),
                new GamePoint("Inside white shop icon2, can be yellow or mouse-over 2", PointColor.BrighterThanDark,
                    new Dictionary<string, string> { [_1080p] = "155-834", [_720p] = "103-556"})
            } );


            survivorLookingForMatchText = new Gfx( "'looking for match...' text for survivors when searching for lobby",
            false, new List<GamePoint>
            {
                new GamePoint("'h' char", PointColor.White,
                    new Dictionary<string, string> { [_1080p] = "1710-927", [_720p] = "1140-618"}),
                new GamePoint("Between 'for' and 'match'", PointColor.NotWhite,
                    new Dictionary<string, string> { [_1080p] = "1659-932", [_720p] = "1106-621"}),
                new GamePoint("Between 'looking' and 'for'", PointColor.NotWhite,
                    new Dictionary<string, string> { [_1080p] = "1630-932", [_720p] = "1086-621"}),
                new GamePoint("'k' char", PointColor.White,
                    new Dictionary<string, string> { [_1080p] = "1593-931", [_720p] = "1061-620"}),
                new GamePoint("Inside first 'L'", PointColor.NotWhite,
                    new Dictionary<string, string> { [_1080p] = "1563-930", [_720p] = "1042-619"})

            } );


            survivorCancelIcon = new Gfx( "'Cancel' text when searching for lobby as survivor. This is with 'Crossplay' icon shown",
            false, new List<GamePoint>
            {
                new GamePoint("Vertical red line in survivor 'Cancel' button", PointColor.Red,
                    new Dictionary<string, string> { [_1080p] = "1774-1007", [_720p] = "1214-670"}),
                new GamePoint("To the right of 'L' char", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "1748-1007", [_720p] = "1197-670"}),
                new GamePoint("'L' character", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "1743-1012", [_720p] = "1192-671"}),
                new GamePoint("Inside 'C' character", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "1645-1007", [_720p] = "1171-671"}),
                new GamePoint("'C' character", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "1640-1007", [_720p] = "1166-672"})
            } );


            killerCancelIcon = new Gfx( "'Cancel' text when searching for lobby as killer",
            false, new List<GamePoint>
            {
                new GamePoint("Vertical red line in 'Cancel'", PointColor.Red,
                    new Dictionary<string, string> { [_1080p] = "1827-940", [_720p] = "1218-627"}),
                new GamePoint("To the right of 'L' char", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "1812-940", [_720p] = "1209-626"}),
                new GamePoint("'L' character", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "1808-940", [_720p] = "1205-627"}),
                new GamePoint("Inside 'C' character", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "1751-940", [_720p] = "1193-627"}),
                new GamePoint("'C' character", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "1746-940", [_720p] = "1164-627"})
            } );


            unreadyIcon = new Gfx( "'UN' from 'Unready'. For both killer and survivor. With crosslay icon shown",
            false, new List<GamePoint>
            {
                new GamePoint("Between 'N' and 'R'", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "1656-1007", [_720p] = "1104-672"}),
                new GamePoint("Letter 'N'", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "1639-1001", [_720p] = "1101 -676"}),
                new GamePoint("Between 'U' and 'N'", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "1634-1007", [_720p] = "1090-671"}),
                new GamePoint("Letter 'U'", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "1617-1007", [_720p] = "1087-670"}),
                new GamePoint("Inside 'U'", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "1623-1007", [_720p] = "1083-670"})
            } );



            readyIcon = new Gfx( "Ready button killer/survivor with crosslay icon shown",
            false, new List<GamePoint>
            {
                new GamePoint("Red vertical line", PointColor.Red,
                    new Dictionary<string, string   > { [_1080p] = "1775-1008", [_720p] = "1183-671"}),
                new GamePoint("Letter 'Y', white only when mouseover", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "1745-1007", [_720p] = "1164-671"}),
                new GamePoint("Inside letter 'Y'", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "1745-998", [_720p] = "1164-665"}),
                new GamePoint("Letter 'R'", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "1660-998", [_720p] = "1113-672"}),
                new GamePoint("Indise letter 'R'", PointColor.NotFontColor,
                    new Dictionary<string, string> { [_1080p] = "1665-1003", [_720p] = "1111-668"})
            } );






            crossPlayIcon = new Gfx( "Crossplay icon in the lobby, when crossplay is off",
            false, new List<GamePoint>
            {
                new GamePoint("Red X", PointColor.Red,
                    new Dictionary<string, string> { [_1080p] = "1817-1016", [_720p] = "1211-677"}),
                new GamePoint("The globe", PointColor.FontColor,
                    new Dictionary<string, string> { [_1080p] = "1804-1004", [_720p] = "1200-659"}),
                new GamePoint("Inside the globe icon", PointColor.Black,
                    new Dictionary<string, string> { [_1080p] = "1800-1000", [_720p] = "1206-671"})
            } );

            endgameScoreSelectedPlayer = new Gfx( "Selected player in the Endgame Scoreboard table",
            false, new List<GamePoint>
            {
                new GamePoint("Right above the player rank number", PointColor.BrightnessMax15,
                    new Dictionary<string, string> { [_1080p] = "131-294", [_720p] = "87-197"}),
                new GamePoint("Right under the player rank number", PointColor.BrightnessMax15,
                    new Dictionary<string, string> { [_1080p] = "132-324", [_720p] = "87-215"})
            } );

            endgameObservingScreen = new Gfx( "Observing someone else playing after ending our survivor game",
                false, new List<GamePoint>
                {
                    new GamePoint("Black patch, above player name in the bottom-center", PointColor.Black,
                        new Dictionary<string, string> { [_1080p] = "955-977", [_720p] = "638-655" } ),
                    new GamePoint("Black patch, under player name in the bottom-center", PointColor.Black,
                        new Dictionary<string, string> { [_1080p] = "955-1020", [_720p] = "638-674" } ),
                    new GamePoint("Back button, vertical red line", PointColor.Red,
                        new Dictionary<string, string> { [_1080p] = "1820-1000", [_720p] = "1214-663" } ),
                    new GamePoint("Back button, 'K' char", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "1793-1001", [_720p] = "1195-667" } ),
                    new GamePoint("Back button, inside 'C' char", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "1781-1000", [_720p] = "1187-666" } ),
                    new GamePoint("Back button, 'B' char", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "1741-1000", [_720p] = "1160-666" } ),
                    new GamePoint("Back button, before 'B' char", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "1734-1000", [_720p] = "1153-667" } )
                } );
        }


        public Gfx(string name, bool bDebug, List<GamePoint> gamePoints )
        {
            this.name = name;
            this.bDebug = bDebug;
            this.gamePoints = gamePoints;

            // Remember all in a list to be able to do tests with
            // all these objects on startup
            ScreenParser.allGfxElements.Add(this);

        }

        public bool recognize( string resolution, bool bDebug )
        {
            return recognize( resolution, 0, 0, bDebug );
        }

        /// <summary>
        /// Recognize itself on image
        /// </summary>
        /// <param name="verticalOffset">Used as offset for recognizing survivor result icons
        /// (escaped/killed/DCed) at the endgame scoreboard</param>
        public bool recognize(string resolution, int horizontalOffset = 0, int verticalOffset = 0,
            bool bDebug = false, PointColor color = PointColor.Default )
        {
            foreach(var gamePoint in gamePoints)
            {
                //if(Dbg.bTest)
                //{
                //    Debugger.Break();
                //}

                bool bRecognized = isGamePointRecognized( gamePoint, resolution, 
                    horizontalOffset, verticalOffset, color );

                if(bDebug && !bRecognized)
                {
                    //Debugger.Break();
                    //List<Point> point = gamePoint.getAlternatePoint( resolution );

                    //PointColor pointColor = gamePoint.getPointColor();
                    //var pixelColor = ScreenCapture.getScreenShot().GetPixel( point.X + horizontalOffset,
                    //    point.Y + verticalOffset );

                    //Log.log( "Point '" + gamePoint.getDescription() + ": " + gamePoint.getPointColor() + 
                    //    "' not recognized (" + point.ToString() + " - " + pixelColor.ToString() + ")" );

                    Log.log( "Point '" + gamePoint.getDescription() + ": " + gamePoint.getPointColor() +
                        "' not recognized" );
                }

                if ( !bRecognized )
                {
                    return false;
                }
            }

            return true;
        }

        public bool recognize( )
        {
            Dbg.assert( this is EndscoreBpDigitGfx == false, "This class is not supported by this method" );

            var gameResolution = ScreenCapture.getScreenshotResolutionAsString();

            return recognize( gameResolution );
        }

        public bool recognize( PointColor color )
        {
            Dbg.assert( color != PointColor.WillProvidLater &&
                color != PointColor.Error, "Wrong color provided" );

            var gameResolution = ScreenCapture.getScreenshotResolutionAsString();

            return recognize( gameResolution, 0, 0, false, color );
        }

        public bool recognize( int horizontalOffset, int verticalOffset, bool bDebug = false )
        {
            var gameResolution = ScreenCapture.getScreenshotResolutionAsString();

            return recognize( gameResolution, horizontalOffset, verticalOffset, bDebug );
        }

        /// <summary>
        /// Get point coord and check its color if it fits
        /// </summary>
        private bool isGamePointRecognized(GamePoint gamePoint, string resolution,
            int horizontalOffset = 0, int verticalOffset = 0, PointColor color = PointColor.Default )
        {
            PointColor pointColor = gamePoint.getPointColor();

            // Do we pass the color with the call and not during data definition?
            if( pointColor == PointColor.WillProvidLater)
            {
                // Make sure we have provided a real color
                Dbg.assert( color != PointColor.WillProvidLater && color != PointColor.Default &&
                    color != PointColor.Error );

                pointColor = color;
            }

            List<Point> alternatePoints = gamePoint.getAlternatePoint( resolution );

            // If 1 of any alternate points is recognized, then point of gfx object is recognized
            foreach(Point point in alternatePoints)
            {
                var pixelColor = ScreenCapture.getScreenShot().GetPixel( point.X + horizontalOffset,
                    point.Y + verticalOffset );

                if ( isPixelColorFitsGameColor( pixelColor, pointColor ) )
                    return true;
            }


            return false;
        }

        /// <summary>
        /// Check if color fits the provided PointColor
        /// </summary>
        private static bool isPixelColorFitsGameColor(Color color, PointColor pointColor)
        {
            switch ( pointColor )
            {
                case PointColor.BrighterThanDark:
                    return ScreenParser.isPixelBrighterThanDark( color );
                case PointColor.BrightnessMax15:
                    return ScreenParser.isPixelMaxBrightless15( color );
                case PointColor.Black:
                    return ScreenParser.isPixelBlack( color );
                case PointColor.PitchBlack:
                    return ScreenParser.isPixelPitchBlack( color );
                case PointColor.DarkBlue:
                    return ScreenParser.isPixelVeryDarkBlue( color );
                case PointColor.DarkGrey:
                    return ScreenParser.isPixelDarkGrey( color );
                case PointColor.VeryDarkGrey:
                    return ScreenParser.isPixelVeryDarkGrey( color );
                case PointColor.DarkOrVeryDarkGrey:
                    return ScreenParser.isPixelDarkOrVeryDarkGrey( color );
                case PointColor.Red:
                    return ScreenParser.isPixelRed( color );
                case PointColor.DarkAndNotRed:
                    return ScreenParser.isPixelDarkAndNotRed( color );
                case PointColor.DbdLogoColor:
                    return ScreenParser.isPixelOfDbdLogoColor( color );
                case PointColor.White:
                    return ScreenParser.isPixelWhite( color );
                case PointColor.NotWhite:
                    return !ScreenParser.isPixelWhite( color );
                case PointColor.FontColor:
                    return ScreenParser.isPixelOfFontColor( color );
                case PointColor.NotFontColor:
                    return !ScreenParser.isPixelOfFontColor( color );
                case PointColor.Error:
                default:
                    throw new Exception( "Wrong color" );
            }
        }

        /// <summary>
        /// Given survivor index, calculate vertical position of his result icon (escaped/killed/DC)
        /// </summary>
        public static int getEndgameVerticalIconOffsetForPlayer( PlayerIndex survivorIndex )
        {
            Dbg.assertPlayerIndexIsInRangeAndNotInvalid( survivorIndex );

            var resolution = ScreenCapture.getScreenshotResolutionAsString();

            int vOffset = Gfx.verticalOffsetBetweenEndscorePlayerIcons[ resolution ][ ( int )survivorIndex ];

            //int curVerticalIconOffset = 0;

            //switch ( survivorIndex )
            //{
            //    case PlayerIndex.Survivor1:
            //        curVerticalIconOffset = 0;
            //        break;
            //    case PlayerIndex.Survivor2:
            //        curVerticalIconOffset = vOffset;
            //        break;
            //    case PlayerIndex.Survivor3:
            //        curVerticalIconOffset = 2 * vOffset;
            //        break;
            //    case PlayerIndex.Survivor4:
            //        curVerticalIconOffset = 3 * vOffset - 3;
            //        break;
            //    case PlayerIndex.Killer:
            //        curVerticalIconOffset = 4 * vOffset - 5;
            //        break;
            //    default:
            //        Debug.Assert( false, "hasEndgameScoreboardSurv1EscapeIcon() - wrong player index" );
            //        break;
            //}

            return vOffset;
        }

    }

}
