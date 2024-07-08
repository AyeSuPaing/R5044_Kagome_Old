/*
=========================================================================================================
  Module      : コマンド基底クラス(CommandBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Logger;

namespace w2.Commerce.Batch.ExternalOrderImport.Commands
{
	/// <summary>
	/// コマンド基底クラス
	/// </summary>
	public abstract class CommandBase : ICommand
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CommandBase()
		{
			this.ExecuteResult = new CommandResultBase { ErrorLog = new List<string>() };
		}

		/// <summary>
		/// 開始前処理
		/// </summary>
		public void OnStart()
		{
			this.ExecuteResult.BeginTime = DateTime.Now;
		}

		/// <summary>
		/// 実行
		/// </summary>
		public abstract void Exec();

		/// <summary>
		/// 終了処理
		/// </summary>
		public void OnComplete()
		{
			this.ExecuteResult.EndTime = DateTime.Now;
			WriteErrorLog();
		}

		/// <summary>
		/// 異常時処理
		/// </summary>
		public void OnError()
		{
			this.ExecuteResult.EndTime = DateTime.Now;
			WriteErrorLog();
		}

		/// <summary>
		/// エラーログ出力
		/// </summary>
		protected void WriteErrorLog()
		{
			this.ExecuteResult.ErrorLog.ForEach(result => FileLogger.WriteError(result));
		}

		/// <summary>
		/// 文字列表現取得
		/// </summary>
		/// <returns>文字列表現</returns>
		public override string ToString()
		{
			return this.GetType().Name;
		}

		/// <summary>処理実行結果</summary>
		public CommandResultBase ExecuteResult { get; set; }
	}
}