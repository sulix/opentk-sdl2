 #region License
 //
 // The Open Toolkit Library License
 //
 // Copyright (c) 2006 - 2010 the Open Toolkit library.
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
using System.Diagnostics;
using OpenTK.Input;

namespace OpenTK.Platform.SDL2
{
    // Note: we cannot create a background window to retrieve events,
    // because X11 doesn't deliver core pointer events to background
    // windows (unless we grab, which will break *everything*).
    // The only solution is to poll.
    // Note 2: this driver only supports absolute positions. Relative motion
    // is faked through SetPosition. This is called automatically when
    // NativeWindow.CursorVisible = false, otherwise it must be called
    // by the user.
    // Note 3: this driver cannot drive the mouse wheel reliably.
    // See comments in ProcessEvents() for more information.
    // (If someone knows of a solution, please tell!)
    sealed class SDL2Mouse : IMouseDriver2
    {
        MouseState mouse = new MouseState();

		public static SDL2Mouse newestMouse;

        public SDL2Mouse()
        {
            mouse.IsConnected = true;
			newestMouse = this;
        }

		~SDL2Mouse ()
		{
			newestMouse = null;
		}

        public MouseState GetState()
        {
            return mouse;
        }

        public MouseState GetState(int index)
        {
            // SDL2Mouse supports only one device at the moment.
			// TODO: Support multiple mice
            if (index == 0)
                return mouse;
            else
                return new MouseState();
        }

        public void SetPosition(double x, double y)
        {
            // Update the current location, otherwise the pointer
            // may become locked (for instance, if we call
            // SetPosition too often, like X11GLNative does).


        }

        void WriteBit(MouseButton offset, bool enabled)
        {
            if (enabled)
                mouse.EnableBit((int)offset);
            else
                mouse.DisableBit((int)offset);
        }

             internal void ProcessEvent(ref API.Event e)
        {
            switch (e.type)
            {
                case API.EventType.MouseButtonDown:
                    if      (e.button.button == 1) WriteBit(MouseButton.Left, true);
                    else if (e.button.button == 2) WriteBit(MouseButton.Middle, true);
                    else if (e.button.button == 3) WriteBit(MouseButton.Right, true);
                    else if (e.button.button == 4) WriteBit(MouseButton.Button1, true);
                    else if (e.button.button == 5) WriteBit(MouseButton.Button2, true);
                    else if (e.button.button == 6) WriteBit(MouseButton.Button3, true);
                    else if (e.button.button == 7) WriteBit(MouseButton.Button4, true);
                    else if (e.button.button == 8) WriteBit(MouseButton.Button5, true);
                    else if (e.button.button == 9) WriteBit(MouseButton.Button6, true);
                    else if (e.button.button == 10) WriteBit(MouseButton.Button7, true);
                    else if (e.button.button == 11) WriteBit(MouseButton.Button8, true);
                    else if (e.button.button == 12) WriteBit(MouseButton.Button9, true);
                    
                    break;

                case API.EventType.MouseButtonUp:
                    if      (e.button.button == 1) WriteBit(MouseButton.Left, false);
                    else if (e.button.button == 2) WriteBit(MouseButton.Middle, false);
                    else if (e.button.button == 3) WriteBit(MouseButton.Right, false);
                    else if (e.button.button == 4) WriteBit(MouseButton.Button1, false);
                    else if (e.button.button == 5) WriteBit(MouseButton.Button2, false);
                    else if (e.button.button == 6) WriteBit(MouseButton.Button3, false);
                    else if (e.button.button == 7) WriteBit(MouseButton.Button4, false);
                    else if (e.button.button == 8) WriteBit(MouseButton.Button5, false);
                    else if (e.button.button == 9) WriteBit(MouseButton.Button6, false);
                    else if (e.button.button == 10) WriteBit(MouseButton.Button7, false);
                    else if (e.button.button == 11) WriteBit(MouseButton.Button8, false);
                    else if (e.button.button == 12) WriteBit(MouseButton.Button9, false);
                    break;

                case API.EventType.MouseMotion:
					mouse.X = e.motion.x;
					mouse.Y = e.motion.y;
                    break;

				case API.EventType.MouseWheel:
					mouse.WheelPrecise += e.wheel.y;
					break;

            }
        }

    }
}

