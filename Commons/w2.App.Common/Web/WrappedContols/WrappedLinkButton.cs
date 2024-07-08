/*
=========================================================================================================
  Module      : ラップ済リンクボタン(WrappedLinkButton.cs)
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
	/// ラップ済リンクボタン
	/// </summary>
	public class WrappedLinkButton : WrappedWebControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedLinkButton(
			string parentControlID,
			string controlID,
			HtmlForm hfForm,
			StateBag sbViewState)
			: base(
				parentControlID,
				controlID,
				hfForm,
				sbViewState,
				false)
		{
			// なにもしない //
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		/// <param name="defaultValue">デフォルト値</param>
		public WrappedLinkButton(
			string parentControlID,
			string controlID,
			HtmlForm hfForm,
			StateBag sbViewState,
			bool defaultValue)
			: base(
				parentControlID,
				controlID,
				hfForm,
				sbViewState,
				defaultValue)
		{
			// なにもしない //
		}

		/// <summary>内部コントロール取得</summary>
		public new LinkButton InnerControl
		{
			get { return (LinkButton)base.InnerControl; }
		}
		/// <summary>CommandArgument</summary>
		public string CommandArgument
		{
			get { return (this.InnerControl != null) ? this.InnerControl.CommandArgument : ""; }
			set { if (this.InnerControl != null) this.InnerControl.CommandArgument = value; }
		}
		/// <summary>OnClientClick</summary>
		public string OnClientClick
		{
			get { return (this.InnerControl != null) ? this.InnerControl.OnClientClick : ""; }
			set { if (this.InnerControl != null) this.InnerControl.OnClientClick = value; }
		}
		/// <summary>検証グループ</summary>
		public string ValidationGroup
		{
			get
			{
				return this.HasInnerControl
					? this.InnerControl.ValidationGroup
					: ((string)(this.ViewState[this.UniqueValueName + ".ValidationGroup"] ?? ""));
			}
			set
			{
				if (this.HasInnerControl) this.InnerControl.ValidationGroup = value;
				else this.ViewState[this.UniqueValueName + ".ValidationGroup"] = value;
			}
		}
		/// <summary>Enaled</summary>
		public bool Enabled
		{
			get { return this.InnerControl.Enabled = (this.InnerControl != null); }
			set { if (this.InnerControl != null) this.InnerControl.Enabled = value; }
		}
	}
}