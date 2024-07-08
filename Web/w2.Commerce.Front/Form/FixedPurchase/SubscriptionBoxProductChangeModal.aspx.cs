/*
=========================================================================================================
  Module      : 頒布会商品変更モーダル (SubscriptionBoxProductChangeModal.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using Extensions;
using System.Linq;
using System.Web.UI.WebControls;
using Input.FixedPurchase;
using w2.Domain.FixedPurchase;
using w2.Domain.SubscriptionBox;
using w2.Domain.UpdateHistory.Helper;

public partial class Form_FixedPurchase_SubscriptionBoxProductChangeModal : BasePage
{
	#region Meta properties
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } } // HTTPSアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } } // ログイン必須
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!this.IsPostBack)
		{
			var fixedPurchase = new FixedPurchaseService().Get(this.FixedPurchaseId);
			if ((fixedPurchase == null)
				|| string.IsNullOrWhiteSpace(this.FixedPurchaseId)
				|| (fixedPurchase.UserId != this.LoginUserId))
			{
				// 不正遷移やデータ不正はシステムエラー扱いにする。
				throw new Exception("頒布会 商品変更モーダルで定期購入情報の取得に失敗しました。定期台帳ID=" + this.FixedPurchaseId);
			}

			var items = new SubscriptionBoxService().GetSubscriptionItemsWithProductInfo(
				fixedPurchase.FixedPurchaseId,
				fixedPurchase.SubscriptionBoxCourseId);
			if ((items.Any() == false) || string.IsNullOrWhiteSpace(fixedPurchase.SubscriptionBoxCourseId))
			{
				throw new Exception("頒布会 商品変更モーダルで頒布会情報の取得に失敗しました。定期台帳ID=" + this.FixedPurchaseId);
			}

			this.FixedPurchaseNextShippingDate = fixedPurchase.NextShippingDate.Value;
			this.SubscriptionBoxOrderCount = fixedPurchase.SubscriptionBoxOrderCount;
			this.Course = new SubscriptionBoxService().GetByCourseId(fixedPurchase.SubscriptionBoxCourseId);
			this.Input = SubscriptionBoxNextShippingProductInput.Create(
				this.Course,
				items,
				this.PreserveSelections,
				this.FixedPurchaseNextShippingDate,
				this.SubscriptionBoxOrderCount,
				(this.Course.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE));
		}
		else
		{	
			this.Input = GetAndUpdateInput();
		}

		ValidateAndDisplayErrors();

		DataBind();
	}

	/// <summary>
	/// 入力値取得・更新
	/// </summary>
	/// <returns>入力</returns>
	private SubscriptionBoxNextShippingProductInput GetAndUpdateInput()
	{
		foreach (RepeaterItem item in rItems.Items)
		{
			var shopId = item.FindControlAs<HiddenField>("hfShopId").Value;
			var productId = item.FindControlAs<HiddenField>("hfProductId").Value;
			var variationId = item.FindControlAs<HiddenField>("hfVariationId").Value;
			var quantityString = item.FindControlAs<TextBox>("tbQuantity").Text;

			var inputItem = this.Input.Items[shopId, productId, variationId];
			inputItem.IsSelected = item.FindControlAs<CheckBox>("cbSelected").Checked;
			inputItem.Quantity = string.IsNullOrWhiteSpace(quantityString) ? 0 : int.Parse(quantityString);

			if (inputItem.IsSelected == false)
			{
				inputItem.Quantity = 0;
			}

			if (inputItem.IsSelected && inputItem.Quantity <= 0)
			{
				inputItem.Quantity = 1;
			}
		}

		return this.Input;
	}

	/// <summary>
	/// 変更するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSubmitChanges_OnClick(object sender, EventArgs e)
	{
		if (ValidateAndDisplayErrors())
		{
			return;
		}

		var models = this.Input.Items
			.Where(i => i.IsSelected)
			.Select(
				(item, index) => new FixedPurchaseItemModel
				{
					FixedPurchaseId = this.FixedPurchaseId,
					FixedPurchaseItemNo = index + 1,
					FixedPurchaseShippingNo = 1,
					ShopId = item.ShopId,
					ProductId = item.ProductId,
					VariationId = item.VariationId,
					ItemQuantity = item.Quantity,
					ItemQuantitySingle = item.Quantity,
					ProductOptionTexts = string.Empty,
				})
			.ToArray();

		new FixedPurchaseService().UpdateItems(
			models,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);

		// JS経由でFixedPurchaseDetail.aspx側でモーダルを閉じさせる
		System.Web.UI.ScriptManager.RegisterStartupScript(
			upProductList,
			upProductList.GetType(),
			"closeModal",
			"window.app.closeModal();",
			true);
	}

	/// <summary>
	/// 入力バリデーション・エラー表示
	/// </summary>
	/// <returns>エラーがあればTRUE</returns>
	public bool ValidateAndDisplayErrors()
	{
		// HACK: 破壊的再代入を繰り返していてよくない

		bool dmy;
		var errors = this.Input.IsMeetingCondition(
			this.Course,
			this.SubscriptionBoxOrderCount,
			this.FixedPurchaseNextShippingDate,
			out dmy);
		lErrors.Text = WebSanitizer.HtmlEncodeChangeToBr(string.Join(Environment.NewLine, errors));

		// 選択されていない商品を、仮に選択した場合にエラーになる場合は
		// 選択不可商品となる
		bool shouldActivateAllItems = false;
		foreach (var inputItem in this.Input.Items.Where(i => i.IsSelected == false))
		{
			inputItem.IsSelected = true;
			inputItem.Quantity = 1;

			bool violationMinimumConditions;
			var errorTmp = this.Input.IsMeetingCondition(
				this.Course,
				this.SubscriptionBoxOrderCount,
				this.FixedPurchaseNextShippingDate,
				out violationMinimumConditions);

			if (violationMinimumConditions) shouldActivateAllItems = true;
			inputItem.IsSelectable = (errorTmp.Any() == false);
			inputItem.IsSelected = false;
			inputItem.Quantity = 0;
		}

		if (shouldActivateAllItems)
		{
			foreach (var inputItem in this.Input.Items)
			{
				inputItem.IsSelectable = true;
			}
		}

		lbSubmitChanges.Enabled = (errors.Any() == false);
		return errors.Any();
	}

	/// <summary>
	/// 日付の文字列化
	/// </summary>
	/// <param name="dt">日時</param>
	/// <returns>文字列化された日時</returns>
	protected string DateTimeToString(DateTime? dt)
	{
		var result = dt.HasValue
			? DateTimeUtility.ToStringFromRegion(dt.Value, DateTimeUtility.FormatType.LongDate1LetterNoneServerTime)
			: "";
		return result;
	}

	/// <summary>定期購入ID</summary>
	protected string FixedPurchaseId
	{
		get { return this.Request[Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_ID] ?? ""; }
	}
	/// <summary>初期状態で選択状態を保持するか</summary>
	protected bool PreserveSelections
	{
		get { return string.IsNullOrWhiteSpace(this.Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_PRESERVE_SELECTION]) == false; }
	}
	/// <summary>頒布会注文回数</summary>
	protected int SubscriptionBoxOrderCount
	{
		get { return (int)this.ViewState["Form_FixedPurchase_SubscriptionBoxProductChangeModal__SubscriptionBoxOrderCount"]; }
		set { this.ViewState["Form_FixedPurchase_SubscriptionBoxProductChangeModal__SubscriptionBoxOrderCount"] = value; }
	}
	/// <summary>定期台帳 次回配送日</summary>
	protected DateTime FixedPurchaseNextShippingDate
	{
		get { return (DateTime)this.ViewState["Form_FixedPurchase_SubscriptionBoxProductChangeModal__FixedPurchaseNextShippingDate"]; }
		set { this.ViewState["Form_FixedPurchase_SubscriptionBoxProductChangeModal__FixedPurchaseNextShippingDate"] = value; }
	}
	/// <summary>入力値</summary>
	protected SubscriptionBoxNextShippingProductInput Input
	{
		get { return (SubscriptionBoxNextShippingProductInput)this.ViewState["Form_FixedPurchase_SubscriptionBoxProductChangeModal__Input"]; }
		set { this.ViewState["Form_FixedPurchase_SubscriptionBoxProductChangeModal__Input"] = value; }
	}
	/// <summary>コース設定</summary>
	protected SubscriptionBoxModel Course
	{
		get { return (SubscriptionBoxModel)this.ViewState["Form_FixedPurchase_SubscriptionBoxProductChangeModal__Course"]; }
		set { this.ViewState["Form_FixedPurchase_SubscriptionBoxProductChangeModal__Course"] = value; }
	}
}
