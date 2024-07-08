/*
=========================================================================================================
  Module      : 受注情報出力コントローラ処理(BodyOrderConfirm.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Api;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Input.Order;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.DSKDeferred;
using w2.App.Common.Order.Payment.DSKDeferred.GetAuth;
using w2.App.Common.Order.Payment.DSKDeferred.GetInvoice;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.JACCS.ATODENE;
using w2.App.Common.Order.Payment.JACCS.ATODENE.GetInvoice;
using w2.App.Common.Order.Payment.Veritrans.ObjectElement;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.OrderExtend;
using w2.App.Common.Product;
using w2.App.Common.User;
using w2.Common.Helper;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.Atodene;
using w2.Domain.DeliveryCompany;
using w2.Domain.DeliveryCompany.Helper;
using w2.Domain.InvoiceDskDeferred;
using w2.Domain.InvoiceVeritrans;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.Point.Helper;
using w2.Domain.Score;
using w2.Domain.ShopShipping;
using w2.Domain.TwOrderInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserManagementLevel;

public partial class Form_Common_BodyOrderConfirm : BaseUserControl
{
	/// <summary>Session key: Is focus</summary>
	private const string SESSION_KEY_IS_FOCUS = "isFocus";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 処理区分チェック
			CheckActionStatus();

			// 各プロパティセット
			SetProperty();

			// コンポーネント初期化
			InitializeComponents();

			// コンポーネントに値セット
			SetValueToComponents();

			// Set Uniform Type
			SetVisibleForUniformOption();

			// データバインド
			DataBind();

			if ((Session[SESSION_KEY_IS_FOCUS] != null) && (bool)Session[SESSION_KEY_IS_FOCUS])
			{
				lbTwInvoiceECPayFocus.Focus();
				Session.Remove(SESSION_KEY_IS_FOCUS);
			}
		}

		// 項目メモ一覧取得
		this.FieldMemoSettingData = BasePage.GetFieldMemoSettingList(Constants.TABLE_ORDER);
	}

	/// <summary>
	/// ユーザ詳細へ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUserDetail_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateUserDetailUrl(this.OrderInput.UserId));
	}

	/// <summary>
	/// リピーター：配送伝票番号更新ボタンクリック
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rShippingList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		if (e.CommandName == "update_shipping_check_no")
		{
			var args = e.CommandArgument.ToString().Split(',');
			int index = int.Parse(args[0]);			// リピータのItemIndex
			string orderShippingNo = args[1];		// 配送先枝番
			var shippingCheckNoOld = args[2];		// 元配送伝票番号
			var shippingCheckNoNew = ((TextBox)rShippingList.Items[index].FindControl("tbShippingCheckNo")).Text;
			var execShippingCheckNoApi = OrderCommon.CanShipmentEntry(this.OrderInput.OrderPaymentKbn)
				&& ((CheckBox)rShippingList.Items[index].FindControl("cbExecExternalShipmentEntry")).Checked;

			// 外部連携：出荷情報登録
			if (execShippingCheckNoApi)
			{
				var deliveryCompany = this.DeliveryCompanyList.First(i => i.DeliveryCompanyId == this.OrderInput.Shippings[0].DeliveryCompanyId);
				var errorMessage = OrderCommon.ShipmentEntry(
					this.OrderInput.OrderId,
					this.OrderInput.PaymentOrderId,
					this.OrderInput.OrderPaymentKbn,
					shippingCheckNoNew,
					shippingCheckNoOld,
					this.OrderInput.CardTranId,
					DeliveryCompanyUtil.GetDeliveryCompanyType(deliveryCompany.DeliveryCompanyId, this.OrderInput.OrderPaymentKbn),
					UpdateHistoryAction.DoNotInsert);
				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				// 決済連携メモ追記
				new OrderService().AddPaymentMemo(
					this.OrderInput.OrderId,
					OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
						string.IsNullOrEmpty(this.OrderInput.PaymentOrderId) ? this.OrderInput.OrderId : this.OrderInput.PaymentOrderId,
						this.OrderInput.OrderPaymentKbn,
						this.OrderInput.CardTranId,
						Constants.ACTION_NAME_SHIPPING_REPORT,
						this.OrderInput.LastBilledAmount.ToPriceDecimal()),
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					null);
			}

			var orderHistory = new OrderHistory
			{
				OrderId = this.OrderInput.OrderId,
				Action = OrderHistory.ActionType.EcOrderConfirm,
				OpearatorName = this.LoginOperatorName,
				UpdateAction = new Hashtable
				{
					{Constants.TABLE_ORDERSHIPPING, new List<string>() {Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO}}
				},
			};

			// Begin write history
			orderHistory.HistoryBegin();

			// 更新（更新履歴も落とす）
			new OrderService().UpdateOrderShippingCheckNo(
				this.OrderInput.OrderId,
				int.Parse(orderShippingNo),
				shippingCheckNoNew,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

			// Write history complete
			orderHistory.HistoryComplete();

			// 注文詳細へ
			Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
		}
		else if (e.CommandName == "GetInvoice")
		{
			if (Constants.PAYMENT_CVS_DEF_KBN == w2.App.Common.Constants.PaymentCvsDef.Atodene)
			{
				var adp = new AtodeneGetInvoiceModelAdapter(this.OrderInput.CreateModel());
				var res = adp.Execute();
				if (res.Result != AtodeneConst.RESULT_OK)
				{
					var msg = WebMessages
							.GetMessages(WebMessages.ERRMSG_MANAGER_GET_INVOICE_ERROR)
							.Replace("@@ 1 @@", string.Join("\r\n", res.Errors.Error.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)).ToArray()));
					Session[Constants.SESSION_KEY_ERROR_MSG] = msg;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
				// 印字データの登録
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();
					res.InsertInvoice(this.OrderInput.OrderId, accessor);
					accessor.CommitTransaction();
				}
			}
			else if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk)
			{
				var adapter = new DskDeferredGetInvoiceAdapter(this.OrderInput.CreateModel());
				var response = adapter.Execute();
				if (response.IsResultOk == false)
				{
					var message = WebMessages
						.GetMessages(WebMessages.ERRMSG_MANAGER_GET_INVOICE_ERROR)
						.Replace("@@ 1 @@", string.Join("\r\n", response.Errors.Error.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)).ToArray()));
					Session[Constants.SESSION_KEY_ERROR_MSG] = message;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
				// 印字データの登録
				adapter.InsertInvoice(this.OrderInput.OrderId, response);
				this.InvoiceData = GetDskDeferredInvoice();
				((Button)e.Item.FindControl("btnGetInvoice")).Enabled = false;
			}
			else if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score)
			{
				var errorMessage = new ProcessAfterUpdateOrderStatus().UpdatedInvoiceByOrderStatus(
					orderInput: this.OrderInput,
					updateStatus: w2.App.Common.Constants.StatusType.Order,
					orderStatus: Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED,
					accessor: null,
					lastChanged: this.LoginOperatorName);

				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				// 注文詳細へ
				Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
			}
			else if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Veritrans)
			{
				var response = new PaymentVeritransCvsDef().GetInvoiceData(this.OrderInput.PaymentOrderId);

				if (response.Mstatus != VeriTransConst.RESULT_STATUS_OK)
				{
					var errorMessge = response.Errors != null
						? string.Join("\r\n", response.Errors.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)).ToArray())
						: LogCreator.CreateErrorMessage(response.VResultCode, response.MerrMsg);
					var message = WebMessages
						.GetMessages(WebMessages.ERRMSG_MANAGER_GET_INVOICE_ERROR)
						.Replace("@@ 1 @@", errorMessge);
					Session[Constants.SESSION_KEY_ERROR_MSG] = message;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				// 印字データの登録
				var invoiceVeritransModel = new InvoiceElement(response).CreateModel(this.OrderInput.OrderId, this.OrderInput.LastChanged);
				DomainFacade.Instance.InvoiceVeritransService.InsertUpdate(invoiceVeritransModel);
				this.InvoiceData = GetVeritransInvoice();
			}
		}
	}

	/// <summary>
	/// Atodene後払い請求書取得
	/// </summary>
	/// <returns>後払い請求書</returns>
	protected InvoiceAtodeneModel GetAtodeneInvoice()
	{
		var model = new AtodeneService().Get(this.OrderInput.OrderId);
		return model;
	}

	/// <summary>
	/// DSK後払い請求書取得
	/// </summary>
	/// <returns>後払い請求書</returns>
	protected InvoiceDskDeferredModel GetDskDeferredInvoice()
	{
		var model = new InvoiceDskDeferredService().Get(this.OrderInput.OrderId);
		return model;
	}

	/// <summary>
	/// スコア後払い請求書取得
	/// </summary>
	/// <returns>スコア後払い請求書</returns>
	private InvoiceScoreModel GetScoreInvoice()
	{
		var model = new InvoiceScoreService().Get(this.OrderInput.OrderId);
		return model;
	}

	/// <summary>
	/// ベリトランス後払い請求書取得
	/// </summary>
	/// <returns>後払い請求書</returns>
	protected InvoiceVeritransModel GetVeritransInvoice()
	{
		var invoice = DomainFacade.Instance.InvoiceVeritransService.Get(this.OrderInput.OrderId);
		return invoice;
	}

	/// <summary>
	/// 管理メモ更新
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateManagementMemo_Click(object sender, EventArgs e)
	{
		// Get data input action
		var actionInput = new Hashtable
		{
			{ Constants.TABLE_ORDER, new List<string>() { Constants.FIELD_ORDER_MANAGEMENT_MEMO } }
		};
		var orderHistory = new OrderHistory
		{
			OrderId = this.OrderInput.OrderId,
			Action = OrderHistory.ActionType.EcOrderConfirm,
			OpearatorName = this.LoginOperatorName,
			UpdateAction = actionInput,
		};

		// Begin write history
		orderHistory.HistoryBegin();

		// 更新
		new OrderService().UpdateManagementMemo(
			this.OrderInput.OrderId,
			StringUtility.RemoveUnavailableControlCode(tbManagementMemo.Text),
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);

		// Write history complete
		orderHistory.HistoryComplete();

		// 注文詳細画面へ
		Response.Redirect(Request.Url.AbsoluteUri);
	}

	/// <summary>
	/// 配送メモ更新
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateShippingMemo_Click(object sender, EventArgs e)
	{
		var orderHistory = new OrderHistory
		{
			OrderId = this.OrderInput.OrderId,
			Action = OrderHistory.ActionType.EcOrderConfirm,
			OpearatorName = this.LoginOperatorName,
			UpdateAction = new Hashtable
			{
				{ Constants.TABLE_ORDER, new List<string>() { Constants.FIELD_ORDER_SHIPPING_MEMO } }
			},
		};

		// 更新前の履歴設定
		orderHistory.HistoryBegin();

		// 更新
		new OrderService().UpdateShippingMemo(
			this.OrderInput.OrderId,
			StringUtility.RemoveUnavailableControlCode(tbShippingMemo.Text),
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);

		// 更新後の履歴設定
		orderHistory.HistoryComplete();

		// 注文詳細画面へ
		Response.Redirect(Request.Url.AbsoluteUri);
	}

	/// <summary>
	/// ユーザー情報更新
	/// ※ユーザメモ＆ユーザー管理レベルID更新
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateUserInfo_Click(object sender, EventArgs e)
	{
		// ユーザメモ＆ユーザー管理レベルID更新
		new UserService().UpdateUserMemoAndUserManagementLevelId(
			this.OrderInput.UserId,
			StringUtility.RemoveUnavailableControlCode(tbUserMemo.Text),
			ddlUserManagementLevel.SelectedValue,
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);

		// 注文詳細画面へ
		Response.Redirect(Request.Url.AbsoluteUri);
	}

	/// <summary>
	/// 返品商品在庫更新
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReturnStock_Click(object sender, EventArgs e)
	{
		var noItemChecked = true;
		using (var accessor = new SqlAccessor())
		{
			try
			{
				// トランザクション開始
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// Get data input action
				var actionInput = new Hashtable()
									{
					{ Constants.TABLE_ORDERITEM,
						new List<string>(){
										Constants.FIELD_ORDERITEM_ORDER_ID,
										Constants.FIELD_ORDERITEM_ORDER_ITEM_NO,
										Constants.FIELD_ORDERITEM_STOCK_RETURNED_FLG
						}
					}
				};
				var orderHistory = new OrderHistory
				{
					OrderId = this.OrderInput.OrderId,
					Action = OrderHistory.ActionType.EcOrderConfirm,
					OpearatorName = this.LoginOperatorName,
					Accessor = accessor,
					UpdateAction = actionInput,
				};

				// Begin write history
				orderHistory.HistoryBegin();

				var itemList = (Repeater)(((Button)sender).Parent.FindControl("rItemList"));
				foreach (RepeaterItem item in itemList.Items)
				{
					var orderShippingNo = ((HiddenField)item.FindControl("hfOrderShippingNo")).Value;
					var orderItemNo = ((HiddenField)item.FindControl("hfOrderItemNo")).Value;

					var orderItems = this.OrderInput.Shippings.Where(s => s.OrderShippingNo == orderShippingNo).SelectMany(s => s.Items);
					var orderItem = orderItems.First(i => i.OrderItemNo == orderItemNo);

					var checkReturn = orderItem.IsProductSet ? (CheckBox)item.FindControl("cbItemSet") : (CheckBox)item.FindControl("cbItem");
					if ((checkReturn.Enabled) && (checkReturn.Checked) && (checkReturn.Visible))
					{
						noItemChecked = false;
						// 商品の在庫を更新します。
						using (var statement = new SqlStatement("ProductStock", "UpdateProductStockForOrder"))
						{
							var input = new Hashtable
							{
								{ Constants.FIELD_PRODUCTSTOCK_SHOP_ID, orderItem.ShopId },
								{ Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, orderItem.ProductId },
								{ Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, orderItem.VariationId },
								{ Constants.FIELD_ORDERITEM_ITEM_QUANTITY, int.Parse(orderItem.ItemQuantity) * (-1) },
								{ Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, this.LoginOperatorName }
							};
							statement.ExecStatement(accessor, input);
						}

						// 商品の在庫履歴を挿入します。
						using (var statement = new SqlStatement("ProductStock", "InsertProductStockHistoryForOrder"))
						{
							var input = new Hashtable
						{
								{ Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, this.OrderInput.OrderId },
								{ Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, orderItem.ShopId },
								{ Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, orderItem.ProductId },
								{ Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, orderItem.VariationId },
								{ Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS, Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_RETURNED },
								{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, int.Parse(orderItem.ItemQuantity) * (-1) },
								{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK, 0 },
								{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED, 0 },
								{ Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_STOCK, DBNull.Value },
								{ Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK, DBNull.Value },
								{ Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_RESERVED, DBNull.Value },
								{ Constants.FIELD_PRODUCTSTOCKHISTORY_LAST_CHANGED, this.LoginOperatorName }
							};
							statement.ExecStatement(accessor, input);
						}

						// 在庫戻し済みフラグを更新します。
						using (var statement = new SqlStatement("Order", "UpdateItemStockReturnedFlag"))
						{
							var input = new Hashtable
						{
								{ Constants.FIELD_ORDERITEM_ORDER_ID, this.OrderInput.OrderId },
								{ Constants.FIELD_ORDERITEM_ORDER_ITEM_NO, ((HiddenField)item.FindControl("hfOrderItemNo")).Value },
								{ Constants.FIELD_ORDERITEM_STOCK_RETURNED_FLG, Constants.FLG_ORDERITEM_STOCK_RETURNED_FLG_RETURNED }
							};
							statement.ExecStatement(accessor, input);
						}
						// 最終更新日時更新
						new OrderService().UpdateOrderDateChangedByOrderId(this.OrderInput.OrderId, accessor);
					}
				}

				// 更新履歴登録
				new UpdateHistoryService().InsertForOrder(this.OrderInput.OrderId, this.LoginOperatorName, accessor);

				// Write history complete
				orderHistory.HistoryComplete();

				// トランザクションコミット
				accessor.CommitTransaction();
			}
			catch
			{
				// トランザクションロールバック
				accessor.RollbackTransaction();
				throw;
			}
		}

		if (noItemChecked)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERITEM_STOCK_RETURNED_NO_CHECK);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		else
		{
			// 注文詳細へ
			Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
		}
	}

	/// <summary>
	/// 領収書希望がクリックされた場合
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblReceiptFlg_SelectedIndexChanged(object sender, EventArgs e)
	{
		tbReceiptAddress.Enabled = tbReceiptProviso.Enabled
			= rblReceiptOutputFlg.Enabled = (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON);
		if (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_OFF) tbReceiptAddress.Text = tbReceiptProviso.Text = string.Empty;
	}

	/// <summary>
	/// 与信取得ボタンをクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGetAuth_Click(object sender, EventArgs e)
	{
		switch (Constants.PAYMENT_CVS_DEF_KBN)
		{
			case Constants.PaymentCvsDef.Dsk:
				ExecDskAuthResult();
				break;

			case Constants.PaymentCvsDef.Veritrans:
				ExecVeritransAuthResult();
				break;
		}
		Response.Redirect(Request.Url.AbsoluteUri);
	}

	#region メソッド
	/// <summary>
	/// 処理区分チェック
	/// </summary>
	private void CheckActionStatus()
	{
		// アクション区分チェック
		// 詳細表示でない場合はエラー
		if (this.ActionStatus != Constants.ACTION_STATUS_DETAIL)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// 各プロパティセット
	/// </summary>
	private void SetProperty()
	{
		// 注文情報セット
		SetOrder();

		// 配送種別情報セット
		this.ShopShipping = new ShopShippingService().Get(this.LoginOperatorShopId, this.OrderInput.ShippingId);

		// ユーザークレジットカード情報セット
		SetUserCreditCardInfo();
	}

	/// <summary>
	/// 注文情報セット
	/// </summary>
	private void SetOrder()
	{
		// ページ側からセットされている場合はスルー
		if (this.OrderInput == null)
		{
			// 注文情報取得
			var order = new OrderService().GetOrderInfoByOrderId(this.RequestOrderId);
			// 注文情報が存在しない？
			if (order == null)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// 外部決済連携ログがあった場合、末尾の改行文字列を削除
			if (string.IsNullOrEmpty(order.ExternalPaymentCooperationLog) == false)
			{
				order.ExternalPaymentCooperationLog = order.ExternalPaymentCooperationLog.TrimEnd('\r', '\n');
			}

			// 注文情報セット
			// ※呼び出し元からセットされる場合もある
			this.OrderInput = new OrderInput(order);

			if (OrderCommon.DisplayTwInvoiceInfo())
			{
				// Get TwOrderInvoice
				var twOrderInvoice = new TwOrderInvoiceService().GetOrderInvoice(
					order.OrderId,
					order.Shippings[0].OrderShippingNo);

				if (this.TwOrderInvoiceInput == null && twOrderInvoice != null)
				{
					// Set Taiwan Order Invoice
					this.TwOrderInvoiceInput = new TwOrderInvoiceInput(twOrderInvoice);
				}
			}
		}

		// 返品交換注文の場合、元注文情報をセット
		if (this.OrderInput.IsNotReturnExchangeOrder == false)
		{
			this.OriginalOrder = new Order(new OrderService().GetOrderInDataView(this.OrderInput.OrderIdOrg));
		}
	}

	/// <summary>
	/// ユーザークレジットカード情報セット
	/// </summary>
	private void SetUserCreditCardInfo()
	{
		if (((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				|| (this.OrderInput.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID))
			&& (string.IsNullOrEmpty(this.OrderInput.CreditBranchNo) == false))
		{
			this.UserCreditCardInfo = UserCreditCard.Get(this.OrderInput.UserId, int.Parse(this.OrderInput.CreditBranchNo));
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// ユーザー管理レベルドロップダウン作成
		var models = new UserManagementLevelService().GetAllList();
		ddlUserManagementLevel.Items.AddRange(
			models.Select(m => new ListItem(m.UserManagementLevelName, m.UserManagementLevelId)).ToArray());

		// 督促情報
		if (Constants.DEMAND_OPTION_ENABLE && this.IsNotDisplayDemandStatus)
		{
			trDemandStatus.Visible = true;
			trDemandDay.Visible = true;
		}

		// 返金メモ
		if (this.RepaymentBank == null)
		{
			var emptyRepaymentBank = new Dictionary<string, string>
			{
				{ Constants.CONST_ORDER_REPAYMENT_BANK_CODE, string.Empty },
				{ Constants.CONST_ORDER_REPAYMENT_BANK_NAME, string.Empty },
				{ Constants.CONST_ORDER_REPAYMENT_BANK_BRANCH, string.Empty },
				{ Constants.CONST_ORDER_REPAYMENT_BANK_ACCOUNT_NO, string.Empty },
				{ Constants.CONST_ORDER_REPAYMENT_BANK_ACCOUNT_NAME, string.Empty },
			};
			// nullの場合、Keyが無くエラーが出てしまうため、Valueが空のDictionaryをセットする
			this.RepaymentBank = emptyRepaymentBank;
		}
		else
		{
			trRepaymentMemo.Visible = false;
			tbodyRepaymentBank.Visible = true;
		}
	}

	/// <summary>
	/// コンポーネントに値セット
	/// </summary>
	private void SetValueToComponents()
	{
		// 拡張ステータス＆更新日情報
		var orderExtendStatusSettingList = OrderPage.GetOrderExtendStatusSettingList(this.LoginOperatorShopId);
		rOrderExtendStatusList.DataSource = orderExtendStatusSettingList;
		rOrderExtendStatusDates.DataSource = orderExtendStatusSettingList;

		// ユーザー情報取得
		var user = new UserService().Get(this.OrderInput.UserId);
		if (user != null)
		{
			ListItem li = ddlUserManagementLevel.Items.FindByValue(user.UserManagementLevelId);
			if (li != null) li.Selected = true;
			tbUserMemo.Text = user.UserMemo;
		}

		// 利用ポイント詳細取得
		this.OrderUsePointDetail = new PointService().GetUserPointHistoriesNotRestoredByOrderId(
			this.OrderInput.UserId,
			(string.IsNullOrEmpty(this.OrderInput.OrderIdOrg) == false)
				? this.OrderInput.OrderIdOrg
				: this.OrderInput.OrderId);

		// ユーザポイント情報取得
		if (UserService.IsUser(user.UserKbn))
		{
			// ユーザポイント情報取得（本・仮ポイント）
			var userPoint = new PointService().GetUserPointByOrderId(this.OrderInput.UserId, this.OrderInput.OrderId);
			var userPointTempList = new UserPointList(userPoint).UserPointTemp;
			this.PointTemporary = decimal.ToInt32(userPointTempList.Select(x => x.Point).Sum());

			// 表示初期化
			trOrderPointAdd.Visible = (StringUtility.ToNumeric(decimal.Parse(this.OrderInput.OrderPointAdd) - this.PointTemporary) != "0");
			rOrderPointAddTemp.Visible = (userPointTempList.Length != 0);
			rOrderPointAddTemp.DataSource = userPointTempList;
		}

		if (this.OrderInput.OrderIdOrg != "")
		{
			// ポイント表示、クーポン情報非表示
			divPointUse.Visible = true;
			divCoupon.Visible = false;
		}

		// 返品交換情報非表示
		divReturnExchange.Visible
			= divReturnExchangeDate.Visible
			= (this.OrderInput.OrderIdOrg != "");

		// 領収書情報
		if (Constants.RECEIPT_OPTION_ENABLED)
		{
			// 領収書希望フラグ
			rblReceiptFlg.Items.AddRange(
				ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RECEIPT_FLG));
			rblReceiptFlg.SelectedValue = this.OrderInput.ReceiptFlg;

			// 領収書出力フラグ
			rblReceiptOutputFlg.Items.AddRange(
				ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RECEIPT_OUTPUT_FLG));
			rblReceiptOutputFlg.SelectedValue = this.OrderInput.ReceiptOutputFlg;

			// 領収書出力リンクの制御：希望なしの場合、リンクを無効にする
			lbReceiptExport.Enabled = (this.OrderInput.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON);
		}

		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			var model = this.OrderInput.CreateModel();
			var orderExtend = OrderExtendCommon.CreateOrderExtendForManager(model);
			var input = new OrderExtendInput(orderExtend);
			rOrderExtendInput.DataSource = input.OrderExtendItems;
			rOrderExtendInput.DataBind();
		}

		if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
		{
			switch (Constants.PAYMENT_CVS_DEF_KBN)
			{
				case Constants.PaymentCvsDef.Dsk:
					this.InvoiceData = GetDskDeferredInvoice();
					break;

				case Constants.PaymentCvsDef.Score:
					this.InvoiceData = GetScoreInvoice();
					break;

				case Constants.PaymentCvsDef.Veritrans:
					this.InvoiceData = GetVeritransInvoice();
					break;
			}
		}
	}

	/// <summary>
	/// ユーザ詳細URL作成
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <returns>ユーザ詳細URL</returns>
	protected string CreateUserDetailUrl(string userId)
	{
		var page =
			(Request.FilePath.ToLower().IndexOf(Constants.PAGE_MANAGER_ORDER_CONFIRM.ToLower()) != -1)
			? Constants.PAGE_MANAGER_USER_CONFIRM
			: Constants.PAGE_MANAGER_USER_CONFIRM_POPUP;
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + page);
		urlCreator
			.AddParam(Constants.REQUEST_KEY_USER_ID, userId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP);

		var url = urlCreator.CreateUrl();
		return url;
	}

	/// <summary>
	/// 受注詳細URL作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateOrderDetailUrl(string orderId)
	{
		var url = OrderPage.CreateOrderDetailUrl(
			orderId,
			(this.IsPopUp),
			((string)Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_RELOAD_PARENT_WINDOW] != Constants.KBN_RELOAD_PARENT_WINDOW_OFF),
			(string)Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_POPUP_PARENT_NAME]);
		return url;
	}

	/// <summary>
	/// 配送希望日表示文言取得
	/// </summary>
	/// <param name="shippingDate">配送希望日</param>
	/// <returns>配送希望日表示文言</returns>
	protected string GetShippingDateText(string shippingDate)
	{
		var orderShippingDate = DateTimeUtility.ToStringForManager(
			shippingDate,
			DateTimeUtility.FormatType.LongDateWeekOfDay2Letter,
			ReplaceTag("@@DispText.shipping_date_list.none@@"));
		if (Constants.MALLCOOPERATION_OPTION_ENABLED
			&& (string.IsNullOrEmpty(shippingDate) == false)
			&& shippingDate.StartsWith("1900"))
		{
			//「指定あり（メモ欄参照）」
			return ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_BODY_ORDER_CONFIRM,
				Constants.VALUETEXT_PARAM_SHIPPING_DATE_TEXT,
				Constants.VALUETEXT_PARAM_DESIGNATION);
		}
		else
		{
			return orderShippingDate;
		}
	}

	/// <summary>
	/// 配送希望時間帯表示文言取得
	/// </summary>
	/// <param name="deliveryCompanyId">配送会社ID</param>
	/// <param name="shippingTimeId">配送希望時間帯</param>
	/// <returns>配送希望時間帯表示文言</returns>
	protected string GetShippingTimeText(string shippingCmpanyId, string shippingTimeId)
	{
		var deliveryCompany = this.DeliveryCompanyList.FirstOrDefault(i => i.DeliveryCompanyId == shippingCmpanyId);
		var shippingTimeText = string.Empty;
		if (deliveryCompany != null)
		{
			shippingTimeText = deliveryCompany.GetShippingTimeMessage(shippingTimeId);
		}
		return (string.IsNullOrEmpty(shippingTimeText) == false) ? shippingTimeText : ReplaceTag("@@DispText.shipping_time_list.none@@");
	}

	/// <summary>
	/// 引渡済みシリアルキー一覧文字列取得
	/// </summary>
	/// <param name="orderItemNo">注文商品枝番</param>
	/// <returns>引渡済みシリアルキー一覧文字列</returns>
	protected string GetDeliveredSerialKeyList(string orderItemNo)
	{
		// デジタルコンテンツオプションOFFであればreturn
		if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED == false) return string.Empty;

		if (this.SerialKeyList == null)
		{
			// シリアルキー情報取得
			using (var accessor = new SqlAccessor())
			using (var statements = new SqlStatement("Order", "GetSendOrderMail_SerialKey"))
			{
				var input = new Hashtable
			{
					{ Constants.FIELD_ORDER_ORDER_ID, this.OrderInput.OrderId }
				};
				this.SerialKeyList = statements.SelectSingleStatementWithOC(accessor, input);
			}
		}

		var deliveredSerialKeyList = new StringBuilder();
		for (int i = 0; i < this.SerialKeyList.Count; i++)
		{
			if (((int)SerialKeyList[i][Constants.FIELD_SERIALKEY_ORDER_ITEM_NO]).ToString() == orderItemNo)
			{
				deliveredSerialKeyList.Append("\r\n");
				deliveredSerialKeyList.Append(string.Format("{0} ： ", ReplaceTag("@@Product.serial_key.name@@")));
				deliveredSerialKeyList.Append(SerialKeyUtility.GetFormattedKeyString(SerialKeyUtility.DecryptSerialKey((string)SerialKeyList[i][Constants.FIELD_SERIALKEY_SERIAL_KEY])));
			}
		}

		return deliveredSerialKeyList.ToString();
	}

	/// <summary>
	/// 配送会社名取得
	/// </summary>
	/// <param name="deliveryCompanyId">配送会社ID</param>
	/// <returns>配送会社名</returns>
	public string GetDeliveryCompanyName(string deliveryCompanyId)
	{
		var deliveryCompany = this.DeliveryCompanyList.FirstOrDefault(i => i.DeliveryCompanyId == deliveryCompanyId);
		return deliveryCompany != null ? deliveryCompany.DeliveryCompanyName : "";
	}

	/// <summary>
	/// 最終利用ポイント詳細ツールチップのテキスト作成
	/// </summary>
	/// <returns>ツールチップ用テキスト</returns>
	protected string CreateOrderPointUseDetailToolTipText()
	{
		var text = string.Join(
			Environment.NewLine,
			this.OrderUsePointDetail
				.Select(x => string.Format(
					"{0}pt {1} - {2}",
					StringUtility.ToNumeric(x.PointInc * -1),
					ValueText.GetValueText(Constants.TABLE_USERPOINT, Constants.FIELD_USERPOINT_POINT_KBN, x.PointKbn),
					DateTimeUtility.ToStringForManager(x.UserPointExp, DateTimeUtility.FormatType.ShortDate2Letter))));
		return text;
	}

	/// <summary>
	/// 領収書情報更新
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateReceipt_Click(object sender, EventArgs e)
	{
		var address = StringUtility.RemoveUnavailableControlCode(tbReceiptAddress.Text);
		var proviso = StringUtility.RemoveUnavailableControlCode(tbReceiptProviso.Text);
		// 領収書希望ありの時に、宛名と但し書きの入力チェックを行う
		if (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_RECEIPT_ADDRESS, address },
				{ Constants.FIELD_ORDER_RECEIPT_PROVISO, proviso },
			};
			var errorMessage = Validator.Validate("OrderReceipt", input);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				tbdyReceiptErrorMessages.Visible = true;
				lbReceiptErrorMessages.Text = errorMessage;
				return;
			}
		}
		// 領収書情報更新
		new OrderService().UpdateOrderReceiptInfo(
			this.OrderInput.OrderId,
			rblReceiptFlg.SelectedValue,
			rblReceiptOutputFlg.SelectedValue,
			address,
			proviso,
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);
		// 注文詳細画面へ
		Response.Redirect(Request.Url.AbsoluteUri);
	}

	/// <summary>
	/// 領収書出力
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReceiptExport_Click(object sender, EventArgs e)
	{
		// 検索情報をセッションに保存
		Session[Constants.SESSION_KEY_PARAM] = new Hashtable
		{
			{ Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId },
			{ Constants.FIELD_ORDER_ORDER_ID + "_identical", this.OrderInput.OrderId },
		};
		// PDF出力ページへ遷移
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PDF_OUTPUT)
			.AddParam(Constants.REQUEST_KEY_PDF_OUTPUT, Constants.KBN_PDF_OUTPUT_ORDER)
			.AddParam(Constants.REQUEST_KEY_PDF_KBN, Constants.KBN_PDF_OUTPUT_RECEIPT)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// Get Taiwan Carry Type Option
	/// </summary>
	/// <returns>string.Empty | 携帯載具コード : TwCarryTypeOption | 自然人証明コード : TwCarryTypeOption</returns>
	public string GetTwCarryTypeOption()
	{
		if (this.TwOrderInvoiceInput == null) return string.Empty;

		if (string.IsNullOrEmpty(this.TwOrderInvoiceInput.TwCarryType))
		{
			return ValueText.GetValueText(
				Constants.TABLE_TWORDERINVOICE,
				Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, this.TwOrderInvoiceInput.TwCarryType);
		}
		else
		{
			return string.Format("{0} ： {1}",
				ValueText.GetValueText(
					Constants.TABLE_TWORDERINVOICE,
					Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE, this.TwOrderInvoiceInput.TwCarryType),
				this.TwOrderInvoiceInput.TwCarryTypeOption);
		}
	}

	/// <summary>
	/// Check Uniform
	/// </summary>
	protected void SetVisibleForUniformOption()
	{
		var isPersonal = false;
		var isCompany = false;
		var isDonate = false;

		if (this.TwOrderInvoiceInput != null)
		{
			OrderCommon.CheckUniformType(
				this.TwOrderInvoiceInput.TwUniformInvoice,
				ref isPersonal,
				ref isDonate,
				ref isCompany);
		}

		this.IsCompany = isCompany;
		this.IsDonate = isDonate;
		this.IsPersonal = isPersonal;
	}

	/// <summary>
	/// Cvs Payment Slip Printing Click Event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCVSPaymentSlipPrinting_Click(object sender, EventArgs e)
	{
		var clientScript = ECPayUtility.CreateScriptForDownloadPaymentSlip(
			this.OrderInput.DeliveryTranId,
			this.OrderInput.Shippings[0].ShippingCheckNo,
			this.OrderInput.Shippings[0].ShippingReceivingStoreType);
		ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ConvenienceStorePaymentSlipPrinting", clientScript, true);
	}

	/// <summary>
	/// Cvs Cooperation Click Event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCVSCooperation_Click(object sender, EventArgs e)
	{
		var orderInfo = new OrderService().GetOrderInfoByOrderId(this.OrderInput.OrderId);
		var request = ECPayUtility.CreateRequestDataForUpdateOrder(orderInfo);
		var response = ECPayApiFacade.UpdateOrderInfo(request);

		if (response.IsExecUpdateApiSuccess == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CONVENIENCE_STORE_COPPERATION_HAS_FAILED);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE_SHIPPING;

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_MODIFY_INPUT)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, this.OrderInput.OrderId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE_SHIPPING)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// Download issued invoice click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDownloadIssuedInvoice_Click(object sender, EventArgs e)
	{
		var parameters = TwInvoiceApi.CreateParameters(
			"invoice",
			this.TwOrderInvoiceInput.TwInvoiceNo);
		var response = TwInvoiceApi.CreateTwInvoiceResponse(TwInvoiceApi.URL_GET_ISSUED_INVOICE, parameters);

		if (CheckAndSetErrorMessageForTwInvoice(response.ReturnCode, response.ReturnMessage) == false) return;

		RegisterClientScript(TwInvoiceApi.URL_GET_ISSUED_INVOICE, parameters);
	}

	/// <summary>
	/// Download refunded invoice click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDownloadRefundedInvoice_Click(object sender, EventArgs e)
	{
		var parameters = TwInvoiceApi.CreateParameters(
			"allowance",
			this.TwOrderInvoiceInput.TwInvoiceNo);
		var response = TwInvoiceApi.CreateTwInvoiceResponse(TwInvoiceApi.URL_GET_REFUNDED_INVOICE, parameters);

		if (CheckAndSetErrorMessageForTwInvoice(response.ReturnCode, response.ReturnMessage) == false) return;

		RegisterClientScript(TwInvoiceApi.URL_GET_REFUNDED_INVOICE, parameters);
	}

	/// <summary>
	/// Check and set error message for Taiwan invoice
	/// </summary>
	/// <param name="code">Code</param>
	/// <param name="message">Message</param>
	/// <returns>True: if no error, otherwise is false</returns>
	private bool CheckAndSetErrorMessageForTwInvoice(string code, string message)
	{
		if ((string.IsNullOrEmpty(code) == false)
			|| (string.IsNullOrEmpty(message) == false))
		{
			lbDownloadInvoiceErrorMessage.Text = message;
			lbDownloadInvoiceErrorMessage.Visible = true;
			return false;
		}

		lbDownloadInvoiceErrorMessage.Text = string.Empty;
		lbDownloadInvoiceErrorMessage.Visible = false;
		return true;
	}

	/// <summary>
	/// Send data and get pdf file from api
	/// </summary>
	/// <param name="url">Url</param>
	/// <param name="parameters">Parameters</param>
	private void RegisterClientScript(string url, List<KeyValuePair<string, string>> parameters)
	{
		var clientScript = string.Format(
			"postDataToDownloadInvoice('{0}{1}','Electronic Invoice',['{2}'],['{3}'])",
			Constants.TWINVOICE_URL,
			url,
			string.Join("','", parameters.Select(item => item.Key)),
			string.Join("','", parameters.Select(item => HttpUtility.UrlEncode(item.Value))));

		ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "InvoicePrinting", clientScript, true);
	}

	/// <summary>
	/// Click invoice issue button
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInvoiceIssue_Click(object sender, EventArgs e)
	{
		using (var accessor = new SqlAccessor())
		{
			try
			{
				// Begin transaction
				accessor.OpenConnection();
				accessor.BeginTransaction();
				var invoiceEcPayApi = new TwInvoiceEcPayApi();

				// Get order information
				var order = new OrderService().GetOrderInfoByOrderId(this.RequestOrderId);

				// Get Taiwan order invoice information
				var twOrderInvoice = new TwOrderInvoiceService().GetOrderInvoice(
					order.OrderId,
					order.Shippings.FirstOrDefault().OrderShippingNo);
				if (twOrderInvoice == null) return;

				// Execute issue api
				var request = invoiceEcPayApi.CreateRequestObject(
					TwInvoiceEcPayApi.ExecuteTypes.Issue,
					order,
					twOrderInvoice,
					accessor);
				var response = invoiceEcPayApi.ReceiveResponseObject(
					TwInvoiceEcPayApi.ExecuteTypes.Issue,
					request);

				// Display error messages
				if (response.IsSuccess == false)
				{
					dvInvoiceErrorMessages.Visible = true;
					lbInvoiceErrorMessages.Text = response.Message;
					return;
				}

				// Update Taiwan order invoice for modify
				twOrderInvoice.TwInvoiceDate = DateTime.Parse(response.Response.Data.InvoiceDate);
				twOrderInvoice.TwInvoiceNo = response.Response.Data.InvoiceNo;
				twOrderInvoice.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_ISSUED_LINKED;
				new TwOrderInvoiceService().UpdateTwOrderInvoiceForModify(
					twOrderInvoice,
					this.LoginOperatorName,
					UpdateHistoryAction.Insert,
					accessor);

				// Commit transaction
				accessor.CommitTransaction();
			}
			catch
			{
				// Rollback transaction
				accessor.RollbackTransaction();
				throw;
			}
		}
		Session[SESSION_KEY_IS_FOCUS] = true;

		// Reload page
		Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
	}

	/// <summary>
	/// Click invoice cancel button
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInvoiceCancel_Click(object sender, EventArgs e)
	{
		using (var accessor = new SqlAccessor())
		{
			try
			{
				// Begin transaction
				accessor.OpenConnection();
				accessor.BeginTransaction();
				var invoiceEcPayApi = new TwInvoiceEcPayApi();

				// Get order information
				var order = new OrderService().GetOrderInfoByOrderId(this.RequestOrderId);

				// Get Taiwan order invoice information
				var twOrderInvoice = new TwOrderInvoiceService().GetOrderInvoice(
					order.OrderId,
					order.Shippings.FirstOrDefault().OrderShippingNo,
					accessor);
				if (twOrderInvoice == null) return;

				// Execute invalid api
				var request = invoiceEcPayApi.CreateRequestCancelObject(
					TwInvoiceEcPayApi.ExecuteTypes.Invalid,
					order,
					twOrderInvoice,
					accessor);
				var response = invoiceEcPayApi.ReceiveResponseObject(
					TwInvoiceEcPayApi.ExecuteTypes.Invalid,
					request);

				// Display error messages
				if ((response.IsSuccess == false)
					&& response.IsInvoiceCanceled)
				{
					dvInvoiceErrorMessages.Visible = true;
					lbInvoiceErrorMessages.Text = response.Message;
					return;
				}

				// Update Taiwan order invoice status
				new TwOrderInvoiceService().UpdateTwOrderInvoiceStatus(
					order.OrderId,
					order.Shippings.FirstOrDefault().OrderShippingNo,
					Constants.FLG_ORDER_INVOICE_STATUS_CANCEL,
					response.Response.Data.InvoiceNo,
					accessor);

				// Commit transaction
				accessor.CommitTransaction();
			}
			catch
			{
				// Rollback transaction
				accessor.RollbackTransaction();
				throw;
			}
		}
		Session[SESSION_KEY_IS_FOCUS] = true;

		// Reload page
		Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
	}

	/// <summary>
	/// 電子発票払い戻しボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInvoiceRefund_Click(object sender, EventArgs e)
	{
		using (var accessor = new SqlAccessor())
		{
			try
			{
				// Begin transaction
				accessor.OpenConnection();
				accessor.BeginTransaction();
				var invoiceEcPayApi = new TwInvoiceEcPayApi();

				// Get order information
				var order = new OrderService().GetOrderInfoByOrderId(this.RequestOrderId);

				// Get Taiwan order invoice information
				var twOrderInvoice = new TwOrderInvoiceService().GetOrderInvoice(
					order.OrderId,
					order.Shippings.FirstOrDefault().OrderShippingNo,
					accessor);
				if (twOrderInvoice == null) return;

				// Execute allowance api
				var request = invoiceEcPayApi.CreateRequestObject(
					TwInvoiceEcPayApi.ExecuteTypes.Allowance,
					order,
					twOrderInvoice,
					accessor);
				var response = invoiceEcPayApi.ReceiveResponseObject(
					TwInvoiceEcPayApi.ExecuteTypes.Allowance,
					request);

				// Display error messages
				if (response.IsSuccess)
				{
					twOrderInvoice.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_REFUND_COMPLETED;
				}
				else
				{
					dvInvoiceErrorMessages.Visible = true;
					lbInvoiceErrorMessages.Text = response.Message;
					return;
				}

				// Update Taiwan order invoice for modify
				twOrderInvoice.TwInvoiceNo = response.Response.Data.IAAllowNo;
				new TwOrderInvoiceService().UpdateTwOrderInvoiceForModify(
					twOrderInvoice,
					this.LoginOperatorName,
					UpdateHistoryAction.Insert,
					accessor);

				// Commit transaction
				accessor.CommitTransaction();
			}
			catch
			{
				// Rollback transaction
				accessor.RollbackTransaction();
				throw;
			}
		}
		Session[SESSION_KEY_IS_FOCUS] = true;

		// Reload page
		Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
	}

	/// <summary>
	/// Check display of issue button
	/// </summary>
	/// <returns>True: If can display, otherwise: false</returns>
	protected bool CheckDisplayIssueButton()
	{
		var result = false;
		if (this.TwOrderInvoiceInput == null) return false;
		result = (this.OrderInput.IsCancelOrder)
			? false
			: (this.OrderInput.IsReturnOrder)
				? (this.TwOrderInvoiceInput.TwInvoiceStatus != Constants.FLG_ORDER_INVOICE_STATUS_CANCEL)
				: ((this.TwOrderInvoiceInput.TwInvoiceStatus == Constants.FLG_ORDER_INVOICE_STATUS_NOT_ISSUED)
					|| (this.TwOrderInvoiceInput.TwInvoiceStatus == Constants.FLG_ORDER_INVOICE_STATUS_CANCEL));
		return result;
	}

	/// <summary>
	/// Check display of cancel button
	/// </summary>
	/// <returns>True: If can display, otherwise: false</returns>
	protected bool CheckDisplayCancelButton()
	{
		var result = false;
		if (this.TwOrderInvoiceInput == null) return false;
		result = (this.TwOrderInvoiceInput.TwInvoiceStatus
			== Constants.FLG_ORDER_INVOICE_STATUS_ISSUED_LINKED);
		return result;
	}

	/// <summary>
	/// 請求書取得表示チェック
	/// </summary>
	/// <returns>表示ならTRUE</returns>
	protected bool CheckDisplayInvoice()
	{
		return (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			&& ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene)
				|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk)
				|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score)
				|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Veritrans));
	}

	/// <summary>
	/// 請求書取得ボタンチェック
	/// </summary>
	/// <returns>押下可能ならTRUE</returns>
	protected bool CheckEnabledInvoiceButton()
	{
		switch (Constants.PAYMENT_CVS_DEF_KBN)
		{
			case Constants.PaymentCvsDef.Dsk:
				return this.InvoiceData == null;

			case Constants.PaymentCvsDef.Score:
			case Constants.PaymentCvsDef.Veritrans:
				return this.OrderInput.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON;

			default:
				return false;
		}
	}

	/// <summary>
	/// 返品注文の定額価格がマイナスになるか
	/// </summary>
	/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
	/// <returns>マイナスになるであればTRUE</returns>
	protected bool IsFixedAmountMinusWhenReturn(string subscriptionBoxCourseId)
	{
		if (this.OrderInput.IsNotReturnExchangeOrder) return false;

		var result = this.OriginalOrder.AllReturnedFixedAmountCourseIds
			.Contains(subscriptionBoxCourseId);
		return result;
	}

	/// <summary>
	/// DSK後払い与信情報取得実行
	/// </summary>
	private void ExecDskAuthResult()
	{
		var adapter = new DskDeferredGetAuthResultAdapter(
			this.OrderInput.CardTranId,
			this.OrderInput.PaymentOrderId,
			this.OrderInput.LastBilledAmount.ToPriceString());
		var response = adapter.Execute();

		if (response.IsResultOk == false)
		{
			var apiErrorMessages = response.Errors.Error.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)).ToArray();
			var externalPaymentMessage = string.Join("\t", apiErrorMessages);
			OrderCommon.AppendExternalPaymentCooperationLog(
				(string.IsNullOrEmpty(externalPaymentMessage) == false),
				this.OrderInput.OrderId,
				externalPaymentMessage,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages
				.GetMessages(WebMessages.ERRMSG_MANAGER_GET_AUTH_RESULT_ERROR)
				.Replace("@@ 1 @@", string.Join("\r\n", apiErrorMessages));
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		switch (response.Transaction.AuthorResult)
		{
			case DskDeferredConst.GET_AUTH_RESULT_AUTH_RESULT_OK:
				this.OrderInput.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
				this.OrderInput.ExternalPaymentAuthDate = DateTime.Now.ToString();
				break;

			case DskDeferredConst.GET_AUTH_RESULT_AUTH_RESULT_NG:
				this.OrderInput.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR;
				break;

			case DskDeferredConst.GET_AUTH_RESULT_AUTH_RESULT_PENDING:
				this.OrderInput.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_PEND;
				break;

			case DskDeferredConst.GET_AUTH_RESULT_AUTH_RESULT_HOLD:
				break;
		}

		new OrderService().UpdateOrderStatusAndExternalPaymentStatus(
			this.OrderInput.OrderId,
			this.OrderInput.OrderStatus,
			this.OrderInput.ExternalPaymentStatus,
			DateTime.Parse(this.OrderInput.ExternalPaymentAuthDate),
			DateTime.Now,
			this.LoginOperatorName,
			UpdateHistoryAction.Insert
		);

		var apiPendingMessgae = "";
		if (response.Transaction.HoldReason != null)
		{
			apiPendingMessgae = string.Join("\t", response.Transaction.HoldReason
			.Select(x => LogCreator.CreateErrorMessage(x.ReasonCode, x.Reason)).ToArray());
		}

		OrderCommon.AppendExternalPaymentCooperationLog(
			true,
			this.OrderInput.OrderId,
			(string.IsNullOrEmpty(apiPendingMessgae) == false)
				? apiPendingMessgae
				: LogCreator.CrateMessageWithCardTranId(
					this.OrderInput.CardTranId, this.OrderInput.PaymentOrderId),
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// ベリトランス後払い与信結果取得実行
	/// </summary>
	private void ExecVeritransAuthResult()
	{
		var response = new PaymentVeritransCvsDef().GetAuthResult(this.OrderInput.PaymentOrderId);

		if (response.Mstatus == VeriTransConst.RESULT_STATUS_NG)
		{
			var apiErrorMessage = response.Errors != null
				? string.Join(
					"\t",
					response.Errors.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)).ToArray())
				: LogCreator.CreateErrorMessage(response.VResultCode, response.MerrMsg);

			AppLogger.WriteError(string.Format("ベリトランス後払い与信結果取得失敗：{0}：{1}", apiErrorMessage, this.OrderInput.OrderId));

			OrderCommon.AppendExternalPaymentCooperationLog(
				string.IsNullOrEmpty(apiErrorMessage) == false,
				this.OrderInput.OrderId,
				apiErrorMessage,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert
			);
		}

		if (response.AuthorResult == VeriTransConst.VeritransAuthorResult.Ok.ToText())
		{
			this.OrderInput.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			this.OrderInput.ExternalPaymentAuthDate = DateTime.Now.ToString();
		}
		else if (response.AuthorResult == VeriTransConst.VeritransAuthorResult.Ng.ToText())
		{
			this.OrderInput.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR;
		}
		else if (response.AuthorResult == VeriTransConst.VeritransAuthorResult.Hold.ToText())
		{
			this.OrderInput.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST;
		}
		else if (response.AuthorResult == VeriTransConst.VeritransAuthorResult.Hr.ToText())
		{
			this.OrderInput.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_PEND;
		}

		new OrderService().UpdateOrderStatusAndExternalPaymentStatus(
			this.OrderInput.OrderId,
			this.OrderInput.OrderStatus,
			this.OrderInput.ExternalPaymentStatus,
			DateTime.Parse(this.OrderInput.ExternalPaymentAuthDate),
			DateTime.Now,
			this.LoginOperatorName,
			UpdateHistoryAction.Insert
		);

		var apiPendingMessgae = string.Empty;
		if (response.HoldReasons != null)
		{
			apiPendingMessgae = string.Join(
				"\t",
				response.HoldReasons.Select(x => LogCreator.CreateErrorMessage(x.ReasonCode, x.Reason)).ToArray());
		}

		OrderCommon.AppendExternalPaymentCooperationLog(
			true,
			this.OrderInput.OrderId,
			string.IsNullOrEmpty(apiPendingMessgae) == false
				? apiPendingMessgae
				: LogCreator.CrateMessageWithCardTranId(this.OrderInput.CardTranId, this.OrderInput.PaymentOrderId),
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);
	}
	#endregion

	#region プロパティ
	/// <summary>リクエスト：注文ID</summary>
	protected string RequestOrderId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ID]).Trim(); }
	}
	/// <summary>注文入力情報</summary>
	protected OrderInput OrderInput
	{
		get { return (OrderInput)ViewState["OrderInput"]; }
		set { ViewState["OrderInput"] = value; }
	}
	/// <summary>配送種別情報</summary>
	protected ShopShippingModel ShopShipping
	{
		get { return (ShopShippingModel)ViewState["ShopShipping"]; }
		set { ViewState["ShopShipping"] = value; }
	}
	/// <summary>引渡済みシリアルキー一覧（詳細画面表示用）</summary>
	protected DataView SerialKeyList = null;
	/// <summary>アイテムテーブル用追加カラム数</summary>
	protected int AddColumnCountForItemTable
	{
		get
		{
			return
				(Constants.PRODUCT_SALE_OPTION_ENABLED ? 1 : 0)
				+ (this.OrderInput.HasSetProduct ? 1 : 0)
				+ (Constants.REALSTOCK_OPTION_ENABLED ? 1 : 0)
				+ ((Constants.NOVELTY_OPTION_ENABLED || Constants.RECOMMEND_OPTION_ENABLED) ? 1 : 0)
				+ ((this.OrderInput.IsNotReturnExchangeOrder == false) ? 1 : 0)
				+ (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 1 : 0)
				+ ((this.OrderInput.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON) ? 1 : 0)
				+ (this.IsFixedPurchaseCountAreaShow ? 2 : 0)
				+ (this.DisplayItemSubscriptionBoxCourseIdArea ? 1 : 0);
		}
	}
	/// <summary>UserCreditCardInfo</summary>
	public UserCreditCard UserCreditCardInfo { get; set; }
	/// <summary>配送会社リスト</summary>
	public DeliveryCompanyModel[] DeliveryCompanyList
	{
		get
		{
			if (m_deliveryComapnyList == null)
			{
				var service = new DeliveryCompanyService();
				this.m_deliveryComapnyList = service.GetAll();
			}
			return m_deliveryComapnyList;
		}
	}
	public DeliveryCompanyModel[] m_deliveryComapnyList;
	/// <summary>利用ポイント詳細</summary>
	protected UserPointHistoryModel[] OrderUsePointDetail
	{
		get { return (UserPointHistoryModel[])ViewState["OrderUsePoihtDetail"]; }
		set { ViewState["OrderUsePoihtDetail"] = value; }
	}
	/// <summary>Taiwain Order Invoice</summary>
	protected TwOrderInvoiceInput TwOrderInvoiceInput
	{
		get { return (TwOrderInvoiceInput)ViewState["TwOrderInvoiceInput"]; }
		set { ViewState["TwOrderInvoiceInput"] = value; }
	}
	/// <summary>Invoice Uniform: COMPANY</summary>
	protected bool IsCompany { get; set; }
	/// <summary>Invoice Uniform: DONATE</summary>
	protected bool IsDonate { get; set; }
	/// <summary>Invoice Uniform: PERSONAL</summary>
	protected bool IsPersonal { get; set; }
	/// <summary>Is Not Display Demand Status</summary>
	protected bool IsNotDisplayDemandStatus
	{
		get
		{
			return ((this.OrderInput.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
				|| (this.OrderInput.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE));
		}
	}
	/// <summary>Is CVS And Have Shipping Check No</summary>
	protected bool IsCVSAndHaveShippingCheckNo
	{
		get
		{
			return ((this.OrderInput.Shippings[0].DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)
				&& (string.IsNullOrEmpty(this.OrderInput.Shippings[0].ShippingCheckNo) == false));
		}
	}
	/// <summary>Is CVS 7 Eleven</summary>
	protected bool IsCVS7Eleven
	{
		get
		{
			return ECPayUtility.CheckShippingReceivingStoreType7Eleven(this.OrderInput.Shippings[0].ShippingReceivingStoreType);
		}
	}
	/// <summary>Is Shipping Convenience</summary>
	protected bool IsShippingConvenience
	{
		get
		{
			var result = this.OrderInput.Shippings.Any(
				shipping => (shipping.ShippingReceivingStoreFlg
					== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON));
			return result;
		}
	}

	protected object InvoiceData { get; set; }
	/// <summary>与信結果がHOLDの注文か(現在はコンビニ後払い(DSK))のみ利用)</summary>
	protected bool IsAuthResultHoldOrder
	{
		get
		{
			return ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk)
					&& (this.OrderInput.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST));
		}
	}
	/// <summary>返金先銀行口座情報</summary>
	protected Dictionary<string, string> RepaymentBank
	{
		get
		{
			return m_repaymentBank ?? (m_repaymentBank = OrderCommon.CreateRepaymentBankDictionary(this.OrderInput.RepaymentMemo));
		}
		set { m_repaymentBank = value; }
	}
	private Dictionary<string, string> m_repaymentBank;
	/// <summary>定期購入回数エリア表示するかどうか</summary>
	protected bool IsFixedPurchaseCountAreaShow
	{
		get
		{
			var result = (this.OrderInput.Shippings[0].Items.Any(orderItem => orderItem.IsFixedPurchaseItem)
				&& Constants.FIXEDPURCHASE_OPTION_ENABLED);
			return result;
		}
	}
	/// <summary>仮ポイント</summary>
	public decimal PointTemporary { get; set; }
	/// <summary>注文商品の頒布会コースIDエリアを表示するか</summary>
	protected bool DisplayItemSubscriptionBoxCourseIdArea
	{
		get
		{
			if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED == false) return false;

			// 返品交換注文は元注文を参照する
			return this.OrderInput.IsNotReturnExchangeOrder
				? this.OrderInput.IsOrderCombinedWithSubscriptionBoxItem
					&& (this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false)
				: this.OriginalOrder.IsOrderCombinedWithSubscriptionBoxItem
					&& (this.OriginalOrder.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false);
		}
	}
	/// <summary>定額頒布会商品を含む時、注文商品エリアを全て表示するか</summary>
	protected bool DisplayAllWhenHasFixedAmountItem
	{
		get
		{
			// 返品交換注文は元注文を参照する
			var result = this.OrderInput.IsNotReturnExchangeOrder
				? this.OrderInput.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false
				: this.OriginalOrder.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false;
			return result;
		}
	}
	/// <summary>元注文情報</summary>
	private Order OriginalOrder { get; set; }
	/// <summary>与信結果が保留中の注文か(現在はコンビニ後払い(Veritrans))のみ利用)</summary>
	protected bool IsAuthResultPendOrder
	{
		get
		{
			return ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Veritrans)
				&& (this.OrderInput.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_PEND));
		}
	}
	/// <summary>Is store pick up order</summary>
	public bool IsStorePickUpOrder { get; set; }
	#endregion
}
