/*
=========================================================================================================
  Module      : 在庫管理API StockEditのレスポンスクラス(StockEditResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace w2.App.Common.LohacoCreatorWebApi.StockEdit
{
	/// <summary>
	/// 在庫管理API StockEditのレスポンスクラス
	/// </summary>
	[XmlRoot("ResultSet")]
	[Serializable]
	public class StockEditResponse : BaseResponse
	{
		#region +StockEditResponse コンストラクタ
		/// <summary>
		/// 在庫管理API StockEditResponseのデフォルトコンストラクタ
		/// </summary>
		public StockEditResponse()
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
			// 個人情報を含めないので、XML文字列のままに返却
			return WebApiHelper.SerializeXml(this);
		}
		#endregion
		
		#region プロパティ
		/// <summary>レスポンス一覧</summary>
		[XmlElement("ListResult")]
		public StockEditListResult ListResult { get; set; }
		#endregion

		#region StockEditListResult 内部クラス
		/// <summary>
		/// 在庫管理API 結果一覧クラス
		/// </summary>
		[Serializable]
		public class StockEditListResult
		{
			#region プロパティ
			/// <summary>レスポンス</summary>
			[XmlElement("Result")]
			public List<StockEditResult> Results { get; set; }
			#endregion
		}
		#endregion

		#region StockEditResult 内部クラス
		/// <summary>
		/// 在庫管理API 結果クラス
		/// </summary>
		[Serializable]
		public class StockEditResult
		{
			#region プロパティ
			/// <summary>ステータス</summary>
			[XmlElement("Status")]
			public LohacoConstants.StockStatus Status { get; set; }
			/// <summary>内容。 エラーの詳細な内容などを記載。</summary>
			[XmlElement("Content")]
			public string Content { get; set; }
			/// <summary>セラーID（エラー対象）</summary>
			[XmlElement("SellerId")]
			public string SellerId { get; set; }
			/// <summary>商品グループコード（エラー対象）</summary>
			[XmlElement("GroupCd")]
			public string GroupCd { get; set; }
			/// <summary>商品コード（エラー対象）</summary>
			[XmlElement("ItemCd")]
			public string ItemCd { get; set; }
			/// <summary>カタログ商品コード（エラー対象）</summary>
			[XmlElement("CatalogItemCd")]
			public string CatalogItemCd { get; set; }
			/// <summary>ファイル名（エラー対象）</summary>
			[XmlElement("FileName")]
			public string FileName { get; set; }
			/// <summary>エラーコード</summary>
			[XmlElement("ErrorCode")]
			public LohacoConstants.ErrorCode ErrorCode { get; set; }
			/// <summary>エラーメッセージ</summary>
			[XmlElement("ErrorMessage")]
			public string ErrorMessage { get; set; }
			/// <summary>データチェック日時(yyyyMMddHHmmssフォーマット)</summary>
			[XmlElement("CheckDate")]
			public string CheckDate { get; set; }
			#endregion
		}
		#endregion
	}
}
