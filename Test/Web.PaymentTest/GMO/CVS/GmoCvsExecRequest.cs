/*
=========================================================================================================
  Module      : GmoCvsExecRequest(GmoCvsExecRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;

public partial class GmoCvsExecRequest : GmoCvsApi
{
	/// <summary>
	/// Page Load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var segments = HttpContext.Current.Request.Url.Segments;
		var pageRequest = segments[segments.Length - 1].Split('.')[0];
		ExecRequest(pageRequest);
	}
}