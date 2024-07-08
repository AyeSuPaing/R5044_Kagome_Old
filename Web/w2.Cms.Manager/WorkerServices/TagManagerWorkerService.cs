/*
=========================================================================================================
  Module      : タグマネージャー ワーカーサービス(TagManagerWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Text;
using w2.App.Common.RefreshFileManager;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.TagManager;
using w2.Cms.Manager.ViewModels.TagManager;
using w2.Common.Extensions;
using w2.Domain;
using w2.Domain.Affiliate;
using w2.Domain.Affiliate.Helper;
using w2.Domain.ShopOperator;
using Constants = w2.Cms.Manager.Codes.Constants;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// タグマネージャー ワーカーサービス
	/// </summary>
	public class TagManagerWorkerService : BaseWorkerService
	{
		/// <summary>
		/// 検索用ListViewモデル作成
		/// </summary>
		/// <param name="pm">検索パラメタモデル</param>
		/// <returns>Viewモデル</returns>
		public TagManagerListViewModel CreateListVm(TagManagerListParamModel pm)
		{
			var searchCondition = new AffiliateTagSettingListSearchCondition
			{
				AffiliateName = pm.AffiliateName,
				AffiliateKbn = pm.AffiliateKbn,
				ValidFlg = pm.ValidFlg,
				Page = pm.Page,
				ProductId = pm.SearchProductId,
				AdvertisementCod = pm.SearchAdvertisementCode,
				AdvcodeMediaTypeId = pm.AdvCodeMediaType,
				BeginRowNumber = (pm.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
				EndRowNumber = pm.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST
			};

			var loginOperator = new ShopOperatorService().Get(this.SessionWrapper.LoginShopId, this.SessionWrapper.LoginOperator.OperatorId);
			var usableAffiliateTags = loginOperator.GetUsableAffiliateTagIdsArray();
			var usableMediaTypes = loginOperator.GetUsableAdvcodeMediaTypeIdsArray();
			var usableLocations = loginOperator.GetUsableOutputLocationsArray();

			var totalCount = DomainFacade.Instance.AffiliateTagSettingService.GetSearchHitCount(
				searchCondition,
				usableAffiliateTags,
				usableMediaTypes,
				usableLocations);

			var searchResults = DomainFacade.Instance.AffiliateTagSettingService.Search(
				searchCondition,
				usableAffiliateTags,
				usableMediaTypes,
				usableLocations);

			if (searchResults.Any() == false)
			{
				return new TagManagerListViewModel
				{
					ParamModel = pm,
					ErrorMessage = WebMessages.NoHitListError
				};
			}

			return new TagManagerListViewModel
			{
				ParamModel = pm,
				TagManagerSearchResultListViewModel = searchResults
					.Select(r => new TagManagerSearchResultListViewModel(r, this.UrlHelper))
					.ToArray(),
				PagerHtml = WebPager.CreateDefaultListPager(
					totalCount,
					pm.PagerNo,
					this.UrlHelper.Action(
						"List",
						Constants.CONTROLLER_W2CMS_MANAGER_TAG_MANAGER,
						pm))
			};
		}

		/// <summary>
		/// 表示Viewモデル生成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="affiliateId">アフィリエイトタグID</param>
		/// <returns>Viewモデル</returns>
		public TagManagerRegisterViewModel CreateRegisterVmDetail(ActionStatus actionStatus, string affiliateId)
		{
			switch (actionStatus)
			{
				case ActionStatus.Update:
					var updateModel = new AffiliateTagSettingService().Get(int.Parse(affiliateId));
					if (updateModel != null)
					{
						var affiliateTagConditionModels =
							new AffiliateTagSettingService().AffiliateTagConditionGetAllByAffiliateId(
								int.Parse(affiliateId));
						return new TagManagerRegisterViewModel(updateModel, affiliateTagConditionModels, this.SessionWrapper.LoginOperator)
						{
							ActionStatus = actionStatus
						};

					}
					return null;

				case ActionStatus.Insert:
					int affiliateIdNum;
					if (int.TryParse(affiliateId, out affiliateIdNum))
					{
						var insertModel = new AffiliateTagSettingService().Get(affiliateIdNum);
						if (insertModel != null)
						{
							var affiliateTagConditionModels =
								new AffiliateTagSettingService().AffiliateTagConditionGetAllByAffiliateId(
									affiliateIdNum);
							return new TagManagerRegisterViewModel(insertModel, affiliateTagConditionModels, this.SessionWrapper.LoginOperator)
							{
								ActionStatus = actionStatus
							};
						}
					}

					return new TagManagerRegisterViewModel(this.SessionWrapper.LoginOperator)
					{
						ActionStatus = actionStatus,
					};


				default:
					throw new Exception("未対応のactionStatus：" + actionStatus);
			}
		}

		/// <summary>
		/// 更新Viewモデル
		/// </summary>
		/// <param name="input">タグ入力内容</param>
		/// <param name="conditionInput">タグ表示条件 入力内容</param>
		/// <returns>Viewモデル</returns>
		public TagManagerRegisterViewModel CreateRegisterVmUpdate(TagManagerInput input, TagManagerConditionInput conditionInput)
		{
			var model = input.CreateModel();
			model.LastChanged = this.SessionWrapper.LoginOperatorName;

			var affiliateTagSettingService = new AffiliateTagSettingService();
			affiliateTagSettingService.Update(model);

			string conditionValuesPage;
			var usableLocations = this.SessionWrapper.LoginOperator.GetUsableOutputLocationsArray();
			if (input.IsAllPageCheck)
			{
				conditionValuesPage = string.Empty;
			}
			else if (usableLocations.Any())
			{
				var beforeSettings = affiliateTagSettingService.AffiliateTagConditionGetAllByAffiliateId(model.AffiliateId);
				conditionValuesPage = beforeSettings
					.Where(condition => (condition.IsModifiablePageCondition(usableLocations) == false))
					.Select(condition => condition.ConditionValue)
					.Union(
						input.Pages.Where(page => page.IsCheck).Select(page => page.Path))
					.JoinToString(Environment.NewLine);
			}
			else
			{
				conditionValuesPage = string.Join(Environment.NewLine, input.Pages.Where(i => i.IsCheck).Select(i => i.Path).ToArray());
			}
			conditionInput.AffiliateId = model.AffiliateId.ToString();
			conditionInput.ConditionValuesPage = conditionValuesPage;

			affiliateTagSettingService.AffiliateTagConditionInsertAll(
				conditionInput.CreateModels().ToArray());

			// アフィリエイトタグ設定のキャッシュ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.AffiliateTagSetting).CreateUpdateRefreshFile();

			var newModel = affiliateTagSettingService.Get(model.AffiliateId);
			var affiliateTagConditionModels = affiliateTagSettingService.AffiliateTagConditionGetAllByAffiliateId(model.AffiliateId);
			var vm = new TagManagerRegisterViewModel(newModel, affiliateTagConditionModels, this.SessionWrapper.LoginOperator)
			{
				ActionStatus = ActionStatus.Update,
				UpdateInsertSuccessFlg = true
			};
			return vm;
		}


		/// <summary>
		/// 登録Viewモデル
		/// </summary>
		/// <param name="input">タグ入力内容</param>
		/// <param name="conditionInput">タグ表示条件 入力内容</param>
		/// <returns>Viewモデル</returns>
		public TagManagerRegisterViewModel CreateRegisterVmInsert(TagManagerInput input, TagManagerConditionInput conditionInput)
		{
			var model = input.CreateModel();
			model.LastChanged = this.SessionWrapper.LoginOperatorName;
			var affiliateId = new AffiliateTagSettingService().Insert(model);

			// オペレータの閲覧可能なタグID更新
			var service = new ShopOperatorService();
			var ht = new Hashtable
			{
				{ Constants.FIELD_SHOPOPERATOR_SHOP_ID, this.SessionWrapper.LoginShopId },
				{ Constants.FIELD_SHOPOPERATOR_LAST_CHANGED, this.SessionWrapper.LoginOperatorName },
				{ Constants.FIELD_SHOPOPERATOR_OPERATOR_ID, this.SessionWrapper.LoginOperator.OperatorId },
			};
			var operatorInfo = service.Get(
				(string)ht[Constants.FIELD_SHOPOPERATOR_SHOP_ID],
				(string)ht[Constants.FIELD_SHOPOPERATOR_OPERATOR_ID]
				);
			if (string.IsNullOrEmpty(operatorInfo.UsableAffiliateTagIdsInReport) == false)
			{
				ht.Add(
					Constants.FIELD_SHOPOPERATOR_USABLE_AFFILIATE_TAG_IDS_IN_REPORT,
					string.Format("{0},{1}", operatorInfo.UsableAffiliateTagIdsInReport, affiliateId)
					);
				ht.Add(
					Constants.FIELD_SHOPOPERATOR_USABLE_ADVCODE_MEDIA_TYPE_IDS,
					operatorInfo.UsableAdvcodeMediaTypeIds);
				ht.Add(
					Constants.FIELD_SHOPOPERATOR_USABLE_OUTPUT_LOCATIONS,
					operatorInfo.UsableOutputLocations);
				service.UpdateOperatorTagAuthority(ht);
			}

			conditionInput.AffiliateId = affiliateId.ToString();
			conditionInput.ConditionValuesPage = (input.IsAllPageCheck)
				? string.Empty
				: string.Join(Environment.NewLine, input.Pages.Where(i => i.IsCheck).Select(i => i.Path).ToArray());

			new AffiliateTagSettingService().AffiliateTagConditionInsertAll(
				conditionInput.CreateModels().ToArray());

			// アフィリエイトタグ設定のキャッシュ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.AffiliateTagSetting).CreateUpdateRefreshFile();

			var newModel = new AffiliateTagSettingService().Get(affiliateId);
			var affiliateTagConditionModels =
				new AffiliateTagSettingService().AffiliateTagConditionGetAllByAffiliateId(affiliateId);
			var vm = new TagManagerRegisterViewModel(newModel, affiliateTagConditionModels, this.SessionWrapper.LoginOperator)
			{
				ActionStatus = ActionStatus.Update,
				UpdateInsertSuccessFlg = true
			};
			return vm;
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="affiliateId">アフィリエイトタグID</param>
		public void Delete(string affiliateId)
		{
			new AffiliateTagSettingService().Delete(int.Parse(affiliateId));
			new AffiliateTagSettingService().AffiliateTagConditionDeleteAll(int.Parse(affiliateId));
			new AffiliateService().DeleteByTagId(int.Parse(affiliateId));

			// アフィリエイトタグ設定のキャッシュ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.AffiliateTagSetting).CreateUpdateRefreshFile();
		}

		/// <summary>
		/// 入力内容エラーチェック
		/// </summary>
		/// <param name="input">タグ入力内容</param>
		/// <returns>エラーメッセージ</returns>
		public StringBuilder ErrorCheck(TagManagerInput input)
		{
			var errorMessages = new StringBuilder();
			errorMessages.Append(input.Validate());

			if ((input.IsAllPageCheck == false) && (input.Pages.Any(i => i.IsCheck) == false))
			{
				errorMessages.Append(WebMessages.TagManagerSelectPageError);
			}

			return errorMessages;
		}
	}
}
