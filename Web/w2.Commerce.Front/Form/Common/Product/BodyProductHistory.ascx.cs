/*
=========================================================================================================
  Module      : 商品表示履歴出力コントローラ処理(BodyProductHistory.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.NameTranslationSetting;
using w2.Domain.Product;

public partial class Form_Common_Product_BodyProductHistory : ProductUserControl
{
	#region ラップ済コントロール宣言
	WrappedRepeater WrProductHistory { get { return GetWrappedControl<WrappedRepeater>("rProductHistory"); } }
	# endregion

	const int CONST_DISPLAY_PRODUCTNAME_LENGTH = 10;	// 商品名表示文字数

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
			// 管理側プレビュー表示処理
			// ※セッションに商品表示履歴がないとプレビューができないため、ダミーデータ投入しています
			//------------------------------------------------------
			if (this.IsPreview)
			{
				if (Session[Constants.SESSION_KEY_PRODUCTHISTORY_LIST] == null)
				{
					Session[Constants.SESSION_KEY_PRODUCTHISTORY_LIST] = new ProductHistoryObject();
					ProductHistoryObject ph = (ProductHistoryObject)Session[Constants.SESSION_KEY_PRODUCTHISTORY_LIST];

					// 商品情報追加
					DataView products = w2.App.Common.Order.ProductCommon.GetDummyProductInfo(this.ShopId);
					foreach (DataRowView drv in products)
					{
						// 5件追加されていれば、抜ける
						if (ph.Count == 5) { break; }
						ph.Add(drv);
					}
				}
			}

			// 商品履歴オブジェクトが生成されている場合
			if (Session[Constants.SESSION_KEY_PRODUCTHISTORY_LIST] != null)
			{
				// セッションデータ取得
				ProductHistoryObject ph = (ProductHistoryObject)Session[Constants.SESSION_KEY_PRODUCTHISTORY_LIST];
			
				// 表示制御
				this.WrProductHistory.Visible = (ph.Count != 0);

				// 翻訳情報設定
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					ph = SetTranslationData(ph);
				}

				// データバインド
				this.WrProductHistory.DataSource = ph;
				this.WrProductHistory.DataBind();
			}
		}
	}

	/// <summary>
	/// 商品名取得
	/// </summary>
	/// <param name="strProductName">商品名</param>
	/// <returns></returns>
	/// <remarks>
	/// 引数商品名の文字数が商品名表示文字数を超えた場合、11文字以降を省略
	/// </remarks>
	protected string GetProductName(string strProductName)
	{
		string productNameDest = StringUtility.ToEmpty(strProductName);
		if (productNameDest.Length > CONST_DISPLAY_PRODUCTNAME_LENGTH)
		{
			System.Globalization.StringInfo productNameDestStringInfo = new System.Globalization.StringInfo(productNameDest);
			productNameDest = productNameDestStringInfo.SubstringByTextElements(0, CONST_DISPLAY_PRODUCTNAME_LENGTH) + "...";
		}

		return productNameDest;
	}

	/// <summary>
	/// 翻訳情報設定
	/// </summary>
	/// <param name="ph">商品履歴オブジェクト</param>
	/// <returns>翻訳後商品履歴オブジェクト</returns>
	private ProductHistoryObject SetTranslationData(ProductHistoryObject ph)
	{
		var products = ph.Select(
			p => new ProductModel
			{
				ProductId = (string)p[Constants.FIELD_PRODUCT_PRODUCT_ID]
			}).ToArray();
		var translationSettings = NameTranslationCommon.GetProductAndVariationTranslationSettings(
			products,
			RegionManager.GetInstance().Region.LanguageCode,
			RegionManager.GetInstance().Region.LanguageLocaleId);

		if (translationSettings.Any() == false) return ph;

		var history = new ProductHistoryObject();
		history.AddRange(ph.Select(p => SetProductTranslationData(p, translationSettings)));
		return history;
	}

	/// <summary>
	/// 商品翻訳情報設定
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="translationSettings">翻訳設定情報</param>
	/// <returns>翻訳後商品情報</returns>
	private Hashtable SetProductTranslationData(Hashtable product, NameTranslationSettingModel[] translationSettings)
	{
		var translationSetting = translationSettings.FirstOrDefault(
			setting => ((setting.MasterId1 == (string)product[Constants.FIELD_PRODUCT_PRODUCT_ID])
				&& (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_NAME)));

		product[Constants.FIELD_PRODUCT_NAME] = (translationSetting != null)
			? translationSetting.AfterTranslationalName
			: product[Constants.FIELD_PRODUCT_NAME];

		return product;
	}
}
