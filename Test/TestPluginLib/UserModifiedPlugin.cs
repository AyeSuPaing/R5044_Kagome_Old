using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Plugin.User;
using w2.Plugin;

namespace w2.TestPluginLib
{
	public class UserModifiedPlugin
		: IUserModifiedPlugin
	{
		#region IUserModifiedPlugin メンバー

		public void OnModified()
		{
			this.Host.WriteInfoLog("変更後処理のみ実装！成功！");
		}

		#endregion

		#region IPlugin メンバー

		public void Initialize(IPluginHost host)
		{
			this.Host = host;
		}

		public IPluginHost Host { get; set; }

		#endregion

	}
}
