/*
=========================================================================================================
  Module      : 楽天IDConnect基底レスポンスデータクラス(RakutenIDConnectBaseResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.Common.Helper;
using w2.Common.Util;

namespace w2.App.Common.Auth.RakutenIDConnect
{
	/// <summary>
	/// 楽天IDConnect基底レスポンスデータクラス
	/// </summary>
	[Serializable]
	public class RakutenIDConnectBaseResponseData
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseString">レスポンス文字列</param>
		internal RakutenIDConnectBaseResponseData(string responseString)
		{
			this.ResponseString = responseString;
			this.ResponseData = SerializeHelper.DeserializeJson<Dictionary<string, object>>(responseString);
		}
		#endregion

		#region メソッド
		/// <summary>
		/// レスポンスデータ取得
		/// </summary>
		/// <param name="key">キー</param>
		protected object GetResponseDataValue(string key)
		{
			if (this.ResponseData.ContainsKey(key) == false) return null;
			return this.ResponseData[key];
		}
		#endregion

		#region プロパティ
		/// <summary>レスポンス文字列</summary>
		public string ResponseString { get; set; }
		/// <summary>レスポンスデータ</summary>
		protected Dictionary<string, object> ResponseData { get; set; }
		/// <summary>エラーコード</summary>
		public string Error
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("error")); }
		}
		/// <summary>エラーに関する追加情報</summary>
		public string ErrorDescription
		{
			get { return StringUtility.ToEmpty(GetResponseDataValue("error_description")); }
		}
		/// <summary>成功？</summary>
		public bool IsSuccess
		{
			get { return string.IsNullOrEmpty(this.Error); }
		}
		#endregion
	}
}