/*
=========================================================================================================
  Module      : お気に入り一覧画面処理(FavoriteList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util.Security;
using w2.Domain.Product;
using w2.Domain.Favorite;

public partial class Form_Product_FavoriteList : ProductPage
{
	/// <summary>リピートプラスONEリダイレクト必須判定</summary>
	public override bool RepeatPlusOneNeedsRedirect { get { return Constants.REPEATPLUSONE_OPTION_ENABLED; } }
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示
	/// <summary>購入数</summary>
	private const string ORDER_QUANTITY = "1";

	#region ラップ済みコントロール宣言
	WrappedHtmlGenericControl WpNotFavoriteItemError { get { return GetWrappedControl<WrappedHtmlGenericControl>("pNotFavoriteItemError"); } }
	WrappedRepeater WrFavoriteList { get { return GetWrappedControl<WrappedRepeater>("rFavoriteList"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>
	/// ・初期遷移時
	///		→request「Constants.REQUEST_KEY_FAVORITE_KBN」にConstants.KBN_REQUEST_FAVORITE_ADD、
	///		  Constants.REQUEST_KEY_PRODUCT_IDに商品IDが設定されている場合
	///		  はお気に入り追加処理実行しお気に入り一覧を表示。そうでない場合はお気に入り一覧を表示
	///	・ポストバック時
	///		→お気に入り一覧を表示
	/// </remarks>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエストよりパラメタ取得（お気に入り一覧共通処理）
			//------------------------------------------------------
			GetParameters();

			if (string.IsNullOrEmpty(this.Request[Constants.REQUEST_KEY_FAV_TOKEN]) == false)
			{
				var favQuery = this.Request[Constants.REQUEST_KEY_FAV_TOKEN];
				var favSession = this.Session[Constants.SESSION_KEY_FAV_TOKEN];

				if (favQuery == (string)favSession)
				{
					var rcAuthenticationKey
						= new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
					var decrypt = rcAuthenticationKey.Decrypt(favQuery).Split(':');

					if (IsAlreadyRegisterFavorite(decrypt[0], this.LoginUserId, decrypt[1], this.VariationId) == false)
					{
						// お気に入り登録
						RegisterFavorite(decrypt[0], this.LoginUserId, decrypt[1], decrypt[2]);
					}
					this.Session[Constants.SESSION_KEY_FAV_TOKEN] = string.Empty;
				}
			}
			
			//------------------------------------------------------
			// セッションよりユーザID取得（お気に入り一覧共通処理）
			//------------------------------------------------------
			string userId = this.LoginUserId;

			//------------------------------------------------------
			// お気に入り一覧
			//------------------------------------------------------
			int totalFavoriteCounts;
			DataView favoriteList = new FavoriteService().GetFavoriteListAsDataView(userId, PageNumber, Constants.CONST_DISP_CONTENTS_DEFAULT_LIST);
			if (favoriteList.Count != 0)
			{
				totalFavoriteCounts = (int)favoriteList[0]["row_count"];

				// 0件でなければ、ページャーを設定
				this.PagerHtml = WebPager.CreateDefaultListPager(totalFavoriteCounts, this.PageNumber, Constants.PATH_ROOT + Constants.PAGE_FRONT_FAVORITE_LIST);
			}
			else
			{
				totalFavoriteCounts = 0;

				// 画面制御
				this.WrFavoriteList.Visible = false;

				// エラーメッセージ設定
				this.ErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FAVORITE_NO_ITEM);
			}

			if (Constants.VARIATION_FAVORITE_CORRESPONDENCE)
			{
				var count = 0;
				foreach (DataRowView product in favoriteList)
				{
					if ((string)product[Constants.FIELD_PRODUCT_VARIATION_ID] != "")
					{
						favoriteList[count][Constants.FIELD_PRODUCT_IMAGE_HEAD] = GetFavoriteImage(
							(string)product[Constants.FIELD_PRODUCT_SHOP_ID],
							(string)product[Constants.FIELD_PRODUCT_PRODUCT_ID],
							(string)product[Constants.FIELD_PRODUCT_VARIATION_ID]);

						favoriteList[count][Constants.FIELD_PRODUCT_NAME] += ProductCommon.CreateVariationName(
							(string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
							(string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2],
							(string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]);
					}
					count++;
				}
			}

			// 翻訳情報設定
			var products = favoriteList.Cast<DataRowView>().Select(drv => new ProductModel(drv)).ToArray();
			favoriteList = (DataView)NameTranslationCommon.Translate(favoriteList, products);

			// 商品バリエーション取得
			this.ProductVariationList = GetGroupedVariationList(GetVariationList(favoriteList, this.MemberRankId));

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			this.WrFavoriteList.DataSource = favoriteList;
			this.DataBind();
		}
	}

	/// <summary>
	/// お気に入り商品のID表示
	/// </summary>
	/// <param name="Item">商品</param>
	/// <returns>商品IDかバリエーションID</returns>
	protected string FavoriteDisplayId(object Item)
	{
		if (Constants.VARIATION_FAVORITE_CORRESPONDENCE)
		{
			return String.IsNullOrEmpty((string)GetKeyValue(Item, Constants.FIELD_FAVORITE_VARIATION_ID))
				? Constants.FIELD_FAVORITE_PRODUCT_ID
				: Constants.FIELD_FAVORITE_VARIATION_ID;
		}
		else
		{
			return Constants.FIELD_FAVORITE_PRODUCT_ID;
		}
	}

	/// <summary>
	/// バリエーション単位のお気に入り画像取得
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>お気に入り商品画像URL</returns>
	private static string GetFavoriteImage(string shopId, string productId, string variationId)
	{
		var favorite = new FavoriteModel()
		{
			ShopId = shopId,
			ProductId = productId,
			VariationId = variationId,
		};
		return new FavoriteService().GetFavoriteImage(favorite);
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender">オブジェクト</param>
	/// <param name="e">イベント</param>
	protected void lbDelete_Click(object sender, EventArgs e)
	{
		var argument = ((LinkButton) sender).CommandArgument.ToString().Split(';');
		var productId = argument[0];
		var variationId = argument[1];
		DeleteFromFavorite(Constants.CONST_DEFAULT_SHOP_ID, this.LoginUserId, productId, variationId);

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_FAVORITE_LIST);
	}

	/// <summary>
	/// お気に入り削除処理
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="userId">ユーザID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	public static void DeleteFromFavorite(string shopId, string userId, string productId, string variationId)
	{
		// モデル作成
		var favorite = new FavoriteModel()
		{
			ShopId = shopId,
			UserId = userId,
			ProductId = productId,
			VariationId = variationId,
		};

		// 削除
		new FavoriteService().Delete(favorite);
	}

	/// <summary>
	/// カートに入れるボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCartAdd_Click(object sender, EventArgs e)
	{
		SetProduct(sender);
		if (IsAddCartByGiftType())
		{
			lbCartAddGift_Click(sender, e);
			return;
		}
		AddCart(sender, Constants.AddCartKbn.Normal);
	}

	/// <summary>
	/// カートに入れる(定期購入)ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCartAddFixedPurchase_Click(object sender, EventArgs e)
	{
		AddCart(sender, Constants.AddCartKbn.FixedPurchase);
	}

	/// <summary>
	/// カートに入れる(ギフト購入)ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCartAddGift_Click(object sender, EventArgs e)
	{
		AddCart(sender, Constants.AddCartKbn.GiftOrder);
	}

	/// <summary>
	/// カート投入処理
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="addCartKbn">カート投入区分</param>
	private void AddCart(object product, Constants.AddCartKbn addCartKbn)
	{
		SetProduct(product);
		this.ErrorMessage = AddCart(addCartKbn, ORDER_QUANTITY, Constants.KBN_REDIRECT_AFTER_ADDPRODUCT_CARTLIST);
		Session[Constants.SESSION_KEY_ERROR_FOR_ADD_CART] = this.ErrorMessage;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
	}

	/// <summary>
	/// 商品情報をプロパティにセット
	/// </summary>
	/// <param name="product">商品情報</param>
	private void SetProduct(object product)
	{
		var argument = ((LinkButton)product).CommandArgument.Split(';');
		this.ProductId = argument[0];
		this.VariationId = (argument[1] == "") ? argument[0] : argument[1];
		this.ShopId = argument[2];
	}

	/// <summary>
	/// カートに入れるボタンを表示できる商品か
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>可否</returns>
	protected bool CheckAddCart(object product)
	{
		this.StockDisplay = false;
		this.StockMessage = "";

		// 販売期間か
		if (ProductCommon.IsSellTerm(product) == false) return false;

		// バリエーションチェック
		// variationフラグがオンでもvariationがない場合はOK
		var productVariation = (List<DataRowView>)GetKeyValue(this.ProductVariationList, (string)GetKeyValue(product, Constants.FIELD_PRODUCT_PRODUCT_ID));
		if (((string)GetKeyValue(product, Constants.FIELD_PRODUCT_VALID_FLG) == Constants.FLG_PRODUCT_VALID_FLG_VALID)
			&& ((string)GetKeyValue(product, Constants.FIELD_PRODUCT_VARIATION_ID) == string.Empty))
		{
			var result = productVariation.Where(
				variation => ((string)variation[Constants.FIELD_PRODUCT_PRODUCT_ID]
					!= (string)variation[Constants.FIELD_PRODUCT_VARIATION_ID]));
			if (result.Any()) { return false; }
		}

		// 付帯情報チェック
		if ((string)GetKeyValue(product, Constants.FIELD_PRODUCT_PRODUCT_OPTION_SETTINGS) != string.Empty)
		{
			return false;
		}

		// 在庫表示チェック
		var productStockManagementKbn = (string)GetKeyValue(product, Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN);
		if (productStockManagementKbn != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
		{
			this.StockDisplay = true;

			// 在庫文言を取得
			var productVariationStockInfos = new ProductService().GetProductVariationStockInfos(
				this.ShopId,
				(string)GetKeyValue(product, Constants.FIELD_PRODUCT_PRODUCT_ID),
				this.LoginUserMemberRankId);
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				NameTranslationCommon.SetStockTranslationData(ref productVariationStockInfos, this.ProductId);
			}

			foreach (var productStock in productVariationStockInfos)
			{
				var variationId = productStock.VariationId;
				var stockMessage = productStock.StockMessage;

				if ((variationId == (string)GetKeyValue(product, Constants.FIELD_PRODUCT_VARIATION_ID))
					|| (variationId == (string)GetKeyValue(product, Constants.FIELD_PRODUCT_PRODUCT_ID)))
				{
					this.StockMessage = stockMessage;
				}

				// 在庫が0個以下の時販売しない商品か
				if ((productStockManagementKbn == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYNG)
					&& ((int)GetKeyValue(product, Constants.FIELD_PRODUCTSTOCK_STOCK) <= 0))
				{
					return false;
				}
			}
		}

		// カート投入できるか
		this.CanAddCart = (Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED
				&& ((string)GetKeyValue(product, Constants.FIELD_PRODUCT_GIFT_FLG) != Constants.FLG_PRODUCT_GIFT_FLG_INVALID))
			|| (((string)GetKeyValue(product, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY)
				&& ((string)GetKeyValue(product, Constants.FIELD_PRODUCT_GIFT_FLG) != Constants.FLG_PRODUCT_GIFT_FLG_ONLY));
		this.CanFixedPurchase = Constants.FIXEDPURCHASE_OPTION_ENABLED
			&& ((string)GetKeyValue(product, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG)
				!= Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)
			&& ((string)GetKeyValue(product, "shipping_" + Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG)
				== Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID);
		this.CanGiftOrder = (Constants.GIFTORDER_OPTION_ENABLED
			&& ((string)GetKeyValue(product, Constants.FIELD_PRODUCT_GIFT_FLG) != Constants.FLG_PRODUCT_GIFT_FLG_INVALID)
			&& (Constants.CART_LIST_LP_OPTION == false)
			&& (Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED == false));

		return true;
	}

	#region レコナイズ削除予定記述
	/// <summary>
	/// レコナイズ用お気に入り商品ID取得
	/// </summary>
	/// <returns>お気に入り商品の商品ID</returns>
	/// <remarks>
	/// ・商品IDの頭に"P"をつける
	/// ・カンマ区切りで並べる
	/// ・最大10件
	/// </remarks>
	protected string GetFavoriteProductsForReconize()
	{
		//return RecommendReconize.GetFavoriteProductsForReconize(this.LoginUserId);
		//削除予定処理（マイナーバージョンアップでエラー防止のためメソッドは残す）
		return "";
	}
	#endregion

	/// <summary>商品バリエーションリスト</summary>
	protected Dictionary<string, List<DataRowView>> ProductVariationList { get; set; }
	/// <summary>ページャーHTML</summary>
	protected string PagerHtml 
	{
		get { return (string)ViewState["PagerHtml"]; }
		private set { ViewState["PagerHtml"] = value; }
	}
	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage 
	{
		get { return (string)ViewState["ErrorMessage"]; }
		private set { ViewState["ErrorMessage"] = value; }
	}
	/// <summary>カートに追加できるか</summary>
	protected bool CanAddCart { get; set; }
	/// <summary>定期購入でカートに追加できるか</summary>
	protected bool CanFixedPurchase { get; set; }
	/// <summary>ギフトでカートに追加できるか</summary>
	protected bool CanGiftOrder { get; set; }
	/// <summary>在庫文言</summary>
	protected string StockMessage { get; set; }
	/// <summary>在庫を表示するか</summary>
	protected bool StockDisplay { get; set; }
}
