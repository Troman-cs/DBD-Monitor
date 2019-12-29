using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DBDMN.GamePoint;
using static DBDMN.ScreenParser;

namespace DBDMN
{
    /// <summary>
    /// Digits of the BP score of players (killer/survivor) on the Endscore screen
    /// </summary>
    public class EndscoreBpDigitGfx : Gfx
    {
        public const int INVALID_BP_AMOUNT = -1;    // int.MinValue;

        public static EndscoreBpDigitGfx[] digit = new EndscoreBpDigitGfx[10];

        // From left to right
        private static Dictionary<string, Dictionary<PlayerIndex, List<int>>> horizontalDigitOffset = 
            new Dictionary<string, Dictionary<PlayerIndex, List<int>>>();

        public static Dictionary<string, int[]> verticalOffsetBetweenEndscorePlayerBpDigits =
            new Dictionary<string, int[]>();

        public enum DigitEnum
        {
            Digit0 = 0, Digit1, Digit2, Digit3, Digit4, Digit5, Digit6, Digit7, Digit8, Digit9, Error
        }

        /// <summary>
        /// Index of a digit in BP score. 0-based. E.g. char at index '3' from "14 567" is "6"
        /// </summary>
        public enum DigitPosition
        {
            DigitPos0 = 0, DigitPos1, DigitPos2, DigitPos3, DigitPos4, Error
        }

