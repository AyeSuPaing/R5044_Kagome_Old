/*
=========================================================================================================
  Module      : e-SCOTT レスポンスベースクラス(EScottResponseBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace w2.App.Common.Order.Payment.EScott.DataSchema
{
	/// <summary>
	/// レスポンスベースクラス
	/// </summary>
	public abstract class EScottResponseBase
	{
		/// <summary>
		/// 成功かどうか
		/// </summary>
		/// <param name="approved">承認の文字列</param>
		/// <returns>承認成功か失敗</returns>
		protected bool IsSuccessRequest(string approved)
		{
			var result = EScottConstants.REQUEST_APPROVED == approved;
			return result;
		}

		/// <summary>
		/// トランザクションID生成
		/// </summary>
		/// <param name="processId">プロセスID</param>
		/// <param name="processPassword">プロセスパスワード</param>
		/// <param name="kaiinId">会員ID</param>
		/// <param name="kaiinPass">会員パスワード</param>
		/// <returns>トランザクションID</returns>
		protected string GenerateTransactionId(string processId, string processPassword, string kaiinId, string kaiinPass)
		{
			var result = kaiinId + "," + processId + "," + processPassword + "," + kaiinPass;
			return result;
		}

		/// <summary>
		/// e-SCOTTレスポンスコードから詳細情報取得
		/// </summary>
		/// <param name="responseCode">レスポンスコード</param>
		/// <returns>エラーメッセージ</returns>
		protected string GetEScottResponseCodeDetails(string responseCode)
		{
			var responseCdList = responseCode.Split('|');
			var messageList = new StringBuilder();
			foreach (var resCd in responseCdList)
			{
				messageList.Append(GetMessage(Properties.Resources.EScottResponseCodeDetails, resCd));
			}

			var errorMessage = messageList.ToString();
			return errorMessage;
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="messageXmlString">メッセージXML文字列</param>
		/// <param name="code">コード</param>
		/// <returns>エラーメッセージ</returns>
		private string GetMessage(string messageXmlString, string code)
		{
			if (string.IsNullOrEmpty(code)) return "";

			var document = XDocument.Parse(messageXmlString);
			var errorMessage = document.Root.Elements("Message")
				.Where(e => e.Attributes("code").First().Value == code)
				.Select(e => e.Value).FirstOrDefault();

			if (errorMessage == null)
			{
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					"",
					PaymentFileLogger.PaymentType.EScott,
					PaymentFileLogger.PaymentProcessingType.GetErrorMessage,
					"エラーメッセージが変換できませんでした。");
			}

			return errorMessage;
		}
	}
}
