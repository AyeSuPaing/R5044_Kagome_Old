using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Plugin.Order;
using System.Web;
using w2.Plugin;

namespace w2.TestPluginLib
{
	public class OrderFailedErrorPlugin
		: IOrderFailedPlugin
	{
		#region IOrderFailedPlugin メンバー

		public void OnFailed()
		{
			this.Host.WriteErrorLog("注文失敗時処理のみ実装！失敗！");
			this.Host.Context.Session["atHost"] = "外から貰ったセッションに格納！";
			HttpContext.Current.Session["atInner"] = "中で拾ったセッションに格納！";
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
