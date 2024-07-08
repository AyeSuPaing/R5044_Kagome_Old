using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Plugin.User;
using w2.Plugin;

namespace w2.TestPluginLib
{
	public class UserRegisteredErrorPlugin
		: IUserRegisteredPlugin
	{
		#region IPlugin メンバー

		public void Initialize(IPluginHost host)
		{
			this.Host = host;
		}

		public IPluginHost Host { get; set; }

		#endregion


		#region IUserRegisteredPlugin メンバー

		public void OnRegistered()
		{
			this.Host.WriteErrorLog("登録後処理のみ実装！失敗！");
		}

		#endregion
	}
}
