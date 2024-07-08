/*
=========================================================================================================
  Module      : カート系置換処理クラス(CartReplaceTagManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Text.RegularExpressions;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Config;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.Affiliate;

namespace w2.App.Common.Affiliate
{
	/// <summary>
	/// カート系置換処理クラス
	/// </summary>
	public class CartReplaceTagManager : ReplaceTagManager
	{
		/// <summary>
		/// 置換処理
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="data">カートリストデータ</param>
		/// <param name="value">処理対象文字列</param>
		/// <param name="regionModel">リージョンモデル</param>
		/// <returns>置換処理後の文字列</returns>
		public string ReplaceTag(
			AffiliateTagSettingModel model,
			CartObjectList data,
			string value,
			RegionModel regionModel)
		{
			var result = value;

			if (data == null) return result;

			result = CartListReplace(data, model.AffiliateProductTagSettingModel, result, regionModel);

			return result;
		}

		/// <summary>
		/// カートリスト全体の置換処理
		/// </summary>
		/// <param name="cartList">カートリストデータ</param>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="value">処理対象文字列</param>
		/// <param name="regionModel">リージョンモデル</param>
		/// <returns>置換処理後の文字列</returns>
		private string CartListReplace(
			CartObjectList cartList,
			AffiliateProductTagSettingModel model,
			string value,
			RegionModel regionModel)
		{
			var temp = value;
			// カートループ用タグが指定されている場合(複数カート)
			if (temp.Contains(this.CartReplaceTags[ReplaceTagKey.CART_LOOP_START].Tag)
				&& (temp.Contains(this.CartReplaceTags[ReplaceTagKey.CART_LOOP_END].Tag)))
			{
				var pattern = string.Format(
					@"({0})(?<cart>[\s\S]+?)({1})",
					this.CartReplaceTags[ReplaceTagKey.CART_LOOP_START].Tag,
					this.CartReplaceTags[ReplaceTagKey.CART_LOOP_END].Tag);
				var cartContext = Regex.Match(temp, pattern).Groups["cart"].Value;

				if ((cartList == null) || (cartList.Items.Count == 0))
					return temp.Replace(cartContext, "")
						.Replace(this.CartReplaceTags[ReplaceTagKey.CART_LOOP_START].Tag, "").Replace(
							this.CartReplaceTags[ReplaceTagKey.CART_LOOP_END].Tag,
							"");

				temp = temp.Replace(
					cartContext,
					string.Join("", cartList.Items.Select(c => "@@W2_CART_" + c.CartId + "@@")));
				temp = temp.Replace(this.CartReplaceTags[ReplaceTagKey.CART_LOOP_START].Tag, "").Replace(
					this.CartReplaceTags[ReplaceTagKey.CART_LOOP_END].Tag,
					"");
				foreach (CartObject cart in cartList.Items)
				{
					temp = temp.Replace(
						"@@W2_CART_" + cart.CartId + "@@",
						CartReplace(cart, model, cartContext, regionModel));
				}
			}
			// カートループ用タグが指定されていない場合(シングルカート)
			else
			{
				temp = temp.Replace(this.CartReplaceTags[ReplaceTagKey.CART_LOOP_START].Tag, "").Replace(
					this.CartReplaceTags[ReplaceTagKey.CART_LOOP_END].Tag,
					"");

				if (cartList == null) return temp;

				temp = CartReplace(cartList.Items.FirstOrDefault(), model, temp, regionModel);
			}

			return temp;
		}

		/// <summary>
		/// カート単位での置換処理
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="value">処理対象文字列</param>
		/// <param name="regionModel">リージョンモデル</param>
		/// <returns>置換処理後の文字列</returns>
		private string CartReplace(
			CartObject cart,
			AffiliateProductTagSettingModel model,
			string value,
			RegionModel regionModel)
		{
			var temp = value;

			temp = (cart != null)
				? CartProductReplace(cart, model, value, regionModel)
				: temp.Replace(this.CartReplaceTags[ReplaceTagKey.PRODUCT].Tag, string.Empty);

			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.CART_PRICE_SUB_TOTAL].Tag,
				(cart != null) ? cart.PriceSubtotal.ToPriceString() : string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.CART_PRICE_SUB_TOTAL_TAX_EXCLUDED].Tag,
				(cart != null) ? (cart.PriceSubtotal - cart.PriceSubtotalTax).ToPriceString() : string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.CART_PRICE_SUB_TOTAL_TAX].Tag,
				(cart != null) ? cart.PriceSubtotalTax.ToPriceString() : string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.CART_ITEM_QUANTITY].Tag,
				(cart != null) 
					? (cart.Items.Select(p => p.QuantitiyUnallocatedToSet).Sum() 
						+ cart.SetPromotions.Items.SelectMany(p => p.Items.Select(cp => cp.QuantityAllocatedToSet[p.CartSetPromotionNo])).Sum()).ToString()
					: string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.OWNER_USER_EMAIL].Tag,
				((cart != null) && (cart.Owner != null)) ? HtmlSanitizer.HtmlEncode(cart.Owner.MailAddr) : string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.OWNER_USER_TEL1_WITH_COUNTRY_CODE].Tag,
				((cart != null) && (cart.Owner != null)) ? HtmlSanitizer.HtmlEncode(AddInternationalTelCode(cart)) : string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.OWNER_USER_SEX].Tag,
				((cart != null) && (cart.Owner != null)) ? HtmlSanitizer.HtmlEncode(cart.Owner.Sex) : string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.OWNER_USER_FAMILY_NAME].Tag,
				((cart != null) && (cart.Owner != null)) ? HtmlSanitizer.HtmlEncode(cart.Owner.Name1) : string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.OWNER_USER_FIRST_NAME].Tag,
				((cart != null) && (cart.Owner != null)) ? HtmlSanitizer.HtmlEncode(cart.Owner.Name2) : string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.OWNER_USER_FAMILY_NAME_KANA].Tag,
				((cart != null) && (cart.Owner != null)) ? HtmlSanitizer.HtmlEncode(cart.Owner.NameKana1) : string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.OWNER_USER_FIRST_NAME_KANA].Tag,
				((cart != null) && (cart.Owner != null)) ? HtmlSanitizer.HtmlEncode(cart.Owner.NameKana2) : string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.OWNER_USER_BIRTH_DAY].Tag,
				((cart != null) && (cart.Owner != null) && (cart.Owner.Birth != null)) ? cart.Owner.Birth.ToString() : string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.OWNER_USER_AGE].Tag,
				((cart != null) && (cart.Owner != null) && (cart.Owner.Birth != null)) ? UserAge(cart.Owner.Birth.Value).ToString() : string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.OWNER_USER_ZIP].Tag,
				((cart != null) && (cart.Owner != null) && (cart.Owner.Zip != null)) ? HtmlSanitizer.HtmlEncode(cart.Owner.Zip.Replace("-", "")) : string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.OWNER_USER_PREF].Tag,
				((cart != null) && (cart.Owner != null)) ? HtmlSanitizer.HtmlEncode(cart.Owner.Addr1) : string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.SHIPPING_USER_FAMILY_NAME].Tag,
				((cart != null) && cart.Shippings.Count > 0)
					? HtmlSanitizer.HtmlEncode(StringUtility.ToEmpty(cart.Shippings[0].Name1))
					: string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.SHIPPING_USER_FIRST_NAME].Tag,
				((cart != null) && cart.Shippings.Count > 0)
					? HtmlSanitizer.HtmlEncode(StringUtility.ToEmpty(cart.Shippings[0].Name2))
					: string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.SHIPPING_USER_FAMILY_NAME_KANA].Tag,
				((cart != null) && cart.Shippings.Count > 0)
					? HtmlSanitizer.HtmlEncode(StringUtility.ToEmpty(cart.Shippings[0].NameKana1))
					: string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.SHIPPING_USER_FIRST_NAME_KANA].Tag,
				((cart != null) && cart.Shippings.Count > 0)
					? HtmlSanitizer.HtmlEncode(StringUtility.ToEmpty(cart.Shippings[0].NameKana2))
					: string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.SHIPPING_USER_ZIP].Tag,
				((cart != null) && cart.Shippings.Count > 0)
					? HtmlSanitizer.HtmlEncode(StringUtility.ToEmpty(cart.Shippings[0].Zip).Replace("-", ""))
					: string.Empty);
			temp = temp.Replace(
				this.CartReplaceTags[ReplaceTagKey.SHIPPING_USER_PREF].Tag,
				((cart != null) && cart.Shippings.Count > 0)
					? HtmlSanitizer.HtmlEncode(StringUtility.ToEmpty(cart.Shippings[0].Addr1))
					: string.Empty);

			return temp;
		}

		/// <summary>
		/// 国番号付与
		/// </summary>
		/// <param name="cart">カート</param>
		/// <returns>電話番号</returns>
		private string AddInternationalTelCode(CartObject cart)
		{
			var telNumber = "";
			if (string.IsNullOrEmpty(cart.Owner.Tel1)) return "";

			var code = GlobalConfigs.GetInstance().GlobalSettings.InternationalTelephoneCode
				.FirstOrDefault(x => (x.Iso == cart.Owner.AddrCountryIsoCode));

			if (Constants.GLOBAL_OPTION_ENABLE == false)
			{
				var isoCode = new Regex("0");
				telNumber = isoCode.Replace(cart.Owner.Tel1, "81", 1).Replace("-", "");
				return telNumber;
			}

			if (code == null)
			{
				telNumber = cart.Owner.Tel1.Replace("-", "");
				return telNumber;
			}

			telNumber = code.Number;
			telNumber += cart.Owner.Tel1.StartsWith("0")
				? string.Join("", cart.Owner.Tel1.Skip(1))
				: cart.Owner.Tel1;

			var result = telNumber.Replace("-", "");
			return result;
		}

		/// <summary>
		/// カート内商品の置換処理
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="value">処理対象文字列</param>
		/// <param name="regionModel">リージョンモデル</param>
		/// <returns>置換処理後の文字列</returns>
		private string CartProductReplace(
			CartObject cart,
			AffiliateProductTagSettingModel model,
			string value,
			RegionModel regionModel)
		{
			var temp = value;
			if ((model == null) || (temp.Contains(this.CartReplaceTags[ReplaceTagKey.PRODUCT].Tag) == false))
				return temp;

			// 通常商品
			var products =
				cart.Items.Where(c => (c.IsSetItem == false) && (c.QuantitiyUnallocatedToSet != 0)).Select(
					c => model.TagContent
						.Replace(this.CartProductReplaceTags[ReplaceTagKey.PRODUCT_ID].Tag, HtmlSanitizer.HtmlEncode(c.ProductId))
						.Replace(this.CartProductReplaceTags[ReplaceTagKey.VARIATION_ID].Tag, HtmlSanitizer.HtmlEncode(c.VariationId))
						.Replace(
							this.CartProductReplaceTags[ReplaceTagKey.PRODUCT_NAME].Tag,
							HtmlSanitizer.HtmlEncode((Constants.GLOBAL_OPTION_ENABLE == false)
								? c.ProductJointName
								: NameTranslationCommon.GetOrderItemProductTranslationName(
									c.ProductJointName,
									c.ProductId,
									c.VariationId,
									regionModel.LanguageCode,
									regionModel.LanguageLocaleId)))
						.Replace(
							this.CartProductReplaceTags[ReplaceTagKey.QUANTITY].Tag,
							(c.QuantitiyUnallocatedToSet.ToString()))
						.Replace(this.CartProductReplaceTags[ReplaceTagKey.PRODUCT_PRICE].Tag, c.Price.ToPriceString())
						.Replace(
							this.CartProductReplaceTags[ReplaceTagKey.PRODUCT_PRICE_TAX_EXCLUDE].Tag,
							(c.Price - TaxCalculationUtility.RoundTaxFraction(
								c.PriceTax,
								Constants.TAX_EXCLUDED_FRACTION_ROUNDING)).ToPriceString())).ToList();


			// セットプロモーション商品
			var setPromotionProducts = 
				cart.SetPromotions.Items.SelectMany(
					c => c.Items.Select(
						cp => model.TagContent
							.Replace(this.CartProductReplaceTags[ReplaceTagKey.PRODUCT_ID].Tag, HtmlSanitizer.HtmlEncode(cp.ProductId))
							.Replace(this.CartProductReplaceTags[ReplaceTagKey.VARIATION_ID].Tag, HtmlSanitizer.HtmlEncode(cp.VariationId))
							.Replace(
								this.CartProductReplaceTags[ReplaceTagKey.PRODUCT_NAME].Tag,
								HtmlSanitizer.HtmlEncode((Constants.GLOBAL_OPTION_ENABLE == false)
									? cp.ProductJointName
									: NameTranslationCommon.GetOrderItemProductTranslationName(
										cp.ProductJointName,
										cp.ProductId,
										cp.VariationId,
										regionModel.LanguageCode,
										regionModel.LanguageLocaleId)))
							.Replace(
								this.CartProductReplaceTags[ReplaceTagKey.QUANTITY].Tag,
								(cp.QuantityAllocatedToSet[c.CartSetPromotionNo].ToString()))
							.Replace(
								this.CartProductReplaceTags[ReplaceTagKey.PRODUCT_PRICE].Tag,
								cp.Price.ToPriceString()).Replace(
								this.CartProductReplaceTags[ReplaceTagKey.PRODUCT_PRICE_TAX_EXCLUDE].Tag,
								(cp.Price - TaxCalculationUtility.RoundTaxFraction(
									cp.PriceTax,
									Constants.TAX_EXCLUDED_FRACTION_ROUNDING)).ToPriceString()))).ToList();

			products.AddRange(setPromotionProducts);

			var result = ProductTagDelimiterSetSerialNumber(products, model.TagDelimiter);

			temp = temp.Replace(this.CartReplaceTags[ReplaceTagKey.PRODUCT].Tag, result);

			return temp;
		}
	}
}