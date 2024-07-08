/*
=========================================================================================================
  Module      : コマンドインターフェース(ICommand.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.Batch.LiaiseFacebookMall.Commands
{
	/// <summary>
	/// コマンドインターフェース
	/// </summary>
	interface ICommand
	{
		/// <summary>
		/// 開始時処理
		/// </summary>
		void OnStart();

		/// <summary>
		/// 実行
		/// </summary>
		void Exec();

		/// <summary>
		/// 終了時処理
		/// </summary>
		void OnComplete();

		/// <summary>
		/// 異常時処理
		/// </summary>
		void OnError();
	}
}