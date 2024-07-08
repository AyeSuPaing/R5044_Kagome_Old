/*
=========================================================================================================
  Module      : 電子発票情報リポジトリ (TwInvoiceRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.TwInvoice
{
	/// <summary>
	/// 電子発票情報リポジトリ
	/// </summary>
	internal class TwInvoiceRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "TwInvoice";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TwInvoiceRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">Accessor</param>
		public TwInvoiceRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(TwInvoiceModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(TwInvoiceModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);

			return result;
		}
		#endregion

		#region +GetInvoiceForOrder
		/// <summary>
		/// Get Invoice ForOrder
		/// </summary>
		/// <param name="currentDate">Current Date</param>
		/// <returns>Invoice</returns>
		public TwInvoiceModel GetInvoiceForOrder(string currentDate)
		{
			var input = new Hashtable()
			{
				{"current_date", currentDate}
			};

			var data = Get(XML_KEY_NAME, "GetInvoiceForOrder", input);
			if (data.Count == 0) return null;

			return new TwInvoiceModel(data[0]);
		}
		#endregion

		#region + GetAll
		/// <summary>
		/// Get All
		/// </summary>
		/// <returns>All TwInvoice</returns>
		public TwInvoiceModel[] GetAll()
		{
			var data = Get(XML_KEY_NAME, "GetAll");

			return (data.Count == 0)
				? null
				: data.Cast<DataRowView>().Select(row => new TwInvoiceModel(row)).ToArray();
		}
		#endregion

		#region +InsertAfterGetApi
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertAfterGetApi(TwInvoiceModel model)
		{
			Exec(XML_KEY_NAME, "InsertAfterGetApi", model.DataSource);
		}
		#endregion

		#region + GetListValidInvoice
		/// <summary>
		/// Get List Valid Invoice
		/// </summary>
		/// <param name="currentDate">Current Date</param>
		/// <returns>Invoices</returns>
		public TwInvoiceModel[] GetListValidInvoice(string currentDate)
		{
			var input = new Hashtable()
			{
				{"current_date", currentDate}
			};

			var data = Get(XML_KEY_NAME, "GetInvoiceForOrder", input);

			return ((data.Count == 0)
				? null
				: data.Cast<DataRowView>().Select(row => new TwInvoiceModel(row)).ToArray());
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
			var input = new Hashtable()
			{
				{Constants.FIELD_TWINVOICE_TW_INVOICE_ALERT_COUNT, invoiceAlertCount}
			};

			return Exec(XML_KEY_NAME, "UpdateInvoiceAlertCountForValidInvoice", input);
		}
		#endregion

		#region +GetCurrentAlertCountInvoice
		/// <summary>
		/// GetCurrentAlertCountInvoice
		/// </summary>
		/// <param name="currentDate">Current Date</param>
		/// <returns>Invoices</returns>
		public TwInvoiceModel GetCurrentAlertCountInvoice(string currentDate)
		{
			var input = new Hashtable()
			{
				{"current_date", currentDate}
			};

			var data = Get(XML_KEY_NAME, "GetCurrentAlertCountInvoice", input);

			return (data.Count == 0)
				? null
				: new TwInvoiceModel(data[0]);
		}
		#endregion

		#region +GetListTwInvoice
		/// <summary>
		/// Get List TwInvoice
		/// </summary>
		/// <param name="currentDate">Current Date</param>
		/// <returns>Array Model</returns>
		public TwInvoiceModel[] GetListTwInvoice(string currentDate)
		{
			var input = new Hashtable
			{
				{"current_date", currentDate},
			};
			var data = Get(XML_KEY_NAME, "GetInvoiceForOrder", input);
			if (data.Count == 0) return null;

			return data.Cast<DataRowView>().Select(drv => new TwInvoiceModel(drv)).ToArray();
		}
		#endregion

		#region +Get Invoice New
		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>モデル</returns>
		public TwInvoiceModel GetInvoiceNew()
		{
			var dv = Get(XML_KEY_NAME, "GetInvoiceNew");

			return (dv.Count == 0)
				? null
				: new TwInvoiceModel(dv[0]);
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
			var input = new Hashtable
			{
				{Constants.FIELD_TWINVOICE_DATE_CREATED, twInvoiceDateCreate},
				{Constants.FIELD_TWINVOICE_TW_INVOICE_DATE_START, twInvoiceDateStart},
			};
			var dv = Get(XML_KEY_NAME, "CheckNextPeriodInvoiceExists", input);

			return (dv.Count != 0);
		}
		#endregion
	}
}
