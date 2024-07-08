/*
=========================================================================================================
  Module      : 定期購入情報入力クラス (FixedPurchaseInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.App.Common.Input;

/// <summary>
/// 定期購入情報入力クラス
/// </summary>
public class FixedPurchaseInput : InputBase<FixedPurchaseModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public FixedPurchaseInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="container">表示用定期購入情報</param>
	public FixedPurchaseInput(FixedPurchaseContainer container)
		: this()
	{
		this.FixedPurchaseId = container.FixedPurchaseId;
		this.FixedPurchaseKbn = container.FixedPurchaseKbn;
		this.FixedPurchaseSetting1 = container.FixedPurchaseSetting1;
		this.FixedPurchaseStatus = container.FixedPurchaseStatus;
		this.PaymentStatus = container.PaymentStatus;
		this.LastOrderDate = (container.LastOrderDate != null) ? container.LastOrderDate.ToString() : null;
		this.OrderCount = container.OrderCount.ToString();
		this.UserId = container.UserId;
		this.ShopId = container.ShopId;
		this.OrderKbn = container.OrderKbn;
		this.OrderPaymentKbn = container.OrderPaymentKbn;
		this.FixedPurchaseDateBgn = container.FixedPurchaseDateBgn.ToString();
		this.ValidFlg = container.ValidFlg;
		this.DateCreated = container.DateCreated.ToString();
		this.DateChanged = container.DateChanged.ToString();
		this.LastChanged = container.LastChanged;
		this.CreditBranchNo = (container.CreditBranchNo != null) ? container.CreditBranchNo.ToString() : null;
		this.NextShippingDate = (container.NextShippingDate != null) ? container.NextShippingDate.ToString() : null;
		this.NextNextShippingDate = (container.NextNextShippingDate != null) ? container.NextNextShippingDate.ToString() : null;
		this.FixedPurchaseManagementMemo = container.FixedPurchaseManagementMemo;
		this.CardInstallmentsCode = container.CardInstallmentsCode;
		// 配送先リスト
		this.Shippings = container.Shippings.Select(s => new FixedPurchaseShippingInput(s)).ToArray();
		// 配送メモ
		this.ShippingMemo = container.ShippingMemo;
		this.Memo = container.Memo;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override FixedPurchaseModel CreateModel()
	{
		var model = new FixedPurchaseModel
		{
			FixedPurchaseId = this.FixedPurchaseId,
			FixedPurchaseKbn = this.FixedPurchaseKbn,
			FixedPurchaseSetting1 = this.FixedPurchaseSetting1,
			FixedPurchaseStatus = this.FixedPurchaseStatus,
			PaymentStatus = this.PaymentStatus,
			LastOrderDate = (this.LastOrderDate != null) ? DateTime.Parse(this.LastOrderDate) : (DateTime?)null,
			OrderCount = int.Parse(this.OrderCount),
			UserId = this.UserId,
			ShopId = this.ShopId,
			OrderKbn = this.OrderKbn,
			OrderPaymentKbn = this.OrderPaymentKbn,
			FixedPurchaseDateBgn = DateTime.Parse(this.FixedPurchaseDateBgn),
			ValidFlg = this.ValidFlg,
			DateCreated = DateTime.Parse(this.DateCreated),
			DateChanged = DateTime.Parse(this.DateChanged),
			LastChanged = this.LastChanged,
			CreditBranchNo = (this.CreditBranchNo != null) ? int.Parse(this.CreditBranchNo) : (int?)null,
			NextShippingDate = (this.NextShippingDate != null) ? DateTime.Parse(this.NextShippingDate) : (DateTime?)null,
			NextNextShippingDate = (this.NextNextShippingDate != null) ? DateTime.Parse(this.NextNextShippingDate) : (DateTime?)null,
			FixedPurchaseManagementMemo = this.FixedPurchaseManagementMemo,
			CardInstallmentsCode = this.CardInstallmentsCode,
			ShippingMemo=this.ShippingMemo,
			Memo = this.Memo
		};
		// 配送先リスト
		model.Shippings = this.Shippings.Select(s => s.CreateModel()).ToArray();

		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ(チェック区分で分けられたディクショナリ)</returns>
	public Dictionary<string, string> Validate()
	{
		return Validator.ValidateAndGetErrorContainer("OrderShipping", this.DataSource);
	}
	#endregion

	#region プロパティ
	/// <summary>定期購入ID</summary>
	public string FixedPurchaseId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID] = value; }
	}
	/// <summary>定期購入区分</summary>
	public string FixedPurchaseKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN] = value; }
	}
	/// <summary>定期購入設定１</summary>
	public string FixedPurchaseSetting1
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1] = value; }
	}
	/// <summary>定期購入設定1_1_1: 月間隔</summary>
	public string FixedPurchaseSetting1_1_1
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_MONTH]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_MONTH] = value; }
	}
	/// <summary>定期購入設定1_1_2：日付</summary>
	public string FixedPurchaseSetting1_1_2
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE] = value; }
	}
	/// <summary>定期購入設定: 月間隔（月間隔・週・曜日指定のパターン用）</summary>
	public string FixedPurchaseSettingIntervalMonths
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS] = value; }
	}
	/// <summary>定期購入設定１_2_1：週</summary>
	public string FixedPurchaseSetting1_2_1
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH] = value; }
	}
	/// <summary>定期購入設定１_2_2：曜日</summary>
	public string FixedPurchaseSetting1_2_2
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK] = value; }
	}
	/// <summary>定期購入設定１_3：配送日間隔</summary>
	public string FixedPurchaseSetting1_3
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS] = value; }
	}
	/// <summary>定期購入設定１_4_1：週間隔</summary>
	public string FixedPurchaseSetting1_4_1
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK] = value; }
	}
	/// <summary>定期購入設定１_4_2：曜日</summary>
	public string FixedPurchaseSetting1_4_2
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK] = value; }
	}
	/// <summary>定期購入ステータス</summary>
	public string FixedPurchaseStatus
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS] = value; }
	}
	/// <summary>決済ステータス</summary>
	public string PaymentStatus
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_PAYMENT_STATUS]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_PAYMENT_STATUS] = value; }
	}
	/// <summary>最終購入日</summary>
	public string LastOrderDate
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE] = value; }
	}
	/// <summary>購入回数</summary>
	public string OrderCount
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_COUNT]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_COUNT] = value; }
	}
	/// <summary>ユーザID</summary>
	public string UserId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_USER_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_USER_ID] = value; }
	}
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHOP_ID] = value; }
	}
	/// <summary>注文区分</summary>
	public string OrderKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_KBN]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_KBN] = value; }
	}
	/// <summary>支払区分</summary>
	public string OrderPaymentKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN] = value; }
	}
	/// <summary>定期購入開始日時</summary>
	public string FixedPurchaseDateBgn
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN] = value; }
	}
	/// <summary>有効フラグ</summary>
	public string ValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_VALID_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_LAST_CHANGED] = value; }
	}
	/// <summary>クレジットカード枝番</summary>
	public string CreditBranchNo
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_CREDIT_BRANCH_NO]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_CREDIT_BRANCH_NO] = value; }
	}
	/// <summary>次回配送日</summary>
	public string NextShippingDate
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE] = value; }
	}
	/// <summary>次々回配送日</summary>
	public string NextNextShippingDate
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE] = value; }
	}
	/// <summary>定期購入管理メモ</summary>
	public string FixedPurchaseManagementMemo
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO] = value; }
	}
	/// <summary>カード支払い回数コード</summary>
	public string CardInstallmentsCode
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_CARD_INSTALLMENTS_CODE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_CARD_INSTALLMENTS_CODE] = value; }
	}
	/// <summary>次回購入の利用ポイント数</summary>
	public string NextShippingUsePoint
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT] = value; }
	}
	/// <summary>配送方法</summary>
	public string ShippingMethod
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD]; }
		set { this.DataSource[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD] = value; }
	}
	/// <summary>配送先リスト</summary>
	public FixedPurchaseShippingInput[] Shippings
	{
		get { return (FixedPurchaseShippingInput[])this.DataSource["Shippings"]; }
		set { this.DataSource["Shippings"] = value; }
	}
	/// <summary>配送メモ</summary>
	public string ShippingMemo
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO] = value; }
	}
	/// <summary>メモ</summary>
	public string Memo
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_MEMO]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_MEMO] = value; }
	}
	#endregion
}