        public static void intitialize()
        {
            verticalOffsetBetweenEndscorePlayerBpDigits[ Gfx._720p ] = new int[ 5 ];
            verticalOffsetBetweenEndscorePlayerBpDigits[ Gfx._720p ][ ( int )PlayerIndex.Survivor1 ] = 204;
            verticalOffsetBetweenEndscorePlayerBpDigits[ Gfx._720p ][ ( int )PlayerIndex.Survivor2 ] = 278;
            verticalOffsetBetweenEndscorePlayerBpDigits[ Gfx._720p ][ ( int )PlayerIndex.Survivor3 ] = 353;
            verticalOffsetBetweenEndscorePlayerBpDigits[ Gfx._720p ][ ( int )PlayerIndex.Survivor4 ] = 426;
            verticalOffsetBetweenEndscorePlayerBpDigits[ Gfx._720p ][ ( int )PlayerIndex.Killer ] = 502;

            verticalOffsetBetweenEndscorePlayerBpDigits[ Gfx._1080p ] = new int[ 5 ];
            verticalOffsetBetweenEndscorePlayerBpDigits[ Gfx._1080p ][ ( int )PlayerIndex.Survivor1 ] = 306;
            verticalOffsetBetweenEndscorePlayerBpDigits[ Gfx._1080p ][ ( int )PlayerIndex.Survivor2 ] = 418;
            verticalOffsetBetweenEndscorePlayerBpDigits[ Gfx._1080p ][ ( int )PlayerIndex.Survivor3 ] = 530;
            verticalOffsetBetweenEndscorePlayerBpDigits[ Gfx._1080p ][ ( int )PlayerIndex.Survivor4 ] = 639;
            verticalOffsetBetweenEndscorePlayerBpDigits[ Gfx._1080p ][ ( int )PlayerIndex.Killer ] = 754;

            

            // Init horizontal digit offset
            // 1080p
            var offset1080p = new List<int>();
            offset1080p.Add( 661 );   // Left-most digit
            offset1080p.Add( 683 );
            offset1080p.Add( 715 );
            offset1080p.Add( 738 );
            offset1080p.Add( 760 );   // Right-most digit

            // For 4 survivors offsets are the same
            horizontalDigitOffset[ Gfx._1080p ] = new Dictionary<PlayerIndex, List<int>>();
            for ( int playerIndex = ( int )PlayerIndex.Survivor1; playerIndex <= ( int )PlayerIndex.Survivor4; playerIndex++ )
            {
                horizontalDigitOffset[ Gfx._1080p ][ ( PlayerIndex )playerIndex ] = offset1080p;
            }

            // All digits except for the first one are moved by 1 pixel for killer in 1080p
            horizontalDigitOffset[ Gfx._1080p ][ PlayerIndex.Killer ] = new List<int> { offset1080p[ 0 ],
                offset1080p[ 1 ] + 1, offset1080p[ 2 ] + 1, offset1080p[ 3 ] + 1, offset1080p[ 4 ] + 1 };

            // 720p
            var offset720p = new List<int>();
            offset720p.Add( 441 );   // Left-most digit
            offset720p.Add( 456 );
            offset720p.Add( 477 );
            offset720p.Add( 492 );
            offset720p.Add( 507 );   // Right-most digit

            // For all 5 players aoffsets are the same
            horizontalDigitOffset[ Gfx._720p ] = new Dictionary<PlayerIndex, List<int>>();
            for ( int playerIndex = (int)PlayerIndex.Survivor1; playerIndex <= (int)PlayerIndex.Killer; playerIndex++ )
            {
                horizontalDigitOffset[ Gfx._720p ][ ( PlayerIndex )playerIndex ] = offset720p;
            }


            digit[ ( int )DigitEnum.Digit0 ] = new EndscoreBpDigitGfx( "Endscore Player BP Digit 0",
                false, new List<GamePoint>
                {
                    new GamePoint( "Top, in the center", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "10-1", [_720p] = "6-0" } ),
                    new GamePoint( "Bottom, in the center", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "10-27", [_720p] = "6-19" } ),
                    new GamePoint( "Left, in the center", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "2-14", [_720p] = "1-9" } ),
                    new GamePoint( "Right, in the center", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "17-14", [_720p] = "11-4" } ),
                    new GamePoint( "Inside '0'", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "10-13", [_720p] = "7-9" } ),
                    new GamePoint( "Make sure it's not 6: check middle arc of '6'", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "10-11", [_720p] = "7-7" } ),
                    new GamePoint( "Make sure it's not 9: check middle arc of '9'", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "9-18", [_720p] = "6-12" } ),
                } );

            digit[ ( int )DigitEnum.Digit1 ] = new EndscoreBpDigitGfx( "Endscore Player BP Digit 1",
                false, new List<GamePoint>
                {
                    new GamePoint("Top", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "11-1", [_720p] = "7-0" } ),
                    new GamePoint("Left top part of '1'", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "4-4", [_720p] = "2-2" }),
                    new GamePoint("Slightly above the bottom, to skip botom lines of 2, 9 etc", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "11-23", [_720p] = "7-16" }),
                    new GamePoint("To the left of 1", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "6-16", [_720p] = "4-13"}),
                    new GamePoint("To the right of 1", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "17-16", [_720p] = "11-13"}),

                } );

            digit[ ( int )DigitEnum.Digit2 ] = new EndscoreBpDigitGfx( "Endscore Player BP Digit 2",
                false, new List<GamePoint>
                {
                    new GamePoint("Top", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "9-0", [_720p] = "6-0"}),
                    new GamePoint("Left-most part", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "2-7", [_720p] = "1-4"}),
                    new GamePoint("Right-most part on top", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "16-7", [_720p] = "11-5"}),
                    new GamePoint("Left-most part on bottom line", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "3-27", [_720p] = "2-19"}),
                    new GamePoint("Right-most part on bottom line", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "17-27", [_720p] = "12-19"}),
                    new GamePoint("In the center", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "9-9", [_720p] = "6-6"}),
                    new GamePoint("In the bottom right part", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "16-22", [_720p] = "10-15"}),
                } );

            digit[ ( int )DigitEnum.Digit3 ] = new EndscoreBpDigitGfx( "Endscore Player BP Digit 3",
                false, new List<GamePoint>
                {
                    new GamePoint("Top, in the middle", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "10-1", [_720p] = "6-0"}),
                    new GamePoint("Center", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "9-13", [_720p] = "7-9"}),
                    new GamePoint("Bottom, in the middle", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "10-27", [_720p] = "7-19"}),
                    new GamePoint("Left-most part on top", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "2-6", [_720p] = "1-4"}),
                    new GamePoint("Left-most part on bottom", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "2-22", [_720p] = "1-15"}),
                    new GamePoint("Right-most part on top", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "17-7", [_720p] = "11-5"}),
                    new GamePoint("Right-most part on bottom", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "17-20", [_720p] = "11-15"}),
                    new GamePoint("Inside top 'o'", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "10-7", [_720p] = "6-5"}),
                    new GamePoint("Inside bottom 'o'", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "10-20", [_720p] = "7-14"}),
                    new GamePoint("Make sure it's not 8, bottom-left arc", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "3-17", [_720p] = "2-12"}),
                    new GamePoint("Make sure it's not 8, top-left arc ", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "3-10", [_720p] = "2-7"})
                } );

            digit[ ( int )DigitEnum.Digit4 ] = new EndscoreBpDigitGfx( "Endscore Player BP Digit 4",
                false, new List<GamePoint>
                {
                    new GamePoint("Top", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "14-1", [_720p] = "9-0"}),
                    new GamePoint("Bottom", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "14-27", [_720p] = "10-19"}),
                    new GamePoint("Left-most part", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "2-20", [_720p] = "1-14"}),
                    new GamePoint("Right-most part of middle line", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "18-20", [_720p] = "13-14"}),
                    new GamePoint("Intersection of 2 lines", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "14-20", [_720p] = "10-14"}),
                    new GamePoint("Inside '4'", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "10-16", [_720p] = "6-10"}),
                    new GamePoint("Inside bottom-left part", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "6-25", [_720p] = "4-17"}),
                    new GamePoint("Inside bottom-right part", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "18-25", [_720p] = "13-17"}),
                    new GamePoint("Inside top-left part", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "4-4", [_720p] = "2-3"}),
                    new GamePoint("Inside top-right part", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "18-10", [_720p] = "12-5"})
                } );

            digit[ ( int )DigitEnum.Digit5 ] = new EndscoreBpDigitGfx( "Endscore Player BP Digit 5",
                false, new List<GamePoint>
                {
                    new GamePoint("Top line, right-most: make sure it's not 6 or 0", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "17-0", [_720p] = "11-0"}),
                    new GamePoint("Bottom", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "11-27", [_720p] = "8-19"}),
                    new GamePoint("Center", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "11-11", [_720p] = "7-7"}),
                    new GamePoint("Top vertical line, in the cebter", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "4-6", [_720p] = "3-4"}),
                    new GamePoint("Left-most bottom arc, on top of arc", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "3-22", [_720p] = "2-15"}),
                    new GamePoint("Right-most of 'o'", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "18-18", [_720p] = "12-13"}),
                    new GamePoint("Inside 'o'", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "11-18", [_720p] = "7-13"}),
                    new GamePoint("Top part", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "12-5", [_720p] = "8-4"}),
                    new GamePoint("Left-most part of 'o': Make sure it's not 6 or 0", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "2-17", [_720p] = "1-12"}),
                    new GamePoint("Make sure there is no top-right arc of '0'", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "16-5", [_720p] = "11-4"}),
                } );

            digit[ ( int )DigitEnum.Digit6 ] = new EndscoreBpDigitGfx( "Endscore Player BP Digit 6",
                false, new List<GamePoint>
                {
                    new GamePoint("Top", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "11-1", [_720p] = "8-0"}),
                    new GamePoint("Bottom", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "10-27", [_720p] = "7-19"}),
                    // for 720p - intersection of arc and 'o', otherwise vertical line is too thin
                    new GamePoint("Left-most in the middle", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "2-16", [_720p] = "1-12 or 2-12"}),
                    new GamePoint("Right-most in the middle", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "17-19", [_720p] = "12-13"}),
                    new GamePoint("Top of 'o'", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "10-11", [_720p] = "7-7"}),
                    new GamePoint("Top: top-left arc, in its center", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "6-4", [_720p] = "4-3"}),
                    new GamePoint("Top-right", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "17-5", [_720p] = "11-4"}),
                    new GamePoint("Inside 'o'", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "10-19", [_720p] = "7-13"})
                } );

            digit[ ( int )DigitEnum.Digit7 ] = new EndscoreBpDigitGfx( "Endscore Player BP Digit 7",
                false, new List<GamePoint>
                {
                    new GamePoint("Top line, left-most part", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "2-0", [_720p] = "1-0"}),
                    new GamePoint("Diagonal line, bottom third", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "9-20", [_720p] = "6-14"}),
                    new GamePoint("Very bottom of the diagonal line", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "6-27", [_720p] = "4-18"}),
                    new GamePoint("Inside top-left area", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "6-10", [_720p] = "3-8"}),
                    new GamePoint("Inside bottom-right area", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "16-22", [_720p] = "11-15"})
                } );

            digit[ ( int )DigitEnum.Digit8 ] = new EndscoreBpDigitGfx( "Endscore Player BP Digit 8",
                true, new List<GamePoint>
                {
                    new GamePoint("Top", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "10-1", [_720p] = "6-0"}),
                    new GamePoint("Bottom", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "10-27", [_720p] = "6-19"}),
                    new GamePoint("Center", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "10-13", [_720p] = "7-9"}),
                    new GamePoint("Left-most of the top 'o'", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "2-6", [_720p] = "2-4"}),
                    new GamePoint("Left-most at bottom 'o'", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "2-20", [_720p] = "1-14"}),
                    new GamePoint("Right-most at top 'o'", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "17-7", [_720p] = "11-4"}),
                    new GamePoint("Right-most at bottom 'o'", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "17-21", [_720p] = "12-14"}),
                    new GamePoint("Inside top 'o'", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "10-7", [_720p] = "7-5"}),
                    new GamePoint("Inside top 'o'", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "10-21", [_720p] = "6-14"}),
                    new GamePoint("Make sure it's not 3, check missing bottom-left arc", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "3-17", [_720p] = "2-11"}),
                    new GamePoint("Make sure it's not 3, check missing top-left arc of 3", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "3-10", [_720p] = "2-7"})
                } );

            digit[ ( int )DigitEnum.Digit9 ] = new EndscoreBpDigitGfx( "Endscore Player BP Digit 9",
                true, new List<GamePoint>
                {
                    new GamePoint("Top", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "9-1", [_720p] = "7-0"}),
                    new GamePoint("Left-most on bottom", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "6-27", [_720p] = "5-19"}),
                    new GamePoint("Center", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "9-18", [_720p] = "6-12"}),
                    new GamePoint("Left-most of top 'o'", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "1-9", [_720p] = "1-6"}),
                    new GamePoint("Right-most, in the middle", PointColor.FontColor,
                        new Dictionary<string, string> { [_1080p] = "1-9", [_720p] = "11-8"}),
                    new GamePoint("Inside 'o'", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "9-8", [_720p] = "6-6"}),
                    new GamePoint("Inside bottom area", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "8-23", [_720p] = "3-16"}),
                    new GamePoint("Make sure it's not 0: check for lower-left arc of '0'", PointColor.NotFontColor,
                        new Dictionary<string, string> { [_1080p] = "2-21", [_720p] = "2-15"})
                } );

            //verticalPosOfFirstSurvivorBPDigit[ _1080p ] = 306;
            //verticalPosOfFirstSurvivorBPDigit[ _720p ] = 204;


        }

        //public static int getHorizontalDigitOffsetForPlayer(PlayerIndex playerIndex, DigitPosition digitPos )
        //{
        //    // We might have casted it from an int, check if it's inside enum range
        //    Dbg.assertPlayerIndexIsInRangeAndNotInvalid( playerIndex );
        //    Dbg.assertDigitPosEnumIsInRangeAndNotInvalid( digitPos );

        //    string resolution = ScreenCapture.getScreenshotResolutionAsString();

        //    //horizontalDigitOffset[ ( int )DigitPosition.DigitPos0 ] = 661;   // Left-most digit
        //    //horizontalDigitOffset[ ( int )DigitPosition.DigitPos1 ] = 683;
        //    //horizontalDigitOffset[ ( int )DigitPosition.DigitPos2 ] = 715;
        //    //horizontalDigitOffset[ ( int )DigitPosition.DigitPos3 ] = 738;
        //    //horizontalDigitOffset[ ( int )DigitPosition.DigitPos4 ] = 760;   // Right-most digit

        //    switch ( playerIndex )
        //    {
        //        case PlayerIndex.Survivor1:
        //        case PlayerIndex.Survivor2:
        //        case PlayerIndex.Survivor3:
        //        case PlayerIndex.Survivor4:
        //            return horizontalDigitOffset[ resolution ][( int )digitPos ];

        //        case PlayerIndex.Killer:
        //            // All digits except for the first one are moved by 1 pixel for killer
        //            if ( digitPos == DigitPosition.DigitPos0 )
        //                return horizontalDigitOffset[ resolution ][ ( int )digitPos ];

        //            return horizontalDigitOffset[ resolution ][ ( int )digitPos ] + 1;

        //        default:
        //            Dbg.onError( "Invalid player" );
        //            break;
        //    }

        //    Dbg.onError( "Error" );

        //    return 0;
        //}

        /// <summary>
        /// digitIndex is 0 based
        /// </summary>
        public bool recognize( PlayerIndex playerIndex, DigitPosition digitPos, bool bDebug = false )
        {
            // We might have casted it from an int, check if it's inside enum range
            Dbg.assertPlayerIndexIsInRangeAndNotInvalid( playerIndex );
            Dbg.assertDigitPosEnumIsInRangeAndNotInvalid( digitPos );

            string resolution = ScreenCapture.getScreenshotResolutionAsString();

            //int verticalBeginningOfDigits = verticalPosOfFirstSurvivorBPDigit[ resolution ];

            int horizontalOffset = horizontalDigitOffset[ resolution ][ playerIndex ][ ( int )digitPos ];
            int verticalOffset = verticalOffsetBetweenEndscorePlayerBpDigits[resolution][ ( int )playerIndex ];

            //// For killer it is moved 5 pixels down
            //if ( playerIndex == PlayerIndex.Killer )
            //    verticalOffset = verticalOffset + 5;

            return base.recognize( horizontalOffset, verticalOffset, bDebug );
        }

        /// <summary>
        /// Hide the parameter-less method with no offsets, since it makes no sense here
        /// </summary>
        [Obsolete( "", false )]
        new public bool recognize() { return false; }

        /// <summary>
        /// Take player index and index of a digit in BP score and return the digit.
        /// E.g. digit at index '2' from BP score "14 356" will be '3'
        /// </summary>
        public static DigitEnum recognizeDigitAtDigitPosForPlayer( PlayerIndex playerIndex, DigitPosition digitPos )
        {
            // We might have casted it from an int, check if it's inside enum range
            Dbg.assertPlayerIndexIsInRangeAndNotInvalid( playerIndex );
            Dbg.assertDigitPosEnumIsInRangeAndNotInvalid( digitPos );


            List<bool> boolDigitList = new List<bool>();

            // Try to recognize ALL possible digits at that place
            for ( int d = 0; d <= 9; d++ )
            {
                boolDigitList.Add( digit[ d ].recognize( playerIndex, digitPos ) );
            }

            Dbg.ensureMaxOneBoolIsTrue( boolDigitList );

            // see which one is true, if any
            DigitEnum recognizedDigit = DigitEnum.Error;
            for ( int d = (int)DigitEnum.Digit0; d <= (int)DigitEnum.Digit9; d++ )
            {
                // Already set it before, so we have more than 1 value => error
                if ( boolDigitList[ d ] && recognizedDigit != DigitEnum.Error )
                    return DigitEnum.Error;

                if ( boolDigitList[ d ] )
                    recognizedDigit = ( DigitEnum )d;
            }

            Dbg.assertDigitEnumIsInRange( recognizedDigit );

            return recognizedDigit;
        }

        /// <summary>
        /// Recognize DB number digit sequence for a given player
        /// </summary>
        public static List<DigitEnum> recognizeAllDigitsForPlayer( PlayerIndex playerIndex )
        {
            Dbg.assertPlayerIndexIsInRangeAndNotInvalid( playerIndex );

            DigitEnum recognizedDigit = DigitEnum.Error;

            List<DigitEnum> digits = new List<DigitEnum>();

            //string sNumber = "";

            // For all 5 digit positions
            for ( int digitPos = ( int )DigitPosition.DigitPos0; digitPos <= ( int )DigitPosition.DigitPos4; digitPos++ )
            {
                recognizedDigit = recognizeDigitAtDigitPosForPlayer( playerIndex, ( DigitPosition )digitPos );

                Dbg.assertDigitEnumIsInRange( recognizedDigit );

                digits.Add( recognizedDigit );

                //sNumber = sNumber + ( ( int )recognizedDigit ).ToString();
            }

            return digits;
        }

        /// <summary>
        /// Recognize all digits of BP score for a certain player
        /// </summary>
        public static int recognizePlayerBPNumber( PlayerIndex playerIndex )
        {
            Dbg.assertPlayerIndexIsInRangeAndNotInvalid( playerIndex );

            // Get all 5 digits
            var all5Digits = recognizeAllDigitsForPlayer( playerIndex );

            // We recognized them correctly?
            var bSequenceValid = isDigitSequenceNotInterrupting( all5Digits, playerIndex );

            // Construct int from digit sequence
            int resultInt = INVALID_BP_AMOUNT;
            if(bSequenceValid)
                resultInt = getIntFromDigitSequence( all5Digits );

            return resultInt;
        }

        /// <summary>
        /// Convert all digits to an int
        /// </summary>
        public static int getIntFromDigitSequence(List<DigitEnum> digits)
        {
            // Must have 5 digits for BP score
            Dbg.assert( digits.Count == 5 );

            string sNumber = "";

            for (int d = (int)DigitPosition.DigitPos0; d <= (int)DigitPosition.DigitPos4; d++ )
            {
                Dbg.assertDigitEnumIsInRange( digits[ d ] );

                // Skip errors at the beginning, that means there is no digit
                // because the number is small, like " 7365"
                if( digits[ d ] != DigitEnum.Error )
                    sNumber = sNumber + ( ( int )digits[ d ] ).ToString();
            }

            // Error recognizing?
            if ( sNumber.Trim() == "" )
                return INVALID_BP_AMOUNT;

            return int.Parse( sNumber );
        }

        /// <summary>
        /// We can only be missing digits at the beginning (because the number is small),
        /// not in the center, or at the end. E.g. "33333" - valid,
        /// "33_33" - not valid, "3333_" - not valid
        /// </summary>
        public static bool isDigitSequenceNotInterrupting( List<DigitEnum> digits, PlayerIndex playerIndex )
        {
            Dbg.assertPlayerIndexIsInRangeAndNotInvalid( playerIndex );

            // Must have 5 digits for BP score
            Dbg.assert( digits.Count == 5 );

            bool bSequenceStarted = false;
            DigitEnum firstDigit = DigitEnum.Error;
            DigitPosition firstDigitIndex  = DigitPosition.Error;

            // From left to right - For all digits in list see if sequence interrupts
            for ( int d = ( int )DigitPosition.DigitPos0; d <= (int)DigitPosition.DigitPos4; d++ )
            {
                Dbg.assertDigitEnumIsInRange( digits[ d ] );

                // We previously found a valid digit, must be followed by digits until the end
                // If it's not then we have an error
                if( bSequenceStarted && digits[ d ] == DigitEnum.Error )
                {
                    //Dbg.onError( "Wrong BP digit sequence at pos: " + d.ToString() +
                    //    " for player: " + playerIndex.ToString() );

                    return false;
                }

                // Did we find the first valid digit?
                if ( !bSequenceStarted && digits[ d ] != DigitEnum.Error )
                {
                    firstDigit = digits[ d ];
                    firstDigitIndex = ( DigitPosition )d;
                    bSequenceStarted = true;
                }
            }

            // We can only have '0' at the beginning if it's the last digit (usually on DC)
            if(firstDigit == DigitEnum.Digit0)
            {
                if ( firstDigitIndex != DigitPosition.DigitPos4 )
                    return false;
            }

            return true;
        }

        private EndscoreBpDigitGfx( string name, bool bDebug, List<GamePoint> gamePoints ):
            base( name, bDebug, gamePoints )
        {

        }

    }
}
