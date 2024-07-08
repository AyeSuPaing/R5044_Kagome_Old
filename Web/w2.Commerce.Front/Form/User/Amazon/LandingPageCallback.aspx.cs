/*
=========================================================================================================
  Module      : Amazon ランディングページコールバック画面(LandingPageCallback.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Util;
using w2.App.Common.AmazonCv2;
using w2.Common.Web;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// Amazon ランディングページコールバック画面
/// </summary>
public partial class Form_User_Amazon_LandingPageCallback : AmazonOrderPage
{
	/// <summary>
	/// ぺージロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (IsPostBack)
		{
			// 初期処理
			InitPage();

			var state = Request[AmazonConstants.REQUEST_KEY_AMAZON_STATE];

			if (Constants.AMAZON_PAYMENT_CV2_ENABLED)
			{
				state = new UrlCreator(Request[AmazonConstants.REQUEST_KEY_AMAZON_STATE])
					.AddParam(
						AmazonCv2Constants.REQUEST_KEY_AMAZON_CHECKOUT_SESSION_ID,
						Request[AmazonCv2Constants.REQUEST_KEY_AMAZON_CHECKOUT_SESSION_ID])
					.CreateUrl();

				state = string.Format("{0}#CartList", state);
			}

			// AmazonユーザーIDからユーザー情報取得
			var w2User = AmazonUtil.GetUserByAmazonUserId(this.AmazonModel.UserId);

			// メールアドレスが既にユーザー登録しているが、Amazon未連携している場合、自動Amazon連携をする
			if (w2User == null) w2User = UpdateCooperation();

			// Amazon未連携ユーザーか既にログイン済みの場合
			if ((w2User == null) || this.IsLoggedIn)
			{
				Response.Redirect(state);
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

			// ログイン処理
			SetLoginUserData(w2User, UpdateHistoryAction.DoNotInsert);
			ExecLoginSuccessActionAndGoNextInner(w2User, state, UpdateHistoryAction.Insert);
		}
	}
}
