/*
=========================================================================================================
  Module      : 決済種別情報登録ページ処理(PaymentRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.User;
using w2.Domain.UserManagementLevel;

public partial class Form_Payment_PaymentRegister : PaymentPage
{
	protected Hashtable m_htParam = new Hashtable();					// 決済種別情報データバインド用
	protected ArrayList m_alParam = new ArrayList();					// 決済手数料情報データバインド用

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
				Hashtable htParam = new Hashtable();
				htParam.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE_NO, 1);	// 枝番
				htParam.Add(Constants.FIELD_PAYMENTPRICE_TGT_PRICE_BGN, 0);		// 対象購入金額（以上）
				htParam.Add(Constants.FIELD_PAYMENTPRICE_TGT_PRICE_END, 0);		// 対象購入金額（以下）
				htParam.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE, 0);		// 決済手数料
				m_alParam.Add(htParam);
			}
			// コピー新規・編集？
			else if (strActionStatus == Constants.ACTION_STATUS_COPY_INSERT ||
				strActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				// セッションより決済種別情報取得
				m_htParam = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PAYMENT_INFO];							// 決済種別情報
				m_alParam = (ArrayList)m_htParam[Constants.SESSIONPARAM_KEY_PAYMENT_PRICE_INFO];					// 決済手数料情報
				ViewState.Add(Constants.FIELD_PAYMENT_PAYMENT_ID, m_htParam[Constants.FIELD_PAYMENT_PAYMENT_ID]);

				//ddlPaymentKbn.SelectedValue = ;				// 決済種別
				foreach (ListItem li in rblPaymentPriceKbn.Items)
				{
					li.Selected = (li.Value == (string)m_htParam[Constants.FIELD_PAYMENT_PAYMENT_PRICE_KBN]);		// 決済手数料区分
				}

				// 決済手数料入力域表示切替
				DisplayPaymentPrice(rblPaymentPriceKbn.SelectedValue);

				// Set values to user management level not use payment check box list
				SetSearchCheckBoxValue(cblUserManagementLevelList, ((string)m_htParam[Constants.FIELD_PAYMENT_USER_MANAGEMENT_LEVEL_NOT_USE]).Split(','));

				// 使用不可注文者区分チェックボックスにデータセット
				SetSearchCheckBoxValue(cblOrderOwnerKbnLevelList, ((string)m_htParam[Constants.FIELD_PAYMENT_ORDER_OWNER_KBN_NOT_USE]).Split(','));
			}
			// それ以外の場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// データバインド
			DataBind();
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		// 変数宣言

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

		// 手数料区分
		bool blDefalut = true;
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_PAYMENT, Constants.FIELD_PAYMENT_PAYMENT_PRICE_KBN))
		{
			li.Selected = blDefalut;
			rblPaymentPriceKbn.Items.Add(li);
			blDefalut = false;
		}

		// モバイル表示フラグ
		blDefalut = true;
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_PAYMENT, Constants.FIELD_PAYMENT_MOBILE_DISP_FLG))
		{
			li.Selected = blDefalut;
			ddlMobileDispFlg.Items.Add(li);
			blDefalut = false;
		}

		// Generate check box list of user management level not use payment
		cblUserManagementLevelList.Items.Clear();

		// ユーザー管理レベルドロップダウン作成
		var models = new UserManagementLevelService().GetAllList()
			.Select(m => new ListItem(m.UserManagementLevelName, m.UserManagementLevelId))
			.ToArray();
		cblUserManagementLevelList.Items.AddRange(models);

		// 注文者区分リスト作成
		// 全ての会員、全てのゲストをセットする
		cblOrderOwnerKbnLevelList.Items.Clear();
		cblOrderOwnerKbnLevelList.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDEROWNER, Constants.VALUE_TEXT_KEY_ORDEROWNER_USER_KBN_ALL));
	}


	/// <summary>
	/// 決済手数料入力域表示切替処理
	/// </summary>
	/// <param name="strPaymentPriceKbn">決済手数料区分</param>
	private void DisplayPaymentPrice(string strPaymentPriceKbn)
	{
		// 決済手数料を分けない
		if (strPaymentPriceKbn == Constants.FLG_PAYMENT_PAYMENT_PRICE_KBN_SINGULAR)
		{
			trPaymentPrice.Visible = true;
			rPaymentPrice.Visible = false;
		}
		// 決済手数料を分ける
		else if (strPaymentPriceKbn == Constants.FLG_PAYMENT_PAYMENT_PRICE_KBN_PLURAL)
		{
			trPaymentPrice.Visible = false;
			rPaymentPrice.Visible = true;
		}
	}

	/// <summary>
	/// ドロップダウンリスト用アイテムデータ作成
	/// ColorTextField = strText
	/// ColorValueField = strValue
	/// </summary>
	/// <param name="strText">テキスト値</param>
	/// <param name="strValue">値</param>
	/// <param name="dt">データテーブル</param>
	/// <returns>データテーブル</returns>
	private DataRow CreateRow(string strText, string strValue, DataTable dt)
	{
		// 変数宣言
		DataRow drResult = dt.NewRow();

		drResult[0] = strText;
		drResult[1] = strValue;

		return drResult;
	}


	/// <summary>
	/// 決済手数料クリックイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	public void rPaymentPrice_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
	{
		// 変数宣言
		Hashtable htParam = new Hashtable();

		// 決済手数料情報を取得
		int iPaymentPrices = rPaymentPrice.Items.Count;
		for (int iLoop = 0; iLoop < iPaymentPrices; iLoop++)
		{
			Hashtable htPaymentPrice = new Hashtable();
			htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE_NO, iLoop + 1);		// 枝番
			htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_TGT_PRICE_BGN, 0);					// 対象購入金額（以上）
			htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_TGT_PRICE_END,
				((TextBox)rPaymentPrice.Items[iLoop].FindControl("tbTgtPriceEnd")).Text);		// 対象購入金額（以下）
			htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE,
				((TextBox)rPaymentPrice.Items[iLoop].FindControl("tbPaymentPrices")).Text);		// 決済手数料
			m_alParam.Add(htPaymentPrice);
		}

		// 手数料追加の場合
		if (e.CommandName == "InsertPaymentPrice")
		{
			htParam.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE_NO, m_alParam.Count + 1);		// 枝番
			htParam.Add(Constants.FIELD_PAYMENTPRICE_TGT_PRICE_BGN, 0);							// 対象購入金額（以上）
			htParam.Add(Constants.FIELD_PAYMENTPRICE_TGT_PRICE_END, 0);							// 対象購入金額（以下）
			htParam.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE, 0);							// 決済手数料
			m_alParam.Add(htParam);
		}
		// 手数料削除の場合
		else if (e.CommandName == "DeletePaymentPrice")
		{
			// 削除処理
			int iDeletePrice = int.Parse(e.CommandArgument.ToString());
			m_alParam.RemoveAt(iDeletePrice - 1);
			for (int iLoop = 0; iLoop < m_alParam.Count; iLoop++)
			{
				htParam = (Hashtable)m_alParam[iLoop];
				htParam[Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE_NO] = iLoop + 1;
			}

		}

		rPaymentPrice.DataSource = m_alParam;
		rPaymentPrice.DataBind();
	}


	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, System.EventArgs e)
	{
		// 変数宣言
		string strPaymentId = (string)ViewState[Constants.FIELD_PAYMENT_PAYMENT_ID];
		string strValidator = null;
		string strErrorMessagesPaymentPrice = String.Empty;
		decimal dTgtPriceBgn = 0;

		//------------------------------------------------------
		// 処理ステータス
		//------------------------------------------------------		
		// 新規・コピー新規
		if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT ||
			(string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_COPY_INSERT)
		{
			strValidator = "PaymentRegist";
		}
		// 変更
		else if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
		{
			strValidator = "PaymentModify";
		}

		//------------------------------------------------------
		// パラメタ格納
		//------------------------------------------------------
		Hashtable htParam = new Hashtable();
		ArrayList alPaymentPrice = new ArrayList();

		htParam.Add(Constants.FIELD_PAYMENT_SHOP_ID, this.LoginOperatorShopId);						// 店舗ID
		htParam.Add(Constants.FIELD_PAYMENT_PAYMENT_ID, strPaymentId);									// 決済種別ID
		htParam.Add(Constants.FIELD_PAYMENT_PAYMENT_NAME, tbPaymentName.Text);							// 決済種別名
		htParam.Add(Constants.FIELD_PAYMENT_USABLE_PRICE_MIN, (tbUsablePriceBgn.Text.Trim().Length != 0) ? tbUsablePriceBgn.Text : null);	// 金額下限
		htParam.Add(Constants.FIELD_PAYMENT_USABLE_PRICE_MAX, (tbUsablePriceEnd.Text.Trim().Length != 0) ? tbUsablePriceEnd.Text : null);	// 金額上限
		htParam.Add(Constants.FIELD_PAYMENT_PAYMENT_PRICE_KBN, rblPaymentPriceKbn.SelectedValue);		// 決済手数料区分
		htParam.Add(Constants.FIELD_PAYMENT_MOBILE_DISP_FLG, ddlMobileDispFlg.SelectedValue);			// モバイル表示フラグ
		htParam.Add(Constants.FIELD_PAYMENT_VALID_FLG,
			(cbValidFlg.Checked ? Constants.FLG_PAYMENT_VALID_FLG_VALID : Constants.FLG_PAYMENT_INVALID_FLG_VALID));	// 有効フラグ
		htParam.Add(Constants.FIELD_PAYMENT_DISPLAY_ORDER, tbDisplayOrder.Text);						// 表示順
		htParam.Add(Constants.FIELD_PAYMENT_LAST_CHANGED,
			this.LoginOperatorName);								// 最終更新者
		htParam.Add(Constants.FIELD_PAYMENT_USER_MANAGEMENT_LEVEL_NOT_USE, CreateSearchStringParts(cblUserManagementLevelList.Items));		// Add param for user level list not use payment
		htParam.Add(Constants.FIELD_PAYMENT_ORDER_OWNER_KBN_NOT_USE, CreateSearchStringParts(cblOrderOwnerKbnLevelList.Items));		// Add param for order owner kbn list not use payment

		//------------------------------------------------------
		// 決済手数料読込
		//------------------------------------------------------
		// 決済手数料を分けない場合
		if (rblPaymentPriceKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_PRICE_KBN_SINGULAR)
		{
			// 入力チェックパラメータ設定
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PAYMENTPRICE_TGT_PRICE_END, "0".ToPriceString());				// 対象購入金額（以下）
			htInput.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE,
				tbPaymentPrice.Text);													// 決済手数料

			// 入力チェック
			strErrorMessagesPaymentPrice = Validator.Validate("PaymentPriceRegist", htInput).Replace("@@ 1 @@", "");
			// 入力エラーではない場合
			if (strErrorMessagesPaymentPrice == "")
			{
				htParam.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE,
					decimal.Parse(tbPaymentPrice.Text));										// 決済手数料

				// 決済手数料設定
				Hashtable htPaymentPrice = new Hashtable();
				htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE_NO, 1);			// 枝番
				htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_TGT_PRICE_BGN, 0);				// 対象購入金額（以上）
				htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_TGT_PRICE_END, 0);				// 対象購入金額（以下）
				htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE,
					decimal.Parse(tbPaymentPrice.Text));										// 決済手数料
				alPaymentPrice.Add(htPaymentPrice);
			}
		}
		// 決済手数料を分ける
		else if (rblPaymentPriceKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_PRICE_KBN_PLURAL)
		{
			int iPaymentPrices = rPaymentPrice.Items.Count;

			string strPaymentPriceBefore = "0".ToPriceString();
			for (int iLoop = 0; iLoop < iPaymentPrices; iLoop++)
			{
				// 決済手数料情報取得
				string strTgtPriceEnd = ((TextBox)rPaymentPrice.Items[iLoop].FindControl("tbTgtPriceEnd")).Text;
				string strPaymentPrice = ((TextBox)rPaymentPrice.Items[iLoop].FindControl("tbPaymentPrices")).Text;

				// 入力チェックパラメータ設定
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_PAYMENTPRICE_TGT_PRICE_END, strTgtPriceEnd);		// 対象購入金額（以下）
				htInput.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE, strPaymentPrice);		// 決済手数料

				// 入力チェック
				string strError = Validator.Validate("PaymentPriceRegist", htInput);

				if (strError.Length == 0)
				{
					// 対象購入金額整合性チェック
					if (decimal.Parse(strPaymentPriceBefore) >= decimal.Parse(strTgtPriceEnd))
					{
						strError = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_PAYMENTPRICE_THRESHOLD_ERROR);
					}
					strPaymentPriceBefore = strTgtPriceEnd;
				}

				strErrorMessagesPaymentPrice += strError.Replace("@@ 1 @@",
					String.Format(ReplaceTag("@@DispText.common_message.location_no@@"),
						(iLoop + 1).ToString(),
						string.Empty));
			}

			// エラーではない場合
			if (strErrorMessagesPaymentPrice == "")
			{
				// 対象購入金額（以下）の昇順で格納
				for (int iLoop = 0; iLoop < iPaymentPrices; iLoop++)
				{
					// 決済手数料情報取得
					decimal dTgtPriceEnd = decimal.Parse(((TextBox)rPaymentPrice.Items[iLoop].FindControl("tbTgtPriceEnd")).Text);
					decimal dPaymentPrice = decimal.Parse(((TextBox)rPaymentPrice.Items[iLoop].FindControl("tbPaymentPrices")).Text);

					Hashtable htPaymentPrice = new Hashtable();
					htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_TGT_PRICE_END, dTgtPriceEnd);						// 対象購入金額（以下）
					htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE, dPaymentPrice);						// 決済手数料

					// 対象購入金額（以下）
					int iPaymentIndex = 0;
					for (int iLoop2 = 0; iLoop2 < alPaymentPrice.Count; iLoop2++)
					{
						Hashtable htPaymentItem = (Hashtable)alPaymentPrice[iLoop2];
						decimal dTempTgtPriceEnd = (decimal)htPaymentItem[Constants.FIELD_PAYMENTPRICE_TGT_PRICE_END];
						if (dTgtPriceEnd < dTempTgtPriceEnd)
						{
							break;
						}
						iPaymentIndex++;
					}
					// 対象購入金額（To）でソート
					alPaymentPrice.Insert(iPaymentIndex, htPaymentPrice);
				}

				for (int iLoop = 0; iLoop < alPaymentPrice.Count; iLoop++)
				{
					Hashtable htPaymentPrice = (Hashtable)alPaymentPrice[iLoop];
					htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE_NO, iLoop + 1);			// 枝番
					htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_TGT_PRICE_BGN, dTgtPriceBgn);			// 対象購入金額（以上）

					dTgtPriceBgn = (decimal)htPaymentPrice[Constants.FIELD_PAYMENTPRICE_TGT_PRICE_END] + 1;	// 
				}
			}
		}
		htParam.Add(Constants.SESSIONPARAM_KEY_PAYMENT_PRICE_INFO, alPaymentPrice);

		// 入力チェック＆重複チェック
		string strErrorMessages = Validator.Validate(strValidator, htParam) + strErrorMessagesPaymentPrice;
		if (strErrorMessages != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// パラメタをセッションへ格納
		Session[Constants.SESSIONPARAM_KEY_PAYMENT_INFO] = htParam;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];

		// 決済種別情報確認ページへ遷移			
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PAYMENT_CONFIRM +
			"?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS]);
	}


	/// <summary>
	/// 手数料区分がクリックされた場合
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblPaymentPriceKbn_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		this.DisplayPaymentPrice(rblPaymentPriceKbn.SelectedValue);
	}
}
