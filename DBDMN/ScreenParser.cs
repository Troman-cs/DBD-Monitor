using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DBDMN.GamePoint;
using static DBDMN.GameResult;
using static DBDMN.StateManager;
using static DBDMN.Stats;

namespace DBDMN
{
    public static class ScreenParser
    {
        public static List<Gfx> allGfxElements = new List<Gfx>();

        /// <summary>
        /// Time when we found DBD window
        /// </summary>
        private static DateTime timeFoundDBD = DateTime.MinValue;

        public enum EndgameSurvivorIcon
        {
            Escaped,
            DCed,
            KilledSacrificed,           // Thin skull
            KilledBleedoutOrMoried,           // Thick skull
            SurvIconStillPlaying,   // Still playing or someone else DCed
            NoPlayer,               // In a custom game certain survivor spots might not be filled
            DefaultKillerIcon,      // Just a default killer knife
            Error
        }

        //// Screen width/height for the points defined here
        //private static int baseGameScreenWidth = 1920;
        //private static int baseGameScreenHeight = 1080;

        // Current resolution of the game that we are dealing with
        private static int curGameScreenWidth = int.MinValue;     // recalc aspect ratio at the beginning
        private static int curGameScreenHeight = int.MinValue;
        //private static float gameDPI = 0f;
        //private static float myDPI = 0f;
        //private static bool bIsWindowed = false;        // DBD is windowed mode?

        //// Aspect ratio relative to our base resolution
        //private static float curHorizontalAspectRatio = 0f;
        //private static float curVerticalAspectRatio = 0f;

        //private static int[] playerCheckmarkX = new int[5];
        //private static int[] playerCheckmarkY = new int[5];

        //private static Dictionary<string, Dictionary<PlayerIndex, int>>  playerCheckmarkX = 
        //    new Dictionary<string, Dictionary<PlayerIndex, int>>();
        //private static Dictionary<string, Dictionary<PlayerIndex, int>> playerCheckmarkY = 
        //    new Dictionary<string, Dictionary<PlayerIndex, int>>();



        /* Darkness values:
         * pitch-black: RGB <= 5
         * black: RGB <= 25
         * very dark: RGB < 50
         * dark: RGB < 100
         * medium: 100-150
         * bright: 150-200
         * very bright: 200-240
         * white: RGB > 240
         */

        //// Crossplay Icon
        //private static Point crossplayIconRed = new Point( 1817, 1016 );
        //private static Point crossplayIconGreyOrWhite = new Point( 1804, 1004 );
        //private static Point crossplayIconBlack = new Point( 1800, 1000);      // Inside the globe icon



        //#region Ready button killer/survivor
        ///// <summary>
        ///// Lobby "Ready" button.
        ///// For both killer and survivor. All coords are with "crossplay" icon shown.
        ///// </summary>
        //private static Point readyIconRedVerticalLine = new Point( 1775, 1008);
        //private static Point readyIconGreyOrWhite = new Point(1745, 1007);    // Letter "Y", white only when mouseover
        //private static Point readyIconNotGrey1 = new Point(1745, 998);       // Inside letter "Y"
        //private static Point readyIconGreyOrWhile2 = new Point(1660, 998);       // Letter "R"
        //private static Point readyIconNotGrey2 = new Point(1665, 1003);       // Indise letter "R"
        //#endregion

        //#region Unready button for killer/survivor
        //// "UN" from "Unready". For both killer and survivor. All coords are with "crossplay" icon shown.
        //private static Point unreadyIconNotGrey1 = new Point(1656, 1007);       // Between "N" and "R"
        //private static Point unreadyIconGreyOrWhite1 = new Point(1639, 1001);    // Letter "N"
        //private static Point unreadyIconNotGrey2 = new Point(1634, 1007);       // Between "U" and "N"
        //private static Point unreadyIconGreyOrWhite2 = new Point(1617, 1007);    // Letter "U"
        //private static Point unreadyIconNotGrey3 = new Point(1623, 1007);       // Inside "U"
        //#endregion

        //#region "Cancel" text when searching for lobby as killer
        //private static Point killerCancelIconRedVerticalLine = new Point(1827, 940);
        //private static Point killerCancelIconNotGrey1 = new Point(1812, 940);        // To the right of "L" char
        //private static Point killerCancelIconGreyOrWhite1 = new Point(1808, 940);           // "L" character
        //private static Point killerCancelIconNotGrey2 = new Point(1751, 940);        // Inside "C" character
        //private static Point killerCancelIconGreyOrWhite2 = new Point(1746, 940);        // "C" character
        //#endregion

        //#region "Cancel" text when searching for lobby as survivor. This is with "Crossplay" icon shown
        //private static Point survivorCancelIconRedVerticalLine = new Point(1774, 1007);
        //private static Point survivorCancelIconNotGrey1 = new Point(1748, 1007);        // To the right of "L" char
        //private static Point survivorCancelIconGreyOrWhite1 = new Point(1743, 1012);           // "L" character
        //private static Point survivorCancelIconNotGrey2 = new Point(1645, 1007);        // Inside "C" character
        //private static Point survivorCancelIconGreyOrWhite2 = new Point(1640, 1007);        // "C" character
        //#endregion

        //#region "looking for match..." text for survivors when searching for lobby
        //private static Point survivorLookingForMatchTextWhite1 = new Point(1710, 927);   // "h" char
        //private static Point survivorLookingForMatchTextNonWhite1 = new Point(1659, 932);   // Between "for" and "match"
        //private static Point survivorLookingForMatchTextNonWhite2 = new Point(1630, 932);   // Between "looking" and "for"
        //private static Point survivorLookingForMatchTextWhite2 = new Point(1593, 931);   // "k" char
        //private static Point survivorLookingForMatchTextNonWhite3 = new Point(1563, 930);   // Inside "L"
        //#endregion

        //#region shopIcon
        //private static Point shopIconPitchBlack1 = new Point(88, 794);
        //private static Point shopIconPitchBlack2 = new Point(184, 795);
        //private static Point shopIconPitchBlack3 = new Point(180, 880);
        //private static Point shopIconPitchBlack4 = new Point(88, 880);
        //private static Point shopIconGreyOrWhite1 = new Point(136, 844);
        //private static Point shopIconGreyOrWhite2 = new Point(155, 834);
        //#endregion

        //#region overlay messages
        //private static Point overlayMessageVeryDarkBlue1 = new Point(85, 465);     // Left
        //private static Point overlayMessageVeryDarkBlue2 = new Point(1820, 465);   // Right
        //private static Point overlayMessagePitchBlack1 = new Point(85, 585);   // Left
        //private static Point overlayMessagePitchBlack2 = new Point(1820, 585);   // Right

        //// "An unknown error occured" on the main meny or lobby
        //private static Point unknownErrorWhite1 = new Point(921, 466);   // "E" char in "Error" msg caption
        //private static Point unknownErrorNotWhite1 = new Point(921, 471);   // Inside "E" char
        //private static Point unknownErrorNotWhite2 = new Point(903, 466);   // Before "E" char

        //private static Point leaveLobbyConfirmationMsgWhite1 = new Point(856, 464);   // First "L" char
        //private static Point leaveLobbyConfirmationMsgNotWhite1 = new Point(865, 464);   // Inside "L" char
        //private static Point leaveLobbyConfirmationMsgWhite2 = new Point(1070, 467);   // "Y" char
        //#endregion

        //#region Endgame - "Scoreboard"
        //private static Point endgameScoreboardRed1 = 
        //    new Point(104, 150);   // "S" char in red "scoreboard" text at the top, center of char
        //private static Point endgameScoreboardDarkAndNotRed1 = 
        //    new Point(104, 156);   // Inside "S", max ~70 brightness
        //private static Point endgameScoreboardRed2 = 
        //    new Point(318, 150);       // "D" char in "scoreboard"
        //private static Point endgameScoreboardDarkAndNotRed2 = 
        //    new Point(326, 150);   // Inside "D" char, max ~70 brightness
        //private static Point endgameScoreboardFontColor1 = 
        //    new Point(1802, 999);   // "E" char in "Continue"
        //private static Point endgameScoreboardNotFontColor =
        //    new Point(1802, 1002);   // Inside "E" char in "Continue"
        //private static Point endgameScoreboardFontColor2 =
        //    new Point(1802, 1006);   // "E" char in "Continue"
        //private static Point endgameScoreboardRed3 =
        //    new Point(1821, 1005);   // Vertical red line to the right of "Continue"
        //private static Point endgameScoreboardBlackFadeInLineMaxBrightness15 =
        //    new Point( 189, 235 );   // Top horizontal black like. It fades-in, needs 2 secs to get fully shown
        //// Need this one to see if the chart is up already and not just the base screen.
        //// First the base screen gets shown, the chart with stats gets shown a little delayed.
        //private static Point endgameScoreboardRedKillerChartLineRedColor =
        //    new Point( 402, 725 );




