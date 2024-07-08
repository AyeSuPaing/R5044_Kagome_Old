/*
=========================================================================================================
  Module      : 相対カレンダークラス(RelativeCalendar.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Util
{

	/// <summary>
	/// 相対カレンダークラス
	/// 
	/// 今週２週間つまり、２週間前から今週末まで、という期間を算出する。
	/// 今週２週間といって、今週と来週を意味したい場合は、TodayF[to Future]とかそういうメソッドを追加してください。
	/// </summary>
	public class RelativeCalendar
	{
		/// <summary>
		/// "2w" ⇒ 今週２週間 というように文字列から期間を取得する
		/// 例えば今日が7/19日金曜日だとすると、7/7(日)～7/20（土）までが今週２週間となる。
		/// "0w"はだめ。１以上で指定すること。
		///
		/// 未実装：未来を追加するなら"1wf"。末尾fで判定。
		/// </summary>
		/// <param name="txt"></param>
		/// <returns></returns>
		/// <example>RelativeCalendar.FromText("2w"); // 2W -> OK</example>
		/// <example>RelativeCalendar.FromText("2d");</example>
		/// <example>RelativeCalendar.FromText("2y");</example>
		/// <example>RelativeCalendar.FromText("2"); // error!! </example>
		public static AbsoluteTimeSpan FromText(string txt)
		{
			char type;
			int num;

			// 期間指定文字列と計算の対応定義
			var methodDic =
				new Dictionary<char, Func<int, AbsoluteTimeSpan>>()
				{
					{'d', (n) => Today(n)},
					{'w', (n) => ThisWeek(n)},
					{'m', (n) => ThisMonth(n)},
					{'y', (n) => ThisYear(n)},
				};

			// パース
			try
			{
				type = txt.ToLower()[txt.Length - 1];
				num = int.Parse(txt.Substring(0, txt.Length - 1));

				if (methodDic.Keys.Contains(type) == false) throw new ArgumentException(string.Format("不適切な期間指定文字が使用されました:'{0}'", type));
			}
			catch(Exception ex)
			{
				throw new ArgumentException(string.Format("パースに失敗しました:'{0}'", txt), ex);
			}

			// 計算
			return methodDic[type].Invoke(num);
		}

		/// <summary>
		/// n日前から今日までの期間
		/// </summary>
		/// <param name="n">日数</param>
		/// <returns>期間</returns>
		public static AbsoluteTimeSpan Today(int n)
		{
			return new AbsoluteTimeSpan(
				DateTime.Today.AddDays(-n + 1), 
				DateTime.Today);
		}

		/// <summary>
		/// n週間前～今週までの期間
		/// </summary>
		/// <param name="n">週</param>
		/// <returns>期間</returns>
		public static AbsoluteTimeSpan ThisWeek(int n)
		{
			return new AbsoluteTimeSpan(
				DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).AddDays(7 * (-n + 1)),
				DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 6));
		}

		/// <summary>
		/// n月前～今月までの期間
		/// </summary>
		/// <param name="n">月</param>
		/// <returns>期間</returns>
		public static AbsoluteTimeSpan ThisMonth(int n)
		{
			return new AbsoluteTimeSpan(
				DateTime.Today.AddDays(- DateTime.Today.Day + 1).AddMonths(-n + 1),
				DateTime.Today.AddMonths(1).AddDays(- DateTime.Today.Day));
		}

		/// <summary>
		/// n年前～今年までの期間
		/// </summary>
		/// <param name="n">年</param>
		/// <returns>期間</returns>
		public static AbsoluteTimeSpan ThisYear(int n)
		{
			return new AbsoluteTimeSpan(
				new DateTime(DateTime.Today.Year, 1, 1).AddYears(-n + 1),
				new DateTime(DateTime.Today.Year, 12, 31));
		}
	}
}
