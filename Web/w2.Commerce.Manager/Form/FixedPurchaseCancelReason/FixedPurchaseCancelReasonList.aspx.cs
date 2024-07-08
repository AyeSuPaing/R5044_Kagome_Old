/*
=========================================================================================================
  Module      : 定期解約理由区分設定一覧ページ処理(FixedPurchaseCancelReasonList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Input;
using w2.App.Common.DataCacheController;
using w2.Domain.FixedPurchase;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;

public partial class Form_FixedPurchaseCancelReason_FixedPurchaseCancelReasonList : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{		
		if (!IsPostBack)
		{
			// 画面に値をセット
			SetValues();
		}
	}

	/// <summary>
	/// 追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAdd_Click(object sender, EventArgs e)
	{
		// メッセージ非表示
		trMessages.Visible = false;

		// 定期解約理由区分設定入力内容リスト取得
		var cancelReasonInputList = GetCancelReasonInputList().ToList();

		// 追加
		var cancelReason = new FixedPurchaseCancelReasonInput()
		{
			DisplayKbn = FixedPurchaseCancelReasonModel.DisplayKbnValue.PC.ToString() + "," + FixedPurchaseCancelReasonModel.DisplayKbnValue.EC.ToString(),
			IsNew = true
		};
		cancelReasonInputList.Add(cancelReason);

		// 画面に値をセット
		SetValues(cancelReasonInputList.ToArray());
	}

	/// <summary>
	/// キャンセルボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCancel_Click(object sender, EventArgs e)
	{
		// 定期解約理由区分設定入力内容リスト取得
		var cancelReasonInputList = GetCancelReasonInputList().ToList();

		// 削除
		var index = 0;
		if (int.TryParse(StringUtility.ToEmpty((((LinkButton)sender).CommandArgument)), out index))
		{
			cancelReasonInputList.Remove(cancelReasonInputList[index]);
		}

		// 画面に値をセット
		SetValues(cancelReasonInputList.ToArray());
	}

	/// <summary>
	/// 一括更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAllUpdate_Click(object sender, EventArgs e)
	{
		// 定期解約理由区分設定入力内容リスト取得
		var cancelReasonInputList = GetCancelReasonInputList().ToArray();

		// 入力チェック対象抽出（削除以外）
		var cancelReasonInputListForInpuCheck = 
			cancelReasonInputList.Where(cr => cr.IsDelete == false);

		// 入力チェック
		var errorMessages = new StringBuilder();
		foreach (var cancelReasonInput in cancelReasonInputListForInpuCheck)
		{
			errorMessages.Append(cancelReasonInput.Validate());
		}

		// 解約理由区分ID重複チェック
		var duplicateCancelReasonIds =
			cancelReasonInputList
				.Where(cr => (cr.IsDelete == false) && (string.IsNullOrEmpty(cr.CancelReasonId) == false))
				.GroupBy(cr => cr.CancelReasonId.ToLower())
				.Where(cr => cr.Count() > 1)
				.Select(cr => cr.Key);
		if (duplicateCancelReasonIds.Any())
		{
			// 解約理由区分ID重複チェック対象抽出（削除以外、ID指定あり）
			var cancelReasonInputListForDuplicate =
				cancelReasonInputList.Where(cr => (cr.IsDelete == false) && (string.IsNullOrEmpty(cr.CancelReasonId) == false));
			foreach (var cancelReasonId in duplicateCancelReasonIds)
			{
				errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_CANCELREASON_DUPLICATE_ERROR)
										.Replace("@@ 1 @@", cancelReasonId)
										.Replace("@@ 2 @@", string.Join(", ", cancelReasonInputListForDuplicate.Where(cr => cr.CancelReasonId == cancelReasonId).Select(cr => cr.No))));
			}
		}

		// 定期購入情報で利用している定期解約理由区分取得
		var usedCancelReasonList = new FixedPurchaseService().GetUsedCancelReasonAll().Select(cr => cr.CancelReasonId);

		// 既に定期購入情報で利用されているかチェック
		var cancelReasonInputListForUsed =
			cancelReasonInputList.Where(cr => (cr.IsDelete) && (string.IsNullOrEmpty(cr.CancelReasonId) == false) && (usedCancelReasonList.Contains(cr.CancelReasonId)));
		foreach (var cancelReasonInput in cancelReasonInputListForUsed)
		{
			errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_CANCELREASON_HAS_BEEN_USE_ERROR)
												.Replace("@@ 1 @@", cancelReasonInput.CancelReasonId));
		}
		
		// エラーの場合、エラーメッセージをセットし処理を抜ける
		if (errorMessages.Length != 0)
		{
			trMessages.Visible = true;
			lMessages.ForeColor = System.Drawing.Color.Red;
			lMessages.Text = errorMessages.ToString();

			return;
		}

		// 更新処理（DELETE => INSERT）
		using (SqlAccessor accessor = new SqlAccessor())
		{
			try
			{
				// トランザクション：開始
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 削除
				var service = new FixedPurchaseService();
				service.DeleteCancelReasonAll(accessor);
				foreach (var cancelReasonInput in cancelReasonInputList)
				{
					// 削除の場合、次の要素へ
					if (cancelReasonInput.IsDelete) continue;

					// 新規登録?
					if (cancelReasonInput.IsNew)
					{
						cancelReasonInput.DateCreated = DateTime.Now.ToString();
						cancelReasonInput.DateChanged = DateTime.Now.ToString();
					}
					// 更新?
					else
					{
						// 変更あり?
						var model = cancelReasonInput.CreateModel();
						var cancelReasonOld = this.CancelReasonList[cancelReasonInput.No - 1];
						if ((model.CancelReasonName != cancelReasonOld.CancelReasonName)
							|| (model.DisplayOrder != cancelReasonOld.DisplayOrder)
							|| (model.DisplayKbn != cancelReasonOld.DisplayKbn))
						{
							cancelReasonInput.DateChanged = DateTime.Now.ToString();
						}
					}
					cancelReasonInput.LastChanged = this.LoginOperatorId;

					// 登録
					service.InsertCancelReason(cancelReasonInput.CreateModel(), accessor);
				}

				// トランザクション：コミット
				accessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				// トランザクション：ロールバック
				AppLogger.WriteError(ex.ToString());
				accessor.RollbackTransaction();
			}
		}

		// 登録/更新完了メッセージをセット
		trMessages.Visible = true;
		lMessages.ForeColor = System.Drawing.Color.Empty;
		lMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RESULT_REGISTERED_UPDATED);

		// 画面に値をセット
		SetValues();
	}

	/// <summary>
	/// 画面に値をセット（※DBから定期解約理由区分設定取得）
	/// </summary>
	private void SetValues()
	{
		// 定期解約理由区分設定取得
		this.CancelReasonList = new FixedPurchaseService().GetCancelReasonAll();

		// 画面に値をセット
		var cancelReasonInputList = this.CancelReasonList.Select(cr => new FixedPurchaseCancelReasonInput(cr)).ToArray();

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			this.FixedPurchaseCancelReasonTranslationData = GetFixedPurchaseCancelReasonTranslationSettings();
			cancelReasonInputList = cancelReasonInputList.Select(SetCancelReasonNameTranslationData).ToArray();
		}
		SetValues(cancelReasonInputList);
	}
	/// <summary>
	/// 画面に値をセット
	/// </summary>
	/// <param name="cancelReasonInputList">定期解約理由区分設定入力内容リスト</param>
	private void SetValues(FixedPurchaseCancelReasonInput[] cancelReasonInputList)
	{
		// 1件以上の場合
		if (cancelReasonInputList.Length > 0)
		{
			// 一括ボタン表示
			btnAllUpdateTop.Visible = btnAllUpdateBottom.Visible = true;
			// 該当データなしメッセージ非表示
			trListError.Visible = false;
		}
		else
		{
			// 一括ボタン非表示
			btnAllUpdateTop.Visible = btnAllUpdateBottom.Visible = false;
			// 該当データなしメッセージ表示
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// データソースにセット&データバインド
		rCancelReasonList.DataSource = cancelReasonInputList;
		rCancelReasonList.DataBind();
	}

	/// <summary>
	/// 定期解約理由区分設定入力内容リスト取得
	/// </summary>
	/// <returns>定期解約理由区分設定入力内容リスト</returns>
	private IEnumerable<FixedPurchaseCancelReasonInput> GetCancelReasonInputList()
	{
		var no = 0;
		foreach (RepeaterItem item in rCancelReasonList.Items)
		{
			// 入力内容取得
			no++;
			var cancelReasonName = ((TextBox)item.FindControl("tbCancelReasonName")).Text.Trim();
			var displayOrder = ((TextBox)item.FindControl("tbDisplayOrder")).Text.Trim();
			var displayKbn = (((CheckBox)item.FindControl("cbPcDisplayFlg")).Checked) ? FixedPurchaseCancelReasonModel.DisplayKbnValue.PC.ToString() : "";
			if (((CheckBox)item.FindControl("cbEcDisplayFlg")).Checked)
			{
				if (displayKbn.Length != 0) displayKbn += ",";
				displayKbn += FixedPurchaseCancelReasonModel.DisplayKbnValue.EC.ToString();
			}

			// 新規?
			if ((item.ItemIndex + 1) > this.CancelReasonList.Length)
			{
				yield return new FixedPurchaseCancelReasonInput()
				{
					No = no,
					CancelReasonId = ((TextBox)item.FindControl("tbCancelReasonId")).Text.Trim(),
					CancelReasonName = cancelReasonName,
					DisplayOrder = displayOrder,
					DisplayKbn = displayKbn,
					IsDelete = false,
					IsNew = true
				};
			}
			// 更新?
			else
			{
				var cancelReason = new FixedPurchaseCancelReasonInput(this.CancelReasonList[item.ItemIndex]);
				cancelReason.No = no;
				cancelReason.CancelReasonName = cancelReasonName;
				cancelReason.DisplayOrder = displayOrder;
				cancelReason.DisplayKbn = displayKbn;
				cancelReason.IsDelete = ((CheckBox)item.FindControl("cbDeleteFlg")).Checked;
				cancelReason.IsNew = false;

				yield return cancelReason;
			}
		}
	}

	#region -GetFixedPurchaseCancelReasonTranslationSettings 定期解約理由区分設定翻訳情報取得
	/// <summary>
	/// 定期解約理由区分設定翻訳情報取得
	/// </summary>
	/// <returns>定期解約理由区分設定翻訳情報</returns>
	private NameTranslationSettingModel[] GetFixedPurchaseCancelReasonTranslationSettings()
	{
		var searchCondition = new NameTranslationSettingSearchCondition
		{
			DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON,
		};
		var translationData = new NameTranslationSettingService().GetTranslationSettingsByDataKbn(searchCondition);
		return translationData;
	}
	#endregion

	#region -SetCancelReasonNameTranslationData 解約理由区分名翻訳情報設定
	/// <summary>
	/// 解約理由区分名翻訳情報設定
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	private FixedPurchaseCancelReasonInput SetCancelReasonNameTranslationData(FixedPurchaseCancelReasonInput input)
	{
		input.CancelReasonNameTranslationData = this.FixedPurchaseCancelReasonTranslationData
			.Where(d => (d.MasterId1 == input.CancelReasonId)).ToArray();
		return input;
	}
	#endregion

	#region +プロパティ
	/// <summary>定期解約理由区分設定リスト</summary>
	private FixedPurchaseCancelReasonModel[] CancelReasonList
	{
		get { return (FixedPurchaseCancelReasonModel[])ViewState["CancelReasonList"]; }
		set { ViewState["CancelReasonList"] = value; }
	}
	/// <summary>定期購入解約理由情報翻訳設定情報</summary>
	protected NameTranslationSettingModel[] FixedPurchaseCancelReasonTranslationData
	{
		get { return (NameTranslationSettingModel[])ViewState["fixedpurchasecancelreason_translation_data"]; }
		set { ViewState["fixedpurchasecancelreason_translation_data"] = value; }
	}
	#endregion

	/// <summary>
	/// 翻訳設定出力リンククリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbExportTranslationData_OnClick(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM] = new string[0];
		Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN] = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_FIXEDPURCHASECANCELREASON;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_EXPORT);
	}
}