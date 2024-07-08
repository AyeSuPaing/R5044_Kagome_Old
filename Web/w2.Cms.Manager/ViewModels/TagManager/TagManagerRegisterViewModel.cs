/*
=========================================================================================================
  Module      : タグマネージャー 登録Viewモデル(TagManagerRegisterViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using w2.App.Common.Affiliate;
using w2.App.Common.Manager;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Util;
using w2.Cms.Manager.Input;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.Affiliate;
using w2.Domain.MenuAuthority.Helper;
using w2.Domain.ShopOperator;

namespace w2.Cms.Manager.ViewModels.TagManager
{
	/// <summary>
	/// タグマネージャー 登録Viewモデル
	/// </summary>
	public class TagManagerRegisterViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TagManagerRegisterViewModel(ShopOperatorModel shopOperator)
		{
			Init(shopOperator);
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">タグ設定内容 Inputモデル</param>
		/// <param name="affiliateTagConditionModels">タグ表示条件 Inputモデル</param>
		/// <param name="shopOperator">ログインオペレータ</param>
		public TagManagerRegisterViewModel(
			AffiliateTagSettingModel model,
			AffiliateTagConditionModel[] affiliateTagConditionModels,
			ShopOperatorModel shopOperator)
			: this(shopOperator)
		{
			this.Input = new TagManagerInput(model);
			this.ConditionInput = new TagManagerConditionInput(this.Input.AffiliateId, affiliateTagConditionModels);

			var usableLocations = this.ShopOperator.GetUsableOutputLocationsArray();
			this.Input.Pages = TagSetting.GetInstance().Setting.TargetPages
				.Where(
					page => ((usableLocations.Any() == false)
						|| usableLocations.Contains(page.Path)))
				.Select(
					page => new TargetPageChackBoxModel
					{
						Name = page.Name,
						Path = page.Path,
						ActionType = page.ActionType,
						Logging = page.Logging,
						IsCheck = affiliateTagConditionModels.Any(
							m => ((m.ConditionType == Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PAGE)
								&& (m.ConditionValue == page.Path))),
					})
				.ToList();

			if (string.IsNullOrEmpty(
				affiliateTagConditionModels.FirstOrDefault(
					condition => (condition.ConditionType == Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PAGE)).ConditionValue))
			{
				this.Input.IsAllPageCheck = true;
			}

			if (this.Input.IsAllPageCheck)
			{
				this.ReplaseTagDescriptionList = new ReplaceTagManager()
					.SelectPageReplaceTagList(TagSetting.ACTION_TYPE_ALL)
					.Select(
						m => new ListItem
						{
							Value = m,
							Text = ValueText.GetValueText("AffiliateTag", "ReplaseTagDescription", m)
						})
					.ToArray();
			}
			else
			{
				this.ReplaseTagDescriptionList = new ReplaceTagManager()
					.SelectPageReplaceTagList(
						this.Input.Pages
							.Where(m => m.IsCheck)
							.Select(m => m.Path)
							.ToList())
					.Select(
						m => new ListItem
						{
							Value = m,
							Text = ValueText.GetValueText("AffiliateTag", "ReplaseTagDescription", m)
						})
					.ToArray();
			}

			this.ProductTagViewModel = new ProductTagViewModel(model.AffiliateProductTagId.ToString());
		}

		/// <summary>
		/// 初期設定
		/// </summary>
		private void Init(ShopOperatorModel shopOperator)
		{
			this.Input = new TagManagerInput();
			this.ShopOperator = shopOperator;
			this.ConditionInput = new TagManagerConditionInput();
			this.ReplaseTagDescriptionList = new ListItem[0];
			this.ProductTagViewModel = new ProductTagViewModel(string.Empty);

			this.OutputLocationItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_AFFILIATETAGSETTING, Constants.FIELD_AFFILIATETAGSETTING_OUTPUT_LOCATION)
				.Select(
					location => new SelectListItem
					{
						Value = location.Value,
						Text = location.Text
					})
				.ToArray();

			this.AffiliateKbnItems = ValueTextForCms.GetValueSelectListItems(Constants.TABLE_AFFILIATETAGSETTING, Constants.FIELD_AFFILIATETAGSETTING_AFFILIATE_KBN)
				.Select(
					affiliateKbn => new SelectListItem
					{
						Value = affiliateKbn.Value,
						Text = affiliateKbn.Text
					})
				.ToArray();

			var usableLocations = this.ShopOperator.GetUsableOutputLocationsArray();
			this.Input.Pages = TagSetting.GetInstance().Setting.TargetPages
				.Where(
					page => ((usableLocations.Any() == false)
						|| usableLocations.Contains(page.Path)))
				.Select(
					ts => new TargetPageChackBoxModel
					{
						Name = ts.Name,
						Path = ts.Path,
						ActionType = ts.ActionType,
						Logging = ts.Logging,
						IsCheck = false
					})
				.ToList();

			this.ProductSearchUrl = SingleSignOnUrlCreator.CreateForMvc(
				MenuAuthorityHelper.ManagerSiteType.Ec,
				new UrlCreator(Constants.PAGE_MANAGER_PRODUCT_SEARCH)
					.AddParam(Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN, Constants.KBN_PRODUCT_SEARCH_PRODUCT)
					.AddParam(Constants.REQUEST_KEY_PRODUCT_VALID_FLG, Constants.FLG_PRODUCT_VALID_FLG_VALID)
					.CreateUrl());
		}

		/// <summary>
		/// 全ページ出力可能か判定
		/// </summary>
		/// <returns>全ページ出力可能か</returns>
		public bool IsAllowAllLocations()
		{
			if (this.ShopOperator.GetUsableOutputLocationsArray().Any() == false) return true;

			var isAllowAllLocations = TagSetting.GetInstance().Setting.TargetPages.All(
				targetPage => this.ShopOperator.GetUsableOutputLocationsArray().Contains(targetPage.Path));
			return isAllowAllLocations;
		}

		/// <summary>タグ入力内容</summary>
		public TagManagerInput Input { get; set; }
		/// <summary>タグ条件入力内容</summary>
		public TagManagerConditionInput ConditionInput { get; set; }
		/// <summary>商品タグViewモデル</summary>
		public ProductTagViewModel ProductTagViewModel { get; set; }
		/// <summary>出力箇所ドロップダウンリスト</summary>
		public SelectListItem[] OutputLocationItems { get; set; }
		/// <summary>表示区分ドロップダウンリスト</summary>
		public SelectListItem[] AffiliateKbnItems { get; set; }
		/// <summary>置換タグ表示内容</summary>
		public ListItem[] ReplaseTagDescriptionList { get; set; }
		/// <summary>更新・登録 成功フラグ</summary>
		public bool UpdateInsertSuccessFlg { get; set; }
		/// <summary>商品検索ポップアップウィンドウURL</summary>
		public string ProductSearchUrl { get; set; }
		/// <summary>オペレータ</summary>
		public ShopOperatorModel ShopOperator { get; set; }
	}

	/// <summary>
	/// 出力箇所 選択状況管理クラス
	/// </summary>
	public class TargetPageChackBoxModel : TargetPage
	{
		/// <summary>選択状況</summary>
		public bool IsCheck { get; set; }
	}

	/// <summary>
	/// 商品タグViewモデル
	/// </summary>
	public class ProductTagViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="affiliateProductTagId">商品タグID</param>
		public ProductTagViewModel(string affiliateProductTagId)
		{
			if (string.IsNullOrEmpty(affiliateProductTagId) == false)
			{
				this.ProductTag = new AffiliateTagSettingService().AffiliateProductTagSettingGet(int.Parse(affiliateProductTagId));
				if (this.ProductTag != null)
				{
					this.AffiliateProductTagId = this.ProductTag.AffiliateProductTagId.ToString();
				}
				else
				{
					this.ProductTag = new AffiliateProductTagSettingModel();
				}
			}
			else
			{
				this.ProductTag = new AffiliateProductTagSettingModel();

			}

			this.AffiliateProductTagIdItems = new AffiliateTagSettingService().AffiliateProductTagSettingGetAll()
				.Select(s => new SelectListItem
				{
					Value = s.AffiliateProductTagId.ToString(),
					Text = s.TagName
				}).ToArray();
		}

		/// <summary>商品タグモデル</summary>
		public AffiliateProductTagSettingModel ProductTag { get; set; }
		/// <summary>商品タグ選択肢ドロップダウンリスト</summary>
		public SelectListItem[] AffiliateProductTagIdItems { get; set; }
		/// <summary>商品タグID</summary>
		public string AffiliateProductTagId { get; set; }
	}
}