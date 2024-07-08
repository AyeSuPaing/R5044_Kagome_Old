/*
=========================================================================================================
  Module      : GMOアトカラ 取引変更・キャンセル APIクラス(PaymentGmoAtokaraCancelApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;
using w2.Domain.Order;
using w2.App.Common.Order.Payment.GMOAtokara.Utils;

namespace w2.App.Common.Order.Payment.GMOAtokara
{
	/// <summary>
	/// GMOアトカラ 取引変更・キャンセル APIクラス
	/// </summary>
	public class PaymentGmoAtokaraCancelApi : PaymentGmoAtokaraBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentGmoAtokaraCancelApi()
			: this(PaymentGmoAtokaraSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">設定</param>
		public PaymentGmoAtokaraCancelApi(
			PaymentGmoAtokaraSetting settings)
			: base(Constants.PAYMENT_GMOATOKARA_DEFERRED_URL_ORDERMODIFYCANCEL, settings, new PaymentGmoAtokaraCancelResponseData())
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="updateKind">取引更新種別</param>
		/// <param name="order">注文情報</param>
		/// <returns>実行結果</returns>
		public bool Exec(
			PaymentGmoAtokaraTypes.UpdateKind updateKind,
			OrderModel order)
		{
			return Exec(
				updateKind,
				order.CardTranId,
				order.OrderId,
				order.OrderDate.HasValue ? order.OrderDate.Value.Date.ToString("yyyy/MM/dd") : "",
				order.Owner.OwnerName,
				order.Owner.OwnerNameKana,
				order.Owner.OwnerZip,
				order.Owner.OwnerAddr1 + order.Owner.OwnerAddr2 + order.Owner.OwnerAddr3 + order.Owner.OwnerAddr4,
				"",
				"",
				order.Owner.OwnerTel1,
				"",
				order.Owner.OwnerMailAddr,
				"",
				order.LastBilledAmount.ToPriceString(),
				"",
				"",
				"",
				"",
				PaymentGmoAtokaraConstants.TRANSACTIONTYPE_CLOSING,
				order.Shippings[0].ShippingName,
				"",
				order.Shippings[0].ShippingZip,
				order.Shippings[0].ShippingAddr1 + order.Shippings[0].ShippingAddr2 + order.Shippings[0].ShippingAddr3 + order.Shippings[0].ShippingAddr4,
				"",
				"",
				order.Shippings[0].ShippingTel1,
				order.Items[0].ProductName,
				order.Items[0].ProductPrice.ToPriceString(),
				order.Items[0].ItemQuantity.ToString(),
				"",
				"",
				"",
				"",
				"");
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="updateKind">取引更新種別</param>
		/// <param name="gmoTransactionId">GMO取引ID</param>
		/// <param name="shopTransactionId">加盟店取引ID</param>
		/// <param name="shopOrderDate">加盟店注文日</param>
		/// <param name="fullName">氏名（漢字）</param>
		/// <param name="fullKanaName">氏名（かな）</param>
		/// <param name="zipCode">郵便番号</param>
		/// <param name="address">住所</param>
		/// <param name="companyName">会社名</param>
		/// <param name="departmentName">部署名</param>
		/// <param name="tel1">電話番号１</param>
		/// <param name="tel2">電話番号２</param>
		/// <param name="email1">メールアドレス１</param>
		/// <param name="email2">メールアドレス２</param>
		/// <param name="billedAmount">顧客請求額</param>
		/// <param name="gmoExtend1">GMO拡張項目１</param>
		/// <param name="sex">性別</param>
		/// <param name="birthday">誕生日</param>
		/// <param name="memberId">会員ID</param>
		/// <param name="transactionType">取引種別</param>
		/// <param name="deliveryFullName">配送先氏名（漢字）</param>
		/// <param name="deliveryFullKanaName">配送先氏名（かな）</param>
		/// <param name="deliveryZipCode">配送先郵便番号</param>
		/// <param name="deliveryAddress">配送先住所</param>
		/// <param name="deliveryCompanyName">配送先会社名</param>
		/// <param name="deliveryDepartmentName">配送先部署名</param>
		/// <param name="deliveryTel">配送先電話番号</param>
		/// <param name="detailName">明細名</param>
		/// <param name="detailPrice">明細単価</param>
		/// <param name="detailQuantity">明細数量</param>
		/// <param name="gmoExtend2">GMO拡張項目２</param>
		/// <param name="gmoExtend3">GMO拡張項目３</param>
		/// <param name="gmoExtend4">GMO拡張項目４</param>
		/// <param name="detailBrand">ブランド</param>
		/// <param name="detailCategory">カテゴリ</param>
		/// <returns>実行結果</returns>
		private bool Exec(
			PaymentGmoAtokaraTypes.UpdateKind updateKind,
			string gmoTransactionId,
			string shopTransactionId,
			string shopOrderDate,
			string fullName,
			string fullKanaName,
			string zipCode,
			string address,
			string companyName,
			string departmentName,
			string tel1,
			string tel2,
			string email1,
			string email2,
			string billedAmount,
			string gmoExtend1,
			string sex,
			string birthday,
			string memberId,
			string transactionType,
			string deliveryFullName,
			string deliveryFullKanaName,
			string deliveryZipCode,
			string deliveryAddress,
			string deliveryCompanyName,
			string deliveryDepartmentName,
			string deliveryTel,
			string detailName,
			string detailPrice,
			string detailQuantity,
			string gmoExtend2,
			string gmoExtend3,
			string gmoExtend4,
			string detailBrand,
			string detailCategory)
		{
			var requestXml = CreateRequestXml(
				updateKind,
				gmoTransactionId,
				shopTransactionId,
				shopOrderDate,
				fullName,
				fullKanaName,
				zipCode,
				address,
				companyName,
				departmentName,
				tel1,
				tel2,
				email1,
				email2,
				billedAmount,
				gmoExtend1,
				sex,
				birthday,
				memberId,
				transactionType,
				deliveryFullName,
				deliveryFullKanaName,
				deliveryZipCode,
				deliveryAddress,
				deliveryCompanyName,
				deliveryDepartmentName,
				deliveryTel,
				detailName,
				detailPrice,
				detailQuantity,
				gmoExtend2,
				gmoExtend3,
				gmoExtend4,
				detailBrand,
				detailCategory);

			return Post(requestXml);
		}

		/// <summary>
		/// リクエストXML作成
		/// </summary>
		/// <param name="updateKind">取引更新種別</param>
		/// <param name="gmoTransactionId">GMO取引ID</param>
		/// <param name="shopTransactionId">加盟店取引ID</param>
		/// <param name="shopOrderDate">加盟店注文日</param>
		/// <param name="fullName">氏名（漢字）</param>
		/// <param name="fullKanaName">氏名（かな）</param>
		/// <param name="zipCode">郵便番号</param>
		/// <param name="address">住所</param>
		/// <param name="companyName">会社名</param>
		/// <param name="departmentName">部署名</param>
		/// <param name="tel1">電話番号１</param>
		/// <param name="tel2">電話番号２</param>
		/// <param name="email1">メールアドレス１</param>
		/// <param name="email2">メールアドレス２</param>
		/// <param name="billedAmount">顧客請求額</param>
		/// <param name="gmoExtend1">GMO拡張項目１</param>
		/// <param name="sex">性別</param>
		/// <param name="birthday">誕生日</param>
		/// <param name="memberId">会員ID</param>
		/// <param name="transactionType">取引種別</param>
		/// <param name="deliveryFullName">配送先氏名（漢字）</param>
		/// <param name="deliveryFullKanaName">配送先氏名（かな）</param>
		/// <param name="deliveryZipCode">配送先郵便番号</param>
		/// <param name="deliveryAddress">配送先住所</param>
		/// <param name="deliveryCompanyName">配送先会社名</param>
		/// <param name="deliveryDepartmentName">配送先部署名</param>
		/// <param name="deliveryTel">配送先電話番号</param>
		/// <param name="detailName">明細名</param>
		/// <param name="detailPrice">明細単価</param>
		/// <param name="detailQuantity">明細数量</param>
		/// <param name="gmoExtend2">GMO拡張項目２</param>
		/// <param name="gmoExtend3">GMO拡張項目３</param>
		/// <param name="gmoExtend4">GMO拡張項目４</param>
		/// <param name="detailBrand">ブランド</param>
		/// <param name="detailCategory">カテゴリ</param>
		/// <returns>リクエストXML</returns>
		private XDocument CreateRequestXml(
			PaymentGmoAtokaraTypes.UpdateKind updateKind,
			string gmoTransactionId,
			string shopTransactionId,
			string shopOrderDate,
			string fullName,
			string fullKanaName,
			string zipCode,
			string address,
			string companyName,
			string departmentName,
			string tel1,
			string tel2,
			string email1,
			string email2,
			string billedAmount,
			string gmoExtend1,
			string sex,
			string birthday,
			string memberId,
			string transactionType,
			string deliveryFullName,
			string deliveryFullKanaName,
			string deliveryZipCode,
			string deliveryAddress,
			string deliveryCompanyName,
			string deliveryDepartmentName,
			string deliveryTel,
			string detailName,
			string detailPrice,
			string detailQuantity,
			string gmoExtend2,
			string gmoExtend3,
			string gmoExtend4,
			string detailBrand,
			string detailCategory)
		{
			var document = new XDocument(new XDeclaration("1.0", "UTF-8", ""));
			document.Add(
				new XElement("request",
					CreateShopInfoNode(),
					new XElement("kindInfo",
						new XElement("updateKind", (int)updateKind)
					),
					new XElement("buyer",
						new XElement("gmoTransactionId", gmoTransactionId),
						new XElement("shopTransactionId", shopTransactionId),
						new XElement("shopOrderDate", shopOrderDate),
						new XElement("fullName", fullName),
						new XElement("fullKanaName", fullKanaName),
						new XElement("zipCode", zipCode),
						new XElement("address", address),
						new XElement("companyName", companyName),
						new XElement("departmentName", departmentName),
						new XElement("tel1", tel1),
						new XElement("tel2", tel2),
						new XElement("email1", email1),
						new XElement("email2", email2),
						new XElement("billedAmount", billedAmount),
						new XElement("gmoExtend1", gmoExtend1),
						new XElement("sex", sex),
						new XElement("birthday", birthday),
						new XElement("memberId", memberId),
						new XElement("transactionType", transactionType)
					),
					new XElement("deliveries",
						new XElement("delivery",
							new XElement("deliveryCustomer",
								new XElement("fullName", deliveryFullName),
								new XElement("fullKanaName", deliveryFullKanaName),
								new XElement("zipCode", deliveryZipCode),
								new XElement("address", deliveryAddress),
								new XElement("companyName", deliveryCompanyName),
								new XElement("departmentName", deliveryDepartmentName),
								new XElement("tel", deliveryTel)
							),
							new XElement("details",
								new XElement("detail",
									new XElement("detailName", detailName),
									new XElement("detailPrice", detailPrice),
									new XElement("detailQuantity", detailQuantity),
									new XElement("gmoExtend2", gmoExtend2),
									new XElement("gmoExtend3", gmoExtend3),
									new XElement("gmoExtend4", gmoExtend4),
									new XElement("detailBrand", detailBrand),
									new XElement("detailCategory", detailCategory)
								)
							)
						)
					)
				));

			return document;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentGmoAtokaraCancelResponseData ResponseData { get { return (PaymentGmoAtokaraCancelResponseData)this.ResponseDataInner; } }
	}
}
