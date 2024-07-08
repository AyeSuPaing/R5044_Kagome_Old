/*
=========================================================================================================
  Module      : 商品税率カテゴリ登録ページ処理(ProductTaxCategoryRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.Common.Web;
using w2.Domain.MasterExportSetting;
using w2.Domain.Product;
using w2.Domain.ProductTaxCategory;
using w2.Domain.SubscriptionBox;

public partial class Form_TaxCategory_TaxCategoryList : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 初期表示
		if (!IsPostBack)
		{
			var ProductTaxCategoryList = (Session[Constants.SESSIONPARAM_KEY_PRODUCT_TAX_CATEGORY_INFO] == null)
				? new ProductTaxCategoryService().GetAllTaxCategory().Select(ptcm => new ProductTaxCategoryInput(ptcm)).ToArray()
				: (ProductTaxCategoryInput[])Session[Constants.SESSIONPARAM_KEY_PRODUCT_TAX_CATEGORY_INFO];

			this.ProductTaxCategoryCount = ProductTaxCategoryList.Length;

			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponents();

			CheckMasterExportSetting();

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			DataBind(ProductTaxCategoryList.ToList());
		}
	}

	/// <summary>
	/// 一括更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAllUpdate_Click(object sender, EventArgs e)
	{
		var productTaxCategoryInputList = rProductTaxCategoryList.Items
			.Cast<RepeaterItem>()
			.Select(GetInputProductTaxCategory).ToList();

		var errorMessages = productTaxCategoryInputList
			.Select(ptci => ptci.Validate().Replace("@@ 1 @@", HtmlSanitizer.HtmlEncode(ptci.TaxCategoryId)))
			.Where(message => (string.IsNullOrEmpty(message) == false));

		if (errorMessages.Any()) RedirectErrorPage(string.Join("", errorMessages.ToArray()));

		var notRegisteredProductTaxCategoryInputList = productTaxCategoryInputList.Where(ptci => (ptci.IsRegistered == false));
		errorMessages = notRegisteredProductTaxCategoryInputList
			.Select(ptci => CheckDuplicationError(ptci, notRegisteredProductTaxCategoryInputList))
			.Where(message => (string.IsNullOrEmpty(message) == false));
		if (errorMessages.Any()) RedirectErrorPage(string.Join("</br>", errorMessages.ToArray()));

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			productTaxCategoryInputList.ToList().ForEach(ptci => UpdateProductTaxCategory(ptci, sqlAccessor));
		}

		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_TAX_CATEGORY_REGISTER);
	}

	/// <summary>
	/// 追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAdd_Click(object sender, EventArgs e)
	{
		// データ作成
		var ProductTaxCategoryInputList = rProductTaxCategoryList.Items.Cast<RepeaterItem>().Select(ri => GetInputProductTaxCategory(ri)).ToList();
		var ProductTaxCategory = new ProductTaxCategoryInput();
		ProductTaxCategory.RegisteredKbn = Constants.FLG_PRODUCT_TAXCATEGORY_KBN_NOT_REGISTERED;
		ProductTaxCategoryInputList.Add(ProductTaxCategory);
		this.ProductTaxCategoryCount += 1;

		// コンポーネント初期化
		InitializeComponents();

		// データバインド
		DataBind(ProductTaxCategoryInputList.ToList());
	}

	/// <summary>
	/// キャンセルボタンクリック ※DB未登録情報をリストから削除
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCancel_Click(object sender, EventArgs e)
	{
		// 該当情報の削除
		var ProductTaxCategoryList = rProductTaxCategoryList.Items.Cast<RepeaterItem>().Select(ri => GetInputProductTaxCategory(ri)).ToList();
		int iIndex = 0;
		if (int.TryParse(StringUtility.ToEmpty((((Button)sender).CommandArgument)), out iIndex))
		{
			ProductTaxCategoryList.Remove(ProductTaxCategoryList[iIndex]);
			this.ProductTaxCategoryCount -= 1;
		}

		// コンポーネント初期化
		InitializeComponents();

		// データバインド
		DataBind(ProductTaxCategoryList);
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		// 該当情報の削除
		var ProductTaxCategoryList = rProductTaxCategoryList.Items.Cast<RepeaterItem>().Select(ri => GetInputProductTaxCategory(ri)).ToList();
		int iIndex = 0;
		if (int.TryParse(StringUtility.ToEmpty((((Button)sender).CommandArgument)), out iIndex))
		{
			var targetTaxCategoryInput = ProductTaxCategoryList[iIndex];
			var message = CheckDeleteImpossibleError(targetTaxCategoryInput);
			if (string.IsNullOrEmpty(message))
				new ProductTaxCategoryService().Delete(targetTaxCategoryInput.CreateModel().TaxCategoryId);
			else
				RedirectErrorPage(message);
		}

		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_DELETE;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_TAX_CATEGORY_REGISTER);
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 設定情報が1件以上なら表示
		btnAllUpdateTop.Visible = btnAllUpdateBottom.Visible = (this.ProductTaxCategoryCount != 0);

		// 登録/更新完了メッセージ表示制御
		dvUpdateComplete.Visible = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ACTION_STATUS]) == Constants.ACTION_STATUS_COMPLETE;
		dvDeleteComplete.Visible = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ACTION_STATUS]) == Constants.ACTION_STATUS_DELETE;
		dvMessage.Visible = dvUpdateComplete.Visible || dvDeleteComplete.Visible;
		Session[Constants.SESSION_KEY_ACTION_STATUS] = null;

		Session[Constants.SESSIONPARAM_KEY_PRODUCT_TAX_CATEGORY_INFO] = null;
	}

	/// <summary>
	/// データバインド
	/// </summary>
	/// <param name="ProductTaxCategoryInputList">商品税率カテゴリインプットのリスト</param>
	private void DataBind(List<ProductTaxCategoryInput> ProductTaxCategoryInputList)
	{
		rProductTaxCategoryList.DataSource = ProductTaxCategoryInputList;
		rProductTaxCategoryList.DataBind();
	}

	/// <summary>
	/// 商品税率カテゴリ重複チェック
	/// </summary>
	/// <param name="ptci">商品税率カテゴリインプット(未登録)</param>
	/// <param name="ptciList">商品税率カテゴリインプットリスト(未登録)</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckDuplicationError(ProductTaxCategoryInput ptci, IEnumerable<ProductTaxCategoryInput> ptciList)
	{
		var errorMessage = string.Empty;
		if (new ProductTaxCategoryService().Get(ptci.TaxCategoryId) != null) errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_TAX_CATEGORY_DUPLICATE_ERROR).Replace("@@ 1 @@", ptci.TaxCategoryId);
		if (ptciList.Count(listPci => (ptci.TaxCategoryId == listPci.TaxCategoryId)) > 1) errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_TAX_CATEGORY_DUPLICATE_ERROR).Replace("@@ 1 @@", ptci.TaxCategoryId);
		return errorMessage;
	}

	/// <summary>
	/// 商品税率カテゴリ削除不可チェック
	/// </summary>
	/// <param name="ptci">商品税率カテゴリインプット(未登録)</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckDeleteImpossibleError(ProductTaxCategoryInput ptci)
	{
		var product = new ProductService().GetProductByTaxCategoryId(ptci.TaxCategoryId);
		if (product != null)
		{
			var errorMessage = WebMessages
				.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_TAX_CATEGORY_DELETE_IMPOSSIBLE_ERROR)
				.Replace("@@ 1 @@", ptci.TaxCategoryId);
			return errorMessage;
		}

		var category = new SubscriptionBoxService().GetByTaxCategoryId(ptci.TaxCategoryId);
		if (category != null)
		{
			var errorMessage = WebMessages
				.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_TAX_CATEGORY_DELETE_IMPOSSIBLE_ERROR_BY_SUBSCRIPTION_BOX)
				.Replace("@@ 1 @@", ptci.TaxCategoryId);
			return errorMessage;
		}
		return string.Empty;
	}

	/// <summary>
	/// 商品税率カテゴリ更新
	/// </summary>
	/// <param name="ptci">商品税率カテゴリインプット(未登録)</param>
	/// <param name="accesor">アクセサ</param>
	private void UpdateProductTaxCategory(ProductTaxCategoryInput ptci, SqlAccessor accesor)
	{
		var productTaxCategoryService = new ProductTaxCategoryService();
		if (ptci.IsRegistered) productTaxCategoryService.Update(ptci.CreateModel());
		else productTaxCategoryService.Insert(ptci.CreateModel());
	}

	/// <summary>
	/// 画面からデータ取得
	/// </summary>
	/// <param name="ri"></param>
	/// <returns>商品税率カテゴリ情報</returns>
	protected ProductTaxCategoryInput GetInputProductTaxCategory(RepeaterItem ri)
	{
		var productTaxCategoryInput = new ProductTaxCategoryInput
		{
			TaxCategoryId = ((TextBox)ri.FindControl("tbProductTaxCategoryId")).Text.Trim(),
			TaxCategoryName = ((TextBox)ri.FindControl("tbProductTaxCategoryName")).Text.Trim(),
			TaxRate = ((TextBox)ri.FindControl("tbTaxRate")).Text.Trim(),
			DisplayOrder = ((TextBox)ri.FindControl("tbDisplayOrder")).Text.Trim(),
			LastChanged = this.LoginOperatorName,
			RegisteredKbn = ((HiddenField)ri.FindControl("hfRegisteredKbn")).Value
		};

		return productTaxCategoryInput;
	}

	/// <summary>
	/// エラーページへ遷移
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	private void RedirectErrorPage(string errorMessage)
	{
		Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
		var ProductTaxCategoryInputList = rProductTaxCategoryList.Items
			.Cast<RepeaterItem>()
			.Select(ri => GetInputProductTaxCategory(ri)).ToArray();
		Session[Constants.SESSIONPARAM_KEY_PRODUCT_TAX_CATEGORY_INFO] = ProductTaxCategoryInputList;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
	}

	/// <summary>
	/// マスタ出力設定値のチェック
	/// </summary>
	private void CheckMasterExportSetting()
	{
		var illegalTaxRate = new List<string>();

		var taxFields = new ProductTaxCategoryService().GetMasterExportSettingFieldNames();
		var masterExportSettingService = new MasterExportSettingService();
		var masterExportSettingOrder = masterExportSettingService.GetAllByMaster(
			this.LoginOperatorShopId,
			Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER);
		foreach (var settings in masterExportSettingOrder)
		{
			var fields = StringUtility.ToEmpty(settings.Fields);
			var settingTaxFieldNames = StringUtility.SplitCsvLine(fields)
				.Where(name => name.Contains("_by_rate_"))
				.Distinct();
			illegalTaxRate.AddRange(settingTaxFieldNames
				.Where(name =>
					taxFields.Any(taxField => name == taxField) == false).ToArray());
		}

		var masterExportSettingOrderItem = masterExportSettingService.GetAllByMaster(
			this.LoginOperatorShopId,
			Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM);
		foreach (var settings in masterExportSettingOrderItem)
		{
			var fields = StringUtility.ToEmpty(settings.Fields);
			var settingTaxFieldNames = StringUtility.SplitCsvLine(fields)
				.Where(name => name.Contains("_by_rate_"))
				.Distinct();
			illegalTaxRate.AddRange(settingTaxFieldNames
				.Where(name =>
					taxFields.Any(taxField => name == taxField) == false).ToArray());
		}

		if (illegalTaxRate.Any())
		{
			var masterName = string.Format(ReplaceTag("@@DispText.common_message.condition_AorB@@"),
				ValueText.GetValueText(
					Constants.TABLE_MASTEREXPORTSETTING,
					Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN,
					Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER),
				ValueText.GetValueText(
					Constants.TABLE_MASTEREXPORTSETTING,
					Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN,
					Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM));
			lAlert.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CSV_OUTPUT_TAX_FIELD_ERROR).Replace("@@ 1 @@", string.Join("<br>", illegalTaxRate.Distinct())).Replace("@@ 2 @@", masterName);
		}
	}

	/// <summary>商品税率カテゴリの件数</summary>
	protected int ProductTaxCategoryCount
	{
		get { return (int)ViewState["ProductTaxCategoryCount"]; }
		private set { ViewState["ProductTaxCategoryCount"] = value; }
	}
}