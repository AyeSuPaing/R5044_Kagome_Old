/*
=========================================================================================================
  Module      : ブランド情報一覧ページ(ProductBrandList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Web;
using w2.App.Common.Product;

public partial class Form_ProductBrand_ProductBrandList : ProductBrandPage
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
			// リクエスト情報取得
			//------------------------------------------------------
			Hashtable htParam = GetParameters();

			//------------------------------------------------------
			// デフォルトブランド取得
			//------------------------------------------------------
			this.DefaultBrandId = ProductBrandUtility.GetDefaultBrandId();

			//------------------------------------------------------
			// ブランド一覧件数取得
			//------------------------------------------------------
			int iTotalCounts = ProductBrandUtility.GetProductBrandCount();

			//------------------------------------------------------
			// エラー表示制御
			//------------------------------------------------------
			bool blDisplayPager = true;
			StringBuilder sbErrorMessage = new StringBuilder();

			// 該当件数なし？
			if (iTotalCounts == 0)
			{
				sbErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
			}

			tdErrorMessage.InnerHtml = sbErrorMessage.ToString();
			trListError.Visible = (sbErrorMessage.ToString().Length != 0);

			//------------------------------------------------------
			// ブランド一覧情報表示
			//------------------------------------------------------
			if (trListError.Visible == false)
			{
				// ブランド一覧情報取得
				DataView dvProductBrandList = ProductBrandUtility.GetProductBrandListPager(
					Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1,
					Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo);

				// データバインド
				rList.DataSource = dvProductBrandList;
				rList.DataBind();
			}

			//------------------------------------------------------
			// ページャ作成（一覧処理で総件数を取得）
			//------------------------------------------------------
			if (blDisplayPager)
			{
				string strNextUrl = CreateListUrl();
				lbPager1.Text = WebPager.CreateDefaultListPager(iTotalCounts, this.CurrentPageNo, strNextUrl);
			}
		}
	}

	/// <summary>
	/// ブランド情報一覧パラメタ取得
	/// </summary>
	/// <returns>パラメタが格納されたHashtable</returns>
	private Hashtable GetParameters()
	{
		Hashtable htParam = new Hashtable();

		try
		{
			// ページ番号（ページャ動作時のみもちまわる）
			int iPageNo;
			if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out iPageNo) == false)
			{
				iPageNo = 1;
			}
			htParam.Add(Constants.REQUEST_KEY_PAGE_NO, iPageNo.ToString());
			this.CurrentPageNo = iPageNo;
		}
		catch (Exception ex)
		{
			AppLogger.WriteError(ex);

			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		return htParam;
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 新規登録画面へ
		Response.Redirect(CreateProductBrandRegistUrl("", Constants.ACTION_STATUS_INSERT));
	}

	/// <summary>
	/// デフォルト設定リセットボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDefaultReset_Click(object sender, EventArgs e)
	{
		// デフォルトブランドをリセット
		ProductBrandUtility.ResetDefaultBrandId(this.LoginOperatorName);

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTBRAND_LIST);
	}

	/// <summary>
	/// デフォルト設定更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDefaultUpdate_Click(object sender, EventArgs e)
	{
		// デフォルトブランドを変更
		ProductBrandUtility.SetDefaultBrandId(Request["rbDefaultBrand"], this.LoginOperatorName);

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTBRAND_LIST);
	}

	/// <summary>
	/// 翻訳設定出力リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbExportTranslationData_OnClick(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM] = new string[0];
		Session[Constants.SESSION_KEY_NAMETRANSLATIONSETTING_EXPORT_TARGET_DATAKBN] = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NAMETRANSLATIONSETTING_EXPORT);
	}
	
	/// <summary>カレントページNO</summary>
	protected int CurrentPageNo
	{
		get { return (int)ViewState[Constants.REQUEST_KEY_PAGE_NO]; }
		set { ViewState[Constants.REQUEST_KEY_PAGE_NO] = value; }
	}
	/// <summary>デフォルトブランドID</summary>
	public string DefaultBrandId { get; set; }

}