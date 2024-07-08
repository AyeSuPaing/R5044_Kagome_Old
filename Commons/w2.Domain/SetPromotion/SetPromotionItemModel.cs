/*
=========================================================================================================
  Module      : セットプロモーション商品マスタモデル (SetPromotionItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.SetPromotion
{
	/// <summary>
	/// セットプロモーション商品マスタモデル
	/// </summary>
	[Serializable]
	public partial class SetPromotionItemModel : ModelBase<SetPromotionItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SetPromotionItemModel()
		{
			this.SetpromotionItemNo = 1;
			this.SetpromotionItemQuantity = 1;
			this.SetpromotionItemQuantityMoreFlg = Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY_MORE_FLG_INVALID;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SetPromotionItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SetPromotionItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>セットプロモーションID</summary>
		public string SetpromotionId
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ID]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ID] = value; }
		}
		/// <summary>セットプロモーションアイテム枝番</summary>
		public int SetpromotionItemNo
		{
			get { return (int)this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_NO] = value; }
		}
		/// <summary>セットプロモーションアイテム区分</summary>
		public string SetpromotionItemKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN] = value; }
		}
		/// <summary>対象商品</summary>
		public string SetpromotionItems
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEMS]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEMS] = value; }
		}
		/// <summary>数量</summary>
		public int SetpromotionItemQuantity
		{
			get { return (int)this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY] = value; }
		}
		/// <summary>数量以上フラグ</summary>
		public string SetpromotionItemQuantityMoreFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY_MORE_FLG]; }
			set { this.DataSource[Constants.FIELD_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY_MORE_FLG] = value; }
		}
		#endregion
	}
}
