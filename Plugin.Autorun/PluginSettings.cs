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
		[Description("Autostart application with operating system")]
		public Boolean Enabled
		{
			get => this._autorun;
			set
			{
				if(this._autorun != value)
				{
					this._autorun = value;
					this.SetAutorunKey();//Если будет исключение, то статус поменяется.
				}
			}
		}
		/// <summary>Получить наименование приложения для которого прописывается функция автозапуска</summary>
		private String ApplicationName
		{
			get
			{
				String application = Assembly.GetEntryAssembly() == null
					? Process.GetCurrentProcess().ProcessName
					: Assembly.GetEntryAssembly().GetName().Name;

				foreach(IPluginDescription kernel in this.Plugin.Host.Plugins.FindPluginType<IPluginKernel>())
					application += "|" + kernel.ID;

				return application;
			}
		}

		internal PluginSettings(PluginWindows plugin)
			=> this.Plugin = plugin;

		/// <summary>Установить в реестре возможность автозапуска</summary>
		internal void SetAutorunKey()
			=> ThreadPool.QueueUserWorkItem(SetAutorunKeyAsync, this);

		/// <summary>Установить в реестре возможность автозапуска</summary>
		/// <remarks>
		/// Проверка осуществляется при каждом запуске приложения. Т.к. приложение может быть запущено в первый раз, а значение необходимо записать.
		/// В будущем, надо решить по другому этот вариант
		/// </remarks>
		/// <param name="state">Аргкменты передаваемые в поток</param>
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