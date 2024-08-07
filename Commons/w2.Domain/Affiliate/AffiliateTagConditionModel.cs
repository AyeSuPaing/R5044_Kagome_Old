/*
=========================================================================================================
  Module      : アフィリエイトタグの出力条件管理モデル (AffiliateTagConditionModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Affiliate
{
	/// <summary>
	/// アフィリエイトタグの出力条件管理モデル
	/// </summary>
	[Serializable]
	public partial class AffiliateTagConditionModel : ModelBase<AffiliateTagConditionModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AffiliateTagConditionModel()
		{
			this.MatchType = Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_PERFECT;
			this.ConditionType = string.Empty;
			this.ConditionSortNo = 1;
			this.ConditionValue = "";
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AffiliateTagConditionModel(DataRowView source) : this(source.ToHashtable())
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AffiliateTagConditionModel(Hashtable source) : this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>アフィリエイトID</summary>
		public int AffiliateId
		{
			get { return (int)this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_AFFILIATE_ID]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_AFFILIATE_ID] = value; }
		}
		/// <summary>条件タイプ</summary>
		public string ConditionType
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_TYPE]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_TYPE] = value; }
		}
		/// <summary>登録順序</summary>
		public int ConditionSortNo
		{
			get { return (int)this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_SORT_NO]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_SORT_NO] = value; }
		}
		/// <summary>内容</summary>
		public string ConditionValue
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_VALUE]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_CONDITION_VALUE] = value; }
		}
		/// <summary>一致条件タイプ</summary>
		public string MatchType
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_MATCH_TYPE]; }
			set { this.DataSource[Constants.FIELD_AFFILIATETAGCONDITION_MATCH_TYPE] = value; }
		}
		#endregion
	}
}