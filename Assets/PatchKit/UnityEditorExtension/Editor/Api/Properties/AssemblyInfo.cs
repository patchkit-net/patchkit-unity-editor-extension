using System.Reflection;
using System.Runtime.InteropServices;
using PatchKit.Api.Properties;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("PatchKit.Api")]
[assembly: AssemblyDescription("Library with classes for communicating with PatchKit API.")]
[assembly: AssemblyCompany("Upsoft")]
[assembly: AssemblyProduct("PatchKit.Api")]
[assembly: AssemblyCopyright("Copyright © Upsoft 2018")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("6AB60F43-EBF5-4122-A437-D1C4E1A41608")]

// Based on https://codingforsmarties.wordpress.com/2016/01/21/how-to-version-assemblies-destined-for-nuget/
[assembly: AssemblyVersion(Version.Major + ".0.0.0")]
[assembly: AssemblyFileVersion(Version.Major + "." + Version.Minor + "." + Version.Patch + ".0")]
[assembly: AssemblyInformationalVersion(Version.Major + "." + Version.Minor + "." + Version.Patch + Version.Suffix)]