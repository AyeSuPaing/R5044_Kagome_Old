/*
=========================================================================================================
  Module      : HtmlFormAdapterクラス(HtmlFormAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.Adapters;
using System.Text;
using System.Text.RegularExpressions;

///*********************************************************************************************
/// <summary>
/// レンダラをカスタマイズするためのクラス
/// </summary>
///*********************************************************************************************
public class HtmlFormAdapter : ControlAdapter
{
	/// <summary>
	/// レンダラ（オーバーライド）
	/// </summary>
	/// <param name="writer">HtmlTextWriter</param>
	protected override void Render(HtmlTextWriter htwWriter)
	{
		if (htwWriter.GetType() == typeof(HtmlTextWriter))
		{
			base.Render(new HtmlTextWriterWrapper(htwWriter));
		}
		else
		{
			// 現行の方法でレンダリング（ASP.NET AJAX等で利用）
			base.Render(htwWriter);
		}
	}

	///*********************************************************************************************
	/// <summary>
	/// HtmlTextWriterWrapperクラス
	/// </summary>
	/// <remarks>
	/// ・フレンドリURLの場合に相対パスを絶対パスに書き換える（商品説明など。AJAX更新部分以外「../」「./」に対応）
	/// </remarks>
	///*********************************************************************************************
	internal class HtmlTextWriterWrapper : HtmlTextWriter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="htwWriter">HTMLテキストライタ</param>
		public HtmlTextWriterWrapper(HtmlTextWriter htwWriter)
			: base(htwWriter)
		{
			this.InnerWriter = htwWriter.InnerWriter;
		}

		/// <summary>
		/// 書き込み（オーバーライド）
		/// </summary>
		/// <param name="strValue">書き込み文字列</param>
		public override void Write(string strValue)
		{
			//------------------------------------------------------
			// フレンドリURLの場合は
			//------------------------------------------------------
			// フレンドリURL？（RawUrlをとAbsolutePath比較の場合、AJAXを検地できないためページ指定）
			if (Constants.FRIENDLY_URL_ENABLED
				&& (w2.Common.Web.WebUtility.GetRawUrl(HttpContext.Current.Request).StartsWith(HttpContext.Current.Request.Url.AbsolutePath) == false))
			{
				// 相対パスを含みそう？
				if ((strValue != null)
					&& (strValue.Contains("..")))
				{
					// 相対パスを絶対パス置換
					foreach (Match match in Regex.Matches(strValue, "[\"'][\\.]+/[^\"']*[\"']"))
					{
						strValue = strValue.Replace(
							match.Value,
							match.Value[0]
								+ new Uri(new Uri(HttpContext.Current.Request.Url.AbsoluteUri), match.Value.Substring(1, match.Value.Length - 2)).PathAndQuery
								+ match.Value[match.Value.Length - 1]);
					}
				}
			}
			this.InnerWriter.Write(strValue);
		}
	}
}
