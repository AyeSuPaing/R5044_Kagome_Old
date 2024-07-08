/*
=========================================================================================================
  Module      : 商品一覧表示設定コントローラ(ProductListDispSettingController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web.Mvc;
using w2.App.Common.RefreshFileManager;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.WorkerServices;
using w2.Common.Util;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// 商品一覧表示設定コントローラ
	/// </summary>
	public class ProductListDispSettingController : BaseController
	{
		/// <summary>
		/// 編集画面
		/// </summary>
		/// <returns>結果</returns>
		public ActionResult Modify()
		{
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				this.SessionWrapper.TranslationSearchCondition = new string[0];
				this.SessionWrapper.TranslationExportTargetDataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTLISTDISPSETTING;
			}

			var vm = this.Service.CreateModifyVm();
			return View(vm);
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="input">商品一覧表示設定入力</param>
		/// <returns>アクション結果</returns>
		[HttpPost]
		[ActionName("Modify")]
		public ActionResult Update(ProductListDispSettingInput input)
		{
			// リフレッシュファイル更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.ProductListDispSetting).CreateUpdateRefreshFile();

			var vm = this.Service.CreateModifyVm(true);
			if (this.ModelState.IsValid) this.ModelState.Clear();
			return View(vm);
		}

		/// <summary>
		/// 入力確認
		/// </summary>
		/// <param name="input">商品一覧表示設定入力</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(ProductListDispSettingInput input)
		{
			var errorMessage = this.Service.Update(input);
			return StringUtility.ToEmpty(errorMessage);
		}

		/// <summary>サービス</summary>
		private ProductListDispSettingWorkerService Service { get { return GetDefaultService<ProductListDispSettingWorkerService>(); } }
	}
}
