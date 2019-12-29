using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DBDMN.EndscoreBpDigitGfx;
using static DBDMN.ScreenParser;

namespace DBDMN
{
    public static class Dbg
    {
        public static bool bDebug = true;
        public static bool bErrorMsg = false;
        public static bool bTest = false;
        

        public static void assert(bool bMustBeTrue, string msg = "Error")
        {
            if ( !bMustBeTrue )
                onDebugError( msg );
        }

        public static void showMsg( string msg )
        {
            // Only one at a time
            if ( bErrorMsg )
                return;

            // stop showing multiple assert error messages
            bErrorMsg = true;

            saveErrorImageToFile();

            MessageBox.Show( msg );

            bErrorMsg = false;
        }

        public static void onDebugError( string msg )
        {
            // Only one at a time
            if ( bErrorMsg )
                return;

            // stop showing multiple assert error messages
            bErrorMsg = true;

            saveErrorImageToFile();

            Debug.Assert( false, msg );

            bErrorMsg = false;
        }

        public static void ensureMaxOneBoolIsTrue( List<bool> boolList )
        {
            // make sure only one boolean is true
            int numTrue = getNumOfBoolIsTrue( boolList );

            assert( numTrue <= 1, "More than 1 bool is true" );
        }

        public static void ensureExactlyOneBoolIsTrue( List<bool> boolList )
        {
            // make sure only one boolean is true
            int numTrue = getNumOfBoolIsTrue( boolList );

            assert( numTrue == 1, "Not exactly 1 bool is true" );
        }
        private static int getNumOfBoolIsTrue( List<bool> boolList )
        {
            // make sure only one boolean is true
            int numTrue = 0;
            foreach ( bool boolean in boolList )
            {
                if ( boolean )
                    numTrue++;
            }

            return numTrue;
        }

        public static void saveErrorImageToFile()
        {
            string errorPath = Utils.getAppPath() + "\\error";

            try
            {
                if( !Directory.Exists( errorPath ) )
                    Directory.CreateDirectory( errorPath );

                Bitmap b = new Bitmap( ScreenCapture.getScreenShot() );

                b.Save( errorPath+ "\\error.jpg" );
            }
            catch ( Exception e )
            {
            }

        }

        public static void initialDiagnostics()
        {
            PlayerIndex survivor1 = PlayerIndex.Survivor1;
            assert( ( int )survivor1 == 0, "PlayerIndex.Survivor1 must be 0" );

            PlayerIndex killer = PlayerIndex.Killer;
            assert( ( int )killer == 4, "PlayerIndex.Killer must be 4" );

            assert( ( ( int )EndscoreBpDigitGfx.DigitEnum.Error ) == 10 );
            assert( ( ( int )EndscoreBpDigitGfx.DigitPosition.Error ) == 5 );

            // Check digit sequences for interruption-detection
            var invalidDigitSequence1 = new List<DigitEnum> { DigitEnum.Digit1, DigitEnum.Digit2,
                DigitEnum.Error, DigitEnum.Digit9, DigitEnum.Digit8 };
            var invalidDigitSequence2 = new List<DigitEnum> { DigitEnum.Digit1, DigitEnum.Digit2,
                DigitEnum.Digit9, DigitEnum.Digit8, DigitEnum.Error };
            var invalidDigitSequence3 = new List<DigitEnum> { DigitEnum.Digit0, DigitEnum.Digit2,
                DigitEnum.Digit9, DigitEnum.Digit8, DigitEnum.Error };


            assert( !EndscoreBpDigitGfx.isDigitSequenceNotInterrupting( invalidDigitSequence1, 
                PlayerIndex.Survivor1 ) );
            assert( !EndscoreBpDigitGfx.isDigitSequenceNotInterrupting( invalidDigitSequence2, 
                PlayerIndex.Survivor1 ) );
            assert( !EndscoreBpDigitGfx.isDigitSequenceNotInterrupting( invalidDigitSequence3,
                PlayerIndex.Survivor1 ) );

            // Valid sequences
            var validDigitSequence1 = new List<DigitEnum> { DigitEnum.Error, DigitEnum.Digit2,
                DigitEnum.Digit9, DigitEnum.Digit8, DigitEnum.Digit8 };
            var validDigitSequence2 = new List<DigitEnum> { DigitEnum.Error, DigitEnum.Error,
                DigitEnum.Error, DigitEnum.Error, DigitEnum.Digit0 };

            assert( EndscoreBpDigitGfx.isDigitSequenceNotInterrupting( validDigitSequence1,
                PlayerIndex.Survivor1 ) );
            assert( EndscoreBpDigitGfx.isDigitSequenceNotInterrupting( validDigitSequence2,
                PlayerIndex.Survivor1 ) );
        }

        public static void assertIsValidSurvivorIndex(int survivorIndex)
        {
            assertIndexIsWithinRange( survivorIndex, 0, 3 );
        }

        public static void assertIndexIsWithinRange( int index, int lowestAcceptable, int higherstAcceptable )
        {
            Dbg.assert( index >= lowestAcceptable, "Index too low" );
            Dbg.assert( index <= higherstAcceptable, "Index too high" );
        }

        public static void assertPlayerIndexIsInRangeAndNotInvalid(PlayerIndex playerIndex)
        {
            assert( Enum.IsDefined( typeof( PlayerIndex ), ( int )playerIndex ) );
            assert( playerIndex != PlayerIndex.Error );
        }

        public static void assertDigitEnumIsInRangeAndNotInvalid( DigitEnum digit )
        {
            assertDigitEnumIsInRange( digit );
            assert( digit != DigitEnum.Error );
        }

        public static void assertDigitEnumIsInRange( DigitEnum digit )
        {
            assert( Enum.IsDefined( typeof( DigitEnum ), ( int )digit ) );
        }

        public static void assertDigitPosEnumIsInRangeAndNotInvalid( DigitPosition digitPos )
        {
            assert( Enum.IsDefined( typeof( DigitPosition ), ( int )digitPos ) );
            assert( digitPos != DigitPosition.Error );
        }
    }
}
