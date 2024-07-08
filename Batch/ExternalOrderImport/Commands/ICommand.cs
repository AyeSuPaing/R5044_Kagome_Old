/*
=========================================================================================================
  Module      : コマンドインターフェース(ICommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Commerce.Batch.ExternalOrderImport.Commands
{
	/// <summary>
	/// コマンドインターフェース
	/// </summary>
	interface ICommand
	{
		/// <summary>
		/// 開始前処理
		/// </summary>
		void OnStart();

		/// <summary>
		/// 実行
		/// </summary>
		void Exec();

		/// <summary>
		/// 終了処理
		/// </summary>
		void OnComplete();

		/// <summary>
		/// 異常時処理
		/// </summary>
		void OnError();

		/// <summary>
		/// 文字列表現取得
		/// </summary>
		/// <returns>文字列表現</returns>
		string ToString();

		/// <summary>処理実行結果</summary>
		CommandResultBase ExecuteResult { get; set; }
	}
}