        //#region survivor edgame escape icon
        //private static Point survivor1EscapeIconFontColor1 =
        //    new Point(834, 310);    // Inside the head
        //private static Point survivor1EscapeIconNotFontColor1 =
        //    new Point(841, 308);    // To the right of the head
        //private static Point survivor1EscapeIconFontColor2 =
        //    new Point(830, 322);    // Inside the body
        //private static Point survivor1EscapeIconNotFontColor2 =
        //    new Point(840, 332);    // To the right of the body
        //#endregion

        //#region endscore survivor endgame DC icon
        //private static Point survivor1DCIconRed1 = new Point(851, 327);         // Red X mark in DC icon
        //private static Point survivor1DCIconFontColor1 = new Point(834, 320);    // Left part of the plug
        //private static Point survivor1DCIconFontColor2 = new Point(849, 317);    // Right part of the plug
        //#endregion

        //#region endscore player head icon (when someone else DCed)
        //private static Point survivor1PlayerHeadIconFontColor1 = new Point(836, 306);    // Top of the head
        //private static Point survivor1PlayerHeadIconFontColor2 = new Point(822, 331);    // Bottom left
        //private static Point survivor1PlayerHeadIconFontColor3 = new Point(850, 333);    // Bottom right
        //private static Point survivor1PlayerHeadIconNotFontColor1 = new Point(830, 322);    // To the left of the neck
        //private static Point survivor1PlayerHeadIconNotFontColor2 = new Point(843, 321);    // To the right of the neck
        //#endregion

        //#region endscore killed icon (thin skull)
        //private static Point survivor1PlayerKilledIconFontColor1 = new Point(837, 307);    // Top of the head
        //private static Point survivor1PlayerKilledIconFontColor2 = new Point(832, 320);    // Left part of the head
        //private static Point survivor1PlayerKilledIconFontColor3 = new Point(842, 321);    // Left part of the head
        //private static Point survivor1PlayerKilledIconNotFontColor1 = new Point(832, 314);    // Left eye
        //private static Point survivor1PlayerKilledIconNotFontColor2 = new Point(843, 315);    // Right eye
        //private static Point survivor1PlayerKilledIconNotFontColor3 = new Point(838, 329);    // Mouth
        //#endregion

        //#region endscore killed icon (thick skull)
        //private static Point survivor1PlayerKilled2IconFontColor1 = new Point(837, 307);    // Top of the head
        //private static Point survivor1PlayerKilled2IconFontColor2 = new Point(832, 323);    // Left part of the head
        //private static Point survivor1PlayerKilled2IconFontColor3 = new Point(842, 323);    // Left part of the head
        //private static Point survivor1PlayerKilled2IconNotFontColor1 = new Point(830, 318);    // Left eye
        //private static Point survivor1PlayerKilled2IconNotFontColor2 = new Point(842, 318);    // Right eye
        //private static Point survivor1PlayerKilled2IconNotFontColor3 = new Point(837, 324);    // Nose
        //#endregion

        //#endregion

 

        #region parse bools
        public static bool bHasCrossplayIcon = false;
        public static bool bHasReadyGfx = false;       // "Ready" text visible?
        public static bool bHasUnTextGfx = false;       // Has "Un" text visible (beginning of "Unready")
        public static bool bHasReadyButton = false;
        public static bool bHasUnreadyButton = false;
        public static bool bHasSurvivorLookingForMatchText = false;
        public static bool bHasShopIcon = false;
        public static bool bKillerCancelButton = false;
        public static bool bSurvivorCancelButton = false;
        //public static bool bAllPlayersInLobby = false;
        //public static bool bAllPlayersReady = false;
        public static bool bHasAnyMessageOverlay = false;
        public static bool bHasErrorMessage = false;
        public static bool bHasLeaveLobbyConfirmationMessage = false;
        private static bool bEnteredKillerOrSurvivorLobby = false;
        private static bool bInPreLobbyIdlingSurvivorOrKiller = false;
        private static bool bEndgameScoreboard = false;     // Showing Endgame's Scoreboard page
        #endregion

        public static bool hasLoadingBarAlmostFinished()
        {
            //return Gfx.anyBlackLoadingScreenWithDbdLogo.recognize() &&
            //    Gfx.loadingBarAlmostFinished.recognize();

            // For new players there is no big DBD logo in the center of the screent,
            // but help messages instead
            return Gfx.loadingBarAlmostFinished.recognize();
        }

        /// <summary>
        /// Recognize the icon on the scoreboard to the right of the survivor name:
        /// Survivor escaped/killed/DCed icon etc
        /// </summary>
        public static EndgameSurvivorIcon recognizeEndgameScoreboardSurvIcon(PlayerIndex survivorIndex)
        {
            // Can't pass killer index here
            Dbg.assert( (int)survivorIndex <= 3 );

            EndgameSurvivorIcon result = EndgameSurvivorIcon.Error;

            bool bHasEscapeIcon = hasEndgameScoreboardSurvEscapeIcon(survivorIndex);
            bool bHasDcIcon = hasEndgameScoreboardSurvDCIcon(survivorIndex);
            bool bHasThinSkullIcon = hasEndgameScoreboardSurvThinSkullIcon(survivorIndex);
            bool bHasThickSkullIcon = hasEndgameScoreboardSurvThickSkullIcon(survivorIndex);
            bool bHasSurvHeadIcon = hasEndgameScoreboardSurvHeadIcon(survivorIndex);

            if (bHasEscapeIcon)
                result = EndgameSurvivorIcon.Escaped;

            if (bHasDcIcon)
                result = EndgameSurvivorIcon.DCed;

            if (bHasThinSkullIcon)
                result = EndgameSurvivorIcon.KilledSacrificed;

            if (bHasThickSkullIcon)
                result = EndgameSurvivorIcon.KilledBleedoutOrMoried;

            if (bHasSurvHeadIcon)
                result = EndgameSurvivorIcon.SurvIconStillPlaying;

            // Make sure we didn't recognize more than 1 icon
            Dbg.ensureMaxOneBoolIsTrue( new List<bool> { bHasEscapeIcon, bHasDcIcon, bHasThinSkullIcon,
                bHasThickSkullIcon, bHasSurvHeadIcon } );

            return result;
        }

        public static bool hasEndgameScoreboardSurvThickSkullIcon(PlayerIndex survivorIndex)
        {
            // get veretical icon offset
            int verticalIconOffset = Gfx.getEndgameVerticalIconOffsetForPlayer(survivorIndex);

            return Gfx.scoreboardSurvivorMoriedIcon.recognize( 0, verticalIconOffset );

            //int yCoord1 = survivor1PlayerKilled2IconFontColor1.Y + verticalIconOffset;
            //var fontColor1 = getPixelColor(survivor1PlayerKilled2IconFontColor1.X, yCoord1);

            //int yCoord2 = survivor1PlayerKilled2IconFontColor2.Y + verticalIconOffset;
            //var fontColor2 = getPixelColor(survivor1PlayerKilled2IconFontColor2.X, yCoord2);

            //int yCoord3 = survivor1PlayerKilled2IconFontColor3.Y + verticalIconOffset;
            //var fontColor3 = getPixelColor(survivor1PlayerKilled2IconFontColor3.X, yCoord3);

            //int yCoord4 = survivor1PlayerKilled2IconNotFontColor1.Y + verticalIconOffset;
            //var notFontColor1 = getPixelColor(survivor1PlayerKilled2IconNotFontColor1.X, yCoord4);

            //int yCoord5 = survivor1PlayerKilled2IconNotFontColor2.Y + verticalIconOffset;
            //var notFontColor2 = getPixelColor(survivor1PlayerKilled2IconNotFontColor2.X, yCoord5);

            //int yCoord6 = survivor1PlayerKilled2IconNotFontColor3.Y + verticalIconOffset;
            //var notFontColor3 = getPixelColor(survivor1PlayerKilled2IconNotFontColor3.X, yCoord6);

            //bool bIsPixelOfFontColor1 = isPixelOfFontColor(fontColor1);
            //bool bIsPixelOfFontColor2 = isPixelOfFontColor(fontColor2);
            //bool bIsPixelOfFontColor3 = isPixelOfFontColor(fontColor3);
            //bool bIsPixelOfNotFontColor1 = !isPixelOfFontColor(notFontColor1);
            //bool bIsPixelOfNotFontColor2 = !isPixelOfFontColor(notFontColor2);
            //bool bIsPixelOfNotFontColor3 = !isPixelOfFontColor(notFontColor3);

            //return bIsPixelOfFontColor1 && bIsPixelOfFontColor2 && bIsPixelOfFontColor3 &&
            //    bIsPixelOfNotFontColor1 && bIsPixelOfNotFontColor2 && bIsPixelOfNotFontColor3;
        }

