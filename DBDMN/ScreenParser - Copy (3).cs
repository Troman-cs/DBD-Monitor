using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DBDMN.GamePoint;

namespace DBDMN
{
    public static class ScreenParser
    {
        public static List<Gfx> allGfxElements = new List<Gfx>();

        public enum EndgameSurvivorIcon
        {
            Escaped, DCed, KilledThinSkull, KilledThickSkull, SurvIconSomeoneElseDCed, Error
        }

        // Screen width/height for the points defined here
        private static int baseGameScreenWidth = 1920;
        private static int baseGameScreenHeight = 1080;

        // Current resolution of the game that we are dealing with
        private static int curGameScreenWidth = int.MinValue;     // recalc aspect ratio at the beginning
        private static int curGameScreenHeight = int.MinValue;
        private static float gameDPI = 0f;
        private static float myDPI = 0f;
        private static bool bIsWindowed = false;        // DBD is windowed mode?

        // Aspect ratio relative to our base resolution
        private static float curHorizontalAspectRatio = 0f;
        private static float curVerticalAspectRatio = 0f;

        private static int[] playerCheckmarkX = new int[5];
        private static int[] playerCheckmarkY = new int[5];

        /* pitch-black: RGB <= 5
         * black: RGB <= 25
         * very dark: RGB < 50
         * dark: RGB < 100
         * white: RGB > 240
         */
        private static Point crossplayIconRed = new Point( 1817, 1016 );
        private static Point crossplayIconGreyOrWhite = new Point( 1804, 1004 );
        private static Point crossplayIconBlack = new Point( 1800, 1000);      // Inside the globe icon

        /// <summary>
        /// Width of the crossplay icon. It moved "Ready" button by this amount to the left
        /// </summary>
        private static int crossplayIconWidth = 1827 - 1781;    // 46

        #region Ready button killer/survivor
        /// <summary>
        /// Lobby "Ready" button.
        /// For both killer and survivor. All coords are with "crossplay" icon shown.
        /// </summary>
        private static Point readyIconRedVerticalLine = new Point( 1775, 1008);
        private static Point readyIconGreyOrWhite = new Point(1745, 1007);    // Letter "Y", white only when mouseover
        private static Point readyIconNotGrey1 = new Point(1745, 998);       // Inside letter "Y"
        private static Point readyIconGreyOrWhile2 = new Point(1660, 998);       // Letter "R"
        private static Point readyIconNotGrey2 = new Point(1665, 1003);       // Indise letter "R"
        #endregion

        #region Unready button for killer/survivor
        // "UN" from "Unready". For both killer and survivor. All coords are with "crossplay" icon shown.
        private static Point unreadyIconNotGrey1 = new Point(1656, 1007);       // Between "N" and "R"
        private static Point unreadyIconGreyOrWhite1 = new Point(1639, 1001);    // Letter "N"
        private static Point unreadyIconNotGrey2 = new Point(1634, 1007);       // Between "U" and "N"
        private static Point unreadyIconGreyOrWhite2 = new Point(1617, 1007);    // Letter "U"
        private static Point unreadyIconNotGrey3 = new Point(1623, 1007);       // Inside "U"
        #endregion

        #region "Cancel" text when searching for lobby as killer
        private static Point killerCancelIconRedVerticalLine = new Point(1827, 940);
        private static Point killerCancelIconNotGrey1 = new Point(1812, 940);        // To the right of "L" char
        private static Point killerCancelIconGreyOrWhite1 = new Point(1808, 940);           // "L" character
        private static Point killerCancelIconNotGrey2 = new Point(1751, 940);        // Inside "C" character
        private static Point killerCancelIconGreyOrWhite2 = new Point(1746, 940);        // "C" character
        #endregion

        #region "Cancel" text when searching for lobby as survivor. This is with "Crossplay" icon shown
        private static Point survivorCancelIconRedVerticalLine = new Point(1774, 1007);
        private static Point survivorCancelIconNotGrey1 = new Point(1750, 1007);        // To the right of "L" char
        private static Point survivorCancelIconGreyOrWhite1 = new Point(1743, 1007);           // "L" character
        private static Point survivorCancelIconNotGrey2 = new Point(1648, 1007);        // Inside "C" character
        private static Point survivorCancelIconGreyOrWhite2 = new Point(1640, 1007);        // "C" character
        #endregion

        #region "looking for match..." text for survivors when searching for lobby
        private static Point survivorLookingForMatchTextWhite1 = new Point(1710, 927);   // "h" char
        private static Point survivorLookingForMatchTextNonWhite1 = new Point(1659, 932);   // Between "for" and "match"
        private static Point survivorLookingForMatchTextNonWhite2 = new Point(1630, 932);   // Between "looking" and "for"
        private static Point survivorLookingForMatchTextWhite2 = new Point(1593, 931);   // "k" char
        private static Point survivorLookingForMatchTextNonWhite3 = new Point(1563, 930);   // Inside "L"
        #endregion

        #region shopIcon
        private static Point shopIconPitchBlack1 = new Point(88, 794);
        private static Point shopIconPitchBlack2 = new Point(184, 795);
        private static Point shopIconPitchBlack3 = new Point(180, 880);
        private static Point shopIconPitchBlack4 = new Point(88, 880);
        private static Point shopIconGreyOrWhite1 = new Point(136, 844);
        private static Point shopIconGreyOrWhite2 = new Point(155, 834);
        #endregion

        #region overlay messages
        private static Point overlayMessageVeryDarkBlue1 = new Point(85, 465);     // Left
        private static Point overlayMessageVeryDarkBlue2 = new Point(1820, 465);   // Right
        private static Point overlayMessagePitchBlack1 = new Point(85, 585);   // Left
        private static Point overlayMessagePitchBlack2 = new Point(1820, 585);   // Right

