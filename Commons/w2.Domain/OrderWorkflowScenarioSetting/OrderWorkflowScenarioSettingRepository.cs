/*
=========================================================================================================
  Module      : 受注ワークフローシナリオ設定リポジトリ (OrderWorkflowScenarioSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.OrderWorkflowScenarioSetting
{
	/// <summary>
	/// 受注ワークフローシナリオ設定リポジトリ
	/// </summary>
	internal class OrderWorkflowScenarioSettingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "OrderWorkflowScenarioSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal OrderWorkflowScenarioSettingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal OrderWorkflowScenarioSettingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="scenarioSettingId">シナリオ設定ID</param>
		/// <returns>モデル</returns>
		internal OrderWorkflowScenarioSettingModel Get(string scenarioSettingId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCENARIO_SETTING_ID, scenarioSettingId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			return (dv.Count != 0) ? new OrderWorkflowScenarioSettingModel(dv[0]) : null;
		}
		#endregion

		#region ~GetByOrderworkflowSettingPrimaryKey 取得
		/// <summary>
		/// 受注ワークフロー設定PKで取得
		/// </summary>
		/// <returns>モデル(複数)</returns>
		internal OrderWorkflowScenarioSettingItemModel[] GetByOrderworkflowSettingPrimaryKey(
			string shopId,
			string workflowKbn,
			int workflowNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SHOP_ID, shopId},
				{Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_WORKFLOW_KBN, workflowKbn},
				{Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_WORKFLOW_NO, workflowNo},
			};
			var scenarioSettings = Get(XML_KEY_NAME, "GetByOrderworkflowSettingPrimaryKey", ht).Cast<DataRowView>()
				.Select(drv => new OrderWorkflowScenarioSettingItemModel(drv)).ToArray();
			return scenarioSettings;
		}
		#endregion

		#region ~GetAll 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>モデル(複数)</returns>
		internal OrderWorkflowScenarioSettingModel[] GetAll()
		{
			var scenarioSettings = Get(XML_KEY_NAME, "GetAll").Cast<DataRowView>()
				.Select(drv => new OrderWorkflowScenarioSettingModel(drv)).ToArray();
			return scenarioSettings;
		}
		#endregion

		#region ~GetItems アイテムを取得
		/// <summary>
		/// アイテムを取得
		/// </summary>
		/// <param name="scenarioSettingId">シナリオ設定ID</param>
		/// <returns>モデル(複数)</returns>
		internal OrderWorkflowScenarioSettingItemModel[] GetItems(string scenarioSettingId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCENARIO_SETTING_ID, scenarioSettingId },
			};
			var scenarioSettingItem = Get(XML_KEY_NAME, "GetItems", ht).Cast<DataRowView>()
				.Select(drv => new OrderWorkflowScenarioSettingItemModel(drv)).ToArray();
			return scenarioSettingItem;
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(OrderWorkflowScenarioSettingModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~InsertItem アイテム登録
		/// <summary>
		/// アイテム登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertItem(OrderWorkflowScenarioSettingItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertItem", model.DataSource);
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(OrderWorkflowScenarioSettingModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="scenarioSettingId">シナリオ設定ID</param>
		/// <returns>影響を受けた件数</returns>
		internal int Delete(string scenarioSettingId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCENARIO_SETTING_ID, scenarioSettingId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region ~DeleteItemsAll アイテムを削除
		/// <summary>
		/// アイテムすべて削除
		/// </summary>
		/// <param name="scenarioSettingId">シナリオ設定ID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteItems(string scenarioSettingId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCENARIO_SETTING_ID, scenarioSettingId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteItems", ht);
			return result;
		}
		#endregion
	}
}
