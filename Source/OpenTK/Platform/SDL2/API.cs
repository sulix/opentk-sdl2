#region --- License ---
/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * Contributions from Erik Ylvisaker
 * (This SDL2 part by David Gow)
 * See license.txt for license info
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

#pragma warning disable 3019    // CLS-compliance checking
#pragma warning disable 0649    // struct members not explicitly initialized
#pragma warning disable 0169    // field / method is never used.
#pragma warning disable 0414    // field assigned but never used.

namespace OpenTK.Platform.SDL2
{
    #region Types



    #endregion

    #region internal static class API

    internal static class API
    {
        #region --- Fields ---

        private const string _dll_name = "libSDL2";
        
		public static Object sdl_api_lock = new Object();

  		#endregion

		#region SDL.h

		public const UInt32 INIT_TIMER = 0x00000001;
		public const UInt32 INIT_AUDIO = 0x00000010;
		public const UInt32 INIT_VIDEO = 0x00000020;
		public const UInt32 INIT_JOYSTICKS = 0x00000200;
		public const UInt32 INIT_HAPTIC = 0x00001000;
		public const UInt32 INIT_GAMECONTROLLER = 0x00002000;
		public const UInt32 INIT_NOPARACHUTE = 0x00100000;
		public const UInt32 INIT_EVERYTHING = 0x0000FFFF;

		[DllImport(_dll_name, EntryPoint = "SDL_Init")]
		extern public static int Init(UInt32 flags);

		[DllImport(_dll_name, EntryPoint = "SDL_InitSubSystem")]
		extern public static int InitSubSystem(UInt32 flags);

		[DllImport(_dll_name, EntryPoint = "SDL_QuitSubSystem")]
		extern public static void QuitSubSystem(UInt32 flags);

		[DllImport(_dll_name, EntryPoint = "SDL_WasInit")]
		extern public static UInt32 WasInit(UInt32 flags);

		#endregion

		#region SDL_rect

		[StructLayout(LayoutKind.Sequential)]
		internal struct Point
		{
			int x;
			int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct Rect
		{
			int x, y;
			int w, h;
		}


		//TODO: Like a boatload of other functions I don't care that much about.

		#endregion

		#region SDL_video

		[StructLayout(LayoutKind.Sequential)]
		internal struct DisplayMode
		{
			UInt32 format;
			int w;
			int h;
			int refresh_rate;
			IntPtr driverdata;
		}

		[Flags]
		internal enum WindowFlags// : uint //Uint32
		{
			Fullscreen = 0x00000001,
			OpenGL = 0x00000002,
			Shown = 0x00000004,
			Hidden = 0x00000008,
			Borderless = 0x00000010,
			Resizable = 0x00000020,
			Minimized = 0x00000040,
			Maximized = 0x00000080,
			InputGrabbed = 0x00000100,
			InputFocus = 0x00000200,
			MouseFocus = 0x00000400,
			FullscreenDesktop = 0x00001001,
			Foreign = 0x00000800
		}

		internal enum WindowEventId// : int
		{
			None,
			Shown,
			Hidden,
			Exposed,
			Moved,
			Resized,
			SizeChanged,
			Minimized,
			Maximized,
			Restored,
			Enter,
			Leave,
			FocusGained,
			FocusLost,
			Close
		}

		internal enum GLAttr// : int
		{
			RedSize,
			GreenSize,
			BlueSize,
			AlphaSize,
			BufferSize,
			DoubleBuffer,
			DepthSize,
			StencilSize,
			AccumRedSize,
			AccumGreenSize,
			AccumBlueSize,
			AccumAlphaSize,
			Stereo,
			MultisampleBuffers,
			MultisampleSamples,
			AcceleratedVisual,
			RetainedBacking,
			ContextMajorVersion,
			ContextMinorVersion,
			ContextEGL,
			ContextFlags,
			ContextProfileMask,
			ShareWithCurrentContext
		}

		[Flags]
		internal enum GLProfile// : int
		{
			Core = 0x0001,
			Compatibility = 0x0002,
			ES = 0x0004
		}

