/*
=========================================================================================================
  Module      : ラップ済ラジオボタングループ(WrappedHtmlGenericControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System.Web.UI;
using System.Web.UI.HtmlControls;
using w2.App.Common.Web.WebCustomControl;

namespace w2.App.Common.Web.WrappedContols
{
	/// <summary>
	/// ラップ済ラジオボタングループ
	/// </summary>
	public class WrappedRadioButtonGroup : WrappedWebControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedRadioButtonGroup(
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
		public WrappedRadioButtonGroup(
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
		public new RadioButtonGroup InnerControl { get { return (RadioButtonGroup)base.InnerControl; } }
		/// <summary>チェック？</summary>
		public bool Checked
		{
			get
			{
				if(this.InnerControl != null)
				{
					return this.InnerControl.Checked;
				}

				return (bool)this.ViewState[this.UniqueValueName];
			}
			set 
			{
				if (this.InnerControl != null)
				{
					this.InnerControl.Checked = value;
				}
				else
				{
					this.ViewState[this.UniqueValueName] = value;
				}
			}
		}
	}
}