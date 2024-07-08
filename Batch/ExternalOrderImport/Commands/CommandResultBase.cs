/*
=========================================================================================================
  Module      : コマンド実行結果基底クラス(CommandResultBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Commerce.Batch.ExternalOrderImport.Commands
{
	/// <summary>
	/// コマンド実行結果基底クラス
	/// </summary>
	public class CommandResultBase
	{
		/// <summary>処理名</summary>
		public string AppName { get; set; }
		/// <summary>処理開始時間</summary>
		public DateTime BeginTime { get; set; }
		/// <summary>処理終了時間</summary>
		public DateTime EndTime { get; set; }
		/// <summary>処理総件数</summary>
		public int ExecuteCount { get; set; }
		/// <summary>成功件数</summary>
		public int SuccessCount { get; set; }
		/// <summary>失敗件数</summary>
		public int FailureCount { get; set; }
		/// <summary>処理スキップ件数</summary>
		public int SkipCount { get; set; }
		/// <summary>エラーログ</summary>
		public List<string> ErrorLog { get; set; }
		/// <summary>メールテンプレートID</summary>
		public string MailTemplateId { get; set; }
	}
}