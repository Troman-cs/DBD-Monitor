using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDMN
{
    public static class ScreenParser
    {
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

        private static int crossplayIconRedX = 1817;
        private static int crossplayIconRedY = 1016;
        private static int crossplayIconGreyOrWhiteX = 1804;
        private static int crossplayIconGreyOrWhiteY = 1004;
        private static int crossplayIconBlackX = 1800;      // Inside the globe icon
        private static int crossplayIconBlackY = 1000;

        /// <summary>
        /// Width of the crossplay icon. It moved "Ready" button by this amount to the left
        /// </summary>
        private static int crossplayIconWidth = 1827 - 1781;    // 46

        /// <summary>
        /// Lobby "Ready" button.
        /// For both killer and survivor. All coords are with "crossplay" icon shown.
        /// </summary>
        private static int readyIconRedVerticalLineX = 1775;
        private static int readyIconRedVerticalLineY = 1008;
        private static int readyIconGreyOrWhiteX = 1745;    // Letter "Y", white only when mouseover
        private static int readyIconGreyOrWhiteY = 1007;
        private static int readyIconNotGrey1X = 1745;       // Inside letter "Y"
        private static int readyIconNotGrey1Y = 998;
        private static int readyIconGreyOrWhile2X = 1660;       // Letter "R"
        private static int readyIconGreyOrWhile2Y = 998;
        private static int readyIconNotGrey2X = 1665;       // Indise letter "R"
        private static int readyIconNotGrey2Y = 1003;

        // "UN" from "Unready". For both killer and survivor. All coords are with "crossplay" icon shown.
        private static int unreadyIconNotGrey1X = 1656;       // Between "N" and "R"
        private static int unreadyIconNotGrey1Y = 1007;
        private static int unreadyIconGreyOrWhite1X = 1639;    // Letter "N"
        private static int unreadyIconGreyOrWhite1Y = 1001;
        private static int unreadyIconNotGrey2X = 1634;       // Between "U" and "N"
        private static int unreadyIconNotGrey2Y = 1007;
        private static int unreadyIconGreyOrWhite2X = 1617;    // Letter "U"
        private static int unreadyIconGreyOrWhite2Y = 1007;
        private static int unreadyIconNotGrey3X = 1623;       // Inside "U"
        private static int unreadyIconNotGrey3Y = 1007;

        #region "Cancel" text when searching for lobby as killer
        private static int killerCancelIconRedVerticalLineX = 1827;
        private static int killerCancelIconRedVerticalLineY = 940;
        private static int killerCancelIconNotGrey1X = 1812;        // To the right of "L" char
        private static int killerCancelIconNotGrey1Y = 940;
        private static int killerCancelIconGreyOrWhite1X = 1808;           // "L" character
        private static int killerCancelIconGreyOrWhite1Y = 940;
        private static int killerCancelIconNotGrey2X = 1751;        // Inside "C" character
        private static int killerCancelIconNotGrey2Y = 940;
        private static int killerCancelIconGreyOrWhite2X = 1746;        // "C" character
        private static int killerCancelIconGreyOrWhite2Y = 940;
        #endregion

        #region "Cancel" text when searching for lobby as survivor. This is with "Crossplay" icon shown
        private static int survivorCancelIconRedVerticalLineX = 1774;
        private static int survivorCancelIconRedVerticalLineY = 1007;
        private static int survivorCancelIconNotGrey1X = 1750;        // To the right of "L" char
        private static int survivorCancelIconNotGrey1Y = 1007;
        private static int survivorCancelIconGreyOrWhite1X = 1743;           // "L" character
        private static int survivorCancelIconGreyOrWhite1Y = 1007;
        private static int survivorCancelIconNotGrey2X = 1648;        // Inside "C" character
        private static int survivorCancelIconNotGrey2Y = 1007;
        private static int survivorCancelIconGreyOrWhite2X = 1640;        // "C" character
        private static int survivorCancelIconGreyOrWhite2Y = 1007;
        #endregion

        #region "looking for match..." text for survivors when searching for lobby
        private static int survivorLookingForMatchTextWhite1X = 1710;   // "h" char
        private static int survivorLookingForMatchTextWhite1Y = 927;
        private static int survivorLookingForMatchTextNonWhite1X = 1659;   // Between "for" and "match"
        private static int survivorLookingForMatchTextNonWhite1Y = 932;
        private static int survivorLookingForMatchTextNonWhite2X = 1630;   // Between "looking" and "for"
        private static int survivorLookingForMatchTextNonWhite2Y = 932;
        private static int survivorLookingForMatchTextWhite2X = 1593;   // "k" char
        private static int survivorLookingForMatchTextWhite2Y = 931;
        private static int survivorLookingForMatchTextNonWhite3X = 1563;   // Inside "L"
        private static int survivorLookingForMatchTextNonWhite3Y = 930;
        #endregion

        private static int shopIconPitchBlack1X = 88;
        private static int shopIconPitchBlack1Y = 794;
        private static int shopIconPitchBlack2X = 184;
        private static int shopIconPitchBlack2Y = 795;
        private static int shopIconPitchBlack3X = 180;
        private static int shopIconPitchBlack3Y = 880;
        private static int shopIconPitchBlack4X = 88;
        private static int shopIconPitchBlack4Y = 880;
        private static int shopIconGreyOrWhite1X = 136;
        private static int shopIconGreyOrWhite1Y = 844;
        private static int shopIconGreyOrWhite2X = 155;
        private static int shopIconGreyOrWhite2Y = 834;

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
        #endregion


        public static bool hasAllPlayersInLobby()
        {
            return playerInLobby(PlayerIndex.Killer) &&
                playerInLobby(PlayerIndex.Survivor1) &&
                playerInLobby(PlayerIndex.Survivor2) &&
                playerInLobby(PlayerIndex.Survivor3) &&
                playerInLobby(PlayerIndex.Survivor4);
        }

        public static bool hasShopIcon()
        {
            var pitchBkack1Color = getPixelColor(shopIconPitchBlack1X, shopIconPitchBlack1Y);
            var pitchBkack2Color = getPixelColor(shopIconPitchBlack2X, shopIconPitchBlack2Y);
            var pitchBkack3Color = getPixelColor(shopIconPitchBlack3X, shopIconPitchBlack3Y);
            var pitchBkack4Color = getPixelColor(shopIconPitchBlack4X, shopIconPitchBlack4Y);
            var greyOrWhite1Color = getPixelColor(shopIconGreyOrWhite1X, shopIconGreyOrWhite1Y);
            var greyOrWhite2Color = getPixelColor(shopIconGreyOrWhite2X, shopIconGreyOrWhite2Y);

            return isPixelBlack(pitchBkack1Color) && isPixelBlack(pitchBkack2Color) &&
                isPixelBlack(pitchBkack3Color) && isPixelBlack(pitchBkack4Color) &&
                isPixelGreyOrWhite(greyOrWhite1Color) & isPixelGreyOrWhite(greyOrWhite2Color);
        }

        public static bool hasSurvivorLookingForMatchText()
        {
            var white1Color = getPixelColor(survivorLookingForMatchTextWhite1X, survivorLookingForMatchTextWhite1Y);
            var notWhite1Color = getPixelColor(survivorLookingForMatchTextNonWhite1X, survivorLookingForMatchTextNonWhite1Y);
            var notWhite2Color = getPixelColor(survivorLookingForMatchTextNonWhite2X, survivorLookingForMatchTextNonWhite2Y);
            var white2Color = getPixelColor(survivorLookingForMatchTextWhite2X, survivorLookingForMatchTextWhite2Y);
            var notWhite3Color = getPixelColor(survivorLookingForMatchTextNonWhite3X, survivorLookingForMatchTextNonWhite3Y);

            return isPixelWhite(white1Color) && !isPixelWhite(notWhite1Color) &&
                !isPixelWhite(notWhite2Color) && isPixelWhite(white2Color) &&
                !isPixelWhite(notWhite3Color);
        }

        public static void tick()
        {
            // Take screenshot from the game
            bool bSuccess = ScreenCapture.makeGameScreenshot();

            // No game - no screenshot - exit
            if (!bSuccess || !ScreenCapture.haveGameHwnd())
                return;

            // Game resolution changed?
            if (isGameResolutionChanged())
                updateGameAspectRatio();

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

            float dpiX, dpiY;
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                dpiX = graphics.DpiX;
                dpiY = graphics.DpiY;
            }

            // Draw bounds
            int move = 0;
            newGraphics.DrawLine(new Pen(Color.Red),
                0 + move, 0 + move, screenshot.Width - move, 0 + move);

            newGraphics.DrawLine(new Pen(Color.Red),
                0 + move, 0 + move, 0 + move, screenshot.Height);

            //newGraphics.DrawLine(new Pen(Color.Red),
            //    screenshot.Width- move, 0, screenshot.Width- move, screenshot.Height);

            //newGraphics.DrawLine(new Pen(Color.Red),
            //    0, screenshot.Height- move, screenshot.Width, screenshot.Height- move);

            newGraphics.DrawLine(new Pen(Color.Red, 3),
    xcoord( baseGameScreenWidth ) - move, 0, xcoord(baseGameScreenWidth) - move, ycoord(baseGameScreenHeight));

            newGraphics.DrawLine(new Pen(Color.Red, 3),
    0, ycoord(baseGameScreenHeight) - move, screenshot.Width, ycoord(baseGameScreenHeight) - move);


        }

        private static bool isGameResolutionChanged()
        {
            var screenshot = ScreenCapture.getScreenShot();

            // Game resolution changed?
            return (screenshot.Width != curGameScreenWidth || screenshot.Height != curGameScreenHeight);
        }

        /// <summary>
        /// Game resolution has changed. Recalc aspect ratio, which is relative
        /// to our base aspect ration, with which we defined various points for parsing
        /// </summary>
        private static void updateGameAspectRatio()
        {
            var screenshot = ScreenCapture.getScreenShot();

            // remember new game resolution
            curGameScreenWidth = screenshot.Width;
            curGameScreenHeight = screenshot.Height;

            // calc new aspect ratio, relative to our base game aspect ratio ratio
            curHorizontalAspectRatio = (float)baseGameScreenWidth / (float)curGameScreenWidth;
            curVerticalAspectRatio = (float)baseGameScreenHeight / (float)curGameScreenHeight;

            // Game DPI
            gameDPI = 100f * (ScreenCapture.User32.GetDpiForWindow(ScreenCapture.gameHWND) / 96f);
            myDPI = Graphics.FromHwnd(IntPtr.Zero).DpiX;

            // Windowed mode?
            bIsWindowed = ScreenCapture.isDBDInWindowMode();


            if (Graphics.FromHwnd(IntPtr.Zero).DpiX != Graphics.FromHwnd(IntPtr.Zero).DpiY)
            {
                throw new Exception("Wrong DPI: " +  Graphics.FromHwnd(IntPtr.Zero).DpiX +
                    "/" + Graphics.FromHwnd(IntPtr.Zero).DpiY);
            }
        }

        public static void parseGameScreenshot()
        {
            bHasCrossplayIcon = hasCrossplayIcon();
            bHasUnTextGfx = hasUnGfx();
            bHasReadyGfx = hasReadyGfx();
            bHasReadyButton = hasReadyButton();
            bHasUnreadyButton = hasUnreadyButton();
            bHasSurvivorLookingForMatchText = hasSurvivorLookingForMatchText();
            bHasShopIcon = hasShopIcon();
            bKillerCancelButton = hasKillerCancelButton();
            bSurvivorCancelButton = hasSurvivorCancelButton();

            // nothing recognized - recalc aspect ratio
            if (!(bHasCrossplayIcon || bHasUnTextGfx || bHasReadyGfx || bHasReadyButton ||
                bHasUnreadyButton || bHasSurvivorLookingForMatchText || bHasShopIcon))
            {
                updateGameAspectRatio();
            }
        }



        /// <summary>
        /// Has "Un", as beginning of "Unready". For both killers and survivors
        /// </summary>
        public static bool hasUnGfx()
        {
            int horizontalOffset = getHorizontalOffsetForCrossplayIcon();

            var notGrey1Color = getPixelColor(unreadyIconNotGrey1X + horizontalOffset,
                unreadyIconNotGrey1Y);
            var greyOrWhite1Color = getPixelColor(unreadyIconGreyOrWhite1X + horizontalOffset,
                unreadyIconGreyOrWhite1Y);
            var notGrey2Color = getPixelColor(unreadyIconNotGrey2X + horizontalOffset,
                unreadyIconNotGrey2Y);
            var greyOrWhite2Color = getPixelColor(unreadyIconGreyOrWhite2X + horizontalOffset,
                unreadyIconGreyOrWhite2Y);
            var notGrey3Color = getPixelColor(unreadyIconNotGrey3X + horizontalOffset,
                unreadyIconNotGrey3Y);

            return !isPixelGrey(notGrey1Color) && isPixelGreyOrWhite(greyOrWhite1Color) &&
                !isPixelGrey(notGrey2Color) && isPixelGreyOrWhite(greyOrWhite2Color) &&
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

            var redColor = getPixelColor(readyIconRedVerticalLineX + horizontalOffset,
                readyIconRedVerticalLineY);
            var greyOrWhiteColor = getPixelColor(readyIconGreyOrWhiteX + horizontalOffset,
                readyIconGreyOrWhiteY);
            var notGreyColor = getPixelColor(readyIconNotGrey1X + horizontalOffset,
                readyIconNotGrey1Y);
            var greyOrWhite2Color = getPixelColor(readyIconGreyOrWhile2X + horizontalOffset,
                readyIconGreyOrWhile2Y);
            var notGrey2Color = getPixelColor(readyIconNotGrey2X + horizontalOffset,
                readyIconNotGrey2Y);

            bool red = isPixelRed(redColor);
            bool greyOrWhite1 = isPixelGreyOrWhite(greyOrWhiteColor);
            bool grey1 = isPixelGrey(notGreyColor);
            bool greyOrWhite2 = isPixelGreyOrWhite(greyOrWhite2Color);
            bool grey2 = isPixelGrey(notGrey2Color);







            var screenshot = ScreenCapture.getScreenShot();

            Graphics newGraphics = Graphics.FromImage(screenshot);

            newGraphics.DrawLine(new Pen(Color.Yellow),
                xcoord(182), 0,
                xcoord(182), screenshot.Height);

            newGraphics.DrawLine(new Pen(Color.Yellow),
                0, ycoord(1005),
                screenshot.Width, ycoord(1005));



            return red && greyOrWhite1 &&
                !grey1 && greyOrWhite2 &&
                !grey2;
        }

        /// <summary>
        /// If "Cancel" text is displayed when searching for a lobby as killer
        /// </summary>
        public static bool hasSurvivorCancelButton()
        {
            int horizontalOffset = getHorizontalOffsetForCrossplayIcon();

            var redColor = getPixelColor(survivorCancelIconRedVerticalLineX + horizontalOffset,
                survivorCancelIconRedVerticalLineY);
            var notGreyColor = getPixelColor(survivorCancelIconNotGrey1X + horizontalOffset,
                survivorCancelIconNotGrey1Y);
            var greyColor = getPixelColor(survivorCancelIconGreyOrWhite1X + horizontalOffset,
                survivorCancelIconGreyOrWhite1Y);
            var notGrey2Color = getPixelColor(survivorCancelIconNotGrey2X + horizontalOffset,
                survivorCancelIconNotGrey2Y);
            var grey2Color = getPixelColor(survivorCancelIconGreyOrWhite2X + horizontalOffset,
                survivorCancelIconGreyOrWhite2Y);

            return isPixelRed(redColor) && !isPixelGrey(notGreyColor) &&
                isPixelGreyOrWhite(greyColor) && !isPixelGrey(notGrey2Color) &&
                isPixelGreyOrWhite(grey2Color);
        }

        /// <summary>
        /// If "Cancel" text is displayed when searching for a lobby as killer
        /// </summary>
        public static bool hasKillerCancelButton()
        {
            var redColor = getPixelColor(killerCancelIconRedVerticalLineX, killerCancelIconRedVerticalLineY);
            var notGreyColor = getPixelColor(killerCancelIconNotGrey1X, killerCancelIconNotGrey1Y);
            var greyColor = getPixelColor(killerCancelIconGreyOrWhite1X, killerCancelIconGreyOrWhite1Y);
            var notGrey2Color = getPixelColor(killerCancelIconNotGrey2X, killerCancelIconNotGrey2Y);
            var grey2Color = getPixelColor(killerCancelIconGreyOrWhite2X, killerCancelIconGreyOrWhite2Y);

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
            var redColor = getPixelColor(crossplayIconRedX, crossplayIconRedY);
            var greyOrWhiteColor = getPixelColor(crossplayIconGreyOrWhiteX, crossplayIconGreyOrWhiteY);
            var blackColor = getPixelColor(crossplayIconBlackX, crossplayIconBlackY);

            bool red = isPixelRed(redColor);
            bool greyOrWhite = isPixelGreyOrWhite(greyOrWhiteColor);
            bool black = isPixelBlack(blackColor);


            return red && greyOrWhite && black;
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

        /// <summary>
        /// Fix horizontal aspect ratio
        /// </summary>
        private static int xcoord(int x, bool addBorderOffset = true)
        {
            var result = (int)((float)x / (float)curHorizontalAspectRatio);
            //result = (int) ((float)x / (float)1.51f);

            // add only for points, not to some width
            if (addBorderOffset)
            {
                if (bIsWindowed)
                    //result = result + 6;
                    result = result - 0;
            }

            Debug.Assert(result >= 0);

            return result;  // (int)(result / (gameDPI / myDPI));
        }

        /// <summary>
        /// Fix vertival aspect ratio
        /// </summary>
        private static int ycoord(int y)
        {
            var result = (int)((float)y / (float)curVerticalAspectRatio);
            result = result + 7;
            if (bIsWindowed)
                result = result + 1;

            Size s = new Size();

            return result;  // (int)(result / (gameDPI / myDPI));
        }

        public static bool isPixelGrey(Color c)
        {
            // Black or white is not grey
            if (isPixelBlack(c) || isPixelWhite(c))
                return false;

            return !isTooBrightForGrey(c) && !isTooDarkFoGrey(c) && isRgbValuesCloseForGreyColor(c);

            bool isTooDarkFoGrey(Color greyShade)
            {
                return greyShade.R < 50 && greyShade.G < 50 && greyShade.B < 50;
            }

            bool isTooBrightForGrey(Color greyShade)
            {
                return greyShade.R > 240 && greyShade.G > 240 && greyShade.B > 240;
            }
        }

        private static bool isRgbValuesCloseForGreyColor(Color c)
        {
            return Math.Abs(c.R - c.G) < 20 &&
                Math.Abs(c.R - c.B) < 20 &&
                Math.Abs(c.G - c.B) < 20;
        }

        public static bool isPixelBlack(Color c)
        {
            return c.R < 25 && c.G < 25 && c.B < 25 &&
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

        public static bool isPixelWhite(Color c)
        {
            return c.R > 240 && c.G > 240 && c.B > 240 &&
                isRgbValuesCloseForGreyColor(c);
        }

        public static bool isPixelGreyOrWhite(Color c)
        {
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

        public static bool playerInLobby(PlayerIndex playerIndex)
        {
            return getPlayerIsUnready(playerIndex) || getPlayerIsReady(playerIndex);
        }
    }
}
