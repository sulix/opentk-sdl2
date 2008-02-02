﻿#region --- License ---
/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 */
#endregion

#region --- Using Directives ---

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Diagnostics;
using System.IO;
using OpenTK.Graphics.OpenGL.Enums;

#endregion --- Using Directives ---

namespace Examples.Tutorial
{
    /// <summary>
    /// Demonstrates how to load and use a simple OpenGL shader program. Example is incomplete (documentation).
    /// </summary>
    [Example("First shader", ExampleCategory.GLSL, 1)]
    public class T10_GLSL_Cube : GameWindow
    {
        #region --- Fields ---

        static float angle = 0.0f, rotation_speed = 3.0f;
        int vertex_shader_object, fragment_shader_object, shader_program;
        int vertex_buffer_object, color_buffer_object, element_buffer_object;

        Shapes.Shape shape = new Examples.Shapes.Cube();

        #endregion

        #region --- Constructors ---

        public T10_GLSL_Cube()
            : base(new DisplayMode(800, 600))
        { }

        #endregion

        #region OnLoad

        /// <summary>
        /// This is the place to load resources that change little
        /// during the lifetime of the GameWindow. In this case, we
        /// check for GLSL support, and load the shaders.
        /// </summary>
        /// <param name="e">Not used.</param>
        public override void OnLoad(EventArgs e)
        {
            // Check for necessary capabilities:
            if (!GL.SupportsExtension("VERSION_2_0"))
            {
                MessageBox.Show("You need at least OpenGL 2.0 to run this example. Aborting.", "GLSL not supported",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Exit();
                return;
            }

            GL.ClearColor(Color.SteelBlue);
            GL.Enable(EnableCap.DepthTest);

            CreateVBO();

            using (StreamReader vs = new StreamReader("Data/Shaders/Simple_VS.glsl"))
            using (StreamReader fs = new StreamReader("Data/Shaders/Simple_FS.glsl"))
                CreateShaders(vs.ReadToEnd(), fs.ReadToEnd(),
                    out vertex_shader_object, out fragment_shader_object,
                    out shader_program);
        }

        #endregion

        #region CreateShaders

        void CreateShaders(string vs, string fs,
            out int vertexObject, out int fragmentObject, 
            out int program)
        {
            int status_code;
            string info;

            vertexObject = GL.CreateShader(ShaderType.VertexShader);
            fragmentObject = GL.CreateShader(ShaderType.FragmentShader);

            // Compile vertex shader
            GL.ShaderSource(vertexObject, vs);
            GL.CompileShader(vertexObject);
            GL.GetShaderInfoLog(vertexObject, out info);
            GL.GetShader(vertexObject, ShaderParameter.CompileStatus, out status_code);

            if (status_code != 1)
                throw new ApplicationException(info);

            // Compile vertex shader
            GL.ShaderSource(fragmentObject, fs);
            GL.CompileShader(fragmentObject);
            GL.GetShaderInfoLog(fragmentObject, out info);
            GL.GetShader(fragmentObject, ShaderParameter.CompileStatus, out status_code);
            
            if (status_code != 1)
                throw new ApplicationException(info);

            program = GL.CreateProgram();
            GL.AttachShader(program, fragmentObject);
            GL.AttachShader(program, vertexObject);

            GL.LinkProgram(program);
            GL.UseProgram(program);
        }

        #endregion

        #region private void CreateVBO()

        void CreateVBO()
        {
            int size;

            GL.GenBuffers(1, out vertex_buffer_object);
            GL.GenBuffers(1, out color_buffer_object);
            GL.GenBuffers(1, out element_buffer_object);

            // Upload the vertex data.
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer_object);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(shape.Vertices.Length * 3 * sizeof(float)), shape.Vertices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (size != shape.Vertices.Length * 3 * sizeof(Single))
                throw new ApplicationException("Problem uploading vertex data to VBO");

            // Upload the color data.
            GL.BindBuffer(BufferTarget.ArrayBuffer, color_buffer_object);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(shape.Colors.Length * sizeof(int)), shape.Colors, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (shape.Colors.Length * sizeof(int) != size)
                throw new ApplicationException("Problem uploading color data to VBO");
            
            // Upload the index data (elements inside the vertex data, not color indices as per the IndexPointer function!)
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer_object);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(shape.Indices.Length * sizeof(Int32)), shape.Indices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (shape.Indices.Length * 4 != size)
                throw new ApplicationException("Problem uploading index data to VBO");
        }

        #endregion

        #region OnUnload

        public override void OnUnload(EventArgs e)
        {
            GL.DeleteProgram(shader_program);
            GL.DeleteShader(fragment_shader_object);
            GL.DeleteShader(vertex_shader_object);
            GL.DeleteBuffers(1, ref vertex_buffer_object);
            GL.DeleteBuffers(1, ref element_buffer_object);
        }

        #endregion

        #region OnResize

        /// <summary>
        /// Called when the user resizes the window.
        /// </summary>
        /// <param name="e">Contains the new width/height of the window.</param>
        /// <remarks>
        /// You want the OpenGL viewport to match the window. This is the place to do it!
        /// </remarks>
        protected override void OnResize(OpenTK.Platform.ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            double ratio = e.Width / (double)e.Height;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Glu.Perspective(45.0, ratio, 1.0, 64.0);
        }

        #endregion

        #region OnUpdateFrame

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

            if ((Keyboard[OpenTK.Input.Key.AltLeft] || Keyboard[OpenTK.Input.Key.AltRight]) &&
                Keyboard[OpenTK.Input.Key.Enter])
                Fullscreen = !Fullscreen;
        }

        #endregion

        #region OnRenderFrame

        /// <summary>
        /// Place your rendering code here.
        /// </summary>
        public override void OnRenderFrame(RenderFrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit |
                     ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Glu.LookAt(0.0, 5.0, 5.0,
                       0.0, 0.0, 0.0,
                       0.0, 1.0, 0.0);

            angle += rotation_speed * (float)e.ScaleFactor;
            GL.Rotate(angle, 0.0f, 1.0f, 0.0f);

            GL.EnableClientState(EnableCap.VertexArray);
            GL.EnableClientState(EnableCap.ColorArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer_object);
            GL.VertexPointer(3, VertexPointerType.Float, 0, IntPtr.Zero);
            GL.BindBuffer(BufferTarget.ArrayBuffer, color_buffer_object);
            GL.ColorPointer(4, ColorPointerType.UnsignedByte, 0, IntPtr.Zero);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer_object);

            GL.DrawElements(BeginMode.Triangles, shape.Indices.Length,
                DrawElementsType.UnsignedInt, IntPtr.Zero);

            //GL.DrawArrays(GL.Enums.BeginMode.POINTS, 0, shape.Vertices.Length);

            GL.DisableClientState(EnableCap.VertexArray);
            GL.DisableClientState(EnableCap.ColorArray);
            

            //int error = GL.GetError();
            //if (error != 0)
            //    Debug.Print(Glu.ErrorString(Glu.Enums.ErrorCode.INVALID_OPERATION));

            Context.SwapBuffers();
        }

        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (T10_GLSL_Cube example = new T10_GLSL_Cube())
            {
                // Get the title and category  of this example using reflection.
                ExampleAttribute info = ((ExampleAttribute)example.GetType().GetCustomAttributes(false)[0]);
                example.Title = String.Format("OpenTK | {0} {1}: {2}", info.Category, info.Difficulty, info.Title);
                example.Run(30.0, 0.0);
            }
        }

        #endregion
    }
}
