/*
=========================================================================================================
  Module      : 注文者情報クラス(OrderOwner.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.App.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// OrderOwner の概要の説明です
	/// </summary>
	[Serializable]
	public class OrderOwner
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="drvOwner">注文者情報</param>
		public OrderOwner(DataRowView drvOwner)
		{
			//-----------------------------------------------------
			// 注文者情報設定
			//-----------------------------------------------------
			this.OrderId = (string) drvOwner[Constants.FIELD_ORDEROWNER_ORDER_ID];
			this.OwnerKbn = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_KBN];
			this.OwnerName = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME];
			this.OwnerName1 = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME1];
			this.OwnerName2 = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME2];
			this.OwnerNameKana = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA];
			this.OwnerNameKana1 = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1];
			this.OwnerNameKana2 = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2];
			this.OwnerMailAddr = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR];
			this.OwnerMailAddr2 = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2];
			this.OwnerAddrCountryIsoCode = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE];
			this.OwnerAddrCountryName = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME];
			this.OwnerZip = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_ZIP];
			this.OwnerAddr1 = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR1];
			this.OwnerAddr2 = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR2];
			this.OwnerAddr3 = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR3];
			this.OwnerAddr4 = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR4];
			this.OwnerAddr5 = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_ADDR5];
			this.OwnerCompanyName = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME];
			this.OwnerCompanyPostName = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME];
			this.OwnerTel1 = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_TEL1];
			this.OwnerTel2 = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_TEL2];
			this.OwnerSex = (string) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_SEX];
			this.OwnerBirth = (drvOwner[Constants.FIELD_ORDEROWNER_OWNER_BIRTH] != DBNull.Value)
				? ((DateTime) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_BIRTH]).ToString("yyyy/MM/dd")
				: null;
			this.OwnerBirthDisp = (drvOwner[Constants.FIELD_ORDEROWNER_OWNER_BIRTH] != DBNull.Value)
				? DateTimeUtility.ToStringForManager(
					(DateTime) drvOwner[Constants.FIELD_ORDEROWNER_OWNER_BIRTH],
					DateTimeUtility.FormatType.LongDate2Letter)
				: null;
			this.DateCreated = drvOwner[Constants.FIELD_ORDEROWNER_DATE_CREATED].ToString();
			this.DateChanged = drvOwner[Constants.FIELD_ORDEROWNER_DATE_CHANGED].ToString();
			this.AccessCountryIsoCode = (string) drvOwner[Constants.FIELD_ORDEROWNER_ACCESS_COUNTRY_ISO_CODE];
			this.DispLanguageCode = (string) drvOwner[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_CODE];
			this.DispLanguageLocaleId = (string) drvOwner[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_LOCALE_ID];
			this.DispCurrencyCode = (string) drvOwner[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_CODE];
			this.DispCurrencyLocaleId = (string) drvOwner[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_LOCALE_ID];

		}

		#region "プロパティ"
		/// <summary>注文ID</summary>
		public string OrderId { get; set; }
		/// <summary>注文者区分</summary>
		public string OwnerKbn { get; set; }
		/// <summary>注文者氏名</summary>
		public string OwnerName { get; set; }
		/// <summary>注文者氏名1</summary>
		public string OwnerName1 { get; set; }
		/// <summary>注文者氏名2</summary>
		public string OwnerName2 { get; set; }
		/// <summary>注文者氏名かな</summary>
		public string OwnerNameKana { get; set; }
		/// <summary>注文者氏名かな1</summary>
		public string OwnerNameKana1 { get; set; }
		/// <summary>注文者氏名かな2</summary>
		public string OwnerNameKana2 { get; set; }
		/// <summary>メールアドレス</summary>
		public string OwnerMailAddr { get; set; }
		/// <summary>モバイルメールアドレス</summary>
		public string OwnerMailAddr2 { get; set; }
		/// <summary>住所国ISOコード</summary>
		public string OwnerAddrCountryIsoCode { get; set; }
		/// <summary>国名</summary>
		public string OwnerAddrCountryName { get; set; }
		/// <summary>郵便番号</summary>
		public string OwnerZip { get; set; }
		/// <summary>住所1</summary>
		public string OwnerAddr1 { get; set; }
		/// <summary>住所2</summary>
		public string OwnerAddr2 { get; set; }
		/// <summary>住所3</summary>
		public string OwnerAddr3 { get; set; }
		/// <summary>住所4</summary>
		public string OwnerAddr4 { get; set; }
		/// <summary>住所5</summary>
		public string OwnerAddr5 { get; set; }
		/// <summary>企業名</summary>
		public string OwnerCompanyName { get; set; }
		/// <summary>部署名</summary>
		public string OwnerCompanyPostName { get; set; }
		/// <summary>電話番号1</summary>
		public string OwnerTel1 { get; set; }
		/// <summary>Telephone 2 for owner</summary>
		public string OwnerTel2 { get; set; }
		/// <summary>性別</summary>
		public string OwnerSex { get; set; }
		/// <summary>生年月日</summary>
		public string OwnerBirth { get; set; }
		/// <summary>生年月日（表示用）</summary>
		public string OwnerBirthDisp { get; set; }
		/// <summary>作成日</summary>
		public string DateCreated { get; set; }
		/// <summary>更新日</summary>
		public string DateChanged { get; set; }
		/// <summary>ユーザー特記欄</summary>
		public string UserMemo { get; set; }
		/// <summary>ユーザー管理レベルID</summary>
		public string UserManagementLevelId { get; set; }
		/// <summary>アクセス国ISOコード</summary>
		public string AccessCountryIsoCode { get; set; }
		/// <summary>表示言語コード</summary>
		public string DispLanguageCode { get; set; }
		/// <summary>表示言語ロケールID</summary>
		public string DispLanguageLocaleId { get; set; }
		/// <summary>表示通貨コード</summary>
		public string DispCurrencyCode { get; set; }
		/// <summary>表示通貨ロケールID</summary>
		public string DispCurrencyLocaleId { get; set; }
		#endregion
	}
}