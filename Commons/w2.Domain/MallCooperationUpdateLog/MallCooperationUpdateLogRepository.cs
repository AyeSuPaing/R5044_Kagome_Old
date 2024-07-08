/*
=========================================================================================================
  Module      : モール連携更新ログリポジトリ (MallCooperationUpdateLogRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.MallCooperationUpdateLog
{
	/// <summary>
	/// モール連携更新ログリポジトリ
	/// </summary>
	internal class MallCooperationUpdateLogRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "MallCooperationUpdateLog";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal MallCooperationUpdateLogRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal MallCooperationUpdateLogRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion
		#region ~GetTargetStockUpdate 在庫連携対象取得
		/// <summary>
		/// 在庫連携対象取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <param name="amazonSkuColumn">AmazonSKU取得列</param>
		/// <param name="fulfilmentColumn">出荷作業日数取得列</param>
		/// <returns>在庫連携対象リスト</returns>
		internal MallCooperationUpdateLogModel[] GetTargetStockUpdate(string shopId, string mallId, string amazonSkuColumn, string fulfilmentColumn)
		{
			using (var statement = new SqlStatement(XML_KEY_NAME, "GetTargetStockUpdate"))
			{
				var ht = new Hashtable
				{
					{ Constants.FIELD_MALLCOOPERATIONUPDATELOG_SHOP_ID, shopId },
					{ Constants.FIELD_MALLCOOPERATIONUPDATELOG_MALL_ID, mallId },
				};
				statement.ReplaceStatement("@@ amazon_product_sku_select @@", amazonSkuColumn);
				statement.ReplaceStatement("@@ fulfilmentlatency_select @@", fulfilmentColumn);
				var dv = statement.SelectSingleStatementWithOC(this.Accessor, ht);
				return dv.Cast<DataRowView>().Select(drv => new MallCooperationUpdateLogModel(drv)).ToArray();
			}
		}
		#endregion

		#region ~GetLohacoTargetStockUpdate Lohaco在庫連携対象取得
		/// <summary>
		/// Lohaco在庫連携対象取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <returns>Lohaco在庫連携対象リスト</returns>
		internal MallCooperationUpdateLogModel[] GetLohacoTargetStockUpdate(string shopId, string mallId)
		{
			using (var statement = new SqlStatement(XML_KEY_NAME, "GetLohacoTargetStockUpdate"))
			{
				var ht = new Hashtable
				{
					{ Constants.FIELD_MALLCOOPERATIONUPDATELOG_SHOP_ID, shopId },
					{ Constants.FIELD_MALLCOOPERATIONUPDATELOG_MALL_ID, mallId },
				};
				var dv = statement.SelectSingleStatementWithOC(this.Accessor, ht);
				return dv.Cast<DataRowView>().Select(drv => new MallCooperationUpdateLogModel(drv)).ToArray();
			}
		}
		#endregion

		#region ~UpdateExcludedStockUpdate 在庫連携処理対象外更新
		/// <summary>
		/// 在庫連携処理対象外更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateExcludedStockUpdate(string shopId, string mallId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_MALLCOOPERATIONUPDATELOG_SHOP_ID, shopId },
				{ Constants.FIELD_MALLCOOPERATIONUPDATELOG_MALL_ID, mallId },
			};
			var result = Exec(XML_KEY_NAME, "UpdateExcludedStockUpdate", ht);
			return result;
		}
		#endregion

		#region ~UpdateActionStatus 処理ステータス更新
		/// <summary>
		/// 処理ステータス更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="updateLogNoList">更新対象ログNoリスト</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateActionStatus(MallCooperationUpdateLogModel model, string updateLogNoList)
		{
			using (var statement = new SqlStatement(XML_KEY_NAME, "UpdateActionStatus"))
			{
				var ht = new Hashtable
				{
					{ Constants.FIELD_MALLCOOPERATIONUPDATELOG_SHOP_ID, model.ShopId },
					{ Constants.FIELD_MALLCOOPERATIONUPDATELOG_MALL_ID, model.MallId },
					{ Constants.FIELD_MALLCOOPERATIONUPDATELOG_MASTER_KBN, model.MasterKbn },
					{ Constants.FIELD_MALLCOOPERATIONUPDATELOG_ACTION_KBN, model.ActionKbn },
					{ Constants.FIELD_MALLCOOPERATIONUPDATELOG_ACTION_STATUS, model.ActionStatus }
				};
				statement.ReplaceStatement("@@ log_no_where @@", updateLogNoList);
				var result = statement.ExecStatementWithOC(this.Accessor, ht);
				return result;
			}
		}
		#endregion
	}
}
