﻿#region License
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
using System.Globalization;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Build.Tasks
{
    /// <summary>
    /// Returns a date stamp in the form yyMMdd.
    /// </summary>
    public class DateStamp : Task
    {
        string date;

        /// <summary>
        /// Gets a <see cref="System.String"/> represting the date stamp.
        /// </summary>
        [Output]
        public string Date
        {
            get { return date; }
            private set { date = value; }
        }

        public override bool Execute()
        {
            try
            {
                // Build number is defined as the number of days since 1/1/2010.
                // Revision number is defined as the fraction of the current day, expressed in seconds.
                double timespan = DateTime.UtcNow.Subtract(new DateTime(2010, 1, 1)).TotalDays;
                string build = ((int)timespan).ToString();
                string revision = ((int)((timespan - (int)timespan) * UInt16.MaxValue)).ToString();
                Date = String.Format("{0}.{1}", build, revision);
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }
            return true;
        }
    }
}
