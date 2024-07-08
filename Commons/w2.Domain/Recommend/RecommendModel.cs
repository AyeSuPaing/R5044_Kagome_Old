/*
=========================================================================================================
  Module      : レコメンド設定モデル (RecommendModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Recommend
{
	/// <summary>
	/// レコメンド設定モデル
	/// </summary>
	[Serializable]
	public partial class RecommendModel : ModelBase<RecommendModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public RecommendModel()
		{
			this.RecommendDisplayPage = Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_PAGE_ORDER_CONFIRM;
			this.RecommendKbn = Constants.FLG_RECOMMEND_RECOMMEND_KBN_UP_SELL;
			this.DateEnd = null;
			this.Priority = 1;
			this.ValidFlg = Constants.FLG_RECOMMEND_VALID_FLG_VALID;
			this.RecommendDisplayKbnPc = Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_KBN_PC_TEXT;
			this.RecommendDisplayKbnSp = Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_KBN_SP_TEXT;
			this.ApplyConditionItems = new RecommendApplyConditionItemModel[0];
			this.UpsellTargetItem = null;
			this.Items = new RecommendItemModel[0];
			this.OnetimeFlg = Constants.FLG_RECOMMEND_ONETIME_FLG_INVALID;
			this.ChatbotUseFlg = Constants.FLG_RECOMMEND_CHATBOT_USE_FLG_INVALID;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_SHOP_ID] = value; }
		}
		/// <summary>レコメンドID</summary>
		public string RecommendId
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_ID]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_ID] = value; }
		}
		/// <summary>レコメンド名（管理用）</summary>
		public string RecommendName
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_NAME]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_NAME] = value; }
		}
		/// <summary>説明（管理用）</summary>
		public string Discription
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_DISCRIPTION]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_DISCRIPTION] = value; }
		}
		/// <summary>レコメンド表示ページ</summary>
		public string RecommendDisplayPage
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_PAGE]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_PAGE] = value; }
		}
		/// <summary>レコメンド区分</summary>
		public string RecommendKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_KBN]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_KBN] = value; }
		}
		/// <summary>開始日時</summary>
		public DateTime DateBegin
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_RECOMMEND_DATE_BEGIN]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_DATE_BEGIN] = value; }
		}
		/// <summary>終了日時</summary>
		public DateTime? DateEnd
		{
			get
			{
				if (this.DataSource[Constants.FIELD_RECOMMEND_DATE_END] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_RECOMMEND_DATE_END];
			}
			set { this.DataSource[Constants.FIELD_RECOMMEND_DATE_END] = value; }
		}
		/// <summary>適用優先順</summary>
		public int Priority
		{
			get { return (int)this.DataSource[Constants.FIELD_RECOMMEND_PRIORITY]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_PRIORITY] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_VALID_FLG] = value; }
		}
		/// <summary>レコメンド表示区分PC</summary>
		public string RecommendDisplayKbnPc
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_KBN_PC]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_KBN_PC] = value; }
		}
		/// <summary>レコメンド表示PC</summary>
		public string RecommendDisplayPc
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_PC]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_PC] = value; }
		}
		/// <summary>レコメンド表示区分SP</summary>
		public string RecommendDisplayKbnSp
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_KBN_SP]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_KBN_SP] = value; }
		}
		/// <summary>レコメンド表示SP</summary>
		public string RecommendDisplaySp
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_SP]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_SP] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_RECOMMEND_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_RECOMMEND_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_LAST_CHANGED] = value; }
		}
		/// <summary>ワンタイム表示フラグ</summary>
		public string OnetimeFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_ONETIME_FLG]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_ONETIME_FLG] = value; }
		}
		/// <summary>Chat bot use flg</summary>
		public string ChatbotUseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_CHATBOT_USE_FLG]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_CHATBOT_USE_FLG] = value; }
		}
		#endregion
	}
}