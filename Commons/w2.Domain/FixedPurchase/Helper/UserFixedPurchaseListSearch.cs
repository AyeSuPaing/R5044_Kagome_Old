/*
=========================================================================================================
  Module      : ユーザ定期購入一覧検索のためのヘルパクラス (UserFixedPurchaseListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.FixedPurchase.Helper
{
	#region +ユーザ定期購入一覧検索条件クラス
	/// <summary>
	/// ユーザ定期購入一覧検索条件クラス
	/// </summary>
	[Serializable]
	public class UserFixedPurchaseListSearchCondition : BaseDbMapModel
	{
		/// <summary>ユーザID</summary>
		[DbMapName("user_id")]
		public string UserId { get; set; }
		/// <summary>開始行番号</summary>
		[DbMapName("bgn_row_num")]
		public int? BeginRowNumber { get; set; }
		/// <summary>終了行番号</summary>
		[DbMapName("end_row_num")]
		public int? EndRowNumber { get; set; }
		/// <summary>管理同梱キャンセル</summary>
		[DbMapName("extend_status36")]
		public string IsCancelByOperatorsCombine { get; set; }
		/// <summary>定期ステータス</summary>
		[DbMapName("fixedpurchase_status")]
		public string FixedPurchaseStatusParameter { get; set; }
	}
	#endregion

	#region +ユーザ定期購入一覧検索結果クラス
	/// <summary>
	/// ユーザ定期購入一覧検索結果クラス
	/// ※FixedPurchaseModelを拡張
	/// </summary>
	[Serializable]
	public class UserFixedPurchaseListSearchResult : FixedPurchaseModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserFixedPurchaseListSearchResult(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>配送先リスト</summary>
		public new UserFixedPurchaseShippingListSearchResult[] Shippings
		{
			get { return (UserFixedPurchaseShippingListSearchResult[])this.DataSource["EX_Shippings"]; }
			set { this.DataSource["EX_Shippings"] = value; }
		}
		/// <summary>注文者名</summary>
		public string OwnerName
		{
			get { return (string)this.DataSource["owner_name"]; }
		}
		/// <summary>注文者メールアドレス</summary>
		public string OwnerMailAddr
		{
			get { return (string)this.DataSource["owner_mail_addr"]; }
		}
		/// <summary>注文者メールアドレス2</summary>
		public string OwnerMailAddr2
		{
			get { return (string)this.DataSource["owner_mail_addr2"]; }
		}
		/// <summary>未出荷商品の配送希望日</summary>
		public DateTime? FixedPurchaseShippingDateNearest
		{
			get
			{
				return (this.DataSource["fixedpurchase_shipping_date_nearest"] == DBNull.Value)
					? null
					: (DateTime?)this.DataSource["fixedpurchase_shipping_date_nearest"];
			}
		}
		/// <summary>
		/// 頒布会表示名
		/// </summary>
		public string SubscriptionBoxDisplayName
		{
			get { return (this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DISPLAY_NAME] == DBNull.Value ? "" : (string)this.DataSource[Constants.FIELD_SUBSCRIPTIONBOX_DISPLAY_NAME]); }
		}
		#endregion
	}

	/// <summary>
	/// ユーザ定期購入一覧検索結果クラス（配送先）
	/// ※FixedPurchaseShippingModelを拡張
	/// </summary>
	[Serializable]
	public class UserFixedPurchaseShippingListSearchResult : FixedPurchaseShippingModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserFixedPurchaseShippingListSearchResult(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>アイテムリスト</summary>
		public new UserFixedPurchaseItemListSearchResult[] Items
		{
			get { return (UserFixedPurchaseItemListSearchResult[])this.DataSource["EX_Items"]; }
			set { this.DataSource["EX_Items"] = value; }
		}
		#endregion
	}

	/// <summary>
	/// ユーザ定期購入一覧検索結果クラス（商品）
	/// ※FixedPurchaseItemModelを拡張
	/// </summary>
	[Serializable]
	public class UserFixedPurchaseItemListSearchResult : FixedPurchaseItemModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserFixedPurchaseItemListSearchResult(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 優先順位順で値がある金額を取得
		/// </summary>
		/// <returns>優先度の高い価格</returns>
		public decimal GetValidPrice()
		{
			return FixedPurchasePriceHelper.GetValidPrice(
				this.FixedPurchasePrice,
				this.MemberRankPrice,
				this.SpecialPrice,
				this.Price);
		}

		/// <summary>
		/// 明細金額（小計）取得
		/// </summary>
		/// <returns>明細金額（小計）</returns>
		public decimal GetItemPrice()
		{
			return FixedPurchasePriceHelper.GetItemPrice(this.ItemQuantity, GetValidPrice());
		}
		#endregion

		#region プロパティ
		/// <summary>商品名</summary>
		public string Name
		{
			get { return (string)this.DataSource["product_name"]; }
		}
		/// <summary>バリエーション名1</summary>
		public string VariationName1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]; }
		}
		/// <summary>バリエーション名2</summary>
		public string VariationName2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]; }
		}
		/// <summary>価格</summary>
		public decimal Price
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE]; }
		}
		/// <summary>特別価格</summary>
		public decimal? SpecialPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE];
			}
		}
		/// <summary>会員ランク価格</summary>
		public decimal? MemberRankPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE];
			}
		}
		/// <summary>定期購入価格</summary>
		public decimal? FixedPurchasePrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE];
			}
		}
		/// <summary>Fixed purchase flag</summary>
		public string FixedPurchaseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG]; }
		}
		#endregion
	}
	#endregion
}