        public static bool hasEndgameScoreboardSurvThinSkullIcon(PlayerIndex survivorIndex)
        {
            // get veretical icon offset
            int verticalIconOffset = Gfx.getEndgameVerticalIconOffsetForPlayer(survivorIndex);

            return Gfx.scoreboardSurvivorSacrificedIcon.recognize( 0, verticalIconOffset );

            //int yCoord1 = survivor1PlayerKilledIconFontColor1.Y + verticalIconOffset;
            //var fontColor1 = getPixelColor(survivor1PlayerKilledIconFontColor1.X, yCoord1);

            //int yCoord2 = survivor1PlayerKilledIconFontColor2.Y + verticalIconOffset;
            //var fontColor2 = getPixelColor(survivor1PlayerKilledIconFontColor2.X, yCoord2);

            //int yCoord3 = survivor1PlayerKilledIconFontColor3.Y + verticalIconOffset;
            //var fontColor3 = getPixelColor(survivor1PlayerKilledIconFontColor3.X, yCoord3);

            //int yCoord4 = survivor1PlayerKilledIconNotFontColor1.Y + verticalIconOffset;
            //var notFontColor1 = getPixelColor(survivor1PlayerKilledIconNotFontColor1.X, yCoord4);

            //int yCoord5 = survivor1PlayerKilledIconNotFontColor2.Y + verticalIconOffset;
            //var notFontColor2 = getPixelColor(survivor1PlayerKilledIconNotFontColor2.X, yCoord5);

            //int yCoord6 = survivor1PlayerKilledIconNotFontColor3.Y + verticalIconOffset;
            //var notFontColor3 = getPixelColor(survivor1PlayerKilledIconNotFontColor3.X, yCoord6);

            //bool bIsPixelOfFontColor1 = isPixelOfFontColor(fontColor1);
            //bool bIsPixelOfFontColor2 = isPixelOfFontColor(fontColor2);
            //bool bIsPixelOfFontColor3 = isPixelOfFontColor(fontColor3);
            //bool bIsPixelOfNotFontColor1 = !isPixelOfFontColor(notFontColor1);
            //bool bIsPixelOfNotFontColor2 = !isPixelOfFontColor(notFontColor2);
            //bool bIsPixelOfNotFontColor3 = !isPixelOfFontColor(notFontColor3);

            //return bIsPixelOfFontColor1 && bIsPixelOfFontColor2 && bIsPixelOfFontColor3 &&
            //    bIsPixelOfNotFontColor1 && bIsPixelOfNotFontColor2 && bIsPixelOfNotFontColor3;
        }

        /// <summary>
        /// Survivor player head icon (when someone else DCed)
        /// </summary>
        public static bool hasEndgameScoreboardSurvHeadIcon(PlayerIndex survivorIndex)
        {
            // get veretical icon offset
            int verticalIconOffset = Gfx.getEndgameVerticalIconOffsetForPlayer(survivorIndex);

            return Gfx.scoreboardSurvivorPlayerHeadIcon.recognize( 0, verticalIconOffset );

            //int yCoord1 = survivor1PlayerHeadIconFontColor1.Y + verticalIconOffset;
            //var fontColor1 = getPixelColor(survivor1PlayerHeadIconFontColor1.X, yCoord1);

            //int yCoord2 = survivor1PlayerHeadIconFontColor2.Y + verticalIconOffset;
            //var fontColor2 = getPixelColor(survivor1PlayerHeadIconFontColor2.X, yCoord2);

            //int yCoord3 = survivor1PlayerHeadIconFontColor3.Y + verticalIconOffset;
            //var fontColor3 = getPixelColor(survivor1PlayerHeadIconFontColor3.X, yCoord3);

            //int yCoord4 = survivor1PlayerHeadIconNotFontColor1.Y + verticalIconOffset;
            //var notFontColor1 = getPixelColor(survivor1PlayerHeadIconNotFontColor1.X, yCoord4);

            //int yCoord5 = survivor1PlayerHeadIconNotFontColor2.Y + verticalIconOffset;
            //var notFontColor2 = getPixelColor(survivor1PlayerHeadIconNotFontColor2.X, yCoord5);

            //bool bIsPixelOfFontColor1 = isPixelOfFontColor(fontColor1);
            //bool bIsPixelOfFontColor2 = isPixelOfFontColor(fontColor2);
            //bool bIsPixelOfFontColor3 = isPixelOfFontColor(fontColor3);
            //bool bIsPixelOfNotFontColor1 = !isPixelOfFontColor(notFontColor1);
            //bool bIsPixelOfNotFontColor2 = !isPixelOfFontColor(notFontColor2);

            //return bIsPixelOfFontColor1 && bIsPixelOfFontColor2 && bIsPixelOfFontColor3 &&
            //    bIsPixelOfNotFontColor1 && bIsPixelOfNotFontColor2;
        }

        

        public static bool hasEndgameScoreboardSurvDCIcon(PlayerIndex survivorIndex)
        {
            // get veretical icon offset
            int verticalIconOffset = Gfx.getEndgameVerticalIconOffsetForPlayer(survivorIndex);

            return Gfx.scoreboardSurvivorDCIcon.recognize( 0, verticalIconOffset );

            //int yCoord1 = survivor1DCIconRed1.Y + verticalIconOffset;
            //var redColor1 = getPixelColor(survivor1DCIconRed1.X, yCoord1);

            //int yCoord2 = survivor1DCIconFontColor1.Y + verticalIconOffset;
            //var fontColor1 = getPixelColor(survivor1DCIconFontColor1.X, yCoord2);

            //int yCoord3 = survivor1DCIconFontColor2.Y + verticalIconOffset;
            //var fontColor2 = getPixelColor(survivor1DCIconFontColor2.X, yCoord3);

            //bool bIsRedColor1 = isPixelRed(redColor1);
            //bool bIsPixelOfFontColor1 = isPixelOfFontColor(fontColor1);
            //bool bIsPixelOfFontColor2 = isPixelOfFontColor(fontColor2);

            //return bIsRedColor1 && bIsPixelOfFontColor1 && bIsPixelOfFontColor2;
        }

        public static bool hasEndgameScoreboardSurvEscapeIcon(PlayerIndex survivorIndex)
        {
            // get veretical icon offset
            int verticalIconOffset = Gfx.getEndgameVerticalIconOffsetForPlayer(survivorIndex);

            return Gfx.scoreboardSurvivorEscapeIcon.recognize( 0, verticalIconOffset );

            //int yCoord1 = survivor1EscapeIconFontColor1.Y + verticalIconOffset;
            //var fontColor1 = getPixelColor(survivor1EscapeIconFontColor1.X, yCoord1);

            //int yCoord2 = survivor1EscapeIconNotFontColor1.Y + verticalIconOffset;
            //var notFontColor1 = getPixelColor(survivor1EscapeIconNotFontColor1.X, yCoord2);

            //int yCoord3 = survivor1EscapeIconFontColor2.Y + verticalIconOffset;
            //var fontColor2 = getPixelColor(survivor1EscapeIconFontColor2.X, yCoord3);

            //int yCoord4 = survivor1EscapeIconNotFontColor2.Y + verticalIconOffset;
            //var notFontColor2 = getPixelColor(survivor1EscapeIconNotFontColor2.X, yCoord4);

            //bool bIsPixelOfFontColor1 = isPixelOfFontColor(fontColor1);
            //bool bIsNotPixelOfFontColor1 = !isPixelOfFontColor(notFontColor1);
            //bool bIsPixelOfFontColor2 = isPixelOfFontColor(fontColor2);
            //bool bIsNotPixelOfFontColor = !isPixelOfFontColor(notFontColor2);

            //return bIsPixelOfFontColor1 && bIsNotPixelOfFontColor1 &&
            //    bIsPixelOfFontColor2 && bIsNotPixelOfFontColor;
        }

