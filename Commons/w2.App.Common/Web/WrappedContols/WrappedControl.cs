/*
=========================================================================================================
  Module      : ラップ済WEBコントロール(WrappedControl.cs)
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
	/// ラップ済コントロール
	/// </summary>
	///*********************************************************************************************
	public class WrappedControl : Control
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedControl(
			string parentControlID,
			string controlID,
			HtmlForm hfForm,
			StateBag sbViewState)
			: this(
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
		public WrappedControl(
			string parentControlID,
			string controlID,
			HtmlForm hfForm,
			StateBag sbViewState,
			object objDefaultValue)
		{
			this.UniqueValueName = parentControlID + "->" + controlID + ".Value";
			var cParentControl = hfForm.FindControl(parentControlID);
			if ((cParentControl != null)
				&& cParentControl.FindControl(controlID) != null)
			{
				this.UniqueId = cParentControl.FindControl(controlID).UniqueID;
			}
			else
			{
				this.UniqueId = "";
			}
			this.Form = hfForm;

			this.ViewState = sbViewState;
			if (this.ViewState[this.UniqueValueName] == null)
			{
				this.ViewState[this.UniqueValueName] = objDefaultValue;
			}
		}

		/// <summary>
		/// フォーカス
		/// </summary>
		public override void Focus()
		{
			if (this.InnerControl != null)
			{
				this.InnerControl.Focus();
			}
		}

		/// <summary>内部コントロール</summary>
		public bool HasInnerControl { get { return (this.InnerControl != null); } }
		/// <summary>内部コントロール</summary>
		public Control InnerControl { get { return this.Form.FindControl(this.UniqueId); } }
		/// <summary>コントロール識別名</summary>
		public string UniqueValueName { get; protected set; }
		/// <summary>ユニークID</summary>
		public string UniqueId { get; protected set; }
		/// <summary>ユニークID（オーバーライド）</summary>
		public override string UniqueID { get { return this.UniqueId; } }
		/// <summary>クライアントID</summary>
		public override string ClientID 
		{
			get
			{
				if (this.InnerControl != null)
				{
					return this.InnerControl.ClientID;
				}
				return "";
			}
		}
		/// <summary>フォーム</summary>
		public HtmlForm Form { get; private set; }
		/// <summary>親コントロール</summary>
		public override Control Parent
		{
			get
			{
				if ((this.InnerControl != null) && (this.InnerControl.Parent != null))
				{
					return this.InnerControl.Parent;
				}
				return new Control();
			}
		}
		/// <summary>ViewState</summary>
		public new StateBag ViewState { get; protected set; }
		/// <summary>表示・非表示</summary>
		public override bool Visible
		{
			get
			{
				if(this.InnerControl != null)
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
		/// データバインド
		/// </summary>
		public override void DataBind()
		{
			if (this.InnerControl != null)
			{
				this.InnerControl.DataBind();
			}
		}

	}
}
