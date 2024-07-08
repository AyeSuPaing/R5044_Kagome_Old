/*
=========================================================================================================
  Module      : バックアップ抽象クラス(BackUp.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/


namespace w2.ExternalAPI.Common.Logging
{
	/// <summary>
	///	バックアップの抽象クラス
	/// </summary>
	/// <remarks>
	/// バックアップ処理を行う場合はこのクラスを実装すること
	/// </remarks>
	public abstract class BackUp
	{
		#region メンバ変数
		private readonly ExecuteTarget m_executeTarget;
		private string m_backUpPath;
		#endregion

		#region コンストラクタ
		/// <summary>コンストラクタ</summary>
		/// <param name="executeTarget">ターゲット情報</param>
		protected BackUp(ExecuteTarget executeTarget)
		{
			m_executeTarget = executeTarget;
		}
		#endregion

		#region #Execute バックアップ実行
		/// <summary>
		/// バックアップ実行
		/// </summary>
		protected abstract void Execute(string backupPath);
		#endregion

		#region +Do 実行する
		/// <summary>実行する</summary>
		/// <remarks>
		/// CreateBackUpPath→PreExecute→Execute
		/// →PostExecutの順に処理実行
		/// </remarks>
		public string Do()
		{
			m_backUpPath = m_executeTarget.BackupPath;
			Execute(m_backUpPath);
			return m_backUpPath;
		}
		#endregion

		#region プロパティ
		/// <summary>ターゲット情報プロパティ</summary>
		protected ExecuteTarget ExecuteTarget
		{
			get { return m_executeTarget; }
		}

		/// <summary>バックアップパスプロパティ</summary>
		public string BackUpPath
		{
			get { return m_backUpPath; }
		}
		#endregion
	}
}
