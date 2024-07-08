/*
=========================================================================================================
  Module      : 外部システム連携用データ設定基底クラス(DataExporterBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Resources;
using System.Text;
using System.Web;
using System.Xml;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.App.Common.DataExporters
{
	///*********************************************************************************************
	/// <summary>
	/// 外部システム連携用データ設定基底クラス（Dispose必須）
	/// </summary>
	///*********************************************************************************************
	public abstract class DataExporterBase : IDisposable
	{
		protected bool isBeforeRead = false;
		protected SqlStatement sqlStatement = null;
		protected SqlStatementDataReader ssdrDataReader = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strFileName">ファイル名</param>
		/// <param name="strSeparateCharacter">データ区切り文字</param>
		/// <param name="strBundleCharacter">データ括り文字</param>
		/// <param name="eTextEncoding">文字エンコード</param>
		/// <param name="eExportKbn">データ出力区分</param>
		/// <param name="strStatementPath">SQLステートメントパス</param>
		/// <param name="strSQLStatementResourceValue">SQLステートメントリソース値</param>
		/// <param name="strAnchorText">ダウンロードアンカーテキスト</param>
		public DataExporterBase(
				SqlAccessor accessor,
				string strFileName,
				string strSeparateCharacter,
				string strBundleCharacter,
				Encoding eTextEncoding,
				Constants.ExportKbn eExportKbn,
				string strStatementPath,
				string strSQLStatementResourceValue,
				string strAnchorText)
		{
			//------------------------------------------------------
			// 外部システム連携用データ設定
			//------------------------------------------------------
			this.SqlAccessor = accessor;
			this.FileName = strFileName;
			this.SeparateCharacter = strSeparateCharacter;
			this.BundleCharacter = strBundleCharacter;
			this.TextEncoding = eTextEncoding;
			this.ExportKbn = eExportKbn;
			this.SQLStatementPath = strStatementPath;
			this.SQLStatementResourceValue = strSQLStatementResourceValue;
			this.DownloadAnchorText = strAnchorText;
		}

		/// <summary>
		/// 基幹システム連携用単位データ設定
		/// </summary>
		/// <param name="ssdrDataReader">SQLデータリーダー</param>
		/// <returns>単位出力データ</returns>
		public abstract string GetExportUnitData(SqlStatementDataReader ssdrDataReader);

		/// <summary>
		/// 出力データのエスケープ
		/// </summary>
		/// <param name="strData">エスケープ対象出力データ</param>
		/// <returns>エスケープ後出力データ</returns>
		protected string EscapeBundleString(string strData)
		{
			if ((strData != null)
				&& (this.BundleCharacter == "\""))
			{
				// データ括り文字がダブルクオテーションの場合、エスケープを行う
				return strData.Replace("\"", "\"\"");
			}

			return strData;
		}

		/// <summary>
		/// 行出力データ作成
		/// </summary>
		/// <param name="lRowDataList">行出力データリスト</param>
		/// <returns>行出力データ</returns>
		protected string CreateRowData(List<string> lRowDataList)
		{
			StringBuilder sbRowData = new StringBuilder();
			int indexData = 0;

			foreach (string strData in lRowDataList)
			{
				if (indexData > 0)
				{
					sbRowData.Append(this.SeparateCharacter);
				}
				sbRowData.Append(this.BundleCharacter);
				sbRowData.Append(EscapeBundleString(strData));
				sbRowData.Append(this.BundleCharacter);

				indexData++;
			}
			sbRowData.Append("\r\n");

			return sbRowData.ToString();
		}

		/// <summary>
		/// １行読み込み
		/// </summary>
		/// <param name="strLine">読み込み行</param>
		/// <returns>読み込み結果</returns>
		public virtual bool Read(out string strLine)
		{
			//------------------------------------------------------
			// 初回読み込み時、データを取得
			//------------------------------------------------------
			if (isBeforeRead == false)
			{
				// XMLリソースを読み込み
				XmlDocument xdSQLStatement = new XmlDocument();
				xdSQLStatement.LoadXml(this.SQLStatementResourceValue);

				// データ取得クエリを実行
				sqlStatement = new SqlStatement(xdSQLStatement.SelectSingleNode(this.SQLStatementPath).InnerText);
				ssdrDataReader = new SqlStatementDataReader(this.SqlAccessor, sqlStatement, false);

				isBeforeRead = true;
			}

			//------------------------------------------------------
			// 取得データを読み込み、出力データを作成
			//------------------------------------------------------
			if (ssdrDataReader.Read())
			{
				strLine = this.GetExportUnitData(ssdrDataReader);
				return true;
			}

			//------------------------------------------------------
			// 読み込み失敗の場合、falseを返却
			//------------------------------------------------------
			strLine = null;
			return false;
		}

		/// <summary>
		/// Process for data exporter
		/// </summary>
		/// <param name="response">Response</param>
		public virtual void Process(HttpResponse response)
		{
			//------------------------------------------------------
			// Write data output
			//------------------------------------------------------
			// Content encoding
			response.ContentEncoding = this.TextEncoding;
			// Setting file name
			response.AppendHeader("Content-Disposition", "attachment; filename=" + this.FileName);

			//------------------------------------------------------
			// Writting output
			//------------------------------------------------------
			string tempLine;
			// Writting header
			tempLine = "運送依頼番号,運送送り状番号,集荷希望日,荷受人住所ｺｰﾄﾞ,荷受人ｺｰﾄﾞ,荷受人名(漢字）,荷受人電話番号,荷受人郵便番号,荷受人住所(漢字）１,荷受人住所(漢字）２,荷受人住所（漢字）３,荷受人部門名１,荷受人部門名２,荷送人ｺｰﾄﾞ,荷送人名(漢字）,荷送人電話番号,荷送人郵便番号,荷送人住所（漢字）１,荷送人住所（漢字）2,荷送人住所（漢字）3,荷送人部門名１,荷送人部門名2,運賃請求先コード1,運賃請求先コード2,荷物取扱条件１,荷物取扱条件２,配達付帯作業(漢字）,品代金,保管温度区分,運送品標記用品名ｺｰﾄﾞ１,商品名１(漢字）,運送品標記用品名ｺｰﾄﾞ２,商品名２(漢字）,記述式運送梱包寸法,着荷指定日,着荷指定時刻条件,個数(依頼）,出荷区分,データ作成日,FILLER,受注番号,受注枝番（行№）,ギフトフラグ,店舗コード,のしフラグ,商品合計(税込),ブラック顧客表記名（漢字）,色（漢字）,サイズ（漢字）,送料合計,担当者コード,レシート№,受注or返品フラグ,受注日or返品日,決済手数料,商品コード,カテゴリコード,アイテムコード,商品型番,商品名,税込単価,税抜単価,数量,値引き額,税込金額,税抜金額,消費税,合計金額,備考（漢字）,IMAGEフラグ,支払い方法,使用ポイント数,配送メモ（漢字）,時間\r\n";

			response.Write(tempLine);

			while (this.Read(out tempLine))
			{
				response.Write(tempLine);
			}

			response.End();
		}

		/// <summary>
		/// 解放処理
		/// </summary>
		public virtual void Dispose()
		{
			if (ssdrDataReader != null)
			{
				ssdrDataReader.Dispose();
			}

			if (sqlStatement != null)
			{
				sqlStatement.Dispose();
			}
		}

		/// <summary>SqlAccessor</summary>
		public SqlAccessor SqlAccessor { get; private set; }
		/// <summary>ファイル名</summary>
		public string FileName { get; private set; }
		/// <summary>データ区切り文字</summary>
		public string SeparateCharacter { get; private set; }
		/// <summary>データ括り文字</summary>
		public string BundleCharacter { get; private set; }
		/// <summary>文字エンコード</summary>
		public Encoding TextEncoding { get; private set; }
		/// <summary>データ出力区分</summary>
		public Constants.ExportKbn ExportKbn { get; private set; }
		/// <summary>SQLステートメントパス</summary>
		public string SQLStatementPath { get; private set; }
		/// <summary>SQLステートメントリソース値</summary>
		public string SQLStatementResourceValue { get; private set; }
		/// <summary>ダウンロードアンカーテキスト</summary>
		public string DownloadAnchorText { get; private set; }
	}
}
