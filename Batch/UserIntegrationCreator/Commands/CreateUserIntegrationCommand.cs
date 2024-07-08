/*
=========================================================================================================
  Module      : ユーザー統合情報作成コマンド(CreateUserIntegrationCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.UserIntegration;

namespace w2.Commerce.Batch.UserIntegrationCreator.Commands
{
	/// <summary>
	/// ユーザー統合情報作成コマンド
	/// </summary>
	public class CreateUserIntegrationCommand
	{
		#region +実行
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="targetStart">対象日時（開始）</param>
		/// <param name="targetEnd">対象日時（終了）</param>
		/// <returns>実行結果メッセージ</returns>
		public string Exec(DateTime targetStart, DateTime targetEnd)
		{
			// 名寄せを行いユーザー統合情報の登録・更新・削除を行う&実行結果メッセージを返す
			return new UserIntegrationService()
				.RegisterUserIntegrationAfterDuplicateIdentification(
					targetStart,
					targetEnd,
					Constants.PARALLEL_WORKERTHREADS,
					Constants.FLG_LASTCHANGED_BATCH);
		}
		#endregion
	}
}
