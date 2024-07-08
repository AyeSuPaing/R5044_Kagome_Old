/*
=========================================================================================================
  Module      : ブランド情報確認ページ(ProductBrandConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;
using System.Web;
using w2.App.Common.Product;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;

public partial class Form_ProductBrand_ProductBrandConfirm : ProductBrandPage
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
			InitializeComponents();

			switch (this.ActionStatus)
			{
				//------------------------------------------------------
				// 確認画面表示処理
				//------------------------------------------------------
				case Constants.ACTION_STATUS_INSERT:		// 新規登録確認
				case Constants.ACTION_STATUS_COPY_INSERT:	// コピー登録確認
				case Constants.ACTION_STATUS_UPDATE:		// 更新確認

					// 処理区分チェック
					CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

					// ブランド情報取得
					this.ProductBrandMaster = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTBRAND_INFO];

					break;

				//------------------------------------------------------
				// 詳細画面表示処理
				//------------------------------------------------------
				case Constants.ACTION_STATUS_DETAIL:

					// ブランド情報取得
					DataView dvProductBrand = ProductBrandUtility.GetProductBrand(this.BrandId);

					// 情報がない場合はエラー画面へ
					if (dvProductBrand.Count == 0)
					{
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}

					this.ProductBrandMaster = dvProductBrand[0];
					Session[Constants.SESSIONPARAM_KEY_PRODUCTBRAND_INFO] = null;

					if (Constants.GLOBAL_OPTION_ENABLE)
					{
						this.ProductBrandTranslationData = GetProductBrandTranslationData(this.BrandId);
						rTranslationBrandName.DataBind();
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
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 新規・コピー新規？
		if ((this.ActionStatus == Constants.ACTION_STATUS_INSERT)
			|| (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
		{
			btnInsertTop.Visible = true;
			btnInsertBottom.Visible = true;
			trConfirm.Visible = true;
			btnBackListTop.Visible = false;
			btnBackListBottom.Visible = false;
		}
		// 更新？
		else if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			btnUpdateTop.Visible = true;
			btnUpdateBottom.Visible = true;
			trConfirm.Visible = true;
			btnBackListTop.Visible = false;
			btnBackListBottom.Visible = false;
		}
		// 詳細
		else if (this.ActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			btnBackTop.Visible = false;
			btnBackBottom.Visible = false;
			btnEditTop.Visible = true;
			btnEditBottom.Visible = true;
			btnCopyInsertTop.Visible = true;
			btnCopyInsertBottom.Visible = true;
			btnDeleteTop.Visible = true;
			btnDeleteBottom.Visible = true;
			trDateCreated.Visible = true;
			trDateChanged.Visible = true;
			trLastChanged.Visible = true;
			trDetail.Visible = true;
			btnBackListTop.Visible = true;
			btnBackListBottom.Visible = true;
		}
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
			// 詳細
			case Constants.ACTION_STATUS_DETAIL:

				// 一覧画面へ
				Response.Redirect(CreateListUrl());
				break;

			// 新規、コピー新規、更新
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:

				// 編集画面へ
				Response.Redirect(CreateProductBrandRegistUrl(this.BrandId, this.ActionStatus));
				break;
		}
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へ
		Response.Redirect(CreateProductBrandRegistUrl(Request[Constants.REQUEST_KEY_PRODUCTBRAND_BRAND_ID], Constants.ACTION_STATUS_UPDATE));
	}

	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT;

		// 登録画面へ
		Response.Redirect(CreateProductBrandRegistUrl(Request[Constants.REQUEST_KEY_PRODUCTBRAND_BRAND_ID], Constants.ACTION_STATUS_COPY_INSERT));
	}

	/// <summary>
	/// 登録する/更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertUpdate_Click(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// ブランド情報、ブランドアイテム情報取得
		//------------------------------------------------------
		Hashtable htProductBrand = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTBRAND_INFO];
		htProductBrand[Constants.FIELD_PRODUCTBRAND_LAST_CHANGED] = this.LoginOperatorName;		// 最終更新者

		// ブランドIDの整合性チェック
		if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
			&& ((string)Request[Constants.REQUEST_KEY_PRODUCTBRAND_BRAND_ID] != (string)htProductBrand[Constants.FIELD_PRODUCTBRAND_BRAND_ID]))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_POPUP_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// DB登録/更新
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			//------------------------------------------------------
			// ブランド情報登録/更新
			//------------------------------------------------------
			if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
			{
				// 更新
				ProductBrandUtility.UpdateBrand(htProductBrand, sqlAccessor);
			}
			else
			{
				// 新規・新規コピー
				ProductBrandUtility.InsertBrand(htProductBrand, sqlAccessor);
			}
		}

		//------------------------------------------------------
		// 詳細表示画面へ遷移
		//------------------------------------------------------
		Response.Redirect(CreateProductBrandDetailUrl((string)htProductBrand[Constants.FIELD_PRODUCTBRAND_BRAND_ID]));
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// DB削除
		//------------------------------------------------------
		ProductBrandUtility.DeleteBrand(this.BrandId);

		//------------------------------------------------------
		// 一覧画面へ戻る
		//------------------------------------------------------
		Response.Redirect(CreateListUrl());
	}

	#region -GetProductBrandTranslationData 商品ブランド翻訳設定情報取得
	/// <summary>
	/// 商品ブランド翻訳設定情報取得
	/// </summary>
	/// <param name="brandId">ブランドID</param>
	/// <returns>商品ブランド翻訳設定情報</returns>
	private NameTranslationSettingModel[] GetProductBrandTranslationData(string brandId)
	{
		var searchCondition = new NameTranslationSettingSearchCondition
		{
			DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND,
			MasterId1 = brandId,
			MasterId2 = string.Empty,
			MasterId3 = string.Empty,
		};
		var translationData = new NameTranslationSettingService().GetTranslationSettingsByMasterId(searchCondition);
		return translationData;
	}
	#endregion

	/// <summary>ブランドマスタ</summary>
	protected object ProductBrandMaster { get; set; }
	/// <summary>ブランドID</summary>
	protected string BrandId
	{
		get { return (string)ViewState["BrandId"]; }
		set { ViewState["BrandId"] = value; }
	}
	/// <summary>商品ブランド翻訳設定情報</summary>
	protected NameTranslationSettingModel[] ProductBrandTranslationData
	{
		get { return (NameTranslationSettingModel[])ViewState["productbrand_translation_data"]; }
		set { ViewState["productbrand_translation_data"] = value; }
	}
}

