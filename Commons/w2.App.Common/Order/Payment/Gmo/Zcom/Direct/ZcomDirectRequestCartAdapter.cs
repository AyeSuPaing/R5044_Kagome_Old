/*
=========================================================================================================
  Module      : Zcom決済カートアダプタ (ZcomDirectRequestCartAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.Common.Util;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.GMO.Zcom.Direct
{
	/// <summary>
	/// Zcom決済カートアダプタ
	/// </summary>
	public class ZcomDirectRequestCartAdapter : BaseZcomDirectRequestAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private ZcomDirectRequestCartAdapter()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート</param>
		public ZcomDirectRequestCartAdapter(CartObject cart)
			: this(cart, null)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="apisetting">api設定</param>
		public ZcomDirectRequestCartAdapter(CartObject cart, ZcomApiSetting apisetting)
			: base(apisetting)
		{
			this.Cart = cart;
		}

		/// <summary>
		/// ユーザーID取得
		/// </summary>
		/// <returns>ユーザーID</returns>
		protected override string GetUserId()
		{
			if (string.IsNullOrEmpty(this.PaymentUserId))
			{
				if (this.Cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
				{
					// 新規カードの場合は作る
					this.PaymentUserId = this.Cart.OrderUserId + "." + DateTime.Now.ToString("yyyyMMddHHmmssfff");
				}
				else
				{
					// 新規カードじゃない場合は流用
					this.PaymentUserId = this.Cart.Payment.UserCreditCard.CooperationId;
				}
			}
			return this.PaymentUserId;
		}

		/// <summary>
		/// ユーザー名取得
		/// </summary>
		/// <returns>ユーザー名</returns>
		protected override string GetUserName()
		{
			return StringUtility.ToEmpty(this.Cart.Owner.Name);
		}

		/// <summary>
		/// メールアドレス取得
		/// </summary>
		/// <returns>メールアドレス</returns>
		protected override string GetUserMailAdd()
		{
			return StringUtility.ToEmpty(this.Cart.Owner.MailAddr);
		}

		/// <summary>
		/// 商品コード取得
		/// </summary>
		/// <returns>商品コード</returns>
		protected override string GetItemCode()
		{
			return StringUtility.ToEmpty(this.Cart.Items.First().ProductId);
		}

		/// <summary>
		/// 商品名取得
		/// </summary>
		/// <returns>商品名</returns>
		protected override string GetItemName()
		{
			return StringUtility.ToEmpty(this.Cart.Items.First().ProductName);
		}

		/// <summary>
		/// オーダー番号取得
		/// </summary>
		/// <returns>オーダー番号</returns>
		protected override string GetOrderNumber()
		{
			if (string.IsNullOrEmpty(this.PaymentOrderId)) { this.PaymentOrderId = OrderCommon.CreatePaymentOrderId(this.Cart.ShopId); }

			return this.PaymentOrderId;
		}

		/// <summary>
		/// 価格取得
		/// </summary>
		/// <returns>価格</returns>
		protected override decimal GetItemPrice()
		{
			var total = this.Cart.SendingAmount;
			return total;
		}

		/// <summary>
		/// クレジットカード番号取得
		/// </summary>
		/// <returns>クレジットカード番号</returns>
		protected override string GetCardNumber()
		{
			if (this.Cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				// 新規の場合はカード番号返す
				return StringUtility.ToEmpty(this.Cart.Payment.CreditCardNo);
			}

			// 新規じゃなければ空
			return "";
		}

		/// <summary>
		/// 有効期限年取得
		/// </summary>
		/// <returns>有効期限年</returns>
		protected override string GetExpireYear()
		{
			if (this.Cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				return "20" + StringUtility.ToEmpty(this.Cart.Payment.CreditExpireYear);
			}

			return "";
		}

		/// <summary>
		/// 有効期限月取得
		/// </summary>
		/// <returns>有効期限月</returns>
		protected override string GetExpireMonth()
		{
			if (this.Cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				return StringUtility.ToEmpty(this.Cart.Payment.CreditExpireMonth).TrimStart('0');
			}

			return "";
		}

		/// <summary>
		/// セキュリティコード取得
		/// </summary>
		/// <returns>セキュリティコード</returns>
		protected override string GetSecurityCode()
		{
			if (this.Cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				return StringUtility.ToEmpty(this.Cart.Payment.CreditSecurityCode);
			}

			return "";
		}

		/// <summary>
		/// カードホルダー名取得
		/// </summary>
		/// <returns>カードホルダー名</returns>
		protected override string GetCardHolderName()
		{
			if (this.Cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			{
				return StringUtility.ToEmpty(this.Cart.Payment.CreditAuthorName);
			}
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
			return (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && this.Cart.HasDigitalContents)
				? Constants.PAYMENT_CREDIT_ZCOM_APIADDINFO1_FORDIGITALCONTENTS
				: Constants.PAYMENT_CREDIT_ZCOM_APIADDINFO1;
		}

		/// <summary>
		/// Get back url
		/// </summary>
		/// <returns>Back url</returns>
		protected override string GetBackUrl()
		{
			var backUrl = new UrlCreator(this.UrlString).CreateUrl();
			return backUrl;
		}

		/// <summary>
		/// Get error url
		/// </summary>
		/// <returns>Error url</returns>
		protected override string GetErrUrl()
		{
			var errorUrl = new UrlCreator(this.UrlString).CreateUrl();
			return errorUrl;
		}

		/// <summary>
		/// Get success url
		/// </summary>
		/// <returns>Success url</returns>
		protected override string GetSuccessUrl()
		{
			var successUrl = new UrlCreator(this.UrlString).CreateUrl();
			return successUrl;
		}

		/// <summary>カート</summary>
		protected CartObject Cart { get; set; }
		/// <summary>Url string</summary>
		protected string UrlString
		{
			get
			{
				var result = string.Format(
					"{0}{1}{2}{3}",
					Constants.PROTOCOL_HTTPS,
					Constants.SITE_DOMAIN,
					Constants.PATH_ROOT_FRONT_PC,
					Constants.PAGE_FRONT_PAYMENT_ZCOM_CARD_3DSECURE_GET_RESULT);
				return result;
			}
		}
	}
}
