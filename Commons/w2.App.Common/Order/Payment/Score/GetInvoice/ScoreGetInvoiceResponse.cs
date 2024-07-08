/*
=========================================================================================================
  Module      : スコア後払い払込票印字データ取得のレスポンス値(ScoreGetInvoiceResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Score.ObjectsElement;
using w2.Common.Util;
using w2.Domain.Score;

namespace w2.App.Common.Order.Payment.Score.GetInvoice
{
	/// <summary>
	/// スコア後払い払込票印字データ取得のレスポンス値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class ScoreGetInvoiceResponse : BaseScoreResponse
	{
		/// <summary>
		/// 請求書モデル生成
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデル</returns>
		public InvoiceScoreModel CreateModel(string orderId)
		{
			var model = new InvoiceScoreModel
			{
				OrderId = orderId,
				InvoiceBarCode = this.InvoiceDataResult.InvoiceBarCode,
				InvoiceCode = this.InvoiceDataResult.InvoiceCode,
				InvoiceKbn = this.InvoiceDataResult.InvoiceKbn,
				HistorySeq = this.InvoiceDataResult.HistorySeq,
				RemindedKbn = this.InvoiceDataResult.RemindedKbn,
				CompanyName = this.InvoiceDataResult.CompanyName,
				Department = this.InvoiceDataResult.Department,
				CustomerName = this.InvoiceDataResult.CustomerName,
				CustomerZip = this.InvoiceDataResult.CustomerZip,
				CustomerAddress1 = this.InvoiceDataResult.CustomerAddress1,
				CustomerAddress2 = this.InvoiceDataResult.CustomerAddress2,
				CustomerAddress3 = this.InvoiceDataResult.CustomerAddress3,
				ShopZip = this.InvoiceDataResult.ShopZip,
				ShopAddress1 = this.InvoiceDataResult.ShopAddress1,
				ShopAddress2 = this.InvoiceDataResult.ShopAddress2,
				ShopAddress3 = this.InvoiceDataResult.ShopAddress3,
				ShopTel = this.InvoiceDataResult.ShopTel,
				ShopFax = this.InvoiceDataResult.ShopFax,
				BilledAmount = this.InvoiceDataResult.BilledAmount,
				Tax = this.InvoiceDataResult.Tax,
				TimeOfReceipts = this.InvoiceDataResult.TimeOfReceipts,
				InvoiceStartDate = this.InvoiceDataResult.InvoiceStartDate,
				InvoiceTitle = this.InvoiceDataResult.InvoiceTitle,
				Message1 = this.InvoiceDataResult.Message1,
				Message2 = this.InvoiceDataResult.Message2,
				Message3 = this.InvoiceDataResult.Message3,
				Message4 = this.InvoiceDataResult.Message4,
				InvoiceShopsiteName = this.InvoiceDataResult.InvoiceShopsiteName,
				ShopEmail = this.InvoiceDataResult.ShopEmail,
				Name = this.InvoiceDataResult.NissenName,
				QaUrl = this.InvoiceDataResult.NissenQaUrl,
				ShopOrderDate = this.InvoiceDataResult.ShopOrderDate,
				ShopCode = this.InvoiceDataResult.ShopCode,
				TransactionId = this.InvoiceDataResult.NissenTransactionId,
				ShopTransactionId1 = this.InvoiceDataResult.ShopTransactionId1,
				ShopTransactionId2 = this.InvoiceDataResult.ShopTransactionId2,
				ShopMessage1 = this.InvoiceDataResult.ShopMessage1,
				ShopMessage2 = this.InvoiceDataResult.ShopMessage2,
				ShopMessage3 = this.InvoiceDataResult.ShopMessage3,
				ShopMessage4 = this.InvoiceDataResult.ShopMessage4,
				ShopMessage5 = this.InvoiceDataResult.ShopMessage5,
				InvoiceForm = StringUtility.ToEmpty(this.InvoiceDataResult.InvoiceForm),
				PostalAccountNumber = StringUtility.ToEmpty(this.InvoiceDataResult.PostalAccountNumber),
				PostalAccountHolderName = StringUtility.ToEmpty(this.InvoiceDataResult.PostalAccountHolderName),
				PostalFontBottomRow = StringUtility.ToEmpty(this.InvoiceDataResult.PostalFontBottomRow),
				PostalFontTopRow = StringUtility.ToEmpty(this.InvoiceDataResult.PostalFontTopRow),
				PremittanceAddress = StringUtility.ToEmpty(this.InvoiceDataResult.PremittanceAddress),
				XSymbol = StringUtility.ToEmpty(this.InvoiceDataResult.XSymbol),
				Details = CreateDetailModels(orderId)
			};
			return model;
		}

		/// <summary>
		/// 請求書明細モデル生成
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデル</returns>
		public InvoiceScoreDetailModel[] CreateDetailModels(string orderId)
		{
			var models = this.DetailList.GoodsDetail
				.Select(
					(detail, index) =>
					{
						var model = new InvoiceScoreDetailModel
						{
							OrderId = orderId,
							DetailNo = (index + 1),
							GoodsName = detail.GoodsName,
							GoodsPrice = detail.GoodsPrice,
							GoodsNum = detail.GoodsNum,
						};
						return model;
					})
				.ToArray();

			return models;
		}

		/// <summary>印字データ</summary>
		[XmlElement("invoiceDataResult")]
		public InvoiceDataResultElement InvoiceDataResult { get; set; }
		/// <summary>印字詳細項目データ</summary>
		[XmlElement("detailList")]
		public DetailListElement DetailList { get; set; }
	}
}
