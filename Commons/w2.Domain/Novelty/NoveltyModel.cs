/*
=========================================================================================================
  Module      : ノベルティ設定モデル (NoveltyModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Novelty
{
	/// <summary>
	/// ノベルティ設定モデル
	/// </summary>
	[Serializable]
	public partial class NoveltyModel : ModelBase<NoveltyModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public NoveltyModel()
		{
			this.ValidFlg = Constants.FLG_NOVELTY_VALID_FLG_VALID;
			this.TargetItemList = new NoveltyTargetItemModel[0];
			this.GrantConditionsList = new NoveltyGrantConditionsModel[0];
			this.AutoAdditionalFlg = Constants.FLG_NOVELTY_AUTO_ADDITIONAL_FLG_INVALID;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public NoveltyModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public NoveltyModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_SHOP_ID] = value; }
		}
		/// <summary>ノベルティID</summary>
		public string NoveltyId
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_NOVELTY_ID]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_NOVELTY_ID] = value; }
		}
		/// <summary>ノベルティ名（表示用）</summary>
		public string NoveltyDispName
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_NOVELTY_DISP_NAME]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_NOVELTY_DISP_NAME] = value; }
		}
		/// <summary>ノベルティ名（管理用）</summary>
		public string NoveltyName
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_NOVELTY_NAME]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_NOVELTY_NAME] = value; }
		}
		/// <summary>説明（管理用）</summary>
		public string Discription
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_DISCRIPTION]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_DISCRIPTION] = value; }
		}
		/// <summary>開始日時</summary>
		public DateTime DateBegin
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_NOVELTY_DATE_BEGIN]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_DATE_BEGIN] = value; }
		}
		/// <summary>終了日時</summary>
		public DateTime? DateEnd
		{
			get
			{
				if (this.DataSource[Constants.FIELD_NOVELTY_DATE_END] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_NOVELTY_DATE_END];
			}
			set { this.DataSource[Constants.FIELD_NOVELTY_DATE_END] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_NOVELTY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_NOVELTY_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_LAST_CHANGED] = value; }
		}
		/// <summary>自動付与フラグ</summary>
		public string AutoAdditionalFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_NOVELTY_AUTO_ADDITIONAL_FLG]; }
			set { this.DataSource[Constants.FIELD_NOVELTY_AUTO_ADDITIONAL_FLG] = value; }
		}
		#endregion
	}
}
