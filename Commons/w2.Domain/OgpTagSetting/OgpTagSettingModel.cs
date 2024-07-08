/*
=========================================================================================================
  Module      : OGPタグ設定モデル (OgpTagSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.OgpTagSetting
{
	/// <summary>
	/// OGPタグ設定モデル
	/// </summary>
	[Serializable]
	public partial class OgpTagSettingModel : ModelBase<OgpTagSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OgpTagSettingModel()
		{
			this.DataKbn = string.Empty;
			this.SiteTitle = string.Empty;
			this.PageTitle = string.Empty;
			this.Description = string.Empty;
			this.ImageUrl = string.Empty;
			this.LastChanged = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OgpTagSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OgpTagSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>データ区分</summary>
		public string DataKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_OGPTAGSETTING_DATA_KBN]; }
			set { this.DataSource[Constants.FIELD_OGPTAGSETTING_DATA_KBN] = value; }
		}
		/// <summary>サイト名</summary>
		public string SiteTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_OGPTAGSETTING_SITE_TITLE]; }
			set { this.DataSource[Constants.FIELD_OGPTAGSETTING_SITE_TITLE] = value; }
		}
		/// <summary>ページ名</summary>
		public string PageTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_OGPTAGSETTING_PAGE_TITLE]; }
			set { this.DataSource[Constants.FIELD_OGPTAGSETTING_PAGE_TITLE] = value; }
		}
		/// <summary>ディスクリプション</summary>
		public string Description
		{
			get { return (string)this.DataSource[Constants.FIELD_OGPTAGSETTING_DESCRIPTION]; }
			set { this.DataSource[Constants.FIELD_OGPTAGSETTING_DESCRIPTION] = value; }
		}
		/// <summary>画像URL</summary>
		public string ImageUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_OGPTAGSETTING_IMAGE_URL]; }
			set { this.DataSource[Constants.FIELD_OGPTAGSETTING_IMAGE_URL] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_OGPTAGSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_OGPTAGSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_OGPTAGSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_OGPTAGSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_OGPTAGSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_OGPTAGSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
