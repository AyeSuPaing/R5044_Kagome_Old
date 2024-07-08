/*
=========================================================================================================
  Module      : 在庫管理API StockEditのコマンドクラス(StockEditCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.LohacoCreatorWebApi.StockEdit
{
	/// <summary>
	/// 在庫管理API StockEditのコマンドクラス
	/// </summary>
	public class StockEditCommand : BaseCommand<StockEditRequest, StockEditResponse>
	{
		#region +OnExecute 在庫管理API StockEditコマンドの処理
		/// <summary>
		/// 在庫管理API StockEditコマンドの処理
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <param name="sellerId">スタアアカウント</param>
		/// <param name="privateKey">秘密鍵</param>
		/// <param name="errorResponse">基礎エラーレスポンス情報（基礎エラーレスポンスがあれば場合）</param>
		/// <param name="isWriteDebugLogEnabled">デバッグログ出力かどうか</param>
		/// <param name="isMaskPersonalInfoEnabled">個人情報をマスクかどうか</param>
		/// <returns>レスポンス</returns>
		public override StockEditResponse OnExecute(
			StockEditRequest request,
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
