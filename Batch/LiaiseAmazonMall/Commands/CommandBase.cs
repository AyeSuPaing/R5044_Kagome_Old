/*
=========================================================================================================
  Module      : コマンド基底クラス(CommandBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.Batch.LiaiseAmazonMall.Commands
{
	/// <summary>
	/// コマンド基底クラス
	/// </summary>
	public abstract class CommandBase : ICommand
	{
		/// <summary>
		/// 開始時処理
		/// </summary>
		public abstract void OnStart();

		/// <summary>
		/// 実行
		/// </summary>
		public abstract void Exec();

		/// <summary>
		/// 完了時処理
		/// </summary>
		public abstract void OnComplete();

		/// <summary>
		/// エラー時処理
		/// </summary>
		public abstract void OnError();
	}
}
