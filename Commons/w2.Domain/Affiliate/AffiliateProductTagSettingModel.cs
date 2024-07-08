/*
=========================================================================================================
  Module      : アフィリエイト商品タグ設定マスタモデル (AffiliateProductTagSettingModel.cs)
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
	/// アフィリエイト商品タグ設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class AffiliateProductTagSettingModel : ModelBase<AffiliateProductTagSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AffiliateProductTagSettingModel()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AffiliateProductTagSettingModel(DataRowView source) : this(source.ToHashtable())
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AffiliateProductTagSettingModel(Hashtable source) : this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>アフィリエイト商品タグID</summary>
		public int AffiliateProductTagId
		{
			get { return (int)this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_AFFILIATE_PRODUCT_TAG_ID]; }
			set { this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_AFFILIATE_PRODUCT_TAG_ID] = value; }
		}
		/// <summary>タグ名称</summary>
		public string TagName
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_NAME]; }
			set { this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_NAME] = value; }
		}
		/// <summary>タグ内容</summary>
		public string TagContent
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_CONTENT]; }
			set { this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_CONTENT] = value; }
		}
		/// <summary>区切り文字</summary>
		public string TagDelimiter
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_DELIMITER]; }
			set { this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_DELIMITER] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_AFFILIATEPRODUCTTAGSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}