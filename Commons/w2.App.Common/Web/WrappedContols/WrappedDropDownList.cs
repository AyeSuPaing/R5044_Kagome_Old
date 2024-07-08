/*
=========================================================================================================
  Module      : ラップ済ドロップダウンリスト(WrappedDropDownList.cs)
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
	/// ラップ済ドロップダウンリスト
	/// </summary>
	///*********************************************************************************************
	public class WrappedDropDownList : WrappedListControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedDropDownList(
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
		public WrappedDropDownList(
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

		/// <summary>内部コントロール取得</summary>
		public new DropDownList InnerControl
		{
			get { return (DropDownList)base.InnerControl; }
		}
		/// <summary>データソース</summary>
		public new object DataSource
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
	}
}