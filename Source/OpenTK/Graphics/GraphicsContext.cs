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
using System.Diagnostics;

using OpenTK.Platform;

namespace OpenTK.Graphics
{
    /// <summary>
    /// Represents and provides methods to manipulate an OpenGL render context.
    /// </summary>
    public sealed class GraphicsContext : IGraphicsContext, IGraphicsContextInternal
    {
        #region --- Fields ---

        IGraphicsContext implementation;  // The actual render context implementation for the underlying platform.
        List<IDisposable> dispose_queue = new List<IDisposable>();
        bool disposed;
        // Indicates that this context was created through external means, e.g. Tao.Sdl or GLWidget#.
        // In this case, We'll assume that the external program will manage the lifetime of this
        // context - we'll not destroy it manually.
        //bool is_external;
        bool check_errors = true;

        static bool share_contexts = true;
        static bool direct_rendering = true;
        readonly static object context_lock = new object();        
        // Maps OS-specific context handles to GraphicsContext weak references.
        readonly static Dictionary<ContextHandle, WeakReference> available_contexts = new Dictionary<ContextHandle, WeakReference>();

        #endregion

        #region --- Constructors ---

        static GraphicsContext()
        {
            GetCurrentContext = Factory.Default.CreateGetCurrentGraphicsContext();
        }
        
        // Necessary to allow creation of dummy GraphicsContexts (see CreateDummyContext static method).
        GraphicsContext(ContextHandle handle)
        {
            implementation = new OpenTK.Platform.Dummy.DummyGLContext(handle);

            lock (context_lock)
            {
                available_contexts.Add((implementation as IGraphicsContextInternal).Context, new WeakReference(this));
            }
        }
        
        /// <summary>
        /// Constructs a new GraphicsContext with the specified GraphicsMode and attaches it to the specified window.
        /// </summary>
        /// <param name="mode">The OpenTK.Graphics.GraphicsMode of the GraphicsContext.</param>
        /// <param name="window">The OpenTK.Platform.IWindowInfo to attach the GraphicsContext to.</param>
        public GraphicsContext(GraphicsMode mode, IWindowInfo window)
            : this(mode, window, 1, 0, GraphicsContextFlags.Default)
        { }

        /// <summary>
        /// Constructs a new GraphicsContext with the specified GraphicsMode, version and flags,  and attaches it to the specified window.
        /// </summary>
        /// <param name="mode">The OpenTK.Graphics.GraphicsMode of the GraphicsContext.</param>
        /// <param name="window">The OpenTK.Platform.IWindowInfo to attach the GraphicsContext to.</param>
        /// <param name="major">The major version of the new GraphicsContext.</param>
        /// <param name="minor">The minor version of the new GraphicsContext.</param>
        /// <param name="flags">The GraphicsContextFlags for the GraphicsContext.</param>
        /// <remarks>
        /// Different hardware supports different flags, major and minor versions. Invalid parameters will be silently ignored.
        /// </remarks>
        public GraphicsContext(GraphicsMode mode, IWindowInfo window, int major, int minor, GraphicsContextFlags flags)
        {
            bool designMode = false;
            if (mode == null && window == null)
                designMode = true;
            else if (mode == null) throw new ArgumentNullException("mode", "Must be a valid GraphicsMode.");
            else if (window == null) throw new ArgumentNullException("window", "Must point to a valid window.");

            // Silently ignore invalid major and minor versions.
            if (major <= 0)
                major = 1;
            if (minor < 0)
                minor = 0;

            Debug.Print("Creating GraphicsContext.");
            try
            {
                Debug.Indent();
                Debug.Print("GraphicsMode: {0}", mode);
                Debug.Print("IWindowInfo: {0}", window);
                Debug.Print("GraphicsContextFlags: {0}", flags);
                Debug.Print("Requested version: {0}.{1}", major, minor);

                IGraphicsContext shareContext = null;
                if (GraphicsContext.ShareContexts)
                {
                    lock (context_lock)
                    {
                        // A small hack to create a shared context with the first available context.
                        foreach (WeakReference r in GraphicsContext.available_contexts.Values)
                        {
                            shareContext = (IGraphicsContext)r.Target;
                            break;
                        }
                    }
                }

                // Todo: Add a DummyFactory implementing IPlatformFactory.
                if (designMode)
                    implementation = new Platform.Dummy.DummyGLContext();
                else
                    switch ((flags & GraphicsContextFlags.Embedded) == GraphicsContextFlags.Embedded)
                    {
                        case false: implementation = Factory.Default.CreateGLContext(mode, window, shareContext, direct_rendering, major, minor, flags); break;
                        case true: implementation = Factory.Embedded.CreateGLContext(mode, window, shareContext, direct_rendering, major, minor, flags); break;
                    }

                lock (context_lock)
                {
                    available_contexts.Add((this as IGraphicsContextInternal).Context, new WeakReference(this));
                }
            }
            finally
            {
                Debug.Unindent();
            }
        }

