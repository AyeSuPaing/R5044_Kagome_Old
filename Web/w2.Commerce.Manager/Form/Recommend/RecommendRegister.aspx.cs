/*
 =========================================================================================================
  Module      : レコメンド設定登録ページ処理(RecommendRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.Domain.Recommend;
using w2.Domain.Recommend.Helper;
using w2.App.Common.Recommend;
using w2.App.Common.Order;
using w2.App.Common.RefreshFileManager;
using w2.Common.Extensions;
using w2.Common.Web;
using w2.App.Common.Manager;
using w2.Domain.MenuAuthority.Helper;

public partial class Form_Recommend_RecommendRegister : RecommendPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 完了メッセージ非表示
		divComp.Visible = false;

		if (!IsPostBack)
		{
			// レコメンドID（※ボタン画像保存用）セット（GUIDで生成）
			this.TempRecommendId = Guid.NewGuid().ToString("N");

			// 表示コンポーネント初期化
			InitializeComponents();

			// ボタン画像ディレクトリ作成＆古い（作成日が1日以上経った）一時ボタン画像ファイルを削除
			var buttonImageOperator = new RecommendButtonImageOperator();
			buttonImageOperator.CreateRecommendButtonImageDirectory();
			buttonImageOperator.DeleteOldTempRecommendButtonImageFile();

			// レコメンド設定セット
			SetRecommend();

			// レコメンド区分変更イベント実行
			rblRecommendKbn_SelectedIndexChanged(sender, e);

			// アップセル対象アイテムセットイベント実行
			lbSetUpsellTargetItem_Click(sender, e);

			// ボタン画像プレビューイベント実行
			lbDisplayButtonImagePreview_Click(sender, e);

			//頒布会商品が設定されているかチェック
			RecommendItem_Subscription_Checked();
		}
	}

	/// <summary>
	/// レポートを見るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnToReport_Click(object sender, EventArgs e)
	{
		var url = SingleSignOnUrlCreator.CreateForWebForms(
			MenuAuthorityHelper.ManagerSiteType.Mp,
			new UrlCreator(Constants.PATH_ROOT_MP + Constants.PAGE_W2MP_MANAGER_RECOMMEND_REPORT)
				.AddParam(Constants.REQUEST_KEY_RECOMMEND_ID, this.RequestRecommendId)
				.CreateUrl());

		Response.Redirect(url);
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnToList_Click(object sender, EventArgs e)
	{
		Response.Redirect(this.SearchInfo.CreateRecommendListUrl(true));
	}

	/// <summary>
	/// コピー新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		var url = CreateRecommendRegisterUrl(this.RequestRecommendId, Constants.ACTION_STATUS_COPY_INSERT);
		Response.Redirect(url);
	}

	/// <summary>
	/// 登録・更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertUpdate_Click(object sender, EventArgs e)
	{
		// 登録？
		var isInsert = (this.ActionStatus != Constants.ACTION_STATUS_UPDATE);

		// 入力情報取得
		var input = GetInputData();

		// 入力チェック
		var success = input.Validate(isInsert);

		// 画面にエラーメッセージをセット
		trRecommendErrorMessagesTitle.Visible =
			trRecommendErrorMessages.Visible = (input.ErrorMessages.Length != 0);
		lbRecommendErrorMessages.Text = input.ErrorMessages;
		var errorMessages = input.ApplyConditionItemErrorMessages;
		trApplyConditionItemErrorMessagesTitle.Visible =
			trApplyConditionItemErrorMessages.Visible = (errorMessages.Length != 0);
		lbApplyConditionItemErrorMessages.Text = errorMessages;
		errorMessages =
			input.UpsellTargetItemErrorMessages + input.RecommendItemErrorMessages;
		trRecommendItemErrorMessagesTitle.Visible =
			trRecommendItemErrorMessages.Visible = (errorMessages.Length != 0);
		lbRecommendItemErrorMessages.Text = errorMessages;

		// エラーの場合、処理を抜ける
		if (success == false) return;

		// DB登録・更新
		var model = input.CreateModel();
		if (isInsert)
		{
			new RecommendService().Insert(model);
		}
		else
		{
			new RecommendService().Update(model);
		}

		// ボタン画像保存
		SaveRecommendButtonImageFile(model.RecommendId);

		// 各サイトの情報を最新状態にする
		RefreshFileManagerProvider.GetInstance(RefreshFileType.Recommend).CreateUpdateRefreshFile();

		// レコメンド登録・更新画面へ遷移（登録更新完了メッセージ表示）
		var url = 
			CreateRecommendRegisterUrl(model.RecommendId, Constants.ACTION_STATUS_UPDATE)
			+ string.Format("&{0}=1", REQUEST_KEY_SUCCESS);
		Response.Redirect(url);
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		// DBから削除
		new RecommendService().Delete(this.LoginOperatorShopId, this.RequestRecommendId);

		// 各サイトの情報を最新状態にする
		RefreshFileManagerProvider.GetInstance(RefreshFileType.Recommend).CreateUpdateRefreshFile();

		// ボタン画像削除
		new RecommendButtonImageOperator(ButtonImageType.AddItemPc).DeleteRecommendButtonImageFile(this.RequestRecommendId);
		new RecommendButtonImageOperator(ButtonImageType.AddItemSp).DeleteRecommendButtonImageFile(this.RequestRecommendId);

		// レコメンド一覧画面へ遷移
		Response.Redirect(this.SearchInfo.CreateRecommendListUrl(true));
	}

	/// <summary>
	/// レコメンド区分変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblRecommendKbn_SelectedIndexChanged(object sender, EventArgs e)
	{
		// レコメンドアイテム設定エリアの表示制御
		divRecommendItem.Visible = (rblRecommendKbn.SelectedValue != Constants.FLG_RECOMMEND_RECOMMEND_KBN_RECOMMEND_HTML);
		divUpSell.Visible = (rblRecommendKbn.SelectedValue == Constants.FLG_RECOMMEND_RECOMMEND_KBN_UP_SELL);
		divCrossSell.Visible = (rblRecommendKbn.SelectedValue == Constants.FLG_RECOMMEND_RECOMMEND_KBN_CROSS_SELL);

		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
		{
			// 商品タイプ更新
			rApplyConditionItemsBuy.DataSource = GetApplyConditionItems(
				Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_BUY).ToArray();
			rApplyConditionItemsNotBuy.DataSource = GetApplyConditionItems(
				Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_NOT_BUY).ToArray();
			rApplyConditionItemsBuy.DataBind();
			rApplyConditionItemsNotBuy.DataBind();
		}

		if (divCrossSell.Visible)
		{
			CheckOffRecommendItemAddQuantityType();
			cbRecommendItemAddQuantityType_CheckedChanged(sender, e);
		}
	}

	/// <summary>
	/// 適用条件アイテム削除（購入している）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rApplyConditionItemsBuy_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		int deleteItemIndex;
		if (int.TryParse(e.CommandArgument.ToString(), out deleteItemIndex) == false) return;

		// 適用条件アイテムを画面にセット
		SetApplyConditionItems(Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_BUY, deleteItemIndex);
	}

	/// <summary>
	/// 適用条件アイテム削除（購入していない）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rApplyConditionItemsNotBuy_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		int deleteItemIndex;
		if (int.TryParse(e.CommandArgument.ToString(), out deleteItemIndex) == false) return;

		// 適用条件アイテムを画面にセット
		SetApplyConditionItems(Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_NOT_BUY, deleteItemIndex);
	}

	/// <summary>
	/// 適用条件アイテムセット
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSetApplyConditionItem_Click(object sender, EventArgs e)
	{
		// 商品ID、商品バリエーションID取得
		var applyConditionItemValue = hfApplyConditionItem.Value.Split(',');
		if (applyConditionItemValue.Length != 2) return;
		hfApplyConditionItem.Value = string.Empty;
		var productId = applyConditionItemValue[0];
		var variationId = applyConditionItemValue[1];

		// レコメンド適用条件種別取得
		var recommendApplyConditionType = hfRecommendApplyConditionType.Value;
		hfRecommendApplyConditionType.Value = string.Empty;

		// 商品情報が存在する？
		var product =
			(productId == variationId)
			? ProductCommon.GetProductInfoUnuseMemberRankPrice(this.LoginOperatorShopId, productId)
			: ProductCommon.GetProductVariationInfo(this.LoginOperatorShopId, productId, variationId, string.Empty);
		if (product.Count != 0)
		{
			// 適用条件アイテム入力インスタンス作成
			var applyConditionItem = new RecommendApplyConditionItemInput()
			{
				ShopId = this.LoginOperatorShopId,
				RecommendId = this.RequestRecommendId,
				RecommendApplyConditionItemType = 
				Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE_NORMAL,
				RecommendApplyConditionType = recommendApplyConditionType,
				RecommendApplyConditionItemUnitType = (productId == variationId)
				? Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE_PRODUCT
				: Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_UNIT_TYPE_VARIATION,
				RecommendApplyConditionItemProductId = productId,
				RecommendApplyConditionItemVariationId = variationId,
				ProductName = (string)product[0][Constants.FIELD_PRODUCT_NAME],
				VariationName1 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
				VariationName2 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2],
				VariationName3 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3],
				FixedPurchaseFlg = (string)product[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG],
				SubscriptionBoxFlg = (string)product[0][Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG]
			};

			var applyConditionItems = new List<RecommendApplyConditionItemInput>();
			int itemIndex;
			if (int.TryParse(hfApplyConditionItemIndex.Value, out itemIndex))
			{
				// 登録されているものであれば変更
				hfApplyConditionItemIndex.Value = string.Empty;
				var i = 0;
				foreach (var item in GetApplyConditionItems(recommendApplyConditionType))
				{
					applyConditionItems.Add((i == itemIndex) ? applyConditionItem : item);
					i++;
				}
			}
			else
			{
				// 追加
				applyConditionItems.AddRange(GetApplyConditionItems(recommendApplyConditionType));
				applyConditionItems.Add(applyConditionItem);
			}

			// 適用条件アイテムを画面にセット
			SetApplyConditionItems(recommendApplyConditionType, applyConditionItems.ToArray());
		}
	}

	/// <summary>
	/// アップセル対象アイテム種別変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUpsellTargetItemType_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetRecommendItems(GetItems().ToArray());
		RecommendItem_Subscription_Checked();
	}

	/// <summary>
	/// アップセル対象アイテムセット
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSetUpsellTargetItem_Click(object sender, EventArgs e)
	{
		// 商品ID、商品バリエーションID取得
		var upsellTargetItemValue = hfUpsellTargetItem.Value.Split(',');
		hfUpsellTargetItem.Value = string.Empty;
		if (upsellTargetItemValue.Length != 2) return;
		var productId = upsellTargetItemValue[0];
		var variationId = upsellTargetItemValue[1];

		// 商品情報が存在する？
		var product =
			(productId == variationId)
			? ProductCommon.GetProductInfoUnuseMemberRankPrice(this.LoginOperatorShopId, productId)
			: ProductCommon.GetProductVariationInfo(this.LoginOperatorShopId, productId, variationId, string.Empty);
		if (product.Count != 0)
		{
			// アップセルアイテム入力インスタンス作成
			var upsellTargetItem = new RecommendUpsellTargetItemInput
			{
				ShopId = this.LoginOperatorShopId,
				RecommendId = this.RequestRecommendId,
				RecommendUpsellTargetItemType  = ddlUpsellTargetItemType.SelectedValue,
				RecommendUpsellTargetItemProductId = productId,
				RecommendUpsellTargetItemVariationId = variationId,
				ProductName = (string)product[0][Constants.FIELD_PRODUCT_NAME],
				VariationName1 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
				VariationName2 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2],
				VariationName3 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3],
				ShippingType = (string)product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE],
			};

			// 商品名、商品ID、商品バリエーションIDを画面にセット
			lUpsellTargetItem.Text = WebSanitizer.HtmlEncode(upsellTargetItem.CreateProductJointNameAndVariationId());
			hfRecommendUpsellTargetItemProductId.Value = productId;
			hfRecommendUpsellTargetItemVariationId.Value = variationId;
			hfRecommendUpsellTargetItemProductName.Value = upsellTargetItem.ProductName;
			hfRecommendUpsellTargetItemVariationName1.Value = upsellTargetItem.VariationName1;
			hfRecommendUpsellTargetItemVariationName2.Value = upsellTargetItem.VariationName2;
			hfRecommendUpsellTargetItemVariationName3.Value = upsellTargetItem.VariationName3;
			hfRecommendUpsellTargetItemShippingType.Value = upsellTargetItem.ShippingType;
			hfRecommendUpsellTargetItemFixedPurchaseFlg.Value = (string)product[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG];
			hfRecommendUpsellTargetItemSubscriptionBoxFlg.Value = (string)product[0][Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG];
		}
	}

	/// <summary>
	/// レコメンドアイテムセット
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSetRecommendItem_Click(object sender, EventArgs e)
	{
		// 商品ID、商品バリエーションID取得
		var recommendProductValue = hfRecommendItem.Value.Split(',');
		hfRecommendItem.Value = string.Empty;
		var productId = recommendProductValue[0];
		var variationId = recommendProductValue[1];

		// 商品情報が存在する？
		var product =
			(productId == variationId)
			? ProductCommon.GetProductInfoUnuseMemberRankPrice(this.LoginOperatorShopId, productId)
			: ProductCommon.GetProductVariationInfo(this.LoginOperatorShopId, productId, variationId, string.Empty);
		if (product.Count != 0)
		{
			// レコメンドアイテム入力インスタンス作成
			var recommendItem = new RecommendItemInput()
			{
				ShopId = this.LoginOperatorShopId,
				RecommendId = this.RequestRecommendId,
				RecommendItemType = Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_TYPE_NORMAL,
				RecommendItemProductId = productId,
				RecommendItemVariationId = variationId,
				ProductName = (string)product[0][Constants.FIELD_PRODUCT_NAME],
				VariationName1 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
				VariationName2 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2],
				VariationName3 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3],
				ShippingType = (string)product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE],
				FixedPurchaseFlg = (string)product[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG],
				SubscriptionBoxFlg = (string)product[0][Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG],
			};

			var recommendItems = new List<RecommendItemInput>();
			int itemIndex;
			if (int.TryParse(hfRecommendItemIndex.Value, out itemIndex))
			{
				// 登録されているものであれば変更
				hfRecommendItemIndex.Value = string.Empty;
				var i = 0;
				foreach (var item in GetItems())
				{
					recommendItems.Add((i == itemIndex) ? recommendItem : item);
					i++;
				}
			}
			else
			{
				// 追加
				recommendItems.AddRange(GetItems());
				recommendItems.Add(recommendItem);
			}

			// レコメンドアイテムを画面にセット
			SetRecommendItems(recommendItems.ToArray());
		}
	}

	/// <summary>
	/// 定期配送パターン設定セット
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSetFixedPurchaseShippingPattern_Click(object sender, EventArgs e)
	{
		var fixedPurchaseKbn = hfFixedPurchaseShippingPattern.Value.Split(',').First();
		var fixedPurchaseSetting = hfFixedPurchaseShippingPattern.Value.Substring(hfFixedPurchaseShippingPattern.Value.IndexOf(',') + 1);

		var recommendItems = GetItems().ToArray();
		int itemIndex;
		if (int.TryParse(hfFixedPurchaseShippingPatternIndex.Value, out itemIndex) 
			&& (recommendItems.Length > itemIndex))
		{
			recommendItems[itemIndex].FixedPurchaseKbn = fixedPurchaseKbn;
			recommendItems[itemIndex].FixedPurchaseSetting1 = fixedPurchaseSetting;
		}

		hfFixedPurchaseShippingPattern.Value = string.Empty;
		hfFixedPurchaseShippingPatternIndex.Value = string.Empty;

		SetRecommendItems(recommendItems);
	}

	/// <summary>
	/// レコメンドアイテム削除
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rRecommendItems_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		int itemIndex;
		if (int.TryParse(e.CommandArgument.ToString(), out itemIndex) == false) return;

		// 削除対象のインデックス以外をリストにする
		var recommendItems = GetItems().ToArray().Where((item, i) => i != itemIndex).ToArray();
		// レコメンドアイテムを画面にセット
		SetRecommendItems(recommendItems.ToArray());
	}

	/// <summary>
	/// 変更商品と同じ数量チェックボックスチェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbRecommendItemAddQuantityType_CheckedChanged(object sender, EventArgs e)
	{
		foreach (RepeaterItem ri in rRecommendItems.Items)
		{
			var check = ((CheckBox)ri.FindControl("cbRecommendItemAddQuantityType")).Checked;
			((TextBox)ri.FindControl("tbRecommendItemAddQuantity")).Enabled = 
				((check && rblRecommendKbn.SelectedValue == Constants.FLG_RECOMMEND_RECOMMEND_KBN_UP_SELL) == false);
		}
	}

	/// <summary>
	/// レコメンドアイテム種別変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlRecommendItemType_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetRecommendItems(GetItems().ToArray());
		RecommendItem_Subscription_Checked();
	}

	/// <summary>
	/// ボタン画像プレビュー表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayButtonImagePreview_Click(object sender, EventArgs e)
	{
		SetButtonImagePreview();
	}

	#region メソッド
	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 表示制御
		switch (this.ActionStatus)
		{
			// 新規登録・コピー新規登録
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				trDisplayRecommendId.Visible = false;
				trInputRecommendId.Visible = true;
				btnToReportTop.Visible = btnToReportBottom.Visible = false;
				btnInsertTop.Visible = btnInsertBottom.Visible = true;
				btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = false;
				btnDeleteTop.Visible = btnDeleteBottom.Visible = false;
				btnUpdateTop.Visible = btnUpdateBottom.Visible = false;
				break;
			// 更新
			case Constants.ACTION_STATUS_UPDATE:
				trInputRecommendId.Visible = false;
				trDisplayRecommendId.Visible = true;
				btnToReportTop.Visible = btnToReportBottom.Visible = true;
				btnInsertTop.Visible = btnInsertBottom.Visible = false;
				btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = true;
				btnDeleteTop.Visible = btnDeleteBottom.Visible = true;
				btnUpdateTop.Visible = btnUpdateBottom.Visible = true;
				// 登録・更新メッセージ表示
				divComp.Visible = this.IsDisplaySuccess;
				// 作成日・更新日・最終更新者表示
				trDateCreated.Visible = trDateChanged.Visible = trLastChanged.Visible = true;
				break;
		}

		var productTypeList = Constants.SUBSCRIPTION_BOX_OPTION_ENABLED
			? ValueText.GetValueItemArray(
				Constants.TABLE_RECOMMEND,
				Constants.FIELD_RECOMMEND_RECOMMEND_PRODUCT_TYPE_VALID_SUBSCRIPTION_BOX)
			: ValueText.GetValueItemArray(
				Constants.TABLE_RECOMMEND,
				Constants.FIELD_RECOMMEND_RECOMMEND_PRODUCT_TYPE);

		ddlUpsellTargetItemType.Items.AddRange(productTypeList);

		// レコメンド区分
		rblRecommendKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_RECOMMEND, Constants.FIELD_RECOMMEND_RECOMMEND_KBN));
		rblRecommendKbn.Items[0].Selected = true;

		// レコメンド表示区分PC/SP
		rblRecommendDisplayKbnPc.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_RECOMMEND, Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_KBN_PC));
		rblRecommendDisplayKbnPc.Items[0].Selected = true;
		rblRecommendDisplayKbnSp.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_RECOMMEND, Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_KBN_SP));
		rblRecommendDisplayKbnSp.Items[0].Selected = true;

		// レコメンド表示ページ
		rblRecommendDisplayPage.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_RECOMMEND, Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_PAGE));
		rblRecommendDisplayPage.Items[0].Selected = true;
	}

	/// <summary>
	/// レコメンド設定をセット
	/// </summary>
	private void SetRecommend()
	{
		switch (this.ActionStatus)
		{
			// 更新・コピー新規登録
			case Constants.ACTION_STATUS_UPDATE:
			case Constants.ACTION_STATUS_COPY_INSERT:
				// DBからデータを取得し画面にセット
				SetValues(GetRecommend());
				break;
			// 新規登録
			case Constants.ACTION_STATUS_INSERT:
				break;

			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}
	}

	/// <summary>
	/// DBから表示用レコメンド設定取得
	/// </summary>
	/// <returns>表示用レコメンド設定</returns>
	private RecommendContainer GetRecommend()
	{
		// レコメンド設定（表示用）取得
		var container = new RecommendService().GetContainer(this.LoginOperatorShopId, this.RequestRecommendId);
		if (container == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		return container;
	}

	/// <summary>
	/// 画面に値をセット
	/// </summary>
	/// <param name="container">表示用レコメンド設定</param>
	private void SetValues(RecommendContainer container)
	{
		// 入力情報作成
		var recommend = new RecommendInput(container);

		cbChatbotUseFlg.Checked = (recommend.ChatbotUseFlg == Constants.FLG_RECOMMEND_CHATBOT_USE_FLG_VALID);
		SetDataRecommendDisplay();

		// 基本情報
		tbRecommendId.Text = lRecommendId.Text = recommend.RecommendId;
		tbRecommendName.Text = recommend.RecommendName;
		tbDiscription.Text = recommend.Discription;
		rblRecommendKbn.SelectedValue = recommend.RecommendKbn;
		if (string.IsNullOrEmpty(recommend.DateEnd))
		{
			ucDisplayPeriod.SetStartDate(DateTime.Parse(recommend.DateBegin));
		}
		else
		{
			ucDisplayPeriod.SetPeriodDate(
				DateTime.Parse(recommend.DateBegin),
				DateTime.Parse(recommend.DateEnd));
		}
		tbPriority.Text = recommend.Priority;
		cbValidFlg.Checked = recommend.IsValid;
		lDateCreated.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				recommend.DateCreated,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
		lDateChanged.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				recommend.DateChanged,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
		lLastChanged.Text = WebSanitizer.HtmlEncode(recommend.LastChanged);

		// 適用条件アイテム設定
		SetApplyConditionItems(
			Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_BUY,
			recommend.ApplyConditionItems.Where(i => i.IsRecommendApplyConditionTypeBuy).ToArray());
		SetApplyConditionItems(
			Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_NOT_BUY,
			recommend.ApplyConditionItems.Where(i => i.IsRecommendApplyConditionTypeNotBuy).ToArray());

		// アップセル対象アイテム設定
		if (recommend.IsUpsell)
		{
			var upsellTargetItem = recommend.UpsellTargetItem;
			ddlUpsellTargetItemType.SelectedValue = upsellTargetItem.RecommendUpsellTargetItemType;
			hfUpsellTargetItem.Value = 
				string.Join(",", upsellTargetItem.RecommendUpsellTargetItemProductId, upsellTargetItem.RecommendUpsellTargetItemVariationId);
			hfRecommendUpsellTargetItemShippingType.Value = upsellTargetItem.ShippingType;
		}

		// レコメンドアイテム設定
		SetRecommendItems(recommend.Items);

		// レコメンドHTML設定
		rblRecommendDisplayKbnPc.SelectedValue = recommend.RecommendDisplayKbnPc;
		tbRecommendDisplayPc.Text = recommend.RecommendDisplayPc;
		rblRecommendDisplayKbnSp.SelectedValue = recommend.RecommendDisplayKbnSp;
		tbRecommendDisplaySp.Text = recommend.RecommendDisplaySp;
		// コピー新規の場合はボタン画像を一時ディレクトリにコピー
		if (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
		{
			CopyTempRecommendButtonImageFile(recommend.RecommendId, this.TempRecommendId);
		}

		// レコメンド表示ページ
		rblRecommendDisplayPage.SelectedValue = recommend.RecommendDisplayPage;

		// ワンタイム表示フラグ
		cbOnetimeFlg.Checked = recommend.IsOnetime;

		// 不整合アラート
		if (new RecommendService().IsConsistent(this.LoginOperatorShopId, this.RequestRecommendId) == false)
		{
			lMessages.Visible = true;
			lMessages.Text = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECCOMEND_DATA_CONSISTENCY_IS_MISSED));
		}
	}

	/// <summary>
	/// 適用条件アイテムを画面にセット
	/// </summary>
	/// <param name="recommendApplyConditionType">レコメンド適用条件種別</param>
	/// <param name="deleteItemIndex">削除対象のインデックス</param>
	private void SetApplyConditionItems(string recommendApplyConditionType, int deleteItemIndex)
	{
		// 削除対象のインデックス以外をリストにする
		var applyConditionItems =
			GetApplyConditionItems(recommendApplyConditionType).ToArray()
			.Where((item, i) => i != deleteItemIndex).ToArray();
		// 適用条件アイテムを画面にセット
		SetApplyConditionItems(recommendApplyConditionType, applyConditionItems);
	}
	/// <summary>
	/// 適用条件アイテムを画面にセット
	/// </summary>
	/// <param name="recommendApplyConditionType">レコメンド適用条件種別</param>
	/// <param name="applyConditionItems">適用条件アイテムリスト</param>
	private void SetApplyConditionItems(string recommendApplyConditionType, RecommendApplyConditionItemInput[] applyConditionItems)
	{
		// レコメンドアイテム並び順を更新
		applyConditionItems.Select((item, sortNo) => item.RecommendApplyConditionItemSortNo = sortNo.ToString());

		// データバインド
		var repeater =
			(recommendApplyConditionType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_BUY)
			? rApplyConditionItemsBuy
			: rApplyConditionItemsNotBuy;
		repeater.DataSource = applyConditionItems;
		repeater.DataBind();
	}

	/// <summary>
	/// レコメンドアイテムを画面にセット
	/// </summary>
	/// <param name="recommendItems">レコメンドアイテムリスト</param>
	private void SetRecommendItems(RecommendItemInput[] recommendItems)
	{
		// レコメンドアイテム並び順を更新
		recommendItems.Select((item, sortNo) => item.RecommendItemSortNo = sortNo.ToString());

		// データバインド
		rRecommendItems.DataSource = recommendItems;
		rRecommendItems.DataBind();
	}

	/// <summary>
	/// 画面の入力情報を取得
	/// </summary>
	/// <returns>レコメンド設定入力情報</returns>
	private RecommendInput GetInputData()
	{
		var input = new RecommendInput();

		// レコメンド設定
		input.ShopId = this.LoginOperatorShopId;
		input.RecommendId = StringUtility.ToHankaku(tbRecommendId.Text.Trim());
		input.RecommendName = tbRecommendName.Text.Trim();
		input.Discription = tbDiscription.Text.Trim();
		input.RecommendKbn = rblRecommendKbn.SelectedValue;
		input.DateBegin = ucDisplayPeriod.StartDateTimeString;
		input.DateEnd = string.IsNullOrEmpty(ucDisplayPeriod.EndDateTimeString)
			? null
			: ucDisplayPeriod.EndDateTimeString;
		input.Priority = StringUtility.ToHankaku(tbPriority.Text).Trim();
		input.ValidFlg = (cbValidFlg.Checked ? Constants.FLG_RECOMMEND_VALID_FLG_VALID : Constants.FLG_RECOMMEND_VALID_FLG_INVALID);
		input.LastChanged = this.LoginOperatorName;

		// 適用条件アイテム設定
		input.ApplyConditionItems = GetApplyConditionItems();

		// アップセル対象アイテム設定
		input.UpsellTargetItem = GetUpsellTargetItem();

		// レコメンドアイテム設定
		input.Items = GetItems().ToArray();

		// レコメンドHTML設定
		input.RecommendDisplayKbnPc = rblRecommendDisplayKbnPc.SelectedValue;
		input.RecommendDisplayPc = tbRecommendDisplayPc.Text;
		input.RecommendDisplayKbnSp = rblRecommendDisplayKbnSp.SelectedValue;
		input.RecommendDisplaySp = tbRecommendDisplaySp.Text;

		// レコメンド表示ページ
		input.RecommendDisplayPage = rblRecommendDisplayPage.SelectedValue;
		
		// ワンタイム表示フラグ
		input.OnetimeFlg = cbOnetimeFlg.Checked
			? Constants.FLG_RECOMMEND_ONETIME_FLG_VALID
			: Constants.FLG_RECOMMEND_ONETIME_FLG_INVALID;

		// Chatbot use flg
		input.ChatbotUseFlg = cbChatbotUseFlg.Checked
			? Constants.FLG_RECOMMEND_CHATBOT_USE_FLG_VALID
			: Constants.FLG_RECOMMEND_CHATBOT_USE_FLG_INVALID;

		return input;
	}

	/// <summary>
	/// 適用条件アイテム入力内容取得
	/// </summary>
	/// <returns>適用条件アイテム入力内容</returns>
	private RecommendApplyConditionItemInput[] GetApplyConditionItems()
	{
		var result = new List<RecommendApplyConditionItemInput>();

		// 購入している？適用条件アイテム取得
		var applyConditionItems = GetApplyConditionItems(Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_BUY);
		result.AddRange(applyConditionItems);
		// 購入していない？適用条件アイテム取得
		applyConditionItems = GetApplyConditionItems(Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_NOT_BUY);
		result.AddRange(applyConditionItems);

		return result.ToArray();
	}
	/// <summary>
	/// 適用条件アイテム入力内容取得
	/// </summary>
	/// <param name="recommendApplyConditionType">レコメンド適用条件種別</param>
	/// <returns>適用条件アイテム入力内容</returns>
	private IEnumerable<RecommendApplyConditionItemInput> GetApplyConditionItems(string recommendApplyConditionType)
	{
		// レコメンド適用条件種別に応じて、リピータ取得
		var repeater =
			(recommendApplyConditionType == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_TYPE_BUY)
			? rApplyConditionItemsBuy
			: rApplyConditionItemsNotBuy;

		int sortNo = 0;
		foreach (RepeaterItem ri in repeater.Items)
		{
			sortNo++;
			var item = new RecommendApplyConditionItemInput
			{
				ShopId = this.LoginOperatorShopId,
				RecommendId = this.RequestRecommendId,
				RecommendApplyConditionType = recommendApplyConditionType,
				RecommendApplyConditionItemType = ((DropDownList)ri.FindControl("ddlRecommendApplyConditionItemType")).SelectedValue,
				RecommendApplyConditionItemUnitType = ((HiddenField)ri.FindControl("hfRecommendApplyConditionItemUnitType")).Value,
				RecommendApplyConditionItemProductId = ((HiddenField)ri.FindControl("hfRecommendApplyConditionItemProductId")).Value,
				RecommendApplyConditionItemVariationId = ((HiddenField)ri.FindControl("hfRecommendApplyConditionItemVariationId")).Value,
				RecommendApplyConditionItemSortNo = sortNo.ToString(),
				ProductName = ((HiddenField)ri.FindControl("hfProductName")).Value,
				VariationName1 = ((HiddenField)ri.FindControl("hfVariationName1")).Value,
				VariationName2 = ((HiddenField)ri.FindControl("hfVariationName2")).Value,
				VariationName3 = ((HiddenField)ri.FindControl("hfVariationName3")).Value,
				FixedPurchaseFlg = ((HiddenField)ri.FindControl("hfFixedPurchaseFlg")).Value,
				SubscriptionBoxFlg = ((HiddenField)ri.FindControl("hfSubscriptionBoxFlg")).Value
			};
			yield return item;
		}
	}

	/// <summary>
	/// アップセル対象アイテム入力内容取得
	/// </summary>
	/// <returns>アップセル対象アイテム入力内容</returns>
	private RecommendUpsellTargetItemInput GetUpsellTargetItem()
	{
		// アップセル以外はnullを返す
		if (rblRecommendKbn.SelectedValue != Constants.FLG_RECOMMEND_RECOMMEND_KBN_UP_SELL) return null;

		return
			new RecommendUpsellTargetItemInput
			{
				ShopId = this.LoginOperatorShopId,
				RecommendId = this.RequestRecommendId,
				RecommendUpsellTargetItemType = ddlUpsellTargetItemType.SelectedValue,
				RecommendUpsellTargetItemProductId = hfRecommendUpsellTargetItemProductId.Value,
				RecommendUpsellTargetItemVariationId = hfRecommendUpsellTargetItemVariationId.Value,
				ProductName = hfRecommendUpsellTargetItemProductName.Value,
				VariationName1 = hfRecommendUpsellTargetItemVariationName1.Value,
				VariationName2 = hfRecommendUpsellTargetItemVariationName2.Value,
				VariationName3 = hfRecommendUpsellTargetItemVariationName3.Value,
				ShippingType = hfRecommendUpsellTargetItemShippingType.Value,
				FixedPurchaseFlg = hfRecommendUpsellTargetItemFixedPurchaseFlg.Value, 
				SubscriptionBoxFlg= hfRecommendUpsellTargetItemSubscriptionBoxFlg.Value,
			};
	}

	/// <summary>
	/// レコメンドアイテム入力内容取得
	/// </summary>
	/// <returns>レコメンドアイテム入力内容</returns>
	private IEnumerable<RecommendItemInput> GetItems()
	{
		int sortNo = 0;
		foreach (RepeaterItem ri in rRecommendItems.Items)
		{
			sortNo++;
			var item = new RecommendItemInput
			{
				ShopId = this.LoginOperatorShopId,
				RecommendId = this.RequestRecommendId,
				RecommendItemType = ((DropDownList)ri.FindControl("ddlRecommendItemType")).SelectedValue,
				RecommendItemProductId = ((HiddenField)ri.FindControl("hfRecommendItemProductId")).Value,
				RecommendItemVariationId = ((HiddenField)ri.FindControl("hfRecommendItemVariationId")).Value,
				RecommendItemAddQuantityType =
				(
					((CheckBox)ri.FindControl("cbRecommendItemAddQuantityType")).Checked
					&& (rblRecommendKbn.SelectedValue == Constants.FLG_RECOMMEND_RECOMMEND_KBN_UP_SELL)
				)
				? Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY_TYPE_SAME_QUANTITY
				: Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_ADD_QUANTITY_TYPE_SPECIFY_QUANTITY,
				RecommendItemAddQuantity = ((TextBox)ri.FindControl("tbRecommendItemAddQuantity")).Text.ConvertIfNullEmpty("0"),
				RecommendItemSortNo = sortNo.ToString(),
				ProductName = ((HiddenField)ri.FindControl("hfProductName")).Value,
				VariationName1 = ((HiddenField)ri.FindControl("hfVariationName1")).Value,
				VariationName2 = ((HiddenField)ri.FindControl("hfVariationName2")).Value,
				VariationName3 = ((HiddenField)ri.FindControl("hfVariationName3")).Value,
				ShippingType = ((HiddenField)ri.FindControl("hfShippingType")).Value,
				FixedPurchaseFlg = ((HiddenField)ri.FindControl("hfFixedPurchaseFlg")).Value,
				FixedPurchaseKbn = ((HiddenField)ri.FindControl("hfFixedPurchaseKbn")).Value,
				FixedPurchaseSetting1 = ((HiddenField)ri.FindControl("hfFixedPurchaseSetting1")).Value,
				IsValidateFixedPurchaseShippingPattern = bool.Parse(((HiddenField)ri.FindControl("hfIsValidateFixedPurchaseShippingPattern")).Value),
				SubscriptionBoxFlg = ((HiddenField)ri.FindControl("hfSubscriptionBoxFlg")).Value,
			};
			yield return item;
		}
	}

	/// <summary>
	/// 変更商品と同じ数量チェックボックスのチェックを外す
	/// </summary>
	private void CheckOffRecommendItemAddQuantityType()
	{
		foreach (RepeaterItem ri in rRecommendItems.Items)
		{
			((CheckBox)ri.FindControl("cbRecommendItemAddQuantityType")).Checked = false;
		}
	}

	/// <summary>
	/// レコメンドアイテム定期商品配送パターン表示するか？
	/// </summary>
	/// <param name="item">レコメンドアイテム</param>
	/// <returns>表示：True、非表示：False</returns>
	protected bool DisplayFixedPurchaseShippingPattern(RecommendItemInput item)
	{
		// レコメンドアイテムが通常商品の場合は非表示
		if (item.IsNormal) return false;

		// 「レコメンド設定 定期商品の配送パターンをレコメンド設定側で強制するか：FALSE」の場合、
		// アップセル対象の配送パターンが優先される
		// そのため、以下の条件の場合、非表示とする
		// ・レコメンド設定 定期商品の配送パターンをレコメンド設定側で強制するか：FALSE
		// ・アップセル
		// ・アップセル対象アイテム：定期商品
		// ・レコメンドアイテム：定期商品
		// ・アップセル対象アイテム配送種別 == レコメンドアイテム配送種別
		var upsellTargetItem = GetUpsellTargetItem();
		if ((Constants.RECOMMENDOPTION_IS_FORCED_FIXEDPURCHASE_SETTING_BY_RECOMMEND == false)
			&& (rblRecommendKbn.SelectedValue == Constants.FLG_RECOMMEND_RECOMMEND_KBN_UP_SELL)
			&& ((upsellTargetItem != null) && upsellTargetItem.IsFixedPurchase)
			&& item.IsFixedPurchase
			&& (item.ShippingType == upsellTargetItem.ShippingType)) return false;

		return true;
	}

	/// <summary>
	/// ボタン画像プレビュー表示
	/// </summary>
	private void SetButtonImagePreview()
	{
		// プレビューリンクを全て非表示
		aButtonImageAddItemPc.Visible =
			aButtonImageAddItemSp.Visible = false;

		// プレビューリンク表示
		foreach (ButtonImageType buttonImageType in Enum.GetValues(typeof(ButtonImageType)))
		{
			// 既にアップ済みのボタン画像
			SetButtonImagePreview(buttonImageType, false);

			// 一時保存中のボタン画像（※こちらを優先）
			SetButtonImagePreview(buttonImageType, true);
		}
	}
	/// <summary>
	/// ボタン画像プレビュー表示
	/// </summary>
	/// <param name="buttonImageType">ボタン画像種別</param>
	/// <param name="isTemp">一時保存中のボタン画像か？</param>
	private void SetButtonImagePreview(ButtonImageType buttonImageType, bool isTemp)
	{
		// レコメンドID取得
		var recommendId = (isTemp == false)
			? this.RequestRecommendId
			: this.TempRecommendId;

		// ファイルパス取得
		var buttonImageOperator = new RecommendButtonImageOperator(buttonImageType);
		var filePath = buttonImageOperator.GetRecommendButtonImageFilePath(recommendId, isTemp);
		if (string.IsNullOrEmpty(filePath)) return;

		// ボタン画像プレビュー表示
		switch (buttonImageType)
		{
			// PCレコメンド商品投入ボタン？
			case ButtonImageType.AddItemPc:
				aButtonImageAddItemPc.Visible = true;
				imgButtonImageAddItemPc.Src = filePath;
				break;
			// SPレコメンド商品投入ボタン？
			case ButtonImageType.AddItemSp:
				aButtonImageAddItemSp.Visible = true;
				imgButtonImageAddItemSp.Src = filePath;
				break;
		}
	}

	/// <summary>
	/// ボタン画像ファイル保存
	/// </summary>
	/// <param name="recommendId">レコメンドID</param>
	private void SaveRecommendButtonImageFile(string recommendId)
	{
		var buttonImageOperator = new RecommendButtonImageOperator();
		buttonImageOperator.SaveRecommendButtonImageFile(recommendId, this.TempRecommendId);
	}

	/// <summary>
	/// ボタン画像ファイルを一時ディレクトリにコピー
	/// </summary>
	/// <param name="recommendId">レコメンドID</param>
	/// <param name="tempRecommendId">一時レコメンドID</param>
	private void CopyTempRecommendButtonImageFile(string recommendId, string tempRecommendId)
	{
		var buttonImageOperator = new RecommendButtonImageOperator();
		buttonImageOperator.CopyTempRecommendButtonImageFile(recommendId, this.TempRecommendId);
		SetButtonImagePreview();
	}

	/// <summary>
	/// cbChatbotUseFlg on checked changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbChatbotUseFlg_OnCheckedChanged(object sender, EventArgs e)
	{
		SetDataRecommendDisplay();
	}

	/// <summary>
	/// 「チャットボットでも同様にレコメンドする」レ点が入るとき、画面制御を行う
	/// </summary>
	private void SetDataRecommendDisplay()
	{
		var beforeRecommendKbn = rblRecommendKbn.SelectedValue;
		var beforeRecommendDisplayPage = rblRecommendDisplayPage.SelectedValue;
		if (cbChatbotUseFlg.Checked)
		{
			var removeItemKbn = rblRecommendKbn.Items.FindByValue(Constants.FLG_RECOMMEND_RECOMMEND_KBN_RECOMMEND_HTML);
			removeItemKbn.Enabled = false;

			if (beforeRecommendKbn == Constants.FLG_RECOMMEND_RECOMMEND_KBN_RECOMMEND_HTML)
			{
				beforeRecommendKbn = Constants.FLG_RECOMMEND_RECOMMEND_KBN_UP_SELL;
			}
		}
		else
		{
			rblRecommendKbn.Items.Clear();
			rblRecommendKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_RECOMMEND, Constants.FIELD_RECOMMEND_RECOMMEND_KBN));
			rblRecommendDisplayPage.Items.Clear();
			rblRecommendDisplayPage.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_RECOMMEND, Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_PAGE));
		}

		rblRecommendKbn.SelectedValue = beforeRecommendKbn;
		rblRecommendDisplayPage.SelectedValue = beforeRecommendDisplayPage;
	}

	/// <summary>
	/// 頒布会商品が設定されているかチェック
	/// </summary>
	private void RecommendItem_Subscription_Checked()
	{
		if (rblRecommendKbn.SelectedValue == Constants.FLG_RECOMMEND_RECOMMEND_KBN_CROSS_SELL) return;

		//頒布会商品が設定されている場合クロスセルを選択不可に
		var removeItemKbn = rblRecommendKbn.Items.FindByValue(Constants.FLG_RECOMMEND_RECOMMEND_KBN_CROSS_SELL);

		if (rApplyConditionItemsBuy.Items.Cast<RepeaterItem>().Any(item =>
			((DropDownList)item.FindControl("ddlRecommendApplyConditionItemType")).SelectedValue == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE_SUBSCRIPTION_BOX))
		{
			removeItemKbn.Enabled = false;
			return;
		}

		if (rApplyConditionItemsNotBuy.Items.Cast<RepeaterItem>().Any(item => ((DropDownList)item.FindControl("ddlRecommendApplyConditionItemType")).SelectedValue == Constants.FLG_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_APPLY_CONDITION_ITEM_TYPE_SUBSCRIPTION_BOX))
		{
			removeItemKbn.Enabled = false;
			return;
		}

		if (ddlUpsellTargetItemType.SelectedValue == Constants.FLG_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE_SUBSCRIPTION_BOX)
		{
			removeItemKbn.Enabled = false;
			return;
		}

		if (rRecommendItems.Items.Cast<RepeaterItem>().Any(item => ((DropDownList)item.FindControl("ddlRecommendItemType")).SelectedValue == Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_TYPE_SUBSCRIPTION_BOX))
		{
			removeItemKbn.Enabled = false;
			return;
		}

		removeItemKbn.Enabled = true;
	}
	#endregion

	/// <summary>
	/// DDLの選択値の有効性を確認
	/// </summary>
	/// <param name="selectedValue">選択値</param>
	/// <returns>選択値</returns>
	protected string ValidateSelectedProductType(string selectedValue)
	{
		if ((selectedValue == Constants.FLG_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE_SUBSCRIPTION_BOX)
			&& (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED == false)) return Constants.FLG_RECOMMENDUPSELLTARGETITEM_RECOMMEND_UPSELL_TARGET_ITEM_TYPE_NORMAL;

		return selectedValue;
	}

	#region プロパティ
	/// <summary>一時レコメンドID（※ボタン画像保存用）</summary>
	protected string TempRecommendId
	{
		get { return (string)ViewState["TempRecommendId"]; }
		set { ViewState["TempRecommendId"] = value; }
	}
	/// <summary>商品タイプ</summary>
	protected ListItemCollection ProductTypeList
	{
		get
		{
			var productTypeList = (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED
				&& (rblRecommendKbn.SelectedValue != Constants.FLG_RECOMMEND_RECOMMEND_KBN_CROSS_SELL))
				? ValueText.GetValueItemList(
					Constants.TABLE_RECOMMEND,
					Constants.FIELD_RECOMMEND_RECOMMEND_PRODUCT_TYPE_VALID_SUBSCRIPTION_BOX)
				: ValueText.GetValueItemList(
					Constants.TABLE_RECOMMEND,
					Constants.FIELD_RECOMMEND_RECOMMEND_PRODUCT_TYPE);

			return productTypeList;
		}
	}
	#endregion
}