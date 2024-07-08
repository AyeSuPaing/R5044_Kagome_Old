/*
=========================================================================================================
  Module      : ラップ済HTMLジェネリックコントロール(WrappedHtmlGenericControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/

using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace w2.App.Common.Web.WrappedContols
{
	/// <summary>
	/// ラップ済HTMLジェネリックコントロール
	/// </summary>
	public class WrappedHtmlGenericControl : WrappedControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールのID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedHtmlGenericControl(
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
		/// <param name="sbViewState">ViewState</param>
		/// <param name="defaultValue">デフォルト値</param>
		public WrappedHtmlGenericControl(
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
		public new HtmlGenericControl InnerControl { get { return (HtmlGenericControl)base.InnerControl; } }
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
		/// <summary>内部テキスト</summary>
		public string InnerText
		{
			get
			{
				if (this.InnerControl != null)
				{
					return this.InnerControl.InnerText;
				}

				return "";
			}
			set
			{
				if (this.InnerControl != null)
				{
					this.InnerControl.InnerText = value;
				}
			}
		}
		/// <summary>内部Html</summary>
		public string InnerHtml
		{
			get
			{
				if (this.InnerControl != null)
				{
					return this.InnerControl.InnerHtml;
				}

				return "";
			}
			set
			{
				if (this.InnerControl != null)
				{
					this.InnerControl.InnerHtml = value;
				}
			}
		}
	}
}