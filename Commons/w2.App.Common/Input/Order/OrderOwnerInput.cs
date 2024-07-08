/*
=========================================================================================================
  Module      : 注文者情報入力クラス (OrderOwnerInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Global;
using w2.Domain.Order;

namespace w2.App.Common.Input.Order
{
	/// <summary>
	/// 注文者情報入力クラス（登録、編集で利用）
	/// </summary>
	[Serializable]
	public class OrderOwnerInput : InputBase<OrderOwnerModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderOwnerInput()
		{
			this.OrderId = string.Empty;
			this.OwnerKbn = Constants.FLG_ORDEROWNER_OWNER_KBN_PC_USER;
			this.OwnerName = string.Empty;
			this.OwnerName1 = string.Empty;
			this.OwnerName2 = string.Empty;
			this.OwnerNameKana = string.Empty;
			this.OwnerNameKana1 = string.Empty;
			this.OwnerNameKana2 = string.Empty;
			this.OwnerZip = string.Empty;
			this.OwnerZip1 = string.Empty;
			this.OwnerZip2 = string.Empty;
			this.OwnerAddr1 = string.Empty;
			this.OwnerAddr2 = string.Empty;
			this.OwnerAddr3 = string.Empty;
			this.OwnerAddr4 = string.Empty;
			this.OwnerTel1 = string.Empty;
			this.OwnerTel1_1 = string.Empty;
			this.OwnerTel1_2 = string.Empty;
			this.OwnerTel1_3 = string.Empty;
			this.OwnerSex = Constants.FLG_ORDEROWNER_OWNER_SEX_UNKNOWN;
			this.OwnerBirth = null;
			this.OwnerCompanyName = string.Empty;
			this.OwnerCompanyPostName = string.Empty;
			this.OwnerCompanyExectiveName = string.Empty;
			this.DelFlg = "0";
			this.DateCreated = DateTime.Now.ToString();
			this.DateChanged = DateTime.Now.ToString();
			this.AccessCountryIsoCode = string.Empty;
			this.DispLanguageCode = string.Empty;
			this.DispLanguageLocaleId = string.Empty;
			this.DispCurrencyCode = string.Empty;
			this.DispCurrencyLocaleId = string.Empty;
			this.OwnerAddrCountryIsoCode = string.Empty;
			this.OwnerAddrCountryName = string.Empty;
			this.OwnerAddr5 = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public OrderOwnerInput(OrderOwnerModel model)
			: this()
		{
			this.OrderId = model.OrderId;
			this.OwnerKbn = model.OwnerKbn;
			this.OwnerName = model.OwnerName;
			this.OwnerNameKana = model.OwnerNameKana;
			this.OwnerMailAddr = model.OwnerMailAddr;
			this.OwnerZip = model.OwnerZip;
			var zip = this.OwnerZip.Split('-');
			this.OwnerZip1 = (zip.Length > 0) ? zip[0] : string.Empty;
			this.OwnerZip2 = (zip.Length > 1) ? zip[1] : string.Empty;
			this.OwnerAddr1 = model.OwnerAddr1;
			this.OwnerAddr2 = model.OwnerAddr2;
			this.OwnerAddr3 = model.OwnerAddr3;
			this.OwnerAddr4 = model.OwnerAddr4;
			this.OwnerTel1 = model.OwnerTel1;
			var tel1 = this.OwnerTel1.Split('-');
			this.OwnerTel1_1 = (tel1.Length > 0) ? tel1[0] : string.Empty;
			this.OwnerTel1_2 = (tel1.Length > 1) ? tel1[1] : string.Empty;
			this.OwnerTel1_3 = (tel1.Length > 2) ? tel1[2] : string.Empty;
			this.OwnerTel2 = model.OwnerTel2;
			this.OwnerTel3 = model.OwnerTel3;
			this.OwnerFax = model.OwnerFax;
			this.OwnerSex = model.OwnerSex;
			this.OwnerBirth = (model.OwnerBirth != null) ? model.OwnerBirth.ToString() : null;
			this.OwnerCompanyName = model.OwnerCompanyName;
			this.OwnerCompanyPostName = model.OwnerCompanyPostName;
			this.OwnerCompanyExectiveName = model.OwnerCompanyExectiveName;
			this.DelFlg = model.DelFlg;
			this.DateCreated = model.DateCreated.ToString();
			this.DateChanged = model.DateChanged.ToString();
			this.OwnerName1 = model.OwnerName1;
			this.OwnerName2 = model.OwnerName2;
			this.OwnerNameKana1 = model.OwnerNameKana1;
			this.OwnerNameKana2 = model.OwnerNameKana2;
			this.OwnerMailAddr2 = model.OwnerMailAddr2;
			this.AccessCountryIsoCode = model.AccessCountryIsoCode;
			this.DispLanguageCode = model.DispLanguageCode;
			this.DispLanguageLocaleId = model.DispLanguageLocaleId;
			this.DispCurrencyCode = model.DispCurrencyCode;
			this.DispCurrencyLocaleId = model.DispCurrencyLocaleId;
			this.OwnerAddrCountryIsoCode = model.OwnerAddrCountryIsoCode;
			this.OwnerAddrCountryName = model.OwnerAddrCountryName;
			this.OwnerAddr5 = model.OwnerAddr5;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override OrderOwnerModel CreateModel()
		{
			var model = new OrderOwnerModel
			{
				OrderId = this.OrderId,
				OwnerKbn = this.OwnerKbn,
				OwnerName = this.OwnerName1 + this.OwnerName2,
				OwnerNameKana = this.OwnerNameKana1 + this.OwnerNameKana2,
				OwnerMailAddr = this.OwnerMailAddr,
				OwnerZip = (string.IsNullOrEmpty(this.OwnerZip1 + this.OwnerZip2) == false)
					? string.Join("-", this.OwnerZip1, this.OwnerZip2)
					: this.OwnerZip,
				OwnerAddr1 = this.OwnerAddr1,
				OwnerAddr2 = this.OwnerAddr2,
				OwnerAddr3 = this.OwnerAddr3,
				OwnerAddr4 = this.OwnerAddr4,
				OwnerTel1 = (string.IsNullOrEmpty(this.OwnerTel1_1 + this.OwnerTel1_2 + OwnerTel1_3) == false)
					? string.Join("-", this.OwnerTel1_1, this.OwnerTel1_2, this.OwnerTel1_3)
					: this.OwnerTel1,
				OwnerTel2 = this.OwnerTel2,
				OwnerTel3 = this.OwnerTel3,
				OwnerFax = this.OwnerFax,
				OwnerSex = this.OwnerSex,
				OwnerBirth = (this.OwnerBirth != null) ? DateTime.Parse(this.OwnerBirth) : (DateTime?)null,
				OwnerCompanyName = this.OwnerCompanyName,
				OwnerCompanyPostName = this.OwnerCompanyPostName,
				OwnerCompanyExectiveName = this.OwnerCompanyExectiveName,
				DelFlg = this.DelFlg,
				DateCreated = DateTime.Parse(this.DateCreated),
				DateChanged = DateTime.Parse(this.DateChanged),
				OwnerName1 = this.OwnerName1,
				OwnerName2 = this.OwnerName2,
				OwnerNameKana1 = this.OwnerNameKana1,
				OwnerNameKana2 = this.OwnerNameKana2,
				OwnerMailAddr2 = this.OwnerMailAddr2,
				AccessCountryIsoCode = this.AccessCountryIsoCode,
				DispLanguageCode = this.DispLanguageCode,
				DispLanguageLocaleId = this.DispLanguageLocaleId,
				DispCurrencyCode = this.DispCurrencyCode,
				DispCurrencyLocaleId = this.DispCurrencyLocaleId,
				OwnerAddrCountryIsoCode = this.OwnerAddrCountryIsoCode,
				OwnerAddrCountryName = this.OwnerAddrCountryName,
				OwnerAddr5 = this.OwnerAddr5,
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="orderPaymentKbn">支払区分</param>
		/// <returns>エラーメッセージ</returns>
		public w2.Common.Util.Validator.ErrorMessageList Validate(string orderPaymentKbn)
		{
			var validationName = this.IsAddrJp
				? (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
					? "OrderOwnerAmazonModifyInput"
					: "OrderOwnerModifyInput"
				: "OrderOwnerModifyInputGlobal";
			return w2.Common.Util.Validator.Validate(
				validationName,
				this.DataSource,
				Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE,
				this.OwnerAddrCountryIsoCode);
		}

		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_ORDER_ID] = value; }
		}
		/// <summary>注文者区分</summary>
		public string OwnerKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_KBN] = value; }
		}
		/// <summary>注文者氏名</summary>
		public string OwnerName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME] = value; }
		}
		/// <summary>注文者氏名かな</summary>
		public string OwnerNameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA] = value; }
		}
		/// <summary>メールアドレス</summary>
		public string OwnerMailAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR] = value; }
		}
		/// <summary>郵便番号</summary>
		public string OwnerZip
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ZIP]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ZIP] = value; }
		}
		/// <summary>郵便番号1</summary>
		public string OwnerZip1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ZIP + "_1"]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ZIP + "_1"] = value; }
		}
		/// <summary>郵便番号2</summary>
		public string OwnerZip2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ZIP + "_2"]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ZIP + "_2"] = value; }
		}
		/// <summary>住所1</summary>
		public string OwnerAddr1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR1]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR1] = value; }
		}
		/// <summary>住所2</summary>
		public string OwnerAddr2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR2]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR2] = value; }
		}
		/// <summary>住所3</summary>
		public string OwnerAddr3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR3]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR3] = value; }
		}
		/// <summary>住所４</summary>
		public string OwnerAddr4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR4]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR4] = value; }
		}
		/// <summary>電話番号1</summary>
		public string OwnerTel1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL1]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL1] = value; }
		}
		/// <summary>電話番号1_1</summary>
		public string OwnerTel1_1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL1 + "_1"]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL1 + "_1"] = value; }
		}
		/// <summary>電話番号1_2</summary>
		public string OwnerTel1_2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL1 + "_2"]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL1 + "_2"] = value; }
		}
		/// <summary>電話番号1_3</summary>
		public string OwnerTel1_3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL1 + "_3"]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL1 + "_3"] = value; }
		}
		/// <summary>電話番号2</summary>
		public string OwnerTel2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL2]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL2] = value; }
		}
		/// <summary>電話番号3</summary>
		public string OwnerTel3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL3]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL3] = value; }
		}
		/// <summary>ＦＡＸ</summary>
		public string OwnerFax
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_FAX]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_FAX] = value; }
		}
		/// <summary>性別</summary>
		public string OwnerSex
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_SEX]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_SEX] = value; }
		}
		/// <summary>生年月日</summary>
		public string OwnerBirth
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_BIRTH]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_BIRTH] = value; }
		}
		/// <summary>企業名</summary>
		public string OwnerCompanyName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME] = value; }
		}
		/// <summary>部署名</summary>
		public string OwnerCompanyPostName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME] = value; }
		}
		/// <summary>役職名</summary>
		public string OwnerCompanyExectiveName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_EXECTIVE_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_EXECTIVE_NAME] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_DATE_CHANGED] = value; }
		}
		/// <summary>注文者氏名1</summary>
		public string OwnerName1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME1]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME1] = value; }
		}
		/// <summary>注文者氏名2</summary>
		public string OwnerName2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME2]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME2] = value; }
		}
		/// <summary>注文者氏名かな1</summary>
		public string OwnerNameKana1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1] = value; }
		}
		/// <summary>注文者氏名かな2</summary>
		public string OwnerNameKana2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2] = value; }
		}
		/// <summary>メールアドレス2</summary>
		public string OwnerMailAddr2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2] = value; }
		}
		/// <summary>ユーザー管理レベルID</summary>
		public string UserManagementLevelId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID]; }
			set { this.DataSource[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID] = value; }
		}
		/// <summary>ユーザーメモ</summary>
		public string UserMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_USER_MEMO]; }
			set { this.DataSource[Constants.FIELD_USER_USER_MEMO] = value; }
		}
		/// <summary>アクセス国ISOコード</summary>
		public string AccessCountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_ACCESS_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_ACCESS_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>表示言語コード</summary>
		public string DispLanguageCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_CODE] = value; }
		}
		/// <summary>表示言語ロケールID</summary>
		public string DispLanguageLocaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_LOCALE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_LOCALE_ID] = value; }
		}
		/// <summary>表示通貨コード</summary>
		public string DispCurrencyCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_CODE] = value; }
		}
		/// <summary>表示通貨ロケールID</summary>
		public string DispCurrencyLocaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_LOCALE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_LOCALE_ID] = value; }
		}
		/// <summary>住所国ISOコード</summary>
		public string OwnerAddrCountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>住所国名</summary>
		public string OwnerAddrCountryName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME] = value; }
		}
		/// <summary>住所5</summary>
		public string OwnerAddr5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR5]; }
			set { this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_ADDR5] = value; }
		}
		/// <summary>住所は日本か</summary>
		public bool IsAddrJp
		{
			get { return GlobalAddressUtil.IsCountryJp(this.OwnerAddrCountryIsoCode); }
		}
		/// <summary>Is Shipping Address Taiwan</summary>
		public bool IsAddrTw
		{
			get { return GlobalAddressUtil.IsCountryTw(this.OwnerAddrCountryIsoCode); }
		}
		#endregion
	}
}
