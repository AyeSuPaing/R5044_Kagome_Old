/*
=========================================================================================================
  Module      : カレンダーユーティリティモジュール(CalendarUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;

public class CalendarUtility
{
	/// <summary>
	/// 年月カレンダー作成
	/// </summary>
	/// <param name="currentYear">対象年</param>
	/// <param name="currentMonth">対象月</param>
	/// <param name="url">遷移先URL</param>
	/// <param name="urlParam">遷移先URLのパラメータ</param>
	/// <param name="paramNameYear">年パラメタ名</param>
	/// <param name="paramNameMonth">月パラメタ名</param>
	/// <param name="dispMonthLink">日付のリンクを表示するか</param>
	/// <param name="isShipmentForecastByDaysPage">日別出荷予測レポートページか</param>
	/// <param name="totalShippedForEachMonth">各月の出荷合計数</param>
	/// <returns>HTML</returns>
	public static string CreateHtmlYMCalendar(int currentYear, int currentMonth, string url, string urlParam, string paramNameYear, string paramNameMonth, bool dispMonthLink = true, bool isShipmentForecastByDaysPage = false, long[] totalShippedForEachMonth = null)
	{
		var backButtonUrl1 = Constants.PATH_ROOT + "Images/Common/paging_back_01.gif";
		var backButtonUrl2 = Constants.PATH_ROOT + "Images/Common/paging_back_02.gif";
		var nextButtonUrl1 = Constants.PATH_ROOT + "Images/Common/paging_next_01.gif";
		var nextButtonUrl2 = Constants.PATH_ROOT + "Images/Common/paging_next_02.gif";
		var yearLabel = (Constants.GLOBAL_OPTION_ENABLE
			&& (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false))
			? ""
			: "年";
		var monthLabel = (Constants.GLOBAL_OPTION_ENABLE
			&& (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false))
			? ""
			: "月";

		var html = new StringBuilder();
		html.Append("<table cellSpacing=\"0\" cellpadding=\"0\" width=\"250\" border=\"0\" align=\"center\" style=\"margin-left: auto; margin-right: auto;\" >");
		html.Append("<tr>");
		html.Append("<td align=\"center\" style=\"margin-left: auto; margin-right: auto;\" >");
		html.Append("<table cellpadding=\"1\" cellspacing=\"0\" border=\"0\" align=\"center\" style=\"margin-left: auto; margin-right: auto;\" >");
		html.Append("<tr>");
		html.Append("<td>");
		html.Append("<a href=\"").Append(WebSanitizer.UrlAttrHtmlEncode(url + "?" + paramNameYear + "=" + (currentYear-1).ToString() + "&" + paramNameMonth + "=" + currentMonth.ToString() + "&" + urlParam)).Append("\">");
		html.Append("<img src=\"").Append(backButtonUrl1).Append("\" border=\"0\" hspace=\"10\" onmouseover=\"JavaScript:this.src='").Append(backButtonUrl2).Append("'\" onmouseout=\"JavaScript:this.src='").Append(backButtonUrl1).Append("'\">");
		html.Append("</a>");
		html.Append("</td>");
		html.Append("<td>").Append(currentYear.ToString()).Append(yearLabel).Append("</td>");
		html.Append("<td>");
		html.Append("<a href=\"").Append(WebSanitizer.UrlAttrHtmlEncode(url + "?" + paramNameYear + "=" + (currentYear+1).ToString() + "&" + paramNameMonth + "=" + currentMonth.ToString() + "&" + urlParam)).Append("\">");
		html.Append("<img src=\"").Append(nextButtonUrl1).Append("\" border=\"0\" hspace=\"10\" onmouseover=\"JavaScript:this.src='").Append(nextButtonUrl2).Append("'\" onmouseout=\"JavaScript:this.src='").Append(nextButtonUrl1).Append("'\">");
		html.Append("</a>");
		html.Append("</td>");
		html.Append("</tr>");
		html.Append("</table>");
		html.Append("</td>");
		html.Append("</tr>");
		if (dispMonthLink)
		{
			html.Append("<tr>");
			html.Append("<td align=\"center\">");
			html.Append("<table cellpadding=\"1\" cellspacing=\"3\" border=\"0\" align=\"center\" style=\"margin-left: auto; margin-right: auto;\" >");
			html.Append("<tr>");
			for (var i = 1; i <= 12; i++)
			{
				if (isShipmentForecastByDaysPage
					&& (totalShippedForEachMonth[i - 1] == 0))
				{
					html.Append("<td class=\"calendar_unselectable_bg\">");
					html.Append("<nobr>").Append(i.ToString().PadRight(2, ' ')).Append(monthLabel + "</nobr>");
					html.Append("</a>");
					html.Append("</td>");

					if (i == 6) html.Append("</tr><tr>");
				}
				else
				{
					html.Append((i == currentMonth) ? "<td class=\"calendar_selected_bg\">" : "<td class=\"calendar_unselected_bg\">");
					html.Append("<a href=\"").Append(WebSanitizer.UrlAttrHtmlEncode(url + "?" + paramNameYear + "=" + currentYear.ToString() + "&" + paramNameMonth + "=" + i.ToString() + "&" + urlParam)).Append("\">");
					html.Append("<nobr>").Append(i.ToString().PadRight(2, ' ')).Append(monthLabel + "</nobr>");
					html.Append("</a>");
					html.Append("</td>");

					if (i == 6) html.Append("</tr><tr>");
				}
			}
			html.Append("</tr>");
			html.Append("</table>");
			html.Append("</td>");
			html.Append("</tr>");
		}
		html.Append("</table>");

		return html.ToString();
	}
}

