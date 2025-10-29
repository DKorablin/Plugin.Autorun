using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Microsoft.Win32;
using SAL.Flatbed;
using System.Text;

namespace Plugin.Autorun
{
	public class PluginSettings
	{
		private Boolean _autorun;
		private const String RegKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

		private PluginWindows Plugin { get; set; }

		[DefaultValue(false)]
		[Category("Automation")]
		[Description("AutoStart application with operating system")]
		public Boolean Enabled
		{
			get => this._autorun;
			set
			{
				if(this._autorun != value)
				{
					this._autorun = value;
					this.SetAutorunKey();//If there is an exception, the status will change..
				}
			}
		}
		/// <summary>Get the name of the application for which the autoStart function is registered</summary>
		private String ApplicationName
		{
			get
			{
				String assemblyName = Assembly.GetEntryAssembly() == null
					? Process.GetCurrentProcess().ProcessName
					: Assembly.GetEntryAssembly().GetName().Name;

				StringBuilder application = new StringBuilder(assemblyName);

				foreach(IPluginDescription kernel in this.Plugin.Host.Plugins.FindPluginType<IPluginKernel>())
					application.Append("|" + kernel.ID);

				return application.ToString();
			}
		}

		internal PluginSettings(PluginWindows plugin)
			=> this.Plugin = plugin;

		/// <summary>Set the autoStart option in the registry</summary>
		internal void SetAutorunKey()
			=> ThreadPool.QueueUserWorkItem(SetAutorunKeyAsync, this);

		/// <summary>Set autoStart option in the registry</summary>
		/// <remarks>
		/// The check is performed each time the application is launched. This is because the application may be launched for the first time, and the value needs to be written.
		/// In the future, this option should be addressed differently.
		/// </remarks>
		/// <param name="state">Arguments passed to the stream</param>
		private static void SetAutorunKeyAsync(Object state)
		{
			PluginSettings pThis = (PluginSettings)state;

			RegistryKey key = Registry.CurrentUser.OpenSubKey(PluginSettings.RegKey, true);
			String application = pThis.ApplicationName;

			if(pThis.Enabled)
			{
				StringBuilder args = new StringBuilder();
				foreach(String arg in Environment.GetCommandLineArgs())
					args.AppendFormat("\"{0}\" ", arg);
				key.SetValue(application, args.ToString().TrimEnd(), RegistryValueKind.String);
			} else
				key.DeleteValue(application, false);
		}
	}
}