/*
=========================================================================================================
  Module      : 定期配送パターン設定ページ処理(FixedPurchaseShippingPattern.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Domain.Product;
using w2.Domain.ShopShipping;
using w2.App.Common.Order;

public partial class Form_Common_FixedPurchaseShippingPattern : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			if (string.IsNullOrEmpty(this.ShopId)
				|| string.IsNullOrEmpty(this.ProductId)
				|| string.IsNullOrEmpty(this.VariationId))
			{
				// パラメータが不足している場合エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_NEXT_PRODUCT_SETTING_NOT_SET_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			this.Product = new ProductService().GetProductVariation(this.ShopId, this.ProductId, this.VariationId, string.Empty);
			if (this.Product == null)
			{
				// 商品が見つからない場合エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_NEXT_PRODUCT_SETTING_INPUT_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			this.ShopShipping = new ShopShippingService().Get(this.ShopId, this.Product.ShippingId);
			if (this.ShopShipping == null)
			{
				// 配送種別情報が取得できない場合エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_NEXT_PRODUCT_GET_SHIPPING_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			InitializeComponents();
		}
	}

	#region メソッド
	/// <summary>
	/// 初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 配送パターン・表示制御
		dtMonthlyDate.Visible = ddMonthlyDate.Visible = this.ShopShipping.IsValidFixedPurchaseKbn1Flg;
		dtWeekAndDay.Visible = ddWeekAndDay.Visible = this.ShopShipping.IsValidFixedPurchaseKbn2Flg;
		dtIntervalDays.Visible = ddIntervalDays.Visible = this.ShopShipping.IsValidFixedPurchaseKbn3Flg;
		dtEveryNWeek.Visible = ddEveryNWeek.Visible = this.ShopShipping.IsValidFixedPurchaseKbn4Flg;
		// 第X週ドロップダウン作成
		ddlWeekOfMonth.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST));
		// Y曜日ドロップダウン作成
		ddlDayOfWeek.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST));
		// X日ドロップダウン作成
		if (string.IsNullOrEmpty(this.ShopShipping.FixedPurchaseKbn1Setting2))
		{
			ddlMonthlyDate.Items.AddRange(ValueText.GetValueItemArray(
				Constants.TABLE_SHOPSHIPPING,
				Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DATE_LIST));
		}
		else
		{
			ddlMonthlyDate.Items.AddRange(
				OrderCommon.GetKbn1FixedPurchaseIntervalListItems(
					this.ShopShipping.FixedPurchaseKbn1Setting2.Replace("(", "").Replace(")", ""),
					string.Empty,
					true));
		}

		// Xか月ごとリストボックス作成
		var selectedValue = (this.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE)
			? this.FixedPurchaseSetting1.Split(',')[0]
			: string.Empty;
		ddlMonth.Items.AddRange(
			OrderCommon.GetKbn1FixedPurchaseIntervalListItems(
				OrderCommon.GetFixedPurchaseIntervalSettingExceptLimitedValue(
					this.Product.LimitedFixedPurchaseKbn1Setting.Split(
						new[] { "," },
						StringSplitOptions.RemoveEmptyEntries).ToList(),
					Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING,
					this.ShopShipping.ShippingId),
				selectedValue));
		ddlIntervalMonths.Items.AddRange(
			OrderCommon.GetKbn1FixedPurchaseIntervalListItems(
				OrderCommon.GetFixedPurchaseIntervalSettingExceptLimitedValue(
					this.Product.LimitedFixedPurchaseKbn1Setting.Split(
						new[] { "," },
						StringSplitOptions.RemoveEmptyEntries).ToList(),
					Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING,
					this.ShopShipping.ShippingId),
				selectedValue));

		// X日間隔
		var interval = (this.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS)
			? this.FixedPurchaseSetting1
			: string.Empty;
		ddlIntervalDays.Items.AddRange(
			OrderCommon.GetKbn3FixedPurchaseIntervalListItems(
				OrderCommon.GetFixedPurchaseIntervalSettingExceptLimitedValue(
					this.Product.LimitedFixedPurchaseKbn3Setting.Split(
						new[] { "," },
						StringSplitOptions.RemoveEmptyEntries).ToList(),
					Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_SETTING,
					this.ShopShipping.ShippingId).Replace("(", "").Replace(")", ""),
				interval));

		//X週・曜日指定
		if (this.ShopShipping.IsValidFixedPurchaseKbn4Flg)
		{
			var kbn4SelectedValues = (this.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY)
				? this.FixedPurchaseSetting1.Split(',')
				: new[] { string.Empty };
			ddlFixedPurchaseEveryNWeek_Week.Items.Clear();
			ddlFixedPurchaseEveryNWeek_Week.Items.AddRange(
				OrderCommon.GetKbn4Setting1FixedPurchaseIntervalListItems(
					OrderCommon.GetFixedPurchaseIntervalSettingExceptLimitedValue(
						this.Product.LimitedFixedPurchaseKbn4Setting.Split(
							new[] { "," },
							StringSplitOptions.RemoveEmptyEntries).ToList(),
						Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING1,
						this.ShopShipping.ShippingId).Replace("(", "").Replace(")", ""),
					kbn4SelectedValues[0]));

			var selectedDayOfWeek = (kbn4SelectedValues.Length > 1) ? kbn4SelectedValues[1] : string.Empty;
			ddlFixedPurchaseEveryNWeek_DayOfWeek.Items.Clear();
			ddlFixedPurchaseEveryNWeek_DayOfWeek.Items.AddRange(
				OrderCommon.GetKbn4Setting2FixedPurchaseIntervalListItems(
					this.ShopShipping.FixedPurchaseKbn4Setting2,
					selectedDayOfWeek));
		}

		// 配送パターン
		switch (this.FixedPurchaseKbn)
		{
			// Xか月ごとY日
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
				rbFixedPurchaseDays.Checked = true;
				var monthAndDays = this.FixedPurchaseSetting1.Split(',');
				ddlMonth.SelectedValue = monthAndDays[0];
				ddlMonthlyDate.SelectedValue = monthAndDays[1];
				break;

			// Xか月ごと第YZ曜日
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
				var splitedFixedPurchaseSetting1 = StringUtility.ToEmpty(this.FixedPurchaseSetting1).Split(',');
				rbFixedPurchaseWeekAndDay.Checked = true;
				if (splitedFixedPurchaseSetting1.Length > 0)
				{
					ddlIntervalMonths.SelectedValue = splitedFixedPurchaseSetting1[0];
					ddlWeekOfMonth.SelectedValue = splitedFixedPurchaseSetting1[1];
					ddlDayOfWeek.SelectedValue = splitedFixedPurchaseSetting1[2];
				}
				break;

			// X日 間隔
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
				rbFixedPurchaseIntervalDays.Checked = true;
				ddlIntervalDays.SelectedValue = this.FixedPurchaseSetting1;
				break;

			// X週 Y曜日
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
				rbFixedPurchaseEveryNWeek.Checked = true;
				var weekAndDayOfWeek = this.FixedPurchaseSetting1.Split(',');
				ddlFixedPurchaseEveryNWeek_Week.SelectedValue = weekAndDayOfWeek[0];
				ddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue = weekAndDayOfWeek[1];
				break;
		}
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	protected string ShopId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SHOP_ID]); }
	}
	/// <summary>商品ID</summary>
	protected string ProductId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ID]); }
	}
	/// <summary>バリエーションID</summary>
	protected string VariationId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_VARIATION_ID]); }
	}
	/// <summary>定期購入区分</summary>
	protected string FixedPurchaseKbn
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RECOMMENDITEM_FIXED_PURCHASE_KBN]); }
	}
	/// <summary>定期購入設定1</summary>
	protected string FixedPurchaseSetting1
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RECOMMENDITEM_FIXED_PURCHASE_SETTING1]); }
	}
	/// <summary>商品情報</summary>
	protected ProductModel Product
	{
		get { return (ProductModel)ViewState["Product"]; }
		set { ViewState["Product"] = value; }
	}
	/// <summary>配送種別設定</summary>
	protected ShopShippingModel ShopShipping
	{
		get { return (ShopShippingModel)ViewState["ShopShipping"]; }
		set { ViewState["ShopShipping"] = value; }
	}
	#endregion
}