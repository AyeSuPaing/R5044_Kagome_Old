/*
=========================================================================================================
  Module      : e-SCOTT 会員系リクエストレシーバ（MemberReceive.aspx.cs）
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.Payment.EScott;

public partial class EScott_MemberReceive : System.Web.UI.Page
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
		if (Request.InputStream.Length == 0)
		{
			Response.Write("ストリームが取得出来ませんでした。（直アクセス？）");
			return;
		}

		try
		{
			// body取得
			var body = Request.Form.ToString();
			var result = new EScottResponseCreator().CreateApiResponse(body);
			Response.Write(result);

		}
		catch (Exception ex)
		{
			Response.Write("リクエストデータの解析に失敗しました。：" + ex.ToString());
		}
    }
}