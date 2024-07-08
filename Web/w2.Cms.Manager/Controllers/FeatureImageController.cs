/*
=========================================================================================================
  Module      : 特集画像管理コントローラ(FeatureImageController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web;
using System.Web.Mvc;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.FeatureImage;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// 特集画像管理コントローラ
	/// </summary>
	public class FeatureImageController : BaseController
	{
		/// <summary>
		/// メイン画面
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult Main()
		{
			var vm = this.Service.CreateMainVm();
			return View(vm);
		}

		/// <summary>
		/// グループ一覧の取得
		/// </summary>
		/// <param name="paramModel">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult GroupList(FeatureImageListSearchParamModel paramModel)
		{
			var vm = this.Service.CreateGroupListVm(paramModel);
			return PartialView("_MainContentList", vm);
		}

		/// <summary>
		/// グループJSON取得
		/// </summary>
		/// <param name="paramModel">パラメタモデル</param>
		/// <returns>アクション結果</returns>
		public ActionResult GroupListJson(FeatureImageListSearchParamModel paramModel)
		{
			var vm = this.Service.CreateGroupListVm(paramModel);
			return Json(vm);
		}

		/// <summary>
		/// グループ順序更新
		/// </summary>
		/// <param name="groupIds">グループ順序配列</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		public ActionResult UpdateGroupSort(long[] groupIds)
		{
			this.Service.GroupSortUpdate(groupIds);
			return null;
		}

		/// <summary>
		/// 画像順序更新
		/// </summary>
		/// <param name="groupId">対象グループID</param>
		/// <param name="imagePath">画像パス</param>
		/// <returns>アクション結果 なし</returns>
		[HttpPost]
		public ActionResult UpdateImageSort(long groupId, string[] imagePath)
		{
			this.Service.ImageSortUpdate(groupId, imagePath);
			return null;
		}

		/// <summary>
		/// グループ追加
		/// </summary>
		/// <param name="input">グループ入力内容</param>
		/// <returns>アクション結果 なし</returns>
		[HttpPost]
		public ActionResult GroupAdd(FeatureImageGroupInput input)
		{
			this.Service.GroupAdd(input);
			return null;
		}

		/// <summary>
		/// グループIDとキーワードで検索
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="keyword">検索キーワード</param>
		/// <param name="imageType">商品画像タイプ</param>
		/// <returns>アクション結果</returns>
		public ActionResult DefaultImageSearch(string groupId, string keyword, ImageType imageType = ImageType.Normal)
		{
			var result = ImageSearch(groupId, keyword, imageType, ImageSearchCache.Register);
			return result;
		}

		/// <summary>
		/// グループIDとキーワードで検索
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="keyword">検索キーワード</param>
		/// <param name="imageType">商品画像タイプ</param>
		/// <param name="cache">画像検索キャッシュ登録方法</param>
		/// <returns>アクション結果</returns>
		public ActionResult ImageSearch(
			string groupId,
			string keyword,
			ImageType imageType = ImageType.Normal,
			ImageSearchCache cache = ImageSearchCache.Restore)
		{
			var vm = new FeatureImageWorkerService(imageType, cache)
				.CreateGroupListVm(
					new FeatureImageListSearchParamModel { GroupId = groupId, Keyword = keyword },
					SessionWrapper.FeatureImageList,
					SessionWrapper.FeatureImageGroupList);
			return PartialView("_ImageGroup", vm);
		}

		/// <summary>
		/// グループ削除
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <returns>アクション結果 なし</returns>
		[HttpPost]
		public ActionResult GroupDelete(long groupId)
		{
			this.Service.DeleteGroup(groupId);
			return null;
		}

		/// <summary>
		/// グループ名変更
		/// </summary>
		/// <param name="input">グループ入力内容</param>
		/// <returns>アクション結果 なし</returns>
		[HttpPost]
		public ActionResult GroupNameEdit(FeatureImageGroupInput input)
		{
			this.Service.GroupNameEdit(input);
			return null;
		}

		/// <summary>
		/// アップロード
		/// </summary>
		/// <param name="files">画像</param>
		/// <param name="groupId">対象グループID</param>
		/// <returns>アクション結果</returns>
		public ActionResult Upload(HttpPostedFileBase files, string groupId)
		{
			var errorMessage = this.Service.Upload(files, groupId);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				return JavaScript("upload_failed()");
			}

			var vm = this.Service.CreateGroupListVm(new FeatureImageListSearchParamModel());
			var ret = PartialView("_MainContentList", vm);
			return ret;
		}

		/// <summary>
		/// 画像削除
		/// </summary>
		/// <param name="imageId">画像ID</param>
		/// <returns>アクション結果 なし</returns>
		[HttpPost]
		public ActionResult ImageDelete(string imageId)
		{
			this.Service.ImageDelete(imageId);
			return null;
		}

		/// <summary>
		/// 画像名変更
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <param name="imageId">画像ID</param>
		/// <returns>アクション結果 なし</returns>
		[HttpPost]
		public ActionResult ImageNameEdit(string fileName, string imageId)
		{
			var errorMessage = this.Service.ImageNameEdit(fileName, imageId);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				return JavaScript("image_name_edit_failed()");
			}
			return JavaScript("image_name_edit_success()");
		}

		/// <summary>
		/// ファイル存在チェック
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>true: 存在する false:存在しない</returns>
		[HttpPost]
		public ActionResult CheckFileExist(string fileName)
		{
			var isExist = this.Service.CheckFileExist(fileName);
			return Json(isExist, JsonRequestBehavior.AllowGet);
		}

		/// <summary>サービス</summary>
		private FeatureImageWorkerService Service { get { return GetDefaultService<FeatureImageWorkerService>(); } }
	}
}
