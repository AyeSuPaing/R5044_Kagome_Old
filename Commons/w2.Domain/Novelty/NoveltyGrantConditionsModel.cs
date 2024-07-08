/*
=========================================================================================================
  Module      : ノベルティ付与条件モデル (NoveltyGrantConditionsModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Novelty
{
	/// <summary>
	/// ノベルティ付与条件モデル
	/// </summary>
	[Serializable]
	public partial class NoveltyGrantConditionsModel : ModelBase<NoveltyGrantConditionsModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public NoveltyGrantConditionsModel()
		{
			this.ConditionNo = 1;
			this.AmountBegin = 0;
			this.GrantItemList = new NoveltyGrantItemModel[0];
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public NoveltyGrantConditionsModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public NoveltyGrantConditionsModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_SHOP_ID] = value; }
		}
		/// <summary>ノベルティID</summary>
		public string NoveltyId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_NOVELTY_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_NOVELTY_ID] = value; }
		}
		/// <summary>ノベルティ付与条件枝番</summary>
		public int ConditionNo
		{
			get { return (int)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_CONDITION_NO]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_CONDITION_NO] = value; }
		}
		/// <summary>対象会員</summary>
		public string UserRankId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_USER_RANK_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_USER_RANK_ID] = value; }
		}
		/// <summary>対象金額（以上）</summary>
		public decimal AmountBegin
		{
			get { return (decimal)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_AMOUNT_BEGIN]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_AMOUNT_BEGIN] = value; }
		}
		/// <summary>対象金額（以下）</summary>
		public decimal? AmountEnd
		{
			get
			{
				if (this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_AMOUNT_END] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_AMOUNT_END];
			}
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_AMOUNT_END] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NOVELTYGRANTCONDITIONS_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
