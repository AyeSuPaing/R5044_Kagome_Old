/*
=========================================================================================================
  Module      : ラップ済アップデートパネル(WrappedUpdatePanel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/

using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace w2.App.Common.Web.WrappedContols
{
	///*********************************************************************************************
	/// <summary>
	/// ラップ済アップデートパネル
	/// </summary>
	///*********************************************************************************************
	public class WrappedUpdatePanel : WrappedControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedUpdatePanel(
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
		public WrappedUpdatePanel(
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

		/// <summary>
		/// 更新
		/// </summary>
		public void Update()
		{
			if (this.InnerControl != null)
			{ 
				this.InnerControl.Update();
			}
		}

		/// <summary>内部コントロール取得</summary>
		public new UpdatePanel InnerControl
		{
			get { return (UpdatePanel)base.InnerControl; }
		}
		/// <summary>UpdatePanelのイベントトリガー</summary>
		public UpdatePanelTriggerCollection Triggers
		{
			get
			{
				return this.InnerControl != null
					? this.InnerControl.Triggers
					: new UpdatePanelTriggerCollection(new UpdatePanel());
			}
		}
	}
}
