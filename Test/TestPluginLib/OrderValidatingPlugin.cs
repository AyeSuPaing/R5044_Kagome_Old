using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Plugin.Order;
using w2.Plugin;

namespace w2.TestPluginLib
{
	public class OrderValidatingPlugin
		: IOrderValidatingPlugin
	{
		#region IOrderValidatingPlugin メンバー

		public void OnValidating()
		{
			this.IsSuccess = true;
			this.Message = "メッセージ呼び出し";
			this.Host.WriteInfoLog("注文検証処理のみ実装！成功！");
		}

		public bool IsSuccess { get; set; }

		public string Message { get; set; }

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
