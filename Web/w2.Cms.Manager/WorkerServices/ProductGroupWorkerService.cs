/*
=========================================================================================================
  Module      : 商品グループワーカーサービス(ProductGroupWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using w2.App.Common.RefreshFileManager;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Controllers;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.ProductGroup;
using w2.Cms.Manager.ViewModels.ProductGroup;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.ProductGroup;
using w2.Domain.ProductGroup.Helper;
using Validator = w2.Common.Util.Validator;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// 商品グループワーカーサービス
	/// </summary>
	public class ProductGroupWorkerService : BaseWorkerService
	{
		/// <summary>
		/// リストビューモデル作成
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>ビューモデル</returns>
		public ListViewModel CreateListVm(ProductGroupListParamModel pm)
		{
			pm.Dates.CorrectLastDayOfMonth();
			var result = this.GetSearchResult(pm);

			if (pm.Dates.CheckDate() == false) { return new ListViewModel { ParamModel = pm, ErrorMessage = WebMessages.InvalidDateError }; }
			if (result.Item1 == 0) { return new ListViewModel { ParamModel = pm, ErrorMessage = WebMessages.NoHitListError }; }

			return new ListViewModel
			{
				ParamModel = pm,
				SearchResultListViewModel =
					result.Item2.Select(r => new SearchResultListViewModel(r, this.UrlHelper)).ToArray(),
				PagerHtml = WebPager.CreateDefaultListPager(
					result.Item1,
					pm.PagerNo,
					this.UrlHelper.Action("List", Constants.CONTROLLER_W2CMS_MANAGER_PRODUCT_GROUP, pm))
			};
		}

		/// <summary>
		/// 検索結果取得
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>
		/// Item1：検索結果件数
		/// Item2：検索結果
		/// </returns>
		private Tuple<int, List<ProductGroupListSearchResult>> GetSearchResult(ProductGroupListParamModel pm)
		{
			var startFromDate = string.Format("{0}/{1}/{2}", pm.Dates.BeginYearFrom, pm.Dates.BeginMonthFrom, pm.Dates.BeginDayFrom);
			var startToDate = string.Format("{0}/{1}/{2}", pm.Dates.BeginYearTo, pm.Dates.BeginMonthTo, pm.Dates.BeginDayTo);
			var endFromDate = string.Format("{0}/{1}/{2}", pm.Dates.EndYearFrom, pm.Dates.EndMonthFrom, pm.Dates.EndDayFrom);
			var endToDate = string.Format("{0}/{1}/{2}", pm.Dates.EndYearTo, pm.Dates.EndMonthTo, pm.Dates.EndDayTo);

			var searchCondition = new ProductGroupListSearchCondition
			{
				ProductGroupId = pm.ProductGroupId,
				ProductGroupName = pm.ProductGroupName,
				ValidFlg = string.IsNullOrEmpty(pm.ValidFlg) ? null : pm.ValidFlg,
				MasterId = pm.ProductId,
				BeginDateFrom = Validator.IsDate(startFromDate) ? startFromDate : null,
				BeginDateTo = Validator.IsDate(startToDate) ? startToDate : null,
				EndDateFrom = Validator.IsDate(endFromDate) ? endFromDate : null,
				EndDateTo = Validator.IsDate(endToDate) ? endToDate : null,
				BeginRowNumber = (pm.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
				EndRowNumber = pm.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST
			};
			var sv = new ProductGroupService();
			var total = sv.GetSearchHitCount(searchCondition);
			var list = sv.Search(searchCondition);

			if ((list == null) || (list.Length == 0))
			{
				return new Tuple<int, List<ProductGroupListSearchResult>>(0, new List<ProductGroupListSearchResult>());
			}

			// まとめて返す
			return new Tuple<int, List<ProductGroupListSearchResult>>(total, list.ToList());
		}

		/// <summary>
		/// 編集ビュー生成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="productGroupId">商品グループID</param>
		/// <param name="pageLayout">ページレイアウト</param>
		/// <returns>編集・登録画面用のビューモデル</returns>
		public EditViewModel CreateEditVm(ActionStatus actionStatus, string productGroupId, string pageLayout)
		{
			var sv = new ProductGroupService();
			switch (actionStatus)
			{
				case ActionStatus.Update:
				case ActionStatus.CopyInsert:
					var updateModel = sv.Get(productGroupId);
					if (updateModel != null)
					{
						return new EditViewModel(updateModel) { ActionStatus = actionStatus, PageLayout = pageLayout};
					}
					else
					{
						return new EditViewModel()
						{
							ActionStatus = actionStatus,
							ErrorMessage = WebMessages.MasterExportSettingIrregularParameterError
						};
					}

				case ActionStatus.Insert:
					var vm = new EditViewModel
					{
						ActionStatus = actionStatus,
						PageLayout = pageLayout,
					};
					// 新規登録の場合の初期値設定
					var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
					vm.Input.BeginDate = today.ToString();
					vm.Input.BeginDateYear = today.ToString("yyyy");
					vm.Input.BeginDateMonth = today.ToString("MM");
					vm.Input.BeginDateDay = today.ToString("dd");
					vm.Input.BeginDateHour = today.ToString("HH");
					vm.Input.BeginDateMinute = today.ToString("mm");
					vm.Input.BeginDateSecond = today.ToString("ss");
					return vm;

				default:
					return new EditViewModel()
					{
						ActionStatus = actionStatus,
						ErrorMessage = WebMessages.MasterExportSettingIrregularParameterError
					};
			}
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="input">入力値</param>
		public void Delete(ProductGroupInput input)
		{
			var sv = new ProductGroupService();
			sv.Delete(input.ProductGroupId);

			// リフレッシュファイル更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.ProductGroup).CreateUpdateRefreshFile();
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>編集・登録画面用のビューモデル</returns>
		public EditViewModel Update(ProductGroupInput input)
		{
			input.SetDate();

			var model = input.CreateModel();
			model.LastChanged = base.SessionWrapper.LoginOperatorName;

			foreach (var item in model.Items)
			{
				item.LastChanged = model.LastChanged;
			}

			var sv = new ProductGroupService();
			sv.Update(model);

			// リフレッシュファイル更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.ProductGroup).CreateUpdateRefreshFile();

			var vm = this.CreateEditVm(ActionStatus.Update, model.ProductGroupId, ProductGroupController.m_pageLayout);
			vm.UpdateInsertSuccessFlg = true;
			return vm;
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>編集・登録画面用のビューモデル</returns>
		public EditViewModel Register(ProductGroupInput input)
		{
			input.SetDate();

			var model = input.CreateModel();
			model.LastChanged = base.SessionWrapper.LoginOperatorName;

			foreach (var item in model.Items)
			{
				item.LastChanged = model.LastChanged;
			}

			var sv = new ProductGroupService();
			sv.Insert(model);

			// リフレッシュファイル更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.ProductGroup).CreateUpdateRefreshFile();

			var vm = this.CreateEditVm(ActionStatus.Update, model.ProductGroupId, ProductGroupController.m_pageLayout);
			vm.UpdateInsertSuccessFlg = true;
			return vm;
		}

		/// <summary>
		/// プレビューファイル作成
		/// </summary>
		/// <param name="contents">プレビューコンテンツ</param>
		/// <param name="htmlkbn">HTML区分</param>
		public void CreatePreviewFile(string contents, string htmlkbn)
		{
			DeleteFile();
			CreateFile(contents, htmlkbn);
		}

		/// <summary>
		/// プレビュー用ファイル作成
		/// </summary>
		/// <param name="contents">プレビューコンテンツ</param>
		/// <param name="htmlkbn">HTML区分</param>
		private void CreateFile(string contents, string htmlkbn)
		{
			var productGroupPageContents = GetProductGroupPageContents(contents, htmlkbn);
			// プレビューファイル出力
			using (var sw = new StreamWriter(this.GetPreviewFilePhysicalPath(), false, System.Text.Encoding.UTF8))
			{
				sw.WriteLine("<%@ Page Language=\"C#\" MasterPageFile=\"~/Form/Common/DefaultPage.master\" %>\r\n");
				sw.WriteLine(
					"<asp:Content ID=\"Content2\" ContentPlaceHolderID=\"ContentPlaceHolder1\" Runat=\"Server\">\r\n");
				sw.WriteLine(productGroupPageContents + "\r\n");
				sw.WriteLine("</asp:content>\r\n");
			}
		}

		/// <summary>
		/// 商品グループページ表示内容HTML取得
		/// </summary>
		/// <param name="htmlkbn">HTML区分（0:TEXT、1:HTML）</param>
		/// <param name="contents">商品グループページ表示内容</param>
		/// <returns>商品グループページ表示内容HTML</returns>
		private string GetProductGroupPageContents(string contents, string htmlkbn)
		{
			var contentsHtml = StringUtility.ToEmpty(contents);
			switch (StringUtility.ToEmpty(htmlkbn))
			{
				case Constants.FLG_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS_KBN_HTML:
					// 相対パスを絶対パスに置換(aタグ、imgタグのみ）
					MatchCollection relativePath = Regex.Matches(contentsHtml, "(a[\\s]+href=|img[\\s]+src=)([\"|']([^/|#][^\\\"':]+)[\"|'])", RegexOptions.IgnoreCase);
					var frontUrl = new Uri(Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC);

					foreach (Match match in relativePath)
					{
						var resourceUri = new Uri(frontUrl, match.Groups[3].ToString());
						contentsHtml = contentsHtml
							.Replace(match.Groups[2].ToString()
								.Replace("../", "")
								.Replace("./", ""), "\"" + frontUrl + match.Groups[3] + resourceUri.Fragment + "\"");
					}
					return contentsHtml;

				case Constants.FLG_PRODUCTGROUP_PRODUCT_GROUP_PAGE_CONTENTS_KBN_TEXT:
					return HtmlSanitizer.HtmlEncodeChangeToBr(contentsHtml);

				default:
					return "";
			}
		}

		/// <summary>
		/// プレビュー用ファイル削除
		/// </summary>
		private void DeleteFile()
		{
			if (File.Exists(this.GetPreviewFilePhysicalPath())) { File.Delete(this.GetPreviewFilePhysicalPath()); }
		}

		/// <summary>
		/// プレビュー用ビューモデル作成
		/// </summary>
		/// <returns>プレビュー用のビューモデル</returns>
		public PreviewViewModel CreatePreviewVm()
		{
			return new PreviewViewModel() { PreviewUrl = this.GetPreviewFileUrl() };
		}

		/// <summary>プレビューファイルパス取得</summary>
		private string GetPreviewFilePhysicalPath()
		{
			var path = Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, Constants.PAGE_FRONT_PRODUCT_GROUP.Replace(@"\", "/") + ".Preview.aspx");
			return path;
		}

		/// <summary>プレビューファイルURL取得</summary>
		private string GetPreviewFileUrl()
		{
			var path = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC + Constants.PAGE_FRONT_PRODUCT_GROUP + ".Preview.aspx";
			return path;
		}
	}
}