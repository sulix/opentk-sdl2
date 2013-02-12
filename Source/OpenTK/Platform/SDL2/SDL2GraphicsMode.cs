#region --- License ---
/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the OpenTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing detailed licensing details.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

using OpenTK.Graphics;

namespace OpenTK.Platform.SDL2
{
    class SDL2GraphicsMode : IGraphicsMode
    {

        #region Constructors

        public SDL2GraphicsMode()
        {
        }

        #endregion

        #region IGraphicsMode Members

        public GraphicsMode SelectGraphicsMode (ColorFormat color, int depth, int stencil, int samples, ColorFormat accum,
                                               int buffers, bool stereo)
		{
			//TODO: Implement
			// Actually, you know what, never mind. This will all work magically on anything from the last
			// decade anyway.
            return new GraphicsMode(IntPtr.Zero,color,depth,stencil,samples,accum,buffers,stereo);

        }

        #endregion


    }
}
