﻿#region --- License ---
/* Copyright (c) 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

//using OpenTK.OpenGL;

namespace OpenTK.Platform.X11
{
    /// <summary>
    /// Drives GameWindow on X11.
    /// This class supports OpenTK, and is not intended for use by OpenTK programs.
    /// </summary>
    internal sealed class X11GLNative : INativeGLWindow, IDisposable
    {
        #region --- Fields ---

        private X11GLContext glContext;
        private WindowInfo info = new WindowInfo();
        private DisplayMode mode = new DisplayMode();

        // Number of pending events.
        private int pending = 0;

        // C# ResizeEventArgs
        private ResizeEventArgs resizeEventArgs = new ResizeEventArgs();

        // Low level X11 resize request
        private X11.Event xresize = new Event();
        // Event used for event loop.
        private Event e = new Event();
        // This is never written in the code. If at some point it gets != 0,
        // then memory corruption is taking place from the xresize struct.
        int memGuard = 0;
        private ConfigureNotifyEvent configure = new ConfigureNotifyEvent();
        private ReparentNotifyEvent reparent = new ReparentNotifyEvent();
        private ExposeEvent expose = new ExposeEvent();
        private CreateWindowEvent createWindow = new CreateWindowEvent();
        private DestroyWindowEvent destroyWindow = new DestroyWindowEvent();

        private bool disposed;
        private bool created;

        #endregion

        #region --- Public Constructors ---

        /// <summary>
        /// Constructs and initializes a new X11GLNative window.
        /// Call CreateWindow to create the actual render window.
        /// </summary>
        public X11GLNative()
        {
            Debug.Print("Native window driver: {0}", this.ToString());
        }

        #endregion

        #region --- INativeGLWindow Members ---

        #region public void CreateWindow(DisplayMode mode)

        /// <summary>
        /// Opens a new render window with the given DisplayMode.
        /// </summary>
        /// <param name="mode">The DisplayMode of the render window.</param>
        /// <remarks>
        /// Creates the window visual and colormap. Associates the colormap/visual
        /// with the window and raises the window on top of the window stack.
        /// <para>
        /// Colormap creation is currently disabled.
        /// </para>
        /// </remarks>
        public void CreateWindow(DisplayMode mode)
        {
            Debug.Print("Creating native window with mode: {0}", mode.ToString());
            Debug.Indent();

            info.Display = API.OpenDisplay(null); // null == default display
            if (info.Display == IntPtr.Zero)
            {
                throw new Exception("Could not open connection to X");
            }
            info.Screen = API.DefaultScreen(info.Display);
            info.RootWindow = API.RootWindow(info.Display, info.Screen);

            Debug.Print(
                "Display: {0}, Screen {1}, Root window: {2}",
                info.Display,
                info.Screen,
                info.RootWindow
            );

            glContext = new X11GLContext(info, mode);
            glContext.CreateVisual();

            // Create a window on this display using the visual above
            Debug.Write("Creating output window... ");

            SetWindowAttributes wnd_attributes = new SetWindowAttributes();
            wnd_attributes.background_pixel = 0;
            wnd_attributes.border_pixel = 0;
            wnd_attributes.colormap = glContext.XColormap;
            //API.CreateColormap(display, rootWindow, glxVisualInfo.visual, 0/*AllocNone*/);
            wnd_attributes.event_mask =
                EventMask.StructureNotifyMask |
                EventMask.SubstructureNotifyMask |
                EventMask.ExposureMask;

            CreateWindowMask cw_mask =
                CreateWindowMask.CWBackPixel |
                CreateWindowMask.CWBorderPixel |
                CreateWindowMask.CWColormap |
                CreateWindowMask.CWEventMask;

            info.Handle = API.CreateWindow(
                info.Display,
                info.RootWindow,
                0, 0,
                640, 480,
                0,
                //glxVisualInfo.depth,
                glContext.XVisualInfo.depth,
                Constants.InputOutput,
                //glxVisualInfo.visual,
                glContext.XVisualInfo.visual,
                cw_mask,
                wnd_attributes
            );

            if (info.Handle == IntPtr.Zero)
            {
                throw new Exception("Could not create window.");
            }

            Debug.WriteLine("done! (id: " + info.Handle + ")");

            // Set the window hints
            /*
            SizeHints hints = new SizeHints();
            hints.x = 0;
            hints.y = 0;
            hints.width = 640;
            hints.height = 480;
            hints.flags = USSize | USPosition;
            X11Api.SetNormalHints(display, window, hints);
            X11Api.SetStandardProperties(
                display,
                window,
                name,
                name,
                0,  // None
                null,
                0,
                hints
            );
            */

            //glContext.ContainingWindow = info.Window;


            glContext.windowInfo.Handle = info.Handle;
            glContext.CreateContext(null, true);

            API.MapRaised(info.Display, info.Handle);

            Debug.WriteLine("Mapped window.");

            //glContext.MakeCurrent();

            Debug.WriteLine("Our shiny new context is now current - ready to rock 'n' roll!");
            Debug.Unindent();
            created = true;
        }

        #endregion

        #region public void Exit()

        public void Exit()
        {
            /*Event e = new Event();
            X11Api.SendEvent(
                display,
                window,
                false,
                0,*/
            //quit = true;
        }

        #endregion

        #region public void ProcessEvents()

        public void ProcessEvents()
        {
            // Process all pending events
            while (true)
            {
                pending = API.Pending(info.Display);

                if (pending == 0)
                    return;

                //API.NextEvent(info.Display, e);
                API.PeekEvent(info.Display, e);
                //API.NextEvent(info.Display, eventPtr);


                Debug.WriteLine(String.Format("Event: {0} ({1} pending)", e.Type, pending));
                //Debug.WriteLine(String.Format("Event: {0} ({1} pending)", eventPtr, pending));

                // Check whether memory was corrupted by the NextEvent call.
                Debug.Assert(memGuard == 0, "memGuard2 tripped", String.Format("Guard: {0}", memGuard));
                memGuard = 0;

                // Respond to the event e
                switch (e.Type)
                {
                    case EventType.ReparentNotify:
                        API.NextEvent(info.Display, reparent);
                        // Do nothing
                        break;

                    case EventType.CreateNotify:
                        API.NextEvent(info.Display, createWindow);

                        // Set window width/height
                        mode.Width = createWindow.width;
                        mode.Height = createWindow.height;
                        this.OnCreate(EventArgs.Empty);
                        Debug.WriteLine(
                            String.Format("OnCreate fired: {0}x{1}", mode.Width, mode.Height)
                        );
                        break;

                    case EventType.DestroyNotify:
                        API.NextEvent(info.Display, destroyWindow);
                        quit = true;
                        Debug.WriteLine("Window destroyed, shutting down.");
                        break;


                    case EventType.ConfigureNotify:
                        API.NextEvent(info.Display, configure);

                        // If the window size changed, raise the C# Resize event.
                        if (configure.width != mode.Width ||
                            configure.height != mode.Height)
                        {
                            Debug.WriteLine(
                                String.Format(
                                    "New res: {0}x{1}",
                                    configure.width,
                                    configure.height
                                )
                            );

                            resizeEventArgs.Width = configure.width;
                            resizeEventArgs.Height = configure.height;
                            this.OnResize(resizeEventArgs);
                        }
                        break;

                    default:
                        API.NextEvent(info.Display, e);
                        Debug.WriteLine(String.Format("{0} event was not handled", e.Type));
                        break;
                }
            }
        }

        #endregion

        #region public event CreateEvent Create;

        public event CreateEvent Create;

        private void OnCreate(EventArgs e)
        {
            if (this.Create != null)
            {
                this.Create(this, e);
            }
        }

        #endregion

        #region public bool Created

        /// <summary>
        /// Returns true if a render window/context exists.
        /// </summary>
        public bool Created
        {
            get { return created; }
        }

        #endregion

        #region public bool Quit

        private bool quit;
        public bool Quit
        {
            get { return quit; }
        }

        #endregion

        #region public bool IsIdle

        public bool IsIdle
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region public bool Fullscreen

        public bool Fullscreen
        {
            get
            {
                return false;
                //throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                
                //throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region public IGLContext Context

        public OpenTK.Platform.IGLContext Context
        {
            get { return glContext; }
        }

        #endregion

        #region public IntPtr Handle

        /// <summary>
        /// Gets the current window handle.
        /// </summary>
        public IntPtr Handle
        {
            get { return this.info.Handle; }
        }

        #endregion

        #region public IWindowInfo WindowInfo

        public IWindowInfo WindowInfo
        {
            get { return info; }
        }

        #endregion

        #endregion

        #region --- IResizable Members ---

        #region public int Width

        public int Width
        {
            get
            {
                return mode.Width;
            }
            set
            {/*
                // Clear event struct
                //Array.Clear(xresize.pad, 0, xresize.pad.Length);
                // Set requested parameters
                xresize.ResizeRequest.type = EventType.ResizeRequest;
                xresize.ResizeRequest.display = this.display;
                xresize.ResizeRequest.width = value;
                xresize.ResizeRequest.height = mode.Width;
                API.SendEvent(
                    this.display,
                    this.window,
                    false,
                    EventMask.StructureNotifyMask,
                    ref xresize
                );*/
            }
        }

        #endregion

        #region public int Height

        public int Height
        {
            get
            {
                return mode.Height;
            }
            set
            {/*
                // Clear event struct
                //Array.Clear(xresize.pad, 0, xresize.pad.Length);
                // Set requested parameters
                xresize.ResizeRequest.type = EventType.ResizeRequest;
                xresize.ResizeRequest.display = this.display;
                xresize.ResizeRequest.width = mode.Width;
                xresize.ResizeRequest.height = value;
                API.SendEvent(
                    this.display,
                    this.window,
                    false,
                    EventMask.StructureNotifyMask,
                    ref xresize
                );*/
            }
        }

        #endregion

        #region public event ResizeEvent Resize

        public event ResizeEvent Resize;

        private void OnResize(ResizeEventArgs e)
        {
            mode.Width = e.Width;
            mode.Height = e.Height;
            if (this.Resize != null)
            {
                this.Resize(this, e);
            }
        }

        #endregion

        #endregion

        #region --- IDisposable Members ---

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool manuallyCalled)
        {
            if (!disposed)
            {
                API.DestroyWindow(info.Display, info.Handle);
                // Kills connection to the X-Server. We don't want that,
                // 'cause it kills the ExampleLauncher too.
                //API.CloseDisplay(display);

                if (manuallyCalled)
                {
                    glContext.Dispose();
                }
                disposed = true;
            }
        }

        ~X11GLNative()
        {
            this.Dispose(false);
        }

        #endregion
    }
}