        /// <summary>
        /// Endgame's "Scoreboard" page is shown
        /// </summary>
        public static bool hasEndgameScoreboard()
        {
            return Gfx.endgameScoreboard.recognize();

            //var redColor1 = getPixelColor(endgameScoreboardRed1.X, endgameScoreboardRed1.Y);
            //var darkAndNotRedColor1 = getPixelColor(endgameScoreboardDarkAndNotRed1.X, endgameScoreboardDarkAndNotRed1.Y);
            //var redColor2 = getPixelColor(endgameScoreboardRed2.X, endgameScoreboardRed2.Y);
            //var darkAndNotRedColor2 = getPixelColor(endgameScoreboardDarkAndNotRed2.X, endgameScoreboardDarkAndNotRed2.Y);

            //var fontColor1 = getPixelColor(endgameScoreboardFontColor1.X, endgameScoreboardFontColor1.Y);
            //var notFontColor1 = getPixelColor(endgameScoreboardNotFontColor.X, endgameScoreboardNotFontColor.Y);
            //var fontColor2 = getPixelColor(endgameScoreboardFontColor2.X, endgameScoreboardFontColor2.Y);
            //var redColor3 = getPixelColor(endgameScoreboardRed3.X, endgameScoreboardRed3.Y);
            //var maxBrightness15 = getPixelColor( endgameScoreboardBlackFadeInLineMaxBrightness15.X,
            //    endgameScoreboardBlackFadeInLineMaxBrightness15.Y );
            //var redColor4 = getPixelColor( endgameScoreboardRedKillerChartLineRedColor.X,
            //    endgameScoreboardRedKillerChartLineRedColor.Y );


            //var bIsPixelRed1 = isPixelRed(redColor1);
            //var bIsPixelDarkAndNotRed1 = isPixelDarkAndNotRed(darkAndNotRedColor1);
            //var bIsPixelRed2 = isPixelRed(redColor2);
            //var bIsPixelDarkAndNotRed2 = isPixelDarkAndNotRed(darkAndNotRedColor2);
            //var bIsPixelOfContColor1 = isPixelOfFontColor(fontColor1);
            //var bIsNotPixelOfContColor1 = !isPixelOfFontColor(notFontColor1);
            //var bIsPixelOfContColor2 = isPixelOfFontColor(fontColor2);
            //var bIsPixelRed3 = isPixelRed(redColor3);
            //var bIsPixelMaxBrightness15 = isPixelMaxBrightless15( maxBrightness15 );
            //var bIsPixelRed4 = isPixelRed( redColor4 );

            //return bIsPixelRed1 && bIsPixelDarkAndNotRed1 &&
            //    bIsPixelRed2 && bIsPixelDarkAndNotRed2 &&
            //    bIsPixelOfContColor1 && bIsNotPixelOfContColor1 &&
            //    bIsPixelOfContColor2 && bIsPixelRed3 && bIsPixelMaxBrightness15 && bIsPixelRed4;
        }

        public static bool hasAnyMessageOverlay()
        {
            return Gfx.overlayDarkBlueMsg.recognize();

            //var veryDarkBlue1Color = getPixelColor(overlayMessageVeryDarkBlue1.X, overlayMessageVeryDarkBlue1.Y);
            //var veryDarkBlue2Color = getPixelColor(overlayMessageVeryDarkBlue2.X, overlayMessageVeryDarkBlue2.Y);
            //var pitchBlack1Color = getPixelColor(overlayMessagePitchBlack1.X, overlayMessagePitchBlack1.Y);
            //var pitchBlack2Color = getPixelColor(overlayMessagePitchBlack2.X, overlayMessagePitchBlack2.Y);

            //return isPixelVeryDarkBlue(veryDarkBlue1Color) && isPixelVeryDarkBlue(veryDarkBlue2Color) &&
            //    isPixelPitchBlack(pitchBlack1Color) && isPixelPitchBlack(pitchBlack2Color);
        }

        public static bool hasErrorMessageOverlay()
        {
            if (!bHasAnyMessageOverlay)
                return false;

            return Gfx.unknownErrorMsg.recognize();

            //var white1Color = getPixelColor(unknownErrorWhite1.X, unknownErrorWhite1.Y);
            //var notWhite1Color = getPixelColor(unknownErrorNotWhite1.X, unknownErrorNotWhite1.Y);
            //var notWhite2Color = getPixelColor(unknownErrorNotWhite2.X, unknownErrorNotWhite2.Y);

            //return isPixelWhite(white1Color) && !isPixelWhite(notWhite1Color) &&
            //    !isPixelWhite(notWhite2Color);
        }

        public static bool hasLeaveLobbyConfirmationMessage()
        {
            if (!bHasAnyMessageOverlay)
                return false;

            return Gfx.leaveLobbyConfirmationMsg.recognize();

            //var white1Color = getPixelColor(leaveLobbyConfirmationMsgWhite1.X, 
            //    leaveLobbyConfirmationMsgWhite1.Y);
            //var notWhite1Color = getPixelColor(leaveLobbyConfirmationMsgNotWhite1.X, 
            //    leaveLobbyConfirmationMsgNotWhite1.Y);
            //var white2Color = getPixelColor(leaveLobbyConfirmationMsgWhite2.X, 
            //    leaveLobbyConfirmationMsgWhite2.Y);

            //return isPixelWhite(white1Color) && !isPixelWhite(notWhite1Color) &&
            //    isPixelWhite(white2Color);
        }

        /// <summary>
        /// All 4 survivors and killer has clicked "Ready" or the game did it
        /// so automatically 5 secs before the start of the game
        /// </summary>
        public static bool hasAllPlayersClickedReady()
        {
            return getPlayerIsReady(PlayerIndex.Killer) &&
                getPlayerIsReady(PlayerIndex.Survivor1) &&
                getPlayerIsReady(PlayerIndex.Survivor2) &&
                getPlayerIsReady(PlayerIndex.Survivor3) &&
                getPlayerIsReady(PlayerIndex.Survivor4);
        }

        /// <summary>
        /// Get number of SWF survivors in the lobby (excludes killer)
        /// </summary>
        public static int getNumOfSwfSurvivorsInPreLobby()
        {
            int numSwfPlayers = 0;

            for(int survivorIndex = (int)PlayerIndex.Survivor1; 
                survivorIndex <= (int)PlayerIndex.Survivor4; survivorIndex++ )
            {
                if ( isPlayerInLobby( ( PlayerIndex )survivorIndex ) )
                    numSwfPlayers++;
            }

            return numSwfPlayers;
        }

        public static bool hasAllPlayersEnteredLobby()
        {
            bool bKiller = isPlayerInLobby( PlayerIndex.Killer );
            bool bSurv1 = isPlayerInLobby( PlayerIndex.Survivor1 );
            bool bSurv2 = isPlayerInLobby( PlayerIndex.Survivor1 );
            bool bSurv3 = isPlayerInLobby( PlayerIndex.Survivor1 );
            bool bSurv4 = isPlayerInLobby( PlayerIndex.Survivor1 );

            return bKiller && bSurv1 && bSurv2 && bSurv3 && bSurv4;
        }

        public static bool hasShopIcon()
        {
            return Gfx.shopIcon.recognize();

            //var pitchBkack1Color = getPixelColor(shopIconPitchBlack1.X, shopIconPitchBlack1.Y);
            //var pitchBkack2Color = getPixelColor(shopIconPitchBlack2.X, shopIconPitchBlack2.Y);
            //var pitchBkack3Color = getPixelColor(shopIconPitchBlack3.X, shopIconPitchBlack3.Y);
            //var pitchBkack4Color = getPixelColor(shopIconPitchBlack4.X, shopIconPitchBlack4.Y);
            //var greyOrWhite1Color = getPixelColor(shopIconGreyOrWhite1.X, shopIconGreyOrWhite1.Y);
            //var greyOrWhite2Color = getPixelColor(shopIconGreyOrWhite2.X, shopIconGreyOrWhite2.Y);

            //return isPixelBlack(pitchBkack1Color) && isPixelBlack(pitchBkack2Color) &&
            //    isPixelBlack(pitchBkack3Color) && isPixelBlack(pitchBkack4Color) &&
            //    isPixelGreyOrWhite(greyOrWhite1Color) & isPixelGreyOrWhite(greyOrWhite2Color);
        }

