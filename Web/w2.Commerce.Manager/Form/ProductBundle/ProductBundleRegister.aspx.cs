/*
=========================================================================================================
 Module      : 商品同梱設定・編集ページ(ProductBundleRegister.aspx.cs)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Manager.Menu;
using w2.Domain.MenuAuthority.Helper;
using w2.Domain.MenuAuthority;
using w2.Domain.ProductBundle;
using w2.Common.Web;

/// <summary>
/// 商品同梱設定・編集ページ
/// </summary>
public partial class Form_ProductBundle_ProductBundleRegister : ProductBundlePage
{
	#region 定数
	/// <summary>ポップアップでの検索区分：対象商品 商品ID</summary>
	protected const string POPUP_PRODUCT_SEARCH_KBN_TARGET_PRODUCT = "target_product";
	/// <summary>ポップアップでの検索区分：対象商品 バリエーションID</summary>
	protected const string POPUP_PRODUCT_SEARCH_KBN_TARGET_VARIATION = "target_variation";
	/// <summary>ポップアップでの検索区分：対象商品 商品カテゴリID</summary>
	protected const string POPUP_PRODUCT_SEARCH_KBN_TARGET_CATEGORY = "target_product_category";
	/// <summary>ポップアップでの検索区分：対象外商品 商品ID</summary>
	protected const string POPUP_PRODUCT_SEARCH_KBN_EXCEPT_PRODUCT = "except_product";
	/// <summary>ポップアップでの検索区分：対象外商品 バリエーションID</summary>
	protected const string POPUP_PRODUCT_SEARCH_KBN_EXCEPT_VARIATION = "except_variation";
	/// <summary>ポップアップでの検索区分：対象外商品 商品カテゴリID</summary>
	protected const string POPUP_PRODUCT_SEARCH_KBN_EXCEPT_PRODUCT_CATEGORY = "except_product_category";
	/// <summary>ポップアップでの検索区分：ターゲットリスト</summary>
	protected const string POPUP_PRODUCT_SEARCH_KBN_TARGETLIST = "targetlist";
	/// <summary>ポップアップでの検索区分：初回広告コード</summary>
	protected const string POPUP_PRODUCT_SEARCH_KBN_TARGET_ADVCODES_FIRST = "target_advcodes_first";
	/// <summary>ポップアップでの検索区分：最新広告コード</summary>
	protected const string POPUP_PRODUCT_SEARCH_KBN_TARGET_ADVCODES_NEW = "target_advcodes_new";
	/// <summary>ポップアップでの検索区分：同梱商品ID</summary>
	protected const string POPUP_PRODUCT_SEARCH_KBN_GRANT_PRODUCT = "grant_product";
	/// <summary>対象商品ID入力欄リサイズボタンコマンド</summary>
	protected const string RESIZE_PRODUCT_TEXTBOX = "lbProductResize";
	/// <summary>対象バリエーションID入力欄リサイズボタンコマンド</summary>
	protected const string RESIZE_PRODUCT_VARIATION_TEXTBOX = "lbProductVariationResize";
	/// <summary>対象商品カテゴリID入力欄リサイズボタンコマンド</summary>
	protected const string RESIZE_PRODUCT_CATEGORY_TEXTBOX = "lbProductCategoryResize";
	/// <summary>対象外商品ID入力欄リサイズボタンコマンド</summary>
	protected const string RESIZE_EXCEPT_PRODUCT_TEXTBOX = "lbExceptProductResize";
	/// <summary>対象外バリエーションID入力欄リサイズボタンコマンド</summary>
	protected const string RESIZE_EXCEPT_PRODUCT_VARIATION_TEXTBOX = "lbExceptProductVariationResize";
	/// <summary>対象外商品カテゴリID入力欄リサイズボタンコマンド</summary>
	protected const string RESIZE_EXCEPT_PRODUCT_CATEGORY_TEXTBOX = "lbExceptProductCategoryResize";
	/// <summary>初回広告コード入力欄リサイズボタンコマンド</summary>
	protected const string RESIZE_TARGET_ADVCODES_FIRST_TEXTBOX = "lbTargetAdvCodesFirstResize";
	/// <summary>最新広告コード入力欄リサイズボタンコマンド</summary>
	protected const string RESIZE_TARGET_ADVCODES_NEW_TEXTBOX = "lbTargetAdvCodesNewResize";
	/// <summary>クーポンコード入力欄リサイズボタンコマンド</summary>
	private const string RESIZE_TARGET_COUPON_CODE_TEXTBOX = "lbTargetCouponCodesResize";
	/// <summary>入力欄標準行数</summary>
	private const int TEXTAREA_ROWS_NORMAL = 3;
	/// <summary>対象商品・バリエーションID入力欄拡大時行数</summary>
	private const int TEXTAREA_ROWS_LARGE = 10;
	/// <summary>同梱商品個数初期値</summary>
	private const int DEFAULT_GRANT_PRODUCT_COUNT = 1;
	/// <summary>同梱商品ボタン処理名：追加</summary>
	private const string COMMAND_NAME_ADD_PRODUCT = "Add";
	/// <summary>同梱商品ボタン処理名：削除</summary>
	private const string COMMAND_NAME_DELETE_PRODUCT = "Delete";
	/// <summary>登録処理区分</summary>
	private enum RegisterMode
	{
		/// <summary>新規登録</summary>
		Insert,
		/// <summary>更新</summary>
		Update
	}
	/// <summary>クーポンメニューフォルダパス</summary>
	private const string FOLDER_PATH_COUPON_MENU = "Form/Coupon/";
	#endregion

	#region イベントハンドラ
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			InitializeComponents();

			ViewProductBundleInfo();
		}
	}
	#endregion

	#region #btnBack_Click 一覧へ戻るボタンクリック
	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		var listParameters = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTBUNDLE_SEARCH_INFO] ?? CreateDefaultListParameters();
		Response.Redirect(CreateProductBundleListUrl(listParameters, true));
	}
	#endregion

	#region #rGrantProductIdList_ItemCommand 同梱商品ID入力欄のボタンクリック
	/// <summary>
	/// 同梱商品ID入力欄のボタンクリック
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rGrantProductIdList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		switch (e.CommandName)
		{
			case COMMAND_NAME_DELETE_PRODUCT:	// 削除ボタン
				DeleteAnGrantProductIdInput(int.Parse(e.CommandArgument.ToString()));
				break;
		}
	}
	#endregion

	#region -btnUpdate_Click 更新するボタンクリック
	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		ProductBundleRegister(RegisterMode.Update);
	}
	#endregion

	#region -btnInsert_Click 登録するボタンクリック
	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		ProductBundleRegister(RegisterMode.Insert);
	}
	#endregion

	#region -btnDelete_Click 削除するボタンクリック
	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		ProductBundleDelete();
	}
	#endregion

	#region -btnCopyInsert_Click コピー新規登録するボタンクリック
	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		var url = CreateProductBundleRegister(Constants.ACTION_STATUS_COPY_INSERT, this.ProductBundleInfo.ProductBundleId);
		Response.Redirect(url);
	}
	#endregion
	
	#region #lbResize_Click 拡大縮小ボタンクリック
	/// <summary>
	/// 拡大縮小ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbResize_Click(object sender, EventArgs e)
	{
		switch (((LinkButton)sender).CommandName)
		{
			case RESIZE_PRODUCT_TEXTBOX:
				this.ProductIdRowsCount = (IsSizeNormal(this.ProductIdRowsCount)) ? TEXTAREA_ROWS_LARGE : TEXTAREA_ROWS_NORMAL;
				break;

			case RESIZE_PRODUCT_VARIATION_TEXTBOX:
				this.ProductVariationIdRowsCount =
					(IsSizeNormal(this.ProductVariationIdRowsCount)) ? TEXTAREA_ROWS_LARGE : TEXTAREA_ROWS_NORMAL;
				break;

			case RESIZE_PRODUCT_CATEGORY_TEXTBOX:
				this.ProductCategoryIdRowsCount =
					(IsSizeNormal(this.ProductCategoryIdRowsCount)) ? TEXTAREA_ROWS_LARGE : TEXTAREA_ROWS_NORMAL;
				break;

			case RESIZE_EXCEPT_PRODUCT_TEXTBOX:
				this.ExcludedProductIdRowsCount =
					(IsSizeNormal(this.ExcludedProductIdRowsCount)) ? TEXTAREA_ROWS_LARGE : TEXTAREA_ROWS_NORMAL;
				break;

			case RESIZE_EXCEPT_PRODUCT_VARIATION_TEXTBOX:
				this.ExcludedProductVariationIdRowsCount =
					(IsSizeNormal(this.ExcludedProductVariationIdRowsCount)) ? TEXTAREA_ROWS_LARGE : TEXTAREA_ROWS_NORMAL;
				break;

			case RESIZE_EXCEPT_PRODUCT_CATEGORY_TEXTBOX:
				this.ExcludedProductCategoryIdRowsCount =
					(IsSizeNormal(this.ExcludedProductCategoryIdRowsCount)) ? TEXTAREA_ROWS_LARGE : TEXTAREA_ROWS_NORMAL;
				break;

			case RESIZE_TARGET_ADVCODES_FIRST_TEXTBOX:
				this.AdvCodeFirstRowsCount =
					(IsSizeNormal(this.AdvCodeFirstRowsCount)) ? TEXTAREA_ROWS_LARGE : TEXTAREA_ROWS_NORMAL;
				break;

			case RESIZE_TARGET_ADVCODES_NEW_TEXTBOX:
				this.AdvCodeNewRowsCount =
					(IsSizeNormal(this.AdvCodeNewRowsCount)) ? TEXTAREA_ROWS_LARGE : TEXTAREA_ROWS_NORMAL;
				break;

			case RESIZE_TARGET_COUPON_CODE_TEXTBOX:
				this.TargetCouponCodesRowsCount =
					(IsSizeNormal(this.TargetCouponCodesRowsCount)) ? TEXTAREA_ROWS_LARGE : TEXTAREA_ROWS_NORMAL;
				break;

			default:
				return;
		}

		SetTextAreaSize();
	}
	#endregion

	#region btnTargetProductIdAdd_Click 同梱商品行追加ボタンクリック
	/// <summary>
	/// 同梱商品行追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnTargetProductIdAdd_Click(object sender, EventArgs e)
	{
		CreateAnEmptyGrantProductIdInput();
	}
	#endregion
	#endregion

	#region 画面初期化
	#region -InitializeComponents 各種コントロール初期化
	/// <summary>
	/// 各種コントロール初期化
	/// </summary>
	private void InitializeComponents()
	{
		if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
			|| (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE))
		{
			trEdit.Visible = true;
			btnUpdateTop.Visible = btnUpdateBottom.Visible = true;
			btnDeleteTop.Visible = btnDeleteBottom.Visible = true;
			btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = true;
			tbBundleId.Visible = false;
		}
		else if ((this.ActionStatus == Constants.ACTION_STATUS_INSERT)
			|| (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
		{
			trRegister.Visible = true;
			btnInsertTop.Visible = btnInsertBottom.Visible = true;
			lBundleId.Visible = false;
		}

		if (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE)
		{
			lMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTBUNDLE_REGIST_UPDATE_SUCCESS);
			divComp.Visible = true;
		}

		if (Constants.PRODUCT_CTEGORY_OPTION_ENABLE) trProductCategoryID.Visible = true;
		
		if (this.IsPopUp)
		{
			btnBack.Visible = btnBackBottom.Visible = false;
		}

		rblTargetOrderType.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_PRODUCTBUNDLE,
				Constants.SUBSCRIPTION_BOX_OPTION_ENABLED
					? Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_TYPE_SUBSCRIPTION_BOX
					: Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_TYPE));

		// 決済種別
		foreach (DataRowView payment in GetPaymentValidList(this.LoginOperatorShopId))
		{
			cblTargetPaymentIds.Items.Add(
				new ListItem(
					(string)payment[Constants.FIELD_PAYMENT_PAYMENT_NAME],
					(string)payment[Constants.FIELD_PAYMENT_PAYMENT_ID]));
		}

		SetUsableTimeskbnItems();
		SetApplyTypeItems();
		SetTextAreaSize();
	}
	#endregion

	#region -SetUsableTimesKbnItems ユーザ利用可能回数ラジオボタンの初期化
	/// <summary>
	/// ユーザ利用可能回数ラジオボタンの初期化
	/// </summary>
	private void SetUsableTimeskbnItems()
	{
		var usableTimesKbn = new Dictionary<string, string>();
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_PRODUCTBUNDLE, Constants.FIELD_PRODUCTBUNDLE_USABLE_TIMES_KBN))
		{
			usableTimesKbn.Add(li.Value, li.Text);
		}
		rblUsableTimesKbn.DataSource = usableTimesKbn;
		rblUsableTimesKbn.DataTextField = "Value";
		rblUsableTimesKbn.DataValueField = "Key";
		rblUsableTimesKbn.DataBind();
	}
	#endregion

	#region -SetApplyTypeDropDownList 商品同梱設定適用種別指定用DropDownListの初期化
	/// <summary>
	/// 商品同梱設定適用種別指定用ラジオボタンの初期化
	/// </summary>
	private void SetApplyTypeItems()
	{
		var applyType = new Dictionary<string, string>();
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_PRODUCTBUNDLE, Constants.FIELD_PRODUCTBUNDLE_APPLY_TYPE))
		{
			applyType.Add(li.Value, li.Text);
		}
		rblApplyType.DataSource = applyType;
		rblApplyType.DataTextField = "Value";
		rblApplyType.DataValueField = "Key";
		rblApplyType.DataBind();
	}
	#endregion
	#endregion

	#region -ViewProductBundleInfo 商品同梱情報表示
	/// <summary>
	/// 商品同梱情報表示
	/// </summary>
	private void ViewProductBundleInfo()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_COMPLETE:
			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
				GetProductBundleInfo();
				tbBundleId.Text = this.ProductBundleInfo.ProductBundleId;
				lBundleId.Text = this.ProductBundleInfo.ProductBundleId;
				tbBundleName.Text = this.ProductBundleInfo.ProductBundleName;
				rblTargetOrderType.SelectedValue = this.ProductBundleInfo.TargetOrderType;
				cbMultipleApplyFlg.Checked = (this.ProductBundleInfo.MultipleApplyFlg == Constants.FLG_PRODUCTBUNDLE_MULTIPLE_APPLY_FLG_VALID);
				tbApplyOrder.Text = this.ProductBundleInfo.ApplyOrder.ToString();
				tbDescription.Text = this.ProductBundleInfo.Description;
				if (this.ProductBundleInfo.EndDatetime.HasValue)
				{
					ucDisplayPeriod.SetPeriodDate(this.ProductBundleInfo.StartDatetime, this.ProductBundleInfo.EndDatetime.Value);
				}
				else
				{
					ucDisplayPeriod.SetStartDate(this.ProductBundleInfo.StartDatetime);
				}
				rblTargetProductKbn.SelectedValue = this.ProductBundleInfo.TargetProductKbn;
				if (this.ProductBundleInfo.TargetProductKbn == Constants.FLG_PRODUCTBUNDLE_TARGET_PRODUCT_KBN_SELECT)
				{
					tbProductId.Text = string.Join(Environment.NewLine, this.ProductBundleInfo.GetTargetProductIds());
					tbProductVariationId.Text = string.Join(Environment.NewLine, this.ProductBundleInfo.GetTargetProductVariationIds());
					tbProductCategoryId.Text = this.ProductBundleInfo.TargetProductCategoryIds;
				}
				tbTargetOrderFixedPurchaseCountFrom.Text = ((this.ProductBundleInfo.TargetOrderType != Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_NORMAL)
					&& this.ProductBundleInfo.TargetOrderFixedPurchaseCountFrom.HasValue)
					? this.ProductBundleInfo.TargetOrderFixedPurchaseCountFrom.ToString()
					: string.Empty;
				tbTargetOrderFixedPurchaseCountTo.Text = ((this.ProductBundleInfo.TargetOrderType != Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_NORMAL)
					&& this.ProductBundleInfo.TargetOrderFixedPurchaseCountTo.HasValue)
					? this.ProductBundleInfo.TargetOrderFixedPurchaseCountTo.ToString()
					: string.Empty;
				rblUsableTimesKbn.SelectedValue = this.ProductBundleInfo.UsableTimesKbn;
				spanUsableTimes.Visible = this.IsNumSpecify;
				tbUsableTimes.Text = this.ProductBundleInfo.UsableTimes.HasValue
					? this.ProductBundleInfo.UsableTimes.ToString()
					: string.Empty;
				rblApplyType.SelectedValue = this.ProductBundleInfo.ApplyType;
				cbValidFlg.Checked = (this.ProductBundleInfo.ValidFlg == Constants.FLG_PRODUCTBUNDLE_VALID_FLG_VALID);
				if (this.ProductBundleInfo.Items.Any()) 
				{
					CreateGrantProductIdInput(this.ProductBundleInfo.Items);
				}
				else
				{
					CreateAnEmptyGrantProductIdInput();
				}
				tbExceptProductId.Text = string.Join(Environment.NewLine, this.ProductBundleInfo.GetExceptProductIds());
				tbExceptProductVariationId.Text = string.Join(Environment.NewLine, this.ProductBundleInfo.GetExceptProductVariationIds());
				tbExceptProductCategoryId.Text = this.ProductBundleInfo.ExceptProductCategoryIds;
				tbTargetList.Text = this.ProductBundleInfo.TargetId;
				cbTargetIdExceptFlg.Checked =
					(this.ProductBundleInfo.TargetIdExceptFlg == Constants.FLG_PRODUCTBUNDLE_TARGET_ID_EXCEPT_FLG_EXCEPT);
				tbTargetOrderPriceSubtotalMin.Text = this.ProductBundleInfo.TargetOrderPriceSubtotalMin.HasValue
					? this.ProductBundleInfo.TargetOrderPriceSubtotalMin.ToPriceString()
					: string.Empty;
				tbTargetProductCountMin.Text = this.ProductBundleInfo.TargetProductCountMin.HasValue
					? this.ProductBundleInfo.TargetProductCountMin.ToString()
					: string.Empty;
				tbTargetAdvCodesFirst.Text = this.ProductBundleInfo.TargetAdvCodesFirst;
				tbTargetAdvCodesNew.Text = this.ProductBundleInfo.TargetAdvCodesNew;
				tbTargetCouponCodes.Text = this.ProductBundleInfo.TargetCouponCodes;

				SetSearchCheckBoxValue(cblTargetPaymentIds, this.ProductBundleInfo.TargetPaymentIds.Split(','));
				break;

			case Constants.ACTION_STATUS_INSERT:
				rblTargetProductKbn.SelectedValue = Constants.FLG_PRODUCTBUNDLE_TARGET_PRODUCT_KBN_SELECT;
				rblTargetOrderType.SelectedValue = Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_NORMAL;
				rblUsableTimesKbn.SelectedValue = Constants.FLG_PRODUCTBUNDLE_USABLE_TIMES_KBN_ONCETIME;
				rblApplyType.SelectedValue = Constants.FLG_PRODUCTBUNDLE_APPLY_TYPE_FOR_ORDER;
				tbApplyOrder.Text = Constants.FLG_PRODUCTBUNDLE_APPLY_ORDER_DEFAULT.ToString();
				ucDisplayPeriod.SetStartDate(DateTime.Today);
				CreateAnEmptyGrantProductIdInput();
				break;

			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}
	}
	#endregion

	#region -GetProductBundleInfo 商品同梱情報取得
	/// <summary>
	/// 商品同梱情報取得
	/// </summary>
	private void GetProductBundleInfo()
	{
		var productBundleId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID]);
		var productBundleInfo = new ProductBundleService().Get(productBundleId);
		if (productBundleInfo == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		this.ProductBundleInfo = productBundleInfo;
	}
	#endregion

	#region -SetTextAreaSize 各入力欄サイズ制御
	/// <summary>
	/// 各入力欄サイズ制御
	/// </summary>
	private void SetTextAreaSize()
	{
		// 対象商品 商品ID
		tbProductId.Rows = this.ProductIdRowsCount;
		lbProductResizeLarge.Visible = IsSizeNormal(this.ProductIdRowsCount);
		lbProductResizeNormal.Visible = (IsSizeNormal(this.ProductIdRowsCount) == false);

		// 対象商品 バリエーションID
		tbProductVariationId.Rows = this.ProductVariationIdRowsCount;
		lbProductVariationResizeLarge.Visible = IsSizeNormal(this.ProductVariationIdRowsCount);
		lbProductVariationResizeNormal.Visible = (IsSizeNormal(this.ProductVariationIdRowsCount) == false);

		// 対象商品 商品カテゴリID
		tbProductCategoryId.Rows = this.ProductCategoryIdRowsCount;
		lbProductCategoryResizeLarge.Visible = IsSizeNormal(this.ProductCategoryIdRowsCount);
		lbProductCategoryResizeNormal.Visible = (IsSizeNormal(this.ProductCategoryIdRowsCount) == false);

		// 対象外商品 商品ID
		tbExceptProductId.Rows = this.ExcludedProductIdRowsCount;
		lbExceptProductResizeLarge.Visible = IsSizeNormal(this.ExcludedProductIdRowsCount);
		lbExceptProductResizeNormal.Visible = (IsSizeNormal(this.ExcludedProductIdRowsCount) == false);

		// 対象外商品 バリエーションID
		tbExceptProductVariationId.Rows = this.ExcludedProductVariationIdRowsCount;
		lbExceptProductVariationResizeLarge.Visible = IsSizeNormal(this.ExcludedProductVariationIdRowsCount);
		lbExceptProductVariationResizeNormal.Visible = (IsSizeNormal(this.ExcludedProductVariationIdRowsCount) == false);

		// 対象外商品 商品カテゴリID
		tbExceptProductCategoryId.Rows = this.ExcludedProductCategoryIdRowsCount;
		lbExceptProductCategoryResizeLarge.Visible = IsSizeNormal(this.ExcludedProductCategoryIdRowsCount);
		lbExceptProductCategoryResizeNormal.Visible = (IsSizeNormal(this.ExcludedProductCategoryIdRowsCount) == false);

		// 初回広告コード
		tbTargetAdvCodesFirst.Rows = this.AdvCodeFirstRowsCount;
		lbTargetAdvCodesFirstResizeLarge.Visible = IsSizeNormal(this.AdvCodeFirstRowsCount);
		lbTargetAdvCodesFirstResizeNormal.Visible = (IsSizeNormal(this.AdvCodeFirstRowsCount) == false);

		// 最新広告コード
		tbTargetAdvCodesNew.Rows = this.AdvCodeNewRowsCount;
		lbTargetAdvCodesNewResizeLarge.Visible = IsSizeNormal(this.AdvCodeNewRowsCount);
		lbTargetAdvCodesNewResizeNormal.Visible = (IsSizeNormal(this.AdvCodeNewRowsCount) == false);

		// クーポンコード
		tbTargetCouponCodes.Rows = this.TargetCouponCodesRowsCount;
		lbTargetCouponCodesResizeLarge.Visible = IsSizeNormal(this.TargetCouponCodesRowsCount);
		lbTargetCouponCodesResizeNormal.Visible = (IsSizeNormal(this.TargetCouponCodesRowsCount) == false);
	}
	#endregion

	#region -IsSizeNormal 入力欄は通常サイズか
	/// <summary>
	/// 入力欄は通常サイズか
	/// </summary>
	/// <param name="rowsCount">入力欄サイズ</param>
	/// <returns>TRUE:通常サイズ、FALSE：拡大サイズ</returns>
	private bool IsSizeNormal(int rowsCount)
	{
		return (rowsCount == TEXTAREA_ROWS_NORMAL);
	}
	#endregion

	#region -IsProductVariationIdSizeNormal バリエーションID入力欄は通常サイズか
	/// <summary>
	/// バリエーションID入力欄は通常サイズか
	/// </summary>
	private bool IsProductVariationIdSizeNormal()
	{
		var isProductVariationIdSizeNormal = (this.ProductVariationIdRowsCount == TEXTAREA_ROWS_NORMAL);
		return isProductVariationIdSizeNormal;
	}
	#endregion

	#region -CreateAnEmptyGrantProductIdInput 空の同梱商品ID入力欄を1つ生成
	/// <summary>
	/// 空の同梱商品ID入力欄を一つ生成
	/// </summary>
	private void CreateAnEmptyGrantProductIdInput()
	{
		var bundleItem = new ProductBundleItemModel
		{
			GrantProductCount = DEFAULT_GRANT_PRODUCT_COUNT
		};
		CreateGrantProductIdInput(new [] { bundleItem });
	}
	#endregion

	#region -CreateGrantProductIdInput 同梱商品ID入力欄を生成
	/// <summary>
	/// 同梱商品ID入力欄を生成
	/// </summary>
	/// <param name="bundleItems">同梱商品モデル</param>
	private void CreateGrantProductIdInput(ProductBundleItemModel[] bundleItems)
	{
		var grantItems = GetGrantProductIdInputData().ToList();
		grantItems.AddRange(bundleItems.Select(bundleItem => new ProductBundleItemInput(bundleItem)));
		
		rGrantProductIdList.DataSource = grantItems;
		rGrantProductIdList.DataBind();
	}
	#endregion

	#region -GetGrantProductIdInputData 同梱商品ID入力欄の入力内容取得
	/// <summary>
	/// 同梱商品ID入力欄の入力内容取得
	/// </summary>
	private IEnumerable<ProductBundleItemInput> GetGrantProductIdInputData()
	{
		var grantItemInput = rGrantProductIdList.Items
			.OfType<RepeaterItem>()
			.Select(item => new ProductBundleItemInput
				{
					GrantProductId = ((TextBox)item.FindControl("tbGrantProductId")).Text.Trim(),
					GrantProductVariationId = ((TextBox)item.FindControl("tbGrantProductVariationId")).Text.Trim(),
					GrantProductCount = ((TextBox)item.FindControl("tbGrantProductCount")).Text.Trim(),
					OrderedProductExceptFlg = ((CheckBox)item.FindControl("cbOrderedProductExceptFlg")).Checked
				});
		return grantItemInput;
	}
	#endregion

	#region -DeleteAnGrantProductIdInput 同梱商品ID入力欄の削除
	/// <summary>
	/// 同梱商品ID入力欄の削除
	/// </summary>
	/// <param name="itemIndex">削除する入力欄のインデックス</param>
	private void DeleteAnGrantProductIdInput(int itemIndex)
	{
		var grantItems = GetGrantProductIdInputData().ToList();
		grantItems.RemoveAt(itemIndex);
		
		if (grantItems.Any() == false)
		{
			grantItems.Add(new ProductBundleItemInput(
				new ProductBundleItemModel
				{
					GrantProductCount = DEFAULT_GRANT_PRODUCT_COUNT
				}));
		}

		rGrantProductIdList.DataSource = grantItems;
		rGrantProductIdList.DataBind();
	}
	#endregion

	#region -CreateInputData 入力クラス生成
	/// <summary>
	/// 入力クラス生成
	/// </summary>
	/// <returns></returns>
	private ProductBundleInput CreateInputData()
	{
		var input = new ProductBundleInput();
		input.ProductBundleId = StringUtility.ToHankaku(tbBundleId.Text.Trim());
		input.ProductBundleName = tbBundleName.Text.Trim();
		input.TargetOrderType = Constants.FIXEDPURCHASE_OPTION_ENABLED
			? rblTargetOrderType.SelectedValue
			: Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_NORMAL;
		input.MultipleApplyFlg = cbMultipleApplyFlg.Checked
			? Constants.FLG_PRODUCTBUNDLE_MULTIPLE_APPLY_FLG_VALID 
			: Constants.FLG_PRODUCTBUNDLE_MULTIPLE_APPLY_FLG_INVALID;
		input.ApplyOrder = tbApplyOrder.Text.Trim();
		input.Description = tbDescription.Text.Trim();
		input.StartDatetime = ucDisplayPeriod.StartDateTimeString;
		input.EndDatetime = ucDisplayPeriod.EndDateTimeString;
		input.TargetProductKbn = rblTargetProductKbn.SelectedValue;
		input.TargetProductIds = tbProductId.Text.Trim();
		input.TargetProductVariationIds = tbProductVariationId.Text.Trim();
		input.TargetProductCategoryIds = tbProductCategoryId.Text.Trim();
		input.TargetOrderFixedPurchaseCountFrom = tbTargetOrderFixedPurchaseCountFrom.Text.Trim();
		input.TargetOrderFixedPurchaseCountTo = tbTargetOrderFixedPurchaseCountTo.Text.Trim();
		input.UsableTimesKbn = rblUsableTimesKbn.SelectedValue;
		input.UsableTimes = (rblUsableTimesKbn.SelectedValue == Constants.FLG_PRODUCTBUNDLE_USABLE_TIMES_KBN_NUMSPECIFY)
			? StringUtility.ToHankaku(tbUsableTimes.Text.Trim())
			: null;
		input.ApplyType = rblApplyType.SelectedValue;
		input.ValidFlg = cbValidFlg.Checked
			? Constants.FLG_PRODUCTBUNDLE_VALID_FLG_VALID 
			: Constants.FLG_PRODUCTBUNDLE_VALID_FLG_INVALID;
		input.BundleItems = GetGrantProductIdInputData().ToArray();
		input.LastChanged = this.LoginOperatorName;
		input.ExceptProductIds = tbExceptProductId.Text.Trim();
		input.ExceptProductVariationIds = tbExceptProductVariationId.Text.Trim();
		input.ExceptProductCategoryIds = tbExceptProductCategoryId.Text.Trim();
		input.TargetId = tbTargetList.Text.Trim();
		input.TargetIdExceptFlg = cbTargetIdExceptFlg.Checked;
		input.TargetOrderPriceSubtotalMin = StringUtility.ToHankaku(tbTargetOrderPriceSubtotalMin.Text.Trim());
		input.TargetProductCountMin = StringUtility.ToHankaku(tbTargetProductCountMin.Text.Trim());
		input.TargetAdvCodesFirst = tbTargetAdvCodesFirst.Text.Trim();
		input.TargetAdvCodesNew = tbTargetAdvCodesNew.Text.Trim();
		input.DeptId = this.LoginOperatorShopId;
		var selectedPaymentIds = cblTargetPaymentIds.Items.Cast<ListItem>()
			.Where(item => item.Selected)
			.Select(item => item.Value)
			.ToArray();
		input.TargetPaymentIds = string.Join(",", selectedPaymentIds);
		input.TargetCouponCodes = tbTargetCouponCodes.Text.Trim();

		return input;
	}
	#endregion

	#region -ProductBundleRegister 同梱設定登録
	/// <summary>
	/// 同梱設定登録
	/// </summary>
	/// <param name="mode">登録処理区分</param>
	private void ProductBundleRegister(RegisterMode mode)
	{
		var input = CreateInputData();
		var errorMessage = input.Validate((mode == RegisterMode.Insert), this.LoginOperatorDeptId);
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		var model = input.CreateModel();
		switch (mode)
		{
			case (RegisterMode.Insert):
				new ProductBundleService().Insert(model);
				break;

			case (RegisterMode.Update):
				new ProductBundleService().Update(model);
				break;
		}

		Response.Redirect(CreateProductBundleRegister(Constants.ACTION_STATUS_COMPLETE, model.ProductBundleId));
	}
	#endregion

	#region -ProductBundleDelete 同梱設定削除
	/// <summary>
	/// 同梱設定削除
	/// </summary>
	private void ProductBundleDelete()
	{
		new ProductBundleService().Delete(this.ProductBundleInfo.ProductBundleId);

		var listParameters = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTBUNDLE_SEARCH_INFO] ?? CreateDefaultListParameters();
		Response.Redirect(CreateProductBundleListUrl(listParameters, true));
	}
	#endregion

	#region -CreateDefaultListParametesr 同梱設定一覧画面の検索パラメタを初期値で作成
	/// <summary>
	/// 同梱設定一覧画面の検索パラメタを初期値で作成
	/// </summary>
	/// <returns>検索パラメタ</returns>
	private Hashtable CreateDefaultListParameters()
	{
		var parameters = new Hashtable
		{
			{ Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID, string.Empty },
			{ Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_NAME, string.Empty },
			{ Constants.REQUEST_KEY_PRODUCTBUNDLE_TARGET_ORDER_TYPE, string.Empty },
			{ Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_PRODUCTBUNDLE_LIST_DEFAULT },
			{ Constants.REQUEST_KEY_PAGE_NO, DEFAULT_PAGE_NO }
		};
		return parameters;
	}
	#endregion

	/// <summary>
	/// ユーザ利用可能回数表示切替
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblUsableTimesKbn_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		spanUsableTimes.Visible = this.IsNumSpecify;
		if (this.IsNumSpecify == false) tbUsableTimes.Text = string.Empty;
	}
	
	/// <summary>
	/// クーポンリストポップアップページのリンク取得
	/// </summary>
	/// <returns>クーポンリストポップアップページリンク</returns>
	protected string GetCouponListPopupPage()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_COUPON_LIST_POPUP)
			.AddParam(Constants.REQUEST_KEY_COUPONLISTPOPUP_VALID_FLG, Constants.FLG_COUPON_VALID_FLG_VALID)
			.CreateUrl();
		return url;
	}

	#region プロパティ
	/// <summary>商品同梱情報</summary>
	protected ProductBundleModel ProductBundleInfo
	{
		get { return (ProductBundleModel)ViewState["ProductBundleInfo"]; }
		set { ViewState["ProductBundleInfo"] = value; }
	}
	/// <summary>商品ID入力欄のサイズ</summary>
	protected int ProductIdRowsCount
	{
		get { return (int?)ViewState["ProductIdRowsCount"] ?? TEXTAREA_ROWS_NORMAL; }
		set { ViewState["ProductIdRowsCount"] = value; }
	}
	/// <summary>バリエーションID入力欄のサイズ</summary>
	protected int ProductVariationIdRowsCount
	{
		get { return (int?)ViewState["ProductVariationIdRowsCount"] ?? TEXTAREA_ROWS_NORMAL; }
		set { ViewState["ProductVariationIdRowsCount"] = value; }
	}
	/// <summary>対象商品カテゴリID入力欄のサイズ</summary>
	protected int ProductCategoryIdRowsCount
	{
		get { return (int?)ViewState["ProductCategoryIdRowsCount"] ?? TEXTAREA_ROWS_NORMAL; }
		set { ViewState["ProductCategoryIdRowsCount"] = value; }
	}
	/// <summary>対象外商品ID入力欄のサイズ</summary>
	protected int ExcludedProductIdRowsCount
	{
		get { return (int?)ViewState["ExcludedProductIdRowsCount"] ?? TEXTAREA_ROWS_NORMAL; }
		set { ViewState["ExcludedProductIdRowsCount"] = value; }
	}
	/// <summary>対象外バリエーションID入力欄のサイズ</summary>
	protected int ExcludedProductVariationIdRowsCount
	{
		get { return (int?)ViewState["ExcludedProductVariationIdRowsCount"] ?? TEXTAREA_ROWS_NORMAL; }
		set { ViewState["ExcludedProductVariationIdRowsCount"] = value; }
	}
	/// <summary>対象外商品カテゴリID入力欄のサイズ</summary>
	protected int ExcludedProductCategoryIdRowsCount
	{
		get { return (int?)ViewState["ExcludedProductCategoryIdRowsCount"] ?? TEXTAREA_ROWS_NORMAL; }
		set { ViewState["ExcludedProductCategoryIdRowsCount"] = value; }
	}
	/// <summary>初回広告コード入力欄のサイズ</summary>
	protected int AdvCodeFirstRowsCount
	{
		get { return (int?)ViewState["AdvCodeFirstRowsCount"] ?? TEXTAREA_ROWS_NORMAL; }
		set { ViewState["AdvCodeFirstRowsCount"] = value; }
	}
	/// <summary>最新コード入力欄のサイズ</summary>
	protected int AdvCodeNewRowsCount
	{
		get { return (int?)ViewState["AdvCodeNewRowsCount"] ?? TEXTAREA_ROWS_NORMAL; }
		set { ViewState["AdvCodeNewRowsCount"] = value; }
	}
	/// <summary>最新コード入力欄のサイズ</summary>
	private int TargetCouponCodesRowsCount
	{
		get { return (int?)ViewState["TargetCouponCodesRowsCount"] ?? TEXTAREA_ROWS_NORMAL; }
		set { ViewState["TargetCouponCodesRowsCount"] = value; }
	}
	/// <summary>回数指定か</summary>
	protected bool IsNumSpecify
	{
		get { return (rblUsableTimesKbn.SelectedValue == Constants.FLG_PRODUCTBUNDLE_USABLE_TIMES_KBN_NUMSPECIFY); }
	}
	/// <summary>クーポン検索が使用可能か</summary>
	protected bool UsableCouponSearch
	{
		get
		{
			var hasCouponMenuAccess = new OperatorMenuManager().CheckPageMenuAuthorityWithManagetType(
				this.LoginShopOperator.GetMenuAccessLevel(MenuAuthorityHelper.ManagerSiteType.Mp),
				this.LoginOperatorShopId,
				FOLDER_PATH_COUPON_MENU,
				MenuAuthorityHelper.ManagerSiteType.Mp);

			return Constants.W2MP_COUPON_OPTION_ENABLED && hasCouponMenuAccess;
		}
	}
	#endregion
}
