/*
=========================================================================================================
  Module      : DSK後払い請求書印字データ取得レスポンス(DskDeferredGetInvoiceResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Xml.Serialization;
using w2.Common.Util;
using w2.Domain.InvoiceDskDeferred;
using w2.Domain.InvoiceDskDeferredDetail;

namespace w2.App.Common.Order.Payment.DSKDeferred.GetInvoice
{
	/// <summary>
	/// DSK後払い請求書印字データ取得レスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false)]
	[XmlType(TypeName = "sqPrtDtGetResForm")]
	public class DskDeferredGetInvoiceResponse : BaseDskDeferredResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DskDeferredGetInvoiceResponse()
		{
			this.InvoiceDataResult = new InvoiceDataResultElement();
		}

		/// <summary>
		/// 請求書モデル生成
		/// </summary>
		/// <returns>モデル</returns>
		public InvoiceDskDeferredModel CreateModel()
		{
			var model = new InvoiceDskDeferredModel
			{
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
				// レスポンスの日付に空白が入るため除去
				TimeOfReceipts = this.InvoiceDataResult.TimeOfReceipts.Replace(" ", ""),
				// レスポンスの日付に空白が入るため除去
				InvoiceStartDate = this.InvoiceDataResult.InvoiceStartDate.Replace(" ", ""),
				InvoiceTitle = this.InvoiceDataResult.InvoiceTitle,
				Message1 = this.InvoiceDataResult.Message1,
				Message2 = this.InvoiceDataResult.Message2,
				Message3 = this.InvoiceDataResult.Message3,
				Message4 = this.InvoiceDataResult.Message4,
				InvoiceShopsiteName = this.InvoiceDataResult.InvoiceShopsiteName,
				ShopEmail = this.InvoiceDataResult.ShopEmail,
				Name = this.InvoiceDataResult.Name,
				QaUrl = this.InvoiceDataResult.QaUrl,
				// レスポンスの日付に空白が入るため除去
				ShopOrderDate = this.InvoiceDataResult.ShopOrderDate.Replace(" ", ""),
				ShopCode = this.InvoiceDataResult.ShopCode,
				TransactionId = this.InvoiceDataResult.TransactionId,
				ShopTransactionId1 = this.InvoiceDataResult.ShopTransactionId1,
				ShopTransactionId2 = this.InvoiceDataResult.ShopTransactionId2,
				ShopMessage1 = this.InvoiceDataResult.ShopMessage1,
				ShopMessage2 = this.InvoiceDataResult.ShopMessage2,
				ShopMessage3 = this.InvoiceDataResult.ShopMessage3,
				ShopMessage4 = this.InvoiceDataResult.ShopMessage4,
				ShopMessage5 = this.InvoiceDataResult.ShopMessage5,
				Yobi1 = StringUtility.ToEmpty(this.InvoiceDataResult.Yobi1),
				Yobi2 = StringUtility.ToEmpty(this.InvoiceDataResult.Yobi2),
				Yobi3 = StringUtility.ToEmpty(this.InvoiceDataResult.Yobi3),
				Yobi4 = StringUtility.ToEmpty(this.InvoiceDataResult.Yobi4),
				Yobi5 = StringUtility.ToEmpty(this.InvoiceDataResult.Yobi5),
				Yobi6 = StringUtility.ToEmpty(this.InvoiceDataResult.Yobi6),
				Yobi7 = StringUtility.ToEmpty(this.InvoiceDataResult.Yobi7),
				Details = CreateDeteailsModel()
			};
			return model;
		}

		/// <summary>
		/// 請求書明細モデル生成
		/// </summary>
		/// <returns>モデル</returns>
		public InvoiceDskDeferredDetailModel[] CreateDeteailsModel()
		{
			var models = this.DetailList.GoodsDetail
				.Select(
					(detail, i) =>
					{
						var model = new InvoiceDskDeferredDetailModel
						{
							GoodsName = detail.GoodsName,
							GoodsPrice = detail.GoodsPrice,
							GoodsNum = detail.GoodsNum,
						};
						model.DetailNo = (i + 1);
						return model;
					})
				.ToArray();

			return models;
		}

		/// <summary>印字データ</summary>
		[XmlElement("invoiceDataResult")]
		public InvoiceDataResultElement InvoiceDataResult;
		/// <summary>明細項目</summary>
		[XmlElement("detailList")]
		public DetailListElement DetailList;
	}

	/// <summary>
	/// 印字データ要素
	/// </summary>
	public class InvoiceDataResultElement
	{
		/// <summary>請求書バーコード</summary>
		[XmlElement("invoiceBarCode")]
		public string InvoiceBarCode;
		/// <summary>請求書コード</summary>
		[XmlElement("invoiceCode")]
		public string InvoiceCode;
		/// <summary>発行区分</summary>
		[XmlElement("invoiceKbn")]
		public string InvoiceKbn;
		/// <summary>履歴番号</summary>
		[XmlElement("historySeq")]
		public string HistorySeq;
		/// <summary>督促区分</summary>
		[XmlElement("remindedKbn")]
		public string RemindedKbn;
		/// <summary>会社名</summary>
		[XmlElement("companyName")]
		public string CompanyName;
		/// <summary>部署名</summary>
		[XmlElement("department")]
		public string Department;
		/// <summary>購入者氏名</summary>
		[XmlElement("customerName")]
		public string CustomerName;
		/// <summary>購入者郵便番号</summary>
		[XmlElement("customerZip")]
		public string CustomerZip;
		/// <summary>購入者住所都道府県</summary>
		[XmlElement("customerAddress1")]
		public string CustomerAddress1;
		/// <summary>購入者住所市区町村</summary>
		[XmlElement("customerAddress2")]
		public string CustomerAddress2;
		/// <summary>購入者住所それ以降の住所</summary>
		[XmlElement("customerAddress3")]
		public string CustomerAddress3;
		/// <summary>加盟店郵便番号</summary>
		[XmlElement("shopZip")]
		public string ShopZip;
		/// <summary>加盟店住所都道府県</summary>
		[XmlElement("shopAddress1")]
		public string ShopAddress1;
		/// <summary>購入者住所市区町村</summary>
		[XmlElement("shopAddress2")]
		public string ShopAddress2;
		/// <summary>加盟店住所それ以降の住所</summary>
		[XmlElement("shopAddress3")]
		public string ShopAddress3;
		/// <summary>加盟店電話</summary>
		[XmlElement("shopTel")]
		public string ShopTel;
		/// <summary>加盟店FAX番号</summary>
		[XmlElement("shopFax")]
		public string ShopFax;
		/// <summary>顧客請求金額</summary>
		[XmlElement("billedAmount")]
		public string BilledAmount;
		/// <summary>消費税</summary>
		[XmlElement("tax")]
		public string Tax;
		/// <summary>購入者払込期限日</summary>
		[XmlElement("timeOfReceipts")]
		public string TimeOfReceipts;
		/// <summary>請求書発行日付</summary>
		[XmlElement("invoiceStartDate")]
		public string InvoiceStartDate;
		/// <summary>帳票タイトル</summary>
		[XmlElement("invoiceTitle")]
		public string InvoiceTitle;
		/// <summary>通信欄1</summary>
		[XmlElement("message1")]
		public string Message1;
		/// <summary>通信欄2</summary>
		[XmlElement("message2")]
		public string Message2;
		/// <summary>通信欄3</summary>
		[XmlElement("message3")]
		public string Message3;
		/// <summary>通信欄4</summary>
		[XmlElement("message4")]
		public string Message4;
		/// <summary>加盟店サイト名称</summary>
		[XmlElement("invoiceShopsiteName")]
		public string InvoiceShopsiteName;
		/// <summary>加盟店メールアドレス</summary>
		[XmlElement("shopEmail")]
		public string ShopEmail;
		/// <summary>文字列(固定文言)</summary>
		[XmlElement("name")]
		public string Name;
		/// <summary>文字列(固定文言)</summary>
		[XmlElement("qaUrl")]
		public string QaUrl;
		/// <summary>加盟店注文日</summary>
		[XmlElement("shopOrderDate")]
		public string ShopOrderDate;
		/// <summary>加盟店ID</summary>
		[XmlElement("shopCode")]
		public string ShopCode;
		/// <summary>注文ID</summary>
		[XmlElement("transactionId")]
		public string TransactionId;
		/// <summary>加盟店注文ID1</summary>
		[XmlElement("shopTransactionId1")]
		public string ShopTransactionId1;
		/// <summary>加盟店注文ID2</summary>
		[XmlElement("shopTransactionId2")]
		public string ShopTransactionId2;
		/// <summary>加盟店通信欄1</summary>
		[XmlElement("shopMessage1")]
		public string ShopMessage1;
		/// <summary>加盟店通信欄2</summary>
		[XmlElement("shopMessage2")]
		public string ShopMessage2;
		/// <summary>加盟店通信欄3</summary>
		[XmlElement("shopMessage3")]
		public string ShopMessage3;
		/// <summary>加盟店通信欄4</summary>
		[XmlElement("shopMessage4")]
		public string ShopMessage4;
		/// <summary>加盟店通信欄5</summary>
		[XmlElement("shopMessage5")]
		public string ShopMessage5;
		/// <summary>請求書形式</summary>
		[XmlElement("yobi1")]
		public string Yobi1;
		/// <summary>郵便口座番号</summary>
		[XmlElement("yobi2")]
		public string Yobi2;
		/// <summary>郵便口座名義 </summary>
		[XmlElement("yobi3")]
		public string Yobi3;
		/// <summary>郵便OCR-Bフォント上段</summary>
		[XmlElement("yobi4")]
		public string Yobi4;
		/// <summary>郵便OCR-Bフォント下段</summary>
		[XmlElement("yobi5")]
		public string Yobi5;
		/// <summary>払込取扱用購入者住所</summary>
		[XmlElement("yobi6")]
		public string Yobi6;
		/// <summary>X印</summary>
		[XmlElement("yobi7")]
		public string Yobi7;
	}

	/// <summary>
	/// 明細項目要素
	/// </summary>
	public class DetailListElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DetailListElement()
		{
			this.GoodsDetail = new[] { new GoodsDetailElement() };
		}

		/// <summary>明細詳細情報</summary>
		[XmlElement("goodsDetail")]
		public GoodsDetailElement[] GoodsDetail;
	}

	/// <summary>
	/// 明細詳細情報要素
	/// </summary>
	public class GoodsDetailElement
	{
		/// <summary>明細名</summary>
		[XmlElement("goodsName")]
		public string GoodsName;
		/// <summary>単価</summary>
		[XmlElement("goodsPrice")]
		public string GoodsPrice;
		/// <summary>数量</summary>
		[XmlElement("goodsNum")]
		public string GoodsNum;
	}
}
