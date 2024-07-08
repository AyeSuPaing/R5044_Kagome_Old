/*
=========================================================================================================
  Module      : 商品在庫一覧出力コントローラ処理(BodyProductStockList.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Global.Translation;
using w2.App.Common.Preview;
using w2.App.Common.UserProductArrivalMail;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Product;

public partial class Form_Common_Product_BodyProductStockList : ProductUserControl
{
	# region ラップ済コントロール宣言
	WrappedHtmlGenericControl WdivSingleVariation { get { return GetWrappedControl<WrappedHtmlGenericControl>(this, "divSingleVariation"); } }
	WrappedHtmlGenericControl WdivPluralVariation { get { return GetWrappedControl<WrappedHtmlGenericControl>(this, "divPluralVariation"); } }
	WrappedRepeater WrStockList { get { return GetWrappedControl<WrappedRepeater>(this, "rStockList"); } }
	WrappedRepeater WrStockMatrix { get { return GetWrappedControl<WrappedRepeater>(this, "rStockMatrix"); } }
	WrappedRepeater WrStockMatrixHorizonalTitile { get { return GetWrappedControl<WrappedRepeater>(this, "rStockMatrixHorizonalTitile"); } }
	# endregion
	
	protected string[,] m_strVariationMatrixs = null;

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
			// 商品在庫状況情報取得
			//------------------------------------------------------
			var productVariationStockInfos = new ProductService().GetProductVariationStockInfos(
				this.ShopId,
				this.ProductId,
				this.LoginUserMemberRankId);
			if (productVariationStockInfos.Length == 0)
			{
				// プレビューモードチェック(商品新規登録時のプレビューは在庫表を表示しない)
				if ((string)Request[Constants.REQUEST_KEY_PREVIEW_HASH] != null)
				{
					// ハッシュチェック
					if ((string)Request[Constants.REQUEST_KEY_PREVIEW_HASH] == ProductPreview.CreateProductDetailHash())
					{
						return;
					}
				}

				// 無ければエラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_UNDISP);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				NameTranslationCommon.SetStockTranslationData(ref productVariationStockInfos, this.ProductId);
			}

			//------------------------------------------------------
			// 画面セット処理
			//------------------------------------------------------
			// 商品名セット
			this.ProductName = productVariationStockInfos[0].Name;

			// 複数バリエーションチェック
			bool IsPluralVariation = false;
			foreach (var productVariationStockInfo in productVariationStockInfos)
			{
				if (productVariationStockInfo.VariationName2.Length != 0)
				{
					IsPluralVariation = true;
					break;
				}
			}
			// 複数バリエーション？
			if (IsPluralVariation)
			{
				this.WdivPluralVariation.Visible = true;
				this.WdivSingleVariation.Visible = false;

				// マトリクス決定
				List<string> lVariationName1s = new List<string>();
				var variationName23s = new List<string>();
				foreach (var productVariationStockInfo in productVariationStockInfos)
				{
					string strVariationName1 = productVariationStockInfo.VariationName1;
					var variationName23 = ((string.IsNullOrEmpty(productVariationStockInfo.VariationName2) == false)
						&& (string.IsNullOrEmpty(productVariationStockInfo.VariationName3) == false))
							? (productVariationStockInfo.VariationName2 + "-" + productVariationStockInfo.VariationName3)
							: (productVariationStockInfo.VariationName2 + productVariationStockInfo.VariationName3);
					if (lVariationName1s.IndexOf(strVariationName1) == -1)
					{
						lVariationName1s.Add(strVariationName1);
					}
					if (variationName23s.IndexOf(variationName23) == -1)
					{
						variationName23s.Add(variationName23);
					}
				}

				// 空のマトリクス作成
				string[,] strVariationMatrixs = new string[lVariationName1s.Count, variationName23s.Count];

				// マトリクス埋め
				foreach (var productVariationStockInfo in productVariationStockInfos)
				{
					var variationName23 = ((string.IsNullOrEmpty(productVariationStockInfo.VariationName2) == false)
						&& (string.IsNullOrEmpty(productVariationStockInfo.VariationName3) == false))
							? (productVariationStockInfo.VariationName2 + "-" + productVariationStockInfo.VariationName3)
							: (productVariationStockInfo.VariationName2 + productVariationStockInfo.VariationName3);

					strVariationMatrixs[
						lVariationName1s.IndexOf(productVariationStockInfo.VariationName1),
						variationName23s.IndexOf(variationName23)] =
						(productVariationStockInfo.StockManagementKbn != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
							? productVariationStockInfo.StockMessage
							: productVariationStockInfo.StockMessageDef;
				}

				m_strVariationMatrixs = strVariationMatrixs;
				this.VariationName1s = lVariationName1s;
				this.VariationName2s = new List<string>(); 
				this.VariationName23s = variationName23s;

				// マトリクスタイトルデータバインド
				this.WrStockMatrixHorizonalTitile.DataSource = variationName23s;
				this.WrStockMatrixHorizonalTitile.DataBind();

				// マトリクスデータバインド
				this.WrStockMatrix.DataSource = lVariationName1s;
				this.WrStockMatrix.DataBind();
			}
			// 単数バリエーション？
			else
			{
				this.WdivPluralVariation.Visible = false;
				this.WdivSingleVariation.Visible = true;

				this.WrStockList.DataSource = productVariationStockInfos.Select(info => info.DataSource);
				this.WrStockList.DataBind();
			}
		}
	}

	/// <summary>
	/// 入荷通知メール区分取得
	/// </summary>
	/// <param name="objProduct"></param>
	/// <returns>入荷通知メール区分</returns>
	protected string GetArrivalMailKbn(object objProduct)
	{
		return UserProductArrivalMailCommon.GetArrivalMailKbn(
			HasVariation(objProduct),
			true,
			new DataView(),
			(DataRowView)objProduct); ;
	}

	/// <summary>
	/// 入荷通知メール登録URL取得
	/// </summary>
	/// <param name="objProduct"></param>
	/// <returns>入荷通知メール登録URL</returns>
	protected string GetRegistUserProductArrivalMailUrl(object objProduct)
	{
		return ProductPage.CreateRegistUserProductArrivalMailUrl(
			(string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_SHOP_ID),
			(string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_PRODUCT_ID),
			(string)GetKeyValue(objProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID),
			GetArrivalMailKbn(objProduct),
			this.RawUrl);
	}

	/// <summary>
	/// 在庫メッセージ取得
	/// </summary>
	/// <param name="info">商品バリエーション在庫リスト情報</param>
	/// <returns>在庫メッセージ</returns>
	protected string GetStockMessage(object info)
	{
		var productVariationStockInfo = new ProductVariationStockInfo((Hashtable)info);
		var message = (productVariationStockInfo.StockManagementKbn != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
			? productVariationStockInfo.StockMessage
			: productVariationStockInfo.StockMessageDef;
		return message;
	}


	/// <summary>バリエーション名1リスト</summary>
	protected List<string> VariationName1s { get; set; }
	/// <summary>バリエーション名2リスト</summary>
	protected List<string> VariationName2s { get; set; }
	/// <summary>バリエーション名23リスト</summary>
	protected List<string> VariationName23s { get; set; }
	/// <summary>商品名</summary>
	protected string ProductName
	{
		get { return (string)ViewState["ProductName"]; }
		private set { ViewState["ProductName"] = value; }
	}
	/// <summary>商品価格を表示するか</summary>
	public bool DisplayPrice
	{
		get { return (ViewState["DisplayPrice"] != null) ? (bool)ViewState["DisplayPrice"] : false; }
		set { ViewState["DisplayPrice"] = value; }
	}
	/// <summary>在庫表タイトルを表示するか</summary>
	public bool ProductStockTitle
	{
		get { return (ViewState["ProductStockTitle"] != null) ? (bool)ViewState["ProductStockTitle"] : false; }
		set { ViewState["ProductStockTitle"] = value; }
	}
}