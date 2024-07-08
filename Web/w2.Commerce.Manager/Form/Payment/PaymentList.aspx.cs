/*
=========================================================================================================
  Module      : 決済種別情報一覧ページ処理(PaymentList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.App.Common.Extensions.Currency;
using w2.Domain.Payment;

public partial class Form_Payment_PaymentList : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 決済種別情報一覧表示
			ViewPaymentList();
		}
	}


	/// <summary>
	/// 決済種別情報一覧表示(DataGridにDataView(決済種別情報)を設定)
	/// </summary>
	private void ViewPaymentList()
	{
		// 変数宣言
		Hashtable htParam = new Hashtable();

		//------------------------------------------------------
		// リクエスト情報取得
		//------------------------------------------------------
		htParam = GetParameters(Request);
		// 不正パラメータが存在した場合
		if ((bool)htParam[Constants.ERROR_REQUEST_PRAMETER])
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		int iCurrentPageNumber = (int)htParam[Constants.REQUEST_KEY_PAGE_NO];

		//------------------------------------------------------
		// 決済種別一覧
		//------------------------------------------------------
		int iTotalPaymentCounts = 0;	// ページング可能総商品数

		var paymentList = new PaymentService().GetPaymentList(
			this.LoginOperatorShopId,
			Constants.CONST_DISP_CONTENTS_PAYMENT_LIST,
			iCurrentPageNumber,
			Constants.AMAZON_PAYMENT_OPTION_ENABLED,
			Constants.AMAZON_PAYMENT_CV2_ENABLED,
			Constants.PAYMENT_GMO_POST_ENABLED,
			Constants.PAYMENT_GMOATOKARA_ENABLED);

		if (paymentList.Length != 0)
		{
			iTotalPaymentCounts = new PaymentService().GetPaymentListEnabled(
				this.LoginOperatorShopId,
				Constants.AMAZON_PAYMENT_OPTION_ENABLED,
				Constants.AMAZON_PAYMENT_CV2_ENABLED,
				Constants.PAYMENT_GMO_POST_ENABLED,
				Constants.PAYMENT_GMOATOKARA_ENABLED).Length;
			// エラー非表示制御
			trListError.Visible = false;
		}
		else
		{
			iTotalPaymentCounts = 0;
			// エラー表示制御
			trListError.Visible = true;
			tdErrorMessage.InnerHtml =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);

		}
		// データソースセット
		rList.DataSource = paymentList;

		//------------------------------------------------------
		// ページャ作成（一覧処理で総件数を取得）
		//------------------------------------------------------
		string strNextUrl = Constants.PATH_ROOT + Constants.PAGE_MANAGER_PAYMENT_LIST;
		lbPager1.Text = WebPager.CreatePaymentPager(iTotalPaymentCounts, iCurrentPageNumber, strNextUrl);

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		DataBind();
	}


	/// <summary>
	/// 決済種別情報一覧パラメタ取得
	/// </summary>
	/// <param name="hrRequest">決済種別情報一覧のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	/// <remarks>
	/// </remarks>
	private static Hashtable GetParameters(System.Web.HttpRequest hrRequest)
	{
		// 変数宣言
		Hashtable htResult = new Hashtable();

		int iCurrentPageNumber = 1;
		bool blParamError = false;

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		// ページ番号（ページャ動作時のみもちまわる）
		try
		{
			if (StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PAGE_NO]) != "")
			{
				iCurrentPageNumber = int.Parse(hrRequest[Constants.REQUEST_KEY_PAGE_NO]);
			}
		}
		catch
		{
			blParamError = true;
		}

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		htResult.Add(Constants.REQUEST_KEY_PAGE_NO, iCurrentPageNumber);
		htResult.Add(Constants.ERROR_REQUEST_PRAMETER, blParamError);

		return htResult;
	}

	/// <summary>
	/// 有効金額文字列作成
	/// </summary>
	/// <param name="drvPayment"></param>
	/// <returns></returns>
	protected string CreateUsablePriceString(object objUsablePrice)
	{
		return (objUsablePrice != DBNull.Value) ? objUsablePrice.ToPriceString(true) : "-";
	}


	/// <summary>
	/// データバインド用決済種別情報詳細URL作成
	/// </summary>
	/// <param name="strPaymentId">決済種別ID</param>
	/// <returns>決済種別情報詳細URL</returns>
	protected string CreatePaymentDetailUrl(string strPaymentId)
	{
		string strResult = "";
		strResult += Constants.PATH_ROOT + Constants.PAGE_MANAGER_PAYMENT_CONFIRM;
		strResult += "?";
		strResult += Constants.REQUEST_KEY_PAYMENT_ID + "=" + strPaymentId;
		strResult += "&";
		strResult += Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_DETAIL;

		return strResult;
	}


	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 新規登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PAYMENT_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}

	/// <summary>
	/// 翻訳データ出力リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbExportTranslationData_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM] = new string[0];
		Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN] = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_EXPORT);
	}
}

