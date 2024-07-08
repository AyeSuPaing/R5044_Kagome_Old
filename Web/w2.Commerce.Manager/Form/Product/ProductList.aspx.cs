/*
=========================================================================================================
  Module      : 商品情報一覧ページ処理(ProductList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Flaps;
using w2.App.Common.Option;
using w2.App.Common.Order.Workflow;
using w2.App.Common.Product;
using w2.Common.Logger;
using w2.Domain.ProductTaxCategory;

public partial class Form_Product_ProductList : ProductPage
{
	//=========================================================================================
	// 商品一覧定数
	//=========================================================================================
	const string HASH_KEY_PRODUCT_DISPLAY_KBN = "disp_kbn";		// 表示区分
	const string HASH_KEY_PRODUCT_SELL_KBN = "sell_kbn";		// 販売区分
	const string HASH_KEY_PRODUCT_CATEGORY_ID = "category_id";	// カテゴリID

	const string KBN_PRODUCT_SELL_KBN_SELL = "0";				// 販売区分：未販売
	const string KBN_PRODUCT_SELL_KBN_NOT_SELL = "1";			// 販売区分：販売中

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, EventArgs e)
	{
		// ユーザーコントロール割り当て
		uMasterDownload.OnCreateSearchInputParams += this.CreateSearchParams;

		// FLAPS連携 実行中でなければボタン有効化
		btnGetLatestInfoFromErpBottom.Enabled = btnGetLatestInfoFromErpTop.Enabled =
			(new FlapsIntegrationLocker(Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCT).IsLocked() == false);

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponents();

			//------------------------------------------------------
			// 登録系のセッションをクリア
			//------------------------------------------------------
			Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO] = null;
			Session[Constants.SESSIONPARAM_KEY_PRODUCTVARIATION_INFO] = null;
			Session[Constants.SESSIONPARAM_KEY_PRODUCTEXTEND_INFO] = null;
			Session[Constants.SESSIONPARAM_KEY_PRODUCTMALLEXHIBITS_INFO] = null;
			Session[Constants.SESSIONPARAM_KEY_UPLODED_MASTER] = null;
			Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = null;

			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			Hashtable htParam = GetParameters();

			SetMenuAuthority();

			if (this.IsNotSearchDefault) return;

			// 検索条件をセッションに保存
			Session[Constants.SESSIONPARAM_KEY_PRODUCT_SEARCH_INFO] = htParam;

			//------------------------------------------------------
			// SQL検索パラメータ取得
			//------------------------------------------------------
			Hashtable htSqlParam = GetSearchSqlInfo(htParam);

			//------------------------------------------------------
			// 商品該当件数取得
			//------------------------------------------------------
			int totalProductCount = 0;	// ページング可能総商品数
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("Product", "GetProductCount"))
			{
				DataView productCount = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSqlParam);
				if (productCount.Count != 0)
				{
					totalProductCount = (int)productCount[0]["row_count"];
				}
			}

			//------------------------------------------------------
			// エラー表示制御
			//------------------------------------------------------
			bool displayPager = true;
			StringBuilder errorMessage = new StringBuilder();
			// 上限件数より多い？
			if (totalProductCount > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST));
				errorMessage.Replace("@@ 1 @@", StringUtility.ToNumeric(totalProductCount));
				errorMessage.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));

				displayPager = false;
			}
			// 該当件数なし？
			else if (totalProductCount == 0)
			{
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
			}
			tdErrorMessage.InnerHtml = errorMessage.ToString();
			trListError.Visible = (errorMessage.ToString().Length != 0);

			//------------------------------------------------------
			// 商品一覧情報表示
			//------------------------------------------------------
			DataView productList = null;
			if (trListError.Visible == false)
			{
				// 商品一覧情報取得
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("Product", "GetProductList"))
				{
					htSqlParam.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1);
					htSqlParam.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo);
					productList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htSqlParam);
				}

				// Redirect to last page when current page no don't have any data
				CheckRedirectToLastPage(
					productList.Count,
					totalProductCount,
					CreateProductListUrlWithoutPageNo(htParam));

				for (int count = 0; count < productList.Count; count++)
				{
					productList[count]["display"] = ((string)productList[count]["display"] != "-")
						? ReplaceTag("@@DispText.common_message.now_displaying@@")
						: "-";

					productList[count]["sell"] = ((string)productList[count]["sell"] != "-")
						? ReplaceTag("@@DispText.common_message.now_selling@@")
						: "-";
				}

				// データバインド
				rList.DataSource = productList;
				rList.DataBind();
			}

			this.ProductIdListOfDisplayedData = (productList != null)
				? productList.Cast<DataRowView>().Select(drv => (string)drv[Constants.FIELD_PRODUCT_PRODUCT_ID]).ToArray()
				: new string[0];

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			if (displayPager)
			{
				lbPager1.Text = WebPager.CreateDefaultListPager(
					totalProductCount, this.CurrentPageNo, CreateProductListUrlWithoutPageNo(htParam));
			}

		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 配送サイズ区分
		ddlShippingSizeKbn.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN))
		{
			ddlShippingSizeKbn.Items.Add(li);
		}

		// 表示期間
		ddlDisplay.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_PRODUCT, HASH_KEY_PRODUCT_DISPLAY_KBN))
		{
			ddlDisplay.Items.Add(li);
		}

		// 販売期間
		ddlSell.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_PRODUCT, HASH_KEY_PRODUCT_SELL_KBN))
		{
			ddlSell.Items.Add(li);
		}
		// 商品表示期間
		ddlSearchDisplayKbn.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_DISPLAY_KBN))
		{
			ddlSearchDisplayKbn.Items.Add(li);
		}

		// 有効フラグ
		dllValidFlg.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_VALID_FLG))
		{
			dllValidFlg.Items.Add(li);
		}

		// 配送種別ドロップダウン作成
		ddlShippingType.Items.Add(new ListItem("", ""));
		DataView dvShopShipping = GetShopShippingsAll(this.LoginOperatorShopId);
		foreach (DataRowView drv in dvShopShipping)
		{
			ddlShippingType.Items.Add(new ListItem((string)drv[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME], (string)drv[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID]));
		}

		// 会員ランクオプション有り
		if (Constants.MEMBER_RANK_OPTION_ENABLED)
		{
			// 会員ランク割引対象フラグ
			cbMemberRankDiscountFlg.Items.Add(new ListItem("", ""));
			cbMemberRankDiscountFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG));

			// 閲覧可能会員ランク
			ddlDisplayMemberRank.Items.Add(new ListItem("", ""));
			ddlDisplayMemberRank.Items.AddRange(MemberRankOptionUtility.GetMemberRankList()
				.Select(memberRank => new ListItem(memberRank.MemberRankName, memberRank.MemberRankId))
				.ToArray());

			// 購入可能会員ランク
			ddlBuyableMemberRank.Items.Add(new ListItem("", ""));
			ddlBuyableMemberRank.Items.AddRange(MemberRankOptionUtility.GetMemberRankList()
				.Select(memberRank => new ListItem(memberRank.MemberRankName, memberRank.MemberRankId))
				.ToArray());
		}

		// ブランド
		ddlBrandId.Items.Add(new ListItem("", ""));
		foreach (DataRowView drv in ProductBrandUtility.GetProductBrandList())
		{
			ddlBrandId.Items.Add(new ListItem((string)drv[Constants.FIELD_PRODUCTBRAND_BRAND_ID], (string)drv[Constants.FIELD_PRODUCTBRAND_BRAND_ID]));
		}

		// Limited Payment List
		ddlLimitedPayment.Items.Add(new ListItem(string.Empty, string.Empty));
		var payments = GetPaymentValidList(this.LoginOperatorShopId);
		ddlLimitedPayment.Items.AddRange(
			payments
				.Cast<DataRowView>()
				.Select(
					payment => new ListItem(
						AbbreviateString((string)payment[Constants.FIELD_PAYMENT_PAYMENT_NAME], 12),
						(string)payment[Constants.FIELD_PAYMENT_PAYMENT_ID]))
				.ToArray());

		// 同梱商品明細表示フラグ
		ddlBundleItemDisplayType.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlBundleItemDisplayType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE));

		// 商品区分
		ddlProductType.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlProductType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_PRODUCT_TYPE));

		// 商品カラー
		ddlColors.Items.AddRange(GetColorListForDropDownList());

		// 商品税率カテゴリ
		ddlProductTaxCategory.Items.Add(new ListItem("", ""));
		ddlProductTaxCategory.Items.AddRange(new ProductTaxCategoryService().GetAllTaxCategory().Select(taxCategory => new ListItem(taxCategory.TaxCategoryName + ":" + taxCategory.TaxRate + "%", taxCategory.TaxCategoryId)).ToArray());

		// 頒布会フラグ
		ddlSubscriptionBoxFlg.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlSubscriptionBoxFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG));

		// Get value text for flag fixed purchase limit
		ddDisplayFixedPurchaseMemberLimitFlg.Items.Add(new ListItem(string.Empty, string.Empty));
		ddDisplayFixedPurchaseMemberLimitFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG));
		ddBuyableFixedPurchaseMemberLimitFlg.Items.Add(new ListItem(string.Empty, string.Empty));
		ddBuyableFixedPurchaseMemberLimitFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG));
	}

	/// <summary>
	/// 商品一覧パラメタ取得
	/// </summary>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters()
	{
		Hashtable htResult = new Hashtable();
		try
		{
			// 商品ID
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID]));
			tbProductId.Text = (string)htResult[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID];

			// 商品名
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_NAME, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_NAME]));
			tbName.Text = (string)htResult[Constants.REQUEST_KEY_PRODUCT_NAME];

			// サプライヤID
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_SUPPLIER_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SUPPLIER_ID]));
			tbSupplierId.Text = (string)htResult[Constants.REQUEST_KEY_PRODUCT_SUPPLIER_ID];

			// 商品連携ID
			for (int index = 1; index <= Constants.COOPERATION_ID_COLUMNS_COUNT; index++)
			{
				var key = Constants.REQUEST_KEY_PRODUCT_COOPERATION_ID_HEAD + index;
				htResult.Add(key, StringUtility.ToEmpty(Request[key]));
			}
			tbCooperationId1.Text = (string)htResult[Constants.REQUEST_KEY_PRODUCT_COOPERATION_ID1];
			tbCooperationId2.Text = (string)htResult[Constants.REQUEST_KEY_PRODUCT_COOPERATION_ID2];
			tbCooperationId3.Text = (string)htResult[Constants.REQUEST_KEY_PRODUCT_COOPERATION_ID3];
			tbCooperationId4.Text = (string)htResult[Constants.REQUEST_KEY_PRODUCT_COOPERATION_ID4];
			tbCooperationId5.Text = (string)htResult[Constants.REQUEST_KEY_PRODUCT_COOPERATION_ID5];

			// 配送種別
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE]));
			ddlShippingType.SelectedValue = (string)Request[Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE];

			// 配送サイズ区分
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_SHIPPING_SIZE_KBN, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SHIPPING_SIZE_KBN]));
			ddlShippingSizeKbn.SelectedValue = (string)Request[Constants.REQUEST_KEY_PRODUCT_SHIPPING_SIZE_KBN];

			// 表示期間
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_DISPLAY_KBN, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_DISPLAY_KBN]));
			ddlDisplay.SelectedValue = (string)Request[Constants.REQUEST_KEY_PRODUCT_DISPLAY_KBN];

			// 販売期間
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_SELL_KBN, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_KBN]));
			ddlSell.SelectedValue = (string)Request[Constants.REQUEST_KEY_PRODUCT_SELL_KBN];

			// 商品表示期間
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_SEARCH_DISPLAY_KBN, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SEARCH_DISPLAY_KBN]));
			ddlSearchDisplayKbn.SelectedValue = (string)Request[Constants.REQUEST_KEY_PRODUCT_SEARCH_DISPLAY_KBN];

			// 有効フラグ
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_VALID_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_VALID_FLG]));
			dllValidFlg.SelectedValue = (string)Request[Constants.REQUEST_KEY_PRODUCT_VALID_FLG];

			// カテゴリID
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_CATEGORY_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_CATEGORY_ID]));
			tbCategoryId.Text = (string)Request[Constants.REQUEST_KEY_PRODUCT_CATEGORY_ID];

			// ブランドID
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_BRAND_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_BRAND_ID]));
			ddlBrandId.SelectedValue = (string)Request[Constants.REQUEST_KEY_PRODUCT_BRAND_ID];

			// ブランドID
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_LIMITED_PAYMENT, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_LIMITED_PAYMENT]));
			ddlLimitedPayment.SelectedValue = Request[Constants.REQUEST_KEY_PRODUCT_LIMITED_PAYMENT];

			// アイコン1
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG1, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG1]));
			cbIconFlg1.Checked = (string)Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG1] == Constants.FLG_PRODUCT_ICON_ON;
			// アイコン2
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG2, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG2]));
			cbIconFlg2.Checked = (string)Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG2] == Constants.FLG_PRODUCT_ICON_ON;
			// アイコン3
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG3, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG3]));
			cbIconFlg3.Checked = (string)Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG3] == Constants.FLG_PRODUCT_ICON_ON;
			// アイコン4
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG4, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG4]));
			cbIconFlg4.Checked = (string)Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG4] == Constants.FLG_PRODUCT_ICON_ON;
			// アイコン5
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG5, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG5]));
			cbIconFlg5.Checked = (string)Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG5] == Constants.FLG_PRODUCT_ICON_ON;
			// アイコン6
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG6, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG6]));
			cbIconFlg6.Checked = (string)Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG6] == Constants.FLG_PRODUCT_ICON_ON;
			// アイコン7
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG7, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG7]));
			cbIconFlg7.Checked = (string)Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG7] == Constants.FLG_PRODUCT_ICON_ON;
			// アイコン8
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG8, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG8]));
			cbIconFlg8.Checked = (string)Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG8] == Constants.FLG_PRODUCT_ICON_ON;
			// アイコン9
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG9, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG9]));
			cbIconFlg9.Checked = (string)Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG9] == Constants.FLG_PRODUCT_ICON_ON;
			// アイコン10
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG10, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG10]));
			cbIconFlg10.Checked = (string)Request[Constants.REQUEST_KEY_PRODUCT_ICON_FLG10] == Constants.FLG_PRODUCT_ICON_ON;

			// Set request key order update date from date to
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_SELL_TO_DATE_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_TO_DATE_FROM]));
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_SELL_TO_DATE_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_TO_DATE_TO]));

			// Set request key order update time from time to
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_SELL_TO_TIME_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_TO_TIME_FROM]));
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_SELL_TO_TIME_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_TO_TIME_TO]));

			var productSellToDateFrom = string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_TO_DATE_FROM])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_TO_TIME_FROM])
					.Replace(":", string.Empty));

			var productSellToDateTo = string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_TO_DATE_TO])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_TO_TIME_TO])
					.Replace(":", string.Empty));

			ucProductSellToDate.SetPeriodDate(
				productSellToDateFrom,
				productSellToDateTo);

			// Set request key order update date from date to
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_SELL_FROM_DATE_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_DATE_FROM]));
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_SELL_FROM_DATE_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_DATE_TO]));

			// Set request key order update time from time to
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_SELL_FROM_TIME_FROM,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_TIME_FROM]));
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_SELL_FROM_TIME_TO,
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_TIME_TO]));

			var productSellFromDateFrom = string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_DATE_FROM])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_TIME_FROM])
					.Replace(":", string.Empty));

			var productSellFromDateTo= string.Format("{0}{1}",
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_DATE_TO])
					.Replace("/", string.Empty),
				StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_TIME_TO])
					.Replace(":", string.Empty));

			ucProductSellFromDate.SetPeriodDate(
				productSellFromDateFrom,
				productSellFromDateTo);


			// 会員ランクOPがONなら
			if (Constants.MEMBER_RANK_OPTION_ENABLED)
			{
				// 会員ランク割引対象フラグ
				htResult.Add(Constants.REQUEST_KEY_PRODUCT_MEMBER_RANK_DISCOUNT_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_MEMBER_RANK_DISCOUNT_FLG]));
				cbMemberRankDiscountFlg.SelectedValue = (string)Request[Constants.REQUEST_KEY_PRODUCT_MEMBER_RANK_DISCOUNT_FLG];

				// 閲覧可能会員ランク
				htResult.Add(Constants.REQUEST_KEY_PRODUCT_DISP_MEMBER_RANK, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_DISP_MEMBER_RANK]));
				ddlDisplayMemberRank.SelectedValue = (string)Request[Constants.REQUEST_KEY_PRODUCT_DISP_MEMBER_RANK];

				// 購入可能会員ランク
				htResult.Add(Constants.REQUEST_KEY_PRODUCT_BUYABLE_MEMBER_RANK, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_BUYABLE_MEMBER_RANK]));
				ddlBuyableMemberRank.SelectedValue = (string)Request[Constants.REQUEST_KEY_PRODUCT_BUYABLE_MEMBER_RANK];
			}

			// 表示優先順
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_DISPLAY_PRIORITY, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_DISPLAY_PRIORITY]));
			tbDisplayPriority.Text = (string)htResult[Constants.REQUEST_KEY_PRODUCT_DISPLAY_PRIORITY];

			// ソート
			switch (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SORT_KBN]))
			{
				case Constants.KBN_SORT_PRODUCT_LIST_PRODUCT_ID_ASC:			// 商品ID/昇順
				case Constants.KBN_SORT_PRODUCT_LIST_PRODUCT_ID_DESC:			// 商品ID/降順
				case Constants.KBN_SORT_PRODUCT_LIST_NAME_ASC:					// 商品名/昇順
				case Constants.KBN_SORT_PRODUCT_LIST_NAME_DESC:					// 商品名/降順
				case Constants.KBN_SORT_PRODUCT_LIST_NAME_KANA_ASC:				// 商品フリガナ/昇順
				case Constants.KBN_SORT_PRODUCT_LIST_NAME_KANA_DESC:			// 商品フリガナ/降順
				case Constants.KBN_SORT_PRODUCT_LIST_DATE_CREATED_ASC:			// 作成日/昇順
				case Constants.KBN_SORT_PRODUCT_LIST_DATE_CREATED_DESC:			// 作成日/降順
				case Constants.KBN_SORT_PRODUCT_LIST_DATE_CHANGED_ASC:			// 更新日/昇順
				case Constants.KBN_SORT_PRODUCT_LIST_DATE_CHANGED_DESC:			// 更新日/降順
					htResult.Add(Constants.REQUEST_KEY_SORT_KBN, Request[Constants.REQUEST_KEY_SORT_KBN]);
					break;
				default:
					htResult.Add(Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_PRODUCT_LIST_DEFAULT);
					break;
			}
			ddlSortKbn.SelectedValue = (string)htResult[Constants.REQUEST_KEY_SORT_KBN];

			// ページ番号（ページャ動作時のみもちまわる）
			int iPageNo;
			if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out iPageNo) == false)
			{
				iPageNo = 1;
			}
			htResult.Add(Constants.REQUEST_KEY_PAGE_NO, iPageNo.ToString());
			this.CurrentPageNo = iPageNo;

			// 同梱商品明細表示フラグ
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE, Request[Constants.REQUEST_KEY_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE]);
			ddlBundleItemDisplayType.SelectedValue = (string)Request[Constants.REQUEST_KEY_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE];

			// 商品区分
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_PRODUCT_TYPE, Request[Constants.REQUEST_KEY_PRODUCT_PRODUCT_TYPE]);
			ddlProductType.SelectedValue = (string)Request[Constants.REQUEST_KEY_PRODUCT_PRODUCT_TYPE];

			// 商品カラー
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_COLOR_ID, Request[Constants.REQUEST_KEY_PRODUCT_COLOR_ID]);

			// 商品税率カテゴリ
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_TAX_CATEGORY_ID, Request[Constants.REQUEST_KEY_PRODUCT_TAX_CATEGORY_ID]);
			ddlProductTaxCategory.SelectedValue = (string)Request[Constants.REQUEST_KEY_PRODUCT_TAX_CATEGORY_ID];

			// Flag fixed purchase limit add to hashtable
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG]));
			ddDisplayFixedPurchaseMemberLimitFlg.SelectedValue = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG]);
			htResult.Add(Constants.REQUEST_KEY_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG]));
			ddBuyableFixedPurchaseMemberLimitFlg.SelectedValue = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG]);

			// 頒布会フラグ
			if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
			{
				htResult.Add(Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG, Request[Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG]);
				ddlSubscriptionBoxFlg.SelectedValue = (string)Request[Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG];
			}
		}
		catch (Exception ex)
		{
			AppLogger.WriteError(ex);

			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		return htResult;
	}

	/// <summary>
	/// 検索値取得
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchSqlInfo(Hashtable htParam)
	{
		Hashtable htResult = new Hashtable();

		// 店舗ID
		htResult.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);
		// 商品ID
		htResult.Add(Constants.FIELD_PRODUCT_PRODUCT_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htParam[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID]));
		// 商品名
		htResult.Add(Constants.FIELD_PRODUCT_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htParam[Constants.REQUEST_KEY_PRODUCT_NAME]));
		// サプライヤID
		htResult.Add(Constants.FIELD_PRODUCT_SUPPLIER_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htParam[Constants.REQUEST_KEY_PRODUCT_SUPPLIER_ID]));
		// 商品連携ID
		for (int index = 1; index <= Constants.COOPERATION_ID_COLUMNS_COUNT; index++)
		{
			htResult.Add(Constants.HASH_KEY_COOPERATION_ID + index + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htParam[Constants.REQUEST_KEY_PRODUCT_COOPERATION_ID_HEAD + index]));
		}
		// 配送種別
		htResult.Add(Constants.FIELD_PRODUCT_SHIPPING_TYPE, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE]));
		// 配送サイズ区分
		htResult.Add(Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_SHIPPING_SIZE_KBN]));
		// 表示期間
		htResult.Add(HASH_KEY_PRODUCT_DISPLAY_KBN, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_DISPLAY_KBN]));
		// 販売期間
		htResult.Add(HASH_KEY_PRODUCT_SELL_KBN, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_SELL_KBN]));
		// 商品表示区分
		htResult.Add(Constants.FIELD_PRODUCT_DISPLAY_KBN, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_SEARCH_DISPLAY_KBN]));
		// 有効フラグ
		htResult.Add(Constants.FIELD_PRODUCT_VALID_FLG, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_VALID_FLG]));
		// カテゴリID
		htResult.Add(HASH_KEY_PRODUCT_CATEGORY_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(htParam[Constants.REQUEST_KEY_PRODUCT_CATEGORY_ID]));
		// ブランドID
		htResult.Add("brand_id", StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_BRAND_ID]));
		// アイコン1
		htResult.Add(Constants.FIELD_PRODUCT_ICON_FLG1, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG1]));
		// アイコン2
		htResult.Add(Constants.FIELD_PRODUCT_ICON_FLG2, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG2]));
		// アイコン3
		htResult.Add(Constants.FIELD_PRODUCT_ICON_FLG3, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG3]));
		// アイコン4
		htResult.Add(Constants.FIELD_PRODUCT_ICON_FLG4, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG4]));
		// アイコン5
		htResult.Add(Constants.FIELD_PRODUCT_ICON_FLG5, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG5]));
		// アイコン6
		htResult.Add(Constants.FIELD_PRODUCT_ICON_FLG6, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG6]));
		// アイコン7
		htResult.Add(Constants.FIELD_PRODUCT_ICON_FLG7, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG7]));
		// アイコン8
		htResult.Add(Constants.FIELD_PRODUCT_ICON_FLG8, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG8]));
		// アイコン9
		htResult.Add(Constants.FIELD_PRODUCT_ICON_FLG9, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG9]));
		// アイコン10
		htResult.Add(Constants.FIELD_PRODUCT_ICON_FLG10, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG10]));
		// ソート区分
		htResult.Add("sort_kbn", (string)htParam[Constants.REQUEST_KEY_SORT_KBN]);
		// 会員ランク割引対象フラグ
		htResult.Add(Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_MEMBER_RANK_DISCOUNT_FLG]));
		// 閲覧可能会員ランク
		htResult.Add(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_DISP_MEMBER_RANK]));
		// 購入可能会員ランク
		htResult.Add(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_BUYABLE_MEMBER_RANK]));
		// 表示優先順
		htResult.Add(Constants.FIELD_PRODUCT_DISPLAY_PRIORITY, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_DISPLAY_PRIORITY]));

		const string dateTemplate = "{0}/{1}/{2} {3}:{4}:{5}";
		const string dateFormat = "yyyy/MM/dd HH:mm:ss";

		// 販売時期(品販売の開始日)
		htResult.Add(Constants.FIELD_PRODUCT_SELL_FROM + "_from", System.DBNull.Value);
		var sellFromDateFrom = string.Format("{0} {1}",
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_DATE_FROM]),
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_TIME_FROM])
		);
		if (Validator.IsDate(sellFromDateFrom))
		{
			htResult[Constants.FIELD_PRODUCT_SELL_FROM + "_from"] = DateTime.ParseExact(sellFromDateFrom, dateFormat, null);
		}

		// 販売時期(品販売の開始日)
		htResult.Add(Constants.FIELD_PRODUCT_SELL_FROM + "_to", System.DBNull.Value);
		var sellFromDateTo = string.Format("{0} {1}",
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_DATE_TO]),
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_TIME_TO])
		);
		if (Validator.IsDate(sellFromDateTo))
		{
			htResult[Constants.FIELD_PRODUCT_SELL_FROM + "_to"] = DateTime.ParseExact(sellFromDateTo, dateFormat, null);
		}

		// 販売時期(製品販売の終了日)
		htResult.Add(Constants.FIELD_PRODUCT_SELL_TO + "_from", System.DBNull.Value);
		var sellToDateFrom = string.Format("{0} {1}",
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_SELL_TO_DATE_FROM]),
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_SELL_TO_TIME_FROM])
		);
		if (Validator.IsDate(sellToDateFrom))
		{
			htResult[Constants.FIELD_PRODUCT_SELL_TO + "_from"] = DateTime.ParseExact(sellToDateFrom, dateFormat, null);
		}

		// 販売時期(製品販売の終了日)
		htResult.Add(Constants.FIELD_PRODUCT_SELL_TO + "_to", System.DBNull.Value);
		var sellToDateTo = string.Format("{0} {1}",
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_SELL_TO_DATE_TO]),
			StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_SELL_TO_TIME_TO])
		);
		if (Validator.IsDate(sellToDateTo))
		{
			htResult[Constants.FIELD_PRODUCT_SELL_TO + "_to"] = DateTime.ParseExact(sellToDateTo, dateFormat, null);
		}

		// Limited Payment
		htResult.Add(Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS + "_like_escaped", StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_LIMITED_PAYMENT]));

		// 同梱商品明細表示フラグ
		htResult.Add(Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE]));

		// 商品区分
		htResult.Add(Constants.FIELD_PRODUCT_PRODUCT_TYPE, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_PRODUCT_TYPE]));

		// カラー（商品とバリエーション共通）
		htResult.Add(Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_COLOR_ID]));

		// 商品税率カテゴリ
		htResult.Add(Constants.FIELD_PRODUCT_TAX_CATEGORY_ID, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_TAX_CATEGORY_ID]));
		//Flag fixed purchase limit
		htResult.Add(Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG]));
		htResult.Add(Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG]));

		// 頒布会フラグ
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
		{
			htResult.Add(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG, StringUtility.ToEmpty(htParam[Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG]));
		}

		return htResult;
	}

	/// <summary>
	/// 各検索コントロールから検索情報取得
	/// </summary>
	/// <returns>検索情報</returns>
	private Hashtable GetSearchInfoFromControl()
	{
		Hashtable htSearch = new Hashtable();
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID, tbProductId.Text.Trim());								// 商品ID
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_NAME, tbName.Text.Trim());											// 商品名
		htSearch.Add(Constants.REQUEST_KEY_SORT_KBN, ddlSortKbn.SelectedValue);											// ソート
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_SUPPLIER_ID, tbSupplierId.Text.Trim());								// サプライヤID
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_COOPERATION_ID1, tbCooperationId1.Text.Trim());	// 商品連携ID1
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_COOPERATION_ID2, tbCooperationId2.Text.Trim());	// 商品連携ID2
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_COOPERATION_ID3, tbCooperationId3.Text.Trim());	// 商品連携ID3
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_COOPERATION_ID4, tbCooperationId4.Text.Trim());	// 商品連携ID4
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_COOPERATION_ID5, tbCooperationId5.Text.Trim());	// 商品連携ID5
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE, ddlShippingType.SelectedValue);						// 配送種別
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_SHIPPING_SIZE_KBN, ddlShippingSizeKbn.SelectedValue);				// 配送サイズ区分
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_DISPLAY_KBN, ddlDisplay.SelectedValue);								// 表示期間
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_SELL_KBN, ddlSell.SelectedValue);									// 販売期間
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_SEARCH_DISPLAY_KBN, ddlSearchDisplayKbn.SelectedValue);	// 商品表示区分
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_VALID_FLG, dllValidFlg.SelectedValue);								// 有効フラグ
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_CATEGORY_ID, tbCategoryId.Text.Trim());								// カテゴリID
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_BRAND_ID, ddlBrandId.SelectedValue);									// ブランドID
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG1, cbIconFlg1.Checked ? Constants.FLG_PRODUCT_ICON_ON : "");	// アイコン1
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG2, cbIconFlg2.Checked ? Constants.FLG_PRODUCT_ICON_ON : "");	// アイコン2
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG3, cbIconFlg3.Checked ? Constants.FLG_PRODUCT_ICON_ON : "");	// アイコン3
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG4, cbIconFlg4.Checked ? Constants.FLG_PRODUCT_ICON_ON : "");	// アイコン4
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG5, cbIconFlg5.Checked ? Constants.FLG_PRODUCT_ICON_ON : "");	// アイコン5
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG6, cbIconFlg6.Checked ? Constants.FLG_PRODUCT_ICON_ON : "");	// アイコン6
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG7, cbIconFlg7.Checked ? Constants.FLG_PRODUCT_ICON_ON : "");	// アイコン7
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG8, cbIconFlg8.Checked ? Constants.FLG_PRODUCT_ICON_ON : "");	// アイコン8
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG9, cbIconFlg9.Checked ? Constants.FLG_PRODUCT_ICON_ON : "");	// アイコン9
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_ICON_FLG10, cbIconFlg10.Checked ? Constants.FLG_PRODUCT_ICON_ON : "");	// アイコン10
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_MEMBER_RANK_DISCOUNT_FLG, cbMemberRankDiscountFlg.SelectedValue);	// 会員ランク割引対象フラグ
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_DISP_MEMBER_RANK, ddlDisplayMemberRank.SelectedValue);				// 閲覧可能会員ランク
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_BUYABLE_MEMBER_RANK, ddlBuyableMemberRank.SelectedValue);			// 購入可能会員ランク
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_DISPLAY_PRIORITY, tbDisplayPriority.Text.Trim());			// 表示優先順
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_SELL_FROM_DATE_FROM, ucProductSellFromDate.HfStartDate.Value);		// Product sell date from(From)
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_SELL_FROM_DATE_TO, ucProductSellFromDate.HfEndDate.Value);			// Product sell date to(From)
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_SELL_FROM_TIME_FROM, ucProductSellFromDate.HfStartTime.Value);		// Product sell time from(From)
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_SELL_FROM_TIME_TO, ucProductSellFromDate.HfEndTime.Value);			// Product sell time to(From)
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_SELL_TO_DATE_FROM, ucProductSellToDate.HfStartDate.Value);			// Product sell date from(To)
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_SELL_TO_DATE_TO, ucProductSellToDate.HfEndDate.Value);				// Product sell date to(To)
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_SELL_TO_TIME_FROM, ucProductSellToDate.HfStartTime.Value);			// Product sell time from(To)
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_SELL_TO_TIME_TO, ucProductSellToDate.HfEndTime.Value);				// Product sell time to(To)
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_LIMITED_PAYMENT, ddlLimitedPayment.SelectedValue);							// Limited Payment
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE, ddlBundleItemDisplayType.SelectedValue);			// 同梱商品明細表示フラグ
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_PRODUCT_TYPE, ddlProductType.SelectedValue);			// 商品区分
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_COLOR_ID, ddlColors.SelectedValue);											// 商品カラー
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_TAX_CATEGORY_ID, ddlProductTaxCategory.SelectedValue);											// 商品税率カテゴリ
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG, ddDisplayFixedPurchaseMemberLimitFlg.SelectedValue);	// Fixed purchase member (Display)
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG, ddBuyableFixedPurchaseMemberLimitFlg.SelectedValue);	// Fixed purchase member (Buyable)
		// 頒布会フラグ
		htSearch.Add(Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG, ddlSubscriptionBoxFlg.SelectedValue);

		return htSearch;
	}

	/// <summary>
	/// ドロップダウンリスト用商品カラーリスト取得
	/// </summary>
	/// <returns>ドロップダウン表示用ListItemArray</returns>
	protected ListItem[] GetColorListForDropDownList()
	{
		var productColors = new[] { new ListItem("", "") }.Concat(
			DataCacheControllerFacade.GetProductColorCacheController().GetProductColorList().Select(
				color =>
				{
					var item = new ListItem(color.DispName, color.Id);
					if (item.Value == Request[Constants.REQUEST_KEY_PRODUCT_COLOR_ID]) item.Selected = true;
					return item;
				})).ToArray();
		return productColors.ToArray();
	}

	/// <summary>
	/// 検索実行イベントハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		// 入力チェック
		Hashtable htSearch = GetSearchInfoFromControl();
		var errorMessages = Validator.Validate("ProductSearch", htSearch);
		if (errorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 検索用パラメタ作成し、同じ画面にリダイレクト
		Response.Redirect(CreateProductListUrlWithoutPageNo(htSearch));
	}

	/// <summary>
	/// 商品詳細ページURL出力リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbExprotProductDetailUrl_Click(object sender, EventArgs e)
	{
		uMasterDownload.ExportProductDetailUrl(GetSearchSqlInfo(GetSearchInfoFromControl()));
	}

	/// <summary>
	/// 新規ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		// セッション初期化
		Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO] = null;
		Session[Constants.SESSIONPARAM_KEY_PRODUCTVARIATION_INFO] = null;
		Session[Constants.SESSIONPARAM_KEY_PRODUCTEXTEND_INFO] = null;

		// 画面遷移
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = null;

		// 新規登録画面へ
		Response.Redirect(
			CreateProductRegistUrl(string.Empty, Constants.ACTION_STATUS_INSERT, true));
	}

	/// <summary>
	/// 商品初期設定ボタンクリック
	/// </summary>
	protected void btnDefaultSetting_Click(object sender, EventArgs e)
	{
		// 商品初期設定画面へ遷移
		Response.Redirect(CreateProductRegistDefaultSettingUrl());
	}

	/// <summary>
	/// メニュー権限制御
	/// </summary>
	private void SetMenuAuthority()
	{
		lbExportProductDetailUrl.Visible = MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_PRODUCT_DETAILURL_DL);

		var canUseDefaultSetting = MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_PRODUCT_DEFAULT_SETTINGS_EDIT);
		btnDefaultSettingTop.Visible = canUseDefaultSetting;
		btnDefaultSettingBottom.Visible = canUseDefaultSetting;
	}

	/// <summary>
	/// 翻訳設定出力リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbExportTranslationData_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM] = this.ProductIdListOfDisplayedData ?? new string[0];
		Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN] = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_EXPORT);
	}

	/// <summary>
	/// マスタデータ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>マスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	public Hashtable CreateSearchParams()
	{
		return GetSearchSqlInfo(GetParameters());
	}

	/// <summary>
	/// ERPから最新情報取得クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGetLatestInfoFromErp_OnClick(object sender, EventArgs e)
	{
		if (new FlapsIntegrationLocker(Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCT).IsLocked())
		{
			FileLogger.WriteWarn("FLAPS商品同期処理中です。もうしばらくお待ちください。");
			return;
		}
		var arg = string.Format("\"{0}\"", Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCT);
		System.Diagnostics.Process.Start(Constants.PHYSICALDIRPATH_FLAPS_INTEGRATION_EXE, arg);
	}

	/// <summary>カレントページNO</summary>
	protected int CurrentPageNo
	{
		get { return (int)ViewState[Constants.REQUEST_KEY_PAGE_NO]; }
		set { ViewState[Constants.REQUEST_KEY_PAGE_NO] = value; }
	}
	/// <summary>画面に表示されている商品IDリスト</summary>
	protected string[] ProductIdListOfDisplayedData
	{
		get { return (string[])ViewState["productid_list_of_displayed_data"]; }
		set { ViewState["productid_list_of_displayed_data"] = value; }
	}
}

