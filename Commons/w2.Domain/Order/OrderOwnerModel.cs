/*
=========================================================================================================
  Module      : 注文者情報モデル (OrderOwnerModel.cs)
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
	/// 注文者情報モデル
	/// </summary>
	[Serializable]
	public partial class OrderOwnerModel : ModelBase<OrderOwnerModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderOwnerModel()
		{
			this.OrderId = "";
			this.OwnerKbn = Constants.FLG_ORDEROWNER_OWNER_KBN_PC_USER;
			this.OwnerName = "";
			this.OwnerNameKana = "";
			this.OwnerMailAddr = "";
			this.OwnerZip = "";
			this.OwnerAddr1 = "";
			this.OwnerAddr2 = "";
			this.OwnerAddr3 = "";
			this.OwnerAddr4 = "";
			this.OwnerTel1 = "";
			this.OwnerTel2 = "";
			this.OwnerTel3 = "";
			this.OwnerFax = "";
			this.OwnerSex = Constants.FLG_ORDEROWNER_OWNER_SEX_UNKNOWN;
			this.OwnerBirth = null;
			this.OwnerCompanyName = "";
			this.OwnerCompanyPostName = "";
			this.OwnerCompanyExectiveName = "";
			this.DelFlg = "0";
			this.OwnerName1 = "";
			this.OwnerName2 = "";
			this.OwnerNameKana1 = "";
			this.OwnerNameKana2 = "";
			this.OwnerMailAddr2 = "";
			this.AccessCountryIsoCode = "";
			this.DispLanguageCode = "";
			this.DispLanguageLocaleId = "";
			this.DispCurrencyCode = "";
			this.DispCurrencyLocaleId = "";
			this.OwnerAddrCountryIsoCode = "";
			this.OwnerAddrCountryName = "";
			this.OwnerAddr5 = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderOwnerModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderOwnerModel(Hashtable source)
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
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_ORDER_ID] = value; }
		}
		/// <summary>注文者区分</summary>
		[UpdateData(2, "owner_kbn")]
		public string OwnerKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_KBN] = value; }
		}
		/// <summary>注文者氏名</summary>
		[UpdateData(3, "owner_name")]
		public string OwnerName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME] = value; }
		}
		/// <summary>注文者氏名かな</summary>
		[UpdateData(4, "owner_name_kana")]
		public string OwnerNameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA] = value; }
		}
		/// <summary>メールアドレス</summary>
		[UpdateData(5, "owner_mail_addr")]
		public string OwnerMailAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR] = value; }
		}
		/// <summary>郵便番号</summary>
		[UpdateData(6, "owner_zip")]
		public string OwnerZip
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ZIP]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ZIP] = value; }
		}
		/// <summary>住所1</summary>
		[UpdateData(7, "owner_addr1")]
		public string OwnerAddr1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR1]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR1] = value; }
		}
		/// <summary>住所2</summary>
		[UpdateData(8, "owner_addr2")]
		public string OwnerAddr2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR2]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR2] = value; }
		}
		/// <summary>住所3</summary>
		[UpdateData(9, "owner_addr3")]
		public string OwnerAddr3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR3]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR3] = value; }
		}
		/// <summary>住所４</summary>
		[UpdateData(10, "owner_addr4")]
		public string OwnerAddr4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR4]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR4] = value; }
		}
		/// <summary>電話番号1</summary>
		[UpdateData(11, "owner_tel1")]
		public string OwnerTel1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL1]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL1] = value; }
		}
		/// <summary>電話番号2</summary>
		[UpdateData(12, "owner_tel2")]
		public string OwnerTel2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL2]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL2] = value; }
		}
		/// <summary>電話番号3</summary>
		[UpdateData(13, "owner_tel3")]
		public string OwnerTel3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL3]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL3] = value; }
		}
		/// <summary>ＦＡＸ</summary>
		[UpdateData(14, "owner_fax")]
		public string OwnerFax
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_FAX]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_FAX] = value; }
		}
		/// <summary>性別</summary>
		[UpdateData(15, "owner_sex")]
		public string OwnerSex
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_SEX]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_SEX] = value; }
		}
		/// <summary>生年月日</summary>
		[UpdateData(16, "owner_birth")]
		public DateTime? OwnerBirth
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_BIRTH] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_BIRTH];
			}
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_BIRTH] = value; }
		}
		/// <summary>企業名</summary>
		[UpdateData(17, "owner_company_name")]
		public string OwnerCompanyName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME] = value; }
		}
		/// <summary>部署名</summary>
		[UpdateData(18, "owner_company_post_name")]
		public string OwnerCompanyPostName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME] = value; }
		}
		/// <summary>役職名</summary>
		[UpdateData(19, "owner_company_exective_name")]
		public string OwnerCompanyExectiveName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_EXECTIVE_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_EXECTIVE_NAME] = value; }
		}
		/// <summary>削除フラグ</summary>
		[UpdateData(20, "del_flg")]
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		[UpdateData(21, "date_created")]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDEROWNER_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[UpdateData(22, "date_changed")]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDEROWNER_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_DATE_CHANGED] = value; }
		}
		/// <summary>注文者氏名1</summary>
		[UpdateData(23, "owner_name1")]
		public string OwnerName1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME1]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME1] = value; }
		}
		/// <summary>注文者氏名2</summary>
		[UpdateData(24, "owner_name2")]
		public string OwnerName2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME2]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME2] = value; }
		}
		/// <summary>注文者氏名かな1</summary>
		[UpdateData(25, "owner_name_kana1")]
		public string OwnerNameKana1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1] = value; }
		}
		/// <summary>注文者氏名かな2</summary>
		[UpdateData(26, "owner_name_kana2")]
		public string OwnerNameKana2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2] = value; }
		}
		/// <summary>メールアドレス2</summary>
		[UpdateData(27, "owner_mail_addr2")]
		public string OwnerMailAddr2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2] = value; }
		}
		/// <summary>アクセス国ISOコード</summary>
		[UpdateDataAttribute(28, "access_country_iso_code")]
		public string AccessCountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_ACCESS_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_ACCESS_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>表示言語コード</summary>
		[UpdateDataAttribute(29, "disp_language_code")]
		public string DispLanguageCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_CODE] = value; }
		}
		/// <summary>表示言語ロケールID</summary>
		[UpdateDataAttribute(30, "disp_language_locale_id")]
		public string DispLanguageLocaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_LOCALE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_LOCALE_ID] = value; }
		}
		/// <summary>表示通貨コード</summary>
		[UpdateDataAttribute(31, "disp_currency_code")]
		public string DispCurrencyCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_CODE] = value; }
		}
		/// <summary>表示通貨ロケールID</summary>
		[UpdateDataAttribute(32, "disp_currency_locale_id")]
		public string DispCurrencyLocaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_LOCALE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_LOCALE_ID] = value; }
		}
		/// <summary>住所国ISOコード</summary>
		[UpdateDataAttribute(33, "owner_addr_country_iso_code")]
		public string OwnerAddrCountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>住所国名</summary>
		[UpdateDataAttribute(34, "owner_addr_country_name")]
		public string OwnerAddrCountryName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME] = value; }
		}
		/// <summary>住所5</summary>
		[UpdateDataAttribute(35, "owner_addr5")]
		public string OwnerAddr5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR5]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR5] = value; }
		}
		#endregion
	}
}
