#region --- License ---
/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 */
#endregion

using System;
using System.Collections.Generic;
#if !MINIMAL
using System.Drawing;
#endif
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

using OpenTK.Input;

namespace OpenTK.Platform.SDL2
{
    /// \internal
    /// <summary>
    /// Drives the InputDriver on X11.
    /// This class supports OpenTK, and is not intended for users of OpenTK.
    /// </summary>
    internal sealed class SDL2Input : IInputDriver
    {
        KeyboardDevice keyboard = new KeyboardDevice();
        MouseDevice mouse = new MouseDevice();
        List<KeyboardDevice> dummy_keyboard_list = new List<KeyboardDevice>();
        List<MouseDevice> dummy_mice_list = new List<MouseDevice>();

        

        #region --- Constructors ---

        public SDL2Input(IWindowInfo attach)
        {
			mouse.Description = "Default SDL2 legacy mouse";
            mouse.DeviceID = IntPtr.Zero;
            mouse.NumberOfButtons = 12;
            mouse.NumberOfWheels = 1;
            dummy_mice_list.Add(mouse);

        }

        #endregion


        #region --- IInputDriver Members ---

        #region public IList<Keyboard> Keyboard

        public IList<KeyboardDevice> Keyboard
        {
            get { return dummy_keyboard_list;  }//return keyboardDriver.Keyboard;
        }

        #endregion

        #region public IList<Mouse> Mouse

        public IList<MouseDevice> Mouse
        {
            get { return dummy_mice_list; } //return mouseDriver.Mouse;
        }

        #endregion

        #region public IList<JoystickDevice> Joysticks

        public IList<JoystickDevice> Joysticks
        {
            get { return null; }
        }

        #endregion


		#region internal void ProcessEvent(ref API.Event e)

        internal void ProcessEvent(ref API.Event e)
        {
            switch (e.type)
            {
                case API.EventType.KeyDown:
                case API.EventType.KeyUp:
					//TODO: Make this work
                    bool pressed = e.type == API.EventType.KeyDown;

					break;

                case API.EventType.MouseButtonDown:
                    if      (e.button.button == 1) mouse[OpenTK.Input.MouseButton.Left] = true;
                    else if (e.button.button == 2) mouse[OpenTK.Input.MouseButton.Middle] = true;
                    else if (e.button.button == 3) mouse[OpenTK.Input.MouseButton.Right] = true;
                    else if (e.button.button == 4) mouse[OpenTK.Input.MouseButton.Button1] = true;
                    else if (e.button.button == 5) mouse[OpenTK.Input.MouseButton.Button2] = true;
                    else if (e.button.button == 6) mouse[OpenTK.Input.MouseButton.Button3] = true;
                    else if (e.button.button == 7) mouse[OpenTK.Input.MouseButton.Button4] = true;
                    else if (e.button.button == 8) mouse[OpenTK.Input.MouseButton.Button5] = true;
                    else if (e.button.button == 9) mouse[OpenTK.Input.MouseButton.Button6] = true;
                    else if (e.button.button == 10) mouse[OpenTK.Input.MouseButton.Button7] = true;
                    else if (e.button.button == 11) mouse[OpenTK.Input.MouseButton.Button8] = true;
                    else if (e.button.button == 12) mouse[OpenTK.Input.MouseButton.Button9] = true;
                    
                    break;

                case API.EventType.MouseButtonUp:
                    if      (e.button.button == 1) mouse[OpenTK.Input.MouseButton.Left] = false;
                    else if (e.button.button == 2) mouse[OpenTK.Input.MouseButton.Middle] = false;
                    else if (e.button.button == 3) mouse[OpenTK.Input.MouseButton.Right] = false;
                    else if (e.button.button == 4) mouse[OpenTK.Input.MouseButton.Button1] = false;
                    else if (e.button.button == 5) mouse[OpenTK.Input.MouseButton.Button2] = false;
                    else if (e.button.button == 6) mouse[OpenTK.Input.MouseButton.Button3] = false;
                    else if (e.button.button == 7) mouse[OpenTK.Input.MouseButton.Button4] = false;
                    else if (e.button.button == 8) mouse[OpenTK.Input.MouseButton.Button5] = false;
                    else if (e.button.button == 9) mouse[OpenTK.Input.MouseButton.Button6] = false;
                    else if (e.button.button == 10) mouse[OpenTK.Input.MouseButton.Button7] = false;
                    else if (e.button.button == 11) mouse[OpenTK.Input.MouseButton.Button8] = false;
                    else if (e.button.button == 12) mouse[OpenTK.Input.MouseButton.Button9] = false;
                    break;

                case API.EventType.MouseMotion:
                    mouse.Position = new Point(e.motion.x, e.motion.y);
                    break;

				case API.EventType.MouseWheel:
					mouse.Wheel += e.wheel.y;
					break;

            }
        }

        #endregion

        #region public void Poll()

        /// <summary>
        /// Polls and updates state of all keyboard, mouse and joystick devices.
        /// </summary>
        public void Poll()
        {
            
        }

        #endregion

        #endregion

        #region --- IDisposable Members ---

        public void Dispose()
        {

        }


        #endregion
    }
}