        ///// <summary>
        ///// SWF PreLobby looking for match (not a custom game, but ranked SWF)
        ///// </summary>
        //public static bool hasSWFLookingForMatch()
        //{
        //    int numSurvivorsInLobby = getNumOfSwfSurvivorsInPreLobby();

        //    // If killer is in pre-lobby, then it is not ranked SWF, but probably a custom game
        //    // Ignore custom games
        //    bool bKillerIsInLobby = isKillerInLobby();

        //    return numSurvivorsInLobby > 1 && !bKillerIsInLobby &&
        //        bHasUnreadyButton && bHasShopIcon && !bHasSurvivorLookingForMatchText;
        //}

        public static bool hasSurvivorLookingForMatchText()
        {
            return Gfx.survivorLookingForMatchText.recognize();

            //var white1Color = getPixelColor(survivorLookingForMatchTextWhite1.X, survivorLookingForMatchTextWhite1.Y);
            //var notWhite1Color = getPixelColor(survivorLookingForMatchTextNonWhite1.X, survivorLookingForMatchTextNonWhite1.Y);
            //var notWhite2Color = getPixelColor(survivorLookingForMatchTextNonWhite2.X, survivorLookingForMatchTextNonWhite2.Y);
            //var white2Color = getPixelColor(survivorLookingForMatchTextWhite2.X, survivorLookingForMatchTextWhite2.Y);
            //var notWhite3Color = getPixelColor(survivorLookingForMatchTextNonWhite3.X, survivorLookingForMatchTextNonWhite3.Y);

            //return isPixelWhite(white1Color) && !isPixelWhite(notWhite1Color) &&
            //    !isPixelWhite(notWhite2Color) && isPixelWhite(white2Color) &&
            //    !isPixelWhite(notWhite3Color);
        }

        public static bool tick()
        {
            bool bGameWasAlreadyRunning = ScreenCapture.haveGameHwnd();

            // Take screenshot from the game
            bool bSuccessFindingDBD = ScreenCapture.makeGameScreenshot();


            // No game - no screenshot - exit
            if ((!bSuccessFindingDBD || !ScreenCapture.haveGameHwnd()) && !ScreenCapture.haveDebugPicture())
                return false;

            // Just found DBD? // Remember the exact time when we found DBD running
            if ( !bGameWasAlreadyRunning && bSuccessFindingDBD )
                timeFoundDBD = DateTime.Now;

            // Skip invalid image that is shown at startup for 1 second
            if ( ScreenCapture.isDBD_1344x714_InvalidStartupImage() )
            {
                Console.WriteLine( "isDBD_1344x714_InvalidStartupImage: true" );

                return false;
            }

            // Current DBD resolution supported?
            if ( !ScreenCapture.isCurrentGameResolutionSupported() )
            {
                Application.Exit();
                System.Environment.Exit( 1 );
                return false;
            }

            // Parse screenshot
            parseGameScreenshot();


            var screenshot = ScreenCapture.getScreenShot();

            Graphics newGraphics = Graphics.FromImage(screenshot);

            return true;
        }

        private static bool isGameResolutionChanged()
        {
            var screenshot = ScreenCapture.getScreenShot();

            // Game resolution changed?
            return (screenshot.Width != curGameScreenWidth || screenshot.Height != curGameScreenHeight);
        }

        ///// <summary>
        ///// Game resolution has changed. Recalc aspect ratio, which is relative
        ///// to our base aspect ration, with which we defined various points for parsing
        ///// </summary>
        //private static void updateGameAspectRatio()
        //{
        //    var screenshot = ScreenCapture.getScreenShot();

        //    // remember new game resolution
        //    curGameScreenWidth = screenshot.Width;
        //    curGameScreenHeight = screenshot.Height;

        //    // calc new aspect ratio, relative to our base game aspect ratio ratio
        //    curHorizontalAspectRatio = (float)baseGameScreenWidth / (float)curGameScreenWidth;
        //    curVerticalAspectRatio = (float)baseGameScreenHeight / (float)curGameScreenHeight;

        //    // Game DPI
        //    gameDPI = 100f * (ScreenCapture.User32.GetDpiForWindow(ScreenCapture.gameHWND) / 96f);
        //    myDPI = Graphics.FromHwnd(IntPtr.Zero).DpiX;

        //    // Windowed mode?
        //    bIsWindowed = ScreenCapture.isDBDInWindowMode();


        //    if (Graphics.FromHwnd(IntPtr.Zero).DpiX != Graphics.FromHwnd(IntPtr.Zero).DpiY)
        //    {
        //        throw new Exception("Wrong DPI: " +  Graphics.FromHwnd(IntPtr.Zero).DpiX +
        //            "/" + Graphics.FromHwnd(IntPtr.Zero).DpiY);
        //    }
        //}

        private static bool hasKillerLookingForMatch()
        {
            return bKillerCancelButton && !bHasSurvivorLookingForMatchText &&
                !bHasShopIcon && !bHasReadyButton && !bHasUnreadyButton;
        }

        private static bool hasSurvivorLookingForMatch()
        {
            return bHasSurvivorLookingForMatchText && !bHasShopIcon &&
                !bHasReadyButton && !bKillerCancelButton;
        }

        /// <summary>
        /// Regognize elements like "Ready" button etc, that are used for more
        /// than one state
        /// </summary>
        private static void recognizeCommonGfxElements()
        {
            bHasCrossplayIcon = hasCrossplayIcon();
            bHasShopIcon = hasShopIcon();
            bHasUnTextGfx = hasUnGfx();
            bHasReadyGfx = hasReadyGfx();

            // Ready button
            bHasReadyButton = hasReadyButton();

            // Unready button
            bHasUnreadyButton = hasUnreadyButton();

            bKillerCancelButton = hasKillerCancelButton();
            bSurvivorCancelButton = hasSurvivorCancelButton();

            bHasSurvivorLookingForMatchText = hasSurvivorLookingForMatchText();
        }

        /// <summary>
        /// Do we have any messages displayed in the game?
        /// </summary>
        private static bool recognizeOverlayMessage()
        {
            // Any message window (error or confirmation)
            bHasAnyMessageOverlay = hasAnyMessageOverlay();


            // Error message
            bHasErrorMessage = hasErrorMessageOverlay();
            StateManager.setHaveErrorMessageDisplayed( bHasErrorMessage );


            // "Leave lobby?" confirmation message
            bHasLeaveLobbyConfirmationMessage = hasLeaveLobbyConfirmationMessage();
            StateManager.setHaveLeaveLobbyConfirmationMessageDisplayed( bHasLeaveLobbyConfirmationMessage );

            // Can't have both messages at the same time
            Dbg.ensureMaxOneBoolIsTrue( 
                new List<bool> { bHasErrorMessage, bHasLeaveLobbyConfirmationMessage} );

            return StateManager.haveAnyMessageDisplayed();
        }

        public static void parseGameScreenshot()
        {
            StateManager.beforeAnyStateUpdates();

            // Leave here and freeze all other logic until this message is gone,
            // otherwise we will change current game state in the logic,
            // because we won't recognize lobby here becayse of this message
            if ( recognizeOverlayMessage () )
                return;



            // Ready button, crossplay icon etc 
            recognizeCommonGfxElements();

            // Survivor or killer looking for a lobby
            bool bHaskillerLookingForMatch = hasKillerLookingForMatch();
            bool bHasSurvivorLookingForMatch = hasSurvivorLookingForMatch();
            if ( bHasSurvivorLookingForMatch || bHaskillerLookingForMatch )
            {
                GameType gameType = recognizeGameType();

                StateManager.setLookingForLobbyState( gameType );
            }

            // Just entered pre-lobby (we are not looking for a lobby and we are not in the lobby)?
            bInPreLobbyIdlingSurvivorOrKiller = bHasReadyButton && bHasShopIcon && !bHasSurvivorLookingForMatchText;
            if ( bInPreLobbyIdlingSurvivorOrKiller )
            {
                GameType gameType = recognizeGameType();

                StateManager.setSurvivorOrKillerIdlingInPreLobbyState( gameType );
            }


            // Entered the actual lobby
            bEnteredKillerOrSurvivorLobby = hasSurvivorOrKillerLobby();
            if( bEnteredKillerOrSurvivorLobby )
                StateManager.setSurvivorOrKillerEnteredLobbyState();

            // Can't have both active at the same time
            Dbg.ensureMaxOneBoolIsTrue( 
                new List<bool> { bInPreLobbyIdlingSurvivorOrKiller, bEnteredKillerOrSurvivorLobby } );



            // Almost finished loading the match (the actual game)
            if(hasLoadingBarAlmostFinished())
                StateManager.setAlmostFinishedLoadingMatchState();

            // Endgame - Scoreboard
            bEndgameScoreboard = hasEndgameScoreboard();
            if( bEndgameScoreboard )
                StateManager.setEndgameScoreboardState();

            // Endgame - observing someone
            //;this doesn't get recognized maybe because test screenshot is not 1080p
            bool bTempEndgameObserving = Gfx.endgameObservingScreen.recognize();
            if ( bTempEndgameObserving )
                StateManager.setEndgameObservingSomeoneState();


            //StateManager.updateStateDetails();

            //// nothing recognized - recalc aspect ratio
            //if (!(bHasCrossplayIcon || bHasUnTextGfx || bHasReadyGfx || bHasReadyButton ||
            //    bHasUnreadyButton || bHasSurvivorLookingForMatchText || bHasShopIcon))
            //{
            //    updateGameAspectRatio();
            //}
        }

