/*
=========================================================================================================
  Module      : ソフトバンクペイメント クレジット「決済要求（トークン利用）」APIクラス(PaymentSBPSCreditAuthWithTokenApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ソフトバンクペイメント クレジット「決済要求（トークン利用）」APIクラス
	/// </summary>
	public class PaymentSBPSCreditAuthWithTokenApi : PaymentSBPSBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentSBPSCreditAuthWithTokenApi()
			: this(PaymentSBPSSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">SBPS設定</param>
		public PaymentSBPSCreditAuthWithTokenApi(
			PaymentSBPSSetting settings)
			: base(settings, new PaymentSBPSCreditAuthWithTokenResponseData(settings))
		{
		}

		/// <summary>
		/// 決済要求（トークンで決済する場合）
		/// </summary>
		/// <param name="custCode">顧客ID（決済情報保管時の紐付けキー）</param>
		/// <param name="sbpsOrderId">注文ID</param>
		/// <param name="itemId">商品検索用ID</param>
		/// <param name="itemName">商品表示名</param>
		/// <param name="productItems">商品情報</param>
		/// <param name="amount">注文金額合計</param>
		/// <param name="creditTokenInfo">トークン情報</param>
		/// <param name="divideInfo">分割情報</param>
		/// <param name="saveCardInfo">カード情報保存指定</param>
		/// <returns>実行結果</returns>
		public bool Exec(
			string custCode,
			string sbpsOrderId,
			string itemId,
			string itemName,
			List<ProductItem> productItems,
			decimal amount,
			CartPayment.CreditTokenInfoBase creditTokenInfo,
			PaymentSBPSCreditDivideInfo divideInfo,
			bool saveCardInfo)
		{
			// XML作成
			XDocument requestXml = CreateAuthXml(
				custCode,
				sbpsOrderId,
				itemId,
				itemName,
				productItems,
				amount,
				(CartPayment.CreditTokenInfoSbps)creditTokenInfo,
				divideInfo,
				saveCardInfo);

			// 実行
			return Post(requestXml);
		}

		/// <summary>
		/// 決済要求XML作成
		/// </summary>
		/// <param name="custCode">顧客ID（決済情報保管時の紐付けキー）</param>
		/// <param name="sbpsOrderId">注文ID</param>
		/// <param name="itemId">商品検索用ID</param>
		/// <param name="itemName">商品表示名</param>
		/// <param name="productItems">商品情報</param>
		/// <param name="amount">注文金額合計</param>
		/// <param name="creditTokenInfo">トークン情報</param>
		/// <param name="divideInfo">分割情報</param>
		/// <param name="saveCardInfo">カード情報保存指定</param>
		/// <returns>リアル与信XML</returns>
		private XDocument CreateAuthXml(
			string custCode,
			string sbpsOrderId,
			string itemId,
			string itemName,
			List<ProductItem> productItems,
			decimal amount,
			CartPayment.CreditTokenInfoSbps creditTokenInfo,
			PaymentSBPSCreditDivideInfo divideInfo,
			bool saveCardInfo)
		{
			// XML作成
			XDocument document = new XDocument(new XDeclaration("1.0", "Shift_JIS", ""));
			document.Add(
				new XElement("sps-api-request", new XAttribute("id", "ST01-00131-101"),
					new XElement("merchant_id", this.HashCalculator.Add(this.Settings.MerchantId)),
					new XElement("service_id", this.HashCalculator.Add(this.Settings.ServiceId)),
					new XElement("cust_code", this.HashCalculator.Add(custCode)),
					new XElement("order_id", this.HashCalculator.Add(sbpsOrderId)),
					new XElement("item_id", this.HashCalculator.Add(itemId)),	// ここで設定することによってソフトバンクペイメントで検索が可能
					new XElement("item_name", EncodeMultibyteToBase64(this.HashCalculator.Add(itemName))),
				//new XElement("Tax", this.HashCalculator.Add("")),
					new XElement("amount", this.HashCalculator.Add(amount.ToPriceString())),
				//new XElement("free1", this.HashCalculator.Add("")),
				//new XElement("free2", this.HashCalculator.Add("")),
				//new XElement("free3", this.HashCalculator.Add("")),
				//new XElement("order_rowno", this.HashCalculator.Add("")),	// 請求番号枝番。再入力時にインクリメントされて送られてくる。2 重購入要求を検知するために設定。
				//new XElement("sps_cust_info_return_flg", this.HashCalculator.Add("0")),	// 省略時はSPS顧客情報を返却しない。
					CreateProductItemNode(productItems),
					CreatePayMethodInfo(divideInfo),
					CreatePayOptionManage(creditTokenInfo.Token, creditTokenInfo.TokenKey, saveCardInfo),
					new XElement("encrypted_flg", this.HashCalculator.Add("1")),
					new XElement("request_date", this.HashCalculator.Add(DateTime.Now.ToString("yyyyMMddHHmmss"))),
				//new XElement("limit_second", this.HashCalculator.Add("")),	// リクエスト時の許容時間（省略の場合は事前設定値を適用）
					new XElement("sps_hashcode", this.HashCalculator.ComputeHashSHA1AndClearBuffer())	// チェックサム
			));
			return document;
		}

		/// <summary>
		/// リアル与信API「pay_method_info」エレメント作成
		/// </summary>
		/// <param name="divideInfo">分割情報</param>
		/// <returns>pay_method_info</returns>
		private XElement CreatePayMethodInfo(
			PaymentSBPSCreditDivideInfo divideInfo)
		{
			return new XElement("pay_method_info",
				CreateDivideTypeAndTimes(divideInfo));
		}

		/// <summary>
		/// リアル与信API「pay_method_info」エレメント作成
		/// </summary>
		/// <param name="token">トークン</param>
		/// <param name="tokenKey">トークンキー</param>
		/// <param name="saveCardInfo">カード情報保存指定</param>
		/// <returns>pay_method_info</returns>
		private XElement CreatePayOptionManage(
			string token,
			string tokenKey,
			bool saveCardInfo)
		{
			var custManageFlg = saveCardInfo ? "1" : "0";

			return new XElement("pay_option_manage",
				new XElement("token", this.HashCalculator.Add(token)),
				new XElement("token_key", this.HashCalculator.Add(tokenKey)),
				new XElement("cust_manage_flg", this.HashCalculator.Add(custManageFlg)));
		}

		/// <summary>
		/// 取引区分・分割回数エレメント取得
		/// </summary>
		/// <param name="divideInfo">分割情報</param>
		/// <returns>分割回数エレメント</returns>
		private IEnumerable<XElement> CreateDivideTypeAndTimes(PaymentSBPSCreditDivideInfo divideInfo)
		{
			// パラメタ変換
			string dealingsType = divideInfo.GetDealingsTypeString();
			if (dealingsType == null) return null;

			// エレメント作成
			var result = new List<XElement>();
			result.Add(new XElement("dealings_type", GetEncryptedData(this.HashCalculator.Add(dealingsType))));
			if ((divideInfo.DivideType == PaymentSBPSCreditDivideInfo.DivideTypes.Divide) && divideInfo.DivideTimes.HasValue)
			{
				result.Add(new XElement("divide_times", GetEncryptedData(this.HashCalculator.Add(divideInfo.DivideTimes.Value.ToString()))));
			}
			return result;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentSBPSCreditAuthWithTokenResponseData ResponseData { get { return (PaymentSBPSCreditAuthWithTokenResponseData)this.ResponseDataInner; } }
	}
}
