/*
=========================================================================================================
  Module      : ポイントルールスケジュール実行ページ処理(ActionWindowScheduleExecute.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Domain.Coupon;
using w2.Domain.Point;
using w2.Domain.TaskSchedule;

public partial class Form_Common_ActionWindowScheduleExecute : BasePage
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
			// パラメタ取得
			this.ActionNo = -1;

			var actionProperty = GetActionProperty();

			// 画面設定
			lId.Text = WebSanitizer.HtmlEncode(actionProperty.Id);
			lName.Text = WebSanitizer.HtmlEncode(actionProperty.Name);
		}
		else
		{
			// スケジュールが取得できれば状態取得
			if ((this.ActionNo != -1) || (this.TargetMasterId != null))
			{
				// ステータス表示
				DisplayTaskStatus();
			}
		}
	}

	/// <summary>
	/// アクション属性取得
	/// </summary>
	/// <returns>アクション属性</returns>
	private ActionProperty GetActionProperty()
	{
		switch (this.ActionKbn)
		{
			case Constants.FLG_TASKSCHEDULE_ACTION_KBN_ADD_POINT:
				var pointService = new PointService();
				var pointRuleSchedule = pointService.GetPointRuleSchedule(this.ActionMasterId);
				return new ActionProperty(pointRuleSchedule.PointRuleScheduleId, pointRuleSchedule.PointRuleScheduleName);

			case Constants.FLG_TASKSCHEDULE_ACTION_KBN_PUBLISH_COUPON:
				var couponService = new CouponService();
				var couponSchedule = couponService.GetCouponSchedule(this.ActionMasterId);
				return new ActionProperty(couponSchedule.CouponScheduleId, couponSchedule.CouponScheduleName);


			default:
				return new ActionProperty("", "");
		}
	}

	/// <summary>
	/// タスクステータス表示
	/// </summary>
	protected void DisplayTaskStatus()
	{
		// ステータス表示
		var taskSchedule = new TaskScheduleService().Get(
			this.LoginOperatorShopId
			, this.ActionKbn
			, this.ActionMasterId
			, this.ActionNo);
		if (taskSchedule != null)
		{
			// 準備ステータス
			lbPrepareStatus.Text = ValueText.GetValueText(Constants.TABLE_TASKSCHEDULE, Constants.FIELD_TASKSCHEDULE_PREPARE_STATUS, taskSchedule.PrepareStatus);
			// 実行ステータス
			lbExecuteStatus.Text = ValueText.GetValueText(Constants.TABLE_TASKSCHEDULE, Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS, taskSchedule.ExecuteStatus);
			// 進捗
			lbProgress.Text = taskSchedule.Progress;

			switch (taskSchedule.ExecuteStatus)
			{
				// 終了 or 停止
				case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE:
				case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP:
					// 実行ボタンを有効化
					btnStart.Enabled = true;

					// 停止ボタンを無効化
					btnStop.Enabled = false;
					break;
			}
		}

		// ターゲット抽出ステータス表示
		if (btnTargetStartExtract.Enabled == false)
		{
			var taskScheduleTarget = new TaskScheduleService().Get(this.LoginOperatorShopId, Constants.FLG_TASKSCHEDULE_ACTION_KBN_CREATE_TARGETLIST, this.TargetMasterId, this.TargetActionNo);
			if (taskScheduleTarget != null)
			{
				lbTargetExecuteStatus.Text = "";

				var status = ValueText.GetValueText(Constants.TABLE_TASKSCHEDULE, Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS, taskScheduleTarget.ExecuteStatus);
				var progress = taskScheduleTarget.Progress;

				lbTargetExecuteStatus.Text += taskScheduleTarget.ActionMasterId + "：　" + status;

				if (progress != string.Empty)
				{
					lbTargetExecuteStatus.Text += "(" + progress + ")";
				}

				switch (taskScheduleTarget.ExecuteStatus)
				{
					// 終了 or 停止
					case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE:
					case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP:
						// ターゲット初期化
						this.TargetMasterId = null;
						this.TargetActionNo = -1;

						// 抽出実行ボタンを有効化
						btnTargetStartExtract.Enabled = true;
						break;
				}
			}
		}
	}

	/// <summary>
	/// 実行
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnStart_Click(object sender, EventArgs e)
	{
		lbPrepareStatus.Text = "";
		lbExecuteStatus.Text = "";

		var actionNo = InsertTaskSchedule(this.ActionKbn, this.ActionMasterId);

		if (actionNo > 0)
		{
			this.ActionNo = actionNo;
			DisplayTaskStatus();
		}

		// 実行ボタンを無効化
		btnStart.Enabled = false;

		// 停止ボタンを有効化
		btnStop.Enabled = true;
	}

	/// <summary>
	/// 停止
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnStop_Click(object sender, EventArgs e)
	{
		// タスクスケジュール停止
		new TaskScheduleService().SetTaskScheduleStopped(this.LoginOperatorShopId, Constants.FLG_TASKSCHEDULE_ACTION_KBN_ADD_POINT, this.ActionMasterId, this.LoginOperatorName);

		// 停止ボタンを無効化
		btnStop.Enabled = false;
	}

	/// <summary>
	/// 抽出ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnStartExtract_Click(object sender, EventArgs e)
	{
		// 抽出ボタンを無効化
		btnTargetStartExtract.Enabled = false;
		lTargetStartExtract.Visible = false;

		int actionNo;
		switch (this.ActionKbn)
		{
			case Constants.FLG_TASKSCHEDULE_ACTION_KBN_ADD_POINT:
				var pointRuleSchedule = new PointService().GetPointRuleSchedule(this.ActionMasterId);
				actionNo = InsertTaskSchedule(Constants.FLG_TASKSCHEDULE_ACTION_KBN_CREATE_TARGETLIST, pointRuleSchedule.TargetId);
				if (actionNo > 0)
				{
					this.TargetMasterId = pointRuleSchedule.TargetId;
					this.TargetActionNo = actionNo;

					DisplayTaskStatus();
				}
				break;

			case Constants.FLG_TASKSCHEDULE_ACTION_KBN_PUBLISH_COUPON:
				var couponSchedule = new CouponService().GetCouponSchedule(this.ActionMasterId);
				actionNo = InsertTaskSchedule(Constants.FLG_TASKSCHEDULE_ACTION_KBN_CREATE_TARGETLIST, couponSchedule.TargetId);
				if (actionNo > 0)
				{
					this.TargetMasterId = couponSchedule.TargetId;
					this.TargetActionNo = actionNo;

					DisplayTaskStatus();
				}
				break;
		}
	}

	/// <summary>
	/// タスクスケジュール登録
	/// </summary>
	/// <param name="actionKbn">アクション区分</param>
	/// <param name="actionMasterId">アクションマスターID</param>
	/// <returns>アクションNO</returns>
	private int InsertTaskSchedule(string actionKbn, string actionMasterId)
	{
		var model = new TaskScheduleService().InsertTaskScheduleForExecute(this.LoginOperatorDeptId, actionKbn, actionMasterId, this.LoginOperatorName);

		return (model != null) ? model.ActionNo : -1;
	}

	/// <summary>アクション区分</summary>
	protected string ActionKbn
	{
		get { return (string)Request[Constants.REQUEST_KEY_ACTION_KBN] ?? ""; }
	}
	/// <summary>マスターID</summary>
	private string ActionMasterId
	{
		get { return (string)Request[Constants.REQUEST_KEY_MASTER_ID]; }
	}
	/// <summary>アクションNO</summary>
	private int ActionNo
	{
		get { return (int)ViewState["ActionNo"]; }
		set { ViewState["ActionNo"] = value; }
	}
	/// <summary>ターゲットマスターID</summary>
	private string TargetMasterId
	{
		get { return (string)ViewState["TargetMasterId"]; }
		set { ViewState["TargetMasterId"] = value; }
	}
	/// <summary>ターゲットアクションNO</summary>
	private int TargetActionNo
	{
		get { return (int)ViewState["TargetActionNo"]; }
		set { ViewState["TargetActionNo"] = value; }
	}

	/// <summary>
	/// アクション属性
	/// </summary>
	private class ActionProperty
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="id">ID</param>
		/// <param name="name">名称</param>
		public ActionProperty(string id, string name)
		{
			this.Id = id;
			this.Name = name;
		}
		/// <summary>ID</summary>
		public string Id { get; private set; }
		/// <summary>名称</summary>
		public string Name { get; private set; }
	}
}