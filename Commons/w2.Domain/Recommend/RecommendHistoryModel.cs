/*
=========================================================================================================
  Module      : レコメンド表示履歴モデル (RecommendHistoryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Recommend
{
	/// <summary>
	/// レコメンド表示履歴モデル
	/// </summary>
	[Serializable]
	public partial class RecommendHistoryModel : ModelBase<RecommendHistoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public RecommendHistoryModel()
		{
			this.ShopId = string.Empty;
			this.RecommendId = string.Empty;
			this.UserId = string.Empty;
			this.RecommendHistoryNo = 0;
			this.TargetOrderId = string.Empty;
			this.OrderedFlg = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendHistoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendHistoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDHISTORY_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDHISTORY_SHOP_ID] = value; }
		}
		/// <summary>レコメンドID</summary>
		public string RecommendId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDHISTORY_RECOMMEND_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDHISTORY_RECOMMEND_ID] = value; }
		}
		/// <summary>ユーザーID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDHISTORY_USER_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDHISTORY_USER_ID] = value; }
		}
		/// <summary>履歴枝番</summary>
		public int RecommendHistoryNo
		{
			get { return (int)this.DataSource[Constants.FIELD_RECOMMENDHISTORY_RECOMMEND_HISTORY_NO]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDHISTORY_RECOMMEND_HISTORY_NO] = value; }
		}
		/// <summary>レコメンド表示対象注文ID</summary>
		public string TargetOrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDHISTROY_TARGET_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDHISTROY_TARGET_ORDER_ID] = value; }
		}
		/// <summary>表示区分</summary>
		public string DisplayKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDHISTORY_DISPLAY_KBN]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDHISTORY_DISPLAY_KBN] = value; }
		}
		/// <summary>購入フラグ</summary>
		public string OrderedFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDHISTORY_ORDERED_FLG]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDHISTORY_ORDERED_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_RECOMMENDHISTROY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDHISTROY_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_RECOMMENDHISTORY_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDHISTORY_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMENDHISTORY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_RECOMMENDHISTORY_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
