/*
=========================================================================================================
  Module      : 会員証バーコードコントロール(BodyMemberCardBarcode.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;

public partial class MiniApp_Form_Common_BodyMemberCardBarcode : LineMiniAppControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.Process.Page_Load(sender, e);
		DataBind();
	}

	/// <summary>
	/// 店舗カード番号取得
	/// </summary>
	/// <returns>CROSS POINT ユーザー拡張項目(店舗カード番号)</returns>
	protected string GetCrossPointShopCardNo()
	{
		return this.IsLoggedIn
			? this.Process.GetCrossPointShopCardNo(this.LoginUser.UserExtend)
			: string.Empty;
	}
}