/*
=========================================================================================================
  Module      : DKIM署名クラス(DkimSigner.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using MimeKit;
using MimeKit.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using w2.Common.Extensions;

namespace w2.Common.Net.Mail
{
	/// <summary>
	/// DKIM署名作成クラス
	/// </summary>
	public static class DkimHelper
	{
		/// <summary>定数: 秘密鍵ディレクトリ名</summary>
		private const string KEY_FILE_DIR = @"DkimKey\";

		/// <summary>
		/// 署名
		/// </summary>
		/// <param name="mailMessage">メールメッセージ</param>
		/// <param name="headers">ヘッダー</param>
		/// <param name="bodys">本文</param>
		/// <returns>署名値</returns>
		/// <remarks>
		/// この処理は設定値によってシスエラが出る可能性があるが、それによってメール送信が停止してしまうのは本末転倒であるためTryCatchで囲うべき
		/// </remarks>
		public static string Sign(MailMessage mailMessage, List<MailSendTextTemp> headers, List<MailSendTextTemp> bodys)
		{
			var fromMailDomain = mailMessage.From.AddressRaw.Split('@')[1];
			var fromMailDomainReplased = fromMailDomain
				.Replace("\\", string.Empty)
				.Replace("/", string.Empty)
				.Replace(":", string.Empty)
				.Replace("*", string.Empty)
				.Replace("?", string.Empty)
				.Replace("\"", string.Empty)
				.Replace("<", string.Empty)
				.Replace(">", string.Empty)
				.Replace("|", string.Empty);
			
			var message = headers
				.Concat(bodys)
				.Select(x => x.MailSendText)
				.JoinToString("\r\n");

			// MimeKitを利用して署名を作成する
			using (var reader = new MemoryStream(mailMessage.HeaderMIMEEncoder.Encoding.GetBytes(message)))
			{
				var mimeMessage = MimeMessage.Load(reader, persistent: true);

				var signer = new DkimSigner(
					$"{Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE}{KEY_FILE_DIR}{fromMailDomainReplased}.private",
					fromMailDomain,
					Constants.MAIL_DKIM_SELECTOR)
				{
					HeaderCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Relaxed,
					BodyCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Relaxed,
				};
				var headersToSign = new[]
				{
					HeaderId.From,
					HeaderId.Subject,
					HeaderId.Date,
				};

				signer.Sign(mimeMessage, headersToSign);
				var dkim = mimeMessage.Headers.Single(h => h.Id == HeaderId.DkimSignature);

				return dkim.Value;
			}
		}
	}
}
