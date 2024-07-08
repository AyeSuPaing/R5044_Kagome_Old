/*
=========================================================================================================
  Module      : ラップ済テキストボックス(WrappedTextBox.cs)
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
	/// ラップ済テキストボックス
	/// </summary>
	public class WrappedTextBox : WrappedWebControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedTextBox(
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
		public WrappedTextBox(
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
		public new TextBox InnerControl { get { return (TextBox)base.InnerControl; } }
		/// <summary>属性 (表示専用) のコレクション</summary>
		public AttributeCollection Attributes
		{
			get 
			{
				if (m_acAttributes == null)
				{
					m_acAttributes = (this.InnerControl != null) ? this.InnerControl.Attributes : new AttributeCollection(this.ViewState); 
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
		public TextBoxMode TextMode
		{
			get
			{
				return (this.InnerControl != null) ? this.InnerControl.TextMode : TextBoxMode.SingleLine;
			}
			set
			{
				if (this.InnerControl != null)
				{
					this.InnerControl.TextMode = value;
				}
			}
		}
		/// <summary>最大文字数</summary>
		public int MaxLength
		{
			get
			{
				return (this.InnerControl != null) ? this.InnerControl.MaxLength : 0;
			}
			set
			{
				if (this.InnerControl != null)
				{
					this.InnerControl.MaxLength = value;
				}
			}
		}
		/// <summary>Enaled</summary>
		public bool Enabled
		{
			get { return this.InnerControl.Enabled = (this.InnerControl != null); }
			set { if (this.InnerControl != null) this.InnerControl.Enabled = value; }
		}
	}
}