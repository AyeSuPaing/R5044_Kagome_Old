using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using w2.Plugin.P0011_Intercom.WebService.Inproc;
using w2.Plugin.P0011_Intercom.WebService.Util;

namespace w2.Plugin.P0011_Intercom.WebService
{
	/// <summary>
	/// w2Service の概要の説明です
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// この Web サービスを、スクリプトから ASP.NET AJAX を使用して呼び出せるようにするには、次の行のコメントを解除します。 
	// [System.Web.Script.Services.ScriptService]
	public class w2Service : System.Web.Services.WebService
	{

		[WebMethod]
		public string TestMethod(string value)
		{
			return "動作確認 送信地は：" + value;
		}
		
		#region +UserSyncExecute 会員情報連携

		/// <summary>
		/// 会員情報連携
		/// </summary>
		/// <param name="ds"></param>
		/// <returns></returns>
		[WebMethod]
		public DataSet UserSyncExecute(DataSet ds)
		{
			IWebServiceInProc inproc = new UserSyncExecuteProc();
			return inproc.Execute(ds);
		}

		#endregion

		#region +CreateOnetimePassword ワンタイムパスワード発行
		/// <summary>
		/// ワンタイムパスワード発行
		/// </summary>
		/// <param name="ds"></param>
		/// <returns></returns>
		[WebMethod]
		public DataSet CreateOnetimePassword(DataSet ds)
		{
			IWebServiceInProc inproc = new OnetimePasswordProc();
			return inproc.Execute(ds);
		}

		#endregion
	}
}
