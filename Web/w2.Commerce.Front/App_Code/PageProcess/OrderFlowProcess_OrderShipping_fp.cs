/*
=========================================================================================================
  Module      : 注文フロー（注文配送先-定期購入）プロセス(OrderFlowProcess_OrderShipping_fp.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Order;
using w2.App.Common.Order.FixedPurchase;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.Holiday.Helper;
using w2.Domain.ShopShipping;

/// <summary>
/// OrderLpInputs の概要の説明です
/// </summary>
public partial class OrderFlowProcess
{
	#region  配送情報入力（定期）画面系処理
	/// <summary>カート配送先ごとの定期購入設定</summary>
	private readonly List<FixedPurchaseSetting> m_lFixedPurchaseSettings = new List<FixedPurchaseSetting>();

	/// <summary>
	/// 定期購入設定作成
	/// </summary>
	public void CreateFixedPurchaseSettings()
	{
		// 配送種別情報取得後＆画面データバインド前
		foreach (CartObject co in this.CartList.Items)
		{
			if (co.Shippings.Count > 0)	// ギフトなどの場合は配送先が作成されていない場合がある
			{
				bool blKbn1 = (co.Shippings[0].FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE);
				bool blKbn2 = (co.Shippings[0].FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY);
				bool blKbn3 = (co.Shippings[0].FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS);
				bool blKbn4 = (co.Shippings[0].FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY);

				FixedPurchaseSetting fixedPurchaseSetting = new FixedPurchaseSetting();
				fixedPurchaseSetting.Kbn1CheckedValue = blKbn1;
				fixedPurchaseSetting.Kbn2CheckedValue = blKbn2;
				fixedPurchaseSetting.Kbn3CheckedValue = blKbn3;
				fixedPurchaseSetting.Kbn4CheckedValue = blKbn4;
				fixedPurchaseSetting.MonthSelectedValue = blKbn1 ? co.Shippings[0].FixedPurchaseSetting.Split(',')[0] : "";
				fixedPurchaseSetting.MonthlyDateSelectedValue = blKbn1 ? co.Shippings[0].FixedPurchaseSetting.Split(',')[1] : "";
				fixedPurchaseSetting.IntervalMonthsSelectedValue = blKbn2 ? co.Shippings[0].FixedPurchaseSetting.Split(',')[0] : "";
				fixedPurchaseSetting.WeekOfMonthSelectedValue = blKbn2 ? co.Shippings[0].FixedPurchaseSetting.Split(',')[1] : "";
				fixedPurchaseSetting.DayOfWeekSelectedValue = blKbn2 ? co.Shippings[0].FixedPurchaseSetting.Split(',')[2] : "";
				fixedPurchaseSetting.IntervalDaysSelectedValue = blKbn3 ? co.Shippings[0].FixedPurchaseSetting : "";
				fixedPurchaseSetting.EveryNWeekWeekSelectedValue = blKbn4 ? co.Shippings[0].FixedPurchaseSetting.Split(',')[0] : "";
				fixedPurchaseSetting.EveryNWeekDayOfWeekSelectedValue = blKbn4 ? co.Shippings[0].FixedPurchaseSetting.Split(',')[1] : "";
				fixedPurchaseSetting.Kbn1ValidFlag = this.ShopShippingList[this.CartList.Items.IndexOf(co)].IsValidFixedPurchaseKbn1Flg;
				fixedPurchaseSetting.Kbn2ValidFlag = this.ShopShippingList[this.CartList.Items.IndexOf(co)].IsValidFixedPurchaseKbn2Flg;
				fixedPurchaseSetting.Kbn3ValidFlag = this.ShopShippingList[this.CartList.Items.IndexOf(co)].IsValidFixedPurchaseKbn3Flg;
				fixedPurchaseSetting.Kbn4ValidFlag = this.ShopShippingList[this.CartList.Items.IndexOf(co)].IsValidFixedPurchaseKbn4Flg;
				fixedPurchaseSetting.FixedPurchaseShippingDisplayFlg =
					((Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG == false)
						|| fixedPurchaseSetting.Kbn3ValidFlag == false)
					|| this.ShopShippingList[this.CartList.Items.IndexOf(co)].IsFixedPurchaseShippingDisplay;

				m_lFixedPurchaseSettings.Add(fixedPurchaseSetting);
			}
		}
	}

	/// <summary>
	/// 定期購入区分（1, 2, 3, 4）のデフォルトチェックの優先順位を設定
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <param name="p1">順位1の区分（1 or 2 or 3 or 4）</param>
	/// <param name="p2">順位2の区分（1 or 2 or 3 or 4）</param>
	/// <param name="p3">順位3の区分（1 or 2 or 3 or 4）</param>
	/// <param name="p4">順位4の区分（1 or 2 or 3 or 4）</param>
	/// <returns>空文字列 ""</returns>
	public string SetFixedPurchaseDefaultCheckPriority(int index, int p1, int p2, int p3, int p4)
	{
		List<int> priorityList = new List<int>() { p1, p2, p3, p4 };

		var existIntervalMonths = ExistFixedPurchaseInterval(this.CartList.Items[index], true);
		var existIntervalDays = ExistFixedPurchaseInterval(this.CartList.Items[index], false);

		foreach (int i in priorityList)
		{
			// どれかにチェックが入っていれば return
			if (m_lFixedPurchaseSettings[index].Kbn1CheckedValue
				|| m_lFixedPurchaseSettings[index].Kbn2CheckedValue
				|| m_lFixedPurchaseSettings[index].Kbn3CheckedValue
				|| m_lFixedPurchaseSettings[index].Kbn4CheckedValue)
			{
				// ok.
				return "";
			}

			switch (i)
			{
				case 1:
					m_lFixedPurchaseSettings[index].Kbn1CheckedValue = (m_lFixedPurchaseSettings[index].Kbn1ValidFlag && existIntervalMonths);
					break;

				case 2:
					m_lFixedPurchaseSettings[index].Kbn2CheckedValue = (m_lFixedPurchaseSettings[index].Kbn2ValidFlag && existIntervalMonths);
					break;

				case 3:
					m_lFixedPurchaseSettings[index].Kbn3CheckedValue = (m_lFixedPurchaseSettings[index].Kbn3ValidFlag && existIntervalDays);
					break;

				case 4:
					m_lFixedPurchaseSettings[index].Kbn4CheckedValue = (m_lFixedPurchaseSettings[index].Kbn4ValidFlag);
					break;
			}
		}

		return "";
	}

	/// <summary>
	/// 配送希望日のドロップダウン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlFixedPurchaseShippingDate_OnCheckedChanged(object sender, System.EventArgs e)
	{
		var riCart = GetParentRepeaterItem((Control)sender, "rCartList");
		var riCartShipping = GetParentRepeaterItem((Control)sender, "rCartShippings");
		var riTarget = riCartShipping ?? riCart;
		var shippingIndex = (riCartShipping != null) ? riCartShipping.ItemIndex : 0;

		if (this.CartList.Items[riCart.ItemIndex].HasFixedPurchase)
		{
			UpdateNextShippingDateDropDownList(sender);
		}

		// 選択した配送希望日をセット
		var wdtShippingDate = GetWrappedControl<WrappedHtmlGenericControl>(riTarget, "dtShippingDate");
		var wddlShippingDate = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingDate");
		var shippingDate =
			(wdtShippingDate.Visible && (string.IsNullOrEmpty(wddlShippingDate.SelectedValue) == false))
				? (DateTime?)DateTime.Parse(wddlShippingDate.SelectedValue)
				: null;
		var shipping = this.CartList.Items[riCart.ItemIndex].Shippings[shippingIndex];

		shipping.ShippingDate = shippingDate;
		shipping.ShippingDateForCalculation = shippingDate;
		shipping.CalculateScheduledShippingDate(
			this.CartList.Items[riCart.ItemIndex].ShopId,
			shipping.ShippingCountryIsoCode ?? this.CartList.Owner.AddrCountryIsoCode,
			shipping.IsTaiwanCountryShippingEnable
				? shipping.Addr2 ?? this.CartList.Owner.Addr2
				: shipping.Addr1 ?? this.CartList.Owner.Addr1,
			shipping.Zip ?? this.CartList.Owner.Zip);

		// 予定出荷日を計算し、セット
		var wlbScheduledShippingDate = GetWrappedControl<WrappedLabel>(riTarget, "lbScheduledShippingDate");
		wlbScheduledShippingDate.Text = shippingDate.HasValue
			? shipping.GetScheduledShippingDate()
			: string.Empty;

		UpdateFirstShippingDateDropDownList(sender, e);
	}

	/// <summary>
	/// 配送パターン選択のラジオボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void rbFixedPurchaseShippingPattern_OnCheckedChanged(object sender, System.EventArgs e)
	{
		var rbFixedPurchaseMonthlyPurchase_Date = GetWrappedControl<WrappedRadioButton>(((Control)sender).Parent, "rbFixedPurchaseMonthlyPurchase_Date");
		var rbFixedPurchaseMonthlyPurchase_WeekAndDay = GetWrappedControl<WrappedRadioButton>(((Control)sender).Parent, "rbFixedPurchaseMonthlyPurchase_WeekAndDay");
		var rbFixedPurchaseRegularPurchase_IntervalDays = GetWrappedControl<WrappedRadioButton>(((Control)sender).Parent, "rbFixedPurchaseRegularPurchase_IntervalDays");
		var rbFixedPurchaseEveryNWeek = GetWrappedControl<WrappedRadioButton>(((Control)sender).Parent, "rbFixedPurchaseEveryNWeek");

		var wddFixedPurchaseMonthlyPurchase_Date = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "ddFixedPurchaseMonthlyPurchase_Date");
		var wddFixedPurchaseMonthlyPurchase_WeekAndDay = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "ddFixedPurchaseMonthlyPurchase_WeekAndDay");
		var wddFixedPurchaseRegularPurchase_IntervalDays = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "ddFixedPurchaseRegularPurchase_IntervalDays");
		var wddFixedPurchaseEveryNWeek = GetWrappedControl<WrappedHtmlGenericControl>(((Control)sender).Parent, "ddFixedPurchaseEveryNWeek");

		wddFixedPurchaseMonthlyPurchase_Date.Visible = rbFixedPurchaseMonthlyPurchase_Date.Checked;
		wddFixedPurchaseMonthlyPurchase_WeekAndDay.Visible = rbFixedPurchaseMonthlyPurchase_WeekAndDay.Checked;
		wddFixedPurchaseRegularPurchase_IntervalDays.Visible = rbFixedPurchaseRegularPurchase_IntervalDays.Checked;
		wddFixedPurchaseEveryNWeek.Visible = rbFixedPurchaseEveryNWeek.Checked;

		CheckShippingPatternForFirstShippingDate(sender);
		UpdateNextShippingDateDropDownList(sender);
		UpdateFirstShippingDateDropDownList(sender, e);
	}

	/// <summary>
	/// 配送パターン各アイテムのドロップダウン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlFixedPurchaseShippingPatternItem_OnCheckedChanged(object sender, System.EventArgs e)
	{
		CheckShippingPatternForFirstShippingDate(sender);
		UpdateNextShippingDateDropDownList(sender);
		UpdateFirstShippingDateDropDownList(sender, e);
	}

	/// <summary>
	/// 次回配送日変更用ドロップダウンへのデータバインド完了
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlNextShippingDate_OnDataBound(object sender, System.EventArgs e)
	{
		if (this.CartList.Items[GetParentRepeaterItem((Control)sender, "rCartList").ItemIndex].HasFixedPurchase)
		{
			UpdateNextShippingDateDropDownList(sender);
			UpdateFirstShippingDateDropDownList(sender, e);
		}
	}

	/// <summary>
	/// 配送予定日セット：
	/// 選択された配送パターンに応じて、初回配送予定日および2回目配送日選択ドロップダウンをセットします。
	/// </summary>
	/// <param name="sender"></param>
	protected void UpdateNextShippingDateDropDownList(object sender)
	{
		var riCart = GetParentRepeaterItem((Control)sender, "rCartList");
		var riCartShipping = GetParentRepeaterItem((Control)sender, "rCartShippings");
		var riTarget = riCartShipping ?? riCart;
		var shippingIndex = (riCartShipping != null) ? riCartShipping.ItemIndex : 0;

		//------------------------------------------------------
		// 定期購入情報取得（OrderCartPage_OrderShipping.csと同様の処理）
		//------------------------------------------------------
		string validationName = this.IsLoggedIn ? "OrderShipping" : "OrderShippingGuest";

		Hashtable htFixedPurchase;
		string kbn;		// 配送パターン選択値
		string setting;	// 配送パターンプルダウン選択値
		CreateFixedPurchaseData(riTarget, out htFixedPurchase, out kbn, out setting);

		//------------------------------------------------------
		// 初回配送予定日のラベルを更新
		//------------------------------------------------------
		string dateValueFormat = "yyyy/MM/dd";
		var whfFixedPurchaseDaysRequired = GetWrappedControl<WrappedHiddenField>(riTarget, "hfFixedPurchaseDaysRequired");
		var whfFixedPurchaseMinSpan = GetWrappedControl<WrappedHiddenField>(riTarget, "hfFixedPurchaseMinSpan");
		int fixedPurchaseDaysRequired = int.Parse(whfFixedPurchaseDaysRequired.Value);
		int fixedPurchaseMinSpan = int.Parse(whfFixedPurchaseMinSpan.Value);
		var shipping = this.CartList.Items[riCart.ItemIndex].Shippings[shippingIndex];
		if (this.CartList.Items[riCart.ItemIndex].Shippings[shippingIndex].DeliversOwnerAddress())
		{
			shipping.UpdateShippingAddr(this.CartList.Owner, false);
		}

		// 配送希望日
		var wdtShippingDate = GetWrappedControl<WrappedHtmlGenericControl>(riTarget, "dtShippingDate");
		var wddlShippingDate = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingDate");
		DateTime? shippingDate;
		if ((wddlShippingDate.Items.Count > 0)
			&& (this.UsePreviousShippingDate == false))
		{
			shippingDate = (wdtShippingDate.Visible && wddlShippingDate.SelectedValue != "")
				? (DateTime?)DateTime.Parse(wddlShippingDate.SelectedValue)
				: null;
		}
		else
		{
			shippingDate = shipping.ShippingDate;
		}

		// 初回配送予定日をセット
		var addressOfJp = string.IsNullOrEmpty(shipping.Addr1)
			? Constants.CONST_DEFAULT_SHIPPING_ADDR1
			: shipping.Addr1;
		var addressOfTw = Constants.TW_COUNTRY_SHIPPING_ENABLE
			? string.IsNullOrEmpty(shipping.Addr2)
				? Constants.CONST_DEFAULT_SHIPPING_ADDRESS2_TW
				: shipping.Addr2
			: string.Empty;

		var zip = string.IsNullOrEmpty(shipping.Zip)
			? "dummy" // ZIPによるリードタイム引当を無効とするためダミーを設定
			: shipping.Zip;

		// 初回配送予定日
		var firstShippingDate = OrderCommon.GetFirstShippingDateBasedOnToday(
			this.ShopId,
			fixedPurchaseDaysRequired,
			shippingDate,
			shipping.ShippingMethod,
			shipping.DeliveryCompanyId,
			shipping.ShippingCountryIsoCode,
			shipping.IsTaiwanCountryShippingEnable
				? addressOfTw
				: addressOfJp,
			zip);

		// 配送希望日が指定なく配送希望日のデフォルト値指定なしの場合、配送希望日の最短を取得
		var shortestShippingDate = ((Constants.DISPLAY_DEFAULTSHIPPINGDATE_ENABLED == false) && (shippingDate == null))
			? GetShortestShippingDate(
				this.CartList.Items[riCart.ItemIndex],
				this.ShopShippingList[riCart.ItemIndex])
			: null;

		var wlblFirstShippingDate = GetWrappedControl<WrappedLabel>(riTarget, "lblFirstShippingDate");

		wlblFirstShippingDate.Text = HtmlSanitizer.HtmlEncode(
			DateTimeUtility.ToStringFromRegion(
				(shortestShippingDate != null)
					? shortestShippingDate
					: firstShippingDate,
				DateTimeUtility.FormatType.LongDateWeekOfDay1Letter));

		var wlblFirstShippingDateNoteMessage = GetWrappedControl<WrappedLabel>(riTarget, "lblFirstShippingDateNoteMessage");
		wlblFirstShippingDateNoteMessage.Visible =
			Constants.SCHEDULED_SHIPPING_DATE_OPTION_ENABLE
			&& (OrderCommon.IsLeadTimeFlgOff(shipping.DeliveryCompanyId) == false)
			&& (shippingDate.HasValue == false);

		var wlFirstShippingDateDayOfWeekNoteMessage = GetWrappedControl<WrappedLiteral>(riTarget, "lFirstShippingDateDayOfWeekNoteMessage");
		var wrbFixedPurchaseMonthlyPurchase_WeekAndDay = GetWrappedControl<WrappedRadioButton>(riTarget, "rbFixedPurchaseMonthlyPurchase_WeekAndDay");
		var wrbFixedPurchaseEveryNWeek = GetWrappedControl<WrappedRadioButton>(riTarget, "rbFixedPurchaseEveryNWeek");
		wlFirstShippingDateDayOfWeekNoteMessage.Visible = (wrbFixedPurchaseMonthlyPurchase_WeekAndDay.Checked || wrbFixedPurchaseEveryNWeek.Checked);

		//------------------------------------------------------
		// エラーでなければ次回/次々回配送希望日を更新
		//------------------------------------------------------
		if (Validator.Validate(validationName, htFixedPurchase) == "")
		{
			// 次回/次々回配送予定日
			var service = new FixedPurchaseService();
			var calculateMode = service.GetCalculationMode(kbn, Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
			var nextShippingDate = service.CalculateFollowingShippingDate(kbn, setting, firstShippingDate, fixedPurchaseMinSpan, calculateMode);
			var nextNextShippingDate = service.CalculateFollowingShippingDate(kbn, setting, nextShippingDate, fixedPurchaseMinSpan, calculateMode);
			// 追加で選択可能な最短配送日
			var earliestShippingDate = service.CalculateFollowingShippingDate(kbn, setting, firstShippingDate, fixedPurchaseMinSpan, NextShippingCalculationMode.Earliest);
			DateTime selectedNextShippingnDate = this.CartList.Items[riCart.ItemIndex].Shippings[0].NextShippingDate;

			ListItem[] nextShippingDateListItems = null;
			var next = new ListItem(
				DateTimeUtility.ToStringFromRegion(nextShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter),
				nextShippingDate.ToString(dateValueFormat));
			next.Selected = true;
			if (earliestShippingDate == nextShippingDate)
			{
				var nextNext = new ListItem(
					DateTimeUtility.ToStringFromRegion(nextNextShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter),
					nextNextShippingDate.ToString(dateValueFormat));
				nextShippingDateListItems = new ListItem[] { next, nextNext };
			}
			else
			{
				var earliest = new ListItem(
					DateTimeUtility.ToStringFromRegion(earliestShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter),
					earliestShippingDate.ToString(dateValueFormat));
				nextShippingDateListItems = new ListItem[] { earliest, next };
			}
			bool isSelected = nextShippingDateListItems
				.Count(listItem => ((listItem.Value == selectedNextShippingnDate.ToString(dateValueFormat))
					&& (selectedNextShippingnDate > DateTime.Now))) > 0;

			// 次回配送日ラベルをセット
			var wlblNextShippingDate = GetWrappedControl<WrappedLabel>(riTarget, "lblNextShippingDate");
			wlblNextShippingDate.Visible = true;
			wlblNextShippingDate.Text = isSelected
				? DateTimeUtility.ToStringFromRegion(selectedNextShippingnDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter)
				: DateTimeUtility.ToStringFromRegion(nextShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);
			// 次回配送日プルダウンをセット
			var wddlNextShippingDate = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlNextShippingDate");
			if (wddlNextShippingDate.InnerControl != null)
			{
				while (wddlNextShippingDate.InnerControl.Items.Count != 0)
				{
					wddlNextShippingDate.InnerControl.Items.RemoveAt(0);
				}

				// AmazonPayログイン時に一部残ってしまう情報を削除
				if (wddlNextShippingDate.Items.Count != 0) wddlNextShippingDate.ClearItems();

				wddlNextShippingDate.AddItems(nextShippingDateListItems);
				wddlNextShippingDate.Visible = true;
				wddlNextShippingDate.SelectedValue = isSelected ? selectedNextShippingnDate.ToString(dateValueFormat) : nextShippingDate.ToString(dateValueFormat);
			}
		}
	}

	/// <summary>
	/// 定期購入配送パターン/プルダウンの選択結果を元に、定期購入に必要なデータを組み立てます。
	/// 結果は引数の hashtable, kbn, setting に格納して返されます。
	/// </summary>
	/// <param name="control">入力フィールドを取得するためのコントロール</param>
	/// <param name="fpInput">パターン/プルダウンの選択値（入力チェック用）</param>
	/// <param name="fpKbn">配送パターン選択値</param>
	/// <param name="fpSetting">配送パターンプルダウン選択値</param>
	private void CreateFixedPurchaseData(Control control, out Hashtable fpInput, out string fpKbn, out string fpSetting)
	{
		fpInput = new Hashtable();

		var wrbFixedPurchaseMonthlyPurchase_Date = GetWrappedControl<WrappedRadioButton>(control, "rbFixedPurchaseMonthlyPurchase_Date");
		var wddlFixedPurchaseMonth = GetWrappedControl<WrappedDropDownList>(control, "ddlFixedPurchaseMonth");
		var wddlFixedPurchaseMonthlyDate = GetWrappedControl<WrappedDropDownList>(control, "ddlFixedPurchaseMonthlyDate");
		var wrbFixedPurchaseMonthlyPurchase_WeekAndDay = GetWrappedControl<WrappedRadioButton>(control, "rbFixedPurchaseMonthlyPurchase_WeekAndDay");
		var wddlFixedPurchaseIntervalMonths = GetWrappedControl<WrappedDropDownList>(control, "ddlFixedPurchaseIntervalMonths");
		var wddlFixedPurchaseWeekOfMonth = GetWrappedControl<WrappedDropDownList>(control, "ddlFixedPurchaseWeekOfMonth");
		var wddlFixedPurchaseDayOfWeek = GetWrappedControl<WrappedDropDownList>(control, "ddlFixedPurchaseDayOfWeek");
		var wrbFixedPurchaseRegularPurchase_IntervalDays = GetWrappedControl<WrappedRadioButton>(control, "rbFixedPurchaseRegularPurchase_IntervalDays");
		var wddlFixedPurchaseIntervalDays = GetWrappedControl<WrappedDropDownList>(control, "ddlFixedPurchaseIntervalDays");
		var wrbFixedPurchaseEveryNWeek = GetWrappedControl<WrappedRadioButton>(control, "rbFixedPurchaseEveryNWeek");
		var wddlFixedPurchaseEveryNWeek_Week = GetWrappedControl<WrappedDropDownList>(control, "ddlFixedPurchaseEveryNWeek_Week");
		var wddlFixedPurchaseEveryNWeek_DayOfWeek = GetWrappedControl<WrappedDropDownList>(control, "ddlFixedPurchaseEveryNWeek_DayOfWeek");

		// 配送パターン選択のデータ格納
		fpInput.Add(
			Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN,
			wrbFixedPurchaseMonthlyPurchase_Date.Checked ? Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE :
			wrbFixedPurchaseMonthlyPurchase_WeekAndDay.Checked ? Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY :
			wrbFixedPurchaseRegularPurchase_IntervalDays.Checked ? Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS :
			wrbFixedPurchaseEveryNWeek.Checked ? Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY : "");

		// 各配送パターンプルダウン選択のデータ格納
		var intervalMonth = ((wrbFixedPurchaseMonthlyPurchase_Date.Checked && (wddlFixedPurchaseMonth.InnerControl == null))
			|| (wrbFixedPurchaseMonthlyPurchase_WeekAndDay.Checked && (wddlFixedPurchaseIntervalMonths.InnerControl == null)))
				? "1"
				: wrbFixedPurchaseMonthlyPurchase_Date.Checked
					? wddlFixedPurchaseMonth.SelectedValue
					: wddlFixedPurchaseIntervalMonths.SelectedValue;
		if (wrbFixedPurchaseMonthlyPurchase_Date.Checked)
		{
			fpInput.Add(Constants.FIXED_PURCHASE_SETTING_MONTH, intervalMonth);
			fpInput.Add(Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE, wddlFixedPurchaseMonthlyDate.SelectedValue);
		}
		if (wrbFixedPurchaseMonthlyPurchase_WeekAndDay.Checked)
		{
			fpInput.Add(Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS, intervalMonth);
			fpInput.Add(Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH, wddlFixedPurchaseWeekOfMonth.SelectedValue);
			fpInput.Add(Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK, wddlFixedPurchaseDayOfWeek.SelectedValue);
		}
		if (wrbFixedPurchaseRegularPurchase_IntervalDays.Checked)
		{
			fpInput.Add(Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS, wddlFixedPurchaseIntervalDays.SelectedValue);
		}
		if (wrbFixedPurchaseEveryNWeek.Checked)
		{
			fpInput.Add(Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK, wddlFixedPurchaseEveryNWeek_Week.SelectedValue);
			fpInput.Add(Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK, wddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue);
		}

		// 定期購入情報セット
		fpKbn =
			wrbFixedPurchaseMonthlyPurchase_Date.Checked ? Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE :
			wrbFixedPurchaseMonthlyPurchase_WeekAndDay.Checked ? Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY :
			wrbFixedPurchaseRegularPurchase_IntervalDays.Checked ? Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS :
			wrbFixedPurchaseEveryNWeek.Checked ? Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY : "";

		fpSetting = wrbFixedPurchaseMonthlyPurchase_Date.Checked
			? string.Format(
				"{0},{1}",
				intervalMonth,
				wddlFixedPurchaseMonthlyDate.SelectedValue)
			: wrbFixedPurchaseMonthlyPurchase_WeekAndDay.Checked
				? string.Format(
					"{0},{1},{2}",
					intervalMonth,
					wddlFixedPurchaseWeekOfMonth.SelectedValue,
					wddlFixedPurchaseDayOfWeek.SelectedValue)
				: wrbFixedPurchaseRegularPurchase_IntervalDays.Checked
					? string.Format(
						"{0}",
						wddlFixedPurchaseIntervalDays.SelectedValue)
					: wrbFixedPurchaseEveryNWeek.Checked
						? string.Format(
							"{0},{1}",
							wddlFixedPurchaseEveryNWeek_Week.SelectedValue,
							wddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue)
						: string.Empty;
	}

	/// <summary>
	/// 定期購入配送間隔日・月設定値ドロップダウンリスト作成
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <param name="isIntervalMonths">配送間隔月か
	/// （TRUE：配送間隔月；FALSE：配送間隔日）</param>
	/// （TRUE：月間隔・週・曜日指定パターン；FALSE：月間隔日付指定パターン）<param name="isKbn2">区分2か
	/// </param>
	/// <param name="isDays">日付か</param>
	/// <returns>定期購入配送間隔日・月設定値ドロップダウンリスト</returns>
	public ListItem[] GetFixedPurchaseIntervalDropdown(int index, bool isIntervalMonths, bool isKbn2 = false, bool isDays = false)
	{
		var limitedSettings =
			this.CartList.Items[index].Items
				.Where(product => ((product.AddCartKbn == Constants.AddCartKbn.FixedPurchase)
					&& (string.IsNullOrEmpty(isIntervalMonths
						? product.LimitedFixedPurchaseKbn1Setting
						: product.LimitedFixedPurchaseKbn3Setting) == false)))
				.Select(product =>
					isIntervalMonths ? product.LimitedFixedPurchaseKbn1Setting : product.LimitedFixedPurchaseKbn3Setting)
				.Distinct().ToList();

		var baseSetting = OrderCommon.GetFixedPurchaseIntervalSettingExceptLimitedValue(
			limitedSettings,
			isIntervalMonths
				? isDays
					? Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING2
					: Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING
				: Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_SETTING,
			this.CartList.Items[index].ShippingType);

		var intervalValuesList = isIntervalMonths
			? isDays
				? OrderCommon.GetKbn1FixedPurchaseIntervalListItems(
					baseSetting,
					isIntervalMonths
						? isKbn2
							? m_lFixedPurchaseSettings[index].IntervalMonthsSelectedValue
							: m_lFixedPurchaseSettings[index].MonthlyDateSelectedValue
						: m_lFixedPurchaseSettings[index].IntervalDaysSelectedValue,
					true)
				: OrderCommon.GetKbn1FixedPurchaseIntervalListItems(
				baseSetting,
				isIntervalMonths
					? isKbn2 ? m_lFixedPurchaseSettings[index].IntervalMonthsSelectedValue :
					m_lFixedPurchaseSettings[index].MonthSelectedValue
					: m_lFixedPurchaseSettings[index].IntervalDaysSelectedValue)
			: OrderCommon.GetKbn3FixedPurchaseIntervalListItems(
				baseSetting,
				isIntervalMonths
					? isKbn2 ? m_lFixedPurchaseSettings[index].IntervalMonthsSelectedValue :
					m_lFixedPurchaseSettings[index].MonthSelectedValue
					: m_lFixedPurchaseSettings[index].IntervalDaysSelectedValue);

		// カートごとの該当定期購入配送間隔未設定の場合、設定
		if (isIntervalMonths)
		{
			if (string.IsNullOrEmpty(m_lFixedPurchaseSettings[index].MonthSelectedValue))
			{
				m_lFixedPurchaseSettings[index].MonthSelectedValue =
					intervalValuesList.Any() ? intervalValuesList[0].Value : null;
			}
			if (string.IsNullOrEmpty(m_lFixedPurchaseSettings[index].IntervalMonthsSelectedValue))
			{
				m_lFixedPurchaseSettings[index].IntervalMonthsSelectedValue =
					intervalValuesList.Any() ? intervalValuesList[0].Value : null;
			}
		}
		else
		{
			if (string.IsNullOrEmpty(m_lFixedPurchaseSettings[index].IntervalDaysSelectedValue))
			{
				// フロントに表示する間隔日だけのリストを作成
				var usebleBaseSetting = baseSetting.Split(',').Where(s => s.Contains("(") == false).ToArray();

				m_lFixedPurchaseSettings[index].IntervalDaysSelectedValue =
					intervalValuesList.Any()
						? (Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG == false)
							? intervalValuesList[0].Value
							: usebleBaseSetting[0]
						: null;
			}
		}

		return intervalValuesList;
	}

	/// <summary>
	/// 定期購入配送週間隔・曜日設定値ドロップダウンリスト作成
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <param name="isIntervalWeek">週間隔か（TRUE：配送週間隔；FALSE：配送曜日）</param>
	/// <returns>定期購入配送週間隔・曜日設定値ドロップダウンリスト</returns>
	public ListItem[] GetFixedPurchaseEveryNWeekDropdown(int index, bool isIntervalWeek)
	{
		// 商品側で設定している「定期購入配送間隔週利用不可」の取得
		var limitedSettings = isIntervalWeek
			? this.CartList.Items[index].Items
				.Where(product => (product.AddCartKbn == Constants.AddCartKbn.FixedPurchase)
					&& (string.IsNullOrEmpty(product.LimitedFixedPurchaseKbn4Setting) == false))
				.Select(product => product.LimitedFixedPurchaseKbn4Setting)
				.Distinct()
				.ToList()
			: new List<string>();

		var selectedValue = isIntervalWeek
			? m_lFixedPurchaseSettings[index].EveryNWeekWeekSelectedValue
			: m_lFixedPurchaseSettings[index].EveryNWeekDayOfWeekSelectedValue;

		// 有効な週間隔 or 曜日の設定値取得
		var baseSetting = OrderCommon.GetFixedPurchaseIntervalSettingExceptLimitedValue(
			limitedSettings,
			isIntervalWeek
				? Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING1
				: Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING2,
			this.CartList.Items[index].ShippingType);

		// baseSettingをもとにドロップダウンリスト作成
		var intervalValuesList = (isIntervalWeek)
			? OrderCommon.GetKbn4Setting1FixedPurchaseIntervalListItems(
				baseSetting,
				selectedValue)
			: OrderCommon.GetKbn4Setting2FixedPurchaseIntervalListItems(
				baseSetting,
				selectedValue);

		return intervalValuesList;
	}

	/// <summary>
	/// 定期購入区分有効判定
	/// </summary>
	/// <param name="iItemIndex">カートindex</param>
	/// <param name="iKbnNo">区分番号</param>
	/// <returns>有効判定</returns>
	public bool GetFixedPurchaseKbnEnabled(int iItemIndex, int iKbnNo)
	{
		return m_lFixedPurchaseSettings[iItemIndex].KbnValidFlags[iKbnNo - 1];
	}

	/// <summary>
	/// 定期購入配送パターン表示フラグ
	/// </summary>
	/// <param name="repeaterItem">repeaterItem</param>
	/// <returns>定期購入配送パターンを表示するか</returns>
	public bool DisplayFixedPurchaseShipping(RepeaterItem repeaterItem)
	{
		return (repeaterItem.DataItem is CartObject
				? ((CartObject)repeaterItem.DataItem).HasFixedPurchase
				: (FindCart(repeaterItem.DataItem)).HasFixedPurchase)
			&& m_lFixedPurchaseSettings[repeaterItem.ItemIndex].FixedPurchaseShippingDisplayFlg
			&& ((Constants.HIDE_FIXEDPURCHASE_SHIPPING_PATTERN_AREA_IN_ADD_CART_URL_ENABLED == false)
				|| (((CartObject)repeaterItem.DataItem).HasFixedPurchaseShippingPattern == false));
	}

	/// <summary>
	/// 定期購入判断
	/// </summary>
	/// <param name="repeaterItem">repeaterItem</param>
	/// <returns>定期購入かどうか</returns>
	public bool HasFixedPurchase(RepeaterItem repeaterItem)
	{
		var result = (repeaterItem.DataItem is CartObject
			? ((CartObject)repeaterItem.DataItem).HasFixedPurchase
			: (FindCart(repeaterItem.DataItem)).HasFixedPurchase);

		return result;
	}

	/// <summary>
	/// 定期購入区分有効判定（全区分）
	/// </summary>
	/// <param name="itemIndex">カートindex</param>
	/// <returns>有効判定</returns>
	public bool GetAllFixedPurchaseKbnEnabled(int itemIndex)
	{
		var enabled = (GetFixedPurchaseKbnEnabled(itemIndex, 1) && (GetFixedPurchaseIntervalDropdown(itemIndex, true).Length > 1));
		if (enabled) return true;

		enabled = (GetFixedPurchaseKbnEnabled(itemIndex, 2) && (GetFixedPurchaseIntervalDropdown(itemIndex, true, true).Length > 1));
		if (enabled) return true;

		var dropdownLength = GetFixedPurchaseIntervalDropdown(itemIndex, false).Length;
		enabled = (GetFixedPurchaseKbnEnabled(itemIndex, 3)
			&& (Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG
				? (dropdownLength >= 1)
				: (dropdownLength > 1)));
		if (enabled) return true;

		enabled = (GetFixedPurchaseKbnEnabled(itemIndex, 4)
			&& (GetFixedPurchaseEveryNWeekDropdown(itemIndex, true).Length > 1));

		return enabled;
	}

	/// <summary>
	/// 定期購入区分入力部分表示判定
	/// </summary>
	/// <param name="iKbnNo">区分番号</param>
	/// <param name="iItemIndex">カートindex</param>
	/// <returns>選択判定</returns>
	public bool GetFixedPurchaseKbnInputChecked(int iItemIndex, int iKbnNo)
	{
		return GetFixedPurchaseKbnEnabled(iItemIndex, iKbnNo)
			&& m_lFixedPurchaseSettings[iItemIndex].KbnCheckedValues[iKbnNo - 1];
	}
	/// <summary>
	/// 定期購入区分入力部分表示判定
	/// </summary>
	/// <param name="iItemIndex">カートindex</param>
	/// <param name="strName">値名称</param>
	/// <returns>選択判定</returns>
	public string GetFixedPurchaseSelectedValue(int iItemIndex, string strName)
	{
		switch (strName)
		{
			case Constants.FIXED_PURCHASE_SETTING_MONTH:
				if (string.IsNullOrEmpty(m_lFixedPurchaseSettings[iItemIndex].MonthSelectedValue) == false)
				{
					return m_lFixedPurchaseSettings[iItemIndex].MonthSelectedValue;
				}
				// 第２目の選択肢はデフォルト値とする
				var listMonthKbn1 = GetFixedPurchaseIntervalDropdown(iItemIndex, true);
				return (listMonthKbn1.Length > 1)
					? listMonthKbn1[1].Value
					: m_lFixedPurchaseSettings[iItemIndex].MonthSelectedValue;

			case Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE:
				return m_lFixedPurchaseSettings[iItemIndex].MonthlyDateSelectedValue;

			case Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS:
				if (string.IsNullOrEmpty(m_lFixedPurchaseSettings[iItemIndex].IntervalMonthsSelectedValue) == false)
				{
					return m_lFixedPurchaseSettings[iItemIndex].IntervalMonthsSelectedValue;
				}
				// 第２目の選択肢はデフォルト値とする
				var listMonthKbn2 = GetFixedPurchaseIntervalDropdown(iItemIndex, true, true);
				return (listMonthKbn2.Length > 1)
					? listMonthKbn2[1].Value
					: m_lFixedPurchaseSettings[iItemIndex].IntervalMonthsSelectedValue;

			case Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH:
				return m_lFixedPurchaseSettings[iItemIndex].WeekOfMonthSelectedValue;

			case Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK:
				return m_lFixedPurchaseSettings[iItemIndex].DayOfWeekSelectedValue;

			case Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS:
				return m_lFixedPurchaseSettings[iItemIndex].IntervalDaysSelectedValue;

			case Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK:
				return m_lFixedPurchaseSettings[iItemIndex].EveryNWeekWeekSelectedValue;

			case Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK:
				return m_lFixedPurchaseSettings[iItemIndex].EveryNWeekDayOfWeekSelectedValue;
		}

		return "";
	}

	/// <summary>
	/// 選択可能な定期購入配送間隔日・月設定値があるか
	/// </summary>
	/// <param name="cart">カート</param>
	/// <param name="isIntervalMonths">配送間隔月か
	/// （TRUE：配送間隔月；FALSE：配送間隔日）</param>
	/// <returns>選択可能な設定値があるか</returns>
	private bool ExistFixedPurchaseInterval(CartObject cart, bool isIntervalMonths)
	{
		var limitedSettings = cart.Items
			.Where(
				product =>
					((product.AddCartKbn == Constants.AddCartKbn.FixedPurchase)
					&& (string.IsNullOrEmpty(
						isIntervalMonths
							? product.LimitedFixedPurchaseKbn1Setting
							: product.LimitedFixedPurchaseKbn3Setting) == false)))
			.Select(
				product =>
					isIntervalMonths
						? product.LimitedFixedPurchaseKbn1Setting
						: product.LimitedFixedPurchaseKbn3Setting)
			.Distinct()
			.ToList();

		var expectedSettingValue = OrderCommon.GetFixedPurchaseIntervalSettingExceptLimitedValue(
			limitedSettings,
			isIntervalMonths
				? Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING
				: Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_SETTING,
			cart.ShippingType);

		var usableSettingValue = Regex.Replace(expectedSettingValue, @"\([0-9]*?\)", string.Empty).Split(',');
		return usableSettingValue.Any(settingValue => string.IsNullOrEmpty(settingValue) == false);
	}

	/// <summary>
	/// First shipping date on data bound
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlFirstShippingDate_OnDataBound(object sender, System.EventArgs e)
	{
		if (this.CartList.Items[GetParentRepeaterItem((Control)sender, "rCartList").ItemIndex].HasFixedPurchase)
		{
			var riCart = GetOuterControl((Control)sender, typeof(RepeaterItem));
			CheckShippingPatternForFirstShippingDate(sender);
			UpdateFirstShippingDateDropDownList(sender, e);
		}
	}

	/// <summary>
	/// Update first shipping date dropdown list
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void UpdateFirstShippingDateDropDownList(object sender, System.EventArgs e)
	{
		if (this.IsGiftPage) return;

		// 定期購入情報取得（OrderCartPage_OrderShipping.csと同様の処理）
		string validationName = this.IsLoggedIn ? "OrderShipping" : "OrderShippingGuest";

		var riCart = GetParentRepeaterItem((Control)sender, "rCartList");
		var riCartShipping = GetParentRepeaterItem((Control)sender, "rCartShippings");
		var riTarget = riCartShipping ?? riCart;
		var shippingIndex = (riCartShipping != null) ? riCartShipping.ItemIndex : 0;

		Hashtable htFixedPurchase;
		string kbn;
		string setting;
		CreateFixedPurchaseData(riTarget, out htFixedPurchase, out kbn, out setting);

		// 初回配送予定日のラベルを更新
		var dateValueFormat = "yyyy/MM/dd";
		var whfFixedPurchaseDaysRequired = GetWrappedControl<WrappedHiddenField>(riTarget, "hfFixedPurchaseDaysRequired");
		var whfFixedPurchaseMinSpan = GetWrappedControl<WrappedHiddenField>(riTarget, "hfFixedPurchaseMinSpan");
		var fixedPurchaseDaysRequired = int.Parse(whfFixedPurchaseDaysRequired.Value);
		var fixedPurchaseMinSpan = int.Parse(whfFixedPurchaseMinSpan.Value);
		var shipping = this.CartList.Items[riCart.ItemIndex].Shippings[shippingIndex];

		// 配送希望日
		var wdtShippingDate = GetWrappedControl<WrappedHtmlGenericControl>(riTarget, "dtShippingDate");
		var wddShippingDate = GetWrappedControl<WrappedHtmlGenericControl>(riTarget, "ddShippingDate");
		var wddlShippingDate = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingDate");

		// Get wrapped control
		var wlblFirstShippingDate = GetWrappedControl<WrappedLabel>(riTarget, "lblFirstShippingDate");
		var wlblNextShippingDate = GetWrappedControl<WrappedLabel>(riTarget, "lblNextShippingDate");
		var wddlFirstShippingDate = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlFirstShippingDate");
		var wddlNextShippingDate = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlNextShippingDate");

		// Check if option is enabled and flag is valid
		var shopShipping = this.ShopShippingList[riCart.ItemIndex];
		var isMail = this.CartList.Items[riCart.ItemIndex].Shippings[shippingIndex].ShippingMethod
			== Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL;
		if ((shopShipping.CanUseFixedPurchaseFirstShippingDateNextMonth(kbn) == false)
			|| isMail)
		{
			wddlFirstShippingDate.Visible = false;
			return;
		}

		DateTime? shippingDate;
		if (wddlShippingDate.Items.Count > 0)
		{
			shippingDate = (wdtShippingDate.Visible && wddlShippingDate.SelectedValue != string.Empty)
				? (DateTime?)DateTime.Parse(wddlShippingDate.SelectedValue)
				: null;
		}
		else
		{
			shippingDate = wdtShippingDate.Visible ? shipping.ShippingDate : null;
		}

		if (Validator.Validate(validationName, htFixedPurchase) != string.Empty) return;

		// 初回配送予定日をセット
		var addressOfJp = string.IsNullOrEmpty(shipping.Addr1)
			? Constants.CONST_DEFAULT_SHIPPING_ADDR1
			: shipping.Addr1;
		var addressOfTw = Constants.TW_COUNTRY_SHIPPING_ENABLE
			? string.IsNullOrEmpty(shipping.Addr2)
				? Constants.CONST_DEFAULT_SHIPPING_ADDRESS2_TW
				: shipping.Addr2
			: string.Empty;

		var zip = string.IsNullOrEmpty(shipping.Zip)
			? "dummy"
			: shipping.Zip;

		// 初回配送予定日
		var wddlDeliveryCompany = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlDeliveryCompany");
		var deliveryCompanyId = wddlDeliveryCompany.SelectedValue;
		var shortestShippingDate = HolidayUtil.GetShortestShippingDateBasedOnToday(deliveryCompanyId);

		// Get next 1 month for first shipping date dropdown list option
		var settingTmp = setting.Remove(0, 1).Insert(0, "1");
		var fixedPurchaseService = DomainFacade.Instance.FixedPurchaseService;
		var calculateMode = fixedPurchaseService.GetCalculationMode(
			kbn,
			Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);

		// Calculate earliest, next and next next shipping date
		var earliesShippingDate = OrderCommon.GetFirstShippingDateBasedOnToday(
			this.ShopId,
			fixedPurchaseDaysRequired,
			null,
			shipping.ShippingMethod,
			shipping.DeliveryCompanyId,
			shipping.ShippingCountryIsoCode,
			shipping.IsTaiwanCountryShippingEnable
				? addressOfTw
				: addressOfJp,
			zip);

		var nextShippingDate = fixedPurchaseService.CalculateFirstShippingDate(
			kbn,
			settingTmp,
			earliesShippingDate.AddMonths(-1),
			fixedPurchaseMinSpan,
			calculateMode);

		if (nextShippingDate < earliesShippingDate)
		{
			nextShippingDate = fixedPurchaseService.CalculateFirstShippingDate(
				kbn,
				settingTmp,
				earliesShippingDate,
				fixedPurchaseMinSpan,
				calculateMode);
			shipping.FirstShippingDate = nextShippingDate;
		}
		else if (nextShippingDate.Month == DateTime.Now.Month)
		{
			nextShippingDate =
				DomainFacade.Instance.FixedPurchaseService.CalculateFirstShippingDate(
					kbn,
					settingTmp,
					nextShippingDate,
					fixedPurchaseMinSpan,
					calculateMode);
			shipping.FirstShippingDate = nextShippingDate;
		}

		var nextNextShippingDate = fixedPurchaseService.CalculateFollowingShippingDate(
			kbn,
			setting,
			nextShippingDate,
			fixedPurchaseMinSpan,
			calculateMode);

		// Update wrapped control value
		wddlNextShippingDate.Items.Clear();
		var next = ConvertToListItem(nextShippingDate);

		// Update list item data
		var tmpNextShippingDate = fixedPurchaseService.CalculateFirstShippingDate(
			kbn,
			settingTmp,
			nextShippingDate,
			fixedPurchaseMinSpan,
			calculateMode);
		var tmpNext = ConvertToListItem(tmpNextShippingDate);
		var nextNext = ConvertToListItem(nextNextShippingDate);
		var firstShippingDateListItems = new ListItem[] { next, tmpNext };

		// Update wrapped control datas
		var wlbScheduledShippingDate = GetWrappedControl<WrappedLabel>(riTarget, "lbScheduledShippingDate");
		wlbScheduledShippingDate.Text = DateTimeUtility.ToStringFromRegion(
			nextShippingDate,
			DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);
		wlblFirstShippingDate.Text = DateTimeUtility.ToStringFromRegion(nextShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);
		var index = (int)this.ShopShippingList[riCart.ItemIndex].ShippingDateSetEnd;

		InsertShippingDate(wddlShippingDate, index, next);
		wddlFirstShippingDate.Items.Clear();
		wddlFirstShippingDate.ClearItems();
		wddlFirstShippingDate.AddItems(firstShippingDateListItems);

		wlblNextShippingDate.Text = DateTimeUtility.ToStringFromRegion(
			nextNextShippingDate,
			DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);
		wddlNextShippingDate.Items.Insert(0, nextNext);

		wddlNextShippingDate.Visible = true;
		wlblNextShippingDate.Visible = true;
		wdtShippingDate.Visible = false;
		wddShippingDate.Visible = false;
		wddlNextShippingDate.Visible = wddlShippingDate.Visible;

		// Get First shipping selected when return from confirm page
		var selectedShippingDate = this.CartList.Items[riCart.ItemIndex].Shippings[0].ShippingDate;

		var validFirstShippingDate = wddlFirstShippingDate.Items
			.FindByValue(shipping.FirstShippingDate.ToString(dateValueFormat));
		if (validFirstShippingDate != null)
		{
			wlblFirstShippingDate.Text = DateTimeUtility.ToStringFromRegion(
				shipping.FirstShippingDate,
				DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);
			wddlFirstShippingDate.SelectedValue = shipping.FirstShippingDate.ToString(dateValueFormat);
			ddlFirstShippingDate_ItemSelected(sender, e);
		}
		else
		{
			DateTime selectedFirstShippingDate;
			if (DateTime.TryParse(wddlFirstShippingDate.SelectedValue, out selectedFirstShippingDate))
			{
				shipping.FirstShippingDate = DateTime.Parse(wddlFirstShippingDate.SelectedValue);
			}
		}

		if (selectedShippingDate.HasValue == false) return;
		if (selectedShippingDate.Value == tmpNextShippingDate)
		{
			var isSelectedShippingDate = firstShippingDateListItems
				.Count(listItem => listItem.Value == selectedShippingDate.Value.ToString(dateValueFormat)) > 0;

			wddlFirstShippingDate.SelectedValue = isSelectedShippingDate
				? selectedShippingDate.Value.ToString(dateValueFormat)
				: nextShippingDate.ToString(dateValueFormat);
			ddlFirstShippingDate_ItemSelected(sender, e);
		}
		var selectedNextShippingDate = this.CartList.Items[riCart.ItemIndex].Shippings[0].NextShippingDate;
		var nextShippingDateListItems = wddlNextShippingDate.Items.Cast<ListItem>().ToArray();
		var isSelectedNextShippingDate = nextShippingDateListItems
			.Count(listItem => listItem.Value == selectedNextShippingDate.ToString(dateValueFormat)) > 0;

		wlblNextShippingDate.Text = isSelectedNextShippingDate
			? DateTimeUtility.ToStringFromRegion(
				selectedNextShippingDate,
				DateTimeUtility.FormatType.LongDateWeekOfDay1Letter)
			: wddlNextShippingDate.SelectedText;
	}

	/// <summary>
	/// Dropdown list first shipping date item selected
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ddlFirstShippingDate_ItemSelected(object sender, System.EventArgs e)
	{
		var riCart = GetParentRepeaterItem((Control)sender, "rCartList");
		var riCartShipping = GetParentRepeaterItem((Control)sender, "rCartShippings");
		var riTarget = riCartShipping ?? riCart;
		var shippingIndex = (riCartShipping != null) ? riCartShipping.ItemIndex : 0;

		// Update first shipping date label value
		var wlblFirstShippingDate = GetWrappedControl<WrappedLabel>(riTarget, "lblFirstShippingDate");
		var wlblNextShippingDate = GetWrappedControl<WrappedLabel>(riTarget, "lblNextShippingDate");
		var wddlFirstShippingDate = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlFirstShippingDate");
		var wddlNextShippingDate = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlNextShippingDate");
		var wddlShippingDate = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingDate");
		string validationName = this.IsLoggedIn ? "OrderShipping" : "OrderShippingGuest";

		Hashtable htFixedPurchase;
		string kbn;
		string setting;
		CreateFixedPurchaseData(riTarget, out htFixedPurchase, out kbn, out setting);

		if (Validator.Validate(validationName, htFixedPurchase) != string.Empty) return;

		var whfFixedPurchaseMinSpan = GetWrappedControl<WrappedHiddenField>(riTarget, "hfFixedPurchaseMinSpan");
		var fixedPurchaseMinSpan = int.Parse(whfFixedPurchaseMinSpan.Value);
		var tmpDate = DateTime.Parse(wddlFirstShippingDate.SelectedItem.Value);
		var shipping = this.CartList.Items[riCart.ItemIndex].Shippings[shippingIndex];
		shipping.FirstShippingDate = tmpDate;
		var calculateMode = DomainFacade.Instance.FixedPurchaseService.GetCalculationMode(
			kbn,
			Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
		var tmpNextShippingDate = DomainFacade.Instance.FixedPurchaseService.CalculateFollowingShippingDate(
			kbn,
			setting,
			tmpDate,
			fixedPurchaseMinSpan,
			calculateMode);
		var tmpNextNextShippingDate = DomainFacade.Instance.FixedPurchaseService.CalculateFollowingShippingDate(
			kbn,
			setting,
			tmpNextShippingDate,
			fixedPurchaseMinSpan,
			calculateMode);
		var tmpNext = ConvertToListItem(tmpNextShippingDate);
		var tmpNextNext = ConvertToListItem(tmpNextNextShippingDate);

		// Update control value
		wlblFirstShippingDate.Text = wddlFirstShippingDate.SelectedItem.Text;

		wddlShippingDate.SelectedIndex = -1;
		var shippingData = new ListItem(wddlFirstShippingDate.SelectedItem.Text, wddlFirstShippingDate.SelectedItem.Value.ToString());
		var dateEnd = (int)this.ShopShippingList[riCart.ItemIndex].ShippingDateSetEnd;
		InsertShippingDate(wddlShippingDate, dateEnd, shippingData);

		wlblNextShippingDate.Text = DateTimeUtility.ToStringFromRegion(tmpNextShippingDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);
		wddlNextShippingDate.Items.Clear();
		wddlNextShippingDate.Items.Insert(0, tmpNext);
		wddlNextShippingDate.Items.Insert(1, tmpNextNext);

		var wlbScheduledShippingDate = GetWrappedControl<WrappedLabel>(riTarget, "lbScheduledShippingDate");
		wlbScheduledShippingDate.Text = DateTimeUtility.ToStringFromRegion(
			tmpDate,
			DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);
		shipping.FirstShippingDate = tmpDate;
	}

	/// <summary>
	/// Insert shipping date
	/// </summary>
	/// <param name="wddlShippingDate">Wrapped drop down list shipping date</param>
	/// <param name="index">Index</param>
	/// <param name="nextDate">Next date</param>
	private void InsertShippingDate(WrappedDropDownList wddlShippingDate, int index, ListItem nextDate)
	{
		index = Constants.DISPLAY_DEFAULTSHIPPINGDATE_ENABLED
			? index + 1
			: index;
		if (wddlShippingDate.Items.Count > index)
		{
			wddlShippingDate.Items.RemoveAt(index);
		}
		if (wddlShippingDate.Items.Count != 0)
		{
			var shippingDates = wddlShippingDate.Items.Cast<ListItem>().ToArray();
			var isSelected = shippingDates.Count(listItem => listItem.Value == nextDate.Value) > 0;
			if (isSelected)
			{
				wddlShippingDate.SelectedValue = nextDate.Value;
			}
			else
			{
				wddlShippingDate.Items.Insert(index, nextDate);
				wddlShippingDate.SelectedIndex = index;
			}
		}
	}

	/// <summary>
	/// Convert to list item
	/// </summary>
	/// <param name="date">Date</param>
	/// <returns>List item</returns>
	private ListItem ConvertToListItem(DateTime date)
	{
		var result = new ListItem(
			DateTimeUtility.ToStringFromRegion(date, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter),
			date.ToString("yyyy/MM/dd"));
		return result;
	}

	/// <summary>
	/// Check shipping pattern for first shipping date
	/// </summary>
	/// <param name="sender"></param>
	private void CheckShippingPatternForFirstShippingDate(object sender)
	{
		var riCart = GetParentRepeaterItem((Control)sender, "rCartList");
		var riCartShippingx = GetParentRepeaterItem((Control)sender, "rCartShippings");
		var riTarget = riCartShippingx ?? riCart;
		var shippingIndex = (riCartShippingx != null) ? riCartShippingx.ItemIndex : 0;

		// 定期購入情報取得（OrderCartPage_OrderShipping.csと同様の処理）
		string validationName = this.IsLoggedIn ? "OrderShipping" : "OrderShippingGuest";

		Hashtable htFixedPurchase;
		string kbn;
		string setting;
		CreateFixedPurchaseData(riTarget, out htFixedPurchase, out kbn, out setting);

		var wddlFirstShippingDateTmp = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlFirstShippingDate");
		var wddlShippingDateTmp = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlShippingDate");
		var wdtShippingDate = GetWrappedControl<WrappedHtmlGenericControl>(riTarget, "dtShippingDate");
		var wddShippingDate = GetWrappedControl<WrappedHtmlGenericControl>(riTarget, "ddShippingDate");

		// Check if option is enabled and flag is valid
		var shopShipping = this.ShopShippingList[riCart.ItemIndex];
		var isMail = this.CartList.Items[riCart.ItemIndex].Shippings[shippingIndex].ShippingMethod
			== Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL;
		if ((shopShipping.CanUseFixedPurchaseFirstShippingDateNextMonth(kbn) == false)
			&& (isMail == false)
			&& (shopShipping.ShippingDateSetFlg
				== Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID))
		{
			var dateEndTmp = (int)this.ShopShippingList[riTarget.ItemIndex].ShippingDateSetEnd + 1;
			wddlFirstShippingDateTmp.Visible = false;
			wdtShippingDate.Visible = true;
			wddShippingDate.Visible = true;
			wddlShippingDateTmp.Visible = true;
			if (wddlShippingDateTmp.Items.Count > dateEndTmp) wddlShippingDateTmp.Items.RemoveAt(dateEndTmp);
		}
		else
		{
			wddlFirstShippingDateTmp.Visible = true;
			wdtShippingDate.Visible = false;
			wddShippingDate.Visible = false;
			wddlShippingDateTmp.Visible = false;
			wddlShippingDateTmp.SelectedIndex = -1;
			if (Validator.Validate(validationName, htFixedPurchase) != string.Empty) wddlFirstShippingDateTmp.Visible = false;
		}
	}

	/// <summary>
	/// Dropdown list first shipping date item selected
	/// </summary>
	/// <param name="sender"></param>
	public void ddlNextShippingDate_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		// Update first shipping date label value
		var riTarget = GetOuterControl((Control)sender, typeof(RepeaterItem));
		var wlblNextShippingDate = GetWrappedControl<WrappedLabel>(riTarget, "lblNextShippingDate");
		var wddlNextShippingDate = GetWrappedControl<WrappedDropDownList>(riTarget, "ddlNextShippingDate");

		wlblNextShippingDate.Text = wddlNextShippingDate.SelectedItem.Text;
	}

	///*********************************************************************************************
	/// <summary>
	/// 定期購入設定
	/// </summary>
	///*********************************************************************************************
	protected class FixedPurchaseSetting
	{
		/// <summary>配送先ごとのKbn有効/無効</summary>
		public bool[] KbnValidFlags
		{
			get
			{
				return new[]
				{
					this.Kbn1ValidFlag,
					this.Kbn2ValidFlag,
					this.Kbn3ValidFlag,
					this.Kbn4ValidFlag
				};
			}
		}
		/// <summary>配送先ごとのKbn1有効/無効</summary>
		public bool Kbn1ValidFlag { get; set; }
		/// <summary>配送先ごとのKbn2有効/無効</summary>
		public bool Kbn2ValidFlag { get; set; }
		/// <summary>配送先ごとのKbn3有効/無効</summary>
		public bool Kbn3ValidFlag { get; set; }
		/// <summary>配送先ごとのKbn4有効/無効</summary>
		public bool Kbn4ValidFlag { get; set; }
		/// <summary>配送先ごとのKbn初期チェック状態</summary>
		public bool[] KbnCheckedValues
		{
			get
			{
				return new[]
				{
					this.Kbn1CheckedValue,
					this.Kbn2CheckedValue,
					this.Kbn3CheckedValue,
					this.Kbn4CheckedValue
				};
			}
		}
		/// <summary>配送先ごとのKbn1初期チェック状態</summary>
		public bool Kbn1CheckedValue { get; set; }
		/// <summary>配送先ごとのKbn2初期チェック状態</summary>
		public bool Kbn2CheckedValue { get; set; }
		/// <summary>配送先ごとのKbn3初期チェック状態</summary>
		public bool Kbn3CheckedValue { get; set; }
		/// <summary>配送先ごとのKbn4初期チェック状態</summary>
		public bool Kbn4CheckedValue { get; set; }
		/// <summary>Kbn1チェック時: ddlFixedPurchaseMonthの初期選択状態</summary>
		public string MonthSelectedValue { get; set; }
		/// <summary>Kbn1チェック時: ddlFixedPurchaseMonthlyDateの初期選択状態</summary>
		public string MonthlyDateSelectedValue { get; set; }
		/// <summary>Kbn2チェック時: ddlFixedPurchaseIntervalMonthsの初期選択状態</summary>
		public string IntervalMonthsSelectedValue { get; set; }
		/// <summary>Kbn2チェック時: ddlFixedPurchaseWeekOfMonthの初期選択状態</summary>
		public string WeekOfMonthSelectedValue { get; set; }
		/// <summary>Kbn2チェック時: ddlFixedPurchaseDayOfWeekの初期選択状態</summary>
		public string DayOfWeekSelectedValue { get; set; }
		/// <summary>Kbn3チェック時: ddlFixedPurchaseIntervalDaysの初期選択状態</summary>
		public string IntervalDaysSelectedValue { get; set; }
		/// <summary>Kbn4チェック時: ddlFixedPurchaseEveryNWeek_Weekの初期選択状態</summary>
		public string EveryNWeekWeekSelectedValue { get; set; }
		/// <summary>Kbn4チェック時: ddlFixedPurchaseEveryNWeek_DayOfWeekの初期選択状態</summary>
		public string EveryNWeekDayOfWeekSelectedValue { get; set; }
		/// <summary>定期購入：配送パターンの指定表示/非表示</summary>
		public bool FixedPurchaseShippingDisplayFlg { get; set; }
	}
	#endregion
}
