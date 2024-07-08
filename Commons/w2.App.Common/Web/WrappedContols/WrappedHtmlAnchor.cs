/*
=========================================================================================================
  Module      : ラップ済HTMLアンカー(WrappedHtmlAnchor.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/

using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace w2.App.Common.Web.WrappedContols
{
	/// <summary>
	/// ラップ済HTMLアンカー
	/// </summary>
	public class WrappedHtmlAnchor : WrappedControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールのID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedHtmlAnchor(
			string parentControlID,
			string controlID,
			HtmlForm hfForm,
			StateBag sbViewState)
			: base(
				parentControlID,
				controlID,
				hfForm,
				sbViewState)
		{
			// 何もしない //
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールのID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		/// <param name="defaultValue">デフォルト値</param>
		public WrappedHtmlAnchor(
			string parentControlID,
			string controlID,
			HtmlForm hfForm,
			StateBag sbViewState,
			string defaultValue)
			: base(
				parentControlID,
				controlID,
				hfForm,
				sbViewState,
				defaultValue)
		{
			// 何もしない //
		}

		/// <summary>内部コントロール取得</summary>
		public new HtmlAnchor InnerControl { get { return (HtmlAnchor)base.InnerControl; } }
		/// <summary>リンクURLターゲット</summary>
		public string HRef
		{
			get
			{
				if (this.InnerControl != null)
				{
					return this.InnerControl.HRef;
				}

				return "";
			}
			set
			{
				if (this.InnerControl != null)
				{
					this.InnerControl.HRef = value;
				}
			}
		}
	}
}