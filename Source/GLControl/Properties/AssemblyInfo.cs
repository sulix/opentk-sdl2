﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("OpenTK.GLControl")]
[assembly: AssemblyDescription("Provides integration with System.Windows.Forms.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("The Open Toolkit Library")]
[assembly: AssemblyProduct("The Open Toolkit Library")]
[assembly: AssemblyCopyright("Copyright ©  2006-2010 the Open Toolkit Library")]
[assembly: AssemblyTrademark("OpenTK")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("5414b90b-d7be-4382-b0e1-f07ce154f7f7")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.201")]
[assembly: AssemblyFileVersion("1.0.0.201")]

[assembly: System.CLSCompliant(true)]
[assembly: System.Security.AllowPartiallyTrustedCallers]
#if NET40
[assembly: System.Security.SecurityRules(System.Security.SecurityRuleSet.Level1)]
#endif

#if SIGN_ASSEMBLY
[assembly: AssemblyKeyFile(@"../../../OpenTK.snk")]
#endif