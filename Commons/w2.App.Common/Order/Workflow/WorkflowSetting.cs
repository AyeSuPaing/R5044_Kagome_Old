/*
=========================================================================================================
  Module      : ワークフロー設定(WorkflowSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using jp.veritrans.tercerog.mdk;
using jp.veritrans.tercerog.mdk.dto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using w2.App.Common.Amazon;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Api;
using w2.App.Common.CrossMall;
using w2.App.Common.CrossPoint.Helper;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Flaps;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Input.Order;
using w2.App.Common.NextEngine;
using w2.App.Common.NextEngine.Helper;
using w2.App.Common.NextEngine.Response;
using w2.App.Common.Order.FixedPurchase;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Aftee;
using w2.App.Common.Order.Payment.Atobaraicom.Invoice;
using w2.App.Common.Order.Payment.Atobaraicom.Shipping;
using w2.App.Common.Order.Payment.Atone;
using w2.App.Common.Order.Payment.Boku;
using w2.App.Common.Order.Payment.Boku.Utils;
using w2.App.Common.Order.Payment.DSKDeferred.GetInvoice;
using w2.App.Common.Order.Payment.DSKDeferred.Helper;
using w2.App.Common.Order.Payment.DSKDeferred.Shipment;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.EScott;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMOAtokara;
using w2.App.Common.Order.Payment.GMOAtokara.Utils;
using w2.App.Common.Order.Payment.GMO.BillingConfirmation;
using w2.App.Common.Order.Payment.GMO.Helper;
using w2.App.Common.Order.Payment.GMO.Shipment;
using w2.App.Common.Order.Payment.GMO.Zcom.Sales;
using w2.App.Common.Order.Payment.JACCS.ATODENE;
using w2.App.Common.Order.Payment.JACCS.ATODENE.GetInvoice;
using w2.App.Common.Order.Payment.JACCS.ATODENE.Helper;
using w2.App.Common.Order.Payment.JACCS.ATODENE.Shipping;
using w2.App.Common.Order.Payment.LinePay;
using w2.App.Common.Order.Payment.NewebPay;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Payment.Paidy;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.Paypay;
using w2.App.Common.Order.Payment.Rakuten;
using w2.App.Common.Order.Payment.Score;
using w2.App.Common.Order.Payment.Score.Cancel;
using w2.App.Common.Order.Payment.Score.Delivery;
using w2.App.Common.Order.Payment.Score.Helper;
using w2.App.Common.Order.Payment.TriLinkAfterPay;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Request;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Payment.Veritrans.Helper;
using w2.App.Common.Order.Payment.Veritrans.ObjectElement;
using w2.App.Common.Order.Payment.Zeus;
using w2.App.Common.Payment;
using w2.App.Common.Recustomer;
using w2.App.Common.User;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Wrapper;
using w2.Domain;
using w2.Domain.DeliveryCompany;
using w2.Domain.DeliveryCompany.Helper;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchaseWorkflowSetting;
using w2.Domain.InvoiceAtobaraicom;
using w2.Domain.InvoiceDskDeferred;
using w2.Domain.Order;
using w2.Domain.OrderWorkflowSetting;
using w2.Domain.Point;
using w2.Domain.Score;
using w2.Domain.SerialKey;
using w2.Domain.ShopShipping;
using w2.Domain.TwOrderInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using GMOReissue = w2.App.Common.Order.Payment.GMO.Reissue;
using TransactionElement = w2.App.Common.Order.Payment.GMO.Shipment.TransactionElement;
using w2.App.Common.Order.Payment.Score.Helper;
using w2.Domain.Score;
using w2.App.Common.Order.Payment.Paygent;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// ワークフロー設定
	/// </summary>
	public class WorkflowSetting
	{
		// ステータス更新日付条件定数（設定時のドロップダウン値・DBにも条件として格納）
		public const DayOfWeek m_FIRST_DAY = DayOfWeek.Sunday;
		/// <summary>当日</summary>
		public const string m_SEARCH_STATUS_DATE_TODAY = "1";
		/// <summary>今週分</summary>
		public const string m_SEARCH_STATUS_DATE_THISWEEK = "2";
		/// <summary>前週分</summary>
		public const string m_SEARCH_STATUS_DATE_LASTWEEK = "3";
		/// <summary>今月分</summary>
		public const string m_SEARCH_STATUS_DATE_THISMONTH = "4";
		/// <summary>前月分</summary>
		public const string m_SEARCH_STATUS_DATE_LASTMONTH = "5";
		/// <summary>期間指定</summary>
		public const string m_SEARCH_STATUS_DATE_FROMTO = "0";
		/// <summary>受注ワークフロー用フィールド定数</summary>
		public static readonly string[] m_FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE =
		{
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE1,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE2,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE3,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE4,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE5,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE6,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE7,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE8,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE9,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE10,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE11,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE12,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE13,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE14,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE15,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE16,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE17,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE18,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE19,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE20,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE21,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE22,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE23,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE24,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE25,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE26,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE27,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE28,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE29,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE30,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE31,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE32,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE33,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE34,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE35,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE36,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE37,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE38,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE39,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE40,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE41,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE42,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE43,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE44,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE45,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE46,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE47,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE48,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE49,
			Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE50,
		};
		/// <summary>受注ワークフロー用フィールド定数（カセット）</summary>
		public static readonly string[] m_FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE =
		{
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE1,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE2,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE3,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE4,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE5,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE6,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE7,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE8,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE9,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE10,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE11,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE12,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE13,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE14,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE15,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE16,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE17,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE18,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE19,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE20,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE21,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE22,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE23,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE24,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE25,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE26,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE27,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE28,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE29,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE30,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE31,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE32,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE33,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE34,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE35,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE36,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE37,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE38,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE39,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE40,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE41,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE42,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE43,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE44,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE45,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE46,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE47,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE48,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE49,
			Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE50,
		};
		/// <summary>定期ワークフロー用フィールド定数</summary>
		public static readonly string[] m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE =
		{
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE1,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE2,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE3,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE4,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE5,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE6,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE7,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE8,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE9,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE10,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE11,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE12,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE13,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE14,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE15,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE16,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE17,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE18,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE19,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE20,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE21,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE22,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE23,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE24,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE25,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE26,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE27,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE28,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE29,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE30,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE31,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE32,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE33,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE34,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE35,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE36,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE37,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE38,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE39,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE40,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE41,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE42,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE43,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE44,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE45,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE46,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE47,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE48,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE49,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE50,
		};
		/// <summary>定期ワークフロー用フィールド定数（カセット） </summary>
		public static readonly string[] m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE =
		{
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE1,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE2,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE3,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE4,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE5,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE6,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE7,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE8,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE9,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE10,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE11,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE12,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE13,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE14,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE15,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE16,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE17,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE18,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE19,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE20,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE21,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE22,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE23,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE24,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE25,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE26,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE27,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE28,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE29,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE30,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE31,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE32,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE33,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE34,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE35,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE36,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE37,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE38,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE39,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE40,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE41,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE42,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE43,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE44,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE45,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE46,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE47,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE48,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE49,
			Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE50,
		};
		/// <summary>楽天ポイント利用方法</summary>
		public const string m_FIELD_RAKUTEN_POINT_USE_TYPE = "rakuten_point_use_type";
		/// <summary>返品更新返金更新日</summary>
		public const string m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS = "return_exchange_update_status";
		/// <summary>返品更新返金更新日（日）</summary>
		public const string m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_DAY = "return_exchange_update_status_day";
		/// <summary>返品更新返金更新日（From）</summary>
		public const string m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_FROM = "return_exchange_update_status_from";
		/// <summary>返品更新返金更新日（To）</summary>
		public const string m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_TO = "return_exchange_update_status_to";
		/// <summary>返品更新返金更新日（時）</summary>
		public const string m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_HOUR = "return_exchange_update_status_hour";
		/// <summary>返品更新返金更新日（分）</summary>
		public const string m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_MINUTE = "return_exchange_update_status_minute";
		/// <summary>返品更新返金更新日（秒）</summary>
		public const string m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_SECOND = "return_exchange_update_status_second";
		/// <summary>返品更新返金更新日（時間）</summary>
		public const string m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_TIME = "return_exchange_update_status_time";
		/// <summary>ステータス更新日</summary>
		public const string m_FIELD_UPDATE_STATUS = "update_status";
		/// <summary>ステータス更新日（日）</summary>
		public const string m_FIELD_UPDATE_STATUS_DAY = "update_status_day";
		/// <summary>ステータス更新日（From）</summary>
		public const string m_FIELD_UPDATE_STATUS_FROM = "update_status_from";
		/// <summary>ステータス更新日（To）</summary>
		public const string m_FIELD_UPDATE_STATUS_TO = "update_status_to";
		/// <summary>ステータス更新日（時）</summary>
		public const string m_FIELD_UPDATE_STATUS_HOUR = "update_status_hour";
		/// <summary>ステータス更新日（分）</summary>
		public const string m_FIELD_UPDATE_STATUS_MINUTE = "update_status_minute";
		/// <summary>ステータス更新日（秒）</summary>
		public const string m_FIELD_UPDATE_STATUS_SECOND = "update_status_second";
		/// <summary>ステータス更新日（時間）</summary>
		public const string m_FIELD_UPDATE_STATUS_TIME = "update_status_time";
		/// <summary>最終与信日時指定を含む</summary>
		public const string m_EXTERNAL_PAYMENT_AUTH_DATE_INCLUDE = "include";
		/// <summary>最終与信日時指定を含まない</summary>
		public const string m_EXTERNAL_PAYMENT_AUTH_DATE_EXCLUDE = "exclude";
		/// <summary>配送希望日指定あり</summary>
		public const string m_SHIPPINGDATE_SPECIFIED = "specified";
		/// <summary>配送希望日指定なし</summary>
		public const string m_SHIPPINGDATE_UNSPECIFIED = "unspecified";
		/// <summary>配送指定を含む</summary>
		public const string m_SHIPPINGDATE_INCLUDE = "include";
		/// <summary>配送指定を含まない</summary>
		public const string m_SHIPPINGDATE_EXCLUDE = "exclude";
		/// <summary>Shipping date from</summary>
		public const string m_SHIPPINGDATE_DATEFROM = "shipping_date_from";
		/// <summary>Shipping date to</summary>
		public const string m_SHIPPINGDATE_DATETO = "shipping_date_to";
		/// <summary>Shipping date separator character</summary>
		public const char m_SHIPPINGDATE_SEPARATOR_CHARACTER = '~';
		/// <summary>ノベルティID指定あり</summary>
		public const string m_NOVELTY_ID_SPECIFIED = "specified";
		/// <summary>ノベルティID指定なし</summary>
		public const string m_NOVELTY_ID_UNSPECIFIED = "unspecified";
		/// <summary>レコメンドID指定あり</summary>
		public const string m_RECOMMEND_ID_SPECIFIED = "specified";
		/// <summary>レコメンドID指定なし</summary>
		public const string m_RECOMMEND_ID_UNSPECIFIED = "unspecified";
		/// <summary>Scheduled shipping date specified</summary>
		public const string m_SCHEDULED_SHIPPINGDATE_SPECIFIED = "specified";
		/// <summary>Scheduled shipping date unspecified</summary>
		public const string m_SCHEDULED_SHIPPINGDATE_UNSPECIFIED = "unspecified";
		/// <summary>Scheduled shipping date from</summary>
		public const string m_SCHEDULED_SHIPPINGDATE_FROM = "scheduled_shipping_date_from";
		/// <summary>Scheduled shipping date to</summary>
		public const string m_SCHEDULED_SHIPPINGDATE_TO = "scheduled_shipping_date_to";
		/// <summary>Scheduled shipping date include</summary>
		/// <summary>Scheduled shipping date exclude</summary>
		public const string m_SCHEDULED_SHIPPINGDATE_EXCLUDE = "exclude";
		public const string m_SCHEDULED_SHIPPINGDATE_INCLUDE = "include";
		/// <summary>Scheduled shipping date separator character</summary>
		public const char m_SCHEDULED_SHIPPINGDATE_SEPARATOR_CHARACTER = '~';
		/// <summary>購入回数(出荷基準)FROM</summary>
		public const string m_FIXEDPURCHASE_SHIPPED_COUNT_FROM = "fixedPurchase_shipped_count_from";
		/// <summary>購入回数(出荷基準)TO</summary>
		public const string m_FIXEDPURCHASE_SHIPPED_COUNT_TO = "fixedPurchase_shipped_count_to";
		/// <summary>購入回数(注文基準)FROM</summary>
		public const string m_FIXEDPURCHASE_ORDER_COUNT_FROM = "fixedPurchase_order_count_from";
		/// <summary>購入回数(注文基準)TO</summary>
		public const string m_FIXEDPURCHASE_ORDER_COUNT_TO = "fixedPurchase_order_count_to";
		/// <summary>頒布会注文回数FROM</summary>
		public const string SUBSCRIPTIONBOX_ORDER_COUNT_FROM = "subscriptionBox_order_count_from";
		/// <summary>頒布会注文回数TO</summary>
		public const string SUBSCRIPTIONBOX_ORDER_COUNT_TO = "subscriptionBox_order_count_to";
		/// <summary>定期作成日時TO</summary>
		public const string m_FIXEDPURCHASE_DATE_CREATED_TO = "fixedPurchase_date_created_to";
		/// <summary>定期更新日時TO</summary>
		public const string m_FIXEDPURCHASE_DATE_CHANGED_TO = "fixedPurchase_date_changed_to";
		/// <summary>次回配送日時FROM</summary>
		public const string m_FIXEDPURCHASE_NEXT_SHIPPING_DATE_FROM = "fixedPurchase_next_shipping_date_from";
		/// <summary>次回配送日時TO</summary>
		public const string m_FIXEDPURCHASE_NEXT_SHIPPING_DATE_TO = "fixedPurchase_next_shipping_date_to";
		/// <summary>次々回配送日時FROM</summary>
		public const string m_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE_FROM = "fixedPurchase_next_next_shipping_date_from";
		/// <summary>次々回配送日時TO</summary>
		public const string m_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE_TO = "fixedPurchase_next_next_shipping_date_to";
		/// <summary>最終購入日時TO</summary>
		public const string m_FIXEDPURCHASE_LAST_ORDER_DATE_TO = "fixedPurchase_last_order_date_to";
		/// <summary>定期購入開始日時TO</summary>
		public const string m_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN_TO = "fixedPurchase_date_bgn_to";
		/// <summary>定期再開予定日時FROM</summary>
		public const string m_FIXEDPURCHASE_RESUME_DATE_FROM = "resume_date_from";
		/// <summary>定期再開予定日時TO</summary>
		public const string m_FIXEDPURCHASE_RESUME_DATE_TO = "resume_date_to";
		/// <summary>定期再開予定日指定あり</summary>
		public const string m_FIXEDPURCHASE_RESUME_DATE_SPECIFIED = "specified";
		/// <summary>定期再開予定日指定なし</summary>
		public const string m_FIXEDPURCHASE_RESUME_DATE_UNSPECIFIED = "unspecified";
		/// <summary>定期再開予定日分離子文字</summary>
		public const char m_FIXEDPURCHASE_RESUME_DATE_CHARACTER = '~';
		/// <summary>返品交換：決済種別</summary>
		public const string m_ORDER_RETURN_PAYMENT_KBN = "return_payment";
		/// <summary>外部決済ステータス：クレジットカード</summary>
		public const string m_ORDER_EXTERNAL_PAYMENT_STATUS_CARD = "external_payment_status_card";
		/// <summary>返品交換：外部決済ステータス：クレジットカード</summary>
		public const string m_ORDER_RETURN_EXTERNAL_PAYMENT_STATUS_CARD = "return_external_payment_status_card";
		/// <summary>外部決済ステータス：コンビニ（後払い）</summary>
		public const string m_ORDER_EXTERNAL_PAYMENT_STATUS_CVS = "external_payment_status_cvs";
		/// <summary>返品交換：外部決済ステータス：コンビニ（後払い）</summary>
		public const string m_ORDER_RETURN_EXTERNAL_PAYMENT_STATUS_CVS = "return_external_payment_status_cvs";
		/// <summary>外部決済ステータス：台湾後払い</summary>
		public const string m_ORDER_EXTERNAL_PAYMENT_STATUS_TRYLINK_AFTERPAY = "external_payment_status_trylink_afterpay";
		/// <summary>返品交換：外部決済ステータス：コンビニ（後払い）</summary>
		public const string m_ORDER_RETURN_EXTERNAL_PAYMENT_STATUS_TRYLINK_AFTERPAY = "return_external_payment_status_trylink_afterpay";
		/// <summary>外部決済ステータス：EcPay</summary>
		public const string m_ORDER_EXTERNAL_PAYMENT_STATUS_ECPAY = "external_payment_status_ecpay";
		/// <summary>外部決済ステータス：NewebPay</summary>
		public const string m_ORDER_EXTERNAL_PAYMENT_STATUS_NEWEBPAY = "external_payment_status_newebpay";
		/// <summary>最終与信日指定あり</summary>
		public const string m_LAST_AUTH_DATE_SPECIFIED = "specified";
		/// <summary>最終与信日指定なし</summary>
		public const string m_LAST_AUTH_DATE_UNSPECIFIED = "unspecified";
		/// <summary>最終与信日 from</summary>
		public const string m_LAST_AUTH_DATE_DATEFROM = "last_auth_date_from";
		/// <summary>最終与信日 to</summary>
		public const string m_LAST_AUTH_DATE_DATETO = "last_auth_shipping_date_to";
		/// <summary>最終与信日指定区切り文字</summary>
		public const char m_LAST_AUTH_DATE_SEPARATOR_CHARACTER = '~';
		/// <summary>モール連携ステータス</summary>
		public const string m_FIELD_MALL_LINK_STATUS = "mall_link_status";
		/// <summary>外部連携ステータス</summary>
		public const string m_FIELD_EXTERNAL_IMPORT_STATUS = "external_import_status";
		/// <summary>ワークフロー種別：受注ワークフロー</summary>
		public const string m_KBN_WORKFLOW_TYPE_ORDER = "order";
		/// <summary>ワークフロー種別：定期台帳ワークフロー</summary>
		public const string m_KBN_WORKFLOW_TYPE_FIXEDPURCHASE = "fixed_purchase";
		/// <summary>Workflow exec type: all</summary>
		public const string m_KBN_EXEC_TYPE_ALL = "all";
		/// <summary>対象注文種別(通常注文/定期注文)</summary>
		public const string m_TARGET_ORDER_TYPE = "target_order_type";
		/// <summary>注文拡張ステータス更新結果表示フラグ</summary>
		public bool[] m_displayUpdateOrderExtendStatusStatementResult = new bool[Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX];
		/// <summary>定期拡張ステータス更新結果表示フラグ</summary>
		public bool[] m_displayUpdateFixedPurchaseExtendStatusStatementResult = new bool[Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX];
		/// <summary>注文拡張ステータス更新必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool[] m_needsUpdateOrderExtendStatus = new bool[Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX];
		/// <summary>定期拡張ステータス更新必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool[] m_needsUpdateFixedPurchaseExtendStatus = new bool[Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX];
		/// <summary>Call API Recognize Flag</summary>
		public bool hasInvoiceUpdateOK = false;
		/// <summary>ステータス更新日（FROM）</summary>
		public const string UPDATE_STATUS_DATE_FROM = "update_status_date_from";
		/// <summary>ステータス更新日（TO）</summary>
		public const string UPDATE_STATUS_DATE_TO = "update_status_date_to";
		/// <summary>Realshop and storepickup</summary>
		public const string REALSHOP_AND_STOREPICKUP = "realshop_and_storepickup";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <remarks>ワークフロー未選択時などに利用</remarks>
		public WorkflowSetting()
		{
			this.OrderExtendStatusChangeValues = new string[Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX];
			this.FixedPurchaseExtendStatusChangeValues = new string[Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX];
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="drvOrderWorkflow">受注ワークフロー設定</param>
		/// <param name="strLoginOperatorDeptId">ログインオペレータ識別ID</param>
		/// <param name="strLoginOperatorName">ログインオペレータ名</param>
		/// <param name="workflowType">ワークフロー種別</param>
		public WorkflowSetting(
			DataRowView drvOrderWorkflow,
			string strLoginOperatorDeptId,
			string strLoginOperatorName,
			WorkflowTypes workflowType)
			: this()
		{
			//------------------------------------------------------
			// ワークフロー設定をプロパティへセット
			//------------------------------------------------------
			this.Setting = drvOrderWorkflow;
			this.LoginOperatorDeptId = strLoginOperatorDeptId;
			this.LoginOperatorName = strLoginOperatorName;
			this.WorkflowType = workflowType;

			//------------------------------------------------------
			// ライン表示の場合は条件を取得
			//------------------------------------------------------
			if (this.IsDisplayKbnLine)
			{
				GetSettingForDisplayLine();
			}
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <remarks>返品交換時などに利用</remarks>
		/// <param name="drvOrderWorkflow">受注ワークフロー設定</param>
		/// <param name="strLoginOperatorDeptId">ログインオペレータ識別ID</param>
		/// <param name="strLoginOperatorName">ログインオペレータ名</param>
		/// <param name="workflowType">ワークフロー種別</param>
		/// <param name="registerCardTranId">値登録向け・カード取引ID（登録する場合に値を格納する）</param>
		/// <param name="registerPaymentOrderId">値登録向け・決済注文ID（登録する場合に値を格納する）</param>
		/// <param name="registerExternalPaymentStatus">値登録向け・外部決済与信日時（登録する場合に値を格納する）</param>
		/// <param name="registerExternalPaymentAuthDate">値登録向け・外部決済ステータス（登録する場合に値を格納する）</param>
		/// <param name="registerOnlinePaymentStatus">値登録向け・オンライン決済ステータス（登録する場合に値を格納する）</param>
		/// <param name="registerPaymentMemo">値登録向け・決済連携メモ（登録する場合に値を格納する）</param>
		public WorkflowSetting(
			DataRowView drvOrderWorkflow,
			string strLoginOperatorDeptId,
			string strLoginOperatorName,
			WorkflowTypes workflowType,
			string registerCardTranId,
			string registerPaymentOrderId,
			string registerExternalPaymentStatus,
			string registerExternalPaymentAuthDate,
			string registerOnlinePaymentStatus,
			string registerPaymentMemo)
			: this(drvOrderWorkflow, strLoginOperatorDeptId, strLoginOperatorName, workflowType)
		{
			this.RegisterCardTranId = registerCardTranId;
			this.RegisterPaymentOrderId = registerPaymentOrderId;
			this.RegisterExternalPaymentStatus = registerExternalPaymentStatus;
			this.RegisterExternalPaymentAuthDate = registerExternalPaymentAuthDate;
			this.RegisterOnlinePaymentStatus = registerOnlinePaymentStatus;
			this.RegisterPaymentMemo = registerPaymentMemo;
		}

		/// <summary>
		/// 表示区分が一行表示の場合のセッティング読み込み
		/// </summary>
		private void GetSettingForDisplayLine()
		{
			// 各更新フラグ判定・ステートメント取得（受注ワーク）
			// 注文ステータス更新ステートメント取得
			this.OrderStatusChangeValue = GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STATUS_CHANGE, WorkflowTypes.Order);
			this.OrderStatusStatement = OrderCommon.GetUpdateOrderStatusStatement(this.OrderStatusChangeValue);
			this.NeedsUpdateOrderStatus = (string.IsNullOrEmpty(this.OrderStatusStatement) == false);

			// Order Invoice Status Api
			this.ApiInvoiceStatusValue = GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_API, WorkflowTypes.Order);
			this.NeedsCallApiInvoiceStatus = Constants.TWINVOICE_ENABLED && (string.IsNullOrEmpty(this.ApiInvoiceStatusValue) == false);

			// Order Invoice Status Change
			this.OrderInvoiceStatusChangeValue = GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_CHANGE, WorkflowTypes.Order);
			this.NeedsUpdateOrderInvoiceStatus = OrderCommon.DisplayTwInvoiceInfo()
				&& (string.IsNullOrEmpty(this.OrderInvoiceStatusChangeValue) == false);

			// 商品在庫変更判定
			this.ProductRealStockChangeValue = GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE, WorkflowTypes.Order);
			this.NeedsUpdateProductRealStock = Constants.REALSTOCK_OPTION_ENABLED
				&& ((this.ProductRealStockChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_RESERVED_STCOK)
					|| (this.ProductRealStockChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_FORWARD_STCOK)
					|| (this.ProductRealStockChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_CANCEL_REALSTCOK));

			// 入金ステータス更新判定・ステートメント取得
			this.PaymentStatusChangeValue = GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE, WorkflowTypes.Order);
			this.PaymentStatusStatement = OrderCommon.GetUpdateOrderPaymentStatusStatement(this.PaymentStatusChangeValue);
			this.NeedsUpdatePaymentStatus = (this.PaymentStatusStatement != null);

			// External Order Information Action
			this.ExternalOrderInfoActionValue = GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION, WorkflowTypes.Order);
			this.NeedsExecExternalOrderInfoAction = (string.IsNullOrEmpty(this.ExternalOrderInfoActionValue) == false);

			// 外部決済連携判定
			this.ExternalPaymentActionValue = GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, WorkflowTypes.Order);
			this.NeedsExecExternalPaymentAction = (string.IsNullOrEmpty(this.ExternalPaymentActionValue) == false);

			// 指定督促レベル変更判定・ステートメント取得
			this.DemandStatusChangeValue = GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_DEMAND_STATUS_CHANGE, WorkflowTypes.Order);
			if (string.IsNullOrEmpty(this.DemandStatusChangeValue) == false)
			{
				this.NeedsUpdateDemandStatus = true;
				this.DemandStatusStatement = "UpdateOrderDemandStatusLevel";	// 督促ステータスを「指定督促レベル」に変更
			}

			// 指定領収書出力フラグ変更判定・ステートメント取得
			if (Constants.RECEIPT_OPTION_ENABLED)
			{
				this.ReceiptOutputFlgChangeValue = GetValue(
					Constants.FIELD_ORDERWORKFLOWSETTING_RECEIPT_OUTPUT_FLG_CHANGE,
					WorkflowTypes.Order);
				if (string.IsNullOrEmpty(this.ReceiptOutputFlgChangeValue) == false)
				{
					this.ReceiptOutputFlgStatement = "UpdateOrderReceiptOutputFlg";
					this.NeedsUpdateReceiptOutputFlg = true;
				}
			}

			// 注文拡張ステータス判定
			for (var index = 0; index < Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; index++)
			{
				this.OrderExtendStatusChangeValues[index] = GetValue(m_FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE[index], WorkflowTypes.Order);
				this.NeedsUpdateOrderExtendStatus[index] = ((string.IsNullOrEmpty(this.OrderExtendStatusChangeValues[index])) == false);
			}

			// 定期拡張ステータス判定
			for (var index = 0; index < Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; index++)
			{
				this.FixedPurchaseExtendStatusChangeValues[index] = GetValue(m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE[index], WorkflowTypes.FixedPurchase);
				this.NeedsUpdateFixedPurchaseExtendStatus[index] = ((string.IsNullOrEmpty(this.FixedPurchaseExtendStatusChangeValues[index])) == false);
			}

			// 返品交換ステータス更新判定・ステートメント取得
			this.ReturnExchangeStatusChangeValue = GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_EXCHANGE_STATUS_CHANGE, WorkflowTypes.Order);
			this.ReturnExchangeStatusStatement = OrderCommon.GetUpdateOrderReturnExchangeStatusStatement(this.ReturnExchangeStatusChangeValue);
			this.NeedsUpdateReturnExchangeStatus = (this.ReturnExchangeStatusStatement != null);

			// 返金ステータス更新判定・ステートメント取得
			this.RepaymentStatusChangeValue = GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE, WorkflowTypes.Order);
			this.RepaymentStatusStatement = OrderCommon.GetUpdateOrderRepaymentStatusStatement(this.RepaymentStatusChangeValue);
			this.NeedsUpdateRepaymentStatus = (this.RepaymentStatusStatement != null);

			this.DisplayMailSendResult |= ((this.WorkflowType == WorkflowTypes.Order)
				&& (string.IsNullOrEmpty(GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_MAIL_ID)) == false));

			// メールIDセット
			this.MailIdValue = GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_MAIL_ID, WorkflowTypes.Order);

			// Update scheduled shipping date
			this.NeedsUpdateScheduledShippingDate =
				Constants.SCHEDULED_SHIPPING_DATE_OPTION_ENABLE
					&& (GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION, WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION_ON);

			// Order return
			this.NeedsUpdateOrderReturn =
				(GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_ACTION, WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_ACCEPT_RETURN);

			// 配送希望日更新判定
			this.NeedsUpdateShippingDate =
				((GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION, WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION_ON_CALCULATE_SCHEDULED_SHIPPING_DATE)
					|| (GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION, WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION_ON_NONCALCULATE_SCHEDULED_SHIPPING_DATE));

			// 出荷予定日自動計算判定
			this.NeedsCalculateScheduledShippingDate =
				(GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION, WorkflowTypes.Order) == Constants.FLG_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION_ON_CALCULATE_SCHEDULED_SHIPPING_DATE);

			// 各更新フラグ判定・ステートメント取得（定期ワーク）
			// 定期購入状態更新ステートメント取得
			this.FixedPurchaseIsAliveChangeValue = GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE, WorkflowTypes.FixedPurchase);
			this.NeedsUpdateFixedPurchaseIsAlive = string.IsNullOrEmpty(this.FixedPurchaseIsAliveChangeValue) == false;

			// 注文ステータス更新ステートメント取得
			this.FixedPurchasePaymentStatusChangeValue = GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE, WorkflowTypes.FixedPurchase);
			this.NeedsUpdateFixedPurchasePaymentStatus = string.IsNullOrEmpty(this.FixedPurchasePaymentStatusChangeValue) == false;

			// 次回配送日更新ステートメント取得
			this.NextShippingDateChangeValue = GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_SHIPPING_DATE_CHANGE, WorkflowTypes.FixedPurchase);
			this.NeedsUpdateNextShippingDate = string.IsNullOrEmpty(this.NextShippingDateChangeValue) == false;

			// 次々回配送日更新ステートメント取得
			this.NextNextShippingDateChangeValue = GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_NEXT_SHIPPING_DATE_CHANGE, WorkflowTypes.FixedPurchase);
			this.NeedsUpdateNextNextShippingDate = string.IsNullOrEmpty(this.NextNextShippingDateChangeValue) == false;

			// 次々回配送日自動計算判定
			this.NeedsCalculateNextNextShippingDate =
				(this.NextShippingDateChangeValue == Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_NEXT_SHIPPING_DATE_CHANGE_ACTION_ON_WITH_CALCULATE_NEXT_NEXT_SHIPPINGDATE)
				&& (this.NeedsUpdateNextNextShippingDate == false);

			// 配送不可エリア変更ステートメント取得
			this.FixedPurchaseStopUnavailableShippingAreaChangeValue
				= GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE, WorkflowTypes.FixedPurchase);
			this.NeedsUpdateFixedPurchaseStopUnavailableShippingArea
				= (string.IsNullOrEmpty(this.FixedPurchaseStopUnavailableShippingAreaChangeValue) == false);

			this.StorePickupStatusChangeValue = GetValue(
				Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STOREPICKUP_STATUS_CHANGE,
				WorkflowTypes.Order);

			this.NeedsUpdateStorePickupStatus = Constants.STORE_PICKUP_OPTION_ENABLED
				&& (string.IsNullOrEmpty(this.StorePickupStatusChangeValue) == false);

			//------------------------------------------------------
			// 完了画面表示フラグ設定
			//------------------------------------------------------
			this.DisplayUpdateOrderStatusResult = this.NeedsUpdateOrderStatus;
			this.DisplayUpdateProductRealStockResult = this.NeedsUpdateProductRealStock;
			this.DisplayUpdatePaymentStatusResult = this.NeedsUpdatePaymentStatus;
			this.DisplayExecExternalPaymentActionResult = this.NeedsExecExternalPaymentAction;
			this.DisplayUpdateDemandStatusResult = this.NeedsUpdateDemandStatus;
			this.DisplayUpdateReturnExchangeStatusResult = this.NeedsUpdateReturnExchangeStatus;
			this.DisplayUpdateRepaymentStatusResult = this.NeedsUpdateRepaymentStatus;
			this.DisplayUpdateScheduledShippingDateStatusResult = this.NeedsUpdateScheduledShippingDate;
			this.DisplayUpdateShippingDateStatusResult = this.NeedsUpdateShippingDate;
			this.DisplayUpdateFixedPurchaseIsAliveResult = this.NeedsUpdateFixedPurchaseIsAlive;
			this.DisplayUpdateFixedPurchasePaymentStatusResult = this.NeedsUpdateFixedPurchasePaymentStatus;
			this.DisplayUpdateNextShippingDateResult = this.NeedsUpdateNextShippingDate;
			this.DisplayUpdateNextNextShippingDateResult = this.NeedsUpdateNextNextShippingDate;
			this.DisplayUpdateFixedPurchaseStopUnavailableShippingAreaChangeResult = this.NeedsUpdateFixedPurchaseStopUnavailableShippingArea;
			for (var index = 0; index < Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; index++)
			{
				this.DisplayUpdateOrderExtendStatusStatementResult[index] = this.NeedsUpdateOrderExtendStatus[index];
				this.DisplayUpdateFixedPurchaseExtendStatusStatementResult[index] = this.NeedsUpdateFixedPurchaseExtendStatus[index];
			}
			this.DisplayUpdateOrderReturnResult = this.NeedsUpdateOrderReturn;
			this.DisplayUpdateReceiptOutputFlgResult = this.NeedsUpdateReceiptOutputFlg;
			this.DisplayUpdateOrderInvoiceStatusResult = this.NeedsUpdateOrderInvoiceStatus;
			this.DisplayUpdateOrderInvoiceApiResult = this.NeedsCallApiInvoiceStatus;
			this.DisplayExecExternalOrderInfoActionResult = this.NeedsExecExternalOrderInfoAction;
			this.DisplayUpdateStorePicupStatusResult = this.NeedsUpdateStorePickupStatus;
		}

		/// <summary>
		/// 注文ワークフロー設定情報データビュー取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="workFlowKbn">ワークフロー区分</param>
		/// <param name="workFlowNo">枝番</param>
		/// <param name="workflowType"></param>
		/// <returns>注文ワークフロー設定情報データビュー</returns>
		public DataView GetOrderWorkflowSetting(string shopId, string workFlowKbn, string workFlowNo, string workflowType)
		{
			if (workflowType == m_KBN_WORKFLOW_TYPE_ORDER)
			{
				var orderworkflowSetting =
					new OrderWorkflowSettingService().GetOrderWorkflowSettingInDataView(shopId, workFlowKbn, workFlowNo);
				return orderworkflowSetting;
			}

			var fixedPurchaseWorkflowSetting =
				new FixedPurchaseWorkflowSettingService()
					.GetFixedPurchaseWorkflowSettingInDataView(
						shopId,
						workFlowKbn,
						workFlowNo);
			return fixedPurchaseWorkflowSetting;
		}

		/// <summary>
		/// 表示区分がカセット表示の場合のセッティング読み込み
		/// </summary>
		/// <param name="strCasetteSelectedFieldAndStatusAndMail">選択フィールド＆ステータス＆メール</param>
		private void GetSettingForDisplayCassette(string strCasetteSelectedFieldAndStatusAndMail)
		{
			//------------------------------------------------------
			// 初期化
			//------------------------------------------------------
			this.OrderStatusChangeValue
				= this.ProductRealStockChangeValue
				= this.PaymentStatusChangeValue
				= this.ExternalPaymentActionValue
				= this.DemandStatusChangeValue
				= this.ReturnExchangeStatusChangeValue
				= this.RepaymentStatusChangeValue
				= this.FixedPurchaseStopUnavailableShippingAreaChangeValue
				= this.FixedPurchaseIsAliveChangeValue
				= this.FixedPurchasePaymentStatusChangeValue
				= this.OrderInvoiceStatusChangeValue
				= this.ApiInvoiceStatusValue
				= this.ExternalOrderInfoActionValue = null;
			this.OrderExtendStatusChangeValues = new string[Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX];
			this.ReceiptOutputFlgChangeValue = "";

			this.NeedsUpdateOrderStatus
				= this.NeedsUpdateProductRealStock
				= this.NeedsUpdatePaymentStatus
				= this.NeedsExecExternalPaymentAction
				= this.NeedsUpdateDemandStatus
				= this.NeedsUpdateReturnExchangeStatus
				= this.NeedsUpdateRepaymentStatus
				= this.NeedsUpdateFixedPurchaseIsAlive
				= this.NeedsUpdateFixedPurchasePaymentStatus
				= this.NeedsUpdateFixedPurchaseStopUnavailableShippingArea
				= this.NeedsUpdateOrderReturn
				= this.NeedsUpdateReceiptOutputFlg
				= this.NeedsUpdateOrderInvoiceStatus
				= this.NeedsCallApiInvoiceStatus
				= this.NeedsExecExternalOrderInfoAction = false;
			this.FixedPurchaseExtendStatusChangeValues = new string[Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX];

			for (var index = 0; index < Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; index++)
			{
				this.NeedsUpdateOrderExtendStatus[index] = false;
				this.NeedsUpdateFixedPurchaseExtendStatus[index] = false;
			}

			this.MailIdValue = null;

			//------------------------------------------------------
			// カセット表示時条件取得
			//------------------------------------------------------
			// ステータス更新あり？
			if (strCasetteSelectedFieldAndStatusAndMail != "")
			{
				// ステータス、メール送信ID取得
				string[] strCassetteStatuses = strCasetteSelectedFieldAndStatusAndMail.Split('&');
				string strCassetteStatusFieldName = strCassetteStatuses[0];
				string strCassetteStatusValue = strCassetteStatuses[1];
				string strCassetteMailId = strCassetteStatuses[2];

				// 一度でも「メール送信」対象となった
				this.DisplayMailSendResult |= (strCassetteMailId != "");

				//------------------------------------------------------
				// 注文ステータス更新ステートメント取得
				//------------------------------------------------------
				if (strCassetteStatusFieldName == Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STATUS_CHANGE)
				{
					this.OrderStatusChangeValue = strCassetteStatusValue;
					this.OrderStatusStatement = OrderCommon.GetUpdateOrderStatusStatement(this.OrderStatusChangeValue);
					if (this.OrderStatusStatement != null)
					{
						this.NeedsUpdateOrderStatus = true;
					}
				}

				if (OrderCommon.DisplayTwInvoiceInfo()
					&& (strCassetteStatusFieldName
						== Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_CHANGE))
				{
					this.OrderInvoiceStatusChangeValue = strCassetteStatusValue;
					this.NeedsUpdateOrderInvoiceStatus = (string.IsNullOrEmpty(this.OrderInvoiceStatusChangeValue) == false);
				}

				if (Constants.TWINVOICE_ENABLED
					&& (strCassetteStatusFieldName
						== Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_API))
				{
					this.ApiInvoiceStatusValue = strCassetteStatusValue;
					this.NeedsCallApiInvoiceStatus = Constants.TWINVOICE_ENABLED
						&& (string.IsNullOrEmpty(this.ApiInvoiceStatusValue) == false);
				}

				//------------------------------------------------------
				// 実在庫連動判定
				//------------------------------------------------------
				if (strCassetteStatusFieldName == Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PRODUCT_REALSTOCK_CHANGE)
				{
					// 実在庫利用が有効な場合
					if (Constants.REALSTOCK_OPTION_ENABLED)
					{
						this.ProductRealStockChangeValue = strCassetteStatusValue;

						// 商品在庫変更判定（実在庫引当（在庫引当）、実在庫出荷（商品出荷）、実在庫引当戻しの場合）
						if ((this.ProductRealStockChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_RESERVED_STCOK)
							|| (this.ProductRealStockChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_FORWARD_STCOK)
							|| (this.ProductRealStockChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_CANCEL_REALSTCOK))
						{
							this.NeedsUpdateProductRealStock = true;
						}
					}
				}

				//------------------------------------------------------
				// 入金ステータス更新ステートメント取得
				//------------------------------------------------------
				if (strCassetteStatusFieldName == Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PAYMENT_STATUS_CHANGE)
				{
					this.PaymentStatusChangeValue = strCassetteStatusValue;
					this.PaymentStatusStatement = OrderCommon.GetUpdateOrderPaymentStatusStatement(this.PaymentStatusChangeValue);
					if (this.PaymentStatusStatement != null)
					{
						this.NeedsUpdatePaymentStatus = true;
					}
				}

				//------------------------------------------------------
				// 外部決済連携判定
				//------------------------------------------------------
				if (strCassetteStatusFieldName == Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_PAYMENT_ACTION)
				{
					this.ExternalPaymentActionValue = strCassetteStatusValue;
					if (this.ExternalPaymentActionValue != "")
					{
						this.NeedsExecExternalPaymentAction = true;
					}
				}

				// External Order Information Action
				if (strCassetteStatusFieldName == Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_ORDER_INFO_ACTION)
				{
					this.ExternalOrderInfoActionValue = strCassetteStatusValue;
					this.NeedsExecExternalOrderInfoAction = (string.IsNullOrEmpty(this.ExternalOrderInfoActionValue) == false);
				}

				// 配送不可エリア停止変更判定
				if (strCassetteStatusFieldName == Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE)
				{
					this.FixedPurchaseStopUnavailableShippingAreaChangeValue = strCassetteStatusValue;
					this.NeedsUpdateFixedPurchaseStopUnavailableShippingArea = (string.IsNullOrEmpty(this.FixedPurchaseStopUnavailableShippingAreaChangeValue) == false);
				}

				//------------------------------------------------------
				// 督促ステータス判定
				//------------------------------------------------------
				if (strCassetteStatusFieldName == Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEMAND_STATUS_CHANGE)
				{
					this.DemandStatusChangeValue = strCassetteStatusValue;
					if (this.DemandStatusChangeValue != "")
					{
						// 督促ステータスを「指定督促レベル」に変更
						this.DemandStatusStatement = "UpdateOrderDemandStatusLevel";

						this.NeedsUpdateDemandStatus = true;
					}
				}

				// 領収書出力フラグ判定
				if (Constants.RECEIPT_OPTION_ENABLED
					&& (strCassetteStatusFieldName == Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RECEIPT_OUTPUT_FLG_CHANGE))
				{
					this.ReceiptOutputFlgChangeValue = strCassetteStatusValue;
					if (string.IsNullOrEmpty(this.ReceiptOutputFlgChangeValue) == false)
					{
						this.ReceiptOutputFlgStatement = "UpdateOrderReceiptOutputFlg";
						this.NeedsUpdateReceiptOutputFlg = true;
					}
				}

				//------------------------------------------------------
				// 返品交換ステータス判定
				//------------------------------------------------------
				if (strCassetteStatusFieldName == Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_EXCHANGE_STATUS_CHANGE)
				{
					this.ReturnExchangeStatusChangeValue = strCassetteStatusValue;
					this.ReturnExchangeStatusStatement = OrderCommon.GetUpdateOrderReturnExchangeStatusStatement(this.ReturnExchangeStatusChangeValue);
					if (this.ReturnExchangeStatusStatement != null)
					{
						this.NeedsUpdateReturnExchangeStatus = true;
					}
				}

				//------------------------------------------------------
				// 返金ステータス更新ステートメント取得
				//------------------------------------------------------
				if (strCassetteStatusFieldName == Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_REPAYMENT_STATUS_CHANGE)
				{
					this.RepaymentStatusChangeValue = strCassetteStatusValue;
					this.RepaymentStatusStatement = OrderCommon.GetUpdateOrderRepaymentStatusStatement(this.RepaymentStatusChangeValue);
					if (this.RepaymentStatusStatement != null)
					{
						this.NeedsUpdateRepaymentStatus = true;
					}
				}

				//------------------------------------------------------
				// 注文拡張ステータス判定
				//------------------------------------------------------
				for (var index = 0; index < Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; index++)
				{
					if (strCassetteStatusFieldName == m_FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[index])
					{
						this.OrderExtendStatusChangeValues[index] = strCassetteStatusValue;
						this.NeedsUpdateOrderExtendStatus[index]
							= (this.OrderExtendStatusChangeValues[index] != "");
					}
					else
					{
						this.OrderExtendStatusChangeValues[index] = "";
					}
				}

				// Get update order return
				if (strCassetteStatusFieldName == Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_ACTION)
				{
					if (string.IsNullOrEmpty(strCassetteStatusValue) == false)
					{
						this.NeedsUpdateOrderReturn = true;
					}
				}

				// 定期拡張ステータス判定
				for (var index = 0; index < Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; index++)
				{
					if (strCassetteStatusFieldName == m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[index])
					{
						this.FixedPurchaseExtendStatusChangeValues[index] = strCassetteStatusValue;
						this.NeedsUpdateFixedPurchaseExtendStatus[index]
							= (this.FixedPurchaseExtendStatusChangeValues[index] != "");
					}
					else
					{
						this.FixedPurchaseExtendStatusChangeValues[index] = "";
					}
				}

				// 定期購入状況更新ステートメント取得
				if (strCassetteStatusFieldName == Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_IS_ALIVE_CHANGE)
				{
					this.FixedPurchaseIsAliveChangeValue = strCassetteStatusValue;
					if (string.IsNullOrEmpty(this.FixedPurchaseIsAliveChangeValue) == false)
					{
						this.NeedsUpdateFixedPurchaseIsAlive = true;
					}
				}

				// 注文ステータス更新ステートメント取得
				if (strCassetteStatusFieldName == Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE)
				{
					this.FixedPurchasePaymentStatusChangeValue = strCassetteStatusValue;
					if (string.IsNullOrEmpty(this.FixedPurchasePaymentStatusChangeValue) == false)
					{
						this.NeedsUpdateFixedPurchasePaymentStatus = true;
					}
				}

				if (Constants.STORE_PICKUP_OPTION_ENABLED
					&& (strCassetteStatusFieldName == Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STOREPICKUP_STATUS_CHANGE))
				{
					this.StorePickupStatusChangeValue = strCassetteStatusValue;
					this.NeedsUpdateStorePickupStatus = (string.IsNullOrEmpty(this.StorePickupStatusChangeValue) == false);
				}

				//------------------------------------------------------
				// 注文送信メール設定
				//------------------------------------------------------
				this.MailIdValue = strCassetteMailId;

				//------------------------------------------------------
				// 完了画面表示フラグ設定
				//  - カセット表示時はtrueの値を引き継ぐ（trueの列が最終的に結果画面へ表示される）
				//------------------------------------------------------
				this.DisplayUpdateOrderStatusResult |= this.NeedsUpdateOrderStatus;
				this.DisplayUpdateProductRealStockResult |= this.NeedsUpdateProductRealStock;
				this.DisplayUpdatePaymentStatusResult |= this.NeedsUpdatePaymentStatus;
				this.DisplayExecExternalPaymentActionResult |= this.NeedsExecExternalPaymentAction;
				this.DisplayUpdateDemandStatusResult |= this.NeedsUpdateDemandStatus;
				this.DisplayUpdateReturnExchangeStatusResult |= this.NeedsUpdateReturnExchangeStatus;
				this.DisplayUpdateRepaymentStatusResult |= this.NeedsUpdateRepaymentStatus;
				this.DisplayUpdateScheduledShippingDateStatusResult |= this.NeedsUpdateScheduledShippingDate;
				this.DisplayUpdateShippingDateStatusResult |= this.NeedsUpdateShippingDate;
				this.DisplayUpdateFixedPurchaseIsAliveResult = this.NeedsUpdateFixedPurchaseIsAlive;
				this.DisplayUpdateFixedPurchasePaymentStatusResult = this.NeedsUpdateFixedPurchasePaymentStatus;
				this.DisplayUpdateFixedPurchaseStopUnavailableShippingAreaChangeResult |= this.NeedsUpdateFixedPurchaseStopUnavailableShippingArea;
				for (var index = 0; index < Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; index++)
				{
					this.DisplayUpdateOrderExtendStatusStatementResult[index] |= this.NeedsUpdateOrderExtendStatus[index];
					this.DisplayUpdateFixedPurchaseExtendStatusStatementResult[index] |= this.NeedsUpdateFixedPurchaseExtendStatus[index];
				}
				this.DisplayUpdateOrderReturnResult |= this.NeedsUpdateOrderReturn;
				this.DisplayUpdateReceiptOutputFlgResult |= this.NeedsUpdateReceiptOutputFlg;
				this.DisplayUpdateOrderInvoiceStatusResult |= this.NeedsUpdateOrderInvoiceStatus;
				this.DisplayUpdateOrderInvoiceApiResult |= this.NeedsCallApiInvoiceStatus;
				this.DisplayExecExternalOrderInfoActionResult |= this.NeedsExecExternalOrderInfoAction;
				this.DisplayUpdateStorePicupStatusResult |= this.NeedsUpdateStorePickupStatus;
			}
		}

		/// <summary>
		/// アクション(一行表示用）
		/// </summary>
		/// <param name="htOrder">注文情報</param>
		/// <param name="orderOld">更新前注文</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="updateNextengineOrder">更新ネクストエンジン受注情報</param>
		/// <param name="deliveryCompanies">配送会社配列</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>実行結果格納用ActionResultUnit</returns>
		public ActionResultUnit ActionForLine(Hashtable htOrder,
			DataView orderOld,
			UpdateHistoryAction updateHistoryAction,
			NEOrder[] updateNextengineOrder,
			DeliveryCompanyModel[] deliveryCompanies,
			SqlAccessor sqlAccessor)
		{
			return ActionForCassette(
				htOrder,
				orderOld,
				null,
				updateHistoryAction,
				updateNextengineOrder,
				deliveryCompanies,
				sqlAccessor);
		}

		/// <summary>
		/// アクション(一行表示用）定期
		/// </summary>
		/// <param name="fixedPurchase">注文情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>実行結果格納用ActionResultUnit</returns>
		public ActionResultUnit ActionForLineFixedPurchase(Hashtable fixedPurchase, UpdateHistoryAction updateHistoryAction, SqlAccessor sqlAccessor)
		{
			return ActionForCassetteFixedPurchase(fixedPurchase, null, updateHistoryAction, sqlAccessor);
		}

		/// <summary>
		/// アクション(カセット表示用）
		/// </summary>
		/// <param name="htOrder">注文情報</param>
		/// <param name="orderOld">更新前注文</param>
		/// <param name="strCasetteSelectedFieldAndStatusAndMail">選択フィールド＆ステータス＆メール</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="updateNextengineOrder">更新ネクストエンジン受注情報</param>
		/// <param name="deliveryCompanies">配送会社配列</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>実行結果格納用ActionResultUnit</returns>
		public ActionResultUnit ActionForCassette(
			Hashtable htOrder,
			DataView orderOld,
			string strCasetteSelectedFieldAndStatusAndMail,
			UpdateHistoryAction updateHistoryAction,
			NEOrder[] updateNextengineOrder,
			DeliveryCompanyModel[] deliveryCompanies,
			SqlAccessor sqlAccessor)
		{
			//------------------------------------------------------
			// カセット表示ワークフロー設定
			//------------------------------------------------------
			if (this.IsDisplayKbnCassette)
			{
				GetSettingForDisplayCassette(strCasetteSelectedFieldAndStatusAndMail);
			}

			//------------------------------------------------------
			// アクション
			//------------------------------------------------------
			return Action(htOrder,
				orderOld,
				updateHistoryAction,
				updateNextengineOrder,
				deliveryCompanies,
				sqlAccessor);
		}

		/// <summary>
		/// アクション(カセット表示用）
		/// </summary>
		/// <param name="fixedPurchase">定期台帳情報</param>
		/// <param name="casetteSelectedFieldAndStatusAndMail">選択フィールド＆ステータス＆メール</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>実行結果格納用ActionResultUnit</returns>
		public ActionResultUnit ActionForCassetteFixedPurchase(
			Hashtable fixedPurchase,
			string casetteSelectedFieldAndStatusAndMail,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// カセット表示ワークフロー設定
			if (this.IsDisplayKbnCassette) GetSettingForDisplayCassette(casetteSelectedFieldAndStatusAndMail);

			// アクション
			return ActionForFixedPurchase(fixedPurchase, updateHistoryAction, accessor);
		}

		/// <summary>
		/// アクション(受注)
		/// </summary>
		/// <param name="htOrder">注文情報</param>
		/// <param name="orderOld">更新前注文</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="updateNextengineOrder">更新ネクストエンジン受注情報</param>
		/// <param name="deliveryCompanies">配送会社配列</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>実行結果格納用ActionResultUnit</returns>
		private ActionResultUnit Action(Hashtable htOrder,
			DataView orderOld,
			UpdateHistoryAction updateHistoryAction,
			NEOrder[] updateNextengineOrder,
			DeliveryCompanyModel[] deliveryCompanies,
			SqlAccessor sqlAccessor)
		{
			var targetOrderId = (string)htOrder[Constants.FIELD_ORDER_ORDER_ID];
			var targetShopId = (string)htOrder[Constants.FIELD_ORDER_SHOP_ID];
			var targetUserId = (string)htOrder[Constants.FIELD_ORDER_USER_ID];

			// 結果格納Hashtabe作成
			ActionResultUnit aruResult = new ActionResultUnit(targetOrderId);

			// デジタルコンテンツ商品あり判定
			var IsDigitalContents = false;
			if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
			{
				foreach (DataRowView drv in orderOld)
				{
					if ((string)drv[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG] == Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID)
					{
						IsDigitalContents = true;
						break;
					}
				}
			}

			// 注文ステータス・商品在庫変更・入金・外部決済連携・督促ステータス、返品交換ステータス、返金ステータス判定
			if (this.NeedsUpdate)
			{
				try
				{
					// はじめに更新ロックを掛けておく
					new OrderService().GetUpdLock(targetOrderId, sqlAccessor);

					if (this.NeedsExecExternalOrderInfoAction
						&& (this.ExternalOrderInfoActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_NEXTENGINE_IMPORT))
					{
						var errorMessage = string.Empty;
						this.OrderStatusChangeValue = GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STATUS_CHANGE, WorkflowTypes.Order);
						this.OrderStatusStatement = OrderCommon.GetUpdateOrderStatusStatement(this.OrderStatusChangeValue);
						this.NeedsUpdateOrderStatus = (string.IsNullOrEmpty(this.OrderStatusStatement) == false);

						var isGift = ((string)orderOld[0][Constants.FIELD_ORDER_GIFT_FLG] == Constants.FLG_ORDER_GIFT_FLG_ON);

						if (Constants.GIFTORDER_OPTION_ENABLED && Constants.NE_OPTION_ENABLED && isGift)
						{
							errorMessage = ExecUpdateOrderForNextEngineWithGift(
								targetOrderId,
								updateNextengineOrder,
								deliveryCompanies,
								sqlAccessor,
								orderOld);
						}
						else
						{
							errorMessage = ExecUpdateOrderForNextEngine(
								targetOrderId,
								updateNextengineOrder,
								deliveryCompanies,
								sqlAccessor);
						}

						if (string.IsNullOrEmpty(htOrder[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE].ToString()))
						{
							htOrder[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE] = DateTime.Now;
						}

						if (string.IsNullOrEmpty(errorMessage) == false)
						{
							aruResult.ResultExternalOrderInfoAction = OrderCommon.ResultKbn.UpdateNG;
							aruResult.ErrorMessages.Add(errorMessage);
						}
					}

					var crossMallGetApiErrorMessage = string.Empty;
					if(this.NeedsExecExternalOrderInfoAction
						&& (this.ExternalOrderInfoActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_CROSSMALL_UPDATE_STATUS))
					{
						var crossMallOrderResult = CrossMallApiFacade.GetOrder(targetOrderId);
						crossMallGetApiErrorMessage = crossMallOrderResult.ResultStatus.Message;

						var isShipped = CrossMallApiFacade.CheckOrderIsShippedByResultSet(crossMallOrderResult, targetOrderId);
						this.NeedsUpdateOrderStatus = (string.IsNullOrEmpty(this.OrderStatusStatement) == false) && isShipped;
					}

					// Get orderworkflow information
					var action = string.Format(
						"{0}:{1}, {2}:{3}, {4}:{5}{6}",
						Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN,
						GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN),
						Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO,
						GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO),
						Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NAME,
						GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NAME),
						(string.IsNullOrEmpty(this.MailIdValue) ? "" : string.Format(", {0}:{1}", Constants.FIELD_ORDERWORKFLOWSETTING_MAIL_ID, this.MailIdValue)));

					OrderHistory history = new OrderHistory
					{
						OrderId = targetOrderId,
						Action = OrderHistory.ActionType.EcOrderWorkflow,
						OpearatorName = this.LoginOperatorName,
						Accessor = sqlAccessor,
						UpdateAction = GetWorkflowActionForHistory(targetOrderId, sqlAccessor, IsDigitalContents),
						ExtendAction = action
					};

					// Begin write history
					history.HistoryBegin();

					// 出荷予定日更新
					if (this.NeedsUpdateScheduledShippingDate)
					{
						// Update Scheduled Shipping Date All
						var orderShipping = new OrderService().Get(targetOrderId, sqlAccessor).Shippings;
						DateTime tmpScheduledShippingDate;
						orderShipping.First().ScheduledShippingDate =
							DateTime.TryParse(StringUtility.ToEmpty(htOrder[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE]), out tmpScheduledShippingDate)
								? tmpScheduledShippingDate
								: (DateTime?)null;
						var scheduledShippingDate = StringUtility.ToEmpty(htOrder[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE]);
						orderShipping.ToList().ForEach(
							shipping => shipping.ScheduledShippingDate = string.IsNullOrEmpty(scheduledShippingDate)
							? (DateTime?)null
								: DateTime.Parse(scheduledShippingDate));
						var result = new OrderService().UpdateShippingForModify(orderShipping, this.LoginOperatorName, UpdateHistoryAction.DoNotInsert, sqlAccessor);

						if (result <= 0)
						{
							aruResult.ResultScheduledShippingDateAction = OrderCommon.ResultKbn.UpdateNG;
							aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SCHEDULE_SHIPPING_DATE_UPDATE_NG));
						}
					}
					else
					{
						aruResult.ResultScheduledShippingDateAction = OrderCommon.ResultKbn.NoUpdate;
					}

					// 配送希望日更新
					if (this.NeedsUpdateShippingDate)
					{
						// 配送希望日一括更新
						var orderService = new OrderService();
						var order = orderService.Get(targetOrderId, sqlAccessor);
						var orderShippingDate =
							StringUtility.ToEmpty(htOrder[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE]);

						var orderShippings = order.Shippings.Select(shipping => CalculateShippingDate(orderShippingDate, shipping)).ToArray();

						var result = orderService.UpdateShippingForModify(orderShippings, this.LoginOperatorName, UpdateHistoryAction.DoNotInsert, sqlAccessor);

						if (result <= 0)
						{
							aruResult.ResultShippingDateAction = OrderCommon.ResultKbn.UpdateNG;
							aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SHIPPING_DATE_UPDATE_NG));
						}
					}
					else
					{
						aruResult.ResultShippingDateAction = OrderCommon.ResultKbn.NoUpdate;
					}

					//------------------------------------------------------
					// 商品在庫を更新する場合
					// ※各関数内でトランザクション中のテーブル更新順を守るための注文テーブルの更新ロック取得を行っている
					//------------------------------------------------------
					if (this.NeedsUpdateProductRealStock)
					{
						switch (this.ProductRealStockChangeValue)
						{
							// 実在庫引当処理・結果格納
							case Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_RESERVED_STCOK:
								aruResult.ResultProductRealStockChange = OrderCommon.UpdateOrderItemRealStockReserved(
									targetOrderId,
									targetShopId,
									this.LoginOperatorName,
									UpdateHistoryAction.DoNotInsert,
									sqlAccessor);
								break;

							// 実在庫出荷
							case Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_FORWARD_STCOK:
								aruResult.ResultProductRealStockChange = OrderCommon.UpdateOrderItemRealStockShipped(
									targetOrderId,
									targetShopId,
									this.LoginOperatorName,
									UpdateHistoryAction.DoNotInsert,
									sqlAccessor);
								break;

							// 実在庫引当戻し
							case Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_CANCEL_REALSTCOK:
								aruResult.ResultProductRealStockChange = OrderCommon.UpdateOrderItemRealStockCanceled(
									targetOrderId,
									targetShopId,
									this.LoginOperatorName,
									UpdateHistoryAction.DoNotInsert,
									sqlAccessor);
								break;
						}

						if (aruResult.ResultProductRealStockChange == OrderCommon.ResultKbn.UpdateNG)
						{
							aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_PRODUCTSTOCK_UPDATE_NG));
						}
					}
					// 更新処理を行わない場合
					else
					{
						aruResult.ResultProductRealStockChange = OrderCommon.ResultKbn.NoUpdate;
					}

					// Return Order
					if (this.NeedsUpdateOrderReturn)
					{
						var workPage = new WorkflowReturnExchange(this.LoginOperatorName, this.LoginOperatorDeptId);

						var orderModelOld = new OrderModel(orderOld[0]);
						var returnOrder = workPage.GetReturnOrder(orderModelOld.OrderId, sqlAccessor);
						var hadOrderReturn = workPage.HadOrderReturn(orderModelOld.OrderId, sqlAccessor);

						if ((hadOrderReturn == false)
							&& (returnOrder != null)
							&& (orderModelOld.OrderRepaymentStatus != Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_COMPLETE)
							&& ((orderModelOld.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
								|| (orderModelOld.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP)))
						{
							// Get Return Order Info
							var orderReturnReasonValue = GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_KBN, WorkflowTypes.Order);
							var orderReturnMenoValue = GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_MEMO, WorkflowTypes.Order);
							var user = new UserService().Get(returnOrder.UserId, sqlAccessor);
							var cart = workPage.CreateCart(returnOrder, user);

							var orderNew = workPage.CreateReturnOrderNew(
								returnOrder,
								orderModelOld,
								orderReturnReasonValue,
								orderReturnMenoValue,
								cart,
								this.RegisterCardTranId,
								this.RegisterPaymentOrderId,
								this.RegisterExternalPaymentStatus,
								this.RegisterExternalPaymentAuthDate,
								this.RegisterOnlinePaymentStatus,
								this.RegisterPaymentMemo,
								sqlAccessor);

							// Process Execute External Payment
							new OrderExtend().ProcessExecuteExternalPayment(orderModelOld, orderNew, this.LoginOperatorName, sqlAccessor);

							workPage.ExecuteRegistReturnOrder(
								orderNew,
								returnOrder,
								orderModelOld,
								updateHistoryAction,
								sqlAccessor);

							if (Constants.TWINVOICE_ECPAY_ENABLED)
							{
								aruResult.ResultOrderReturnChange = workPage.ExecuteRegistTwOrderInvoiceEcPay(
									orderNew,
									orderModelOld,
									sqlAccessor);

								if (aruResult.ResultOrderReturnChange == OrderCommon.ResultKbn.UpdateNG)
								{
									aruResult.ErrorMessages.Add(
										CommerceMessages.GetMessages(CommerceMessages.ERRMSG_RETURN_ORDER_UPDATE_NG));
								}
							}
						}
						else
						{
							aruResult.ResultOrderReturnChange = OrderCommon.ResultKbn.UpdateNG;
							aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_RETURN_ORDER_UPDATE_NG));
						}
					}
					else
					{
						aruResult.ResultOrderReturnChange = OrderCommon.ResultKbn.NoUpdate;
					}

					//------------------------------------------------------
					// 注文ステータスを更新する場合
					//------------------------------------------------------
					if (this.NeedsUpdateOrderStatus)
					{
						// 商品在庫変更が行われていない OR 商品在庫変更で全ての注文商品の引当が行われた場合
						if ((aruResult.ResultProductRealStockChange == OrderCommon.ResultKbn.NoUpdate)
							|| (aruResult.ResultProductRealStockChange == OrderCommon.ResultKbn.UpdateOK))
						{
							var updateStatusCount = 0;
							var targetOrderStatus = StringUtility.ToEmpty(htOrder[Constants.FIELD_ORDER_ORDER_STATUS]);

							// 注文ステータス更新
							if (CanChangeOrderStatus(targetOrderStatus))
							{
								updateStatusCount =
									DomainFacade.Instance.OrderService.UpdateForOrderWorkflow(
										this.OrderStatusStatement,
										htOrder,
										sqlAccessor);
							}

							if (updateStatusCount <= 0)
							{
								aruResult.ResultOrderStatusChange = OrderCommon.ResultKbn.UpdateNG; // 更新NG
								aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ORDER_STATUS_UPDATE_NG));
							}

							if (Constants.FLAPS_OPTION_ENABLE)
							{
								// "ERP連携済みフラグ"をオンにする
								if ((this.OrderStatusChangeValue
										== Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_SHIP_COMP)
									|| (this.OrderStatusChangeValue
										== Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_DELIVERY_COMP))
								{
									new FlapsIntegrationFacade().TurnOnErpIntegrationFlg(
										targetOrderId,
										this.LoginOperatorName,
										sqlAccessor);
								}

								// FLAPS注文キャンセル処理実行
								if ((this.OrderStatusChangeValue
										== Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_ORDER_CANCELED)
									|| (this.OrderStatusChangeValue
										== Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_TEMP_CANCELED))
								{
									new FlapsIntegrationFacade().CancelOrder(
										targetOrderId,
										"",
										sqlAccessor);
								}
							}

							if (aruResult.ResultOrderStatusChange != OrderCommon.ResultKbn.UpdateNG)
							{
								//------------------------------------------------------
								// 仮ポイント→本ポイント更新処理（出荷完了の場合 かつ 注文本ポイント自動付与の場合）
								//------------------------------------------------------
								if (Constants.W2MP_POINT_OPTION_ENABLED)
								{
									// 出荷完了の場合 かつ 注文本ポイント自動付与 かつ 仮ポイントを持つ場合
									if ((this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_SHIP_COMP)
										&& Constants.GRANT_ORDER_POINT_AUTOMATICALLY
										&& (OrderCommon.CheckPointTypeTemp(targetUserId, targetOrderId)))
									{
										var rtn = 0;

										if (Constants.CROSS_POINT_OPTION_ENABLED)
										{
											var pointApiInput = new PointApiInput()
											{
												MemberId = targetUserId,
												OrderId = targetOrderId,
												UserCode = Constants.FLG_LASTCHANGED_BATCH
											};

											var point = new CrossPointPointApiService().Grant(pointApiInput.GetParam(PointApiInput.RequestType.Grant));
											if (point.IsSuccess == false)
											{
												var error = ErrorHelper.CreateCrossPointApiError(point.ErrorMessage, targetUserId);
												AppLogger.WriteError(error);
											}
											else
											{
												// 仮ポイント→本ポイント更新
												rtn = new PointService().TempToRealPoint(
													targetUserId,
													targetOrderId,
													this.LoginOperatorName,
													UpdateHistoryAction.DoNotInsert,
													sqlAccessor);

												var user = DomainFacade.Instance.UserService.Get(targetUserId);
												UserUtility.AdjustPointAndMemberRankByCrossPointApi(user, sqlAccessor);
											}
										}
										else
										{
											// 仮ポイント→本ポイント更新
											var sv = new PointService();
											rtn = sv.TempToRealPoint(
												targetUserId,
												targetOrderId,
												this.LoginOperatorName,
												UpdateHistoryAction.DoNotInsert,
												sqlAccessor);
										}
										if (rtn <= 0)
										{
											aruResult.ResultOrderStatusChange = OrderCommon.ResultKbn.UpdateNG;	// 更新NG
											aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_TEMP_TO_REAL_POINT_UPDATE_NG));
										}
									}
								}

								// 出荷手配済み処理
								var orderOldPaymentKbn = (string)orderOld[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN];
								if ((this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_SHIP_ARRANGED)
									&& OrderCommon.IsInvoiceBundleServiceUsable(orderOldPaymentKbn)
									&& (orderOldPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
								{
									var order = new OrderModel(orderOld[0]);
									switch (Constants.PAYMENT_CVS_DEF_KBN)
									{
										// Atodeneの場合の印字データ取得処理
										case Constants.PaymentCvsDef.Atodene:
											if (order.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF) break;

											var responseAtodene = new AtodeneGetInvoiceModelAdapter(order).Execute();
											if (responseAtodene.Result != AtodeneConst.RESULT_OK)
											{
												aruResult.ResultOrderStatusChange = OrderCommon.ResultKbn.UpdateNG;
												aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ATODENE_INVOICE_GET_NG));
												break;
											}

											// 印字データの登録
											responseAtodene.InsertInvoice(order.OrderId, sqlAccessor);
											aruResult.ResultOrderStatusChange = OrderCommon.ResultKbn.UpdateOK;
											break;

										case Constants.PaymentCvsDef.Dsk:
											if ((order.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF)
												|| (new InvoiceDskDeferredService().Get(order.OrderId, sqlAccessor) != null))
											{
												break;
											}

											var adapaterDsk = new DskDeferredGetInvoiceAdapter(order);
											var responseDsk = adapaterDsk.Execute();
											if (responseDsk.IsResultOk == false)
											{
												aruResult.ResultOrderStatusChange = OrderCommon.ResultKbn.UpdateNG;
												aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_DSK_DEFERRED_INVOICE_GET_NG));
												break;
											}

											// 印字データの登録
											adapaterDsk.InsertInvoice(order.OrderId, responseDsk);
											aruResult.ResultOrderStatusChange = OrderCommon.ResultKbn.UpdateOK;
											break;

										case Constants.PaymentCvsDef.Veritrans:
											if ((order.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF)
												|| (DomainFacade.Instance.InvoiceVeritransService.Get(order.OrderId, sqlAccessor) != null))
											{
												break;
											}

											var response = new PaymentVeritransCvsDef().GetInvoiceData(order.PaymentOrderId);

											if (response.Mstatus != VeriTransConst.RESULT_STATUS_OK)
											{
												aruResult.ResultOrderStatusChange = OrderCommon.ResultKbn.UpdateNG;
												aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_VERITRANS_INVOICE_GET_NG));
												break;
											}

											var invoiceVeritransModel = new InvoiceElement(response).CreateModel(order.OrderId, order.LastChanged);
											DomainFacade.Instance.InvoiceVeritransService.InsertUpdate(invoiceVeritransModel, sqlAccessor);
											aruResult.ResultOrderStatusChange = OrderCommon.ResultKbn.UpdateOK;
											break;

										case Constants.PaymentCvsDef.Gmo:
										case Constants.PaymentCvsDef.YamatoKa:
											break;

										case Constants.PaymentCvsDef.Score:
											var invoiceScoreService = new InvoiceScoreService();
											if ((order.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF)
												|| (invoiceScoreService.Get(order.OrderId, sqlAccessor) != null))
											{
												break;
											}

											var orderInput = new OrderInput
											{
												OrderId = order.OrderId,
												OrderPaymentKbn = order.OrderPaymentKbn,
												CardTranId = order.CardTranId,
												PaymentOrderId = order.PaymentOrderId,
												LastBilledAmount = order.LastBilledAmount.ToPriceString(),
												ExternalPaymentAuthDate = order.ExternalPaymentAuthDate.ToString(),
												OrderStatus = order.OrderStatus,
												InvoiceBundleFlg = order.InvoiceBundleFlg
											};

											var scoreErrorMessage = new ProcessAfterUpdateOrderStatus().UpdatedInvoiceByOrderStatus(
												orderInput: orderInput,
												updateStatus: Constants.StatusType.Order,
												orderStatus: Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED,
												accessor: sqlAccessor,
												lastChanged: this.LoginOperatorName);

											if (string.IsNullOrEmpty(scoreErrorMessage) == false)
											{
												aruResult.ResultOrderStatusChange = OrderCommon.ResultKbn.UpdateNG;
												aruResult.ErrorMessages.Add(scoreErrorMessage);
												break;
											}

											aruResult.ResultOrderStatusChange = OrderCommon.ResultKbn.UpdateOK;
											break;

										case Constants.PaymentCvsDef.Atobaraicom:
											if (Constants.PAYMENT_SETTING_ATOBARAICOM_USE_INVOICE_SYSTEM_SERVICE == false) break;

											var request = new InvoiceRequest.TransferPrintQueueRequest(order.PaymentOrderId, order.InvoiceBundleFlg);
											var responseApi = InvoiceApiFacade.CallApi<InvoiceResponse.TransferPrintQueueResponse>(
												Constants.PAYMENT_ATOBARAICOM_TRANSFER_PRINT_QUEUE_APIURL,
												request.BillingObject);

											if (responseApi.ResultsObject.ResultObject.ExecResult != InvoiceConstants.API_RESULT_OK)
											{
												var errorMessage = (responseApi.ResultsObject.ResultObject.ExecResult == InvoiceConstants.API_RESULT_ERROR)
													? string.Format(
														"{0}: {1}",
														responseApi.ResultsObject.ResultObject.ErrorObject.ErrorCode,
														responseApi.ResultsObject.ResultObject.ErrorObject.ErrorBody)
													: string.Empty;
												aruResult.ResultOrderStatusChange = OrderCommon.ResultKbn.UpdateNG;
												aruResult.ErrorMessages.Add(errorMessage);
												break;
											}

											// Call api get response list target invoice
											var requestQue = new InvoiceRequest.GetListTargetInvoiceRequest();
											var responseQue = InvoiceApiFacade.CallApi<InvoiceResponse.GetListTargetInvoiceResponse>(
												Constants.PAYMENT_ATOBARAICOM_GET_LIST_TARGET_INVOICE_APIURL,
												requestQue.BillingObject);

											if ((responseQue.Status == InvoiceConstants.API_RESULT_STATUS_OK) && (responseQue.ResultsObject.ResultObject != null))
											{
												// Perform insert/update invoice before export
												foreach (var item in responseQue.ResultsObject.ResultObject)
												{
													var model = new InvoiceAtobaraicomModel
													{
														OrderId = item.Ent_OrderId,
														UseAmount = int.Parse(item.UseAmount),
														TaxAmount = item.TaxAmount,
														LimitDate = DateTime.Parse(item.LimitDate),
														NameKj = item.NameKj,
														CvBarcodeData = item.Cv_BarcodeData,
														CvBarcodeString1 = item.Cv_BarcodeString1,
														CvBarcodeString2 = item.Cv_BarcodeString2,
														YuMtOcrCode1 = item.Yu_MtOcrCode1,
														YuMtOcrCode2 = item.Yu_MtOcrCode2,
														YuAccountName = item.Yu_SubscriberName,
														YuAccountNo = item.Yu_AccountNumber,
														YuLoadKind = item.Yu_ChargeClass,
														CvsCompanyName = item.Cv_ReceiptAgentName,
														CvsUserName = item.Cv_SubscriberName,
														BkCode = item.Bk_BankCode,
														BkBranchCode = item.Bk_BranchCode,
														BkName = item.Bk_BankName,
														BkBranchName = item.Bk_BranchName,
														BkAccountKind = item.Bk_DepositClass,
														BkAccountNo = item.Bk_AccountNumber,
														BkAccountName = item.Bk_AccountHolder,
														BkAccountKana = item.Bk_AccountHolderKn,
														MypagePwd = item.MypagePassword,
														MypageUrl = item.MypageUrl,
														CreditDeadline = StringUtility.ToEmpty(item.CreditLimitDate),
													};

													DomainFacade.Instance.InvoiceAtobaraicomService.InsertUpdateInvoiceAtobaraicom(model, sqlAccessor);
												}
											}

											// 請求書発行処理実行
											var requestInv = new InvoiceRequest.InvoiceProcessExecuteRequest(order.PaymentOrderId);
											var responseInv = InvoiceApiFacade.CallApi<InvoiceResponse.InvoiceProcessExecuteResponse>(Constants.PAYMENT_ATOBARAICOM_INVOICE_PROCESS_EXECUTE_APIURL, requestInv.BillingObject);

											if (responseInv.ResultsObject.ResultObject.ExecResult != InvoiceConstants.API_RESULT_OK)
											{
												var errorMessage = (responseInv.ResultsObject.ResultObject.ExecResult == InvoiceConstants.API_RESULT_ERROR)
													? string.Format(
														"{0}: {1}",
														responseInv.ResultsObject.ResultObject.ErrorObject.ErrorCode,
														responseInv.ResultsObject.ResultObject.ErrorObject.ErrorBody)
													: string.Empty;
											}
											break;
									}
								}

								//------------------------------------------------------
								// 注文情報出荷完了（or 配送完了）処理
								//------------------------------------------------------
								if ((this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_SHIP_COMP)
									|| (this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_DELIVERY_COMP))
								{
									// 定期購入情報更新
									var order = new OrderModel(orderOld[0]);
									OrderCommon.UpdateFixedPurchaseShippedCount(order, this.LoginOperatorName, UpdateHistoryAction.DoNotInsert, sqlAccessor);
								}

								//------------------------------------------------------
								// 注文キャンセル付随処理
								//------------------------------------------------------
								// 注文キャンセルまたは仮注文キャンセルの場合
								if ((this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_ORDER_CANCELED)
									|| (this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_TEMP_CANCELED))
								{
									var order = new OrderModel(orderOld[0]);
									var isSuccess = false;
									switch (order.OrderPaymentKbn)
									{
										// Cancel payment Atone by workflow
										case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
											var requestAtone = new AtoneRefundPaymentRequest()
											{
												AmountRefund = CurrencyManager.GetSettlementAmount(
													order.LastBilledAmount,
													order.SettlementRate,
													order.SettlementCurrency).ToString("0"),
												RefundReason = "キャンセル",
												DescriptionRefund = string.Empty
											};

											var atoneResponse = AtonePaymentApiFacade.RefundPayment(
												StringUtility.ToEmpty(order.CardTranId),
												requestAtone);

											if (atoneResponse.IsSuccess == false)
											{
												aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ATONE_REFUND_PAYMENT_API_NG));
												break;
											}
											if (atoneResponse.IsAuthorizationSuccess == false)
											{
												aruResult.ErrorMessages.Add(atoneResponse.AuthorizationResultNgReasonMessage);
												break;
											}

											isSuccess = true;

											OrderCommon.UpdateCancelPaymentForPaymentAtoneOrAftee(
												order,
												this.LoginOperatorName,
												sqlAccessor);
											break;

										// Cancel payment Aftee by workflow
										case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
											// Check transaction status
											var response = AfteePaymentApiFacade.GetPayment(order.CardTranId);
											// Not refund when already refund
											if (response.Refunds == null)
											{
												var requestAftee = new AfteeRefundPaymentRequest()
												{
													AmountRefund = CurrencyManager.GetSettlementAmount(
														order.LastBilledAmount,
														order.SettlementRate,
														order.SettlementCurrency).ToString("0"),
													RefundReason = "キャンセル",
													DescriptionRefund = string.Empty
												};

												var afteeResponse = AfteePaymentApiFacade.RefundPayment(
													StringUtility.ToEmpty(order.CardTranId),
													requestAftee);
												isSuccess = afteeResponse.IsSuccess;

												if (isSuccess)
												{
													OrderCommon.UpdateCancelPaymentForPaymentAtoneOrAftee(
														order,
														this.LoginOperatorName,
														sqlAccessor);
												}
												else
												{
													aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_AFTEE_REFUND_PAYMENT_API_NG));
												}
											}
											else
											{
												isSuccess = true;
											}
											break;

										// Cancel payment EcPay by workflow
										case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
											isSuccess = (order.ExternalPaymentType != Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT);
											if (isSuccess == false)
											{
												// If order is sales then call api Refund else call api cancel
												var isSaleOrder = ((order.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
													&& (order.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP));
												var request = (isSaleOrder
													? ECPayUtility.CreateRequestForCancelRefundAndCapturePayment(
														order,
														false,
														true,
														order.SettlementAmount)
													: ECPayUtility.CreateRequestForCancelRefundAndCapturePayment(order, true));
												var responseEcPay = ECPayApiFacade.CancelRefundAndCapturePayment(request);

												isSuccess = responseEcPay.IsSuccess;
												if (isSuccess)
												{
													OrderCommon.UpdateCancelPaymentForEcPayAndNewebPay(
														order,
														this.LoginOperatorName,
														sqlAccessor);
												}
												else
												{
													aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ECPAY_REFUND_PAYMENT_API_NG));
												}
											}
											break;

										// Cancel Payment NewebPay By Workflow
										case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
											isSuccess = (order.ExternalPaymentType != Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT);
											if (isSuccess == false)
											{
												// If Order Is Sales Then Call Api Refund Else Call Api Cancel
												var isSaleOrder = ((order.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
													&& (order.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP));
												var request = NewebPayUtility.CreateCancelRefundCaptureRequest(order, (isSaleOrder == false), isSaleOrder);
												var responseNewebPay = NewebPayApiFacade.SendCancelRefundAndCaptureOrder(request, true);

												isSuccess = responseNewebPay.IsSuccess;
												if (isSuccess)
												{
													OrderCommon.UpdateCancelPaymentForEcPayAndNewebPay(
														order,
														this.LoginOperatorName,
														sqlAccessor);
												}
											}
											break;

										// Cancel Payment Boku By Workflow
										case Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU:
											var refundId = string.Format("refund{0}", order.CardTranId);
											var refund = new PaymentBokuRefundChargeApi().Exec(
												null,
												order.CardTranId,
												refundId,
												BokuConstants.CONST_BOKU_REASON_CODE_GOOD_WILL,
												null,
												(order.OrderTaxIncludedFlg == Constants.FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX));

											if (refund == null)
											{
												isSuccess = false;
												aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR));
												break;
											}

											if ((refund.IsSuccess == false)
												|| (refund.RefundStatus == BokuConstants.CONST_BOKU_REFUND_STATUS_FAILED))
											{
												isSuccess = false;
												aruResult.ErrorMessages.Add(refund.Result.Message);
												break;
											}

											if (order.IsFixedPurchaseOrder == false)
											{
												var cancel = new PaymentBokuCancelOptinApi().Exec(order.PaymentOrderId);
												if ((cancel == null) || (cancel.IsSuccess == false))
												{
													isSuccess = false;
													aruResult.ErrorMessages.Add((cancel == null)
														? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR)
														: cancel.Result.Message);
													break;
												}
											}

											isSuccess = true;
											OrderCommon.UpdateCancelPaymentForBokuPay(
												order,
												this.LoginOperatorName,
												sqlAccessor);

											break;

										// Cancel payment PayPay by workflow
										case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
											if (order.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP)
											{
												isSuccess = true;
												break;
											}
											var orderService = new OrderService();
											var isSaleOrderPaypay = ((order.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
												&& (order.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP));
											var isSuccessSale = true;

											switch (Constants.PAYMENT_PAYPAY_KBN)
											{
												case Constants.PaymentPayPayKbn.SBPS:
													if (isSaleOrderPaypay == false)
													{
														var responseSale = new PaymentSBPSPaypaySaleApi().Exec(order.CardTranId, order.OrderPriceTotal);
														isSuccessSale = responseSale;

														if (responseSale)
														{
															// 決済連携メモ更新
															var paymentMemo = OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
																string.IsNullOrEmpty(order.PaymentOrderId)
																	? order.OrderId
																	: order.PaymentOrderId,
																order.OrderPaymentKbn,
																order.CardTranId,
																"売上確定",
																order.LastBilledAmount);
															orderService.AddPaymentMemo(
																order.OrderId,
																paymentMemo,
																this.LoginOperatorName,
																UpdateHistoryAction.DoNotInsert,
																sqlAccessor);
														}
													}

													if (isSuccessSale)
													{
														var responseCancel = new PaymentSBPSPaypayCancelApi();
														isSuccess = responseCancel.Exec(order.CardTranId, order.OrderPriceTotal);
													}

													if (isSuccess)
													{
														OrderCommon.UpdateCancelPayment(
															order,
															this.LoginOperatorName,
															sqlAccessor);
													}
													else
													{
														aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_PAYPAY_REFUND_PAYMENT_API_NG));
													}
													break;

												case Constants.PaymentPayPayKbn.GMO:
													var paypayFacade = new PaypayGmoFacade();

													if (isSaleOrderPaypay == false)
													{
														var responseSale = paypayFacade.CapturePayment(order);
														isSuccessSale = (responseSale.Result == Results.Success);
														if (responseSale.Result == Results.Success)
														{
															// 決済連携メモ更新
															var paymentMemo = OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
																string.IsNullOrEmpty(order.PaymentOrderId)
																	? order.OrderId
																	: order.PaymentOrderId,
																order.OrderPaymentKbn,
																order.CardTranId,
																"売上確定",
																order.LastBilledAmount);
															orderService.AddPaymentMemo(
																order.OrderId,
																paymentMemo,
																this.LoginOperatorName,
																UpdateHistoryAction.DoNotInsert,
																sqlAccessor);
														}
													}

													if (isSuccessSale)
													{
														var responseCancel = paypayFacade.RefundPayment(order, order.LastBilledAmount);
														isSuccess = (responseCancel.Result == Results.Success);
													}

													if (isSuccess)
													{
														OrderCommon.UpdateCancelPayment(
															order,
															this.LoginOperatorName,
															sqlAccessor);
													}
													else
													{
														aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_PAYPAY_REFUND_PAYMENT_API_NG));
													}
													break;

												case Constants.PaymentPayPayKbn.VeriTrans:
													var paymentVeritransPaypayApi = new PaymentVeritransPaypay();
													var veriTransResult = isSaleOrderPaypay
														? (IResponseDto)paymentVeritransPaypayApi.Refund(order.PaymentOrderId, order.LastBilledAmount)
														: (IResponseDto)paymentVeritransPaypayApi.Cancel(order.PaymentOrderId);

													if (veriTransResult.Mstatus == VeriTransConst.RESULT_STATUS_OK)
													{
														isSuccess = true;
														OrderCommon.UpdateCancelPayment(
															order,
															this.LoginOperatorName,
															sqlAccessor);
														break;
													}
													aruResult.ErrorMessages.Add(
														CommerceMessages.GetMessages(CommerceMessages.ERRMSG_PAYPAY_REFUND_PAYMENT_API_NG));
													break;
											}
											break;

										// Gmo掛け払い注文キャンセル
										case Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO:
										case Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE:
											// Api確認
											var transactionAPi = new GmoTransactionApi();
											var requestGmo = new Payment.GMO.TransactionModifyCancel.GmoRequestTransactionModifyCancel(order);
											requestGmo.KindInfo.UpdateKind = UpdateKindType.OrderCancel;
											var responseGmo = transactionAPi.TransactionModifyCancel(requestGmo);
											// GmoレスポンスがNGの場合エラーメッセージ取得
											if (responseGmo.Result == ResultCode.NG)
											{
												aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ORDER_STATUS_UPDATE_NG));
											}
											else
											{
												isSuccess = true;
											}
											break;

										// コンビニ後払いキャンセル
										case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:

											// スコア後払い
											if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score)
											{
												// 連携していない受注は成功とする
												if (string.IsNullOrEmpty(order.CardTranId))
												{
													isSuccess = true;
													break;
												}

												var cancelRequest = new ScoreCancelRequest(order);
												var cancelResponse = new ScoreApiFacade().OrderCancel(cancelRequest);

												// NGの場合はエラーメッセージを表示
												if (cancelResponse.Result == ScoreResult.Ng.ToText())
												{
													aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ORDER_STATUS_UPDATE_NG));
												}
												else
												{
													isSuccess = true;
													OrderCommon.UpdateCancelPayment(
														order: order,
														lastChanged: this.LoginOperatorName,
														sqlAccessor: sqlAccessor);
												}
											}
											// ベリトランス後払い
											else if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Veritrans)
											{
												var result = new PaymentVeritransCvsDef().OrderCancel(order.PaymentOrderId);
												if (result.Mstatus == VeriTransConst.RESULT_STATUS_OK)
												{
													isSuccess = true;
													OrderCommon.UpdateCancelPayment(
														order: order,
														lastChanged: this.LoginOperatorName,
														sqlAccessor: sqlAccessor);
													break;
												}
												aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ORDER_STATUS_UPDATE_NG));
											}
											break;

										// GMOアトカラ注文キャンセル
										case Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA:
											var cancelApi = new PaymentGmoAtokaraCancelApi();
											var cancelOrder = new OrderService().Get(order.OrderId, sqlAccessor);
											var apiResult = cancelApi.Exec(PaymentGmoAtokaraTypes.UpdateKind.Cancel, cancelOrder);

											if (apiResult == false)
											{
												aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ORDER_STATUS_UPDATE_NG));
											}
											else
											{
												isSuccess = true;
											}
											break;

										default:
											isSuccess = true;
											break;
									}

									if (isSuccess == false)
									{
										aruResult.ResultOrderStatusChange = OrderCommon.ResultKbn.UpdateNG;
										aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ORDER_STATUS_UPDATE_NG));
									}
									else
									{
										OrderCommon.CancelOrder(
											orderOld[0],
											false,
											Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER_CANCEL,
											this.LoginOperatorDeptId,
											this.LoginOperatorName,
											UpdateHistoryAction.DoNotInsert,
											sqlAccessor);
									}
								}

								//------------------------------------------------------
								// シリアルキーステータス更新処理（注文ステータス更新時）
								// 注文ステータス「出荷完了」「配送完了」かつ入金ステータス「入金済み」であればキーを引き渡す
								//------------------------------------------------------
								if (((this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_SHIP_COMP) || (this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_DELIVERY_COMP))
									&& ((string)htOrder[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE))
								{
									new SerialKeyService().DeliverSerialKey(
										(string)htOrder[Constants.FIELD_ORDER_ORDER_ID],
										(string)htOrder[Constants.FIELD_ORDER_LAST_CHANGED],
										sqlAccessor);
								}
							}
						}
						// 商品在庫変更が未引当・一部引当の場合、注文ステータスの更新は行わない
						// 注文ステータス更新が行えない = 更新NGとする
						else
						{
							aruResult.ResultOrderStatusChange = OrderCommon.ResultKbn.UpdateNG;
							aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ORDER_STATUS_UPDATE_NG));
						}
					}
					else
					{
						// 更新処理を行わない場合
						aruResult.ResultOrderStatusChange = OrderCommon.ResultKbn.NoUpdate;
					}

					//------------------------------------------------------
					// 入金ステータスを更新する場合
					//------------------------------------------------------
					if (this.NeedsUpdatePaymentStatus)
					{
						// 入金ステータス更新
						var updateStatusCount = new OrderService()
							.UpdateForOrderWorkflow(this.PaymentStatusStatement, htOrder, sqlAccessor);
						if (updateStatusCount <= 0)
						{
							aruResult.ResultPaymentStatusChange = OrderCommon.ResultKbn.UpdateNG; // 更新NG
							aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_PAYMENT_STATUS_UPDATE_NG));
						}

						if (aruResult.ResultPaymentStatusChange != OrderCommon.ResultKbn.UpdateNG)
						{
							//------------------------------------------------------
							// シリアルキーステータス更新処理（入金ステータス更新時）
							// 注文ステータス「出荷完了」「配送完了」かつ入金ステータス「入金済み」であればキーを引き渡す
							//------------------------------------------------------
							if ((this.PaymentStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE_COMPLETE)
								&& (((string)htOrder[Constants.FIELD_ORDER_ORDER_STATUS] == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
									|| (string)htOrder[Constants.FIELD_ORDER_ORDER_STATUS] == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP))
							{
								new SerialKeyService().DeliverSerialKey(
									(string)htOrder[Constants.FIELD_ORDER_ORDER_ID],
									(string)htOrder[Constants.FIELD_ORDER_LAST_CHANGED],
									sqlAccessor);
							}

							// Update FixPurchaseMemberFlg By Settings
							var order = new OrderService().Get(targetOrderId, sqlAccessor);
							if ((this.PaymentStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE_COMPLETE)
								&& Constants.FIXEDPURCHASE_MEMBER_CONDITION_INCLUDES_ORDER_PAYMENT_STATUS_COMPLETE
								&& order.IsFixedPurchaseOrder
								&& (aruResult.ResultPaymentStatusChange == OrderCommon.ResultKbn.UpdateOK))
							{
								new UserService().UpdateFixedPurchaseMemberFlg(
									targetUserId,
									Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON,
									Constants.FLG_LASTCHANGED_BATCH,
									UpdateHistoryAction.DoNotInsert,
									sqlAccessor);
							}
						}
					}
					// 更新処理を行わない場合
					else
					{
						aruResult.ResultPaymentStatusChange = OrderCommon.ResultKbn.NoUpdate;
					}

					//------------------------------------------------------
					// 督促ステータスを更新する場合
					//------------------------------------------------------
					if (this.NeedsUpdateDemandStatus)
					{
						// 督促ステータス更新
						htOrder.Add(Constants.FIELD_ORDER_DEMAND_STATUS, this.DemandStatusChangeValue);
						var updateStatusCount = new OrderService()
							.UpdateForOrderWorkflow(this.DemandStatusStatement, htOrder, sqlAccessor);
						if (updateStatusCount <= 0)
						{
							aruResult.ResultDemandStatusChange = OrderCommon.ResultKbn.UpdateNG; // 更新NG
							aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_DEMAND_STATUS_UPDATE_NG));
						}
					}
					// 更新処理を行わない場合
					else
					{
						aruResult.ResultDemandStatusChange = OrderCommon.ResultKbn.NoUpdate;
					}

					// 領収書出力フラグを更新する場合
					if (Constants.RECEIPT_OPTION_ENABLED && this.NeedsUpdateReceiptOutputFlg)
					{
						htOrder[Constants.FIELD_ORDER_RECEIPT_OUTPUT_FLG] = this.ReceiptOutputFlgChangeValue;
						var updateStatusCount = new OrderService().UpdateForOrderWorkflow(
							this.ReceiptOutputFlgStatement,
							htOrder,
							sqlAccessor);
						if (updateStatusCount <= 0)
						{
							aruResult.ResultReceiptOutputFlgChange = OrderCommon.ResultKbn.UpdateNG; // 更新NG
							aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_RECEIPT_OUTPUT_FLG_UPDATE_NG));
						}
					}
					else
					{
						aruResult.ResultReceiptOutputFlgChange = OrderCommon.ResultKbn.NoUpdate;
					}

					// Call Api Convert Status In Server
					if (this.NeedsCallApiInvoiceStatus)
					{
						var isOrderReturnExchange = (targetOrderId.Split('-').Length > 1);
						var orderId = isOrderReturnExchange
							? targetOrderId.Split('-')[0]
							: targetOrderId;

						var orderInvoice = new TwOrderInvoiceService().GetOrderInvoice(
							targetOrderId,
							int.Parse(StringUtility.ToEmpty(orderOld[0][Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO])));
						var orderStatus = StringUtility.ToEmpty(orderOld[0][Constants.FIELD_ORDER_ORDER_STATUS]);

						var success = false;
						switch (this.ApiInvoiceStatusValue)
						{
							case Constants.FLG_ORDER_INVOICE_STATUS_ISSUED:
								if ((orderInvoice != null)
									&& (orderInvoice.TwInvoiceStatus == Constants.FLG_ORDER_INVOICE_STATUS_NOT_ISSUED)
									&& (isOrderReturnExchange == false)
									&& (orderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
									&& (orderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED))
								{
									success = OrderCommon.InvoiceReleased(htOrder);
									if (success)
									{
										new TwOrderInvoiceService().UpdateTwOrderInvoiceStatus(
											orderInvoice.OrderId,
											orderInvoice.OrderShippingNo,
											Constants.FLG_ORDER_INVOICE_STATUS_ISSUED,
											StringUtility.ToEmpty(htOrder[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_NO]),
											sqlAccessor);
									}
								}
								break;

							case Constants.FLG_ORDER_INVOICE_STATUS_ISSUED_LINKED:
								if ((orderInvoice != null)
									&& (orderInvoice.TwInvoiceStatus == Constants.FLG_ORDER_INVOICE_STATUS_ISSUED))
								{
									orderInvoice.TwInvoiceDate = orderInvoice.TwInvoiceDate.HasValue
										? orderInvoice.TwInvoiceDate
										: DateTime.Now;

									var response = new TwInvoiceApi().InvoiceRelease(orderInvoice, orderOld);
									success = response.IsSuccess;
									if (success)
									{
										orderInvoice.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_ISSUED_LINKED;
										new TwOrderInvoiceService().UpdateTwOrderInvoiceForModify(
											orderInvoice,
											this.LoginOperatorName,
											updateHistoryAction,
											sqlAccessor);
									}
								}
								break;

							case Constants.FLG_ORDER_INVOICE_STATUS_REFUND:
								TwInvoiceResponse responseReturn = null;
								if (isOrderReturnExchange
									|| (orderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED))
								{
									orderInvoice.TwInvoiceDate = orderInvoice.TwInvoiceDate.HasValue
										? orderInvoice.TwInvoiceDate
										: DateTime.Now;

									responseReturn = new TwInvoiceApi().InvoiceReturn(
										orderInvoice,
										orderOld,
										isOrderReturnExchange);

									success = responseReturn.IsSuccess;
									if (success)
									{
										orderInvoice.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_REFUND_COMPLETED;
										new TwOrderInvoiceService().UpdateTwOrderInvoiceForModify(
											orderInvoice,
											this.LoginOperatorName,
											updateHistoryAction,
											sqlAccessor);
									}
								}
								else
								{
									var orderRelate = new OrderService().GetRelatedOrdersDataView(orderId);
									foreach (DataRowView orderInfo in orderRelate)
									{
										if (StringUtility.ToEmpty(orderInfo[Constants.FIELD_ORDER_ORDER_ID]) == orderId) continue;

										orderInvoice = new TwOrderInvoiceService().GetOrderInvoice(
											StringUtility.ToEmpty(orderInfo[Constants.FIELD_ORDER_ORDER_ID]),
											int.Parse(StringUtility.ToEmpty(orderInfo[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO])),
											sqlAccessor);

										if (orderInvoice.TwInvoiceStatus == Constants.FLG_ORDER_INVOICE_STATUS_REFUND_COMPLETED) continue;

										orderInvoice.TwInvoiceDate = orderInvoice.TwInvoiceDate.HasValue
											? orderInvoice.TwInvoiceDate
											: DateTime.Now;

										responseReturn = new TwInvoiceApi().InvoiceReturn(
											orderInvoice,
											orderInfo,
											true);
										success = responseReturn.IsSuccess;
										if (success)
										{
											orderInvoice.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_REFUND_COMPLETED;
											new TwOrderInvoiceService().UpdateTwOrderInvoiceForModify(
												orderInvoice,
												this.LoginOperatorName,
												updateHistoryAction,
												sqlAccessor);
										}
									}
								}
								break;
						}

						if (success == false)
						{
							aruResult.ResultOrderInvoiveApiChange = OrderCommon.ResultKbn.UpdateNG;
							aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ORDER_INVOICE_API_NG));
						}
					}
					else
					{
						aruResult.ResultOrderInvoiveApiChange = OrderCommon.ResultKbn.NoUpdate;
					}

					// Update Order Invoice Status In DataBase
					if (this.NeedsUpdateOrderInvoiceStatus)
					{
						var orderInvoice = new TwOrderInvoiceService().GetOrderInvoice(
							targetOrderId,
							int.Parse(StringUtility.ToEmpty(orderOld[0][Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO])),
							sqlAccessor);
						if ((orderInvoice != null)
							&& (orderInvoice.TwInvoiceStatus != this.OrderInvoiceStatusChangeValue))
						{
							orderInvoice.TwInvoiceStatus = this.OrderInvoiceStatusChangeValue;
							var result = new TwOrderInvoiceService().UpdateTwOrderInvoiceForModify(
								orderInvoice,
								this.LoginOperatorName,
								updateHistoryAction,
								sqlAccessor);

							if (result == 0)
							{
								aruResult.ResultOrderInvoiveStatusChange = OrderCommon.ResultKbn.UpdateNG;
								aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ORDER_INVOICE_STATUS_UPDATE_NG));
							}
						}
					}
					else
					{
						aruResult.ResultOrderInvoiveStatusChange = OrderCommon.ResultKbn.NoUpdate;
					}

					// For case has external order information action
					if (this.NeedsExecExternalOrderInfoAction
						&& (this.ExternalOrderInfoActionValue != Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_CROSSMALL_UPDATE_STATUS))
					{
						var orderInfo
							= new OrderService().GetOrderInfoByOrderId(
								targetOrderId,
								sqlAccessor,
								(this.ExternalOrderInfoActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_RECUSTOMER));

						var errorMessage = string.Empty;

						switch (this.ExternalOrderInfoActionValue)
						{
							case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_ECPAY:
								errorMessage = ExecExternalOrderInfo(
									orderInfo,
									updateHistoryAction,
									sqlAccessor);
								break;

							case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_NEXTENGINE:
								errorMessage = ExecSaveOrderForNextEngine(orderInfo);
								break;

							case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_RECUSTOMER:
								var shippedDate
									= (string.IsNullOrEmpty(StringUtility.ToEmpty(orderInfo.OrderShippedDate)) == false)
										? orderInfo.OrderShippedDate.ToString()
										: DateTime.Now.ToString();
								errorMessage = ExecOrderCooperationRecustomer(
									orderInfo,
									(string)htOrder[Constants.FIELD_ORDER_ORDER_STATUS],
									shippedDate,
									sqlAccessor);
								break;

							default:
								break;
						}

						if (string.IsNullOrEmpty(errorMessage))
						{
							aruResult.ResultExternalOrderInfoAction = OrderCommon.ResultKbn.UpdateOK;
						}
						else
						{
							aruResult.ResultExternalOrderInfoAction = OrderCommon.ResultKbn.UpdateNG;
							aruResult.ExternalOrderInfoErrorMessage = errorMessage;
							aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_EXTERNAL_ORDER_INFO_COOPERATION_NG));
						}
					}
					else if(this.NeedsExecExternalOrderInfoAction
						&& (this.ExternalOrderInfoActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_CROSSMALL_UPDATE_STATUS))
					{
						// エラーある場合、ステータス更新しないが両方ともNGにする
						if(string.IsNullOrEmpty(crossMallGetApiErrorMessage) == false)
						{
							aruResult.ResultExternalOrderInfoAction = OrderCommon.ResultKbn.UpdateNG;
							aruResult.ResultOrderStatusChange = OrderCommon.ResultKbn.UpdateNG;
						}
						else
						{
							aruResult.ResultExternalOrderInfoAction = OrderCommon.ResultKbn.UpdateOK;
						}
					}
					else
					{
						aruResult.ResultExternalOrderInfoAction = OrderCommon.ResultKbn.NoUpdate;
					}

					//------------------------------------------------------
					// 注文拡張ステータス１～３０を更新する場合(※利用数のみ)
					//------------------------------------------------------
					for (int index = 0; index < Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; index++)
					{
						if (this.NeedsUpdateOrderExtendStatus[index])
						{
							// 注文拡張ステータス更新
							int updatedCount = new OrderService().UpdateOrderExtendStatus(
								targetOrderId,
								index + 1,
								this.OrderExtendStatusChangeValues[index],
								DateTime.Parse((string)htOrder["update_date"]),
								(string)htOrder[Constants.FIELD_ORDER_LAST_CHANGED],
								UpdateHistoryAction.DoNotInsert,
								sqlAccessor);

							if (updatedCount <= 0)
							{
								aruResult.ResultOrderExtendStatusChange[index] = OrderCommon.ResultKbn.UpdateNG;	// 更新NG
								aruResult.ErrorMessages.Add(
									string.Format(
										CommerceMessages.GetMessages(
											CommerceMessages.ERRMSG_ORDER_EXTEND_STATUS_UPDATE_NG),
										index + 1));
							}
						}
						else
						{
							aruResult.ResultOrderExtendStatusChange[index] = OrderCommon.ResultKbn.NoUpdate;
						}
					}

					//------------------------------------------------------
					// 返金ステータスを更新する場合
					//------------------------------------------------------
					if (this.NeedsUpdateRepaymentStatus)
					{
						var updateStatusCount = new OrderService().UpdateForOrderWorkflow(this.RepaymentStatusStatement, htOrder, sqlAccessor);
						if (updateStatusCount <= 0)
						{
							aruResult.ResultRepaymentStatusChange = OrderCommon.ResultKbn.UpdateNG;
							aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_REPAYMENT_STATUS_UPDATE_NG));
						}
					}
					// 更新処理を行わない場合
					else
					{
						aruResult.ResultRepaymentStatusChange = OrderCommon.ResultKbn.NoUpdate;
					}

					//------------------------------------------------------
					// 返品交換ステータスを更新する場合
					//------------------------------------------------------
					if (this.NeedsUpdateReturnExchangeStatus)
					{
						// 返品交換区分が「交換」でない OR 返金区分が「未返金」でない OR  返品交換ステータスが「返品交換完了」に更新されないの場合はステータス更新
						DataRowView drvOrder = OrderCommon.GetOrder(targetOrderId, sqlAccessor)[0];
						if (((string)drvOrder[Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN] != Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
							|| ((string)drvOrder[Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS] != Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_CONFIRM)
							|| (this.ReturnExchangeStatusChangeValue != Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE))
						{
							// 返品交換ステータス更新
							var updateStatusCount = new OrderService()
								.UpdateForOrderWorkflow(this.ReturnExchangeStatusStatement, htOrder, sqlAccessor);
							if (updateStatusCount <= 0)
							{
								aruResult.ResultReturnExchangeStatusChange = OrderCommon.ResultKbn.UpdateNG;
							}
						}
						else
						{
							aruResult.ResultReturnExchangeStatusChange = OrderCommon.ResultKbn.UpdateNG;	// 処理エラーとする
						}

						//------------------------------------------------------
						// 返品交換区分が「交換」、返金区分が「返金無し」「返金済み」の注文の返品交換ステータスが「返品交換完了」に更新された場合
						// ※交換注文を通常注文に変更する。
						//   具体的には、
						//   ・注文ステータスを「指定無し」→「注文済み」
						//   ・注文日時を「更新日時」★更新日指定の日時ではない！
						//   ・入金ステータスを「指定無し」→「入金確認待ち」
						//   ・督促ステータスを「指定無し」→「督促なし」
						//   に更新する。
						//------------------------------------------------------
						drvOrder = OrderCommon.GetOrder(targetOrderId, sqlAccessor)[0];
						if ((string)drvOrder[Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN] == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE
							&& ((string)drvOrder[Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS] == Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_NOREPAYMENT
								|| (string)drvOrder[Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS] == Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_COMPLETE)
							&& (string)drvOrder[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS] == Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE)
						{
							var updateStatusCount = new OrderService().UpdateForOrderWorkflow("UpdateOrderExchange", htOrder, sqlAccessor);
							if (updateStatusCount <= 0)
							{
								aruResult.ResultReturnExchangeStatusChange = OrderCommon.ResultKbn.UpdateNG;
							}
						}

						// 返品交換区分が「返品」かつ返品交換ステータスが「返品交換完了」に更新された場合、
						// 全返品チェックを行い、全返品であれば定期購入情報「購入回数（注文・出荷基準）」を減算する
						if (((string)drvOrder[Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN] == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN)
							&& (this.ReturnExchangeStatusChangeValue == Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE))
						{
							// 定期購入OP有効?
							if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
							{
								var orderId = (string)drvOrder[Constants.FIELD_ORDER_ORDER_ID];
								var orderIdOrg = (string)drvOrder[Constants.FIELD_ORDER_ORDER_ID_ORG];

								// 注文（返品交換含む）取得
								var service = new OrderService();
								var relatedOrders = service.GetRelatedOrders(orderIdOrg, sqlAccessor);

								// 元注文取得
								var orderOrg = relatedOrders.First(o => o.IsOriginalOrder);
								// 返品注文取得
								var returnOrder = relatedOrders.First(item => (item.IsOriginalOrder == false));

								// 定期購入注文?
								if (orderOrg.FixedPurchaseId != "")
								{
									// 本注文の返品交換ステータスを「返品交換完了」にセット
									var order = relatedOrders.First(o => o.OrderId == orderId);
									order.OrderReturnExchangeStatus = Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE;

									var fixedPurchaseServer = new FixedPurchaseService();
									foreach (var item in returnOrder.Shippings[0].Items)
									{
										fixedPurchaseServer.UpdateForReturnOrderItem(
											orderOrg.FixedPurchaseId,
											orderOrg.OrderId,
											item.VariationId,
											this.LoginOperatorName,
											UpdateHistoryAction.DoNotInsert,
											sqlAccessor);
									}

									// 注文商品が全て返品されているか？
									if (service.InspectReturnAllItems(relatedOrders, sqlAccessor))
									{
										// 定期購入：注文返品更新
										fixedPurchaseServer.UpdateForReturnOrder(
											orderOrg.FixedPurchaseId,
											orderOrg.OrderId,
											this.LoginOperatorName,
											UpdateHistoryAction.DoNotInsert,
											sqlAccessor);
									}
								}
							}
						}

						if (aruResult.ResultReturnExchangeStatusChange == OrderCommon.ResultKbn.UpdateNG)
						{
							aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_RETURN_EXCHANGE_STATUS_UPDATE_NG));
						}
					}
					// 更新処理を行わない場合
					else
					{
						aruResult.ResultReturnExchangeStatusChange = OrderCommon.ResultKbn.NoUpdate;
					}

					if ((StringUtility.ToEmpty(htOrder[Constants.FIELD_ORDER_ORDER_STATUS])
							== Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
						&& this.NeedsUpdateStorePickupStatus)
					{
						var updateStatusCount = 0;
						if (CanChangeStorePickupStatus(targetOrderId, sqlAccessor))
						{
							updateStatusCount = DomainFacade.Instance.OrderService
							.UpdateStorePickupSatus(
								orderId: targetOrderId,
								storePickupStatus: this.StorePickupStatusChangeValue,
								lastChanged: this.LoginOperatorName,
								updateHistoryAction: UpdateHistoryAction.DoNotInsert,
								accessor: sqlAccessor);
						}

						aruResult.ResultStorePickupStatusChange = (updateStatusCount > 0)
							? OrderCommon.ResultKbn.UpdateOK
							: OrderCommon.ResultKbn.UpdateNG;
					}
					else
					{
						aruResult.ResultStorePickupStatusChange = OrderCommon.ResultKbn.NoUpdate;
					}

					// 結果格納（全て成功しているか）
					// CrossMallの外部連携失敗の場合は他のアクションに影響させないため、最終的な結果から除外する
					bool blResult = ((aruResult.ResultOrderStatusChange != OrderCommon.ResultKbn.UpdateNG)
							|| (aruResult.ResultProductRealStockChange == OrderCommon.ResultKbn.UpdatePart)
							|| (aruResult.ResultProductRealStockChange == OrderCommon.ResultKbn.UpdateOK)
							|| ((aruResult.ResultOrderStatusChange == OrderCommon.ResultKbn.UpdateNG)
								&& (aruResult.ResultExternalOrderInfoAction == OrderCommon.ResultKbn.UpdateNG)
								&& (this.ExternalOrderInfoActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_CROSSMALL_UPDATE_STATUS)))
						&& (aruResult.ResultProductRealStockChange != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultPaymentStatusChange != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultDemandStatusChange != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultReturnExchangeStatusChange != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultRepaymentStatusChange != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultReceiptOutputFlgChange != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultOrderInvoiveStatusChange != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultOrderInvoiveApiChange != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultExternalOrderInfoAction != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultStorePickupStatusChange != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultExternalOrderInfoAction != OrderCommon.ResultKbn.UpdateNG
							|| ((aruResult.ResultExternalOrderInfoAction == OrderCommon.ResultKbn.UpdateNG)
								&& (this.ExternalOrderInfoActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_CROSSMALL_UPDATE_STATUS)));

					// Get Update Result For Call API
					var resultInvoiceApi = (aruResult.ResultOrderInvoiveStatusChange != OrderCommon.ResultKbn.UpdateNG)
							&& (aruResult.ResultOrderInvoiveApiChange != OrderCommon.ResultKbn.UpdateNG)
							&& (aruResult.ResultOrderInvoiveStatusChange != OrderCommon.ResultKbn.NoUpdate);

					foreach (OrderCommon.ResultKbn rkResultKbn in aruResult.ResultOrderExtendStatusChange)
					{
						blResult &= (rkResultKbn != OrderCommon.ResultKbn.UpdateNG);
					}

					var isUpdateOnlinePaymentStatusForPaypay = ((this.ExternalPaymentActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYPAY_PAYMENT)
						&& this.NeedsUpdateOrderStatus
						&& ((this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_ORDER_CANCELED)
							|| (this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_TEMP_CANCELED)));

					//------------------------------------------------------
					// 外部決済（全て成功していたら）
					//------------------------------------------------------
					// 外部決済の前にオンライン決済ステータスを更新
					if (CanUpdateExternalPaymentStatusForSalesOrShipment(blResult, true)
						&& (isUpdateOnlinePaymentStatusForPaypay == false))
					{
						blResult = (new OrderService().UpdateOnlinePaymentStatus(
							(string)htOrder[Constants.FIELD_ORDER_ORDER_ID],
							Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
							this.LoginOperatorName,
							UpdateHistoryAction.DoNotInsert,
							sqlAccessor) > 0);
						if (blResult == false) aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ONLINE_PAYMENT_STATUS_UPDATE_NG));
					}

					if (resultInvoiceApi) hasInvoiceUpdateOK = true;

					if (blResult)
					{
						blResult = ExecExternalPayment(
							htOrder,
							targetOrderId,
							aruResult,
							IsDigitalContents,
							UpdateHistoryAction.DoNotInsert,
							sqlAccessor);
					}

					// 外部決済連携エラーの場合、エラーメッセージ追加
					if (aruResult.ResultExternalPaymentAction == OrderCommon.ResultKbn.UpdateNG)
					{
						aruResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_EXTERNAL_PAYMENT_COOPERATION_NG));
					}

					// 外部決済後に外部決済ステータス・決済連携メモ更新
					// With Payment EcPay or NewebPay or PayPay : If ResultExternalPaymentAction is NoUpdate Then No Update ExternalPaymentStatus
					var isExternalPaymentAction = ((aruResult.ResultExternalPaymentAction == OrderCommon.ResultKbn.NoUpdate)
						&& ((this.ExternalPaymentActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_EC_PAYMENT)
							|| (this.ExternalPaymentActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_NEWEB_PAYMENT)
							|| (this.ExternalPaymentActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYPAY_PAYMENT)));
					if (CanUpdateExternalPaymentStatusForSalesOrShipment(
						blResult,
						(isExternalPaymentAction == false)))
					{
						var orderService = new OrderService();
						var order = new OrderModel(
							OrderCommon.GetOrder((string)htOrder[Constants.FIELD_ORDER_ORDER_ID], sqlAccessor)[0]);

						if (Constants.EXTERNAL_PAYMENT_STATUS_SHIPCOMP_ORDERWORKFLOW_EXTERNALSHIPMENTACTION
								&& ((this.ExternalPaymentActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CVS_DEF_SHIP)
									|| (this.ExternalPaymentActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_TRILINK_AFTERPAY_SHIP)
									|| (this.ExternalPaymentActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATODENE_CVS_DEF_SHIP)
									|| (this.ExternalPaymentActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_NP_AFTERPAY_SHIP)
									|| (this.ExternalPaymentActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATOBARAICOM_CVS_DEF_SHIP)
									|| (this.ExternalPaymentActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SCORE_CVS_DEF_SHIP)
									|| (this.ExternalPaymentActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_VERITRANS_CVS_DEF_SHIP)))
						{
							// 出荷報告の場合は外部決済ステータスを「出荷報告済み」にする
							orderService.UpdateExternalPaymentStatusShipmentComplete(
								order.OrderId,
								this.LoginOperatorName,
								UpdateHistoryAction.DoNotInsert,
								sqlAccessor);
						}
						else
						{
							// 外部決済ステータス更新
							orderService.UpdateExternalPaymentStatusSalesComplete(
								order.OrderId,
								this.LoginOperatorName,
								UpdateHistoryAction.DoNotInsert,
								sqlAccessor);

							// Convert Amount Write Payment Memo When Payment With Atone Or Aftee
							var lastBilledAmount = order.LastBilledAmount;
							switch (order.OrderPaymentKbn)
							{
								case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
								case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
								case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
								case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
									lastBilledAmount = CurrencyManager.GetSettlementAmount(
										lastBilledAmount,
										order.SettlementRate,
										order.SettlementCurrency);
									break;
							}

							// 決済連携メモ更新
							var paymentMemo = OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
								string.IsNullOrEmpty(order.PaymentOrderId)
									? order.OrderId
									: order.PaymentOrderId,
								order.OrderPaymentKbn,
								order.CardTranId,
								"売上確定",
								lastBilledAmount);
							orderService.AddPaymentMemo(
								order.OrderId,
								paymentMemo,
								this.LoginOperatorName,
								UpdateHistoryAction.DoNotInsert,
								sqlAccessor);
						}
					}

					//------------------------------------------------------
					// コミット/ロールバック
					//------------------------------------------------------
					if (blResult
						|| resultInvoiceApi)
					{
						// 更新履歴登録
						if (updateHistoryAction == UpdateHistoryAction.Insert)
						{
							new UpdateHistoryService().InsertAllForOrder(
								(string)htOrder[Constants.FIELD_ORDER_ORDER_ID],
								this.LoginOperatorName,
								sqlAccessor);
						}

						// Write history complete
						history.HistoryComplete();

						if (this.NeedsExecExternalPaymentAction
							&& (isExternalPaymentAction == false))
						{
							OrderCommon.AppendExternalPaymentCooperationLog(
							true,
							targetOrderId,
							this.SuccessMessage ?? "決済処理成功",
							this.LoginOperatorName,
							UpdateHistoryAction.Insert,
							sqlAccessor);
						}

						// 更新処理確定
						sqlAccessor.CommitTransaction();
					}
					else
					{
						// 更新処理ロールバック
						sqlAccessor.RollbackTransaction();

						if (this.NeedsExecExternalPaymentAction)
						{
							OrderCommon.AppendExternalPaymentCooperationLog(
							false,
							targetOrderId,
							this.ApiErrorMessage ?? "決済エラー",
							this.LoginOperatorName,
							UpdateHistoryAction.Insert);
						}

						// Recustomer連携の場合、失敗時にも連携メモを残す
						if (this.ExternalOrderInfoActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_RECUSTOMER)
						{
							var order = DomainFacade.Instance.OrderService.Get(targetOrderId, sqlAccessor);
							DomainFacade.Instance.OrderService.AppendRelationMemo(
								order.OrderId,
								((string.IsNullOrEmpty(order.RelationMemo) ? string.Empty : "\r\n") + aruResult.ExternalOrderInfoErrorMessage),
								this.LoginOperatorName,
								UpdateHistoryAction.Insert);
						}

						// 処理結果を全て更新NGにする
						aruResult.Reset(OrderCommon.ResultKbn.UpdateNG, hasInvoiceUpdateOK);
					}
				}
				catch (Exception ex)
				{
					// ログに書き込み
					AppLogger.WriteError("ワークフローのステータス更新部分で例外発生：注文ID=" + targetOrderId, ex);

					// 更新処理ロールバック
					sqlAccessor.RollbackTransaction();

					// 処理結果を全て更新NGにする
					aruResult.Reset(OrderCommon.ResultKbn.UpdateNG, hasInvoiceUpdateOK);
				}
			}
			else
			{
				// 全てのステータスが更新対象外のための結果表示
				aruResult.Reset(OrderCommon.ResultKbn.NoUpdate, hasInvoiceUpdateOK);
			}

			//------------------------------------------------------
			// メール送信
			//------------------------------------------------------
			if (string.IsNullOrEmpty(this.MailIdValue) == false)
			{
				var blSendMailResult = false;

				// メール送信処理のみの場合
				if (this.NeedsUpdate)
				{
					blSendMailResult
						= ((aruResult.ResultOrderStatusChange != OrderCommon.ResultKbn.UpdateNG)
						&& ((aruResult.ResultProductRealStockChange == OrderCommon.ResultKbn.NoUpdate)
							|| (aruResult.ResultProductRealStockChange == OrderCommon.ResultKbn.UpdateOK))
						&& (aruResult.ResultPaymentStatusChange != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultExternalPaymentAction != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultDemandStatusChange != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultReturnExchangeStatusChange != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultRepaymentStatusChange != OrderCommon.ResultKbn.UpdateNG)
						// aruResult.ResultOrderExtendStatusChange[0～MAX29]の中に OrderCommon.ResultKbn.UpdateNG が一切無いことを確認する処理
						&& aruResult.ResultOrderExtendStatusChange.ToList().TrueForAll(valResultKbn => valResultKbn != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultScheduledShippingDateAction != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultShippingDateAction != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultReceiptOutputFlgChange != OrderCommon.ResultKbn.UpdateNG)
						&& (aruResult.ResultStorePickupStatusChange != OrderCommon.ResultKbn.UpdateNG)
						);
				}
				// メール送信のみの場合
				else
				{
					blSendMailResult = true;
				}

				// 連絡メールを送信
				if (blSendMailResult
					&& (this.MailIdValue != Constants.CONST_MAIL_ID_TO_REAL_SHOP))
				{
					blSendMailResult = OrderCommon.SendOrderMail(targetOrderId, this.MailIdValue, Constants.MailSendMethod.Manual);
				}
				// 更新結果格納
				aruResult.ResultMailSend = blSendMailResult ? OrderCommon.ResultKbn.UpdateOK : OrderCommon.ResultKbn.UpdateNG;
				if (aruResult.ResultMailSend == OrderCommon.ResultKbn.UpdateNG)
				{
					aruResult.ErrorMessages.Add(
						CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SEND_ORDER_MAIL_NG));
				}
			}
			// メール送信処理を行わない場合
			else
			{
				aruResult.ResultMailSend = OrderCommon.ResultKbn.NoUpdate;
			}

			return aruResult;
		}

		/// <summary>
		/// アクション(定期)
		/// </summary>
		/// <param name="fixedPurchase">定期情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>実行結果格納用ActionResultUnit</returns>
		private ActionResultUnit ActionForFixedPurchase(Hashtable fixedPurchase, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			var targetFixedPurchaseId = (string)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID];

			// 結果格納Hashtabe作成
			var fixedPurchaseContainer = new FixedPurchaseService().GetContainer(
				targetFixedPurchaseId,
				false,
				accessor);

			var actionResult = new ActionResultUnit(targetFixedPurchaseId);

			if (this.NeedsUpdate)
			{
				try
				{
					// はじめに更新ロックをかけておく
					new FixedPurchaseService().GetUpdLock(targetFixedPurchaseId, accessor);

					// 定期購入状況更新
					if (this.NeedsUpdateFixedPurchaseIsAlive)
					{
						switch (this.FixedPurchaseIsAliveChangeValue)
						{
							// 解約
							case Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_CANCEL:
								// 継続課金を解約する
								string apiError;
								var success = FixedPurchaseHelper.CancelPaymentContinuous(
									fixedPurchaseContainer.FixedPurchaseId,
									fixedPurchaseContainer.OrderPaymentKbn,
									fixedPurchaseContainer.ExternalPaymentAgreementId,
									this.LoginOperatorName,
									out apiError,
									accessor);

								// 定期購入解約
								var cancelFixedPurchaseReasonValue = GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_REASON_ID, WorkflowTypes.FixedPurchase);
								var cancelFixedPurchaseMemoValue = GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_MEMO, WorkflowTypes.FixedPurchase);
								success = success && new FixedPurchaseService().CancelFixedPurchase(
									fixedPurchaseContainer,
									cancelFixedPurchaseReasonValue,
									cancelFixedPurchaseMemoValue,
									this.LoginOperatorName,
									Constants.CONST_DEFAULT_DEPT_ID,
									Constants.W2MP_POINT_OPTION_ENABLED,
									UpdateHistoryAction.DoNotInsert,
									accessor);
								if (success == false)
								{
									actionResult.ResultFixedPurchaseIsAliveChangeAction = OrderCommon.ResultKbn.UpdateNG;
									actionResult.ErrorMessages.Add(
										string.IsNullOrEmpty(apiError)
											? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_CANCEL_FIXEDPURCHASE_NG)
											: CommerceMessages.GetMessages(
													CommerceMessages.ERRMSG_MANAGE_PAYMENT_CONTINUOUS_CANCEL_NG_FOR_CANCEL_FP)
												.Replace("@@ 1 @@", apiError)
												.Replace("@@ 2 @@", fixedPurchaseContainer.ExternalPaymentAgreementId));
								}
								break;

							// 定期購入再開
							case Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_RESTART:
								var updated = new FixedPurchaseService().Resume(
									fixedPurchaseContainer.FixedPurchaseId,
									this.LoginOperatorName,
									null,
									null,
									UpdateHistoryAction.DoNotInsert,
									accessor);
								if (updated == false)
								{
									actionResult.ResultFixedPurchaseIsAliveChangeAction = OrderCommon.ResultKbn.UpdateNG;
									actionResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_RESUME_FIXEDPURCHASE_NG));
								}
								break;

							// 定期購入休止
							case Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_SUSPEND:
								var suspendSuccess = new FixedPurchaseService().SuspendFixedPurchase(
									fixedPurchaseContainer,
									null,
									null,
									null,
									string.Empty,
									this.LoginOperatorName,
									UpdateHistoryAction.DoNotInsert,
									accessor);
								if (suspendSuccess == false)
								{
									actionResult.ResultFixedPurchaseIsAliveChangeAction = OrderCommon.ResultKbn.UpdateNG;
									actionResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SUSPEND_FIXEDPURCHASE_NG));
								}
								break;

							// 定期購入完了
							case Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_COMPLETE:
								var complete = new FixedPurchaseService().Complete(
									fixedPurchaseContainer.FixedPurchaseId,
									this.LoginOperatorName,
									fixedPurchaseContainer.NextShippingDate,
									fixedPurchaseContainer.NextNextShippingDate,
									UpdateHistoryAction.DoNotInsert,
									accessor);
								if (complete == false)
								{
									actionResult.ResultFixedPurchaseIsAliveChangeAction = OrderCommon.ResultKbn.UpdateNG;
									actionResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_COMPLETE_FIXEDPURCHASE_NG));
								}
								break;
						}
					}
					else
					{
						actionResult.ResultFixedPurchaseIsAliveChangeAction = OrderCommon.ResultKbn.NoUpdate;
					}

					// 定期決済ステータス更新
					if (this.NeedsUpdateFixedPurchasePaymentStatus)
					{
						// 更新
						var paymentStatus =
							((this.FixedPurchasePaymentStatusChangeValue == Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE_ACTION_NOMAL)
								? Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_NORMAL
								: Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_ERROR);
						var updatedCount = new FixedPurchaseService().UpdatePaymentStatus(
							fixedPurchaseContainer.FixedPurchaseId,
							paymentStatus,
							this.LoginOperatorName,
							UpdateHistoryAction.DoNotInsert,
							accessor);
						if (updatedCount <= 0)
						{
							actionResult.ResultFixedPurchasePaymentStatusChangeAction = OrderCommon.ResultKbn.UpdateNG;	// 更新NG
							actionResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FIXEDPURCHASE_PAYMENT_STATUS_UPDATE_NG));
						}
					}
					// 更新処理を行わない場合
					else
					{
						actionResult.ResultFixedPurchasePaymentStatusChangeAction = OrderCommon.ResultKbn.NoUpdate;
					}

					// 次回配送日を更新
					if (this.NeedsUpdateNextShippingDate)
					{
						var shopShipping = new ShopShippingService().Get(fixedPurchaseContainer.ShopId, fixedPurchaseContainer.Shippings[0].Items[0].ShippingType);
						var nextShippingDate = DateTime.Parse(
							StringUtility.ToEmpty(fixedPurchase[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE]));
						var fixedPurchaseService = new FixedPurchaseService();
						var calculateMode = fixedPurchaseService.GetCalculationMode(
							fixedPurchaseContainer.FixedPurchaseKbn,
							Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
						var nextNextShippingDate = (this.NeedsCalculateNextNextShippingDate
							? (DateTime?)fixedPurchaseService.CalculateNextShippingDate(
								fixedPurchaseContainer.FixedPurchaseKbn,
								fixedPurchaseContainer.FixedPurchaseSetting1,
								nextShippingDate,
								shopShipping.FixedPurchaseShippingDaysRequired,
								shopShipping.FixedPurchaseMinimumShippingSpan,
								calculateMode)
							: null);

						var updated = fixedPurchaseService.UpdateShippingDate(
							fixedPurchaseContainer.FixedPurchaseId,
							nextShippingDate,
							nextNextShippingDate,
							this.LoginOperatorName,
							UpdateHistoryAction.DoNotInsert,
							accessor);
						if (updated == false)
						{
							actionResult.ResultNextShippingDateChangeAction = OrderCommon.ResultKbn.UpdateNG;
							actionResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_NEXT_SHIPPING_DATE_UPDATE_NG));
						}
					}
					else
					{
						actionResult.ResultNextShippingDateChangeAction = OrderCommon.ResultKbn.NoUpdate;
					}

					// 次々回配送日更新
					if (this.NeedsUpdateNextNextShippingDate)
					{
						var updated = new FixedPurchaseService().UpdateShippingDate(
							fixedPurchaseContainer.FixedPurchaseId,
							null,
							DateTime.Parse(StringUtility.ToEmpty(fixedPurchase[Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE])),
							this.LoginOperatorName,
							UpdateHistoryAction.DoNotInsert,
							accessor);
						if (updated == false)
						{
							actionResult.ResultNextNextShippingDateChangeAction = OrderCommon.ResultKbn.UpdateNG;
							actionResult.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_NEXT_NEXT_SHIPPING_DATE_UPDATE_NG));
						}
					}
					else
					{
						actionResult.ResultNextNextShippingDateChangeAction = OrderCommon.ResultKbn.NoUpdate;
					}

					// 配送不可エリア停止変更
					if (this.NeedsUpdateFixedPurchaseStopUnavailableShippingArea)
					{
						switch (this.FixedPurchaseStopUnavailableShippingAreaChangeValue)
						{
							// 配送不可エリア停止
							case Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE_ACTION_ON:
								new FixedPurchaseService().
									UpdateFixedPurchaseStatusToStopUnavailableShippingArea(
										fixedPurchaseContainer.FixedPurchaseId,
										this.LoginOperatorName,
										UpdateHistoryAction.Insert,
										accessor);
								break;

							// 配送不可エリア停止解除
							case Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE_ACTION_OFF:
								new FixedPurchaseService().
									UpdateFixedPurchaseStatusStopUnavailableShippingAreaToNormal(
										fixedPurchaseContainer.FixedPurchaseId,
										this.LoginOperatorName,
										UpdateHistoryAction.Insert,
										accessor);
								break;
						}
					}
					else
					{
						actionResult.ResultFixedPurchaseStopUnavailableShippingAreaChangeAction = OrderCommon.ResultKbn.NoUpdate;
					}

					// 注文拡張ステータス１～４０を更新する場合(※利用数のみ)
					for (var index = 0; index < Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; index++)
					{
						if (this.NeedsUpdateFixedPurchaseExtendStatus[index])
						{
							// 注文拡張ステータス更新
							var updatedCount = new FixedPurchaseService().UpdateExtendStatus(
								targetFixedPurchaseId,
								index + 1,
								this.FixedPurchaseExtendStatusChangeValues[index],
								DateTime.Today,
								(string)fixedPurchase[Constants.FIELD_ORDER_LAST_CHANGED],
								UpdateHistoryAction.DoNotInsert,
								accessor);

							if (updatedCount <= 0)
							{
								actionResult.ResultFixedPurchaseExtendStatusChange[index] = OrderCommon.ResultKbn.UpdateNG;	// 更新NG
								actionResult.ErrorMessages.Add(
									string.Format(
										CommerceMessages.GetMessages(
											CommerceMessages.ERRMSG_ORDER_EXTEND_STATUS_UPDATE_NG),
										index + 1));
							}
						}
						else
						{
							actionResult.ResultFixedPurchaseExtendStatusChange[index] = OrderCommon.ResultKbn.NoUpdate;
						}
					}

					// 結果格納（全て成功しているか）
					var resultUpdate = (actionResult.ResultFixedPurchaseIsAliveChangeAction != OrderCommon.ResultKbn.UpdateNG)
						&& (actionResult.ResultFixedPurchasePaymentStatusChangeAction != OrderCommon.ResultKbn.UpdateNG)
						&& (actionResult.ResultNextShippingDateChangeAction != OrderCommon.ResultKbn.UpdateNG)
						&& (actionResult.ResultNextNextShippingDateChangeAction != OrderCommon.ResultKbn.UpdateNG);
					foreach (var resultFixedPurchaseExtendStatusChange in actionResult.ResultFixedPurchaseExtendStatusChange)
					{
						resultUpdate &= (resultFixedPurchaseExtendStatusChange != OrderCommon.ResultKbn.UpdateNG);
					}

					// コミット/ロールバック
					if (resultUpdate)
					{
						// 更新履歴登録
						if (updateHistoryAction == UpdateHistoryAction.Insert)
						{
							new UpdateHistoryService().InsertForFixedPurchase(
								(string)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID],
								this.LoginOperatorName,
								accessor);
						}

						// 更新処理確定
						accessor.CommitTransaction();
					}
					else
					{
						// 更新処理ロールバック
						accessor.RollbackTransaction();

						// 処理結果を全て更新NGにする
						actionResult.Reset(OrderCommon.ResultKbn.UpdateNG, hasInvoiceUpdateOK);
					}
				}
				catch (Exception ex)
				{
					// ログに書き込み
					AppLogger.WriteError("ワークフローのステータス更新部分で例外発生：定期購入ID=" + targetFixedPurchaseId, ex);

					// 更新処理ロールバック
					accessor.RollbackTransaction();

					// 処理結果を全て更新NGにする
					actionResult.Reset(OrderCommon.ResultKbn.UpdateNG, hasInvoiceUpdateOK);
				}
			}
			else
			{
				// 全てのステータスが更新対象外のための結果表示
				actionResult.Reset(OrderCommon.ResultKbn.NoUpdate, hasInvoiceUpdateOK);
			}

			return actionResult;
		}

		/// <summary>
		/// 外部決済実行
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="targetOrderId">対象注文ID</param>
		/// <param name="actionResult">アクション結果</param>
		/// <param name="isDigitalContents">デジタルコンテンツか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>実行結果</returns>
		private bool ExecExternalPayment(
			Hashtable order,
			string targetOrderId,
			ActionResultUnit actionResult,
			bool isDigitalContents,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var paymentDetailType = "";
			var accountSettlementCompanyName = PaymentFileLogger.PaymentType.Unknown;
			if (this.NeedsExecExternalPaymentAction && ((decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT] != 0))
			{
				DataRowView drvOrder = OrderCommon.GetOrder(targetOrderId, accessor)[0];
				order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = drvOrder[Constants.FIELD_ORDER_PAYMENT_ORDER_ID];
				order.Add(Constants.FIELD_ORDER_SETTLEMENT_CURRENCY, drvOrder[Constants.FIELD_ORDER_SETTLEMENT_CURRENCY]);
				order.Add(Constants.FIELD_ORDER_SETTLEMENT_RATE, drvOrder[Constants.FIELD_ORDER_SETTLEMENT_RATE]);
				order.Add(Constants.FIELD_ORDER_SETTLEMENT_AMOUNT, drvOrder[Constants.FIELD_ORDER_SETTLEMENT_AMOUNT]);
				order.Add(Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID, drvOrder[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID]);
				order.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, drvOrder[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]);

				order.Add(Constants.FIELD_ORDER_ORDER_SHIPPED_DATE, drvOrder[Constants.FIELD_ORDER_ORDER_SHIPPED_DATE]);
				order.Add(Constants.FIELD_ORDER_ORDER_SHIPPING_DATE, drvOrder[Constants.FIELD_ORDER_ORDER_SHIPPING_DATE]);
				order.Add(Constants.FIELD_ORDER_ORDER_DELIVERING_DATE, drvOrder[Constants.FIELD_ORDER_ORDER_DELIVERING_DATE]);
				order.Add(Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG, drvOrder[Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG]);

				var orderModel = new OrderModel(order);
				switch (this.ExternalPaymentActionValue)
				{
					// Zeus決済連携の場合
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ZEUS_CREDITCARD_PAYMENT:
						paymentDetailType = "";
						// 仮売り・本売りの場合(ZEUSクーリングオフ)
						if ((isDigitalContents ? Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD) == Constants.PaymentCreditCardPaymentMethod.PAYMENT_WITH_AUTH)
						{
							paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT;
							accountSettlementCompanyName = PaymentFileLogger.PaymentType.Zcom;
							var resultZeus = new ZeusCoolingOffBatchApi().Exec(
								(decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT],
								DateTime.Parse((string)order["update_date"]).ToString("yyyyMMdd"),
								(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID]);

							var isResultZeusSuccess = resultZeus.Success;

							if (isResultZeusSuccess == false)
							{
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
								this.ApiErrorMessage = resultZeus.ErrorMessage;
							}
							else
							{
								this.SuccessMessage = LogCreator.CrateMessageWithCardTranId(
									(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
									(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
							}
						}
						// 与信後決済の場合(SecureLinkで即売)
						else if ((isDigitalContents ? Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD) == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
						{
							paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT;
							accountSettlementCompanyName = PaymentFileLogger.PaymentType.Zcom;
							// 仮決済取引IDが格納済みかどうかの確認
							if ((string)drvOrder[Constants.FIELD_ORDER_CARD_TRAN_ID] == Constants.FLG_REALSALES_TEMP_TRAN_ID)
							{
								var mailAddress = string.Empty;
								if (((string)drvOrder[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR]).Length != 0)
								{
									mailAddress = (string)drvOrder[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR];
								}
								else if (((string)drvOrder[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2]).Length != 0)
								{
									mailAddress = (string)drvOrder[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2];
								}

								// 管理画面ではユーザの登録削除にかかわらず取得する
								var userCreditCard = UserCreditCard.Get(
									(string)order[Constants.FIELD_ORDER_USER_ID],
									(int)drvOrder[Constants.FIELD_ORDER_CREDIT_BRANCH_NO]);
								try
								{
									// ログファイルにログを落とす
									PaymentFileLogger.WritePaymentLog(
										null,
										Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
										PaymentFileLogger.PaymentType.Zeus,
										PaymentFileLogger.PaymentProcessingType.OrderWorkFlowSettlementLinkageProcessing,
										"",
										new Dictionary<string, string>
										{
											{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
											{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
										});

									var resultZeus = new ZeusSecureLinkBatchApi().Exec(
										(decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT],
										userCreditCard.CooperationInfo.ZeusTelNo,
										mailAddress,
										userCreditCard.CooperationInfo.ZeusSendId,
										(string)drvOrder[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE]);

									var finalZeusResult = resultZeus.Success && (new OrderService().UpdateCardTranId(
										(string)order[Constants.FIELD_ORDER_ORDER_ID],
										resultZeus.ZeusOrderId,
										this.LoginOperatorName,
										UpdateHistoryAction.DoNotInsert,
										accessor) > 0);
									if (finalZeusResult == false)
									{
										actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
										this.ApiErrorMessage = resultZeus.ErrorMessage;
									}
									else
									{
										this.SuccessMessage = LogCreator.CreateMessageWithZeus(
											(string)drvOrder[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE],
											userCreditCard.CooperationInfo.ZeusSendId);
									}
								}
								catch (WebException wEx)
								{
									actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.NoUpdate;
									string errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_CONNECTION_ERROR);
									// 誤って再実行されるのを防ぐため、決済取引IDにエラーメッセージを格納
									new OrderService().UpdateCardTranId(
										(string)order[Constants.FIELD_ORDER_ORDER_ID],
										errorMessage,
										this.LoginOperatorName,
										UpdateHistoryAction.DoNotInsert,
										accessor);

									// ログ格納処理
									PaymentFileLogger.WritePaymentLog(
										false,
										Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
										PaymentFileLogger.PaymentType.Zeus,
										PaymentFileLogger.PaymentProcessingType.OrderWorkFlowSettlementLinkageProcessing,
										BaseLogger.CreateExceptionMessage(wEx),
										new Dictionary<string, string>
										{
											{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
											{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
										});
								}
							}
							else
							{
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
								var errorMessage = CommerceMessages
									.GetMessages(CommerceMessages.ERRMSG_MANAGER_ORDER_CARD_ALREADY_REALSALES_ERROR).Replace(
										"@@ 1 @@",
										(string)drvOrder[Constants.FIELD_ORDER_ORDER_ID]);

								AppLogger.WriteError(errorMessage);

								// ログ格納処理
								PaymentFileLogger.WritePaymentLog(
									null,
									Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
									PaymentFileLogger.PaymentType.Zeus,
									PaymentFileLogger.PaymentProcessingType.OrderWorkFlowSettlementLinkageProcessing,
									errorMessage,
									new Dictionary<string, string>
									{
										{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
										{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
									});
							}
						}
						break;

					// SBPSクレジット決済連携の場合
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_CREDITCARD_SALES:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Sbps;

						// 「指定日売上」の場合
						if ((isDigitalContents ? Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD) == Constants.PaymentCreditCardPaymentMethod.PAYMENT_WITH_AUTH)
						{
							PaymentSBPSCreditSaleApi saleApi = new PaymentSBPSCreditSaleApi();
							var saleApiExecResult = saleApi.Exec(
								(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
								(decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]);

							if (saleApiExecResult == false)
							{
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
								this.ApiErrorMessage = LogCreator.CreateErrorMessage(
									saleApi.ResponseData.ResErrMessages,
									saleApi.ResponseData.ResErrCode);
							}
							else
							{
								this.SuccessMessage = LogCreator.CrateMessageWithCardTranId(
									(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
									string.Empty);
							}
						}
						// 与信後決済の場合
						else if ((isDigitalContents ? Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD) == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
						{
							paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT;
							accountSettlementCompanyName = PaymentFileLogger.PaymentType.Sbps;
							// 仮決済取引IDが格納済みかどうかの確認
							if ((string)drvOrder[Constants.FIELD_ORDER_CARD_TRAN_ID] == Constants.FLG_REALSALES_TEMP_TRAN_ID)
							{
								// 管理画面ではユーザの登録削除にかかわらず取得する
								var userCreditCard = UserCreditCard.Get(
									(string)order[Constants.FIELD_ORDER_USER_ID],
									(int)drvOrder[Constants.FIELD_ORDER_CREDIT_BRANCH_NO]);
								try
								{
									// ログファイル処理
									PaymentFileLogger.WritePaymentLog(
										null,
										Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
										PaymentFileLogger.PaymentType.Sbps,
										PaymentFileLogger.PaymentProcessingType.OrderWorkFlowSettlementLinkageProcessing,
										"",
										new Dictionary<string, string>
										{
											{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
											{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
										});

									var authApi = new PaymentSBPSCreditAuthApi();
									var commitApi = new PaymentSBPSCreditCommitApi();
									// 決済注文ID生成
									var paymentOrderId = OrderCommon.CreatePaymentOrderId((string)order[Constants.FIELD_ORDER_SHOP_ID]);

									// SBPSクレジット「リアル与信」実行
									var authApiAndCommitApiExecResult = authApi.Exec(
											userCreditCard.CooperationInfo.SBPSCustCode,
											paymentOrderId,
											Constants.PAYMENT_SETTING_SBPS_ITEM_ID,
											Constants.PAYMENT_SETTING_SBPS_ITEM_NAME,
											new List<PaymentSBPSBase.ProductItem>(),
											(decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT],
											PaymentSBPSUtil.GetCreditDivideInfo((string)drvOrder[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE]))
										&&
										// SBPSクレジット「コミット」処理
										commitApi.Exec(authApi.ResponseData.ResTrackingId)
										&&
										(new OrderService().UpdateCardTranId(
											(string)order[Constants.FIELD_ORDER_ORDER_ID],
											authApi.ResponseData.ResTrackingId,
											this.LoginOperatorName,
											UpdateHistoryAction.DoNotInsert,
											accessor) > 0);

									if (authApiAndCommitApiExecResult == false)
									{
										var authApiAndCommitErrorMessage = LogCreator.CreateErrorMessage(
											authApi.ResponseData.ResErrCode,
											authApi.ResponseData.ResErrMessages);
										actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
										this.ApiErrorMessage = authApiAndCommitErrorMessage;
									}
									else
									{
										var authApiAndCommitSuccessMessage = LogCreator.CreateMessageWithTrackingId(
											(string)order[Constants.FIELD_ORDER_ORDER_ID],
											authApi.ResponseData.ResTrackingId);
										this.SuccessMessage = authApiAndCommitSuccessMessage;
									}
								}
								catch (WebException)
								{
									actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.NoUpdate;
									var errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_CONNECTION_ERROR);
									// 誤って再実行されるのを防ぐため、決済取引IDにエラーメッセージを格納
									new OrderService().UpdateCardTranId(
										(string)order[Constants.FIELD_ORDER_ORDER_ID],
										errorMessage,
										this.LoginOperatorName,
										UpdateHistoryAction.DoNotInsert,
										accessor);

									// ログ格納処理
									PaymentFileLogger.WritePaymentLog(
										false,
										Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
										PaymentFileLogger.PaymentType.Sbps,
										PaymentFileLogger.PaymentProcessingType.OrderWorkFlowSettlementLinkageProcessing,
										errorMessage,
										new Dictionary<string, string>
										{
											{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
											{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
										});
								}
							}
							else
							{
								paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT;
								accountSettlementCompanyName = PaymentFileLogger.PaymentType.Sbps;
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;

								var errorMessage = CommerceMessages
									.GetMessages(CommerceMessages.ERRMSG_MANAGER_ORDER_CARD_ALREADY_REALSALES_ERROR).Replace(
										"@@ 1 @@",
										(string)drvOrder[Constants.FIELD_ORDER_ORDER_ID]);

								// ログ格納処理
								PaymentFileLogger.WritePaymentLog(
									null,
									Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
									PaymentFileLogger.PaymentType.Sbps,
									PaymentFileLogger.PaymentProcessingType.OrderWorkFlowSettlementLinkageProcessing,
									errorMessage,
									new Dictionary<string, string>
									{
										{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
										{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
									});
							}
						}
						break;

					// GMOクレジット決済連携の場合
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CREDITCARD_SALES:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Gmo;
						// 仮売上⇒実売上の場合
						if (Constants.PAYMENT_SETTING_GMO_PAYMENTMETHOD == Constants.GmoCreditCardPaymentMethod.Auth)
						{
							// w2、GMO側で金額が異なるか？
							PaymentGmoCredit paymentGMO = new PaymentGmoCredit();
							if (paymentGMO.IsPriceChange((string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
								(decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]))
							{
								// GMO側の金額変更
								var isChangeTranSucces = paymentGMO.ChangeTran(
									(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
									(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
									(decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]);

								// ログ格納処理
								PaymentFileLogger.WritePaymentLog(
									isChangeTranSucces,
									Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
									PaymentFileLogger.PaymentType.Gmo,
									PaymentFileLogger.PaymentProcessingType.OrderWorkFlowSettlementLinkageProcessing,
									isChangeTranSucces
										? ""
										: paymentGMO.ErrorMessages,
									new Dictionary<string, string>
									{
										{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
										{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
									});

								if (isChangeTranSucces == false)
								{
									actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
									this.ApiErrorMessage = paymentGMO.ErrorMessages;
								}
								else
								{
									this.SuccessMessage = LogCreator.CrateMessageWithCardTranId(
										(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
										(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
								}
							}

							if (actionResult.ResultExternalPaymentAction != OrderCommon.ResultKbn.UpdateNG)
							{
								// 仮売上⇒実売上
								var salesResult = paymentGMO.Sales(
									(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
									(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
									(decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]);

								// ログ格納処理
								PaymentFileLogger.WritePaymentLog(
									salesResult,
									Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
									PaymentFileLogger.PaymentType.Gmo,
									PaymentFileLogger.PaymentProcessingType.OrderWorkFlowSettlementLinkageProcessingForVirtualToReal,
									salesResult
										? LogCreator.CreateMessageWithTrackingId(
											(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
											(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID])
										: paymentGMO.ErrorMessages,
									new Dictionary<string, string>
									{
										{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
										{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
									});

								if (salesResult == false)
								{
									actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
									this.ApiErrorMessage = paymentGMO.ErrorMessages;
								}
								else
								{
									this.SuccessMessage = LogCreator.CreateMessageWithTrackingId(
										(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
										(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
								}
							}
						}
						break;
					
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_INVOICE:
						paymentDetailType = (string)order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN];
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Gmo;
						var confirmBillingError = OrderCommon.ExecConfirmBillingGmoPost((string)order[Constants.FIELD_ORDER_ORDER_ID], Constants.FLG_LASTCHANGED_BATCH, (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID], accessor);

						if (string.IsNullOrEmpty(confirmBillingError))
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateOK;
							this.SuccessMessage = LogCreator.CrateMessageWithCardTranId(
								(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
						}
						else
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = confirmBillingError;
						}

						PaymentFileLogger.WritePaymentLog(
							string.IsNullOrEmpty(confirmBillingError),
							paymentDetailType,
							PaymentFileLogger.PaymentType.Gmo,
							PaymentFileLogger.PaymentProcessingType.OrderWorkFlowSettlementLinkageProcessing,
							confirmBillingError,
							new Dictionary<string, string>
							{
								{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
								{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
							});

						break;

					// ZCOMクレジット決済連携の場合
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ZCOM_CREDITCARD_SALES:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Zcom;
						// 売上確定
						var zcomAdp = new ZcomSalesRequestAdapter(StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]));
						var res = zcomAdp.Execute();

						if (res.IsSuccessResult() == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = LogCreator.CreateErrorMessage(
								res.GetErrorCodeValue(),
								res.GetErrorDetailValue());
						}
						else
						{
							this.SuccessMessage = LogCreator.CrateMessageWithCardTranId(
								"",
								StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]));
						}

						break;

					// e-SCOTTクレジット決済連携の場合
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ESCOTT_CREDITCARD_SALES:

						var paymentMethod = isDigitalContents
							? Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD_FORDIGITALCONTENTS
							: Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD;

						var escottSuccess = false;
						var escottResponseCd = string.Empty;
						var escottResponseMessage = string.Empty;

						if (paymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
						{
							// 仮決済取引IDが格納済みかどうかの確認
							if ((string)drvOrder[Constants.FIELD_ORDER_CARD_TRAN_ID] == Constants.FLG_REALSALES_TEMP_TRAN_ID)
							{
								var userCreditCard = UserCreditCard.Get(
								(string)order[Constants.FIELD_ORDER_USER_ID],
								(int)drvOrder[Constants.FIELD_ORDER_CREDIT_BRANCH_NO]);

								try
								{
									// ログファイルにログを落とす
									PaymentFileLogger.WritePaymentLog(
										null,
										Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
										PaymentFileLogger.PaymentType.Zeus,
										PaymentFileLogger.PaymentProcessingType.OrderWorkFlowSettlementLinkageProcessing,
										"",
										new Dictionary<string, string>
										{
											{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
											{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] }
										});

									var escottAdp = EScottMaster1GatheringApi.CreateEScottMaster1GatheringApi(
										(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
										(decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT],
										(string)drvOrder[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE],
										userCreditCard,
										DateTime.Parse((string)order["update_date"]));
									var escottAuthApiResult = escottAdp.ExecRequest();

									var ecottAuthResult = escottAuthApiResult.IsSuccess && (new OrderService().UpdateCardTranId(
										(string)order[Constants.FIELD_ORDER_ORDER_ID],
										escottAuthApiResult.CardTranId,
										this.LoginOperatorName,
										UpdateHistoryAction.DoNotInsert,
										accessor) > 0);
									escottSuccess = ecottAuthResult;
									escottResponseCd = escottAuthApiResult.ResponseCd;
									escottResponseMessage = escottAuthApiResult.ResponseMessage;
								}
								catch (WebException wEx)
								{
									actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.NoUpdate;
									var errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_CONNECTION_ERROR);
									// 誤って再実行されるのを防ぐため、決済取引IDにエラーメッセージを格納
									new OrderService().UpdateCardTranId(
										(string)order[Constants.FIELD_ORDER_ORDER_ID],
										errorMessage,
										this.LoginOperatorName,
										UpdateHistoryAction.DoNotInsert,
										accessor);

									// ログ格納処理
									PaymentFileLogger.WritePaymentLog(
										false,
										Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
										PaymentFileLogger.PaymentType.Zeus,
										PaymentFileLogger.PaymentProcessingType.OrderWorkFlowSettlementLinkageProcessing,
										BaseLogger.CreateExceptionMessage(wEx),
										new Dictionary<string, string>
										{
											{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
											{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] }
										});
								}
							}
						}
						else
						{

							var escottAdp = EScottProcess1CaptureApi.CreateEScottProcess1CaptureApi(
								(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
								DateTime.Parse((string)order["update_date"]));
							var escottRes = escottAdp.ExecRequest();

							escottSuccess = escottRes.IsSuccess;
							escottResponseCd = escottRes.ResponseCd;
							escottResponseMessage = escottRes.ResponseMessage;

						}

						if (escottSuccess == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = LogCreator.CreateErrorMessage(
								escottResponseCd,
								escottResponseMessage);
						}
						else
						{
							this.SuccessMessage = LogCreator.CrateMessageWithCardTranId(
								(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
								StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]));
						}

						break;

					// ベリトランスクレジット決済連携の場合
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_VERITRANS_CREDITCARD_SALES:

						var paymentMethodVeriTrans = isDigitalContents
							? Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD_FORDIGITALCONTENTS
							: Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD;

						if (paymentMethodVeriTrans == Constants.VeritransCreditCardPaymentMethod.Auth)
						{
							var responseVeritrans = new PaymentVeritransCredit().Capture(
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
								order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT].ToPriceString());

							if (responseVeritrans.Mstatus != VeriTransConst.RESULT_STATUS_OK)
							{
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
								this.ApiErrorMessage = LogCreator.CreateErrorMessage(
									responseVeritrans.VResultCode,
									responseVeritrans.MerrMsg);
							}
							else
							{
								this.SuccessMessage = LogCreator.CrateMessageWithCardTranId(
									responseVeritrans.CustTxn,
									StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]));
							}
						}

						break;
					
					// ペイジェントクレジット決済連携の場合
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYGENT_CREDITCARD_SALES:
						var paygentPaymentMethod = Constants.DIGITAL_CONTENTS_OPTION_ENABLED && isDigitalContents
							? Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS
							: Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD;

						// 与信後決済の場合だけ処理実行
						if (paygentPaymentMethod == Constants.PaygentCreditCardPaymentMethod.Auth)
						{
							var realSaleParams = new PaygentApiHeader(PaygentConstants.PAYGENT_APITYPE_CARD_REALSALE);
							realSaleParams.PaymentId = (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID];
							var realSaleResult = PaygentApiFacade.SendRequest(realSaleParams);
							// 成功・失敗後処理
							if ((string)realSaleResult[PaygentConstants.RESPONSE_STATUS] == PaygentConstants.PAYGENT_RESPONSE_STATUS_SUCCESS)
							{
								this.SuccessMessage = LogCreator.CrateMessageWithCardTranId(
									(string)realSaleResult["payment_id"],
									StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID])
									);
							}
							else
							{
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
								this.ApiErrorMessage = LogCreator.CreateErrorMessage(
									(string)realSaleResult[PaygentConstants.RESPONSE_CODE],
									(string)realSaleResult[PaygentConstants.RESPONSE_DETAIL]);
							}
						}
						break;


					// ドコモケータイ払い決済の場合
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_DOCOMO_PAYMENT:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Unknown;
						var docomoPayment = new DocomoPayment();

						var sendDecisionResult = docomoPayment.SendDecision(order);

						var docomoErrorMessage = PaymentFileLogger.PaymentProcessingType.DocomoTelePhonePayment + "\t"
							+ docomoPayment.ErrorMessage;

						// ログファイル格納処理
						PaymentFileLogger.WritePaymentLog(
							sendDecisionResult,
							Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG,
							PaymentFileLogger.PaymentType.Sbps,
							PaymentFileLogger.PaymentProcessingType.DocomoTelePhonePayment,
							sendDecisionResult
								? ""
								: docomoErrorMessage,
							new Dictionary<string, string>
							{
								{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
								{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
							});

						if (sendDecisionResult == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = docomoErrorMessage;
						}
						else
						{
							this.SuccessMessage = LogCreator.CreateMessage(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								"");
						}

						break;

					// S!まとめて支払い決済の場合
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SOFTBANK_PAYMENT:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_ORG;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Sbps;

						var softbankPayment = new SoftbankPayment();
						var sendDecitionResult = softbankPayment.SendDecision(order);

						var softBankSuccessMessage = string.Format(
							"{0}：{1}",
							PaymentFileLogger.PaymentProcessingType.SettlementCardPaymentNumberOfTimesStatement.ToText(),
							order[Constants.FIELD_ORDER_CARD_INSTRUMENTS]);

						var softBankFailureMessage = PaymentFileLogger.PaymentProcessingType.SoftBankPaymentTogether
							+ "\t" + softbankPayment.ErrorMessage;

						// ログ格納処理
						PaymentFileLogger.WritePaymentLog(
							sendDecitionResult,
							Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_ORG,
							PaymentFileLogger.PaymentType.Sbps,
							PaymentFileLogger.PaymentProcessingType.SoftBankPaymentTogether,
							sendDecitionResult
								? softBankSuccessMessage
								: softBankFailureMessage,
							new Dictionary<string, string>
							{
								{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
								{
									Constants.FIELD_ORDER_PAYMENT_ORDER_ID,
									(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]
								}
							});

						if (sendDecitionResult == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = softBankFailureMessage;
						}
						else
						{
							this.SuccessMessage = softBankSuccessMessage;
						}

						break;

					// SBPS ソフトバンク・ワイモバイルまとめて支払い売上処理
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_SOFTBANKKETAI_PAYMENT:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Sbps;

						var softbankKetaiSaleApi = new PaymentSBPSCareerSoftbankKetaiSaleApi();
						var softbankKetaiSaleApiResult =
							softbankKetaiSaleApi.Exec((string)order[Constants.FIELD_ORDER_CARD_TRAN_ID]);

						var softbankKetaiSaleApiErrorMessage = softbankKetaiSaleApiResult == false ? LogCreator.CreateErrorMessage(
							softbankKetaiSaleApi.ResponseData.ResErrCode,
							softbankKetaiSaleApi.ResponseData.ResErrMessages) : "";

						// ログ格納処理
						PaymentFileLogger.WritePaymentLog(
							softbankKetaiSaleApiResult,
							Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS,
							PaymentFileLogger.PaymentType.Sbps,
							PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
							softbankKetaiSaleApiResult
								? ""
								: softbankKetaiSaleApiErrorMessage,
							new Dictionary<string, string>
							{
								{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
								{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
							});

						if (softbankKetaiSaleApiResult == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = softbankKetaiSaleApiErrorMessage;
						}
						else
						{
							this.SuccessMessage = LogCreator.CreateMessage(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
						}

						break;

					// SBPS ドコモケータイ売上処理
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_DOCOMOKETAI_PAYMENT:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Sbps;

						var docomoKetaiSaleApi = new PaymentSBPSCareerDocomoKetaiSaleApi();
						var docomoKetaiSaleApiExecResult = docomoKetaiSaleApi.Exec(
							(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
							(decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]);

						var docomoKetaiSaleApiErrorMessage = (docomoKetaiSaleApiExecResult == false) ? LogCreator.CreateErrorMessage(
							docomoKetaiSaleApi.ResponseData.ResErrCode,
							docomoKetaiSaleApi.ResponseData.ResErrMessages) : "";

						// ログ格納処理
						PaymentFileLogger.WritePaymentLog(
							docomoKetaiSaleApiExecResult,
							Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS,
							PaymentFileLogger.PaymentType.Sbps,
							PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
							docomoKetaiSaleApiExecResult
								? ""
								: docomoKetaiSaleApiErrorMessage,
							new Dictionary<string, string>
							{
								{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
								{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
							});

						if (docomoKetaiSaleApiExecResult == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = docomoKetaiSaleApiErrorMessage;
						}
						else
						{
							this.SuccessMessage = LogCreator.CreateMessage(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
						}

						break;

					// SBPS auかんたん決済売上処理
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_AUKANTAN_PAYMENT:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Sbps;

						var auKantanSaleApi = new PaymentSBPSCareerAuKantanSaleApi();
						var auKantanSaleApiExecResult = auKantanSaleApi.Exec(
							(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
							(decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]);

						var aukantanSaleApiErrorMessage = (auKantanSaleApiExecResult == false) ? LogCreator.CreateErrorMessage(
							auKantanSaleApi.ResponseData.ResErrCode,
							auKantanSaleApi.ResponseData.ResErrMessages) : "";

						// ログ格納処理
						PaymentFileLogger.WritePaymentLog(
							auKantanSaleApiExecResult,
							Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS,
							PaymentFileLogger.PaymentType.Sbps,
							PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
							auKantanSaleApiExecResult
								? ""
								: aukantanSaleApiErrorMessage,
							new Dictionary<string, string>
							{
								{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
								{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
							});

						if (auKantanSaleApiExecResult == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = aukantanSaleApiErrorMessage;
						}
						else
						{
							this.SuccessMessage = LogCreator.CreateMessage(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
						}

						break;

					// SBPS リクルートかんたん決済売上処理
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_RECRUIT_PAYMENT:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Sbps;

						var rKantanSaleApi = new PaymentSBPSRecruitSaleApi();
						var rKantanSaleApiExecResult = rKantanSaleApi.Exec(
							(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
							(decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]);

						var rKantanSaleApiErrorMessage = (rKantanSaleApiExecResult == false) ? LogCreator.CreateErrorMessage(
							rKantanSaleApi.ResponseData.ResErrCode,
							rKantanSaleApi.ResponseData.ResErrMessages) + "\t" + LogCreator.CreateMessage(
							(string)order[Constants.FIELD_ORDER_ORDER_ID],
							"") : "";

						//ログ格納処理
						PaymentFileLogger.WritePaymentLog(
							rKantanSaleApiExecResult,
							Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS,
							PaymentFileLogger.PaymentType.Sbps,
							PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
							rKantanSaleApiExecResult
								? ""
								: rKantanSaleApiErrorMessage,
							new Dictionary<string, string>
							{
								{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
								{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
							});

						if (rKantanSaleApiExecResult == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = rKantanSaleApiErrorMessage;
						}
						else
						{
							this.SuccessMessage = this.SuccessMessage = LogCreator.CreateMessage(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
						}

						break;

					// SBPS 楽天ID決済売上処理
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_RAKUTEN_ID_PAYMENT:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Sbps;

						var rakutenIdSaleApi = new PaymentSBPSRakutenIdSaleApi();
						var rakutenIdSaleApiExecResult = rakutenIdSaleApi.Exec(
							(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
							(decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]);

						var rakutenIdSaleApiErrorMessage = rakutenIdSaleApiExecResult == false ? LogCreator.CreateErrorMessage(
							rakutenIdSaleApi.ResponseData.ResErrCode,
							rakutenIdSaleApi.ResponseData.ResErrMessages) + "\t" + LogCreator.CreateMessage(
							(string)order[Constants.FIELD_ORDER_ORDER_ID],
							"") : "";

						// ログ格納処理
						PaymentFileLogger.WritePaymentLog(
							rakutenIdSaleApiExecResult,
							Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS,
							PaymentFileLogger.PaymentType.Sbps,
							PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
							rakutenIdSaleApiExecResult
								? ""
								: rakutenIdSaleApiErrorMessage,
							new Dictionary<string, string>
							{
								{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
								{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
							});

						if (rakutenIdSaleApiExecResult == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = rakutenIdSaleApiErrorMessage;
						}
						else
						{
							this.SuccessMessage = this.SuccessMessage = LogCreator.CreateMessage(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
						}

						break;

					// GMO後払い出荷報告処理
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CVS_DEF_SHIP:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Gmo;

						var facade = new GmoDeferredApiFacade();
						var request = new GmoRequestShipment();

						request.Transaction = new TransactionElement();
						request.Transaction.GmoTransactionId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_CARD_TRAN_ID]);
						request.Transaction.Pdcompanycode = PaymentGmoDeliveryServiceCode.GetDeliveryServiceCode(DeliveryCompanyUtil.GetDeliveryCompanyType(
							(string)order[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID],
							(string)order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]));
						request.Transaction.Slipno = StringUtility.ToEmpty(order[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO]);

						var resultGmo = facade.Shipment(request);

						var isResultGmoOk = (resultGmo.Result == ResultCode.OK);

						var gmoErrorMessage = (isResultGmoOk == false) ? string.Join(
							"\t",
							resultGmo.Errors.Error.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)).ToArray()) : "";

						// ログ格納処理
						PaymentFileLogger.WritePaymentLog(
							isResultGmoOk,
							Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
							PaymentFileLogger.PaymentType.Gmo,
							PaymentFileLogger.PaymentProcessingType.ShippingReport,
							isResultGmoOk
								? ""
								: gmoErrorMessage,
							new Dictionary<string, string>
							{
								{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
								{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
							});

						if (isResultGmoOk == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = gmoErrorMessage;
						}
						else
						{
							this.SuccessMessage = this.SuccessMessage = LogCreator.CreateMessage(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
						}

						break;

					// Atodene後払い出荷報告処理
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATODENE_CVS_DEF_SHIP:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Atodene;
						var deliveryServiceCode =
							PaymentAtodeneDeliveryServiceCode.GetDeliveryServiceCode(
								DeliveryCompanyUtil.GetDeliveryCompanyType(
									StringUtility.ToEmpty(order[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID]),
									Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF));

						var atodeneAdp = new AtodeneShippingAdapter(
							StringUtility.ToEmpty(order[Constants.FIELD_ORDER_CARD_TRAN_ID]),
							StringUtility.ToEmpty(order[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO]),
							deliveryServiceCode);

						var result = atodeneAdp.Execute();

						var isAtodeneAdpExecuteSuccess = (result.Result == AtodeneConst.RESULT_OK);

						var atodeneErrorMessage = (string)order[Constants.FIELD_ORDER_ORDER_ID] + "\r\n"
							+ (((result.Errors != null) && (result.Errors.Error != null))
								? string.Join(
									"\t",
									result.Errors.Error.Select(x => x.ErrorCode + "：" + x.ErrorMessage).ToArray())
								: "");

						// ログ格納処理
						PaymentFileLogger.WritePaymentLog(
							isAtodeneAdpExecuteSuccess,
							Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
							PaymentFileLogger.PaymentType.Atodene,
							PaymentFileLogger.PaymentProcessingType.ShippingReportForPostpaid,
							isAtodeneAdpExecuteSuccess
								? ""
								: atodeneErrorMessage,
							new Dictionary<string, string>
							{
								{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
								{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
							});

						if (isAtodeneAdpExecuteSuccess == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = atodeneErrorMessage;
						}
						else
						{
							this.SuccessMessage = this.SuccessMessage = LogCreator.CreateMessage(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
						}

						break;

					// DSK後払い出荷報告処理
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_DSK_CVS_DEF_SHIP:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Dsk;
						var dskDeliveryServiceCode =
							PaymentDskDeferredDeliveryServiceCode.GetDeliveryServiceCode(
								DeliveryCompanyUtil.GetDeliveryCompanyType(
									StringUtility.ToEmpty(order[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID]),
									Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF));
						var dskOrderModel = new OrderModel(drvOrder);
						dskOrderModel.Shippings = new OrderService().GetShippingAll(dskOrderModel.OrderId, accessor);

						var dskDeferredAdapter = new DskDeferredShipmentAdapter(
							dskOrderModel,
							(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
							(string)order[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO],
							dskDeliveryServiceCode);

						var dskDeferredResult = dskDeferredAdapter.Execute();

						var isDskDefrredAdpExecuteSuccess = dskDeferredResult.IsResultOk;

						var dskDefrredErrorMessage = (string)order[Constants.FIELD_ORDER_ORDER_ID] + "\r\n"
							+ (((dskDeferredResult.Errors != null) && (dskDeferredResult.Errors.Error != null))
								? string.Join(
									"\t",
									dskDeferredResult.Errors.Error.Select(x => x.ErrorCode + "：" + x.ErrorMessage).ToArray())
								: "");

						// ログ格納処理
						PaymentFileLogger.WritePaymentLog(
							isDskDefrredAdpExecuteSuccess,
							Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
							PaymentFileLogger.PaymentType.Dsk,
							PaymentFileLogger.PaymentProcessingType.ShippingReportForPostpaid,
							isDskDefrredAdpExecuteSuccess
								? ""
								: dskDefrredErrorMessage,
							new Dictionary<string, string>
							{
								{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
								{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
							});

						if (isDskDefrredAdpExecuteSuccess == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = dskDefrredErrorMessage;
						}
						else
						{
							this.SuccessMessage = this.SuccessMessage = LogCreator.CreateMessage(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
						}
						break;

					// Amaozon Pay売上処理
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_AMAZON_PAYMENT:
						paymentDetailType = OrderCommon.GetAmazonPayPaymentId();
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Amazon;
						var isSuccess = true;
						if (Constants.AMAZON_PAYMENT_CV2_ENABLED)
						{
							var charge = new AmazonCv2ApiFacade().CaptureCharge(
								(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
								(decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]);

							isSuccess = charge.Success && (new OrderService().UpdateCardTranId(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								charge.ChargeId,
								this.LoginOperatorName,
								UpdateHistoryAction.DoNotInsert,
								accessor) > 0);

							var error = AmazonCv2ApiFacade.GetErrorCodeAndMessage(charge);

							var amazonApiErrorMessage = PaymentFileLogger.PaymentProcessingType.SalesConfirmation.ToText()
								+ "\t" + (string)order[Constants.FIELD_ORDER_ORDER_ID] + "\t"
								+ LogCreator.CreateErrorMessage(error.ReasonCode, error.Message);

							// ログ格納処理
							PaymentFileLogger.WritePaymentLog(
								isSuccess,
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
								PaymentFileLogger.PaymentType.Amazon,
								PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
								isSuccess
									? string.Empty
									: amazonApiErrorMessage,
								new Dictionary<string, string>
								{
									{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
									{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
								});

							if (isSuccess)
							{
								order[Constants.FIELD_ORDER_CARD_TRAN_ID] = charge.ChargeId;
								this.SuccessMessage = LogCreator.CreateMessage(
									(string)order[Constants.FIELD_ORDER_ORDER_ID],
									(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
							}
							else
							{
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
								this.ApiErrorMessage = amazonApiErrorMessage;
							}
						}
						else
						{
							var response = AmazonApiFacade.Capture(
							(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
							(decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT],
							(string)order[Constants.FIELD_ORDER_ORDER_ID] + "_" + DateTime.Now.ToString("HHmmssfff"));

							isSuccess = response.GetSuccess() && (new OrderService().UpdateCardTranId(
									(string)order[Constants.FIELD_ORDER_ORDER_ID],
									response.GetCaptureId(),
									this.LoginOperatorName,
									UpdateHistoryAction.DoNotInsert,
								accessor) > 0);

							var amazonApiErrorMessage = PaymentFileLogger.PaymentProcessingType.SalesConfirmation.ToText()
								+ "\t" + (string)order[Constants.FIELD_ORDER_ORDER_ID] + "\t"
								+ LogCreator.CreateErrorMessage(response.GetErrorCode(), response.GetErrorMessage());

							// ログ格納処理
							PaymentFileLogger.WritePaymentLog(
								isSuccess,
								Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT,
								PaymentFileLogger.PaymentType.Amazon,
								PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
								isSuccess
									? ""
									: amazonApiErrorMessage,
								new Dictionary<string, string>
								{
									{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
									{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
								});

							if (isSuccess)
							{
								order[Constants.FIELD_ORDER_CARD_TRAN_ID] = response.GetCaptureId();
								this.SuccessMessage = this.SuccessMessage = LogCreator.CreateMessage(
									(string)order[Constants.FIELD_ORDER_ORDER_ID],
									(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
							}
							else
							{
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
								this.ApiErrorMessage = amazonApiErrorMessage;
							}
						}
						break;

					// PayPal売上処理
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYPAL_PAYMENT:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.PayPal;

						if (Constants.PAYPAL_PAYMENT_METHOD == w2.App.Common.Constants.PayPalPaymentMethod.AUTH)
						{
							var paypalResult = PayPalUtility.Payment.Sales(
								(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
								(decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]);

							var paypalErrorMessage = PaymentFileLogger.PaymentProcessingType.SalesConfirmation.ToText()
								+ "\t" + (string)order[Constants.FIELD_ORDER_ORDER_ID] + "\t"
								+ string.Join(",", paypalResult.Errors);

							// ログ格納処理
							PaymentFileLogger.WritePaymentLog(
								paypalResult.IsSuccess(),
								Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL,
								PaymentFileLogger.PaymentType.PayPal,
								PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
								paypalResult.IsSuccess()
									? ""
									: paypalErrorMessage,
								new Dictionary<string, string>
								{
									{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
									{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID,(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
								});

							if (paypalResult.IsSuccess() == false)
							{
								this.ApiErrorMessage = paypalErrorMessage;
							}
							else
							{
								this.SuccessMessage = this.SuccessMessage = LogCreator.CreateMessage(
									(string)order[Constants.FIELD_ORDER_ORDER_ID],
									(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
							}
						}
						break;

					// TriLink後付款出荷報告処理
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_TRILINK_AFTERPAY_SHIP:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.TriLink;
						var triLinkRequest = new TriLinkAfterPayShipmentCompleteRequest(
							Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_DELIVERY_COMPANY_CODE,
							StringUtility.ToEmpty(order[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO]),
							StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]));

						var triLinkResponse = TriLinkAfterPayApiFacade.CompleteShipment(triLinkRequest);

						if (triLinkResponse.Code != "201")
						{
							var errorCode = string.Empty;
							var errorResponseMessage = string.Empty;
							if (triLinkResponse.Code == "400")
							{
								errorCode = triLinkResponse.Errors.FirstOrDefault().Reason;
								errorResponseMessage = triLinkResponse.Errors.FirstOrDefault().Message;
							}
							else if ((triLinkResponse.Code == "401") || (triLinkResponse.Code == "404"))
							{
								errorCode = triLinkResponse.ErrorCode;
								errorResponseMessage = triLinkResponse.Message;
							}
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;

							var errorMessage = LogCreator.CreateErrorMessage(
								errorCode,
								errorResponseMessage,
								PaymentFileLogger.PaymentProcessingType.ShippingReportForPostpaid.ToText());

							this.ApiErrorMessage = errorMessage;

							// ログ格納処理
							PaymentFileLogger.WritePaymentLog(
								false,
								Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY,
								PaymentFileLogger.PaymentType.TriLink,
								PaymentFileLogger.PaymentProcessingType.ShippingReport,
								errorMessage,
								new Dictionary<string, string>
								{
									{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
									{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
								});
						}
						else
						{
							var successMessage = LogCreator.CreateMessageWithOrderIdAndShippingCheckNo(
								StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]),
								"");
							this.SuccessMessage = this.SuccessMessage = LogCreator.CreateMessage(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);

							// ログ格納処理
							PaymentFileLogger.WritePaymentLog(
								true,
								Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY,
								PaymentFileLogger.PaymentType.TriLink,
								PaymentFileLogger.PaymentProcessingType.ShippingReport,
								successMessage,
								new Dictionary<string, string>
								{
									{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
									{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
								});
						}
						break;

					// NP後払い出荷報告 翌月払い
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_NP_AFTERPAY_SHIP:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.NpAfterPay;

						// Check Bill Issued Date
						var billIssuedDate = string.Empty;
						var hasError = false;
						var errorMessageNPAfterPay = NPAfterPayUtility.CheckBillIssuedDate(
							orderModel,
							out billIssuedDate,
							accessor);
						if (string.IsNullOrEmpty(errorMessageNPAfterPay) == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
						}
						else
						{
							// Execute Shipment
							var requestShipment = NPAfterPayUtility.CreateShipmentRequestData(
								StringUtility.ToEmpty(order[Constants.FIELD_ORDER_CARD_TRAN_ID]),
								StringUtility.ToEmpty(order[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO]),
								DeliveryCompanyUtil.GetDeliveryCompanyType(
									StringUtility.ToEmpty(order[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID]),
									StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN])),
								billIssuedDate);
							var resultShipment = NPAfterPayApiFacade.ShipmentOrder(requestShipment);
							if (resultShipment.IsSuccess == false)
							{
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
								errorMessageNPAfterPay = resultShipment.GetApiErrorMessage();
								hasError = true;
							}
						}

						// ログ格納処理
						PaymentFileLogger.WritePaymentLog(
							(hasError == false),
							Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY,
							PaymentFileLogger.PaymentType.NpAfterPay,
							PaymentFileLogger.PaymentProcessingType.ShippingReport,
							hasError
								? errorMessageNPAfterPay
								: string.Empty,
							new Dictionary<string, string>
							{
								{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]) },
								{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]) },
							});
						break;

					// Paidy 売上処理
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAIDY_PAYMENT:
						if (this.NeedsUpdateOrderReturn == false)
						{
							var errorMessage = string.Empty;
							switch (Constants.PAYMENT_PAIDY_KBN)
							{
								case Constants.PaymentPaidyKbn.Direct:
									errorMessage = CapturePaidyPayment(order, accessor);
									break;

								case Constants.PaymentPaidyKbn.Paygent:
									errorMessage = GetPaidySettlement(order, accessor);
									break;
							}
							var idDictionary = new Dictionary<string, string>
							{
								{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]) },
								{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]) },
							};

							var success = string.IsNullOrEmpty(errorMessage);
							PaymentFileLogger.WritePaymentLog(
								success,
								StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]),
								PaymentFileLogger.PaymentType.Paidy,
								PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
								errorMessage,
								idDictionary);

							if (success)
							{
								this.SuccessMessage = LogCreator.CreateMessage(
									(string)order[Constants.FIELD_ORDER_ORDER_ID],
									(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
							}
							else
							{
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
								this.ApiErrorMessage = errorMessage;
							}
						}
						break;

					// atone翌月払い
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATONE_PAYMENT:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_ATONE;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Atone;
						var userService = new UserService();
						var orderService = new OrderService();
						var orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);

						// Get Token => Execute Sale Process
						var user = userService.Get(StringUtility.ToEmpty(order[Constants.FIELD_ORDER_USER_ID]), accessor);
						var tokenId = ((user != null)
							? user.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_ATONE_TOKEN_ID]
							: string.Empty);
						if (string.IsNullOrEmpty(tokenId) == false)
						{
							// Update Transaction Options On Api
							var orderInfo = orderService.Get(orderId, accessor);
							orderInfo.IsOrderSalesSettled = true;
							AtoneResponse atoneResponse = null;
							var errorMessageWhenCallApi = string.Empty;
							if (orderInfo.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST)
							{
								var atoneRequest = AtonePaymentApiFacade.CreateDataAtoneAuthoriteForReturnExchange(orderInfo, orderInfo);
								atoneRequest.Data.AuthenticationToken = tokenId;
								atoneRequest.Data.UpdatedTransactions = new[] { orderInfo.CardTranId };
								atoneResponse = AtonePaymentApiFacade.CreatePayment(atoneRequest);
								errorMessageWhenCallApi = atoneResponse.ErrorMessageWhenCallApi;
							}
							if (string.IsNullOrEmpty(errorMessageWhenCallApi))
							{
								// Check Order Sale: If Not Sale Then Execute Sale Process
								var cardTranId = (atoneResponse != null)
									? atoneResponse.TranId
									: orderInfo.CardTranId;
								atoneResponse = AtonePaymentApiFacade.GetPayment(cardTranId);
								errorMessageWhenCallApi = atoneResponse.ErrorMessageWhenCallApi;

								if (string.IsNullOrEmpty(errorMessageWhenCallApi)
									&& string.IsNullOrEmpty(atoneResponse.SalesSettledDatetime))
								{
									atoneResponse = AtonePaymentApiFacade.CapturePayment(
										tokenId,
										cardTranId);
									cardTranId = atoneResponse.TranId;
									errorMessageWhenCallApi = atoneResponse.ErrorMessageWhenCallApi;
								}
								order[Constants.FIELD_ORDER_CARD_TRAN_ID] = cardTranId;
							}

							// Update Transaction Id To Card Tran Id in DataBase
							if (string.IsNullOrEmpty(errorMessageWhenCallApi) == false)
							{
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
								this.ApiErrorMessage = errorMessageWhenCallApi;

								PaymentFileLogger.WritePaymentLog(
									false,
									paymentDetailType,
									accountSettlementCompanyName,
									PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
									"Error message API Atone",
									new Dictionary<string, string>
									{
										{ Constants.FIELD_ORDER_ORDER_ID, orderId },
										{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] }
									});
								break;
							}
							if (atoneResponse.IsAuthorizationSuccess == false)
							{
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
								this.ApiErrorMessage = atoneResponse.AuthorizationResultNgReasonMessage;

								PaymentFileLogger.WritePaymentLog(
									false,
									paymentDetailType,
									accountSettlementCompanyName,
									PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
									"Error message API Atone",
									new Dictionary<string, string>
									{
										{ Constants.FIELD_ORDER_ORDER_ID, orderId },
										{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] }
									});
								break;
							}
							orderService.UpdateCardTranId(
								orderId,
								StringUtility.ToEmpty(order[Constants.FIELD_ORDER_CARD_TRAN_ID]),
								this.LoginOperatorName,
								UpdateHistoryAction.DoNotInsert,
								accessor);
						}
						break;

					// aftee翌月払い
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_AFTEE_PAYMENT:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Aftee;
						userService = new UserService();
						orderService = new OrderService();
						orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
						// Get Token => Execute Sale Process
						user = userService.Get(StringUtility.ToEmpty(order[Constants.FIELD_ORDER_USER_ID]), accessor);
						tokenId = ((user != null)
							? user.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_AFTEE_TOKEN_ID]
							: string.Empty);
						if (string.IsNullOrEmpty(tokenId) == false)
						{
							// Update Transaction Options On Api
							var orderInfo = orderService.Get(orderId, accessor);
							orderInfo.IsOrderSalesSettled = true;
							AfteeResponse afteeResponse = null;
							var errorMessageWhenCallApi = string.Empty;
							if (orderInfo.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST)
							{
								var afteeRequest =
									AfteePaymentApiFacade.CreateDataAfteeAuthoriteForReturnExchange(orderInfo, orderInfo);
								afteeRequest.Data.AuthenticationToken = tokenId;
								afteeRequest.Data.UpdatedTransactions = orderInfo.CardTranId;
								afteeResponse = AfteePaymentApiFacade.CreatePayment(afteeRequest);
								errorMessageWhenCallApi = afteeResponse.Message;
							}
							if (string.IsNullOrEmpty(errorMessageWhenCallApi))
							{
								// Check Order Sale: If Not Sale Then Execute Sale Process
								var cardTranId = (afteeResponse != null)
									? afteeResponse.TranId
									: orderInfo.CardTranId;
								afteeResponse = AfteePaymentApiFacade.GetPayment(cardTranId);
								errorMessageWhenCallApi = afteeResponse.Message;

								if (string.IsNullOrEmpty(errorMessageWhenCallApi)
									&& string.IsNullOrEmpty(afteeResponse.SalesSettledDatetime))
								{
									afteeResponse = AfteePaymentApiFacade.CapturePayment(
										tokenId,
										cardTranId);
									cardTranId = afteeResponse.TranId;
									errorMessageWhenCallApi = afteeResponse.Message;
								}
								order[Constants.FIELD_ORDER_CARD_TRAN_ID] = cardTranId;
							}

							// Update Transaction Id To Card Tran Id in DataBase
							isSuccess = string.IsNullOrEmpty(errorMessageWhenCallApi);
							if (isSuccess)
							{
								orderService.UpdateCardTranId(
									orderId,
									StringUtility.ToEmpty(order[Constants.FIELD_ORDER_CARD_TRAN_ID]),
									this.LoginOperatorName,
									UpdateHistoryAction.DoNotInsert,
									accessor);
							}
							else
							{
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
								this.ApiErrorMessage = errorMessageWhenCallApi;

								// ログ格納処理
								PaymentFileLogger.WritePaymentLog(
									false,
									paymentDetailType,
									accountSettlementCompanyName,
									PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
									errorMessageWhenCallApi,
									new Dictionary<string, string>
									{
										{Constants.FIELD_ORDER_ORDER_ID, orderId},
										{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
									});
							}
						}
						break;

					// LINE PAY翌月払い
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_LINE_PAYMENT:
						var orderID = (string)order[Constants.FIELD_ORDER_ORDER_ID];
						var tranId = (string)order[Constants.FIELD_ORDER_CARD_TRAN_ID];

						if ((string.IsNullOrEmpty(tranId) == false)
							&& ((string)order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY))
						{
							var lineResponse = LinePayApiFacade.CapturePayment(
								tranId,
								(decimal)order[Constants.FIELD_ORDER_SETTLEMENT_AMOUNT],
								(string)order[Constants.FIELD_ORDER_SETTLEMENT_CURRENCY],
								new LinePayApiFacade.LinePayLogInfo(order));
							if (lineResponse.IsSuccess)
							{
								new OrderService().UpdateCardTranId(
									orderID,
									lineResponse.Info.TransactionId,
									this.LoginOperatorName,
									UpdateHistoryAction.DoNotInsert,
									accessor);

								order[Constants.FIELD_ORDER_CARD_TRAN_ID] = lineResponse.Info.TransactionId;
							}
							else
							{
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							}

							var idKeyAndValueDictionary = new Dictionary<string, string>
							{
								{ Constants.FIELD_ORDER_ORDER_ID, orderID },
								{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] }
							};

							// ログ格納処理
							PaymentFileLogger.WritePaymentLog(
								lineResponse.IsSuccess,
								Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
								PaymentFileLogger.PaymentType.LinePay,
								PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
								lineResponse.ReturnMessage,
								idKeyAndValueDictionary);
						}
						else
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.NoUpdate;
						}
						break;

					// EcPay
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_EC_PAYMENT:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.EcPay;
						orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
						orderService = new OrderService();
						var orderInfomation = orderService.Get(orderId, accessor);
						if ((orderInfomation != null)
							&& (orderInfomation.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT))
						{
							var requestCapture = ECPayUtility.CreateRequestForCancelRefundAndCapturePayment(orderInfomation);
							var responseCapture = ECPayApiFacade.CancelRefundAndCapturePayment(requestCapture);

							var paymentOrderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
							var idDictionary = new Dictionary<string, string>
							{
								{ Constants.FIELD_ORDER_ORDER_ID, orderId },
								{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId },
							};
							// ログ格納処理
							PaymentFileLogger.WritePaymentLog(
								responseCapture.IsSuccess,
								paymentDetailType,
								accountSettlementCompanyName,
								PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
								responseCapture.IsSuccess
									? string.Empty
									: responseCapture.ReturnMessage,
								idDictionary);

							if (responseCapture.IsSuccess)
							{
								orderService.UpdateCardTranId(
									StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
									responseCapture.TradeNo,
									this.LoginOperatorName,
									UpdateHistoryAction.DoNotInsert,
									accessor);

								this.SuccessMessage = LogCreator.CreateMessage(
									orderId,
									paymentOrderId);
							}
							else
							{
								this.ApiErrorMessage = responseCapture.ReturnMessage;
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							}
						}
						else
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.NoUpdate;
						}
						break;

					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CVS_DEF_INVOICE_REISSUE:
						var service = new OrderService();
						var orderGmoInformation = service.Get(
							StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
							accessor);
						var extendStatusNo = int.Parse(Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CVS_DEF_INVOICE_REISSUE);

						if ((orderGmoInformation.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
							&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Gmo))
						{
							var requestGmo = new GMOReissue.GmoRequestReissue(orderGmoInformation);
							var responseGmo = new GmoDeferredApiFacade().Reissue(requestGmo);

							if (responseGmo.IsSuccess)
							{
								// Be sure to turn off the extended status after output
								service.UpdateReissueInvoiceStatusAndPaymentMemo(
									orderGmoInformation.OrderId,
									extendStatusNo,
									responseGmo.GetOrderPaymentMemoForReissue(orderGmoInformation.PaymentOrderId),
									this.LoginOperatorName,
									UpdateHistoryAction.DoNotInsert,
									accessor);
							}
							else
							{
								// エラーの場合は更新NG
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							}
						}
						else
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.NoUpdate;
						}
						break;

					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATOBARAICOM_CVS_DEF_SHIP:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Atobaraicom;

						var atobaraicomOrderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
						var atobaraicomPaymentOrderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
						var atobaraicomShippingCheckNo = StringUtility.ToEmpty(order[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO]);
						var deliveryCompanyInfo = new DeliveryCompanyService().Get(
							StringUtility.ToEmpty(order[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID]));
						var invoiceBundleFlg = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG]);
						var atobaraicomShippingDeliveryId = (deliveryCompanyInfo != null)
							? deliveryCompanyInfo.DeliveryCompanyTypePostPayment
							: string.Empty;
						var atobaraicomShippingResponse = new AtobaraicomShippingRegistrationApi().ExecShippingRegistration(
							atobaraicomPaymentOrderId,
							atobaraicomShippingCheckNo,
							atobaraicomShippingDeliveryId,
							invoiceBundleFlg);
						var isAtobaraicomShippingSuccess = atobaraicomShippingResponse.IsSuccess;
						var apiMessages = atobaraicomShippingResponse.ApiMessages;

						// ログ格納処理
						PaymentFileLogger.WritePaymentLog(
							isAtobaraicomShippingSuccess,
							paymentDetailType,
							accountSettlementCompanyName,
							PaymentFileLogger.PaymentProcessingType.ShippingReportForPostpaid,
							isAtobaraicomShippingSuccess
								? string.Empty
								: apiMessages,
							new Dictionary<string, string>
							{
								{ Constants.FIELD_ORDER_ORDER_ID, atobaraicomOrderId },
								{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, atobaraicomPaymentOrderId }
							});

						if (isAtobaraicomShippingSuccess == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = apiMessages;
						}
						else
						{
							this.SuccessMessage = this.SuccessMessage = LogCreator.CreateMessage(
								atobaraicomOrderId,
								atobaraicomPaymentOrderId);
						}
						break;

					// NewebPay
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_NEWEB_PAYMENT:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.NewebPay;
						orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
						orderService = new OrderService();
						var orderInformation = orderService.Get(orderId, accessor);
						var isSaleOrderNewebpay = (((orderInformation.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
								|| (orderInformation.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED))
							&& ((orderInformation.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP)
								|| (orderInformation.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL)));

						if ((orderInformation != null)
							&& (isSaleOrderNewebpay == false)
							&& (orderInformation.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT))
						{
							var requestCapture = NewebPayUtility.CreateCancelRefundCaptureRequest(orderInformation, false);
							var responseCapture = NewebPayApiFacade.SendCancelRefundAndCaptureOrder(requestCapture, false);

							var paymentOrderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
							var idDictionary = new Dictionary<string, string>
							{
								{ Constants.FIELD_ORDER_ORDER_ID, orderId },
								{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId },
							};
							// ログ格納処理
							PaymentFileLogger.WritePaymentLog(
								responseCapture.IsSuccess,
								paymentDetailType,
								accountSettlementCompanyName,
								PaymentFileLogger.PaymentProcessingType.SalesConfirmation,
								responseCapture.IsSuccess
									? string.Empty
									: responseCapture.Response.Message,
								idDictionary);

							if (responseCapture.IsSuccess)
							{
								orderService.UpdateCardTranId(
									StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
									responseCapture.Response.TradeNo,
									this.LoginOperatorName,
									UpdateHistoryAction.DoNotInsert,
									accessor);

								this.SuccessMessage = LogCreator.CreateMessage(
									orderId,
									paymentOrderId);
							}
							else
							{
								this.ApiErrorMessage = responseCapture.Response.Message;
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							}
						}
						else
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.NoUpdate;
						}
						break;

					// PayPay
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYPAY_PAYMENT:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.PayPay;
						orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
						orderService = new OrderService();
						var orderInfoPaypay = orderService.Get(orderId, accessor);
						var isSaleOrderPaypay = (((orderInfoPaypay.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
								|| (orderInfoPaypay.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED))
							&& ((orderInfoPaypay.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP)
								|| (orderInfoPaypay.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL)));

						if (isSaleOrderPaypay == false)
						{
							switch (Constants.PAYMENT_PAYPAY_KBN)
							{
								case Constants.PaymentPayPayKbn.SBPS:
									var api = new PaymentSBPSPaypaySaleApi();
									var responseCapture = api.Exec(
										orderInfoPaypay.CardTranId,
										orderInfoPaypay.OrderPriceTotal);
									if (responseCapture == false)
									{
										this.ApiErrorMessage = api.ResponseData.ResErrMessages;
										actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
									}
									break;

								case Constants.PaymentPayPayKbn.GMO:
									var capture = new PaypayGmoFacade().CapturePayment(orderInfoPaypay);
									if (capture.Result == Results.Failed)
									{
										this.ApiErrorMessage = capture.ErrorMessage;
										actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
									}
									break;

								case Constants.PaymentPayPayKbn.VeriTrans:
									var capturePayment = new PaymentVeritransPaypay().Capture(orderInfoPaypay);
									if (capturePayment.Mstatus != VeriTransConst.RESULT_STATUS_OK)
									{
										this.ApiErrorMessage = LogCreator.CreateErrorMessage(capturePayment.VResultCode, capturePayment.MerrMsg);
										actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
									}
									break;
							}
						}
						else
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.NoUpdate;
						}
						break;

					// Rakuten
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_RAKUTEN_CREDITCARD_PAYMENT:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Rakuten;

						if (Constants.PAYMENT_RAKUTEN_CREDIT_PAYMENT_METHOD == Constants.RakutenPaymentType.AUTH)
						{
							var rakutenApiExecResult = RakutenApiFacade.Capture(new RakutenCaptureRequest
							{
								PaymentId = (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]
							});

							if ((rakutenApiExecResult.ResultType == RakutenConstants.RESULT_TYPE_FAILURE)
								|| (rakutenApiExecResult.ResultType == RakutenConstants.RESULT_TYPE_PENDING))
							{
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
								this.ApiErrorMessage = LogCreator.CreateErrorMessage(
									rakutenApiExecResult.ErrorMessage,
									rakutenApiExecResult.ErrorCode);
							}
							else
							{
								this.SuccessMessage = LogCreator.CrateMessageWithCardTranId(
									(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
									string.Empty);
							}
						}
						break;

					// Boku売上処理
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_CARRIERBILLING_BOKU:
						var bokuOrderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
						var bokuPaymentOrderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
						var orderInfoBoku = DomainFacade.Instance.OrderService.Get(bokuOrderId, accessor);
						if (orderInfoBoku != null)
						{
							var productNames = string.Join(
								",",
								orderInfoBoku.Items.Select(item => item.ProductName));
							var charge = new PaymentBokuChargeApi().Exec(
								orderInfoBoku.SettlementCurrency,
								string.Empty,
								productNames,
								orderInfoBoku.OrderId,
								orderInfoBoku.PaymentOrderId,
								orderInfoBoku.SettlementAmount.ToString(),
								(orderInfoBoku.OrderTaxIncludedFlg == Constants.FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX),
								orderInfoBoku.RemoteAddr,
								orderInfoBoku.IsFixedPurchaseOrder,
								(orderInfoBoku.IsFixedPurchaseOrder && (orderInfoBoku.FixedPurchaseOrderCount > 1)),
								(orderInfoBoku.IsFixedPurchaseOrder ? orderInfoBoku.FixedPurchaseKbn : string.Empty),
								(orderInfoBoku.IsFixedPurchaseOrder ? orderInfoBoku.FixedPurchaseSetting1 : string.Empty));

							if (charge == null)
							{
								this.ApiErrorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
								break;
							}

							if (charge.IsSuccess)
							{
								order[Constants.FIELD_ORDER_CARD_TRAN_ID] = charge.ChargeId;
								this.SuccessMessage = LogCreator.CreateMessage(
									bokuOrderId,
									bokuPaymentOrderId);

								DomainFacade.Instance.OrderService.UpdateCardTranId(
									bokuOrderId,
									StringUtility.ToEmpty(order[Constants.FIELD_ORDER_CARD_TRAN_ID]),
									this.LoginOperatorName,
									UpdateHistoryAction.DoNotInsert,
									accessor);

								this.SuccessMessage = LogCreator.CreateMessage(
									bokuOrderId,
									bokuPaymentOrderId);
							}
							else if (charge.ChargeStatus != BokuConstants.CONST_BOKU_CHARGE_STATUS_SUCCESS)
							{
								this.ApiErrorMessage = (charge.ChargeStatus == BokuConstants.CONST_BOKU_CHARGE_STATUS_IN_PROGRESS)
									? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_BOKU_PAYMENT_PROCESS_TIME_OUT)
									: charge.Result.Message;
								actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							}
						}
						else
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.NoUpdate;
						}
						break;

					// スコア後払い出荷報告処理
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SCORE_CVS_DEF_SHIP:
						paymentDetailType = Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF;
						accountSettlementCompanyName = PaymentFileLogger.PaymentType.Score;

						var scoreFacade = new ScoreApiFacade();
						var scoreDeliveryRequest = new ScoreDeliveryRegisterRequest
						{
							Transaction =
							{
								NissenTransactionId = StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDER_CARD_TRAN_ID]),
								ShopTransactionId = StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]),
								BilledAmount = ((decimal)drvOrder[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]).ToPriceString()
							},
							PdRequest =
							{
								Pdcompanycode = PaymentScoreDeferredDeliveryServiceCode.GetDeliveryServiceCode(
									DeliveryCompanyUtil.GetDeliveryCompanyType(
										(string)drvOrder[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID],
										(string)drvOrder[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN])),
								Slipno = StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO]),
								Address1 = StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1]),
								Address2 = StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2]),
								Address3 = StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3])
									+ StringUtility.ToEmpty(order[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4])
							}
						};

						var scoreResult = scoreFacade.DeliveryRegister(scoreDeliveryRequest);
						var isScoreResultOk = scoreResult.Result == ScoreResult.Ok.ToText();

						var scoreErrorMessage = (isScoreResultOk == false)
							? string.Join(
								"\t",
								scoreResult.Errors.ErrorList
									.Select(
										x => LogCreator.CreateErrorMessage(
											errorCode: x.ErrorCode,
											errorMessage: x.ErrorMessage))
									.ToArray())
							: string.Empty;

						// ログ格納処理
						PaymentFileLogger.WritePaymentLog(
							success: isScoreResultOk,
							paymentDetailType: Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
							accountSettlementCompanyName: PaymentFileLogger.PaymentType.Score,
							processingContent: PaymentFileLogger.PaymentProcessingType.ShippingReport,
							externalPaymentCooperationLog: scoreErrorMessage,
							idKeyAndValueDictionary: new Dictionary<string, string>
							{
								{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
								{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] }
							});

						if (isScoreResultOk == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = scoreErrorMessage;
						}
						else
						{
							this.SuccessMessage = this.SuccessMessage = LogCreator.CreateMessage(
								orderId: (string)order[Constants.FIELD_ORDER_ORDER_ID],
								paymentOrderId: (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
						}
						break;

					// ベリトランス後払い出荷報告処理
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_VERITRANS_CVS_DEF_SHIP:
						var requestData = new ScoreatpayCaptureRequestDto
						{
							OrderId = StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]),
							PdCompanyCode = PaymentVeritransDeferredDeliveryServiceCode.GetDeliveryServiceCode(
								DeliveryCompanyUtil.GetDeliveryCompanyType(
									StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID]),
									StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]))),
							SlipNo = StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO]),
							DeliveryId = StringUtility.ToEmpty(drvOrder[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO]),
						};

						var veritransResult = new PaymentVeritransCvsDef().DeliveryRegister(requestData);
						var isVeritransResultOk = veritransResult.Mstatus == VeriTransConst.RESULT_STATUS_OK;

						var veritransErrorMessage = string.Empty;
						if (isVeritransResultOk == false)
						{
							veritransErrorMessage = veritransResult.Errors != null
								? string.Join(
									"\t",
									veritransResult.Errors
										.Select(
											x => LogCreator.CreateErrorMessage(
												x.ErrorCode,
												x.ErrorMessage))
										.ToArray())
								: string.Format("{0}：{1}", veritransResult.VResultCode, veritransResult.MerrMsg);
						}

						PaymentFileLogger.WritePaymentLog(
							isVeritransResultOk,
							Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
							PaymentFileLogger.PaymentType.VeriTrans,
							PaymentFileLogger.PaymentProcessingType.ShippingReport,
							veritransErrorMessage,
							new Dictionary<string, string>
							{
								{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
								{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] }
							});

						if (isVeritransResultOk == false)
						{
							actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
							this.ApiErrorMessage = veritransErrorMessage;
						}
						else
						{
							this.SuccessMessage = LogCreator.CreateMessage(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
						}
						break;
				}

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForOrder(
						(string)order[Constants.FIELD_ORDER_ORDER_ID],
						this.LoginOperatorName,
						accessor);
				}
			}
			else if (this.NeedsExecExternalPaymentAction && ((decimal)order[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT] == 0))
			{
				// 最終請求金額が0円の場合はエラーとする
				AppLogger.WriteError(
					PaymentFileLogger.PaymentProcessingType.FinalBillAmount + "0円"
					+ (string)order[Constants.FIELD_ORDER_ORDER_ID]);

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					paymentDetailType,
					accountSettlementCompanyName,
					PaymentFileLogger.PaymentProcessingType.FinalBillAmount,
					PaymentFileLogger.PaymentProcessingType.FinalBillAmount + "0円",
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID]},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]}
					});
				actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.UpdateNG;
			}
			else
			{
				actionResult.ResultExternalPaymentAction = OrderCommon.ResultKbn.NoUpdate;
			}

			// 結果追加
			return (actionResult.ResultExternalPaymentAction != OrderCommon.ResultKbn.UpdateNG);
		}

		/// <summary>
		/// 指定配送希望日から、配送先情報を更新
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="orderShippingDate">配送希望日</param>
		/// <param name="shipping">注文配送先</param>
		/// <returns>注文配送先</returns>
		private OrderShippingModel CalculateShippingDate(string orderShippingDate, OrderShippingModel shipping)
		{
			// 指定された配送希望日を配送先にセット
			shipping.ShippingDate = Validator.IsDate(orderShippingDate)
				? DateTime.Parse(orderShippingDate)
				: (DateTime?)null;

			if (this.NeedsCalculateScheduledShippingDate == false) return shipping;

			var scheduledShippingDate = OrderCommon.CalculateScheduledShippingDateBasedOnToday((string)this.Setting[Constants.FIELD_SHOPOPERATOR_SHOP_ID], shipping);

			shipping.ScheduledShippingDate = scheduledShippingDate;
			return shipping;
		}

		/// <summary>
		/// 値取得
		/// </summary>
		/// <param name="field">キー</param>
		/// <returns>値</returns>
		public string GetValue(string field)
		{
			if (this.Setting == null)
			{
				return null;
			}

			return StringUtility.ToEmpty(this.Setting[field]);
		}

		/// <summary>
		/// 値取得(ワークフロー種別指定)
		/// </summary>
		/// <param name="field">キー</param>
		/// <param name="workflowType">ワークフロー種別</param>
		/// <returns>値</returns>
		public string GetValue(string field, WorkflowTypes workflowType)
		{
			if ((this.Setting == null) || (this.WorkflowType != workflowType))
			{
				return null;
			}

			return StringUtility.ToEmpty(this.Setting[field]);
		}

		/// <summary>
		/// 実売上連動処理前決済取引ID更新処理
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="cardTranId">決済連携ID</param>
		/// <param name="hasDigitalContents">デジタルコンテンツありフラグ</param>
		/// <param name="externalPaymentActionValue">外部決済連携処理区分</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>true : 連動処理可能状態 / false : 連動処理不可状態</returns>
		public bool UpdateCardTranIdBeforeRealSales(
			string orderId,
			string cardTranId,
			bool hasDigitalContents,
			string externalPaymentActionValue,
			UpdateHistoryAction updateHistoryAction)
		{
			Constants.PaymentCreditCardPaymentMethod? paymentMethod = null;
			switch (externalPaymentActionValue)
			{
				case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ZEUS_CREDITCARD_PAYMENT:
					paymentMethod = hasDigitalContents ? Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD;
					break;

				case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_CREDITCARD_SALES:
					paymentMethod = hasDigitalContents ? Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD;
					break;

				case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ESCOTT_CREDITCARD_SALES:
					paymentMethod = hasDigitalContents ? Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD;
					break;
			}

			bool canUpdate = true;
			if (paymentMethod != null)
			{
				// 与信後決済の場合のみ
				if (paymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
				{
					if (cardTranId == "")
					{
						new OrderService().UpdateCardTranId(
							orderId,
							Constants.FLG_REALSALES_TEMP_TRAN_ID,
							this.LoginOperatorName,
							updateHistoryAction);
					}
					else
					{
						// 与信後決済で決済取引IDが空以外の場合のみ更新不可
						canUpdate = false;
					}
				}
			}
			return canUpdate;
		}

		/// <summary>
		/// Get parameters action for history
		/// </summary>
		/// <param name="targetOrderId">Target OrderId</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <param name="isDigitalContents">デジタルコンテンツか</param>
		/// <returns>Parameters</returns>
		private Hashtable GetWorkflowActionForHistory(string targetOrderId, SqlAccessor accessor, bool isDigitalContents)
		{
			Hashtable actionInput = new Hashtable();
			List<string> actionOrder = new List<string>();
			List<string> actionOrderItem = new List<string>();

			if (this.NeedsUpdateOrderStatus) actionOrder.AddRange(OrderHistory.GetOrderStatusAction(this.OrderStatusChangeValue));

			if (this.NeedsUpdateProductRealStock)
			{
				switch (this.ProductRealStockChangeValue)
				{
					// 実在庫引当処理
					case Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_RESERVED_STCOK:
						actionOrder.Add(Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS);

						actionOrderItem.Add(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO);
						actionOrderItem.Add(Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED);
						break;

					// 実在庫出荷処理
					case Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_FORWARD_STCOK:
						actionOrder.Add(Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS);

						actionOrderItem.Add(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO);
						actionOrderItem.Add(Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_SHIPPED);
						break;

					// 実在庫戻し処理
					case Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_CANCEL_REALSTCOK:
						actionOrder.Add(Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS);

						actionOrderItem.Add(Constants.FIELD_ORDERITEM_ORDER_ITEM_NO);
						actionOrderItem.Add(Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED);
						break;
				}
			}

			if (this.NeedsUpdatePaymentStatus) actionOrder.AddRange(OrderHistory.GetOrderPaymentStatusAction());

			if (this.NeedsExecExternalPaymentAction)
			{
				switch (this.ExternalPaymentActionValue)
				{
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ZEUS_CREDITCARD_PAYMENT:
					case Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_CREDITCARD_SALES:
						if ((isDigitalContents ? Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD) == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
						{
							DataRowView order = OrderCommon.GetOrder(targetOrderId, accessor)[0];

							// 仮決済取引IDが格納済みかどうかの確認
							if ((string)order[Constants.FIELD_ORDER_CARD_TRAN_ID] == Constants.FLG_REALSALES_TEMP_TRAN_ID)
							{
								actionOrder.Add(Constants.FIELD_ORDER_CARD_TRAN_ID);
							}
						}
						break;
				}

				actionOrder.Add(Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS);
			}

			if (this.NeedsUpdateDemandStatus) actionOrder.AddRange(OrderHistory.GetOrderDemandStatusAction());

			// 領収書出力フラグ
			if (Constants.RECEIPT_OPTION_ENABLED && this.NeedsUpdateReceiptOutputFlg)
			{
				actionOrder.Add(Constants.FIELD_ORDER_RECEIPT_OUTPUT_FLG);
			}

			if (this.NeedsUpdateReturnExchangeStatus)
			{
				// handle when return order status complete
				var order = OrderCommon.GetOrder(targetOrderId, accessor)[0];

				if (((string)order[Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN] == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
					&& (((string)order[Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS] == Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_NOREPAYMENT)
						|| ((string)order[Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS] == Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_COMPLETE))
					&& ((string)order[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS] == Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE))
				{
					actionOrder.Add(Constants.FIELD_ORDER_ORDER_STATUS);
					actionOrder.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS);
					actionOrder.Add(Constants.FIELD_ORDER_DEMAND_STATUS);
				}

				actionOrder.AddRange(OrderHistory.GetOrderReturnExchangeStatusAction(this.ReturnExchangeStatusChangeValue));
			}

			if (this.NeedsUpdateRepaymentStatus) actionOrder.AddRange(OrderHistory.GetOrderRepaymentStatusAction());

			// Get Max extend status
			var extendStatuMax = ((Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX < this.NeedsUpdateOrderExtendStatus.Length)
				? Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX
				: this.NeedsUpdateOrderExtendStatus.Count());

			for (int index = 0; index < extendStatuMax; index++)
			{
				if (this.NeedsUpdateOrderExtendStatus[index])
				{
					actionOrder.Add(string.Format("{0}{1}", Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME, (index + 1).ToString()));
					actionOrder.Add(string.Format("{0}{1}", Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME, (index + 1).ToString()));
				}
			}

			if (actionOrder.Count > 0) actionInput.Add(Constants.TABLE_ORDER, actionOrder);
			if (actionOrderItem.Count > 0) actionInput.Add(Constants.TABLE_ORDERITEM, actionOrderItem);

			return actionInput;
		}

		/// <summary>
		/// Capturing Paidy payment
		/// </summary>
		/// <param name="order">Order data</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Error message</returns>
		private string CapturePaidyPayment(Hashtable order, SqlAccessor accessor)
		{
			// Get payment
			var paymentOrderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
			var paymentResponse = PaidyPaymentApiFacade.GetPayment(paymentOrderId);

			// Handle failed payment
			if (paymentResponse.HasError) return paymentResponse.GetApiErrorMessages();

			// Capture Payment
			var captureResponse = PaidyPaymentApiFacade.CapturePayment(paymentOrderId);
			if (captureResponse.HasError)
			{
				captureResponse.GetApiErrorMessages();
			}

			new OrderService().UpdateCardTranId(
				StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
				captureResponse.Payment.Captures[0].Id,
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// Success
			return string.Empty;
		}

		/// <summary>
		/// Get Paidy settlement
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Error message</returns>
		private string GetPaidySettlement(
			Hashtable order,
			SqlAccessor accessor = null)
		{
			var paymentId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_CARD_TRAN_ID]);
			var settlementResult = new PaygentApiFacade().PaidySettlement(paymentId);
			if (settlementResult.IsSuccess == false) return settlementResult.GetErrorMessage();

			return string.Empty;
		}

		/// <summary>
		/// Execute External Order Information
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Error Message</returns>
		private string ExecExternalOrderInfo(
			OrderModel order,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var request = ECPayUtility.CreateRequestDataForSendOrder(order, dontNotifyUpdate: true);
			var response = ECPayApiFacade.SendOrderInfo(request);

			if (response.IsExecRegisterApiSuccess == false)
			{
				var errorMessage = ECPayUtility.CreateMessage(order.OrderId, response.ErrorMessage);
				PaymentFileLogger.WritePaymentLog(
					false,
					order.PaymentName ?? string.Empty,
					PaymentFileLogger.PaymentType.Unknown,
					PaymentFileLogger.PaymentProcessingType.Unknown,
					errorMessage,
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, order.OrderId }
					});
				return errorMessage;
			}

			var responseReturnCode = response.GetResponseValue(ECPayConstants.PARAM_RTN_CODE);
			var responseReturnMsg = response.GetResponseValue(ECPayConstants.PARAM_RTN_MSG);
			var shippingStatusConvert = ValueText.GetValueText(
				Constants.TABLE_ORDERSHIPPING,
				string.Format(
					"{0}_{1}",
					Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CONVERT,
					request.LogisticsSubType),
				responseReturnCode);
			var orderShipping = order.Shippings[0];

			orderShipping.ShippingExternalDelivertyStatus = responseReturnCode;
			orderShipping.ShippingStatus = shippingStatusConvert;

			var isShippingStatusAbnormal = (shippingStatusConvert == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_STATUS_ABNORMAL);
			var responseStatusUpdateDate = response.GetResponseValue(ECPayConstants.PARAM_UPDATE_STATUS_DATE);
			var relationMemo = ECPayUtility.CreateRelationMemo(
				responseReturnCode,
				responseReturnMsg,
				string.Empty);

			if (isShippingStatusAbnormal == false)
			{
				orderShipping.ShippingStatusUpdateDate = (string.IsNullOrEmpty(responseStatusUpdateDate) == false)
					? DateTime.Parse(responseStatusUpdateDate)
					: (DateTime?)null;
			}

			var orderService = new OrderService();
			var orderId = response.GetResponseValue(ECPayConstants.PARAM_MERCHANT_TRADE_NO);
			var responseDeliveryTranId = response.GetResponseValue(ECPayConstants.PARAM_ALL_PAY_LOGISTICS_ID);

			orderService.UpdateDeliveryTransactionIdAndRelationMemo(
				orderId,
				responseDeliveryTranId,
				relationMemo,
				this.LoginOperatorName,
				updateHistoryAction,
				accessor);

			if (isShippingStatusAbnormal == false)
			{
				orderService.UpdateOnlineDeliveryStatus(
					orderId,
					Constants.FLG_ORDER_ONLINE_DELIVERY_STATUS_SETTLED,
					this.LoginOperatorName,
					updateHistoryAction,
					accessor);
			}

			orderService.UpdateOrderShipping(orderShipping, accessor);

			var successMessage = ECPayUtility.CreateLogMessageForSendOrderRegister(
				order.OrderId,
				responseDeliveryTranId,
				responseReturnCode,
				responseReturnMsg);

			PaymentFileLogger.WritePaymentLog(
				true,
				order.PaymentName ?? string.Empty,
				PaymentFileLogger.PaymentType.Unknown,
				PaymentFileLogger.PaymentProcessingType.Unknown,
				successMessage,
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, order.OrderId }
				});

			var result = (isShippingStatusAbnormal)
				? ECPayUtility.CreateMessage(order.OrderId, relationMemo)
				: string.Empty;
			return result;
		}

		/// <summary>
		/// ネクストエンジン受注アップロード用データ書き込み
		/// </summary>
		/// <param name="order">注文モデル</param>
		public string ExecSaveOrderForNextEngine(OrderModel order)
		{
			var isSuccess = true;
			var result = string.Empty;

			if (Constants.GIFTORDER_OPTION_ENABLED
				&& Constants.NE_OPTION_ENABLED
				&& (order.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON))
			{
				foreach (var shipping in order.Shippings)
				{
					shipping.DeliveryCompanyShippingTimeMessage = new DeliveryCompanyService()
						.Get(shipping.DeliveryCompanyId)
						.GetShippingTimeMessage(shipping.ShippingTime);

					var newOrder = order;
					foreach (var orderItem in shipping.Items)
					{
						isSuccess &= NextEngineOrderModel.SaveToCsv(newOrder, orderItem, shipping, shipping.OrderShippingNo, true);
					}
				}

				result = isSuccess
					? string.Empty
					: order.OrderId + "：注文情報保存失敗";
				return result;
			}

			order.Shippings[0].DeliveryCompanyShippingTimeMessage
				= new DeliveryCompanyService()
					.Get(order.Shippings[0].DeliveryCompanyId)
					.GetShippingTimeMessage(order.Shippings[0].ShippingTime);

			foreach (var orderItem in order.Items)
			{
				isSuccess &= NextEngineOrderModel.SaveToCsv(order, orderItem, order.Shippings[0]);
			}

			result = isSuccess
				? string.Empty
				: order.OrderId + "：注文情報保存失敗";
			return result;
		}

		/// <summary>
		/// ネクストエンジン受注ステータス更新
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <param name="updateNextengineOrder">更新ネクストエンジン受注情報</param>
		/// <param name="deliveryCompanies">配送会社配列</param>
		/// <param name="accessor">アクセサ</param>
		private string ExecUpdateOrderForNextEngine(
			string orderId,
			NEOrder[] updateNextengineOrder,
			DeliveryCompanyModel[] deliveryCompanies,
			SqlAccessor accessor)
		{
			if (updateNextengineOrder.Any(neOrder => (neOrder.OrderId == orderId)) == false) return "ネクストエンジン受注情報が見つかりませんでした。";

			var updateNeOrder = updateNextengineOrder.First(neOrder => (neOrder.OrderId == orderId));
			var errorMessage = string.Empty;

			// キャンセル処理
			if (updateNeOrder.CancelTypeId != NextEngineConstants.FLG_SEARCH_ORDER_ORDER_CANCEL_TYPE_ID_VALID_ORDER)
			{
				errorMessage = CancelProcessForNextEngine(orderId, accessor);

				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					return errorMessage;
				}
			}

			// 受注ステータス更新(出荷済み）
			if (updateNeOrder.OrderStatusId == NextEngineConstants.FLG_SEARCH_ORDER_ORDER_STATUS_ID_COMPLETED)
			{
				this.NeedsUpdateOrderStatus = true;
				this.OrderStatusChangeValue = Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_SHIP_COMP;
				ShippingProcessForNextEngine(orderId, updateNeOrder, deliveryCompanies, accessor);
			}

			this.OrderStatusStatement = OrderCommon.GetUpdateOrderStatusStatement(this.OrderStatusChangeValue);

			return string.Empty;
		}

		/// <summary>
		/// ネクストエンジン受注ステータス更新
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <param name="updateNextengineOrder">更新ネクストエンジン受注情報</param>
		/// <param name="deliveryCompanies">配送会社配列</param>
		/// <param name="accessor">アクセサ</param>
		/// <param name="orderOld">更新前注文</param>
		private string ExecUpdateOrderForNextEngineWithGift(
			string orderId,
			NEOrder[] updateNextengineOrder,
			DeliveryCompanyModel[] deliveryCompanies,
			SqlAccessor accessor,
			DataView orderOld = null)
		{
			var uncancel = 0;
			var allCount = 0;
			var shippingCompleteCount = 0;

			var canceledIdList = new List<string>();
			var shippingedIdList = new List<string>();
			var allList = new List<string>();

			foreach (DataRowView drv in orderOld)
			{
				var orderShippingNo = ((int)drv[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO]);
				var neOrderId = NextEngineOrderModel.CreateNeGiftOrderId(orderShippingNo, orderId);

				if (allList.Contains(neOrderId)) continue;

				allList.Add(neOrderId);
				allCount++;

				if (updateNextengineOrder.Any(neOrder => (neOrder.OrderId == neOrderId)) == false) return "ネクストエンジン受注情報が見つかりませんでした。";

				var updateNeOrder = updateNextengineOrder.First(neOrder => (neOrder.OrderId == neOrderId));

				// キャンセル計算
				if (updateNeOrder.CancelTypeId == NextEngineConstants.FLG_SEARCH_ORDER_ORDER_CANCEL_TYPE_ID_VALID_ORDER)
				{
					uncancel++;
				}
				else
				{
					canceledIdList.Add(neOrderId);
				}

				// 出荷処理
				if (updateNeOrder.OrderStatusId == NextEngineConstants.FLG_SEARCH_ORDER_ORDER_STATUS_ID_COMPLETED)
				{
					ShippingProcessForNextEngine(orderId, updateNeOrder, deliveryCompanies, accessor, orderShippingNo);
					shippingCompleteCount++;
					shippingedIdList.Add(neOrderId);
				}
			}

			FileLogger.Write("NextEngine", "受注ステータス取込注文ID：" + orderId);
			FileLogger.Write(
				"NextEngine",
				string.Format(
					"キャンセル件数：{0}件/{1}件、キャンセルID：{2}",
					(allCount - uncancel),
					allCount,
					string.Join("、", canceledIdList)));
			FileLogger.Write(
				"NextEngine",
				string.Format(
					"出荷件数：{0}件/{1}件、出荷ID：{2}",
					shippingCompleteCount,
					allCount,
					string.Join("、", shippingedIdList)));

			var errorMessage = string.Empty;

			// キャンセル処理
			if (uncancel == 0)
			{
				errorMessage = CancelProcessForNextEngine(orderId, accessor);

				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					return errorMessage;
				}
			}

			// 受注ステータス更新(出荷済み）
			if (allCount == shippingCompleteCount)
			{
				this.OrderStatusChangeValue = Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_SHIP_COMP;

				this.NeedsUpdateOrderStatus = true;
			}

			this.OrderStatusStatement = OrderCommon.GetUpdateOrderStatusStatement(this.OrderStatusChangeValue);

			return string.Empty;
		}

		/// <summary>
		/// ネクストエンジン用キャンセル処理
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>エラーメッセージ</returns>
		private string CancelProcessForNextEngine(string orderId, SqlAccessor accessor)
		{
			var cancelOrder = new OrderService().Get(orderId, accessor);

			// 注文キャンセル時に自動でキャンセル連携される決済を除外
			var isNeedCancelPayment = true;
			switch (cancelOrder.OrderPaymentKbn)
			{
				case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
				case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
				case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
				case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
					isNeedCancelPayment = false;
					break;
			}

			var errorMessage = isNeedCancelPayment
				? OrderCommon.CancelExternalCooperationPayment(cancelOrder, accessor)
				: string.Empty;

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				return errorMessage;
			}

			OrderCommon.UpdateExternalPaymentStatusesAndMemoForCancel(
				orderId,
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			this.NeedsUpdateOrderStatus = true;
			this.OrderStatusChangeValue = Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_ORDER_CANCELED;

			return errorMessage;
		}

		/// <summary>
		/// ネクストエンジン用出荷処理
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <param name="updateNeOrder"></param>
		/// <param name="deliveryCompanies">配送会社配列</param>
		/// <param name="accessor">アクセサ</param>
		/// <param name="orderShippingNo">注文配送先枝番</param>
		private void ShippingProcessForNextEngine(string orderId, NEOrder updateNeOrder, DeliveryCompanyModel[] deliveryCompanies, SqlAccessor accessor, int orderShippingNo = 1)
		{
			var orderService = new OrderService();

			// 配送伝票番号更新
			orderService.UpdateOrderShippingCheckNo(
				orderId,
				orderShippingNo,
				updateNeOrder.DeliveryCutFormId,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert,
				accessor);

			if (string.IsNullOrEmpty(updateNeOrder.DeliveryName) == false)
			{
				if (deliveryCompanies.Any(d => (d.DeliveryCompanyName == updateNeOrder.DeliveryName)))
				{
					var deliveryCompany = deliveryCompanies
						.First(d => (d.DeliveryCompanyName == updateNeOrder.DeliveryName));

					var orderShipping = orderService.GetShipping(
						orderId,
						orderShippingNo,
						accessor);

					orderShipping.DeliveryCompanyId = deliveryCompany.DeliveryCompanyId;
					orderShipping.DeliveryCompanyName = deliveryCompany.DeliveryCompanyName;

					orderService.UpdateOrderShipping(orderShipping, accessor);
				}
				else
				{
					orderService.AppendManagementMemo(
						orderId,
						string.Format("ネクストエンジン受注取込 配送サービスが見つかりませんでした：{0}", updateNeOrder.DeliveryName),
						this.LoginOperatorName,
						UpdateHistoryAction.Insert,
						accessor);
				}
			}
		}

		/// <summary>
		/// Recustomer受注連携
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="oldOrderStaus">更新前注文ステータス</param>
		/// <param name="shippedDate">出荷完了日</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>エラーメッセージ</returns>
		private string ExecOrderCooperationRecustomer(
			OrderModel order,
			string oldOrderStaus,
			string shippedDate,
			SqlAccessor accessor)
		{
			var errorMessage = string.Empty;

			if(oldOrderStaus != Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
			{
				errorMessage
					= RecustomerApiLogger.CreateRelationMemoForError(RecustomerApiLogger.ErrorKbn.UnShipped);
			}
			else if(order.IsDigitalContents || order.IsGiftOrder)
			{
				errorMessage
					= RecustomerApiLogger.CreateRelationMemoForError(order.IsGiftOrder ? RecustomerApiLogger.ErrorKbn.Gift : RecustomerApiLogger.ErrorKbn.DigitalContents);
			}
			
			if(string.IsNullOrEmpty(errorMessage) == false)
			{
				DomainFacade.Instance.OrderService.AppendRelationMemo(
					order.OrderId,
					((string.IsNullOrEmpty(order.RelationMemo) ? string.Empty : "\r\n") + errorMessage),
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);
				return errorMessage;
			}

			errorMessage = RecustomerApiFacade.OrderImporter(order, shippedDate, this.LoginOperatorName, accessor);

			if(string.IsNullOrEmpty(errorMessage))
			{
				DomainFacade.Instance.OrderService.UpdateOrderExtendStatus(
					order.OrderId,
					Constants.RECUSTOMER_ORDER_EXTEND_STATUS_NO,
					Constants.FLG_ORDER_EXTEND_STATUS_ON,
					DateTimeWrapper.Instance.Now,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);
			}

			return errorMessage;
		}

		/// <summary>
		/// Can change order status
		/// </summary>
		/// <param name="targetOrderStatus">Target order status</param>
		/// <returns>True: Order status can be changed, otherwise: false</returns>
		private bool CanChangeOrderStatus(string targetOrderStatus)
		{
			if (Constants.ORDERWORKFLOW_LIMIT_UPDATEORDERSTATUS_ENABLED == false) return true;

			var canChangeOrderStatus = true;
			switch (targetOrderStatus)
			{
				case Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED:
					canChangeOrderStatus = false;
					break;

				case Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP:
				case Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP:
					canChangeOrderStatus = (this.OrderStatusChangeValue == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
						|| (this.OrderStatusChangeValue == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP);
					break;
			}

			return canChangeOrderStatus;
		}

		/// <summary>
		/// Can Update External Payment Status For Sales Or Shipment
		/// </summary>
		/// <param name="result">Result</param>
		/// <param name="isNotExternalPayment">Is not external payment action</param>
		/// <returns>If can update external payment status for sales or shipment: True, otherwise: False</returns>
		public bool CanUpdateExternalPaymentStatusForSalesOrShipment(
			bool result,
			bool isNotExternalPayment)
		{
			return (this.NeedsExecExternalPaymentAction
				&& result
				&& isNotExternalPayment
				&& (this.ExternalPaymentActionValue
					!= Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CVS_DEF_INVOICE_REISSUE)
				&& (this.ExternalPaymentActionValue
					!= Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_INVOICE));
		}

		/// <summary>
		/// Can change store pickup status
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>True: Store pickup status can be changed, otherwise: false</returns>
		private bool CanChangeStorePickupStatus(string orderId, SqlAccessor accessor)
		{
			var canChangeStorePickupStatus = false;
			var storePickupStatus = DomainFacade.Instance.OrderService
				.Get(orderId, accessor).StorePickupStatus;

			if (string.IsNullOrEmpty(storePickupStatus)) return true;

			switch (storePickupStatus)
			{
				case Constants.FLG_ORDER_ORDER_STORE_PICKUP_STATUS_PENDING:
					if ((this.StorePickupStatusChangeValue == Constants.FLG_ORDER_ORDER_STORE_PICKUP_STATUS_PENDING)
						|| (this.StorePickupStatusChangeValue == Constants.FLG_ORDER_ORDER_STORE_PICKUP_STATUS_ARRIVED)
						|| (this.StorePickupStatusChangeValue == Constants.FLG_ORDER_ORDER_STORE_PICKUP_STATUS_DELIVERED))
					{
						canChangeStorePickupStatus = true;
					}
					break;

				case Constants.FLG_ORDER_ORDER_STORE_PICKUP_STATUS_ARRIVED:
					if ((this.StorePickupStatusChangeValue == Constants.FLG_ORDER_ORDER_STORE_PICKUP_STATUS_ARRIVED)
						|| (this.StorePickupStatusChangeValue == Constants.FLG_ORDER_ORDER_STORE_PICKUP_STATUS_DELIVERED)
						|| (this.StorePickupStatusChangeValue == Constants.FLG_ORDER_ORDER_STORE_PICKUP_STATUS_RETURNED))
					{
						canChangeStorePickupStatus = true;
					}
					break;

				case Constants.FLG_ORDER_ORDER_STORE_PICKUP_STATUS_DELIVERED:
				case Constants.FLG_ORDER_ORDER_STORE_PICKUP_STATUS_RETURNED:
					canChangeStorePickupStatus = true;
					break;
			}

			return canChangeStorePickupStatus;
		}

		/// <summary>受注ワークフロー設定</summary>
		public DataRowView Setting { get; set; }
		/// <summary>ログインオペレータ識別ID</summary>
		protected string LoginOperatorDeptId { get; private set; }
		/// <summary>ログインオペレータ名</summary>
		protected string LoginOperatorName { get; private set; }
		/// <summary>注文ステータス変更値（カセットの場合は実行毎に変化します）</summary>
		protected string OrderStatusChangeValue { get; private set; }
		/// <summary>商品実在庫変更値（カセットの場合は実行毎に変化します）</summary>
		protected string ProductRealStockChangeValue { get; private set; }
		/// <summary>入金ステータス変更値（カセットの場合は実行毎に変化します）</summary>
		protected string PaymentStatusChangeValue { get; private set; }
		/// <summary>外部決済アクション変更値（カセットの場合は実行毎に変化します）</summary>
		public string ExternalPaymentActionValue { get; private set; }
		/// <summary>督促ステータス変更値（カセットの場合は実行毎に変化します）</summary>
		protected string DemandStatusChangeValue { get; private set; }
		/// <summary>注文拡張ステータス変更値（カセットの場合は実行毎に変化します）</summary>
		protected string[] OrderExtendStatusChangeValues { get; private set; }
		/// <summary>注文返品ステータス変更値（カセットの場合は実行毎に変化します）</summary>
		protected string ReturnExchangeStatusChangeValue { get; private set; }
		/// <summary>注文返金ステータス変更値（カセットの場合は実行毎に変化します）</summary>
		protected string RepaymentStatusChangeValue { get; private set; }
		/// <summary>送信メールテンプレートID</summary>
		public string MailIdValue { get; private set; }
		/// <summary>定期購入状態変更値（カセットの場合は実行毎に変化します）</summary>
		protected string FixedPurchaseIsAliveChangeValue { get; private set; }
		/// <summary>定期決済ステータス変更値（カセットの場合は実行毎に変化します）</summary>
		protected string FixedPurchasePaymentStatusChangeValue { get; private set; }
		/// <summary>次回配送日変更値（カセットの場合は実行毎に変化します）</summary>
		protected string NextShippingDateChangeValue { get; private set; }
		/// <summary>次々回配送日変更値（カセットの場合は実行毎に変化します）</summary>
		protected string NextNextShippingDateChangeValue { get; private set; }
		/// <summary>配送不可エリア停止変更値（カセットの場合は実行毎に変化します）</summary>
		protected string FixedPurchaseStopUnavailableShippingAreaChangeValue { get; private set; }
		/// <summary>定期拡張ステータス変更値（カセットの場合は実行毎に変化します）</summary>
		protected string[] FixedPurchaseExtendStatusChangeValues { get; private set; }
		/// <summary>注文ステータス更新必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool NeedsUpdateOrderStatus { get; private set; }
		/// <summary>商品実在庫更新必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool NeedsUpdateProductRealStock { get; private set; }
		/// <summary>入金ステータス更新必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool NeedsUpdatePaymentStatus { get; private set; }
		/// <summary>外部決済実行必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool NeedsExecExternalPaymentAction { get; private set; }
		/// <summary>督促ステータス更新必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool NeedsUpdateDemandStatus { get; private set; }
		/// <summary>注文返品ステータス更新必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool NeedsUpdateReturnExchangeStatus { get; private set; }
		/// <summary>注文返金ステータス更新必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool NeedsUpdateRepaymentStatus { get; private set; }
		/// <summary>定期購入状態変更必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool NeedsUpdateFixedPurchaseIsAlive { get; private set; }
		/// <summary>定期決済ステータス変更必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool NeedsUpdateFixedPurchasePaymentStatus { get; private set; }
		/// <summary>次回配送日変更必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool NeedsUpdateNextShippingDate { get; private set; }
		/// <summary>次々回配送日変更必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool NeedsUpdateNextNextShippingDate { get; private set; }
		/// <summary>次々回配送日自動計算フラグ）</summary>
		public bool NeedsCalculateNextNextShippingDate { get; private set; }
		/// <summary>配送不可エリア停止変更必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool NeedsUpdateFixedPurchaseStopUnavailableShippingArea { get; private set; }
		/// <summary>注文拡張ステータス更新必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool[] NeedsUpdateOrderExtendStatus
		{
			get { return m_needsUpdateOrderExtendStatus; }
		}
		/// <summary>注文拡張ステータス更新必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool[] NeedsUpdateFixedPurchaseExtendStatus
		{
			get { return m_needsUpdateFixedPurchaseExtendStatus; }
		}
		/// <summary>Needs Update Order Return</summary>
		public bool NeedsUpdateOrderReturn { get; private set; }
		/// <summary>注文拡張ステータス更新結果表示フラグ</summary>
		public bool[] DisplayUpdateOrderExtendStatusStatementResult
		{
			get { return m_displayUpdateOrderExtendStatusStatementResult; }
		}
		/// <summary>定期拡張ステータス更新結果表示フラグ</summary>
		public bool[] DisplayUpdateFixedPurchaseExtendStatusStatementResult
		{
			get { return m_displayUpdateFixedPurchaseExtendStatusStatementResult; }
		}
		/// <summary>注文ステータス更新ステートメント</summary>
		public string OrderStatusStatement { get; private set; }
		/// <summary>入金ステータス更新ステートメント</summary>
		public string PaymentStatusStatement { get; private set; }
		/// <summary>注文督促ステータス更新ステートメント</summary>
		public string DemandStatusStatement { get; private set; }
		/// <summary>注文返品ステータス更新ステートメント</summary>
		public string ReturnExchangeStatusStatement { get; private set; }
		/// <summary>注文返金ステータス更新ステートメント</summary>
		public string RepaymentStatusStatement { get; private set; }
		/// <summary>注文ステータス更新結果表示フラグ</summary>
		public bool DisplayUpdateOrderStatusResult { get; set; }
		/// <summary>商品実在庫更新必要フラグ</summary>
		public bool DisplayUpdateProductRealStockResult { get; set; }
		/// <summary>入金ステータス更新必要フラグ</summary>
		public bool DisplayUpdatePaymentStatusResult { get; set; }
		/// <summary>外部決済実行必要フラグ</summary>
		public bool DisplayExecExternalPaymentActionResult { get; set; }
		/// <summary>注督促ステータス更新必要フラグ</summary>
		public bool DisplayUpdateDemandStatusResult { get; set; }
		/// <summary>定期購入状態変更表示フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool DisplayUpdateFixedPurchaseIsAliveResult { get; private set; }
		/// <summary>定期決済ステータス変更表示フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool DisplayUpdateFixedPurchasePaymentStatusResult { get; private set; }
		/// <summary>次回配送日変更表示フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool DisplayUpdateNextShippingDateResult { get; private set; }
		/// <summary>次々回配送日変更表示フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool DisplayUpdateNextNextShippingDateResult { get; private set; }
		/// <summary>配送不可エリア停止変更表示フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool DisplayUpdateFixedPurchaseStopUnavailableShippingAreaChangeResult { get; private set; }
		/// <summary>注文返品ステータス更新結果表示フラグ</summary>
		public bool DisplayUpdateReturnExchangeStatusResult { get; set; }
		/// <summary>注文返金ステータス更新結果表示フラグ</summary>
		public bool DisplayUpdateRepaymentStatusResult { get; set; }
		/// <summary>メール送信結果表示フラグ</summary>
		public bool DisplayMailSendResult { get; set; }
		/// <summary>Scheduled Shipping Date Update</summary>
		public bool DisplayUpdateScheduledShippingDateStatusResult { get; set; }
		/// <summary>配送希望日更新フラグ</summary>
		public bool DisplayUpdateShippingDateStatusResult { get; set; }
		/// <summary>Order Return Update</summary>
		public bool DisplayUpdateOrderReturnResult { get; private set; }
		/// <summary>Order Invoice Api Update</summary>
		public bool DisplayUpdateOrderInvoiceApiResult { get; private set; }
		/// <summary>Order Invoice Status Update</summary>
		public bool DisplayUpdateOrderInvoiceStatusResult { get; private set; }
		/// <summary>Display Execute External Order Information Action Result</summary>
		public bool DisplayExecExternalOrderInfoActionResult { get; private set; }
		/// <summary>Store pickup status update</summary>
		public bool DisplayUpdateStorePicupStatusResult { get; private set; }
		/// <summary>ワークフロー詳細区分</summary>
		public string DetailKbn
		{
			get { return GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN); }
		}
		/// <summary>表示区分</summary>
		public string DisplayKbn
		{
			get { return GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_KBN); }
		}
		/// <summary>追加検索可能フラグ</summary>
		public string AdditionalSearchFlg
		{
			get { return GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG); }
		}
		/// <summary>詳細区分通常判定</summary>
		public bool IsDetailKbnNormal
		{
			get
			{
				if (this.Setting != null)
				{
					return (this.DetailKbn == Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_NORMAL);
				}
				return false;
			}
		}
		/// <summary>Is Detail Kbn Return</summary>
		public bool IsDetailKbnReturn
		{
			get
			{
				return ((this.Setting != null)
					&& (this.DetailKbn == Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_RETURN));
			}
		}
		/// <summary>詳細区分注文関連ファイル取込ポップアップ判定</summary>
		public bool IsDetailKbnOrderImport
		{
			get
			{
				if (this.Setting != null)
				{
					return (this.DetailKbn == Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_ODR_IMP);
				}
				return false;
			}
		}
		/// <summary>表示区分一行表示判定</summary>
		public bool IsDisplayKbnLine
		{
			get
			{
				if (this.IsDetailKbnNormal || this.IsDetailKbnReturn)
				{
					return (this.DisplayKbn == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE);
				}
				return false;
			}
		}
		/// <summary>表示区分カセット表示判定</summary>
		public bool IsDisplayKbnCassette
		{
			get
			{
				if (this.IsDetailKbnNormal || this.IsDetailKbnReturn)
				{
					return (this.DisplayKbn == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_CASSETTE);
				}
				return false;
			}
		}
		/// <summary>追加検索可能フラグ判定</summary>
		public bool IsAdditionalSearchFlgOn
		{
			get
			{
				return ((this.AdditionalSearchFlg == Constants.FLG_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG_ON)
					|| (this.AdditionalSearchFlg == Constants.FLG_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FIXEDPURCHASE_FLG_ON));
			}
		}
		/// <summary>ステータス更新日有効判定</summary>
		public bool UpdateStatusValid
		{
			// 「ステータス更新日指定」の表示を有効にする必要があるかどうかを判定する処理
			get
			{
				return (((this.Setting != null) && (this.WorkflowType == WorkflowTypes.Order))
					&& ((this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_ORDER_RECOGNIZED)
						|| (this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_STCOK_RESERVED)
						|| (this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_SHIP_ARRANGED)
						|| (this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_SHIP_COMP)
						|| (this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_DELIVERY_COMP)
						|| (this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_ORDER_CANCELED)
						|| (this.OrderStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_TEMP_CANCELED)
						|| (this.PaymentStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE_COMPLETE)
						|| (this.PaymentStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE_SHORTAGE)
						|| (this.ExternalPaymentActionValue != "")
						|| (this.DemandStatusChangeValue != "")
						|| (this.ReturnExchangeStatusChangeValue != "")
						|| (this.ExternalOrderInfoActionValue == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_NEXTENGINE_IMPORT)
						// OrderExtendStatusChangeValues[0～MAX39] の配列の中から空文字以外のものを探す（1個でもあったらtrue）
						|| this.OrderExtendStatusChangeValues.Any(valueString => valueString != "")
						|| (this.RepaymentStatusChangeValue == Constants.FLG_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE_COMPLETE)
						|| (this.IsDisplayKbnCassette)
						|| (this.ReceiptOutputFlgChangeValue != "")));
			}
		}
		/// <summary>ステータス更新日有効判定</summary>
		public bool NeedsUpdate
		{
			// 何らかの処理を行うかどうかを確認する処理。ステータスの変更などがある場合にここが true になる（と思われる）
			get
			{
				return (this.NeedsUpdateOrderStatus
					|| this.NeedsUpdateProductRealStock
					|| this.NeedsUpdatePaymentStatus
					|| this.NeedsExecExternalPaymentAction
					|| this.NeedsUpdateDemandStatus
					|| this.NeedsUpdateReturnExchangeStatus
					|| this.NeedsUpdateRepaymentStatus
					// NeedsUpdateOrderExtendStatus[0～MAX39] の配列の中にtrueがあるか探す（ひとつでも存在すればtrue）
					|| this.NeedsUpdateOrderExtendStatus.Contains(true)
					|| this.NeedsUpdateScheduledShippingDate
					|| this.NeedsUpdateShippingDate
					|| this.NeedsUpdateFixedPurchaseIsAlive
					|| this.NeedsUpdateFixedPurchasePaymentStatus
					|| this.NeedsUpdateNextShippingDate
					|| this.NeedsUpdateNextNextShippingDate
					|| this.NeedsUpdateFixedPurchaseStopUnavailableShippingArea
					|| this.NeedsUpdateFixedPurchaseExtendStatus.Contains(true)
					|| this.NeedsUpdateOrderReturn
					|| this.NeedsUpdateReceiptOutputFlg
					|| this.NeedsCallApiInvoiceStatus
					|| this.NeedsUpdateOrderInvoiceStatus
					|| this.NeedsExecExternalOrderInfoAction
					|| this.NeedsUpdateStorePickupStatus);
			}
		}
		/// <summary>出荷予定日有効判定</summary>
		public bool NeedsUpdateScheduledShippingDate { get; set; }
		/// <summary>配送希望日有効判定</summary>
		public bool NeedsUpdateShippingDate { get; set; }
		/// <summary>出荷予定日自動計算判定</summary>
		public bool NeedsCalculateScheduledShippingDate { get; set; }
		/// <summary>ワークフロー種別</summary>
		public WorkflowTypes WorkflowType { get; set; }
		/// <summary>Apiエラーメッセージ</summary>
		public string ApiErrorMessage { get; set; }
		/// <summary>決済処理成功時のメッセージ</summary>
		public string SuccessMessage { get; set; }
		/// <summary>値登録向け・カード取引ID（登録する場合は値が格納される）</summary>
		protected string RegisterCardTranId { get; set; }
		/// <summary>値登録向け・決済注文ID（登録する場合は値が格納される）</summary>
		protected string RegisterPaymentOrderId { get; set; }
		/// <summary>値登録向け・外部決済与信日時（登録する場合は値が格納される）</summary>
		protected string RegisterExternalPaymentAuthDate { get; set; }
		/// <summary>値登録向け・外部決済ステータス（登録する場合は値が格納される）</summary>
		protected string RegisterExternalPaymentStatus { get; set; }
		/// <summary>値登録向け・オンライン決済ステータス（登録する場合は値が格納される）</summary>
		protected string RegisterOnlinePaymentStatus { get; set; }
		/// <summary>値登録向け・決済連携メモ（登録する場合は値が格納される）</summary>
		protected string RegisterPaymentMemo { get; set; }
		/// <summary>領収書出力フラグ変更値（カセットの場合は実行毎に変化します）</summary>
		protected string ReceiptOutputFlgChangeValue { get; private set; }
		/// <summary>領収書出力フラグ更新必要フラグ（カセットの場合は実行毎に変化します）</summary>
		public bool NeedsUpdateReceiptOutputFlg { get; private set; }
		/// <summary>領収書出力フラグ更新結果表示フラグ</summary>
		public bool DisplayUpdateReceiptOutputFlgResult { get; set; }
		/// <summary>領収書出力フラグ更新ステートメント</summary>
		public string ReceiptOutputFlgStatement { get; private set; }
		/// <summary>Api Invoice Status Value</summary>
		protected string ApiInvoiceStatusValue { get; private set; }
		/// <summary>Needs Call Api Invoice Status</summary>
		public bool NeedsCallApiInvoiceStatus { get; private set; }
		/// <summary>Order Invoice Status Change Value</summary>
		public string OrderInvoiceStatusChangeValue { get; private set; }
		/// <summary>Needs Order Update Invoice Status</summary>
		public bool NeedsUpdateOrderInvoiceStatus { get; private set; }
		/// <summary>External Order Information Action Value</summary>
		public string ExternalOrderInfoActionValue { get; private set; }
		/// <summary>Needs Execute External Order Information Action</summary>
		public bool NeedsExecExternalOrderInfoAction { get; private set; }
		/// <summary>Store pickup status change value</summary>
		protected string StorePickupStatusChangeValue { get; private set; }
		/// <summary>Needs update shop pickup status</summary>
		public bool NeedsUpdateStorePickupStatus { get; private set; }
		/// <summary>ワークフロー種別</summary>
		public enum WorkflowTypes
		{
			/// <summary>受注</summary>
			Order,
			/// <summary>定期</summary>
			FixedPurchase,
		}
	}
}
