/*
=========================================================================================================
  Module      : 設定情報ページ処理(SiteConfigurationConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common;
using System.Xml.Linq;
using System.IO;
using System.Web.Configuration;
using w2.Common.Web;
using w2.Common.Logger;
using System.Text;
using w2.App.Common.Manager;

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

			// 楽観的排他制御
			if (IsConfigUpToDate() == false)
			{
				tbErrorMessage.Visible = true;
				lbErrorMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CONFIG_LASTUPDATED_MISMATCH);
				btnUpdate.Enabled = false;
			}
			else
			{
				tbErrorMessage.Visible = false;
				btnUpdate.Enabled = true;
			};

			// 更新対象の設定値のみ取得
			var targetSettings = GetUpdateTargetSettings();

			if (targetSettings.Any() == false)
			{
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SITE_CONFIGRATION);
			}

			rReadKbn.DataSource = targetSettings;
			rReadKbn.DataBind();
		}
	}

	/// <summary>
	/// 更新対象の設定値の取得
	/// </summary>
	/// <returns>更新対象の設定値</returns>
	protected List<SettingNode> GetUpdateTargetSettings()
	{
		var result = this.AllConfigSettings.Where(stg => stg.BeforeValue != stg.Value).ToList();
		return result;
	}

	/// <summary>
	/// 読取区分による設定情報取得
	/// </summary>
	/// <param name="readKbn">読取区分</param>
	/// <returns>設定情報</returns>
	protected List<SettingNode> GetConfigSettings(string readKbn)
	{
		var result = GetUpdateTargetSettings()
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
	/// 戻るボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void backbutton_Click(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SITE_CONFIGRATION)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE)
			.AddParam(Constants.PARAM_GUID_STRING, this.GuidString)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 更新ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		if (IsConfigUpToDate() == false)
		{
			tbErrorMessage.Visible = true;
			lbErrorMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CONFIG_LASTUPDATED_MISMATCH);
			btnUpdate.Enabled = false;
			return;
		}

		var configSetting = new ConfigurationSetting(WebConfigurationManager.AppSettings["ConfigFileDirPath"]);

		// 更新ファイルパスの指定
		var otherConfigPaths = configSetting.ConfigFilePathList.Skip(2).ToList();
		var projectConfigPath = configSetting.ConfigFilePathList.First(cs => cs.Contains("Project.Config"));
		var appallConfigPath = configSetting.ConfigFilePathList.First(cs => cs.Contains("AppAll.Config"));
		var otherDoc = otherConfigPaths.ToDictionary(oc => oc, oc => XDocument.Load(oc));
		var projectDoc = XDocument.Load(projectConfigPath);
		var appallDoc = XDocument.Load(appallConfigPath);
		var targetSettings = GetUpdateTargetSettings();
		var log = new StringBuilder();

		// ヘッダー情報記載
		var logHeader = "設定ファイルの変更が行われました（OperatorId:{0} IpAddress:{1} SessionId:{2}）";
		log.AppendFormat(logHeader,
			this.LoginOperatorId,
			new ShopOperatorLoginManager().GetIpAddress(),
			Session.SessionID).AppendLine();

		var logUpdateKbn = @"({0}) ファイル更新区分：{1}";
		var logBody = @"【変更{0}】ファイル名: {1}、読取区分 : {2}、設定キー : {3}、設定値 : {4}";
		var countIndex = 1;

		foreach (var config in targetSettings)
		{
			var fileName = config.PrimaryConfig.Split('\\');

			// AppAllでもProjectでもないコンフィグなら、そのまま更新
			if ((config.PrimaryConfig != projectConfigPath) && (config.PrimaryConfig != appallConfigPath))
			{
				// 開発用.Configから、指定されたキーに一致するSetting要素を検索
				var readKbnElement = otherDoc.First(od => od.Key == config.PrimaryConfig)
					.Value
					.Root
					.Element(config.ReadKbn);

				if (readKbnElement != null)
				{
					var settingElement = readKbnElement
						.Descendants("Setting")
						.FirstOrDefault(el => el.Attribute("key").Value == config.Key);

					if (settingElement != null)
					{
						// value属性の値を変更
						settingElement.Attribute("value").Value = config.Value;
						log.AppendFormat(logUpdateKbn, countIndex, "更新").AppendLine();
						log.AppendFormat(logBody, "前", fileName[fileName.Length -1], config.ReadKbn, config.Key, config.BeforeValue).AppendLine();
						log.AppendFormat(logBody, "後", fileName[fileName.Length -1], config.ReadKbn, config.Key, config.Value).AppendLine();
						countIndex++;
						continue;
					}
				}
			}

			// Project.Configが最優先なら、そのまま更新
			if (config.PrimaryConfig == projectConfigPath)
			{
				// Project.Configから、指定されたキーに一致するSetting要素を検索
				var readKbnElement = projectDoc
					.Root
					.Element(config.ReadKbn);
				if (readKbnElement != null)
				{
					var settingElement = readKbnElement
						.Descendants("Setting")
						.FirstOrDefault(el => el.Attribute("key").Value == config.Key);

					// Project.Configを更新する
					if (settingElement != null)
					{
						// value属性の値を変更
						settingElement.Attribute("value").Value = config.Value;
						log.AppendFormat(logUpdateKbn, countIndex, "更新").AppendLine();
						log.AppendFormat(logBody, "前", fileName[fileName.Length - 1], config.ReadKbn, config.Key, config.BeforeValue).AppendLine();
						log.AppendFormat(logBody, "後", fileName[fileName.Length - 1], config.ReadKbn, config.Key, config.Value).AppendLine();
						countIndex++;
						continue;
					}
				}
			}

			// AppAll.Configが最優先なら、ProjectConfigに設定値を追加する
			if (config.PrimaryConfig == appallConfigPath)
			{
				// AppAll.Configから、指定されたキーに一致するSetting要素を検索
				var readKbnElement = appallDoc
					.Root
					.Element(config.ReadKbn);
				if (readKbnElement != null)
				{
					var settingElement = readKbnElement
						.Descendants("Setting")
						.FirstOrDefault(el => el.Attribute("key").Value == config.Key);

					if (settingElement != null)
					{
						// 設定値の読取区分を取得
						var proReadKbnElement = projectDoc.Root.Element(config.ReadKbn);

						// 設定値の読み取り区分がない場合は作成する。
						if (proReadKbnElement == null)
						{
							projectDoc.Root.Add(new XElement(config.ReadKbn));
							proReadKbnElement = projectDoc.Root.Element(config.ReadKbn);
						}

						// AppAll.Configをもとに追加したい設定値までのリストを作成する。
						var elements = readKbnElement.Descendants("Setting").ToList();
						var index = elements.IndexOf(settingElement);
						var beforeTargetElements = elements.GetRange(0, index);

						// 設定値リストを逆順に並び替える
						beforeTargetElements.Reverse();

						// 設定値を追加したかどうかのフラグ
						var isAddElement = false;
						// コメントの作成とvalue属性の値の変更
						var comment = new XComment(config.Comment);
						settingElement.Attribute("value").Value = config.Value;

						// Project.Config内で設定値を探し、見つかった場合はその下に追加したい設定値を追加する
						foreach (var element in beforeTargetElements)
						{
							// 追加したい場所を探し、見つかった場合はProject.Configファイルに追加する。
							var addLocation = proReadKbnElement
								.Descendants("Setting")
								.FirstOrDefault(el => el.Attribute("key").Value == element.Attribute("key").Value);
							if (addLocation != null)
							{
								// Project.Configファイルに追加
								addLocation.AddAfterSelf(comment);
								comment.AddAfterSelf(settingElement);
								isAddElement = true;
								log.AppendFormat(logUpdateKbn, countIndex, "追加").AppendLine();
								log.AppendFormat(logBody, "前", "AppAll.Config", config.ReadKbn, config.Key, config.BeforeValue).AppendLine();
								log.AppendFormat(logBody, "後", "Project.Config", config.ReadKbn, config.Key, config.Value).AppendLine();
								countIndex++;
								break;
							}
						}

						// 設定値が追加できなかった場合は同一読み取り区分内の一番下に追加する
						if (isAddElement == false)
						{
							proReadKbnElement.Add(comment);
							proReadKbnElement.Add(settingElement);
							log.AppendFormat(logUpdateKbn, countIndex, "追加").AppendLine();
							log.AppendFormat(logBody, "前", "AppAll.Config", config.ReadKbn, config.Key, config.BeforeValue).AppendLine();
							log.AppendFormat(logBody, "後", "Project.Config", config.ReadKbn, config.Key, config.Value).AppendLine();
							countIndex++;
						}
						continue;
					}
				}
			}
		}

		// 変更をその他ファイルに保存
		foreach (var otherConfig in otherDoc)
		{
			using (FileStream fs = new FileStream(otherConfig.Key, FileMode.Truncate))
			{
				otherConfig.Value.Save(fs);
			}
		}

		// 変更をProjectファイルに保存
		using (FileStream fs = new FileStream(projectConfigPath, FileMode.Truncate))
		{
			projectDoc.Save(fs);
		}

		// ログ書き込み
		FileLogger.Write("config", log.ToString(), Constants.PHYSICALDIRPATH_LOGFILE);

		// セッションクリア
		this.AllConfigSettings = null;
		this.ConfigLastUpdatedTimes = null;

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SITE_CONFIGRATION)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_COMPLETE)
			.AddParam(Constants.PARAM_GUID_STRING, Request[Constants.PARAM_GUID_STRING])
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 最終更新時間の比較
	/// </summary>
	/// <returns>更新出来るか</returns>
	protected bool IsConfigUpToDate()
	{
		if (this.ConfigLastUpdatedTimes == null) return false;

		// 新しい最終更新情報を取得
		var configSetting = new ConfigurationSetting(
			WebConfigurationManager.AppSettings["ConfigFileDirPath"]);

		var currentConfigLastUpdatedTimes = configSetting.ConfigFilePathList
			.ToDictionary(path => path, path => File.GetLastWriteTime(path));

		// セッションに保存されている情報と現在の情報を比較
		var result = currentConfigLastUpdatedTimes.All(entry =>
			(this.ConfigLastUpdatedTimes.ContainsKey(entry.Key) == false) ||
			(entry.Value <= this.ConfigLastUpdatedTimes[entry.Key]));

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
			var targetSettings = GetUpdateTargetSettings();
			if (targetSettings.Any() == false) return new string[0];

			var configList = targetSettings
				.Select(config => config.ReadKbn)
				.Distinct()
				.ToArray();
			return configList;
		}
	}
}
