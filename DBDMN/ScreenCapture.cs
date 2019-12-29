using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Threading;
using System.Diagnostics;
using System.Linq;

namespace DBDMN
{
	/// <summary>
	/// Provides functions to capture the entire screen, or a particular window, and save it to a file.
	/// </summary>
	public class ScreenCapture
	{
		private static Bitmap lastScreenShot = null;

		// or ( uint )0 to capture all window
		private static int PW_CLIENTONLY = 0x1;

		public static IntPtr gameHWND = IntPtr.Zero;

        public static string getWindowTitle(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return null;

            var sWindowTitle = User32.GetWindowTitle(hwnd);
            if (sWindowTitle == null)
                return null;

            return sWindowTitle;
        }

        public static int getClientWindowWidth(IntPtr firefoxHwnd)
		{
            if (firefoxHwnd == IntPtr.Zero)
                return -1;

            ScreenCapture.User32.RECT rct = new ScreenCapture.User32.RECT();
            ScreenCapture.User32.GetClientRect(firefoxHwnd, ref rct);

            return rct.right;
		}

		public static int getClientWindowHeight(IntPtr firefoxHwnd)
		{
            if (firefoxHwnd == IntPtr.Zero)
                return -1;

            ScreenCapture.User32.RECT rct = new ScreenCapture.User32.RECT();
            ScreenCapture.User32.GetClientRect(firefoxHwnd, ref rct);

            return rct.bottom;
        }

		public static int getWindowHeight(IntPtr hwnd)
		{
			ScreenCapture.User32.RECT rct = new ScreenCapture.User32.RECT();
			ScreenCapture.User32.GetClientRect( hwnd, ref rct );

			return rct.bottom;
		}

		public static int getWindowWidth( IntPtr hwnd )
		{
			ScreenCapture.User32.RECT cientRect = new ScreenCapture.User32.RECT();
			ScreenCapture.User32.GetClientRect( hwnd, ref cientRect );

            ScreenCapture.User32.RECT windowRect = new ScreenCapture.User32.RECT();
            ScreenCapture.User32.GetWindowRect(hwnd, ref windowRect);

            return cientRect.right;
		}

		public static IntPtr findWindow(string lpClassName, string lpWindowName)
		{
			return User32.FindWindow(lpClassName, lpWindowName);
		}

		public static bool haveScreenShot()
		{
			return lastScreenShot != null;
		}

        public static Bitmap getScreenShot()
        {
            return lastScreenShot;
        }

        /// <summary>
        /// Get "1920x1080"
        /// </summary>
        public static string getScreenshotResolutionAsString()
        {
            if ( !haveScreenShot() )
                return null;

            return getScreenShot().Width + "x" + getScreenShot().Height;
        }

        public static void setScreenShot(Bitmap b)
        {
            lastScreenShot = b;
        }

		public static Color getScreenShotPointColor(int x, int y)
		{
			return lastScreenShot.GetPixel( x, y );
		}

		public static bool isColorOK(Color c)
		{
			if( c.R == 0 && c.G == 0 && c.B == 0 )
				return false;

			return true;
		}

        public static bool isDBDWindowFocused()
        {
            //var gameHwnd = ScreenCapture.gameHWND;

            IntPtr activeWindowHwnd = ScreenCapture.User32.GetForegroundWindow();

            if (activeWindowHwnd == IntPtr.Zero)
                return false;

            string sActiveWindowTitle = ScreenCapture.getWindowTitle(activeWindowHwnd);

            if (sActiveWindowTitle == null)
                return false;

            var sWindowClassName = User32.GetClassNameFromHandle(activeWindowHwnd);
            if (sWindowClassName == null)
                return false;

            return sActiveWindowTitle.ToUpperInvariant().Trim() == "DeadByDaylight".ToUpperInvariant() &&
                sWindowClassName.ToUpper() == "UnrealWindow".ToUpperInvariant();
        }

        /// <summary>
        /// Bring window to front
        /// </summary>
        public static void activateGame()
        {
            if (haveGameHwnd())
            {
                Log.log("activateGame()");

                User32.SetForegroundWindow(getGameHwnd());
            }
        }

