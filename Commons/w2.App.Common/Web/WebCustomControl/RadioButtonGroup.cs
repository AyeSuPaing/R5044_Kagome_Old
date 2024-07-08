/*
=========================================================================================================
  Module      : リピータデータバインド内でもGroupNameが設定可能なラジオボタン
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
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
	/// ラジオボタングループコントロール
	/// </summary>
	[ToolboxData("<{0}:RadioButtonGroup runat=server></{0}:RadioButtonGroup>")]
	public class RadioButtonGroup : RadioButton
	{
		/// <summary>
		/// POSTデータをもとに、コントロールの状態を復元する
		/// </summary>
		/// <param name="strPostDataKey">想定するキー</param>
		/// <param name="nvcPostCollection">POSTデータが含まれるコレクション</param>
		/// <returns>コントロールの状態が変更された(～Changedイベントを発生させる必要がある)かどうか</returns>
		protected override bool LoadPostData(string strPostDataKey, System.Collections.Specialized.NameValueCollection nvcPostCollection)
		{
			// このコントロールが選択されたなら
			bool blNewChecked = (nvcPostCollection[this.GroupName] == this.UniqueID);
			if (this.Checked != blNewChecked)
			{
				this.Checked = blNewChecked;
				// コントロールが新たに選択されたときのみtrueを返す
				return blNewChecked;
			}
			return false;
		}

		/// <summary>
		/// コントロールを出力する
		/// </summary>
		/// <param name="htwWriter">出力先</param>
		protected override void Render(HtmlTextWriter htwWriter)
		{
			// 出力先を独自のTextWriterとして出力する
			base.Render(new WriterWrapper(htwWriter, this));
		}

		///*********************************************************************************************
		/// <summary>
		/// 属性を変更して出力するためのTextWriter
		/// </summary>
		///*********************************************************************************************
		private class WriterWrapper : HtmlTextWriter
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="htwWriter"></param>
			/// <param name="rbgControl"></param>
			public WriterWrapper(HtmlTextWriter htwWriter, RadioButtonGroup rbgControl)
				: base(htwWriter)
			{
				this.Control = rbgControl;
			}

			/// <summary>
			/// 属性の追加
			/// </summary>
			/// <param name="htwaKey">属性を表す列挙値</param>
			/// <param name="strValue">属性に設定する値</param>
			/// <remarks>
			/// RadioButtonが属性の設定のために本メソッドを呼び出すため、本メソッドで変更したい属性値を設定する
			/// </remarks>
			public override void AddAttribute(HtmlTextWriterAttribute htwaKey, string strValue)
			{
				switch (htwaKey)
				{
					case HtmlTextWriterAttribute.Name:
						// ラジオボタンの"name"属性書き込み時にはGroupNameを出力する
						base.AddAttribute(htwaKey, this.Control.GroupName);
						break;

					case HtmlTextWriterAttribute.Value:
						// POST時に出力する値は(コントロールを識別する必要があるため)ユニークとする
						base.AddAttribute(htwaKey, this.Control.UniqueID);
						break;

					default:
						base.AddAttribute(htwaKey, strValue);
						break;
				}
			}

			/// <summary>対象のコントロール</summary>
			private RadioButtonGroup Control { get; set; }
		}
	}
}