/*
=========================================================================================================
  Module      : 商品詳細モーダル画面 (ProductDetailModal.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
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
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.App.Common.UserProductArrivalMail;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.ContentsLog;
using w2.Domain.Product;
using w2.Domain.SetPromotion;
using w2.Domain.SubscriptionBox;

public partial class Form_Common_Product_ProductDetailModal : ProductUserControl
{
	/// <summary>バリエーション未選択によるエラーメッセージ</summary>
	protected string MESSAGE_ERROR_VARIATION_UNSELECTED = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_VARIATION_UNSELECTED);
	/// <summary>オプション未選択によるエラーメッセージ</summary>
	protected string MESSAGE_ERROR_OPTION_UNSELECTED = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_OPTION_UNSELECTED);
	/// <summary>JAFログイン連携：ログイン必須エラー</summary>
	protected string MESSAGE_ERROR_JAF_NEEDS_LOGIN_ERROR = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_JAF_NEEDS_LOGIN_ERROR);
	/// <summary>JAFログイン連携：会員登録時の詳細説明文・注意事項</summary>
	protected string MESSAGE_ERROR_JAF_REGISTER_DESCRIPTION = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_JAF_REGISTER_DESCRIPTION);
	/// <summary>ローディングアニメーションタイプ：上部</summary>
	protected const string LOADING_TYPE_UPPER = "Upper";
	/// <summary>ローディングアニメーションタイプ：下部</summary>
	protected const string LOADING_TYPE_LOWER = "Lower";

	#region ラップ済みコントロール宣言
	protected WrappedDropDownList WddlVariationSelect1 { get { return GetWrappedControl<WrappedDropDownList>("ddlVariationSelect1"); } }
	protected WrappedDropDownList WddlVariationSelect2 { get { return GetWrappedControl<WrappedDropDownList>("ddlVariationSelect2"); } }
	protected WrappedDropDownList WddlVariationSelect { get { return GetWrappedControl<WrappedDropDownList>("ddlVariationSelect"); } }
	protected WrappedRepeater WrVariationSingleList { get { return GetWrappedControl<WrappedRepeater>("rVariationSingleList"); } }
	protected WrappedRepeater WrVariationPluralY { get { return GetWrappedControl<WrappedRepeater>("rVariationPluralY"); } } // ※下位互換用のため削除禁止
	protected WrappedRepeater WrAddCartVariationList { get { return GetWrappedControl<WrappedRepeater>("rAddCartVariationList"); } }
	WrappedRepeater WrVariationPluralX { get { return GetWrappedControl<WrappedRepeater>("rVariationPluralX"); } }
	WrappedRepeater WrVariationMatrixY { get { return GetWrappedControl<WrappedRepeater>("rVariationMatrixY"); } }
	protected WrappedHiddenField WhfShowOutOfStockCrossSellList { get { return GetWrappedControl<WrappedHiddenField>("hfShowOutOfStockCrossSellList", Constants.SHOW_OUT_OF_STOCK_ITEMS.ToString()); } }
	protected WrappedHiddenField WhfShowOutOfStockUpSellList { get { return GetWrappedControl<WrappedHiddenField>("hfShowOutOfStockUpSellList", Constants.SHOW_OUT_OF_STOCK_ITEMS.ToString()); } }
	protected WrappedHiddenField WhfVariationSelectedIndex { get { return GetWrappedControl<WrappedHiddenField>("hfVariationSelectedIndex", string.Empty); } }
	protected WrappedLinkButton WlbRequestArrivalMail2 { get { return GetWrappedControl<WrappedLinkButton>("lbRequestArrivalMail2"); } }
	protected WrappedButton WlbRequestReleaseMail2 { get { return GetWrappedControl<WrappedButton>("lbRequestReleaseMail2"); } }
	protected WrappedButton WlbRequestResaleMail2 { get { return GetWrappedControl<WrappedButton>("lbRequestResaleMail2"); } }
	protected WrappedHiddenField WhIsSelectingVariationExist { get { return GetWrappedControl<WrappedHiddenField>("hIsSelectingVariationExist"); } }
	protected WrappedHiddenField WhfSubscriptionBoxDisplayName { get { return GetWrappedControl<WrappedHiddenField>("hfSubscriptionBoxDisplayName"); } }
	protected WrappedDropDownList WddlSubscriptionBox { get { return GetWrappedControl<WrappedDropDownList>("ddlSubscriptionBox"); } }
	protected WrappedLinkButton WlbCartAdd { get { return GetWrappedControl<WrappedLinkButton>("lbCartAdd"); } }
	protected WrappedLinkButton WlbCartAddFixedPurchase { get { return GetWrappedControl<WrappedLinkButton>("lbCartAddFixedPurchase"); } }
	protected WrappedLinkButton WlbCartAddSubscriptionBox { get { return GetWrappedControl<WrappedLinkButton>("lbCartAddSubscriptionBox"); } }
	protected WrappedLinkButton WlbCartAddForGift { get { return GetWrappedControl<WrappedLinkButton>("lbCartAddForGift"); } }
	protected WrappedLiteral WlCombinationErrorMessage { get { return GetWrappedControl<WrappedLiteral>("lCombinationErrorMessage"); } }
	#endregion

	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Init(object sender, System.EventArgs e)
	{
		if (string.IsNullOrEmpty(this.ProductIdForModal))
		{
			SetDummyProductId();
		}

		this.ProductId = this.ProductIdForModal;

		// 商品データを画面に設定
		SetProductDataForDisplay();

		// 画面全体をデータバインド
		DataBindChildren();
	}

	/// <summary>
	/// ページ表示終了時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_LoadComplete(object sender, EventArgs e)
	{
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && (this.SubscriptionBoxFlg != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID))
		{
			// 頒布会の価格データを画面に設定
			SetSubscriptionBoxPriceForDisplay();
		}
	}

	/// <summary>
	/// モーダル初期化
	/// </summary>
	public void Initialize()
	{
		// 商品データを画面に設定
		this.VariationSelected = false;
		SetProductDataForDisplay();
		DataBindChildren();
	}

	/// <summary>
	/// ダミー商品ID設定
	/// </summary>
	private void SetDummyProductId()
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("Product", "GetDummyProductId"))
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, this.MemberRankId },
				{ Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG, this.UserFixedPurchaseMemberFlg },
			};

			var dv = statement.SelectSingleStatementWithOC(accessor, ht);
			var dummyProductId = dv.Count > 0
				? (string)dv[0][Constants.FIELD_PRODUCT_PRODUCT_ID]
				: string.Empty;

			this.ProductIdForModal = dummyProductId;
		}
	}

	/// <summary>
	/// 商品のズーム画像用のURLを生成する
	/// </summary>
	/// <returns>商品のズーム画像用のURL</returns>
	protected string CreateUrlForProductZoomImage()
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_PRODUCT_ZOOM_IMAGE)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, this.ProductId)
			.AddParam(
				"ihead",
				(string)this.ProductMaster[this.VariationSelected
					? Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD
					: Constants.FIELD_PRODUCT_IMAGE_HEAD])
			.AddParam(Constants.REQUEST_KEY_SHOP_ID, this.ShopId)
			.AddParam("width", "900")
			.AddParam("height", "650");
		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// 商品データ設定
	/// </summary>
	private void SetProductDataForDisplay()
	{
		var dvProduct = ProductCommon.GetProductInfo(this.ShopId, this.ProductId, this.MemberRankId, this.UserFixedPurchaseMemberFlg);

		if (dvProduct.Count == 0)
		{
			// 商品が見つからない場合はエラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		this.ProductMaster = dvProduct[0];

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 翻訳情報設定
			var productTranslationSettings = NameTranslationCommon.GetProductAndVariationTranslationSettingsByProductId(
				this.ProductId,
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);

			var stockMessageTranslationSettings = (this.ProductMaster[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF] != DBNull.Value)
				? NameTranslationCommon.GetProductStockMessageTranslationSettings(
					(string)this.ProductMaster[Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID],
					RegionManager.GetInstance().Region.LanguageCode,
					RegionManager.GetInstance().Region.LanguageLocaleId)
				: null;
			dvProduct = NameTranslationCommon.SetProductAndVariationTranslationDataToDataView(dvProduct, productTranslationSettings);
			this.ProductMaster = dvProduct[0];
			this.ProductMaster = NameTranslationCommon.SetProductStockMessageTranslationData(this.ProductMaster, stockMessageTranslationSettings);
		}

		this.ProductOptionSettingList = new ProductOptionSettingList((string)dvProduct[0][Constants.FIELD_PRODUCT_PRODUCT_OPTION_SETTINGS]);
		this.ProductName = (string)this.ProductMaster[Constants.FIELD_PRODUCT_NAME];
		this.HasVariation = HasVariation(this.ProductMaster);
		this.CanAddCart = Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED
			&& ((string)dvProduct[0][Constants.FIELD_PRODUCT_GIFT_FLG] != Constants.FLG_PRODUCT_GIFT_FLG_INVALID)
			|| (((string)this.ProductMaster[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY)
				&& ((string)dvProduct[0][Constants.FIELD_PRODUCT_GIFT_FLG] != Constants.FLG_PRODUCT_GIFT_FLG_ONLY));
		this.CanFixedPurchase = Constants.FIXEDPURCHASE_OPTION_ENABLED
			&& ((string)this.ProductMaster[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)
			&& ((string)dvProduct[0]["shipping_" + Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG] == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID);
		this.CanGiftOrder = (Constants.GIFTORDER_OPTION_ENABLED
			&& ((string)dvProduct[0][Constants.FIELD_PRODUCT_GIFT_FLG] != Constants.FLG_PRODUCT_GIFT_FLG_INVALID)
			&& (Constants.CART_LIST_LP_OPTION == false)
			&& (Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED == false));
		this.HasStockMessage = (this.ProductMaster[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF] != DBNull.Value);
		this.SelectVariationKbn = SetVariationKbn();
		this.DisplaySell = ((string)this.ProductMaster[Constants.FIELD_PRODUCT_DISPLAY_SELL_FLG] == Constants.FLG_PRODUCT_DISPLAY_SELL_FLG_DISP);
		this.IsUserFixedPurchaseAble = this.Process.CheckFixedPurchaseLimitedUserLevel(this.ShopId, this.ProductId) == false;
		this.SubscriptionBoxFlg = (string)this.ProductMaster[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG];
		this.IsStockManaged = ((string)this.ProductMaster[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED);

		// 商品バリエーションがある場合，ない場合の処理
		if (this.HasVariation)
		{
			this.ProductVariationMasterList = dvProduct;

			// 初期表示 かつ バリエーションが1つの時選択済みにするオプションがオン かつ バリエーションが1つの場合
			if (!IsPostBack
				&& string.IsNullOrEmpty(this.VariationId)
				&& (this.SelectVariationKbn == Constants.SelectVariationKbn.PANEL)
				&& (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIX)
				|| (Constants.PRODUCTDETAIL_VARIATION_SINGLE_SELECTED
					&& (this.ProductVariationMasterList.Count == 1)))
			{
				// バリエーションをセット
				this.VariationId = (string)this.ProductMaster[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID];
				this.VariationSelected = true;
			}

			// 商品バリエーション選択方法別に表示制御
			switch (this.SelectVariationKbn)
			{
				case Constants.SelectVariationKbn.STANDARD:
				case Constants.SelectVariationKbn.DROPDOWNLIST:
					SetVariationSelectForDropDownList();
					break;

				case Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST:
					SetVariationIdForVariationName();
					SetVariationSelectForDoubleDropDownList();
					break;

				case Constants.SelectVariationKbn.MATRIX:
				case Constants.SelectVariationKbn.MATRIXANDMESSAGE:
					SetVariationSelectForMatrix();
					break;

				case Constants.SelectVariationKbn.PANEL:
					SetVariationForPanel();
					SetVariationIdForPanelVariationName();
					break;
			}

			// 商品バリエーション表示名1・2の画像リスト設定
			SetProductVariationImageList();
		}
		else
		{
			// 商品バリエーションID(商品バリエーションが無い場合は=商品ID)
			this.VariationId = (string)this.ProductMaster[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID];

			// バリエーション選択状態
			this.VariationSelected = true;
		}

		// 頒布会申し込みボタンを表示する(定期が有効なユーザーのみ)
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && this.IsUserFixedPurchaseAble)
		{
			this.IsSubscriptionBoxOnly = (this.SubscriptionBoxFlg == Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_ONLY);
			var isSubscriptionBoxValid = (this.SubscriptionBoxFlg != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID)
				&& ((string)this.ProductMaster[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID);
			if (isSubscriptionBoxValid)
			{
				var subscriptionBoxes = DataCacheControllerFacade.GetSubscriptionBoxCacheController().GetSubscriptionBoxesByProductId(this.ShopId, this.ProductId, this.VariationId);
				var subscriptionBoxedForDisplay = new List<SubscriptionBoxModel>();
				foreach (var subscriptionBox in subscriptionBoxes)
				{
					// フロントでの商品変更可否がFALSEの時
					if (subscriptionBox.ItemsChangeableByUser == Constants.FLG_SUBSCRIPTIONBOX_ITEMS_CHANGEABLE_BY_USER_FALSE)
					{
						// 回数指定 期間指定
						if (subscriptionBox.OrderItemDeterminationType == Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_NUMBER_TIME)
						{
							foreach (var defaultProduct in subscriptionBox.DefaultOrderProducts)
							{
								if (defaultProduct.Count != 1) continue;
								if (defaultProduct.VariationId != this.VariationId) continue;
								foreach (var selectProduct in subscriptionBox.SelectableProducts)
								{
									if ((selectProduct.VariationId
											== this.VariationId)
										&& ((selectProduct.SelectableSince == null)
											|| (selectProduct.SelectableSince <= DateTime.Now)
											&& ((selectProduct.SelectableUntil == null)
												|| (DateTime.Now < selectProduct.SelectableUntil))))
									{
										subscriptionBoxedForDisplay.Add(subscriptionBox);
									}
								}
							}
						}
						else
						{
							foreach (var product in subscriptionBox.DefaultOrderProducts)
							{
								if ((product.VariationId == this.VariationId)
									&& ((product.TermSince <= DateTime.Now)
										&& (DateTime.Now < product.TermUntil)))
								{
									subscriptionBoxedForDisplay.Add(subscriptionBox);
								}
							}
						}
					}
					else
					{
						foreach (var selectProduct in subscriptionBox.SelectableProducts)
						{
							if ((selectProduct.VariationId == this.VariationId)
								&& ((selectProduct.SelectableSince == null)
									|| ((selectProduct.SelectableSince <= DateTime.Now)
										&& (DateTime.Now < selectProduct.SelectableUntil))))
							{
								subscriptionBoxedForDisplay.Add(subscriptionBox);
							}
						}
					}
				}
				this.WddlSubscriptionBox.Visible = (subscriptionBoxedForDisplay.Count > 1);
				this.WddlSubscriptionBox.DataSource = subscriptionBoxedForDisplay;
				this.WddlSubscriptionBox.DataBind();
				if (subscriptionBoxedForDisplay.Count > 0)
				{
					if (subscriptionBoxedForDisplay.Count == 1)
					{
						this.WhfSubscriptionBoxDisplayName.Value = subscriptionBoxedForDisplay.First().DisplayName;
					}
					this.WddlSubscriptionBox.SelectedIndex = 0;
					this.IsSubscriptionBoxValid = true;
				}
				else
				{
					this.IsSubscriptionBoxValid = false;
				}
			}
			else
			{
				WddlSubscriptionBox.Visible = this.IsSubscriptionBoxValid = false;
			}
		}
		
		// 表示期間チェック（非表示の場合エラーページへ）
		int iDispFlg = (int)this.ProductMaster["disp_flg"];
		if ((iDispFlg == 0) && (this.IsPreview == false))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_UNDISP);
			// フレンドリーURL利用時は494を捕捉できないのため、302→404転送とする
			if (Constants.FRIENDLY_URL_ENABLED)
			{
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
			else
			{
				Response.StatusCode = 404;
				Response.End();
			}
		}

		// 閲覧会員ランクチェック
		string strDisplayMemberRank = (string)this.ProductMaster[Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK];

		if (MemberRankOptionUtility.CheckMemberRankPermission(this.MemberRankId, strDisplayMemberRank) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_DISPLAY_MEMBER_RANK).Replace("@@ 1 @@", MemberRankOptionUtility.GetMemberRankName(strDisplayMemberRank));
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// 商品履歴オブジェクトが生成されている場合商品表示履歴追加
		if (Session[Constants.SESSION_KEY_PRODUCTHISTORY_LIST] == null)
		{
			Session[Constants.SESSION_KEY_PRODUCTHISTORY_LIST] = new ProductHistoryObject();
		}
		ProductHistoryObject ph = (ProductHistoryObject)Session[Constants.SESSION_KEY_PRODUCTHISTORY_LIST];
		ph.Add(this.ProductMaster);

		// 販売期間チェック && 在庫有無チェック（カートボタン非表示・在庫文言非表示、文言表示）
		var productCount = 1;
		// バリエーション有り && バリエーション未選択
		bool blStockFlg = false;
		if (this.HasVariation && (this.VariationSelected == false))
		{
			blStockFlg = CheckProductStockBuyable(this.ProductVariationMasterList, productCount);   // 在庫有無チェック
		}
		// バリエーションなし || バリエーション選択済み
		else
		{
			blStockFlg = CheckProductStockBuyable(this.ProductMaster, productCount); // 在庫有無チェック
		}

		// 購入会員ランク・販売期間・在庫有無チェック
		string strErrorMessage = CheckBuyableMemberRank(this.ProductMaster, blStockFlg);

		// 販売可能数量のチェック
		if (string.IsNullOrEmpty(strErrorMessage))
		{
			strErrorMessage = GetMaxSellQuantityError(
				ShopId,
				ProductId,
				VariationId,
				productCount
			);
		}

		// バリエーション選択状況に応じて「商品名(＋バリエーション名)」を置換
		var productName = ((this.HasVariation) && (this.VariationSelected == false))
			? (string)this.ProductMaster[Constants.FIELD_PRODUCT_NAME]
			: ProductCommon.CreateProductJointName(this.ProductMaster);

		// Check buyable fixed purchase member
		var errorMessage = CheckBuyableFixedPurchaseMember(dvProduct[0]);
		if (string.IsNullOrEmpty(errorMessage) == false) this.ErrorMessageFixedPurchaseMember = errorMessage.Replace("@@ 1 @@", productName);
		// 購入可能判定
		if ((strErrorMessage != "") || (string.IsNullOrEmpty(errorMessage) == false))
		{
			this.Buyable = false;
			// バリエーション選択状況に応じて、エラーメッセージの商品名をバリメーション名に置換
			if (this.HasVariation && this.VariationSelected)
			{
				var variationName = CreateVariationName(
					this.ProductMaster,
					string.Empty,
					string.Empty,
					Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION);

				this.AlertMessage = strErrorMessage.Replace(
					this.ProductName,
					variationName);
			}
			else
			{
				this.AlertMessage = strErrorMessage;
			}
		}
		else
		{
			this.Buyable = true;
			this.AlertMessage = "";
		}

		if (IsSelectingVariationExist == false)
		{
			var variationName = (string)this.ProductMaster[Constants.FIELD_PRODUCT_NAME] + ProductPage.CreateVariationName(this.SelectedVariationName1, this.SelectedVariationName2, this.SelectedVariationName3);
			this.AlertMessageVariationNotExist = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OF_DOES_NOT_EXIST).Replace("@@ 1 @@", variationName);
		}

		// セットプロモーション情報
		this.SetPromotions = DataCacheControllerFacade.GetSetPromotionCacheController().GetSetPromotionByProduct(
			this.ProductMaster,
			(this.HasVariation && this.VariationSelected),
			this.MemberRankId,
			(this.IsPc ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE),
			this.LoginUserHitTargetListIds);

		// 入荷通知メール区分取得
		this.ArrivalMailKbn = UserProductArrivalMailCommon.GetArrivalMailKbn(this.HasVariation, this.VariationSelected, this.ProductVariationMasterList, this.ProductMaster);

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();

			// 商品アップセルデータ取得（データバインドは画面全体で）
			this.ProductUpSellList = ProductCommon.GetProductUpSellProducts(this.ProductMaster, sqlAccessor, (this.WhfShowOutOfStockUpSellList.Value.ToLower() == "true"), this.LoginUserId);

			// 商品クロスセルデータ取得（データバインドは画面全体で）
			this.ProductCrossSellList = ProductCommon.GetProductCrossSellProducts(this.ProductMaster, sqlAccessor, (this.WhfShowOutOfStockCrossSellList.Value.ToLower() == "true"), this.LoginUserId);

			// 商品名の翻訳名称取得
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				var productUpSellList = this.ProductUpSellList.Cast<DataRowView>().Select(drv => new ProductModel(drv)).ToArray();
				this.ProductUpSellList = (DataView)NameTranslationCommon.Translate(this.ProductUpSellList, productUpSellList);

				var productCrossSellList = this.ProductCrossSellList.Cast<DataRowView>().Select(drv => new ProductModel(drv)).ToArray();
				this.ProductCrossSellList = (DataView)NameTranslationCommon.Translate(this.ProductCrossSellList, productCrossSellList);
			}
		}

		// サブ画像設定取得（存在するもののみ）
		this.ProductSubImageList = new List<DataRowView>();
		{
			// サブ画像設定取得
			DataView dvProductSubImageSettings = ProductCommon.GetProductSubImageSettingList(this.ShopId);

			List<DataRowView> lProductSubImageListTmp = new List<DataRowView>();
			foreach (DataRowView drvProductSubImage in dvProductSubImageSettings)
			{
				if (CheckProductSubImageExist(this.ProductMaster, (int)drvProductSubImage[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO]))
				{
					lProductSubImageListTmp.Add(drvProductSubImage);
				}
			}
			// メイン・サブ画像をリストへ追加
			if (lProductSubImageListTmp.Count != 0)
			{
				DataRowView drvMainImage = dvProductSubImageSettings.AddNew();
				drvMainImage[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NO] = Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO + 1; // 商品サブ画像Noの上限値よりも+1大きい商品サブ画像Noの時はメイン画像として扱う
				drvMainImage[Constants.FIELD_PRODUCTSUBIMAGESETTING_PRODUCT_SUB_IMAGE_NAME] = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT_DETAIL,
					Constants.VALUETEXT_PARAM_PRODUCT_DATA_SETTING_MESSAGE,
					Constants.VALUETEXT_PARAM_PRODUCT_DATA_MAIN_IMAGE);

				this.ProductSubImageList.Add(drvMainImage);
				foreach (DataRowView drvProductSubImage in lProductSubImageListTmp)
				{
					this.ProductSubImageList.Add(drvProductSubImage);
				}
			}
		}

		// Alert message limited payment
		if (String.IsNullOrEmpty(this.ProductMaster[Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS].ToString()) == false)
		{
			var limitedPaymentList = OrderCommon.GetLimitedPayments(
					StringUtility.ToEmpty(this.ProductMaster[Constants.FIELD_PRODUCT_SHOP_ID]),
					this.ProductMaster[Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS].ToString().Split(','))
				.Select(payment => payment.PaymentName).ToArray();

			this.LimitedPaymentMessages = (limitedPaymentList.Any() == false)
				? String.Empty
				: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCTDETAIL_LIMITED_PAYMENT_DISPLAYED)
					.Replace("@@ 1 @@", String.Join(", ", limitedPaymentList));
		}
	}

	/// <summary>
	/// バリエーション選択方法をセット
	/// </summary>
	private Constants.SelectVariationKbn SetVariationKbn()
	{
		// Check variation name 3 is exist
		this.IsVariationName3 = false;
		foreach (DataRowView data in this.ProductMaster.DataView)
		{
			if (string.IsNullOrEmpty((string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]) == false)
			{
				this.IsVariationName3 = true;
				break;
			}
		}

		foreach (Enum e in Enum.GetValues(typeof(Constants.SelectVariationKbn)))
		{
			if (e.ToString().ToUpper() == (string)this.ProductMaster[Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN])
			{
				if (IsVariationName3 && (((Constants.SelectVariationKbn)e == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST)
					|| ((Constants.SelectVariationKbn)e == Constants.SelectVariationKbn.MATRIX)
					|| ((Constants.SelectVariationKbn)e == Constants.SelectVariationKbn.MATRIXANDMESSAGE)))
				{
					return Constants.SelectVariationKbn.PANEL;
				}

				// スマートフォンサイトの場合、マトリックス表記は標準のドロップダウンに変換する
				//（マトリックス表記の場合、タッチパネルでは著しく操作しづらいと判断したため）
				if (SmartPhoneUtility.CheckSmartPhoneSite(this.Request.Path))
				{
					if ((Constants.SelectVariationKbn)e == Constants.SelectVariationKbn.MATRIX
						|| (Constants.SelectVariationKbn)e == Constants.SelectVariationKbn.MATRIXANDMESSAGE)
					{
						return Constants.SelectVariationKbn.STANDARD;
					}
				}

				return (Constants.SelectVariationKbn)e;
			}
		}

		// どれでもなければスタンダード
		return Constants.SelectVariationKbn.STANDARD;
	}

	/// <summary>
	/// バリエーション選択方法の表示制御：スタンダードまたはドロップダウンリスト
	/// </summary>
	/// <remarks>スタンダードとDDLの違い
	/// スタンダード：売切れ状態のときのみ「在庫なし」を表示
	/// ドロップダウンリスト：在庫状況に応じて在庫文言を表示</remarks>
	private void SetVariationSelectForDropDownList()
	{
		this.ProductValirationListItemCollection = new ListItemCollection();
		this.ProductValirationListItemCollection.Add(
			new ListItem(ReplaceTag("@@DispText.variation_name_list.unselected@@"), ""));

		// 在庫文言を取得
		var productVariationStockInfos = new ProductService().GetProductVariationStockInfos(
			this.ShopId,
			this.ProductId,
			this.MemberRankId);
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			NameTranslationCommon.SetStockTranslationData(ref productVariationStockInfos, this.ProductId);
		}
		foreach (DataRowView drvProduct in this.ProductVariationMasterList)
		{
			string strProductStockMessage = "";
			if (this.SelectVariationKbn == Constants.SelectVariationKbn.DROPDOWNLIST)
			{
				var productVariationStockInfo =
					productVariationStockInfos.FirstOrDefault(
						info => info.VariationId == (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
				if (productVariationStockInfo != null)
				{
					strProductStockMessage = productVariationStockInfo.StockMessage;
				}

				strProductStockMessage = (strProductStockMessage != "") ? " - " + strProductStockMessage : "";
			}
			else if (this.SelectVariationKbn == Constants.SelectVariationKbn.STANDARD)
			{
				strProductStockMessage = CheckProductStockBuyable(drvProduct, 1)
					? ""
					: ReplaceTag("@@DispText.product_stock_message.none@@");
			}

			// ドロップダウン作成
			StringBuilder sbListItemtext = new StringBuilder();
			sbListItemtext.Append(CreateVariationName(drvProduct, "", "", Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION)).Append(" ").Append(strProductStockMessage);
			ListItem liVariation = new ListItem(sbListItemtext.ToString(), (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
			this.ProductValirationListItemCollection.Add(liVariation);

			// バリエーションの選択状態を保持
			SetProductVariationStatus(drvProduct, (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
		}
	}

	/// <summary>
	/// バリエーション選択方法の表示制御：ダブルドロップダウンリスト
	/// </summary>
	private void SetVariationSelectForDoubleDropDownList()
	{
		this.ProductValirationListItemCollection = new ListItemCollection
		{
			new ListItem(ReplaceTag("@@DispText.variation_name_list.unselected@@"), "")
		};
		this.ProductValirationListItemCollection2 = new ListItemCollection
		{
			new ListItem(ReplaceTag("@@DispText.variation_name_list.unselected@@"), "")
		};
		ListItem liVariation = null;

		foreach (DataRowView drvProduct in this.ProductVariationMasterList)
		{
			// ドロップダウンリスト１作成
			liVariation = new ListItem((string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]);
			if (this.ProductValirationListItemCollection.Contains(liVariation) == false)
			{
				this.ProductValirationListItemCollection.Add(liVariation);
			}

			// ドロップダウンリスト２作成
			if ((string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2] != "")
			{
				liVariation = new ListItem((string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]);
				if (this.ProductValirationListItemCollection2.Contains(liVariation) == false)
				{
					this.ProductValirationListItemCollection2.Add(liVariation);
				}
			}

			// バリエーションの選択状態を保持
			SetProductVariationStatus(drvProduct, (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
		}

		if (string.IsNullOrEmpty(this.VariationId) != false)
		{
			this.SelectedVariationName1 = (string)this.ProductMaster[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
			this.SelectedVariationName2 = (string)this.ProductMaster[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
		}

		// 選択された組み合わせのバリエーションがない場合、エラーメッセージ表示、カート投入ボタン非活性化
		VariationCombinationCheck();
	}

	/// <summary>
	/// バリエーション選択方法の表示制御：MATRIX
	/// </summary>
	private void SetVariationSelectForMatrix()
	{
		// バリエーション2軸チェック
		this.IsPluralVariation = false;
		foreach (DataRowView drv in this.ProductVariationMasterList)
		{
			if (((string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]).Length != 0)
			{
				this.IsPluralVariation = true;
				break;
			}
		}

		// 2軸の場合マトリクスリスト作成処理
		if (this.IsPluralVariation)
		{
			
			// 旧マトリクスリスト作成（V5.2以前）
			// ※下位互換のため、旧マトリクス表示処理は削除禁止
			{
				this.ProductValirationListItemCollection2 = new ListItemCollection();
				ListItem liVariation = null;
				foreach (DataRowView drvProduct in this.ProductVariationMasterList)
				{
					foreach (DataRowView drv in this.ProductVariationMasterList)
					{
						liVariation = new ListItem((string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2], "");
						if (this.ProductValirationListItemCollection2.Contains(liVariation) == false)
						{
							this.ProductValirationListItemCollection2.Add(liVariation);
						}
					}
				}
				this.WrVariationPluralX.DataSource = this.ProductValirationListItemCollection2;
				this.WrVariationPluralX.DataBind();
				this.WrVariationPluralY.DataSource = this.ProductVariationMasterList;
				this.WrVariationPluralY.DataBind();
			}

			
			// マトリクスリスト作成（V5.3以降）
			this.VariationName1List = new List<string>();
			this.VariationName2List = new List<string>();
			foreach (DataRowView drv in this.ProductVariationMasterList)
			{
				if (this.VariationName1List.Contains((string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]) == false)
				{
					this.VariationName1List.Add((string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]);
				}

				if (this.VariationName2List.Contains((string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]) == false)
				{
					this.VariationName2List.Add((string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]);
				}
			}
		}
		// 1軸の場合
		else
		{
			this.WrVariationSingleList.DataSource = this.ProductVariationMasterList;
			this.WrVariationSingleList.DataBind();
		}

		// バリエーションの選択状態を保持
		foreach (DataRowView drv in this.ProductVariationMasterList)
		{
			SetProductVariationStatus(drv, (string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
		}
	}

	/// <summary>
	/// パネルのバリエーション名に対してバリエーションIDを設定する
	/// </summary>
	private void SetVariationIdForPanelVariationName()
	{
		// 選択されたバリエーション名が1つ以上存在する場合
		if ((string.IsNullOrEmpty(this.SelectedVariationName1) == false)
			|| (string.IsNullOrEmpty(this.SelectedVariationName2) == false)
			|| (string.IsNullOrEmpty(this.SelectedVariationName3) == false))
		{
			this.VariationId = string.Empty;
		}
		var variationSelectedIndex = 0;

		// バリエーションIDが値を持っていない場合
		if (string.IsNullOrEmpty(this.VariationId))
		{
			foreach (DataRowView data in this.ProductVariationMasterList)
			{
				if ((this.SelectedVariationName1 == (string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1])
					&& (this.SelectedVariationName2 == (string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2])
					&& (this.SelectedVariationName3 == (string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]))
				{
					this.VariationId = (string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID];
					// バリエーション選択状態
					this.VariationSelected = true;
					break;
				}
				variationSelectedIndex++;
			}
			this.IsSelectingVariationExist = true;

			// バリエーション名1によって存在しないバリエーションを追加
			var newVariationMasterList = this.ProductVariationMasterList.ToTable();
			if ((string.IsNullOrEmpty(this.SelectedVariationName1) == false)
				&& string.IsNullOrEmpty(this.VariationId))
			{
				var variationAdd = newVariationMasterList.DefaultView.Cast<DataRowView>()
					.FirstOrDefault(row => ((string)row[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1] == this.SelectedVariationName1));
				this.VariationId = (variationAdd != null)
					? (string)variationAdd[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]
					: string.Empty;

				variationSelectedIndex = 0;
				foreach (DataRowView data in this.ProductVariationMasterList)
				{
					if (this.VariationId == (string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID])
					{
						data[Constants.FIELD_PRODUCTSTOCK_STOCK] = 0;
						break;
					}
					variationSelectedIndex++;
				}
				this.IsSelectingVariationExist = false;
			}
		}
		else
		{
			// バリエーションIDが値を持っている場合は画像を選択
			foreach (DataRowView data in this.ProductVariationMasterList)
			{
				if (this.VariationId == (string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID])
				{
					this.SelectedVariationName1 = (string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
					this.SelectedVariationName2 = (string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
					this.SelectedVariationName3 = (string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3];

					break;
				}
				variationSelectedIndex++;
			}

			this.IsSelectingVariationExist = true;
		}

		// カートに追加ボタンを有効に設定
		SetEnableAddCart();

		foreach (DataRowView data in this.ProductVariationMasterList)
		{
			// バリエーションの選択状態を保持
			SetProductVariationStatus(data, (string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
		}

		// Variation selected for SP
		this.WhfVariationSelectedIndex.Value = (this.VariationSelected == false) ? "0" : variationSelectedIndex.ToString();
	}

	/// <summary>
	/// パネルに対してバリエーションを設定する
	/// </summary>
	private void SetVariationForPanel()
	{
		// 既存のバリエーションIDが存在するかどうかを確認
		var isExistsVariationId = this.ProductVariationMasterList
			.Cast<DataRowView>()
			.Select(drv => (string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID])
			.Contains(StringUtility.ToEmpty(this.VariationId));

		// パネルに対してデフォルトの選択されたバリエーションを設定
		if (isExistsVariationId == false)
		{
			this.VariationId = StringUtility.ToEmpty(this.ProductVariationMasterList[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
		}

		// 製品バリエーション名のリストを作成
		this.ProductVariationName1List = new List<string>();
		this.ProductVariationName2List = new List<string>();
		this.ProductVariationName3List = new List<string>();

		if (string.IsNullOrEmpty(this.SelectedVariationName1))
		{
			foreach (DataRowView productVariation in this.ProductVariationMasterList)
			{
				if ((string)productVariation[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] == this.VariationId)
				{
					this.SelectedVariationName1 = (string)productVariation[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
					this.SelectedVariationName2 = (string)productVariation[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
					this.SelectedVariationName3 = (string)productVariation[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3];
					break;
				}
			}
		}

		// HACK:表示名が増えても対応出来るようにしたい（現状では増えるたびに処理を追加する必要がある）

		// 表示名1・2の項目設定
		foreach (DataRowView data in this.ProductVariationMasterList)
		{
			if (this.ProductVariationName1List.Contains((string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]) == false)
			{
				this.ProductVariationName1List.Add((string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]);
			}

			if ((this.ProductVariationName2List.Contains((string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]) == false)
				&& (string.IsNullOrEmpty((string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]) == false)
				&& ((string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1] == this.SelectedVariationName1))
			{
				this.ProductVariationName2List.Add((string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]);
			}
		}

		// 表示名2の選択値設定
		this.SelectedVariationName2 = (this.ProductVariationName2List.Any())
			? (this.ProductVariationName2List.Contains(this.SelectedVariationName2) == false)
				? this.ProductVariationName2List[0]
				: this.SelectedVariationName2
			: string.Empty;

		// 表示名3の項目設定
		foreach (DataRowView data in this.ProductVariationMasterList)
		{
			if ((this.ProductVariationName3List.Contains((string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]) == false)
				&& (string.IsNullOrEmpty((string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]) == false)
				&& ((string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1] == this.SelectedVariationName1)
				&& ((string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2] == this.SelectedVariationName2))
			{
				this.ProductVariationName3List.Add((string)data[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]);
			}
		}

		// 表示名3の選択値設定
		this.SelectedVariationName3 = (this.ProductVariationName3List.Any())
			? (this.ProductVariationName3List.Contains(this.SelectedVariationName3) == false)
				? this.ProductVariationName3List[0]
				: this.SelectedVariationName3
			: string.Empty;
	}

	/// <summary>
	/// バリエーションの選択状態を保持
	/// </summary>
	/// <param name="drvProduct">商品</param>
	/// <param name="strVariationId">バリエーションID</param>
	protected void SetProductVariationStatus(DataRowView drvProduct, string strVariationId)
	{
		if (string.IsNullOrEmpty(this.VariationId)) return;

		// 仕様的にはvariation_idは大文字小文字区別しない
		if (this.VariationId.Equals(strVariationId, StringComparison.OrdinalIgnoreCase))
		{
			// 選択バリエーションを保持
			this.ProductMaster = drvProduct;

			// バリエーション選択状態
			this.VariationSelected = true;
		}
	}

	/// <summary>
	/// カートに追加可能かを設定する
	/// </summary>

	protected void SetEnableAddCart()
	{
		if (this.IsSelectingVariationExist)
		{
			this.WlbRequestArrivalMail2.Visible = true;
			this.WlbRequestReleaseMail2.Visible = true;
			this.WlbRequestResaleMail2.Visible = true;
		}
		else
		{
			this.WlbRequestArrivalMail2.Visible = false;
			this.WlbRequestReleaseMail2.Visible = false;
			this.WlbRequestResaleMail2.Visible = false;
		}
	}

	/// <summary>
	/// バリエーションID取得：MATRIX
	/// </summary>
	/// <param name="strCulumnX">X軸カラム番号</param>
	/// <param name="strCulumnY">Y軸カラム番号</param>
	/// <param name="strX">S軸値</param>
	/// <param name="strY">Y軸値</param>
	/// <returns>バリエーションID</returns>
	protected string GetVariationIdForMatrix(string strCulumnX, object strX, string strCulumnY, object strY)
	{
		foreach (DataRowView drv in this.ProductVariationMasterList)
		{
			if (((string)drv[strCulumnX] == (string)strX) && ((string)drv[strCulumnY] == (string)strY))
			{
				return (string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID];
			}
		}

		return "";
	}

	/// <summary>
	/// 在庫文言取得：MATRIX
	/// </summary>
	protected string GetStockMessageForMatrix(string strCulumnX, object strX, string strCulumnY, object strY)
	{
		foreach (DataRowView drv in this.ProductVariationMasterList)
		{
			if (((string)drv[strCulumnX] == (string)strX) && ((string)drv[strCulumnY] == (string)strY))
			{
				return w2.App.Common.Order.ProductCommon.CreateProductStockMessage(drv, true);
			}
		}

		return "";
	}

	/// <summary>
	/// 通知メール登録フォーム表示 - クリック時の処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ViewRegsiterArrivalMailForm_Command(object sender, CommandEventArgs e)
	{
		if (this.HasVariation && string.IsNullOrEmpty(this.VariationId))
		{
			this.ErrorMessage = MESSAGE_ERROR_VARIATION_UNSELECTED;
			return;
		}

		var bpamrForm = GetWrappedControl<WrappedControl>("ucBpamr" + e.CommandArgument.ToString());

		if (this.HasVariation && bpamrForm.HasInnerControl)
		{
			var innerBpamrForm = (ProductArrivalMailRegisterUserControl)bpamrForm.InnerControl;
			if (string.IsNullOrEmpty(this.VariationId) == false) innerBpamrForm.VariationId = this.VariationId;
			if (string.IsNullOrEmpty(innerBpamrForm.VariationId))
			{
				this.ErrorMessage = MESSAGE_ERROR_VARIATION_UNSELECTED;
				return;
			}
		}

		bpamrForm.Visible = (bpamrForm.Visible == false);
	}

	/// <summary>
	/// バリエーション表示名1、表示名2からバリエーションIDを取得
	/// </summary>
	/// <remarks>表示名1,表示名2の1セットが重複しない前提</remarks>
	private void SetVariationIdForVariationName()
	{
		foreach (DataRowView drv in this.ProductVariationMasterList)
		{
			if ((this.WddlVariationSelect1.SelectedValue == (string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]) &&
				(this.WddlVariationSelect2.SelectedValue == (string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]))
			{
				this.VariationId = (string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID];
				break;
			}
		}
	}

	/// <summary>
	/// バリエーション変更共通処理
	/// </summary>
	private void ChangeVariation()
	{
		// バリエーション選択状態セット
		this.VariationSelected = (string.IsNullOrEmpty(this.VariationId) == false);

		// 商品データを画面に設定
		SetProductDataForDisplay();

		// バリエーション更新対象をデータバインド
		BasePage.DataBindByClass(this.Page, "ChangesByVariation");
	}

	/// <summary>
	/// 商品バリエーション表示名1・2の画像リスト設定
	/// </summary>
	protected void SetProductVariationImageList()
	{
		DataTable dtVariationImageListName1 = this.ProductVariationMasterList.ToTable();
		DataTable dtVariationImageListName2 = dtVariationImageListName1.Clone();
		dtVariationImageListName1.Clear();
		dtVariationImageListName2.Clear();

		foreach (DataRowView drvProduct in this.ProductVariationMasterList)
		{
			// 必要分を追加
			if ((string)this.ProductVariationMasterList[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1] == (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1])
			{
				dtVariationImageListName1.ImportRow(drvProduct.Row);
			}
			if ((string)this.ProductVariationMasterList[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2] == (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2])
			{
				dtVariationImageListName2.ImportRow(drvProduct.Row);
			}
		}

		this.ProductVariationImageListName1 = dtVariationImageListName2.DefaultView;
		this.ProductVariationImageListName2 = dtVariationImageListName1.DefaultView;
	}

	/// <summary>
	/// 在庫数取得：MATRIX
	/// </summary>
	/// <param name="strCulumnX">X軸カラム番号</param>
	/// <param name="strCulumnY">Y軸カラム番号</param>
	/// <param name="strX">S軸値</param>
	/// <param name="strY">Y軸値</param>
	/// <remarks>在庫数による文言をデザイン側で表示させたい場合に使用する。</remarks>
	protected int GetStockForMatrix(string strCulumnX, object strX, string strCulumnY, object strY)
	{
		foreach (DataRowView drv in this.ProductVariationMasterList)
		{
			if (((string)drv[strCulumnX] == (string)strX) && ((string)drv[strCulumnY] == (string)strY))
			{
				return (int)StringUtility.ToValue(drv[Constants.FIELD_PRODUCTSTOCK_STOCK], 0);
			}
		}

		return 0;
	}

	/// <summary>
	/// カートボタン（定期購入）クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCartAddFixedPurchase_Click(object sender, EventArgs e)
	{
		var scoringSale = this.ScoringSale;
		this.ScoringSale = null;
		var errorMessage = AddCart(
			Constants.AddCartKbn.FixedPurchase,
			"1",
			"CART",
			new ProductOptionSettingList(),
			this.ContentsLog);

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			this.ScoringSale = scoringSale;
		}

		AfterAddCart(true, errorMessage);
	}

	/// <summary>
	/// 頒布会申し込みクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCartAddSubscriptionBox_Click(object sender, EventArgs e)
	{
		SessionManager.ProductOptionSettingList = new ProductOptionSettingList();

		var subscriptionBoxDetailUrl =
			new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_SUBSCRIPTIONBOX_DETAIL)
				.AddParam(Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_COURSE_ID, this.WddlSubscriptionBox.SelectedValue)
				.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, this.ProductId)
				.AddParam(Constants.REQUEST_KEY_VARIATION_ID, this.VariationId)
				.AddParam(Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_FOR_COURSE_LIST, Constants.REDIRECT_TO_SUBSCRIPTION_BOX_DETAIL_FROM_PRODUCT_DETAIL)
				.AddParam(Constants.REQUEST_KEY_ITEM_QUANTITY, "1")
				.CreateUrl();

		Response.Redirect(subscriptionBoxDetailUrl);
	}

	/// <summary>
	/// カートへボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCartAdd_Click(object sender, System.EventArgs e)
	{
		if (IsAddCartByGiftType())
		{
			lbCartAddGift_Click(sender, e);
			return;
		}

		var scoringSale = this.ScoringSale;
		this.ScoringSale = null;
		var errorMessage = AddCart(
			Constants.AddCartKbn.Normal,
			"1",
			"CART",
			new ProductOptionSettingList(),
			this.ContentsLog);

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			this.ScoringSale = scoringSale;
		}

		AfterAddCart(true, errorMessage);
	}

	/// <summary>
	/// 画像の重複を排除して表示する
	/// </summary>
	/// <param name="variations">バリエーション</param>
	/// <param name="keys">キー</param>
	/// <returns>画像一覧</returns>
	protected IEnumerable<DataRowView> FilteringImages(DataView variations, params string[] keys)
	{
		var results = new List<DataRowView>();
		if (variations == null) return results;

		foreach (DataRowView variation in variations)
		{
			if (results.Any(result => keys.All(key => ((string)variation[key] == (string)result[key])))) continue;
			results.Add(variation);
		}
		return results.ToArray();
	}

	/// <summary>
	/// カートボタン（ギフト購入）クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCartAddGift_Click(object sender, EventArgs e)
	{
		string errorMessage = AddCart(Constants.AddCartKbn.GiftOrder, "1", "CART", new ProductOptionSettingList());
		AfterAddCart(true, errorMessage);
	}

	/// <summary>
	/// カート投入後の後処理
	/// </summary>
	/// <param name="singleSelectAddCart">単体カート投入有無（単体:true、複数:false）</param>
	/// <param name="errorMessage">カート投入処理時のエラーメッセージ</param>
	private void AfterAddCart(bool singleSelectAddCart, string errorMessage)
	{
		if (string.IsNullOrEmpty(errorMessage)) return;

		// 商品データを画面に設定
		SetProductDataForDisplay();

		if (singleSelectAddCart)
		{
			this.AlertMessage = errorMessage;
			this.Buyable = false;
		}

		// 画面全体をデータバインド
		this.DataBind();
	}

	/// <summary>
	/// 商品データ取得
	/// </summary>
	/// <param name="key">キー（フィールド）</param>
	/// <returns>商品データ</returns>
	protected object GetProductData(string key)
	{
		return GetProductData(this.ProductMaster, key);
	}

	/// <summary>
	/// 頒布会購入価格をセット
	/// </summary>
	protected void SetSubscriptionBoxPriceForDisplay()
	{
		foreach (RepeaterItem rAddCartVariationItem in WrAddCartVariationList.Items)
		{
			var wddlSubscriptionBox = GetWrappedControl<WrappedDropDownList>(rAddCartVariationItem, "ddlSubscriptionBox");
			var whfVariation = GetWrappedControl<WrappedHiddenField>(rAddCartVariationItem, "hfVariation");
			var whfProduct = GetWrappedControl<WrappedHiddenField>(rAddCartVariationItem, "hfProduct");
			var wdSubscriptionBoxItemPrice = GetWrappedControl<WrappedHtmlGenericControl>(rAddCartVariationItem, "dSubscriptionBoxItemPrice");
			var wdSubscriptionBoxItemPriceStrikethrough = GetWrappedControl<WrappedHtmlGenericControl>(rAddCartVariationItem, "dSubscriptionBoxItemPriceStrikethrough");
			var wdSubscriptionBoxItemCampaignPrice = GetWrappedControl<WrappedHtmlGenericControl>(rAddCartVariationItem, "dSubscriptionBoxItemCampaignPrice");
			var wlSubscriptionBoxItemCampaignPrice = GetWrappedControl<WrappedLiteral>(rAddCartVariationItem, "lSubscriptionBoxItemCampaignPrice");
			var wdSubscriptionBoxItemCampaignPeriod = GetWrappedControl<WrappedHtmlGenericControl>(rAddCartVariationItem, "dSubscriptionBoxItemCampaignPeriod");
			var wlSubscriptionBoxItemCampaignPeriodSince = GetWrappedControl<WrappedLiteral>(rAddCartVariationItem, "lSubscriptionBoxItemCampaignPeriodSince");
			var wlSubscriptionBoxItemCampaignPeriodUntil = GetWrappedControl<WrappedLiteral>(rAddCartVariationItem, "lSubscriptionBoxItemCampaignPeriodUntil");
			var wdSubscriptionBoxPrice = GetWrappedControl<WrappedHtmlGenericControl>(rAddCartVariationItem, "dSubscriptionBoxPrice");
			var wlSubscriptionBoxPriceSince = GetWrappedControl<WrappedLiteral>(rAddCartVariationItem, "lSubscriptionBoxPriceSince");
			var wlSubscriptionBoxPriceUntil = GetWrappedControl<WrappedLiteral>(rAddCartVariationItem, "lSubscriptionBoxPriceUntil");
			var wsSince = GetWrappedControl<WrappedHtmlGenericControl>(rAddCartVariationItem, "sSince");


			if (string.IsNullOrEmpty(wddlSubscriptionBox.SelectedValue)) return;

			var selectedSubscriptionBox = DataCacheControllerFacade
				.GetSubscriptionBoxCacheController()
				.Get(wddlSubscriptionBox.SelectedValue);

			var subscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
				x => (x.ProductId == whfProduct.Value)
					&& (x.VariationId == whfVariation.Value));

			if (subscriptionBoxItem == null) return;

			// 初期化
			wdSubscriptionBoxItemPrice.Visible
				= wdSubscriptionBoxPrice.Visible
					= true;
			wdSubscriptionBoxItemPriceStrikethrough.Visible
				= wdSubscriptionBoxPrice.Visible
					= wsSince.Visible
						= false;
			wlSubscriptionBoxPriceSince.Text = wlSubscriptionBoxPriceUntil.Text =
				wlSubscriptionBoxItemCampaignPrice.Text = wlSubscriptionBoxItemCampaignPeriodSince.Text =
					wlSubscriptionBoxItemCampaignPeriodUntil.Text = String.Empty;

			// キャンペーン期間であればキャンペーン期間価格を適用
			if (OrderCommon.IsSubscriptionBoxCampaignPeriod(subscriptionBoxItem))
			{
				wdSubscriptionBoxItemPriceStrikethrough.Visible
					= wdSubscriptionBoxItemCampaignPrice.Visible
						= wdSubscriptionBoxItemCampaignPeriod.Visible
							= true;
				wdSubscriptionBoxItemPrice.Visible = false;
				wlSubscriptionBoxItemCampaignPrice.Text = HtmlSanitizer.HtmlEncode(
					CurrencyManager.ToPrice(
						StringUtility.ToNumeric(subscriptionBoxItem.CampaignPrice.ToPriceString())));
				wlSubscriptionBoxItemCampaignPeriodSince.Text =
					HtmlSanitizer.HtmlEncode(StringUtility.ToEmpty(subscriptionBoxItem.CampaignSince));
				wlSubscriptionBoxItemCampaignPeriodUntil.Text =
					HtmlSanitizer.HtmlEncode(StringUtility.ToEmpty(subscriptionBoxItem.CampaignUntil));
			}

			if (selectedSubscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE)
			{
				wlSubscriptionBoxPriceSince.Text = HtmlSanitizer.HtmlEncode(
					CurrencyManager.ToPrice(
						StringUtility.ToNumeric(selectedSubscriptionBox.FixedAmount.ToPriceString())));
				wdSubscriptionBoxItemPrice.Visible
					= wdSubscriptionBoxItemPriceStrikethrough.Visible
						= wdSubscriptionBoxItemCampaignPrice.Visible
							= wdSubscriptionBoxItemCampaignPeriod.Visible
								= wlSubscriptionBoxPriceUntil.Visible
									= wsSince.Visible
										= false;
				wdSubscriptionBoxPrice.Visible = true;
			}
			else
			{
				if (selectedSubscriptionBox.MinimumAmount != null || selectedSubscriptionBox.MaximumAmount != null)
				{
					wlSubscriptionBoxPriceSince.Text = HtmlSanitizer.HtmlEncode(
						CurrencyManager.ToPrice(
							StringUtility.ToNumeric(StringUtility.ToEmpty(selectedSubscriptionBox.MinimumAmount))));
					wlSubscriptionBoxPriceUntil.Text = HtmlSanitizer.HtmlEncode(
						CurrencyManager.ToPrice(
							StringUtility.ToNumeric(StringUtility.ToEmpty(selectedSubscriptionBox.MaximumAmount))));
					wsSince.Visible
						= wdSubscriptionBoxPrice.Visible
							= true;
				}
			}
		}
	}

	/// <summary>
	/// 選択された組み合わせのバリエーションがない場合、エラーメッセージ表示、カート投入ボタン非活性化
	/// </summary>
	private void VariationCombinationCheck()
	{
		this.WlCombinationErrorMessage.Text = "";
		this.WlbCartAdd.Enabled = true;
		this.WlbCartAddFixedPurchase.Enabled = true;
		this.WlbCartAddSubscriptionBox.Enabled = true;
		this.WlbCartAddForGift.Enabled = true;
		this.WlbRequestArrivalMail2.Enabled = true;

		if ((string.IsNullOrEmpty(this.SelectedVariationName1)) || (string.IsNullOrEmpty(this.SelectedVariationName2))) return;
		foreach (DataRowView drvProduct in this.ProductVariationMasterList)
		{
			var variationName1 = (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
			var variationName2 = (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
			if ((variationName1 == this.SelectedVariationName1) && (variationName2 == this.SelectedVariationName2)) return;
		}

		this.WlCombinationErrorMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USE_DOUBLEDROPDOWNLIST_COMBINATION_ERROR);
		this.WlbCartAdd.Enabled = false;
		this.WlbCartAddFixedPurchase.Enabled = false;
		this.WlbCartAddSubscriptionBox.Enabled = false;
		this.WlbCartAddForGift.Enabled = false;
		this.WlbRequestArrivalMail2.Enabled = false;
	}

	/// <summary>
	/// バリエーション画像選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbVariaionImages_OnClick(object sender, EventArgs e)
	{
		this.VariationId = ((LinkButton)sender).CommandArgument;
		this.WddlVariationSelect1.SelectedValue = "";
		this.WddlVariationSelect2.SelectedValue = "";

		this.SelectedVariationName1 = string.Empty;
		this.SelectedVariationName2 = string.Empty;
		this.SelectedVariationName3 = string.Empty;

		// バリエーション変更共通処理
		ChangeVariation();
	}

	/// <summary>
	/// バリエーション名1のクリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbVariationName1List_OnClick(object sender, EventArgs e)
	{
		this.SelectedVariationName1 = ((LinkButton)sender).CommandArgument;
		ChangeVariation();
	}

	/// <summary>
	/// バリエーション名2のクリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbVariationName2List_OnClick(object sender, EventArgs e)
	{
		this.SelectedVariationName2 = ((LinkButton)sender).CommandArgument;
		ChangeVariation();
	}

	/// <summary>
	/// バリエーション名3のクリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbVariationName3List_OnClick(object sender, EventArgs e)
	{
		this.SelectedVariationName3 = ((LinkButton)sender).CommandArgument;
		ChangeVariation();
	}


	/// <summary>
	/// 商品バリエーション選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlVariationId_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 商品バリエーション選択方法別にバリエーションID取得
		switch (this.SelectVariationKbn)
		{
			case Constants.SelectVariationKbn.STANDARD:
			case Constants.SelectVariationKbn.DROPDOWNLIST:
				this.VariationId = this.WddlVariationSelect.SelectedValue;
				break;

			case Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST:
				if (string.IsNullOrEmpty(this.WddlVariationSelect1.SelectedValue) || string.IsNullOrEmpty(this.WddlVariationSelect2.SelectedValue))
				{
					this.VariationId = "";
				}
				this.SelectedVariationName1 = this.WddlVariationSelect1.SelectedValue;
				this.SelectedVariationName2 = this.WddlVariationSelect2.SelectedValue;
				break;

			case Constants.SelectVariationKbn.MATRIX:
			case Constants.SelectVariationKbn.MATRIXANDMESSAGE:
				// シングルリスト または 旧マトリクスリスト の場合
				// ※下位互換のため、旧マトリクスリスト「this.WrVariationPluralY」のバリエーションID取得処理を残している
				WrappedRepeater wrVariationList = this.IsPluralVariation ? this.WrVariationPluralY : this.WrVariationSingleList;
				foreach (RepeaterItem ri in wrVariationList.Items)
				{
					var wrbgVariationId = GetWrappedControl<WrappedRadioButtonGroup>(ri, "rbgVariationId");
					var whfVariationId = GetWrappedControl<WrappedHiddenField>(ri, "hfVariationId", "");
					if (wrbgVariationId.Checked)
					{
						this.VariationId = whfVariationId.Value;
						break;
					}
				}
				// マトリクスリストの場合
				if (this.IsPluralVariation)
				{
					foreach (RepeaterItem riY in WrVariationMatrixY.Items)
					{
						foreach (RepeaterItem riX in ((Repeater)(riY.FindControl("rVariationMatrixX"))).Items)
						{
							var wrbgVariationId = GetWrappedControl<WrappedRadioButtonGroup>(riX, "rbgVariationId");
							var whfVariationId = GetWrappedControl<WrappedHiddenField>(riX, "hfVariationId", "");
							if (wrbgVariationId.Checked)
							{
								this.VariationId = whfVariationId.Value;
								break;
							}
						}
					}
				}
				break;
		}

		// バリエーション変更共通処理
		ChangeVariation();
	}

	/// <summary>商品名（フレンドリーURL生成用）</summary>
	protected string ProductName
	{
		get { return (string)ViewState["ProductName"]; }
		set { ViewState["ProductName"] = value; }
	}
	/// <summary>商品マスタ</summary>
	protected DataRowView ProductMaster { get; private set; }
	protected ProductOptionSettingList ProductOptionSettingList
	{
		get { return (ProductOptionSettingList)ViewState["product_option_setting_list"]; }
		set { ViewState["product_option_setting_list"] = value; }
	}
	/// <summary>商品バリエーションマスタ</summary>
	protected DataView ProductVariationMasterList { get; private set; }
	////// <summary>商品サブ画像リスト</summary>
	protected List<DataRowView> ProductSubImageList { get; private set; }
	/// <summary>クロスセル商品リスト</summary>
	public DataView ProductCrossSellList { get; set; }
	/// <summary>アップセル商品リスト</summary>
	public DataView ProductUpSellList { get; set; }
	/// <summary>購入可能かどうか</summary>
	protected bool Buyable
	{
		get { return (ViewState["Buyable"] != null) ? (bool)ViewState["Buyable"] : false; }
		private set { ViewState["Buyable"] = value; }
	}
	/// <summary>バリエーションを持っているか</summary>
	protected new bool HasVariation
	{
		get { return ViewState["HasVariation"] != null ? (bool)ViewState["HasVariation"] : false; }
		private set { ViewState["HasVariation"] = value; }
	}
	/// <summary>定期購入可能か</summary>
	protected bool CanFixedPurchase
	{
		get { return (bool)ViewState["CanFixedPurchase"]; }
		private set { ViewState["CanFixedPurchase"] = value; }
	}
	/// <summary>ギフト購入可能か</summary>
	protected bool CanGiftOrder
	{
		get { return (bool)ViewState["CanGiftOrder"]; }
		private set { ViewState["CanGiftOrder"] = value; }
	}
	/// <summary>バリエーションが選択されているか</summary>
	protected bool VariationSelected
	{
		get { return (ViewState["SelectVariation"] != null) ? (bool)ViewState["SelectVariation"] : false; }
		private set { ViewState["SelectVariation"] = value; }
	}
	/// <summary>在庫文言を持っているか</summary>
	protected bool HasStockMessage
	{
		get
		{
			if (IsStockManaged == false) return false;
			return (ViewState["HasStockMessage"] != null) ? (bool)ViewState["HasStockMessage"] : false;
		}
		private set { ViewState["HasStockMessage"] = value; }
	}
	/// <summary>カート投入可能か</summary>
	protected bool CanAddCart
	{
		get { return (bool)ViewState["CanAddCart"]; }
		private set { ViewState["CanAddCart"] = value; }
	}
	/// <summary>カート投入可能か</summary>
	protected string AlertMessage
	{
		get { return (string)ViewState["AlertMessage"]; }
		private set { ViewState["AlertMessage"] = value; }
	}
	/// <summary>The alert messages for limited payment</summary>
	protected string LimitedPaymentMessages
	{
		get { return (string)ViewState["LimitedPaymentMessages"]; }
		private set { ViewState["LimitedPaymentMessages"] = value; }
	}
	/// <summary>入荷通知メール区分</summary>
	protected string ArrivalMailKbn
	{
		get { return (string)ViewState["ArrivalMailKbn"]; }
		private set { ViewState["ArrivalMailKbn"] = value; }
	}
	/// <summary>商品バリエーションリストアイテムコレクション</summary>
	protected ListItemCollection ProductValirationListItemCollection { get; private set; }
	/// <summary>商品バリエーションリストアイテムコレクション</summary>
	protected ListItemCollection ProductValirationListItemCollection2 { get; private set; }
	/// <summary>商品バリエーション選択方法</summary>
	protected Constants.SelectVariationKbn SelectVariationKbn
	{
		get { return (Constants.SelectVariationKbn)ViewState["SelectVariationKbn"]; }
		private set { ViewState["SelectVariationKbn"] = value; }
	}
	/// <summary>選択したバリエーションID</summary>
	protected string SelectVariationId
	{
		get { return (string)ViewState["SelectVariationId"]; }
		set { ViewState["SelectVariationId"] = value; }
	}
	/// <summary>バリエーション表示名1</summary>
	protected string SelectedVariationName1
	{
		get { return (ViewState["SelectedVariationName1"] != null) ? (string)ViewState["SelectedVariationName1"] : ""; }
		set { ViewState["SelectedVariationName1"] = value; }
	}
	/// <summary>バリエーション表示名2</summary>
	protected string SelectedVariationName2
	{
		get { return (ViewState["SelectedVariationName2"] != null) ? (string)ViewState["SelectedVariationName2"] : ""; }
		set { ViewState["SelectedVariationName2"] = value; }
	}
	/// <summary>バリエーション表示名3</summary>
	protected string SelectedVariationName3
	{
		get { return (ViewState["SelectedVariationName3"] != null) ? (string)ViewState["SelectedVariationName3"] : string.Empty; }
		set { ViewState["SelectedVariationName3"] = value; }
	}
	/// <summary>2軸の商品か</summary>
	protected bool IsPluralVariation
	{
		get { return (ViewState["IsPluralVariation"] != null) ? (bool)ViewState["IsPluralVariation"] : false; }
		set { ViewState["IsPluralVariation"] = value; }
	}
	/// <summary>販売期間表示か</summary>
	protected bool DisplaySell
	{
		get { return (bool)ViewState["DisplaySell"]; }
		private set { ViewState["DisplaySell"] = value; }
	}
	/// <summary>バリエーションX軸画像リスト</summary>
	protected DataView ProductVariationImageListName1 { get; private set; }
	/// <summary>バリエーションY軸画像リスト</summary>
	protected DataView ProductVariationImageListName2 { get; private set; }
	/// <summary>表示名１リスト</summary>
	protected List<string> VariationName1List { get; private set; }
	/// <summary>表示名２リスト</summary>
	protected List<string> VariationName2List { get; private set; }
	/// <summary>この商品を含むセットプロモーションリスト</summary>
	protected SetPromotionModel[] SetPromotions { get; private set; }
	/// <summary>Error message</summary>
	protected string ErrorMessageFixedPurchaseMember { get; private set; }

	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage { get; private set; }
	/// <summary>商品バリエーション名1リスト</summary>
	protected List<string> ProductVariationName1List
	{
		get { return (ViewState["ProductVariationName1List"] != null) ? (List<string>)ViewState["ProductVariationName1List"] : null; }
		set { ViewState["ProductVariationName1List"] = value; }
	}
	/// <summary>商品バリエーション名2リスト</summary>
	protected List<string> ProductVariationName2List
	{
		get { return (ViewState["ProductVariationName2List"] != null) ? (List<string>)ViewState["ProductVariationName2List"] : null; }
		set { ViewState["ProductVariationName2List"] = value; }
	}
	/// <summary>商品バリエーション名3リスト</summary>
	protected List<string> ProductVariationName3List
	{
		get { return (ViewState["ProductVariationName3List"] != null) ? (List<string>)ViewState["ProductVariationName3List"] : null; }
		set { ViewState["ProductVariationName3List"] = value; }
	}
	/// <summary>商品バリエーション名3か</summary>
	protected bool IsVariationName3
	{
		get { return (ViewState["IsVariationName3"] != null) ? (bool)ViewState["IsVariationName3"] : false; }
		set { ViewState["IsVariationName3"] = value; }
	}
	/// <summary>選択したバリエーションがあるか</summary>
	protected bool IsSelectingVariationExist
	{
		get { return (ViewState["IsSelectingVariationExist"] != null) ? (bool)ViewState["IsSelectingVariationExist"] : true; }
		set { ViewState["IsSelectingVariationExist"] = value; }
	}
	/// <summary>バリエーション無しエラー</summary>
	protected string AlertMessageVariationNotExist
	{
		get { return (ViewState["AlertMessageVariationNotExist"] != null) ? (string)ViewState["AlertMessageVariationNotExist"] : string.Empty; }
		private set { ViewState["AlertMessageVariationNotExist"] = value; }
	}
	/// <summary>スコアリングセール</summary>
	protected ScoringSaleInput ScoringSale
	{
		get { return (ScoringSaleInput)Session["ScoringSaleInput"]; }
		set { Session["ScoringSaleInput"] = value; }
	}
	/// <summary>コンテンツログ</summary>
	protected ContentsLogModel ContentsLog
	{
		get { return (ContentsLogModel)ViewState["ContentsLog"]; }
		set { ViewState["ContentsLog"] = value; }
	}
	/// <summary>頒布会フラグ</summary>
	protected string SubscriptionBoxFlg
	{
		get { return (string)ViewState["SubscriptionBoxFlg"]; }
		set { ViewState["SubscriptionBoxFlg"] = value; }
	}
	/// <summary>在庫管理を行っているか</summary>
	protected bool IsStockManaged
	{
		get { return (bool)ViewState["IsStockManaged"]; }
		private set { ViewState["IsStockManaged"] = value; }
	}
	/// <summary>在庫表の表示をするか</summary>
	protected bool ShouldShowStockList
	{
		get { return (this.HasStockMessage && this.HasVariation); }
	}
	/// <summary>バリエーション在庫表示をするか</summary>
	protected bool IsDisplayValiationStock
	{
		get
		{
			return (this.IsStockManaged
				&& (((this.IsSelectingVariationExist)
				&& (this.SelectVariationKbn == Constants.SelectVariationKbn.PANEL))
					|| ((this.IsSelectingVariationExist) && this.IsVariationName3
				&& ((this.SelectVariationKbn == Constants.SelectVariationKbn.DOUBLEDROPDOWNLIST)
					|| (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIX)
					|| (this.SelectVariationKbn == Constants.SelectVariationKbn.MATRIXANDMESSAGE)))));
		}
	}
	/// <summary>商品概要表示フラグ</summary>
	protected bool IsProductOutlineVisible
	{
		get { return string.IsNullOrEmpty(GetProductDataHtml(this.ProductMaster, Constants.FIELD_PRODUCT_OUTLINE)) == false; }
	}
	/// <summary>商品詳細説明表示フラグ</summary>
	protected bool IsProductDetailVisible
	{
		get
		{
			return (string.IsNullOrEmpty(GetProductDataHtml(this.ProductMaster, Constants.FIELD_PRODUCT_DESC_DETAIL1))
				&& string.IsNullOrEmpty(GetProductDataHtml(this.ProductMaster, Constants.FIELD_PRODUCT_DESC_DETAIL2))
				&& string.IsNullOrEmpty(GetProductDataHtml(this.ProductMaster, Constants.FIELD_PRODUCT_DESC_DETAIL3))
				&& string.IsNullOrEmpty(GetProductDataHtml(this.ProductMaster, Constants.FIELD_PRODUCT_DESC_DETAIL4))) == false;
		}
	}
	/// <summary>配送料無料適応外文言表示か</summary>
	protected bool IsDisplayExcludeFreeShippingText
	{
		get { return Constants.FREE_SHIPPING_FEE_OPTION_ENABLED && ((string)this.ProductMaster[Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG] == Constants.FLG_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG_VALID); }
	}
	/// <summary>モーダル表示用商品ID</summary>
	private string ProductIdForModal
	{
		get { return this.Process.ProductIdForModal; }
		set { this.Process.ProductIdForModal = value; }
	}
}
