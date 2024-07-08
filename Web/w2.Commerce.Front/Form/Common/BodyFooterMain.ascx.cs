/*
=========================================================================================================
  Module      : 共通フッタ出力コントローラ処理(BodyFooterMain.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

public partial class Form_Common_BodyFooterMain : BaseUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		//なにもしない
	}

	/// <summary>遷移後URL</summary>
	protected string NextUrl
	{
		get
		{
			// 要求ページがログイン画面ではない場合、要求ページのURI絶対パスを遷移後URLとして返却
			if (Request.Url.AbsolutePath != (Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN))
			{
				return this.RawUrl;
			}

			// 既に遷移後URLが存在する場合、存在する遷移後URLを返却　そうでない場合、TOPページを遷移後URLとして返却
			return (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NEXT_URL]) != "") ? NextUrlValidation(Request[Constants.REQUEST_KEY_NEXT_URL]) : Constants.PATH_ROOT;
		}
	}
}
