/*
=========================================================================================================
  Module      : 登録解除画面(Unsubscribe.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Net.Mail;
using w2.Common.Web;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

public partial class Form_Mail_Unsubscribe : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			var userId = Request[Constants.MAIL_LISTUNSUBSCRIBE_REQUEST_KEY_USER_ID];
			var verificationKey = Request[Constants.MAIL_LISTUNSUBSCRIBE_REQUEST_KEY_VERIFICATION_KEY];

			// ユーザーIDがないメールで配信解除を選択することはまずありえないため、ページトップへ遷移する
			if (string.IsNullOrEmpty(userId)) Response.Redirect(Constants.PATH_ROOT);

			var user = new UserService().Get(userId);
			if ((user == null) || (UnsubscribeVarificationHelper.Verification(verificationKey, userId, user.MailAddr) == false))
			{
				// ユーザーIDが指定されているのにユーザーが存在しない場合や、検証に失敗した場合は悪意のある遷移の可能性が高いためエラーページに遷移し、エラーメッセージを表示する
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_MAIL_UNSUBSCRIBEL_VERIFICATION_ERROR);
				Response.Redirect(
					new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
						.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
						.CreateUrl());
			}

			new UserService().MailMagazineCancel(
				user.UserId,
				user.MailAddr,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);

			// 実際に解除できたかに関わらず、「登録を解除した」旨の画面を表示する
			// これはリロードをして同じURLを2回踏んでしまった場合などで、エラーメッセージが出るとエンドユーザーが混乱して問い合わせに繋がる可能性があるためである
		}

	}
}
