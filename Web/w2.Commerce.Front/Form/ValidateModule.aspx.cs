/*
=========================================================================================================
  Module      : クライアント検証用検証モジュール処理(ValidateModule.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;

public partial class Form_ValidateModule : System.Web.UI.Page
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// キャッシュ無効
		//------------------------------------------------------
		Response.Expires = -1;
		Response.CacheControl = "no-cache";
		Response.AddHeader("Pragma", "no-cache");
		Response.ContentType = "text/html";

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		var validateGroup = StringUtility.ToEmpty(Request["group"]);
		var validateControl = StringUtility.ToEmpty(Request["control"]);
		var validateValue = StringUtility.ToEmpty(Request["value"]);
		var languageLocaleId = StringUtility.ToEmpty(Request["languageLocaleId"]);
		var countyIsoCode = StringUtility.ToEmpty(Request["countyIsoCode"]);

		//------------------------------------------------------
		// 元々のコントロール名を取得
		//------------------------------------------------------
		string[] controlElements = validateControl.Split('$');
		string validateControlForValidator = controlElements[controlElements.Length - 1];

		//------------------------------------------------------
		// 検証・結果出力
		//------------------------------------------------------
		string errorMessage = Validator.ValidateControl(
			validateGroup,
			validateControlForValidator,
			validateValue,
			languageLocaleId,
			countyIsoCode);

		Response.Clear();

		// 下記2件の不具合対応のためXMLヘッダを出力しておく（通常はXMLヘッダを省略できる）
		// ・MACの旧Safariでは日本語をajaxでやりとりすると文字化けしてしまう
		// ・Firefoxでは空文字をResponse.Writeするとエラーコンソールにエラーが出力されてしまう（WindowsServer2008R2＆IIS7.5環境で確認）
		Response.Write(Constants.CONST_INPUT_ERROR_XML_HEADER);

		Response.Write(WebSanitizer.HtmlEncodeChangeToBr(errorMessage));
		Response.Flush();
		Response.End();
	}
}
