using System.Reflection;
using System.Runtime.InteropServices;

[assembly: Guid("1c4a8e73-b448-4e35-ad86-9ab8abc342c1")]
[assembly: System.CLSCompliant(true)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://dkorablin.ru/project/Default.aspx?File=85")]
#else

[assembly: AssemblyDescription("Autostart application on system startup")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2011-2012")]
#endif