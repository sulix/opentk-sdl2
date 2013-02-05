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

		//TODO: Grab, Brightness, Gamma

		[DllImport(_dll_name, EntryPoint = "SDL_DestroyWindow")]
		extern public static void DestroyWindow(IntPtr window);

		//TODO: ScreenSaver

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
			UInt32 type;
			UInt32 timestamp;
			UInt32 windowID;
			Byte eventid;
			Byte padding1;
			Byte padding2;
			Byte padding3;
			int data1;
			int data2;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct KeyboardEvent
		{
			UInt32 type;
			UInt32 timestamp;
			UInt32 windowID;
			Byte state;
			Byte repeat;
			Byte padding2;
			Byte padding3;
			int keysym; // TODO: Write Keysym
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct TextInputEvent
		{
			UInt32 type;
			UInt32 timestamp;
			UInt32 windowID;
			//char text[32]; //TODO: Work out how fixed sized arrays actually work in C#
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct MouseMotionEvent
		{
			UInt32 type;
			UInt32 timestamp;
			UInt32 windowID;
			Byte state;
			Byte padding1;
			Byte padding2;
			Byte padding3;
			int x;
			int y;
			int xrel;
			int yrel;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct MouseButtonEvent
		{
			UInt32 type;
			UInt32 timestamp;
			UInt32 windowID;
			Byte button;
			Byte state;
			Byte padding1;
			Byte padding2;
			int x;
			int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct MouseWheelEvent
		{
			UInt32 type;
			UInt32 timestamp;
			UInt32 windowID;
			int x;
			int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct JoyAxisEvent
		{
			UInt32 type;
			UInt32 timestamp;
			Byte which;
			Byte axis;
			Byte padding1;
			Byte padding2;
			int value;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct JoyBallEvent
		{
			UInt32 type;
			UInt32 timestamp;
			Byte which;
			Byte ball;
			Byte padding1;
			Byte padding2;
			int xrel;
			int yrel;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct JoyHatEvent
		{
			UInt32 type;
			UInt32 timestamp;
			Byte which;
			Byte hat;
			Byte value;
			Byte padding1;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct JoyButtonEvent
		{
			UInt32 type;
			UInt32 timestamp;
			Byte which;
			Byte button;
			Byte state;
			Byte padding1;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct JoyDeviceEvent
		{
			UInt32 type;
			UInt32 timestamp;
			UInt32 which;
		}

		//TODO: Controller events, Touch events, Gesture events, Drop Event
		/*
		[StructLayout(LayoutKind.Sequential)]
		internal struct DropEvent
		{
			UInt32 type;
			UInt32 timestamp;
			string file;
		}
		*/

		[StructLayout(LayoutKind.Sequential)]
		internal struct QuitEvent
		{
			UInt32 type;
			UInt32 timestamp;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct UserEvent
		{
			UInt32 type;
			UInt32 timestamp;
			UInt32 windowID;
			int code;
			IntPtr data1;
			IntPtr data2;
		}

		//Doing nasty union hacks
		[StructLayout(LayoutKind.Explicit)]
		public struct Event
		{
			[FieldOffset(0)]
			public EventType type;
			[FieldOffset(0)]
			WindowEvent window;
			[FieldOffset(0)]
			KeyboardEvent key;
			//[FieldOffset(0)]
			//TextEditingEvent edit;
			[FieldOffset(0)]
			TextInputEvent text;
			[FieldOffset(0)]
			MouseMotionEvent motion;
			[FieldOffset(0)]
			MouseButtonEvent button;
			[FieldOffset(0)]
			MouseWheelEvent wheel;
			[FieldOffset(0)]
			JoyAxisEvent jaxis;
			[FieldOffset(0)]
			JoyBallEvent jball;
			[FieldOffset(0)]
			JoyHatEvent jhat;
			[FieldOffset(0)]
			JoyButtonEvent jbutton;
			[FieldOffset(0)]
			JoyDeviceEvent jdevice;
			[FieldOffset(0)]
			QuitEvent quit;
			[FieldOffset(0)]
			UserEvent user;
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
