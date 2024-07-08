/*
=========================================================================================================
  Module      : ターゲットリスト設定登録ページ処理(TargetListRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using w2.App.Common.DataCacheController;
using w2.App.Common.OrderExtend;
using w2.App.Common.TargetList;
using w2.Common.Web;
using w2.Domain.FixedPurchase;
using w2.Domain.User.Helper;

public partial class Form_TargetList_TargetListRegister : BasePage
{
	string m_strTargetId = null;
	public string m_strActionKbn = null;
	UserExtendSettingList m_userExtendSettingList = null;
	const string CONST_USER_EXTEND_SETTING_LIST = "userExtendSettingList";

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
			// パラメタ取得
			//------------------------------------------------------
			m_strActionKbn = (string)Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState[Constants.REQUEST_KEY_ACTION_STATUS] = m_strActionKbn;

			m_strTargetId = (string)Request[Constants.REQUEST_KEY_TARGET_ID];
			ViewState[Constants.REQUEST_KEY_TARGET_ID] = m_strTargetId;

			// ユーザー拡張項目の設定をDBから読込む
			m_userExtendSettingList = new UserExtendSettingList(this.LoginOperatorId);
			ViewState[CONST_USER_EXTEND_SETTING_LIST] = m_userExtendSettingList;

			//------------------------------------------------------
			// On Load イベント追加（スケジュール設定保持用）
			//------------------------------------------------------
			this.Master.OnLoadEvents += "RefleshTargetListSchedule();"; // ユーザーコントロールに存在

			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponent(m_strActionKbn);

			//------------------------------------------------------
			// ターゲット名、条件、スケジュール表示
			//------------------------------------------------------
			TargetListConditionList lTargetListCondition = null;	// switch内で宣言がかぶるのでここで宣言
			var dataValue = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
			switch (m_strActionKbn)
			{
				case Constants.ACTION_STATUS_INSERT:

					if (dataValue == null)
					{
						// 条件部分初期化
						lTargetListCondition = new TargetListConditionList();
						var tlc = new TargetListCondition();
						lTargetListCondition.TargetConditionList.Add(tlc);
					}
					else
					{
						// Assign value if session has data
						tbTargetListName.Text = (string)dataValue[Constants.FIELD_TARGETLIST_TARGET_NAME];
						lTargetListCondition = (TargetListConditionList)dataValue["lTargetListCondition"];
						SetExecTiming(dataValue);
					}

					TargetListConditonBind(lTargetListCondition);
					break;

				case Constants.ACTION_STATUS_UPDATE:
				case Constants.ACTION_STATUS_COPY_INSERT:

					// Assign Value If Session Has Data
					if ((dataValue != null) && (StringUtility.ToEmpty(dataValue[Constants.FIELD_TARGETLIST_TARGET_ID]) == this.m_strTargetId))
					{
						lbTargetId.Text = WebSanitizer.HtmlEncode(this.m_strTargetId);
						tbTargetListName.Text = (string)dataValue[Constants.FIELD_TARGETLIST_TARGET_NAME];
						lTargetListCondition = (TargetListConditionList)dataValue["lTargetListCondition"];
					}

					if (lTargetListCondition == null)
					{
						// ターゲットリストデータ取得
						DataRowView targetList = null;
						DataView targetListDatas = TargetListUtility.GetTargetList(this.LoginOperatorDeptId, this.m_strTargetId);
						if (targetListDatas.Count == 0)
						{
							// エラーページへ
							Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
							Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
						}

						targetList = targetListDatas[0];

						// ターゲット基本情報出力
						lbTargetId.Text = WebSanitizer.HtmlEncode(targetList[Constants.FIELD_TARGETLIST_TARGET_ID]);
						tbTargetListName.Text = (string)targetList[Constants.FIELD_TARGETLIST_TARGET_NAME];

						// 抽出条件
						lTargetListCondition = TargetListConditionRelationXml.CreateTargetListConditionList((string)targetList[Constants.FIELD_TARGETLIST_TARGET_CONDITION]);

						// Create Hashtable ObjValue
						dataValue = CreateScheduleKbnHashtable(targetList);
					}

					// Binding data
					SetExecTiming(dataValue);
					TargetListConditonBind(lTargetListCondition);
					break;
			}

			// Clear Session
			Session[Constants.SESSION_KEY_PARAM] = null;
			//------------------------------------------------------
			// 条件部分初期化
			//------------------------------------------------------

		}
		else
		{
			m_strActionKbn = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];
			m_strTargetId = (string)ViewState[Constants.REQUEST_KEY_TARGET_ID];
			m_userExtendSettingList = (UserExtendSettingList)ViewState[CONST_USER_EXTEND_SETTING_LIST];
		}
	}

	/// <summary>
	/// Create Schedule Kbn HashTable
	/// </summary>
	/// <param name="targetList">Target List</param>
	/// <returns>Schedule Hashtable</returns>
	private Hashtable CreateScheduleKbnHashtable(DataRowView targetList)
	{
		return new Hashtable(){
			{Constants.FIELD_TARGETLIST_EXEC_TIMING, StringUtility.ToEmpty(targetList[Constants.FIELD_TARGETLIST_EXEC_TIMING])},
			{Constants.FIELD_TARGETLIST_SCHEDULE_KBN, StringUtility.ToEmpty(targetList[Constants.FIELD_TARGETLIST_SCHEDULE_KBN])},
			{Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK, StringUtility.ToEmpty(targetList[Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK])},
			{Constants.FIELD_TARGETLIST_SCHEDULE_YEAR, (string.IsNullOrEmpty(StringUtility.ToEmpty(targetList[Constants.FIELD_TARGETLIST_SCHEDULE_YEAR])) ? 0 : (int)targetList[Constants.FIELD_TARGETLIST_SCHEDULE_YEAR])},
			{Constants.FIELD_TARGETLIST_SCHEDULE_MONTH, (string.IsNullOrEmpty(StringUtility.ToEmpty(targetList[Constants.FIELD_TARGETLIST_SCHEDULE_MONTH])) ? 0 : (int)targetList[Constants.FIELD_TARGETLIST_SCHEDULE_MONTH])},
			{Constants.FIELD_TARGETLIST_SCHEDULE_DAY, (string.IsNullOrEmpty(StringUtility.ToEmpty(targetList[Constants.FIELD_TARGETLIST_SCHEDULE_DAY])) ? 0 : (int)targetList[Constants.FIELD_TARGETLIST_SCHEDULE_DAY])},
			{Constants.FIELD_TARGETLIST_SCHEDULE_HOUR, (string.IsNullOrEmpty(StringUtility.ToEmpty(targetList[Constants.FIELD_TARGETLIST_SCHEDULE_HOUR])) ? 0 : (int)targetList[Constants.FIELD_TARGETLIST_SCHEDULE_HOUR])},
			{Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE, (string.IsNullOrEmpty(StringUtility.ToEmpty(targetList[Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE])) ? 0 : (int)targetList[Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE])},
			{Constants.FIELD_TARGETLIST_SCHEDULE_SECOND, (string.IsNullOrEmpty(StringUtility.ToEmpty(targetList[Constants.FIELD_TARGETLIST_SCHEDULE_SECOND])) ? 0 : (int)targetList[Constants.FIELD_TARGETLIST_SCHEDULE_SECOND])}};
	}

	/// <summary>
	/// Set Exec Timing
	/// </summary>
	/// <param name="scheduleKbn">scheduleKbn</param>
	private void SetExecTiming(Hashtable scheduleKbn)
	{
		// スケジュール情報出力
		switch ((string)scheduleKbn[Constants.FIELD_TARGETLIST_EXEC_TIMING])
		{
			case Constants.FLG_TARGETLIST_EXEC_TIMING_SCHEDULE:
				ucScheduleRegisterForm.ExecKbn = Constants.FLG_TARGETLIST_EXEC_TIMING_SCHEDULE;
				ucScheduleRegisterForm.ScheKbn = (string)scheduleKbn[Constants.FIELD_TARGETLIST_SCHEDULE_KBN];
				switch ((string)scheduleKbn[Constants.FIELD_TARGETLIST_SCHEDULE_KBN])
				{
					case Constants.FLG_TARGETLIST_SCHEDULE_KBN_DAY:
						// 時間のみ
						break;

					case Constants.FLG_TARGETLIST_SCHEDULE_KBN_WEEK:
						ucScheduleRegisterForm.ScheDayOfWeek = (string)scheduleKbn[Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK];
						break;

					case Constants.FLG_TARGETLIST_SCHEDULE_KBN_MONTH:
						ucScheduleRegisterForm.ScheDay = (int)scheduleKbn[Constants.FIELD_TARGETLIST_SCHEDULE_DAY];
						break;

					case Constants.FLG_TARGETLIST_SCHEDULE_KBN_ONCE:
						ucScheduleRegisterForm.ScheYear = (int)scheduleKbn[Constants.FIELD_TARGETLIST_SCHEDULE_YEAR];
						ucScheduleRegisterForm.ScheMonth = (int)scheduleKbn[Constants.FIELD_TARGETLIST_SCHEDULE_MONTH];
						ucScheduleRegisterForm.ScheDay = (int)scheduleKbn[Constants.FIELD_TARGETLIST_SCHEDULE_DAY];
						break;
				}
				ucScheduleRegisterForm.ScheHour = (int)scheduleKbn[Constants.FIELD_TARGETLIST_SCHEDULE_HOUR];
				ucScheduleRegisterForm.ScheMinute = (int)scheduleKbn[Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE];
				ucScheduleRegisterForm.ScheSecond = (int)scheduleKbn[Constants.FIELD_TARGETLIST_SCHEDULE_SECOND];

				break;

			case Constants.FLG_TARGETLIST_EXEC_TIMING_MANUAL:
				ucScheduleRegisterForm.ExecKbn = Constants.FLG_TARGETLIST_EXEC_TIMING_MANUAL;
				break;
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	/// <param name="strActionKbn">アクション区分</param>
	private void InitializeComponent(string strActionKbn)
	{
		trTargetId.Visible = (strActionKbn == Constants.ACTION_STATUS_UPDATE);
	}

	/// <summary>
	/// 追加クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAdd_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// データ取得＆追加
		//------------------------------------------------------
		TargetListConditionList lTargetListCondition = CreateTargetListConditionList();
		var tlc = new TargetListCondition();

		// 初期値AND設定
		if (lTargetListCondition.TargetConditionList[0].GetConditionType(lTargetListCondition.TargetConditionList[0]) == null)
		{
			tlc.ConditionType = TargetListCondition.CONDITION_TYPE_AND;
		}
		else
		{
			tlc.ConditionType = lTargetListCondition.TargetConditionList[0].GetConditionType(lTargetListCondition.TargetConditionList[0]);
		}
		lTargetListCondition.TargetConditionList.Add(tlc);

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		TargetListConditonBind(lTargetListCondition);
	}

	/// <summary>
	/// 削除クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		// rConditionsのIndex
		var conditionListIndex = ((RepeaterItem)((Button)sender).Parent.Parent.Parent).ItemIndex;
		// rGroupConditionsのIndex
		int groupIndex = ((RepeaterItem)((Button)sender).Parent).ItemIndex;

		//------------------------------------------------------
		// データ取得＆削除
		//------------------------------------------------------
		TargetListConditionList lTargetListCondition = CreateTargetListConditionList();

		// Interface取得
		var groupCondition = lTargetListCondition.TargetConditionList[conditionListIndex];

		groupCondition.Remove(lTargetListCondition, conditionListIndex, groupIndex);

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		TargetListConditonBind(lTargetListCondition);
	}

	/// <summary>
	/// データ区分選択変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDataKbn_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 対象リピータアイテム取得
		RepeaterItem riTarget = ((RepeaterItem)((DropDownList)sender).Parent);

		// データフィールド初期化
		InitializeDataFieldComponent(riTarget, "");

		// 値初期化
		InitializeValueComponent(riTarget, null);

		// イコールサイン初期化
		InitializeEqualSignComponent(riTarget, "");

		// 注文ありなしドロップダウン初期化
		InitializeOrderExist(riTarget, "");

		// 定期注文ありなしドロップダウン初期化
		InitializeFixedPurchaseOrderExist(riTarget, "");

		// ポイントありなしドロップダウン初期化
		InitializePointExist(riTarget, "");

		// DM発送履歴情報ありなし「ドロップダウン初期化
		InitializeDmShippingHistory(riTarget, "");

		// お気に入り商品ありなしドロップダウン初期化
		InitializeFavoriteExist(riTarget, "");
	}

	/// <summary>
	/// データフィールド選択変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDataField_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 対象リピータアイテム取得
		RepeaterItem riTarget = ((RepeaterItem)((DropDownList)sender).Parent.Parent);

		if (((DropDownList)riTarget.FindControl("ddlDataField")).SelectedValue != string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1))
		{
			((DropDownList)riTarget.FindControl("ddlEqualSign")).Visible = true;
			((HtmlGenericControl)riTarget.FindControl("spanFixedPurchasePattern")).Visible = false;
			((DropDownList)riTarget.FindControl("ddlFixedPurchasePattern")).Visible = false;
		}
		else
		{
			((DropDownList)riTarget.FindControl("ddlEqualSign")).Visible = false;
		}

		// 値初期化
		InitializeValueComponent(riTarget, null);

		// イコールサイン初期化
		InitializeEqualSignComponent(riTarget, "");

		// 注文ありなしドロップダウン初期化
		InitializeOrderExist(riTarget, "");

		// 定期注文ありなしドロップダウン初期化
		InitializeFixedPurchaseOrderExist(riTarget, "");

		// ポイントありなしドロップダウン初期化
		InitializePointExist(riTarget, "");

		// DM発送履歴情報ありなし「ドロップダウン初期化
		InitializeDmShippingHistory(riTarget, "");

		// お気に入り商品ありなしドロップダウン初期化
		InitializeFavoriteExist(riTarget, "");
	}

	/// <summary>
	/// 定期配送パターンドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlFixedPurchasePattern_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 対象リピータアイテム取得
		RepeaterItem riTarget = ((RepeaterItem)((DropDownList)sender).Parent.Parent.Parent);
		var fixedPurchasePatternItem = ((DropDownList)riTarget.FindControl("ddlFixedPurchasePattern")).SelectedValue;
		var dayOfWeek = ((DropDownList)sender).Parent.FindControl("spanDayOfWeek");

		InitializeFixedPurchasePattern(riTarget, null, "");

		// イコールサイン初期化
		InitializeEqualSignComponent(riTarget, "");

		// 定期注文ありなしドロップダウン初期化
		InitializeFixedPurchaseOrderExist(riTarget, "");

		((DropDownList)sender).Parent.FindControl("spanMonth").Visible = ((DropDownList)sender).Parent.FindControl("spanDay").Visible
			= IsValidPatern(
				fixedPurchasePatternItem,
				Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE);

		((DropDownList)sender).Parent.FindControl("spanIntervalDay").Visible = IsValidPatern(
				fixedPurchasePatternItem,
				Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS);

		((DropDownList)sender).Parent.FindControl("spanIntervalMonth").Visible = dayOfWeek.Visible
			= IsValidPatern(
				fixedPurchasePatternItem,
				Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY);

		if (dayOfWeek.Visible) return;

		((DropDownList)sender).Parent.FindControl("spanIntervalWeek").Visible = dayOfWeek.Visible
			= IsValidPatern(
				fixedPurchasePatternItem,
				Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY);

		if (string.IsNullOrEmpty(((DropDownList)riTarget.FindControl("ddlFixedPurchasePattern")).SelectedValue)) return;

		((DropDownList)riTarget.FindControl("ddlEqualSign")).Visible = true;
		((DropDownList)riTarget.FindControl("ddlFixedPurchaseOrderExist")).Visible = true;
	}

	/// <summary>
	/// 定期配送パターン文言表示判定
	/// </summary>
	/// <param name="item">定期配送パターンドロップダウンリストのアイテム</param>
	/// <param name="fixedPurchasePattern">定期配送パターン</param>
	/// <returns>表示するか</returns>
	private bool IsValidPatern(string item, string fixedPurchasePattern)
	{
		var result = item == fixedPurchasePattern;
		return result;
	}

	/// <summary>
	/// 定期配送パターン設定
	/// </summary>
	/// <param name="repeaterItem">対象リピータアイテム</param>
	/// <param name="defaultValues">デフォルト選択値</param>
	/// <param name="fixedPurchasePattern">デフォルト選択値</param>
	private void InitializeFixedPurchasePattern(RepeaterItem repeaterItem, List<TargetListCondition.Data> defaultValues, string fixedPurchasePattern)
	{
		var tbFixedPurchasePattern = (TextBox)repeaterItem.FindControl("tbFixedPurchasePattern");
		var tbFixedPurchasePatternDetail = (TextBox)repeaterItem.FindControl("tbFixedPurchasePatternDetail");
		var ddlFixedPurchasePattern = (DropDownList)repeaterItem.FindControl("ddlFixedPurchasePattern");
		var ddlDayOfWeek = (DropDownList)repeaterItem.FindControl("ddlDayOfWeek");

		((DropDownList)repeaterItem.FindControl("ddlValue")).Visible = false;
		((TextBox)repeaterItem.FindControl("tbValue1")).Visible = false;
		ddlFixedPurchasePattern.Visible = true;

		if (ddlFixedPurchasePattern.Items.Count == 0)
		{
			ddlFixedPurchasePattern.Items.Add("");
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1))
			{
				ddlFixedPurchasePattern.Items.Add(li);
			}
		}

		if (string.IsNullOrEmpty(fixedPurchasePattern) == false)
		{
			foreach (ListItem li in ddlFixedPurchasePattern.Items)
			{
				li.Selected = (li.Text == fixedPurchasePattern);
			}
		}

		var defaultValue = ((defaultValues != null) && (defaultValues.Count > 0)) ? defaultValues[0].Value.Split(',') : null;
		var firstValue = ((defaultValue != null) && (defaultValue.Length > 0)) ? defaultValue[0] : "";
		var secondValue = ((defaultValue != null) && (defaultValue.Length > 1)) ? defaultValue[1] : "";
		var thirdValue = ((defaultValue != null) && (defaultValue.Length > 2))
			? ValueText.GetValueText(
				Constants.TABLE_FIXEDPURCHASE,
				Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_DAY_OF_WEEK,
				defaultValue[2])
			: "";

		ddlDayOfWeek.Items.Clear();
		ddlDayOfWeek.Items.Add("");
		foreach (ListItem li in ValueText.GetValueItemList("w2_FixedPurchase", "schedule_day_of_week"))
		{
			ddlDayOfWeek.Items.Add(li);
		}

		if (string.IsNullOrEmpty(((DropDownList)repeaterItem.FindControl("ddlFixedPurchasePattern")).SelectedValue))
		{
			((DropDownList)repeaterItem.FindControl("ddlEqualSign")).Visible = false;
			((DropDownList)repeaterItem.FindControl("ddlFixedPurchaseOrderExist")).Visible = false;
			((DropDownList)repeaterItem.FindControl("ddlFixedPurchaseOrderExist")).Items.Clear();
			((HtmlGenericControl)repeaterItem.FindControl("spanFixedPurchasePattern")).Visible = false;
			return;
		}

		((HtmlGenericControl)repeaterItem.FindControl("spanFixedPurchasePattern")).Visible = true;

		switch (ddlFixedPurchasePattern.SelectedValue)
		{
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
				tbFixedPurchasePattern.Visible = true;
				tbFixedPurchasePatternDetail.Visible = true;
				ddlDayOfWeek.Visible = false;

				if (string.IsNullOrEmpty(firstValue) == false)
				{
					tbFixedPurchasePattern.Text = firstValue;
				}

				if (string.IsNullOrEmpty(secondValue) == false)
				{
					tbFixedPurchasePatternDetail.Text = secondValue;
				}

				break;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
				tbFixedPurchasePattern.Visible = true;
				tbFixedPurchasePatternDetail.Visible = true;
				ddlDayOfWeek.Visible = true;

				tbFixedPurchasePattern.Text = firstValue;
				tbFixedPurchasePatternDetail.Text = secondValue;
				if (string.IsNullOrEmpty(thirdValue) == false)
				{
					foreach (ListItem li in ddlDayOfWeek.Items)
					{
						li.Selected = (li.Text == thirdValue);
					}
				}

				break;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
				tbFixedPurchasePattern.Visible = false;
				tbFixedPurchasePatternDetail.Visible = true;
				ddlDayOfWeek.Visible = false;
				if (string.IsNullOrEmpty(firstValue) == false)
				{
					tbFixedPurchasePatternDetail.Text = firstValue;
				}

				break;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
				tbFixedPurchasePattern.Visible = true;
				tbFixedPurchasePatternDetail.Visible = false;
				ddlDayOfWeek.Visible = true;

				if (string.IsNullOrEmpty(firstValue) == false)
				{
					tbFixedPurchasePattern.Text = firstValue;
				}

				if (string.IsNullOrEmpty(secondValue) == false)
				{
					secondValue = ValueText.GetValueText(
						Constants.TABLE_FIXEDPURCHASE,
						Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_DAY_OF_WEEK,
						secondValue);
				}

				if (string.IsNullOrEmpty(secondValue) == false)
				{
					foreach (ListItem li in ddlDayOfWeek.Items)
					{
						li.Selected = (li.Text == secondValue);
					}
				}
				break;
		}
	}

	/// <summary>
	/// データフィールド ドロップダウン初期化
	/// </summary>
	/// <param name="riTarget">対象リピータアイテム</param>
	/// <param name="strDefaultValue">デフォルト選択値</param>
	private void InitializeDataFieldComponent(RepeaterItem riTarget, string strDefaultValue)
	{
		DropDownList ddlDataKbn = ((DropDownList)riTarget.FindControl("ddlDataKbn"));
		DropDownList ddlDataField = ((DropDownList)riTarget.FindControl("ddlDataField"));
		DropDownList ddlEqualSign = ((DropDownList)riTarget.FindControl("ddlEqualSign"));
		var lPrefix = ((Literal)riTarget.FindControl("lPrefix"));

		((DropDownList)riTarget.FindControl("ddlFixedPurchasePattern")).Visible = false;

		// いったん削除
		ddlDataField.Items.Clear();

		// データ区分が未選択の場合、"の"からデータフィールド以降を非表示にする
		if (ddlDataKbn.SelectedValue.Length == 0)
		{
			((HtmlGenericControl)riTarget.FindControl("spanInputCondition")).Visible = false;
			((HtmlGenericControl)riTarget.FindControl("spanInputSql")).Visible = false;
			return;
		}
		WebSanitizer.HtmlEncode(lPrefix.Text = ddlDataKbn.SelectedValue == TargetListCondition.DATAKBN_FAVORITE_PRODUCT_INFO ? "で" : "の");
		switch (ddlDataKbn.SelectedValue)
		{
			case TargetListCondition.DATAKBN_USER_INFO:
			case TargetListCondition.DATAKBN_USER_ATTRIBUTE_INFO:
			case TargetListCondition.DATAKBN_ORDER_INFO:
			case TargetListCondition.DATAKBN_POINT_INFO:
			case TargetListCondition.DATAKBN_AGGREGATED_POINT_INFO:
			case TargetListCondition.DATAKBN_LIMITED_TERM_POINT_INFO:
			case TargetListCondition.DATAKBN_CART_INFO:
			case TargetListCondition.DATAKBN_FIXEDPURCHASE_INFO:
			case TargetListCondition.DATAKBN_COUPON_INFO:
			case TargetListCondition.DATAKBN_MAIL_CLICK_INFO:
			case TargetListCondition.DATAKBN_DM_SHIPPING_HISTORY_INFO:
			case TargetListCondition.DATAKBN_ORDER_AGGREGATE:
			case TargetListCondition.DATAKBN_FAVORITE_PRODUCT_INFO:
				((HtmlGenericControl)riTarget.FindControl("spanInputCondition")).Visible = true;
				((HtmlGenericControl)riTarget.FindControl("spanInputSql")).Visible = false;

				// DataField作成
				XmlDocument xdDataField = new XmlDocument();
				xdDataField.Load(AppDomain.CurrentDomain.BaseDirectory + "/Xml/TargetList/DataField.Xml");
				XmlNode xnDataFields = xdDataField.SelectSingleNode("/DataField/" + ddlDataKbn.SelectedValue);
				if (xnDataFields != null)
				{
					ddlDataField.Items.Add("");
					foreach (XmlNode xnField in xnDataFields.ChildNodes)
					{
						if (xnField.NodeType != XmlNodeType.Comment)
						{
							// オプション判定
							if (xnField.Attributes["option"] != null)
							{
								switch (xnField.Attributes["option"].Value)
								{
									case "cpm":
										if (Constants.CPM_OPTION_ENABLED == false) continue;
										break;

									case "global":
										if (Constants.GLOBAL_OPTION_ENABLE == false) continue;
										break;

									case "subscription":
										if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED == false) continue;
										break;
								}
							}

							// モール連携オプション未使用の場合、「モールID」をデータフィールドに追加しない
							if ((Constants.MALLCOOPERATION_OPTION_ENABLED == false)
								&& (xnField.Attributes["field"].Value == Constants.TABLE_USER + "." + Constants.FIELD_USER_MALL_ID))
							{
								continue;
							}

							// 会員ランクＯＰ・定期購入ＯＰのいずれか未利用の場合
							// かつ 取得した値が「ユーザー情報」の「定期会員ランク」場合、「定期会員ランク」をデータフィールドに追加しない
							if (((Constants.MEMBER_RANK_OPTION_ENABLED == false) || (Constants.FIXEDPURCHASE_OPTION_ENABLED == false))
								&& (xnField.Attributes["field"].Value == Constants.TABLE_USER + "." + Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG))
							{
								continue;
							}

							// クーポンオプション未使用の場合、「クーポンコード」をデータフィールドに追加しない
							if ((Constants.MARKETINGPLANNER_COUPON_OPTION_ENABLE == false)
								&& (xnField.Attributes["field"].Value == Constants.TABLE_ORDERCOUPON + "." + Constants.FIELD_ORDERCOUPON_COUPON_CODE))
							{
								continue;
							}

							// バリエーションオプション未使用の場合、「バリエーションID」をデータフィールドに追加しない
							if ((Constants.VARIATION_FAVORITE_CORRESPONDENCE == false)
								&& (xnField.Attributes["field"].Value == Constants.TABLE_FAVORITE + "." + Constants.FIELD_FAVORITE_VARIATION_ID))
							{
								continue;
							}

							// モバイルデータの表示と非表示OFF時はモバイルメールアドレスを追加しない
							if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
								&& (xnField.Attributes["field"].Value == Constants.TABLE_USER + "." + Constants.FIELD_USER_MAIL_ADDR2)) continue;

							// Not add field when realshop option and storepickup option off
							if (Constants.STORE_PICKUP_OPTION_ENABLED == false)
							{
								switch (xnField.Attributes["field"].Value)
								{
									case Constants.TABLE_ORDER + "." + Constants.FIELD_ORDER_STOREPICKUP_STATUS:
									case Constants.TABLE_ORDER + "." + Constants.FIELD_ORDER_STOREPICKUP_STORE_ARRIVED_DATE:
									//case Constants.TABLE_ORDERSHIPPING + "." + Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG:
									case Constants.TABLE_ORDERSHIPPING + "." + Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID:
										continue;
								}
							}

							ddlDataField.Items.Add(new ListItem(xnField.Attributes["name"].Value, xnField.Attributes["field"].Value));
						}
					}
				}

				// 受注情報の場合は拡張項目セット
				if (ddlDataKbn.SelectedValue == TargetListCondition.DATAKBN_ORDER_INFO)
				{
					DataView dvOrderExtend = null;
					using (SqlAccessor sqlAccessor = new SqlAccessor())
					using (SqlStatement sqlStatement = new SqlStatement("OrderExtendStatusSetting", "GetOrderExtendStatusSetting"))
					{
						Hashtable htInput = new Hashtable();
						htInput.Add(Constants.FIELD_ORDEREXTENDSTATUSSETTING_SHOP_ID, this.LoginOperatorShopId);
						htInput.Add("extend_status_max", Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX);
						dvOrderExtend = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
					}

					foreach (DataRowView drv in dvOrderExtend)
					{
						StringBuilder sbOrderExtendQuery = new StringBuilder();
						sbOrderExtendQuery.Append(Constants.TABLE_ORDER);
						sbOrderExtendQuery.Append(".extend_status");
						sbOrderExtendQuery.Append(drv[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO].ToString());

						ddlDataField.Items.Add(new ListItem((string)drv[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME], sbOrderExtendQuery.ToString()));
					}
				}

				// デフォルト値セット
				if (string.IsNullOrEmpty(strDefaultValue) == false)
				{
					foreach (ListItem li in ddlDataField.Items)
					{
						li.Selected = (li.Value == strDefaultValue);
					}
				}
				break;

			case TargetListCondition.DATAKBN_USER_EXTEND_INFO:
				((HtmlGenericControl)riTarget.FindControl("spanInputCondition")).Visible = true;
				((HtmlGenericControl)riTarget.FindControl("spanInputSql")).Visible = false;

				// ユーザマスタ拡張項目の追加(w2_UserExtendSetting)
				ddlDataField.Items.Add("");

				foreach (var userExtendSetting in m_userExtendSettingList.Items)
				{
					// ユーザー拡張項目は、DB定義上NULLは入らない
					StringBuilder userExtendSettingQuery = new StringBuilder();
					userExtendSettingQuery.Append(Constants.TABLE_USEREXTEND);
					userExtendSettingQuery.Append(".");
					userExtendSettingQuery.Append(userExtendSetting.SettingId);

					// ddlDataFieldに項目追加
					ddlDataField.Items.Add(new ListItem(userExtendSetting.SettingName, userExtendSettingQuery.ToString()));
				}

				// 初期値or選択済み項目の選択
				if (strDefaultValue.Length != 0)
				{
					foreach (ListItem li in ddlDataField.Items)
					{
						li.Selected = (li.Value == strDefaultValue);
					}
				}
				break;

			case TargetListCondition.DATAKBN_ORDER_EXTEND_INFO:
			case TargetListCondition.DATAKBN_FIXEDPURCHASE_ORDER_EXTEND_INFO:
				((HtmlGenericControl)riTarget.FindControl("spanInputCondition")).Visible = true;
				((HtmlGenericControl)riTarget.FindControl("spanInputSql")).Visible = false;
				ddlDataField.Items.Add("");
				foreach (var model in DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels)
				{
					var extendSettingQuery = new StringBuilder();
					var table = (ddlDataKbn.SelectedValue == TargetListCondition.DATAKBN_ORDER_EXTEND_INFO)
						? Constants.TABLE_ORDER
						: Constants.TABLE_FIXEDPURCHASE;
					extendSettingQuery.Append(table);
					extendSettingQuery.Append(".");
					extendSettingQuery.Append(model.SettingId);
					ddlDataField.Items.Add(new ListItem(model.SettingName, extendSettingQuery.ToString()));
				}

				if (strDefaultValue.Length != 0)
				{
					foreach (ListItem li in ddlDataField.Items)
					{
						li.Selected = (li.Value == strDefaultValue);
					}
				}
				break;

			case TargetListCondition.DATAKBN_SQL_CONDITION:
				((HtmlGenericControl)riTarget.FindControl("spanInputCondition")).Visible = false;
				((HtmlGenericControl)riTarget.FindControl("spanInputSql")).Visible = true;
				break;

			default:
				((HtmlGenericControl)riTarget.FindControl("spanInputCondition")).Visible = true;
				((HtmlGenericControl)riTarget.FindControl("spanInputSql")).Visible = false;
				break;
		}
	}

	/// <summary>
	/// 値 テキストボックス/ドロップダウン初期化
	/// </summary>
	/// <param name="riTarget">対象リピータアイテム</param>
	/// <param name="lDefaultValues">デフォルト設定値</param>
	private void InitializeValueComponent(RepeaterItem riTarget, List<TargetListCondition.Data> lDefaultValues)
	{
		DropDownList ddlDataKbn = ((DropDownList)riTarget.FindControl("ddlDataKbn"));
		DropDownList ddlDataField = ((DropDownList)riTarget.FindControl("ddlDataField"));
		//HiddenField hfDataType = ((HiddenField)riTarget.FindControl("hfDataType"));
		TextBox tbValue1 = ((TextBox)riTarget.FindControl("tbValue1"));
		TextBox tbValue2 = ((TextBox)riTarget.FindControl("tbValue2"));
		DropDownList ddlValue = ((DropDownList)riTarget.FindControl("ddlValue"));
		DropDownList ddlFixedPurchasePattern = ((DropDownList)riTarget.FindControl("ddlFixedPurchasePattern"));

		var defaultValue = ((lDefaultValues != null) && (lDefaultValues.Count > 0)) ? lDefaultValues[0].Value : null;

		// データフィールド選択有無により"が"から値以降を非表示とする
		if (ddlDataField.SelectedValue.Length == 0)
		{
			((HtmlGenericControl)riTarget.FindControl("spanInputConditionDetail")).Visible = false;
			((HtmlGenericControl)riTarget.FindControl("spanFixedPurchasePattern")).Visible = false;
			ddlValue.Items.Clear();
			ddlFixedPurchasePattern.Items.Clear();
			tbValue1.Text = "";
			return;
		}
		else
		{
			((HtmlGenericControl)riTarget.FindControl("spanInputConditionDetail")).Visible = true;
		}

		switch (ddlDataKbn.SelectedValue)
		{
			case TargetListCondition.DATAKBN_SQL_CONDITION:
				if (defaultValue != null) tbValue2.Text = defaultValue;
				break;

			case TargetListCondition.DATAKBN_USER_EXTEND_INFO:
				ddlValue.Items.Clear();
				ddlFixedPurchasePattern.Items.Clear();
				ddlValue.Items.Add("");
				tbValue1.Text = "";
				// ユーザー拡張項目を条件として選んでいる場合、利用可能な項目をセットする
				if (ddlDataField.SelectedValue.Length != 0)
				{
					// 選択項目の設定内容により、「tbValue」or「ddlValue」を変化させる
					string selectedValueSettingId = ddlDataField.SelectedValue.Replace(Constants.TABLE_USEREXTEND + ".", "");
					// DDLなどのvalueはTBL名も含まれているのでSettingIdと一致するよう整形
					var userExtendSetting =
						m_userExtendSettingList.Items.Find(userExtend => userExtend.SettingId == selectedValueSettingId);

					if (userExtendSetting.InputType == Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT)
					{
						// 拡張項目の設定方法がテキストボックスの場合
						if (defaultValue != null) tbValue1.Text = defaultValue;

						// 表示調整
						ddlValue.Visible = false;
						tbValue1.Visible = true;
					}
					else
					{
						// 拡張項目の設定方法がテキストボックス以外の場合
						AddItemToDropDownList(ddlValue, ((List<ListItem>)userExtendSetting.ListItem), defaultValue);

						// 表示調整
						ddlValue.Visible = true;
						tbValue1.Visible = false;
					}
					break;
				}
				break;

			case TargetListCondition.DATAKBN_FIXEDPURCHASE_ORDER_EXTEND_INFO:
			case TargetListCondition.DATAKBN_ORDER_EXTEND_INFO:
				ddlValue.Items.Clear();
				ddlFixedPurchasePattern.Items.Clear();
				ddlValue.Items.Add("");
				tbValue1.Text = "";
				if (ddlDataField.SelectedValue.Length != 0)
				{
					var table = (ddlDataKbn.SelectedValue == TargetListCondition.DATAKBN_ORDER_EXTEND_INFO)
						? Constants.TABLE_ORDER
						: Constants.TABLE_FIXEDPURCHASE;
					var selectedValueSettingId = ddlDataField.SelectedValue.Replace(table + ".", "");
					var orderExtendSettingModel =
						DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels.FirstOrDefault(m => m.SettingId == selectedValueSettingId);

					if (orderExtendSettingModel == null) break;

					if (orderExtendSettingModel.IsInputTypeText)
					{
						// 拡張項目の設定方法がテキストボックスの場合
						if (defaultValue != null) tbValue1.Text = defaultValue;

						// 表示調整
						ddlValue.Visible = false;
						tbValue1.Visible = true;
					}
					else
					{
						// 拡張項目の設定方法がテキストボックス以外の場合
						AddItemToDropDownList(ddlValue, OrderExtendCommon.GetListItemForManager(orderExtendSettingModel.InputDefault), defaultValue);

						// 表示調整
						ddlValue.Visible = true;
						tbValue1.Visible = false;
					}
				}
				break;

			default:
				ddlValue.Items.Clear();
				ddlFixedPurchasePattern.Items.Clear();
				tbValue1.Text = "";
				if (ddlDataField.SelectedValue.Length != 0)
				{
					XmlDocument xdDataField = new XmlDocument();
					xdDataField.Load(Server.MapPath(Constants.PATH_ROOT + "/Xml/TargetList/DataField.Xml"));
					XmlNode xnDataFields = xdDataField.SelectSingleNode("/DataField/" + ddlDataKbn.SelectedValue);
					if (xnDataFields != null)
					{
						string strSqlStatementName = null;
						string[] strValueTextKeyPair = null;
						string customSetting = null;
						foreach (XmlNode xnField in xnDataFields.ChildNodes)
						{
							if ((xnField.NodeType != XmlNodeType.Comment)
								&& (ddlDataField.SelectedValue == xnField.Attributes["field"].Value))
							{
								strSqlStatementName = (xnField.Attributes["SqlStatement"] != null)
									? xnField.Attributes["SqlStatement"].Value
									: null;
								strValueTextKeyPair = (xnField.Attributes["ValueText"] != null)
									? xnField.Attributes["ValueText"].Value.Split(':')
									: null;
								customSetting = (xnField.Attributes["CustomSetting"] != null) ? xnField.Attributes["CustomSetting"].Value : null;
								break;
							}
							else if ((xnField.NodeType != XmlNodeType.Comment)
									 && (ddlDataField.SelectedValue.Contains(Constants.TABLE_ORDER + ".extend_status")))
							{
								strValueTextKeyPair = new string[] { Constants.TABLE_ORDER, "extend_status" };
								break;
							}
						}

						// SQLクエリから値選択ドロップダウン作成（「SqlStatement」タグ指定）
						ddlValue.Visible = ((strValueTextKeyPair != null) || (customSetting != null) || (strSqlStatementName != null));
						tbValue1.Visible = ((strValueTextKeyPair == null) && (customSetting == null) && (strSqlStatementName == null));
						if (strSqlStatementName != null)
						{
							DataView dvTableFromDataFieldXml = null;
							using (SqlAccessor sqlAccessor = new SqlAccessor())
							using (SqlStatement sqlStatement = new SqlStatement("TargetList", strSqlStatementName))
							{
								Hashtable htInput = new Hashtable();
								htInput.Add("shop_id", this.LoginOperatorShopId);
								htInput.Add("dept_id", this.LoginOperatorDeptId);
								dvTableFromDataFieldXml = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
							}

							AddItemToDropDownList(
								ddlValue,
								dvTableFromDataFieldXml.Cast<DataRowView>()
									.Select(drv => new ListItem((string)drv["text"], (string)drv["value"])),
								defaultValue);
						}
						// ValueTextから値選択ドロップダウン作成（「ValueText」タグ指定）
						else if (strValueTextKeyPair != null)
						{
							if (ddlDataField.SelectedValue == string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1))
							{
								ddlValue.Visible = false;
								ddlValue.Items.Clear();
								ddlFixedPurchasePattern.Visible = true;
								ddlFixedPurchasePattern.Items.Clear();
								ddlFixedPurchasePattern.Items.Add("");
								AddItemToDropDownList(
									ddlFixedPurchasePattern,
									ValueText.GetValueItemList(strValueTextKeyPair[0], strValueTextKeyPair[1]).Cast<ListItem>(),
									defaultValue);
							}
							else
							{
								ddlValue.Visible = true;
								ddlFixedPurchasePattern.Visible = false;
								ddlFixedPurchasePattern.Items.Clear();
							}

							AddItemToDropDownList(
								ddlValue,
								ValueText.GetValueItemList(strValueTextKeyPair[0], strValueTextKeyPair[1]).Cast<ListItem>(),
								defaultValue,
								ddlDataKbn.SelectedItem.Value,
								ddlDataField.SelectedItem.Value);
						}
						// customSettingからドロップダウン作成
						else if (customSetting != null)
						{
							AddItemToDropDownList(
								ddlValue,
								GetCustomLisItems(customSetting),
								defaultValue);
						}
						// テキストボックス作成（タグ指定なし）
						else
						{
							if (ddlDataField.SelectedValue == string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_CANCEL_REASON_ID))
							{
								ddlValue.Visible = true;
								tbValue1.Visible = false;
								var cancelReasonList = new FixedPurchaseService().GetUsedCancelReasonAll();
								ddlValue.Items.Add("");
								ddlValue.Items.AddRange(
									cancelReasonList.Select(item => new ListItem(item.CancelReasonName, item.CancelReasonId)).ToArray());
								if (defaultValue != null) ddlValue.Text = defaultValue;
								return;
							}
							if (defaultValue != null) tbValue1.Text = defaultValue;
						}
					}
				}
				break;
		}
	}

	/// <summary>
	/// ドロップダウンリストにアイテム追加
	/// </summary>
	/// <param name="ddl">追加先</param>
	/// <param name="listItems">リストアイテム列</param>
	/// <param name="defaultValue">デフォルト値（指定無し：null）</param>
	/// <param name="targetListKbn">Target list kbn</param>
	/// <param name="targetListField">Target list field</param>
	private void AddItemToDropDownList(
		DropDownList ddl,
		IEnumerable<ListItem> listItems,
		string defaultValue,
		string targetListKbn = "",
		string targetListField = "")
	{
		if (listItems.First().Value == Constants.FLG_USER_USER_KBN_PC_USER)
		{
			ddl.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_USER, Constants.VALUE_TEXT_KEY_USER_USER_KBN_ALL));
		}
		foreach (var li in listItems)
		{
			// モバイルデータの表示と非表示OFF時、顧客区分がMB_USERとMB_GEST区分、または注文区分がモバイル注文を追加しない
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
				&& ((li.Value == Constants.FLG_USER_USER_KBN_MOBILE_USER)
					|| (li.Value == Constants.FLG_USER_USER_KBN_MOBILE_GUEST)
					|| (li.Value == Constants.FLG_ORDER_ORDER_KBN_MOBILE))) continue;

			// つくーるAPI連携無効の場合、「つくーる」を追加しない
			if ((Constants.URERU_AD_IMPORT_ENABLED == false)
				&& (li.Value == Constants.FLG_USER_MALL_ID_URERU_AD)) continue;
			
			if (((Constants.REALSHOP_OPTION_ENABLED == false)
					|| (Constants.STORE_PICKUP_OPTION_ENABLED == false))
				&& (targetListKbn == TargetListCondition.DATAKBN_ORDER_INFO)
				&& (targetListField == string.Format(
					"{0}.{1}",
					Constants.TABLE_ORDERSHIPPING,
					Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG))
				&& (li.Value == Constants.FLG_ORDERSHIPPING_SHIPPING_STORE_PICKUP_FLG)) continue;

			if (defaultValue != null) li.Selected = (li.Value == defaultValue);
			ddl.Items.Add(li);
		}
	}

	/// <summary>
	/// カスタムListItem取得
	/// </summary>
	/// <param name="customSetting">カスタムアイテム</param>
	/// <returns>作成したカスタムListItem</returns>
	private IEnumerable<ListItem> GetCustomLisItems(string customSetting)
	{
		var unassigned = ReplaceTag("@@DispText.common_message.unassigned@@");
		switch (customSetting)
		{
			case "w2_UserAttribute:cpm_cluster_id":
				return (new ListItem[] { new ListItem(unassigned, string.Empty), }).Concat(
					Constants.CPM_CLUSTER_SETTINGS.Settings1.SelectMany(s =>
						Constants.CPM_CLUSTER_SETTINGS.Settings2.Select(si =>
							new ListItem(s.Name + Constants.CPM_CLUSTER_SETTINGS.ClusterNameSeparator + si.Name, s.Id + si.Id))));

			case "w2_UserAttribute:cpm_cluster_attribute1":
				return (new ListItem[] { new ListItem(unassigned, string.Empty), }).Concat(
					Constants.CPM_CLUSTER_SETTINGS.Settings1.Select(s => new ListItem(s.Name, s.Id)));

			case "w2_UserAttribute:cpm_cluster_attribute2":
				return (new ListItem[] { new ListItem(unassigned, string.Empty), }).Concat(
				Constants.CPM_CLUSTER_SETTINGS.Settings2.Select(s => new ListItem(s.Name, s.Id)));

			case "w2_User:access_country_iso_code":
			case "w2_OrderOwner:access_country_iso_code":
				var countryListItems = (new ListItem[] { new ListItem(unassigned, string.Empty), }).Concat(
					Constants.GLOBAL_CONFIGS.GlobalSettings.CountryIsoCodes.Select(cic => new ListItem(cic, cic)));
				return countryListItems;

			case "w2_User:disp_language_code":
			case "w2_OrderOwner:disp_language_code":
				var languageListItems = (new ListItem[] { new ListItem(unassigned, string.Empty), }).Concat(
					Constants.GLOBAL_CONFIGS.GlobalSettings.Languages.Select(l => new ListItem(l.Code, l.Code)));
				return languageListItems;

			case "w2_User:disp_currency_code":
			case "w2_OrderOwner:disp_currency_code":
				var currencyListItems = (new ListItem[] { new ListItem(unassigned, string.Empty), }).Concat(
					Constants.GLOBAL_CONFIGS.GlobalSettings.Currencies.Select(c => new ListItem(c.Code, c.Code)));
				return currencyListItems;
		}
		throw new Exception("リストが作成できませんでした。customSetting:" + customSetting);
	}

	/// <summary>
	/// イコールサイン ドロップダウン初期化
	/// </summary>
	/// <param name="riTarget">対象リピータアイテム</param>
	/// <param name="strDefaultValue">デフォルト選択値</param>
	private void InitializeEqualSignComponent(RepeaterItem riTarget, string strDefaultValue)
	{
		DropDownList ddlDataKbn = ((DropDownList)riTarget.FindControl("ddlDataKbn"));
		DropDownList ddlDataField = ((DropDownList)riTarget.FindControl("ddlDataField"));
		HiddenField hfDataType = ((HiddenField)riTarget.FindControl("hfDataType"));
		//TextBox tbValue1 = ((TextBox)riTarget.FindControl("tbValue1"));
		//DropDownList ddlValue = ((DropDownList)riTarget.FindControl("ddlValue"));
		DropDownList ddlEqualSign = ((DropDownList)riTarget.FindControl("ddlEqualSign"));

		// いったん削除
		ddlEqualSign.Items.Clear();

		// データフィールドが未選択の場合非表示となるため処理スキップ
		if (ddlDataField.SelectedValue.Length == 0) return;

		if ((ddlDataField.SelectedValue == string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1))
			&& (((DropDownList)riTarget.FindControl("ddlFixedPurchasePattern")).SelectedValue.Length == 0)) return;

		if (ddlDataField.SelectedValue.Length != 0)
		{
			XmlDocument xdDataField = new XmlDocument();
			xdDataField.Load(AppDomain.CurrentDomain.BaseDirectory + "/Xml/TargetList/DataField.Xml");
			XmlNode xnDataFields = xdDataField.SelectSingleNode("/DataField/" + ddlDataKbn.SelectedValue);
			if (xnDataFields != null)
			{
				hfDataType.Value = "string";	// デフォルト
				foreach (XmlNode xnField in xnDataFields.ChildNodes)
				{
					if ((xnField.NodeType != XmlNodeType.Comment)
						&& (ddlDataField.SelectedValue == xnField.Attributes["field"].Value))
					{
						hfDataType.Value = xnField.Attributes["type"].Value;
						break;
					}
					else if ((xnField.NodeType != XmlNodeType.Comment)
						&& (ddlDataField.SelectedValue.Contains(Constants.TABLE_ORDER + ".extend_status")))
					{
						hfDataType.Value = "select";
						break;
					}
				}
				SetEqualSignDropDownList(hfDataType, ddlEqualSign);
			}
			else if (ddlDataKbn.SelectedValue == TargetListCondition.DATAKBN_USER_EXTEND_INFO)
			{
				string selectedValueSettingId = ddlDataField.SelectedValue.Replace(Constants.TABLE_USEREXTEND + ".", ""); // DDLなどのvalueはTBL名も含まれているのでSettingIdと一致するよう整形
				var userExtendSetting = m_userExtendSettingList.Items.Find(item => (item.SettingId == selectedValueSettingId)); // 現在選択中の項目と一致する設定を取得

				if (userExtendSetting != null)
				{
					// ユーザ拡張項目の設定内容を元に、ターゲット設定時に設定する形へ変換する
					string equalSignType;
					switch (userExtendSetting.InputType)
					{
						case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
						case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
							equalSignType = "select";
							break;
						case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
							equalSignType = "check";
							break;
						default:
							equalSignType = "string";
							break;
					}

					// 実際に画面に表示、設定可能になる設定種別
					hfDataType.Value = equalSignType;
				}

				SetEqualSignDropDownList(hfDataType, ddlEqualSign);
			}
			else if ((ddlDataKbn.SelectedValue == TargetListCondition.DATAKBN_ORDER_EXTEND_INFO)
				|| (ddlDataKbn.SelectedValue == TargetListCondition.DATAKBN_FIXEDPURCHASE_ORDER_EXTEND_INFO))
			{
				var table = (ddlDataKbn.SelectedValue == TargetListCondition.DATAKBN_ORDER_EXTEND_INFO)
					? Constants.TABLE_ORDER
					: Constants.TABLE_FIXEDPURCHASE;
				var selectedValueSettingId = ddlDataField.SelectedValue.Replace(table + ".", ""); // DDLなどのvalueはTBL名も含まれているのでSettingIdと一致するよう整形
				var orderExtendSettingModel = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels.FirstOrDefault(item => (item.SettingId == selectedValueSettingId)); // 現在選択中の項目と一致する設定を取得

				if (orderExtendSettingModel != null)
				{
					string equalSignType;
					switch (orderExtendSettingModel.InputType)
					{
						case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
						case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_RADIO:
							equalSignType = "select";
							break;
						case Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
							equalSignType = "check";
							break;
						default:
							equalSignType = "string";
							break;
					}
					hfDataType.Value = equalSignType;
				}

				SetEqualSignDropDownList(hfDataType, ddlEqualSign);
			}

			// デフォルト値セット
			if (strDefaultValue.Length != 0)
			{
				foreach (ListItem li in ddlEqualSign.Items)
				{
					li.Selected = (li.Value == strDefaultValue);
				}
			}
		}
	}

	/// <summary>
	/// イコールサインのDDLを生成する
	/// </summary>
	/// <param name="hfDataType">データタイプ</param>
	/// <param name="ddlEqualSign">等しい、等しくない条件</param>
	private static void SetEqualSignDropDownList(HiddenField hfDataType, DropDownList ddlEqualSign)
	{

		switch (hfDataType.Value)
		{
			case "select":
			case "fixedpurchase":
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_SELECT_EQUAL), TargetListCondition.EQUALSIGN_SELECT_EQUAL));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_SELECT_NOT_EQUAL), TargetListCondition.EQUALSIGN_SELECT_NOT_EQUAL));
				break;

			case "check":
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_CHECK_EQUAL), TargetListCondition.EQUALSIGN_CHECK_EQUAL));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_CHECK_NOT_EQUAL), TargetListCondition.EQUALSIGN_CHECK_NOT_EQUAL));
				break;

			case "string":
			case "birth":
			case "favorite":
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_STRING_EQUAL), TargetListCondition.EQUALSIGN_STRING_EQUAL));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_STRING_NOT_EQUAL), TargetListCondition.EQUALSIGN_STRING_NOT_EQUAL));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_STRING_CONTAIN), TargetListCondition.EQUALSIGN_STRING_CONTAIN));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_STRING_NOT_CONTAIN), TargetListCondition.EQUALSIGN_STRING_NOT_CONTAIN));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_STRING_BEGIN_WITH), TargetListCondition.EQUALSIGN_STRING_BEGIN_WITH));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_STRING_END_WITH), TargetListCondition.EQUALSIGN_STRING_END_WITH));
				break;

			case "number":
			case "price":
			case TargetListCondition.DATATYPE_COUNT:
			case TargetListCondition.DATATYPE_SUM:
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_NUMBER_EQUAL), TargetListCondition.EQUALSIGN_NUMBER_EQUAL));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_NUMBER_BIGGER_THAN), TargetListCondition.EQUALSIGN_NUMBER_BIGGER_THAN));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_NUMBER_SMALLER_THAN), TargetListCondition.EQUALSIGN_NUMBER_SMALLER_THAN));
				break;

			case "datetime":
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DATE_EQUAL), TargetListCondition.EQUALSIGN_DATE_EQUAL));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DATE_BEFORE_THAN), TargetListCondition.EQUALSIGN_DATE_BEFORE_THAN));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DATE_AFTER_THAN), TargetListCondition.EQUALSIGN_DATE_AFTER_THAN));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DATE_DAY_BEFORE), TargetListCondition.EQUALSIGN_DATE_DAY_BEFORE));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DATE_MORE_THAN_DAY), TargetListCondition.EQUALSIGN_DATE_MORE_THAN_DAY));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DATE_LESS_THAN_DAY), TargetListCondition.EQUALSIGN_DATE_LESS_THAN_DAY));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DATE_DAY_AFTER), TargetListCondition.EQUALSIGN_DATE_DAY_AFTER));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DATE_DAY_AFTER_PAST), TargetListCondition.EQUALSIGN_DATE_DAY_AFTER_PAST));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DATE_DAY_BEFORE_FUTURE), TargetListCondition.EQUALSIGN_DATE_DAY_BEFORE_FUTURE));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DATE_LESS_THAN_WEEK), TargetListCondition.EQUALSIGN_DATE_LESS_THAN_WEEK));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DATE_LESS_THAN_MONTH), TargetListCondition.EQUALSIGN_DATE_LESS_THAN_MONTH));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DATE_LESS_THAN_YEAR), TargetListCondition.EQUALSIGN_DATE_LESS_THAN_YEAR));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DATE_DAY_NULL), TargetListCondition.EQUALSIGN_DATE_DAY_NULL));
				break;

			case "dayafter":
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DAYAFTER_EQUAL), TargetListCondition.EQUALSIGN_DAYAFTER_EQUAL));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DAYAFTER_BIGGER_THAN), TargetListCondition.EQUALSIGN_DAYAFTER_BIGGER_THAN));
				ddlEqualSign.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "EqualSign", TargetListCondition.EQUALSIGN_DAYAFTER_SMALLER_THAN), TargetListCondition.EQUALSIGN_DAYAFTER_SMALLER_THAN));
				break;
		}
	}

	/// <summary>
	/// 注文ありなし ドロップダウン初期化
	/// </summary>
	/// <param name="target">対象リピータアイテム</param>
	/// <param name="defaultValue">デフォルト選択値</param>
	private void InitializeOrderExist(RepeaterItem target, string defaultValue)
	{
		DropDownList ddlOrderExist = ((DropDownList)target.FindControl("ddlOrderExist"));

		// いったん削除
		ddlOrderExist.Items.Clear();

		// データフィールドが未選択の場合非表示となるため処理スキップ
		if (((DropDownList)target.FindControl("ddlDataField")).SelectedValue.Length == 0) return;

		if (((DropDownList)target.FindControl("ddlDataKbn")).SelectedValue == TargetListCondition.DATAKBN_ORDER_INFO)
		{
			switch (((HiddenField)target.FindControl("hfDataType")).Value)
			{
				case "select":
				case "string":
				case "birth":
				case "number":
				case "price":
				case "dayafter":
				case "fixedpurchase":
				case TargetListCondition.DATATYPE_COUNT:
				case TargetListCondition.DATATYPE_SUM:
					ddlOrderExist.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "OrderExist", TargetListCondition.ORDEREXIST_EXIST), TargetListCondition.ORDEREXIST_EXIST));
					ddlOrderExist.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "OrderExist", TargetListCondition.ORDEREXIST_NOTEXIST), TargetListCondition.ORDEREXIST_NOTEXIST));
					break;

				case "datetime":
					ddlOrderExist.Items.Add(new ListItem("の" + ValueText.GetValueText("w2_TargetList_TargetInfo", "OrderExist", TargetListCondition.ORDEREXIST_EXIST), TargetListCondition.ORDEREXIST_EXIST));
					ddlOrderExist.Items.Add(new ListItem("の" + ValueText.GetValueText("w2_TargetList_TargetInfo", "OrderExist", TargetListCondition.ORDEREXIST_NOTEXIST), TargetListCondition.ORDEREXIST_NOTEXIST));
					break;

				default:
					break;
			}

			ddlOrderExist.Visible = true;

			// 値セット
			if (defaultValue.Length != 0)
			{
				foreach (ListItem li in ddlOrderExist.Items)
				{
					li.Selected = (li.Value == defaultValue);
				}
			}
		}
		else
		{
			ddlOrderExist.Visible = false;
		}
	}

	/// <summary>
	/// 定期注文ありなし ドロップダウン初期化
	/// </summary>
	/// <param name="target">対象リピータアイテム</param>
	/// <param name="defaultValue">デフォルト選択値</param>
	private void InitializeFixedPurchaseOrderExist(RepeaterItem target, string defaultValue)
	{
		DropDownList ddlFixedPurchaseOrderExist = ((DropDownList)target.FindControl("ddlFixedPurchaseOrderExist"));

		// いったん削除
		ddlFixedPurchaseOrderExist.Items.Clear();

		// データフィールドが未選択の場合非表示となるため処理スキップ
		if (((DropDownList)target.FindControl("ddlDataField")).SelectedValue.Length == 0) return;

		if (((DropDownList)target.FindControl("ddlDataKbn")).SelectedValue == TargetListCondition.DATAKBN_FIXEDPURCHASE_INFO)
		{
			switch (((HiddenField)target.FindControl("hfDataType")).Value)
			{
				case "select":
				case "string":
				case "birth":
				case "number":
				case "price":
				case "dayafter":
				case "fixedpurchase":
				case TargetListCondition.DATATYPE_COUNT:
				case TargetListCondition.DATATYPE_SUM:
					ddlFixedPurchaseOrderExist.Items.Add(
						new ListItem(
							ValueText.GetValueText(
								"w2_TargetList_TargetInfo",
								"FixedPurchaseOrderExist",
								TargetListCondition.FIXEDPURCHASEORDEREXIST_EXIST),
							TargetListCondition.FIXEDPURCHASEORDEREXIST_EXIST));
					ddlFixedPurchaseOrderExist.Items.Add(
						new ListItem(
							ValueText.GetValueText(
								"w2_TargetList_TargetInfo",
								"FixedPurchaseOrderExist",
								TargetListCondition.FIXEDPURCHASEORDEREXIST_NOTEXIST),
							TargetListCondition.FIXEDPURCHASEORDEREXIST_NOTEXIST));
					break;

				case "datetime":
					ddlFixedPurchaseOrderExist.Items.Add(
						new ListItem(
							"の" + ValueText.GetValueText(
								"w2_TargetList_TargetInfo",
								"FixedPurchaseOrderExist",
								TargetListCondition.FIXEDPURCHASEORDEREXIST_EXIST),
							TargetListCondition.FIXEDPURCHASEORDEREXIST_EXIST));
					ddlFixedPurchaseOrderExist.Items.Add(
						new ListItem(
							"の" + ValueText.GetValueText(
								"w2_TargetList_TargetInfo",
								"FixedPurchaseOrderExist",
								TargetListCondition.FIXEDPURCHASEORDEREXIST_NOTEXIST),
							TargetListCondition.FIXEDPURCHASEORDEREXIST_NOTEXIST));
					break;

				default:
					break;
			}

			ddlFixedPurchaseOrderExist.Visible = true;

			// 値セット
			if (string.IsNullOrEmpty(defaultValue) == false)
			{
				foreach (ListItem li in ddlFixedPurchaseOrderExist.Items)
				{
					li.Selected = (li.Value == defaultValue);
				}
			}

			if ((((DropDownList)target.FindControl("ddlDataField")).SelectedValue == string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1))
				&& (((DropDownList)target.FindControl("ddlFixedPurchasePattern")).SelectedValue.Length == 0))
			{
				ddlFixedPurchaseOrderExist.Visible = false;
				ddlFixedPurchaseOrderExist.Items.Clear();
			}
		}
		else
		{
			ddlFixedPurchaseOrderExist.Visible = false;
		}
	}

	/// <summary>
	/// ポイント有りなしドロップダウン初期化
	/// </summary>
	/// <param name="target">対象リピータアイテム</param>
	/// <param name="defaultValue">デフォルト選択値</param>
	private void InitializePointExist(RepeaterItem target, string defaultValue)
	{
		var ddlPointExist = ((DropDownList)target.FindControl("ddlPointExist"));
		var ddlDataField = ((DropDownList)target.FindControl("ddlDataField"));

		// いったん削除
		ddlPointExist.Items.Clear();

		// ドロップダウンが非表示になるパターン
		if ((((DropDownList)target.FindControl("ddlDataKbn")).SelectedValue != TargetListCondition.DATAKBN_LIMITED_TERM_POINT_INFO
			|| (ddlDataField.SelectedValue.Length == 0)))
		{
			ddlPointExist.Visible = false;
			return;
		}

		ddlPointExist.Visible = true;
		if (new[] { "LimitedTermPoint.effective_date", "LimitedTermPoint.point_exp" }.Contains(ddlDataField.SelectedValue) == false)
		{
			ddlPointExist.Items.AddRange(ValueText.GetValueItemArray("w2_TargetList_TargetInfo", "PointExist"));
		}
		else
		{
			ddlPointExist.Items.AddRange(ValueText.GetValueItemArray("w2_TargetList_TargetInfo", "PointExist")
				.Select(item => new ListItem(("の" + item.Text), item.Value))
				.ToArray());
		}

		// 値セット
		if (string.IsNullOrEmpty(defaultValue) == false)
		{
			foreach (ListItem li in ddlPointExist.Items)
			{
				li.Selected = (li.Value == defaultValue);
			}
		}
	}

	/// <summary>
	/// DM発送履歴情報ありなしドロップダウン初期化
	/// </summary>
	/// <param name="target">対象リピータアイテム</param>
	/// <param name="defaultValue">デフォルト選択値</param>
	private void InitializeDmShippingHistory(RepeaterItem target, string defaultValue)
	{
		var ddlDmShippingHistory = ((DropDownList)target.FindControl("ddlDmShippingHistory"));

		// いったん削除
		ddlDmShippingHistory.Items.Clear();

		// データフィールドが未選択の場合非表示となるため処理スキップ
		if (((DropDownList)target.FindControl("ddlDataField")).SelectedValue.Length == 0) return;

		if (((DropDownList)target.FindControl("ddlDataKbn")).SelectedValue == TargetListCondition.DATAKBN_DM_SHIPPING_HISTORY_INFO)
		{
			switch (((HiddenField)target.FindControl("hfDataType")).Value)
			{
				case "select":
				case "string":
				case "birth":
				case "number":
				case "price":
				case "dayafter":
				case TargetListCondition.DATATYPE_COUNT:
				case TargetListCondition.DATATYPE_SUM:
					ddlDmShippingHistory.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "DmShippingHistoryExist", TargetListCondition.DMSHIPPINGHISTORYINFO_EXIST), TargetListCondition.DMSHIPPINGHISTORYINFO_EXIST));
					ddlDmShippingHistory.Items.Add(new ListItem(ValueText.GetValueText("w2_TargetList_TargetInfo", "DmShippingHistoryExist", TargetListCondition.DMSHIPPINGHISTORYINFO_NOTEXIST), TargetListCondition.DMSHIPPINGHISTORYINFO_NOTEXIST));
					break;

				case "datetime":
					ddlDmShippingHistory.Items.Add(new ListItem("の" + ValueText.GetValueText("w2_TargetList_TargetInfo", "DmShippingHistoryExist", TargetListCondition.DMSHIPPINGHISTORYINFO_EXIST), TargetListCondition.DMSHIPPINGHISTORYINFO_EXIST));
					ddlDmShippingHistory.Items.Add(new ListItem("の" + ValueText.GetValueText("w2_TargetList_TargetInfo", "DmShippingHistoryExist", TargetListCondition.DMSHIPPINGHISTORYINFO_NOTEXIST), TargetListCondition.DMSHIPPINGHISTORYINFO_NOTEXIST));
					break;

				default:
					break;
			}

			ddlDmShippingHistory.Visible = true;

			// 値セット
			if (defaultValue.Length != 0)
			{
				foreach (ListItem li in ddlDmShippingHistory.Items)
				{
					li.Selected = (li.Value == defaultValue);
				}
			}
		}
		else
		{
			ddlDmShippingHistory.Visible = false;
		}
	}
	/// <summary>
	/// お気に入り商品ありなし ドロップダウン初期化
	/// </summary>
	/// <param name="target">対象リピータアイテム</param>
	/// <param name="defaultValue">デフォルト選択値</param>
	private void InitializeFavoriteExist(RepeaterItem target, string defaultValue)
	{
		DropDownList ddlFavoriteExist = ((DropDownList)target.FindControl("ddlFavoriteExist"));

		// いったん削除
		ddlFavoriteExist.Items.Clear();

		// データフィールドが未選択の場合非表示となるため処理スキップ
		if (((DropDownList)target.FindControl("ddlDataField")).SelectedValue.Length == 0) return;

		if (((DropDownList)target.FindControl("ddlDataKbn")).SelectedValue != TargetListCondition.DATAKBN_FAVORITE_PRODUCT_INFO)
		{
			ddlFavoriteExist.Visible = false;
			return;
		}
		switch (((HiddenField)target.FindControl("hfDataType")).Value)
		{
			case "favorite":
				ddlFavoriteExist.Items.Add(
					new ListItem(
						ValueText.GetValueText(
							"w2_TargetList_TargetInfo", "FavoriteExist", TargetListCondition.FAVORITEEXIST_EXIST), TargetListCondition.FAVORITEEXIST_EXIST));
				ddlFavoriteExist.Items.Add(
					new ListItem(
						ValueText.GetValueText(
							"w2_TargetList_TargetInfo", "FavoriteExist", TargetListCondition.FAVORITEEXIST_NOTEXIST), TargetListCondition.FAVORITEEXIST_NOTEXIST));
				break;

			default:
				break;
		}

		ddlFavoriteExist.Visible = true;
		// 値セット
		if (defaultValue.Length != 0)
		{
			foreach (ListItem li in ddlFavoriteExist.Items)
			{
				li.Selected = (li.Value == defaultValue);
			}
		}
	}

	/// <summary>
	/// ターゲットリスト条件取得
	/// </summary>
	/// <returns>ターゲットリストデータリスト</returns>
	private TargetListConditionList CreateTargetListConditionList()
	{
		var targetListConditionList = new TargetListConditionList();

		foreach (RepeaterItem groupConditions in rConditions.Items)
		{
			var rGroupConditions = (Repeater)groupConditions.FindControl("rGroupConditions");

			// 条件がグループ化されていなかったら
			ITargetListCondition tlc = null;
			if (rGroupConditions.Items.Count == 1)
			{
				tlc = MakeConditionFromRepeater(rGroupConditions.Items[0]);
			}
			// 条件がグループ化されていたら
			else
			{
				var group = new TargetListConditionGroup();
				foreach (RepeaterItem ri in rGroupConditions.Items)
				{
					group.TargetGroup.Add(MakeConditionFromRepeater(ri));
				}
				tlc = group;
			}
			targetListConditionList.TargetConditionList.Add(tlc);
		}

		return targetListConditionList;
	}

	/// <summary>
	/// リピータの値から条件作成
	/// </summary>
	/// <param name="ri">対象のリピータアイテム</param>
	/// <returns>作成した条件</returns>
	public TargetListCondition MakeConditionFromRepeater(RepeaterItem ri)
	{
		var ddlDataKbn = ((DropDownList)ri.FindControl("ddlDataKbn"));
		var ddlDataField = ((DropDownList)ri.FindControl("ddlDataField"));
		var ddlEqualSign = ((DropDownList)ri.FindControl("ddlEqualSign"));
		var ddlOrderExist = ((DropDownList)ri.FindControl("ddlOrderExist"));
		var ddlFavoriteExist = ((DropDownList)ri.FindControl("ddlFavoriteExist"));
		var ddlFixedPurchaseOrderExist = ((DropDownList)ri.FindControl("ddlFixedPurchaseOrderExist"));
		var ddlPointExist = ((DropDownList)ri.FindControl("ddlPointExist"));
		var ddlDmShippingHistory = ((DropDownList)ri.FindControl("ddlDmShippingHistory"));
		var tbFixedPurchasePattern = ((TextBox)ri.FindControl("tbFixedPurchasePattern"));
		var tbFixedPurchasePatternDetail = ((TextBox)ri.FindControl("tbFixedPurchasePatternDetail"));
		var ddlDayOfWeek = ((DropDownList)ri.FindControl("ddlDayOfWeek"));
		var ddlFixedPurchasePattern = ((DropDownList)ri.FindControl("ddlFixedPurchasePattern")).SelectedValue;

		TargetListCondition tlc = new TargetListCondition();
		tlc.DataKbn = ddlDataKbn.SelectedValue;
		if (ddlDataKbn.SelectedItem != null)
		{
			tlc.DataKbnString = ddlDataKbn.SelectedItem.Text;
		}

		switch (ddlDataKbn.SelectedValue)
		{
			case TargetListCondition.DATAKBN_USER_INFO:
			case TargetListCondition.DATAKBN_USER_EXTEND_INFO:
			case TargetListCondition.DATAKBN_USER_ATTRIBUTE_INFO:
			case TargetListCondition.DATAKBN_ORDER_INFO:
			case TargetListCondition.DATAKBN_ORDER_EXTEND_INFO:
			case TargetListCondition.DATAKBN_FIXEDPURCHASE_ORDER_EXTEND_INFO:
			case TargetListCondition.DATAKBN_POINT_INFO:
			case TargetListCondition.DATAKBN_AGGREGATED_POINT_INFO:
			case TargetListCondition.DATAKBN_LIMITED_TERM_POINT_INFO:
			case TargetListCondition.DATAKBN_CART_INFO:
			case TargetListCondition.DATAKBN_FIXEDPURCHASE_INFO:
			case TargetListCondition.DATAKBN_MAIL_CLICK_INFO:
			case TargetListCondition.DATAKBN_COUPON_INFO:
			case TargetListCondition.DATAKBN_DM_SHIPPING_HISTORY_INFO:
			case TargetListCondition.DATAKBN_ORDER_AGGREGATE:
			case TargetListCondition.DATAKBN_FAVORITE_PRODUCT_INFO:
				tlc.DataField = ddlDataField.SelectedValue;
				if (ddlDataField.SelectedItem != null)
				{
					tlc.DataFieldString = ddlDataField.SelectedItem.Text;
				}
				tlc.DataType = ((HiddenField)ri.FindControl("hfDataType")).Value;

				switch (tlc.DataType)
				{
					case "select":
					case "check":
						tlc.Values.Add(new TargetListCondition.Data(
							(((DropDownList)ri.FindControl("ddlValue")).SelectedItem != null) ? ((DropDownList)ri.FindControl("ddlValue")).SelectedItem.Text : "",
							((DropDownList)ri.FindControl("ddlValue")).SelectedValue));
						break;

					case "string":
					case "birth":
					case "favorite":
						tlc.Values.Add(new TargetListCondition.Data(((TextBox)ri.FindControl("tbValue1")).Text));
						break;

					case "number":
					case "price":
					case TargetListCondition.DATATYPE_COUNT:
					case TargetListCondition.DATATYPE_SUM:
						tlc.Values.Add(new TargetListCondition.Data(((TextBox)ri.FindControl("tbValue1")).Text));
						break;

					case "datetime":
						tlc.Values.Add(new TargetListCondition.Data((ddlEqualSign.SelectedValue == TargetListCondition.EQUALSIGN_DATE_DAY_NULL) ? "" : ((TextBox)ri.FindControl("tbValue1")).Text));
						break;

					case "dayafter":
						tlc.Values.Add(new TargetListCondition.Data(((TextBox)ri.FindControl("tbValue1")).Text));
						break;

					case "fixedpurchase":
						if (ddlDataField.SelectedValue == string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1))
						{
							switch (ddlFixedPurchasePattern)
							{
								case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
									tlc.FixedPurchaseKbn = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE;
									tlc.Values.Add(new TargetListCondition.Data(string.Format("{0},{1}", tbFixedPurchasePattern.Text.Trim(), tbFixedPurchasePatternDetail.Text.Trim())));
									break;

								case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
									tlc.FixedPurchaseKbn = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS;
									tlc.Values.Add(new TargetListCondition.Data(tbFixedPurchasePatternDetail.Text.Trim()));
									break;

								case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
									tlc.Values.Add(new TargetListCondition.Data(string.Format("{0},{1},{2}", tbFixedPurchasePattern.Text.Trim(), tbFixedPurchasePatternDetail.Text.Trim(), ddlDayOfWeek.Text)));
									tlc.FixedPurchaseKbn = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY;
									break;

								case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
									tlc.FixedPurchaseKbn = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY;
									tlc.Values.Add(new TargetListCondition.Data(string.Format("{0},{1}", tbFixedPurchasePattern.Text.Trim(), ddlDayOfWeek.Text.Trim())));
									break;
							}
						}
						break;
				}

				tlc.EqualSign = ddlEqualSign.SelectedValue;
				if (ddlEqualSign.SelectedItem != null)
				{
					tlc.EqualSignString = ddlEqualSign.SelectedItem.Text;
				}
				break;

			case TargetListCondition.DATAKBN_SQL_CONDITION:
				tlc.Values.Add(new TargetListCondition.Data(((TextBox)ri.FindControl("tbValue2")).Text));
				break;
		}

		if (ddlDataKbn.SelectedValue == TargetListCondition.DATAKBN_ORDER_INFO)
		{
			tlc.OrderExist = ddlOrderExist.SelectedValue;
			if (ddlOrderExist.SelectedItem != null)
			{
				tlc.OrderExistString = ddlOrderExist.SelectedItem.Text;
			}
		}

		if (ddlDataKbn.SelectedValue == TargetListCondition.DATAKBN_FIXEDPURCHASE_INFO)
		{
			tlc.FixedPurchaseOrderExist = ddlFixedPurchaseOrderExist.SelectedValue;
			if (ddlFixedPurchaseOrderExist.SelectedItem != null)
			{
				tlc.FixedPurchaseOrderExistString = ddlFixedPurchaseOrderExist.SelectedItem.Text;
			}
		}

		if (ddlDataKbn.SelectedValue == TargetListCondition.DATAKBN_DM_SHIPPING_HISTORY_INFO)
		{
			tlc.DmShippingHistoryExist = ddlDmShippingHistory.SelectedValue;
			if (ddlDmShippingHistory.SelectedItem != null)
			{
				tlc.DmShippingHistoryExistString = ddlDmShippingHistory.SelectedItem.Text;
			}
		}

		if (ddlDataKbn.SelectedValue == TargetListCondition.DATAKBN_FAVORITE_PRODUCT_INFO)
		{
			tlc.FavoriteExist = ddlFavoriteExist.SelectedValue;
			if (ddlFavoriteExist.SelectedItem != null)
			{
				tlc.FavoriteExistString = ddlFavoriteExist.SelectedItem.Text.Trim();
			}
		}

		var groupNo = int.Parse(((HiddenField)ri.FindControl("hfGroupNo")).Value);
		if (groupNo != 0)
		{
			tlc.GroupNo = groupNo;
		}

		if (ddlDataKbn.SelectedValue == TargetListCondition.DATAKBN_LIMITED_TERM_POINT_INFO)
		{
			tlc.PointExist = ddlPointExist.SelectedValue;
			if (ddlPointExist.SelectedItem != null)
			{
				tlc.PointExistString = ddlPointExist.SelectedItem.Text;
			}
		}

		tlc.ConditionType = ((HiddenField)ri.FindControl("hfConditionType")).Value;
		if (((HiddenField)ri.FindControl("hfGroupConditionType")).Value != null)
		{
			tlc.GroupConditionType = ((HiddenField)ri.FindControl("hfGroupConditionType")).Value;
		}

		if ((tlc.Values.Count != 0) && ((tlc.Values.Last().Value == Constants.FLG_USER_USER_KBN_ALL_USER)
			|| (tlc.Values.Last().Value == Constants.FLG_USER_USER_KBN_ALL_GUEST)))
		{
			tlc.DataType = "string";
			tlc.EqualSign = (tlc.EqualSign == TargetListCondition.EQUALSIGN_SELECT_EQUAL)
				? TargetListCondition.EQUALSIGN_STRING_CONTAIN
				: TargetListCondition.EQUALSIGN_STRING_NOT_CONTAIN;
		}

		return tlc;
	}

	/// <summary>
	/// 条件リピータデータバインド＆値セット
	/// </summary>
	/// <param name="lTargetListCondition">条件</param>
	private void TargetListConditonBind(TargetListConditionList lTargetListCondition)
	{
		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		rConditions.DataSource = lTargetListCondition.TargetConditionList;
		rConditions.DataBind();

		//------------------------------------------------------
		// 値再セット
		//------------------------------------------------------
		foreach (RepeaterItem groupCondition in rConditions.Items)
		{
			var rGroupConditions = (Repeater)groupCondition.FindControl("rGroupConditions");
			var ddlConditionType = (DropDownList)groupCondition.FindControl("ddlConditionType");

			// 対象の条件
			var targetListCondition = lTargetListCondition.TargetConditionList[groupCondition.ItemIndex];

			if (targetListCondition is TargetListConditionGroup)
			{
				foreach (RepeaterItem riTarget in rGroupConditions.Items)
				{
					var tlc = ((TargetListConditionGroup)targetListCondition).TargetGroup[riTarget.ItemIndex];
					SetCondition(tlc, riTarget, ddlConditionType);
				}
			}
			else
			{
				SetCondition((TargetListCondition)targetListCondition, rGroupConditions.Items[0], ddlConditionType);
			}
		}
	}

	/// <summary>
	/// 表示する条件作成
	/// </summary>
	/// <param name="tlc">条件内容</param>
	/// <param name="riTarget">条件のリピータアイテム</param>
	/// <param name="ddlConditionType">AND、OR条件</param>
	protected void SetCondition(TargetListCondition tlc, RepeaterItem riTarget, DropDownList ddlConditionType)
	{
		var ddlDataKbn = ((DropDownList)riTarget.FindControl("ddlDataKbn"));

		var ddlGroupConditionType = (DropDownList)riTarget.FindControl("ddlGroupConditionType");

		// データ区分セット
		ddlDataKbn.Items.Add(new ListItem(""));
		foreach (ListItem li in ValueText.GetValueItemList("w2_TargetList_TargetInfo", "DataKbn"))
		{
			// ターゲットリストSQL抽出利用有効?
			if ((Constants.MARKETINGPLANNER_TARGETLIST_SQL_CONDITION_ENABLE == false)
				&& (li.Value == TargetListCondition.DATAKBN_SQL_CONDITION))
			{
				continue;
			}

			// 定期購入OP有効?
			if ((Constants.FIXEDPURCHASE_OPTION_ENABLED == false)
				&& (li.Value == TargetListCondition.DATAKBN_FIXEDPURCHASE_INFO))
			{
				continue;
			}

			// クーポンOP有効?
			if ((Constants.MARKETINGPLANNER_COUPON_OPTION_ENABLE == false)
				&& (li.Value == TargetListCondition.DATAKBN_COUPON_INFO))
			{
				continue;
			}

			// DM発送履歴OP有効?
			if ((Constants.DM_SHIPPING_HISTORY_OPTION_ENABLED == false)
				&& (li.Value == TargetListCondition.DATAKBN_DM_SHIPPING_HISTORY_INFO))
			{
				continue;
			}

			// ユーザー属性OPがOFF
			if ((Constants.USER_ATTRIBUTE_OPTION_ENABLE == false)
				&& (li.Value == TargetListCondition.DATAKBN_USER_ATTRIBUTE_INFO))
			{
				continue;
			}

			// 注文拡張項目OPがOFF
			if ((Constants.ORDER_EXTEND_OPTION_ENABLED == false)
				&& ((li.Value == TargetListCondition.DATAKBN_ORDER_EXTEND_INFO)
					|| (li.Value == TargetListCondition.DATAKBN_FIXEDPURCHASE_ORDER_EXTEND_INFO)))
			{
				continue;
			}

			ddlDataKbn.Items.Add(li);
		}

		foreach (ListItem li in ddlDataKbn.Items)
		{
			li.Selected = (li.Value == tlc.DataKbn);
		}

		// 全体のConditionType設定
		foreach (ListItem li in ddlConditionType.Items)
		{
			li.Selected = (li.Value == tlc.ConditionType);
		}

		// グループ化されていればグループのConditionType設定
		if (ddlGroupConditionType != null)
		{
			foreach (ListItem li in ddlGroupConditionType.Items)
			{
				li.Selected = (li.Value == tlc.GroupConditionType);
			}
		}

		// データフィールドセット
		InitializeDataFieldComponent(riTarget, tlc.DataField);

		if ((tlc.Values.Count > 0)
			&& ((tlc.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE)
				|| (tlc.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY)
				|| (tlc.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS)
				|| (tlc.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY)))
		{
			var fixedPurchasePatternText = ValueText.GetValueText(
				Constants.TABLE_FIXEDPURCHASE,
				Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1,
			tlc.FixedPurchaseKbn);
			InitializeFixedPurchasePattern(riTarget, tlc.Values, fixedPurchasePatternText);
		}
		else
		{
			// 値セット
			InitializeValueComponent(riTarget, tlc.Values);
		}

		// イコールサイン初期化
		InitializeEqualSignComponent(riTarget, tlc.EqualSign);

		// 注文ありなしドロップダウン初期化
		InitializeOrderExist(riTarget, tlc.OrderExist);

		// 定期注文ありなしドロップダウン初期化
		InitializeFixedPurchaseOrderExist(riTarget, tlc.FixedPurchaseOrderExist);

		// ポイントありなしドロップダウン初期化
		InitializePointExist(riTarget, tlc.PointExist);

		// DM発送履歴情報ありなしドロップダウン初期化
		InitializeDmShippingHistory(riTarget, tlc.DmShippingHistoryExist);

		// お気に入り商品ありなしドロップダウン初期化
		InitializeFavoriteExist(riTarget, tlc.FavoriteExist);
	}

	/// <summary>
	/// Back button click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		var actionStatus = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ACTION_STATUS]);
		switch (actionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
				Response.Redirect(CreateTargetListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_TARGETLIST_SEARCH_INFO]));
				break;

			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
				Response.Redirect(string.Format("{0}{1}?{2}={3}&{4}={5}",
					Constants.PATH_ROOT,
					Constants.PAGE_W2MP_MANAGER_TARGETLIST_CONFIRM,
					Constants.REQUEST_KEY_TARGET_ID,
					this.m_strTargetId,
					Constants.REQUEST_KEY_ACTION_STATUS,
					Constants.ACTION_STATUS_DETAIL));
				break;
		}
	}

	/// <summary>
	/// 確認ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		StringBuilder sbErrorMessage = new StringBuilder();

		Hashtable htInput = new Hashtable();

		//------------------------------------------------------
		// 値セット
		//------------------------------------------------------
		htInput.Add(Constants.FIELD_TARGETLIST_DEPT_ID, this.LoginOperatorDeptId);
		htInput.Add(Constants.FIELD_TARGETLIST_TARGET_ID, m_strTargetId);
		htInput.Add(Constants.FIELD_TARGETLIST_TARGET_NAME, tbTargetListName.Text);
		htInput.Add(Constants.FIELD_TARGETLIST_TARGET_TYPE, Constants.FLG_TARGETLIST_TARGET_TYPE_MANUAL);	// ターゲットタイプ：手動
		htInput.Add(Constants.FIELD_TARGETLIST_LAST_CHANGED, this.LoginOperatorName);

		TargetListConditionList lTargetListCondition = CreateTargetListConditionList();
		htInput.Add(Constants.FIELD_TARGETLIST_TARGET_CONDITION, TargetListConditionRelationXml.CreateTargetListConditionXml(lTargetListCondition));
		htInput.Add("lTargetListCondition", lTargetListCondition);

		Session[Constants.SESSION_KEY_PARAM] = htInput;

		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		// ターゲットリスト名
		var targetListLineDisplayName = ValueText.GetValueText(
			Constants.TABLE_TARGETLIST,
			Constants.VALUETEXT_PARAM_TARGET_LIST_CONDITION,
			Constants.VALUETEXT_PARAM_TARGET_LIST_CONDITION_TARGET_LIST_NAME);
		var messageTitleFormat = ValueText.GetValueText(
			Constants.TABLE_TARGETLIST,
			Constants.VALUETEXT_PARAM_TARGET_LIST_CONDITION,
			Constants.VALUETEXT_PARAM_TARGET_LIST_CONDITION_MESSAGE_TITLE_FORMAT);
		sbErrorMessage.Append(Validator.CheckNecessaryError(targetListLineDisplayName, tbTargetListName.Text));
		sbErrorMessage.Append(Validator.CheckByteLengthMaxError(targetListLineDisplayName, tbTargetListName.Text, 60));
		sbErrorMessage.Append((sbErrorMessage.Length != 0) ? "<br />" : "");
		int iLoop = 1;
		foreach (var tlcGroup in lTargetListCondition.TargetConditionList)
		{
			var messageTitle = string.Format(messageTitleFormat, iLoop++.ToString());
			if (tlcGroup is TargetListConditionGroup)
			{
				foreach (var tlc in ((TargetListConditionGroup)tlcGroup).TargetGroup)
				{
					// 各行の入力値チェック
					sbErrorMessage.Append(CheckInputParams(tlc, messageTitle));
				}
			}
			else
			{
				// 各行の入力値チェック
				sbErrorMessage.Append(CheckInputParams((TargetListCondition)tlcGroup, messageTitle));
			}

		}

		//------------------------------------------------------
		// スケジュールセット・入力チェック
		//------------------------------------------------------
		sbErrorMessage.Append(ucScheduleRegisterForm.Validate(htInput));

		//------------------------------------------------------
		// エラーがあれば画面遷移
		//------------------------------------------------------
		if (sbErrorMessage.Length != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = sbErrorMessage.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// 確認画面へリダイレクト
		//------------------------------------------------------
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_TARGETLIST_CONFIRM);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(m_strActionKbn);
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_TARGET_ID).Append("=").Append(HttpUtility.UrlEncode(m_strTargetId));

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// 入力値チェック
	/// </summary>
	/// <param name="tlc">1行あたりのターゲットリスト情報</param>
	/// <param name="strLine">N行目</param>
	/// <returns>エラーメッセージ</returns>
	protected string CheckInputParams(TargetListCondition tlc, string strLine)
	{
		StringBuilder sbErrorMessages = new StringBuilder();
		string strErrorMessage = "";

		// データ区分
		strErrorMessage = Validator.CheckNecessaryError(
			strLine + ValueText.GetValueText(
				Constants.TABLE_TARGETLIST,
				Constants.VALUETEXT_PARAM_TARGET_LIST_CONDITION,
				Constants.VALUETEXT_PARAM_TARGET_LIST_CONDITION_DATA_KBN),
			tlc.DataKbnString);
		sbErrorMessages.Append(strErrorMessage).Append(strErrorMessage.Length != 0 ? "<br />" : "");

		// データフィールド
		if (tlc.DataKbn != TargetListCondition.DATAKBN_SQL_CONDITION)
		{
			strErrorMessage = Validator.CheckNecessaryError(
				strLine + ValueText.GetValueText(
					Constants.TABLE_TARGETLIST,
					Constants.VALUETEXT_PARAM_TARGET_LIST_CONDITION,
					Constants.VALUETEXT_PARAM_TARGET_LIST_CONDITION_DATA_FIELD),
				tlc.DataFieldString);
			sbErrorMessages.Append(strErrorMessage).Append(strErrorMessage.Length != 0 ? "<br />" : "");
		}

		if (tlc.DataField == string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_CANCEL_REASON_ID)
			&& (tlc.Values.Count > 0))
		{
			strErrorMessage = Validator.CheckNecessaryError(strLine + tlc.DataFieldString, tlc.Values[0].Value);
		}

		if (tlc.DataField == string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1))
		{
			var textValue = "";

			switch (tlc.FixedPurchaseKbn)
			{
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
					textValue = (tlc.Values[0].Value.Length > 1) ? tlc.Values[0].Value.Replace(",", "") : "";
					strErrorMessage = Validator.CheckNecessaryError(strLine + tlc.DataFieldString, textValue);
					strErrorMessage += Validator.CheckHalfwidthNumberError(strLine + tlc.DataFieldString, textValue);
					break;

				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
					textValue = (tlc.Values[0].Value.Length > 2) ? tlc.Values[0].Value.Replace(",", "") : "";
					strErrorMessage = Validator.CheckNecessaryError(strLine + tlc.DataFieldString, textValue);
					strErrorMessage += Validator.CheckHalfwidthNumberError(strLine + tlc.DataFieldString, textValue);
					break;

				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
					textValue = (tlc.Values[0].Value.Length > 2) ? tlc.Values[0].Value.Replace(",", "") : "";
					strErrorMessage = Validator.CheckNecessaryError(strLine + tlc.DataFieldString, textValue);
					strErrorMessage += Validator.CheckHalfwidthNumberError(strLine + tlc.DataFieldString, textValue);
					break;

				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
					strErrorMessage = Validator.CheckNecessaryError(strLine + tlc.DataFieldString, tlc.Values[0].Value);
					strErrorMessage += Validator.CheckHalfwidthNumberError(strLine + tlc.DataFieldString, tlc.Values[0].Value);
					break;
			}
		}

		// 入力フィールド
		if (tlc.Values.Count > 0)
		{
			switch (tlc.DataKbn)
			{
				case TargetListCondition.DATAKBN_USER_INFO:
				case TargetListCondition.DATAKBN_USER_EXTEND_INFO:
				case TargetListCondition.DATAKBN_USER_ATTRIBUTE_INFO:
				case TargetListCondition.DATAKBN_ORDER_INFO:
				case TargetListCondition.DATAKBN_ORDER_EXTEND_INFO:
				case TargetListCondition.DATAKBN_FIXEDPURCHASE_ORDER_EXTEND_INFO:
				case TargetListCondition.DATAKBN_POINT_INFO:
				case TargetListCondition.DATAKBN_AGGREGATED_POINT_INFO:
				case TargetListCondition.DATAKBN_LIMITED_TERM_POINT_INFO:
				case TargetListCondition.DATAKBN_CART_INFO:
				case TargetListCondition.DATAKBN_FIXEDPURCHASE_INFO:
				case TargetListCondition.DATAKBN_COUPON_INFO:
				case TargetListCondition.DATAKBN_MAIL_CLICK_INFO:
				case TargetListCondition.DATAKBN_DM_SHIPPING_HISTORY_INFO:
				case TargetListCondition.DATAKBN_ORDER_AGGREGATE:
				case TargetListCondition.DATAKBN_FAVORITE_PRODUCT_INFO:
					switch (tlc.DataType)
					{
						case "select":
						case "check":
							// 初期化時に設定済みのため不要 //
							break;

						case "string":
						case "favorite":
							// とくになし //
							break;

						case "birth":
							var tmpString = tlc.Values[0].Value.ToLower();
							strErrorMessage = Validator.CheckNecessaryError(strLine + tlc.DataFieldString, tmpString);
							strErrorMessage += Validator.CheckBirthDataType(strLine + tlc.DataFieldString, tmpString);
							break;

						case "number":
						case TargetListCondition.DATATYPE_COUNT:
						case TargetListCondition.DATATYPE_SUM:
							strErrorMessage = Validator.CheckNecessaryError(strLine + tlc.DataFieldString, tlc.Values[0].Value);
							strErrorMessage += Validator.CheckHalfwidthNumberError(strLine + tlc.DataFieldString, tlc.Values[0].Value);
							strErrorMessage += Validator.CheckByteLengthMaxError(strLine + tlc.DataFieldString, tlc.Values[0].Value, 10);
							break;

						case "price":
							strErrorMessage = Validator.CheckNecessaryError(strLine + tlc.DataFieldString, tlc.Values[0].Value);
							strErrorMessage += Validator.CheckCurrency(strLine + tlc.DataFieldString, tlc.Values[0].Value, Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId);
							break;

						case "datetime":
							if (tlc.EqualSign != TargetListCondition.EQUALSIGN_DATE_DAY_NULL)
							{
								strErrorMessage = Validator.CheckNecessaryError(strLine + tlc.DataFieldString, tlc.Values[0].Value);
							}
							switch (tlc.EqualSign)
							{
								// 日付指定
								case TargetListCondition.EQUALSIGN_DATE_EQUAL:
								case TargetListCondition.EQUALSIGN_DATE_BEFORE_THAN:
								case TargetListCondition.EQUALSIGN_DATE_AFTER_THAN:
									var tmpDate = tlc.Values[0].Value.ToLower();
									strErrorMessage = Validator.CheckNecessaryError(strLine + tlc.DataFieldString, tmpDate);
									strErrorMessage += Validator.CheckDateTimeError(strLine + tlc.DataFieldString, tmpDate);
									break;

								// ～以内指定
								case TargetListCondition.EQUALSIGN_DATE_LESS_THAN_DAY:
								case TargetListCondition.EQUALSIGN_DATE_LESS_THAN_WEEK:
								case TargetListCondition.EQUALSIGN_DATE_LESS_THAN_MONTH:
								case TargetListCondition.EQUALSIGN_DATE_LESS_THAN_YEAR:
								case TargetListCondition.EQUALSIGN_DATE_DAY_BEFORE:
								case TargetListCondition.EQUALSIGN_DATE_DAY_AFTER:
								case TargetListCondition.EQUALSIGN_DATE_DAY_BEFORE_FUTURE:
								case TargetListCondition.EQUALSIGN_DATE_DAY_AFTER_PAST:
									strErrorMessage += Validator.CheckHalfwidthNumberError(strLine + tlc.DataFieldString, tlc.Values[0].Value);
									break;
							}
							break;

						case "dayafter":
							strErrorMessage = Validator.CheckNecessaryError(strLine + tlc.DataFieldString, tlc.Values[0].Value);
							strErrorMessage += Validator.CheckHalfwidthNumberError(strLine + tlc.DataFieldString, tlc.Values[0].Value);
							strErrorMessage += Validator.CheckByteLengthMaxError(strLine + tlc.DataFieldString, tlc.Values[0].Value, 4);
							break;

						case "datetime_point":
							strErrorMessage = Validator.CheckNecessaryError(strLine + tlc.DataFieldString, tlc.Values[0].Value);
							switch (tlc.EqualSign)
							{
								// 日付指定
								case TargetListCondition.EQUALSIGN_DATE_EQUAL:
								case TargetListCondition.EQUALSIGN_DATE_BEFORE_THAN:
								case TargetListCondition.EQUALSIGN_DATE_AFTER_THAN:
									strErrorMessage += Validator.CheckDateExactError(strLine + tlc.DataFieldString, tlc.Values[0].Value, "yyyy/M/d");
									break;

								// ～以内指定
								case TargetListCondition.EQUALSIGN_DATE_LESS_THAN_DAY:
								case TargetListCondition.EQUALSIGN_DATE_LESS_THAN_WEEK:
								case TargetListCondition.EQUALSIGN_DATE_LESS_THAN_MONTH:
								case TargetListCondition.EQUALSIGN_DATE_LESS_THAN_YEAR:
									strErrorMessage += Validator.CheckHalfwidthNumberError(strLine + tlc.DataFieldString, tlc.Values[0].Value);
									break;
							}
							break;
					}
					break;

				case TargetListCondition.DATAKBN_SQL_CONDITION:
					strErrorMessage = Validator.CheckNecessaryError(strLine + tlc.DataFieldString, tlc.Values[0].Value);
					break;
			}
			sbErrorMessages.Append(strErrorMessage).Append(strErrorMessage.Length != 0 ? "<br />" : "");
		}

		// 評価式フィールド
		if (tlc.DataKbn != TargetListCondition.DATAKBN_SQL_CONDITION)
		{
			sbErrorMessages.Append(Validator.CheckNecessaryError(
				strLine + ValueText.GetValueText(
					Constants.TABLE_TARGETLIST,
					Constants.VALUETEXT_PARAM_TARGET_LIST_CONDITION,
					Constants.VALUETEXT_PARAM_TARGET_LIST_CONDITION_EQUAL_SIGN),
				tlc.EqualSignString));
			sbErrorMessages.Append((StringUtility.ToEmpty(tlc.EqualSignString).Length == 0) ? "<br />" : "");
		}

		return sbErrorMessages.ToString();
	}

	/// <summary>
	/// ターゲットリストテンプレートID変更時(ターゲットリストテンプレート一覧画面でテンプレート選択後にjavascript経由で実行
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void hfTargetListTemplateId_ValueChanged(object sender, EventArgs e)
	{
		// ターゲットリストテンプレートIDがない場合は処理中止
		if (string.IsNullOrEmpty(hfTargetListTemplateId.Value)) return;

		var input = new Hashtable
		{
			{Constants.FIELD_TARGETLIST_DEPT_ID, this.LoginOperatorDeptId},
			{Constants.FIELD_TARGETLIST_TARGET_ID, m_strTargetId},
			{Constants.FIELD_TARGETLIST_TARGET_NAME, string.Format(
				// 「テンプレート{0}」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_TARGET_LIST_REGISTER,
					Constants.VALUETEXT_PARAM_TARGET_NAME,
					Constants.VALUETEXT_PARAM_TEAMPLATE),
				hfTargetListTemplateId.Value) },
			{Constants.FIELD_TARGETLIST_LAST_CHANGED, this.LoginOperatorName},
			{"lTargetListCondition", TargetListTemplate.GetTemplateList().First(template => (template.TemplateId == hfTargetListTemplateId.Value)).TemplateConditionList}
		};

		// 入力された抽出タイミングの情報を設定 (編集途中のデータのため、入力チェック結果は無視する)
		ucScheduleRegisterForm.Validate(input);
		Session[Constants.SESSION_KEY_PARAM] = input;

		var actionStatus = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ACTION_STATUS]);

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_TARGETLIST_REGISTER)
			.AddParam(Constants.REQUEST_KEY_TARGET_ID, this.m_strTargetId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, actionStatus)
			.CreateUrl();

		// テンプレートを適用した条件を表示
		Response.Redirect(url);
	}

	/// <summary>
	/// グループ化ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnMakeGroup_Click(object sender, EventArgs e)
	{
		var targetListConditionList = CreateTargetListConditionList();

		// 付与するグループNo
		var maxGroupNo = 0;
		foreach (var conditions in targetListConditionList.TargetConditionList)
		{
			if (maxGroupNo < conditions.GetGroupNo(conditions))
			{
				maxGroupNo = conditions.GetGroupNo(conditions);
			}
		}
		maxGroupNo++;

		// グループ化する条件を格納するList
		var concatList = new TargetListConditionGroup();

		// 削除する条件を格納しておくList
		var removeList = new TargetListConditionList();

		// 追加する場所を保持しておくためのIndex
		// 初めのIndexは0なので-1を格納しておく
		var groupIndex = -1;

		// グループ文回す
		foreach (RepeaterItem groupConditions in rConditions.Items)
		{
			Repeater rGroupConditions = (Repeater)groupConditions.FindControl("rGroupConditions");

			foreach (RepeaterItem conditionsItem in rGroupConditions.Items)
			{
				var condition = targetListConditionList.TargetConditionList[groupConditions.ItemIndex];
				// グループ条件にはチェックボックスがないのでグループ化されていない条件を取得
				if (condition is TargetListCondition)
				{
					var tlc = (TargetListCondition)condition;

					var checkBox = (CheckBox)conditionsItem.FindControl("cbGroupItem");
					if (checkBox.Checked)
					{
						// チェックされている最初のIndexのみ格納
						if (groupIndex == -1)
						{
							groupIndex = groupConditions.ItemIndex;
						}
						tlc.GroupNo = maxGroupNo;

						// 全体のコンディションタイプとは違うコンディションタイプを格納
						tlc.GroupConditionType = GetOppositeConditionType(tlc.ConditionType);

						concatList.TargetGroup.Add(tlc);
						removeList.TargetConditionList.Add(condition);
					}
				}
			}
		}

		// グループ化した条件追加
		targetListConditionList.TargetConditionList.Insert(groupIndex, concatList);

		// グループ化した元の条件削除
		foreach (var removeCondition in removeList.TargetConditionList)
		{
			targetListConditionList.TargetConditionList.Remove(removeCondition);
		}

		TargetListConditonBind(targetListConditionList);
	}

	/// <summary>
	/// グループ外のAND、ORドロップダウンリスト変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlAllCondition_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var selectedValue = ((DropDownList)sender).SelectedValue;

		var targetListCondition = CreateTargetListConditionList();

		foreach (var tlc in targetListCondition.TargetConditionList)
		{
			tlc.ChangeConditionType(tlc, selectedValue);
		}
		TargetListConditonBind(targetListCondition);
	}

	/// <summary>
	/// グループ内のAND、ORドロップダウンリスト変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlGroupConditionType_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		// rConditionsのIndex
		var conditionListIndex = ((RepeaterItem)((DropDownList)sender).Parent.Parent.Parent.Parent).ItemIndex;
		// rGroupConditionsのIndex
		var groupIndex = ((RepeaterItem)((DropDownList)sender).Parent.Parent).ItemIndex;

		var selectedValue = ((DropDownList)sender).SelectedValue;

		var targetListCondition = CreateTargetListConditionList();
		var targetGroup = (TargetListConditionGroup)targetListCondition.TargetConditionList[conditionListIndex];
		var groupNo = targetGroup.TargetGroup[groupIndex].GroupNo;

		foreach (var conditions in targetGroup.TargetGroup)
		{
			if (conditions.GroupNo == groupNo)
			{
				conditions.GroupConditionType = selectedValue;
				conditions.ConditionType = GetOppositeConditionType(selectedValue);
			}
		}

		TargetListConditonBind(targetListCondition);
	}

	/// <summary>
	/// グループ解除ボタン押下時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCancelGroup_OnClick(object sender, EventArgs e)
	{
		// rConditionsのIndex
		var conditionListIndex = ((RepeaterItem)((Button)sender).Parent.Parent.Parent.Parent).ItemIndex;

		var targetListCondition = CreateTargetListConditionList();

		var conditionGroup = (TargetListConditionGroup)targetListCondition.TargetConditionList[conditionListIndex];

		var insertIndex = 0;
		foreach (var condition in conditionGroup.TargetGroup)
		{
			condition.GroupNo = 0;
			condition.GroupConditionType = null;
			targetListCondition.TargetConditionList.Insert(conditionListIndex + insertIndex, condition);
			insertIndex++;
		}

		targetListCondition.TargetConditionList.Remove(conditionGroup);

		TargetListConditonBind(targetListCondition);
	}

	/// <summary>
	/// 全体のAND、ORとは対照のAND、OR取得
	/// </summary>
	/// <param name="conditionType">全体のコンディションタイプ</param>
	/// <returns>ANDならOR、ORならAND</returns>
	private string GetOppositeConditionType(string conditionType)
	{
		return (conditionType == TargetListCondition.CONDITION_TYPE_AND)
			? TargetListCondition.CONDITION_TYPE_OR
			: TargetListCondition.CONDITION_TYPE_AND;
	}
}
