/*
=========================================================================================================
  Module      : OrderPaymentNotification(OrderPaymentNotification.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using w2.App.Common.Order.Payment.GMO;
using w2.Common.Logger;

public partial class GMO_CVS_OrderPaymentNotification : GmoCvsApi
{
	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		GetParamInput(m_xmlFilePath);
	}

	/// <summary>
	/// Send order notice action
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSendOrderNotice_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(tbUrl.Text)) return;

		var paymentGmoCvs = new PaymentGmoCvs();
		lMessage.Text = paymentGmoCvs.SendParams(tbUrl.Text.Trim(), m_paramNameValue) ? "OK" : "NG";
	}

	/// <summary>
	/// Get param input
	/// </summary>
	/// <param name="xmlFilePath">Xml file path</param>
	/// <returns>Parameters input</returns>
	private void GetParamInput(string xmlFilePath)
	{
		var parameters = new NameValueCollection();
		var listParam = new List<String>();

		try
		{
			var xdoc = XDocument.Load(xmlFilePath);
			xdoc.DescendantNodes().OfType<XComment>().Remove();
			var responseNode = (XElement)xdoc.Root.FirstNode;
			foreach (XElement elementNode in responseNode.Elements())
			{
				parameters.Add(elementNode.Name.ToString(), elementNode.Value.Replace("@@datetime@@", DateTime.Now.ToString("yyyyMMddHHmmss")));
				listParam.Add(string.Format("{0}={1}", elementNode.Name, elementNode.Value.Replace("@@datetime@@", DateTime.Now.ToString("yyyyMMddHHmmss"))));
			}
			m_paramNameValue = parameters;
			m_paramString = string.Join("&", listParam);
		}
		catch (Exception exception)
		{
			FileLogger.WriteError(exception);
			parameters.Add("ErrCode", "E01");
			parameters.Add("ErrInfo", "Exception");
		}
	}

	/// <summary>
	/// Create params action
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCreateParam_Click(object sender, EventArgs e)
	{
		tbParams.Text = m_paramString;
	}

	/// <summary>Xml file path</summary>
	private string m_xmlFilePath = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				"Xml",
				"GmoCvs",
				"OrderPaymentNotification.xml");

	/// <summary>Param string</summary>
	private string m_paramString;
	/// <summary>Param name value</summary>
	private NameValueCollection m_paramNameValue;
}