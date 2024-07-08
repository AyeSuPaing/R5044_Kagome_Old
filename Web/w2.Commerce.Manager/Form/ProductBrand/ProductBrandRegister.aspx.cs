/*
=========================================================================================================
  Module      : ブランド情報登録ページ(ProductBrandRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Text;
using System.Web;
using w2.App.Common.Product;

public partial class Form_ProductBrand_ProductBrandRegister : ProductBrandPage
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
			//------------------------------------------------------
			// リクエストよりパラメタ取得
			//------------------------------------------------------
			this.BrandId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCTBRAND_BRAND_ID]);

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(this.ActionStatus);

			//------------------------------------------------------
			// 処理区分チェック
			//------------------------------------------------------
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			//------------------------------------------------------
			// セッションにあれば値セット
			//------------------------------------------------------
			if (Session[Constants.SESSIONPARAM_KEY_PRODUCTBRAND_INFO] != null)
			{
				this.ProductBrandMaster = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTBRAND_INFO];
			}

			//------------------------------------------------------
			// 情報取得処理
			//------------------------------------------------------
			switch (this.ActionStatus)
			{
				// 新規登録
				case Constants.ACTION_STATUS_INSERT:
					break;

				// コピー新規登録 OR 編集
				case Constants.ACTION_STATUS_COPY_INSERT:
				case Constants.ACTION_STATUS_UPDATE:

					// ブランド情報取得
					if (this.ProductBrandMaster == null)
					{
						DataView dvProductBrand = ProductBrandUtility.GetProductBrand(this.BrandId);
						if (dvProductBrand.Count == 0)
						{
							// 情報がない場合はエラー画面へ
							Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
							Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
						}

						this.ProductBrandMaster = dvProductBrand[0];
					}
					break;

				default:
					// エラーページへ
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					break;
			}

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			DataBind();
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	/// <param name="strActionStatus">アクション区分</param>
	private void InitializeComponents(string strActionStatus)
	{
		//------------------------------------------------------
		// 表示切替
		//------------------------------------------------------
		if (strActionStatus == Constants.ACTION_STATUS_INSERT)
		{
			trRegister.Visible = true;
			trEdit.Visible = false;
			trBrandIdRegister.Visible = true;
		}
		else if (strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
		{
			trRegister.Visible = true;
			trEdit.Visible = false;
			trBrandIdRegister.Visible = true;
		}
		else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			trRegister.Visible = false;
			trEdit.Visible = true;
			trBrandIdEdit.Visible = true;
		}
	}

	/// <summary>
	///  ブランド情報を画面から取得する
	/// </summary>
	/// <returns>ブランド情報</returns>
	private Hashtable GetInputProductBrand()
	{
		Hashtable htInput = new Hashtable();
		htInput.Add(Constants.FIELD_PRODUCTBRAND_BRAND_ID, tbBrandId.Text.Trim());			// ブランドID
		htInput.Add(Constants.FIELD_PRODUCTBRAND_BRAND_NAME, tbBrandName.Text);				// ブランド名
		htInput.Add(Constants.FIELD_PRODUCTBRAND_BRAND_TITLE, tbBrandTitle.Text);			// ブランドタイトル
		htInput.Add(Constants.FIELD_PRODUCTBRAND_SEO_KEYWORD, tbSeoKeyword.Text);			// SEOキーワード
		htInput.Add(Constants.FIELD_PRODUCTBRAND_ADDITIONAL_DESIGN_TAG, tbMetaInfo.Text);	// 追加デザインタグ
		htInput.Add(Constants.FIELD_PRODUCTBRAND_VALID_FLG,									// 有効フラグ
			(cbValidFlg.Checked) ? Constants.FLG_PRODUCTBRAND_VALID_FLG_VALID : Constants.FLG_PRODUCTBRAND_VALID_FLG_INVALID);

		return htInput;
	}

	/// <summary>
	///  ブランド情報の入力チェック
	/// </summary>
	/// <param name="htProductBrand">ブランド情報</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckInputProductBrandData(Hashtable htProductBrand)
	{
		string strErrorMessage = "";

		// 入力チェック＆重複チェック
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				strErrorMessage = Validator.Validate("ProductBrandRegist", htProductBrand);
				break;

			case Constants.ACTION_STATUS_UPDATE:
				strErrorMessage = Validator.Validate("ProductBrandModity", htProductBrand);
				break;
		}

		return strErrorMessage;
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, System.EventArgs e)
	{
		switch (this.ActionStatus)
		{
			// 新規
			case Constants.ACTION_STATUS_INSERT:

				// 一覧画面へ
				Response.Redirect(CreateListUrl());
				break;

			// 更新・コピー新規
			case Constants.ACTION_STATUS_UPDATE:
			case Constants.ACTION_STATUS_COPY_INSERT:

				// 詳細画面へ
				Response.Redirect(CreateProductBrandDetailUrl(this.BrandId));
				break;
		}
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 値取得・セッションへセット
		//------------------------------------------------------
		Hashtable htProductBrand = GetInputProductBrand();
		Session[Constants.SESSIONPARAM_KEY_PRODUCTBRAND_INFO] = htProductBrand;

		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		StringBuilder sbErrorMessages = new StringBuilder();
		sbErrorMessages.Append(CheckInputProductBrandData(htProductBrand));

		if (sbErrorMessages.ToString() != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = sbErrorMessages.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// 画面遷移
		//------------------------------------------------------
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = this.ActionStatus;

		// ブランド情報確認ページへ遷移
		Response.Redirect(
			Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTBRAND_CONFIRM +
			"?" + Constants.REQUEST_KEY_PRODUCTBRAND_BRAND_ID + "=" + HttpUtility.UrlEncode(this.BrandId) +
			"&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + this.ActionStatus);
	}

	/// <summary>ブランドマスタ</summary>
	protected object ProductBrandMaster { get; set; }
	/// <summary>ブランドID</summary>
	protected string BrandId
	{
		get { return (string)ViewState["BrandId"]; }
		set { ViewState["BrandId"] = value; }
	}
}
