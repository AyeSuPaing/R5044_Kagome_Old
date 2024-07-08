/*
=========================================================================================================
  Module      : Advertisement Code Register(AdvertisementCodeRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.RefreshFileManager;
using w2.App.Common.MasterExport;
using w2.Domain.ShopOperator;
using w2.Domain.AdvCode;
using Input.AdvCode;
using w2.App.Common.Option;
using w2.Common.Web;
using w2.Domain.UserManagementLevel;

public partial class Form_AdvertisementCode_AdvertisementCodeRegister : BasePage
{
	protected Hashtable m_inputParam = new Hashtable();
	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// Initial screen controls
			//------------------------------------------------------
			InitializeComponents(this.ActionStatus);

			//------------------------------------------------------
			// Set screen data 
			//------------------------------------------------------
			switch (this.ActionStatus)
			{
				// New registration
				case Constants.ACTION_STATUS_INSERT:
					break;

				// Update
				case Constants.ACTION_STATUS_UPDATE:
				case Constants.ACTION_STATUS_COPY_INSERT:
					// Get advertisement code
					var advCode = new AdvCodeService().GetAdvCodeFromAdvertisementCode(Request[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE]);

					if (advCode != null)
					{
						m_inputParam = advCode.DataSource;
						if (this.IsBackFromConfirm == false)
						{
							var advPublishDateFrom = string.Empty;
							var advPublishDateTo = string.Empty;
							if (string.IsNullOrEmpty(StringUtility.ToEmpty(m_inputParam[Constants.FIELD_ADVCODE_PUBLICATION_DATE_FROM])) == false)
							{
								advPublishDateFrom = DateTime.Parse(StringUtility.ToEmpty(m_inputParam[Constants.FIELD_ADVCODE_PUBLICATION_DATE_FROM]))
									.ToString("yyyyMMddHHmmss");
							}

							if (string.IsNullOrEmpty(StringUtility.ToEmpty(m_inputParam[Constants.FIELD_ADVCODE_PUBLICATION_DATE_TO])) == false)
							{
								advPublishDateTo = DateTime.Parse(StringUtility.ToEmpty(m_inputParam[Constants.FIELD_ADVCODE_PUBLICATION_DATE_TO]))
									.ToString("yyyyMMddHHmmss");
							}
							ucPublishDatePeriod.SetPeriodDate(advPublishDateFrom, advPublishDateTo);

							if (string.IsNullOrEmpty(StringUtility.ToEmpty(m_inputParam[Constants.FIELD_ADVCODE_ADVERTISEMENT_DATE])) == false)
							{
								ucAdvCodeDate.SetStartDate(DateTime.Parse(StringUtility
									.ToEmpty(m_inputParam[Constants.FIELD_ADVCODE_ADVERTISEMENT_DATE])));
							}
						}

						// Set value
						foreach (ListItem item in ddlAdvCodeMediaType.Items)
						{
							item.Selected = (item.Value == advCode.AdvcodeMediaTypeId);
						}
						foreach (ListItem item in ddlMemberRank.Items)
						{
							item.Selected = (item.Value == advCode.MemberRankIdGrantedAtAccountRegistration);
						}
						foreach (ListItem item in ddlUserManagementLevel.Items)
						{
							item.Selected = (item.Value == advCode.UserManagementLevelIdGrantedAtAccountRegistration);
						}
					}
					else
					{
						// To Error page
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
					}

					break;

				default:
					// If the status is not set
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					break;
			}

			// Data binding
			DataBind();
		}

		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;
	}

	/// <summary>
	/// Initialize Components
	/// </summary>
	/// <param name="actionStatus">Action Status</param>
	private void InitializeComponents(string actionStatus)
	{
		// Check action status
		switch (actionStatus)
		{
			// Insert
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				trRegister.Visible = true;
				btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = false;
				btnInsertUpdateTopRegist.Visible = true;
				btnInsertUpdateBottomRegist.Visible = true;
				btnInsertUpdateTopUpdate.Visible = false;
				btnInsertUpdateBottomUpdate.Visible = false;
				break;

			// Update
			case Constants.ACTION_STATUS_UPDATE:
				trEdit.Visible = true;
				btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = true;
				btnInsertUpdateTopRegist.Visible = false;
				btnInsertUpdateBottomRegist.Visible = false;
				btnInsertUpdateTopUpdate.Visible = true;
				btnInsertUpdateBottomUpdate.Visible = true;
				btnDeleteTop.Visible = true;
				btnDeleteBottom.Visible = true;
				break;
		}

		// Get AdvCode media type list
		var advCodeMediaTypeList = new AdvCodeService().GetAdvCodeMediaTypeListAll();
		ddlAdvCodeMediaType.Items.Add(new ListItem("", ""));
		foreach (AdvCodeMediaTypeModel item in advCodeMediaTypeList)
		{
			ddlAdvCodeMediaType.Items.Add(new ListItem(item.AdvcodeMediaTypeName, item.AdvcodeMediaTypeId));
		}

		// 会員ランクドロップダウン作成
		var memberRank = MemberRankOptionUtility.GetMemberRankList();
		ddlMemberRank.Items.Add(new ListItem("", ""));
		ddlMemberRank.Items.AddRange(
			memberRank.Select(m => new ListItem(m.MemberRankName, m.MemberRankId)).ToArray());

		// ユーザー管理レベルドロップダウン作成
		var userManagemetLevel = new UserManagementLevelService().GetAllList();
		ddlUserManagementLevel.Items.Add(new ListItem("", ""));
		ddlUserManagementLevel.Items.AddRange(
			userManagemetLevel.Select(m => new ListItem(m.UserManagementLevelName, m.UserManagementLevelId)).ToArray());

		if (actionStatus == Constants.ACTION_STATUS_INSERT)
		{
			ucPublishDatePeriod.SetStartDate(DateTime.Now.Date);
		}

		// If the completion screen
		if (Session[Constants.SESSIONPARAM_KEY_ADVCODE_INFO] != null)
		{
			Hashtable sessionData = (Hashtable)Session[Constants.SESSIONPARAM_KEY_ADVCODE_INFO];
			if (StringUtility.ToEmpty(sessionData[Constants.REQUEST_KEY_ACTION_STATUS]) == Constants.ACTION_STATUS_COMPLETE)
			{
				divComp.Visible = true;
				sessionData[Constants.REQUEST_KEY_ACTION_STATUS] = null;
			}

			if (this.IsBackFromConfirm
				&& string.IsNullOrEmpty((string)sessionData[Constants.FIELD_ADVCODE_PUBLICATION_DATE_FROM]) == false)
			{
				var publishDatePeriodFrom = (string)sessionData[Constants.FIELD_ADVCODE_PUBLICATION_DATE_FROM];
				var publishDatePeriodTo = (string)sessionData[Constants.FIELD_ADVCODE_PUBLICATION_DATE_TO];

				if (string.IsNullOrEmpty(publishDatePeriodFrom) == false)
				{
					ucPublishDatePeriod.SetStartDate(DateTime.Parse(publishDatePeriodFrom));
				}
				else
				{
					ucPublishDatePeriod.SetPeriodDate(string.Empty, string.Empty);
				}

				if (string.IsNullOrEmpty(publishDatePeriodTo) == false)
				{
					ucPublishDatePeriod.SetEndDate(DateTime.Parse(publishDatePeriodTo));
				}
				else
				{
					ucPublishDatePeriod.SetPeriodDate(string.Empty, string.Empty);
				}

				if (string.IsNullOrEmpty(StringUtility.ToEmpty(sessionData[Constants.FIELD_ADVCODE_ADVERTISEMENT_DATE])) == false)
				{
					ucAdvCodeDate.SetStartDate(DateTime.Parse(StringUtility
						.ToEmpty(sessionData[Constants.FIELD_ADVCODE_ADVERTISEMENT_DATE])));
				}
				else
				{
					ucAdvCodeDate.SetPeriodDate(string.Empty, string.Empty);
				}
			}
			else
			{
				ucPublishDatePeriod.SetPeriodDate(string.Empty, string.Empty);
				ucAdvCodeDate.SetPeriodDate(string.Empty, string.Empty);
			}
		}
	}

	/// <summary>
	/// Create AdvCode List Url
	/// </summary>
	/// <returns>AdvCode List Url</returns>
	protected string CreateAdvCodeListUrl()
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_LIST);
		if (Session[Constants.SESSIONPARAM_KEY_ADVCODE_SEARCH_INFO] != null)
		{
			var searchParams = (Hashtable)Session[Constants.SESSIONPARAM_KEY_ADVCODE_SEARCH_INFO];

			// Advertisement Code
			urlCreator.AddParam(
				Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE,
				(string)searchParams[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE]);
			// Media Name
			urlCreator.AddParam(
				Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME,
				(string)searchParams[Constants.REQUEST_KEY_ADVCODE_MEDIA_NAME]);
			// Media Type Id
			urlCreator.AddParam(
				Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID,
				(string)searchParams[Constants.REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID]);
			// Sort
			urlCreator.AddParam(
				Constants.REQUEST_KEY_SORT_KBN,
				(string)searchParams[Constants.REQUEST_KEY_SORT_KBN]);
			// Page no
			urlCreator.AddParam(
				Constants.REQUEST_KEY_PAGE_NO,
				Request[Constants.REQUEST_KEY_PAGE_NO]);
		}

		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// Create AdvCode Detail Url
	/// </summary>
	/// <param name="advCode">Advertisement Code</param>
	/// <returns>AdvCode Detail Url</returns>
	protected string CreateAdvCodeDetailUrl(string advCode)
	{
		var resultUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE, advCode)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE)
			.AddParam(Constants.REQUEST_KEY_PAGE_NO, Request[Constants.REQUEST_KEY_PAGE_NO])
			.CreateUrl();
		return resultUrl;
	}

	/// <summary>
	/// 登録/更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertUpdate_Click(object sender, EventArgs e)
	{
		var advCodeInput = GetInputData();

		// パラメタをセッションへ格納
		Session[Constants.SESSIONPARAM_KEY_ADVCODE_INFO] = advCodeInput.DataSource;

		if ((string.IsNullOrEmpty(advCodeInput.PublicationDateFrom) == false)
			|| (string.IsNullOrEmpty(advCodeInput.PublicationDateTo) == false)
			|| (string.IsNullOrEmpty(advCodeInput.AdvertisementDate) == false))
		{
			Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = 1;
		}

		var errorMessages = advCodeInput.Validate(this.ActionStatus, this.LoginOperatorDeptId, Request[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE]);
		if (string.IsNullOrEmpty(errorMessages) == false)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		// 登録/更新
		var model = advCodeInput.CreateModel();

		// Insert/ Update AdvCode
		InsertUpdateAdvCode(model);

		// 閲覧権限更新
		UpdateOperatorAdvAuthority();

		// アフィリエイトタグ設定のキャッシュ更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.AdvCode).CreateUpdateRefreshFile();

		// 完了メッセージ表示用パラメータ設定
		if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE) && (Session[Constants.SESSIONPARAM_KEY_ADVCODE_INFO] != null))
		{
			Hashtable sessionData = (Hashtable)Session[Constants.SESSIONPARAM_KEY_ADVCODE_INFO];
			sessionData[Constants.REQUEST_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;
		}

		// 登録更新画面へ戻る
		var pageUrl = string.IsNullOrEmpty(hfAdvCodeNo.Value)
			? CreateAdvCodeListUrl()
			: CreateAdvCodeDetailUrl(tbAdvertisementCode.Text);

		Response.Redirect(pageUrl);
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		// 削除
		new AdvCodeService().DeleteAdvCode(long.Parse(hfAdvCodeNo.Value));

		// アフィリエイトタグ設定のキャッシュ更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.AdvCode).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_LIST);
	}

	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		var resultUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE, tbAdvertisementCode.Text)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_COPY_INSERT)
			.AddParam(Constants.REQUEST_KEY_PAGE_NO, Request[Constants.REQUEST_KEY_PAGE_NO])
			.CreateUrl();
		Response.Redirect(resultUrl);
	}

	/// <summary>
	/// Get the input data.
	/// </summary>
	/// <returns>The Advcode input</returns>
	private AdvCodeInput GetInputData()
	{
		var advCodeInput = new AdvCodeInput();

		advCodeInput.AdvcodeNo = string.IsNullOrEmpty(hfAdvCodeNo.Value) ? "0" : hfAdvCodeNo.Value;
		advCodeInput.DeptId = this.LoginOperatorDeptId;
		advCodeInput.AdvertisementCode = tbAdvertisementCode.Text.Trim();
		advCodeInput.AdvcodeMediaTypeId = ddlAdvCodeMediaType.SelectedValue;

		if (string.IsNullOrEmpty(ucAdvCodeDate.HfStartDate.Value) == false)
		{
			advCodeInput.AdvertisementDate = string.Format("{0} {1}",
				ucAdvCodeDate.HfStartDate.Value,
				ucAdvCodeDate.HfStartTime.Value);
			advCodeInput.DataSource[Constants.FIELD_ADVCODE_ADVERTISEMENT_DATE + "_chk"] = string.Format("{0} {1}",
				ucAdvCodeDate.HfStartDate.Value,
				ucAdvCodeDate.HfStartTime.Value);
		}
		else
		{
			advCodeInput.AdvertisementDate = null;
		}

		advCodeInput.MediaName = tbMediaName.Text.Trim();
		advCodeInput.MediaCost = StringUtility.ToNull(tbMediaCost.Text);
		advCodeInput.Memo = tbMemo.Text.Trim();
		if (string.IsNullOrEmpty(ucPublishDatePeriod.HfStartDate.Value) == false)
		{
			advCodeInput.PublicationDateFrom = string.Format("{0} {1}",
				ucPublishDatePeriod.HfStartDate.Value,
				ucPublishDatePeriod.HfStartTime.Value);
			advCodeInput.DataSource[Constants.FIELD_ADVCODE_PUBLICATION_DATE_FROM] = advCodeInput.PublicationDateFrom;
		}
		else
		{
			advCodeInput.PublicationDateFrom = null;
		}

		if (string.IsNullOrEmpty(ucPublishDatePeriod.HfEndDate.Value) == false)
		{
			advCodeInput.PublicationDateTo = string.Format("{0} {1}",
				ucPublishDatePeriod.HfEndDate.Value,
				ucPublishDatePeriod.HfEndTime.Value);
			advCodeInput.DataSource[Constants.FIELD_ADVCODE_PUBLICATION_DATE_TO] = advCodeInput.PublicationDateTo;
		}
		else
		{
			advCodeInput.PublicationDateTo = null;
		}

		advCodeInput.MemberRankIdGrantedAtAccountRegistration = ddlMemberRank.SelectedValue;
		advCodeInput.UserManagementLevelIdGrantedAtAccountRegistration = ddlUserManagementLevel.SelectedValue;
		advCodeInput.ValidFlg = cbValidFlg.Checked ? Constants.FLG_ADVCODE_VALID_FLG_VALID : Constants.FLG_ADVCODE_VALID_FLG_INVALID;
		advCodeInput.LastChanged = this.LoginOperatorName;

		return advCodeInput;
	}

	/// <summary>
	/// DBに登録
	/// <param name="model">Model</param>
	/// </summary>
	private void InsertUpdateAdvCode(AdvCodeModel model)
	{
		var advCodeService = new AdvCodeService();
		if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			advCodeService.UpdateAdvCode(model);
		}
		else
		{
			advCodeService.InsertAdvCode(model);
		}
	}

	/// <summary>
	/// 閲覧権限登録
	/// </summary>
	private void UpdateOperatorAdvAuthority()
	{
		var shopOperatorService = new ShopOperatorService();

		var operatorAdvAuth = shopOperatorService.Get(this.LoginOperatorShopId, this.LoginOperatorId).UsableAdvcodeNosInReport;

		// オペレータは閲覧権限されていれば、追加更新
		if (string.IsNullOrEmpty(operatorAdvAuth) == false)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_SHOPOPERATOR_USABLE_ADVCODE_NOS_IN_REPORT,
					MasterExportSettingUtility.GetFieldsEscape(string.Format("{0}{1}{2}", operatorAdvAuth, ",\n", tbAdvertisementCode.Text.Trim())) },
				{ Constants.FIELD_SHOPOPERATOR_SHOP_ID, this.LoginOperatorShopId },
				{ Constants.FIELD_SHOPOPERATOR_LAST_CHANGED, this.LoginOperatorName },
				{ Constants.FIELD_SHOPOPERATOR_OPERATOR_ID, this.LoginOperatorId },
			};

			shopOperatorService.UpdateOperatorAdvAuthority(ht);
		}
	}

	/// <summary>出稿日</summary>
	protected DateTime? AdvertisementDate
	{
		get
		{
			DateTime date;
			return DateTime.TryParse(
				StringUtility.ToEmpty(m_inputParam[Constants.FIELD_ADVCODE_ADVERTISEMENT_DATE]),
				out date)
				? date
				: (DateTime?)null;
		}
	}

	/// <summary>確認画面から戻ってきたか</summary>
	protected bool IsBackFromConfirm
	{
		get { return (Session[Constants.SESSION_KEY_PARAM_FOR_BACK] != null); }
	}
}
