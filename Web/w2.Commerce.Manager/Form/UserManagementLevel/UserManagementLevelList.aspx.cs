/*
=========================================================================================================
  Module      : ユーザー管理レベル登録ページ処理(userManagementLevelRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.Domain.User;
using w2.Domain.UserManagementLevel;

public partial class Form_UserManagementLevel_UserManagementLevelList : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 初期表示
		if (!IsPostBack)
		{
			List<UserManagementLevelInput> userManagementLevelInputModels;
			if (Session[SESSIONPARAM_KEY_USERMANAGEMENTLEVEL_INFO] == null)
			{
				var models =
					new UserManagementLevelService().GetAllListSeqSort(UserManagementLevelService.SortType.Desc);
				userManagementLevelInputModels = models.Select(m => new UserManagementLevelInput(m)).ToList();
			}
			else
			{
				userManagementLevelInputModels =
					(List<UserManagementLevelInput>)Session[SESSIONPARAM_KEY_USERMANAGEMENTLEVEL_INFO];
			}

			this.UserManagementLevelCount = userManagementLevelInputModels.Count;

			InitializeComponents();

			rUserManagementLevelList.DataSource = userManagementLevelInputModels;
			rUserManagementLevelList.DataBind();
		}
	}

	/// <summary>
	/// 一括更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAllUpdate_Click(object sender, EventArgs e)
	{
		var userManagementLevelList = GetInputUserManagementLevel(true);
		var errorMessages = "";

		// 入力チェック
		foreach (var userManagementLevel in userManagementLevelList)
		{
			// 新規登録のユーザー管理レベルのみチェック
			if (string.IsNullOrEmpty(userManagementLevel.UserManagementLevelId)) continue;

			errorMessages += userManagementLevel.Validate();
		}

		if (errorMessages.Length != 0) RedirectErrorPage(errorMessages);

		// 重複チェック
		var lUserManagementLevelList = new List<string>();
		foreach (var userManagementLevel in userManagementLevelList)
		{
			if (string.IsNullOrEmpty(userManagementLevel.UserManagementLevelId)) continue;

			if (lUserManagementLevelList.ConvertAll(s => s.ToLower()).Contains(userManagementLevel.UserManagementLevelId.ToLower()))
			{
				errorMessages += "ユーザー管理レベルID[" + userManagementLevel.UserManagementLevelId + "]が重複しています。<br />";
				WebMessages.GetMessages(
						WebMessages.ERRMSG_MANAGER_USER_MANAGER_LEVEL_ID_DUPLICATED)
					.Replace("@@ 1 @@", StringUtility.ToEmpty(userManagementLevel.UserManagementLevelId));
			}
			else
			{
				lUserManagementLevelList.Add(userManagementLevel.UserManagementLevelId);
			}
		}

		if (errorMessages.Length != 0) RedirectErrorPage(errorMessages);

		var service = new UserManagementLevelService();

		var userService = new UserService();

		// 使用チェック
		foreach (var userManagementLevel in userManagementLevelList)
		{
			if (string.IsNullOrEmpty(userManagementLevel.SeqNo)
				|| ((userManagementLevel.DelFlg != UserManagementLevelInput.FLG_DELETE_VALID))) continue;

			var useUserCount = userService.UserManagementLevelUserCount(userManagementLevel.UserManagementLevelId);

			if (useUserCount != 0)
			{
				errorMessages += errorMessages.Length == 0 ? "ユーザー管理レベル情報を削除できません。<br/>" : "";
				errorMessages += "ユーザー管理レベルID[" + userManagementLevel.UserManagementLevelId + "]は、ユーザーに設定されています。<br />";
				errorMessages += (errorMessages.Length == 0)
					? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_UNABLE_DELETE_USER_MANAGER)
					: string.Empty;
				errorMessages += StringUtility.ChangeToBrTag(WebMessages.GetMessages(
						WebMessages.ERRMSG_MANAGER_UNABLE_DELETE_USER_MANAGER)
					.Replace("@@ 1 @@", StringUtility.ToEmpty(userManagementLevel.UserManagementLevelId)));

			}
		}

		if (errorMessages.Length != 0) RedirectErrorPage(errorMessages);

		// ユーザー管理レベルの登録
		foreach (var userManagementLevel in GetInputUserManagementLevel())
		{
			// 変更なし
			if ((userManagementLevel.UserManagementLevelName == userManagementLevel.UserManagementLevelNameOld)
				&& (userManagementLevel.DisplayOrder == userManagementLevel.DisplayOrderOld)
				&& (userManagementLevel.DelFlg == userManagementLevel.DelFlgOld))
			{
				continue;
			}

			var model = userManagementLevel.CreateModel();
			if (string.IsNullOrEmpty(userManagementLevel.SeqNo) == false)
			{
				if ((userManagementLevel.DelFlg == UserManagementLevelInput.FLG_DELETE_VALID))
				{
					service.Delete(model.UserManagementLevelId);
				}
				else
				{
					service.Update(model);
				}
			}
			else if (string.IsNullOrEmpty(userManagementLevel.UserManagementLevelId) == false)
			{
				service.Insert(model);
			}
		}

		//------------------------------------------------------
		// 正常遷移
		//------------------------------------------------------
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_LIST);
	}

	/// <summary>
	/// 追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAdd_Click(object sender, EventArgs e)
	{
		// データ作成
		var userManagementLevelInputModels = GetInputUserManagementLevel();
		userManagementLevelInputModels.Add(new UserManagementLevelInput());
		this.UserManagementLevelCount += 1;

		// コンポーネント初期化
		InitializeComponents();

		// データバインド
		rUserManagementLevelList.DataSource = userManagementLevelInputModels;
		rUserManagementLevelList.DataBind();
	}

	/// <summary>
	/// キャンセルボタンクリック ※DB未登録情報をHashtableから削除
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCancel_Click(object sender, EventArgs e)
	{
		// 該当情報の削除
		var userManagementLevelInputModels = GetInputUserManagementLevel();
		var iIndex = 0;
		if (int.TryParse(StringUtility.ToEmpty((((LinkButton)sender).CommandArgument)), out iIndex))
		{
			userManagementLevelInputModels.Remove(userManagementLevelInputModels[iIndex]);
			this.UserManagementLevelCount -= 1;
		}

		// コンポーネント初期化
		InitializeComponents();

		// データバインド
		rUserManagementLevelList.DataSource = userManagementLevelInputModels;
		rUserManagementLevelList.DataBind();
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 設定情報が1件以上なら表示
		btnAllUpdateTop.Visible = btnAllUpdateBottom.Visible = (this.UserManagementLevelCount != 0);

		// 登録/更新完了メッセージ表示制御
		dvComplete.Visible = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ACTION_STATUS])
			== Constants.ACTION_STATUS_COMPLETE;
		Session[Constants.SESSION_KEY_ACTION_STATUS] = null;

		Session[SESSIONPARAM_KEY_USERMANAGEMENTLEVEL_INFO] = null;
	}

	/// <summary>
	/// 画面からデータ取得
	/// </summary>
	/// <param name="check"></param>
	/// <returns>ユーザー管理レベル情報</returns>
	protected List<UserManagementLevelInput> GetInputUserManagementLevel(bool check = false)
	{
		var userManagementLevelList = new List<UserManagementLevelInput>();
		foreach (RepeaterItem ri in rUserManagementLevelList.Items)
		{
			var temp = new UserManagementLevelInput()
			{
				SeqNo = StringUtility.ToEmpty(((HiddenField)ri.FindControl("hfSeqNo")).Value),
				UserManagementLevelName =
					StringUtility.ToEmpty(((TextBox)ri.FindControl("tbUserManagementLevelName")).Text),
				UserManagementLevelNameOld =
					StringUtility.ToEmpty(((HiddenField)ri.FindControl("hfUserManagementLevelName_old")).Value),
				DelFlg = (((CheckBox)ri.FindControl("cbDeleteFlg")).Checked)
					? UserManagementLevelInput.FLG_DELETE_VALID
					: UserManagementLevelInput.FLG_DELETE_INVALID,
				DelFlgOld = StringUtility.ToEmpty(((HiddenField)ri.FindControl("hfDelFlg_old")).Value),
				LastChanged = this.LoginOperatorName
			};

			temp.UserManagementLevelId = (string.IsNullOrEmpty(temp.SeqNo))
				? StringUtility.ToEmpty(((TextBox)ri.FindControl("tbUserManagementLevelId")).Text).Trim()
				: StringUtility.ToEmpty(((HiddenField)ri.FindControl("hfUserManagementLevelId")).Value);

			if (check && IsDefaultUserManagementLevel(temp.UserManagementLevelId))
			{
				temp.DisplayOrder = temp.DisplayOrderOld = UserManagementLevelInput.FLG_DELETE_VALID;
			}
			else
			{
				temp.DisplayOrder = StringUtility.ToEmpty(((TextBox)ri.FindControl("tbDisplayOrder")).Text);
				temp.DisplayOrderOld = StringUtility.ToEmpty(((HiddenField)ri.FindControl("hfDisplayOrder_old")).Value);
			}

			userManagementLevelList.Add(temp);
		}

		return userManagementLevelList;
	}

	/// <summary>
	/// エラーページへ遷移
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	private void RedirectErrorPage(string errorMessage)
	{
		Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
		Session[SESSIONPARAM_KEY_USERMANAGEMENTLEVEL_INFO] = GetInputUserManagementLevel();
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
	}

	/// <summary>
	/// デフォルトユーザー管理レベルチェック
	/// </summary>
	/// <param name="userManagementLevel">ユーザー管理レベル</param>
	/// <returns></returns>
	protected bool IsDefaultUserManagementLevel(string userManagementLevel)
	{
		var result = userManagementLevel == Constants.FLG_USER_USER_MANAGEMENT_LEVEL_NORMAL;
		return result;
	}

	/// <summary>ユーザー管理レベルの件数</summary>
	protected int UserManagementLevelCount
	{
		get { return (int)ViewState["UserManagementLevelCount"]; }
		private set { ViewState["UserManagementLevelCount"] = value; }
	}
	/// <summary>セッションキー：ユーザー管理レベル情報</summary>
	protected const string SESSIONPARAM_KEY_USERMANAGEMENTLEVEL_INFO = "usermanagementlevel_info";
}