        /// <summary>
        /// Solo, SWF or custom game? Depends on what players are in the lobby
        /// </summary>
        private static Stats.GameType recognizeGameType()
        {
            bool bKillerIsInLobby = isKillerInLobby();
            int numSurvivorsInLobby = getNumOfSwfSurvivorsInPreLobby();

            // Solo game: killer
            if ( bKillerIsInLobby && numSurvivorsInLobby == 0 )
                return GameType.Solo;

            // Solo game: survivor
            if ( !bKillerIsInLobby && numSurvivorsInLobby == 1 )
                return GameType.Solo;

            // Ranked SWF
            if ( !bKillerIsInLobby && numSurvivorsInLobby > 1 )
                return GameType.SWF;

            // Unranked custom game
            if ( bKillerIsInLobby && numSurvivorsInLobby >= 1 )
                return GameType.CustomGame;

            return GameType.Error;
        }

        /// <summary>
        /// Player we are playing as (killer or survivor index)
        /// Assumes that we have a screen of the endgame scoreboard
        /// </summary>
        public static PlayerIndex recognizeScoreboardSelectedPlayer(bool bSuppressStateDebugCheck = false)
        {
            if( !bSuppressStateDebugCheck )
                Debug.Assert( StateManager.getState() == State.Endgame_ScoreBoard );

            var verticalOffsetSurvivor1 = Gfx.getEndgameVerticalIconOffsetForPlayer( PlayerIndex.Survivor1 );
            var verticalOffsetSurvivor2 = Gfx.getEndgameVerticalIconOffsetForPlayer( PlayerIndex.Survivor2 );
            var verticalOffsetSurvivor3 = Gfx.getEndgameVerticalIconOffsetForPlayer( PlayerIndex.Survivor3 );
            var verticalOffsetSurvivor4 = Gfx.getEndgameVerticalIconOffsetForPlayer( PlayerIndex.Survivor4 );
            var verticalOffsetKiller = Gfx.getEndgameVerticalIconOffsetForPlayer( PlayerIndex.Killer );

            var gameResolution = ScreenCapture.getScreenshotResolutionAsString();

            var survivor1Selected = Gfx.endgameScoreSelectedPlayer.recognize( gameResolution, 0, verticalOffsetSurvivor1 );
            var survivor2Selected = Gfx.endgameScoreSelectedPlayer.recognize( gameResolution, 0, verticalOffsetSurvivor2 );
            var survivor3Selected = Gfx.endgameScoreSelectedPlayer.recognize( gameResolution, 0, verticalOffsetSurvivor3 );
            var survivor4Selected = Gfx.endgameScoreSelectedPlayer.recognize( gameResolution, 0, verticalOffsetSurvivor4 );
            var killerSelected = Gfx.endgameScoreSelectedPlayer.recognize( gameResolution, 0, verticalOffsetKiller );

            // Debug
            Dbg.ensureExactlyOneBoolIsTrue( new List<bool> { survivor1Selected, survivor2Selected,
                survivor3Selected, survivor4Selected, killerSelected } );

            if ( survivor1Selected )
                return PlayerIndex.Survivor1;
            else if ( survivor2Selected )
                return PlayerIndex.Survivor2;
            else if ( survivor3Selected )
                return PlayerIndex.Survivor3;
            else if ( survivor4Selected )
                return PlayerIndex.Survivor4;
            else
            {
                Debug.Assert( killerSelected );

                return PlayerIndex.Killer;
            }
        }

        /// <summary>
        /// Are we currently in the lobby? (not pre-lobby)
        /// Killer or survivor
        /// </summary>
        public static bool hasSurvivorOrKillerLobby()
        {
            // Make sure someone in lobby. This check is needed, otherwise when we exit the
            // prelobby to the main screen, during this process it can be confused with
            // "we are in the lobby" state, because some gfx gets removed from the screen
            bool bSomeoneIsInLobby = ScreenParser.isAnyPlayerInLobbyOrPrelobby();

            return (bHasReadyButton || bHasUnreadyButton)
                && !bHasShopIcon && !bHasSurvivorLookingForMatchText && bSomeoneIsInLobby;
        }


        /// <summary>
        /// Has "Un", as beginning of "Unready". For both killers and survivors
        /// </summary>
        public static bool hasUnGfx()
        {
            int horizontalOffset = getHorizontalOffsetForCrossplayIcon();

            return Gfx.unreadyIcon.recognize( horizontalOffset, 0 );

            //int horizontalOffset = getHorizontalOffsetForCrossplayIcon();

            //var notGrey1Color = getPixelColor(unreadyIconNotGrey1.X + horizontalOffset,
            //    unreadyIconNotGrey1.Y);
            //var fontColor1 = getPixelColor(unreadyIconGreyOrWhite1.X + horizontalOffset,
            //    unreadyIconGreyOrWhite1.Y);
            //var notGrey2Color = getPixelColor(unreadyIconNotGrey2.X + horizontalOffset,
            //    unreadyIconNotGrey2.Y);
            //var fontColor2 = getPixelColor(unreadyIconGreyOrWhite2.X + horizontalOffset,
            //    unreadyIconGreyOrWhite2.Y);
            //var notGrey3Color = getPixelColor(unreadyIconNotGrey3.X + horizontalOffset,
            //    unreadyIconNotGrey3.Y);

            //return !isPixelGrey(notGrey1Color) && isPixelGreyOrWhite(fontColor1) &&
            //    !isPixelGrey(notGrey2Color) && isPixelGreyOrWhite(fontColor2) &&
            //    !isPixelGrey(notGrey3Color);
        }

        public static bool hasUnreadyButton()
        {
            return bHasUnTextGfx && bHasReadyGfx;
        }

        public static bool hasReadyButton()
        {
            return bHasReadyGfx && !bHasUnTextGfx;
        }

        /// <summary>
        /// "Ready" text, can be part of "Unready". For both killers and survivors
        /// </summary>
        public static bool hasReadyGfx()
        {
            int horizontalOffset = getHorizontalOffsetForCrossplayIcon();
            return Gfx.readyIcon.recognize( horizontalOffset, 0 );

            //int horizontalOffset = getHorizontalOffsetForCrossplayIcon();

            //var redColor = getPixelColor(readyIconRedVerticalLine.X + horizontalOffset,
            //    readyIconRedVerticalLine.Y);
            //var greyOrWhiteColor = getPixelColor(readyIconGreyOrWhite.X + horizontalOffset,
            //    readyIconGreyOrWhite.Y);
            //var notGreyColor = getPixelColor(readyIconNotGrey1.X + horizontalOffset,
            //    readyIconNotGrey1.Y);
            //var greyOrWhite2Color = getPixelColor(readyIconGreyOrWhile2.X + horizontalOffset,
            //    readyIconGreyOrWhile2.Y);
            //var notGrey2Color = getPixelColor(readyIconNotGrey2.X + horizontalOffset,
            //    readyIconNotGrey2.Y);

            //bool red = isPixelRed(redColor);
            //bool greyOrWhite1 = isPixelGreyOrWhite(greyOrWhiteColor);
            //bool grey1 = isPixelGrey(notGreyColor);
            //bool greyOrWhite2 = isPixelGreyOrWhite(greyOrWhite2Color);
            //bool grey2 = isPixelGrey(notGrey2Color);





            


            //bool result = red && greyOrWhite1 && !grey1 && greyOrWhite2 && !grey2;

            //return result;
        }

