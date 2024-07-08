/*
=========================================================================================================
  Module      : フォローリスト出力コントローラ処理(FollowList.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Translation;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util.Security;
using w2.Domain.RealShop;
using w2.Domain.Staff;
using w2.Domain.User;

public partial class Form_User_FollowList : CoordinatePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示

	#region ラップ済みコントロール宣言
	WrappedRepeater WrFollowList { get { return GetWrappedControl<WrappedRepeater>("rFollowList"); } }
	WrappedLabel WrPager1 { get { return GetWrappedControl<WrappedLabel>("lPager1"); } }
	WrappedLabel WrPager2 { get { return GetWrappedControl<WrappedLabel>("lPager2"); } }
	WrappedLabel WrAlertMessage { get { return GetWrappedControl<WrappedLabel>("lAlertMessage"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!this.IsPostBack)
		{
			if (string.IsNullOrEmpty(this.Request[Constants.REQUEST_KEY_FOLLOW_TOKEN]) == false)
			{
				InsertFollowByRequestKey();
			}

			// フォローリストを設定
			SetFollowList();
		}
	}

	/// <summary>
	/// フォローリストを設定
	/// </summary>
	public void SetFollowList()
	{
		var models = new StaffService().GetFollowList(
			this.LoginUserId,
			this.PageNumber,
			Constants.CONST_DISP_CONTENTS_DEFAULT_LIST);
		var total = new StaffService().GetFollowCount(
			this.LoginUserId);

		// 0件でなければ、ページャーを設定
		if (total != 0)
		{
			this.WrPager1.Text = this.WrPager2.Text = WebPager.CreateDefaultListPager(total, this.PageNumber, Constants.PATH_ROOT + Constants.PAGE_FRONT_FOLLOW_LIST);
		}
		else
		{
			this.WrFollowList.Visible = false;
			// エラーメッセージ設定
			this.WrAlertMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FOLLOW_NO_ITEM);
		}

		// 表示データ取得
		this.FollowList = new List<StaffModel>();
		var staffService = new StaffService();
		var shopService = new RealShopService();
		foreach (var model in models)
		{
			var staff = staffService.Get(model.StaffId);
			if (staff != null)
			{
				var realShop = shopService.Get(staff.RealShopId);
				if (realShop != null)
				{
					if (Constants.GLOBAL_OPTION_ENABLE)
					{
						realShop = NameTranslationCommon.SetRealShopTranslationData(realShop)[0];
					}
					staff.RealShopName = realShop.Name;
				}
				this.FollowList.Add(staff);
			}
		}
		this.FollowListDataSource = this.FollowList;

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 翻訳情報設定
			this.FollowListDataSource = NameTranslationCommon.SetStaffTranslationData(this.FollowListDataSource.ToArray()).ToList();
		}
		this.WrFollowList.DataSource = this.FollowListDataSource;
		this.WrFollowList.DataBind();
	}

	/// <summary>
	/// リクエストキーでフォローを登録
	/// </summary>
	public void InsertFollowByRequestKey()
	{
		var followQuery = this.Request[Constants.REQUEST_KEY_FOLLOW_TOKEN];
		var followSession = this.Session[Constants.SESSION_KEY_FOLLOW_TOKEN];

		if (followQuery == (string)followSession)
		{
			var rcAuthenticationKey
				= new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
			var decrypt = rcAuthenticationKey.Decrypt(followQuery).Split(':');

			var service = new UserService();
			var model = new UserActivityModel
			{
				UserId = this.LoginUserId,
				MasterKbn = Constants.FLG_USERACTIVITY_MASTER_KBN_COORDINATE_FOLLOW,
				MasterId = decrypt[1]
			};

			if (service.GetUserActivity(model.UserId, model.MasterKbn, model.MasterId) == null)
			{
				// アクティビティ登録
				service.InsertUserActivity(model);
			}

			this.Session[Constants.SESSION_KEY_FOLLOW_TOKEN] = string.Empty;
		}
	}

	/// <summary>
	/// フォローを外すボタンをクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDelete_Click(object sender, System.EventArgs e)
	{
		var staffId = ((LinkButton)sender).CommandArgument;
		new UserService().DeleteUserActivity(this.LoginUserId, Constants.FLG_USERACTIVITY_MASTER_KBN_COORDINATE_FOLLOW, staffId);

		this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_FOLLOW_LIST);
	}

	/// <summary>フォローリスト</summary>
	public List<StaffModel> FollowList { get; set; }
	/// <summary>フォローリストデータソース</summary>
	public List<StaffModel> FollowListDataSource
	{
		get { return (List<StaffModel>)this.ViewState["FollowListDataSource"]; }
		private set { this.ViewState["FollowListDataSource"] = value; }
	}
}
