/*
=========================================================================================================
  Module      : SqlStatementWrapperラッパークラス(SqlStatementWrapper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data;
using System.IO;
using w2.Common.Sql;
using System.Linq;
using System.Xml.Linq;

namespace w2.ExternalAPI.Common.FrameWork.DB
{

	/// <summary>
	///	SqlStatementWrapperラッパークラス
	/// </summary>
	/// <remarks>
	/// SQL分定義・SQLパラメタの制御を行い、SqlStatementWrapperをラップ
	/// 外部連携処理はかならずこのラッパークラスを利用してSqlStatementWrapperを利用すること
	/// </remarks>
	public class SqlStatementWrapper : SqlStatement
	{
		#region メンバ変数
		private static readonly string m_sqlStatementXmlResouce;
		#endregion

		#region 静的コンストラクタ
		/// <summary>
		/// 静的コンストラクタ
		/// </summary>
		/// <remarks> 
		/// リソースファイルからXML形式のSQL定義情報を読み込み、保持しておく
		/// </remarks>
		static SqlStatementWrapper()
		{
			m_sqlStatementXmlResouce = Properties.Resources.ExternalApi;
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <remarks> 
		/// 静的コンストラクタで保持しているSQL定義情報を利用して
		/// SqlStatementのコンストラクタを呼び出す
		/// </remarks>
		public SqlStatementWrapper(string strPageName, string strStatementName)
			: base(GetStatementSql(strPageName, strStatementName))
		{
			AddParameters(strPageName, strStatementName,base.AddInputParameters);
		}
		#endregion

		#region -AddParameters SQLパラメタ追加
		/// <summary>
		/// SQLパラメタ追加
		/// </summary>
		///<param name="strPageName">対象ステートメント定義ページ名（XMLの一階層目）</param>
		///<param name="strStatementName">対象ステートメント定義名（XMLの二階層目）</param>
		///<param name="action">
		/// パラメタ追加処理
		/// base.AddInputParametersを指定しとけば基本的に問題なし
		/// </param>
		private void AddParameters(string strPageName, string strStatementName
			, Action<string, SqlDbType, string> action)
		{
			
			// パラメータXML読み込み
			var xmlDoc = XDocument.Load(new StringReader(m_sqlStatementXmlResouce));

			var result = (
				from parameters in
					XDocument.Load(
					new StringReader(m_sqlStatementXmlResouce))
					.Elements(strPageName)
					.Elements(strStatementName)
					.Elements("Parameter")
					.Elements("Input")
				select parameters
				);

			foreach (var para in result)
			{
				var xAttributeName = para.Attribute("Name");
				var xAttributeType = para.Attribute("Type");
				var xAttributeSize = para.Attribute("Size");

				if(xAttributeName != null && xAttributeType != null)
				{
					action(xAttributeName.Value,
						GetSqlDbType(xAttributeType.Value),
						(xAttributeSize != null) ? xAttributeSize.Value:null);
				}

			}
		}
		#endregion

		#region +GetSqlDbType DbType取得
		/// <summary>
		/// DbType取得
		/// </summary>
		/// <param name="strDbType">DbTypeを表す文字列</param>
		public static SqlDbType GetSqlDbType(string strDbType)
		{
			switch (strDbType.ToUpper())
			{
				case "BIGINT":
					return SqlDbType.BigInt;
				case "BINARY":
					return SqlDbType.Binary;
				case "BIT":
					return SqlDbType.Bit;
				case "CHAR":
					return SqlDbType.Char;
				case "DATETIME":
					return SqlDbType.DateTime;
				case "DECIMAL":
					return SqlDbType.Decimal;
				case "FLOAT":
					return SqlDbType.Float;
				case "IMAGE":
					return SqlDbType.Image;
				case "INT":
					return SqlDbType.Int;
				case "MONEY":
					return SqlDbType.Money;
				case "NCHAR":
					return SqlDbType.NChar;
				case "NTEXT":
					return SqlDbType.NText;
				case "NVARCHAR":
					return SqlDbType.NVarChar;
				case "REAL":
					return SqlDbType.Real;
				case "SMALLDATETIME":
					return SqlDbType.SmallDateTime;
				case "SMALLINT":
					return SqlDbType.SmallInt;
				case "SMALLMONEY":
					return SqlDbType.SmallMoney;
				case "TEXT":
					return SqlDbType.Text;
				case "TIMESTAMP":
					return SqlDbType.Timestamp;
				case "TINYINT":
					return SqlDbType.TinyInt;
				case "UNIQUEIDENTIFIER":
					return SqlDbType.UniqueIdentifier;
				case "VARBINARY":
					return SqlDbType.VarBinary;
				case "VARCHAR":
					return SqlDbType.VarChar;
				case "VARIANT":
					return SqlDbType.Variant;
				default:
					throw new Exception("指定されたDbTypeが取得できません。");
			}
		}
		#endregion

		#region +GetStatementSql SQLステートメント取得
		/// <summary>
		/// SQLステートメント取得
		/// </summary>
		///<param name="strPageName">対象ステートメント定義ページ名（XMLの一階層目）</param>
		///<param name="strStatementName">対象ステートメント定義名（XMLの二階層目）</param>
		public static string GetStatementSql(string strPageName, string strStatementName)
		{
			string retunval = "";

			var xmlDoc = XDocument.Load(new StringReader(m_sqlStatementXmlResouce));

			var result = (
			             	from statement
			             		in XDocument.Load(
			             			new StringReader(m_sqlStatementXmlResouce))
			             		.Elements(strPageName)
			             		.Elements(strStatementName)
			             		.Elements("Statement")
			             	select statement.Value);

			foreach(string statement in result)
			{
				// ステートメント取得
				retunval = statement;
			}

			return retunval;

		}
		#endregion

	}

	
}
