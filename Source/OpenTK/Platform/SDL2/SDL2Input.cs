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
        List<KeyboardDevice> dummy_keyboard_list = new List<KeyboardDevice>(1);
        List<MouseDevice> dummy_mice_list = new List<MouseDevice>(1);

        

        #region --- Constructors ---

        /// <summary>
        /// Constructs a new X11Input driver. Creates a hidden InputOnly window, child to
        /// the main application window, which selects input events and routes them to 
        /// the device specific drivers (Keyboard, Mouse, Hid).
        /// </summary>
        /// <param name="attach">The window which the InputDriver will attach itself on.</param>
        public SDL2Input(IWindowInfo attach)
        {
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
            get { return (IList<MouseDevice>)dummy_mice_list; } //return mouseDriver.Mouse;
        }

        #endregion

        #region public IList<JoystickDevice> Joysticks

        public IList<JoystickDevice> Joysticks
        {
            get { return null; }
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
