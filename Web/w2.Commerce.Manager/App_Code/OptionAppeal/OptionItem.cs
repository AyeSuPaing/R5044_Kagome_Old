/*
=========================================================================================================
  Module      : オプションアイテムクラス (OptionItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace OptionAppeal
{
	/// <summary>
	/// オプションアイテムクラス
	/// </summary>
	[Serializable]
	public class OptionItem
	{
		/// <summary>カテゴリーID</summary>
		private const string OPTIONAPPEAL_OPTION_CATEGORY_ID = "CategoryId";
		/// <summary>カテゴリーID</summary>
		private const string OPTIONAPPEAL_OPTION_CATEGORY_NAME = "CategoryName";
		/// <summary>ルートカテゴリーID</summary>
		private const string OPTIONAPPEAL_OPTION_PARENT_CATEGORY_ID = "ParentCategoryId";

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OptionItem()
		{
			this.CategoryId = string.Empty;
			this.CategoryName = string.Empty;
			this.ParentCategoryId = string.Empty;
			this.OptionId = string.Empty;
			this.OptionName = string.Empty;
			this.OptionSummary = string.Empty;
			this.OptionDetals = string.Empty;
			this.OptionIconPath = string.Empty;
			this.OptionSupportSiteUrl = string.Empty;
			this.OptionPlan = string.Empty;
			this.OptionAvailable = string.Empty;
			this.OptionInitial = string.Empty;
			this.OptionMonthly = string.Empty;
			this.OptionAncillaryInformation = string.Empty;
			this.OptionEnable = false;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="categoryId">親カテゴリID</param>
		/// <param name="categoryName">親カテゴリ名</param>
		/// <param name="parentCategoryId">ルートカテゴリーID</param>
		/// <param name="optionId">オプションID</param>
		/// <param name="optionName">オプション名</param>
		/// <param name="optionSummary">説明</param>
		/// <param name="optionDetals">詳細</param>
		/// <param name="optionIconPath">アイコン</param>
		/// <param name="optionSupportSiteUrl">サポートサイトURL</param>
		/// <param name="optionPlan">プラン</param>
		/// <param name="optionAvailable">有効性</param>
		/// <param name="optionInitial">初期費用</param>
		/// <param name="optionMonthly">月額</param>
		/// <param name="optionAncillaryInformation">付帯情報</param>
		/// <param name="optionEnable">導入済み判定</param>
		public OptionItem(
			string categoryId,
			string categoryName,
			string parentCategoryId,
			string optionId,
			string optionName,
			string optionSummary,
			string optionDetals,
			string optionIconPath,
			string optionSupportSiteUrl,
			string optionPlan,
			string optionAvailable,
			string optionInitial,
			string optionMonthly,
			string optionAncillaryInformation,
			bool optionEnable)
		{
			this.CategoryId = categoryId;
			this.CategoryName = categoryName;
			this.ParentCategoryId = parentCategoryId;
			this.OptionId = optionId;
			this.OptionName = optionName;
			this.OptionSummary = optionSummary;
			this.OptionDetals = optionDetals;
			this.OptionIconPath = optionIconPath;
			this.OptionSupportSiteUrl = optionSupportSiteUrl;
			this.OptionPlan = optionPlan;
			this.OptionAvailable = optionAvailable;
			this.OptionInitial = optionInitial;
			this.OptionMonthly = optionMonthly;
			this.OptionAncillaryInformation = optionAncillaryInformation;
			this.OptionEnable = optionEnable;
		}

		/// <summary>
		/// オプション情報をベースからバージョンに書き換え
		/// </summary>
		/// <param name="baseOptionItem">ベースオプション情報</param>
		/// <param name="versionOptionItem">バージョン別オプション情報</param>
		/// <returns>オプション情報</returns>
		public List<OptionItem> OverwritFromBaseToVersion(List<OptionItem> baseOptionItem, List<OptionItem> versionOptionItem)
		{
			foreach (var optionItem in baseOptionItem)
			{
				var canVersion = versionOptionItem.FirstOrDefault(p => p.OptionId == optionItem.OptionId);
				if (canVersion == null) continue;

				optionItem.CategoryId = canVersion.CategoryId;
				optionItem.OptionName = canVersion.OptionName;
				optionItem.OptionSummary = canVersion.OptionSummary;
				optionItem.OptionDetals = canVersion.OptionDetals;
				optionItem.OptionIconPath = canVersion.OptionIconPath;
				optionItem.OptionSupportSiteUrl = canVersion.OptionSupportSiteUrl;
				optionItem.OptionPlan = canVersion.OptionPlan;
				optionItem.OptionAvailable = canVersion.OptionAvailable;
				optionItem.OptionInitial = canVersion.OptionInitial;
				optionItem.OptionMonthly = canVersion.OptionMonthly;
				optionItem.OptionAncillaryInformation = canVersion.OptionAncillaryInformation;
			}

			return baseOptionItem;
		}

		/// <summary>親カテゴリーID</summary>
		public string CategoryId { get; set; }
		/// <summary>親カテゴリー名</summary>
		public string CategoryName { get; set; }
		/// <summary>ルートカテゴリーID</summary>
		public string ParentCategoryId { get; set; }
		/// <summary>オプションID</summary>
		public string OptionId { get; set; }
		/// <summary>オプション名</summary>
		public string OptionName { get; set; }
		/// <summary>説明</summary>
		public string OptionSummary { get; set; }
		/// <summary>詳細</summary>
		public string OptionDetals { get; set; }
		/// <summary>アイコン</summary>
		public string OptionIconPath { get; set; }
		/// <summary>サポートサイトURL</summary>
		public string OptionSupportSiteUrl { get; set; }
		/// <summary>プラン</summary>
		public string OptionPlan { get; set; }
		/// <summary>有効性</summary>
		public string OptionAvailable { get; set; }
		/// <summary>初期費用</summary>
		public string OptionInitial { get; set; }
		/// <summary>月額</summary>
		public string OptionMonthly { get; set; }
		/// <summary>付帯情報</summary>
		public string OptionAncillaryInformation { get; set; }
		/// <summary>導入済み判定</summary>
		public bool OptionEnable { get; set; }
	}
}
