/*
=========================================================================================================
  Module      : シナリオ設定アイテムモデル (OrderWorkflowScenarioSettingItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.OrderWorkflowScenarioSetting
{
	/// <summary>
	/// シナリオ設定アイテムモデル
	/// </summary>
	[Serializable]
	public partial class OrderWorkflowScenarioSettingItemModel : ModelBase<OrderWorkflowScenarioSettingItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderWorkflowScenarioSettingItemModel()
		{
			this.ScenarioSettingId = "";
			this.ScenarioNo = 1;
			this.ShopId = "";
			this.WorkflowKbn = "";
			this.TargetWorkflowKbn = Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderWorkflowScenarioSettingItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderWorkflowScenarioSettingItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>シナリオ設定ID</summary>
		public string ScenarioSettingId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SCENARIO_SETTING_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SCENARIO_SETTING_ID] = value; }
		}
		/// <summary>シナリオ枝番</summary>
		public int ScenarioNo
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SCENARIO_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SCENARIO_NO] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SHOP_ID] = value; }
		}
		/// <summary>ワークフロー区分</summary>
		public string WorkflowKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_WORKFLOW_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_WORKFLOW_KBN] = value; }
		}
		/// <summary>ワークフロー枝番</summary>
		public int? WorkflowNo
		{
			get { return (int?)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_WORKFLOW_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_WORKFLOW_NO] = value; }
		}
		/// <summary>実行対象ワークフロー区分</summary>
		public string TargetWorkflowKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_TARGET_WORKFLOW_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_TARGET_WORKFLOW_KBN] = value; }
		}
		#endregion
	}
}
