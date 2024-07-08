/*
=========================================================================================================
  Module      : Program (Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Api;
using w2.App.Common.Order;
using w2.App.Common.Util;
using w2.Commerce.Batch.TwInvoice.Util;
using w2.Common.Logger;
using w2.Domain.TwInvoice;

namespace w2.Commerce.Batch.TwInvoice
{
	class Program
	{
		/// <summary>
		/// Main Thread
		/// </summary>
		/// <param name="args">Execute parameter</param>
		[STAThread]
		private static void Main(string[] args)
		{
			try
			{
				// Read Common Setting
				Initialize.ReadCommonConfig();

				FileLogger.WriteInfo("起動");

				// Excute Single Instance
				var executeSuccess = ProcessUtility.ExcecWithProcessMutex(() => MainExecute());
				if (executeSuccess == false)
				{
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}

				FileLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				// Write Log
				FileLogger.WriteError(ex.ToString());
			}
		}

		/// <summary>
		/// Main Execute
		/// </summary>
		private static void MainExecute()
		{
			if (Constants.TWINVOICE_ECPAY_ENABLED) return;

			var mail = new MailSendUtil();
			// Check condition
			if (CheckConditionGet())
			{
				var twInvoiceResponse = new TwInvoiceApi().GetInvoice(Constants.TWINVOICE_GET_COUNT.ToString());

				mail.SendMail((twInvoiceResponse.IsSuccess == false),
					twInvoiceResponse.ReturnCode,
					twInvoiceResponse.Trackinfo,
					twInvoiceResponse.ReturnMessage);
			}

			// Check quantity exits
			var quantityExits = OrderCommon.CheckQuantityExistence();
			var twInvoiceNew = new TwInvoiceService().GetInvoiceNew();

			while ((twInvoiceNew == null) || (quantityExits < twInvoiceNew.TwInvoiceAlertCount))
			{
				var twInvoiceResponse = new TwInvoiceApi().GetInvoice(Constants.TWINVOICE_GET_COUNT.ToString(), true);

				// Send mail alert
				if (twInvoiceResponse.IsSuccess == false)
				{
					mail.SendMailAlert(quantityExits);
					break;
				}
				quantityExits = OrderCommon.CheckQuantityExistence();
				twInvoiceNew = new TwInvoiceService().GetInvoiceNew();
			}
		}

		/// <summary>
		/// Check Condition Get
		/// </summary>
		/// <returns>true if qualified, else return false</returns>
		private static bool CheckConditionGet()
		{
			// Check month
			var dateNow = DateTime.Now;
			if (dateNow.Month % 2 != 0) return false;

			// Check Day
			var settingDay = Constants.TWINVOICE_GET_DATE;
			if (dateNow.Day < settingDay) return false;

			// 来期電子発票の存在チェック
			var twInvoiceDateCreate = new DateTime(dateNow.Year, dateNow.Month, 1);
			var twInvoiceDateStart = GetInvoiceDateStart(dateNow);

			var nextPeriodInvoiceExists = new TwInvoiceService().CheckNextPeriodInvoiceExists(
				twInvoiceDateCreate.ToString(),
				twInvoiceDateStart.ToString());

			if (nextPeriodInvoiceExists) return false;

			return true;
		}

		/// <summary>
		/// 電子発票の開始日を取得
		/// </summary>
		/// <param name="dateNow">現在の日時</param>
		/// <returns>電子発票の開始日</returns>
		private static DateTime GetInvoiceDateStart(DateTime dateNow)
		{
			var year = ((dateNow.Month == Constants.TWINVOICE_MONTH_12
				? (dateNow.Year + 1)
				: dateNow.Year));

			var period = (((dateNow.Month / 2) == Constants.TWINVOICE_MAX_PERIOD)
				? Constants.TWINVOICE_PERIOD_DEFAULT
				: (dateNow.Month / 2));

			var month = GetMonthEnd(period) - 1;
			var ressult = new DateTime(year, month, 1);

			return ressult;
		}

		/// <summary>
		/// 台湾電子発票期別の最後月を取得
		/// </summary>
		/// <param name="period">期別</param>
		/// <returns>期別の最後月</returns>
		private static int GetMonthEnd(int period)
		{
			switch (period)
			{
				case 1:
					return Constants.TWINVOICE_MONTH_4;

				case 2:
					return Constants.TWINVOICE_MONTH_6;

				case 3:
					return Constants.TWINVOICE_MONTH_8;

				case 4:
					return Constants.TWINVOICE_MONTH_10;

				case 5:
					return Constants.TWINVOICE_MONTH_12;

				default:
					return Constants.TWINVOICE_MONTH_2;
			}
		}
	}
}