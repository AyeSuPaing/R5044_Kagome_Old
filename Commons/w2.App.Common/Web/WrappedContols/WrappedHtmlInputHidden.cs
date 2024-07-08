/*
=========================================================================================================
  Module      : Wrapped Html Input Hidden (WrappedHtmlInputHidden.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace w2.App.Common.Web.WrappedContols
{
	/// <summary>
	/// Wrapped html input hidden
	/// </summary>
	public class WrappedHtmlInputHidden : WrappedControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールのID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedHtmlInputHidden(
			string parentControlID,
			string controlID,
			HtmlForm hfForm,
			StateBag sbViewState)
			: base(parentControlID, controlID, hfForm, sbViewState)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールのID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		/// <param name="defaultValue">デフォルト値</param>
		public WrappedHtmlInputHidden(
			string parentControlID,
			string controlID,
			HtmlForm hfForm,
			StateBag sbViewState,
			string defaultValue)
			: base(parentControlID, controlID, hfForm, sbViewState, defaultValue)
		{
		}

		/// <summary>内部コントロール取得</summary>
		public new HtmlInputHidden InnerControl
		{
			get { return (HtmlInputHidden)base.InnerControl; } 
		}
		/// <summary>値</summary>
		public string Value
		{
			get
			{
				if (this.InnerControl != null)
				{
					return this.InnerControl.Value;
				}
				else
				{
					return (string)this.ViewState[this.UniqueValueName];
				}
			}
			set
			{
				if (this.InnerControl != null)
				{
					this.InnerControl.Value = value;
				}
				else
				{
					this.ViewState[this.UniqueValueName] = value;
				}
			}
		}
	}
}