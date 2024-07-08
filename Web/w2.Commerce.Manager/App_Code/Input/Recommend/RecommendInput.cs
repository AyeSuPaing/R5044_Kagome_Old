/*
=========================================================================================================
  Module      : レコメンド設定入力クラス (RecommendInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using System.Linq;
using w2.App.Common.Input;
using w2.Domain.Recommend;
using w2.Domain.Recommend.Helper;
using w2.App.Common.Order;

/// <summary>
/// レコメンド設定入力クラス
/// </summary>
[Serializable]
public class RecommendInput : InputBase<RecommendModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public RecommendInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="container">レコメンド設定</param>
	public RecommendInput(RecommendContainer container)
		: this()
	{
		this.ShopId = container.ShopId;
		this.RecommendId = container.RecommendId;
		this.RecommendName = container.RecommendName;
		this.Discription = container.Discription;
		this.RecommendDisplayPage = container.RecommendDisplayPage;
		this.RecommendKbn = container.RecommendKbn;
		this.DateBegin = container.DateBegin.ToString();
		this.DateEnd = (container.DateEnd != null) ? container.DateEnd.ToString() : null;
		this.Priority = container.Priority.ToString();
		this.ValidFlg = container.ValidFlg;
		this.RecommendDisplayKbnPc = container.RecommendDisplayKbnPc;
		this.RecommendDisplayPc = container.RecommendDisplayPc;
		this.RecommendDisplayKbnSp = container.RecommendDisplayKbnSp;
		this.RecommendDisplaySp = container.RecommendDisplaySp;
		this.DateCreated = container.DateCreated.ToString();
		this.DateChanged = container.DateChanged.ToString();
		this.LastChanged = container.LastChanged;
		// レコメンド適用条件アイテムリスト
		this.ApplyConditionItems =
			container.ApplyConditionItems.Select(i => new RecommendApplyConditionItemInput(i)).ToArray();
		// レコメンドアップセル対象アイテム
		if (container.IsUpsell)
		{
			this.UpsellTargetItem = (container.UpsellTargetItem != null)
				? new RecommendUpsellTargetItemInput(container.UpsellTargetItem)
				: new RecommendUpsellTargetItemInput();
		}
		// レコメンドアイテムリスト
		this.Items = container.Items.Select(i => new RecommendItemInput(i)).ToArray();
		this.OnetimeFlg = container.OnetimeFlg;
		this.ChatbotUseFlg = container.ChatbotUseFlg;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override RecommendModel CreateModel()
	{
		// レコメンド設定
		var model = new RecommendModel
		{
			ShopId = this.ShopId,
			RecommendId = this.RecommendId,
			RecommendName = this.RecommendName,
			Discription = this.Discription,
			RecommendDisplayPage = this.RecommendDisplayPage,
			RecommendKbn = this.RecommendKbn,
			DateBegin = DateTime.Parse(this.DateBegin),
			DateEnd = (this.DateEnd != null) ? DateTime.Parse(this.DateEnd) : (DateTime?)null,
			Priority = int.Parse(this.Priority),
			ValidFlg = this.ValidFlg,
			RecommendDisplayKbnPc = this.RecommendDisplayKbnPc,
			RecommendDisplayPc = this.RecommendDisplayPc,
			RecommendDisplayKbnSp = this.RecommendDisplayKbnSp,
			RecommendDisplaySp = this.RecommendDisplaySp,
			LastChanged = this.LastChanged,
			OnetimeFlg = this.OnetimeFlg,
			ChatbotUseFlg = this.ChatbotUseFlg,
		};
		// レコメンド適用条件アイテム
		model.ApplyConditionItems = this.ApplyConditionItems.Select(i => i.CreateModel(model)).ToArray();
		// レコメンドアップセル対象アイテム
		if (this.IsUpsell)
		{
			model.UpsellTargetItem = this.UpsellTargetItem.CreateModel(model);
		}
		// レコメンドアイテム
		if (this.IsUpsell || this.IsCrosssell)
		{
			model.Items = this.Items.Select(i => i.CreateModel(model)).ToArray();
		}
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <param name="checkDuplication">重複チェックするか</param>
	/// <returns>正常：true、エラー：false</returns>
	public bool Validate(bool checkDuplication)
	{
		var result = true;

		// レコメンド設定
		var errorMessage = new StringBuilder();
		errorMessage.Append(Validator.Validate("Recommend", this.DataSource));
		if (checkDuplication && new RecommendService().Get(this.ShopId, this.RecommendId) != null)
		{
			// 重複チェックエラー
			errorMessage.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_DUPLICATION)
				.Replace("@@ 1 @@", "レコメンドID")).Append("<br />");
		}

		// レコメンド適用条件アイテムの重複チェック
		var isApplyConditionItemsDuplicate  = this.ApplyConditionItems
			.GroupBy(
				applyConditionItems => new {
					applyConditionItems.ShopId,
					applyConditionItems.RecommendId,
					applyConditionItems.RecommendApplyConditionType,
					applyConditionItems.RecommendApplyConditionItemType,
					applyConditionItems.RecommendApplyConditionItemUnitType,
					applyConditionItems.RecommendApplyConditionItemProductId,
					applyConditionItems.RecommendApplyConditionItemVariationId
				})
			.Any(applyConditionItems => applyConditionItems.Count() > 1);

		if (isApplyConditionItemsDuplicate)
		{
			errorMessage.Append(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_APPLY_CONDITION_ITEMS_DUPLICATION)).Append("<br />");
		}

		// レコメンドアイテムの重複チェック
		var isItemsDuplicate = this.Items
			.GroupBy(
				items => new
				{
					items.ShopId,
					items.RecommendId,
					items.RecommendItemType,
					items.RecommendItemProductId,
					items.RecommendItemVariationId
				})
			.Any(items => items.Count() > 1);

		if (isItemsDuplicate)
		{
			errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_ITEMS_DUPLICATION)).Append("<br />");
		}

		// 「通常or定期→頒布会・頒布会→通常or定期」の場合はエラー
		if (this.IsUpsell)
		{
			foreach (var item in this.Items)
			{
				if ((item.IsSubscriptionBox && this.UpsellTargetItem.IsSubscriptionBox)
					|| ((item.IsSubscriptionBox == false) && (this.UpsellTargetItem.IsSubscriptionBox == false))) continue;

				errorMessage.Append(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_ITEMS_UPSELL_TARGET_SUBSCRIPTION_BOX)).Append("<br />");
				break;
			}
		}

		//クロスセルで頒布会商品選択していないかチェック
		if (this.IsCrosssell)
		{
			foreach (var item in ApplyConditionItems)
			{
				if (item.RecommendApplyConditionItemType != Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_TYPE_SUBSCRIPTION_BOX) continue;

				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_ITEMS_INVALID_SUBSCRIPTION_BOX)
					.Replace("@@ 1 @@", item.ProductName)).Append("<br />");

				item.ErrorMessages += errorMessage.ToString();
				if (item.ErrorMessages.Length != 0) result = false;
			}

			foreach (var item in this.Items)
			{
				if (item.RecommendItemType != Constants.FLG_RECOMMENDITEM_RECOMMEND_ITEM_TYPE_SUBSCRIPTION_BOX) continue;

				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_ITEMS_INVALID_SUBSCRIPTION_BOX)
					.Replace("@@ 1 @@", item.ProductName)).Append("<br />");

				item.ErrorMessages += errorMessage.ToString();
				if (item.ErrorMessages.Length != 0) result = false;
			}
		}

		// エラーメッセージをセット
		this.ErrorMessages = errorMessage.ToString();
		if (this.ErrorMessages.Length != 0) result = false;

		// レコメンド適用条件アイテム
		foreach (var applyConditionItem in this.ApplyConditionItems)
		{
			applyConditionItem.Validate();
			applyConditionItem.ErrorMessages +=
				CheckValidProduct(applyConditionItem.ShopId, applyConditionItem.RecommendApplyConditionItemProductId);
			if (applyConditionItem.ErrorMessages.Length != 0) result = false;
		}

		// レコメンドアップセル対象アイテム
		if (this.IsUpsell)
		{
			this.UpsellTargetItem.Validate();
			this.UpsellTargetItem.ErrorMessages +=
				CheckValidProduct(this.UpsellTargetItem.ShopId, this.UpsellTargetItem.RecommendUpsellTargetItemProductId);
			if (this.UpsellTargetItem.ErrorMessages.Length != 0) result = false;
		}

		// レコメンドアイテム入力チェック
		if (this.IsUpsell || this.IsCrosssell)
		{
			foreach (var item in this.Items)
			{
				item.Validate();
				item.ErrorMessages += CheckValidProduct(item.ShopId, item.RecommendItemProductId);

				// レコメンドアイテムにレコメンドアップセル対象アイテムが含まれているか？チェック
				if (this.IsUpsell)
				{
					if ((this.UpsellTargetItem.ShopId == item.ShopId)
						&& (this.UpsellTargetItem.RecommendUpsellTargetItemProductId == item.RecommendItemProductId)
						&& (this.UpsellTargetItem.RecommendUpsellTargetItemVariationId == item.RecommendItemVariationId)
						&& (this.UpsellTargetItem.IsFixedPurchase == item.IsFixedPurchase))
					{
						item.ErrorMessages +=
							WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECOMMEND_ITEM_UPSELL_TARGET)
								.Replace("@@ 1 @@", item.RecommendItemProductId)
								.Replace("@@ 2 @@", item.IsFixedPurchase ? "定期商品" : "通常商品");
					}
				}

				if (item.ErrorMessages.Length != 0) result = false;
			}
		}

		return result;
	}

	/// <summary>
	/// 商品有効性チェック
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckValidProduct(string shopId, string productId)
	{
		var product = ProductCommon.GetProductInfoUnuseMemberRankPrice(shopId, productId);
		if (product.Count == 0)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_DELETE).Replace("@@ 1 @@", productId);
		}
		else if ((string)product[0][Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID).Replace("@@ 1 @@", productId);
		}

		return string.Empty;
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_SHOP_ID] = value; }
	}
	/// <summary>レコメンドID</summary>
	public string RecommendId
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_ID]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_ID] = value; }
	}
	/// <summary>レコメンド名（管理用）</summary>
	public string RecommendName
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_NAME]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_NAME] = value; }
	}
	/// <summary>説明（管理用）</summary>
	public string Discription
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_DISCRIPTION]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_DISCRIPTION] = value; }
	}
	/// <summary>レコメンド表示ページ</summary>
	public string RecommendDisplayPage
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_PAGE]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_PAGE] = value; }
	}
	/// <summary>レコメンド区分</summary>
	public string RecommendKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_KBN]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_KBN] = value; }
	}
	/// <summary>開始日時</summary>
	public string DateBegin
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_DATE_BEGIN]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_DATE_BEGIN] = value; }
	}
	/// <summary>終了日時</summary>
	public string DateEnd
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_DATE_END]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_DATE_END] = value; }
	}
	/// <summary>適用優先順</summary>
	public string Priority
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_PRIORITY]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_PRIORITY] = value; }
	}
	/// <summary>有効フラグ</summary>
	public string ValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_VALID_FLG] = value; }
	}
	/// <summary>レコメンド表示区分PC</summary>
	public string RecommendDisplayKbnPc
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_KBN_PC]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_KBN_PC] = value; }
	}
	/// <summary>レコメンド表示PC</summary>
	public string RecommendDisplayPc
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_PC]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_PC] = value; }
	}
	/// <summary>レコメンド表示区分SP</summary>
	public string RecommendDisplayKbnSp
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_KBN_SP]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_KBN_SP] = value; }
	}
	/// <summary>レコメンド表示SP</summary>
	public string RecommendDisplaySp
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_SP]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_SP] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_LAST_CHANGED] = value; }
	}
	/// <summary>ワンタイム表示フラグ</summary>
	public string OnetimeFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_ONETIME_FLG]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_ONETIME_FLG] = value; }
	}
	/// <summary>
	/// アップセルか？
	/// </summary>
	public bool IsUpsell
	{
		get { return (this.RecommendKbn == Constants.FLG_RECOMMEND_RECOMMEND_KBN_UP_SELL); }
	}
	/// <summary>
	/// クロスセルか？
	/// </summary>
	public bool IsCrosssell
	{
		get { return (this.RecommendKbn == Constants.FLG_RECOMMEND_RECOMMEND_KBN_CROSS_SELL); }
	}
	/// <summary>
	/// レコメンドHTMLか？
	/// </summary>
	public bool IsRecommendHtml
	{
		get { return (this.RecommendKbn == Constants.FLG_RECOMMEND_RECOMMEND_KBN_RECOMMEND_HTML); }
	}
	/// <summary>
	/// 有効フラグが有効？
	/// </summary>
	public bool IsValid { get { return (this.ValidFlg == Constants.FLG_RECOMMEND_VALID_FLG_VALID); } }
	/// <summary>ワンタイム表示フラグ有効か</summary>
	public bool IsOnetime
	{
		get { return (this.OnetimeFlg == Constants.FLG_RECOMMEND_ONETIME_FLG_VALID); }
	}
	/// <summary>
	/// レコメンド適用条件アイテムリスト
	/// </summary>
	public RecommendApplyConditionItemInput[] ApplyConditionItems
	{
		get { return (RecommendApplyConditionItemInput[])this.DataSource["ApplyConditionItems"]; }
		set { this.DataSource["ApplyConditionItems"] = value; }
	}
	/// <summary>
	/// レコメンドアップセル対象アイテム
	/// </summary>
	public RecommendUpsellTargetItemInput UpsellTargetItem
	{
		get { return (RecommendUpsellTargetItemInput)this.DataSource["UpsellTargetItem"]; }
		set { this.DataSource["UpsellTargetItem"] = value; }
	}
	/// <summary>
	/// レコメンドアイテムリスト
	/// </summary>
	public RecommendItemInput[] Items
	{
		get { return (RecommendItemInput[])this.DataSource["Items"]; }
		set { this.DataSource["Items"] = value; }
	}
	/// <summary>エラーメッセージ</summary>
	public string ErrorMessages
	{
		get { return StringUtility.ToEmpty(this.DataSource["ErrorMessages"]); }
		set { this.DataSource["ErrorMessages"] = value; }
	}
	/// <summary>レコメンド適用条件アイテムエラーメッセージ</summary>
	public string ApplyConditionItemErrorMessages
	{
		get { return string.Join(string.Empty, this.ApplyConditionItems.Select(i => i.ErrorMessages)); }
	}
	/// <summary>レコメンドアップセル対象アイテムエラーメッセージ</summary>
	public string UpsellTargetItemErrorMessages
	{
		get
		{
			if (this.IsUpsell == false) return string.Empty;
			return this.UpsellTargetItem.ErrorMessages;
		}
	}
	/// <summary>レコメンドアイテムエラーメッセージ</summary>
	public string RecommendItemErrorMessages
	{
		get { return string.Join(string.Empty, this.Items.Select(i => i.ErrorMessages)); }
	}
	/// <summary>注文完了ページで表示？</summary>
	public bool IsDispOrderComplete
	{
		get { return (this.RecommendDisplayPage == Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_PAGE_ORDER_COMPLETE); }
	}
	/// <summary>Chat bot use flg</summary>
	public string ChatbotUseFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_CHATBOT_USE_FLG]; }
		set { this.DataSource[Constants.FIELD_RECOMMEND_CHATBOT_USE_FLG] = value; }
	}
	#endregion
}