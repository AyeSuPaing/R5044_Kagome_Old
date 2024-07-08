/*
=========================================================================================================
  Module      : お知らせワーカーサービス(NewsWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.News;
using w2.Cms.Manager.ViewModels.News;
using w2.Common.Util;
using w2.Domain.News;
using w2.Domain.News.Helper;
using Validator = w2.Common.Util.Validator;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// お知らせワーカーサービス
	/// </summary>
	public class NewsWorkerService : BaseWorkerService
	{
		/// <summary>
		/// リストビューモデル作成
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>ビューモデル</returns>
		public ListViewModel CreateListVm(NewsListParamModel pm)
		{
			pm.Dates.CorrectLastDayOfMonth();
			// 開始日時(From)
			var displayDateFrom = string.Format("{0}/{1}/{2}", pm.Dates.BeginYearFrom, pm.Dates.BeginMonthFrom, pm.Dates.BeginDayFrom);

			// 開始日時(To)
			var displayDateTo = string.Format("{0}/{1}/{2}", pm.Dates.BeginYearTo, pm.Dates.BeginMonthTo, pm.Dates.BeginDayTo);

			// 終了日時(From)
			var displayDateToFrom = string.Format("{0}/{1}/{2}", pm.Dates.EndYearFrom, pm.Dates.EndMonthFrom, pm.Dates.EndDayFrom);

			// 終了日時(To)
			var displayDateToTo = string.Format("{0}/{1}/{2}", pm.Dates.EndYearTo, pm.Dates.EndMonthTo, pm.Dates.EndDayTo);

			var searchCondition = new NewsListSearchCondition
			{
				ShopId = this.SessionWrapper.LoginShopId,
				NewsText = pm.NewsText,
				SortKbn = pm.SortKbn,
				ValidFlg = pm.Valid,
				DispFlg = pm.Disp,
				MobileDispFlg = string.Empty,
				DisplayDateFromFrom = Validator.IsDate(displayDateFrom) ? displayDateFrom : null,
				DisplayDateFromTo = Validator.IsDate(displayDateTo) ? displayDateTo : null,
				DisplayDateToFrom = Validator.IsDate(displayDateToFrom) ? displayDateToFrom : null,
				DisplayDateToTo = Validator.IsDate(displayDateToTo) ? displayDateToTo : null,
				BeginRowNumber = (pm.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
				EndRowNumber = pm.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST
			};
			var total = new NewsService().GetSearchHitCount(searchCondition);
			var list = new NewsService().Search(searchCondition);
			if (pm.Dates.CheckDate() == false)
				return new ListViewModel
				{
					ParamModel = pm,
					ErrorMessage = WebMessages.InvalidDateError
				};
			if (list.Length == 0)
				return new ListViewModel
				{
					ParamModel = pm,
					ErrorMessage = WebMessages.NoHitListError
				};

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				this.SessionWrapper.TranslationSearchCondition = list.Select(r => r.NewsId).ToArray();
				this.SessionWrapper.TranslationExportTargetDataKbn = Database.Common.Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NEWS;
			}

			return new ListViewModel
			{
				ParamModel = pm,
				SearchResultListViewModel =
					list.Select(r => new SearchResultListViewModel(r, this.UrlHelper)).ToArray(),
				PagerHtml = WebPager.CreateDefaultListPager(total,
					pm.PagerNo,
					this.UrlHelper.Action(
						"List",
						Constants.CONTROLLER_W2CMS_MANAGER_NEWS,
						pm))
			};
		}

		/// <summary>
		/// 登録・編集 ビュー作成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="newsId">新着ID</param>
		/// <returns>ビューモデル</returns>
		public RegisterViewModel CreateRegisterVm(ActionStatus actionStatus, string newsId)
		{
			switch (actionStatus)
			{
				case ActionStatus.Update:
					var updateModel = new NewsService().Get(this.SessionWrapper.LoginShopId, newsId);
					if (updateModel != null)
					{
						return new RegisterViewModel(updateModel)
						{
							ActionStatus = actionStatus
						};

					}
					else
					{
						throw new Exception("NewsIdが不正です");
					}

				case ActionStatus.Insert:
					var insertModel = new NewsService().Get(this.SessionWrapper.LoginShopId, newsId);
					if (insertModel != null)
						return new RegisterViewModel(insertModel)
						{
							ActionStatus = actionStatus
						};
					else
						return new RegisterViewModel
						{
							ActionStatus = actionStatus
						};


				default:
					throw new Exception("未対応のactionStatus：" + actionStatus);
			}
		}

		/// <summary>
		/// Top表示更新
		/// </summary>
		/// <param name="newsId">新着ID</param>
		/// <param name="disp">Top表示フラグ</param>
		public void DispFlgModify(string newsId, string disp)
		{
			var dispFlg = false;
			if (bool.TryParse(disp, out dispFlg) == false) return;

			new NewsService().UpdateDispFlg(
				this.SessionWrapper.LoginShopId,
				newsId,
				dispFlg
					? Database.Common.Constants.FLG_NEWS_DISP_FLG_ALL
					: Database.Common.Constants.FLG_NEWS_DISP_FLG_LIST);

			// フロント系サイトを最新情報へ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.News).CreateUpdateRefreshFile();
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="newsId">新着ID</param>
		public void Delete(string newsId)
		{
			// 削除
			new NewsService().Delete(this.SessionWrapper.LoginShopId, newsId);

			// フロント系サイトを最新情報へ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.News).CreateUpdateRefreshFile();
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="input">お知らせ 入力情報</param>
		/// <returns>ビューモデル</returns>
		public RegisterViewModel Update(NewsInput input)
		{
			var model = input.CreateModel();
			model.ShopId = this.SessionWrapper.LoginShopId;
			model.LastChanged = this.SessionWrapper.LoginOperatorName;

			var newsService = new NewsService();
			newsService.Update(model);

			// フロント系サイトを最新情報へ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.News).CreateUpdateRefreshFile();

			var newModel = newsService.Get(this.SessionWrapper.LoginShopId, model.NewsId);
			var vm = new RegisterViewModel(newModel)
			{
				ActionStatus = ActionStatus.Update,
				UpdateInsertSuccessFlg = true
			};
			return vm;
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="input">お知らせ 入力情報</param>
		/// <returns>ビューモデル</returns>
		public RegisterViewModel Insert(NewsInput input)
		{
			input.NewsId = NumberingUtility.CreateKeyId(
				this.SessionWrapper.LoginShopId,
				Constants.NUMBER_KEY_NEWS_ID,
				10);

			var model = input.CreateModel();
			model.ShopId = this.SessionWrapper.LoginShopId;
			model.LastChanged = this.SessionWrapper.LoginOperatorName;

			var newsService = new NewsService();
			newsService.Insert(model);
			newsService.UpdateDisplayOrder(this.SessionWrapper.LoginShopId, model.NewsId);

			// フロント系サイトを最新情報へ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.News).CreateUpdateRefreshFile();

			var newModel = newsService.Get(this.SessionWrapper.LoginShopId, model.NewsId);
			var vm = new RegisterViewModel(newModel)
			{
				ActionStatus = ActionStatus.Update,
				UpdateInsertSuccessFlg = true
			};
			return vm;
		}
	}
}