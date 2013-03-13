#region --- License ---
/* Copyright (c) OpenTK developers
 * SDL2 Backend by David Gow
 * See license.txt for license info
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

using OpenTK.Graphics;

namespace OpenTK.Platform.SDL2
{
    /// \internal
    /// <summary>
    /// Provides methods to create and control an opengl context on the SDL2 platform.
    /// This class supports OpenTK, and is not intended for use by OpenTK programs.
    /// </summary>
    internal sealed class SDL2GLContext : DesktopGraphicsContext
    {
        #region Fields

		IntPtr context;
		IntPtr window;
        int swap_interval = 1; // As defined in GLX_SGI_swap_control
        bool glx_loaded;

		[ThreadStatic]
		private static IntPtr currentThreadContext = IntPtr.Zero;

        #endregion

		public static IntPtr GetCurrentContext ()
		{
			return currentThreadContext;
		}

        #region --- Constructors ---

        public SDL2GLContext (GraphicsMode mode, IWindowInfo windowInfo, IGraphicsContext shared, bool direct,
            int major, int minor, GraphicsContextFlags flags)
		{
			SDL2WindowInfo currentWindow = (SDL2WindowInfo)windowInfo;
			window = currentWindow.WindowHandle;
            
			if (shared != null) {
				shared.MakeCurrent (windowInfo);
				lock (API.sdl_api_lock) {
					API.GL_SetAttribute (API.GLAttr.ShareWithCurrentContext, 1);
				}
			}

			lock (API.sdl_api_lock) {
				context = API.GL_CreateContext (currentWindow.WindowHandle);
			}

			MakeCurrent (windowInfo);

			if (shared != null) {
				shared.MakeCurrent (windowInfo);
			}
			Handle = new ContextHandle(context);
        }

        public SDL2GLContext(IntPtr ctxhandle, IWindowInfo windowInfo)
        {
			Console.WriteLine("WARNING! Creating context in a way we don't quite understand.");
            SDL2WindowInfo currentWindow = (SDL2WindowInfo)windowInfo;
			window = currentWindow.WindowHandle;
			context = ctxhandle;
			Handle = new ContextHandle(context);
			MakeCurrent(windowInfo);
        }

        #endregion

        #region --- Private Methods ---



        bool SupportsExtension (SDL2WindowInfo window, string e)
		{
			if (e == null)
				throw new ArgumentNullException ("e");
            
			bool supported = false;

			lock (API.sdl_api_lock) {
				supported = API.GL_ExtensionSupported (e);
			}
			return supported;
        }

        #endregion

        #region --- IGraphicsContext Members ---

        #region SwapBuffers()

        public override void SwapBuffers()
        {
			lock (API.sdl_api_lock) {
				API.GL_SwapWindow(window);
			}
        }

        #endregion

        #region MakeCurrent

        public override void MakeCurrent(IWindowInfo window)
        {
			lock (API.sdl_api_lock) {
	            if (window == null)
	            {
					API.GL_MakeCurrent(IntPtr.Zero, IntPtr.Zero);
					currentThreadContext = IntPtr.Zero;
				
	            }
	            else
	            {
	                SDL2WindowInfo w = (SDL2WindowInfo)window;
					if (API.GL_MakeCurrent(w.WindowHandle, context) < 0)
					{
						Console.WriteLine("Error: Could not make context current.");
					}
					currentThreadContext = context;
					this.window = ((SDL2WindowInfo)window).WindowHandle;
				
	            }
			}

        }

        #endregion

        #region IsCurrent

        public override bool IsCurrent
        {
            get
            {
				return context == currentThreadContext;
            }
        }

        #endregion

        #region SwapInterval

        public override int SwapInterval
        {
            get
            {
	            return 0; //TODO: Implement
            }
            set
            {
				//TODO: Implement
            }
        }

        #endregion

        #region GetAddress

        public override IntPtr GetAddress (string function)
		{
			IntPtr func = IntPtr.Zero;
			lock (API.sdl_api_lock) {
				func = API.GL_GetProcAddress (function);
			}

			return func;
        }

        #endregion

        #region LoadAll

        public override void LoadAll()
        {

            base.LoadAll();
        }

        #endregion

        #endregion


        #region --- IDisposable Members ---

        public override void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool manuallyCalled)
        {
            IsDisposed = true;
        }
        
        #endregion
    }
}

