/*
=========================================================================================================
  Module      : 注文詳細APIコマンドのクラス(OrderInfoCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.LohacoCreatorWebApi.OrderInfo
{
	/// <summary>
	/// 注文詳細APIコマンドのクラス
	/// </summary>
	public class OrderInfoCommand : BaseCommand<OrderInfoRequest, OrderInfoResponse>
	{
		#region +OnExecute 注文詳細APIコマンドの処理
		/// <summary>
		/// 注文詳細APIコマンドの処理
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <param name="sellerId">スタアアカウント</param>
		/// <param name="privateKey">秘密鍵</param>
		/// <param name="errorResponse">基礎エラーレスポンス情報（基礎エラーレスポンスがあれば場合）</param>
		/// <param name="isWriteDebugLogEnabled">デバッグログ出力かどうか</param>
		/// <param name="isMaskPersonalInfoEnabled">個人情報をマスクかどうか</param>
		/// <returns>レスポンス</returns>
		public override OrderInfoResponse OnExecute(
			OrderInfoRequest request,
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
