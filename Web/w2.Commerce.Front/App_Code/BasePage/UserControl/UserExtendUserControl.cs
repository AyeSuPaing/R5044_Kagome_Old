/*
=========================================================================================================
  Module      : ユーザ拡張項目出力コントローラ処理(UserExtendUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util;
using w2.Domain.User;
using w2.Domain.User.Helper;

public class UserExtendUserControl : BaseUserControl
{
	#region 定数・ラップの宣言
	public WrappedRepeater WrUserExtendInput { get { return GetWrappedControl<WrappedRepeater>("rUserExtendInput"); } }
	protected WrappedRepeater WrUserExtendDisplay { get { return GetWrappedControl<WrappedRepeater>("rUserExtendDisplay"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// ユーザ拡張項目設定のうち、登録/編集時に表示する項目のみ取得
		var cacheController = DataCacheControllerFacade.GetUserExtendSettingCacheController();
		this.UserExtendSettingList = cacheController.GetModifyUserExtendSettingList(this.HasRegist == false, Constants.FLG_USEREXTENDSETTING_DISPLAY_PC);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			this.UserExtendSettingList = NameTranslationCommon.SetUserExtendSettingTranslationData(
				this.UserExtendSettingList,
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);
		}

		if (Session[Constants.SESSION_KEY_IS_BACK_CROSS_POINT] == null)
		{
			SessionManager.IsBackForCrossPoint = false;
		}

		if (!IsPostBack)
		{
			// 入力画面か？
			if (this.HasInput)
			{
				InputDataBind();
			}
			else
			{
				ConfirmDataBind();
			}
		}
		else
		{
			// 入力画面か？
			if (this.HasInput)
			{
				// 入力値を取得してValueObjectに格納
				this.UserExtend = CreateUserExtendFromInputData();
				// バリデータチェック
				this.ErrorMsg = Validator.ValidateAndGetErrorContainer(Constants.FILE_NAME_XML_USEREXTEND, this.UserExtend.UserExtendDataValue);
			}
			else
			{
				// ターゲットページ設定
				Session[Constants.SESSION_KEY_TARGET_PAGE + "_extend"]
					= (this.HasRegist) ? Constants.PAGE_FRONT_USER_REGIST_INPUT : Constants.PAGE_FRONT_USER_MODIFY_INPUT;
			}
		}
	}

	/// <summary>
	/// 情報のロード＆バインド(入力画面用)
	/// </summary>
	private void InputDataBind()
	{
		this.WrUserExtendInput.DataSource = this.UserExtendSettingList;
		this.WrUserExtendInput.DataBind();

		// 確認画面からの戻りならセッション情報のValueObjectから復元
		if (CheckReturnFromConfirmPage())
		{
			SetUserExtendFromSession();
			Session[Constants.SESSION_KEY_TARGET_PAGE + "_extend"] = null;
		}
		// 確認画面以外から入力画面へ遷移
		else
		{
			if (this.HasRegist && (SessionManager.HasTemporaryUserId == false))
			{
				SetUserExtendFromDefault();
			}
			else
			{
				SetUserExtendFromLoginUser();
			}
		}

		SessionManager.IsBackForCrossPoint = false;
	}

	/// <summary>
	/// 情報のロード＆バインド(確認画面用)
	/// </summary>
	private void ConfirmDataBind()
	{
		// セッション情報のValueObjectからバインド
		this.UserExtend = ((UserInput)Session[Constants.SESSION_KEY_PARAM]).UserExtendInput;
		WrUserExtendDisplay.DataSource = this.UserExtend.UserExtendColumns
			.Where(columnName => (columnName != Constants.CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG))
			.ToList();
		WrUserExtendDisplay.DataBind();
	}

	#region +リピータ内処理
	/// <summary>
	/// ユーザ拡張項目設定(w2_UserExtendSetting)からデフォルト情報を取得し表示(会員登録時)
	/// </summary>
	public void SetUserExtendFromDefault()
	{
		foreach (RepeaterItem item in this.WrUserExtendInput.Items)
		{
			switch (GetInputType(item))
			{
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
					WrappedTextBox wtbSelect = GetWrappedInputText(item);
					var settingId = this.UserExtendSettingList.Items[item.ItemIndex].SettingId;
					if (string.IsNullOrEmpty(SessionManager.AppKeyForCrossPoint) == false)
					{
						if (settingId == Constants.CROSS_POINT_USREX_SHOP_CARD_NO)
						{
							wtbSelect.Text = StringUtility.ToEmpty(SessionManager.MemberIdForCrossPoint);
						}

						if (settingId == Constants.CROSS_POINT_USREX_SHOP_CARD_PIN)
						{
							wtbSelect.Text = StringUtility.ToEmpty(SessionManager.PinCodeForCrossPoint);
						}
					}
					else
					{
						wtbSelect.Text = ((string)(this.UserExtendSettingList.Items[item.ItemIndex].ListItemDefaultSelected));
					}
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
					WrappedDropDownList wddlSelect = GetWrappedDdlSelect(item);
					((List<ListItem>)(this.UserExtendSettingList.Items[item.ItemIndex].ListItemDefaultSelected)).ForEach(listitem => wddlSelect.Items.Add(listitem));
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
					WrappedRadioButtonList wrblSelect = GetWrappedRadioSelect(item);
					((List<ListItem>)(this.UserExtendSettingList.Items[item.ItemIndex].ListItemDefaultSelected)).ForEach(listitem => wrblSelect.Items.Add(listitem));
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
					WrappedCheckBoxList wcblSelect = GetWrappedCheckSelect(item);
					((List<ListItem>)(this.UserExtendSettingList.Items[item.ItemIndex].ListItemDefaultSelected)).ForEach(listitem => wcblSelect.Items.Add(listitem));
					break;

				default:
					// なにもしない
					break;
			}
		}

	}

	/// <summary>
	/// セッションからユーザ拡張項目の入力情報を取得し表示(確認画面からの戻り時)
	/// </summary>
	public void SetUserExtendFromSession()
	{
		var userExtendInput = this.IsLandingPage
			? (UserExtendInput)Session[Constants.SESSION_KEY_USER_EXTEND_INPUT]
			: ((UserInput)Session[Constants.SESSION_KEY_PARAM]).UserExtendInput;

		SetUserExtendFromUserExtendObject(userExtendInput);
	}
	
	/// <summary>
	/// ログインユーザに紐づくユーザ拡張項目(w2_UserExtend)から入力情報を取得し表示(会員編集時)
	/// </summary>
	private void SetUserExtendFromLoginUser()
	{
		var userExtendModel = new UserService().GetUserExtend(this.LoginUserId);
		SetUserExtendFromUserExtendObject(new UserExtendInput(userExtendModel));
	}

	/// <summary>
	/// ユーザ拡張項目オブジェクトから入力情報を取得し表示
	/// (存在しない項目の場合は拡張項目設定の内容からデフォルトをセット)
	/// </summary>
	/// <remarks>
	/// ・会員変更（または確認画面から戻ってきた際）ではデフォルト設定値を設定せず、ユーザが選択した値を設定する。
	/// ・登録時に存在しなかった項目などはデフォルト設定せず未選択状態にする。
	/// ・ユーザ拡張項目の値がアイテムリストに存在しない場合、入力方法に合わせ対応
	/// 　CHECK,RADIIO：、未選択状態
	/// 　DDL：空のアイテムリストを追加
	/// </remarks>
	private void SetUserExtendFromUserExtendObject(UserExtendInput userExtend)
	{
		foreach (RepeaterItem item in this.WrUserExtendInput.Items)
		{
			var settingId = GetSettingId(item);
			switch (GetInputType(item))
			{
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
					WrappedTextBox wtbSelect = GetWrappedInputText(item);
					wtbSelect.Text =
						userExtend.UserExtendDataValue.ContainsKey(settingId)
							&& ((settingId != Constants.CROSS_POINT_USREX_SHOP_CARD_NO)
									&& (SessionManager.HasTemporaryUserId == false)
								|| SessionManager.IsBackForCrossPoint)
							&& (settingId != Constants.CROSS_POINT_USREX_SHOP_CARD_PIN
								|| SessionManager.IsBackForCrossPoint)
							? StringUtility.ToEmpty(userExtend.UserExtendDataValue[settingId])
							: "";
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
					WrappedDropDownList wddlSelect = GetWrappedDdlSelect(item);
					((List<ListItem>)(this.UserExtendSettingList.Items[item.ItemIndex].ListItem)).ForEach(listitem => wddlSelect.Items.Add(listitem));
					if (userExtend.UserExtendDataValue.ContainsKey(settingId))
					{
						if (wddlSelect.Items.FindByValue(StringUtility.ToEmpty(userExtend.UserExtendDataValue[settingId])) != null)
						{
							wddlSelect.Items.FindByValue(StringUtility.ToEmpty(userExtend.UserExtendDataValue[settingId])).Selected = true;
						}
					}
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
					WrappedRadioButtonList wrblSelect = GetWrappedRadioSelect(item);
					((List<ListItem>)(this.UserExtendSettingList.Items[item.ItemIndex].ListItem)).ForEach(listitem => wrblSelect.Items.Add(listitem));
					if (userExtend.UserExtendDataValue.ContainsKey(settingId))
					{
						if (wrblSelect.Items.FindByValue(StringUtility.ToEmpty(userExtend.UserExtendDataValue[settingId])) != null)
						{
							wrblSelect.Items.FindByValue(StringUtility.ToEmpty(userExtend.UserExtendDataValue[settingId])).Selected = true;
						}
					}
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
					WrappedCheckBoxList wcblSelect = GetWrappedCheckSelect(item);
					((List<ListItem>)(this.UserExtendSettingList.Items[item.ItemIndex].ListItem)).ForEach(listitem => wcblSelect.Items.Add(listitem));
					if (userExtend.UserExtendDataValue.ContainsKey(settingId))
					{
						string[] selectedItem = (StringUtility.ToEmpty(userExtend.UserExtendDataValue[settingId])).Split(',');
						foreach (ListItem listitem in wcblSelect.Items)
						{
							listitem.Selected = selectedItem.Contains(listitem.Value);
						}
					}
					break;

				default:
					// ここに来るパターンは対応外のため、何もしないでスキップ
					break;
			}
		}
	}

	/// <summary>
	/// 入力値を取得してUserExtendに格納
	/// </summary>
	/// <rereturns>UserExtendオブジェクト</rereturns>
	private UserExtendInput CreateUserExtendFromInputData()
	{
		UserExtendInput userExtend = new UserExtendInput();
		foreach (RepeaterItem item in this.WrUserExtendInput.Items)
		{
			string text = "";
			string data = "";
			switch (GetInputType(item))
			{
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
					WrappedTextBox wtbSelect = GetWrappedInputText(item);
					text = StringUtility.ToEmpty(wtbSelect.Text);
					data = StringUtility.ToEmpty(wtbSelect.Text);
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
					WrappedDropDownList wddlSelect = GetWrappedDdlSelect(item);
					text = (wddlSelect.SelectedItem != null) ? wddlSelect.SelectedItem.Text : "";
					data = (wddlSelect.SelectedItem != null) ? wddlSelect.SelectedItem.Value : "";
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
					WrappedRadioButtonList wrblSelect = GetWrappedRadioSelect(item);
					text = (wrblSelect.SelectedItem != null) ? wrblSelect.SelectedItem.Text : "";
					data = (wrblSelect.SelectedItem != null) ? wrblSelect.SelectedItem.Value : "";
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
					WrappedCheckBoxList wcblSelect = GetWrappedCheckSelect(item);
					foreach (ListItem listitem in wcblSelect.Items)
					{
						if (listitem.Selected)
						{
							text += (text != "") ? "," : "";
							text += listitem.Text;
							data += (data != "") ? "," : "";
							data += listitem.Value;
						}
					}
					break;

				default:
					// なにもしない
					break;
			}

			string settingId = GetSettingId(item);
			// hiddenタグを削除した場合は拡張項目が更新対象にならないようにする。（user_id,date_created,date_changedのみ更新）
			if ((settingId != "") && (userExtend.UserExtendColumns.Contains(settingId) == false))
			{
				userExtend.UserExtendColumns.Add(settingId);
				userExtend.UserExtendDataText.Add(settingId, text);
				userExtend.UserExtendDataValue.Add(settingId, data);
			}
		}
		return userExtend;
	}
	
	/// <summary>
	/// リピータ毎の拡張項目設定ID取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>拡張項目設定ID</returns>
	private string GetSettingId(RepeaterItem item)
	{
		var whfSettingId = GetWrappedControl<WrappedHiddenField>(item, "hfSettingId");
		return whfSettingId.Value;
	}
	
	/// <summary>
	/// リピータ毎の入力方法取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>入力方法</returns>
	private string GetInputType(RepeaterItem item)
	{
		var whfInputType = GetWrappedControl<WrappedHiddenField>(item, "hfInputType");
		return whfInputType.Value;
	}
	
	/// <summary>
	/// リピータ毎のラップ済みテキストボックスコントロール取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>ラップ済みテキストボックス</returns>
	private WrappedTextBox GetWrappedInputText(RepeaterItem item)
	{
		return GetWrappedControl<WrappedTextBox>(item, "tbSelect");
	}
	
	/// <summary>
	/// リピータ毎のラップ済みドロップダウンリストコントロール取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>ラップ済みドロップダウンリスト</returns>
	private WrappedDropDownList GetWrappedDdlSelect(RepeaterItem item)
	{
		return GetWrappedControl<WrappedDropDownList>(item, "ddlSelect");
	}
	
	/// <summary>
	/// リピータ毎のラップ済みラジオボタンリストコントロール取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>ラップ済みラジオボタンリスト</returns>
	private WrappedRadioButtonList GetWrappedRadioSelect(RepeaterItem item)
	{
		var wrblSelect = GetWrappedControl<WrappedRadioButtonList>(item, "rblSelect");
		return wrblSelect;
	}
	
	/// <summary>
	/// リピータ毎のラップ済みチェックボックスリストコントロール取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>ラップ済みチェックボックスリスト</returns>
	private WrappedCheckBoxList GetWrappedCheckSelect(RepeaterItem item)
	{
		var wcblSelect = GetWrappedControl<WrappedCheckBoxList>(item, "cblSelect");
		return wcblSelect;
	}
	#endregion

	/// <summary>
	/// エラーメッセージを出力設定
	/// </summary>
	public void SetErrMessage()
	{
		foreach (RepeaterItem item in this.WrUserExtendInput.Items)
		{
			string settingId = GetSettingId(item);
			var wlbErrMessage = GetWrappedControl<WrappedLabel>(item, "lbErrMessage");
			wlbErrMessage.Text = this.ErrorMsg.ContainsKey(settingId) ? this.ErrorMsg[settingId] : "";
		}
	}

	/// <summary>
	/// 確認画面からの戻りか
	/// </summary>
	/// <returns>true：確認画面から戻ってきた</returns>
	public bool CheckReturnFromConfirmPage()
	{
		var checkSessionInputType = this.IsLandingPage
			? (Session[Constants.SESSION_KEY_USER_EXTEND_INPUT] is UserExtendInput)
			: (Session[Constants.SESSION_KEY_PARAM] is UserInput);
		var target = this.IsLandingPage
			? Constants.PAGE_FRONT_LANDING_LANDING_CART_INPUT
			: (this.HasRegist ? Constants.PAGE_FRONT_USER_REGIST_INPUT : Constants.PAGE_FRONT_USER_MODIFY_INPUT);

		return checkSessionInputType
			&& (this.Session[Constants.SESSION_KEY_TARGET_PAGE + "_extend"] != null)
			&& ((string)this.Session[Constants.SESSION_KEY_TARGET_PAGE + "_extend"] == target);
	}

	/// <summary>
	/// 概要取得(Text,Html判定）
	/// </summary>
	/// <param name="type">HTML区分（TEXT/HTML）</param>
	/// <param name="content">概要内容</param>
	/// <returns>区分に合わせた概要内容</returns>
	protected string GetUserExtendSettingOutLine(string type, string content)
	{
		if (type == Constants.FLG_USEREXTENDSETTING_OUTLINE_HTML)
		{
			// 相対パスを絶対パスに置換(aタグ、imgタグのみ）
			MatchCollection relativePath = Regex.Matches(content, "(a[\\s]+href=|img[\\s]+src=)([\"|']([^/|#][^\\\"':]+)[\"|'])", RegexOptions.IgnoreCase);
			foreach (Match match in relativePath)
			{
				Uri resourceUri = new Uri(HttpContext.Current.Request.Url, match.Groups[3].ToString());
				content = content.Replace(match.Groups[2].ToString(), "\"" + resourceUri.PathAndQuery + resourceUri.Fragment + "\"");
			}
			return content;
		}
		else if (type == Constants.FLG_USEREXTENDSETTING_OUTLINE_TEXT)
		{
			return WebSanitizer.HtmlEncodeChangeToBr(content);
		}

		throw new ArgumentException("パラメタエラー: outlineType is [" + type + "]");
	}

	/// <summary>
	/// 指定した拡張項目IDは必須項目か？
	/// </summary>
	/// <param name="settingId">拡張項目ID</param>
	/// <returns>true:必須項目</returns>
	protected bool IsNecessary(string settingId)
	{
		ValidatorXmlColumn columnValidation = this.UserExtendValidatorXml.FindColumn(settingId);
		return (columnValidation != null) ? (columnValidation.Necessary == "1") : false;
	}

	/// <summary>
	/// ユーザ拡張項目から入力情報を取得し表示
	/// (存在しない項目の場合は拡張項目設定の内容からデフォルトをセット)
	/// </summary>
	/// <param name="userExtend">ユーザー拡張項目</param>
	public void SetUserExtendFromUserExtendInput(UserExtendInput userExtend)
	{
		foreach (RepeaterItem item in this.WrUserExtendInput.Items)
		{
			var settingId = GetSettingId(item);
			switch (GetInputType(item))
			{
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
					WrappedTextBox wtbSelect = GetWrappedInputText(item);
					if (userExtend.UserExtendDataValue.ContainsKey(settingId))
					{
						wtbSelect.Text = StringUtility.ToEmpty(userExtend.UserExtendDataValue[settingId]);
					}
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
					WrappedDropDownList wddlSelect = GetWrappedDdlSelect(item);
					((List<ListItem>)(this.UserExtendSettingList.Items[item.ItemIndex].ListItem)).ForEach(listitem => wddlSelect.Items.Add(listitem));
					if (userExtend.UserExtendDataValue.ContainsKey(settingId))
					{
						wddlSelect.SelectedText = StringUtility.ToEmpty(userExtend.UserExtendDataValue[settingId]);
						wddlSelect.SelectedValue = StringUtility.ToEmpty(userExtend.UserExtendDataValue[settingId]);
						wddlSelect.SelectItemByValue(StringUtility.ToEmpty(userExtend.UserExtendDataValue[settingId]));
					}
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
					WrappedRadioButtonList wrblSelect = GetWrappedRadioSelect(item);
					((List<ListItem>)(this.UserExtendSettingList.Items[item.ItemIndex].ListItem)).ForEach(listitem => wrblSelect.Items.Add(listitem));
					if (userExtend.UserExtendDataValue.ContainsKey(settingId))
					{
						wrblSelect.SelectedText = StringUtility.ToEmpty(userExtend.UserExtendDataValue[settingId]);
						wrblSelect.SelectedValue = StringUtility.ToEmpty(userExtend.UserExtendDataValue[settingId]);
					}
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
					WrappedCheckBoxList wcblSelect = GetWrappedCheckSelect(item);
					((List<ListItem>)(this.UserExtendSettingList.Items[item.ItemIndex].ListItem)).ForEach(listitem => wcblSelect.Items.Add(listitem));
					if (userExtend.UserExtendDataValue.ContainsKey(settingId))
					{
						wcblSelect.SelectedText = (userExtend.UserExtendDataValue[settingId]);
						wcblSelect.SelectedValue = (userExtend.UserExtendDataValue[settingId]);
						wcblSelect.SelectItemByValue(StringUtility.ToEmpty(userExtend.UserExtendDataValue[settingId]));
					}
					break;

				default:
					// ここに来るパターンは対応外のため、何もしないでスキップ
					break;
			}
		}
	}

#region プロパティ
/// <summary>ユーザ拡張項目設定一覧</summary>
public UserExtendSettingList UserExtendSettingList { get; set; }
	/// <summary>ユーザ拡張項目</summary>
	public UserExtendInput UserExtend { get; set; }
	/// <summary>バリデータ内容</summary>
	public Dictionary<string, string> ErrorMsg { get; set; }
	/// <summary>入力画面か？</summary>
	public bool HasInput { get { return m_HasInput; } set { m_HasInput = value; } }
	private bool m_HasInput = true;
	/// <summary>会員登録か？</summary>
	public bool HasRegist { get { return m_HasRegist; } set { m_HasRegist = value; } }
	private bool m_HasRegist = true;
	/// <summary>LandingPageかどうか</summary>
	public bool IsLandingPage { get; set; }
	/// <summary>既存ユーザー</summary>
	public UserModel ExistingUser { get; set; }
	/// <summary>ユーザ拡張項目バリデータ情報</summary>
	private ValidatorXml UserExtendValidatorXml
	{
		get
		{
			if (m_userExtendValidatorXml == null)
			{
				m_userExtendValidatorXml = new ValidatorXml(Constants.PHYSICALDIRPATH_FRONT_PC + Constants.PHYSICALPATH_FILE_XML_USEREXTEND, Constants.FILE_NAME_XML_USEREXTEND);
			}
			return m_userExtendValidatorXml;
		}
	}
	private ValidatorXml m_userExtendValidatorXml;
	#endregion
}
