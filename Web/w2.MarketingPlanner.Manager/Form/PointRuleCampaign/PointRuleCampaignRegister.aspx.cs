/*
=========================================================================================================
  Module      : ポイントキャンペーンルール登録ページページ処理(PointRuleCampaignRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Input.Point;
using w2.App.Common.Util;
using w2.Common.Web;
using w2.Domain.Point;
using Calendar = System.Web.UI.WebControls.Calendar;

public partial class Form_PointRuleCampaign_PointRuleCampaignRegister : BasePage
{
	//=========================================================================================
	// カレンダー用定数
	//=========================================================================================
	protected const string CAMPAIGN_CALENDAR_DATE = "campaign_calendar_date";
	protected const string CAMPAIGN_CALENDAR_DATE_NO = "campaign_calendar_date_no";

	protected ArrayList m_alCampaign = new ArrayList();					// カレンダーリピータデータバインド用

	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		// 指定した言語ロケールIDにより、カルチャーを変換する
		if (Constants.GLOBAL_OPTION_ENABLE && string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false)
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE);
		}

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
			// 新規かつセッション情報あり
			if ((strActionStatus == Constants.ACTION_STATUS_INSERT) && MpSessionWrapper.PointRuleInput != null)
			{
				trRegister.Visible = true;

				// セッションよりクーポン情報取得
				this.Input = MpSessionWrapper.PointRuleInput;
				ViewState.Add(Constants.FIELD_POINTRULE_POINT_RULE_ID, Input.PointRuleId); // ポイントルールID
				SetDefaultValue();
			}

			// 新規？
			else if (strActionStatus == Constants.ACTION_STATUS_INSERT)
			{
				// 処理無し
				Input = new PointRuleInput();
			}
			// コピー新規・編集？
			else if (strActionStatus == Constants.ACTION_STATUS_COPY_INSERT
				|| strActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				// セッションよりポイントキャンペーンルール情報取得
				Input = MpSessionWrapper.PointRuleInput;
				ViewState.Add(Constants.FIELD_POINTRULE_POINT_RULE_ID, Input.PointRuleId); // ポイントルールID
				SetDefaultValue();
			}
			// それ以外の場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}

			// データバインド
			DataBind();

			// カレンダー描画
			CreateCampaignCalendar();

			// 入力域制御
			ddlPointIncKbn_SelectedIndexChanged(sender, e);
			rbCampaignTermKbn_CheckedChanged(sender, e);
			PointKbn_OnCheckedChanged(sender, e);
			ExpireKbn_CheckedChanged(sender, e);
		}

		// ポイント有効期限延長
		trPointExpEntend.Visible = IsPointExpKbnValid(
			(string.IsNullOrEmpty(this.Input.PointKbn) == false)
				? this.Input.PointKbn
				: Constants.FLG_USERPOINT_POINT_KBN_BASE);
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
			// 更新
			case Constants.ACTION_STATUS_UPDATE:
			trEdit.Visible = true;
				break;
		}

		// ポイント期限延長ドロップダウン作成
		ddlPointExpEntend.Items.Add(new ListItem("", ""));
		ddlPointExpEntend.Items.AddRange(Enumerable.Range(0, 37).Select(i => new ListItem(i.ToString("00"))).ToArray());

		// キャンペーン期間タイプ(日)ドロップダウン作成
		ddlCampaignTermValue1.Items.Add(new ListItem("", ""));
		ddlCampaignTermValue1.Items.AddRange(Enumerable.Range(1, 31).Select(i => new ListItem(i.ToString("00"))).ToArray());

		// キャンペーン期間タイプ(曜日)ドロップダウン作成
		ddlCampaignTermValue2.Items.Add(new ListItem("", ""));
		ddlCampaignTermValue2.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_CAMPAIGN_TERM_VALUE));

		ddlCampaignTermValue3.Items.Add(new ListItem("", ""));
		ddlCampaignTermValue3.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_CAMPAIGN_TERM_VALUE));

		// ポイント加算区分ドロップダウン作成
		ddlPointIncKbn.Items.Add(new ListItem("", ""));
		foreach (ListItem item in ValueText.GetValueItemList(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_POINT_INC_KBN))
		{
			if ((item.Value == Constants.FLG_POINTRULE_POINT_INC_KBN_VERSATILE_POINT_RULE)
				|| (item.Value == Constants.FLG_POINTRULE_POINT_INC_KBN_BIRTHDAY_POINT)
				|| (item.Value == Constants.FLG_POINTRULE_POITN_INC_KBN_CLICK)
				|| (((Constants.PRODUCTREVIEW_ENABLED == false) || (Constants.REVIEW_REWARD_POINT_ENABLED == false))
					&& (item.Value == Constants.FLG_POINTRULE_POINT_INC_KBN_REWARD_POINT))
				|| ((item.Value == Constants.FLG_POINTRULE_POINT_INC_KBN_LOGIN)
					&& (Constants.CROSS_POINT_LOGIN_POINT_ENABLED == false)
					&& Constants.CROSS_POINT_OPTION_ENABLED)) continue;
			ddlPointIncKbn.Items.Add(item);
		}

		// ポイント発行オフセット区分DDL作成
		ddlEffectiveOffsetType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_EFFECTIVE_OFFSET_TYPE));

		// 期間限定ポイント期間区分DDL作成
		ddlTermType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_TERM_TYPE));

		// ポイント加算方法ドロップダウン作成
		ddlPointIncType.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_INC_TYPE))
		{
			ddlPointIncType.Items.Add(li);
		}

		ddlFixedPurchasePointIncType.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_POINTRULE, Constants.FIELD_POINTRULE_FIXED_PURCHASE_INC_TYPE))
		{
			ddlFixedPurchasePointIncType.Items.Add(li);
		}
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
		// 変数宣言
		bool blResult = true;

		var sv = new PointService();
		var res = sv.GetPointMaster();
		var point = res.FirstOrDefault(i => (i.PointKbn == pointKbn));
		if (point != null)
		{
			blResult = point.PointExpKbn == Constants.FLG_POINT_POINT_EXP_KBN_VALID;
		}
		else
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHOPPOINT_NO_DATA);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		return blResult;
	}
	#endregion

	#region -CheckCampaignExp キャンペーン有効期間チェック
	/// <summary>
	/// キャンペーン有効期間チェック
	/// </summary>
	private void CheckCampaignExp()
	{
		// 変数宣言
		string strErrorMessages = String.Empty;
		DateTime expBgn;
		DateTime expEnd;

		// キャンペーン有効期間取得
		var expBgnString = (string.IsNullOrEmpty(ucExpire.StartDateTimeString) == false)
			? ucExpire.StartDateTimeString
			: DateTime.Now.ToString();
		var expEndString = (string.IsNullOrEmpty(ucExpire.EndDateTimeString) == false)
			? ucExpire.EndDateTimeString
			: DateTime.Now.ToString();

		// キャンペーン有効期間（開始）
		try
		{
			// 正しい日付取得
			expBgn = DateTime.Parse(expBgnString);
		}
		// キャストエラーの場合
		catch
		{
			expBgn = DateTime.Parse(string.Format("{0}/{1}/01",
				DateTime.Parse(ucExpire.HfStartDate.Value).Year,
				DateTime.Parse(ucExpire.HfStartDate.Value).Month));
			expBgn = expBgn.AddMonths(1).AddDays(-1);

			strErrorMessages += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_CAMPAIGN_EXP_ERROR);
			strErrorMessages = strErrorMessages.Replace("@@ 1 @@", WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CAMPAIGN_VALIDITY_PERIOD_START));
			strErrorMessages = strErrorMessages.Replace(
				"@@ 2 @@",
				DateTimeUtility.ToStringFromRegion(expBgn, DateTimeUtility.FormatType.LongYearMonth));
			strErrorMessages = strErrorMessages.Replace("@@ 3 @@", string.Format(
				// 「{0}日」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_POINT_RULE_CAMPAIGN_REGISTER,
					Constants.VALUETEXT_PARAM_POINT_RULE_CAMPAIGN_REGISTER_TIME_KBN,
					Constants.VALUETEXT_PARAM_POINT_RULE_CAMPAIGN_REGISTER_TIME_KBN_DAY),
				expBgn.Day));
		}

		// キャンペーン有効期間（終了）
		try
		{
			expEnd = DateTime.Parse(expEndString);
		}
		// キャストエラーの場合
		catch
		{
			// 正しい日付取得
			expEnd = DateTime.Parse(string.Format("{0}/{1}/01",
				DateTime.Parse(ucExpire.HfEndDate.Value).Year,
				DateTime.Parse(ucExpire.HfStartDate.Value).Month));
			expEnd = expEnd.AddMonths(1).AddDays(-1);

			if (strErrorMessages != "")
			{
				strErrorMessages += "<br/>";
			}
			strErrorMessages += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_CAMPAIGN_EXP_ERROR);
			strErrorMessages = strErrorMessages.Replace("@@ 1 @@", WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CAMPAIGN_VALIDITY_PERIOD_END));
			strErrorMessages = strErrorMessages.Replace(
				"@@ 2 @@",
				DateTimeUtility.ToStringFromRegion(expEnd, DateTimeUtility.FormatType.LongYearMonth));
			strErrorMessages = strErrorMessages.Replace("@@ 3 @@", string.Format(
				// 「{0}日」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_POINT_RULE_CAMPAIGN_REGISTER,
					Constants.VALUETEXT_PARAM_POINT_RULE_CAMPAIGN_REGISTER_TIME_KBN,
					Constants.VALUETEXT_PARAM_POINT_RULE_CAMPAIGN_REGISTER_TIME_KBN_DAY),
				expEnd.Day));
		}
		if (strErrorMessages == "")
		{
			// キャンペーン有効期間（開始）> キャンペーン有効期間（終了）の場合
			if (expBgn > expEnd)
			{
				strErrorMessages = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CAMPAIGN_EXP_BGN_END_ERROR);
			}
		}

		// エラーの場合
		if (strErrorMessages != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}
	}
	#endregion

	#region -CreateCampaignCalendar カレンダー再描画(キャンペーン期間)
	/// <summary>
	/// カレンダー再描画(キャンペーン期間)
	/// </summary>
	/// <remarks>
	/// キャンペーン有効期間からカレンダー用リピータのデータソースを作成
	/// </remarks>
	private void CreateCampaignCalendar()
	{
		// 変数宣言
		int iCalendarNo = 1;
		var expBgnString = (ucExpire.StartDateTimeString != string.Empty)
			? ucExpire.StartDateTimeString
			: DateTime.Now.ToString();
		var expEndString = (ucExpire.EndDateTimeString != string.Empty)
			? ucExpire.EndDateTimeString
			: DateTime.Now.ToString();

		// キャンペーン期間チェック
		CheckCampaignExp();

		// キャンペーン有効期間取得
		var expBgn = DateTime.Parse(string.Format("{0}/{1}/01",
			DateTime.Parse(expBgnString).Year,
			DateTime.Parse(expBgnString).Month));
		var expEnd = DateTime.Parse(string.Format("{0}/{1}/01",
			DateTime.Parse(expEndString).Year,
			DateTime.Parse(expEndString).Month));

		// キャンペーン有効期間(開始) < キャンペーン有効期間(終了)ループ
		while (expBgn <= expEnd)
		{
			Hashtable htInput = new Hashtable();

			htInput.Add(CAMPAIGN_CALENDAR_DATE,
				DateTime.Parse(expBgn.Year.ToString() + "/" + expBgn.Month.ToString() + "/01"));	// カレンダー日付
			htInput.Add(CAMPAIGN_CALENDAR_DATE_NO, iCalendarNo);										// カレンダーNo

			// カレンダー情報を格納
			m_alCampaign.Add(htInput);

			expBgn = expBgn.AddMonths(1);	// 1月単位で追加
			iCalendarNo++;
		}

		// データバインド
		rCampaign.DataBind();
	}
	#endregion

	#region -SetCampaignCalendar カレンダー再描画(キャンペーン期間タイプ)
	/// <summary>
	/// カレンダー再描画(キャンペーン期間タイプ)
	/// </summary>
	/// <remarks>
	/// キャンペーン有効期間から指定された日を選択
	/// </remarks>
	private void SetCampaignCalendar()
	{
		// キャンペーン期間チェック
		CheckCampaignExp();

		// カレンダー選択情報取得
		ArrayList alCalendarInfo = (ViewState[Constants.SESSIONPARAM_KEY_POINTRULE_CAMPAIGN_CALENDAR_INFO] != null) ?
			(ArrayList)ViewState[Constants.SESSIONPARAM_KEY_POINTRULE_CAMPAIGN_CALENDAR_INFO] : new ArrayList();

		string errorMessages = string.Empty;

		// キャンペーン有効期間取得
		DateTime expBgn;
		if (DateTime.TryParse(ucExpire.StartDateTimeString, out expBgn) == false)
		{
			errorMessages = WebMessages.GetMessages(WebMessages.INPUTCHECK_DATERANGE)
				.Replace("@@ 1 @@", WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CAMPAIGN_VALIDITY_PERIOD_START));
		}

		DateTime expEnd;
		if (DateTime.TryParse(ucExpire.EndDateTimeString, out expEnd) == false)
		{
			errorMessages += WebMessages.GetMessages(WebMessages.INPUTCHECK_DATERANGE)
				.Replace("@@ 1 @@", WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CAMPAIGN_VALIDITY_PERIOD_END));
		}

		if (string.IsNullOrEmpty(errorMessages) == false)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		// キャンペーン有効期間(開始) < キャンペーン有効期間(終了)ループ
		while (expBgn <= expEnd)
		{
			// 全ての場合
			if (rbCampaignTermKbn1.Checked)
			{
				// 日付が含まれていない場合(yyyy/mm/dd)
				if (alCalendarInfo.Contains(expBgn.Date) == false)
				{
					alCalendarInfo.Add(expBgn);
				}
				expBgn = expBgn.AddDays(1);		// 1日単位で追加
			}
			// 毎月の場合
			else if (rbCampaignTermKbn2.Checked)
			{
				// 日が一致している場合
				if (expBgn.Day.ToString("00") == ddlCampaignTermValue1.SelectedValue)
				{
					// 日付が含まれていない場合(yyyy/mm/dd)
					if (alCalendarInfo.Contains(expBgn.Date) == false)
					{
						alCalendarInfo.Add(expBgn);
					}
					expBgn = expBgn.AddMonths(1);	// 1月単位で追加
				}
				else
				{
					expBgn = expBgn.AddDays(1);		// 1日単位で追加
				}
			}
			// 毎週の場合
			else if (rbCampaignTermKbn3.Checked)
			{
				// 曜日が一致している場合
				if (((int)expBgn.DayOfWeek).ToString() == ddlCampaignTermValue2.SelectedValue)
				{
					// 日付が含まれていない場合(yyyy/mm/dd)
					if (alCalendarInfo.Contains(expBgn.Date) == false)
					{
						alCalendarInfo.Add(expBgn);
					}
					expBgn = expBgn.AddDays(7);		// 1週単位で追加
				}
				else
				{
					expBgn = expBgn.AddDays(1);		// 1日単位で追加
				}
			}
			// 隔週の場合
			else if (rbCampaignTermKbn4.Checked)
			{
				// 曜日が一致している場合
				if (((int)expBgn.DayOfWeek).ToString() == ddlCampaignTermValue3.SelectedValue)
				{
					// 日付が含まれていない場合(yyyy/mm/dd)
					if (alCalendarInfo.Contains(expBgn.Date) == false)
					{
						alCalendarInfo.Add(expBgn);
					}
					expBgn = expBgn.AddDays(14);	// 2週単位で追加
				}
				else
				{
					expBgn = expBgn.AddDays(1);		// 1日単位で追加
				}
			}
		}

		// カレンダー選択情報をビューステートに保存
		// cldCampaign1_DayRenderイベントで選択描画処理を行う
		ViewState[Constants.SESSIONPARAM_KEY_POINTRULE_CAMPAIGN_CALENDAR_INFO] = alCalendarInfo;

		Input.RuleDate = alCalendarInfo
			.Cast<DateTime>()
			.Where(item => (DateTime.Parse(ucExpire.StartDateTimeString) <= item)
				&& (item <= DateTime.Parse(ucExpire.EndDateTimeString)))
			.Select(item => new PointRuleDateInput
			{
				DeptId = this.LoginOperatorDeptId,
				PointRuleId = Input.PointRuleId,
				TgtDate = item.ToString("yyyy/MM/dd HH:mm:ss")
			})
			.ToArray();

	}
	#endregion

	#region #btnReflect_Click カレンダーに反映ボタンクリック(キャンペーン期間反映)
	/// <summary>
	/// カレンダーに反映ボタンクリック(キャンペーン期間反映)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReflect_Click(object sender, System.EventArgs e)
	{
		// カレンダー再描画
		CreateCampaignCalendar();
	}
	#endregion

	#region #btnReflect2_Click カレンダーに反映ボタンクリック(キャンペーン期間タイプ反映)
	/// <summary>
	/// カレンダーに反映ボタンクリック(キャンペーン期間タイプ反映)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReflect2_Click(object sender, System.EventArgs e)
	{
		// カレンダー再描画
		SetCampaignCalendar();
	}
	#endregion

	#region カレンダー日付が選択されたときに呼び出されるイベント
	/// <summary>
	/// カレンダー日付が選択されたときに呼び出されるイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cldCampaign1_SelectionChanged(object sender, System.EventArgs e)
	{
		// カレンダー選択情報取得
		var alCalendarInfo = Input.RuleDate.Select(i => DateTime.Parse(i.TgtDate)).ToArray();

		// カレンダー取得
		Calendar cldCampaign = (Calendar)sender;

		// 選択されていない場合
		if (alCalendarInfo.Contains(cldCampaign.SelectedDate) == false)
		{
			var addDate = new[]
				{
					new PointRuleDateInput
						{
							DeptId = this.LoginOperatorDeptId,
							PointRuleId = Input.PointRuleId,
							TgtDate = cldCampaign.SelectedDate.ToString()
						}
				};

			// 選択
			Input.RuleDate = Input.RuleDate.Concat(addDate).ToArray();

		}
		// 既に選択されている場合
		else
		{
			// 選択解除
			Input.RuleDate = Input.RuleDate.Where(i => i.TgtDate != cldCampaign.SelectedDate.ToString()).ToArray();
		}

		// そしてカレンダオブジェクトの選択を解除
		cldCampaign.SelectedDates.Clear();
	}
	#endregion

	#region +cldCampaign1_DayRender カレンダー日付描画毎に呼び出されるイベント
	/// <summary>
	/// カレンダー日付描画毎に呼び出されるイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void cldCampaign1_DayRender(object sender, System.Web.UI.WebControls.DayRenderEventArgs e)
	{
		// 変数宣言
		string strDay = String.Empty;

		// カレンダー選択情報取得
		var alCalendarInfo = Input.RuleDate.Select(i => DateTime.Parse(i.TgtDate)).ToArray();

		// カレンダー取得
		Calendar cldCampaign = (Calendar)sender;

		// 同じ月の場合
		e.Cell.BackColor = Color.FromArgb(0xee, 0xee, 0xee);
		if (cldCampaign.VisibleDate.Month == e.Day.Date.Month)
		{
			// カレンダー選択情報に対象セルの日付が含まれている場合
			if (alCalendarInfo.Any(date => date.Date == e.Day.Date))
			{
				e.Cell.BackColor = Color.FromArgb(0xbe, 0xd2, 0xff);
				e.Cell.Font.Bold = true;
				e.Cell.ForeColor = Color.FromArgb(0x08, 0x64, 0xAA);
			}
		}
		// 別の月の場合
		else
		{
			// リンク削除
			strDay = ((LiteralControl)e.Cell.Controls[0]).Text;
			LiteralControl hcDay = new LiteralControl(strDay);
			e.Cell.Controls.Clear();
			e.Cell.Controls.Add(hcDay);
		}
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
		string pointRuleId = (string)ViewState[Constants.FIELD_POINTRULE_POINT_RULE_ID];
		string incType = String.Empty;
		string incRate = String.Empty;
		string inc = String.Empty;
		string userTempFlg = Constants.FLG_POINTRULE_USE_TEMP_FLG_INVALID;	// 仮ポイントを利用しない
		string campaignTermKbn = String.Empty;
		string campaignTermValue = String.Empty;
		string incFixedPurchaseType = String.Empty;
		string incFixedPurchaseRate = String.Empty;
		string incFixedPurchase = String.Empty;

		// キャンペーン有効期間だけ受け継ぐ
		var input = new PointRuleInput
		{
			RuleDate = this.Input.RuleDate.ToArray()
		};

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
				input.PeriodBegin = string.Format("{0} {1}",
					ucPeriod.HfStartDate.Value,
					ucPeriod.HfStartTime.Value);
				input.PeriodEnd = string.Format("{0} {1}",
					ucPeriod.HfEndDate.Value,
					ucPeriod.HfEndTime.Value);
			}
			// 有効期限を指定する場合
			else if (rbLimitedTermPointExpirationDay.Checked)
			{
				// 入力チェック用 DBには反映しない
				input.LimitedTermPointExpireKbn = PointRuleInput.LIMTIED_TERM_POINT_EXPIRE_KBN_RELATIVE;
				input.EffectiveOffset = tbEffectiveOffset.Text.Trim();
				input.EffectiveOffsetType = ddlEffectiveOffsetType.SelectedValue;
				input.Term = tbTerm.Text.Trim();
				input.TermType = ddlTermType.SelectedValue;
			}
		}

		input.DeptId = this.LoginOperatorDeptId;
		input.PointRuleId = pointRuleId;
		input.PointRuleName = tbPointRuleName.Text;
		input.PointIncKbn = ddlPointIncKbn.SelectedValue;
		input.ExpBgn = ucExpire.StartDateTimeString;
		input.ExpEnd = ucExpire.EndDateTimeString;
		// ポイント有効期限延長
		input.PointExpExtend = Constants.CROSS_POINT_OPTION_ENABLED
			? "+000000"
			: ((string.IsNullOrEmpty(ddlPointExpEntend.SelectedValue) == false)
				? string.Format("+00{0}00", ddlPointExpEntend.SelectedValue)
				: string.Empty);

		// ポイントルール区分はこの画面の場合は常に「02：キャンペーン」
		input.PointRuleKbn = Constants.FLG_POINTRULE_POINT_RULE_KBN_CAMPAIGN;

		// 全体
		if (rbCampaignTermKbn1.Checked)
		{
			// 処理しない
		}
		// 毎月
		else if (rbCampaignTermKbn2.Checked)
		{
			campaignTermKbn = Constants.FLG_POINTRULE_CAMPAIGN_TERM_KBN_MONTH;
			campaignTermValue = ddlCampaignTermValue1.SelectedValue;
		}
		// 毎週
		else if (rbCampaignTermKbn3.Checked)
		{
			campaignTermKbn = Constants.FLG_POINTRULE_CAMPAIGN_TERM_WEEK;
			campaignTermValue = ddlCampaignTermValue2.SelectedValue;
		}
		// 隔週
		else if (rbCampaignTermKbn4.Checked)
		{
			campaignTermKbn = Constants.FLG_POINTRULE_CAMPAIGN_TERM_KBN_EVERY_OTHER_WEEK;
			campaignTermValue = ddlCampaignTermValue3.SelectedValue;
		}

		input.CampaignTermKbn = campaignTermKbn;
		input.CampaignTermValue = campaignTermValue;
		input.Priority = tbPriority.Text;

		// 基本ルールとの二重適用
		input.AllowDuplicateApplyFlg = cbAllowDuplicateApply.Checked
			? Constants.FLG_POINTRULE_DUPLICATE_APPLY_ALLOW
			: Constants.FLG_POINTRULE_DUPLICATE_APPLY_DISALLOW;

		input.PriorityChk = Validator.IsHalfwidthNumber(tbPriority.Text) ? tbPriority.Text : "0";
		input.ValidFlg = cbValidFlg.Checked ? Constants.FLG_POINTRULE_VALID_FLG_VALID : Constants.FLG_POINTRULE_VALID_FLG_INVALID;
		input.LastChanged = this.LoginOperatorName;

		// 購入時ポイント発行、初回購入ポイント発行の場合
		if ((input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BUY)
			|| (input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY))
		{
			// ポイントを使用
			if ((ddlPointIncType.SelectedValue == Constants.FLG_POINTRULE_INC_TYPE_NUM)
				|| (ddlPointIncType.SelectedValue == Constants.FLG_POINTRULE_INC_TYPE_RATE))
			{
				incType = ddlPointIncType.SelectedValue;
				inc = tbIncPoint.Text;
				incRate = (ddlPointIncType.SelectedValue == Constants.FLG_POINTRULE_INC_TYPE_NUM) ? "0" : tbIncPoint.Text;
			}
			// 定期ポイントを使用
			if ((ddlFixedPurchasePointIncType.SelectedValue == Constants.FLG_POINTRULE_INC_TYPE_NUM)
				|| (ddlFixedPurchasePointIncType.SelectedValue == Constants.FLG_POINTRULE_INC_TYPE_RATE))
			{
				incFixedPurchaseType = ddlFixedPurchasePointIncType.SelectedValue;
				incFixedPurchase = ((tbIncFixedPurchasePoint.Text == string.Empty) ? "0" : tbIncFixedPurchasePoint.Text);
				incFixedPurchaseRate = (ddlFixedPurchasePointIncType.SelectedValue == Constants.FLG_POINTRULE_INC_TYPE_NUM)
					? "0"
					: ((tbIncFixedPurchasePoint.Text == string.Empty) ? "0" : tbIncFixedPurchasePoint.Text);
			}

			userTempFlg = input.NeedsToUseTemporaryPointInSetting
				? Constants.FLG_POINTRULE_USE_TEMP_FLG_VALID
				: Constants.FLG_POINTRULE_USE_TEMP_FLG_INVALID;
		}
		// 新規登録ポイント発行、ログイン毎ポイント発行の場合
		else
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
			|| (input.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_REWARD_POINT))
		{
			input.UseTempFlg = userTempFlg;
			input.IncType = incType;
			input.Inc = inc;
			input.IncFixedPurchaseType = incFixedPurchaseType;
			input.IncFixedPurchase = incFixedPurchase;
		}

		// 入力チェックを回避するため、 加算数、加算率のどちらかを
		// 格納する。ただし、ステートメント実行時に両方のパラメータを渡しているため設定すること
		// ポイント加算数の場合
		if (incType == Constants.FLG_POINTRULE_INC_TYPE_NUM)
		{
			input.IncNum = inc;
		}
		// ポイント加算率の場合
		else
		{
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
		var input = this.CreateInput();

		string validator = "";

		// 新規・コピー新規
		if (((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT)
			|| ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_COPY_INSERT))
		{
			validator = "PointRuleCampaignRegist";
		}
		// 変更
		else if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
		{
			validator = "PointRuleCampaignModify";
		}

		// パラメタをセッションへ格納
		MpSessionWrapper.PointRuleInput = input;

		// 入力チェック＆重複チェック
		string errorMessages = input.Validate(validator);

		if (input.RuleDate.Length == 0)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CAMPAIGN_EXP_NO_SELECTED_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		if ((input.UseScheduleIncKbnPointRule == false) && (Validator.CheckDateRange(input.ExpBgn, input.ExpEnd) == false))
		{
			errorMessages += WebMessages.GetMessages(WebMessages.INPUTCHECK_DATERANGE)
				.Replace("@@ 1 @@", WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CAMPAIGN_VALIDITY_PERIOD));
		}

		// キャンペーン有効日チェック
		DateTime startDate;
		DateTime endDate;
		if ((DateTime.TryParse(ucExpire.StartDateTimeString, out startDate)) && (DateTime.TryParse(ucExpire.EndDateTimeString, out endDate)))
		{
			if ((input.UseScheduleIncKbnPointRule == false)
				&& input.RuleDate
					.Any(item => ((DateTime.Parse(item.TgtDate) < startDate)
						|| (endDate < DateTime.Parse(item.TgtDate)))))
			{
				errorMessages += WebMessages.GetMessages(WebMessages.INPUTCHECK_DATERANGE)
					.Replace("@@ 1 @@", WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CAMPAIGN_VALIDITY_DATE));
			}

			if (startDate == endDate)
			{
				errorMessages += WebMessages.GetMessages(WebMessages.INPUTCHECK_DATERANGE)
					.Replace("@@ 1 @@", WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CAMPAIGN_VALIDITY_DATE));
			}
		}

		if (errorMessages != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		// ポイント基本ルール確認ページへ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_CAMPAIGN_CONFIRM +
			"?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS]);
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
		// ポイント使用制御（ポイント加算区分選択時）
		switch (ddlPointIncKbn.SelectedValue)
		{
			case Constants.FLG_POINTRULE_POINT_INC_KBN_BUY:
			case Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY:
				ddlPointIncType.Enabled = true;
				dvFixedPurchaseSetting.Visible = (ddlPointIncKbn.SelectedValue == Constants.FLG_POINTRULE_POINT_INC_KBN_BUY);

				rbLimitedTermPointPeriod.Checked = false;
				rbLimitedTermPointExpirationDay.Checked = true;
				rbLimitedTermPointPeriod.Enabled = false;
				break;

			case Constants.FLG_POINTRULE_POINT_INC_KBN_USER_REGISTER:
			case Constants.FLG_POINTRULE_POINT_INC_KBN_LOGIN:
			case Constants.FLG_POINTRULE_POINT_INC_KBN_REWARD_POINT:
				ddlPointIncType.SelectedValue = Constants.FLG_POINTRULE_INC_TYPE_NUM;
				ddlPointIncType.Enabled = false;
				dvFixedPurchaseSetting.Visible = false;
				rbLimitedTermPointPeriod.Enabled = true;
				break;
		}

		divReviewPoint.Visible = (ddlPointIncKbn.SelectedValue == Constants.FLG_POINTRULE_POINT_INC_KBN_REWARD_POINT);

		ExpireKbn_CheckedChanged(sender, e);
	}
	#endregion

	#region #rbCampaignTermKbn_CheckedChanged キャンペーン有効期間詳細設定選択
	/// <summary>
	/// キャンペーン有効期間詳細設定選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbCampaignTermKbn_CheckedChanged(object sender, EventArgs e)
	{
		// キャンペーンタイプ使用制御
		// 全体の場合
		if (rbCampaignTermKbn1.Checked)
		{
			ddlCampaignTermValue1.SelectedIndex = 0;
			ddlCampaignTermValue2.SelectedIndex = 0;
			ddlCampaignTermValue3.SelectedIndex = 0;
			ddlCampaignTermValue1.Enabled = false;
			ddlCampaignTermValue2.Enabled = false;
			ddlCampaignTermValue3.Enabled = false;
		}
		// 毎月の場合
		else if (rbCampaignTermKbn2.Checked)
		{
			ddlCampaignTermValue2.SelectedIndex = 0;
			ddlCampaignTermValue3.SelectedIndex = 0;
			ddlCampaignTermValue1.Enabled = true;
			ddlCampaignTermValue2.Enabled = false;
			ddlCampaignTermValue3.Enabled = false;
		}
		// 毎週の場合
		else if (rbCampaignTermKbn3.Checked)
		{
			ddlCampaignTermValue1.SelectedIndex = 0;
			ddlCampaignTermValue3.SelectedIndex = 0;
			ddlCampaignTermValue1.Enabled = false;
			ddlCampaignTermValue2.Enabled = true;
			ddlCampaignTermValue3.Enabled = false;
		}
		// 隔週の場合
		else if (rbCampaignTermKbn4.Checked)
		{
			ddlCampaignTermValue1.SelectedIndex = 0;
			ddlCampaignTermValue2.SelectedIndex = 0;
			ddlCampaignTermValue1.Enabled = false;
			ddlCampaignTermValue2.Enabled = false;
			ddlCampaignTermValue3.Enabled = true;
		}
	}
	#endregion

	#region #PointKbn_OnCheckedChanged ポイント区分選択
	/// <summary>
	/// ポイント区分選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void PointKbn_OnCheckedChanged(object sender, EventArgs e)
	{
		var selectedOld = ddlPointExpEntend.SelectedValue;
		trLimitedTermPointExpiration.Visible = rbPointKbnLimitedTermPoint.Checked;

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

		// ポイント有効期限延長
		trPointExpEntend.Visible = IsPointExpKbnValid(
			(string.IsNullOrEmpty(this.Input.PointKbn) == false)
				? this.Input.PointKbn
				: Constants.FLG_USERPOINT_POINT_KBN_BASE);
	}
	#endregion

	#region #ExpireKbn_CheckedChanged 期限/期間選択
	/// <summary>
	/// 期限/期間選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ExpireKbn_CheckedChanged(object sender, EventArgs e)
	{
		pLimitedTermPointExpirationDay.Visible = rbLimitedTermPointExpirationDay.Checked;
		pLimitedTermPointPeriod.Visible = rbLimitedTermPointPeriod.Checked;
	}
	#endregion

	#region #lbBack_OnClick 戻るボタンクリック
	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_OnClick(object sender, EventArgs e)
	{
		var parameters = (Hashtable)Session[Constants.SESSIONPARAM_KEY_POINTRULE_CAMPAIGN_SEARCH_INFO];
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_POINTRULE_CAMPAIGN_LIST)
				.AddParam(Constants.REQUEST_KEY_SEARCH_WORD, (string)parameters[Constants.REQUEST_KEY_SEARCH_WORD])
				.AddParam(Constants.REQUEST_KEY_SORT_KBN, (string)parameters[Constants.REQUEST_KEY_SORT_KBN])
				.AddParam(Constants.REQUEST_KEY_PAGE_NO, parameters[Constants.REQUEST_KEY_PAGE_NO].ToString())
				.CreateUrl();

		Response.Redirect(url);
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
		if (string.IsNullOrEmpty(this.Input.ExpBgn) == false)
		{
			ucExpire.SetStartDate(DateTime.Parse(this.Input.ExpBgn));
		}
		if (string.IsNullOrEmpty(this.Input.ExpEnd) == false)
		{
			ucExpire.SetEndDate(DateTime.Parse(this.Input.ExpEnd));
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
			var lastDate = new DateTime(
				DateTime.Now.Year,
				DateTime.Now.Month,
				DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
			if (this.Input == null) return lastDate;
			DateTime date;
			return DateTime.TryParse(this.Input.ExpEnd, out date) ? date : lastDate;
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

