/*
=========================================================================================================
  Module      : 定期購入継続分析テーブルサービスのインタフェース(IFixedPurchaseRepeatAnalysisService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.FixedPurchaseRepeatAnalysis
{
	/// <summary>
	/// 定期購入継続分析テーブルサービスのインタフェース
	/// </summary>
	public interface IFixedPurchaseRepeatAnalysisService : IService
	{
		/// <summary>
		/// ユーザーIDで定期継続分析取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		FixedPurchaseRepeatAnalysisModel[] GetRepeatAnalysisByUserId(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// 注文IDで定期継続分析取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>モデル</returns>
		FixedPurchaseRepeatAnalysisModel[] GetRepeatAnalysisByOrderId(string orderId, SqlAccessor accessor);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		void Insert(FixedPurchaseRepeatAnalysisModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		int Update(FixedPurchaseRepeatAnalysisModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="dataNo">データ番号</param>
		/// <param name="accessor">SQLアクセサ</param>
		void Delete(long dataNo, SqlAccessor accessor = null);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>影響を受けた行数</returns>
		int DeleteByOrder(string orderId, string lastChanged, SqlAccessor accessor);

		/// <summary>
		/// 定期台帳IDをもとに削除
		/// </summary>
		/// <param name="fixedPurchaseId">注文ID</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>影響を受けた行数</returns>
		int DeleteByFixedPurchaseId(string fixedPurchaseId, SqlAccessor accessor);

		/// <summary>
		/// 最大購入回数
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>購入回数のモデル</returns>
		FixedPurchaseRepeatAnalysisModel GetRepeatAnalysisMaxCountByUserProduct(string userId, string productId, string variationId, SqlAccessor accessor);

		/// <summary>
		/// 定期継続分析更新(定期注文ID)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセッサ</param>
		void UpdateRepeatAnalysisFixedPurchaseIdByOrderId(string orderId, string fixedPurchaseId, string lastChanged, SqlAccessor accessor);

		/// <summary>
		/// 定期継続分析更新(ステータス)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="status">ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセッサ</param>
		void UpdateRepeatAnalysisStatusByOrderId(string orderId, string status, string lastChanged, SqlAccessor accessor);

		/// <summary>
		/// 定期商品登録
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセッサ</param>
		void RegistFixedpurchaseItem(string userId, string productId, string variationId, string fixedPurchaseId, string lastChanged, SqlAccessor accessor);

		/// <summary>
		/// 定期購入継続分析削除（注文）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセッサ</param>
		void DeleteAnalysisOrder(string userId, string productId, string variationId, string orderId, string lastChanged, SqlAccessor accessor);

		/// <summary>
		/// 定期商品登録
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセッサ</param>
		void ModifyOrderItem(string userId, string productId, string variationId, string orderId, string fixedPurchaseId, string lastChanged, SqlAccessor accessor);

		/// <summary>
		/// 定期商品離脱
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセッサ</param>
		void FallOutFixedpurchaseItem(string userId, string productId, string variationId, string fixedPurchaseId, string lastChanged, SqlAccessor accessor);

		/// <summary>
		/// 定期商品離脱（全商品）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセッサ</param>
		void FallOutFixedPurchaseAllItem(string userId, string fixedPurchaseId, string lastChanged, SqlAccessor accessor);
	}
}