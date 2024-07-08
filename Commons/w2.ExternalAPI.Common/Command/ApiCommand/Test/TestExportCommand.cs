using System.Data;

using w2.ExternalAPI.Common.FrameWork.DB;

namespace w2.ExternalAPI.Common.Command.ApiCommand.Test
{
	///**************************************************************************************
	/// <summary>
	/// エクスポートAPIコマンドクラス
	/// </summary>
	///**************************************************************************************
	public class TestExportCommand : ApiCommandBase
	{
		#region コンストラクタ
		/// <summary>コンストラクタ</summary>
		public TestExportCommand()
		{
			Init();
		}
		#endregion

		#region #Init 初期化処理
		/// <summary>初期化処理</summary>
		protected override void Init()
		{
			
		}
		#endregion

		#region #Execute コマンド実行処理
		/// <summary>
		/// コマンド実行処理
		/// </summary>
		/// <param name="apiCommandArg"></param>
		/// <returns></returns>
		protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg)
		{
			DataTable dt;
			// ※テスト用にデータ取得
			using (SqlAccesorWrapper sqlAccessorwWrapper = new SqlAccesorWrapper())
			using (SqlStatementWrapper sqlStatementWrapper = new SqlStatementWrapper("ExternalApi", "TestSql"))
			{
				dt = sqlStatementWrapper.SelectSingleStatement(sqlAccessorwWrapper).Table;
			}

			TestExportApiCommandResult testExportApiCommandResult = new TestExportApiCommandResult(EnumResultStatus.Complete,  dt);

			return testExportApiCommandResult;
		}
		#endregion

		#region #End 終了処理
		/// <summary>
		/// 終了処理
		/// </summary>
		protected override void End()
		{
			
		}
		#endregion
	}
}
