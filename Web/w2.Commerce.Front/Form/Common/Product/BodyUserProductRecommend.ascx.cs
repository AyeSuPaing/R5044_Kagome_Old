/*
=========================================================================================================
  Module      : ユーザおすすめ商品ランダム出力コントローラ処理(BodyUserProductRecommend.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;

public partial class Form_Common_Product_BodyUserProductRecommend : ProductUserControl
{
	#region ラップ済コントロール宣言
	WrappedRepeater WrProducts { get { return GetWrappedControl<WrappedRepeater>("rProducts"); } }
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
			// ユーザーおすすめ商品情報取得
			//------------------------------------------------------
			DataView products = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("ProductDisplay", "GetUserProductRecommend"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_PRODUCTCATEGORY_SHOP_ID, StringUtility.ToEmpty(this.ShopId));
				htInput.Add(Constants.FIELD_USER_USER_ID, StringUtility.ToEmpty(this.LoginUserId));
				htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, this.MemberRankId);
				htInput.Add(Constants.KEY_SHOW_OUT_OF_STOCK_ITEMS, this.ShowOutOfStock ? Constants.FLG_SHOW_OUT_OF_STOCK_ITEMS_VALID : Constants.FLG_SHOW_OUT_OF_STOCK_ITEMS_INVALID);
				htInput.Add(Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG, this.UserFixedPurchaseMemberFlg);
				products = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			//------------------------------------------------------
			// 表示する分のみ取得してデータソースへセット
			//------------------------------------------------------
			List<DataRowView> displayProductList = new List<DataRowView>();
			for (int iLoop = 0; ((iLoop < products.Count) && (iLoop < this.MaxDispCount)); iLoop++)
			{
				displayProductList.Add(products[iLoop]);
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				// 翻訳情報設定(w2_Product単位、List<DataRowView>になってるっぽい)
				displayProductList = SetTranslationData(displayProductList);
			}

			//------------------------------------------------------
			// 画面セット
			//------------------------------------------------------
			this.WrProducts.DataSource = displayProductList;
			this.WrProducts.DataBind();

			this.WrProducts.Visible = (products.Count != 0);
		}
	}

	/// <summary>
	/// 翻訳情報設定
	/// </summary>
	/// <param name="displayProductList">表示する商品リスト</param>
	/// <returns>翻訳情報設定後のリスト</returns>
	private List<DataRowView> SetTranslationData(List<DataRowView> displayProductList)
	{
		if (displayProductList == null || displayProductList.Count == 0) return displayProductList;

		var productIds = displayProductList.Select(drv => (string)drv[Constants.FIELD_PRODUCT_PRODUCT_ID]).ToList();

		var searchCondition = new NameTranslationSettingSearchCondition
		{
			DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT,
			MasterId1List = productIds,
			LanguageCode = RegionManager.GetInstance().Region.LanguageCode,
			LanguageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId,
		};
		var translationSettings =
			new NameTranslationSettingService().GetTranslationSettingsByMultipleMasterId1(searchCondition);

		var translationProductList = displayProductList
			.Select(drv => NameTranslationCommon.SetTranslationDataToDataRowView(drv, Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT, translationSettings))
			.ToList();

		return translationProductList;
	}

	#region プロパティ
	/// <summary>商品最大表示数（外部から設定可能）</summary>
	protected int MaxDispCount
	{
		get { return maxDispCount; }
		set { maxDispCount = value; }
	}
	int maxDispCount = 5;
	/// <summary>在庫切れ商品を表示するかどうか</summary>
	protected bool ShowOutOfStock
	{
		get { return m_showOutOfStock; }
		set { m_showOutOfStock = value; }
	}
	private bool m_showOutOfStock = Constants.SHOW_OUT_OF_STOCK_ITEMS;
	/// <summary>商品画像サイズ（外部から設定可能）</summary>
	protected string ImageSize { get; set; }
	/// <summary>商品数</summary>
	public int ProductCount { get; set; }
	#endregion
}
