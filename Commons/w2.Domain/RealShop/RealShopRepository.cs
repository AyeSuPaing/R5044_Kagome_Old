/*
=========================================================================================================
Module      : リアル店舗情報リポジトリ (RealShopRepository.cs)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.RealShop
{
	/// <summary>
	/// リアル店舗情報リポジトリ
	/// </summary>
	internal class RealShopRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "RealShop";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal RealShopRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal RealShopRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion
		/*
		#region ~GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		internal int GetSearchHitCount(RealShopListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion
		*/
		/*
		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal RealShopListSearchResult[] Search(RealShopListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new RealShopListSearchResult(drv)).ToArray();
		}
		#endregion
		*/

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="realShopId">リアル店舗ID</param>
		/// <returns>モデル</returns>
		internal RealShopModel Get(string realShopId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_REALSHOP_REAL_SHOP_ID, realShopId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new RealShopModel(dv[0]);
		}
		#endregion


		#region ~GetAll 全取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>モデル</returns>
		internal RealShopModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll");
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new RealShopModel(drv)).ToArray();
		}
		#endregion


		/*
		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(RealShopModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion
		*/
		/*
		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(RealShopModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion
		*/
		/*
		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="realShopId">リアル店舗ID</param>
		internal int Delete(string realShopId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_REALSHOP_REAL_SHOP_ID, realShopId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion
		*/

		#region ~GetRealShopProductStockTopForPreview 先頭のリアル店舗商品在庫取得 (プレビュー用)
		/// <summary>
		/// 先頭のリアル店舗商品在庫取得 (プレビュー用)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>先頭のリアル店舗商品在庫取得 (プレビュー用)</returns>
		internal RealShopProductStockModel GetRealShopProductStockTopForPreview(string shopId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId }
			};
			var dv = Get(XML_KEY_NAME, "GetRealShopProductStockTopForPreview", ht);
			return (dv.Count > 0) ? new RealShopProductStockModel(dv[0]) : null;
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, "GetRealShopMaster", input, replaces);
			return reader;
		}

		/// <summary>
		/// マスタ取得（Excel出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>DataView</returns>
		public DataView GetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, "GetRealShopMaster", input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		public void CheckRealShopFieldsForGetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			Get(XML_KEY_NAME, "CheckRealShopFields", input, replaces: replaces);
		}
		#endregion

		#region +GetRealShops
		/// <summary>
		/// Get real shops
		/// </summary>
		/// <param name="areaId">Area id</param>
		/// <param name="brandId">Brand id</param>
		/// <returns>Array of real shop model</returns>
		internal RealShopModel[] GetRealShops(string areaId, string brandId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_REALSHOP_AREA_ID, areaId },
				{ Constants.FIELD_REALSHOP_BRAND_ID, brandId },
			};

			var realShops = Get(XML_KEY_NAME, "GetRealShops", input);
			var result = realShops
				.Cast<DataRowView>()
				.Select(realShop => new RealShopModel(realShop))
				.ToArray();

			return result;
		}
		#endregion

		#region +GetRealShops
		/// <summary>
		/// Get real shops
		/// </summary>
		/// <param name="areaId">Area id</param>
		/// <param name="brandId">Brand id</param>
		/// <returns>Array of real shop model</returns>
		internal RealShopModel[] GetRealShopsByAddr1(string addr1, string brandId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_REALSHOP_ADDR1, addr1 },
				{ Constants.FIELD_REALSHOP_BRAND_ID, brandId },
			};

			var realShops = Get(XML_KEY_NAME, "GetRealShopsByAddr1", input);
			var result = realShops
				.Cast<DataRowView>()
				.Select(realShop => new RealShopModel(realShop))
				.ToArray();

			return result;
		}
		#endregion
	}
}
