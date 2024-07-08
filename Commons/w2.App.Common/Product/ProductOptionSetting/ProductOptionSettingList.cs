/*
=========================================================================================================
  Module      : 商品付帯情報一覧クラス(ProductOptionSettingList.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order;
using w2.App.Common.Web.Page;
using w2.Common.Helper;
using w2.Common.Util;
using w2.Domain.Product;
using w2.Domain.Product.Helper;

namespace w2.App.Common.Product
{
	///*********************************************************************************************
	/// <summary>
	/// 商品付帯情報一覧クラス
	/// </summary>
	///*********************************************************************************************
	[Serializable]
	public class ProductOptionSettingList : IEnumerable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductOptionSettingList()
		{
			this.Items = new List<ProductOptionSetting>();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strProductOptionSettings">商品付帯情報設定文字列</param>
		public ProductOptionSettingList(string strProductOptionSettings)
			: this()
		{
			SetProductOptionSettingAll(strProductOptionSettings);
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		public ProductOptionSettingList(string strShopId, string strProductId)
			: this()
		{
			GetProductInfoAndSetProductOptionSettingAll(strShopId, strProductId);
		}

		/// <summary>
		/// IEnumerable.GetEnumerator()の実装
		/// </summary>
		/// <returns>IEnumerator</returns>
		public IEnumerator GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		/// <summary>
		/// 商品付帯情報追加
		/// </summary>
		/// <param name="pos">商品付帯情報</param>
		public void Add(ProductOptionSetting pos)
		{
			this.Items.Add(pos);
		}

		/// <summary>
		/// <para>商品付帯情報文字列取得</para>
		/// <para>リストに格納されている全ての商品付帯情報を文字列に変換する</para>
		/// </summary>
		public string GetProductOptionValuesString()
		{
			// 保持している商品付帯情報を１つの文字列に結合
			StringBuilder sbProductOptionValues = new StringBuilder();
			foreach(ProductOptionSetting pov in this.Items)
			{
				sbProductOptionValues.Append(pov.GetProductOptionSettingString());
			}

			return sbProductOptionValues.ToString();
		}

		/// <summary>
		/// <para>商品付帯情報名取得</para>
		/// <para>商品付帯情報一覧の特定のインデックスの商品付帯情報名を返却</para>
		/// <para>（インデックスエラーの場合は空文字を返却）</para>
		/// </summary>
		/// <param name="iNo">インデックス</param>
		/// <param name="blIsMobile">モバイルか？</param>
		/// <returns>商品付帯情報名</returns>
		public string GetProductOptionSettingValueName(int iNo, bool blIsMobile)
		{
			if (IsExsitProductOptionSetting(iNo) == false)
			{
				return "";
			}

			return (blIsMobile) ? StringUtility.ToHankakuKatakana(this.Items[iNo].ValueName) : this.Items[iNo].ValueName;
		}

		/// <summary>
		/// <para>商品付帯情報表示区分取得</para>
		/// <para>商品付帯情報一覧の特定のインデックスの表示区分を返却</para>
		/// <para>（インデックスエラーの場合はから文字を返却）</para>
		/// </summary>
		/// <param name="iNo">インデックス</param>
		/// <returns>表示区分 C：チェックボックス、S：セレクトメニュー</returns>
		public string GetProductOptionSettingDispKbn(int iNo)
		{
			if (IsExsitProductOptionSetting(iNo) == false)
			{
				return "";
			}

			return this.Items[iNo].GetProductOptionSettingDisplayKbn();
		}

		/// <summary>
		/// <para>商品付帯情報設定値取得</para>
		/// <para>商品付帯情報一覧の特定のインデックスの設定値取得をカンマ区切りで返却</para>
		/// <para>（インデックスエラーの場合は空文字を返却）</para>
		/// </summary>
		/// <param name="iNo">インデックス</param>
		/// <returns>商品付帯情報設定値取得</returns>
		public string GetProductOptionSettingValue(int iNo)
		{
			if (IsExsitProductOptionSetting(iNo) == false)
			{
				return "";
			}

			return this.Items[iNo].GetProductOptionSettingValue();
		}

		/// <summary>
		/// 商品付帯情報設定一括設定
		/// </summary>
		/// <param name="strProductOptionSettings">商品付帯情報設定一覧文字列</param>
		public void SetProductOptionSettingAll(string strProductOptionSettings)
		{
			var pattern = new Regex(Constants.REGEX_PATTERN_PRODUCT_OPTION_BASIC);
			var mc = pattern.Matches(strProductOptionSettings);

			int iLoop = 0;
			foreach(Match match in mc)
			{
				iLoop++;
				if (iLoop > Constants.PRODUCTOPTIONVALUES_MAX_COUNT)
				{
					break;
				}

				this.Items.Add(new ProductOptionSetting(match.Value
					.Replace(Constants.PRODUCT_OPTION_PREFIX_CHARACTER, string.Empty)
					.Replace(Constants.PRODUCT_OPTION_TERMINATING_CHARACTER, string.Empty)));
			}
		}

		/// <summary>
		/// 商品付帯情報選択値全取得（表示用）
		/// </summary>
		/// <remarks>
		/// "項目名1：選択値　項目名2：選択値…" の形式
		/// </remarks>
		public string GetDisplayProductOptionSettingSelectValues()
		{
			var sbSelectValues = new StringBuilder();
			foreach (var pos in this.Items)
			{
				if (pos.IsSelectedProductOptionValue && (sbSelectValues.Length != 0))
				{
					sbSelectValues.Append("　");
				}

				sbSelectValues.Append(pos.GetDisplayProductOptionSettingSelectValue());
			}

			return sbSelectValues.ToString();
		}

		/// <summary>
		/// 商品付帯情報選択値取得（JSON形式）
		/// </summary>
		/// <param name="includesEmptySelection">選択値が空の付帯情報も含めるか</param>
		/// <returns>商品付帯情報選択値</returns>
		public string GetJsonStringFromSelectValues(bool includesEmptySelection = false)
		{
			var selectedProductOptionValues = this.Items.Select(pos => pos.GetProductOptionSelectedValue(includesEmptySelection))
				.Where(pos => (string.IsNullOrEmpty(pos.Value) == false) || includesEmptySelection)
				.ToArray();
			var json = (selectedProductOptionValues.Length > 0)
				? JsonConvert.SerializeObject(selectedProductOptionValues)
				: string.Empty;

			return json;
		}

		/// <summary>
		/// <para>商品付帯情報htmlタグ出力（任意）</para>
		/// <para>商品付帯情報設定一覧の任意の設定のhtmlタグを出力</para>
		/// <para>表示形式を自動で判別し、チェックボックス、セレクトメニュータグを出力</para>
		/// <para>任意の商品付帯情報が存在しない場合、空文字を返却</para>
		/// </summary>
		/// <param name="iNo">商品付帯情報番号</param>
		/// <param name="blIsMobile">モバイルか？</param>
		/// <returns>商品付帯情報htmlタグ</returns>
		public string GetHtmlAnyProductOptionSetting(int iNo, bool blIsMobile)
		{
			if (IsExsitProductOptionSetting(iNo) == false)
			{
				return "";
			}

			return this.Items[iNo].GetHtmlProductOptionSetting(iNo, blIsMobile);
		}

		/// <summary>
		/// 商品付帯情報存在確認
		/// </summary>
		/// <param name="iNo">インデックス</param>
		private bool IsExsitProductOptionSetting(int iNo)
		{
			return ((this.Items.Count) >= (iNo + 1));
		}

		/// <summary>
		/// <para>商品付帯情報選択値一括取得</para>
		/// <para>商品付帯情報から</para>
		/// </summary>
		/// <returns>商品付帯情報選択値（全て）</returns>
		public string GetProductOptionSettingSelectedValues()
		{
			StringBuilder sbSelectedValues = new StringBuilder();
			int iLoop = 0;
			foreach(ProductOptionSetting pos in this.Items)
			{
				if (iLoop != 0)
				{
					sbSelectedValues.Append(Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SETTING_VALUE);
				}

				sbSelectedValues.Append(StringUtility.ToEmpty(pos.SelectedSettingValue));

				iLoop++;
			}

			return sbSelectedValues.ToString();
		}

		/// <summary>
		/// <para>商品情報取得及び商品付帯情報一括設定</para>
		/// <para>店舗ID、商品IDを元に商品情報を取得し、取得した商品情報の商品付帯情報カラムから</para>
		/// <para>商品付帯情報一覧を作成する</para>
		/// </summary>
		/// <returns>商品付帯情報選択値（全て）</returns>
		public void GetProductInfoAndSetProductOptionSettingAll(string strShopId, string strProductId)
		{
			// 商品情報取得
			DataView dvProduct = ProductCommon.GetProductInfoUnuseMemberRankPrice(strShopId, strProductId);

			// 商品情報が取得できなかった場合は処理を行わない
			if (dvProduct.Count == 0)
			{
				return;
			}

			SetProductOptionSettingAll((string)dvProduct[0][Constants.FIELD_PRODUCT_PRODUCT_OPTION_SETTINGS]);
		}

		/// <summary>
		/// 商品付帯情報選択値から商品付帯情報のデフォルト値を設定
		/// </summary>
		/// <param name="productOptionTexts">商品付帯情報選択値</param>
		/// <param name="itemsToAddAsSettings">存在しない項目を追加設定として登録するか</param>
		/// <param name="onlySelectedItems">現在選択されている項目のみ取得するか</param>
		/// <remarks>
		/// 商品付帯情報選択値：新形式もしくは旧型式で入ってくる点に注意<br/>
		/// ・新形式：JSON形式<br/>
		/// ・旧型式："項目名：選択値　項目名：選択値…"
		/// </remarks>
		public void SetDefaultValueFromProductOptionTexts(
			string productOptionTexts,
			bool itemsToAddAsSettings = false,
			bool onlySelectedItems = false)
		{
			// データ処理用クラスに変換
			var productSelectedValues = GetProductOptionSelectedValues(productOptionTexts);

			SetDefaultValue(productSelectedValues);

			if (itemsToAddAsSettings) UpdateAndAddProductOptions(productSelectedValues);

			// 選択値がある付帯情報のみ抽出
			if (onlySelectedItems)
			{
				this.Items = this.Items.Where(item => string.IsNullOrEmpty(item.SelectedSettingValue) == false).ToList();
			}
		}

		/// <summary>
		/// 商品付帯情報選択値配列からデフォルト値を設定
		/// </summary>
		/// <param name="productOptionSelectedValues">商品付帯情報選択値配列</param>
		private void SetDefaultValue(ProductOptionSelectedValue[] productOptionSelectedValues)
		{
			foreach (var posItem in this.Items)
			{
				foreach (var poSelectedValue in productOptionSelectedValues)
				{
					// NOTE: 付帯情報設定の"項目名が同一"で"選択値情報が異なる"ケースが存在する為、全ての選択値を確認する。
					var isSetDefaultValue = SetSelectedValueToDefaultValue(posItem, poSelectedValue);
				}
			}

			// 商品付帯情報の選択値をもとに商品付帯情報設定のデフォルト値をセット
			bool SetSelectedValueToDefaultValue(ProductOptionSetting setting, ProductOptionSelectedValue selectedProductOption)
			{
				// 項目名が一致しないものは無視する
				if (setting.ValueName != selectedProductOption.Name) return false;

				// 入力欄の表示区分により、デフォルト値を設定
				switch (setting.DisplayKbn)
				{
					case Constants.PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX:
						setting.DefaultValue = selectedProductOption.Value;
						setting.SelectedSettingValue = StringUtility.ToEmpty(selectedProductOption.Value);
						return true;

					case Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX:
					case Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX:
						if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED
							&& (Regex.IsMatch(selectedProductOption.Value, Constants.REGEX_PATTERN_OPTION_PRICE_EXPRESSION_RAW)
								|| Regex.IsMatch(selectedProductOption.Value, Constants.REGEX_PATTERN_OPTION_PRICE_EXPRESSION)))
						{
							var selectedValueList = new List<string>();
							if (Regex.IsMatch(selectedProductOption.Value, Constants.REGEX_PATTERN_OPTION_PRICE_EXPRESSION_RAW))
							{
								// 商品付帯情報選択値の内容からデフォルト値を設定する
								// NOTE: 商品付帯情報選択値の表示区分がチェックボックス(価格も)の場合は選択値が複数になる為、選択値のリストを作成する

								if ((selectedProductOption.Type == Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX)
									|| (selectedProductOption.Type == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX))
								{
									// 商品付帯情報の選択値から選択値のリストを作成する
									// NOTE: オプション価格の選択値は "○○{{1000}},○○{{2000}}…"の形式で保存されるため
									// 複数ある選択値毎の終端 ")," でリスト分割している
									selectedValueList = selectedProductOption.Value.Replace(
										Constants.PRODUCT_OPTION_PRICE_SUFFIX_FOR_DB + ",",
										Constants.PRODUCT_OPTION_PRICE_SUFFIX_FOR_DB + "\n")
										.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
								}
								
								// 選択値のリストから商品付帯情報のデフォルト値をセットする
								foreach (var selectedValue in selectedValueList)
								{
									foreach (var settingValue in setting.OptionPriceSettingValues)
									{
										if (settingValue == selectedValue)
										{
											var price = 0m;
											var settingValueForDisplay = Regex.Replace(settingValue, Constants.REGEX_PATTERN_OPTION_PRICE_SEARCH_PATTERN, match =>
											{
												price = ObjectUtility.TryParseDecimal(Regex.Match(match.Value, @"[0-9]+").Value, 0);
												return "(+" + CurrencyManager.ToPrice(price) + ")";
											});
											setting.SelectedSettingValue += settingValueForDisplay + Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE;
										}
									}
								}
							}
							else
							{
								if ((selectedProductOption.Type == Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX)
									|| (selectedProductOption.Type == Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX))
								{
									// 商品付帯情報の選択値から選択値のリストを作成する
									// NOTE: オプション価格の選択値は "○○(+\1,000),○○(+\2,000)…"の形式で保存されるため
									// 複数ある選択値毎の終端 ")," でリスト分割している
									selectedValueList = selectedProductOption.Value.Replace("),", ")" + "\n")
										.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
								}

								// 選択値のリストから商品付帯情報のデフォルト値をセットする
								foreach (var selectedValue in selectedValueList)
								{
									foreach (var settingSettingValue in setting.SettingValues)
									{
										if (IsValueEqual(settingSettingValue, selectedValue))
										{
											setting.SelectedSettingValue += settingSettingValue + Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE;
										}
									}
								}
							}
							return true;
						}
						else
						{
							setting.SelectedSettingValue = (selectedProductOption.Type == Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX)
								? selectedProductOption.Value.Replace(",", Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE)
								: selectedProductOption.Value;
							return true;
						}

					case Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU:
					case Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_DROPDOWNMENU:
						if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED
							&& (Regex.IsMatch(selectedProductOption.Value, Constants.REGEX_PATTERN_OPTION_PRICE_EXPRESSION_RAW)
								|| Regex.IsMatch(selectedProductOption.Value, Constants.REGEX_PATTERN_OPTION_PRICE_EXPRESSION)))
						{
							if (Regex.IsMatch(selectedProductOption.Value, Constants.REGEX_PATTERN_OPTION_PRICE_EXPRESSION_RAW))
							{
								// 商品付帯情報選択値の内容からデフォルト値を設定する
								// ドロップダウン形式は選択値は単一の為、チェックボックスの様に分割してリスト化はしない
								foreach (var settingValue in setting.OptionPriceSettingValues)
								{
									if (settingValue == selectedProductOption.Value)
									{
										var price = 0m;
										var settingValueForDisplay = Regex.Replace(settingValue, Constants.REGEX_PATTERN_OPTION_PRICE_SEARCH_PATTERN, match =>
										{
											price = ObjectUtility.TryParseDecimal(Regex.Match(match.Value, @"[0-9]+").Value, 0);
											return "(+" + CurrencyManager.ToPrice(price) + ")";
										});

										setting.SelectedSettingValue += settingValueForDisplay;
										return true;
									}
								}
							}
							else
							{
								// 商品付帯情報選択値の内容からデフォルト値を設定する
								// ドロップダウン形式は選択値は単一の為、チェックボックスの様に分割してリスト化はしない
								foreach (var settingSettingValue in setting.SettingValues)
								{
									if (IsValueEqual(settingSettingValue, selectedProductOption.Value))
									{
										setting.SelectedSettingValue += settingSettingValue;
										return true;
									}
								}
							}
						}
						else
						{
							if (setting.SettingValues.Any(settingValue => settingValue.Equals(selectedProductOption.Value)))
							{
								setting.SelectedSettingValue = selectedProductOption.Value;
								return true;
							}
						}
						break;
				}

				return false;
			}
		}

		/// <summary>
		/// 付帯情報設定値と選択された設定値比較
		/// </summary>
		/// <param name="itemSettingValue">設定値</param>
		/// <param name="selectedSettingValue">商品付帯情報選択値</param>
		/// <returns>TRUE：同じ；　FALSE：異なる値</returns>
		private bool IsValueEqual(string itemSettingValue, string selectedSettingValue)
		{
			var result = itemSettingValue.Contains("(")
				&& selectedSettingValue.Contains("(")
				&& itemSettingValue.Substring(0, itemSettingValue.IndexOf("(")).Equals(selectedSettingValue.Substring(0, selectedSettingValue.IndexOf("(")));
			return result;
		}

		/// <summary>
		/// 最新の付帯情報と比較し、マッチする項目を更新し、新規項目を追加
		/// </summary>
		/// <param name="latestProductOptions">比較対象となる最新の付帯リスト</param>
		public void UpdateAndAddProductOptions(ProductOptionSelectedValue[] latestProductOptions)
		{
			var matchedIndices = new List<int>();
			var unmatchedProductOptions = new List<ProductOptionSetting>();
			int targetIndex = 0;

			if (this.Items.Any() == false)
			{
				foreach (var item in latestProductOptions)
				{
					var newItem = CreateUnmatchedProductOption(item, targetIndex + 1);
					unmatchedProductOptions.Add(newItem);
					targetIndex++;
				}
			}
			else
			{
				foreach (var item in latestProductOptions)
				{
					bool isMatched = false;

					for (int i = 0; i < this.Items.Count; i++)
					{
						if (matchedIndices.Contains(i)) continue;

						isMatched = CheckAndUpdateMatchingOption(this.Items[i], item, matchedIndices, i);

						if (isMatched) break;
					}

					if (isMatched == false)
					{
						var newItem = CreateUnmatchedProductOption(item, targetIndex + 1);
						unmatchedProductOptions.Add(newItem);
					}

					targetIndex++;
				}
			}

			this.Items.AddRange(unmatchedProductOptions);
		}

		/// <summary>
		/// 現在のカートとDBの商品付帯情報を比較する
		/// </summary>
		/// <param name="bindingCartList">バインドカートリスト</param>
		/// <returns>チェックボックス判定結果</returns>
		public bool CheckCartListToBindingProductOptionSettingList(List<CartObject> bindingCartList)
		{
			// 商品付帯情報チェック
			foreach (var bindingCart in bindingCartList.Select((v, i) => new { Value = v, Index = i }))
			{
				foreach (var bindingCartItem in bindingCart.Value.Items)
				{
					if (bindingCartItem.IsOrderCombine
						&& (string.IsNullOrEmpty(bindingCartItem.OrderCombineOrgOrderId) == false)) continue;

					var matchedIndices = new List<int>();
					var productOptionSettingList = new ProductOptionSettingList();
					var bindingCartProductOptionSettingList = bindingCartItem.ProductOptionSettingList;

					var latestProduct = new ProductService().Get(bindingCartItem.ShopId, bindingCartItem.ProductId);
					var latestProductOptionSettingList = ProductOptionSettingHelper
						.GetProductOptionSettingList(
							latestProduct.ShopId,
							latestProduct.ProductId,
							latestProduct.ProductOptionSettings);

					if (bindingCartProductOptionSettingList.Items.Count != latestProductOptionSettingList.Items.Count)
					{
						return false;
					}

					var productOptionTexts = bindingCartProductOptionSettingList.GetJsonStringFromSelectValues(includesEmptySelection:true);

					// データ処理用クラスに変換
					var productOptionSelectedValues = bindingCartProductOptionSettingList
						.GetProductOptionSelectedValues(productOptionTexts);

					// 商品付帯情報内容変更チェック
					foreach (var productOptionSelectedValue in productOptionSelectedValues)
					{
						var isMatched = false;

						for (int i = 0; i < latestProductOptionSettingList.Items.Count; i++)
						{
							if (matchedIndices.Contains(i)) continue;

							var latestProductOptionSetting = latestProductOptionSettingList.Items[i];

							isMatched = productOptionSettingList
								.CheckAndUpdateMatchingOption(
									latestProductOptionSetting,
									productOptionSelectedValue,
									matchedIndices,
									i);

							if (isMatched) break;

							return false;
						}
					}

					// 商品付帯情報必須チェック
					for (int i = 0; i < latestProductOptionSettingList.Items.Count; i++)
					{
						var latestProductOptionSettingItem = latestProductOptionSettingList.Items[i];

						// 商品付帯価格は必須チェック考慮外のため処理をを飛ばす
						if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED
							&& latestProductOptionSettingItem.IsDropDownPrice
							|| latestProductOptionSettingItem.IsCheckBoxPrice)
						{
							continue;
						}
							

						if (latestProductOptionSettingItem.IsNecessary)
						{
							switch (latestProductOptionSettingItem.DisplayKbn)
							{
								case Constants.PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX:
									if (string.IsNullOrEmpty(bindingCartProductOptionSettingList.Items[i].SelectedSettingValueForTextBox))
									{
										return false;
									}
									break;

								case Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX:
								case Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU:
									if (string.IsNullOrEmpty(bindingCartProductOptionSettingList.Items[i].SelectedSettingValue))
									{
										return false;
									}
									break;
							}
						}
					}
				}
			}
			return true;
		}

		/// <summary>
		/// ProductOptionSetting項目がマッチするか確認し、更新
		/// </summary>
		/// <param name="currentItem">最新のオプション</param>
		/// <param name="comparisonItem">比較対象のオプション</param>
		/// <param name="matchedIndices">既にマッチ済みのインデックスリスト</param>
		/// <param name="index">インデックス</param>
		/// <returns>マッチした場合はtrue、それ以外の場合はfalseを返却</returns>
		public bool CheckAndUpdateMatchingOption(ProductOptionSetting currentItem, ProductOptionSelectedValue comparisonItem, List<int> matchedIndices, int index)
		{
			switch (currentItem.DisplayKbn)
			{
				case "":
					return false;

				case Constants.PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX:
					if (comparisonItem.Name == currentItem.ValueName)
					{
						matchedIndices.Add(index);
						return true;
					}
					return false;

				case Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX:
				case Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX:
					if (comparisonItem.Name == currentItem.ValueName)
					{
						// チェックボックスはテキストボックスと同様に選択が任意なため、空文字は一致とみなす
						if (string.IsNullOrEmpty(comparisonItem.Value))
						{
							matchedIndices.Add(index);
							return true;
						}

						List<string> selectedValueList;
						if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED
							&& (Regex.IsMatch(comparisonItem.Value, Constants.REGEX_PATTERN_OPTION_PRICE_EXPRESSION_RAW)))
						{
							selectedValueList = comparisonItem.Value.Replace(
								Constants.PRODUCT_OPTION_PRICE_SUFFIX_FOR_DB + ",",
								Constants.PRODUCT_OPTION_PRICE_SUFFIX_FOR_DB + "\n")
									.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

							var isMatchAll = true;
							foreach (var settingValue in selectedValueList)
							{
								if (currentItem.OptionPriceSettingValues.Contains(settingValue)) continue;
								isMatchAll = false;
							}

							if (isMatchAll)
							{
								matchedIndices.Add(index);
							}

							return isMatchAll;
						}
						else
						{
							selectedValueList = comparisonItem.Value.Split(',').ToList();
						}

						if (selectedValueList.Except(currentItem.SettingValues).Any() == false)
						{
							matchedIndices.Add(index);
							return true;
						}
					}
					return false;

				case Constants.PRODUCTOPTIONVALUES_DISP_KBN_SELECTMENU:
				case Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_DROPDOWNMENU:
					if (comparisonItem.Name == currentItem.ValueName)
					{
						if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED
							&& Regex.IsMatch(comparisonItem.Value, Constants.REGEX_PATTERN_OPTION_PRICE_EXPRESSION_RAW))
						{
							foreach (var settingValue in currentItem.OptionPriceSettingValues)
							{
								if (settingValue == comparisonItem.Value)
								{
									matchedIndices.Add(index);
									return true;
								}
							}
						}
						else if (currentItem.SettingValues.Contains(comparisonItem.Value))
						{
							matchedIndices.Add(index);
							return true;
						}
					}
					return false;

				default:
					return false;
			}
		}

		/// <summary>
		/// マッチしない現在の項目に対して新しいProductOptionSetting項目を作成
		/// </summary>
		/// <param name="currentItem">付帯オプション</param>
		/// <param name="nameIndex">項目名インデックス</param>
		/// <returns>新しいProductOptionSetting項目</returns>
		private ProductOptionSetting CreateUnmatchedProductOption(ProductOptionSelectedValue currentItem, int nameIndex)
		{
			var newItem = new ProductOptionSetting
			{
				DisplayKbn = Constants.PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX,
				SelectedSettingValue = string.IsNullOrEmpty(currentItem.Value)
					? currentItem.Name
					: currentItem.Value,
				ValueName = string.IsNullOrEmpty(currentItem.Name)
					? CommonPage.ReplaceTag("@@DispText.product_option_name.name@@") + nameIndex
					: currentItem.Name,
				MatchesLatestProductMaster = false
			};
			return newItem;
		}

		/// <summary>
		/// 商品付帯情報テキストから商品付帯情報選択値を取得する
		/// </summary>
		/// <param name="productOptionTexts">商品付帯情報選択値</param>
		/// <returns>チェックボックス判定結果</returns>
		/// <remarks>
		/// バージョン毎にDBに保存されている付帯情報の形式が異なるため、扱いやすいクラスへ変換
		/// </remarks>
		public ProductOptionSelectedValue[] GetProductOptionSelectedValues(string productOptionTexts)
		{
			// JSONに変換できるかチェック
			var productSelectedValues = SerializeHelper.GetDeserializeJson<ProductOptionSelectedValue[]>(productOptionTexts);

			if (productSelectedValues == null)
			{
				// （旧型式 or フォーマット無視）
				var productOptionValues = productOptionTexts.Split(new[] { "　" }, StringSplitOptions.RemoveEmptyEntries);
				productSelectedValues = productOptionValues.Select(
					po =>
					{
						// 項目名と選択値を取得（"項目名：選択値"）
						var keyValue = po.Split(new[] { "：" }, StringSplitOptions.RemoveEmptyEntries);
						var hasNameAndValue = keyValue.Length > 1;
						var isCheckBox = hasNameAndValue ? GetIsCheckBox(keyValue[1]) : false;

						return new ProductOptionSelectedValue
						{
							Name = keyValue[0],
							Type = isCheckBox ? Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX : Constants.PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX,
							Value = hasNameAndValue ? keyValue[1] : string.Empty
						};
					}).ToArray();
			}

			return productSelectedValues;
		}

		/// <summary>
		/// 付帯情報がチェックボックス形式かを判定する
		/// </summary>
		/// <param name="selectedValue">付帯情報選値</param>
		/// <returns>チェックボックス判定結果</returns>
		public bool GetIsCheckBox(string selectedValue)
		{
			// NOTE: チェックボックスは選択値毎にカンマ区切りの形式で保持するが、
			// オプション価格には "," が含まれており、カンマを使用しての分割は不可になる。
			// その為、")" で分割し選択値が複数存在するかを判定する
			var isCheckBox = Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED && selectedValue.Contains(")")
				? selectedValue.Split(')').Length > 1
				: selectedValue.Split(',').Length > 1;
			return isCheckBox;
		}

		/// <summary>商品付帯情報リスト</summary>
		public List<ProductOptionSetting> Items { get; set; }
		/// <summary>商品付帯情報が選択されているか</summary>
		public bool IsSelectedProductOptionValueAll
		{
			get
			{
				foreach(ProductOptionSetting pos in this.Items)
				{
					if (pos.IsSelectedProductOptionValue)
					{
						return true;
					}
				}

				return false;
			}
		}
		/// <returns>オプション価格</returns>
		public decimal SelectedOptionTotalPrice
		{
			get
			{
				return this.Items.Sum(productOptionSetting => productOptionSetting.GetPriceOfSelectedValue());
			}
		}
		/// <returns>オプション価格があるか</returns>
		public bool HasOptionPrice
		{
			get
			{
				return this.Items.Any(productOptionSetting => productOptionSetting.HasOptionPrice());
			}
		}
	}
}
