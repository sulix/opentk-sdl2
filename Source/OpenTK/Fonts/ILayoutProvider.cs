﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace OpenTK.Fonts
{
    /// <summary>
    /// Defines the interface for a TextPrinter.
    /// </summary>
    public interface ITextPrinter
    {
        void Prepare(string text, TextureFont font, out TextHandle handle);
        void Prepare(string text, TextureFont font, out TextHandle handle, float width, bool wordWarp);
        void Prepare(string text, TextureFont font, out TextHandle handle, float width, bool wordWarp, StringAlignment alignment);
        void Prepare(string text, TextureFont font, out TextHandle handle, float width, bool wordWarp, StringAlignment alignment, bool rightToLeft);
        void Draw(TextHandle handle);
    }
}