        // "An unknown error occured" on the main meny or lobby
        private static Point unknownErrorWhite1 = new Point(921, 466);   // "E" char in "Error" msg caption
        private static Point unknownErrorNotWhite1 = new Point(921, 471);   // Inside "E" char
        private static Point unknownErrorNotWhite2 = new Point(903, 466);   // Before "E" char

        private static Point leaveLobbyConfirmationMsgWhite1 = new Point(856, 464);   // First "L" char
        private static Point leaveLobbyConfirmationMsgNotWhite1 = new Point(865, 464);   // Inside "L" char
        private static Point leaveLobbyConfirmationMsgWhite2 = new Point(1070, 467);   // "Y" char
        #endregion

        #region Endgame - "Scoreboard"
        private static Point endgameScoreboardRed1 = 
            new Point(104, 150);   // "S" char in "scoreboard", center
        private static Point endgameScoreboardDarkAndNotRed1 = 
            new Point(104, 156);   // Inside "S", max ~70 brightness
        private static Point endgameScoreboardRed2 = 
            new Point(318, 150);       // "D" char in "scoreboard"
        private static Point endgameScoreboardDarkAndNotRed2 = 
            new Point(326, 150);   // Inside "D" char, max ~70 brightness
        private static Point endgameScoreboardFontColor1 = 
            new Point(1802, 999);   // "E" char in "Continue"
        private static Point endgameScoreboardNotFontColor =
            new Point(1802, 1002);   // Inside "E" char in "Continue"
        private static Point endgameScoreboardFontColor2 =
            new Point(1802, 1006);   // "E" char in "Continue"
        private static Point endgameScoreboardRed3 =
            new Point(1821, 1005);   // Vertical red line to the right of "Continue"



        #region survivor edgame escape icon
        private static Point survivor1EscapeIconFontColor1 =
            new Point(834, 310);    // Inside the head
        private static Point survivor1EscapeIconNotFontColor1 =
            new Point(841, 308);    // To the right of the head
        private static Point survivor1EscapeIconFontColor2 =
            new Point(830, 322);    // Inside the body
        private static Point survivor1EscapeIconNotFontColor2 =
            new Point(840, 332);    // To the right of the body
        #endregion

        #region endscore survivor endgame DC icon
        private static Point survivor1DCIconRed1 = new Point(851, 327);    // Red X mark in DC icon
        private static Point survivor1DCIconFontColor1 = new Point(834, 320);    // Left part of the plug
        private static Point survivor1DCIconFontColor2 = new Point(849, 317);    // Right part of the plug
        #endregion

        #region endscore player head icon (when someone else DCed)
        private static Point survivor1PlayerHeadIconFontColor1 = new Point(836, 306);    // Top of the head
        private static Point survivor1PlayerHeadIconFontColor2 = new Point(822, 331);    // Bottom left
        private static Point survivor1PlayerHeadIconFontColor3 = new Point(850, 333);    // Bottom right
        private static Point survivor1PlayerHeadIconNotFontColor1 = new Point(830, 322);    // To the left of the neck
        private static Point survivor1PlayerHeadIconNotFontColor2 = new Point(843, 321);    // To the right of the neck
        #endregion

        #region endscore killed icon (thin skull)
        private static Point survivor1PlayerKilledIconFontColor1 = new Point(837, 307);    // Top of the head
        private static Point survivor1PlayerKilledIconFontColor2 = new Point(832, 320);    // Left part of the head
        private static Point survivor1PlayerKilledIconFontColor3 = new Point(842, 321);    // Left part of the head
        private static Point survivor1PlayerKilledIconNotFontColor1 = new Point(832, 314);    // Left eye
        private static Point survivor1PlayerKilledIconNotFontColor2 = new Point(843, 315);    // Right eye
        private static Point survivor1PlayerKilledIconNotFontColor3 = new Point(838, 329);    // Mouth
        #endregion

        #region endscore killed icon (thick skull)
        private static Point survivor1PlayerKilled2IconFontColor1 = new Point(837, 307);    // Top of the head
        private static Point survivor1PlayerKilled2IconFontColor2 = new Point(832, 323);    // Left part of the head
        private static Point survivor1PlayerKilled2IconFontColor3 = new Point(842, 323);    // Left part of the head
        private static Point survivor1PlayerKilled2IconNotFontColor1 = new Point(830, 318);    // Left eye
        private static Point survivor1PlayerKilled2IconNotFontColor2 = new Point(842, 318);    // Right eye
        private static Point survivor1PlayerKilled2IconNotFontColor3 = new Point(837, 324);    // Nose
        #endregion

        #endregion

 

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
        public static bool bAllPlayersInLobby = false;
        public static bool bAllPlayersReady = false;
        public static bool bHasAnyMessageOverlay = false;
        public static bool bHasErrorMessage = false;
        public static bool bHasLeaveLobbyConfirmationMessage = false;
        private static bool bEnteredLobby = false;
        private static bool bInPreLobbyIdling = false;
        private static bool bEndgameScoreboard = false;     // Showing Endgame's Scoreboard page
        #endregion



        public static EndgameSurvivorIcon getEndgameScoreboardSurvIcon(PlayerIndex survivorIndex)
        {
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
                result = EndgameSurvivorIcon.KilledThinSkull;

            if (bHasThickSkullIcon)
                result = EndgameSurvivorIcon.KilledThickSkull;

            if (bHasSurvHeadIcon)
                result = EndgameSurvivorIcon.SurvIconSomeoneElseDCed;

            // Make sure we didn't recognize more than 1 icon
            Utils.ensureMaxOneBoolIsTrue( new List<bool> { bHasEscapeIcon, bHasDcIcon, bHasThinSkullIcon,
                bHasThickSkullIcon, bHasSurvHeadIcon } );

            return result;
        }

