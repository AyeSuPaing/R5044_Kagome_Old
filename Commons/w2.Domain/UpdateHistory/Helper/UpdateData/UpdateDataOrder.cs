/*
=========================================================================================================
  Module      : 注文情報更新データクラス (UpdateDataOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using w2.Domain.UserCreditCard;
using w2.Domain.Order;
using w2.Domain.TwOrderInvoice;

namespace w2.Domain.UpdateHistory.Helper.UpdateData
{
	/// <summary>
	/// 注文情報更新データクラス
	/// </summary>
	[XmlRoot("w2_Order")]
	public class UpdateDataOrder : UpdateDataBase, IUpdateData
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataOrder()
			: base()
		{
			this.Owner = new UpdateDataOrderOwner();
			this.Shippings = new UpdateDataOrderShipping[0];
			this.Coupons = new UpdateDataOrderCoupon[0];
			this.SetPromotions = new UpdateDataOrderSetPromotion[0];
			this.Invoices = new UpdateDataOrderInvoice[0];
			this.UserCreditCard = new UpdateDataUserCreditCard();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">注文情報モデル</param>
		/// <param name="userCreditCard">ユーザークレジットカードモデル</param>
		public UpdateDataOrder(OrderModel model, UserCreditCardModel userCreditCard)
			: this()
		{
			// 注文情報
			this.KeyValues = model.CreateUpdateDataList();

			// 注文者情報
			this.Owner = new UpdateDataOrderOwner(model.Owner);

			// 注文配送先情報
			this.Shippings = model.Shippings.Select(s => new UpdateDataOrderShipping(s)).ToArray();

			// 注文クーポン情報
			if (model.Coupons.Any())
			{
				this.Coupons = model.Coupons.Select(c => new UpdateDataOrderCoupon(c)).ToArray();
			}
			// 注文セットプロモーション情報
			if (model.SetPromotions.Any())
			{
				this.SetPromotions = model.SetPromotions.Select(s => new UpdateDataOrderSetPromotion(s)).ToArray();
			}
			// ユーザークレジットカード
			if (model.UseUserCreditCard && (userCreditCard != null))
			{
				this.UserCreditCard = new UpdateDataUserCreditCard(userCreditCard);
			}
			// Order Invoice
			if (model.Invoices.Any())
			{
				this.Invoices = model.Invoices.Select(s => new UpdateDataOrderInvoice(s)).ToArray();
			}
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 更新データ変換（モデル⇒バイナリ）
		/// </summary>
		/// <param name="updateData">更新データ</param>
		/// <returns>更新データインスタンス</returns>
		public static UpdateDataOrder Deserialize(byte[] updateData)
		{
			return UpdateDataConverter.Deserialize<UpdateDataOrder>(updateData);
		}
		/// <summary>
		/// 更新データXML変換（モデル⇒バイナリ）
		/// </summary>
		/// <returns>シリアライズ化された更新履歴、ハッシュ文字列</returns>
		public Tuple<byte[], string> SerializeAndCreateHashString()
		{
			var result = base.SerializeAndCreateHashString(
				// 履歴に格納しないフィールド
				new string[0],
				// ハッシュに含めないフィールド
				new[]
				{
					Constants.FIELD_ORDER_CREDIT_BRANCH_NO,
					Constants.FIELD_ORDER_DATE_CHANGED,
					Constants.FIELD_ORDER_LAST_CHANGED,
				});
			return result;
		}
		#endregion

		#region プロパティ
		/// <summary>注文者情報</summary>
		[XmlElement("w2_OrderOwner")]
		public UpdateDataOrderOwner Owner { get; set; }
		/// <summary>注文配送先情報リスト</summary>
		[XmlArray("w2_OrderShippings")]
		[XmlArrayItem("w2_OrderShipping")]
		public UpdateDataOrderShipping[] Shippings { get; set; }
		/// <summary>注文クーポン情報リスト</summary>
		[XmlArray("w2_OrderCoupons")]
		[XmlArrayItem("w2_OrderCoupon")]
		public UpdateDataOrderCoupon[] Coupons { get; set; }
		/// <summary>注文セットプロモーション情報リスト</summary>
		[XmlArray("w2_OrderSetPromotions")]
		[XmlArrayItem("w2_OrderSetPromotion")]
		public UpdateDataOrderSetPromotion[] SetPromotions { get; set; }
		/// <summary>ユーザークレジットカード</summary>
		[XmlElement("w2_UserCreditCard")]
		public UpdateDataUserCreditCard UserCreditCard { get; set; }
		/// <summary>Update Data Order Invoice</summary>
		[XmlArray("w2_TwOrderInvoices")]
		[XmlArrayItem("w2_TwOrderInvoice")]
		public UpdateDataOrderInvoice[] Invoices { get; set; }
		#endregion
	}

	/// <summary>
	/// 注文情報更新データクラス（注文者）
	/// </summary>
	public class UpdateDataOrderOwner : UpdateDataBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataOrderOwner()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">注文者情報モデル</param>
		public UpdateDataOrderOwner(OrderOwnerModel model)
			: this()
		{
			this.KeyValues = model.CreateUpdateDataList();
		}
		#endregion

		#region プロパティ
		#endregion
	}

	/// <summary>
	/// 注文情報更新データクラス（注文配送先）
	/// </summary>
	public class UpdateDataOrderShipping : UpdateDataBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataOrderShipping()
			: base()
		{
			this.Items = new UpdateDataOrderItem[0];
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">注文配送先情報モデル</param>
		public UpdateDataOrderShipping(OrderShippingModel model)
			: this()
		{
			this.KeyValues = model.CreateUpdateDataList();

			// 定期購入商品情報
			this.Items = model.Items.Select(i => new UpdateDataOrderItem(i)).ToArray();
		}
		#endregion

		#region プロパティ
		/// <summary>注文商品リスト</summary>
		[XmlArray("w2_OrderItems")]
		[XmlArrayItem("w2_OrderItem")]
		public UpdateDataOrderItem[] Items { get; set; }
		#endregion
	}

	/// <summary>
	/// 注文情報更新データクラス（注文商品）
	/// </summary>
	public class UpdateDataOrderItem : UpdateDataBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataOrderItem()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">注文商品情報モデル</param>
		public UpdateDataOrderItem(OrderItemModel model)
			: this()
		{
			this.KeyValues = model.CreateUpdateDataList();
		}
		#endregion

		#region プロパティ
		#endregion
	}

	/// <summary>
	/// 注文情報更新データクラス（注文クーポン）
	/// </summary>
	public class UpdateDataOrderCoupon : UpdateDataBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataOrderCoupon()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">注文クーポン情報モデル</param>
		public UpdateDataOrderCoupon(OrderCouponModel model)
			: this()
		{
			this.KeyValues = model.CreateUpdateDataList();
		}
		#endregion

		#region プロパティ
		/// <summary>注文商品リスト</summary>
		[XmlArray("w2_OrderItems")]
		[XmlArrayItem("w2_OrderItem")]
		public UpdateDataOrderItem[] Items { get; set; }
		#endregion
	}

	/// <summary>
	/// 注文情報更新データクラス（注文セットプロモーション）
	/// </summary>
	public class UpdateDataOrderSetPromotion : UpdateDataBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataOrderSetPromotion()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">注文セットプロモーション情報モデル</param>
		public UpdateDataOrderSetPromotion(OrderSetPromotionModel model)
			: this()
		{
			this.KeyValues = model.CreateUpdateDataList();
		}
		#endregion
	}

	/// <summary>
	/// Update Data Order Invoice
	/// </summary>
	public class UpdateDataOrderInvoice : UpdateDataBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataOrderInvoice()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">Tw Order Invoice Model</param>
		public UpdateDataOrderInvoice(TwOrderInvoiceModel model)
			: this()
		{
			this.KeyValues = model.CreateUpdateDataList();
		}
		#endregion
	}
}