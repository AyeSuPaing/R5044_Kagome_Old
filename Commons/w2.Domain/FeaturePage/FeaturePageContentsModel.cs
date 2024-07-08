/*
=========================================================================================================
  Module      : 特集ページコンテンツモデル (FeaturePageContentsModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FeaturePage
{
	/// <summary>
	/// 特集ページコンテンツモデル
	/// </summary>
	public class FeaturePageContentsModel : ModelBase<FeaturePageContentsModel>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeaturePageContentsModel()
		{
			this.ContentsKbn = Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_PC;
			this.PageTitle = string.Empty;
			this.AltText = string.Empty;
			this.ProductGroupId = string.Empty;
			this.ProductListTitle = string.Empty;
			this.DispNum = 0;
			this.Pagination = Constants.FLG_FEATUREPAGECONTENTS_PAGINATION_FLG_OFF;
			this.LastChanged = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeaturePageContentsModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeaturePageContentsModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>特集ページID</summary>
		public long FeaturePageId
		{
			get { return (long)this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_FEATURE_PAGE_ID]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_FEATURE_PAGE_ID] = value; }
		}
		/// <summary>コンテンツ区分</summary>
		public string ContentsKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_CONTENTS_KBN]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_CONTENTS_KBN] = value; }
		}
		/// <summary>コンテンツタイプ</summary>
		public string ContentsType
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_CONTENTS_TYPE]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_CONTENTS_TYPE] = value; }
		}
		/// <summary>コンテンツ表示順序</summary>
		public int ContentsSortNumber
		{
			get { return (int)this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_CONTENTS_SORT_NUMBER]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_CONTENTS_SORT_NUMBER] = value; }
		}
		/// <summary>ページタイトル</summary>
		public string PageTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_PAGE_TITLE]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_PAGE_TITLE] = value; }
		}
		/// <summary>代替テキスト</summary>
		public string AltText
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_ALT_TEXT]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_ALT_TEXT] = value; }
		}
		/// <summary>商品グループID</summary>
		public string ProductGroupId
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_PRODUCT_GROUP_ID]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_PRODUCT_GROUP_ID] = value; }
		}
		/// <summary>商品一覧タイトル</summary>
		public string ProductListTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_PRODUCT_LIST_TITLE]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_PRODUCT_LIST_TITLE] = value; }
		}
		/// <summary>表示件数</summary>
		public int DispNum
		{
			get { return (int)this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_DISPLAY_NUMBER]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_DISPLAY_NUMBER] = value; }
		}
		/// <summary>ページ送り</summary>
		public string Pagination
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_PAGINATION_FLG]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_PAGINATION_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FEATUREPAGECONTENTS_LAST_CHANGED] = value; }
		}
		#endregion
	}
}