/*
=========================================================================================================
  Module      : お気に入り商品リポジトリ (FavoriteRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.Favorite
{
	/// <summary>
	/// お気に入り商品リポジトリ
	/// </summary>
	internal class FavoriteRepository : RepositoryBase
	{
		private const string XML_KEY_NAME = "Favorite";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal FavoriteRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal FavoriteRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <returns>モデル</returns>
		public FavoriteModel Get(string shopId, string userId, string productId, string variationId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FAVORITE_SHOP_ID, shopId },
				{ Constants.FIELD_FAVORITE_USER_ID, userId },
				{ Constants.FIELD_FAVORITE_PRODUCT_ID, productId },
				{ Constants.FIELD_FAVORITE_VARIATION_ID, variationId },
			};

			var dv = Get(XML_KEY_NAME, "Get", ht);
			return (dv.Count != 0) ? new FavoriteModel(dv[0]) : null;
		}
		#endregion

		#region +GetFavoriteListAsDataView お気に入り一覧取得
		/// <summary>
		/// お気に入り一覧取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="pageNumber">ページNo</param>
		/// <param name="dispContents">ページ内表示件数</param>
		/// <returns>お気に入り一覧</returns>
		/// <remarks>モデルで返すべきだが、それに伴う影響範囲が広いためDataViewで返す</remarks>
		public DataView GetFavoriteListAsDataView(string userId, int pageNumber, int dispContents)
		{
			var ht = new Hashtable()
			{
				{ Constants.FIELD_FAVORITE_USER_ID, userId },
				{ "current_date", DateTime.Now },
				{ Constants.FIELD_COMMON_END_NUM, dispContents * pageNumber },
				{ Constants.FIELD_COMMON_BEGIN_NUM, dispContents * (pageNumber - 1) + 1 },
			};

			var dv = Get(XML_KEY_NAME, "GetFavoriteList", ht);
			return dv;
		}
		#endregion

		#region +GetFavoriteByProduct 商品のお気に入り登録人数取得
		/// <summary>
		/// 商品のお気に入り登録人数取得
		/// </summary>
		/// <param name="model">お気に入りモデル</param>
		/// <returns>商品のお気に入り登録数</returns>
		public int GetFavoriteByProduct(FavoriteModel model)
		{
			var dv = Get(XML_KEY_NAME, "GetFavoriteCountOfProduct", model.DataSource);
			return (int)dv[0][0];
		}
		#endregion

		#region +GetFavoriteByProductId 商品IDでお気に入り登録数取得
		/// <summary>
		/// 商品IDでお気に入り情報取得
		/// </summary>
		/// <param name="model">お気に入りモデル</param>
		/// <returns>お気に入り登録数</returns>
		public int GetFavoriteByProductId(FavoriteModel model)
		{
			var dv = Get(XML_KEY_NAME, "GetFavoriteCountByProduct", model.DataSource);
			return (int)dv[0][0];
		}
		#endregion

		#region +GetFavoriteTotalByProduct 商品のお気に入り登録人数を全て取得
		/// <summary>
		/// 商品のお気に入り登録人数を全て取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productIds">商品ID配列</param>
		/// <returns>商品のお気に入り登録数合計</returns>
		public Dictionary<string, int> GetFavoriteTotalByProduct(string shopId, string[] productIds)
		{
			if (productIds.Any() == false) return new Dictionary<string, int>();
			var ht = new Hashtable()
			{
				{ Constants.FIELD_FAVORITE_SHOP_ID, shopId},
			};
			var replaces = new KeyValuePair<string, string>(
				"@@ product_ids @@",
				string.Join(",", productIds.Select(pid => String.Format("'{0}'", pid)).ToArray()));

			var dv = Get(XML_KEY_NAME, "GetFavoriteTotalByProduct", ht, replaces: replaces);
			return dv.Cast<DataRowView>().ToDictionary(drv => (string)drv["product_id"], drv => (int)drv["favorite_count"]);
		}
		#endregion

		#region GetFavoriteImage
		/// <summary>
		/// お気に入り商品画像URL取得
		/// </summary>
		/// <param name="model">お気に入りモデル</param>
		/// <returns>お気に入り商品画像URL</returns>
		public string GetFavoriteImage(FavoriteModel model)
		{
			var dv = Get(XML_KEY_NAME, "GetFavoriteImage", model.DataSource);
			return (string)dv[0][Constants.FIELD_FAVORITE_PRODUCT_IMAGE];
		}
		#endregion

		#region ~GetStockAlertProducts 取得
		/// <summary>
		/// 在庫アラート商品取得
		/// </summary>
		/// <param name="stockThreshol">在庫閾値</param>
		/// <returns>ProductStockModelリスト</returns>
		internal FavoriteModel[] GetStockAlertProducts(int stockThreshol)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCTSTOCK_STOCK, stockThreshol },
			};
			var dv = Get(XML_KEY_NAME, "GetStockAlertProducts", ht);

			var result = (dv.Count > 0) ? dv.Cast<DataRowView>().Select(drv => new FavoriteModel(drv)).ToArray() : Array.Empty<FavoriteModel>();
			return result;
		}
		#endregion

		#region GetFavoriteImage
		/// <summary>
		/// 対象在庫アラートメール情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shopId">ショップID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <returns>対象在庫アラートメール情報</returns>
		public DataView GetSendTarget(string userId, string shopId, string productId, string variationId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FAVORITE_USER_ID, userId },
				{ Constants.FIELD_FAVORITE_SHOP_ID, shopId },
				{ Constants.FIELD_FAVORITE_PRODUCT_ID, productId },
				{ Constants.FIELD_FAVORITE_VARIATION_ID, variationId }
			};
			var dv = Get(XML_KEY_NAME, "GetSendTarget", ht);
			return dv;
		}
		#endregion

		#region GetSendNoVariationTarget
		/// <summary>
		/// バリエーションなし対象在庫アラートメール情報取得
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="productId">商品ID</param>
		/// <returns>対象在庫アラートメール情報</returns>
		public DataView GetSendNoVariationTarget(string shopId, string productId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID,shopId },
				{ Constants.FIELD_PRODUCT_PRODUCT_ID, productId },
			};
			var dv = Get(XML_KEY_NAME, "GetSendNoVariationTarget", ht);
			return dv;
		}
		#endregion

		#region +IsAlreadyRegisterFavorite 既にお気に入りにしているか
		/// <summary>
		/// 既にお気に入りにしているか
		/// </summary>
		/// <param name="model">お気に入りモデル</param>
		/// <returns>true: 登録澄み、false:未登録</returns>
		public bool IsAlreadyRegisterFavorite(FavoriteModel model)
		{
			var dv = Get(XML_KEY_NAME, "GetFavorite", model.DataSource);
			return (dv.Count >= 1);
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(FavoriteModel model)
		{
			Exec(XML_KEY_NAME, "AddToFavorite", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(FavoriteModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +BulkUpdateAlertSendFlg 更新
		/// <summary>
		/// アラート送信フラグ一斉更新
		/// </summary>
		/// <param name="alertSendFlg">送信フラグ</param>
		/// <param name="threshold">閾値</param>
		public void BulkUpdateAlertSendFlg(string alertSendFlg, int threshold)
		{
			var ht = new Hashtable
			{
				{ "stock_alert_mail_threshold", threshold },
				{ Constants.FIELD_FAVORITE_ALERTMAIL_SEND_FLG, alertSendFlg }
			};
			Exec(XML_KEY_NAME, "BulkUpdateAlertSendFlg", ht);
		}
		#endregion

		#region +UpdateAlertSendFlg 更新
		/// <summary>
		/// アラート送信フラグ更新
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="shopId">ショップID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="alertSendFlg">送信フラグ</param>
		public void UpdateAlertSendFlg(string userId, string shopId, string productId, string variationId, string alertSendFlg)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FAVORITE_USER_ID, userId },
				{ Constants.FIELD_FAVORITE_SHOP_ID, shopId },
				{ Constants.FIELD_FAVORITE_PRODUCT_ID, productId },
				{ Constants.FIELD_FAVORITE_VARIATION_ID, variationId },
				{ Constants.FIELD_FAVORITE_ALERTMAIL_SEND_FLG, alertSendFlg },
			};

			Exec(XML_KEY_NAME, "UpdateAlertSendFlg", ht);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(FavoriteModel model)
		{
			var result = Exec(XML_KEY_NAME, "DeleteFromFavorite", model.DataSource);
			return result;
		}
		#endregion
	}
}
