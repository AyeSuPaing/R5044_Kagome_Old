/*
=========================================================================================================
  Module      : W3Cログ取込プロセスクラス(P01_ImportW3cLog.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Net.Mail;
using w2.App.Common.Util;
using System.Globalization;

namespace w2.MarketingPlanner.Batch.AccessLogImporter.Process
{
	class P01_ImportW3cLog : P00_BaseProcess
	{
		const string STR_LOGHEAD_FIELD_DEFINE = "#Fields: ";
		const string STR_FIELD_CS_URI_STEM = "cs-uri-stem";
		const string W3C_LOGFILE_HEADER_EX = "ex"; // IIS側のエンコード指定なし
		const string W3C_LOGFILE_HEADER_U_EX = "u_ex"; // IIS側のエンコード指定「UTF-8」
		Dictionary<string, int> m_dicColumnOrder = null;

		// メール送信情報用
		public List<string> NotImportFileNames { get; private set; }
		public List<string> SuccessFileNames { get; private set; }
		public List<string> FailedFileNames { get; private set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public P01_ImportW3cLog()
		{
			this.NotImportFileNames = new List<string>();
			this.SuccessFileNames = new List<string>();
			this.FailedFileNames = new List<string>();
		}

		/// <summary>
		/// 全ファイル取込処理
		/// </summary>
		/// <param name="blTargetPc">ＰＣ対象</param>
		public void ImportAll(bool blTargetPc)
		{
			string strProcessName = null;
			try
			{
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				{
					sqlAccessor.OpenConnection();

					//------------------------------------------------------
					// １．Importディレクトリ作成
					//------------------------------------------------------
					strProcessName = "１．Importディレクトリ作成";
					if (blTargetPc)
					{
						if (Directory.Exists(Constants.PATH_W3CLOG_IMPORT) == false)
						{
							Directory.CreateDirectory(Constants.PATH_W3CLOG_IMPORT);
						}
					}

					//------------------------------------------------------
					// ２．エラーディレクトリにあるログファイル移動
					//------------------------------------------------------
					strProcessName = "２．エラーディレクトリにあるログファイル移動";
					if (blTargetPc)
					{
						foreach (string strFilePath in Directory.GetFiles(Constants.PATH_W3CLOG_ERROR))
						{
							string strDstFilePath = Constants.PATH_W3CLOG_IMPORT + Path.GetFileName(strFilePath);
							if (File.Exists(strDstFilePath) == false)
							{
								File.Move(strFilePath, Constants.PATH_W3CLOG_IMPORT + Path.GetFileName(strFilePath));
							}
						}
					}

					//------------------------------------------------------
					// ３．W3Cログ格納ディレクトリよりImportディレクトリへ移動
					//------------------------------------------------------
					strProcessName = "３．W3Cログ格納ディレクトリよりImportディレクトリへ移動";
					if (blTargetPc)
					{
						int iW3cLogIndex = 1;
						foreach (string strW3cLogDirectory in Constants.W3CLOG_DIRECTORIES)
						{
							foreach (string strW3CFilePath in Directory.GetFiles(strW3cLogDirectory))
							{
								if (strW3CFilePath.EndsWith(W3C_LOGFILE_HEADER_EX + DateTime.Now.ToString("yyMMdd") + ".log") == false)
								{
									// ヘッダをつけて移動
									File.Move(strW3CFilePath, Constants.PATH_W3CLOG_IMPORT + Constants.IMPORTLOG_HEADER + iW3cLogIndex.ToString() + Path.GetFileName(strW3CFilePath));
								}
							}

							iW3cLogIndex++;
						}
					}

					//------------------------------------------------------
					// ４．Importディレクトリファイルを取り込む
					//------------------------------------------------------
					if (blTargetPc)
					{
						// 各取得元（サーバーのパス）に対してループ
						foreach (string strFilePath in Directory.GetFiles(Constants.PATH_W3CLOG_IMPORT))
						{
							//------------------------------------------------------
							// ４－１．各種情報設定
							//------------------------------------------------------
							strProcessName = "４－１．各種情報設定";
							// ファイル名・各ファイルパス決定
							string strFileName = Path.GetFileName(strFilePath);
							string strActiveFilePath = Constants.PATH_W3CLOG_ACTIVE + strFileName;
							string strCompleteFilePath = Constants.PATH_W3CLOG_COMPLETE + strFileName;
							string strErrorFilePath = Constants.PATH_W3CLOG_ERROR + strFileName;

							if ((strFileName.StartsWith(Constants.IMPORTLOG_HEADER) == false) || (strFilePath.EndsWith(".log") == false))
							{
								// 正しいログファイルではない
								this.NotImportFileNames.Add(strFileName);
								continue;
							}

							// ファイルカウント取得
							string strTmpCount = strFileName.Substring(Constants.IMPORTLOG_HEADER.Length);
							// ※IISのエンコード設定が「UTF-8」の場合、ログファイル名が「u_ex」に変更されている。
							strTmpCount = strTmpCount.Substring(0, strTmpCount.Contains(W3C_LOGFILE_HEADER_U_EX) ? strTmpCount.IndexOf(W3C_LOGFILE_HEADER_U_EX) : strTmpCount.IndexOf(W3C_LOGFILE_HEADER_EX));

							// 対象日付取得
							string strTargetDate = strFilePath.Replace(".log", "");
							strTargetDate = strTargetDate.Substring(strTargetDate.Length - 6);
							strTargetDate = "20" + strTargetDate.Substring(0, 2) + "/" + strTargetDate.Substring(2, 2) + "/" + strTargetDate.Substring(4, 2);

							DateTime dateTimeCheck;
							if (DateTime.TryParseExact(strTargetDate, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeCheck) == false)
							{
								// Not Import File When File Name Has Invalid Date Time Format
								this.NotImportFileNames.Add(strFileName);
								continue;
							}

							//------------------------------------------------------
							// ４－２．取込ステータス変更
							//------------------------------------------------------
							strProcessName = "４－２．取込ステータス変更";
							{
								DataView dvResult = null;
								using (SqlStatement sqlStatement = new SqlStatement("ImportW3cLog", "ChangeImportStatus"))
								{
									Hashtable htInput = new Hashtable();
									htInput.Add(Constants.FIELD_ACCESSLOGSTATUS_TARGET_DATE, strTargetDate);
									htInput.Add(Constants.FIELD_ACCESSLOGSTATUS_TARGET_FILENAME, strFileName);
									htInput.Add(Constants.FIELD_ACCESSLOGSTATUS_IMPORT_FILES_TOTAL, Constants.W3CLOG_DIRECTORIES.Count);
									htInput.Add(Constants.FIELD_ACCESSLOGSTATUS_IMPORT_FILES, strTmpCount);
									htInput.Add(Constants.FIELD_ACCESSLOGSTATUS_DAY_STATUS, Constants.FLG_ACCESSLOGPROCSTAT_DAY_STATUS_IMPORT_RUN);

									dvResult = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
								}

								if ((int)dvResult[0]["result"] != 0)
								{
									// 対象外ファイル格納（ステータスチェックＮＧ）
									this.NotImportFileNames.Add(strFileName);
									continue;
								}
							}

							//------------------------------------------------------
							// ４－３．ログファイル取込
							//------------------------------------------------------
							strProcessName = "４－３．ログファイル取込";
							try
							{
								// アクティブフォルダへファイル移動
								File.Move(strFilePath, strActiveFilePath);

								// アクティブファイル取込
								ImportActiveFile(strTargetDate, strActiveFilePath);

								// コンプリートディレクトリへファイル移動
								File.Move(strActiveFilePath, strCompleteFilePath);

								// ステータス変更（成功）
								using (SqlStatement sqlStatement = new SqlStatement("ImportW3cLog", "ChangeImportStatusSuccess"))
								{
									sqlStatement.ExecStatement(sqlAccessor);
								}

								// 成功ファイル名格納
								this.SuccessFileNames.Add(strFileName);
							}
							catch (Exception)
							{
								// 失敗ファイル名・エラーメッセージ格納
								this.FailedFileNames.Add(strFileName);

								// 失敗ファイルを移動
								File.Move(strActiveFilePath, strErrorFilePath);

								// エラーになったらスロー
								throw;
							}

						}	// foreach (string strFilePath in Directory.GetFiles(Constants.PATH_W3CLOG_IMPORT))
					}

					//------------------------------------------------------
					// ５．dept_id更新
					//------------------------------------------------------
					if (blTargetPc)
					{
						if (this.SuccessFileNames.Count != 0)
						{
							strProcessName = "５．dept_id更新";
							{
								using (SqlStatement sqlStatement = new SqlStatement("ImportW3cLog", "AssignDeptIdToAccessLog"))
								{
									sqlStatement.CommandTimeout = Constants.SQLTIMEOUT_SEC;
									sqlStatement.ExecStatement(sqlAccessor);
								}
							}
						}
					}

				}	// using (SqlAccessor sqlAccessor = new SqlAccessor())
			}
			catch (Exception ex)
			{
				this.ProcessException = new Exception(this.GetType().Name + " " + strProcessName + " でエラーが発生しました。\r\n", ex);

				this.ProcessResult = P00_BaseProcess.PROCESS_RESULT.FAILED;
				return;
			}

			//------------------------------------------------------
			// プロセスのステータス更新
			//------------------------------------------------------
			if (this.SuccessFileNames.Count != 0)
			{
				if (this.FailedFileNames.Count != 0)
				{
					this.ProcessResult = PROCESS_RESULT.FAILED_A_PART;
				}
				else
				{
					this.ProcessResult = PROCESS_RESULT.SUCCESS;
				}
			}
			else if ((this.FailedFileNames.Count != 0)
				|| (this.ProcessException != null))
			{
				this.ProcessResult = PROCESS_RESULT.FAILED;
			}
			else if (this.NotImportFileNames.Count != 0)
			{
				this.ProcessResult = PROCESS_RESULT.ALERT;
			}
			else
			{
				this.ProcessResult = PROCESS_RESULT.NONE;
			}
		}

		/// <summary>
		/// アクティブファイル取込処理
		/// </summary>
		/// <param name="strTargetDate">対象日付</param>
		/// <param name="strActiveFilePath">アクティブファイルパス</param>
		/// <remarks>
		/// メモリ使用量を節約するためトランザクションを利用しないようにする。
		/// 失敗時のロールバックは複数ファイル取り込みを考慮し、日付指定削除ではなくログNOでの削除を行う
		/// </remarks>
		private void ImportActiveFile(string strTargetDate, string strActiveFilePath)
		{
			string[] strSplitLine = null;	// エラー情報作成のため外に出しておく

			// IISのエンコード設定が「UTF-8」であれば「UTF-8」とし、それ以外は「Shift-JIS」とする。
			Encoding eEncode = Path.GetFileName(strActiveFilePath).Contains(W3C_LOGFILE_HEADER_U_EX) ? Encoding.UTF8 : Encoding.GetEncoding(932);

			// エンコードを指定してファイルオープン
			using (StreamReader streamReader = new StreamReader(strActiveFilePath, eEncode))
			{
				//------------------------------------------------------
				// インサート処理設定
				//------------------------------------------------------
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				{
					// コネクションオープン
					sqlAccessor.OpenConnection();

					long lLogNoMax = 0;
					try
					{
						//------------------------------------------------------
						// 最新ログ番号取得（ロールバック用）
						//------------------------------------------------------
						DataView dvLogNoMax;
						using (SqlStatement sqlStatement = new SqlStatement("ImportW3cLog", "GetLogNoMax"))
						{
							sqlStatement.CommandTimeout = Constants.SQLTIMEOUT_SEC;

							dvLogNoMax = sqlStatement.SelectSingleStatement(sqlAccessor);
						}
						lLogNoMax = (long)StringUtility.ToValue(dvLogNoMax[0][0], (long)0);

						//------------------------------------------------------
						// ファイル終端まで繰り返す
						//------------------------------------------------------
						using (SqlStatement sqlStatement = new SqlStatement("ImportW3cLog", "Insert"))
						{
							// SQLタイムアウト値設定（各ＤＢファイルの自動拡張を考慮して多めにとる）
							sqlStatement.CommandTimeout = Constants.SQLTIMEOUT_SEC;

							List<string> lColumns = null;
							while (streamReader.Peek() != -1)
							{
								// １行読み込む
								string strLine = streamReader.ReadLine();

								// カラム定義作成（ログの途中から発生することもある）
								if (strLine.StartsWith(STR_LOGHEAD_FIELD_DEFINE))
								{
									lColumns = new List<string>();

									string[] strColumns = strLine.Replace(STR_LOGHEAD_FIELD_DEFINE, "").Split(' ');
									m_dicColumnOrder = new Dictionary<string, int>();

									// フィールド定義作成
									for (int iLoop = 0; iLoop < strColumns.Length; iLoop++)
									{
										if (strColumns[iLoop] != "")
										{
											lColumns.Add(strColumns[iLoop]);
											m_dicColumnOrder.Add(strColumns[iLoop], iLoop);
										}
									}

									// 定義作成完了。次の行へ
									continue;
								}
								else if (strLine.StartsWith("#") || strLine.StartsWith(" "))
								{
									// コメント・ラストの空白は読み飛ばす
									continue;
								}

								// 呼び出しファイル名が定義に含まれていない場合は読み飛ばす
								if (m_dicColumnOrder.ContainsKey(STR_FIELD_CS_URI_STEM) == false)
								{
									continue;
								}

								// 半角スペースで分割
								strSplitLine = strLine.Split(' ');

								// 定義とカラム数が合わない場合は読み飛ばす
								if (strSplitLine.Length != lColumns.Count)
								{
									continue;
								}

								// 取り込み対象でない場合は読み飛ばす
								if (strSplitLine[m_dicColumnOrder[STR_FIELD_CS_URI_STEM]] != Constants.TARGETURL_LOGIMPORT)
								{
									continue;
								}

								// 日付計算してパラメタ格納（日本時間は＋９）
								DateTime dtGMT = DateTime.Parse(strSplitLine[0] + " " + strSplitLine[1]);
								DateTime dtJapan = dtGMT.AddHours(9);
								Hashtable htInput = new Hashtable();
								htInput.Add(Constants.FIELD_ACCESSLOG_ACCESS_DATE, dtJapan.ToString("yyyy/MM/dd"));
								htInput.Add(Constants.FIELD_ACCESSLOG_ACCESS_TIME, dtJapan.ToString("HH:mm:ss"));
								htInput.Add(Constants.FIELD_ACCESSLOG_CLIENT_IP, (m_dicColumnOrder.ContainsKey(Constants.W3CLOG_COLUMN_CLIENTIP)) ? strSplitLine[m_dicColumnOrder[Constants.W3CLOG_COLUMN_CLIENTIP]] : "");
								htInput.Add(Constants.FIELD_ACCESSLOG_SERVER_NAME, (m_dicColumnOrder.ContainsKey(Constants.W3CLOG_COLUMN_SERVERNAME)) ? strSplitLine[m_dicColumnOrder[Constants.W3CLOG_COLUMN_SERVERNAME]] : "");
								htInput.Add(Constants.FIELD_ACCESSLOG_SERVER_IP, (m_dicColumnOrder.ContainsKey(Constants.W3CLOG_COLUMN_SERVERIP)) ? strSplitLine[m_dicColumnOrder[Constants.W3CLOG_COLUMN_SERVERIP]] : "");
								htInput.Add(Constants.FIELD_ACCESSLOG_SERVER_PORT, (m_dicColumnOrder.ContainsKey(Constants.W3CLOG_COLUMN_SERVER_PORT)) ? strSplitLine[m_dicColumnOrder[Constants.W3CLOG_COLUMN_SERVER_PORT]] : "");
								htInput.Add(Constants.FIELD_ACCESSLOG_PROTOCOL_STATUS, (m_dicColumnOrder.ContainsKey(Constants.W3CLOG_COLUMN_PROTOCOLSTAT)) ? strSplitLine[m_dicColumnOrder[Constants.W3CLOG_COLUMN_PROTOCOLSTAT]] : "");
								htInput.Add(Constants.FIELD_ACCESSLOG_USER_AGENT, (m_dicColumnOrder.ContainsKey(Constants.W3CLOG_COLUMN_USERAGENT)) ? strSplitLine[(int)m_dicColumnOrder[Constants.W3CLOG_COLUMN_USERAGENT]] : "");
								htInput.Add(Constants.FIELD_ACCESSLOG_USER_AGENT_KBN, Constants.SMARTPHONE_OPTION_ENABLED && SmartPhoneUtility.CheckSmartPhone((string)htInput[Constants.FIELD_ACCESSLOG_USER_AGENT]) ? "SP" : "PC");
								// ↓cs(Referer) はあてにならないので削除
								// URL分割
								//string strAccessUrl = ((m_htColumnOrder.Contains(W3CLOG_COLUMN_URL)) ? strSplitLine[(int)m_htColumnOrder[W3CLOG_COLUMN_URL]] : "");
								//string[] strUrls = SplitUrl(System.Web.HttpUtility.UrlDecode(strAccessUrl, System.Text.Encoding.GetEncoding("shift_jis")));
								//htInput.Add(Constants.FIELD_ACCESSLOG_URL_DOMAIN, strUrls[1]);
								//htInput.Add(Constants.FIELD_ACCESSLOG_URL_PAGE, strUrls[2]);
								//htInput.Add(Constants.FIELD_ACCESSLOG_URL_PARAM, strUrls[3]);
								// ↑cs(Referer) はあてにならないので削除
								// 画像パラメタ解析
								string[] strParams = ((m_dicColumnOrder.ContainsKey(Constants.W3CLOG_COLUMN_PARAMS)) ? strSplitLine[m_dicColumnOrder[Constants.W3CLOG_COLUMN_PARAMS]] : "").Replace("?", "").Split('&');
								foreach (string strParam in strParams)
								{
									if (strParam.StartsWith(Constants.W3CLOG_PARAM_ACCOUNTID + "="))
									{
										if (htInput.ContainsKey(Constants.FIELD_ACCESSLOG_ACCOUNT_ID) == false)
										{
											htInput.Add(Constants.FIELD_ACCESSLOG_ACCOUNT_ID, strParam.Split('=')[1]);
										}
									}
									else if (strParam.StartsWith(Constants.W3CLOG_PARAM_ACCESSUSERID + "="))
									{
										if (htInput.ContainsKey(Constants.FIELD_ACCESSLOG_ACCESS_USER_ID) == false)
										{
											htInput.Add(Constants.FIELD_ACCESSLOG_ACCESS_USER_ID, strParam.Split('=')[1]);
										}
									}
									else if (strParam.StartsWith(Constants.W3CLOG_PARAM_SESSIONID + "="))
									{
										if (htInput.ContainsKey(Constants.FIELD_ACCESSLOG_SESSION_ID) == false)
										{
											htInput.Add(Constants.FIELD_ACCESSLOG_SESSION_ID, strParam.Split('=')[1]);
										}
									}
									else if (strParam.StartsWith(Constants.W3CLOG_PARAM_REALUSERID + "="))
									{
										if (htInput.ContainsKey(Constants.FIELD_ACCESSLOG_REAL_USER_ID) == false)
										{
											htInput.Add(Constants.FIELD_ACCESSLOG_REAL_USER_ID, strParam.Split('=')[1]);
										}
									}
									else if (strParam.StartsWith(Constants.W3CLOG_PARAM_FIRSTLOGINFLG + "="))
									{
										if (htInput.ContainsKey(Constants.FIELD_ACCESSLOG_FIRST_LOGIN_FLG) == false)
										{
											htInput.Add(Constants.FIELD_ACCESSLOG_FIRST_LOGIN_FLG, strParam.Split('=')[1]);
										}
									}
									else if (strParam.StartsWith(Constants.W3CLOG_PARAM_REFERER + "="))
									{
										if (htInput.ContainsKey(Constants.FIELD_ACCESSLOG_REFERRER_DOMAIN) == false)
										{
											string[] strRef = SplitUrl(System.Web.HttpUtility.UrlDecode(strParam.Split('=')[1]));
											htInput.Add(Constants.FIELD_ACCESSLOG_REFERRER_DOMAIN, strRef[1]);
											htInput.Add(Constants.FIELD_ACCESSLOG_REFERRER_PAGE, strRef[2]);
											htInput.Add(Constants.FIELD_ACCESSLOG_REFERRER_PARAM, strRef[3]);
										}
									}
									else if (strParam.StartsWith(Constants.W3CLOG_PARAM_SRCHENGN + "="))
									{
										if (htInput.ContainsKey(Constants.FIELD_ACCESSLOG_SEARCH_ENGINE) == false)
										{
											htInput.Add(Constants.FIELD_ACCESSLOG_SEARCH_ENGINE, strParam.Split('=')[1]);
										}
									}
									else if (strParam.StartsWith(Constants.W3CLOG_PARAM_SRCHWORD + "="))
									{
										if (htInput.ContainsKey(Constants.FIELD_ACCESSLOG_SEARCH_WORDS) == false)
										{
											// URLデコードを行う
											htInput.Add(Constants.FIELD_ACCESSLOG_SEARCH_WORDS, XX_SearchWordUtility.UrlDecode(strParam.Split('=')[1]));
										}
									}
									else if (strParam.StartsWith(Constants.W3CLOG_PARAM_ACTIONKBN + "="))
									{
										if (htInput.ContainsKey(Constants.FIELD_ACCESSLOG_ACTION_KBN) == false)
										{
											htInput.Add(Constants.FIELD_ACCESSLOG_ACTION_KBN, strParam.Split('=')[1]);
										}
									}
									else if (strParam.StartsWith(Constants.W3CLOG_PARAM_URLDOMAIN + "="))
									{
										if (htInput.ContainsKey(Constants.FIELD_ACCESSLOG_URL_DOMAIN) == false)
										{
											htInput.Add(Constants.FIELD_ACCESSLOG_URL_DOMAIN, System.Web.HttpUtility.UrlDecode(strParam.Split('=')[1]));
										}
									}
									else if (strParam.StartsWith(Constants.W3CLOG_PARAM_URLPAGE + "="))
									{
										if (htInput.ContainsKey(Constants.FIELD_ACCESSLOG_URL_PAGE) == false)
										{
											htInput.Add(Constants.FIELD_ACCESSLOG_URL_PAGE, System.Web.HttpUtility.UrlDecode(strParam.Split('=')[1]));
										}
									}
									else if (strParam.StartsWith(Constants.W3CLOG_PARAM_URLPARAM + "="))
									{
										if (htInput.ContainsKey(Constants.FIELD_ACCESSLOG_URL_PARAM) == false)
										{
											htInput.Add(Constants.FIELD_ACCESSLOG_URL_PARAM, System.Web.HttpUtility.UrlDecode(strParam.Split('=')[1]));
										}
									}
									else if (strParam.StartsWith(Constants.W3CLOG_PARAM_ACS_INTERVAL + "="))
									{
										string strValue = strParam.Split('=')[1];

										/* int.TryParseで数値チェックを行うためコメント
										// Macの特定ブラウザでNaNとなっている場合がある
										if (strValue == "NaN")
										{
											strValue = "0";
										}
										// JavaScirpt側で何日ぶりにアクセスしたかの日数を秒数のIntervalから
										// parseIntを用いて算出しているが「e-」が出力されることがあるらしい。その場合は「0」に補正する。
										else if (strValue.Contains("e-"))
										{
											strValue = "0";
										}*/
										// 数値に変換出来ない文字列が入っていた場合、「0」に補正する。
										int iCheckValue;
										if (int.TryParse(strValue, out iCheckValue) == false)
										{
											iCheckValue = 0;
										}

										if (htInput.ContainsKey(Constants.FIELD_ACCESSLOG_ACCESS_INTERVAL) == false)
										{
											htInput.Add(Constants.FIELD_ACCESSLOG_ACCESS_INTERVAL, iCheckValue.ToString());
										}
									}
									else if (strParam.StartsWith(Constants.W3CLOG_PARAM_S_HEAD))
									{
										string[] strParamS = strParam.Split('=');
										if (htInput.ContainsKey("s" + strParamS[0].Replace(Constants.W3CLOG_PARAM_S_HEAD, "")) == false)
										{
											htInput.Add("s" + strParamS[0].Replace(Constants.W3CLOG_PARAM_S_HEAD, ""), System.Web.HttpUtility.UrlDecode(strParamS[1]));
										}
									}
									else if (strParam.StartsWith(Constants.W3CLOG_PARAM_P_HEAD))
									{
										string[] strParamP = strParam.Split('=');
										if (htInput.ContainsKey("p" + strParamP[0].Replace(Constants.W3CLOG_PARAM_P_HEAD, "")) == false)
										{
											htInput.Add("p" + strParamP[0].Replace(Constants.W3CLOG_PARAM_P_HEAD, ""), System.Web.HttpUtility.UrlDecode(strParamP[1]));
										}
									}
								}

								// パラメタチェック
								foreach (string strInsertField in Constants.ACCESSLOG_INSERT_FIELDS)
								{
									if (htInput.Contains(strInsertField) == false)
									{
										htInput.Add(strInsertField, "");
									}
								}

								//------------------------------------------------------
								// インサート実行
								//------------------------------------------------------
								sqlStatement.ExecStatement(sqlAccessor, htInput);

								//------------------------------------------------------
								// スリープ実行
								//------------------------------------------------------
								System.Threading.Thread.Sleep(5);

							} // while (streamReader.Peek() != -1)

						} // using (SqlStatement sqlStatement ～

					}
					catch (Exception ex)
					{
						try
						{
							// 途中まで取り込んだログをロールバック
							using (SqlStatement sqlStatement = new SqlStatement("ImportW3cLog", "DeleteLog"))
							{
								Hashtable htInput = new Hashtable();
								htInput.Add(Constants.FIELD_ACCESSLOG_LOG_NO, lLogNoMax);

								sqlStatement.ExecStatement(sqlAccessor, htInput);
							}
						}
						catch (Exception ex2)
						{
							AppLogger.WriteError(ex2);
						}

						// エラー情報格納してスロー
						string strErrorInfo = "ファイル名：" + strActiveFilePath.Substring(strActiveFilePath.LastIndexOf(@"\") + 1);
						if ((strSplitLine != null) && (strSplitLine.Length > 1))
						{
							strErrorInfo += ", 対象:" + strSplitLine[0] + " " + strSplitLine[1];
						}

						throw new Exception(strErrorInfo + "\r\n", ex);
					}
				}
			}

			//------------------------------------------------------
			// 結果作成
			//------------------------------------------------------
			if (this.NotImportFileNames.Count + this.FailedFileNames.Count + this.SuccessFileNames.Count != 0)
			{
				if (this.NotImportFileNames.Count == 0)
				{
					// 成功が１つもない場合、警告
					this.ProcessResult = PROCESS_RESULT.ALERT;
				}
				else if (this.FailedFileNames.Count == 0)
				{
					// 失敗が無い場合のみ成功
					this.ProcessResult = PROCESS_RESULT.SUCCESS;
				}
				else
				{
					// 失敗がひとつでもあれば失敗
					this.ProcessResult = PROCESS_RESULT.FAILED;
				}
			}
		}

		/// <summary>
		/// URLをプロトコル・ドメイン・パス・パラメタへ分割する
		/// </summary>
		/// <param name="strUrl">対象URL</param>
		/// <returns>文字配列（0:プロトコル、1:ドメイン、2:パス、3:パラメタ）</returns>
		private string[] SplitUrl(string strSrcUrl)
		{
			string[] strUrlParam = strSrcUrl.Split('?');
			string strUrl = strUrlParam[0];
			string strProtocol = null;
			string strDomain = null;
			string strPagePath = null;
			string strParam = (strUrlParam.Length != 1) ? strUrlParam[1] : "";

			// プロトコル取得（プロトコルなしの場合も対応）
			int iPointer = 0;
			if (strUrl.IndexOf("://") != -1)
			{
				strProtocol = strUrl.Substring(0, strUrl.IndexOf("://"));
				iPointer = strUrl.IndexOf("://") + 3;
			}
			else
			{
				strProtocol = "";
			}

			// ドメイン取得
			if (strUrl.IndexOf("/", iPointer) != -1)
			{
				strDomain = strUrl.Substring(iPointer, strUrl.IndexOf("/", iPointer) - iPointer);
				iPointer = strUrl.IndexOf("/", iPointer) + 1;
			}
			else
			{
				// ドメインまでの場合（スラッシュなし）
				strDomain = strUrl.Substring(iPointer);
				iPointer = strUrl.Length;
			}

			// ページパス取得
			if (iPointer != strUrl.Length)
			{
				strPagePath = strUrl.Substring(iPointer);
			}
			else
			{
				strPagePath = "";
			}

			return new string[] { strProtocol, strDomain, strPagePath, strParam };
		}

		/// <summary>
		/// 完了ログ圧縮
		/// </summary>
		public void CompressCompleteLogFiles()
		{
			foreach (string strFilePath in Directory.GetFiles(Constants.PATH_W3CLOG_COMPLETE))
			{
				if (strFilePath.ToLower().EndsWith(".log"))
				{
					// ログ圧縮＆削除
					try
					{
						w2.Common.Util.Archiver.ZipArchiver zaZipArchiver = new w2.Common.Util.Archiver.ZipArchiver();
						zaZipArchiver.CompressFile(
							strFilePath,
							Constants.PATH_W3CLOG_COMPLETE,
							Path.GetDirectoryName(strFilePath) + @"\" + Path.GetFileNameWithoutExtension(strFilePath) + ".zip");

						File.Delete(strFilePath);
					}
					catch (Exception ex)
					{
						AppLogger.WriteError("ログファイルの圧縮に失敗しました", ex);
					}
				}
				else if (strFilePath.ToLower().EndsWith(".zip"))
				{
					// 3ヶ月経過したzipファイルは削除
					string strFileNameNoExtention = Path.GetFileNameWithoutExtension(strFilePath);
					try
					{
						DateTime dtFileTime = DateTime.Parse("20" + strFileNameNoExtention.Substring(strFileNameNoExtention.Length - 6, 2)
							+ "/" + strFileNameNoExtention.Substring(strFileNameNoExtention.Length - 4, 2)
							+ "/" + strFileNameNoExtention.Substring(strFileNameNoExtention.Length - 2, 2)
						);

						if (DateTime.Now > dtFileTime.AddMonths(3))
						{
							File.Delete(strFilePath);
						}
					}
					catch (Exception) { }
				}
			}
		}
	}
}