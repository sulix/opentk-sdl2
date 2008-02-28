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

using OpenTK.Platform;

namespace OpenTK.Graphics
{
    /// <summary>
    /// Provides methods for creating and interacting with an OpenGL context.
    /// </summary>
    public interface IGraphicsContext : IDisposable
    {
        /// <summary>
        /// Creates an OpenGL context with the specified direct/indirect rendering mode and sharing state with the
        /// specified IGraphicsContext.
        /// </summary>
        /// <param name="direct">Set to true for direct rendering or false otherwise.</param>
        /// <param name="source">The source IGraphicsContext to share state from.</param>.
        /// <seealso cref="CreateContext(bool)"/>
        void CreateContext(bool direct, IGraphicsContext source);

        /// <summary>Swaps buffers, presenting the rendered scene to the user.</summary>
        void SwapBuffers();

        /// <summary>Makes the GraphicsContext current in the calling thread.</summary>
        /// <remarks>
        /// <para>OpenGL commands in one thread, affect the GraphicsContext which is current in that thread.</para>
        /// <para>It is an error to issue an OpenGL command in a thread without a current GraphicsContext.</para>
        /// </remarks>
        void MakeCurrent();

        /// <summary>
        /// Gets or sets a System.Boolean indicating whether the GraphicsContext is current in the calling thread.
        /// </summary>
        bool IsCurrent { get; set;  }

        /// <summary>
        /// Raised when a Context is destroyed.
        /// </summary>
        event DestroyEvent<IGraphicsContext> Destroy;

        /// <summary>
        /// Gets or sets a value indicating whether VSyncing is enabled.
        /// </summary>
        bool VSync { get; set; }
    }

    public delegate void DestroyEvent<T>(T sender, EventArgs e);

    // TODO: Remove in 0.3.15
    internal interface IGLContextCreationHack
    {
        bool SelectDisplayMode(DisplayMode mode, IWindowInfo info);
        void SetWindowHandle(IntPtr handle);
    }

    // Functions for internal use by OpenTK.
    // TODO: RegisterForDisposal/DisposeResources for 0.3.15 (GC & OpenGL)
    // TODO: Remove or move GetDisplayModes to another class.
    internal interface IGLContextInternal
    {
        /// <summary>
        /// Gets a handle to the OpenGL rendering context.
        /// </summary>
        ContextHandle Context { get; }

        /// <summary>
        /// Gets the IWindowInfo describing the window associated with this context.
        /// </summary>
        IWindowInfo Info { get; }

        /// <summary>Gets the GraphicsFormat of the context.</summary>
        GraphicsMode GraphicsFormat { get; }

        ///// <summary>
        ///// Gets a System.IntPtr containing the handle to the OpenGL context which is current in the
        ///// calling thread, or IntPtr.Zero if no OpenGL context is current.
        ///// </summary>
        ///// <returns>A System.IntPtr that holds the handle to the current OpenGL context.</returns>
        //ContextHandle GetCurrentContext();

        /// <summary>
        /// Registers an OpenGL resource for disposal.
        /// </summary>
        /// <param name="resource">The OpenGL resource to dispose.</param>
        /// <remarks>
        /// You may not destroy OpenGL resources in finalizers, since they run in
        /// a different thread. To avoid this problem, use this method to register
        /// a resource for disposal during the finalizer call, and call DisposeResources()
        /// from the main thread to dispose it.
        /// </remarks>
        void RegisterForDisposal(IDisposable resource);

        /// <summary>
        /// Disposes all registered OpenGL resources.
        /// </summary>
        void DisposeResources();

        /// <summary>
        /// Returns the display modes supported by the current opengl context.
        /// </summary>
        /// <returns>An IEnumerable containing all supported display modes.</returns>
        IEnumerable<DisplayMode> GetDisplayModes();

        /// <summary>
        /// Gets the address of an OpenGL extension function.
        /// </summary>
        /// <param name="function">The name of the OpenGL function (e.g. "glGetString")</param>
        /// <returns>
        /// A pointer to the specified function or IntPtr.Zero if the function isn't
        /// available in the current opengl context.
        /// </returns>
        // /// <see cref="Marshal.GetDelegateForFunctionPointer"/>
        IntPtr GetAddress(string function);
    }
}
