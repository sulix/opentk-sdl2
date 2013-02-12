#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2008 the Open Toolkit library, except where noted.
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
using System.Runtime.InteropServices;
using OpenTK.Input;

namespace OpenTK.Platform.SDL2
{
	struct SDL2JoyDetails { IntPtr handle; }

    sealed class SDL2Joystick : IJoystickDriver
    {
        #region Fields

        List<JoystickDevice> sticks = new List<JoystickDevice>();
		List<IntPtr> joystickHandles = new List<IntPtr>();
        IList<JoystickDevice> sticks_readonly;

        bool disposed;

        #endregion

        #region Constructors

        public SDL2Joystick ()
		{
			sticks_readonly = sticks.AsReadOnly ();

			int number = 0, max_sticks;
			lock (API.sdl_api_lock) {
				API.Init (API.INIT_JOYSTICKS);
				max_sticks = API.NumJoysticks ();
			}
            while (number < max_sticks)
            {
                JoystickDevice stick = OpenJoystick(number++);
                if (stick != null)
                {
					stick.Description = API.JoystickNameForIndex(number - 1);
                    sticks.Add(stick);
                }
            }

        }

        #endregion

        #region IJoystickDriver

        public int DeviceCount
        {
            get { return sticks.Count; }
        }

        public IList<JoystickDevice> Joysticks
        {
            get { return sticks_readonly; }
        }

        public void Poll()
        {
            foreach (JoystickDevice js in sticks)
            {
				lock(API.sdl_api_lock)
				{
					API.JoystickUpdate();
					for (int axis = 0; axis < js.Axis.Count; ++axis)
					{
						js.SetAxis((JoystickAxis)axis, API.JoystickGetAxis(joystickHandles[js.Id],axis)/32767.0f);
					}

					for (int button = 0; button < js.Button.Count; ++button)
					{
						js.SetButton((JoystickButton)button, API.JoystickGetButton(joystickHandles[js.Id], button) != 0);
					}


				}
                
            }
        }

        #endregion

        #region Private Members

        JoystickDevice<SDL2JoyDetails> OpenJoystick (int number)
		{
			JoystickDevice<SDL2JoyDetails> stick = null;

			int num_axes, num_buttons;
			IntPtr joystick;
            
			lock (API.sdl_api_lock) {
				joystick = API.JoystickOpen (number);
				num_axes = API.JoystickNumAxes (joystick);
				num_buttons = API.JoystickNumButtons (joystick);
			}

            stick = new JoystickDevice<SDL2JoyDetails>(number, num_axes, num_buttons);
			joystickHandles.Add(joystick);
            

            return stick;
        }

		#endregion


        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool manual)
        {
            if (!disposed)
            {
                disposed = true;
            }
        }

        ~SDL2Joystick()
        {
            Dispose(false);
        }

        #endregion
    }
}
