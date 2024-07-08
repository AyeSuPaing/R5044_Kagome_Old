/*
=========================================================================================================
  Module      : 注文関連ファイル取込出力コントローラ処理(BodyOrderFileImport.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.Common.Extensions;

public partial class Form_Common_OrderFileImport : BaseUserControl
{
	private const string VIEWSTATE_KEY_ORDERFILE_INFO = "orderfile_info"; // 取り込むファイルの説明
	private const string VIEWSTATE_KEY_FILENAME_PATTERN = "filename_pattern"; // ファイル名パターン
	private const string VIEWSTATE_KEY_PAST_MONTHS = "past_Months"; // 検索期間
	private const string VIEWSTATE_KEY_USED_MAILTEMPLATEID = "used_mailtemplateid";						// Used mail template id view state key

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			List<string> orderFileInfoList = new List<string>(); // 取り込むファイルの説明
			List<string> fileNamePatternList = new List<string>(); // ファイル名パターン
			List<string> pastMonthsList = new List<string>(); // 検索期間
			var usedMailTemplateIdList = new List<string>();		// Used mail template id list

			// XMLの設定ファイル読み込み
			XElement mainElement = XElement.Load(Constants.PHYSICALDIRPATH_COMMERCE_MANAGER + Constants.FILE_XML_ORDERFILEIMPORT_SETTING);
			foreach (var settingNode in mainElement.Elements("OrderFile"))
			{
				// ドロップダウン設定
				ddlOrderFile.Items.Add(new ListItem(settingNode.Element("Name").Value, settingNode.Element("Value").Value));

				// ファイル種別に関する情報格納
				orderFileInfoList.Add(settingNode.Element("Info").Value);

				// Dictionary型にしてから設定値をAddしていく
				Dictionary<string, string> importSettings
						= settingNode.Elements("ImportFileSetting").ToDictionary(node => node.Attribute("key").Value, node => node.Attribute("value").Value);
				fileNamePatternList.Add(importSettings.ContainsKey("FileNamePattern") ? importSettings["FileNamePattern"] : "");
				pastMonthsList.Add(importSettings.ContainsKey("PastMonths") ? importSettings["PastMonths"] : "");
				
				var onSuccessElement = settingNode.Element("OnSuccess");

				// Used mail template id
				usedMailTemplateIdList.Add((onSuccessElement != null) ? onSuccessElement.Attribute("usedMailTemplateId").Value : string.Empty);
			}

			// ドロップダウンリストのファイル種別に関する情報をウケトル連携専用のものに変更
			if (Constants.UKETORU_TRACKERS_API_ENABLED == false)
			{
				var index = 0;
				foreach (ListItem orderFileItem in ddlOrderFile.Items)
				{
					if (orderFileItem.Value == Constants.KBN_ORDERFILE_UKETORU_LINK)
					{
						orderFileInfoList.RemoveAt(index);
						ddlOrderFile.Items.RemoveAt(index);
						break;
					}
					index++;
				}
			}

			// ファイル種別に関する情報設定 AND ViewStateに保存(ファイル種別選択イベントで利用)
			lbOrderFileInfo.Text = orderFileInfoList[0];
			ViewState[VIEWSTATE_KEY_ORDERFILE_INFO] = orderFileInfoList;
			ViewState[VIEWSTATE_KEY_FILENAME_PATTERN] = fileNamePatternList;
			ViewState[VIEWSTATE_KEY_PAST_MONTHS] = pastMonthsList;
			ViewState[VIEWSTATE_KEY_USED_MAILTEMPLATEID] = usedMailTemplateIdList;

			// データバインド
			ddlOrderFile.DataBind();
		}

		// 画面メッセージ初期化
		lbResultMessage.Text = "";
		divError.Visible = false;
		trResultTitle.Visible = false;
		trResultDetail.Visible = false;
		lbMessageExcludeMessage.Visible = false;
	}

	/// <summary>
	/// ファイル種別選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlOrderFile_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		// 出荷情報登録連携のチェックボックスが存在するものはデフォルトでチェックをつける
		if (CanShipmentEntry()) cbExecExternalShipmentEntry.Checked = true;

		// ファイル種別に関する情報設定
		StringBuilder orderFileInfo = new StringBuilder();
		orderFileInfo.Append(((List<string>)ViewState[VIEWSTATE_KEY_ORDERFILE_INFO])[ddlOrderFile.SelectedIndex]);
		// 検索期間が設定されている場合は表示する
		if (((List<string>)ViewState[VIEWSTATE_KEY_PAST_MONTHS])[ddlOrderFile.SelectedIndex] != "")
		{
			orderFileInfo.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERFILEIMPORT_PAST_MONTHS_INFO)
				.Replace("@@ 1 @@", ((List<string>)ViewState[VIEWSTATE_KEY_PAST_MONTHS])[ddlOrderFile.SelectedIndex]));
		}

		// Add message need to turn on extend status
		if (Constants.DATAMIGRATION_OPTION_ENABLED
			&& (DateTime.Now <= Constants.DATAMIGRATION_END_DATETIME)
			&& (ddlOrderFile.SelectedValue == Constants.KBN_ORDERFILE_IMPORT_ORDER_MIGRATION))
		{
			orderFileInfo.Append(
				WebMessages.GetMessages(WebMessages.ERRMSG_NEED_TO_TURN_ON_EXTENDED_STATUSES_MESSAGE));
		}

		lbOrderFileInfo.Text = orderFileInfo.ToString();
		var isVisible = (ddlOrderFile.SelectedValue == Constants.KBN_ORDERFILE_IMPORT_ORDER_STATUS);
		orderStatusTitle.Visible = isVisible;
		rOrderStatusDescription.DataSource = isVisible ? GetOrderStatusSetting() : null;
		rOrderStatusDescription.DataBind();
	}

	/// <summary>
	/// ファイル取込ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnImport_Click(object sender, EventArgs e)
	{
		string fileNamePattern = ((List<string>)ViewState[VIEWSTATE_KEY_FILENAME_PATTERN])[ddlOrderFile.SelectedIndex];
		string mailTemplateId = ((List<string>)ViewState[VIEWSTATE_KEY_USED_MAILTEMPLATEID])[ddlOrderFile.SelectedIndex];

		var importResponse = OrderFileImportProcess.Start(
			ddlOrderFile.SelectedValue,
			fFile.PostedFile,
			fileNamePattern,
			mailTemplateId,
			cbExecExternalShipmentEntry.Checked,
			this.LoginOperatorName);

		var selectedValue = ddlOrderFile.SelectedValue.Split(':')[0];

		// バッチ処理時は完了画面へ遷移
		if (Constants.ORDER_FILE_IMPORT_ASYNC
			|| (selectedValue == Constants.KBN_ORDERFILE_IMPORT_ORDER)
			|| (selectedValue == Constants.KBN_ORDERFILE_IMPORT_ORDER_STATUS))
		{
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERFILEIMPORT_COMP);
			return;
		}

		// 結果表示制御
		lbResultMessage.Visible = true;
		trResultTitle.Visible = true;
		trResultDetail.Visible = true;

		// 処理結果メッセージ色設定・表示
		lbResultMessage.ForeColor = (importResponse.ImportSuccess) ? Color.Empty : Color.Red;
		lbResultMessage.Text = importResponse.ResultMessage;
		lbMessageExcludeMessage.Text = importResponse.ExcludeMessage;

		// エラーデータの表示
		if (importResponse.ImportSuccess == false)
		{
			var errorshippingDataList = importResponse.ErrorData;
			divError.Visible = ((errorshippingDataList != null) && (errorshippingDataList.Count > 0));
			rErrorList.DataSource = errorshippingDataList;
			rErrorList.DataBind();
		}
	}

	/// <summary>
	/// 出荷情報登録可能か
	/// </summary>
	/// <returns>出荷情報登録可能か</returns>
	protected bool CanShipmentEntry()
	{
		var selectedValue = ddlOrderFile.SelectedValue.Split(':')[0];
		var canShipmentEntry =
			OrderCommon.CanShipmentEntry()
			&& ((selectedValue == Constants.KBN_ORDERFILE_SHIPPING_NO_LINK)
				|| (selectedValue == Constants.KBN_ORDERFILE_ECAT2000LINK)
				|| (selectedValue == Constants.KBN_B2_RAKUTEN_INCL_LINK)
				|| (selectedValue == Constants.KBN_B2_RAKUTEN_INCL_LINK_CLOUD)
				|| (selectedValue == Constants.KBN_ORDERFILE_SHIPPING_DATA)
				|| (selectedValue == Constants.KBN_ORDERFILE_UKETORU_LINK)
				|| (selectedValue == Constants.KBN_ORDERFILE_PELICAN_RESULT_REPORT_LINK));
		return canShipmentEntry;
	}

	/// <summary>
	/// 受注情報ステータスの一括更新の設定ファイルを取得する
	/// </summary>
	/// <returns>設定値</returns>
	protected List<DisplayOrderStatus> GetOrderStatusSetting()
	{
		var xml = XElement.Load(Constants.PHYSICALDIRPATH_COMMERCE_MANAGER + Constants.FILE_XML_ORDERFILEIMPORT_SETTING);

		foreach (var element in xml.Elements("OrderFile").Elements("OrderStatusDispText"))
		{
			Dictionary<string, string> importSettings
				= element.Elements("status_name").Elements("Value").ToDictionary(
					node => node.Attribute("text").Value,
					node => node.Attribute("value").Value);
			var statusValues = importSettings
				.Select(status => new ListItem(status.Key, status.Value)).ToArray();
			var statusList = statusValues
				.Select(status => new DisplayOrderStatus(status.Value, status.Text, element)).ToList();

			return statusList;
		}

		return null;
	}

	/// <summary>
	/// 表示用注文ステータスクラス
	/// </summary>
	protected class DisplayOrderStatus
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="statusKbn">ステータス区分</param>
		/// <param name="statusName">ステータス名</param>
		/// <param name="xml">xml</param>
		public DisplayOrderStatus(string statusKbn, string statusName, XElement xml)
		{
			this.StatusDispNames = GetStatus(
					xml
					,statusKbn
					,statusName
					)
				.Select(status => new ListItem(status.Key, status.Value)).ToArray();
		}

		/// <summary>
		/// xmlに紐づくステータスを取得
		/// </summary>
		/// <param name="xml">xml</param>
		/// <param name="kbn">ステータス区分</param>
		/// <returns>対応表のカラム名と値</returns>
		private Dictionary<string, string> GetStatus(XElement xml, string kbn, string name)
		{
			this.StatusKbn = kbn;
			this.StatusName = name;

			Dictionary<string, string> importSettings = xml.Elements(kbn).Elements("Value").ToDictionary(
				node => node.Attribute("text").Value,
				node => node.Attribute("value").Value);

			// 実在庫連携オプションがOFFの場合在庫引当済みステータスは表示しない
			if(Constants.REALSTOCK_OPTION_ENABLED == false)
			{
				importSettings.Remove("在庫引当済み");
			}

			if (importSettings.Count == 0)
			{
				this.StatusKbn = string.Empty;
				this.StatusName = string.Empty;
				return importSettings;
			}

			return importSettings;
		}

		/// <summary> ステータス区分 </summary>
		public string StatusKbn { get; set; }
		/// <summary> ステータス名 </summary>
		public string StatusName { get; set; }
		/// <summary> ステータス設定値 </summary>
		public ListItem[] StatusItems { get; set; }
		/// <summary> ステータス表示名 </summary>
		public ListItem[] StatusDispNames { get; set; }
	}
}
