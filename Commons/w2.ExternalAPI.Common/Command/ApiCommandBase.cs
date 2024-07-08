/*
=========================================================================================================
  Module      : APIコマンドの抽象クラス(ApiCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using w2.ExternalAPI.Common.Logging;

namespace w2.ExternalAPI.Common.Command
{
	/// <summary>
	/// コマンド実行結果ステータス列挙体
	/// </summary>
	public enum EnumResultStatus { Complete = 0, Warn = 1, Faile = 2, Na = 3 }

	/// <summary>
	///	APIコマンドの抽象クラス
	/// </summary>
	/// <remarks>
	/// 各コマンドはこのクラスを実装すること
	/// </remarks>
	public abstract class ApiCommandBase
	{
		
		#region メンバ変数
		protected ApiCommandResult m_apiComandResult;
		#endregion

		#region コンストラクタ
		/// <summary>コンストラクタ</summary>
		public ApiCommandBase()
		{
			Init();
		}
		#endregion

		#region #Init 初期化処理
		/// <summary>初期化処理</summary>
		protected virtual void Init() 
		{
			// 起動ログ
			ApiLogger.Write(LogLevel.info, "コマンド開始:" + GetType().Name, "");
		}
		#endregion

		#region #Execute コマンド実行処理
		/// <summary>コマンド実行処理</summary>
		protected abstract ApiCommandResult Execute(ApiCommandArgBase apiCommandArg);
		#endregion

		#region #End 終了処理
		/// <summary>終了処理</summary>
		protected virtual void End() 
		{
			// 終了ログ
			ApiLogger.Write(LogLevel.info, "コマンド終了:" + GetType().Name, "");
		}
		#endregion

		#region +Do 実行する
		/// <summary>実行する</summary>
		/// <remarks>Execute→Endの順番で実行する</remarks>
		public ApiCommandResult Do(ApiCommandArgBase apiCommandArg)
		{
			m_apiComandResult = Execute(apiCommandArg);
			
			// 終了処理
			End();

			return m_apiComandResult;
		}
		#endregion
	
	}
}
