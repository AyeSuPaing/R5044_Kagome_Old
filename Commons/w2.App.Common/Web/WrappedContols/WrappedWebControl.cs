/*
=========================================================================================================
  Module      : ラップ済WEBコントロール(WrappedControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace w2.App.Common.Web.WrappedContols
{
	/// <summary>
	/// ラップ済Webコントロール
	/// </summary>
	public class WrappedWebControl : WrappedControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedWebControl(
			string parentControlID,
			string controlID,
			HtmlForm hfForm,
			StateBag sbViewState)
			: this(
				parentControlID,
				controlID,
				hfForm,
				sbViewState,
				"")
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		/// <param name="objDefaultValue">デフォルト値</param>
		public WrappedWebControl(
			string parentControlID,
			string controlID,
			HtmlForm hfForm,
			StateBag sbViewState,
			object objDefaultValue)
			: base(parentControlID, controlID, hfForm, sbViewState, objDefaultValue)
		{
		}

		/// <summary>内部コントロール</summary>
		public new WebControl InnerControl { get { return (WebControl)base.InnerControl; } }
		/// <summary>CSS CLASS</summary>
		public string CssClass
		{
			get { return (this.InnerControl != null) ? this.InnerControl.CssClass : ""; }
			set { if (this.InnerControl != null) { this.InnerControl.CssClass = value; } }
		}
	}
}