		[Flags]
		internal enum GLContextFlag// : int
		{
			Debug = 0x0001,
			ForwardCompatible = 0x0002,
			RobustAccess = 0x0004,
			ResetIsolation = 0x0008
		}

		[DllImport(_dll_name, EntryPoint = "SDL_GetNumVideoDrivers")]
		extern public static int GetNumVideoDrivers();

		[DllImport(_dll_name, EntryPoint = "SDL_GetVideoDriver")]
		extern public static string GetVideoDriver(int index);

		[DllImport(_dll_name, EntryPoint = "SDL_VideoInit")]
		extern public static int VideoInit(string driver_name, int flags);

		[DllImport(_dll_name, EntryPoint = "SDL_VideoQuit")]
		extern public static void VideoQuit();

		[DllImport(_dll_name, EntryPoint = "SDL_GetCurrentVideoDriver")]
		extern public static string GetCurrentVideoDriver();

		[DllImport(_dll_name, EntryPoint = "SDL_GetNumVideoDisplays")]
		extern public static int GetNumVideoDisplays();

		[DllImport(_dll_name, EntryPoint = "SDL_GetDisplayBounds")]
		extern public static int GetDisplayBounds(int displayIndex, out API.Rect rect);

		[DllImport(_dll_name, EntryPoint = "SDL_GetNumDisplayModes")]
		extern public static int GetNumDisplayModes(int displayIndex);

		[DllImport(_dll_name, EntryPoint = "SDL_GetDisplayMode")]
		extern public static int GetDisplayMode(int displayIndex, int modeIndex, out API.Rect mode);

		[DllImport(_dll_name, EntryPoint = "SDL_GetDesktopDisplayMode")]
		extern public static int GetDesktopDisplayMode(int displayIndex, out API.Rect mode);

		[DllImport(_dll_name, EntryPoint = "SDL_GetCurrentDisplayMode")]
		extern public static int GetCurrentDisplayMode(int displayIndex, out API.Rect mode);

		//TODO: GetClosestDisplayMode

		[DllImport(_dll_name, EntryPoint = "SDL_GetWindowDisplay")]
		extern public static int GetWindowDisplay(IntPtr window);

		//TODO: Set/Get WindowDisplayMode

		//TODO: GetWindowPixelFormat

		[DllImport(_dll_name, EntryPoint = "SDL_CreateWindow")]
		extern public static IntPtr CreateWindow(string title, int x, int y, int w, int h, WindowFlags flags);

		[DllImport(_dll_name, EntryPoint = "SDL_CreateWindowFrom")]
		extern public static IntPtr CreateWindowFrom(IntPtr data);

		[DllImport(_dll_name, EntryPoint = "SDL_GetWindowID")]
		extern public static UInt32 GetWindowID(IntPtr window);

		[DllImport(_dll_name, EntryPoint = "SDL_GetWindowFromID")]
		extern public static IntPtr GetWindowFromID(UInt32 id);

		[DllImport(_dll_name, EntryPoint = "SDL_GetWindowFlags")]
		extern public static WindowFlags GetWindowFlags(IntPtr window);

		//TODO: Ensure strings here are UTF-8

		[DllImport(_dll_name, EntryPoint = "SDL_SetWindowTitle")]
		extern public static void SetWindowTitle(IntPtr window, string title);

		[DllImport(_dll_name, EntryPoint = "SDL_GetWindowTitle")]
		extern public static string GetWindowTitle(IntPtr window);

		//TODO: SetWindowIcon, Set/Get WindowData

		[DllImport(_dll_name, EntryPoint = "SDL_SetWindowPosition")]
		extern public static void SetWindowPosition(IntPtr window, int x, int y);

		[DllImport(_dll_name, EntryPoint = "SDL_GetWindowPosition")]
		extern public static void GetWindowPosition(IntPtr window, out int x, out int y);

		[DllImport(_dll_name, EntryPoint = "SDL_SetWindowSize")]
		extern public static void SetWindowSize(IntPtr window, int x, int y);

		[DllImport(_dll_name, EntryPoint = "SDL_GetWindowSize")]
		extern public static void GetWindowSize(IntPtr window, out int x, out int y);

