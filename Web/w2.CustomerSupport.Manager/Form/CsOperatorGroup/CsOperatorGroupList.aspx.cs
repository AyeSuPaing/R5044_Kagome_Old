/*
=========================================================================================================
  Module      : CSオペレータ所属グループ一覧ページ処理(CsOperatorGroupList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using w2.App.Common.Cs.CsOperator;

public partial class Form_CsOperatorGroup_CsOperatorGroupList : BasePage
{
	#region +Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// グループリスト表示
			DisplayGroupList();
		}
	}
	#endregion

	#region -DisplayGroupList グループリスト表示
	/// <summary>
	/// グループリスト表示
	/// </summary>
	private void DisplayGroupList()
	{
		rList.DataSource = GetGroupList();
		rList.DataBind();
	}
	#endregion

	#region -GetGroupList 一覧表示用のモデル群取得
	/// <summary>
	/// 一覧表示用のモデル群取得
	/// </summary>
	/// <returns></returns>
	private CsGroupModel[] GetGroupList()
	{
		CsGroupService service = new CsGroupService(new CsGroupRepository());
		return service.GetValidAllWithValidOperators(this.LoginOperatorDeptId);
	}
	#endregion

	#region #CreateRegisterUrl 登録用画面URL作成
	/// <summary>
	/// データバインド用：登録用画面URL作成
	/// </summary>
	/// <param name="csGroupId">CSグループID</param>
	/// <returns>オペレータ所属グループ登録用画面URL</returns>
	protected string CreateRegisterUrl(string csGroupId)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_CSOPERATORGROUP_REGISTER
			+ "?" + Constants.REQUEST_KEY_CS_GROUP_ID + "=" + StringUtility.ToEmpty(csGroupId);
	}
	#endregion
}
