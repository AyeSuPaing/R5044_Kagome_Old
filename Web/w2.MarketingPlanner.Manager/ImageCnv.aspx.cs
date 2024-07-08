/*
=========================================================================================================
  Module      : 画像変換ページ処理(ImageCnv.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Drawing.Imaging;
using System.IO;
using w2.App.Common.Product;

public partial class ImageCnv : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		string fullPath = Server.MapPath(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_IMGCNV_FILE]));
		byte[] buffer = ProductImage.GetConvertToBytes(
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_IMGCNV_SIZE]),
			StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_IMGCNV_FORMAT]).ToLower(),
			fullPath);

		Response.Buffer = true;
		Response.Clear();
		Response.AddHeader("Content-Disposition", "inline;filename=" + Path.GetFileName(fullPath));
		Response.ContentType = ProductImage.GetImageConverterFormat(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_IMGCNV_FORMAT]).ToLower());
		Response.OutputStream.Write(buffer, 0, buffer.Length);
		Response.Flush();
	}
}