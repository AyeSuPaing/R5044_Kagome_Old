/*
=========================================================================================================
  Module      : CSオペレータ所属グループ登録ページ処理(CsOperatorGroupRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.MailAssignSetting;

public partial class Form_CsOperatorGroup_CsOperatorGroupRegister : BasePage
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
			// グループ情報表示
			DisplayGroup();

			// オペレータリスト表示
			DisplayOperatorList();
		}
	}

	/// <summary>
	/// グループ情報表示
	/// </summary>
	private void DisplayGroup()
	{
		// グループ名を設定
		CsGroupService service = new CsGroupService(new CsGroupRepository());
		CsGroupModel model = service.Get(this.LoginOperatorDeptId, this.CsGroupId);
		if (model == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		lbGroupName.Text = WebSanitizer.HtmlEncode(model.CsGroupName);
	}

	/// <summary>
	/// オペレータリスト表示
	/// </summary>
	private void DisplayOperatorList()
	{
		// オペレータ一覧を取得
		CsOperatorService operatorService = new CsOperatorService(new CsOperatorRepository());
		CsOperatorModel[] operatorsAll = operatorService.GetValidAll(this.LoginOperatorDeptId);
		CsOperatorGroupService operatorGroupService = new CsOperatorGroupService(new CsOperatorGroupRepository());
		CsOperatorModel[] operatorsAssigned = operatorGroupService.GetValidOperators(this.LoginOperatorDeptId, this.CsGroupId);

		// 所属オペレータとして割り当て
		foreach (CsOperatorModel operatorModel in operatorsAssigned)
		{
			lbAssignedOperatorList.Items.Add(new ListItem(operatorModel.EX_ShopOperatorName, operatorModel.OperatorId));
		}

		// 未所属オペレータとして割り当て
		IEnumerable<string> assignedIds = operatorsAssigned.Select(item => item.OperatorId);
		foreach (CsOperatorModel operatorModel in operatorsAll)
		{
			if (assignedIds.Contains(operatorModel.OperatorId) == false)
			{
				lbUnassignedOperatorList.Items.Add(new ListItem(operatorModel.EX_ShopOperatorName, operatorModel.OperatorId));
			}
		}
	}

	/// <summary>
	/// 所属ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAssign_Click(object sender, EventArgs e)
	{
		var selectedList = (from ListItem li in lbUnassignedOperatorList.Items where li.Selected select li).ToArray();
		foreach (ListItem li in selectedList)
		{
			lbAssignedOperatorList.Items.Add(li);
			lbUnassignedOperatorList.Items.Remove(li);
			li.Selected = false; // 選択状態が残ってしまうので解除する。
		}
	}

	/// <summary>
	/// 所属解除ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUnassign_Click(object sender, EventArgs e)
	{
		var selectedList = (from ListItem li in lbAssignedOperatorList.Items where li.Selected select li).ToArray();
		foreach (ListItem li in selectedList)
		{
			lbUnassignedOperatorList.Items.Add(li);
			lbAssignedOperatorList.Items.Remove(li);
			li.Selected = false; // 選択状態が残ってしまうので解除する。
		}
	}

	/// <summary>
	/// 更新するボタン押下時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		var service = new CsOperatorGroupService(new CsOperatorGroupRepository());
		var removeOperators = service.GetValidOperators(this.LoginOperatorDeptId, this.CsGroupId);

		// 入力値を元にモデル作成
		List<CsOperatorGroupModel> models = new List<CsOperatorGroupModel>();
		foreach (ListItem li in lbAssignedOperatorList.Items)
		{
			if (removeOperators.Any(ro => ro.OperatorId == li.Value))
			{
				//変わらず登録のままなのでこのリストから削除
				removeOperators = removeOperators.Where(ro => ro.OperatorId != li.Value).ToArray();
			}
			CsOperatorGroupModel model = new CsOperatorGroupModel();
			model.DeptId = this.LoginOperatorDeptId;
			model.CsGroupId = this.CsGroupId;
			model.OperatorId = li.Value;
			model.LastChanged = this.LoginOperatorName;
			models.Add(model);
		}

		var assignService = new CsMailAssignSettingService(new CsMailAssignSettingRepository());
		assignService.UpdateMailAssignSettingByRemoveOperator(this.LoginOperatorDeptId, this.CsGroupId, removeOperators);

		// 更新処理
		service.Update(this.LoginOperatorDeptId, this.CsGroupId, models.ToArray());

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_CSOPERATORGROUP_LIST);
	}
	
	/// <summary>
	/// 戻るボタン押下時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_CSOPERATORGROUP_LIST);
	}

	#region プロパティ
	/// <summary>
	/// CSグループID
	/// </summary>
	private string CsGroupId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CS_GROUP_ID]); }
	}
	#endregion
}
