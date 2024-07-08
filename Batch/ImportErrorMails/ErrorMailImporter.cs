/*
=========================================================================================================
  Module      : エラーメール取込処理(ErrorMailImporter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using w2.Common.Net.Mail;
using w2.Common.Sql;
using w2.Domain.MailErrorAddr;

namespace w2.MarketingPlanner.Batch.ImportErrorMails
{
	public class ErrorMailImporter
	{
		const string SETTING_MAILADDR_STRING = "メールアドレス";
		const string SETTING_MAILADDR_PATTERN = "[0-9a-zA-Z\\-_\\?\\/\\+\"][0-9a-zA-Z~_\\.\\-\\?\\/\\+\"]*@[0-9a-zA-Z][0-9a-zA-Z\\.\\-]*\\.[0-9a-zA-Z]+";

		/// <summary>エラーメールパターン設定</summary>
		XmlDocument m_errorMailPatternSetting = new XmlDocument();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ErrorMailImporter()
			: this(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Xml\Setting\ErrorMailPatternSetting.xml"))
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settingXmlFilePath">設定XMLファイルパス</param>
		public ErrorMailImporter(string settingXmlFilePath)
		{
			// XML読み込み
			m_errorMailPatternSetting.Load(settingXmlFilePath);
		}

		/// <summary>
		/// エラーメール取込
		/// </summary>
		/// <param name="pop3Messages">POP3メッセージ</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		public void Import(Pop3Message pop3Messages, SqlAccessor sqlAccessor)
		{
			if (IsExclusionMail(pop3Messages.Subject, pop3Messages.BodyDecoded))
			{
				w2.Common.Logger.FileLogger.WriteWarn("除外:" + pop3Messages.From + " " + pop3Messages.Subject);
				return;
			}

			if (ImportInner(pop3Messages, sqlAccessor) == false)
			{
				// マッチングの対象が見つからなかった場合ログに落とす
				StringBuilder unmatchedErrorMail = new StringBuilder();
				unmatchedErrorMail.Append("\r\n");
				unmatchedErrorMail.Append(pop3Messages.Subject).Append("\r\n");
				unmatchedErrorMail.Append(pop3Messages.BodyDecoded).Append("\r\n");
				w2.Common.Logger.FileLogger.Write("unmatched", unmatchedErrorMail.ToString());
			}
		}

		/// <summary>
		/// 除外メール判定
		/// </summary>
		/// <param name="subject">メール件名</param>
		/// <param name="body">メール本文</param>
		private bool IsExclusionMail(string subject, string body)
		{
			if ((Regex.IsMatch(subject, @"^\[AntiSpam\]"))
				|| (Regex.IsMatch(subject, @"^Facebookで.*をチェック")))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// エラーメール取込
		/// </summary>
		/// <param name="pop3Messages">POP3メッセージ</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="updateDB">DB更新するか(パターン追加用に単体テストを動かすときはfalse指定)</param>
		/// <returns>判定失敗</returns>
		public bool ImportInner(Pop3Message pop3Messages, SqlAccessor sqlAccessor, bool updateDB = true)
		{
			// パターンについて繰り返し
			foreach (XmlNode xnPattern in m_errorMailPatternSetting.SelectNodes("ErrorMailPatternSetting/Error/Pattern"))
			{
				if ((xnPattern.Attributes != null)
					&& (xnPattern.Attributes.Count > 0))
				{
					switch (xnPattern.Attributes["target"].Value)
					{
						case "header":
							if ((pop3Messages.Header != null)
								&& MatchForPattern(pop3Messages.Header.Source, xnPattern)) return true;
							break;

						case "body":
							if ((pop3Messages.BodyDecoded != null)
								&& MatchForPattern(pop3Messages.BodyDecoded, xnPattern)) return true;
							break;

						case "headerAndBody":
							if ((pop3Messages.Header != null)
								&& MatchForPattern(pop3Messages.Header.Source, xnPattern)) return true;
							if ((pop3Messages.BodyDecoded != null)
								&& MatchForPattern(pop3Messages.BodyDecoded, xnPattern)) return true;
							break;
					}
				}
				else
				{
					if ((pop3Messages.BodyDecoded != null)
						&& MatchForPattern(pop3Messages.BodyDecoded, xnPattern)) return true;
				}
			}

			return false;
		}

		/// <summary>
		/// エラーメールパターンをマッチング
		/// </summary>
		/// <param name="mailText">メールボディ、あるいはヘッダー</param>
		/// <param name="xnPattern">パターン</param>
		/// <param name="updateDB">DB更新するか(パターン追加用に単体テストを動かすときはfalse指定)</param>
		/// <returns></returns>
		private bool MatchForPattern(string mailText, XmlNode xnPattern, bool updateDB = true)
		{
			foreach (Match match in Regex.Matches(
				mailText.Replace("=\r\n", ""),
				xnPattern.InnerText.Replace(SETTING_MAILADDR_STRING, SETTING_MAILADDR_PATTERN),
				RegexOptions.IgnoreCase))
			{
				//------------------------------------------------------
				// エラーレベル取得
				//------------------------------------------------------
				int iErrorPoint;
				if (int.TryParse(xnPattern.ParentNode.Attributes["Point"].Value, out iErrorPoint) == false)
				{
					iErrorPoint = 1;	// デフォルト1
				}
				if (iErrorPoint == 0) return true;

				//------------------------------------------------------
				// 対象エラーメールアドレス取得
				//------------------------------------------------------
				var mailAddr = match.Value;
				mailAddr = Regex.Replace(mailAddr, xnPattern.InnerText.Substring(0, xnPattern.InnerText.IndexOf(SETTING_MAILADDR_STRING)), "");
				mailAddr = Regex.Replace(mailAddr, xnPattern.InnerText.Substring(xnPattern.InnerText.IndexOf(SETTING_MAILADDR_STRING) + SETTING_MAILADDR_STRING.Length), "");

				//------------------------------------------------------
				// エラーアドレス登録＆ポイント追加
				//------------------------------------------------------
				if (updateDB) new MailErrorAddrService().AddErrorPoint(mailAddr.Replace("\"", ""), iErrorPoint);

				//------------------------------------------------------
				// ＤＢ更新したらこのメールについては終了
				// （先頭のエラーのみ取得。こちらからの送信メールとマッチングしてしまうとまずいので・・・）
				//------------------------------------------------------
				return true;
			}
			return false;
		}
	}
}
