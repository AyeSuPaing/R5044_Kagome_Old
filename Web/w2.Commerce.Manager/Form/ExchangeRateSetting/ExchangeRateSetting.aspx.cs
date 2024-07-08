/*
=========================================================================================================
  Module      : 為替レート設定 (ExchangeRateSetting.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using w2.App.Common.Global.Region.Currency;
using w2.Common.Extensions;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain;

/// <summary>
/// 為替レート設定
/// </summary>
public partial class Form_ExchangeRateSetting_ExchangeRateSetting : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		divErrorMessage.Visible = false;
		lMessage.Text = string.Empty;
		divDisplayMessage.Visible = (this.IsDisplayCurrencyCodes == false);
		divDisplayList.Visible = this.IsDisplayCurrencyCodes;
		this.IsApiSuccess = false;

		if (!IsPostBack)
		{
			ExchangeRateDataBind();
		}
	}

	/// <summary>
	/// Item bound
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	protected void ItemBound(object sender, RepeaterItemEventArgs args)
	{
		// Get src currency code by repeater index
		var rListIndex = args.Item.ItemIndex;
		var srcCurrencyCode = this.SrcCurrencyCodes[rListIndex];

		// Create date for binding
		var exchangeRates = new List<ExchangeRateInput>();
		foreach (var dstCurrencyCode in this.DstCurrencyCodes)
		{
			ExchangeRateInput input = null;
			var model = DomainFacade.Instance.ExchangeRateService.Get(srcCurrencyCode, dstCurrencyCode);

			if (model == null)
			{
				input = new ExchangeRateInput()
				{
					SrcCurrencyCode = srcCurrencyCode,
					DstCurrencyCode = dstCurrencyCode,
					ExchangeRate = string.Empty
				};
			}
			else
			{
				model.ExchangeRate /= 1.000000000000m;
				input = new ExchangeRateInput(model);
			}

			exchangeRates.Add(input);
		}

		ViewState["DataToLog"] = exchangeRates;

		if ((args.Item.ItemType == ListItemType.Item)
			|| (args.Item.ItemType == ListItemType.AlternatingItem))
		{
			var rExchangeRate = (Repeater)args.Item.FindControl("rExchangeRate");
			rExchangeRate.DataSource = exchangeRates;
			rExchangeRate.DataBind();
		}
	}

	/// <summary>
	/// Exchange rate data bind
	/// </summary>
	protected void ExchangeRateDataBind()
	{
		rDstCurrencyCode.DataSource = this.DstCurrencyCodes;
		rDstCurrencyCode.DataBind();

		rSrcCurrencyCode.DataSource = this.SrcCurrencyCodes;
		rSrcCurrencyCode.DataBind();

		rList.DataSource = this.SrcCurrencyCodes;
		rList.DataBind();
	}

	/// <summary>
	/// Get exchange rate
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGetExchangeRate_Click(object sender, EventArgs e)
	{
		var apiUrl = new UrlCreator(Constants.EXCHANGERATE_BASEURL + "live")
			.AddParam("access_key", Constants.EXCHANGERATE_ACCESSKEY)
			.CreateUrl();
		var response = new HttpClient().GetStringAsync(new Uri(apiUrl)).Result;
		var jsonData = (JObject)JsonConvert.DeserializeObject(response);
		this.IsApiSuccess = (jsonData != null) && jsonData["success"].Value<bool>();
		if (string.IsNullOrEmpty(response) || (this.IsApiSuccess == false))
		{
			divErrorMessage.Visible = true;
			lMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_EXCHANGE_RATE_EXCHANGE_RATE_API_NOT_VALID);
			lMessage.Style.Add("color", "red");
			return;
		}

		ConvertToJsonAndLog();
		using (var process = new Process())
		{
			process.StartInfo.FileName = Constants.PHYSICALDIRPATH_GET_EXCHANGERATE_EXE;
			process.Start();
			process.WaitForExit();
		}

		ExchangeRateDataBind();
		DisplaySuccessMessage();
	}

	/// <summary>
	/// Clear exchange rate
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnClear_Click(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_EXCHANGERATE_SETTING).CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// Bulk update
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateAll_Click(object sender, EventArgs e)
	{
		ConvertToJsonAndLog();
		var exchangeRateList = new List<string>();
		foreach (RepeaterItem repeaterItem in rList.Items)
		{
			var rExchangeRateList = (Repeater)repeaterItem.FindControl("rExchangeRate");
			foreach (RepeaterItem rExchangeRateItem in rExchangeRateList.Items)
			{
				var exchangeRate = (TextBox)rExchangeRateItem.FindControl("tbExchangeRate");
				exchangeRateList.Add(exchangeRate.Text);
			}
		}

		var exchangeRateInputs = new List<ExchangeRateInput>();
		var exchangeRateListIndex = 0;
		foreach (var srcCurrencyCode in this.SrcCurrencyCodes)
		{
			foreach (var dstCurrencyCode in this.DstCurrencyCodes)
			{
				if (srcCurrencyCode != dstCurrencyCode)
				{
					var exchangeRateInput = new ExchangeRateInput
					{
						SrcCurrencyCode = srcCurrencyCode,
						DstCurrencyCode = dstCurrencyCode,
						ExchangeRate = exchangeRateList[exchangeRateListIndex]
					};
					exchangeRateInputs.Add(exchangeRateInput);
				}
				exchangeRateListIndex++;
			}
		}

		// Validate
		var errorMessage = string.Empty;
		foreach (var exchangeRateInput in exchangeRateInputs)
		{
			errorMessage += exchangeRateInput.Validate();
		}

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			divErrorMessage.Visible = true;
			lMessage.Text = errorMessage;
			lMessage.Style.Add("color", "red");
			return;
		}

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			DomainFacade.Instance.ExchangeRateService.Truncate(accessor);
			var exchangeRateDatas = exchangeRateInputs.Select(exchangeRateData => new
			{
				SrcCurrencyCode = exchangeRateData.SrcCurrencyCode,
				DstCurrencyCode = exchangeRateData.DstCurrencyCode,
				ExchangeRate = decimal.Parse(exchangeRateData.ExchangeRate)
			});
			DomainFacade.Instance.ExchangeRateService.BulkInsert(exchangeRateDatas.AsDataReader(), accessor);
			accessor.CommitTransaction();

			CurrencyManager.UpdateExchangeRateCache();

			ExchangeRateDataBind();
			DisplaySuccessMessage();
		}
	}

	/// <summary>
	/// Display success message
	/// </summary>
	private void DisplaySuccessMessage()
	{
		divErrorMessage.Visible = true;
		lMessage.Text = this.IsApiSuccess
			? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_GET_EXCHANGE_RATE_COMPLETED)
			: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PARAM_COMPLETED);
		lMessage.Style.Clear();
	}

	/// <summary>
	/// オブジェクトをJSON文字列に変換します
	/// </summary>
	private void ConvertToJsonAndLog()
	{
		var exchangeRates = new List<ExchangeRateInput>();
		foreach (var srcCurrencyCode in this.SrcCurrencyCodes)
		{
			foreach (var dstCurrencyCode in this.DstCurrencyCodes)
			{
				ExchangeRateInput input = null;
				var model = DomainFacade.Instance.ExchangeRateService.Get(srcCurrencyCode, dstCurrencyCode);

				if (model == null)
				{
					input = new ExchangeRateInput()
					{
						SrcCurrencyCode = srcCurrencyCode,
						DstCurrencyCode = dstCurrencyCode,
						ExchangeRate = string.Empty
					};
				}
				else
				{
					model.ExchangeRate /= 1.000000000000m;
					input = new ExchangeRateInput(model);
				}

				exchangeRates.Add(input);
			}
		}

		var contents = JsonConvert.SerializeObject(
			exchangeRates.Select(er => new { er.SrcCurrencyCode, er.DstCurrencyCode, er.ExchangeRate }),
			Formatting.Indented,
			new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			});

		FileLogger.Write("exchange_rate_info", contents);
	}

	/// <summary>Is display currency codes</summary>
	protected bool IsDisplayCurrencyCodes
	{
		get
		{
			var result = ((this.DstCurrencyCodes.Length > 0)
				&& (this.SrcCurrencyCodes.Length > 0));
			return result;
		}
	}
	/// <summary>Destination currency codes</summary>
	protected string[] DstCurrencyCodes
	{
		get { return Constants.EXCHANGERATE_DSTCURRENCYCODES.OrderBy(item => item).ToArray(); }
	}
	/// <summary>Source currency codes</summary>
	protected string[] SrcCurrencyCodes
	{
		get { return Constants.EXCHANGERATE_SRCCURRENCYCODES.OrderBy(item => item).ToArray(); }
	}
	/// <summary>Api成功可否</summary>
	protected bool IsApiSuccess { get; set; }
}
