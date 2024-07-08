/*
=========================================================================================================
  Module      : Ftpsアップロード用クラス(FtpsUploadUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Logger;

namespace w2.Commerce.Batch.CustomerRingsExport.Util
{
	public class FtpsUploadUtility
	{
		/// <summary>
		/// アップロード実行
		/// </summary>
		/// <param name="targetUrl">アップロード先Url</param>
		/// <param name="fileName">ファイル名</param>
		/// <param name="userName">ユーザー名</param>
		/// <param name="password">パスワード</param>
		/// <returns>成功/失敗</returns>
		public bool ExecUpload(string targetUrl, string fileName, string userName, string password)
		{
			var uri = new Uri(targetUrl);
			var ftpWebRequest = (FtpWebRequest)WebRequest.Create(uri);
			ftpWebRequest.Credentials = new NetworkCredential(userName, password);
			ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;
			ftpWebRequest.KeepAlive = false; // 接続を閉じる
			//ftpWebRequest.UseBinary = true; // バイナリモード
			ftpWebRequest.UsePassive = Constants.USE_PASSIVE; //
			ftpWebRequest.EnableSsl = true; // SSL暗号化

			// 自己証明を回避するためのコールバック登録
			if (Constants.CHECK_SSL == false)
			{
				ServicePointManager.ServerCertificateValidationCallback = OnRemoteCertificateValidationCallback;
			}

			try
			{
				// FTPアップロード
				using (var stream = ftpWebRequest.GetRequestStream())
				using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					byte[] buffer = new byte[1024];
					int readSize = 0;
					while ((readSize = fileStream.Read(buffer, 0, buffer.Length)) != 0)
					{
						stream.Write(buffer, 0, readSize);
					}
				}

				// 完了チェック
				using (var ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse())
				{
					// FTPサーバーから送信されたステータスを表示
					if (ftpWebResponse.StatusCode != FtpStatusCode.ClosingData)
					{
						throw new Exception("FTPSアップロードに失敗しました。ステータスコード：" + ftpWebResponse.StatusCode + "\r\n" + ftpWebResponse.StatusDescription);
					}
				}
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
				return false;
			}
			return true;
		}

		/// <summary>
		/// 自己証明許可のためのコールバック用
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="certificate"></param>
		/// <param name="chain"></param>
		/// <param name="sslPolicyErrors"></param>
		/// <returns></returns>
		private static bool OnRemoteCertificateValidationCallback(
			Object sender,
			X509Certificate certificate,
			X509Chain chain,
			SslPolicyErrors sslPolicyErrors)
		{
			// 信用したことにする
			return true;
		}
	}
}
