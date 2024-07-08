/*
=========================================================================================================
  Module      : 1円与信用のアダプタ (ZcomDirectRequestModelAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.User;
using w2.Common.Util;

namespace w2.App.Common.Order.Payment.GMO.Zcom.Direct
{
	/// <summary>
	/// 1円与信用のアダプタ
	/// </summary>
	/// <remarks>カード登録に利用</remarks>
	public class ZcomDirectRequestUserCreditCardAdapter : BaseZcomDirectRequestAdapter
	{
		/// <summary>ユーザークレジットカード情報</summary>
		private readonly UserCreditCardInput m_userCreditCardInput;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private ZcomDirectRequestUserCreditCardAdapter()
			: base()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userCreditCardInput">ユーザークレジットカード情報</param>
		public ZcomDirectRequestUserCreditCardAdapter(UserCreditCardInput userCreditCardInput)
			: this()
		{
			this.m_userCreditCardInput = userCreditCardInput;
		}

		/// <summary>
		/// ユーザーID取得
		/// </summary>
		/// <returns>ユーザーID</returns>
		protected override string GetUserId()
		{
			if (string.IsNullOrEmpty(this.PaymentUserId))
			{
				this.PaymentUserId = StringUtility.ToEmpty(this.m_userCreditCardInput.UserId) + "." + DateTime.Now.ToString("yyyyMMddHHmmssfff");
			}

			return this.PaymentUserId;
		}

		/// <summary>
		/// ユーザー名取得
		/// </summary>
		/// <returns>ユーザー名</returns>
		protected override string GetUserName()
		{
			return "CardRegister";
		}

		/// <summary>
		/// メールアドレス取得
		/// </summary>
		/// <returns>メールアドレス</returns>
		protected override string GetUserMailAdd()
		{
			return "";
		}

		/// <summary>
		/// 商品コード取得
		/// </summary>
		/// <returns>商品コード</returns>
		protected override string GetItemCode()
		{
			return "dummy";
		}

		/// <summary>
		/// 商品名取得
		/// </summary>
		/// <returns>商品名</returns>
		protected override string GetItemName()
		{
			return "dummy";
		}

		/// <summary>
		/// オーダー番号取得
		/// </summary>
		/// <returns>オーダー番号</returns>
		protected override string GetOrderNumber()
		{
			if (string.IsNullOrEmpty(this.PaymentOrderId)) { this.PaymentOrderId = OrderCommon.CreatePaymentOrderId(Constants.CONST_DEFAULT_SHOP_ID); }

			return this.PaymentOrderId;
		}

		/// <summary>
		/// 価格取得
		/// </summary>
		/// <returns>価格</returns>
		protected override decimal GetItemPrice()
		{
			return 1;
		}

		/// <summary>
		/// カード番号取得
		/// </summary>
		/// <returns>カード番号</returns>
		protected override string GetCardNumber()
		{
			return StringUtility.ToEmpty(this.m_userCreditCardInput.CardNo);
		}

		/// <summary>
		/// 有効期限年取得
		/// </summary>
		/// <returns>有効期限年</returns>
		protected override string GetExpireYear()
		{
			return "20" + StringUtility.ToEmpty(this.m_userCreditCardInput.ExpirationYear);
		}

		/// <summary>
		/// 有効期限月取得
		/// </summary>
		/// <returns>有効期限月</returns>
		protected override string GetExpireMonth()
		{
			return StringUtility.ToEmpty(this.m_userCreditCardInput.ExpirationMonth).TrimStart('0');
		}

		/// <summary>
		/// セキュリティコード取得
		/// </summary>
		/// <returns>セキュリティコード</returns>
		protected override string GetSecurityCode()
		{
			return StringUtility.ToEmpty(this.m_userCreditCardInput.SecurityCode);
		}

		/// <summary>
		/// カードホルダー名取得
		/// </summary>
		/// <returns>カードホルダー名</returns>
		protected override string GetCardHolderName()
		{
			return StringUtility.ToEmpty(this.m_userCreditCardInput.AuthorName);
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
	}
}
