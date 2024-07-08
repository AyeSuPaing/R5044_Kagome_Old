/*
=========================================================================================================
  Module      : オプションxml読み込みクラス (OptionXmlReader.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using w2.App.Common.OptionAppeal;

namespace OptionAppeal
{
	/// <summary>
	/// オプションxml読み込みクラス
	/// </summary>
	[Serializable]
	public class OptionXmlReader
	{
		/// <summary> オプション訴求機能：有効性フラグ </summary>
		private static string OPTIONAPPEAL_FLG = "TRUE";
		/// <summary> オプション訴求機能：プラン標準機能 </summary>
		private static string OPTIONAPPEAL_INITIAL_DEFAULT = "標準";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OptionXmlReader()
		{
			this.BaseOption = string.Empty;
			this.VersionOption = string.Empty;
			this.BaseSlider = string.Empty;
			this.PlanSlider = string.Empty;
			this.ProjectSlider = string.Empty;
			this.OptionCategoryList = new List<OptionCategory>();
			this.ContainIntroducedOptionCategoryList = new List<OptionCategory>();
			this.OptionList = new List<OptionItem>();
			this.OptionSliderList = new List<OptionSlider>();
			this.PopularOptionSliderList = new List<PopularOptionSlider>();
		}

		/// <summary>
		/// 読み込み
		/// </summary>
		/// <returns>xml情報</returns>
		public OptionXmlReader Read()
		{
			var optionContentsGetter = new OptionContentsGetter();
			// ベースオプション情報取得
			this.BaseOption = optionContentsGetter.GetOptionBase();

			// バージョン別オプション情報取得
			this.VersionOption = optionContentsGetter.GetOptionVersion(Constants.OPTIONAPPEAL_PROJECT_OPTION_VERSION);

			// スライダーベース取得
			this.BaseSlider = optionContentsGetter.GetSliderBase();

			// プラン別スライダー取得
			if (string.IsNullOrEmpty(Constants.OPTIONAPPEAL_PROJECT_PLAN) == false)
			{
				this.PlanSlider = optionContentsGetter.GetSliderPlan(Constants.OPTIONAPPEAL_PROJECT_PLAN);
			}

			// 案件別スライダー取得
			if (string.IsNullOrEmpty(Constants.OPTIONAPPEAL_PROJECT_NO) == false)
			{
				this.ProjectSlider = optionContentsGetter.GetSliderProject(Constants.OPTIONAPPEAL_PROJECT_NO);
			}

			// オプション情報取得
			GetXmlData();
			
			// スライダー情報取得
			GetSliderData();

			return this;
		}

		/// <summary>
		/// XMLカテゴリーデータ取得
		/// </summary>
		private void GetXmlData()
		{
			var xml = this.BaseOption;
			var optionXDocument = XDocument.Parse(xml);
			var categories = optionXDocument.Descendants("Categories");

			// オプション情報取得
			GetOptions();

			foreach (var categorieNode in categories.Nodes())
			{
				var xElements = XElement.Parse(categorieNode.ToString());
				var categoryId = xElements.FirstAttribute;
				var categoryIconPath = xElements.Element(Constants.OPTIONAPPEAL_CATEGORY_ICON).Value;
				var innerOptions = OptionIconChange(
					this.OptionList.Where(
						p => p.CategoryId == categoryId.Value && p.OptionAvailable == OPTIONAPPEAL_FLG).ToList(),
					categoryIconPath);
				if (innerOptions.Any() == false) continue;

				this.ContainIntroducedOptionCategoryList.Add(
					new OptionCategory()
					{
						CategoryId = categoryId.Value,
						CategoryName = xElements.Element(Constants.OPTIONAPPEAL_CATEGORY_NAME).Value,
						CategoryParent = xElements.Element(Constants.OPTIONAPPEAL_CATEGORY_PARENT).Value,
						CategoryIcon = categoryIconPath,
						Options = innerOptions
					});

				innerOptions = innerOptions.Where(p => p.OptionEnable == false).ToList();
				if (innerOptions.Any() == false) continue;

				this.OptionCategoryList.Add(
					new OptionCategory()
				{
					CategoryId = categoryId.Value,
					CategoryName = xElements.Element(Constants.OPTIONAPPEAL_CATEGORY_NAME).Value,
					CategoryParent = xElements.Element(Constants.OPTIONAPPEAL_CATEGORY_PARENT).Value,
					CategoryIcon = categoryIconPath,
					Options = innerOptions
				});
			}
		}

		/// <summary>
		/// XMLオプションデータ取得
		/// </summary>
		private void GetOptions()
		{
			var optionItemList = GetOptionItems(this.BaseOption);
			var versionOptionItemList = GetOptionItems(this.VersionOption);

			// オプション情報をベースからバージョンに書き換え
			optionItemList = new OptionItem().OverwritFromBaseToVersion(optionItemList, versionOptionItemList);

			this.OptionList = optionItemList.Where(p => p.OptionInitial != OPTIONAPPEAL_INITIAL_DEFAULT).ToList();
		}

		/// <summary>
		/// オプションデータ取得
		/// </summary>
		/// <param name="xml">xml</param>
		/// <returns>オプションデータ</returns>
		private List<OptionItem> GetOptionItems(string xml)
		{
			var optionXDocument = XDocument.Parse(xml);
			var options = optionXDocument.Descendants("Options");
			var usedOptionList = Constants.OPTIONAPPEAL_PROJECT_USED_OPTIONS.Split(',');
			var optionItemList = new List<OptionItem>();
			foreach (var optionNode in options.Nodes())
			{
				var optionXElement = XElement.Parse(optionNode.ToString());
				var priceEachPlan = optionXElement.Descendants(Constants.OPTIONAPPEAL_OPTION_PRICE)
					.Descendants(Constants.OPTIONAPPEAL_PROJECT_PLAN);
				var priceEachPlanXElement = XElement.Parse(priceEachPlan.Last().ToString());
				var priceInCategory = priceEachPlanXElement.Descendants("Category");
				var priceInCategoryXElement = XElement.Parse(priceInCategory.Last().ToString());

				var categoryId = priceInCategoryXElement.Attribute(Constants.OPTIONAPPEAL_CATEGORY_ID).Value;
				var optionId = optionXElement.FirstAttribute.Value;
				optionItemList.Add(
					new OptionItem
					{
						CategoryId = categoryId,
						OptionId = optionId,
						OptionName = optionXElement.Element(Constants.OPTIONAPPEAL_OPTION_NAME).Value,
						OptionSummary = optionXElement.Element(Constants.OPTIONAPPEAL_OPTION_SUMMARY).Value,
						OptionDetals = WebSanitizer.HtmlEncodeChangeToBr(optionXElement.Element(Constants.OPTIONAPPEAL_OPTION_DETAILS).Value),
						OptionIconPath = optionXElement.Element(Constants.OPTIONAPPEAL_OPTION_ICON_PATH).Value,
						OptionSupportSiteUrl = optionXElement.Element(Constants.OPTIONAPPEAL_OPTION_SUPPORTSITE_URL).Value,
						OptionPlan = Constants.OPTIONAPPEAL_PROJECT_PLAN,
						OptionAvailable = priceEachPlanXElement.Element(Constants.OPTIONAPPEAL_OPTION_AVAILABLE).Value,
						OptionInitial = priceInCategoryXElement.Element(Constants.OPTIONAPPEAL_OPTION_INITIAL).Value,
						OptionMonthly = WebSanitizer.HtmlEncodeChangeToBr(priceInCategoryXElement.Element(Constants.OPTIONAPPEAL_OPTION_MONTHLY).Value),
						OptionAncillaryInformation = WebSanitizer.HtmlEncodeChangeToBr(priceInCategoryXElement.Element(Constants.OPTIONAPPEAL_OPTION_ANCILLARYINFORMATION).Value),
						OptionEnable = (usedOptionList.Contains(optionId))
					});
			}

			return optionItemList;
		}

		/// <summary>
		/// オプションアイコン変更
		/// </summary>
		/// <param name="options">オプション一覧</param>
		/// <param name="categoryIconPath">カテゴリーアイコンパス</param>
		/// <returns>変更後のオプション一覧</returns>
		private List<OptionItem> OptionIconChange(List<OptionItem> options, string categoryIconPath)
		{
			foreach (var option in options.Where(option => string.IsNullOrEmpty(option.OptionIconPath)))
			{
				option.OptionIconPath = categoryIconPath;
			}

			return options;
		}

		/// <summary>
		/// スライダー取得
		/// </summary>
		private void GetSliderData()
		{
			var baseOptionSliderList = GetOptionSliders(this.BaseSlider);
			var planOptionSliderList = GetOptionSliders(this.PlanSlider);
			var projectOptionSliderList = GetOptionSliders(this.ProjectSlider);

			var basePopularOptionSliderList = GetPopularOptionSliders(this.BaseSlider);
			var planPopularOptionSliderList = GetPopularOptionSliders(this.PlanSlider);
			var projectPopularOptionSliderList = GetPopularOptionSliders(this.ProjectSlider);

			var optionSlider = new OptionSlider();
			var popularOptionSlider = new PopularOptionSlider();
			// スライダー情報をベースからプランに書き換え
			baseOptionSliderList = optionSlider.ChangeOptionSliders(baseOptionSliderList, planOptionSliderList);
			basePopularOptionSliderList = popularOptionSlider.ChangePopularOptionSliders(basePopularOptionSliderList, planPopularOptionSliderList);

			// スライダー情報をベースから案件に書き換え
			baseOptionSliderList = optionSlider.ChangeOptionSliders(baseOptionSliderList, projectOptionSliderList);
			basePopularOptionSliderList = popularOptionSlider.ChangePopularOptionSliders(basePopularOptionSliderList, projectPopularOptionSliderList);

			this.OptionSliderList = baseOptionSliderList
				.Where(p => (p.Visible == OPTIONAPPEAL_FLG)
					&& (string.IsNullOrEmpty(p.ImagePath) == false)).ToList();
			this.PopularOptionSliderList = basePopularOptionSliderList
				.Where(p => (p.Visible == OPTIONAPPEAL_FLG)
					&& (string.IsNullOrEmpty(p.ImagePath) == false)).ToList();
		}

		/// <summary>
		/// オプションスライダー取得
		/// </summary>
		/// <param name="xml">xml</param>
		/// <returns>スライダー情報</returns>
		private List<OptionSlider> GetOptionSliders(string xml)
		{
			var optionSliderList = new List<OptionSlider>();
			if (string.IsNullOrEmpty(xml)) return optionSliderList;
			var sliderXElements = XDocument.Parse(xml).Descendants("Sliders");
			foreach (var node in sliderXElements.Nodes())
			{
				var slider = XElement.Parse(node.ToString());
				var sliderId = slider.FirstAttribute;
				optionSliderList.Add(
					new OptionSlider()
					{
						SliderId = sliderId.Value,
						ImagePath = slider.Element(Constants.OPTIONAPPEAL_SLIDER_IMAGE_PATH).Value,
						OptionId = slider.Element(Constants.OPTIONAPPEAL_SLIDER_OPTION_ID).Value,
						Visible = slider.Element(Constants.OPTIONAPPEAL_SLIDER_VISIBLE).Value
					});
			}

			return optionSliderList;
		}

		/// <summary>
		/// 人気オプションスライダー取得
		/// </summary>
		/// <param name="xml">xml</param>
		/// <returns>スライダー情報</returns>
		private List<PopularOptionSlider> GetPopularOptionSliders(string xml)
		{
			var popularOptionSliderList = new List<PopularOptionSlider>();
			if (string.IsNullOrEmpty(xml)) return popularOptionSliderList;
			var popularSliderXDocument = XDocument.Parse(xml).Descendants("PopularOptions");
			foreach (var node in popularSliderXDocument.Nodes())
			{
				var slider = XElement.Parse(node.ToString());
				var sliderId = slider.FirstAttribute;
				popularOptionSliderList.Add(
					new PopularOptionSlider()
					{
						SliderId = sliderId.Value,
						ImagePath = slider.Element(Constants.OPTIONAPPEAL_POPULAR_SLIDER_IMAGE_PATH).Value,
						OptionId = slider.Element(Constants.OPTIONAPPEAL_POPULAR_SLIDER_OPTION_ID).Value,
						Visible = slider.Element(Constants.OPTIONAPPEAL_POPULAR_SLIDER_VISIBLE).Value
					});
			}

			return popularOptionSliderList;
		}

		/// <summary> カテゴリ一覧 </summary>
		public List<OptionCategory> OptionCategoryList { get; set; }
		/// <summary> カテゴリ一覧（導入済みを含む） </summary>
		public List<OptionCategory> ContainIntroducedOptionCategoryList { get; set; }
		/// <summary> オプション一覧 </summary>
		public List<OptionItem> OptionList { get; set; }
		/// <summary> スライダー一覧 </summary>
		public List<OptionSlider> OptionSliderList { get; set; }
		/// <summary> 人気スライダー一覧 </summary>
		public List<PopularOptionSlider> PopularOptionSliderList { get; set; }
		/// <summary> ベースオプション情報 </summary>
		public string BaseOption { get; set; }
		/// <summary> バージョンオプション情報 </summary>
		public string VersionOption { get; set; }
		/// <summary> ベーススライダー情報 </summary>
		public string BaseSlider { get; set; }
		/// <summary> プラン別スライダー情報 </summary>
		public string PlanSlider { get; set; }
		/// <summary> 案件別スライダー情報 </summary>
		public string ProjectSlider { get; set; }
	}
}