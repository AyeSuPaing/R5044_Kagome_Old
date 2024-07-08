/*
=========================================================================================================
  Module      :領収書ダウンロードページ(ReceiptDownload.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.IO;
using System.Net.Mime;
using w2.App.Common.Pdf.PdfCreater;
using w2.App.Common.SendMail;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User.Helper;
public partial class Form_Receipt_ReceiptDownload : BasePage
{
	# region ラップ済みコントロール宣言
	protected WrappedHtmlGenericControl WdvReceiptError { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvReceiptError"); } }
	protected WrappedLiteral WlReceiptErrorMessage { get { return GetWrappedControl<WrappedLiteral>("lReceiptErrorMessage"); } }
	protected WrappedLinkButton WlbReceiptDownload { get { return GetWrappedControl<WrappedLinkButton>("lbReceiptDownload"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// HTTPS通信チェック（HTTPの場合、HTTPSで再読込）
		CheckHttps(this.SecurePageProtocolAndHost + Request.Url.PathAndQuery);
		if (!IsPostBack)
		{
			this.OrderId = string.Empty;
			try
			{
				// 復号化して注文ID取得
				this.OrderId = UserPassowordCryptor.PasswordDecrypt(this.EncryptedOrderId);

				// ダウンロード可能のチェック
				CheckDownloadError();
			}
			catch (Exception)
			{
				this.ErrorMessages = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECEIPT_NO_RECEIPTINFO);
			}

			// メッセージ表示
			DisplayMessages();
		}
	}

	/// <summary>
	/// ダウンロード可能のチェック
	/// </summary>
	private void CheckDownloadError()
	{
		// 注文情報取得
		var order = new OrderService().Get(this.OrderId);

		// 領収書情報が取得できない場合はエラー
		if ((order == null) || (order.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_OFF))
		{
			this.ErrorMessages = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECEIPT_NO_RECEIPTINFO);
		}
		// 注文キャンセル済の場合はエラー
		else if (order.IsCanceled)
		{
			this.ErrorMessages = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECEIPT_ORDER_CANCELED);
		}
		// 領収書出力済みの場合はエラー
		else if (order.ReceiptOutputFlg == Constants.FLG_ORDER_RECEIPT_OUTPUT_FLG_ON)
		{
			this.ErrorMessages = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECEIPT_SETTLED_RECEIPTDOWNLOAD);
		}
	}

	/// <summary>
	/// メッセージ表示
	/// </summary>
	private void DisplayMessages()
	{
		if (this.WlbReceiptDownload.HasInnerControl)
		{
			this.WlbReceiptDownload.InnerControl.Visible = string.IsNullOrEmpty(this.ErrorMessages);
		}
		this.WdvReceiptError.Visible = (string.IsNullOrEmpty(this.ErrorMessages) == false);
		this.WlReceiptErrorMessage.Text = WebSanitizer.HtmlEncode(this.ErrorMessages);
	}

	/// <summary>
	/// 領収書ダウンロードボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReceiptDownload_Click(object sender, EventArgs e)
	{
		// ダウンロード可能のチェック
		CheckDownloadError();
		if (string.IsNullOrEmpty(this.ErrorMessages) == false)
		{
			DisplayMessages();
			return;
		}

		// 領収書出力
		try
		{
			this.Response.Clear();

			// PDF作成
			var order = new OrderService().Get(this.OrderId);
			var param = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, this.OrderId },
				{ Constants.FIELD_ORDERITEM_DATE_CREATED, order.DateCreated },
				{ Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, order.OrderPriceTotal },
			};
			new ReceiptCreater().Create(this.Response.OutputStream, Constants.KBN_PDF_OUTPUT_ORDER, param);
			SendReceipMail(param);
			UpdateReceiptOutputFlg(this.OrderId);

			// 出力ファイル名：Receipt_<ordered_date>_<order_id>_<order_price_total>.pdf
			var contentDisposition = new ContentDisposition
			{
				Inline = false,
				FileName = string.Format(
					"Receipt_{0}_{1}_{2}.pdf",
					order.DateCreated.ToString("yyyyMMddHHmmss"),
					(string)param[Constants.FIELD_ORDER_ORDER_ID],
					decimal.ToInt32((decimal)param[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL]))
			};

			// PDF出力
			this.Response.ContentType = "application/pdf";
			this.Response.AppendHeader("Content-Disposition", contentDisposition.ToString());

			this.Response.Flush();
			this.Response.End();
		}
		catch (ApplicationException ex)
		{
			// システムエラーがある場合はログを出力
			if (ex.InnerException != null)
			{
				AppLogger.WriteError(ex);
				this.ErrorMessages = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECEIPT_FAILURE_RECEIPTDOWNLOAD);
				DisplayMessages();
			}
		}
	}

	/// <summary>
	/// 領収書発行メール送信
	/// </summary>
	/// <param name="param">検索パラメタ</param>
	private void SendReceipMail(Hashtable param)
	{
		var filePath = new ReceiptCreater().CreateMailFile(Constants.KBN_PDF_OUTPUT_ORDER, param, this.OrderId);
		SendMailCommon.SendReceipFiletMail(this.OrderId, filePath);
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
	}

	/// <summary>
	/// 領収書ダウンロード済みフラグ更新
	/// </summary>
	/// <param name="orderId">注文ID</param>
	private void UpdateReceiptOutputFlg(string orderId)
	{
		new OrderService().Modify(
			orderId,
			order =>
			{
				order.ReceiptOutputFlg = Constants.FLG_ORDER_RECEIPT_OUTPUT_FLG_ON;
				order.LastChanged = Constants.FLG_LASTCHANGED_USER;
				order.DateChanged = DateTime.Now;
			},
			UpdateHistoryAction.Insert);
	}

	/// <summary>符号された注文ID</summary>
	private string EncryptedOrderId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ID_FOR_RECEIPT]); }
	}
	/// <summary>注文ID</summary>
	private string OrderId
	{
		get { return (string)this.ViewState["orderId"]; }
		set { this.ViewState["orderId"] = value; }
	}
	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessages { get; set; }
}
