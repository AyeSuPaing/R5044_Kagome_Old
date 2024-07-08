/*
=========================================================================================================
  Module      :広告コード閲覧権限ページ(AdvertisementCodeAuthorityRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.MasterExport;
using w2.Common.Web;
using w2.Domain.AdvCode;
using w2.Domain.AdvCode.Helper;
using w2.Domain.ShopOperator;

/// <summary>
/// AdvertisementCodeAuthorityRegisterの概要の説明です
/// </summary>
public partial class Form_AdvertisementCodeAuthority_AdvertisementCodeAuthorityRegister : BasePage
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
			GetOperatorList();

			ViewAdvCodeList();

			// 完了画面の場合
			if (StringUtility.ToEmpty(this.Session[Constants.SESSION_KEY_ACTION_STATUS])
				!= Constants.ACTION_STATUS_COMPLETE) return;
			divComp.Visible = true;
			this.Session[Constants.SESSION_KEY_ACTION_STATUS] = null;
		}
	}

	/// <summary>
	/// オペレータ一覧を取得する
	/// </summary>
	protected void GetOperatorList()
	{
		var htInput = new Hashtable
		{
			{ Constants.FIELD_SHOPOPERATOR_SHOP_ID, this.LoginOperatorShopId },
			{ Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG, this.LoginOperatorMenuAccessLevel },
			{ Constants.FIELD_COMMON_BEGIN_NUM, (this.CurrentPageNo - 1) * Constants.CONST_DISP_CONTENTS_AD_CODEAUTHORITY_REGISTER + 1 },
			{ Constants.FIELD_COMMON_END_NUM, this.CurrentPageNo * Constants.CONST_DISP_CONTENTS_AD_CODEAUTHORITY_REGISTER }
		};

		var dvOperatorList = new ShopOperatorService().GetOperatorList(htInput);

		if (dvOperatorList.Length != 0)
		{
			var iTotal = int.Parse(dvOperatorList[0].RowCount.ToString());

			var strNextUrl =
				new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_AUTHORITY_REGISTER)
					.AddParam(Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE, tbSearchAdvertisementCode.Text.Trim())
					.AddParam(Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_ADVCODE_DEFAULT)
					.AddParam(Constants.REQUEST_KEY_SHOP_OPERATOR_ID, hfOperatorId.Value).CreateUrl();

			lbPager.Text = WebPager.CreateListPager(
				iTotal,
				this.CurrentPageNo,
				strNextUrl,
				Constants.CONST_DISP_CONTENTS_AD_CODEAUTHORITY_REGISTER);

			rOperatorList.DataSource = dvOperatorList;
			rOperatorList.DataBind();

			var operatorId = this.Request[Constants.REQUEST_KEY_OPERATOR_ID];

			if (string.IsNullOrEmpty(operatorId) == false)
			{
				foreach (RepeaterItem ri in rOperatorList.Items)
				{
					if (((HiddenField)ri.FindControl("hfOperatorId")).Value == operatorId)
					{
						((RadioButton)ri.FindControl("rbOperator")).Checked = true;

						lOperatorName.Text = ((HiddenField)ri.FindControl("hfOperatorName")).Value;
						hfOperatorId.Value = ((HiddenField)ri.FindControl("hfOperatorId")).Value;

						var dvOperator = new ShopOperatorService().Get(this.LoginOperatorShopId, operatorId);
						tbAdAuthorityFields.Text = StringUtility.ToEmpty(dvOperator.UsableAdvcodeNosInReport.Replace(",", ",\n"));

						break;
					}
				}
			}
			else
			{
				((RadioButton)rOperatorList.Items[0].FindControl("rbOperator")).Checked = true;

				lOperatorName.Text = ((HiddenField)rOperatorList.Items[0].FindControl("hfOperatorName")).Value;
				hfOperatorId.Value = ((HiddenField)rOperatorList.Items[0].FindControl("hfOperatorId")).Value;

				var dvOperator = new ShopOperatorService().Get(this.LoginOperatorShopId, hfOperatorId.Value);
				tbAdAuthorityFields.Text = StringUtility.ToEmpty(dvOperator.UsableAdvcodeNosInReport.Replace(",", ",\n"));
			}
		}
		else
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
	}

	/// <summary>
	/// 広告コード一覧を取得する
	/// </summary>
	private void ViewAdvCodeList()
	{
		// リクエスト情報取得
		var parameters = GetParameters(this.Request);

		// 検索値を画面にセット
		SetParameters(parameters);

		// 広告コード情報取得
		var searchCondition = CreateSearchCondition(parameters);

		var advService = new AdvCodeService();
		searchCondition.EndRowNumber = advService.GetAdvCodeSearchHitCount(searchCondition);
		var advCodeData = advService.SearchAdvCode(searchCondition);

		if (advCodeData.Length != 0)
		{
			trListError.Visible = false;
		}
		else
		{
			// 一覧非表示・エラー表示制御
			trAdvCodeListError.Visible = true;
			tdAdvCodeErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		rAdvCodeList.DataSource = advCodeData;
		rAdvCodeList.DataBind();
	}

	/// <summary>
	/// リクエストURLのパラメータを取得
	/// </summary>
	/// <param name="requestParams">リクエストパラメタ</param>
	/// <returns>リクエストパラメタ</returns>
	protected Hashtable GetParameters(HttpRequest requestParams)
	{
		var resultData = new Hashtable
		{
			{ Constants.FIELD_ADVCODE_DEPT_ID, this.LoginOperatorDeptId },
			{
				Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE,
				StringUtility.ToEmpty(requestParams[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE])
			},
			{ Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_ADVCODE_DEFAULT }

		};

		return resultData;
	}

	/// <summary>
	/// 画面に検索値をセット
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	private void SetParameters(Hashtable parameters)
	{
		tbSearchAdvertisementCode.Text = (string)parameters[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE];
	}

	/// <summary>
	/// 検索条件作成
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>広告コード検索条件</returns>
	private AdvCodeListSearchCondition CreateSearchCondition(Hashtable parameters)
	{
		var condition = new AdvCodeListSearchCondition
		{
			DeptId = this.LoginOperatorDeptId,
			AdvertisementCode = (string)parameters[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE],
			MediaName = string.Empty,
			AdvcodeMediaTypeId = string.Empty,
			BeginRowNumber = 1,
			EndRowNumber = 1,
			SortKbn = (string)parameters[Constants.REQUEST_KEY_SORT_KBN],
			ValidFlg = string.Empty
		};

		return condition;
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		RedirectToSearch();
	}

	/// <summary>
	/// URL作成してリダイレクト
	/// </summary>
	protected void RedirectToSearch()
	{
		var urlCreator =
			new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_AUTHORITY_REGISTER)
				.AddParam(Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE, tbSearchAdvertisementCode.Text.Trim())
				.AddParam(Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_ADVCODE_DEFAULT)
				.AddParam(Constants.REQUEST_KEY_SHOP_OPERATOR_ID, hfOperatorId.Value.Trim())
				.AddParam(Constants.REQUEST_KEY_PAGE_NO, this.CurrentPageNo.ToString());

		Response.Redirect(urlCreator.CreateUrl());
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
			lOperatorName.Text = ((HiddenField)ri.FindControl("hfOperatorName")).Value;
		}

		var dvOperator = new ShopOperatorService().Get(this.LoginOperatorShopId, hfOperatorId.Value);
		tbAdAuthorityFields.Text = StringUtility.ToEmpty(dvOperator.UsableAdvcodeNosInReport.Replace(",", ",\n"));

		RedirectToSearch();
	}

	/// <summary>
	/// 更新する
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_OnClick(object sender, EventArgs e)
	{
		var ht = new Hashtable
		{
			{
				Constants.FIELD_SHOPOPERATOR_USABLE_ADVCODE_NOS_IN_REPORT,
				MasterExportSettingUtility.GetFieldsEscape(tbAdAuthorityFields.Text.Trim())
			}
		};

		InputValidate(ht);

		using (var sqlAccessor = new SqlAccessor())
		using (var sqlStatement = new SqlStatement("ShopOperator", "UpdateOperatorAdvAuthority"))
		{

			ht.Add(Constants.FIELD_SHOPOPERATOR_SHOP_ID, this.LoginOperatorShopId);
			ht.Add(Constants.FIELD_SHOPOPERATOR_LAST_CHANGED, this.LoginOperatorName);
			ht.Add(Constants.FIELD_SHOPOPERATOR_OPERATOR_ID, hfOperatorId.Value.Trim());

			sqlStatement.ExecStatementWithOC(sqlAccessor, ht);
		}

		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COMPLETE;
		RedirectToSearch();

	}

	/// <summary>
	/// 入力チェック
	/// </summary>
	/// <param name="input">チェック値</param>
	private void InputValidate(Hashtable input)
	{
		var errorMessages = Validator.Validate("AdAuthoritySettingModify", input);

		if (string.IsNullOrEmpty(errorMessages) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 閲覧可能な広告コード存在するかどうかのチェック
		if (CheckAdvertisementCode((string)input[Constants.FIELD_SHOPOPERATOR_USABLE_ADVCODE_NOS_IN_REPORT]) == false)
		{
			var errorInfo = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_REGISTER_SETTING_ERRORADVCODE);

			// エラーページへ
			if (this.ErrorAdvertisementCode.Length != 0)
			{
				errorInfo += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIELD_NOT_APPLICABLE)
					.Replace("@@ 1 @@", WebSanitizer.HtmlEncode(this.ErrorAdvertisementCode));
			}

			Session[Constants.SESSION_KEY_ERROR_MSG] = errorInfo;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// 入力広告コードが存在するかをチェック
	/// </summary>
	/// <param name="advCodes">入力広告コード</param>
	private bool CheckAdvertisementCode(string advCodes)
	{
		if (string.IsNullOrEmpty(advCodes)) return true;

		var updateAdCodeArray = StringUtility.SplitCsvLine(advCodes);

		// リクエスト情報取得
		var parameters = new Hashtable
		{
			{ Constants.FIELD_ADVCODE_DEPT_ID, this.LoginOperatorDeptId },
			{ Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE, "" },
			{ Constants.REQUEST_KEY_SORT_KBN, Constants.KBN_SORT_ADVCODE_DEFAULT }

		};

		// 広告コード情報取得
		var searchCondition = CreateSearchCondition(parameters);

		var advService = new AdvCodeService();
		searchCondition.EndRowNumber = advService.GetAdvCodeSearchHitCount(searchCondition);
		var advCodeData = advService.SearchAdvCode(searchCondition);

		var adCodeCsv = new StringBuilder();
		this.ErrorAdvertisementCode = new StringBuilder();

		foreach (var updateAdCodeItem in updateAdCodeArray)
		{
			var existsCode = advCodeData.Any(item => (item.AdvertisementCode == updateAdCodeItem));

			if (existsCode)
			{
				if (adCodeCsv.Length != 0)
				{
					adCodeCsv.Append(", ");
				}
				adCodeCsv.Append(updateAdCodeItem);
			}
			else
			{
				if (this.ErrorAdvertisementCode.Length != 0)
				{
					this.ErrorAdvertisementCode.Append(", ");
				}
				this.ErrorAdvertisementCode.Append(updateAdCodeItem);

				return false;
			}
		}

		return true;
	}

	#region プロパティ
	/// <summary>現在のページ番号</summary>
	private int CurrentPageNo
	{
		get
		{
			var pageNo = 1;
			return int.TryParse(this.Request[Constants.REQUEST_KEY_PAGE_NO], out pageNo) ? pageNo : 1;
		}
	}
	/// <summary>エラーがあった広告コード</summary>
	public StringBuilder ErrorAdvertisementCode{ get; set; }
	#endregion
}