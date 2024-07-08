/*
=========================================================================================================
  Module      : Zcom決済モデルアダプタ (ZcomDirectRequestModelAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.App.Common.Global.Region.Currency;
using w2.Common.Util;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.GMO.Zcom.Direct
{
	/// <summary>
	/// Zcom決済モデルアダプタ
	/// </summary>
	public class ZcomDirectRequestModelAdapter : BaseZcomDirectRequestAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private ZcomDirectRequestModelAdapter()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <param name="userCreditCard">ユーザークレジットカード情報</param>
		public ZcomDirectRequestModelAdapter(OrderModel order, UserCreditCard userCreditCard)
			: this(order, userCreditCard, null)
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <param name="userCreditCard">ユーザークレジットカード情報</param>
		/// <param name="apisetting">Api設定</param>
		public ZcomDirectRequestModelAdapter(OrderModel order, UserCreditCard userCreditCard, ZcomApiSetting apisetting)
			: base(apisetting)
		{
			this.Order = order;
			this.UserCreditCardInfo = userCreditCard;
		}

		/// <summary>
		/// ユーザーID取得
		/// </summary>
		/// <returns>ユーザーID</returns>
		protected override string GetUserId()
		{
			if (string.IsNullOrEmpty(this.PaymentUserId))
			{
				this.PaymentUserId = StringUtility.ToEmpty(this.UserCreditCardInfo.CooperationId);
			}

			return this.PaymentUserId;
		}

		/// <summary>
		/// ユーザー名取得
		/// </summary>
		/// <returns>ユーザー名</returns>
		protected override string GetUserName()
		{
			return StringUtility.ToEmpty(this.Order.Owner.OwnerName);
		}

		/// <summary>
		/// メールアドレス取得
		/// </summary>
		/// <returns>メールアドレス</returns>
		protected override string GetUserMailAdd()
		{
			return StringUtility.ToEmpty(this.Order.Owner.OwnerMailAddr);
		}

		/// <summary>
		/// 商品コード取得
		/// </summary>
		/// <returns>商品コード</returns>
		protected override string GetItemCode()
		{
			return StringUtility.ToEmpty(this.Order.Items.First().ProductId);
		}

		/// <summary>
		/// 商品名取得
		/// </summary>
		/// <returns>商品名</returns>
		protected override string GetItemName()
		{
			return StringUtility.ToEmpty(this.Order.Items.First().ProductName);
		}

		/// <summary>
		/// オーダー番号取得
		/// </summary>
		/// <returns>オーダー番号</returns>
		protected override string GetOrderNumber()
		{
			if (string.IsNullOrEmpty(this.PaymentOrderId)) { this.PaymentOrderId = OrderCommon.CreatePaymentOrderId(this.Order.ShopId); }
			return this.PaymentOrderId;
		}

		/// <summary>
		/// 価格取得
		/// </summary>
		/// <returns>価格</returns>
		protected override decimal GetItemPrice()
		{
			return CurrencyManager.GetSendingAmount(this.Order.LastBilledAmount, this.Order.SettlementAmount, this.Order.SettlementCurrency);
		}

		/// <summary>
		/// クレジットカード番号取得
		/// </summary>
		/// <returns>クレジットカード番号</returns>
		protected override string GetCardNumber()
		{
			// 登録カード使うので空
			return "";
		}

		/// <summary>
		/// 有効期限年取得
		/// </summary>
		/// <returns>有効期限年</returns>
		protected override string GetExpireYear()
		{
			// 登録カード使うので空
			return "";
		}

		/// <summary>
		/// 有効期限月取得
		/// </summary>
		/// <returns>有効期限月</returns>
		protected override string GetExpireMonth()
		{
			// 登録カード使うので空
			return "";
		}

		/// <summary>
		/// セキュリティコード取得
		/// </summary>
		/// <returns>セキュリティコード</returns>
		protected override string GetSecurityCode()
		{
			// 登録カード使うので空
			return "";
		}

		/// <summary>
		/// カードホルダー名取得
		/// </summary>
		/// <returns>カードホルダー名</returns>
		protected override string GetCardHolderName()
		{
			// 登録カード使うので空
			return "";
		}

		/// <summary>
		/// メモ1取得
		/// </summary>
		/// <returns>メモ1</returns>
		protected override string GetMemo1()
		{
			return "";
		}

		/// <summary>
		/// メモ2取得
		/// </summary>
		/// <returns>メモ2</returns>
		protected override string GetMemo2()
		{
			return "";
		}

		/// <summary>
		/// 追加1（仮売り・即時判断フラグ）取得
		/// </summary>
		/// <returns>追加1（仮売り・即時判断フラグ）</returns>
		protected override string GetAddInfo1()
		{
			// デジコンかどうかで設定分ける
			return (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && this.Order.IsDigitalContents)
				? Constants.PAYMENT_CREDIT_ZCOM_APIADDINFO1_FORDIGITALCONTENTS
				: Constants.PAYMENT_CREDIT_ZCOM_APIADDINFO1;
		}

		/// <summary>注文モデル</summary>
		public OrderModel Order { get; set; }
		/// <summary>ユーザークレジットカード情報</summary>
		public UserCreditCard UserCreditCardInfo { get; set; }
	}
}
