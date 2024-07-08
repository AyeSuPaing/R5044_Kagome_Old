/*
=========================================================================================================
  Module      : YAHOO API 注文詳細API リクエストDTO クラス(YahooApiOrderInfoRequestDto.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;
using w2.Common.Helper;

namespace w2.App.Common.Mall.Yahoo.Dto
{
	/// <summary>
	/// YAHOO API 注文詳細API リクエストDTO クラス
	/// </summary>
	public class YahooApiOrderInfoRequestDto
	{
		/// <summary>
		/// YAHOO API 注文詳細API リクエストDTO クラス
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="fields">取得フィールド群</param>
		/// <param name="sellerId">セラーID</param>
		public YahooApiOrderInfoRequestDto(string orderId, string[] fields, string sellerId)
		{
			this.Req = new Req
			{
				Target = new Target($"{sellerId}-{orderId}", fields),
				SellerId = sellerId,
			};
		}

		/// <summary>
		/// シリアライズ
		/// </summary>
		/// <returns>シリアライズした文字列</returns>
		public string Serialize()
		{
			var result = SerializeHelper.Serialize(this.Req);
			return result;
		}

		/// <summary>リクエスト値</summary>
		public Req Req { get; }
	}

	/// <summary>
	/// リクエスト値
	/// </summary>
	[XmlRoot("Req")]
	public class Req
	{
		/// <summary>レスポンスに含めるfiled値</summary>
		[XmlElement("Target")]
		public Target Target { get; set; }
		/// <summary>セラーID (ストアアカウントを指定)</summary>
		[XmlElement("SellerId")]
		public string SellerId { get; set; }
	}

	/// <summary>
	/// レスポンスに含めるfiled値
	/// </summary>
	public class Target
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Target() { }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="fields">取得フィールド群</param>
		public Target(string orderId, string[] fields)
		{
			this.OrderId = orderId;
			this.Field = string.Join(",", fields);
		}
		
		/// <summary>注文ID</summary>
		[XmlElement("OrderId")]
		public string OrderId { get; set; }
		/// <summary>取得フィールド群</summary>
		[XmlElement("Field")]
		public string Field { get; set; }
	}
}
