/*
=========================================================================================================
  Module      : マスタ出力定義登録ページ処理(MasterExportSettingRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.MasterExport;
using w2.Domain.MasterExportSetting;
using w2.Domain.User.Helper;

public partial class Form_MasterExportSetting_MasterExportSettingRegister : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents();

			//------------------------------------------------------
			// マスタ出力定義登録表示
			//------------------------------------------------------
			ViewMasterExportSettingRegister();
		}
	}

	#region -マスタ出力表示
	/// <summary>
	/// マスタ出力定義登録表示
	/// </summary>
	private void ViewMasterExportSettingRegister()
	{
		//------------------------------------------------------
		// リクエスト情報取得
		//------------------------------------------------------
		Hashtable htParam = GetParameters(Request);

		// 不正パラメータが存在した場合エラーページへ
		if ((bool)htParam[Constants.ERROR_REQUEST_PRAMETER])
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		if (Session[Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID] != null)
		{
			ddlSelectSetting.SelectedIndex = (int)Session[Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID];
			Session[Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID] = null;
		}
		else
		{
			btnDeleteTop.Enabled = false;
		}

		// マスタ区分取得
		string masterKbn = (string)htParam[Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN];

		// マスタ出力定義情報取得
		var masterExportSettings = new MasterExportSettingService().GetAllByMaster(this.LoginOperatorShopId, masterKbn);

		// 該当データが存在する場合
		if (masterExportSettings.Length > 0)
		{
			// マスタ出力定義情報をビューステートに保存
			var settings = masterExportSettings[ddlSelectSetting.SelectedIndex];
			ViewState[Constants.SESSIONPARAM_KEY_MASTEREXPORTSETTING_INFO] = settings.DataSource;

			// フィールド列設定
			tbFields.Text = StringUtility.ToEmpty(settings.Fields).Replace(",", ",\n");

			// マスタ出力形式の値設定
			ddlExportFileType.SelectedValue = settings.ExportFileType;

			// 更新ボタン表示
			btnUpdateTop.Visible = true;
			btnDeleteTop.Visible = true;
			btnDeleteTop.Enabled = ddlSelectSetting.SelectedIndex != 0;
		}
		else
		{
			ddlSelectSetting.Visible = false;
			tbSettingName.Enabled = false;
			tbSettingName.Text = rblMasterKbn.SelectedItem + Constants.MASTEREXPORTSETTING_MASTER_UNREMOVABLE;

		}
		btnRegisterTop.Visible = true;

		// マスタフィールド取得
		List<Hashtable> alMasterField = MasterExportSettingUtility.GetMasterExportSettingFieldList(masterKbn);

		// マスタ区分がユーザーマスタの場合
		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA)
		{
			// Extend title
			var settingExtendsTitleName = MasterExportSettingUtility.GetMasterExportSettingFieldList(
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_EXTEND_TITLE);
			var extendTitleName = settingExtendsTitleName[0][Constants.MASTEREXPORTSETTING_XML_J_NAME];

			var userExtendSettings = new UserExtendSettingList(this.LoginOperatorId);
			foreach (var userExtendSetting in userExtendSettings.Items)
			{
				Hashtable userExtend = new Hashtable();
				userExtend.Add(Constants.MASTEREXPORTSETTING_XML_NAME, userExtendSetting.SettingId);
				userExtend.Add(Constants.MASTEREXPORTSETTING_XML_J_NAME, string.Format(
					StringUtility.ToEmpty(extendTitleName),
					(userExtendSetting.SettingName != string.Empty)
						? userExtendSetting.SettingName
						: userExtendSetting.SettingId));

				alMasterField.Add(userExtend);
			}

			// ユーザー属性
			var userAttributeFields = MasterExportSettingUtility.GetMasterExportSettingFieldList(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA_USERATTRIBUTE);
			alMasterField.AddRange(userAttributeFields);
		}

		// データソース設定
		rList.DataSource = MasterFieldSetting.RemoveMasterFields(alMasterField, masterKbn);
		// データバインド
		rList.DataBind();
	}

	/// <summary>
	///　マスタ出力定義登録パラメタ取得
	/// </summary>
	/// <param name="hrRequest">マスタ出力定義登録のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters(HttpRequest hrRequest)
	{
		Hashtable result = new Hashtable();
		string masterKbn = String.Empty;
		bool paramError = false;

		try
		{
			switch (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN]))
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_AFFILIATECOOPLOG:
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA:
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE:
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE_MEDIA_TYPE:
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT_DETAIL:
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON:
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERCOUPON:
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON_USE_USER:
					masterKbn = hrRequest[Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN].ToString();
					break;

				case "":
					masterKbn = (Constants.MARKETINGPLANNER_MULTIPURPOSE_AFFILIATE_OPTION_ENABLE ? Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_AFFILIATECOOPLOG :
						Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA);
					break;

				default:
					paramError = true;
					break;
			}
			result.Add(Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN, masterKbn);
			foreach (ListItem li in rblMasterKbn.Items)
			{
				li.Selected = (li.Value == masterKbn);
			}
		}
		catch
		{
			paramError = true;
		}

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		result.Add(Constants.ERROR_REQUEST_PRAMETER, paramError);

		return result;
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// マスタ種別ラジオボタンリスト作成
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MASTEREXPORTSETTING, Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN))
		{
			bool enable = true;
			switch (li.Value)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_AFFILIATECOOPLOG: // 汎用アフィリエイトOP
					enable = Constants.MARKETINGPLANNER_MULTIPURPOSE_AFFILIATE_OPTION_ENABLE;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE:				// 広告コード
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE_MEDIA_TYPE:	// 広告媒体区分マスタ
					enable = Constants.W2MP_AFFILIATE_OPTION_ENABLED;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT: // ユーザーポイント
					enable = false;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT_DETAIL: // ユーザーポイント(詳細)
					enable = Constants.MARKETINGPLANNER_POINT_OPTION_ENABLE;
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON: // クーポンマスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERCOUPON: // ユーザクーポンマスタ
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON_USE_USER: // クーポン利用ユーザー
					enable = Constants.W2MP_COUPON_OPTION_ENABLED;
					break;

			}

			if (enable == false) continue;

			rblMasterKbn.Items.Add(li);
		}

		Hashtable input = GetParameters(Request);
		// 不正パラメータが存在した場合エラーページへ
		if ((bool)input[Constants.ERROR_REQUEST_PRAMETER])
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 出力ファイル形式ドロップダウン
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MASTEREXPORTSETTING, Constants.FIELD_MASTEREXPORTSETTING_EXPORT_FILE_TYPE))
		{
			ddlExportFileType.Items.Add(li);
		}

		foreach (var exportSetting in new MasterExportSettingService().GetAllByMaster(this.LoginOperatorShopId, rblMasterKbn.SelectedValue))
		{
			ddlSelectSetting.Items.Add(
				new ListItem(
					(exportSetting.SettingId == Constants.MASTEREXPORTSETTING_MASTER_SETTING_ID)
						? rblMasterKbn.SelectedItem + Constants.MASTEREXPORTSETTING_MASTER_UNREMOVABLE
						: exportSetting.SettingName,
					exportSetting.SettingId));
		}

		rblMasterKbn.DataBind();
		rblMasterKbn.SelectedIndex = 0;

		// 完了画面の場合
		if (StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ACTION_STATUS]) == Constants.ACTION_STATUS_COMPLETE)
		{
			divComp.Visible = true;
			Session[Constants.SESSION_KEY_ACTION_STATUS] = null;
		}
	}

	#endregion

	#region -マスタ出力 登録/更新/削除
	/// <summary>
	/// 出力設定登録・更新
	/// </summary>
	/// <param name="isRegister">登録アクション</param>
	private void InsertUpdateExportSetting(bool isRegister)
	{
		string shopId = this.LoginOperatorShopId;

		// パラメタ格納
		var input = GetExportSettingInfo(shopId);
		if (isRegister)
		{
			input.SettingName = tbSettingName.Text.Trim();
		}
		else
		{
			input.SettingId = ddlSelectSetting.SelectedValue;
		}

		// 入力チェック
		var errorMessages = input.Validate(isRegister);
		if (errorMessages != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// フィールド列チェック
		if (CheckMasterFields(shopId, input.Fields, input.MasterKbn) == false)
		{
			string errorInfo = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTEREXPORTSETTING_FIELDS_ERROR);
			if (this.ErrorFieldName.Length != 0)
			{
				errorInfo += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIELD_NOT_APPLICABLE)
					.Replace("@@ 1 @@", WebSanitizer.HtmlEncode(this.ErrorFieldName.ToString()));
			}

			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorInfo;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		var model = input.CreateModel();
		if (isRegister)
		{
			new MasterExportSettingService().Insert(model);
			LoadSettingList(rblMasterKbn.SelectedValue, ddlSelectSetting.Items.Count);
		}
		else
		{
			new MasterExportSettingService().Update(model);
		}

		// 完了メッセージ表示用パラメータ設定
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;

		// 登録画面へ戻る
		Session[Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID] = ddlSelectSetting.SelectedIndex;
		Response.Redirect(CreateMasterExportSettingRegiestUrl(input.MasterKbn));
	}

	/// <summary>
	/// フィールドチェック
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="namesCsv">名前列</param>
	/// <param name="masterKbn">マスタ区分</param>
	/// <returns>フィールドチェック結果</returns>
	private bool CheckMasterFields(string shopId, string namesCsv, string masterKbn)
	{
		// フィールド名変換 ※名称からフィールド名を取得できるようHashtable作成
		var fields = MasterExportSettingUtility.GetMasterExportSettingFields(masterKbn);

		// マスタ区分がユーザーマスタの場合、ユーザー拡張項目＆ユーザー属性取得
		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA)
		{
			// 名称からフィールド名を取得できるようHashtable作成
			UserExtendSettingList userExtendSettingList = new UserExtendSettingList(this.LoginOperatorId);
			userExtendSettingList.Items.ForEach(userExtendSetting =>
				fields.Add(userExtendSetting.SettingId, Constants.TABLE_USEREXTEND + "." + userExtendSetting.SettingId));

			// ユーザー属性
			var userAttributeFields = MasterExportSettingUtility.GetMasterExportSettingFields(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA_USERATTRIBUTE);
			foreach (var key in userAttributeFields.Keys)
			{
				fields[key] = userAttributeFields[key];
			}
		}

		// リアルフィールド作成
		var names = StringUtility.SplitCsvLine(namesCsv);
		var missedField = names.FirstOrDefault(name => (fields.ContainsKey(name) == false));
		if (missedField != null)
		{
			if (this.ErrorFieldName.Length > 0) this.ErrorFieldName.Append(", ");
			this.ErrorFieldName.Append(missedField);
			return false;
		}

		// フィールドチェック
		var sqlFieldNames = string.Join(",", names.Select(name => (string)fields[name]));
		var result = MasterExportSettingUtility.CheckFieldsExists(shopId, masterKbn, sqlFieldNames);
		return result;
	}


	/// <summary>
	/// 出力設定情報取得
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <returns>入力設定情報</returns>
	private MasterExportSettingInput GetExportSettingInfo(string shopId)
	{
		var input = new MasterExportSettingInput
		{
			ShopId = shopId,
			MasterKbn = rblMasterKbn.SelectedValue,
			Fields = MasterExportSettingUtility.GetFieldsEscape(tbFields.Text),
			LastChanged = this.LoginOperatorName,
			ExportFileType = ddlExportFileType.SelectedValue,
		};
		return input;
	}

	#endregion

	#region イベント
	/// <summary>
	/// 登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegisterTop_Click(object sender, EventArgs e)
	{
		InsertUpdateExportSetting(true);
	}

	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateTop_Click(object sender, EventArgs e)
	{
		InsertUpdateExportSetting(false);
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteTop_Click(object sender, EventArgs e)
	{
		new MasterExportSettingService().Delete(
			this.LoginOperatorShopId,
			rblMasterKbn.SelectedValue,
			ddlSelectSetting.SelectedValue);

		LoadSettingList(rblMasterKbn.SelectedValue, ddlSelectSetting.SelectedIndex - 1);

		// 登録画面へ戻る
		Session[Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID] = ddlSelectSetting.SelectedIndex;
		Response.Redirect(CreateMasterExportSettingRegiestUrl(rblMasterKbn.SelectedValue));
	}

	/// <summary>
	/// マスタ種別変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblMasterKbn_SelectedIndexChanged1(object sender, EventArgs e)
	{
		// マスタ種別を設定し、再度読込
		Response.Redirect(CreateMasterExportSettingRegiestUrl(rblMasterKbn.SelectedValue));
	}

	/// <summary>
	/// マスタ出力設定変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlSelectSetting_SelectedIndexChanged(object sender, EventArgs e)
	{
		btnDeleteTop.Enabled = (ddlSelectSetting.SelectedItem.Value != Constants.MASTEREXPORTSETTING_MASTER_SETTING_ID);
		tbSettingName.Text = string.Empty;
		LoadField();
	}
	#endregion

	/// <summary>
	/// 設定一覧ロード
	/// </summary>
	/// <param name="masterKbn">マスタ区分</param>
	/// <param name="index">選択インデックス</param>
	private void LoadSettingList(string masterKbn, int index)
	{
		ddlSelectSetting.Items.Clear();
		foreach (var setting in new MasterExportSettingService().GetAllByMaster(this.LoginOperatorShopId, masterKbn))
		{
			ddlSelectSetting.Items.Add(
				new ListItem(
					(setting.SettingId == Constants.MASTEREXPORTSETTING_MASTER_SETTING_ID)
						? rblMasterKbn.SelectedItem + Constants.MASTEREXPORTSETTING_MASTER_UNREMOVABLE
						: setting.SettingName,
					setting.SettingId));
		}
		ddlSelectSetting.SelectedIndex = index;
	}

	/// <summary>
	/// データバインド用マスタ出力情報詳細URL作成
	/// </summary>
	/// <param name="masterKbn">マスタ区分</param>
	/// <returns>マスタ出力情報URL</returns>
	private string CreateMasterExportSettingRegiestUrl(string masterKbn)
	{
		string result = "";
		result += Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MASTEREXPORTSETTING_REGISTER;
		result += "?";
		result += Constants.REQUEST_KEY_MASTEREXPORTSETTING_MASTER_KBN + "=" + HttpUtility.UrlEncode(masterKbn);

		return result;
	}

	/// <summary>
	/// フィールドロード
	/// </summary>
	private void LoadField()
	{
		var input = new MasterExportSettingService()
			.GetAllByMaster(this.LoginOperatorShopId, rblMasterKbn.SelectedValue)[ddlSelectSetting.SelectedIndex];
		ddlExportFileType.SelectedValue = input.ExportFileType;
		tbFields.Text = StringUtility.ToEmpty(input.Fields).Replace(",", ",\n");
	}

	#region プロパティ
	/// <summary>エラーがあったフィールド</summary>
	public StringBuilder ErrorFieldName
	{
		get { return m_errorFieldName; }
		set { m_errorFieldName = value; }
	}
	private StringBuilder m_errorFieldName = new StringBuilder();
	#endregion
}
