/*
=========================================================================================================
  Module      : 頒布会コース設定画面 (SubscriptionBoxRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Input.SubscriptionBox;
using w2.App.Common.RefreshFileManager;
using w2.App.Common.Web.WebCustomControl;
using w2.Common.Web;
using w2.Domain.Product;
using w2.Domain.ProductTaxCategory;
using w2.Domain.SubscriptionBox;

public partial class Form_SubscriptionBox_SubscriptionBoxRegister : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			InitializeComponents();

			this.Input = GetInputForInitialization();

			// 最終商品繰り返し設定が有効の場合、商品決定方法変更不可
			rblDeterminationMethod.Enabled = this.Input.IsIndefinitePeriod == false;

			rbRenewalType.SelectedValue = this.Input.IsAutoRenewal
				? Constants.FLG_SUBSCRIPTIONBOX_RENEWAL_TYPE_AUTO
				: this.Input.IsIndefinitePeriod
					? Constants.FLG_SUBSCRIPTIONBOX_RENEWAL_TYPE_INFINITE
					: Constants.FLG_SUBSCRIPTIONBOX_RENEWAL_TYPE_NONE;

			// ドロップダウン初期値設定
			SetDefaultValue();
		}
		else
		{
			// ポストバックの度にInputを逆バインド
			this.Input = GetInput();
		}
	}

	/// <summary>
	/// ページロード完了
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_LoadComplete(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(this.ErrorMessage) == false)
		{
			this.ErrorMessage = this.ErrorMessage.Trim();
		}
		DataBind();
	}

	#region Event handlers
	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SUBSCRIPTION_BOX_REGISTER)
			.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID, this.tbCourseId.Text)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_COPY_INSERT)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBackList_Click(object sender, EventArgs e)
	{
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SUBSCRIPTION_BOX_LIST);
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		new SubscriptionBoxService().Delete(this.CourseId);

		RefreshFileManagerProvider.GetInstance(RefreshFileType.SubscriptionBox).CreateUpdateRefreshFile();
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SUBSCRIPTION_BOX_LIST);
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		var errors = this.Input.Validate();
		if (new SubscriptionBoxService().CheckDuplicationCourseId(this.Input.CourseId))
		{
			errors += (WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_ID_EXISTS));
		}
		if (string.IsNullOrEmpty(errors) == false)
		{
			this.ErrorMessage = errors;
			this.MaintainScrollPositionOnPostBack = false;
			return;
		}

		new SubscriptionBoxService().Insert(this.Input.CreateModel());

		OnCompletionInsertOrUpdate();
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		var errors = this.Input.Validate();
		if (string.IsNullOrEmpty(errors) == false)
		{
			this.ErrorMessage = errors;
			this.MaintainScrollPositionOnPostBack = false;
			return;
		}

		new SubscriptionBoxService().Update(this.Input.CreateModel());

		OnCompletionInsertOrUpdate();
	}

	/// <summary>
	/// 期間・回数追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddDefault_Click(object sender, EventArgs e)
	{
		this.Input.DefaultItems.Add(
			new SubscriptionBoxDefaultItemInput
			{
				Count = this.Input.IsOrderDeterminationTypeNumberTime
					? SubscriptionBoxDefaultItemInput.GetNextCountNumber(this.Input.DefaultItems.ToArray())
					: "",
				TermSince = "",
				TermUntil = "",
				Items = new List<SubscriptionBoxDefaultSubItemInput>
				{
					new SubscriptionBoxDefaultSubItemInput
					{
						ShopId = "",
						ProductId = "",
						VariationId = "",
						ItemQuantity = "1",
						BranchNo = SubscriptionBoxDefaultItemInput.GetNextBranchNo(this.Input.DefaultItems.ToArray()),
					},
				}
			});
	}

	/// <summary>
	/// 商品決定方法変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblDeterminationMethod_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (this.Input.DefaultItems.Any())
		{
			if (this.Input.IsOrderDeterminationTypeNumberTime
			&& (this.Input.DefaultItems[0].Items[0].ShopId == this.LoginOperatorShopId)
			&& string.IsNullOrEmpty(this.Input.DefaultItems[0].Items[0].ProductId)
			&& string.IsNullOrEmpty(this.Input.DefaultItems[0].Items[0].VariationId))
			{
				this.Input.DefaultItems[0].Items[0].ShopId = string.Empty;
			}
			else if (this.Input.IsOrderDeterminationTypePeriod)
			{
				foreach (var item in this.Input.DefaultItems
					.SelectMany(defaultItem => defaultItem.Items
						.Where(item => string.IsNullOrEmpty(item.ProductId))))
				{
					item.ShopId = string.Empty;
				}
			}
		}
		this.Input.RenumberDefaultItems();
	}

	/// <summary>
	/// 選択可能商品リピーターアイテムコマンド
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rItems_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		switch (e.CommandName)
		{
			case "delete":
				var index = int.Parse(e.CommandArgument.ToString());
				var item = this.Input.Items[index];
				this.Input.Items.Remove(item);

				// デフォルト注文商品でその商品が選択されてればドロップダウンリセット
				foreach (var subItem in this.Input.DefaultItems
					.SelectMany(di => di.Items)
					.Where(i => item.ShopId == i.ShopId
						&& item.ProductId == i.ProductId
						&& item.VariationId == i.VariationId))
				{
					subItem.ShopId = "";
					subItem.ProductId = "";
					subItem.VariationId = "";
				}

				this.Input.RenumberItems();

				break;
		}
	}

	/// <summary>
	/// デフォルト注文商品（サブ）アイテム作成イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rDefaultSubItems_OnItemCreated(object sender, RepeaterItemEventArgs e)
	{
		var ddlDefaultItem = (DropDownList)e.Item.FindControl("ddlDefaultItem");
		// 親リピーターのループ数を取得する
		var repeaterItem = (RepeaterItem)((Repeater)sender).Parent;
		var displayCount = ((IDataItemContainer)repeaterItem).DisplayIndex;
		ddlDefaultItem.Items.AddRange(CreateDefaultItemDropDownDataSource(displayCount).Cast<ListItem>().ToArray());
	}

	/// <summary>
	/// 商品追加隠しボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSetSelectableProduct_OnClick(object sender, EventArgs e)
	{
		var productId = hfProductId.Value;
		var variationId = hfVariationId.Value;
		if (string.IsNullOrEmpty(variationId))
		{
			variationId = productId;
		}
		else
		{
			variationId = productId + variationId;
		}

		var product = new ProductService().Get(this.LoginOperatorShopId, productId);
		if (product == null) return;

		if (this.Input.Items.Any()
			&& (this.Input.Items.First().ShippingType != product.ShippingType))
		{
			this.ErrorMessage
				= WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_SHIPPING_TYPE_MUST_BE_SAME);
			return;
		}

		if (this.Input.Items.Any(item => item.VariationId == variationId))
		{
			this.ErrorMessage = WebMessages.GetMessages(
				WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_PRODUCT_DUPLICATE_PRODUCT_ADDED);
			return;
		}

		this.Input.Items.Add(
			new SubscriptionBoxItemInput
			{
				SubscriptionBoxCourseId = this.Input.CourseId,
				BranchNo = this.Input.Items.Any()
					? (this.Input.Items.Max(i => i.BranchNo) + 1).ToString()
					: "0",
				ShopId = this.LoginOperatorShopId,
				ProductId = productId,
				VariationId = variationId,
				SelectableSince = "",
				SelectableUntil = "",
				ProductName = hfProductName.Value,
				ShippingType = product.ShippingType,
				CampaignSince = "",
				CampaignUntil = "",
				CampaignPrice = "",
			});
	}

	/// <summary>
	/// デフォルト注文商品リピーターアイテムコマンド
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rDefaultItems_OnItemCommand(object source, RepeaterCommandEventArgs e)
	{
		switch (e.CommandName)
		{
			case "add_subscription_box_default":
				this.Input.DefaultItems[int.Parse((string)e.CommandArgument)].Items.Add(
					new SubscriptionBoxDefaultSubItemInput
					{
						BranchNo = SubscriptionBoxDefaultItemInput.GetNextBranchNo(this.Input.DefaultItems.ToArray()),
						ShopId = "",
						ProductId = "",
						VariationId = "",
						ItemQuantity = "1",
						NecessaryProductFlg = Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID
					});
				DataBind();
				break;
		}
	}

	/// <summary>
	/// デフォルト注文サブ商品リピーターアイテムコマンド
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rDefaultSubItems_OnItemCommand(object source, RepeaterCommandEventArgs e)
	{
		switch (e.CommandName)
		{
			case "delete":
				var branchNo = e.CommandArgument.ToString();
				var subItem = this.Input.DefaultItems.SelectMany(s => s.Items).First(i => (i.BranchNo == branchNo));
				var item = this.Input.DefaultItems.First(i => i.Items.Contains(subItem));

				item.Items.Remove(subItem);
				if (item.Items.Any() == false)
				{
					this.Input.DefaultItems.Remove(item);
				}

				this.Input.RenumberDefaultItems();
				DataBind();
				break;
		}
	}

	/// <summary>
	/// 選択可能商品リピーターアイテムバインド
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rItems_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		if (IsPostBack)
		{
			var ucSelectableDateRange = (DateTimePickerPeriodInputControl)e.Item.FindControl("ucSelectableDateRange");
			var inputItem = (SubscriptionBoxItemInput)e.Item.DataItem;

			ucSelectableDateRange.SetStartDate(inputItem.SelectableSinceDate);
			ucSelectableDateRange.SetEndDate(inputItem.SelectableUntilDate);
		}

		var campaignDatePeriod = (DateTimePickerPeriodInputControl)e.Item.FindControl("ucOrderScheduledCampaignDatePeriod");
		if (campaignDatePeriod == null) return;

		var subscriptionBoxItemInput = (SubscriptionBoxItemInput)(e.Item.DataItem);
		
		// キャンペーン期間をセット
		if (string.IsNullOrEmpty(subscriptionBoxItemInput.CampaignSince) == false)
		{
			campaignDatePeriod.SetStartDate(DateTime.Parse(subscriptionBoxItemInput.CampaignSince));
		}
		if (string.IsNullOrEmpty(subscriptionBoxItemInput.CampaignUntil) == false)
		{
			campaignDatePeriod.SetEndDate(DateTime.Parse(subscriptionBoxItemInput.CampaignUntil));
		}
	}

	/// <summary>
	/// デフォルト注文商品リピーターアイテムバインド
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rDefaultItems_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		if (!IsPostBack) return;

		var ucTermDateRange = (DateTimePickerPeriodInputControl)e.Item.FindControl("ucTermDateRange");
		var inputItem = (SubscriptionBoxDefaultItemInput)e.Item.DataItem;

		ucTermDateRange.SetStartDate(inputItem.TermSinceDate);
		ucTermDateRange.SetEndDate(inputItem.TermUntilDate);
	}
	#endregion

	#region Methods
	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 商品決定タイプ
		rblDeterminationMethod.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_SUBSCRIPTIONBOX,
				Constants.FIELD_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE));

		// 繰り返し設定
		rbRenewalType.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_SUBSCRIPTIONBOX,
				"renewal_type"));

		//税率カテゴリバインド
		var taxCategories = new ProductTaxCategoryService().GetAllTaxCategory();
		ddlTaxCategory.Items.AddRange(taxCategories
			.Select(taxCategory =>
				new ListItem(
					string.Format(
						"{0}:{1}(%)", 
						taxCategory.TaxCategoryName, 
						taxCategory.TaxRate),
					taxCategory.TaxCategoryId))
			.ToArray());

		switch (this.ActionStatus)
		{
			// 新規登録
			case Constants.ACTION_STATUS_INSERT:
				this.trRegister.Visible = true;
				this.trEdit.Visible = false;
				this.btnDeleteTop.Visible = this.btnDeleteBottom.Visible = false;
				this.btnInsertTop.Visible = this.btnInsertBottom.Visible = true;
				this.btnUpdateTop.Visible = this.btnUpdateBottom.Visible = false;
				this.btnCopyInsertTop.Visible = this.btnCopyInsertBottom.Visible = false;
				break;

			// 編集
			case Constants.ACTION_STATUS_UPDATE:
			case Constants.ACTION_STATUS_COMPLETE:
				this.trRegister.Visible = false;
				this.trEdit.Visible = true;
				this.btnDeleteTop.Visible = this.btnDeleteBottom.Visible = true;
				this.btnInsertTop.Visible = this.btnInsertBottom.Visible = false;
				this.btnUpdateTop.Visible = this.btnUpdateBottom.Visible = true;
				this.btnCopyInsertTop.Visible = this.btnCopyInsertBottom.Visible = true;
				break;

			// コピー新規登録
			case Constants.ACTION_STATUS_COPY_INSERT:
				this.trRegister.Visible = true;
				this.trEdit.Visible = false;
				this.btnDeleteTop.Visible = this.btnDeleteBottom.Visible = false;
				this.btnInsertTop.Visible = this.btnInsertBottom.Visible = true;
				this.btnUpdateTop.Visible = this.btnUpdateBottom.Visible = false;
				this.btnCopyInsertTop.Visible = this.btnCopyInsertBottom.Visible = false;
				break;

			// エラーページへ
			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}
	}

	/// <summary>
	/// 初期入力値を取得
	/// </summary>
	private SubscriptionBoxInput GetInputForInitialization()
	{
		if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
			|| (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
			|| (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE))
		{
			var course = new SubscriptionBoxService().GetByCourseId(this.CourseId);
			if (course == null)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			return new SubscriptionBoxInput(course);
		}

		return new SubscriptionBoxInput();
	}

	/// <summary>
	/// ドロップダウン初期値設定
	/// </summary>
	private void SetDefaultValue()
	{
		//税率カテゴリドロップダウン初期値を設定
		if (ddlTaxCategory.Items.FindByValue(this.Input.TaxCategoryId) != null)
		{
			ddlTaxCategory.SelectedValue = this.Input.TaxCategoryId;
		}
	}

	/// <summary>
	/// 商品検索ポップアップURLを取得
	/// </summary>
	/// <returns></returns>
	protected string GetProductSearchPopupUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN, Constants.KBN_PRODUCT_SEARCH_ORDERPRODUCT)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_VALID_FLG, Constants.FLG_PRODUCT_VALID_FLG_VALID)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG, Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_SEARCH_FLG_VALID)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE, this.Input.Items.Any() ? this.Input.Items.First().ShippingType : "")
			.CreateUrl();
		return url;
	}
	
	/// <summary>
	/// 登録、更新後処理
	/// </summary>
	private void OnCompletionInsertOrUpdate()
	{
		// リフレッシュファイル更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.SubscriptionBox).CreateUpdateRefreshFile();

		// リダイレクト
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SUBSCRIPTION_BOX_REGISTER)
			.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID, this.Input.CourseId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_COMPLETE)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 商品選択ドロップダウンのデータソース作成（データバインド用）
	/// </summary>
	/// /// <param name="displayCount">表示回数</param>
	/// <returns>データソース</returns>
	protected ListItemCollection CreateDefaultItemDropDownDataSource(int displayCount)
	{
		var collection = new ListItemCollection
		{
			new ListItem("", "//")
		};

		if ((displayCount != 0) && (this.Input.IsOrderDeterminationTypePeriod == false))
		{
			collection.Add(new ListItem("前回の商品を引き継ぐ", string.Format("{0}{1}", this.LoginOperatorShopId, "//")));
		}

		var items = this.Input.Items
			.Select(
				i => new ListItem(
					string.IsNullOrEmpty(i.ProductName)
							? "削除済み商品" 
							: i.ProductName,
							string.Format(
							"{0}/{1}/{2}",
							i.ShopId,
							i.ProductId,
							i.VariationId)))
				.ToArray();
		collection.AddRange(items);
		return collection;
	}

	/// <summary>
	/// 入力値を取得
	/// </summary>
	/// <returns>入力値</returns>
	private SubscriptionBoxInput GetInput()
	{
		var input = new SubscriptionBoxInput
		{
			CourseId = tbCourseId.Text.Trim(),
			ManagementName = tbManagementName.Text.Trim(),
			DisplayName = tbDisplayName.Text.Trim(),
			MaximumPurchaseQuantity = StringUtility.ToHankaku(tbMaximumPurchaseQuantity.Text.Trim()),
			MinimumPurchaseQuantity = StringUtility.ToHankaku(tbMinimumPurchaseQuantity.Text.Trim()),
			MinimumNumberOfProducts = StringUtility.ToHankaku(tbMinimumNumberOfProducts.Text.Trim()),
			MaximumNumberOfProducts = StringUtility.ToHankaku(tbMaximumNumberOfProducts.Text.Trim()),
			AreItemsChangeableByUser = cbAllowChangeProductOnFront.Checked,
			IsAutoRenewal = rbRenewalType.SelectedValue == Constants.FLG_SUBSCRIPTIONBOX_RENEWAL_TYPE_AUTO,
			IsValid = cbValidFlg.Checked,
			OrderItemDeterminationType = rblDeterminationMethod.SelectedValue,
			DateCreated = this.Input.DateCreated,
			DateChanged = this.Input.DateChanged,
			LastChanged = this.LoginOperatorName,
			IsFixedAmount = cbFixedAmountFlg.Checked,
			FixedAmount = StringUtility.ToHankaku(tbFixedAmount.Text.Trim()),
			TaxCategoryId = ddlTaxCategory.SelectedValue,
			DefaultItems = GetDefaultItemInput().ToList(),
			Items = GetItemInput().ToList(),
			DisplayPriority = StringUtility.ToHankaku(tbSubScriptionBoxPriority.Text.Trim()),
			MinimumAmount = StringUtility.ToHankaku(tbMinimumAmount.Text.Trim()),
			MaximumAmount = StringUtility.ToHankaku(tbMaximumAmount.Text.Trim()),
			IsIndefinitePeriod = rbRenewalType.SelectedValue == Constants.FLG_SUBSCRIPTIONBOX_RENEWAL_TYPE_INFINITE,
		};

		input.FirstSelectableFlg = input.CheckFirstSelectableFlg(cbFirstSelectableFlg.Checked)
			? Constants.FLG_SUBSCRIPTIONBOX_FIRST_SELECTABLE_PAGE_TRUE
			: Constants.FLG_SUBSCRIPTIONBOX_FIRST_SELECTABLE_PAGE_FALSE;

		// 期間指定では自動更新不可
		if (input.IsOrderDeterminationTypePeriod)
		{
			input.IsAutoRenewal = false;
		}

		// 無期限設定フラグが有効の場合、商品決定方法変更不可
		rblDeterminationMethod.Enabled = input.IsIndefinitePeriod == false;

		return input;
	}

	/// <summary>
	/// 選択可能商品の入力値を取得
	/// </summary>
	/// <returns>選択可能商品</returns>
	private SubscriptionBoxItemInput[] GetItemInput()
	{
		var result = new List<SubscriptionBoxItemInput>();
		var branchNo = 0;
		foreach (RepeaterItem repeaterItem in rItems.Items)
		{
			var ucSelectableDateRange = (DateTimePickerPeriodInputControl)repeaterItem.FindControl("ucSelectableDateRange");

			branchNo++;
			result.Add(
				new SubscriptionBoxItemInput
				{
					SubscriptionBoxCourseId = tbCourseId.Text.Trim(),
					BranchNo = branchNo.ToString(),
					ShopId = ((HiddenField)repeaterItem.FindControl("hfShopId")).Value,
					ProductId = ((HiddenField)repeaterItem.FindControl("hfProductId")).Value,
					VariationId = ((HiddenField)repeaterItem.FindControl("hfVariationId")).Value,
					ShippingType = ((HiddenField)repeaterItem.FindControl("hfShippingType")).Value,
					ProductName = ((HiddenField)repeaterItem.FindControl("hfProductName")).Value,
					SelectableSince = ucSelectableDateRange.StartDateString,
					SelectableUntil = ucSelectableDateRange.EndDateString,
					CampaignSince = ((DateTimePickerPeriodInputControl)repeaterItem.FindControl("ucOrderScheduledCampaignDatePeriod")).StartDateTimeString,
					CampaignUntil = ((DateTimePickerPeriodInputControl)repeaterItem.FindControl("ucOrderScheduledCampaignDatePeriod")).EndDateTimeString,
					CampaignPrice = ((TextBox)repeaterItem.FindControl("tbCampaignPrice")).Text.Trim(),
				});
		}

		return result.ToArray();
	}

	/// <summary>
	/// デフォルト注文商品の入力値を取得
	/// </summary>
	/// <returns></returns>
	private SubscriptionBoxDefaultItemInput[] GetDefaultItemInput()
	{
		var result = new List<SubscriptionBoxDefaultItemInput>();
		var branchNo = 0;
		foreach (RepeaterItem repeaterItem in rDefaultItems.Items)
		{
			var ucTermDateRange = (DateTimePickerPeriodInputControl)repeaterItem.FindControl("ucTermDateRange");

			var input = new SubscriptionBoxDefaultItemInput
			{
				SubscriptionBoxCourseId = tbCourseId.Text.Trim(),
				Count = ((HiddenField)repeaterItem.FindControl("hfCount")).Value,
				TermSince = ucTermDateRange.StartDateString,
				TermUntil = ucTermDateRange.EndDateString,
			};
			var count = branchNo + 1;
			var rSubItems = (Repeater)repeaterItem.FindControl("rDefaultSubItems");
			var cantakeOver = false;
			foreach (RepeaterItem repeaterSubItem in rSubItems.Items)
			{
				branchNo++;
				var ddlProducts = (DropDownList)repeaterSubItem.FindControl("ddlDefaultItem");
				var splittedValues = ddlProducts.SelectedValue.Split('/');
				if (ddlProducts.SelectedValue == "0//") cantakeOver = true;

				input.Items.Add(
					new SubscriptionBoxDefaultSubItemInput
					{
						BranchNo = branchNo.ToString(),
						ShopId = splittedValues.ElementAtOrDefault(0) ?? "",
						ProductId = splittedValues.ElementAtOrDefault(1) ?? "",
						VariationId = splittedValues.ElementAtOrDefault(2) ?? "",
						ItemQuantity = ((TextBox)repeaterSubItem.FindControl("tbItemQuantity")).Text.Trim(),
						NecessaryProductFlg = (((CheckBox)repeaterSubItem.FindControl("cbNecessaryProductFlg")).Checked)
							? Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID
							: Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID
					});
			}

			// 前回の商品を引き継ぐが選択されていた場合項目を減らす
			if (cantakeOver)
			{
				var list = new List<SubscriptionBoxDefaultSubItemInput>();
				list.Add( new SubscriptionBoxDefaultSubItemInput
				{
					BranchNo = count.ToString(),
					ShopId = this.LoginOperatorShopId,
					ProductId = "",
					VariationId = "",
					ItemQuantity = "1",
					NecessaryProductFlg = Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID
				});
				input.Items = list;
			}
			result.Add(input);
		}

		return result.ToArray();
	}

	/// <summary>
	/// 前回の商品を引き継ぐ選択時商品追加ボタン無効化判定
	/// </summary>
	/// <param name="selectedValue">選択値</param>
	/// <returns>判定結果</returns>
	protected bool CheckActivateAddProduct(string selectedValue)
	{
		var result = (selectedValue != string.Format("{0}{1}", this.LoginOperatorShopId, "//"));
		return result;
	}

	/// <summary>
	/// 頒布会カート投入URL取得
	/// </summary>
	/// <returns>頒布会カート投入URL</returns>
	protected string GetSubscriptionBoxAddCartUrl()
	{
		var url = new UrlCreator(
				Constants.PROTOCOL_HTTPS
					+ Constants.SITE_DOMAIN
					+ Constants.PATH_ROOT_FRONT_PC
					+ Constants.PAGE_FRONT_CART_LIST)
				.AddParam(Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_COURSE_ID, this.CourseId)
				.CreateUrl();
		return url;
	}

	/// <summary>
	/// 頒布会初回選択画面URL取得
	/// </summary>
	/// <returns>頒布会初回選択画面URL</returns>
	protected string GetFirstSelectionPageUrl()
	{
		var url = new UrlCreator(
			Constants.PROTOCOL_HTTPS
				+ Constants.SITE_DOMAIN
				+ Constants.PATH_ROOT_FRONT_PC
				+ Constants.PAGE_FRONT_SUBSCRIPTIONBOX_FIRSTSELECTABLE)
			.AddParam(Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_COURSE_ID, this.CourseId)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// 初回選択画面が有効時数量のテキストボックスが表示可能か
	/// </summary>
	/// <param name="itemIndex">アイテム番号</param>
	/// <returns>true：可能 false：不可</returns>

	protected bool CanDisplayItemQuantityWithFirstSelectableFlgValid(int itemIndex)
	{
		return (this.Input.IsUsingFirstSelectablePage == false) || (itemIndex > 0);
	}
	#endregion
	#region Properties
	/// <summary>コースID</summary>
	protected string CourseId
	{
		get { return Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID]; }
	}
	/// <summary>コースIDが編集可能か</summary>
	protected bool CanEditCourseId
	{
		get
		{
			return (this.ActionStatus == Constants.ACTION_STATUS_INSERT)
				|| (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT);
		}
	}
	/// <summary>入力値</summary>
	protected SubscriptionBoxInput Input
	{
		get { return (SubscriptionBoxInput)this.ViewState["Input"]; }
		set { this.ViewState["Input"] = value; }
	}
	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage { get; set; }
	/// <summary>最終更新者等を表示する必要があるか</summary>
	protected bool ShouldShowLastChanged
	{
		get
		{
			return (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
				|| (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE);
		}
	}
	/// <summary>カート投入URLを表示するか</summary>
	protected bool DisplayAddCartUrl
	{
		get
		{
			return (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
				|| (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE);
		}
	}
	/// <summary>頒布会初回選択画面URLを表示するか</summary>
	protected bool DisplayFirstSelectionPageUrl
	{
		get
		{
			return this.DisplayAddCartUrl && this.Input.IsUsingFirstSelectablePage;
		}
	}
	#endregion
}
