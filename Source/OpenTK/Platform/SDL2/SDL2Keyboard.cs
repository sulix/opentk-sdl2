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
    // Standard keyboard driver that relies on xlib input events.
    // Only one keyboard supported.
    sealed class SDL2Keyboard : IKeyboardDriver2
    {
        KeyboardState state = new KeyboardState();
		SDL2KeyMap keymap = new SDL2KeyMap();
		public static SDL2Keyboard newestKeyboard;

        public SDL2Keyboard()
        {
			newestKeyboard = this;
        }

		~SDL2Keyboard ()
		{
			newestKeyboard = null;
		}

        public KeyboardState GetState()
        {
            ProcessEvents();
            return state;
        }

        public KeyboardState GetState(int index)
        {
            ProcessEvents();
            if (index == 0)
                return state;
            else
                return new KeyboardState();
        }

        public string GetDeviceName(int index)
        {
            if (index == 0)
                return "SDL2 Fake Keyboard";
            else
                return String.Empty;
        }

		internal void ProcessEvent(ref API.Event e)
        {
			Debug.Print("Scancode {0} -> {1}", e.key.keysym.scancode, keymap[e.key.keysym.scancode]);
            switch (e.type)
            {
                case API.EventType.KeyDown:
					if (keymap.ContainsKey(e.key.keysym.scancode))
                        state.EnableBit((int)keymap[e.key.keysym.scancode]);
                    else
                        Debug.Print("Scancode {0}", e.key.keysym.scancode);

					break;
                case API.EventType.KeyUp:
					//TODO: Make this work
                    if (keymap.ContainsKey(e.key.keysym.scancode))
                        state.DisableBit((int)keymap[e.key.keysym.scancode]);
                    else
                        Debug.Print("Scancode {0}", e.key.keysym.scancode);

					break;
					
            }
        }

        void ProcessEvents()
        {
            
        }
    }
}

