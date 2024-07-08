/*
=========================================================================================================
  Module      : 注文検索API OrderListコマンドのクラス(OrderListCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.LohacoCreatorWebApi.OrderList
{
	/// <summary>
	/// 注文検索API OrderListコマンドのクラス
	/// </summary>
	public class OrderListCommand : BaseCommand<OrderListRequest, OrderListResponse>
	{
		#region +OnExecute 注文検索API OrderListコマンドの処理
		/// <summary>
		/// 注文検索API OrderListコマンドの処理
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <param name="sellerId">スタアアカウント</param>
		/// <param name="privateKey">秘密鍵</param>
		/// <param name="errorResponse">基礎エラーレスポンス情報（基礎エラーレスポンスがあれば場合）</param>
		/// <param name="isWriteDebugLogEnabled">デバッグログ出力かどうか</param>
		/// <param name="isMaskPersonalInfoEnabled">個人情報をマスクかどうか</param>
		/// <returns>レスポンス</returns>
		public override OrderListResponse OnExecute(
			OrderListRequest request,
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
