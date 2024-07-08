/*
=========================================================================================================
  Module      : 定期商品変更設定リポジトリ (FixedPurchaseProductChangeSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.FixedPurchaseProductChangeSetting.Helper;

namespace w2.Domain.FixedPurchaseProductChangeSetting
{
	/// <summary>
	/// 定期商品変更設定リポジトリ
	/// </summary>
	public class FixedPurchaseProductChangeSettingRepository : RepositoryBase
	{
		/// <summary>キー名</summary>
		private const string XML_KEY_NAME = "FixedPurchaseProductChangeSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseProductChangeSettingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FixedPurchaseProductChangeSettingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(FixedPurchaseProductChangeSettingListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		public FixedPurchaseProductChangeSettingListSearchResult[] Search(FixedPurchaseProductChangeSettingListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseProductChangeSettingListSearchResult(drv)).ToArray();
		}
		#endregion

		#region +GetByFixedPurchaseProductChangeId 取得：定期商品変更ID
		/// <summary>
		/// 取得：定期商品変更ID
		/// </summary>
		/// <param name="fixedPurchaseProductChangeId">定期商品変更ID</param>
		/// <returns>定期商品変更設定モデル</returns>
		public FixedPurchaseProductChangeSettingModel GetByFixedPurchaseProductChangeId(string fixedPurchaseProductChangeId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID, fixedPurchaseProductChangeId },
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;

			return new FixedPurchaseProductChangeSettingModel(dv[0]);
		}
		#endregion

		#region +GetByFixedPurchaseProductChangeIds 取得：複数定期商品変更ID
		/// <summary>
		/// 取得：複数定期商品変更ID
		/// </summary>
		/// <param name="fixedPurchaseProductChangeIds">定期商品変更ID</param>
		/// <returns>定期商品変更設定モデル</returns>
		public FixedPurchaseProductChangeSettingModel GetByFixedPurchaseProductChangeIds(string[] fixedPurchaseProductChangeIds)
		{
			var ht = new Hashtable();

			var joinedIds = string.Join(
				",",
				fixedPurchaseProductChangeIds.Select(
					item => string.Format("'{0}'", item)));
			var replaceTag = new[]
			{
				new KeyValuePair<string, string>("@@ fixed_purchase_product_change_ids @@", joinedIds)
			};
			var dv = Get(XML_KEY_NAME, "GetByFixedPurchaseProductChangeIds", ht, dymamicParametersTuple: null, replaceTag);
			if (dv.Count == 0) return null;

			return new FixedPurchaseProductChangeSettingModel(dv[0]);
		}
		#endregion

		#region +GetBeforeChangeItems 変更元定期商品取得
		/// <summary>
		/// 定期変更元商品取得
		/// </summary>
		/// <param name="fixedPurchaseProductChangeId">定期商品変更ID</param>
		/// <returns>定期変更元商品</returns>
		public FixedPurchaseBeforeChangeItemModel[] GetBeforeChangeItems(string fixedPurchaseProductChangeId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_FIXED_PURCHASE_PRODUCT_CHANGE_ID, fixedPurchaseProductChangeId },
			};
			var dv = Get(XML_KEY_NAME, "GetBeforeChangeItems", ht);
			if (dv.Count == 0) return null;

			var model = dv.Cast<DataRowView>().Select(drv => new FixedPurchaseBeforeChangeItemModel(drv)).ToArray();
			return model;
		}
		#endregion

		#region +GetAfterChangeItems 変更後定期商品取得
		/// <summary>
		/// 定期変更後商品取得
		/// </summary>
		/// <param name="fixedPurchaseProductChangeId">定期商品変更ID</param>
		/// <returns>定期変更後商品</returns>
		public FixedPurchaseAfterChangeItemModel[] GetAfterChangeItems(string fixedPurchaseProductChangeId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_FIXED_PURCHASE_PRODUCT_CHANGE_ID, fixedPurchaseProductChangeId },
			};
			var dv = Get(XML_KEY_NAME, "GetAfterChangeItems", ht);
			if (dv.Count == 0) return null;

			var model = dv.Cast<DataRowView>().Select(drv => new FixedPurchaseAfterChangeItemModel(drv)).ToArray();
			return model;
		}
		#endregion

		#region +GetBeforeChangeItemsByProductId 変更元定期商品取得：商品ID
		/// <summary>
		/// 変更元定期商品取得：商品ID
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="shopId">店舗ID</param>
		/// <returns>定期変更元商品</returns>
		public FixedPurchaseBeforeChangeItemModel[] GetBeforeChangeItemsByProductId(string productId, string variationId, string shopId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_PRODUCT_ID, productId },
				{ Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_VARIATION_ID, variationId },
				{ Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_SHOP_ID, shopId },
			};
			var dv = Get(XML_KEY_NAME, "GetBeforeChangeItemsByProductId", ht);
			if (dv.Count == 0) return null;

			var model = dv.Cast<DataRowView>().Select(drv => new FixedPurchaseBeforeChangeItemModel(drv)).ToArray();
			return model;
		}
		#endregion

		#region +GetContainer
		/// <summary>
		/// 表示用定期商品変更設定モデル取得
		/// </summary>
		/// <param name="fixedPurchaseProductChangeId">定期商品変更ID</param>
		/// <returns>表示用定期商品変更設定モデル</returns>
		public FixedPurchaseProductChangeSettingContainer GetContainer(string fixedPurchaseProductChangeId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID, fixedPurchaseProductChangeId },
			};
			var dv = Get(XML_KEY_NAME, "GetContainer", ht);
			if (dv.Count == 0) return null;

			return new FixedPurchaseProductChangeSettingContainer(dv[0]);
		}
		#endregion

		#region +GetBeforeChangeItemContainers 表示用定期変更元商品取得
		/// <summary>
		/// 表示用定期変更元商品取得
		/// </summary>
		/// <param name="fixedPurchaseProductChangeId">定期商品変更ID</param>
		/// <returns>定期変更元商品</returns>
		public FixedPurchaseBeforeChangeItemContainer[] GetBeforeChangeItemContainers(string fixedPurchaseProductChangeId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_FIXED_PURCHASE_PRODUCT_CHANGE_ID, fixedPurchaseProductChangeId },
			};
			var dv = Get(XML_KEY_NAME, "GetBeforeChangeItemContainers", ht);
			if (dv.Count == 0) return null;

			var model = dv.Cast<DataRowView>().Select(drv => new FixedPurchaseBeforeChangeItemContainer(drv)).ToArray();
			return model;
		}
		#endregion

		#region +GetBeforeChangeItemContainers 表示用定期変更後商品取得
		/// <summary>
		/// 表示用定期変更後商品取得
		/// </summary>
		/// <param name="fixedPurchaseProductChangeId">定期商品変更ID</param>
		/// <returns>定期変更元商品</returns>
		public FixedPurchaseAfterChangeItemContainer[] GetAfterChangeItemContainers(string fixedPurchaseProductChangeId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_FIXED_PURCHASE_PRODUCT_CHANGE_ID, fixedPurchaseProductChangeId },
			};
			var dv = Get(XML_KEY_NAME, "GetAfterChangeItemContainers", ht);
			if (dv.Count == 0) return null;

			var model = dv.Cast<DataRowView>().Select(drv => new FixedPurchaseAfterChangeItemContainer(drv)).ToArray();
			return model;
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">定期商品変更設定モデル</param>
		public void Insert(FixedPurchaseProductChangeSettingModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +InsertBeforeChangeItem 定期変更元商品登録
		/// <summary>
		/// 定期変更元商品登録
		/// </summary>
		/// <param name="model">定期変更元商品モデル</param>
		public void InsertBeforeChangeItem(FixedPurchaseBeforeChangeItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertBeforeChangeItem", model.DataSource);
		}
		#endregion

		#region +InsertAfterChangeItem 定期変更後商品登録
		/// <summary>
		/// 定期変更後商品登録
		/// </summary>
		/// <param name="model">定期変更元商品モデル</param>
		public void InsertAfterChangeItem(FixedPurchaseAfterChangeItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertAfterChangeItem", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">定期商品変更設定モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(FixedPurchaseProductChangeSettingModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="fixedPurchaseProductChangeSettingId">定期商品変更設定ID</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(string fixedPurchaseProductChangeSettingId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID, fixedPurchaseProductChangeSettingId },
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region +DeleteBeforeChangeItem 定期変更元商品削除
		/// <summary>
		/// 定期変更元商品削除
		/// </summary>
		/// <param name="fixedPurchaseProductChangeSettingId">定期商品変更設定ID</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteBeforeChangeItem(string fixedPurchaseProductChangeSettingId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_FIXED_PURCHASE_PRODUCT_CHANGE_ID, fixedPurchaseProductChangeSettingId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteBeforeChangeItem", ht);
			return result;
		}
		#endregion

		#region +DeleteAfterChangeItem 定期変更後商品更新
		/// <summary>
		/// 定期変更後商品更新
		/// </summary>
		/// <param name="fixedPurchaseProductChangeSettingId">定期商品変更設定ID</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteAfterChangeItem(string fixedPurchaseProductChangeSettingId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASEAFTERCHANGEITEM_FIXED_PURCHASE_PRODUCT_CHANGE_ID, fixedPurchaseProductChangeSettingId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteAfterChangeItem", ht);
			return result;
		}
		#endregion
	}
}
