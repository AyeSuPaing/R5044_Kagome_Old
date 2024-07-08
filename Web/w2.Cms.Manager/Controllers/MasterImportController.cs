/*
=========================================================================================================
  Module      : マスタインポートコントローラ(MasterImportController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.ViewModels.MasterImport;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// マスタインポートコントローラ
	/// </summary>
	public class MasterImportController : BaseController
	{
		/// <summary>
		/// 一覧表示
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult MasterImportList()
		{
			var vm = this.Service.CreateListVm(
				new SessionWrapper(this.Session).LoginOperator.ShopId,
				this.TempData.MasterType);

			// ドロップダウンデフォルト選択
			if (this.TempData.MasterType != null)
			{
				vm.MasterType = this.TempData.MasterType;
			}
			else
			{
				this.TempData.MasterType = vm.MasterItems[0].Value;
			}

			return View(vm);
		}

		/// <summary>
		/// マスタアップロード完了画面表示
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult MasterImportComplete()
		{
			return View();
		}

		/// <summary>
		/// アップロード
		/// </summary>
		/// <param name="fUpload">アップロードファイル</param>
		/// <returns>アクション結果</returns>
		public ActionResult Upload(HttpPostedFileWrapper fUpload)
		{
			var errorMessage = this.Service.Upload(fUpload);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				return CreateErrorPageAction(errorMessage);
			}

			// 一覧ページへ（選択マスタ保持しておく）
			return RedirectToAction("MasterImportList");
		}

		/// <summary>
		/// ドロップダウンリスト変更時処理
		/// </summary>
		/// <param name="vm">ビューモデル</param>
		/// <returns>アクション結果</returns>
		[ActionName("Upload")]
		[Button(ButtonName = "SelectedIndexChangeBtn")]
		public ActionResult SelectedIndexChanged(ListViewModel vm)
		{
			// ページの有効期限切れを防ぐため、自ページへ遷移
			this.TempData.MasterType = vm.MasterType;

			return RedirectToAction("MasterImportList");
		}

		/// <summary>
		/// ファイル取込
		/// </summary>
		/// <param name="fileName">ファイルネーム</param>
		/// <returns>アクション結果</returns>
		public ActionResult ImportFile(string fileName)
		{
			this.Service.ImportFile(fileName, this.TempData.MasterType);

			// 取込実行完了画面へ遷移
			return RedirectToAction("MasterImportComplete");
		}

		/// <summary>
		/// ファイル削除
		/// </summary>
		/// <param name="fileName">ファイルネーム</param>
		/// <returns>アクション結果</returns>
		[ActionName("ImportFile")]
		[Button(ButtonName = "DeleteFileBtn")]
		public ActionResult DeleteFile(string fileName)
		{
			this.Service.DeleteFile(fileName);

			// ページ更新のためリロード
			return RedirectToAction("MasterImportList");
		}

		/// <summary>サービス</summary>
		private MasterImportWorkerService Service { get { return GetDefaultService<MasterImportWorkerService>(); } }
	}
}
