#region --- License ---
/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the OpenTK team.
 * SDL2 version by David Gow
 * This notice may not be removed.
 * See license.txt for licensing detailed licensing details.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using OpenTK.Input;

namespace OpenTK.Platform.SDL2
{
    internal class SDL2KeyMap : Dictionary<API.Scancode, Key>
    {
        internal SDL2KeyMap()
        {
            try
            {
                this.Add(API.Scancode.ESCAPE, Key.Escape);
                this.Add(API.Scancode.RETURN, Key.Enter);
                this.Add(API.Scancode.SPACE, Key.Space);
                this.Add(API.Scancode.BACKSPACE, Key.BackSpace);

                this.Add(API.Scancode.LSHIFT, Key.ShiftLeft);
                this.Add(API.Scancode.RSHIFT, Key.ShiftRight);
                this.Add(API.Scancode.LALT, Key.AltLeft);
                this.Add(API.Scancode.RALT, Key.AltRight);
                this.Add(API.Scancode.LCTRL, Key.ControlLeft);
                this.Add(API.Scancode.RCTRL, Key.ControlRight);
                this.Add(API.Scancode.LGUI, Key.WinLeft);
                this.Add(API.Scancode.RGUI, Key.WinRight);
                //TODO: AltGr?

                this.Add(API.Scancode.MENU, Key.Menu);
                this.Add(API.Scancode.TAB, Key.Tab);
                this.Add(API.Scancode.MINUS, Key.Minus);
                this.Add(API.Scancode.EQUALS, Key.Plus);

                this.Add(API.Scancode.CAPSLOCK, Key.CapsLock);
                this.Add(API.Scancode.NUMLOCKCLEAR, Key.NumLock);

                for (int i = (int)API.Scancode.F1; i <= (int)API.Scancode.F12; i++)
                {
                    this.Add((API.Scancode)i, (Key)((int)Key.F1 + (i - (int)API.Scancode.F1)));
                }

				for (int i = (int)API.Scancode.F13; i <= (int)API.Scancode.F24; i++)
                {
                    this.Add((API.Scancode)i, (Key)((int)Key.F1 + (i - (int)API.Scancode.F1)));
                }

                for (int i = (int)API.Scancode.A; i <= (int)API.Scancode.Z; i++)
                {
                    this.Add((API.Scancode)i, (Key)((int)Key.A + (i - (int)API.Scancode.A)));
                }

                for (int i = (int)API.Scancode.KBD_0; i <= (int)API.Scancode.KBD_9; i++)
                {
                    this.Add((API.Scancode)i, (Key)((int)Key.Number0 + (i - (int)API.Scancode.KBD_0)));
                }

                for (int i = (int)API.Scancode.KP_0; i <= (int)API.Scancode.KP_9; i++)
                {
                    this.Add((API.Scancode)i, (Key)((int)Key.Keypad0 + (i - (int)API.Scancode.KP_0)));
                }

                this.Add(API.Scancode.PAUSE, Key.Pause);
                this.Add(API.Scancode.PRINTSCREEN, Key.PrintScreen);
                
                this.Add(API.Scancode.BACKSLASH, Key.BackSlash);
				this.Add(API.Scancode.NONUSBACKSLASH, Key.BackSlash);
                this.Add(API.Scancode.LEFTBRACKET, Key.BracketLeft);
                this.Add(API.Scancode.RIGHTBRACKET, Key.BracketRight);
                this.Add(API.Scancode.SEMICOLON, Key.Semicolon);
                this.Add(API.Scancode.APOSTROPHE, Key.Quote);

                this.Add(API.Scancode.COMMA, Key.Comma);
                this.Add(API.Scancode.PERIOD, Key.Period);
                this.Add(API.Scancode.SLASH, Key.Slash);

                this.Add(API.Scancode.LEFT, Key.Left);
                this.Add(API.Scancode.DOWN, Key.Down);
                this.Add(API.Scancode.RIGHT, Key.Right);
                this.Add(API.Scancode.UP, Key.Up);

                this.Add(API.Scancode.DELETE, Key.Delete);
                this.Add(API.Scancode.HOME, Key.Home);
                this.Add(API.Scancode.END, Key.End);
                //this.Add(XKey.Prior, Key.PageUp);   // XKey.Prior == XKey.Page_Up
                this.Add(API.Scancode.PAGEUP, Key.PageUp);
                this.Add(API.Scancode.PAGEDOWN, Key.PageDown);
                //this.Add(XKey.Next, Key.PageDown);  // XKey.Next == XKey.Page_Down

                this.Add(API.Scancode.KP_PLUS, Key.KeypadAdd);
                this.Add(API.Scancode.KP_MINUS, Key.KeypadSubtract);
                this.Add(API.Scancode.KP_MULTIPLY, Key.KeypadMultiply);
                this.Add(API.Scancode.KP_DIVIDE, Key.KeypadDivide);
                this.Add(API.Scancode.KP_DECIMAL, Key.KeypadDecimal);
                this.Add(API.Scancode.KP_ENTER, Key.KeypadEnter);

            }
            catch (ArgumentException e)
            {
                Debug.Print("Exception while creating keymap: '{0}'.", e.ToString());
            }
        }
    }
}
