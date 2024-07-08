/*
=========================================================================================================
  Module      : クリックポイント付与画面処理(ClickPoint.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using w2.Domain;
using w2.Domain.Point;
using w2.Common.Util.Security;
using w2.Domain.UpdateHistory;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.Domain.UpdateHistory.Helper;

public partial class Form_ClickPoint : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		// ログイン
		CheckLoggedIn();

		// 何も格納されない場合を避けるため、エラーステータスで初期化
		this.ClickPointSelect = ClickPointStatus.Error;

		// ルールID空値判定
		var encryptPointRuleId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_POINT_RULE_ID]);
		if (encryptPointRuleId.Equals(string.Empty))
		{
			this.ClickPointSelect = ClickPointStatus.UrlError;
		}
		else
		{
			DecisionClickPoint(encryptPointRuleId);
		}
	}

	/// <summary>
	/// クリックポイント付与判定
	/// </summary>
	/// <param name="encryptPointRuleId">ポイントルールID</param>
	private void DecisionClickPoint(string encryptPointRuleId)
	{
		var clickPointFlag = true;
		var rcPointRuleId = new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
		var pointRuleId = rcPointRuleId.Decrypt(encryptPointRuleId);
		var userPointHistory = new PointService().GetUserPointHistories(this.LoginUserId);
		var pointRule = new PointService().GetPointRule(Constants.W2MP_DEPT_ID, pointRuleId);

		// ポイントルールが存在しない場合
		if (pointRule == null) return;

		// ポイント加算区分がクリックポイントか
		if ((pointRule.PointIncKbn == Constants.FLG_POINTRULE_POITN_INC_KBN_CLICK) && (Constants.POINTRULE_OPTION_CLICKPOINT_ENABLED))
		{
			// 既に付与しているか
			foreach (UserPointHistoryModel user in userPointHistory)
			{
				if (user.PointRuleId == pointRuleId)
				{
					clickPointFlag = false;
					this.ClickPointSelect = ClickPointStatus.SpentPoint;
					break;
				}
			}
			// 有効フラグがＯＮでまだポイント付与していないか
			if ((clickPointFlag) && (pointRule.ValidFlg) == Constants.FLG_POINTRULE_VALID_FLG_VALID)
			{
				// 有効期限が有効範囲内か
				if (pointRule.ExpBgn > DateTime.Now || pointRule.ExpEnd < DateTime.Now)
				{
					this.ClickPointSelect = ClickPointStatus.Expired;
				}
				else
				{
					if (Constants.CROSS_POINT_OPTION_ENABLED
						&& Constants.POINTRULE_OPTION_CLICKPOINT_ENABLED)
					{
						var userModel = DomainFacade.Instance.UserService.Get(this.LoginUserId);
						var result = CrossPointUtility.UpdateCrossPointApiWithWebErrorMessage(
							userModel,
							pointRule.IncNum,
							CrossPointUtility.GetValue(
								Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID,
								pointRule.PointIncKbn));

						if (string.IsNullOrEmpty(result) == false)
						{
							Session[Constants.SESSION_KEY_ERROR_MSG] = result;
							Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
						}
					}

					// ポイント付与
					new PointService().IssuePointByRule(pointRule, this.LoginUserId, string.Empty, pointRule.IncNum, pointRule.LastChanged, UpdateHistoryAction.DoNotInsert);

					new UpdateHistoryService().InsertForUser(this.LoginUserId, pointRule.LastChanged);
					var userPoint =  PointOptionUtility.GetUserPoint(this.LoginUserId);
					this.LoginUserPoint = userPoint;
					this.IncNum = pointRule.IncNum;
					this.ClickPointSelect = ClickPointStatus.ProvidingPoint;
				}
			}
		}
	}

	/// <summary>
	/// クリックポイントステータス
	/// </summary>
	protected enum ClickPointStatus
	{
		/// <summary>URLに間違いがあった時</summary>
		Error,
		/// <summary>URLにルールIDがない時</summary>
		UrlError,
		/// <summary>既にポイント付与している時</summary>
		SpentPoint,
		/// <summary>ポイント付与期間が終了した時</summary>
		Expired,
		/// <summary>ポイント付与する時</summary>
		ProvidingPoint
	}
	/// <summary>クリックポイントステータス</summary>
	protected ClickPointStatus ClickPointSelect { get; set; }
	/// <summary>ポイント加算数</summary>
	protected decimal IncNum { get; set; }
}