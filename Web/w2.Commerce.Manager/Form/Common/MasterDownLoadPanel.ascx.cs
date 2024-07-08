/*
=========================================================================================================
  Module      : マスタダウンロード系の出力コントローラ(MasterDownLoadPanel.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Manager.Menu;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.Domain.MasterExportSetting;

/// <summary>
/// マスタ出力用パネルコントロール
/// </summary>
public partial class Form_Common_MasterDownLoadPanel : BaseUserControl
{
	#region 列挙体
	/// <summary>
	/// マスタダウンロード種別
	/// </summary>
	public enum MasterDownloadType
	{
		/// <summary>商品</summary>
		Product,
		/// <summary>商品バリエーション</summary>
		//ProductVariation,
		/// <summary>商品タグ</summary>
		//ProductTag,
		/// <summary>商品拡張項目</summary>
		//ProductExtend,
		/// <summary>商品在庫</summary>
		ProductStock,
		/// <summary>商品カテゴリー</summary>
		ProductCategory,
		/// <summary>商品価格</summary>
		//ProductPrice,
		/// <summary>注文</summary>
		Order,
		/// <summary>注文商品</summary>
		//OrderItem,
		/// <summary>注文セットプロモーション</summary>
		//OrderSetPromotion,
		/// <summary>注文ワークフロー</summary>
		OrderWorkflow,
		/// <summary>注文商品ワークフロー</summary>
		//OrderItemWorkflow,
		/// <summary>注文セットプロモーションワークフロー</summary>
		//OrderSetPromotionWorkflow,
		/// <summary>ユーザー</summary>
		User,
		/// <summary>発注情報</summary>
		StockOrder,
		/// <summary>発注商品情報</summary>
		//StockOrderItem,
		/// <summary>商品+バリエーション</summary>
		//ProductView,
		/// <summary>モール出品設定情報</summary>
		//MallExhibitsConfig,
		/// <summary>入荷通知メール情報</summary>
		UserProductArrivalMail,
		/// <summary>商品レビュー</summary>
		ProductReview,
		/// <summary>商品セール価格</summary>
		ProductSalePrice,
		/// <summary>ショートURL</summary>
		ShortUrl,
		/// <summary>定期購入マスタ</summary>
		FixedPurchase,
		/// <summary>定期購入ワークフロー</summary>
		FixedPurchaseWorkflow,
		/// <summary>定期購入商品マスタ</summary>
		//FixedPurchaseItem,
		/// <summary>シリアルキーマスタ</summary>
		SerialKey,
		/// <summary>メッセージマスタ</summary>
		//CsMessage,
		/// <summary>リアル店舗マスタ</summary>
		RealShop,
		/// <summary>リアル店舗商品在庫マスタ</summary>
		RealShopProductStock,
		/// <summary>オペレータマスタ</summary>
		Operator,
		/// <summary>データ結合マスタ</summary>
		DataBinding
	}
	#endregion

	#region イベント
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// ドロップダウンのバインド
			DdlBind();

			// ドロップダウンに一個もアイテムなければ非表示
			this.tMasterDownload.Visible = ddlExportSetting.Items.Count > 1;

			this.DataBind();
		}
	}

	/// <summary>
	/// 出力ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnExport_Click(object sender, EventArgs e)
	{
		// 選択してなければエラー画面へ
		if (string.IsNullOrEmpty(ddlExportSetting.SelectedValue))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MASTEREXPORTSETTING_SETTING_ID_NOT_SELECTED);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		Hashtable param = new Hashtable();

		// イベントがある場合のみ実行
		if (OnCreateSearchInputParams != null)
		{
			param = OnCreateSearchInputParams();
		}

		param[Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID] = int.Parse(ddlExportSetting.SelectedValue.Split('-')[0]) - 1;
		param[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN] = ddlExportSetting.SelectedValue.Split('-')[1];
		Session[Constants.SESSION_KEY_PARAM] = param;

		// 出力ページへ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MASTEREXPORT);
	}
	#endregion

	#region -DdlBind ドロップダウンバインド
	/// <summary>
	/// ドロップダウンバインド
	/// </summary>
	private void DdlBind()
	{
		this.ddlExportSetting.Items.Add(new ListItem("", ""));
		this.ddlExportSetting.Items.AddRange(this.CreateDdlItems(this.DownloadType));
		this.ddlExportSetting.DataBind();
	}
	#endregion

	#region -CreateDdlItems ドロップダウン用のアイテム生成
	/// <summary>
	/// ドロップダウン用のアイテム生成
	/// </summary>
	/// <param name="type">ダウンロード種別</param>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItems(MasterDownloadType type)
	{
		switch (type)
		{
			case MasterDownloadType.Product:
				return CreateDdlItemForProduct();

			case MasterDownloadType.ProductStock:
				return CreateDdlItemForProductStock();

			case MasterDownloadType.ProductCategory:
				return CreateDdlItemForProductCategory();

			case MasterDownloadType.ProductReview:
				return CreateDdlItemForProductReview();

			case MasterDownloadType.Order:
				return CreateDdlItemForOrder();

			case MasterDownloadType.OrderWorkflow:
				return CreateDdlItemForOrderWorkFlow();

			case MasterDownloadType.User:
				return CreateDdlItemForUser();

			case MasterDownloadType.StockOrder:
				return CreateDdlItemForStockOrder();

			case MasterDownloadType.UserProductArrivalMail:
				return CreateDdlItemForUserProductArrivalMail();

			case MasterDownloadType.ProductSalePrice:
				return CreateDdlItemForProductSalePrice();

			case MasterDownloadType.ShortUrl:
				return CreateDdlItemForShortUrl();

			case MasterDownloadType.FixedPurchase:
				return CreateDdlItemForFixedPurchase();

			case MasterDownloadType.FixedPurchaseWorkflow:
				return CreateDdlItemFixedPurchaseWorkFlow();

			case MasterDownloadType.SerialKey:
				return CreateDdlItemForSerialKey();

			case MasterDownloadType.RealShop:
				return CreateDdlItemForRealShop();

			case MasterDownloadType.RealShopProductStock:
				return CreateDdlItemForRealShopProductStock();
		}

		return new ListItem[] { };
	}

	/// <summary>
	/// ドロップダウン用のアイテム生成
	/// </summary>
	/// <param name="masterKbn">DBではなくSQL振り分けのためのマスター種別</param>
	/// <param name="dbMasterKbn">DB上のマスター種別</param>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItems(string masterKbn, string dbMasterKbn)
	{
		var valueText = ValueText.GetValueText(
			Constants.TABLE_MASTEREXPORTSETTING,
			Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN,
			dbMasterKbn);
		var result = new MasterExportSettingService()
			.GetAllByMaster(this.LoginOperatorShopId, dbMasterKbn)
			.Select(item => CreateDdlItem(item, valueText, masterKbn))
			.ToArray();

		return result;
	}
	#endregion

	#region -CreateDdlItem ドロップダウン用のアイテム生成（単一)
	/// <summary>
	/// ドロップダウン用のアイテム生成（単一)
	/// </summary>
	/// <param name="setting">設定ID</param>
	/// <param name="exportName">出力名</param>
	/// <param name="masterKbn">DBではなくSQL振り分けのためのマスター種別</param>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem CreateDdlItem(MasterExportSettingModel setting, string exportName, string masterKbn)
	{
		return new ListItem(
			(setting.SettingId == Constants.MASTEREXPORTSETTING_MASTER_SETTING_ID) ? string.Format("{0}){0}", exportName) : string.Format("{0}){1}", exportName, setting.SettingName),
			string.Format("{0}-{1}", setting.SettingId, masterKbn));
	}
	#endregion

	#region -CreateDdlItemFor*** ドロップダウン用のアイテム生成（個別)
	/// <summary>
	/// ダウンロード種別が商品の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForProduct()
	{
		var result = new List<ListItem>();

		// 商品
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_PRODUCT_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCT, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCT));
		}

		// 商品バリエーション
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_PRODUCT_VAL_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTVARIATION, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTVARIATION));
		}

		// 商品価格
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_PRODUCT_PRICE_DL) && Constants.MEMBER_RANK_OPTION_ENABLED)
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTPRICE, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTPRICE));
		}

		// 商品タグ
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_PRODUCT_TAG_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTTAG, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTTAG));
		}

		// 商品拡張項目
		if (Constants.MALLCOOPERATION_OPTION_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_PRODUCT_EXD_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTEXTEND, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTEXTEND));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別が商品在庫の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForProductStock()
	{
		var result = new List<ListItem>();

		// 商品在庫
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_PRODUCTSTK_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSTOCK, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSTOCK));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別が商品カテゴリの場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForProductCategory()
	{
		var result = new List<ListItem>();

		// 商品カテゴリ
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_PRODUCTCAT_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTCATEGORY, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTCATEGORY));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別が商品レビューの場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForProductReview()
	{
		var result = new List<ListItem>();

		// 商品レビュー
		if (Constants.PRODUCTREVIEW_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_PRODUCTREVIEW_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTREVIEW, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTREVIEW));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別が注文の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForOrder()
	{
		var result = new List<ListItem>();

		// 注文
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ORDER_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER));
		}
		// 注文商品
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ORDER_ITEM_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM));
		}
		// 注文セットプロモーション
		if (Constants.SETPROMOTION_OPTION_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ORDER_SETPROMOTION_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION));
		}
		//データ結合
		if (Constants.MASTEREXPORT_DATABINDING_OPTION_ENABLE && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_DATABINDING_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別が注文の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForOrderWorkFlow()
	{
		var result = new List<ListItem>();

		// 注文
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ORDERWF_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER_WORKFLOW, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER));
		}
		// 注文商品
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ORDERWF_ITEM_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM_WORKFLOW, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM));
		}
		// 注文セットプロモーション
		if (Constants.SETPROMOTION_OPTION_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ORDERWF_SETPROMOTION_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION_WORKFLOW, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION));
		}
		//データ結合
		if (Constants.MASTEREXPORT_DATABINDING_OPTION_ENABLE && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_DATABINDING_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別がユーザーの場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForUser()
	{
		var result = new List<ListItem>();

		// ユーザー
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_USER_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER));
		}

		// ユーザー配送先情報
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_USER_SHIPPING_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERSHIPPING, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERSHIPPING));
		}

		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_USER_TAIWAN_INVOICE_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERINVOICE, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERINVOICE));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別が発注の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForStockOrder()
	{
		var result = new List<ListItem>();

		// 発注情報
		if (Constants.REALSTOCK_OPTION_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_STOCKORDER_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDER, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDER));
		}

		// 発注商品
		if (Constants.REALSTOCK_OPTION_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_STOCKORDER_ITEM_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDERITEM, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDERITEM));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別が入荷通知の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForUserProductArrivalMail()
	{
		var result = new List<ListItem>();

		// 入荷通知
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_ARRIVALMAIL_DL))
		{
			// 入荷通知サマリー
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL));
			// 入荷通知明細
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL_DETAIL, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL_DETAIL));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別が商品セールの場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForProductSalePrice()
	{
		var result = new List<ListItem>();

		// 商品セール
		if (Constants.PRODUCT_SALE_OPTION_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_PRODUCTSALEPRICE_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSALEPRICE, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSALEPRICE));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別が定期の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForFixedPurchase()
	{
		var result = new List<ListItem>();

		// 定期
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_FIXEDPURCHASE_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE));
		}

		// 定期詳細
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_FIXEDPURCHASE_ITEM_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別が定期の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemFixedPurchaseWorkFlow()
	{
		var result = new List<ListItem>();

		// 定期
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_FIXEDPURCHASEWF_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE_WORKFLOW, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE));
		}

		// 定期詳細
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_FIXEDPURCHASEWF_ITEM_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM_WORKFLOW, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別がショートURLの場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForShortUrl()
	{
		var result = new List<ListItem>();

		// 商品在庫
		if (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_SHORTURL_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SHORTURL, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SHORTURL));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別がシリアルキーの場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForSerialKey()
	{
		var result = new List<ListItem>();

		// シリアルキー
		if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_SERIALKEY_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SERIALKEY, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SERIALKEY));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別がリアル店舗の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForRealShop()
	{
		var result = new List<ListItem>();

		// リアル店舗
		if (Constants.REALSHOP_OPTION_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_REALSHOP_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOP, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOP));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別がリアル店舗商品在庫の場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForRealShopProductStock()
	{
		var result = new List<ListItem>();

		// リアル店舗商品在庫
		if (Constants.REALSHOP_OPTION_ENABLED && MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_REALSHOPPRODUCTSTOCK_DL))
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOPPRODUCTSTOCK, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOPPRODUCTSTOCK));
		}

		return result.ToArray();
	}

	/// <summary>
	/// ダウンロード種別がオペレータの場合のドロップダウン用アイテム生成
	/// </summary>
	/// <returns>ドロップダウン用リストアイテム</returns>
	private ListItem[] CreateDdlItemForOperator()
	{
		var result = new List<ListItem>();

		if (Constants.REALSHOP_OPTION_ENABLED)
		{
			result.AddRange(CreateDdlItems(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_OPERATOR, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_OPERATOR));
		}

		return result.ToArray();
	}
	#endregion

	#region -ExportProductDetailUrl 商品詳細ページURL出力処理
	/// <summary>
	/// 商品詳細ページURL出力処理
	/// </summary>
	/// <param name="searchParam">検索用ハッシュテーブル</param>
	public void ExportProductDetailUrl(Hashtable searchParam)
	{
		//------------------------------------------------------
		// HTTPヘッダセット
		//------------------------------------------------------
		Response.ContentEncoding = System.Text.Encoding.GetEncoding("Shift_JIS");
		Response.ContentType = "application/csv";
		Response.AppendHeader("Content-Disposition", "attachment; filename=" + "ProductDetailUrl" + DateTime.Now.ToString("yyyyMMdd") + ".csv");
		Response.ContentEncoding = System.Text.Encoding.GetEncoding("Shift_JIS");

		//------------------------------------------------------
		// ヘッダ行書き込み
		//------------------------------------------------------
		Response.Write("product_id,product_name");
		// PCサイトがある場合
		if (Constants.PHYSICALDIRPATH_FRONT_PC != "")
		{
			Response.Write(",pc url");
		}
		Response.Write("\r\n");

		//------------------------------------------------------
		// データ行書き込み
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("Product", "GetProductForExportUrl"))
		using (SqlStatementDataReader ssdrDataReader = new SqlStatementDataReader(sqlAccessor, sqlStatement, searchParam, true))
		{
			while (ssdrDataReader.Read())
			{
				Response.Write("\"");
				Response.Write(StringUtility.EscapeCsvColumn((string)ssdrDataReader[Constants.FIELD_PRODUCT_PRODUCT_ID]));
				Response.Write("\",\"");
				Response.Write(StringUtility.EscapeCsvColumn((string)ssdrDataReader[Constants.FIELD_PRODUCT_NAME]));
				Response.Write("\"");
				// PCサイトがある場合
				if (Constants.PHYSICALDIRPATH_FRONT_PC != "")
				{
					// PCサイトの商品詳細ページURL作成
					string strUrl = ProductCommon.CreateProductDetailUrl(
						ssdrDataReader[Constants.FIELD_PRODUCT_SHOP_ID],
						"",
						ssdrDataReader[Constants.FIELD_PRODUCT_BRAND_ID1],
						"",
						ssdrDataReader[Constants.FIELD_PRODUCT_PRODUCT_ID],
						(string)ssdrDataReader[Constants.FIELD_PRODUCT_NAME],
						ProductBrandUtility.GetProductBrandName((string)ssdrDataReader[Constants.FIELD_PRODUCT_BRAND_ID1]));

					// サイトルートを空文字に置き換え
					strUrl = strUrl.Replace(Constants.PATH_ROOT, "");

					Response.Write(",\"");
					Response.Write(StringUtility.EscapeCsvColumn(strUrl));
					Response.Write("\"");
				}
				// モバイルサイトがある場合
				if (Constants.PHYSICALDIRPATH_FRONT_MOBILE != "")
				{
					// モバイルサイトの商品詳細ページURL作成
					string strUrl = ProductCommon.CreateMobileProductDetailUrl(
						(string)ssdrDataReader[Constants.FIELD_PRODUCT_SHOP_ID],
						(string)ssdrDataReader[Constants.FIELD_PRODUCT_PRODUCT_ID],
						"",
						(string)ssdrDataReader[Constants.FIELD_PRODUCT_BRAND_ID1]);

					Response.Write(",\"");
					Response.Write(StringUtility.EscapeCsvColumn(strUrl));
					Response.Write("\"");
				}
				Response.Write("\r\n");
			}
		}

		//------------------------------------------------------
		// 出力ストップ
		//------------------------------------------------------
		Response.End();
	}
	#endregion

	#region -ExportProductListUrl 商品一覧ページURL出力処理
	/// <summary>
	/// 商品一覧ページURL出力処理
	/// </summary>
	public void ExportProductListUrl(Hashtable searchParam)
	{
		//------------------------------------------------------
		// HTTPヘッダセット
		//------------------------------------------------------
		Response.ContentEncoding = System.Text.Encoding.GetEncoding("Shift_JIS");
		Response.ContentType = "application/csv";
		Response.AppendHeader("Content-Disposition", "attachment; filename=" + "ProductListUrl" + DateTime.Now.ToString("yyyyMMdd") + ".csv");
		Response.ContentEncoding = System.Text.Encoding.GetEncoding("Shift_JIS");

		//------------------------------------------------------
		// ヘッダ行書き込み
		//------------------------------------------------------
		Response.Write("category_id,category_name");
		//ブランド使用時
		if (Constants.PRODUCT_BRAND_ENABLED)
		{
			Response.Write(",brand_id,brand_name");
		}
		// PCサイトがある場合
		if (Constants.PHYSICALDIRPATH_FRONT_PC != "")
		{
			Response.Write(",pc url");
		}
		Response.Write("\r\n");

		//------------------------------------------------------
		// データ行書き込み
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductCategory", (Constants.PRODUCT_BRAND_ENABLED ? "GetProductCategoryBrandEnabled" : "GetProductCategoryBrandUnenabled")))
		using (SqlStatementDataReader ssdrDataReader = new SqlStatementDataReader(sqlAccessor, sqlStatement, searchParam, true))
		{
			while (ssdrDataReader.Read())
			{
				Response.Write("\"");
				Response.Write(StringUtility.EscapeCsvColumn((string)ssdrDataReader[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID]));
				Response.Write("\",\"");
				Response.Write(StringUtility.EscapeCsvColumn((string)ssdrDataReader[Constants.FIELD_PRODUCTCATEGORY_NAME]));
				Response.Write("\"");
				//ブランド使用時
				if (Constants.PRODUCT_BRAND_ENABLED)
				{
					Response.Write(",\"");
					Response.Write(StringUtility.EscapeCsvColumn((string)ssdrDataReader[Constants.FIELD_PRODUCTBRAND_BRAND_ID]));
					Response.Write("\",\"");
					Response.Write(StringUtility.EscapeCsvColumn((string)ssdrDataReader[Constants.FIELD_PRODUCTBRAND_BRAND_NAME]));
					Response.Write("\"");
				}
				// PCサイトがある場合
				if (Constants.PHYSICALDIRPATH_FRONT_PC != "")
				{
					// PCサイトの商品一覧ページURL作成
					string strUrl = ProductCommon.CreateProductListUrl(
						ssdrDataReader[Constants.FIELD_PRODUCTCATEGORY_SHOP_ID],
						ssdrDataReader[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID],
						"",
						"",
						"",
						"",
						"",
						"",
						(string)ssdrDataReader[Constants.FIELD_PRODUCTBRAND_BRAND_ID],
						Constants.KBN_REQUEST_DISP_IMG_KBN_DEFAULT,
						"",
						(string)ssdrDataReader[Constants.FIELD_PRODUCTCATEGORY_NAME],
						(string)ssdrDataReader[Constants.FIELD_PRODUCTBRAND_BRAND_NAME],
						"",
						"",
						-1);

					// サイトルートから作られるので空文字に置き換え
					strUrl = strUrl.Replace(Constants.PATH_ROOT, "");

					Response.Write(",\"");
					Response.Write(StringUtility.EscapeCsvColumn(strUrl));
					Response.Write("\"");
				}
				// モバイルサイトがある場合
				if (Constants.PHYSICALDIRPATH_FRONT_MOBILE != "")
				{
					// モバイルサイトの商品一覧ページURL作成
					string strUrl = ProductCommon.CreateMobileProductListUrl(
						(string)ssdrDataReader[Constants.FIELD_PRODUCTCATEGORY_SHOP_ID],
						(string)ssdrDataReader[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID],
						"",
						"",
						"",
						"",
						"",
						Constants.KBN_REQUEST_DISP_IMG_KBN_DEFAULT,
						StringUtility.ToEmpty(ssdrDataReader[Constants.FIELD_PRODUCTBRAND_BRAND_ID]));

					Response.Write(",\"");
					Response.Write(StringUtility.EscapeCsvColumn(strUrl));
					Response.Write("\"");
				}
				Response.Write("\r\n");
			}
		}

		//------------------------------------------------------
		// 出力ストップ
		//------------------------------------------------------
		Response.End();
	}
	#endregion

	#region プロパティ
	/// <summary>ログインオペレータメニュー</summary>
	protected List<MenuLarge> LoginOperatorMenu
	{
		get { return (List<MenuLarge>)Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU]; }
	}
	/// <summary>ログインオペレータメニュー権限</summary>
	protected string LoginOperatorMenuAccessLevel
	{
		get { return Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU_ACCESS_LEVEL].ToString(); }
	}
	/// <summary>
	/// 検索用ハッシュテーブル作成処理のためのデリゲート
	/// </summary>
	/// <returns></returns>
	public delegate Hashtable CreateSearchParamsHandler();
	/// <summary>
	/// 検索用ハッシュテーブル作成処理イベント
	/// 利用側で処理割り当て
	/// </summary>
	public event CreateSearchParamsHandler OnCreateSearchInputParams;
	/// <summary>マスタダウンロード種別</summary>
	public MasterDownloadType DownloadType { get; set; }
	/// <summary>テーブルサイズ</summary>
	public string TableWidth { get; set; }
	#endregion
}
