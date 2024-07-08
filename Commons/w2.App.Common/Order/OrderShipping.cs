/*
=========================================================================================================
  Module      : 配送先情報クラス(OrderShipping.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// OrderShipping の概要の説明です
	/// </summary>
	[Serializable]
	public class OrderShipping
	{
		// 定数
		public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_ID = "shipping_time_id"; // 配送希望時間帯
		public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_MESSAGE = "shipping_time_message"; // 配送希望時間帯文言
		public const string CONST_DELETE_TARGET_SHIPPING = "delete_target_shipping"; // 削除対象配送先フラグ

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderShipping()
		{
			// 何もしない //
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="drvShipping">配送先情報</param>
		public OrderShipping(DataRowView drvShipping)
		{
			//-----------------------------------------------------
			// 配送先情報設定
			//-----------------------------------------------------
			this.OrderId = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_ORDER_ID];
			this.OrderShippingNo = drvShipping[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO].ToString();
			this.ShippingName = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME];
			this.ShippingName1 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1];
			this.ShippingName2 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2];
			this.ShippingNameKana = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA];
			this.ShippingNameKana1 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1];
			this.ShippingNameKana2 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2];
			this.ShippingCountryIsoCode = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE];
			this.ShippingCountryName = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME];
			this.ShippingZip = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP];
			this.ShippingAddr1 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1];
			this.ShippingAddr2 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2];
			this.ShippingAddr3 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3];
			this.ShippingAddr4 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4];
			this.ShippingAddr5 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5];
			this.ShippingCompanyName = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME];
			this.ShippingCompanyPostName =
				(string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME];
			this.ShippingTel1 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1];
			this.ShippingDate = drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE] != DBNull.Value
				? StringUtility.ToDateString(drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE], "yyyy/MM/dd")
				: null;
			this.ShippingTime = drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME] != DBNull.Value
				? drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME].ToString()
				: null;
			this.ShippingCheckNo = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO];
			this.DateCreated = drvShipping[Constants.FIELD_ORDERSHIPPING_DATE_CREATED].ToString();
			this.DateChanged = drvShipping[Constants.FIELD_ORDERSHIPPING_DATE_CHANGED].ToString();
			this.SenderName = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_NAME];
			this.SenderName1 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_NAME1];
			this.SenderName2 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_NAME2];
			this.SenderNameKana = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA];
			this.SenderNameKana1 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1];
			this.SenderNameKana2 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2];
			this.SenderCountryIsoCode = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE];
			this.SenderCountryName = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME];
			this.SenderZip = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP];
			this.SenderAddr1 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1];
			this.SenderAddr2 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2];
			this.SenderAddr3 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3];
			this.SenderAddr4 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4];
			this.SenderAddr5 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5];
			this.SenderCompanyName = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME];
			this.SenderCompanyPostName = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME];
			this.SenderTel1 = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1];
			this.WrappingPaperType = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE];
			this.WrappingPaperName = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME];
			this.WrappingBagType = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE];
			this.AnotherShippingFlag = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG];
			this.ShippingMethod = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD];
			this.DeliveryCompanyId = (string) drvShipping[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID];
			this.StorepickupRealShopId = (string)drvShipping[Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID];
		}

		#region "プロパティ"
		/// <summary>注文ID</summary>
		public string OrderId { get; set; }
		/// <summary>配送先枝番</summary>
		public string OrderShippingNo { get; set; }
		/// <summary>配送先氏名</summary>
		public string ShippingName { get; set; }
		/// <summary>配送先氏名1</summary>
		public string ShippingName1 { get; set; }
		/// <summary>配送先氏名2</summary>
		public string ShippingName2 { get; set; }
		/// <summary>配送先氏名（かな）</summary>
		public string ShippingNameKana { get; set; }
		/// <summary>配送先氏名1（かな）</summary>
		public string ShippingNameKana1 { get; set; }
		/// <summary>配送先氏名2（かな）</summary>
		public string ShippingNameKana2 { get; set; }
		/// <summary>配送先国ISOコード</summary>
		public string ShippingCountryIsoCode { get; set; }
		/// <summary>配送先国名</summary>
		public string ShippingCountryName { get; set; }
		/// <summary>郵便番号</summary>
		public string ShippingZip { get; set; }
		/// <summary>住所1</summary>
		public string ShippingAddr1 { get; set; }
		/// <summary>住所2</summary>
		public string ShippingAddr2 { get; set; }
		/// <summary>住所3</summary>
		public string ShippingAddr3 { get; set; }
		/// <summary>住所4</summary>
		public string ShippingAddr4 { get; set; }
		/// <summary>住所5</summary>
		public string ShippingAddr5 { get; set; }
		/// <summary>企業名</summary>
		public string ShippingCompanyName { get; set; }
		/// <summary>部署名</summary>
		public string ShippingCompanyPostName { get; set; }
		/// <summary>電話番号1</summary>
		public string ShippingTel1 { get; set; }
		/// <summary>配送希望日</summary>
		public string ShippingDate { get; set; }
		/// <summary>配送希望時間帯</summary>
		public string ShippingTime { get; set; }
		/// <summary>配送伝票番号</summary>
		public string ShippingCheckNo { get; set; }
		/// <summary>作成日</summary>
		public string DateCreated { get; set; }
		/// <summary>更新日</summary>
		public string DateChanged { get; set; }
		/// <summary>配送希望時間帯文言</summary>
		public string ShippingTimeMessage { get; set; }
		/// <summary>送り主氏名</summary>
		public string SenderName { get; set; }
		/// <summary>送り主氏名1</summary>
		public string SenderName1 { get; set; }
		/// <summary>送り主氏名2</summary>
		public string SenderName2 { get; set; }
		/// <summary>送り主氏名（かな）</summary>
		public string SenderNameKana { get; set; }
		/// <summary>送り主氏名1（かな）</summary>
		public string SenderNameKana1 { get; set; }
		/// <summary>送り主氏名2（かな）</summary>
		public string SenderNameKana2 { get; set; }
		/// <summary>送り主国ISOコード</summary>
		public string SenderCountryIsoCode { get; set; }
		/// <summary>送り主国名</summary>
		public string SenderCountryName { get; set; }
		/// <summary>送り主郵便番号</summary>
		public string SenderZip { get; set; }
		/// <summary>送り主住所1</summary>
		public string SenderAddr1 { get; set; }
		/// <summary>送り主住所2</summary>
		public string SenderAddr2 { get; set; }
		/// <summary>送り主住所3</summary>
		public string SenderAddr3 { get; set; }
		/// <summary>送り主住所4</summary>
		public string SenderAddr4 { get; set; }
		/// <summary>送り主住所5</summary>
		public string SenderAddr5 { get; set; }
		/// <summary>送り主企業名</summary>
		public string SenderCompanyName { get; set; }
		/// <summary>送り主部署名</summary>
		public string SenderCompanyPostName { get; set; }
		/// <summary>送り主電話番号1</summary>
		public string SenderTel1 { get; set; }
		/// <summary>のし種類</summary>
		public string WrappingPaperType { get; set; }
		/// <summary>宛て名</summary>
		public string WrappingPaperName { get; set; }
		/// <summary>包装種類</summary>
		public string WrappingBagType { get; set; }
		/// <summary>削除対象フラグ</summary>
		/// <remarks>True:削除対象 False:削除対象外</remarks>
		public bool DeleteTarget { get; set; }
		/// <summary>別出荷フラグ</summary>
		public string AnotherShippingFlag { get; set; }
		/// <summary>配送方法</summary>
		public string ShippingMethod { get; set; }
		/// <summary>配送会社ID</summary>
		public string DeliveryCompanyId { get; set; }
		/// <summary>受取店舗</summary>
		public string StorepickupRealShopId { get; set; }
		#endregion
	}
}
