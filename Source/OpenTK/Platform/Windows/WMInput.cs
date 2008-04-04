﻿#region --- License ---
/* Copyright (c) 2007 Stefanos Apostolopoulos
 * See license.txt for license information
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using OpenTK.Input;
using System.Diagnostics;

namespace OpenTK.Platform.Windows
{
    /// <summary>
    /// Input driver for legacy (pre XP) Windows platforms.
    /// </summary>
    internal sealed class WMInput : NativeWindow, IInputDriver
    {
        // Driver supports only one keyboard and mouse;
        KeyboardDevice keyboard = new KeyboardDevice();
        MouseDevice mouse = new MouseDevice();
        IList<KeyboardDevice> keyboards = new List<KeyboardDevice>(1);
        IList<MouseDevice> mice = new List<MouseDevice>(1);
        internal static readonly WinKeyMap KeyMap = new WinKeyMap();
        // Used to distinguish left and right control, alt and enter keys.
        const long ExtendedBit = 1 << 24;
        // Used to distinguish left and right shift keys.
        static readonly uint ShiftRightScanCode = Functions.MapVirtualKey(VirtualKeys.RSHIFT, 0);

        #region --- Constructor ---

        public WMInput(WinWindowInfo parent)
        {
            Debug.WriteLine("Initalizing WMInput driver.");
            Debug.Indent();

            AssignHandle(parent.WindowHandle);
            Debug.Print("Input window attached to parent {0}", parent);
            
            Debug.Unindent();

            keyboard.Description = "Standard Windows keyboard";
            keyboard.NumberOfFunctionKeys = 12;
            keyboard.NumberOfKeys = 101;
            keyboard.NumberOfLeds = 3;

            mouse.Description = "Standard Windows mouse";
            mouse.NumberOfButtons = 3;
            mouse.NumberOfWheels = 1;

            keyboards.Add(keyboard);
            mice.Add(mouse);
        }

        #endregion

        #region protected override void WndProc(ref Message msg)

        protected override void WndProc(ref Message msg)
        {
            switch ((WindowMessage)msg.Msg)
            {
                // Mouse events:
                case WindowMessage.MOUSEMOVE:
                //case WindowMessage.NCMOUSEMOVE:
                    mouse.X = msg.LParam.ToInt32() & 0x0000FFFF;
                    mouse.Y = (int)(msg.LParam.ToInt32() & 0xFFFF0000) >> 16;
                    return;

                case WindowMessage.MOUSEWHEEL:
                    mouse.Wheel += (int)(msg.WParam.ToInt32() >> 16) / 120;
                    return;

                case WindowMessage.LBUTTONDOWN:
                    mouse[MouseButton.Left] = true;
                    return;

                case WindowMessage.MBUTTONDOWN:
                    mouse[MouseButton.Middle] = true;
                    return;

                case WindowMessage.RBUTTONDOWN:
                    mouse[MouseButton.Right] = true;
                    return;

                case WindowMessage.XBUTTONDOWN:
                    mouse[((msg.WParam.ToInt32() & 0xFFFF0000) >> 16) != (int)MouseKeys.XButton1 ? MouseButton.Button1 : MouseButton.Button2] = true;
                    return;

                case WindowMessage.LBUTTONUP:
                    mouse[MouseButton.Left] = false;
                    return;

                case WindowMessage.MBUTTONUP:
                    mouse[MouseButton.Middle] = false;
                    return;

                case WindowMessage.RBUTTONUP:
                    mouse[MouseButton.Right] = false;
                    return;

                case WindowMessage.XBUTTONUP:
                    // TODO: Is this correct?
                    mouse[((msg.WParam.ToInt32() & 0xFFFF0000) >> 16) != (int)MouseKeys.XButton1 ? MouseButton.Button1 : MouseButton.Button2] = false;
                    return;

                // Keyboard events:
                case WindowMessage.KEYDOWN:
                case WindowMessage.KEYUP:
                case WindowMessage.SYSKEYDOWN:
                case WindowMessage.SYSKEYUP:
                    bool pressed = (WindowMessage)msg.Msg == WindowMessage.KEYDOWN ||
                                   (WindowMessage)msg.Msg == WindowMessage.SYSKEYDOWN;

                    // Shift/Control/Alt behave strangely when e.g. ShiftRight is held down and ShiftLeft is pressed
                    // and released. It looks like neither key is released in this case, or that the wrong key is
                    // released in the case of Control and Alt.
                    // To combat this, we are going to release both keys when either is released. Hacky, but should work.
                    // Win95 does not distinguish left/right key constants (GetAsyncKeyState returns 0).
                    // In this case, both keys will be reported as pressed.

                    bool extended = (msg.LParam.ToInt64() & ExtendedBit) != 0;
                    switch ((VirtualKeys)msg.WParam)
                    {
                        case VirtualKeys.SHIFT:
                            // The behavior of this key is very strange. Unlike Control and Alt, there is no extended bit
                            // to distinguish between left and right keys. Moreover, pressing both keys and releasing one
                            // may result in both keys being held down (but not always).
                            // The only reliably way to solve this was reported by BlueMonkMN at the forums: we should
                            // check the scancodes. It looks like GLFW does the same thing, so it should be reliable.
                            
                            // TODO: Not 100% reliable, when both keys are pressed at once.
                            if (ShiftRightScanCode != 0)
                            {
                                if ((((int)msg.LParam >> 16) & 0xFF) == ShiftRightScanCode)
                                    keyboard[Input.Key.ShiftRight] = pressed;
                                else
                                    keyboard[Input.Key.ShiftLeft] = pressed;
                            }
                            else
                            {
                                // Should only fall here on Windows 9x and NT4.0-
                                keyboard[Input.Key.ShiftLeft] = pressed;
                            }
                            return;

                        case VirtualKeys.CONTROL:
                            if (extended)
                                keyboard[Input.Key.ControlRight] = pressed;
                            else
                                keyboard[Input.Key.ControlLeft] = pressed;
                            return;

                        case VirtualKeys.MENU:
                            if (extended)
                                keyboard[Input.Key.AltRight] = pressed;
                            else
                                keyboard[Input.Key.AltLeft] = pressed;
                            return;

                        case VirtualKeys.RETURN:
                            if (extended)
                                keyboard[Key.KeypadEnter] = pressed;
                            else
                                keyboard[Key.Enter] = pressed;
                            return;

                        default:
                            if (!WMInput.KeyMap.ContainsKey((VirtualKeys)msg.WParam))
                            {
                                Debug.Print("Virtual key {0} ({1}) not mapped.", (VirtualKeys)msg.WParam, (int)msg.WParam);
                                break;
                            }
                            else
                            {
                                keyboard[WMInput.KeyMap[(VirtualKeys)msg.WParam]] = pressed;
                                return;
                            }
                    }
                    break;

                case WindowMessage.KILLFOCUS:
                    keyboard.ClearKeys();
                    break;

                case WindowMessage.DESTROY:
                    Debug.Print("Input window detached from parent {0}.", Handle);
                    ReleaseHandle();
                    break;

                case WindowMessage.QUIT:
                    Debug.WriteLine("Input window quit.");
                    this.Dispose();
                    break;
            }

            base.WndProc(ref msg);
        }

        #endregion

        #region IInputDriver Members

        public void Poll()
        {
            // No polling needed.
        }

        #endregion

        #region IKeyboardDriver Members

        public IList<KeyboardDevice> Keyboard
        {
            get { return keyboards; }
        }

        #endregion

        #region IMouseDriver Members

        public IList<MouseDevice> Mouse
        {
            get { return mice; }
        }

        #endregion

        #region --- IDisposable Members ---

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                    this.ReleaseHandle();

                disposed = true;
            }
        }

        ~WMInput()
        {
            Dispose(false);
        }

        #endregion
    }
}
