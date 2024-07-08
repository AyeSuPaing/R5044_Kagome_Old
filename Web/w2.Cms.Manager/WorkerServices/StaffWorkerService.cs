/*
=========================================================================================================
  Module      : スタッフワーカーサービス(StaffWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.ParamModels.Staff;
using w2.Cms.Manager.ViewModels.Staff;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.Staff;
using w2.Domain.Staff.Helper;
using w2.Domain.User;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// スタッフワーカーサービス
	/// </summary>
	public class StaffWorkerService : BaseWorkerService
	{
		/// <summary>スタッフ画像パス</summary>
		public static string m_staffPath = @"Contents\Staff\";
		/// <summary>スタッフ画像ルート</summary>
		public string m_staffRoot = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, m_staffPath);

		/// <summary>
		/// リストビューモデル作成
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>ビューモデル</returns>
		public ListViewModel CreateListVm(ListParamModel pm)
		{
			pm.HeightLowerLimit = (Regex.IsMatch(pm.HeightLowerLimit, @"^[0-9]+$")) ? pm.HeightLowerLimit : string.Empty;
			pm.HeightUpperLimit = (Regex.IsMatch(pm.HeightUpperLimit, @"^[0-9]+$")) ? pm.HeightUpperLimit : string.Empty;
			var searchCondition = new StaffListSearchCondition
			{
				StaffId = pm.StaffId,
				StaffName = pm.StaffName,
				HeightLowerLimit = pm.HeightLowerLimit,
				HeightUpperLimit = pm.HeightUpperLimit,
				BeginRowNumber = (pm.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
				EndRowNumber = pm.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST,
			};
			var total = new StaffService().GetSearchHitCount(searchCondition);
			var list = new StaffService().Search(searchCondition);
			if (list.Length == 0)
			{
				return new ListViewModel
				{
					ParamModel = pm,
					ErrorMessage = WebMessages.NoHitListError,
				};
			}

			var url = this.UrlHelper.Action(
				"List",
				Constants.CONTROLLER_W2CMS_MANAGER_STAFF,
				new
				{
					pm.StaffId,
					pm.StaffName,
					pm.HeightLowerLimit,
					pm.HeightUpperLimit,
				});
			return new ListViewModel
			{
				ParamModel = pm,
				List = list,
				PagerHtml = WebPager.CreateDefaultListPager(total, pm.PagerNo, url),
			};
		}

		/// <summary>
		/// 登録編集ビューモデル作成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="staffId">スタッフID</param>
		/// <returns>ビューモデル</returns>
		public RegisterViewModel CreateRegisterVm(ActionStatus actionStatus, string staffId)
		{
			if ((actionStatus == ActionStatus.Update)
				|| (string.IsNullOrEmpty(staffId) == false))
			{
				var ss = new StaffService().Get(staffId);
				return new RegisterViewModel
				{
					ActionStatus = actionStatus,
					OperatorId = ss.OperatorId,
					StaffName = ss.StaffName,
					StaffHeight = (ss.StaffHeight == 0) ? string.Empty : ss.StaffHeight.ToString(),
					StaffInstagramId = ss.StaffInstagramId,
					StaffProfile = ss.StaffProfile,
					RealShopId =ss.RealShopId,
					ValidFlg = ss.ValidFlg,
					ModelFlg = ss.ModelFlg,
					StaffId = ss.StaffId,
					StaffSex = ss.StaffSex
				};
			}
			else if (actionStatus == ActionStatus.Insert)
			{
				return new RegisterViewModel
				{
					ActionStatus = actionStatus, 
					OperatorId = this.SessionWrapper.LoginOperator.OperatorId,
					StaffSex = Constants.FLG_STAFF_SEX_UNKNOWN,
					ValidFlg = Constants.FLG_STAFF_VALID_FLG_VALID
				};
			}
			throw new Exception("未対応のactionStatus：" + actionStatus);
		}

		/// <summary>
		/// 確認詳細ビューモデル作成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="staffId">スタッフID</param>
		/// <param name="pageLayout">ページレイアウト</param>
		/// <param name="tempDataManager">一時的データ</param>
		/// <returns>確認ビューモデル</returns>
		public ConfirmViewModel CreateConfirmVm(
			ActionStatus actionStatus,
			string staffId,
			string pageLayout,
			TempDataManager tempDataManager)
		{
			switch (actionStatus)
			{
				case ActionStatus.Detail:
					var cmm = new StaffService().Get(staffId);
					return new ConfirmViewModel(actionStatus, this.SessionWrapper.LoginShopId, pageLayout, cmm);

				case ActionStatus.Insert:
				case ActionStatus.Update:
					return new ConfirmViewModel(actionStatus, this.SessionWrapper.LoginShopId,  pageLayout,tempDataManager.Staff);

				default:
					throw new Exception("未対応のactionStatus：" + actionStatus);
			}
		}

		/// <summary>
		/// 登録更新
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="staff">スタッフ</param>
		public void InsertUpdate(ActionStatus actionStatus, StaffModel staff)
		{
			staff.LastChanged = this.SessionWrapper.LoginOperatorName;

			var service = new StaffService();
			switch (actionStatus)
			{
				case ActionStatus.Insert:
					staff.StaffId = NumberingUtility.CreateKeyId(
						this.SessionWrapper.LoginShopId,
						Constants.NUMBER_KEY_CMS_STAFF_ID,
						Constants.CONST_STAFF_ID_LENGTH);
					service.Insert(staff);
					break;

				case ActionStatus.Update:
					service.Update(staff);
					break;

				default:
					throw new Exception("未対応のactionStatus：" + actionStatus);
			}
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="staffId">スタッフID</param>
		public void Delete(string staffId)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				new StaffService().Delete(staffId, accessor);
				new UserService().DeleteUserActivityForManager(Constants.FLG_USERACTIVITY_MASTER_KBN_COORDINATE_FOLLOW, staffId, accessor);

				accessor.CommitTransaction();
			}
		}

		/// <summary>
		/// サムネイル取得
		/// </summary>
		/// <param name="staffId">スタッフID</param>
		/// <returns>サムネイル</returns>
		public static string GetStaffThumbnail(string staffId)
		{
			var dir = Constants.PATH_STAFF;
			var fileName = staffId + "." + Constants.KBN_IMAGEFORMAT_JPG;

			// 条件判断して画像ファイル名などを決定
			var imageFileUrlPart = File.Exists(Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, dir, fileName))
				? fileName
				: Constants.PRODUCTIMAGE_NOIMAGE_PC;

			// 画像パス作成
			var rootPath = Constants.PATH_ROOT_FRONT_PC;
			var pathTmp = Path.Combine(rootPath, dir, imageFileUrlPart);
			var path = rootPath.StartsWith("/") ? CreateImageCnvUrl(pathTmp, Constants.KBN_IMAGEFORMAT_JPG, 80) : pathTmp;
			return path;
		}

		/// <summary>
		/// イメージコンバータURLの作成
		/// </summary>
		/// <param name="imageFileUrl">商品画像URL</param>
		/// <param name="format">拡張子</param>
		/// <param name="width">サイズ</param>
		/// <returns>イメージコンバータ経由で参照可能なURL</returns>
		private static string CreateImageCnvUrl(string imageFileUrl, string format, int width)
		{
			var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_IMAGECONVERTER)
				.AddParam(Constants.REQUEST_KEY_IMGCNV_FILE, imageFileUrl)
				.AddParam(Constants.REQUEST_KEY_IMGCNV_FORMAT, format)
				.AddParam(Constants.REQUEST_KEY_IMGCNV_SIZE, width.ToString())
				.CreateUrl();
			return url;
		}

		/// <summary>
		/// メイン画像アップロード
		/// </summary>
		/// <param name="image"></param>
		/// <param name="staffId"></param>
		public string Upload(HttpPostedFileBase image, string staffId)
		{
			if (image == null) return string.Empty;

			try
			{
				var uploadPath = Path.Combine(m_staffRoot, staffId + ".jpg");
				image.SaveAs(uploadPath);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return WebMessages.ContentsManagerFileOperationError;
			}

			return string.Empty;
		}

		/// <summary>
		/// ファイル存在チェック
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>true:あり false:なし</returns>
		public bool CheckFileExist(string fileName)
		{
			var isExist = File.Exists(Path.Combine(m_staffRoot, fileName));
			return isExist;
		}

		/// <summary>
		/// エクスポート
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>エクスポートするファイルデータ</returns>
		public MasterExportHelper.ExportFileData Export(ListParamModel pm)
		{
			var masterKbn = pm.DataExportType.Split('-')[1];
			var settingId = int.Parse(pm.DataExportType.Split('-')[0]) - 1;
			var cond = new StaffListSearchCondition
			{
				StaffId = pm.StaffId,
				StaffName = pm.StaffName,
				HeightLowerLimit = pm.HeightLowerLimit,
				HeightUpperLimit = pm.HeightUpperLimit,
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