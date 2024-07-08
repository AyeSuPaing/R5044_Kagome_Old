/*
=========================================================================================================
  Module      : ポイント基本ルール登録ページ処理(PointRuleRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Web.UI.WebControls;
using Input.Point;
using w2.Domain.Point;

public partial class Form_PointRule_PointRuleRegister : BasePage
{
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		ClearBrowserCache();

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

			// コンポーネント初期化
			InitializeComponents(strActionStatus);

			//------------------------------------------------------
			// 表示用値設定処理
			//------------------------------------------------------
			switch (strActionStatus)
			{
				case Constants.ACTION_STATUS_INSERT:
					if (Session[MpSessionWrapper.SESSION_KEY_POINTRULE_INPUT] != null)
					{
						this.Input = (PointRuleInput)Session[MpSessionWrapper.SESSION_KEY_POINTRULE_INPUT];
						ViewState.Add(Constants.FIELD_POINTRULE_POINT_RULE_ID, this.Input.PointRuleId);
					}
					else
					{
						this.Input = new PointRuleInput();
					}
					break;

				case Constants.ACTION_STATUS_UPDATE:
				case Constants.ACTION_STATUS_COPY_INSERT:
					// セッションよりポイント基本ルール情報取得
					this.Input = (PointRuleInput)this.Session[MpSessionWrapper.SESSION_KEY_POINTRULE_INPUT]; // ポイント基本ルール情報
					ViewState.Add(Constants.FIELD_POINTRULE_POINT_RULE_ID, this.Input.PointRuleId); // ポイントルールID
					SetDefaultValue();
					break;

				// それ以外の場合
				default:
					// エラーページへ
					Session[Constants.SESSION_KEY_ERROR_MSG] =
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
					break;
			}

			SetDefaultValue();
			// データバインド
			DataBind();

			// 入力域制御
			rbBuyIncKbn_CheckedChanged(sender, e);
			ddlPointIncKbn_SelectedIndexChanged(sender, e);
			PointKbn_OnCheckedChanged(sender, e);
			ExpireKbn_CheckedChanged(sender, e);
		}

		// ポイント有効期限延長
		trPointExpEntend.Visible = IsPointExpKbnValid(Constants.FLG_USERPOINT_POINT_KBN_BASE);
	}
	#endregion

	#region -InitializeComponents 表示コンポーネント初期化
	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		switch (strActionStatus)
		{
			// 新規登録？
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				trRegister.Visible = true;
				break;

			// 更新？
			case Constants.ACTION_STATUS_UPDATE:
				trEdit.Visible = true;
				break;
		}

		// ポイント有効期限延長DDL作成
		ddlPointExpEntend.Items.Add(new ListItem("", ""));
		ddlPointExpEntend.Items.AddRange(Enumerable.Range(0, 37).Select(i => new ListItem(i.ToString("00"))).ToArray());

		// ポイント加算区分ドロップダウン作成
		ddlPointIncKbn.Items.Add(new ListItem("", ""));
		ddlPointIncKbn.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_POINTRULE,
				Constants.FIELD_POINTRULE_POINT_INC_KBN)
					// クリックポイントオプション無効の場合は表示しない
					.Where(x => (Constants.POINTRULE_OPTION_CLICKPOINT_ENABLED || (x.Value != Constants.FLG_POINTRULE_POITN_INC_KBN_CLICK)))
					.Where(item => ((Constants.PRODUCTREVIEW_ENABLED && Constants.REVIEW_REWARD_POINT_ENABLED)
						|| (item.Value != Constants.FLG_POINTRULE_POINT_INC_KBN_REWARD_POINT)))
					.Where(item => ((Constants.CROSS_POINT_OPTION_ENABLED == false)
						|| Constants.CROSS_POINT_LOGIN_POINT_ENABLED
						|| (item.Value != Constants.FLG_POINTRULE_POINT_INC_KBN_LOGIN)))
					.ToArray());

		// ポイント発行オフセット区分DDL作成
		ddlEffectiveOffsetType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_EFFECTIVE_OFFSET_TYPE));

		// 期間限定ポイント期間区分DDL作成
		ddlTermType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_TERM_TYPE));

		// ポイント加算方法DDL作成
		// 通常購入
		ddlPointIncType.Items.Add(new ListItem("", ""));
		ddlPointIncType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_INC_TYPE));

		// 定期購入
		ddlFixedPurchasePointIncType.Items.Add(new ListItem("", ""));
		ddlFixedPurchasePointIncType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_TYPE));
	}
	#endregion

	#region -GetPointExpKbn ポイント有効期限設定の有無
	/// <summary>
	/// ポイント有効期限設定の有無
	/// </summary>
	/// <param name="pointKbn">ポイント区分</param>
	/// <returns>ポイント有効期限設定の有無(True:有 False:無)</returns>
	private bool IsPointExpKbnValid(string pointKbn)
	{
		// 該当ポイント区分の情報を特定する
		var point = new PointService().GetPointMaster().FirstOrDefault(i => (i.PointKbn == pointKbn));

		if (point == null)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHOPPOINT_NO_DATA);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		return (point.PointExpKbn == Constants.FLG_POINT_POINT_EXP_KBN_VALID);
	}
	#endregion

	#region -CreateInput 入力値クラスを生成
	/// <summary>
	/// 入力値クラスを生成
	/// </summary>
	/// <returns></returns>
	private PointRuleInput CreateInput()
	{
		// 変数宣言
		var pointRuleId = (string)ViewState[Constants.FIELD_POINTRULE_POINT_RULE_ID];
		var incType = string.Empty;
		var incRate = string.Empty;
		var inc = string.Empty;
		var userTempFlg = Constants.FLG_POINTRULE_USE_TEMP_FLG_INVALID; // 仮ポイントを利用しない
		var incFixedPurchaseType = string.Empty;
		var incFixedPurchaseRate = string.Empty;
		var incFixedPurchase = string.Empty;

		var input = new PointRuleInput();

		input.DeptId = "0";
		input.PointRuleId = pointRuleId;
		input.PointRuleName = tbPointRuleName.Text;
		input.PointIncKbn = ddlPointIncKbn.SelectedValue;

		// ポイント区分
		input.PointKbn = rbPointKbnNormalPoint.Checked
			? Constants.FLG_USERPOINT_POINT_KBN_BASE
			: rbPointKbnLimitedTermPoint.Checked
				? Constants.FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT
				: string.Empty;

		// 期間限定ポイントのみの入力値
		if (rbPointKbnLimitedTermPoint.Checked)
		{
			// 有効期間を指定する場合
			if (rbLimitedTermPointPeriod.Checked)
			{
				// 入力チェック用 DBには反映しない
				input.LimitedTermPointExpireKbn = PointRuleInput.LIMTIED_TERM_POINT_EXPIRE_KBN_ABSOLUTE;
				input.PeriodBegin = ucPeriod.StartDateTimeString;
				input.PeriodEnd = ucPeriod.EndDateTimeString;
			}
			// 有効期限を指定する場合
			else if (rbLimitedTermPointExpirationDay.Checked)
			{
				input.LimitedTermPointExpireKbn = PointRuleInput.LIMTIED_TERM_POINT_EXPIRE_KBN_RELATIVE;
				input.EffectiveOffset = tbEffectiveOffset.Text.Trim();
				input.EffectiveOffsetType = ddlEffectiveOffsetType.SelectedValue;
				input.Term = tbTerm.Text.Trim();
				input.TermType = ddlTermType.SelectedValue;
			}
		}

		// ルール有効期間（開始）
		var ucExpireStart = ucExpireDatePeriod.StartDateTimeString;
		var ucExpireEnd = ucExpireDatePeriod.EndDateTimeString;
		input.ExpBgn = (input.UseScheduleIncKbnPointRule == false) ? ucExpireStart : null;
		// ルール有効期間（開始）(重複チェック用)
		input.ExpBgnChk = ((input.UseScheduleIncKbnPointRule == false)
			&& (input.IsClickPoint == false))
				? ucExpireDatePeriod.HfStartDate.Value
				: null;

		// ルール有効期間（終了）
		input.ExpEnd = (input.UseScheduleIncKbnPointRule == false) ? ucExpireEnd : null;
		// ルール有効期間（終了）(重複チェック用)
		input.ExpEndChk = ((input.UseScheduleIncKbnPointRule == false)
			&& (input.IsClickPoint == false))
				? ucExpireDatePeriod.HfEndDate.Value
				: null;

		// ポイント有効期限延長
		input.PointExpExtend = Constants.CROSS_POINT_OPTION_ENABLED
			? "+000000"
			: ((string.IsNullOrEmpty(ddlPointExpEntend.SelectedValue) == false)
				? string.Format("+00{0}00", ddlPointExpEntend.SelectedValue)
				: string.Empty);

		// 優先順位
		input.Priority = "100";
		// 基本ルールとの二重適用（この画面は常に「許可しない」）
		input.AllowDuplicateApplyFlg = Constants.FLG_POINTRULE_DUPLICATE_APPLY_DISALLOW;
		// 有効フラグ
		input.ValidFlg = cbValidFlg.Checked
			? Constants.FLG_POINTRULE_VALID_FLG_VALID
			: Constants.FLG_POINTRULE_VALID_FLG_INVALID;
		// 最終更新者
		input.LastChanged = this.LoginOperatorName;

		// ポイントルール区分はこの画面の場合は常に「01：基本」
		input.PointRuleKbn = Constants.FLG_POINTRULE_POINT_RULE_KBN_BASE;

		if (input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BUY)
		{
			// 購入時ポイント発行の場合

			// 購入金額毎
			if (rbBuyIncKbnPrice.Checked)
			{
				// ポイント使用設定
				if (ddlPointIncType.SelectedValue == Constants.FLG_POINTRULE_INC_TYPE_NUM
					|| ddlPointIncType.SelectedValue == Constants.FLG_POINTRULE_INC_TYPE_RATE)
				{
					incType = ddlPointIncType.SelectedValue;
					inc = tbIncPoint.Text;
					incRate = (ddlPointIncType.SelectedValue == Constants.FLG_POINTRULE_INC_TYPE_NUM) ? "0" : tbIncPoint.Text;
				}
				// 定期ポイント使用設定
				if (ddlFixedPurchasePointIncType.SelectedValue == Constants.FLG_POINTRULE_INC_TYPE_NUM
					|| ddlFixedPurchasePointIncType.SelectedValue == Constants.FLG_POINTRULE_INC_TYPE_RATE)
				{
					incFixedPurchaseType = ddlFixedPurchasePointIncType.SelectedValue;
					incFixedPurchase = ((tbIncFixedPurchasePoint.Text == string.Empty) ? "0" : tbIncFixedPurchasePoint.Text);
					incFixedPurchaseRate =
						(ddlFixedPurchasePointIncType.SelectedValue == Constants.FLG_POINTRULE_INC_TYPE_NUM)
							? "0"
							: ((tbIncFixedPurchasePoint.Text == string.Empty) ? "0" : tbIncFixedPurchasePoint.Text);
				}
			}
			// 商品毎
			else if (rbBuyIncKbnProduct.Checked)
			{
				incType = Constants.FLG_POINTRULE_INC_TYPE_PRODUCT;
				inc = "0";
				incRate = "0";
				incFixedPurchaseType = Constants.FLG_POINTRULE_INC_TYPE_PRODUCT;
				incFixedPurchase = "0";
				incFixedPurchaseRate = "0";
			}

			// 仮ポイントを利用するか
			userTempFlg = input.NeedsToUseTemporaryPointInSetting
				? Constants.FLG_POINTRULE_USE_TEMP_FLG_VALID
				: Constants.FLG_POINTRULE_USE_TEMP_FLG_INVALID;
		}
		else if (input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY)
		{
			// 初回購入ポイント発行の場合

			// ポイント加算数を使用
			if (ddlPointIncType.SelectedValue == Constants.FLG_POINTRULE_INC_TYPE_NUM)
			{
				incType = Constants.FLG_POINTRULE_INC_TYPE_NUM;
				inc = tbIncPoint.Text;
				incRate = "0";
			}
			// ポイント加算率を使用
			else if (ddlPointIncType.SelectedValue == Constants.FLG_POINTRULE_INC_TYPE_RATE)
			{
				incType = Constants.FLG_POINTRULE_INC_TYPE_RATE;
				inc = tbIncPoint.Text;
				incRate = tbIncPoint.Text;
			}

			// 仮ポイントを利用するか
			userTempFlg = input.NeedsToUseTemporaryPointInSetting
				? Constants.FLG_POINTRULE_USE_TEMP_FLG_VALID
				: Constants.FLG_POINTRULE_USE_TEMP_FLG_INVALID;
		}
		// 新規登録ポイント発行、ログイン毎ポイント発行、汎用ポイントルール、お誕生日ポイントの場合
		else if ((input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_USER_REGISTER)
			|| (input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_LOGIN)
			|| (input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_VERSATILE_POINT_RULE)
			|| (input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BIRTHDAY_POINT)
			|| (input.PointIncKbn == Constants.FLG_POINTRULE_POITN_INC_KBN_CLICK)
			|| (input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_REWARD_POINT))
		{
			// ポイント加算数を使用
			incType = Constants.FLG_POINTRULE_INC_TYPE_NUM;
			inc = tbIncPoint.Text;
			incRate = "0";
		}

		// ポイント加算区分が選択されている場合
		if ((input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BUY)
			|| (input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY)
			|| (input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_USER_REGISTER)
			|| (input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_LOGIN)
			|| (input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_VERSATILE_POINT_RULE)
			|| (input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BIRTHDAY_POINT)
			|| (input.PointIncKbn == Constants.FLG_POINTRULE_POITN_INC_KBN_CLICK)
			|| (input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_REWARD_POINT))
		{
			// 仮ポイント使用フラグ
			input.UseTempFlg = userTempFlg;
			// ポイント加算方法
			input.IncType = incType;
			input.IncFixedPurchaseType = incFixedPurchaseType;
			// ポイント加算ルール(必須チェック用)
			input.Inc = inc;
			input.IncFixedPurchase = incFixedPurchase;
		}

		// 入力チェックを回避するため、 加算数、加算率のどちらかを
		// 格納する。ただし、ステートメント実行時に両方のパラメータを渡しているため設定すること
		if (incType == Constants.FLG_POINTRULE_INC_TYPE_NUM)
		{
			// ポイント加算数の場合
			// ポイント加算ルール(加算数)
			input.IncNum = inc;
		}
		else
		{
			// ポイント加算率の場合
			// ポイント加算ルール(加算率)
			input.IncRate = incRate;
		}

		if (incFixedPurchaseType == Constants.FLG_POINTRULE_INC_TYPE_NUM)
		{
			// 定期ポイント加算数の場合(加算数)
			input.IncFixedPurchaseNum = incFixedPurchase;
		}
		else
		{
			// 定期ポイント加算率の場合(加算率)
			input.IncFixedPurchaseRate = incFixedPurchaseRate;
		}

		return input;
	}
	#endregion

	#region #btnConfirmTop_Click 確認するボタンクリック
	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirmTop_Click(object sender, System.EventArgs e)
	{
		// 入力値生成
		var input = this.CreateInput();

		// パラメタをセッションへ格納
		MpSessionWrapper.PointRuleInput = input;

		// 入力チェック＆重複チェック
		var strErrorMessages = input.Validate();

		if (strErrorMessages != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		// ポイント基本ルール確認ページへ遷移
		Response.Redirect(
			Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_CONFIRM + "?"
			+ Constants.REQUEST_KEY_ACTION_STATUS + "=" + (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS]);
	}
	#endregion

	#region #ddlPointIncKbn_SelectedIndexChanged ポイント加算区分選択
	/// <summary>
	/// ポイント加算区分選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlPointIncKbn_SelectedIndexChanged(object sender, EventArgs e)
	{
		// ポイント計算方法表示非表示
		if (ddlPointIncKbn.SelectedValue == Constants.FLG_POINTRULE_POINT_INC_KBN_BUY)
		{
			// 表示
			trBuyIncKbn.Style["display"] = "";
		}
		else
		{
			trBuyIncKbn.Style["display"] = "none";
		}

		// ポイント有効期限延長
		trPointExpEntend.Visible = IsPointExpKbnValid(Constants.FLG_USERPOINT_POINT_KBN_BASE);

		// ポイント有効期間
		trPointExpire.Visible = true;

		// ポイント使用制御（ポイント加算区分選択時）
		switch (ddlPointIncKbn.SelectedValue)
		{
			// 購入時ポイント
			case Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY:
			case Constants.FLG_POINTRULE_POINT_INC_KBN_BUY:
				rbLimitedTermPointPeriod.Checked = false;
				rbLimitedTermPointExpirationDay.Checked = true;
				rbLimitedTermPointPeriod.Enabled = false;
				ExpireKbn_CheckedChanged(sender, e);
				rbBuyIncKbn_CheckedChanged(sender, e);
				dvFixedPurchaseSetting.Visible = (ddlPointIncKbn.SelectedValue == Constants.FLG_POINTRULE_POINT_INC_KBN_BUY);
				break;

			// 新規登録ポイント発行、ログイン毎ポイント発行、汎用ポイントルール、お誕生日ポイントの場合
			case Constants.FLG_POINTRULE_POINT_INC_KBN_USER_REGISTER:
			case Constants.FLG_POINTRULE_POINT_INC_KBN_LOGIN:
			case Constants.FLG_POINTRULE_POINT_INC_KBN_VERSATILE_POINT_RULE:
			case Constants.FLG_POINTRULE_POINT_INC_KBN_BIRTHDAY_POINT:
			case Constants.FLG_POINTRULE_POITN_INC_KBN_CLICK:
			case Constants.FLG_POINTRULE_POINT_INC_KBN_REWARD_POINT:
				ddlPointIncType.SelectedValue = Constants.FLG_POINTRULE_INC_TYPE_NUM;
				tbIncPoint.Enabled = true;
				ddlPointIncType.Enabled = false;
				dvFixedPurchaseSetting.Visible = false;
				rbLimitedTermPointPeriod.Enabled = true;
				if ((ddlPointIncKbn.SelectedValue == Constants.FLG_POINTRULE_POINT_INC_KBN_VERSATILE_POINT_RULE)
					|| (ddlPointIncKbn.SelectedValue == Constants.FLG_POINTRULE_POINT_INC_KBN_BIRTHDAY_POINT))
				{
					trPointExpire.Visible = false;
				}
				break;
		}
	}
	#endregion

	#region #rbBuyIncKbn_CheckedChanged ポイント計算方法変更
	/// <summary>
	/// ポイント計算方法変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbBuyIncKbn_CheckedChanged(object sender, EventArgs e)
	{
		// 購入金額毎の場合
		if (rbBuyIncKbnPrice.Checked)
		{
			ddlPointIncType.Enabled = true;
			tbIncPoint.Enabled = true;
			tbIncFixedPurchasePoint.Enabled = true;
			ddlFixedPurchasePointIncType.Enabled = true;
		}
		// 商品毎の場合
		else if (rbBuyIncKbnProduct.Checked)
		{
			ddlPointIncType.Enabled = false;
			tbIncPoint.Enabled = false;
			tbIncFixedPurchasePoint.Enabled = false;
			ddlFixedPurchasePointIncType.Enabled = false;
		}
	}
	#endregion

	#region PointKbn_OnCheckedChanged ポイント区分選択
	/// <summary>
	/// ポイント区分選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void PointKbn_OnCheckedChanged(object sender, EventArgs e)
	{
		trLimitedTermPointExpiration.Attributes.Add(
			"style",
			rbPointKbnLimitedTermPoint.Checked
				? "display: contents"
				: "display: none");

		var selectedOld = ddlPointExpEntend.SelectedValue;

		if (rbPointKbnNormalPoint.Checked)
		{
			ddlPointExpEntend.Items.Remove(new ListItem("00"));
			if (selectedOld == "00") return;
		}
		else
		{
			// 単純にADDすると順番がおかしくなるので一回クリアする
			ddlPointExpEntend.Items.Clear();
			ddlPointExpEntend.Items.Add(new ListItem("", ""));
			ddlPointExpEntend.Items.AddRange(Enumerable.Range(0, 37).Select(i => new ListItem(i.ToString("00"))).ToArray());
		}

		foreach (ListItem item in ddlPointExpEntend.Items)
		{
			item.Selected = (selectedOld == item.Value);
		}

		// ポイント有効期限延長 表示非表示スイッチ
		trPointExpEntend.Visible = IsPointExpKbnValid(
			(string.IsNullOrEmpty(this.Input.PointKbn) == false)
				? this.Input.PointKbn
				: Constants.FLG_USERPOINT_POINT_KBN_BASE);
	}
	#endregion

	#region ExpireKbn_CheckedChanged 期限/期間選択
	/// <summary>
	/// 期限/期間選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ExpireKbn_CheckedChanged(object sender, EventArgs e)
	{
		pLimitedTermPointExpirationDay.Visible = rbLimitedTermPointExpirationDay.Checked;
		spLimitedTermPointPeriod.Attributes.Add(
			"style",
			rbLimitedTermPointPeriod.Checked
				? "margin: 0 15px; display: inline-block"
				: "margin: 0 15px; display: none");
	}
	#endregion

	/// <summary>
	/// ドロップダウン初期値設定
	/// </summary>
	private void SetDefaultValue()
	{
		if (ddlPointIncType.Items.FindByValue(StringUtility.ToEmpty(this.Input.IncType)) != null)
		{
			ddlPointIncType.SelectedValue = StringUtility.ToEmpty(this.Input.IncType);
		}
		if (ddlFixedPurchasePointIncType.Items.FindByValue(StringUtility.ToEmpty(this.Input.IncFixedPurchaseType)) != null)
		{
			ddlFixedPurchasePointIncType.SelectedValue = StringUtility.ToEmpty(this.Input.IncFixedPurchaseType);
		}

		if (string.IsNullOrEmpty(this.Input.ExpBgn))
		{
			ucPeriod.SetPeriodDate(
				this.PeriodBegin,
				this.PeridEnd);

			ucExpireDatePeriod.SetPeriodDate(
				this.ExpireBegin,
				this.ExpireEnd);
		}
		else
		{
			var startExpireDatePeriod =
				DateTime.Parse(this.Input.ExpBgn);
			var endExpireDatePeriod =
				DateTime.Parse(this.Input.ExpEnd);
			ucExpireDatePeriod.SetPeriodDate(startExpireDatePeriod, endExpireDatePeriod);

			var startPeriod =
				DateTime.Parse(this.Input.ExpBgn);
			var endPeriod =
				DateTime.Parse(this.Input.ExpEnd);
			ucPeriod.SetPeriodDate(startPeriod, endPeriod);
		}
	}

	#region プロパティ
	/// <summary>ポイントルール入力値クラス</summary>
	protected PointRuleInput Input
	{
		get { return (PointRuleInput)ViewState["point_rule_input"]; }
		set { ViewState["point_rule_input"] = value; }
	}
	/// <summary>有効期間：開始日</summary>
	protected DateTime ExpireBegin
	{
		get
		{
			if (this.Input == null) return DateTime.Now.Date;
			DateTime date;
			return DateTime.TryParse(this.Input.ExpBgn, out date) ? date : DateTime.Now.Date;
		}
	}
	/// <summary>有効期間：終了日</summary>
	protected DateTime ExpireEnd
	{
		get
		{
			if (this.Input == null) return DateTime.Now.Date.AddYears(1);
			DateTime date;
			return DateTime.TryParse(this.Input.ExpEnd, out date) ? date : DateTime.Now.Date.AddYears(1);
		}
	}
	/// <summary>期間限定ポイント有効期間：開始日</summary>
	protected DateTime PeriodBegin
	{
		get
		{
			DateTime dt;
			// Inputから値を取れない場合今月（システム日付）の月初を返す
			if ((this.Input == null)
				|| DateTime.TryParse(this.Input.PeriodBegin, out dt) == false)
			{
				dt = DateTime.Now;
				return new DateTime(dt.Year, dt.Month, 1);
			}

			return dt;
		}
	}
	/// <summary>期間限定ポイント有効期間：終了日</summary>
	protected DateTime PeridEnd
	{
		get
		{
			DateTime dt;
			// Inputから値を取れない場合今月（システム日付）の月末を返す
			if ((this.Input == null)
				|| DateTime.TryParse(this.Input.PeriodEnd, out dt) == false)
			{
				dt = DateTime.Now;
				return new DateTime(dt.Year, dt.Month, DateTimeUtility.GetLastDayOfMonth(dt.Year, dt.Month));
			}

			return dt;
		}
	}
	#endregion
}
