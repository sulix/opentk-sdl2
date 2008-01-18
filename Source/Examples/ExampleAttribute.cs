﻿#region --- License ---
/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Examples
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExampleAttribute : System.Attribute
    {
        string title;
        public string Title { get { return title; } internal set { title = value; } }
        public readonly ExampleCategory Category;
        public readonly int Difficulty;
        public readonly bool Visible = true;

        public ExampleAttribute(string title, ExampleCategory category)
            : this(title, category, 0, true) { }

        public ExampleAttribute(string title, ExampleCategory category, int difficulty)
            : this(title, category, difficulty, true) { }

        public ExampleAttribute(string title, ExampleCategory category, int difficulty, bool visible)
        {
            this.Title = title;
            this.Category = category;
            this.Difficulty = difficulty;
            this.Visible = visible;
        }

        public override string ToString()
        {
            if (Difficulty != 0)
                return String.Format("{0} {1}: {2}", Category, Difficulty, Title);
            return String.Format("{0}: {1}", Category, Title);
        }
    }

    public enum ExampleCategory
    {
        OpenGL = 0,
        OpenAL,
        Tutorial,
        GLSL,
        WinForms,
        Test,
        Last
    }
}
