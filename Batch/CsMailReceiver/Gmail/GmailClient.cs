/*
=========================================================================================================
  Module      : Gmail Client(GmailClient.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Common.Net.Mail;

namespace w2.CustomerSupport.Batch.CsMailReceiver
{
	/// <summary>
	/// Gmail Client
	/// </summary>
	public class GmailClient: Pop3Client
	{
		/// <summary>Flg Gmail Label Inbox</summary>
		private const string FLG_GMAIL_LABEL_INBOX = "INBOX";
		/// <summary>Gmail User Id</summary>
		private const string GMAIL_USER_ID = "me";
		/// <summary>Gmail User</summary>
		private const string GMAIL_USER = "user";
		/// <summary>Url Create Token</summary>
		private const string URL_CREATE_TOKEN = "https://oauth2.googleapis.com/token";
		/// <summary>Content Type</summary>
		private const string CONTENT_TYPE = "application/x-www-form-urlencoded";


		/// <summary>
		/// Constructor
		/// </summary>
		public GmailClient(GmailAccountSetting setting)
			: base()
		{
			this.Setting = setting;
		}

		/// <summary>
		/// Get Messages For Account Gmail
		/// </summary>
		/// <returns>Array Message</returns>
		public Pop3Message[] GetMessagesForAccountGmail()
		{
			var messageList = new List<Pop3Message>();
			try
			{
				var service = CreateMessageService();
				if (service == null) return messageList.ToArray();

				var request = service.List(GMAIL_USER_ID);
				request.LabelIds = new string[] { FLG_GMAIL_LABEL_INBOX };
				var messages = request.Execute().Messages;

				if (messages == null) return messageList.ToArray();

				foreach (var item in messages)
				{
					var gmailService = service.Get(GMAIL_USER_ID, item.Id);
					gmailService.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Raw;
					var message = gmailService.Execute();

					// Get encode charset
					gmailService.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;
					var messageFull = gmailService.Execute();
					var mailEncoding = GetMailEncoding(messageFull);

					// Get Header And Body
					var mailDataWithBody = GetContentGmail(
						StringUtility.ToEmpty(message.Raw),
						mailEncoding);
					var pop3Message = new Pop3Message(mailDataWithBody);
					if (pop3Message == null) continue;

					pop3Message.GmailMessageId = message.Id;
					messageList.Add(pop3Message);
				}
			}
			catch (Exception exception)
			{
				var errorMessage = string.Format("{0}:{1}", this.Setting.Account, exception);
				FileLogger.Write("GmailApi", errorMessage);
				MailSendDueToOccurredException(errorMessage);
			}
			return messageList.ToArray();
		}

		/// <summary>
		/// Create Token From File
		/// </summary>
		/// <returns>Token</returns>
		private TokenResponse CreateTokenFromFile()
		{
			var pathToken = Path.Combine(
				this.Setting.TokenDirectory,
				this.Setting.TokenFileNameDefault);
			if (File.Exists(pathToken) == false)
			{
				FileLogger.Write(
					"GmailApi",
					string.Format("{0}:「トークンは存在していません。」", this.Setting.Account));
				return null;
			}
			using (var reader = new StreamReader(pathToken, Encoding.UTF8))
			{
				var token = JsonConvert.DeserializeObject<TokenResponse>(reader.ReadToEnd());
				var result = CheckRefreshTokenExpired(token.RefreshToken)
					? token
					: null;
				return result;
			}
		}

		/// <summary>
		/// Check Refresh Token Expired
		/// </summary>
		/// <param name="refreshToken">Refresh Token</param>
		/// <returns>True: If create access token from refresh token success</returns>
		public bool CheckRefreshTokenExpired(string refreshToken)
		{
			try
			{
				var request = (HttpWebRequest)WebRequest.Create(URL_CREATE_TOKEN);
				request.Method = "POST";
				request.ContentType = CONTENT_TYPE;

				var body = string.Format(
					"grant_type=refresh_token&refresh_token={0}&client_id={1}&client_secret={2}",
					refreshToken,
					this.Setting.ClientId,
					this.Setting.ClientSecret);

				var byteData = Encoding.UTF8.GetBytes(body);
				request.ContentLength = byteData.Length;

				// Write POST data
				var stream = request.GetRequestStream();
				stream.Write(byteData, 0, byteData.Length);

				// Get response
				using (var response = request.GetResponse())
				using (stream = response.GetResponseStream())
				using (var reader = new StreamReader(stream))
				{
					var token = JsonConvert.DeserializeObject<TokenResponse>(reader.ReadToEnd());
					return (string.IsNullOrEmpty(token.AccessToken) == false);
				}
			}
			catch
			{
				var errorMessage = string.Format("{0}:「設定が誤っているか、リフレッシュトークンが無効です。」", this.Setting.Account);
				FileLogger.Write("GmailApi", errorMessage);
				MailSendDueToOccurredException(errorMessage);
				return false;
			}
		}

		/// <summary>
		/// Delete Messages For Gmail
		/// </summary>
		/// <param name="gmailMessageIds">Gmail Message Ids</param>
		public void DeleteMessagesForGmail(string[] gmailMessageIds)
		{
			if (gmailMessageIds.Length == 0) return;

			var service = CreateMessageService();
			foreach (var gmailMessageId in gmailMessageIds)
			{
				service.Delete(GMAIL_USER_ID, gmailMessageId).Execute();
			}
		}

		/// <summary>
		/// Create Message Service
		/// </summary>
		/// <returns>Message Service</returns>
		private UsersResource.MessagesResource CreateMessageService()
		{
			var credential = CreateUserCredential();
			if (credential == null) return null;

			// Create API service
			var gmailService = new GmailService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = Constants.APPLICATION_NAME,
			});
			var service = gmailService.Users.Messages;
			return service;
		}

		/// <summary>
		/// Create User Credential
		/// </summary>
		/// <returns>User Credential</returns>
		private UserCredential CreateUserCredential()
		{
			var token = CreateTokenFromFile();
			if (token == null) return null;

			var initializer = new GoogleAuthorizationCodeFlow.Initializer
			{
				ClientSecrets = new ClientSecrets
				{
					ClientId = this.Setting.ClientId,
					ClientSecret = this.Setting.ClientSecret
				}
			};
			var credential = new UserCredential(
				new GoogleAuthorizationCodeFlow(initializer),
				GMAIL_USER,
				token);
			return credential;
		}

		/// <summary>
		/// Get Content Gmail
		/// </summary>
		/// <param name="data">Data</param>
		/// <param name="encoding">Encoding</param>
		/// <returns>Content</returns>
		public string GetContentGmail(string data, Encoding encoding)
		{
			data = data.Replace("-", "+").Replace("_", "/");
			var bytes = Convert.FromBase64String(data);
			var result = encoding.GetString(bytes);
			return result;
		}

		/// <summary>
		/// Get mail encoding
		/// </summary>
		/// <param name="message">Message</param>
		/// <returns>Mail encoding</returns>
		private Encoding GetMailEncoding(Message message)
		{
			// If not found content type return default encoding
			var contentTypeHeader = message.Payload.Headers?.FirstOrDefault(
				header => header.Name == "Content-Type");
			if (contentTypeHeader == null) return Encoding.Default;

			// If not found charset in content type return default encoding
			var contentType = new ContentType(contentTypeHeader.Value);
			if (string.IsNullOrEmpty(contentType.CharSet)) return Encoding.Default;

			// Get encoding from charset
			var encoding = Pop3MessageHeader.GetEncoding(contentType.CharSet);
			return encoding;
		}

		/// <summary>Setting</summary>
		public GmailAccountSetting Setting { get; private set; }
	}
}
