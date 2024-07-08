/*
=========================================================================================================
  Module      : 共有情報確認ページ処理(ShareInfoConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using w2.App.Common.Cs.ShareInfo;
using w2.Common.Extensions;

public partial class Form_ShareInfo_ShareInfoConfirm : BasePage
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
			// レイアウト上、マスタページの閉じるボタンを表示しない
			((Form_Common_PopupPage)Master).HideCloseButton = true;

			// 更新・続けて更新
			if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE) Display();
		}
	}
	#endregion

	#region -Display 画面表示
	/// <summary>
	/// 画面表示
	/// </summary>
	private void Display()
	{
		var shareInfoService = new CsShareInfoService(new CsShareInfoRepository());
		var shareInfo = shareInfoService.Get(this.LoginOperatorDeptId, this.InfoNo);
		var readService = new CsShareInfoReadService(new CsShareInfoReadRepository());
		var readInfo = readService.Get(this.LoginOperatorDeptId, this.InfoNo, this.LoginOperatorId);
		if ((shareInfo == null) || (readInfo == null))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		divInfoRead.Visible = (readInfo.ReadFlg == Constants.FLG_CSSHAREINFOREAD_READ_FLG_READ);
		divInfoUnread.Visible = (readInfo.ReadFlg == Constants.FLG_CSSHAREINFOREAD_READ_FLG_UNREAD);
		divInfoPinned.Visible = (readInfo.PinnedFlg == Constants.FLG_CSSHAREINFOREAD_PINNED_FLG_PINNED);
		divInfoNopin.Visible = (readInfo.PinnedFlg == Constants.FLG_CSSHAREINFOREAD_PINNED_FLG_NOPIN);

		lShareInfoKbn.Text = WebSanitizer.HtmlEncode(shareInfo.EX_InfoKbnName);
		lSenderName.Text = WebSanitizer.HtmlEncode(shareInfo.EX_SenderName);
		lImportance.Text = WebSanitizer.HtmlEncode(shareInfo.EX_InfoImportanceName);
		lDateCreated.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				shareInfo.DateCreated,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
		if (shareInfo.InfoTextKbn == Constants.FLG_CSSHAREINFO_INFO_TEXT_KBN_TEXT)
		{
			lInfoText.Text = WebSanitizer.HtmlEncodeChangeToBr(shareInfo.InfoText);
		}
		else if (shareInfo.InfoTextKbn == Constants.FLG_CSSHAREINFO_INFO_TEXT_KBN_HTML)
		{
			lInfoText.Text = shareInfo.InfoText;
		}
	}
	#endregion

	#region #btnRead_Click 確認済みにするボタンクリック
	/// <summary>
	/// 確認済みにするボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRead_Click(object sender, System.EventArgs e)
	{
		UpdateReadFlg(Constants.FLG_CSSHAREINFOREAD_READ_FLG_READ);
	}
	#endregion

	#region #btnUnread_Click 未確認へボタンクリック
	/// <summary>
	/// 未確認へボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUnread_Click(object sender, System.EventArgs e)
	{
		UpdateReadFlg(Constants.FLG_CSSHAREINFOREAD_READ_FLG_UNREAD);
	}
	#endregion

	#region #btnPin_Click 
	/// <summary>
	/// ピン留めするボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnPin_Click(object sender, System.EventArgs e)
	{
		UpdatePinnedFlg(Constants.FLG_CSSHAREINFOREAD_PINNED_FLG_PINNED);
	}
	#endregion

	#region #btnUnpin_Click ピン留め外すボタンクリック
	/// <summary>
	/// ピン留め外すボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUnpin_Click(object sender, System.EventArgs e)
	{
		UpdatePinnedFlg(Constants.FLG_CSSHAREINFOREAD_PINNED_FLG_NOPIN);
	}
	#endregion

	#region -UpdateReadFlg 既読フラグ更新
	/// <summary>
	/// 既読フラグ更新
	/// </summary>
	/// <param name="readFlg"></param>
	private void UpdateReadFlg(string readFlg)
	{
		// 既読フラグ更新
		var model = CreateShareInfoReadModel();
		model.ReadFlg = readFlg;
		model.LastChanged = this.LoginOperatorName;
		var service = new CsShareInfoReadService(new CsShareInfoReadRepository());
		service.UpdateReadFlg(model);

		// 表示
		Display();

		// 未読件数バッジの更新
		RefreshTopShareInfo();
	}
	#endregion

	#region -UpdatePinnedFlg ピン止めフラグ更新
	/// <summary>
	/// ピン止めフラグ更新
	/// </summary>
	private void UpdatePinnedFlg(string pinnedFlg)
	{
		// ピン止めフラグ更新
		var model = CreateShareInfoReadModel();
		model.PinnedFlg = pinnedFlg;
		model.LastChanged = this.LoginOperatorName;
		var service = new CsShareInfoReadService(new CsShareInfoReadRepository());
		service.UpdatePinnedFlg(model);

		// 表示
		Display();
	}
	#endregion

	#region -CreateShareInfoReadModel 主キーのみセットされた共有情報既読モデル作成
	/// <summary>
	/// 主キーのみセットされた共有情報既読モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	private CsShareInfoReadModel CreateShareInfoReadModel()
	{
		var model = new CsShareInfoReadModel();
		model.DeptId = this.LoginOperatorDeptId;
		model.InfoNo = this.InfoNo;
		model.OperatorId = this.LoginOperatorId;
		return model;
	}
	#endregion

	#region -RefreshTopShareInfo トップページ共有情報更新
	/// <summary>
	/// トップページ共有情報更新
	/// </summary>
	private void RefreshTopShareInfo()
	{
		ScriptManager.RegisterStartupScript(this, this.GetType(), "", "refresh_share_info_count();", true);
	}
	#endregion

	#region プロパティ
	/// <summary>ウインドウタイプ</summary>
	protected string WindowTypeStatus
	{
		get { return Request[Constants.REQUEST_KEY_WINDOW_TYPE]; }
	}
	/// <summary>共有情報NO</summary>
	protected long InfoNo
	{
		get
		{
			long infoNo;
			return long.TryParse(Request[Constants.REQUEST_KEY_INFO_NO], out infoNo) ? infoNo : 1;
		}
	}
	#endregion
}
