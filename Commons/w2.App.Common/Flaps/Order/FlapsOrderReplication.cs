/*
=========================================================================================================
  Module      : FLAPS注文連携クラス (FlapsOrderReplication.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using w2.App.Common.Order;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Flaps.Order
{
	/// <summary>
	/// FLAPS注文連携クラス
	/// </summary>
	[XmlRoot(ElementName = "FWS")]
	public class FlapsOrderReplication : FlapsEntity
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FlapsOrderReplication()
		{
			this.Method = "iPTS/DoSubmitOrder";
			this.Request = OrderApiService.Get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文</param>
		/// <param name="cart">カート</param>
		/// <remarks>カートフロー処理に合わせるため、あえてHashtable型の値を受け取る</remarks>
		public FlapsOrderReplication(Hashtable order, CartObject cart) : this()
		{
			// FLAPS設定値情報
			this.Identifier = (string)order[Constants.FIELD_ORDER_ORDER_ID];
			this.CashRegisterCode = Constants.FLAPS_ORDER_CASH_REGISTER_CODE;
			this.SalePointCode = Constants.FLAPS_ORDER_SALE_POINT_CODE;
			this.UserIdCode = Constants.FLAPS_ORDER_USER_ID_CODE;
			this.MemberSerNo = Constants.FLAPS_ORDER_MEMBER_SER_NO;

			// 購入系情報
			this.Date = ((DateTime)order[Constants.FIELD_ORDER_ORDER_DATE]).ToString("yyyyMMdd");
			this.MemberAdd = "0";
			var owner = cart.Owner;
			this.Name = owner.Name;
			this.CellPhone = owner.Tel1.Replace("-", "");
			this.Sex = owner.Sex;
			this.EmployeeCode = Constants.FLAPS_ORDER_EMPLOYEE_CODE;
			this.ReceiveTel = this.ReceiveShortMessage = this.ReceiveDm = this.ReceiveEMail = owner.MailFlg ? "1" : "0";
			this.UseBonus = cart.UsePoint.ToString();
			this.TotalBonus = order.ContainsKey(Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT)
				? ((decimal)order[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT]).ToString("#")
				: "";
			var hasAppliedCoupon = (Constants.W2MP_COUPON_OPTION_ENABLED && (cart.Coupon != null));
			this.CouponNo = hasAppliedCoupon ? cart.Coupon.CouponCode : "";
			this.UseCouponTotal = hasAppliedCoupon ? cart.UseCouponPrice.ToString("#") : "";
			// 注文時点で 電子発票 を発行していないので空を格納
			this.InvoiceNo = "";

			// 配送
			var shipping = cart.Shippings[0];
			this.Receiver = shipping.Name;
			this.ReceiverAddress = shipping.ConcatenateAddressWithoutCountryName();
			this.ReceiverPhone = shipping.Tel1;

			// 決済
			switch (cart.Payment.PaymentId)
			{
				case Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT:
					this.PayWayCode = Constants.FLG_FLAPS_PAYMENT_TYPE_CASH;
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
					this.PayWayCode = Constants.FLG_FLAPS_PAYMENT_TYPE_CREDIT_CARD;
					break;

				default:
					this.PayWayCode = Constants.FLG_FLAPS_PAYMENT_TYPE_OTHERS;
					break;
			}

			this.Remark = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_MEMO]);
			switch (StringUtility.ToEmpty(shipping.UniformInvoiceType))
			{
				case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
					this.UniqueNo = StringUtility.ToEmpty(shipping.UniformInvoiceOption1);
					this.UniqueNoTitle = StringUtility.ToEmpty(shipping.UniformInvoiceOption2);
					this.InvoiceType = "1";
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
					this.UniqueNo = this.UniqueNoTitle = "";
					this.InvoiceType = "3";
					if (StringUtility.ToEmpty(shipping.CarryType) == Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE)
					{
						this.InvoiceCarrierId = StringUtility.ToEmpty(shipping.CarryTypeOption);
					}

					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
					this.UniqueNo = this.UniqueNoTitle = "";
					this.InvoiceType = "5";
					this.InvoiceNpoban = StringUtility.ToEmpty(shipping.UniformInvoiceTypeOption);
					break;

				default:
					this.UniqueNo = this.UniqueNoTitle =
						this.InvoiceType = this.InvoiceCarrierId = this.InvoiceNpoban = "";
					break;

			}

			// 商品
			this.TaxRate = cart.Items[0].TaxRate.ToString("#");
			this.Products = new List<FlapsOrderProduct>();
			foreach (var product in cart.Items.Where(cp => (cp.QuantitiyUnallocatedToSet > 0)))
			{
				this.Products.Add(new FlapsOrderProduct(product, cart.Shippings[0].IsDutyFree));
			}

			foreach (CartSetPromotion setPromotion in cart.SetPromotions)
			{
				foreach (var product in setPromotion.Items)
				{
					this.Products.Add(new FlapsOrderProduct(product, cart.Shippings[0].IsDutyFree, setPromotion));
				}
			}

			// 税計算
			// 計算処理の重複をさけるために商品(this.Products)の処理終了後に実行
			this.Tax = this.Products.Sum(p => p.TaxTotal).ToString("#");
			this.NonTaxedTotal = this.Products.Sum(p => decimal.Parse(p.RealTotal)).ToString("#");
			this.TaxedTotal = this.Products.Sum(p => decimal.Parse(p.RealTaxedTotal)).ToString("#");

		}

		/// <summary>
		/// 注文API処理実行
		/// </summary>
		/// <returns>連携結果</returns>
		internal OrderResult Post()
		{
			var result = Get<OrderResult>();
			return result;
		}

		/// <summary>
		/// ショップカウンターカードを保持
		/// </summary>
		/// <param name="posNoSerNo">ショップカウンターカード</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>結果</returns>
		/// <remarks>ショップカウンターカードはFLAPS側の注文IDのようなものでキャンセル時に必要</remarks>
		internal bool StorePosNoSerNo(string posNoSerNo, SqlAccessor accessor)
		{
			var result = new OrderService().UpdateOrderExtend(
				this.Identifier,
				Constants.FLG_LASTCHANGED_FLAPS,
				new Dictionary<string, string>
				{
					{ Constants.FLAPS_ORDEREXTENDSETTING_ATTRNO_FOR_POSNOSERNO, posNoSerNo },
				},
				UpdateHistoryAction.DoNotInsert,
				accessor);
			return (result > 0);
		}

		/// <summary>識別キー</summary>
		[XmlElement(ElementName = "Identifier")]
		public string Identifier { get; set; }
		/// <summary>レジコード</summary>
		[XmlElement(ElementName = "CashRegisterCode")]
		public string CashRegisterCode { get; set; }
		/// <summary>ショップカウンターコード</summary>
		[XmlElement(ElementName = "SalePointCode")]
		public string SalePointCode { get; set; }
		/// <summary>販売者コード</summary>
		[XmlElement(ElementName = "UserIDCode")]
		public string UserIdCode { get; set; }
		/// <summary>"発票日付/受注日</summary>
		[XmlElement(ElementName = "Date")]
		public string Date { get; set; }
		/// <summary>今回会員新規登録するか</summary>
		[XmlElement(ElementName = "MemberAdd")]
		public string MemberAdd { get; set; }
		/// <summary>会員唯一番号</summary>
		[XmlElement(ElementName = "MemberSerNo")]
		public string MemberSerNo { get; set; }
		/// <summary>会員コード</summary>
		[XmlElement(ElementName = "MemberCode")]
		public string MemberCode { get; set; }
		/// <summary>会員カードNo</summary>
		[XmlElement(ElementName = "MemberCardNo")]
		public string MemberCardNo { get; set; }
		/// <summary>会員名前</summary>
		[XmlElement(ElementName = "Name")]
		public string Name { get; set; }
		/// <summary>オンラインアカウント</summary>
		[XmlElement(ElementName = "WebAccount")]
		public string WebAccount { get; set; }
		/// <summary>証明書のカテゴリ</summary>
		[XmlElement(ElementName = "IDType")]
		public string IdType { get; set; }
		/// <summary>ID番号</summary>
		[XmlElement(ElementName = "IDNo")]
		public string IdNo { get; set; }
		/// <summary>電話番号</summary>
		[XmlElement(ElementName = "CellPhone")]
		public string CellPhone { get; set; }
		/// <summary>会員カード種別コード</summary>
		[XmlElement(ElementName = "MemberCardTypeCode")]
		public string MemberCardTypeCode { get; set; }
		/// <summary>性別</summary>
		[XmlElement(ElementName = "Sex")]
		public string Sex { get; set; }
		/// <summary>会員カード発行者コード</summary>
		[XmlElement(ElementName = "EmployeeCode")]
		public string EmployeeCode { get; set; }
		/// <summary>電話受けるフラグ</summary>
		[XmlElement(ElementName = "ReceiveTel")]
		public string ReceiveTel { get; set; }
		/// <summary>SMS受けるフラグ</summary>
		[XmlElement(ElementName = "ReceiveShortMessage")]
		public string ReceiveShortMessage { get; set; }
		/// <summary>DM受けるフラグ</summary>
		[XmlElement(ElementName = "ReceiveDM")]
		public string ReceiveDm { get; set; }
		/// <summary>メール受けるフラグ</summary>
		[XmlElement(ElementName = "ReceiveEMail")]
		public string ReceiveEMail { get; set; }
		/// <summary>業界共有受けるフラグ</summary>
		[XmlElement(ElementName = "ReceiveShare")]
		public string ReceiveShare { get; set; }
		/// <summary>利用ポイント</summary>
		[XmlElement(ElementName = "UseBonus")]
		public string UseBonus { get; set; }
		/// <summary>獲得ポイント</summary>
		[XmlElement(ElementName = "TotalBonus")]
		public string TotalBonus { get; set; }
		/// <summary>クーポンコード</summary>
		[XmlElement(ElementName = "CouponNo")]
		public string CouponNo { get; set; }
		/// <summary>クーポン金額</summary>
		[XmlElement(ElementName = "UseCouponTotal")]
		public string UseCouponTotal { get; set; }
		/// <summary>発票番号</summary>
		[XmlElement(ElementName = "InvoiceNo")]
		public string InvoiceNo { get; set; }
		/// <summary>統一番号</summary>
		[XmlElement(ElementName = "UniqueNo")]
		public string UniqueNo { get; set; }
		/// <summary>統一番号会社名</summary>
		[XmlElement(ElementName = "UniqueNoTitle")]
		public string UniqueNoTitle { get; set; }
		/// <summary>発票フォーマット</summary>
		[XmlElement(ElementName = "Format")]
		public string Format { get; set; }
		/// <summary>税額</summary>
		[XmlElement(ElementName = "Tax")]
		public string Tax { get; set; }
		/// <summary>税抜販売総額</summary>
		[XmlElement(ElementName = "NonTaxedTotal")]
		public string NonTaxedTotal { get; set; }
		/// <summary>税込み販売総額</summary>
		[XmlElement(ElementName = "TaxedTotal")]
		public string TaxedTotal { get; set; }
		/// <summary>税率</summary>
		[XmlElement(ElementName = "TaxRate")]
		public string TaxRate { get; set; }
		/// <summary>稅別</summary>
		[XmlElement(ElementName = "TaxType")]
		public string TaxType { get; set; }
		/// <summary>配送先氏名</summary>
		[XmlElement(ElementName = "Receiver")]
		public string Receiver { get; set; }
		/// <summary>配送先住所</summary>
		[XmlElement(ElementName = "ReceiverAddress")]
		public string ReceiverAddress { get; set; }
		/// <summary>配送先電話番号</summary>
		[XmlElement(ElementName = "ReceiverPhone")]
		public string ReceiverPhone { get; set; }
		/// <summary>顧客唯一コード</summary>
		[XmlElement(ElementName = "CustomerSerNo")]
		public string CustomerSerNo { get; set; }
		/// <summary>税込み配送料</summary>
		[XmlElement(ElementName = "TransFee")]
		public string TransFee { get; set; }
		/// <summary>支払方法コード</summary>
		[XmlElement(ElementName = "PayWayCode")]
		public string PayWayCode { get; set; }
		/// <summary>支払方法カードNo</summary>
		[XmlElement(ElementName = "PayCardNo")]
		public string PayCardNo { get; set; }
		/// <summary>備考</summary>
		[XmlElement(ElementName = "Remark")]
		public string Remark { get; set; }
		/// <summary>発票</summary>
		[XmlElement(ElementName = "InvoiceType")]
		public string InvoiceType { get; set; }
		/// <summary>載具コード</summary>
		[XmlElement(ElementName = "InvoiceCarrierType")]
		public string InvoiceCarrierType { get; set; }
		/// <summary>載具番号</summary>
		[XmlElement(ElementName = "InvoiceCarrierId")]
		public string InvoiceCarrierId { get; set; }
		/// <summary>寄付コード</summary>
		[XmlElement(ElementName = "InvoiceNPOBAN")]
		public string InvoiceNpoban { get; set; }
		/// <summary>発票連絡者</summary>
		[XmlElement(ElementName = "InvoiceContactPerson")]
		public string InvoiceContactPerson { get; set; }
		/// <summary>発票連絡電話番号</summary>
		[XmlElement(ElementName = "InvoiceContactTel")]
		public string InvoiceContactTel { get; set; }
		/// <summary>発票連絡電話住所</summary>
		[XmlElement(ElementName = "InvoiceDeliveryAddress")]
		public string InvoiceDeliveryAddress { get; set; }
		/// <summary>発票連絡メール</summary>
		[XmlElement(ElementName = "InvoiceContactEMail")]
		public string InvoiceContactEMail { get; set; }
		/// <summary>購入する商品</summary>
		[XmlArray("Datas")]
		[XmlArrayItem("Data")]
		public List<FlapsOrderProduct> Products { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class FlapsOrderProduct
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FlapsOrderProduct()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="product">商品</param>
		/// <param name="isTaxFree">TaxFreeかどうか</param>
		public FlapsOrderProduct(CartProduct product, bool isTaxFree)
		{
			var productCount = product.IsSetItem ? product.Count : product.QuantitiyUnallocatedToSet;
			this.Qty = productCount.ToString();
			this.GoodsCode = product.VariationId;
			this.PriceTax = isTaxFree ? 0 : product.PriceTax;

			var originalSalePrice = (product.PriceOrg - this.PriceTax);
			this.OriginalSalePrice = originalSalePrice.ToString("#");
			var taxRate = isTaxFree ? 0 : (product.TaxRate / 100m);
			var originalTaxedSalePrice = (originalSalePrice * (1 + taxRate));
			this.Price = originalTaxedSalePrice.ToString("#");
			this.OriginalTaxedSalePrice = originalTaxedSalePrice.ToString("#");
			this.OriginalTotal = (originalSalePrice * productCount).ToString("#");
			this.OriginalTaxedTotal = (originalTaxedSalePrice * productCount).ToString("#");
			
			var realSalePrice = (product.Price - this.PriceTax);
			this.RealSalePrice = realSalePrice.ToString("#");
			var realTaxedSalePrice = (realSalePrice * (1 + taxRate));
			this.RealTaxedSalePrice = realTaxedSalePrice.ToString("#");
			this.RealTotal = (realSalePrice * productCount).ToString("#");
			this.RealTaxedTotal = (realTaxedSalePrice * productCount).ToString("#");

			this.TaxTotal = (realTaxedSalePrice * productCount) - (realSalePrice * productCount);
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="product">商品</param>
		/// <param name="isTaxFree">TaxFreeかどうか</param>
		/// <param name="setPromotion">セットプロモーション</param>
		public FlapsOrderProduct(CartProduct product, bool isTaxFree, CartSetPromotion setPromotion)
		{
			var quantity = product.QuantityAllocatedToSet[setPromotion.CartSetPromotionNo];
			this.Qty = quantity.ToString();
			this.GoodsCode = product.VariationId;
			this.Price = product.Price.ToString("#");
			this.PriceTax = isTaxFree ? 0 : product.PriceTax;

			var originalSalePrice = (product.PriceOrg - this.PriceTax);
			this.OriginalSalePrice = originalSalePrice.ToString("#");
			var taxRate = isTaxFree ? 0 : (product.TaxRate / 100m);
			var originalTaxedSalePrice = (originalSalePrice * (1 + taxRate));
			this.Price = originalTaxedSalePrice.ToString("#");
			this.OriginalTaxedSalePrice = originalTaxedSalePrice.ToString("#");
			this.OriginalTotal = (originalSalePrice * quantity).ToString("#");
			this.OriginalTaxedTotal = (originalTaxedSalePrice * quantity).ToString("#");

			var realSalePrice = (product.Price - this.PriceTax);
			this.RealSalePrice = realSalePrice.ToString("#");
			var realTaxedSalePrice = (realSalePrice * (1 + taxRate));
			this.RealTaxedSalePrice = realTaxedSalePrice.ToString("#");
			this.RealTotal = (realSalePrice * quantity).ToString("#");
			this.RealTaxedTotal = (realTaxedSalePrice * quantity).ToString("#");

			this.TaxTotal = (realTaxedSalePrice * quantity) - (realSalePrice * quantity);
		}

		/// <summary>商品唯一番号</summary>
		[XmlElement(ElementName = "SerNo")]
		public string SerNo { get; set; }
		/// <summary>商品コード</summary>
		[XmlElement(ElementName = "GoodsCode")]
		public string GoodsCode { get; set; }
		/// <summary>商品数量</summary>
		[XmlElement(ElementName = "Qty")]
		public string Qty { get; set; }
		/// <summary>商品価格</summary>
		[XmlElement(ElementName = "Price")]
		public string Price { get; set; }
		/// <summary>原税抜商品金額</summary>
		[XmlElement(ElementName = "OriginalSalePrice")]
		public string OriginalSalePrice { get; set; }
		/// <summary>原税込み商品金額</summary>
		[XmlElement(ElementName = "OriginalTaxedSalePrice")]
		public string OriginalTaxedSalePrice { get; set; }
		/// <summary>原税抜商品総金額</summary>
		[XmlElement(ElementName = "OriginalTotal")]
		public string OriginalTotal { get; set; }
		/// <summary>原税込み商品総金額</summary>
		[XmlElement(ElementName = "OriginalTaxedTotal")]
		public string OriginalTaxedTotal { get; set; }
		/// <summary>実際（割引後）税抜商品金額</summary>
		[XmlElement(ElementName = "RealSalePrice")]
		public string RealSalePrice { get; set; }
		/// <summary>実際（割引後）税込み商品金額</summary>
		[XmlElement(ElementName = "RealTaxedSalePrice")]
		public string RealTaxedSalePrice { get; set; }
		/// <summary>実際（割引後）税込み商品総金額</summary>
		[XmlElement(ElementName = "RealTotal")]
		public string RealTotal { get; set; }
		/// <summary>実際（割引後）税抜商品総金額</summary>
		[XmlElement(ElementName = "RealTaxedTotal")]
		public string RealTaxedTotal { get; set; }
		/// <summary>商品税額</summary>
		[XmlIgnore]
		public decimal PriceTax { get; set; }
		/// <summary>商品税額トータル (商品税額 * 数量)</summary>
		[XmlIgnore]
		public decimal TaxTotal { get; set; }
	}
}
