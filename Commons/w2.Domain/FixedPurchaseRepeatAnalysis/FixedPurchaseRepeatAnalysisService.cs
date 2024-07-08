/*
=========================================================================================================
  Module      : 定期購入継続分析テーブルサービス (FixedPurchaseRepeatAnalysisService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Transactions;
using w2.Common.Sql;

namespace w2.Domain.FixedPurchaseRepeatAnalysis
{
	/// <summary>
	/// 定期購入継続分析テーブルサービス
	/// </summary>
	public class FixedPurchaseRepeatAnalysisService : ServiceBase, IFixedPurchaseRepeatAnalysisService
	{
		#region +GetRepeatAnalysisByUserId ユーザーIDで定期継続分析取得
		/// <summary>
		/// ユーザーIDで定期継続分析取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public FixedPurchaseRepeatAnalysisModel[] GetRepeatAnalysisByUserId(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepeatAnalysisRepository(accessor))
			{
				var items = repository.GetRepeatAnalysisByUser(userId);
				return items;
			}
		}
		#endregion

		#region +GetRepeatAnalysisByOrderId 注文IDで定期継続分析取得
		/// <summary>
		/// 注文IDで定期継続分析取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>モデル</returns>
		public FixedPurchaseRepeatAnalysisModel[] GetRepeatAnalysisByOrderId(string orderId, SqlAccessor accessor)
		{
			using (var repository = new FixedPurchaseRepeatAnalysisRepository(accessor))
			{
				var result = repository.GetRepeatAnalysisByOrderId(orderId);
				return result;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(FixedPurchaseRepeatAnalysisModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepeatAnalysisRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int Update(FixedPurchaseRepeatAnalysisModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepeatAnalysisRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="dataNo">データ番号</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(long dataNo, SqlAccessor accessor = null)
		{
			using (var repository = new FixedPurchaseRepeatAnalysisRepository(accessor))
			{
				var result = repository.Delete(dataNo);
			}
		}
		#endregion

		#region +DeleteByOrder 注文をもとに削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>影響を受けた行数</returns>
		public int DeleteByOrder(string orderId, string lastChanged, SqlAccessor accessor)
		{
			var result = 0;
			using (var repository = new FixedPurchaseRepeatAnalysisRepository(accessor))
			{
				var models = repository.GetRepeatAnalysisByOrderId(orderId);
				foreach (var model in models)
				{
					DeleteAnalysisOrder(model.UserId, model.ProductId, model.VariationId, model.OrderId, lastChanged, accessor);
				}

				result = models.Length;
			}

			return result;
		}
		#endregion

		#region +DeleteByFixedPurchaseId 定期台帳IDをもとに削除
		/// <summary>
		/// 定期台帳IDをもとに削除
		/// </summary>
		/// <param name="fixedPurchaseId">注文ID</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>影響を受けた行数</returns>
		public int DeleteByFixedPurchaseId(string fixedPurchaseId, SqlAccessor accessor)
		{
			using (var repository = new FixedPurchaseRepeatAnalysisRepository(accessor))
			{
				return repository.DeleteByFixedPurchaseId(fixedPurchaseId);
			}
		}
		#endregion

		#region +GetRepeatAnalysisMaxCountByUserProduct
		/// <summary>
		/// 最大購入回数
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>購入回数のモデル</returns>
		public FixedPurchaseRepeatAnalysisModel GetRepeatAnalysisMaxCountByUserProduct(string userId, string productId, string variationId, SqlAccessor accessor)
		{
			using (var repository = new FixedPurchaseRepeatAnalysisRepository(accessor))
			{
				var items = repository.GetRepeatAnalysisByUserProduct(userId, productId, variationId);
				var list = items.Where(item => (item.IsFallOut == false)).OrderByDescending(item => item.Count);

				if (items.Any(item => (item.IsFallOut == false)))
				{
					var first = list.Cast<FixedPurchaseRepeatAnalysisModel>().ToArray().FirstOrDefault();
					return first ?? new FixedPurchaseRepeatAnalysisModel();
				}
				return new FixedPurchaseRepeatAnalysisModel();
			}
		}
		#endregion

		#region +UpdateRepeatAnalysisFixedPurchaseIdByOrderId 定期継続分析更新(定期注文ID)
		/// <summary>
		/// 定期継続分析更新(定期注文ID)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセッサ</param>
		public void UpdateRepeatAnalysisFixedPurchaseIdByOrderId(string orderId, string fixedPurchaseId, string lastChanged, SqlAccessor accessor)
		{
			using (var repository = new FixedPurchaseRepeatAnalysisRepository(accessor))
			{
				var models = repository.GetRepeatAnalysisByOrderId(orderId);
				foreach (var model in models)
				{
					model.FixedPurchaseId = fixedPurchaseId;
					model.LastChanged = lastChanged;
					repository.Update(model);
				}
			}
		}
		#endregion

		#region +UpdateRepeatAnalysisStatusByOrderId 定期継続分析更新(ステータス)
		/// <summary>
		/// 定期継続分析更新(ステータス)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="status">ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセッサ</param>
		public void UpdateRepeatAnalysisStatusByOrderId(string orderId, string status, string lastChanged, SqlAccessor accessor)
		{
			using (var repository = new FixedPurchaseRepeatAnalysisRepository(accessor))
			{
				var models = repository.GetRepeatAnalysisByOrderId(orderId);
				foreach (var model in models)
				{
					model.Status = status;
					model.LastChanged = lastChanged;
					repository.Update(model);
				}
			}
		}
		#endregion

		#region +RegistFixedpurchaseItem 定期商品登録
		/// <summary>
		/// 定期商品登録
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセッサ</param>
		public void RegistFixedpurchaseItem(string userId, string productId, string variationId, string fixedPurchaseId, string lastChanged, SqlAccessor accessor)
		{
			using (var repository = new FixedPurchaseRepeatAnalysisRepository(accessor))
			{
				var items = repository.GetRepeatAnalysisByUserProduct(userId, productId, variationId);

				// 離脱がある場合
				if (items.Any(item => (item.Status == Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_FALLOUT)))
				{
					var first = items.First(item => (item.Status == Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_FALLOUT));

					repository.Delete(first.DataNo);
				}

				var model = new FixedPurchaseRepeatAnalysisModel
				{
					UserId = userId,
					ProductId = productId,
					VariationId = variationId,
					Count = 0,
					OrderId = "",
					FixedPurchaseId = fixedPurchaseId,
					Status = Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_EXISTS,
					LastChanged = lastChanged,
				};
				repository.Insert(model);
			}
		}
		#endregion

		#region +DeleteAnalysisOrder 定期購入継続分析削除（注文）
		/// <summary>
		/// 定期購入継続分析削除（注文）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセッサ</param>
		public void DeleteAnalysisOrder(string userId, string productId, string variationId, string orderId, string lastChanged, SqlAccessor accessor)
		{
			using (var repository = new FixedPurchaseRepeatAnalysisRepository(accessor))
			{
				var items = repository.GetRepeatAnalysisByUserProduct(userId, productId, variationId);

				var data = items.FirstOrDefault(item => (item.OrderId == orderId));

				if (data != null)
				{
					repository.Delete(data.DataNo);

					foreach (var item in items.Where(item => ((item.Count >= data.Count) && (item.DataNo != data.DataNo))))
					{
						item.Count--;
						item.LastChanged = lastChanged;
						repository.Update(item);
					}
				}
			}
		}
		#endregion

		#region +ModifyOrderItem 注文商品変更
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
		public void ModifyOrderItem(string userId, string productId, string variationId, string orderId, string fixedPurchaseId, string lastChanged, SqlAccessor accessor)
		{
			using (var repository = new FixedPurchaseRepeatAnalysisRepository(accessor))
			{
				var items = repository.GetRepeatAnalysisByUserProduct(userId, productId, variationId);

				if (items.Any(item => (item.OrderId == orderId))) return;

				if (items.Any(item => ((item.Status == Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_EXISTS) && (item.FixedPurchaseId == fixedPurchaseId))))
				{
					var count = (items.Where(item => (item.Status != Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_EXISTS)).OrderByDescending(item => item.Count).FirstOrDefault() ?? new FixedPurchaseRepeatAnalysisModel()).Count + 1;
					var model = new FixedPurchaseRepeatAnalysisModel
					{
						UserId = userId,
						ProductId = productId,
						VariationId = variationId,
						Count = count,
						OrderId = orderId,
						FixedPurchaseId = fixedPurchaseId,
						Status = Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_ORDER,
						LastChanged = lastChanged,
					};
					repository.Insert(model);
				}
			}
		}
		#endregion

		#region +FallOutFixedpurchaseItem 定期商品離脱
		/// <summary>
		/// 定期商品離脱
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセッサ</param>
		public void FallOutFixedpurchaseItem(string userId, string productId, string variationId, string fixedPurchaseId, string lastChanged, SqlAccessor accessor)
		{
			using (var repository = new FixedPurchaseRepeatAnalysisRepository(accessor))
			{
				var items = repository.GetRepeatAnalysisByUserProduct(userId, productId, variationId);
				var first = items.FirstOrDefault(item => ((item.Status == Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_EXISTS) && (item.FixedPurchaseId == fixedPurchaseId)));

				if (first == null) return;

				repository.Delete(first.DataNo);

				// 他の定期に商品がない場合、一度でも買っている場合
				if ((items.Any(item => ((item.Status == Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_EXISTS) && (item.DataNo != first.DataNo))) == false)
					&& (items.Any(item => ((item.Status == Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_ORDER) || (item.Status == Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_DELIVERED)))))
				{
					var model = new FixedPurchaseRepeatAnalysisModel
					{
						UserId = first.UserId,
						ProductId = first.ProductId,
						VariationId = first.VariationId,
						Count = GetRepeatAnalysisMaxCountByUserProduct(userId, productId, variationId, accessor).Count,
						OrderId = "",
						FixedPurchaseId = "",
						Status = Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_FALLOUT,
						LastChanged = lastChanged,
					};
					repository.Insert(model);
				}
			}
		}
		#endregion

		#region +FallOutFixedPurchaseAllItem 定期商品離脱（全商品）
		/// <summary>
		/// 定期商品離脱（全商品）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセッサ</param>
		public void FallOutFixedPurchaseAllItem(string userId, string fixedPurchaseId, string lastChanged, SqlAccessor accessor)
		{
			using (var repository = new FixedPurchaseRepeatAnalysisRepository(accessor))
			{
				var items = repository.GetRepeatAnalysisByUser(userId);
				foreach (var item in items.Where(item => ((item.FixedPurchaseId == fixedPurchaseId) && (item.Status == Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_EXISTS))))
				{
					this.FallOutFixedpurchaseItem(item.UserId, item.ProductId, item.VariationId, item.FixedPurchaseId, lastChanged, accessor);
				}
			}
		}
		#endregion
	}
}
