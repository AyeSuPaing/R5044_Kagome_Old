/*
=========================================================================================================
  Module      : マスタ出力定義コントローラ(MasterExportSettingController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.WorkerServices;
using w2.Common.Util;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// マスタ出力定義権限コントローラ
	/// </summary>
	public class MasterExportSettingController : BaseController
	{
		/// <summary>
		/// 登録編集画面表示
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <returns>アクション結果</returns>
		public ActionResult MasterExportSettingRegister(ActionStatus? actionStatus)
		{
			// リクエスト情報取得
			var param = this.Service.GetParameters(Request);

			// 不正パラメータが存在した場合エラーページへ
			if ((bool)param[Constants.ERROR_REQUEST_PRAMETER])
			{
				return CreateErrorPageAction(WebMessages.MasterExportSettingIrregularParameterError);
			}
			var masterKbn = (string)param[Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN];

			var vm = this.Service.CreateListVm(masterKbn, this.TempData, actionStatus);
			return View(vm);
		}

		/// <summary>
		/// 出力設定の変更
		/// </summary>
		/// <param name="input">マスタ種別設定入力値</param>
		/// <returns>アクション結果</returns>
		[ActionName("MasterExportSettingRegister")]
		[Button(ButtonName = "SelectedIndexChangedBtn")]
		public ActionResult SelectedIndexChanged(MasterExportSettingInput input)
		{
			// 表示設定
			this.TempData.MasterExportSettingInput = this.Service.SetSelectSettingList(input, this.TempData);

			// マスタ種別を設定し、再度読込
			return RedirectToAction(
				"MasterExportSettingRegister",
				new
				{
					actionStatus = ActionStatus.SelectChange,
					mk = this.TempData.MasterExportSettingInput.MasterKbn.Value
				});
		}

		/// <summary>
		/// マスタ種別の変更
		/// </summary>
		/// <param name="input">マスタ種別設定入力値</param>
		/// <returns>アクション結果</returns>
		[ActionName("MasterExportSettingRegister")]
		[Button(ButtonName = "MasterSelectedIndexChangedBtn")]
		public ActionResult Master_SelectedIndexChanged(MasterExportSettingInput input)
		{
			var masterKbn = input.MasterKbn.Value;
			this.TempData.MasterExportSettingInput = new MasterExportSettingInput
			{
				MasterKbn = new MasterExportSettingInput.MasterKbnInput{ Value = masterKbn }
			};

			// マスタ種別を設定し、再度読込
			return RedirectToAction("MasterExportSettingRegister", new { mk = masterKbn });
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="input">マスタ種別設定入力値</param>
		/// <returns>アクション結果</returns>
		[ActionName("MasterExportSettingRegister")]
		[Button(ButtonName = "Insert")]
		public ActionResult Insert(MasterExportSettingInput input)
		{
			input.IsInsert = true;
			this.Service.InsertUpdateExportSetting(input);
			// 表示設定
			this.TempData.MasterExportSettingInput = this.Service.SetSelectSettingList(input, this.TempData);

			return RedirectToAction(
				"MasterExportSettingRegister", 
				new
				{ 
					actionStatus = ActionStatus.Update,
					mk = this.TempData.MasterExportSettingInput.MasterKbn.Value
				});
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="input">マスタ種別設定入力値</param>
		/// <returns>アクション結果</returns>
		[ActionName("MasterExportSettingRegister")]
		[Button(ButtonName = "Update")]
		public ActionResult Update(MasterExportSettingInput input)
		{
			input.IsInsert = false;
			this.Service.InsertUpdateExportSetting(input);
			// 表示設定
			this.TempData.MasterExportSettingInput = this.Service.SetSelectSettingList(input, this.TempData);

			return RedirectToAction(
				"MasterExportSettingRegister",
				new 
				{ 
					actionStatus = ActionStatus.Update,
					mk = this.TempData.MasterExportSettingInput.MasterKbn.Value
				});
		}

		/// <summary>
		/// 入力確認
		/// </summary>
		/// <param name="input">入力値</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(MasterExportSettingInput input)
		{
			var errorMessage = this.Service.Validate(input);
			return StringUtility.ToEmpty(errorMessage);
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="input">マスタ種別入力</param>
		/// <returns>アクション結果</returns>
		[ActionName("MasterExportSettingRegister")]
		[Button(ButtonName = "Delete")]
		public ActionResult Delete(MasterExportSettingInput input)
		{
			this.Service.Delete(input);

			// 表示設定
			this.TempData.MasterExportSettingInput = this.Service.SetSelectSettingList(input, this.TempData);

			return RedirectToAction(
				"MasterExportSettingRegister",
				new
				{
					actionStatus = ActionStatus.Delete,
					mk = this.TempData.MasterExportSettingInput.MasterKbn.Value
				});
		}

		/// <summary>サービス</summary>
		private MasterExportSettingWorkerService Service { get { return GetDefaultService<MasterExportSettingWorkerService>(); } }
	}
}
