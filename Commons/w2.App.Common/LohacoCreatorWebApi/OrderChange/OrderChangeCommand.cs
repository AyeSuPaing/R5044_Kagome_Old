/*
=========================================================================================================
  Module      : 注文内容変更API OrderChangeのコマンドクラス(OrderChangeCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.LohacoCreatorWebApi.OrderChange
{
	/// <summary>
	/// 注文内容変更API OrderChangeのコマンドクラス
	/// </summary>
	public class OrderChangeCommand : BaseCommand<OrderChangeRequest, OrderChangeResponse>
	{
		#region +OnExecute 注文内容変更APIコマンドの処理
		/// <summary>
		/// 注文内容変更APIコマンドの処理
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <param name="sellerId">スタアアカウント</param>
		/// <param name="privateKey">秘密鍵</param>
		/// <param name="errorResponse">基礎エラーレスポンス情報（基礎エラーレスポンスがあれば場合）</param>
		/// <param name="isWriteDebugLogEnabled">デバッグログ出力かどうか</param>
		/// <param name="isMaskPersonalInfoEnabled">個人情報をマスクかどうか</param>
		/// <returns>レスポンス</returns>
		public override OrderChangeResponse OnExecute(
			OrderChangeRequest request,
			string sellerId,
			string privateKey,
			out BaseErrorResponse errorResponse,
			bool isWriteDebugLogEnabled = false,
			bool isMaskPersonalInfoEnabled = true)
		{
			return ExecuteWithSerialize(request, sellerId, privateKey, out errorResponse, isWriteDebugLogEnabled, isMaskPersonalInfoEnabled);
		}
		#endregion
	}
}