		public static bool haveGameHwnd()
		{
			return gameHWND != IntPtr.Zero;
		}


		private static string _findAllWindowClass = null;
		private static string _findAllWindowTitle = null;
        private static HashSet<IntPtr> windowHandles = new HashSet<IntPtr>();
		public static HashSet<IntPtr> findAllWindowsByClass( string sClass, string sWindowCaption = null )
		{
			try
			{
				_findAllWindowClass = sClass.Trim();

                if(sWindowCaption != null)
                    _findAllWindowTitle = sWindowCaption.Trim();

                windowHandles.Clear();
				User32.EnumWindowsProc callBackPtr = GetWindowHandle;
				User32.EnumWindows( callBackPtr, IntPtr.Zero );
			}
			catch( Exception e )
			{
				Log.logError( "findAllWindowsByClass: " + e.ToString() );
			}

			return windowHandles;   
		}
		
		private static bool GetWindowHandle( IntPtr windowHandle, IntPtr lParam )
		{
			try
			{
				var sWindowClassName = User32.GetClassNameFromHandle( windowHandle );
				if( sWindowClassName.ToUpperInvariant() ==
					_findAllWindowClass.ToUpperInvariant() )
				{
                    var sWindowTitle = User32.GetWindowTitle(windowHandle).Trim();

                    if (_findAllWindowTitle == null ||
                        (sWindowTitle.ToUpperInvariant() == _findAllWindowTitle.ToUpperInvariant()))
                    {
                        windowHandles.Add(windowHandle);
                    }
				}
			}
			catch( Exception e )
			{
				//Log.logError( "GetWindowHandle: " + e.ToString() );
			}

			return true;
		}

		public static bool updateScreenshotFromGame(IntPtr hWnd, int x, int y)
		{
			bool success = false;

			if( hWnd != IntPtr.Zero )
			{
				gameHWND = hWnd;

				success = User32.CaptureWindowBitmap( hWnd, x, y );
				//CaptureWindowToFile( hWnd_Flash, @"c:\temp\a.jpg", ImageFormat.Bmp );

				Int32 errr = Marshal.GetLastWin32Error();

				return success;
			}

			return success;
		}

        public static Image debugImageFromFile = null;

        public static bool haveDebugPicture()
        {
            return debugImageFromFile != null;
        }

        public static void setDebugImageFile(string pngFilePath)
        {
            debugImageFromFile = Image.FromFile(pngFilePath.Trim());

            ScreenCapture.makeGameScreenshot();
        }