        public static bool hasEndgameScoreboardSurvThickSkullIcon(PlayerIndex survivorIndex)
        {
            // get veretical icon offset
            int verticalIconOffset = getEndgameVerticalIconOffsetForPlayer(survivorIndex);

            int yCoord1 = survivor1PlayerKilled2IconFontColor1.Y + verticalIconOffset;
            var fontColor1 = getPixelColor(survivor1PlayerKilled2IconFontColor1.X, yCoord1);

            int yCoord2 = survivor1PlayerKilled2IconFontColor2.Y + verticalIconOffset;
            var fontColor2 = getPixelColor(survivor1PlayerKilled2IconFontColor2.X, yCoord2);

            int yCoord3 = survivor1PlayerKilled2IconFontColor3.Y + verticalIconOffset;
            var fontColor3 = getPixelColor(survivor1PlayerKilled2IconFontColor3.X, yCoord3);

            int yCoord4 = survivor1PlayerKilled2IconNotFontColor1.Y + verticalIconOffset;
            var notFontColor1 = getPixelColor(survivor1PlayerKilled2IconNotFontColor1.X, yCoord4);

            int yCoord5 = survivor1PlayerKilled2IconNotFontColor2.Y + verticalIconOffset;
            var notFontColor2 = getPixelColor(survivor1PlayerKilled2IconNotFontColor2.X, yCoord5);

            int yCoord6 = survivor1PlayerKilled2IconNotFontColor3.Y + verticalIconOffset;
            var notFontColor3 = getPixelColor(survivor1PlayerKilled2IconNotFontColor3.X, yCoord6);

            bool bIsPixelOfFontColor1 = isPixelOfFontColor(fontColor1);
            bool bIsPixelOfFontColor2 = isPixelOfFontColor(fontColor2);
            bool bIsPixelOfFontColor3 = isPixelOfFontColor(fontColor3);
            bool bIsPixelOfNotFontColor1 = !isPixelOfFontColor(notFontColor1);
            bool bIsPixelOfNotFontColor2 = !isPixelOfFontColor(notFontColor2);
            bool bIsPixelOfNotFontColor3 = !isPixelOfFontColor(notFontColor3);

            return bIsPixelOfFontColor1 && bIsPixelOfFontColor2 && bIsPixelOfFontColor3 &&
                bIsPixelOfNotFontColor1 && bIsPixelOfNotFontColor2 && bIsPixelOfNotFontColor3;
        }

        public static bool hasEndgameScoreboardSurvThinSkullIcon(PlayerIndex survivorIndex)
        {
            // get veretical icon offset
            int verticalIconOffset = getEndgameVerticalIconOffsetForPlayer(survivorIndex);

            int yCoord1 = survivor1PlayerKilledIconFontColor1.Y + verticalIconOffset;
            var fontColor1 = getPixelColor(survivor1PlayerKilledIconFontColor1.X, yCoord1);

            int yCoord2 = survivor1PlayerKilledIconFontColor2.Y + verticalIconOffset;
            var fontColor2 = getPixelColor(survivor1PlayerKilledIconFontColor2.X, yCoord2);

            int yCoord3 = survivor1PlayerKilledIconFontColor3.Y + verticalIconOffset;
            var fontColor3 = getPixelColor(survivor1PlayerKilledIconFontColor3.X, yCoord3);

            int yCoord4 = survivor1PlayerKilledIconNotFontColor1.Y + verticalIconOffset;
            var notFontColor1 = getPixelColor(survivor1PlayerKilledIconNotFontColor1.X, yCoord4);

            int yCoord5 = survivor1PlayerKilledIconNotFontColor2.Y + verticalIconOffset;
            var notFontColor2 = getPixelColor(survivor1PlayerKilledIconNotFontColor2.X, yCoord5);

            int yCoord6 = survivor1PlayerKilledIconNotFontColor3.Y + verticalIconOffset;
            var notFontColor3 = getPixelColor(survivor1PlayerKilledIconNotFontColor3.X, yCoord6);

            bool bIsPixelOfFontColor1 = isPixelOfFontColor(fontColor1);
            bool bIsPixelOfFontColor2 = isPixelOfFontColor(fontColor2);
            bool bIsPixelOfFontColor3 = isPixelOfFontColor(fontColor3);
            bool bIsPixelOfNotFontColor1 = !isPixelOfFontColor(notFontColor1);
            bool bIsPixelOfNotFontColor2 = !isPixelOfFontColor(notFontColor2);
            bool bIsPixelOfNotFontColor3 = !isPixelOfFontColor(notFontColor3);

            return bIsPixelOfFontColor1 && bIsPixelOfFontColor2 && bIsPixelOfFontColor3 &&
                bIsPixelOfNotFontColor1 && bIsPixelOfNotFontColor2 && bIsPixelOfNotFontColor3;
        }

