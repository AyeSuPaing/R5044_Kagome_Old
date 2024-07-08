/*
=========================================================================================================
  Module      : CSレポートページ(ReportPageCs.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;
using w2.App.Common;
using w2.App.Common.User;
using w2.App.Common.Cs.Reports;

/// <summary>
/// ReportPageCs の概要の説明です
/// </summary>
public class ReportPageCs : BasePageCs
{
	/// <summary> CSレポート定義 </summary>
	public const string CS_REPORT = "CsReport";
	/// <summary> 曜日リスト定義 </summary>
	public const string WEEK_DAY_LIST = "week_day_list";

	#region #GetMonthList 月リスト取得
	/// <summary>
	/// 月リスト取得
	/// </summary>
	/// <param name="begin">開始</param>
	/// <param name="end">終了</param>
	/// <returns>リスト</returns>
	protected int[] GetMonthList(DateTime begin, DateTime end)
	{
		var result = new List<int>();
		for (var dt = DateTime.Parse(begin.ToString("yyyy/MM/01")); dt <= end; dt = dt.AddMonths(1))
		{
			if (result.Contains(dt.Month)) break;
			result.Add(dt.Month);
		}
		result.Sort();
		return result.ToArray();
	}
	#endregion

	#region #GetMonthDayList 月-日リスト取得
	/// <summary>
	/// 月-日リスト取得
	/// </summary>
	/// <param name="begin">開始</param>
	/// <param name="end">終了</param>
	/// <returns>リスト</returns>
	protected DateTime[] GetMonthDayList(DateTime begin, DateTime end)
	{
		var monthDayList = new List<DateTime>();
		for (var dt = begin; dt <= end; dt = dt.AddDays(1))
		{
			monthDayList.Add(dt);
		}
		return monthDayList.ToArray();
	}
	#endregion

	#region #GetWeekdayList 曜日リスト取得
	/// <summary>
	/// 曜日リスト取得
	/// </summary>
	/// <param name="begin">開始</param>
	/// <param name="end">終了</param>
	/// <returns>リスト</returns>
	protected int[] GetWeekdayList(DateTime begin, DateTime end)
	{
		var result = new List<int>();
		for (var dt = begin; dt <= end; dt = dt.AddDays(1))
		{
			if (result.Contains((int)dt.DayOfWeek)) break;
			result.Add((int)dt.DayOfWeek);
		}
		result.Sort();
		return result.ToArray();
	}
	#endregion

	#region #DispReportValue レポート値表示
	/// <summary>
	/// レポート値表示
	/// </summary>
	/// <param name="value">値</param>
	/// <param name="nullValue">値がnullの場合の代替文字</param>
	/// <returns>表示値（3桁区切りの数値）</returns>
	protected string DispReportValue(int? value, string nullValue)
	{
		return value.HasValue ? StringUtility.ToNumeric(value) : nullValue;
	}
	#endregion

	#region #GetDisplayName 表示名取得
	/// <summary>
	/// 表示名取得
	/// </summary>
	/// <param name="row">行モデル</param>
	/// <returns>名称</returns>
	protected string GetDisplayName(ReportRowModel row)
	{
		string name;
		if (row.Name == "")
		{
			name = (row.Id != "") ? "(削除済)" : "(未設定)";
		}
		else
		{
			name = row.Name;
		}
		return (row.IsIndent ? "　　" : "") + name;
	}
	#endregion
	#region #GetDisplayName 表示名取得
	/// <summary>
	/// 表示名取得
	/// </summary>
	/// <param name="row">行モデル</param>
	/// <returns>名称</returns>
	protected string GetDisplayName(ReportMatrixRowModelForCsWorkflow row)
	{
		string name;
		if (row.Name == "")
		{
			name = "(削除済)";
		}
		else
		{
			name = row.Name;
		}
		return (row.IsIndent ? "　　" : "") + name;
	}
	#endregion
}