        #endregion

        #region --- Static Members ---

        #region public static GraphicsContext CreateDummyContext()

        /// <summary>
        /// Creates a dummy GraphicsContext to allow OpenTK to work with contexts created by external libraries.
        /// </summary>
        /// <returns>A new, dummy GraphicsContext instance.</returns>
        /// <remarks>
        /// <para>Instances created by this methodwill not be functional. Instance methods will have no effect.</para>
        /// </remarks>
        public static GraphicsContext CreateDummyContext()
        {
            ContextHandle handle = GetCurrentContext();
            if (handle == ContextHandle.Zero)
                throw new InvalidOperationException("No GraphicsContext is current on the calling thread.");

            return CreateDummyContext(handle);
        }

        public static GraphicsContext CreateDummyContext(ContextHandle handle)
        {
            if (handle == ContextHandle.Zero)
                throw new ArgumentOutOfRangeException("handle");

            return new GraphicsContext(handle);
        }

        #endregion

        #region public static void Assert()

        /// <summary>
        /// Checks if a GraphicsContext exists in the calling thread and throws a GraphicsContextMissingException if it doesn't.
        /// </summary>
        /// <exception cref="GraphicsContextMissingException">Generated when no GraphicsContext is current in the calling thread.</exception>
        public static void Assert()
        {
            if (GraphicsContext.CurrentContext == null)
                throw new GraphicsContextMissingException();
        }

        #endregion

        #region public static IGraphicsContext CurrentContext

        internal delegate ContextHandle GetCurrentContextDelegate();
        internal static GetCurrentContextDelegate GetCurrentContext;

        /// <summary>
        /// Gets the GraphicsContext that is current in the calling thread.
        /// </summary>
        public static IGraphicsContext CurrentContext
        {
            get
            {
                lock (context_lock)
                {
                    if (available_contexts.Count > 0)
                    {
                        ContextHandle handle = GetCurrentContext();
                        if (handle.Handle != IntPtr.Zero)
                            return (GraphicsContext)available_contexts[handle].Target;
                    }
                    return null;
                }
            }
        }

        #endregion

        #region public static bool ShareContexts

        /// <summary>Gets or sets a System.Boolean, indicating whether GraphicsContext resources are shared</summary>
        /// <remarks>
        /// <para>If ShareContexts is true, new GLContexts will share resources. If this value is
        /// false, new GLContexts will not share resources.</para>
        /// <para>Changing this value will not affect already created GLContexts.</para>
        /// </remarks>
        public static bool ShareContexts { get { return share_contexts; } set { share_contexts = value; } }

        #endregion

        #region public static bool DirectRendering

        /// <summary>Gets or sets a System.Boolean, indicating whether GraphicsContexts will perform direct rendering.</summary>
        /// <remarks>
        /// <para>
        /// If DirectRendering is true, new contexts will be constructed with direct rendering capabilities, if possible.
        /// If DirectRendering is false, new contexts will be constructed with indirect rendering capabilities.
        /// </para>
        /// <para>This property does not affect existing GraphicsContexts, unless they are recreated.</para>
        /// <para>
        /// This property is ignored on Operating Systems without support for indirect rendering, like Windows and OS X.
        /// </para>
        /// </remarks>
        public static bool DirectRendering
        {
            get { return direct_rendering; }
            set { direct_rendering = value; }
        }

        #endregion

        #endregion

        #region --- IGraphicsContext Members ---

