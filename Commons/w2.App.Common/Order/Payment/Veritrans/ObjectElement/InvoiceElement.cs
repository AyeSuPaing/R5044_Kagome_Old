/*
=========================================================================================================
  Module      : 請求書情報要素 (InvoiceElement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using jp.veritrans.tercerog.mdk.dto;
using System;
using System.Linq;
using w2.Common.Util;
using w2.Domain.InvoiceVeritrans;

namespace w2.App.Common.Order.Payment.Veritrans.ObjectElement
{
	public class InvoiceElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseDto">レスポンスデータ</param>
		public InvoiceElement(ScoreatpayGetInvoiceDataResponseDto responseDto)
		{
			this.ServiceType = StringUtility.ToEmpty(responseDto.ServiceType);
			this.MStatus = StringUtility.ToEmpty(responseDto.Mstatus);
			this.VResultCode = StringUtility.ToEmpty(responseDto.VResultCode);
			this.MErrMsg = StringUtility.ToEmpty(responseDto.MerrMsg);
			this.MArchTxn = StringUtility.ToEmpty(responseDto.MarchTxn);
			this.OrderId = StringUtility.ToEmpty(responseDto.OrderId);
			this.CustTxn = StringUtility.ToEmpty(responseDto.CustTxn);
			this.TxnVersion = StringUtility.ToEmpty(responseDto.TxnVersion);
			this.InvoiceBarCode = StringUtility.ToEmpty(responseDto.InvoiceBarCode);
			this.InvoiceCode = StringUtility.ToEmpty(responseDto.InvoiceCode);
			this.InvoiceKbn = StringUtility.ToEmpty(responseDto.InvoiceKbn);
			this.HistorySeq = StringUtility.ToEmpty(responseDto.HistorySeq);
			this.RemindedKbn = StringUtility.ToEmpty(responseDto.RemindedKbn);
			this.CompanyName = StringUtility.ToEmpty(responseDto.CompanyName);
			this.Department = StringUtility.ToEmpty(responseDto.Department);
			this.CustomerName = StringUtility.ToEmpty(responseDto.CustomerName);
			this.CustomerZip = StringUtility.ToEmpty(responseDto.CustomerZip);
			this.CustomerAddress1 = StringUtility.ToEmpty(responseDto.CustomerAddress1);
			this.CustomerAddress2 = StringUtility.ToEmpty(responseDto.CustomerAddress2);
			this.CustomerAddress3 = StringUtility.ToEmpty(responseDto.CustomerAddress3);
			this.ShopZip = StringUtility.ToEmpty(responseDto.ShopZip);
			this.ShopAddress1 = StringUtility.ToEmpty(responseDto.ShopAddress1);
			this.ShopAddress2 = StringUtility.ToEmpty(responseDto.ShopAddress2);
			this.ShopAddress3 = StringUtility.ToEmpty(responseDto.ShopAddress3);
			this.ShopTel = StringUtility.ToEmpty(responseDto.ShopTel);
			this.ShopFax = StringUtility.ToEmpty(responseDto.ShopFax);
			this.BilledAmount = StringUtility.ToEmpty(responseDto.BilledAmount);
			this.Tax = StringUtility.ToEmpty(responseDto.Tax);
			this.TimeOfReceipts = StringUtility.ToEmpty(responseDto.TimeOfReceipts);
			this.InvoiceStartDate = StringUtility.ToEmpty(responseDto.InvoiceStartDate);
			this.InvoiceTitle = StringUtility.ToEmpty(responseDto.InvoiceTitle);
			this.NissenMessage1 = StringUtility.ToEmpty(responseDto.NissenMessage1);
			this.NissenMessage2 = StringUtility.ToEmpty(responseDto.NissenMessage2);
			this.NissenMessage3 = StringUtility.ToEmpty(responseDto.NissenMessage3);
			this.NissenMessage4 = StringUtility.ToEmpty(responseDto.NissenMessage4);
			this.InvoiceShopsiteName = StringUtility.ToEmpty(responseDto.InvoiceShopsiteName);
			this.ShopEmail = StringUtility.ToEmpty(responseDto.ShopEmail);
			this.NissenName = StringUtility.ToEmpty(responseDto.NissenName);
			this.NissenQaUrl = StringUtility.ToEmpty(responseDto.NissenQaUrl);
			this.ShopOrderDate = StringUtility.ToEmpty(responseDto.ShopOrderDate);
			this.ShopCode = StringUtility.ToEmpty(responseDto.ShopCode);
			this.NissenTransactionId = StringUtility.ToEmpty(responseDto.NissenTransactionId);
			this.ShopTransactionId1 = StringUtility.ToEmpty(responseDto.ShopTransactionId1);
			this.ShopTransactionId2 = StringUtility.ToEmpty(responseDto.ShopTransactionId2);
			this.ShopMessage1 = StringUtility.ToEmpty(responseDto.ShopMessage1);
			this.ShopMessage2 = StringUtility.ToEmpty(responseDto.ShopMessage2);
			this.ShopMessage3 = StringUtility.ToEmpty(responseDto.ShopMessage3);
			this.ShopMessage4 = StringUtility.ToEmpty(responseDto.ShopMessage4);
			this.ShopMessage5 = StringUtility.ToEmpty(responseDto.ShopMessage5);
			this.Details = responseDto.Details;
			this.Errors = responseDto.Errors;
			this.InvoiceForm = StringUtility.ToEmpty(responseDto.Yobi1);
			this.PostalAccountNumber = StringUtility.ToEmpty(responseDto.Yobi2);
			this.PostalAccountHolderName = StringUtility.ToEmpty(responseDto.Yobi3);
			this.PostalFontTopRow = StringUtility.ToEmpty(responseDto.Yobi4);
			this.PostalFontBottomRow = StringUtility.ToEmpty(responseDto.Yobi5);
			this.RemittanceAddress = StringUtility.ToEmpty(responseDto.Yobi6);
			this.XSymbol = StringUtility.ToEmpty(responseDto.Yobi7);
		}

		/// <summary>
		/// 請求書モデル作成
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>請求書モデル</returns>
		public InvoiceVeritransModel CreateModel(string orderId, string lastChanged)
		{
			var model = new InvoiceVeritransModel
			{
				OrderId = orderId,
				ServiceType = this.ServiceType,
				MStatus = this.MStatus,
				VResulCcode = this.VResultCode,
				MErrMsg = this.MErrMsg,
				MArchTxn = this.MArchTxn,
				PaymentOrderId = this.OrderId,
				CustTxn = this.CustTxn,
				TxnVersion = this.TxnVersion,
				InvoiceBarCode = this.InvoiceBarCode,
				InvoiceCode = this.InvoiceCode,
				InvoiceKbn = this.InvoiceKbn,
				HistorySeq = this.HistorySeq,
				RemindedKbn = this.RemindedKbn,
				CompanyName = this.CompanyName,
				Department = this.Department,
				CustomerName = this.CustomerName,
				CustomerZip = this.CustomerZip,
				CustomerAddress1 = this.CustomerAddress1,
				CustomerAddress2 = this.CustomerAddress2,
				CustomerAddress3 = this.CustomerAddress3,
				ShopZip = this.ShopZip,
				ShopAddress1 = this.ShopAddress1,
				ShopAddress2 = this.ShopAddress2,
				ShopAddress3 = this.ShopAddress3,
				ShopTel = this.ShopTel,
				ShopFax = this.ShopFax,
				BilledAmount = this.BilledAmount,
				Tax = this.Tax,
				TimeOfReceipts = string.IsNullOrEmpty(this.TimeOfReceipts) == false
					? (DateTime?)DateTime.Parse(this.TimeOfReceipts)
					: null,
				InvoiceStartDate = string.IsNullOrEmpty(this.InvoiceStartDate) == false
					? (DateTime?)DateTime.Parse(this.InvoiceStartDate)
					: null,
				InvoiceTitle = this.InvoiceTitle,
				NissenMessage1 = this.NissenMessage1,
				NissenMessage2 = this.NissenMessage2,
				NissenMessage3 = this.NissenMessage3,
				NissenMessage4 = this.NissenMessage4,
				InvoiceShopsiteName = this.InvoiceShopsiteName,
				ShopEmail = this.ShopEmail,
				NissenName = this.NissenName,
				NissenQaUrl = this.NissenQaUrl,
				ShopOrderDate = string.IsNullOrEmpty(this.ShopOrderDate) == false
					? (DateTime?)DateTime.Parse(this.ShopOrderDate)
					: null,
				ShopCode = this.ShopCode,
				NissenTransactionId = this.NissenTransactionId,
				ShopTransactionId1 = this.ShopTransactionId1,
				ShopTransactionId2 = this.ShopTransactionId2,
				ShopMessage1 = this.ShopMessage1,
				ShopMessage2 = this.ShopMessage2,
				ShopMessage3 = this.ShopMessage3,
				ShopMessage4 = this.ShopMessage4,
				ShopMessage5 = this.ShopMessage5,
				InvoiceForm = this.InvoiceForm,
				PostalAccountNumber = this.PostalAccountNumber,
				PostalAccountHolderName = this.PostalAccountHolderName,
				PostalFontTopRow = this.PostalFontTopRow,
				PostalFontBottomRow = this.PostalFontBottomRow,
				RemittanceAddress = this.RemittanceAddress,
				XSymbol = this.XSymbol,
				LastChanged = lastChanged,
				Details = CreateDetailModels(orderId, this.Details),
			};
			return model;
		}

		/// <summary>
		/// 請求書明細モデル作成
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="detailDto">明細情報オブジェクト</param>
		/// <returns>請求書明細モデル</returns>
		private InvoiceVeritransDetailModel[] CreateDetailModels(string orderId, ScoreatpayDetailDto[] detailDto)
		{
			var models = detailDto.Select(
				(detail, index) =>
				new InvoiceVeritransDetailModel
				{
					OrderId = orderId,
					DetailNo = (index + 1),
					GoodsName = detail.DetailName,
					GoodsPrice = detail.DetailPrice,
					GoodsNum = detail.DetailQuantity,
				}).ToArray();
			return models;
		}

		/// <summary>決済サービスタイプ</summary>
		private string ServiceType { get; }
		/// <summary>処理結果コード</summary>
		private string MStatus { get; }
		/// <summary>詳細結果コード</summary>
		private string VResultCode { get; }
		/// <summary>エラーメッセージ</summary>
		private string MErrMsg { get; }
		/// <summary>電文ID</summary>
		private string MArchTxn { get; }
		/// <summary>取引ID</summary>
		private string OrderId { get; }
		/// <summary>取引毎に付くID</summary>
		private string CustTxn { get; }
		/// <summary>MDKバージョン</summary>
		private string TxnVersion { get; }
		/// <summary>請求書バーコード</summary>
		private string InvoiceBarCode { get; }
		/// <summary>請求書コード</summary>
		private string InvoiceCode { get; }
		/// <summary>発行区分</summary>
		private string InvoiceKbn { get; }
		/// <summary>履歴番号</summary>
		private string HistorySeq { get; }
		/// <summary>督促区分</summary>
		private string RemindedKbn { get; }
		/// <summary>会社名</summary>
		private string CompanyName { get; }
		/// <summary>部署名</summary>
		private string Department { get; }
		/// <summary>購入者氏名</summary>
		private string CustomerName { get; }
		/// <summary>購入者郵便番号</summary>
		private string CustomerZip { get; }
		/// <summary>購入者住所：都道府県</summary>
		private string CustomerAddress1 { get; }
		/// <summary>購入者住所：市区町村</summary>
		private string CustomerAddress2 { get; }
		/// <summary>購入者住所：それ以降の住所</summary>
		private string CustomerAddress3 { get; }
		/// <summary>加盟店郵便番号</summary>
		private string ShopZip { get; }
		/// <summary>加盟店住所：都道府県</summary>
		private string ShopAddress1 { get; }
		/// <summary>加盟店住所：市区町村</summary>
		private string ShopAddress2 { get; }
		/// <summary>加盟店住所：それ以降の住所</summary>
		private string ShopAddress3 { get; }
		/// <summary>加盟店電話</summary>
		private string ShopTel { get; }
		/// <summary>加盟店FAX番号</summary>
		private string ShopFax { get; }
		/// <summary>顧客請求金額</summary>
		private string BilledAmount { get; }
		/// <summary>消費税</summary>
		private string Tax { get; }
		/// <summary>購入者払込期限日</summary>
		private string TimeOfReceipts { get; }
		/// <summary>請求書発行日付</summary>
		private string InvoiceStartDate { get; }
		/// <summary>帳票タイトル</summary>
		private string InvoiceTitle { get; }
		/// <summary>スコア通信欄1</summary>
		private string NissenMessage1 { get; }
		/// <summary>スコア通信欄2</summary>
		private string NissenMessage2 { get; }
		/// <summary>スコア通信欄3</summary>
		private string NissenMessage3 { get; }
		/// <summary>スコア通信欄4</summary>
		private string NissenMessage4 { get; }
		/// <summary>加盟店サイト名称</summary>
		private string InvoiceShopsiteName { get; }
		/// <summary>加盟店メールアドレス</summary>
		private string ShopEmail { get; }
		/// <summary>スコア社名</summary>
		private string NissenName { get; }
		/// <summary>スコア連絡先URL</summary>
		private string NissenQaUrl { get; }
		/// <summary>加盟店注文日</summary>
		private string ShopOrderDate { get; }
		/// <summary>加盟店コード</summary>
		private string ShopCode { get; }
		/// <summary>スコア注文ID</summary>
		private string NissenTransactionId { get; }
		/// <summary>加盟店注文ID1</summary>
		private string ShopTransactionId1 { get; }
		/// <summary>加盟店注文ID2</summary>
		private string ShopTransactionId2 { get; }
		/// <summary>加盟店通信欄1</summary>
		private string ShopMessage1 { get; }
		/// <summary>加盟店通信欄2</summary>
		private string ShopMessage2 { get; }
		/// <summary>加盟店通信欄3</summary>
		private string ShopMessage3 { get; }
		/// <summary>加盟店通信欄4</summary>
		private string ShopMessage4 { get; }
		/// <summary>加盟店通信欄5</summary>
		private string ShopMessage5 { get; }
		/// <summary>明細</summary>
		private ScoreatpayDetailDto[] Details { get; }
		/// <summary>エラー詳細</summary>
		private ScoreatpayErrorDto[] Errors { get; }
		/// <summary>請求書形式</summary>
		private string InvoiceForm { get; }
		/// <summary>郵便口座番号</summary>
		private string PostalAccountNumber { get; }
		/// <summary>郵便口座名義</summary>
		private string PostalAccountHolderName { get; }
		/// <summary>郵便 OCR-B フォント：上段</summary>
		private string PostalFontTopRow { get; }
		/// <summary>郵便 OCR-B フォント：下段</summary>
		private string PostalFontBottomRow { get; }
		/// <summary>払込取扱用 購入者住所</summary>
		private string RemittanceAddress { get; }
		/// <summary>X印</summary>
		private string XSymbol { get; }
	}
}
