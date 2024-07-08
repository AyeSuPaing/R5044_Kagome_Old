/*
=========================================================================================================
  Module      : テスト用APIコマンドクラス(TestApiCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

#region Import

using System;


#endregion

namespace w2.ExternalAPI.Common.Command.ApiCommand.Test
{
	///**************************************************************************************
	/// <summary>
	///	テスト用APIコマンドクラス
	/// </summary>
	/// <remarks>
	/// テスト用のコマンド実装
	/// </remarks>
	///**************************************************************************************
	[Api("TestApiCommand")]
	public class TestApiCommand : ApiCommandBase
	{
		#region #Init 初期処理
		protected override void Init()
		{
		}
		#endregion

		#region #Execute コマンド実行処理
		protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg)
		{
			//かならずキャスト
			TestApiCommandArg testApiCommandArg = (TestApiCommandArg) apiCommandArg;
			
			//パラメタ分解
			Console.WriteLine("テストメソッド実行:" + testApiCommandArg.Para1);
			Console.WriteLine("テストメソッド実行:" + testApiCommandArg.Para2);
			Console.WriteLine("テストメソッド実行:" + testApiCommandArg.Para3);

			//実行結果返却
			return  new ApiCommandResult(EnumResultStatus.Complete);
		}
		#endregion

		#region #End 終了処理
		protected override void End()
		{

		}
		#endregion
	}
}
