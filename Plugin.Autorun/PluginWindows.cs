using System;
using SAL.Flatbed;

namespace Plugin.Autorun
{
	public class PluginWindows : IPlugin, IPluginSettings<PluginSettings>
	{
		private PluginSettings _settings;

		internal IHost Host { get; }

		/// <summary>Settings for interaction from the host</summary>
		Object IPluginSettings.Settings => this.Settings;

		/// <summary>Settings for interaction from the plugin</summary>
		public PluginSettings Settings
		{
			get
			{
				if(this._settings == null)
				{
					this._settings = new PluginSettings(this);
					this.Host.Plugins.Settings(this).LoadAssemblyParameters(this._settings);
				}
				return this._settings;
			}
		}

		public PluginWindows(IHost host)
			=> this.Host = host ?? throw new ArgumentNullException(nameof(host));

		Boolean IPlugin.OnConnection(ConnectMode mode)
		{
			this.Settings.SetAutorunKey();

			return true;
		}
		
		Boolean IPlugin.OnDisconnection(DisconnectMode mode)
			=> true;
	}
}