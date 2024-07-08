/*
=========================================================================================================
  Module      : 楽天IDConnectログイン認証レスポンスデータクラス(RakutenIDConnectAuthResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Web;
using w2.Common.Util;

namespace w2.App.Common.Auth.RakutenIDConnect
{
	/// <summary>
	/// 楽天IDConnectログイン認証レスポンスデータクラス
	/// ※楽天IDConnect基底レスポンスデータクラスを継承しない
	/// </summary>
	[Serializable]
	public class RakutenIDConnectAuthResponseData
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="request">リクエスト</param>
		public RakutenIDConnectAuthResponseData(HttpRequest request)
		{
			// レスポンスデータセット
			SetResponseData(request);
		}
		#endregion

		#region メソッド
		/// <summary>
		/// レスポンスデータセット
		/// </summary>
		/// <param name="request">リクエスト</param>
		private void SetResponseData(HttpRequest request)
		{
			this.ResponseData = new Hashtable();
			foreach (var key in request.QueryString.AllKeys)
			{
				this.ResponseData.Add(key, request.QueryString[key]);
			}
		}
		#endregion

		#region プロパティ
		/// <summary>レスポンスデータ</summary>
		private Hashtable ResponseData { get; set; }
		/// <summary>Authorization Code</summary>
		public string Code
		{
			get { return StringUtility.ToEmpty(this.ResponseData["code"]); }
		}
		/// <summary>ステート</summary>
		public string State
		{
			get { return StringUtility.ToEmpty(this.ResponseData["state"]); }
		}
		/// <summary>エラーコード</summary>
		public string Error
		{
			get { return StringUtility.ToEmpty(this.ResponseData["error"]); }
		}
		/// <summary>エラーに関する追加情報</summary>
		public string ErrorDescription
		{
			get { return StringUtility.ToEmpty(this.ResponseData["error_description"]); }
		}
		/// <summary>成功？</summary>
		public bool IsSuccess
		{
			get
			{
				// Authorization Codeが存在する？
				return (string.IsNullOrEmpty(this.Code) == false);
			}
		}
		#endregion
	}
}