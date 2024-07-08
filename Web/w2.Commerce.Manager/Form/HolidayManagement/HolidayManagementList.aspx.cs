/*
=========================================================================================================
  Module      : 休日管理一覧 (HolidayManagementList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using w2.Common.Web;
using w2.Domain.Holiday;
using w2.Domain.Holiday.Helper;

public partial class Form_HolidayManagement_HolidayManagementList : BasePage
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
			// 休日情報取得
			var holidayModels = new HolidayService().Get();
			if (holidayModels.Length > 0)
			{
				rList.DataSource = CreateHolidayInfoForDisplay(holidayModels);
				rList.DataBind();
			}
			else
			{
				// 表示件数が０の場合、メッセージを表示
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}
		}
	}

	/// <summary>
	/// 表示のための休日一覧作成
	/// </summary>
	/// <param name="models">休日モデル</param>
	/// <returns>休日一覧</returns>
	private List<HolidaysInfoListItem> CreateHolidayInfoForDisplay(HolidayModel[] models)
	{
		// 初期化
		var yearBefore = models[0].Year;
		var dayNumbers = 0;
		var dateChanged = DateTime.MinValue;
		var lastChanged = "";
		var infoList = new List<HolidaysInfoListItem>();
		foreach (var model in models)
		{
			if (model.Year == yearBefore)
			{
				dayNumbers += model.Holidays.Length;
			}
			else
			{
				infoList.Add(new HolidaysInfoListItem(
					yearBefore,
					dayNumbers,
					dateChanged,
					lastChanged));

				yearBefore = model.Year;
				dayNumbers = model.Holidays.Length;
			}

			dateChanged = model.DateChanged;
			lastChanged = model.LastChanged;
		}

		// 最終アイテムを追加
		infoList.Add(new HolidaysInfoListItem(
			yearBefore,
			dayNumbers,
			dateChanged,
			lastChanged));

		return infoList;
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_HOLIDAY_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_INSERT)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 休日詳細ページのURL作成
	/// </summary>
	/// <param name="year">年</param>
	/// <returns>該当年の休日詳細ページのURL</returns>
	protected string CreateHolidayDetailUrl(int year)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_HOLIDAY_REGISTER)
			.AddParam(Constants.REQUEST_KEY_HOLIDAY_YEAR, year.ToString())
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE)
			.CreateUrl();
		return url;
	}
}
