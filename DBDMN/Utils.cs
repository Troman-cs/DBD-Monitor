using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDMN
{
    public static class Utils
    {
        /// <summary>
        /// "C:\\somepath\\somepath"
        /// </summary>
        public static string getAppPath()
        {
            return System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location );
        }

        /// <summary>
        /// Format: dd.mm.yyyy
        /// </summary>
        public static string getCurrentDateAsString( DateTime date )
        {
            return date.Day + "." + date.Month + "." + date.Year;
        }

        /// <summary>
        /// Date format: dd.mm.yyyy
        /// </summary>
        public static DateTime parseDateFromDdMmYyyyString( string date )
        {
            if ( date == null )
                return DateTime.MinValue;

            date = date.Trim();
            if ( date == "" )
                return DateTime.MinValue;

            // Split by '.'
            var values = date.Split( '.' );

            Dbg.assert( values.Length == 3, "Wrong date format: " + date );

            int day = int.Parse( values[ 0 ].Trim() );
            int month = int.Parse( values[ 1 ].Trim() );
            int year = int.Parse( values[ 2 ].Trim() );

            return new DateTime( year, month, day );

            // return DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year;
        }
    }

}