        /// <summary>
        /// DBD has resolution of 1344x714 when it starts fora second, ignore it
        /// </summary>
        public static bool isDBD_1344x714_InvalidStartupImage()
        {
            if ( !haveGameHwnd() )
                return false;

            var b = ScreenCapture.getScreenShot();
            
            var black = Color.FromArgb( 0, 0, 0 );

            if(b.Width == 1344 && b.Height == 714)
            {
                // Check if most pixels are black
                for(int x = 0; x < b.Width; x++ )
                {
                    for(int y = 0; y < b.Height; y++ )
                    {
                        if ( b.GetPixel( x, y ) != black )
                            return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static bool isCurrentGameResolutionSupported()
        {
            var currentResolution = getScreenshotResolutionAsString();

            if ( !Gfx.supportedResolutions.Contains( currentResolution ) )
            {
                string sListOfSupportedResolutions = String.Join( ", ", Gfx.supportedResolutions );

                MessageBox.Show( "Game resolution (" + currentResolution + ") is not supported. \n\n" +
                    "Supported resolutions: \n" + sListOfSupportedResolutions );

                return false;
            }

            // Window mode?
            if(ScreenCapture.isDBDInWindowMode())
            {
                MessageBox.Show( "DBD in windowed mode is not supported" );

                return false;
            }

            return true;
        }

        public static bool makeGameScreenshot()
        {
            gameHWND = IntPtr.Zero;

            if (debugImageFromFile != null)
            {
                ScreenCapture.setScreenShot(new Bitmap(debugImageFromFile));
                return true;
            }

            var dbdWindows = ScreenCapture.findAllWindowsByClass("UnrealWindow", "DeadByDaylight");

            // Can only have 1 valid DBD instance
            if(dbdWindows.Count == 0 || dbdWindows.Count > 1)
            {
                return false;
            }

            gameHWND = dbdWindows.First();

            int width = ScreenCapture.getWindowWidth(gameHWND);
            int height = ScreenCapture.getWindowHeight(gameHWND);

            return ScreenCapture.updateScreenshotFromGame(gameHWND, width, height);
        }

        public static IntPtr getGameHwnd()
        {
            return gameHWND;
        }

        public static IntPtr getWindowStyles(IntPtr hwnd)
        {
            IntPtr result = User32.GetWindowLongPtr(hwnd, User32.GWL.GWL_STYLE);

            return result;
        }

        public static bool isDBDInWindowMode()
        {
            if (!haveGameHwnd())
                return false;

            var styles = (uint)getWindowStyles(getGameHwnd());

            // has caption = windowed mode
            return ((styles & (uint)User32.WindowStyles.WS_CAPTION) ==
                (uint)User32.WindowStyles.WS_CAPTION);
        }

        
		/// <summary>
		/// Helper class containing User32 API functions
		/// </summary>
		public class User32
		{
			[StructLayout(LayoutKind.Sequential)]
			public struct RECT
			{
				public int left;
				public int top;
				public int right;
				public int bottom;
			}

			[DllImport("user32.dll")]
			public static extern IntPtr GetDesktopWindow();
			[DllImport("user32.dll")]
			public static extern IntPtr GetWindowDC(IntPtr hWnd);
			[DllImport("user32.dll")]
			public static extern IntPtr ReleaseDC(IntPtr hWnd,IntPtr hDC);
			[DllImport("user32.dll")]
			public static extern IntPtr GetWindowRect(IntPtr hWnd,ref RECT rect);
			[DllImport( "user32.dll" )]
			public static extern IntPtr GetClientRect( IntPtr hWnd, ref RECT rect );
			[DllImport( "user32.dll", SetLastError = true )]
			public static extern IntPtr FindWindow( string lpClassName, string lpWindowName );

			public delegate bool EnumWindowsProc( IntPtr hWnd, IntPtr lParam );

			[DllImport( "user32.dll" )]
			[return: MarshalAs( UnmanagedType.Bool )]
			public static extern bool EnumWindows( EnumWindowsProc lpEnumFunc, IntPtr lParam );

			[DllImport( "user32" )]
			[return: MarshalAs( UnmanagedType.Bool )]
			public static extern bool EnumChildWindows( IntPtr window, EnumWindowProc callback, IntPtr i );

			[DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
			static extern int GetWindowText( IntPtr hWnd, StringBuilder lpString, int nMaxCount );

			/// <summary>
			/// Returns a list of child windows
			/// </summary>
			/// <param name="parent">Parent of the windows to return</param>
			/// <returns>List of child windows</returns>
			public static List<IntPtr> GetChildWindows( IntPtr parent )
			{
				List<IntPtr> result = new List<IntPtr>();
				GCHandle listHandle = GCHandle.Alloc( result );
				try
				{
					EnumWindowProc childProc = new EnumWindowProc( EnumWindow );
					EnumChildWindows( parent, childProc, GCHandle.ToIntPtr( listHandle ) );
				}
				finally
				{
					if( listHandle.IsAllocated )
						listHandle.Free();
				}
				return result;
			}


            /// <summary>
            /// Callback method to be used when enumerating windows.
            /// </summary>
            /// <param name="handle">Handle of the next window</param>
            /// <param name="pointer">Pointer to a GCHandle that holds a reference to the list to fill</param>
            /// <returns>True to continue the enumeration, false to bail</returns>
            private static bool EnumWindow( IntPtr handle, IntPtr pointer )
			{
				GCHandle gch = GCHandle.FromIntPtr( pointer );
				List<IntPtr> list = gch.Target as List<IntPtr>;
				if( list == null )
				{
					throw new InvalidCastException( "GCHandle Target could not be cast as List<IntPtr>" );
				}
				list.Add( handle );
				//  You can modify this to check to see if you want to cancel the operation, then return a null here
				return true;
			}

			/// <summary>
			/// Delegate for the EnumChildWindows method
			/// </summary>
			/// <param name="hWnd">Window handle</param>
			/// <param name="parameter">Caller-defined variable; we use it for a pointer to our list</param>
			/// <returns>True to continue enumerating, false to bail.</returns>
			public delegate bool EnumWindowProc( IntPtr hWnd, IntPtr parameter );
            
			[DllImport( "user32.dll" )]
			public static extern bool IsWindowVisible( IntPtr hWnd );

			[DllImport( "user32.dll" )]
			[return: MarshalAs( UnmanagedType.Bool )]
			public static extern bool IsWindow( IntPtr hWnd );

			[DllImport( "kernel32.dll" )]
			public static extern int GetProcessId( IntPtr handle );

			// get process from window handle
			[DllImport( "user32.dll", SetLastError = true )]
			public static extern uint GetWindowThreadProcessId( IntPtr hWnd, out uint lpdwProcessId );

			[DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Auto )]
			static extern int GetWindowTextLength( IntPtr hWnd );

			public static string GetWindowTitle( IntPtr hWnd )
			{
				try
				{
					// Allocate correct string length first
					int length = GetWindowTextLength( hWnd );
					StringBuilder sb = new StringBuilder( length + 1 );
					GetWindowText( hWnd, sb, sb.Capacity );
					return sb.ToString();
				}
				catch( Exception e )
				{
					//Log.logError( "GetWindowTitle: " + e.ToString() );
				}

				return null;
			}


			[DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Auto )]
			static extern int GetClassName( IntPtr hWnd, StringBuilder lpClassName, int nMaxCount );

			[DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
			public static extern IntPtr FindWindowEx( IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle );

			public static string GetClassNameFromHandle( IntPtr hWnd )
			{
				try
				{
					int nRet;
					StringBuilder ClassName = new StringBuilder( 100 );
					//Get the window class name
					nRet = GetClassName( hWnd, ClassName, ClassName.Capacity );

                    return ClassName.ToString();
				}
				catch( Exception e )
				{
					//Log.logError( "GetClassNameFromHandle: " + e.ToString() );
				}

				return null;
			}


			[DllImport( "user32.dll" )]
			public static extern bool PrintWindow( IntPtr hwnd, IntPtr hdcBlt, uint nFlags );

			static Graphics captureGrfx;
			static Graphics captureGraphics;
			static IntPtr capture_hDC;

			public static bool CaptureWindowBitmap( IntPtr hWnd,
				int x, int y)
			{
				// 2nd method:  System.Drawing.Graphics -> CopyFromScreen


				Rectangle rctForm = Rectangle.Empty;

				try
				{
					using( captureGrfx = Graphics.FromHdc( GetWindowDC( hWnd ) ) )
					{
						rctForm = Rectangle.Round( captureGrfx.VisibleClipBounds );
                    }
                }
				catch( Exception )
				{
					//Log.logError( "CaptureWindowBitmap: FromHdc() failed. " +
					//	Environment.NewLine + "Hwnd: " + hWnd.ToString() );
					return false;
				}

				// prevent an error
				if( rctForm.Width == 0 || rctForm.Height == 0 )
					return false;

				try
				{
                    // First time or game resolution has changed
                    if (lastScreenShot == null || 
                        lastScreenShot.Width != x || lastScreenShot.Height != y)
                    {
                        lastScreenShot = new Bitmap(x, y);
                    }

					captureGraphics = Graphics.FromImage( lastScreenShot );
                }
				catch( Exception )
				{
					//MessageBox.Show( "CaptureWindowBitmap: FromImage() failed. " +
					//    Environment.NewLine + "Hwnd: " + hWnd.ToString() );
					return false;
				}

				try
				{
					capture_hDC = captureGraphics.GetHdc();
				}
				catch( Exception )
				{
					//MessageBox.Show( "CaptureWindowBitmap: GetHdc() failed. " +
					//    Environment.NewLine + "Hwnd: " + hWnd.ToString() );
					return false;
				}

				//paint control onto graphics using provided options
				try
				{
					PrintWindow( hWnd, capture_hDC, ( uint )PW_CLIENTONLY );
				}
				finally
				{
					captureGraphics.ReleaseHdc( capture_hDC );
					//captureGraphics.Dispose();
				}

				return true;
			}

			public static System.Drawing.Bitmap CaptureWindowBitmapB( IntPtr hWnd )
			{
				Bitmap bmp = new Bitmap( 2000, 1500 );
				Graphics g = Graphics.FromImage( bmp );
				IntPtr dc = g.GetHdc();

				PrintWindow( hWnd, dc,(uint) PW_CLIENTONLY );

				g.ReleaseHdc();
				g.Dispose();

				return bmp;
			}

			[DllImport( "user32.dll" )]
			public static extern bool InvalidateRect( IntPtr hWnd, IntPtr lpRect, bool bErase );


			[System.Runtime.InteropServices.DllImport( "user32.dll" )]
			public static extern IntPtr WindowFromPoint( Point pnt );

			[StructLayout( LayoutKind.Sequential )]
			public struct POINT
			{
				public int X;
				public int Y;

				public POINT( int x, int y )
				{
					this.X = x;
					this.Y = y;
				}

				public POINT( System.Drawing.Point pt ) : this( pt.X, pt.Y ) { }

				public static implicit operator System.Drawing.Point( POINT p )
				{
					return new System.Drawing.Point( p.X, p.Y );
				}

				public static implicit operator POINT( System.Drawing.Point p )
				{
					return new POINT( p.X, p.Y );
				}
			}

			[DllImport( "user32.dll" )]
			public static extern bool ScreenToClient( IntPtr hWnd, ref POINT lpPoint );

			[DllImport( "user32.dll" )]
			public static extern IntPtr SetCapture( IntPtr hWnd );

			[DllImport( "user32.dll" )]
			public static extern bool ReleaseCapture();

			[DllImport( "user32.dll" )]
			public static extern bool ClientToScreen( IntPtr hWnd, ref POINT lpPoint );

			[DllImport( "user32.dll" )]
			public static extern IntPtr GetForegroundWindow();

			//[DllImport( "user32.dll", SetLastError = true )]
			//static public extern uint GetWindowThreadProcessId( IntPtr hWnd, out uint lpdwProcessId );

			[DllImport( "user32.dll" )]
			public static extern bool FlashWindow( IntPtr hwnd, bool bInvert );

			[DllImport( "user32.dll" )]
			[return: MarshalAs( UnmanagedType.Bool )]
			public static extern bool SetForegroundWindow( IntPtr hWnd );


            [DllImport("user32.dll")]
            public static extern int GetDpiForWindow(IntPtr hWnd);

            /// <summary>
            /// For window styles
            /// </summary>
            public enum GWL
            {
                GWL_WNDPROC = (-4),
                GWL_HINSTANCE = (-6),
                GWL_HWNDPARENT = (-8),
                GWL_STYLE = (-16),
                GWL_EXSTYLE = (-20),
                GWL_USERDATA = (-21),
                GWL_ID = (-12)
            }

            /// <summary>
            /// For window styles
            /// </summary>
            [Flags]
            public enum WindowStyles : uint
            {
                WS_BORDER = 0x800000,
                WS_CAPTION = 0xc00000,
                WS_CHILD = 0x40000000,
                WS_CLIPCHILDREN = 0x2000000,
                WS_CLIPSIBLINGS = 0x4000000,
                WS_DISABLED = 0x8000000,
                WS_DLGFRAME = 0x400000,
                WS_GROUP = 0x20000,
                WS_HSCROLL = 0x100000,
                WS_MAXIMIZE = 0x1000000,
                WS_MAXIMIZEBOX = 0x10000,
                WS_MINIMIZE = 0x20000000,
                WS_MINIMIZEBOX = 0x20000,
                WS_OVERLAPPED = 0x0,
                WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
                WS_POPUP = 0x80000000u,
                WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
                WS_SIZEFRAME = 0x40000,
                WS_SYSMENU = 0x80000,
                WS_TABSTOP = 0x10000,
                WS_VISIBLE = 0x10000000,
                WS_VSCROLL = 0x200000
            }

            /// <summary>
            /// For window styles
            /// </summary>
            [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
            public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, GWL nIndex);
        }


	}
}
