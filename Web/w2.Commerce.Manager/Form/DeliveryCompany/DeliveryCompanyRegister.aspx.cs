/*
=========================================================================================================
  Module      : 配送会社情報登録ページ(DeliveryCompanyRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.Domain.DeliveryCompany;

public partial class Form_DeliveryCompany_DeliveryCompanyRegister : DeliveryCompanyPage
{
	// Const shipping lead time default
	protected const string FLG_SHIPPING_LEAD_TIME_DEFAULT = "0";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// コンポーネント初期化
			InitializeComponents();

			// 値取得
			var deliveryCompanyInfo = GetValue();

			// 画面表示
			if (Session[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO] != null)
			{
				var deliveryCompanyInputInfo = (DeliveryCompanyInput)Session[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO];
				DisplayDeliveryCompanyInfo(deliveryCompanyInputInfo);
			}
			else
			{
				DisplayDeliveryCompanyInfo(deliveryCompanyInfo);
			}

			// Display Delivery Times Zone
			DisplayDeliveryTimesZone();

			trDeliveryCompanyTypeCreditcard.Visible = IsDeliveryCompanyTypeCreditcardByPaymentKbn();
			trDeliveryCompanyTypePostPayment.Visible = IsDeliveryCompanyTypePostPaymentByPaymentKbn();
			trDeliveryCompanyTypeNpPostPayment.Visible = Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED;
			trDeliveryCompanyTypeGooddeal.Visible = Constants.TWSHIPPING_GOODDEAL_OPTION_ENABLED;
			trDeliveryCompanyTypeGmoAtokara.Visible = Constants.PAYMENT_GMOATOKARA_ENABLED;
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 新規登録、コピー新規登録
		if ((this.ActionStatus == Constants.ACTION_STATUS_INSERT) || (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
		{
			trRegister.Visible = true;
			tdShippingIdEdit.Visible = true;
		}
		// 更新
		else if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			trEdit.Visible = true;
			tdShippingIdView.Visible = true;
		}
		CreateDeliveryCompanyTypeListItems();
		CreateDeadLineTimeDropDownList();
	}

	/// <summary>
	/// 出荷連携配送会社ドロップダウンリスト作成
	/// </summary>
	private void CreateDeliveryCompanyTypeListItems()
	{
		// 出荷連携配送会社(クレジットカード)
		switch (Constants.PAYMENT_CARD_KBN)
		{
			case Constants.PaymentCard.YamatoKwc:
				ddlDeliveryCompanyTypeCreditcard.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_DELIVERYCOMPANY, Constants.DELOVERY_COMPANY_TYPE_YAMATO));
				break;

			default:
				ddlDeliveryCompanyTypeCreditcard.Items.Add(string.Empty);
				break;
		}

		// 出荷連携配送会社(後払い)
		switch (Constants.PAYMENT_CVS_DEF_KBN)
		{
			case Constants.PaymentCvsDef.YamatoKa:
				ddlDeliveryCompanyTypePostPayment.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_DELIVERYCOMPANY, Constants.DELOVERY_COMPANY_TYPE_YAMATO));
				break;

			case Constants.PaymentCvsDef.Gmo:
				ddlDeliveryCompanyTypePostPayment.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_DELIVERYCOMPANY, Constants.DELOVERY_COMPANY_TYPE_GMO));
				break;

			case Constants.PaymentCvsDef.Atodene:
				ddlDeliveryCompanyTypePostPayment.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_DELIVERYCOMPANY, Constants.DELOVERY_COMPANY_TYPE_ATODENE));
				break;

			case Constants.PaymentCvsDef.Dsk:
				ddlDeliveryCompanyTypePostPayment.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_DELIVERYCOMPANY, Constants.DELOVERY_COMPANY_TYPE_DSK_DEFERRED));
				break;

			case Constants.PaymentCvsDef.Atobaraicom:
				ddlDeliveryCompanyTypePostPayment.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_DELIVERYCOMPANY, Constants.DELIVERY_COMPANY_TYPE_ATOBARAICOM));
				break;

			case Constants.PaymentCvsDef.Score:
				ddlDeliveryCompanyTypePostPayment.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_DELIVERYCOMPANY, Constants.DELIVERY_COMPANY_TYPE_SCORE_DEFERRED));
				break;

			default:
				ddlDeliveryCompanyTypePostPayment.Items.Add(string.Empty);
				break;
		}

		if (Constants.TWSHIPPING_GOODDEAL_OPTION_ENABLED)
		{
			ddlDeliveryCompanyTypeGooddeal.Items.AddRange(
				ValueText.GetValueItemArray(
					Constants.TABLE_DELIVERYCOMPANY,
					Constants.DELIVERY_COMPANY_TYPE_GOODDEAL));
		}
		
		ddlDeliveryCompanyTypeNpPostPayment.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_DELIVERYCOMPANY,
				Constants.DELIVERY_COMPANY_TYPE_NP));

		ddlDeliveryCompanyTypeGmoAtokara.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_DELIVERYCOMPANY,
				Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_GMO_ATOKARA_PAYMENT));
	}

	/// <summary>
	/// 値取得
	/// </summary>
	/// <returns>配送会社情報</returns>
	private DeliveryCompanyModel GetValue()
	{
		var deliveryCompanyInfo = new DeliveryCompanyModel();

		// コピー新規登録、更新
		if ((this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT) || (this.ActionStatus == Constants.ACTION_STATUS_UPDATE))
		{
			deliveryCompanyInfo = this.DeliveryCompanyService.Get(this.DeliveryCompanyId);

			if (deliveryCompanyInfo == null)
			{
				// エラーページ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
				Response.Redirect(string.Format("{0}{1}", Constants.PATH_ROOT, Constants.PAGE_MANAGER_ERROR));
			}
		}
		else if (this.ActionStatus != Constants.ACTION_STATUS_INSERT)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(string.Format("{0}{1}", Constants.PATH_ROOT, Constants.PAGE_MANAGER_ERROR));
		}

		return deliveryCompanyInfo;
	}

	/// <summary>
	/// 画面表示
	/// </summary>
	/// <param name="deliveryCompanyInfo">配送会社情報</param>
	private void DisplayDeliveryCompanyInfo(DeliveryCompanyModel deliveryCompanyInfo)
	{
		tbDeliveryCompanyId.Text = deliveryCompanyInfo.DeliveryCompanyId;
		lDeliveryCompanyId.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.DeliveryCompanyId);
		tbDeliveryCompanyName.Text = deliveryCompanyInfo.DeliveryCompanyName;
		ddlDeliveryCompanyTypeCreditcard.SelectedValue = (string.IsNullOrEmpty(deliveryCompanyInfo.DeliveryCompanyTypeCreditcard))
			? ddlDeliveryCompanyTypeCreditcard.Items[0].Value
			: deliveryCompanyInfo.DeliveryCompanyTypeCreditcard;
		ddlDeliveryCompanyTypePostPayment.SelectedValue = (string.IsNullOrEmpty(deliveryCompanyInfo.DeliveryCompanyTypePostPayment))
			? ddlDeliveryCompanyTypePostPayment.Items[0].Value
			: deliveryCompanyInfo.DeliveryCompanyTypePostPayment;
		ddlDeliveryCompanyTypeNpPostPayment.SelectedValue = (string.IsNullOrEmpty(deliveryCompanyInfo.DeliveryCompanyTypePostNpPayment))
			? ddlDeliveryCompanyTypeNpPostPayment.Items[0].Value
			: deliveryCompanyInfo.DeliveryCompanyTypePostNpPayment;
		if (Constants.TWSHIPPING_GOODDEAL_OPTION_ENABLED)
		{
			ddlDeliveryCompanyTypeGooddeal.SelectedValue = (string.IsNullOrEmpty(deliveryCompanyInfo.DeliveryCompanyTypeGooddeal))
				? ddlDeliveryCompanyTypeGooddeal.Items[0].Value
				: deliveryCompanyInfo.DeliveryCompanyTypeGooddeal;
		}
		ddlDeliveryCompanyTypeGmoAtokara.SelectedValue = (string.IsNullOrEmpty(deliveryCompanyInfo.DeliveryCompanyTypeGmoAtokaraPayment))
			? ddlDeliveryCompanyTypeGmoAtokara.Items[0].Value
			: deliveryCompanyInfo.DeliveryCompanyTypeGmoAtokaraPayment;
		tbDisplayOrder.Text = deliveryCompanyInfo.DisplayOrder.ToString();
		var deadLine =
			(string.IsNullOrEmpty(deliveryCompanyInfo.DeliveryCompanyDeadlineTime) == false)
				? deliveryCompanyInfo.DeliveryCompanyDeadlineTime.Split(':')
				: new string[2];
		tbDeliveryCompanyMailSizeLimit.Text = deliveryCompanyInfo.DeliveryCompanyMailSizeLimit.ToString();
		ddlTimeHour.SelectedValue = deadLine[0];
		ddlTimeMinute.SelectedValue = deadLine[1];
		cbShippingTimeSetFlg.Checked = deliveryCompanyInfo.IsValidShippingTimeSetFlg;
		tbShippingTimeId1.Text = deliveryCompanyInfo.ShippingTimeId1;
		tbShippingTimeMessage1.Text = deliveryCompanyInfo.ShippingTimeMessage1;
		tbShippingTimeId2.Text = deliveryCompanyInfo.ShippingTimeId2;
		tbShippingTimeMessage2.Text = deliveryCompanyInfo.ShippingTimeMessage2;
		tbShippingTimeId3.Text = deliveryCompanyInfo.ShippingTimeId3;
		tbShippingTimeMessage3.Text = deliveryCompanyInfo.ShippingTimeMessage3;
		tbShippingTimeId4.Text = deliveryCompanyInfo.ShippingTimeId4;
		tbShippingTimeMessage4.Text = deliveryCompanyInfo.ShippingTimeMessage4;
		tbShippingTimeId5.Text = deliveryCompanyInfo.ShippingTimeId5;
		tbShippingTimeMessage5.Text = deliveryCompanyInfo.ShippingTimeMessage5;
		tbShippingTimeId6.Text = deliveryCompanyInfo.ShippingTimeId6;
		tbShippingTimeMessage6.Text = deliveryCompanyInfo.ShippingTimeMessage6;
		tbShippingTimeId7.Text = deliveryCompanyInfo.ShippingTimeId7;
		tbShippingTimeMessage7.Text = deliveryCompanyInfo.ShippingTimeMessage7;
		tbShippingTimeId8.Text = deliveryCompanyInfo.ShippingTimeId8;
		tbShippingTimeMessage8.Text = deliveryCompanyInfo.ShippingTimeMessage8;
		tbShippingTimeId9.Text = deliveryCompanyInfo.ShippingTimeId9;
		tbShippingTimeMessage9.Text = deliveryCompanyInfo.ShippingTimeMessage9;
		tbShippingTimeId10.Text = deliveryCompanyInfo.ShippingTimeId10;
		tbShippingTimeMessage10.Text = deliveryCompanyInfo.ShippingTimeMessage10;
		cbDeliveryLeadTimeSetFlg.Checked = deliveryCompanyInfo.IsValidDeliveryLeadTimeSetFlg;
		tbShippingLeadTimeDefault.Text = StringUtility.ToEmpty(deliveryCompanyInfo.ShippingLeadTimeDefault);
		tbShippingTimeMessageMatching1.Text = deliveryCompanyInfo.ShippingTimeMatching1;
		tbShippingTimeMessageMatching2.Text = deliveryCompanyInfo.ShippingTimeMatching2;
		tbShippingTimeMessageMatching3.Text = deliveryCompanyInfo.ShippingTimeMatching3;
		tbShippingTimeMessageMatching4.Text = deliveryCompanyInfo.ShippingTimeMatching4;
		tbShippingTimeMessageMatching5.Text = deliveryCompanyInfo.ShippingTimeMatching5;
		tbShippingTimeMessageMatching6.Text = deliveryCompanyInfo.ShippingTimeMatching6;
		tbShippingTimeMessageMatching7.Text = deliveryCompanyInfo.ShippingTimeMatching7;
		tbShippingTimeMessageMatching8.Text = deliveryCompanyInfo.ShippingTimeMatching8;
		tbShippingTimeMessageMatching9.Text = deliveryCompanyInfo.ShippingTimeMatching9;
		tbShippingTimeMessageMatching10.Text = deliveryCompanyInfo.ShippingTimeMatching10;

		tbShippingTime.Visible = deliveryCompanyInfo.IsValidShippingTimeSetFlg;
		tbDeliveryLeadTimeSet.Visible = dvAddZone.Visible = cbDeliveryLeadTimeSetFlg.Checked;
	}
	/// <summary>
	/// 画面表示
	/// </summary>
	/// <param name="deliveryCompanyInfo">配送会社情報</param>
	private void DisplayDeliveryCompanyInfo(DeliveryCompanyInput deliveryCompanyInfo)
	{
		tbDeliveryCompanyId.Text = deliveryCompanyInfo.DeliveryCompanyId;
		lDeliveryCompanyId.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.DeliveryCompanyId);
		tbDeliveryCompanyName.Text = deliveryCompanyInfo.DeliveryCompanyName;
		ddlDeliveryCompanyTypeCreditcard.SelectedValue = (string.IsNullOrEmpty(deliveryCompanyInfo.DeliveryCompanyTypeCreditcard))
			? ddlDeliveryCompanyTypeCreditcard.Items[0].Value
			: deliveryCompanyInfo.DeliveryCompanyTypeCreditcard;
		ddlDeliveryCompanyTypePostPayment.SelectedValue = (string.IsNullOrEmpty(deliveryCompanyInfo.DeliveryCompanyTypePostPayment))
			? ddlDeliveryCompanyTypePostPayment.Items[0].Value
			: deliveryCompanyInfo.DeliveryCompanyTypePostPayment;
		ddlDeliveryCompanyTypeNpPostPayment.SelectedValue = (string.IsNullOrEmpty(deliveryCompanyInfo.DeliveryCompanyTypePostNpPayment))
			? ddlDeliveryCompanyTypeNpPostPayment.Items[0].Value
			: deliveryCompanyInfo.DeliveryCompanyTypePostNpPayment;
		if (Constants.TWSHIPPING_GOODDEAL_OPTION_ENABLED)
		{
			ddlDeliveryCompanyTypeGooddeal.SelectedValue = (string.IsNullOrEmpty(deliveryCompanyInfo.DeliveryCompanyTypeGooddeal))
				? ddlDeliveryCompanyTypeGooddeal.Items[0].Value
				: deliveryCompanyInfo.DeliveryCompanyTypeGooddeal;

		}
		ddlDeliveryCompanyTypeGmoAtokara.SelectedValue = (string.IsNullOrEmpty(deliveryCompanyInfo.DeliveryCompanyTypeGmoAtokaraPayment))
			? ddlDeliveryCompanyTypeGmoAtokara.Items[0].Value
			: deliveryCompanyInfo.DeliveryCompanyTypeGmoAtokaraPayment;
		var deadLine =
			(string.IsNullOrEmpty(deliveryCompanyInfo.DeliveryCompanyDeadlineTime) == false)
				? deliveryCompanyInfo.DeliveryCompanyDeadlineTime.Split(':')
				: new string[2];
		ddlTimeHour.SelectedValue = deadLine[0];
		ddlTimeMinute.SelectedValue = deadLine[1];
		tbDisplayOrder.Text = deliveryCompanyInfo.DisplayOrder;
		cbShippingTimeSetFlg.Checked = (deliveryCompanyInfo.ShippingTimeSetFlg == Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID);
		tbShippingTimeId1.Text = deliveryCompanyInfo.ShippingTimeId1;
		tbShippingTimeMessage1.Text = deliveryCompanyInfo.ShippingTimeMessage1;
		tbShippingTimeId2.Text = deliveryCompanyInfo.ShippingTimeId2;
		tbShippingTimeMessage2.Text = deliveryCompanyInfo.ShippingTimeMessage2;
		tbShippingTimeId3.Text = deliveryCompanyInfo.ShippingTimeId3;
		tbShippingTimeMessage3.Text = deliveryCompanyInfo.ShippingTimeMessage3;
		tbShippingTimeId4.Text = deliveryCompanyInfo.ShippingTimeId4;
		tbShippingTimeMessage4.Text = deliveryCompanyInfo.ShippingTimeMessage4;
		tbShippingTimeId5.Text = deliveryCompanyInfo.ShippingTimeId5;
		tbShippingTimeMessage5.Text = deliveryCompanyInfo.ShippingTimeMessage5;
		tbShippingTimeId6.Text = deliveryCompanyInfo.ShippingTimeId6;
		tbShippingTimeMessage6.Text = deliveryCompanyInfo.ShippingTimeMessage6;
		tbShippingTimeId7.Text = deliveryCompanyInfo.ShippingTimeId7;
		tbShippingTimeMessage7.Text = deliveryCompanyInfo.ShippingTimeMessage7;
		tbShippingTimeId8.Text = deliveryCompanyInfo.ShippingTimeId8;
		tbShippingTimeMessage8.Text = deliveryCompanyInfo.ShippingTimeMessage8;
		tbShippingTimeId9.Text = deliveryCompanyInfo.ShippingTimeId9;
		tbShippingTimeMessage9.Text = deliveryCompanyInfo.ShippingTimeMessage9;
		tbShippingTimeId10.Text = deliveryCompanyInfo.ShippingTimeId10;
		tbShippingTimeMessage10.Text = deliveryCompanyInfo.ShippingTimeMessage10;
		cbDeliveryLeadTimeSetFlg.Checked = (deliveryCompanyInfo.DeliveryLeadTimeSetFlg == Constants.FLG_DELIVERYCOMPANY_LEAD_TIME_SET_FLG_VALID);
		tbShippingLeadTimeDefault.Text = StringUtility.ToEmpty(deliveryCompanyInfo.ShippingLeadTimeDefault);
		tbShippingTime.Visible = (deliveryCompanyInfo.ShippingTimeSetFlg == Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID);
		tbDeliveryLeadTimeSet.Visible = dvAddZone.Visible = cbDeliveryLeadTimeSetFlg.Checked;
		tbShippingTimeMessageMatching1.Text = deliveryCompanyInfo.ShippingTimeMatching1;
		tbShippingTimeMessageMatching2.Text = deliveryCompanyInfo.ShippingTimeMatching2;
		tbShippingTimeMessageMatching3.Text = deliveryCompanyInfo.ShippingTimeMatching3;
		tbShippingTimeMessageMatching4.Text = deliveryCompanyInfo.ShippingTimeMatching4;
		tbShippingTimeMessageMatching5.Text = deliveryCompanyInfo.ShippingTimeMatching5;
		tbShippingTimeMessageMatching6.Text = deliveryCompanyInfo.ShippingTimeMatching6;
		tbShippingTimeMessageMatching7.Text = deliveryCompanyInfo.ShippingTimeMatching7;
		tbShippingTimeMessageMatching8.Text = deliveryCompanyInfo.ShippingTimeMatching8;
		tbShippingTimeMessageMatching9.Text = deliveryCompanyInfo.ShippingTimeMatching9;
		tbShippingTimeMessageMatching10.Text = deliveryCompanyInfo.ShippingTimeMatching10;
	}

	/// <summary>
	/// Display delivery times zone
	/// </summary>
	private void DisplayDeliveryTimesZone()
	{
		var deliveryLeadTimeAllInfo = new List<DeliveryLeadTimeInput>();
		switch (this.ActionStatus)
		{
			// Get all delivery lead time
			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
				Array.ForEach(this.DeliveryLeadTimeService.GetAll(this.LoginOperatorShopId, this.DeliveryCompanyId),
					item => deliveryLeadTimeAllInfo.Add(new DeliveryLeadTimeInput(item)));
				if (deliveryLeadTimeAllInfo.Count != 0)
				{
					// Delivery lead times zone
					var prefecturesCount = this.PrefecturesList.Length;
					var deliveryLeadTimesZoneInfo = deliveryLeadTimeAllInfo
						.Where(item => (Int32.Parse(item.LeadTimeZoneNo) <= prefecturesCount));
					if (Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO] != null)
					{
						deliveryLeadTimesZoneInfo = (List<DeliveryLeadTimeInput>)Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO];
						Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO] = null;
					}
					rDeliveryLeadTimeZone.DataSource = deliveryLeadTimesZoneInfo;

					// Delivery lead times zone special
					var deliveryLeadTimesSpecialInfo = deliveryLeadTimeAllInfo
						.Where(item => (Int32.Parse(item.LeadTimeZoneNo) > prefecturesCount));
					if (Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO + "_special"] != null)
					{
						deliveryLeadTimesSpecialInfo = (List<DeliveryLeadTimeInput>)Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO + "_special"];
						Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO + "_special"] = null;
					}
					rDeliveryLeadTimeZoneSpecial.DataSource = deliveryLeadTimesSpecialInfo;
				}
				else
				{
					rDeliveryLeadTimeZone.DataSource = GetDefaultDeliveryLeadTimes();
				}
				RefreshComponentsDeliveryLeadTimeSet_OnCheckedChanged(null, null);
				break;

			// Get default delivery lead times
			case Constants.ACTION_STATUS_INSERT:
				// Delivery lead times zone
				if (Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO] != null)
				{
					var deliveryLeadTimesZoneInfo = (List<DeliveryLeadTimeInput>)Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO];
					Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO] = null;
					rDeliveryLeadTimeZone.DataSource = deliveryLeadTimesZoneInfo;

					// Delivery lead times zone special
					if (Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO + "_special"] != null)
					{
						var deliveryLeadTimesSpecialInfo = (List<DeliveryLeadTimeInput>)Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO + "_special"];
						Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO + "_special"] = null;
						rDeliveryLeadTimeZoneSpecial.DataSource = deliveryLeadTimesSpecialInfo;
					}
				}
				else
				{
					rDeliveryLeadTimeZone.DataSource = GetDefaultDeliveryLeadTimes();
				}
				RefreshComponentsDeliveryLeadTimeSet_OnCheckedChanged(null, null);
				break;

			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(string.Format("{0}{1}", Constants.PATH_ROOT, Constants.PAGE_MANAGER_ERROR));
				break;
		}

		rDeliveryLeadTimeZoneSpecial.DataBind();
		rDeliveryLeadTimeZone.DataBind();
	}

	/// <summary>
	/// Get default delivery lead times
	/// </summary>
	/// <returns>List delivery lead time input</returns>
	protected List<DeliveryLeadTimeInput> GetDefaultDeliveryLeadTimes()
	{
		var deliveryLeadTimeInfo = new List<DeliveryLeadTimeInput>();
		for (int index = 0; index < this.PrefecturesList.Length; index++)
		{
			var input = new DeliveryLeadTimeInput()
			{
				ShopId = this.LoginOperatorShopId,
				DeliveryCompanyId = tbDeliveryCompanyId.Text.Trim(),
				LeadTimeZoneName = this.PrefecturesList[index],
				LeadTimeZoneNo = StringUtility.ToEmpty(index + 1),
				Zip = string.Empty,
				ShippingLeadTime = FLG_SHIPPING_LEAD_TIME_DEFAULT,
				LastChanged = this.LoginOperatorName,
			};
			deliveryLeadTimeInfo.Add(input);
		}

		return deliveryLeadTimeInfo;
	}

	/// <summary>
	/// リフレッシュ処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void RefreshComponents_OnCheckedChanged(object sender, System.EventArgs e)
	{
		tbShippingTime.Visible = (cbShippingTimeSetFlg.Checked);
	}

	/// <summary>
	/// リードタイム設定の利用の有無
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void RefreshComponentsDeliveryLeadTimeSet_OnCheckedChanged(object sender, System.EventArgs e)
	{
		tbDeliveryLeadTimeSet.Visible = trShippingLeadTimeDefault.Visible = dvAddZone.Visible = (cbDeliveryLeadTimeSetFlg.Checked);
	}

	/// <summary>
	/// Delete delivery lead time zone special item
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rDeliveryLeadTimeZoneSpecial_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		if (e.CommandName == "DeleteZone")
		{
			((HtmlGenericControl)e.Item.FindControl("tbodyAddZoneItem")).Visible = false;
		}
	}

	/// <summary>
	/// Add delivery lead time zone special item
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddDeliveryLeadTimeZoneSpecial_Click(object sender, EventArgs e)
	{
		// Get list delivery lead time zone special
		var listDeliveryLeadTimeZoneSpecial = GetDeliveryLeadTimeZoneSpecialFromInput();

		// New item delivery lead time
		var deliveryLeadTimeInput = new DeliveryLeadTimeInput()
		{
			ShippingLeadTime = FLG_SHIPPING_LEAD_TIME_DEFAULT,
		};
		listDeliveryLeadTimeZoneSpecial.Add(deliveryLeadTimeInput);

		// Data Binding
		rDeliveryLeadTimeZoneSpecial.DataSource = listDeliveryLeadTimeZoneSpecial;
		rDeliveryLeadTimeZoneSpecial.DataBind();
	}

	/// <summary>
	/// Get list delivery lead time zone special
	/// </summary>
	/// <returns>List delivery lead time input</returns>
	private List<DeliveryLeadTimeInput> GetDeliveryLeadTimeZoneSpecialFromInput()
	{
		var listDeliveryLeadTimeZoneSpecial = new List<DeliveryLeadTimeInput>();

		// Lead time zone no
		var zoneNo = this.PrefecturesList.Length + 1;

		foreach (RepeaterItem ri in rDeliveryLeadTimeZoneSpecial.Items)
		{
			if (((HtmlGenericControl)ri.FindControl("tbodyAddZoneItem")).Visible)
			{
				var input = new DeliveryLeadTimeInput()
				{
					LeadTimeZoneName = ((TextBox)ri.FindControl("tbZoneName")).Text.Trim(),
					Zip = ((TextBox)ri.FindControl("tbZip")).Text.Trim(),
					ShippingLeadTime = ((TextBox)ri.FindControl("tbShippingLeadTime")).Text.Trim(),
					LeadTimeZoneNo = StringUtility.ToEmpty(zoneNo),
					DeliveryCompanyId = tbDeliveryCompanyId.Text.Trim(),
					LastChanged = this.LoginOperatorName,
					ShopId = this.LoginOperatorShopId
				};
				listDeliveryLeadTimeZoneSpecial.Add(input);

				zoneNo++;
			}
		}

		return listDeliveryLeadTimeZoneSpecial;
	}

	/// <summary>
	/// Get list delivery lead time zone basic
	/// </summary>
	/// <returns>List delivery lead time input</returns>
	private List<DeliveryLeadTimeInput> GetDeliveryLeadTimeZoneBasicFromInput()
	{
		var listDeliveryLeadTimeZone = new List<DeliveryLeadTimeInput>();

		if (cbDeliveryLeadTimeSetFlg.Checked == false) return GetDefaultDeliveryLeadTimes();

		// Lead time zone no
		var zoneNo = 1;

		foreach (RepeaterItem ri in rDeliveryLeadTimeZone.Items)
		{
			var input = new DeliveryLeadTimeInput()
			{
				ShopId = this.LoginOperatorShopId,
				DeliveryCompanyId = tbDeliveryCompanyId.Text.Trim(),
				LeadTimeZoneNo = StringUtility.ToEmpty(zoneNo),
				LeadTimeZoneName = ((Label)ri.FindControl("lLeadTimeZoneName")).Text.Trim(),
				Zip = string.Empty,
				ShippingLeadTime = ((TextBox)ri.FindControl("tbShippingLeadTime")).Text.Trim(),
				LastChanged = this.LoginOperatorName,

			};
			listDeliveryLeadTimeZone.Add(input);

			zoneNo++;
		}

		return listDeliveryLeadTimeZone;
	}

	/// <summary>
	/// 「確認する」ボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, System.EventArgs e)
	{
		// 入力情報
		var shippingCompnanyInfo = CreateInputData();
		Session[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO] = shippingCompnanyInfo;

		// 入力チェック
		var errorMessages = string.Empty;

		switch (StringUtility.ToEmpty(this.ActionStatus))
		{
			// 新規登録、コピー新規登録
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				errorMessages = shippingCompnanyInfo.Validate(DeliveryCompanyInput.EnumDeliveryCompanyInputValidationKbn.DeliveryCompanyRegist);
				break;

			// 更新
			case Constants.ACTION_STATUS_UPDATE:
				errorMessages = shippingCompnanyInfo.Validate(DeliveryCompanyInput.EnumDeliveryCompanyInputValidationKbn.DeliveryCompanyModify);
				break;
		}

		// Get list delivery lead time zone basic
		var listDeliveryLeadTimeZoneBasic = GetDeliveryLeadTimeZoneBasicFromInput();
		Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO] = listDeliveryLeadTimeZoneBasic;

		// Check validate delivery lead time zone basic
		errorMessages += ValidateDeliveryLeadTimeZone(listDeliveryLeadTimeZoneBasic);

		// Get list delivery lead time zone special
		var listDeliveryLeadTimeZoneSpecial = GetDeliveryLeadTimeZoneSpecialFromInput();
		Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO + "_special"] = listDeliveryLeadTimeZoneSpecial;

		if (listDeliveryLeadTimeZoneSpecial.Count != 0)
		{
			// Check validate list delivery lead time zone special
			errorMessages += ValidateDeliveryLeadTimeZone(listDeliveryLeadTimeZoneSpecial, true);
		}

		if (string.IsNullOrEmpty(errorMessages) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(string.Format("{0}{1}", Constants.PATH_ROOT, Constants.PAGE_MANAGER_ERROR));
		}

		// 確認画面へ遷移
		Response.Redirect(CreateDeliveryCompanyUrl(StringUtility.ToEmpty(this.DeliveryCompanyId), Constants.PAGE_MANAGER_DELIVERY_COMPANY_CONFIRM, this.ActionStatus));
	}

	/// <summary>
	/// Validate delivery lead time zone
	/// </summary>
	/// <param name="deliveryLeadTimeInputList">Delivery lead time input list</param>
	/// <param name="isDeliveryLeadTimeZoneSpecial">Is delivery lead time zone special</param>
	/// <returns>Error messages</returns>
	private string ValidateDeliveryLeadTimeZone(IEnumerable<DeliveryLeadTimeInput> deliveryLeadTimeInputList, bool isDeliveryLeadTimeZoneSpecial = false)
	{
		var errorMessages = new StringBuilder();
		foreach (var item in deliveryLeadTimeInputList)
		{
			var errorMessage = item.Validate(this.ActionStatus, isDeliveryLeadTimeZoneSpecial);
			if (string.IsNullOrEmpty(errorMessage))
			{
				errorMessages.Append(errorMessage);
			}
			else
			{
				errorMessages.AppendLine(string.Format("{0}: {1}", item.LeadTimeZoneName, errorMessage));
			}
		};

		return errorMessages.ToString();
	}

	/// <summary>
	/// 入力情報作成
	/// </summary>
	/// <returns>配送会社情報</returns>
	private DeliveryCompanyInput CreateInputData()
	{
		var input = new DeliveryCompanyInput
		{
			DeliveryCompanyId = StringUtility.ToHankaku(tbDeliveryCompanyId.Text.Trim()),
			DeliveryCompanyName = tbDeliveryCompanyName.Text.Trim(),
			DeliveryCompanyTypeCreditcard = (IsDeliveryCompanyTypeCreditcardByPaymentKbn())
				? ddlDeliveryCompanyTypeCreditcard.SelectedValue
				: string.Empty,
			DeliveryCompanyTypePostPayment =
				(IsDeliveryCompanyTypePostPaymentByPaymentKbn())
					? ddlDeliveryCompanyTypePostPayment.SelectedValue
					: string.Empty,
			DeliveryCompanyTypePostNpPayment =
				(Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED)
					? ddlDeliveryCompanyTypeNpPostPayment.SelectedValue
					: Constants.FLG_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_POST_NP_PAYMENT_DEFAULT,
			DeliveryCompanyTypeGooddeal =
				(Constants.TWSHIPPING_GOODDEAL_OPTION_ENABLED)
					? ddlDeliveryCompanyTypeGooddeal.SelectedValue
					: string.Empty,
			DeliveryCompanyDeadlineTime =
				((string.IsNullOrEmpty(ddlTimeHour.SelectedValue) == false)
					|| (string.IsNullOrEmpty(ddlTimeMinute.SelectedValue) == false))
					? (ddlTimeHour.SelectedValue + ":" + ddlTimeMinute.SelectedValue)
					: string.Empty,
			DisplayOrder = tbDisplayOrder.Text.Trim(),
			DeliveryCompanyMailSizeLimit = (string.IsNullOrEmpty(tbDeliveryCompanyMailSizeLimit.Text))
				? "999"
				: tbDeliveryCompanyMailSizeLimit.Text.Trim(),
			ShippingTimeSetFlg = cbShippingTimeSetFlg.Checked ? Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_VALID : Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_INVALID,
			ShippingTimeId1 = tbShippingTimeId1.Text.Trim(),
			ShippingTimeMessage1 = tbShippingTimeMessage1.Text.Trim(),
			ShippingTimeMatching1 = tbShippingTimeMessageMatching1.Text.Trim(),
			ShippingTimeId2 = tbShippingTimeId2.Text.Trim(),
			ShippingTimeMessage2 = tbShippingTimeMessage2.Text.Trim(),
			ShippingTimeMatching2 = tbShippingTimeMessageMatching2.Text.Trim(),
			ShippingTimeId3 = tbShippingTimeId3.Text.Trim(),
			ShippingTimeMessage3 = tbShippingTimeMessage3.Text.Trim(),
			ShippingTimeMatching3 = tbShippingTimeMessageMatching3.Text.Trim(),
			ShippingTimeId4 = tbShippingTimeId4.Text.Trim(),
			ShippingTimeMessage4 = tbShippingTimeMessage4.Text.Trim(),
			ShippingTimeMatching4 = tbShippingTimeMessageMatching4.Text.Trim(),
			ShippingTimeId5 = tbShippingTimeId5.Text.Trim(),
			ShippingTimeMessage5 = tbShippingTimeMessage5.Text.Trim(),
			ShippingTimeMatching5 = tbShippingTimeMessageMatching5.Text.Trim(),
			ShippingTimeId6 = tbShippingTimeId6.Text.Trim(),
			ShippingTimeMessage6 = tbShippingTimeMessage6.Text.Trim(),
			ShippingTimeMatching6 = tbShippingTimeMessageMatching6.Text.Trim(),
			ShippingTimeId7 = tbShippingTimeId7.Text.Trim(),
			ShippingTimeMessage7 = tbShippingTimeMessage7.Text.Trim(),
			ShippingTimeMatching7 = tbShippingTimeMessageMatching7.Text.Trim(),
			ShippingTimeId8 = tbShippingTimeId8.Text.Trim(),
			ShippingTimeMessage8 = tbShippingTimeMessage8.Text.Trim(),
			ShippingTimeMatching8 = tbShippingTimeMessageMatching8.Text.Trim(),
			ShippingTimeId9 = tbShippingTimeId9.Text.Trim(),
			ShippingTimeMessage9 = tbShippingTimeMessage9.Text.Trim(),
			ShippingTimeMatching9 = tbShippingTimeMessageMatching9.Text.Trim(),
			ShippingTimeId10 = tbShippingTimeId10.Text.Trim(),
			ShippingTimeMessage10 = tbShippingTimeMessage10.Text.Trim(),
			ShippingTimeMatching10 = tbShippingTimeMessageMatching10.Text.Trim(),
			DeliveryLeadTimeSetFlg = cbDeliveryLeadTimeSetFlg.Checked ? Constants.FLG_DELIVERYCOMPANY_LEAD_TIME_SET_FLG_VALID : Constants.FLG_DELIVERYCOMPANY_LEAD_TIME_SET_FLG_INVALID,
			ShippingLeadTimeDefault = (cbDeliveryLeadTimeSetFlg.Checked) ? tbShippingLeadTimeDefault.Text.Trim() : StringUtility.ToEmpty(Constants.FLG_SHIPPING_LEAD_TIME_DEFAULT),
			DeliveryCompanyTypeGmoAtokaraPayment =
				Constants.PAYMENT_GMOATOKARA_ENABLED
					? ddlDeliveryCompanyTypeGmoAtokara.SelectedValue
					: string.Empty,
		};

		return input;
	}

	/// <summary>
	/// 当日出荷締め時間ドロップダウンリスト作成
	/// </summary>
	private void CreateDeadLineTimeDropDownList()
	{
		ddlTimeHour.Items.Add(string.Empty);
		ddlTimeMinute.Items.Add(string.Empty);
		ddlTimeHour.Items.AddRange(DateTimeUtility.GetHourListItem("00"));
		ddlTimeMinute.Items.AddRange(DateTimeUtility.GetMinuteListItem("00"));
	}
}
