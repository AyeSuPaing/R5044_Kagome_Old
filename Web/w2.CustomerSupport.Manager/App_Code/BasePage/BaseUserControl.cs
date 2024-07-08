/*
=========================================================================================================
  Module      : 基底ユーザーコントロール(BaseUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Web.Process;
using w2.Domain.ShopOperator;

/// <summary>
/// BaseUserControl の概要の説明です
/// </summary>
public class BaseUserControl : System.Web.UI.UserControl
{
	/// <summary>
	/// タグ置換
	/// </summary>
	/// <param name="targetString">置換対象文字列</param>
	/// <param name="countyIsoCode">国ISOコード（配送先の国によって切り替わるときに利用）</param>
	/// <returns>置換後文字列</returns>
	public string ReplaceTag(string targetString, string countyIsoCode = "")
	{
		return CommonPageProcess.ReplaceTag(targetString, countyIsoCode);
	}

	/// <summary>RawUrl（IISのバージョンによる機能の違いを吸収）</summary>
	public string RawUrl
	{
		get { return w2.Common.Web.WebUtility.GetRawUrl(Request); }
	}
	/// <summary>ログイン店舗オペレータ</summary>
	protected ShopOperatorModel LoginShopOperator
	{
		get { return (ShopOperatorModel)Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR] ?? new ShopOperatorModel(); }
	}
	/// <summary>ログインオペレータ店舗ID</summary>
	protected string LoginOperatorShopId
	{
		get { return this.LoginShopOperator.ShopId; }
	}
	/// <summary>ログインオペレータ識別ID</summary>
	protected string LoginOperatorDeptId
	{
		get { return this.LoginShopOperator.ShopId; }
	}
	/// <summary>ログインオペレータID</summary>
	protected string LoginOperatorId
	{
		get { return this.LoginShopOperator.OperatorId; }
	}
	/// <summary>ログインオペレータ名</summary>
	protected string LoginOperatorName
	{
		get { return this.LoginShopOperator.Name; }
	}
	/// <summary>ログインCSオペレータ情報</summary>
	protected CsOperatorModel LoginOperatorCsInfo
	{
		get { return (CsOperatorModel)Session[Constants.SESSION_KEY_LOGIN_OPERTOR_CS_GINFO]; }
		set { Session[Constants.SESSION_KEY_LOGIN_OPERTOR_CS_GINFO] = value; }
	}
	/// <summary>アクションステータス</summary>
	protected string ActionStatus
	{
		get { return (string)Request[Constants.REQUEST_KEY_ACTION_STATUS]; }
	}
}
