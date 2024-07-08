/*
=========================================================================================================
  Module      : 商品詳細画面処理(ProductDetail.aspx.cs)
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
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Preview;
using w2.App.Common.Product;
using w2.App.Common.UserProductArrivalMail;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.ContentsLog;
using w2.Domain.Favorite;
using w2.Domain.Product;
using w2.Domain.SetPromotion;
using w2.Domain.SubscriptionBox;

public partial class Form_Product_ProductDetail : ProductPage
{
	/// <summary>リピートプラスONEリダイレクト必須判定</summary>
	public override bool RepeatPlusOneNeedsRedirect { get { return Constants.REPEATPLUSONE_OPTION_ENABLED; } }
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Http; } }	// Httpアクセス

	#region ラップ済みコントロール宣言
	WrappedRepeater WrProductOptionValueSettings { get { return GetWrappedControl<WrappedRepeater>("rProductOptionValueSettings"); } }
	WrappedHiddenField WhfIsRedirectAfterAddProduct { get { return GetWrappedControl<WrappedHiddenField>("hfIsRedirectAfterAddProduct", ""); } }
	protected WrappedDropDownList WddlVariationSelect1 { get { return GetWrappedControl<WrappedDropDownList>("ddlVariationSelect1"); } }
	protected WrappedDropDownList WddlVariationSelect2 { get { return GetWrappedControl<WrappedDropDownList>("ddlVariationSelect2"); } }
	protected WrappedDropDownList WddlVariationSelect { get { return GetWrappedControl<WrappedDropDownList>("ddlVariationSelect"); } }
	protected WrappedRepeater WrVariationSingleList { get { return GetWrappedControl<WrappedRepeater>("rVariationSingleList"); } }
	protected WrappedRepeater WrVariationPluralY { get { return GetWrappedControl<WrappedRepeater>("rVariationPluralY"); } } // ※下位互換用のため削除禁止
	protected WrappedRepeater WrAddCartVariationList { get { return GetWrappedControl<WrappedRepeater>("rAddCartVariationList"); } }
	WrappedRepeater WrVariationPluralX { get { return GetWrappedControl<WrappedRepeater>("rVariationPluralX"); } }
	WrappedTextBox WtbCartAddProductCount { get { return GetWrappedControl<WrappedTextBox>("tbCartAddProductCount", "1"); } }
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
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{

			// リクエストよりパラメタ取得
			GetParameters();

			// ブランドチェック
			BrandCheck(this.ProductId);

			// モバイルオプションがON かつ モバイルからのアクセスだったらモバイルページへ
			if (Constants.MOBILEOPTION_ENABLED && this.IsMobile)
			{
				// 一部のSoftbank機種がAUと判断されてしまうためリダイレクト前にContentsTypeを固定で指定
				Response.ContentType = "text/html";
				Response.Redirect(GetMobileUrl());
			}

			// 商品データを画面に設定
			SetProductDataForDisplay();

			// お気に入りデータを画面に設定
			SetFavoriteDataForDisplay();

			// 商品タグ情報設定
			SetProductTagInfoForDisplay();

			// 画面全体をデータバインド
			this.DataBind();
		}

		// SEOメタデータ情報取得・設定(V5.13以前に構築された環境用)
		if (!this.IsPostBack && Constants.SEOTAG_AND_OGPTAG_IN_PRODUCTDETAIL_ENABLED && Constants.SEOTAGDISPSETTING_OPTION_ENABLED)
		{
			var utility = new SeoUtility(this.Request, this.Page, this.Session);
			this.ViewState["title"] = utility.SeoData.HtmlTitle;
			this.SeoDescription = utility.SeoData.MetadataDesc;
			this.SeoKeywords = utility.SeoData.MetadataKeywords;
			this.SeoComment = utility.SeoData.Comment;
			this.Header.DataBind();
			this.Title = (string)this.ViewState["title"];
		}
		else if (Constants.SEOTAG_AND_OGPTAG_IN_PRODUCTDETAIL_ENABLED && Constants.SEOTAGDISPSETTING_OPTION_ENABLED)
		{
			var utility = new SeoUtility(this.Request, this.Page, this.Session);
			this.ViewState["title"] = utility.SeoData.HtmlTitle;
			this.Title = (string)this.ViewState["title"];
		}

		if (this.ScoringSale != null)
		{
			this.ContentsLog = new ContentsLogModel
			{
				ContentsId = this.ScoringSale.ScoringSaleId,
				ContentsType = Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_SCORINGSALE,
			};
		}
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
	/// ページプリレンダー
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_PreRender(object sender, RepeaterCommandEventArgs e)
	{
		if (IsPostBack || (e.CommandName != Constants.COMMAND_NAME_SMART_ARRIVALMAIL))
		{
			// ミニカートから商品を削除した時等に表示を更新する
			SetProductDataForDisplay();
			DataBind();
		}
	}

	/// <summary>
	/// 商品データ設定
	/// </summary>
	private void SetProductDataForDisplay()
	{
		//------------------------------------------------------
		// プレビューモードチェック
		//------------------------------------------------------
		bool bPreviewMode = false;
		if ((string)Request[Constants.REQUEST_KEY_PREVIEW_HASH] != null)
		{
			// ハッシュチェック
			if (this.IsPreviewProductMode)
			{
				bPreviewMode = true;

				// プレビューの場合、キャッシュを残さない
				Response.AddHeader("Cache-Control", "private, no-store, no-cache, must-revalidate");
				Response.AddHeader("Pragma", "no-cache");
				Response.Cache.SetAllowResponseInBrowserHistory(false);
			}
		}

		//------------------------------------------------------
		// 商品情報取得
		//------------------------------------------------------
		if (this.IsPreview) this.ProductId = Uri.UnescapeDataString(this.ProductId);

		DataView dvProduct = (bPreviewMode) ?
			ProductPreview.GetProductDetailPreview(this.ShopId, this.ProductId)
			: ProductCommon.GetProductInfo(this.ShopId, this.ProductId, this.MemberRankId, this.UserFixedPurchaseMemberFlg);

		if (dvProduct.Count == 0)
		{
			// 商品が見つからない場合はエラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_UNDISP);
			// フレンドリーURL利用時は494を捕捉できないのため、302→404転送とする
			if (Constants.FRIENDLY_URL_ENABLED || this.IsPreview)
			{
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
			else
			{
				Response.StatusCode = 404;
				Response.End();
			}
		}

			//------------------------------------------------------
			// 各種プロパティセット
			//------------------------------------------------------
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
			this.IsUserFixedPurchaseAble = (CheckFixedPurchaseLimitedUserLevel(this.ShopId, this.ProductId) == false);
			this.SubscriptionBoxFlg = (string)this.ProductMaster[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG];
			this.IsStockManaged = ((string)this.ProductMaster[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED);

			//------------------------------------------------------
			// 商品バリエーションがある場合，ない場合の処理
			//------------------------------------------------------
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
										&&((product.TermSince <= DateTime.Now)
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
			//------------------------------------------------------
			// 表示期間チェック（非表示の場合エラーページへ）
			//------------------------------------------------------
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

			//------------------------------------------------------
			// 閲覧会員ランクチェック
			//------------------------------------------------------
			string strDisplayMemberRank = (string)this.ProductMaster[Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK];

			// プレビューモードを除く
			if ((bPreviewMode == false) && (this.IsPreview == false))
			{
				if (MemberRankOptionUtility.CheckMemberRankPermission(this.MemberRankId, strDisplayMemberRank) == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_DISPLAY_MEMBER_RANK).Replace("@@ 1 @@", MemberRankOptionUtility.GetMemberRankName(strDisplayMemberRank));
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}

			//------------------------------------------------------
			// 商品表示履歴追加
			//------------------------------------------------------
			// 商品履歴オブジェクトが生成されている場合
			if (Session[Constants.SESSION_KEY_PRODUCTHISTORY_LIST] == null)
			{
				Session[Constants.SESSION_KEY_PRODUCTHISTORY_LIST] = new ProductHistoryObject();
			}
			ProductHistoryObject ph = (ProductHistoryObject)Session[Constants.SESSION_KEY_PRODUCTHISTORY_LIST];
			ph.Add(this.ProductMaster);

			//------------------------------------------------------
			// 販売期間チェック && 在庫有無チェック（カートボタン非表示・在庫文言非表示、文言表示）
			//------------------------------------------------------
			int productCount;
			if (int.TryParse(StringUtility.ToEmpty(this.WtbCartAddProductCount.Text), out productCount) == false)
			{
				productCount = 1;
			}
			// バリエーション有り && バリエーション未選択
			bool blStockFlg = false;
			if (this.HasVariation && (this.VariationSelected == false))
			{
				blStockFlg = CheckProductStockBuyable(this.ProductVariationMasterList, productCount);	// 在庫有無チェック
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

			// 注文数および加算数のチェック
			if (strErrorMessage == "")
			{
				strErrorMessage = CheckAddProductCount(StringUtility.ToHankaku(this.WtbCartAddProductCount.Text));
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

			//------------------------------------------------------
			// 入荷通知メール区分取得
			//------------------------------------------------------
			this.ArrivalMailKbn = UserProductArrivalMailCommon.GetArrivalMailKbn(this.HasVariation, this.VariationSelected, this.ProductVariationMasterList, this.ProductMaster);

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();

			//------------------------------------------------------
			// 商品アップセルデータ取得（データバインドは画面全体で）
			//------------------------------------------------------
			this.ProductUpSellList = ProductCommon.GetProductUpSellProducts(this.ProductMaster, sqlAccessor, (this.WhfShowOutOfStockUpSellList.Value.ToLower() == "true"), this.LoginUserId);

			//------------------------------------------------------
			// 商品クロスセルデータ取得（データバインドは画面全体で）
			//------------------------------------------------------
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

			//------------------------------------------------------
			// サブ画像設定取得（存在するもののみ）
			//------------------------------------------------------
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

			//------------------------------------------------------
			// バリエーション毎のカート投入リスト作成
			//------------------------------------------------------
			var variationList = GetVariationAddCartList(dvProduct);
			// バリエーションを持っている場合は、エラーメッセージの商品名をバリデーション名に置換
			if (this.HasVariation)
			{
				foreach (var variation in variationList)
				{
					if (variation.ContainsKey("ErrorMessage") == false) continue;

					var variationName = CreateVariationName(
						variation,
						string.Empty,
						string.Empty,
						Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION);

					variation["ErrorMessage"] = ((string)variation["ErrorMessage"]).Replace(
						this.ProductName,
						variationName);
				}
			}
			this.ProductVariationAddCartList = variationList;

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
	/// 全お気に入り登録人数取得
	/// </summary>
	/// <returns>お気に入り登録人数合計</returns>
	protected int SetTotalFavoriteDataForDisplay()
	{
		return SetFavoriteDataForDisplay(this.ProductId, string.Empty);
			}

	/// <summary>
	/// 商品のお気に入り登録人数取得
	/// </summary>
	/// <returns>商品のお気に入り登録人数</returns>
	protected int SetFavoriteDataOfProductForDisplay()
	{
		var favorite = new FavoriteModel()
		{
			ShopId = this.ShopId,
			ProductId = this.ProductId,
			VariationId = String.Empty,
		};

		// 商品のお気に入り登録数取得
		return this.FavoriteUserCount = new FavoriteService().GetFavoriteByProduct(favorite);
	}

	/// <summary>
	/// お気に入りデータ設定
	/// </summary>
	/// <returns>お気に入り登録者数</returns>
	protected void SetFavoriteDataForDisplay()
	{
		// 商品のお気に入り登録数設定
		this.FavoriteUserCount = SetFavoriteDataForDisplay(this.ProductId, this.VariationId);
	}

	/// <summary>
	/// お気に入りデータ設定
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>お気に入り登録者数</returns>
	protected int SetFavoriteDataForDisplay(string productId, string variationId)
	{
		var favorite = new FavoriteModel
		{
			ShopId = this.ShopId,
			ProductId = productId,
			VariationId = variationId,
		};
		return new FavoriteService().GetFavoriteCountByProduct(favorite);
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
		this.ProductValirationListItemCollection = new ListItemCollection();
		this.ProductValirationListItemCollection.Add(
			new ListItem(ReplaceTag("@@DispText.variation_name_list.unselected@@"), ""));
		this.ProductValirationListItemCollection2 = new ListItemCollection();
		this.ProductValirationListItemCollection2.Add(
			new ListItem(ReplaceTag("@@DispText.variation_name_list.unselected@@"), ""));
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

		if (this.VariationId != "")
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
		//------------------------------------------------------
		// バリエーション2軸チェック
		//------------------------------------------------------
		this.IsPluralVariation = false;
		foreach (DataRowView drv in this.ProductVariationMasterList)
		{
			if (((string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]).Length != 0)
			{
				this.IsPluralVariation = true;
				break;
			}
		}

		//------------------------------------------------------
		// マトリクスリスト作成処理
		//------------------------------------------------------
		// 2軸の場合
		if (this.IsPluralVariation)
		{
			//------------------------------------------------------
			// 旧マトリクスリスト作成（V5.2以前）
			//------------------------------------------------------
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

			//------------------------------------------------------
			// マトリクスリスト作成（V5.3以降）
			//------------------------------------------------------
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

		//------------------------------------------------------
		// バリエーションの選択状態を保持
		//------------------------------------------------------
		foreach (DataRowView drv in this.ProductVariationMasterList)
		{
			SetProductVariationStatus(drv, (string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
		}
	}

	/// <summary>
	/// Set variation id for panel variation name
	/// </summary>
	private void SetVariationIdForPanelVariationName()
	{
		if ((string.IsNullOrEmpty(this.SelectedVariationName1) == false)
			|| (string.IsNullOrEmpty(this.SelectedVariationName2) == false)
			|| (string.IsNullOrEmpty(this.SelectedVariationName3) == false))
		{
			this.VariationId = string.Empty;
		}
		var variationSelectedIndex = 0;

		// In case of variation id do not have value
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

			// Add variation NOT exist by variation name 1
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
			// For variation id have value to select image
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

		// Set enable add cart button
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
	/// Set variation for panel
	/// </summary>
	private void SetVariationForPanel()
	{
		var isExistsVariationId = this.ProductVariationMasterList
			.Cast<DataRowView>()
			.Select(drv => (string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID])
			.Contains(StringUtility.ToEmpty(this.VariationId));

		// Set default selected variation for panel
		if (isExistsVariationId == false)
		{
			this.VariationId = StringUtility.ToEmpty(this.ProductVariationMasterList[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
		}

		// Create product variation name list
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
		this.SelectedVariationName2 = (this.ProductVariationName2List.Count > 0)
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
		this.SelectedVariationName3 = (this.ProductVariationName3List.Count > 0)
			? (this.ProductVariationName3List.Contains(this.SelectedVariationName3) == false)
				? this.ProductVariationName3List[0]
				: this.SelectedVariationName3
			: string.Empty;
	}

	/// <summary>
	/// バリエーションの選択状態を保持
	/// </summary>
	protected void SetProductVariationStatus(DataRowView drvProduct, string strVariationId)
	{
		if (this.VariationId.Equals(strVariationId, StringComparison.OrdinalIgnoreCase))	// 仕様的にはvariation_idは大文字小文字区別しない
		{
			// 選択バリエーションを保持
			this.ProductMaster = drvProduct;

			// バリエーション選択状態
			this.VariationSelected = true;
		}
	}

	/// <summary>
	/// Set enable add cart
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
	/// 在庫数取得：MATRIX
	/// </summary>
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
				if ((this.WddlVariationSelect1.SelectedValue == "") || (this.WddlVariationSelect2.SelectedValue == ""))
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

	/// <summary>
	/// 商品タグ設定
	/// </summary>
	private void SetProductTagInfoForDisplay()
	{
		var productTag = DomainFacade.Instance.ProductTagService.GetTagSettingList();
		List<Hashtable> productTagList = new List<Hashtable>();
		foreach (DataRowView drv in productTag)
		{
			var productTagId = (string)drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID];
			var productTagName = Constants.GLOBAL_OPTION_ENABLE ? GetProductTranslationTagName(productTagId, (string)drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME]) : (string)drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME];
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTTAGSETTING_TAG_ID, productTagId},
				{Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME, productTagName},
			};
			productTagList.Add(ht);
		}
		this.ProductTagList = productTagList;
	}

	/// <summary>
	/// 商品タグ翻訳名取得
	/// </summary>
	/// <returns>商品翻訳名</returns>
	private string GetProductTranslationTagName(string tagId, string tagNameDefault)
	{
		// 翻訳名取得
		try
		{
			var productTranslationTagName = NameTranslationCommon.GetTranslationName(
				tagId,
				Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTTAGSETTING,
				Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTTAGSETTING_TAG_NAME,
				tagNameDefault,
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);

			return productTranslationTagName;
		}
		catch (Exception ex)
		{
			FileLogger.WriteError(ex);
			return tagNameDefault;
		}
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
	/// バリエーション変更共通処理
	/// </summary>
	private void ChangeVariation()
	{
		// バリエーション選択状態セット
		this.VariationSelected = (this.VariationId != "");

		// 商品データの画面設定にて入力済みの商品付帯情報がクリアされるため、一時退避
		var inputtedProductOptionValueSettings = GetInputtedProductOptionValueSettings();

		// 商品データを画面に設定
		SetProductDataForDisplay();

		// バリエーション更新対象をデータバインド
		DataBindByClass(this.Page, "ChangesByVariation");

		// 一時退避した入力済み商品付帯情報を再セット
		SetProductOptionValueSettings(inputtedProductOptionValueSettings);

		// 通知メール登録機能用のデータセット
		SetBodyProductArrivalMailRegisterVariationId();
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
	/// お気に入りへボタンクリック
	/// </summary>
	/// <param name="sender">送信情報</param>
	/// <param name="e">イベント情報</param>
	protected void lbAddFavorite_Click(object sender, EventArgs e)
	{
		// お気に入り登録
		AddToFavorite(this.ShopId, this.ProductId, this.CategoryId, String.Empty);

		this.FavoriteUserCount = SetFavoriteDataForDisplay(this.ProductId, string.Empty);

		Reload();
	}

	/// <summary>
	/// カートへボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCartAdd_Click(object sender, System.EventArgs e)
	{
		if(GetProductOptionValueSetting()) // 商品付帯情報の用意
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
				this.WtbCartAddProductCount.Text,
				this.WhfIsRedirectAfterAddProduct.Value,
				this.ProductOptionSettingList,
				this.ContentsLog);

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				this.ScoringSale = scoringSale;
			}

			AfterAddCart(true, errorMessage);
		}
	}

	/// <summary>
	/// カートボタン（定期購入）クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCartAddFixedPurchase_Click(object sender, EventArgs e)
	{
		if (GetProductOptionValueSetting()) // 商品付帯情報の用意
		{
			var scoringSale = this.ScoringSale;
			this.ScoringSale = null;
			var errorMessage = AddCart(
				Constants.AddCartKbn.FixedPurchase,
				this.WtbCartAddProductCount.Text,
				this.WhfIsRedirectAfterAddProduct.Value,
				this.ProductOptionSettingList,
				this.ContentsLog);

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				this.ScoringSale = scoringSale;
			}

			AfterAddCart(true, errorMessage);
		}
	}

	/// <summary>
	/// 頒布会申し込みクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCartAddSubscriptionBox_Click(object sender, EventArgs e)
	{
		// 商品付帯情報の用意
		if (GetProductOptionValueSetting())
		{
			SessionManager.ProductOptionSettingList = this.ProductOptionSettingList;

			var subscriptionBoxDetailUrl =
				new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_SUBSCRIPTIONBOX_DETAIL)
					.AddParam(Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_COURSE_ID, this.WddlSubscriptionBox.SelectedValue)
					.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, this.ProductId)
					.AddParam(Constants.REQUEST_KEY_VARIATION_ID, this.VariationId)
					.AddParam(Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_FOR_COURSE_LIST, Constants.REDIRECT_TO_SUBSCRIPTION_BOX_DETAIL_FROM_PRODUCT_DETAIL)
					.AddParam(Constants.REQUEST_KEY_ITEM_QUANTITY, WtbCartAddProductCount.Text.Trim())
					.CreateUrl();

			Response.Redirect(subscriptionBoxDetailUrl);
		}
	}

	/// <summary>
	/// カートボタン（ギフト購入）クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCartAddGift_Click(object sender, EventArgs e)
	{
		if(GetProductOptionValueSetting()) // 商品付帯情報の用意
		{
			string errorMessage = AddCart(Constants.AddCartKbn.GiftOrder, this.WtbCartAddProductCount.Text, WhfIsRedirectAfterAddProduct.Value, this.ProductOptionSettingList);
			AfterAddCart(true, errorMessage);
		}
	}

	/// <summary>
	/// リピータコマンド（カート投入バリエーション一覧）
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rAddCartVariationList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		// カート投入バリエーション一覧で選択されたバリエーションIDをプロパティにセット
		this.VariationId = ((HiddenField)e.Item.FindControl("hfVariationId")).Value;
		var wddlSubscriptionBox =  GetWrappedControl<WrappedDropDownList>(e.Item, "ddlSubscriptionBox");
		// 注文数を1固定、複数バリエーション選択カート投入
		string cartAddProductCount = "1";
		
		// カート投入エラーメッセージ
		string errorMessage = "";

		// 商品付帯情報の用意
		if(GetProductOptionValueSetting())
		{
			switch (e.CommandName)
			{
				// カート投入処理
				case "CartAdd":
					var addCartType = IsAddCartByGiftType()
						? Constants.AddCartKbn.GiftOrder
						: Constants.AddCartKbn.Normal;
					errorMessage = AddCart(
						addCartType,
						cartAddProductCount,
						this.WhfIsRedirectAfterAddProduct.Value,
						this.ProductOptionSettingList,
						this.ContentsLog);
					AfterAddCart(false, errorMessage);
					break;

				case "CartAddFixedPurchase":
					errorMessage = AddCart(
						Constants.AddCartKbn.FixedPurchase,
						cartAddProductCount,
						this.WhfIsRedirectAfterAddProduct.Value,
						this.ProductOptionSettingList,
						this.ContentsLog);
					AfterAddCart(false, errorMessage);
					break;

				case "CartAddSubscriptionBox":
					SessionManager.ProductOptionSettingList = this.ProductOptionSettingList;

					var subscriptionBoxDetailUrl =
						new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_SUBSCRIPTIONBOX_DETAIL)
							.AddParam(Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_COURSE_ID, wddlSubscriptionBox.SelectedValue)
							.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, this.ProductId)
							.AddParam(Constants.REQUEST_KEY_VARIATION_ID, this.VariationId)
							.AddParam(Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_FOR_COURSE_LIST, Constants.REDIRECT_TO_SUBSCRIPTION_BOX_DETAIL_FROM_PRODUCT_DETAIL)
							.AddParam(Constants.REQUEST_KEY_ITEM_QUANTITY, WtbCartAddProductCount.Text.Trim())
							.CreateUrl();

					Response.Redirect(subscriptionBoxDetailUrl);
					break;

				case "AddFavorite":
					AddToFavorite(this.ShopId, this.ProductId, this.CategoryId, this.VariationId);
					Reload();
					break;

				case "CartAddGift":
					errorMessage = AddCart(Constants.AddCartKbn.GiftOrder, cartAddProductCount, WhfIsRedirectAfterAddProduct.Value, this.ProductOptionSettingList);
					AfterAddCart(false, errorMessage);
					break;

				// 通知登録関連処理
				case "ArrivalMail":
					// 入荷通知メール一覧画面へ
					Response.Redirect(CreateRegistUserProductArrivalMailUrl(
						this.ShopId,
						this.ProductId,
						this.VariationId,
						((HiddenField)e.Item.FindControl("htArrivalMailKbn")).Value,
						this.RawUrl));
					break;

				case Constants.COMMAND_NAME_SMART_ARRIVALMAIL:
					ViewArrivalMailForm(e);
					break;
			}
		}
	}

	/// <summary>
	/// Variation name 1 click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbVariationName1List_OnClick(object sender, EventArgs e)
	{
		this.SelectedVariationName1 = ((LinkButton)sender).CommandArgument;
		ChangeVariation();
	}

	/// <summary>
	/// Variation name 2 click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbVariationName2List_OnClick(object sender, EventArgs e)
	{
		this.SelectedVariationName2 = ((LinkButton)sender).CommandArgument;
		ChangeVariation();
	}

	/// <summary>
	/// Variation name 3 click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbVariationName3List_OnClick(object sender, EventArgs e)
	{
		this.SelectedVariationName3 = ((LinkButton)sender).CommandArgument;
		ChangeVariation();
	}

	/// <summary>
	/// カート投入後の後処理
	/// </summary>
	/// <param name="singleSelectAddCart">単体カート投入有無（単体:true、複数:false）</param>
	/// <param name="errorMessage">カート投入処理時のエラーメッセージ</param>
	private void AfterAddCart(bool singleSelectAddCart, string errorMessage)
	{
		if (errorMessage == "") return;

		//------------------------------------------------------
		// 商品データを画面に設定
		//------------------------------------------------------
		SetProductDataForDisplay();

		if (singleSelectAddCart)
		{
			this.AlertMessage = errorMessage;
			this.Buyable = false;
		}
		else
		{
			List<Dictionary<string, object>> lProductVariationAddCartList = new List<Dictionary<string, object>>(this.ProductVariationAddCartList);
			foreach (Dictionary<string, object> dicVariationAddCart in this.ProductVariationAddCartList)
			{
				if ((string)dicVariationAddCart[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] == this.VariationId)
				{
					dicVariationAddCart["CanCart"] = false;
					dicVariationAddCart["CanFixedPurchase"] = false;
					dicVariationAddCart["CanGiftOrder"] = false;
					dicVariationAddCart["ErrorMessage"] = errorMessage;
					break;
				}
			}
		}

		// 画面全体をデータバインド
		this.DataBind();
	}

	/// <summary>
	/// 付帯情報の取得
	/// </summary>
	/// <returns>取得できたか</returns>
	private bool GetProductOptionValueSetting()
	{
		StringBuilder errorMessages = new StringBuilder();

		// 商品付帯情報取得
		foreach (RepeaterItem riProductOptionValueSetting in this.WrProductOptionValueSettings.Items)
		{
			var wrCblProductOptionValueSetting = GetWrappedControl<WrappedRepeater>(riProductOptionValueSetting, "rCblProductOptionValueSetting");
			var wddlProductOptionValueSetting = GetWrappedControl<WrappedDropDownList>(riProductOptionValueSetting, "ddlProductOptionValueSetting");
			var wtbProductOptionValueSetting = GetWrappedControl<WrappedTextBox>(riProductOptionValueSetting, "txtProductOptionValueSetting");
			var wlblProductOptionErrorMessage = GetWrappedControl<WrappedLabel>(riProductOptionValueSetting, "lblProductOptionErrorMessage");
			var wlblProductOptionCheckboxErrorMessage = GetWrappedControl<WrappedLabel>(riProductOptionValueSetting, "lblProductOptionCheckboxErrorMessage");
			var wlblProductOptionDropdownErrorMessage = GetWrappedControl<WrappedLabel>(riProductOptionValueSetting, "lblProductOptionDropdownErrorMessage");

			if (wrCblProductOptionValueSetting.Visible)
			{
				var productOptionSetting = this.ProductOptionSettingList.Items[riProductOptionValueSetting.ItemIndex];
				var sbSelectedValue = new StringBuilder();
				var checkBoxCount = 0;
				foreach (RepeaterItem riCheckBox in wrCblProductOptionValueSetting.Items)
				{
					var wcbProductOptionValueSetting = GetWrappedControl<WrappedCheckBox>(riCheckBox, "cbProductOptionValueSetting");
					if (sbSelectedValue.Length != 0)
					{
						sbSelectedValue.Append(Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE);
					}
					sbSelectedValue.Append(wcbProductOptionValueSetting.Checked ? wcbProductOptionValueSetting.Text : "");
					if (wcbProductOptionValueSetting.Checked) checkBoxCount++;
				}

				this.ProductOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].SelectedSettingValue = sbSelectedValue.ToString();
				wlblProductOptionCheckboxErrorMessage.Text = (checkBoxCount == 0) && productOptionSetting.IsNecessary
					? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OPTION_NOT_SELECTED_ITEM_ERROR).Replace("@@ 1 @@", productOptionSetting.ValueName)
					: string.Empty;
				errorMessages.Append(wlblProductOptionCheckboxErrorMessage.Text);
			}
			else if (wddlProductOptionValueSetting.Visible)
			{
				var productOptionSetting = this.ProductOptionSettingList.Items[riProductOptionValueSetting.ItemIndex];
				this.ProductOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].SelectedSettingValue
					= wddlProductOptionValueSetting.SelectedValue == ReplaceTag("@@DispText.variation_name_list.unselected@@")
						? string.Empty
						: wddlProductOptionValueSetting.SelectedValue;
				wlblProductOptionDropdownErrorMessage.Text = (wddlProductOptionValueSetting.SelectedIndex == 0) && productOptionSetting.IsNecessary
					? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OPTION_NOT_SELECTED_ITEM_ERROR).Replace("@@ 1 @@", productOptionSetting.ValueName)
					: string.Empty;
				errorMessages.Append(wlblProductOptionDropdownErrorMessage.Text);
			}
			else if (wtbProductOptionValueSetting.Visible)
			{
				ProductOptionSetting pos = this.ProductOptionSettingList.Items[riProductOptionValueSetting.ItemIndex];
				string checkKbn = "OptionValueValidate";

				// XML ドキュメントの検証を生成します。
				ValidatorXml validatorXML = pos.CreateValidatorXml(checkKbn);
				Hashtable param = new Hashtable();
				param[pos.ValueName] = wtbProductOptionValueSetting.Text;

				wlblProductOptionErrorMessage.Text = Validator.Validate(checkKbn, validatorXML.InnerXml, param);
				errorMessages.Append(wlblProductOptionErrorMessage.Text);

				// 設定値には全角スペースと全角：は入力させない
				if (wtbProductOptionValueSetting.Text.Contains('　') || wtbProductOptionValueSetting.Text.Contains('：'))
				{
					wlblProductOptionErrorMessage.Text += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OPTION_ERROR).Replace("@@ 1 @@", pos.ValueName);
					errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OPTION_ERROR).Replace("@@ 1 @@", pos.ValueName));
				}

				if ((errorMessages.Length == 0) && (string.IsNullOrWhiteSpace(wtbProductOptionValueSetting.Text) == false))
				{
					this.ProductOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].SelectedSettingValue = wtbProductOptionValueSetting.Text;
				}
			}
		}

		return string.IsNullOrEmpty(errorMessages.ToString());
	}

	/// <summary>
	/// 商品データ取得
	/// </summary>
	/// <param name="key">キー（フィールド）</param>
	/// <returns></returns>
	/// <returns>商品データ</returns>
	protected object GetProductData(string key)
	{
		return GetProductData(this.ProductMaster, key);
	}

	/// <summary>
	/// 商品タグ翻訳名称取得
	/// </summary>
	/// <param name="key">キー（タグID）</param>
	/// <returns>翻訳したタグ名称</returns>
	protected string GetProductTagName(string key)
	{
		var tagName = (string)this.ProductTagList.Where(id => (id[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID].ToString() == key))
			.Select(name => name[Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME]).FirstOrDefault();
		return (tagName ?? "");
	}

	/// <summary>
	/// 商品「会員ランク価格」数値取得（バリエーション共用）
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <returns>会員ランク価格</returns>
	public string GetProductMemberRankPrice(object objProduct)
	{
		return ProductPage.GetProductMemberRankPrice(objProduct, this.VariationSelected);
	}

	/// <summary>
	/// 商品説明取得(Text,Html判定）
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="strDescField">フィールド名</param>
	/// <returns>商品説明</returns>
	protected string GetProductDataHtml(string strDescField)
	{
		return GetProductDataHtml(this.ProductMaster, strDescField);
	}

	/// <summary>
	/// 入荷通知メール申込ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRequestUserProductArrivalMail_Click(object sender, System.EventArgs e)
	{
		// 入荷通知メール一覧画面へ
		Response.Redirect(CreateRegistUserProductArrivalMailUrl(
			this.ShopId,
			this.ProductId,
			this.VariationId,
			this.ArrivalMailKbn,
			this.RawUrl));
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
	/// ユーザーコントロールに選択中のバリエーションIDをセット
	/// </summary>
	private void SetBodyProductArrivalMailRegisterVariationId()
	{
		if(string.IsNullOrEmpty(this.VariationId)) return;

		var bpamrFormArrival = GetWrappedControl<WrappedControl>("ucBpamrArrival");
		var bpamrFormRelease = GetWrappedControl<WrappedControl>("ucBpamrRelease");
		var bpamrFormResale = GetWrappedControl<WrappedControl>("ucBpamrResale");
		if (bpamrFormArrival.HasInnerControl)
		{
			((ProductArrivalMailRegisterUserControl)(bpamrFormArrival.InnerControl)).VariationId = this.VariationId;
		}
		if (bpamrFormRelease.HasInnerControl)
		{
			((ProductArrivalMailRegisterUserControl)(bpamrFormRelease.InnerControl)).VariationId = this.VariationId;
		}
		if (bpamrFormResale.HasInnerControl)
		{
			((ProductArrivalMailRegisterUserControl)(bpamrFormResale.InnerControl)).VariationId = this.VariationId;
		}
	}

	/// <summary>
	/// モバイル用URL取得
	/// </summary>
	/// <returns></returns>
	protected string GetMobileUrl()
	{
		return ProductCommon.CreateMobileProductDetailUrl(Request.Url.Authority, this.ShopId, this.ProductId, this.VariationId, this.BrandId);
	}

	/// <summary>
	/// 商品詳細ページURL（絶対パス）取得
	/// </summary>
	/// <returns>商品詳細ページURL（絶対パス）</returns>
	protected string CreateProductDetailUrl()
	{
		StringBuilder sbProductDetailUrl = new StringBuilder();
		sbProductDetailUrl.Append(Constants.PROTOCOL_HTTP);
		sbProductDetailUrl.Append(Request.Url.Authority);
		sbProductDetailUrl.Append(ProductCommon.CreateProductDetailUrl(this.ShopId, this.CategoryId, this.BrandId, this.SearchWord, this.ProductId, this.ProductName, this.BrandName));

		return sbProductDetailUrl.ToString();
	}

	/// <summary>
	/// ツイッター投稿用URL取得
	/// </summary>
	/// <returns>ツイッター投稿用URL</returns>
	protected string CreateTwitterUrl(string strTwitterComment)
	{
		StringBuilder sbTwitterUrl = new StringBuilder();
		sbTwitterUrl.Append("http://twitter.com/share");
		sbTwitterUrl.Append("?text=").Append(HttpUtility.UrlEncode(strTwitterComment));
		sbTwitterUrl.Append("&url=").Append(HttpUtility.UrlEncode(CreateProductDetailUrl()));

		return sbTwitterUrl.ToString();
	}

	/// <summary>
	/// 入力済みの商品付帯情報を取得
	/// </summary>
	/// <returns>入力済み商品付帯情報</returns>
	private Dictionary<int, List<string>> GetInputtedProductOptionValueSettings()
	{
		// 入力済みの商品付帯情報を取得
		var inputtedProductOptionValueSettings = new Dictionary<int, List<string>>();
		foreach (RepeaterItem riProductOptionValueSetting in this.WrProductOptionValueSettings.Items)
		{
			var wrCblProductOptionValueSetting = GetWrappedControl<WrappedRepeater>(riProductOptionValueSetting, "rCblProductOptionValueSetting");
			var wddlProductOptionValueSetting = GetWrappedControl<WrappedDropDownList>(riProductOptionValueSetting, "ddlProductOptionValueSetting");
			var wtbProductOptionValueSetting = GetWrappedControl<WrappedTextBox>(riProductOptionValueSetting, "txtProductOptionValueSetting");
			var wlblProductOptionErrorMessage = GetWrappedControl<WrappedLabel>(riProductOptionValueSetting, "lblProductOptionErrorMessage");

			var InputtedProductOptionValueSetting = new List<string>();

			// チェックボックスの場合
			if (wrCblProductOptionValueSetting.Visible)
			{
				foreach (RepeaterItem riCheckBox in wrCblProductOptionValueSetting.Items)
				{
					var wcbProductOptionValueSetting = GetWrappedControl<WrappedCheckBox>(riCheckBox, "cbProductOptionValueSetting");
					if (wcbProductOptionValueSetting.Checked)
					{
						InputtedProductOptionValueSetting.Add(wcbProductOptionValueSetting.Text);
					}
				}
			}
			// ドロップダウンリストの場合
			else if (wddlProductOptionValueSetting.Visible)
			{
				InputtedProductOptionValueSetting.Add(wddlProductOptionValueSetting.SelectedValue);
			}
			// テキストボックスの場合
			else if (wtbProductOptionValueSetting.Visible)
			{
				InputtedProductOptionValueSetting.Add(wtbProductOptionValueSetting.Text);
			}

			inputtedProductOptionValueSettings.Add(riProductOptionValueSetting.ItemIndex, InputtedProductOptionValueSetting);
		}

		return inputtedProductOptionValueSettings;
	}

	/// <summary>
	/// 指定された値で商品付帯情報をセット
	/// </summary>
	/// <param name="inputtedProductOptionValueSettings">商品付帯情報</param>
	private void SetProductOptionValueSettings(Dictionary<int, List<string>> inputtedProductOptionValueSettings)
	{
		// 商品付帯情報が0件の場合、処理中止
		if (inputtedProductOptionValueSettings.Count == 0) return;

		// 商品付帯情報をセット
		foreach (RepeaterItem riProductOptionValueSetting in this.WrProductOptionValueSettings.Items)
		{
			var wrCblProductOptionValueSetting = GetWrappedControl<WrappedRepeater>(riProductOptionValueSetting, "rCblProductOptionValueSetting");
			var wddlProductOptionValueSetting = GetWrappedControl<WrappedDropDownList>(riProductOptionValueSetting, "ddlProductOptionValueSetting");
			var wtbProductOptionValueSetting = GetWrappedControl<WrappedTextBox>(riProductOptionValueSetting, "txtProductOptionValueSetting");
			var wlblProductOptionErrorMessage = GetWrappedControl<WrappedLabel>(riProductOptionValueSetting, "lblProductOptionErrorMessage");
			var inputtedProductOptionValueSetting = inputtedProductOptionValueSettings[riProductOptionValueSetting.ItemIndex];

			// チェックボックスの場合
			if (wrCblProductOptionValueSetting.Visible)
			{
				foreach (RepeaterItem riCheckBox in wrCblProductOptionValueSetting.Items)
				{
					var wcbProductOptionValueSetting = GetWrappedControl<WrappedCheckBox>(riCheckBox, "cbProductOptionValueSetting");
					if (inputtedProductOptionValueSetting.Contains(wcbProductOptionValueSetting.Text))
					{
						wcbProductOptionValueSetting.Checked = true;
					}
				}
			}
			// ドロップダウンリストの場合
			else if (wddlProductOptionValueSetting.Visible)
			{
				wddlProductOptionValueSetting.SelectedValue = inputtedProductOptionValueSetting[0];
			}
			// テキストボックスの場合
			else if (wtbProductOptionValueSetting.Visible)
			{
				wtbProductOptionValueSetting.Text = inputtedProductOptionValueSetting[0];
			}
		}
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
	/// 商品詳細CanonicalタグURL作成
	/// </summary>
	/// <returns>CanonicalタグURL</returns>
	protected string CreateProductDetailCanonicalUrl()
	{
		var canonicalUrl = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + CreateProductDetailUrl(this.ProductMaster, false);
		return canonicalUrl;
	}

	/// <summary>
	/// JAF会員ログインして購入URL作成
	/// </summary>
	/// <param name="isFixedPurchase">定期購入であるか</param>
	/// <returns>JAF会員ログインして購入URL</returns>
	protected string CreateUrlForJafLoginPurchase(bool isFixedPurchase = false)
	{
		var url = (this.IsLoggedIn)
			? CreateUrlForJafSingleSignOn(
				Constants.JAF_LOGIN_API_URL,
				this.ProductId,
				this.VariationId,
				isFixedPurchase)
			: new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN)
				.AddParam(Constants.REQUEST_KEY_NEXT_URL, this.NextUrl)
				.CreateUrl();
		return url;
	}

	/// <summary>
	/// rAddCartVariationList ItemDataBound handler
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rAddCartVariationList_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
		{
			var item = e.Item;
			var wddlSubscriptionBoxForVariation = GetWrappedControl<WrappedDropDownList>(item, "ddlSubscriptionBox");
			if (wddlSubscriptionBoxForVariation.HasInnerControl)
			{
				var wlbCartAddSubscriptionBoxList = GetWrappedControl<WrappedLinkButton>(item, "lbCartAddSubscriptionBoxList");
				var whfProduct = GetWrappedControl<WrappedHiddenField>(item, "hfProduct");
				var whfVariation = GetWrappedControl<WrappedHiddenField>(item, "hfVariation");
				var whfShopId = GetWrappedControl<WrappedHiddenField>(item, "htShopId");
				var whfSubscriptionBoxFlg =  GetWrappedControl<WrappedHiddenField>(item, "hfSubscriptionBoxFlg");
				var whfFixedPurchase =  GetWrappedControl<WrappedHiddenField>(item, "hfFixedPurchase");
				var whfCanFixedPurchase =  GetWrappedControl<WrappedHiddenField>(item, "hfCanFixedPurchase");
				var whgcNotSubscriptionBoxOnly = GetWrappedControl<WrappedHtmlGenericControl>(item, "plhNotSubscriptionBoxOnly");

				whgcNotSubscriptionBoxOnly.Visible = whfSubscriptionBoxFlg.Value != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_ONLY;

				var isSubscriptionBoxValid = ((whfSubscriptionBoxFlg.Value != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID)
					&& (whfFixedPurchase.Value != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && (whfCanFixedPurchase.Value == "True") && (this.IsUserFixedPurchaseAble));
				if (isSubscriptionBoxValid)
				{
					var subscriptionBoxes = GetAvailableSubscriptionBoxesByProductId(whfShopId.Value, whfProduct.Value, whfVariation.Value);
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
									if ((defaultProduct.VariationId
										!= this.ProductVariationAddCartList[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_ID].ToString())) continue;
									foreach (var selectProduct in subscriptionBox.SelectableProducts)
									{
										if ((selectProduct.VariationId
												== this.ProductVariationAddCartList[0][Constants
													.FIELD_PRODUCTVARIATION_VARIATION_ID].ToString())
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
									if (((product.TermSince <= DateTime.Now)
										&& (DateTime.Now < product.TermUntil))
										&& (product.VariationId == this.ProductVariationAddCartList[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_ID].ToString()))
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
								if ((selectProduct.VariationId
										== this.ProductVariationAddCartList[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_ID].ToString())
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
					wddlSubscriptionBoxForVariation.Visible = (subscriptionBoxedForDisplay.Count > 1);
					wddlSubscriptionBoxForVariation.DataSource = subscriptionBoxedForDisplay;
					wddlSubscriptionBoxForVariation.DataBind();
					if (subscriptionBoxedForDisplay.Count > 0)
					{
						var subscriptionBoxDisplayName = (HiddenField)item.FindControl("hfSubscriptionBoxDisplayName");
						subscriptionBoxDisplayName.Value = subscriptionBoxedForDisplay.First().DisplayName;
						wddlSubscriptionBoxForVariation.SelectedIndex = 0;
						wlbCartAddSubscriptionBoxList.Visible = true;
					}
					else
					{
						wlbCartAddSubscriptionBoxList.Visible = false;
					}
				}
				else
				{
					wddlSubscriptionBoxForVariation.Visible = wlbCartAddSubscriptionBoxList.Visible = false;
				}
			}
		}
	}

	/// <summary>
	/// 画面再描画
	/// </summary>
	protected void Reload()
	{
		// 商品データを画面に設定
		SetProductDataForDisplay();

		// お気に入りデータを画面に設定
		SetFavoriteDataForDisplay();

		// 画面全体をデータバインド
		DataBind();
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
	/// Create url for product zoom image
	/// </summary>
	/// <returns>Url for product zoom image</returns>
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

		// For the case preview product from manager site
		if (this.IsPreviewProductMode)
		{
			urlCreator.AddParam(Constants.REQUEST_KEY_PREVIEW_HASH, Request[Constants.REQUEST_KEY_PREVIEW_HASH])
				.AddParam(Constants.REQUEST_KEY_PREVIEW_GUID_STRING, PreviewGuidString);
		}
		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// レコメンドエンジン連携用の選択中の商品ID取得
	/// </summary>
	/// <returns>レコメンドエンジン連携用の選択中の商品ID</returns>
	public string GetRecommendProductId()
	{
		if (this.ProductMaster == null) return string.Empty;
		return (string)this.ProductMaster[Constants.FIELD_PRODUCT_RECOMMEND_PRODUCT_ID];
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

		if ((this.SelectedVariationName1 == "") || (this.SelectedVariationName2 == ""))return;
		foreach (DataRowView drvProduct in this.ProductVariationMasterList)
		{
			var variationName1 = (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
			var variationName2 = (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
			if (variationName1 == this.SelectedVariationName1 && variationName2 == this.SelectedVariationName2)return;
		}

		this.WlCombinationErrorMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USE_DOUBLEDROPDOWNLIST_COMBINATION_ERROR);
		this.WlbCartAdd.Enabled = false;
		this.WlbCartAddFixedPurchase.Enabled = false;
		this.WlbCartAddSubscriptionBox.Enabled = false;
		this.WlbCartAddForGift.Enabled = false;
		this.WlbRequestArrivalMail2.Enabled = false;
	}

	/// <summary>
	/// 商品付帯情報設定値一覧の最初にデフォルト値を追加（ドロップダウン形式）
	/// </summary>
	/// <param name="lic">商品付帯情報設定値一覧</param>
	/// <param name="isNecessary">必須かどうか</param>
	/// <returns>ドロップダウン形式の商品付帯情報設定値一覧</returns>
	/// <remarks>
	/// ※付帯価格オプション有効時のみ※<br/>
	/// ・デフォルト値："選択してください"<br/>
	/// ・ドロップダウン形式の商品付帯情報にて、設定値一覧（選択可能項目）の最初にデフォルト値を追加する<br/>
	/// </remarks>
	protected ListItemCollection InsertDefaultAtFirstToDdlProductOptionSettingList(ListItemCollection lic, bool isNecessary)
	{
		if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED && isNecessary == false)
		{
			lic.Insert(0, new ListItem(ReplaceTag("@@DispText.variation_name_list.unselected@@"), ""));
		}
		return lic;
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
	/// <summary>バリエーション毎のカート投入リスト</summary>
	protected List<Dictionary<string, object>> ProductVariationAddCartList { get; private set; }
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
		get { return (bool)ViewState["HasVariation"]; }
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
	/// <summary>お気に入りのユーザー登録数</summary>
	protected int FavoriteUserCount { get; private set; }
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
	/// <summary>Product variation name 1 list</summary>
	protected List<Dictionary<string, object>> ProductVariationList { get; private set; }
	/// <summary>Product variation name 1 list</summary>
	protected List<string> ProductVariationName1List
	{
		get { return (ViewState["ProductVariationName1List"] != null) ? (List<string>)ViewState["ProductVariationName1List"] : null; }
		set { ViewState["ProductVariationName1List"] = value; }
	}
	/// <summary>Product variation name 2 list</summary>
	protected List<string> ProductVariationName2List
	{
		get { return (ViewState["ProductVariationName2List"] != null) ? (List<string>)ViewState["ProductVariationName2List"] : null; }
		set { ViewState["ProductVariationName2List"] = value; }
	}
	/// <summary>Product variation name 3 list</summary>
	protected List<string> ProductVariationName3List
	{
		get { return (ViewState["ProductVariationName3List"] != null) ? (List<string>)ViewState["ProductVariationName3List"] : null; }
		set { ViewState["ProductVariationName3List"] = value; }
	}
	/// <summary>Is variation name 3</summary>
	protected bool IsVariationName3
	{
		get { return (ViewState["IsVariationName3"] != null) ? (bool)ViewState["IsVariationName3"] : false; }
		set { ViewState["IsVariationName3"] = value; }
	}
	/// <summary>Is selecting variation exist</summary>
	protected bool IsSelectingVariationExist
	{
		get { return (ViewState["IsSelectingVariationExist"] != null) ? (bool)ViewState["IsSelectingVariationExist"] : true; }
		set { ViewState["IsSelectingVariationExist"] = value; }
	}
	/// <summary>Alert mesage variation not exist</summary>
	protected string AlertMessageVariationNotExist
	{
		get { return (ViewState["AlertMessageVariationNotExist"] != null) ? (string)ViewState["AlertMessageVariationNotExist"] : string.Empty; }
		private set { ViewState["AlertMessageVariationNotExist"] = value; }
	}
	/// <summary>商品タグリスト</summary>
	protected List<Hashtable> ProductTagList
	{
		get { return (List<Hashtable>)ViewState["ProductTagList"]; }
		set { ViewState["ProductTagList"] = value; }
	}
	/// <summary>商品概要表示フラグ</summary>
	protected bool IsProductOutlineVisible
	{
		get { return string.IsNullOrEmpty(GetProductDataHtml(Constants.FIELD_PRODUCT_OUTLINE)) == false; }
	}
	/// <summary>商品詳細説明表示フラグ</summary>
	protected bool IsProductDetailVisible { 
		get 
		{ 
			return (string.IsNullOrEmpty(GetProductDataHtml(Constants.FIELD_PRODUCT_DESC_DETAIL1))
				&& string.IsNullOrEmpty(GetProductDataHtml(Constants.FIELD_PRODUCT_DESC_DETAIL2))
				&& string.IsNullOrEmpty(GetProductDataHtml(Constants.FIELD_PRODUCT_DESC_DETAIL3))
				&& string.IsNullOrEmpty(GetProductDataHtml(Constants.FIELD_PRODUCT_DESC_DETAIL4))) == false;
		} 
	}
	/// <summary>遷移後URL</summary>
	protected string NextUrl
	{
		get
		{
			// 注文完了画面だったら、次は強制的にTOPへ飛ばす
			if (Request.Url.AbsolutePath == (Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_COMPLETE))
			{
				return Constants.PATH_ROOT;
			}

			// 要求ページがログイン画面ではない場合、要求ページのURI絶対パスを遷移後URLとして返却
			if (Request.Url.AbsolutePath != (Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN))
			{
				return this.RawUrl;
			}

			// 既に遷移後URLが存在する場合、存在する遷移後URLを返却　そうでない場合、TOPページを遷移後URLとして返却
			return (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_NEXT_URL]) == false)
				? NextUrlValidation(Request[Constants.REQUEST_KEY_NEXT_URL])
				: Constants.PATH_ROOT;
		}
	}
	/// <summary>Scoring sale</summary>
	protected ScoringSaleInput ScoringSale
	{
		get { return (ScoringSaleInput)Session["ScoringSaleInput"]; }
		set { Session["ScoringSaleInput"] = value; }
	}
	/// <summary>Contents log</summary>
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
	/// <summary>配送料無料適応外文言表示か</summary>
	protected bool IsDisplayExcludeFreeShippingText
	{
		get { return Constants.FREE_SHIPPING_FEE_OPTION_ENABLED && ((string)this.ProductMaster[Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG] == Constants.FLG_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG_VALID); }
	}
}
