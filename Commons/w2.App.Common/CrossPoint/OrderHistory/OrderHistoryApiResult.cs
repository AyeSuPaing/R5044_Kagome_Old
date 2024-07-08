/*
=========================================================================================================
  Module      : Order History Api Result (OrderHistoryApiResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint.OrderHistory
{
	/// <summary>
	/// 伝票取得結果モデル
	/// </summary>
	public class OrderHistoryApiResult : ApiResultBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderHistoryApiResult()
		{
			this.No = 0;
			this.OrderNo = string.Empty;
			this.CardNo = string.Empty;
			this.ShopCode = string.Empty;
			this.ShopName = string.Empty;
			this.OrderId = string.Empty;
			this.OrderDate = null;
			this.PosNo = string.Empty;
			this.DeleteFlg = string.Empty;
			this.PriceTotalInTax = 0m;
			this.PriceTotalNoTax = 0m;
			this.ItemCount = string.Empty;
			this.GrantPoint = 0m;
			this.UsePoint = 0m;
		}

		/// <summary>伝票No</summary>
		[XmlElement("CpUniqueSlipNo")]
		public string OrderNo { get; set; }
		/// <summary>店舗カード番号</summary>
		[XmlElement("RealShopCardNo")]
		public string CardNo { get; set; }
		/// <summary>購入店舗コード</summary>
		[XmlElement("ShopCd")]
		public string ShopCode { get; set; }
		/// <summary>購入店舗名</summary>
		[XmlElement("ShopName")]
		public string ShopName { get; set; }
		/// <summary>伝票番号</summary>
		[XmlElement("SlipNo")]
		public string OrderId { get; set; }
		/// <summary>購入日時</summary>
		[XmlElement("SlipDate")]
		public string OrderDate { get; set; }
		/// <summary>POS番号</summary>
		[XmlElement("PosNo")]
		public string PosNo { get; set; }
		/// <summary>削除フラグ</summary>
		[XmlElement("DeleteFlg")]
		public string DeleteFlg { get; set; }
		/// <summary>税込伝票合計金額</summary>
		[XmlElement("AmountTotalInTax")]
		public decimal PriceTotalInTax { get; set; }
		/// <summary>税抜伝票合計金額</summary>
		[XmlElement("AmountTotalNoTax")]
		public decimal PriceTotalNoTax { get; set; }
		/// <summary>明細数</summary>
		[XmlElement("DetailNum")]
		public string ItemCount { get; set; }
		/// <summary>付与ポイント</summary>
		[XmlElement("GrantPoint")]
		public decimal GrantPoint { get; set; }
		/// <summary>利用ポイント</summary>
		[XmlElement("UsePoint")]
		public decimal UsePoint { get; set; }
		/// <summary>ネットショップ会員ID</summary>
		public string NetShopMemberId
		{
			get
			{
				var netShopMemberId = string.Format(
					"{0}{1}",
					Constants.CROSS_POINT_ELEMENT_MEMBER_INFO_NET_SHOP_MEMBER_ID,
					Constants.CROSS_POINT_AUTH_SHOP_CODE);

				var result = GetElementValue(this.OtherElements, netShopMemberId);
				return result;
			}
		}
		/// <summary>商品詳細</summary>
		public OrderDetail[] Items
		{
			get
			{
				var result = this.OtherElements
					.Where(element => element.Name.StartsWith(Constants.CROSS_POINT_ELEMENT_ORDER_HISTORY_DETAIL))
					.Select(
						element =>
						{
							var child = element.ChildNodes.Cast<XmlElement>().ToArray();
							var orderDetail = new OrderDetail
								{
									JanCode = GetElementValue(child, Constants.CROSS_POINT_PARAM_POINT_JAN_CODE),
									ProductName = GetElementValue(child, Constants.CROSS_POINT_PARAM_POINT_ITEM_NAME),
									ProductId = GetElementValue(child, Constants.CROSS_POINT_PARAM_POINT_ITEM_CODE),
									SalesPriceInTax = GetElementValue(child, Constants.CROSS_POINT_PARAM_POINT_UNIT_PRICE_INTAX),
									SalesPriceNoTax = GetElementValue(child, Constants.CROSS_POINT_PARAM_POINT_UNIT_PRICE_NO_TAX),
									Quantity = GetElementValue(child, Constants.CROSS_POINT_PARAM_POINT_SALES_NUM),
								};
							return orderDetail;
						})
					.ToArray();
				return result;
			}
		}
	}
}
