/*
=========================================================================================================
  Module      : Friend Referral (FriendReferral.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using w2.App.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// Friend referral
/// </summary>
public partial class Form_FriendReferral : BasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var userInfo = DomainFacade.Instance.UserService.Get(this.LoginUserId);
		if (userInfo == null) return;

		// Check referral code
		if (string.IsNullOrEmpty(userInfo.ReferralCode) == false)
		{
			this.ReferralCode = userInfo.ReferralCode;
			return;
		}
		
		var arrayOfBytes = Encoding.ASCII.GetBytes(userInfo.UserId);
		userInfo.ReferralCode
			= this.ReferralCode
			= Crc32Utility.Get(arrayOfBytes).ToString("x");

		// Update user referral code
		DomainFacade.Instance.UserService.UpdateUserReferralCode(
			userInfo.UserId,
			userInfo.ReferralCode,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// 紹介リンクを取得
	/// </summary>
	/// <returns>紹介リンク</returns>
	protected string GetReferralUrl()
	{
		var referralUrl = new UrlCreator(Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC)
			.AddParam(Constants.REQUEST_KEY_REFERRAL_CODE, this.ReferralCode)
			.CreateUrl();
		return referralUrl;
	}

	#region +Properties
	/// <summary>Referral code</summary>
	protected string ReferralCode { get; set; }
	#endregion
}
