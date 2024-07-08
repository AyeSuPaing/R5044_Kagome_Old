/*
=========================================================================================================
  Module      : 特別配送先情報登録ページ(ShippingZoneRegister.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using w2.Domain.DeliveryCompany;
using w2.Domain.ShopShipping;

public partial class Form_ShippingZone_ShippingZoneRegister : ShopShippingPage
{
	protected Hashtable m_htParam;

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
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			string strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, strActionStatus);

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(strActionStatus);

			//------------------------------------------------------
			// 処理区分チェック
			//------------------------------------------------------
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			//------------------------------------------------------
			// 表示用値設定処理
			//------------------------------------------------------
			// 新規？
			if (strActionStatus == Constants.ACTION_STATUS_INSERT)
			{
				// 配送料初期表示用
				m_htParam = new Hashtable
				{
					{ Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ID, ddlName.SelectedValue },
					{ DELIVERY_ZONE_PRICES, CreateDeliverySpecialZoneWithDefaultPrices() }
				};
			}
			// コピー新規・編集？
			else if (strActionStatus == Constants.ACTION_STATUS_COPY_INSERT ||
				strActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				// セッションより配送料地帯情報取得
				m_htParam = this.ShippingZoneInSession;
			}
			// それ以外の場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// ViewStateに配送料地帯情報を格納
			this.ShippingZoneInViewState = m_htParam;

			// データバインド
			DataBind();
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		// 新規登録？
		if (strActionStatus == Constants.ACTION_STATUS_INSERT)
		{
			trRegister.Visible = true;
		}
		else if (strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
		{
			trRegister.Visible = true;
		}
		else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			trEdit.Visible = true;
		}

		DataView dvShopShippings = GetShopShippingsAll(this.LoginOperatorShopId);
		// 該当データが有りの場合
		if (dvShopShippings.Count != 0)
		{
			// 配送料取得をドロップダウンリストに設定
			for (int iCount = 0; iCount < dvShopShippings.Count; iCount++)
			{
				ListItem liTemp = new ListItem();
				liTemp.Value = StringUtility.ToEmpty(
					dvShopShippings[iCount][Constants.FIELD_SHOPSHIPPING_SHIPPING_ID].ToString());
				liTemp.Text = StringUtility.ToEmpty(
					dvShopShippings[iCount][Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME].ToString());

				ddlName.Items.Add(liTemp);
			}
		}
		// 該当データ無しの場合
		else
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHIPPINGZONE_SHOP_SHIPPING_NO_DATA);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);

		}
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, System.EventArgs e)
	{
		// 変数宣言
		var validator = string.Empty;
		var zone = this.ShippingZoneInViewState;

		// パラメタ格納
		var inputParam = GetInputData();

		// 新規・コピー新規
		if (((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT)
			|| ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_COPY_INSERT))
		{
			validator = "ShopShippingZoneRegist";
		}
		// 変更
		else if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
		{
			validator = "ShopShippingZoneModify";

			// 旧配送料設定ID
			inputParam.Add(
				Constants.FIELD_SHOPSHIPPING_SHIPPING_ID + "_old",
				zone[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID].ToString());
			// 旧配送料地帯区分
			inputParam.Add(
				Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO + "_old",
				zone[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO].ToString());
		}

		// 入力チェック＆重複チェック
		var inputForCheck = (Hashtable)inputParam.Clone();
		if (IsCountryTw(Constants.OPERATIONAL_BASE_ISO_CODE))
		{
			inputForCheck[CONST_KEY_SHOPSHIPPINGZONE_ZIP_TW] = inputParam[Constants.FIELD_SHOPSHIPPINGZONE_ZIP];
			inputForCheck.Remove(Constants.FIELD_SHOPSHIPPINGZONE_ZIP);
			inputForCheck[CONST_KEY_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO_TW] = inputParam[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO];
			inputForCheck.Remove(Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO);
		}
		var errorMessages = Validator.Validate(validator, inputForCheck);
		if (errorMessages != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		var modelCount = 0;
		// 配送サービスごとの配送料地帯情報を補正
		foreach (var model in (List<ShopShippingZoneModel>)inputParam[DELIVERY_ZONE_PRICES])
		{
			model.ShopId = (string)inputParam[Constants.FIELD_SHOPSHIPPINGZONE_SHOP_ID];
			model.ShippingId = (string)inputParam[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ID];
			model.LastChanged = this.LoginOperatorName;
			model.ShippingZoneNo = int.Parse((string)inputParam[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO]);
			model.ShippingZoneName = (string)inputParam[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NAME];
			model.Zip = (string)inputParam[Constants.FIELD_SHOPSHIPPINGZONE_ZIP];

			// チェックボックスの状態を取得
			var check = (CheckBox)rDeliveryCompanyName.Items[modelCount].FindControl("cbUnavailableShippingDelivaryCompanyName");
			model.UnavailableShippingAreaFlg =
				check.Checked
					? Constants.FLG_SHOPSHIPPINGZONE_UNAVAILABLE_SHIPPING_AREA_VALID
					: Constants.FLG_SHOPSHIPPINGZONE_UNAVAILABLE_SHIPPING_AREA_INVALID;

			modelCount++;
		}

		// パラメタをセッションへ格納
		this.ShippingZoneInSession = inputParam;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];

		// 配送料地帯情報確認ページへ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_ZONE_CONFIRM +
			"?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS]);
	}

	/// <summary>
	/// デフォルート配送料での配送サービスごとの特別地帯情報の作成
	/// </summary>
	/// <returns>デフォルート配送料での配送サービスごとの特別地帯情報</returns>
	private List<ShopShippingZoneModel> CreateDeliverySpecialZoneWithDefaultPrices()
	{
		// 配送種別に紐づく配送サービス情報を取得
		var deliveries = new DeliveryCompanyService().GetByShippingId(ddlName.SelectedValue);
		var result =deliveries.Select(
			model => new ShopShippingZoneModel
			{
				DeliveryCompanyId = model.DeliveryCompanyId,
				SizeMailShippingPrice = decimal.Parse(Constants.DEFAULT_SHIPPING_PRICE),
				SizeXxsShippingPrice = decimal.Parse(Constants.DEFAULT_SHIPPING_PRICE),
				SizeXsShippingPrice = decimal.Parse(Constants.DEFAULT_SHIPPING_PRICE),
				SizeSShippingPrice = decimal.Parse(Constants.DEFAULT_SHIPPING_PRICE),
				SizeMShippingPrice = decimal.Parse(Constants.DEFAULT_SHIPPING_PRICE),
				SizeLShippingPrice = decimal.Parse(Constants.DEFAULT_SHIPPING_PRICE),
				SizeXlShippingPrice = decimal.Parse(Constants.DEFAULT_SHIPPING_PRICE),
				SizeXxlShippingPrice = decimal.Parse(Constants.DEFAULT_SHIPPING_PRICE)
			}).ToList();

		return result;
	}

	/// <summary>
	/// 配送種別の選択肢変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlName_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		m_htParam = GetInputData(false);
		DataBind();
	}

	/// <summary>
	/// 入力配送料地帯情報取得
	/// </summary>
	/// <param name="isReplaceZip">郵便番号を置換するか</param>
	/// <returns>入力配送料地帯情報</returns>
	private Hashtable GetInputData(bool isReplaceZip = true)
	{
		// ViewStateより配送料地帯情報取得
		var shippingZone = (Hashtable)ViewState[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO];

		var zipCode = isReplaceZip ? tbZip.Text.Replace("\r\n", "").Replace("\r", "").Replace("\n", "") : tbZip.Text;
		var inputShippingZone = new Hashtable
		{
			{ Constants.FIELD_SHOPSHIPPINGZONE_SHOP_ID, this.LoginOperatorShopId },
			{ Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ID, ddlName.SelectedValue },
			{ Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME, ddlName.SelectedItem.Text },
			{ Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO, tbShippingZoneNo.Text },
			{ Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NAME, tbName.Text },
			{ Constants.FIELD_SHOPSHIPPINGZONE_ZIP, zipCode },
			{ Constants.FIELD_SHOPSHIPPING_LAST_CHANGED, this.LoginOperatorName }
		};

		// 配送サービスごとの配送料地帯情報をセット
		inputShippingZone[DELIVERY_ZONE_PRICES] =
			(ddlName.SelectedValue == (string)shippingZone[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ID])
				? shippingZone[DELIVERY_ZONE_PRICES]
				: CreateDeliverySpecialZoneWithDefaultPrices();

		return inputShippingZone;
	}
}
