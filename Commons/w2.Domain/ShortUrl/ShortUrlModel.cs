/*
=========================================================================================================
  Module      : ショートURLマスタモデル (ShortUrlModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ShortUrl
{
	/// <summary>
	/// ショートURLマスタモデル
	/// </summary>
	[Serializable]
	public partial class ShortUrlModel : ModelBase<ShortUrlModel>
	{
		#region コンストラクタ

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ShortUrlModel()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ShortUrlModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ShortUrlModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}

		#endregion

		#region プロパティ

		/// <summary>ショートURL NO</summary>
		public long SurlNo
		{
			get { return (long)this.DataSource[Constants.FIELD_SHORTURL_SURL_NO]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_SURL_NO] = value; }
		}

		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHORTURL_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_SHOP_ID] = value; }
		}

		/// <summary>ショートURL</summary>
		public string ShortUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_SHORTURL_SHORT_URL]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_SHORT_URL] = value; }
		}

		/// <summary>ロングURL</summary>
		public string LongUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_SHORTURL_LONG_URL]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_LONG_URL] = value; }
		}

		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHORTURL_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_DATE_CREATED] = value; }
		}

		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHORTURL_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_DATE_CHANGED] = value; }
		}

		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SHORTURL_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_LAST_CHANGED] = value; }
		}

		#endregion
	}
}