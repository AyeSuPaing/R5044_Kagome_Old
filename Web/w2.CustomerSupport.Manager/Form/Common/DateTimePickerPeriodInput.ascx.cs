/*
=========================================================================================================
  Module      : Date Time Picker Period Input (DateTimePickerPeriodInput.ascx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using w2.App.Common.Web.WebCustomControl;

/// <summary>
/// Date time picker period input
/// </summary>
public partial class Form_Common_DateTimePickerPeriodInput : DateTimePickerPeriodInputControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void Page_Load(object sender, EventArgs e)
	{
		base.Page_Load(sender, e);
	}

	#region +GetDateInput 入力日付取得（エラーであればエラーページ遷移）
	/// <summary>
	/// 入力日付取得（エラーであればエラーページ遷移）
	/// </summary>
	/// <param name="begin">開始日</param>
	/// <param name="end">終了日</param>
	public void GetDateInput(out DateTime begin, out DateTime end)
	{
		var errorMessages = new List<string>();

		var culture =
			string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)
				? CultureInfo.CurrentCulture
				: new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE);

		var isSuccess = DateTime.TryParse(this.StartDateTimeString, culture, DateTimeStyles.None, out begin);
			isSuccess = DateTime.TryParse(this.EndDateTimeString, culture, DateTimeStyles.None, out end);

		if ((errorMessages.Count == 0) && (end - begin).Days > 100)
		{
			errorMessages.Add(WebMessages.GetMessages(WebMessages.ERROR_MANAGER_PERIOD_WITHIN_100_DAYS));
		}
		if (errorMessages.Count != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = string.Join("<br />", errorMessages.ToArray());
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}
	#endregion


	#region +SetDefaultDateIfEmpty 日付指定が空ならデフォルト値をセットする
	/// <summary>
	/// 日付指定が空ならデフォルト値をセットする
	/// </summary>
	public void SetDefaultDateIfEmpty()
	{
		if ((string.IsNullOrEmpty(this.HfStartDate.Value))
			&& (string.IsNullOrEmpty(this.HfStartTime.Value))
			&& (string.IsNullOrEmpty(this.HfEndDate.Value))
			&& (string.IsNullOrEmpty(this.HfEndTime.Value)))
		{
			this.HfStartDate.Value = DateTimeUtility.GetDisplayDateString(DateTime.Now.AddDays(-100));
			this.HfStartTime.Value = "00:00:00";
			this.HfEndDate.Value = DateTimeUtility.GetDisplayDateString(DateTime.Now);
			this.HfEndTime.Value = "23:59:59";
		}
	}
	#endregion
}