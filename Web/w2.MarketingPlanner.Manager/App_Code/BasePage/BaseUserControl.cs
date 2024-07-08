/*
=========================================================================================================
  Module      : 基底ユーザーコントロール(BaseUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.Option;
using w2.Domain.ShopOperator;

/// <summary>
/// BaseUserControl の概要の説明です
/// </summary>
public class BaseUserControl : System.Web.UI.UserControl
{
	/// <summary>商品IDを含まないバリエーションID</summary>
	protected const string FIELD_PRODUCTVARIATION_V_ID = "v_id";

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
	/// <summary>アクションステータス</summary>
	protected string ActionStatus
	{
		get { return (string)Request[Constants.REQUEST_KEY_ACTION_STATUS]; }
	}
	/// <summary>商品税込み表示フラグ</summary>
	public bool ProductIncludedTaxFlg
	{
		get { return Constants.MANAGEMENT_INCLUDED_TAX_FLAG; }
	}
	/// <summary>商品価格区分表示文言</summary>
	public string ProductPriceTextPrefix
	{
		get { return TaxCalculationUtility.GetTaxTypeText(); }
	}
}
