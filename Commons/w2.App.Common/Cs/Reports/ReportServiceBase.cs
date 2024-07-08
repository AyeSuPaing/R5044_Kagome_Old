/*
=========================================================================================================
  Module      : 基底レポートサービス(ReportServiceBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Sql;
using w2.App.Common.Cs.CsOperator;

namespace w2.App.Common.Cs.Reports
{
	/// <summary>
	/// 基底レポートサービス
	/// </summary>
	public abstract class ReportServiceBase
	{
		/// <summary>レポジトリ</summary>
		protected ReportRepositoryBase Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public ReportServiceBase(ReportRepositoryBase repository)
		{
			this.Repository = repository;
		}
		#endregion

		#region +GetGroupReport グループ毎レポート取得
		/// <summary>
		/// グループ毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>モデル</returns>
		public ReportRowModel[] GetGroupReport(Hashtable ht)
		{
			var dv = this.Repository.GetGroupReport(ht);
			var models = (from DataRowView drv in dv select new ReportRowModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region +GetOperatorReport オペレータ毎レポート取得
		/// <summary>
		/// オペレータ毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>モデル</returns>
		public ReportRowModel[] GetOperatorReport(Hashtable ht)
		{
			var dv = this.Repository.GetOperatorReport(ht);
			var models = (from DataRowView drv in dv select new ReportRowModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region +GetGroupOperatorReport グループ-オペレータ毎レポート取得
		/// <summary>
		/// グループ-オペレータ毎レポート取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="ht">パラメタ</param>
		/// <returns>モデル</returns>
		public ReportRowModel[] GetGroupOperatorReport(string deptId, Hashtable ht)
		{
			var list = new List<ReportRowModel>();

			var dv = this.Repository.GetGroupOperatorReport(ht);
			var groupOperatorReports = (from DataRowView drv in dv select new ReportRowModel(drv)).ToArray();

			// オペレータはインデントする
			groupOperatorReports.ToList().ForEach(o => o.IsIndent = true);

			// グループ用の行を追加したものを作成
			ReportRowModel currentGroup = null;	// キーブレイク用
			foreach (var gor in groupOperatorReports)
			{
				if ((currentGroup == null)
					|| (currentGroup.Id != gor.Id2))
				{
					var groupRow = new ReportRowModel();
					groupRow.Name = gor.Name2;
					groupRow.Valid = gor.Valid2;
					groupRow.Id = gor.Id2;
					groupRow.Count = null;
					list.Add(groupRow);
					currentGroup = groupRow;
				}
				list.Add(gor);
			}
			return list.ToArray();
		}
		#endregion

		#region +GetCategoryReport カテゴリ毎レポート取得
		/// <summary>
		/// カテゴリ毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>モデル</returns>
		public ReportRowModel[] GetCategoryReport(Hashtable ht)
		{
			var dv = this.Repository.GetCategoryReport(ht);
			var models = (from DataRowView drv in dv select new ReportRowModel(drv)).ToArray();

			// 親カテゴリは子カテゴリの件数を含むように値セット
			AddCountToParentCategories(models);

			return models;
		}
		#endregion

		#region -AddCountToParentCategories 親カテゴリに件数を加算
		/// <summary>
		/// 親カテゴリに件数を加算
		/// </summary>
		/// <param name="models">レポート行モデル（値を変更します）</param>
		private void AddCountToParentCategories(ReportRowModel[] models)
		{
			List<ReportRowModel> temps = new List<ReportRowModel>();
			foreach (var row in models)
			{
				if (row.RankNo > temps.Count - 1)
				{
					temps.Add(row);
				}
				else
				{
					temps[row.RankNo] = row;
				}
				for (int i = 0; i < row.RankNo; i++)
				{
					temps[i].Count += row.Count;
				}
			}
		}
		#endregion

		#region +GetMonthReport 月毎レポート取得
		/// <summary>
		/// 月毎レポート取得
		/// </summary>
		/// <param name="monthList">月リスト</param>
		/// <param name="ht">パラメタ</param>
		/// <returns>モデル</returns>
		public ReportRowModel[] GetMonthReport(int[] monthList, Hashtable ht)
		{
			var dv = this.Repository.GetMonthReport(ht);
			var models = (from DataRowView drv in dv select new ReportRowModel(drv)).ToArray();

			var result = new List<ReportRowModel>();
			foreach (int month in  monthList)
			{
				var finds = models.Where(m => m.Month == month).ToArray();
				if (finds.Length == 0)
				{
					finds = new[] { new ReportRowModel("", 0, true) };
					finds[0].Month = month;
				}
				result.AddRange(finds);
			}
			result.ForEach(r => r.Name = r.Month.ToString());	// キーブレイクのためNameに一時的に値格納
			return result.ToArray();
		}
		#endregion

		#region +GetMonthDayReport 月-日毎レポート取得
		/// <summary>
		/// 月-日毎レポート取得
		/// </summary>
		/// <param name="monthDayList">月-日リスト</param>
		/// <param name="ht">パラメタ</param>
		/// <returns>モデル</returns>
		public ReportRowModel[] GetMonthDayReport(DateTime[] monthDayList, Hashtable ht)
		{
			var dv = this.Repository.GetMonthDayReport(ht);
			var models = (from DataRowView drv in dv select new ReportRowModel(drv)).ToArray();

			var result = new List<ReportRowModel>();
			foreach (DateTime monthDay in monthDayList)
			{
				var finds = models.Where(m => DateTime.Parse(m.MonthDay).Date == monthDay.Date).ToArray();
				if (finds.Length == 0)
				{
					finds = new[] { new ReportRowModel("", 0, true) };
					finds[0].MonthDay = monthDay.Date.ToString("yyyy/MM/dd");
				}
				result.AddRange(finds);
			}
			result.ForEach(r => r.Name = r.MonthDay);	// キーブレイクのためNameに一時的に値格納
			return result.ToArray();
		}
		#endregion
		
		#region +GetWeekdayReport 曜日毎レポート取得
		/// <summary>
		/// 曜日毎レポート取得
		/// </summary>
		/// <param name="weekdayList">曜日リスト</param>
		/// <param name="ht">パラメタ</param>
		/// <returns>モデル</returns>
		public ReportRowModel[] GetWeekdayReport(int[] weekdayList, Hashtable ht)
		{
			var dv = this.Repository.GetWeekdayReport(ht);
			var models = (from DataRowView drv in dv select new ReportRowModel(drv)).ToArray();

			var result = new List<ReportRowModel>();
			foreach (int weekday in weekdayList)
			{
				var finds = models.Where(m => m.Weekday == weekday).ToArray();
				if (finds.Length == 0)
				{
					finds = new[] { new ReportRowModel("", 0, true) };
					finds[0].Weekday = weekday;
				}
				result.AddRange(finds);
			}
			result.ForEach(r => r.Name = r.Weekday.ToString());	// キーブレイクのためNameに一時的に値格納
			return result.ToArray();
		}
		#endregion

		#region +GetTimeReport 時間帯毎レポート取得
		/// <summary>
		/// 時間帯毎レポート取得
		/// </summary>
		/// <param name="hourList">時間リスト</param>
		/// <param name="ht">パラメタ</param>
		/// <returns>モデル</returns>
		public ReportRowModel[] GetTimeReport(int[] hourList, Hashtable ht)
		{
			var dv = this.Repository.GetTimeReport(ht);
			var models = (from DataRowView drv in dv select new ReportRowModel(drv)).ToArray();

			var result = new List<ReportRowModel>();
			foreach (int hour in hourList)
			{
				var finds = models.Where(m => m.Hour == hour).ToArray();
				if (finds.Length == 0)
				{
					finds = new[] { new ReportRowModel("", 0, true) };
					finds[0].Hour = hour;
				}
				result.AddRange(finds);
			}
			result.ForEach(r => r.Name = r.Hour.ToString());	// キーブレイクのためNameに一時的に値格納
			return result.ToArray();
		}
		#endregion

		#region +GetWeekdayTimeReport 曜日-時間帯毎レポート取得
		/// <summary>
		/// 曜日-時間帯毎レポート取得
		/// </summary>
		/// <param name="weekdayList">曜日リスト</param>
		/// <param name="hourList">時間リスト</param>
		/// <param name="ht">パラメタ</param>
		/// <returns>モデル</returns>
		public ReportRowModel[] GetWeekdayTimeReport(int[] weekdayList, int[] hourList, Hashtable ht)
		{
			var dv = this.Repository.GetWeekdayTimeReport(ht);
			var models = (from DataRowView drv in dv select new ReportRowModel(drv)).ToArray();

			var result = new List<ReportRowModel>();
			foreach (int weekday in weekdayList)
			{
				var rowTmp = new ReportRowModel("", null, true);
				rowTmp.Weekday = weekday;
				result.Add(rowTmp);

				foreach (int hour in hourList)
				{
					var finds = models.Where(m => (m.Weekday == weekday) && (m.Hour == hour)).ToArray();
					if (finds.Length == 0)
					{
						finds = new[] { new ReportRowModel("", 0, true) };
						finds[0].Hour = hour;
					}
					result.AddRange(finds);
				}
			}
			result.ForEach(r => r.Name = r.Hour.ToString());	// キーブレイクのためNameに一時的に値格納
			return result.ToArray();
		}
		#endregion
	}
}
