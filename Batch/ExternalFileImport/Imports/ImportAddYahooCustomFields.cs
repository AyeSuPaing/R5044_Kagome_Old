/*
=========================================================================================================
  Module      : ヤフーカスタムフィールドデータ取込処理(ImportAddYahooCustomFields.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Common.Sql;
using w2.Domain.UpdateHistory;

namespace w2.Commerce.Batch.ExternalFileImport.Imports
{
	class ImportAddYahooCustomFields : ImportBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		public ImportAddYahooCustomFields(string strShopId)
			: base(strShopId, "AddYahooCustomFields")
		{
		}

		/// <summary>
		/// ファイル取込
		/// </summary>
		/// <param name="strFilePath">取込ファイルパス</param>
		/// <returns>取込件数</returns>
		public override int Import(string strActiveFilePath)
		{
			int iImportCount = 0;
			DateTime dtNow = DateTime.Now;

			using (FileStream fileStream = new FileStream(strActiveFilePath, FileMode.Open))
			using (StreamReader streamReader = new StreamReader(fileStream, Encoding.GetEncoding(Constants.CONST_ENCODING_DEFAULT)))
			{
				ArrayList alHeaders = new ArrayList(StringUtility.SplitCsvLine(streamReader.ReadLine()));

				//------------------------------------------------------
				// トランザクション
				//------------------------------------------------------
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				{
					sqlAccessor.OpenConnection();
					sqlAccessor.BeginTransaction();

					try
					{
						Dictionary<string, string> dicOrderCustomField = new Dictionary<string, string>(); 
						string strOrderId = null;

						//------------------------------------------------------
						// カスタムフィールド集計処理
						//------------------------------------------------------
						while (streamReader.EndOfStream == false)
						{
							// フィールドに改行が含まれる場合を考慮して行を結合（CSVの行に改行がある場合、「"」は奇数個のはず）
							string strLineBuffer = streamReader.ReadLine();
							while ((((strLineBuffer.Length - strLineBuffer.Replace("\"", "").Length) % 2) != 0) && (streamReader.EndOfStream == false))
							{
								strLineBuffer += "\r\n" + streamReader.ReadLine();
							}

							// １行をCSV分割・フィールド数がヘッダの数と合っているかチェック
							string[] strDatas = StringUtility.SplitCsvLine(strLineBuffer);
							if (alHeaders.Count != strDatas.Length)
							{
								throw new ApplicationException("ヘッダのフィールド数とフィールド数が一致しません(" + (iImportCount + 1).ToString() + "行目)");
							}

							// キーブレイク判定
							string strOrderIdTemp = strDatas[alHeaders.IndexOf("OrderId")];
							if (strOrderId != strOrderIdTemp)
							{
								strOrderId = strOrderIdTemp;

								//------------------------------------------------------
								// 注文情報存在チェック＆ユーザメモ取得
								//------------------------------------------------------
								DataView dvOrder = null;
								using (SqlStatement sqlStatement = new SqlStatement(m_strFileType, "CheckOrderId"))
								{
									Hashtable htInput = new Hashtable();
									htInput.Add(Constants.FIELD_ORDER_SHOP_ID, m_strShopId);
									htInput.Add(Constants.FIELD_ORDER_ORDER_ID, strOrderId);

									dvOrder = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
								}

								if (dvOrder.Count == 0)
								{
									FileLogger.WriteError("注文IDが存在しません。この注文情報はスキップされます。(order_id:" + strOrderId + ")");
									continue;
								}

								// ユーザメモを新規に配列格納
								if (dicOrderCustomField.ContainsKey(strOrderId) == false)
								{
									dicOrderCustomField.Add(strOrderId, (string)dvOrder[0][Constants.FIELD_ORDER_MEMO] + "\r\n■カスタムフィールド■\r\n");
								}
							}

							//------------------------------------------------------
							// カスタムフィールド格納
							//------------------------------------------------------
							if (dicOrderCustomField.ContainsKey(strOrderId))
							{
								// 注文ID以外はすべて取り込む
								for (int i = 0; i < alHeaders.Count; i++)
								{
									if ((string)alHeaders[i] == "OrderId") continue;

									StringBuilder customField = new StringBuilder();
									customField.Append("－－").Append(alHeaders[i]).Append("－－").Append("\r\n");
									customField.Append(strDatas[i]).Append("\r\n");

									// ユーザメモの後にカスタムフィールドを追加する
									dicOrderCustomField[strOrderId] += customField.ToString();
								}
							}

							iImportCount++;
						}

						//------------------------------------------------------
						// ユーザメモ更新処理（カスタムフィールド追加済み）
						//------------------------------------------------------
						foreach (KeyValuePair<string, string> kvpOrder in dicOrderCustomField)
						{
							Hashtable htOrder = new Hashtable();
							htOrder.Add(Constants.FIELD_ORDER_ORDER_ID, kvpOrder.Key);
							htOrder.Add(Constants.FIELD_ORDER_MEMO, kvpOrder.Value);

							using (SqlStatement sqlStatement = new SqlStatement(m_strFileType, "AddOrderMemo"))
							{
								sqlStatement.ExecStatement(sqlAccessor, htOrder);
							}

							// 更新履歴登録
							new UpdateHistoryService().InsertForOrder(kvpOrder.Key, Constants.FLG_LASTCHANGED_BATCH, sqlAccessor);
						}

						// トランザクションコミット
						sqlAccessor.CommitTransaction();
					}
					catch
					{
						// トランザクションロールバック
						sqlAccessor.RollbackTransaction();
						throw;
					}
				}
			}

			return iImportCount;
		}
	}
}
