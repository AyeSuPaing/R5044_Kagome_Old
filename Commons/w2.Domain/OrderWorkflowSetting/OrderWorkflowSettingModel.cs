/*
=========================================================================================================
  Module      : 注文ワークフロー設定モデル (OrderWorkflowSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.OrderWorkflowSetting
{
	/// <summary>
	/// 注文ワークフロー設定モデル
	/// </summary>
	[Serializable]
	public partial class OrderWorkflowSettingModel : ModelBase<OrderWorkflowSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderWorkflowSettingModel()
		{
			this.ShopId = "";
			this.WorkflowKbn = "";
			this.WorkflowNo = 1;
			this.WorkflowRefNo = 0;
			this.WorkflowName = "";
			this.Desc1 = "";
			this.Desc2 = "";
			this.Desc3 = "";
			this.DisplayOrder = 1;
			this.DisplayCount = 1;
			this.ValidFlg = Constants.FLG_ORDERWORKFLOWSETTING_VALID_FLG_VALID;
			this.WorkflowDetailKbn = Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_NORMAL;
			this.DisplayKbn = Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE;
			this.AdditionalSearchFlg = Constants.FLG_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG_OFF;
			this.SearchSetting = "";
			this.OrderStatusChange = "";
			this.ProductRealstockChange = "";
			this.PaymentStatusChange = "";
			this.ExternalPaymentAction = "";
			this.DemandStatusChange = "";
			this.ReturnExchangeStatusChange = "";
			this.RepaymentStatusChange = "";
			this.OrderExtendStatusChange1 = "";
			this.OrderExtendStatusChange2 = "";
			this.OrderExtendStatusChange3 = "";
			this.OrderExtendStatusChange4 = "";
			this.OrderExtendStatusChange5 = "";
			this.OrderExtendStatusChange6 = "";
			this.OrderExtendStatusChange7 = "";
			this.OrderExtendStatusChange8 = "";
			this.OrderExtendStatusChange9 = "";
			this.OrderExtendStatusChange10 = "";
			this.MailId = "";
			this.CassetteDefaultSelect = "";
			this.CassetteNoUpdate = Constants.FLG_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE_OFF;
			this.CassetteOrderStatusChange = "";
			this.CassetteProductRealstockChange = "";
			this.CassettePaymentStatusChange = "";
			this.CassetteExternalPaymentAction = "";
			this.CassetteDemandStatusChange = "";
			this.CassetteReturnExchangeStatusChange = "";
			this.CassetteRepaymentStatusChange = "";
			this.CassetteOrderExtendStatusChange1 = "";
			this.CassetteOrderExtendStatusChange2 = "";
			this.CassetteOrderExtendStatusChange3 = "";
			this.CassetteOrderExtendStatusChange4 = "";
			this.CassetteOrderExtendStatusChange5 = "";
			this.CassetteOrderExtendStatusChange6 = "";
			this.CassetteOrderExtendStatusChange7 = "";
			this.CassetteOrderExtendStatusChange8 = "";
			this.CassetteOrderExtendStatusChange9 = "";
			this.CassetteOrderExtendStatusChange10 = "";
			this.DelFlg = Constants.FLG_ORDERWORKFLOWSETTING_DEL_FLG_NOMAL;
			this.LastChanged = "";
			this.OrderExtendStatusChange11 = "";
			this.OrderExtendStatusChange12 = "";
			this.OrderExtendStatusChange13 = "";
			this.OrderExtendStatusChange14 = "";
			this.OrderExtendStatusChange15 = "";
			this.OrderExtendStatusChange16 = "";
			this.OrderExtendStatusChange17 = "";
			this.OrderExtendStatusChange18 = "";
			this.OrderExtendStatusChange19 = "";
			this.OrderExtendStatusChange20 = "";
			this.OrderExtendStatusChange21 = "";
			this.OrderExtendStatusChange22 = "";
			this.OrderExtendStatusChange23 = "";
			this.OrderExtendStatusChange24 = "";
			this.OrderExtendStatusChange25 = "";
			this.OrderExtendStatusChange26 = "";
			this.OrderExtendStatusChange27 = "";
			this.OrderExtendStatusChange28 = "";
			this.OrderExtendStatusChange29 = "";
			this.OrderExtendStatusChange30 = "";
			this.CassetteOrderExtendStatusChange11 = "";
			this.CassetteOrderExtendStatusChange12 = "";
			this.CassetteOrderExtendStatusChange13 = "";
			this.CassetteOrderExtendStatusChange14 = "";
			this.CassetteOrderExtendStatusChange15 = "";
			this.CassetteOrderExtendStatusChange16 = "";
			this.CassetteOrderExtendStatusChange17 = "";
			this.CassetteOrderExtendStatusChange18 = "";
			this.CassetteOrderExtendStatusChange19 = "";
			this.CassetteOrderExtendStatusChange20 = "";
			this.CassetteOrderExtendStatusChange21 = "";
			this.CassetteOrderExtendStatusChange22 = "";
			this.CassetteOrderExtendStatusChange23 = "";
			this.CassetteOrderExtendStatusChange24 = "";
			this.CassetteOrderExtendStatusChange25 = "";
			this.CassetteOrderExtendStatusChange26 = "";
			this.CassetteOrderExtendStatusChange27 = "";
			this.CassetteOrderExtendStatusChange28 = "";
			this.CassetteOrderExtendStatusChange29 = "";
			this.CassetteOrderExtendStatusChange30 = "";
			this.OrderExtendStatusChange31 = "";
			this.OrderExtendStatusChange32 = "";
			this.OrderExtendStatusChange33 = "";
			this.OrderExtendStatusChange34 = "";
			this.OrderExtendStatusChange35 = "";
			this.OrderExtendStatusChange36 = "";
			this.OrderExtendStatusChange37 = "";
			this.OrderExtendStatusChange38 = "";
			this.OrderExtendStatusChange39 = "";
			this.OrderExtendStatusChange40 = "";
			this.CassetteOrderExtendStatusChange31 = "";
			this.CassetteOrderExtendStatusChange32 = "";
			this.CassetteOrderExtendStatusChange33 = "";
			this.CassetteOrderExtendStatusChange34 = "";
			this.CassetteOrderExtendStatusChange35 = "";
			this.CassetteOrderExtendStatusChange36 = "";
			this.CassetteOrderExtendStatusChange37 = "";
			this.CassetteOrderExtendStatusChange38 = "";
			this.CassetteOrderExtendStatusChange39 = "";
			this.CassetteOrderExtendStatusChange40 = "";
			this.OrderExtendStatusChange41 = "";
			this.OrderExtendStatusChange42 = "";
			this.OrderExtendStatusChange43 = "";
			this.OrderExtendStatusChange44 = "";
			this.OrderExtendStatusChange45 = "";
			this.OrderExtendStatusChange46 = "";
			this.OrderExtendStatusChange47 = "";
			this.OrderExtendStatusChange48 = "";
			this.OrderExtendStatusChange49 = "";
			this.OrderExtendStatusChange50 = "";
			this.CassetteOrderExtendStatusChange41 = "";
			this.CassetteOrderExtendStatusChange42 = "";
			this.CassetteOrderExtendStatusChange43 = "";
			this.CassetteOrderExtendStatusChange44 = "";
			this.CassetteOrderExtendStatusChange45 = "";
			this.CassetteOrderExtendStatusChange46 = "";
			this.CassetteOrderExtendStatusChange47 = "";
			this.CassetteOrderExtendStatusChange48 = "";
			this.CassetteOrderExtendStatusChange49 = "";
			this.CassetteOrderExtendStatusChange50 = "";
			this.StorePickupStatusChange = string.Empty;
			this.CassetteStorePickupStatusChange = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderWorkflowSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderWorkflowSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID] = value; }
		}
		/// <summary>ワークフロー区分</summary>
		public string WorkflowKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN] = value; }
		}
		/// <summary>枝番</summary>
		public int? WorkflowNo
		{
			get { return (int?)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO] = value; }
		}
		/// <summary>参照枝番</summary>
		public int WorkflowRefNo
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_REF_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_REF_NO] = value; }
		}
		/// <summary>ワークフロー名</summary>
		public string WorkflowName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NAME] = value; }
		}
		/// <summary>説明1</summary>
		public string Desc1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DESC1]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DESC1] = value; }
		}
		/// <summary>説明2</summary>
		public string Desc2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DESC2]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DESC2] = value; }
		}
		/// <summary>説明3</summary>
		public string Desc3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DESC3]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DESC3] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_ORDER] = value; }
		}
		/// <summary>表示件数</summary>
		public int DisplayCount
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_VALID_FLG] = value; }
		}
		/// <summary>ワークフロー詳細区分</summary>
		public string WorkflowDetailKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN] = value; }
		}
		/// <summary>表示区分</summary>
		public string DisplayKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_KBN] = value; }
		}
		/// <summary>追加検索可否FLG</summary>
		public string AdditionalSearchFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG] = value; }
		}
		/// <summary>抽出検索条件</summary>
		public string SearchSetting
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_SEARCH_SETTING]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_SEARCH_SETTING] = value; }
		}
		/// <summary>注文ステータス変更区分</summary>
		public string OrderStatusChange
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STATUS_CHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STATUS_CHANGE] = value; }
		}
		/// <summary>商品実在庫変更区分</summary>
		public string ProductRealstockChange
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE] = value; }
		}
		/// <summary>入金ステータス変更区分</summary>
		public string PaymentStatusChange
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE] = value; }
		}
		/// <summary>外部決済連携処理区分</summary>
		public string ExternalPaymentAction
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION] = value; }
		}
		/// <summary>督促ステータス変更区分</summary>
		public string DemandStatusChange
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DEMAND_STATUS_CHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DEMAND_STATUS_CHANGE] = value; }
		}
		/// <summary>返品交換ステータス変更区分</summary>
		public string ReturnExchangeStatusChange
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_EXCHANGE_STATUS_CHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_EXCHANGE_STATUS_CHANGE] = value; }
		}
		/// <summary>返金ステータス変更区分</summary>
		public string RepaymentStatusChange
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE] = value; }
		}
		/// <summary>注文拡張ステータス変更区分1</summary>
		public string OrderExtendStatusChange1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE1]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE1] = value; }
		}
		/// <summary>注文拡張ステータス変更区分2</summary>
		public string OrderExtendStatusChange2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE2]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE2] = value; }
		}
		/// <summary>注文拡張ステータス変更区分3</summary>
		public string OrderExtendStatusChange3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE3]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE3] = value; }
		}
		/// <summary>注文拡張ステータス変更区分4</summary>
		public string OrderExtendStatusChange4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE4]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE4] = value; }
		}
		/// <summary>注文拡張ステータス変更区分5</summary>
		public string OrderExtendStatusChange5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE5]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE5] = value; }
		}
		/// <summary>注文拡張ステータス変更区分6</summary>
		public string OrderExtendStatusChange6
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE6]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE6] = value; }
		}
		/// <summary>注文拡張ステータス変更区分7</summary>
		public string OrderExtendStatusChange7
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE7]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE7] = value; }
		}
		/// <summary>注文拡張ステータス変更区分8</summary>
		public string OrderExtendStatusChange8
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE8]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE8] = value; }
		}
		/// <summary>注文拡張ステータス変更区分9</summary>
		public string OrderExtendStatusChange9
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE9]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE9] = value; }
		}
		/// <summary>注文拡張ステータス変更区分10</summary>
		public string OrderExtendStatusChange10
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE10]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE10] = value; }
		}
		/// <summary>送信メールテンプレートID</summary>
		public string MailId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_MAIL_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_MAIL_ID] = value; }
		}
		/// <summary>カセット表示用初期選択値</summary>
		public string CassetteDefaultSelect
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEFAULT_SELECT]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEFAULT_SELECT] = value; }
		}
		/// <summary>カセット表示用未実行フラグ</summary>
		public string CassetteNoUpdate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE] = value; }
		}
		/// <summary>カセット表示用注文ステータス変更区分</summary>
		public string CassetteOrderStatusChange
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STATUS_CHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STATUS_CHANGE] = value; }
		}
		/// <summary>カセット表示用商品実在庫変更区分</summary>
		public string CassetteProductRealstockChange
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PRODUCT_REALSTOCK_CHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PRODUCT_REALSTOCK_CHANGE] = value; }
		}
		/// <summary>カセット表示用入金ステータス変更区分</summary>
		public string CassettePaymentStatusChange
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PAYMENT_STATUS_CHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PAYMENT_STATUS_CHANGE] = value; }
		}
		/// <summary>カセット表示用外部決済連携処理区分</summary>
		public string CassetteExternalPaymentAction
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_PAYMENT_ACTION]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_PAYMENT_ACTION] = value; }
		}
		/// <summary>カセット表示用督促ステータス変更区分</summary>
		public string CassetteDemandStatusChange
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEMAND_STATUS_CHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEMAND_STATUS_CHANGE] = value; }
		}
		/// <summary>カセット表示用返品交換ステータス変更区分</summary>
		public string CassetteReturnExchangeStatusChange
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_EXCHANGE_STATUS_CHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_EXCHANGE_STATUS_CHANGE] = value; }
		}
		/// <summary>カセット表示用返金ステータス変更区分</summary>
		public string CassetteRepaymentStatusChange
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_REPAYMENT_STATUS_CHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_REPAYMENT_STATUS_CHANGE] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分1</summary>
		public string CassetteOrderExtendStatusChange1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE1]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE1] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分2</summary>
		public string CassetteOrderExtendStatusChange2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE2]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE2] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分3</summary>
		public string CassetteOrderExtendStatusChange3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE3]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE3] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分4</summary>
		public string CassetteOrderExtendStatusChange4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE4]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE4] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分5</summary>
		public string CassetteOrderExtendStatusChange5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE5]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE5] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分6</summary>
		public string CassetteOrderExtendStatusChange6
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE6]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE6] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分7</summary>
		public string CassetteOrderExtendStatusChange7
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE7]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE7] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分8</summary>
		public string CassetteOrderExtendStatusChange8
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE8]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE8] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分9</summary>
		public string CassetteOrderExtendStatusChange9
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE9]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE9] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分10</summary>
		public string CassetteOrderExtendStatusChange10
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE10]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE10] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_LAST_CHANGED] = value; }
		}
		/// <summary>注文拡張ステータス変更区分11</summary>
		public string OrderExtendStatusChange11
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE11]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE11] = value; }
		}
		/// <summary>注文拡張ステータス変更区分12</summary>
		public string OrderExtendStatusChange12
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE12]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE12] = value; }
		}
		/// <summary>注文拡張ステータス変更区分13</summary>
		public string OrderExtendStatusChange13
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE13]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE13] = value; }
		}
		/// <summary>注文拡張ステータス変更区分14</summary>
		public string OrderExtendStatusChange14
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE14]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE14] = value; }
		}
		/// <summary>注文拡張ステータス変更区分15</summary>
		public string OrderExtendStatusChange15
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE15]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE15] = value; }
		}
		/// <summary>注文拡張ステータス変更区分16</summary>
		public string OrderExtendStatusChange16
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE16]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE16] = value; }
		}
		/// <summary>注文拡張ステータス変更区分17</summary>
		public string OrderExtendStatusChange17
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE17]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE17] = value; }
		}
		/// <summary>注文拡張ステータス変更区分18</summary>
		public string OrderExtendStatusChange18
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE18]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE18] = value; }
		}
		/// <summary>注文拡張ステータス変更区分19</summary>
		public string OrderExtendStatusChange19
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE19]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE19] = value; }
		}
		/// <summary>注文拡張ステータス変更区分20</summary>
		public string OrderExtendStatusChange20
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE20]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE20] = value; }
		}
		/// <summary>注文拡張ステータス変更区分21</summary>
		public string OrderExtendStatusChange21
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE21]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE21] = value; }
		}
		/// <summary>注文拡張ステータス変更区分22</summary>
		public string OrderExtendStatusChange22
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE22]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE22] = value; }
		}
		/// <summary>注文拡張ステータス変更区分23</summary>
		public string OrderExtendStatusChange23
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE23]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE23] = value; }
		}
		/// <summary>注文拡張ステータス変更区分24</summary>
		public string OrderExtendStatusChange24
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE24]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE24] = value; }
		}
		/// <summary>注文拡張ステータス変更区分25</summary>
		public string OrderExtendStatusChange25
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE25]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE25] = value; }
		}
		/// <summary>注文拡張ステータス変更区分26</summary>
		public string OrderExtendStatusChange26
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE26]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE26] = value; }
		}
		/// <summary>注文拡張ステータス変更区分27</summary>
		public string OrderExtendStatusChange27
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE27]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE27] = value; }
		}
		/// <summary>注文拡張ステータス変更区分28</summary>
		public string OrderExtendStatusChange28
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE28]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE28] = value; }
		}
		/// <summary>注文拡張ステータス変更区分29</summary>
		public string OrderExtendStatusChange29
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE29]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE29] = value; }
		}
		/// <summary>注文拡張ステータス変更区分30</summary>
		public string OrderExtendStatusChange30
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE30]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE30] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分11</summary>
		public string CassetteOrderExtendStatusChange11
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE11]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE11] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分12</summary>
		public string CassetteOrderExtendStatusChange12
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE12]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE12] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分13</summary>
		public string CassetteOrderExtendStatusChange13
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE13]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE13] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分14</summary>
		public string CassetteOrderExtendStatusChange14
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE14]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE14] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分15</summary>
		public string CassetteOrderExtendStatusChange15
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE15]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE15] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分16</summary>
		public string CassetteOrderExtendStatusChange16
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE16]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE16] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分17</summary>
		public string CassetteOrderExtendStatusChange17
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE17]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE17] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分18</summary>
		public string CassetteOrderExtendStatusChange18
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE18]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE18] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分19</summary>
		public string CassetteOrderExtendStatusChange19
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE19]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE19] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分20</summary>
		public string CassetteOrderExtendStatusChange20
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE20]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE20] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分21</summary>
		public string CassetteOrderExtendStatusChange21
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE21]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE21] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分22</summary>
		public string CassetteOrderExtendStatusChange22
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE22]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE22] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分23</summary>
		public string CassetteOrderExtendStatusChange23
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE23]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE23] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分24</summary>
		public string CassetteOrderExtendStatusChange24
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE24]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE24] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分25</summary>
		public string CassetteOrderExtendStatusChange25
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE25]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE25] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分26</summary>
		public string CassetteOrderExtendStatusChange26
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE26]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE26] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分27</summary>
		public string CassetteOrderExtendStatusChange27
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE27]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE27] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分28</summary>
		public string CassetteOrderExtendStatusChange28
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE28]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE28] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分29</summary>
		public string CassetteOrderExtendStatusChange29
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE29]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE29] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分30</summary>
		public string CassetteOrderExtendStatusChange30
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE30]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE30] = value; }
		}
		/// <summary>注文拡張ステータス変更区分31</summary>
		public string OrderExtendStatusChange31
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE31]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE31] = value; }
		}
		/// <summary>注文拡張ステータス変更区分32</summary>
		public string OrderExtendStatusChange32
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE32]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE32] = value; }
		}
		/// <summary>注文拡張ステータス変更区分33</summary>
		public string OrderExtendStatusChange33
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE33]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE33] = value; }
		}
		/// <summary>注文拡張ステータス変更区分34</summary>
		public string OrderExtendStatusChange34
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE34]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE34] = value; }
		}
		/// <summary>注文拡張ステータス変更区分35</summary>
		public string OrderExtendStatusChange35
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE35]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE35] = value; }
		}
		/// <summary>注文拡張ステータス変更区分36</summary>
		public string OrderExtendStatusChange36
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE36]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE36] = value; }
		}
		/// <summary>注文拡張ステータス変更区分37</summary>
		public string OrderExtendStatusChange37
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE37]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE37] = value; }
		}
		/// <summary>注文拡張ステータス変更区分38</summary>
		public string OrderExtendStatusChange38
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE38]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE38] = value; }
		}
		/// <summary>注文拡張ステータス変更区分39</summary>
		public string OrderExtendStatusChange39
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE39]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE39] = value; }
		}
		/// <summary>注文拡張ステータス変更区分40</summary>
		public string OrderExtendStatusChange40
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE40]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE40] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分31</summary>
		public string CassetteOrderExtendStatusChange31
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE31]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE31] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分32</summary>
		public string CassetteOrderExtendStatusChange32
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE32]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE32] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分33</summary>
		public string CassetteOrderExtendStatusChange33
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE33]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE33] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分34</summary>
		public string CassetteOrderExtendStatusChange34
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE34]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE34] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分35</summary>
		public string CassetteOrderExtendStatusChange35
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE35]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE35] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分36</summary>
		public string CassetteOrderExtendStatusChange36
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE36]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE36] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分37</summary>
		public string CassetteOrderExtendStatusChange37
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE37]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE37] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分38</summary>
		public string CassetteOrderExtendStatusChange38
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE38]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE38] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分39</summary>
		public string CassetteOrderExtendStatusChange39
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE39]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE39] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分40</summary>
		public string CassetteOrderExtendStatusChange40
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE40]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE40] = value; }
		}
		/// <summary>注文拡張ステータス変更区分41</summary>
		public string OrderExtendStatusChange41
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE41]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE41] = value; }
		}
		/// <summary>注文拡張ステータス変更区分42</summary>
		public string OrderExtendStatusChange42
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE42]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE42] = value; }
		}
		/// <summary>注文拡張ステータス変更区分43</summary>
		public string OrderExtendStatusChange43
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE43]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE43] = value; }
		}
		/// <summary>注文拡張ステータス変更区分44</summary>
		public string OrderExtendStatusChange44
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE44]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE44] = value; }
		}
		/// <summary>注文拡張ステータス変更区分45</summary>
		public string OrderExtendStatusChange45
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE45]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE45] = value; }
		}
		/// <summary>注文拡張ステータス変更区分46</summary>
		public string OrderExtendStatusChange46
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE46]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE46] = value; }
		}
		/// <summary>注文拡張ステータス変更区分47</summary>
		public string OrderExtendStatusChange47
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE47]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE47] = value; }
		}
		/// <summary>注文拡張ステータス変更区分48</summary>
		public string OrderExtendStatusChange48
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE48]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE48] = value; }
		}
		/// <summary>注文拡張ステータス変更区分49</summary>
		public string OrderExtendStatusChange49
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE49]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE49] = value; }
		}
		/// <summary>注文拡張ステータス変更区分50</summary>
		public string OrderExtendStatusChange50
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE50]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE50] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分41</summary>
		public string CassetteOrderExtendStatusChange41
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE41]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE41] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分42</summary>
		public string CassetteOrderExtendStatusChange42
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE42]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE42] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分43</summary>
		public string CassetteOrderExtendStatusChange43
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE43]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE43] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分44</summary>
		public string CassetteOrderExtendStatusChange44
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE44]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE44] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分45</summary>
		public string CassetteOrderExtendStatusChange45
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE45]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE45] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分46</summary>
		public string CassetteOrderExtendStatusChange46
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE46]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE46] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分47</summary>
		public string CassetteOrderExtendStatusChange47
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE47]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE47] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分48</summary>
		public string CassetteOrderExtendStatusChange48
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE48]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE48] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分49</summary>
		public string CassetteOrderExtendStatusChange49
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE49]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE49] = value; }
		}
		/// <summary>カセット表示用注文拡張ステータス変更区分50</summary>
		public string CassetteOrderExtendStatusChange50
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE50]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE50] = value; }
		}
		/// <summary>Workflow return action</summary>
		public string WorkflowReturnAction
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_ACTION]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_ACTION] = value; }
		}
		/// <summary>Workflow cassette return action</summary>
		public string WorkflowCassetteReturnAction
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_ACTION]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_ACTION] = value; }
		}
		/// <summary>Store pickup status change</summary>
		public string StorePickupStatusChange
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STOREPICKUP_STATUS_CHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STOREPICKUP_STATUS_CHANGE] = value; }
		}
		/// <summary>Cassette store pickup status change</summary>
		public string CassetteStorePickupStatusChange
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STOREPICKUP_STATUS_CHANGE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STOREPICKUP_STATUS_CHANGE] = value; }
		}
		#endregion
	}
}
