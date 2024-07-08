/*
=========================================================================================================
  Module      : 設定情報ページ処理(SiteConfiguration.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Extensions;
using w2.Common.Web;

public partial class Form_Configuration : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 許可されたIPアドレスか
			if (IsAllowedIpAddress(Constants.ALLOWED_IP_ADDRESS_FOR_SYSTEMSETTINGS) == false)
			{
				Response.Redirect(Constants.PAGE_MANAGER_ERROR);
			}

			// GUID生成
			if (string.IsNullOrEmpty(Request.QueryString[Constants.PARAM_GUID_STRING]))
			{
				var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SITE_CONFIGRATION)
					.AddParam(Constants.PARAM_GUID_STRING, this.GuidString)
					.CreateUrl();

				Response.Redirect(url);
			}

			// コンポーネント初期化
			InitializeComponentsForDisplay();

			var defaultReadKbn = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_SITE_CONFIGURATION,
				Constants.VALUETEXT_PARAM_SITE_CONFIGURATION_READ_KBN,
				Constants.VALUETEXT_PARAM_SITE_CONFIGURATION_READ_KBN_NAME);

			ddlTabs.Items.Add(new ListItem(defaultReadKbn, "#tabs-0"));
			for (int i = 0; i < this.ReadKbnList.Length; i++)
			{
				ddlTabs.Items.Add(new ListItem(this.ReadKbnList[i], "#tabs-" + (i + 1)));
			}

			// リクエスト取得
			var actionStatus = StringUtility.ToEmpty(Request.QueryString[Constants.REQUEST_KEY_ACTION_STATUS]);

			// 表示非表示制御
			switch (actionStatus)
			{
				case Constants.ACTION_STATUS_UPDATE:
					this.IsDisplayOnly = false;
					btnConfirmTop.Visible = true;
					btnConfirmBottom.Visible = true;
					btnReloadTop.OnClientClick = "return confirmLoadingConfig();";
					btnReloadBottom.OnClientClick = "return confirmLoadingConfig();";
					RestoreSearchValue();
					break;

				case Constants.ACTION_STATUS_COMPLETE:
					this.IsDisplayOnly = true;
					divComp.Visible = true;
					tbErrorMessage.Visible = false;
					btnModifyTop.Visible = true;
					btnModifyBottom.Visible = true;
					this.SearchParam = null;
					break;

				default:
					this.IsDisplayOnly = true;
					btnModifyTop.Visible = true;
					btnModifyBottom.Visible = true;

					RestoreSearchValue();
					break;
			}

			rReadKbn.DataSource = this.AllConfigSettings;
			ddlTabs.DataBind();
			rReadKbn.DataBind();
		}
	}

	/// <summary>
	/// 初期化（表示用）
	/// </summary>
	protected void InitializeComponentsForDisplay()
	{
		if (this.AllConfigSettings == null)
		{
			// 全読み取り区分のオプションを読み込み
			var configSetting = new ConfigurationSetting(
				WebConfigurationManager.AppSettings["ConfigFileDirPath"]);
			this.AllConfigSettings = configSetting.SettingNodeList;
		}
	}

	/// <summary>
	/// 初期化（編集用）
	/// </summary>
	/// <remarks>
	/// このメソッドを通ると、一時的に全区分の設定値が読み込まれるため、必要な時以外利用しないこと
	/// </remarks>
	protected void InitializeComponentsForModify()
	{
		// 全読み取り区分のオプションを初期化
		var configSetting = new ConfigurationSetting(
			WebConfigurationManager.AppSettings["ConfigFileDirPath"],
			(ConfigurationSetting.ReadKbn[])Enum.GetValues(typeof(ConfigurationSetting.ReadKbn)));

		// _w2cManagerで必要なオプションのみ読み込みを初期化
		// Hack:ConfigurationSettingの初期化処理を通すのでなく、読み取り専用の処理を作りたい
		// 本当は初期化処理自体も作り変えたい
		ConfigurationSetting.CreateInstanceByReadKbn(
			WebConfigurationManager.AppSettings["ConfigFileDirPath"],
			ConfigurationSetting.ReadKbn.C300_ComerceManager);

		// 各コンフィグの最終更新時間を更新
		if (this.ConfigLastUpdatedTimes == null)
		{
			var configLastUpdatedTimes = configSetting.ConfigFilePathList
				.ToDictionary(path => path, path => File.GetLastWriteTime(path));

			this.ConfigLastUpdatedTimes = configLastUpdatedTimes;
		}

		this.AllConfigSettings = configSetting.SettingNodeList;
	}

	/// <summary>
	/// 検索値復元
	/// </summary>
	private void RestoreSearchValue()
	{
		divComp.Visible = false;
		tbErrorMessage.Visible = false;

		if (this.SearchParam != null)
		{
			string selectedTab;
			this.SearchParam.TryGetValue("SelectedDDL", out selectedTab);
			ddlTabs.SelectedValue = selectedTab;

			string searchValue;
			this.SearchParam.TryGetValue("SearchText", out searchValue);
			tbSearchText.Value = searchValue;
		}
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnModify_Click(object sender, EventArgs e)
	{
		InitializeComponentsForModify();

		// 検索値を保持
		this.SearchParam = new Dictionary<string, string>()
		{
			{ "SelectedDDL", ddlTabs.SelectedValue },
			{ "SearchText", tbSearchText.Value }
		};

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SITE_CONFIGRATION)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE)
			.AddParam(Constants.PARAM_GUID_STRING, this.GuidString)
			.CreateUrl();

		Response.Redirect(url);
	}

	/// <summary>
	/// 確認ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		// 検索値を保持
		this.SearchParam = new Dictionary<string, string>()
		{
			{ "SelectedDDL", ddlTabs.SelectedValue },
			{ "SearchText", tbSearchText.Value }
		};

		var errorMessages = new Dictionary<string, string>();

		foreach (RepeaterItem rKbn in rReadKbn.Items)
		{
			var settings = GetWrappedControl<WrappedRepeater>(rKbn, "rSettings");
			foreach (RepeaterItem setting in settings.Items)
			{
				var kbn = GetWrappedControl<WrappedHtmlInputHidden>(setting, "hfConfigurationReadKbn").Value;
				var key = GetWrappedControl<WrappedHtmlInputHidden>(setting, "hfConfigurationKey").Value;
				var value = GetWrappedControl<WrappedHtmlInputHidden>(setting, "hfConfigurationValue").Value;
				var errorControl = setting.FindControl("lbErrorMessageLine") as Label;

				var targetSetting = this.AllConfigSettings.FirstOrDefault(s => ((s.ReadKbn == kbn) && (s.Key == key)));

				if ((targetSetting == null) || (targetSetting.Value == value)) continue;

				var errorMessage = Validate(key, value);
				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					errorMessages.Add(key, Validate(key, value));
					errorControl.Visible= true;
					errorControl.Text = "<br>" + string.Join("<br>", errorMessage);
					continue;
				}

				targetSetting.Value= value;
			}
		}

		var hasError = false;
		if (this.AllConfigSettings.All(stg => stg.BeforeValue == stg.Value))
		{
			tbErrorMessage.Visible = true;
			lbErrorMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CONFIG_NOTHING_MODIFIED);
			hasError= true;
		}

		if (errorMessages.Count > 0)
		{
			var errorHtml = "<a href=\"#\" class=\"error-link\" data-target=\".line-{0}\" style=\"color: red\">{1}</a>";
			var errorMessage = errorMessages.Select(em => string.Format(errorHtml, em.Key, em.Value));
			tbErrorMessage.Visible = true;
			lbErrorMessage.Text = string.Join("<br>", errorMessage);
			hasError = true;
		}

		if (hasError)
		{
			ddlTabs.SelectedIndex = 0;
			tbSearchText.Value = string.Empty;
			divComp.Visible = false;
			return;
		}

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SITE_CONFIGRATION_CONFIRM)
			.AddParam(Constants.PARAM_GUID_STRING, this.GuidString)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 設定値のバリデート
	/// </summary>
	/// <param name="key">バリデート対象のキー</param>
	/// <param name="value">バリデート対象の設定値</param>
	/// <returns>エラーメッセージ</returns>
	/// <remarks>
	/// ConfigurationSetting内で初期化を行ってる読み取り区分のみバリデート可能
	/// 各バッチで独自に初期化を行ってる設定値に対しては、データ型の判別が出来ないためバリデート出来ない
	/// </remarks>
	private string Validate(string key, string value)
	{
		var setting = this.AllConfigSettings.FirstOrDefault(cfg => cfg.Key == key);
		var type = setting.DataType;
		var errorMessage = string.Empty;
		var messageTemplate = "アプリケーション設定{0}が{1}に変換できませんでした。";

		// 型変換
		if (type == typeof(int))
		{
			int iResult = 0;
			if (int.TryParse(value, out iResult) == false)
			{
				errorMessage = string.Format(messageTemplate, key, type.ToString());
			}
		}
		else if (type == typeof(int?))
		{
			if (string.IsNullOrEmpty(value)) return string.Empty;
			int result;
			if (int.TryParse(value, out result) == false)
			{
				errorMessage = string.Format(messageTemplate, key, type.ToString());
			}
		}
		else if (type == typeof(bool))
		{
			bool blResult = false;
			if (bool.TryParse(value, out blResult) == false)
			{
				errorMessage = string.Format(messageTemplate, key, type.ToString());
			}
		}
		else if (type == typeof(DateTime))
		{
			DateTime dtResult = new DateTime(0);
			if (DateTime.TryParse(value, out dtResult) == false)
			{
				errorMessage = string.Format(messageTemplate, key, type.ToString());
			}
		}
		else if (type == typeof(MailAddress))
		{
			try
			{
				var mail = new MailAddress(value);
			}
			catch (FormatException)
			{
				errorMessage = string.Format(messageTemplate, key, type.ToString());
			}
		}
		else if (type == typeof(decimal))
		{
			var result = 0m;
			if (decimal.TryParse(value, out result) == false)
			{
				errorMessage = string.Format(messageTemplate, key, type.ToString());
			}
		}

		return errorMessage;
	}

	/// <summary>
	/// 読取区分による設定情報取得
	/// </summary>
	/// <param name="readKbn">読取区分</param>
	/// <returns>設定情報</returns>
	protected List<SettingNode> GetConfigSettings(string readKbn)
	{
		var result = this.AllConfigSettings
			.Where(config => config.ReadKbn == readKbn)
			.ToList();

		return result;
	}

	/// <summary>
	/// 読取区分を表示する？（true:表示、false:非表示）
	/// </summary>
	/// <param name="readKbn">読取区分</param>
	protected bool IsDisplayReadKbn(string readKbn)
	{
		if (readKbn == this.ReadKbn) return false;

		this.ReadKbn = readKbn;
		return true;
	}

	/// <summary>
	/// 設定再読み込みボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReload_Click(object sender, EventArgs e)
	{
		this.AllConfigSettings = null;
		this.ConfigLastUpdatedTimes = null;

		if (Request.QueryString[Constants.REQUEST_KEY_ACTION_STATUS] != Constants.ACTION_STATUS_UPDATE)
		{
			this.IsDisplayOnly= true;
			InitializeComponentsForDisplay();
		}
		else
		{
			this.IsDisplayOnly = false;
			InitializeComponentsForModify();
		}

		divComp.Visible = false;
		tbErrorMessage.Visible = false;

		rReadKbn.DataSource = this.AllConfigSettings;
		ddlTabs.DataBind();
		rReadKbn.DataBind();
	}

	/// <summary>
	/// ラジオボタンを使うか
	/// </summary>
	/// <param name="node">設定</param>
	/// <returns>ラジオボタンを使うか</returns>
	public bool IsUseRadioButton(SettingNode settingNode)
	{
		bool node;
		var result = ((settingNode.DataType == typeof(bool)) || bool.TryParse(settingNode.Value, out node));
		return result;
	}

	/// <summary>読取区分</summary>
	public string ReadKbn
	{
		get { return StringUtility.ToEmpty(ViewState["ReadKbn"]); }
		set { ViewState["ReadKbn"] = value; }
	}
	/// <summary>読取区分リスト</summary>
	public string[] ReadKbnList
	{
		get
		{
			if (this.AllConfigSettings == null) return new string[0];

			var configList = this.AllConfigSettings
				.Select(config => config.ReadKbn)
				.Distinct()
				.ToArray();
			return configList;
		}
	}
	/// <summary>
	/// 検索値
	/// </summary>
	public Dictionary<string, string> SearchParam
	{
		get { return (Dictionary<string, string>)Session[Constants.SESSIONPARAM_KEY_ALL_CONFIGRATION_SEARCH_PARAM + this.GuidString]; }
		set { Session[Constants.SESSIONPARAM_KEY_ALL_CONFIGRATION_SEARCH_PARAM + this.GuidString] = value; }
	}
	/// <summary>表示のみか</summary>
	protected bool IsDisplayOnly {
		get
		{
			return (ViewState["is_display_only"] == null) || ((bool)ViewState["is_display_only"]);
		}
		set
		{
			ViewState["is_display_only"] = value;
		}
	}
}
