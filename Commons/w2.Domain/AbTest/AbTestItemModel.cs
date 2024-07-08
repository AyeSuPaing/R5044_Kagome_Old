/*
=========================================================================================================
  Module      : ABテストアイテムモデル (AbTestItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.AbTest
{
	/// <summary>
	/// ABテストアイテムモデル
	/// </summary>
	[Serializable]
	public class AbTestItemModel : ModelBase<AbTestItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AbTestItemModel()
		{
			this.AbTestId = "";
			this.ItemNo = "";
			this.PageId = "";
			this.DistributionRate = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AbTestItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AbTestItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ABテストID</summary>
		public string AbTestId
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTESTITEM_AB_TEST_ID]; }
			set { this.DataSource[Constants.FIELD_ABTESTITEM_AB_TEST_ID] = value; }
		}
		/// <summary>アイテムNO</summary>
		public string ItemNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTESTITEM_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_ABTESTITEM_ITEM_NO] = value; }
		}
		/// <summary>ランディングページID</summary>
		public string PageId
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTESTITEM_PAGE_ID]; }
			set { this.DataSource[Constants.FIELD_ABTESTITEM_PAGE_ID] = value; }
		}
		/// <summary>振り分け比率</summary>
		public int DistributionRate
		{
			get { return (int)this.DataSource[Constants.FIELD_ABTESTITEM_DISTRIBUTION_RATE]; }
			set { this.DataSource[Constants.FIELD_ABTESTITEM_DISTRIBUTION_RATE] = value; }
		}
		#endregion
	}
}