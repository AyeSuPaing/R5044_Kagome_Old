/*
=========================================================================================================
  Module      : 拡張テキストボックスコントロール
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace w2.App.Common.Web.WebCustomControl
{
	/// <summary>
	/// 拡張テキストボックスコントロール
	/// </summary>
	[ToolboxData("<{0}:ExtendedTextBox runat=server></{0}:ExtendedTextBox>")]
	public class ExtendedTextBox : TextBox
	{
		/// <summary>
		/// コントロールを出力する
		/// </summary>
		/// <param name="htwWriter">出力先</param>
		protected override void Render(HtmlTextWriter htwWriter)
		{
			// 出力先を独自のTextWriterとして出力する
			base.Render(new WriterWrapper(htwWriter, this));
		}

		/// <summary>タイプ</summary>
		public string Type { get; set; }

		/// <summary>
		/// 属性を変更して出力するためのTextWriter
		/// </summary>
		private class WriterWrapper : HtmlTextWriter
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="htwWriter">HTMLテキストライタ</param>
			/// <param name="etbControl">コントロール</param>
			public WriterWrapper(HtmlTextWriter htwWriter, ExtendedTextBox etbControl)
				: base(htwWriter)
			{
				this.Control = etbControl;
			}

			/// <summary>
			/// 属性の追加
			/// </summary>
			/// <param name="attr">属性を表す列挙値</param>
			/// <param name="value">属性に設定する値</param>
			public override void AddAttribute(HtmlTextWriterAttribute attr, string value)
			{
				switch (attr)
				{
					case HtmlTextWriterAttribute.Type:
						base.AddAttribute(attr, this.Control.Type ?? value);
						break;

					default:
						base.AddAttribute(attr, value);
						break;
				}
			}

			/// <summary>対象のコントロール</summary>
			private ExtendedTextBox Control { get; set; }
		}
	}
}