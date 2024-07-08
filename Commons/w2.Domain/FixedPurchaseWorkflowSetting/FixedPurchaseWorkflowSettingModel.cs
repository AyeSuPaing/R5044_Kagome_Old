/*
=========================================================================================================
  Module      : 定期ワークフロー設定モデル (FixedPurchaseWorkflowSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FixedPurchaseWorkflowSetting
{
	/// <summary>
	/// 定期ワークフロー設定モデル
	/// </summary>
	[Serializable]
	public partial class FixedPurchaseWorkflowSettingModel : ModelBase<FixedPurchaseWorkflowSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseWorkflowSettingModel()
		{
			this.ShopId = "";
			this.WorkflowKbn = "";
			this.WorkflowNo = 1;
			this.WorkflowName = "";
			this.Desc1 = "";
			this.Desc2 = "";
			this.Desc3 = "";
			this.DisplayOrder = 1;
			this.DisplayCount = 1;
			this.ValidFlg = Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_VALIDFLG_TRUE;
			this.WorkflowDetailKbn = Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_NORMAL;
			this.DisplayKbn = Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_KBN_LINE;
			this.AdditionalSearchFlg = Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG_NOMAL;
			this.SearchSetting = "";
			this.FixedPurchaseIsAliveChange = "";
			this.FixedPurchasePaymentStatusChange = "";
			this.NextShippingDateChange = "";
			this.NextNextShippingDateChange = "";
			this.FixedPurchaseStopUnavailableShippingAreaChange = "";
			this.FixedPurchaseExtendStatusChange1 = "";
			this.FixedPurchaseExtendStatusChange2 = "";
			this.FixedPurchaseExtendStatusChange3 = "";
			this.FixedPurchaseExtendStatusChange4 = "";
			this.FixedPurchaseExtendStatusChange5 = "";
			this.FixedPurchaseExtendStatusChange6 = "";
			this.FixedPurchaseExtendStatusChange7 = "";
			this.FixedPurchaseExtendStatusChange8 = "";
			this.FixedPurchaseExtendStatusChange9 = "";
			this.FixedPurchaseExtendStatusChange10 = "";
			this.CassetteDefaultSelect = "";
			this.CassetteNoUpdate = Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NO_UPDATE_OFF;
			this.CassetteFixedPurchaseIsAliveChange = "";
			this.CassetteFixedPurchasePaymentStatusChange = "";
			this.CassettefixedPurchaseStopUnavailableShippingAreaChange = "";
			this.CassetteNextShippingDateChange = "";
			this.CassetteNextNextShippingDateChange = "";
			this.CassetteFixedPurchaseExtendStatusChange1 = "";
			this.CassetteFixedPurchaseExtendStatusChange2 = "";
			this.CassetteFixedPurchaseExtendStatusChange3 = "";
			this.CassetteFixedPurchaseExtendStatusChange4 = "";
			this.CassetteFixedPurchaseExtendStatusChange5 = "";
			this.CassetteFixedPurchaseExtendStatusChange6 = "";
			this.CassetteFixedPurchaseExtendStatusChange7 = "";
			this.CassetteFixedPurchaseExtendStatusChange8 = "";
			this.CassetteFixedPurchaseExtendStatusChange9 = "";
			this.CassetteFixedPurchaseExtendStatusChange10 = "";
			this.LastChanged = "";
			this.FixedPurchaseExtendStatusChange11 = "";
			this.FixedPurchaseExtendStatusChange12 = "";
			this.FixedPurchaseExtendStatusChange13 = "";
			this.FixedPurchaseExtendStatusChange14 = "";
			this.FixedPurchaseExtendStatusChange15 = "";
			this.FixedPurchaseExtendStatusChange16 = "";
			this.FixedPurchaseExtendStatusChange17 = "";
			this.FixedPurchaseExtendStatusChange18 = "";
			this.FixedPurchaseExtendStatusChange19 = "";
			this.FixedPurchaseExtendStatusChange20 = "";
			this.FixedPurchaseExtendStatusChange21 = "";
			this.FixedPurchaseExtendStatusChange22 = "";
			this.FixedPurchaseExtendStatusChange23 = "";
			this.FixedPurchaseExtendStatusChange24 = "";
			this.FixedPurchaseExtendStatusChange25 = "";
			this.FixedPurchaseExtendStatusChange26 = "";
			this.FixedPurchaseExtendStatusChange27 = "";
			this.FixedPurchaseExtendStatusChange28 = "";
			this.FixedPurchaseExtendStatusChange29 = "";
			this.FixedPurchaseExtendStatusChange30 = "";
			this.CassetteFixedPurchaseExtendStatusChange11 = "";
			this.CassetteFixedPurchaseExtendStatusChange12 = "";
			this.CassetteFixedPurchaseExtendStatusChange13 = "";
			this.CassetteFixedPurchaseExtendStatusChange14 = "";
			this.CassetteFixedPurchaseExtendStatusChange15 = "";
			this.CassetteFixedPurchaseExtendStatusChange16 = "";
			this.CassetteFixedPurchaseExtendStatusChange17 = "";
			this.CassetteFixedPurchaseExtendStatusChange18 = "";
			this.CassetteFixedPurchaseExtendStatusChange19 = "";
			this.CassetteFixedPurchaseExtendStatusChange20 = "";
			this.CassetteFixedPurchaseExtendStatusChange21 = "";
			this.CassetteFixedPurchaseExtendStatusChange22 = "";
			this.CassetteFixedPurchaseExtendStatusChange23 = "";
			this.CassetteFixedPurchaseExtendStatusChange24 = "";
			this.CassetteFixedPurchaseExtendStatusChange25 = "";
			this.CassetteFixedPurchaseExtendStatusChange26 = "";
			this.CassetteFixedPurchaseExtendStatusChange27 = "";
			this.CassetteFixedPurchaseExtendStatusChange28 = "";
			this.CassetteFixedPurchaseExtendStatusChange29 = "";
			this.CassetteFixedPurchaseExtendStatusChange30 = "";
			this.FixedPurchaseExtendStatusChange31 = "";
			this.FixedPurchaseExtendStatusChange32 = "";
			this.FixedPurchaseExtendStatusChange33 = "";
			this.FixedPurchaseExtendStatusChange34 = "";
			this.FixedPurchaseExtendStatusChange35 = "";
			this.FixedPurchaseExtendStatusChange36 = "";
			this.FixedPurchaseExtendStatusChange37 = "";
			this.FixedPurchaseExtendStatusChange38 = "";
			this.FixedPurchaseExtendStatusChange39 = "";
			this.FixedPurchaseExtendStatusChange40 = "";
			this.CassetteFixedPurchaseExtendStatusChange31 = "";
			this.CassetteFixedPurchaseExtendStatusChange32 = "";
			this.CassetteFixedPurchaseExtendStatusChange33 = "";
			this.CassetteFixedPurchaseExtendStatusChange34 = "";
			this.CassetteFixedPurchaseExtendStatusChange35 = "";
			this.CassetteFixedPurchaseExtendStatusChange36 = "";
			this.CassetteFixedPurchaseExtendStatusChange37 = "";
			this.CassetteFixedPurchaseExtendStatusChange38 = "";
			this.CassetteFixedPurchaseExtendStatusChange39 = "";
			this.CassetteFixedPurchaseExtendStatusChange40 = "";
			this.FixedPurchaseExtendStatusChange41 = "";
			this.FixedPurchaseExtendStatusChange42 = "";
			this.FixedPurchaseExtendStatusChange43 = "";
			this.FixedPurchaseExtendStatusChange44 = "";
			this.FixedPurchaseExtendStatusChange45 = "";
			this.FixedPurchaseExtendStatusChange46 = "";
			this.FixedPurchaseExtendStatusChange47 = "";
			this.FixedPurchaseExtendStatusChange48 = "";
			this.FixedPurchaseExtendStatusChange49 = "";
			this.FixedPurchaseExtendStatusChange50 = "";
			this.CassetteFixedPurchaseExtendStatusChange41 = "";
			this.CassetteFixedPurchaseExtendStatusChange42 = "";
			this.CassetteFixedPurchaseExtendStatusChange43 = "";
			this.CassetteFixedPurchaseExtendStatusChange44 = "";
			this.CassetteFixedPurchaseExtendStatusChange45 = "";
			this.CassetteFixedPurchaseExtendStatusChange46 = "";
			this.CassetteFixedPurchaseExtendStatusChange47 = "";
			this.CassetteFixedPurchaseExtendStatusChange48 = "";
			this.CassetteFixedPurchaseExtendStatusChange49 = "";
			this.CassetteFixedPurchaseExtendStatusChange50 = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseWorkflowSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseWorkflowSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_SHOP_ID] = value; }
		}
		/// <summary>ワークフロー区分</summary>
		public string WorkflowKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_KBN]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_KBN] = value; }
		}
		/// <summary>枝番</summary>
		public int WorkflowNo
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_NO]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_NO] = value; }
		}
		/// <summary>ワークフロー名</summary>
		public string WorkflowName
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_NAME]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_NAME] = value; }
		}
		/// <summary>説明1</summary>
		public string Desc1
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DESC1]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DESC1] = value; }
		}
		/// <summary>説明2</summary>
		public string Desc2
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DESC2]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DESC2] = value; }
		}
		/// <summary>説明3</summary>
		public string Desc3
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DESC3]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DESC3] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_ORDER] = value; }
		}
		/// <summary>表示件数</summary>
		public int DisplayCount
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_COUNT]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_COUNT] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_VALID_FLG] = value; }
		}
		/// <summary>ワークフロー詳細区分</summary>
		public string WorkflowDetailKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_DETAIL_KBN]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_DETAIL_KBN] = value; }
		}
		/// <summary>表示区分</summary>
		public string DisplayKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_KBN]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_KBN] = value; }
		}
		/// <summary>追加検索可否FLG</summary>
		public string AdditionalSearchFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG] = value; }
		}
		/// <summary>抽出検索条件</summary>
		public string SearchSetting
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_SEARCH_SETTING]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_SEARCH_SETTING] = value; }
		}
		/// <summary>定期状態変更</summary>
		public string FixedPurchaseIsAliveChange
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE] = value; }
		}
		/// <summary>決済ステータス更新</summary>
		public string FixedPurchasePaymentStatusChange
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE] = value; }
		}
		/// <summary>次回配送日更新</summary>
		public string NextShippingDateChange
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_SHIPPING_DATE_CHANGE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_SHIPPING_DATE_CHANGE] = value; }
		}
		/// <summary>次々回配送日更新</summary>
		public string NextNextShippingDateChange
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_NEXT_SHIPPING_DATE_CHANGE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_NEXT_SHIPPING_DATE_CHANGE] = value; }
		}
		/// <summary>配送不可エリア停止更新</summary>
		public string FixedPurchaseStopUnavailableShippingAreaChange
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE] = value; }
		}
		/// <summary>注文拡張ステータス変更区分1</summary>
		public string FixedPurchaseExtendStatusChange1
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE1]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE1] = value; }
		}
		/// <summary>注文拡張ステータス変更区分2</summary>
		public string FixedPurchaseExtendStatusChange2
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE2]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE2] = value; }
		}
		/// <summary>注文拡張ステータス変更区分3</summary>
		public string FixedPurchaseExtendStatusChange3
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE3]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE3] = value; }
		}
		/// <summary>注文拡張ステータス変更区分4</summary>
		public string FixedPurchaseExtendStatusChange4
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE4]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE4] = value; }
		}
		/// <summary>注文拡張ステータス変更区分5</summary>
		public string FixedPurchaseExtendStatusChange5
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE5]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE5] = value; }
		}
		/// <summary>注文拡張ステータス変更区分6</summary>
		public string FixedPurchaseExtendStatusChange6
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE6]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE6] = value; }
		}
		/// <summary>注文拡張ステータス変更区分7</summary>
		public string FixedPurchaseExtendStatusChange7
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE7]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE7] = value; }
		}
		/// <summary>注文拡張ステータス変更区分8</summary>
		public string FixedPurchaseExtendStatusChange8
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE8]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE8] = value; }
		}
		/// <summary>注文拡張ステータス変更区分9</summary>
		public string FixedPurchaseExtendStatusChange9
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE9]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE9] = value; }
		}
		/// <summary>注文拡張ステータス変更区分10</summary>
		public string FixedPurchaseExtendStatusChange10
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE10]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE10] = value; }
		}
		/// <summary>カセット表示用初期選択値</summary>
		public string CassetteDefaultSelect
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_DEFAULT_SELECT]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_DEFAULT_SELECT] = value; }
		}
		/// <summary>カセット表示用未実行フラグ</summary>
		public string CassetteNoUpdate
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NO_UPDATE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NO_UPDATE] = value; }
		}
		/// <summary>カセット表示用定期状態変更</summary>
		public string CassetteFixedPurchaseIsAliveChange
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_IS_ALIVE_CHANGE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_IS_ALIVE_CHANGE] = value; }
		}
		/// <summary>カセット表示用決済ステータス更新</summary>
		public string CassetteFixedPurchasePaymentStatusChange
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE] = value; }
		}
		/// <summary>カセット表示用配送不可エリア停止変更</summary>
		public string CassettefixedPurchaseStopUnavailableShippingAreaChange
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE] = value; }
		}
		/// <summary>カセット表示用次回配送日更新</summary>
		public string CassetteNextShippingDateChange
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NEXT_SHIPPING_DATE_CHANGE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NEXT_SHIPPING_DATE_CHANGE] = value; }
		}
		/// <summary>カセット表示用次々回配送日更新</summary>
		public string CassetteNextNextShippingDateChange
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NEXT_NEXT_SHIPPING_DATE_CHANGE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NEXT_NEXT_SHIPPING_DATE_CHANGE] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分1</summary>
		public string CassetteFixedPurchaseExtendStatusChange1
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE1]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE1] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分2</summary>
		public string CassetteFixedPurchaseExtendStatusChange2
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE2]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE2] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分3</summary>
		public string CassetteFixedPurchaseExtendStatusChange3
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE3]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE3] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分4</summary>
		public string CassetteFixedPurchaseExtendStatusChange4
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE4]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE4] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分5</summary>
		public string CassetteFixedPurchaseExtendStatusChange5
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE5]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE5] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分6</summary>
		public string CassetteFixedPurchaseExtendStatusChange6
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE6]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE6] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分7</summary>
		public string CassetteFixedPurchaseExtendStatusChange7
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE7]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE7] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分8</summary>
		public string CassetteFixedPurchaseExtendStatusChange8
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE8]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE8] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分9</summary>
		public string CassetteFixedPurchaseExtendStatusChange9
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE9]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE9] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分10</summary>
		public string CassetteFixedPurchaseExtendStatusChange10
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE10]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE10] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_LAST_CHANGED] = value; }
		}
		/// <summary>注文拡張ステータス変更区分11</summary>
		public string FixedPurchaseExtendStatusChange11
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE11]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE11] = value; }
		}
		/// <summary>注文拡張ステータス変更区分12</summary>
		public string FixedPurchaseExtendStatusChange12
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE12]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE12] = value; }
		}
		/// <summary>注文拡張ステータス変更区分13</summary>
		public string FixedPurchaseExtendStatusChange13
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE13]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE13] = value; }
		}
		/// <summary>注文拡張ステータス変更区分14</summary>
		public string FixedPurchaseExtendStatusChange14
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE14]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE14] = value; }
		}
		/// <summary>注文拡張ステータス変更区分15</summary>
		public string FixedPurchaseExtendStatusChange15
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE15]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE15] = value; }
		}
		/// <summary>注文拡張ステータス変更区分16</summary>
		public string FixedPurchaseExtendStatusChange16
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE16]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE16] = value; }
		}
		/// <summary>注文拡張ステータス変更区分17</summary>
		public string FixedPurchaseExtendStatusChange17
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE17]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE17] = value; }
		}
		/// <summary>注文拡張ステータス変更区分18</summary>
		public string FixedPurchaseExtendStatusChange18
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE18]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE18] = value; }
		}
		/// <summary>注文拡張ステータス変更区分19</summary>
		public string FixedPurchaseExtendStatusChange19
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE19]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE19] = value; }
		}
		/// <summary>注文拡張ステータス変更区分20</summary>
		public string FixedPurchaseExtendStatusChange20
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE20]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE20] = value; }
		}
		/// <summary>注文拡張ステータス変更区分21</summary>
		public string FixedPurchaseExtendStatusChange21
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE21]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE21] = value; }
		}
		/// <summary>注文拡張ステータス変更区分22</summary>
		public string FixedPurchaseExtendStatusChange22
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE22]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE22] = value; }
		}
		/// <summary>注文拡張ステータス変更区分23</summary>
		public string FixedPurchaseExtendStatusChange23
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE23]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE23] = value; }
		}
		/// <summary>注文拡張ステータス変更区分24</summary>
		public string FixedPurchaseExtendStatusChange24
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE24]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE24] = value; }
		}
		/// <summary>注文拡張ステータス変更区分25</summary>
		public string FixedPurchaseExtendStatusChange25
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE25]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE25] = value; }
		}
		/// <summary>注文拡張ステータス変更区分26</summary>
		public string FixedPurchaseExtendStatusChange26
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE26]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE26] = value; }
		}
		/// <summary>注文拡張ステータス変更区分27</summary>
		public string FixedPurchaseExtendStatusChange27
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE27]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE27] = value; }
		}
		/// <summary>注文拡張ステータス変更区分28</summary>
		public string FixedPurchaseExtendStatusChange28
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE28]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE28] = value; }
		}
		/// <summary>注文拡張ステータス変更区分29</summary>
		public string FixedPurchaseExtendStatusChange29
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE29]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE29] = value; }
		}
		/// <summary>注文拡張ステータス変更区分30</summary>
		public string FixedPurchaseExtendStatusChange30
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE30]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE30] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分11</summary>
		public string CassetteFixedPurchaseExtendStatusChange11
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE11]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE11] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分12</summary>
		public string CassetteFixedPurchaseExtendStatusChange12
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE12]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE12] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分13</summary>
		public string CassetteFixedPurchaseExtendStatusChange13
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE13]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE13] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分14</summary>
		public string CassetteFixedPurchaseExtendStatusChange14
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE14]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE14] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分15</summary>
		public string CassetteFixedPurchaseExtendStatusChange15
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE15]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE15] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分16</summary>
		public string CassetteFixedPurchaseExtendStatusChange16
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE16]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE16] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分17</summary>
		public string CassetteFixedPurchaseExtendStatusChange17
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE17]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE17] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分18</summary>
		public string CassetteFixedPurchaseExtendStatusChange18
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE18]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE18] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分19</summary>
		public string CassetteFixedPurchaseExtendStatusChange19
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE19]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE19] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分20</summary>
		public string CassetteFixedPurchaseExtendStatusChange20
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE20]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE20] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分21</summary>
		public string CassetteFixedPurchaseExtendStatusChange21
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE21]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE21] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分22</summary>
		public string CassetteFixedPurchaseExtendStatusChange22
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE22]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE22] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分23</summary>
		public string CassetteFixedPurchaseExtendStatusChange23
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE23]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE23] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分24</summary>
		public string CassetteFixedPurchaseExtendStatusChange24
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE24]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE24] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分25</summary>
		public string CassetteFixedPurchaseExtendStatusChange25
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE25]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE25] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分26</summary>
		public string CassetteFixedPurchaseExtendStatusChange26
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE26]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE26] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分27</summary>
		public string CassetteFixedPurchaseExtendStatusChange27
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE27]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE27] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分28</summary>
		public string CassetteFixedPurchaseExtendStatusChange28
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE28]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE28] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分29</summary>
		public string CassetteFixedPurchaseExtendStatusChange29
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE29]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE29] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分30</summary>
		public string CassetteFixedPurchaseExtendStatusChange30
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE30]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE30] = value; }
		}
		/// <summary>注文拡張ステータス変更区分31</summary>
		public string FixedPurchaseExtendStatusChange31
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE31]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE31] = value; }
		}
		/// <summary>注文拡張ステータス変更区分32</summary>
		public string FixedPurchaseExtendStatusChange32
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE32]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE32] = value; }
		}
		/// <summary>注文拡張ステータス変更区分33</summary>
		public string FixedPurchaseExtendStatusChange33
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE33]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE33] = value; }
		}
		/// <summary>注文拡張ステータス変更区分34</summary>
		public string FixedPurchaseExtendStatusChange34
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE34]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE34] = value; }
		}
		/// <summary>注文拡張ステータス変更区分35</summary>
		public string FixedPurchaseExtendStatusChange35
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE35]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE35] = value; }
		}
		/// <summary>注文拡張ステータス変更区分36</summary>
		public string FixedPurchaseExtendStatusChange36
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE36]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE36] = value; }
		}
		/// <summary>注文拡張ステータス変更区分37</summary>
		public string FixedPurchaseExtendStatusChange37
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE37]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE37] = value; }
		}
		/// <summary>注文拡張ステータス変更区分38</summary>
		public string FixedPurchaseExtendStatusChange38
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE38]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE38] = value; }
		}
		/// <summary>注文拡張ステータス変更区分39</summary>
		public string FixedPurchaseExtendStatusChange39
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE39]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE39] = value; }
		}
		/// <summary>注文拡張ステータス変更区分40</summary>
		public string FixedPurchaseExtendStatusChange40
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE40]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE40] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分31</summary>
		public string CassetteFixedPurchaseExtendStatusChange31
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE31]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE31] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分32</summary>
		public string CassetteFixedPurchaseExtendStatusChange32
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE32]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE32] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分33</summary>
		public string CassetteFixedPurchaseExtendStatusChange33
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE33]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE33] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分34</summary>
		public string CassetteFixedPurchaseExtendStatusChange34
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE34]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE34] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分35</summary>
		public string CassetteFixedPurchaseExtendStatusChange35
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE35]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE35] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分36</summary>
		public string CassetteFixedPurchaseExtendStatusChange36
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE36]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE36] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分37</summary>
		public string CassetteFixedPurchaseExtendStatusChange37
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE37]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE37] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分38</summary>
		public string CassetteFixedPurchaseExtendStatusChange38
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE38]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE38] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分39</summary>
		public string CassetteFixedPurchaseExtendStatusChange39
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE39]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE39] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分40</summary>
		public string CassetteFixedPurchaseExtendStatusChange40
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE40]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE40] = value; }
		}
		/// <summary>注文拡張ステータス変更区分41</summary>
		public string FixedPurchaseExtendStatusChange41
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE41]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE41] = value; }
		}
		/// <summary>注文拡張ステータス変更区分42</summary>
		public string FixedPurchaseExtendStatusChange42
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE42]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE42] = value; }
		}
		/// <summary>注文拡張ステータス変更区分43</summary>
		public string FixedPurchaseExtendStatusChange43
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE43]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE43] = value; }
		}
		/// <summary>注文拡張ステータス変更区分44</summary>
		public string FixedPurchaseExtendStatusChange44
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE44]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE44] = value; }
		}
		/// <summary>注文拡張ステータス変更区分45</summary>
		public string FixedPurchaseExtendStatusChange45
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE45]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE45] = value; }
		}
		/// <summary>注文拡張ステータス変更区分46</summary>
		public string FixedPurchaseExtendStatusChange46
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE46]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE46] = value; }
		}
		/// <summary>注文拡張ステータス変更区分47</summary>
		public string FixedPurchaseExtendStatusChange47
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE47]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE47] = value; }
		}
		/// <summary>注文拡張ステータス変更区分48</summary>
		public string FixedPurchaseExtendStatusChange48
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE48]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE48] = value; }
		}
		/// <summary>注文拡張ステータス変更区分49</summary>
		public string FixedPurchaseExtendStatusChange49
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE49]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE49] = value; }
		}
		/// <summary>注文拡張ステータス変更区分50</summary>
		public string FixedPurchaseExtendStatusChange50
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE50]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_EXTEND_STATUS_CHANGE50] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分41</summary>
		public string CassetteFixedPurchaseExtendStatusChange41
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE41]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE41] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分42</summary>
		public string CassetteFixedPurchaseExtendStatusChange42
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE42]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE42] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分43</summary>
		public string CassetteFixedPurchaseExtendStatusChange43
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE43]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE43] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分44</summary>
		public string CassetteFixedPurchaseExtendStatusChange44
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE44]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE44] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分45</summary>
		public string CassetteFixedPurchaseExtendStatusChange45
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE45]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE45] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分46</summary>
		public string CassetteFixedPurchaseExtendStatusChange46
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE46]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE46] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分47</summary>
		public string CassetteFixedPurchaseExtendStatusChange47
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE47]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE47] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分48</summary>
		public string CassetteFixedPurchaseExtendStatusChange48
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE48]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE48] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分49</summary>
		public string CassetteFixedPurchaseExtendStatusChange49
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE49]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE49] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分50</summary>
		public string CassetteFixedPurchaseExtendStatusChange50
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE50]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_EXTEND_STATUS_CHANGE50] = value; }
		}
		#endregion
	}
}
