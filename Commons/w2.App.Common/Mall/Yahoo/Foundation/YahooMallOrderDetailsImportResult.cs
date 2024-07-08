/*
=========================================================================================================
  Module      : YAHOO API Yahoo注文詳細取込結果 クラス(YahooMallOrderDetailsImportResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;

namespace w2.App.Common.Mall.Yahoo.Foundation
{
	/// <summary>
	/// YAHOO API Yahoo注文詳細取込結果 クラス
	/// </summary>
	public class YahooMallOrderDetailsImportResult
	{
		/// <summary>
		/// 総件数セット
		/// </summary>
		public void SetTotalCount(int total) => this.Total = total;
		
		/// <summary>
		/// 成功件数カウンターカウントアップ
		/// </summary>
		public void IncrementSuccessCount() => this.Success++;

		/// <summary>
		/// 警告件数カウンターカウントアップ
		/// </summary>
		public void IncrementWarningCount() => this.Warning++;

		/// <summary>
		/// 失敗件数カウンターカウントアップ
		/// </summary>
		public void IncrementFailureCount() => this.Failure++;

		/// <summary>
		/// 結果Stats生成
		/// </summary>
		/// <returns></returns>
		public string GenerateResultMessage() =>
			$"件数={this.Total},成功={this.Success},警告={this.Warning},失敗{this.Failure}";

		/// <summary>
		/// Eメール置換インプット生成
		/// </summary>
		/// <returns>Eメール置換インプット</returns>
		public Hashtable GenerateEmailBodyInput(string mallId)
		{
			var publicKeyAuthorizedMessage = (this.IsSuccessPublicKeyAuthorized == false)
				? "公開鍵認証に失敗しました。"
				: string.Empty;
			var result = new Hashtable
			{
				{ "mall_id", mallId },
				{ "total_count", this.Total.ToString() },
				{ "successful_count", this.Success.ToString() },
				{ "warning_count", this.Warning.ToString() },
				{ "failed_count", this.Failure.ToString() },
				{ "public_key_authorized_message", publicKeyAuthorizedMessage },
			};
			return result;
		}

		/// <summary>
		/// 公開鍵認証成功かをセット
		/// </summary>
		public void SetPublicKeyAuthorized(bool isSuccess) => this.IsSuccessPublicKeyAuthorized = isSuccess;

		/// <summary>
		/// エラーか警告があるかどうか
		/// </summary>
		/// <returns>エラーか警告があるかどうか</returns>
		public bool HasAnyErrorOrWarning() => (this.Failure > 0) || (this.Warning > 0);

		/// <summary>処理件数</summary>
		public int Total { get; private set; }
		/// <summary>成功件数</summary>
		public int Success { get; private set; }
		/// <summary>警告件数</summary>
		public int Warning { get; private set; }
		/// <summary>失敗件数</summary>
		public int Failure { get; private set; }
		/// <summary>公開鍵認証成功か</summary>
		public bool IsSuccessPublicKeyAuthorized { get; private set; }
	}
}
