/*
=========================================================================================================
  Module      : 項目メモ表示コントローラ処理(BodyFieldMemoSetting.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.Domain.FieldMemoSetting;

public partial class Form_Common_FieldMemoSetting_BodyFieldMemoSetting : BaseUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.DataBind();

		linkMemo.Attributes.Add("title", this.Title);
		linkMemo.Attributes.Add("href-data",
			Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIELD_MEMO_SETTING
			+ "?table_name="
			+ this.TableName
			+ "&field_name="
			+ this.FieldName);

		if (this.FieldMemoSettingList != null
			&& this.FieldMemoSettingList.Any(m => (m.FieldName == this.FieldName))
			&& this.FieldMemoSettingList.FirstOrDefault(m => (m.FieldName == this.FieldName)).Memo != "")
		{
			divTitle.Visible = true;
			lblTitle.Text = WebSanitizer.HtmlEncode(
				this.FieldMemoSettingList.FirstOrDefault(m => (m.FieldName == this.FieldName)).Memo);
		}
		else
		{
			divTitle.Visible = false;
		}
	}

	/// <summary>項目メモ一覧</summary>
	public FieldMemoSettingModel[] FieldMemoSettingList { get; set; }

	/// <summary>タイトル</summary>
	public string Title { get; set; }

	/// <summary>テーブル名</summary>
	public string TableName { get; set; }

	/// <summary>フィールド名</summary>
	public string FieldName { get; set; }
}