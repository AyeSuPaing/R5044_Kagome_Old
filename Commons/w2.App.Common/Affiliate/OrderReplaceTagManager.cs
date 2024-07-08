/*
=========================================================================================================
  Module      : 注文系置換処理クラス(CartReplaceTagManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Config;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.Web;
using w2.Common.Web;
using w2.Domain.Affiliate;

namespace w2.App.Common.Affiliate
{
	/// <summary>
	/// 注文系置換処理クラス
	/// </summary>
	public class OrderReplaceTagManager : ReplaceTagManager
	{
		/// <summary>
		/// 置換処理
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="data">注文データ</param>
		/// <param name="value">処理対象文字列</param>
		/// <param name="regionModel">リージョンモデル</param>
		/// <returns>置換処理後の文字列</returns>
		public string ReplaceTag(AffiliateTagSettingModel model, List<DataView> data, string value, RegionModel regionModel)
		{
			var result = value;

			result = OrderListReplace(data, model.AffiliateProductTagSettingModel, result, regionModel);

			return result;
		}

		/// <summary>
		/// 注文全体の置換処理
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="data">注文データ</param>
		/// <param name="value">処理対象文字列</param>
		/// <param name="regionModel">リージョンモデル</param>
		/// <returns>置換処理後の文字列</returns>
		private string OrderListReplace(List<DataView> data, AffiliateProductTagSettingModel model, string value, RegionModel regionModel)
		{
			var temp = value;

			if (data == null) return temp;

			// カートループ用タグが指定されている場合(複数カート)
			if (temp.Contains(this.OrderReplaceTags[ReplaceTagKey.ORDER_LOOP_START].Tag)
				&& (temp.Contains(this.OrderReplaceTags[ReplaceTagKey.ORDER_LOOP_END].Tag)))
			{
				var pattern = string.Format(
					@"({0})(?<order>[\s\S]+?)({1})",
					this.OrderReplaceTags[ReplaceTagKey.ORDER_LOOP_START].Tag,
					this.OrderReplaceTags[ReplaceTagKey.ORDER_LOOP_END].Tag);
				var orderContext = Regex.Match(temp, pattern).Groups["order"].Value;
				temp = temp.Replace(
					orderContext,
					string.Join(
						"",
						data.Cast<DataView>().Select(
							d => "@@W2_ORDER_" + (string)d[0][Constants.FIELD_ORDER_ORDER_ID] + "@@")));
				temp = temp.Replace(this.OrderReplaceTags[ReplaceTagKey.ORDER_LOOP_START].Tag, "").Replace(
					this.OrderReplaceTags[ReplaceTagKey.ORDER_LOOP_END].Tag,
					"");

				foreach (DataView order in data)
				{
					var orderTemp = orderContext;
					orderTemp = OrderReplace(order, model, orderContext, regionModel);
					temp = temp.Replace(
						"@@W2_ORDER_" + (string)order[0][Constants.FIELD_ORDER_ORDER_ID] + "@@",
						orderTemp);
				}
			}
			else
			{
				temp = temp.Replace(this.OrderReplaceTags[ReplaceTagKey.ORDER_LOOP_START].Tag, "").Replace(
					this.OrderReplaceTags[ReplaceTagKey.ORDER_LOOP_END].Tag,
					"");
				temp = OrderReplace(data.FirstOrDefault(), model, temp, regionModel);
			}

			return temp;
		}

		/// <summary>
		/// 注文ごとの置換処理
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="data">注文データ</param>
		/// <param name="value">処理対象文字列</param>
		/// <param name="regionModel">リージョンモデル</param>
		/// <returns>置換処理後の文字列</returns>
		private string OrderReplace(DataView data, AffiliateProductTagSettingModel model, string value, RegionModel regionModel)
		{
			var temp = value;

			temp = (data != null)
				? OrderProductReplace(data, model, value, regionModel)
				: temp.Replace(this.OrderReplaceTags[ReplaceTagKey.PRODUCT].Tag, string.Empty);

			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.ORDER_PRICE_SUB_TOTAL].Tag,
				(data != null) ? data[0][Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL].ToPriceString() : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.ORDER_PRICE_SUB_TOTAL_EXCLUDED].Tag,
				(data != null)
					? (decimal.Parse(data[0][Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL].ToString())
						- decimal.Parse(data[0][Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX].ToString())).ToPriceString()
					: string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.ORDER_PRICE_SUB_TOTAL_TAX].Tag,
				(data != null) ? data[0][Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX].ToPriceString() : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.ORDER_ITEM_QUANTITY].Tag,
				(data != null)
					? data.Cast<DataRowView>()
						.Select(oi => Convert.ToInt32(oi[Constants.FIELD_ORDERITEM_ITEM_QUANTITY])).Sum().ToString()
					: string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.ORDER_ID].Tag,
				(data != null) ? HtmlSanitizer.HtmlEncode(data[0][Constants.FIELD_ORDER_ORDER_ID].ToString()) : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.OWNER_USER_EMAIL].Tag,
				(data != null) ? HtmlSanitizer.HtmlEncode(HtmlSanitizer.HtmlEncode(data[0][Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR].ToString())) : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.OWNER_USER_TEL1_WITH_COUNTRY_CODE].Tag,
				(data != null) ? HtmlSanitizer.HtmlEncode(HtmlSanitizer.HtmlEncode(AddInternationalTelCode(data))) : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.OWNER_USER_SEX].Tag,
				(data != null) ? HtmlSanitizer.HtmlEncode(HtmlSanitizer.HtmlEncode(data[0][Constants.FIELD_ORDEROWNER_OWNER_SEX].ToString())) : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.OWNER_USER_FAMILY_NAME].Tag,
				(data != null) ? HtmlSanitizer.HtmlEncode(data[0][Constants.FIELD_ORDEROWNER_OWNER_NAME1].ToString()) : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.OWNER_USER_FIRST_NAME].Tag,
				(data != null) ? HtmlSanitizer.HtmlEncode(data[0][Constants.FIELD_ORDEROWNER_OWNER_NAME2].ToString()) : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.OWNER_USER_FAMILY_NAME_KANA].Tag,
				(data != null) ? HtmlSanitizer.HtmlEncode(data[0][Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1].ToString()) : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.OWNER_USER_FIRST_NAME_KANA].Tag,
				(data != null) ? HtmlSanitizer.HtmlEncode(data[0][Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2].ToString()) : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.OWNER_USER_BIRTH_DAY].Tag,
				(data != null) ? data[0][Constants.FIELD_ORDEROWNER_OWNER_BIRTH].ToString() : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.OWNER_USER_AGE].Tag,
				(data != null && (data[0][Constants.FIELD_ORDEROWNER_OWNER_BIRTH] != DBNull.Value)) ? UserAge((DateTime)data[0][Constants.FIELD_ORDEROWNER_OWNER_BIRTH]).ToString() : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.OWNER_USER_ZIP].Tag,
				(data != null)
					? HtmlSanitizer.HtmlEncode(data[0][Constants.FIELD_ORDEROWNER_OWNER_ZIP].ToString().Replace("-", ""))
					: string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.OWNER_USER_PREF].Tag,
				(data != null) ? HtmlSanitizer.HtmlEncode(data[0][Constants.FIELD_ORDEROWNER_OWNER_ADDR1].ToString()) : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.SHIPPING_USER_FAMILY_NAME].Tag,
				(data != null) ? HtmlSanitizer.HtmlEncode(data[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1].ToString()) : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.SHIPPING_USER_FIRST_NAME].Tag,
				(data != null) ? HtmlSanitizer.HtmlEncode(data[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2].ToString()) : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.SHIPPING_USER_FAMILY_NAME_KANA].Tag,
				(data != null) ? HtmlSanitizer.HtmlEncode(data[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1].ToString()) : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.SHIPPING_USER_FIRST_NAME_KANA].Tag,
				(data != null) ? HtmlSanitizer.HtmlEncode(data[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2].ToString()) : string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.SHIPPING_USER_ZIP].Tag,
				(data != null)
					? HtmlSanitizer.HtmlEncode(data[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP].ToString().Replace("-", ""))
					: string.Empty);
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.SHIPPING_USER_PREF].Tag,
				(data != null) ? HtmlSanitizer.HtmlEncode(data[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1].ToString()) : string.Empty);

			// Cookieに保存されている有効な広告コードの値で置換
			var validAdvParameterName = (string)SessionManager.Session[Constants.SESSION_KEY_ADV_PARAMETER_NAME];
			temp = temp.Replace(
				this.OrderReplaceTags[ReplaceTagKey.AFFILIATE_REPORT].Tag,
				AffiliateCookieManager.GetCookieValue(validAdvParameterName));

			return temp;
		}

		/// <summary>
		/// 国番号付与
		/// </summary>
		/// <param name="data">注文データ</param>
		/// <returns>電話番号</returns>
		private string AddInternationalTelCode(DataView data)
		{
			var telNumber = "";
			var tel = data[0][Constants.FIELD_ORDEROWNER_OWNER_TEL1].ToString();
			if (string.IsNullOrEmpty(tel)) return "";
			var code = GlobalConfigs.GetInstance().GlobalSettings.InternationalTelephoneCode
				.FirstOrDefault(x => (x.Iso == (string)data[0][Constants.FIELD_ORDEROWNER_ACCESS_COUNTRY_ISO_CODE]));

			if (Constants.GLOBAL_OPTION_ENABLE == false)
			{
				var isoCode = new Regex("0");
				telNumber = isoCode.Replace(tel, "81", 1).Replace("-", "");
				return telNumber;
			}

			if (code == null)
			{
				telNumber = tel.Replace("-", "");
				return telNumber;
			}

			telNumber = code.Number;
			telNumber += tel.StartsWith("0")
				? string.Join("", tel.Skip(1))
				: tel;

			var result = telNumber.Replace("-", "");
			return result;
		}

		/// <summary>
		/// 注文商品の置換処理
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="data">注文データ</param>
		/// <param name="value">処理対象文字列</param>
		/// <param name="regionModel">リージョンモデル</param>
		/// <returns>置換処理後の文字列</returns>
		private string OrderProductReplace(DataView data, AffiliateProductTagSettingModel model, string value, RegionModel regionModel)
		{
			var temp = value;

			if (data == null) return temp;

			if ((model == null) || (temp.Contains(this.OrderReplaceTags[ReplaceTagKey.PRODUCT].Tag) == false)) return temp;

			var products = data.Cast<DataRowView>().Select(
				d => model.TagContent
					.Replace(
						this.CartProductReplaceTags[ReplaceTagKey.PRODUCT_ID].Tag,
						(string)d[Constants.FIELD_ORDERITEM_PRODUCT_ID])
					.Replace(
						this.CartProductReplaceTags[ReplaceTagKey.VARIATION_ID].Tag,
						(string)d[Constants.FIELD_ORDERITEM_VARIATION_ID])
					.Replace(
						this.CartProductReplaceTags[ReplaceTagKey.PRODUCT_NAME].Tag,
						HtmlSanitizer.HtmlEncode((Constants.GLOBAL_OPTION_ENABLE == false) 
							? (string)d[Constants.FIELD_ORDERITEM_PRODUCT_NAME] 
							: NameTranslationCommon.GetOrderItemProductTranslationName(
							(string)d[Constants.FIELD_ORDERITEM_PRODUCT_NAME],
							(string)d[Constants.FIELD_ORDERITEM_PRODUCT_ID],
							(string)d[Constants.FIELD_ORDERITEM_VARIATION_ID],
							regionModel.LanguageCode,
							regionModel.LanguageLocaleId)))
					.Replace(
						this.CartProductReplaceTags[ReplaceTagKey.QUANTITY].Tag,
						d[Constants.FIELD_ORDERITEM_ITEM_QUANTITY].ToString())
					.Replace(
						this.CartProductReplaceTags[ReplaceTagKey.PRODUCT_PRICE_TOTAL].Tag,
						string.Format("{0:F0}", d[Constants.FIELD_ORDERITEM_ITEM_PRICE]))
					.Replace(
						this.CartProductReplaceTags[ReplaceTagKey.PRODUCT_PRICE].Tag,
						d[Constants.FIELD_ORDERITEM_PRODUCT_PRICE].ToPriceString())
					.Replace(
						this.CartProductReplaceTags[ReplaceTagKey.PRODUCT_PRICE_TAX_EXCLUDE].Tag,
						(decimal.Parse(d[Constants.FIELD_ORDERITEM_PRODUCT_PRICE].ToString())
							- TaxCalculationUtility.GetTaxPrice(
								decimal.Parse(d[Constants.FIELD_ORDERITEM_PRODUCT_PRICE].ToString()),
								decimal.Parse(d[Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE].ToString()),
								d[Constants.FIELD_ORDERITEM_PRODUCT_TAX_ROUND_TYPE].ToString())).ToPriceString()))
					.ToList();

			var result = ProductTagDelimiterSetSerialNumber(products, model.TagDelimiter);

			temp = temp.Replace(this.OrderReplaceTags[ReplaceTagKey.PRODUCT].Tag, result);
			return temp;
		}
	}
}