		[DllImport(_dll_name, EntryPoint = "SDL_SetWindowMinimumSize")]
		extern public static void SetWindowMinimumSize(IntPtr window, int x, int y);

		[DllImport(_dll_name, EntryPoint = "SDL_GetWindowMinimumSize")]
		extern public static void GetWindowMinimumSize(IntPtr window, out int x, out int y);

		[DllImport(_dll_name, EntryPoint = "SDL_SetWindowBordered")]
		extern public static void SetWindowBordered(IntPtr window, bool bordered);

		[DllImport(_dll_name, EntryPoint = "SDL_ShowWindow")]
		extern public static void ShowWindow(IntPtr window);

		[DllImport(_dll_name, EntryPoint = "SDL_HideWindow")]
		extern public static void HideWindow(IntPtr window);

		[DllImport(_dll_name, EntryPoint = "SDL_RaiseWindow")]
		extern public static void RaiseWindow(IntPtr window);

		//TODO: Maximize/Minimize/Restore Window

		[DllImport(_dll_name, EntryPoint = "SDL_SetWindowFullscreen")]
		extern public static int SetWindowFullscreen(IntPtr window, WindowFlags flags);

		//TODO *WindowSurface*

		[DllImport(_dll_name, EntryPoint = "SDL_SetWindowGrab")]
		extern public static void SetWindowGrab(IntPtr window, bool grabbed);

		[DllImport(_dll_name, EntryPoint = "SDL_GetWindowGrab")]
		extern public static bool GetWindowGrab(IntPtr window);

		[DllImport(_dll_name, EntryPoint = "SDL_SetWindowBrightness")]
		extern public static int SetWindowBrightness(IntPtr window, float brightness);

		[DllImport(_dll_name, EntryPoint = "SDL_GetWindowBrightness")]
		extern public static float GetWindowBrightness(IntPtr window);

		//TODO: Gamma

		[DllImport(_dll_name, EntryPoint = "SDL_DestroyWindow")]
		extern public static void DestroyWindow(IntPtr window);

		[DllImport(_dll_name, EntryPoint = "SDL_IsScreenSaverEnabled")]
		extern public static bool IsScreenSaverEnabled();

		[DllImport(_dll_name, EntryPoint = "SDL_EnableScreenSaver")]
		extern public static void EnableScreenSaver();

		[DllImport(_dll_name, EntryPoint = "SDL_DisableScreenSaver")]
		extern public static void DisableScreenSaver();

		// OpenGL support functions

		[DllImport(_dll_name, EntryPoint = "SDL_GL_LoadLibrary")]
		extern public static void GL_LoadLibrary(string path);

		[DllImport(_dll_name, EntryPoint = "SDL_GL_GetProcAddress")]
		extern public static IntPtr GL_GetProcAddress(string proc);

		[DllImport(_dll_name, EntryPoint = "SDL_GL_UnloadLibrary")]
		extern public static void GL_UnloadLibrary();

		[DllImport(_dll_name, EntryPoint = "SDL_GL_ExtensionSupported")]
		extern public static bool GL_ExtensionSupported(string extension);

		[DllImport(_dll_name, EntryPoint = "SDL_GL_SetAttribute")]
		extern public static int GL_SetAttribute(API.GLAttr attr, int value);

		[DllImport(_dll_name, EntryPoint = "SDL_GL_GetAttribute")]
		extern public static int GL_SetAttribute(API.GLAttr attr, out int value);

		[DllImport(_dll_name, EntryPoint = "SDL_GL_CreateContext")]
		extern public static IntPtr GL_CreateContext(IntPtr window);

		[DllImport(_dll_name, EntryPoint = "SDL_GL_MakeCurrent")]
		extern public static int GL_MakeCurrent(IntPtr window, IntPtr context);

		[DllImport(_dll_name, EntryPoint = "SDL_GL_SetSwapInterval")]
		extern public static int GL_SetSwapInterval(int interval);

		[DllImport(_dll_name, EntryPoint = "SDL_GL_GetSwapInterval")]
		extern public static int GL_GetSwapInterval();

		[DllImport(_dll_name, EntryPoint = "SDL_GL_SwapWindow")]
		extern public static void GL_SwapWindow(IntPtr window);

