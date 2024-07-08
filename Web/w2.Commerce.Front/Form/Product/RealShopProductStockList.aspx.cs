/*
=========================================================================================================
  Module      : リアル店舗商品在庫一覧画面処理(RealShopProductStockList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using System.Data;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.App.Common.Web.WrappedContols;

public partial class Form_Product_RealShopProductStockList : ProductPage
{
	#region ラップ済コントロール宣言
	WrappedDropDownList WddlVariationSelect { get { return GetWrappedControl<WrappedDropDownList>("ddlVariationSelect"); } }
	# endregion

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
			GetParameters();

			//------------------------------------------------------
			// 商品情報取得
			//------------------------------------------------------
			DataView product = ProductCommon.GetProductInfo(Constants.CONST_DEFAULT_SHOP_ID, this.ProductId, this.MemberRankId, this.UserFixedPurchaseMemberFlg);
			if (product.Count == 0)
			{
				// 商品情報が見つからない場合はエラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_UNDISP);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			//------------------------------------------------------
			// プロパティセット
			//------------------------------------------------------
			this.ProductMaster = product[0];
			this.ProductVariationMaster = product;
			this.HasVariation = HasVariation(this.ProductMaster);

			// 翻訳情報設定
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				SetProductTranslationData();
			}

			//------------------------------------------------------
			// バリエーション選択肢セット
			//------------------------------------------------------
			if (this.HasVariation)
			{
				foreach (DataRowView productVariation in this.ProductVariationMaster)
				{
					ListItem li = new ListItem(CreateVariationName(productVariation, "", "", Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION), (string)productVariation[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
					if (li.Value == this.VariationId)
					{
						li.Selected = true;
						this.ProductMaster = productVariation;
					}

					WddlVariationSelect.AddItem(li);
				}
			}

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			DataBind();
		}
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
	///  バリエーションのドロップダウン変更
	///  ※バリエーションIDを付与してリダイレクト
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlVariationSelect_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_REALSHOPPRODUCTSTOCK_LIST);
		url.Append("?").Append(Constants.REQUEST_KEY_PRODUCT_ID).Append("=").Append(Server.UrlEncode(this.ProductId));
		url.Append("&").Append(Constants.REQUEST_KEY_VARIATION_ID).Append("=").Append(Server.UrlEncode(ddlVariationSelect.SelectedValue));
		// 商品ID&商品バリエーションID以外のパラメータを付与
		// パーツ「BodyRealShopProductStockList.ascx」の表示制御などで利用するため
		foreach (string key in Request.QueryString.AllKeys)
		{
			if ((key == Constants.REQUEST_KEY_PRODUCT_ID) || (key == Constants.REQUEST_KEY_VARIATION_ID)) continue;
			url.Append("&").Append(key).Append("=").Append(Server.UrlEncode(Request[key]));
		}

		Response.Redirect(url.ToString());
	}

	/// <summary>
	/// 商品翻訳情報設定
	/// </summary>
	private void SetProductTranslationData()
	{
		// 翻訳設定情報取得
		var translationSettings = NameTranslationCommon.GetProductAndVariationTranslationSettingsByProductId(
			(string)this.ProductMaster[Constants.FIELD_PRODUCT_PRODUCT_ID],
			RegionManager.GetInstance().Region.LanguageCode,
			RegionManager.GetInstance().Region.LanguageLocaleId);

		// 翻訳情報設定
		this.ProductMaster = NameTranslationCommon.SetProductAndVariationTranslationDataToDataRowView(
			this.ProductMaster,
			translationSettings);
		this.ProductVariationMaster = NameTranslationCommon.SetProductAndVariationTranslationDataToDataView(this.ProductVariationMaster, translationSettings);
	}

	/// <summary>商品マスタ</summary>
	protected DataRowView ProductMaster { get; private set; }
	/// <summary>商品マスタ（バリエーション含む）</summary>
	protected DataView ProductVariationMaster { get; private set; }
	/// <summary>バリエーション有り？</summary>
	protected new bool HasVariation { get; private set; }
}