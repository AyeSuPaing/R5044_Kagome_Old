/*
=========================================================================================================
  Module      : 問合せ完了画面処理(InquiryComplete.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Web.WrappedContols;

public partial class Form_Inquiry_InquiryComplete : BasePage
{
	#region ラップ済みコントロール宣言
	WrappedHtmlGenericControl WspShopping { get { return GetWrappedControl<WrappedHtmlGenericControl>("spShopping"); } }
	#endregion
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
			// セッションチェック
			//------------------------------------------------------
			if (Session[Constants.SESSION_KEY_PARAM] != null)
			{
				// 商品ページへのリンク表示
				try
				{
					Hashtable htInquiryInput = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
					WspShopping.Visible = StringUtility.ToEmpty(htInquiryInput[Constants.INQUIRY_KEY_PRODUCT_URL]) != "";
				}
				catch
				{
					// パラメタ取得失敗エラー
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NO_PARAM);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}
		}
	}

	/// <summary>
	/// 買い物を続けるリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbKeepShopping_Click(object sender, EventArgs e)
	{
		// 商品ページへ
		Response.Redirect((string)((Hashtable)Session[Constants.SESSION_KEY_PARAM])[Constants.INQUIRY_KEY_PRODUCT_URL]);
	}
}