/*
=========================================================================================================
  Module      : 注文配送先情報モデル (OrderShippingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Order
{
	/// <summary>
	/// 注文配送先情報モデル
	/// </summary>
	[Serializable]
	public partial class OrderShippingModel : ModelBase<OrderShippingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderShippingModel()
		{
			this.OrderId = "";
			this.OrderShippingNo = 1;
			this.ShippingName = "";
			this.ShippingNameKana = "";
			this.ShippingZip = "";
			this.ShippingAddr1 = "";
			this.ShippingAddr2 = "";
			this.ShippingAddr3 = "";
			this.ShippingAddr4 = "";
			this.ShippingTel1 = "";
			this.ShippingTel2 = "";
			this.ShippingTel3 = "";
			this.ShippingFax = "";
			this.ShippingCompany = "0";
			this.ShippingDate = null;
			this.ShippingTime = "";
			this.ShippingCheckNo = "";
			this.DelFlg = "0";
			this.ShippingName1 = "";
			this.ShippingName2 = "";
			this.ShippingNameKana1 = "";
			this.ShippingNameKana2 = "";
			this.SenderName = "";
			this.SenderName1 = "";
			this.SenderName2 = "";
			this.SenderNameKana = "";
			this.SenderNameKana1 = "";
			this.SenderNameKana2 = "";
			this.SenderZip = "";
			this.SenderAddr1 = "";
			this.SenderAddr2 = "";
			this.SenderAddr3 = "";
			this.SenderAddr4 = "";
			this.SenderTel1 = "";
			this.WrappingPaperType = "";
			this.WrappingPaperName = "";
			this.WrappingBagType = "";
			this.ShippingCompanyName = "";
			this.ShippingCompanyPostName = "";
			this.SenderCompanyName = "";
			this.SenderCompanyPostName = "";
			this.AnotherShippingFlg = Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID;
			this.ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
			this.DeliveryCompanyId = "";
			this.ShippingCountryIsoCode = string.Empty;
			this.ShippingCountryName = string.Empty;
			this.ShippingAddr5 = string.Empty;
			this.SenderCountryIsoCode = string.Empty;
			this.SenderCountryName = string.Empty;
			this.SenderAddr5 = string.Empty;
			this.Items = new OrderItemModel[0];
			this.ScheduledShippingDate = null;
			this.ExternalShippingCooperationId = "";
			this.ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;
			this.ShippingReceivingStoreId = string.Empty;
			this.ShippingExternalDelivertyStatus = string.Empty;
			this.ShippingStatus = string.Empty;
			this.ShippingStatusUpdateDate = null;
			this.ShippingReceivingMailDate = null;
			this.ShippingReceivingStoreType = string.Empty;
			this.ShippingStatusCode = string.Empty;
			this.ShippingOfficeName = string.Empty;
			this.ShippingHandyTime = string.Empty;
			this.ShippingCurrentStatus = string.Empty;
			this.ShippingStatusDetail = string.Empty;
			this.StorePickupRealShopId = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderShippingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderShippingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		[UpdateData(1, "order_id")]
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_ORDER_ID] = value; }
		}
		/// <summary>配送先枝番</summary>
		[UpdateData(2, "order_shipping_no")]
		public int OrderShippingNo
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO] = value; }
		}
		/// <summary>配送先氏名</summary>
		[UpdateData(3, "shipping_name")]
		public string ShippingName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME] = value; }
		}
		/// <summary>配送先氏名かな</summary>
		[UpdateData(4, "shipping_name_kana")]
		public string ShippingNameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA] = value; }
		}
		/// <summary>郵便番号</summary>
		[UpdateData(5, "shipping_zip")]
		public string ShippingZip
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP] = value; }
		}
		/// <summary>住所1</summary>
		[UpdateData(6, "shipping_addr1")]
		public string ShippingAddr1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1] = value; }
		}
		/// <summary>住所2</summary>
		[UpdateData(7, "shipping_addr2")]
		public string ShippingAddr2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2] = value; }
		}
		/// <summary>住所3</summary>
		[UpdateData(8, "shipping_addr3")]
		public string ShippingAddr3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3] = value; }
		}
		/// <summary>住所４</summary>
		[UpdateData(9, "shipping_addr4")]
		public string ShippingAddr4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4] = value; }
		}
		/// <summary>電話番号1</summary>
		[UpdateData(10, "shipping_tel1")]
		public string ShippingTel1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1] = value; }
		}
		/// <summary>電話番号2</summary>
		[UpdateData(11, "shipping_tel2")]
		public string ShippingTel2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL2] = value; }
		}
		/// <summary>電話番号3</summary>
		[UpdateData(12, "shipping_tel3")]
		public string ShippingTel3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL3]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL3] = value; }
		}
		/// <summary>ＦＡＸ</summary>
		[UpdateData(13, "shipping_fax")]
		public string ShippingFax
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_FAX]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_FAX] = value; }
		}
		/// <summary>配送業者</summary>
		[UpdateData(14, "shipping_company")]
		public string ShippingCompany
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY] = value; }
		}
		/// <summary>配送希望日</summary>
		[UpdateData(15, "shipping_date")]
		public DateTime? ShippingDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE] = value; }
		}
		/// <summary>配送希望時間帯</summary>
		[UpdateData(16, "shipping_time")]
		public string ShippingTime
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME] = value; }
		}
		/// <summary>配送伝票番号</summary>
		[UpdateData(17, "shipping_check_no")]
		public string ShippingCheckNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO] = value; }
		}
		/// <summary>削除フラグ</summary>
		[UpdateData(18, "del_flg")]
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		[UpdateData(19, "date_created")]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDERSHIPPING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[UpdateData(20, "date_changed")]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDERSHIPPING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_DATE_CHANGED] = value; }
		}
		/// <summary>配送先氏名1</summary>
		[UpdateData(21, "shipping_name1")]
		public string ShippingName1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1] = value; }
		}
		/// <summary>配送先氏名2</summary>
		[UpdateData(22, "shipping_name2")]
		public string ShippingName2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2] = value; }
		}
		/// <summary>配送先氏名かな1</summary>
		[UpdateData(23, "shipping_name_kana1")]
		public string ShippingNameKana1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1] = value; }
		}
		/// <summary>配送先氏名かな2</summary>
		[UpdateData(24, "shipping_name_kana2")]
		public string ShippingNameKana2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2] = value; }
		}
		/// <summary>送り主氏名</summary>
		[UpdateData(25, "sender_name")]
		public string SenderName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME] = value; }
		}
		/// <summary>送り主氏名1</summary>
		[UpdateData(26, "sender_name1")]
		public string SenderName1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME1] = value; }
		}
		/// <summary>送り主氏名2</summary>
		[UpdateData(27, "sender_name2")]
		public string SenderName2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME2] = value; }
		}
		/// <summary>送り主氏名かな</summary>
		[UpdateData(28, "sender_name_kana")]
		public string SenderNameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA] = value; }
		}
		/// <summary>送り主氏名かな1</summary>
		[UpdateData(29, "sender_name_kana1")]
		public string SenderNameKana1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1] = value; }
		}
		/// <summary>送り主氏名かな2</summary>
		[UpdateData(30, "sender_name_kana2")]
		public string SenderNameKana2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2] = value; }
		}
		/// <summary>送り主郵便番号</summary>
		[UpdateData(31, "sender_zip")]
		public string SenderZip
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP] = value; }
		}
		/// <summary>送り主住所1</summary>
		[UpdateData(32, "sender_addr1")]
		public string SenderAddr1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1] = value; }
		}
		/// <summary>送り主住所2</summary>
		[UpdateData(33, "sender_addr2")]
		public string SenderAddr2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2] = value; }
		}
		/// <summary>送り主住所3</summary>
		[UpdateData(34, "sender_addr3")]
		public string SenderAddr3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3] = value; }
		}
		/// <summary>送り主住所４</summary>
		[UpdateData(35, "sender_addr4")]
		public string SenderAddr4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4] = value; }
		}
		/// <summary>送り主電話番号1</summary>
		[UpdateData(36, "sender_tel1")]
		public string SenderTel1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1] = value; }
		}
		/// <summary>のし種類</summary>
		[UpdateData(37, "wrapping_paper_type")]
		public string WrappingPaperType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE] = value; }
		}
		/// <summary>のし差出人</summary>
		[UpdateData(38, "wrapping_paper_name")]
		public string WrappingPaperName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME] = value; }
		}
		/// <summary>包装種類</summary>
		[UpdateData(39, "wrapping_bag_type")]
		public string WrappingBagType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE] = value; }
		}
		/// <summary>配送先企業名</summary>
		[UpdateData(40, "shipping_company_name")]
		public string ShippingCompanyName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME] = value; }
		}
		/// <summary>配送先部署名</summary>
		[UpdateData(41, "shipping_company_post_name")]
		public string ShippingCompanyPostName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME] = value; }
		}
		/// <summary>送り主企業名</summary>
		[UpdateData(42, "sender_company_name")]
		public string SenderCompanyName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME] = value; }
		}
		/// <summary>送り主部署名</summary>
		[UpdateData(43, "sender_company_post_name")]
		public string SenderCompanyPostName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME] = value; }
		}
		/// <summary>別送フラグ</summary>
		[UpdateData(44, "another_shipping_flg")]
		public string AnotherShippingFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG] = value; }
		}
		/// <summary>配送方法</summary>
		[UpdateData(45, "shipping_method")]
		public string ShippingMethod
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD] = value; }
		}
		/// <summary>配送会社ID</summary>
		[UpdateData(46, "delivery_company_id")]
		public string DeliveryCompanyId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID] = value; }
		}
		/// <summary>配送先国ISOコード</summary>
		[UpdateDataAttribute(47, "shipping_country_iso_code")]
		public string ShippingCountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>配送先国名</summary>
		[UpdateDataAttribute(48, "shipping_country_name")]
		public string ShippingCountryName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME] = value; }
		}
		/// <summary>住所5</summary>
		[UpdateDataAttribute(49, "shipping_addr5")]
		public string ShippingAddr5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5] = value; }
		}
		/// <summary>送り主国ISOコード</summary>
		[UpdateDataAttribute(50, "sender_country_iso_code")]
		public string SenderCountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>送り主国名</summary>
		[UpdateDataAttribute(51, "sender_country_name")]
		public string SenderCountryName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME] = value; }
		}
		/// <summary>送り主住所5</summary>
		[UpdateDataAttribute(52, "sender_addr5")]
		public string SenderAddr5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5] = value; }
		}
		/// <summary>出荷予定日</summary>
		[UpdateDataAttribute(54, "scheduled_shipping_date")]
		public DateTime? ScheduledShippingDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE] = value; }
		}
		/// <summary>外部連携ID</summary>
		[UpdateDataAttribute(55, "external_shipping_cooperation_id")]
		public string ExternalShippingCooperationId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_EXTERNAL_SHIPPING_COOPERATION_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_EXTERNAL_SHIPPING_COOPERATION_ID] = value; }
		}
		/// <summary>店舗受取フラグ</summary>
		[UpdateDataAttribute(56, "shipping_receiving_store_flg")]
		public string ShippingReceivingStoreFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG] = value; }
		}
		/// <summary>店舗受取店舗ID</summary>
		[UpdateDataAttribute(57, "shipping_receiving_store_id")]
		public string ShippingReceivingStoreId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID] = value; }
		}
		/// <summary>配送のペリカン管理番号</summary>
		[UpdateDataAttribute(58, "shipping_external_deliverty_status")]
		public string ShippingExternalDelivertyStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_EXTERNAL_DELIVERTY_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_EXTERNAL_DELIVERTY_STATUS] = value; }
		}
		/// <summary>配送状態</summary>
		[UpdateDataAttribute(59, "shipping_status")]
		public string ShippingStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS] = value; }
		}
		/// <summary>配送状態更新日</summary>
		[UpdateDataAttribute(60, "shipping_status_update_date")]
		public DateTime? ShippingStatusUpdateDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_UPDATE_DATE] == DBNull.Value) return null;

				return (DateTime?)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_UPDATE_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_UPDATE_DATE] = value; }
		}
		/// <summary>店舗受取メール送信日</summary>
		[UpdateDataAttribute(61, "shipping_receiving_mail_date")]
		public DateTime? ShippingReceivingMailDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_MAIL_DATE] == DBNull.Value) return null;

				return (DateTime?)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_MAIL_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_MAIL_DATE] = value; }
		}
		/// <summary>コンビニ受取：受取方法</summary>
		[UpdateDataAttribute(62, "shipping_receiving_store_type")]
		public string ShippingReceivingStoreType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE] = value; }
		}
		/// <summary>完了状態コード</summary>
		[UpdateDataAttribute(63, "shipping_status_code")]
		public string ShippingStatusCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE] = value; }
		}
		/// <summary>営業所略称</summary>
		[UpdateDataAttribute(64, "shipping_office_name")]
		public string ShippingOfficeName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_OFFICE_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_OFFICE_NAME] = value; }
		}
		/// <summary>Handy操作時間</summary>
		[UpdateDataAttribute(65, "shipping_handy_time")]
		public string ShippingHandyTime
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_HANDY_TIME]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_HANDY_TIME] = value; }
		}
		/// <summary>現在の状態</summary>
		[UpdateDataAttribute(66, "shipping_current_status")]
		public string ShippingCurrentStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS] = value; }
		}
		/// <summary>状態説明</summary>
		[UpdateDataAttribute(67, "shipping_status_detail")]
		public string ShippingStatusDetail
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_DETAIL]; }
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_DETAIL] = value; }
		}
		/// <summary>受取店舗</summary>
		[UpdateDataAttribute(68, "storepickup_real_shop_id")]
		public string StorePickupRealShopId
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID] == DBNull.Value) return string.Empty;
				return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID];
			}
			set { this.DataSource[Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID] = value; }
		}
		#endregion
	}
}
