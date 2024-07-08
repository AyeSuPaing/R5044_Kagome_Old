/*
=========================================================================================================
  Module      : Exchange Client (ExchangeClient.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using w2.Common.Logger;
using w2.Common.Net.Mail;

namespace w2.CustomerSupport.Batch.CsMailReceiver
{
	/// <summary>
	/// Exchange client
	/// </summary>
	public class ExchangeClient: Pop3Client
	{
		/// <summary>Log kbn</summary>
		private const string LOG_KBN = "Exchange";
		/// <summary>Exchange service</summary>
		private ExchangeService _service;

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setting">Exchange account setting</param>
		public ExchangeClient(ExchangeAccountSetting setting)
			: base()
		{
			this.Setting = setting;
			_service = CreateExchangeService();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Get and save token
		/// </summary>
		/// <param name="setting">Exchange account setting</param>
		/// <returns>An Exchange token</returns>
		public static ExchangeToken GetAndSaveToken(ExchangeAccountSetting setting)
		{
			ExchangeToken token = null;

			try
			{
				token = GetToken(setting);
				if ((token == null)
					|| string.IsNullOrEmpty(token.AccessToken))
				{
					FileLogger.WriteError(GetTokenErrorLog(setting.Account));
					return null;
				}

				SaveTokenToFile(setting, token);
			}
			catch (AggregateException ex)
			{
				FileLogger.WriteError(GetTokenErrorLog(setting.Account), ex.InnerException);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(GetTokenErrorLog(setting.Account), ex);
			}

			return token;
		}

		/// <summary>
		/// Get Exchange mail messages
		/// </summary>
		/// <returns>An array of mail messages</returns>
		public Pop3Message[] GetMailMessages()
		{
			try
			{
				if (_service == null) return null;

				// Maximum received mails is int.MaxValue
				var items = _service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue));
				if (items == null) return null;

				var messages = new List<Pop3Message>();
				foreach (EmailMessage item in items)
				{
					item.Load(new PropertySet(ItemSchema.MimeContent));
					var bodyWithHeader = GetMailBodyWithHeader(item);
					var pop3Message = new Pop3Message(bodyWithHeader);

					if (pop3Message == null) continue;
					item.Load(new PropertySet(
						BasePropertySet.FirstClassProperties,
						ItemSchema.TextBody));

					pop3Message.ExchangeMessageId = item.Id.UniqueId;
					messages.Add(pop3Message);
				}

				return messages.ToArray();
			}
			catch (Exception exception)
			{
				WriteErrorMessage(exception);
				return null;
			}
		}

		/// <summary>
		/// Delete mail messages
		/// </summary>
		/// <param name="mailIds">An array of mail IDs</param>
		public void DeleteMailMessages(string[] mailIds)
		{
			if (mailIds.Length == 0) return;

			try
			{
				// Maximum received mails is int.MaxValue
				var items = _service.FindItems(WellKnownFolderName.Inbox, new ItemView(int.MaxValue));
				if (items == null) return;

				var deletedItemIds = new List<ItemId>();
				foreach (var item in items)
				{
					if (mailIds.Any(id => id == item.Id.UniqueId) == false) continue;
					deletedItemIds.Add(item.Id);
				}

				_service.DeleteItems(
					deletedItemIds,
					DeleteMode.MoveToDeletedItems,
					null,
					null);
			}
			catch (Exception exception)
			{
				var errorMessage = WriteErrorMessage(exception);
				MailSendDueToOccurredException(errorMessage);
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Write error message
		/// </summary>
		/// <param name="exception">Exception</param>
		/// <returns>An error message</returns>
		private string WriteErrorMessage(Exception exception)
		{
			var errorMessage = string.Format(
				"{0}:{1}",
				this.Setting.Account,
				exception);
			FileLogger.Write(LOG_KBN, errorMessage);
			return errorMessage;
		}

		/// <summary>
		/// Create Exchange service
		/// </summary>
		/// <returns>An Exchange service</returns>
		private ExchangeService CreateExchangeService()
		{
			try
			{
				var service = GetExchangeService();
				return service;
			}
			catch
			{
				try
				{
					GetAndSaveToken(this.Setting);
					var service = GetExchangeService();

					return service;
				}
				catch
				{
					var errorMessage = string.Format(
						"{0}:{1}",
						this.Setting.Account,
						"「設定が誤っているか、[Client Secret] の有効期限切れ。」");
					FileLogger.Write(LOG_KBN, errorMessage);
					MailSendDueToOccurredException(errorMessage);

					return null;
				}
			}
		}

		/// <summary>
		/// Get Exchange token
		/// </summary>
		/// <returns>An Exchange token</returns>
		private ExchangeToken GetExchangeToken()
		{
			var tokenFilePath = Path.Combine(
				this.Setting.TokenDirectory,
				this.Setting.DefaultTokenFileName);
			if (File.Exists(tokenFilePath) == false)
			{
				FileLogger.Write(
					LOG_KBN,
					string.Format(
						"{0}:「トークンは存在していません。」",
						this.Setting.Account));

				return null;
			}

			using (var reader = new StreamReader(tokenFilePath, Encoding.UTF8))
			{
				var token = JsonConvert.DeserializeObject<ExchangeToken>(reader.ReadToEnd());
				return token;
			}
		}

		/// <summary>
		/// Get Exchange service
		/// </summary>
		/// <returns>Exchange service</returns>
		private ExchangeService GetExchangeService()
		{
			var token = GetExchangeToken();
			if (token == null) return null;

			var service = new ExchangeService
			{
				Url = new Uri(Constants.EXCHANGE_WEB_SERVICES_URL),
				Credentials = new OAuthCredentials(token.AccessToken),
				ImpersonatedUserId =
					new ImpersonatedUserId(ConnectingIdType.SmtpAddress, this.Setting.Account)
			};

			// Test access token is valid: Try to get mail
			service.FindItems(WellKnownFolderName.Inbox, new ItemView(1));

			return service;
		}

		/// <summary>
		/// Get token
		/// </summary>
		/// <param name="setting">Exchange account setting</param>
		/// <returns>An Exchange token</returns>
		private static ExchangeToken GetToken(ExchangeAccountSetting setting)
		{
			var client = ConfidentialClientApplicationBuilder
				.Create(setting.ClientId)
				.WithClientSecret(setting.ClientSecret)
				.WithTenantId(setting.TenantId)
				.Build();

			var authResult = client.AcquireTokenForClient(Constants.EXCHANGE_WEB_SERVICES_SCOPES)
				.ExecuteAsync().Result;
			var token = new ExchangeToken(authResult);

			WriteTokenResponseLog(setting, token);

			return token;
		}

		/// <summary>
		/// Get token error log
		/// </summary>
		/// <param name="account">An account</param>
		/// <returns>An error message</returns>
		public static string GetTokenErrorLog(string account)
		{
			var errorMessage = string.Format("{0} :「トークンを取得できません。」", account);
			return errorMessage;
		}

		/// <summary>
		/// Save token to file
		/// </summary>
		/// <param name="setting">Exchange account setting</param>
		/// <param name="token">Exchange token</param>
		private static void SaveTokenToFile(ExchangeAccountSetting setting, ExchangeToken token)
		{
			if (Directory.Exists(setting.TokenDirectory) == false)
			{
				Directory.CreateDirectory(setting.TokenDirectory);
			}

			File.WriteAllText(
				Path.Combine(setting.TokenDirectory, setting.DefaultTokenFileName),
				JsonConvert.SerializeObject(token));
		}

		/// <summary>
		/// Write token response log
		/// </summary>
		/// <param name="setting">Exchange account setting</param>
		/// <param name="token">Exchange token</param>
		private static void WriteTokenResponseLog(ExchangeAccountSetting setting, ExchangeToken token)
		{
			var result = JsonConvert.SerializeObject(token, Formatting.Indented);
			FileLogger.WriteInfo(string.Format("{0}:\r\n{1}", setting.Account, result));
		}

		/// <summary>
		/// Get mail body with header
		/// </summary>
		/// <param name="emailMessage">Email message</param>
		/// <returns>Mail body with header</returns>
		private static string GetMailBodyWithHeader(EmailMessage emailMessage)
		{
			var encoding = Encoding.Default;
			var pop3MessageHeader = new Pop3MessageHeader(encoding.GetString(emailMessage.MimeContent.Content));
			if (string.IsNullOrEmpty(pop3MessageHeader.ContentType.CharSet) == false)
			{
				encoding = Pop3MessageHeader.GetEncoding(pop3MessageHeader.ContentType.CharSet)
					?? encoding;
			}
			return encoding.GetString(emailMessage.MimeContent.Content);
		}
		#endregion

		#region Properties
		/// <summary>Setting</summary>
		public ExchangeAccountSetting Setting { get; private set; }
		#endregion
	}
}
