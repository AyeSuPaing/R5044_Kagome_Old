/*
=========================================================================================================
  Module      : お気に入り商品サービス (FavoriteService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;

namespace w2.Domain.Favorite
{
	/// <summary>
	/// お気に入り商品サービス
	/// </summary>
	public class FavoriteService : ServiceBase
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>モデル</returns>
		public FavoriteModel Get(
			string shopId,
			string userId,
			string productId,
			string variationId,
			SqlAccessor accessor = null)
		{
			using (var repository = new FavoriteRepository(accessor))
			{
				var model = repository.Get(shopId, userId, productId, variationId);

				return model;
			}
		}
		#endregion

		#region +GetFavoriteListAsDataView お気に入り一覧取得
		/// <summary>
		/// お気に入り一覧取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="pageNo">ページNo</param>
		/// <param name="dispContents">ページ内表示件数</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>お気に入り一覧</returns>
		/// <remarks>モデルで返すべきだが、それに伴う影響範囲が広いためDataViewで返す</remarks>
		public DataView GetFavoriteListAsDataView(string userId, int pageNo, int dispContents, SqlAccessor accessor = null)
		{
			using (var repository = new FavoriteRepository(accessor))
			{
				return repository.GetFavoriteListAsDataView(userId, pageNo, dispContents);
			}
		}
		#endregion

		#region +GetFavoriteByProduct 商品のお気に入り登録人数取得
		/// <summary>
		/// お気に入り登録数取得
		/// </summary>
		/// <param name="model">お気に入りモデル</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>お気に入り情報</returns>
		public int GetFavoriteByProduct(FavoriteModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FavoriteRepository(accessor))
			{
				return repository.GetFavoriteByProduct(model);
			}
		}
		#endregion

		#region +GetFavoriteCountByProduct お気に入り登録数取得
		/// <summary>
		/// お気に入り登録数取得
		/// </summary>
		/// <param name="model">お気に入りモデル</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>お気に入り情報</returns>
		public int GetFavoriteCountByProduct(FavoriteModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FavoriteRepository(accessor))
			{
				return repository.GetFavoriteByProductId(model);
			}
		}
		#endregion

		#region +GetFavoriteTotalByProduct お気に入り登録数取得
		/// <summary>
		/// 商品のお気に入り登録人数を全て取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productIds">商品ID配列</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>商品のお気に入り登録数合計</returns>
		public Dictionary<string, int> GetFavoriteTotalByProduct(string shopId, string[] productIds, SqlAccessor accessor = null)
		{
			using (var repository = new FavoriteRepository(accessor))
			{
				return repository.GetFavoriteTotalByProduct(shopId, productIds);
			}
		}
		#endregion

		#region GetFavoriteImage
		/// <summary>
		/// お気に入り商品画像URL取得
		/// </summary>
		/// <param name="model">お気に入りモデル</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>お気に入り商品画像URL</returns>
		public string GetFavoriteImage(FavoriteModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FavoriteRepository(accessor))
			{
				return repository.GetFavoriteImage(model);
			}
		}
		#endregion

		#region +GetStockAlertProducts 取得
		/// <summary>
		/// 在庫アラート商品取得
		/// </summary>
		/// <param name="stockThreshol">在庫閾値</param>
		/// <returns>ProductStockModelリスト</returns>
		public FavoriteModel[] GetStockAlertProducts(int stockThreshol)
		{
			using (var repository = new FavoriteRepository())
			{
				var models = repository.GetStockAlertProducts(stockThreshol);
				return models;
			}
		}
		#endregion

		#region GetSendTarget
		/// <summary>
		/// 対象在庫アラートメール情報取得
		/// </summary>
		/// <param name="accessor">アクセサ</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shopId">ショップID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリデーションID</param>
		/// <returns>対象在庫アラートメール情報</returns>
		public DataView GetSendTarget(string userId, string shopId, string productId, string variationId, SqlAccessor accessor = null)
		{
			using (var repository = new FavoriteRepository(accessor))
			{
				var result = string.IsNullOrEmpty(variationId)
					? repository.GetSendNoVariationTarget(shopId, productId)
					: repository.GetSendTarget(userId, shopId, productId, variationId);

				return result;
			}
		}
		#endregion

		#region +IsAlreadyRegisterFavorite 既にお気に入りにしているか
		/// <summary>
		/// 既にお気に入りにしているか
		/// </summary>
		/// <param name="model">お気に入りモデル</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>true: 登録澄み、false:未登録</returns>
		public bool IsAlreadyRegisterFavorite(FavoriteModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FavoriteRepository(accessor))
			{
				return repository.IsAlreadyRegisterFavorite(model);
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="accessor">アクセサ</param>
		/// <param name="model">モデル</param>
		public void Insert(FavoriteModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FavoriteRepository(accessor))
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
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(FavoriteModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FavoriteRepository(accessor))
			{
				return repository.Update(model);
			}
		}
		#endregion

		#region +BulkUpdateAlertSendFlg 更新
		/// <summary>
		/// アラート送信フラグ一斉更新
		/// </summary>
		/// <param name="alertSendFlg">アラート送信フラグ</param>
		/// <param name="threshold">閾値</param>
		public void BulkUpdateAlertSendFlg(string alertSendFlg, int threshold)
		{
			using (var repository = new FavoriteRepository())
			{
				repository.Accessor.BeginTransaction(IsolationLevel.ReadUncommitted);
				repository.BulkUpdateAlertSendFlg(alertSendFlg, threshold);
				repository.Accessor.CommitTransaction();
			}
		}
		#endregion

		#region +UpdateAlertSendFlg 更新
		/// <summary>
		/// アラート送信フラグ更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shopId">ショップID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="alertSendFlg">アラート送信フラグ</param>
		public void UpdateAlertSendFlg(string userId, string shopId, string productId, string variationId, string alertSendFlg)
		{
			using (var repository = new FavoriteRepository())
			{
				repository.UpdateAlertSendFlg(userId, shopId, productId, variationId, alertSendFlg);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(FavoriteModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FavoriteRepository(accessor))
			{
				return repository.Delete(model);
			}
		}
		#endregion
	}
}
