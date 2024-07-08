/*
=========================================================================================================
  Module      : レポート行モデル(ReportRowModel.cs)
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
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.Reports
{
	/// <summary>
	/// レポート行モデル
	/// </summary>
	[Serializable]
	public class ReportRowModel : ModelBase<ReportRowModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ReportRowModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ReportRowModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ReportRowModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="count">件数</param>
		/// <param name="valid">有効状態</param>
		public ReportRowModel(string name, int? count, bool valid)
			: this()
		{
			this.Name = name;
			this.Count = count;
			this.Valid = valid;
		}
		#endregion
	
		#region プロパティ

		/// <summary>名称</summary>
		public string Name
		{
			get { return StringUtility.ToEmpty(this.DataSource["Name"]); }
			set { this.DataSource["Name"] = value; }
		}
		/// <summary>名称2（グループ-オペレータ向け）</summary>
		public string Name2
		{
			get { return StringUtility.ToEmpty(this.DataSource["Name2"]); }
			set { this.DataSource["Name2"] = value; }
		}
		/// <summary>件数</summary>
		public int? Count
		{
			get { return (int?)this.DataSource["Count"]; }
			set { this.DataSource["Count"] = value; }
		}
		/// <summary>有効状態</summary>
		public bool Valid
		{
			get { return StringUtility.ToEmpty(this.DataSource["valid_flg"]) == "1"; }
			set { this.DataSource["valid_flg"] = value ? "1" : "0"; }
		}
		/// <summary>有効状態2（グループ-オペレータ向け。グループが有効かを格納）</summary>
		public bool Valid2
		{
			get { return StringUtility.ToEmpty(this.DataSource["valid_flg2"]) == "1"; }
			set { this.DataSource["valid_flg2"] = value ? "1" : "0"; }
		}
		/// <summary>ランク考慮有効状態（グループ向け。親グループの有効状態を引き継いで有効かを格納）</summary>
		public bool RankedValid
		{
			get { return StringUtility.ToEmpty(this.DataSource["ranked_valid_flg"]) == "1"; }
			set { this.DataSource["ranked_valid_flg"] = value ? "1" : "0"; }
		}
		/// <summary>ID（グループ、オペレータのみ。集計元のIDを格納してマスタにあるかを判定）</summary>
		public string Id
		{
			get { return StringUtility.ToEmpty(this.DataSource["Id"]); }
			set { this.DataSource["Id"] = value; }
		}
		/// <summary>ID2（グループ-オペレータ向け。集計元のIDを格納してマスタにあるかを判定）</summary>
		public string Id2
		{
			get { return StringUtility.ToEmpty(this.DataSource["Id2"]); }
			set { this.DataSource["Id2"] = value; }
		}
		/// <summary>インデントするか（階層表示に利用）</summary>
		public bool IsIndent
		{
			get { return (bool)(this.DataSource["IsIndent"] ?? false); }
			set { this.DataSource["IsIndent"] = value; }
		}
		/// <summary>ランクNO（カテゴリで利用）</summary>
		public int RankNo
		{
			get { return (int)this.DataSource["rank_no"]; }
			set { this.DataSource["rank_no"] = value; }
		}
		/// <summary>月（月別集計のみ）</summary>
		public int? Month
		{
			get { return (int?)this.DataSource["Month"]; }
			set { this.DataSource["Month"] = value; }
		}
		/// <summary>月日（月日集計のみ）</summary>
		public string MonthDay
		{
			get { return StringUtility.ToEmpty(this.DataSource["MonthDay"]); }
			set { this.DataSource["MonthDay"] = value; }
		}
		/// <summary>曜日（曜日、曜日-時間帯集計のみ）</summary>
		public int? Weekday
		{
			get { return (int?)this.DataSource["Weekday"]; }
			set { this.DataSource["Weekday"] = value; }
		}
		/// <summary>時間（時間帯集計のみ）</summary>
		public int? Hour
		{
			get { return (int?)this.DataSource["Hour"]; }
			set { this.DataSource["Hour"] = value; }
		}
		/// <summary>メディア区分（メッセージのみ）</summary>
		public string MediaKbn
		{
			get { return StringUtility.ToEmpty(this.DataSource["MediaKbn"]); }
			set { this.DataSource["MediaKbn"] = value; }
		}
		/// <summary>受発信区分（メッセージのみ）</summary>
		public string DirectionKbn
		{
			get { return StringUtility.ToEmpty(this.DataSource["DirectionKbn"]); }
			set { this.DataSource["DirectionKbn"] = value; }
		}
		#endregion
	}
}
