/*
=========================================================================================================
  Module      :タグ閲覧権限ページ(TagAuthorityRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common.Affiliate;
using w2.App.Common.MasterExport;
using w2.Common.Extensions;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.AdvCode;
using w2.Domain.AdvCode.Helper;
using w2.Domain.Affiliate;
using w2.Domain.Affiliate.Helper;
using w2.Domain.ShopOperator;

public partial class Form_TagAuthority_TagAuthorityRegister : BasePage
{
	#region イベント
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			var authorityKbn = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TAG_AUTHORITY_AUTHORITY_KBN]);
			this.AuthorityKbn = (string.IsNullOrEmpty(authorityKbn) == false)
				? authorityKbn
				: Constants.FLG_TAG_AUTHORITY_KBN_TAG;

			GetOperatorList();

			// 値リスト設定
			SetViewTagIdList();
			SetViewMediaTypeList();
			SetViewLocationList();
			upAuthorities.DataBind();

			// 選択値表示
			DisplayAuthorityFields();

			var tabWrapperControls = new[]
			{
				lbSwitchTagAuthority,
				lbSwitchMediaTypeAuthority,
				lbSwitchLocationAuthority,
			};
			SetTabWrapperCurrentCss(tabWrapperControls);

			// 完了画面の場合
			if (StringUtility.ToEmpty(this.Session[Constants.SESSION_KEY_ACTION_STATUS]) != Constants.ACTION_STATUS_COMPLETE)
			{
				return;
			}

			divComp.Visible = true;
			switch (this.AuthorityKbn)
			{
				case Constants.FLG_TAG_AUTHORITY_KBN_TAG:
					lCompKbnText.Text = WebSanitizer.HtmlEncode(Constants.FLG_TAG_AUTHORITY_TEXT_TAG_ID);
					break;

				case Constants.FLG_TAG_AUTHORITY_KBN_MEDIA_TYPE:
					lCompKbnText.Text = WebSanitizer.HtmlEncode(Constants.FLG_TAG_AUTHORITY_TEXT_MEDIA_TYPE);
					break;

				case Constants.FLG_TAG_AUTHORITY_KBN_LOCATION:
					lCompKbnText.Text = WebSanitizer.HtmlEncode(Constants.FLG_TAG_AUTHORITY_TEXT_LOCATION);
					break;
			}
			this.Session[Constants.SESSION_KEY_ACTION_STATUS] = null;
		}
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		switch (this.AuthorityKbn)
		{
			case Constants.FLG_TAG_AUTHORITY_KBN_TAG:
				SetViewTagIdList();
				DisplayTagAuthorityFields();
				break;

			case Constants.FLG_TAG_AUTHORITY_KBN_MEDIA_TYPE:
				SetViewMediaTypeList();
				DisplayMediaTypeAuthorityFields();
				break;
		}
	}

	/// <summary>
	/// オペレータ変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbOperator_CheckedChanged(object sender, EventArgs e)
	{
		var riSender = (RepeaterItem)((RadioButton)sender).Parent;

		foreach (RepeaterItem ri in rOperatorList.Items)
		{
			((RadioButton)ri.FindControl("rbOperator")).Checked = (ri == riSender);

			if (ri != riSender) continue;

			hfOperatorId.Value = ((HiddenField)ri.FindControl("hfOperatorId")).Value;
		}

		RedirectToSearch();
	}

	/// <summary>
	/// 更新する
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_OnClick(object sender, EventArgs e)
	{
		var ht = GetAuthorityInput();

		InputValidate(ht);

		ht.Add(Constants.FIELD_SHOPOPERATOR_SHOP_ID, this.LoginOperatorShopId);
		ht.Add(Constants.FIELD_SHOPOPERATOR_LAST_CHANGED, this.LoginOperatorName);
		ht.Add(Constants.FIELD_SHOPOPERATOR_OPERATOR_ID, hfOperatorId.Value.Trim());
		new ShopOperatorService().UpdateOperatorTagAuthority(ht);

		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;
		RedirectToSearch();
	}

	/// <summary>
	/// 閲覧権限設定取得
	/// </summary>
	/// <returns>閲覧権限設定</returns>
	private Hashtable GetAuthorityInput()
	{
		var authorities = new Hashtable
		{
			{ Constants.FIELD_SHOPOPERATOR_USABLE_AFFILIATE_TAG_IDS_IN_REPORT, this.SelectedOperator.UsableAffiliateTagIdsInReport },
			{ Constants.FIELD_SHOPOPERATOR_USABLE_ADVCODE_MEDIA_TYPE_IDS, this.SelectedOperator.UsableAdvcodeMediaTypeIds },
			{ Constants.FIELD_SHOPOPERATOR_USABLE_OUTPUT_LOCATIONS, this.SelectedOperator.UsableOutputLocations },
		};

		switch (this.AuthorityKbn)
		{
			case Constants.FLG_TAG_AUTHORITY_KBN_TAG:
				var tagIds = MasterExportSettingUtility.GetFieldsEscape(this.SelectedTagIds.JoinToString(","));
				authorities[Constants.FIELD_SHOPOPERATOR_USABLE_AFFILIATE_TAG_IDS_IN_REPORT] = tagIds;
				break;

			case Constants.FLG_TAG_AUTHORITY_KBN_MEDIA_TYPE:
				var mediaTypes = MasterExportSettingUtility.GetFieldsEscape(this.SelectedMediaTypeIds.JoinToString(","));
				authorities[Constants.FIELD_SHOPOPERATOR_USABLE_ADVCODE_MEDIA_TYPE_IDS] = mediaTypes;
				break;

			case Constants.FLG_TAG_AUTHORITY_KBN_LOCATION:
				var locations = MasterExportSettingUtility.GetFieldsEscape(this.SelectedLocations.JoinToString(","));
				authorities[Constants.FIELD_SHOPOPERATOR_USABLE_OUTPUT_LOCATIONS] = locations;
				break;
		}

		return authorities;
	}

	/// <summary>
	/// タブ切り替えイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSwitch_OnClick(object sender, EventArgs e)
	{
		var selectedAuthorityKbn = ((LinkButton)sender).CommandArgument;
		if (this.AuthorityKbn == selectedAuthorityKbn) return;

		this.AuthorityKbn = selectedAuthorityKbn;

		SwitchDisplayAuthority();
		SetDifferential();
	}

	/// <summary>
	/// 一括変更ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAllChange_OnClick(object sender, EventArgs e)
	{
		Repeater rTarget;
		List<string> selectedValues;
		switch (this.AuthorityKbn)
		{
			case Constants.FLG_TAG_AUTHORITY_KBN_TAG:
				rTarget = rTagIdList;
				selectedValues = this.SelectedTagIds;
				break;

			case Constants.FLG_TAG_AUTHORITY_KBN_MEDIA_TYPE:
				rTarget = rMediaTypeList;
				selectedValues = this.SelectedMediaTypeIds;
				break;

			case Constants.FLG_TAG_AUTHORITY_KBN_LOCATION:
				rTarget = rLocationList;
				selectedValues = this.SelectedLocations;
				break;

			default:
				return;
		}

		var isChecked = (((Button)sender).CommandArgument == Constants.FLG_TAG_AUTHORITY_ISCHECKED_VALID);
		foreach (RepeaterItem ri in rTarget.Items)
		{
			var cbIsSelected = (CheckBox)ri.FindControl("cbIsSelected");
			var hfValue = (HiddenField)ri.FindControl("hfValue");
			if ((cbIsSelected == null) || (hfValue == null)) continue;

			cbIsSelected.Checked = isChecked;
			if (isChecked && (selectedValues.Contains(hfValue.Value) == false))
			{
				selectedValues.Add(hfValue.Value);
			}
			else if ((isChecked == false) && selectedValues.Contains(hfValue.Value))
			{
				selectedValues.Remove(hfValue.Value);
			}
		}

		SetDifferential();
	}

	/// <summary>
	/// チェックボックス状態変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbIsSelected_OnCheckedChanged(object sender, EventArgs e)
	{
		var hfValue = (HiddenField)((CheckBox)sender).BindingContainer.FindControl("hfValue");
		List<string> selectedAuthority;
		switch (this.AuthorityKbn)
		{
			case Constants.FLG_TAG_AUTHORITY_KBN_TAG:
				selectedAuthority = this.SelectedTagIds;
				break;

			case Constants.FLG_TAG_AUTHORITY_KBN_MEDIA_TYPE:
				selectedAuthority = this.SelectedMediaTypeIds;
				break;

			case Constants.FLG_TAG_AUTHORITY_KBN_LOCATION:
				selectedAuthority = this.SelectedLocations;
				break;

			default:
				return;
		}

		if (selectedAuthority.Contains(hfValue.Value))
		{
			selectedAuthority.Remove(hfValue.Value);
		}
		else
		{
			selectedAuthority.Add(hfValue.Value);
		}

		SetDifferential();
	}
	#endregion

	#region Private
	/// <summary>
	/// オペレータ一覧を取得する
	/// </summary>
	private void GetOperatorList()
	{
		var input = new Hashtable
		{
			{ Constants.FIELD_SHOPOPERATOR_SHOP_ID, this.LoginOperatorShopId },
			{ Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG, this.LoginOperatorMenuAccessLevel },
			{ Constants.FIELD_COMMON_BEGIN_NUM, (this.CurrentPageNo - 1) * Constants.CONST_DISP_CONTENTS_TAG_AUTHORITY_REGISTER + 1 },
			{ Constants.FIELD_COMMON_END_NUM, this.CurrentPageNo * Constants.CONST_DISP_CONTENTS_TAG_AUTHORITY_REGISTER }
		};

		var operatorList = new ShopOperatorService().GetOperatorList(input);

		if (operatorList.Length == 0)
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			return;
		}

		var total = int.Parse(operatorList[0].RowCount.ToString());

		var nextUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_TAG_AUTHORITY_REGISTER)
			.AddParam(Constants.REQUEST_KEY_SHOP_OPERATOR_ID, hfOperatorId.Value)
			.CreateUrl();

		lbPager.Text = WebPager.CreateListPager(
			total,
			this.CurrentPageNo,
			nextUrl,
			Constants.CONST_DISP_CONTENTS_TAG_AUTHORITY_REGISTER);

		rOperatorList.DataSource = operatorList;
		rOperatorList.DataBind();

		var operatorId = this.Request[Constants.REQUEST_KEY_OPERATOR_ID];

		if (string.IsNullOrEmpty(operatorId) == false)
		{
			this.SelectedOperator = new ShopOperatorService().Get(this.LoginOperatorShopId, operatorId);
			var isUnExistOperator = (this.SelectedOperator == null);
			if (isUnExistOperator)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}

			foreach (RepeaterItem ri in rOperatorList.Items)
			{
				if (((HiddenField)ri.FindControl("hfOperatorId")).Value == operatorId)
				{
					((RadioButton)ri.FindControl("rbOperator")).Checked = true;

					hfOperatorId.Value = ((HiddenField)ri.FindControl("hfOperatorId")).Value;
					break;
				}
			}
		}
		else
		{
			((RadioButton)rOperatorList.Items[0].FindControl("rbOperator")).Checked = true;

			hfOperatorId.Value = ((HiddenField)rOperatorList.Items[0].FindControl("hfOperatorId")).Value;
			this.SelectedOperator = new ShopOperatorService().Get(this.LoginOperatorShopId, hfOperatorId.Value);
		}

		this.SelectedTagIds = this.SelectedOperator.GetUsableAffiliateTagIdsArray().ToList();
		this.SelectedMediaTypeIds = this.SelectedOperator.GetUsableAdvcodeMediaTypeIdsArray().ToList();
		this.SelectedLocations = this.SelectedOperator.GetUsableOutputLocationsArray().ToList();
	}

	/// <summary>
	/// タグID選択肢一覧を設定する
	/// </summary>
	private void SetViewTagIdList()
	{
		// タグ情報取得
		var searchWord = tbSearchTagId.Text.Trim();
		var tagInfo = DomainFacade.Instance.AffiliateTagSettingService.SearchByKeyword(searchWord);

		if (tagInfo.Length != 0)
		{
			trTagListError.Visible = false;
		}
		else
		{
			// 一覧非表示・エラー表示制御
			trTagListError.Visible = true;
			tdTagErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
		rTagIdList.DataSource = tagInfo.OrderByDescending(tagSetting => tagSetting.AffiliateId);
		rTagIdList.DataBind();
	}

	/// <summary>
	/// 広告媒体区分選択肢一覧を設定する
	/// </summary>
	private void SetViewMediaTypeList()
	{
		var searchWord = tbSearchMediaTypeId.Text.Trim();
		var mediaTypes = DomainFacade.Instance.AdvCodeService.SearchMediaTypeByKeyword(searchWord);

		if (mediaTypes.Any())
		{
			rMediaTypeList.DataSource = mediaTypes;
		}
		else
		{
			tdMediaTypeErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
		trMediaTypeListError.Visible = (mediaTypes.Any() == false);
		rMediaTypeList.DataBind();
	}

	/// <summary>
	/// 設置箇所選択肢一覧を設定する
	/// </summary>
	private void SetViewLocationList()
	{
		rLocationList.DataSource = TagSetting.GetInstance().Setting.TargetPages;
		rLocationList.DataBind();
	}

	/// <summary>
	/// タブ切り替え表示CSS設定
	/// </summary>
	/// <param name="controls">タブ切り替えトリガーコントロール群</param>
	private void SetTabWrapperCurrentCss(LinkButton[] controls)
	{
		foreach (var control in controls)
		{
			control.CssClass = (this.AuthorityKbn == control.CommandArgument)
				? "current"
				: string.Empty;
		}
	}

	/// <summary>
	/// URL作成してリダイレクト
	/// </summary>
	private void RedirectToSearch()
	{
		var urlCreater = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_TAG_AUTHORITY_REGISTER)
			.AddParam(Constants.REQUEST_KEY_SHOP_OPERATOR_ID, hfOperatorId.Value)
			.AddParam(Constants.REQUEST_KEY_PAGE_NO, this.CurrentPageNo.ToString())
			.AddParam(Constants.REQUEST_KEY_TAG_AUTHORITY_AUTHORITY_KBN, this.AuthorityKbn)
			.CreateUrl();

		Response.Redirect(urlCreater);
	}

	/// <summary>
	/// 入力チェック
	/// </summary>
	/// <param name="input">チェック値</param>
	private void InputValidate(Hashtable input)
	{
		var errorMessage = Validator.Validate("ShopOperatorModify", input);

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		var errorInfo = new List<string>();

		// タグIDの存在チェック
		if (CheckTagId((string)input[Constants.FIELD_SHOPOPERATOR_USABLE_AFFILIATE_TAG_IDS_IN_REPORT]) == false)
		{
			var message = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_REGISTER_SETTING_ERRORTAGID)
				.Replace("@@ 1 @@", this.ErrorTagId.ToString());
			errorInfo.Add(message);
		}

		// 広告媒体区分の存在チェック
		if (CheckMediaType((string)input[Constants.FIELD_SHOPOPERATOR_USABLE_ADVCODE_MEDIA_TYPE_IDS]) == false)
		{
			var message = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_REGISTER_SETTING_ERROR_MEDIATYPE)
				.Replace("@@ 1 @@", this.ErrorMediaType.ToString());
			errorInfo.Add(message);
		}

		// 設置箇所の存在チェック
		if (CheckOutputLocaiton((string)input[Constants.FIELD_SHOPOPERATOR_USABLE_OUTPUT_LOCATIONS]) == false)
		{
			var message = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_REGISTER_SETTING_ERROR_LOCATION)
				.Replace("@@ 1 @@", this.ErrorLocation.ToString());
			errorInfo.Add(message);
		}

		if (errorInfo.Any() == false) return;

		// エラーページへ
		Session[Constants.SESSION_KEY_ERROR_MSG] = WebSanitizer.HtmlEncodeChangeToBr(errorInfo.JoinToString(Environment.NewLine));
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
	}

	/// <summary>
	/// タグIDの存在チェック
	/// </summary>
	/// <param name="tagId">タグID</param>
	/// <returns>存在するか</returns>
	private bool CheckTagId(string tagId)
	{
		if (string.IsNullOrEmpty(tagId)) return true;

		var updateTagIdArray = StringUtility.SplitCsvLine(tagId);

		// タグ情報取得
		var tagInfo = new AffiliateTagSettingService().GetAllIncludeConditionModels();

		this.ErrorTagId = new StringBuilder();

		foreach (var updateTagIdItem in updateTagIdArray)
		{
			var existsCode = ((string.IsNullOrEmpty(updateTagIdItem) == false)
				&& tagInfo.Any(item => (item.AffiliateId == int.Parse(updateTagIdItem))));

			if (existsCode) continue;

			if (this.ErrorTagId.Length != 0)
			{
				this.ErrorTagId.Append(",");
			}
			this.ErrorTagId.Append(updateTagIdItem);

			return false;
		}

		return true;
	}

	/// <summary>
	/// 広告媒体区分の存在チェック
	/// </summary>
	/// <param name="mediaTypeId">広告媒体区分ID</param>
	/// <returns>存在するか</returns>
	private bool CheckMediaType(string mediaTypeId)
	{
		if (string.IsNullOrEmpty(mediaTypeId)) return true;

		var updateMediaTypes = StringUtility.SplitCsvLine(mediaTypeId);
		var mediaTypes = DomainFacade.Instance.AdvCodeService.GetAdvCodeMediaTypeListAll();

		this.ErrorMediaType = new StringBuilder();

		foreach (var updateMediaType in updateMediaTypes)
		{
			var isExists = mediaTypes.Any(mediaType => (mediaType.AdvcodeMediaTypeId == updateMediaType));
			if (isExists) continue;

			if (this.ErrorMediaType.Length > 0)
			{
				this.ErrorMediaType.Append(",");
			}
			this.ErrorMediaType.Append(updateMediaType);

			return false;
		}

		return true;
	}

	/// <summary>
	/// 設置箇所の存在チェック
	/// </summary>
	/// <param name="outputLocation">設置箇所</param>
	/// <returns>存在するか</returns>
	private bool CheckOutputLocaiton(string outputLocation)
	{
		if (string.IsNullOrEmpty(outputLocation)) return true;

		var updateLocations = StringUtility.SplitCsvLine(outputLocation);

		this.ErrorLocation = new StringBuilder();

		foreach (var updateLocation in updateLocations)
		{
			var isExists = TagSetting.GetInstance().Setting.TargetPages.Any(page => (page.Path == updateLocation));
			if (isExists) continue;

			if (this.ErrorLocation.Length > 0)
			{
				this.ErrorLocation.Append(",");
			}
			this.ErrorLocation.Append(updateLocation);

			return false;
		}

		return true;
	}

	/// <summary>
	/// 権限設定欄表示制御
	/// </summary>
	private void DisplayAuthorityFields()
	{
		if (this.SelectedOperator == null) return;

		DisplayTagAuthorityFields();
		DisplayMediaTypeAuthorityFields();
		DisplayLocationAuthorityFields();
	}

	/// <summary>
	/// 閲覧可能なタグID表示
	/// </summary>
	private void DisplayTagAuthorityFields()
	{
		// オペレータの閲覧可能なタグ取得
		if (this.SelectedTagIds.Any() == false)
		{
			return;
		}

		SetSelectedValue(rTagIdList, this.SelectedTagIds);
	}

	/// <summary>
	/// 閲覧可能な広告媒体区分表示
	/// </summary>
	private void DisplayMediaTypeAuthorityFields()
	{
		// オペレータの閲覧可能な広告媒体区分取得
		if (string.IsNullOrEmpty(this.SelectedOperator.UsableAdvcodeMediaTypeIds))
		{
			return;
		}

		SetSelectedValue(rMediaTypeList, this.SelectedMediaTypeIds);
	}

	/// <summary>
	/// 閲覧可能な設置箇所表示
	/// </summary>
	private void DisplayLocationAuthorityFields()
	{
		// オペレータの閲覧可能な設置箇所取得
		if (string.IsNullOrEmpty(this.SelectedOperator.UsableOutputLocations))
		{
			return;
		}

		SetSelectedValue(rLocationList, this.SelectedLocations);
	}

	/// <summary>
	/// 選択値設定
	/// </summary>
	/// <param name="rTarget">閲覧可能設定リピーター</param>
	/// <param name="selectedValues">選択値配列</param>
	private void SetSelectedValue(Repeater rTarget, List<string> selectedValues)
	{
		if ((rTarget == null) || (selectedValues.Any() == false)) return;

		foreach (RepeaterItem ri in rTarget.Items)
		{
			var hfValue = (HiddenField)ri.FindControl("hfValue");
			if (hfValue == null) continue;

			var cbIsSelected = (CheckBox)ri.FindControl("cbIsSelected");
			cbIsSelected.Checked = selectedValues.Contains(hfValue.Value);
		}
	}

	/// <summary>
	/// 閲覧可能設定区分表示変更
	/// </summary>
	private void SwitchDisplayAuthority()
	{
		var authorityKbns = new[]
		{
			Constants.FLG_TAG_AUTHORITY_KBN_TAG,
			Constants.FLG_TAG_AUTHORITY_KBN_MEDIA_TYPE,
			Constants.FLG_TAG_AUTHORITY_KBN_LOCATION,
		};

		foreach (var authorityKbn in authorityKbns)
		{
			var isMatchAuthorityKbn = (this.AuthorityKbn == authorityKbn);

			switch (authorityKbn)
			{
				case Constants.FLG_TAG_AUTHORITY_KBN_TAG:
					SwitchDisplay(lbSwitchTagAuthority, tblTagAuthority, isMatchAuthorityKbn);
					break;

				case Constants.FLG_TAG_AUTHORITY_KBN_MEDIA_TYPE:
					SwitchDisplay(lbSwitchMediaTypeAuthority, tblMediaTypeAuthority, isMatchAuthorityKbn);
					break;

				case Constants.FLG_TAG_AUTHORITY_KBN_LOCATION:
					SwitchDisplay(lbSwitchLocationAuthority, tblLocationAuthority, isMatchAuthorityKbn);
					break;
			}
		}
	}

	/// <summary>
	/// 表示変更処理
	/// </summary>
	/// <param name="lbSwitch">タブ切り替えボタン</param>
	/// <param name="tblAuthority">閲覧可能設定テーブル</param>
	/// <param name="isDisplay">表示状態</param>
	private void SwitchDisplay(LinkButton lbSwitch, HtmlControl tblAuthority, bool isDisplay)
	{
		lbSwitch.CssClass = isDisplay
			? "current"
			: string.Empty;
		tblAuthority.Visible = isDisplay;
	}

	/// <summary>
	/// 差分有無情報セット
	/// </summary>
	private void SetDifferential()
	{
		string[] operatorAuthority;
		List<string> selectedAuthority;
		switch (this.AuthorityKbn)
		{
			case Constants.FLG_TAG_AUTHORITY_KBN_TAG:
				operatorAuthority = this.SelectedOperator.GetUsableAffiliateTagIdsArray();
				selectedAuthority = this.SelectedTagIds;
				break;

			case Constants.FLG_TAG_AUTHORITY_KBN_MEDIA_TYPE:
				operatorAuthority = this.SelectedOperator.GetUsableAdvcodeMediaTypeIdsArray();
				selectedAuthority = this.SelectedMediaTypeIds;
				break;

			case Constants.FLG_TAG_AUTHORITY_KBN_LOCATION:
				operatorAuthority = this.SelectedOperator.GetUsableOutputLocationsArray();
				selectedAuthority = this.SelectedLocations;
				break;

			default:
				return;
		}

		var differenceElements = operatorAuthority
			.Union(selectedAuthority)
			.Except(operatorAuthority.Intersect(selectedAuthority))
			.ToArray();

		hfDifferential.Value = differenceElements.Any().ToString();
	}
	#endregion

	#region プロパティ
	/// <summary>現在のページ番号</summary>
	private int CurrentPageNo
	{
		get
		{
			int pageNo;
			return int.TryParse(this.Request[Constants.REQUEST_KEY_PAGE_NO], out pageNo) ? pageNo : 1;
		}
	}
	/// <summary>エラーがあったタグID</summary>
	private StringBuilder ErrorTagId { get; set; }
	/// <summary>エラーがあった広告媒体区分</summary>
	private StringBuilder ErrorMediaType { get; set; }
	/// <summary>エラーがあった設置箇所</summary>
	private StringBuilder ErrorLocation { get; set; }
	/// <summary>権限設定区分</summary>
	protected string AuthorityKbn
	{
		get { return (string)ViewState["AuthorityKbn"]; }
		private set { ViewState["AuthorityKbn"] = value; }
	}
	/// <summary>オペレーター名</summary>
	protected string OperatorName
	{
		get
		{
			if (this.SelectedOperator == null) return string.Empty;

			return this.SelectedOperator.Name;
		}
	}
	/// <summary>選択中オペレーター情報</summary>
	private ShopOperatorModel SelectedOperator
	{
		get { return (ShopOperatorModel)ViewState["SelectedOperator"]; }
		set { ViewState["SelectedOperator"] = value; }
	}
	/// <summary>タグID選択状態保持</summary>
	private List<string> SelectedTagIds
	{
		get { return (List<string>)ViewState["SelectedTagIds"]; }
		set { ViewState["SelectedTagIds"] = value; }
	}
	/// <summary>広告媒体区分選択状態保持</summary>
	private List<string> SelectedMediaTypeIds
	{
		get { return (List<string>)ViewState["SelectedMediaTypeIds"]; }
		set { ViewState["SelectedMediaTypeIds"] = value; }
	}
	/// <summary>設置箇所選択状態保持</summary>
	private List<string> SelectedLocations
	{
		get { return (List<string>)ViewState["SelectedLocations"]; }
		set { ViewState["SelectedLocations"] = value; }
	}
	#endregion
}