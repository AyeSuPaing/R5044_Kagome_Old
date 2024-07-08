/*
=========================================================================================================
  Module      : 会員ランク設定登録ページ処理(MemberRankRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Text;
using w2.App.Common.Extensions.Currency;
using w2.Common.Util;
using w2.Domain.MemberRank;

public partial class Form_MemberRank_MemberRankRegister : BasePage
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
			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			// ランクID
			string strRankId = Request[Constants.REQUEST_KEY_MEMBERRANK_ID];
			ViewState.Add(Constants.REQUEST_KEY_MEMBERRANK_ID, strRankId);
			// アクションステータス
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
			// 新規登録
			if (strActionStatus == Constants.ACTION_STATUS_INSERT)
			{
				// 初期値指定
				rbOrderDiscountTypeNone.Checked = true;			// 注文割引指定：割引しない
				rbPointAddTypeNone.Checked = true;				// ポイント加算指定：加算しない
				rbShippingDiscountTypeNone.Checked = true;		// 配送料割引指定：割引しない
			}
			// 編集
			else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				// 会員ランク情報取得
				var memberRank = MemberRankService.Get(strRankId);

				// 画面セット
				if (memberRank != null)
				{
					lRankId.Text = memberRank.MemberRankId;						// ランクID
					tbRankOrder.Text = StringUtility.ToEmpty(memberRank.MemberRankOrder);	// ランク順位
					tbRankName.Text = (string)memberRank.MemberRankName;					// ランク名
					// 注文割引方法指定
					string strOrderDiscountValue = StringUtility.ToEmpty(memberRank.OrderDiscountValue);
					switch ((string)memberRank.OrderDiscountType)
					{
						case Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_NONE:			// 割引きしない
							rbOrderDiscountTypeNone.Checked = true;
							break;

						case Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_RATE:			// 割引率指定
							rbOrderDiscountTypeRate.Checked = true;
							tbOrderDiscountRateValue.Text = DecimalUtility.DecimalRound(strOrderDiscountValue.ToPriceDecimal().Value, DecimalUtility.Format.RoundDown, 0).ToString();
							break;

						case Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_FIXED:		// 割引金額指定
							rbOrderDiscountTypeFixed.Checked = true;
							tbOrderDiscountFixedValue.Text = strOrderDiscountValue.ToPriceString();
							break;

						default:
							break;
					}
					// 注文金額割引き閾値
					tbOrderDiscountThresholdPrice.Text = StringUtility.ToEmpty(memberRank.OrderDiscountThresholdPrice).ToPriceString();
					// ポイント加算方法指定
					switch ((string)memberRank.PointAddType)
					{
						case Constants.FLG_MEMBERRANK_POINT_ADD_TYPE_NONE:				// ポイントを加算しない
							rbPointAddTypeNone.Checked = true;
							break;

						case Constants.FLG_MEMBERRANK_POINT_ADD_TYPE_RATE:				// 加算率指定
							rbPointAddTypeRate.Checked = true;
							tbPointAddRateValue.Text = StringUtility.ToEmpty(memberRank.PointAddValue);
							break;

						default:
							break;
					}
					// 配送料割引数指定
					switch ((string)memberRank.ShippingDiscountType)
					{
						case Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE:		// 配送手数料割引しない
							rbShippingDiscountTypeNone.Checked = true;
							break;

						case Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FIXED:		// 割引金額指定
							rbShippingDiscountTypeFixed.Checked = true;
							tbShippingDiscountFixedValue.Text = StringUtility.ToEmpty(memberRank.ShippingDiscountValue).ToPriceString();
							break;

						case Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FREE:		// 配送手数料無料
							rbShippingDiscountTypeFree.Checked = true;
							break;

						case Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FRESHP_TSD:// 配送料無料最低金額設定
							rbShippingDiscountTypeFreeShippingThreshold.Checked = true;
							tbShippingDiscountFreeShippingThresholdValue.Text = StringUtility.ToEmpty(memberRank.ShippingDiscountValue).ToPriceString();
							break;

						default:
							break;
					}
					// 定期会員割引率
					if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
					{
						tbFixedPurchaseDiscountRate.Text = StringUtility.ToEmpty(DecimalUtility.DecimalRound(memberRank.FixedPurchaseDiscountRate, DecimalUtility.Format.RoundDown, 0));
					}
					// ランクメモ
					tbRankMemo.Text = (string)memberRank.MemberRankMemo;
					// 有効フラグ
					cbValidFlg.Checked = memberRank.IsValid;
				}

				// 編集時の入力チェック用
				ViewState.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ORDER, tbRankOrder.Text);	// ランク順位
			}
			// それ以外の場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
			}
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	/// <param name="strActionStatus"></param>
	private void InitializeComponents(string strActionStatus)
	{
		// 新規登録
		if (strActionStatus == Constants.ACTION_STATUS_INSERT)
		{
			trRegister.Visible = true;
			trRankIdRegister.Visible = true;
		}
		// 編集
		else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			trEdit.Visible = true;
			trRankIdEdit.Visible = true;
		}
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirmTop_Click(object sender, System.EventArgs e)
	{
		string strRankId = GetRankIdFromInput();
		string strActionStatus = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];

		string strValidator = null;
		string strCheckRankId = null;		// 入力チェック用
		string strCheckRankOrder = null;	// 入力チェック用

		//------------------------------------------------------
		// 処理ステータス
		//------------------------------------------------------
		switch (strActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:	// 新規登録
				strValidator = "MemberRankRegist";
				strCheckRankId = tbRankId.Text.Trim();
				strCheckRankOrder = tbRankOrder.Text.Trim();
				break;

			case Constants.ACTION_STATUS_UPDATE:	// 編集
				strValidator = "MemberRankModify";
				strCheckRankId = (string)ViewState[Constants.REQUEST_KEY_MEMBERRANK_ID];
				strCheckRankOrder = (string)ViewState[Constants.FIELD_MEMBERRANK_MEMBER_RANK_ORDER];
				break;
		}

		//------------------------------------------------------
		// パラメタ格納
		//------------------------------------------------------
		Hashtable htInput = new Hashtable();
		htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, strRankId);					// ランクID
		htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID + "_old", strCheckRankId);
		htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ORDER, tbRankOrder.Text.Trim());		// ランク順位
		htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ORDER + "_old", strCheckRankOrder);
		htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME, tbRankName.Text.Trim());			// ランク名

		// 初期化
		htInput.Add(Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_VALUE, null);					// 注文割引数
		htInput.Add(Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_THRESHOLD_PRICE, null);		// 注文金額割引き閾値

		// 注文割引方法指定
		if (rbOrderDiscountTypeNone.Checked)
		{
			// 割引きしない
			htInput.Add(Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_TYPE, Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_NONE);
		}
		else if (rbOrderDiscountTypeRate.Checked)
		{
			// 割引率指定
			htInput.Add(Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_TYPE, Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_RATE);
			htInput[Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_VALUE] = tbOrderDiscountRateValue.Text.Trim();
			htInput.Add("order_discount_value_rate", tbOrderDiscountRateValue.Text.Trim());	// 入力チェック用
		}
		else if (rbOrderDiscountTypeFixed.Checked)
		{
			// 割引金額指定
			htInput.Add(Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_TYPE, Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_FIXED);
			htInput[Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_VALUE] = tbOrderDiscountFixedValue.Text.Trim();
			htInput.Add("order_discount_value_fixed", tbOrderDiscountFixedValue.Text.Trim());	// 入力チェック用
			htInput[Constants.FIELD_MEMBERRANK_ORDER_DISCOUNT_THRESHOLD_PRICE] = (string.IsNullOrEmpty(tbOrderDiscountThresholdPrice.Text) == false) ? tbOrderDiscountThresholdPrice.Text.Trim() : null;
		}

		// ポイント加算方法指定
		if (rbPointAddTypeNone.Checked)
		{
			// ポイントを加算しない
			htInput.Add(Constants.FIELD_MEMBERRANK_POINT_ADD_TYPE, Constants.FLG_MEMBERRANK_POINT_ADD_TYPE_NONE);
			htInput.Add(Constants.FIELD_MEMBERRANK_POINT_ADD_VALUE, null);
		}
		else if (rbPointAddTypeRate.Checked)
		{
			// 加算率指定
			htInput.Add(Constants.FIELD_MEMBERRANK_POINT_ADD_TYPE, Constants.FLG_MEMBERRANK_POINT_ADD_TYPE_RATE);
			htInput.Add(Constants.FIELD_MEMBERRANK_POINT_ADD_VALUE, tbPointAddRateValue.Text.Trim());
			htInput.Add("point_add_value_rate", tbPointAddRateValue.Text.Trim());	// 入力チェック用
		}

		// 配送料割引数指定
		if (rbShippingDiscountTypeNone.Checked)
		{
			// 配送手数料割引しない
			htInput.Add(Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_TYPE, Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE);
			htInput.Add(Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_VALUE, null);
		}
		else if (rbShippingDiscountTypeFixed.Checked)
		{
			// 割引金額指定
			htInput.Add(Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_TYPE, Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FIXED);
			htInput.Add(Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_VALUE, tbShippingDiscountFixedValue.Text.Trim());
			htInput.Add("shipping_discount_value_fixed", tbShippingDiscountFixedValue.Text.Trim());	// 入力チェック用
		}
		else if (rbShippingDiscountTypeFree.Checked)
		{
			// 配送手数料無料
			htInput.Add(Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_TYPE, Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FREE);
			htInput.Add(Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_VALUE, null);
		}
		else if (rbShippingDiscountTypeFreeShippingThreshold.Checked)
		{
			// 配送料無料最低金額設定
			htInput.Add(Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_TYPE, Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FRESHP_TSD);
			htInput.Add(Constants.FIELD_MEMBERRANK_SHIPPING_DISCOUNT_VALUE, tbShippingDiscountFreeShippingThresholdValue.Text.Trim());
			htInput.Add("shipping_discount_value_free_shipping_threshold", tbShippingDiscountFreeShippingThresholdValue.Text.Trim());
		}

		// 定期会員割引率
		htInput.Add(Constants.FIELD_MEMBERRANK_FIXED_PURCHASE_DISCOUNT_RATE,
			Constants.FIXEDPURCHASE_OPTION_ENABLED ? tbFixedPurchaseDiscountRate.Text.Trim() : "0");
		// ランクメモ
		htInput.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_MEMO, tbRankMemo.Text.Trim());
		// 有効フラグ
		htInput.Add(Constants.FIELD_MEMBERRANK_VALID_FLG, cbValidFlg.Checked ? Constants.FLG_MEMBERRANK_VALID_FLG_VALID : Constants.FLG_MEMBERRANK_VALID_FLG_INVALID);

		//--------------------------------------------
		// 入力チェック＆重複チェック
		//--------------------------------------------
		string strErrorMessage = Validator.Validate(strValidator, htInput);
		if (strErrorMessage.Length != 0)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		//--------------------------------------------
		// 画面遷移
		//--------------------------------------------
		// パラメタをセッションへ格納
		Session[Constants.SESSIONPARAM_KEY_MEMBERRANK_INFO] = htInput;
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_PARAM] = strActionStatus;

		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_CONFIRM);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_MEMBERRANK_ID).Append("=").Append(strRankId);
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(strActionStatus);

		// 会員ランク情報確認画面へ
		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	///  ランクIDを画面から取得
	/// </summary>
	/// <returns>ランクID</returns>
	private string GetRankIdFromInput()
	{
		switch ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS])
		{
			case Constants.ACTION_STATUS_INSERT:	// 新規登録
				return tbRankId.Text.Trim();

			case Constants.ACTION_STATUS_UPDATE:	// 編集
				return lRankId.Text;
		}

		return null;
	}
}
