/*
=========================================================================================================
  Module      : Webカスタムコントロールサンプル
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace w2.App.Common.Web.WebCustomControl
{
	/// <summary>
	/// サンプル
	/// </summary>
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:WebCustomControlSample runat=server></{0}:WebCustomControl1>")]
	public class WebCustomControlSample : WebControl
	{
		/// <summary></summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string Text
		{
			get
			{
				string strTmp = (string)ViewState["Text"];
				return (strTmp ?? "");
			}

			set
			{
				ViewState["Text"] = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			RenderContents(writer);
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="output"></param>
		protected override void RenderContents(HtmlTextWriter output)
		{
			output.Write(Text);
		}
	}
}
