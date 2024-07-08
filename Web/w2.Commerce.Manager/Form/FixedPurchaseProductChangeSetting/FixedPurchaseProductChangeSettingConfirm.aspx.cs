/*
=========================================================================================================
  Module      : 定期商品変更設定詳細/確認画面処理(FixedPurchaseProductChangeSettingConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order;
using w2.Common.Web;
using w2.Domain.FixedPurchaseProductChangeSetting;

public partial class Form_FixedPurchaseProductChangeSetting_FixedPurchaseProductChangeSettingConfirm : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// コンポーネント初期化
			InitializeComponent();

			// データ取得
			switch (this.ActionStatus)
			{
				case Constants.ACTION_STATUS_DETAIL:
					this.Input = GetFixedPurchaseProductChangeSetting();
					break;

				case Constants.ACTION_STATUS_INSERT:
				case Constants.ACTION_STATUS_COPY_INSERT:
				case Constants.ACTION_STATUS_UPDATE:
					this.Input = (FixedPurchaseProductChangeSettingInput)Session[Constants.SESSION_KEY_PARAM_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING];
					break;
			}

			// 定期商品変更設定を画面にセット
			SetFixedPurchaseProductChangeSetting();

			DataBind();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponent()
	{
		// ボタン表示
		btnBuckHistoryBackTop.Visible = btnBuckHistoryBackBottom.Visible = (this.ActionStatus != Constants.ACTION_STATUS_DETAIL);
		btnBackListPageTop.Visible = btnBackListPageBottom.Visible = (this.ActionStatus == Constants.ACTION_STATUS_DETAIL);
		btnEditTop.Visible = btnEditBottom.Visible = (this.ActionStatus == Constants.ACTION_STATUS_DETAIL);
		btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = (this.ActionStatus == Constants.ACTION_STATUS_DETAIL);
		btnDeleteTop.Visible = btnDeleteBottom.Visible = (this.ActionStatus == Constants.ACTION_STATUS_DETAIL);
		btnInsertTop.Visible = btnInsertBottom.Visible = ((this.ActionStatus == Constants.ACTION_STATUS_INSERT) || (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT));
		btnUpdateTop.Visible = btnUpdateBottom.Visible = (this.ActionStatus == Constants.ACTION_STATUS_UPDATE);

		// 項目非表示
		if (this.ActionStatus != Constants.ACTION_STATUS_DETAIL)
		{
			trDateCreated.Visible = false;
			trDateChanged.Visible = false;
			trLastChanged.Visible = false;
		}
	}

	/// <summary>
	/// 定期商品変更設定取得
	/// </summary>
	/// <returns>定期商品変更設定</returns>
	private FixedPurchaseProductChangeSettingInput GetFixedPurchaseProductChangeSetting()
	{
		var fixedPurchaseProductChangeId = Request[Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID];
		var container = new FixedPurchaseProductChangeSettingService().GetContainer(fixedPurchaseProductChangeId);
		return new FixedPurchaseProductChangeSettingInput(container);
	}

	/// <summary>
	/// 定期商品変更設定を画面にセット
	/// </summary>
	private void SetFixedPurchaseProductChangeSetting()
	{
		lFixedPurchaseProductChangeId.Text = WebSanitizer.HtmlEncode(
			this.Input.FixedPurchaseProductChangeId);
		lFixedPurchaseProductChangeName.Text = WebSanitizer.HtmlEncode(
			this.Input.FixedPurchaseProductChangeName);
		lPriority.Text = WebSanitizer.HtmlEncode(
			this.Input.Priority);
		lValidFlg.Text = WebSanitizer.HtmlEncode(
			ValueText.GetValueText(
				Constants.TABLE_FIXEDPURCHASEPRODUCTCHANGESETTING,
				Constants.FIELD_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG,
				this.Input.ValidFlg));

		if (this.ActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			lbDateCreated.Text = this.Input.DateCreated;
			lbDateChanged.Text = this.Input.DateChanged;
			lbLastChanged.Text = this.Input.LastChanged;
		}

		rBeforeChangeItems.DataSource = this.Input.BeforeChangeItems;
		rAfterChangeItems.DataSource = this.Input.AfterChangeItems;
	}

	/// <summary>
	/// 一覧に戻るボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBackListPage_Click(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_LIST)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBuckHistoryBack_Click(object sender, EventArgs e)
	{
		var uniqueKey = CreateUniqueKeyForSaveFixedPurchaseProductChangeSettingInput(this.ActionStatus, this.Input.FixedPurchaseProductChangeId);
		Session[Constants.SESSION_KEY_PARAM] = null;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK + uniqueKey] = this.Input;
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus)
			.AddParam(Constants.REQUEST_KEY_UNIQUE_KEY, uniqueKey)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 登録ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		// 商品の存在チェック
		var errorMessage = CheckExistProducts();
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 登録
		new FixedPurchaseProductChangeSettingService().Insert(this.Input.CreateModel());

		// リダイレクト
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
			.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID, this.Input.FixedPurchaseProductChangeId)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 更新ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		// 商品の存在チェック
		var errorMessage = CheckExistProducts();
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 更新
		new FixedPurchaseProductChangeSettingService().Update(this.Input.CreateModel());

		// リダイレクト
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
			.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID, this.Input.FixedPurchaseProductChangeId)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 編集ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING] = this.Input;
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// コピー新規登録ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING] = this.Input;
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_COPY_INSERT)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 削除ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>

	protected void btnDelete_Click(object sender, EventArgs e)
	{
		new FixedPurchaseProductChangeSettingService().Delete(this.Input.FixedPurchaseProductChangeId);
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_LIST).CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 商品の存在チェック
	/// </summary>
	/// <returns>存在チェック結果</returns>
	private string CheckExistProducts()
	{
		var errorMessages = string.Empty;
		foreach (var beforeChangeItem in this.Input.BeforeChangeItems)
		{
			var product = ProductCommon.GetProductInfoUnuseMemberRankPrice(beforeChangeItem.ShopId, beforeChangeItem.ProductId);
			if (product.Count == 0)
			{
				errorMessages += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_DELETE).Replace("@@ 1 @@", beforeChangeItem.ProductName);
			}
		}
		foreach (var afterChangeItem in this.Input.AfterChangeItems)
		{
			var product = ProductCommon.GetProductInfoUnuseMemberRankPrice(afterChangeItem.ShopId, afterChangeItem.ProductId);
			if (product.Count == 0)
			{
				errorMessages += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_DELETE).Replace("@@ 1 @@", afterChangeItem.ProductName);
			}
		}
		return errorMessages;
	}

	/// <summary>定期商品変更設定入力クラス</summary>
	private FixedPurchaseProductChangeSettingInput Input
	{
		get { return (FixedPurchaseProductChangeSettingInput)ViewState["Input"]; }
		set { ViewState["Input"] = value; }
	}
}
