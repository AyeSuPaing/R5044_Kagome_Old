/*
=========================================================================================================
  Module      : POP3クライアントモジュール(Pop3Client.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.Common.Net.Mail
{
	///**************************************************************************************
	/// <summary>
	/// POP3接続してメールを受信する
	/// </summary>
	///**************************************************************************************
	public class Pop3Client : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Pop3Client()
			: this(Constants.SERVER_POP, Constants.SERVER_POP_PORT, Constants.SERVER_POP_AUTH_USER_NAME, Constants.SERVER_POP_AUTH_PASSOWRD, Constants.SERVER_POP_TYPE)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="setting">POPアカウント情報</param>
		public Pop3Client(PopAccountSetting setting)
		{
			this.PopSetting = setting;
			this.Sslversions = (SslProtocols)0xFF0;	// TLSv1.2 - SSLv3.0
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="popServer">サーバ名</param>
		/// <param name="popPort">ポート番号</param>
		/// <param name="popUserId">ユーザ名</param>
		/// <param name="popPassword">パスワード</param>
		/// <param name="popType">POPタイプ</param>
		public Pop3Client(string popServer, int popPort, string popUserId, string popPassword, PopType popType)
			: this(new PopAccountSetting(popServer, popPort, popUserId, popPassword, popType))
		{
		}

		/// <summary>
		/// 接続
		/// </summary>
		public void Connect()
		{
			//------------------------------------------------------
			// 接続＆設定
			//------------------------------------------------------
			// TCP接続
			this.TcpClientObject = new TcpClient();
			this.TcpClientObject.Connect(this.PopSetting.Server, this.PopSetting.Port);

			// TCPストリーム取得
			var tcpStream = this.TcpClientObject.GetStream();
			tcpStream.ReadTimeout = Constants.MAILRECEIVE_NETWORK_STREAM_READ_TIMEOUT;
			tcpStream.WriteTimeout = Constants.MAILRECEIVE_NETWORK_STREAM_WRITE_TIMEOUT;

			if (this.PopSetting.Port == 995)
			{
				// POP3SポートだったらSSL昇格
				var sslStream = new SslStream(tcpStream, false,
					(c, e, r, t) => true,	// サーバー証明書は検証しない（ほとんどの場合一致しない）
					null);

				sslStream.AuthenticateAsClient(this.PopSetting.Server, null, this.Sslversions, false);
				this.RWStream = sslStream;
			}
			else
			{
				this.RWStream = tcpStream;
			}

			//------------------------------------------------------
			// サーバから挨拶行取得
			//------------------------------------------------------
			var line = ReceiveLine();
			if (IsOK(line) == false)
			{
				throw new InvalidDataException(string.Format("サーバからの応答コードが不正です。:{0}", line));
			}

			//------------------------------------------------------
			// 認証
			//------------------------------------------------------
			// APOP？
			if (this.PopSetting.PopType == PopType.Apop)
			{
				// APOPの場合は「<」と「>」の中身が必要なので取り出す
				string[] tmp = line.Split(' ');
				ApopAuth(this.PopSetting.UserId, this.PopSetting.Password, tmp[tmp.Length - 1]);
			}
			// POP？
			else
			{
				PopAuth(this.PopSetting.UserId, this.PopSetting.Password);
			}
		}

		/// <summary>
		/// APOP認証
		/// </summary>
		/// <param name="strUserName">ユーザ名</param>
		/// <param name="strPassword">パスワード</param>
		/// <param name="strChallenge">POP3サーバーの応答の1行目から得られるチャレンジ文字列</param>
		private void ApopAuth(string strUserName, string strPassword, string strChallenge)
		{
			// パスワードを結合
			string strPlain = strChallenge + strPassword;

			// MD5に変換
			MD5 md5 = MD5.Create();
			byte[] bDatas = md5.ComputeHash(Encoding.ASCII.GetBytes(strPlain));

			// 16進数小文字の文字列にする
			StringBuilder md5response = new StringBuilder();
			foreach (byte bData in bDatas)
			{
				md5response.Append(bData.ToString("x2"));
			}

			// APOPコマンド送信
			SendLine(string.Format("APOP {0} {1}", strUserName, md5response.ToString()));

			var line = ReceiveLine();
			if (IsOK(line) == false)
			{
				throw new SecurityException(string.Format("認証に失敗しました:{0}", line));
			}
		}

		/// <summary>
		/// POP認証
		/// </summary>
		/// <param name="strUserName">認証ユーザー名</param>
		/// <param name="strPassword">認証ユーザーパスワード</param>
		private void PopAuth(string strUserName, string strPassword)
		{
			// USERコマンド
			SendLine(string.Format("USER {0}", strUserName));
			var line = ReceiveLine();
			if (IsOK(line) == false)
			{
				throw new SecurityException(string.Format("認証に失敗しました:{0}", line));
			}

			// PASSコマンド
			SendLine(string.Format("PASS {0}", strPassword));
			line = ReceiveLine();
			if (IsOK(line) == false)
			{
				throw new SecurityException(string.Format("認証に失敗しました:{0}", line));
			}
		}

		/// <summary>
		/// メッセージ取得
		/// </summary>
		/// <param name="blIsDelete">削除可否</param>
		/// <returns>メールメッセージ</returns>
		public IList<Pop3Message> GetMessages(bool blIsDelete)
		{
			try
			{
				var list = GetAndDeleteMessages(blIsDelete ? DeleteMode.Always : DeleteMode.NoDelete);
				return list;
			}
			catch (Exception ex)
			{
				MailSendDueToOccurredException();
				// ログファイル出力
				FileLogger.Write("PopError", "", ex);
				throw;
			}
		}

		/// <summary>
		/// 例外発生時のメール通知
		/// </summary>
		/// <param name="gmailAccount">Gmail Account</param>
		protected void MailSendDueToOccurredException(string gmailAccount = null)
		{
			// 例外が発生したらメール通知
			if (Constants.MAIL_RECV_ERROR_MAILADDR_TO.Length != 0)
			{
				using (var sender = new SmtpMailSender())
				{
					sender.SetFrom(Constants.MAIL_RECV_ERROR_MAILADDR_FROM);
					Constants.MAIL_RECV_ERROR_MAILADDR_TO.ToList().ForEach(sender.AddTo);
					sender.SetSubject(Constants.APPLICATION_NAME_WITH_PROJECT_NO + "：メール受信に失敗しました");
					sender.SetBody(string.Format(
						"対象メールアドレス：{0}",
						gmailAccount ?? this.PopSetting.UserId));

					sender.SendMail();
				}
			}
		}

		/// <summary>
		/// メッセージ削除
		/// </summary>
		/// <param name="deleteList">削除リスト</param>
		public void DeleteMessages(HashSet<Pop3Message> deleteList)
		{
			if (deleteList == null) throw new Exception();
			GetAndDeleteMessages(DeleteMode.OnList, deleteList);
		}

		/// <summary>
		/// メッセージ取得＆削除
		/// </summary>
		/// <param name="delMode">削除モード</param>
		/// <param name="delList">削除対象リスト（DeleteOnList時に指定）</param>
		/// <remarks>DeleteOnListモードで削除対象リストが指定されないときは DeleteAlways 扱いとなります。</remarks>
		/// <returns>取得メッセージリスト</returns>
		private Pop3Message[] GetAndDeleteMessages(DeleteMode delMode, HashSet<Pop3Message> delList = null)
		{
			var mailList = GetMailLists();

			// メール数だけループ処理
			var messageList = new List<Pop3Message>();
			foreach (int mailKey in mailList.Keys)
			{
				var headerBody = GetMailBody(mailKey);
				if (headerBody == null) continue;

				var message = GetDecodedMail(headerBody);
				// デコードエラー
				if (message == null)
				{
					// ファイルに出力しているから削除する
					DeleteMessageByMailKey(mailKey);

					continue;
				}

				messageList.Add(message);

				// 削除（デコード失敗したものはモードに応じて削除選択）
				if ((delMode == DeleteMode.Always)
					|| (delMode == DeleteMode.Success)
					|| ((delMode == DeleteMode.OnList) && ((delList == null) || delList.Any(list => list.Equals(message)))))
				{
					DeleteMessageByMailKey(mailKey);
				}

			} // foreach (int iKey in dicMsglist.Keys)

			return messageList.ToArray();
		}

		/// <summary>
		/// メッセージ取得＆削除
		/// </summary>
		/// <param name="mailKey">メールキー</param>
		public void DeleteMessageByMailKey(int mailKey)
		{
			SendLine(string.Format("DELE {0}", mailKey));
			var line = ReceiveLine();
			if (IsOK(line) == false)
			{
				throw new w2Exception(line.Split(new[] { ' ' })[1] + "\r\n");
			}
		}

		/// <summary>
		/// メールリストの取得
		/// </summary>
		/// <returns>Mail list</returns>
		public Dictionary<int, int> GetMailLists()
		{
			// 接続されていなければ例外
			if ((this.TcpClientObject == null) || (this.TcpClientObject.Connected == false))
			{
				throw new IOException("接続されていません");
			}

			SendLine("LIST");
			var line = ReceiveLine();
			if (IsOK(line) == false)
			{
				throw new InvalidDataException(string.Format("一覧取得に失敗しました:{0}", line));
			}

			var mailList = new Dictionary<int, int>();
			while ((line = ReceiveLine()) != ".")
			{
				string[] val = line.Split(' ');
				mailList.Add(int.Parse(val[0]), int.Parse(val[1]));
			}

			return mailList;
		}

		/// <summary>
		/// メール本体の取得
		/// </summary>
		/// <param name="mailKey">メールキー</param>
		/// <returns>メールヘッダ・本文のペア</returns>
		private Tuple<Pop3MessageHeader, string> GetMailBody(int mailKey)
		{
			try
			{
				SendLine(string.Format("RETR {0}", mailKey));
				var line = "";
				// コマンド成否判定メッセージ取得までループする
				// ※前メールの取り込みが途中で失敗していた場合、残データがストリームに残っている可能性があるため
				while (IsMessageOfSuccessOrError(line) == false)
				{
					line = ReceiveLine();
				}
				// ヘッダー取得
				var headerString = GetMailHeader(line);
				// BODY取得
				var bodyBytes = ReceiveDataPart();

				// ヘッダ・本文作成
				try
				{
					var messageHeaderBody = MakeHeaderBodyPair(headerString.ToString(), bodyBytes);
					return messageHeaderBody;
				}
				catch (Exception ex)
				{
					WriteParseErrorLog("ヘッダ・本文の作成に失敗しました。", headerString.ToString(), ex);
					return null;
				}
			}
			catch (Exception ex)
			{
				WriteParseErrorLog("ヘッダ・本文の取得に失敗しました。", null, ex);
				return null;
			}
		}

		/// <summary>
		/// 成否判定メッセージか
		/// </summary>
		/// <param name="message">応答メッセージ</param>
		/// <returns>TRUE：成否判定メッセージ</returns>
		private bool IsMessageOfSuccessOrError(string message)
		{
			var result = (message.StartsWith("+OK") || message.StartsWith("-ERR"));
			return result;
		}

		/// <summary>
		/// ヘッダー取得
		/// </summary>
		/// <param name="line">行データ</param>
		/// <returns>ヘッダー</returns>
		private StringBuilder GetMailHeader(string line)
		{
			if (IsOK(line) == false)
			{
				throw new InvalidDataException(string.Format("2.メール受信失敗:{0}", line));
			}

			// ヘッダ取得
			var headerString = new StringBuilder();
			while ((line = ReceiveLine()) != ".")
			{
				if (line == "") break;
				headerString.AppendLine(line);
			}
			return headerString;
		}

		/// <summary>
		/// ヘッダー・本文作成
		/// 
		/// </summary>
		/// <param name="headerString">ヘッダー文字列</param>
		/// <param name="bodyBytes">本文</param>
		/// <returns>ヘッダーと本文</returns>
		protected static Tuple<Pop3MessageHeader, string> MakeHeaderBodyPair(string headerString, byte[] bodyBytes)
		{
			var header = new Pop3MessageHeader(headerString);
			var bodyEncoding = header.GetBodyReadEncoding();

			// ヘッダからエンコーディングが確定できない場合は推測
			if (bodyEncoding == null) bodyEncoding = JCode.GetEncoding(bodyBytes);

			var bodyRaw = bodyEncoding.GetString(bodyBytes);
			var messageHeaderBody = new Tuple<Pop3MessageHeader, string>(header, bodyRaw);
			return messageHeaderBody;
		}

		/// <summary>
		/// デコードして整形メールの取得
		/// </summary>
		/// <param name="headerBody">メールヘッダ・本文</param>
		/// <returns>メール</returns>
		protected Pop3Message GetDecodedMail(Tuple<Pop3MessageHeader, string> headerBody)
		{
			Pop3Message message = null;
			try
			{
				message = new Pop3Message(headerBody.Item1, headerBody.Item2);
			}
			catch (Exception ex)
			{
				WriteParseErrorLog(
					"メールのParseに失敗しました。",
					((headerBody != null) ? headerBody.Item1.Source : "") + "\r\n" + headerBody.Item2, ex);
				message = null;
			}
			return message;
		}

		/// <summary>
		/// メッセージ取得
		/// </summary>
		/// <param name="mailKey">メールキー</param>
		/// <returns>取得メッセージ</returns>
		public Pop3Message GetMessageByMailKey(int mailKey)
		{
			try
			{
				var message = GetMessage(mailKey);
				return message;
			}
			catch (Exception ex)
			{
				MailSendDueToOccurredException();
				// ログファイル出力
				FileLogger.Write("PopError", "", ex);
				throw;
			}
		}

		/// <summary>
		/// メッセージ取得＆削除
		/// </summary>
		/// <param name="mailKey">メールキー</param>
		/// <returns>取得メッセージ</returns>
		private Pop3Message GetMessage(int mailKey)
		{
			var headerBody = GetMailBody(mailKey);
			if (headerBody == null) return null;

			Pop3Message message = GetDecodedMail(headerBody);
			if (message == null) return null;

			return message;
		}

		/// <summary>
		/// パースエラーログ書き込み
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="source">ソース</param>
		/// <param name="ex">例外</param>
		private static void WriteParseErrorLog(string message, string source, Exception ex)
		{
			// ファイル出力
			var filePath = WriteErrorMail(source);

			var errorMailMessage = message + "\r\n"
				+ "---- [SRC]" + "\r\n"
				+ filePath + "\r\n";
			AppLogger.WriteFatal(errorMailMessage, ex);
		}

		/// <summary>
		/// エラーメール出力
		/// </summary>
		/// <param name="source">ソース</param>
		/// <returns>ファイル名</returns>
		private static string WriteErrorMail(string source)
		{
			// 出力ファイル
			var dirPath = Path.Combine(Constants.PHYSICALDIRPATH_LOGFILE, "FatalMail");
			var fileName = string.Format("{0}.txt", DateTime.Now.ToString("yyyyMMddhhmmss"));
			var filePath = Path.Combine(dirPath, fileName);

			// ディレクトリ作成
			if (Directory.Exists(dirPath) == false) Directory.CreateDirectory(dirPath);

			// メールの書き出し
			using (var sw = new StreamWriter(filePath, false, Encoding.GetEncoding("Shift_JIS")))
			{
				sw.Write(source);
			}
			return filePath;
		}

		/// <summary>
		/// 閉じる
		/// </summary>
		public void Quit()
		{
			if ((this.TcpClientObject != null)
				&& (this.TcpClientObject.Connected))
			{
				// ユーザ認証エラーなどのときにここに来ると
				// NullReffernceExceptionエラーとなることがあるためtry～catchで回避
				try
				{
					SendLine("QUIT");

					string line = ReceiveLine();
					if (IsOK(line) == false)
					{
						throw new InvalidDataException(string.Format("切断時にエラーが発生しました。:{0}", line));
					}
				}
				catch { }
				finally
				{
					// 閉じる
					if (this.RWStream != null)
					{
						try { this.RWStream.Close(); }
						finally { this.RWStream = null; }
					}
					if (this.TcpClientObject != null)
					{
						try { this.TcpClientObject.Close(); }
						finally { this.TcpClientObject = null; }
					}
				}
			}
		}

		/// <summary>
		/// 応答メッセージ成功判定
		/// </summary>
		/// <param name="message">応答メッセージ</param>
		/// <returns>成功可否</returns>
		private bool IsOK(string message)
		{
			if (message.StartsWith("+OK")) return true;
			if (message.StartsWith("-ERR")) return false;
			throw new InvalidDataException(string.Format("サーバからの応答コードが不正です。:{0}", message));
		}

		/// <summary>
		/// データ送信
		/// </summary>
		/// <param name="message">送信メッセージ</param>
		private void SendLine(string message)
		{
			var data = Encoding.ASCII.GetBytes(message + "\r\n");
			this.RWStream.Write(data, 0, data.Length);
		}

		/// <summary>
		/// １行データ受信
		/// </summary>
		private string ReceiveLine()
		{
			return ReceiveLine(Encoding.ASCII);
		}
		/// <summary>
		/// １行データ受信
		/// </summary>
		/// <param name="enc">エンコード</param>
		private string ReceiveLine(Encoding enc)
		{
			using (var ms = new MemoryStream())
			{
				var tmp = 0;
				var last = 0;

				while (true)
				{
					//受信
					tmp = this.RWStream.ReadByte();
					if (tmp == -1) throw new InvalidDataException("予期しないEOFが検出されました（接続が切断されました）。");
					ms.WriteByte((byte)tmp);

					// 改行であれば抜ける
					if ((last == 0x0D) && (tmp == 0x0A))
					{
						ms.SetLength(ms.Length - 2);	// CR LF
						break;
					}
					last = tmp;
				}

				var data = enc.GetString(ms.ToArray());
				return data;
			}
		}

		/// <summary>
		/// データ部分全受信
		/// </summary>
		/// <returns>データ部分</returns>
		private byte[] ReceiveDataPart()
		{
			var CRLF_DOT_CRLF = Encoding.ASCII.GetBytes("\r\n.\r\n");
			var DOT_CRLF = Encoding.ASCII.GetBytes(".\r\n");

			using (var ms = new MemoryStream())
			{
				var buff = new byte[1 * 1024 * 1024];
				while (true)
				{
					// 受信
					var size = this.RWStream.Read(buff, 0, buff.Length);
					if (size == 0) throw new InvalidDataException("予期しないEOFが検出されました（接続が切断されました）。");

					ms.Write(buff, 0, size);

					var data = ms.ToArray();

					if (IsBytesEndWith(data, CRLF_DOT_CRLF))
					{
						return data.Take(data.Length - CRLF_DOT_CRLF.Length).ToArray();
					}
					if (IsBytesStartWith(data, DOT_CRLF))
					{
						return data.Take(data.Length - DOT_CRLF.Length).ToArray();
					}
				}
			}
		}

		/// <summary>
		/// 指定バイト列で始まるか
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="search">始まる値</param>
		/// <returns>始まるか</returns>
		internal bool IsBytesStartWith(byte[] input, byte[] search)
		{
			if (input == null) return false;
			if (input.Length == 0) return false;

			if (search == null) return false;
			if (search.Length == 0) return false;

			if (input.Length < search.Length) return false;

			return input.Take(search.Length).ToArray().SequenceEqual(search);
		}
		/// <summary>
		/// 指定バイト列で終わるか
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="search">終わる値</param>
		/// <returns>終わるか</returns>
		internal bool IsBytesEndWith(byte[] input, byte[] search)
		{
			if (input == null) return false;
			if (input.Length == 0) return false;

			if (search == null) return false;
			if (search.Length == 0) return false;

			if (input.Length < search.Length) return false;

			return input.Skip(input.Length - search.Length).ToArray().SequenceEqual(search);
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			Quit();
		}

		/// <summary>POPアカウント情報</summary>
		private PopAccountSetting PopSetting { get; set; }

		/// <summary>TCPクライアント</summary>
		TcpClient TcpClientObject { get; set; }
		/// <summary>TcpClientのNetworkStream or SslStream</summary>
		Stream RWStream { get; set; }
		/// <summary>SSLプロトコルバージョン</summary>
		private SslProtocols Sslversions;

		/// <summary>
		/// 削除モード列挙体
		/// </summary>
		private enum DeleteMode
		{
			/// <summary>デコード成否にかかわらず削除</summary>
			Always,
			/// <summary>デコード成功したら削除</summary>
			Success,
			/// <summary>リストに存在すれば削除</summary>
			OnList,
			/// <summary>削除しない</summary>
			NoDelete,
		}

		/// <summary>
		/// POPアカウント情報クラス
		/// </summary>
		public class PopAccountSetting
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="server">サーバ名</param>
			/// <param name="port">ポート番号</param>
			/// <param name="user">ユーザ名</param>
			/// <param name="password">パスワード</param>
			/// <param name="popType">認証方式</param>
			public PopAccountSetting(string server, int port, string user, string password, PopType popType)
			{
				this.Server = server;
				this.Port = port;
				this.UserId = user;
				this.Password = password;
				this.PopType = popType;
			}
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="setting">POPアカウント情報</param>
			public PopAccountSetting(string[] setting)
				: this(setting[0], int.Parse(setting[1]), setting[2], setting[3], (PopType)Enum.Parse(typeof(PopType), setting[4]))
			{
			}

			/// <summary>サーバ名</summary>
			public string Server { get; private set; }
			/// <summary>ポート番号</summary>
			public int Port { get; private set; }
			/// <summary>ユーザ名</summary>
			public string UserId { get; private set; }
			/// <summary>パスワード</summary>
			public string Password { get; private set; }
			/// <summary>認証方式</summary>
			public PopType PopType { get; private set; }
		}
	}
}
