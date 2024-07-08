/*
=========================================================================================================
  Module      : 外部システム連携用データ作成共通処理クラス(DataExporterCreater.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;
using w2.Common.Logger;

namespace w2.App.Common.DataExporters
{
	///*********************************************************************************************
	/// <summary>
	/// 外部システム連携用データ作成共通処理クラス
	/// </summary>
	///*********************************************************************************************
	public partial class DataExporterCreater
	{
		/// <summary>
		/// 基幹システム連携用データ取得
		/// </summary>
		/// <param name="strProjectNo">プロジェクトNo</param>
		/// <param name="iFileIndex">ファイルインデクス</param>
		/// <returns>連携用データ設定クラス</returns>
		public static DataExporterBase CreateDataExporter(string strProjectNo, int iFileIndex, SqlAccessor sqlAccessor)
		{
			// データ設定クラスのTypeを取得
			Type tDataExporterType = Type.GetType(typeof(DataExporterBase).Namespace + ".DataExporter_" + strProjectNo + "_" + iFileIndex.ToString());

			// データ設定クラスのインスタンスを作成して返す
			return (DataExporterBase)Activator.CreateInstance(tDataExporterType, new object[] { sqlAccessor }); ;
		}

		/// <summary>
		/// 管理画面表示用ダウンロードアンカーテキスト取得
		/// </summary>
		/// <param name="strProjectNo">プロジェクトN0</param>
		/// <param name="eExportKbn">呼び出し元データ出力区分</param>
		/// <returns>アンカーテキストリスト</returns>
		public static Dictionary<int, string> GetDownloadAnchorTextForCommerceManager(string strProjectNo, Constants.ExportKbn eExportKbn)
		{
			Dictionary<int, string> dDownloadAnchorTextList = new Dictionary<int, string>();
			int iClassIndex = 0;

			// 該当プロジェクトNoのクラス（設定）をインデクスで検索し、アンカーテキストを取得する
			while(true)
			{
				// データ設定クラスのTypeを取得
				Type tNewDataExporterType = Type.GetType(typeof(DataExporterBase).Namespace + ".DataExporter_" + strProjectNo + "_" + iClassIndex.ToString());

				// クラスが存在しなければ、ループを抜ける
				if (tNewDataExporterType == null)
				{
					break;
				}

				// データ設定クラスのインスタンスを作成
				DataExporterBase dataExporterBase = (DataExporterBase)Activator.CreateInstance(tNewDataExporterType, new object[] {null });

				// データ出力区分が、「全て」または呼び出し元の画面と等しければアンカーテキストを取得
				if ((dataExporterBase.ExportKbn == Constants.ExportKbn.ALL)
					|| (dataExporterBase.ExportKbn == eExportKbn))
				{
					// アンカーテキストを取得
					dDownloadAnchorTextList.Add(iClassIndex, dataExporterBase.DownloadAnchorText);
				}
				
				iClassIndex++;
			}

			return dDownloadAnchorTextList;
		}
	}
}
	
