/*
=========================================================================================================
  Module      : SEOメタデータマスタモデル (SeoMetadatasModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.SeoMetadatas
{
	/// <summary>
	/// SEOメタデータマスタモデル
	/// </summary>
	[Serializable]
	public partial class SeoMetadatasModel : ModelBase<SeoMetadatasModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SeoMetadatasModel()
		{
			this.DelFlg = "0";
			this.HtmlTitle = string.Empty;
			this.MetadataDesc = string.Empty;
			this.MetadataKeywords = string.Empty;
			this.Comment = string.Empty;
			this.SeoText = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SeoMetadatasModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SeoMetadatasModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_SHOP_ID] = value; }
		}
		/// <summary>データ区分</summary>
		public string DataKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_DATA_KBN]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_DATA_KBN] = value; }
		}
		/// <summary>タイトル</summary>
		public string HtmlTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_HTML_TITLE]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_HTML_TITLE] = value; }
		}
		/// <summary>キーワード</summary>
		public string MetadataKeywords
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_METADATA_KEYWORDS]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_METADATA_KEYWORDS] = value; }
		}
		/// <summary>ディスクリプション</summary>
		public string MetadataDesc
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_METADATA_DESC]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_METADATA_DESC] = value; }
		}
		/// <summary>コメント</summary>
		public string Comment
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_COMMENT]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_COMMENT] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SEOMETADATAS_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SEOMETADATAS_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_LAST_CHANGED] = value; }
		}
		/// <summary>SEO文言</summary>
		public string SeoText
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_SEO_TEXT]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_SEO_TEXT] = value; }
		}
		/// <summary>デフォルト文言</summary>
		public string DefaultText
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_DEFAULT_TEXT]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_DEFAULT_TEXT] = value; }
		}
		#endregion
	}
}
