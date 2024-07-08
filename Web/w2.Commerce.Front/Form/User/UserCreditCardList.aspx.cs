/*
=========================================================================================================
  Module      : ユーザクレジットカード一覧画面処理(UserCreditCardList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserCreditCard;
using w2.App.Common.Order;
using w2.Common.Web;

public partial class Form_User_UserCreditCardList : BasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示

	private const string SESSION_KEY_DELTE_MESSAGE_FLG = "delete_message_flg";		// 削除時メッセージ

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 登録可能かどうかチェック※登録できない場合にはエラーページへ
			//------------------------------------------------------
			if (Constants.MAX_NUM_REGIST_CREDITCARD <= 0)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR + "?" + Constants.REQUEST_KEY_ERRORPAGE_KBN + "=" + Constants.KBN_REQUEST_ERRORPAGE_GOTOP);
			}

			//------------------------------------------------------
			// ユーザクレジット情報取得
			//------------------------------------------------------
			var userCreditCards = UserCreditCard.GetUsable(this.LoginUserId);

			//------------------------------------------------------
			// ユーザクレジット一覧設定
			//------------------------------------------------------
			if (userCreditCards.Length == 0)
			{
				// エラーメッセージ
				lErrorMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USERCREDITCARD_NO_CARD);

				rUserCreditCardList.Visible = false;
			}

			// データバインド
			rUserCreditCardList.DataSource = userCreditCards;
			rUserCreditCardList.DataBind();

			//------------------------------------------------------
			// 表示制御
			//------------------------------------------------------
			lbInsert.Visible = OrderCommon.GetCreditCardRegistable(this.IsLoggedIn, userCreditCards.Length);

			//------------------------------------------------------
			// 削除時メッセージがあれば削除
			//------------------------------------------------------
			if (Session[SESSION_KEY_DELTE_MESSAGE_FLG] != null)
			{
				lDeleteMessage.Visible = true;
				Session[SESSION_KEY_DELTE_MESSAGE_FLG] = null;
			}
		}
	}

	/// <summary>
	/// リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rUserCreditCardList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		int iCreditBranchNo = int.Parse(e.CommandArgument.ToString());

		// 登録クレジット情報更新
		if (e.CommandName == "Update")
		{
			// クレジットカード入力画面へ
			var url = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_INPUT)
				.AddParam(Constants.REQUEST_KEY_CREDITCARD_NO, iCreditBranchNo.ToString())
				.CreateUrl();
			this.SessionParamTargetPage = null;
			// クレジットカード入力画面へ
			Response.Redirect(url);
		}

		//------------------------------------------------------
		// 登録クレジット情報削除
		//------------------------------------------------------
		if (e.CommandName == "Delete")
		{
			// ユーザ登録クレジット情報削除
			new UserCreditCardService().UpdateDispFlg(
				this.LoginUserId,
				iCreditBranchNo,
				false,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);

			// 削除時メッセージフラグ
			Session[SESSION_KEY_DELTE_MESSAGE_FLG] = true;

			// 配送先一覧へ戻る
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_LIST);
		}
	}

	/// <summary>
	/// 登録クレジット情報の追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbInsert_Click(object sender, System.EventArgs e)
	{
		// 登録クレジット情報入力画面へ
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_CREDITCARD_INPUT);
	}
}