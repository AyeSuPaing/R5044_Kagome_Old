/*
=========================================================================================================
  Module      : メール配信設定登録ページ処理(MailDistSettingRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;

public partial class Form_MailDistSetting_MailDistSettingRegister : BasePage
{
	public string m_strActionStatus = null;
	protected Hashtable m_htMailDistSet = new Hashtable();

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// On Load イベント追加
			//------------------------------------------------------
			this.Master.OnLoadEvents += "RefleshTargetListSchedule();";

			//------------------------------------------------------
			// アクションステータス取得
			//------------------------------------------------------
			m_strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, m_strActionStatus);

			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponent();

			//------------------------------------------------------
			// メール配信設定取得＆画面セット
			//------------------------------------------------------
			if ((m_strActionStatus == Constants.ACTION_STATUS_UPDATE)
				|| (m_strActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
			{
				// メール配信設定取得
				DataView dvMailDistSetting = null;
				DataView dvMailDistExceptList = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("MailDistSetting", "GetMailDistSetting"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_MAILDISTSETTING_DEPT_ID, this.LoginOperatorDeptId);
					htInput.Add(Constants.FIELD_MAILDISTSETTING_MAILDIST_ID, Request[Constants.REQUEST_KEY_MAILDIST_ID]);

					DataSet dsMailDistSetting = sqlStatement.SelectStatementWithOC(sqlAccessor, htInput);
					dvMailDistSetting = dsMailDistSetting.Tables[0].DefaultView;
					dvMailDistExceptList = dsMailDistSetting.Tables[1].DefaultView;
				}
				foreach (DataColumn dc in dvMailDistSetting.Table.Columns)
				{
					m_htMailDistSet.Add(dc.ColumnName, dvMailDistSetting[0][dc.ColumnName]);
				}

				StringBuilder sbExceptList = new StringBuilder();
				foreach (DataRowView drv in dvMailDistExceptList)
				{
					sbExceptList.Append((sbExceptList.Length != 0) ? "," : "");
					sbExceptList.Append(drv[Constants.FIELD_MAILDISTEXCEPTLIST_MAIL_ADDR]);
				}

				// 画面セット
				lbMailDistId.Text = WebSanitizer.HtmlEncode(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID]);
				hdnMailDistId.Value = (string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID];
				tbMailDistName.Text = (string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME];
				foreach (ListItem li in ddlTargetId.Items)
				{
					li.Selected = (li.Value == (string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_TARGET_ID]);
				}
				foreach (ListItem li in ddlTargetId2.Items)
				{
					li.Selected = (li.Value == (string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_TARGET_ID2]);
				}
				foreach (ListItem li in ddlTargetId3.Items)
				{
					li.Selected = (li.Value == (string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_TARGET_ID3]);
				}
				foreach (ListItem li in ddlTargetId4.Items)
				{
					li.Selected = (li.Value == (string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_TARGET_ID4]);
				}
				foreach (ListItem li in ddlTargetId5.Items)
				{
					li.Selected = (li.Value == (string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_TARGET_ID5]);
				}
				cbTargetExtract.Checked = ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG] == Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_ON);
				cbTargetExtract2.Checked = ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG2] == Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_ON);
				cbTargetExtract3.Checked = ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG3] == Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_ON);
				cbTargetExtract4.Checked = ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG4] == Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_ON);
				cbTargetExtract5.Checked = ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG5] == Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_ON);
				foreach (ListItem li in ddlExceptErrorPoint.Items)
				{
					li.Selected = (li.Value == ((int)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_EXCEPT_ERROR_POINT]).ToString());
				}
				cbExceptMobileMail.Checked = ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG] == Constants.FLG_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG_ON);
				tbExceptList.Text = sbExceptList.ToString();
				foreach (ListItem li in ddlMailTextId.Items)
				{
					li.Selected = (li.Value == (string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_MAILTEXT_ID]);
				}
				cbValidFlg.Checked = ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_VALID_FLG] == Constants.FLG_MAILDISTSETTING_VALID_FLG_VALID);
				cbEnableDeduplication.Checked = ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_ENABLE_DEDUPLICATION] == Constants.FLG_MAILDISTSETTING_ENABLE_DEDUPLICATION_ENABLED);

				// スケジュール実行？
				rbExecByManual.Checked = ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_EXEC_TIMING] == Constants.FLG_MAILDISTSETTING_EXEC_TIMING_MANUAL);
				rbExecBySchedule.Checked = ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_EXEC_TIMING] == Constants.FLG_MAILDISTSETTING_EXEC_TIMING_SCHEDULE);
				if ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_EXEC_TIMING] == Constants.FLG_MAILDISTSETTING_EXEC_TIMING_SCHEDULE)
				{
					rbScheRepeatDay.Checked = ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN] == Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_DAY);
					rbScheRepeatWeek.Checked = ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN] == Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_WEEK);
					rbScheRepeatMonth.Checked = ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN] == Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_MONTH);
					rbScheRepeatOnce.Checked = ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN] == Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_ONCE);

					if (rbScheRepeatWeek.Checked)
					{
						foreach (ListItem li in rblScheDayOfWeek.Items)
						{
							li.Selected = (li.Value == ((string)m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY_OF_WEEK]));
						}
					}
					else
					{
						rblScheDayOfWeek.SelectedIndex = 0;
					}

					if (rbScheRepeatOnce.Checked && (this.IsBackFromConfirm == false))
					{
						var scheDateTimeOnce = string.Format("{0}/{1}/{2} {3}:{4}:{5}",
							StringUtility.ToEmpty(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_YEAR]),
							StringUtility.ToEmpty(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MONTH]),
							StringUtility.ToEmpty(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY]),
							StringUtility.ToEmpty(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_HOUR]),
							StringUtility.ToEmpty(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MINUTE]),
							StringUtility.ToEmpty(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_SECOND]));

						if (String.IsNullOrEmpty(scheDateTimeOnce) == false)
						{
							ucScheDateTimeOnce.SetStartDate(DateTime.Parse(scheDateTimeOnce));
						}
					}
					else
					{
						// 実行タイミングの日付セット
						ucScheDateTime.Year = m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_YEAR].ToString();
						ucScheDateTime.Month = PadLeftZero2Letter(StringUtility.ToEmpty(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MONTH]));
						ucScheDateTime.Day = PadLeftZero2Letter(StringUtility.ToEmpty(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY]));
						ucScheDateTime.Hour = PadLeftZero2Letter(StringUtility.ToEmpty(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_HOUR]));
						ucScheDateTime.Minute = PadLeftZero2Letter(StringUtility.ToEmpty(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MINUTE]));
						ucScheDateTime.Second = PadLeftZero2Letter(StringUtility.ToEmpty(m_htMailDistSet[Constants.FIELD_MAILDISTSETTING_SCHEDULE_SECOND]));
					}
				}
				// 手動実行？（スケジュールはデフォルト値を設定しておく）
				else
				{
					rbScheRepeatOnce.Checked = true;
					rbScheRepeatDay.Checked = rbScheRepeatWeek.Checked = rbScheRepeatMonth.Checked = false;
					rblScheDayOfWeek.SelectedIndex = 0;
				}
			}
		}
		else
		{
			m_strActionStatus = (string)Request[Constants.REQUEST_KEY_ACTION_STATUS];
		}

		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponent()
	{
		//------------------------------------------------------
		// ターゲットリストドロップダウンセット
		//------------------------------------------------------
		DataView dvTargetLists = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatetment = new SqlStatement("TargetList", "GetTargetListAll"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_TARGETLIST_DEPT_ID, this.LoginOperatorDeptId);
			dvTargetLists = sqlStatetment.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
		int itemIndex = ddlTargetId.Items.Count;
		foreach (DataRowView drv in dvTargetLists)
		{
			(new List<DropDownList> { ddlTargetId, ddlTargetId2, ddlTargetId3, ddlTargetId4, ddlTargetId5 }).ForEach(ddl =>
			{
				ddl.Items.Add(new ListItem(string.Format("{0}({1})", drv[Constants.FIELD_TARGETLIST_TARGET_NAME], drv[Constants.FIELD_TARGETLIST_DATA_COUNT]), (string)drv[Constants.FIELD_TARGETLIST_TARGET_ID]));
				ddl.Items[itemIndex].Attributes["disable_extract"] = Constants.TARGET_LIST_IMPORT_TYPE_LIST.Contains((string)drv[Constants.FIELD_TARGETLIST_TARGET_TYPE]).ToString();
			});
			itemIndex++;
		}

		//------------------------------------------------------
		// メール配信除外設定ドロップダウンセット
		//------------------------------------------------------
		Enumerable.Range(1, 10).ToList().ForEach(iLoop => ddlExceptErrorPoint.Items.Add(iLoop.ToString()));
		foreach (ListItem li in ddlExceptErrorPoint.Items)
		{
			li.Selected = (li.Value == "5");
		}

		//------------------------------------------------------
		// メール文章ドロップダウンセット
		//------------------------------------------------------
		DataView dvMailTextLists = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatetment = new SqlStatement("MailDistText", "GetMailDistTextAll"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MAILDISTTEXT_DEPT_ID, this.LoginOperatorDeptId);
			dvMailTextLists = sqlStatetment.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		ddlMailTextId.Items.Add(new ListItem("", ""));
		foreach (DataRowView drv in dvMailTextLists)
		{
			ddlMailTextId.Items.Add(new ListItem((string)drv[Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME], (string)drv[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID]));
		}

		//------------------------------------------------------
		// スケジュール
		//------------------------------------------------------
		// ラジオボタン系
		rbExecByManual.Text = ValueText.GetValueText(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_EXEC_TIMING, Constants.FLG_MAILDISTSETTING_EXEC_TIMING_MANUAL);
		//rbExecByAction.Text = ValueText.GetValueText(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_EXEC_TIMING, Constants.FLG_MAILDISTSETTING_EXEC_TIMING_ACTION);
		rbExecBySchedule.Text = ValueText.GetValueText(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_EXEC_TIMING, Constants.FLG_MAILDISTSETTING_EXEC_TIMING_SCHEDULE);
		rbExecByManual.Checked = true;

		rbScheRepeatDay.Text = ValueText.GetValueText(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN, Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_DAY);
		rbScheRepeatWeek.Text = ValueText.GetValueText(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN, Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_WEEK);
		rbScheRepeatMonth.Text = ValueText.GetValueText(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN, Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_MONTH);
		rbScheRepeatOnce.Text = ValueText.GetValueText(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN, Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_ONCE);
		rbScheRepeatOnce.Checked = true;

		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MAILDISTSETTING, Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY_OF_WEEK))
		{
			rblScheDayOfWeek.Items.Add(li);
		}
		if (rblScheDayOfWeek.Items.Count > 0)
		{
			rblScheDayOfWeek.Items[0].Selected = true;
		}

		ucScheDateTimeOnce.SetStartDate(DateTime.Today);
		if (this.IsBackFromConfirm
			&& (Session[Constants.SESSION_KEY_PARAM] != null))
		{
			var htParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM];

			var scheDateTimeOnce = string.Format("{0}/{1}/{2} {3}:{4}:{5}",
				StringUtility.ToEmpty(htParam[Constants.FIELD_MAILDISTSETTING_SCHEDULE_YEAR]),
				StringUtility.ToEmpty(htParam[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MONTH]),
				StringUtility.ToEmpty(htParam[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY]),
				StringUtility.ToEmpty(htParam[Constants.FIELD_MAILDISTSETTING_SCHEDULE_HOUR]),
				StringUtility.ToEmpty(htParam[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MINUTE]),
				StringUtility.ToEmpty(htParam[Constants.FIELD_MAILDISTSETTING_SCHEDULE_SECOND]));

			var scheDateTimeOnceStart = new DateTime();
			if (DateTime.TryParse(scheDateTimeOnce, out scheDateTimeOnceStart))
			{
				ucScheDateTimeOnce.SetStartDate(scheDateTimeOnceStart);
			}
		}

		// 実行タイミングの日付初期化
		ucScheDateTime.SetDate(DateTime.Now.Date);
	}

	/// <summary>
	/// 確認ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 入力値格納
		//------------------------------------------------------
		// 基本情報セット
		Hashtable htInput = new Hashtable();
		if (m_strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			htInput.Add(Constants.FIELD_MAILDISTSETTING_MAILDIST_ID, hdnMailDistId.Value);
		}
		htInput.Add(Constants.FIELD_MAILDISTSETTING_DEPT_ID, this.LoginOperatorDeptId);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME, tbMailDistName.Text);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_MAILTEXT_ID, ddlMailTextId.SelectedValue);
		htInput.Add(Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME, ddlMailTextId.SelectedItem.Text);	// 表示用
		htInput.Add(Constants.FIELD_MAILDISTSETTING_TARGET_ID, ddlTargetId.SelectedValue);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_TARGET_ID2, ddlTargetId2.SelectedValue);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_TARGET_ID3, ddlTargetId3.SelectedValue);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_TARGET_ID4, ddlTargetId4.SelectedValue);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_TARGET_ID5, ddlTargetId5.SelectedValue);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG, ((ddlTargetId.SelectedValue != "") && cbTargetExtract.Checked) ? Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_ON : Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_OFF);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG2, ((ddlTargetId2.SelectedValue != "") && cbTargetExtract2.Checked) ? Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_ON : Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_OFF);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG3, ((ddlTargetId3.SelectedValue != "") && cbTargetExtract3.Checked) ? Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_ON : Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_OFF);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG4, ((ddlTargetId4.SelectedValue != "") && cbTargetExtract4.Checked) ? Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_ON : Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_OFF);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG5, ((ddlTargetId5.SelectedValue != "") && cbTargetExtract5.Checked) ? Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_ON : Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_OFF);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_EXCEPT_ERROR_POINT, ddlExceptErrorPoint.SelectedValue);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG, cbExceptMobileMail.Checked ? Constants.FLG_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG_ON : Constants.FLG_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG_OFF);
		htInput.Add(
			Constants.FIELD_MAILDISTSETTING_ENABLE_DEDUPLICATION,
			cbEnableDeduplication.Checked
				? Constants.FLG_MAILDISTSETTING_ENABLE_DEDUPLICATION_ENABLED
				: Constants.FLG_MAILDISTSETTING_ENABLE_DEDUPLICATION_DISABLED);

		// 排除リスト生成（重複排除）
		string[] strExceptLists = tbExceptList.Text.Replace(" ","").Replace("\r","").Replace("\n","").Replace("\t","").ToLower().Split(',');
		Hashtable htTemp = new Hashtable();
		StringBuilder sbExceptLists = new StringBuilder();
		foreach (string strExcept in strExceptLists)
		{
			if ((strExcept.Length != 0) && (htTemp.ContainsKey(strExcept) == false))
			{
				if (sbExceptLists.Length != 0)
				{
					sbExceptLists.Append(",");
				}

				sbExceptLists.Append(strExcept);
				htTemp[strExcept] = null;
			}
		}
		htInput.Add("except_list", sbExceptLists.ToString());

		htInput.Add(Constants.FIELD_MAILDISTSETTING_VALID_FLG,
			(cbValidFlg.Checked ? Constants.FLG_MAILDISTSETTING_VALID_FLG_VALID : Constants.FLG_MAILDISTSETTING_VALID_FLG_INVALID));
		htInput.Add(Constants.FIELD_MAILDISTSETTING_LAST_CHANGED, this.LoginOperatorName);

		// 入力チェック用
		htInput.Add("target_ids", ddlTargetId.SelectedValue + ddlTargetId2.SelectedValue + ddlTargetId3.SelectedValue + ddlTargetId4.SelectedValue + ddlTargetId5.SelectedValue);

		// 配信タイミング情報セット(初期化)
		htInput.Add(Constants.FIELD_MAILDISTSETTING_EXEC_TIMING, "");
		htInput.Add(Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN, "");

		htInput.Add(Constants.FIELD_MAILDISTSETTING_SCHEDULE_YEAR, null);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_SCHEDULE_MONTH, null);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY, null);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY_OF_WEEK, "");
		htInput.Add(Constants.FIELD_MAILDISTSETTING_SCHEDULE_HOUR, null);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_SCHEDULE_MINUTE, null);
		htInput.Add(Constants.FIELD_MAILDISTSETTING_SCHEDULE_SECOND, null);

		// 配信タイミング情報セット
		if (rbExecByManual.Checked)
		{
			htInput[Constants.FIELD_MAILDISTSETTING_EXEC_TIMING] = Constants.FLG_MAILDISTSETTING_EXEC_TIMING_MANUAL;
			htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN] = "";
		}
		else if (rbExecBySchedule.Checked)
		{
			htInput[Constants.FIELD_MAILDISTSETTING_EXEC_TIMING] = Constants.FLG_MAILDISTSETTING_EXEC_TIMING_SCHEDULE;

			if (rbScheRepeatDay.Checked)
			{
				htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN] = Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_DAY;
			}
			else if (rbScheRepeatWeek.Checked)
			{
				htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN] = Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_WEEK;
				htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY_OF_WEEK] = rblScheDayOfWeek.SelectedValue;
			}
			else if (rbScheRepeatMonth.Checked)
			{
				htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN] = Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_MONTH;
				htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY] = ucScheDateTime.Day;
			}
			else if (rbScheRepeatOnce.Checked)
			{
				htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN] = Constants.FLG_MAILDISTSETTING_SCHEDULE_KBN_ONCE;

				var scheDate = DateTime.Parse(ucScheDateTimeOnce.HfStartDate.Value);
				htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_YEAR] = scheDate.Year.ToString();
				htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MONTH] = scheDate.Month.ToString();
				htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY] = scheDate.Day.ToString();

				// 日付チェック用
				htInput["schedule_date"] = ucScheDateTime.DateString;
			}

			if (rbScheRepeatOnce.Checked)
			{
				var scheDate = DateTime.Parse(ucScheDateTimeOnce.HfStartTime.Value);
				htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_HOUR] = scheDate.Hour.ToString();
				htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MINUTE] = scheDate.Minute.ToString();
				htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_SECOND] = scheDate.Second.ToString();
			}
			else
			{
				htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_HOUR] = ucScheDateTime.Hour;
				htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MINUTE] = ucScheDateTime.Minute;
				htInput[Constants.FIELD_MAILDISTSETTING_SCHEDULE_SECOND] = ucScheDateTime.Second;
			}

		}

		// セッションへセット
		Session[Constants.SESSION_KEY_PARAM] = htInput;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = 1;

		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		string strErrorMessage = Validator.Validate("MailDistSettingRegist", htInput);

		// 排除リスト入力のメールアドレス形式チェック
		if (((string)htInput["except_list"]).Length != 0)
		{
			strErrorMessage += Validator.CheckAllMailAddrInputs((string)htInput["except_list"]);
		}

		if (strErrorMessage.Length != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// リダイレクト
		//------------------------------------------------------
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MAILDIST_SETTING_CONFIRM);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_CONFIRM);
		if (m_strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_MAILDIST_ID).Append("=").Append(HttpUtility.UrlEncode(hdnMailDistId.Value));
		}

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>確認画面から戻ってきたか</summary>
	protected bool IsBackFromConfirm
	{
		get { return (Session[Constants.SESSION_KEY_PARAM_FOR_BACK] != null); }
	}
}
