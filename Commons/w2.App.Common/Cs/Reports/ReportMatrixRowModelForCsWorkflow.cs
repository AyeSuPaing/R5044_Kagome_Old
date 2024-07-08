/*
=========================================================================================================
  Module      : 業務フロー集計向けレポートマトリクス行モデル(ReportMatrixRowModelForCsWorkflow.cs)
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
	/// 業務フロー集計向けレポートマトリクス行モデル
	/// </summary>
	[Serializable]
	public class ReportMatrixRowModelForCsWorkflow : ModelBase<ReportMatrixRowModelForCsWorkflow>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ReportMatrixRowModelForCsWorkflow()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ReportMatrixRowModelForCsWorkflow(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ReportMatrixRowModelForCsWorkflow(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>名称</summary>
		public string Name
		{
			get { return StringUtility.ToEmpty(this.DataSource["Name"]); }
			set { this.DataSource["Name"] = value; }
		}
		/// <summary>有効状態</summary>
		public bool Valid
		{
			get { return StringUtility.ToEmpty(this.DataSource["valid_flg"]) == "1"; }
			set { this.DataSource["valid_flg"] = value ? "1" : "0"; }
		}
		/// <summary>インデントするか（グループ-オペレータのとき等で利用）</summary>
		public bool IsIndent
		{
			get { return (bool)(this.DataSource["IsIndent"] ?? false); }
			set { this.DataSource["IsIndent"] = value; }
		}
		/// <summary>承認依頼・依頼数</summary>
		public int ApprReqCount
		{
			get { return (int?)this.DataSource["ApprReqCount"] ?? 0; }
			set { this.DataSource["ApprReqCount"] = value; }
		}
		/// <summary>承認依頼・取り下げ数</summary>
		public int ApprReqCancelCount
		{
			get { return (int?)this.DataSource["ApprReqCancelCount"] ?? 0; }
			set { this.DataSource["ApprReqCancelCount"] = value; }
		}
		/// <summary>承認対応・OK件数</summary>
		public int ApprOkCount
		{
			get { return (int?)this.DataSource["ApprOkCount"] ?? 0; }
			set { this.DataSource["ApprOkCount"] = value; }
		}
		/// <summary>承認対応・NG件数</summary>
		public int ApprNgCount
		{
			get { return (int?)this.DataSource["ApprNgCount"] ?? 0; }
			set { this.DataSource["ApprNgCount"] = value; }
		}
		/// <summary>送信依頼・依頼数</summary>
		public int SendReqCount
		{
			get { return (int?)this.DataSource["SendReqCount"] ?? 0; }
			set { this.DataSource["SendReqCount"] = value; }
		}
		/// <summary>送信依頼・取り下げ数</summary>
		public int SendReqCancelCount
		{
			get { return (int?)this.DataSource["SendReqCancelCount"] ?? 0; }
			set { this.DataSource["SendReqCancelCount"] = value; }
		}
		/// <summary>送信対応・OK件数</summary>
		public int SendOkCount
		{
			get { return (int?)this.DataSource["SendOkCount"] ?? 0; }
			set { this.DataSource["SendOkCount"] = value; }
		}
		/// <summary>送信対応・NG件数</summary>
		public int SendNgCount
		{
			get { return (int?)this.DataSource["SendNgCount"] ?? 0; }
			set { this.DataSource["SendNgCount"] = value; }
		}
		#endregion
	}
}
