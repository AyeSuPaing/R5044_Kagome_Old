/*
=========================================================================================================
  Module      : DentalLab 受注情報入力クラス(ApiImportCommandBuilder_R1054_DentalLab_ImportOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.IO;
using w2.App.Common;
using w2.Common.Util;
using w2.Domain.MailTemplate;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Import;

namespace R1054_DentalLab.w2.Commerce.ExternalAPI
{
	/// <summary>
	/// DentalLab 受注情報入力クラス
	/// </summary>
	public class ApiImportCommandBuilder_R1054_DentalLab_ImportOrder : ApiImportCommandBuilder
	{
		#region #Import インポート処理の実装
		/// <summary>
		/// インポート処理の実装
		/// </summary>
		/// <param name="apiEntry">処理対象の情報を持つApiEntry</param>
		protected override void Import(ApiEntry apiEntry)
		{
			this.LinesCount++;

			// 「注文番号,配送伝票番号」形式以外の場合、抜ける
			if (apiEntry.Data.ItemArray.Length != 2)
			{
				this.ErrorMessage += string.Format("行{0}：{1}\r\n", this.LinesCount, CustomConstants.IMPORT_MAILTEMPLATE_FILE_FORMAT_ERRMSG);
				return;
			}

			try
			{
				//分割したデータを元にコマンド用引数クラス生成
				var shipOrderArg = GetArg(apiEntry);

				// コマンド実行
				var shipOrder = new ShipOrder();
				var apiCommandResult = shipOrder.Do(shipOrderArg);

				if (apiCommandResult.ResultStatus == EnumResultStatus.Complete)
				{
					this.UpdatedCount++;
				}
			}
			catch (Exception ex)
			{
				this.ErrorMessage += string.Format("{0}\r\n", ex.Message);
				throw ex;
			}
		}

		/// <summary>
		/// 引数設定
		/// </summary>
		/// <param name="apiEntry">インポート情報</param>
		/// <returns>引数</returns>
		private static ShipOrderArg GetArg(ApiEntry apiEntry)
		{
			var shipOrderArg = new ShipOrderArg
			{
				OrderId = apiEntry.Data[0].ToString(),
				ShippingNo = 1,
				ShippingCheckNo = apiEntry.Data[1].ToString(),
				DoesMail = true,
				ApiMemo = string.Empty,
				IsOverwriteMemo = false,
				ShippedDate = DateTime.Today
			};
			return shipOrderArg;
		}
		#endregion

		#region #ParepareImportFile インポート対象ファイルの準備処理
		/// <summary>
		/// インポート対象ファイルの準備処理
		/// </summary>
		/// <param name="importFilepath">準備予定のファイルパス</param>
		public override void ParepareImportFile(string importFilepath)
		{
			this.ImportFileName = Path.GetFileName(importFilepath);
		}
		#endregion

		#region #PostDo 実行後処理
		/// <summary>
		/// 実行後処理
		/// </summary>
		public override void PostDo()
		{
			var mailTemplate = new MailTemplateService().Get(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_EXTERNAL_IMPORT);

			if (mailTemplate == null) return;

			var resultMessage = CustomConstants.IMPORT_MAILTEMPLATE_RESULT_SUCCSESS_MSG
				.Replace("@@ 1 @@", StringUtility.ToNumeric(this.UpdatedCount))
				.Replace("@@ 2 @@", StringUtility.ToNumeric(this.LinesCount));

			if (string.IsNullOrEmpty(this.ErrorMessage) == false)
			{
				resultMessage += string.Format("\r\n\r\n{0}", "エラー内容：");
				resultMessage += string.Format("\r\n{0}", this.ErrorMessage);
			}

			var input = new Hashtable
			{
				{ CustomConstants.IMPORT_MAILTEMPLATE_KEY_FILE_TYPE, CustomConstants.IMPORT_MAILTEMPLATE_FILE_TYPE },
				{ CustomConstants.IMPORT_MAILTEMPLATE_KEY_FILE_NAME, this.ImportFileName },
				{ CustomConstants.IMPORT_MAILTEMPLATE_KEY_RESULT, ((string.IsNullOrEmpty(this.ErrorMessage)) ? CustomConstants.IMPORT_MAILTEMPLATE_RESULT_SUCCSESS : CustomConstants.IMPORT_MAILTEMPLATE_RESULT_FAIL) },
				{ CustomConstants.IMPORT_MAILTEMPLATE_KEY_MSG, resultMessage }
			};

			// メール送信
			using (var mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_EXTERNAL_IMPORT,
				string.Empty,
				input,
				true,
				Constants.MailSendMethod.Manual))
			{
				mailSender.SendMail();
			}
		}
		#endregion

		#region プロパティ
		/// <summary>更新件数</summary>
		private int UpdatedCount { get; set; }
		/// <summary>処理行数</summary>
		private int LinesCount { get; set; }
		/// <summary>取込ファイル名</summary>
		private string ImportFileName { get; set; }
		/// <summary>エラーメッセージ</summary>
		private string ErrorMessage { get; set; }
		#endregion
	}
}
