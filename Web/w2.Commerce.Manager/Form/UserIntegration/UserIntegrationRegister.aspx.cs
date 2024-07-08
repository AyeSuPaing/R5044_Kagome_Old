/*
 =========================================================================================================
  Module      : ユーザー統合登録ページ処理(UserIntegrationRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.CrossPoint.User;
using w2.App.Common.User;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.FixedPurchase;
using w2.Domain.MallCooperationSetting;
using w2.Domain.Order;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.UserIntegration;

public partial class Form_UserIntegration_UserIntegrationRegister : UserIntegrationPage
{
	/// <summary>Session key: Is async</summary>
	private const string SESSION_KEY_IS_ASYNC = "w2Manager_user_integration_is_async";
	/// <summary>Session key: Error message</summary>
	private const string SESSION_KEY_ERROR_MESSAGE = "w2Manager_user_integration_error_message";

	#region +列挙対
	/// <summary>並び替え列挙対</summary>
	protected enum SortKbn
	{
		/// <summary>ユーザーID/昇順</summary>
		UserIdASC,
		/// <summary>ユーザーID/降順</summary>
		UserIdDESC,
		/// <summary>作成日/昇順</summary>
		DateCreatedASC,
		/// <summary>作成日/降順</summary>
		DateCreatedDESC,
		/// <summary>更新日/昇順</summary>
		DateChangedASC,
		/// <summary>更新日/降順</summary>
		DateChangedDESC,
		/// <summary>最終ログイン日時/昇順</summary>
		DateLastLoggedinASC,
		/// <summary>最終ログイン日時/降順</summary>
		DateLastLoggedinDESC
	}
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
		if (!IsPostBack)
		{
			// パラメータが不正？
			if ((this.ActionStatus != Constants.ACTION_STATUS_INSERT)
				&& (this.ActionStatus != Constants.ACTION_STATUS_UPDATE)
				&& (this.ActionStatus != Constants.ACTION_STATUS_COMPLETE))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// 更新 or 完了？
			if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
				|| this.ActionStatus == Constants.ACTION_STATUS_COMPLETE)
			{
				// ユーザー統合Noパラメータなし？
				var isExsit = true;
				if (string.IsNullOrEmpty(this.RequestUserIntegrationNo)) isExsit = false;

				// ユーザー統合情報取得
				if (isExsit)
				{
					var userIntegration = new UserIntegrationService().GetContainer(int.Parse(this.RequestUserIntegrationNo));
					if (userIntegration != null)
					{
						this.UserIntegration = new UserIntegrationInput(userIntegration);
					}
					else
					{
						isExsit = false;
					}
				}

				// データが存在しない場合はエラーページへ遷移
				if (isExsit == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
			}
			// 新規登録？
			else
			{
				this.UserIntegration = new UserIntegrationInput();
			}

			// モール連携設定情報をセット
			this.MallCooperationSettingList = new MallCooperationSettingService().GetAll(this.LoginOperatorShopId);

			// 初期化
			Initialize();

			// 画面表示
			SetValues();
		}
		else
		{
			divComp.Visible = false;
		}
    }

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnToList_Click(object sender, EventArgs e)
	{
		Response.Redirect(this.SearchInfo.CreateUserIntegrationListUrl());
	}

	/// <summary>
	/// 解除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnNone_Click(object sender, EventArgs e)
	{
		// 入力チェック
		if (CheckUserIntegration() == false) return;

		var representativeUser = this.UserIntegration.Users
			.FirstOrDefault(user => user.IsOnRepresentativeFlg);

		var orderCount = DomainFacade.Instance.OrderService
			.GetOrdersByUserId(representativeUser.UserId).Length;

		if (orderCount >= Constants.ORDER_QUANTITY_TO_EXECUTE_ASYNC_CANCEL_USER_INTEGRATE)
		{
			var userIntegrationNo = this.RequestUserIntegrationNo;
			var actionStatus = this.ActionStatus;
			var loginOperatorDeptId = this.LoginOperatorDeptId;
			var loginOperatorName = this.LoginOperatorName;
			this.IsAsync = true;

			Task.Run(() =>
			{
				try
				{
					this.ErrorMessage = CancelUserIntegrationProccess(
						actionStatus,
						loginOperatorDeptId,
						loginOperatorName);
				}
				catch
				{
					this.ErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SYSTEM_ERROR);
				}

				SendCancelUserIntegrateAsyncMail(userIntegrationNo, this.ErrorMessage);
			});
		}
		else
		{
			this.IsAsync = false;
			this.ErrorMessage = CancelUserIntegrationProccess(
				this.ActionStatus,
				this.LoginOperatorDeptId,
				this.LoginOperatorName);
		}

		// 再表示
		Response.Redirect(CreateUserIntegrationRegisterlUrl(this.UserIntegration.UserIntegrationNo, Constants.ACTION_STATUS_COMPLETE));
	}

	/// <summary>
	/// 統合解除できるかチェック
	/// </summary>
	/// <param name="representativeUser">代表ユーザー</param>
	/// <returns>ユーザーが登録していないクレジットカードリスト</returns>
	private List<string> CheckCanCancelUserIntegration(UserIntegrationUserModel representativeUser)
	{
		var creditList = new List<string>();
		var orderService = new OrderService();
		var fixedPurchaseService = new FixedPurchaseService();
		var creditService = new UserCreditCardService();
		var representativeOrders = orderService.GetOrdersByUserId(representativeUser.UserId)
			.Where(o => (o.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (o.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED));
		var representativeFixedPurchases = fixedPurchaseService.GetFixedPurchasesByUserId(representativeUser.UserId)
			.Where(o => (o.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT));
		var representativeCard = creditService.GetByUserId(representativeUser.UserId);
		// 非代表ユーザーの注文が非代表ユーザーのクレジットカード以外になっていないか確認
		foreach (var user in this.UserIntegration.Users.Where(u => (u.IsOnRepresentativeFlg == false)))
		{
			var notRepresentativeCard = creditService.GetByUserId(user.UserId);

			foreach (var orderHistory in user.Histories.Where(h => (h.TableName == Constants.TABLE_ORDER)))
			{
				var relatedOrders = orderService.GetRelatedOrders(orderHistory.PrimaryKey1)
					.Where(o => (o.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (o.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED));

				foreach (var relatedOrder in relatedOrders)
				{
					if (relatedOrder.CreditBranchNo != null)
					{
						var currentOrderCreditCard = creditService.Get(representativeUser.UserId, (int)relatedOrder.CreditBranchNo);
						if (notRepresentativeCard.Any(c => (c.CooperationId == currentOrderCreditCard.CooperationId)) == false)
						{
							creditList.Add(CreateErrorOrderLink(orderHistory.PrimaryKey1, orderHistory.UserId));
						}
					}

					// 代表ユーザーの注文から非代表ユーザーの注文を消す。
					representativeOrders = representativeOrders
						.Where(r => (r.OrderId != relatedOrder.OrderId)).ToArray();
				}
			}

			// 非代表ユーザーの定期注文が非代表ユーザーのクレジットカード以外になっていないか確認
			foreach (var fixedPurchaseHistory in user.Histories.Where(h => h.TableName == Constants.TABLE_FIXEDPURCHASE))
			{
				var fp = fixedPurchaseService.Get(fixedPurchaseHistory.PrimaryKey1);
				if ((fp.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (notRepresentativeCard.Any(
						c => (fp.CreditBranchNo != null) && (c.CooperationId == creditService.Get(representativeUser.UserId, (int)fp.CreditBranchNo).CooperationId)) == false))
				{
					creditList.Add(CreateErrorFixedPurchaseLink(fixedPurchaseHistory.PrimaryKey1, fixedPurchaseHistory.UserId));
				}

				// 代表ユーザーの定期台帳から非代表ユーザーの定期台帳を消す。
				representativeFixedPurchases = representativeFixedPurchases
					.Where(r => (r.FixedPurchaseId != fp.FixedPurchaseId)).ToArray();
			}

			// 代表のクレジットカードから非代表のクレジットカードを消す
			foreach (var credit in user.Histories.Where(h => (h.TableName == Constants.TABLE_USERCREDITCARD)))
			{
				representativeCard = representativeCard.Where(
					r => credit.PrimaryKey2 != r.BranchNo.ToString()).ToArray();
			}
		}

		// 代表ユーザーの注文が代表ユーザーのクレジットカード以外になっていないか確認
		foreach (var representativeOrder in representativeOrders)
		{
			if (representativeOrder.CreditBranchNo != null)
			{
				var currentOrderCreditCard = creditService.Get(representativeUser.UserId, (int)representativeOrder.CreditBranchNo);
				if (representativeCard.Any(c => (c.BranchNo == currentOrderCreditCard.BranchNo)) == false)
				{
					creditList.Add(CreateErrorOrderLink(representativeOrder.OrderId, representativeUser.UserId));
				}
			}
		}

		// 代表ユーザーの定期台帳が代表ユーザーのクレジットカード以外になっていないか確認
		foreach (var representativeFixedPurchase in representativeFixedPurchases)
		{
			if (representativeFixedPurchase.CreditBranchNo != null)
			{
				var currentFixedPurchaseCreditCard = creditService.Get(representativeUser.UserId, (int)representativeFixedPurchase.CreditBranchNo);
				if ((representativeFixedPurchase.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (representativeCard.Any(c => (c.BranchNo == currentFixedPurchaseCreditCard.BranchNo)) == false))
				{
					creditList.Add(CreateErrorFixedPurchaseLink(representativeFixedPurchase.FixedPurchaseId, representativeUser.UserId));
				}
			}
		}

		return creditList;
	}

	/// <summary>
	/// 注文用のエラーリンクを作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="userId">ユーザーID</param>
	/// <returns>エラーリンク</returns>
	private string CreateErrorOrderLink(string orderId, string userId)
	{
		var errormessage = string.Format(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERINTEGRATION_CREDITCARD_ERROR), 
			"<a href=\"javascript:open_window('"
			+ WebSanitizer.UrlAttrHtmlEncode(
				OrderPage.CreateOrderDetailUrl(
					orderId,
					true,
					true,
					(string)this.Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_POPUP_PARENT_NAME]))
			+ "','order','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');\">" + orderId + "</a>",
			"<a href=\"javascript:open_window('" + WebSanitizer.UrlAttrHtmlEncode(CreateUserDetailUrl(userId))
			+ "','user','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');\">" + userId + "</a>");

		return errormessage;
	}

	/// <summary>
	/// 定期用のエラーリンクを作成
	/// </summary>
	/// <param name="fixedPurchaseId">定期台帳ID</param>
	/// <param name="userId">ユーザーID</param>
	/// <returns>エラーリンク</returns>
	private string CreateErrorFixedPurchaseLink(string fixedPurchaseId, string userId)
	{
		var errormessage = string.Format(
			WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERINTEGRATION_CREDITCARD_ERROR),
			"<a href=\"javascript:open_window('"
			+ WebSanitizer.UrlAttrHtmlEncode(FixedPurchasePage.CreateFixedPurchaseDetailUrl(fixedPurchaseId, true))
			+ "','order','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');\">" + fixedPurchaseId
			+ "</a>",
			"<a href=\"javascript:open_window('" + WebSanitizer.UrlAttrHtmlEncode(CreateUserDetailUrl(userId))
			+ "','user','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');\">" + userId + "</a>");

		return errormessage;
	}

	/// <summary>
	/// 保留するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSuspend_Click(object sender, EventArgs e)
	{
		// 入力チェック
		if (CheckUserIntegration() == false) return;

		// ステータスを保留に更新
		var model = this.UserIntegration.CreateModel();
		new UserIntegrationService()
			.SuspendUserIntegration(
				model,
				(this.ActionStatus == Constants.ACTION_STATUS_INSERT),
				this.LoginOperatorName);

		// 再表示
		Response.Redirect(CreateUserIntegrationRegisterlUrl(model.UserIntegrationNo.ToString(), Constants.ACTION_STATUS_COMPLETE));
	}

	/// <summary>
	/// 統合するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDone_Click(object sender, EventArgs e)
	{
		// 入力チェック
		if (CheckUserIntegration() == false) return;

		// 最新ユーザー情報取得
		var userList = this.UserIntegration.Users.Select(u => new UserService().Get(u.UserId));

		// 会員が含まれている AND ゲストユーザーが代表の場合、統合できないのでエラーを表示
		var userMemberList = userList.Where(u => u.IsMember);
		if (userMemberList.Any())
		{
			var representativeUser = this.UserIntegration.Users.FirstOrDefault(u => u.IsOnRepresentativeFlg);
			if (representativeUser.IsMember == false)
			{
				trUserIntegrationErrorMessagesTitle.Visible =
					trUserIntegrationErrorMessages.Visible = true;
				lbUserIntegrationErrorMessages.Text =
					string.Format(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERINTEGRATION_USER_ERROR), string.Join(",", userMemberList.Select(u => u.UserId)), representativeUser.UserId);
				return;
			}
		}

		// 既に統合済みのユーザー（非代表）が存在する場合、統合できないのでエラーを表示
		var userIntegratedList = userList.Where(u => u.IsIntegrated);
		if (userIntegratedList.Any())
		{
			trUserIntegrationErrorMessagesTitle.Visible =
				trUserIntegrationErrorMessages.Visible = true;
			lbUserIntegrationErrorMessages.Text =
				string.Format(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERINTEGRATION_INTEGRATED_ERROR), string.Join(",", userIntegratedList.Select(u => u.UserId)));
			return;
		}

		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			var representativeUserId = this.UserIntegration.Users.FirstOrDefault(user => user.IsOnRepresentativeFlg).UserId;
			var notRepresentativeUsers = this.UserIntegration.Users.Where(user => (user.IsOnRepresentativeFlg == false));

			foreach (var user in notRepresentativeUsers)
			{
				var userExtend = DomainFacade.Instance.UserService.GetUserExtend(user.UserId);

				if (userExtend.UserExtendDataText.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_NO)
					&& userExtend.UserExtendDataText.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN))
				{
					var result = new CrossPointUserApiService().Merge(
						representativeUserId,
						userExtend.UserExtendDataValue.CrossPointShopCardNo,
						userExtend.UserExtendDataValue.CrossPointShopCardPin);

					if (result.IsSuccess == false)
					{
						Session[Constants.SESSION_KEY_ERROR_MSG] = MessageManager.GetMessages(
							w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
					}

					var userModel = DomainFacade.Instance.UserService.Get(representativeUserId);

					// Adjust point and member rank by Cross Point api
					UserUtility.AdjustPointAndMemberRankByCrossPointApi(userModel);
				}
			}
		}

		// ステータスを統合済みに更新
		var model = this.UserIntegration.CreateModel();
		new UserIntegrationService()
			.ExecuteUserIntegration(
				model,
				(this.ActionStatus == Constants.ACTION_STATUS_INSERT),
				this.LoginOperatorDeptId,
				this.LoginOperatorName,
				Constants.USEREXTENDSETTING_SYSTEM_USED_ITEMS,
				(Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus),
				(Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE
					== Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_USER_ID));

		// 再表示
		Response.Redirect(CreateUserIntegrationRegisterlUrl(model.UserIntegrationNo.ToString(), Constants.ACTION_STATUS_COMPLETE));
	}

	/// <summary>
	/// 除外するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnExcluded_Click(object sender, EventArgs e)
	{
		// 入力チェック
		if (CheckUserIntegration() == false) return;

		// ステータスを除外に更新
		var model = this.UserIntegration.CreateModel();
		new UserIntegrationService()
			.ExcludedUserIntegration(
				model,
				(this.ActionStatus == Constants.ACTION_STATUS_INSERT),
				this.LoginOperatorName);

		// 再表示
		Response.Redirect(CreateUserIntegrationRegisterlUrl(model.UserIntegrationNo.ToString(), Constants.ACTION_STATUS_COMPLETE));
	}

	/// <summary>
	/// 代表選択用ボタン（非表示） クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRepresentativeFlg_Click(object sender, EventArgs e)
	{
		// 代表フラグセット
		SetRepresentativeFlg();

		// 画面に値をセット
		SetValues();
	}

	/// <summary>
	/// 並び順プルダウン選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlSort_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		// 画面に値をセット
		SetValues();
	}

	/// <summary>
	/// ユーザー追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddUser_Click(object sender, EventArgs e)
	{
		// 既に登録済みの場合は、処理を抜ける
		var userId = hfUserId.Value;
		if (this.UserIntegration.Users.Any(u => u.UserId == userId)) return;

		// ユーザー統合ユーザー情報作成
		var user = new UserService().Get(userId);
		var mallName = "";
		if (this.MallCooperationSettingList.Any()) mallName = this.MallCooperationSettingList.First().MallName;
		var input = new UserIntegrationUserInput()
		{
			UserIntegrationNo = this.UserIntegration.UserIntegrationNo,
			UserId = userId,
			RepresentativeFlg = Constants.FLG_USERINTEGRATIONUSER_REPRESENTATIVE_FLG_OFF,
			DateCreated = DateTime.Now.ToString(),
			DateChanged = DateTime.Now.ToString(),
			LastChanged = this.LoginOperatorName,
			UserKbn = user.UserKbn,
			MallId = user.MallId,
			MallName = mallName,
			Name = user.Name,
			Name1 = user.Name1,
			Name2 = user.Name2,
			NameKana = user.NameKana,
			NameKana1 = user.NameKana1,
			NameKana2 = user.NameKana2,
			MailAddr = user.MailAddr,
			Zip = user.Zip,
			Addr = user.Addr,
			Addr1 = user.Addr1,
			Addr2 = user.Addr2,
			Addr3 = user.Addr3,
			Addr4 = user.Addr4,
			Addr5 = user.Addr5,
			AddrCountryName = user.AddrCountryName,
			Tel1 = user.Tel1,
			Tel2 = user.Tel2,
			Sex = user.Sex,
			Birth = user.Birth,
			UserDateCreated = user.DateCreated,
			UserDateChanged = user.DateChanged,
			DateLastLoggedin = user.DateLastLoggedin,
			HasCreditCards = user.HasCreditCards
		};

		// ユーザー統合ユーザー情報追加
		var users = this.UserIntegration.Users.ToList();
		users.Add(input);
		this.UserIntegration.Users = users.ToArray();

		// 代表フラグセット
		SetRepresentativeFlg();

		// 画面に値をセット
		SetValues();
	}

	/// <summary>
	/// ユーザ情報リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rUserList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		// ユーザー統合ユーザー情報削除?
		if (e.CommandName == "delete")
		{
			// ユーザー統合ユーザー情報削除
			var users = this.UserIntegration.Users.ToList();
			users.Remove(users[int.Parse(e.CommandArgument.ToString())]);
			this.UserIntegration.Users = users.ToArray();

			// 代表フラグセット
			SetRepresentativeFlg();

			// 画面に値をセット
			SetValues();
		}
	}	

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// 完了メッセージ
		if (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE)
		{
			divComp.Visible = true;

			lCompMessages.Text = this.IsAsync
				? HtmlSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USER_INTEGRATION_CANCEL_ASYNC))
				: string.IsNullOrEmpty(this.ErrorMessage)
					? WebMessages.GetMessages(
						WebMessages.ERRMSG_MANAGER_USER_INTEGRATION_UPDATE_COMPLETE,
						this.UserIntegration.StatusText)
					: this.ErrorMessage;

			this.IsAsync = false;
		}

		// ボタン制御
		if (this.ActionStatus == Constants.ACTION_STATUS_INSERT)
		{
			btnNoneTop.Visible = btnNoneBottom.Visible = true;
			btnNoneTop.Text = btnNoneBottom.Text =
				//「登録する」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER,
					Constants.VALUETEXT_PARAM_USER_INTEGRATION,
					Constants.VALUETEXT_PARAM_USER_INTEGRATION_REGIST_BUTTON);
			btnSuspendTop.Visible = btnSuspendBottom.Visible = true;
			btnDoneTop.Visible = btnDoneBottom.Visible = true;
			btnExcludedTop.Visible = btnExcludedBottom.Visible = true;
		}
		else
		{
			divDetail.Visible = true;
			btnNoneTop.Visible = btnNoneBottom.Visible = (this.UserIntegration.IsNoneStatus == false);
			if (this.UserIntegration.IsDoneStatus)
			{
				btnNoneTop.Visible = btnNoneBottom.Visible =
					(Constants.CROSS_POINT_OPTION_ENABLED == false);

				btnNoneTop.Text = btnNoneBottom.Text =
					//「統合したユーザーを元に戻す」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_USER,
						Constants.VALUETEXT_PARAM_USER_INTEGRATION,
						Constants.VALUETEXT_PARAM_USER_INTEGRATION_UNDO_BUTTON);
			}
			btnSuspendTop.Visible = btnSuspendBottom.Visible = this.UserIntegration.IsNoneStatus;
			btnDoneTop.Visible = btnDoneBottom.Visible = this.UserIntegration.IsNoneStatus;
			btnExcludedTop.Visible = btnExcludedBottom.Visible = this.UserIntegration.IsNoneStatus;

			// 初期は最終ログイン日時/降順
			ddlSort.SelectedValue = SortKbn.DateLastLoggedinDESC.ToString();
		}
	}

	/// <summary>
	/// 画面に値をセット
	/// </summary>
	private void SetValues()
	{
		// 基本情報
		if (this.ActionStatus != Constants.ACTION_STATUS_INSERT)
		{
			lNo.Text = WebSanitizer.HtmlEncode(this.UserIntegration.UserIntegrationNo);
			lStatus.Text = WebSanitizer.HtmlEncode(this.UserIntegration.StatusText);
			lDateCreated.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					this.UserIntegration.DateCreated,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
			lDateChanged.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					this.UserIntegration.DateChanged,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
			lLastChanged.Text = WebSanitizer.HtmlEncode(this.UserIntegration.LastChanged);
		}

		// 並び順の表示制御（未確定 かつ 2件以上の場合のみ表示）
		ddlSort.Visible = (this.UserIntegration.IsNoneStatus) && (this.UserIntegration.Users.Length >= 2);

		// 並び替え
		SetSortUserList();

		// 代表フラグが全て「代表ではない」場合、先頭データを「代表である」をセット
		if ((this.UserIntegration.Users.Length != 0)
			&& (this.UserIntegration.Users.Any(u => u.IsOnRepresentativeFlg) == false))
		{
			this.UserIntegration.Users[0].RepresentativeFlg = Constants.FLG_USERINTEGRATIONUSER_REPRESENTATIVE_FLG_ON;
		}

		// ユーザー統合ユーザー情報セット
		rUserList.DataSource = this.UserIntegration.Users;
		rUserList.DataBind();

		// ユーザー統合履歴情報セット
		divHistores.Visible = this.UserIntegration.IsDoneStatus;
		if (divHistores.Visible)
		{
			// 代表ユーザーのユーザーIDをセット
			var user = this.UserIntegration.Users.Where(u => u.IsOnRepresentativeFlg).FirstOrDefault();
			lUserId.Text = WebSanitizer.HtmlEncode(user.UserId);

			// 代表以外のユーザー統合ユーザー情報をセット
			rHistores.DataSource = this.UserIntegration.Users.Where(u => u.IsOnRepresentativeFlg == false).ToArray();
			rHistores.DataBind();
		}
	}

	/// <summary>
	/// ユーザー統合ユーザー情報を並び替え
	/// </summary>
	private void SetSortUserList()
	{
		// 未確定以外の場合、代表フラグ降順, ユーザーID降順で並び替え
		if (this.UserIntegration.IsNoneStatus == false)
		{
			this.UserIntegration.Users = this.UserIntegration.Users.OrderByDescending(u => u.RepresentativeFlg).ThenByDescending(u => u.UserId).ToArray();
			return;
		}

		// 選択値に応じて並び替え
		switch ((SortKbn)Enum.Parse(typeof(SortKbn), ddlSort.SelectedValue))
		{
			case SortKbn.UserIdASC:
				this.UserIntegration.Users = this.UserIntegration.Users.OrderBy(u => u.UserId).ToArray();
				break;
			case SortKbn.UserIdDESC:
				this.UserIntegration.Users = this.UserIntegration.Users.OrderByDescending(u => u.UserId).ToArray();
				break;
			case SortKbn.DateCreatedASC:
				this.UserIntegration.Users = this.UserIntegration.Users.OrderBy(u => u.UserDateCreated).ToArray();
				break;
			case SortKbn.DateCreatedDESC:
				this.UserIntegration.Users = this.UserIntegration.Users.OrderByDescending(u => u.UserDateCreated).ToArray();
				break;
			case SortKbn.DateChangedASC:
				this.UserIntegration.Users = this.UserIntegration.Users.OrderBy(u => u.UserDateChanged).ToArray();
				break;
			case SortKbn.DateChangedDESC:
				this.UserIntegration.Users = this.UserIntegration.Users.OrderByDescending(u => u.UserDateChanged).ToArray();
				break;
			case SortKbn.DateLastLoggedinASC:
				this.UserIntegration.Users = this.UserIntegration.Users.OrderBy(u => u.DateLastLoggedin.HasValue ? u.DateLastLoggedin : DateTime.MinValue).ToArray();
				break;
			case SortKbn.DateLastLoggedinDESC:
				this.UserIntegration.Users = this.UserIntegration.Users.OrderByDescending(u => u.DateLastLoggedin.HasValue ? u.DateLastLoggedin : DateTime.MinValue).ToArray();
				break;
		}
	}

	/// <summary>
	/// ユーザー統合ユーザー情報に代表フラグをセット
	/// </summary>
	private void SetRepresentativeFlg()
	{
		// リクエスト：代表フラグが存在しない場合、処理を抜ける
		if (this.RequestRepresentativeFlg == null) return;

		// 代表フラグをセット
		int index = 0;
		foreach (var user in this.UserIntegration.Users)
		{
			user.RepresentativeFlg =
				(this.RequestRepresentativeFlg == index) ? Constants.FLG_USERINTEGRATIONUSER_REPRESENTATIVE_FLG_ON : Constants.FLG_USERINTEGRATIONUSER_REPRESENTATIVE_FLG_OFF;
			index++;
		}
	}

	/// <summary>
	/// ユーザー統合情報入力チェック
	/// </summary>
	/// <returns>正常：True, エラー：False</returns>
	private bool CheckUserIntegration()
	{
		trUserIntegrationErrorMessagesTitle.Visible 
			= trUserIntegrationErrorMessages.Visible = false;
		lbUserIntegrationErrorMessages.Text = "";

		string errorMessage = this.UserIntegration.Validate();
		if (errorMessage.Length != 0)
		{
			// エラーメッセージ表示
			trUserIntegrationErrorMessagesTitle.Visible =
				trUserIntegrationErrorMessages.Visible = true;
			lbUserIntegrationErrorMessages.Text = errorMessage.ToString();

			return false;
		}

		return true;
	}

	/// <summary>
	/// ユーザー情報詳細URL作成
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <returns>ユーザー情報詳細URL</returns>
	protected string CreateUserDetailUrl(string userId)
	{
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_USER_CONFIRM_POPUP);
		url.Append("?").Append(Constants.REQUEST_KEY_USER_ID).Append("=").Append(HttpUtility.UrlEncode(userId));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);
		url.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(Constants.KBN_WINDOW_POPUP);

		return url.ToString();
	}

	/// <summary>
	/// CSメッセージページURL作成
	/// </summary>
	/// <param name="incidentId">インシデントID</param>
	/// <returns>CSメッセージページURL</returns>
	protected string CreateCsMessageUrl(string incidentId)
	{
		return string.Format("{0}?{1}={2}&{3}={4}&{5}={6}&{7}={8}",
					Constants.PATH_ROOT_CS + Constants.PAGE_W2CS_MANAGER_MESSAGE_MESSAGE_INPUT,
					Constants.REQUEST_KEY_MESSAGE_MEDIA_MODE,
					HttpUtility.UrlEncode(Constants.KBN_MESSAGE_MEDIA_MODE_TEL),
					Constants.REQUEST_KEY_MESSAGE_EDIT_MODE,
					HttpUtility.UrlEncode(Constants.KBN_MESSAGE_EDIT_MODE_NEW),
					Constants.REQUEST_KEY_INCIDENT_ID,
					HttpUtility.UrlEncode(incidentId),
					Constants.REQUEST_KEY_MESSAGE_NO,
					1);
	}

	/// <summary>
	/// CSメール送信ログ確認ページURL作成
	/// </summary>
	/// <param name="logNo">ログNo</param>
	/// <returns>CSメール送信ログ確認ページURL</returns>
	protected string CreateCsMailSendLogConfirmUrl(string logNo)
	{
		return string.Format("{0}?{1}={2}",
					Constants.PATH_ROOT_CS + Constants.PAGE_W2CS_MANAGER_MESSAGE_MAILSENDLOG_CONFIRM,
					Constants.REQUEST_KEY_MAILSENDLOG_LOG_NO,
					HttpUtility.UrlEncode(logNo));
	}

	/// <summary>
	/// サイト名取得
	/// </summary>
	/// <param name="user">ユーザー統合ユーザ入力情報</param>
	/// <returns>サイト名（モール名）</returns>
	protected string CreateSiteNameOnly(UserIntegrationUserInput user)
	{
		return CreateSiteNameOnly(user.MallId, user.MallName);
	}

	/// <summary>
	/// 代表ユーザーと一致していない項目の場合、マーカーCSSを返す
	/// </summary>
	/// <param name="index">インデックス</param>
	/// <param name="key">項目</param>
	/// <returns>マーカーCSS</returns>
	protected string CreateMarkerCss(int index, string key)
	{
		var user = this.UserIntegration.Users[index];
		if (user.IsOnRepresentativeFlg) return "";

		var unMatch = false; 
		switch (key)
		{
			#region 各キー毎に一致していないか？
			case Constants.FIELD_USER_NAME1:
				unMatch = (user.Name1 != this.RepresentativeUser.Name1);
				break;
			case Constants.FIELD_USER_NAME2:
				unMatch = (user.Name2 != this.RepresentativeUser.Name2);
				break;
			case Constants.FIELD_USER_NAME_KANA1:
				unMatch = (user.NameKana1 != this.RepresentativeUser.NameKana1);
				break;
			case Constants.FIELD_USER_NAME_KANA2:
				unMatch = (user.NameKana2 != this.RepresentativeUser.NameKana2);
				break;
			case Constants.FIELD_USER_MAIL_ADDR:
				unMatch = (user.MailAddr != this.RepresentativeUser.MailAddr) && (user.MailAddr != this.RepresentativeUser.MailAddr2);
				break;
			case Constants.FIELD_USER_MAIL_ADDR2:
				unMatch = (user.MailAddr2 != this.RepresentativeUser.MailAddr2) && (user.MailAddr2 != this.RepresentativeUser.MailAddr);
				break;
			case Constants.FIELD_USER_ZIP:
				unMatch = (user.Zip != this.RepresentativeUser.Zip);
				break;
			case Constants.FIELD_USER_ADDR1:
				unMatch = (user.Addr1 != this.RepresentativeUser.Addr1);
				break;
			case Constants.FIELD_USER_ADDR2:
				unMatch = (user.Addr2 != this.RepresentativeUser.Addr2);
				break;
			case Constants.FIELD_USER_ADDR3:
				unMatch = (user.Addr3 != this.RepresentativeUser.Addr3);
				break;
			case Constants.FIELD_USER_ADDR4:
				unMatch = (user.Addr4 != this.RepresentativeUser.Addr4);
				break;
			case Constants.FIELD_USER_TEL1:
				unMatch = (user.Tel1 != this.RepresentativeUser.Tel1) && (user.Tel1 != this.RepresentativeUser.Tel2);
				break;
			case Constants.FIELD_USER_TEL2:
				unMatch = (user.Tel2 != this.RepresentativeUser.Tel2) && (user.Tel2 != this.RepresentativeUser.Tel1);
				break;
			case Constants.FIELD_USER_SEX:
				unMatch = (user.Sex != this.RepresentativeUser.Sex);
				break;
			case Constants.FIELD_USER_BIRTH:
				if (user.Birth.HasValue && this.RepresentativeUser.Birth.HasValue)
				{
					unMatch = (user.Birth.Value.ToShortDateString() != this.RepresentativeUser.Birth.Value.ToShortDateString());
				}
				else if ((user.Birth.HasValue == false) && (this.RepresentativeUser.Birth.HasValue == false))
				{
					unMatch = false;
				}
				break;
			#endregion
		}

		return (unMatch ? "marker_ps" : "");
	}

	/// <summary>
	/// Cancel user integration proccess
	/// </summary>
	/// <param name="actionStatus">Action status</param>
	/// <param name="loginOperatorDeptId">Login operator dept id</param>
	/// <param name="loginOperatorName">Login operator name</param>
	/// <returns>Error messages</returns>
	protected string CancelUserIntegrationProccess(
		string actionStatus,
		string loginOperatorDeptId,
		string loginOperatorName)
	{
		// 代表ユーザー取得
		var model = this.UserIntegration.CreateModel();
		var representativeUser = model.Users.FirstOrDefault(user => user.IsOnRepresentativeFlg);

		var errorMessages = string.Empty;

		// 既に統合済み？
		var service = new UserIntegrationService();
		if (model.IsDoneStatus)
		{
			// 統合解除順チェック
			// <例>以下のような順で統合を実施した場合、
			// (1)（ユーザーA（代表）, ユーザーB）を統合
			// (2)（ユーザーA（代表）, ユーザーC）を統合
			// 「(1)」の統合解除は「(2)」の統合解除を行わないとできない。
			var userIntegrationNos = new List<string>();
			foreach (var userIntegration in service.GetUnintegratedUsers(model.UserIntegrationNo)
				.Where(user => user.IsDoneStatus)
				.OrderByDescending(user => user.DateChanged))
			{
				// 代表ユーザーが含まれている AND 更新日時が新しい？
				if (userIntegration.Users.Any(user => user.UserId == representativeUser.UserId)
					&& (userIntegration.DateChanged > DateTime.Parse(this.UserIntegration.DateChanged)))
				{
					var userIntegrationNoString = string.Format(
						"<a href=\"{0}\">No.{1}</a>",
						WebSanitizer.UrlAttrHtmlEncode(CreateUserIntegrationRegisterlUrl(
							userIntegration.UserIntegrationNo.ToString(),
							Constants.ACTION_STATUS_UPDATE)),
						userIntegration.UserIntegrationNo);

					userIntegrationNos.Add(
						string.Format(
							//「ユーザー統合{0}の統合解除」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER,
								Constants.VALUETEXT_PARAM_USER_INTEGRATION,
								Constants.VALUETEXT_PARAM_USER_INTEGRATION_UNINTEGRATE),
							userIntegrationNoString));
				}
			}

			if (userIntegrationNos.Count != 0)
			{
				trUserIntegrationErrorMessagesTitle.Visible = true;
				trUserIntegrationErrorMessages.Visible = true;

				var userIntegrationErrorMessages = string.Format(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERINTEGRATION_CANCEL_ERROR),
					string.Join(" => ", userIntegrationNos) + string.Format(" => "
						//「ユーザー統合No.{0}（本統合）の統合解除」
						+ ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_USER,
							Constants.VALUETEXT_PARAM_USER_INTEGRATION,
							Constants.VALUETEXT_PARAM_USER_INTEGRATION_UNINTEGRATE_NO), model.UserIntegrationNo) + "<br/>");
				lbUserIntegrationErrorMessages.Text = userIntegrationErrorMessages;

				return userIntegrationErrorMessages;
			}

			// 統合前の代表ユーザー・非代表ユーザーの注文・定期注文のクレジット情報が、登録されていないクレジット情報になっていた場合は解除できない。
			var creditList = CheckCanCancelUserIntegration(representativeUser);
			if (creditList.Any())
			{
				trUserIntegrationErrorMessagesTitle.Visible = true;
				trUserIntegrationErrorMessages.Visible = true;
				var userIntegrationErrorMessages = string.Join(string.Empty, creditList);
				lbUserIntegrationErrorMessages.Text = userIntegrationErrorMessages;

				return userIntegrationErrorMessages;
			}
		}

		// ステータスを未確定に更新
		service.CancelUserIntegration(
			model,
			(actionStatus == Constants.ACTION_STATUS_INSERT),
			loginOperatorDeptId,
			loginOperatorName,
			(Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE
				== Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_USER_ID));

		return string.Empty;
	}

	/// <summary>
	/// Send cancel user integrate async mail
	/// </summary>
	/// <param name="userIntegrationNo">User integration no</param>
	/// <param name="errorMessage">Error message</param>
	protected void SendCancelUserIntegrateAsyncMail(
		string userIntegrationNo,
		string errorMessage)
	{
		var result = string.IsNullOrEmpty(errorMessage)
			? "成功"
			: "失敗";

		var input = new Hashtable
		{
			{ Constants.FIELD_USERINTEGRATION_USER_INTEGRATION_NO, userIntegrationNo },
			{ "result", result },
			{ "error_message", errorMessage }
		};

		using (var mailSender = new MailSendUtility(
			this.LoginOperatorShopId,
			Constants.CONST_MAIL_ID_CANCEL_USERINTEGRATION_ASYNC,
			this.LoginOperatorId,
			input))
		{
			mailSender.SetFrom(Constants.MAIL_RECV_ERROR_MAILADDR_FROM);
			Constants.MAIL_RECV_ERROR_MAILADDR_TO.ToList()
				.ForEach(mailSender.AddTo);

			mailSender.SendMail();
		}
	}

	#region +プロパティ
	/// <summary>リクエスト：代表ユーザーフラグ</summary>
	protected int? RequestRepresentativeFlg
	{
		get
		{
			if (Request.Form["rbRepresentativeFlg"] == null) return null;
			return int.Parse(Request.Form["rbRepresentativeFlg"]);
		}
	}
	/// <summary>ユーザー統合入力情報</summary>
	protected UserIntegrationInput UserIntegration
	{
		get { return (UserIntegrationInput)ViewState["UserIntegration"]; }
		set { ViewState["UserIntegration"] = value; }
	}
	/// <summary>ユーザー統合代表ユーザー情報</summary>
	protected UserIntegrationUserInput RepresentativeUser
	{
		get { return this.UserIntegration.Users.FirstOrDefault(u => u.IsOnRepresentativeFlg); }
	}
	/// <summary>モール連携設定情報リスト</summary>
	protected MallCooperationSettingModel[] MallCooperationSettingList
	{
		get { return (MallCooperationSettingModel[])ViewState["MallCooperationSettingList"]; }
		set { ViewState["MallCooperationSettingList"] = value; }
	}
	/// <summary>モール連携設定情報リスト</summary>
	protected bool IsCouponIntegration
	{
		get { return (bool)ViewState["IsCouponIntegration"]; }
		set { ViewState["IsCouponIntegration"] = value; }
	}
	/// <summary>Is async</summary>
	protected bool IsAsync
	{
		get { return (bool)(Session[SESSION_KEY_IS_ASYNC] ?? false); }
		set { Session[SESSION_KEY_IS_ASYNC] = value; }
	}
	/// <summary>Error message</summary>
	protected string ErrorMessage
	{
		get { return StringUtility.ToEmpty(Session[SESSION_KEY_ERROR_MESSAGE]); }
		set { Session[SESSION_KEY_ERROR_MESSAGE] = value; }
	}
	#endregion
}
