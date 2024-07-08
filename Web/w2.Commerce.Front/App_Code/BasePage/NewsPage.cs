/*
=========================================================================================================
  Module      : 新着系基底ページ(NewsPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using w2.Domain.News;

///*********************************************************************************************
/// <summary>
/// 新着系基底ページ
/// </summary>
///*********************************************************************************************
public class NewsPage : BasePage
{
	/// <summary>店舗ID</summary>
	protected string ShopId
	{
		get { return StringUtility.ToValue(Request[Constants.REQUEST_KEY_SHOP_ID], Constants.CONST_DEFAULT_SHOP_ID).ToString(); }
	}
}
