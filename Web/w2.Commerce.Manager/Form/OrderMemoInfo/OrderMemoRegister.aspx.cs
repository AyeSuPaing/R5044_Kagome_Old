/*
=========================================================================================================
  Module      : 注文メモ情報登録／編集ページ処理(OrderMemoRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Util;
using System.Text;

public partial class Form_OrderMemoInfo_OrderMemoRegister : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			string strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, strActionStatus);

			//------------------------------------------------------
			// 処理区分チェック
			//------------------------------------------------------
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(strActionStatus);

			//------------------------------------------------------
			// 表示用値設定処理
			//------------------------------------------------------
			// 新規？
			var htDateInput = (Hashtable)Session[Constants.SESSIONPARAM_KEY_ORDER_MEMO_INFO];
			if (strActionStatus == Constants.ACTION_STATUS_INSERT)
			{
				// デフォルト設定
				Hashtable htOrderMemoSetting = new Hashtable();
				htOrderMemoSetting.Add(Constants.FIELD_ORDERMEMOSETTING_DISPLAY_ORDER, 10);
				htOrderMemoSetting.Add(Constants.FIELD_ORDERMEMOSETTING_MAX_LENGTH, 400);
				this.OrderMemoSetting = htOrderMemoSetting;
			}
			// コピー新規・編集？
			else if ((strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
				|| (strActionStatus == Constants.ACTION_STATUS_UPDATE))
			{
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatements = new SqlStatement("OrderMemoSetting", "GetOrderMemoSettingFromId"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID, Request[Constants.REQUEST_KEY_ORDER_MEMO_ID]);

					DataView dvOrderMemo = sqlStatements.SelectSingleStatementWithOC(sqlAccessor, htInput);

					// 該当データ無しの場合
					if (dvOrderMemo.Count == 0)
					{
						// エラーページへ
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}
					this.OrderMemoSetting = dvOrderMemo[0];

					if ((string.IsNullOrEmpty(dvOrderMemo[0][Constants.FIELD_ORDERMEMOSETTING_TERM_BGN].ToString()) == false)
						&& (string.IsNullOrEmpty(dvOrderMemo[0][Constants.FIELD_ORDERMEMOSETTING_TERM_END].ToString()) == false)
						&& ((htDateInput == null) || (this.IsBackFromConfirm == false)))
					{
						var orderMemoSettingTermDateStart =
							DateTime.Parse(dvOrderMemo[0][Constants.FIELD_ORDERMEMOSETTING_TERM_BGN].ToString());

						var orderMemoSettingTermDateEnd =
							DateTime.Parse(dvOrderMemo[0][Constants.FIELD_ORDERMEMOSETTING_TERM_END].ToString());

						ucDisplayPeriod.SetPeriodDate(orderMemoSettingTermDateStart, orderMemoSettingTermDateEnd);
					}
				}
			}
			// それ以外の場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
			if ((htDateInput != null) && this.IsBackFromConfirm)
			{
				var orderMemoSettingTermDateStart =
					DateTime.Parse(StringUtility.ToEmpty(htDateInput[Constants.FIELD_ORDERMEMOSETTING_TERM_BGN]));

				var orderMemoSettingTermDateEnd =
					DateTime.Parse(StringUtility.ToEmpty(htDateInput[Constants.FIELD_ORDERMEMOSETTING_TERM_END]));

				ucDisplayPeriod.SetPeriodDate(orderMemoSettingTermDateStart, orderMemoSettingTermDateEnd);
				Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = null;
			}
			// データバインド
			DataBind();
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		//------------------------------------------------------
		// アクションごと表示切替
		//------------------------------------------------------
		if (strActionStatus == Constants.ACTION_STATUS_INSERT)
		{
			trRegister.Visible = true;
			trOrderMemoRegist.Visible = true;
		}
		else if (strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
		{
			trRegister.Visible = true;
			trOrderMemoRegist.Visible = true;
		}
		else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			trEdit.Visible = true;
			trOrderMemoEdit.Visible = true;
		}

		this.ucDisplayPeriod.SetPeriodDateToday();
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// パラメタ格納
		//------------------------------------------------------
		Hashtable htParam = new Hashtable();
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = 1;

		//------------------------------------------------------
		// 処理ステータス
		//------------------------------------------------------		
		// 新規・コピー新規
		string strOrdermemoId = "";
		string strValidator = "";
		if (((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT)
			|| ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_COPY_INSERT))
		{
			strOrdermemoId = tbOrderMemoId.Text;
			strValidator = "OrderMemoSettingRegist";
		}
		// 変更
		else if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
		{
			strOrdermemoId = lOrderMemoId.Text;
			strValidator = "OrderMemoSettingModify";
		}

		htParam.Add(Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID, strOrdermemoId);		// 注文メモID
		htParam.Add(Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_NAME, tbOrderMemoName.Text);		// 注文メモ名称
		htParam.Add(Constants.FIELD_ORDERMEMOSETTING_HEIGHT, (tbHeight.Text == "") ? null : StringUtility.ToHankaku(tbHeight.Text));		// 入力欄縦幅
		htParam.Add(Constants.FIELD_ORDERMEMOSETTING_WIDTH, (tbWidth.Text == "") ? null : StringUtility.ToHankaku(tbWidth.Text));		// 入力欄横幅
		htParam.Add(Constants.FIELD_ORDERMEMOSETTING_CSS_CLASS, tbCssClass.Text);						// CSS Class
		htParam.Add(Constants.FIELD_ORDERMEMOSETTING_MAX_LENGTH, StringUtility.ToHankaku(tbMaxLength.Text));						// 入力可能最大文字数
		htParam.Add(Constants.FIELD_ORDERMEMOSETTING_DEFAULT_TEXT, tbDefaultText.Text);					// デフォルト文字列

		// 有効期間（開始）
		var expBgnDay = ucDisplayPeriod.StartDateTimeString;
		htParam.Add(Constants.FIELD_ORDERMEMOSETTING_TERM_BGN, expBgnDay);

		// 有効期間（終了）
		var expEndDay = ucDisplayPeriod.EndDateTimeString;
		htParam.Add(Constants.FIELD_ORDERMEMOSETTING_TERM_END, expEndDay);

		htParam.Add(Constants.FIELD_ORDERMEMOSETTING_DISPLAY_ORDER, StringUtility.ToHankaku(tbDisplayOrder.Text));				// 表示順
		htParam.Add(Constants.FIELD_ORDERMEMOSETTING_VALID_FLG, cbValidFlg.Checked ? Constants.FLG_ORDER_MEMO_SETTING_VALID_FLG_VALID : Constants.FLG_ORDER_MEMO_SETTING_VALID_FLG_INVALID);		// 有効フラグ
		htParam.Add(Constants.FIELD_SHOPSHIPPING_LAST_CHANGED, this.LoginOperatorName);			// 最終更新者

		// 入力チェック＆重複チェック
		string strErrorMessages = Validator.Validate(strValidator, htParam);

		// 開始日と終了日の比較
		if (Validator.CheckDateRange(expBgnDay, expEndDay) == false)
		{
			strErrorMessages += WebMessages.GetMessages(WebMessages.INPUTCHECK_DATERANGE)
				.Replace("@@ 1 @@",
				//「有効期間」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER,
						Constants.VALUETEXT_PARAM_ORDER_MEMO_INFO,
						Constants.VALUETEXT_PARAM_ORDER_MEMO_INFO_VALIDITY_PERIOD));
		}

		if (Validator.IsHalfwidthNumber((string)htParam[Constants.FIELD_ORDERMEMOSETTING_MAX_LENGTH]))
		{
			// デフォルトテキスト入力文字数チェック(入力した最大入力可能文字数)
			strErrorMessages += Validator.CheckLengthMaxError(
				//「デフォルトテキスト」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER,
					Constants.VALUETEXT_PARAM_ORDER_MEMO_INFO,
					Constants.VALUETEXT_PARAM_ORDER_MEMO_INFO_DEFAULT),
				(string)htParam[Constants.FIELD_ORDERMEMOSETTING_DEFAULT_TEXT],
				int.Parse((string)htParam[Constants.FIELD_ORDERMEMOSETTING_MAX_LENGTH]));
		}

		// エラーページへ
		if (strErrorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// パラメタをセッションへ格納
		Session[Constants.SESSIONPARAM_KEY_ORDER_MEMO_INFO] = htParam;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];

		// 注文メモ情報確認ページへ遷移			
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_MEMO_CONFIRM +
			"?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS]);
	}
	/// <summary>注文メモ設定</summary>
	public object OrderMemoSetting { get; private set; }

	/// <summary>確認画面から戻ってきたか</summary>
	protected bool IsBackFromConfirm
	{
		get { return (Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] != null); }
	}
}
