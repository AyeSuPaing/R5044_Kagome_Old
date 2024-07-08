/*
=========================================================================================================
  Module      : 注文情報保持クラス／注文データ抽象定義 (Order.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using w2.Commerce.MallBatch.MailOrderGetter.MailAnalyze;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.Commerce.MallBatch.MailOrderGetter.Process.Base
{
	///**************************************************************************************
	/// <summary>
	/// 注文情報保持クラス
	/// </summary>
	///**************************************************************************************
	public abstract class Order : BaseProcess
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Order()
		{
			// プロパティ初期化
			this.To = new MailAddress();
			this.OrderItems = new List<object>();
		}

		/// <summary>
		/// プロパティへ設定する前の値チェックを行う
		/// </summary>
		/// <param name="htParam">値</param>
		/// <param name="strKey">キー</param>
		protected static string CheckProperty(Hashtable htParam, string strKey)
		{
			try
			{
				if (strKey == null)
				{
					throw new NullReferenceException("[CheckProperty]key にnullを渡すことはできません");
				}

				if (htParam[strKey] != null)
				{
					return (string)htParam[strKey];
				}
				else
				{
					throw new NullReferenceException("[CheckProperty]" + strKey + "を参照できませんでした。");
				}
			}
			catch (NullReferenceException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new Exception("[CheckProperty]予期しない例外", ex);
			}
		}

		/// <summary>
		/// 決済区分名称から決済区分を取得する
		/// </summary>
		/// <param name="orderPayment">決済区分名称</param>
		/// <returns>決済区分</returns>
		protected static string GetOrderPaymentKbn(string orderPayment)
		{
			string result = orderPayment;

			if (orderPayment.IndexOf(Constants.ORDER_PAYMENT_KBN_NAME_CREDIT) != -1)
			{
				result = Constants.ORDER_PAYMENT_KBN_NAME_CREDIT;
			}
			else if (orderPayment.IndexOf(Constants.ORDER_PAYMENT_KBN_NAME_CASH_ON_DELIVERY) != -1)
			{
				result = Constants.ORDER_PAYMENT_KBN_NAME_CASH_ON_DELIVERY;
			}
			else if (orderPayment.IndexOf(Constants.ORDER_PAYMENT_KBN_NAME_BANK_TRANSFER_CASH_BEFORE_DELIVERY) != -1)
			{
				result = Constants.ORDER_PAYMENT_KBN_NAME_BANK_TRANSFER_CASH_BEFORE_DELIVERY;
			}
			else if (orderPayment.IndexOf(Constants.ORDER_PAYMENT_KBN_NAME_BANK_TRANSFER_DEFERRED_PAYMENT) != -1)
			{
				result = Constants.ORDER_PAYMENT_KBN_NAME_BANK_TRANSFER_DEFERRED_PAYMENT;
			}
			else if (orderPayment.IndexOf(Constants.ORDER_PAYMENT_KBN_NAME_CONVENIENCE_STORE_CASH_BEFORE_DELIVERY) != -1)
			{
				result = Constants.ORDER_PAYMENT_KBN_NAME_CONVENIENCE_STORE_CASH_BEFORE_DELIVERY;
			}
			else if (orderPayment.IndexOf(Constants.ORDER_PAYMENT_KBN_NAME_CONVENIENCE_STORE_DEFERRED_PAYMENT) != -1)
			{
				result = Constants.ORDER_PAYMENT_KBN_NAME_CONVENIENCE_STORE_DEFERRED_PAYMENT;
			}
			else if (orderPayment.IndexOf(Constants.ORDER_PAYMENT_KBN_NAME_POST_TRANSFER_CASH_BEFORE_DELIVERY) != -1)
			{
				result = Constants.ORDER_PAYMENT_KBN_NAME_POST_TRANSFER_CASH_BEFORE_DELIVERY;
			}
			else if (orderPayment.IndexOf(Constants.ORDER_PAYMENT_KBN_NAME_POST_TRANSFER_DEFERRED_PAYMENT) != -1)
			{
				result = Constants.ORDER_PAYMENT_KBN_NAME_POST_TRANSFER_DEFERRED_PAYMENT;
			}
			else if (orderPayment.IndexOf(Constants.ORDER_PAYMENT_KBN_NAME_NON_PAYMENT) != -1)
			{
				result = Constants.ORDER_PAYMENT_KBN_NAME_NON_PAYMENT;
			}
			else
			{
				// コンビニ決済などの場合、支払方法に改行が入る場合があるため、先頭行のみ取得
				// <例>
				// --------------
				// [支払方法] セブンイレブン決済(前払)
				// のちほどショップより払込票のURLをメールにてご連絡します。
				// 払込票をプリントアウトし、お近くのセブンイレブンレジにて
				// 期限内にお支払い下さい。入金確認後、商品を発送します。
				// --------------
				result = orderPayment.Split('\r')[0];
			}

			return result;
		}

		/// <summary>
		/// 別出荷フラグをチェック
		/// </summary>
		/// <param name="owner">注文者情報</param>
		/// <param name="shipping">配送先情報</param>
		/// <returns>有効:true</returns>
		protected bool IsAnotherShippingFlagValid(OrderOwner owner, ShippingTo shipping)
		{
			return IsAnotherShippingFlagValid(
				StringUtility.ToEmpty(owner.OrderOwnerName1),
				StringUtility.ToEmpty(owner.OrderOwnerName2),
				StringUtility.ToEmpty(owner.Zip),
				StringUtility.ToEmpty(owner.Addr1),
				StringUtility.ToEmpty(owner.Addr2),
				StringUtility.ToEmpty(owner.Addr3),
				StringUtility.ToEmpty(owner.Addr4),
				StringUtility.ToEmpty(owner.Phone),
				StringUtility.ToEmpty(shipping.ShippingToName1),
				StringUtility.ToEmpty(shipping.ShippingToName2),
				StringUtility.ToEmpty(shipping.Zip),
				StringUtility.ToEmpty(shipping.Addr1),
				StringUtility.ToEmpty(shipping.Addr2),
				StringUtility.ToEmpty(shipping.Addr3),
				StringUtility.ToEmpty(shipping.Addr4),
				StringUtility.ToEmpty(shipping.Phone));
		}
		/// <summary>
		/// 別出荷フラグをチェック
		/// </summary>
		/// <param name="ownerName1">注文者の氏名1</param>
		/// <param name="ownerName2">注文者の氏名2</param>
		/// <param name="ownerZip">注文者の郵便番号</param>
		/// <param name="ownerAddr1">注文者の住所1</param>
		/// <param name="ownerAddr2">注文者の住所2</param>
		/// <param name="ownerAddr3">注文者の住所3</param>
		/// <param name="ownerAddr4">注文者の住所4</param>
		/// <param name="ownerPhone">注文者の電話番号</param>
		/// <param name="shippingToName1">配送先の氏名1</param>
		/// <param name="shippingToName2">配送先の氏名2</param>
		/// <param name="shippingToZip">配送先の郵便番号</param>
		/// <param name="shippingToAddr1">配送先の住所1</param>
		/// <param name="shippingToAddr2">配送先の住所2</param>
		/// <param name="shippingToAddr3">配送先の住所3</param>
		/// <param name="shippingToAddr4">配送先の住所4</param>
		/// <param name="shippingToPhone">配送先の電話番号</param>
		/// <returns>true：別出荷有効（配送先が注文者と異なる）、false：別出荷無効（配送先が注文者と同じ）</returns>
		protected static bool IsAnotherShippingFlagValid(
			string ownerName1,
			string ownerName2,
			string ownerZip,
			string ownerAddr1,
			string ownerAddr2,
			string ownerAddr3,
			string ownerAddr4,
			string ownerPhone,
			string shippingToName1,
			string shippingToName2,
			string shippingToZip,
			string shippingToAddr1,
			string shippingToAddr2,
			string shippingToAddr3,
			string shippingToAddr4,
			string shippingToPhone)
		{
			return ((ownerName1 != shippingToName1)
				|| (ownerName2 != shippingToName2)
				|| (ownerZip != shippingToZip)
				|| (ownerAddr1 != shippingToAddr1)
				|| (ownerAddr2 != shippingToAddr2)
				|| (ownerAddr3 != shippingToAddr3)
				|| (ownerAddr4 != shippingToAddr4)
				|| (ownerPhone != shippingToPhone));
		}

		/// <summary>
		/// 注文情報をDBへ投入する
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strMallId">モールID</param>
		/// <param name="strMailFilePath">操作中のメールパス</param>
		/// <param name="strBaseMailDetail">注文メール詳細情報</param>
		public abstract void InsertOrder(
			SqlAccessor sqlAccessor,
			string strShopId,
			string strMallId,
			string strMailFilePath,
			string strBaseMailDetail);

		/// <summary>注文区分</summary>
		public string OrderKbn { get; protected set; }

		/// <summary>モバイル注文フラグ　true時モバイル／false時ＰＣ</summary>
		public bool MobileFlag { get; protected set; }

		/// <summary>注文者メールアドレス</summary>
		public MailAddress To { get; protected set; }

		/// <summary>受注番号</summary>
		public string OrderNo { get; protected set; }

		/// <summary>日時</summary>
		public string OrderDateTime { get; protected set; }

		/// <summary>注文者</summary>
		public string OrderOwner { get; protected set; }

		/// <summary>支払方法</summary>
		public string OrderPaymentKbn { get; protected set; }

		/// <summary>支払方法/元記述</summary>
		public string OrderPaymentKbnFull { get; protected set; }

		/// <summary>ポイント利用方法</summary>
		public string PointUsage { get; protected set; }

		/// <summary>配送方法</summary>
		public string ShippingStyle { get; protected set; }

		/// <summary>配送時間指定</summary>
		public string ShippingOrderTime { get; protected set; }

		/// <summary>配送日指定</summary>
		public string ShippingOrderDate { get; protected set; }

		/// <summary>備考</summary>
		public string Summary { get; protected set; }

		/// <summary>ショップ名</summary>
		public string ShopName { get; protected set; }

		/// <summary>ショップID</summary>
		public string ShopId { get; protected set; }

		/// <summary>送付先</summary>
		public string ShippingTo { get; protected set; }

		/// <summary>商品</summary>
		public string Products { get; protected set; }

		/// <summary>小計</summary>
		public decimal Subtotal { get; protected set; }

		/// <summary>消費税</summary>
		public decimal Tax { get; protected set; }

		/// <summary>送料</summary>
		public decimal ShippingCharge { get; protected set; }

		/// <summary>代引料</summary>
		public decimal CODCommission { get; protected set; }

		/// <summary>ポイント利用(金額)</summary>
		public decimal PointUsePrice { get; protected set; }

		/// <summary>調整金額 (小計 + 配送料金 + 代引手数料 + 消費税 - 総合計) * -1</summary>
		public decimal ReglationPrice
		{
			get { return (this.Subtotal + this.ShippingCharge + this.CODCommission + this.Tax - this.GrandTotal) * -1; }
		}

		/// <summary>合計</summary>
		public decimal GrandTotal { get; protected set; }

		/// <summary>注文商品　※OrderItemクラスを継承したクラスを格納する</summary>
		public List<object> OrderItems { get; protected set; }

		/// <summary>調整金額メモ</summary>
		public string RegulationMemo { get; protected set; }

		/// <summary>外部連携メモ</summary>
		public string RelationMemo { get; protected set; }

		/// <summary>Coupon Use Price</summary>
		public decimal CouponUsePrice { get; protected set; }
	}
}
