/*
=========================================================================================================
  Module      : ユーザー情報出力コントローラ処理(BodyUserConfirm.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.User.Helper;
using w2.Domain.UserCreditCard;
using w2.Domain.UserIntegration;
using w2.Domain.UserShipping;
using w2.Domain.DmShippingHistory;
using w2.Common.Web;
using w2.App.Common.DefaultSetting;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Config;
using w2.App.Common.Manager.Menu;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.User;
using w2.Domain.TwUserInvoice;
using w2.Domain.UserBusinessOwner;
using w2.App.Common.Global.Region.Currency;

public partial class Form_Common_BodyUserConfirm : BaseUserControl
{
	private const string REQUEST_KEY_USER_WITHDRAWAL = "withdrawal";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 画面設定処理
			//------------------------------------------------------
			// 表示非表示設定
			dvFixedPurchaseHistory.Visible = dvOrderHistory.Visible = dvUserMemberRankHistory.Visible = (this.ActionStatus == Constants.ACTION_STATUS_DETAIL);
			trUserID.Visible = (this.ActionStatus != Constants.ACTION_STATUS_INSERT);
			var uniqueKey = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_UNIQUE_KEY]);

			// 通常の詳細情報表示
			if (this.ActionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				// ユーザー情報取得（存在しない場合はエラーページへ）
				var userService = new UserService();
				var user = userService.Get(this.UserId);
				if (user == null)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				this.UserKbn = user.UserKbn;

				if (Constants.CROSS_POINT_OPTION_ENABLED)
				{
					// Adjust point and member rank by Cross Point api
					UserUtility.AdjustPointAndMemberRankByCrossPointApi(user);
				}

				DisplayUserInfo(user);
				DisplayuserBusinessOwnerInfo(new UserBusinessOwnerService().GetByUserId(this.UserId));
				lUserOrderCountOrderRealtime.Text = WebSanitizer.HtmlEncode(user.OrderCountOrderRealtime.ToString());

				lUserDateLastLoggedin.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						user.DateLastLoggedin,
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
				lUserDateCreated.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						user.DateCreated,
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
				lUserDateChanged.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						user.DateChanged,
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
				lUserLastChanged.Text = WebSanitizer.HtmlEncode(user.LastChanged);

				// エラーポイント取得
				var errorPoint = userService.GetErrorPoint(user.UserId);
				hUserMailAddrErrorPoint.Value = StringUtility.ToEmpty(errorPoint.ErrorPoint);
				hUserMailAddr2ErrorPoint.Value = StringUtility.ToEmpty(errorPoint.ErrorPoint2);

				lUserMailAddrErrorPoint.Text += WebSanitizer.HtmlEncode(hUserMailAddrErrorPoint.Value + "pt)");
				lUserMailAddr2ErrorPoint.Text += WebSanitizer.HtmlEncode(hUserMailAddr2ErrorPoint.Value + "pt)");

				lUserMailAddrErrorPoint.Visible = (string.IsNullOrEmpty(StringUtility.ToEmpty(user.MailAddr)) == false);
				lUserMailAddr2ErrorPoint.Visible = (string.IsNullOrEmpty(StringUtility.ToEmpty(user.MailAddr2)) == false);

				if ((Constants.UPDATE_POINT_ERROR_MAIL_OPTION_ENABLED)
					&& (Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU] != null)
					&& (MenuUtility.HasAuthority((List<MenuLarge>)Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU], this.RawUrl, Constants.KBN_MENU_FUNCTION_USER_UPDATE_POINT_ERROR_MAIL)))
				{
					btnUserMailAddrErrorPoint.Visible = lUserMailAddrErrorPoint.Visible;
					btnUserMailAddr2ErrorPoint.Visible = lUserMailAddr2ErrorPoint.Visible;
				}

				// アフィリエイトOPが有効の場合
				if (Constants.W2MP_AFFILIATE_OPTION_ENABLED == true)
				{
					lUserAdvCode.Text = WebSanitizer.HtmlEncode(user.AdvcodeFirst);
				}

				// ユーザー属性値
				var attributeModel = new w2.Domain.User.UserService().GetUserAttribute(this.UserId);
				tbdyUserAttribute.Visible = (attributeModel != null);
				if (attributeModel != null)
				{
					lFirstOrderDate.Text = WebSanitizer.HtmlEncode(
						DateTimeUtility.ToStringForManager(
							attributeModel.FirstOrderDate,
							DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
							"-"));
					lSecondOrderDate.Text = WebSanitizer.HtmlEncode(
						DateTimeUtility.ToStringForManager(
							attributeModel.SecondOrderDate,
							DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
							"-"));
					lLastOrderDate.Text = WebSanitizer.HtmlEncode(
						DateTimeUtility.ToStringForManager(
							attributeModel.LastOrderDate,
							DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
							"-"));
					lEnrollmentDays.Text = WebSanitizer.HtmlEncode(attributeModel.EnrollmentDays.HasValue ? StringUtility.ToNumeric(attributeModel.EnrollmentDays) : "-");
					lAwayDays.Text = WebSanitizer.HtmlEncode(attributeModel.AwayDays.HasValue ? StringUtility.ToNumeric(attributeModel.AwayDays) : "-");
					lOrderAmountOrderAll.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(attributeModel.OrderAmountOrderAll.ToPriceString()));
					lOrderAmountOrderFp.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(attributeModel.OrderAmountOrderFp.ToPriceString()));
					lOrderCountOrderAll.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(attributeModel.OrderCountOrderAll));
					lOrderCountOrderFp.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(attributeModel.OrderCountOrderFp));
					lOrderAmountShipAll.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(attributeModel.OrderAmountShipAll.ToPriceString()));
					lOrderAmountShipFp.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(attributeModel.OrderAmountShipFp.ToPriceString()));
					lOrderCountShipAll.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(attributeModel.OrderCountShipAll));
					lOrderCountShipFp.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(attributeModel.OrderCountShipFp));
					lAttributeCalculateDate.Text = WebSanitizer.HtmlEncode(
						DateTimeUtility.ToStringForManager(
							attributeModel.DateChanged,
							DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
							"-"));
					if (Constants.CPM_OPTION_ENABLED)
					{
						if (string.IsNullOrEmpty(attributeModel.CpmClusterId) == false)
						{
							var cpmCluster = string.Format(
								//「{0}客」
								ValueText.GetValueText(
									Constants.VALUETEXT_PARAM_BODY_USER_CONFIRM,
									Constants.VALUETEXT_PARAM_CMP_CLUSTER,
									Constants.VALUETEXT_PARAM_CUSTOMER),
								WebSanitizer.HtmlEncode(attributeModel.GetCpmClusterName(Constants.CPM_CLUSTER_SETTINGS)));
							lCpmCluster.Text = cpmCluster;
							lCpmClusterChangedDate.Text = WebSanitizer.HtmlEncode(
								DateTimeUtility.ToStringForManager(
									attributeModel.CpmClusterChangedDate,
									DateTimeUtility.FormatType.ShortDate2Letter,
									"-"));
						}
						if (string.IsNullOrEmpty(attributeModel.CpmClusterIdBefore) == false)
						{
							lCpmClusterBefore.Text = WebSanitizer.HtmlEncode(
								string.Format(
									//「（以前：{0}客）」
									ValueText.GetValueText(
										Constants.VALUETEXT_PARAM_BODY_USER_CONFIRM,
										Constants.VALUETEXT_PARAM_CMP_CLUSTER,
										Constants.VALUETEXT_PARAM_BEFORE),
									attributeModel.GetCpmClusterNameBefore(Constants.CPM_CLUSTER_SETTINGS)));
						}
					}
				}
				else
				{
					lAttributeCalculateDate.Text = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
						"@@DispText.calculate_date.unaggregated@@",
						Constants.GLOBAL_OPTION_ENABLE
							? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
							: "");
				}

				//------------------------------------------------------
				// 注文履歴一覧取得
				//------------------------------------------------------
				var orderInfo = new OrderService().GetOrderHistoryList(this.UserId);

				//Check display view more
				trOrderMore.Visible = (orderInfo.Length > Constants.ITEMS_HISTORY_FIRST_DISPLAY);

				//Show order history count when list order history have values > 0
				if (orderInfo.Length > 0)
				{
					// エラー非表示制御
					trOrderListError.Visible = false;

					//Order History Count
					lbOrderHistoryCount.Text = WebSanitizer.HtmlEncode(
						string.Format(
							"({0})",
							string.Format(
								ReplaceTag("@@DispText.common_message.unit_of_quantity@@"),
								orderInfo.Length)));
					lbOrderHistoryCount.Visible = true;
				}
				else
				{
					// 一覧非表示・エラー表示制御
					trOrderListError.Visible = true;
					tdOrderListErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
				}
				// データバインド
				rOrderList.DataSource = orderInfo;
				rOrderList.DataBind();

				//------------------------------------------------------
				// 会員ランク更新履歴一覧取得
				//------------------------------------------------------
				DataView dvMemberRankHistory = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("UserMemberRank", "GetMemberRankHistoryList"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_USERMEMBERRANKHISTORY_USER_ID, this.UserId);

					dvMemberRankHistory = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
				}

				//Check display view more
				trMemberRankMore.Visible = (dvMemberRankHistory.Count > Constants.ITEMS_HISTORY_FIRST_DISPLAY);

				//Show member rank history count when list member rank history have values > 0
				if (dvMemberRankHistory.Count > 0)
				{
					// エラー非表示制御
					trMemberRankListError.Visible = false;

					//Member Rank History Count
					lbMemberRankHistoryCount.Text = WebSanitizer.HtmlEncode(
						string.Format(
							"({0})",
							string.Format(
								ReplaceTag("@@DispText.common_message.unit_of_quantity@@"),
								dvMemberRankHistory.Count)));
					lbMemberRankHistoryCount.Visible = true;
				}
				else
				{
					// 一覧非表示・エラー表示制御
					trMemberRankListError.Visible = true;
					tdMemberRankListErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
				}
				// データバインド
				rMemberRankList.DataSource = dvMemberRankHistory;
				rMemberRankList.DataBind();

				//------------------------------------------------------
				// 定期購入一覧表示
				//------------------------------------------------------
				if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
				{
					// 定期購入一覧取得					
					var searchCondition = new UserFixedPurchaseListSearchCondition
					{
						UserId = this.UserId
					};
					// データバインド
					var fixedPurchaseList = new FixedPurchaseService().SearchUserFixedPurchase(searchCondition);

					//Check display view more
					trFixedPurchaseMore.Visible = (fixedPurchaseList.Length > Constants.ITEMS_HISTORY_FIRST_DISPLAY);

					//Show fixed purchase history when list purchase history have other values 0
					if (fixedPurchaseList.Length != 0)
					{
						rFixedPurchaseList.DataSource = fixedPurchaseList;
						rFixedPurchaseList.DataBind();

						//Fixed Purchase History Count
						lbFixedPurchaseHistoryCount.Text = WebSanitizer.HtmlEncode(
							string.Format(
								"({0})",
								string.Format(
									ReplaceTag("@@DispText.common_message.unit_of_quantity@@"),
									fixedPurchaseList.Length)));
						lbFixedPurchaseHistoryCount.Visible = true;
					}
					else
					{
						trFixedPurchaseListError.Visible = true;
						tdFixedPurchaseListErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
					}
				}

				//------------------------------------------------------
				// ユーザー拡張項目を表示する（詳細情報の表示）
				//------------------------------------------------------
				if (user.UserExtend != null)
				{
					rUserExtendList.DataSource = GetDisplayUserExtendList(new UserExtendInput(user.UserExtend));
					rUserExtendList.DataBind();
				}

				GetDisplayUserShippingList();

				GetDisplayTaiWanUserInvoiceList();

				// ユーザクレジット情報取得
				BindCreditCardList();

				// 退会ボタン表示制御(権限別)
				// ユーザ情報更新可否によって出し分け
				var menu = (List<MenuLarge>)Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU];
				bool blUpdateEnabled = MenuUtility.HasAuthority(menu, this.RawUrl, Constants.KBN_MENU_FUNCTION_USER_UPDATE);
				dvUserWithdrawal.Visible = blUpdateEnabled;

				// 退会者の場合は退会ボタン、アドレス帳登録ボタン、クレジットカード登録ボタン、注文登録ボタンを非表示
				if (WebSanitizer.HtmlEncode(user.DelFlg) == "1")
				{
					dvUserWithdrawal.Visible = false;
					btnAddShipping.Visible = false;
					btnInsertCreditCard.Visible = false;
					btnGoToOrderRegist.Visible = false;
					btnUserMailAddrErrorPoint.Visible = false;
					btnUserMailAddr2ErrorPoint.Visible = false;
				}

				// ポイントOPが有効の場合
				if (Constants.W2MP_POINT_OPTION_ENABLED == true && UserService.IsUser(this.UserKbn))
				{
					this.m_isDispPointInfo = true;
					//------------------------------------------------------
					// ユーザーポイント情報取得
					//------------------------------------------------------
					GetUserPointData(this.UserId);
				}

				//------------------------------------------------------
				// ユーザー統合情報一覧取得
				//------------------------------------------------------
				if (Constants.USERINTEGRATION_OPTION_ENABLED)
				{
					// 統合済みのユーザー統合情報取得
					var doneUserIntegrationList = new UserIntegrationService().GetUserIntegrationByUserId(this.UserId).Where(ui => ui.IsDoneStatus);

					// 統合された非代表ユーザーの場合は統合済みメッセージ表示
					if (user.IntegratedFlg == Constants.FLG_USER_INTEGRATED_FLG_DONE)
					{
						trMessages.Visible = trUserIntegrationFlg.Visible = true;
						var representativeUser = doneUserIntegrationList
							.SelectMany(ui => ui.Users.Where(u => u.IsOnRepresentativeFlg)).LastOrDefault();
						lbUserIntegrationFlg.Text =
							WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USER_INTERGRATION_REPRESENTATION)
								.Replace(
									"@@ 1 @@",
									string.Format(
										"<a href=\"{0}\">{1}</a>",
										WebSanitizer.UrlAttrHtmlEncode(CreateUserDetailUrl(representativeUser.UserId)),
										representativeUser.UserId))
								.Replace("@@ 2 @@", doneUserIntegrationList.FirstOrDefault().DateChanged.ToString());
					}
					// 統合された代表ユーザー or 統合されていないユーザーの場合はユーザー統合情報表示
					else
					{
						doneUserIntegrationList = doneUserIntegrationList.Where(ui => ui.Users.Any(u => (u.UserId == this.UserId) && u.IsOnRepresentativeFlg));
						if (doneUserIntegrationList.Any())
						{
							dvUserIntegration.Visible = true;
							rUserIntegrationList.DataSource = doneUserIntegrationList.OrderByDescending(ui => ui.DateChanged);
							rUserIntegrationList.DataBind();
						}
					}
				}

				// DM発送履歴一覧取得
				if (Constants.DM_SHIPPING_HISTORY_OPTION_ENABLED)
				{
					var dvDmShippingHistory = DmShippingHistoryService.GetDmShippingHistoryByUserId(this.UserId);
					lbDmShippingHistoryCount.Text = dvDmShippingHistory.Length.ToString();
					trDmShippingHistoryMore.Visible = (dvDmShippingHistory.Length > Constants.ITEMS_HISTORY_FIRST_DISPLAY);
					if (dvDmShippingHistory.Length > 0)
					{
						trDmShippingHistoryListError.Visible = false;
					}
					else
					{
						trDmShippingHistoryListError.Visible = true;
						tdDmShippingHistoryListErrorMessage.InnerHtml =
							WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
					}
					rDmShippingHistoryList.DataSource = dvDmShippingHistory;
					rDmShippingHistoryList.DataBind();
				}
			}
			else if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				var userInput = (UserInput)Session[Constants.SESSION_KEY_PARAM_FOR_USER_INPUT + uniqueKey];
				DisplayUserInfo(userInput.CreateModel());
				DisplayuserBusinessOwnerInfo(userInput.BusinessOwner != null? userInput.BusinessOwner.CreateModel() : null);
				tbdyUserUpdateInfo.Visible = false;
				//------------------------------------------------------
				// ユーザー拡張項目を表示する（新規登録の入力内容を表示）
				//------------------------------------------------------
				rUserExtendList.DataSource = GetDisplayUserExtendList(userInput.UserExtendInput);
				rUserExtendList.DataBind();

				// 退会ボタン非表示
				dvUserWithdrawal.Visible = false;
			}
			else if (this.ActionStatus == Constants.ACTION_STATUS_INSERT)
			{
				var userInput = (UserInput)Session[Constants.SESSION_KEY_PARAM_FOR_USER_INPUT + uniqueKey];
				lSiteName.Text = WebSanitizer.HtmlEncode(BasePage.CreateSiteNameForDetail(Constants.FLG_USER_MALL_ID_OWN_SITE, ""));
				DisplayUserInfo(userInput.CreateModel());
				DisplayuserBusinessOwnerInfo(userInput.BusinessOwner != null ? userInput.BusinessOwner.CreateModel() : null);
				tbdyUserUpdateInfo.Visible = false;
				//------------------------------------------------------
				// ユーザー拡張項目を表示する（新規登録の入力内容を表示）
				//------------------------------------------------------
				rUserExtendList.DataSource = GetDisplayUserExtendList(userInput.UserExtendInput);
				rUserExtendList.DataBind();

				// アフィリエイトOPが有効の場合
				if (Constants.W2MP_AFFILIATE_OPTION_ENABLED == true)
				{
					lUserAdvCode.Text = WebSanitizer.HtmlEncode(userInput.AdvcodeFirst);
				}

				// 退会ボタン非表示
				dvUserWithdrawal.Visible = false;
			}
			// それ以外の場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// 画面データバインディング
			this.DataBind();
		}
	}

	/// <summary>
	/// 退会制限チェック
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <returns>制限あり</returns>
	protected bool IsWithdrawalLimit(string userId)
	{
		var isWithdrawalLimited = new FixedPurchaseService().HasActiveFixedPurchaseInfo(userId);
		return isWithdrawalLimited;
	}

	/// <summary>
	/// ビジネスオーナーコントロール表示
	/// </summary>
	/// <param name="userBusinessOwner">ユーザーモデル</param>
	private void DisplayuserBusinessOwnerInfo(UserBusinessOwnerModel userBusinessOwner) 
	{
		if (userBusinessOwner != null)
		{
			this.IsBusinessOwner = (string.IsNullOrWhiteSpace(userBusinessOwner.OwnerName1) == false);
			lpresidentNameFamily.Text = WebSanitizer.HtmlEncode(userBusinessOwner.OwnerName1);
			lpresidentName.Text = WebSanitizer.HtmlEncode(userBusinessOwner.OwnerName2);
			lpresidentNameFamilyKana.Text = WebSanitizer.HtmlEncode(userBusinessOwner.OwnerNameKana1);
			lpresidentNameKana.Text = WebSanitizer.HtmlEncode(userBusinessOwner.OwnerNameKana2);

			if (userBusinessOwner.Birth != null)
			{
				lOwnerBirth.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(DateTime.Parse(userBusinessOwner.Birth.ToString()), DateTimeUtility.FormatType.LongDate2Letter));
			}
			lreqUpperLimit.Text = CurrencyManager.ToPrice(userBusinessOwner.RequestBudget);
		}
		else
		{
			this.IsBusinessOwner = false;
		}	
	}

	/// <summary>
	/// ユーザー情報をコントロールに表示する
	/// </summary>
	/// <param name="user">ユーザーモデル</param>
	private void DisplayUserInfo(UserModel user)
	{
		lUserId.Text = WebSanitizer.HtmlEncode(user.UserId);
		lSiteName.Text = WebSanitizer.HtmlEncode(
			(this.ActionStatus == Constants.ACTION_STATUS_INSERT)
				? BasePage.CreateSiteNameForDetail(Constants.FLG_USER_MALL_ID_OWN_SITE, "")
				: BasePage.CreateSiteNameForDetail(user.MallId, user.GetMallName()));
		lUserKbn.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN, user.UserKbn));
		lUserName.Text = WebSanitizer.HtmlEncode(user.Name);
		lUserNameKana.Text = WebSanitizer.HtmlEncode(user.NameKana);
		lUserNickName.Text = WebSanitizer.HtmlEncode(user.NickName);
		lUserSex.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_SEX, user.Sex));
		if (user.Birth != null)
		{
			lUserBirth.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(user.Birth.Value, DateTimeUtility.FormatType.LongDate2Letter));
		}
		lUserMailAddr.Text = WebSanitizer.HtmlEncode(user.MailAddr);
		lUserMailAddr2.Text = WebSanitizer.HtmlEncode(user.MailAddr2);
		lUserZip.Text = WebSanitizer.HtmlEncode(user.Zip);
		lUserAddr1.Text = WebSanitizer.HtmlEncode(user.Addr1);
		lUserAddr2.Text = WebSanitizer.HtmlEncode(user.Addr2);
		lUserAddr3.Text = WebSanitizer.HtmlEncode(user.Addr3);
		lUserAddr4.Text = WebSanitizer.HtmlEncode(user.Addr4);
		lUserCompanyName.Text = WebSanitizer.HtmlEncode(user.CompanyName);
		lUserCompanyPostName.Text = WebSanitizer.HtmlEncode(user.CompanyPostName);
		lUserTel1.Text = WebSanitizer.HtmlEncode(user.Tel1);
		lUserTel2.Text = WebSanitizer.HtmlEncode(user.Tel2);
		lUserMailFlg.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_MAIL_FLG, user.MailFlg));
		lUserEasyRegisterFlg.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_EASY_REGISTER_FLG, user.EasyRegisterFlg));
		lUserLoginId.Text = WebSanitizer.HtmlEncode(user.LoginId);
		//権限持ちのオペレータだけにパスワードを表示する
		if (this.HavePasswordDisplayPower)
		{
			this.ShouldDisplayRemarksToPassword = ((user.PasswordDecrypted == user.PasswordDecrypted.Replace(" ", "□")) == false);
			lUserPassword.Text = WebSanitizer.HtmlEncode(user.PasswordDecrypted.Replace(" ", "□"));
		}
		lRemoteAddr.Text = WebSanitizer.HtmlEncode(user.RemoteAddr);
		lUserMemo.Text = WebSanitizer.HtmlEncodeChangeToBr(user.UserMemo);
		lMemberRank.Text = WebSanitizer.HtmlEncode(MemberRankOptionUtility.GetMemberRankName(user.MemberRankId));
		lFixedPurchaseMember.Text
			= WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_USER, Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG, user.FixedPurchaseMemberFlg));
		lUserManagementLevel.Text = WebSanitizer.HtmlEncode(UserManagementLevelUtility.GetUserManagementLevelName(user.UserManagementLevelId));
		lLastBirthdayPointAddYear.Text = WebSanitizer.HtmlEncode(user.LastBirthdayPointAddYear);
		lLastBirthdayCouponPublishYear.Text = WebSanitizer.HtmlEncode(user.LastBirthdayCouponPublishYear);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			lAccessCountryIsoCode.Text = WebSanitizer.HtmlEncode(user.AccessCountryIsoCode);
			lDispLanguageCode.Text = (string.IsNullOrEmpty(user.DispLanguageCode)) ? string.Empty : WebSanitizer.HtmlEncode(string.Format(Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
				"@@DispText.disp_code.language_code@@",
				Constants.GLOBAL_OPTION_ENABLE
					? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
					: ""),
				user.DispLanguageCode));
			lDispLanguageLocaleId.Text = WebSanitizer.HtmlEncode(GlobalConfigUtil.LanguageLocaleIdDisplayFormat(user.DispLanguageLocaleId));
			lDispCurrencyCode.Text = (string.IsNullOrEmpty(user.DispCurrencyCode)) ? string.Empty : WebSanitizer.HtmlEncode(string.Format(Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
				"@@DispText.disp_code.currency_code@@",
				Constants.GLOBAL_OPTION_ENABLE
					? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
					: ""),
				user.DispCurrencyCode));
			lDispCurrencyLocaleId.Text = WebSanitizer.HtmlEncode(GlobalConfigUtil.CurrencyLocaleIdDisplayFormat(user.DispCurrencyLocaleId));

			this.UserAddrCountryIsoCode = user.AddrCountryIsoCode;
			lUserAddrCountryName.Text = WebSanitizer.HtmlEncode(user.AddrCountryName);
			lUserAddr5.Text = WebSanitizer.HtmlEncode(user.Addr5);
			lUserZipGlobal.Text = WebSanitizer.HtmlEncode(user.Zip);
		}

		// アフィリエイトOPが有効の場合
		if (Constants.W2MP_AFFILIATE_OPTION_ENABLED == true)
		{
			lUserAdvCode.Text = WebSanitizer.HtmlEncode(user.AdvcodeFirst);
		}
	}

	/// <summary>
	/// ユーザポイント情報データ取得
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	private void GetUserPointData(string userId)
	{
		var userPoint = new PointService().GetUserPoint(userId, string.Empty);

		var pointObject = new UserPointObjectWrapper(userPoint, userPoint.Any());
		this.UserPoint = pointObject;
	}

	/// <summary>
	/// ユーザー拡張項目の表示用のリストを生成する
	/// </summary>
	/// <param name="userExtend">該当ユーザーの拡張項目(表示の元になるUserExtendInput)</param>
	/// <returns>表示名をDictionary型で返す</returns>
	private Dictionary<string, string> GetDisplayUserExtendList(UserExtendInput userExtend)
	{
		// 最新の設定を取得する
		UserExtendSettingList userExtendSettingList = new UserExtendSettingList(this.LoginOperatorName);

		// 管理画面へ表示する項目のみ表示用にリストを作りなおす取得
		Dictionary<string, string> userExtendDisplayValues = new Dictionary<string, string>();
		foreach (var userExtendSetting in userExtendSettingList.Items.Where(item =>
					(item.DisplayKbn.Contains(Constants.FLG_USEREXTENDSETTING_DISPLAY_EC)) && userExtend.UserExtendDataText.ContainsKey(item.SettingId)))
		{
			// 現在DBに入っている文字列を取得
			userExtendDisplayValues.Add(userExtendSetting.SettingName, (string)userExtend.UserExtendDataText[userExtendSetting.SettingId]);
		}
		return userExtendDisplayValues;
	}

	/// <summary>
	/// 受注詳細URL作成
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateOrderDetailUrl(string strOrderId)
	{
		return OrderPage.CreateOrderDetailUrl(
			strOrderId,
			true,
			false,
			null);
	}

	/// <summary>
	/// 注文登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGoToOrderRegist_Click(object sender, EventArgs e)
	{
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_ORDER_REGIST_INPUT);
		url.Append("?").Append(Constants.REQUEST_KEY_USER_ID).Append("=").Append(HttpUtility.UrlEncode(this.UserId));

		Response.Redirect(url.ToString());
	}

	/// <summary>
	/// アドレス帳一覧編集、削除ボタンクリック
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rUserShippingList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		int shippingNo = int.Parse(e.CommandArgument.ToString());
		if (e.CommandName == "Delete")
		{
			// ユーザ配送先情報削除
			new UserShippingService().Delete(
				this.UserId,
				shippingNo,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert,
				Constants.TWOCLICK_OPTION_ENABLE);

			// ユーザ情報詳細画面へ
			Response.Redirect(CreateUserDetailUrl(this.UserId));
		}
	}

	/// <summary>
	/// Edit & Delete TaiWan UserInvoice
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rUserInvoiceList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		var twInvoiceNo = int.Parse(e.CommandArgument.ToString());
		if (e.CommandName == "Delete")
		{
			new TwUserInvoiceService().Delete(
				this.UserId,
				twInvoiceNo,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

			GetDisplayTaiWanUserInvoiceList();
		}
	}

	/// <summary>
	/// データバインド用ユーザ詳細URL作成
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <returns>ユーザ詳細URL</returns>
	public static string CreateUserDetailUrl(string userId)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_USER_CONFIRM);
		url.Append("?").Append(Constants.REQUEST_KEY_USER_ID).Append("=").Append(HttpUtility.UrlEncode(userId));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return url.ToString();
	}

	/// <summary>
	/// データバインド用ユーザーポイント履歴URL作成
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <returns>ユーザーポイント履歴URL</returns>
	public static string CreateUserPointHistoryUrl(string userId)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT_MP).Append(Constants.PAGE_W2MP_MANAGER_USERPOINTHISTORY_LIST);
		url.Append("?").Append(Constants.REQUEST_KEY_USER_ID).Append("=").Append(HttpUtility.UrlEncode(userId));
		url.Append("&").Append(Constants.REQUEST_KEY_POINT_KBN).Append("=").Append(Constants.FLG_USERPOINT_POINT_KBN_BASE);
		url.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(Constants.KBN_WINDOW_POPUP);
		return url.ToString();
	}

	/// <summary>
	/// アドレス帳詳細URL作成
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <param name="shippingNo">配送先枝番</param>
	/// <returns>情報ページへのURL</returns>
	public static string CreateShippingDetailUrl(string userId, int shippingNo)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_USER_SHIPPING_INPUT);
		url.Append("?").Append(Constants.REQUEST_KEY_USER_ID).Append("=").Append(HttpUtility.UrlEncode(userId));
		url.Append("&").Append(Constants.REQUEST_KEY_SHIPPING_NO).Append("=").Append(HttpUtility.UrlEncode(shippingNo.ToString()));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPDATE);

		return url.ToString();
	}

	/// <summary>
	/// アドレス帳登録URL作成
	/// </summary>
	/// <returns>アドレス帳登録</returns>
	public string CreateShippingInsertUrl()
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_USER_SHIPPING_INPUT);
		url.Append("?").Append(Constants.REQUEST_KEY_USER_ID).Append("=").Append(HttpUtility.UrlEncode(this.UserId));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_INSERT);

		return url.ToString();
	}

	/// <summary>
	/// Create User Invoic eDetai lUrl
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <param name="shippingNo">配送先枝番</param>
	/// <returns>情報ページへのURL</returns>
	public static string CreateUserInvoiceDetailUrl(string userId, int invoiceNo)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_USER_INVOICE_INPUT);
		url.Append("?").Append(Constants.REQUEST_KEY_USER_ID).Append("=").Append(HttpUtility.UrlEncode(userId));
		url.Append("&").Append(Constants.REQUEST_KEY_INVOICE_NO).Append("=").Append(HttpUtility.UrlEncode(invoiceNo.ToString()));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPDATE);

		return url.ToString();
	}

	/// <summary>
	/// Create User Invoice Insert Url
	/// </summary>
	/// <returns>アドレス帳登録</returns>
	public string CreateUserInvoiceInsertUrl()
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_USER_INVOICE_INPUT);
		url.Append("?").Append(Constants.REQUEST_KEY_USER_ID).Append("=").Append(HttpUtility.UrlEncode(this.UserId));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_INSERT);

		return url.ToString();
	}

	/// <summary>
	/// ユーザー統合登録URL作成
	/// </summary>l
	/// <param name="userIntegrationNo">ユーザー統合No</param>
	/// <returns>ユーザー統合登録URL</returns>
	public static string CreateUserIntegrationRegisterlUrl(string userIntegrationNo)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_INTEGRATION_REGISTER);
		urlCreator
			.AddParam(Constants.REQUEST_KEY_USERINTEGRATION_USER_INTEGRATION_NO, userIntegrationNo)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP);

		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// Create link search user in CS
	/// </summary>
	/// <param name="userId">UserId</param>
	/// <returns>Link to search user in CS</returns>
	public string CreateLinkUserSearchCS(string userId)
	{
		return string.Format("{0}{1}?{2}={3}",
			Constants.PATH_ROOT_CS,
			Constants.PAGE_W2CS_MANAGER_MESSAGE_USER_SEARCH,
			Constants.REQUEST_KEY_USER_ID,
			HttpUtility.UrlEncode(userId));
	}

	/// <summary>
	/// アドレス帳一覧を表示
	/// </summary>
	protected void GetDisplayUserShippingList()
	{
		// ユーザ配送先情報取得
		var userShippings = new UserShippingService().GetAllOrderByShippingNoDesc(this.UserId).ToArray();

		// アドレス帳一覧設定
		if (userShippings.Length == 0)
		{
			// 0件の場合、エラーメッセージ表示
			this.rUserShippingList.Visible = false;
			this.trShippingListMessage.Visible = true;
			this.tdShippingListMessageError.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERSHIPPING_NO_SHIPPING);
		}
		else
		{
			this.trShippingListMessage.Visible = false;
		}

		// データバインド
		this.rUserShippingList.DataSource = userShippings;
		this.rUserShippingList.DataBind();
	}

	/// <summary>
	/// Get Display TaiWan User Invoice List
	/// </summary>
	protected void GetDisplayTaiWanUserInvoiceList()
	{
		var userInvoices = new TwUserInvoiceService().GetAllUserInvoiceByUserId(this.UserId);
		if (userInvoices == null)
		{
			this.rUserInvoiceList.Visible = false;
			this.trUserInvoiceListMessage.Visible = true;
			this.tdUserInvoiceListMessageError.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USERINVOICE_NO_INVOICE);
		}

		// データバインド
		this.rUserInvoiceList.DataSource = userInvoices;
		this.rUserInvoiceList.DataBind();
	}

	/// <summary>電子発票管理枝番</summary>
	private int InvoiceNo
	{
		get
		{
			var invoiceNo = 0;
			int.TryParse(Request[Constants.REQUEST_KEY_INVOICE_NO], out invoiceNo);

			return invoiceNo;
		}
	}

	/// <summary>
	/// リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rUserCreditCardList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		//------------------------------------------------------
		// 登録クレジット情報削除
		//------------------------------------------------------
		if (e.CommandName == "Delete")
		{
			var arg = e.CommandArgument.ToString().Split(' ');
			var creditBranchNo = int.Parse(arg[0]);

			var userCreditCard = new UserCreditCardService().Get(this.UserId, int.Parse(e.CommandArgument.ToString()));
			if (userCreditCard.IsRegisterdStatusUnregisterd)
			{
				// 削除
				new UserCreditCardService().Delete(this.UserId, creditBranchNo, this.LoginOperatorName, UpdateHistoryAction.Insert);
			}
			else
			{
				// 表示フラグオフ
				new UserCreditCardService().UpdateDispFlg(this.UserId, creditBranchNo, false, this.LoginOperatorName, UpdateHistoryAction.Insert);
			}
				BindCreditCardList();
			}
		}

	/// <summary>
	/// ユーザクレジット情報バインド
	/// </summary>
	public void BindCreditCardList()
	{
		if (UserService.IsUser(this.UserKbn) == true && Constants.MAX_NUM_REGIST_CREDITCARD > 0 && Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_DETAIL)
		{
			//------------------------------------------------------
			// ユーザクレジット情報取得
			//------------------------------------------------------
			var userCreditCards = UserCreditCard.GetUsableOrUnregisterd(this.UserId);

			// データバインド
			rUserCreditCardList.DataSource = userCreditCards;
			rUserCreditCardList.DataBind();

			// エラーメッセージ
			trCreditCardErrorMessage.Visible = (userCreditCards.Length == 0);

			//------------------------------------------------------
			// 表示制御
			//------------------------------------------------------
			var allowAddMore = OrderCommon.GetCreditCardRegistable(this.LoginOperatorId != null, userCreditCards.Length) && UserService.IsUser(this.UserKbn);
			divUserCreditCardRegisterable.Visible = allowAddMore;
			btnInsertCreditCard.Visible = (allowAddMore && (this.IsPopUp == false) && ((Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) || this.IsUserPayTg));
		}
	}

	/// <summary>
	/// Update User PC mail error point
	/// </summary>
	/// <param name="sender">差し出し人</param>
	/// <param name="e">イベント</param>
	protected void btnUserMailAddrErrorPoint_Click(object sender, EventArgs e)
	{
		ClearUserMailAddrErrorPoint(lUserMailAddr.Text);
	}

	/// <summary>
	/// Update User Mobile mail error point
	/// </summary>
	/// <param name="sender">差し出し人</param>
	/// <param name="e">イベント</param>
	protected void btnUserMailAddr2ErrorPoint_Click(object sender, EventArgs e)
	{
		ClearUserMailAddrErrorPoint(lUserMailAddr2.Text);
	}

	/// <summary>
	/// Clear User Mail Addr Error Point
	/// </summary>
	/// <param name="mailAddr"> mail addr</param>
	private void ClearUserMailAddrErrorPoint(string mailAddr)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MailErrorAddr", "DeleteMailErrorAddr"))
		{
			Hashtable mailAddrErrorPointInput = new Hashtable();
			mailAddrErrorPointInput.Add(Constants.FIELD_MAILERRORADDR_MAIL_ADDR, mailAddr);

			sqlStatement.ExecStatementWithOC(sqlAccessor, mailAddrErrorPointInput);
		}

		Response.Redirect(CreateUserDetailUrl(StringUtility.ToEmpty(this.UserId)));
	}

	/// <summary>顧客区分</summary>
	protected string UserKbn
	{
		get { return (string)ViewState[Constants.FIELD_USER_USER_KBN] ?? string.Empty; }
		set { ViewState[Constants.FIELD_USER_USER_KBN] = value; }
	}

	/// <summary>
	/// 会員退会ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void btnUserWithdrawal_Click(object sender, EventArgs e)
	{
		// 退会処理
		var errorMessage = string.Empty;
		UserUtility.Withdrawal(
			this.UserId,
			this.LoginOperatorName,
			out errorMessage,
			UpdateHistoryAction.Insert);
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// 退会処理済のユーザー情報詳細ページへ遷移
		Response.Redirect(CreateUserDetailUrl(lUserId.Text) + "&" + REQUEST_KEY_USER_WITHDRAWAL + "=1");

	}

	/// <summary>
	/// 受注履歴一覧で再注文ボタンクリック
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rOrderList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		string orderId = e.CommandArgument.ToString();

		if (e.CommandName == "ReOrder")
		{
			var url = new StringBuilder();
			url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_ORDER_REGIST_INPUT);
			url.Append("?").Append(Constants.REQUEST_KEY_REORDER_ID).Append("=").Append(HttpUtility.UrlEncode(orderId));

			Response.Redirect(url.ToString());
		}
	}

	/// <summary>
	/// 数値表示
	/// </summary>
	/// <param name="objNum">数量</param>
	/// <returns>数量</returns>
	public static string GetNumeric(object objNum)
	{
		return StringUtility.ToNumeric(objNum);
	}

	/// <summary>再注文の可否判断</summary>
	/// <param name="orderInfo">注文情報</param>
	/// <returns>true: 可能、false:不可</returns>
	public bool CanReOrder(OrderModel orderInfo)
	{
		// 下記の注文に対しては、再注文できない。
		//  ・セット商品を購入した注文
		//  ・返品／交換注文
		//  ・定期購入のみの商品を購入した注文
		if ((orderInfo.ReturnExchangeKbn != Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN)
			|| orderInfo.Items.Any(orderItem => orderItem.ProductSetId != string.Empty)
			|| orderInfo.Items.Any(orderItem => orderItem.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY))
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// 再注文機能の使用可否フラグ
	/// </summary>
	/// <returns>true: 使用可能・false:使用不可</returns>
	public bool CanUseReOrderFunction()
	{
		// ログインオペレータメニュ―に新規注文登録があるかで権限を判定
		var hasAuthorityToUseOrderRegistInput = this.LoginOperatorMenu.Exists(ml => (ml.SmallMenus.Any(ms => (Constants.PAGE_MANAGER_ORDER_REGIST_INPUT.IndexOf(ms.MenuPath) >= 0))));

		// 注文登録メニューの権限が持っていない場合、又は、注文登録画面以外からアクセスする場合
		if ((hasAuthorityToUseOrderRegistInput == false)
			|| ((this.IsPopUp) && (Request[Constants.REQUEST_KEY_REORDER_FLG] != Constants.FLG_REORDER_FLG_ON)))
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// 購入商品情報の文字列を取得
	/// </summary>
	/// <param name="orderItemsInfo">購入商品情報</param>
	/// <returns>購入商品情報の文字列</returns>
	protected string GetHistoryOrderItemDetail(OrderItemModel[] orderItemsInfo)
	{
		if ((orderItemsInfo == null) || (orderItemsInfo.Length == 0)) return string.Empty;

		var tooltipBuild = new StringBuilder();
		foreach (var orderItem in orderItemsInfo)
		{
			var productId = string.Format("[{0}]",
				((orderItem.VariationId != string.Empty) && (orderItem.VariationId != orderItem.ProductId))
				? string.Format(
					"{0} + {1}",
					orderItem.ProductId,
					orderItem.ProductId.StartsWith(orderItem.VariationId)
						? orderItem.VariationId.Substring(orderItem.ProductId.Length)
						: orderItem.VariationId)
				: orderItem.ProductId);

			var tooltip = string.Format("{0}\t{1}\tx{2}", productId, orderItem.ProductName, orderItem.ItemQuantity);
			tooltipBuild.AppendLine(tooltip);
		}

		return tooltipBuild.ToString();
	}

	/// <summary>
	/// Get Display Code
	/// </summary>
	/// <param name="userInvoiceModel">User Invoice data item</param>
	/// <returns>String</returns>
	protected string GetDisplayCode(TwUserInvoiceModel userInvoiceModel)
	{
		var result = string.Empty;

		switch (userInvoiceModel.TwUniformInvoice)
		{
			case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
				result = userInvoiceModel.TwCarryTypeOption;
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
				result = string.Format("{0} : {1}<br/>{2} : {3}",
					ReplaceTag("@@TwInvoice.uniform_invoice_company_code_option.name@@"),
					userInvoiceModel.TwUniformInvoiceOption1,
					ReplaceTag("@@TwInvoice.uniform_invoice_company_name_option.name@@"),
					userInvoiceModel.TwUniformInvoiceOption2);
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				result = string.Format("{0}<br/>{1}",
					ReplaceTag("@@TwInvoice.uniform_invoice_donate_code_option.name@@"),
					userInvoiceModel.TwUniformInvoiceOption1);
				break;
		}

		return result;
	}

	/// <summary>
	/// クレジットカード登録向けリロード処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReloadForRegisterdCreditCard_Click(object sender, EventArgs e)
	{
		BindCreditCardList();
	}
	/// <summary>パスワードの備考表示フラグ</summary>
	protected bool ShouldDisplayRemarksToPassword { get; private set; }
	/// <summary>（パラメタから取得できる）ユーザーID</summary>
	public string UserId
	{
		get { return Request[Constants.REQUEST_KEY_USER_ID]; }
	}
	/// <summary>ユーザー本ポイント表示フラグ</summary>
	public bool IsDispPointInfo
	{
		get { return this.m_isDispPointInfo; }
	}
	/// <summary>ユーザー本ポイント</summary>
	public decimal UserPointUsable
	{
		get { return (this.UserPoint != null) ? this.UserPoint.PointUsable : 0; }
	}
	/// <summary>ユーザー仮ポイント</summary>
	public decimal UserPointTemp
	{
		get { return (this.UserPoint != null) ? this.UserPoint.PointTemp : 0; }
	}
	/// <summary>ユーザーポイント有効期限</summary>
	public DateTime? UserPointExpiry
	{
		get { return (this.UserPoint != null) ? this.UserPoint.BasicPoint.PointCompExpiryDate : null; }
	}
	/// <summary>ログインオペレータメニュー</summary>
	protected List<MenuLarge> LoginOperatorMenu
	{
		get { return (List<MenuLarge>)Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU]; }
	}
	/// <summary>ユーザーポイント</summary>
	public UserPointObjectWrapper UserPoint { get; set; }
	/// <summary>ユーザー本ポイント表示フラグ</summary>
	private bool m_isDispPointInfo = false;
	/// <summary>ユーザーの住所は日本か</summary>
	protected bool IsUserAddrJp
	{
		get { return IsCountryJp(this.UserAddrCountryIsoCode); }
	}
	/// <summary>ユーザー住所国ISOコード</summary>
	protected string UserAddrCountryIsoCode
	{
		get { return (string)ViewState["UserAddrCountryIsoCode"]; }
		set { ViewState["UserAddrCountryIsoCode"] = value; }
	}

	/// <summary> 「パスワード欄表示」権限の判定 </summary>
	protected bool HavePasswordDisplayPower
	{
		get { return (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_USER_PASSWORD_DISPLAY)); }
	}

	/// <summary>ビジネスオーナーコントロール表示</summary>
	public bool IsBusinessOwner
	{
		get { return (bool)ViewState["IsBusinessOwner"]; }
		set { ViewState["IsBusinessOwner"] = value; }
	}

	/// <summary>
	/// ユーザーポイントオブジェクトのラッパー
	/// </summary>
	public class UserPointObjectWrapper : w2.App.Common.Option.UserPointObject
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="pointObject">ラップするポイントオブジェクト</param>
		/// <param name="existsUserPointRecord">ポイントレコードを持つかどうか</param>
		public UserPointObjectWrapper(UserPointModel[] pointObject, bool existsUserPointRecord)
			: base(pointObject)
		{
			this.ExistsUserPointRecord = existsUserPointRecord;
		}

		/// <summary>
		/// ポイントレコードを持つかどうか
		/// True:もつ
		/// False：持たない
		/// </summary>
		public bool ExistsUserPointRecord { get; private set; }
	}

	/// <summary>
	/// クレジットカード登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertCreditCard_Click(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_CREDITCARD_INPUT)
			.AddParam(Constants.REQUEST_KEY_USER_ID, Request[Constants.FIELD_USER_USER_ID])
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_DEFAULT)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS,Constants.ACTION_STATUS_INSERT).CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// Get default setting display field
	/// </summary>
	/// <param name="tableName">Table name</param>
	/// <param name="fieldName">Field name/param>
	/// <returns>Can display</returns>
	protected bool GetDefaultSettingDisplayField(string tableName, string fieldName)
	{
		var canDisplay = DefaultSettingPage.IsDefaultSettingDisplayField(
			this.DefaultSettingInfo,
			tableName,
			fieldName);
		return canDisplay;
	}

	/// <summary>
	/// Get default setting comment for display
	/// </summary>
	/// <param name="tableName">Table name</param>
	/// <param name="fieldName">Field name</param>
	/// <returns>Comment</returns>
	protected string GetDefaultSettingCommentForDisplay(string tableName, string fieldName)
	{
		var comment = DefaultSettingPage.GetDefaultSettingComment(
			this.DefaultSettingInfo,
			tableName,
			fieldName);

		if (string.IsNullOrEmpty(comment)) return string.Empty;

		var result = string.Format(
			"[{0}]",
			this.DefaultSettingInfo.DefaultSettingTables[tableName].GetDefaultSettingCommentField(fieldName));
		return result;
	}

	#region +Properties
	/// <summary>Is default setting page</summary>
	protected bool IsDefaultSettingPage
	{
		get { return (this.ActionStatus == Constants.ACTION_STATUS_DEFAULTSETTING); }
	}
	/// <summary>Default setting</summary>
	private DefaultSetting m_defaultSetting = null;
	/// <summary>Default setting info</summary>
	protected DefaultSetting DefaultSettingInfo
	{
		get
		{
			if (m_defaultSetting == null)
			{
				m_defaultSetting = new DefaultSetting();
				m_defaultSetting.LoadDefaultSetting(this.LoginOperatorShopId, Constants.TABLE_USER);
			}
			return m_defaultSetting;
		}
	}
	/// <summary>PayTgを利用するか</summary>
	protected bool IsUserPayTg
	{
		get
		{
			var result = (Constants.PAYMENT_SETTING_PAYTG_ENABLED
				&& ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
					|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)));
			return result;
		}
	}
	#endregion

	/// <summary>
	/// クレジットカード変更画面URL作成
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <param name="branchNo">カード枝番</param>
	/// <returns>情報ページへのURL</returns>
	public static string CreateCreditCardEdit(string userId, int branchNo)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_CREDITCARD_INPUT)
			.AddParam(Constants.REQUEST_KEY_USER_ID, HttpUtility.UrlEncode(userId))
			.AddParam(Constants.REQUEST_KEY_CREDITCARD_NO,HttpUtility.UrlEncode(branchNo.ToString()))
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS,Constants.ACTION_STATUS_UPDATE).CreateUrl();

		return url;
	}
}
