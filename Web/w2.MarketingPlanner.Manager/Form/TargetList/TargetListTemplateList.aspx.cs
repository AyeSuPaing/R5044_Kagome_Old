/*
=========================================================================================================
  Module      : ターゲットリストテンプレート一覧ページ処理(TargetListTemplateList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using w2.App.Common.TargetList;

public partial class Form_TargetList_TargetListTemplateList : System.Web.UI.Page
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
			rTargetListTemplates.DataSource = TargetListTemplate.GetTemplateList();
			rTargetListTemplates.DataBind();

			// データがなければ、メッセージ表示
			CheckAndDisplayErrorMessages();
		}
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		// 一覧データバインド
		rTargetListTemplates.DataSource = TargetListTemplate.GetTemplateList(
			ddlSearchCategory.SelectedValue,
			tbSearchTemplateName.Text,
			ddlSortKbn.SelectedValue);
		rTargetListTemplates.DataBind();

		// データがなければ、メッセージ表示
		CheckAndDisplayErrorMessages();
	}

	/// <summary>
	/// データ存在チェック＆エラーメッセージ表示
	/// </summary>
	private void CheckAndDisplayErrorMessages()
	{
		trListError.Visible = (rTargetListTemplates.Items.Count == 0) ;
		tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
	}
}