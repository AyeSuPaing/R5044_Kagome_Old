/*
=========================================================================================================
  Module      : レコメンド設定表示のためのヘルパクラス (RecommendContainer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace w2.Domain.Recommend.Helper
{
	#region +表示用レコメンド設定クラス
	/// <summary>
	/// 表示用レコメンド設定クラス
	/// ※RecommendModelを拡張
	/// </summary>
	[Serializable]
	public class RecommendContainer : RecommendModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendContainer(Hashtable source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>
		/// レコメンド適用条件アイテムリスト
		/// </summary>
		public new RecommendApplyConditionItemContainer[] ApplyConditionItems
		{
			get { return (RecommendApplyConditionItemContainer[])this.DataSource["EX_ApplyConditionItems"]; }
			set { this.DataSource["EX_ApplyConditionItems"] = value; }
		}
		/// <summary>
		/// レコメンドアップセル対象アイテム
		/// </summary>
		public new RecommendUpsellTargetItemContainer UpsellTargetItem
		{
			get { return (RecommendUpsellTargetItemContainer)this.DataSource["EX_UpsellTargetItem"]; }
			set { this.DataSource["EX_UpsellTargetItem"] = value; }
		}
		/// <summary>
		/// レコメンドアイテムリスト
		/// </summary>
		public new RecommendItemContainer[] Items
		{
			get { return (RecommendItemContainer[])this.DataSource["EX_Items"]; }
			set { this.DataSource["EX_Items"] = value; }
		}
		#endregion
	}
	#endregion

	#region +表示用レコメンド適用条件アイテムクラス
	/// <summary>
	/// 表示用レコメンド適用条件アイテムクラス
	/// ※RecommendApplyConditionItemModelを拡張
	/// </summary>
	[Serializable]
	public class RecommendApplyConditionItemContainer : RecommendApplyConditionItemModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendApplyConditionItemContainer(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>商品名</summary>
		public string ProductName
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
		/// <summary>バリエーション名3</summary>
		public string VariationName3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]; }
		}
		/// <summary>定期購入フラグ</summary>
		public string FixedPurchaseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] = value; }
		}
		/// <summary>頒布会購入フラグ</summary>
		public string SubscriptionBoxFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG] = value; }
		}
		#endregion
	}
	#endregion

	#region +表示用レコメンドアップセル対象アイテムクラス
	/// <summary>
	/// 表示用レコメンドアップセル対象アイテムクラス
	/// ※RecommendUpsellTargetItemModelを拡張
	/// </summary>
	[Serializable]
	public class RecommendUpsellTargetItemContainer : RecommendUpsellTargetItemModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendUpsellTargetItemContainer(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>商品名</summary>
		public string ProductName
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
		/// <summary>バリエーション名3</summary>
		public string VariationName3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]; }
		}
		/// <summary>配送種別</summary>
		public string ShippingType
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_TYPE]; }
		}
		/// <summary>定期購入フラグ</summary>
		public string FixedPurchaseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] = value; }
		}
		/// <summary>頒布会購入フラグ</summary>
		public string SubscriptionBoxFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG] = value; }
		}
		#endregion
	}
	#endregion

	#region +表示用レコメンドアイテムクラス
	/// <summary>
	/// 表示用レコメンドアイテムクラス
	/// ※RecommendItemModelを拡張
	/// </summary>
	[Serializable]
	public class RecommendItemContainer : RecommendItemModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendItemContainer(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>商品名</summary>
		public string ProductName
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
		/// <summary>バリエーション名3</summary>
		public string VariationName3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]; }
		}
		/// <summary>配送種別</summary>
		public string ShippingType
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_TYPE]; }
		}
		/// <summary>定期購入フラグ</summary>
		public string FixedPurchaseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] = value; }
		}
		/// <summary>頒布会購入フラグ</summary>
		public string SubscriptionBoxFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG] = value; }
		}
		#endregion
	}
	#endregion
}