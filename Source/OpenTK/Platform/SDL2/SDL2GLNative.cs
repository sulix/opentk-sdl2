#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2009 the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
#if !MINIMAL
using System.Drawing;
#endif
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Input;

namespace OpenTK.Platform.SDL2
{
    internal sealed class SDL2GLNative : INativeWindow, IDisposable
    {
        #region Fields
        
        const int _min_width = 30, _min_height = 30;

        SDL2WindowInfo window = new SDL2WindowInfo();

		private bool isFullscreen = false;

        // Legacy input support
        //MouseDevice mouse;
        //readonly KeyPressEventArgs KPEventArgs = new KeyPressEventArgs('\0');

		private SDL2Input inputDriver;

        public static bool MouseWarpActive = false;

        #endregion

        #region Constructors

        public SDL2GLNative(int x, int y, int width, int height, string title,
            GraphicsMode mode,GameWindowFlags options, DisplayDevice device)
            : this()
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", "Must be higher than zero.");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", "Must be higher than zero.");


            Debug.Indent();

			IntPtr windowId;
			isFullscreen = options.HasFlag(GameWindowFlags.Fullscreen);
			lock (API.sdl_api_lock) {
				API.Init (API.INIT_VIDEO);
				API.VideoInit("",0);
				// NOTE: Seriously, letting the user set x and y coords is a _bad_ idea. We'll let the WM take care of it.
				windowId = API.CreateWindow(title, 0x1FFF0000, 0x1FFF0000, width, height, API.WindowFlags.OpenGL | ((isFullscreen)?API.WindowFlags.Fullscreen:0));
			}
			window = new SDL2WindowInfo(windowId);

