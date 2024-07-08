/*
=========================================================================================================
  Module      : ラップ済ラベルリテラル(WrappedLabel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace w2.App.Common.Web.WrappedContols
{
	/// <summary>
	/// ラップ済ラベルコントロール
	/// </summary>
	public class WrappedLabel : WrappedWebControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="stateBagViewState">ViewState</param>
		public WrappedLabel(
			string parentControlID,
			string controlID,
			HtmlForm hfForm,
			StateBag stateBagViewState)
			: base(
				parentControlID,
				controlID,
				hfForm,
				stateBagViewState)
		{
			// なにもしない //
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="stateBagViewState">ViewState</param>
		/// <param name="defaultValue">デフォルト値</param>
		public WrappedLabel(
			string parentControlID,
			string controlID,
			HtmlForm hfForm,
			StateBag stateBagViewState,
			string defaultValue)
			: base(
				parentControlID,
				controlID,
				hfForm,
				stateBagViewState,
				defaultValue)
		{
			// なにもしない //
		}

		/// <summary>内部コントロール取得</summary>
		public new Label InnerControl { get { return (Label)base.InnerControl; } }
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
	}
}