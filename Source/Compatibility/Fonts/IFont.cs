﻿#region --- License ---
/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;


namespace OpenTK.Graphics
{
    [Obsolete]
    public interface IFont : IDisposable
    {
        void LoadGlyphs(string glyphs);
        float Height { get; }
        void MeasureString(string str, out float width, out float height);
    }
}
