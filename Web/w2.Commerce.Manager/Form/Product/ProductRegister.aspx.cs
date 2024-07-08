/*
=========================================================================================================
  Module      : 商品情報登録ページ処理(ProductRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Option;
using w2.App.Common.Product;
using w2.App.Common.ProductDefaultSetting;
using w2.App.Common.Web.WebCustomControl;
using w2.Domain.Product;
using w2.Domain.SubscriptionBox;
using w2.Domain;
using w2.Domain.FixedPurchase;
using w2.Domain.Product;

/// <summary>
/// 商品情報登録ページ処理
/// </summary>
public partial class Form_Product_ProductRegister : ProductPage
{
	/// <summary>Product size factor default</summary>
	protected const string PRODUCT_SIZE_FACTOR_DEFAULT = "1";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender">パラメータの説明を記述</param>
	/// <param name="e">パラメータの説明を記述</param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		this.HasErrorOnPostback = false;

		if (!IsPostBack)
		{
			// Clear Cache of Browser
			ClearBrowserCache();

			// 画面制御
			InitializeComponents();

			// データバインド
			DataBind();

			SetDefaultValue();
		}

		// Set display data
		Display();
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 配送サイズ区分
		ddlShippingSizeKbn.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlShippingSizeKbn.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_PRODUCT,
				Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN));

		// 在庫管理方法
		ddlStockManagementKbn.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_PRODUCT,
				Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN));

		ddlStockMessageId.Items.Add(new ListItem(string.Empty, string.Empty));
		var productStockMessages = DomainFacade.Instance.ProductStockMessageService
			.GetProductStockMessages(this.LoginOperatorShopId);
		ddlStockMessageId.Items.AddRange(
			productStockMessages.Select(
				item => new ListItem(
					item.StockMessageName,
					item.StockMessageId)).ToArray());

		// 配送種別（ない場合はエラー）
		var shopShippings = DomainFacade.Instance.ShopShippingService.GetAllShopShippings(this.LoginOperatorShopId);
		if (shopShippings.Length == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_SHOP_SHIPPING_NO_DATA);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		ddlShippingType.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlShippingType.Items.AddRange(
			shopShippings.Select(
				item => new ListItem(
					item.ShopShippingName,
					item.ShippingId)).ToArray());

		// 商品税率カテゴリ（ない場合はエラー）
		var taxCategories = DomainFacade.Instance.ProductTaxCategoryService.GetAllTaxCategory();
		ddlTaxCategory.Items.AddRange(
			taxCategories.Select(
				taxCategory => new ListItem(
					string.Format("{0}:{1}(%)", taxCategory.TaxCategoryName, taxCategory.TaxRate),
					taxCategory.TaxCategoryId)).ToArray());

		// モール出品設定見出し項目設定
		var mallExhibitsConfigs = ValueText.GetValueItemList(
			Constants.TABLE_MALLCOOPERATIONSETTING,
			Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG);
		var mallCooperationSettings = DomainFacade.Instance.MallCooperationSettingService.GetAll(this.LoginOperatorShopId);
		foreach (ListItem item in mallExhibitsConfigs)
		{
			foreach (var setting in mallCooperationSettings)
			{
				if (setting.MallExhibitsConfig == item.Value)
				{
					cblMallExhibitsConfig.Items.Add(
						new ListItem(
							setting.MallName,
							setting.MallExhibitsConfig));
					break;
				}
			}
		}

		// 会員ランク設定
		if (Constants.MEMBER_RANK_OPTION_ENABLED)
		{
			ddlDisplayMemberRank.Items.Add(new ListItem(string.Empty, string.Empty));
			ddlDisplayMemberRank.Items.AddRange(
				MemberRankOptionUtility.GetMemberRankList()
				.Select(memberRank => new ListItem(memberRank.MemberRankName, memberRank.MemberRankId))
				.ToArray());

			ddlBuyableMemberRank.Items.Add(new ListItem(string.Empty, string.Empty));
			ddlBuyableMemberRank.Items.AddRange(
				MemberRankOptionUtility.GetMemberRankList()
				.Select(memberRank => new ListItem(memberRank.MemberRankName, memberRank.MemberRankId))
				.ToArray());
		}

		// 商品バリエーション選択方法
		ddlSelectVariationKbn.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_PRODUCT,
				Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN));

		// ブランドID
		ddlBrandId1.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlBrandId2.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlBrandId3.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlBrandId4.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlBrandId5.Items.Add(new ListItem(string.Empty, string.Empty));
		var brandSettings = DomainFacade.Instance.ProductBrandService.GetProductBrands();
		foreach (var item in brandSettings)
		{
			ddlBrandId1.Items.Add(new ListItem(item.BrandName, item.BrandId));
			ddlBrandId2.Items.Add(new ListItem(item.BrandName, item.BrandId));
			ddlBrandId3.Items.Add(new ListItem(item.BrandName, item.BrandId));
			ddlBrandId4.Items.Add(new ListItem(item.BrandName, item.BrandId));
			ddlBrandId5.Items.Add(new ListItem(item.BrandName, item.BrandId));
		}

		// 定期購入フラグ
		ddlFixedPurchaseFlg.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_PRODUCT,
				Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG));

		// 頒布会フラグ
		ddlSubscriptionBoxFlg.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_PRODUCT,
				Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG));

		// ギフト購入フラグ
		ddlGiftFlg.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_PRODUCT,
				Constants.FIELD_PRODUCT_GIFT_FLG));

		// 決済利用不可
		var payments = GetPaymentValidList(this.LoginOperatorShopId);
		cblLimitedPayment.Items.AddRange(
			payments
				.Cast<DataRowView>()
				.Select(
					payment => new ListItem(
						(string)payment[Constants.FIELD_PAYMENT_PAYMENT_NAME],
						(string)payment[Constants.FIELD_PAYMENT_PAYMENT_ID]))
				.ToArray());

		// ユーザー管理レベル利用不可
		var models = DomainFacade.Instance.UserManagementLevelService.GetAllList();
		cblLimitedUserLevel.Items.AddRange(
			models.Select(m => new ListItem(m.UserManagementLevelName, m.UserManagementLevelId)).ToArray());

		// ポイント区分
		ddlPointKbn.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlPointKbn.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_PRODUCT,
				Constants.FIELD_PRODUCT_POINT_KBN1));

		ddlFixedPurchasePointKbn.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlFixedPurchasePointKbn.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_PRODUCT,
				Constants.FIELD_PRODUCT_POINT_KBN2));

		// 商品区分
		rblProductType.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_PRODUCT,
				Constants.FIELD_PRODUCT_PRODUCT_TYPE));

		// 商品カラー
		this.ProductColors = new List<ProductColor>(
			new[] { new ProductColor { Id = string.Empty, DispName = string.Empty } }
			.Concat(DataCacheControllerFacade.GetProductColorCacheController().GetProductColorList()));
	}

	/// <summary>
	/// ドロップダウン初期値設定
	/// </summary>
	private void SetDefaultValue()
	{
		var shippingType = GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SHIPPING_TYPE);
		if (ddlShippingType.Items.FindByValue(shippingType) != null)
		{
			ddlShippingType.SelectedValue = shippingType;
		}

		var taxCategoryId = GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_TAX_CATEGORY_ID);
		if (ddlTaxCategory.Items.FindByValue(taxCategoryId) != null)
		{
			ddlTaxCategory.SelectedValue = taxCategoryId;
		}

		var shippingSizeKbn = GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN);
		if (ddlShippingSizeKbn.Items.FindByValue(shippingSizeKbn) != null)
		{
			ddlShippingSizeKbn.SelectedValue = shippingSizeKbn;
		}

		var pointKbn1 = GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_POINT_KBN1);
		if (ddlPointKbn.Items.FindByValue(pointKbn1) != null)
		{
			ddlPointKbn.SelectedValue = pointKbn1;
		}

		var pointKbn2 = GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_POINT_KBN2);
		if (ddlFixedPurchasePointKbn.Items.FindByValue(pointKbn2) != null)
		{
			ddlFixedPurchasePointKbn.SelectedValue = pointKbn2;
		}

		// 商品在庫文言設定
		foreach (ListItem item in ddlStockMessageId.Items)
		{
			item.Selected = (item.Value == GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID));
		}

		// 会員ランクオプション有り
		if (Constants.MEMBER_RANK_OPTION_ENABLED)
		{
			// 会員ランク設定
			var memberRankDiscountFlg = GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG);
			cbMemberRankDiscountFlg.Checked = (memberRankDiscountFlg == Constants.FLG_PRODUCT_MEMBER_RANK_DISCOUNT_FLG_VALID);

			ddlDisplayMemberRank.SelectedValue = GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK);
			ddlBuyableMemberRank.SelectedValue = GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK);

			cbDisplayFixedPurchaseMemberLimit.Checked =
				(GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG)
					== Constants.FLG_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG_ON);
			cbBuyableFixedPurchaseMemberLimit.Checked =
				(GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG)
					== Constants.FLG_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG_ON);
		}

		// For the case brand option is enabled
		if (Constants.PRODUCT_BRAND_ENABLED)
		{
			// ブランドID
			ddlBrandId1.SelectedValue = GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_BRAND_ID1);
			ddlBrandId2.SelectedValue = GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_BRAND_ID2);
			ddlBrandId3.SelectedValue = GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_BRAND_ID3);
			ddlBrandId4.SelectedValue = GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_BRAND_ID4);
			ddlBrandId5.SelectedValue = GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_BRAND_ID5);
		}

		// 決済利用不可
		foreach (ListItem item in cblLimitedPayment.Items)
		{
			var payments = StringUtility.ToEmpty(GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS));
			item.Selected = payments.Contains(item.Value);
		}

		// 定期購入利用不可ユーザー管理レベル
		foreach (ListItem item in cblLimitedUserLevel.Items)
		{
			var userLevels = StringUtility.ToEmpty(GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS));
			item.Selected = userLevels.Contains(item.Value);
		}

		// キャンペーンアイコン初期値をビューステートにセット
		this.InitialIconFlgTermEnds = Enumerable.Range(1, Constants.ICON_FLG_TERM_END_NAMES.Length)
			.Select(i => GetIconFlgTermEnd(i))
			.ToArray();

		var productOptionSettingKeys = ProductOptionSettingHelper.GetAllProductOptionSettingKeys();
		var productOptionValueDefaultSettings = productOptionSettingKeys
			.Select(posKey => GetProductDefaultSettingFieldValue(posKey)).ToArray();
		rProductOptionValueDefaultSetting.DataSource = productOptionValueDefaultSettings;
		rProductOptionValueDefaultSetting.DataBind();

		// 表示期間
		DateTime displayStart;
		ucDisplay.SetStartDate(DateTime.Today);
		if (DateTime.TryParse(GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_FROM), out displayStart))
		{
			ucDisplay.SetStartDate(displayStart);
		}
		DateTime displayEnd;
		ucDisplay.SetEndDate((DateTime?)null);
		if (DateTime.TryParse(GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_DISPLAY_TO), out displayEnd))
		{
			ucDisplay.SetEndDate(displayEnd);
		}

		// 販売期間
		DateTime sellStart;
		ucSell.SetStartDate(DateTime.Today);
		if (DateTime.TryParse(GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SELL_FROM), out sellStart))
		{
			ucSell.SetStartDate(sellStart);
		}
		DateTime sellEnd;
		ucSell.SetEndDate((DateTime?)null);
		if (DateTime.TryParse(GetProductDefaultSettingFieldValue(Constants.FIELD_PRODUCT_SELL_TO), out sellEnd))
		{
			ucSell.SetEndDate(sellEnd);
		}
	}

	/// <summary>
	/// Display
	/// </summary>
	private void Display()
	{
		if ((ddlShippingSizeKbn.SelectedValue == Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL)
			&& string.IsNullOrEmpty(tbProductSizeFactor.Text))
		{
			tbProductSizeFactor.Text = PRODUCT_SIZE_FACTOR_DEFAULT;
		}

		// Set checked status of the default setting checkbox to checked
		cbDisplayOrderDefaultSetting.Checked = true;
		cbBrandId1DefaultSetting.Checked = true;
	}

	/// <summary>
	/// Get product default setting input
	/// </summary>
	/// <returns>Product default setting table</returns>
	private ProductDefaultSettingTable GetProductDefaultSettingInput()
	{
		var productDefaultSettingTable = new ProductDefaultSettingTable(Constants.TABLE_PRODUCT);

		// 店舗ID
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_SHOP_ID,
			this.LoginOperatorShopId,
			string.Empty,
			false);

		// 商品ID
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_PRODUCT_ID,
			cbProductIdHasDefault.Checked
				? tbProductId.Text.Trim()
				: null,
			tbProductIdDefaultSetting.Text.Trim(),
			cbProductIdDefaultSetting.Checked);

		// 備考
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_NOTE,
			cbNoteHasDefault.Checked
				? tbNote.Text.Trim()
				: null,
			tbNoteDefaultSetting.Text.Trim(),
			cbNoteDefaultSetting.Checked);

		// サプライヤID
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_SUPPLIER_ID,
			cbSupplierIdHasDefault.Checked
				? tbSupplierId.Text.Trim()
				: null,
			tbSupplierIdDefaultSetting.Text.Trim(),
			cbSupplierIdDefaultSetting.Checked);

		// 商品連携ID1
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_COOPERATION_ID1,
			cbCooperationId1HasDefault.Checked
				? tbCooperationId1.Text.Trim()
				: null,
			tbCooperationId1DefaultSetting.Text.Trim(),
			cbCooperationId1DefaultSetting.Checked);

		// 商品連携ID2
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_COOPERATION_ID2,
			cbCooperationId2HasDefault.Checked
				? tbCooperationId2.Text.Trim()
				: null,
			tbCooperationId2DefaultSetting.Text.Trim(),
			cbCooperationId2DefaultSetting.Checked);

		// 商品連携ID3
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_COOPERATION_ID3,
			cbCooperationId3HasDefault.Checked
				? tbCooperationId3.Text.Trim()
				: null,
			tbCooperationId3DefaultSetting.Text.Trim(),
			cbCooperationId3DefaultSetting.Checked);

		// 商品連携ID4
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_COOPERATION_ID4,
			cbCooperationId4HasDefault.Checked
				? tbCooperationId4.Text.Trim()
				: null,
			tbCooperationId4DefaultSetting.Text.Trim(),
			cbCooperationId4DefaultSetting.Checked);

		// 商品連携ID5
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_COOPERATION_ID5,
			cbCooperationId5HasDefault.Checked
				? tbCooperationId5.Text.Trim()
				: null,
			tbCooperationId5DefaultSetting.Text.Trim(),
			cbCooperationId5DefaultSetting.Checked);

		// 商品連携ID6
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_COOPERATION_ID6,
			cbCooperationId6HasDefault.Checked
				? tbCooperationId6.Text.Trim()
				: null,
			tbCooperationId6DefaultSetting.Text.Trim(),
			cbCooperationId6DefaultSetting.Checked);

		// 商品連携ID7
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_COOPERATION_ID7,
			cbCooperationId7HasDefault.Checked
				? tbCooperationId7.Text.Trim()
				: null,
			tbCooperationId7DefaultSetting.Text.Trim(),
			cbCooperationId7DefaultSetting.Checked);

		// 商品連携ID8
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_COOPERATION_ID8,
			cbCooperationId8HasDefault.Checked
				? tbCooperationId8.Text.Trim()
				: null,
			tbCooperationId8DefaultSetting.Text.Trim(),
			cbCooperationId8DefaultSetting.Checked);

		// 商品連携ID9
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_COOPERATION_ID9,
			cbCooperationId9HasDefault.Checked
				? tbCooperationId9.Text.Trim()
				: null,
			tbCooperationId9DefaultSetting.Text.Trim(),
			cbCooperationId9DefaultSetting.Checked);

		// 商品連携ID10
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_COOPERATION_ID10,
			cbCooperationId10HasDefault.Checked
				? tbCooperationId10.Text.Trim()
				: null,
			tbCooperationId10DefaultSetting.Text.Trim(),
			cbCooperationId10DefaultSetting.Checked);

		// モール拡張商品ID
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID,
			cbMallProductIdHasDefault.Checked
				? tbMallProductId.Text.Trim()
				: null,
			tbMallProductIdDefaultSetting.Text.Trim(),
			cbMallProductIdDefaultSetting.Checked);

		// モール出品設定
		productDefaultSettingTable.Add(
			Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG,
			null,
			tblMallExhibitsConfigDefaultSetting.Text.Trim(),
			cblMallExhibitsConfigDefaultSetting.Checked);

		// 商品名
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_NAME,
			cbNameHasDefault.Checked
				? tbName.Text.Trim()
				: null,
			tbNameDefaultSetting.Text.Trim(),
			cbNameDefaultSetting.Checked);

		// 商品名(フリガナ)
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_NAME_KANA,
			cbNameKanaHasDefault.Checked
				? tbNameKana.Text.Trim()
				: null,
			tbNameKanaDefaultSetting.Text.Trim(),
			cbNameKanaDefaultSetting.Checked);

		// SEOキーワード
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_SEO_KEYWORDS,
			cbSeoKeywordsHasDefault.Checked
				? tbSeoKeywords.Text.Trim()
				: null,
			tbSeoKeywordsDefaultSetting.Text.Trim(),
			cbSeoKeywordsDefaultSetting.Checked);

		// キャッチコピー
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_CATCHCOPY,
			cbCatchcopyHasDefault.Checked
				? tbCatchcopy.Text.Trim()
				: null,
			tbCatchcopyDefaultSetting.Text.Trim(),
			cbCatchcopyDefaultSetting.Checked);

		// 検索キーワード
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_SEARCH_KEYWORD,
			cbSearchKeywordHasDefault.Checked
				? tbSearchKeyword.Text.Trim()
				: null,
			tbSearchKeywordDefaultSetting.Text.Trim(),
			cbSearchKeywordDefaultSetting.Checked);

		// 商品概要(TEXT OR HTML)
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_OUTLINE_KBN,
			cbOutlineHasDefault.Checked
				? rbOutlineFlg.SelectedValue
				: null,
			tbOutlineDefaultSetting.Text.Trim(),
			cbOutlineDefaultSetting.Checked);

		// 商品概要
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_OUTLINE,
			cbOutlineHasDefault.Checked
				? tbOutline.Text.Trim()
				: null,
			tbOutlineDefaultSetting.Text.Trim(),
			cbOutlineDefaultSetting.Checked);

		// 商品詳細説明1(TEXT OR HTML)
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1,
			cbDescDetail1HasDefault.Checked
				? rbDescDetailFlg1.SelectedValue
				: null,
			tbDescDetail1DefaultSetting.Text.Trim(),
			cbDescDetail1DefaultSetting.Checked);

		// 商品詳細説明1
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DESC_DETAIL1,
			cbDescDetail1HasDefault.Checked
				? tbDescDetail1.Text.Trim()
				: null,
			tbDescDetail1DefaultSetting.Text.Trim(),
			cbDescDetail1DefaultSetting.Checked);

		// 商品詳細説明2(TEXT OR HTML)
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2,
			cbDescDetail2HasDefault.Checked
				? rbDescDetailFlg2.SelectedValue
				: null,
			tbDescDetail2DefaultSetting.Text.Trim(),
			cbDescDetail2DefaultSetting.Checked);

		// 商品詳細説明2
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DESC_DETAIL2,
			cbDescDetail2HasDefault.Checked
				? tbDescDetail2.Text.Trim()
				: null,
			tbDescDetail2DefaultSetting.Text.Trim(),
			cbDescDetail2DefaultSetting.Checked);

		// 商品詳細説明3(TEXT OR HTML)
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DESC_DETAIL_KBN3,
			cbDescDetail3HasDefault.Checked
				? rbDescDetailFlg3.SelectedValue
				: null,
			tbDescDetail3DefaultSetting.Text.Trim(),
			cbDescDetail3DefaultSetting.Checked);

		// 商品詳細説明3
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DESC_DETAIL3,
			cbDescDetail3HasDefault.Checked
				? tbDescDetail3.Text.Trim()
				: null,
			tbDescDetail3DefaultSetting.Text.Trim(),
			cbDescDetail3DefaultSetting.Checked);

		// 商品詳細説明4(TEXT OR HTML)
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DESC_DETAIL_KBN4,
			cbDescDetail4HasDefault.Checked
				? rbDescDetailFlg4.SelectedValue
				: null,
			tbDescDetail4DefaultSetting.Text.Trim(),
			cbDescDetail4DefaultSetting.Checked);

		// 商品詳細説明4
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DESC_DETAIL4,
			cbDescDetail4HasDefault.Checked
				? tbDescDetail4.Text.Trim()
				: null,
			tbDescDetail4DefaultSetting.Text.Trim(),
			cbDescDetail4DefaultSetting.Checked);

		// 返品交換文言
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE,
			cbReturnExchangeMessageHasDefault.Checked
				? tbReturnExchangeMessage.Text.Trim()
				: null,
			tbReturnExchangeMessageDefaultSetting.Text.Trim(),
			cbReturnExchangeMessageDefaultSetting.Checked);

		// 紹介URL
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_URL,
			cbUrlHasDefault.Checked
				? tbUrl.Text.Trim()
				: null,
			tbUrlDefaultSetting.Text.Trim(),
			cbUrlDefaultSetting.Checked);

		// 問い合わせ用メールアドレス
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_INQUIRE_EMAIL,
			cbInquiteEmailHasDefault.Checked
				? tbInquiteEmail.Text.Trim()
				: null,
			tbInquiteEmailDefaultSetting.Text.Trim(),
			cbInquiteEmailDefaultSetting.Checked);

		// 問い合わせ用電話番号
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_INQUIRE_TEL,
			cbInquiteTelHasDefault.Checked
				? tbInquiteTel.Text.Trim()
				: null,
			tbInquiteTelDefaultSetting.Text.Trim(),
			cbInquiteTelDefaultSetting.Checked);

		// 販売価格
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DISPLAY_PRICE,
			cbPriceHasDefault.Checked
				? tbPrice.Text.Trim()
				: null,
			tbPriceDefaultSetting.Text.Trim(),
			cbPriceDefaultSetting.Checked);

		// 特別価格
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE,
			cbSpecialPriceHasDefault.Checked
				? StringUtility.ToNull(tbSpecialPrice.Text.Trim())
				: null,
			tbSpecialPriceDefaultSetting.Text.Trim(),
			cbSpecialPriceDefaultSetting.Checked);

		// 通常購入制限チェック
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG,
			cbCheckProductOrderLimitHasDefault.Checked
				? cbCheckProductOrderLimit.Checked
					? Constants.FLG_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG_VALID
					: Constants.FLG_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG_INVALID
				: null,
			tbCheckProductOrderLimitDefaultSetting.Text.Trim(),
			cbCheckProductOrderLimitDefaultSetting.Checked);

		// 定期購入制限チェック
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG,
			cbCheckFixedProductOrderLimitHasDefault.Checked
				? cbCheckFixedProductOrderLimit.Checked
					? Constants.FLG_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG_VALID
					: Constants.FLG_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG_INVALID
				: null,
			tbCheckFixedProductOrderLimitDefaultSetting.Text.Trim(),
			cbCheckFixedProductOrderLimitDefaultSetting.Checked);

		// 定期初回購入価格
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE,
			cbFixedPurchaseFirsttimePriceHasDefault.Checked
				? StringUtility.ToNull(tbFixedPurchaseFirsttimePrice.Text.Trim())
				: null,
			tbFixedPurchaseFirsttimePriceDefaultSetting.Text.Trim(),
			cbFixedPurchaseFirsttimePriceDefaultSetting.Checked);

		// 定期購入価格
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE,
			cbFixedPurchasePriceHasDefault.Checked
				? StringUtility.ToNull(tbFixedPurchasePrice.Text.Trim())
				: null,
			tbFixedPurchasePriceDefaultSetting.Text.Trim(),
			cbFixedPurchasePriceDefaultSetting.Checked);

		// 商品税率カテゴリ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_TAX_CATEGORY_ID,
			cbTaxCategoryHasDefault.Checked
				? ddlTaxCategory.SelectedValue
				: null,
			tbTaxCategoryDefaultSetting.Text.Trim(),
			cbTaxCategoryDefaultSetting.Checked);

		// 商品会員ランク価格
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE,
			null,
			tbMemberRankPriceDefaultSetting.Text.Trim(),
			cbMemberRankPriceDefaultSetting.Checked);

		// 付与ポイント区分1
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_POINT_KBN1,
			(Constants.W2MP_POINT_OPTION_ENABLED && cbPointHasDefault.Checked)
				? ddlPointKbn.SelectedValue
				: null,
			tbPointDefaultSetting.Text.Trim(),
			cbPointDefaultSetting.Checked);

		// 付与ポイント1
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_POINT1,
			(Constants.W2MP_POINT_OPTION_ENABLED && cbPointHasDefault.Checked)
				? tbPoint.Text.Trim()
				: null,
			tbPointDefaultSetting.Text.Trim(),
			cbPointDefaultSetting.Checked);

		// 付与ポイント区分2
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_POINT_KBN2,
			cbPointHasDefault.Checked
				? ddlFixedPurchasePointKbn.SelectedValue
				: null,
			tbPointDefaultSetting.Text.Trim(),
			cbPointDefaultSetting.Checked);

		// 付与ポイント2
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_POINT2,
			cbPointHasDefault.Checked
				? tbIncFixedPurchasePoint.Text.Trim()
				: null,
			tbPointDefaultSetting.Text.Trim(),
			cbPointDefaultSetting.Checked);

		// 配送種別
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_SHIPPING_TYPE,
			cbShippingTypeHasDefault.Checked
				? ddlShippingType.SelectedValue
				: null,
			tbShippingTypeDefaultSetting.Text.Trim(),
			cbShippingTypeDefaultSetting.Checked);

		// 配送種別(確認画面表示用)
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN,
			cbShippingSizeKbnHasDefault.Checked
				? ddlShippingSizeKbn.SelectedValue
				: null,
			tbShippingSizeKbnDefaultSetting.Text.Trim(),
			cbShippingSizeKbnDefaultSetting.Checked);

		// 商品サイズ係数
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR,
			cbCProductSizeFactorHasDefault.Checked
				? tbProductSizeFactor.Text.Trim()
				: null,
			tbProductSizeFactorDefaultSetting.Text.Trim(),
			cbCProductSizeFactorDefaultSetting.Checked);

		// 商品カラーID（商品単位）
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID,
			cbColorImageHasDefault.Checked
				? ddlColorImage.SelectedValue
				: null,
			tbColorImageDefaultSetting.Text.Trim(),
			cbColorImageDefaultSetting.Checked);

		// 税区分
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG,
			cbTaxIncludeHasDefault.Checked
				? TaxCalculationUtility.GetPrescribedProductTaxIncludedFlag()
				: null,
			tbTaxIncludeDefaultSetting.Text.Trim(),
			cbTaxIncludeDefaultSetting.Checked);

		// 表示期間(FROM)
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DISPLAY_FROM,
			cbDisplayHasDefault.Checked
				? ucDisplay.StartDateTimeString
				: null,
			tbDisplayDefaultSetting.Text.Trim(),
			cbDisplayDefaultSetting.Checked);

		// 表示期間(TO)
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DISPLAY_TO,
			cbDisplayHasDefault.Checked
				? ucDisplay.EndDateTimeString
				: null,
			tbDisplayDefaultSetting.Text.Trim(),
			cbDisplayDefaultSetting.Checked);
		productDefaultSettingTable.Add(
			ProductDefaultSettingTable.FIELD_PRODUCT_DISPLAY_TO_FOR_CHECK,
			(cbDisplayHasDefault.Checked && (string.IsNullOrEmpty(ucDisplay.EndDateTimeString) == false))
				? ucDisplay.EndDateTimeString
				: null,
			tbDisplayDefaultSetting.Text.Trim(),
			cbDisplayDefaultSetting.Checked);

		// 販売期間(FROM)
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_SELL_FROM,
			cbSellHasDefault.Checked
				? ucSell.StartDateTimeString
				: null,
			tbSellDefaultSetting.Text.Trim(),
			cbSellDefaultSetting.Checked);

		// 販売期間(TO)
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_SELL_TO,
			cbSellHasDefault.Checked
				? ucSell.EndDateTimeString
				: null,
			tbSellDefaultSetting.Text.Trim(),
			cbSellDefaultSetting.Checked);
		productDefaultSettingTable.Add(
			ProductDefaultSettingTable.FIELD_PRODUCT_SELL_TO_FOR_CHECK,
			(cbSellHasDefault.Checked && (string.IsNullOrEmpty(ucSell.EndDateTimeString) == false))
				? ucSell.EndDateTimeString
				: null,
			tbSellDefaultSetting.Text.Trim(),
			cbSellDefaultSetting.Checked);

		// 販売期間表示フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DISPLAY_SELL_FLG,
			cbSellHasDefault.Checked
				? cbDisplaySellFlg.Checked
					? Constants.FLG_PRODUCT_DISPLAY_SELL_FLG_DISP
					: Constants.FLG_PRODUCT_DISPLAY_SELL_FLG_UNDISP
				: null,
			tbSellDefaultSetting.Text.Trim(),
			cbSellDefaultSetting.Checked);

		// 表示優先順
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DISPLAY_PRIORITY,
			cbDisplayPriorityHasDefault.Checked
				? tbDisplayPriority.Text.Trim()
				: null,
			tbisplayPriorityDefaultSetting.Text.Trim(),
			cbDisplayPriorityDefaultSetting.Checked);

		// 販売可能数量
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY,
			cbMaxSellQuantityHasDefault.Checked
				? tbMaxSellQuantity.Text.Trim()
				: null,
			tbMaxSellQuantityDefaultSetting.Text.Trim(),
			cbMaxSellQuantityDefaultSetting.Checked);

		// 在庫管理
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN,
			cbStockManagementKbnHasDefault.Checked
				? ddlStockManagementKbn.SelectedValue
				: null,
			tbStockManagementKbnDefaultSetting.Text.Trim(),
			cbStockManagementKbnDefaultSetting.Checked);

		// 在庫文言
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID,
			cbStockMessageIdHasDefault.Checked
				? ddlStockMessageId.SelectedValue
				: null,
			tbStockMessageIdDefaultSetting.Text.Trim(),
			cbStockMessageIdDefaultSetting.Checked);

		// 定期購入フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG,
			cbFixedPurchaseFlgHasDefault.Checked
				? ddlFixedPurchaseFlg.SelectedValue
				: null,
			tbFixedPurchaseFlgDefaultSetting.Text.Trim(),
			cbFixedPurchaseFlgDefaultSetting.Checked);

		// 頒布会フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG,
			cbDistributionFlgHasDefault.Checked
				? (ddlFixedPurchaseFlg.SelectedValue != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)
					? ddlSubscriptionBoxFlg.SelectedValue
					: Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID
				: null,
			tbDistributionFlgDefaultSetting.Text.Trim(),
			cbDistributionFlgDefaultSetting.Checked);

		// ギフト購入フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_GIFT_FLG,
			cbGiftFlgHasDefault.Checked
				? ddlGiftFlg.SelectedValue
				: null,
			tbGiftFlgDefaultSetting.Text.Trim(),
			cbGiftFlgDefaultSetting.Checked);

		// 有効フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_VALID_FLG,
			cbValidFlgHasDefault.Checked
				? cbValidFlg.Checked
					? Constants.FLG_PRODUCT_VALID_FLG_VALID
					: Constants.FLG_PRODUCT_VALID_FLG_INVALID
				: null,
			tbValidFlgDefaultSetting.Text.Trim(),
			cbValidFlgDefaultSetting.Checked);

		// 商品表示区分
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DISPLAY_KBN,
			cbDisplayKbnHasDefault.Checked
				? rbDisplayKbn1.Checked
					? Constants.FLG_PRODUCT_DISPLAY_DISP_ALL
					: rbDisplayKbn2.Checked
						? Constants.FLG_PRODUCT_DISPLAY_DISP_ONLY_DETAIL
						: rbDisplayKbn3.Checked
							? Constants.FLG_PRODUCT_DISPLAY_UNDISP_ALL
							: null
				: null,
			tbDisplayKbnDefaultSetting.Text.Trim(),
			cbDisplayKbnDefaultSetting.Checked);

		// 会員ランク割引対象フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG,
			cbMemberRankDiscountFlgHasDefault.Checked
				? cbMemberRankDiscountFlg.Checked
					? Constants.FLG_PRODUCT_MEMBER_RANK_DISCOUNT_FLG_VALID
					: Constants.FLG_PRODUCT_MEMBER_RANK_DISCOUNT_FLG_INVALID
				: null,
			tbMemberRankDiscountFlgDefaultSetting.Text.Trim(),
			cbMemberRankDiscountFlgDefaultSetting.Checked);

		// 閲覧可能会員フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK,
			cbDisplayMemberRankHasDefault.Checked
				? ddlDisplayMemberRank.SelectedValue
				: null,
			tbDisplayMemberRankDefaultSetting.Text.Trim(),
			cbDisplayMemberRankDefaultSetting.Checked);

		// 購入可能会員フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK,
			cbBuyableMemberRankHasDefault.Checked
				? ddlBuyableMemberRank.SelectedValue
				: null,
			tbBuyableMemberRankDefaultSetting.Text.Trim(),
			cbBuyableMemberRankDefaultSetting.Checked);

		// Googleショッピング連携設定
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG,
			cbGoogleShoppingFlgHasDefault.Checked
				? cbGoogleShoppingFlg.Checked
					? Constants.FLG_PRODUCT_GOOGLE_SHOPPING_FLG_VALID
					: Constants.FLG_PRODUCT_GOOGLE_SHOPPING_FLG_INVALID
				: null,
			tbGoogleShoppingFlgDefaultSetting.Text.Trim(),
			cbGoogleShoppingFlgDefaultSetting.Checked);

		// 再入荷通知メール有効フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG,
			cbArrivalMailValidFlgHasDefault.Checked
				? cbArrivalMailValidFlg.Checked
					? Constants.FLG_PRODUCT_ARRIVAL_MAIL_VALID_FLG_VALID
					: Constants.FLG_PRODUCT_ARRIVAL_MAIL_VALID_FLG_INVALID
				: null,
			tbArrivalMailValidFlgDefaultSetting.Text.Trim(),
			cbArrivalMailValidFlgDefaultSetting.Checked);

		// 販売開始通知メール有効フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG,
			cbReleaseMailValidFlgHasDefault.Checked
				? cbReleaseMailValidFlg.Checked
					? Constants.FLG_PRODUCT_RELEASE_MAIL_VALID_FLG_VALID
					: Constants.FLG_PRODUCT_RELEASE_MAIL_VALID_FLG_INVALID
				: null,
			tbReleaseMailValidFlgDefaultSetting.Text.Trim(),
			cbReleaseMailValidFlgDefaultSetting.Checked);

		// 再販売通知メール有効フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG,
			cbResaleMailValidFlgHasDefault.Checked
				? cbResaleMailValidFlg.Checked
					? Constants.FLG_PRODUCT_RESALE_MAIL_VALID_FLG_VALID
					: Constants.FLG_PRODUCT_RESALE_MAIL_VALID_FLG_INVALID
				: null,
			tbResaleMailValidFlgDefaultSetting.Text.Trim(),
			cbResaleMailValidFlgDefaultSetting.Checked);

		// PC用商品バリエーション選択方法
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN,
			cbSelectVariationKbnHasDefault.Checked
				? ddlSelectVariationKbn.SelectedValue
				: null,
			tbSelectVariationKbnDefaultSetting.Text.Trim(),
			cbSelectVariationKbnDefaultSetting.Checked);

		// 外部レコメンド利用フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG,
			cbUseRecommendFlgHasDefault.Checked
				? cbUseRecommendFlg.Checked
					? Constants.FLG_PRODUCT_USE_RECOMMEND_FLG_VALID
					: Constants.FLG_PRODUCT_USE_RECOMMEND_FLG_INVALID
				: null,
			tbUseRecommendFlgDefaultSetting.Text.Trim(),
			cbUseRecommendFlgDefaultSetting.Checked);

		// 配送料複数個無料フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG,
			cbPluralShippingPriceFreeHasDefault.Checked
				? cbPluralShippingPriceFree.Checked
					? Constants.FLG_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG_VALID
					: Constants.FLG_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG_INVALID
				: null,
			tbPluralShippingPriceFreeDeflultSetting.Text.Trim(),
			cbPluralShippingPriceFreeDefaultSetting.Checked);

		// 年齢制限フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_AGE_LIMIT_FLG,
			cbAgeLimitFlgHasDefault.Checked
				? cbAgeLimitFlg.Checked
					? Constants.FLG_PRODUCT_AGE_LIMIT_FLG_VALID
					: Constants.FLG_PRODUCT_AGE_LIMIT_FLG_INVALID
				: null,
			tbAgeLimitFlgDefaultSetting.Text.Trim(),
			cbAgeLimitFlgDefaultSetting.Checked);

		// デジタルコンテンツ商品フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG,
			cbDigitalContentsFlgHasDefault.Checked
				? cbDigitalContentsFlg.Checked
					? Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID
					: Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_INVALID
				: null,
			tbDigitalContentsFlgDefaultSetting.Text.Trim(),
			cbDigitalContentsFlgDefaultSetting.Checked);

		// カート投入URL制限フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG,
			cbAddCartUrlLimitFlgHasDefault.Checked
				? cbAddCartUrlLimitFlg.Checked
					? Constants.FLG_PRODUCT_ADD_CART_URL_LIMIT_FLG_VALID
					: Constants.FLG_PRODUCT_ADD_CART_URL_LIMIT_FLG_INVALID
				: null,
			tbAddCartUrlLimitFlgDefaultSetting.Text.Trim(),
			cbAddCartUrlLimitFlgDefaultSetting.Checked);

		// ダウンロードURL
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DOWNLOAD_URL,
			cbDownloadUrlHasDefault.Checked
				? tbDownloadUrl.Text.Trim()
				: null,
			tbDownloadUrlDefaultSetting.Text.Trim(),
			cbDownloadUrlDefaultSetting.Checked);

		// Limited Payment
		var limitedPayments = cblLimitedPayment.Items.Cast<ListItem>()
			.Where(item => item.Selected)
			.Select(item => item.Value);
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS,
			cbLimitedPaymentHasDefault.Checked
				? string.Join(",", limitedPayments)
				: null,
			tbLimitedPaymentDefaultSetting.Text.Trim(),
			cbLimitedPaymentDefaultSetting.Checked);

		// 同梱商品明細表示フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE,
			cbBundleItemDisplayTypeHasDefault.Checked
				? cbBundleItemDisplayType.Checked
					? Constants.FLG_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE_VALID
					: Constants.FLG_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE_INVALID
				: null,
			tbBundleItemDisplayTypeDefaultSetting.Text.Trim(),
			cbBundleItemDisplayTypeDefaultSetting.Checked);

		// 商品区分
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_PRODUCT_TYPE,
			cbProductTypeHasDefault.Checked
				? rblProductType.SelectedValue
				: null,
			tbProductTypeDefaultSetting.Text.Trim(),
			cbProductTypeDefaultSetting.Checked);

		// ＆mall連携予約商品フラグ設定
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG,
			cbAndMallReservationFlgDefaultSetting.Checked
				? cbAndMallReservationFlg.Checked
					? Constants.FLG_PRODUCT_ANDMALL_RESERVATION_FLG_RESERVATION
					: Constants.FLG_PRODUCT_ANDMALL_RESERVATION_FLG_COMMON
				: null,
			tbAndMallReservationFlgDefaultSetting.Text.Trim(),
			cbAndMallReservationFlgDefaultSetting.Checked);

		// 定期購入解約可能回数
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT,
			cbFixedPurchaseCancelableCountHasDefault.Checked
				? tbFixedPurchaseCancelableCount.Text.Trim()
				: null,
			tbFixedPurchaseCancelableCountDefaultSetting.Text.Trim(),
			cbFixedPurchaseCancelableCountDefaultSetting.Checked);

		// 定期購入利用不可ユーザー管理レベル
		var limitedUserLevels = cblLimitedUserLevel.Items.Cast<ListItem>()
			.Where(item => item.Selected)
			.Select(item => item.Value);
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS,
			cbLimitedUserLevelHasDefault.Checked
				? string.Join(",", limitedUserLevels)
				: null,
			tbLimitedUserLevelDefaultSetting.Text.Trim(),
			cbLimitedUserLevelDefaultSetting.Checked);

		// 商品付帯情報
		var defaultProductOptionValue = new StringBuilder();
		foreach (var povdsItem in rProductOptionValueDefaultSetting.Items.Cast<RepeaterItem>())
		{
			// 商品付帯情報（個別）
			var cbProductOptionValueHasDefault = (CheckBox)povdsItem.FindControl("cbProductOptionValueHasDefault");
			var tbProductOptionValueDefaultSetting = (TextBox)povdsItem.FindControl("tbProductOptionValueDefaultSetting");
			var cbProductOptionValueDefaultSetting = (CheckBox)povdsItem.FindControl("cbProductOptionValueDefaultSetting");
			var tbProductOptionValueDefaultValue = (TextBox)povdsItem.FindControl("tbProductOptionValueDefaultValue");
			var key = ProductOptionSettingHelper.GetProductOptionSettingKey(povdsItem.ItemIndex + 1);
			productDefaultSettingTable.Add(
				key,
				(cbProductOptionValueHasDefault.Checked)
					? tbProductOptionValueDefaultValue.Text.Trim()
					: null,
				tbProductOptionValueDefaultSetting.Text,
				cbProductOptionValueDefaultSetting.Checked);

			// 商品付帯情報（全体）
			if (cbProductOptionValueHasDefault.Checked)
			{
				defaultProductOptionValue.Append(tbProductOptionValueDefaultValue.Text.Trim());
			}
			else
			{
				tbProductOptionValueDefaultValue.Text = string.Empty;
			}
		}

		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_PRODUCT_OPTION_SETTINGS,
			(defaultProductOptionValue.Length != 0)
				? defaultProductOptionValue.ToString()
				: null,
			string.Empty,
			(defaultProductOptionValue.Length != 0));

		// カテゴリ1
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_CATEGORY_ID1,
			cbCategoryId1HasDefault.Checked
				? tbCategoryId1.Text.Trim()
				: null,
			tbCategoryId1DefaultSetting.Text.Trim(),
			cbCategoryId1DefaultSetting.Checked);

		// カテゴリ2
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_CATEGORY_ID2,
			cbCategoryId2HasDefault.Checked
				? tbCategoryId2.Text.Trim()
				: null,
			tbCategoryId2DefaultSetting.Text.Trim(),
			cbCategoryId2DefaultSetting.Checked);

		// カテゴリ3
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_CATEGORY_ID3,
			cbCategoryId3HasDefault.Checked
				? tbCategoryId3.Text.Trim()
				: null,
			tbCategoryId3DefaultSetting.Text.Trim(),
			cbCategoryId3DefaultSetting.Checked);

		// カテゴリ4
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_CATEGORY_ID4,
			cbCategoryId4HasDefault.Checked
				? tbCategoryId4.Text.Trim()
				: null,
			tbCategoryId4DefaultSetting.Text.Trim(),
			cbCategoryId4DefaultSetting.Checked);

		// カテゴリ5
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_CATEGORY_ID5,
			cbCategoryId5HasDefault.Checked
				? tbCategoryId5.Text.Trim()
				: null,
			tbCategoryId5DefaultSetting.Text.Trim(),
			cbCategoryId5DefaultSetting.Checked);

		// ブランドID1
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_BRAND_ID1,
			cbBrandId1HasDefault.Checked
				? ddlBrandId1.SelectedValue
				: null,
			tbBrandId1DefaultSetting.Text.Trim(),
			cbBrandId1DefaultSetting.Checked);
		productDefaultSettingTable.Add(
			ProductDefaultSettingTable.FIELD_PRODUCT_BRAND_ID1_FOR_CHECK,
			cbBrandId1HasDefault.Checked
				? ddlBrandId1.SelectedValue
				: null,
			tbBrandId1DefaultSetting.Text.Trim(),
			cbBrandId1DefaultSetting.Checked);
		// ブランドID2
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_BRAND_ID2,
			cbBrandId2HasDefault.Checked
				? ddlBrandId2.SelectedValue
				: null,
			tbBrandId2DefaultSetting.Text.Trim(),
			cbBrandId2DefaultSetting.Checked);

		// ブランドID3
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_BRAND_ID3,
			cbBrandId3HasDefault.Checked
				? ddlBrandId3.SelectedValue
				: null,
			tbBrandId3DefaultSetting.Text.Trim(),
			cbBrandId3DefaultSetting.Checked);

		// ブランドID4
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_BRAND_ID4,
			cbBrandId4HasDefault.Checked
				? ddlBrandId4.SelectedValue
				: null,
			tbBrandId4DefaultSetting.Text.Trim(),
			cbBrandId4DefaultSetting.Checked);

		// ブランドID5
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_BRAND_ID5,
			cbBrandId5HasDefault.Checked
				? ddlBrandId5.SelectedValue
				: null,
			tbBrandId5DefaultSetting.Text.Trim(),
			cbBrandId5DefaultSetting.Checked);

		// 商品関連ID1（クロスセル）
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1,
			cbRelatedProductIdCsHasDefault.Checked
				? tbRelatedProductIdCs1.Text.Trim()
				: null,
			tbRelatedProductIdCs1DefaultSetting.Text.Trim(),
			cbRelatedProductIdCs1DefaultSetting.Checked);

		// 商品関連ID2（クロスセル）
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS2,
			cbRelatedProductIdCsHasDefault.Checked
				? tbRelatedProductIdCs2.Text.Trim()
				: null,
			tbRelatedProductIdCs1DefaultSetting.Text.Trim(),
			cbRelatedProductIdCs1DefaultSetting.Checked);

		// 商品関連ID3（クロスセル）
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS3,
			cbRelatedProductIdCsHasDefault.Checked
				? tbRelatedProductIdCs3.Text.Trim()
				: null,
			tbRelatedProductIdCs1DefaultSetting.Text.Trim(),
			cbRelatedProductIdCs1DefaultSetting.Checked);

		// 商品関連ID4（クロスセル）
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS4,
			cbRelatedProductIdCsHasDefault.Checked
				? tbRelatedProductIdCs4.Text.Trim()
				: null,
			tbRelatedProductIdCs1DefaultSetting.Text.Trim(),
			cbRelatedProductIdCs1DefaultSetting.Checked);

		// 商品関連ID5（クロスセル）
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS5,
			cbRelatedProductIdCsHasDefault.Checked
				? tbRelatedProductIdCs5.Text.Trim()
				: null,
			tbRelatedProductIdCs1DefaultSetting.Text.Trim(),
			cbRelatedProductIdCs1DefaultSetting.Checked);

		// 商品関連ID1（アップセル）
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1,
			cbRelatedProductIdUsHasDefault.Checked
				? tbRelatedProductIdUs1.Text.Trim()
				: null,
			tbRelatedProductIdUs1DefaultSetting.Text.Trim(),
			cbRelatedProductIdUs1DefaultSetting.Checked);

		// 商品関連ID2（アップセル）
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US2,
			cbRelatedProductIdUsHasDefault.Checked
				? tbRelatedProductIdUs2.Text.Trim()
				: null,
			tbRelatedProductIdUs1DefaultSetting.Text.Trim(),
			cbRelatedProductIdUs1DefaultSetting.Checked);

		// 商品関連ID3（アップセル）
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US3,
			cbRelatedProductIdUsHasDefault.Checked
				? tbRelatedProductIdUs3.Text.Trim()
				: null,
			tbRelatedProductIdUs1DefaultSetting.Text.Trim(),
			cbRelatedProductIdUs1DefaultSetting.Checked);

		// 商品関連ID4（アップセル）
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US4,
			cbRelatedProductIdUsHasDefault.Checked
				? tbRelatedProductIdUs4.Text.Trim()
				: null,
			tbRelatedProductIdUs1DefaultSetting.Text.Trim(),
			cbRelatedProductIdUs1DefaultSetting.Checked);

		// 商品関連ID5（アップセル）
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US5,
			cbRelatedProductIdUsHasDefault.Checked
				? tbRelatedProductIdUs5.Text.Trim()
				: null,
			tbRelatedProductIdUs1DefaultSetting.Text.Trim(),
			cbRelatedProductIdUs1DefaultSetting.Checked);

		// 商品画像
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_IMAGE_HEAD,
			null,
			string.Empty,
			false);

		// アイコン表示フラグ1
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_FLG1,
			cbIconFlg1HasDefault.Checked
				? cbIconFlg1.Checked
					? Constants.FLG_PRODUCT_ICON_ON
					: Constants.FLG_PRODUCT_ICON_OFF
				: null,
			tbIconFlg1DefaultSetting.Text.Trim(),
			cbIconFlg1DefaultSetting.Checked);

		// アイコン表示期限1
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_TERM_END1,
			cbIconFlg1HasDefault.Checked
				? cbIconFlg1.Checked
					? ucIconFlgTermEnd1.DateTimeString
					: null
				: null,
			tbIconFlg1DefaultSetting.Text.Trim(),
			cbIconFlg1DefaultSetting.Checked);

		// アイコン表示フラグ2
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_FLG2,
			cbIconFlg2HasDefault.Checked
				? cbIconFlg2.Checked
					? Constants.FLG_PRODUCT_ICON_ON
					: Constants.FLG_PRODUCT_ICON_OFF
				: null,
			tbIconFlg2DefaultSetting.Text.Trim(),
			cbIconFlg2DefaultSetting.Checked);

		// アイコン表示期限2
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_TERM_END2,
			cbIconFlg2HasDefault.Checked
				? cbIconFlg2.Checked
					? ucIconFlgTermEnd2.DateTimeString
					: null
				: null,
			tbIconFlg2DefaultSetting.Text.Trim(),
			cbIconFlg2DefaultSetting.Checked);

		// アイコン表示フラグ3
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_FLG3,
			cbIconFlg3HasDefault.Checked
				? cbIconFlg3.Checked
					? Constants.FLG_PRODUCT_ICON_ON
					: Constants.FLG_PRODUCT_ICON_OFF
				: null,
			tbIconFlg3DefaultSetting.Text.Trim(),
			cbIconFlg3DefaultSetting.Checked);

		// アイコン表示期限3
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_TERM_END3,
			cbIconFlg3HasDefault.Checked
				? cbIconFlg3.Checked
					? ucIconFlgTermEnd3.DateTimeString
					: null
				: null,
			tbIconFlg3DefaultSetting.Text.Trim(),
			cbIconFlg3DefaultSetting.Checked);

		// アイコン表示フラグ4
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_FLG4,
			cbIconFlg4HasDefault.Checked
				? cbIconFlg4.Checked
					? Constants.FLG_PRODUCT_ICON_ON
					: Constants.FLG_PRODUCT_ICON_OFF
				: null,
			tbIconFlg4DefaultSetting.Text.Trim(),
			cbIconFlg4DefaultSetting.Checked);

		// アイコン表示期限4
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_TERM_END4,
			cbIconFlg4HasDefault.Checked
				? cbIconFlg4.Checked
					? ucIconFlgTermEnd4.DateTimeString
					: null
				: null,
			tbIconFlg4DefaultSetting.Text.Trim(),
			cbIconFlg4DefaultSetting.Checked);

		// アイコン表示フラグ5
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_FLG5,
			cbIconFlg5HasDefault.Checked
				? cbIconFlg5.Checked
					? Constants.FLG_PRODUCT_ICON_ON
					: Constants.FLG_PRODUCT_ICON_OFF
				: null,
			tbIconFlg5DefaultSetting.Text.Trim(),
			cbIconFlg5DefaultSetting.Checked);

		// アイコン表示期限5
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_TERM_END5,
			cbIconFlg5HasDefault.Checked
				? cbIconFlg5.Checked
					? ucIconFlgTermEnd5.DateTimeString
					: null
				: null,
			tbIconFlg5DefaultSetting.Text.Trim(),
			cbIconFlg5DefaultSetting.Checked);

		// アイコン表示フラグ6
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_FLG6,
			cbIconFlg6HasDefault.Checked
				? cbIconFlg6.Checked
					? Constants.FLG_PRODUCT_ICON_ON
					: Constants.FLG_PRODUCT_ICON_OFF
				: null,
			tbIconFlg6DefaultSetting.Text.Trim(),
			cbIconFlg6DefaultSetting.Checked);

		// アイコン表示期限6
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_TERM_END6,
			cbIconFlg6HasDefault.Checked
				? cbIconFlg6.Checked
					? ucIconFlgTermEnd6.DateTimeString
					: null
				: null,
			tbIconFlg6DefaultSetting.Text.Trim(),
			cbIconFlg6DefaultSetting.Checked);

		// アイコン表示フラグ7
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_FLG7,
			cbIconFlg7HasDefault.Checked
				? cbIconFlg7.Checked
					? Constants.FLG_PRODUCT_ICON_ON
					: Constants.FLG_PRODUCT_ICON_OFF
							: null,
			tbIconFlg7DefaultSetting.Text.Trim(),
			cbIconFlg7DefaultSetting.Checked);

		// アイコン表示期限7
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_TERM_END7,
			cbIconFlg7HasDefault.Checked
				? cbIconFlg7.Checked
					? ucIconFlgTermEnd7.DateTimeString
					: null
				: null,
			tbIconFlg7DefaultSetting.Text.Trim(),
			cbIconFlg7DefaultSetting.Checked);

		// アイコン表示フラグ8
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_FLG8,
			cbIconFlg8HasDefault.Checked
				? cbIconFlg8.Checked
					? Constants.FLG_PRODUCT_ICON_ON
					: Constants.FLG_PRODUCT_ICON_OFF
						: null,
			tbIconFlg8DefaultSetting.Text.Trim(),
			cbIconFlg8DefaultSetting.Checked);

		// アイコン表示期限8
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_TERM_END8,
			cbIconFlg8HasDefault.Checked
				? cbIconFlg8.Checked
					? ucIconFlgTermEnd8.DateTimeString
					: null
				: null,
			tbIconFlg8DefaultSetting.Text.Trim(),
			cbIconFlg8DefaultSetting.Checked);

		// アイコン表示フラグ9
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_FLG9,
			cbIconFlg9HasDefault.Checked
				? cbIconFlg9.Checked
					? Constants.FLG_PRODUCT_ICON_ON
					: Constants.FLG_PRODUCT_ICON_OFF
				: null,
			tbIconFlg9DefaultSetting.Text.Trim(),
			cbIconFlg9DefaultSetting.Checked);

		// アイコン表示期限9
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_TERM_END9,
			cbIconFlg9HasDefault.Checked
				? cbIconFlg9.Checked
					? ucIconFlgTermEnd9.DateTimeString
					: null
				: null,
			tbIconFlg9DefaultSetting.Text.Trim(),
			cbIconFlg9DefaultSetting.Checked);

		// アイコン表示フラグ10
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_FLG10,
			cbIconFlg10HasDefault.Checked
				? cbIconFlg10.Checked
					? Constants.FLG_PRODUCT_ICON_ON
					: Constants.FLG_PRODUCT_ICON_OFF
				: null,
			tbIconFlg10DefaultSetting.Text.Trim(),
			cbIconFlg10DefaultSetting.Checked);

		// アイコン表示期限10
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_ICON_TERM_END10,
			cbIconFlg10HasDefault.Checked
				? cbIconFlg10.Checked
					? ucIconFlgTermEnd10.DateTimeString
					: null
				: null,
			tbIconFlg10DefaultSetting.Text.Trim(),
			cbIconFlg10DefaultSetting.Checked);

		// 配送間隔月利用不可
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING,
			null,
			tbLimitedFixedPurchaseKbn1SettingDefaultSetting.Text.Trim(),
			cbLimitedFixedPurchaseKbn1SettingDefaultSetting.Checked);

		// 配送間隔利用不可
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING,
			null,
			tbLimitedFixedPurchaseKbn3SettingDefaultSetting.Text.Trim(),
			cbLimitedFixedPurchaseKbn3SettingDefaultSetting.Checked);

		// 配送間隔週利用不可
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING,
			null,
			tbLimitedFixedPurchaseKbn4SettingDefaultSetting.Text.Trim(),
			cbLimitedFixedPurchaseKbn4SettingDefaultSetting.Checked);

		// Default setting flag display fixed purchase member
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG,
			cbDisplayFixedPurchaseMemberLimitHasDefault.Checked
				? cbDisplayFixedPurchaseMemberLimit.Checked
					? Constants.FLG_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG_ON
					: Constants.FLG_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG_OFF
				: null,
			tbDisplayFixedPurchaseMemberLimitDefaultSetting.Text.Trim(),
			cbDisplayFixedPurchaseMemberLimitDefaultSetting.Checked);

		// Default setting flag sell fixed purchase member
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG,
			cbBuyableFixedPurchaseMemberLimitHasDefault.Checked
				? cbBuyableFixedPurchaseMemberLimit.Checked
					? Constants.FLG_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG_ON
					: Constants.FLG_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG_OFF
				: null,
			tbBuyableFixedPurchaseMemberLimitDefaultSetting.Text.Trim(),
			cbBuyableFixedPurchaseMemberLimitDefaultSetting.Checked);

		// 重量
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM,
			cbProductWeightGramHasDefault.Checked
				? Constants.GLOBAL_OPTION_ENABLE
					? tbProductWeightGram.Text.Trim()
					: "0"
				: null,
			tbProductWeightGramDefaultSetting.Text.Trim(),
			cbProductWeightGramDefaultSetting.Checked);

		//For product fixed purchase next shipping product settings
		productDefaultSettingTable.Add(
			PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING,
			null,
			tbFixedPurchaseNextShippingSettingDefaultSetting.Text.Trim(),
			cbFixedPurchaseNextShippingSettingDefaultSetting.Checked);
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID,
			null,
			tbFixedPurchaseNextShippingSettingDefaultSetting.Text.Trim(),
			cbFixedPurchaseNextShippingSettingDefaultSetting.Checked);
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID,
			null,
			tbFixedPurchaseNextShippingSettingDefaultSetting.Text.Trim(),
			cbFixedPurchaseNextShippingSettingDefaultSetting.Checked);
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY,
			null,
			tbFixedPurchaseNextShippingSettingDefaultSetting.Text.Trim(),
			cbFixedPurchaseNextShippingSettingDefaultSetting.Checked);

		// 定期購入スキップ制限回数
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT,
			cbFixedPurchaseLimitedSkippedCountHasDefault.Checked
				? StringUtility.ToNull(tbFixedPurchaseLimitedSkippedCount.Text.Trim())
				: null,
			tbFixedPurchaseLimitedSkippedCountDefaultSetting.Text.Trim(),
			cbFixedPurchaseLimitedSkippedCountDefaultSetting.Checked);

		// 会員ランクのポイント加算チェック
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG,
			cbMemberRankPointExcludeDefaultSetting.Checked
				? Constants.FLG_PRODUCT_CHECK_MEMBER_RANK_POINT_EXCLUDE_FLG_VALID
				: Constants.FLG_PRODUCT_CHECK_MEMBER_RANK_POINT_EXCLUDE_FLG_INVALID,
		tbPointDefaultSetting.Text.Trim(),
			cbPointDefaultSetting.Checked);

		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_STOREPICKUP_FLG,
			cbStorePickupFlgHasDefault.Checked
				? cbStorePickupFlg.Checked
					? Constants.FLG_ON
					: Constants.FLG_OFF
				: null,
			tbStorePickupFlgDefaultSetting.Text.Trim(),
			cbStorePickupFlgDefaultSetting.Checked);

		// 配送料複数個無料フラグ
		productDefaultSettingTable.Add(
			Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG,
			cbExcludeFreeShippingHasDefault.Checked
				? cbExcludeFreeShipping.Checked
					? Constants.FLG_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG_VALID
					: Constants.FLG_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG_INVALID
				: null,
			tbExcludeFreeShippingDefaultSetting.Text.Trim(),
			cbExcludeFreeShippingDefaultSetting.Checked);

		return productDefaultSettingTable;
	}

	/// <summary>
	/// Get product variation default setting input
	/// </summary>
	/// <returns>Product variation default setting table</returns>
	private ProductDefaultSettingTable GetProductVariationDefaultSettingInput()
	{
		var productVariationDefalutSettingTable = new ProductDefaultSettingTable(Constants.TABLE_PRODUCTVARIATION);

		// 店舗ID
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_SHOP_ID,
			this.LoginOperatorShopId,
			string.Empty,
			true);

		// 商品ID(結合後)
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID,
			tbProductId.Text.Trim(),
			tbProductIdDefaultSetting.Text.Trim(),
			cbProductIdDefaultSetting.Checked);

		// バリエーション名1
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1,
			cbVariationName1HasDefault.Checked
				? tbVariationName1.Text.Trim()
				: null,
			tbVariationName1DefaultSetting.Text.Trim(),
			cbVariationName1DefaultSetting.Checked);

		// バリエーション名2
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2,
			cbVariationName2HasDefault.Checked
				? tbVariationName2.Text.Trim()
				: null,
			tbVariationName2DefaultSetting.Text.Trim(),
			cbVariationName2DefaultSetting.Checked);

		// バリエーション名3
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3,
			cbVariationName3HasDefault.Checked
				? tbVariationName3.Text.Trim()
				: null,
			tbVariationName3DefaultSetting.Text.Trim(),
			cbVariationName3DefaultSetting.Checked);

		// 商品カラーID（バリエーション単位）
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID,
			cbVariationColorImageHasDefault.Checked
				? ddlVariationColorImage.SelectedValue
				: null,
			tbVariationColorImageDefaultSetting.Text.Trim(),
			cbVariationColorImageDefaultSetting.Checked);

		// 表示順
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER,
			cbDisplayOrderHasDefault.Checked
				? tbDisplayOrder.Text.Trim()
				: null,
			tbDisplayOrderDefaultSetting.Text.Trim(),
			cbDisplayOrderDefaultSetting.Checked);

		// 販売価格
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_PRICE,
			cbVariationPriceHasDefault.Checked
				? tbVariationPrice.Text.Trim()
				: null,
			tbVariationPriceDefaultSetting.Text.Trim(),
			cbVariationPriceDefaultSetting.Checked);

		// 特別価格
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE,
			cbVariationSpecialPriceHasDefault.Checked
				? StringUtility.ToNull(tbVariationSpecialPrice.Text.Trim())
				: null,
			tbVariationSpecialPriceDefaultSetting.Text.Trim(),
			cbVariationSpecialPriceDefaultSetting.Checked);

		// 定期初回購入価格
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE,
			cbVariationFixedPurchaseFirsttimePriceHasDefault.Checked
				? StringUtility.ToNull(tbVariationFixedPurchaseFirsttimePrice.Text.Trim())
				: null,
			tbVariationFixedPurchaseFirsttimePriceDefaultSetting.Text.Trim(),
			cbVariationFixedPurchaseFirsttimePriceDefaultSetting.Checked);

		// 定期購入価格
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE,
			cbVariationFixedPurchasePriceHasDefault.Checked
				? StringUtility.ToNull(tbVariationFixedPurchasePrice.Text.Trim())
				: null,
			tbVariationFixedPurchasePriceDefaultSetting.Text.Trim(),
			cbVariationFixedPurchasePriceDefaultSetting.Checked);

		// 会員ランク価格
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE,
					null,
			tbVariationMemberRankPriceDefaultSetting.Text.Trim(),
			cbVariationMemberRankPriceDefaultSetting.Checked);

		// 商品バリエーション連携ID1
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1,
			cbVariationCooperationId1HasDefault.Checked
				? tbVariationCooperationId1.Text.Trim()
				: null,
			tbVariationCooperationId1DefaultSetting.Text.Trim(),
			cbVariationCooperationId1DefaultSetting.Checked);

		// 商品バリエーション連携ID2
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2,
			cbVariationCooperationId2HasDefault.Checked
				? tbVariationCooperationId2.Text.Trim()
				: null,
			tbVariationCooperationId2DefaultSetting.Text.Trim(),
			cbVariationCooperationId2DefaultSetting.Checked);

		// 商品バリエーション連携ID3
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3,
			cbVariationCooperationId3HasDefault.Checked
				? tbVariationCooperationId3.Text.Trim()
				: null,
			tbVariationCooperationId3DefaultSetting.Text.Trim(),
			cbVariationCooperationId3DefaultSetting.Checked);

		// 商品バリエーション連携ID4
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4,
			cbVariationCooperationId4HasDefault.Checked
				? tbVariationCooperationId4.Text.Trim()
				: null,
			tbVariationCooperationId4DefaultSetting.Text.Trim(),
			cbVariationCooperationId4DefaultSetting.Checked);

		// 商品バリエーション連携ID5
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5,
			cbVariationCooperationId5HasDefault.Checked
				? tbVariationCooperationId5.Text.Trim()
				: null,
			tbVariationCooperationId5DefaultSetting.Text.Trim(),
			cbVariationCooperationId5DefaultSetting.Checked);

		// 商品バリエーション連携ID6
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6,
			cbVariationCooperationId6HasDefault.Checked
				? tbVariationCooperationId6.Text.Trim()
				: null,
			tbVariationCooperationId6DefaultSetting.Text.Trim(),
			cbVariationCooperationId6DefaultSetting.Checked);

		// 商品バリエーション連携ID7
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7,
			cbVariationCooperationId7HasDefault.Checked
				? tbVariationCooperationId7.Text.Trim()
				: null,
			tbVariationCooperationId7DefaultSetting.Text.Trim(),
			cbVariationCooperationId7DefaultSetting.Checked);

		// 商品バリエーション連携ID8
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8,
			cbVariationCooperationId8HasDefault.Checked
				? tbVariationCooperationId8.Text.Trim()
				: null,
			tbVariationCooperationId8DefaultSetting.Text.Trim(),
			cbVariationCooperationId8DefaultSetting.Checked);

		// 商品バリエーション連携ID9
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9,
			cbVariationCooperationId9HasDefault.Checked
				? tbVariationCooperationId9.Text.Trim()
				: null,
			tbVariationCooperationId9DefaultSetting.Text.Trim(),
			cbVariationCooperationId9DefaultSetting.Checked);

		// 商品バリエーション連携ID10
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10,
			cbVariationCooperationId10HasDefault.Checked
				? tbVariationCooperationId10.Text.Trim()
				: null,
			tbVariationCooperationId10DefaultSetting.Text.Trim(),
			cbVariationCooperationId10DefaultSetting.Checked);

		// ＆mall連携予約商品フラグ
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG,
			cbVariationAndMallReservationFlgHasDefault.Checked
				? cbVariationAndMallReservationFlg.Checked
					? Constants.FLG_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG_RESERVATION
					: Constants.FLG_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG_COMMON
				: null,
			tbVariationAndMallReservationFlgDefaultSetting.Text.Trim(),
			cbVariationAndMallReservationFlgDefaultSetting.Checked);

		// ダウンロードURL
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL,
			cbVariationDownloadUrlHasDefault.Checked
				? tbVariationDownloadUrl.Text.Trim()
				: null,
			tbVariationDownloadUrlDefaultSetting.Text.Trim(),
			cbVariationDownloadUrlDefaultSetting.Checked);

		// モールバリエーションID1
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1,
			cbMallVariationId1HasDefault.Checked
				? tbMallVariationId1.Text.Trim()
				: null,
			tbMallVariationId1DefaultSetting.Text.Trim(),
			cbMallVariationId1DefaultSetting.Checked);

		// モールバリエーションID2
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2,
			cbMallVariationId1HasDefault.Checked
				? tbMallVariationId2.Text.Trim()
				: null,
			tbMallVariationId1DefaultSetting.Text.Trim(),
			cbMallVariationId1DefaultSetting.Checked);

		// モールバリエーション種別
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE,
			cbMallVariationId1HasDefault.Checked
				? tbMallVariationType.Text.Trim()
				: null,
			tbMallVariationId1DefaultSetting.Text.Trim(),
			cbMallVariationId1DefaultSetting.Checked);

		// バリエーション画像名ヘッダ
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD,
			null,
			string.Empty,
			false);

		// 重量（g）
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM,
			cbVariationWeightGramHasDefault.Checked
				? tbVariationWeightGram.Text.Trim()
				: null,
			tbVariationWeightGramDefaultSetting.Text.Trim(),
			cbVariationWeightGramDefaultSetting.Checked);

		// カート投入URL制限フラグ
		productVariationDefalutSettingTable.Add(
			Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG,
			cbValiationAddCartUrlLimitFlgHasDefault.Checked
				? cbValiationAddCartUrlLimitFlg.Checked
					? Constants.FLG_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG_VALID
					: Constants.FLG_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG_INVALID
				: null,
			tbAddCartUrlLimitFlgDefaultSetting.Text.Trim(),
			cbValiationAddCartUrlLimitFlgDefaultSetting.Checked);

		return productVariationDefalutSettingTable;
	}

	#region ~ボタンクリック系処理
	/// <summary>
	/// 一覧へ戻るボタン処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBackList_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateProductListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCT_SEARCH_INFO]));
	}

	/// <summary>
	/// 更新するボタンクリック（商品初期設定画面用）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateDefaultSetting_Click(object sender, EventArgs e)
	{
		// 入力チェック
		var productDefaultSetting = new ProductDefaultSetting
		{
			Product = GetProductDefaultSettingInput(),
			ProductVariation = GetProductVariationDefaultSettingInput()
		};

		// 入力チェック
		var hasError = (DisplayProductErrorMessages(productDefaultSetting)
			| DisplayProductRelationErrorMessages(productDefaultSetting)
			| DisplayProductIconErrorMessages(productDefaultSetting)
			| DisplayVariationDefaultSettingErrorMessages(productDefaultSetting));

		//エラーがあれば抜ける
		if (hasError)
		{
			this.HasErrorOnPostback = true;
			return;
		}

		// 商品初期設定情報更新
		productDefaultSetting.UpdateProductDefaultSetting(this.LoginOperatorShopId, this.LoginOperatorName);

		// 更新完了メッセージ表示
		divDisplayCompleteMessage.Visible = true;
	}

	/// <summary>
	/// リセットボタンをクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnResetIconFlgTermEnd_OnClick(object sender, EventArgs e)
	{
		var resetButton = ((Button)sender);
		var icon = (DateTimeInputControl)resetButton.FindControl(resetButton.CommandName);
		icon.SetDate(GetInitialIconFlgTermEnd(int.Parse(resetButton.CommandArgument)));
	}

	/// <summary>
	/// 商品付帯情報初期設定リピーターのデータバインド
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rProductOptionValueDefaultSetting_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var productOptionSettingKey = string.Format(
			"{0}{1}",
			Constants.HASH_KEY_PRODUCTOPTIONSETTING,
			e.Item.ItemIndex + 1);
		var defaultSettingDisplayField = GetDefaultSettingDisplayField(Constants.TABLE_PRODUCT, productOptionSettingKey);
		((CheckBox)e.Item.FindControl("cbProductOptionValueDefaultSetting")).Checked = defaultSettingDisplayField;
		var hasDefault = HasProductDefaultSettingFieldValue(productOptionSettingKey);
		((CheckBox)e.Item.FindControl("cbProductOptionValueHasDefault")).Checked = hasDefault;
		var text = GetProductDefaultSettingComment(productOptionSettingKey);
		((TextBox)e.Item.FindControl("tbProductOptionValueDefaultSetting")).Text = text;
	}
	#endregion

	/// <summary>
	/// アイコンフラグ有効期限初期値を取得
	/// </summary>
	/// <param name="index">アイコンのインデックス（1～10）</param>
	/// <returns>日付</returns>
	private DateTime GetInitialIconFlgTermEnd(int index)
	{
		return this.InitialIconFlgTermEnds[index - 1];
	}

	/// <summary>
	/// アイコンフラグ有効終了日取得
	/// </summary>
	/// <param name="index">アイコンのインデックス（1～10）</param>
	/// <returns>日付</returns>
	protected DateTime GetIconFlgTermEnd(int index)
	{
		var dateTime = GetDefaultDate(
			this.ProductDefaultSetting.Product,
			Constants.ICON_FLG_TERM_END_NAMES[index - 1],
			DateTime.Now.Date.AddYears(1));
		return dateTime.Value;
	}

	/// <summary>
	/// 商品基本情報のエラーメッセージ表示
	/// </summary>
	/// <param name="setting">Product default setting</param>
	/// <returns>メッセージ表示したか</returns>
	private bool DisplayProductErrorMessages(ProductDefaultSetting setting)
	{
		var errorMessage = setting.ValidateProduct();
		lbProductErrorMessages.Text = GetEncodedHtmlDisplayMessage(errorMessage);
		tblProductErrorMessages.Visible = (string.IsNullOrEmpty(errorMessage) == false);

		return tblProductErrorMessages.Visible;
	}

	/// <summary>
	/// 商品に関連する情報のエラーメッセージ表示
	/// </summary>
	/// <param name="setting">Product default setting</param>
	/// <returns>メッセージ表示したか</returns>
	private bool DisplayProductRelationErrorMessages(ProductDefaultSetting setting)
	{
		var errorMessage = setting.ValidateProductRelations();
		lbProductRelationErrorMessages.Text = GetEncodedHtmlDisplayMessage(errorMessage);
		lbProductRelationErrorMessages.Visible = (string.IsNullOrEmpty(errorMessage) == false);

		return lbProductRelationErrorMessages.Visible;
	}

	/// <summary>
	/// 商品キャンペーンアイコンのエラーメッセージ表示
	/// </summary>
	/// <param name="setting">Product default setting</param>
	/// <returns>メッセージ表示したか</returns>
	private bool DisplayProductIconErrorMessages(ProductDefaultSetting setting)
	{
		var errorMessage = setting.ValidateProductIcons();
		lbIconErrorMessages.Text = GetEncodedHtmlDisplayMessage(errorMessage);
		tblIconErrorMessages.Visible = (string.IsNullOrEmpty(errorMessage) == false);

		return tblIconErrorMessages.Visible;
	}

	/// <summary>
	/// 商品バリエーション初期設定のエラーメッセージ表示
	/// </summary>
	/// <param name="setting">Product default setting</param>
	/// <returns>メッセージ表示したか</returns>
	private bool DisplayVariationDefaultSettingErrorMessages(ProductDefaultSetting setting)
	{
		var errorMessage = setting.ValidateProductVariation();
		lbVariationErrorMessages.Text = GetEncodedHtmlDisplayMessage(errorMessage);
		tblVariationErrorMessages.Visible = (string.IsNullOrEmpty(errorMessage) == false);

		return tblVariationErrorMessages.Visible;
	}

	/// <summary>
	/// Get edit mode by default settingf ield value
	/// </summary>
	/// <param name="fieldName">Field name</param>
	/// <returns>Edit mode (Text, HTML)</returns>
	protected string GetEditModeByDefaultSettingFieldValue(string fieldName)
	{
		return ((GetProductDefaultSettingFieldValue(fieldName) != Constants.FLG_PRODUCT_DESC_DETAIL_HTML)
			? Constants.FLG_PRODUCT_DESC_DETAIL_TEXT
			: Constants.FLG_PRODUCT_DESC_DETAIL_HTML);
	}

	/// <summary>
	/// Is show fixed purchase point setting
	/// </summary>
	/// <param name="fieldName">Field name</param>
	/// <returns>True: Is show fixed purchase point setting</returns>
	protected bool IsShowFixedPurchasePointSetting(string fieldName)
	{
		var settingValue = GetProductDefaultSettingFieldValue(fieldName);
		return ((settingValue == Constants.FLG_PRODUCT_POINT_KBN1_NUM)
			|| (settingValue == Constants.FLG_PRODUCT_POINT_KBN1_RATE));
	}

	/// <summary>
	/// Get stock management kbn setting
	/// </summary>
	/// <param name="fieldName">Field name</param>
	/// <returns>Stock management kbn setting</returns>
	protected string GetStockManagementKbnSetting(string fieldName)
	{
		var settingValue = GetProductDefaultSettingFieldValue(fieldName);
		return (string.IsNullOrEmpty(settingValue)
			? Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED
			: settingValue);
	}

	/// <summary>
	/// Get fixed purchase flag setting
	/// </summary>
	/// <param name="fieldName">Field name</param>
	/// <returns>Fixed purchase flag setting</returns>
	protected string GetFixedPurchaseFlgSetting(string fieldName)
	{
		var settingValue = GetProductDefaultSettingFieldValue(fieldName);
		return (string.IsNullOrEmpty(settingValue)
			? Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID
			: settingValue);
	}

	/// <summary>
	/// Get subscription box flag setting
	/// </summary>
	/// <param name="fieldName">Field name</param>
	/// <returns>Subscription box flag setting</returns>
	protected string GetSubscriptionBoxFlgSetting(string fieldName)
	{
		var settingValue = GetProductDefaultSettingFieldValue(fieldName);
		return (string.IsNullOrEmpty(settingValue)
			? Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID
			: settingValue);
	}

	/// <summary>
	/// Get gift flag setting
	/// </summary>
	/// <param name="fieldName">Field name</param>
	/// <returns>Gift flag setting</returns>
	protected string GetGiftFlgSetting(string fieldName)
	{
		var settingValue = GetProductDefaultSettingFieldValue(fieldName);
		return (string.IsNullOrEmpty(settingValue)
			? Constants.FLG_PRODUCT_GIFT_FLG_INVALID
			: settingValue);
	}

	/// <summary>
	/// Get select variation kbn setting
	/// </summary>
	/// <param name="fieldName">Field name</param>
	/// <returns>Select variation kbn setting</returns>
	protected string GetSelectVariationKbnSetting(string fieldName)
	{
		var settingValue = GetProductDefaultSettingFieldValue(fieldName);
		return (string.IsNullOrEmpty(settingValue)
			? Constants.SelectVariationKbn.STANDARD.ToString()
			: settingValue);
	}

	/// <summary>
	/// Get product type setting
	/// </summary>
	/// <param name="fieldName">Field name</param>
	/// <returns>Product type setting</returns>
	protected string GetProductTypeSetting(string fieldName)
	{
		var settingValue = GetProductDefaultSettingFieldValue(fieldName);
		return (string.IsNullOrEmpty(settingValue)
			? Constants.FLG_PRODUCT_PRODUCT_TYPE_PRODUCT
			: settingValue);
	}

	/// <summary>
	/// Is all display kbn setting
	/// </summary>
	/// <param name="fieldName">Field name</param>
	/// <returns>True: If setting all display</returns>
	protected bool IsAllDisplayKbnSetting(string fieldName)
	{
		var settingValue = GetProductDefaultSettingFieldValue(fieldName);
		if (string.IsNullOrEmpty(settingValue)) return true;

		return (settingValue == Constants.FLG_PRODUCT_DISPLAY_DISP_ALL);
	}

	/// <summary>
	/// Get product color list item
	/// </summary>
	/// <returns>List item</returns>
	protected ListItem[] GetProductColorListItem()
	{
		var results = this.ProductColors.Select(color => new ListItem(color.DispName, color.Id))
			.ToArray();
		return results;
	}

	/// <summary>
	/// 商品IDは既に「商品ID+バリエーションID」として使用されるかチェック
	/// </summary>
	/// <param name="shopId">商品の店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckProductIdIsUsedAsVariationId(string shopId, string productId)
	{
		var errorMessage = string.Empty;
		var product = new ProductService();

		if (product.IsExistingProduct(shopId, productId) == false)
		{
			var isUsed = product.CheckProductIdIsUsedAsVariationId(shopId, productId);
			if (isUsed)
			{
				errorMessage =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_ID_IS_USED_AS_VARIATION_ID_ERROR);
			}
		}

		return errorMessage;
	}

	#region プロパティ
	/// <summary>商品カラーリスト</summary>
	protected List<ProductColor> ProductColors
	{
		get { return (List<ProductColor>)ViewState["ProductColors"]; }
		set { ViewState["ProductColors"] = value; }
	}
	/// <summary>キャンペーンアイコン有効期限初期値</summary>
	private DateTime[] InitialIconFlgTermEnds
	{
		get { return (DateTime[])ViewState["InitialIconFlgTermEnds"]; }
		set { ViewState["InitialIconFlgTermEnds"] = value; }
	}
	/// <summary>ポストバック時、エラーが発生したか</summary>
	protected bool HasErrorOnPostback
	{
		get { return (bool)ViewState["HasErrorOnPostback"]; }
		set { ViewState["HasErrorOnPostback"] = value; }
	}
	#endregion
}
