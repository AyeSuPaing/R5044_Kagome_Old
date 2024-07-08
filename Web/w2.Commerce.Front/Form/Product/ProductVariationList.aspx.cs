/*
=========================================================================================================
  Module      : 商品バリエーション一覧画面処理(ProductVariationList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Util;
using System.Linq;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Product;
using w2.Common.Web;

public partial class Form_Product_ProductVariationList : ProductPage
{
	/// <summary>リピートプラスONEリダイレクト必須判定</summary>
	public override bool RepeatPlusOneNeedsRedirect { get { return Constants.REPEATPLUSONE_OPTION_ENABLED; } }
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Http; } }	// Httpアクセス

	#region ラップ済みコントロール宣言
	WrappedHtmlGenericControl WspPager1 { get { return GetWrappedControl<WrappedHtmlGenericControl>("spPager1"); } }
	WrappedHtmlGenericControl WspPager2 { get { return GetWrappedControl<WrappedHtmlGenericControl>("spPager2"); } }
	WrappedHtmlGenericControl WdivClosedmarketLogin { get { return GetWrappedControl<WrappedHtmlGenericControl>("divClosedmarketLogin"); } }
	WrappedRepeater WrProductList { get { return GetWrappedControl<WrappedRepeater>("rProductList"); } }
	protected WrappedTextBox WtbClosedMarketPassword { get { return GetWrappedControl<WrappedTextBox>("tbClosedMarketPassword"); } }
	WrappedHiddenField WhfIsRedirectAfterAddProduct { get { return GetWrappedControl<WrappedHiddenField>("hfIsRedirectAfterAddProduct", ""); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエストよりパラメタ取得
			//------------------------------------------------------
			// 商品一覧共通処理
			GetParameters();

			// パラメタ取得
			this.ProductSaleId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTSALE_ID]);
			this.ProductSaleKbn = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTSALE_KBN]);

			//------------------------------------------------------
			// 表示制御
			//------------------------------------------------------
			// 闇市ログイン表示判定
			bool blDispClosedmarketLogin = true;
			switch (this.ProductSaleKbn)
			{
				// タイムセール
				case Constants.KBN_PRODUCTSALE_KBN_TIMESALES:
					blDispClosedmarketLogin = false;
					break;

				// 闇市
				case Constants.KBN_PRODUCTSALE_KBN_CLOSEDMARKET:
					if (Session[Constants.SESSION_KEY_CLOSEDMARKET_ID + this.ProductSaleId] != null)
					{
						blDispClosedmarketLogin = false;
					}
					break;

				// 上記以外の場合
				default:
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_IRREGULAR_PARAMETER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);

					break;
			}

			//------------------------------------------------------
			// 闇市ログインページ表示
			//------------------------------------------------------
			if (blDispClosedmarketLogin)
			{
				this.WrProductList.Visible = false;
				this.WspPager1.Visible = this.WspPager2.Visible = false;
			}
			else
			{
				this.WdivClosedmarketLogin.Visible = false;
				this.WspPager1.Visible = this.WspPager2.Visible = true;
			}

			if (blDispClosedmarketLogin == false)
			{
				//------------------------------------------------------
				// 商品一覧取得
				//------------------------------------------------------
				DataView dvProductList = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("ProductSale", "GetProductSaleList"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_PRODUCTSALE_SHOP_ID, this.ShopId);
					htInput.Add(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID, this.ProductSaleId);
					htInput.Add(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN, this.ProductSaleKbn);
					htInput.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_PRODUCTVARIATION_LIST * (this.PageNumber - 1) + 1);
					htInput.Add("end_row_num", Constants.CONST_DISP_CONTENTS_PRODUCTVARIATION_LIST * this.PageNumber);

					// 会員ランク
					htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, this.MemberRankId);

					dvProductList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
				}

				if (dvProductList.Count == 0)
				{
					this.WspPager1.Visible = this.WspPager2.Visible = false;

					this.AlertMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
				}

				this.ProductVariationList = GetVariationAddCartList(dvProductList);

				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					dvProductList = SetTranslationData(dvProductList);
				}

				// データバインド
				this.WrProductList.DataSource = dvProductList;
				this.WrProductList.DataBind();

				//------------------------------------------------------
				// ページャ作成（商品一覧処理で総件数を取得）
				//------------------------------------------------------
				int iTotalProductCounts = (dvProductList.Count != 0) ? (int)dvProductList[0].Row["row_count"] : 0;

				StringBuilder sbNextUrl = new StringBuilder();
				sbNextUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_PRODUCTVARIATION_LIST);
				sbNextUrl.Append("?").Append(Constants.REQUEST_KEY_SHOP_ID).Append("=").Append(HttpUtility.UrlEncode(this.ShopId));
				sbNextUrl.Append("&").Append(Constants.REQUEST_KEY_PRODUCTSALE_ID).Append("=").Append(HttpUtility.UrlEncode(this.ProductSaleId));
				sbNextUrl.Append("&").Append(Constants.REQUEST_KEY_PRODUCTSALE_KBN).Append("=").Append(HttpUtility.UrlEncode(this.ProductSaleKbn));
				this.PagerHtml = WebPager.CreateProductVariationListPager(iTotalProductCounts, this.PageNumber, sbNextUrl.ToString());
			}
		}

		//------------------------------------------------------
		// 購入可能会員ランクチェック
		//------------------------------------------------------
		string strMemberRankId = this.MemberRankId;
		foreach (RepeaterItem ri in this.WrProductList.Items)
		{
			var whfBuyableMemberRank = GetWrappedControl<WrappedHiddenField>(ri, "hfBuyableMemberRank");
			var wlbAddCart = GetWrappedControl<WrappedHtmlGenericControl>(ri, "lbAddCart");
			var whfProductId = GetWrappedControl<WrappedHiddenField>(ri, "hfProductId");
			var whfVariationId = GetWrappedControl<WrappedHiddenField>(ri, "hfVariationId");

			wlbAddCart.Visible = CanDisplayAddCart(whfProductId.Value, whfVariationId.Value);

			if (MemberRankOptionUtility.CheckMemberRankPermission(strMemberRankId, whfBuyableMemberRank.Value) == false)
			{
				var wsErrorMessage = GetWrappedControl<WrappedHtmlGenericControl>(ri, "sErrorMessage");
				var product = new ProductService().Get(this.ShopId, whfProductId.Value);

				wlbAddCart.Visible = false;
				wsErrorMessage.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_BUYABLE_MEMBER_RANK)
					.Replace("@@ 1 @@", product.Name)
					.Replace("@@ 2 @@", MemberRankOptionUtility.GetMemberRankName(whfBuyableMemberRank.Value));
			}
		}
	}

	/// <summary>
	/// Can Display Add Cart
	/// </summary>
	/// <param name="productId">Product Id</param>
	/// <param name="variationId">Variation Id</param>
	/// <returns>true: add cart visible false: otherwise</returns>
	protected bool CanDisplayAddCart(string productId, string variationId)
	{
		var productInfo = this.ProductVariationList.FirstOrDefault(product =>
			((StringUtility.ToEmpty(product[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID]) == productId)
			&& (StringUtility.ToEmpty(product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]) == variationId)));

		return (GetKeyValue(productInfo, "CanCart") != null) && (bool)GetKeyValue(productInfo, "CanCart");
	}

	/// <summary>
	/// 闇市ログインボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSubmitClosedMarketPassword_Click(object sender, EventArgs e)
	{
		var strErrorMessage = "";

		// 闇市ログイン（情報取得）
		DataView dvClosedMarketSaleList = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductSale", "LoginClosedMarketSaleList"))
		{
			var htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCTSALE_SHOP_ID, this.ShopId);
			htInput.Add(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID, this.ProductSaleId);
			htInput.Add(Constants.FIELD_PRODUCTSALE_CLOSEDMARKET_PASSWORD, this.WtbClosedMarketPassword.Text);

			dvClosedMarketSaleList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		//------------------------------------------------------
		// 闇市ログインチェック
		//------------------------------------------------------
		if (dvClosedMarketSaleList.Count == 0)
		{
			// パスワード無効
			strErrorMessage = WebMessages.ERRMSG_FRONT_CLOSEDMARKET_PASSOWRD_ERROR;
		}
		else if ((string)dvClosedMarketSaleList[0][Constants.FIELD_PRODUCTSALE_VALID_FLG] == Constants.FLG_PRODUCTSALE_VALID_FLG_INVALID)
		{
			// 闇市無効
			strErrorMessage = WebMessages.ERRMSG_FRONT_CLOSEDMARKET_INVALID_ERROR;
		}
		else if ((DateTime)dvClosedMarketSaleList[0][Constants.FIELD_PRODUCTSALE_DATE_BGN] > DateTime.Now)
		{
			// 闇市開催前
			strErrorMessage = WebMessages.ERRMSG_FRONT_CLOSEDMARKET_PREPARING_ERROR;
		}
		else if ((DateTime)dvClosedMarketSaleList[0][Constants.FIELD_PRODUCTSALE_DATE_END] < DateTime.Now)
		{
			// 闇市開催終了
			strErrorMessage = WebMessages.ERRMSG_FRONT_CLOSEDMARKET_ENDED_ERROR;
		}

		//------------------------------------------------------
		// 成功処理（セッションに結果を格納しリダイレクト）
		//------------------------------------------------------
		if (strErrorMessage == "")
		{
			Session[Constants.SESSION_KEY_CLOSEDMARKET_ID + this.ProductSaleId] = this.WtbClosedMarketPassword.Text;

			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_PRODUCTVARIATION_LIST
				+ "?" + Constants.REQUEST_KEY_SHOP_ID + "=" + this.ShopId
				+ "&" + Constants.REQUEST_KEY_PRODUCTSALE_ID + "=" + this.ProductSaleId
				+ "&" + Constants.REQUEST_KEY_PRODUCTSALE_KBN + "=" + Constants.KBN_PRODUCTSALE_KBN_CLOSEDMARKET);
		}
		//------------------------------------------------------
		// 失敗処理
		//------------------------------------------------------
		else
		{
			this.AlertMessage = WebMessages.GetMessages(strErrorMessage);
		}
	}

	/// <summary>
	/// リピータアイテムイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rProductList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		//------------------------------------------------------
		// カート投入
		//------------------------------------------------------
		if (e.CommandName == "AddCart")
		{
			// リピーター内のHiddenFieldをラップする
			var WhfProductId = GetWrappedControl<WrappedHiddenField>(this.WrProductList.Items[int.Parse((string)e.CommandArgument)], "hfProductId");
			var WhfVariationId = GetWrappedControl<WrappedHiddenField>(this.WrProductList.Items[int.Parse((string)e.CommandArgument)], "hfVariationId");

			//------------------------------------------------------
			// 商品情報取得（存在しない場合は投入しない）
			//------------------------------------------------------
			DataView dvProduct = GetProduct(this.ShopId, WhfProductId.Value, WhfVariationId.Value);
			if (dvProduct.Count == 0)
			{
				return;
			}

			// 商品状態チェック・エラー表示（商品セールは定期・ギフト未対応とする）
			OrderErrorcode oeProductError = OrderCommon.CheckProductStatus(dvProduct[0], 1, Constants.AddCartKbn.Normal, this.LoginUserId);
			if (oeProductError != OrderErrorcode.NoError)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG]
					= OrderCommon.GetErrorMessage(
						oeProductError,
						CreateProductJointName(dvProduct[0]),
						MemberRankOptionUtility.GetMemberRankName((string)dvProduct[0][Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK]));

				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			//------------------------------------------------------
			// カート投入
			//------------------------------------------------------
			// カートオブジェクトリストロード
			if (CanUseCartListLp() == false)
			{
				CartObjectList cartList = GetCartObjectList();
				// カート投入（商品セールは定期・ギフト未対応とする）
				cartList.AddProduct(dvProduct[0], Constants.AddCartKbn.Normal, this.ProductSaleId, 1, new w2.App.Common.Product.ProductOptionSettingList());

				// カート投入後の画面遷移
				switch (WhfIsRedirectAfterAddProduct.Value.ToUpper())
				{
					// カート一覧画面へ
					case Constants.KBN_REDIRECT_AFTER_ADDPRODUCT_CARTLIST:
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
						break;

					// 画面遷移しない
					default:
						break;
				}
			}
			else
			{
				var cartListLpUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST_LP)
					.AddParam(Constants.REQUEST_KEY_LPCART_PRODUCT_ID, StringUtility.ToEmpty(WhfProductId.Value))
					.AddParam(Constants.REQUEST_KEY_LPCART_VARIATION_ID, StringUtility.ToEmpty(WhfVariationId.Value))
					.AddParam(Constants.REQUEST_KEY_PRODUCT_SALE_ID, this.ProductSaleId)
					.AddParam(Constants.REQUEST_KEY_LPCART_ADD_CART_KBN, Constants.FLG_ADD_CART_KBN_NORMAL)
					.AddParam(Constants.REQUEST_KEY_LPCART_PRODUCT_COUNT, StringUtility.ToEmpty(dvProduct.Count))
					.CreateUrl();

				SessionManager.IsOnlyAddCartFirstTime = true;
				Response.Redirect(cartListLpUrl);
			}
		}
	}

	/// <summary>
	/// 購入可能会員ランクチェック
	/// </summary>
	/// <returns></returns>
	protected bool CheckBuyableMemberRank(string strProductRankId)
	{
		return MemberRankOptionUtility.CheckMemberRankPermission(this.MemberRankId, strProductRankId);
	}

	/// <summary>
	/// 翻訳情報設定
	/// </summary>
	/// <param name="productList">商品リスト</param>
	/// <returns>翻訳後商品リスト</returns>
	private DataView SetTranslationData(DataView productList)
	{
		var products = productList.Cast<DataRowView>().Select(
			drv => new ProductModel
			{
				ProductId = (string)drv[Constants.FIELD_PRODUCT_PRODUCT_ID]
			}).ToArray();
		productList = (DataView)NameTranslationCommon.Translate(productList, products);
		return productList;
	}

	/// <summary>商品セールID</summary>
	protected string ProductSaleId
	{
		get { return (string)ViewState["ProductSaleId"]; }
		private set { ViewState["ProductSaleId"] = value; }
	}
	/// <summary>商品セール区分</summary>
	protected string ProductSaleKbn
	{
		get { return (string)ViewState["ProductSaleKbn"]; }
		private set { ViewState["ProductSaleKbn"] = value; }
	}
	/// <summary>アラートメッセージ</summary>
	protected string AlertMessage
	{
		get { return (string)ViewState["AlertMessage"]; }
		private set { ViewState["AlertMessage"] = value; }
	}
	/// <summary>ページャーHTML</summary>
	protected string PagerHtml
	{
		get { return (string)ViewState["PagerHtml"]; }
		private set { ViewState["PagerHtml"] = value; }
	}
	/// <summary>Product Variation List</summary>
	protected List<Dictionary<string, object>> ProductVariationList 
	{
		get { return (List<Dictionary<string, object>>)ViewState["ProductVariationList"]; }
		private set { ViewState["ProductVariationList"] = value; }
	}
}
