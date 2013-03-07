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
#if !MINIMAL
using System.Drawing;
#endif
using System.Runtime.InteropServices;

namespace OpenTK.Platform.SDL2
{
    sealed class SDL2DisplayDevice : DisplayDeviceBase
    {
        // Store a mapping between resolutions and their respective
        // size_index (needed for XRRSetScreenConfig). The size_index
        // is simply the sequence number of the resolution as returned by
        // XRRSizes. This is done per available screen.
        readonly List<Dictionary<DisplayResolution, int>> screenResolutionToIndex =
            new List<Dictionary<DisplayResolution, int>>();
        // Store a mapping between DisplayDevices and their default resolutions.
        readonly Dictionary<DisplayDevice, int> deviceToDefaultResolution = new Dictionary<DisplayDevice, int>();
        
        
        
        #region Constructors

        public SDL2DisplayDevice()
        {
            RefreshDisplayDevices();
        }

        #endregion

        #region Private Methods

        void RefreshDisplayDevices ()
		{
			List<DisplayDevice> devices = new List<DisplayDevice> ();

			System.Console.WriteLine("Refreshing display devices.");

			int numVideoDevices = 0;
			lock (API.sdl_api_lock) {
				numVideoDevices = API.GetNumVideoDisplays ();
				System.Console.WriteLine(String.Format("Got {0} displays!",numVideoDevices));
			}

			for (int i = 0; i < numVideoDevices; ++i)
			{
				DisplayDevice dev = new DisplayDevice();
				List<DisplayResolution> resolutions = new List<DisplayResolution>();
				if (i == 0) dev.IsPrimary = true;
				int numResolutions = 0;
				lock (API.sdl_api_lock) {
					numResolutions = API.GetNumDisplayModes(i);
				}
				for (int res = 0; res < numResolutions; ++res)
				{
					lock (API.sdl_api_lock)
					{
						API.DisplayMode modeRect;
						API.GetDisplayMode(i, res, out modeRect);
						resolutions.Add (new DisplayResolution(0, 0, modeRect.w, modeRect.h, 32, modeRect.refresh_rate));
					}
				}
				dev.AvailableResolutions = resolutions;
				devices.Add(dev);
			}
             
            AvailableDevices.Clear();
            AvailableDevices.AddRange(devices);
            Primary = FindDefaultDevice(devices);
            
        }

        static DisplayDevice FindDefaultDevice(IEnumerable<DisplayDevice> devices)
        {
                foreach (DisplayDevice dev in devices)
                    if (dev.IsPrimary)
                        return dev;

            throw new InvalidOperationException("SDL2 Did not return any VideoDisplays");
        }


        #region static int[] FindAvailableDepths(int screen)

        static int[] FindAvailableDepths(int screen)
        {
			int[] x = {32};
			return x;
        }

        #endregion

        #region static float FindCurrentRefreshRate(int screen)

        static float FindCurrentRefreshRate (int screen)
		{
			API.DisplayMode mode;
			lock (API.sdl_api_lock) {
				API.GetCurrentDisplayMode (screen, out mode);
			}
			return mode.refresh_rate;
        }

        #endregion

        #region private static int FindCurrentDepth(int screen)

        static int FindCurrentDepth(int screen)
        {
			return 32;
        }

        #endregion


        #endregion

        #region IDisplayDeviceDriver Members

		public sealed override bool TryChangeResolution(DisplayDevice device, DisplayResolution resolution)
        {
            // In SDL2, the resolution is automagically handled by the window.
			return true;
        }

        public sealed override bool TryRestoreResolution(DisplayDevice device)
        {
			// SDL2 will decide when to restore the resolution, et al.
			return true;
        }


        #endregion

    }
}
