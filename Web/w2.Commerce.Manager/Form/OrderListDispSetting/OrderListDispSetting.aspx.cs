/*
=========================================================================================================
  Module      : 受注情報一覧表示設定処理(OrderListDispSetting.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.Domain.ManagerListDispSetting;
using w2.Domain.OrderExtendStatusSetting;

public partial class Form_Order_OrderListDispSetting : BasePage
{
	/// <summary>
	/// 定数：タブ選択
	/// </summary>
	private const string FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_SELECT = "select";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 初回読み込みの場合、受注情報のタブを選択する
		if (!IsPostBack)
		{
			hfDispSettingKbn.Value = Constants.FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_ORDERLIST;
			lbChangeToOrderList.CssClass = FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_SELECT;
			lbOrderWorkflow.CssClass = "";
			OrderDipsSettingBinder();
		}
	}

	/// <summary>
	/// タブをクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void SelectTab_Onclick(object sender, CommandEventArgs e)
	{
		switch (e.CommandName)
		{
			// 受注情報
			case "orderListTab":
				hfDispSettingKbn.Value = Constants.FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_ORDERLIST;
				lbChangeToOrderList.CssClass = FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_SELECT;
				lbOrderWorkflow.CssClass = "";
				lbOrderStorePickup.CssClass = string.Empty;
				break;

			// 受注ワークフロー
			case "orderWorkFlowTab":
				hfDispSettingKbn.Value = Constants.FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_ORDERWORKFLOW;
				lbChangeToOrderList.CssClass = "";
				lbOrderStorePickup.CssClass = string.Empty;
				lbOrderWorkflow.CssClass = FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_SELECT;
				break;

			// 店舗受取注文情報
			case "orderStorePickupListTab":
				hfDispSettingKbn.Value = Constants.FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_ORDERSTOREPICKUP;
				lbChangeToOrderList.CssClass = string.Empty;
				lbOrderWorkflow.CssClass = string.Empty;
				lbOrderStorePickup.CssClass = FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_SELECT;
				break;

		}
		OrderDipsSettingBinder();
	}

	/// <summary>
	/// 表示設定先区分を参照して設定情報をrManagerListDispSettingにバインドする
	/// </summary>
	protected void OrderDipsSettingBinder()
	{
		var managerListDispSettings = new List<ManagerListDispSettingModel>();
		var dispSettings = new ManagerListDispSettingService().GetAllByDispSettingKbn(this.LoginOperatorShopId, hfDispSettingKbn.Value);
		var service = new ManagerListDispSettingService();
		foreach (var dispSetting in dispSettings)
		{
			// オプション連携フラグがfalse または 登録されていない拡張ステータス項目かどうか
			if ((IsOptionCooperation(dispSetting.DispColmunName)) 
				|| (IsExtendColumnNameRegistered(dispSetting.DispColmunName) == false))
			{
				service.UpdateDispSettingFlagOff(
					this.LoginOperatorShopId,
					this.LoginOperatorId,
					hfDispSettingKbn.Value,
					dispSetting.DispColmunName);
				continue;
			}

			// 注文IDは固定表示するためsortbleにバインドしない。
			if (dispSetting.DispColmunName == Constants.FIELD_ORDER_ORDER_ID)
			{
				lColmunName.Text = HttpUtility.HtmlEncode(ColumunNameConversionToLogicalName(dispSetting.DispColmunName));
				continue;
			}
			managerListDispSettings.Add(dispSetting);
		}

		rManagerListDispSetting.DataSource = managerListDispSettings;
		rManagerListDispSetting.DataBind();
	}

	/// <summary>
	/// 更新ボタンをクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void btnUpdateTop_OnClick(object sender, EventArgs e)
	{
		var service = new ManagerListDispSettingService();
		foreach (RepeaterItem ri in rManagerListDispSetting.Items)
		{
			var dispOrder = ((HtmlInputHidden)ri.FindControl("hdDispOrderNumber")).Value;
			if (string.IsNullOrEmpty(dispOrder)) continue;

			service.UpdateDispSetting(
				new ManagerListDispSettingModel
				{
					ShopId = this.LoginOperatorShopId,
					DispSettingKbn = hfDispSettingKbn.Value,
					DispColmunName = ((HtmlInputHidden)ri.FindControl("hdColmunName")).Value,
					DispFlag = ((((CheckBox)ri.FindControl("cbDispFlag")).Checked)
						? Constants.FLG_MANAGERLISTDISPSETTING_DISP_FLAG_ON
						: Constants.FLG_MANAGERLISTDISPSETTING_DISP_FLAG_OFF),
					DispOrder = int.Parse(dispOrder),
					DateChanged = DateTime.Now,
					LastChanged = this.LoginOperatorId,
				});
		}

		OrderDipsSettingBinder();
	}

	/// <summary>
	/// 拡張ステータス設定で登録されている、または通常項目か
	/// </summary>
	/// <param name="columnName">項目名</param>
	/// <returns>結果</returns>
	protected bool IsExtendColumnNameRegistered(string columnName)
	{
		if (columnName.Contains(Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME) == false) return true;

		var isRegistered = new OrderExtendStatusSettingService()
			.GetOrderExtendStatusSetting()
			.Any(extend => ((Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + extend.ExtendStatusNo) == columnName));
		return isRegistered;
	}

	/// <summary>
	/// 表示項目名を物理名から論理名に置換する
	/// </summary>
	/// <param name="columnName">項目名</param>
	/// <returns>結果</returns>
	protected string ColumunNameConversionToLogicalName(string columnName)
	{
		string resultColumnName;

		// カラムが拡張ステータスなら、拡張ステータスをつける
		if (columnName.Contains(Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME))
		{
			var extendS = ValueText.GetValueText(
				Constants.TABLE_MANAGERLISTDISPSETTING,
				Constants.FIELD_MANAGERLISTDISPSETTING_DISP_COLUMN_NAME,
				Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME);
			var dispExtendStatus = new OrderExtendStatusSettingService().GetOrderExtendStatusSetting();
			foreach (var extendStatus in dispExtendStatus)
			{
				if (((Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + extendStatus.ExtendStatusNo) == columnName) == false) continue;
				resultColumnName = string.Format("{0}{1}：{2}", extendS, extendStatus.ExtendStatusNo, extendStatus.ExtendStatusName);
				return resultColumnName;
			}

			resultColumnName = string.Format("{0}{1}：", extendS, columnName.Replace(Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME, string.Empty));
			return resultColumnName;
		}

		resultColumnName = ValueText.GetValueText(
			Constants.TABLE_MANAGERLISTDISPSETTING,
			Constants.FIELD_MANAGERLISTDISPSETTING_DISP_COLUMN_NAME,
			columnName);
		return resultColumnName;
	}

	/// <summary>
	/// チェックボックス一括切り替えボタンの値を保持する
	/// </summary>
	/// <returns>結果</returns>
	protected bool CheckedCheckBox()
	{
		foreach (RepeaterItem ri in rManagerListDispSetting.Items)
		{
			if ((((CheckBox)ri.FindControl("cbDispFlag")).Checked)) return false;
		}

		return false;
	}

	/// <summary>オプション連携フラグの設定を確認して項目の表示制御を行う</summary>
	/// <param name="columnName">項目名</param>
	/// <returns>結果</returns>
	protected bool IsOptionCooperation(string columnName)
	{
		switch (columnName)
		{
			// モール連携OP
			case Constants.FIELD_USER_MALL_ID:
				if ((Constants.MALLCOOPERATION_OPTION_ENABLED == false) || (Constants.URERU_AD_IMPORT_ENABLED == false)) return true;
				break;

			// リアル店舗OP
			case Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS:
			case Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS:
				if (Constants.REALSTOCK_OPTION_ENABLED == false) return true;
				break;

			// デジタルコンテンツOP
			case Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG:
				if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED == false) return true;
				break;

			// ギフトOP
			case Constants.FIELD_ORDER_GIFT_FLG:
				if (Constants.GIFTORDER_OPTION_ENABLED == false) return true;
				break;
		}

		return false;
	}
}