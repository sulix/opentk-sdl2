using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTK.Platform
{
    interface IPlatformFactory
    {
        INativeGLWindow CreateGLNative();

        IGLControl CreateGLControl(OpenTK.Graphics.GraphicsMode mode, GLControl owner);

        OpenTK.Graphics.IDisplayDeviceDriver CreateDisplayDeviceDriver();

        OpenTK.Graphics.IGraphicsContext CreateGLContext(OpenTK.Graphics.GraphicsMode mode, IWindowInfo window, OpenTK.Graphics.IGraphicsContext shareContext, bool DirectRendering);

        OpenTK.Graphics.IGraphicsMode CreateGraphicsMode();
    }
}
