/*
=========================================================================================================
  Module      : ラップ済リピータ(WrappedTextBox.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace w2.App.Common.Web.WrappedContols
{
	/// <summary>
	/// ラップ済リピータ
	/// </summary>
	public class WrappedRepeater : WrappedControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedRepeater(
			string parentControlID,
			string controlID,
			HtmlForm hfForm,
			StateBag sbViewState)
			: base(
				parentControlID,
				controlID,
				hfForm,
				sbViewState,
				"")
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
		/// <param name="objDefaultValue">デフォルト値</param>
		public WrappedRepeater(
			string parentControlID,
			string controlID,
			HtmlForm hfForm,
			StateBag sbViewState,
			object objDefaultValue)
			: base(
				parentControlID,
				controlID,
				hfForm,
				sbViewState,
				objDefaultValue)
		{
			// 何もしない //
		}

		/// <summary>
		/// データバインド
		/// </summary>
		public override void DataBind()
		{
			if (this.InnerControl != null)
			{
				this.InnerControl.DataBind();
			}
		}

		/// <summary>内部コントロール</summary>
		public new Repeater InnerControl { get { return (Repeater)base.InnerControl; } }
		/// <summary>データソース</summary>
		public object DataSource
		{
			get
			{
				if (this.InnerControl != null)
				{
					return this.InnerControl.DataSource;
				}

				return null;
			}
			set
			{
				if (this.InnerControl != null)
				{
					this.InnerControl.DataSource = value;
				}
			}
		}
		/// <summary>アイテム</summary>
		public RepeaterItemCollection Items
		{
			get
			{
				if (this.InnerControl != null)
				{
					return this.InnerControl.Items;
				}

				return new RepeaterItemCollection(new ArrayList(0));
			}
		}
		/// <summary>UI としてページに表示するか？</summary>
		public override bool Visible
		{
			get
			{
				if (this.InnerControl != null)
				{
					return this.InnerControl.Visible;
				}

				return false;
			}
			set
			{
				if (this.InnerControl != null)
				{
					this.InnerControl.Visible = value;
				}
			}
		}
		/// <summary>
		/// Repeater コントロールの子コントロールを格納している System.Web.UI.ControlCollection を取得します。
		/// </summary>
		public new ControlCollection Controls
		{
			get
			{
				if (this.InnerControl != null)
				{
					return this.InnerControl.Controls;
				}

				return new ControlCollection(new Control());
			}
		}
	}
}