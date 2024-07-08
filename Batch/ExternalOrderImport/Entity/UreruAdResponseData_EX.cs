/*
=========================================================================================================
  Module      : つくーるAPI連携 レスポンスデータ(UreruAdResponseData_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using w2.Domain.AdvCode;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.User;
using w2.App.Common;
using w2.App.Common.Order;
using w2.App.Common.User;
using w2.Common.Util;
using w2.Domain.SubscriptionBox;
using System;

namespace w2.Commerce.Batch.ExternalOrderImport.Entity
{
/// <summary>
	/// つくーるAPI連携 レスポンスデータレコード
	/// </summary>
	public partial class UreruAdResponseDataItem
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UreruAdResponseDataItem()
		{
			this.ExternalImportStatus = Constants.FLG_ORDER_EXTERNAL_IMPORT_STATUS_ERROR;
			this.FixedPurchaseItemList = new List<FixedPurchaseItem>();
			this.ErrorMessage = new List<string>();
		}

		/// <summary>
		/// 商品IDを取得
		/// </summary>
		/// <param name="productCode">商品コード</param>
		/// <returns>商品ID</returns>
		public string GetProductId(string productCode)
		{
			var productId = productCode.Split(',')[0];
			return productId;
		}

		/// <summary>
		/// バリエーションIDを取得
		/// </summary>
		/// <param name="productCode">商品コード</param>
		/// <returns>バリエーションID</returns>
		public string GetVariationId(string productCode)
		{
			var variationId = (productCode.Split(',').Length > 1)
				? productCode.Split(',')[1]
				: productCode;
			return variationId;
		}

		/// <summary>
		/// 広告コードを取得
		/// </summary>
		/// <returns>広告コード</returns>
		public string GetAdvCode()
		{
			string tmp;
			var result = this.QueryParameters.TryGetValue(
				Constants.URERU_AD_IMPORT_QUERY_STRING_PARAMETER_UTM_CAMPAIGN,
				out tmp)
				? tmp
				: "";
			return result;
		}

		/// <summary>
		/// 頒布会コースIDを取得
		/// </summary>
		/// <returns>頒布会コースID</returns>
		public string GetSubscriptionBoxCourseId()
		{
			string tmp;
			var result = this.QueryParameters.TryGetValue(
				Constants.URERU_AD_IMPORT_QUERY_STRING_PARAMETER_SUBSCRIPTION_BOX_COURSE_ID,
				out tmp)
				? tmp
				: "";
			return result;
		}

		/// <summary>
		/// 決済注文IDを取得
		/// </summary>
		/// <param name="paymentKbn">決済種別ID</param>
		/// <returns>決済注文ID</returns>
		public string GetPaymentOrderId(string paymentKbn)
		{
			var paymentOrderId = string.Empty;
			switch (this.PaymentMethod)
			{
				case Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_CREDIT:
					switch (Constants.PAYMENT_CARD_KBN)
					{
						case Constants.PaymentCard.Zeus:
							break;

						case Constants.PaymentCard.YamatoKwc:
							paymentOrderId = this.CreditKuronekoWebCollectOrderNo;
							break;

						case Constants.PaymentCard.Gmo:
							paymentOrderId = this.CreditGmoOrderId;
							break;

						case Constants.PaymentCard.SBPS:
							paymentOrderId = this.CreditSoftBankPaymentOrderId;
							break;

						case Constants.PaymentCard.VeriTrans:
							paymentOrderId = this.CreditVeritransAccessId;
							break;

						case Constants.PaymentCard.EScott:
							break;
					}
					break;

				case Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_AMAZON_PAYMENTS:
					paymentOrderId = ((paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2) && this.IsFixedPurchase)
						? this.AmazonPaymentsId
						: this.AmazonPaymentsOrderId;
					break;

				case Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_NP:
				case Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_NP_WIZ:
					if (string.IsNullOrEmpty(this.NpTransactionId) == false)
					{
						paymentOrderId = this.ShopTransactionId;
					}
					break;

				default:
					break;
			}
			return paymentOrderId;
		}

		/// <summary>
		/// 決済カード取引IDを取得
		/// </summary>
		/// <returns>決済カード取引ID</returns>
		public string GetCardTranId()
		{
			var cardTranId = string.Empty;
			switch (this.PaymentMethod)
			{
				case Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_CREDIT:
					switch (Constants.PAYMENT_CARD_KBN)
					{
						case Constants.PaymentCard.Zeus:
							cardTranId = this.CreditZeusOrdd;
							break;

						case Constants.PaymentCard.YamatoKwc:
							cardTranId = this.CreditKuronekoWebCollectCrdCResCd;
							break;

						case Constants.PaymentCard.Gmo:
							cardTranId = this.CreditGmoAccessId + " " + this.CreditGmoAccessPass;
							break;

						case Constants.PaymentCard.SBPS:
							cardTranId = this.CreditSoftBankPaymentTrackingId;
							break;

						case Constants.PaymentCard.VeriTrans:
							break;

						case Constants.PaymentCard.EScott:
							cardTranId = this.CreditSonyPaymentKaiinId + "," + this.CreditSonyPaymentProcessId + "," + this.CreditSonyPaymentProcessPass + "," + this.CreditSonyPaymentKaiinPass;
							break;
					}
					break;
				
				case Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_AMAZON_PAYMENTS:
					cardTranId = this.AmazonPaymentsAuthId;
					break;

				case Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_NP:
				case Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_NP_WIZ:
					cardTranId = this.NpTransactionId;
					break;

				default:
				 break;
			}
			return cardTranId;
		}

		/// <summary>
		/// 連携IDを取得
		/// </summary>
		/// <returns>連携ID</returns>
		public string GetCooperationId()
		{
			var cooperationId = string.Empty;
			switch (Constants.PAYMENT_CARD_KBN)
			{
				case Constants.PaymentCard.Zeus:
					cooperationId = this.TelNoFull;
					break;

				case Constants.PaymentCard.YamatoKwc:
					cooperationId = this.CreditKuronekoWebCollectMemberId;
					break;

				case Constants.PaymentCard.Gmo:
					cooperationId = this.CreditGmoMemberId;
					break;

				case Constants.PaymentCard.SBPS:
					cooperationId = this.CreditSoftBankPaymentCustomerId;
					break;

				case Constants.PaymentCard.VeriTrans:
					break;

				case Constants.PaymentCard.EScott:
					cooperationId = this.CreditSonyPaymentKaiinId + "," + this.CreditSonyPaymentKaiinPass;
					break;
			}
			return cooperationId;
		}

		/// <summary>
		/// 連携ID2を取得
		/// </summary>
		/// <returns>連携ID2</returns>
		public string GetCooperationId2()
		{
			var cooperationId2 = string.Empty;
			switch (Constants.PAYMENT_CARD_KBN)
			{
				case Constants.PaymentCard.Zeus:
					cooperationId2 = this.CreditZeusSendId;
					break;

				case Constants.PaymentCard.YamatoKwc:
					cooperationId2 = this.CreditKuronekoWebCollectAuthenticationKey;
					break;

				case Constants.PaymentCard.Gmo:
					break;

				case Constants.PaymentCard.SBPS:
					break;

				case Constants.PaymentCard.VeriTrans:
					cooperationId2 = this.CreditVeritransAccessId;
					break;

				case Constants.PaymentCard.EScott:
					cooperationId2 = this.CreditSonyPaymentProcessId;
					break;
			}
			return cooperationId2;
		}

		/// <summary>
		/// 検証
		/// </summary>
		public void Validate()
		{
			ValidateUser();
			ValidateOrder();
			if (this.ErrorMessage.Any() == false) this.ExternalImportStatus = Constants.FLG_ORDER_EXTERNAL_IMPORT_STATUS_SUCCESS;
		}

		/// <summary>
		/// ユーザー情報検証
		/// </summary>
		private void ValidateUser()
		{
			if (this.IsOneClickAmazonPayments)
			{
				this.Name = this.AmazonPaymentsName;
				this.FamilyName = this.AmazonPaymentsName;
				this.ZipFullHyphen = this.AmazonPaymentsPostalCode;
				var amazonZip = new ZipCode(this.AmazonPaymentsPostalCode);
				this.Zip1 = amazonZip.Zip1;
				this.Zip2 = amazonZip.Zip2;
				this.AddressFull = this.Prefecture + this.AmazonPaymentsAddressLine1Zenkaku + this.AmazonPaymentsAddressLine2Zenkaku + this.AmazonPaymentsAddressLine3Zenkaku;
				this.Address1Zenkaku = this.AmazonPaymentsAddressLine1Zenkaku;
				this.Address2Zenkaku = this.AmazonPaymentsAddressLine2Zenkaku;
				this.Address3Zenkaku = this.AmazonPaymentsAddressLine3Zenkaku;
			}

			if (string.IsNullOrEmpty(this.Name))
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("氏名"));
			}
			else if (this.Name.Length > 40)
			{
				this.ErrorMessage.Add(CreateOverMaxLengthErrorMessage(this.Name, "氏名", 40));
				this.Name = this.Name.Substring(0, 40);
			}

			if (string.IsNullOrEmpty(this.FamilyName))
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("氏名(姓)"));
			}
			else if (this.FamilyName.Length > 20)
			{
				this.ErrorMessage.Add(CreateOverMaxLengthErrorMessage(this.FamilyName, "氏名(姓)", 20));
				this.FamilyName = this.FamilyName.Substring(0, 20);
			}

			if (this.IsOneClickAmazonPayments == false)
			{
				if (string.IsNullOrEmpty(this.GivenName))
				{
					this.ErrorMessage.Add(CreateNecessaryErrorMessage("氏名(名)"));
				}
				else if (this.GivenName.Length > 20)
				{
					this.ErrorMessage.Add(CreateOverMaxLengthErrorMessage(this.GivenName, "氏名(名)", 20));
					this.GivenName = this.GivenName.Substring(0, 20);
				}

				if (string.IsNullOrEmpty(this.Kana))
				{
					this.ErrorMessage.Add(CreateNecessaryErrorMessage("氏名(かな)"));
				}
				else if (this.Kana.Length > 60)
				{
					this.ErrorMessage.Add(CreateOverMaxLengthErrorMessage(this.Kana, "氏名(かな)", 60));
					this.Kana = this.Kana.Substring(0, 60);
				}

				if (string.IsNullOrEmpty(this.FamilyKana))
				{
					this.ErrorMessage.Add(CreateNecessaryErrorMessage("氏名(かな) 姓"));
				}
				else if (this.FamilyKana.Length > 30)
				{
					this.ErrorMessage.Add(CreateOverMaxLengthErrorMessage(this.FamilyKana, "氏名(かな) 姓", 30));
					this.FamilyKana = this.FamilyKana.Substring(0, 30);
				}

				if (string.IsNullOrEmpty(this.GivenKana))
				{
					this.ErrorMessage.Add(CreateNecessaryErrorMessage("氏名(かな) 名"));
				}
				else if (this.GivenKana.Length > 30)
				{
					this.ErrorMessage.Add(CreateOverMaxLengthErrorMessage(this.GivenKana, "氏名(かな) 名", 30));
					this.GivenKana = this.GivenKana.Substring(0, 30);
				}
			}

			if (string.IsNullOrEmpty(this.ZipFullHyphen))
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("郵便番号"));
			}

			if (string.IsNullOrEmpty(this.Zip1))
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("郵便番号(上3桁)"));
			}

			if (string.IsNullOrEmpty(this.Zip2))
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("郵便番号(下4桁)"));
			}

			if (string.IsNullOrEmpty(this.AddressFull))
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("住所"));
			}
			else if (this.AddressFull.Length > 200)
			{
				this.ErrorMessage.Add(CreateOverMaxLengthErrorMessage(this.AddressFull, "住所", 200));
				this.AddressFull = this.AddressFull.Substring(0, 200);
			}

			if (string.IsNullOrEmpty(this.Prefecture))
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("都道府県"));
			}

			if (string.IsNullOrEmpty(this.Address1Zenkaku))
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("市区町村"));
			}
			else if (this.Address1Zenkaku.Length > 50)
			{
				this.ErrorMessage.Add(CreateOverMaxLengthErrorMessage(this.Address1Zenkaku, "市区町村", 50));
				this.Address1Zenkaku = this.Address1Zenkaku.Substring(0, 50);
			}

			if (string.IsNullOrEmpty(this.Address2Zenkaku) && (this.IsOneClickAmazonPayments == false))
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("番地"));
			}
			else if (this.Address2Zenkaku.Length > 50)
			{
				this.ErrorMessage.Add(CreateOverMaxLengthErrorMessage(this.Address2Zenkaku, "番地", 50));
				this.Address2Zenkaku = this.Address2Zenkaku.Substring(0, 50);
			}

			if (this.Address3Zenkaku.Length > 50)
			{
				this.ErrorMessage.Add(CreateOverMaxLengthErrorMessage(this.Address3Zenkaku, "ビル・マンション名", 50));
				this.Address3Zenkaku = this.Address3Zenkaku.Substring(0, 50);
			}

			if (string.IsNullOrEmpty(this.TelNoFullHyphen))
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("電話番号"));
			}

			if (string.IsNullOrEmpty(this.TelNo1))
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("電話番号(市外局番)"));
			}

			if (string.IsNullOrEmpty(this.TelNo2))
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("電話番号(市内局番)"));
			}

			if (string.IsNullOrEmpty(this.TelNo3))
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("電話番号(加入者番号)"));
			}

			if (string.IsNullOrEmpty(this.Email))
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("メールアドレス"));
			}

			if (this.Created.HasValue == false)
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("受注日時"));
			}

			if (this.PaymentMethod == Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_CREDIT)
			{
				ValidateUserCreditCardCooperationId();
			}

			var advCode = GetAdvCode();
			if ((string.IsNullOrEmpty(advCode) == false)
				&& (new AdvCodeService().GetAdvCodeFromAdvertisementCode(advCode) == null))
			{
				this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_ADVCODE_NO_EXIST_ERROR)
					.Replace("@@ 1 @@", this.Id)
					.Replace("@@ 2 @@", advCode));
			}

			// 名前が異なるか、注文情報に外部連携取込ステータスが「警告」のままの注文があればこの注文も「警告」で取り込む
			var user = new UserService().GetUserByMailAddr(this.Email);
			if ((user != null) 
				&& ((user.Name != this.Name)
					|| new OrderService().GetOrdersByUserId(user.UserId).Any(order => order.ExternalImportStatus == Constants.FLG_ORDER_EXTERNAL_IMPORT_STATUS_WARNING)))
			{
				this.ExternalImportStatus = Constants.FLG_ORDER_EXTERNAL_IMPORT_STATUS_WARNING;
				this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_USER_INVALID_BATCH)
					.Replace("@@ 1 @@", this.Id));
			}
		}

		/// <summary>
		/// 注文情報検証
		/// </summary>
		private void ValidateOrder()
		{
			if (new []
				{
					Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_CREDIT,
					Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_COLLECT,
					Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_KURONEKO_PS,
					Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_GMO_PS,
					Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_AMAZON_PAYMENTS,
					Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_NONE,
					Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_NP,
					Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_NP_WIZ,
					Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_ATODENE,
					Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_ATOBARAI_COM,
				}.Any(payment => payment == this.PaymentMethod) == false)
			{
				this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_UNUSABLE_PAYMENT_METHOD).Replace("@@ 1 @@", this.Id));
			}

			var paymentkbn = ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, this.PaymentMethod);
			if (string.IsNullOrEmpty(paymentkbn))
			{
				this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_NOT_EXIST_PAYMENT_METHOD).Replace("@@ 1 @@", this.Id));
			}

			if (this.ProductTotalInc.HasValue == false)
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("小計"));
			}

			if (this.TotalInc.HasValue == false)
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("支払金額合計"));
			}

			var hasProduct = false;
			if ((string.IsNullOrEmpty(this.LandingProductCode) == false)
				|| (string.IsNullOrEmpty(this.LandingProductName) == false)
				|| this.LandingProductPriceInc.HasValue
				|| this.LandingProductQty.HasValue)
			{
				hasProduct = true;
				var subscriptionBoxCourseId = GetSubscriptionBoxCourseId();
				if (string.IsNullOrEmpty(subscriptionBoxCourseId) == false)
				{
					var subscriptionBox = new SubscriptionBoxService().GetByCourseId(subscriptionBoxCourseId);
					if (subscriptionBox == null)
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_SUBSCRIPTION_BOX_NOT_EXIST).Replace("@@ 1 @@", this.Id));
					}
				}

				if (string.IsNullOrEmpty(this.LandingProductCode))
				{
					this.ErrorMessage.Add(CreateNecessaryErrorMessage("商品ID"));
				}
				else
				{
					this.LandingProduct = new ProductService().GetProductVariation(
						Constants.CONST_DEFAULT_SHOP_ID,
						GetProductId(this.LandingProductCode),
						GetVariationId(this.LandingProductCode),
						string.Empty);
					if (this.LandingProduct == null)
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_PRODUCT_NOT_EXIST).Replace("@@ 1 @@", this.Id));
					}
					else
					{
						this.ShippingId = this.LandingProduct.ShippingId;
					}
				}

				if (string.IsNullOrEmpty(this.LandingProductName))
				{
					this.ErrorMessage.Add(CreateNecessaryErrorMessage("商品名"));
				}
				else if (this.LandingProductName.Length > 200)
				{
					this.ErrorMessage.Add(CreateOverMaxLengthErrorMessage(this.LandingProductName, "販売商品名", 200));
					this.LandingProductName = this.LandingProductName.Substring(0, 200);
				}

				if (this.LandingProductPriceInc.HasValue == false)
				{
					this.ErrorMessage.Add(CreateNecessaryErrorMessage("商品価格"));
				}

				if (this.LandingProductQty.HasValue == false)
				{
					this.ErrorMessage.Add(CreateNecessaryErrorMessage("購入数"));
				}

				if (this.IsFixedPurchase && this.LandingProduct.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) 
				{
					this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_PRODUCT_NOT_FIXEDPURCHASE_FLG).Replace("@@ 1 @@", this.Id));
					throw new Exception("対象商品の定期購入フラグがOFFのためエラーが発生しました");
				}
			}

			if ((string.IsNullOrEmpty(this.UpsellProductCode) == false)
				|| (string.IsNullOrEmpty(this.UpsellProductName) == false)
				|| this.UpsellProductPriceInc.HasValue
				|| this.UpsellProductQty.HasValue)
			{
				hasProduct = true;
				if (string.IsNullOrEmpty(this.UpsellProductCode))
				{
					this.ErrorMessage.Add(CreateNecessaryErrorMessage("アップセル商品ID"));
				}
				else
				{
					this.UpsellProduct = new ProductService().GetProductVariation(
						Constants.CONST_DEFAULT_SHOP_ID,
						GetProductId(this.UpsellProductCode),
						GetVariationId(this.UpsellProductCode),
						string.Empty);
					if (this.UpsellProduct == null)
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_PRODUCT_NOT_EXIST).Replace("@@ 1 @@", this.Id));
					}
					else
					{
						this.ShippingId = string.IsNullOrEmpty(this.ShippingId)
							? this.UpsellProduct.ShippingId
							: this.ShippingId;
					}
				}

				if (string.IsNullOrEmpty(this.UpsellProductName))
				{
					this.ErrorMessage.Add(CreateNecessaryErrorMessage("アップセル商品名"));
				}
				else if (this.UpsellProductName.Length > 200)
				{
					this.ErrorMessage.Add(CreateOverMaxLengthErrorMessage(this.UpsellProductName, "アップセル商品名", 200));
					this.UpsellProductName = this.UpsellProductName.Substring(0, 200);
				}

				if (this.UpsellProductPriceInc.HasValue == false)
				{
					this.ErrorMessage.Add(CreateNecessaryErrorMessage("アップセル商品価格"));
				}

				if (this.UpsellProductQty.HasValue == false)
				{
					this.ErrorMessage.Add(CreateNecessaryErrorMessage("アップセル購入数"));
				}
			}

			if (hasProduct == false)
			{
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("商品ID"));
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("商品名"));
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("商品価格"));
				this.ErrorMessage.Add(CreateNecessaryErrorMessage("購入数"));
			}

			if (string.IsNullOrEmpty(this.ShippingId))
			{
				this.ErrorMessage.Add(string.Format("つくーる注文ID：{0}<br />エラー理由：商品情報が取得できないため配送種別が決定できません。管理画面より配送種別を登録してください。", this.Id));
			}

			if (this.PaymentMethod == Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_CREDIT)
			{
				ValidateCardTranId();
				ValidatePaymentOrderId();
			}

			this.ExistsDuplicatedOrder = ((string.IsNullOrEmpty(this.Id) == false) && (new OrderService().GetOrderByExternalOrderId(this.Id) != null));
		}

		/// <summary>
		/// クレジットカード情報の連携ID検証
		/// </summary>
		private void ValidateUserCreditCardCooperationId()
		{
			switch (Constants.PAYMENT_CARD_KBN)
			{
				case Constants.PaymentCard.Zeus:
					if ((string.IsNullOrEmpty(this.TelNoFull)
						|| string.IsNullOrEmpty(this.CreditZeusSendId)))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_COOPERATION_ID).Replace("@@ 1 @@", this.Id));
					}
					break;

				case Constants.PaymentCard.YamatoKwc:
					if (this.IsFixedPurchase
						&& ((string.IsNullOrEmpty(this.CreditKuronekoWebCollectMemberId)
							|| string.IsNullOrEmpty(this.CreditKuronekoWebCollectAuthenticationKey))))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_COOPERATION_ID).Replace("@@ 1 @@", this.Id));
					}
					break;

				case Constants.PaymentCard.Gmo:
					if (this.IsFixedPurchase 
						&& string.IsNullOrEmpty(this.CreditGmoMemberId))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_COOPERATION_ID).Replace("@@ 1 @@", this.Id));
					}
					break;

				case Constants.PaymentCard.SBPS:
					if (string.IsNullOrEmpty(this.CreditSoftBankPaymentCustomerId))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_COOPERATION_ID).Replace("@@ 1 @@", this.Id));
					}
					break;

				case Constants.PaymentCard.VeriTrans:
					if (string.IsNullOrEmpty(this.CreditVeritransAccessId))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_COOPERATION_ID).Replace("@@ 1 @@", this.Id));
					}
					break;

				case Constants.PaymentCard.EScott:
					if (this.IsFixedPurchase
						&& string.IsNullOrEmpty(this.CreditSonyPaymentKaiinId))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_COOPERATION_ID).Replace("@@ 1 @@", this.Id));
					}
					break;
			}
		}

		/// <summary>
		/// 決済カード取引ID検証
		/// </summary>
		private void ValidateCardTranId()
		{
			switch (Constants.PAYMENT_CARD_KBN)
			{
				case Constants.PaymentCard.Zeus:
					if (string.IsNullOrEmpty(this.CreditZeusOrdd))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_CARD_TRAN_ID).Replace("@@ 1 @@", this.Id));
					}
					break;

				case Constants.PaymentCard.YamatoKwc:
					if (string.IsNullOrEmpty(this.CreditKuronekoWebCollectCrdCResCd))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_CARD_TRAN_ID).Replace("@@ 1 @@", this.Id));
					}
					break;

				case Constants.PaymentCard.Gmo:
					if ((string.IsNullOrEmpty(this.CreditGmoAccessId) 
						|| string.IsNullOrEmpty(this.CreditGmoAccessPass)))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_CARD_TRAN_ID).Replace("@@ 1 @@", this.Id));
					}
					break;

				case Constants.PaymentCard.SBPS:
					if (string.IsNullOrEmpty(this.CreditSoftBankPaymentTrackingId))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_CARD_TRAN_ID).Replace("@@ 1 @@", this.Id));
					}
					break;

				case Constants.PaymentCard.VeriTrans:
					if (string.IsNullOrEmpty(this.CreditVeritransAccessId))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_CARD_TRAN_ID).Replace("@@ 1 @@", this.Id));
					}
					break;

				case Constants.PaymentCard.EScott:
					if ((string.IsNullOrEmpty(this.CreditSonyPaymentProcessId)
						|| string.IsNullOrEmpty(this.CreditSonyPaymentProcessPass)))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_CARD_TRAN_ID).Replace("@@ 1 @@", this.Id));
					}
					break;
			}
		}

		/// <summary>
		/// 決済注文ID検証
		/// </summary>
		private void ValidatePaymentOrderId()
		{
			switch (Constants.PAYMENT_CARD_KBN)
			{
				case Constants.PaymentCard.Zeus:
					break; // 処理不要

				case Constants.PaymentCard.YamatoKwc:
					if (string.IsNullOrEmpty(this.CreditKuronekoWebCollectOrderNo))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_PAYMENT_ORDER_ID).Replace("@@ 1 @@", this.Id));
					}
					break;

				case Constants.PaymentCard.Gmo:
					if (string.IsNullOrEmpty(this.CreditGmoOrderId))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_PAYMENT_ORDER_ID).Replace("@@ 1 @@", this.Id));
					}
					break;

				case Constants.PaymentCard.SBPS:
					if (string.IsNullOrEmpty(this.CreditSoftBankPaymentOrderId))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_PAYMENT_ORDER_ID).Replace("@@ 1 @@", this.Id));
					}
					break;

				case Constants.PaymentCard.VeriTrans:
					if (string.IsNullOrEmpty(this.CreditVeritransAccessId))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_PAYMENT_ORDER_ID).Replace("@@ 1 @@", this.Id));
					}
					break;

				case Constants.PaymentCard.EScott:
					if ((string.IsNullOrEmpty(this.CreditSonyPaymentProcessId)
						|| string.IsNullOrEmpty(this.CreditSonyPaymentProcessPass)))
					{
						this.ErrorMessage.Add(CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_INVALID_PAYMENT_ORDER_ID).Replace("@@ 1 @@", this.Id));
					}
					break;
			}
		}

		/// <summary>
		/// 未入力・空文字時のエラーメッセージ作成
		/// </summary>
		/// <param name="message">プロパティ名</param>
		/// <returns>エラーメッセージ</returns>
		private string CreateNecessaryErrorMessage(string propertyName)
		{
			var errorMessage = CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_NECESSARY)
				.Replace("@@ 1 @@", this.Id)
				.Replace("@@ 2 @@", propertyName);
			return errorMessage;
		}

		/// <summary>
		/// 文字数超過エラーメッセージ作成
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="propertyName">プロパティ名</param>
		/// <param name="max">最大長</param>
		/// <returns>エラーメッセージ</returns>
		private string CreateOverMaxLengthErrorMessage(string input, string propertyName, int max)
		{
			var errorMessage = CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_LENGTH_MAX)
				.Replace("@@ 1 @@", this.Id)
				.Replace("@@ 2 @@", propertyName)
				.Replace("@@ 3 @@", max.ToString())
				.Replace("@@ 4 @@", input);
			return errorMessage;
		}
		
		#region プロパティ
		/// <summary>エラーメッセージ</summary>
		public List<string> ErrorMessage { get; set; }
		/// <summary>外部連携取込ステータス</summary>
		public string ExternalImportStatus { get; set; }
		/// <summary>共通の外部連携受注IDを持つ注文があるか</summary>
		public bool ExistsDuplicatedOrder { get; set; }
		/// <summary>定期注文？</summary>
		public bool IsFixedPurchase 
		{ 
			get 
			{ 
				return ((this.LandingProductRecurringFlg == Constants.FLG_URERU_AD_IMPORT_PRODUCT_RECURRING_FLG_FIXED_PURCHASE)
					|| (this.UpsellProductRecurringFlg == Constants.FLG_URERU_AD_IMPORT_PRODUCT_RECURRING_FLG_FIXED_PURCHASE)); 
			}
		}
		/// <summary>クレジットカード枝番</summary>
		public int CreditBranchNo { get; set; }
		/// <summary>注文ID</summary>
		public string OrderId { get; set; }
		/// <summary>定期注文ID</summary>
		public string FixedPurchaseId { get; set; }
		/// <summary>配送種別ID</summary>
		public string ShippingId { get; set; }
		/// <summary>ユーザー情報</summary>
		public UserModel User { get; set; }
		/// <summary>販売商品</summary>
		public ProductModel LandingProduct { get; set; }
		/// <summary>アップセル販売商品</summary>
		public ProductModel UpsellProduct { get; set; }
		/// <summary>カートオブジェクト</summary>
		public CartObject Cart { get; set; }
		/// <summary>定期注文商品情報</summary>
		public List<FixedPurchaseItem> FixedPurchaseItemList { get; set; }
		/// <summary>Amazon Pay?</summary>
		public bool IsOneClickAmazonPayments
		{
			get
			{
				// ワンクリック決済モード以外の場合、通常通り入力されている値を利用される
				// ここではAmazonPaymentsAddressLine1Zenkaku が空かどうかでワンクリック決済かどうかを判定している
				var result = ((this.PaymentMethod == Constants.FLG_URERU_AD_IMPORT_PAYMENT_METHOD_AMAZON_PAYMENTS)
					&& (string.IsNullOrEmpty(this.AmazonPaymentsAddressLine1Zenkaku) == false));
				return result;
			}
		}
		/// <summary>Amazon Pay：住所1（全角）</summary>
		public string AmazonPaymentsAddressLine1Zenkaku
		{
			get { return StringUtility.ToZenkaku(this.AmazonPaymentsAddressLine1); }
		}
		/// <summary>Amazon Pay：住所2（全角）</summary>
		public string AmazonPaymentsAddressLine2Zenkaku
		{
			get { return StringUtility.ToZenkaku(this.AmazonPaymentsAddressLine2); }
		}
		/// <summary>Amazon Pay：住所3（全角）</summary>
		public string AmazonPaymentsAddressLine3Zenkaku
		{
			get { return StringUtility.ToZenkaku(this.AmazonPaymentsAddressLine3); }
		}
		/// <summary>クエリパラメータ</summary>
		public Dictionary<string, string> QueryParameters
		{
			get
			{
				return _queryParameters = _queryParameters
					?? this.QueryString
						.Split('&')
						.Select(section => section.Split('='))
						.GroupBy(pair => pair[0])
						.ToDictionary(
							group => group.Key,
							group => group.Last().ElementAtOrDefault(1) ?? "");
			}
		}
		private Dictionary<string, string> _queryParameters;
		#endregion
	}

	/// <summary>
	/// 定期注文商品
	/// </summary>
	public class FixedPurchaseItem
	{
		/// <summary>商品ID</summary>
		public string ProductId { get; set; }
		/// <summary>バリエーションID</summary>
		public string VariationId { get; set; }
		/// <summary>サプライヤーID</summary>
		public string SupplierId { get; set; }
		/// <summary>注文数</summary>
		public int ItemQuantity { get; set; }
		/// <summary>注文数（セット未考慮）</summary>
		public int ItemQuantitySingle { get; set; }
		/// <summary>小計</summary>
		public decimal ItemPrice { get; set; }
		/// <summary>小計（セット未考慮）</summary>
		public decimal ItemPriceSingle {get; set; } 
	}
}
