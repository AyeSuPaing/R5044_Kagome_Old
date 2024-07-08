/*
=========================================================================================================
  Module      : アドレス帳一覧画面処理(UserShippingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using w2.Domain.UserShipping;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Web.WrappedContols;

public partial class Form_User_UserShippingList : ProductPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示

	# region ラップ済コントロール宣言
	WrappedRepeater WrUserShippingList { get { return GetWrappedControl<WrappedRepeater>("rUserShippingList"); } }
	# endregion

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
			// リクエストよりパラメタ取得
			//------------------------------------------------------
			GetParameters();

			//------------------------------------------------------
			// ユーザ配送先情報取得
			//------------------------------------------------------
			var beginRowNum = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.PageNumber - 1) + 1;
			var endRowNum = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.PageNumber;
			var service = new UserShippingService();
			var userShippingsCount = service.GetSearchHitCount(this.LoginUserId, beginRowNum, endRowNum);
			var userShippings = service.Search(this.LoginUserId, beginRowNum, endRowNum);

			//------------------------------------------------------
			// アドレス帳一覧設定
			//------------------------------------------------------
			if (userShippingsCount != 0)
			{
				// 0件でなければページャを設定
				string strNextUrl = Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_SHIPPING_LIST;
				this.PagerHtml = WebPager.CreateDefaultListPager(userShippingsCount, this.PageNumber, strNextUrl);
			}
			else
			{
				// 0件の場合、エラーメッセージ表示
				this.ErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USERSHIPPING_NO_SHIPPING);

				this.WrUserShippingList.Visible = false;
			}

			// データバインド
			this.WrUserShippingList.DataSource = Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
				? userShippings
				: userShippings.Where(item => item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF);
			this.WrUserShippingList.DataBind();

			//------------------------------------------------------
			// 削除時メッセージがあれば削除
			//------------------------------------------------------
			IsDelete= (Session[SESSION_KEY_DELTE_MESSAGE_FLG] != null);
			Session[SESSION_KEY_DELTE_MESSAGE_FLG] = null;

		}
	}

	/// <summary>
	/// リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rUserShippingList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		int iShippingNo = int.Parse(e.CommandArgument.ToString());

		//------------------------------------------------------
		// ユーザ配送先情報更新？
		//------------------------------------------------------
		if (e.CommandName == "Update")
		{
			// アドレス帳入力画面へ遷移
			StringBuilder sbNextUrl = new StringBuilder();
			sbNextUrl.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_USER_SHIPPING_INPUT);
			sbNextUrl.Append("?").Append(Constants.REQUEST_KEY_SHIPPING_NO).Append("=").Append(iShippingNo.ToString());

			// ユーザー配送先入力画面へ
			Response.Redirect(sbNextUrl.ToString());
		}
		//------------------------------------------------------
		// ユーザ配送先情報削除？
		//------------------------------------------------------
		else if (e.CommandName == "Delete")
		{
			// ユーザ配送先情報削除
			new UserShippingService().Delete(
				this.LoginUserId,
				iShippingNo,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert,
				Constants.TWOCLICK_OPTION_ENABLE);

			// 削除時メッセージフラグ
			Session[SESSION_KEY_DELTE_MESSAGE_FLG] = true;

			// 配送先一覧へ戻る
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_SHIPPING_LIST);
		}
	}

	/// <summary>
	/// アドレス帳の追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbInsert_Click(object sender, System.EventArgs e)
	{
		// アドレス帳入力画面へ
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_SHIPPING_INPUT);
	}

	/// <summary>ページャーHTML</summary>
	protected string PagerHtml
	{
		get { return (string)ViewState["PagerHtml"]; }
		private set { ViewState["PagerHtml"] = value; }
	}
	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage 
	{
		get { return (string)ViewState["ErrorMessage"]; }
		private set { ViewState["ErrorMessage"] = value; }
	}
	/// <summary>デリート？</summary>
	protected bool IsDelete
	{
		get { return (bool)ViewState["DeleteMessage"]; }
		set { ViewState["DeleteMessage"] = value; }
	}
}
