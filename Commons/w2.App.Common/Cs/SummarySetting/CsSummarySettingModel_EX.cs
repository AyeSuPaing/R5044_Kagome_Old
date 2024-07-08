/*
=========================================================================================================
  Module      : 集計区分モデルのパーシャルクラス(CsSummarySettingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.SummarySetting
{
	/// <summary>
	/// 集計区分モデルのパーシャルクラス
	/// </summary>
	public partial class CsSummarySettingModel : ModelBase<CsSummarySettingModel>
	{
		#region プロパティ
		/// <summary>アイテム数(Serchで取得)</summary>
		public int EX_ItemCount
		{
			get { return (int)this.DataSource["item_count"]; }
		}
		/// <summary>集計区分アイテムモデル配列</summary>
		public CsSummarySettingItemModel[] EX_Items { get; set; }
		/// <summary>集計区分アイテムListItem配列</summary>
		public ListItem[] EX_ListItems
		{
			get { return (from item in this.EX_Items select new ListItem(item.SummarySettingItemText, item.SummarySettingItemId)).ToArray(); }
		}
		/// <summary>集計区分アイテムListItem配列（ドロップダウン用 ※先頭にブランク追加）</summary>
		public ListItem[] EX_ListItemsWithEmptyItem
		{
			get { return new ListItem[] { new ListItem("", "") }.Concat(this.EX_ListItems).ToArray(); }
		}
		/// <summary>集計区分入力種別表示文字列</summary>
		public string EX_SummarySettingTypeText
		{
			get { return ValueText.GetValueText(Constants.TABLE_CSSUMMARYSETTING, Constants.FIELD_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE, this.SummarySettingType); }
		}
		/// <summary>集計区分アイテム数表示文字列</summary>
		public string EX_ItemCountText
		{
			get { return (this.SummarySettingType == Constants.FLG_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE_TEXT) ? "-" : this.EX_ItemCount.ToString(); }
		}
		/// <summary>有効フラグ表示文字列</summary>
		public string EX_ValidFlgText
		{
			get { return ValueText.GetValueText(Constants.TABLE_CSSUMMARYSETTING, Constants.FIELD_CSSUMMARYSETTING_VALID_FLG, this.ValidFlg); }
		}
		#endregion
	}
}
