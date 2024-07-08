/*
=========================================================================================================
  Module      : ラップ済ヒドゥンフィールド(WrappedHiddenField.cs)
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
	/// ラップ済ヒドゥンフィールド
	/// </summary>
	public class WrappedHiddenField : WrappedControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールのID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedHiddenField(
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
		/// <param name="defaultValue">デフォルト値</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedHiddenField(
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
		public new HiddenField InnerControl { get { return (HiddenField)base.InnerControl; } }
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