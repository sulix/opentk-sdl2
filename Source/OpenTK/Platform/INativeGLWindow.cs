﻿#region --- License ---
/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the OpenTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing detailed licensing details.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Input;
using OpenTK.Graphics;

namespace OpenTK.Platform
{
    /// <summary>
    /// This interface supports OpenTK, and is not intended for use by OpenTK programs.
    /// </summary>
    internal interface INativeGLWindow : IResizable, IDisposable
    {
        //void CreateWindow(int width, int height, DisplayMode mode, out IGLContext context);
        void CreateWindow(int width, int height, GraphicsFormat mode, out IGLContext context);
        void DestroyWindow();
        void ProcessEvents();
        void PointToClient(ref System.Drawing.Point p);
        void PointToScreen(ref System.Drawing.Point p);

        bool Exists { get; }
        IWindowInfo WindowInfo { get; }

        string Title { get; set; }
        bool Visible { get; set; }
        bool IsIdle { get; }
        //IGLContext Context { get; }
        IInputDriver InputDriver { get; }
        bool Fullscreen { get; set; }

        event CreateEvent Create;
        event DestroyEvent Destroy;
    }

    public delegate void CreateEvent(object sender, EventArgs e);
    public delegate void DestroyEvent(object sender, EventArgs e);
}
