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
    internal interface INativeGLWindow : IResizable, IDisposable
    {
        void CreateWindow(int width, int height, GraphicsMode mode, int major, int minor, GraphicsContextFlags flags, out IGraphicsContext context);
        void DestroyWindow();
        void ProcessEvents();
        void PointToClient(ref System.Drawing.Point p);
        void PointToScreen(ref System.Drawing.Point p);

        bool Exists { get; }
        IWindowInfo WindowInfo { get; }

        string Title { get; set; }
        bool Visible { get; set; }
        bool IsIdle { get; }
        IInputDriver InputDriver { get; }
        WindowState WindowState { get; set; }
        WindowBorder WindowBorder { get; set; }

        event CreateEvent Create;
        event DestroyEvent Destroy;
    }

    public delegate void CreateEvent(object sender, EventArgs e);
    public delegate void DestroyEvent(object sender, EventArgs e);
}
