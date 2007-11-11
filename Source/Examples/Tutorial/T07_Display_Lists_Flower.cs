﻿#region --- License ---
/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 */
#endregion

#region --- Using Directives ---

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using OpenTK;
using OpenTK.OpenGL;
using OpenTK.OpenGL.Enums;

#endregion --- Using Directives ---

namespace Examples.Tutorial
{
    [Example("Display Lists", ExampleCategory.Tutorial, 3)]
    public class T07_Display_Lists_Flower : GameWindow
    {
        #region --- Fields ---

        const int num_lists = 9;
        int[] lists = new int[num_lists];

        #endregion

        #region --- Constructors ---

        public T07_Display_Lists_Flower()
            : base(new DisplayMode(800, 600))
        {
        }

        #endregion

        #region OnLoad

        public override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.1f, 0.1f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            // Build some display lists.
            int first_list = GL.GenLists(num_lists);
            float c = 0;
            for (int i = 0; i < num_lists; i++)
            {
                lists[i] = first_list + i;
                GL.NewList(first_list + i, ListMode.Compile);

                GL.Color3(1.0, c, 1 - c);

                GL.PushMatrix();

                GL.Rotate(c * 360.0f, 0.0, 0.0, 1.0);
                GL.Translate(5.0, 0.0, 0.0);

                GL.Begin(BeginMode.Quads);

                GL.Vertex3(-1.0f, -1.0f, 1.0f);
                GL.Vertex3(1.0f, -1.0f, 1.0f);
                GL.Vertex3(1.0f, 1.0f, 1.0f);
                GL.Vertex3(-1.0f, 1.0f, 1.0f);

                GL.End();

                GL.PopMatrix();

                GL.EndList();

                c += 1 / (float)num_lists;
            }
        }

        #endregion

        #region OnUnload

        public override void OnUnload(EventArgs e)
        {
            GL.DeleteLists(lists[0], num_lists);
        }

        #endregion

        #region OnResize

        protected override void OnResize(OpenTK.Platform.ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            double ratio = 0.0;
            ratio = this.Width / (double)this.Height;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Glu.Perspective(45.0, ratio, 1.0, 64.0);
        }

        #endregion

        #region OnUpdateFrame

        public override void OnUpdateFrame(UpdateFrameEventArgs e)
        {
            if (Keyboard[OpenTK.Input.Key.Escape])
            {
                this.Exit();
            }
        }

        #endregion

        #region OnRenderFrame

        public override void OnRenderFrame(RenderFrameEventArgs e)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Glu.LookAt(
                0.0, 0.0, 16.0,
                0.0, 0.0, 0.0,
                0.0, 1.0, 0.0);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.CallLists(num_lists, ListNameType.Int, lists);

            SwapBuffers();
        }

        #endregion

        #region public static void Main()

        /// <summary>
        /// Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            using (T07_Display_Lists_Flower example = new T07_Display_Lists_Flower())
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