        /// <summary>
        /// Survivor player head icon (when someone else DCed)
        /// </summary>
        public static bool hasEndgameScoreboardSurvHeadIcon(PlayerIndex survivorIndex)
        {
            // get veretical icon offset
            int verticalIconOffset = getEndgameVerticalIconOffsetForPlayer(survivorIndex);

            int yCoord1 = survivor1PlayerHeadIconFontColor1.Y + verticalIconOffset;
            var fontColor1 = getPixelColor(survivor1PlayerHeadIconFontColor1.X, yCoord1);

            int yCoord2 = survivor1PlayerHeadIconFontColor2.Y + verticalIconOffset;
            var fontColor2 = getPixelColor(survivor1PlayerHeadIconFontColor2.X, yCoord2);

            int yCoord3 = survivor1PlayerHeadIconFontColor3.Y + verticalIconOffset;
            var fontColor3 = getPixelColor(survivor1PlayerHeadIconFontColor3.X, yCoord3);

            int yCoord4 = survivor1PlayerHeadIconNotFontColor1.Y + verticalIconOffset;
            var notFontColor1 = getPixelColor(survivor1PlayerHeadIconNotFontColor1.X, yCoord4);

            int yCoord5 = survivor1PlayerHeadIconNotFontColor2.Y + verticalIconOffset;
            var notFontColor2 = getPixelColor(survivor1PlayerHeadIconNotFontColor2.X, yCoord5);

            bool bIsPixelOfFontColor1 = isPixelOfFontColor(fontColor1);
            bool bIsPixelOfFontColor2 = isPixelOfFontColor(fontColor2);
            bool bIsPixelOfFontColor3 = isPixelOfFontColor(fontColor3);
            bool bIsPixelOfNotFontColor1 = !isPixelOfFontColor(notFontColor1);
            bool bIsPixelOfNotFontColor2 = !isPixelOfFontColor(notFontColor2);

            return bIsPixelOfFontColor1 && bIsPixelOfFontColor2 && bIsPixelOfFontColor3 &&
                bIsPixelOfNotFontColor1 && bIsPixelOfNotFontColor2;
        }

        /// <summary>
        /// Given survivor index, calculate vertical position of his result icon (escaped/killed/DC)
        /// </summary>
        public static int getEndgameVerticalIconOffsetForPlayer(PlayerIndex survivorIndex)
        {

            int verticalOffsetBetweenPlayerIcons = 112;

            //private static int verticalOffsetForSurvivor3Icon = 2 * verticalOffsetForSurvivor2Icon;
            //private static int verticalOffsetForSurvivor4Icon = 3 * verticalOffsetForSurvivor2Icon - 3;

            int curVerticalIconOffset = 0;

            switch (survivorIndex)
            {
                case PlayerIndex.Survivor1:
                    curVerticalIconOffset = 0;
                    break;
                case PlayerIndex.Survivor2:
                    curVerticalIconOffset = verticalOffsetBetweenPlayerIcons;
                    break;
                case PlayerIndex.Survivor3:
                    curVerticalIconOffset = 2 * verticalOffsetBetweenPlayerIcons;
                    break;
                case PlayerIndex.Survivor4:
                    curVerticalIconOffset = 3 * verticalOffsetBetweenPlayerIcons - 3;
                    break;
                case PlayerIndex.Killer:
                    curVerticalIconOffset = 4 * verticalOffsetBetweenPlayerIcons - 5;
                    break;
                default:
                    Debug.Assert(false, "hasEndgameScoreboardSurv1EscapeIcon() - wrong player index");
                    break;
            }

            return curVerticalIconOffset;
        }

        public static bool hasEndgameScoreboardSurvDCIcon(PlayerIndex survivorIndex)
        {
            // get veretical icon offset
            int verticalIconOffset = getEndgameVerticalIconOffsetForPlayer(survivorIndex);

            int yCoord1 = survivor1DCIconRed1.Y + verticalIconOffset;
            var redColor1 = getPixelColor(survivor1DCIconRed1.X, yCoord1);

            int yCoord2 = survivor1DCIconFontColor1.Y + verticalIconOffset;
            var fontColor1 = getPixelColor(survivor1DCIconFontColor1.X, yCoord2);

            int yCoord3 = survivor1DCIconFontColor2.Y + verticalIconOffset;
            var fontColor2 = getPixelColor(survivor1DCIconFontColor2.X, yCoord3);

            bool bIsRedColor1 = isPixelRed(redColor1);
            bool bIsPixelOfFontColor1 = isPixelOfFontColor(fontColor1);
            bool bIsPixelOfFontColor2 = isPixelOfFontColor(fontColor2);

            return bIsRedColor1 && bIsPixelOfFontColor1 && bIsPixelOfFontColor2;
        }

        public static bool hasEndgameScoreboardSurvEscapeIcon(PlayerIndex survivorIndex)
        {
            // get veretical icon offset
            int verticalIconOffset = getEndgameVerticalIconOffsetForPlayer(survivorIndex);

            int yCoord1 = survivor1EscapeIconFontColor1.Y + verticalIconOffset;
            var fontColor1 = getPixelColor(survivor1EscapeIconFontColor1.X, yCoord1);

            int yCoord2 = survivor1EscapeIconNotFontColor1.Y + verticalIconOffset;
            var notFontColor1 = getPixelColor(survivor1EscapeIconNotFontColor1.X, yCoord2);

            int yCoord3 = survivor1EscapeIconFontColor2.Y + verticalIconOffset;
            var fontColor2 = getPixelColor(survivor1EscapeIconFontColor2.X, yCoord3);

            int yCoord4 = survivor1EscapeIconNotFontColor2.Y + verticalIconOffset;
            var notFontColor2 = getPixelColor(survivor1EscapeIconNotFontColor2.X, yCoord4);

            bool bIsPixelOfFontColor1 = isPixelOfFontColor(fontColor1);
            bool bIsNotPixelOfFontColor1 = !isPixelOfFontColor(notFontColor1);
            bool bIsPixelOfFontColor2 = isPixelOfFontColor(fontColor2);
            bool bIsNotPixelOfFontColor = !isPixelOfFontColor(notFontColor2);

            return bIsPixelOfFontColor1 && bIsNotPixelOfFontColor1 &&
                bIsPixelOfFontColor2 && bIsNotPixelOfFontColor;
        }

