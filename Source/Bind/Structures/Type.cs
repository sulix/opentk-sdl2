﻿#region --- License ---
/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Bind.Structures
{
    public class Type
    {
        internal static Dictionary<string, string> GLTypes;
        internal static Dictionary<string, string> CSTypes;

        private static bool typesLoaded;

        #region internal static void Initialize(string glTypes, string csTypes)
        
        internal static void Initialize(string glTypes, string csTypes)
        {
            if (!typesLoaded)
            {
                if (GLTypes == null)
                {
                    using (StreamReader sr = Utilities.OpenSpecFile(Settings.InputPath, glTypes))
                    {
                        GLTypes = Bind.MainClass.Generator.ReadTypeMap(sr);
                    }
                }
                if (CSTypes == null)
                {
                    using (StreamReader sr = Utilities.OpenSpecFile(Settings.InputPath, csTypes))
                    {
                        CSTypes = Bind.MainClass.Generator.ReadCSTypeMap(sr);
                    }
                }
                typesLoaded = true;
            }
        }
        
        #endregion

        #region --- Constructors ---
        
        public Type()
        {
        }

        public Type(Type t)
        {
            if (t != null)
            {
                this.CurrentType = t.CurrentType;
                this.PreviousType = t.PreviousType;
                this.WrapperType = t.WrapperType;
                this.Array = t.Array;
                this.Pointer = t.Pointer;
                this.Reference = t.Reference;
            }
        }
        
        #endregion

        #region public string Type

        string type;
        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        public virtual string CurrentType
        {
            //get { return _type; }
            get
            {
                //if (Pointer && Settings.Compatibility == Settings.Legacy.Tao)
                //    return "IntPtr";

                return type;
            }
            set
            {
                if (!String.IsNullOrEmpty(type))
                    PreviousType = type;
                if (!String.IsNullOrEmpty(value))
                    type = value.Trim();

                //Translate();

                if (type.EndsWith("*"))
                {
                    type = type.TrimEnd('*');
                    Pointer = true;
                }
            }
        }

        #endregion

        #region public string PreviousType

        private string _previous_type;

        public string PreviousType
        {
            get { return _previous_type; }
            set { _previous_type = value; }
        }


        #endregion

        #region public bool Reference

        bool reference;

        public bool Reference
        {
            get { return reference; }
            set { reference = value; }
        }

        #endregion

        #region public bool Array

        int array;

        public int Array
        {
            get { return array; }
            set { array = value > 0 ? value : 0; }
        }

        #endregion

        #region public bool Pointer

        bool pointer;

        public bool Pointer
        {
            get { return pointer; }
            set { pointer = value; }
        }

        #endregion

        #region public bool CLSCompliant

        public bool CLSCompliant
        {
            get
            {
                return !(
                    (Pointer && (Settings.Compatibility != Settings.Legacy.Tao)) ||
                    CurrentType.Contains("UInt") ||
                    CurrentType.Contains("SByte"));


                /*(Type.Contains("GLu") && !Type.Contains("GLubyte")) ||
                Type == "GLbitfield" ||
                Type.Contains("GLhandle") ||
                Type.Contains("GLhalf") ||
                Type == "GLbyte");*/
            }
        }

        #endregion

        #region WrapperType property

        private WrapperTypes _wrapper_type = WrapperTypes.None;

        public WrapperTypes WrapperType
        {
            get { return _wrapper_type; }
            set { _wrapper_type = value; }
        }

        #endregion

        #region public string GetFullType()

        public string GetFullType(Dictionary<string, string> CSTypes, bool compliant)
        {
            if (Pointer && Settings.Compatibility == Settings.Legacy.Tao)
                return "IntPtr";

            if (!compliant)
            {
                return
                    CurrentType +
                    (Pointer ? "*" : "") +
                    (Array > 0 ? "[]" : "");
            }

            return
                GetCLSCompliantType() +
                (Pointer ? "*" : "") +
                (Array > 0 ? "[]" : "");

        }

        #endregion

        #region public string GetCLSCompliantType()

        public string GetCLSCompliantType()
        {
            if (!CLSCompliant)
            {
                if (Pointer && Settings.Compatibility == Settings.Legacy.Tao)
                    return "IntPtr";

                switch (CurrentType)
                {
                    case "UInt16":
                    case "ushort":
                        return "Int16";
                    case "UInt32":
                    case "uint":
                        return "Int32";
                    case "UInt64":
                    case "ulong":
                        return "Int64";
                    case "SByte":
                    case "sbyte":
                        return "Byte";
                }
            }

            return CurrentType;
        }

        #endregion

        #region public override string ToString()
        
        public override string ToString()
        {
            return CurrentType;
        }
        
        #endregion
        
        #region internal static Type Translate(Type type)

        internal static Type Translate(Type type)
        {
            Type t = new Type(type);

            if (GLTypes.ContainsKey(t.CurrentType))
                t.CurrentType = GLTypes[t.CurrentType];

            if (CSTypes.ContainsKey(t.CurrentType))
                t.CurrentType = CSTypes[t.CurrentType];

            return t;
        }
        
        #endregion
    }
}