			inputDriver = new SDL2Input(window);
            Debug.Unindent();

        }

        /// <summary>
        /// Constructs and initializes a new X11GLNative window.
        /// Call CreateWindow to create the actual render window.
        /// </summary>
        public SDL2GLNative()
        {
        }

        #endregion



        #region IsWindowBorderResizable

        bool IsWindowBorderResizable
        {
            get
            {
				bool isResizable;
				lock (API.sdl_api_lock) {
					isResizable = ((API.GetWindowFlags(window.WindowHandle) & SDL2.API.WindowFlags.Resizable) != 0);
				}
				return isResizable;
            }
        }

        #endregion
                
        #region bool IsWindowBorderHidden
                
        bool IsWindowBorderHidden
        {
            get
            {    
				bool isBorderless;
				lock (API.sdl_api_lock) {
					isBorderless = ((API.GetWindowFlags(window.WindowHandle) & SDL2.API.WindowFlags.Borderless) != 0);
				}
				return isBorderless;
            }
        }
                
        #endregion

        #region void DisableWindowDecorations()

        void DisableWindowDecorations ()
		{
			lock (API.sdl_api_lock) {
				API.SetWindowBordered (window.WindowHandle, false);
			}
        }
        

		#endregion


        #region void EnableWindowDecorations()

        void EnableWindowDecorations ()
		{
			lock (API.sdl_api_lock) {
				API.SetWindowBordered (window.WindowHandle, true);
			}
        }

		#endregion
        
        #region INativeWindow Members

        #region ProcessEvents

        public void ProcessEvents ()
		{
			API.Event currentEvent;
			int finished = 0;
			while (finished == 0) {
				lock (API.sdl_api_lock)
				{
					finished = 1 - API.PollEvent(out currentEvent);
				}
				if (finished != 0) break;
				switch (currentEvent.type) {
				case API.EventType.Quit:
					CancelEventArgs ceargs = new CancelEventArgs();
					Closing (this, ceargs);
					if (!ceargs.Cancel)
					{
						DestroyWindow();
						Closed(this, new EventArgs());
					}
					break;
				case API.EventType.MouseButtonDown:
				case API.EventType.MouseButtonUp:
				case API.EventType.MouseMotion:
				case API.EventType.MouseWheel:
					inputDriver.ProcessEvent(ref currentEvent);
					if (SDL2Mouse.newestMouse != null) {
						SDL2Mouse.newestMouse.ProcessEvent(ref currentEvent);
					}
					break;
				case API.EventType.KeyDown:
				case API.EventType.KeyUp:
					inputDriver.ProcessEvent(ref currentEvent);
					if (SDL2Keyboard.newestKeyboard != null) {
						SDL2Keyboard.newestKeyboard.ProcessEvent(ref currentEvent);
					}

					// This is horrible, horrible design. To paraphrase Douglas Adams,
					// OpenTK was not so much designed as congealed.
					// I'm just gonna pass the old SDL unicode value along for now. (Truncated to 8 bits) :/
					KeyPress(this,new KeyPressEventArgs((char)currentEvent.key.keysym.unicode));

					break;
				case API.EventType.WindowEvent:
					switch (currentEvent.window.eventid)
					{
					//TODO: Should we use SizeChanged here and get rid of the other Resize() calls?
					case API.WindowEventId.Resized:
						Resize(this, EventArgs.Empty);
						break;
					}
					break;
				}
			}

        }

        #endregion

		#region Private Fullscreen Resolution Impl
		// So: this function doesn't work, and I'm going to restore the old ways.
		// What was the problem? SetWindowDisplayMode() sets the display mode for
		// the _next_ time fullscreen is triggered. Keeping the code in case this is fixed.
		private int FullscreenChangeRes (int x, int y)
		{
			API.DisplayMode desired = new API.DisplayMode ();
			desired.w = x;
			desired.h = y;
			// Maybe we should take the existing window's format?
			desired.format = 0;
			desired.refresh_rate = 0;

			API.DisplayMode closest;

			int desired_display = 0;

			IntPtr mode_success = IntPtr.Zero;

			lock (API.sdl_api_lock) {
				desired_display = API.GetWindowDisplay (window.WindowHandle);

				mode_success = API.GetClosestDisplayMode (desired_display, ref desired, out closest);

			}

			if (mode_success == IntPtr.Zero) {
				Console.WriteLine (String.Format ("Could not find {0}x{1} mode on display {2}", x, y, desired_display));
				return -1;
			}

			Console.WriteLine("Closest:");
			Console.WriteLine(String.Format ("w: {0}, h: {1}, rf_rate: {2}, fmt: {3}",closest.w, closest.h, closest.refresh_rate, closest.format));

			int res = -1;
			lock (API.sdl_api_lock) {
				res = API.SetWindowDisplayMode (window.WindowHandle, ref closest);
			}
			if (res < 0) {
				Console.WriteLine (String.Format ("Failed to change to resolution {0}x{1} on {2}", x, y, desired_display));
			}
			return res;

		}

		#endregion

        #region Bounds

        public Rectangle Bounds
        {
            get {
				int w,h;
				lock (API.sdl_api_lock) {
					API.GetWindowSize(window.WindowHandle, out w, out h);
				}

				return new System.Drawing.Rectangle(0,0,w,h); }
            set
            {	
				if (value != Bounds)
				{
					bool wasFullscreen = isFullscreen;
					// At the moment, we disable fullscreen mode, do the resize and re-enable it.
					// SetWindowSize has no effect on fullscreen windows, so this is a hack to make
					// it work, given the SetWindowDisplayMode stuff is quite useless.
					if (isFullscreen)
						WindowState = WindowState.Normal;

					lock (API.sdl_api_lock)
					{
						API.SetWindowSize (window.WindowHandle,value.Width, value.Height);
					}

					if (wasFullscreen)
						WindowState = WindowState.Fullscreen;

					Resize(this,EventArgs.Empty);
				}
            }
        }

        #endregion

        #region Location

        public Point Location
        {
			get { return new Point(0,0); }
            set
            {
                //TODO: Implement if we care
            }
        }

        #endregion

        #region Size

        public Size Size
        {
            get { int w,h;
				lock (API.sdl_api_lock) {
					API.GetWindowSize(window.WindowHandle,out w, out h);
				}
				return new Size(w,h);
			}
            set
            {
				if (value != Size)
				{
					bool wasFullscreen = isFullscreen;
					// At the moment, we disable fullscreen mode, do the resize and re-enable it.
					// SetWindowSize has no effect on fullscreen windows, so this is a hack to make
					// it work, given the SetWindowDisplayMode stuff is quite useless.
					if (isFullscreen)
						WindowState = WindowState.Normal;

					lock (API.sdl_api_lock) {
						API.SetWindowSize (window.WindowHandle,value.Width, value.Height);
					}

					if (wasFullscreen)
						WindowState = WindowState.Fullscreen;

					// Do we actually need to do this?
					Resize(this,EventArgs.Empty);
				}
            }
        }

        #endregion

        #region ClientRectangle

        public Rectangle ClientRectangle
        {
            get
            {
				return Bounds;
            }
            set
            {
				Bounds = value;
            }
        }

        #endregion

        #region ClientSize

        public Size ClientSize
        {
            get
            {
                return Size;
            }
            set
            {
				Size = value;
            }
        }

        #endregion

        #region Width

        public int Width
        {
            get { return ClientSize.Width; }
            set { ClientSize = new Size(value, Height); }
        }

        #endregion

        #region Height

        public int Height
        {
            get { return ClientSize.Height; }
            set { ClientSize = new Size(Width, value); }
        }

        #endregion

        #region X

        public int X
        {
            get { return Location.X; }
            set { Location = new Point(value, Y); }
        }

        #endregion

        #region Y

        public int Y
        {
            get { return Location.Y; }
            set { Location = new Point(X, value); }
        }

        #endregion

        #region Icon

        public Icon Icon
        {
            get
            {
				return null;
            }
            set
            {
				//TODO: Implement
                    return;

            }
        }

        #endregion

        #region Focused

        public bool Focused
        {
            get
            {
				bool isFocused;
				lock (API.sdl_api_lock)
				{
					isFocused = (API.GetWindowFlags(window.WindowHandle) & API.WindowFlags.InputFocus) != 0;
				}
				return isFocused;
            }
        }

        #endregion

        #region WindowState

        public OpenTK.WindowState WindowState
        {
            get
            {
                return isFullscreen?OpenTK.WindowState.Fullscreen:OpenTK.WindowState.Normal;
            }
            set
            {
				if (value == OpenTK.WindowState.Fullscreen)
				{
					isFullscreen = true;
				}
				else
				{
					isFullscreen = false;
				}
				lock (API.sdl_api_lock)
				{
					API.SetWindowFullscreen(window.WindowHandle, isFullscreen?API.WindowFlags.Fullscreen:0);
				}
				Resize(this, EventArgs.Empty);
            }
        }

        #endregion

        #region WindowBorder

        public OpenTK.WindowBorder WindowBorder
        {
            get
            {
				//TODO: Implement
                return WindowBorder.Fixed;
            }
            set
            {
                //TODO: Implement
            }
        }

        #endregion

        #region Events

        public event EventHandler<EventArgs> Move = delegate { };
        public event EventHandler<EventArgs> Resize = delegate { };
        public event EventHandler<System.ComponentModel.CancelEventArgs> Closing = delegate { };
        public event EventHandler<EventArgs> Closed = delegate { };
        public event EventHandler<EventArgs> Disposed = delegate { };
        public event EventHandler<EventArgs> IconChanged = delegate { };
        public event EventHandler<EventArgs> TitleChanged = delegate { };
        public event EventHandler<EventArgs> VisibleChanged = delegate { };
        public event EventHandler<EventArgs> FocusedChanged = delegate { };
        public event EventHandler<EventArgs> WindowBorderChanged = delegate { };
        public event EventHandler<EventArgs> WindowStateChanged = delegate { };
        public event EventHandler<KeyboardKeyEventArgs> KeyDown = delegate { };
		public event EventHandler<KeyPressEventArgs> KeyPress = delegate { };
		public event EventHandler<KeyboardKeyEventArgs> KeyUp = delegate { };
        public event EventHandler<EventArgs> MouseEnter = delegate { };
        public event EventHandler<EventArgs> MouseLeave = delegate { };
        
        #endregion

        public bool CursorVisible
        {
            get { 
				bool visible = true;
				lock (API.sdl_api_lock)
				{
					visible = (API.ShowCursor(-1) == 1);
				}
				return visible; }
            set
            {
				lock (API.sdl_api_lock)
				{
					API.ShowCursor(value?1:0);
					// This is apparently OpenTK behaviour.
					API.SetWindowGrab(window.WindowHandle, !value);
				}
            }
        }

        #endregion

        #region --- INativeGLWindow Members ---

        #region public IInputDriver InputDriver

        public IInputDriver InputDriver
        {
            get
            {
                return inputDriver;
            }
        }

        #endregion 

        #region public bool Exists

        /// <summary>
        /// Returns true if a render window/context exists.
        /// </summary>
        public bool Exists
        {
            get { return true; }
        }

        #endregion

        #region public bool IsIdle

        public bool IsIdle
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region public IntPtr Handle

        /// <summary>
        /// Gets the current window handle.
        /// </summary>
        public IntPtr Handle
        {
            get { return this.window.WindowHandle; }
        }

        #endregion

        #region public string Title

        /// <summary>
        /// TODO: Use atoms for this property.
        /// Gets or sets the GameWindow title.
        /// </summary>
        public string Title
        {
            get
		    {
				string _title;
				lock (API.sdl_api_lock) {
					_title = API.GetWindowTitle(window.WindowHandle);
				}
				return _title;
            }
            set
            {
				lock (API.sdl_api_lock) {
					API.SetWindowTitle(window.WindowHandle, value);
					TitleChanged(this,EventArgs.Empty);
				}
            }
        }

        #endregion

        #region public bool Visible

        public bool Visible
        {
            get
            {
                return true;
            }
            set
            {
         		//TODO: Implement
            }
        }

        #endregion

        #region public IWindowInfo WindowInfo

        public IWindowInfo WindowInfo
        {
            get { return window; }
        }

        #endregion

        public void Close() { Exit(); }

        #region public void Exit()

        public void Exit ()
		{
			// This code makes me want to curl up and cry.
			// I'm deathly afraid of what C# will do to what _should_ be a union.
			API.Event quitEvent = new API.Event();
			quitEvent.quit = new API.QuitEvent();
			quitEvent.type = API.EventType.Quit;
			lock (API.sdl_api_lock) {
				API.PushEvent (ref quitEvent);
			}
        }

        #endregion

        #region public void DestroyWindow()

        public void DestroyWindow()
        {
			lock (API.sdl_api_lock) {

				API.DestroyWindow(window.WindowHandle);
				Closed(this, new EventArgs());
			}

        }

        #endregion

        #region PointToClient

        public Point PointToClient(Point point)
        {
            return point;
        }

        #endregion

        #region PointToScreen

        public Point PointToScreen(Point point)
        {
            return point;
        }

        #endregion

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool manuallyCalled)
        {

        }
        #endregion
    }
}

