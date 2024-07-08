/*
=========================================================================================================
  Module      : マクロキオスク用SMS連携アダプタ(AdapterMicroKiosk.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Text;
using w2.Common.Web;

namespace w2.Common.Net.SMS.MicroKiosk
{
	/// <summary>
	/// マクロキオスク用SMS連携アダプタ
	/// </summary>
	public class AdapterMicroKiosk : ISMSAdapter
	{
		/// <summary>
		/// パラメタ
		/// </summary>
		private MacroKioskParams m_para = null;

		/// <summary>
		/// パラメタセット
		/// </summary>
		/// <param name="message">送信するメッセージ</param>
		/// <param name="to">送信先の電話番号（国コード付き、ハイフンなしであること）</param>
		/// <param name="from">送信もと</param>
		/// <returns>アダプタ</returns>
		public ISMSAdapter SetParams(string message, string to, string from)
		{
			m_para = new MacroKioskParams();
			m_para
				.SetUser(GetUser())
				.SetPass(GetPass())
				.SetServId(GetServId())
				.SetTo(to)
				.SetFrom(from)
				.SetText(string.Join("", message.Take(70)));
			return this;
		}

		/// <summary>
		/// 送信
		/// </summary>
		/// <returns>送信結果</returns>
		public ISendResult Send()
		{
			var connector = new HttpApiConnector();

			var url = GetUrl();
			var res = connector.Do(url, Encoding.ASCII, m_para, "", "");
			var rtn = new MacroKioskResult(res);
			return rtn;
		}

		/// <summary>
		/// API用ユーザー取得
		/// </summary>
		/// <returns>API用ユーザー</returns>
		private string GetUser()
		{
			return Constants.MACROKIOSK_API_USER;
		}

		/// <summary>
		/// API用パスワード取得
		/// </summary>
		/// <returns>API用パスワード</returns>
		private string GetPass()
		{
			return Constants.MACROKIOSK_API_PASS;
		}

		/// <summary>
		/// API用サービスID取得
		/// </summary>
		/// <returns>API用ServId</returns>
		private string GetServId()
		{
			return Constants.MACROKIOSK_API_SERVID;
		}

		/// <summary>
		/// API用URL取得
		/// </summary>
		/// <returns>API用URL</returns>
		private string GetUrl()
		{
			return Constants.MACROKIOSK_API_URL;
		}

	}
}
