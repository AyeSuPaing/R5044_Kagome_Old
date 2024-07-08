/*
=========================================================================================================
  Module      : 外部連携webサービス処理用クラス(LinkUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : 外部連携webサービス処理用を司るクラス。
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace w2.Plugin.P0011_Intercom.CooperationWebService
{
	public class IntercomWebService
	{

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public IntercomWebService()
		{

		}

		#region -Execute 外部連携処理実行
		
		/// <summary>
		/// 外部連携処理実行
		/// </summary>
		/// <param name="procType">外部連携処理に渡す処理区分</param>
		/// <param name="logFileDirPath">外部連携用ログファイルの出力先ディレクトリパス</param>
		/// <param name="dom">webサービスに認証がかかっている場合のドメイン名</param>
		/// <param name="id">webサービスに認証がかかっている場合のユーザID</param>
		/// <param name="flg">webサービスに認証がかかっているか否かを判断するフラグ
		/// 0：認証なし
		/// 1：認証あり
		/// </param> 
		/// <param name="pa">webサービスに認証がかかっている場合のパスワード</param>
		/// <param name="sendDs">外部連携処理に渡すデータセット</param>
		/// <returns>外部連携処理から返却されてきたデータセット</returns>
		public DataSet Execute(string procType, DataSet sendDs, string logFileDirPath,
			string id, string pa, string dom, string flg)
		{
			DataSetLogWriter dsWriter = new DataSetLogWriter(logFileDirPath);
			ErrorLogWriter errWriter = new ErrorLogWriter(logFileDirPath);
			ErrorCsvLogWriter errCsv = new ErrorCsvLogWriter(logFileDirPath);

			DataSet returnDs = null;
			DataSet paraDs = new DataSet();
			foreach (DataTable dt in sendDs.Tables)
			{
				DataTable clTable = dt.Copy();
				clTable.TableName = dt.TableName;
				paraDs.Tables.Add(clTable);
			}
			

			try
			{
				//処理区分毎
				switch (procType)
				{
					case CooperationWebServiceConst.PROC_TYPE_RECOMMEND_EVENT:

						//レコメンドイベント連携
											
						try
						{
							//webサービス実施
							using (Intercom_Event.RecommendService serviceClient = new Intercom_Event.RecommendService())
							{
								if (flg == "1")
								{
									serviceClient.Credentials
										= System.Net.CredentialCache.DefaultCredentials;
									serviceClient.Credentials
										= new System.Net.NetworkCredential
											(id,
											pa,
											dom);
								}

								//送信前ログ
								dsWriter.Write(paraDs, procType, CooperationWebServiceConst.WAY_SEND);
								returnDs = serviceClient.GetEvent(paraDs);
							}
						}
						catch (Exception ex)
						{
							//webサービス実行がらみのエラー
							//WebServiceExceptionで発生Exceptionをラップ
							throw new WebServiceException("レコメンド連携時にwebサービス実行例外発生", ex);
						}
						break;
					case CooperationWebServiceConst.PROC_TYPE_RECOMMEND_PRODUCT:

						//レコメンド商品連携
											
						try
						{
							//webサービス実施
							using (Intercom_Event.RecommendService serviceClient = new Intercom_Event.RecommendService())
							{
								if (flg == "1")
								{
									serviceClient.Credentials
										= System.Net.CredentialCache.DefaultCredentials;
									serviceClient.Credentials
										= new System.Net.NetworkCredential
											(id,
											pa,
											dom);
								}

								//送信前ログ
								dsWriter.Write(paraDs, procType, CooperationWebServiceConst.WAY_SEND);
								returnDs = serviceClient.GetEventProduct(paraDs);
							}
						}
						catch (Exception ex)
						{
							//webサービス実行がらみのエラー
							//WebServiceExceptionで発生Exceptionをラップ
							throw new WebServiceException("レコメンド連携時にwebサービス実行例外発生", ex);
						}
						break;

					case CooperationWebServiceConst.PROC_TYPE_SERIAL_CHECK:

						//シリアルチェック(シリアルNo入力直後)連携

						try
						{
							//webサービス実施
							using (Intercom_Event.RecommendService serviceClient = new Intercom_Event.RecommendService())
							{
								if (flg == "1")
								{
									serviceClient.Credentials
										= System.Net.CredentialCache.DefaultCredentials;
									serviceClient.Credentials
										= new System.Net.NetworkCredential
											(id,
											pa,
											dom);
								}

								dsWriter.Write(paraDs, procType, CooperationWebServiceConst.WAY_SEND);
								returnDs = serviceClient.AuthSerial(paraDs);
							}
						}
						catch (Exception ex)
						{
							//webサービス実行がらみのエラー
							//WebServiceExceptionで発生Exceptionをラップ
							throw new WebServiceException("シリアルチェック連携時にwebサービス実行例外発生", ex);
						}

						break;

					default:

						//上記以外

						returnDs = null;
						break;

				}
				//戻りログ出力
				dsWriter.Write(returnDs, procType, CooperationWebServiceConst.WAY_RECEIVE);

				//処理結果データテーブル判断
				if (returnDs == null)
				{
					throw new Exception("Webサービスからデータセットが返却されませんでした。");
				}

				if (returnDs.Tables.Count == 0)
				{
					throw new Exception("Webサービスからデータテーブルが一つも返却されませんでした。");
				}

				if (returnDs.Tables.Contains(CooperationWebServiceConst.TAB_RESULT) == false)
				{
					throw new Exception("Webサービスから処理結果データテーブルが一つも返却されませんでした。");
				}

				if (returnDs.Tables[CooperationWebServiceConst.TAB_RESULT].Rows.Count == 0)
				{
					throw new Exception("Webサービスから処理結果行が一件も返却されませんでした。");
				}

				if (returnDs.Tables[CooperationWebServiceConst.TAB_RESULT].Columns.Contains(CooperationWebServiceConst.COL_PROCRESULT) == false)
				{
					throw new Exception("Webサービスから処理結果区分が返却されませんでした。");
				}

				if (returnDs.Tables[CooperationWebServiceConst.TAB_RESULT].Columns.Contains(CooperationWebServiceConst.COL_PROCMSG) == false)
				{
					throw new Exception("Webサービスから処理メッセージが返却されませんでした。");
				}

				//処理結果データテーブル行
				DataRow resultRow = returnDs.Tables[CooperationWebServiceConst.TAB_RESULT].Rows[0];
				//処理結果区分
				string procResult = (string)returnDs.Tables[CooperationWebServiceConst.TAB_RESULT].Rows[0][CooperationWebServiceConst.COL_PROCRESULT];
				//処理結果メッセージ
				string procResulMsgt = (string)returnDs.Tables[CooperationWebServiceConst.TAB_RESULT].Rows[0][CooperationWebServiceConst.COL_PROCMSG];

				//処理結果区分判定
				switch (procResult)
				{
					case CooperationWebServiceConst.PROC_RESULT_SUCCESS:
						//正常終了時
						//特に何もなし
						break;

					case CooperationWebServiceConst.PROC_RESULT_FAILED:
						//異常終了時
						//異常終了メッセージ詰めて返す
						throw new Exception(procResulMsgt);
					default:
						throw new Exception("Webサービスから不正な処理区分が返却されました。");
				}
			}
			catch (WebServiceException ex)
			{
				try
				{
					errWriter.Write("連携呼出でエラーが発生:" +
						"パラメータデータセット：" + GetParamDsLogValue(sendDs) +
						"エラーメッセージ：" + 
						ex.Message + ex.InnerException.Message + ex.InnerException.StackTrace);

				}
				catch
				{

				}
			}
			catch (Exception ex)
			{
				try
				{
					errWriter.Write("エラーが発生:" +
						"パラメータデータセット：" + GetParamDsLogValue(sendDs) +
						"エラーメッセージ：" + 
						ex.Message + ex.StackTrace);

				}
				catch
				{

				}
			}

			return returnDs;

		}

		#endregion

		#region -CreateProcTable 処理区分用データテーブル作成

		/// <summary>
		/// 処理区分用データテーブル作成
		/// </summary>
		/// <param name="procType">設定する処理区分の値</param>
		/// <returns>作成した処理区分用データテーブル</returns>
		private DataTable CreateProcTable(string procType)
		{
			DataTable dt = new DataTable(CooperationWebServiceConst.TAB_PROC);
			dt.Columns.Add(CooperationWebServiceConst.COL_PROCTYPE);

			DataRow dr = dt.NewRow();
			dr[0] = procType;

			dt.Rows.Add(dr);

			return dt;
		}

		#endregion

		#region -GetParamDsLogValue エラーログ出力用受領データログ文字取得

		/// <summary>
		/// エラーログ出力用のHostData内容文字列
		/// </summary>
		/// <returns></returns>
		private string GetParamDsLogValue(DataSet ds)
		{
			StringBuilder strBuilder = new StringBuilder();

			strBuilder.Append("");

			try
			{
				foreach (DataTable dt in ds.Tables)
				{
					strBuilder.Append("テーブル名[" + dt.TableName + "]");
					//データ行カウンタ
					int rCnt = 1;

					foreach (DataRow dr in dt.Rows)
					{
						strBuilder.Append(rCnt.ToString() + "行目<<");
						foreach (DataColumn dc in dt.Columns)
						{
							//項目毎に出力
							strBuilder.Append(dc.ColumnName + ":" + ConvNullToEmpty(dr[dc.ColumnName]) + ";");
						}
						strBuilder.Append(">>" + rCnt.ToString() + "行目End");
						rCnt++;
					}

				}
			}
			catch
			{
				//エラーが出てもスルー
			}

			return strBuilder.ToString();
		}
		#endregion


		#region NullまたはDBNullかどうかをチェック

		/// <summary>
		/// NullまたはDBNullの場合にはEmptyに
		/// </summary>
		/// <param name="targetValue"></param>
		/// <returns></returns>
		private string ConvNullToEmpty(object targetValue)
		{
			if (targetValue == null)
			{
				return "";
			}

			if (targetValue == DBNull.Value)
			{
				return "";
			}

			return targetValue.ToString();

		}

		#endregion

	}
}