        /// <summary>
        /// Gets or sets a System.Boolean, indicating whether automatic error checking should be performed.
        /// Influences the debug version of OpenTK.dll, only.
        /// </summary>
        /// <remarks>Automatic error checking will clear the OpenGL error state. Set CheckErrors to false if you use
        /// the OpenGL error state in your code flow (e.g. for checking supported texture formats).</remarks>
        public bool ErrorChecking
        {
            get { return check_errors; }
            set { check_errors = value; }
        }
        /// <summary>
        /// Creates an OpenGL context with the specified direct/indirect rendering mode and sharing state with the
        /// specified IGraphicsContext.
        /// </summary>
        /// <param name="direct">Set to true for direct rendering or false otherwise.</param>
        /// <param name="source">The source IGraphicsContext to share state from.</param>.
        /// <remarks>
        /// <para>
        /// Direct rendering is the default rendering mode for OpenTK, since it can provide higher performance
        /// in some circumastances.
        /// </para>
        /// <para>
        /// The 'direct' parameter is a hint, and will ignored if the specified mode is not supported (e.g. setting
        /// indirect rendering on Windows platforms).
        /// </para>
        /// </remarks>
        void CreateContext(bool direct, IGraphicsContext source)
        {
            lock (context_lock)
            {
                available_contexts.Add((this as IGraphicsContextInternal).Context, new WeakReference(this));
            }
        }

        /// <summary>
        /// Swaps buffers on a context. This presents the rendered scene to the user.
        /// </summary>
        public void SwapBuffers()
        {
            implementation.SwapBuffers();
        }

        /// <summary>
        /// Makes the GraphicsContext the current rendering target.
        /// </summary>
        /// <param name="window">A valid <see cref="OpenTK.Platform.IWindowInfo" /> structure.</param>
        /// <remarks>
        /// You can use this method to bind the GraphicsContext to a different window than the one it was created from.
        /// </remarks>
        public void MakeCurrent(IWindowInfo window)
        {
            implementation.MakeCurrent(window);
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether this instance is current in the calling thread.
        /// </summary>
        public bool IsCurrent
        {
            get { return implementation.IsCurrent; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether this instance has been disposed.
        /// It is an error to access any instance methods if this property returns true.
        /// </summary>
        public bool IsDisposed
        {
            get { return disposed && implementation.IsDisposed; }
            private set { disposed = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether VSync is enabled.
        /// </summary>
        public bool VSync
        {
            get { return implementation.VSync; }
            set { implementation.VSync = value;  }
        }

        /// <summary>
        /// Updates the graphics context.  This must be called when the render target
        /// is resized for proper behavior on Mac OS X.
        /// </summary>
        /// <param name="window"></param>
        public void Update(IWindowInfo window)
        {
            implementation.Update(window);
        }
        
        #endregion

        #region --- IGraphicsContextInternal Members ---

        /// <summary>
        /// Gets the platform-specific implementation of this IGraphicsContext.
        /// </summary>
        IGraphicsContext IGraphicsContextInternal.Implementation
        {
            get { return implementation; }
        }

        /// <summary>
        /// Loads all OpenGL extensions.
        /// </summary>
        /// <exception cref="OpenTK.Graphics.GraphicsContextException">
        /// Occurs when this instance is not the current GraphicsContext on the calling thread.
        /// </exception>
        void IGraphicsContextInternal.LoadAll()
        {
            if (GraphicsContext.CurrentContext != this)
                throw new GraphicsContextException();

            (implementation as IGraphicsContextInternal).LoadAll();
        }

        /// <summary>
        /// Gets a handle to the OpenGL rendering context.
        /// </summary>
        ContextHandle IGraphicsContextInternal.Context
        {
            get { return ((IGraphicsContextInternal)implementation).Context; }
        }

        /// <summary>
        /// Gets the GraphicsMode of the context.
        /// </summary>
        public GraphicsMode GraphicsMode
        {
            get { return (implementation as IGraphicsContext).GraphicsMode; }
        }

        /// <summary>
        /// Gets the address of an OpenGL extension function.
        /// </summary>
        /// <param name="function">The name of the OpenGL function (e.g. "glGetString")</param>
        /// <returns>
        /// A pointer to the specified function or IntPtr.Zero if the function isn't
        /// available in the current opengl context.
        /// </returns>
        IntPtr IGraphicsContextInternal.GetAddress(string function)
        {
            return (implementation as IGraphicsContextInternal).GetAddress(function);
        }

        #endregion

        #region --- IDisposable Members ---

        /// <summary>
        /// Disposes of the GraphicsContext.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool manual)
        {
            if (!IsDisposed)
            {
                Debug.Print("Disposing context {0}.", (this as IGraphicsContextInternal).Context.ToString());
                lock (context_lock)
                {
                    available_contexts.Remove((this as IGraphicsContextInternal).Context);
                }

                if (manual)
                {
                    if (implementation != null)
                        implementation.Dispose();
                }
                IsDisposed = true;
            }
        }

        //~GraphicsContext()
        //{
        //    this.Dispose(false);
        //}

        #endregion
    }
}
