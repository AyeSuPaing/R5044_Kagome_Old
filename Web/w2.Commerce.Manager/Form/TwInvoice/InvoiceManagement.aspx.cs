/*
=========================================================================================================
  Module      : Invoice Management(InvoiceManagement.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using w2.App.Common.Api;
using w2.Common.Logger;
using w2.Domain.TwInvoice;

public partial class Form_TwInvoice_InvoiceManagement : BasePage
{
	/// <summary>Get Invoice Failed Message</summary>
	private string CONST_GET_INVOICE_FAILED_MESSAGE = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_GET_INVOICE_FAILED_MESSAGE);
	/// <summary>Get Invoice Success Message</summary>
	private string CONST_GET_INVOICE_SUCCESS_MESSAGE = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_GET_INVOICE_SUCCESS_MESSAGE);

	/// <summary>
	/// Page Load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			var service = new TwInvoiceService();
			var models = service.GetAll();

			if (models == null)
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);

				return;
			}

			lbTotalRemainItems.Text = (models == null)
				? "0"
				: models.Where(invoice => IsValidPeriod(invoice))
					.Sum(invoice => CalculateRemaining(invoice)).ToString();
			lbEdit.Text = tbUpdate.Text = (models == null)
				? "0"
				: models.First().TwInvoiceAlertCount.ToString();

			rList.DataSource = models.Take(5).ToList();
			rList.DataBind();
		}
	}

	/// <summary>
	/// Is Valid Period
	/// </summary>
	/// <param name="model">Tw Invoice Model</param>
	/// <returns>Is in period</returns>
	private bool IsValidPeriod(TwInvoiceModel model)
	{
		var month = DateTime.Now.Month;
		int period = ((month % 2 != 0)
			? (month / 2)
			: (month / 2 - 1));
		var dateEnd = TwInvoiceApi.GetInvoiceDateEnd(DateTime.Now.Year, period);
		var dateStart = new DateTime(dateEnd.Year, dateEnd.Month - 1, 1);

		return ((model.TwInvoiceDateStart == dateStart) && (model.TwInvoiceDateEnd == dateEnd));
	}

	/// <summary>
	/// Calculate Remaining
	/// </summary>
	/// <param name="model">Model</param>
	/// <returns>Remaining</returns>
	protected decimal CalculateRemaining(TwInvoiceModel model)
	{
		return ((model.TwInvoiceNo.HasValue)
			? (model.TwInvoiceNoEnd - model.TwInvoiceNo.Value)
			: (model.TwInvoiceNoEnd - model.TwInvoiceNoStart + 1));
	}

	/// <summary>
	/// Button Import Click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnImport_Click(object sender, EventArgs e)
	{
		var response = new TwInvoiceApi().GetInvoice(tbNumber.Text.Trim(), true);
		tableResult.Visible = true;
		trSuccess.Visible = response.IsSuccess;
		trError.Visible = (response.IsSuccess == false);

		if (response.IsSuccess)
		{
			var service = new TwInvoiceService();
			var models = service.GetAll();
			lbTotalRemainItems.Text = (models == null)
				? "0"
				: models.Where(invoice => IsValidPeriod(invoice))
					.Sum(invoice => CalculateRemaining(invoice)).ToString();

			lbMessageGetElectronicTicketSuccess.Text =
				CONST_GET_INVOICE_SUCCESS_MESSAGE.Replace("@@ 1 @@", response.Size);

			rList.DataSource = models.Take(5).ToList();
			rList.DataBind();
		}
		else
		{
			lbErrorMessageGetElectronicTicketFailed.Text = CONST_GET_INVOICE_FAILED_MESSAGE;
			lbErrorMessageElectronicTicket.Text = response.ReturnMessage;
			FileLogger.WriteError(string.Format(
				"API GET INVOICE:{0}returnCode={1}&returnMessege={2}",
				Environment.NewLine,
				response.ReturnCode,
				response.ReturnMessage));
		}
	}

	/// <summary>
	/// Button Edit Click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, EventArgs e)
	{
		trInformation.Visible = false;
		trEdit.Visible = true;
	}

	/// <summary>
	/// Button Update Click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		var service = new TwInvoiceService();
		trInformation.Visible = true;
		trEdit.Visible = false;

		var errorMessage = ValidateInput(tbUpdate.Text);

		if (string.IsNullOrEmpty(errorMessage))
		{
			lbEdit.Text = tbUpdate.Text;
			service.UpdateInvoiceAlertCountForValidInvoice(int.Parse(tbUpdate.Text));
		}
		else
		{
			trInformation.Visible = false;
			trEdit.Visible = true;
			lbErrorAlert.Text = errorMessage;
		}
	}

	/// <summary>
	/// Validate Input For Update Alert Count
	/// </summary>
	/// <param name="updateText">updateText</param>
	/// <returns>errorMessage</returns>
	private string ValidateInput(string updateText)
	{
		var parameter = new Hashtable
			{
				{ Constants.FIELD_TWINVOICE_TW_INVOICE_ALERT_COUNT, updateText }
			};

		return Validator.Validate("TwUserInvoice", parameter);
	}
}