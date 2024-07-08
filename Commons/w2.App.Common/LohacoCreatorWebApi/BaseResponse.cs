/*
=========================================================================================================
  Module      : OrderInfoのレスポインクラス(BaseResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml.Serialization;
using w2.App.Common.LohacoCreatorWebApi.OrderChange;
using w2.App.Common.LohacoCreatorWebApi.OrderInfo;
using w2.App.Common.LohacoCreatorWebApi.OrderList;
using w2.App.Common.LohacoCreatorWebApi.StockEdit;

namespace w2.App.Common.LohacoCreatorWebApi
{
	/// <summary>
	/// レスポンス用データの基底クラス
	/// </summary>
	[XmlRoot("ResultSet")]
	[XmlInclude(typeof(OrderChangeResponse))]
	[XmlInclude(typeof(OrderInfoResponse))]
	[XmlInclude(typeof(OrderListResponse))]
	[XmlInclude(typeof(StockEditResponse))]
	[Serializable]
	public abstract class BaseResponse
	{
		#region #BaseResponse コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		protected BaseResponse()
		{
		}
		#endregion
	
		#region +WriteDebugLog デバッグログの出力
		/// <summary>
		/// デバッグログの出力
		/// </summary>
		/// <param name="isMaskPersonalInfoEnabled">個人情報をマスクかどうか</param>
		/// <returns>デバッグログ内容</returns>
		public virtual string WriteDebugLog(bool isMaskPersonalInfoEnabled = true)
		{
			return (isMaskPersonalInfoEnabled) ? WriteDebugLogWithMaskedPersonalInfo() : WebApiHelper.SerializeXml(this);
		}
		#endregion

		#region +WriteDebugLogWithMaskedPersonalInfo デバッグログ（個人情報がマスクされる状態）の出力
		/// <summary>
		/// デバッグログ（個人情報がマスクされる状態）の出力
		/// </summary>
		/// <returns>デバッグログ（個人情報がマスクされる状態）内容</returns>
		public abstract string WriteDebugLogWithMaskedPersonalInfo();
		#endregion

		#region BaseResult 内部クラス
		/// <summary>
		/// 基本結果の基底クラス
		/// </summary>
		[Serializable]
		public class BaseResult
		{
			/// <summary>取得成否（OK/NG）</summary>
			[XmlElement("Status")]
			public LohacoConstants.Status Status { get; set; }
			/// <summary>警告情報（ある場合）</summary>
			[XmlElement("Warning")]
			public Warning Warning { get; set; }
			/// <summary>エラー情報</summary>
			[XmlElement("Error")]
			public Error Error { get; set; }
		}
		#endregion

		#region Warning 内部クラス
		/// <summary>
		/// 警告結果の基底クラス
		/// </summary>
		[Serializable]
		public class Warning
		{
			/// <summary>警告コード</summary>
			[XmlElement("Code")]
			public LohacoConstants.ErrorCode Code { get; set; }
			/// <summary>警告メッセージ</summary>
			[XmlElement("Message")]
			public string Message { get; set; }
			/// <summary>警告詳細（ある場合。現状なし）</summary>
			[XmlElement("Detail", IsNullable = false)]
			public string Detail { get; set; }
		}
		#endregion

		#region Error 内部クラス
		/// <summary>
		/// エラー結果の基底クラス
		/// </summary>
		[Serializable]
		public class Error
		{
			/// <summary>エラーコード</summary>
			[XmlElement("Code")]
			public LohacoConstants.ErrorCode Code { get; set; }
			/// <summary>エラーメッセージ</summary>
			[XmlElement("Message")]
			public string Message { get; set; }
			/// <summary>エラー詳細（ある場合)</summary>
			[XmlElement("Detail")]
			public ErrorDetail Detail { get; set; }
		}
		#endregion

		#region ErrorDetail 内部クラス
		/// <summary>
		/// エラー詳細の基底クラス
		/// </summary>
		[Serializable]
		public class ErrorDetail
		{
			/// <summary>カード決済APIのエラーコード(エラーコードが「od00006」の場合)</summary>
			[XmlElement("SettleCode", IsNullable = false)]
			public string SettleCode { get; set; }
		}
		#endregion
	}
}
