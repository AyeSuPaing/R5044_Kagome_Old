/*
=========================================================================================================
  Module      : 定期購入情報入力クラス (FixedPurchaseInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Text;
using w2.App.Common.DataCacheController;
using w2.App.Common.Input;
using w2.App.Common.Input.Order;
using w2.App.Common.Order;
using w2.Common.Helper.Attribute;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.SubscriptionBox;

/// <summary>
/// 定期購入情報入力クラス
/// </summary>
[Serializable]
public class FixedPurchaseInput : InputBase<FixedPurchaseModel>
{
	#region +列挙体
	/// <summary>クレジットカード選択区分</summary>
	public enum SelectCreditCardKbn
	{
		/// <summary>現在利用しているクレジットカードを利用する</summary>
		[EnumTextName("現在利用しているクレジットカードを利用する")]
		UseNow,
		/// <summary>新しいクレジットカードを利用する</summary>
		[EnumTextName("新しいクレジットカードを利用する")]
		New,
		/// <summary>登録済みのクレジットカードを利用する</summary>
		[EnumTextName("登録済みのクレジットカードを利用する")]
		Registered
	}
	#endregion

	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public FixedPurchaseInput()
	{
		this.SelectCreditCard = SelectCreditCardKbn.UseNow;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="container">表示用定期購入情報</param>
	public FixedPurchaseInput(FixedPurchaseContainer container)
		: this()
	{
		this.FixedPurchaseId = container.FixedPurchaseId;
		this.FixedPurchaseKbn = container.FixedPurchaseKbn;
		this.FixedPurchaseSetting1 = container.FixedPurchaseSetting1;
		this.FixedPurchaseStatus = container.FixedPurchaseStatus;
		this.PaymentStatus = container.PaymentStatus;
		this.LastOrderDate = (container.LastOrderDate != null) ? container.LastOrderDate.ToString() : null;
		this.OrderCount = container.OrderCount.ToString();
		this.UserId = container.UserId;
		this.ShopId = container.ShopId;
		this.OrderKbn = container.OrderKbn;
		this.OrderPaymentKbn = container.OrderPaymentKbn;
		this.FixedPurchaseDateBgn = container.FixedPurchaseDateBgn.ToString();
		this.ValidFlg = container.ValidFlg;
		this.DateCreated = container.DateCreated.ToString();
		this.DateChanged = container.DateChanged.ToString();
		this.LastChanged = container.LastChanged;
		this.CreditBranchNo = (container.CreditBranchNo != null) ? container.CreditBranchNo.ToString() : CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW;
		this.NextShippingDate = (container.NextShippingDate != null) ? container.NextShippingDate.ToString() : null;
		this.NextNextShippingDate = (container.NextNextShippingDate != null) ? container.NextNextShippingDate.ToString() : null;
		this.FixedPurchaseManagementMemo = container.FixedPurchaseManagementMemo;
		this.CardInstallmentsCode = container.CardInstallmentsCode;
		// 配送先リスト
		this.Shippings = container.Shippings.Select(s => new FixedPurchaseShippingInput(s)).ToArray();
		this.ShippingMemo = container.ShippingMemo;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override FixedPurchaseModel CreateModel()
	{
		int creditBranchNo;
		var model = new FixedPurchaseModel
		{
			FixedPurchaseId = this.FixedPurchaseId,
			FixedPurchaseKbn = this.FixedPurchaseKbn,
			FixedPurchaseSetting1 = this.FixedPurchaseSetting1,
			FixedPurchaseStatus = this.FixedPurchaseStatus,
			PaymentStatus = this.PaymentStatus,
			LastOrderDate = (this.LastOrderDate != null) ? DateTime.Parse(this.LastOrderDate) : (DateTime?)null,
			OrderCount = int.Parse(this.OrderCount),
			UserId = this.UserId,
			ShopId = this.ShopId,
			OrderKbn = this.OrderKbn,
			OrderPaymentKbn = this.OrderPaymentKbn,
			FixedPurchaseDateBgn = DateTime.Parse(this.FixedPurchaseDateBgn),
			ValidFlg = this.ValidFlg,
			DateCreated = DateTime.Parse(this.DateCreated),
			DateChanged = DateTime.Parse(this.DateChanged),
			LastChanged = this.LastChanged,
			CreditBranchNo = int.TryParse(this.CreditBranchNo, out creditBranchNo) ? creditBranchNo : (int?)null,
			NextShippingDate = (this.NextShippingDate != null) ? DateTime.Parse(this.NextShippingDate) : (DateTime?)null,
			NextNextShippingDate =
				(this.NextNextShippingDate != null) ? DateTime.Parse(this.NextNextShippingDate) : (DateTime?)null,
			FixedPurchaseManagementMemo = this.FixedPurchaseManagementMemo,
			CardInstallmentsCode = this.CardInstallmentsCode,
			ShippingMemo = this.ShippingMemo,
		};
		// 配送先リスト
		model.Shippings = this.Shippings.Select(s => s.CreateModel()).ToArray();

		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <param name="isUpdateShipping">Is Update Shipping</param>
	/// <param name="subscriptionBoxCourseId">Subscription Box Id</param>
	/// <returns>正常：true、エラー：false</returns>
	public bool Validate(bool isUpdateShipping, string subscriptionBoxCourseId = "")
	{
		// 定期購入情報
		var errorMessage = new StringBuilder();

		errorMessage.Append(Validator.Validate("FixedPurchaseModifyInput", this.DataSource));

		// 定期購入配送先情報
		if (isUpdateShipping) errorMessage.Append(this.Shippings[0].Validate(this.OrderPaymentKbn, true));

		// 定期購入商品情報
		var errorItem = new StringBuilder();
		var items = this.Shippings[0].Items;
		errorItem.Append(CheckSameItems(items));
		errorItem.Append(CheckFixedPurchaseFlg(items));
		errorItem.Append(CheckShippingType(items));
		errorItem.Append(CheckValidItem(items));
		var numberOfProducts = 0;
		foreach (var item in this.Shippings[0].Items)
		{
			errorItem.Append(item.Validate());
			numberOfProducts++;
		}
		errorMessage.Append(errorItem);

		// Validate subscription box
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && subscriptionBoxCourseId != string.Empty)
		{
			var isValidateQuantity = true;
			var subscriptionBoxService = new SubscriptionBoxService();
			var subscriptionBoxItem = subscriptionBoxService.GetByCourseId(subscriptionBoxCourseId);
			if (subscriptionBoxItem != null)
			{
				if ((subscriptionBoxItem.MinimumPurchaseQuantity == null) && (subscriptionBoxItem.MaximumPurchaseQuantity == null))
				{
					isValidateQuantity = false;
				}
				if (isValidateQuantity)
				{
					var totalProductQuantity = 0;
					foreach (var item in items)
					{
						if (subscriptionBoxItem.SelectableProducts.Any(product => (product.ProductId == item.ProductId)
							&& (product.VariationId == item.VariationId)
							&& (product.ShopId == item.ShopId)))
						{
							var itemQuantity = 0;
							int.TryParse(item.ItemQuantity, out itemQuantity);
							totalProductQuantity += itemQuantity;
						}
						else
						{
							var message = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SUBSCRIPTION_BOX_NOT_AVAILABLE_PRODUCT)
								.Replace("@@ 1 @@", item.ProductId)
								.Replace("@@ 2 @@", item.VariationId)
								.Replace("@@ 3 @@", subscriptionBoxItem.ManagementName);
							errorMessage.Append(message);
						}
					}

					var subscriptionBoxQuantityError = OrderPage.GetSubscriptionBoxQuantityError(
						totalProductQuantity,
						subscriptionBoxItem.MaximumPurchaseQuantity,
						subscriptionBoxItem.MinimumPurchaseQuantity,
						subscriptionBoxItem.ManagementName);
					if (string.IsNullOrEmpty(subscriptionBoxQuantityError) == false)
					{
						errorMessage.Append(subscriptionBoxQuantityError);
					}
				}

				var numberOfProductMessage = new OrderPage().GetSubscriptionBoxProductOfNumberError(
					subscriptionBoxItem.ManagementName,
					numberOfProducts,
					subscriptionBoxItem.MinimumNumberOfProducts,
					subscriptionBoxItem.MaximumNumberOfProducts);

				if (string.IsNullOrEmpty(numberOfProductMessage) == false)
				{
					errorMessage.Append(numberOfProductMessage);
				}
			}
		}

		this.ErrorMessage = errorMessage.ToString();
		return (this.ErrorMessage.Length == 0);
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>正常：true、エラー：false</returns>
	public bool Validate()
	{
		// 定期購入情報
		var errorMessage = new StringBuilder();
		var checkKbn = (this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
			? "FixedPurchaseAmazonModifyInput"
			: "FixedPurchaseModifyInput";

		errorMessage.Append(Validator.Validate(checkKbn, this.DataSource));

		// 定期購入配送先情報
		errorMessage.Append(this.Shippings[0].Validate(checkKbn));

		// 定期購入商品情報
		var errorItem = new StringBuilder();
		var items = this.Shippings[0].Items;
		errorItem.Append(CheckSameItems(items));
		errorItem.Append(CheckFixedPurchaseFlg(items));
		errorItem.Append(CheckShippingType(items));
		errorItem.Append(CheckValidItem(items));
		errorItem.Append(CheckSubscriptionBoxItem(items));
		foreach (var item in this.Shippings[0].Items)
		{
			errorItem.Append(item.Validate(checkKbn));
		}
		errorMessage.Append(errorItem);

		this.ErrorMessage = errorMessage.ToString();
		return (this.ErrorMessage.Length == 0);
	}

	/// <summary>
	/// 同一商品チェック
	/// </summary>
	/// <param name="items">商品情報</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckSameItems(FixedPurchaseItemInput[] items)
	{
		var error = new StringBuilder();
		var temp = new Hashtable();
		foreach (var item in items)
		{
			var key = item.ProductId + "***" + item.VariationId + "***" + item.ProductOptionTexts;
			if (temp.Contains(key))
			{
				error.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERITEM_DUPLICATION_ERROR).Replace("@@ 1 @@", item.ProductId).Replace("@@ 2 @@", item.VariationId));
				break;
			}
			temp.Add(key, "");
		}
		return error.ToString();
	}

	/// <summary>
	/// 定期購入フラグチェック
	/// </summary>
	/// <param name="items">商品情報</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckFixedPurchaseFlg(FixedPurchaseItemInput[] items)
	{
		var error = new StringBuilder();
		foreach (var item in items)
		{
			if (item.IsValidFixedPurchaseFlg() == false)
			{
				error.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_FIXED_PURCHASE_DISABLE).Replace("@@ 1 @@", item.CreateProductJointName()));
				break;
			}
		}
		return error.ToString();
	}

	/// <summary>
	/// 配送種別チェック
	/// </summary>
	/// <param name="items">商品情報</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckShippingType(FixedPurchaseItemInput[] items)
	{
		var error = new StringBuilder();
		var shippingType = "";
		foreach (var item in items)
		{
			if (shippingType == "")
			{
				shippingType = item.ShippingType;
				continue;
			}

			if (shippingType != item.ShippingType)
			{
				error.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_SHIPPING_KBN_DIFF));
				break;
			}
		}
		return error.ToString();
	}

	/// <summary>
	/// 商品有効フラグチェック
	/// </summary>
	/// <param name="items">商品情報</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckValidItem(FixedPurchaseItemInput[] items)
	{
		var error = new StringBuilder();
		foreach (var item in items)
		{
			var product = ProductCommon.GetProductInfoUnuseMemberRankPrice(item.ShopId, item.ProductId);
			if ((product.Count != 0) 
				&& ((string)product[0][Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID))
			{
				error.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID)
					.Replace("@@ 1 @@", item.CreateProductJointName()));
				break;
			}
		}
		return error.ToString();
	}

	/// <summary>
	/// 頒布会コース設定の商品合計金額チェック
	/// </summary>
	/// <param name="items">商品情報</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckSubscriptionBoxItem(FixedPurchaseItemInput[] items)
	{
		if (string.IsNullOrEmpty(this.SubscriptionBoxCourseId)) return string.Empty; 

		var selectedSubscriptionBox = DataCacheControllerFacade
			.GetSubscriptionBoxCacheController()
			.Get(this.SubscriptionBoxCourseId);
		decimal totalAmount = 0;
		foreach (var item in items)
		{
			// 頒布会キャンペーン期間かどうか
			var selectedSubscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
				x => (x.ProductId == item.ProductId) && (x.VariationId == item.VariationId));
			var isCampaignPeriod = OrderCommon.IsSubscriptionBoxCampaignPeriod(selectedSubscriptionBoxItem);
			totalAmount += (isCampaignPeriod
				? (decimal)selectedSubscriptionBoxItem.CampaignPrice
				: item.FixedPurchasePrice ?? (decimal)item.Price) * int.Parse(item.ItemQuantity);
		}

		return OrderCommon.GetSubscriptionBoxTotalAmountError(this.SubscriptionBoxCourseId, totalAmount);
	}
	#endregion

	#region プロパティ
	/// <summary>定期購入ID</summary>
	public string FixedPurchaseId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID] = value; }
	}
	/// <summary>定期購入区分</summary>
	public string FixedPurchaseKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN] = value; }
	}
	/// <summary>定期購入設定１</summary>
	public string FixedPurchaseSetting1
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1] = value; }
	}
	/// <summary>定期購入設定1_1_1: 月間隔</summary>
	public string FixedPurchaseSetting1_1_1
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_MONTH]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_MONTH] = value; }
	}
	/// <summary>定期購入設定1_1_2：日付</summary>
	public string FixedPurchaseSetting1_1_2
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE] = value; }
	}
	/// <summary>定期購入設定: 月間隔</summary>
	public string FixedPurchaseSetting_IntervalMonths
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS] = value; }
	}
	/// <summary>定期購入設定１_2_1：週</summary>
	public string FixedPurchaseSetting1_2_1
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH] = value; }
	}
	/// <summary>定期購入設定１_2_2：曜日</summary>
	public string FixedPurchaseSetting1_2_2
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK] = value; }
	}
	/// <summary>定期購入設定１_3：配送日間隔</summary>
	public string FixedPurchaseSetting1_3
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS] = value; }
	}
	/// <summary>定期購入設定１_4_1：週</summary>
	public string FixedPurchaseSetting1_4_1
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK] = value; }
	}
	/// <summary>定期購入設定１_4_2：曜日</summary>
	public string FixedPurchaseSetting1_4_2
	{
		get { return (string)this.DataSource[Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK]; }
		set { this.DataSource[Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK] = value; }
	}
	/// <summary>定期購入ステータス</summary>
	public string FixedPurchaseStatus
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS] = value; }
	}
	/// <summary>決済ステータス</summary>
	public string PaymentStatus
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_PAYMENT_STATUS]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_PAYMENT_STATUS] = value; }
	}
	/// <summary>最終購入日</summary>
	public string LastOrderDate
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE] = value; }
	}
	/// <summary>購入回数(注文基準)</summary>
	public string OrderCount
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_COUNT]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_COUNT] = value; }
	}
	/// <summary>購入回数(出荷基準)</summary>
	public string ShippedCount
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHIPPED_COUNT]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHIPPED_COUNT] = value; }
	}
	/// <summary>ユーザID</summary>
	public string UserId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_USER_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_USER_ID] = value; }
	}
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHOP_ID] = value; }
	}
	/// <summary>注文区分</summary>
	public string OrderKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_KBN]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_KBN] = value; }
	}
	/// <summary>支払区分</summary>
	public string OrderPaymentKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN] = value; }
	}
	/// <summary>定期購入開始日時</summary>
	public string FixedPurchaseDateBgn
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN] = value; }
	}
	/// <summary>有効フラグ</summary>
	public string ValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_VALID_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_LAST_CHANGED] = value; }
	}
	/// <summary>クレジットカード枝番</summary>
	public string CreditBranchNo
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_CREDIT_BRANCH_NO]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_CREDIT_BRANCH_NO] = value; }
	}
	/// <summary>次回配送日</summary>
	public string NextShippingDate
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE] = value; }
	}
	/// <summary>次々回配送日</summary>
	public string NextNextShippingDate
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE] = value; }
	}
	/// <summary>定期購入管理メモ</summary>
	public string FixedPurchaseManagementMemo
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO] = value; }
	}
	/// <summary>カード支払い回数コード</summary>
	public string CardInstallmentsCode
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_CARD_INSTALLMENTS_CODE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_CARD_INSTALLMENTS_CODE] = value; }
	}
	/// <summary>クレジットカード選択</summary>
	public SelectCreditCardKbn SelectCreditCard
	{
		get { return (SelectCreditCardKbn)this.DataSource["SelectCreditCard"]; }
		set { this.DataSource["SelectCreditCard"] = value; }
	}
	/// <summary>クレジットカード選択「現在利用しているクレジットカードを利用する」が選択されているか？</summary>
	public bool IsSelectCreditCardUseNow
	{
		get { return this.SelectCreditCard == SelectCreditCardKbn.UseNow; }
	}
	/// <summary>クレジットカード選択「新しいクレジットカードを利用する」が選択されているか？</summary>
	public bool IsSelectCreditCardNew
	{
		get { return this.SelectCreditCard == SelectCreditCardKbn.New; }
	}
	/// <summary>クレジットカード選択「登録済みのクレジットカードを利用する」が選択されているか？</summary>
	public bool IsSelectCreditCardRegistered
	{
		get { return this.SelectCreditCard == SelectCreditCardKbn.Registered; }
	}
	/// <summary>配送先リスト</summary>
	public FixedPurchaseShippingInput[] Shippings
	{
		get { return (FixedPurchaseShippingInput[])this.DataSource["Shippings"]; }
		set { this.DataSource["Shippings"] = value; }
	}
	/// <summary>クレジットカード情報</summary>
	public OrderCreditCardInput CreditCardInput
	{
		get { return (OrderCreditCardInput)this.DataSource["CreditCardInput"]; }
		set { this.DataSource["CreditCardInput"] = value; }
	}
	/// <summary>エラーメッセージ</summary>
	public string ErrorMessage
	{
		get { return StringUtility.ToEmpty(this.DataSource["ErrorMessage"]); }
		set { this.DataSource["ErrorMessage"] = value; }
	}
	/// <summary>配送メモ</summary>
	public string ShippingMemo
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO] = value; }
	}
	/// <summary>頒布会コースID</summary>
	public string SubscriptionBoxCourseId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_COURSE_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_COURSE_ID] = value; }
	}
	#endregion
}