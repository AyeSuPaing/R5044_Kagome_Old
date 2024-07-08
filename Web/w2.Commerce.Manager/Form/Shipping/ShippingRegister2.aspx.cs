/*
=========================================================================================================
  Module      : 配送料情報登録ページ(ShippingRegister2.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using w2.Domain.ShopShipping;
using w2.App.Common.Extensions.Currency;

public partial class Form_Shipping_ShippingRegister : ShopShippingPage
{
	/// <summary>配送料情報データバインド用</summary>
	protected Hashtable m_param = new Hashtable();

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 処理区分チェック
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			// セッション情報がなければ、エラーページへ遷移
			if (this.ShippingInfoInSession == null)
			{
				RedirectToErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL));
			}

			// セッションから情報取得
			m_param = this.ShippingInfoInSession;

			// ViewStateに配送料、配送料地帯情報を格納
			this.ShippingInfoInViewState = m_param;

			// 配送料の別途見積り利用の場合、配送料を「0」にして、確認画面へ遷移
			// 日本への配送が不可の場合、配送料を「0」にして、確認画面へ遷移
			if (((string)m_param[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG]
				== Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID)
				|| (this.IsShippingCountryAvailableJp == false))
			{
				SetFreeShippingFee();
				this.ShippingInfoInSession = m_param;
				Response.Redirect(CreateShippingPageUrl(Constants.PAGE_MANAGER_SHIPPING_CONFIRM, this.ActionStatus));
			}

			// データバインド
			DataBind();

			// 画面制御
			InitializeComponents();

			// 画面制御2
			RefreshComponents();
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 登録・編集の文言表示
		trRegisterTop.Visible = (this.IsActionInsert || this.IsActionCopyInsert);
		trEditTop.Visible = this.IsActionUpdate;

		// 配送料設定のラジオボタンと全国一律送料項目の初期化
		foreach (RepeaterItem item in rShippingPostage.Items)
		{
			// 全国一律の配送料の初期化
			InititalizeShippingPriceAll(item);

			// 配送料設定のラジオボタン設定
			InitializeShippingPriceKbn(item);
		}
	}

	/// <summary>
	/// 全国一律の配送料の初期化
	/// </summary>
	/// <param name="item">リピーターアイテム</param>
	private void InititalizeShippingPriceAll(RepeaterItem item)
	{
		// コントロール宣言
		var tbSizeMailShippingPriceAll = (TextBox)item.FindControl("tbSizeMailShippingPriceAll");
		var tbSizeXxsShippingPriceAll = (TextBox)item.FindControl("tbSizeXxsShippingPriceAll");
		var tbSizeXsShippingPriceAll = (TextBox)item.FindControl("tbSizeXsShippingPriceAll");
		var tbSizeSShippingPriceAll = (TextBox)item.FindControl("tbSizeSShippingPriceAll");
		var tbSizeMShippingPriceAll = (TextBox)item.FindControl("tbSizeMShippingPriceAll");
		var tbSizeLShippingPriceAll = (TextBox)item.FindControl("tbSizeLShippingPriceAll");
		var tbSizeXlShippingPriceAll = (TextBox)item.FindControl("tbSizeXlShippingPriceAll");
		var tbSizeXxlShippingPriceAll = (TextBox)item.FindControl("tbSizeXxlShippingPriceAll");

		// 全国一律の配送料の初期化
		var deliveryCompanyId = ((HiddenField)item.FindControl("hfDeliveryCompanyId")).Value;
		var shippingZoneFirst = ((ShopShippingZoneModel[])m_param[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO])
			.First(model => ((model.DeliveryCompanyId == deliveryCompanyId) && (model.ShippingZoneNo == 1)));
		tbSizeMailShippingPriceAll.Text = shippingZoneFirst.SizeMailShippingPrice.ToPriceString();
		tbSizeXxsShippingPriceAll.Text = shippingZoneFirst.SizeXxsShippingPrice.ToPriceString();
		tbSizeXsShippingPriceAll.Text = shippingZoneFirst.SizeXsShippingPrice.ToPriceString();
		tbSizeSShippingPriceAll.Text = shippingZoneFirst.SizeSShippingPrice.ToPriceString();
		tbSizeMShippingPriceAll.Text = shippingZoneFirst.SizeMShippingPrice.ToPriceString();
		tbSizeLShippingPriceAll.Text = shippingZoneFirst.SizeLShippingPrice.ToPriceString();
		tbSizeXlShippingPriceAll.Text = shippingZoneFirst.SizeXlShippingPrice.ToPriceString();
		tbSizeXxlShippingPriceAll.Text = shippingZoneFirst.SizeXxlShippingPrice.ToPriceString();
	}

	/// <summary>
	/// 配送料設定のラジオボタン初期化
	/// </summary>
	/// <param name="item">リピーターアイテム</param>
	private void InitializeShippingPriceKbn(RepeaterItem item)
	{
		// コントロール宣言
		var rbShippingKbn0 = (RadioButton)item.FindControl("rbShippingKbn0");
		var rbShippingKbn1 = (RadioButton)item.FindControl("rbShippingKbn1");
		var rbShippingKbn2 = (RadioButton)item.FindControl("rbShippingKbn2");

		// 配送料設定のラジオボタン設定
		var deliveryCompanyId = ((HiddenField)item.FindControl("hfDeliveryCompanyId")).Value;
		var shippingPriceKbn =
			((ShippingDeliveryPostageModel[])m_param[Constants.SESSIONPARAM_KEY_SHIPPINGDELIVERYPOSTAGE_INFO])
				.First(model => (model.DeliveryCompanyId == deliveryCompanyId))
				.ShippingPriceKbn;
		switch (shippingPriceKbn)
		{
			case Constants.FLG_SHIPPING_PRICE_KBN_NONE:
				rbShippingKbn0.Checked = true;
				break;

			case Constants.FLG_SHIPPING_PRICE_KBN_SAME:
				rbShippingKbn1.Checked = true;
				break;

			case Constants.FLG_SHIPPING_PRICE_KBN_AREA:
				rbShippingKbn2.Checked = true;
				break;
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化2
	/// </summary>
	private void RefreshComponents()
	{
		// 配送料金情報
		foreach (RepeaterItem item in rShippingPostage.Items)
		{
			// コントロール宣言
			var rbShippingKbn0 = (RadioButton)item.FindControl("rbShippingKbn0");
			var rbShippingKbn1 = (RadioButton)item.FindControl("rbShippingKbn1");
			var rbShippingKbn2 = (RadioButton)item.FindControl("rbShippingKbn2");
			var dvShippingFreePrice = (HtmlGenericControl)item.FindControl("dvShippingFreePrice");
			var dvCalculationPlural = (HtmlGenericControl)item.FindControl("dvCalculationPlural");
			var dvShippingZone = (HtmlGenericControl)item.FindControl("dvShippingZone");
			var cbShippingFreePriceFlg = (CheckBox)item.FindControl("cbShippingFreePriceFlg");
			var cbCalculationPluralKbn = (CheckBox)item.FindControl("cbCalculationPluralKbn");
			var trShippingFreePrice = (HtmlTableRow)item.FindControl("trShippingFreePrice");
			var trAnnounceFreeShippingFlg = (HtmlTableRow)item.FindControl("trAnnounceFreeShippingFlg");
			var trCalculationPluralKbn = (HtmlTableRow)item.FindControl("trCalculationPluralKbn");
			var trZoneAll = (HtmlTableRow)item.FindControl("trZoneAll");
			var rShippingZone = (Repeater)item.FindControl("rShippingZone");
			var deliveryCompanyId = ((HiddenField)item.FindControl("hfDeliveryCompanyId")).Value;
			var dvStorePickupFreePriceFlg = (HtmlGenericControl)item.FindControl("dvStorePickupFreePriceFlg");

			// 配送料無しの場合
			if (rbShippingKbn0.Checked)
			{
				dvShippingFreePrice.Visible = false;
				dvCalculationPlural.Visible = false;
				dvShippingZone.Visible = false;
				dvStorePickupFreePriceFlg.Visible = false;
			}
			// 配送料が全国一律の場合
			else if (rbShippingKbn1.Checked)
			{
				dvShippingFreePrice.Visible = true;
				dvCalculationPlural.Visible = true;
				dvShippingZone.Visible = true;
				trZoneAll.Visible = true;


				for (var loop = 0; loop < this.PrefecturesList.Length; loop++)
				{
					((HtmlTableRow)rShippingZone.Items[loop].FindControl("trZone")).Visible = false;
				}

				dvStorePickupFreePriceFlg.Visible = true;
			}
			// 配送料を地域別に設定する場合
			else if (rbShippingKbn2.Checked)
			{
				dvShippingFreePrice.Visible = true;
				dvCalculationPlural.Visible = true;
				dvShippingZone.Visible = true;
				trZoneAll.Visible = false;

				for (var loop = 0; loop < this.PrefecturesList.Length; loop++)
				{
					((HtmlTableRow)rShippingZone.Items[loop].FindControl("trZone")).Visible = true;
				}

				dvStorePickupFreePriceFlg.Visible = true;
			}

			// 配送料無料購入金額情報
			trShippingFreePrice.Visible = cbShippingFreePriceFlg.Checked;
			trAnnounceFreeShippingFlg.Visible = cbShippingFreePriceFlg.Checked;

			// 複数商品計算方法情報
			trCalculationPluralKbn.Visible = cbCalculationPluralKbn.Checked;
		}
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		// ViewStateより配送料情報取得
		var shippingInfo = this.ShippingInfoInViewState;
		var shippingId = (string)shippingInfo[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID];

		// 配送料金情報
		var shippingPostages = new List<ShippingDeliveryPostageInput>();
		var shippingZones = new List<ShopShippingZoneInput>();
		foreach (RepeaterItem item in rShippingPostage.Items)
		{
			// 配送料マスタ取得
			shippingPostages.Add(GetShippingDeliveryPostageInput(item, shippingId));

			// 元の配送料地帯情報取得
			var deliveryCompanyId = ((HiddenField)item.FindControl("hfDeliveryCompanyId")).Value;
			var shippingDeliveryZones = GetShippingZonesByDeliveryCompany(
				(ShopShippingZoneModel[])shippingInfo[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO],
				deliveryCompanyId).ToList();
			// 配送料地帯入力値を取得
			shippingZones.AddRange(GetZonePricesInput(item, shippingDeliveryZones));
		}

		// 配送料マスタ入力チェック
		this.ErrorMessagesHtmlEncoded = new List<KeyValuePair<string, string>>();
		foreach (var postageInput in shippingPostages)
		{
			var postageError = postageInput.Validate();
			if (string.IsNullOrEmpty(postageError) == false)
			{
				this.ErrorMessagesHtmlEncoded.Add(
					new KeyValuePair<string, string>(
						postageInput.DeliveryCompanyId,
						postageError.Replace(
							"@@ 1 @@",
							WebSanitizer.HtmlEncode(GetDeliveryCompanyName(postageInput.DeliveryCompanyId)))));
			}
		}

		// サイズ配送料入力チェック
		foreach (var input in shippingZones)
		{
			var zoneError = input.Validate();
			if (string.IsNullOrEmpty(zoneError) == false)
			{
				this.ErrorMessagesHtmlEncoded.Add(
					new KeyValuePair<string, string>(
						input.DeliveryCompanyId,
						zoneError.Replace(
							"@@ 1 @@",
							WebSanitizer.HtmlEncode(
								string.Format(
									"{0}：{1}",
									GetDeliveryCompanyName(input.DeliveryCompanyId),
									input.ShippingZoneName)))));
			}
		}

		// 入力エラーなければ配送料情報確認ページへ遷移
		if (this.ErrorMessagesHtmlEncoded.Count == 0)
		{
			// 配送料マスタ格納
			shippingInfo[Constants.SESSIONPARAM_KEY_SHIPPINGDELIVERYPOSTAGE_INFO] =
				shippingPostages.Select(input => input.CreateModel()).ToArray();

			// 配送料地帯区分数格納
			shippingInfo[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO] =
				shippingZones.Select(input => input.CreateModel()).ToArray();

			// パラメタをセッションへ格納
			this.ShippingInfoInSession = shippingInfo;

			// 処理区分をセッションへ格納
			Session[Constants.SESSION_KEY_ACTION_STATUS] = this.ActionStatus;

			// 配送料情報確認ページへ遷移
			Response.Redirect(CreateShippingPageUrl(Constants.PAGE_MANAGER_SHIPPING_CONFIRM, this.ActionStatus));
		}

		// エラーメッセージ表示
		DisplayErrorMessages();
	}

	/// <summary>
	/// エラーメッセージ表示
	/// </summary>
	protected void DisplayErrorMessages()
	{
		var serviceErrorMessagesHtmlEncoded = new List<string>();

		foreach (RepeaterItem item in rShippingPostage.Items)
		{
			// コントロール宣言
			var deliveryCompanyId = ((HiddenField)item.FindControl("hfDeliveryCompanyId")).Value;
			var trShippingPostageErrorMessagesTitle = (HtmlTableRow)item.FindControl("trShippingPostageErrorMessagesTitle");
			var trShippingPostageErrorMessages = (HtmlTableRow)item.FindControl("trShippingPostageErrorMessages");
			var lbShippingPostageErrorMessages = (Label)item.FindControl("lbShippingPostageErrorMessages");

			trShippingPostageErrorMessagesTitle.Visible = false;
			trShippingPostageErrorMessages.Visible = false;

			if (string.IsNullOrEmpty(GetErrorMsgByDeliveryCompany(this.ErrorMessagesHtmlEncoded, deliveryCompanyId)) == false)
			{
				trShippingPostageErrorMessagesTitle.Visible = true;
				trShippingPostageErrorMessages.Visible = true;
				lbShippingPostageErrorMessages.Text = GetErrorMsgByDeliveryCompany(this.ErrorMessagesHtmlEncoded, deliveryCompanyId);
				serviceErrorMessagesHtmlEncoded.Add(WebSanitizer.HtmlEncode(GetDeliveryCompanyName(deliveryCompanyId)));
			}
		}

		tblDeliveryCompanyErrorMessages.Visible = (serviceErrorMessagesHtmlEncoded.Count != 0);
		lbDeliveryCompanyErrorMessages.Text = string.Join("<br />", serviceErrorMessagesHtmlEncoded);
	}

	/// <summary>
	/// 配送料マスタ入力値取得
	/// </summary>
	/// <param name="item">リピーターアイテム</param>
	/// <param name="shippingId">配送種別ID</param>
	/// <returns>配送料マスタ</returns>
	private ShippingDeliveryPostageInput GetShippingDeliveryPostageInput(RepeaterItem item, string shippingId)
	{
		// コントロール宣言
		var tbShippingFreePrice = (TextBox)item.FindControl("tbShippingFreePrice");
		var tbPluralShippingPrice = (TextBox)item.FindControl("tbPluralShippingPrice");
		var rbShippingKbn0 = (RadioButton)item.FindControl("rbShippingKbn0");
		var rbShippingKbn1 = (RadioButton)item.FindControl("rbShippingKbn1");
		var cbShippingFreePriceFlg = (CheckBox)item.FindControl("cbShippingFreePriceFlg");
		var cbCalculationPluralKbn = (CheckBox)item.FindControl("cbCalculationPluralKbn");
		var cbAnnounceShippingFreeFlg = (CheckBox)item.FindControl("cbAnnounceShippingFreeFlg");
		var cbStorePickupFreePriceFlg = (CheckBox)item.FindControl("cbStorePickupFreePriceFlg");
		var tbUseFreeShippingFee = (TextBox)item.FindControl("tbUseFreeShippingFee");

		// 配送会社ID取得
		var deliveryCompanyId = ((HiddenField)item.FindControl("hfDeliveryCompanyId")).Value;

		var StorePickupFreePriceFlgTemp = ((ShippingDeliveryPostageModel[])
			this.ShippingInfoInSession["shippingdeliverypostage_info"])[0].StorePickupFreePriceFlg;

		// 配送料マスタ取得
		var result = new ShippingDeliveryPostageInput
		{
			ShopId = this.LoginOperatorShopId,
			ShippingId = shippingId,
			DeliveryCompanyId = deliveryCompanyId,
			ShippingPriceKbn = rbShippingKbn0.Checked
				? Constants.FLG_SHIPPING_PRICE_KBN_NONE
				: rbShippingKbn1.Checked
					? Constants.FLG_SHIPPING_PRICE_KBN_SAME
					: Constants.FLG_SHIPPING_PRICE_KBN_AREA,
			ShippingFreePriceFlg = ((rbShippingKbn0.Checked == false) && cbShippingFreePriceFlg.Checked)
					? Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_VALID
					: Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_INVALID,
			ShippingFreePrice = ((rbShippingKbn0.Checked == false)
				&& cbShippingFreePriceFlg.Checked
				&& (string.IsNullOrEmpty(tbShippingFreePrice.Text.Trim()) == false))
					? tbShippingFreePrice.Text.Trim()
					: "0",
			AnnounceFreeShippingFlg = ((rbShippingKbn0.Checked == false)
				&& cbShippingFreePriceFlg.Checked
				&& cbAnnounceShippingFreeFlg.Checked)
					? Constants.FLG_SHOPSHIPPING_ANNOUNCE_FREE_SHIPPING_FLG_VALID
					: Constants.FLG_SHOPSHIPPING_ANNOUNCE_FREE_SHIPPING_FLG_INVALID,
			CalculationPluralKbn = ((rbShippingKbn0.Checked == false) && cbCalculationPluralKbn.Checked)
					? Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_HIGHEST_SHIPPING_PRICE_PLUS_PLURAL_PRICE
					: Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_SUM_OF_PRODUCT_SHIPPING_PRICE,
			PluralShippingPrice = ((rbShippingKbn0.Checked == false)
				&& cbCalculationPluralKbn.Checked
				&& (string.IsNullOrEmpty(tbPluralShippingPrice.Text.Trim()) == false))
					? tbPluralShippingPrice.Text.Trim()
					: "0",
			LastChanged = this.LoginOperatorName,
			StorePickupFreePriceFlg = Constants.STORE_PICKUP_OPTION_ENABLED
				? (cbStorePickupFreePriceFlg.Checked)
					? Constants.FLG_SHOPSHIPPING_ANNOUNCE_FREE_SHIPPING_FLG_VALID
					: Constants.FLG_SHOPSHIPPING_ANNOUNCE_FREE_SHIPPING_FLG_INVALID
				: StorePickupFreePriceFlgTemp,
			FreeShippingFee = ((rbShippingKbn0.Checked == false)
				&& (string.IsNullOrEmpty(tbUseFreeShippingFee.Text.Trim()) == false))
					? tbUseFreeShippingFee.Text.Trim()
					: "0",
		};

		return result;
	}

	/// <summary>
	/// 配送料地帯入力値取得
	/// </summary>
	/// <param name="item">リピーターアイテム</param>
	/// <param name="shippingDeliveryZones">配送料地帯情報（元の情報）</param>
	/// <returns>配送料地帯入力値</returns>
	private List<ShopShippingZoneInput> GetZonePricesInput(
		RepeaterItem item,
		IEnumerable<ShopShippingZoneModel> shippingDeliveryZones)
	{
		// コントロール宣言
		var rbShippingKbn0 = (RadioButton)item.FindControl("rbShippingKbn0");
		var rbShippingKbn1 = (RadioButton)item.FindControl("rbShippingKbn1");
		var rbShippingKbn2 = (RadioButton)item.FindControl("rbShippingKbn2");
		var deliveryCompanyId = ((HiddenField)item.FindControl("hfDeliveryCompanyId")).Value;
		var zoneList = new List<ShopShippingZoneInput>();
		if (rbShippingKbn0.Checked)
		{
			// 配送料なしの場合
			zoneList.AddRange(shippingDeliveryZones
				.Select(
					model =>
					{
						var input = new ShopShippingZoneInput(model)
						{
							SizeMailShippingPrice = "0",
							SizeXxsShippingPrice = "0",
							SizeXsShippingPrice = "0",
							SizeSShippingPrice = "0",
							SizeMShippingPrice = "0",
							SizeLShippingPrice = "0",
							SizeXlShippingPrice = "0",
							SizeXxlShippingPrice = "0",
							ConditionalShippingPriceThreshold = null,
							ConditionalShippingPrice = null,
						};
						return input;
					})
				.ToList());
		}
		else if (rbShippingKbn1.Checked)
		{
			// 全国一律設定の場合
			zoneList.AddRange(GetZonePriceInputWithSamePrice(item, shippingDeliveryZones));
		}
		else if (rbShippingKbn2.Checked)
		{
			// 地域毎に設定の場合
			zoneList.AddRange(GetZonePriceInputWithDifferentPrice(item, shippingDeliveryZones));
		}

		return zoneList;
	}

	/// <summary>
	/// 全国一律設定時の配送料地帯入力値取得
	/// </summary>
	/// <param name="item">リピーターアイテム</param>
	/// <param name="shippingDeliveryZones">配送料地帯情報（元の情報）</param>
	/// <returns>配送料地帯情報</returns>
	private IEnumerable<ShopShippingZoneInput> GetZonePriceInputWithSamePrice(
		RepeaterItem item,
		IEnumerable<ShopShippingZoneModel> shippingDeliveryZones)
	{
		// コントロール宣言
		var tbSizeMailShippingPriceAll = (TextBox)item.FindControl("tbSizeMailShippingPriceAll");
		var tbSizeXxsShippingPriceAll = (TextBox)item.FindControl("tbSizeXxsShippingPriceAll");
		var tbSizeXsShippingPriceAll = (TextBox)item.FindControl("tbSizeXsShippingPriceAll");
		var tbSizeSShippingPriceAll = (TextBox)item.FindControl("tbSizeSShippingPriceAll");
		var tbSizeMShippingPriceAll = (TextBox)item.FindControl("tbSizeMShippingPriceAll");
		var tbSizeLShippingPriceAll = (TextBox)item.FindControl("tbSizeLShippingPriceAll");
		var tbSizeXlShippingPriceAll = (TextBox)item.FindControl("tbSizeXlShippingPriceAll");
		var tbSizeXxlShippingPriceAll = (TextBox)item.FindControl("tbSizeXxlShippingPriceAll");
		var rShippingZone = (Repeater)item.FindControl("rShippingZone");

		var result = new List<ShopShippingZoneInput>();
		var idx = 0;
		foreach (var zoneModel in shippingDeliveryZones)
		{
			var input = new ShopShippingZoneInput(zoneModel);

			// 都道府県設定
			if (idx < this.PrefecturesList.Length)
			{
				input.SizeMailShippingPrice = tbSizeMailShippingPriceAll.Text.Trim();
				input.SizeXxsShippingPrice = tbSizeXxsShippingPriceAll.Text.Trim();
				input.SizeXsShippingPrice = tbSizeXsShippingPriceAll.Text.Trim();
				input.SizeSShippingPrice = tbSizeSShippingPriceAll.Text.Trim();
				input.SizeMShippingPrice = tbSizeMShippingPriceAll.Text.Trim();
				input.SizeLShippingPrice = tbSizeLShippingPriceAll.Text.Trim();
				input.SizeXlShippingPrice = tbSizeXlShippingPriceAll.Text.Trim();
				input.SizeXxlShippingPrice = tbSizeXxlShippingPriceAll.Text.Trim();
				// 全国一律部分の配送料条件は常に非活性なので、null設定
				input.ConditionalShippingPriceThreshold = null;
				input.ConditionalShippingPrice = null;
			}
			// 特別配送先
			else
			{
				input.SizeMailShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeMailShippingPrice"))).Text.Trim();
				input.SizeXxsShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeXxsShippingPrice"))).Text.Trim();
				input.SizeXsShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeXsShippingPrice"))).Text.Trim();
				input.SizeSShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeSShippingPrice"))).Text.Trim();
				input.SizeMShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeMShippingPrice"))).Text.Trim();
				input.SizeLShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeLShippingPrice"))).Text.Trim();
				input.SizeXlShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeXlShippingPrice"))).Text.Trim();
				input.SizeXxlShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeXxlShippingPrice"))).Text.Trim();
				input.ConditionalShippingPriceThreshold =
					((CheckBox)(rShippingZone.Items[idx].FindControl("chkConditionalShippingPriceFlg"))).Checked
						? ((TextBox)(rShippingZone.Items[idx].FindControl("tbConditionalShippingPriceThreshold"))).Text.Trim()
						: null;
				input.ConditionalShippingPrice =
					((CheckBox)(rShippingZone.Items[idx].FindControl("chkConditionalShippingPriceFlg"))).Checked
						? ((TextBox)(rShippingZone.Items[idx].FindControl("tbConditionalShippingPrice"))).Text.Trim()
						: null;
			}
			result.Add(input);
			idx++;
		}

		return result;
	}

	/// <summary>
	/// 地域毎に設定時の配送料地帯入力値取得
	/// </summary>
	/// <param name="item">リピーターアイテム</param>
	/// <param name="shippingDeliveryZones">配送料地帯情報（元の情報）</param>
	/// <returns>配送料地帯情報</returns>
	private IEnumerable<ShopShippingZoneInput> GetZonePriceInputWithDifferentPrice(
		RepeaterItem item,
		IEnumerable<ShopShippingZoneModel> shippingDeliveryZones)
	{
		// コントロール宣言
		var rShippingZone = (Repeater)item.FindControl("rShippingZone");

		var result = new List<ShopShippingZoneInput>();
		var idx = 0;
		foreach (var zoneModel in shippingDeliveryZones)
		{
			var input = new ShopShippingZoneInput(zoneModel)
			{
				SizeMailShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeMailShippingPrice"))).Text.Trim(),
				SizeXxsShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeXxsShippingPrice"))).Text.Trim(),
				SizeXsShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeXsShippingPrice"))).Text.Trim(),
				SizeSShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeSShippingPrice"))).Text.Trim(),
				SizeMShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeMShippingPrice"))).Text.Trim(),
				SizeLShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeLShippingPrice"))).Text.Trim(),
				SizeXlShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeXlShippingPrice"))).Text.Trim(),
				SizeXxlShippingPrice = ((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeXxlShippingPrice"))).Text.Trim(),
				ConditionalShippingPriceThreshold =
					((CheckBox)(rShippingZone.Items[idx].FindControl("chkConditionalShippingPriceFlg"))).Checked
						? ((TextBox)(rShippingZone.Items[idx].FindControl("tbConditionalShippingPriceThreshold"))).Text.Trim()
						: null,
				ConditionalShippingPrice =
					((CheckBox)(rShippingZone.Items[idx].FindControl("chkConditionalShippingPriceFlg"))).Checked
						? ((TextBox)(rShippingZone.Items[idx].FindControl("tbConditionalShippingPrice"))).Text.Trim()
						: null,
			};
			result.Add(input);
			idx++;
		}

		return result;
	}

	/// <summary>
	/// 「配送料なしの場合」ラジオボタンクリック
	/// 「配送料が全国一律の場合」クリック
	/// 「配送料を地域別に設定する場合」クリック
	/// 「無料購入金額設定の利用の有無」クリック
	/// 「複数商品計算方法の利用の有無」クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void RefreshComponents_OnCheckedChanged(object sender, EventArgs e)
	{
		RefreshComponents();
	}

	/// <summary>
	/// 配送料無料セット
	/// </summary>
	private void SetFreeShippingFee()
	{
		// 配送料マスタ設定
		foreach (var shippingPostage in (ShippingDeliveryPostageModel[])m_param[Constants.SESSIONPARAM_KEY_SHIPPINGDELIVERYPOSTAGE_INFO])
		{
			shippingPostage.ShippingPriceKbn = Constants.FLG_SHIPPING_PRICE_KBN_NONE;
			shippingPostage.ShippingFreePriceFlg = Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_INVALID;
			shippingPostage.AnnounceFreeShippingFlg = Constants.FLG_SHOPSHIPPING_ANNOUNCE_FREE_SHIPPING_FLG_INVALID;
			shippingPostage.ShippingFreePrice = 0;
			shippingPostage.CalculationPluralKbn = Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_SUM_OF_PRODUCT_SHIPPING_PRICE;
			shippingPostage.PluralShippingPrice = 0;
		}

		// すべて地帯による配送料を「0」で設定
		foreach (var shippingZone in (ShopShippingZoneModel[])m_param[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO])
		{
			shippingZone.SizeMailShippingPrice = 0;
			shippingZone.SizeXxsShippingPrice = 0;
			shippingZone.SizeXsShippingPrice = 0;
			shippingZone.SizeSShippingPrice = 0;
			shippingZone.SizeMShippingPrice = 0;
			shippingZone.SizeLShippingPrice = 0;
			shippingZone.SizeXlShippingPrice = 0;
			shippingZone.SizeXxlShippingPrice = 0;
			shippingZone.ConditionalShippingPriceThreshold = null;
			shippingZone.ConditionalShippingPrice = null;
		}
	}

	/// <summary>
	/// 配送料マスタリピーターイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rShippingPostage_ItemCommand(object sender, RepeaterCommandEventArgs e)
	{
		if (e.CommandName != "ChangePrice") return;

		var rbShippingKbn1 = (RadioButton)e.Item.FindControl("rbShippingKbn1");
		var rbShippingKbn2 = (RadioButton)e.Item.FindControl("rbShippingKbn2");
		var value = ((TextBox)e.Item.FindControl("tbAllShippingPrice")).Text;

		// 入力チェック(半角数値)
		int checkValue;
		if ((int.TryParse(value, out checkValue) == false) || (checkValue < 0))
		{
			ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", "alertMessage();", true);
			return;
		}

		// 配送料全国一律更新
		if (rbShippingKbn1.Checked)
		{
			((TextBox)e.Item.FindControl("tbSizeMailShippingPriceAll")).Text = value;
			((TextBox)e.Item.FindControl("tbSizeXxsShippingPriceAll")).Text = value;
			((TextBox)e.Item.FindControl("tbSizeXsShippingPriceAll")).Text = value;
			((TextBox)e.Item.FindControl("tbSizeSShippingPriceAll")).Text = value;
			((TextBox)e.Item.FindControl("tbSizeMShippingPriceAll")).Text = value;
			((TextBox)e.Item.FindControl("tbSizeLShippingPriceAll")).Text = value;
			((TextBox)e.Item.FindControl("tbSizeXlShippingPriceAll")).Text = value;
			((TextBox)e.Item.FindControl("tbSizeXxlShippingPriceAll")).Text = value;
		}

		// 各都道府県の配送料更新（特別配送先を除く）
		if (rbShippingKbn2.Checked)
		{
			var rShippingZone = (Repeater)e.Item.FindControl("rShippingZone");
			for (var idx = 0; idx < this.PrefecturesList.Length; idx++)
			{
				((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeMailShippingPrice"))).Text = value;
				((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeXxsShippingPrice"))).Text = value;
				((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeXsShippingPrice"))).Text = value;
				((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeSShippingPrice"))).Text = value;
				((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeMShippingPrice"))).Text = value;
				((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeLShippingPrice"))).Text = value;
				((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeXlShippingPrice"))).Text = value;
				((TextBox)(rShippingZone.Items[idx].FindControl("tbSizeXxlShippingPrice"))).Text = value;
			}
		}
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		var url = CreateShippingPageUrl(Constants.PAGE_MANAGER_SHIPPING_REGISTER, this.ActionStatus);
		this.Response.Redirect(url);
	}

	/// <summary>指定した配送サービス件数</summary>
	protected int SelectedDeliveryCompanyCount
	{
		get { return ((ShippingDeliveryPostageModel[])m_param[Constants.SESSIONPARAM_KEY_SHIPPINGDELIVERYPOSTAGE_INFO]).Length; }
	}
	/// <summary>エラーメッセージ</summary>
	protected List<KeyValuePair<string, string>> ErrorMessagesHtmlEncoded { get; set; }

	/// <summary>
	/// 配送料条件追加チェックボックスチェック状態変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void chkConditionalShippingPriceFlgOnCheckedChanged(object sender, EventArgs e)
	{
		var chkConditionalShippingPriceFlg = (CheckBox)sender;
		var item = (RepeaterItem)chkConditionalShippingPriceFlg.NamingContainer;
		var plConditionalShippingPrice = item.FindControl("plConditionalShippingPrice");
		plConditionalShippingPrice.Visible = chkConditionalShippingPriceFlg.Checked;
	}

	/// <summary>
	/// 各エリア配送料情報RepeaterのOnItemDataBoundイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rShippingZoneOnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
		{
			var chkConditionalShippingPriceFlg = (CheckBox)e.Item.FindControl("chkConditionalShippingPriceFlg");
			var plConditionalShippingPrice = e.Item.FindControl("plConditionalShippingPrice");
			chkConditionalShippingPriceFlg.Checked = (((ShopShippingZoneModel)e.Item.DataItem).ConditionalShippingPriceThreshold != null);
			plConditionalShippingPrice.Visible = chkConditionalShippingPriceFlg.Checked;
		}
	}
}
