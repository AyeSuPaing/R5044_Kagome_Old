/*
=========================================================================================================
  Module      : 共有情報管理基底ページ(ShareInfoSettingPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.Cs.ShareInfo;

/// <summary>
/// ShareInfoSettingPage の概要の説明です
/// </summary>
public class ShareInfoSettingPage : BasePage
{
	#region プロパティ
	/// <summary>共有情報</summary>
	protected CsShareInfoModel ShareInfo
	{
		get { return (CsShareInfoModel)ViewState[Constants.SESSION_KEY_SHAREINFO_INFO]; }
		set { ViewState[Constants.SESSION_KEY_SHAREINFO_INFO] = value; }
	}
	/// <summary>共有既読情報</summary>
	protected CsShareInfoReadModel[] ShareInfoReads
	{
		get { return (CsShareInfoReadModel[])ViewState[Constants.SESSION_KEY_SHAREINFOREAD_INFO]; }
		set { ViewState[Constants.SESSION_KEY_SHAREINFOREAD_INFO] = value; }
	}
	#endregion
}
