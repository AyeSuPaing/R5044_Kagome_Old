/*
=========================================================================================================
  Module      : 定期購入情報登録クラス(FixedPurchaseRegister.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Global;
using w2.App.Common.OrderExtend;
using w2.App.Common.Product;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchaseRepeatAnalysis;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.SubscriptionBox;
using w2.Domain.TwFixedPurchaseInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Register
{
	/// <summary>
	/// 定期購入情報登録クラス
	/// </summary>
	public class FixedPurchaseRegister
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FixedPurchaseRegister()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="lastChanged">最終更新者</param>
		public FixedPurchaseRegister(Hashtable order, CartObject cart, string lastChanged)
		{
			this.Order = order;
			this.Cart = cart;
			this.LastChanged = lastChanged;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 定期購入登録 & 注文情報の定期情報更新
		/// ※カート内に定期購入商品がある場合のみ
		/// </summary>
		/// <param name="fixedPurchaseStatus">定期購入ステータス</param>
		/// <param name="updateHistoryAction">更新履歴アクション（基本インサートしない）</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>定期購入ID</returns>
		public string RegisterAndUpdateFixedPurchaseInfoForOrder(
			string fixedPurchaseStatus,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 定期購入商品が存在しない場合は、処理を抜ける
			if (this.Cart.Items.Exists(p => (p.IsFixedPurchase) || (p.IsSubscriptionBox)) == false) return null;

			// 定期購入情報モデル作成
			var fixedPurchase = CreateFixedPurchase(true);

			// 定期購入ID + 定期購入回数(注文時点)更新
			var orderService = new OrderService();
			orderService.UpdateFixedPurchaseIdAndFixedPurchaseOrderCount(
				StringUtility.ToEmpty(this.Order[Constants.FIELD_ORDER_ORDER_ID]),
				fixedPurchase.FixedPurchaseId,
				fixedPurchase.OrderCount,
				this.LastChanged,
				UpdateHistoryAction.DoNotInsert, // 下部で履歴作成する
				accessor);

			// 定期商品購入回数（注文時点）更新
			orderService.UpdateFixedPerchaseItemOrderCount(
				StringUtility.ToEmpty(this.Order[Constants.FIELD_ORDER_ORDER_ID]),
				fixedPurchase.Shippings[0].Items,
				this.LastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// 定期購入情報登録
			new FixedPurchaseService().Register(
				fixedPurchase,
				(string)this.Order[Constants.FIELD_ORDER_ORDER_ID],
				fixedPurchaseStatus,
				this.LastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// 定期継続分析更新
			new FixedPurchaseRepeatAnalysisService().UpdateRepeatAnalysisFixedPurchaseIdByOrderId(
				(string)this.Order[Constants.FIELD_ORDER_ORDER_ID],
				fixedPurchase.FixedPurchaseId,
				this.LastChanged,
				accessor);

			// Process Switch Product Fixed Purchase Next Shipping Second Time
			ProcessSwitchProductFixedPurchaseNextShippingSecondTime(
				fixedPurchase,
				this.LastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// Insert Fixed Purchase Invoice Information
			if (fixedPurchase.Invoice != null)
			{
				new TwFixedPurchaseInvoiceService().InsertTaiwanFixedPurchaseInvoice(fixedPurchase.Invoice, accessor);
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(
					(string)this.Order[Constants.FIELD_ORDER_ORDER_ID],
					this.LastChanged,
					 accessor);
				new UpdateHistoryService().InsertForFixedPurchase(
					fixedPurchase.FixedPurchaseId,
					this.LastChanged,
					 accessor);
			}

			return fixedPurchase.FixedPurchaseId;
		}

		/// <summary>
		/// 定期情報更新
		/// </summary>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateFixedPurchaseOrder(UpdateHistoryAction updateHistoryAction)
		{
			// 定期購入商品が存在しない場合は、処理を抜ける
			if (this.Cart.Items.Exists(p => p.IsFixedPurchase) == false) return;

			// 定期購入情報モデル作成
			var fixedPurchase = CreateFixedPurchase(false);
			var service = new FixedPurchaseService();
			var fixedPurchaseOld = service.Get((string)this.Order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]);

			// カード枝番変更あり？
			if (fixedPurchase.CreditBranchNo != fixedPurchaseOld.CreditBranchNo)
			{
				service.UpdateOrderPayment(
					fixedPurchase.FixedPurchaseId,
					fixedPurchase.OrderPaymentKbn,
					fixedPurchase.CreditBranchNo,
					fixedPurchase.CardInstallmentsCode,
					fixedPurchase.ExternalPaymentAgreementId,
					this.LastChanged,
					UpdateHistoryAction.DoNotInsert);
				}

			// 配送方法変更
			fixedPurchase.Shippings.ToList()
				.ForEach(shipping => service.UpdateShipping(shipping, this.LastChanged, UpdateHistoryAction.DoNotInsert));

			// 配送パターン変更あり？
			
			if ((fixedPurchase.FixedPurchaseKbn != fixedPurchaseOld.FixedPurchaseKbn)
				|| (fixedPurchase.FixedPurchaseSetting1 != fixedPurchaseOld.FixedPurchaseSetting1)
				|| (fixedPurchase.NextShippingDate != fixedPurchaseOld.NextShippingDate)
				|| (fixedPurchase.NextNextShippingDate != fixedPurchaseOld.NextNextShippingDate))
				{
				service.UpdatePattern(
					fixedPurchase.FixedPurchaseId,
					fixedPurchase.FixedPurchaseKbn,
					fixedPurchase.FixedPurchaseSetting1,
					fixedPurchase.NextShippingDate,
					fixedPurchase.NextNextShippingDate,
					this.LastChanged,
					UpdateHistoryAction.DoNotInsert);
			}

			// 商品変更？
			fixedPurchase.Shippings.ToList()
				.ForEach(shipping => service.UpdateItems(shipping.Items, this.LastChanged, UpdateHistoryAction.DoNotInsert));
		}
		/// <summary>
		/// 注文取込用
		/// 定期購入登録 & 注文情報の定期情報更新
		/// ※カート内に定期購入商品がある場合のみ
		/// </summary>
		/// <param name="updateHistoryAction">更新履歴アクション（基本インサートしない）</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseStatus">定期購入ステータス</param>
		/// <param name="accessor">Sqlアクセサー</param>
		/// <returns>定期購入ID</returns>
		public string RegisterAndUpdateFixedPurchaseInfoForImportOrderFile(
			string fixedPurchaseId,
			string fixedPurchaseStatus,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
		// 定期購入商品が存在しない場合は、処理を抜ける
			if (this.Cart.Items.Exists(p => p.IsFixedPurchase) == false) return null;

			// 定期購入情報モデル作成
			this.Order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID] = fixedPurchaseId;
			var fixedPurchase = CreateFixedPurchase(string.IsNullOrEmpty(fixedPurchaseId));
			// 登録
			// 定期購入ID + 定期購入回数(注文時点)更新
			new OrderService()
				.UpdateFixedPurchaseIdAndFixedPurchaseOrderCount(
				(string)this.Order[Constants.FIELD_ORDER_ORDER_ID],
				fixedPurchase.FixedPurchaseId,
				fixedPurchase.OrderCount,
				this.LastChanged,
				UpdateHistoryAction.DoNotInsert, // 下の方で履歴作成する
				accessor);

			// 定期購入情報登録
			new FixedPurchaseService().Register(
				fixedPurchase,
				(string)this.Order[Constants.FIELD_ORDER_ORDER_ID],
				Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_TEMP,
				this.LastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);
			

			// 定期継続分析更新
			new FixedPurchaseRepeatAnalysisService()
				.UpdateRepeatAnalysisFixedPurchaseIdByOrderId(
				(string)this.Order[Constants.FIELD_ORDER_ORDER_ID],
				fixedPurchase.FixedPurchaseId,
				this.LastChanged,
				accessor);

			// Process Switch Product Fixed Purchase Next Shipping Second Time
			ProcessSwitchProductFixedPurchaseNextShippingSecondTime(
				fixedPurchase,
				this.LastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertAllForOrder(
					(string)this.Order[Constants.FIELD_ORDER_ORDER_ID],
					this.LastChanged,
					accessor);
			}
			
			// 頒布会コースか判断
			if (string.IsNullOrEmpty((string)this.Order[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID])) return fixedPurchase.FixedPurchaseId;

			// 次回配送商品の更新を行う
			var fixedPurchaseContainer = new FixedPurchaseService().GetContainer(fixedPurchase.FixedPurchaseId, false, accessor);
			if (fixedPurchaseContainer.NextShippingDate != null)
			{
				var nextDate = fixedPurchaseContainer.NextShippingDate.Value;
				var getNextProductsResult = new SubscriptionBoxService().GetFixedPurchaseNextProduct(
					(string)this.Order[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID],
					fixedPurchaseContainer.FixedPurchaseId,
					fixedPurchaseContainer.MemberRankId,
					nextDate,
					fixedPurchase.SubscriptionBoxOrderCount + 1,
					fixedPurchaseContainer.Shippings[0]);
				new FixedPurchaseService().UpdateNextDeliveryForSubscriptionBox(
					fixedPurchaseContainer.FixedPurchaseId,
					this.LastChanged,
					Constants.W2MP_POINT_OPTION_ENABLED,
					getNextProductsResult,
					UpdateHistoryAction.DoNotInsert,
					accessor);
			}

			// 頒布会コースの自動繰り返し設定がOFF且つ注文回数が満期になったものは定期購入ステータスを完了にする
			var subscriptionBox = new SubscriptionBoxService().GetByCourseId(
				(string)this.Order[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID]);
			if ((subscriptionBox.IsNumberTime) && (subscriptionBox.IsAutoRenewal == false))
			{
				var maxCount = subscriptionBox.DefaultOrderProducts.Max(defaultItem => defaultItem.Count).Value;
				if (fixedPurchaseContainer.SubscriptionBoxOrderCount >= maxCount)
				{
					new FixedPurchaseService().Complete(
						fixedPurchaseContainer.FixedPurchaseId,
						Constants.FLG_LASTCHANGED_BATCH,
						fixedPurchaseContainer.NextShippingDate,
						fixedPurchaseContainer.NextNextShippingDate,
						UpdateHistoryAction.Insert,
						accessor);
				}
			}

			if (subscriptionBox.IsNumberTime == false)
			{
				// 次回配送日がデフォルト商品配送期間の最終日より後の時に定期購入ステータスを完了にする
				var lastDate = subscriptionBox.DefaultOrderProducts.Max(dp => dp.TermUntil);
				if ((lastDate != null) && (lastDate < fixedPurchaseContainer.NextShippingDate))
				{
					new FixedPurchaseService().Complete(
						fixedPurchaseContainer.FixedPurchaseId,
						Constants.FLG_LASTCHANGED_BATCH,
						fixedPurchaseContainer.NextShippingDate,
						fixedPurchaseContainer.NextNextShippingDate,
						UpdateHistoryAction.Insert,
						accessor);
				}
			}

			return fixedPurchase.FixedPurchaseId;
		}

		/// <summary>
		/// 定期購入情報モデル作成
		/// </summary>
		/// <param name="doCreateFixedPurchaseId">定期注文IDを採番するか</param>
		private FixedPurchaseModel CreateFixedPurchase(bool doCreateFixedPurchaseId = true)
		{
			int creditBranchNoTmp;
			var subscriptionBoxOrderCountTmp = 0;
			var cartShipping = this.Cart.GetShipping();
			var fixedPurchase = new FixedPurchaseModel
			{
				FixedPurchaseId = doCreateFixedPurchaseId
					? OrderCommon.CreateFixedPurchaseId(this.Cart.ShopId)
					: (string)this.Order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID],
				FixedPurchaseKbn = cartShipping.CanSwitchProductFixedPurchaseNextShippingSecondTime
					? cartShipping.NextShippingItemFixedPurchaseKbn
					: cartShipping.FixedPurchaseKbn,
				FixedPurchaseSetting1 = cartShipping.CanSwitchProductFixedPurchaseNextShippingSecondTime
					? cartShipping.NextShippingItemFixedPurchaseSetting
					: cartShipping.FixedPurchaseSetting,
				LastOrderDate = DateTime.Now,
				OrderCount = 1,
				UserId = (string)this.Order[Constants.FIELD_ORDER_USER_ID],
				ShopId = this.Cart.ShopId,
				OrderKbn = (string)this.Order[Constants.FIELD_ORDER_ORDER_KBN],
				OrderPaymentKbn = this.Cart.Payment.PaymentId,
				FixedPurchaseDateBgn = DateTime.Now,
				LastChanged = this.LastChanged,
				CreditBranchNo = int.TryParse(StringUtility.ToEmpty(this.Order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO]), out creditBranchNoTmp)
					? (int?)creditBranchNoTmp : null,
				NextShippingDate = cartShipping.NextShippingDate,
				NextNextShippingDate = cartShipping.NextNextShippingDate,
				FixedPurchaseManagementMemo = StringUtility.ToEmpty(this.Order[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO]),
				ShippingMemo = StringUtility.ToEmpty(this.Order[Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO]),
				CardInstallmentsCode = StringUtility.ToEmpty(this.Cart.Payment.CreditInstallmentsCode),
				AccessCountryIsoCode = this.Cart.Owner.AccessCountryIsoCode,
				DispLanguageCode = this.Cart.Owner.DispLanguageCode,
				DispLanguageLocaleId = this.Cart.Owner.DispLanguageLocaleId,
				DispCurrencyCode = this.Cart.Owner.DispCurrencyCode,
				DispCurrencyLocaleId = this.Cart.Owner.DispCurrencyLocaleId,
				ExternalPaymentAgreementId = this.Cart.ExternalPaymentAgreementId ?? string.Empty,
				SubscriptionBoxCourseId = this.Cart.SubscriptionBoxCourseId,
				SubscriptionBoxFixedAmount = this.Cart.SubscriptionBoxFixedAmount,
			};
			fixedPurchase.SubscriptionBoxOrderCount = (string.IsNullOrEmpty((string)this.Order[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID]) == false)
				? (int.TryParse((string)this.Order[Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_ORDER_COUNT], out subscriptionBoxOrderCountTmp))
					&& (subscriptionBoxOrderCountTmp != 0) 
				? subscriptionBoxOrderCountTmp
				: 1
				: 0;
			// 定期購入配送先情報モデル作成
			fixedPurchase.Shippings = CreateFixedPurchaseShippings(fixedPurchase);
			fixedPurchase.Memo = this.Cart.ReflectMemoToFixedPurchase ? this.Cart.GetOrderMemos() : string.Empty;

			// 領収書情報をセット
			if (Constants.RECEIPT_OPTION_ENABLED && (this.Cart.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON))
			{
				fixedPurchase.ReceiptFlg = this.Cart.ReceiptFlg;
				fixedPurchase.ReceiptAddress = this.Cart.ReceiptAddress;
				fixedPurchase.ReceiptProviso = this.Cart.ReceiptProviso;
			}

			// Create Fixed Purchase Invoice
			fixedPurchase.Invoice = CreateFixedPurchaseInvoice(fixedPurchase.Shippings[0]);

			if (Constants.BOTCHAN_OPTION && this.Cart.IsBotChanOrder)
			{
				fixedPurchase.ExtendStatus40 = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON;
				fixedPurchase.ExtendStatusDate40 = DateTime.Now;
			}

			OrderExtendCommon.SetOrderExtend(fixedPurchase, this.Cart.OrderExtend);

			// 次回購入の利用ポイントの全適用フラグ
			if (Constants.FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT_ALL_OPTION_ENABLE)
			{
				fixedPurchase.UseAllPointFlg = this.Cart.UseAllPointFlg ? Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON : Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_OFF;
			}

			return fixedPurchase;
		}

		/// <summary>
		/// 定期購入配送先情報モデル作成
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報モデル</param>
		/// <returns>定期購入配送先情報モデル</returns>
		private FixedPurchaseShippingModel[] CreateFixedPurchaseShippings(FixedPurchaseModel fixedPurchase)
		{
			var cartShipping = this.Cart.GetShipping();
			var shipping = new FixedPurchaseShippingModel
			{
				FixedPurchaseId = fixedPurchase.FixedPurchaseId,
				ShippingName = cartShipping.Name1 + cartShipping.Name2,
				ShippingName1 = cartShipping.Name1,
				ShippingName2 = cartShipping.Name2,
				ShippingNameKana = cartShipping.NameKana1 + cartShipping.NameKana2,
				ShippingNameKana1 = cartShipping.NameKana1,
				ShippingNameKana2 = cartShipping.NameKana2,
				ShippingCountryName = cartShipping.ShippingCountryName,
				ShippingCountryIsoCode = cartShipping.ShippingCountryIsoCode,
				ShippingZip = cartShipping.Zip,
				ShippingAddr1 = cartShipping.Addr1,
				ShippingAddr2 = cartShipping.Addr2,
				ShippingAddr3 = cartShipping.Addr3,
				ShippingAddr4 = cartShipping.Addr4,
				ShippingAddr5 = cartShipping.Addr5,
				ShippingTel1 = cartShipping.Tel1,
				ShippingTime = (cartShipping.SpecifyShippingTimeFlg && (cartShipping.ShippingTime != null)) ? cartShipping.ShippingTime : "",
				ShippingCompanyName = cartShipping.CompanyName,
				ShippingCompanyPostName = cartShipping.CompanyPostName,
				ShippingMethod = cartShipping.ShippingMethod,
				DeliveryCompanyId = cartShipping.DeliveryCompanyId,
				ShippingReceivingStoreFlg = StringUtility.ToEmpty(cartShipping.ConvenienceStoreFlg),
				ShippingReceivingStoreId = StringUtility.ToEmpty(cartShipping.ConvenienceStoreId),
				ShippingReceivingStoreType = StringUtility.ToEmpty(cartShipping.ShippingReceivingStoreType)
			};
			// 定期購入商品情報モデル作成
			shipping.Items = CreateFixedPurchaseItems(shipping, fixedPurchase);

			return new[] { shipping };
		}

		/// <summary>
		/// 定期購入商品情報モデル作成
		/// </summary>
		/// <param name="fixedPurchaseShipping">定期購入配送先情報モデル</param>
		/// <returns>定期購入商品情報モデル</returns>
		private FixedPurchaseItemModel[] CreateFixedPurchaseItems(FixedPurchaseShippingModel fixedPurchaseShipping, FixedPurchaseModel fixedPurchase = null)
		{
			var items = new List<FixedPurchaseItemModel>();
			var itemNo = 1;
			var registerItems = this.Cart.GetItemsRegisteredFixedPurchase();
			foreach (var product in registerItems)
			{
				var item = new FixedPurchaseItemModel
				{
					FixedPurchaseId = fixedPurchaseShipping.FixedPurchaseId,
					FixedPurchaseItemNo = itemNo,
					FixedPurchaseShippingNo = fixedPurchaseShipping.FixedPurchaseShippingNo,
					ShopId = product.ShopId,
					ProductId = product.ProductId,
					VariationId = product.VariationId,
					SupplierId = product.SupplierId,
					ItemQuantity = product.Count,
					ItemQuantitySingle = product.CountSingle,
					ProductOptionTexts = (product.ProductOptionSettingList != null)
						? ProductOptionSettingHelper.GetSelectedOptionSettingForFixedPurchaseItem(product.ProductOptionSettingList)
						: string.Empty,
					ItemOrderCount = 1,
				};
				items.Add(item);

				itemNo++;
			}

			return items.ToArray();
		}

		/// <summary>
		/// Register And Update Fixed Purchase Info For Import Order Ureru
		/// </summary>
		/// <param name="fixedPurchase">Fixed Purchase</param>
		/// <param name="orderId">Order Id</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="accessor">SQL Accessor</param>
		public void RegisterAndUpdateFixedPurchaseInfoForImportOrderUreru(
			FixedPurchaseModel fixedPurchase,
			string orderId,
			string lastChanged,
			SqlAccessor accessor)
		{
			// Register fixed purchase
			new FixedPurchaseService().Register(
				fixedPurchase,
				orderId,
				Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_TEMP,
				lastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// Process for switch product fixed purchase next shipping second time
			ProcessSwitchProductFixedPurchaseNextShippingSecondTime(
				fixedPurchase,
				lastChanged,
				UpdateHistoryAction.Insert,
				accessor);
		}

		/// <summary>
		/// Process Switch Product Fixed Purchase Next Shipping Second Time
		/// </summary>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">SQL Accessor</param>
		public void ProcessSwitchProductFixedPurchaseNextShippingSecondTime(
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			// 定期購入商品が存在しない場合は、処理を抜ける
			if (this.Cart.Items.Exists(p => p.IsFixedPurchase) == false) return;

			// 定期購入情報モデル作成
			var fixedPurchaseId = StringUtility.ToEmpty(this.Order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]);
			if (string.IsNullOrEmpty(fixedPurchaseId)) return;

			// Process Switch Product Fixed Purchase Next Shipping Second Time
			var fixedPurchase = CreateFixedPurchase(false);
			ProcessSwitchProductFixedPurchaseNextShippingSecondTime(
				fixedPurchase,
				this.LastChanged,
				updateHistoryAction,
				accessor);
		}

		/// <summary>
		/// Process Switch Product Fixed Purchase Next Shipping Second Time
		/// </summary>
		/// <param name="fixedPurchase">Fixed Purchase</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">SQL Accessor</param>
		public void ProcessSwitchProductFixedPurchaseNextShippingSecondTime(
			FixedPurchaseModel fixedPurchase,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED == false) return;

			var fixedPurchaseItemsNextShippingSecondTime = CreatedFixedPurchaseItemsNextShippingSecondTime(
				fixedPurchase.Shippings[0],
				accessor);
			if (fixedPurchaseItemsNextShippingSecondTime.Length == 0) return;

			// Switch product for fixed purchase next shipping second time
			new FixedPurchaseService().Modify(
				fixedPurchase.FixedPurchaseId,
				Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_ITEMUPDATE,
				(model, historyModel) =>
				{
					// 商品をセット
					model.Shippings[0].Items = fixedPurchaseItemsNextShippingSecondTime;

					// 最終更新者をセット
					model.LastChanged = historyModel.LastChanged = lastChanged;
				},
				lastChanged,
				updateHistoryAction,
				accessor);
		}

		/// <summary>
		/// Created Fixed Purchase Items Next Shipping Second Time
		/// </summary>
		/// <param name="fixedPurchaseShipping">Fixed Purchase Shipping</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Fixed Purchase Items Next Shipping Second Time</returns>
		private FixedPurchaseItemModel[] CreatedFixedPurchaseItemsNextShippingSecondTime(
			FixedPurchaseShippingModel fixedPurchaseShipping,
			SqlAccessor accessor = null)
		{
			var items = new List<FixedPurchaseItemModel>();
			var itemNo = 1;
			var countItemSame = 0;
			foreach (var product in fixedPurchaseShipping.Items)
			{
				var item = GetProductForFixedPurchaseItemNextShippingSecondTime(
					product.ShopId,
					product.ProductId,
					product.VariationId,
					product.ItemQuantity,
					accessor);

				if (item != null)
				{
					item.FixedPurchaseId = fixedPurchaseShipping.FixedPurchaseId;
					item.FixedPurchaseShippingNo = fixedPurchaseShipping.FixedPurchaseShippingNo;
				}
				else
				{
					item = product;
					countItemSame++;
				}

				item.FixedPurchaseItemNo = itemNo;
				items.Add(item);
				itemNo++;
			}

			// For case not setting product fixed purchase at second time
			if (countItemSame == fixedPurchaseShipping.Items.Length)
			{
				return new List<FixedPurchaseItemModel>().ToArray();
			}

			return items.ToArray();
		}

		/// <summary>
		/// Get Product For Fixed Purchase Item Next Shipping Second Time
		/// </summary>
		/// <param name="shopId">Shop Id</param>
		/// <param name="productId">Product Id</param>
		/// <param name="variationId">Variation ID</param>
		/// <param name="itemCount">Item Count</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Product For Fixed Purchase Item Next Shipping Second Time</returns>
		private FixedPurchaseItemModel GetProductForFixedPurchaseItemNextShippingSecondTime(
			string shopId,
			string productId,
			string variationId,
			int itemCount,
			SqlAccessor accessor = null)
		{
			// Get product fixed purchase first time information
			var productService = new ProductService();
			var productInfo = productService.GetProductVariation(
				shopId,
				productId,
				variationId,
				string.Empty,
				accessor);
			if (productInfo == null) return null;

			var fixedPurchaseNextShippingProductId = productInfo.FixedPurchaseNextShippingProductId;
			var fixedPurchaseNextShippingVariationId = productInfo.FixedPurchaseNextShippingVariationId;
			var fixedPurchaseNextShippingItemQuantity = productInfo.FixedPurchaseNextShippingItemQuantity;
			if (string.IsNullOrEmpty(fixedPurchaseNextShippingProductId)
				|| (fixedPurchaseNextShippingItemQuantity <= 0)) return null;

			// Get product fixed purchase second time information
			var productFixedPurchaseNextShippingInfo = productService.GetProductVariation(
				shopId,
				fixedPurchaseNextShippingProductId,
				fixedPurchaseNextShippingVariationId,
				string.Empty,
				accessor);

			// Created fixed purchase item
			var item = new FixedPurchaseItemModel()
			{
				ShopId = shopId,
				ProductId = fixedPurchaseNextShippingProductId,
				VariationId = fixedPurchaseNextShippingVariationId,
				ItemQuantity = (itemCount * fixedPurchaseNextShippingItemQuantity),
				ItemQuantitySingle = (itemCount * fixedPurchaseNextShippingItemQuantity)
			};

			if (productFixedPurchaseNextShippingInfo != null)
			{
				item.SupplierId = productFixedPurchaseNextShippingInfo.SupplierId;
				item.ProductOptionTexts = string.Empty;
			}

			return item;
		}

		/// <summary>
		/// Create Fixed Purchase Invoice
		/// </summary>
		/// <param name="fixedPurchaseShippingModel">Fixed Purchase Shipping Model</param>
		/// <returns>Taiwan Fixed Purchase Invoice Model</returns>
		private TwFixedPurchaseInvoiceModel CreateFixedPurchaseInvoice(FixedPurchaseShippingModel fixedPurchaseShippingModel)
		{
			if ((OrderCommon.DisplayTwInvoiceInfo() == false)
				&& (GlobalAddressUtil.IsCountryTw(fixedPurchaseShippingModel.ShippingCountryIsoCode) == false))
			{
				return null;
			}

			var cartShipping = this.Cart.GetShipping();
			var model = new TwFixedPurchaseInvoiceModel()
			{
				FixedPurchaseId = fixedPurchaseShippingModel.FixedPurchaseId,
				FixedPurchaseShippingNo = fixedPurchaseShippingModel.FixedPurchaseShippingNo,
				TwUniformInvoice = StringUtility.ToEmpty(cartShipping.UniformInvoiceType),
				TwUniformInvoiceOption1 = StringUtility.ToEmpty(cartShipping.UniformInvoiceOption1),
				TwUniformInvoiceOption2 = StringUtility.ToEmpty(cartShipping.UniformInvoiceOption2),
				TwCarryType = StringUtility.ToEmpty(cartShipping.CarryType),
				TwCarryTypeOption = StringUtility.ToEmpty(cartShipping.CarryTypeOptionValue),
			};

			return model;
		}
		#endregion

		#region プロパティ
		/// <summary>注文情報</summary>
		private Hashtable Order { get; set; }
		/// <summary>カート情報</summary>
		private CartObject Cart { get; set; }
		/// <summary>最終更新者</summary>
		private string LastChanged { get; set; }
		#endregion
	}
}
