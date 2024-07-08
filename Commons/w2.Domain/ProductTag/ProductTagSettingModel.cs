/*
=========================================================================================================
  Module      : 商品タグ設定マスタモデル (ProductTagSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductTag
{
	/// <summary>
	/// 商品タグ設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductTagSettingModel : ModelBase<ProductTagSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductTagSettingModel()
		{
			// TODO:定数を利用するよう書き換えてください。
			this.TagValidFlg = Constants.FLG_PRODUCTTAGSETTING_VALID_FLG_VALID;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductTagSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductTagSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>タグNo</summary>
		public long TagNo
		{
			get { return (long)this.DataSource[Constants.FIELD_PRODUCTTAGSETTING_TAG_NO]; }
		}
		/// <summary>タグID</summary>
		public string TagId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID] = value; }
		}
		/// <summary>タイトル</summary>
		public string TagName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME] = value; }
		}
		/// <summary>説明</summary>
		public string TagDiscription
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAGSETTING_TAG_DISCRIPTION]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAGSETTING_TAG_DISCRIPTION] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string TagValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTTAGSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAGSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTTAGSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAGSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAGSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAGSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
