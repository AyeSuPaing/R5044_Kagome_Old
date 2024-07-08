/*
=========================================================================================================
  Module      : メール注文取得／バッチ制御 (Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using w2.Common.Sql;
using w2.Common.Logger;
using w2.App.Common;

namespace w2.Commerce.MallBatch.MailOrder
{
	/// <summary>
	/// メール注文取得・コントロール
	/// </summary>
	class Program
	{
		private static string m_MailOrder_Path_SaveTo = null;
		private static string m_Program_MailOrderGetter = null;
		private static string m_Program_Popget = null;

		/// <summary>
		/// メールを受信し、注文情報をＤＢに投入します
		/// </summary>
		/// <param name="args">
		/// </param>
		static void Main(string[] args)
		{
			try
			{
				//------------------------------------------------------
				// アプリケーション設定読み込み
				//------------------------------------------------------
				// アプリケーション名設定
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				// アプリケーション共通の設定			
				ConfigurationSetting csSetting
					= new ConfigurationSetting(
						Properties.Settings.Default.ConfigFileDirPath,
						ConfigurationSetting.ReadKbn.C000_AppCommon,
						ConfigurationSetting.ReadKbn.C100_BatchCommon,
						ConfigurationSetting.ReadKbn.C300_MailOrder);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				m_MailOrder_Path_SaveTo = csSetting.GetAppStringSetting("MailOrder_Path_SaveTo");
				m_Program_MailOrderGetter = csSetting.GetAppStringSetting("Program_MailOrderGetter");
				m_Program_Popget = csSetting.GetAppStringSetting("Program_Popget");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}

			//========================================
			// 店舗ＩＤ、モールＩＤを取得
			//========================================
			MallIdInfo mii = new MallIdInfo();

			foreach (MallInfo miMallInfo in mii.m_lMallInfos)
			{
				FileLogger.WriteInfo("店舗ID:" + miMallInfo.strShopId + "  モールID:" + miMallInfo.strMallId);

				try
				{
					//========================================
					// 初期化
					//========================================
					FileLogger.WriteInfo("初期化中...");

					// 取得する
					string strShopId = miMallInfo.strShopId;
					string strMallId = miMallInfo.strMallId;

					string strMailSaveDir = m_MailOrder_Path_SaveTo + @"\" + strShopId + @"\" + strMallId;

					// 保存先ディレクトリパスを構築
					if (!Directory.Exists(strMailSaveDir + @"\Get"))
					{
						Directory.CreateDirectory(strMailSaveDir + @"\Get");
					}
					if (!Directory.Exists(strMailSaveDir + @"\Active"))
					{
						Directory.CreateDirectory(strMailSaveDir + @"\Active");
					}
					if (!Directory.Exists(strMailSaveDir + @"\Error"))
					{
						Directory.CreateDirectory(strMailSaveDir + @"\Error");
					}
					if (!Directory.Exists(strMailSaveDir + @"\StockError"))
					{
						Directory.CreateDirectory(strMailSaveDir + @"\StockError");
					}
					if (!Directory.Exists(strMailSaveDir + @"\Success"))
					{
						Directory.CreateDirectory(strMailSaveDir + @"\Success");
					}
					FileLogger.WriteInfo("完了.");

					//========================================
					// ＤＢから受信設定を取り込む
					//========================================
					FileLogger.WriteInfo("ＤＢから受信設定を取り込む...");

					MallCooperationSetting mcs = new MallCooperationSetting(strShopId, strMallId);

					FileLogger.WriteInfo("完了.");

					//========================================
					// メール受信
					//========================================
					FileLogger.WriteInfo("メール受信...");

					popget(mcs.pop_server,
							mcs.pop_port,
							mcs.pop_user_name,
							mcs.pop_password,
							true,
							strMailSaveDir + @"\Get\");

					FileLogger.WriteInfo("完了.");

					//========================================
					// Ａｃｔｉｖｅフォルダに移動
					//========================================
					FileLogger.WriteInfo("Ａｃｔｉｖｅフォルダに移動...");

					string folder_Get = strMailSaveDir + @"\Get\";
					string folder_Active = strMailSaveDir + @"\Active";
					string[] getFolderFiles = Directory.GetFiles(folder_Get);
					foreach (string strMoveFrom in getFolderFiles)
					{
						File.Move(strMoveFrom,
							folder_Active + @"\" + Path.GetFileName(strMoveFrom));
					}

					FileLogger.WriteInfo("完了.");

					//========================================
					// 受注データ取込処理を実行
					//========================================
					FileLogger.WriteInfo("受注データ取込処理を実行...");

					MailOrderGetter(strMailSaveDir, strShopId, strMallId);
					
					FileLogger.WriteInfo("完了.");
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(ex);
				}
			}
		}

		/// <summary>
		/// popgetコマンドを呼び出し
		/// </summary>
		/// <param name="server">サーバホスト名</param>
		/// <param name="port">ポート番号</param>
		/// <param name="user_id">POPユーザＩＤ</param>
		/// <param name="password">POPパスワード</param>
		/// <param name="del_flg">削除フラグ：true時リモートのメールを削除</param>
		/// <param name="saveto">保存先パス</param>
		static void popget(string server,
							int port,
							string user_id,
							string password,
							bool del_flg,
							string saveto)
		{
			string path_exe = m_Program_Popget;
			System.Diagnostics.ProcessStartInfo psInfo = new System.Diagnostics.ProcessStartInfo();
			psInfo.FileName = path_exe; // 実行するファイル
			psInfo.CreateNoWindow = true; // コンソール・ウィンドウを開かない
			psInfo.UseShellExecute = false; // シェル機能を使用しない

			/// [0] POP3サーバー名
			/// [1] POP3サーバーのポート番号
			/// [2] ユーザーID
			/// [3] パスワード
			/// [4] メールを削除するか( -d なら削除する ／ 削除しない場合は-nとでもしておく )
			/// [5] 保存先ディレクト
			psInfo.Arguments = server
					   + " " + port.ToString()
					   + " " + user_id
					   + " " + password
					   + " " + (del_flg ? "-d" : "-n")
					   + " " + saveto;

			//                System.Diagnostics.Process process = System.Diagnostics.Process.Start(psInfo);
			psInfo.RedirectStandardOutput = true; // 標準出力をリダイレクトする
			psInfo.RedirectStandardInput = true; // 標準入力を　　　〃
			psInfo.RedirectStandardError = true; // 標準エラー出力を　　　〃
			System.Diagnostics.Process process = System.Diagnostics.Process.Start(psInfo);
			process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_DataReceived);
			process.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_DataReceived);
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.WaitForExit();
		}

		/// <summary>
		/// メールの注文データをＤＢに取り込みます
		/// </summary>
		/// <param name="strMailPath">取込メールパス</param>
		/// <param name="strShopId">店舗ＩＤ</param>
		/// <param name="strMallId">モールID</param>
		static void MailOrderGetter(string strMailPath, string strShopId, string strMallId)
		{
			string path_exe = m_Program_MailOrderGetter;
			System.Diagnostics.ProcessStartInfo psInfo = new System.Diagnostics.ProcessStartInfo();
			psInfo.FileName = path_exe; // 実行するファイル
			psInfo.CreateNoWindow = true; // コンソール・ウィンドウを開かない
			psInfo.UseShellExecute = false; // シェル機能を使用しない
			psInfo.Arguments = strMailPath + " " + strShopId + " " + strMallId;
			psInfo.RedirectStandardOutput = true; // 標準出力をリダイレクトする
			psInfo.RedirectStandardInput = true; // 標準入力を　　　〃
			psInfo.RedirectStandardError = true; // 標準エラー出力を　　　〃
			System.Diagnostics.Process process = System.Diagnostics.Process.Start(psInfo);
			process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_DataReceived);
			process.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(process_DataReceived);
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.WaitForExit();
		}

		/// <summary>
		/// 外部プロセスからのリダイレクト受信
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="outLine"></param>
		static void process_DataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
		{
			if (!String.IsNullOrEmpty(e.Data))
			{
				FileLogger.WriteInfo(e.Data);
			}
		}
	}

	///*********************************************************************************************
	/// <summary>
	/// モールIDクラス
	/// </summary>
	///*********************************************************************************************
	public class MallIdInfo
	{
		/// <summary>
		/// モール設定のＩＤを一覧作成します
		/// </summary>
		public MallIdInfo()
		{
			DataView dvMallCooperationSettings = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MallCooperationSetting", "GetMallCooperationSettings"))
			{
				dvMallCooperationSettings = sqlStatement.SelectSingleStatementWithOC(sqlAccessor);
			}

			foreach (DataRowView drv in dvMallCooperationSettings)
			{
				m_lMallInfos.Add(new MallInfo((string)drv[Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID], (string)drv[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID]));
			}
		}

		/// <summary>
		/// モール情報
		/// </summary>
		public List<MallInfo> m_lMallInfos = new List<MallInfo>();
	}

	///*********************************************************************************************
	/// <summary>
	/// モール情報クラス
	/// </summary>
	///*********************************************************************************************
	public class MallInfo
	{
		string m_strstrShopId = null;
		string m_strstrMallId = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strstrShopId"></param>
		/// <param name="strstrMallId"></param>
		public MallInfo(string strstrShopId, string strstrMallId)
		{
			m_strstrShopId = strstrShopId;
			m_strstrMallId = strstrMallId;
		}

		public string strShopId
		{
			get { return m_strstrShopId; }
		}
		public string strMallId
		{
			get { return m_strstrMallId; }
		}
	}
}
