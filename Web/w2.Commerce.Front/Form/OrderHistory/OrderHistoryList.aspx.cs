/*
=========================================================================================================
  Module      : 注文履歴一覧画面処理(OrderHistoryList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.CrossPoint.OrderHistory;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.Common.Web;
using w2.Domain.Order;
using w2.Common.Extensions;
using w2.App.Common.Order;
using w2.App.Common.Web.WrappedContols;

public partial class Form_Order_OrderHistoryList : OrderPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示

	#region  ラップ済みコントロール宣言
	WrappedHtmlGenericControl WpInfo { get { return GetWrappedControl<WrappedHtmlGenericControl>("pInfo"); } }
	WrappedHtmlGenericControl WspAlert { get { return GetWrappedControl<WrappedHtmlGenericControl>("spAlert"); } }
	WrappedRepeater WrOrderList { get { return GetWrappedControl<WrappedRepeater>("rOrderList"); } }
	WrappedRepeater WrOrderProductsList { get { return GetWrappedControl<WrappedRepeater>("rOrderProductsList"); } }
	WrappedHiddenField WhfIsRedirectAfterAddProduct { get { return GetWrappedControl<WrappedHiddenField>("hfIsRedirectAfterAddProduct", ""); } }
	#endregion

	protected const string REQUEST_KEY_DISPLAY_TYPE = "disp"; // 表示タイプ
	protected const string FLG_DISPLAY_KBN_ORDERS = "0";  // 注文ID単位で表示
	protected const string FLG_DISPLAY_KBN_PRODUCTS = "1"; // 商品単位で表示
	protected const string CONST_ORDER_ITEM_VARIATION_NAME = "orderitem_variation_name"; // 注文商品の商品名（SQL側で定義）

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			Display();
		}
	}

	/// <summary>
	/// 初期表示
	/// </summary>
	private void Display()
	{
		int totalCounts = 0;			// ページング可能項目数
		int iCurrentPageNumber = 1;		// カレントページ番号
		string displayType = FLG_DISPLAY_KBN_ORDERS; // 表示切替用

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		// ページ番号（ページャ動作時のみもちまわる）
		try
		{
			if (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]) == "")
			{
			}
			else
			{
				iCurrentPageNumber = int.Parse(Request[Constants.REQUEST_KEY_PAGE_NO].ToString());
			}

			if (StringUtility.ToEmpty(Request[REQUEST_KEY_DISPLAY_TYPE]) == "")
			{
				displayType = FLG_DISPLAY_KBN_ORDERS;
			}
			else
			{
				displayType = Request[REQUEST_KEY_DISPLAY_TYPE].ToString();
			}
		}
		catch
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		var bgnColumnNum = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iCurrentPageNumber - 1) + 1;
		var endColumnNum = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iCurrentPageNumber;

		if (displayType == FLG_DISPLAY_KBN_ORDERS)
		{
			// 商品単位のリストを表示しない
			this.WrOrderProductsList.Visible = false;

			//------------------------------------------------------
			// 一覧取得 (注文ID単位)
			//------------------------------------------------------

			// データソースセット
			var orderHistoryList = new OrderService().GetOrderHistoryListByOrdersInDataView(
				this.LoginUserId,
				bgnColumnNum,
				endColumnNum);
			this.WrOrderList.DataSource = orderHistoryList;

			//------------------------------------------------------
			// 総件数取得 (注文ID単位)
			//------------------------------------------------------
			totalCounts = (orderHistoryList.Count != 0) ? int.Parse(orderHistoryList[0].Row["row_count"].ToString()) : 0;
		}
		else
		{
			// 注文ID単位のリストを表示しない
			WrOrderList.Visible = false;

			//------------------------------------------------------
			// 一覧取得 (商品単位)
			//------------------------------------------------------
			var orderProductsList = new OrderService().GetOrderHistoryListByProductsInDataView(
				this.LoginUserId,
				this.MemberRankId,
				bgnColumnNum,
				endColumnNum);

			// 翻訳情報設定
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				SetOrderProductsListTranslationData(orderProductsList);
			}

			// データソースセット
			this.WrOrderProductsList.DataSource = orderProductsList; // DataViewの情報
			this.ProductVariationMasterList = GetVariationAddCartList(orderProductsList); // 画面表示時の購入可否の判定に利用

			// 総件数取得 (商品単位)
			totalCounts = (orderProductsList.Count != 0) ? int.Parse(orderProductsList[0].Row["row_count"].ToString()) : 0;
		}

		// 以下共通処理
		// 表示するものが無い場合の処理
		if (totalCounts == 0)
		{
			this.WpInfo.Visible = false;
			this.WrOrderList.Visible = false;
			this.WrOrderProductsList.Visible = false;
			this.AlertMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDERHISTORY_NO_ITEM);
		}

		//------------------------------------------------------
		// ページャ作成（一覧取得処理で総件数を取得している必要あり）
		//------------------------------------------------------
		var nextUrl = Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_LIST;
		nextUrl += "?" + REQUEST_KEY_DISPLAY_TYPE + "=" + displayType;
		this.PagerHtml = WebPager.CreateDefaultListPager(totalCounts, iCurrentPageNumber, nextUrl);

		//------------------------------------------------------
		// 表示切替URL作成
		//------------------------------------------------------
		var reverseType = (displayType == FLG_DISPLAY_KBN_ORDERS) ? FLG_DISPLAY_KBN_PRODUCTS : FLG_DISPLAY_KBN_ORDERS; // 異なる表示のパラメタを決定する
		this.UrlChangeDisplayType = Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_LIST + "?" + REQUEST_KEY_DISPLAY_TYPE + "=" + reverseType;

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		this.WrOrderList.DataBind();
		this.WrOrderProductsList.DataBind();
	}

	/// <summary>
	/// リピータコマンド（カート投入バリエーション一覧）
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void AddCartVariationList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		// カート投入バリエーション一覧で選択されたバリエーションIDをプロパティにセット
		this.ProductId = ((HiddenField)e.Item.FindControl("hfProductId")).Value;
		this.VariationId = ((HiddenField)e.Item.FindControl("hfVariationId")).Value;

		var dvProduct = GetProduct(this.ShopId, this.ProductId, this.VariationId);
		if (string.IsNullOrEmpty((string)dvProduct[0][Constants.FIELD_PRODUCT_PRODUCT_OPTION_SETTINGS]) == false)
		{
			var productDetailUrl = CreateProductDetailVariationUrl(dvProduct[0]);
			if (string.IsNullOrEmpty(productDetailUrl) == false)
			{
				Response.Redirect(productDetailUrl);
			}
		}
		// 注文数を1固定、複数バリエーション選択カート投入
		string cartAddProductCount = "1";

		// カート投入処理
		switch (e.CommandName)
		{
			case "CartAdd":
				this.AlertMessage = AddCart(Constants.AddCartKbn.Normal, cartAddProductCount, WhfIsRedirectAfterAddProduct.Value);
				break;

			case "CartAddFixedPurchase":
				this.AlertMessage = AddCart(Constants.AddCartKbn.FixedPurchase, cartAddProductCount, WhfIsRedirectAfterAddProduct.Value);
				break;

			case "CartAddGift":
				this.AlertMessage = AddCart(Constants.AddCartKbn.GiftOrder, cartAddProductCount, WhfIsRedirectAfterAddProduct.Value);
				break;

			// 通知登録関連処理
			case "ArrivalMail":
				// 入荷通知メール一覧画面へ
				Response.Redirect(CreateRegistUserProductArrivalMailUrl(
					this.ShopId,
					this.ProductId,
					this.VariationId,
					((HiddenField)e.Item.FindControl("hfArrivalMailKbn")).Value,
					this.RawUrl));
				break;

			case "SmartArrivalMail":
				ViewArrivalMailForm(e);
				break;
		}
	}

	/// <summary>
	/// バリエーション情報を取得する
	/// </summary>
	/// <param name="index">商品を特定するためのインデックス</param>
	/// <param name="keyString">取得したい値のキー</param>
	/// <returns></returns>
	protected object GetHistoryItemKeyValue(int index, string keyString)
	{
		return ProductVariationMasterList[index][keyString];
	}

	/// <summary>
	/// 注文商品一覧翻訳情報設定
	/// </summary>
	/// <param name="orderProductsList">注文商品一覧</param>
	/// <returns>注文商品一覧翻訳情報</returns>
	private void SetOrderProductsListTranslationData(DataView orderProductsList)
	{
		foreach (DataRowView product in orderProductsList)
		{
			product[CONST_ORDER_ITEM_VARIATION_NAME] = NameTranslationCommon.GetOrderItemProductTranslationName(
				(string)product[CONST_ORDER_ITEM_VARIATION_NAME],
				(string)product[Constants.FIELD_ORDERITEM_PRODUCT_ID],
				(string)product[Constants.FIELD_ORDERITEM_VARIATION_ID],
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);
		}
	}

	/// <summary>
	/// Is pickup real shop
	/// </summary>
	/// <param name="order">Order repeater object item</param>
	/// <returns>Is pickup real shop order</returns>
	protected bool IsPickupRealShop(OrderModel order)
	{
		return string.IsNullOrEmpty(order.StorePickupStatus) == false;
	}

	/// <summary>ページャーHTML</summary>
	protected string PagerHtml
	{
		get { return (string)ViewState["PagerHtml"]; }
		private set { ViewState["PagerHtml"] = value; }
	}
	/// <summary>アラートメッセージ</summary>
	protected string AlertMessage
	{
		get { return (string)ViewState["AlertMessage"]; }
		private set { ViewState["AlertMessage"] = value; }
	}
	/// <summary> 商品バリエーション情報 </summary>
	protected List<Dictionary<string, object>> ProductVariationMasterList { get; set; }
	/// <summary> 表示切替用のURL </summary>
	protected string UrlChangeDisplayType { get; set; }
}

