/*
=========================================================================================================
  Module      : 定期購入表示のためのヘルパクラス (FixedPurchaseContainer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.Common.Util;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.FixedPurchase.Helper
{
	#region +表示用定期購入情報クラス
	/// <summary>
	/// 表示用定期購入情報クラス
	/// ※FixedPurchaseModelを拡張
	/// </summary>
	[Serializable]
	public class FixedPurchaseContainer : FixedPurchaseModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseContainer(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>配送先リスト</summary>
		public new FixedPurchaseShippingContainer[] Shippings
		{
			get { return (FixedPurchaseShippingContainer[])this.DataSource["EX_Shippings"]; }
			set { this.DataSource["EX_Shippings"] = value; }
		}
		/// <summary>注文者名</summary>
		[UpdateData(1001, "owner_name")]
		public string OwnerName
		{
			get { return (string)this.DataSource["owner_name"]; }
		}
		/// <summary>注文者メールアドレス</summary>
		[UpdateData(1002, "owner_mail_addr")]
		public string OwnerMailAddr
		{
			get { return (string)this.DataSource["owner_mail_addr"]; }
		}
		/// <summary>注文者メールアドレス2</summary>
		[UpdateData(1003, "owner_mail_addr2")]
		public string OwnerMailAddr2
		{
			get { return (string)this.DataSource["owner_mail_addr2"]; }
		}
		/// <summary>会員ランクID</summary>
		[UpdateData(1004, "member_rank_id")]
		public string MemberRankId
		{
			get { return (string)this.DataSource[Constants.FIELD_USER_MEMBER_RANK_ID]; }
		}
		/// <summary>注文者生年月日</summary>
		[UpdateData(1005, "owner_birth")]
		public DateTime? OwnerBirth
		{
			get
			{
				if (this.DataSource["owner_birth"] == DBNull.Value) return null;
				return (DateTime?)this.DataSource["owner_birth"];
			}
		}
		/// <summary>注文者性別</summary>
		[UpdateData(1006, "owner_sex")]
		public string OwnerSex
		{
			get { return (string)this.DataSource["owner_sex"]; }
		}
		/// <summary>注文者区分</summary>
		[UpdateData(1007, "owner_kbn")]
		public string OwnerKbn
		{
			get { return (string)this.DataSource["owner_kbn"]; }
		}
		/// <summary>配送希望時間帯(メッセージ)</summary>
		[UpdateData(1008, "shipping_time_message")]
		public string ShippingTimeMessage
		{
			get { return (string)this.DataSource["shipping_time_message"]; }
		}
		/// <summary>外部決済連携ログ</summary>
		[UpdateData(1009, "external_payment_cooperation_log")]
		public string ExternalPaymentCooperationLog
		{
			get { return (string)this.DataSource["external_payment_cooperation_log"]; }
		}
		/// <summary>Owner Tel1</summary>
		[UpdateData(1010, "owner_tel1")]
		public string OwnerTel1
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_ORDEROWNER_OWNER_TEL1]); }
		}
		/// <summary>定期購入ステータス：配送不可エリア停止</summary>
		public bool IsUnavailableShippingAreaStatus
		{
			get
			{
				return ((string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS] == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_UNAVAILABLE_SHIPPING_AREA);
			}
		}
		/// <summary>定期台帳の商品は1つでも削除されたか</summary>
		public bool HasDeletedAnyFixedPurchaseItem { get; set; }
		#endregion
	}

	/// <summary>
	/// 表示用定期購入情報クラス（配送先）
	/// ※FixedPurchaseShippingModelを拡張
	/// </summary>
	[Serializable]
	public class FixedPurchaseShippingContainer : FixedPurchaseShippingModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseShippingContainer(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>アイテムリスト</summary>
		public new FixedPurchaseItemContainer[] Items
		{
			get { return (FixedPurchaseItemContainer[])this.DataSource["EX_Items"]; }
			set { this.DataSource["EX_Items"] = value; }
		}
		#endregion
	}

	/// <summary>
	/// 表示用定期購入情報クラス（商品）
	/// ※FixedPurchaseItemModelを拡張
	/// </summary>
	[Serializable]
	public class FixedPurchaseItemContainer : FixedPurchaseItemModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseItemContainer(DataRowView source)
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
			return FixedPurchasePriceHelper.GetValidPrice(this.FixedPurchasePrice, this.MemberRankPrice, this.SpecialPrice, this.Price);
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
		[UpdateData(1001, "product_name")]
		public string Name
		{
			get { return (string)this.DataSource["product_name"]; }
			set { this.DataSource["product_name"] = value; }
		}
		/// <summary>バリエーション名1</summary>
		[UpdateData(1002, "variation_name1")]
		public string VariationName1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]; }
		}
		/// <summary>バリエーション名2</summary>
		[UpdateData(1003, "variation_name2")]
		public string VariationName2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]; }
		}
		/// <summary>優先度の高い価格</summary>
		[UpdateData(1004, "valid_price")]
		public decimal ValidPrice { get { return GetValidPrice(); } }

		/// <summary>価格</summary>
		[UpdateData(1005, "price")]
		public decimal Price
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE]; }
		}
		/// <summary>特別価格</summary>
		[UpdateData(1006, "special_price")]
		public decimal? SpecialPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE];
			}
		}
		/// <summary>会員ランク価格</summary>
		[UpdateData(1007, "member_rank_price")]
		public decimal? MemberRankPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE];
			}
		}
		/// <summary>明細金額（小計）</summary>
		[UpdateData(1008, "item_price")]
		public decimal ItemPrice
		{
			get { return this.ValidPrice * this.ItemQuantity; }
		}
		/// <summary>明細金額（小計・セット未考慮）</summary>
		[UpdateData(1009, "item_price_single")]
		public decimal ItemPriceSingle
		{
			get { return this.ValidPrice * this.ItemQuantitySingle; }
		}
		/// <summary>定期購入価格</summary>
		[UpdateData(1010, "variation_fixed_purchase_price")]
		public decimal? FixedPurchasePrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE];
			}
		}
		/// <summary>配送料種別</summary>
		[UpdateData(1011, "shipping_type")]
		public string ShippingType
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_TYPE]; }
		}
		/// <summary>定期購入フラグ</summary>
		[UpdateData(1012, "fixed_purchase_flg")]
		public string FixedPurchaseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG]; }
		}
		/// <summary>バリエーション名3</summary>
		[UpdateData(1013, "variation_name3")]
		public string VariationName3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]; }
		}
		/// <summary>利用不可定期購入配送間隔月</summary>
		public string LimitedFixedPurchaseKbn1Setting
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING]; }
		}
		/// <summary>利用不可定期購入配送間隔日</summary>
		public string LimitedFixedPurchaseKbn3Setting
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING]; }
		}
		/// <summary>利用不可定期購入配送間隔週</summary>
		public string LimitedFixedPurchaseKbn4Setting
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING]; }
		}
		/// <summary>商品画像名ヘッダ</summary>
		public string ImageHead
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_IMAGE_HEAD]; }
		}
		/// <summary>バリエーション画像名ヘッダ</summary>
		public string VariationImageHead
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD]; }
		}
		/// <summary>配送サイズ区分 </summary>
		public string ShippingSizeKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN]; }
		}
		/// <summary>解約可能回数 </summary>
		public int FixedPurchaseCancelableCount
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT]; }
		}
		/// <summary>Fixed Purchase Limited Skipped Count</summary>
		public int? FixedPurchaseLimitedSkippedCount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT] == DBNull.Value) return null;

				return (int?)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT];
			}
		}
		/// <summary>決済利用不可</summary>
		public string ProductLimitedPaymentIds
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_LIMITED_PAYMENT_IDS]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_LIMITED_PAYMENT_IDS] = value; }

		}
		#endregion
	}
	#endregion
}
