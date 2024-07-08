/*
=========================================================================================================
  Module      : SqlAccesorラッパークラス(SqlAccesorWrapper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common;
using w2.Common.Sql;

namespace w2.ExternalAPI.Common.FrameWork.DB
{
	/// <summary>
	///	SqlAccesorラッパークラス
	/// </summary>
	/// <remarks>
	/// 接続文字列の制御を行い、SqlAccessorをラップ
	/// 外部連携処理はかならずこのラッパークラスを利用してSqlAccessorを利用すること
	/// </remarks>
	public class SqlAccesorWrapper : SqlAccessor
	{
		#region 接続文字列
		private static readonly string m_connStr = "";
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <remarks> 
		/// 静的コンストラクタでロードした接続文字列を利用して
		/// SqlAccesorのコンストラクタを呼び出す
		/// </remarks>
		public SqlAccesorWrapper()
			: base(m_connStr)
		{

		}
		#endregion

		#region 静的コンストラクタ
		/// <summary>
		/// 静的コンストラクタ
		/// </summary>
		/// <remarks>
		/// 接続文字列をAppAll.Configから読み込んで保持しておく
		/// </remarks>
		static SqlAccesorWrapper()
		{
			m_connStr = Constants.STRING_SQL_CONNECTION;
		}
		#endregion
	}
}
