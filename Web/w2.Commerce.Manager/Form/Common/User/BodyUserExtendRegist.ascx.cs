/*
=========================================================================================================
  Module      : ユーザ情報拡張項目編集系の出力コントローラ(BodyUserExtendRegist.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.Common.Util;
using w2.Domain.User;
using w2.Domain.User.Helper;

///
/// 入力時にのみ利用する
/// 表示時は、BodyUserConfirm.ascx でセッション内容を表示するのみ
///
public partial class Form_Common_User_BodyUserExtendRegist : BaseUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		this.UserId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USER_ID]);
		ViewState[Constants.REQUEST_KEY_USER_ID] = this.UserId;
		this.UniqueKey = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_UNIQUE_KEY]);

		// ユーザ拡張項目設定のうち、管理画面非表示のみ削除
		this.UserExtendSettingList = new UserExtendSettingList(
			true,
			Constants.FLG_USEREXTENDSETTING_DISPLAY_EC,
			null,
			this.LoginOperatorName);
		
		if (!IsPostBack)
		{
			InputDataBind();
		}
	}

	/// <summary>
	/// 情報のロード＆バインド(入力画面用)
	/// </summary>
	private void InputDataBind()
	{
		rUserExtendInput.DataSource = this.UserExtendSettingList;
		rUserExtendInput.DataBind();

		if ((this.UserExtendInputForBack != null)
			&& (this.UserExtendInputForBack.UserId == this.UserId))
		{
			SetInputData(this.UserExtendInputForBack.CreateModel());
		}
		else if (this.ActionStatus == Constants.ACTION_STATUS_INSERT)
		{
			if (this.UserExtendInputForBack != null)
			{
				SetInputData(this.UserExtendInputForBack.CreateModel());
			}
			else
			{
				SetInputData();
			}
		}
		else
		{
			SetInputData(new UserService().GetUserExtend(this.UserId));
		}

		this.UserExtendInputForBack = null;
	}

	/// <summary>
	/// ログインユーザに紐づくユーザ拡張項目(w2_UserExtend)から入力情報を取得し表示
	/// </summary>
	/// <param name="userExtend">ユーザー拡張項目</param>
	/// <remarks>
	/// ・管理側は確認画面から戻る際、ヒストリバック。
	/// ・登録時に存在しなかった項目などはデフォルト設定せず未選択状態にする。
	/// ・ユーザ拡張項目の値がアイテムリストに存在しない場合、入力方法に合わせ対応
	/// 　CHECK,RADIIO：、未選択状態
	/// 　DDL：空のアイテムリストを追加
	/// </remarks>
	private void SetInputData(UserExtendModel userExtend = null)
	{
		var userExtendModel = new UserService().GetUserExtend(this.UserId);

		foreach (RepeaterItem item in rUserExtendInput.Items)
		{
			string settingId = GetSettingId(item);
			switch (GetInputType(item))
			{
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
					TextBox tbSelect = GetInputText(item);
					tbSelect.Text = (CheckColumnExists(userExtend, settingId)
						&& (settingId != Constants.CROSS_POINT_USREX_SHOP_CARD_NO)
						&& (settingId != Constants.CROSS_POINT_USREX_SHOP_CARD_PIN))
						? userExtend.UserExtendDataValue[settingId]
						: string.Empty;
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
					DropDownList ddlSelect = GetDdlSelect(item);
					((List<ListItem>)(this.UserExtendSettingList.Items[item.ItemIndex].ListItemDefaultSelected)).ForEach(listitem => ddlSelect.Items.Add(listitem));
					if (CheckColumnExists(userExtend, settingId))
					{
						if (ddlSelect.Items.FindByValue((string)userExtend.UserExtendDataValue[settingId]) != null)
						{
							ddlSelect.SelectedIndex = -1;
							ddlSelect.Items.FindByValue((string)userExtend.UserExtendDataValue[settingId]).Selected = true;
						}

						// 管理側ではメルマガなど選択させたくない場合があるため「未選択」用項目がなければ空アイテム追加する
						if (ddlSelect.Items.FindByValue("") == null)
						{
							ddlSelect.Items.Insert(0, new ListItem("", ""));
						}
					}
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
					RadioButtonList rblSelect = GetRadioSelect(item);
					var listItems = (userExtend == null)
						? (List<ListItem>)this.UserExtendSettingList.Items[item.ItemIndex].ListItemForManagerInsert
						: (List<ListItem>)this.UserExtendSettingList.Items[item.ItemIndex].ListItemForManagerUpdate;
					listItems.ForEach(listitem => rblSelect.Items.Add(listitem));
					if (CheckColumnExists(userExtend, settingId))
					{
						if (rblSelect.Items.FindByValue((string)userExtend.UserExtendDataValue[settingId]) != null)
						{
							rblSelect.Items.FindByValue((string)userExtend.UserExtendDataValue[settingId]).Selected = true;
						}

						// 管理側ではメルマガなど選択させたくない場合があるため「未選択」用項目がなければ空アイテム追加する
						// アプリ会員フラグは「未選択」用項目を追加しない
						if ((rblSelect.Items.FindByValue("") == null)
							&& (settingId != Constants.CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG))
						{
							rblSelect.Items.Insert(0, new ListItem(ReplaceTag("@@DispText.common_message.unselected@@"), string.Empty));
						}

						// アプリ会員フラグは編集不可（閲覧は可能）
						if ((settingId == Constants.CROSS_POINT_USREX_SHOP_APP_MEMBER_FLAG)
							&& (this.ActionStatus == Constants.ACTION_STATUS_UPDATE))
						{
							rblSelect.Enabled = false;
						}
					}
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
					CheckBoxList cblSelect = GetCheckSelect(item);
					((List<ListItem>)(this.UserExtendSettingList.Items[item.ItemIndex].ListItemForManagerInsert)).ForEach(listitem => cblSelect.Items.Add(listitem));
					if (CheckColumnExists(userExtend, settingId))
					{
						string[] selectedItem = ((string)userExtend.UserExtendDataValue[settingId]).Split(',');
						foreach (ListItem listitem in cblSelect.Items)
						{
							listitem.Selected = selectedItem.Contains(listitem.Value);
						}
					}
					break;

				default:
					// なにもしない
					break;
			}
		}

	}

	/// <summary>
	/// 入力値を取得してUserExtendに格納
	/// </summary>
	public UserExtendInput CreateUserExtendFromInputData()
	{
		var userExtend = new UserExtendInput
		{
			UserId = this.UserId
		};

		foreach (RepeaterItem item in rUserExtendInput.Items)
		{
			string text = "";
			string data = "";
			switch (GetInputType(item))
			{
				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
					TextBox tbSelect = GetInputText(item);
					text = StringUtility.ToEmpty(tbSelect.Text);
					data = StringUtility.ToEmpty(tbSelect.Text);
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
					DropDownList ddlSelect = GetDdlSelect(item);
					text = (ddlSelect.SelectedItem != null) ? ddlSelect.SelectedItem.Text : "";
					data = (ddlSelect.SelectedItem != null) ? ddlSelect.SelectedItem.Value : "";
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
					RadioButtonList rblSelect = GetRadioSelect(item);
					text = (rblSelect.SelectedItem != null) ? rblSelect.SelectedItem.Text : "";
					data = (rblSelect.SelectedItem != null) ? rblSelect.SelectedItem.Value : "";
					break;

				case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
					CheckBoxList cblSelect = GetCheckSelect(item);
					foreach (ListItem listitem in cblSelect.Items)
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
	/// リピータ内のTextBoxコントロールを取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>コントロール</returns>
	private TextBox GetInputText(RepeaterItem item)
	{
		return (TextBox)item.FindControl("tbSelect");
	}
	
	/// <summary>
	/// リピータ内のDropDownListコントロールを取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>コントロール</returns>
	private DropDownList GetDdlSelect(RepeaterItem item)
	{
		return (DropDownList)item.FindControl("ddlSelect");
	}
	
	/// <summary>
	/// リピータ内のRadioButtonListコントロールを取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>コントロール</returns>
	private RadioButtonList GetRadioSelect(RepeaterItem item)
	{
		return (RadioButtonList)item.FindControl("rblSelect");
	}
	
	/// <summary>
	/// リピータ内のCheckBoxListコントロールを取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>コントロール</returns>
	private CheckBoxList GetCheckSelect(RepeaterItem item)
	{
		return (CheckBoxList)item.FindControl("cblSelect");
	}
	
	/// <summary>
	/// リピータ内のHiddenFieldコントロールの値取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>入力方法</returns>
	private string GetInputType(RepeaterItem item)
	{
		return ((HiddenField)item.FindControl("hfInputType")).Value;
	}
	
	/// <summary>
	/// リピータ内のHiddenFieldコントロールの値取得
	/// </summary>
	/// <param name="item">リピータアイテム</param>
	/// <returns>設定項目ID</returns>
	private string GetSettingId(RepeaterItem item)
	{
		return ((HiddenField)item.FindControl("hfSettingId")).Value;
	}
	
	/// <summary>
	/// ユーザ拡張項目IDの列が存在するか？
	/// </summary>
	/// <param name="userextend">ユーザ拡張項目オブジェクト</param>
	/// <param name="settingId">設定ID</param>
	/// <returns>true:存在する</returns>
	private bool CheckColumnExists(UserExtendModel userextend, string settingId)
	{
		return ((userextend != null) 
			&& userextend.UserExtendDataValue.ContainsKey(settingId)
			&& ((string)userextend.UserExtendDataValue[settingId] != ""));
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

	/// <summary>ユーザ拡張項目設定一覧</summary>
	public UserExtendSettingList UserExtendSettingList { get; private set; }
	/// <summary>ユーザ拡張項目</summary>
	public UserExtendInput UserExtend { get; set; }
	/// <summary>ユーザID</summary>
	private string UserId { get; set; }
	/// <summary>ユーザ拡張項目のバリデータXML</summary>
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
	/// <summary>ユーザー拡張項目入力値</summary>
	private UserExtendInput UserExtendInputForBack
	{
		get
		{
			if (this.IsBackFromConfirm == false) return null;

			var result = (UserExtendInput)Session[Constants.SESSION_KEY_PARAM_FOR_BACK + "_extend" + this.UniqueKey];
			return result;
		}
		set
		{
			Session[Constants.SESSION_KEY_PARAM_FOR_BACK + "_extend" + this.UniqueKey] = value;
		}
	}
	/// <summary>確認画面から戻ってきたか</summary>
	private bool IsBackFromConfirm
	{
		get { return Session[Constants.SESSION_KEY_PARAM_FOR_BACK + "_extend" + this.UniqueKey] != null; }
	}
	/// <summary>ユーザー拡張項目保存用ユニークキー</summary>
	private string UniqueKey
	{
		get
		{
			var result = StringUtility.ToEmpty(ViewState[Constants.REQUEST_KEY_UNIQUE_KEY]);
			return result;
		}
		set
		{
			ViewState[Constants.REQUEST_KEY_UNIQUE_KEY] = value;
		}
	}
}
