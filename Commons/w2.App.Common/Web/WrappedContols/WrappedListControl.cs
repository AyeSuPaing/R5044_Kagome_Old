/*
=========================================================================================================
  Module      : ラップ済リストコントロール(WrappedListControl.cs)
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
	/// ラップ済リストコントロール
	/// </summary>
	public abstract class WrappedListControl : WrappedWebControl
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parentControlID">親コントロールID</param>
		/// <param name="controlID">コントロールID</param>
		/// <param name="hfForm">コントロールが属するForm</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedListControl(
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
		/// <param name="defaultValue">デフォルト値</param>
		/// <param name="sbViewState">ViewState</param>
		public WrappedListControl(
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
		/// リストアイテムの値と指定した値を比較し、一致した場合選択する
		/// (リストアイテムのSelectedプロパティをtrueにする)
		/// </summary>
		/// <param name="strValue">指定する値</param>
		public void SelectItemByValue(string strValue)
		{
			foreach (ListItem li in this.Items)
			{
				SelectItem(li, li.Value, strValue);
			}
		}

		/// <summary>
		/// リストアイテムのテキストと指定した値を比較し、一致した場合選択する
		/// (リストアイテムのSelectedプロパティをtrueにする)
		/// </summary>
		/// <param name="strValue">指定する値</param>
		public void SelectItemByText(string strText)
		{
			foreach (ListItem li in this.Items)
			{
				SelectItem(li, li.Text, strText);
			}
		}

		/// <summary>
		/// リストアイテムのテキストと指定した値を比較し、一致した場合選択する(InnerControlに関わらずSelectedValueを設定)
		/// (リストアイテムのSelectedプロパティをtrueにする)
		/// </summary>
		/// <param name="text">指定する値</param>
		public void ForceSelectItemByText(string text)
		{
			foreach (ListItem li in this.Items)
			{
				ForceSelectItem(li, li.Text, text);
			}
		}

		/// <summary>
		/// リストアイテムを選択する
		/// </summary>
		/// <param name="li">選択対象のリストアイテム</param>
		/// <param name="strItem">リストアイテム内の比較対象</param>
		/// <param name="strValue">比較する値</param>
		private void SelectItem(ListItem li, string strItem, string strValue)
		{
			li.Selected = (strItem == strValue);

			// InnerControlが存在しない場合
			if (li.Selected && this.InnerControl == null)
			{
				// SelectedValue(viewstate)にセットしないとPostBack後に値が引き継げない
				this.SelectedValue = li.Value;
			}
		}

		/// <summary>
		/// リストアイテムを選択する（InnerControlに関わらずSelectedValueを設定）
		/// </summary>
		/// <param name="li">選択対象のリストアイテム</param>
		/// <param name="item">リストアイテム内の比較対象</param>
		/// <param name="value">比較する値</param>
		private void ForceSelectItem(ListItem li, string item, string value)
		{
			li.Selected = (item == value);

			if (li.Selected)
			{
				// SelectedValue(viewstate)にセットしないとPostBack後に値が引き継げない
				this.SelectedValue = li.Value;
			}
		}

		/// <summary>
		/// アイテムを追加
		/// </summary>
		/// <param name="li">追加するアイテム</param>
		public void AddItem(ListItem li)
		{
			this.Items.Add(li);
		}
		/// <summary>
		/// アイテムを追加
		/// </summary>
		/// <param name="strItem">追加するアイテム</param>
		public void AddItem(string strItem)
		{
			this.Items.Add(strItem);
		}
		/// <summary>
		/// アイテムを追加
		/// </summary>
		/// <param name="lis">追加するアイテムの配列</param>
		public void AddItems(ListItem[] lis)
		{
			this.Items.AddRange(lis);
		}

		/// <summary>
		/// アイテムを削除
		/// </summary>
		public void ClearItems()
		{
			m_licItems = null;
		}

		/// <summary>内部コントロール取得</summary>
		public new ListControl InnerControl
		{
			get { return (ListControl)base.InnerControl; }
		}
		/// <summary>選択値</summary>
		public string SelectedValue
		{
			get
			{
				if (this.InnerControl != null)
				{
					return this.InnerControl.SelectedValue;
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
					this.InnerControl.SelectedValue = value;
				}
				else
				{
					this.ViewState[this.UniqueValueName] = value;
				}
			}
		}
		/// <summary>選択アイテムのテキスト</summary>
		public string SelectedText
		{
			get
			{
				if (this.InnerControl != null)
				{
					return (this.InnerControl.SelectedItem != null) ? this.InnerControl.SelectedItem.Text : "";
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
					this.InnerControl.SelectedValue = value;
				}
				else
				{
					this.ViewState[this.UniqueValueName] = value;
				}
			}
		}
		/// <summary>選択インデックス</summary>
		public int SelectedIndex
		{
			get
			{
				return (this.InnerControl != null) ?this.InnerControl.SelectedIndex : -1;
			}
			set
			{
				if (this.InnerControl != null)
				{
					this.InnerControl.SelectedIndex = value;
				}
			}
		}
		/// <summary>選択アイテム</summary>
		public ListItem SelectedItem
		{
			get
			{
				return (this.InnerControl != null) ? this.InnerControl.SelectedItem : new ListItem();
			}
		}
		/// <summary>リストアイテム</summary>
		/// <remarks>
		/// InnerControlが存在しない場合、Itemsを設定する際に追加で処理が必要になる。
		/// そのため、Itemsプロパティを隠蔽し、Itemsプロパティを使用するメソッドを提供することで使用する際のミスを防ぐ
		/// </remarks>
		public ListItemCollection Items
		{
			get
			{
				// リストにどんどん値を追加していく処理でコントロールがない場合のパフォーマンスを考えて一度作成した
				// ListItemCollectionはプロパティに保持しておく。
				if (m_licItems == null)
				{
					m_licItems = (this.InnerControl != null) ? this.InnerControl.Items : new ListItemCollection();
				}
				return m_licItems;
			}
		}
		private ListItemCollection m_licItems = null;
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
					return (string)this.ViewState[this.UniqueValueName + "_text"];
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
					this.ViewState[this.UniqueValueName + "_text"] = value;
				}
			}
		}

		/// <summary>データソース</summary>
		public object DataSource
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