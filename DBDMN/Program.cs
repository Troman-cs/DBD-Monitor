using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBDMN
{
    static class Program
    {
        private static Mutex mutex = null;

        private static bool createdNew;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            mutex = new Mutex( true, Form1.title, out createdNew );
            if ( !createdNew )
            {
                MessageBox.Show( "Program already running." );
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
