
using System.Data;

namespace w2.ExternalAPI.Common.Command.ApiCommand.Test
{
	///**************************************************************************************
	/// <summary>
	///	エクスポートAPIコマンド実行結果クラス
	/// </summary>
	/// <remarks>
	/// エクスポートAPIコマンドの実行結果を持つクラス
	/// </remarks>
	///**************************************************************************************
	public class TestExportApiCommandResult : ApiCommandResult
	{
		#region メンバ変数
		private DataTable m_DataTable;
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="enumResultStatus"></param>
		/// <param name="rowResult"></param>
		public TestExportApiCommandResult(EnumResultStatus enumResultStatus,
			DataTable DataTable)
			: base(enumResultStatus)
		{
			m_DataTable = DataTable;
		}
		#endregion

		#region プロパティ
		public DataTable DataTable
		{
			get { return m_DataTable; }
		}
		#endregion
	}
}
