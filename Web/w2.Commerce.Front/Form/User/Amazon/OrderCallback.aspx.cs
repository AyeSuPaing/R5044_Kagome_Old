/*
=========================================================================================================
  Module      : Amazon 注文コールバック画面(OrderCallback.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Amazon.Util;
using w2.App.Common.Order.OrderCombine;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// Amazon 注文者決定コールバック画面
/// </summary>
public partial class Form_User_Amazon_OrderCallback : AmazonOrderPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (IsPostBack)
		{
			// 初期処理
			InitPage();

			// AmazonユーザーIDからユーザー情報取得
			var w2User = AmazonUtil.GetUserByAmazonUserId(this.AmazonModel.UserId);

			var nextPath = string.Empty;

			// 注文同梱対象がある場合、遷移先を注文同梱選択画面にする
			if ((Constants.ORDER_COMBINE_OPTION_ENABLED)
				&& (w2User != null)
				&& (OrderCombineUtility.ExistCombinableOrder(w2User.UserId, this.CartList, true, true))
				&& (this.CartList.Items[0].HasFixedPurchase == false))
			{
				nextPath = Constants.PAGE_FRONT_ORDER_COMBINE_SELECT_LIST;
			}
			else
			{
				nextPath = Constants.PAGE_FRONT_ORDER_AMAZON_PAYMENT_INPUT;
			}

			// 通常商品→定期商品(あるいは逆)のレコメンドで2回目OrderCallbackの場合、確認画面に戻る
			if (SessionManager.IsChangedAmazonPayForFixedOrNormal)
			{
				SessionManager.IsChangedAmazonPayForFixedOrNormal = false;
				SessionManager.IsAmazonPayGotRecurringConsent = SessionManager.IsAmazonPayGotRecurringConsent ? false : true;
				nextPath = Constants.PAGE_FRONT_ORDER_CONFIRM;
			}

			// 画面遷移の正当性チェックのため遷移先ページURLを設定
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = nextPath;

			// メールアドレスが既にユーザー登録しているが、Amazon未連携している場合、自動Amazon連携をする
			if (w2User == null) w2User = UpdateCooperation();

			// Amazon未連携ユーザーかログイン済みの場合
			if ((w2User == null) || this.IsLoggedIn)
			{
				Response.Redirect(Constants.PATH_ROOT + nextPath);
			}

			// Amazon連携済ユーザーかつ未ログイン状態の場合カートクリアし、ログイン処理してから遷移
			var input = new Hashtable
			{
				{Constants.FIELD_USER_USER_ID, w2User.UserId},
			};
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("Cart", "DeleteUserCart"))
			{
				sqlStatement.ExecStatementWithOC(sqlAccessor, input);
			}
			SetLoginUserData(w2User, UpdateHistoryAction.DoNotInsert);
			ExecLoginSuccessActionAndGoNextInner(w2User, Constants.PATH_ROOT + nextPath, UpdateHistoryAction.Insert);
		}
	}
}
