﻿#region --- License ---
/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 */
#endregion

#region --- Using directives ---

using System;
using System.Collections.Generic;
using System.Text;

using OpenTK;
using OpenTK.OpenGL;
using OpenTK.Platform;
using System.Threading;
using OpenTK.OpenGL.Enums;
using System.Runtime.InteropServices;
using OpenTK.Math;

#endregion

namespace Examples.Tutorial
{
    public class T08_VBO : GameWindow, IExample
    {
        #region --- Private Fields ---

        Shapes.Shape cube = new Examples.Shapes.Cube();
        Shapes.Shape plane = new Examples.Shapes.Plane(16, 16, 2.0f, 2.0f);

        struct Vbo
        {
            public int VboID, EboID, NumElements;
        }
        Vbo[] vbo = new Vbo[2];
        float angle;

        public static readonly int order = 8;

        #endregion

        #region --- Constructor ---

        public T08_VBO() : base(new DisplayMode(800, 600), "OpenTK Tutorial 08: Vertex Buffer Objects") { }

        #endregion

        #region OnLoad override

        public override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!GL.SupportsExtension("VERSION_1_5"))
            {
                System.Windows.Forms.MessageBox.Show("You need at least OpenGL 1.5 to run this example. Aborting.", "VBOs not supported",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                this.Exit();
            }

            GL.ClearColor(0.1f, 0.1f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);

            // Create the Vertex Buffer Object:
            // 1) Generate the buffer handles.
            // 2) Bind the Vertex Buffer and upload your vertex data. Check that the data was uploaded correctly.
            // 3) Bind the Index Buffer and upload your index data. Check that the data was uploaded correctly.

            vbo[0] = Load(cube.Vertices, cube.Indices);
            vbo[1] = Load(cube.Vertices, cube.Indices);
        }

        #endregion

        #region OnResize override

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            double ratio = e.Width / (double)e.Height;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Glu.Perspective(45.0, ratio, 1.0, 64.0);
        }

        #endregion

        #region OnUpdateFrame override

        /// <summary>
        /// Prepares the next frame for rendering.
        /// </summary>
        /// <remarks>
        /// Place your control logic here. This is the place to respond to user input,
        /// update object positions etc.
        /// </remarks>
        public override void OnUpdateFrame(UpdateFrameEventArgs e)
        {
            if (Keyboard[OpenTK.Input.Key.Escape])
                this.Exit();
        }

        #endregion

        #region OnRenderFrame

        public override void OnRenderFrame(RenderFrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Glu.LookAt(0.0, 5.0, 5.0,
                       0.0, 0.0, 0.0,
                       0.0, 1.0, 0.0);

            GL.Color4(System.Drawing.Color.Black);
            Draw(vbo[0]);

            SwapBuffers();
        }

        #endregion

        #region public void Launch()

        /// <summary>
        /// Launches this example.
        /// </summary>
        /// <remarks>
        /// Provides a simple way for the example launcher to launch the examples.
        /// </remarks>
        public void Launch()
        {
            Run(60.0, 60.0);
        }

        #endregion

        Vbo Load(Vector3[] vertices, int[] indices)
        {
            Vbo handle = new Vbo();
            int size;

            GL.GenBuffers(1, out handle.VboID);
            GL.BindBuffer(Version15.ArrayBuffer, handle.VboID);
            GL.BufferData(Version15.ArrayBuffer, (IntPtr)(vertices.Length * Vector3.SizeInBytes), vertices,
                          Version15.StaticDraw);
            GL.GetBufferParameter(Version15.ArrayBuffer, Version15.BufferSize, out size);
            if (vertices.Length * Vector3.SizeInBytes != size)
                throw new ApplicationException("Vertex array not uploaded correctly");
            //GL.BindBuffer(Version15.ArrayBuffer, 0);

            GL.GenBuffers(1, out handle.EboID);
            GL.BindBuffer(Version15.ElementArrayBuffer, handle.EboID);
            GL.BufferData(Version15.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(int)), indices,
                          Version15.StaticDraw);
            GL.GetBufferParameter(Version15.ElementArrayBuffer, Version15.BufferSize, out size);
            if (indices.Length * sizeof(int) != size)
                throw new ApplicationException("Element array not uploaded correctly");
            //GL.BindBuffer(Version15.ElementArrayBuffer, 0);

            handle.NumElements = indices.Length;
            return handle;
        }

        void Draw(Vbo handle)
        {
            //GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

            //GL.EnableClientState(EnableCap.TextureCoordArray);
            GL.EnableClientState(EnableCap.VertexArray);

            GL.BindBuffer(Version15.StaticDraw, handle.VboID);
            GL.BindBuffer(Version15.ElementArrayBuffer, handle.EboID);

            //GL.TexCoordPointer(2, TexCoordPointerType.Float, vector2_size, (IntPtr)vector2_size);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);

            GL.DrawElements(BeginMode.Triangles, handle.NumElements, All.UnsignedInt, IntPtr.Zero);
            //GL.DrawArrays(BeginMode.LineLoop, 0, vbo.element_count);

            GL.BindBuffer(Version15.ArrayBuffer, 0);
            GL.BindBuffer(Version15.ElementArrayBuffer, 0);

            GL.DisableClientState(EnableCap.VertexArray);
            //GL.DisableClientState(EnableCap.TextureCoordArray);

            //GL.PopClientAttrib();
        }
    }
}
