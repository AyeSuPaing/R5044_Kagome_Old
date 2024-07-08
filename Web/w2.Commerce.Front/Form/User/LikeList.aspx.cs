/*
=========================================================================================================
  Module      : いいねリスト出力コントローラ処理(LikeList.aspx.cs)
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
using w2.Domain.Coordinate;
using w2.Domain.Staff;
using w2.Domain.User;

public partial class Form_User_LikeList : CoordinatePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示

	#region ラップ済みコントロール宣言
	WrappedRepeater WrLikeList { get { return GetWrappedControl<WrappedRepeater>("rLikeList"); } }
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
			if (string.IsNullOrEmpty(this.Request[Constants.REQUEST_KEY_LIKE_TOKEN]) == false)
			{
				InsertLikeByRequestKey();
			}

			// いいねリストを設定
			SetLikeList();
		}
	}

	/// <summary>
	/// リクエストキーでいいねを登録
	/// </summary>
	public void InsertLikeByRequestKey()
	{
		var likeQuery = this.Request[Constants.REQUEST_KEY_LIKE_TOKEN];
		var likeSession = this.Session[Constants.SESSION_KEY_LIKE_TOKEN];

		if (likeQuery == (string)likeSession)
		{
			var rcAuthenticationKey
				= new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
			var decrypt = rcAuthenticationKey.Decrypt(likeQuery).Split(':');

			var service = new UserService();
			var model = new UserActivityModel
			{
				UserId = this.LoginUserId,
				MasterKbn = Constants.FLG_USERACTIVITY_MASTER_KBN_COORDINATE_LIKE,
				MasterId = decrypt[1]
			};

			if (service.GetUserActivity(model.UserId, model.MasterKbn, model.MasterId) == null)
			{
				// アクティビティ登録
				service.InsertUserActivity(model);
			}

			this.Session[Constants.SESSION_KEY_LIKE_TOKEN] = string.Empty;
		}
	}

	/// <summary>
	/// いいねリストを設定
	/// </summary>
	public void SetLikeList()
	{
		var models = new CoordinateService().GetLikeList(
			this.LoginUserId,
			this.PageNumber,
			Constants.CONST_DISP_CONTENTS_DEFAULT_LIST);
		var service = new StaffService();
		foreach (var model in models)
		{
			model.StaffName = ((service.Get(model.StaffId) != null) ? service.Get(model.StaffId).StaffName : string.Empty);
		}

		var total = new CoordinateService().GetLikeListCount(this.LoginUserId);

		// 0件でなければ、ページャーを設定
		if (total != 0)
		{
			this.WrPager1.Text = this.WrPager2.Text = WebPager.CreateDefaultListPager(total, this.PageNumber, Constants.PATH_ROOT + Constants.PAGE_FRONT_LIKE_LIST);
		}
		else
		{
			this.WrLikeList.Visible = false;
			// エラーメッセージ設定
			this.WrAlertMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_LIKE_NO_ITEM);
		}

		this.LikeList = models.ToList();
		this.LikeListDataSource = this.LikeList;

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 翻訳情報設定
			this.LikeListDataSource = NameTranslationCommon.SetCoordinateTranslationDataWithChild(this.LikeListDataSource.ToArray()).ToList();
		}
		this.WrLikeList.DataSource = this.LikeListDataSource;
		this.WrLikeList.DataBind();
	}

	/// <summary>
	/// フォローを外すボタンをクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDelete_Click(object sender, System.EventArgs e)
	{
		var coordinateId = ((LinkButton)sender).CommandArgument;
		new UserService().DeleteUserActivity(this.LoginUserId, Constants.FLG_USERACTIVITY_MASTER_KBN_COORDINATE_LIKE, coordinateId);

		this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_LIKE_LIST);
	}

	/// <summary>いいねリスト</summary>
	public List<CoordinateModel> LikeList { get; set; }
	/// <summary>いいねリストデータソース</summary>
	public List<CoordinateModel> LikeListDataSource
	{
		get { return (List<CoordinateModel>)this.ViewState["LikeListDataSource"]; }
		private set { this.ViewState["LikeListDataSource"] = value; }
	}
}