        /// <summary>
        /// Endgame's "Scoreboard" page is shown
        /// </summary>
        public static bool hasEndgameScoreboard()
        {
            var redColor1 = getPixelColor(endgameScoreboardRed1.X, endgameScoreboardRed1.Y);
            var darkAndNotRedColor1 = getPixelColor(endgameScoreboardDarkAndNotRed1.X, endgameScoreboardDarkAndNotRed1.Y);
            var redColor2 = getPixelColor(endgameScoreboardRed2.X, endgameScoreboardRed2.Y);
            var darkAndNotRedColor2 = getPixelColor(endgameScoreboardDarkAndNotRed2.X, endgameScoreboardDarkAndNotRed2.Y);

            var fontColor1 = getPixelColor(endgameScoreboardFontColor1.X, endgameScoreboardFontColor1.Y);
            var notFontColor1 = getPixelColor(endgameScoreboardNotFontColor.X, endgameScoreboardNotFontColor.Y);
            var fontColor2 = getPixelColor(endgameScoreboardFontColor2.X, endgameScoreboardFontColor2.Y);
            var redColor3 = getPixelColor(endgameScoreboardRed3.X, endgameScoreboardRed3.Y);

            var bIsPixelRed1 = isPixelRed(redColor1);
            var bIsPixelDarkAndNotRed1 = isPixelDarkAndNotRed(darkAndNotRedColor1);
            var bIsPixelRed2 = isPixelRed(redColor2);
            var bIsPixelDarkAndNotRed2 = isPixelDarkAndNotRed(darkAndNotRedColor2);
            var bIsPixelOfContColor1 = isPixelOfFontColor(fontColor1);
            var bIsNotPixelOfContColor1 = !isPixelOfFontColor(notFontColor1);
            var bIsPixelOfContColor2 = isPixelOfFontColor(fontColor2);
            var bIsPixelRed3 = isPixelRed(redColor3);

            return bIsPixelRed1 && bIsPixelDarkAndNotRed1 &&
                bIsPixelRed2 && bIsPixelDarkAndNotRed2 &&
                bIsPixelOfContColor1 && bIsNotPixelOfContColor1 &&
                bIsPixelOfContColor2 && bIsPixelRed3;
        }

        public static bool hasAnyMessageOverlay()
        {
            var veryDarkBlue1Color = getPixelColor(overlayMessageVeryDarkBlue1.X, overlayMessageVeryDarkBlue1.Y);
            var veryDarkBlue2Color = getPixelColor(overlayMessageVeryDarkBlue2.X, overlayMessageVeryDarkBlue2.Y);
            var pitchBlack1Color = getPixelColor(overlayMessagePitchBlack1.X, overlayMessagePitchBlack1.Y);
            var pitchBlack2Color = getPixelColor(overlayMessagePitchBlack2.X, overlayMessagePitchBlack2.Y);

            return isPixelVeryDarkBlue(veryDarkBlue1Color) && isPixelVeryDarkBlue(veryDarkBlue2Color) &&
                isPixelPitchBlack(pitchBlack1Color) && isPixelPitchBlack(pitchBlack2Color);
        }

        public static bool hasErrorMessageOverlay()
        {
            if (!bHasAnyMessageOverlay)
                return false;

            var white1Color = getPixelColor(unknownErrorWhite1.X, unknownErrorWhite1.Y);
            var notWhite1Color = getPixelColor(unknownErrorNotWhite1.X, unknownErrorNotWhite1.Y);
            var notWhite2Color = getPixelColor(unknownErrorNotWhite2.X, unknownErrorNotWhite2.Y);

            return isPixelWhite(white1Color) && !isPixelWhite(notWhite1Color) &&
                !isPixelWhite(notWhite2Color);
        }

