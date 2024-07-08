/*
=========================================================================================================
  Module      : CrossMall Api リクエスト基底クラス (CrossMallRequestBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using w2.Common.Logger;
using w2.Common.Web;

namespace w2.App.Common.CrossMall
{
	/// <summary>
	/// リクエスト基底クラス
	/// </summary>
	public abstract class CrossMallRequestBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CrossMallRequestBase()
		{
			this.Params = new Dictionary<string, string>
			{
				{ CrossMallConstants.CONST_PARAM_KEY_NAME_ACCOUNT, Account}
			};
		}

		/// <summary>
		/// リクエストの署名を取得
		/// </summary>
		/// <param name="encode">エンコード</param>
		/// <returns>署名文字列</returns>
		private string GetSign(Encoding encode = null)
		{
			// パラメータをエンコーディングする
			var paramStr = new UrlCreator(string.Empty).AddRangeParam(this.Params).CreateUrl(encode);
			paramStr = paramStr.Replace("?", string.Empty);
			using (var md5 = MD5.Create())
			{
				var bytes = md5.ComputeHash(
					(encode ?? Encoding.UTF8).GetBytes(
						string.Concat(
							paramStr,
							Constants.CROSS_MALL_AUTHENTICATION_KEY)));

				var sign = string.Concat(bytes.Select(b => string.Format("{0:x2}", b)));
				return sign;
			}
		}

		/// <summary>
		/// Get方式の請求を送信する
		/// </summary>
		/// <param name="requstUrl">リクエストURL</param>
		/// <returns>レスポンスXML文字列</returns>
		/// <remarks>例外システムエラーの場合は空いている文字列を戻す</remarks>
		public string CreateGetRequst(string requstUrl)
		{
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requstUrl);
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				using (Stream stream = response.GetResponseStream())
				using (StreamReader reader = new StreamReader(stream))
				{
					var responseString = reader.ReadToEnd();
					if (Constants.CROSS_MALL_INTEGRATION_ENABLE_LOGGING) CrossMallApiLogger.WriteDetailInfo(requstUrl, responseString);
					return responseString;
				}
				
			}
			catch(Exception ex)
			{
				FileLogger.WriteError(ex);
				return string.Empty;
			}
		}

		/// <summary>
		/// リクエストURL取得する
		/// </summary>
		/// <returns>リクエストURL</returns>
		public string GetRequstUrl()
		{
			this.Params.Add(CrossMallConstants.CONST_PARAM_KEY_NAME_SIGN, GetSign());
			var url = new UrlCreator(RequestRawUrl).AddRangeParam(this.Params).CreateUrl();
			return url;
		}

		#region プロパティ
		/// <summary> パラメータ </summary>
		public Dictionary<string, string> Params { get; set; }
		/// <summary> 署名 </summary>
		public string Sign { get; set; }
		/// <summary> 会社コード </summary>
		public string Account { get { return Constants.CROSS_MALL_ACCOUNT; } }
		/// <summary> リクエストURL(パラメータなし) </summary>
		public string RequestRawUrl { get; set; }
		#endregion
	}
}
