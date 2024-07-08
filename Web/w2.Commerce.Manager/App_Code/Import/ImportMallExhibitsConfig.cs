/*
=========================================================================================================
  Module      : モール出品設定ファイル取込モジュール(ImportMallExhibitsConfig.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// ImportMallExhibitsConfig の概要の説明です
/// </summary>
public class ImportMallExhibitsConfig : IImportExhibits
{
	// プロパティ
	private string m_strErrorMessage = null;
	private int m_iUpdatedCount = 0;
	private int m_iLinesCount = 0; // 全行数をカウント
	private int m_iColumnsCount = 2; // 1行の項目数を設定
	private const int CONST_MALLEXHIBITSCONFIG_COUNT = 20;	// 最大モール数
	private const string CONST_MALLEXHIBITSCONFIG_EXHIBITS_FLG = "exhibits_flg";	// 出品フラグ

	/// <summary>
	/// インポート
	/// </summary>
	/// <remarks>
	/// モール出品設定情報を取り込み
	/// </remarks>
	public bool ImportExhibits(StreamReader sr, string strShopId, string strOpertorName)
	{
		//------------------------------------------------------
		// フィールド数計算
		//------------------------------------------------------
		DataView dvMallCooperationSettings = MallPage.GetMallCooperationSettingAll(strShopId);
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLCOOPERATIONSETTING, Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG))
		{
			foreach (DataRowView drvMallCooperationSetting in dvMallCooperationSettings)
			{
				if ((string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG] == li.Value)
				{
					m_iColumnsCount++;
					break;
				}
			}
		}

			string strLineBuffer = null;
			//------------------------------------------------------
			// 更新処理
			//------------------------------------------------------
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				try
				{
					// 「コミット済みデータ読み取り可能」でトランザクション開始
					sqlAccessor.BeginTransaction(IsolationLevel.ReadCommitted);

					// 全削除
					using (SqlStatement sqlStatement = new SqlStatement("MallExhibitsConfig", "DeleteMallExhibitsConfigAll"))
					{
						int iDeleted = sqlStatement.ExecStatement(sqlAccessor);
					}

					using (SqlStatement sqlStatement = new SqlStatement("MallExhibitsConfig", "InsertMallExhibitsConfig"))
					{
						// 各行を読み取る
						string[] stringHeaderBuffer = null;
						int iCurrentLine = 0;
						while (sr.Peek() != -1)
						{
							// 処理中行カウンタ＋１
							iCurrentLine++;

							// フィールドに改行が含まれる場合を考慮して行を結合（CSVの行に改行がある場合、「"」は奇数個のはず）
							strLineBuffer = sr.ReadLine();
							while (((strLineBuffer.Length - strLineBuffer.Replace("\"", "").Length) % 2) != 0)
							{
								strLineBuffer += "\r\n" + sr.ReadLine();
							}

							// １行をCSV分割・フィールド数が正しい項目数(m_iColumnsCount)と合っているかチェック
							string[] stringBuffer = StringUtility.SplitCsvLine(strLineBuffer);
							if (m_iColumnsCount != stringBuffer.Length)
							{
								m_strErrorMessage = iCurrentLine.ToString() + "行目のフィールド数が定義と一致しませんでした。";
								m_strErrorMessage += "フィールド定義数は" + m_iColumnsCount.ToString() + "ですがデータのフィールド数は" + stringBuffer.Length.ToString() + "でした。";
								return false;
							}
							// ヘッダ行を飛ばす
							if (iCurrentLine == 1)
							{
								stringHeaderBuffer = StringUtility.SplitCsvLine(strLineBuffer);
								continue;
							}

							//------------------------------------------------------
							// SQL実行
							//------------------------------------------------------
							Hashtable htInput = new Hashtable();

							for (int iLoop = 0; iLoop < stringHeaderBuffer.Length; iLoop++)
							{
								htInput.Add(stringHeaderBuffer[iLoop], stringBuffer[iLoop]);
							}
							for (int iLoop = 1; iLoop <= CONST_MALLEXHIBITSCONFIG_COUNT; iLoop++)
							{
								if (htInput.Contains(CONST_MALLEXHIBITSCONFIG_EXHIBITS_FLG + iLoop.ToString()) == false)
								{
									htInput.Add(CONST_MALLEXHIBITSCONFIG_EXHIBITS_FLG + iLoop.ToString(), Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON);
								}
							}
							htInput.Add(Constants.FIELD_MALLEXHIBITSCONFIG_LAST_CHANGED, strOpertorName);

							m_iUpdatedCount += sqlStatement.ExecStatement(sqlAccessor, htInput);
						}
						// 処理行数
						m_iLinesCount = iCurrentLine - 1;
					}

					//------------------------------------------------------
					// トランザクションコミット
					//------------------------------------------------------
					// ここまでエラーがなければはじめてコミット
					sqlAccessor.CommitTransaction();
				}
				catch (Exception ex)
				{
					sqlAccessor.RollbackTransaction();
					throw ex;
				}
			}

		return true;
	}

	/// プロパティ
	/// <summary処理結果メッセージ</summary>
	public string ErrorMessage
	{
		get { return StringUtility.ToEmpty(m_strErrorMessage); }
	}

	/// <summary>更新件数</summary>
	public int UpdatedCount
	{
		get { return m_iUpdatedCount; }
	}

	/// <summary>処理行数</summary>
	public int LinesCount
	{
		get { return m_iLinesCount; }
	}
}
