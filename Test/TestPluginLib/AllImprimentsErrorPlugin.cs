using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Plugin.User;
using w2.Plugin.Order;
using w2.Plugin;

namespace w2.TestPluginLib
{
	public class AllImprimentsErrorPlugin :
		IUserModifiedPlugin,
		IUserRegisteredPlugin,
		IUserWithdrawedPlugin,
		IOrderValidatingPlugin,
		IOrderFailedPlugin,
		IOrderCompletePlugin
	{

		#region IUserWithdrawedPlugin メンバー

		public void OnWithdrawed()
		{
			this.Host.WriteErrorLog("退会後処理！失敗！");
		}

		#endregion

		#region IUserRegisteredPlugin メンバー

		public void OnRegistered()
		{
			this.Host.WriteErrorLog("登録後処理！失敗！");
		}

		#endregion

		#region IUserModifiedPlugin メンバー

		public void OnModified()
		{
			this.Host.WriteErrorLog("変更後処理！失敗！");
		}

		#endregion

		#region IOrderValidatingPlugin メンバー

		public void OnValidating()
		{
			this.Host.WriteErrorLog("注文検証処理！失敗！");
		}

		public bool IsSuccess { get { return false; } }

		public string Message { get { return "注文検証処理！失敗メッセージ！"; } }

		#endregion

		#region IOrderFailedPlugin メンバー

		public void OnFailed()
		{
			this.Host.WriteErrorLog("注文失敗時処理！失敗！");
		}

		#endregion

		#region IOrderCompletePlugin メンバー

		public void OnCompleted()
		{
			this.Host.WriteErrorLog("注文完了時処理！失敗！");
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
