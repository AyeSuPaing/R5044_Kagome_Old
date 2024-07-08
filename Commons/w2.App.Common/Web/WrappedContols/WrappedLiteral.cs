/*
=========================================================================================================
  Module      : ラップ済リテラル(WrappedLiteral.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/

using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace w2.App.Common.Web.WrappedContols
{
	/// <summary>
	/// ラップ済テキストボックス
	/// </summary>
	public class WrappedLiteral : WrappedControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedLiteral(
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
		public WrappedLiteral(
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
			// なにもしない //
		}

		/// <summary>内部コントロール取得</summary>
		public new Literal InnerControl { get { return (Literal)base.InnerControl; } }
		/// <summary>テキスト</summary>
		public string Text
		{
			get
			{
				if (this.InnerControl != null)
				{
					return this.InnerControl.Text;
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
					this.InnerControl.Text = value;
				}
				else
				{
					this.ViewState[this.UniqueValueName] = value;
				}
			}
		}
		/// <summary>テキストモード</summary>
		public LiteralMode Mode
		{
			get
			{
				return (this.InnerControl != null) ? this.InnerControl.Mode : LiteralMode.Transform;
			}
			set
			{
				if (this.InnerControl != null)
				{
					this.InnerControl.Mode = value;
				}
			}
		}
	}
}