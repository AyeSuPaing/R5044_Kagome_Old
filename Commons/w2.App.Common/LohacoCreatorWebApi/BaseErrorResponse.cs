using System;
/*
=========================================================================================================
  Module      : 基礎エラーレスポインクラス(BasErrorResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using Newtonsoft.Json;
namespace w2.App.Common.LohacoCreatorWebApi
{
	/// <summary>
	/// 基礎エラーレスポインクラス
	/// </summary>
	[XmlRoot("Error", Namespace="urn:lohaco:api")]
	[Serializable]
	public class BaseErrorResponse : BaseResponse
	{
		#region #BasErrorResponse コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		protected BaseErrorResponse()
		{
		}
		#endregion

		#region +WriteDebugLogWithMaskedPersonalInfo デバッグログ（個人情報がマスクされる状態）の出力
		/// <summary>
		/// デバッグログ（個人情報がマスクされる状態）の出力
		/// </summary>
		/// <returns>デバッグログ（個人情報がマスクされる状態）内容</returns>
		public override string WriteDebugLogWithMaskedPersonalInfo()
		{
			return WebApiHelper.SerializeXml(this);
		}
		#endregion

		#region プロパティ
		/// <summary>エラーコード</summary>
		[XmlElement("Code")]
		public LohacoConstants.ErrorCode Code { get; set; }
		/// <summary>エラーメッセージ</summary>
		[XmlElement("Message")]
		public string Message { get; set; }
		#endregion
	}
}