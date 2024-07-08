/*
=========================================================================================================
  Module      : 管理画面注文登録向け注文登録クラス(OrderRegisterManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common;
using w2.App.Common.Order;
using w2.App.Common.Order.Register;
using w2.Domain.MailTemplate;
using w2.Domain.Order;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

/// <summary>
/// 管理画面注文登録向け注文登録クラス
/// </summary>
public class OrderRegisterManager : OrderRegisterBase
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="isUser">ユーザーか（ポイント付与判断など）</param>
	/// <param name="lastCahnged">DB最終更新者</param>
	public OrderRegisterManager(bool isUser, string lastCahnged)
		: base(ExecTypes.CommerceManager, isUser, lastCahnged)
	{
	}

	/// <summary>
	/// 外部決済かどうかチェック
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="cart">カート</param>
	/// <returns>外部決済か</returns>
	protected override bool CheckExternalPayment(Hashtable order, CartObject cart)
	{
		return false;
	}

	/// <summary>
	/// 注文完了時の処理
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="cart">カート</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>アラート文言</returns>
	public override string OrderCompleteProcesses(Hashtable order, CartObject cart, UpdateHistoryAction updateHistoryAction)
	{
		try
		{
			// 注文同梱で既存の注文台帳を更新する場合スキップ(親注文定期購購入なしでかつ子注文定期購入あり)
			// 頒布会を含む場合、同コース同士の同梱であればスキップ
			if (cart.HasFixedPurchase
				&& (string.IsNullOrEmpty(cart.OrderCombineParentOrderId)
					|| cart.IsRegisterFixedPurchaseWhenOrderCombine))
			{
				this.TransactionName = "4-1.定期購入ステータス更新処理";
				// 仮登録の定期台帳を更新（更新履歴とともに）
				UpdateFixedPurchaseStatusTempToNormal(order, UpdateHistoryAction.Insert);
			}

			this.TransactionName = "4-2.注文系その他メモ更新";
			UpdateOtherOrderMemos(order, UpdateHistoryAction.DoNotInsert);

			this.TransactionName = "4-3.ユーザーUPDATE処理（注文者情報以外の項目更新）";
			UpdateUserInfoFromOrderRegist(order, UpdateHistoryAction.DoNotInsert);

			this.TransactionName = "4-4.配送伝票番号更新";
			UpdateOrderShippingCheckNo(order, cart, UpdateHistoryAction.DoNotInsert);

			this.TransactionName = "4-5クレジット登録確定処理";
			UpdateUserCreditCard(order, cart, this.IsUser, UpdateHistoryAction.DoNotInsert);

			if ((bool)order["update_user_flg"])
			{
				this.TransactionName = "4-6.ユーザー情報に反映処理";
				UpdateUserFromOrderOwner(cart, UpdateHistoryAction.DoNotInsert);
			}

			if ((Constants.REPEATLINE_OPTION_ENABLED == Constants.RepeatLineOption.CooperationAndMessaging)
				&& (string.IsNullOrEmpty(cart.OrderCombineParentOrderId) == false))
			{
				// LINEを送信
				this.TransactionName = "4-7.LINE連携処理";
				var sendLineMessageFlg = MailSendUtility.GetMailTemplateInfo(
					w2.Domain.Constants.CONST_DEFAULT_SHOP_ID,
					w2.App.Common.Constants.CONST_MAIL_ID_ORDER_COMPLETE).LineUseFlg;
				if (sendLineMessageFlg == MailTemplateModel.LINE_USE_FLG_ON) SendOrderCompleteToLine(order, cart);
			}

			if (Constants.TWINVOICE_ECPAY_ENABLED)
			{
				var errorMessage = StringUtility.ToEmpty(order[OrderCommon.ECPAY_INVOICE_API_MESSAGE]);

				if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertAllForOrder(
					(string)order[Constants.FIELD_ORDER_ORDER_ID],
					(string)order[Constants.FIELD_ORDER_LAST_CHANGED]);
			}
		}
		catch (Exception ex)
		{
			throw new Exception(this.TransactionName + " でエラーが発生しました。", ex);
		}
		return "";
	}

	/// <summary>
	/// 注文完了後の処理（セッションを利用するもの）
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="cart">カート情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	public override void AfterOrderCompleteProcesses(
		Hashtable order,
		CartObject cart,
		UpdateHistoryAction updateHistoryAction)
	{
		// 更新履歴登録
		if (updateHistoryAction == UpdateHistoryAction.Insert)
		{
			new UpdateHistoryService().InsertAllForOrder((string)order[Constants.FIELD_ORDER_ORDER_ID], this.LastChanged);

			//頒布会商品の次回配送商品の更新
			UpdateNextSubscriptionBoxProduct(cart, order);
		}
	}

	/// <summary>
	/// 注文完了スキップ時の処理
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="cart">カート情報</param>
	public override void SkipOrderCompleteProcesses(Hashtable order, CartObject cart)
	{
	}

	/// <summary>
	/// 注文系その他のメモ更新
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	private void UpdateOtherOrderMemos(Hashtable order, UpdateHistoryAction updateHistoryAction)
	{
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			using (var statement = new SqlStatement("OrderRegist", "UpdateOrderMemos"))
			{
				var input = new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_ID, order[Constants.FIELD_ORDER_ORDER_ID] },
					{ Constants.FIELD_ORDER_PAYMENT_MEMO, order[Constants.FIELD_ORDER_PAYMENT_MEMO] },
					{ Constants.FIELD_ORDER_MANAGEMENT_MEMO, order[Constants.FIELD_ORDER_MANAGEMENT_MEMO] },
					{ Constants.FIELD_ORDER_RELATION_MEMO, order[Constants.FIELD_ORDER_RELATION_MEMO] },
					{ Constants.FIELD_USER_LAST_CHANGED, this.LastChanged },
				};
				var updated = statement.ExecStatement(accessor, input);
			if (updated < 1) throw new Exception("更新0件です");
		}
			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder((string)order[Constants.FIELD_ORDER_ORDER_ID], this.LastChanged, accessor);
			}

			accessor.CommitTransaction();
		}
	}

	/// <summary>
	/// ユーザー情報更新処理（注文者情報以外の項目更新）
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	private void UpdateUserInfoFromOrderRegist(Hashtable order, UpdateHistoryAction updateHistoryAction)
	{
		// 完了処理だが注文者反映部分なのでここは更新履歴を更新する
		var result = new UserService().UpdateUserMemoAndUserManagementLevelId(
			(string)order[Constants.FIELD_ORDER_USER_ID],
			(string)order[Constants.FIELD_USER_USER_MEMO],
			(string)order[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID],
			this.LastChanged,
			updateHistoryAction);
		if (result == false) throw new Exception("更新0件です");
		}

	/// <summary>
	/// 配送伝票番号更新処理
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="cart">カート</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	private void UpdateOrderShippingCheckNo(Hashtable order, CartObject cart, UpdateHistoryAction updateHistoryAction)
	{
		int updated = new OrderService().UpdateOrderShippingCheckNo(
			(string)order[Constants.FIELD_ORDER_ORDER_ID],
			1,
			cart.Shippings[0].ShippingCheckNo,
			this.LastChanged,
			updateHistoryAction);
		if (updated < 1) throw new Exception("更新0件です");
	}

	/// <summary>
	/// 注文者情報をユーザー情報に更新処理
	/// </summary>	
	/// <param name="cart">カート</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	private void UpdateUserFromOrderOwner(CartObject cart, UpdateHistoryAction updateHistoryAction)
	{
		var userService = new UserService();
		var user = userService.Get(cart.OrderUserId);

		user.Name = cart.Owner.Name;
		user.Name1 = cart.Owner.Name1;
		user.Name2 = cart.Owner.Name2;
		user.NameKana = cart.Owner.NameKana;
		user.NameKana1 = cart.Owner.NameKana1;
		user.NameKana2 = cart.Owner.NameKana2;
		user.MailAddr = cart.Owner.MailAddr;
		user.MailAddr2 = cart.Owner.MailAddr2;
		user.Zip = cart.Owner.Zip;
		user.Zip1 = cart.Owner.Zip1;
		user.Zip2 = cart.Owner.Zip2;
		user.Addr = cart.Owner.ConcatenateAddress();
		user.Addr1 = cart.Owner.Addr1;
		user.Addr2 = cart.Owner.Addr2;
		user.Addr3 = cart.Owner.Addr3;
		user.Addr4 = cart.Owner.Addr4;
		user.CompanyName = cart.Owner.CompanyName;
		user.CompanyPostName = cart.Owner.CompanyPostName;
		user.Tel1 = cart.Owner.Tel1;
		user.Tel1_1 = cart.Owner.Tel1_1;
		user.Tel1_2 = cart.Owner.Tel1_2;
		user.Tel1_3 = cart.Owner.Tel1_3;
		user.Tel2 = cart.Owner.Tel2;
		user.Tel2_1 = cart.Owner.Tel2_1;
		user.Tel2_2 = cart.Owner.Tel2_2;
		user.Tel2_3 = cart.Owner.Tel2_3;
		user.Sex = cart.Owner.Sex;
		user.Birth = cart.Owner.Birth;
		user.BirthYear = cart.Owner.BirthYear;
		user.BirthMonth = cart.Owner.BirthMonth;
		user.BirthDay = cart.Owner.BirthDay;
		if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED && UserService.IsPcSiteOrOfflineUser(cart.Owner.OwnerKbn))
		{
			user.LoginId = cart.Owner.MailAddr;
		}
		user.EasyRegisterFlg = Constants.FLG_USER_EASY_REGISTER_FLG_NORMAL;
		user.LastChanged = this.LastChanged;
		
		if(Constants.GLOBAL_OPTION_ENABLE)
		{
			user.AccessCountryIsoCode = cart.Owner.AccessCountryIsoCode;
			user.DispLanguageCode = cart.Owner.DispLanguageCode;
			user.DispLanguageLocaleId = cart.Owner.DispLanguageLocaleId;
			user.DispCurrencyCode = cart.Owner.DispCurrencyCode;
			user.DispCurrencyLocaleId = cart.Owner.DispCurrencyLocaleId;
			user.AddrCountryIsoCode = cart.Owner.AddrCountryIsoCode;
			user.AddrCountryName = cart.Owner.AddrCountryName;
			user.Addr5 = cart.Owner.Addr5;
		}

		var updated = userService.UpdateWithUserExtend(user, updateHistoryAction);
		if (updated == false) throw new Exception("更新0件です");
		}
	}
