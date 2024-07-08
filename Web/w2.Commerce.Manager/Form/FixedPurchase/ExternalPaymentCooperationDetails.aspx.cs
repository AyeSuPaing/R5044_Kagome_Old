/*
=========================================================================================================
Module      : 外部決済連携ログ詳細ページ(ExternalPaymentCooperationDetails.aspx)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.FieldMemoSetting;
using w2.Domain.FixedPurchase;

public partial class Form_FixedPurchase_ExternalPaymentCooperationDetails : FixedPurchasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.FieldMemoSettingData =
			GetFieldMemoSettingList(Constants.TABLE_FIXEDPURCHASEHISTORY) ?? new FieldMemoSettingModel[] {};
		this.ExternalPaymentCooperationLog = new FixedPurchaseService().GetDetailExternalPaymentCooperationLog(
			Request.QueryString[Constants.FIELD_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_ID],
			Request.QueryString[Constants.FIELD_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_NO]);

		// 外部決済連携ログがあった場合、末尾の改行文字列を削除
		if (string.IsNullOrEmpty(this.ExternalPaymentCooperationLog) == false)
		{
			this.ExternalPaymentCooperationLog = this.ExternalPaymentCooperationLog.TrimEnd('\r', '\n');
		}
	}

	/// <summary>外部決済連携ログ</summary>
	protected string ExternalPaymentCooperationLog { get; set; }
	/// <summary>項目メモ一覧</summary>
	protected FieldMemoSettingModel[] FieldMemoSettingData { get; set; }
}