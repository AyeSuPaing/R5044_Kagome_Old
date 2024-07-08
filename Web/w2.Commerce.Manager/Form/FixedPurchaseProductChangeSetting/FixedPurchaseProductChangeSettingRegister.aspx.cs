/*
=========================================================================================================
  Module      : 定期商品変更設定登録/編集画面処理(FixedPurchaseProductChangeSettingRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using w2.App.Common.Order;
using w2.Common.Web;

public partial class Form_FixedPurchaseProductChangeSetting_FixedPurchaseProductChangeSettingRegister : BasePage
{
	/// <summary>商品セットイベント種別：変更元商品</summary>
	protected const string CONST_PRODUCT_SET_EVENT_TYPE_BEFORE_ITEM = "BEFORE_ITEM";
	/// <summary>商品セットイベント種別：変更後商品</summary>
	protected const string CONST_PRODUCT_SET_EVENT_TYPE_AFTER_ITEM = "AFTER_ITEM";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// アクションステータスチェック
			if (this.ActionStatus != Constants.ACTION_STATUS_UPDATE
				&& this.ActionStatus != Constants.ACTION_STATUS_COPY_INSERT
				&& this.ActionStatus != Constants.ACTION_STATUS_INSERT)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// コンポーネント初期化
			InitializeComponents();

			var uniqueKey = Request[Constants.REQUEST_KEY_UNIQUE_KEY];
			switch (this.ActionStatus)
			{
				case Constants.ACTION_STATUS_INSERT:
					if (string.IsNullOrEmpty(uniqueKey) == false)
					{
						this.Input = (FixedPurchaseProductChangeSettingInput)Session[Constants.SESSION_KEY_PARAM_FOR_BACK + uniqueKey];
						rBeforeChangeItems.DataSource = this.Input.BeforeChangeItems;
						rAfterChangeItems.DataSource = this.Input.AfterChangeItems;
						this.ChangeItemsShippingType = this.Input.BeforeChangeItems[0].ShippingType;
						break;
					}
					this.Input = new FixedPurchaseProductChangeSettingInput();
					break;

				case Constants.ACTION_STATUS_UPDATE:
				case Constants.ACTION_STATUS_COPY_INSERT:
					if (Session[Constants.SESSION_KEY_PARAM_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING] == null)
					{
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}

					this.Input = string.IsNullOrEmpty(uniqueKey) == false
						? (FixedPurchaseProductChangeSettingInput)Session[Constants.SESSION_KEY_PARAM_FOR_BACK + uniqueKey]
						: (FixedPurchaseProductChangeSettingInput)Session[Constants.SESSION_KEY_PARAM_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING];

					rBeforeChangeItems.DataSource = this.Input.BeforeChangeItems;
					rAfterChangeItems.DataSource = this.Input.AfterChangeItems;
					this.ChangeItemsShippingType = this.Input.BeforeChangeItems[0].ShippingType;
					break;
			}

			DataBind();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				trEditTitle.Visible = false;
				break;

			case Constants.ACTION_STATUS_UPDATE:
				trRegistTitle.Visible = false;
				break;
		}
	}

	/// <summary>
	/// 確認するボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnToConfirm_Click(object sender, EventArgs e)
	{
		// 入力値セット
		SetInputData();

		// 登録か
		var isInsert = (this.ActionStatus != Constants.ACTION_STATUS_UPDATE);

		// 入力値チェック
		var result = this.Input.Validate(isInsert);

		if (result)
		{
			// 定期変更元商品の有効性チェック
			foreach (var beforeChangeItem in this.Input.BeforeChangeItems)
			{
				var errorMessage = beforeChangeItem.CheckValidProduct();
				if (string.IsNullOrEmpty(errorMessage)) continue;
				// エラー画面へ
				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// 定期変更後商品の有効性チェック
			foreach (var afterChangeItem in this.Input.AfterChangeItems)
			{
				var errorMessage = afterChangeItem.CheckValidProduct();
				if (string.IsNullOrEmpty(errorMessage)) continue;
				// エラー画面へ
				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}

		if (result)
		{
			Session[Constants.SESSION_KEY_PARAM_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING] = this.Input;
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_CONFIRM)
				.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus)
				.CreateUrl();
			Response.Redirect(url);
		}

		lErrorMessages.Text = this.Input.ErrorMessages;
		trErrorMessagesTitle.Visible = true;
		trErrorMessages.Visible = true;
		rBeforeChangeItems.DataSource = this.Input.BeforeChangeItems;
		rAfterChangeItems.DataSource = this.Input.AfterChangeItems;
		DataBind();
	}

	/// <summary>
	/// 定期変更元商品追加ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSetBeforeChangeItem_Click(object sender, EventArgs e)
	{
		var productId = hfProductId.Value;
		var variationId = hfVariationId.Value;
		var changeItemIndex = hfChangeItemIndex.Value;

		var product = (productId == variationId)
			? ProductCommon.GetProductInfoUnuseMemberRankPrice(this.LoginOperatorShopId, productId)
			: ProductCommon.GetProductVariationInfo(this.LoginOperatorShopId, productId, variationId, string.Empty);

		// 商品情報が存在するか
		if (product.Count < 1) return;

		if (this.Input.BeforeChangeItems == null) this.Input.BeforeChangeItems = new List<FixedPurchaseBeforeChangeItemInput>();

		// 配送種別を設定
		if (string.IsNullOrEmpty(this.ChangeItemsShippingType))
		{
			this.ChangeItemsShippingType = StringUtility.ToEmpty(product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE]);
		}

		var beforeChangeItemInput = new FixedPurchaseBeforeChangeItemInput
		{
			ItemUnitType = hfUnitType.Value,
			ShopId = this.LoginOperatorShopId,
			ProductId = productId,
			VariationId = hfUnitType.Value == Constants.FLG_FIXEDPURCHASEBEFOREPRODUCTCHANGEITEM_ITEM_UNIT_TYPE_VARIATION
				? variationId
				: string.Empty,
			ProductName = StringUtility.ToEmpty(product[0][Constants.FIELD_PRODUCT_NAME]),
			ShippingType = StringUtility.ToEmpty(product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE]),
			LastChanged = this.LoginOperatorName,
		};

		int index;
		if (int.TryParse(changeItemIndex, out index))
		{
			this.Input.BeforeChangeItems[index] = beforeChangeItemInput;
		}
		else
		{
			this.Input.BeforeChangeItems.Add(beforeChangeItemInput);
		}
		rBeforeChangeItems.DataSource = this.Input.BeforeChangeItems;
		rAfterChangeItems.DataSource = this.Input.AfterChangeItems;
		rBeforeChangeItems.DataBind();
		rAfterChangeItems.DataBind();
		btnToConfirmTop.Enabled = btnToConfirmBottom.Enabled = this.IsEnabledConfirmButton;
	}

	/// <summary>
	/// 定期変更後商品追加ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSetAfterChangeItem_Click(object sender, EventArgs e)
	{
		var productId = hfProductId.Value;
		var variationId = hfVariationId.Value;
		var changeItemIndex = hfChangeItemIndex.Value;

		// 商品情報が存在する？
		var product = (productId == variationId)
			? ProductCommon.GetProductInfoUnuseMemberRankPrice(this.LoginOperatorShopId, productId)
			: ProductCommon.GetProductVariationInfo(this.LoginOperatorShopId, productId, variationId, string.Empty);
		if (product.Count < 1) return;

		// 配送種別を設定
		if (string.IsNullOrEmpty(this.ChangeItemsShippingType))
		{
			this.ChangeItemsShippingType = StringUtility.ToEmpty(product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE]);
		}

		if (this.Input.AfterChangeItems == null) this.Input.AfterChangeItems = new List<FixedPurchaseAfterChangeItemInput>();

		var afterChangeItemInput = new FixedPurchaseAfterChangeItemInput
		{
			ItemUnitType = hfUnitType.Value,
			ShopId = this.LoginOperatorShopId,
			ProductId = productId,
			VariationId = hfUnitType.Value == Constants.FLG_FIXEDPURCHASEAFTERPRODUCTCHANGEITEM_ITEM_UNIT_TYPE_VARIATION
				? variationId
				: string.Empty,
			ProductName = StringUtility.ToEmpty(product[0][Constants.FIELD_PRODUCT_NAME]),
			ShippingType = StringUtility.ToEmpty(product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE]),
			LastChanged = this.LoginOperatorName,
		};

		int index;
		if (int.TryParse(changeItemIndex, out index))
		{
			this.Input.AfterChangeItems[index] = afterChangeItemInput;
		}
		else
		{
			this.Input.AfterChangeItems.Add(afterChangeItemInput);
		}
		rBeforeChangeItems.DataSource = this.Input.BeforeChangeItems;
		rAfterChangeItems.DataSource = this.Input.AfterChangeItems;
		rBeforeChangeItems.DataBind();
		rAfterChangeItems.DataBind();
		btnToConfirmTop.Enabled = btnToConfirmBottom.Enabled = this.IsEnabledConfirmButton;
	}

	/// <summary>
	/// 定期変更元商品削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteBeforeChangeItem_Click(object sender, EventArgs e)
	{
		var itemIndex = int.Parse(((Button)sender).CommandArgument);
		this.Input.BeforeChangeItems.RemoveAt(itemIndex);

		// 変更商品がなければ配送種別をリセット
		var changeItemsCount = 0;
		changeItemsCount += this.Input.BeforeChangeItems != null
			? this.Input.BeforeChangeItems.Count
			: 0;
		changeItemsCount += this.Input.AfterChangeItems != null
			? this.Input.AfterChangeItems.Count
			: 0;
		if (changeItemsCount < 1)
		{
			this.ChangeItemsShippingType = string.Empty;
		}

		rBeforeChangeItems.DataSource = this.Input.BeforeChangeItems;
		rBeforeChangeItems.DataBind();
	}

	/// <summary>
	/// 定期変更後商品削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteAfterChangeItem_Click(object sender, EventArgs e)
	{
		var itemIndex = int.Parse(((Button)sender).CommandArgument);
		this.Input.AfterChangeItems.RemoveAt(itemIndex);
		rAfterChangeItems.DataSource = this.Input.AfterChangeItems;
		rAfterChangeItems.DataBind();
	}

	/// <summary>
	/// 商品リスト画面URL取得
	/// </summary>
	/// <param name="isAddTypeVariation">バリエーション単位での追加か</param>
	/// <param name="isSelectProduct">商品選択か</param>
	/// <returns>商品リスト画面URL</returns>
	protected string GetProductListUrl(bool isAddTypeVariation, bool isSelectProduct = false)
	{
		var beforeProductCount = this.Input.BeforeChangeItems != null
			? this.Input.BeforeChangeItems.Count
			: 0;
		var afterProductCount = this.Input.AfterChangeItems != null
			? this.Input.AfterChangeItems.Count
			: 0;
		var shippingType = string.Empty;
		// 変更商品の配送種別が空ではない
		// かつ、選択商品の場合選択済みの商品数が1つより多い
		if ((string.IsNullOrEmpty(this.ChangeItemsShippingType) == false)
			&& ((isSelectProduct == false)
				|| (beforeProductCount + afterProductCount) > 1))
		{
			shippingType = this.ChangeItemsShippingType;
		}

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH)
			.AddParam(
				Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN,
				isAddTypeVariation
					? Constants.KBN_PRODUCT_SEARCH_VARIATION
					: Constants.KBN_PRODUCT_SEARCH_PRODUCT)
			.AddParam(
				Constants.REQUEST_KEY_PRODUCT_VALID_FLG,
				Constants.FLG_PRODUCT_VALID_FLG_VALID)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE, shippingType)
			.AddParam(
				Constants.REQUEST_KEY_PRODUCT_FIXEDPURCHASE_PRODUCT,
				Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_VALID)
			.CreateUrl();
		return WebSanitizer.UrlAttrHtmlEncode(url);
	}

	/// <summary>
	/// 入力情報をセット
	/// </summary>
	private void SetInputData()
	{
		if (this.ActionStatus != Constants.ACTION_STATUS_UPDATE)
		{
			this.Input.FixedPurchaseProductChangeId = tbFixedPurchaseProductChangeId.Text;
		}
		this.Input.FixedPurchaseProductChangeName = tbFixedPurchaseProductChangeName.Text;
		this.Input.Priority = tbPriority.Text;
		this.Input.ValidFlg = cbValidFlg.Checked
			? Constants.FLG_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID
			: Constants.FLG_FIXEDPURCHASEPRODUCTCHANGESETTING_INVALID;
		this.Input.LastChanged = this.LoginOperatorName;
	}

	/// <summary>定期変更商品設定入力クラス</summary>
	protected FixedPurchaseProductChangeSettingInput Input
	{
		get { return (FixedPurchaseProductChangeSettingInput)ViewState["Input"]; }
		set { ViewState["Input"] = value; }
	}
	/// <summary>変更商品の配送種別</summary>
	private string ChangeItemsShippingType
	{
		get { return (string)ViewState["ChangeItemsShippingType"]; }
		set { ViewState["ChangeItemsShippingType"] = value; }
	}
	protected bool IsEnabledConfirmButton
	{
		get
		{
			// 変更元商品がnullではない
			// かつ、変更元商品数が1以上
			// かつ、変更後商品がnullではない
			// かつ、変更後商品数が1以上
			return (this.Input.BeforeChangeItems != null)
				&& (this.Input.BeforeChangeItems.Count > 0)
				&& (this.Input.AfterChangeItems != null)
				&& (this.Input.AfterChangeItems.Count > 0);

		}
	}
}
