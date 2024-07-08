/*
=========================================================================================================
  Module      : 外部連携eAPI規定クラス(ApiBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.ExternalAPI.Common
{
	/// <summary>
	/// 外部連携API規定クラス
	/// </summary>
	public class ApiBase : IDisposable
	{
		#region メンバ変数
		protected readonly ExecuteTarget m_executeTarget;
		#endregion

		#region コンストラクタ
		/// <summary>コンストラクタ</summary>
		/// <param name="executeTarget">ターゲット情報</param>
		protected ApiBase(ExecuteTarget executeTarget)
		{
			m_executeTarget = executeTarget;
			
			Init();
		}
		#endregion

		#region #Init 初期化処理
		/// <summary>
		/// 初期化処理
		/// </summary>
		protected virtual void Init() { }
		#endregion

		#region #PreExecute 実行前処理
		/// <summary>
		/// 実行前処理
		/// </summary>
		protected virtual void PreExecute(){}
		#endregion

		#region #Execute インポート実行
		/// <summary>
		/// インポート実行
		/// </summary>
		protected virtual bool Execute() { return true; }
		#endregion

		#region #PostExecute 実行後処理
		/// <summary>
		/// 実行後処理
		/// </summary>
		/// <param name="resultExecute">Execute実行結果（成功：true、失敗：false）</param>
		protected virtual void PostExecute(bool resultExecute){}
		#endregion 

		#region #終了処理
		/// <summary>
		/// 終了処理
		/// </summary>
		protected virtual void End() { }
		#endregion

		#region 実行する
		/// <summary>実行する</summary>
		/// <remarks>PreExecute→Execute→PostExecuteの順で処理を実行する</remarks>
		public void Do()
		{
			PreExecute();
			bool resultExecute = Execute();
			PostExecute(resultExecute);
		}
		#endregion

		#region +Dispose
		/// <summary>
		/// Disposeの実装
		/// </summary>
		public void Dispose()
		{
			End();
		}
		#endregion
	}
}
