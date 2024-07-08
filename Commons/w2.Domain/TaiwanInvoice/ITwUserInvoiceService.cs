/*
=========================================================================================================
  Module      : Interface Taiwan User Invoice Service (ITwUserInvoiceService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.TwUserInvoice
{
	/// <summary>
	/// Interface Taiwan User Invoice Service
	/// </summary>
	public interface ITwUserInvoiceService : IService
	{
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="beginRowNum">開始行番号</param>
		/// <param name="endRowNum">終了行番号</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(string userId, int beginRowNum, int endRowNum);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="beginRowNum">開始行番号</param>
		/// <param name="endRowNum">終了行番号</param>
		/// <returns>検索結果列</returns>
		TwUserInvoiceModel[] Search(string userId, int beginRowNum, int endRowNum);

		/// <summary>
		/// Get All User Invoice By User Id
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>All User Invoice By User Id</returns>
		TwUserInvoiceModel[] GetAllUserInvoiceByUserId(string userId);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="twInvoiceNo">電子発票管理枝番</param>
		/// <returns>モデル</returns>
		TwUserInvoiceModel Get(string userId, int twInvoiceNo);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>登録されたユーザー配送先枝番</returns>
		int Insert(
			TwUserInvoiceModel model,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">Accessor</param>
		void Update(
			TwUserInvoiceModel model,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="twInvoiceNo">電子発票管理枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">Sql Accessor</param>
		void Delete(
			string userId,
			int twInvoiceNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);
	}
}
