using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDMN
{
    public class GamePoint
    {
        /// <summary>
        /// Same Point for different game resolutions
        /// </summary>
        private Dictionary<string, List<Point>> point;

        /// <summary>
        /// Which color should this point be?
        /// </summary>
        private PointColor pointColor = PointColor.Error;

        private string pointDescription = null;

        public enum PointColor
        { BrightnessMax15,    // Brightness of r and b and b <= 15
            Black,
            PitchBlack,
            DarkBlue,
            Red,
            DarkAndNotRed,
            White,
            NotWhite,
            FontColor,
            NotFontColor,
            DbdLogoColor,   // Big Black-White DBD logo on the loading screen
            DarkRed,
            DarkGrey,
            VeryDarkGrey,
            DarkOrVeryDarkGrey,
            BrighterThanDark,   // 100-200
            WillProvidLater,    // A dynamic color, will be provided during recognition call
            Default,
            Error }

        public GamePoint(string pointDescription, PointColor pointColor, Dictionary<string, string> points)
        {
            this.point = new Dictionary<string, List<Point>>();

            // Convert from string to Point()
            foreach(var kvp in points)
                this.point.Add( kvp.Key.Trim(), getPointFromString( kvp.Value ) );

            this.pointDescription = pointDescription.Trim();

            Debug.Assert( pointColor != PointColor.Error );

            this.pointColor = pointColor;
        }

        public string getDescription() => this.pointDescription;

        /// <summary>
        /// Get poinr from string "x-y"
        /// </summary>
        public static List<Point> getPointFromString(string p)
        {
            p = p.ToLower().Trim();

            var resultPoints = new List<Point>();

            var alternatePoints = p.Split( new string[] { "or" }, StringSplitOptions.RemoveEmptyEntries );

            // For all "or" points get their x-y
            foreach ( var alternatePoint in alternatePoints )
            {
                var v = alternatePoint.Split( new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries );

                int x = int.Parse( v[ 0 ].Trim() );
                int y = int.Parse( v[ 1 ].Trim() );

                Debug.Assert( x >= 0 && y >= 0 && x < 5000 && y < 5000 );

                resultPoints.Add( new Point( x, y ) );
            }

            return resultPoints;
        }

        public List<Point> getAlternatePoint(string resolution)
        {
            if(!this.point.ContainsKey( resolution ))
                Dbg.saveErrorImageToFile();

            // TODO: Sometimes get 1344x714 at startup
            return this.point[ resolution ];
        }

        public PointColor getPointColor()
        {
            return this.pointColor;
        }

        
    }
}
