/*
=========================================================================================================
  Module      : つくーるAPI連携 リクエストデータ(UreruAdRequestData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Web;

namespace w2.Commerce.Batch.ExternalOrderImport.Entity
{
	/// <summary>
	/// つくーるAPI連携 リクエストデータ
	/// </summary>
	public class UreruAdRequestData : IHttpApiRequestData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UreruAdRequestData()
		{
		}

		/// <summary>
		/// POSTデータ生成
		/// </summary>
		/// <returns>POSTデータ</returns>
		public string CreatePostString()
		{
			var parameters = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("account", this.Account),
				new KeyValuePair<string, string>("pass", this.Pass),
				new KeyValuePair<string, string>("key", this.Key),
				new KeyValuePair<string, string>("start_datetime", this.StartDatetime.ToString(Constants.URERU_AD_IMPORT_REQUEST_DATEFORMAT)),
				new KeyValuePair<string, string>("end_datetime", this.EndDatetime.ToString(Constants.URERU_AD_IMPORT_REQUEST_DATEFORMAT)),
				new KeyValuePair<string, string>("fields", CreateRequestFields())
			};
			var postString = string.Join("&",
				parameters.Select(param =>
					string.Format("{0}={1}", param.Key, param.Value)));
			return postString;
		}

		/// <summary>
		/// 取得フィールド指定文字列生成
		/// </summary>
		/// <returns>取得フィールド指定文字列</returns>
		private string CreateRequestFields()
		{
			var fields = new []
			{
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_NAME,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_FAMILY_NAME,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_GIVEN_NAME,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_KANA,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_FAMILY_KANA,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_GIVEN_KANA,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_KATAKANA,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_FAMILY_KATAKANA,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_GIVEN_KATAKANA,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_SEX,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_BIRTHDAY,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_ZIP_FULL_HYPHEN,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_ZIP1,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_ZIP2,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_ADDRESS_FULL,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_PREFECTURE,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_ADDRESS1_ZENKAKU,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_ADDRESS2_ZENKAKU,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_ADDRESS3_ZENKAKU,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_TEL_NO_FULL,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_TEL_NO_FULL_HYPHEN,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_TEL_NO1,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_TEL_NO2,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_TEL_NO3,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_EMAIL,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_PAYMENT_METHOD,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_GMO_ORDER_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_GMO_ACCESS_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_GMO_ACCESS_PASS,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_GMO_MEMBER_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_ZEUS_ORDD,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_ZEUS_SENDID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_VERITRANS_ACCESS_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SONY_PAYMENT_PROCESS_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SONY_PAYMENT_PROCESS_PASS,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SONY_PAYMENT_KAIIN_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SONY_PAYMENT_KAIIN_PASS,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_KURONEKO_WEB_COLLECT_CRD_C_RES_CD,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_KURONEKO_WEB_COLLECT_ORDER_NO,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_KURONEKO_WEB_COLLECT_MEMBER_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_KURONEKO_WEB_COLLECT_AUTHENTICATION_KEY,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_ORDER_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_AUTH_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_AUTH_REF_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_NAME,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_POSTAL_CODE,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_ADDRESS_LINE1,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_ADDRESS_LINE2,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_ADDRESS_LINE3,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_NP_AUTHORIZE_RESULT,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_SHOP_TRANSACTION_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_NP_TRANSACTION_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_MAIL_OPTIN_FLG,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREATED,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_TYPE,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_TOTAL_INC,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_PRODUCT_TOTAL_INC,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_DISCOUNT,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_COMMISSION,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_SHIPPING_COST,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_LANDING_PRODUCT_CODE,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_LANDING_PRODUCT_NAME,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_LANDING_PRODUCT_PRICE_INC,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_LANDING_PRODUCT_QTY,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_LANDING_PRODUCT_RECURRING_FLG,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_UPSELL_PRODUCT_CODE,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_UPSELL_PRODUCT_NAME,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_UPSELL_PRODUCT_PRICE_INC,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_UPSELL_PRODUCT_QTY,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_UPSELL_PRODUCT_RECURRING_FLG,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_QUERY_STRING,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_IP_ADDRESS,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_USER_AGENT,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_NOTE,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SOFT_BANK_PAYMENT_CUSTOMER_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SOFT_BANK_PAYMENT_ORDER_ID,
				Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SOFT_BANK_PAYMENT_TRACKING_ID,
			};
			var fieldString = string.Join(",", fields);
			return fieldString;
		}

		/// <summary>アカウントID</summary>
		public string Account { get { return Constants.URERU_AD_IMPORT_ACCOUNT; } }
		/// <summary>API用パスワード</summary>
		public string Pass { get { return Constants.URERU_AD_IMPORT_PASS; } }
		/// <summary>API用アクセスキー</summary>
		public string Key { get { return Constants.URERU_AD_IMPORT_KEY; } }
		/// <summary>受注日時From</summary>
		public DateTime StartDatetime { get; set; }
		/// <summary>受注日時To</summary>
		public DateTime EndDatetime { get; set; }
	}
}
