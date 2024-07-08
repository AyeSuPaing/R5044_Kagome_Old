/*
=========================================================================================================
  Module      : 会員登録完了画面処理(UserRegistComplete.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Web.WrappedContols;

public partial class Form_User_UserRegistComplete : UserPage
{
	#region ラップ済みコントロール宣言
	WrappedHtmlGenericControl WspNextUrl { get { return GetWrappedControl<WrappedHtmlGenericControl>("spNextUrl"); } }
	WrappedHtmlGenericControl WspTopPage { get { return GetWrappedControl<WrappedHtmlGenericControl>("spTopPage"); } }
	WrappedHtmlGenericControl WspCart { get { return GetWrappedControl<WrappedHtmlGenericControl>("spCart"); } }
	WrappedHtmlGenericControl WspUserProductArrivalMailList { get { return GetWrappedControl<WrappedHtmlGenericControl>("spUserProductArrivalMailList"); } }
	#endregion
	
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// ユーザメールアドレス表示
			//------------------------------------------------------
			try
			{
				UserInput userInfo = (UserInput)Session[Constants.SESSION_KEY_PARAM];
				this.UserMailAddr = userInfo.MailAddr;
				this.UserMailAddr2 = userInfo.MailAddr2;
			}
			catch
			{
				// パラメタ取得失敗エラー
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NO_PARAM);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			//------------------------------------------------------
			// ボタン表示
			//------------------------------------------------------
			string strNextUrl = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_NEXT_URL]);

			// 三分岐画面からだった場合、「配送先入力画面へ」ボタン表示
			this.WspNextUrl.Visible = (strNextUrl.IndexOf(Constants.PAGE_FRONT_ORDER_SHIPPING) != -1);

			// カートへボタン表示
			this.WspCart.Visible = (strNextUrl.IndexOf(Constants.PAGE_FRONT_CART_LIST) != -1);

			// 入荷お知らせメール情報へボタン表示
			this.WspUserProductArrivalMailList.Visible = (strNextUrl.IndexOf(Constants.PAGE_FRONT_USER_PRODUCT_ARRIVAL_MAIL_LIST) != -1);

			// 更新ボタン押したときの対策用にセッションを消さない
			//// 遷移用パラメタセッションに格納されているユーザデータを破棄
			//Session[Constants.SESSION_KEY_PARAM] = null;
		}

		DataBind();
	}

	/// <summary>
	/// トップページへリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbTopPage_Click(object sender, EventArgs e)
	{
		// トップページへ
		Response.Redirect(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT);
	}

	/// <summary>
	/// 配送先入力画面へリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbShipping_Click(object sender, EventArgs e)
	{
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;

		// 配送先入力画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING);
	}

	/// <summary>
	/// カートへ戻るリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCart_Click(object sender, EventArgs e)
	{
		// カート一覧へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
	}

	/// <summary>
	/// 入荷お知らせメール一覧へリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUserProductArrivalMailList_Click(object sender, EventArgs e)
	{
		// 入荷お知らせメール一覧へ
		Response.Redirect((string)Session[Constants.SESSION_KEY_NEXT_URL]);
	}

	/// <summary>ユーザーメールアドレス</summary>
	protected string UserMailAddr
	{
		get { return (string)ViewState["UserMailAddr"]; }
		set { ViewState["UserMailAddr"] = value; }
	}
	/// <summary>モバイルユーザーメールアドレス</summary>
	protected string UserMailAddr2
	{
		get { return (string)ViewState["UserMailAddr2"]; }
		set { ViewState["UserMailAddr2"] = value; }
	}
}
