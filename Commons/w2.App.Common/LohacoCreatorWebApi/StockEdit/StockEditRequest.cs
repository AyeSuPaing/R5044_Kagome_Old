/*
=========================================================================================================
  Module      : 在庫管理API StockEditのリクエストクラス(StockEditRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace w2.App.Common.LohacoCreatorWebApi.StockEdit
{
	/// <summary>
	/// 在庫管理API StockEditのリクエストクラス
	/// </summary>
	[Serializable]
	public class StockEditRequest : BaseRequest
	{
		#region +StockEditRequest コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public StockEditRequest()
			: base()
		{
			this.Uri = "https://public.lohaco.jp/lohacoApi/v1/stockEdit";
			this.Accept = LohacoConstants.ResponseContentType.Xml;
			this.ContentType = LohacoConstants.RequestContentType.Xml;
		}
		/// <summary>
		/// パラメータ付きコンストラクタ
		/// </summary>
		/// <param name="providerId">スタアアカウント</param>
		public StockEditRequest(string providerId)
			: this()
		{
			this.ProviderId = providerId;
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
		/// <summary>更新対象</summary>
		[XmlElement("Target")]
		[JsonConverter(typeof(WebApiHelper.SingleOrArrayConverter<StockEditTarget>))]
		public List<StockEditTarget> Target { get; set; }	// 一つの場合配列ではなく、オブジェクトとして出力
		#endregion

		#region StockEditTarget 内部クラス
		/// <summary>
		/// 在庫管理API 更新対象を指定クラス
		/// </summary>
		[Serializable]
		public class StockEditTarget
		{
			#region プロパティ
			/// <summary>ストアアカウント</summary>
			[XmlElement("SellerId")]
			public string SellerId { get; set; }
			/// <summary>カタログ商品コード</summary>
			[XmlElement("CatalogItemCd", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string CatalogItemCd { get; set; }
			/// <summary>商品コード</summary>
			[XmlElement("ItemCd", IsNullable = false)]
			[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
			public string ItemCd { get; set; }
			/// <summary>在庫数</summary>
			[XmlElement("StockQuantity")]
			public string StockQuantity { get; set; }
			#endregion
		}
		#endregion
	}
}
