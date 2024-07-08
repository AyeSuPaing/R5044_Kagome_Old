/*
=========================================================================================================
  Module      : 共有情報管理確認ページ処理(ShareInfoSettingConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.App.Common.Cs.ShareInfo;

public partial class Form_ShareInfoSetting_ShareInfoSettingConfirm : ShareInfoSettingPage
{
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 初期化
			Initialize();

			// 共有情報表示
			DisplayShareInfoSetting();
		}
	}
	#endregion

	#region -Initialize 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				trConfirm.Visible = true;
				btnInsertTop.Visible = btnInsertBottom.Visible = true;
				break;

			case Constants.ACTION_STATUS_UPDATE:
				trConfirm.Visible = true;
				trInfoNo.Visible = true;
				trDateCreated.Visible = true;
				btnUpdateTop.Visible = btnUpdateBottom.Visible = true;
				break;

			case Constants.ACTION_STATUS_DETAIL:
				btnEditTop.Visible = btnEditBottom.Visible = true;
				btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = true;
				btnDeleteTop.Visible = btnDeleteBottom.Visible = true;
				trDetail.Visible = true;
				trDateCreated.Visible = true;
				break;

			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}
	}
	#endregion

	#region -DisplayShareInfoSetting 共有情報表示
	/// <summary>
	/// 共有情報表示
	/// </summary>
	private void DisplayShareInfoSetting()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
				this.ShareInfo = (CsShareInfoModel)Session[Constants.SESSION_KEY_SHAREINFO_INFO];
				this.ShareInfoReads = (CsShareInfoReadModel[])Session[Constants.SESSION_KEY_SHAREINFOREAD_INFO];
				break;

			case Constants.ACTION_STATUS_DETAIL:
				var shareInfoService = new CsShareInfoService(new CsShareInfoRepository());
				this.ShareInfo = shareInfoService.Get(this.LoginOperatorDeptId, this.InfoNo);
				var readService = new CsShareInfoReadService(new CsShareInfoReadRepository());
				this.ShareInfoReads = readService.GetAll(this.LoginOperatorDeptId, this.InfoNo);
				break;
		}
		if (this.ShareInfo == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		if ((this.ActionStatus == Constants.ACTION_STATUS_DETAIL)
			|| (this.ActionStatus == Constants.ACTION_STATUS_UPDATE))
		{
			lInfoNo.Text = WebSanitizer.HtmlEncode(this.ShareInfo.InfoNo);
		}
		lInfoKbn.Text = WebSanitizer.HtmlEncode(this.ShareInfo.EX_InfoKbnName);
		lImportance.Text = WebSanitizer.HtmlEncode(this.ShareInfo.EX_InfoImportanceName);
		lSenderName.Text = WebSanitizer.HtmlEncode(this.ShareInfo.EX_SenderName);
		lInfoTextKbn.Text = WebSanitizer.HtmlEncode(this.ShareInfo.EX_InfoTextKbnName);
		if (this.ShareInfo.InfoTextKbn == Constants.FLG_CSSHAREINFO_INFO_TEXT_KBN_TEXT)
		{
			lInfoText.Text = WebSanitizer.HtmlEncodeChangeToBr(this.ShareInfo.InfoText);
		}
		else if (this.ShareInfo.InfoTextKbn == Constants.FLG_CSSHAREINFO_INFO_TEXT_KBN_HTML)
		{
			lInfoText.Text = this.ShareInfo.InfoText;
		}
		if ((this.ActionStatus == Constants.ACTION_STATUS_DETAIL)
			|| (this.ActionStatus == Constants.ACTION_STATUS_UPDATE))
		{
			lDateCreated.Text = WebSanitizer.HtmlEncodeChangeToBr(
				DateTimeUtility.ToStringForManager(
					this.ShareInfo.DateCreated,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
		}
		lReadRate.Text = WebSanitizer.HtmlEncode(this.ShareInfo.EX_ReadRateString);
		lShareCount.Text = WebSanitizer.HtmlEncode(this.ShareInfo.EX_ShareCount);
		lReadCount.Text = WebSanitizer.HtmlEncode(this.ShareInfo.EX_ReadCount);

		rReadList.DataSource = this.ShareInfoReads;
		rReadList.DataBind();
	}
	#endregion

	#region #btnEdit_Click 編集するボタンクリック
	/// <summary>
	/// 編集するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// 共有情報をそのままセッションへセット
		Session[Constants.SESSION_KEY_SHAREINFO_INFO] = this.ShareInfo;
		Session[Constants.SESSION_KEY_SHAREINFOREAD_INFO] = this.ShareInfoReads;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SHAREINFOSETTING_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);
	}
	#endregion 

	#region #btnCopyInsert_Click コピー新規登録するボタンクリック
	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, System.EventArgs e)
	{
		// 共有情報をそのままセッションへセット
		Session[Constants.SESSION_KEY_SHAREINFO_INFO] = this.ShareInfo;
		Session[Constants.SESSION_KEY_SHAREINFOREAD_INFO] = this.ShareInfoReads;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT;

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SHAREINFOSETTING_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COPY_INSERT);
	}
	#endregion

	#region #btnInsert_Click 登録するボタンクリック
	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 登録
		var shareInfoService = new CsShareInfoService(new CsShareInfoRepository());
		long infoNo = shareInfoService.Register(this.ShareInfo);

		// 既読情報登録/削除
		foreach (var read in this.ShareInfoReads) read.InfoNo = infoNo;
		RegisterDeleteShareInfoReads(infoNo);

		// 共有情報一覧ページへ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SHAREINFOSETTING_LIST);
	}
	#endregion

	#region #btnUpdate_Click 更新ボタンクリック
	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		// 更新
		var shareInfoService = new CsShareInfoService(new CsShareInfoRepository());
		shareInfoService.Update(this.ShareInfo);

		// 既読情報登録/削除
		RegisterDeleteShareInfoReads(this.ShareInfo.InfoNo);

		// 共有情報一覧ページへ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SHAREINFOSETTING_LIST);
	}
	#endregion

	#region #btnDelete_Click 削除ボタンクリック
	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		// 既読含めて全て削除
		var shareInfoService = new CsShareInfoService(new CsShareInfoRepository());
		shareInfoService.DeleteWithReads(this.ShareInfo.DeptId, this.ShareInfo.InfoNo);

		// 共有情報一覧ページへ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_SHAREINFOSETTING_LIST);
	}
	#endregion

	#region -RegisterDeleteShareInfoReads 既読情報登録/削除
	/// <summary>
	/// 既読情報登録/削除
	/// </summary>
	/// <param name="infoNo">共有情報NO</param>
	private void RegisterDeleteShareInfoReads(long infoNo)
	{
		var shareInfoReadService = new CsShareInfoReadService(new CsShareInfoReadRepository());
		shareInfoReadService.RegisterDelete(this.LoginOperatorDeptId, infoNo, this.ShareInfoReads);
	}
	#endregion

	#region プロパティ
	/// <summary>共有情報NO</summary>
	protected long InfoNo
	{
		get
		{
			long infoNo;
			return long.TryParse(Request[Constants.REQUEST_KEY_INFO_NO], out infoNo) ? infoNo : 0;
		}
	}
	#endregion

}
