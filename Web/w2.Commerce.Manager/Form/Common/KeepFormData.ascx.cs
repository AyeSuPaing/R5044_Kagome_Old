/*
=========================================================================================================
  Module      : Keep Form Data When Back(KeepFormData.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;

/// <summary>
/// Control is use for storing data input in User Interface
/// </summary>
public partial class Form_Common_KeepFormData : UserControl
{
	/// <summary>Client Javascript Variable Name</summary>
	public const string CLIENT_VARIABLE_NAME = "w2Manager_FormData";

	/// <summary>
	/// Handles the Load event of the Page control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (IsPostBack)
		{
			var skipAspHiddenField = new List<string> { "__EVENTTARGET", "__EVENTARGUMENT", "__VIEWSTATE", "__EVENTVALIDATION", "__SCROLLPOSITIONX", "__SCROLLPOSITIONY", "__LASTFOCUS" };

			foreach (var key in Request.Form.AllKeys.Where(key => (string.IsNullOrEmpty(key) == false) && (skipAspHiddenField.Contains(key) == false)))
			{
				this.FormData.Add(new KeyValuePair<string, string>(key.Replace("$", "_"), Request.Form[key]));
			}

			Session[Request.Url.AbsolutePath] = this.FormData;
		}
	}

	/// <summary>
	/// Handles the PreRender event of the Page control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
	protected void Page_PreRender(object sender, EventArgs e)
	{
		if (!IsPostBack && this.AutoRestoreFormData) RestoreFormData();
	}

	/// <summary>
	/// Restores the form data.
	/// </summary>
	public void RestoreFormData()
	{
		if (Session[Request.Url.AbsolutePath] == null) return;

		this.AutoRestoreFormData = true;
		this.FormData = (List<KeyValuePair<string, string>>)Session[Request.Url.AbsolutePath];
		var jsonData = string.Format("var {0} = {1}", CLIENT_VARIABLE_NAME, (new JavaScriptSerializer()).Serialize(this.FormData));
		ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), CLIENT_VARIABLE_NAME, jsonData, true);
	}

	/// <summary> The form data </summary>
	public List<KeyValuePair<string, string>> FormData = new List<KeyValuePair<string, string>>();

	/// <summary> Gets or sets a value indicating whether [automatic restore data]. </summary>
	public bool AutoRestoreFormData { get; set; }
}