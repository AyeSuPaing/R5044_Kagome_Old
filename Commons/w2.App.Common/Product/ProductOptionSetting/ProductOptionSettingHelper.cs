/*
=========================================================================================================
  Module      : 商品付帯情報ヘルパークラス(ProductOptionSettingHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Text.RegularExpressions;
using w2.Common.Helper;
using w2.Domain.Product.Helper;
using System;
using System.Collections.Generic;
using w2.App.Common.Order;
using w2.Common.Util;
using w2.App.Common.Global.Region.Currency;
using System.Text;
using System.Web.UI.WebControls;

namespace w2.App.Common.Product
{
	/// <summary>
	/// 商品付帯情報ヘルパークラス
	/// </summary>
	public class ProductOptionSettingHelper
	{
		/// <summary>
		/// 利用可能な商品付帯情報の全キー取得
		/// </summary>
		/// <returns>商品付帯情報キー</returns>
		public static string[] GetAllProductOptionSettingKeys()
		{
			var posKeys = Enumerable.Range(1, Constants.PRODUCTOPTIONVALUES_MAX_COUNT).Select(
				num => string.Format("{0}{1}", Constants.HASH_KEY_PRODUCTOPTIONSETTING, num)).ToArray();
			return posKeys;
		}

		/// <summary>
		/// 商品付帯情報のキー取得
		/// </summary>
		/// <param name="number">商品付帯情報のキー番号</param>
		/// <returns>商品付帯情報のキー</returns>
		public static string GetProductOptionSettingKey(int number)
		{
			var posKeys = string.Format("{0}{1}", Constants.HASH_KEY_PRODUCTOPTIONSETTING, number);
			return posKeys;
		}

		/// <summary>
		/// 表示用の商品付帯情報選択値取得
		/// </summary>
		/// <remarks>
		/// ・JSON形式の場合は "項目名1：選択値　項目名2：選択値…" の形式に変換<br/>
		/// （オプション価格の場合は "項目名1：選択値(+\XXX)"の形式で表示する）<br/>
		/// ・JSON形式以外の場合は productOptionTexts の内容をそのまま出力する
		/// </remarks>
		/// <returns>表示用商品付帯情報選択値</returns>
		public static string GetDisplayProductOptionTexts(string productOptionTexts)
		{
			var poSelectedValues = SerializeHelper.GetDeserializeJson<ProductOptionSelectedValue[]>(productOptionTexts);
			if (poSelectedValues == null) return productOptionTexts;

			var poString = poSelectedValues
				.Where(posv => (string.IsNullOrEmpty(posv.Value)) == false)
				.Select(posv => string.Format("{0}:{1}", posv.Name, GetFormattedProductOptionSelectedValue(posv.Value)))
				.ToList();
			var displayProductOptionTexts = string.Join("　", poString);
			return displayProductOptionTexts;

			// オプション価格を考慮した選択値形式に変換し取得
			string GetFormattedProductOptionSelectedValue(string productOptionSelectedValue)
			{
				if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false) return productOptionSelectedValue;

				var formattedValue = Regex.Replace(
					productOptionSelectedValue,
					Constants.REGEX_PATTERN_OPTION_PRICE_SEARCH_PATTERN,
					match =>
					{
						var price = ObjectUtility.TryParseDecimal(Regex.Match(match.Value, @"\d+").Value, 0);
						return "(+" + CurrencyManager.ToPrice(price) + ")";
					});
				return formattedValue;
			}
		}

		/// <summary>
		/// 商品付帯情報のメールテンプレート文作成
		/// </summary>
		/// <param name="productOptionTexts">商品付帯情報</param>
		/// <param name="orderItemString">注文商品情報文字列</param>
		/// <returns>注文商品情報文字列</returns>
		public static StringBuilder GetProductOptionTextForMailTemplate(string productOptionTexts, StringBuilder orderItemString)
		{
			var productOptionSettingSelectValues = GetDisplayProductOptionTexts(productOptionTexts);
			if (string.IsNullOrEmpty(productOptionSettingSelectValues) == false)
			{
				orderItemString.Append("            ").Append(productOptionSettingSelectValues).Append("\r\n");
			}

			return orderItemString;
		}

		/// <summary>
		/// 商品付帯情報一覧の取得
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="optionTexts">商品付帯情報</param>
		/// <returns>商品付帯情報一覧</returns>
		public static ProductOptionSettingList GetProductOptionSettingList(string shopId, string productId, string optionTexts)
		{
			var productOptionSettingList = new ProductOptionSettingList(shopId, productId);
			if (string.IsNullOrEmpty(optionTexts) == false)
			{
				productOptionSettingList.SetDefaultValueFromProductOptionTexts(optionTexts);
			}

			return productOptionSettingList;
		}

		/// <summary>
		/// 商品付帯情報選択値からオプション価格を抽出
		/// </summary>
		/// <param name="productOptionTexts">商品付帯情報選択値</param>
		/// <returns>オプション価格</returns>
		public static decimal ExtractOptionPriceFromProductOptionTexts(string productOptionTexts)
		{
			if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false) return 0;
			var poSelectedValues = SerializeHelper.GetDeserializeJson<ProductOptionSelectedValue[]>(productOptionTexts);
			if(poSelectedValues == null) return 0;

			var optionPriceValues = poSelectedValues
				.SelectMany(posv => Regex.Matches(posv.Value, @"{{[0-9,]+}}|\(\+¥[0-9,]+\)")
					.Cast<Match>()
					.Select(m => m.Value.Replace(",", string.Empty)))
				.ToList();

			return CalculateOptionPriceSum(optionPriceValues, @"[0-9,]+");
		}

		/// <summary>
		/// 付帯価格の合計を計算
		/// </summary>
		/// <param name="poSelectedValues">付帯情報の選択値</param>
		/// <param name="pattern">抽出条件</param>
		/// <returns>付帯価格の合計値</returns>
		private static decimal CalculateOptionPriceSum(IEnumerable<string> poSelectedValues, string pattern)
		{
			var result = poSelectedValues
				.Select(productOptionSelectedValue => Regex.Matches(productOptionSelectedValue, pattern))
				.Select(matches => matches.Cast<Match>().Sum(match => ObjectUtility.TryParseDecimal(Regex.Match(match.Value, @"\d+").Value, 0)))
				.Sum();

			return result;
		}

		/// <summary>
		/// 定期購入商品情報DB保存用の商品付帯情報選択値を取得
		/// </summary>
		/// <param name="orderOptionSettingList">付帯情報の設定リスト</param>
		/// <returns>DB保存用の付帯情報選択値</returns>
		/// <remarks>
		/// ※定期購入商品情報（w2_FixedPurchaseItem）はJSON形式です
		/// </remarks>
		public static string GetSelectedOptionSettingForFixedPurchaseItem(ProductOptionSettingList orderOptionSettingList)
		{
			if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false)
			{
				return orderOptionSettingList.GetJsonStringFromSelectValues();
			}

			var selectedOptionSettingJsonText = GetSelectedOptionSettingForOptionPrice(orderOptionSettingList);
			return selectedOptionSettingJsonText;
		}

		/// <summary>
		/// 定期購入商品情報DB保存用の商品付帯情報選択値を取得
		/// </summary>
		/// <param name="orderOptionSettingList">付帯情報の設定リスト</param>
		/// <returns>DB保存用の付帯情報選択値</returns>
		/// <remarks>
		/// ※注文商品情報（w2_OrderItem）の保存形式はオプションによって異なります<br/>
		/// ・付帯価格オプションが有効：JSON形式<br/>
		/// ・付帯価格オプションが無効："項目名：選択値　項目名：選択値…" の形式
		/// </remarks>
		public static string GetSelectedOptionSettingForOrderItem(ProductOptionSettingList orderOptionSettingList)
		{
			if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false)
			{
				return orderOptionSettingList.GetDisplayProductOptionSettingSelectValues();
			}

			var selectedOptionSettingJsonText = GetSelectedOptionSettingForOptionPrice(orderOptionSettingList);
			return selectedOptionSettingJsonText;
		}

		/// <summary>
		/// 付帯価格を考慮したDB保存用の商品付帯情報選択値を取得
		/// </summary>
		/// <param name="orderOptionSettingList">付帯情報の設定リスト</param>
		/// <returns>DB保存用の付帯情報選択値（JSON形式）</returns>
		private static string GetSelectedOptionSettingForOptionPrice(ProductOptionSettingList orderOptionSettingList)
		{
			var cloneItems = orderOptionSettingList.Items.Select(item => item.Clone()).ToList();

			foreach (var settingItems in orderOptionSettingList.Items)
			{
				if ((settingItems.SelectedSettingValue == null)
					|| ((settingItems.IsCheckBoxPrice == false)
						&& (settingItems.IsDropDownPrice == false)))
				{
					continue;
				}

				var selectedValues = GetArrayOfSelectedOptionValues(settingItems.SelectedSettingValue);
				var tempSettingValues = settingItems.OptionPriceSettingValues;
				settingItems.SelectedSettingValue = string.Empty;
				foreach (var valueStr in selectedValues)
				{
					foreach (var lstr in tempSettingValues)
					{
						if ((valueStr.IndexOf('(') == -1) || (lstr.IndexOf('{') == -1)) continue;
						if (valueStr.Substring(0, valueStr.IndexOf('(')) == lstr.Substring(0, lstr.IndexOf('{')))
						{
							settingItems.SelectedSettingValue +=
								lstr + Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE;
						}
					}
				}
			}

			var result = orderOptionSettingList.GetJsonStringFromSelectValues();
			orderOptionSettingList.Items = cloneItems;
			return result;
		}

		/// <summary>
		/// 選択された付帯情報の配列取得
		/// </summary>
		/// <param name="selectedValues">選択された付帯情報</param>
		/// <returns>選択された付帯情報の配列</returns>
		private static IEnumerable<string> GetArrayOfSelectedOptionValues(string selectedValues)
		{
			var result = selectedValues
				.Replace(Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE, "\n")
				.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			return result;
		}

		/// <summary>
		/// オプション価格持っているか
		/// </summary>
		/// <param name="optionTexts">付帯情報テキスト</param>
		/// <returns>TRUE：オプション価格あり；　FALSE：なし</returns>
		public static bool HasOptionPrice(string optionTexts)
		{
			var poSelectedValues = SerializeHelper.GetDeserializeJson<ProductOptionSelectedValue[]>(optionTexts);
			if (poSelectedValues == null) return false;

			var result = poSelectedValues.Any(x=>x.Value.Contains("{{"));
			return result;
		}

		/// <summary>
		/// 表示用付帯価格の文字列作成
		/// </summary>
		/// <param name="cartProduct">カート商品情報</param>
		/// <returns>表示付帯価格</returns>
		public static string ToDisplayProductOptionPrice(CartProduct cartProduct)
		{
			var result = ToDisplayProductOptionPrice(cartProduct.TotalOptionPrice, cartProduct.Price);
			return result;
		}
		/// <summary>
		/// 表示用付帯価格の文字列作成
		/// </summary>
		/// <param name="totalOptionPrice">合計付帯価格</param>
		/// <param name="price">商品単価</param>
		/// <returns>表示付帯価格</returns>
		public static string ToDisplayProductOptionPrice(decimal totalOptionPrice, decimal price)
		{
			var isPriceAboveZero = totalOptionPrice > 0;
			var priceText = CurrencyManager.ToPrice(price);
			var result = isPriceAboveZero
				? string.Format("({0}+{1})", priceText, CurrencyManager.ToPrice(totalOptionPrice))
				: priceText;
			return result;
		}

		/// <summary>
		/// 表示用付帯価格の文字列作成(商品価格区分表示文言あり)
		/// </summary>
		/// <param name="cartProduct">カート商品情報</param>
		/// <param name="productPriceTextPrefix">商品価格区分表示文言</param>
		/// <returns>表示付帯価格</returns>
		public static string ToDisplayProductOptionPriceAndPrefix(CartProduct cartProduct, string productPriceTextPrefix)
		{
			var result = ToDisplayProductOptionPriceAndPrefix(
				cartProduct.Price,
				cartProduct.TotalOptionPrice,
				productPriceTextPrefix,
				cartProduct.ProductOptionSettingList.HasOptionPrice);

			return result;
		}
		/// <summary>
		/// 表示用付帯価格の文字列作成(商品価格区分表示文言あり)
		/// </summary>
		/// <param name="price">商品単価</param>
		/// <param name="totalOptionPrice">合計付帯価格</param>
		/// <param name="productPriceTextPrefix"></param>
		/// <param name="hasOptionPrice">付帯価格を持っているか</param>
		/// <returns>表示付帯価格</returns>
		public static string ToDisplayProductOptionPriceAndPrefix(
			decimal price,
			decimal totalOptionPrice,
			string productPriceTextPrefix,
			bool hasOptionPrice)
		{
			var result = hasOptionPrice
				? string.Format(
					"{0}({2}) + {1}({2})",
					CurrencyManager.ToPrice(price),
					CurrencyManager.ToPrice(totalOptionPrice),
					productPriceTextPrefix)
				: string.Format("{0}({1})", CurrencyManager.ToPrice(price), productPriceTextPrefix);

			return result;
		}

		/// <summary>
		/// カート内にオプション価格を持っているか
		/// </summary>
		/// <param name="cartObject">カートオブジェクト</param>
		/// <returns>オプション価格を持っているか</returns>
		/// <remarks>
		/// カート内でオプション価格を持っている商品があるかを確認<br/>
		/// ※主にカートリストでオプション価格の項目を表示するか？の判定に利用
		/// </remarks>
		public static bool HasProductOptionPriceInCart(CartObject cartObject)
		{
			var result = Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED
				&& ((cartObject.Items.Sum(cp => cp.TotalOptionPrice) != 0)
					|| cartObject.Items.Any(co => co.ProductOptionSettingList.HasOptionPrice));
			return result;
		}
	}
}
