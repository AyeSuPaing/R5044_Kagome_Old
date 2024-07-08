/*
=========================================================================================================
  Module      : データベースアクセス用ユーティリティクラス(LinkUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : データベースアクセス処理用の共通処理を司るクラス。
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;
using w2.Plugin;

namespace w2.Plugin.P0011_Intercom.Util
{
	/// <summary>
	/// データベースアクセス用ユーティリティクラス
	/// </summary>
	internal class DBUtil
	{
		//接続文字列
		private string m_connStr = null;

		//SQL定義xml
		private XElement m_sqlXml = null;
			
		/// <summary>
		/// Privateコンストラクタ
		/// </summary>
		internal DBUtil(string connStr,string sqlXmlPath)
		{
			//接続文字列セット
			m_connStr = connStr;

			//sql用xmlセット
			m_sqlXml = XElement.Load(sqlXmlPath);
		}

		/// <summary>
		/// SQL実行(Insert、Update、Delete)
		/// </summary>
		/// <param name="secID">SQL定義XMLのセクションID</param>
		/// <param name="dt"></param>
		public void ExecuteSql(string secID, IPluginHost host,DataTable dt)
		{
			//セクションIDから対象のSQLを取得
			string sqlStr = GetSqlStatement(secID);

			//sqlをコマンドに割り当て
			using (SqlCommand cmd = GetSqlCmd(sqlStr))
			{
				
				//パラメタ情報をセット
				SetSqlParams(cmd, host, secID, dt);

				cmd.Connection.Open();

				//コマンド実行
				cmd.ExecuteNonQuery();

				cmd.Connection.Close();
			}
		}

		/// <summary>
		/// データテーブル取得
		/// </summary>
		/// <param name="secID">SQL定義XMLのセクションID</param>
		/// <param name="host">引数データを持つIPluginHost</param>
		/// <returns>引数を基にSQLを実行し、実行結果をDataTableで返却</returns>
		public DataTable GetDataTable(string secID, IPluginHost host,DataTable dt)
		{
			//セクションIDから対象のSQLを取得
			string sqlStr = GetSqlStatement(secID);
			//sqlをコマンドに割り当て
			SqlCommand cmd = GetSqlCmd(sqlStr);
			//パラメタ情報をセット
			SetSqlParams(cmd, host, secID,dt);
			//コマンド実行
			return GetDataTable(cmd);
		}

		/// <summary>
		/// SQLパラメタセット
		/// </summary>
		/// <param name="cmd">パラメタセット対象のSQLCommand</param>
		/// <param name="host">パラメタ値を持つプラグインhost</param>
		/// <param name="secID">SQL定義XMLのセクションID</param>
		public void SetSqlParams(SqlCommand cmd, IPluginHost host, string secID, DataTable dt)
		{

			//LINQでXMLからSQLパラメタ情報
			var query = from n in m_sqlXml.Elements("section")
						where n.Attribute("id").Value.Equals(secID)
						select n.Element("sql").Element("params");
						
			//取得したパラメタ分
			foreach (var n in query)
			{
				foreach (var xx in n.Elements("param"))
				{

					//設定元切り分け
					switch (xx.Attribute("pluginargtarget").Value)
					{
						case "hash" :
							//ハッシュテーブルが元の場合
							cmd.Parameters.AddWithValue(xx.Attribute("name").Value, host.Data[xx.Attribute("pluginargname").Value]);
							break;
						case "coockie":
							//クッキーが元の場合
							cmd.Parameters.AddWithValue(xx.Attribute("name").Value, host.Context.Request.Cookies[xx.Attribute("pluginargname").Value].Value);
							break;
						case "datatable":
							//データテーブルの場合
							cmd.Parameters.AddWithValue(xx.Attribute("name").Value, dt.Rows[0][xx.Attribute("pluginargname").Value]);
							break;

						default:
							//TODO: エラーいれる？							
							break;
					}
				}
			}
		}
				
		/// <summary>
		/// データテーブル取得
		/// </summary>
		/// <param name="cmd">SQL実行元となるSQLCommand</param>
		/// <returns>SQLの実行結果をDataTableで返却</returns>
		public DataTable GetDataTable(SqlCommand cmd)
		{
			DataTable dt = new DataTable();

			using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
			{
				adp.Fill(dt);
			}

			GetSqlStatement("tes");

			return dt;
		}

		/// <summary>
		/// SQLCommand取得(接続文字列付)
		/// </summary>
		/// <param name="sql">SQL文</param>
		/// <returns>SQLCommandを返却</returns>
		public SqlCommand GetSqlCmd(string sql)
		{
			SqlCommand cmd = new SqlCommand();
			cmd.Connection = new SqlConnection(m_connStr);
			cmd.CommandText = sql;

			return cmd;
		}

		/// <summary>
		/// SQL定義XMLからSQL文を取得
		/// </summary>
		/// <param name="secID">対象となるセクションのID</param>
		/// <returns>取得したSQL文</returns>
		public string GetSqlStatement(string secID)
		{
			string sqlState = "";

			//LINQでXMLからSQL情報

			var query = from n in m_sqlXml.Elements("section")
						where n.Attribute("id").Value.Equals(secID)
						select n.Element("sql").Element("state");

			foreach (var n in query)
			{
				sqlState = n.Value;
			}

			return sqlState;

		}
	}
}
