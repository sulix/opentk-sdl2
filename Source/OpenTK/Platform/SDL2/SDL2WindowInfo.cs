#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2013 the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace OpenTK.Platform.SDL2
{
    sealed class SDL2WindowInfo : IWindowInfo
    {
		IntPtr window;

		public SDL2WindowInfo ()
		{
			this.window = IntPtr.Zero;
		}
       
        public SDL2WindowInfo(IntPtr window)
        {
            this.window = window;

        }

        public IntPtr WindowHandle { get { return window; } set { window = value; } }


        public void Dispose()
        {
        }

        public override string ToString()
        {
			return String.Format("SDL2 Window Info: Pointer {0}, ID {1}", this.window, API.GetWindowID(this.window));
        }

		public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            SDL2WindowInfo info = (SDL2WindowInfo)obj;

			return API.GetWindowID (this.window) == API.GetWindowID (info.window);
        }

        public override int GetHashCode()
        {
			return window.GetHashCode();
        }

    }
}
