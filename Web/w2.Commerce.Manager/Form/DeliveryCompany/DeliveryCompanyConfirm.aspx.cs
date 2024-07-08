/*
=========================================================================================================
  Module      : 配送会社情報確認ページ(DeliveryCompanyConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.Domain.DeliveryCompany;
using w2.Domain.DeliveryLeadTime;
using w2.Domain.ShopShipping;

public partial class Form_DeliveryCompany_DeliveryCompanyConfirm : DeliveryCompanyPage
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
			// コンポーネント初期化
			InitializeComponents();

			// 配送会社情報取得
			var deliveryCompanyInfo = GetInfoValue();

			// 画面表示
			DisplayDeliveryCompanyInfo(deliveryCompanyInfo);

			// Display Delivery Lead Times Info
			DisplayDeliveryLeadTimesInfo();

			trDeliveryCompanyTypeCreditcard.Visible = IsDeliveryCompanyTypeCreditcardByPaymentKbn();
			trDeliveryCompanyTypePostPayment.Visible = IsDeliveryCompanyTypePostPaymentByPaymentKbn();
			trDeliveryCompanyTypeNpPostPayment.Visible = Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED;
			trDeliveryCompanyTypeGooddeal.Visible = Constants.TWSHIPPING_GOODDEAL_OPTION_ENABLED;
			trDeliveryCompanyTypeGmoAtokara.Visible = Constants.PAYMENT_GMOATOKARA_ENABLED;
		}
	}

	/// <summary>
	/// Display delivery lead times info
	/// </summary>
	protected void DisplayDeliveryLeadTimesInfo()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_UPDATE:
			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_INSERT:
				rDeliveryLeadTimeZone.DataSource = Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO];
				if (((List<DeliveryLeadTimeInput>)Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO + "_special"]).Count != 0)
				{
					tbDeliveryLeadTimeZoneSpecial.Visible = trShippingLeadTimeDefault.Visible = true;
					rDeliveryLeadTimeZoneSpecial.DataSource = Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO + "_special"];
				}
				break;

			// Action status detail
			default:
				var deliveryLeadTimesInfo = new List<DeliveryLeadTimeInput>();
				Array.ForEach(this.DeliveryLeadTimeService.GetAll(this.LoginOperatorShopId, this.DeliveryCompanyId),
					item => deliveryLeadTimesInfo.Add(new DeliveryLeadTimeInput(item)));

				// Get delivery lead times special info
				var prefecturesCount = this.PrefecturesList.Length;
				var deliveryLeadTimesSpecialInfo = deliveryLeadTimesInfo
					.Where(item => (Int32.Parse(item.LeadTimeZoneNo) > prefecturesCount));
				var deliveryLeadTimesZoneInfo = deliveryLeadTimesInfo
					.Where(item => (Int32.Parse(item.LeadTimeZoneNo) <= prefecturesCount));
				if (deliveryLeadTimesSpecialInfo.Any())
				{
					tbDeliveryLeadTimeZoneSpecial.Visible = trShippingLeadTimeDefault.Visible = true;
					rDeliveryLeadTimeZoneSpecial.DataSource = deliveryLeadTimesSpecialInfo;
				}
				Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO + "_special"] = null;
				Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO] = null;
				rDeliveryLeadTimeZone.DataSource = deliveryLeadTimesZoneInfo;
				break;
		}

		rDeliveryLeadTimeZone.DataBind();
		rDeliveryLeadTimeZoneSpecial.DataBind();
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 新規登録、コピー新規登録
		if ((this.ActionStatus == Constants.ACTION_STATUS_INSERT) || (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
		{
			btnInsertTop.Visible = true;
			btnInsertBottom.Visible = true;
			trDetailTop.Visible = false;
			trConfirmTop.Visible = true;
		}
		// 更新
		else if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			btnUpdateTop.Visible = true;
			btnUpdateBottom.Visible = true;
			trDetailTop.Visible = false;
			trConfirmTop.Visible = true;
		}
		// 詳細
		else if (this.ActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			btnEditTop.Visible = true;
			btnEditBottom.Visible = true;
			btnCopyInsertTop.Visible = true;
			btnCopyInsertBottom.Visible = true;
			btnDeleteTop.Visible = true;
			btnDeleteBottom.Visible = true;
			trDateCreated.Visible = true;
			trDateChanged.Visible = true;
			trLastChanged.Visible = true;
			trDetailTop.Visible = true;
		}
	}

	/// <summary>
	/// 配送会社情報取得
	/// </summary>
	/// <returns>配送会社情報</returns>
	private DeliveryCompanyModel GetInfoValue()
	{
		DeliveryCompanyModel deliveryCompanyInfo = null;

		// 新規登録、コピー新規登録、更新
		if (((this.ActionStatus == Constants.ACTION_STATUS_INSERT) || (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT) || (this.ActionStatus == Constants.ACTION_STATUS_UPDATE))
			&& (Session[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO] != null))
		{
			deliveryCompanyInfo = ((DeliveryCompanyInput)Session[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO]).CreateModel();
		}
		// 詳細
		else if (this.ActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			deliveryCompanyInfo = this.DeliveryCompanyService.Get(this.DeliveryCompanyId);
		}

		if (deliveryCompanyInfo == null)
		{
			// エラーページ
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
		lDeliveryCompanyId.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.DeliveryCompanyId);
		lDeliveryCompanyName.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.DeliveryCompanyName);
		lDeliveryCompanyTypeCreditcard.Text = WebSanitizer.HtmlEncode(GetDeliveryCompanyTypeCreditcard(deliveryCompanyInfo));
		lDeliveryCompanyTypePostPayment.Text = WebSanitizer.HtmlEncode(GetDeliveryCompanyTypePostPayment(deliveryCompanyInfo));
		lDeliveryCompanyTypeNpPostPayment.Text = WebSanitizer.HtmlEncode(GetDeliveryCompanyTypePostPayment(deliveryCompanyInfo, true));
		if (Constants.TWSHIPPING_GOODDEAL_OPTION_ENABLED)
		{
			lDeliveryCompanyTypeGooddeal.Text = WebSanitizer.HtmlEncode(GetDeliveryCompanyTypeGooddeal(deliveryCompanyInfo));
		}
		lDeliveryCompanyTypeGmoAtokara.Text = WebSanitizer.HtmlEncode(GetDeliveryCompanyTypeGmoAtokaraPayment(deliveryCompanyInfo));
		lDisplayOrder.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.DisplayOrder);
		lDeliveryCompanyMailSizeLimit.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.DeliveryCompanyMailSizeLimit);
		lDeadlineTime.Text = WebSanitizer.HtmlEncode((string.IsNullOrEmpty(deliveryCompanyInfo.DeliveryCompanyDeadlineTime) == false) ? deliveryCompanyInfo.DeliveryCompanyDeadlineTime : "-");
		lShippingTimeSetFlg.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_DELIVERYCOMPANY, Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG, deliveryCompanyInfo.ShippingTimeSetFlg));
		lShippingTimeMessageID1.Text = WebSanitizer.HtmlEncode(CreateShippingTime(deliveryCompanyInfo.ShippingTimeId1, deliveryCompanyInfo.ShippingTimeMessage1));
		lShippingTimeMessageID2.Text = WebSanitizer.HtmlEncode(CreateShippingTime(deliveryCompanyInfo.ShippingTimeId2, deliveryCompanyInfo.ShippingTimeMessage2));
		lShippingTimeMessageID3.Text = WebSanitizer.HtmlEncode(CreateShippingTime(deliveryCompanyInfo.ShippingTimeId3, deliveryCompanyInfo.ShippingTimeMessage3));
		lShippingTimeMessageID4.Text = WebSanitizer.HtmlEncode(CreateShippingTime(deliveryCompanyInfo.ShippingTimeId4, deliveryCompanyInfo.ShippingTimeMessage4));
		lShippingTimeMessageID5.Text = WebSanitizer.HtmlEncode(CreateShippingTime(deliveryCompanyInfo.ShippingTimeId5, deliveryCompanyInfo.ShippingTimeMessage5));
		lShippingTimeMessageID6.Text = WebSanitizer.HtmlEncode(CreateShippingTime(deliveryCompanyInfo.ShippingTimeId6, deliveryCompanyInfo.ShippingTimeMessage6));
		lShippingTimeMessageID7.Text = WebSanitizer.HtmlEncode(CreateShippingTime(deliveryCompanyInfo.ShippingTimeId7, deliveryCompanyInfo.ShippingTimeMessage7));
		lShippingTimeMessageID8.Text = WebSanitizer.HtmlEncode(CreateShippingTime(deliveryCompanyInfo.ShippingTimeId8, deliveryCompanyInfo.ShippingTimeMessage8));
		lShippingTimeMessageID9.Text = WebSanitizer.HtmlEncode(CreateShippingTime(deliveryCompanyInfo.ShippingTimeId9, deliveryCompanyInfo.ShippingTimeMessage9));
		lShippingTimeMessageID10.Text = WebSanitizer.HtmlEncode(CreateShippingTime(deliveryCompanyInfo.ShippingTimeId10, deliveryCompanyInfo.ShippingTimeMessage10));
		lDeliveryLeadTimeZoneSetFlg.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_DELIVERYCOMPANY, Constants.FIELD_DELIVERYCOMPANY_DELIVERY_LEAD_TIME_SET_FLG, deliveryCompanyInfo.DeliveryLeadTimeSetFlg));
		lShippingLeadTimeDefault.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.ShippingLeadTimeDefault);
		lShippingTimeMessageMatching1.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.ShippingTimeMatching1);
		lShippingTimeMessageMatching2.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.ShippingTimeMatching2);
		lShippingTimeMessageMatching3.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.ShippingTimeMatching3);
		lShippingTimeMessageMatching4.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.ShippingTimeMatching4);
		lShippingTimeMessageMatching5.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.ShippingTimeMatching5);
		lShippingTimeMessageMatching6.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.ShippingTimeMatching6);
		lShippingTimeMessageMatching7.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.ShippingTimeMatching7);
		lShippingTimeMessageMatching8.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.ShippingTimeMatching8);
		lShippingTimeMessageMatching9.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.ShippingTimeMatching9);
		lShippingTimeMessageMatching10.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.ShippingTimeMatching10);

		tbShippingTime.Visible = deliveryCompanyInfo.IsValidShippingTimeSetFlg;
		tbDeliveryLeadTimeZone.Visible
			= trShippingLeadTimeDefault.Visible
			= deliveryCompanyInfo.IsValidDeliveryLeadTimeSetFlg;

		if (this.ActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			lDateCreated.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					deliveryCompanyInfo.DateCreated,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
			lDateChanged.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					deliveryCompanyInfo.DateChanged,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
			lLastChanged.Text = WebSanitizer.HtmlEncode(deliveryCompanyInfo.LastChanged);
		}
	}

	/// <summary>
	/// 出荷連携配送会社（クレジットカード）取得
	/// </summary>
	/// <param name="deliveryCompanyInfo">配送会社情報</param>
	/// <returns>出荷連携配送会社</returns>
	private string GetDeliveryCompanyTypeCreditcard(DeliveryCompanyModel deliveryCompanyInfo)
	{
		switch (Constants.PAYMENT_CARD_KBN)
		{
			case Constants.PaymentCard.YamatoKwc:
				return ValueText.GetValueText(Constants.TABLE_DELIVERYCOMPANY, Constants.DELOVERY_COMPANY_TYPE_YAMATO, deliveryCompanyInfo.DeliveryCompanyTypeCreditcard);
		}
		return ValueText.GetValueText(Constants.TABLE_DELIVERYCOMPANY, Constants.DELOVERY_COMPANY_TYPE, deliveryCompanyInfo.DeliveryCompanyTypeCreditcard);
	}

	/// <summary>
	/// 出荷連携配送会社（後払い）取得
	/// </summary>
	/// <param name="deliveryCompanyInfo">配送会社情報</param>
	/// <param name="isNPAfterPay">Is NP After Pay</param>
	/// <returns>出荷連携配送会社</returns>
	private string GetDeliveryCompanyTypePostPayment(DeliveryCompanyModel deliveryCompanyInfo, bool isNPAfterPay = false)
	{
		if (isNPAfterPay)
		{
			return ValueText.GetValueText(
				Constants.TABLE_DELIVERYCOMPANY,
				Constants.DELIVERY_COMPANY_TYPE_NP,
				deliveryCompanyInfo.DeliveryCompanyTypePostNpPayment);
		}

		switch (Constants.PAYMENT_CVS_DEF_KBN)
		{
			case Constants.PaymentCvsDef.YamatoKa:
				return ValueText.GetValueText(Constants.TABLE_DELIVERYCOMPANY, Constants.DELOVERY_COMPANY_TYPE_YAMATO, deliveryCompanyInfo.DeliveryCompanyTypePostPayment);

			case Constants.PaymentCvsDef.Gmo:
				return ValueText.GetValueText(Constants.TABLE_DELIVERYCOMPANY, Constants.DELOVERY_COMPANY_TYPE_GMO, deliveryCompanyInfo.DeliveryCompanyTypePostPayment);

			case Constants.PaymentCvsDef.Atodene:
				return ValueText.GetValueText(Constants.TABLE_DELIVERYCOMPANY, Constants.DELOVERY_COMPANY_TYPE_ATODENE, deliveryCompanyInfo.DeliveryCompanyTypePostPayment);

			case Constants.PaymentCvsDef.Dsk:
				return ValueText.GetValueText(Constants.TABLE_DELIVERYCOMPANY, Constants.DELOVERY_COMPANY_TYPE_DSK_DEFERRED, deliveryCompanyInfo.DeliveryCompanyTypePostPayment);

			case Constants.PaymentCvsDef.Atobaraicom:
				return ValueText.GetValueText(Constants.TABLE_DELIVERYCOMPANY, Constants.DELIVERY_COMPANY_TYPE_ATOBARAICOM, deliveryCompanyInfo.DeliveryCompanyTypePostPayment);

			case Constants.PaymentCvsDef.Score:
				return ValueText.GetValueText(Constants.TABLE_DELIVERYCOMPANY, Constants.DELIVERY_COMPANY_TYPE_SCORE_DEFERRED, deliveryCompanyInfo.DeliveryCompanyTypePostPayment);
		}
		return ValueText.GetValueText(Constants.TABLE_DELIVERYCOMPANY, Constants.DELOVERY_COMPANY_TYPE, deliveryCompanyInfo.DeliveryCompanyTypePostPayment);
	}

	/// <summary>
	/// 出荷連携配送会社（Gooddeal）取得
	/// </summary>
	/// <param name="deliveryCompanyInfo">配送会社情報</param>
	/// <returns>出荷連携配送会社</returns>
	private string GetDeliveryCompanyTypeGooddeal(DeliveryCompanyModel deliveryCompanyInfo)
	{
		return ValueText.GetValueText(Constants.TABLE_DELIVERYCOMPANY, Constants.DELIVERY_COMPANY_TYPE_GOODDEAL, deliveryCompanyInfo.DeliveryCompanyTypeGooddeal);
	}

	/// <summary>
	/// 出荷連携配送会社（GMOアトカラ）取得
	/// </summary>
	/// <param name="deliveryCompanyInfo">配送会社情報</param>
	/// <returns>出荷連携配送会社</returns>
	private string GetDeliveryCompanyTypeGmoAtokaraPayment(DeliveryCompanyModel deliveryCompanyInfo)
	{
		return ValueText.GetValueText(Constants.TABLE_DELIVERYCOMPANY,
			Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_GMO_ATOKARA_PAYMENT,
			deliveryCompanyInfo.DeliveryCompanyTypeGmoAtokaraPayment);
	}

	/// <summary>
	/// 配送時間帯取得
	/// </summary>
	/// <param name="shippingTimeId">配送時間帯ID</param>
	/// <param name="shippingTimeMessage">配送時間帯文言</param>
	/// <returns>配送時間帯</returns>
	protected string CreateShippingTime(string shippingTimeId, string shippingTimeMessage)
	{
		var result = "-";

		if ((string.IsNullOrEmpty(shippingTimeId) == false) && (string.IsNullOrEmpty(shippingTimeMessage) == false))
		{
			result = string.Format("{0}　(ID:{1})", shippingTimeMessage, shippingTimeId);
		}

		return result;
	}

	/// <summary>
	/// 「編集」ボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO] = null;

		// 編集画面へ遷移
		Response.Redirect(CreateDeliveryCompanyUrl(this.DeliveryCompanyId, Constants.PAGE_MANAGER_DELIVERY_COMPANY_REGISTER, Constants.ACTION_STATUS_UPDATE));
	}

	/// <summary>
	/// 「コピー新規登録」ボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO] = null;

		// 編集画面へ遷移
		Response.Redirect(CreateDeliveryCompanyUrl(this.DeliveryCompanyId, Constants.PAGE_MANAGER_DELIVERY_COMPANY_REGISTER, Constants.ACTION_STATUS_COPY_INSERT));
	}

	/// <summary>
	/// 「削除」ボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		var errorMessage = string.Empty;
		// 削除
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();

			try
			{
				var shopShippingCompanyModels = new ShopShippingService().GetShippingCompanyByDeliveryCompanyId(this.DeliveryCompanyId, accessor);

				if (shopShippingCompanyModels.Length == 0)
				{
					// Delete delivery company
					this.DeliveryCompanyService.Delete(this.DeliveryCompanyId, accessor);

					// Delete delivery lead time zone
					this.DeliveryLeadTimeService.Delete(this.LoginOperatorShopId, this.DeliveryCompanyId, accessor);
				}
				else
				{
					errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_DELIVERYCOMPANY_USE_SHOPSHIPPING_DELETE_ERROR)
						.Replace("@@ 1 @@", string.Join(",", shopShippingCompanyModels.Select(s => s.ShippingId).Distinct()));
				}

				// トランザクション確定
				accessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				// トランザクションロールバック
				accessor.RollbackTransaction();

				// スローする
				throw ex;
			}
		}

		if (string.IsNullOrEmpty(errorMessage))
		{
			// 一覧表示
			Response.Redirect(string.Format("{0}{1}", Constants.PATH_ROOT, Constants.PAGE_MANAGER_DELIVERY_COMPANY_LIST));
		}
		else
		{
			// エラーページ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(string.Format("{0}{1}", Constants.PATH_ROOT, Constants.PAGE_MANAGER_ERROR));
		}
	}

	/// <summary>
	/// 「新規登録」ボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		// Insert
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();

			try
			{
				// Insert delivery company
				var deliveryCompany = ((DeliveryCompanyInput)Session[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO]).CreateModel();
				deliveryCompany.LastChanged = this.LoginOperatorName;

				this.DeliveryCompanyService.Insert(deliveryCompany, accessor);

				// Insert delivery lead time zone
				var listDeliveryLeadTime = (List<DeliveryLeadTimeInput>)Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO];
				listDeliveryLeadTime.AddRange((List<DeliveryLeadTimeInput>)Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO + "_special"]);

				listDeliveryLeadTime.ForEach(item => this.DeliveryLeadTimeService.Insert(item.CreateModel(), accessor));

				// トランザクション確定
				accessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				// トランザクションロールバック
				accessor.RollbackTransaction();

				// スローする
				throw ex;
			}
		}

		// 一覧表示
		Response.Redirect(string.Format("{0}{1}", Constants.PATH_ROOT, Constants.PAGE_MANAGER_DELIVERY_COMPANY_LIST));
	}

	/// <summary>
	/// 「更新」ボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		// Update
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();

			try
			{
				// Update delivery company
				var deliveryCompany = ((DeliveryCompanyInput)Session[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO]).CreateModel();
				deliveryCompany.LastChanged = this.LoginOperatorName;

				this.DeliveryCompanyService.Update(deliveryCompany, accessor);

				// List delivery lead time input
				var listDeliveryLeadTime = (List<DeliveryLeadTimeInput>)Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO];
				listDeliveryLeadTime.AddRange((List<DeliveryLeadTimeInput>)Session[Constants.SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO + "_special"]);

				// List delivery lead time model
				var listDeliveryLeadTimeModel = new List<DeliveryLeadTimeModel>();
				listDeliveryLeadTime.ForEach(item => listDeliveryLeadTimeModel.Add(item.CreateModel()));

				// Update delivery lead time
				this.DeliveryLeadTimeService.UpdateDeliveryLeadTimeZone(listDeliveryLeadTimeModel, accessor);

				// トランザクション確定
				accessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				// トランザクションロールバック
				accessor.RollbackTransaction();

				// スローする
				throw ex;
			}
		}

		// 一覧表示
		Response.Redirect(string.Format("{0}{1}", Constants.PATH_ROOT, Constants.PAGE_MANAGER_DELIVERY_COMPANY_LIST));
	}
}
