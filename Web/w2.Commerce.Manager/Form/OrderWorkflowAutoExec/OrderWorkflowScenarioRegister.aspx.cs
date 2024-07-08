/*
=========================================================================================================
  Module      : 受注ワークフローシナリオ登録ページ(OrderWorkflowScenarioRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.Common.Web;
using w2.Domain.FixedPurchaseWorkflowSetting;
using w2.Domain.OrderWorkflowScenarioSetting;
using w2.Domain.OrderWorkflowSetting;

namespace Form.OrderWorkflowAutoExec
{
	/// <summary>
	/// 受注ワークフロー自動実行登録ページ
	/// </summary>
	public partial class OrderWorkflowScenarioSettingRegister : OrderWorkflowPage
	{
		/// <summary>
		/// ページロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				// Reset session if request of referral url is null
				if (this.Request.UrlReferrer == null)
				{
					Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_WORKFLOW_SCENARIO_SETTING] = null;
				}

				Initialize();
			}
		}

		/// <summary>
		/// 初期化
		/// </summary>
		private void Initialize()
		{
			switch (this.ActionStatus)
			{
				case Constants.ACTION_STATUS_INSERT:
					HideButtonForResister();
					if (this.ScenarioSetting != null)
					{
						this.ScenarioSettingItemsForDisplay = this.ScenarioSetting.Items.ToList();
					}
					break;

				case Constants.ACTION_STATUS_COPY_INSERT:
					HideButtonForUpdate();
					SetScenarioSettingToProperty();
					break;

				case Constants.ACTION_STATUS_UPDATE:
					HideButtonForUpdate();
					SetScenarioSettingToProperty();
					break;

				case Constants.ACTION_STATUS_COMPLETE:
					divComp.Visible = true;
					lMessage.Text = (string)Session[Constants.SESSION_KEY_ERROR_MSG];
					HideButtonForUpdate();
					SetScenarioSettingToProperty();
					break;
			}

			DisplayScenarioSetting();
			this.Master.OnLoadEvents += "RefleshScenarioSchedule();";
			this.ScenarioSetting = null;
		}

		/// <summary>
		/// 登録系の際にいらないボタンを隠す
		/// </summary>
		private void HideButtonForResister()
		{
			btnBackTop.Visible = false;
			btnBackBottom.Visible = false;
		}

		/// <summary>
		/// 更新系の時にいらないボタンを隠す
		/// </summary>
		private void HideButtonForUpdate()
		{
			btnBackListTop.Visible = false;
			btnBackListBottom.Visible = false;
		}

		/// <summary>
		/// ScenarioSettingのプロパティにアイテムと分けてセット
		/// </summary>
		private void SetScenarioSettingToProperty()
		{
			if (this.ScenarioSetting == null)
			{
				this.ScenarioSetting = new OrderWorkflowScenarioSettingService().GetWithChild(this.RequestParamOfScenarioSettingId);
			}
			this.ScenarioSettingItemsForDisplay = this.ScenarioSetting.Items.ToList();
		}

		/// <summary>
		/// シナリオ設定を表示する
		/// </summary>
		private void DisplayScenarioSetting()
		{
			if (this.ScenarioSetting != null)
			{
				tbScenariotName.Text = HtmlSanitizer.HtmlEncode(this.ScenarioSetting.ScenarioName);
				cbValidFlg.Checked
					= (this.ScenarioSetting.ValidFlg == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_VALID_FLG_VALID);
			}
			SortOfExecNo();
			CreateScenarioEditor(); // シナリオのリピーターを作成
			SetSchedule();
		}

		/// <summary>
		/// スケジュールをセットする
		/// </summary>
		private void SetSchedule()
		{
			if (this.ScenarioSetting == null) return;

			ucScheduleRegisterForm.ExecKbn = this.ScenarioSetting.ExecTiming;
			if (string.IsNullOrEmpty(this.ScenarioSetting.ScheduleKbn) == false)
			{
				ucScheduleRegisterForm.ScheKbn = this.ScenarioSetting.ScheduleKbn;
			}
			if (string.IsNullOrEmpty(this.ScenarioSetting.ScheduleDayOfWeek) == false)
			{
				ucScheduleRegisterForm.ScheDayOfWeek = this.ScenarioSetting.ScheduleDayOfWeek;
			}
			if (this.ScenarioSetting.ScheduleYear.HasValue)
			{
				ucScheduleRegisterForm.ScheYear = this.ScenarioSetting.ScheduleYear.Value;
			}
			if (this.ScenarioSetting.ScheduleMonth.HasValue)
			{
				ucScheduleRegisterForm.ScheMonth = this.ScenarioSetting.ScheduleMonth.Value;
			}
			if (this.ScenarioSetting.ScheduleDay.HasValue)
			{
				ucScheduleRegisterForm.ScheDay = this.ScenarioSetting.ScheduleDay.Value;
			}
			if (this.ScenarioSetting.ScheduleHour.HasValue)
			{
				ucScheduleRegisterForm.ScheHour = this.ScenarioSetting.ScheduleHour.Value;
			}
			if (this.ScenarioSetting.ScheduleMinute.HasValue)
			{
				ucScheduleRegisterForm.ScheMinute = this.ScenarioSetting.ScheduleMinute.Value;
			}
			if (this.ScenarioSetting.ScheduleSecond.HasValue)
			{
				ucScheduleRegisterForm.ScheSecond = this.ScenarioSetting.ScheduleSecond.Value;
			}
		}

		/// <summary>
		/// 追加クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnAdd_Click(object sender, EventArgs e)
		{
			var scenarioSettingItems = this.ScenarioSettingItemsForDisplay;
			scenarioSettingItems.Add(
				new OrderWorkflowScenarioSettingItemModel
				{
					ScenarioNo = this.ScenarioSettingItemsForDisplay.Last().ScenarioNo + 1
				});
			this.ScenarioSettingItemsForDisplay = scenarioSettingItems;
			SortOfExecNo();
			CreateScenarioEditor();
		}

		/// <summary>
		/// 削除クリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnDelete_Click(object sender, EventArgs e)
		{
			var parentRepeaterItem = ((RepeaterItem)((Button)sender).Parent);

			// シナリオのワークフローを削除
			var itemIndex = parentRepeaterItem.ItemIndex;
			var scenarioSettingItems = this.ScenarioSettingItemsForDisplay;
			scenarioSettingItems.RemoveAt(itemIndex);
			this.ScenarioSettingItemsForDisplay = scenarioSettingItems;

			SortOfExecNo();

			// 削除した番号より先の番号のワークフローNoを変更
			var beforeNo = (int.Parse(((HtmlInputHidden)parentRepeaterItem.FindControl("afterNo")).Value));
			scenarioSettingItems = this.ScenarioSettingItemsForDisplay;
			this.ScenarioSettingItemsForDisplay = scenarioSettingItems.Select(
				scenarioSettingItem =>
				{
					if (scenarioSettingItem.ScenarioNo > beforeNo) scenarioSettingItem.ScenarioNo -= 1;

					return scenarioSettingItem;
				}).ToList();

			CreateScenarioEditor();
		}

		/// <summary>
		/// シナリオのワークフロードロップダウンを変更した時の処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ddlWorkflowList_SelectedIndexChanged(object sender, EventArgs e)
		{
			var scenarioSettingItems = this.ScenarioSettingItemsForDisplay;

			var selectedItem = ((DropDownList)sender).SelectedItem;
			var itemIndex = ((RepeaterItem)((DropDownList)sender).Parent).ItemIndex;
			if (string.IsNullOrEmpty(selectedItem.Value))
			{
				scenarioSettingItems[itemIndex].WorkflowKbn = string.Empty;
				scenarioSettingItems[itemIndex].WorkflowNo = null;
			}
			else
			{
				var selectedValueSplit = selectedItem.Value.Split(':');
				var workflowKbn = selectedValueSplit[0];
				var workflowNo = selectedValueSplit[1];
				scenarioSettingItems[itemIndex].WorkflowKbn = workflowKbn;
				scenarioSettingItems[itemIndex].WorkflowNo = int.Parse(workflowNo);
			}
			this.ScenarioSettingItemsForDisplay = scenarioSettingItems;
			SortOfExecNo();
			CreateScenarioEditor();
		}

		/// <summary>
		/// シナリオを作成する
		/// </summary>
		private void CreateScenarioEditor()
		{
			rScenario.DataSource = this.ScenarioSettingItemsForDisplay;
			rScenario.DataBind();

			// 全てのシナリオのリピーターアイテムでワークフローのドロップダウンを作成
			foreach (RepeaterItem repeaterItem in rScenario.Items)
			{
				var ddlWorkflowList = (DropDownList)repeaterItem.FindControl("ddlWorkflowList");

				// 受注ワークフロー選択ドロップダウンを作成
				ddlWorkflowList.Items.Clear();
				ddlWorkflowList.Items.Add("");
				if (this.ScenarioSettingItemsForDisplay[repeaterItem.ItemIndex].TargetWorkflowKbn == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL)
				{
					var workflows = new OrderWorkflowSettingService().GetForScenarioRegistration();

					foreach (var orderWorkflowSetting in workflows)
					{
						// ddl作成
						var listItem = new ListItem(
							orderWorkflowSetting.WorkflowName,
							string.Format("{0}:{1}", orderWorkflowSetting.WorkflowKbn, orderWorkflowSetting.WorkflowNo));

						// Selectedする処理
						SelectedDllWorkflow(listItem, repeaterItem, orderWorkflowSetting);
						// シナリオが1こなら削除ボタンをhidden
						HiddenDeleteButton(repeaterItem);

						ddlWorkflowList.Items.Add(listItem);
					}
				}
				else
				{
					var workflows = new FixedPurchaseWorkflowSettingService().GetForScenarioRegistration();

					foreach (var orderWorkflowSetting in workflows)
					{
						// ddl作成
						var listItem = new ListItem(
							orderWorkflowSetting.WorkflowName,
							string.Format("{0}:{1}", orderWorkflowSetting.WorkflowKbn, orderWorkflowSetting.WorkflowNo));

						// Selectedする処理
						SelectedDllWorkflow(listItem, repeaterItem, orderWorkflowSetting);
						// シナリオが1こなら削除ボタンをhidden
						HiddenDeleteButton(repeaterItem);

						ddlWorkflowList.Items.Add(listItem);
					}
				}
			}
		}

		/// <summary>
		/// 実行順序のソート
		/// </summary>
		private void SortOfExecNo()
		{
			var scenarioSettingItems = this.ScenarioSettingItemsForDisplay;
			scenarioSettingItems = scenarioSettingItems.Select(
				item =>
				{
					foreach (RepeaterItem ri in rScenario.Items)
					{
						var beforeNo = ((HtmlInputHidden)ri.FindControl("beforeNo")).Value;
						var afterNo = ((HtmlInputHidden)ri.FindControl("afterNo")).Value;
						if (int.Parse(beforeNo) == item.ScenarioNo)
						{
							item.ScenarioNo = int.Parse(afterNo);
							break;
						}
					}
					return item;
				}).ToList();
			this.ScenarioSettingItemsForDisplay = scenarioSettingItems.OrderBy(item => item.ScenarioNo).ToList();
		}

		/// <summary>
		/// シナリオのドロップダウンのワークフローをSelectedにする
		/// </summary>
		/// <param name="listItem">リストアイテム</param>
		/// <param name="riTarget">リピーターアイテム</param>
		/// <param name="orderWorkflowSetting">ワークフロー設定モデル</param>
		private void SelectedDllWorkflow(ListItem listItem, RepeaterItem riTarget, object orderWorkflowSetting)
		{
			var listItemValueSplit = listItem.Value.Split(':');
			var workflowKbn = listItemValueSplit[0];
			var workflowNo = listItemValueSplit[1];
			if ((this.ScenarioSettingItemsForDisplay[riTarget.ItemIndex].WorkflowKbn == workflowKbn)
				&& (this.ScenarioSettingItemsForDisplay[riTarget.ItemIndex].WorkflowNo == int.Parse(workflowNo)))
			{
				listItem.Selected = true;
				var lWorkflowDetails = (Literal)riTarget.FindControl("lWorkflowDetails");
				if (this.ScenarioSettingItemsForDisplay[riTarget.ItemIndex].TargetWorkflowKbn == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL)
				{
					lWorkflowDetails.Text = HtmlSanitizer.HtmlEncodeChangeToBr(((OrderWorkflowSettingModel)orderWorkflowSetting).Desc1);
				}
				else
				{
					lWorkflowDetails.Text = HtmlSanitizer.HtmlEncodeChangeToBr(((FixedPurchaseWorkflowSettingModel)orderWorkflowSetting).Desc1);
				}				
			}
		}

		/// <summary>
		/// 必要に応じてデリートボタンを隠す
		/// </summary>
		/// <param name="riTarget">リピーターアイテム</param>
		private void HiddenDeleteButton(RepeaterItem riTarget)
		{
			if (this.ScenarioSettingItemsForDisplay.Count != 1) return;
			var btnDelete = (Button)riTarget.FindControl("btnDelete");
			btnDelete.Visible = false;
		}

		/// <summary>
		/// 確認ボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnConfirm_Click(object sender, EventArgs e)
		{
			SortOfExecNo();
			CreateScenarioEditor();

			var scenarioSettingInputData = GetScenarioSettingInputData();
			if (DisplayValidationMessage(scenarioSettingInputData.Validate())) return;
			this.ScenarioSetting = scenarioSettingInputData.CreateModel();

			Response.Redirect(
				new UrlCreator(
					Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_CONFIRM)
					.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus).CreateUrl());
		}

		/// <summary>
		/// バリデーションメッセージを表示する
		/// </summary>
		/// <param name="errorMessage">バリデーションメッセージを表示する</param>
		/// <returns>バリデーションメッセージを表示すればtrue</returns>
		private bool DisplayValidationMessage(string errorMessage)
		{
			var isErrorMessage = (errorMessage.Length != 0);
			tbdyOrderOwnerErrorMessages.Visible = isErrorMessage;
			lbErrorMessages.Text = errorMessage;
			return isErrorMessage;
		}

		/// <summary>
		/// シナリオ設定のインプットクラスを取得
		/// </summary>
		/// <returns>シナリオ設定のインプット</returns>
		private OrderWorkflowScenarioSettingInput GetScenarioSettingInputData()
		{
			var scheduleCondition = ucScheduleRegisterForm.GetScheduleCondition();
			var scenarioSettingInput = new OrderWorkflowScenarioSettingInput
			{
				ScenarioSettingId = this.RequestParamOfScenarioSettingId,
				ScenarioName = HtmlSanitizer.HtmlEncode(tbScenariotName.Text.Trim()),
				ExecTiming = scheduleCondition.ExecKbn,
				ScheduleKbn = scheduleCondition.ScheKbn.Trim(),
				ScheduleDayOfWeek = scheduleCondition.ScheDayOfWeek.Trim(),
				ScheduleYear = scheduleCondition.ScheYear,
				ScheduleMonth = scheduleCondition.ScheMonth,
				ScheduleDay = scheduleCondition.ScheDay,
				ScheduleHour = scheduleCondition.ScheHour,
				ScheduleMinute = scheduleCondition.ScheMinute,
				ScheduleSecond = scheduleCondition.ScheSecond,
				ValidFlg = cbValidFlg.Checked
					? Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_VALID_FLG_VALID
					: Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_VALID_FLG_INVALID,
				LastChanged = this.LoginOperatorName,
				Items = GetScenarioSettingInputItemData(),
			};
			return scenarioSettingInput;
		}

		/// <summary>
		/// シナリオ設定のインプットアイテムクラス(複数)を取得
		/// </summary>
		/// <returns>シナリオ設定のインプットアイテムクラス(複数)</returns>
		private OrderWorkflowScenarioSettingItemInput[] GetScenarioSettingInputItemData()
		{
			var scenarioSettingItemInput = rScenario.Items.Cast<RepeaterItem>().ToArray().Select(
				ri =>
				{
					var ddlWorkflowList = ((DropDownList)ri.FindControl("ddlWorkflowList"));
					var workflowKbn = string.Empty;
					var workflowNo = string.Empty;
					if (!string.IsNullOrEmpty(ddlWorkflowList.SelectedValue))
					{
						var selectedValueSplit = ddlWorkflowList.SelectedValue.Split(':');
						workflowKbn = selectedValueSplit[0];
						workflowNo = selectedValueSplit[1];
					}

					return new OrderWorkflowScenarioSettingItemInput
					{
						ScenarioSettingId = this.RequestParamOfScenarioSettingId,
						ScenarioNo = (ri.ItemIndex + 1).ToString(),
						ShopId = Constants.CONST_DEFAULT_SHOP_ID,
						WorkflowKbn = workflowKbn,
						WorkflowNo = workflowNo,
						TargetWorkflowKbn = this.ScenarioSettingItemsForDisplay[ri.ItemIndex].TargetWorkflowKbn,
					};
				});
			return scenarioSettingItemInput.ToArray();
		}

		/// <summary>
		/// 戻るボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnBack_Click(object sender, EventArgs e)
		{
			this.ScenarioSetting = null;
			Response.Redirect(
				new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_CONFIRM)
					.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
					.AddParam(
						Constants.REQUEST_KEY_ORDERWORKFLOWSCENARIOSETTING_ORDERWORKFLOW_SCENARIOSETTING_ID,
						RequestParamOfScenarioSettingId)
					.CreateUrl());
		}

		/// <summary>
		/// 一覧に戻るボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnBackList_Click(object sender, EventArgs e)
		{
			this.ScenarioSetting = null;
			Response.Redirect(
				new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_SCENARIO_LIST)
					.CreateUrl());
		}

		/// <summary>
		/// 実行対象ワークフロー区分が受注に変更されたときの処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbTargetWorkflowByNormal_Click(object sender, EventArgs e)
		{
			var itemIndex = ((RepeaterItem)((LinkButton)sender).Parent).ItemIndex;
			this.ScenarioSettingItemsForDisplay[itemIndex].TargetWorkflowKbn = Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL;
			this.ScenarioSettingItemsForDisplay[itemIndex].WorkflowKbn = string.Empty;
			this.ScenarioSettingItemsForDisplay[itemIndex].WorkflowNo = 0;

			ChangeTargetWorkflowKbn((RepeaterItem)((LinkButton)sender).Parent);
		}

		/// <summary>
		/// 実行対象ワークフロー区分が定期に変更されたときの処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbTargetWorkflowByFixedPurchase_Click(object sender, EventArgs e)
		{
			var itemIndex = ((RepeaterItem)((LinkButton)sender).Parent).ItemIndex;
			this.ScenarioSettingItemsForDisplay[itemIndex].TargetWorkflowKbn = Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_FIXEDPURCHASE;
			this.ScenarioSettingItemsForDisplay[itemIndex].WorkflowKbn = string.Empty;
			this.ScenarioSettingItemsForDisplay[itemIndex].WorkflowNo = 0;

			ChangeTargetWorkflowKbn((RepeaterItem)((LinkButton)sender).Parent);
		}

		/// <summary>
		/// 実行対象ワークフロー区分が変更されたときの処理
		/// </summary>
		/// <param name="repeaterItem">リピーター</param>
		private void ChangeTargetWorkflowKbn(RepeaterItem repeaterItem)
		{
			var ddlWorkflowList = (DropDownList)repeaterItem.FindControl("ddlWorkflowList");
			ddlWorkflowList.Items.Clear();
			ddlWorkflowList.Items.Add(string.Empty);

			SortOfExecNo();
			CreateScenarioEditor();
		}

		/// <summary>シナリオセッティングIDのリクエストパラメーター</summary>
		private string RequestParamOfScenarioSettingId
		{
			get
			{
				return Request[Constants.REQUEST_KEY_ORDERWORKFLOWSCENARIOSETTING_ORDERWORKFLOW_SCENARIOSETTING_ID];
			}
		}
		/// <summary>シナリオ設定アイテムが入ったリスト</summary>
		private List<OrderWorkflowScenarioSettingItemModel> ScenarioSettingItemsForDisplay
		{
			get
			{
				if (ViewState["ScenarioSettingItemsForDisplay"] == null)
				{
					ViewState["ScenarioSettingItemsForDisplay"] = new List<OrderWorkflowScenarioSettingItemModel>();
					var scenarioSettingItems
						= (List<OrderWorkflowScenarioSettingItemModel>)ViewState["ScenarioSettingItemsForDisplay"];
					scenarioSettingItems.Add(new OrderWorkflowScenarioSettingItemModel());
					ViewState["ScenarioSettingItemsForDisplay"] = scenarioSettingItems;
				}
				return (List<OrderWorkflowScenarioSettingItemModel>)ViewState["ScenarioSettingItemsForDisplay"];
			}
			set { ViewState["ScenarioSettingItemsForDisplay"] = value; }
		}
		/// <summary>セッションに格納されるシナリオ設定</summary>
		private OrderWorkflowScenarioSettingModel ScenarioSetting
		{
			get { return (OrderWorkflowScenarioSettingModel)Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_WORKFLOW_SCENARIO_SETTING]; }
			set { Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_WORKFLOW_SCENARIO_SETTING] = value; }
		}
	}
}
