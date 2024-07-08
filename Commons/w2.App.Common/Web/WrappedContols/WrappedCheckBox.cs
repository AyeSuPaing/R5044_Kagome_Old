/*
=========================================================================================================
  Module      : ラップ済チェックボックス(WrappedCheckBox.cs)
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
	/// ラップ済チェックボックス
	/// </summary>
	///*********************************************************************************************
	public class WrappedCheckBox : WrappedWebControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedCheckBox(
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
		public WrappedCheckBox(
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

		/// <summary>内部コントロール取得</summary>
		public new CheckBox InnerControl
		{
			get { return (CheckBox)base.InnerControl; }
		}
		/// <summary>属性 (表示専用) のコレクション</summary>
		public AttributeCollection Attributes
		{
			get
			{
				if (m_acAttributes == null)
				{
					m_acAttributes = (this.InnerControl != null)
						? this.InnerControl.Attributes
						: new AttributeCollection(this.ViewState);
				}

				return m_acAttributes;
			}
		}
		private AttributeCollection m_acAttributes = null;

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
					return "";
				}
			}
		}
		/// <summary>チェック状態</summary>
		public bool Checked
		{
			get
			{
				if (this.InnerControl != null)
				{
					return this.InnerControl.Checked;
				}
				else
				{
					return (bool)this.ViewState[this.UniqueValueName];
				}
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
		/// <summary>有効状態</summary>
		public bool Enabled
		{
			get
			{
				if (this.InnerControl != null)
				{
					return this.InnerControl.Enabled;
				}
				else
				{
					return (bool)this.ViewState[this.UniqueValueName];
				}
			}
			set
			{
				if (this.InnerControl != null)
				{
					this.InnerControl.Enabled = value;
				}
				else
				{
					this.ViewState[this.UniqueValueName] = value;
				}
			}
		}
	}
}