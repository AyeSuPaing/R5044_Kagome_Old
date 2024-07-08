/*
=========================================================================================================
  Module      : ユーザー配送先情報サービスのインタフェース(IUserCreditCardService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.UserShipping
{
	/// <summary>
	/// ユーザー配送先情報サービスのインタフェース
	/// </summary>
	public interface IUserShippingService : IService
	{
		/// <summary>
		/// 検索（一覧）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="beginRowNum">開始行番号</param>
		/// <param name="endRowNum">終了行番号</param>
		/// <returns>モデル配列</returns>
		UserShippingModel[] Search(string userId, int beginRowNum, int endRowNum);

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="beginRowNum">開始行番号</param>
		/// <param name="endRowNum">終了行番号</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(string userId, int beginRowNum, int endRowNum);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="shippingNo">配送先枝番</param>
		/// <returns>モデル</returns>
		UserShippingModel Get(string userId, int shippingNo);

		/// <summary>
		/// 取得（全て）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル列</returns>
		UserShippingModel[] GetAllOrderByShippingNoDesc(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>登録されたユーザー配送先枝番</returns>
		int Insert(UserShippingModel model, string lastChanged, UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>登録されたユーザー配送先枝番</returns>
		int Insert(
			UserShippingModel model,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新されたか</returns>
		bool Update(UserShippingModel model, string lastChanged, UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="shippingNo">配送先枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="twoClickFlg">2クリック決済フラグ</param>
		/// <returns>更新されたか</returns>
		bool Delete(string userId, int shippingNo, string lastChanged, UpdateHistoryAction updateHistoryAction, bool twoClickFlg);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="shippingNo">配送先枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新されたか</returns>
		bool Delete(
			string userId,
			int shippingNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);
	}
}