		[DllImport(_dll_name, EntryPoint = "SDL_GL_DeleteContext")]
		extern public static void GL_DeleteContext(IntPtr context);
		#endregion

		#region SDL_scancode
		public enum Scancode
		{
			UNKNOWN = 0,
		    A = 4,
		    B = 5,
		    C = 6,
		    D = 7,
		    E = 8,
		    F = 9,
		    G = 10,
		    H = 11,
		    I = 12,
		    J = 13,
		    K = 14,
		    L = 15,
		    M = 16,
		    N = 17,
		    O = 18,
		    P = 19,
		    Q = 20,
		    R = 21,
		    S = 22,
		    T = 23,
		    U = 24,
		    V = 25,
		    W = 26,
		    X = 27,
		    Y = 28,
		    Z = 29,

		    KBD_1 = 30,
		    KBD_2 = 31,
		    KBD_3 = 32,
		    KBD_4 = 33,
		    KBD_5 = 34,
		    KBD_6 = 35,
		    KBD_7 = 36,
		    KBD_8 = 37,
		    KBD_9 = 38,
		    KBD_0 = 39,

		    RETURN = 40,
		    ESCAPE = 41,
		    BACKSPACE = 42,
		    TAB = 43,
		    SPACE = 44,

		    MINUS = 45,
		    EQUALS = 46,
		    LEFTBRACKET = 47,
		    RIGHTBRACKET = 48,
		    BACKSLASH = 49,
		    NONUSHASH = 50,
		    SEMICOLON = 51,
		    APOSTROPHE = 52,
		    GRAVE = 53, 
		    COMMA = 54,
		    PERIOD = 55,
		    SLASH = 56,

		    CAPSLOCK = 57,

		    F1 = 58,
		    F2 = 59,
		    F3 = 60,
		    F4 = 61,
		    F5 = 62,
		    F6 = 63,
		    F7 = 64,
		    F8 = 65,
		    F9 = 66,
		    F10 = 67,
		    F11 = 68,
		    F12 = 69,

		    PRINTSCREEN = 70,
		    SCROLLLOCK = 71,
		    PAUSE = 72,
		    INSERT = 73, 
		    HOME = 74,
		    PAGEUP = 75,
		    DELETE = 76,
		    END = 77,
		    PAGEDOWN = 78,
		    RIGHT = 79,
		    LEFT = 80,
		    DOWN = 81,
		    UP = 82,

		    NUMLOCKCLEAR = 83,
		    KP_DIVIDE = 84,
		    KP_MULTIPLY = 85,
		    KP_MINUS = 86,
		    KP_PLUS = 87,
		    KP_ENTER = 88,
		    KP_1 = 89,
		    KP_2 = 90,
		    KP_3 = 91,
		    KP_4 = 92,
		    KP_5 = 93,
		    KP_6 = 94,
		    KP_7 = 95,
		    KP_8 = 96,
		    KP_9 = 97,
		    KP_0 = 98,
		    KP_PERIOD = 99,

		    NONUSBACKSLASH = 100,
		    APPLICATION = 101,
		    POWER = 102,
		    KP_EQUALS = 103,
		    F13 = 104,
		    F14 = 105,
		    F15 = 106,
		    F16 = 107,
		    F17 = 108,
		    F18 = 109,
		    F19 = 110,
		    F20 = 111,
		    F21 = 112,
		    F22 = 113,
		    F23 = 114,
		    F24 = 115,
		    EXECUTE = 116,
		    HELP = 117,
		    MENU = 118,
		    SELECT = 119,
		    STOP = 120,
		    AGAIN = 121,
		    UNDO = 122,
		    CUT = 123,
		    COPY = 124,
		    PASTE = 125,
		    FIND = 126,
		    MUTE = 127,
		    VOLUMEUP = 128,
		    VOLUMEDOWN = 129,
		    KP_COMMA = 133,
		    KP_EQUALSAS400 = 134,

