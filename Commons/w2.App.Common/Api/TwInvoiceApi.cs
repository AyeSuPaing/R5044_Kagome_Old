/*
=========================================================================================================
  Module      : Shop Order Api (ShopOrderApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using w2.Common.Logger;
using w2.Common.Net.Mail;
using w2.Common.Util;
using w2.Domain.TwInvoice;
using w2.Domain.TwOrderInvoice;

namespace w2.App.Common.Api
{
	/// <summary>
	/// Shop Order Api
	/// </summary>
	public class TwInvoiceApi
	{
		#region Constant
		/// <summary>Month 12</summary>
		protected const int MONTH_12 = 12;
		/// <summary>Month 10</summary>
		protected const int MONTH_10 = 10;
		/// <summary>Month 8</summary>
		protected const int MONTH_8 = 8;
		/// <summary>Month 6</summary>
		protected const int MONTH_6 = 6;
		/// <summary>Month 4</summary>
		protected const int MONTH_4 = 4;
		/// <summary>Month 2</summary>
		protected const int MONTH_2 = 2;
		/// <summary>Max Period</summary>
		protected const int MAX_PERIOD = 6;
		/// <summary>Size Default</summary>
		protected const string SIZE_DEFAULT = "-1";
		/// <summary>Period Default</summary>
		protected const int PERIOD_DEFAULT = 0;
		/// <summary>Url Get Track List</summary>
		protected const string URL_GET_TRACK_LIST = "get_track_list.php";
		/// <summary>Url Released</summary>
		protected const string URL_RELEASED = "c0401.php";
		/// <summary>Url Return</summary>
		protected const string URL_RETURN = "d0401.php";
		/// <summary>Header Released</summary>
		protected List<string> m_headerReleased = new List<string>()
		{
			"InvoiceNumber",
			"InvoiceDate",
			"InvoiceTime",
			"BuyerIdentifier",
			"BuyerName",
			"BuyerAddress",
			"BuyerTelephoneNumber",
			"BuyerEmailAddress",
			"SalesAmount",
			"FreeTaxSalesAmount",
			"ZeroTaxSalesAmount",
			"TaxType",
			"TaxRate",
			"TaxAmount",
			"TotalAmount",
			"PrintMark",
			"RandomNumber",
			"MainRemark",
			"CarrierType",
			"CarrierId1",
			"CarrierId2",
			"NPOBAN",
			"Description",
			"Quantity",
			"UnitPrice",
			"Amount",
			"Remark"
		};
		/// <summary>Header Return</summary>
		protected List<string> m_headerReturn = new List<string>()
		{
			"AllowanceNumber",
			"AllowanceDate",
			"AllowanceType",
			"TaxAmount",
			"TotalAmount",
			"OriginalInvoiceDate",
			"OriginalInvoiceNumber",
			"OriginalDescription",
			"Quantity",
			"UnitPrice",
			"Amount",
			"Tax",
			"TaxType"
		};
		/// <summary>Url get issued invoice pdf</summary>
		public const string URL_GET_ISSUED_INVOICE = "get_invoice_pdf.php";
		/// <summary>Url get refunded invoice pdf</summary>
		public const string URL_GET_REFUNDED_INVOICE = "get_allowance_pdf.php";
		#endregion

		#region Method
		/// <summary>
		/// Connect Shop Order Api
		/// </summary>
		/// <param name="size">Size</param>
		/// <param name="isGetFromInvoiceManagement">Is Get From Invoice Management</param>
		/// <returns>Response Model</returns>
		public TwInvoiceResponse GetInvoice(string size, bool isGetFromInvoiceManagement = false)
		{
			var month = DateTime.Now.Month;
			var year = 0;
			var period = 0;

			if (isGetFromInvoiceManagement)
			{
				year = DateTime.Now.Year;

				period = ((month % 2 != 0)
					? (month / 2)
					: (month / 2 - 1));
			}
			else
			{
				year = ((month == MONTH_12
					? (DateTime.Now.Year + 1)
					: DateTime.Now.Year));

				period = (((month / 2) == MAX_PERIOD)
					? PERIOD_DEFAULT
					: (month / 2));
			}

			var listParameter = new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>("id", Constants.TWINVOICE_UNIFORM_ID),
				new KeyValuePair<string, string>("user", Constants.TWINVOICE_UNIFORM_ACCOUNT),
				new KeyValuePair<string, string>("passwd", Constants.TWINVOICE_UNIFORM_PASSWORD),
				new KeyValuePair<string, string>("year", year.ToString()),
				new KeyValuePair<string, string>("period", period.ToString()),
				new KeyValuePair<string, string>("size", string.IsNullOrEmpty(size) ? SIZE_DEFAULT : size)
			};

			// Create resquest parameters
			var requestString = CreateParametersAsString(listParameter);

			// Write file log
			FileLogger.WriteInfo("API GET INVOICE");
			FileLogger.WriteInfo("Request Call Api: " + requestString);

			// Post and get data
			var response = PostAndReceiveDataFormApi(URL_GET_TRACK_LIST, Encoding.UTF8.GetBytes(requestString));

			var responseFromServer = ConvertModelFromResponeString(response);

			if (responseFromServer.IsSuccess)
			{
				var models = CreateModelFromResponse(responseFromServer, year, period);

				foreach (var model in models)
				{
					new TwInvoiceService().InsertAfterGetApi(model);
				}
			}

			return responseFromServer;
		}

		/// <summary>
		/// Create Model From Response
		/// </summary>
		/// <param name="response">Response</param>
		/// <param name="year">Year</param>
		/// <param name="period">Period</param>
		/// <returns>List Model</returns>
		private List<TwInvoiceModel> CreateModelFromResponse(TwInvoiceResponse response, int year, int period)
		{
			var result = new List<TwInvoiceModel>();
			var model = new TwInvoiceModel();
			var regex = new Regex(@"^[a-zA-Z]*\z");
			var invoices = response.Trackinfo.Split(',');
			var currentInvoice = new TwInvoiceService().GetCurrentAlertCountInvoice(DateTime.Now.ToString());
			for (var index = 0; index < invoices.Length; index++)
			{
				if (regex.IsMatch(invoices[index]))
				{
					model.TwInvoiceCodeName = invoices[index];

					var start = 0m;
					var end = 0m;
					var invoiceNoStart = invoices[index + 1];
					var invoiceNoEnd = invoices[index + 2];

					model.TwInvoiceNoStart = (decimal.TryParse(invoiceNoStart, out start)
						? start
						: 0);
					model.TwInvoiceNoEnd = (decimal.TryParse(invoiceNoEnd, out end)
						? end
						: 0);
					model.TwInvoiceTypeName = "一般税額計算";
					model.TwInvoiceCode = "7";
					model.TwInvoiceDateEnd = GetInvoiceDateEnd(year, period);
					model.TwInvoiceDateStart = new DateTime(model.TwInvoiceDateEnd.Year, model.TwInvoiceDateEnd.Month - 1, 1);
					model.TwInvoiceId = string.Format("{0}{1:00}{2}{3}", year, model.TwInvoiceDateStart.Month, model.TwInvoiceCodeName, invoiceNoStart);
					model.TwInvoiceAlertCount = (currentInvoice == null) ? 100 : currentInvoice.TwInvoiceAlertCount;

					result.Add(model);
				}
			}

			return result;
		}

		/// <summary>
		/// Get Invoice Date End
		/// </summary>
		/// <param name="year">Year</param>
		/// <param name="period">Period</param>
		/// <returns>Invoice Date End</returns>
		public static DateTime GetInvoiceDateEnd(int year, int period)
		{
			var month = GetMonthEnd(period);
			var day = DateTime.DaysInMonth(year, month);
			var ressult = new DateTime(year, month, day);

			return ressult;
		}

		/// <summary>
		/// Get Month End
		/// </summary>
		/// <param name="period">Period</param>
		/// <returns>Month</returns>
		private static int GetMonthEnd(int period)
		{
			switch (period)
			{
				case 1:
					return MONTH_4;

				case 2:
					return MONTH_6;

				case 3:
					return MONTH_8;

				case 4:
					return MONTH_10;

				case 5:
					return MONTH_12;

				default:
					return MONTH_2;
			}
		}

		/// <summary>
		/// 電子発票発行
		/// </summary>
		/// <param name="orderInvoice">注文電子発票情報</param>
		/// <param name="orderInfo">注文情報</param>
		/// <returns>レスポンス情報</returns>
		public TwInvoiceResponse InvoiceRelease(TwOrderInvoiceModel orderInvoice, DataView orderInfo)
		{
			var values = new List<string>();
			var line = 0;
			foreach (DataRowView order in orderInfo)
			{
				line++;
				values.Add(StringUtility.CreateEscapedCsvString(
					CreateContentInvoiceRelease(
						orderInvoice,
						order,
						(line == 1))));
			}

			var content = CreateCsvContent(
				StringUtility.CreateEscapedCsvString(m_headerReleased),
				values);
			var response = CallApiInvoiceReleaseOrReturn(content, false);
			return response;
		}

		/// <summary>
		/// 電子発票払い戻し
		/// </summary>
		/// <param name="orderInvoice">注文電子発票情報</param>
		/// <param name="orderInfo">注文情報</param>
		/// <param name="isOrderReturnExchange">返品交換か</param>
		/// <returns>Response Model</returns>
		public TwInvoiceResponse InvoiceReturn(
			TwOrderInvoiceModel orderInvoice,
			DataView orderInfo,
			bool isOrderReturnExchange)
		{
			var values = new List<string>();
			var line = 0;
			foreach (DataRowView order in orderInfo)
			{
				line++;
				values.Add(StringUtility.CreateEscapedCsvString(
					CreateContentInvoiceReturn(
						orderInvoice,
						order,
						isOrderReturnExchange,
						(line == 1))));
			}

			var response = InvoiceReturn(values);
			return response;
		}
		/// <summary>
		/// 電子発票払い戻し
		/// </summary>
		/// <param name="orderInvoice">注文電子発票情報</param>
		/// <param name="orderInfo">注文情報</param>
		/// <param name="isOrderReturnExchange">返品交換か</param>
		/// <returns>レスポンス情報</returns>
		public TwInvoiceResponse InvoiceReturn(
			TwOrderInvoiceModel orderInvoice,
			DataRowView orderInfo,
			bool isOrderReturnExchange)
		{　
			var value = StringUtility.CreateEscapedCsvString(
				CreateContentInvoiceReturn(
					orderInvoice,
					orderInfo,
					isOrderReturnExchange,
					true));
			var response = InvoiceReturn(new List<string>{ value });
			return response;
		}
		/// <summary>
		/// 電子発票払い戻し
		/// </summary>
		/// <param name="values">CSV出力値</param>
		/// <returns>レスポンス情報</returns>
		public TwInvoiceResponse InvoiceReturn(List<string> values)
		{
			var content = CreateCsvContent(
				StringUtility.CreateEscapedCsvString(m_headerReturn),
				values);
			var response = CallApiInvoiceReleaseOrReturn(content, true);
			return response;
		}

		/// <summary>
		/// CSV内容出力
		/// </summary>
		/// <param name="header">ヘッダー</param>
		/// <param name="values">値</param>
		/// <returns>CSV内容</returns>
		private static string CreateCsvContent(string header, List<string> values)
		{
			var content = new List<string>{ header };
			content.AddRange(values);
			content.Add("Finish");
			return string.Join("\r\n", content);
		}

		/// <summary>
		/// Call Api Invoice Release Or Return
		/// </summary>
		/// <param name="csv">Csv</param>
		/// <param name="isReturn">isReturn</param>
		/// <returns>Response Model</returns>
		private static TwInvoiceResponse CallApiInvoiceReleaseOrReturn(string csv, bool isReturn)
		{
			var values = new NameValueCollection()
			{
				{ "id", Constants.TWINVOICE_UNIFORM_ID },
				{ "user", Constants.TWINVOICE_UNIFORM_ACCOUNT },
				{ "passwd", Constants.TWINVOICE_UNIFORM_PASSWORD },
				{ "csv", Convert.ToBase64String(Encoding.UTF8.GetBytes(csv)) }
			};

			var messageLog = new StringBuilder();
			messageLog.AppendLine(isReturn ? "API RETURN" : "API RELEASE")
				.AppendLine("CSV: " + csv);

			// Post and get data
			var url = isReturn ? URL_RETURN : URL_RELEASED;
			var responseString = string.Empty;
			using (var client = new WebClient())
			{
				var response = client.UploadValues(Constants.TWINVOICE_URL + url, values);

				responseString = Encoding.Default.GetString(response);
			}

			var result = ConvertModelFromResponeString(responseString);

			messageLog.AppendLine(string.Format("Call Api: {0}", result.IsSuccess
				? "Success"
				: "Error: " + result.ReturnMessage));

			FileLogger.Write("InvoiceApi", messageLog.ToString());

			return result;
		}

		/// <summary>
		/// Convert Model From Respone String
		/// </summary>
		/// <param name="response">Respone</param>
		/// <param name="funcCallback">Func callback</param>
		/// <returns>Model</returns>
		private static TwInvoiceResponse ConvertModelFromResponeString(string response, Func<string, string> funcCallback = null)
		{
			var data = new Hashtable();
			foreach (var item in response.Split('&'))
			{
				var isMessage = item.Contains("rtmessage");
				if (isMessage)
				{
					var result = item.Split(new string[] { "rtmessage=" }, StringSplitOptions.RemoveEmptyEntries);
					data["rtmessage"] = (funcCallback == null)
						? result[0]
						: funcCallback(result[0]);
				}
				else
				{
					var result = item.Split('=');
					data[result[0]] = ((result.Length > 1)
						? result[1]
						: string.Empty);
				}
			}

			var responseFromServer = new TwInvoiceResponse(data);

			return responseFromServer;
		}

		/// <summary>
		/// 電子発票発行CSV内容作成
		/// </summary>
		/// <param name="orderInvoice">注文電子発票情報</param>
		/// <param name="data">注文情報</param>
		/// <param name="isFirstLine">１行目か</param>
		/// <returns>CSV内容</returns>
		private List<string> CreateContentInvoiceRelease(
			TwOrderInvoiceModel orderInvoice,
			DataRowView data,
			bool isFirstLine)
		{
			var cells = new List<string>();

			// Invoice Number
			cells.Add(orderInvoice.TwInvoiceNo);

			// Invoice Date
			var invoiceDate = (isFirstLine && orderInvoice.TwInvoiceDate.HasValue)
				? orderInvoice.TwInvoiceDate.Value.ToString(Constants.DATE_FORMAT_SHORT)
				: string.Empty;
			cells.Add(invoiceDate);

			// Invoice Time
			var invoiceTime = (isFirstLine && orderInvoice.TwInvoiceDate.HasValue)
				? orderInvoice.TwInvoiceDate.Value.ToString("hh:mm:ss")
				: string.Empty;
			cells.Add(invoiceTime);

			// Buyer Identifier
			var buyerIdentifier =
				(isFirstLine && (orderInvoice.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY))
					? orderInvoice.TwUniformInvoiceOption1
					: string.Empty;
			cells.Add(buyerIdentifier);

			// Buyer Name
			cells.Add(isFirstLine ? GetDataRowViewData(data, Constants.FIELD_ORDEROWNER_OWNER_NAME) : string.Empty);

			// Buyer Address
			var address = string.Format("{0}{1}{2}{3}{4}",
				GetDataRowViewData(data, Constants.FIELD_ORDEROWNER_OWNER_ADDR1),
				GetDataRowViewData(data, Constants.FIELD_ORDEROWNER_OWNER_ADDR2),
				GetDataRowViewData(data, Constants.FIELD_ORDEROWNER_OWNER_ADDR3),
				GetDataRowViewData(data, Constants.FIELD_ORDEROWNER_OWNER_ADDR4),
				GetDataRowViewData(data, Constants.FIELD_ORDEROWNER_OWNER_ADDR5));
			cells.Add(isFirstLine ? ((address.Length > 100) ? address.Substring(0, 100) : address) : string.Empty);

			// Buyer Telephone Number
			var tel = GetDataRowViewData(data, Constants.FIELD_ORDEROWNER_OWNER_TEL1);
			cells.Add(isFirstLine ? ((tel.Length > 26) ? tel.Substring(0, 26) : tel) : string.Empty);

			// Buyer Email Address
			var email = GetDataRowViewData(data, Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR);
			cells.Add(isFirstLine ? ((email.Length > 80) ? email.Substring(0, 80) : email) : string.Empty);

			// Sales Amount
			decimal priceTotal;
			var orderPriceTax = 0m;
			var salesAmount = 0m;
			if (decimal.TryParse(GetDataRowViewData(data, Constants.FIELD_ORDER_ORDER_PRICE_TOTAL), out priceTotal)
				&& decimal.TryParse(GetDataRowViewData(data, Constants.FIELD_ORDER_ORDER_PRICE_TAX), out orderPriceTax))
			{
				salesAmount = priceTotal - orderPriceTax;
			}
			cells.Add(isFirstLine ? salesAmount.ToString("0") : string.Empty);

			// Free Tax Sales Amount
			cells.Add(isFirstLine ? "0" : string.Empty);

			// Zero Tax Sales Amount
			cells.Add(isFirstLine ? "0" : string.Empty);

			// Tax Type
			cells.Add(isFirstLine ? "1" : string.Empty);

			// Tax Rate
			var taxrate = 0m;
			if (decimal.TryParse(GetDataRowViewData(data, Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE), out taxrate))
			{
				taxrate = (taxrate / 100);
			}
			cells.Add(isFirstLine ? taxrate.ToString() : string.Empty);

			// Tax Amount
			cells.Add(isFirstLine ? orderPriceTax.ToString("0") : string.Empty);

			// Total Amount
			cells.Add(isFirstLine ? priceTotal.ToString("0") : string.Empty);

			// Print Mark
			cells.Add(isFirstLine ? "N" : string.Empty);

			// Random Number
			cells.Add(isFirstLine ? new Random().Next(0, 9999).ToString("0000") : string.Empty);

			// Main Remark
			cells.Add(string.Empty);

			// Carrier Type, CarrierId1, CarrierId2
			if (isFirstLine)
			{
				switch (orderInvoice.TwCarryType)
				{
					case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
						cells.Add("3J0002");
						cells.Add(orderInvoice.TwCarryTypeOption);
						cells.Add(orderInvoice.TwCarryTypeOption);
						break;

					case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
						cells.Add("CQ0001");
						cells.Add(orderInvoice.TwCarryTypeOption);
						cells.Add(orderInvoice.TwCarryTypeOption);
						break;

					default:
						cells.Add(string.Empty);
						cells.Add(string.Empty);
						cells.Add(string.Empty);
						break;
				}
			}
			else
			{
				cells.Add(string.Empty);
				cells.Add(string.Empty);
				cells.Add(string.Empty);
			}

			// NPOBAN
			cells.Add((isFirstLine && (orderInvoice.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_DONATE))
				? orderInvoice.TwUniformInvoiceOption1
				: string.Empty);

			// Description
			cells.Add(GetDataRowViewData(data, Constants.FIELD_ORDERITEM_PRODUCT_NAME));

			// Quantity
			cells.Add(GetDataRowViewData(data, Constants.FIELD_ORDERITEM_ITEM_QUANTITY));

			// UnitPrice
			decimal unitPrice;
			cells.Add(decimal.TryParse(GetDataRowViewData(data, Constants.FIELD_ORDERITEM_PRODUCT_PRICE), out unitPrice)
				? unitPrice.ToString("0")
				: string.Empty);

			// Amount
			decimal amount;
			cells.Add(decimal.TryParse(GetDataRowViewData(data, Constants.FIELD_ORDERITEM_ITEM_PRICE), out amount)
				? amount.ToString("0")
				: string.Empty);

			// Remark
			cells.Add(string.Empty);

			return cells;
		}

		/// <summary>
		/// 電子発票払い戻しCSV内容作成
		/// </summary>
		/// <param name="orderInvoice">注文電子発票情報</param>
		/// <param name="data">注文情報</param>
		/// <param name="isOrderReturnExchange">返品交換か</param>
		/// <param name="isFirstLine">１行目か</param>
		/// <returns>CSV内容</returns>
		private List<string> CreateContentInvoiceReturn(
			TwOrderInvoiceModel orderInvoice,
			DataRowView data,
			bool isOrderReturnExchange,
			bool isFirstLine)
		{
			var cells = new List<string>();

			// Allowance Number
			cells.Add(orderInvoice.TwInvoiceNo);

			// Allowance Date
			DateTime orderDate;
			var orderDateField = isOrderReturnExchange
				? Constants.FIELD_ORDER_ORDER_DATE
				: Constants.FIELD_ORDER_ORDER_CANCEL_DATE;
			if (isFirstLine && DateTime.TryParse(GetDataRowViewData(data, orderDateField), out orderDate))
			{
				cells.Add(orderDate.ToString(Constants.DATE_FORMAT_SHORT));
			}
			else
			{
				cells.Add(string.Empty);
			}

			// Allowance Type
			cells.Add(isFirstLine ? "2" : string.Empty);

			// TaxAmount
			decimal orderPriceTax;
			if (decimal.TryParse(GetDataRowViewData(data, Constants.FIELD_ORDER_ORDER_PRICE_TAX), out orderPriceTax)
				&& isOrderReturnExchange)
			{
				orderPriceTax *= -1;
			}
			cells.Add(isFirstLine ? orderPriceTax.ToString("0") : string.Empty);

			// Total Amount
			var priceTotal = 0m;
			if (decimal.TryParse(GetDataRowViewData(data, Constants.FIELD_ORDER_ORDER_PRICE_TOTAL), out priceTotal)
				&& isOrderReturnExchange)
			{
				priceTotal *= -1;
			}
			cells.Add(isFirstLine ? (priceTotal - orderPriceTax).ToString("0") : string.Empty);

			// Original Invoice Date, Original Invoice Numbers
			if (isOrderReturnExchange)
			{
				var orderInvoiceOld = new TwOrderInvoiceService().GetOrderInvoice(
					GetDataRowViewData(data, Constants.FIELD_ORDER_ORDER_ID_ORG),
					int.Parse(GetDataRowViewData(data, Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO)));
				if (orderInvoiceOld != null)
				{
					cells.Add(orderInvoiceOld.TwInvoiceDate.HasValue
						? orderInvoiceOld.TwInvoiceDate.Value.ToString(Constants.DATE_FORMAT_SHORT)
						: string.Empty);
					cells.Add(orderInvoiceOld.TwInvoiceNo);
				}
			}
			else
			{
				if ((GetDataRowViewData(data, Constants.FIELD_ORDER_ORDER_STATUS) == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
					&& DateTime.TryParse(GetDataRowViewData(data, Constants.FIELD_ORDER_ORDER_DATE), out orderDate))
				{
					cells.Add(orderDate.ToString(Constants.DATE_FORMAT_SHORT));
				}
				else
				{
					cells.Add(string.Empty);
				}

				cells.Add(orderInvoice.TwInvoiceNo.Split('-')[0]);
			}

			// Original Description
			cells.Add(GetDataRowViewData(data, Constants.FIELD_ORDERITEM_PRODUCT_NAME));

			// Quantity
			var quantity = decimal.Parse(GetDataRowViewData(data, Constants.FIELD_ORDERITEM_ITEM_QUANTITY));
			if (isOrderReturnExchange)
			{
				quantity *= -1;
			}
			cells.Add(quantity.ToString("0"));

			decimal productTaxRate;
			if (decimal.TryParse(
				GetDataRowViewData(data, Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE),
				out productTaxRate) == false)
			{
				productTaxRate = 0m;
			}

			// UnitPrice
			decimal productPrice;
			var unitPrice = 0m;
			if (decimal.TryParse(GetDataRowViewData(data, Constants.FIELD_ORDERITEM_PRODUCT_PRICE), out productPrice))
			{
				unitPrice = productPrice / (1 + (productTaxRate * 0.01m));
			}
			cells.Add(unitPrice.ToString("0"));
			
			// Amount
			decimal itemPrice;
			if (decimal.TryParse(GetDataRowViewData(data, Constants.FIELD_ORDERITEM_ITEM_PRICE), out itemPrice)
				&& isOrderReturnExchange)
			{
				itemPrice *= -1;
			}
			var amount = itemPrice / (1 + (productTaxRate * 0.01m));
			cells.Add(amount.ToString("0"));

			// Tax
			cells.Add(Math.Round(itemPrice - amount).ToString("0"));

			// TaxType
			cells.Add("1");

			return cells;
		}

		/// <summary>
		/// Get DataRowView data
		/// </summary>
		/// <param name="dataRowView">The DataRowView object</param>
		/// <param name="property">A string that contain the specified column</param>
		/// <param name="converter">The converer function callback</param>
		/// <param name="defaultValue">A default value when cannot get data in DataRowView</param>
		/// <returns>The value of specified column of DataRowView</returns>
		private string GetDataRowViewData(DataRowView dataRowView, string property, Func<string, string> converter = null, string defaultValue = "")
		{
			try
			{
				var data = StringUtility.ToEmpty(dataRowView[property]);

				return ((converter != null) ? converter(data) : data);
			}
			catch
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Create parameters as string
		/// </summary>
		/// <param name="parameters">List key value parameter</param>
		/// <returns>Parameters</returns>
		public static string CreateParametersAsString(List<KeyValuePair<string, string>> parameters)
		{
			return string.Join("&", parameters.Select(param => string.Format("{0}={1}", param.Key, param.Value)));
		}

		/// <summary>
		/// Post request and receive data
		/// </summary>
		/// <param name="byteData">Post data</param>
		/// <returns>Json string</returns>
		public static string PostAndReceiveDataFormApi(string url, byte[] byteData)
		{
			try
			{
				var webRequest = (HttpWebRequest)WebRequest.Create(Constants.TWINVOICE_URL + url);
				webRequest.Method = "POST";
				webRequest.ContentType = "application/x-www-form-urlencoded";
				webRequest.ContentLength = byteData.Length;

				// Write POST data
				var responseFromServer = string.Empty;
				using (var stream = webRequest.GetRequestStream())
				{
					stream.Write(byteData, 0, byteData.Length);

					// Get response
					using (var response = (HttpWebResponse)webRequest.GetResponse())
					{
						responseFromServer = WebUtility.UrlDecode(new StreamReader(response.GetResponseStream()).ReadToEnd());
					}
				}

				return responseFromServer;
			}
			catch (Exception exception)
			{
				FileLogger.WriteError(string.Format("{0} - {1}", Constants.TWINVOICE_URL, exception.Message));

				return string.Empty;
			}
		}

		/// <summary>
		/// Create Taiwan invoice response
		/// </summary>
		/// <param name="url">Url api</param>
		/// <param name="parameters">Parameters</param>
		/// <returns>TwInvoice response</returns>
		public static TwInvoiceResponse CreateTwInvoiceResponse(string url, List<KeyValuePair<string, string>> parameters)
		{
			var parametersString = CreateParametersAsString(parameters);
			var dataReceiveFromApi = PostAndReceiveDataFormApi(url, Encoding.UTF8.GetBytes(parametersString));
			var responseFromServer = ConvertModelFromResponeString(dataReceiveFromApi, ReplaceSpaceForDownloadInvoice);
			return responseFromServer;
		}

		/// <summary>
		///  Create parameters
		/// </summary>
		/// <param name="key">Key invoice</param>
		/// <param name="invoiceNo">Invoice no</param>
		/// <returns>Parameters</returns>
		public static List<KeyValuePair<string, string>> CreateParameters(string key, string invoiceNo)
		{
			var parameters = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("id", Constants.TWINVOICE_UNIFORM_ID),
				new KeyValuePair<string, string>("user", Constants.TWINVOICE_UNIFORM_ACCOUNT),
				new KeyValuePair<string, string>("passwd", Constants.TWINVOICE_UNIFORM_PASSWORD),
				new KeyValuePair<string, string>(key, invoiceNo),
				new KeyValuePair<string, string>("type", StringUtility.ToEmpty(Constants.TWINVOICE_PDF_FORMAT)),
			};
			return parameters;
		}

		/// <summary>
		/// Replace space for download invoice
		/// </summary>
		/// <param name="rtMessage">Return message</param>
		/// <returns>The message has been replaced</returns>
		private static string ReplaceSpaceForDownloadInvoice(string rtMessage)
		{
			return rtMessage.Replace(" ", "+");
		}
		#endregion
	}

	/// <summary>
	/// Tw Invoice Response
	/// </summary>
	public partial class TwInvoiceResponse : ModelBase<TwInvoiceResponse>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TwInvoiceResponse(Hashtable source)
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>Return Code</summary>
		public string ReturnCode
		{
			get { return (string)this.DataSource["rtcode"]; }
		}
		/// <summary>Return Message</summary>
		public string ReturnMessage
		{
			get
			{
				try
				{
					return EncodeHelper.DecodeBase64(Encoding.UTF8, StringUtility.ToEmpty(this.DataSource["rtmessage"]));
				}
				catch
				{
					return "Decode 64 Error: " + StringUtility.ToEmpty(this.DataSource["rtmessage"]);
				}
			}
		}
		/// <summary>Trackinfo</summary>
		public string Trackinfo
		{
			get { return (string)this.DataSource["trackinfo"]; }
		}
		/// <summary>Size</summary>
		public string Size
		{
			get { return (string)this.DataSource["size"]; }
		}
		/// <summary>Is Success</summary>
		public bool IsSuccess
		{
			get { return this.ReturnCode == "0000"; }
		}
		#endregion
	}
}
