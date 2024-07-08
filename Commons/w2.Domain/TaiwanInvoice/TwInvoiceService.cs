/*
=========================================================================================================
  Module      : 電子発票情報サービス (TwInvoiceService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.TwInvoice
{
	/// <summary>
	/// 電子発票情報サービス
	/// </summary>
	public class TwInvoiceService : ServiceBase, ITwInvoiceService
	{
		/// <summary>ロックオブジェクト</summary>
		private readonly object m_lockObject = new object();

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(TwInvoiceModel model)
		{
			using (var repository = new TwInvoiceRepository())
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +GetInvoiceNoForOrder
		/// <summary>
		/// Get Invoice For Order
		/// </summary>
		/// <param name="currentDate">Current date</param>
		/// <returns>Invoice for order</returns>
		public string GetInvoiceNoForOrder(string currentDate)
		{
			var invoiceNo = string.Empty;
			lock (m_lockObject)
			{
				using (var repository = new TwInvoiceRepository())
				{
					var invoice = repository.GetInvoiceForOrder(currentDate);
					if (invoice != null)
					{
						invoice.TwInvoiceNo = invoice.TwInvoiceNo.HasValue
							? (invoice.TwInvoiceNo + 1)
							: (invoice.TwInvoiceNoStart);
						invoiceNo = string.Format("{0}{1,0:00000000}", invoice.TwInvoiceCodeName, invoice.TwInvoiceNo);

						repository.Update(invoice);
					}
				}
			}

			return invoiceNo;
		}
		#endregion

		#region +InsertAfterGetApi
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertAfterGetApi(TwInvoiceModel model)
		{
			using (var repository = new TwInvoiceRepository())
			{
				repository.InsertAfterGetApi(model);
			}
		}
		#endregion

		#region +GetAll
		/// <summary>
		/// Get All
		/// </summary>
		/// <returns>モデル</returns>
		public TwInvoiceModel[] GetAll()
		{
			using (var repository = new TwInvoiceRepository())
			{
				var model = repository.GetAll();

				return model;
			}
		}
		#endregion

		#region +GetListValidInvoice
		/// <summary>
		/// Get List Valid Invoice
		/// </summary>
		/// <param name="currentDate">Current date</param>
		/// <returns>Invoice for order</returns>
		public TwInvoiceModel[] GetListValidInvoice(string currentDate)
		{
			using (var repository = new TwInvoiceRepository())
			{
				return repository.GetListValidInvoice(currentDate);
			}
		}
		#endregion

		#region +UpdateInvoiceAlertCountForValidInvoice
		/// <summary>
		/// Update Invoice Alert Count For Valid Invoice
		/// </summary>
		/// <param name="invoiceAlertCount">Invoice Alert Count</param>
		/// <returns>Is Update</returns>
		public int UpdateInvoiceAlertCountForValidInvoice(int invoiceAlertCount)
		{
			using (var repository = new TwInvoiceRepository())
			{
				return repository.UpdateInvoiceAlertCountForValidInvoice(invoiceAlertCount);
			}
		}
		#endregion

		#region +GetCurrentAlertCountInvoice
		/// <summary>
		/// Get Current Alert Count Invoice
		/// </summary>
		/// <param name="currentDate">Current date</param>
		/// <returns>Current Alert Count Invoice</returns>
		public TwInvoiceModel GetCurrentAlertCountInvoice(string currentDate)
		{
			using (var repository = new TwInvoiceRepository())
			{
				return repository.GetCurrentAlertCountInvoice(currentDate);
			}
		}
		#endregion

		#region +Get List TwInvoice
		/// <summary>
		/// Get List TwInvoice
		/// </summary>
		/// <param name="currentDate">Current Date</param>
		/// <returns>モデル</returns>
		public TwInvoiceModel[] GetListTwInvoice(string currentDate)
		{
			using (var repository = new TwInvoiceRepository())
			{
				var models = repository.GetListTwInvoice(currentDate);

				return models;
			}
		}
		#endregion

		#region +Get Invoice New
		/// <summary>
		/// Get Invoice New
		/// </summary>
		/// <returns>Top 1 TwInvoice</returns>
		public TwInvoiceModel GetInvoiceNew()
		{
			using (var repository = new TwInvoiceRepository())
			{
				var model = repository.GetInvoiceNew();

				return model;
			}
		}
		#endregion

		#region +来期電子発票の存在チェック
		/// <summary>
		/// 来期電子発票の存在チェック
		/// </summary>
		/// <param name="twInvoiceDateCreate">電子発票作成日</param>
		/// <param name="twInvoiceDateStart">発番開始日</param>
		/// <returns>true:存在する、false:存在しない</returns>
		public bool CheckNextPeriodInvoiceExists(string twInvoiceDateCreate, string twInvoiceDateStart)
		{
			using (var repository = new TwInvoiceRepository())
			{
				return repository.CheckNextPeriodInvoiceExists(twInvoiceDateCreate, twInvoiceDateStart);
			}
		}
		#endregion
	}
}
