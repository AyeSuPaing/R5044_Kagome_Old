/*
=========================================================================================================
  Module      : ラップ済チェックボックスリスト(WrappedCheckBoxList.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace w2.App.Common.Web.WrappedContols
{
	///*********************************************************************************************
	/// <summary>
	/// ラップ済チェックボックスリスト
	/// </summary>
	///*********************************************************************************************
	public class WrappedCheckBoxList : WrappedListControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedCheckBoxList(
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
		//public WrappedCheckBoxList(
		//    string parentControlID,
		//    string controlID,
		//    HtmlForm hfForm,
		//    StateBag sbViewState,
		//    string defaultValue)
		//    : base(
		//        parentControlID,
		//        controlID,
		//        hfForm,
		//        sbViewState,
		//        defaultValue)
		//{
		//    // HACK: デフォルトが複数選択の場合、先頭値のみ返す
		//    // なにもしない //
		//}

		/// <summary>内部コントロール取得</summary>
		public new CheckBoxList InnerControl
		{
			get { return (CheckBoxList)base.InnerControl; }
		}
	}
}