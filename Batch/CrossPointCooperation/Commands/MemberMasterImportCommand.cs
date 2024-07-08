/*
=========================================================================================================
  Module      : Member Master Import Command (MemberMasterImportCommand.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using w2.App.Common;
using w2.App.Common.User;
using w2.Commerce.Batch.CrossPointCooperation.ImportSettings;
using w2.Commerce.Batch.CrossPointCooperation.Inputs;
using w2.Common;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.Point;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.ExternalAPI.Common.Ftp;
using Constants = w2.Domain.Constants;

namespace w2.Commerce.Batch.CrossPointCooperation.Commands
{
	/// <summary>
	/// Member master import command
	/// </summary>
	public class MemberMasterImportCommand
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MemberMasterImportCommand()
		{
			this.FtpClient = new FluentFtpUtility(
				w2.App.Common.Constants.CROSS_POINT_FTP_HOST,
				w2.App.Common.Constants.CROSS_POINT_FTP_ID,
				w2.App.Common.Constants.CROSS_POINT_FTP_PW,
				false,
				true,
				w2.App.Common.Constants.CROSS_POINT_FTP_PORT);
			this.ImportTarget = string.Empty;
			this.SuffixImportFileName = string.Empty;

			this.ErrorMessagesForExecution = new StringBuilder();
			this.SuccessMessagesForExecution = new StringBuilder();
			this.ErrorMessages = new StringBuilder();

			this.ImportTarget = "MemberMaster";
			this.ImportSetting = new ImportMemberMasterSetting();
			this.SuffixImportFileName = string.Format(
				"_{0}_MEMBER.csv",
				w2.App.Common.Constants.CROSS_POINT_AUTH_TENANT_CODE);
		}

		/// <summary>
		/// Execute
		/// </summary>
		public void Execute()
		{
			try
			{
				this.StartDateTime = DateTime.Now;

				PrepareDirectories();

				// Get last execute date time
				Checker(this.ImportTarget);
				this.LastExecuteDateTime = GetLastExecuteDateTime();

				// Download and save files into the active folder
				DownloadFilesFromFtpServer(w2.App.Common.Constants.CROSS_POINT_FTP_FILE_PATH);

				// Execute import all file in the active folder
				ImportAllActiveFiles();

				this.EndDateTime = DateTime.Now;

				// Update last write time for file last execution
				CreateEndFile(this.StartDateTime);

				var messages = GetMessages();

				if (this.HasError == false)
				{
					SendMailToOperator(this.HasError, messages);
				}
				else
				{
					SendErrorMail(messages);
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// Prepare directories
		/// </summary>
		private void PrepareDirectories()
		{
			Directory.CreateDirectory(Directories.ActiveFilePath);
			Directory.CreateDirectory(Directories.CompleteFilePath);
			Directory.CreateDirectory(Directories.ErrorFilePath);
		}

		/// <summary>
		/// Download files from ftp server
		/// </summary>
		/// <param name="ftpDirectory">Ftp directory</param>
		private void DownloadFilesFromFtpServer(string ftpDirectory)
		{
			var files = this.FtpClient.FileNameListDownload(ftpDirectory);

			// ファイル一覧を日付降順で並び替え
			files.Sort();
			files.Reverse();

			foreach (var file in files)
			{
				if (CanDownloadFile(file) == false) continue;

				var downloadLocation = Path.Combine(Directories.ActiveFilePath, Path.GetFileName(file));
				var source = Path.Combine(ftpDirectory, file);
				this.FtpClient.Download(source, downloadLocation);

					// ダウンロードできたら処理を抜ける
				return;
			}
		}

		/// <summary>
		/// Can download file
		/// </summary>
		/// <param name="filePath">File path</param>
		/// <returns>True if file can be obtained from FTP server, otherwise: false</returns>
		private bool CanDownloadFile(string filePath)
		{
			var fileName = Path.GetFileName(filePath);
			if (fileName.EndsWith(this.SuffixImportFileName) == false) return false;

			// Get last updated date time from file name
			DateTime lastUpdatedDateTime;
			var timestamp = fileName.Replace(this.SuffixImportFileName, string.Empty);
			if (DateTime.TryParseExact(
					timestamp,
					"yyyyMMddHHmmss",
					null,
					System.Globalization.DateTimeStyles.None,
					out lastUpdatedDateTime) == false)
			{
				return false;
			}

			// For the first execution case, get all files
			if (this.LastExecuteDateTime.HasValue == false) return true;

			return (lastUpdatedDateTime >= this.LastExecuteDateTime);
		}

		/// <summary>
		/// Move file
		/// </summary>
		/// <param name="sourcePath">Source path</param>
		/// <param name="destinationPath">Destination path</param>
		private void MoveFile(string sourcePath, string destinationPath)
		{
			if (File.Exists(sourcePath) == false) return;

			File.Copy(sourcePath, destinationPath, true);
			File.Delete(sourcePath);
		}

		/// <summary>
		/// Move import file to error folder
		/// </summary>
		/// <param name="filePath">File patch</param>
		private void MoveImportFileToErrorFolder(string filePath)
		{
			var errorFilePath = Path.Combine(Directories.ErrorFilePath, Path.GetFileName(filePath));
			MoveFile(filePath, errorFilePath);
			this.DownloadFileName = Path.GetFileName(filePath);
		}

		/// <summary>
		/// Move import file to complete folder
		/// </summary>
		/// <param name="filePath">File patch</param>
		private void MoveImportFileToCompleteFolder(string filePath)
		{
			var completeFilePath = Path.Combine(Directories.CompleteFilePath, Path.GetFileName(filePath));
			MoveFile(filePath, completeFilePath);
			this.DownloadFileName = Path.GetFileName(filePath);
		}

		/// <summary>
		/// Import all active files
		/// </summary>
		private void ImportAllActiveFiles()
		{
			foreach (var file in Directory.EnumerateFiles(Directories.ActiveFilePath))
			{
				ImportActiveFile(file);
			}
		}

		/// <summary>
		/// Import active file
		/// </summary>
		/// <param name="filePath">File path</param>
		private void ImportActiveFile(string filePath)
		{
			this.ErrorMessagesForExecution = new StringBuilder();
			this.SuccessMessagesForExecution = new StringBuilder();
			this.ErrorMessages = new StringBuilder();
			this.InsertUpdateCount = 0;
			this.IsEndOfFile = false;
			this.CurrentLine = 1;

			ImportDataProcess(filePath);

			var fileName = Path.GetFileName(filePath);
			if (this.HasErrorOfExecution)
			{
				SetErrorMessages(fileName);
				MoveImportFileToErrorFolder(filePath);
				return;
			}

			SetSuccessMessagesForExecution(fileName);

			MoveImportFileToCompleteFolder(filePath);
		}

		/// <summary>
		/// Read all data csv file
		/// </summary>
		/// <param name="csvFilePath">Csv file path</param>
		/// <returns>Data list</returns>
		protected List<Hashtable> ReadAllDataCsvFile(string csvFilePath)
		{
			var fieldKeyValues = new List<Hashtable>();

			// Encoder acquisition
			byte[] csvByteStream = null;
			using (var csvFileStream = new FileStream(csvFilePath, FileMode.Open, FileAccess.Read))
			{
				csvByteStream = new byte[csvFileStream.Length];
				csvFileStream.Read(csvByteStream, 0, csvByteStream.Length);
			}

			var encoding = StringUtility.GetCode(csvByteStream);
			if (encoding == null)
			{
				SetErrorMessages("ファイルの読取に失敗しました。[Encoding Error.]");
				return fieldKeyValues;
			}

			// Read CSV file
			using (var reader = new StreamReader(csvFilePath, encoding))
			{
				try
				{
					// Skip the first line and take the second line (this line is header define)
					var headerLine = reader.ReadLine();
					headerLine = reader.ReadLine();
					
					// Get a list of field names from the header line
					var fields = GetFieldNames(headerLine);
					this.ImportSetting.SetFieldNames(fields);

					// Get row information
					fieldKeyValues = GetRowKeyValueList(reader);
				}
				catch (w2Exception ex)
				{
					SetErrorMessages(ex.Message);
				}
			}
			return fieldKeyValues;
		}

		/// <summary>
		/// CSVファイルからHashtableのリスト取得
		/// </summary>
		/// <param name="reader">CSV text reader</param>
		/// <returns>List of row information</returns>
		private List<Hashtable> GetRowKeyValueList(TextReader reader)
		{
			var results = new List<Hashtable>();
			var errorCount = 0;
			
			// 途中まで読み込んでいる場合はその行まで飛ばす
			for (var line = 1; line < this.CurrentLine; line++) reader.ReadLine();
			
			// 1回につき最大1000行読み込む
			this.NextStopLine = this.CurrentLine + 1000;

			// ファイルが終了したか1000行読み込んだら抜ける
			while ((reader.Peek() != -1) && (this.CurrentLine < this.NextStopLine))
			{
				// Increment to represent the current target row
				this.CurrentLine++;

				// Get one line of information (read the stream to the end of one line)
				var rowData = new StringBuilder(GetRowData(reader));
				var errorMessages = new StringBuilder();

				// CSV division of one line
				// Check if the number of fields matches the number of header fields
				var dataLines = new List<string>(StringUtility.SplitCsvLine(rowData.ToString()));
				if (this.ImportSetting.HeadersCsv.Count != dataLines.Count)
				{
					errorMessages.AppendFormat(
						"行のフィールド数が定義と一致しませんでした。フィールド定義数は{0}ですがデータのフィールド数は{1}でした。",
						this.ImportSetting.HeadersCsv.Count,
						dataLines.Count);
				}

				// Has an error occurred in the processing up to this point?
				if (errorMessages.Length == 0)
				{
					// Store data in hash table
					var input = new Hashtable();

					foreach (string fieldName in this.ImportSetting.HeadersCsv)
					{
						var index = this.ImportSetting.HeadersCsv.IndexOf(fieldName);
						if (fieldName == "sex")
						{
							dataLines[index] = UserUtility.GetSystemGender(dataLines[index]);
						}

						input.Add(fieldName, dataLines[index]);
					}

					// Data conversion & input check
					this.ImportSetting.ConvertAndCheck(input);

					// Store error message
					if (string.IsNullOrEmpty(this.ImportSetting.ErrorMessages) == false)
					{
						errorMessages.Append(this.ImportSetting.ErrorMessages);
					}
				}

				// Store error message
				if (errorMessages.Length != 0)
				{
					SetErrorMessagesForExecution(errorMessages.ToString(), this.ImportSetting.ErrorOccurredIdInfo);
					errorCount++;
					if (errorCount >= 100)
					{
						this.ErrorMessagesForExecution.AppendLine("※エラーが100件以上あるため、取込処理を打ち切ります。");
						break;
					}
					continue;
				}

				// Store data after data conversion
				results.Add(this.ImportSetting.Data);
			}

			this.IsEndOfFile = (reader.Peek() == -1);
			return results;
		}

		/// <summary>
		/// Get row data
		/// </summary>
		/// <param name="reader">CSV text reader</param>
		/// <returns>Row data</returns>
		private string GetRowData(TextReader reader)
		{
			var rowData = new StringBuilder(reader.ReadLine());

			while (IsCompleteRow(rowData.ToString()) == false)
			{
				// Throw an error if While is still spinning after reaching the last line
				if (reader.Peek() == -1)
				{
					throw new w2Exception(
						string.Format(
							"{0}行目に含まれるダブルクオーテーション（\"）の数が奇数のため、正常に解析を行うことが出来ませんでした。",
							this.CurrentLine));
				}

				rowData.AppendLine(reader.ReadLine());
			}

			return rowData.ToString();
		}

		/// <summary>
		/// Is complete row
		/// </summary>
		/// <param name="rowData">Row data</param>
		/// <returns>Line completion flag</returns>
		private bool IsCompleteRow(string rowData)
		{
			// If there is a line break in the CSV line, "" "should be an odd number
			var result = (rowData.Count(item => (item == '"')) % 2 == 0);
			return result;
		}

		/// <summary>
		/// Get field names
		/// </summary>
		/// <param name="header">Header</param>
		/// <returns>Field name list</returns>
		private List<string> GetFieldNames(string header)
		{
			if (string.IsNullOrEmpty(header)) throw new w2Exception("ファイルの読取に失敗しました。");

			// Field split header line
			var fields = new List<string>(StringUtility.SplitCsvLine(header)).ConvertAll(item => item.ToLower());
			var newFields = CreateNameDuplicate(fields);
			return newFields;
		}

		/// <summary>
		/// Set error messages for execution
		/// </summary>
		/// <param name="errorMessages">Error messages</param>
		/// <param name="errorOccuredIdInfo">Error occured id info</param>
		protected void SetErrorMessagesForExecution(string errorMessages, string errorOccuredIdInfo = "")
		{
			if (this.ErrorMessagesForExecution.Length != 0)
			{
				this.ErrorMessagesForExecution.Append(Environment.NewLine);
			}

			if (this.CurrentLine > 0)
			{
				this.ErrorMessagesForExecution.AppendFormat(
					"(CSVファイルの{0}行目でエラー発生){1}",
					this.CurrentLine,
					Environment.NewLine);
			}

			if (string.IsNullOrEmpty(errorOccuredIdInfo) == false)
			{
				this.ErrorMessagesForExecution.AppendFormat(
					" {0}{1}",
					errorOccuredIdInfo,
					Environment.NewLine);
			}

			this.ErrorMessagesForExecution.AppendLine(errorMessages);
		}

		/// <summary>
		/// Set success message for execution
		/// </summary>
		/// <param name="fileName">File name</param>
		private void SetSuccessMessagesForExecution(string fileName)
		{
			if (this.SuccessMessagesForExecution.Length != 0)
			{
				this.SuccessMessagesForExecution.Append(Environment.NewLine);
			}

			this.SuccessMessagesForExecution
				.AppendFormat("取り込みファイル名：{0}", fileName)
				.AppendFormat("                   INS/UPD    ：{0}件", this.InsertUpdateCount);
		}

		/// <summary>
		/// Set error messages
		/// </summary>
		/// <param name="fileName">File name</param>
		private void SetErrorMessages(string fileName)
		{
			if (this.ErrorMessages.Length != 0)
			{
				this.ErrorMessages.Append(Environment.NewLine);
			}

			this.SuccessMessagesForExecution
				.AppendFormat("取り込みファイル名：{0}", fileName)
				.Append(Environment.NewLine)
				.AppendFormat("エラー：{0}", this.ErrorMessagesForExecution.ToString());
		}

		/// <summary>
		/// Get messages
		/// </summary>
		/// <returns>Messages</returns>
		private string GetMessages()
		{
			var messages = new StringBuilder()
				.AppendFormat("取込開始時間：{0}", this.StartDateTime)
				.Append(Environment.NewLine)
				.AppendFormat("取込終了時間：{0}", this.EndDateTime)
				.Append(Environment.NewLine);

			if (string.IsNullOrEmpty(this.SuccessMessagesForExecution.ToString()) == false)
			{
				messages.Append(Environment.NewLine)
					.Append(this.SuccessMessagesForExecution.ToString());
			}

			if (this.HasError)
			{
				messages.Append(Environment.NewLine)
					.Append(this.ErrorMessages.ToString());
			}

			messages.Append(Environment.NewLine);
			return messages.ToString();
		}

		/// <summary>
		/// Send mail to operator
		/// </summary>
		/// <param name="hasError">エラーを保持しているか</param>
		/// <param name="message">Message</param>
		private void SendMailToOperator(bool hasError, string message)
		{
			var input = new Hashtable
			{
				{ "result", hasError ? "失敗" : "成功" },
				{ "message", message },
				{ "file_name", this.DownloadFileName }
			};

			using (var mailSend = new MailSendUtility(
				Domain.Constants.CONST_DEFAULT_SHOP_ID,
				w2.App.Common.Constants.CONST_MAIL_ID_CROSSPOINT_COOPERATION_FOR_OPERATOR,
				string.Empty,
				input,
				true,
				w2.App.Common.Constants.MailSendMethod.Auto))
			{
				// 送信エラー？
				if (mailSend.SendMail() == false)
				{
					// バッチのエラーログに出力
					FileLogger.WriteError(mailSend.MailSendException);
				}
			}
		}

		/// <summary>
		/// CROSSPOINT連携ファイル取込エラー通知メール送信
		/// </summary>
		/// <param name="message">バッチ実行結果メッセージ</param>
		private void SendErrorMail(string message)
		{
			var inputForError = new Hashtable
			{
				{ "time_begin", this.StartDateTime },
				{ "time_end", this.EndDateTime },
				{ "execute_count", this.InsertUpdateCount },
				{ "message", message },
				{ "error_file_name", this.DownloadFileName }
			};

			using (var mailSend = new MailSendUtility(
				Domain.Constants.CONST_DEFAULT_SHOP_ID,
				w2.App.Common.Constants.CONST_MAIL_ID_CROSSPOINT_COOPERATION_ERROR_MAIL,
				string.Empty,
				inputForError,
				true,
				w2.App.Common.Constants.MailSendMethod.Auto))
			{
				// 送信エラー？
				if (mailSend.SendMail() == false)
				{
					// バッチのエラーログに出力
					FileLogger.WriteError(mailSend.MailSendException);
				}
			}
		}

		/// <summary>
		/// Create name duplicate
		/// </summary>
		/// <param name="listName">List name</param>
		/// <returns>List new name</returns>
		private List<string> CreateNameDuplicate(List<string> listName)
		{
			var result = listName.Take(1).ToList();

			for (var item = 1; item < listName.Count; item++)
			{
				var name = listName[item];
				var count = listName
					.Take(item - 1)
					.Where(itemName => (itemName == name))
					.Count() + 1;
				var newName = (count < 2)
					? name
					: name + count.ToString();
				result.Add(newName.ToLower());
			}

			return result;
		}

		/// <summary>
		/// Import data process
		/// </summary>
		/// <param name="filePath">File path</param>
		private void ImportDataProcess(string filePath)
		{
			// ファイルを読み取り終わったら終了(1000行ずつ読み込み)
			while (this.IsEndOfFile == false)
			{
				this.CurrentInputLine = this.CurrentLine;
				var inputs = GetInputs(filePath);
				if (this.HasErrorOfExecution)
				{
					FileLogger.WriteError(this.ErrorMessagesForExecution.ToString());
					return;
				}

				foreach (var input in inputs)
				{
					using (var accessor = new SqlAccessor())
					{
						try
						{
							// ユーザー1人単位でトランザクションを張る
							accessor.OpenConnection();
							accessor.BeginTransaction();

							var userModel = ConvertUserModel(input, accessor);
							if (userModel == null) continue;

							DomainFacade.Instance.UserService.UpdateWithUserExtend(userModel,
								UpdateHistoryAction.Insert, accessor);

							var userPoint =
								DomainFacade.Instance.PointService.GetUserPoint(userModel.UserId, string.Empty,
									accessor);
							if (userPoint.Length > 0)
							{
								var userPointModel = ConvertUserPointModel(input);

								DomainFacade.Instance.PointService.AdjustPointByCrossPoint(userPointModel, accessor);
							}
							accessor.CommitTransaction();
							this.CurrentInputLine++;
							this.InsertUpdateCount++;
						}
						catch
						{
							accessor.RollbackTransaction();
							throw;
						}
					}
				}
			}
		}

		/// <summary>
		/// Get inputs
		/// </summary>
		/// <param name="filePath">File path</param>
		/// <returns>Array of member input</returns>
		protected MemberInput[] GetInputs(string filePath)
		{
			var csvDataList = ReadAllDataCsvFile(filePath);
			var inputs = csvDataList
				.Select(item => new MemberInput(item))
				.ToArray();
			return inputs;
		}

		/// <summary>
		/// Convert user model
		/// </summary>
		/// <param name="input">Input</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>User model</returns>
		private UserModel ConvertUserModel(MemberInput input, SqlAccessor accessor)
		{
			var originalUser = DomainFacade.Instance.UserService.Get(
				StringUtility.ToEmpty(input.NetShopMemberIdShopCd).Trim()
				, accessor);

			// w2側にユーザーが存在しないかゲストの場合はnullを返す
			if ((originalUser == null) || UserService.IsGuest(originalUser.UserKbn))
			{
				return null;
			}

			var updateUser = originalUser.Clone();
			updateUser.UserExtend = new UserExtendModel(
				(Hashtable)originalUser.UserExtend.DataSource.Clone(),
				originalUser.UserExtend.UserExtendSettings);

			var zip = new ZipCode(StringUtility.ToEmpty(input.PostCode));
			var tel1 = new Tel(StringUtility.ToEmpty(input.Tel));
			var tel2 = new Tel(StringUtility.ToEmpty(input.MbTel));
			var birthday = new Func<DateTime?>(() =>
			{
				DateTime birth;
				if (DateTime.TryParse(StringUtility.ToEmpty(input.Birthday), out birth) == false) return null;
				return birth;
			})();
			updateUser.Name1 = StringUtility.ToEmpty(input.LastName).Trim();
			updateUser.Name2 = StringUtility.ToEmpty(input.FirstName).Trim();
			updateUser.Name = string.Format(
				"{0}{1}",
				updateUser.Name1,
				updateUser.Name2);
			updateUser.NameKana1 = StringUtility.ToEmpty(input.LastNameKana).Trim();
			updateUser.NameKana2 = StringUtility.ToEmpty(input.FirstNameKana).Trim();
			updateUser.NameKana = string.Format(
				"{0}{1}",
				updateUser.NameKana1,
				updateUser.NameKana2);
			updateUser.Sex = UserUtility.GetSystemGender(StringUtility.ToEmpty(input.Sex).Trim());
			if (birthday.HasValue)
			{
				updateUser.Birth = birthday;
				updateUser.BirthDay = birthday.Value.Day.ToString();
				updateUser.BirthMonth =  birthday.Value.Month.ToString();
				updateUser.BirthYear = birthday.Value.Year.ToString();
			}
			updateUser.Zip = StringUtility.ToEmpty(input.PostCode);
			updateUser.Zip1 = zip.Zip1;
			updateUser.Zip2 = zip.Zip2;
			updateUser.Addr1 = StringUtility.ToEmpty(input.PrefName).Trim();
			updateUser.Addr2 = StringUtility.ToEmpty(input.City).Trim();
			updateUser.Addr3 = string.Format(
				"{0}{1}",
				StringUtility.ToEmpty(input.Town).Trim(),
				StringUtility.ToEmpty(input.Address).Trim());
			updateUser.Addr4 = StringUtility.ToEmpty(input.Building).Trim();
			updateUser.Addr = string.Format(
				"{0}{1}{2}{3}",
				updateUser.Addr1,
				updateUser.Addr2,
				updateUser.Addr3,
				updateUser.Addr4);
			updateUser.Tel1 = StringUtility.ToEmpty(input.Tel);
			updateUser.Tel1_1 = tel1.Tel1;
			updateUser.Tel1_2 = tel1.Tel2;
			updateUser.Tel1_3 = tel1.Tel3;
			updateUser.Tel2 = StringUtility.ToEmpty(input.MbTel);
			updateUser.Tel2_1 = tel2.Tel1;
			updateUser.Tel2_2 = tel2.Tel2;
			updateUser.Tel2_3 = tel2.Tel3;
			updateUser.MailAddr = StringUtility.ToEmpty(input.PcMail);
			updateUser.MailAddr2 = StringUtility.ToEmpty(input.MbMail);
			updateUser.MemberRankId = StringUtility.ToEmpty(input.MemberRankId);
			updateUser.UserExtend.UserExtendDataValue.CrossPointShopCardNo = StringUtility.ToEmpty(input.RealShopCardNo);
			updateUser.UserExtend.UserExtendDataValue.CrossPointShopCardPin = StringUtility.ToEmpty(input.PinCd);
			updateUser.UserExtend.UserExtendDataValue.CrossPointAddShopName = StringUtility.ToEmpty(input.AdmissionShopName);
			updateUser.UserExtend.UserExtendDataValue.CrossPointDm =
				(input.PostcardDmUnnecessaryFlg == "必要") ? "1" : "0";
			updateUser.UserExtend.UserExtendDataValue.CrossPointMailFlg =
				(input.EmailDmUnnecessaryFlg == "必要") ? "1" : "0";
			return updateUser;
		}

		/// <summary>
		/// Convert user point model
		/// </summary>
		/// <param name="input">Input</param>
		/// <returns>User point model</returns>
		private UserPointModel ConvertUserPointModel(MemberInput input)
		{
			var model = new UserPointModel
			{
				UserId = StringUtility.ToEmpty(input.NetShopMemberIdShopCd),
				DeptId = w2.App.Common.Constants.W2MP_DEPT_ID,
				Point = decimal.Parse(StringUtility.ToEmpty(input.EffectivePoint)),
				LastChanged = w2.App.Common.Constants.FLG_LASTCHANGED_BATCH
			};

			return model;
		}

		/// <summary>
		/// Checker
		/// </summary>
		/// <param name="target">Target</param>
		public void Checker(string target)
		{
			var tempDirPath = Directories.TmpFilePath;
			if (Directory.Exists(tempDirPath) == false) Directory.CreateDirectory(tempDirPath);

			this.TempFilePath = Path.Combine(
				tempDirPath,
				string.Format("lastExec{0}.tmp", target));
		}

		/// <summary>
		/// Get last execute date time
		/// </summary>
		/// <returns>Last run time</returns>
		public DateTime? GetLastExecuteDateTime()
		{
			if (File.Exists(this.TempFilePath) == false) return null;
			return File.GetLastWriteTime(this.TempFilePath);
		}

		/// <summary>
		/// Create end file
		/// </summary>
		public void CreateEndFile()
		{
			CreateEndFile(DateTime.Now);
		}

		/// <summary>
		/// Create end file
		/// </summary>
		/// <param name="date">Date</param>
		public void CreateEndFile(DateTime date)
		{
			if (File.Exists(this.TempFilePath) == false) CreateTempFile();

			// Date update
			File.SetLastWriteTime(this.TempFilePath, date);
		}

		/// <summary>
		/// Create temp file
		/// </summary>
		private void CreateTempFile()
		{
			using (File.CreateText(this.TempFilePath))
			{
			}
		}

		/// <summary>Ftp client</summary>
		private FluentFtpUtility FtpClient { get; set; }
		/// <summary>Import Setting</summary>
		protected IImportSetting ImportSetting { get; set; }
		/// <summary>Import target</summary>
		protected string ImportTarget { get; set; }
		/// <summary>Suffix import file name</summary>
		protected string SuffixImportFileName { get; set; }
		/// <summary>Last execute date time</summary>
		private DateTime? LastExecuteDateTime { get; set; }
		/// <summary>Start date time</summary>
		private DateTime StartDateTime { get; set; }
		/// <summary>End date time</summary>
		private DateTime EndDateTime { get; set; }
		/// <summary>Error messages</summary>
		protected StringBuilder ErrorMessages { get; set; }
		/// <summary>Has error</summary>
		public bool HasError
		{
			get { return (string.IsNullOrEmpty(this.ErrorMessagesForExecution.ToString()) == false); }
		}
		/// <summary>現在の処理行数(XML読込時)</summary>
		protected int CurrentLine { get; set; }
		/// <summary>現在の処理行数(DB更新時)</summary>
		protected int CurrentInputLine { get; set; }
		/// <summary>次に停止する行</summary>
		private int NextStopLine { get; set; }
		/// <summary>ファイルを読み込み終わったか</summary>
		private bool IsEndOfFile { get; set; }
		/// <summary>Insert update count</summary>
		protected int InsertUpdateCount { get; set; }
		/// <summary>Success messages for execution</summary>
		protected StringBuilder SuccessMessagesForExecution { get; set; }
		/// <summary>Execution error messages</summary>
		protected StringBuilder ErrorMessagesForExecution { get; set; }
		/// <summary>Has error of execution</summary>
		public bool HasErrorOfExecution
		{
			get { return (string.IsNullOrEmpty(this.ErrorMessagesForExecution.ToString()) == false); }
		}
		/// <summary>Temp file path</summary>
		private string TempFilePath { get; set; }
		/// <summary>ダウンロードしようとしたcsvファイル名</summary>
		private string DownloadFileName { get; set; }

	}
}
