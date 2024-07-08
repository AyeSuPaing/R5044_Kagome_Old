/*
=========================================================================================================
  Module      : 注文詳細API OrderInfoのリクエストクラス(OrderInfoRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace w2.App.Common.LohacoCreatorWebApi.OrderInfo
{
	/// <summary>
	/// 注文詳細API OrderInfoのリクエストクラス
	/// </summary>
	[Serializable]
	public class OrderInfoRequest : BaseRequest
	{
		#region +OrderInfoRequest コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderInfoRequest()
			: base()
		{
			this.Uri = "https://public.lohaco.jp/lohacoApi/v1/orderInfo";
			this.Accept = LohacoConstants.ResponseContentType.Xml;
			this.ContentType = LohacoConstants.RequestContentType.Xml;
		}
		/// <summary>
		/// パラメータ付きコンストラクタ
		/// </summary>
		/// <param name="providerId">スタアアカウント</param>
		public OrderInfoRequest(string providerId)
			: this()
		{
			this.SellerId = providerId;
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
		/// <summary>取得対象注文</summary>
		[XmlElement("Target")]
		public OrderInfoTarget Target { get; set; }
		/// <summary>ストアアカウント</summary>
		[XmlElement("SellerId")]
		public string SellerId { get; set; }
		#endregion

		#region OrderInfoTarget 内部クラス
		/// <summary>
		/// 注文詳細API 対象注文情報クラス
		/// </summary>
		[Serializable]
		public class OrderInfoTarget
		{
			#region プロパティ
			/// <summary>注文ID</summary>
			[XmlElement("OrderId")]
			public string OrderId { get; set; }
			/// <summary>レスポンスに含めるFieldをカンマ区切りで指定する</summary>
			[XmlElement("Field")]
			public string Field { get { return string.Join(",", this.Fields.Select(p => Enum.GetName(typeof(LohacoConstants.OrderField), p))); } set { } }
			/// <summary>注文Field一覧</summary>
			[XmlIgnore]
			[JsonIgnore]
			public List<LohacoConstants.OrderField> Fields { get; set; }
			#endregion
		}
		#endregion
	}
}
