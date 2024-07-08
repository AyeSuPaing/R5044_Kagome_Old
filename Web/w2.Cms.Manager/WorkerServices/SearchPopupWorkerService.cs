/*
=========================================================================================================
  Module      : 検索用ポップアップ ワーカーサービス(SearchPopupWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.ParamModels.SearchPopup;
using w2.Cms.Manager.ViewModels.SearchPopup;
using w2.Domain.AdvCode;
using w2.Domain.AdvCode.Helper;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// 検索用ポップアップ ワーカーサービス
	/// </summary>
	public class SearchPopupWorkerService : BaseWorkerService
	{
		/// <summary>
		/// 広告媒体区分 検索用ListView生成
		/// </summary>
		/// <param name="pm">検索 パラメタモデル</param>
		/// <returns>Viewモデル</returns>
		public AdCodeMediaTypeSearchListViewModel CreateAdCodeMediaTypeSearchListVm(AdCodeMediaTypeSearchListParamModel pm)
		{
			var searchCondition = new AdvCodeMediaTypeListSearchCondition
			{
				AdvcodeMediaTypeId = pm.AdvCodeMediaTypeId,
				AdvcodeMediaTypeName = pm.AdvCodeMediaTypeName,
				SortKbn = pm.SortKbn,
				BeginRowNumber = (pm.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
				EndRowNumber = pm.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST,
				UsableMediaTypeIds = this.SessionWrapper.LoginOperator.UsableAdvcodeMediaTypeIds,
			};

			var total = new AdvCodeService().GetAdvCodeMediaTypeSearchHitCount(searchCondition);
			var list = new AdvCodeService().SearchAdvCodeMediaType(searchCondition);
			if (list.Length == 0)
			{
				return new AdCodeMediaTypeSearchListViewModel
				{
					ParamModel = pm,
					ErrorMessage = WebMessages.NoHitListError
				};
			}
			return new AdCodeMediaTypeSearchListViewModel
			{
				ParamModel = pm,
				AdCodeMediaTypeSearchResultListViewModel = list
					.Select(r => new AdCodeMediaTypeSearchResultListViewModel(r)).ToArray(),
				PagerHtml = WebPager.CreateDefaultListPager(total,
					pm.PagerNo,
					this.UrlHelper.Action(
						"AdCodeMediaTypeSearchList",
						Constants.CONTROLLER_W2CMS_MANAGER_SEARCH_POPUP,
						pm))
			};
		}

		/// <summary>
		/// 広告コード 検索用ListView生成
		/// </summary>
		/// <param name="pm">検索 パラメタモデル</param>
		/// <returns>Viewモデル</returns>
		public AdCodeSearchListViewModel CreateAdCodeSearchListVm(AdCodeSearchListParamModel pm)
		{
			var searchCondition = new AdvCodeListSearchCondition
			{
				DeptId = "0",
				AdvcodeMediaTypeId = pm.AdvCodeMediaType,
				AdvertisementCode = pm.AdvCode,
				MediaName = pm.MediaName,
				SortKbn = pm.SortKbn,
				ValidFlg = string.Empty,
				BeginRowNumber = (pm.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
				EndRowNumber = pm.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST
			};
			var usableAdv = SessionWrapper.LoginOperator.UsableAdvcodeNosInReport.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			var total = new AdvCodeService().GetAdvCodeSearchHitCount(searchCondition, usableAdv);
			var list = new AdvCodeService().SearchAdvCode(searchCondition, usableAdv);

			if (list.Length == 0)
			{
				return new AdCodeSearchListViewModel
				{
					ParamModel = pm,
					ErrorMessage = WebMessages.NoHitListError
				};
			}
			return new AdCodeSearchListViewModel
			{
				ParamModel = pm,
				AdCodeSearchSearchResultListViewModel = list
					.Select(r => new AdCodeSearchSearchResultListViewModel(r)).ToArray(),
				PagerHtml = WebPager.CreateDefaultListPager(total,
					pm.PagerNo,
					this.UrlHelper.Action(
						"AdCodeSearchList",
						Constants.CONTROLLER_W2CMS_MANAGER_SEARCH_POPUP,
						pm))
			};
		}
	}
}
