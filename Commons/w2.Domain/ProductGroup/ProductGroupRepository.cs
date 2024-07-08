/*
=========================================================================================================
  Module      : 商品グループリポジトリ (ProductGroupRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.ProductGroup.Helper;

namespace w2.Domain.ProductGroup
{
	/// <summary>
	/// 商品グループリポジトリ
	/// </summary>
	internal class ProductGroupRepository : RepositoryBase
	{
		/// <returns>XMLファイル名</returns>
		private const string XML_KEY_NAME = "ProductGroup";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ProductGroupRepository(SqlAccessor accessor = null)
			: base(accessor)
		{
		}
		#endregion

		#region -GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		internal int GetSearchHitCount(ProductGroupListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region -Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal ProductGroupListSearchResult[] Search(ProductGroupListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new ProductGroupListSearchResult(drv)).ToArray();
		}
		#endregion

		#region -Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="productGroupId">商品グループID</param>
		/// <returns>モデル</returns>
		internal ProductGroupModel Get(string productGroupId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_ID, productGroupId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			return (dv.Count == 0) ? null : new ProductGroupModel(dv[0]);
		}
		#endregion

		#region -GetAll 商品グループ全件取得
		/// <summary>
		/// 商品グループ全件取得
		/// </summary>
		/// <returns>モデル</returns>
		internal ProductGroupModel[] GetAll()
		{
			var ht = new Hashtable{};
			var dv = Get(XML_KEY_NAME, "GetAll", ht);
			return dv.Cast<DataRowView>()
				.Select(drv => new ProductGroupModel(drv)).ToArray();
		}
		#endregion

		#region -Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(ProductGroupModel model)
		{
			Exec(XML_KEY_NAME, "InsertProductGroup", model.DataSource);
		}
		#endregion

		#region -Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(ProductGroupModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateProductGroup", model.DataSource);
			return result;
		}
		#endregion

		#region -Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="productGroupId">商品グループID</param>
		/// <returns>影響を受けた件数</returns>
		internal int Delete(string productGroupId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_ID, productGroupId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteProductGroup", ht);
			return result;
		}
		#endregion

		#region -GetItemsAll アイテムすべて取得
		/// <summary>
		/// アイテムすべて取得
		/// </summary>
		/// <param name="productGroupId">商品グループID</param>
		/// <returns>モデル列</returns>
		internal ProductGroupItemModel[] GetItemsAll(string productGroupId)
		{
			if (productGroupId.Length == 0) return new ProductGroupItemModel[0];

			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_ID, productGroupId},
			};
			var dv = Get(XML_KEY_NAME, "GetProductGroupItem", ht);
			return dv.Cast<DataRowView>().Select(drv => new ProductGroupItemModel(drv)).ToArray();
		}
		#endregion

		#region -DeleteItemsAll アイテムすべて削除
		/// <summary>
		/// アイテムすべて削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="productGroupId">商品グループID</param>
		internal int DeleteItemsAll(string productGroupId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_ID, productGroupId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteProductGroupItem", ht);
			return result;
		}
		#endregion

		#region -InsertItem アイテム登録
		/// <summary>
		/// アイテム登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertItem(ProductGroupItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertProductGroupItem", model.DataSource);
		}
		#endregion

		#region -CheckDupulicationProductGroupId 商品グループID重複チェック
		/// <summary>
		/// 商品グループID重複チェック
		/// </summary>
		/// <param name="productGroupId">商品グループID</param>
		/// <returns>モデル</returns>
		internal ProductGroupModel CheckDupulicationProductGroupId(string productGroupId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_ID, productGroupId},
			};
			var dv = Get(XML_KEY_NAME, "CheckDupulicationProductGroupId", ht);
			return (dv.Count == 0) ? null : new ProductGroupModel(dv[0]);
		}
		#endregion

		#region -GetUsedProductGroupCount 紐づいた件数取得
		/// <summary>
		/// 紐づいた件数取得
		/// </summary>
		/// <param name="productGroupId">商品グループID</param>
		/// <returns>紐づいた件数</returns>
		internal int GetUsedProductGroupCount(string productGroupId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCTGROUP_PRODUCT_GROUP_ID, productGroupId },
			};
			var dv = Get(XML_KEY_NAME, "GetUsedProductGroupCount", ht);
			return (dv.Count == 0) ? 0 : (int)dv[0][0];
		}
		#endregion
	}
}
