/*
=========================================================================================================
  Module      : Rakuten Payment Script(RakutenPaymentScript.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

public partial class Form_Common_RakutenPaymentScript : System.Web.UI.UserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	/// <summary>ユーザークレジットカード情報入力ページか</summary>
	protected bool IsUserCreditcardInputPage
	{
		get
		{
			if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) return false;
			return ((Request.Url.AbsolutePath.ToLower().Contains(Constants.PAGE_FRONT_USER_CREDITCARD_INPUT.ToLower())
					&& (Request.QueryString[Constants.REQUEST_KEY_CREDITCARD_NO] != null))
				== false);
		}
	}
	/// <summary>編集時かどうかのフラグ</summary>
	protected bool IsEdit
	{
		get
		{
			if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) return false;
			return (Request.QueryString[Constants.REQUEST_KEY_CREDITCARD_NO] != null);
		}
	}
}