        /// <summary>
        /// If "Cancel" text is displayed when searching for a lobby as killer
        /// </summary>
        public static bool hasSurvivorCancelButton()
        {
            int horizontalOffset = getHorizontalOffsetForCrossplayIcon();

            return Gfx.survivorCancelIcon.recognize( horizontalOffset, 0 );

            //var redColor = getPixelColor(survivorCancelIconRedVerticalLine.X + horizontalOffset,
            //    survivorCancelIconRedVerticalLine.Y);
            //var notGreyColor = getPixelColor(survivorCancelIconNotGrey1.X + horizontalOffset,
            //    survivorCancelIconNotGrey1.Y);
            //var greyColor = getPixelColor(survivorCancelIconGreyOrWhite1.X + horizontalOffset,
            //    survivorCancelIconGreyOrWhite1.Y);
            //var notGrey2Color = getPixelColor(survivorCancelIconNotGrey2.X + horizontalOffset,
            //    survivorCancelIconNotGrey2.Y);
            //var grey2Color = getPixelColor(survivorCancelIconGreyOrWhite2.X + horizontalOffset,
            //    survivorCancelIconGreyOrWhite2.Y);

            //return isPixelRed(redColor) && !isPixelGrey(notGreyColor) &&
            //    isPixelGreyOrWhite(greyColor) && !isPixelGrey(notGrey2Color) &&
            //    isPixelGreyOrWhite(grey2Color);
        }

        /// <summary>
        /// If "Cancel" text is displayed when searching for a lobby as killer
        /// </summary>
        public static bool hasKillerCancelButton()
        {
            return Gfx.killerCancelIcon.recognize();

            //var redColor = getPixelColor(killerCancelIconRedVerticalLine.X, killerCancelIconRedVerticalLine.Y);
            //var notGreyColor = getPixelColor(killerCancelIconNotGrey1.X, killerCancelIconNotGrey1.Y);
            //var greyColor = getPixelColor(killerCancelIconGreyOrWhite1.X, killerCancelIconGreyOrWhite1.Y);
            //var notGrey2Color = getPixelColor(killerCancelIconNotGrey2.X, killerCancelIconNotGrey2.Y);
            //var grey2Color = getPixelColor(killerCancelIconGreyOrWhite2.X, killerCancelIconGreyOrWhite2.Y);

            //return isPixelRed(redColor) && !isPixelGrey(notGreyColor) &&
            //    isPixelGreyOrWhite(greyColor) && !isPixelGrey(notGrey2Color) &&
            //    isPixelGreyOrWhite(grey2Color);
        }





        // Ready/Cancel buttons are moved to the left, when the "crossplay" icon is present
        private static int getHorizontalOffsetForCrossplayIcon()
        {
            string resolution = ScreenCapture.getScreenshotResolutionAsString();

            int horizontalOffset = xcoord(Gfx.crossplayIconWidth[ resolution], false);

            if (bHasCrossplayIcon)
                horizontalOffset = 0;

            return horizontalOffset;
        }



        public enum LobbyState
        {
            PreLobbyWithCrossplayIcon,  // killer and survivor
            PreLobbyNoCrossplayIcon
        }

        /// <summary>
        /// Survivor1 must be 0, some fuctions expect that
        /// </summary>
        public enum PlayerIndex
        {
            Survivor1 = 0, Survivor2 = 1, Survivor3 = 2, Survivor4 = 3, Killer = 4, Error
        }

        public enum PlayerLobbyStatus
        {
            NotPresent, Unready, Ready, Error
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        public static void initialize()
        {
            //// 1080p
            //playerCheckmarkX[ Gfx._1080p ] = new Dictionary<PlayerIndex, int>();
            //playerCheckmarkY[ Gfx._1080p ] = new Dictionary<PlayerIndex, int>();

            //playerCheckmarkX[ Gfx._1080p ][ PlayerIndex.Killer] = 1794;
            //playerCheckmarkY[ Gfx._1080p ][ PlayerIndex.Killer] = 938;

            //playerCheckmarkX[ Gfx._1080p ][ PlayerIndex.Survivor1] = 1773;
            //playerCheckmarkY[ Gfx._1080p ][ PlayerIndex.Survivor1] = 921;

            //playerCheckmarkX[ Gfx._1080p ][ PlayerIndex.Survivor2] = 1786;
            //playerCheckmarkY[ Gfx._1080p ][ PlayerIndex.Survivor2] = playerCheckmarkY[ Gfx._1080p ][ PlayerIndex.Survivor1 ];

            //playerCheckmarkX[ Gfx._1080p ][ PlayerIndex.Survivor3] = 1800;
            //playerCheckmarkY[ Gfx._1080p ][ PlayerIndex.Survivor3] = playerCheckmarkY[ Gfx._1080p ][ PlayerIndex.Survivor1 ];

            //playerCheckmarkX[ Gfx._1080p ][ PlayerIndex.Survivor4] = 1813;
            //playerCheckmarkY[ Gfx._1080p ][ PlayerIndex.Survivor4] = playerCheckmarkY[ Gfx._1080p ][ PlayerIndex.Survivor1 ];


            //// 720p
            //playerCheckmarkX[ Gfx._720p ] = new Dictionary<PlayerIndex, int>();
            //playerCheckmarkY[ Gfx._720p ] = new Dictionary<PlayerIndex, int>();

            //playerCheckmarkX[ Gfx._720p ][ PlayerIndex.Killer ] = 1174;
            //playerCheckmarkY[ Gfx._720p ][ PlayerIndex.Killer ] = 619;

            //playerCheckmarkX[ Gfx._720p ][ PlayerIndex.Survivor1 ] = 1182;
            //playerCheckmarkY[ Gfx._720p ][ PlayerIndex.Survivor1 ] = 614;

            //playerCheckmarkX[ Gfx._720p ][ PlayerIndex.Survivor2 ] = 1191;
            //playerCheckmarkY[ Gfx._720p ][ PlayerIndex.Survivor2 ] = playerCheckmarkY[ Gfx._720p ][ PlayerIndex.Survivor1 ];

            //playerCheckmarkX[ Gfx._720p ][ PlayerIndex.Survivor3 ] = 1200;
            //playerCheckmarkY[ Gfx._720p ][ PlayerIndex.Survivor3 ] = playerCheckmarkY[ Gfx._720p ][ PlayerIndex.Survivor1 ];

            //playerCheckmarkX[ Gfx._720p ][ PlayerIndex.Survivor4 ] = 1208;
            //playerCheckmarkY[ Gfx._720p ][ PlayerIndex.Survivor4 ] = playerCheckmarkY[ Gfx._720p ][ PlayerIndex.Survivor1 ];
        }

        public static bool hasCrossplayIcon()
        {

            return Gfx.crossPlayIcon.recognize();

            //var redColor = getPixelColor(crossplayIconRed.X, crossplayIconRed.Y);
            //var greyOrWhiteColor = getPixelColor(crossplayIconGreyOrWhite.X, crossplayIconGreyOrWhite.Y);
            //var blackColor = getPixelColor(crossplayIconBlack.X, crossplayIconBlack.Y);

            //bool red = isPixelRed(redColor);
            //bool greyOrWhite = isPixelGreyOrWhite(greyOrWhiteColor);
            //bool black = isPixelBlack(blackColor);

            //bool result = red && greyOrWhite && black;

            //return result;
        }

        private static bool RGBColorWithinDifference(Color c, int maxDifference)
        {
            if (c.R + maxDifference < c.G || c.R + maxDifference < c.B)
                return false;

            if (c.G + maxDifference < c.R || c.G + maxDifference < c.B)
                return false;

            if (c.B + maxDifference < c.R || c.B + maxDifference < c.G)
                return false;

            return true;
        }

        public static bool isPixelRed(Color c)
        {
            return (c.R > 65 && c.R > (c.G * 2.7) && (c.R > c.B * 2.7));
        }

        public static bool isPixelVeryDarkBlue(Color c)
        {
            bool bIsBlue = c.B > c.G && c.B > c.R;

            return bIsBlue && isPixelVeryDark(c);
        }

        /// <summary>
        /// RGB < 50
        /// </summary>
        private static bool isPixelVeryDark(Color c)
        {
            return c.R < 50 && c.G < 50 && c.B < 50;
        }

