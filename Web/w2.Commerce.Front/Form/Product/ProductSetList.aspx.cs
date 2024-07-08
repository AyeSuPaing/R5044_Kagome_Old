/*
=========================================================================================================
  Module      : 商品セット組立画面処理(ProductSetList.aspx.cs)
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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Product;

public partial class Form_Product_ProductSetList : ProductPage
{
	/// <summary>リピートプラスONEリダイレクト必須判定</summary>
	public override bool RepeatPlusOneNeedsRedirect { get { return Constants.REPEATPLUSONE_OPTION_ENABLED; } }
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Http; } }	// Httpアクセス

	#region ラップ済みコントロール宣言
	WrappedRepeater WrProductList { get { return GetWrappedControl<WrappedRepeater>("rProductList"); } }
	WrappedHtmlAnchor WrAddCart { get { return GetWrappedControl<WrappedHtmlAnchor>("lbAddCart"); } }
	# endregion

	#region Repeter内コントロールID宣言
	const string CONTROL_ID_ITEM_COUNT = "ddlItemCount";
	const string CONTROL_ID_FAMILY_FLG = "hfFamilyFlg";
	const string CONTROL_ID_SET_ITEM_PRICE = "hfSetItemPrice";
	const string CONTROL_ID_SET_ITEM_PRICE_TOTAL = "sSetItemPriceTotal";
	const string CONTROL_ID_SET_ITEM_ERROR_MESSAGE = "sSetItemErrorMessage";
	const string CONTROL_ID_PRODUCT_ID = "hfProductId";
	const string CONTROL_ID_VARIATION_ID = "hfVariationId";
	const string CONTROL_ID_CATEGORY_ID = "hfCategoryId";
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
			// リクエストよりパラメタ取得（商品一覧共通処理）
			//------------------------------------------------------
			GetParameters();

			this.ProductSetId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTSET_ID]);
			this.BeforeUrl = (Request.UrlReferrer != null) ? Request.UrlReferrer.AbsoluteUri : Constants.PATH_ROOT;
			string strMemberRankId = this.MemberRankId;

			//------------------------------------------------------
			// セット購入可能商品一覧取得/配送種別が一致するもののみ取得
			//------------------------------------------------------
			List<DataRowView> lValidProducts = new List<DataRowView>();
			{
				// 一覧取得
				DataView dvSetProductInfos = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("ProductSet", "GetProductSetProducts"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_PRODUCTSET_SHOP_ID, this.ShopId);
					htInput.Add(Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID, this.ProductSetId);

					dvSetProductInfos = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
				}
				if (dvSetProductInfos.Count == 0)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCTSET_NOPRODUCT);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}

				// 配送種別が一致するもののみ抽出
				string strShippingType = null;
				foreach (DataRowView drvProduct in dvSetProductInfos)
				{
					if (strShippingType == null)
					{
						strShippingType = (string)drvProduct[Constants.FIELD_PRODUCT_SHIPPING_TYPE];
					}

					if (strShippingType == (string)drvProduct[Constants.FIELD_PRODUCT_SHIPPING_TYPE])
					{
						lValidProducts.Add(drvProduct);
					}
				}

				// 閲覧可能会員ランクチェック
				// 複数ある場合は一番上位のランクを抽出
				if (Constants.MEMBER_RANK_OPTION_ENABLED)
				{
					int iMaxRankNo = 0;
					string strMaxRankId = "";

					foreach (DataRowView drvProduct in lValidProducts)
					{
						string strProductDisplayRankId = (string)drvProduct[Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK];
						int iProductDisplayRankNo = MemberRankOptionUtility.GetMemberRankNo(strProductDisplayRankId);

						// 閲覧不可なら
						if (MemberRankOptionUtility.CheckMemberRankPermission(strMemberRankId, strProductDisplayRankId) == false)
						{
							// 最大の会員ランクを保存
							if ((iMaxRankNo == 0)
								|| (iMaxRankNo > iProductDisplayRankNo))
							{
								iMaxRankNo = iProductDisplayRankNo;
								strMaxRankId = strProductDisplayRankId;
							}
						}
					}

					if (strMaxRankId != "")
					{
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_DISPLAY_MEMBER_RANK).Replace("@@ 1 @@", MemberRankOptionUtility.GetMemberRankName(strMaxRankId));
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
					}
				}
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				lValidProducts = SetTranslationData(lValidProducts);
			}

			//------------------------------------------------------
			// 値セット
			//------------------------------------------------------
			this.ChildMin = (lValidProducts[0][Constants.FIELD_PRODUCTSET_CHILD_MIN] != DBNull.Value) ? (int)lValidProducts[0][Constants.FIELD_PRODUCTSET_CHILD_MIN] : 0;
			this.ChildMax = (lValidProducts[0][Constants.FIELD_PRODUCTSET_CHILD_MAX] != DBNull.Value) ? (int)lValidProducts[0][Constants.FIELD_PRODUCTSET_CHILD_MAX] : 99;
			this.ParentMin = (lValidProducts[0][Constants.FIELD_PRODUCTSET_PARENT_MIN] != DBNull.Value) ? (int)lValidProducts[0][Constants.FIELD_PRODUCTSET_PARENT_MIN] : 0;
			this.ParentMax = (lValidProducts[0][Constants.FIELD_PRODUCTSET_PARENT_MAX] != DBNull.Value) ? (int)lValidProducts[0][Constants.FIELD_PRODUCTSET_PARENT_MAX] : 99;

			//------------------------------------------------------
			// 画面表示
			//------------------------------------------------------
			// 名称セット
			this.SetName = (string)lValidProducts[0][Constants.FIELD_PRODUCTSET_PRODUCT_SET_NAME];

			// 説明セット
			switch ((string)lValidProducts[0][Constants.FIELD_PRODUCTSET_DESCRIPTION_KBN])
			{
				case Constants.FLG_PRODUCTSET_DESCRIPTION_KBN_HTML:
					this.Description = (string)lValidProducts[0][Constants.FIELD_PRODUCTSET_DESCRIPTION];
					break;

				case Constants.FLG_PRODUCTSET_DESCRIPTION_KBN_TEXT:
					this.Description = WebSanitizer.HtmlEncodeChangeToBr((string)lValidProducts[0][Constants.FIELD_PRODUCTSET_DESCRIPTION]);
					break;
			}

			// 商品一覧データバインド
			this.WrProductList.DataSource = lValidProducts;
			this.WrProductList.DataBind();

			//------------------------------------------------------
			// 各コンポーネントセット
			//------------------------------------------------------
			foreach (RepeaterItem ri in this.WrProductList.Items)
			{
				var htRepeaterItem = GetWrappedRepeaterItem(ri);

				DataRowView drvSetProductItem = lValidProducts[ri.ItemIndex];

				// 商品IDセット
				((WrappedHiddenField)htRepeaterItem[CONTROL_ID_PRODUCT_ID]).Value = (string)drvSetProductItem[Constants.FIELD_PRODUCT_PRODUCT_ID];
				((WrappedHiddenField)htRepeaterItem[CONTROL_ID_VARIATION_ID]).Value = (string)drvSetProductItem[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID];
				((WrappedHiddenField)htRepeaterItem[CONTROL_ID_CATEGORY_ID]).Value = (string)drvSetProductItem[Constants.FIELD_PRODUCT_CATEGORY_ID1];

				// 個数ドロップダウンセット
				int iProductItemCountMin = (drvSetProductItem[Constants.FIELD_PRODUCTSETITEM_COUNT_MIN] != DBNull.Value) ? (int)drvSetProductItem[Constants.FIELD_PRODUCTSETITEM_COUNT_MIN] : 0;
				int iProductItemCountMax = (drvSetProductItem[Constants.FIELD_PRODUCTSETITEM_COUNT_MAX] != DBNull.Value) ? (int)drvSetProductItem[Constants.FIELD_PRODUCTSETITEM_COUNT_MAX] : (int)drvSetProductItem[Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY];
				if (iProductItemCountMax < iProductItemCountMin)
				{
					iProductItemCountMax = 0;
					iProductItemCountMin = 0;
				}
				WrappedDropDownList ddlItemCount = (WrappedDropDownList)htRepeaterItem[CONTROL_ID_ITEM_COUNT];
				for (int iLoop = iProductItemCountMin; iLoop <= iProductItemCountMax; iLoop++)
				{
					ddlItemCount.AddItem(iLoop.ToString());
				}

				// 購入可能会員ランクチェック
				if (Constants.MEMBER_RANK_OPTION_ENABLED)
				{
					string strProductName = (string)drvSetProductItem[Constants.FIELD_PRODUCT_NAME];
					string strProductBuyableRankId = (string)drvSetProductItem[Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK];

					// 購入不可なら
					if (MemberRankOptionUtility.CheckMemberRankPermission(strMemberRankId, strProductBuyableRankId) == false)
					{
						((WrappedHtmlGenericControl)htRepeaterItem[CONTROL_ID_SET_ITEM_ERROR_MESSAGE]).InnerHtml
							= WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_BUYABLE_MEMBER_RANK).Replace("@@ 1 @@", strProductName).Replace("@@ 2 @@", MemberRankOptionUtility.GetMemberRankName(strProductBuyableRankId));
					}
				}
			}

			//------------------------------------------------------
			// 再計算
			//------------------------------------------------------
			Calculate();
		}
	}

	/// <summary>
	/// 再計算
	/// </summary>
	private void Calculate()
	{
		int iParentCnt = 0;
		int iChildCnt = 0;

		// 各商品情報取得
		decimal dSetPriceTotal = 0;
		foreach (RepeaterItem riTarget in this.WrProductList.Items)
		{
			// リピーター内のコントロールをラップ
			var htRepeaterItem = GetWrappedRepeaterItem(riTarget);

			int iItemCount = int.Parse(((WrappedDropDownList)htRepeaterItem[CONTROL_ID_ITEM_COUNT]).SelectedValue);

			if (((WrappedHiddenField)htRepeaterItem[CONTROL_ID_FAMILY_FLG]).Value == Constants.FLG_PRODUCTSETITEM_FAMILY_FLG_PARENT)
			{
				iParentCnt += iItemCount;
			}
			else if (((WrappedHiddenField)htRepeaterItem[CONTROL_ID_FAMILY_FLG]).Value == Constants.FLG_PRODUCTSETITEM_FAMILY_FLG_CHILD)
			{
				iChildCnt += iItemCount;
			}

			decimal dSetItemPrice = decimal.Parse(((WrappedHiddenField)htRepeaterItem[CONTROL_ID_SET_ITEM_PRICE]).Value);
			decimal dSetItemPriceTotal = dSetItemPrice * iItemCount;
			dSetPriceTotal += dSetItemPriceTotal;

			((WrappedHtmlGenericControl)htRepeaterItem[CONTROL_ID_SET_ITEM_PRICE_TOTAL]).InnerHtml = WebSanitizer.HtmlEncode(StringUtility.ToPrice(dSetItemPriceTotal));
		}

		// セット合計金額セット(FotterTemplate内の値を動的に変更する）
		if (this.WrProductList.InnerControl != null)
		{
			HtmlGenericControl spSetPriceTotal = (HtmlGenericControl)this.WrProductList.Controls[this.WrProductList.Controls.Count - 1].FindControl("spSetPriceTotal");
			spSetPriceTotal.InnerHtml = WebSanitizer.HtmlEncode(StringUtility.ToPrice(dSetPriceTotal));
		}
	}

	/// <summary>
	/// 個数ドロップダウン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlItemCount_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 再計算
		Calculate();
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, EventArgs e)
	{
		Response.Redirect(this.BeforeUrl);
	}

	/// <summary>
	/// カート投入ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbAddCart_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 個数チェック
		//------------------------------------------------------
		int iParentCnt = 0;
		int iChildCnt = 0;
		foreach (RepeaterItem riTarget in this.WrProductList.Items)
		{
			// RepeaterItemをHashtableにセット
			var htRepeatrItem = GetWrappedRepeaterItem(riTarget);

			int iItemCount = int.Parse(((WrappedDropDownList)htRepeatrItem[CONTROL_ID_ITEM_COUNT]).SelectedValue);
			if (((WrappedHiddenField)htRepeatrItem[CONTROL_ID_FAMILY_FLG]).Value == Constants.FLG_PRODUCTSETITEM_FAMILY_FLG_PARENT)
			{
				iParentCnt += iItemCount;
			}
			else if (((WrappedHiddenField)htRepeatrItem[CONTROL_ID_FAMILY_FLG]).Value == Constants.FLG_PRODUCTSETITEM_FAMILY_FLG_CHILD)
			{
				iChildCnt += iItemCount;
			}
		}
		if (((iParentCnt < this.ParentMin) || (iParentCnt > this.ParentMax))
			|| ((iChildCnt < this.ChildMin) || (iChildCnt > this.ChildMax)))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_SET_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		//------------------------------------------------------
		// 購入可能会員ランクチェック
		//------------------------------------------------------
		if (Constants.MEMBER_RANK_OPTION_ENABLED)
		{
			string strMemberRankId = this.MemberRankId;
			foreach (RepeaterItem riTarget in this.WrProductList.Items)
			{
				// RepeaterItemをHashtableにセット
				var htRepeaterItem = GetWrappedRepeaterItem(riTarget);

				// 個数が０以外なら
				if (int.Parse(((WrappedDropDownList)htRepeaterItem[CONTROL_ID_ITEM_COUNT]).SelectedValue) > 0)
				{
					DataView dvProduct = ProductCommon.GetProductInfoUnuseMemberRankPrice(this.ShopId, ((WrappedHiddenField)htRepeaterItem[CONTROL_ID_PRODUCT_ID]).Value);

					if (dvProduct.Count != 0)
					{
						//------------------------------------------------------
						// 購入可能会員ランクチェック
						//------------------------------------------------------
						string strProductName = (string)dvProduct[0][Constants.FIELD_PRODUCT_NAME];
						string strProductBuyableRankId = (string)dvProduct[0][Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK];

						// 購入不可なら
						if (MemberRankOptionUtility.CheckMemberRankPermission(strMemberRankId, strProductBuyableRankId) == false)
						{
							Session[Constants.SESSION_KEY_ERROR_MSG]
								= WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_BUYABLE_MEMBER_RANK).Replace("@@ 1 @@", strProductName).Replace("@@ 2 @@", MemberRankOptionUtility.GetMemberRankName(strProductBuyableRankId));
							Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
						}
					}
				}
			}
		}

		//------------------------------------------------------
		// 遷移URL作成
		//------------------------------------------------------
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_CART_LIST);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_CART_ACTION).Append("=").Append(Server.UrlEncode(Constants.KBN_REQUEST_CART_ACTION_ADD_SET));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_SHOP_ID).Append("=").Append(Server.UrlEncode(this.ShopId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_PRODUCTSET_ID).Append("=").Append(Server.UrlEncode(this.ProductSetId));

		int iIndex = 1;
		StringBuilder sbUrlProducts = new StringBuilder();
		foreach (RepeaterItem riTarget in this.WrProductList.Items)
		{
			var htRepeaterItem = GetWrappedRepeaterItem(riTarget);

			WrappedDropDownList ddlItemCount = (WrappedDropDownList)htRepeaterItem[CONTROL_ID_ITEM_COUNT];
			decimal dSetItemCount = int.Parse(ddlItemCount.SelectedValue);
			if (dSetItemCount != 0)
			{
				string strProductId = ((WrappedHiddenField)htRepeaterItem[CONTROL_ID_PRODUCT_ID]).Value;
				string strVariationId = ((WrappedHiddenField)htRepeaterItem[CONTROL_ID_VARIATION_ID]).Value;
				string strCategoryId = ((WrappedHiddenField)htRepeaterItem[CONTROL_ID_CATEGORY_ID]).Value;

				sbUrlProducts.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_ID).Append(iIndex).Append("=").Append(Server.UrlEncode(strProductId));
				sbUrlProducts.Append("&").Append(Constants.REQUEST_KEY_VARIATION_ID).Append(iIndex).Append("=").Append(Server.UrlEncode(strVariationId));
				sbUrlProducts.Append("&").Append(Constants.REQUEST_KEY_CATEGORY_ID).Append(iIndex).Append("=").Append(Server.UrlEncode(strCategoryId));
				sbUrlProducts.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_COUNT).Append(iIndex).Append("=").Append(Server.UrlEncode(dSetItemCount.ToString()));

				iIndex++;
			}
		}

		// 商品ゼロであればエラー
		if (sbUrlProducts.Length == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCTSET_UNSELECTED_PRODUCT);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		//------------------------------------------------------
		// 画面遷移（カート投入）
		//------------------------------------------------------
		Response.Redirect(sbUrl.ToString() + sbUrlProducts.ToString());
	}

	/// <summary>
	/// リピーター内のコントロールをラップして、Hashtableに格納する(keyはラップ元のコントロールID)
	/// </summary>
	/// <param name="control">親コントロール</param>
	private Hashtable GetWrappedRepeaterItem(Control control)
	{
		Hashtable htResult = new Hashtable();

		// 商品個数
		var WddlItemCount = GetWrappedControl<WrappedDropDownList>(control, CONTROL_ID_ITEM_COUNT);
		htResult.Add(CONTROL_ID_ITEM_COUNT, WddlItemCount);

		// 親子フラグ
		var WhfFamilyFlg = GetWrappedControl<WrappedHiddenField>(control, CONTROL_ID_FAMILY_FLG);
		htResult.Add(CONTROL_ID_FAMILY_FLG,WhfFamilyFlg);

		// 商品単価
		var WhfSetItemPrice = GetWrappedControl<WrappedHiddenField>(control, CONTROL_ID_SET_ITEM_PRICE);
		htResult.Add(CONTROL_ID_SET_ITEM_PRICE, WhfSetItemPrice);

		// 商品累計
		var WsSetItemPriceTotal = GetWrappedControl<WrappedHtmlGenericControl>(control, CONTROL_ID_SET_ITEM_PRICE_TOTAL);
		htResult.Add(CONTROL_ID_SET_ITEM_PRICE_TOTAL, WsSetItemPriceTotal);

		// 商品ID
		var WhfProductId = GetWrappedControl<WrappedHiddenField>(control, CONTROL_ID_PRODUCT_ID);
		htResult.Add(CONTROL_ID_PRODUCT_ID, WhfProductId);

		// バリエーションID
		var WhfVariationId = GetWrappedControl<WrappedHiddenField>(control, CONTROL_ID_VARIATION_ID);
		htResult.Add(CONTROL_ID_VARIATION_ID, WhfVariationId);

		// カテゴリID
		var WhfCategoryId = GetWrappedControl<WrappedHiddenField>(control, CONTROL_ID_CATEGORY_ID);
		htResult.Add(CONTROL_ID_CATEGORY_ID, WhfCategoryId);

		// エラー文言
		var WsSetItemErrorMessage = GetWrappedControl<WrappedHtmlGenericControl>(control, CONTROL_ID_SET_ITEM_ERROR_MESSAGE);
		htResult.Add(CONTROL_ID_SET_ITEM_ERROR_MESSAGE, WsSetItemErrorMessage);

		return htResult;
	}

	/// <summary>
	/// モバイル用URL取得
	/// </summary>
	/// <returns></returns>
	protected string GetMobileUrl()
	{
		StringBuilder sbMobileUrl = new StringBuilder();
		sbMobileUrl.Append(Constants.PROTOCOL_HTTP).Append(Request.Url.Authority);
		sbMobileUrl.Append(Constants.PATH_MOBILESITE).Append(Constants.PAGE_MFRONT_PRODUCT_SET_LIST);
		sbMobileUrl.Append("?").Append(Constants.REQUEST_KEY_MFRONT_PAGE_ID).Append("=").Append(Constants.PAGEID_MFRONT_PRODUCT_SET_LIST);
		sbMobileUrl.Append("&").Append(Constants.REQUEST_KEY_MFRONT_SHOP_ID).Append("=").Append(this.ShopId);
		sbMobileUrl.Append("&").Append(Constants.REQUEST_KEY_MFRONT_PRODUCTSET_ID).Append("=").Append(this.ProductSetId);

		return sbMobileUrl.ToString();
	}

	/// <summary>
	/// 翻訳情報設定
	/// </summary>
	/// <param name="productList">商品情報リスト</param>
	/// <returns>翻訳後商品情報リスト</returns>
	private List<DataRowView> SetTranslationData(List<DataRowView> productList)
	{
		// 商品／バリエーション情報翻訳
		var products = productList.Select(
			drv => new ProductModel
			{
				ProductId = (string)drv[Constants.FIELD_PRODUCT_PRODUCT_ID]
			}).ToArray();
		var productTranslationSettings = NameTranslationCommon.GetProductAndVariationTranslationSettings(
			products,
			RegionManager.GetInstance().Region.LanguageCode,
			RegionManager.GetInstance().Region.LanguageLocaleId);

		productList = productList.Select(
			product => NameTranslationCommon.SetProductAndVariationTranslationDataToDataRowView(
				product,
				productTranslationSettings)).ToList();

		// 商品セット情報翻訳
		var productSetIdList = productList.Select(drv => (string)drv[Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID]).ToArray();
		var productSetTranslationSettings = NameTranslationCommon.GetProductSetTranslationSettings(
			productSetIdList,
			RegionManager.GetInstance().Region.LanguageCode,
			RegionManager.GetInstance().Region.LanguageLocaleId);

		productList = productList.Select(
			product => NameTranslationCommon.SetTranslationDataToDataRowView(
				product,
				Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET,
				productSetTranslationSettings)).ToList();

		return productList;
	}

	/// <summary>商品セットID</summary>
	protected string ProductSetId
	{
		get { return (string)ViewState["ProductSetId"]; }
		private set { ViewState["ProductSetId"] = value; }
	}
	/// <summary>親商品購入制限(下限)</summary>
	protected int ParentMin
	{
		get { return (int)ViewState["ParentMin"]; }
		private set { ViewState["ParentMin"] = value; }
	}
	/// <summary>親商品購入制限(下限)</summary>
	protected int ParentMax
	{
		get { return (int)ViewState["ParentMax"]; }
		private set { ViewState["ParentMax"] = value; }
	}
	/// <summary>子商品購入制限(下限)</summary>
	protected int ChildMin
	{
		get { return (int)ViewState["ChildMin"]; }
		private set { ViewState["ChildMin"] = value; }
	}
	/// <summary>子商品購入制限(下限)</summary>
	protected int ChildMax
	{
		get { return (int)ViewState["ChildMax"]; }
		private set { ViewState["ChildMax"] = value; }
	}
	/// <summary>遷移前URL</summary>
	protected string BeforeUrl
	{
		get { return (string)ViewState["Request.UrlReferrer.AbsoluteUri"]; }
		private set { ViewState["Request.UrlReferrer.AbsoluteUri"] = value; }
	}
	/// <summary>セット名称</summary>
	protected string SetName 
	{
		get { return (string)ViewState["SetName"]; }
		private set { ViewState["SetName"] = value; }
	}
	/// <summary>セット説明</summary>
	protected string Description
	{
		get { return (string)ViewState["Description"]; }
		private set { ViewState["Description"] = value; }
	}
}