/*
=========================================================================================================
  Module      : 新着情報マスタモデル (NewsModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.News
{
	/// <summary>
	/// 新着情報マスタモデル
	/// </summary>
	[Serializable]
	public partial class NewsModel : ModelBase<NewsModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public NewsModel()
		{
			this.NewsTextKbn = Constants.FLG_NEWS_NEWS_TEXT_KBN_TEXT;
			this.NewsTextKbnMobile = Constants.FLG_NEWS_NEWS_TEXT_KBN_MOBILE_TEXT;
			this.DispFlg = Constants.FLG_NEWS_DISP_FLG_ALL;
			this.MobileDispFlg = Constants.FLG_NEWS_MOBILE_DISP_FLG_ALL;
			this.DisplayOrder = Constants.FLG_NEWS_DISPLAY_ORDER_DEFAULT;
			this.ValidFlg = Constants.FLG_NEWS_VALID_FLG_VALID;
			this.DelFlg = Constants.FLG_NEWS_DEL_FLG_OFF;
			this.DisplayDateTo = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public NewsModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public NewsModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
			this.DisplayDate = this.DisplayDateFrom;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_NEWS_SHOP_ID] = value; }
		}
		/// <summary>新着ID</summary>
		public string NewsId
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_ID]; }
			set { this.DataSource[Constants.FIELD_NEWS_NEWS_ID] = value; }
		}
		/// <summary>表示日付（From）</summary>
		public DateTime DisplayDateFrom
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_NEWS_DISPLAY_DATE_FROM]; }
			set { this.DataSource[Constants.FIELD_NEWS_DISPLAY_DATE_FROM] = value; }
		}
		/// <summary>本文区分</summary>
		public string NewsTextKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_KBN]; }
			set { this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_KBN] = value; }
		}
		/// <summary>本文</summary>
		public string NewsText
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT]; }
			set { this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT] = value; }
		}
		/// <summary>モバイル本文区分</summary>
		public string NewsTextKbnMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_KBN_MOBILE]; }
			set { this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_KBN_MOBILE] = value; }
		}
		/// <summary>モバイル本文</summary>
		public string NewsTextMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_MOBILE]; }
			set { this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_MOBILE] = value; }
		}
		/// <summary>表示フラグ</summary>
		public string DispFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_DISP_FLG]; }
			set { this.DataSource[Constants.FIELD_NEWS_DISP_FLG] = value; }
		}
		/// <summary>モバイル表示フラグ</summary>
		public string MobileDispFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_MOBILE_DISP_FLG]; }
			set { this.DataSource[Constants.FIELD_NEWS_MOBILE_DISP_FLG] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_NEWS_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_NEWS_DISPLAY_ORDER] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_NEWS_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_NEWS_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_NEWS_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_NEWS_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_NEWS_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NEWS_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NEWS_LAST_CHANGED] = value; }
		}
		/// <summary>ブランドID</summary>
		public string BrandId
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_BRAND_ID]; }
			set { this.DataSource[Constants.FIELD_NEWS_BRAND_ID] = value; }
		}
		/// <summary>表示日付（To）</summary>
		public DateTime? DisplayDateTo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_NEWS_DISPLAY_DATE_TO] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_NEWS_DISPLAY_DATE_TO];
			}
			set { this.DataSource[Constants.FIELD_NEWS_DISPLAY_DATE_TO] = value; }
		}
		#endregion
	}
}
