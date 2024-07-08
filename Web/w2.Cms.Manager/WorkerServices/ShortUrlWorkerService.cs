/*
=========================================================================================================
  Module      : ショートURLワーカーサービス(ShortUrlWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.RefreshFileManager;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.ShortUrl;
using w2.Cms.Manager.ViewModels.ShortUrl;
using w2.Domain.ShortUrl;
using w2.Domain.ShortUrl.Helper;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// ショートURLワーカーサービス
	/// </summary>
	public class ShortUrlWorkerService : BaseWorkerService
	{
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>ビューモデル</returns>
		public ShortUrlListViewModel Search(ShortUrlListParamModel pm)
		{
			var searchResult = this.GetSearchResultListViewModel(pm);
			var rtn = new ShortUrlListViewModel(pm.PageLayout)
			{
				ParamModel = pm
			};

			if (searchResult.Item1 == 0)
			{
				rtn.ErrorMessage = WebMessages.NoHitListError;
				return rtn;
			}

			rtn.PagerHtml = WebPager.CreateDefaultListPager(
				searchResult.Item1,
				pm.PagerNo,
				this.UrlHelper.Action("List", Constants.CONTROLLER_W2CMS_MANAGER_SHORT_URL, pm));
			rtn.SearchResult = searchResult.Item2;
			return rtn;
		}

		/// <summary>
		/// 検索結果ビューモデル生成
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>
		/// Item1：検索結果件数
		/// Item2：検索結果ビューモデル
		/// </returns>
		private Tuple<int, List<ShortUrlInput>> GetSearchResultListViewModel(ShortUrlListParamModel pm)
		{
			var cond = new ShortUrlListSearchCondition()
			{
				ShopId = this.SessionWrapper.LoginShopId,
				SortKbn = pm.SortKbn,
				ShortUrl = pm.ShortUrl,
				LongUrl = pm.LongUrl,
				ProtocolAndDomain = pm.ProtocolAndDomain,
				BeginRowNumber = (pm.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
				EndRowNumber = pm.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST
			};

			var total = new ShortUrlService().GetSearchHitCount(cond);
			var list = new ShortUrlService().Search(cond);

			if ((list == null) || list.Length == 0)
			{
				return new Tuple<int, List<ShortUrlInput>>(0, new List<ShortUrlInput>());
			}

			// まとめて返す
			return new Tuple<int, List<ShortUrlInput>>(total, list.Select(i => new ShortUrlInput(i)).ToList());
		}

		/// <summary>
		/// 編集モードへ
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>編集モードのビューモデル</returns>
		public ShortUrlListViewModel ToEditMode(ShortUrlListParamModel pm)
		{
			var s = this.Search(pm);
			if (pm.PageLayout == Constants.POPUP_LAYOUT_PATH_DEFAULT) s.PageLayout = Constants.POPUP_LAYOUT_PATH_DEFAULT;
			s.DisplayMode = ShortUrlListViewModel.Mode.Edit;
			s.ParamModel.BulkTargetInput = s.SearchResult.ToArray();
			return s;
		}

		/// <summary>
		/// 一覧モードへ
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>一覧モードのビューモデル</returns>
		public ShortUrlListViewModel ToListMode(ShortUrlListParamModel pm)
		{
			var s = this.Search(pm);
			if (pm.PageLayout == Constants.POPUP_LAYOUT_PATH_DEFAULT) s.PageLayout = Constants.POPUP_LAYOUT_PATH_DEFAULT;
			s.DisplayMode = ShortUrlListViewModel.Mode.Display;
			return s;
		}

		/// <summary>
		/// 一括更新
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <param name="bt">更新対象入力データ</param>
		/// <returns>一括更新後のビューモデル</returns>
		public ShortUrlListViewModel BulkUpdate(ShortUrlListParamModel pm, ShortUrlInput[] bt)
		{
			var sv = new ShortUrlService();
			// 変更があったもの
			var targets = bt.Where(i => i.IsUrlChanged()).ToArray();

			if (targets.Length == 0)
			{
				var rtn = new ShortUrlListViewModel();
				rtn.ParamModel = pm;
				rtn.ErrorMessage = WebMessages.ShorturlTargetNoSelectedError;
				return rtn;
			}

			// チェック
			var checkModels = sv.GetAll(this.SessionWrapper.LoginShopId);
			var error = new StringBuilder();
			foreach (var target in targets)
			{
				target.ShopId = base.SessionWrapper.LoginShopId;
				error.Append(target.Validate(false, checkModels));
			}

			if (string.IsNullOrEmpty(error.ToString()) == false)
			{
				var rtn = new ShortUrlListViewModel();
				rtn.ParamModel = pm;
				rtn.ErrorMessage = error.ToString();
				return rtn;
			}

			sv.Update(targets.Select(t => t.CreateModel(false)));
			RefreshFileManagerProvider.GetInstance(RefreshFileType.ShortUrl).CreateUpdateRefreshFile();

			var vm = new ShortUrlListViewModel();
			if (pm.PageLayout == Constants.POPUP_LAYOUT_PATH_DEFAULT) vm.PageLayout = Constants.POPUP_LAYOUT_PATH_DEFAULT;
			vm.ParamModel = pm;
			vm.SearchResult = targets.ToList();
			vm.DisplayMode = ShortUrlListViewModel.Mode.Result;
			return vm;
		}

		/// <summary>
		/// 編集続ける
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>編集モードのビューモデル</returns>
		public ShortUrlListViewModel ToContinue(ShortUrlListParamModel pm)
		{
			return ToEditMode(pm);
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <param name="input">登録対象の入力データ</param>
		/// <returns>登録後のビューモデル</returns>
		public ShortUrlListViewModel Register(ShortUrlListParamModel pm, ShortUrlInput input)
		{
			var sv = new ShortUrlService();
			// チェック
			input.ShopId = base.SessionWrapper.LoginShopId;
			var error = input.Validate(true, sv.GetAll(this.SessionWrapper.LoginShopId));
			if (string.IsNullOrEmpty(error) == false)
			{
				var errorVm = new ShortUrlListViewModel();
				errorVm.ParamModel = pm;
				errorVm.ErrorMessage = error;
				return errorVm;
			}

			// 登録
			sv.Insert(input.CreateModel(true));
			RefreshFileManagerProvider.GetInstance(RefreshFileType.ShortUrl).CreateUpdateRefreshFile();

			if (pm.PageLayout == Constants.POPUP_LAYOUT_PATH_DEFAULT) pm.LongUrl = input.LongUrl;
			var vm = this.Search(pm);
			if (pm.PageLayout == Constants.POPUP_LAYOUT_PATH_DEFAULT) vm.PageLayout = Constants.POPUP_LAYOUT_PATH_DEFAULT;
			return vm;
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>削除後のビューモデル</returns>
		public ShortUrlListViewModel Delete(ShortUrlListParamModel pm)
		{
			if (string.IsNullOrEmpty(pm.DelSurlNo) == false)
			{
				new ShortUrlService().Delete(long.Parse(pm.DelSurlNo));
				RefreshFileManagerProvider.GetInstance(RefreshFileType.ShortUrl).CreateUpdateRefreshFile();
			}
			return this.ToEditMode(pm);
		}

		/// <summary>
		/// エクスポート
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>エクスポートするファイルデータ</returns>
		public MasterExportHelper.ExportFileData Export(ShortUrlListParamModel pm)
		{
			var masterKbn = pm.DataExportType.Split('-')[1];
			var settingId = int.Parse(pm.DataExportType.Split('-')[0]) - 1;
			var cond = new ShortUrlListSearchCondition()
			{
				ShopId = this.SessionWrapper.LoginShopId,
				SortKbn = pm.SortKbn,
				ShortUrl = pm.ShortUrl,
				LongUrl = pm.LongUrl,
				ProtocolAndDomain = pm.ProtocolAndDomain,
				BeginRowNumber = (pm.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
				EndRowNumber = pm.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST
			};

			var fileData = MasterExportHelper.CreateExportData(
				base.SessionWrapper.LoginShopId,
				masterKbn,
				settingId,
				cond);

			return fileData;
		}
	}
}