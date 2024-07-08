/*
=========================================================================================================
  Module      : ユーザー配送先情報モデル (UserShippingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.UserShipping
{
	/// <summary>
	/// ユーザー配送先情報モデル
	/// </summary>
	[Serializable]
	public partial class UserShippingModel : ModelBase<UserShippingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserShippingModel()
		{
			this.ShippingNo = 0;	// デフォルトは0（これで有り無し判定している）
			this.ShippingTel2 = "";
			this.ShippingTel3 = "";
			this.ShippingFax = "";
			this.ShippingCompany = "0";
			this.DelFlg = "0";
			this.ShippingCountryIsoCode = string.Empty;
			this.ShippingCountryName = string.Empty;
			this.ShippingAddr5 = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserShippingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserShippingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		[UpdateData(1, "user_id")]
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_USER_ID] = value; }
		}
		/// <summary>配送先枝番</summary>
		[UpdateData(2, "shipping_no")]
		public int ShippingNo
		{
			get { return (int)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NO]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NO] = value; }
		}
		/// <summary>配送先名</summary>
		[UpdateData(3, "name")]
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_NAME]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_NAME] = value; }
		}
		/// <summary>配送先氏名</summary>
		[UpdateData(4, "shipping_name")]
		public string ShippingName
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME] = value; }
		}
		/// <summary>配送先氏名かな</summary>
		[UpdateData(5, "shipping_name_kana")]
		public string ShippingNameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA] = value; }
		}
		/// <summary>郵便番号</summary>
		[UpdateData(6, "shipping_zip")]
		public string ShippingZip
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ZIP]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ZIP] = value; }
		}
		/// <summary>住所1</summary>
		[UpdateData(7, "shipping_addr1")]
		public string ShippingAddr1
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR1]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR1] = value; }
		}
		/// <summary>住所2</summary>
		[UpdateData(8, "shipping_addr2")]
		public string ShippingAddr2
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR2]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR2] = value; }
		}
		/// <summary>住所3</summary>
		[UpdateData(9, "shipping_addr3")]
		public string ShippingAddr3
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR3]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR3] = value; }
		}
		/// <summary>住所4</summary>
		[UpdateData(10, "shipping_addr4")]
		public string ShippingAddr4
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR4]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR4] = value; }
		}
		/// <summary>電話番号1</summary>
		[UpdateData(11, "shipping_tel1")]
		public string ShippingTel1
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL1]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL1] = value; }
		}
		/// <summary>電話番号2</summary>
		[UpdateData(12, "shipping_tel2")]
		public string ShippingTel2
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL2]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL2] = value; }
		}
		/// <summary>電話番号3</summary>
		[UpdateData(13, "shipping_tel3")]
		public string ShippingTel3
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL3]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_TEL3] = value; }
		}
		/// <summary>ＦＡＸ</summary>
		[UpdateData(14, "shipping_fax")]
		public string ShippingFax
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_FAX]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_FAX] = value; }
		}
		/// <summary>配送業者</summary>
		[UpdateData(15, "shipping_company")]
		public string ShippingCompany
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY] = value; }
		}
		/// <summary>削除フラグ</summary>
		[UpdateData(16, "del_flg")]
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		[UpdateData(17, "date_created")]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERSHIPPING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[UpdateData(18, "date_changed")]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERSHIPPING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_DATE_CHANGED] = value; }
		}
		/// <summary>配送先氏名1</summary>
		[UpdateData(19, "shipping_name1")]
		public string ShippingName1
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME1]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME1] = value; }
		}
		/// <summary>配送先氏名2</summary>
		[UpdateData(20, "shipping_name2")]
		public string ShippingName2
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME2]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME2] = value; }
		}
		/// <summary>配送先氏名かな1</summary>
		[UpdateData(21, "shipping_name_kana1")]
		public string ShippingNameKana1
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA1]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA1] = value; }
		}
		/// <summary>配送先氏名かな2</summary>
		[UpdateData(22, "shipping_name_kana2")]
		public string ShippingNameKana2
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA2]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NAME_KANA2] = value; }
		}
		/// <summary>企業名</summary>
		[UpdateData(23, "shipping_company_name")]
		public string ShippingCompanyName
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_NAME]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_NAME] = value; }
		}
		/// <summary>部署名</summary>
		[UpdateData(24, "shipping_company_post_name")]
		public string ShippingCompanyPostName
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_POST_NAME]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COMPANY_POST_NAME] = value; }
		}
		/// <summary>住所国ISOコード</summary>
		[UpdateDataAttribute(25, "shipping_country_iso_code")]
		public string ShippingCountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>住所国名</summary>
		[UpdateDataAttribute(26, "shipping_country_name")]
		public string ShippingCountryName
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COUNTRY_NAME]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_COUNTRY_NAME] = value; }
		}
		/// <summary>住所5</summary>
		[UpdateDataAttribute(27, "shipping_addr5")]
		public string ShippingAddr5
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR5]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_ADDR5] = value; }
		}
		/// <summary>店舗受取フラグ</summary>
		[UpdateDataAttribute(28, "shipping_receiving_store_flg")]
		public string ShippingReceivingStoreFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_RECEIVING_STORE_FLG]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_RECEIVING_STORE_FLG] = value; }
		}
		/// <summary>店舗受取店舗ID</summary>
		[UpdateDataAttribute(29, "shipping_receiving_store_id")]
		public string ShippingReceivingStoreId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_RECEIVING_STORE_ID]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_RECEIVING_STORE_ID] = value; }
		}
		/// <summary>コンビニ受取：受取方法</summary>
		[UpdateDataAttribute(30, "shipping_receiving_store_type")]
		public string ShippingReceivingStoreType
		{
			get { return (string)this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE]; }
			set { this.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE] = value; }
		}
		#endregion
	}
}