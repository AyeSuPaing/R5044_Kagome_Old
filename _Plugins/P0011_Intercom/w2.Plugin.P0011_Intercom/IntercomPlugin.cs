/*
=========================================================================================================
  Module      : Intercomプラグインの実装(ConcretePlugin.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
 2012/05/02 注文確定時、注文メモにシリアルナンバーを入れるタイミングを変更
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using w2.Plugin.P0011_Intercom.Util;
using System.Reflection;
using w2.Crypto;
using w2.Plugin;
using System.Web.UI;
using System.Web;
using System.Security.Cryptography;

namespace w2.Plugin.P0011_Intercom
{
	public class IntercomPlugin :
		IPlugin,
		User.IUserRegisteredPlugin,
		User.IUserWithdrawedPlugin,
		Order.IOrderValidatingPlugin,
		Order.IOrderCompletePlugin,
		User.IUserLoggedInPlugin,
		User.IUserModifiedPlugin
	{

		#region メンバ変数

		//Successプロパティ用
		private bool blSuccsess_ = false;
		//Messageプロパティ用
		private string strMessage_ = "";
		//Hostプロパティ用
		private IPluginHost host_ = null;

		#endregion

		#region IPluginHost実装


		public void Initialize(IPluginHost host)
		{
			//初期処理
			this.host_ = host;
		}

		public IPluginHost Host
		{
			//Hostプロパティ
			get { return this.host_; }
		}

		#endregion

		#region +OnRegistered 会員情報登録時

		/// <summary>
		/// 会員情報登録時
		/// </summary>
		public void OnRegistered()
		{
			DataSet ds = new DataSet();
			DataTable dt = null;

			//会員登録の際に実行
			try
			{
				//平文パスワードの取得
				string nonEncPass = GetNonEncPass();
				//データ取得
				dt = CommUtil._DBUtil().GetDataTable(PluginConst.SQL_SECTION_ID_USER_REGIST, this.host_, null);
				//テーブル名セット
				dt.TableName = PluginConst.TAB_USER;
				ds.Tables.Add(dt);

				//パスワードを平文でセット
				if (dt.Rows.Count > 0)
				{
					dt.Rows[0]["password"] = nonEncPass;
				}
				else
				{
					//取得データが0件の場合はException
					throw new Exception("登録対象の会員情報データが取得できませんでした。");
				}

				//連携処理コール
				DataSet rtnds = CommUtil._LinkUtil().Execute(PluginConst.PROC_TYPE_USER_REGIST,
					ds,
					CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_SENDDATA_LOGDIR));

				//Intercom採番会員Noテーブル
				DataTable userDatatable = rtnds.Tables[PluginConst.TAB_USERID];

				//Intercom採番会員NoをUpdate
				CommUtil._DBUtil().ExecuteSql(PluginConst.SQL_SECTION_ID_USER_REGIST_AFTER_UPDATE, this.host_, userDatatable);

			}
			catch (WebServiceException wEx)
			{
				//Webサービス実行時（通信障害等）エラー

				try
				{
					//エラーログ
					//ここではInnerExceptionを使う
					this.host_.WriteErrorLog(wEx.InnerException, wEx.Message + "HostData#" + GetHostDataKeyValue());
					SendErrorMailAll(wEx.InnerException, wEx);
					CommUtil._FileUtil().WriteErrorCSV(dt, "OnRegistered");
				}
				catch
				{

				}

			}
			catch (Exception ex)
			{
				try
				{
					//エラーログ
					this.host_.WriteErrorLog(ex, "HostData#" + GetHostDataKeyValue());
					SendErrorMail(ex);
					CommUtil._FileUtil().WriteErrorCSV(dt, "OnRegistered");
				}
				catch
				{

				}

			}
			finally
			{

			}

		}

		#endregion

		#region +OnWithdrawed 会員退会時

		/// <summary>
		/// 会員退会時
		/// </summary>
		public void OnWithdrawed()
		{
			//会員退会の際に実行
			try
			{
				//ユーザーID
				string userid = ConvNullToEmpty(this.host_.Data["user_id"]);

				//データ取得
				DataTable dt = CommUtil._DBUtil().GetDataTable(PluginConst.SQL_SECTION_ID_USER_DELETE, this.host_, null);

				//インターコム用ユーザID取得
				string icUserid = ConvNullToEmpty(dt.Rows[0]["attribute1"].ToString());

				//お客様名・メールアドレス取得
				string username = "";
				string mailadr = "";

				try
				{
					username = this.host_.Context.Session["w2cFront_login_user_name"].ToString();
					mailadr = this.host_.Context.Session["w2cFront_login_user_mail"].ToString();
				}
				catch
				{
				}

				//退会メール
				//this.host_.SendMail(CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_USERDELSYNC_SUBJECT),
				//    CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_USERDELSYNC_BODYTEMPLATE).Replace("{0}", userid).Replace("{1}",icUserid),
				//    CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_USERDELSYNC_FROM),
				//    GetAddresList(PluginConst.SETTING_KEY_USERDELSYNC_TOLIST),
				//    GetAddresList(PluginConst.SETTING_KEY_USERDELSYNC_CCLIST),
				//    GetAddresList(PluginConst.SETTING_KEY_USERDELSYNC_BCCLIST));

				this.host_.SendMail(CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_USERDELSYNC_SUBJECT),
					CreateDeleteMailBodyStr(dt, username, mailadr),
					CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_USERDELSYNC_FROM),
					GetAddresList(PluginConst.SETTING_KEY_USERDELSYNC_TOLIST),
					GetAddresList(PluginConst.SETTING_KEY_USERDELSYNC_CCLIST),
					GetAddresList(PluginConst.SETTING_KEY_USERDELSYNC_BCCLIST));

			}
			catch (Exception ex)
			{
				try
				{
					//エラーログ
					this.host_.WriteErrorLog(ex, "HostData#" + GetHostDataKeyValue());
					SendErrorMail(ex);
				}
				catch
				{

				}
			}
		}

		/// <summary>
		/// 退会メールの本文文字列作成
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		private string CreateDeleteMailBodyStr(DataTable dt, string username, string usermail)
		{
			string bodyStr = CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_USERDELSYNC_BODYTEMPLATE);

			if (dt.Rows.Count > 0)
			{
				foreach (DataColumn dc in dt.Columns)
				{
					bodyStr = bodyStr.Replace("@@" + dc.ColumnName, dt.Rows[0][dc.ColumnName].ToString());
				}
			}

			bodyStr = bodyStr.Replace("@@BEF_USERNAME", username);
			bodyStr = bodyStr.Replace("@@BEF_MAILADR", usermail);


			return bodyStr;
		}

		#endregion

		#region +OnValidating 注文検証時

		/// <summary>
		/// 注文検証時
		/// </summary>
		public void OnValidating()
		{
			//送信データセット
			DataSet sendds = new DataSet();

			//連携戻りデータセット
			DataSet rtnds = null;

			//シリアルチェックのwebサービスに渡すためのデータテーブル
			DataTable dt = new DataTable("SerialTargetTable");
			dt.Columns.Add("EventID");
			dt.Columns.Add("SerialNo");
			dt.Columns.Add("ProductID");
			dt.Columns.Add("VariationID");
			dt.Columns.Add("ProductCount");
			//dt.Columns.Add("OrderID"); //注文番号

			try
			{
				//シリアルチェックのwebサービスに渡すためのデータテーブル
				DataTable userdt = new DataTable("UserIDTable");
				userdt.Columns.Add("UserID");
				userdt.Columns.Add("LinkedUserID");
				DataRow userdr = userdt.NewRow();
				userdr["UserID"] = this.host_.Context.Session[PluginConst.PLUGIN_SESSION_KEY_ICID];
				userdr["LinkedUserID"] = this.host_.Context.Session[PluginConst.PLUGIN_SESSION_KEY_W2ID];
				userdt.Rows.Add(userdr);

				//イベント_商品_バリエーションで一位になるリスト
				//複数カートでなぜか購入数は合算できてしまうため
				List<string> eventProductList = new List<string>();

				//-----------------------------------------------------------------------------------------------------
				//セッションからカートリスト取得
				object ob = this.host_.Context.Session[PluginConst.SESSION_KEY_CART_LIST];
				//セッションからイベントディクショナリ取得
				IDictionary<string, Hashtable> evedic = (IDictionary<string, Hashtable>)this.host_.Context.Session[PluginConst.PLUGIN_SESSION_KEY_RECOMMEND];

				//セッションから商品ID、商品バリエーション、イベントID取出してみる
				foreach (IEnumerable catob in (IEnumerable)ob)
				{
					//catob.GetType().GetMethod(
					foreach (object ss in catob)
					{
						//リフレクション使って商品IDとバリエーションIDとってみるテスト
						object pid = ss.GetType().GetProperty("ProductId").GetValue(ss, null);	//商品ID
						object vid = ss.GetType().GetProperty("VariationId").GetValue(ss, null);	//商品バリエーションID
						object cnt = ss.GetType().GetProperty("Count").GetValue(ss, null);	//購入数

						//ProductOptionSettingList(この中にイベントIDいれてある）
						object eveids = ss.GetType().GetProperty("ProductOptionSettingList").GetValue(ss, null);

						object eveid = null;
						string streveid = "";

						if (eveids != null)
						{
							foreach (object so in (IEnumerable)eveids)
							{
								object valname = so.GetType().GetProperty("ValueName").GetValue(so, null);
								if (valname != null)
								{
									if ((string)valname == "eventid")
									{
										eveid = so.GetType().GetProperty("SettingValues").GetValue(so, null);
										if (eveid != null)
										{
											streveid = ((List<string>)eveid)[0];
											string serial = "";
											if (evedic.Keys.Contains(streveid))
											{
												serial = ConvNullToEmpty(evedic[streveid][PluginConst.COL_SERIALNO]);
											}

											//イベントに該当するシリアルNoがとれたときだけ
											if (serial != "")
											{
												//イベントID_商品ID_バリエーションIDがかぶらないものだけ
												if (eventProductList.Contains(streveid + ":" + pid + ":" + vid) == false)
												{

													DataRow dr = dt.NewRow();
													dr["EventID"] = streveid;
													dr["SerialNo"] = serial;
													dr["ProductID"] = pid;
													//商品IDとバリエーションIDが同じであれば、バリエーションIDは空にして連携
													if (pid.ToString() == vid.ToString())
													{
														dr["VariationID"] = "";
													}
													else
													{
														dr["VariationID"] = vid;
													}
													dr["ProductCount"] = cnt;
													dt.Rows.Add(dr);

													//データテーブルに追加したらかぶらないようにリストに登録
													eventProductList.Add(streveid + ":" + pid + ":" + vid);

												}
											}
										}

									}
								}
							}
						}
					}
				}

				sendds.Tables.Add(dt);
				sendds.Tables.Add(userdt);

				//連携処理コール
				if (dt.Rows.Count > 0)
				{
					rtnds = CommUtil._LinkUtil().Execute(PluginConst.PROC_TYPE_SERIAL_PRODUCT_CHECK, sendds,
						CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_SENDDATA_LOGDIR));

					//連携処理結果を取出してプロパティへセット
					if ((string)rtnds.Tables[PluginConst.TAB_RESULT].Rows[0][PluginConst.COL_PROCRESULT]
						== PluginConst.PROC_RESULT_SUCCESS)
					{
						this.blSuccsess_ = true;
						this.strMessage_ = (string)rtnds.Tables[PluginConst.TAB_RESULT].Rows[0][PluginConst.COL_PROCMSG];
					}
					else
					{
						this.blSuccsess_ = false;
						this.strMessage_ = (string)rtnds.Tables[PluginConst.TAB_RESULT].Rows[0][PluginConst.COL_PROCMSG];
					}
				}
				else
				{
					this.blSuccsess_ = true;
					this.strMessage_ = "";

				}


			}
			catch (WebServiceException wEx)
			{
				//Webサービス実行時（通信障害等）エラー

				try
				{
					//エラーログ
					//ここではInnerExceptionを使う
					this.host_.WriteErrorLog(wEx.InnerException, wEx.Message + "HostData#" + GetHostDataKeyValue());
					SendErrorMailAll(wEx.InnerException, wEx);
				}
				catch
				{

				}

			}
			catch (Exception ex)
			{
				//エラーログ
				try
				{
					this.host_.WriteErrorLog(ex, "HostData#" + GetHostDataKeyValue());
					SendErrorMail(ex);
				}
				catch
				{

				}

				this.blSuccsess_ = false;
				try
				{
					this.strMessage_ = (string)rtnds.Tables[PluginConst.TAB_RESULT].Rows[0][PluginConst.COL_PROCMSG]; ;
				}
				catch
				{
					this.strMessage_ = "シリアルNoの検証に失敗しました。";
				}
			}

		}

		/// <summary>
		/// 検証成功可否判断
		/// </summary>
		public bool IsSuccess
		{
			//検証成功可否判断プロパティ
			get { return this.blSuccsess_; }
		}

		/// <summary>
		/// 検証メッセージ(NGの場合のみ)
		/// </summary>
		public string Message
		{
			//検証メッセージプロパティ
			get { return this.strMessage_; }
		}

		#endregion

		#region +OnCompleted 注文確定時
		/// <summary>
		/// 注文確定時
		/// </summary>
		public void OnCompleted()
		{
			//送信データセット
			DataSet sendds = new DataSet();

			//連携戻りデータセット
			DataSet rtnds = null;

			DataTable dt = new DataTable("ConsumedSerialTable");
			dt.Columns.Add("EventID");
			dt.Columns.Add("SerialNo");
			dt.Columns.Add("ProductID");
			dt.Columns.Add("VariationID");
			dt.Columns.Add("ProductCount");
			dt.Columns.Add("OrderID");

			try
			{
				DataTable userdt = new DataTable("UserIDTable");
				userdt.Columns.Add("UserID");
				userdt.Columns.Add("LinkedUserID");
				DataRow userdr = userdt.NewRow();
				userdr["UserID"] = this.host_.Context.Session[PluginConst.PLUGIN_SESSION_KEY_ICID];
				userdr["LinkedUserID"] = this.host_.Context.Session[PluginConst.PLUGIN_SESSION_KEY_W2ID];
				userdt.Rows.Add(userdr);


				//イベント_商品_バリエーションで一位になるリスト
				//複数カートでなぜか購入数は合算できてしまうため
				List<string> eventProductList = new List<string>();

				//注文情報に紐ずく商品情報を取得
				//ここでProductObjectListを検索しているのは、カートリストのみだと注文全ての商品情報が取れてしまうため
				//hostdataからProductObjectList内の商品ID、商品バリエーションを検索
				object objproductObjectList = this.host_.Data[PluginConst.HOST_DATA_KEY_ORDER_PRODUCT_LIST];

				//商品ID、商品バリエーション保持用リスト
				//セミコロン区切りで保持
				List<string> productObjectList = new List<string>();

				//プロダクトリスト分ループ（注文内の商品毎にループ）
				foreach (object productObject in (IEnumerable)objproductObjectList)
				{

					object poPid = ((Hashtable)productObject)["ProductId"].ToString();
					object poVid = ((Hashtable)productObject)["VariationId"].ToString();

					if (productObjectList.Contains(poPid.ToString() + ":" + poVid.ToString()) == false)
					{
						productObjectList.Add(poPid.ToString() + ":" + poVid.ToString());
					}
				}

				//注文ID取得
				string orderid = this.host_.Data[PluginConst.COL_ORDER_ID].ToString();

				//-----------------------------------------------------------------------------------------------------
				//セッションからカートリスト取得
				object ob = this.host_.Context.Session[PluginConst.SESSION_KEY_CART_LIST];
				//セッションからイベントディクショナリ取得
				IDictionary<string, Hashtable> evedic = (IDictionary<string, Hashtable>)this.host_.Context.Session[PluginConst.PLUGIN_SESSION_KEY_RECOMMEND];

				//セッションから商品ID、商品バリエーション、イベントID取出してみる
				foreach (IEnumerable catob in (IEnumerable)ob)
				{
					//catob.GetType().GetMethod(
					foreach (object catval in catob)
					{
						//リフレクション使って商品IDとバリエーションIDとってみるテスト
						object pid = catval.GetType().GetProperty("ProductId").GetValue(catval, null);	//商品ID
						object vid = catval.GetType().GetProperty("VariationId").GetValue(catval, null);	//商品バリエーションID
						object cnt = catval.GetType().GetProperty("Count").GetValue(catval, null);	//購入数

						//ProductOptionSettingList(この中にいべんとIDいれてある）
						object eveids = catval.GetType().GetProperty("ProductOptionSettingList").GetValue(catval, null);

						object eveid = null;
						string streveid = "";

						//productObjectListないの商品ID、バリエーションIDが一致するもののみ
						if (productObjectList.Contains(pid.ToString() + ":" + vid.ToString()) == true)
						{
							//イベントIDが取れている場合のみ
							if (eveids != null)
							{
								//カートオブジェクト内のProductOptionSettingListループ
								foreach (object so in (IEnumerable)eveids)
								{
									//カートオブジェクト内のProductOptionSettingListのValue名
									object valname = so.GetType().GetProperty("ValueName").GetValue(so, null);

									//カートオブジェクト内のProductOptionSettingListのValue名が取れる場合のみ
									if (valname != null)
									{
										//カートオブジェクト内のProductOptionSettingListのValue名がイベントIDの場合
										if ((string)valname == "eventid")
										{
											//カートオブジェクト内のProductOptionSettingListに格納しているイベントIDを取得
											eveid = so.GetType().GetProperty("SettingValues").GetValue(so, null);
											if (eveid != null)
											{
												streveid = ((List<string>)eveid)[0];
												string serial = "";
												if (evedic.Keys.Contains(streveid))
												{
													serial = ConvNullToEmpty(evedic[streveid][PluginConst.COL_SERIALNO]);
												}

												//イベントに該当するシリアルNoがとれたときだけ
												if (serial != "")
												{
													//イベントID_商品ID_バリエーションIDがかぶらないものだけ
													if (eventProductList.Contains(streveid + ":" + pid + ":" + vid) == false)
													{

														DataRow dr = dt.NewRow();
														dr["EventID"] = streveid;
														dr["SerialNo"] = serial;
														dr["ProductID"] = pid;
														dr["OrderID"] = orderid;
														//商品IDとバリエーションIDが同じであれば、バリエーションIDは空にして連携
														if (pid.ToString() == vid.ToString())
														{
															dr["VariationID"] = "";
														}
														else
														{
															dr["VariationID"] = vid;
														}
														dr["ProductCount"] = cnt;
														dt.Rows.Add(dr);

														eventProductList.Add(streveid + ":" + pid + ":" + vid);

													}
												}
											}

										}
									}
								}
							}
						}
					}
				}

				//-----------------------------------------------------------------------------------------------------

				sendds.Tables.Add(dt);
				sendds.Tables.Add(userdt);

				//連携処理コール
				if (dt.Rows.Count > 0)
				{
					// 連携処理呼び出しの前に注文メモにシリアルナンバーセット
					//注文情報の管理メモにシリアルNoをUpdate

					//Updateのパラメタ値をセット
					DataTable orderUpdateParamTable = new DataTable();

					orderUpdateParamTable.Columns.Add(PluginConst.COL_ORDER_ID);
					orderUpdateParamTable.Columns.Add(PluginConst.COL_RELATION_MEMO);
					DataRow orderUpdateParamDr = orderUpdateParamTable.NewRow();

					//注文番号指定
					orderUpdateParamDr[PluginConst.COL_ORDER_ID] = this.host_.Data[PluginConst.HOST_DATA_KEY_ORDER_ID].ToString();

					//シリアルNo指定
					string selNums = "";
					int roopCnt = 0;

					//同じシリアルは重複になるため省くためにリストに入れておく
					List<string> selList = new List<string>();
					//シリアル番号取り出し
					foreach (DataRow selrow in dt.Rows)
					{
						//すでに
						if (selList.Contains(selrow["SerialNo"].ToString()) == false)
						{

							if (roopCnt > 0)
							{
								selNums = selNums + ",";
							}

							selNums = selNums + selrow["SerialNo"].ToString();

							selList.Add(selrow["SerialNo"].ToString());

							roopCnt++;
						}
					}

					orderUpdateParamDr[PluginConst.COL_RELATION_MEMO] = selNums;

					orderUpdateParamTable.Rows.Add(orderUpdateParamDr);

					//Update実行
					CommUtil._DBUtil().ExecuteSql(PluginConst.SQL_SECTION_ID_ORDER_COMP_SERIAL_UPDATE, this.host_, orderUpdateParamTable);

					// テストコード わざとException発生
					// throw new Exception("テスト_____シリアル消しこみのwebサービスエラー想定");

					//連携処理コール
					rtnds = CommUtil._LinkUtil().Execute(PluginConst.PROC_TYPE_SERIAL_DELETE, sendds,
						CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_SENDDATA_LOGDIR));

					// 連携処理が終わったらイベントに該当するシリアルナンバーをクリア
					if (this.host_.Context.Session[PluginConst.PLUGIN_SESSION_KEY_RECOMMEND] != null)
					{
						IDictionary<string, Hashtable> sessEventDic = (IDictionary<string, Hashtable>)this.host_.Context.Session[PluginConst.PLUGIN_SESSION_KEY_RECOMMEND];

						foreach (DataRow evedr in dt.Rows)
						{
							sessEventDic[evedr["EventID"].ToString()][PluginConst.COL_SERIALNO] = "";
						}
					}

					//連携処理結果を取出してプロパティへセット
					if ((string)rtnds.Tables[PluginConst.TAB_RESULT].Rows[0][PluginConst.COL_PROCRESULT]
						== PluginConst.PROC_RESULT_SUCCESS)
					{
						this.blSuccsess_ = true;
						this.strMessage_ = (string)rtnds.Tables[PluginConst.TAB_RESULT].Rows[0][PluginConst.COL_PROCMSG];
					}
					else
					{
						this.blSuccsess_ = false;
						this.strMessage_ = (string)rtnds.Tables[PluginConst.TAB_RESULT].Rows[0][PluginConst.COL_PROCMSG];
					}



				}
				else
				{
					this.blSuccsess_ = true;
					this.strMessage_ = "";
				}


			}
			catch (WebServiceException wEx)
			{
				//Webサービス実行時（通信障害等）エラー

				try
				{
					//エラーログ
					//ここではInnerExceptionを使う
					this.host_.WriteErrorLog(wEx.InnerException, wEx.Message + "HostData#" + GetHostDataKeyValue());
					SendErrorMailAll(wEx.InnerException, wEx);
					CommUtil._FileUtil().WriteErrorCSV(dt, "OnCompleted");
				}
				catch
				{

				}

			}
			catch (Exception ex)
			{
				//エラーログ
				try
				{
					this.host_.WriteErrorLog(ex);
					SendErrorMail(ex);
					CommUtil._FileUtil().WriteErrorCSV(dt, "OnCompleted");
				}
				catch
				{

				}

				this.blSuccsess_ = false;

				try
				{
					this.strMessage_ = (string)rtnds.Tables[PluginConst.TAB_RESULT].Rows[0][PluginConst.COL_PROCMSG]; ;
				}
				catch
				{
					this.strMessage_ = "連携メッセージの取得に失敗しました。";
				}
			}
		}
		#endregion

		#region +OnLoggedIn ログイン時

		/// <summary>
		/// ログイン時
		/// </summary>
		public void OnLoggedIn()
		{
			//送信データセット
			DataSet sendds = new DataSet();


			try
			{

				//データ取得
				DataTable dt = CommUtil._DBUtil().GetDataTable(PluginConst.SQL_SECTION_ID_LOGIN, this.host_, null);
				//平文パスワードの取得
				string nonEncPass = GetNonEncPass(dt);

				//w2用ユーザーID取得
				string w2Userid = ConvNullToEmpty(dt.Rows[0]["LinkedUserID"].ToString());

				//インターコム用ユーザID取得
				string icUserid = ConvNullToEmpty(dt.Rows[0]["UserID"].ToString());

				//セッションにインターコムユーザID入れとく
				if (this.host_.Context.Session != null)
				{
					this.host_.Context.Session.Add(PluginConst.PLUGIN_SESSION_KEY_ICID, icUserid);
				}

				//次画面のURL書き換えのため、ContextからPageオブジェクト取得
				Page pageobj = (Page)this.host_.Context.CurrentHandler;

				if (pageobj != null)
				{

					//ページオブジェクトのVIEWSTATEに格納している次画面URLを書き換え
					foreach (MemberInfo pi in pageobj.GetType().GetMembers(
						BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.InvokeMethod
						))
					{


						if (pi.Name == "get_ViewState")
						{
							StateBag st = (StateBag)pageobj.GetType().InvokeMember("get_ViewState",
							BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.InvokeMethod, null, pageobj, null);

							foreach (string k in st.Keys)
							{

								if (k.ToString() == "NextUrl")
								{

									//NextUrl退避
									string rtnurl = (string)st[k.ToString()];

									//NextUrlにHTTPがついていなかったら(相対パスだったら）つけたる
									if (rtnurl != null)
									{
										if (rtnurl != "")
										{
											if (rtnurl.Length > 4)
											{
												if (rtnurl.Substring(0, 4).ToLower() != "http")
												{
													rtnurl = "https://" + this.host_.Context.Request.Url.Authority + rtnurl;
												}
											}
										}
									}

									////シングルサインオン用に暗号化
									w2Crypt cry = new w2Crypt();

									//セッションからメールメールアドレス取得用
									string mailAdr = "";

									if (this.host_.Data != null)
									{
										if (this.host_.Data.ContainsKey(PluginConst.HOST_DATA_KEY_MAIL_ADR) == true)
										{
											mailAdr = this.host_.Data[PluginConst.HOST_DATA_KEY_MAIL_ADR].ToString();
										}
									}

									//URL生成
									//ログインの場合はmt0をセット
									//クエリストリングはURLエンコードしておく
									string icSSOUrl = //CommUtil._ConfigUtil().GetValue(PluginConst.SETTINF_KEY_IC_SSO_URL_ADD_HEAD) +
										 CommUtil._ConfigUtil().GetValue(PluginConst.SETTINF_KEY_IC_SSO_URL)
										+ "?ml=" + HttpUtility.UrlEncode(cry.Encrypt(mailAdr, CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_ENC)))
										+ "&p=" + HttpUtility.UrlEncode(cry.Encrypt(nonEncPass, CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_ENC)))
										+ "&mt=" + HttpUtility.UrlEncode(cry.Encrypt("0", CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_ENC)))
										+ "&bu=" + HttpUtility.UrlEncode(cry.Encrypt(rtnurl, CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_ENC)));


									st[k.ToString()] = icSSOUrl;
								}
							}
						}
					}
				}

				//データテーブルからパスワード削除
				dt.Columns.Remove("password");
				dt.TableName = PluginConst.TAB_USERID;

				sendds.Tables.Add(dt);

				//レコメンド取得用webサービス呼出
				DataSet rtnDs = CommUtil._LinkUtil().Execute(PluginConst.PROC_TYPE_RECOMMEND_EVENT,
					sendds,
					CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_SENDDATA_LOGDIR));


				//レコメンド用データテーブル取出
				DataTable recommendEventTab = rtnDs.Tables[PluginConst.TAB_RECOMMEND];

				//レコメンドのイベントID毎にディクショナリのKeyにセット
				IDictionary<string, Hashtable> eventdic = new Dictionary<string, Hashtable>();
				foreach (DataRow evedr in recommendEventTab.Rows)
				{
					//シリアル管理フラグを持つハッシュテーブル作成
					Hashtable ht = new Hashtable();
					ht.Add(PluginConst.COL_SERIALFLAG, evedr[PluginConst.COL_SERIALFLAG].ToString());
					//並び順
					ht.Add(PluginConst.COL_DISPORDER, Convert.ToInt32(evedr[PluginConst.COL_DISPORDER]));
					//シリアルNoはEmptyでセット
					ht.Add(PluginConst.COL_SERIALNO, "");
					//商品情報はNullでセット
					ht.Add(PluginConst.COL_EVENT_PRODUCT, null);

					//この時点では商品情報用ハッシュリストはNull入れておく
					eventdic.Add(ConvNullToEmpty(evedr[PluginConst.COL_EVENTID]), ht);
				}

				//レコメンド情報をセッションにセット
				if (this.host_.Context.Session != null)
				{
					this.host_.Context.Session.Add(PluginConst.PLUGIN_SESSION_KEY_RECOMMEND, eventdic);
				}
				//レコメンドフラグをセット(正常に取得できた場合に1セット、イベント一覧画面の判定に利用する)
				if (this.host_.Context.Session != null)
				{
					this.host_.Context.Session.Add(PluginConst.PLUGIN_SESSION_KEY_RECOMMEND_FLAG, "1");
				}

				//シリアル商品をカートに入れていた場合、カート情報を削除
				//会員Noテーブル
				DataTable userDatatable = new DataTable("UseerID");
				userDatatable.Columns.Add("user_id");
				DataRow dr = userDatatable.NewRow();
				dr["user_id"] = w2Userid;
				userDatatable.Rows.Add(dr);

				//カート情報を削除
				CommUtil._DBUtil().ExecuteSql(PluginConst.SQL_SECTION_ID_LOGIN_CART_DELETE, this.host_, userDatatable);


			}
			catch (WebServiceException wEx)
			{
				//Webサービス実行時（通信障害等）エラー

				try
				{
					//エラーログ
					//ここではInnerExceptionを使う
					this.host_.WriteErrorLog(wEx.InnerException, wEx.Message + "HostData#" + GetHostDataKeyValue());
					SendErrorMailAll(wEx.InnerException, wEx);
				}
				catch
				{

				}

			}
			catch (Exception ex)
			{
				try
				{
					//エラーログ
					this.host_.WriteErrorLog(ex, "HostData#" + GetHostDataKeyValue());
					SendErrorMail(ex);
				}
				catch
				{

				}
			}
		}

		#endregion

		#region +OnModified 会員変更時

		/// <summary>
		/// 会員変更時（インターコムの場合は、ゲスト購入後の続けて会員登録のながれのみよばれる）
		/// </summary>
		public void OnModified()
		{
			//ゲスト購入の後に続けて会員登録をした場合は会員変更のプラグインが
			//起動するので、この場合は新規会員登録して取り扱いインターコムと連携する

			DataSet ds = new DataSet();
			DataTable dt = null;

			//会員登録の際に実行
			try
			{
				//平文パスワードの取得
				string nonEncPass = GetNonEncPass();

				//パスワードが取れない場合は配送情報の変更のばあいなので
				//処理はしない
				if (nonEncPass == null)
				{
					return;
				}

				if (nonEncPass == "")
				{
					return;
				}

				//インターコムIDを取得
				DataTable iddt = CommUtil._DBUtil().GetDataTable(PluginConst.SQL_SECTION_ID_LOGIN, this.host_, null);
				string icid = "";
				//インターコムIDに何か値が入っている場合は新規登録しているので後続の処理は行わない
				if (iddt.Rows.Count > 0)
				{
					icid = iddt.Rows[0]["UserID"].ToString();
				}
				if (icid != "")
				{
					return;
				}


				//データ取得
				dt = CommUtil._DBUtil().GetDataTable(PluginConst.SQL_SECTION_ID_USER_REGIST, this.host_, null);
				//テーブル名セット
				dt.TableName = PluginConst.TAB_USER;
				ds.Tables.Add(dt);

				//パスワードを平文でセット
				if (dt.Rows.Count > 0)
				{
					dt.Rows[0]["password"] = nonEncPass;
				}
				else
				{
					//取得データが0件の場合はException
					throw new Exception("登録対象の会員情報データが取得できませんでした。");
				}

				//連携処理コール
				DataSet rtnds = CommUtil._LinkUtil().Execute(PluginConst.PROC_TYPE_USER_REGIST,
					ds,
					CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_SENDDATA_LOGDIR));

				//Intercom採番会員Noテーブル
				DataTable userDatatable = rtnds.Tables[PluginConst.TAB_USERID];

				//Intercom採番会員NoをUpdate
				CommUtil._DBUtil().ExecuteSql(PluginConst.SQL_SECTION_ID_USER_REGIST_AFTER_UPDATE, this.host_, userDatatable);

			}
			catch (WebServiceException wEx)
			{
				//Webサービス実行時（通信障害等）エラー

				try
				{
					//エラーログ
					//ここではInnerExceptionを使う
					this.host_.WriteErrorLog(wEx.InnerException, wEx.Message + "HostData#" + GetHostDataKeyValue());
					SendErrorMailAll(wEx.InnerException, wEx);
					CommUtil._FileUtil().WriteErrorCSV(dt, "OnRegistered");
				}
				catch
				{

				}

			}
			catch (Exception ex)
			{
				try
				{
					//エラーログ
					this.host_.WriteErrorLog(ex, "HostData#" + GetHostDataKeyValue());
					SendErrorMail(ex);
					CommUtil._FileUtil().WriteErrorCSV(dt, "OnRegistered");
				}
				catch
				{

				}

			}
			finally
			{

			}

		}

		#endregion

		/************************************************************************************/

		#region ヘルパメソッド群

		#region -SendErrorMail エラーメール送信
		/// <summary>
		/// エラーメール送信
		/// </summary>
		/// <param name="ex">発生Exception</param>
		private void SendErrorMail(Exception ex)
		{
			//, "HostData#" + GetHostDataKeyValue()
			//本文作成
			string bodyStr = CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_ERROR_BODYTEMPLATE);
			bodyStr = bodyStr.Replace("{0}", ex.Message);
			bodyStr = bodyStr.Replace("{1}", ex.StackTrace);
			bodyStr = bodyStr.Replace("{2}", GetHostDataKeyValue());


			this.host_.SendMail(CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_ERROR_SUBJECT),
					bodyStr,
					CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_ERROR_FROM),
					GetAddresList(PluginConst.SETTING_KEY_ERROR_TOLIST),
					GetAddresList(PluginConst.SETTING_KEY_ERROR_CCLIST),
					GetAddresList(PluginConst.SETTING_KEY_ERROR_BCCLIST));

		}
		#endregion

		#region -SendErrorMailAll 全体向けエラーメール送信
		/// <summary>
		/// 全体向けエラーメール送信
		/// </summary>
		/// <param name="ex">発生Exception</param>
		private void SendErrorMailAll(Exception ex, WebServiceException wEx)
		{
			string bodyStr = CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_ERROR_BODYTEMPLATE_ALL);
			bodyStr = bodyStr.Replace("{0}", wEx.Message);
			bodyStr = bodyStr.Replace("{1}", ex.Message);
			bodyStr = bodyStr.Replace("{2}", ex.StackTrace);


			this.host_.SendMail(CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_ERROR_SUBJECT_ALL),
					bodyStr,
					CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_ERROR_FROM_ALL),
					GetAddresList(PluginConst.SETTING_KEY_ERROR_TOLIST_ALL),
					GetAddresList(PluginConst.SETTING_KEY_ERROR_CCLIST_ALL),
					GetAddresList(PluginConst.SETTING_KEY_ERROR_BCCLIST_ALL));
		}

		#endregion

		#region -GetHostDataKeyValue エラーログ出力用受領データログ文字取得

		/// <summary>
		/// エラーログ出力用のHostData内容文字列
		/// </summary>
		/// <returns></returns>
		private string GetHostDataKeyValue()
		{
			StringBuilder strBuilder = new StringBuilder();

			strBuilder.Append("");

			try
			{

				if (this.host_.Data != null)
				{
					foreach (string key in this.host_.Data.Keys)
					{
						if (this.host_.Data[key] == null)
						{
							strBuilder.Append(key + ":" + "【Null値】" + ";");
						}
						else
						{
							strBuilder.Append(key + ":" + "【" + this.host_.Data[key] + "】" + ";");
						}
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

		#region -GetNonEncPass 平文パスワード取得(データテーブルから探して複合化・取得)
		private string GetNonEncPass(string value)
		{
			string pass = value;
			try
			{
				// ラインダールを設定
				RijndaelManaged rijndael = new RijndaelManaged();
				rijndael.Key = Convert.FromBase64String(CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_KEY));
				rijndael.IV = Convert.FromBase64String(CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_W2_IV));

				Encoding utf8 = Encoding.UTF8;
				byte[] encryptedSource = Transform(Convert.FromBase64String(pass), rijndael.CreateDecryptor());
				return utf8.GetString(encryptedSource);
			}
			catch (Exception ex)
			{

			}

			return pass;
		}
		private string GetNonEncPass(DataTable dt)
		{
			return GetNonEncPass((string)dt.Rows[0]["password"]);
		}
		/// <summary>
		/// 暗号化・複合化処理
		/// </summary>
		/// <param name="source">対象データ</param>
		/// <param name="transform">暗号変換情報</param>
		/// <returns>変換後情報</returns>
		private byte[] Transform(byte[] source, ICryptoTransform transform)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
				{
					cs.Write(source, 0, source.Length);
				}
				return ms.ToArray(); // CryptStreamを閉じてからでないとbyte配列が途中で途切れる可能性がある
			}
		}

		/// <summary>
		/// 暗号化されていないパスワードをセッションKeyparam_datani格納されているvalueobjectから取得
		/// 取得できない場合はから文字
		/// </summary>
		private string GetNonEncPass()
		{
			return GetNonEncPass((string)this.host_.Data["password"] ?? string.Empty);
		}

		#endregion

		#region -GetAddresList メール送信用アドレスリスト取得
		/// <summary>
		/// メール送信用アドレスリスト取得
		/// </summary>
		/// <param name="adrConfName">取得するアドレスのコンフィグKey</param>
		/// <returns>リスト形式でメールアドレスを返却</returns>
		private List<string> GetAddresList(string adrConfName)
		{
			//アドレス区切り文字
			string adrSplitStr = CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_MAIL_SPLIT_STR);

			List<string> adrList = new List<string>();

			foreach (string adr in CommUtil._ConfigUtil().GetValue(adrConfName).Split(adrSplitStr.ToCharArray()))
			{
				adrList.Add(adr);
			}

			return adrList;
		}
		#endregion

		#region -ConvNullToEmpty NullとDBぬるをString.Emptyに置換

		/// <summary>
		/// NullとDBぬるをString.Emptyに置換
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		private string ConvNullToEmpty(object target)
		{
			if (target == null)
			{
				return "";
			}

			if (target == DBNull.Value)
			{
				return "";
			}

			return target.ToString();
		}

		#endregion

		#endregion


	}
}