		    INTERNATIONAL1 = 135,
		    INTERNATIONAL2 = 136,
		    INTERNATIONAL3 = 137,
		    INTERNATIONAL4 = 138,
		    INTERNATIONAL5 = 139,
		    INTERNATIONAL6 = 140,
		    INTERNATIONAL7 = 141,
		    INTERNATIONAL8 = 142,
		    INTERNATIONAL9 = 143,
		    LANG1 = 144,
		    LANG2 = 145, 
		    LANG3 = 146, 
		    LANG4 = 147,
		    LANG5 = 148, 
		    LANG6 = 149,
		    LANG7 = 150, 
		    LANG8 = 151,
		    LANG9 = 152, 

		    ALTERASE = 153,
		    SYSREQ = 154,
		    CANCEL = 155,
		    CLEAR = 156,
		    PRIOR = 157,
		    RETURN2 = 158,
		    SEPARATOR = 159,
		    OUT = 160,
		    OPER = 161,
		    CLEARAGAIN = 162,
		    CRSEL = 163,
		    EXSEL = 164,

		    KP_00 = 176,
		    KP_000 = 177,
		    THOUSANDSSEPARATOR = 178,
		    DECIMALSEPARATOR = 179,
		    CURRENCYUNIT = 180,
		    CURRENCYSUBUNIT = 181,
		    KP_LEFTPAREN = 182,
		    KP_RIGHTPAREN = 183,
		    KP_LEFTBRACE = 184,
		    KP_RIGHTBRACE = 185,
		    KP_TAB = 186,
		    KP_BACKSPACE = 187,
		    KP_A = 188,
		    KP_B = 189,
		    KP_C = 190,
		    KP_D = 191,
		    KP_E = 192,
		    KP_F = 193,
		    KP_XOR = 194,
		    KP_POWER = 195,
		    KP_PERCENT = 196,
		    KP_LESS = 197,
		    KP_GREATER = 198,
		    KP_AMPERSAND = 199,
		    KP_DBLAMPERSAND = 200,
		    KP_VERTICALBAR = 201,
		    KP_DBLVERTICALBAR = 202,
		    KP_COLON = 203,
		    KP_HASH = 204,
		    KP_SPACE = 205,
		    KP_AT = 206,
		    KP_EXCLAM = 207,
		    KP_MEMSTORE = 208,
		    KP_MEMRECALL = 209,
		    KP_MEMCLEAR = 210,
		    KP_MEMADD = 211,
		    KP_MEMSUBTRACT = 212,
		    KP_MEMMULTIPLY = 213,
		    KP_MEMDIVIDE = 214,
		    KP_PLUSMINUS = 215,
		    KP_CLEAR = 216,
		    KP_CLEARENTRY = 217,
		    KP_BINARY = 218,
		    KP_OCTAL = 219,
		    KP_DECIMAL = 220,
		    KP_HEXADECIMAL = 221,

		    LCTRL = 224,
		    LSHIFT = 225,
		    LALT = 226, /**< alt, option */
		    LGUI = 227, /**< windows, command (apple), meta */
		    RCTRL = 228,
		    RSHIFT = 229,
		    RALT = 230, /**< alt gr, option */
		    RGUI = 231, /**< windows, command (apple), meta */

		    MODE = 257,
		    AUDIONEXT = 258,
		    AUDIOPREV = 259,
		    AUDIOSTOP = 260,
		    AUDIOPLAY = 261,
		    AUDIOMUTE = 262,
		    MEDIASELECT = 263,
		    WWW = 264,
		    MAIL = 265,
		    CALCULATOR = 266,
		    COMPUTER = 267,
		    AC_SEARCH = 268,
		    AC_HOME = 269,
		    AC_BACK = 270,
		    AC_FORWARD = 271,
		    AC_STOP = 272,
		    AC_REFRESH = 273,
		    AC_BOOKMARKS = 274,

		    BRIGHTNESSDOWN = 275,
		    BRIGHTNESSUP = 276,
		    DISPLAYSWITCH = 277, 
		                                         
		    KBDILLUMTOGGLE = 278,
		    KBDILLUMDOWN = 279,
		    KBDILLUMUP = 280,
		    EJECT = 281,
		    SLEEP = 282,

		    SDL_NUM_SCANCODES = 512 
		};


		#endregion

		#region SDL_keyboard

