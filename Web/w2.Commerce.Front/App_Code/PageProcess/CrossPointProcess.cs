/*
=========================================================================================================
  Module      : CROSS POINTプロセス(CrossPointProcess)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using System.Web.UI;
using w2.Domain.User;

/// <summary>
/// CROSS POINTプロセス
/// </summary>
public class CrossPointProcess : BasePageProcess
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="caller">呼び出し元</param>
	/// <param name="viewState">ビューステート</param>
	/// <param name="context">コンテキスト</param>
	public CrossPointProcess(object caller, StateBag viewState, HttpContext context)
		: base(caller, viewState, context)
	{
	}

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void Page_Load(object sender, EventArgs e)
	{
		if ((this.IsPreview == false) && this.IsLoggedIn)
		{
			this.LoginUser.UserExtend = new UserService().GetUserExtend(this.LoginUserId);
		}
	}

	/// <summary>
	/// 店舗カード番号取得
	/// </summary>
	/// <param name="userExtend">ユーザー拡張項目モデル</param>
	/// <returns>CROSS POINT ユーザー拡張項目(店舗カード番号)</returns>
	public string GetCrossPointShopCardNo(UserExtendModel userExtend)
	{
		if ((userExtend != null)
			&& userExtend.UserExtendDataValue.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_NO)
			&& (string.IsNullOrEmpty(userExtend.UserExtendDataValue.CrossPointShopCardNo) == false))
		{
			var shopCardNo = userExtend.UserExtendDataValue.CrossPointShopCardNo;
			return shopCardNo;
		}
		return string.Empty;
	}
}