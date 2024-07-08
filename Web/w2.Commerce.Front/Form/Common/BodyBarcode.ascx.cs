/*
=========================================================================================================
  Module      : Body Barcode (BodyBarcode.ascx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Web.Process;
using w2.Domain.User;

public partial class Form_Common_BodyBarcode : BaseUserControl
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
	/// <param name="userExtendModel">ユーザー拡張項目モデル</param>
	/// <returns>CROSS POINT ユーザー拡張項目(店舗カード番号)</returns>
	protected string GetCrossPointShopCardNo(UserExtendModel userExtendModel)
	{
		return this.Process.GetCrossPointShopCardNo(userExtendModel);
	}

	/// <summary>プロセス</summary>
	private new CrossPointProcess Process
	{
		get { return (CrossPointProcess)this.ProcessTemp; }
	}
	/// <summary>プロセステンポラリ</summary>
	protected override IPageProcess ProcessTemp
	{
		get
		{
			if (m_processTmp == null) m_processTmp = new CrossPointProcess(this, this.ViewState, this.Context);
			return m_processTmp;
		}
	}
}