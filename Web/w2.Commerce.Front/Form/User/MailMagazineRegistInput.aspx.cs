/*
=========================================================================================================
  Module      : メールマガジン登録入力画面処理(MailMagazineRegistInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.User;

public partial class Form_User_MailMagazineRegistInput : BasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	#region ラップ済コントロール宣言
	protected WrappedTextBox WtbUserName1 { get { return GetWrappedControl<WrappedTextBox>("tbUserName1"); } }
	protected WrappedTextBox WtbUserName2 { get { return GetWrappedControl<WrappedTextBox>("tbUserName2"); } }
	protected WrappedTextBox WtbUserNameKana1 { get { return GetWrappedControl<WrappedTextBox>("tbUserNameKana1"); } }
	protected WrappedTextBox WtbUserNameKana2 { get { return GetWrappedControl<WrappedTextBox>("tbUserNameKana2"); } }
	protected WrappedTextBox WtbUserMailAddr { get { return GetWrappedControl<WrappedTextBox>("tbUserMailAddr"); } }
	protected WrappedTextBox WtbUserMailAddrConf { get { return GetWrappedControl<WrappedTextBox>("tbUserMailAddrConf"); } }
	protected WrappedTextBox WtbUserNickName { get { return GetWrappedControl<WrappedTextBox>("tbUserNickName"); } }
	protected WrappedDropDownList WddlUserBirthYear { get { return GetWrappedControl<WrappedDropDownList>("ddlUserBirthYear"); } }
	protected WrappedDropDownList WddlUserBirthMonth { get { return GetWrappedControl<WrappedDropDownList>("ddlUserBirthMonth"); } }
	protected WrappedDropDownList WddlUserBirthDay { get { return GetWrappedControl<WrappedDropDownList>("ddlUserBirthDay"); } }
	protected WrappedRadioButtonList WrblUserSex { get { return GetWrappedControl<WrappedRadioButtonList>("rblUserSex", Constants.FLG_USER_SEX_UNKNOWN); } }
	protected WrappedTextBox WtbUserZip1 { get { return GetWrappedControl<WrappedTextBox>("tbUserZip1"); } }
	protected WrappedTextBox WtbUserZip2 { get { return GetWrappedControl<WrappedTextBox>("tbUserZip2"); } }
	protected WrappedDropDownList WddlUserAddr1 { get { return GetWrappedControl<WrappedDropDownList>("ddlUserAddr1"); } }
	protected WrappedTextBox WtbUserAddr2 { get { return GetWrappedControl<WrappedTextBox>("tbUserAddr2"); } }
	protected WrappedTextBox WtbUserAddr3 { get { return GetWrappedControl<WrappedTextBox>("tbUserAddr3"); } }
	protected WrappedTextBox WtbUserAddr4 { get { return GetWrappedControl<WrappedTextBox>("tbUserAddr4"); } }
	protected WrappedTextBox WtbUserCompanyName { get { return GetWrappedControl<WrappedTextBox>("tbUserCompanyName"); } }
	protected WrappedTextBox WtbUserCompanyPostName { get { return GetWrappedControl<WrappedTextBox>("tbUserCompanyPostName"); } }
	protected WrappedTextBox WtbUserTel1_1 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel1"); } }
	protected WrappedTextBox WtbUserTel1_2 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel2"); } }
	protected WrappedTextBox WtbUserTel1_3 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel3"); } }
	protected WrappedTextBox WtbUserTel2_1 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel2_1"); } }
	protected WrappedTextBox WtbUserTel2_2 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel2_2"); } }
	protected WrappedTextBox WtbUserTel2_3 { get { return GetWrappedControl<WrappedTextBox>("tbUserTel2_3"); } }
	protected WrappedCustomValidator WcvUserName1 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserName1"); } }
	protected WrappedCustomValidator WcvUserName2 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserName2"); } }
	protected WrappedCustomValidator WcvUserNameKana1 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserNameKana1"); } }
	protected WrappedCustomValidator WcvUserNameKana2 { get { return GetWrappedControl<WrappedCustomValidator>("cvUserNameKana2"); } }
	protected WrappedCustomValidator WcvUserMailAddr { get { return GetWrappedControl<WrappedCustomValidator>("cvUserMailAddr"); } }
	protected WrappedCustomValidator WcvUserMailAddrConf { get { return GetWrappedControl<WrappedCustomValidator>("cvUserMailAddrConf"); } }
	protected WrappedCustomValidator WcvUserBirth { get { return GetWrappedControl<WrappedCustomValidator>("cvUserBirth"); } }
	protected WrappedCustomValidator WcvUserSex { get { return GetWrappedControl<WrappedCustomValidator>("cvUserSex"); } }
	protected WrappedTextBox WtbUserZip { get { return GetWrappedControl<WrappedTextBox>("tbUserZip"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
    {
		// HTTPS通信チェック（HTTPのとき、HTTPSで再読込）
		CheckHttps(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_MAILMAGAZINE_REGIST_INPUT);

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitComponents();
#if DEBUG
			//------------------------------------------------------
			// デバッグの場合初期値設定
			//------------------------------------------------------
			this.WtbUserName1.Text = "ｗ２";
			this.WtbUserName2.Text = "テスト";
			this.WtbUserNameKana1.Text = "だぶるつ";
			this.WtbUserNameKana2.Text = "てすと";
			this.WtbUserMailAddr.Text = string.Format("bh+{0}@w2s.xyz", DateTime.Now.ToString("yyMMddHHmmss"));
			this.WtbUserMailAddrConf.Text = string.Format("bh+{0}@w2s.xyz", DateTime.Now.ToString("yyMMddHHmmss"));
#endif
			//------------------------------------------------------
			// デフォルト設定
			//------------------------------------------------------
			if (this.WrblUserSex.InnerControl != null) this.WrblUserSex.SelectedValue = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.sex.default@@");

			// 確認画面から戻った場合セッション情報から会員情報を入力欄にセットする
			if (this.SessionParamTargetPage == Constants.PAGE_FRONT_MAILMAGAZINE_REGIST_INPUT)
			{
				UserInput userInput = (UserInput)Session[Constants.SESSION_KEY_PARAM];
				var user = userInput.CreateModel();
				this.DisplayUserInfo(user);
			
				//------------------------------------------------------
				// ターゲットページ設定
				//------------------------------------------------------
				this.SessionParamTargetPage = null;
			}
			else if (IsLoggedIn)
			{
				//------------------------------------------------------
				// ユーザーIDからユーザー情報の取得し、入力欄にセットする
				//------------------------------------------------------
				var user = new UserService().Get(this.LoginUserId);
				if (user != null)
				{
					this.DisplayUserInfo(user);
				}
			}
			// apsx側プロパティセットしているため、バインドを行う
			if (this.Captcha != null)
			{
				this.Captcha.DataBind();
			}
		}
	}

	/// <summary>
	/// 画面にユーザ情報を表示する
	/// </summary>
	/// <param name="user">ユーザー情報</param>
	private void DisplayUserInfo(UserModel user)
	{
		this.WtbUserName1.Text = user.Name1;
		this.WtbUserName2.Text = user.Name2;
		this.WtbUserNameKana1.Text = user.NameKana1;
		this.WtbUserNameKana2.Text = user.NameKana2;
		this.WtbUserMailAddr.Text = user.MailAddr;
		this.WtbUserMailAddrConf.Text = user.MailAddr;
		this.WtbUserNickName.Text = user.NickName;
		this.WddlUserBirthYear.SelectedValue = user.BirthYear;
		this.WddlUserBirthMonth.SelectedValue = user.BirthMonth;
		this.WddlUserBirthDay.SelectedValue = user.BirthDay;
		this.WrblUserSex.SelectItemByValue(user.Sex);
		this.WtbUserZip1.Text = user.Zip1;
		this.WtbUserZip2.Text = user.Zip2;
		this.WddlUserAddr1.SelectedValue = user.Addr1;
		this.WtbUserAddr2.Text = user.Addr2;
		this.WtbUserAddr3.Text = user.Addr3;
		this.WtbUserAddr4.Text = user.Addr4;
		this.WtbUserCompanyName.Text = user.CompanyName;
		this.WtbUserCompanyPostName.Text = user.CompanyPostName;
		this.WtbUserTel1_1.Text = user.Tel1_1;
		this.WtbUserTel1_2.Text = user.Tel1_2;
		this.WtbUserTel1_3.Text = user.Tel1_3;
		this.WtbUserTel2_1.Text = user.Tel2_1;
		this.WtbUserTel2_2.Text = user.Tel2_2;
		this.WtbUserTel2_3.Text = user.Tel2_3;
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitComponents()
	{
		//------------------------------------------------------
		// コンポーネント初期化
		//------------------------------------------------------
		// 性別ラジオボタン設定
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USER, Constants.FIELD_USER_SEX))
		{
			this.WrblUserSex.AddItem(li);
		}

		// 生年月日ドロップダウン作成
		this.WddlUserBirthYear.AddItems(DateTimeUtility.GetBirthYearListItem());
		this.WddlUserBirthYear.SelectedValue = (this.WddlUserBirthYear.InnerControl != null) ? "1970" : "";
		this.WddlUserBirthMonth.AddItems(DateTimeUtility.GetMonthListItem());
		this.WddlUserBirthDay.AddItems(DateTimeUtility.GetDayListItem());
	
		// 都道府県ドロップダウン作成
		this.WddlUserAddr1.AddItem(new ListItem("", ""));
		foreach (string strPrefecture in Constants.STR_PREFECTURES_LIST)
		{
			this.WddlUserAddr1.AddItem(new ListItem(strPrefecture));
		}
	}

	/// <summary>
	/// 郵便番号検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddr_Click(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 郵便番号入力チェック
		//------------------------------------------------------
		string strZip = StringUtility.ToHankaku(this.WtbUserZip1.Text + this.WtbUserZip2.Text);

		// 入力チェック
		StringBuilder sbErrorMessages = new StringBuilder();
		sbErrorMessages.Append(Validator.CheckNecessaryError(ReplaceTag("@@User.zip.name@@"), strZip));
		sbErrorMessages.Append(Validator.CheckHalfwidthNumberError(ReplaceTag("@@User.zip.name@@"), strZip));
		sbErrorMessages.Append(Validator.CheckLengthError(ReplaceTag("@@User.zip.name@@"), strZip, 7));

		//------------------------------------------------------
		// 入力チェックOKの場合、郵便番号検索実行＆セット
		//------------------------------------------------------
		if (sbErrorMessages.Length == 0)
		{
			sbErrorMessages.Append(
				SearchAddrFromZip(
					this.WtbUserZip1,
					this.WtbUserZip2,
					this.WddlUserAddr1,
					this.WtbUserAddr2,
					this.WtbUserZip));
		}

		//------------------------------------------------------
		// エラーメッセージ表示
		//------------------------------------------------------
		this.ZipInputErrorMessage = sbErrorMessages.ToString();
		if (this.ZipInputErrorMessage != "")
		{
			// 郵便番号検索ボックスにフォーカス
			this.WtbUserZip1.Focus();
		}
	}

	/// <summary>
	/// 確認するリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbConfirm_Click(object sender, EventArgs e)
	{
		// キャプチャ認証失敗時は処理終了
		if (CheckCaptcha() == false) return;

		// 入力情報取得
		UserInput userInput = CreateInputData();

		//------------------------------------------------------
		// 入力チェック＆重複チェック
		//------------------------------------------------------
		Dictionary<string, string> errorMessages = userInput.Validate(UserInput.EnumUserInputValidationKbn.MailMagazineRegist);
		if (errorMessages.Count != 0)
		{
			// カスタムバリデータ取得
			List<CustomValidator> lCustomValidators = new List<CustomValidator>();
			CreateCustomValidators(this, lCustomValidators);

			// エラーをカスタムバリデータへ
			SetControlViewsForError("MailMagazineRegist", errorMessages, lCustomValidators);

			return;
		}

		// グローバルOP：ONの場合は入力チェックしないため、後から詰める
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			userInput.NameKana1 = StringUtility.ToZenkaku(this.WtbUserNameKana1.Text);
			userInput.NameKana2 = StringUtility.ToZenkaku(this.WtbUserNameKana2.Text);
			userInput.NameKana = userInput.NameKana1 + userInput.NameKana2;
		}

		//------------------------------------------------------
		// 画面遷移
		//------------------------------------------------------
		// パラメタセット
		Session[Constants.SESSION_KEY_PARAM] = userInput;

		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_MAILMAGAZINE_REGIST_CONFIRM;

		// 画面遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_MAILMAGAZINE_REGIST_CONFIRM);
	}

	/// <summary>
	/// 入力値を取得してValueObjectに格納
	/// </summary>
	private UserInput CreateInputData()
	{
		// 「国」がなくて、グローバル対応しない場合は日本住所とする
		// ※グローバル対応の場合は入力する値のままで、全角に変換しない
		var isJpAddress = (Constants.GLOBAL_OPTION_ENABLE == false);

		UserInput userInput = new UserInput(new UserModel());
		userInput.UserKbn = Constants.FLG_USER_USER_KBN_MAILMAGAZINE;
		userInput.MailFlg = Constants.FLG_USER_MAILFLG_OK;
		userInput.Name1 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserName1.Text, isJpAddress);
		userInput.Name2 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserName2.Text, isJpAddress);
		userInput.Name = userInput.Name1 + userInput.Name2;
		
		// グローバルOP：ONの場合は、かなを入力チェックしない（後から詰める）
		if (Constants.GLOBAL_OPTION_ENABLE == false)
		{
			userInput.NameKana1 = StringUtility.ToZenkaku(this.WtbUserNameKana1.Text);
			userInput.NameKana2 = StringUtility.ToZenkaku(this.WtbUserNameKana2.Text);
			userInput.NameKana = userInput.NameKana1 + userInput.NameKana2;
		}
		
		userInput.NickName = this.WtbUserNickName.Text;
		userInput.BirthYear = this.WddlUserBirthYear.SelectedValue;
		userInput.BirthMonth = this.WddlUserBirthMonth.SelectedValue;
		userInput.BirthDay = this.WddlUserBirthDay.SelectedValue;
		// どれか未入力の時は日付整合性チェックは行わない
		if ((this.WddlUserBirthYear.SelectedValue != "")
			&& (this.WddlUserBirthMonth.SelectedValue != "")
			&& (this.WddlUserBirthDay.SelectedValue != ""))
		{
			userInput.Birth = this.WddlUserBirthYear.SelectedValue + "/" + this.WddlUserBirthMonth.SelectedValue + "/" + this.WddlUserBirthDay.SelectedValue;
		}
		else
		{
			userInput.Birth = null;
		}
		userInput.Sex = this.WrblUserSex.SelectedValue;
		userInput.MailAddr = StringUtility.ToHankaku(this.WtbUserMailAddr.Text);
		userInput.MailAddrConf = StringUtility.ToHankaku(this.WtbUserMailAddrConf.Text);
		userInput.MailAddr2 = string.Empty;
		// PCアドレス必須
		userInput.Zip1 = StringUtility.ToHankaku(this.WtbUserZip1.Text);
		userInput.Zip2 = StringUtility.ToHankaku(this.WtbUserZip2.Text);
		userInput.Zip = userInput.Zip1 + userInput.Zip2;
		userInput.Addr1 = this.WddlUserAddr1.SelectedValue;
		userInput.Addr2 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserAddr2.Text, isJpAddress).Trim();
		userInput.Addr3 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserAddr3.Text, isJpAddress).Trim();
		userInput.Addr4 = DataInputUtility.ConvertToFullWidthBySetting(this.WtbUserAddr4.Text, isJpAddress).Trim();
		userInput.Addr = userInput.ConcatenateAddress();
		userInput.CompanyName = this.WtbUserCompanyName.Text;
		userInput.CompanyPostName = this.WtbUserCompanyPostName.Text;
		userInput.Tel1_1 = StringUtility.ToHankaku(this.WtbUserTel1_1.Text);
		userInput.Tel1_2 = StringUtility.ToHankaku(this.WtbUserTel1_2.Text);
		userInput.Tel1_3 = StringUtility.ToHankaku(this.WtbUserTel1_3.Text);
		userInput.Tel1 = UserService.CreatePhoneNo(userInput.Tel1_1, userInput.Tel1_2, userInput.Tel1_3);
		userInput.Tel2_1 = StringUtility.ToHankaku(this.WtbUserTel2_1.Text);
		userInput.Tel2_2 = StringUtility.ToHankaku(this.WtbUserTel2_2.Text);
		userInput.Tel2_3 = StringUtility.ToHankaku(this.WtbUserTel2_3.Text);
		userInput.Tel2 = UserService.CreatePhoneNo(userInput.Tel2_1, userInput.Tel2_2, userInput.Tel2_3);
		userInput.MailFlg = Constants.FLG_USER_MAILFLG_OK;
		userInput.RemoteAddr = Request.ServerVariables["REMOTE_ADDR"];

		return userInput;
	}

	/// <summary>郵便番号入力チェックエラー文言</summary>
	protected string ZipInputErrorMessage
	{
		get { return StringUtility.ToEmpty(ViewState["ZipInputErrorMessage"]); }
		set { ViewState["ZipInputErrorMessage"] = value; }
	}
}