        /// <summary>
        /// RGB < 100
        /// </summary>
        private static bool isPixelDark(Color c)
        {
            return c.R < 100 && c.G < 100 && c.B < 100;
        }

        public static bool isPixelDarkAndNotRed(Color c)
        {
            return !isPixelRed(c) && isPixelDark(c);
        }

        /// <summary>
        /// RGB 100-150
        /// </summary>
        public static bool isPixelMediumBrightness( Color c )
        {
            return ( c.R >= 100 && c.R < 150 ) &&
                ( c.G >= 100 && c.G < 150 ) &&
                ( c.B >= 100 && c.B < 150 );
        }

        /// <summary>
        /// RGB 150-200
        /// </summary>
        public static bool isPixelBright( Color c )
        {
            return ( c.R >= 150 && c.R < 200 ) &&
                ( c.G >= 150 && c.G < 200 ) &&
                ( c.B >= 150 && c.B < 200 );
        }

        /// <summary>
        /// RGB > 100.
        /// </summary>
        public static bool isPixelBrighterThanDark( Color c )
        {
            return c.R >= 100  && c.G >= 100 && c.B >= 100;
        }

        /// <summary>
        /// Fix horizontal aspect ratio
        /// </summary>
        private static int xcoord(int x, bool addBorderOffset = true)
        {
            //var result = (int)((float)x / (float)curHorizontalAspectRatio);
            //result = (int) ((float)x / (float)1.51f);


            //Debug.Assert(result >= 0);

            return x;
        }

        /// <summary>
        /// Fix vertival aspect ratio
        /// </summary>
        private static int ycoord(int y)
        {
            //var result = (int)((float)y / (float)curVerticalAspectRatio);

            return y;  // (int)(result / (gameDPI / myDPI));
        }

        /// <summary>
        /// Grey shades of 100-240
        /// </summary>
        public static bool isPixelGrey(Color c)
        {
            // Dark or white is not grey
            if (isPixelDark(c) || isPixelWhite(c))
                return false;

            return isRgbValuesCloseForGreyColor(c);
        }

        /// <summary>
        /// Grey shades of 50-100
        /// </summary>
        public static bool isPixelDarkGrey( Color c )
        {
            // Dark or white is not grey (not < 50, not 100-240 or 240-255)
            if ( isPixelVeryDark( c ) || isPixelGreyOrWhite( c ) )
                return false;

            return isRgbValuesCloseForGreyColor( c );
        }

        /// <summary>
        /// Grey shades of 25-50
        /// </summary>
        public static bool isPixelVeryDarkGrey( Color c )
        {
            // Not <= 25, not 50-255
            if ( isPixelBlack( c ) || isPixelDarkGrey( c ) )
                return false;

            return isRgbValuesCloseForGreyColor( c );
        }

        /// <summary>
        /// Grey shades of 25-50 or 50-100
        /// </summary>
        public static bool isPixelDarkOrVeryDarkGrey( Color c )
        {
            return isPixelDarkGrey( c ) || isPixelVeryDarkGrey( c );
        }

        private static bool isRgbValuesCloseForGreyColor(Color c)
        {
            return Math.Abs(c.R - c.G) < 25 &&
                Math.Abs(c.R - c.B) < 25 &&
                Math.Abs(c.G - c.B) < 25;
        }

        /// <summary>
        /// RGB <= 25
        /// </summary>
        public static bool isPixelBlack(Color c)
        {
            return c.R <= 25 && c.G <= 25 && c.B <= 25 &&
                isRgbValuesCloseForGreyColor(c);
        }

        /// <summary>
        /// Very dark black
        /// </summary>
        public static bool isPixelPitchBlack(Color c)
        {
            return c.R <= 5 && c.G <= 5 && c.B <= 5 &&
                isRgbValuesCloseForGreyColor(c);
        }

        /// <summary>
        /// R, G and B is max 15 or less
        /// </summary>
        public static bool isPixelMaxBrightless15( Color c )
        {
            return c.R <= 15 && c.G <= 15 && c.B <= 15;
        }

        /// <summary>
        /// RGB > 240
        /// </summary>
        public static bool isPixelWhite(Color c)
        {
            return c.R > 240 && c.G > 240 && c.B > 240 &&
                isRgbValuesCloseForGreyColor(c);
        }

        /// <summary>
        /// Either grey color or white when mouse is over it
        /// </summary>
        public static bool isPixelOfFontColor(Color c)
        {
            return isPixelGreyOrWhite(c);
        }

        public static bool isPixelGreyOrWhite(Color c)
        {
            // 100-240 or 240-255
            return isPixelGrey(c) || isPixelWhite(c);
        }

        public static bool isPixelOfDbdLogoColor( Color c )
        {
            return isPixelGreyDarkGreyOrWhite( c );
        }

        /// <summary>
        /// Dark grey, grey or white (just not very dark)
        /// </summary>
        public static bool isPixelGreyDarkGreyOrWhite( Color c )
        {
            // 50-100 or 100-240 or 240-255
            return isPixelDarkGrey( c ) || isPixelGrey( c ) || isPixelWhite( c );
        }

        private static Color getPixelColor(int x, int y)
        {
            var newX = xcoord(x);
            var newY = ycoord(y);

            var screen = ScreenCapture.getScreenShot();

            return screen.GetPixel(newX, newY);
        }

        public static bool getPlayerIsReady(PlayerIndex playerIndex)
        {

            //string resolution = ScreenCapture.getScreenshotResolutionAsString();

            //var color = getPixelColor( playerCheckmarkX[ resolution ][ playerIndex ],
            //    playerCheckmarkY[ resolution ][ playerIndex ] );

            //return isPixelRed(color);

            Dbg.assertPlayerIndexIsInRangeAndNotInvalid( playerIndex );


            return Gfx.playerReadyIcon[ ( int )playerIndex ].recognize( PointColor.Red );
        }

        public static bool getPlayerIsUnready(PlayerIndex playerIndex)
        {
            //string resolution = ScreenCapture.getScreenshotResolutionAsString();

            //var color = getPixelColor( playerCheckmarkX[ resolution ][ playerIndex ],
            //    playerCheckmarkY[ resolution ][ playerIndex ] );

            //return isPixelDarkGrey( color);

            Dbg.assertPlayerIndexIsInRangeAndNotInvalid( playerIndex );

            return Gfx.playerReadyIcon[ ( int )playerIndex ].recognize( PointColor.DarkGrey );
        }

        /// <summary>
        /// Returns true if survivor not in the lobby
        /// </summary>
        public static bool getPlayerNotPresent(PlayerIndex playerIndex)
        {
            //string resolution = ScreenCapture.getScreenshotResolutionAsString();

            //var color = getPixelColor( playerCheckmarkX[ resolution ][ playerIndex ],
            //    playerCheckmarkY[ resolution ][ playerIndex ] );

            //return isPixelBlack(color);

            Dbg.assertPlayerIndexIsInRangeAndNotInvalid( playerIndex );

            return Gfx.playerReadyIcon[ ( int )playerIndex ].recognize( PointColor.Black );
        }

        public static PlayerLobbyStatus getPlayerLobbyStatus(PlayerIndex playerIndex)
        {
            if (getPlayerNotPresent(playerIndex))
                return PlayerLobbyStatus.NotPresent;
            else if (getPlayerIsUnready(playerIndex))
                return PlayerLobbyStatus.Unready;
            else if (getPlayerIsReady(playerIndex))
                return PlayerLobbyStatus.Ready;
            else
                return PlayerLobbyStatus.Error;
        }

        /// <summary>
        /// Make sure at least one player is marked as present in lobby or prelobby
        /// </summary>
        public static bool isAnyPlayerInLobbyOrPrelobby()
        {
            return isPlayerInLobby( PlayerIndex.Killer ) ||
                isPlayerInLobby( PlayerIndex.Survivor1 ) || isPlayerInLobby( PlayerIndex.Survivor2 ) ||
                isPlayerInLobby( PlayerIndex.Survivor3 ) || isPlayerInLobby( PlayerIndex.Survivor4 );
        }

        public static bool isPlayerInLobby(PlayerIndex playerIndex)
        {
            return getPlayerIsUnready(playerIndex) || getPlayerIsReady(playerIndex);
        }

        public static bool isKillerInLobby()
        {
            return getPlayerIsUnready( PlayerIndex.Killer ) || getPlayerIsReady( PlayerIndex.Killer );
        }
    }
}
