/*
=========================================================================================================
  Module      : ＦＴＰファイルアップロード (FtpFileUpload.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Net;
using System.Text;
using w2.Common.Logger;
using w2.Commerce.MallBatch.StockUpdate.Mall;
using w2.SFTPClientWrapper;
using WinSCP;

namespace w2.Commerce.MallBatch.StockUpdate.Ftp
{
	///**************************************************************************************
	/// <summary>
	/// ＦＴＰファイルアップロード用クラス
	/// </summary>
	///**************************************************************************************
	class FtpFileUpload
	{
		/// <summary>
		/// ファイル存在チェック（楽天専用）
		/// </summary>
		/// <param name="mallConfigTip">モール設定</param>
		/// <returns>ファイル存在時、true</returns>
		public static bool CheckRakutenRemoteFileExist(MallConfigTip mallConfigTip)
		{
			//------------------------------------------------------
			// 楽天側のファイル存在チェック
			//------------------------------------------------------
			// 商品情報ファイル存在チェック
			if (CheckRemoteFileExist(mallConfigTip, "item.csv"))
			{
				return true;
			}

			// 在庫情報ファイル存在チェック
			if (CheckRemoteFileExist(mallConfigTip, "select.csv"))
			{
				return true;
			}

			// 楽天カテゴリ情報ファイル存在チェック
			if (CheckRemoteFileExist(mallConfigTip, "item-cat.csv"))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// ファイル存在チェック
		/// </summary>
		/// <param name="mallConfigTip">モール設定</param>
		/// <param name="strFileName">ファイル名</param>
		/// <returns>ファイル存在時、true</returns>
		/// <remarks>強制的にエラーを発生させ、エラー内容によりチェックする</remarks>
		private static bool CheckRemoteFileExist(MallConfigTip mallConfigTip, string strFileName)
		{
			// SFTPの場合
			if (mallConfigTip.IsSftp)
			{
				try
				{
					var sftpManager = new SFTPManager(mallConfigTip.SFTPSettings);
					// ファイルがあったらok
					if (sftpManager.CreateSFTPClient().IsExistsServerFile(
							Path.Combine("./", mallConfigTip.FtpUploadDir, strFileName)))
					{
						return true;
					}

					// ファイルがない場合false
					return false;
				}
				catch (Exception ex)
				{
					// ファイルが存在しない以外のエラーの場合、アップロードを待機しておく
					FileLogger.WriteWarn("モールID[" + mallConfigTip.MallId + "] アップロードを待機します（SFTP通信エラー）：「" + ex.Message + "」");
					return true;
				}
			}
			// FTPの場合
			try
			{
				// ダウンロードに成功した場合、FTP先にファイルが存在していると判断する
				using (WebClient webClient = new WebClient())
				{
					webClient.Credentials = new NetworkCredential(mallConfigTip.FtpUserName, mallConfigTip.FtpPassWord);
					webClient.DownloadFile(mallConfigTip.PathFtpUpload + strFileName, Constants.TMP_SAVE_TO + @"\RemoteFileTmp");
				}

				DeleteDownLoadFile(mallConfigTip.MallId, strFileName);
				return true;
			}
			catch (WebException we)
			{
				using (FtpWebResponse ftpWebResponse = (FtpWebResponse)we.Response)
				{
					if ((ftpWebResponse != null) && (ftpWebResponse.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable))
					{
						// ファイルが存在しない以外のエラーの場合、アップロードを待機しておく
						FileLogger.WriteWarn("モールID[" + mallConfigTip.MallId + "] アップロードを待機します（FTP通信エラー）：「" + we.Message + "」");
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// 一時的なダウンロードファイルの削除
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="fileName">ファイル名</param>
		private static void DeleteDownLoadFile(string mallId, string fileName)
		{
			File.Delete(Constants.TMP_SAVE_TO + @"\RemoteFileTmp");
			FileLogger.WriteWarn("モールID[" + mallId + "] アップロードを待機します（ダウンロードに成功）：アップロード先にファイル「" + fileName + "」が存在しています。");
		}

		/// <summary>
		/// 外部プロセスからのリダイレクト受信
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void ProcessDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
		{
			Console.WriteLine(e.Data);
		}

		/// <summary>
		/// アップロードファイルのデータ件数整合性チェック
		/// </summary>
		/// <param name="strMkadvOutput">チェック対象ファイルのパス</param>
		/// <param name="iCount">更新対象元データ件数</param>
		/// <returns>2行以上かつ、更新対象元データ件数と同一：true　それ以外：false</returns>
		public static bool CheckCsvCount(string strMkadvOutput, int iCount)
		{
			//------------------------------------------------------
			// 出力レコード件数チェック
			//------------------------------------------------------
			StreamReader streamReader = new StreamReader(strMkadvOutput, Encoding.GetEncoding(932)); // Sihft_JISで読み込み
			int iCsvRowCount = 0;

			// 行数を計算取得する
			while (streamReader.Peek() != -1)
			{
				iCsvRowCount++;

				// フィールドに改行が含まれる場合を考慮して行を結合（CSVの行に改行がある場合、「"」は奇数個のはず）
				StringBuilder sbLineBuffer = new StringBuilder(streamReader.ReadLine());
				while (((sbLineBuffer.Length - sbLineBuffer.Replace("\"", "").Length) % 2) != 0)
				{
					sbLineBuffer.Append("\r\n").Append(streamReader.ReadLine());
				}
			}

			// 2行以上 かつ 更新対象元データ件数と同一 の場合、trueを返却する
			return (iCsvRowCount >= 2) && (iCsvRowCount == iCount + 1);
		}

		/// <summary>
		/// アップロードファイルのデータ件数整合性チェック
		/// </summary>
		/// <param name="mkadvOutput">チェック対象ファイルのパス</param>
		/// <returns>2行以上：true　それ以外：false）</returns>
		public static bool CheckCsvCount(string strMkadvOutput)
		{
			//------------------------------------------------------
			// 出力レコード件数チェック
			//------------------------------------------------------
			string strFile = File.ReadAllText(strMkadvOutput, Encoding.GetEncoding(932)); // Sihft_JISで読み込み

			// 2行以上の場合、trueを返却する
			return (strFile.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length >= 2);
		}

		/// <summary>
		/// FTPアップロード
		/// </summary>
		/// <param name="strUri">アップロード先Uri</param>
		/// <param name="strFileName">アップロードファイル名</param>
		/// <param name="strUserName">FTPログインユーザ名</param>
		/// <param name="strPassword">FTPログインパスワード</param>
		/// <param name="blUsePassive">PASVモードON</param>
		/// <returns>アップロード成功時true</returns>
		public static bool FtpUpload(
			string strUri, 
			string strFileName, 
			string strUserName, 
			string strPassword, 
			bool blUsePassive)
		{
			// アップロードするファイル
			string strUploadFileName = strFileName;

			// アップロード先のURI
			Uri uri = new Uri(strUri);

			//------------------------------------------------------
			// FTPアップロード設定
			//------------------------------------------------------
			// FtpWebRequestの作成
			FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(uri);

			// ログインユーザー名とパスワードを設定
			ftpWebRequest.Credentials = new NetworkCredential(strUserName, strPassword);

			// MethodにWebRequestMethods.Ftp.UploadFile("ファイル名")を設定
			ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;

			// 要求の完了後に接続を閉じる
			ftpWebRequest.KeepAlive = false;

			// ASCIIモードで転送する
			ftpWebRequest.UseBinary = false;

			// PASVモードを無効にする
			ftpWebRequest.UsePassive = blUsePassive;

			//------------------------------------------------------
			// FTPアップロード
			//------------------------------------------------------
			using (Stream stream = ftpWebRequest.GetRequestStream())
			using (FileStream fileStream = new FileStream(strUploadFileName, FileMode.Open, FileAccess.Read))
			{
				byte[] bBuffers = new byte[1024];
				int iReadSize = 0;
				while ((iReadSize = fileStream.Read(bBuffers, 0, bBuffers.Length)) != 0)
				{
					stream.Write(bBuffers, 0, iReadSize);
				}
			}

			//------------------------------------------------------
			// FTPアップロード完了チェック
			//------------------------------------------------------
			using (FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse())
			{
				// FTPサーバーから送信されたステータスを表示
				if (ftpWebResponse.StatusCode == FtpStatusCode.ClosingData)
				{
					FileLogger.WriteInfo("FTPアップロードが成功しました。");
					return true;
				}
				else
				{
					FileLogger.WriteError("FTPアップロードに失敗しました。ステータスコード：" + ftpWebResponse.StatusCode);
					FileLogger.WriteError(ftpWebResponse.StatusDescription);
				}
			}

			return false;
		}
	}
}
