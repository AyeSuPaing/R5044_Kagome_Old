/*
=========================================================================================================
  Module      : GMOアトカラ 注文登録 クラス(PaymentGmoAtokaraOrderRegister.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment.GMOAtokara.Utils;
using w2.Common.Util;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.GMOAtokara
{
	/// <summary>
	/// GMOアトカラ 注文登録 クラス
	/// </summary>
	public class PaymentGmoAtokaraOrderRegister : PaymentGmoAtokaraBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentGmoAtokaraOrderRegister()
			: this(PaymentGmoAtokaraSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">設定</param>
		public PaymentGmoAtokaraOrderRegister(
			PaymentGmoAtokaraSetting settings)
			: base(Constants.PAYMENT_GMOATOKARA_DEFERRED_URL_SHIPMENT, settings, new PaymentGmoAtokaraShipmentResponseData())
		{
		}

		/// <summary>
		/// リクエストXML作成
		/// </summary>
		/// <param name="gmoTransactionId">GMO取引ID</param>
		/// <param name="pdcompanycode">運送会社コード</param>
		/// <param name="slipno">発送伝票番号</param>
		/// <param name="invoiceIssueDate">請求書発行日</param>
		/// <param name="transactionType">取引種別</param>
		/// <returns>リクエストXML</returns>
		private XDocument CreateRequestXml(
			string gmoTransactionId,
			string pdcompanycode,
			string slipno,
			string invoiceIssueDate,
			string transactionType)
		{
			// XML作成
			var document = new XDocument(new XDeclaration("1.0", "UTF-8", ""));
			document.Add(
				new XElement("request",
					CreateShopInfoNode(),
					new XElement("transaction",
						new XElement("gmoTransactionId", gmoTransactionId),
						new XElement("pdcompanycode", pdcompanycode),
						new XElement("slipno", slipno),
						new XElement("invoiceIssueDate", invoiceIssueDate),
						new XElement("transactionType", transactionType)
					)
				));

			return document;
		}

		/// <summary>
		/// ポストデータ作成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>ポストデータ</returns>
		public List<KeyValuePair<string, string>> CreatePostData(OrderModel order)
		{
			var postData = CreatePostData(
				this.Settings.AuthenticationId,
				this.Settings.ShopCode,
				this.Settings.SmsAuthenticationPassword,
				"",
				"",
				order.OrderId,
				order.OrderDate.HasValue ? order.OrderDate.Value.Date.ToString("yyyy/MM/dd") : "",
				order.Owner.OwnerName,
				order.Owner.OwnerZip,
				order.Owner.OwnerAddr1 + order.Owner.OwnerAddr2 + order.Owner.OwnerAddr3 + order.Owner.OwnerAddr4,
				order.Owner.OwnerTel1,
				order.Owner.OwnerMailAddr,
				order.LastBilledAmount.ToPriceString(),
				PaymentGmoAtokaraConstants.TRANSACTIONTYPE_CLOSING,
				"",
				order.Shippings[0].ShippingName,
				order.Shippings[0].ShippingZip,
				order.Shippings[0].ShippingAddr1 + order.Shippings[0].ShippingAddr2 + order.Shippings[0].ShippingAddr3 + order.Shippings[0].ShippingAddr4,
				order.Shippings[0].ShippingTel1,
				"商品",
				order.Items[0].ProductPrice.ToPriceString(),
				order.Items[0].ItemQuantity.ToString(),
				PaymentGmoAtokaraUtil.CreateCheckSum(
					string.Format("{0}{1}", this.Settings.ShopCode, this.Settings.ConnectPassword),
					order.OrderId,
					order.Owner.OwnerTel1,
					order.Owner.OwnerZip,
					order.LastBilledAmount.ToPriceString(),
					"",
					"",
					"",
					"",
					"",
					"")
			);

			WriteLog(
				true,
				PaymentFileLogger.PaymentProcessingType.Request,
				this.GetType().Name,
				"",
				idKeyAndValueDictionary: postData
					.ToDictionary(row =>
						(string)row.Key, row => StringUtility.ToEmpty(row.Value)));

			return postData;
		}

		/// <summary>
		/// ポストデータ作成
		/// </summary>
		/// <param name="gmoAuthenticationId">認証ID</param>
		/// <param name="gmoShopCode">加盟店コード</param>
		/// <param name="gmoSmsPassword">SMS認証用パスワード</param>
		/// <param name="gmoHttpHeader">HTTPヘッダ情報</param>
		/// <param name="gmoDeviceInfo">デバイス情報</param>
		/// <param name="gmoShopTransactionId">加盟店取引ID</param>
		/// <param name="gmoShopOrderDate">加盟店注文日</param>
		/// <param name="gmoFullName">氏名（漢字）</param>
		/// <param name="gmoZipCode">郵便番号</param>
		/// <param name="gmoAddress">住所</param>
		/// <param name="gmoTel1">電話番号１</param>
		/// <param name="gmoEmail1">メールアドレス１</param>
		/// <param name="gmoBilledAmount">顧客請求額</param>
		/// <param name="gmoBnplTransactionType">取引種別</param>
		/// <param name="gmoPaymentCount">支払回数</param>
		/// <param name="gmoFullNameDelivery">配送先氏名（漢字）</param>
		/// <param name="gmoZipCodeDelivery">配送先郵便番号</param>
		/// <param name="gmoAddressDelivery">配送先住所</param>
		/// <param name="gmoTelDelivery">配送先電話番号</param>
		/// <param name="gmoDetailNameDetail">明細名</param>
		/// <param name="gmoDetailPriceDetail">明細単価</param>
		/// <param name="gmoDetailQuantityDetail">明細数量</param>
		/// <param name="gmoChksumReq">チェックサム</param>
		/// <returns>ポストデータリスト</returns>
		private List<KeyValuePair<string, string>> CreatePostData(
			string gmoAuthenticationId,
			string gmoShopCode,
			string gmoSmsPassword,
			string gmoHttpHeader,
			string gmoDeviceInfo,
			string gmoShopTransactionId,
			string gmoShopOrderDate,
			string gmoFullName,
			string gmoZipCode,
			string gmoAddress,
			string gmoTel1,
			string gmoEmail1,
			string gmoBilledAmount,
			string gmoBnplTransactionType,
			string gmoPaymentCount,
			string gmoFullNameDelivery,
			string gmoZipCodeDelivery,
			string gmoAddressDelivery,
			string gmoTelDelivery,
			string gmoDetailNameDetail,
			string gmoDetailPriceDetail,
			string gmoDetailQuantityDetail,
			string gmoChksumReq)
		{
			var result = new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>("gmoAuthenticationId", gmoAuthenticationId),
					new KeyValuePair<string, string>("gmoShopCode", gmoShopCode),
					new KeyValuePair<string, string>("gmoSmsPassword", gmoSmsPassword),
					new KeyValuePair<string, string>("gmoHttpHeader", gmoHttpHeader),
					new KeyValuePair<string, string>("gmoDeviceInfo", gmoDeviceInfo),
					new KeyValuePair<string, string>("gmoShopTransactionId", gmoShopTransactionId),
					new KeyValuePair<string, string>("gmoShopOrderDate", gmoShopOrderDate),
					new KeyValuePair<string, string>("gmoFullName", gmoFullName),
					new KeyValuePair<string, string>("gmoZipCode", gmoZipCode),
					new KeyValuePair<string, string>("gmoAddress", gmoAddress),
					new KeyValuePair<string, string>("gmoTel1", gmoTel1),
					new KeyValuePair<string, string>("gmoEmail1", gmoEmail1),
					new KeyValuePair<string, string>("gmoBilledAmount", gmoBilledAmount),
					new KeyValuePair<string, string>("gmoBnplTransactionType", gmoBnplTransactionType),
					new KeyValuePair<string, string>("gmoPaymentCount", gmoPaymentCount),
					new KeyValuePair<string, string>("gmoFullNameDelivery", gmoFullNameDelivery),
					new KeyValuePair<string, string>("gmoZipCodeDelivery", gmoZipCodeDelivery),
					new KeyValuePair<string, string>("gmoAddressDelivery", gmoAddressDelivery),
					new KeyValuePair<string, string>("gmoTelDelivery", gmoTelDelivery),
					new KeyValuePair<string, string>("gmoDetailNameDetail", gmoDetailNameDetail),
					new KeyValuePair<string, string>("gmoDetailPriceDetail", gmoDetailPriceDetail),
					new KeyValuePair<string, string>("gmoDetailQuantityDetail", gmoDetailQuantityDetail),
					new KeyValuePair<string, string>("gmoChksumReq", gmoChksumReq),
				};
			return result;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentGmoAtokaraShipmentResponseData ResponseData { get { return (PaymentGmoAtokaraShipmentResponseData)this.ResponseDataInner; } }
	}
}