        public static bool hasLeaveLobbyConfirmationMessage()
        {
            if (!bHasAnyMessageOverlay)
                return false;

            var white1Color = getPixelColor(leaveLobbyConfirmationMsgWhite1.X, 
                leaveLobbyConfirmationMsgWhite1.Y);
            var notWhite1Color = getPixelColor(leaveLobbyConfirmationMsgNotWhite1.X, 
                leaveLobbyConfirmationMsgNotWhite1.Y);
            var white2Color = getPixelColor(leaveLobbyConfirmationMsgWhite2.X, 
                leaveLobbyConfirmationMsgWhite2.Y);

            return isPixelWhite(white1Color) && !isPixelWhite(notWhite1Color) &&
                isPixelWhite(white2Color);
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

        public static bool hasAllPlayersEnteredLobby()
        {
            return isPlayerInLobby(PlayerIndex.Killer) &&
                isPlayerInLobby(PlayerIndex.Survivor1) &&
                isPlayerInLobby(PlayerIndex.Survivor2) &&
                isPlayerInLobby(PlayerIndex.Survivor3) &&
                isPlayerInLobby(PlayerIndex.Survivor4);
        }

        public static bool hasShopIcon()
        {
            var pitchBkack1Color = getPixelColor(shopIconPitchBlack1.X, shopIconPitchBlack1.Y);
            var pitchBkack2Color = getPixelColor(shopIconPitchBlack2.X, shopIconPitchBlack2.Y);
            var pitchBkack3Color = getPixelColor(shopIconPitchBlack3.X, shopIconPitchBlack3.Y);
            var pitchBkack4Color = getPixelColor(shopIconPitchBlack4.X, shopIconPitchBlack4.Y);
            var greyOrWhite1Color = getPixelColor(shopIconGreyOrWhite1.X, shopIconGreyOrWhite1.Y);
            var greyOrWhite2Color = getPixelColor(shopIconGreyOrWhite2.X, shopIconGreyOrWhite2.Y);

            return isPixelBlack(pitchBkack1Color) && isPixelBlack(pitchBkack2Color) &&
                isPixelBlack(pitchBkack3Color) && isPixelBlack(pitchBkack4Color) &&
                isPixelGreyOrWhite(greyOrWhite1Color) & isPixelGreyOrWhite(greyOrWhite2Color);
        }

        public static bool hasSurvivorLookingForMatchText()
        {
            var white1Color = getPixelColor(survivorLookingForMatchTextWhite1.X, survivorLookingForMatchTextWhite1.Y);
            var notWhite1Color = getPixelColor(survivorLookingForMatchTextNonWhite1.X, survivorLookingForMatchTextNonWhite1.Y);
            var notWhite2Color = getPixelColor(survivorLookingForMatchTextNonWhite2.X, survivorLookingForMatchTextNonWhite2.Y);
            var white2Color = getPixelColor(survivorLookingForMatchTextWhite2.X, survivorLookingForMatchTextWhite2.Y);
            var notWhite3Color = getPixelColor(survivorLookingForMatchTextNonWhite3.X, survivorLookingForMatchTextNonWhite3.Y);

            return isPixelWhite(white1Color) && !isPixelWhite(notWhite1Color) &&
                !isPixelWhite(notWhite2Color) && isPixelWhite(white2Color) &&
                !isPixelWhite(notWhite3Color);
        }

        public static void tick()
        {
            // Take screenshot from the game
            bool bSuccess = ScreenCapture.makeGameScreenshot();

            // No game - no screenshot - exit
            if ((!bSuccess || !ScreenCapture.haveGameHwnd()) && !ScreenCapture.haveDebugPicture())
                return;

            //// Game resolution changed?
            //if (isGameResolutionChanged())
            //    updateGameAspectRatio();

            // Parse screenshot
            parseGameScreenshot();



            var screenshot = ScreenCapture.getScreenShot();

            Graphics newGraphics = Graphics.FromImage(screenshot);

            //newGraphics.DrawLine(new Pen(Color.Yellow),
            //xcoord(readyIconRedVerticalLineX), 0,
            //xcoord(readyIconRedVerticalLineX), screenshot.Height);

            //newGraphics.DrawLine(new Pen(Color.Yellow),
            //    0, ycoord(readyIconRedVerticalLineY),
            //    screenshot.Width, ycoord(readyIconRedVerticalLineY));

    //        float dpiX, dpiY;
    //        using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
    //        {
    //            dpiX = graphics.DpiX;
    //            dpiY = graphics.DpiY;
    //        }

    //        // Draw bounds
    //        int move = 0;
    //        newGraphics.DrawLine(new Pen(Color.Red),
    //            0 + move, 0 + move, screenshot.Width - move, 0 + move);

    //        newGraphics.DrawLine(new Pen(Color.Red),
    //            0 + move, 0 + move, 0 + move, screenshot.Height);

    //        //newGraphics.DrawLine(new Pen(Color.Red),
    //        //    screenshot.Width- move, 0, screenshot.Width- move, screenshot.Height);

    //        //newGraphics.DrawLine(new Pen(Color.Red),
    //        //    0, screenshot.Height- move, screenshot.Width, screenshot.Height- move);

    //        newGraphics.DrawLine(new Pen(Color.Red, 3),
    //xcoord( baseGameScreenWidth ) - move, 0, xcoord(baseGameScreenWidth) - move, ycoord(baseGameScreenHeight));

    //        newGraphics.DrawLine(new Pen(Color.Red, 3),
    //0, ycoord(baseGameScreenHeight) - move, screenshot.Width, ycoord(baseGameScreenHeight) - move);


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

        

        public static void parseGameScreenshot()
        {

            // Any message window (error or confirmation)
            bHasAnyMessageOverlay = hasAnyMessageOverlay();

            // Error message
            bool bTempErrorMessage = hasErrorMessageOverlay();
            if (!bHasErrorMessage && bTempErrorMessage)
                Actions.onUnknownErrorOccuredMessage();
            bHasErrorMessage = bTempErrorMessage;

            // "Leave lobby?" confirmation message
            bool bTempHasLeaveLobbyConfirmationMessage = hasLeaveLobbyConfirmationMessage();
            if (!bHasLeaveLobbyConfirmationMessage && bTempHasLeaveLobbyConfirmationMessage)
                Actions.onLeaveLobbyConfirmationMessage();
            bHasLeaveLobbyConfirmationMessage = bTempHasLeaveLobbyConfirmationMessage;

            // Can't have both at the same time
            Debug.Assert(!(bHasErrorMessage && bHasLeaveLobbyConfirmationMessage));

            // Leave here and freeze all other logic until this message is gone,
            // otherwise we will change current game state in the logic,
            // because we won't recognize lobby here becayse of this message
            if (bHasErrorMessage || bHasLeaveLobbyConfirmationMessage)
                return;

            bHasCrossplayIcon = hasCrossplayIcon();
            bHasShopIcon = hasShopIcon();
            bHasUnTextGfx = hasUnGfx();
            bHasReadyGfx = hasReadyGfx();


            // Ready button
            bHasReadyButton = hasReadyButton();



            // Unready button
            bHasUnreadyButton = hasUnreadyButton();



            // Looking for a lobby
            bool bTempHasSurvivorLookingForMatchText = hasSurvivorLookingForMatchText();
            if (!bHasSurvivorLookingForMatchText && bTempHasSurvivorLookingForMatchText)
                Actions.onStartedLookingForLobby();
            bHasSurvivorLookingForMatchText = bTempHasSurvivorLookingForMatchText;

            // Just entered pre-lobby (we are not looking for a lobby and we are not in the lobby)?
            bool bTempInPreLobbyIdling = bHasReadyButton && bHasShopIcon && !bHasSurvivorLookingForMatchText;
            if (!bInPreLobbyIdling && bTempInPreLobbyIdling)
                Actions.onIdlingInPreLobby();
            bInPreLobbyIdling = bTempInPreLobbyIdling;

            // Entered the actual lobby
            bool bTempEnteredLobby = isGameLobbyShown();
            if (!bEnteredLobby && bTempEnteredLobby)
                Actions.onEnteredLobby();
            bEnteredLobby = bTempEnteredLobby;

            // Can't have both active at the same time
            Debug.Assert(!(bInPreLobbyIdling && bEnteredLobby));

            bKillerCancelButton = hasKillerCancelButton();
            bSurvivorCancelButton = hasSurvivorCancelButton();


            // Only check player ready status if we are in the lobby!
            // Otherwise we might recognize ready states on an image
            // where ready-states are not even shown
            if (isGameLobbyShown())
            {
                // All players have just entered lobby?
                bool bNewAllPlayersInLobby = hasAllPlayersEnteredLobby();
                if (!bAllPlayersInLobby && bNewAllPlayersInLobby)
                    Actions.onAllPlayersEnteredGameLobby();
                bAllPlayersInLobby = bNewAllPlayersInLobby;

                // All players clicked "Ready" and have red checkmarks?
                bool bNewAllPlayersReady = hasAllPlayersClickedReady();
                if (!bAllPlayersReady && bNewAllPlayersReady)
                    Actions.onAllPlayersClickedReady();
                bAllPlayersReady = bNewAllPlayersReady;
            }
            else
            {
                // Since we are not in the lobby, we know for sure
                // that there are no ready-states for all players
                bAllPlayersInLobby = false;
                bAllPlayersReady = false;
            }

            bEndgameScoreboard = hasEndgameScoreboard();
            if(bEndgameScoreboard)
            {
                var bHaveSurvivor1EscapeIcon = getEndgameScoreboardSurvIcon(PlayerIndex.Survivor1);
                var bHaveSurvivor2EscapeIcon = getEndgameScoreboardSurvIcon(PlayerIndex.Survivor2);
                var bHaveSurvivor3EscapeIcon = getEndgameScoreboardSurvIcon(PlayerIndex.Survivor3);
                var bHaveSurvivor4EscapeIcon = getEndgameScoreboardSurvIcon(PlayerIndex.Survivor4);
            }

            //// nothing recognized - recalc aspect ratio
            //if (!(bHasCrossplayIcon || bHasUnTextGfx || bHasReadyGfx || bHasReadyButton ||
            //    bHasUnreadyButton || bHasSurvivorLookingForMatchText || bHasShopIcon))
            //{
            //    updateGameAspectRatio();
            //}
        }

        /// <summary>
        /// Are we currently in the lobby? (not pre-lobby)
        /// </summary>
        public static bool isGameLobbyShown()
        {
            return (bHasReadyButton || bHasUnreadyButton)
                && !bHasShopIcon && !bHasSurvivorLookingForMatchText;
        }


        /// <summary>
        /// Has "Un", as beginning of "Unready". For both killers and survivors
        /// </summary>
        public static bool hasUnGfx()
        {
            int horizontalOffset = getHorizontalOffsetForCrossplayIcon();

            var notGrey1Color = getPixelColor(unreadyIconNotGrey1.X + horizontalOffset,
                unreadyIconNotGrey1.Y);
            var fontColor1 = getPixelColor(unreadyIconGreyOrWhite1.X + horizontalOffset,
                unreadyIconGreyOrWhite1.Y);
            var notGrey2Color = getPixelColor(unreadyIconNotGrey2.X + horizontalOffset,
                unreadyIconNotGrey2.Y);
            var fontColor2 = getPixelColor(unreadyIconGreyOrWhite2.X + horizontalOffset,
                unreadyIconGreyOrWhite2.Y);
            var notGrey3Color = getPixelColor(unreadyIconNotGrey3.X + horizontalOffset,
                unreadyIconNotGrey3.Y);

            return !isPixelGrey(notGrey1Color) && isPixelGreyOrWhite(fontColor1) &&
                !isPixelGrey(notGrey2Color) && isPixelGreyOrWhite(fontColor2) &&
                !isPixelGrey(notGrey3Color);
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

            var redColor = getPixelColor(readyIconRedVerticalLine.X + horizontalOffset,
                readyIconRedVerticalLine.Y);
            var greyOrWhiteColor = getPixelColor(readyIconGreyOrWhite.X + horizontalOffset,
                readyIconGreyOrWhite.Y);
            var notGreyColor = getPixelColor(readyIconNotGrey1.X + horizontalOffset,
                readyIconNotGrey1.Y);
            var greyOrWhite2Color = getPixelColor(readyIconGreyOrWhile2.X + horizontalOffset,
                readyIconGreyOrWhile2.Y);
            var notGrey2Color = getPixelColor(readyIconNotGrey2.X + horizontalOffset,
                readyIconNotGrey2.Y);

            bool red = isPixelRed(redColor);
            bool greyOrWhite1 = isPixelGreyOrWhite(greyOrWhiteColor);
            bool grey1 = isPixelGrey(notGreyColor);
            bool greyOrWhite2 = isPixelGreyOrWhite(greyOrWhite2Color);
            bool grey2 = isPixelGrey(notGrey2Color);







            //var screenshot = ScreenCapture.getScreenShot();

            //Graphics newGraphics = Graphics.FromImage(screenshot);

            //newGraphics.DrawLine(new Pen(Color.Yellow),
            //    xcoord(182), 0,
            //    xcoord(182), screenshot.Height);

            //newGraphics.DrawLine(new Pen(Color.Yellow),
            //    0, ycoord(1005),
            //    screenshot.Width, ycoord(1005));


            bool result = red && greyOrWhite1 && !grey1 && greyOrWhite2 && !grey2;

            return result;
        }

        /// <summary>
        /// If "Cancel" text is displayed when searching for a lobby as killer
        /// </summary>
        public static bool hasSurvivorCancelButton()
        {
            int horizontalOffset = getHorizontalOffsetForCrossplayIcon();

            var redColor = getPixelColor(survivorCancelIconRedVerticalLine.X + horizontalOffset,
                survivorCancelIconRedVerticalLine.Y);
            var notGreyColor = getPixelColor(survivorCancelIconNotGrey1.X + horizontalOffset,
                survivorCancelIconNotGrey1.Y);
            var greyColor = getPixelColor(survivorCancelIconGreyOrWhite1.X + horizontalOffset,
                survivorCancelIconGreyOrWhite1.Y);
            var notGrey2Color = getPixelColor(survivorCancelIconNotGrey2.X + horizontalOffset,
                survivorCancelIconNotGrey2.Y);
            var grey2Color = getPixelColor(survivorCancelIconGreyOrWhite2.X + horizontalOffset,
                survivorCancelIconGreyOrWhite2.Y);

            return isPixelRed(redColor) && !isPixelGrey(notGreyColor) &&
                isPixelGreyOrWhite(greyColor) && !isPixelGrey(notGrey2Color) &&
                isPixelGreyOrWhite(grey2Color);
        }

        /// <summary>
        /// If "Cancel" text is displayed when searching for a lobby as killer
        /// </summary>
        public static bool hasKillerCancelButton()
        {
            var redColor = getPixelColor(killerCancelIconRedVerticalLine.X, killerCancelIconRedVerticalLine.Y);
            var notGreyColor = getPixelColor(killerCancelIconNotGrey1.X, killerCancelIconNotGrey1.Y);
            var greyColor = getPixelColor(killerCancelIconGreyOrWhite1.X, killerCancelIconGreyOrWhite1.Y);
            var notGrey2Color = getPixelColor(killerCancelIconNotGrey2.X, killerCancelIconNotGrey2.Y);
            var grey2Color = getPixelColor(killerCancelIconGreyOrWhite2.X, killerCancelIconGreyOrWhite2.Y);

            return isPixelRed(redColor) && !isPixelGrey(notGreyColor) &&
                isPixelGreyOrWhite(greyColor) && !isPixelGrey(notGrey2Color) &&
                isPixelGreyOrWhite(grey2Color);
        }





        // Ready/Cancel buttons are moved to the left, when the "crossplay" icon is present
        private static int getHorizontalOffsetForCrossplayIcon()
        {
            int horizontalOffset = xcoord(crossplayIconWidth, false);

            if (bHasCrossplayIcon)
                horizontalOffset = 0;

            return horizontalOffset;
        }



        public enum LobbyState
        {
            PreLobbyWithCrossplayIcon,  // killer and survivor
            PreLobbyNoCrossplayIcon
        }

        public enum PlayerIndex
        {
            Killer, Survivor1, Survivor2, Survivor3, Survivor4
        }

        public enum PlayerLobbyStatus
        {
            NotPresent, Unready, Ready, Error
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        static ScreenParser()
        {
            playerCheckmarkX[(int)PlayerIndex.Killer] = 1794;
            playerCheckmarkY[(int)PlayerIndex.Killer] = 938;

            playerCheckmarkX[(int)PlayerIndex.Survivor1] = 1773;
            playerCheckmarkY[(int)PlayerIndex.Survivor1] = 921;

            playerCheckmarkX[(int)PlayerIndex.Survivor2] = 1786;
            playerCheckmarkY[(int)PlayerIndex.Survivor2] = playerCheckmarkY[0];

            playerCheckmarkX[(int)PlayerIndex.Survivor3] = 1800;
            playerCheckmarkY[(int)PlayerIndex.Survivor3] = playerCheckmarkY[0];

            playerCheckmarkX[(int)PlayerIndex.Survivor4] = 1813;
            playerCheckmarkY[(int)PlayerIndex.Survivor4] = playerCheckmarkY[0];
        }

        public static bool hasCrossplayIcon()
        {
            var redColor = getPixelColor(crossplayIconRed.X, crossplayIconRed.Y);
            var greyOrWhiteColor = getPixelColor(crossplayIconGreyOrWhite.X, crossplayIconGreyOrWhite.Y);
            var blackColor = getPixelColor(crossplayIconBlack.X, crossplayIconBlack.Y);

            bool red = isPixelRed(redColor);
            bool greyOrWhite = isPixelGreyOrWhite(greyOrWhiteColor);
            bool black = isPixelBlack(blackColor);

            bool result = red && greyOrWhite && black;

            //if (!result)
            //{
            //    Debugger.Break();
            //}

            return result;
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

        private static bool isPixelRed(Color c)
        {
            return (c.R > 70 && c.R > (c.G * 3) && (c.R > c.B * 3));
        }

        private static bool isPixelVeryDarkBlue(Color c)
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

        private static bool isPixelDarkAndNotRed(Color c)
        {
            return !isPixelRed(c) && isPixelDark(c);
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

        private static Color getPixelColor(int x, int y)
        {
            var newX = xcoord(x);
            var newY = ycoord(y);

            var screen = ScreenCapture.getScreenShot();

            return screen.GetPixel(newX, newY);
        }

        public static bool getPlayerIsReady(PlayerIndex playerIndex)
        {
            var color = getPixelColor(playerCheckmarkX[(int)playerIndex], playerCheckmarkY[(int)playerIndex]);

            return isPixelRed(color);
        }

        public static bool getPlayerIsUnready(PlayerIndex playerIndex)
        {
            var color = getPixelColor(playerCheckmarkX[(int)playerIndex], playerCheckmarkY[(int)playerIndex]);

            return isPixelGrey(color);
        }

        /// <summary>
        /// Returns true if survivor not in the lobby
        /// </summary>
        public static bool getPlayerNotPresent(PlayerIndex playerIndex)
        {
            var color = getPixelColor(playerCheckmarkX[(int)playerIndex], playerCheckmarkY[(int)playerIndex]);

            return isPixelBlack(color);
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

        public static bool isPlayerInLobby(PlayerIndex playerIndex)
        {
            return getPlayerIsUnready(playerIndex) || getPlayerIsReady(playerIndex);
        }
    }
}