		[StructLayout(LayoutKind.Sequential)]
		public struct Keysym
		{
			public Scancode scancode;
			public UInt32 sym; //TODO: Implement
			public UInt16 mod;
			public UInt32 unicode; //Deprecated
		}

		[DllImport(_dll_name, EntryPoint = "SDL_GetKeyboardFocus")]
		extern public static IntPtr GetKeyboardFocus();

		//TODO: Implement many functions

		[DllImport(_dll_name, EntryPoint = "SDL_GetScancodeName")]
		extern public static string GetScancodeName(Scancode scancode);

		[DllImport(_dll_name, EntryPoint = "SDL_GetScancodeFromName")]
		extern public static Scancode GetScancodeFromName(string name);

		//TODO: Keycode + Text input

		[DllImport(_dll_name, EntryPoint = "SDL_HasScreenKeyboardSupport")]
		extern public static bool HasScreenKeyboardSupport();

		//TODO: More stuff


		#endregion

		#region SDL_events

		internal enum EventType
		{
			FirstEvent = 0,
			Quit = 0x100,
			WindowEvent = 0x200,
			SysWMEvent,
			KeyDown = 0x300,
			KeyUp,
			TextEditing,
			TextInput,
			MouseMotion = 0x400,
			MouseButtonDown,
			MouseButtonUp,
			MouseWheel,
			InputMotion = 0x500,
			InputButtonDown,
			InputButtonUp,
			InputWheel,
			InputProximityIn,
			InputProximityOut,
			JoyAxisMotion = 0x600,
			JoyBallMotion,
			JoyHatMotion,
			JoyDeviceAdded,
			JoyDeviceRemoved,
			ControllerAxisMotion = 0x650,
			ControllerButtonDown,
			ControllerButtonUp,
			ControllerDeviceAdded,
			ControllerDeviceRemoved,
			FingerDown = 0x700,
			FingerUp,
			FingerMotion,
			TouchButtonDown,
			TouchButtonUp,
			DollarGesture = 0x800,
			DollarRecord,
			MultiGesture,
			ClipboardUpdate = 0x900,
			DropFile = 0x1000,
			UserEvent = 0x8000,
			LastEvent = 0xFFFF
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct WindowEvent
		{
			public UInt32 type;
			public UInt32 timestamp;
			public UInt32 windowID;
			public Byte eventid;
			public Byte padding1;
			public Byte padding2;
			public Byte padding3;
			public int data1;
			public int data2;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct KeyboardEvent
		{
			public UInt32 type;
			public UInt32 timestamp;
			public UInt32 windowID;
			public Byte state;
			public Byte repeat;
			public Byte padding2;
			public Byte padding3;
			public Keysym keysym;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct TextInputEvent
		{
			public UInt32 type;
			public UInt32 timestamp;
			public UInt32 windowID;
			//public char text[32]; //TODO: Work out how fixed sized arrays actually work in C#
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct MouseMotionEvent
		{
			public UInt32 type;
			public UInt32 timestamp;
			public UInt32 windowID;
			public Byte state;
			public Byte padding1;
			public Byte padding2;
			public Byte padding3;
			public int x;
			public int y;
			public int xrel;
			public int yrel;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct MouseButtonEvent
		{
			public UInt32 type;
			public UInt32 timestamp;
			public UInt32 windowID;
			public Byte button;
			public Byte state;
			public Byte padding1;
			public Byte padding2;
			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct MouseWheelEvent
		{
			public UInt32 type;
			public UInt32 timestamp;
			public UInt32 windowID;
			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct JoyAxisEvent
		{
			public UInt32 type;
			public UInt32 timestamp;
			public Byte which;
			public Byte axis;
			public Byte padding1;
			public Byte padding2;
			public int value;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct JoyBallEvent
		{
			public UInt32 type;
			public UInt32 timestamp;
			public Byte which;
			public Byte ball;
			public Byte padding1;
			public Byte padding2;
			public int xrel;
			public int yrel;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct JoyHatEvent
		{
			public UInt32 type;
			public UInt32 timestamp;
			public Byte which;
			public Byte hat;
			public Byte value;
			public Byte padding1;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct JoyButtonEvent
		{
			public UInt32 type;
			public UInt32 timestamp;
			public Byte which;
			public Byte button;
			public Byte state;
			public Byte padding1;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct JoyDeviceEvent
		{
			public UInt32 type;
			public UInt32 timestamp;
			public UInt32 which;
		}

		//TODO: Controller events, Touch events, Gesture events, Drop Event
		/*
		[StructLayout(LayoutKind.Sequential)]
		internal struct DropEvent
		{
			public UInt32 type;
			public UInt32 timestamp;
			public string file;
		}
		*/

		[StructLayout(LayoutKind.Sequential)]
		internal struct QuitEvent
		{
			public UInt32 type;
			public UInt32 timestamp;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct UserEvent
		{
			public UInt32 type;
			public UInt32 timestamp;
			public UInt32 windowID;
			public int code;
			public IntPtr data1;
			public IntPtr data2;
		}

		//Doing nasty union hacks
		[StructLayout(LayoutKind.Explicit)]
		public struct Event
		{
			[FieldOffset(0)]
			public EventType type;
			[FieldOffset(0)]
			public WindowEvent window;
			[FieldOffset(0)]
			public KeyboardEvent key;
			//[FieldOffset(0)]
			//public TextEditingEvent edit;
			[FieldOffset(0)]
			public TextInputEvent text;
			[FieldOffset(0)]
			public MouseMotionEvent motion;
			[FieldOffset(0)]
			public MouseButtonEvent button;
			[FieldOffset(0)]
			public MouseWheelEvent wheel;
			[FieldOffset(0)]
			public JoyAxisEvent jaxis;
			[FieldOffset(0)]
			public JoyBallEvent jball;
			[FieldOffset(0)]
			public JoyHatEvent jhat;
			[FieldOffset(0)]
			public JoyButtonEvent jbutton;
			[FieldOffset(0)]
			public JoyDeviceEvent jdevice;
			[FieldOffset(0)]
			public QuitEvent quit;
			[FieldOffset(0)]
			public UserEvent user;
		}

		[DllImport(_dll_name, EntryPoint = "SDL_PumpEvents")]
		public extern static void PumpEvents();

		internal enum EventAction
		{
			AddEvent,
			PeekEvent,
			GetEvent
		}

		//TODO: PeepEvents

		[DllImport(_dll_name, EntryPoint = "SDL_HasEvent")]
		extern public static bool HasEvent(EventType type);

		//TODO: HasEvents

		[DllImport(_dll_name, EntryPoint = "SDL_FlushEvent")]
		extern public static void FlushEvent(EventType type);

		//TODO: FlushEvents

		[DllImport(_dll_name, EntryPoint = "SDL_PollEvent")]
		extern public static int PollEvent(out Event _event);

		[DllImport(_dll_name, EntryPoint = "SDL_WaitEvent")]
		extern public static int WaitEvent(out Event _event);

		[DllImport(_dll_name, EntryPoint = "SDL_WaitEventTimeout")]
		extern public static int WaitEventTimeout(out Event _event, int timeout);

		[DllImport(_dll_name, EntryPoint = "SDL_PushEvent")]
		extern public static int PushEvent(ref Event _event);

		//TODO: Event filter stuff, event watch stuff, registerevents, etc

		#endregion

		#region SDL_mouse

		public enum SystemCursor
		{
			Arrow,
			IBeam,
			Wait,
			Crosshair,
			WaitArrow,
			SizeNWSE,
			SizeNESW,
			SizeWE,
			SizeNS,
			SizeAll,
			No,
			Hand,
			SDL_NUM_SYSTEM_CURSORS
		}

		[DllImport(_dll_name, EntryPoint = "SDL_GetMouseFocus")]
		extern public static IntPtr GetMouseFocus();

		[DllImport(_dll_name, EntryPoint = "SDL_GetMouseState")]
		extern public static UInt32 GetMouseState(out int x, out int y);

		[DllImport(_dll_name, EntryPoint = "SDL_GetRelativeMouseState")]
		extern public static UInt32 GetRelativeMouseState(out int x, out int y);

		[DllImport(_dll_name, EntryPoint = "SDL_WarpMouseInWindow")]
		extern public static void WarpMouseInWindow(IntPtr window, int x, int y);

		[DllImport(_dll_name, EntryPoint = "SDL_SetRelativeMouseMode")]
		extern public static int SetRelativeMouseMose(bool enabled);

		[DllImport(_dll_name, EntryPoint = "SDL_GetRelativeMouseMode")]
		extern public static bool GetRelativeMouseMode();

		//TODO: CreateCursor, CreateColorCursor

		[DllImport(_dll_name, EntryPoint = "SDL_CreateSystemCursor")]
		extern public static IntPtr CreateSystemCursor(SystemCursor id);

		[DllImport(_dll_name, EntryPoint = "SDL_SetCursor")]
		extern public static void SetCursor(IntPtr cursor);

		[DllImport(_dll_name, EntryPoint = "SDL_GetCursor")]
		extern public static IntPtr GetCursor();

		[DllImport(_dll_name, EntryPoint = "SDL_ShowCursor")]
		extern public static int ShowCursor(int toggle);

		#endregion

		#region SDL_joystick

		[DllImport(_dll_name, EntryPoint = "SDL_NumJoysticks")]
		extern public static int NumJoysticks();

		[DllImport(_dll_name, EntryPoint = "SDL_JoystickNameForIndex")]
		extern public static string JoystickNameForIndex(int device_index);

		[DllImport(_dll_name, EntryPoint = "SDL_JoystickOpen")]
		extern public static IntPtr JoystickOpen(int device_index);

		[DllImport(_dll_name, EntryPoint = "SDL_JoystickName")]
		extern public static string JoystickName(IntPtr joystick);

		//TODO: GUID stuff

		[DllImport(_dll_name, EntryPoint = "SDL_JoystickGetAttached")]
		extern public static bool JoystickGetAttached(IntPtr joystick);

		//TODO: Instance ID

		[DllImport(_dll_name, EntryPoint = "SDL_JoystickNumAxes")]
		extern public static int JoystickNumAxes(IntPtr joystick);

		[DllImport(_dll_name, EntryPoint = "SDL_JoystickNumBalls")]
		extern public static int JoystickNumBalls(IntPtr joystick);

		[DllImport(_dll_name, EntryPoint = "SDL_JoystickNumHats")]
		extern public static int JoystickNumHats(IntPtr joystick);

		[DllImport(_dll_name, EntryPoint = "SDL_JoystickNumButtons")]
		extern public static int JoystickNumButtons(IntPtr joystick);

		[DllImport(_dll_name, EntryPoint = "SDL_JoystickUpdate")]
		extern public static void JoystickUpdate();

		[DllImport(_dll_name, EntryPoint = "SDL_JoystickEventState")]
		extern public static int JoystickEventState(int state);

		[DllImport(_dll_name, EntryPoint = "SDL_JoystickGetAxis")]
		extern public static Int16 JoystickGetAxis(IntPtr joystick, int axis);

		[DllImport(_dll_name, EntryPoint = "SDL_JoystickGetHat")]
		extern public static Byte JoystickGetHat(IntPtr joystick, int hat);

		[DllImport(_dll_name, EntryPoint = "SDL_JoystickGetBall")]
		extern public static int JoystickGetBall(IntPtr joystick, int ball, out int dx, out int dy);

		[DllImport(_dll_name, EntryPoint = "SDL_JoystickGetButton")]
		extern public static Byte JoystickGetButton(IntPtr joystick, int button);

		[DllImport(_dll_name, EntryPoint = "SDL_JoystickClose")]
		extern public static void JoystickClose(IntPtr joystick);


		#endregion

		//TODO: Remove this for non-X11 based platforms
		[DllImport("libX11", EntryPoint = "XInitThreads")]
        public extern static int XInitThreads();

        static API ()
		{
			lock (sdl_api_lock) {
				XInitThreads ();
				Init (INIT_EVERYTHING);
			}
        }
	}
	#endregion

}

#pragma warning restore 3019
#pragma warning restore 0649
#pragma warning restore 0169
#pragma warning restore 0414
