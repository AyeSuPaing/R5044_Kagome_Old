/*
=========================================================================================================
  Module      : デフォルトプレビューページマスタ処理(DefaultPagePreview.master.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Option;
using w2.Domain.TargetList;

public partial class Form_Common_DefaultPagePreview : BaseMasterPage
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
			tbDatePicker.Value = this.ReferenceDateTime.ToString("d");
			tbTimePicker.Value = this.ReferenceDateTime.ToString("HH:mm");

			ddlMemberRank.DataSource = MemberRankOptionUtility.GetMemberRankList();
			ddlMemberRank.DataTextField = "MemberRankName";
			ddlMemberRank.DataValueField = "MemberRankId";
			ddlMemberRank.DataBind();
			ddlMemberRank.Items.Insert(0, new ListItem());
			ddlMemberRank.SelectedValue = (this.ReferenceMemgbeRankModel == null) ? "" : this.ReferenceMemgbeRankModel.MemberRankId;

			lbTargetLists.DataSource = new TargetListService().GetAll(Constants.CONST_DEFAULT_DEPT_ID);
			lbTargetLists.DataTextField = "TargetName";
			lbTargetLists.DataValueField = "TargetId";
			lbTargetLists.DataBind();
			lbTargetLists.Items.Insert(0, new ListItem());

			foreach (ListItem targetItem in lbTargetLists.Items)
			{
				targetItem.Selected = this.ReferenceTargetList.Contains(targetItem.Value);
			}
		}
		PreviewInit();
	}

	/// <summary>
	/// ページアンロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_LoadComplete(object sender, EventArgs e)
	{
		PreviewEnd();
	}

	/// <summary>
	/// プレビュー初期処理
	/// </summary>
	/// <remarks>
	/// セッションのバックアップ
	/// セッションにプレビュー用ダミーデータを配置
	/// </remarks>
	private void PreviewInit()
	{
		if (this.IsPreview == false) return;

		// プレビュー時はキャッシュを残さない
		Preview.SetNonCache(this.Response);

		// セッションのバックアップ
		this.SessionBackup = new List<KeyValuePair<string, object>>();
		foreach (var sessionName in this.SessionNameList
			.Where(s => this.SessionBackupExcludedNameList.Any(ns => ns == s) == false))
		{
			var value = new KeyValuePair<string, object>(sessionName, this.Session[sessionName]);
			this.SessionBackup.Add(value);
			this.Session.Remove(sessionName);
		}

		// セッションにダミーデータを配置
		this.LoginUser = Preview.GetDummyUserModel();
		this.LoginUserId = this.LoginUser.UserId;
		this.LoginUserName = "Preview";

		if (Constants.MEMBER_RANK_OPTION_ENABLED) this.LoginMemberRankInfo = this.ReferenceMemgbeRankModel;

		this.LoginUserHitTargetListIds = this.ReferenceTargetList;
	}

	/// <summary>
	/// プレビュー終了時処理
	/// </summary>
	/// <remarks>
	/// セッションの復元
	/// </remarks>
	private void PreviewEnd()
	{
		if (this.IsPreview == false) return;

		// バックアップからセッションを復元
		foreach (var sessionName in this.SessionNameList
			.Where(s => this.SessionBackupExcludedNameList.Any(ns => ns == s) == false))
		{
			this.Session.Remove(sessionName);
		}
		this.SessionBackup.ForEach(s => { this.Session.Add(s.Key, s.Value); });
	}

	/// <summary>
	/// レビュー入力画面の表示へ戻る
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReDraw_Click(object sender, EventArgs e)
	{
		DateTime referDateTime;
		if (DateTime.TryParse(string.Format("{0} {1}", tbDatePicker.Value, tbTimePicker.Value), out referDateTime) == false)
		{
			referDateTime = DateTime.Now;
		}
		this.ReferenceDateTime = referDateTime;

		var rankId = ddlMemberRank.SelectedValue;
		this.ReferenceMemgbeRankModel = MemberRankOptionUtility.GetMemberRankList()
			.FirstOrDefault(memberRank => memberRank.MemberRankId == rankId);

		this.ReferenceTargetList = lbTargetLists.GetSelectedIndices()
			.Select(index => lbTargetLists.Items[index].Value)
			.ToArray();

		Response.Redirect(Request.RawUrl);
	}

	/// <summary>セッション名一覧</summary>
	private string[] SessionNameList
	{
		get { return this.Session.Cast<object>().Select(sessionName => sessionName.ToString()).ToArray(); }
	}
	/// <summary>プレビュー時のバックアップセッション</summary>
	private List<KeyValuePair<string, object>> SessionBackup { get; set; }
	/// <summary>プレビュー時のバックアップ・削除・復元の対象外セッション</summary>
	private string[] SessionBackupExcludedNameList
	{
		get
		{
			return new[]
			{
				Constants.SESSION_KEY_REFERENCE_DATETIME,
				Constants.SESSION_KEY_REFERENCE_MEMBER_RANK,
				Constants.SESSION_KEY_REFERENCE_TARGET_LIST,
				"ViewStateUserKey",
				Constants.SESSION_KEY_ERROR_MSG
			};
		}
	}

}
