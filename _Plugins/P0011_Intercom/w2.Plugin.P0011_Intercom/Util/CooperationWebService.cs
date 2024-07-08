using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace w2.Plugin.P0011_Intercom.Util
{
	internal class CooperationWebService
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal CooperationWebService()
		{

		}

		/// <summary>
		/// 外部連携処理実行
		/// </summary>
		/// <param name="procType">外部連携処理に渡す処理区分</param>
		/// <param name="sendData">外部連携処理に渡すデータテーブル</param>
		/// <param name="logFileDirPath">外部連携用ログファイルの出力先ディレクトリパス</param>
		/// <returns>外部連携処理から返却されてきたデータセット</returns>
		public DataSet Execute(string procType, DataSet sendData, string logFileDirPath)
		{
			DataSet sendDs = new DataSet();
			DataSet returnDs = null;

			//処理区分毎
			switch (procType)
			{
				case PluginConst.PROC_TYPE_USER_REGIST:

					//会員情報登録_連携

					//処理区分用データテーブル追加
					sendDs.Tables.Add(CreateProcTable(procType));

					//データ用データテーブル追加
					foreach(DataTable dt in sendData.Tables)
					{
						DataTable copyDt = dt.Copy();
						copyDt.TableName = dt.TableName;
						sendDs.Tables.Add(copyDt);
					}

					try
					{
						//webサービス実施
						using (Intercom_User.w2iUserSyncService serviceClient = new Intercom_User.w2iUserSyncService())
						{
							if (CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_AUTH) == "1")
							{
								serviceClient.Credentials 
									= System.Net.CredentialCache.DefaultCredentials;
								serviceClient.Credentials 
									= new System.Net.NetworkCredential
										(CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_ID),
										CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_PA),
										CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_DO));
							}

							//送信前ログ
							CommUtil._FileUtil().WriteSendDataSetLog(sendDs, procType);
							returnDs = serviceClient.UserSync(sendDs);
						}
					}
					catch (Exception ex)
					{
						//webサービス実行がらみのエラー
						//WebServiceExceptionで発生Exceptionをラップ
						throw new WebServiceException("会員情報登録連携時にwebサービス実行例外発生(SyncUserData）", ex);
					}

					break;

				case PluginConst.PROC_TYPE_USER_DELETE:

					//会員情報退会_連携

					//処理区分用データテーブル追加
					sendDs.Tables.Add(CreateProcTable(procType));

					//データ用データテーブル追加
					foreach (DataTable dt in sendData.Tables)
					{
						DataTable copyDt = dt.Copy();
						copyDt.TableName = dt.TableName;
						sendDs.Tables.Add(copyDt);
					}

					try
					{
						//webサービス実施
						using (Intercom_User.w2iUserSyncService serviceClient = new Intercom_User.w2iUserSyncService())
						{
							if (CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_AUTH) == "1")
							{
								serviceClient.Credentials
									= System.Net.CredentialCache.DefaultCredentials;
								serviceClient.Credentials
									= new System.Net.NetworkCredential
										(CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_ID),
										CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_PA),
										CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_DO));
							}

							//送信前ログ
							CommUtil._FileUtil().WriteSendDataSetLog(sendDs, procType);
							returnDs = serviceClient.UserSync(sendDs);
						}
					}
					catch (Exception ex)
					{
						//webサービス実行がらみのエラー
						//WebServiceExceptionで発生Exceptionをラップ
						throw new WebServiceException("会員情報退会連携時にwebサービス実行例外発生(SyncUserData）", ex);
					}

					break;

				case PluginConst.PROC_TYPE_SERIAL_DELETE:

					//シリアル抹消連携

					//データ用データテーブル追加
					foreach (DataTable dt in sendData.Tables)
					{
						DataTable copyDt = dt.Copy();
						copyDt.TableName = dt.TableName;
						sendDs.Tables.Add(copyDt);
					}

					try
					{
						//webサービス実施
						using (Intercom_Event.RecommendService serviceClient = new Intercom_Event.RecommendService())
						{
							if (CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_AUTH) == "1")
							{
								serviceClient.Credentials
									= System.Net.CredentialCache.DefaultCredentials;
								serviceClient.Credentials
									= new System.Net.NetworkCredential
										(CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_ID),
										CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_PA),
										CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_DO));
							}

							//送信前ログ
							CommUtil._FileUtil().WriteSendDataSetLog(sendDs, procType);
							returnDs = serviceClient.ConsumeSerial(sendDs);
						}
					}
					catch (Exception ex)
					{
						//webサービス実行がらみのエラー
						//WebServiceExceptionで発生Exceptionをラップ
						throw new WebServiceException("シリアル抹消連携時にwebサービス実行例外発生(SyncUserData）", ex);
					}

					break;

				case PluginConst.PROC_TYPE_RECOMMEND_EVENT:

					//レコメンドイベント連携

					//データ用データテーブル追加
					foreach (DataTable dt in sendData.Tables)
					{
						DataTable copyDt = dt.Copy();
						copyDt.TableName = dt.TableName;
						sendDs.Tables.Add(copyDt);
					}

					try
					{
						//webサービス実施
						using (Intercom_Event.RecommendService serviceClient = new Intercom_Event.RecommendService())
						{
							if (CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_AUTH) == "1")
							{
								

								serviceClient.Credentials
									= System.Net.CredentialCache.DefaultCredentials;
								serviceClient.Credentials
									= new System.Net.NetworkCredential
										(CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_ID),
										CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_PA),
										CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_DO));

								

							}

							//送信前ログ
							CommUtil._FileUtil().WriteSendDataSetLog(sendDs, procType);
							returnDs = serviceClient.GetEvent(sendDs);
						}
					}
					catch (Exception ex)
					{
						//webサービス実行がらみのエラー
						//WebServiceExceptionで発生Exceptionをラップ
						throw new WebServiceException("レコメンドイベント連携時にwebサービス実行例外発生(RecommendEventID)", ex);
					}
					break;
				case PluginConst.PROC_TYPE_RECOMMEND_PRODUCT:

					//レコメンド商品連携

					//データ用データテーブル追加
					foreach (DataTable dt in sendData.Tables)
					{
						DataTable copyDt = dt.Copy();
						copyDt.TableName = dt.TableName;
						sendDs.Tables.Add(copyDt);
					}

					try
					{
						//webサービス実施
						using (Intercom_Event.RecommendService serviceClient = new Intercom_Event.RecommendService())
						{
							if (CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_AUTH) == "1")
							{
								serviceClient.Credentials
									= System.Net.CredentialCache.DefaultCredentials;
								serviceClient.Credentials
									= new System.Net.NetworkCredential
										(CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_ID),
										CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_PA),
										CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_DO));
							}

							//送信前ログ
							CommUtil._FileUtil().WriteSendDataSetLog(sendDs, procType);
							returnDs = serviceClient.GetEventProduct(sendDs);
						}
					}
					catch (Exception ex)
					{
						//webサービス実行がらみのエラー
						//WebServiceExceptionで発生Exceptionをラップ
						throw new WebServiceException("レコメンド商品連携時にwebサービス実行例外発生(RecommendProductID)", ex);
					}
					break;

				case PluginConst.PROC_TYPE_SERIAL_CHECK:

					//シリアルチェック連携

					//データ用データテーブル追加
					foreach (DataTable dt in sendData.Tables)
					{
						DataTable copyDt = dt.Copy();
						copyDt.TableName = dt.TableName;
						sendDs.Tables.Add(copyDt);
					}

					try
					{
						//webサービス実施
						using (Intercom_Event.RecommendService serviceClient = new Intercom_Event.RecommendService())
						{
							if (CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_AUTH) == "1")
							{
								serviceClient.Credentials
									= System.Net.CredentialCache.DefaultCredentials;
								serviceClient.Credentials
									= new System.Net.NetworkCredential
										(CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_ID),
										CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_PA),
										CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_DO));
							}

							CommUtil._FileUtil().WriteSendDataSetLog(sendDs, procType);
							returnDs = serviceClient.AuthSerial(sendDs);
						}
					}
					catch (Exception ex)
					{
						//webサービス実行がらみのエラー
						//WebServiceExceptionで発生Exceptionをラップ
						throw new WebServiceException("シリアルチェック連携時にwebサービス実行例外発生(SerialCheck)", ex);
					}

					break;

				case PluginConst.PROC_TYPE_SERIAL_PRODUCT_CHECK:

					//シリアルチェック連携

					//データ用データテーブル追加
					foreach (DataTable dt in sendData.Tables)
					{
						DataTable copyDt = dt.Copy();
						copyDt.TableName = dt.TableName;
						sendDs.Tables.Add(copyDt);
					}

					try
					{
						//webサービス実施
						using (Intercom_Event.RecommendService serviceClient = new Intercom_Event.RecommendService())
						{
							if (CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_AUTH) == "1")
							{
								serviceClient.Credentials
									= System.Net.CredentialCache.DefaultCredentials;
								serviceClient.Credentials
									= new System.Net.NetworkCredential
										(CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_ID),
										CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_PA),
										CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_WEBSERVICE_DO));
							}

							CommUtil._FileUtil().WriteSendDataSetLog(sendDs, procType);
							returnDs = serviceClient.AuthSerialOverall(sendDs);
						}
					}
					catch (Exception ex)
					{
						//webサービス実行がらみのエラー
						//WebServiceExceptionで発生Exceptionをラップ
						throw new WebServiceException("シリアルチェック連携時にwebサービス実行例外発生(SerialCheck)", ex);
					}

					break;

				default:

					//上記以外

					returnDs = null;
					break;

			}
			//戻りログ出力
			CommUtil._FileUtil().WriteReceiveDataSetLog(returnDs, procType);
			
			//処理結果データテーブル判断
			if (returnDs == null)
			{
				throw new Exception("Webサービスからデータセットが返却されませんでした。");
			}

			if (returnDs.Tables.Count == 0)
			{
				throw new Exception("Webサービスからデータテーブルが一つも返却されませんでした。");
			}

			if (returnDs.Tables.Contains(PluginConst.TAB_RESULT) == false)
			{
				throw new Exception("Webサービスから処理結果データテーブルが一つも返却されませんでした。");
			}

			if (returnDs.Tables[PluginConst.TAB_RESULT].Rows.Count == 0)
			{
				throw new Exception("Webサービスから処理結果行が一件も返却されませんでした。");
			}

			if (returnDs.Tables[PluginConst.TAB_RESULT].Columns.Contains(PluginConst.COL_PROCRESULT) == false)
			{
				throw new Exception("Webサービスから処理結果区分が返却されませんでした。");
			}

			if (returnDs.Tables[PluginConst.TAB_RESULT].Columns.Contains(PluginConst.COL_PROCMSG) == false)
			{
				throw new Exception("Webサービスから処理メッセージが返却されませんでした。");
			}

			//処理結果データテーブル行
			DataRow resultRow = returnDs.Tables[PluginConst.TAB_RESULT].Rows[0];
			//処理結果区分
			string procResult = (string)returnDs.Tables[PluginConst.TAB_RESULT].Rows[0][PluginConst.COL_PROCRESULT];
			//処理結果メッセージ
			string procResulMsgt = (string)returnDs.Tables[PluginConst.TAB_RESULT].Rows[0][PluginConst.COL_PROCMSG];

			//処理結果区分判定
			switch (procResult)
			{
				case PluginConst.PROC_RESULT_SUCCESS:
					//正常終了時
					//特に何もなし
					break;

				case PluginConst.PROC_RESULT_FAILED:
					//異常終了時
					//異常終了メッセージ詰めて返す
					throw new Exception("Webサービスから処理区分がFailedで返却されました。：" + procResulMsgt);
				default:
					throw new Exception("Webサービスから不正な処理区分が返却されました。");
			}

			return returnDs;

		}

		#region 処理区分用データテーブル作成

		/// <summary>
		/// 処理区分用データテーブル作成
		/// </summary>
		/// <param name="procType">設定する処理区分の値</param>
		/// <returns>作成した処理区分用データテーブル</returns>
		private DataTable CreateProcTable(string procType)
		{
			DataTable dt = new DataTable(PluginConst.TAB_PROC);
			dt.Columns.Add(PluginConst.COL_PROCTYPE);

			DataRow dr = dt.NewRow();
			dr[0] = procType;

			dt.Rows.Add(dr);

			return dt;
		}

		#endregion

	}
}
