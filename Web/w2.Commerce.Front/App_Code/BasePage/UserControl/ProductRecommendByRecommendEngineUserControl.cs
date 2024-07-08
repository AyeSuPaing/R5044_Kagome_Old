/*
=========================================================================================================
  Module      : 商品レコメンドリスト出力コントローラ処理(ProductRecommendByRecommendEngineUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using w2.App.Common.Option;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;

public class ProductRecommendByRecommendEngineUserControl : ProductUserControl
{
	#region ラップ済コントロール宣言
	WrappedRepeater WrrOrderHistoryForRecommend { get { return GetWrappedControl<WrappedRepeater>("rOrderHistoryForRecommend"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (this.IsRecommendEngineSilveregg)
		{
			// データバインド（this.RecommendOrderListは注文完了画面より設定される）
			this.WrrOrderHistoryForRecommend.DataSource = this.RecommendOrderList;
			this.WrrOrderHistoryForRecommend.DataBind();

			// リクエストタグ出力判定用プロパティの編集
			this.IsRequestProductId = SilvereggAigentRecommend.SetIsRequestProductId(this.RecommendCode);
			this.IsRequestOrderList = SilvereggAigentRecommend.SetIsRequestOrderList(this.RecommendCode);
		}
	}

	#region レコナイズ削除予定記述
	/// <summary>
	/// トラッキングログ送信URL作成
	/// </summary>
	/// <param name="objRecommendCategory">レコメンドカテゴリ</param>
	/// <returns>トラッキングログ送信ページURL</returns>
	protected string CreateSendTrackingLogUrl(object objRecommendProduct)
	{
		return CreateSendTrackingLogUrl(objRecommendProduct, Constants.RecommendKbn.ProductRecommend);
	}
	#endregion

	#region 注文商品データから税別価格を算出(シルバーエッグ受注情報連携用)
	/// <summary>
	/// 商品価格と税率から税別価格を算出(シルバーエッグ受注情報連携用)
	/// </summary>
	/// <param name="productData">注文商品データ</param>
	/// <returns>税別金額</returns>
	public string GetTaxExcludedPrice(DataRowView productData)
	{
		return TaxCalculationUtility.GetPriceTaxExcluded(
			(decimal) productData[Constants.FIELD_ORDERITEM_PRODUCT_PRICE],
			TaxCalculationUtility.GetTaxPrice(
				(decimal)productData[Constants.FIELD_ORDERITEM_PRODUCT_PRICE],
				(decimal)productData[Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE],
				Constants.TAX_EXCLUDED_FRACTION_ROUNDING)).ToString();
	}
	#endregion

	/// <summary>商品画像サイズ（外部から設定可能）</summary>
	public string ImageSize { get; set; }
	/// <summary>レコメンドコード（外部から設定可能）</summary>
	public string RecommendCode { get; set; }
	/// <summary>アイテムコード（外部から設定可能）</summary>
	public string ItemCode
	{
		get { return (string)ViewState["ItemCode"]; }
		set
		{
			ViewState["ItemCode"] = value;
		}
	}
	/// <summary>商品最大表示数（外部から設定可能）</summary>
	public int MaxDispCount { get; set; }
	/// <summary>レコメンドエンジン連携用商品ID</summary>
	public string RecommendProductId
	{
		get { return (string)ViewState["RecommendProductId"]; }
		set { ViewState["RecommendProductId"] = value; }
	}
	/// <summary>レコメンドエンジン連携用注文済み商品情報</summary>
	public List<DataView> RecommendOrderList { get; set; }
	/// <summary>レコメンドタイトル</summary>
	public string RecommendTitle { get; set; }
	/// <summary>レコメンド対象にするカテゴリID</summary>
	public string DispCategoryId { get; set; }
	/// <summary>レコメンド非対象にするカテゴリID</summary>
	public string NotDispCategoryId { get; set; }
	/// <summary>レコメンド非対象にするアイテムID</summary>
	public string NotDispRecommendProductId { get; set; }
	/// <summary>リクエストパラメータに商品IDの有無</summary>
	public bool IsRequestProductId { get; set; }
	/// <summary>リクエストパラメータに注文済み商品情報の有無</summary>
	public bool IsRequestOrderList { get; set; }
	/// <summary>シルバーエッグレコメンドを利用中</summary>
	public bool IsRecommendEngineSilveregg
	{
		get
		{
			return (Constants.RECOMMEND_ENGINE_KBN == Constants.RecommendEngine.Silveregg);
		}
	}
}