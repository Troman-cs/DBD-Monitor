using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace DBDMN
{
	public static class Log
	{
		static private StreamWriter mainLog = null;

		static private StreamWriter errorLog = null;

		static private bool bDisabled = false;
		

		public static void disable()
		{
			bDisabled = false;
		}

		public static void logError( string s )
		{

            throw new Exception(s);
            try
			{
				//Form1.instance.addDebugStatus( s );

				if( bDisabled )
					return;

				if( errorLog == null )
				{
					errorLog = new StreamWriter( Path.GetDirectoryName(
						Application.ExecutablePath ) + "\\errorlog.txt", true );
				}

				string date = DateTime.Now.ToString() + ": ";

                string thread = Thread.CurrentThread.ManagedThreadId.ToString();


                errorLog.WriteLine( date + s + " (" + thread + ")");

				errorLog.Flush();

				
			}
			catch( Exception ) { }

            
        }

        public static void log(string s, bool bTimestamp = true)
        {
            if (bDisabled)
                return;

            string result = s;

            if (bTimestamp)
                result = DateTime.Now.ToString("h:mm:ss tt") + ": " + result;

            Console.WriteLine(result);
        }

		public static void logToFile(string s)
		{
			if( bDisabled )
				return;

			try
			{
				if( mainLog == null )
				{
					mainLog = new StreamWriter( Path.GetDirectoryName(
						Application.ExecutablePath ) + "\\log.txt", true );
				}

				string date = DateTime.Now.ToString() + ": ";

				mainLog.WriteLine( date + s );

				mainLog.Flush();
			}
			catch( Exception ) { }
		}

	}
}
