/*
=========================================================================================================
  Module      : カート配送先情報クラス(CartShipping.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using w2.App.Common.Global;
using w2.App.Common.Option;
using w2.App.Common.Order.Cart;
using w2.App.Common.RealShop;
using w2.App.Common.Util;
using w2.App.Common.Web.Page;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.GlobalShipping;
using w2.Domain.MemberRank;
using w2.Domain.Order;
using w2.Domain.RealShop;
using w2.Domain.ShopShipping;
using w2.Domain.User.Helper;
using w2.Domain.UserShipping;

namespace w2.App.Common.Order
{
	///*********************************************************************************************
	/// <summary>
	/// カート配送先情報クラス
	/// </summary>
	///*********************************************************************************************
	[Serializable]
	public class CartShipping
	{
		public const string FIELD_ORDERSHIPPING_SHIPPING_ZIP_1 = Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP + "_1";
		public const string FIELD_ORDERSHIPPING_SHIPPING_ZIP_2 = Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP + "_2";
		public const string FIELD_ORDERSHIPPING_SHIPPING_TEL1_1 = Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_1";
		public const string FIELD_ORDERSHIPPING_SHIPPING_TEL1_2 = Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_2";
		public const string FIELD_ORDERSHIPPING_SHIPPING_TEL1_3 = Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_3";
		public const string FIELD_ORDERSHIPPING_SHIPPING_ADDR5_US = Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5 + "_us";
		public const string FIELD_ORDERSHIPPING_SENDER_ZIP_1 = Constants.FIELD_ORDERSHIPPING_SENDER_ZIP + "_1";
		public const string FIELD_ORDERSHIPPING_SENDER_ZIP_2 = Constants.FIELD_ORDERSHIPPING_SENDER_ZIP + "_2";
		public const string FIELD_ORDERSHIPPING_SENDER_TEL1_1 = Constants.FIELD_ORDERSHIPPING_SENDER_TEL1 + "_1";
		public const string FIELD_ORDERSHIPPING_SENDER_TEL1_2 = Constants.FIELD_ORDERSHIPPING_SENDER_TEL1 + "_2";
		public const string FIELD_ORDERSHIPPING_SENDER_TEL1_3 = Constants.FIELD_ORDERSHIPPING_SENDER_TEL1 + "_3";
		public const string FIELD_ORDERSHIPPING_SENDER_ADDR5_US = Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5 + "_us";

		public const string FIELD_ORDERSHIPPING_IS_SAME_SHIPPING_AS_CART1 = "is_same_shippng_as_cart1";	// 配送先表示区分
		public const string FIELD_ORDERSHIPPING_DELIVERY_TO_OWNER = "delivery_to_owner";				// 配送先選択区分
		public const string FIELD_SHOPSHIPPING_SHIPPING_TIME_MESSAGE = "shipping_time_message";			// 配送希望時間帯文言
		public const string FIELD_USERSHIPPING_REGIST_FLG = "usershipping_regist_flg";					// アドレス帳保存フラグ

		public const string FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER = "OWNER";
		public const string FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW = "NEW";
		public const string FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE = "CONVENIENCE_STORE";
		/// <summary>Order shipping address kbn: Store pickup</summary>
		public const string FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP = "STORE_PICKUP";
		/// <summary>ログイン時にLPカートに遷移しようとしたとき「０」を渡してしまっている。この時の配送先枝番である。</summary>
		public const int FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN = 0;
		/// <summary>配送料地帯区分最大値</summary>
		public const int MAX_SHIPPING_ZONE_NO = 47;

		// 配送先住所区分
		public enum AddrKbn
		{
			/// <summary>注文者</summary>
			Owner,
			/// <summary>新規入力</summary>
			New
		}

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		/// <param name="cartObject">親カートオブジェクト</param>
		public CartShipping(CartObject cartObject)
		{
			this.CartObject = cartObject;
			this.ShippingAddrKbn = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER;
			this.IsSameShippingAsCart1 = true;
			this.IsGlobalShippingPriceCalcError = false;

			this.SenderAddrKbn = AddrKbn.Owner;
			this.ProductCounts = new List<ProductCount>();
			this.ConvenienceStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;
			this.ConvenienceStoreId = string.Empty;
			this.ShippingReceivingStoreType = string.Empty;
		}

		/// <summary>
		/// カート配送先住所情報更新
		/// </summary>
		/// <param name="cartOwner">注文者</param>
		/// <param name="blIsSameShippingAsCart1">配送先がカート1と等しいか</param>
		public void UpdateShippingAddr(CartOwner cartOwner, bool blIsSameShippingAsCart1)
		{
			UpdateShippingAddr(
				cartOwner.Name1,
				cartOwner.Name2,
				cartOwner.NameKana1,
				cartOwner.NameKana2,
				cartOwner.Zip,
				cartOwner.Zip1,
				cartOwner.Zip2,
				cartOwner.Addr1,
				cartOwner.Addr2,
				cartOwner.Addr3,
				cartOwner.Addr4,
				cartOwner.Addr5,
				cartOwner.AddrCountryIsoCode,
				cartOwner.AddrCountryName,
				cartOwner.CompanyName,
				cartOwner.CompanyPostName,
				cartOwner.Tel1,
				cartOwner.Tel1_1,
				cartOwner.Tel1_2,
				cartOwner.Tel1_3,
				blIsSameShippingAsCart1,
				CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER);
		}
		/// <summary>
		/// カート配送先住所情報更新
		/// </summary>
		/// <param name="shippingData">配送先情報</param>
		/// <param name="blIsSameShippingAsCart1">配送先がカート1と等しいか</param>
		/// <param name="strShippingAddrKbn">配送先区分(NEW、OWNER、またはアドレス帳番号)</param>
		public void UpdateShippingAddr(Hashtable shippingData, bool blIsSameShippingAsCart1, string strShippingAddrKbn)
		{
			var tel1 = new string[3];
			if (shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1] == DBNull.Value
				|| shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1] == null)
			{
				tel1[0] = (string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_1"];
				tel1[1] = (string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_2"];
				tel1[2] = (string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_3"];
			}
			else
			{
				tel1 = ((string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1]).Split('-');
			}
			var zip = new string[2];
			if (shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP] == DBNull.Value
				|| shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP] == null)
			{
				zip[0] = (string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP + "_1"];
				zip[1] = (string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP + "_2"];
			}
			else
			{
				zip = ((string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP]).Split('-');
			}
			UpdateShippingAddr(
				(string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1],
				(string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2],
				(string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1],
				(string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2],
				(string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP],
				zip.Length > 0 ? zip[0] : string.Empty,
				zip.Length > 1 ? zip[1] : string.Empty,
				(string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1],
				(string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2],
				(string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3],
				(string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4],
				(string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5],
				(string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE],
				(string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME],
				(string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME],
				(string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME],
				(string)shippingData[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1],
				tel1.Length > 0 ? tel1[0] : string.Empty,
				tel1.Length > 1 ? tel1[1] : string.Empty,
				tel1.Length > 2 ? tel1[2] : string.Empty,
				blIsSameShippingAsCart1,
				strShippingAddrKbn);
		}
		/// <summary>
		/// カート配送先住所情報更新
		/// </summary>
		/// <param name="cartShipping">配送先</param>
		public void UpdateShippingAddr(CartShipping cartShipping)
		{
			UpdateShippingAddr(
				cartShipping.Name1,
				cartShipping.Name2,
				cartShipping.NameKana1,
				cartShipping.NameKana2,
				cartShipping.Zip,
				cartShipping.Zip1,
				cartShipping.Zip2,
				cartShipping.Addr1,
				cartShipping.Addr2,
				cartShipping.Addr3,
				cartShipping.Addr4,
				cartShipping.Addr5,
				cartShipping.ShippingCountryIsoCode,
				cartShipping.ShippingCountryName,
				cartShipping.CompanyName,
				cartShipping.CompanyPostName,
				cartShipping.Tel1,
				cartShipping.Tel1_1,
				cartShipping.Tel1_2,
				cartShipping.Tel1_3,
				cartShipping.IsSameShippingAsCart1,
				cartShipping.ShippingAddrKbn);

			this.UserInvoiceRegistFlg = cartShipping.UserInvoiceRegistFlg;
			this.UniformInvoiceType = cartShipping.UniformInvoiceType;
			this.UniformInvoiceTypeOption = cartShipping.UniformInvoiceTypeOption;
			this.UniformInvoiceOption1 = cartShipping.UniformInvoiceOption1;
			this.UniformInvoiceOption2 = cartShipping.UniformInvoiceOption2;
			this.InvoiceName = cartShipping.InvoiceName;
			this.CarryType = cartShipping.CarryType;
			this.CarryTypeOption = cartShipping.CarryTypeOption;
			this.CarryTypeOptionValue = cartShipping.CarryTypeOptionValue;
			this.ConvenienceStoreFlg = cartShipping.ConvenienceStoreFlg;
			this.ConvenienceStoreId = cartShipping.ConvenienceStoreId;
			this.ShippingReceivingStoreType = cartShipping.ShippingReceivingStoreType;
			this.RealShopAreaId = cartShipping.RealShopAreaId;
			this.RealShopId = cartShipping.RealShopId;
			this.RealShopName = cartShipping.RealShopName;
			this.RealShopOpenningHours = cartShipping.RealShopOpenningHours;
		}
		/// <summary>
		/// カート配送先住所情報更新
		/// </summary>
		/// <param name="fixedPurchaseShipping">定期配送先情報</param>
		public void UpdateShippingAddr(FixedPurchaseShippingModel fixedPurchaseShipping)
		{
			var shippingZip1 = string.Empty;
			var shippingZip2 = string.Empty;
			if (GlobalAddressUtil.IsCountryJp(fixedPurchaseShipping.ShippingCountryIsoCode))
			{
				var shippingZip = (fixedPurchaseShipping.ShippingZip + "-").Split('-');
				shippingZip1 = shippingZip[0];
				shippingZip2 = shippingZip[1];
			}

			var shippingTel1_1 = string.Empty;
			var shippingTel1_2 = string.Empty;
			var shippingTel1_3 = string.Empty;
			if (GlobalAddressUtil.IsCountryJp(fixedPurchaseShipping.ShippingCountryIsoCode))
			{
				var shippingTel1 = (fixedPurchaseShipping.ShippingTel1 + "--").Split('-');
				shippingTel1_1 = shippingTel1[0];
				shippingTel1_2 = shippingTel1[1];
				shippingTel1_3 = shippingTel1[2];
			}

			UpdateShippingAddr(
				fixedPurchaseShipping.ShippingName1,
				fixedPurchaseShipping.ShippingName2,
				fixedPurchaseShipping.ShippingNameKana1,
				fixedPurchaseShipping.ShippingNameKana2,
				fixedPurchaseShipping.ShippingZip,
				shippingZip1,
				shippingZip2,
				fixedPurchaseShipping.ShippingAddr1,
				fixedPurchaseShipping.ShippingAddr2,
				fixedPurchaseShipping.ShippingAddr3,
				fixedPurchaseShipping.ShippingAddr4,
				fixedPurchaseShipping.ShippingAddr5,
				fixedPurchaseShipping.ShippingCountryIsoCode,
				fixedPurchaseShipping.ShippingCountryName,
				fixedPurchaseShipping.ShippingCompanyName,
				fixedPurchaseShipping.ShippingCompanyPostName,
				fixedPurchaseShipping.ShippingTel1,
				shippingTel1_1,
				shippingTel1_2,
				shippingTel1_3,
				this.IsSameShippingAsCart1,
				this.ShippingAddrKbn);
		}
		/// <summary>
		/// カート配送先住所情報更新
		/// </summary>
		/// <param name="strName1">配送先氏名1</param>
		/// <param name="strName2">配送先氏名2</param>
		/// <param name="strNameKana1">配送先氏名かな1</param>
		/// <param name="strNameKana2">配送先氏名かな2</param>
		/// <param name="zip">配送先郵便番号</param>
		/// <param name="strZip1">配送先郵便番号1</param>
		/// <param name="strZip2">配送先郵便番号2</param>
		/// <param name="strAddr1">配送先住所1</param>
		/// <param name="strAddr2">配送先住所2</param>
		/// <param name="strAddr3">配送先住所3</param>
		/// <param name="strAddr4">配送先住所4</param>
		/// <param name="addr5">配送先住所5</param>
		/// <param name="shippingCountryName">配送先国名</param>
		/// <param name="strCompanyName">配送先企業名</param>
		/// <param name="strCompanyPostName">配送先部署名</param>
		/// <param name="tel1">配送先電話番号</param>
		/// <param name="strTel1_1">配送先電話番号1</param>
		/// <param name="strTel1_2">配送先電話番号2</param>
		/// <param name="strTel1_3">配送先電話番号3</param>
		/// <param name="blIsSameShippingAsCart1">配送先がカート1と等しいか</param>
		/// <param name="strShippingAddrKbn">配送先区分(NEW、OWNER、またはアドレス帳番号)</param>
		/// <param name="realShopId">Real shop id</param>
		public void UpdateShippingAddr(
			string strName1,
			string strName2,
			string strNameKana1,
			string strNameKana2,
			string zip,
			string strZip1,
			string strZip2,
			string strAddr1,
			string strAddr2,
			string strAddr3,
			string strAddr4,
			string addr5,
			string shippingCountryIsoCode,
			string shippingCountryName,
			string strCompanyName,
			string strCompanyPostName,
			string tel1,
			string strTel1_1,
			string strTel1_2,
			string strTel1_3,
			bool blIsSameShippingAsCart1,
			string strShippingAddrKbn,
			string realShopId = "")
		{
			this.Name1 = strName1;
			this.Name2 = strName2;
			this.NameKana1 = strNameKana1;
			this.NameKana2 = strNameKana2;
			this.Zip = zip;
			this.Zip1 = strZip1;
			this.Zip2 = strZip2;
			this.Addr1 = strAddr1;
			this.Addr2 = strAddr2;
			this.Addr3 = strAddr3;
			this.Addr4 = strAddr4;
			this.Addr5 = addr5;
			this.ShippingCountryIsoCode = shippingCountryIsoCode;
			this.ShippingCountryName = shippingCountryName;
			this.CompanyName = strCompanyName;
			this.CompanyPostName = strCompanyPostName;
			this.Tel1 = tel1;
			this.Tel1_1 = strTel1_1;
			this.Tel1_2 = strTel1_2;
			this.Tel1_3 = strTel1_3;
			this.IsSameShippingAsCart1 = blIsSameShippingAsCart1;
			this.ShippingAddrKbn = strShippingAddrKbn;

			this.RealShopId = string.Empty;
			this.RealShopName = string.Empty;
			this.RealShopAreaId = string.Empty;
			this.RealShopOpenningHours = string.Empty;

			if (string.IsNullOrEmpty(realShopId) == false)
			{
				var realShop = new RealShopService().Get(realShopId);
				if (realShop != null)
				{
					this.RealShopId = realShopId;
					this.RealShopName = realShop.Name;
					this.RealShopAreaId = realShop.AreaId;
					this.RealShopOpenningHours = realShop.OpeningHours;
				}
			}
		}

		/// <summary>
		/// カート配送料金更新
		/// </summary>
		/// <param name="priceShipping">配送料金</param>
		public void UpdateCartPriceShipping(decimal priceShipping)
		{
			this.PriceShipping = priceShipping;
		}

		/// <summary>
		/// カート配送日時更新
		/// </summary>
		/// <param name="blSpecifyShippingDateFlg">配送日指定フラグ</param>
		/// <param name="blSpecifyShippingTimeFlg">配送時間帯指定フラグ</param>
		/// <param name="dtShippingDate">配送日</param>
		/// <param name="strShippingTime">配送時間帯(コード)</param>
		/// <param name="strShippingTimeMessage">配送時間帯(文言)</param>
		public void UpdateShippingDateTime(
			bool blSpecifyShippingDateFlg,
			bool blSpecifyShippingTimeFlg,
			DateTime? dtShippingDate,
			string strShippingTime,
			string strShippingTimeMessage)
		{
			this.SpecifyShippingDateFlg = blSpecifyShippingDateFlg;
			this.SpecifyShippingTimeFlg = blSpecifyShippingTimeFlg;
			this.ShippingDate = this.IsExpress ? dtShippingDate : null;
			this.ShippingDateForCalculation = dtShippingDate;
			this.ShippingTime = strShippingTime;
			this.ShippingTimeMessage = strShippingTimeMessage;
		}

		/// <summary>
		/// 定期購入情報更新
		/// </summary>
		/// <param name="strFixedPurchaseKbn">定期購入区分</param>
		/// <param name="strFixedPurchaseSetting">定期購入設定</param>
		/// <param name="daysRequired">定期購入配送所要日数</param>
		/// <param name="minSpan">定期購入最低配送間隔</param>
		public void UpdateFixedPurchaseSetting(
			string strFixedPurchaseKbn,
			string strFixedPurchaseSetting,
			int daysRequired,
			int minSpan)
		{
			this.FixedPurchaseKbn = strFixedPurchaseKbn;
			this.FixedPurchaseSetting = strFixedPurchaseSetting;
			this.FixedPurchaseDaysRequired = daysRequired;
			this.FixedPurchaseMinSpan = minSpan;
		}

		/// <summary>
		/// 定期購入次回/次々回配送日更新
		/// </summary>
		/// <param name="nextShippingDate">次回配送日</param>
		/// <param name="nextNextShippingDate">次々回配送日</param>
		public void UpdateNextShippingDates(
			DateTime nextShippingDate,
			DateTime nextNextShippingDate)
		{
			this.NextShippingDate = nextShippingDate;
			this.NextNextShippingDate = nextNextShippingDate;
		}

		/// <summary>
		/// カート配送先アドレス帳保存設定更新
		/// </summary>
		/// <param name="blUserShippingRegistFlg">アドレス帳登録有無</param>
		/// <param name="strUserShippingName">アドレス帳名（保存用）</param>
		public void UpdateUserShippingRegistSetting(
			bool? blUserShippingRegistFlg,
			string strUserShippingName)
		{
			// アドレス帳保存フラグ
			this.UserShippingRegistFlg = (blUserShippingRegistFlg.HasValue) ? blUserShippingRegistFlg.Value : false;

			// アドレス帳名称
			this.UserShippingName = strUserShippingName;
		}

		/// <summary>
		/// 配送希望日取得
		/// </summary>
		/// <param name="isSendOperator">オペレータ向けか</param>
		/// <returns>配送希望日</returns>
		public string GetShippingDate(bool isSendOperator = false)
		{
			// 配送希望日入力可 & 配送希望日指定済みの場合
			if (this.SpecifyShippingDateFlg && this.ShippingDate.HasValue)
			{
				// 言語ロケールIDを取得
				var localeId = (Constants.GLOBAL_OPTION_ENABLE == false)
					? string.Empty
					: (isSendOperator
						? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
						: this.CartObject.Owner.DispLanguageLocaleId);
				// 該当の日付形式へ変換
				var result = DateTimeUtility.ToString(
					this.ShippingDate,
					DateTimeUtility.FormatType.LongDateWeekOfDay1Letter,
					localeId);
				return result;
			}
			else
			{
				return CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@");	// デザイン文言なのでコードと分離したいがメール文言でも利用したいためここで指定する
			}
		}

		/// <summary>
		/// 配送希望時間帯取得
		/// </summary>
		/// <param name="csShipping">配送方法情報/param>
		/// <returns>配送希望時間帯</returns>
		public string GetShippingTime()
		{
			// 配送希望時間帯入力可 & 配送希望時間帯指定済みの場合
			if (this.SpecifyShippingTimeFlg && ((this.ShippingTimeMessage != null) && (this.ShippingTimeMessage != "")))
			{
				return this.ShippingTimeMessage;
			}
			else
			{
				return "指定なし";	// デザイン文言なのでコードと分離したいがメール文言でも利用したいためここで指定する
			}
		}

		/// <summary>
		/// 送り主住所情報更新
		/// </summary>
		/// <param name="cartShipping">カート配送先</param>
		/// <param name="blIsSameSenderAsCart1">送り主がカート1と等しいか</param>
		public void UpdateSenderAddr(CartShipping cartShipping, bool blIsSameSenderAsCart1)
		{
			UpdateSenderAddr(
				cartShipping.SenderName1,
				cartShipping.SenderName2,
				cartShipping.SenderNameKana1,
				cartShipping.SenderNameKana2,
				cartShipping.SenderZip,
				cartShipping.SenderZip1,
				cartShipping.SenderZip2,
				cartShipping.SenderAddr1,
				cartShipping.SenderAddr2,
				cartShipping.SenderAddr3,
				cartShipping.SenderAddr4,
				cartShipping.SenderAddr5,
				cartShipping.SenderCountryIsoCode,
				cartShipping.SenderCountryName,
				cartShipping.SenderCompanyName,
				cartShipping.SenderCompanyPostName,
				cartShipping.SenderTel1,
				cartShipping.SenderTel1_1,
				cartShipping.SenderTel1_2,
				cartShipping.SenderTel1_3,
				blIsSameSenderAsCart1,
				cartShipping.SenderAddrKbn);
		}
		/// <summary>
		/// 送り主住所情報更新
		/// </summary>
		/// <param name="cartOwner">カート注文者</param>
		/// <param name="blIsSameSenderAsCart1">送り主がカート1と等しいか</param>
		public void UpdateSenderAddr(CartOwner cartOwner, bool blIsSameSenderAsCart1)
		{
			UpdateSenderAddr(
				cartOwner.Name1,
				cartOwner.Name2,
				cartOwner.NameKana1,
				cartOwner.NameKana2,
				cartOwner.Zip,
				cartOwner.Zip1,
				cartOwner.Zip2,
				cartOwner.Addr1,
				cartOwner.Addr2,
				cartOwner.Addr3,
				cartOwner.Addr4,
				cartOwner.Addr5,
				cartOwner.AddrCountryIsoCode,
				cartOwner.AddrCountryName,
				cartOwner.CompanyName,
				cartOwner.CompanyPostName,
				cartOwner.Tel1,
				cartOwner.Tel1_1,
				cartOwner.Tel1_2,
				cartOwner.Tel1_3,
				blIsSameSenderAsCart1,
				AddrKbn.Owner);
		}
		/// <summary>
		/// 送り主住所情報更新
		/// </summary>
		/// <param name="name1">氏名1</param>
		/// <param name="name2">氏名2</param>
		/// <param name="nameKana1">氏名かな1</param>
		/// <param name="nameKana2">氏名かな2</param>
		/// <param name="zip">郵便番号</param>
		/// <param name="zip1">郵便番号1</param>
		/// <param name="zip2">郵便番号2</param>
		/// <param name="addr1">住所1</param>
		/// <param name="addr2">住所2</param>
		/// <param name="addr3">住所3</param>
		/// <param name="addr4">住所4</param>
		/// <param name="addr5">住所5</param>
		/// <param name="addrCountryIsoCode">住所国ISOコード</param>
		/// <param name="addrCountryName">住所国名</param>
		/// <param name="companyName">企業名</param>
		/// <param name="companyPostName">部署名</param>
		/// <param name="tel1">電話番号1</param>
		/// <param name="strTel1_1">電話番号1-1</param>
		/// <param name="strTel1_2">電話番号1-2</param>
		/// <param name="strTel1_3">電話番号1-3</param>
		/// <param name="blIsSameSenderAsCart1">送り主がカート1と等しいか</param>
		/// <param name="akSenderAddrKbn">区分(NEW、OWNER)</param>
		public void UpdateSenderAddr(
			string name1,
			string name2,
			string nameKana1,
			string nameKana2,
			string zip,
			string zip1,
			string zip2,
			string addr1,
			string addr2,
			string addr3,
			string addr4,
			string addr5,
			string addrCountryIsoCode,
			string addrCountryName,
			string companyName,
			string companyPostName,
			string tel1,
			string strTel1_1,
			string strTel1_2,
			string strTel1_3,
			bool blIsSameSenderAsCart1,
			AddrKbn akSenderAddrKbn)
		{
			this.SenderName1 = name1;
			this.SenderName2 = name2;
			this.SenderNameKana1 = nameKana1;
			this.SenderNameKana2 = nameKana2;
			this.SenderZip = zip;
			this.SenderZip1 = zip1;
			this.SenderZip2 = zip2;
			this.SenderAddr1 = addr1;
			this.SenderAddr2 = addr2;
			this.SenderAddr3 = addr3;
			this.SenderAddr4 = addr4;
			this.SenderAddr5 = addr5;
			this.SenderCountryIsoCode = addrCountryIsoCode;
			this.SenderCountryName = addrCountryName;
			this.SenderCompanyName = companyName;
			this.SenderCompanyPostName = companyPostName;
			this.SenderTel1 = tel1;
			this.SenderTel1_1 = strTel1_1;
			this.SenderTel1_2 = strTel1_2;
			this.SenderTel1_3 = strTel1_3;
			this.IsSameSenderAsShipping1 = blIsSameSenderAsCart1;
			this.SenderAddrKbn = akSenderAddrKbn;
		}

		/// <summary>
		/// 定期購入配送パターン文字列取得
		/// </summary>
		/// <returns>配送パターン文字列</returns>
		public string GetFixedPurchaseShippingPatternString()
		{
			if (string.IsNullOrEmpty(this.FixedPurchaseKbn)) return string.Empty;
			return OrderCommon.CreateFixedPurchaseSettingMessage(this.FixedPurchaseKbn, this.FixedPurchaseSetting);
		}

		/// <summary>
		/// 定期購入初回配送日取得
		/// </summary>
		/// <returns>初回配送日</returns>
		public DateTime GetFirstShippingDate()
		{
			if (string.IsNullOrEmpty(this.FixedPurchaseKbn)) return DateTime.MinValue;

			var firstShippingDate = OrderCommon.GetFirstShippingDateBasedOnToday(
				this.CartObject.ShopId,
				this.FixedPurchaseDaysRequired,
				this.ShippingDate,
				this.ShippingMethod,
				this.DeliveryCompanyId,
				this.ShippingCountryIsoCode,
				this.IsTaiwanCountryShippingEnable
					? this.Addr2
					: this.Addr1,
				this.Zip);
			return firstShippingDate;
		}

		/// <summary>
		/// 配送料金設定
		/// </summary>
		/// <param name="shippingInfo">配送種別情報</param>
		public void SetShippingPrice(ShopShippingModel shippingInfo)
		{
			// 日本国内かそれともGlobalかを判定して計算
			if (this.UseGlobalShippingPrice)
			{
				SetShippingGlobal(shippingInfo);
			}
			else
			{
				// 日本国内として計算
				SetShippingJapanDomestic(shippingInfo);
			}
		}

		/// <summary>
		/// Set shipping Japan domestic
		/// </summary>
		/// <param name="shipping">Shipping information</param>
		private void SetShippingJapanDomestic(ShopShippingModel shipping)
		{
			SetShippingDomesticInner(shipping);
		}

		/// <summary>
		/// Set shipping Taiwan domestic
		/// </summary>
		/// <param name="shipping">Shipping information</param>
		private void SetShippingTaiwanDomestic(ShopShippingModel shipping)
		{
			SetShippingDomesticInner(shipping);
		}

		/// <summary>
		/// Set shipping domestic inner
		/// </summary>
		/// <param name="shippingInfo">配送種別情報</param>
		private void SetShippingDomesticInner(ShopShippingModel shippingInfo)
		{
			//------------------------------------------------------
			// 配送種別情報取得・設定
			//------------------------------------------------------
			// プロパティに配送種別情報を反映
			SetShippingInfo(shippingInfo);

			var dShippingPriceTotal = 0m;	// 配送料金合計
			var iProductCount = 0;				// 商品数 (複数商品購入特別設定用)
			var lShippingPrice = new List<decimal>(); // 商品毎の配送料金(複数商品購入特別設定用)
			//------------------------------------------------------
			// 配送種別情報取得
			//------------------------------------------------------
			// まとめる
			List<ProductCount> lProductCount;
			if ((this.CartObject.IsGift)
				&& (this.CartObject.Shippings.Count > 1))
			{
				lProductCount = this.ProductCounts
					.Where(pc => pc.Product.IsBundle == false)
					.ToList();
			}
			else
			{
				lProductCount = this.CartObject.Items
					.Where(cp => cp.IsBundle == false)
					.Select(cp => new ProductCount(cp, cp.Count))
					.ToList();
			}

			// 配送料計算
			foreach (var productCount in lProductCount)
			{
				for (var iLoop = 0; iLoop < productCount.Count; iLoop++)	// 商品数 x セット数分だけループ
				{
					var dShippingPrice = GetSizeKbnShippingPrice(
						productCount.Product.ShippingSizeKbn,
						this.IsExpress,
						this.ShippingPriceOfSizeXXS,
						this.ShippingPriceOfSizeXS,
						this.ShippingPriceOfSizeS,
						this.ShippingPriceOfSizeM,
						this.ShippingPriceOfSizeL,
						this.ShippingPriceOfSizeXL,
						this.ShippingPriceOfSizeXXL,
						this.ShippingPriceOfSizeMail);

					// 配送料金を決定するため、商品ごとの配送料金を保持
					lShippingPrice.Add(dShippingPrice);

					// 商品数更新 (複数商品購入特別設定用)
					iProductCount++;

					// 配送料金合計加算（）
					if ((productCount.Product.IsPluralShippingPriceFree == Constants.FLG_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG_INVALID)
						|| (iLoop == 0))
					{
						dShippingPriceTotal += dShippingPrice;
					}
				}
			}

			// 優先配送料金更新(複数商品購入特別設定用)
			var dPreferableShippingPrice = GetPreferableShippingPrice(lShippingPrice);

			// 配送料金特殊設定関連
			dShippingPriceTotal = SetSpecialShippingPrice(
				dPreferableShippingPrice,
				dShippingPriceTotal,
				this.ShippingFreePriceFlg,
				this.ShippingFreePrice,
				this.ShippingZoneNo,
				this.PriceSubtotal,
				this.CalculationPluralKbn,
				this.PluralShippingPrice,
				iProductCount,
				this.ShippingPriceSeparateEstimateFlg,
				this.CartObject.HasFixedPurchase,
				this.FixedPurchaseFreeShippingFlg,
				this.CartObject.SetPromotions.ProductDiscountAmount,
				this.CartObject.MemberRankDiscount,
				this.CartObject.FixedPurchaseMemberDiscountAmount,
				this.CartObject.FixedPurchaseDiscount,
				this.CartObject.UseCouponPrice,
				this.CartObject.MemberRankId,
				this.ConditionalShippingPriceThreshold,
				this.ConditionalShippingPrice);

			this.PriceShipping = dShippingPriceTotal;
		}

		/// <summary>
		/// 海外向けとして送料セット
		/// </summary>
		/// <param name="shippingInfo">配送種別情報</param>
		private void SetShippingGlobal(ShopShippingModel shippingInfo)
		{
			// プロパティに配送種別情報を反映
			SetShippingInfo(shippingInfo);

			if (this.IsShippingStorePickup
				&& (this.ShippingFreePriceFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_VALID))
			{
				this.PriceShipping = 0;
			}

			// Calculate shipping price for taiwan domestic
			if (this.IsTaiwanCountryShippingEnable
				|| (Constants.TW_COUNTRY_SHIPPING_ENABLE
					&& string.IsNullOrEmpty(this.ShippingCountryIsoCode)))
			{
				SetShippingTaiwanDomestic(shippingInfo);
				return;
			}

			// 国の指定がなければ送料計算せずスルー
			if (string.IsNullOrEmpty(this.ShippingCountryIsoCode)
				|| (this.IsShippingConvenience && string.IsNullOrEmpty(this.ConvenienceStoreId)))
			{
				return;
			}

			// カート商品の重量
			var weight = this.CartObject.Items.Sum(p => p.WeightSubTotal);

			// 住所からエリアを特定
			var sv = DomainFacade.Instance.GlobalShippingService;
			var area = sv.DistributesShippingArea(this.ShippingCountryIsoCode, this.Addr5, this.Addr4, this.Addr3, this.Addr2, this.Zip);
			if (area == null)
			{
				// 住所からエリアが特定出来ない場合はログ書いてエラー
				var msg = string.Format("ISO：{0}、ADDR5：{1}、ADDR4：{2}、ADDR3：{3}、ADDR2：{4}、ZIP：{5}、備考：{6}"
					, this.ShippingCountryIsoCode, this.Addr5, this.Addr4, this.Addr3, this.Addr2, this.Zip, "エリア特定失敗");
				FileLogger.WriteError(msg);
				throw new GlobalShippingPriceCalcException(msg);
			}

			// エリアから該当料金表とってくる
			var postageMap = sv.GetAreaPostageByShippingDeliveryCompany(
				this.CartObject.ShopId,
				this.CartObject.ShippingType,
				area.GlobalShippingAreaId,
				this.DeliveryCompanyId);

			// 重量に該当する料金を特定
			// SEQが大きい、一番後に登録されたもの優先
			var postage = postageMap
				.Where(p => p.WeightGramGreaterThanOrEqualTo <= weight && weight <= p.WeightGramLessThan)
				.OrderByDescending(x => x.Seq)
				.ToArray();

			if (postage.Length == 0)
			{
				// 該当料金なければログ書いてエラー
				var msg = string.Format("配送種別：{0}、エリア：{1}、重量：{2}、、備考：{3}"
					, this.CartObject.ShippingType, area.GlobalShippingAreaId, weight, "料金表特定失敗");
				FileLogger.WriteError(msg);
				this.IsGlobalShippingPriceCalcError = true;
				return;
			}

			var shippingPrice = postage.First().GlobalShippingPostage;

			// 配送料金特殊設定関連
			this.PriceShipping = SetSpecialShippingPrice(
				shippingPrice,
				shippingPrice,
				this.ShippingFreePriceFlg,
				this.ShippingFreePrice,
				this.ShippingZoneNo,
				this.PriceSubtotal,
				this.CalculationPluralKbn,
				this.PluralShippingPrice,
				this.CartObject.Items.Sum(p => p.Count),
				this.ShippingPriceSeparateEstimateFlg,
				this.CartObject.HasFixedPurchase,
				this.FixedPurchaseFreeShippingFlg,
				this.CartObject.SetPromotions.ProductDiscountAmount,
				this.CartObject.MemberRankDiscount,
				this.CartObject.FixedPurchaseMemberDiscountAmount,
				this.CartObject.FixedPurchaseDiscount,
				this.CartObject.UseCouponPrice,
				this.CartObject.MemberRankId,
				this.ConditionalShippingPriceThreshold,
				this.ConditionalShippingPrice,
				true);
		}

		/// <summary>
		/// ログを残してエラーページへ遷移
		/// </summary>
		public void TransitionToErrorScreenByGlobalShippingPriceCalcError()
		{
			var weight = this.CartObject.Items.Sum(p => p.WeightSubTotal);
			var sv = new GlobalShippingService();
			var area = sv.DistributesShippingArea(this.ShippingCountryIsoCode, this.Addr5, this.Addr4, this.Addr3, this.Addr2, this.Zip);
			var postageMap = sv.GetAreaPostageByShippingDeliveryCompany(
				this.CartObject.ShopId,
				this.CartObject.ShippingType,
				area.GlobalShippingAreaId,
				this.DeliveryCompanyId);

			// 重量に該当する料金を特定
			// SEQが大きい、一番後に登録されたもの優先
			var postage = postageMap
				.Where(p => ((p.WeightGramGreaterThanOrEqualTo <= weight) && (weight <= p.WeightGramLessThan)))
				.OrderByDescending(x => x.Seq)
				.ToArray();

			if (postage.Length == 0)
			{
				// 該当料金なければログ書いてエラー
				var msg = string.Format("配送種別：{0}、エリア：{1}、重量：{2}、、備考：{3}"
					, this.CartObject.ShippingType, area.GlobalShippingAreaId, weight, "料金表特定失敗");
				FileLogger.WriteError(msg);
				throw new GlobalShippingPriceCalcException(msg);
			}
		}

		/// <summary>
		/// 指定したサイズに応じたサイズ配送料を取得
		/// </summary>
		/// <param name="shippingSizeKbn">配送サイズ区分</param>
		/// <param name="isExpress">宅配便か</param>
		/// <param name="shippingPriceOfSizeXXS">XXSサイズ商品配送料金</param>
		/// <param name="shippingPriceOfSizeXS">XSサイズ商品配送料金</param>
		/// <param name="shippingPriceOfSizeS">Sサイズ商品配送料金</param>
		/// <param name="shippingPriceOfSizeM">Mサイズ商品配送料金</param>
		/// <param name="shippingPriceOfSizeL">Lサイズ商品配送料金</param>
		/// <param name="shippingPriceOfSizeXL">XLサイズ商品配送料金</param>
		/// <param name="shippingPriceOfSizeXXL">XXLサイズ商品配送料金</param>
		/// <param name="shippingPriceOfSizeMail">Mailサイズ商品配送料金</param>
		/// <returns>指定したサイズに応じたサイズ配送料</returns>
		public static decimal GetSizeKbnShippingPrice(
			string shippingSizeKbn,
			bool isExpress,
			decimal shippingPriceOfSizeXXS,
			decimal shippingPriceOfSizeXS,
			decimal shippingPriceOfSizeS,
			decimal shippingPriceOfSizeM,
			decimal shippingPriceOfSizeL,
			decimal shippingPriceOfSizeXL,
			decimal shippingPriceOfSizeXXL,
			decimal shippingPriceOfSizeMail)
		{
			decimal dShippingPrice = 0;
			var sizeKbn = ((shippingSizeKbn == Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL) && isExpress)
				? Constants.SHIPPINGPRICE_SIZE_FOR_EXPRESS
				: shippingSizeKbn;
			switch (sizeKbn)
			{
				case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_XXS:
					dShippingPrice = shippingPriceOfSizeXXS;
					break;

				case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_XS:
					dShippingPrice = shippingPriceOfSizeXS;
					break;

				case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_S:
					dShippingPrice = shippingPriceOfSizeS;
					break;

				case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_M:
					dShippingPrice = shippingPriceOfSizeM;
					break;

				case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_L:
					dShippingPrice = shippingPriceOfSizeL;
					break;

				case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_XL:
					dShippingPrice = shippingPriceOfSizeXL;
					break;

				case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_XXL:
					dShippingPrice = shippingPriceOfSizeXXL;
					break;

				case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL:
					dShippingPrice = shippingPriceOfSizeMail;
					break;
			}
			return dShippingPrice;
		}

		/// <summary>
		/// 優先配送料金の取得(複数商品購入特別設定用)
		/// </summary>
		/// <param name="shippingPriceList">商品ごとの配送料金</param>
		/// <returns>優先配送料金</returns>
		public static decimal GetPreferableShippingPrice(List<decimal> shippingPriceList)
		{
			// 優先配送料金更新(複数商品購入特別設定用)
			decimal dPreferableShippingPrice = 0;
			if (shippingPriceList.Count != 0)
			{
				switch (Constants.SHIPPINGPRIORITY_SETTING)
				{
					case Constants.ShippingPriority.HIGH:
						dPreferableShippingPrice = shippingPriceList.Max();
						break;
					case Constants.ShippingPriority.LOW:
						dPreferableShippingPrice = shippingPriceList.Min();
						break;
				}
			}
			return dPreferableShippingPrice;
		}

		/// <summary>
		/// 配送料金特殊を設定する
		/// </summary>
		/// <param name="preferableShippingPrice">優先配送料金</param>
		/// <param name="shippingPriceTotal">通常商品合計金額</param>
		/// <param name="shippingFreePriceFlg">配送料無料購入金額設定フラグ</param>
		/// <param name="shippingFreePrice">配送料無料購入金額設定</param>
		/// <param name="shippingZoneNo">配送料地帯区分</param>
		/// <param name="priceSubtotal">配送先商品小計</param>
		/// <param name="calculationPluralKbn">複数商品計算区分</param>
		/// <param name="pluralShippingPrice">複数商品配送料</param>
		/// <param name="productCount">商品数</param>
		/// <param name="shippingPriceSeparateEstimateFlg">配送料別途見積もり表示フラグ</param>
		/// <param name="hasFixedPurchase">定期購入あり判定</param>
		/// <param name="fixedPurchaseFreeShippingFlg">定期配送料無料フラグ</param>
		/// <param name="setpromotionProductDiscountAmount">セットプロモーション商品割引額</param>
		/// <param name="memberRankDiscountPrice">会員ランク割引金額</param>
		/// <param name="fixedPurchaseMemberDiscountAmount">定期会員割引金額</param>
		/// <param name="fixedPurchaseDiscountPrice">定期回数割引金額</param>
		/// <param name="useCouponPrice">クーポン割引金額</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="conditionalShippingPriceThreshold">送料特別価格設定閾値</param>
		/// <param name="conditionalShippingPrice">送料特別価格</param>
		/// <param name="useGlobalShippingPrice">海外配送料を使うかどうか。True：使う、False：使わない</param>
		/// <returns></returns>
		public static decimal SetSpecialShippingPrice(
			decimal preferableShippingPrice,
			decimal shippingPriceTotal,
			string shippingFreePriceFlg,
			decimal shippingFreePrice,
			int shippingZoneNo,
			decimal priceSubtotal,
			string calculationPluralKbn,
			decimal pluralShippingPrice,
			int productCount,
			bool shippingPriceSeparateEstimateFlg,
			bool hasFixedPurchase,
			string fixedPurchaseFreeShippingFlg,
			decimal setpromotionProductDiscountAmount,
			decimal memberRankDiscountPrice,
			decimal fixedPurchaseMemberDiscountAmount,
			decimal fixedPurchaseDiscountPrice,
			decimal useCouponPrice,
			string memberRankId,
			decimal? conditionalShippingPriceThreshold,
			decimal? conditionalShippingPrice,
			bool useGlobalShippingPrice = false)
		{
			decimal resultPriceTotal = shippingPriceTotal;

			// 会員ランク配送料割引方法判断用、会員ランク詳細を取得
			var memberRankDetail = MemberRankOptionUtility.GetMemberRankList()
				.FirstOrDefault(memberRank => (memberRank.MemberRankId == memberRankId));
			// 会員ランクオプションがOFFの場合と注文時会員ランクが存在しない場合は初期化生成
			if ((Constants.MEMBER_RANK_OPTION_ENABLED == false) || (memberRankDetail == null))
			{
				memberRankDetail = new MemberRankModel
				{
					ShippingDiscountType = Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE,
					ShippingDiscountValue = null,
				};
			}

			// 配送料無料判断用金額（小計から各種割引を減算）
			var priceSubtotalForFreeJudgement = priceSubtotal - PriceCalculator.GetOrderPriceDiscountTotal(
						memberRankDiscountPrice,
						setpromotionProductDiscountAmount,
				0m, // セットプロモーション配送料割引額は除外
				0m, // セットプロモーション決済手数料割引額は除外
				0m, // ポイント割引額は除外
				0m, // クーポン割引額は除外
						fixedPurchaseMemberDiscountAmount,
				fixedPurchaseDiscountPrice);

			// 配送料無料設定？（ただし特別配送先は除外、海外は除外しない）
			// 会員ランク配送料無料最低金額が設定されている場合は、配送料無料設定を見ない
			if (((shippingFreePriceFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_VALID)
					&& (memberRankDetail.ShippingDiscountType != Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FRESHP_TSD)
					&& (conditionalShippingPriceThreshold == null)
				) && ((shippingZoneNo < 48) || (useGlobalShippingPrice))
				&& (priceSubtotalForFreeJudgement >= shippingFreePrice))
			{
				resultPriceTotal = 0; // 無料
			}
			// 最も優先される送料１点＋x円／個の送料を設定？
			else if ((calculationPluralKbn == Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_HIGHEST_SHIPPING_PRICE_PLUS_PLURAL_PRICE)
				&& (productCount > 1))
			{
				resultPriceTotal = preferableShippingPrice + (productCount - 1) * pluralShippingPrice;
			}

			// 配送先・特別配送先ごと配送料無料最低金額設定を判断
			if ((conditionalShippingPriceThreshold != null)
				&& ((conditionalShippingPrice.Value != 0m)
					|| (memberRankDetail.ShippingDiscountType != Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FRESHP_TSD))
				&& (priceSubtotalForFreeJudgement >= conditionalShippingPriceThreshold))
			{
				resultPriceTotal = conditionalShippingPrice.Value;
			}

			// 配送先の配送料設定
			if (Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED && shippingPriceSeparateEstimateFlg)
			{
				// サイト構成が配送料金別途見積もりフラグ利用 かつ 
				// 配送種別設定が配送料金別途見積もりフラグ有効の場合、配送料金を0円に設定
				resultPriceTotal = 0;
			}

			// 定期配送料無料の場合
			if (hasFixedPurchase && (fixedPurchaseFreeShippingFlg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG_VALID))
			{
				resultPriceTotal = 0;
			}

			// 会員ランク配送料無料最低金額を設定した場合
			if ((memberRankDetail.ShippingDiscountType == Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FRESHP_TSD)
				&& (priceSubtotalForFreeJudgement >= memberRankDetail.ShippingDiscountValue))
			{
				resultPriceTotal = 0;
			}

			return resultPriceTotal;
		}

		/// <summary>
		/// 配送種別情報セット
		/// </summary>
		/// <param name="shippingInfo">配送種別情報</param>
		protected void SetShippingInfo(ShopShippingModel shippingInfo)
		{
			// 配送会社IDプロパティが未設定の場合、一旦配送種別の宅配便のデフォルト配送会社IDを取得
			var deliveryCompanyId = string.IsNullOrEmpty(this.DeliveryCompanyId)
				? shippingInfo.CompanyListExpress.First(c => c.IsDefault).DeliveryCompanyId
				: this.DeliveryCompanyId;

			// 配送会社による配送料マスタ取得
			var postageSetting = shippingInfo.CompanyPostageSettings
				.First(s => (s.DeliveryCompanyId == deliveryCompanyId));
			// 配送会社による配送料地帯情報取得
			var zone = shippingInfo.ZoneList.First(s => (s.DeliveryCompanyId == deliveryCompanyId));

			this.ShippingPriceOfSizeXXS = zone.SizeXxsShippingPrice;
			this.ShippingPriceOfSizeXS = zone.SizeXsShippingPrice;
			this.ShippingPriceOfSizeS = zone.SizeSShippingPrice;
			this.ShippingPriceOfSizeM = zone.SizeMShippingPrice;
			this.ShippingPriceOfSizeL = zone.SizeLShippingPrice;
			this.ShippingPriceOfSizeXL = zone.SizeXlShippingPrice;
			this.ShippingPriceOfSizeXXL = zone.SizeXxlShippingPrice;
			this.ShippingPriceOfSizeMail = zone.SizeMailShippingPrice;
			this.ShippingFreePriceFlg =
				this.IsShippingStorePickup
					? postageSetting.StorePickupFreePriceFlg
					: postageSetting.ShippingFreePriceFlg;
			this.ShippingZoneNo = zone.ShippingZoneNo;
			this.ShippingFreePrice = postageSetting.ShippingFreePrice.GetValueOrDefault(0);
			this.CalculationPluralKbn = postageSetting.CalculationPluralKbn;
			this.PluralShippingPrice = postageSetting.PluralShippingPrice.GetValueOrDefault(0);
			this.AnnounceFreeShippingFlg = postageSetting.AnnounceFreeShippingFlg;
			this.PaymentSelectionFlg = shippingInfo.PaymentSelectionFlg;
			this.PermittedPaymentIds = shippingInfo.PermittedPaymentIds;
			this.ShippingPriceSeparateEstimateFlg = (shippingInfo.ShippingPriceSeparateEstimatesFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID);
			this.ShippingPriceSeparateEstimateMessage = shippingInfo.ShippingPriceSeparateEstimatesMessage;
			this.ShippingPriceSeparateEstimateMessageMobile = shippingInfo.ShippingPriceSeparateEstimatesMessageMobile;
			this.FixedPurchaseFreeShippingFlg = shippingInfo.FixedPurchaseFreeShippingFlg;
			this.IsConditionalShippingPriceFree = zone.IsConditionalShippingPriceFree;
			this.ConditionalShippingPriceThreshold = zone.ConditionalShippingPriceThreshold;
			this.ConditionalShippingPrice = zone.ConditionalShippingPrice;
			this.FreeShippingFee = postageSetting.FreeShippingFee ?? 0;
		}

		/// <summary>
		/// 配送方法更新
		/// </summary>
		/// <param name="shippingMethod">配送方法</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		public void UpdateShippingMethod(string shippingMethod, string deliveryCompanyId)
		{
			this.ShippingMethod = shippingMethod;
			this.DeliveryCompanyId = deliveryCompanyId;
		}

		/// <summary>
		/// 配送希望時間帯更新
		/// </summary>
		/// <param name="shippingTime">配送希望時間帯</param>
		public void UpdateShippingTime(string shippingTime)
		{
			this.SpecifyShippingTimeFlg = true;
			this.ShippingTime = shippingTime;
		}

		/// <summary>
		/// オブジェクト複製
		/// </summary>
		/// <returns>複製したカート配送先情報オブジェクト</returns>
		public CartShipping Clone()
		{
			var clone = (CartShipping)MemberwiseClone();
			clone.CartObject = this.CartObject.CloneCartItem();
			return clone;
		}

		/// <summary>
		/// 注文配送先作成
		/// </summary>
		/// <returns>注文配送先</returns>
		public OrderShippingModel CreateOrderShipping()
		{
			var model = new OrderShippingModel()
			{
				ShippingName = this.Name,
				ShippingName1 = this.Name1,
				ShippingName2 = this.Name2,
				ShippingNameKana = this.NameKana,
				ShippingNameKana1 = this.NameKana1,
				ShippingNameKana2 = this.NameKana2,
				ShippingZip = this.Zip,
				ShippingAddr1 = this.Addr1,
				ShippingAddr2 = this.Addr2,
				ShippingAddr3 = this.Addr3,
				ShippingAddr4 = this.Addr4,
				ShippingCountryIsoCode = this.ShippingCountryIsoCode,
				ShippingCompanyName = this.CompanyName,
				ShippingCompanyPostName = this.CompanyPostName,
				ShippingTel1 = this.Tel1,
				SenderName = (this.CartObject.IsGift) ? StringUtility.ToEmpty(this.SenderName1 + this.SenderName2) : string.Empty,
				SenderName1 = (this.CartObject.IsGift) ? StringUtility.ToEmpty(this.SenderName1) : string.Empty,
				SenderName2 = (this.CartObject.IsGift) ? StringUtility.ToEmpty(this.SenderName2) : string.Empty,
				SenderNameKana = (this.CartObject.IsGift) ? StringUtility.ToEmpty(this.SenderNameKana1 + this.SenderNameKana2) : string.Empty,
				SenderNameKana1 = (this.CartObject.IsGift) ? StringUtility.ToEmpty(this.SenderNameKana1) : string.Empty,
				SenderNameKana2 = (this.CartObject.IsGift) ? StringUtility.ToEmpty(this.SenderNameKana2) : string.Empty,
				SenderZip = (this.CartObject.IsGift) ? StringUtility.ToEmpty(string.Join("-", this.SenderZip1, this.SenderZip2)) : string.Empty,
				SenderAddr1 = (this.CartObject.IsGift) ? StringUtility.ToEmpty(this.SenderAddr1) : string.Empty,
				SenderAddr2 = (this.CartObject.IsGift) ? StringUtility.ToEmpty(this.SenderAddr2) : string.Empty,
				SenderAddr3 = (this.CartObject.IsGift) ? StringUtility.ToEmpty(this.SenderAddr3) : string.Empty,
				SenderAddr4 = (this.CartObject.IsGift) ? StringUtility.ToEmpty(this.SenderAddr4) : string.Empty,
				SenderCompanyName = (this.CartObject.IsGift) ? StringUtility.ToEmpty(this.SenderCompanyName) : string.Empty,
				SenderCompanyPostName = (this.CartObject.IsGift) ? StringUtility.ToEmpty(this.SenderCompanyPostName) : string.Empty,
				SenderTel1 = (this.CartObject.IsGift) ? StringUtility.ToEmpty(string.Join("-", this.SenderTel1_1, this.SenderTel1_2, this.SenderTel1_3)) : string.Empty,
				ShippingDate = (this.SpecifyShippingDateFlg) && (this.ShippingDate != DateTime.MinValue) ? this.ShippingDate : null,
				ShippingMethod = this.ShippingMethod,
				DeliveryCompanyId = this.DeliveryCompanyId,
				ShippingTime = (this.SpecifyShippingTimeFlg) && (this.ShippingTime != null) ? this.ShippingTime : string.Empty,
				WrappingPaperType = (this.WrappingPaperValidFlg) ? StringUtility.ToEmpty(this.WrappingPaperType) : string.Empty,
				WrappingPaperName = (this.WrappingPaperValidFlg) ? StringUtility.ToEmpty(this.WrappingPaperName) : string.Empty,
				WrappingBagType = (this.WrappingBagValidFlg) ? StringUtility.ToEmpty(this.WrappingBagType) : string.Empty,
				AnotherShippingFlg = StringUtility.ToEmpty(this.AnotherShippingFlag),
				ScheduledShippingDate = this.ScheduledShippingDate,
				ShippingReceivingStoreId = StringUtility.ToEmpty(this.ConvenienceStoreId),
				ShippingReceivingStoreFlg = StringUtility.ToEmpty(this.ConvenienceStoreFlg),
				ShippingReceivingStoreType = StringUtility.ToEmpty(this.ShippingReceivingStoreType),
			};

			return model;
		}

		/// <summary>
		/// ユーザー配送先作成
		/// </summary>
		/// <param name="userId"></param>
		/// <returns>ユーザー配送先</returns>
		public UserShippingModel CreateUserShipping(string userId)
		{
			var userShipping = new UserShippingModel
			{
				UserId = userId,
				Name = this.UserShippingName,
				ShippingName = this.Name,
				ShippingName1 = this.Name1,
				ShippingName2 = this.Name2,
				ShippingNameKana = this.NameKana,
				ShippingNameKana1 = this.NameKana1,
				ShippingNameKana2 = this.NameKana2,
				ShippingZip = this.Zip,
				ShippingAddr1 = this.Addr1,
				ShippingAddr2 = this.Addr2,
				ShippingAddr3 = this.Addr3,
				ShippingAddr4 = this.Addr4,
				ShippingAddr5 = this.Addr5,
				ShippingCountryName = this.ShippingCountryName,
				ShippingCountryIsoCode = this.ShippingCountryIsoCode,
				ShippingTel1 = this.Tel1,
				ShippingCompanyName = this.CompanyName,
				ShippingCompanyPostName = this.CompanyPostName,
				ShippingReceivingStoreFlg = this.ConvenienceStoreFlg,
				ShippingReceivingStoreId = this.ConvenienceStoreId,
				ShippingReceivingStoreType = StringUtility.ToEmpty(this.ShippingReceivingStoreType),
			};
			return userShipping;
		}

		/// <summary>
		/// 設定可能な直近の次回配送日、次々回配送日をセット
		/// </summary>
		/// <param name="useFirstShippingDate">現時点での直近の配送日ではなく、定期購入初回配送日をもとに計算するか</param>
		public void CalculateNextShippingDates(bool useFirstShippingDate = false)
		{
			var shippingDate = useFirstShippingDate ? GetFirstShippingDate() : this.ShippingDate;
			var service = new FixedPurchaseService();
			this.NextShippingDate = service.CalculateNextShippingDate(
				this.FixedPurchaseKbn,
				this.FixedPurchaseSetting,
				shippingDate,
				this.FixedPurchaseDaysRequired,
				this.FixedPurchaseMinSpan,
				NextShippingCalculationMode.Earliest);
			this.NextNextShippingDate = service.CalculateNextNextShippingDate(
				this.FixedPurchaseKbn,
				this.FixedPurchaseSetting,
				shippingDate,
				this.FixedPurchaseDaysRequired,
				this.FixedPurchaseMinSpan,
				NextShippingCalculationMode.Earliest);
		}

		/// <summary>
		/// Get scheduled shipping date
		/// </summary>
		/// <returns>Scheduled shipping date</returns>
		public string GetScheduledShippingDate()
		{
			// when scheduled shipping date has value
			if (this.ScheduledShippingDate.HasValue)
			{
				var localeId = (Constants.GLOBAL_OPTION_ENABLE)
					? this.CartObject.Owner.DispLanguageLocaleId
					: string.Empty;

				return DateTimeUtility.ToString(
					this.ScheduledShippingDate,
					DateTimeUtility.FormatType.LongDateWeekOfDay1Letter,
					localeId);
			}
			else
			{
				// when scheduled shipping date has not value
				return CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@");
			}
		}

		/// <summary>
		/// Calculate scheduled shipping date
		/// </summary>
		/// <param name="shopId">The shop ID</param>
		/// <param name="countryIsoCode">国ISOコード</param>
		/// <param name="prefecture">The prefecture</param>
		/// <param name="zip">The zip code</param>
		public void CalculateScheduledShippingDate(string shopId, string countryIsoCode, string prefecture, string zip)
		{
			this.TotalLeadTime = OrderCommon.GetTotalLeadTime(shopId, this.DeliveryCompanyId, prefecture, zip);
			this.ScheduledShippingDate = OrderCommon.CalculateScheduledShippingDateBasedOnToday(
				shopId,
				this.ShippingDateForCalculation,
				this.ShippingMethod,
				this.DeliveryCompanyId,
				countryIsoCode,
				prefecture,
				zip);
		}

		/// <summary>
		/// ユーザが配送方法を未確定の場合は「Cart_ShippingMethod_UnSelected_Priority」の優先度より配送方法を確定
		/// </summary>
		/// <param name="model">配送種別モデル</param>
		public void CartShippingShippingMethodUserUnSelected(ShopShippingModel model)
		{
			// ユーザまたは内部処理による配送方法の確定がない場合、カート内商品からカートリスト上の配送方法を決定
			if (this.UserSelectedShippingMethod) return;

			if (this.CartObject.Shippings[0].IsMail)
			{
				this.ShippingMethod = ((OrderCommon.IsAvailableShippingKbnMail(this.CartObject.Items))
				&& (Constants.CART_SHIPPINGMETHOD_UNSELECTED_PRIORITY == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL))
					? Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL
					: Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
			}

			this.DeliveryCompanyId = ((model != null)
				? model.GetDefaultDeliveryCompany(this.IsExpress).DeliveryCompanyId
				: string.Empty);

			// Set delivery company id for EcPay
			if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
				&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
				&& (this.CartObject.Shippings[0].ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
				&& (model != null)
				&& model.HasConvenienceStoreEcPay(this.IsExpress, Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID))
			{
				this.DeliveryCompanyId = Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID;
			}
		}

		/// <summary>
		/// 配送方法が「メール便」指定でカート内がメール便不可の場合は配送方法を「宅配便」に変更
		/// </summary>
		/// <param name="model">配送種別モデル</param>
		public void UpdateShippingMethod(ShopShippingModel model)
		{
			// カート内がメール便不可となっていて配送方法にメール便が選択されている場合、配送方法は「宅配便」を選択
			if ((OrderCommon.IsAvailableShippingKbnMail(this.CartObject.Items) == false)
				&& this.IsMail)
			{
				UpdateShippingMethod(
					Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS,
					model.GetDefaultDeliveryCompany(true).DeliveryCompanyId);
			}
		}

		/// <summary>
		/// 住所項目結合（国名は含めない）
		/// </summary>
		/// <returns>結合した住所</returns>
		public string ConcatenateAddressWithoutCountryName()
		{
			var address = AddressHelper.ConcatenateAddressWithoutCountryName(
				this.Addr1,
				this.Addr2,
				this.Addr3,
				this.Addr4);

			return address;
		}

		/// <summary>
		/// Update Invoice
		/// </summary>
		/// <param name="uniformInvoiceType">Uniform Invoice Type</param>
		/// <param name="uniformInvoiceOption1">Uniform Invoice Option 1</param>
		/// <param name="uniformInvoiceOption2">Uniform Invoice Option 2</param>
		/// <param name="carryType">Carry Type</param>
		/// <param name="carryTypeOptionValue">Carry Type Option Value</param>
		public void UpdateInvoice(
			string uniformInvoiceType,
			string uniformInvoiceOption1,
			string uniformInvoiceOption2,
			string carryType,
			string carryTypeOptionValue)
		{
			this.UniformInvoiceType = uniformInvoiceType;
			this.UniformInvoiceOption1 = uniformInvoiceOption1;
			this.UniformInvoiceOption2 = uniformInvoiceOption2;
			this.CarryType = carryType;
			this.CarryTypeOptionValue = carryTypeOptionValue;
		}

		/// <summary>
		/// Update User Invoice Regist Setting
		/// </summary>
		/// <param name="userInvoiceRegistFlg">User Invoice Regist Flg</param>
		/// <param name="invoiceName">Invoice Name</param>
		public void UpdateUserInvoiceRegistSetting(
			bool? userInvoiceRegistFlg,
			string invoiceName)
		{
			this.UserInvoiceRegistFlg = (userInvoiceRegistFlg.HasValue) ? userInvoiceRegistFlg.Value : false;
			this.InvoiceName = invoiceName;
		}

		/// <summary>
		/// Update Convenience Store Addr
		/// </summary>
		/// <param name="shippingListKbn">Shipping List Kbn</param>
		/// <param name="convenienceStoreId">Convenience Store Id</param>
		/// <param name="convenienceStoreName">Convenience Store Name</param>
		/// <param name="convenienceStoreAddress">Convenience Store Address</param>
		/// <param name="convenienceStoreTel">Convenience Store Tel</param>
		/// <param name="shippingReceivingStoreType">Shipping Receiving Store Type</param>
		public void UpdateConvenienceStoreAddr(
			string shippingListKbn,
			string convenienceStoreId,
			string convenienceStoreName,
			string convenienceStoreAddress,
			string convenienceStoreTel,
			string shippingReceivingStoreType)
		{
			this.ShippingAddrKbn = shippingListKbn;
			this.ConvenienceStoreId = StringUtility.ToEmpty(convenienceStoreId);
			this.Name1 = StringUtility.ToEmpty(convenienceStoreName);
			this.Addr4 = StringUtility.ToEmpty(convenienceStoreAddress);
			this.Tel1 = StringUtility.ToEmpty(convenienceStoreTel);
			this.ConvenienceStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;
			this.Zip = string.Empty;
			this.Zip1 = string.Empty;
			this.Zip2 = string.Empty;
			this.IsSameShippingAsCart1 = false;
			this.ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_TW;
			this.ShippingCountryName = "Taiwan";
			this.Addr1 = string.Empty;
			this.Addr2 = Constants.TW_CITIES_LIST.FirstOrDefault(city => this.Addr4.Contains(city)) ?? string.Empty;
			this.Addr3 = string.Empty;
			this.SpecifyShippingTimeFlg = false;
			this.Name2 = string.Empty;
			this.NameKana1 = string.Empty;
			this.NameKana2 = string.Empty;
			this.CompanyName = string.Empty;
			this.CompanyPostName = string.Empty;
			this.Addr5 = string.Empty;
			this.ShippingReceivingStoreType = StringUtility.ToEmpty(shippingReceivingStoreType);
		}

		/// <summary>
		/// Update Next Shipping Item Fixed Purchase Informations
		/// </summary>
		/// <param name="nextShippingItemFixedPurchaseKbn">Next Shipping Item Fixed Purchase Kbn</param>
		/// <param name="nextShippingItemFixedPurchaseSetting">Next Shipping Item Fixed Purchase Setting</param>
		public void UpdateNextShippingItemFixedPurchaseInfos(
			string nextShippingItemFixedPurchaseKbn,
			string nextShippingItemFixedPurchaseSetting)
		{
			this.NextShippingItemFixedPurchaseKbn = nextShippingItemFixedPurchaseKbn;
			this.NextShippingItemFixedPurchaseSetting = nextShippingItemFixedPurchaseSetting;
		}

		/// <summary>
		/// Calculate Next Shipping Item Next Next Shipping Date
		/// </summary>
		public void CalculateNextShippingItemNextNextShippingDate()
		{
			if (this.CanSwitchProductFixedPurchaseNextShippingSecondTime == false) return;

			var nextShippingItemNextNextShippingDate = new FixedPurchaseService()
				.CalculateFollowingShippingDate(
					this.NextShippingItemFixedPurchaseKbn,
					this.NextShippingItemFixedPurchaseSetting,
					this.NextShippingDate,
					this.FixedPurchaseMinSpan,
					NextShippingCalculationMode.Earliest);
			this.NextNextShippingDate = nextShippingItemNextNextShippingDate;
		}

		/// <summary>
		/// Get Next Shipping Item Fixed Purchase Shipping Pattern
		/// </summary>
		/// <returns>Fixed Purchase Shipping Pattern</returns>
		public string GetNextShippingItemFixedPurchaseShippingPattern()
		{
			var fixedPurchaseKbn = this.FixedPurchaseKbn;
			var fixedPurchaseSetting = this.FixedPurchaseSetting;
			if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED
				&& this.CanSwitchProductFixedPurchaseNextShippingSecondTime)
			{
				fixedPurchaseKbn = this.NextShippingItemFixedPurchaseKbn;
				fixedPurchaseSetting = this.NextShippingItemFixedPurchaseSetting;
			}
			return OrderCommon.CreateFixedPurchaseSettingMessage(fixedPurchaseKbn, fixedPurchaseSetting);
		}

		/// <summary>
		/// Update convenience store shipping address for EcPay
		/// </summary>
		/// <param name="shippingName">Shipping name</param>
		/// <param name="shippingShopAddress">Shipping shop address</param>
		/// <param name="shippingTel">Shipping tel</param>
		/// <param name="deliveryCompanyId">Delivery company id</param>
		/// <param name="convenienceStoreId">Convenience store id</param>
		/// <param name="shippingReceivingStoreType">Shipping receiving store type</param>
		public void UpdateConvenienceStoreShippingAddressForEcPay(
			string shippingName,
			string shippingShopAddress,
			string shippingTel,
			string deliveryCompanyId,
			string convenienceStoreId,
			string shippingReceivingStoreType)
		{
			this.Name1 = StringUtility.ToEmpty(shippingName);
			this.Addr4 = StringUtility.ToEmpty(shippingShopAddress);
			this.Tel1 = StringUtility.ToEmpty(shippingTel);
			this.DeliveryCompanyId = StringUtility.ToEmpty(deliveryCompanyId);
			this.ConvenienceStoreId = StringUtility.ToEmpty(convenienceStoreId);
			this.ConvenienceStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;
			this.ShippingReceivingStoreType = StringUtility.ToEmpty(shippingReceivingStoreType);
		}

		/// <summary>
		/// Update shipping address
		/// </summary>
		/// <param name="shippingAddr">Shipping address</param>
		public void UpdateShippingAddr(Hashtable shippingAddr)
		{
			this.Name1 = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USER_NAME1]);
			this.Name2 = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USER_NAME2]);
			this.UserShippingName = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USERSHIPPING_SHIPPING_NAME]);
			this.NameKana1 = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USER_NAME_KANA1]);
			this.NameKana2 = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USER_NAME_KANA2]);
			this.Zip = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USER_ZIP]);
			this.Addr1 = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USER_ADDR1]);
			this.Addr2 = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USER_ADDR2]);
			this.Addr3 = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USER_ADDR3]);
			this.Addr4 = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USER_ADDR4]);
			this.Addr5 = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USER_ADDR5]);
			this.Tel1 = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USER_TEL1]);
			this.CompanyName = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USER_COMPANY_NAME]);
			this.CompanyPostName = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USER_COMPANY_POST_NAME]);
			this.ShippingCountryIsoCode = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE]);
			this.ShippingCountryName = StringUtility.ToEmpty(shippingAddr[Constants.FIELD_USER_ADDR_COUNTRY_NAME]);
		}

		/// <summary>
		/// Update sender address
		/// </summary>
		/// <param name="senderAddr">Sender address</param>
		public void UpdateSenderAddr(Hashtable senderAddr)
		{
			this.SenderName1 = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USER_NAME1]);
			this.SenderName2 = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USER_NAME2]);
			this.UserShippingName = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USERSHIPPING_SHIPPING_NAME]);
			this.SenderNameKana1 = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USER_NAME_KANA1]);
			this.SenderNameKana2 = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USER_NAME_KANA2]);
			this.SenderZip = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USER_ZIP]);
			this.SenderAddr1 = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USER_ADDR1]);
			this.SenderAddr2 = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USER_ADDR2]);
			this.SenderAddr3 = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USER_ADDR3]);
			this.SenderAddr4 = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USER_ADDR4]);
			this.SenderAddr5 = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USER_ADDR5]);
			this.SenderTel1 = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USER_TEL1]);
			this.SenderCompanyName = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USER_COMPANY_NAME]);
			this.SenderCompanyPostName = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USER_COMPANY_POST_NAME]);
			this.SenderCountryIsoCode = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE]);
			this.SenderCountryName = StringUtility.ToEmpty(senderAddr[Constants.FIELD_USER_ADDR_COUNTRY_NAME]);
			this.SenderZip1 = this.SenderZip.Split('-')[0];
			this.SenderZip2 = this.SenderZip.Split('-')[1];
		}
		/// <summary>
		/// 配送先が注文者情報の住所かどうか
		/// </summary>
		/// <returns>配送先が注文者情報の住所かどうか</returns>
		public bool DeliversOwnerAddress()
		{
			var result = this.ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER;
			return result;
		}

		/// <summary>
		/// Update Tel1
		/// </summary>
		public void UpdateTel1()
		{
			var shippingTel1 = this.Tel1.Split('-');
			this.Tel1_1 = (shippingTel1.Length > 0) ? shippingTel1[0] : string.Empty;
			this.Tel1_2 = (shippingTel1.Length > 1) ? shippingTel1[1] : string.Empty;
			this.Tel1_3 = (shippingTel1.Length > 2) ? shippingTel1[2] : string.Empty;
		}

		/// <summary>
		/// Update store pickup shipping address
		/// </summary>
		/// <param name="realShopAreaId">Real shop area id</param>
		/// <param name="realShopId">Real shop id</param>
		/// <param name="realShopName">Real shop name</param>
		/// <param name="realShopZip">Real shop zip</param>
		/// <param name="realShopAddr1">Real shop addr1</param>
		/// <param name="realShopAddr2">Real shop addr2</param>
		/// <param name="realShopAddr3">Real shop addr3</param>
		/// <param name="realShopAddr4">Real shop addr4</param>
		/// <param name="realShopAddr5">Real shop addr5</param>
		/// <param name="realShopOpenningHours">Real shop Openning hours</param>
		/// <param name="realShopTel">Real shop tel</param>
		/// <param name="realShopCountryIsoCode">Real shop country iso code</param>
		/// <param name="realShopCountryName">Real shop country name</param>
		/// <param name="shippingName1">Shipping name1</param>
		/// <param name="shippingName2">Shipping name2</param>
		/// <param name="shippingNameKana1">Shipping name kana1</param>
		/// <param name="shippingNameKana2">Shipping name kana2</param>
		/// <param name="shippingCompanyName">Shipping company name</param>
		/// <param name="shippingCompanyPostName">Shipping company post name</param>
		public void UpdateStorePickupShippingAddr(
			string realShopAreaId,
			string realShopId,
			string realShopName,
			string realShopNameKana,
			string realShopZip,
			string realShopAddr1,
			string realShopAddr2,
			string realShopAddr3,
			string realShopAddr4,
			string realShopAddr5,
			string realShopOpenningHours,
			string realShopTel,
			string realShopCountryIsoCode,
			string realShopCountryName)
		{
			this.RealShopAreaId = realShopAreaId;
			this.RealShopId = realShopId;
			this.RealShopName = realShopName;
			this.Name1 = realShopName;
			this.Name2 = string.Empty;
			this.NameKana1 = realShopNameKana;
			this.NameKana2 = string.Empty;
			this.Zip = realShopZip;
			this.Addr1 = realShopAddr1;
			this.Addr2 = realShopAddr2;
			this.Addr3 = realShopAddr3;
			this.Addr4 = realShopAddr4;
			this.Addr5 = realShopAddr5;
			this.RealShopOpenningHours = realShopOpenningHours;
			this.Tel1 = realShopTel;
			this.ShippingCountryIsoCode = realShopCountryIsoCode;
			this.ShippingCountryName = realShopCountryName;
			this.CompanyName = string.Empty;
			this.CompanyPostName = string.Empty;
			this.ShippingAddrKbn = FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP;
		}

		/// <summary>親カートオブジェクト</summary>
		public CartObject CartObject { get; set; }
		/// <summary>配送先氏名</summary>
		public string Name { get { return this.Name1 + this.Name2; } }
		/// <summary>配送先氏名1</summary>
		public string Name1 { get; set; }
		/// <summary>配送先氏名2</summary>
		public string Name2 { get; set; }
		/// <summary>配送先氏名かな</summary>
		public string NameKana { get { return this.NameKana1 + this.NameKana2; } }
		/// <summary>配送先氏名かな1</summary>
		public string NameKana1 { get; set; }
		/// <summary>配送先氏名かな2</summary>
		public string NameKana2 { get; set; }
		/// <summary>郵便番号</summary>
		public string Zip
		{
			get
			{
				if ((string.IsNullOrEmpty(this.Zip1) == false) && (string.IsNullOrEmpty(this.Zip2) == false))
				{
					return this.Zip1 + "-" + this.Zip2;
				}
				return m_zip;
			}
			private set { m_zip = value; }
		}
		private string m_zip;
		/// <summary>郵便番号（ハイフンなし）</summary>
		public string HyphenlessZip
		{
			get
			{
				return StringUtility.ToEmpty(this.Zip).Replace("-", "");
			}
		}
		/// <summary>郵便番号1</summary>
		public string Zip1 { get; set; }
		/// <summary>郵便番号2</summary>
		public string Zip2 { get; set; }
		/// <summary>都道府県</summary>
		public string Addr1 { get; set; }
		/// <summary>市区町村</summary>
		public string Addr2 { get; set; }
		/// <summary>番地</summary>
		public string Addr3 { get; set; }
		/// <summary>ビル・マンション名</summary>
		public string Addr4 { get; set; }
		/// <summary>州</summary>
		public string Addr5 { get; set; }
		/// <summary>住所国名</summary>
		public string ShippingCountryName { get; set; }
		/// <summary>住所国ISOコード</summary>
		public string ShippingCountryIsoCode { get; set; }
		/// <summary>企業名</summary>
		public string CompanyName { get; set; }
		/// <summary>部署名</summary>
		public string CompanyPostName { get; set; }
		/// <summary>電話番号</summary>
		public string Tel1
		{
			get
			{
				if ((string.IsNullOrEmpty(this.Tel1_1) == false)
					&& (string.IsNullOrEmpty(this.Tel1_2) == false)
					&& (string.IsNullOrEmpty(this.Tel1_3) == false))
				{
					return this.Tel1_1 + "-" + this.Tel1_2 + "-" + this.Tel1_3;
				}
				return m_tel1;
			}
			set { m_tel1 = value; }
		}
		private string m_tel1;
		/// <summary>電話番号1-1</summary>
		public string Tel1_1 { get; private set; }
		/// <summary>電話番号1-2</summary>
		public string Tel1_2 { get; private set; }
		/// <summary>電話番号1-3</summary>
		public string Tel1_3 { get; private set; }

		/// <summary>配送先がカート1（と等しい）か</summary>
		public bool IsSameShippingAsCart1 { get; set; }
		/// <summary>配送先区分(NEW、OWNER、またはアドレス帳番号)</summary>
		public string ShippingAddrKbn { get; set; }
		/// <summary>配送日指定フラグ</summary>
		public bool SpecifyShippingDateFlg { get; set; }
		/// <summary>配送時間帯指定フラグ</summary>
		public bool SpecifyShippingTimeFlg { get; set; }
		/// <summary>配送希望日（デフォルトの場合はnull）</summary>
		public DateTime? ShippingDate { get; set; }
		/// <summary>配送希望日計算用</summary>
		public DateTime? ShippingDateForCalculation { get; set; }
		/// <summary>配送希望時間帯（デフォルトの場合はnull）</summary>
		public string ShippingTime { get; set; }
		/// <summary>配送希望時間帯文言（デフォルトの場合はnull）</summary>
		public string ShippingTimeMessage { get; set; }
		/// <summary>配送伝票番号（管理画面用）</summary>
		public string ShippingCheckNo { get; set; }
		/// <summary>定期購入区分</summary>
		public string FixedPurchaseKbn { get; private set; }
		/// <summary>定期購入設定</summary>
		public string FixedPurchaseSetting { get; private set; }
		/// <summary>定期購入配送所要日数</summary>
		public int FixedPurchaseDaysRequired { get; private set; }
		/// <summary>定期購入最低配送間隔</summary>
		public int FixedPurchaseMinSpan { get; private set; }
		/// <summary>初回配送日</summary>
		public DateTime FirstShippingDate { get; set; }
		/// <summary>定期購入次回配送日</summary>
		public DateTime NextShippingDate { get; private set; }
		/// <summary>定期購入次々回配送日</summary>
		public DateTime NextNextShippingDate { get; private set; }
		/// <summary>アドレス帳保存フラグ</summary>
		public bool UserShippingRegistFlg { get; private set; }
		/// <summary>アドレス帳名称（保存用）</summary>
		public string UserShippingName { get; private set; }
		/// <summary>のし利用フラグ</summary>
		public bool WrappingPaperValidFlg { get; set; }
		/// <summary>のし種類</summary>
		public string WrappingPaperType { get; set; }
		/// <summary>のし差出人</summary>
		public string WrappingPaperName { get; set; }
		/// <summary>包装利用フラグ</summary>
		public bool WrappingBagValidFlg { get; set; }
		/// <summary>包装種類</summary>
		public string WrappingBagType { get; set; }

		/// <summary>送り主を配送先1と同じにするか</summary>
		public bool IsSameSenderAsShipping1 { get; set; }
		/// <summary>送り主住所区分(NEW、OWNER) ギフトＯＰオンのみ</summary>
		public AddrKbn SenderAddrKbn { get; set; }
		/// <summary>送り主氏名1</summary>
		public string SenderName1 { get; private set; }
		/// <summary>送り主氏名2</summary>
		public string SenderName2 { get; private set; }
		/// <summary>送り主氏名かな1</summary>
		public string SenderNameKana1 { get; private set; }
		/// <summary>送り主氏名かな2</summary>
		public string SenderNameKana2 { get; private set; }
		/// <summary>送り主国名</summary>
		public string SenderCountryName { get; private set; }
		/// <summary>送り主国ISOコード</summary>
		public string SenderCountryIsoCode { get; private set; }
		/// <summary>送り主郵便番号</summary>
		public string SenderZip
		{
			get
			{
				if ((string.IsNullOrEmpty(this.SenderZip1) == false) && (string.IsNullOrEmpty(this.SenderZip2) == false))
				{
					return this.SenderZip1 + "-" + this.SenderZip2;
				}
				return m_senderZip;
			}
			private set { m_senderZip = value; }
		}
		private string m_senderZip;
		/// <summary>送り主郵便番号1</summary>
		public string SenderZip1 { get; private set; }
		/// <summary>送り主郵便番号2</summary>
		public string SenderZip2 { get; private set; }
		/// <summary>送り主都道府県</summary>
		public string SenderAddr1 { get; private set; }
		/// <summary>送り主市区町村</summary>
		public string SenderAddr2 { get; private set; }
		/// <summary>送り主番地</summary>
		public string SenderAddr3 { get; private set; }
		/// <summary>送り主ビル・マンション名</summary>
		public string SenderAddr4 { get; private set; }
		/// <summary>送り主州</summary>
		public string SenderAddr5 { get; private set; }
		/// <summary>送り主企業名</summary>
		public string SenderCompanyName { get; private set; }
		/// <summary>送り主部署名</summary>
		public string SenderCompanyPostName { get; private set; }

		/// <summary>送り主電話番号1</summary>
		public string SenderTel1
		{
			get
			{
				if ((string.IsNullOrEmpty(this.SenderTel1_1) == false)
					&& (string.IsNullOrEmpty(this.SenderTel1_2) == false)
					&& (string.IsNullOrEmpty(this.SenderTel1_3) == false))
				{
					return this.SenderTel1_1 + "-" + this.SenderTel1_2 + "-" + this.SenderTel1_3;
				}
				return m_senderTel1;
			}
			private set { m_senderTel1 = value; }
		}
		private string m_senderTel1;
		/// <summary>送り主電話番号1-1</summary>
		public string SenderTel1_1 { get; private set; }
		/// <summary>送り主電話番号1-2</summary>
		public string SenderTel1_2 { get; private set; }
		/// <summary>送り主電話番号1-3</summary>
		public string SenderTel1_3 { get; private set; }

		/// <summary>紐付け商品</summary>
		public List<ProductCount> ProductCounts { get; private set; }

		#region プロパティ：配送種別設定
		/// <summary>XXSサイズ商品配送料</summary>
		public decimal ShippingPriceOfSizeXXS { get; protected set; }
		/// <summary>XSサイズ商品配送料</summary>
		public decimal ShippingPriceOfSizeXS { get; protected set; }
		/// <summary>Sサイズ商品配送料</summary>
		public decimal ShippingPriceOfSizeS { get; protected set; }
		/// <summary>Mサイズ商品配送料</summary>
		public decimal ShippingPriceOfSizeM { get; protected set; }
		/// <summary>Lサイズ商品配送料</summary>
		public decimal ShippingPriceOfSizeL { get; protected set; }
		/// <summary>XLサイズ商品配送料</summary>
		public decimal ShippingPriceOfSizeXL { get; protected set; }
		/// <summary>XXLサイズ商品配送料</summary>
		public decimal ShippingPriceOfSizeXXL { get; protected set; }
		/// <summary>メールサイズ商品配送料</summary>
		public decimal ShippingPriceOfSizeMail { get; protected set; }
		/// <summary>配送料無料購入金額設定フラグ</summary>
		public string ShippingFreePriceFlg { get; protected set; }
		/// <summary>配送料無料購入金額設定</summary>
		public decimal ShippingFreePrice { get; protected set; }
		/// <summary>配送料地帯区分</summary>
		public int ShippingZoneNo { get; protected set; }
		/// <summary>複数商品計算区分</summary>
		public string CalculationPluralKbn { get; protected set; }
		/// <summary>複数商品配送料</summary>
		public decimal PluralShippingPrice { get; protected set; }
		/// <summary>配送料無料案内表示フラグ</summary>
		public string AnnounceFreeShippingFlg { get; protected set; }
		/// <summary>決済選択の任意利用フラグ</summary>
		public string PaymentSelectionFlg { get; protected set; }
		/// <summary>決済選択の可能リスト</summary>
		public string PermittedPaymentIds { get; protected set; }
		/// <summary>配送料別途見積もり表示フラグ</summary>
		public bool ShippingPriceSeparateEstimateFlg { get; protected set; }
		/// <summary>配送料別途見積もり文言</summary>
		public string ShippingPriceSeparateEstimateMessage { get; protected set; }
		/// <summary>配送料別途見積もり文言（モバイル）</summary>
		public string ShippingPriceSeparateEstimateMessageMobile { get; protected set; }
		/// <summary>定期配送料無料フラグ</summary>
		public string FixedPurchaseFreeShippingFlg { get; protected set; }
		#endregion

		/// <summary>配送先商品小計</summary>
		public decimal PriceSubtotal
		{
			get
			{
				// 複数配送先の場合は配送先ごとに、複数配送先でない場合はカートごとに計算する
				var priceSubtotal = (this.CartObject.Shippings.Count != 1)
					? this.ProductCounts.Sum(item => PriceCalculator.GetItemPrice(item.Product.Price + item.Product.TotalOptionPrice, item.Count))
					: this.CartObject.Items.Sum(item => PriceCalculator.GetItemPrice(item.Price + item.TotalOptionPrice, item.Count));
				return priceSubtotal;
			}
		}
		/// <summary>配送料金</summary>
		public decimal PriceShipping { get; set; }
		/// <summary>別出荷フラグ</summary>
		public string AnotherShippingFlag { get; set; }
		/// <summary>配送方法</summary>
		public string ShippingMethod { get; set; }
		/// <summary>配送会社ID</summary>
		public string DeliveryCompanyId { get; set; }
		/// <summary>宅配便か？</summary>
		public bool IsExpress
		{
			get
			{
				return (this.ShippingMethod == null)
					|| (this.ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS);
			}
		}
		/// <summary>メール便か？</summary>
		public bool IsMail
		{
			get
			{
				return ((this.ShippingMethod == null)
					|| (this.ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL));
			}
		}
		/// <summary>配送先枝番</summary>
		public string ShippingNo { get; set; }
		/// <summary>配送先は日本の住所か</summary>
		public bool IsShippingAddrJp
		{
			get { return GlobalAddressUtil.IsCountryJp(this.ShippingCountryIsoCode); }
		}
		/// <summary>送り主は日本の住所か</summary>
		public bool IsSenderAddrJp
		{
			get { return GlobalAddressUtil.IsCountryJp(this.SenderCountryIsoCode); }
		}
		/// <summary>配送価格未設定エラーが出ているか</summary>
		public bool IsGlobalShippingPriceCalcError { get; set; }
		/// <summary>免税か</summary>
		public bool IsDutyFree
		{
			get { return (TaxCalculationUtility.CheckShippingPlace(this.ShippingCountryIsoCode, this.Addr5) == false); }
		}
		/// <summary>海外送料とするかどうか</summary>
		public bool UseGlobalShippingPrice
		{
			get
			{
				// オプションオフの場合は常に国内送料
				if (Constants.GLOBAL_OPTION_ENABLE == false) return false;

				// 運用拠点が日本以外は常に海外配送
				if (GlobalAddressUtil.IsCountryJp(Constants.OPERATIONAL_BASE_ISO_CODE) == false) return true;

				// 配送先が日本以外の場合は海外配送
				if (this.IsShippingAddrJp == false) return true;

				// 上記以外は国内配送
				return false;
			}
		}
		/// <summary>ユーザの配送方法選択状況 True:選択済み False:未選択</summary>
		public bool UserSelectedShippingMethod { get; set; }

		/// <summary>The scheduled shipping date</summary>
		public DateTime? ScheduledShippingDate { get; set; }
		/// <summary>総リードタイム</summary>
		public int TotalLeadTime { get; set; }
		/// <summary>Uniform Invoice Type</summary>
		public string UniformInvoiceType { get; set; }
		/// <summary>Uniform Invoice Option 1</summary>
		public string UniformInvoiceOption1 { get; set; }
		/// <summary>Uniform Invoice Option 2</summary>
		public string UniformInvoiceOption2 { get; set; }
		/// <summary>Invoice Name</summary>
		public string InvoiceName { get; set; }
		/// <summary>User Invoice Regist Flag</summary>
		public bool UserInvoiceRegistFlg { get; set; }
		/// <summary>Carry Type</summary>
		public string CarryType { get; set; }
		/// <summary>Carry Type Option</summary>
		public string CarryTypeOption { get; set; }
		/// <summary>Uniform Invoice Type Option</summary>
		public string UniformInvoiceTypeOption { get; set; }
		/// <summary>Carry Type Option Value</summary>
		public string CarryTypeOptionValue { get; set; }
		/// <summary>Is Shipping Address Taiwan</summary>
		public bool IsShippingAddrTw
		{
			get { return GlobalAddressUtil.IsCountryTw(this.ShippingCountryIsoCode); }
		}
		/// <summary>Is Taiwan country shipping enable</summary>
		public bool IsTaiwanCountryShippingEnable
		{
			get
			{
				return (Constants.TW_COUNTRY_SHIPPING_ENABLE && this.IsShippingAddrTw);
			}
		}
		/// <summary>Convenience Store Id</summary>
		public string ConvenienceStoreId { get; set; }
		/// <summary>Convenience Store Flg</summary>
		public string ConvenienceStoreFlg { get; set; }
		/// <summary>Is Shipping Convenience</summary>
		public bool IsShippingConvenience { get { return this.ShippingAddrKbn == FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE; } }
		/// <summary>Next Shipping Item Fixed Purchase Kbn</summary>
		public string NextShippingItemFixedPurchaseKbn { get; private set; }
		/// <summary>Next Shipping Item Fixed Purchase Setting</summary>
		public string NextShippingItemFixedPurchaseSetting { get; private set; }
		/// <summary>Can Switch Product Fixed Purchase Next Shipping Second Time</summary>
		public bool CanSwitchProductFixedPurchaseNextShippingSecondTime { get; set; }
		/// <summary>Shipping receiving store type</summary>
		public string ShippingReceivingStoreType { get; set; }
		/// <summary>定期配送設定を保持しているか</summary>
		public bool HasFixedPurchaseSetting
		{
			get
			{
				return ((string.IsNullOrEmpty(this.FixedPurchaseKbn) == false)
					&& (string.IsNullOrEmpty(this.FixedPurchaseSetting) == false));
			}
		}
		/// <summary>条件付き配送料が無料設定か</summary>
		public bool IsConditionalShippingPriceFree { get; set; }
		/// <summary>条件付き配送料閾値</summary>
		public decimal? ConditionalShippingPriceThreshold { get; set; }
		/// <summary>条件付き配送料</summary>
		public decimal? ConditionalShippingPrice { get; set; }
		/// <summary>Is uniform invoice company</summary>
		public bool IsUniformInvoiceCompany
		{
			get
			{
				var result = (this.UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY);
				return result;
			}
		}
		/// <summary>Is uniform invoice donate</summary>
		public bool IsUniformInvoiceDonate
		{
			get
			{
				var result = (this.UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_DONATE);
				return result;
			}
		}
		/// <summary>Is uniform invoice personal</summary>
		public bool IsUniformInvoicePersonal
		{
			get
			{
				var result = (this.UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL);
				return result;
			}
		}
		/// <summary>Is not uniform invoice personal</summary>
		public bool IsNotUniformInvoicePersonal
		{
			get
			{
				var result = (this.UniformInvoiceType != Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL);
				return result;
			}
		}
		/// <summary>Is convenience store flag on</summary>
		public bool IsConvenienceStoreFlagOn
		{
			get
			{
				var result = (this.ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON);
				return result;
			}
		}
		/// <summary>Is convenience store flag off</summary>
		public bool IsConvenienceStoreFlagOff
		{
			get
			{
				var result = (this.ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF);
				return result;
			}
		}
		/// <summary>Is not shipping address convenience store</summary>
		public bool IsNotShippingAddressConvenienceStore
		{
			get
			{
				var result = (this.ShippingAddrKbn != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE);
				return result;
			}
		}
		/// <summary>Real shop area</summary>
		public string RealShopAreaId { get; set; }
		/// <summary>Real shop id</summary>
		public string RealShopId { get; set; }
		/// <summary>Real shop name</summary>
		public string RealShopName { get; set; }
		/// <summary>Real shop openning hours</summary>
		public string RealShopOpenningHours { get; set; }
		/// <summary>Is shipping store pickup</summary>
		public bool IsShippingStorePickup { get { return this.ShippingAddrKbn == FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP; } }
		/// <summary>配送料無料時の請求料金</summary>
		public decimal FreeShippingFee { get; set; }
		/// <summary>配送料無料時の請求料金利用フラグ</summary>
		public bool IsUseFreeShippingFee { get; set; }
		///*********************************************************************************************
		/// <summary>カート商品数クラス（複数配送先向け。item_noと数量を格納）</summary>
		///*********************************************************************************************
		[Serializable]
		public class ProductCount
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="cartProduct">カート商品</param>
			/// <param name="iCount">数量</param>
			public ProductCount(CartProduct cartProduct, int iCount)
			{
				this.Product = cartProduct;
				this.Count = iCount;
			}

			/// <summary>商品</summary>
			public CartProduct Product { get; set; }
			/// <summary>数</summary>
			public int Count { get; set; }
			/// <summary>商品小計(割引金額の按分処理適用後)</summary>
			public decimal PriceSubtotalAfterDistribution { get; set; }
			/// <summary>調整金額(按分した商品分)</summary>
			public decimal ItemPriceRegulation { get; set; }
			/// <summary>商品小計(割引・調整金額按分適用後</summary>
			public decimal PriceSubtotalAfterDistributionAndRegulation
			{
				get { return this.PriceSubtotalAfterDistribution + this.ItemPriceRegulation; }
			}
			/// <summary>消費税合計(配送先合計)</summary>
			public decimal ItemPriceTax
			{
				get { return this.Product.PriceTax * this.Count; }
			}
		}
	}
}
