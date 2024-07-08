/*
=========================================================================================================
  Module      : クレジットカード入力ページ基底クラス(UserCreditCardPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Order;
using w2.Domain.User;

/// <summary>
/// UserCreditCardPage の概要の説明です
/// </summary>
public class UserCreditCardPage : OrderPage
{
	#region WrappedControl
	/// <summary>注文ID</summary>
	public WrappedLiteral WlOrderId { get { return GetWrappedControl<WrappedLiteral>("lOrderId"); } }
	/// <summary>注文総合計</summary>
	public WrappedLiteral WlOrderPriceTotal { get { return GetWrappedControl<WrappedLiteral>("lOrderPriceTotal"); } }
	/// <summary>注文者名</summary>
	public WrappedLiteral WlOrderOwnerOwnerName { get { return GetWrappedControl<WrappedLiteral>("lOrderOwnerOwnerName"); } }
	/// <summary>注文者名かな</summary>
	public WrappedLiteral WlOrderOwnerOwnerNameKana { get { return GetWrappedControl<WrappedLiteral>("lOrderOwnerOwnerNameKana"); } }
	/// <summary>注文者電話番号1</summary>
	public WrappedLiteral WlOrderOwnerOwnerTel1 { get { return GetWrappedControl<WrappedLiteral>("lOrderOwnerOwnerTel1"); } }
	#endregion

	/// <summary>処理モード種別</summary>
	public enum ProcessModeType
	{
		/// <summary>ユーザークレジットカード登録</summary>
		UserCreditCartdRegister,
		/// <summary>注文クレジットカード与信</summary>
		OrderCreditCardAuth
	}

	/// <summary>
	/// 利用可能チェック
	/// <param name="actionStatus">アクションステータス</param>
	/// </summary>
	protected void CheckUsable(string actionStatus)
	{
		if (this.ProcessMode == ProcessModeType.UserCreditCartdRegister)
		{
			// ユーザー存在チェック
			if (new UserService().Get(this.UserId) == null)
			{
				this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_DATA);
				this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
			// クレジットカード登録可能チェック
			if ((OrderCommon.GetCreditCardRegistable((this.LoginOperatorId != null), this.UserId) == false) && (actionStatus == Constants.ACTION_STATUS_INSERT))
			{
				this.Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERCREDITCARD_MAX_REGIST);
				this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}
	}

	/// <summary>
	/// 注文情報表示
	/// </summary>
	/// <returns>注文モデル</returns>
	protected OrderModel DisplayOrderInfo()
	{
		var order = new OrderService().Get(this.OrderId);
		this.WlOrderId.Text = WebSanitizer.HtmlEncode(order.OrderId);
		this.WlOrderPriceTotal.Text = WebSanitizer.HtmlEncode(order.OrderPriceTotal.ToPriceString(true));
		this.WlOrderOwnerOwnerName.Text = WebSanitizer.HtmlEncode(order.Owner.OwnerName);
		this.WlOrderOwnerOwnerNameKana.Text = WebSanitizer.HtmlEncode(order.Owner.OwnerNameKana);
		this.WlOrderOwnerOwnerTel1.Text = WebSanitizer.HtmlEncode(order.Owner.OwnerTel1);
		return order;
	}

	/// <summary>ユーザーID</summary>
	protected string UserId { get { return Request[Constants.REQUEST_KEY_USER_ID]; } }
	/// <summary>注文ID（ゼウスタブレット決済などで利用）</summary>
	protected string OrderId { get { return Request[Constants.REQUEST_KEY_ORDER_ID]; } }
	/// <summary>処理モード</summary>
	protected ProcessModeType ProcessMode
	{
		get
		{
			return string.IsNullOrEmpty(this.OrderId)
				? ProcessModeType.UserCreditCartdRegister
				: ProcessModeType.OrderCreditCardAuth;
		}
	}
}