using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using w2.Plugin;
using w2.Plugin.User;
using w2.Plugin.Order;

namespace w2.TestPluginLib
{
	public class AllImprimentsPlugin :
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
			this.Host.WriteInfoLog("退会後処理！成功！");
		}

		#endregion

		#region IUserRegisteredPlugin メンバー

		public void OnRegistered()
		{
			this.Host.WriteInfoLog("登録後処理！成功！");
		}

		#endregion

		#region IUserModifiedPlugin メンバー

		public void OnModified()
		{
			this.Host.WriteInfoLog("変更後処理！成功！");
		}

		#endregion

		#region IPlugin メンバー

		public void Initialize(IPluginHost host)
		{
			this.Host = host;
		}

		public IPluginHost Host { get; set; }

		#endregion


		#region IOrderValidatingPlugin メンバー

		public void OnValidating()
		{
			this.Host.WriteInfoLog("注文検証処理！成功！");
		}

		public bool IsSuccess { get { return true; } }

		public string Message　{　get { return "注文検証処理！メッセージ！"; } }

		#endregion

		#region IOrderFailedPlugin メンバー

		public void OnFailed()
		{
			this.Host.WriteInfoLog("注文失敗時処理！成功！");
		}

		#endregion

		#region IOrderCompletePlugin メンバー

		public void OnCompleted()
		{
			this.Host.WriteInfoLog("注文完了時処理！成功！");
		}

		#endregion
	}
}
