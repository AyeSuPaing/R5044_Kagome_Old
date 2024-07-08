/*
=========================================================================================================
  Module      : トップ画面処理(Default.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Text;
using w2.Domain.UpdateHistory;

public partial class Default : BasePage
{
	/// <summary>リピートプラスONEリダイレクト必須判定</summary>
	public override bool RepeatPlusOneNeedsRedirect { get { return Constants.REPEATPLUSONE_OPTION_ENABLED; } }
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Http; } }	// Httpアクセス

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// モバイルチェック
		//------------------------------------------------------
		MobileCheck();

		//------------------------------------------------------
		// ブランドチェック
		//------------------------------------------------------
		BrandCheck();

		//------------------------------------------------------
		// ブランドタイトル設定
		//------------------------------------------------------
		if (this.BrandId != "")
		{
			this.Title = this.BrandTitle;
		}
	}

	/// <summary>
	/// モバイル用URL取得
	/// </summary>
	/// <returns></returns>
	protected string GetMobileUrl()
	{
		StringBuilder mobileUrl = new StringBuilder();
		mobileUrl.Append(Constants.PROTOCOL_HTTP).Append(Request.Url.Authority);
		mobileUrl.Append(Constants.PATH_MOBILESITE).Append(Constants.PAGE_MFRONT_TOP);
		mobileUrl.Append("?").Append(Constants.REQUEST_KEY_MFRONT_PAGE_ID).Append("=").Append(Constants.PAGEID_MFRONT_TOP);
		mobileUrl.Append("&").Append(Constants.REQUEST_KEY_BRAND_ID).Append("=").Append(this.BrandId);

		return mobileUrl.ToString();
	}
}
