/*
=========================================================================================================
  Module      : タグマネージャー リストViewモデル(TagManagerListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Affiliate;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.ParamModels.TagManager;
using w2.Common.Util;
using w2.Domain.AdvCode;
using w2.Domain.Affiliate.Helper;

namespace w2.Cms.Manager.ViewModels.TagManager
{
	/// <summary>
	/// タグマネージャー リストViewモデル
	/// </summary>
	public class TagManagerListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TagManagerListViewModel()
		{
			this.TagManagerSearchResultListViewModel = new TagManagerSearchResultListViewModel[0];
			this.ParamModel = new TagManagerListParamModel();
			this.AffiliateKbnItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_AFFILIATETAGSETTING, Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_KBN)
				.Select(s => new SelectListItem
				{
					Value = s.Value,
					Text = s.Text
				}).ToArray();
			this.ValidFlgItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_AFFILIATETAGSETTING, Constants.FIELD_AFFILIATETAGSETTING_VALID_FLG)
				.Select(s => new SelectListItem
				{
					Value = s.Value,
					Text = s.Text
				}).ToArray();
			this.PageItems = TagSetting.GetInstance().Setting.TargetPages
				.Select(s => new SelectListItem
				{
					Value = s.Path,
					Text = s.Name
				}).ToArray();
			this.AdvCodeMediaType = new AdvCodeService().GetAdvCodeMediaTypeListAll()
				.Select(s => new SelectListItem
				{
					Value = s.AdvcodeMediaTypeId,
					Text = string.Format(
							"{0}(ID:{1})",
							s.AdvcodeMediaTypeName,
							s.AdvcodeMediaTypeId)
				}).ToArray();
		}

		/// <summary>検索 パラメタモデル</summary>
		public TagManagerListParamModel ParamModel { get; set; }
		/// <summary>検索結果 Viewモデル</summary>
		public TagManagerSearchResultListViewModel[] TagManagerSearchResultListViewModel { get; set; }
		/// <summary>表示区分 ドロップダウンリスト</summary>
		public SelectListItem[] AffiliateKbnItems { get; set; }
		/// <summary>設置箇所 ドロップダウンリスト</summary>
		public SelectListItem[] PageItems { get; set; }
		/// <summary>有効フラグ ドロップダウンリスト</summary>
		public SelectListItem[] ValidFlgItems { get; set; }
		/// <summary>広告媒体区分 ドロップダウンリスト</summary>
		public SelectListItem[] AdvCodeMediaType { get; set; }
		/// <summary>ページャHTML</summary>
		public string PagerHtml { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}

	/// <summary>
	/// タグマネージャー検索結果 Viewモデル
	/// </summary>
	public class TagManagerSearchResultListViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="result">タグ検索結果</param>
		/// <param name="urlHelper">URLヘルパー</param>
		public TagManagerSearchResultListViewModel(AffiliateTagSettingListSearchResult result, UrlHelper urlHelper)
		{
			this.AffiliateId = result.AffiliateId.ToString();
			this.AffiliateName = result.AffiliateName;
			this.AffiliateTag1 = result.AffiliateTag1;
			this.TagName = result.TagName;
			this.TagContent = result.TagContent;
			this.TagDelimiter = result.TagDelimiter;
			this.AffiliateKbn = ValueText.GetValueText(Constants.TABLE_AFFILIATETAGSETTING, Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_KBN, result.AffiliateKbn);
			this.PageConditions = GetConditionPageNames(result.PageConditions);
			this.OutputLocation = ValueText.GetValueText(Constants.TABLE_AFFILIATETAGSETTING, Constants.FIELD_AFFILIATETAGSETTING_OUTPUT_LOCATION, result.OutputLocation);

			this.AdcodeMediaTypeConditions = result.AdcodeMediaTypeConditions;
			this.AdvertisementCodeConditions = result.AdvertisementCodeConditions;
			this.ProductIdConditions = result.ProductIdConditions;

			this.DisplayOrder = result.DisplayOrder.ToString();
			this.ValidFlg = ValueText.GetValueText(Constants.TABLE_AFFILIATETAGSETTING, Constants.FIELD_AFFILIATETAGSETTING_VALID_FLG, result.ValidFlg);

			this.RegisterUrl = urlHelper.Action(
				"Register",
				Constants.CONTROLLER_W2CMS_MANAGER_TAG_MANAGER,
				new
				{
					ActionStatus = ActionStatus.Update,
					AffiliateId = this.AffiliateId,
				});
		}

		/// <summary>
		/// 条件ページパスからページ名称を取得
		/// </summary>
		/// <param name="conditionPages">ページパス</param>
		/// <returns>ページ名称</returns>
		private string GetConditionPageNames(string conditionPages)
		{
			var values = conditionPages.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			var result = values
				.Select(s => TagSetting.GetInstance().Setting.TargetPages.FirstOrDefault(t => t.Path == s))
				.Where(m => m != null).Select(m => m.Name).ToArray();
			var resultValue = (result.Length > 0) 
				? string.Join(Environment.NewLine, result)
				: ValueText.GetValueText(Constants.TABLE_AFFILIATETAGSETTING, "all_page", string.Empty);
			return resultValue;
		}

		/// <summary>タグID</summary>
		public string AffiliateId { get; set; }
		/// <summary>タグ名称</summary>
		public string AffiliateName { get; set; }
		/// <summary>タグ内容</summary>
		public string AffiliateTag1 { get; set; }
		/// <summary>商品タグ名称</summary>
		public string TagName { get; set; }
		/// <summary>商品タグ内容</summary>
		public string TagContent { get; set; }
		/// <summary>商品タグ区切り文字</summary>
		public string TagDelimiter { get; set; }
		/// <summary>表示区分</summary>
		public string AffiliateKbn { get; set; }
		/// <summary>設置箇所条件</summary>
		public string PageConditions { get; set; }
		/// <summary>出力箇所</summary>
		public string OutputLocation { get; set; }
		/// <summary>広告媒体区分 条件</summary>
		public string AdcodeMediaTypeConditions { get; set; }
		/// <summary>広告コード 条件</summary>
		public string AdvertisementCodeConditions { get; set; }
		/// <summary>商品ID 条件</summary>
		public string ProductIdConditions { get; set; }
		/// <summary>表示順</summary>
		public string DisplayOrder { get; set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
		/// <summary>登録URL</summary>
		public string RegisterUrl { get; set; }
	}
}