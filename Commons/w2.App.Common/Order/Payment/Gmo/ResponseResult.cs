/*
=========================================================================================================
  Module      : GMOレスポンス結果クラス(ResponseResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.GMO
{
	/// <summary>
	/// レスポンス結果クラス
	/// </summary>
	public class ResponseResult
	{
		/// <summary>エラー詳細コード</summary>
		public const string PARAM_ERRINFO = "ErrInfo";
		/// <summary>エラーコード</summary>
		public const string PARAM_ERRCODE = "ErrCode";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		public ResponseResult(string responseString)
		{
			this.Parameters = HttpUtility.ParseQueryString(responseString);
			this.ErrorMessages = GetErrorMessages((string)this.Parameters[PARAM_ERRINFO]);
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="errorInfo">エラー文字列</param>
		/// <returns>エラーメッセージ取得</returns>
		public string GetErrorMessages(string errorInfo)
		{
			// エラーの場合
			if (this.IsSuccess == false)
			{
				var result = new List<string>();

				// GMO決済エラーメッセージXMLからエラー内容を取得
				var gmoMessages = XDocument.Parse(Properties.Resources.GmoMessages);

				foreach (string error in errorInfo.Split('|'))
				{
					if (string.IsNullOrEmpty(error)) continue;

					var target = (from message in gmoMessages.Descendants((error.StartsWith("42") ? error.Substring(2) : error)) select message).FirstOrDefault();
					result.Add(string.Format("{0}:{1}", error, (target != null) ? target.Attribute("message").Value : string.Empty));
				}

				return string.Join(",", result.ToArray());
			}

			return string.Empty;
		}

		/// <summary>成功？（true:成功、false:失敗）</summary>
		public bool IsSuccess { get { return string.IsNullOrEmpty(this.Parameters.Get(PARAM_ERRCODE)); } }
		/// <summary>パラメータ</summary>
		public NameValueCollection Parameters { get; private set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessages { get; private set; }
	}
}
