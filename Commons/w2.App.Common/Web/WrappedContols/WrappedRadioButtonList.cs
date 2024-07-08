/*
=========================================================================================================
  Module      : ラップ済ラジオボタンリスト(WrappedRadioButtonList.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/

using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace w2.App.Common.Web.WrappedContols
{
	///*********************************************************************************************
	/// <summary>
	/// ラップ済ラジオボタンリスト
	/// </summary>
	///*********************************************************************************************
	public class WrappedRadioButtonList : WrappedListControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedRadioButtonList(
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
		public WrappedRadioButtonList(
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
		public new RadioButtonList InnerControl
		{
			get { return (RadioButtonList)base.InnerControl; }
		}
	}
}