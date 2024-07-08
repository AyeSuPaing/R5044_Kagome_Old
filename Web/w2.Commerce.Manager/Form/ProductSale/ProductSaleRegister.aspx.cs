/*
=========================================================================================================
  Module      : 商品セール情報登録ページ処理(ProductSaleRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using w2.App.Common.Extensions.Currency;
using w2.Domain.ProductSale;

public partial class Form_ProductSale_ProductSaleRegister : ProductPage
{
	string m_strProductSaleId = null;
	string m_strActionStatus = null;

	List<ProductSalePrice> m_lProductSalePrices = new List<ProductSalePrice>();

	const string VIEWSRATE_KEY_PRODUCT_SALE_PRICES = "productsaleprices";

	const string SESSION_KEY_DISP_COMP_MESSAGE = "dispcompmessage";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		// ユーザーコントロール割り当て
		uMasterDownload.OnCreateSearchInputParams += this.CreateSearchParams;

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			string m_strProductSaleId = Request[Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_ID];
			string m_strActionStatus = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ACTION_STATUS]);

			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponents(m_strActionStatus);

			//------------------------------------------------------
			// 画面設定処理
			//------------------------------------------------------
			switch (m_strActionStatus)
			{
				// 新規登録
				case Constants.ACTION_STATUS_INSERT:

					trDispProductSaleKbn.Visible = false;
					trDispProductSaleId.Visible = false;
					trSelectProductSaleKbn.Visible = true;
					break;

				// 編集・コピー新規登録
				case Constants.ACTION_STATUS_UPDATE:
				case Constants.ACTION_STATUS_COPY_INSERT:

					// 商品セールデータ取得
					DataView dvProductSale = null;
					using (SqlAccessor sqlAccessor = new SqlAccessor())
					using (SqlStatement sqlStatements = new SqlStatement("ProductSale", "GetProductSale"))
					{
						Hashtable htInput = new Hashtable();
						htInput.Add(Constants.FIELD_PRODUCTSALE_SHOP_ID, this.LoginOperatorShopId);
						htInput.Add(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID, m_strProductSaleId);

						dvProductSale = sqlStatements.SelectSingleStatementWithOC(sqlAccessor, htInput);
					}

					// 該当データが有りの場合
					if (dvProductSale.Count != 0)
					{
						lProductSaleKbn.Text = ValueText.GetValueText(Constants.TABLE_PRODUCTSALE, Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN, dvProductSale[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN]);
						hfProductSaleKbn.Value = (string)dvProductSale[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN];
						foreach (ListItem li in rblProductSaleKbn.Items)
						{
							li.Selected = (li.Value == (string)dvProductSale[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN]);
						}
						lProductSaleId.Text = (string)dvProductSale[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID];
						tbProductSaleName.Text = (string)dvProductSale[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_NAME];
						tbClosedMarketPassword.Text = (string)dvProductSale[0][Constants.FIELD_PRODUCTSALE_CLOSEDMARKET_PASSWORD];
						ucDisplayPeriod.SetPeriodDate(
							(DateTime)dvProductSale[0][Constants.FIELD_PRODUCTSALE_DATE_BGN],
							(DateTime)dvProductSale[0][Constants.FIELD_PRODUCTSALE_DATE_END]);

						cbValidFlg.Checked = ((string)dvProductSale[0][Constants.FIELD_PRODUCTSALE_VALID_FLG] == Constants.FLG_PRODUCTSALE_VALID_FLG_VALID);
						lSaleOpened.Text = ValueText.GetValueText(Constants.TABLE_PRODUCTSALE, "sale_opened", (string)dvProductSale[0]["sale_opened"]);
						srgSaleOpened.Disabled = (string)dvProductSale[0]["sale_opened"] != "1";
						this.Url = Constants.URL_FRONT_PC_SECURE + "Form/Product/ProductVariationList.aspx?shop=" + this.LoginOperatorShopId + "&pslid=" + lProductSaleId.Text + "&pslkbn=" + hfProductSaleKbn.Value;
						lDateCreated.Text = DateTimeUtility.ToStringForManager(
							dvProductSale[0][Constants.FIELD_PRODUCTSALE_DATE_CREATED],
							DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
						lDateChanged.Text = DateTimeUtility.ToStringForManager(
							dvProductSale[0][Constants.FIELD_PRODUCTSALE_DATE_CHANGED],
							DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
						lLastChanged.Text = (string)dvProductSale[0][Constants.FIELD_PRODUCTSALE_LAST_CHANGED];

						// 価格一覧取得＆設定
						DataView dvProductSalePrice = null;
						using (SqlAccessor sqlAccessor = new SqlAccessor())
						using (SqlStatement sqlStatements = new SqlStatement("ProductSale", "GetProductSalePrices"))
						{
							Hashtable htInput = new Hashtable();
							htInput.Add(Constants.FIELD_PRODUCTSALE_SHOP_ID, this.LoginOperatorShopId);
							htInput.Add(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID, m_strProductSaleId);

							dvProductSalePrice = sqlStatements.SelectSingleStatementWithOC(sqlAccessor, htInput);
						}
						foreach (DataRowView drv in dvProductSalePrice)
						{
							if (drv[Constants.FIELD_PRODUCT_NAME] != DBNull.Value) // 削除考慮
							{
								m_lProductSalePrices.Add(
									new ProductSalePrice(
										(string)drv[Constants.FIELD_PRODUCTSALEPRICE_PRODUCT_ID],
										((string)drv[Constants.FIELD_PRODUCTSETITEM_VARIATION_ID]).Substring(((string)drv[Constants.FIELD_PRODUCTSALEPRICE_PRODUCT_ID]).Length),
										CreateProductAndVariationName(drv),
										StringUtility.ToEmpty(drv[Constants.FIELD_PRODUCTVARIATION_PRICE]),
										StringUtility.ToEmpty(drv[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE]),
										StringUtility.ToEmpty(drv[Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE]),
										StringUtility.ToEmpty(drv[Constants.FIELD_PRODUCTSALEPRICE_DISPLAY_ORDER])));
							}
						}
						if (m_lProductSalePrices.Count < Constants.PRODUCTSALE_REGISTER_UPPER_LIMIT)
						{
							PeoductListDataBind(m_lProductSalePrices);
						}
						else
						{
							trSaleDiscountRateInput.Visible = false;
							trProductSalePrice.Visible = false;
							trProductSalePriceHead.Visible = false;
							dProductButton.Visible = false;
							// マスタアップロードURL設定
							this.MasterUploadUrl = Constants.PATH_ROOT + Constants.PAGE_MANAGER_MASTERIMPORT_LIST;
							trMasterUploadMessage.Visible = true;
							this.IsOverThousandProducts = true;
							// マスタアップロード選択肢設定
							var param = new Hashtable
							{
								{ Constants.REQUEST_KEY_SELECTED_MASTER, Constants.TABLE_PRODUCTSALEPRICE }
							};
							this.Session[Constants.SESSION_KEY_PARAM] = param;
						}

						// CSVダウンロードリンク表示設定
						this.ShowsCSVDownloadLink = ((m_lProductSalePrices.Count > 0)
							&& (MenuUtility.HasAuthority(this.LoginOperatorMenu, this.RawUrl, Constants.KBN_MENU_FUNCTION_PRODUCTSALEPRICE_DL)));
					}
					// 該当データ無しの場合
					else
					{
						// エラーページへ
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}
					break;

				// エラーページへ
				default:

					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					break;
			}

			// 闇市パスワード入力項目は最後に判定
			trClosedMarketPassword.Visible = (rblProductSaleKbn.SelectedValue == Constants.FLG_PRODUCTSALE_PRODUCTSALE_KBN_CLOSEDMARKET);

			// ビューステート格納
			ViewState.Add(VIEWSRATE_KEY_PRODUCT_SALE_PRICES, m_lProductSalePrices);
			ViewState.Add(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_ID, m_strProductSaleId);
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, m_strActionStatus);
		}
		else
		{
			// ビューステート復元
			m_lProductSalePrices = (List<ProductSalePrice>)ViewState[VIEWSRATE_KEY_PRODUCT_SALE_PRICES];
			m_strProductSaleId = (string)ViewState[Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_ID];
			m_strActionStatus = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];
		}

		//------------------------------------------------------
		// 登録/更新完了メッセージ表示制御
		//------------------------------------------------------
		if (Session[SESSION_KEY_DISP_COMP_MESSAGE] != null)
		{
			Session[SESSION_KEY_DISP_COMP_MESSAGE] = null;

			divComp.Visible = true;
		}
		else
		{
			divComp.Visible = false;
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string m_strActionStatus)
	{
		// 表示非表示制御
		switch (m_strActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:

				// 項目表示制御
				trRegistTitle.Visible = true;
				trEditTitle.Visible = false;
				divComp.Visible = false;

				trDispProductSaleKbn.Visible = false;
				trDispProductSaleId.Visible = false;
				trSelectProductSaleKbn.Visible = true;
				tbdyDispUpdateInfo.Visible = false;

				// ボタン表示制御
				btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = false;
				btnDeleteTop.Visible = btnDeleteBottom.Visible = false;
				btnInsertTop.Visible = btnInsertBottom.Visible = true;
				btnUpdateTop.Visible = btnUpdateBottom.Visible = false;
				break;

			case Constants.ACTION_STATUS_UPDATE:
			case Constants.ACTION_STATUS_COMPLETE:

				// 項目表示制御
				trRegistTitle.Visible = false;
				trEditTitle.Visible = true;
				divComp.Visible = false;

				trDispProductSaleKbn.Visible = true;
				trDispProductSaleId.Visible = true;
				trSelectProductSaleKbn.Visible = false;
				tbdyDispUpdateInfo.Visible = true;

				// ボタン表示制御
				btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = true;
				btnDeleteTop.Visible = btnDeleteBottom.Visible = true;
				btnInsertTop.Visible = btnInsertBottom.Visible = false;
				btnUpdateTop.Visible = btnUpdateBottom.Visible = true;
				break;
		}

		// 商品セール区分
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_PRODUCTSALE, Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN))
		{
			rblProductSaleKbn.Items.Add(li);
		}
		rblProductSaleKbn.Items[0].Selected = true;

		// 開始日時・終了日時の初期化
		ucDisplayPeriod.SetPeriodDateToday();

		// 有効フラグ
		cbValidFlg.Checked = true;
	}

	/// <summary>
	/// 商品セール区分変更（闇市パスワード表示制御）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblProductSaleKbn_SelectedIndexChanged(object sender, EventArgs e)
	{
		trClosedMarketPassword.Visible = (rblProductSaleKbn.SelectedValue == Constants.FLG_PRODUCTSALE_PRODUCTSALE_KBN_CLOSEDMARKET);
	}

	/// <summary>
	/// 詳細URL作成
	/// </summary>
	/// <param name="strProductSaleId"></param>
	/// <returns></returns>
	private string CreateDetailUrl(string strProductSaleId)
	{
		StringBuilder sbResultUrl = new StringBuilder();
		sbResultUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTSALE_REGISTER);
		sbResultUrl.Append("?").Append(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_ID).Append("=").Append(HttpUtility.UrlEncode(strProductSaleId));
		sbResultUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPDATE);

		return sbResultUrl.ToString(); ;
	}

	/// <summary>
	/// 一覧URL作成
	/// </summary>
	/// <returns></returns>
	private string CreateListUrl()
	{
		Hashtable htParam = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTSALE_INFO];

		StringBuilder sbResultUrl = new StringBuilder();
		sbResultUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTSALE_LIST);
		sbResultUrl.Append("?").Append(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_INFO).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_INFO]));
		sbResultUrl.Append("&").Append(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_OPENED).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_OPENED]));
		sbResultUrl.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_SORT_KBN]));
		sbResultUrl.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(HttpUtility.UrlEncode(htParam[Constants.REQUEST_KEY_PAGE_NO].ToString()));

		return sbResultUrl.ToString(); ;
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		string strErrorMessages = CheckInputItems(hfProductSaleKbn.Value, m_strProductSaleId);

		if (strErrorMessages.Length != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// セール情報＆価格情報更新
		//------------------------------------------------------
		// データ格納
		Hashtable htInputSaleInformations = GetProductSaleInformationInputData(rblProductSaleKbn.SelectedValue, m_strProductSaleId);
		List<Hashtable> lhtInputSalePrices = GetProductSalePriceInformationInputData();
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			try
			{
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				using (SqlStatement sqlStatement = new SqlStatement("ProductSale", "UpdateProductSale"))
				{
					sqlStatement.ExecStatement(sqlAccessor, htInputSaleInformations);
				}

				if (this.IsOverThousandProducts == false)
				{
					using (SqlStatement sqlStatement = new SqlStatement("ProductSale", "DeleteProductSalePrice"))
					{
						sqlStatement.ExecStatement(sqlAccessor, htInputSaleInformations);
					}

					using (SqlStatement sqlStatement = new SqlStatement("ProductSale", "InsertProductSalePrice"))
					{
						foreach (Hashtable ht in lhtInputSalePrices)
						{
							ht[Constants.FIELD_PRODUCTSALEPRICE_SHOP_ID] = htInputSaleInformations[Constants.FIELD_PRODUCTSALEPRICE_SHOP_ID];
							ht[Constants.FIELD_PRODUCTSALEPRICE_PRODUCTSALE_ID] = htInputSaleInformations[Constants.FIELD_PRODUCTSALEPRICE_PRODUCTSALE_ID];
							ht[Constants.FIELD_PRODUCTSALEPRICE_LAST_CHANGED] = htInputSaleInformations[Constants.FIELD_PRODUCTSALEPRICE_LAST_CHANGED];

							sqlStatement.ExecStatement(sqlAccessor, ht);
						}
					}
				}

				// 商品セール価格テンポラリデータ登録
				new ProductSaleService().RegisterProductSalePriceTmp(
					(string)htInputSaleInformations[Constants.FIELD_PRODUCTSALEPRICE_SHOP_ID],
					(string)htInputSaleInformations[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID],
					this.LoginOperatorName,
					sqlAccessor);

				sqlAccessor.CommitTransaction();
			}
			catch (Exception)
			{
				sqlAccessor.RollbackTransaction();
				throw;
			}
		}

		//------------------------------------------------------
		// 完了メッセージ表示準備＆画面遷移
		//------------------------------------------------------
		Session[SESSION_KEY_DISP_COMP_MESSAGE] = "1";

		Response.Redirect(CreateDetailUrl(m_strProductSaleId));
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		string strErrorMessages = CheckInputItems(rblProductSaleKbn.SelectedValue, "");
		if (strErrorMessages.Length != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// セール情報＆価格情報登録（発行されたIDも取得）
		//------------------------------------------------------
		// データ格納
		Hashtable htInputSaleInformations = GetProductSaleInformationInputData(rblProductSaleKbn.SelectedValue, m_strProductSaleId);
		List<Hashtable> lhtInputSalePrices = GetProductSalePriceInformationInputData();
		string productSaleId = NumberingUtility.CreateNewNumber(Constants.CONST_DEFAULT_SHOP_ID, Constants.NUMBER_KEY_PRODUCTSALE_ID).ToString().PadLeft(Constants.CONST_PRODUCTSALE_ID_LENGTH, '0');
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			try
			{
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				htInputSaleInformations[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID] = productSaleId;
				using (SqlStatement sqlStatement = new SqlStatement("ProductSale", "InsertProductSale"))
				{
					sqlStatement.SelectStatement(sqlAccessor, htInputSaleInformations);
				}

				using (SqlStatement sqlStatement = new SqlStatement("ProductSale", "InsertProductSalePrice"))
				{
					foreach (Hashtable ht in lhtInputSalePrices)
					{
						ht[Constants.FIELD_PRODUCTSALEPRICE_SHOP_ID] = htInputSaleInformations[Constants.FIELD_PRODUCTSALEPRICE_SHOP_ID];
						ht[Constants.FIELD_PRODUCTSALEPRICE_PRODUCTSALE_ID] = htInputSaleInformations[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID];
						ht[Constants.FIELD_PRODUCTSALEPRICE_LAST_CHANGED] = htInputSaleInformations[Constants.FIELD_PRODUCTSALEPRICE_LAST_CHANGED];

						sqlStatement.ExecStatement(sqlAccessor, ht);
					}
				}

				// 商品セール価格テンポラリデータ登録
				new ProductSaleService().RegisterProductSalePriceTmp(
					(string)htInputSaleInformations[Constants.FIELD_PRODUCTSALEPRICE_SHOP_ID],
					(string)htInputSaleInformations[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID],
					this.LoginOperatorName,
					sqlAccessor);

				sqlAccessor.CommitTransaction();
			}
			catch (Exception)
			{
				sqlAccessor.RollbackTransaction();
				throw;
			}
		}

		//------------------------------------------------------
		// 完了メッセージ表示準備＆画面遷移
		//------------------------------------------------------
		Session[SESSION_KEY_DISP_COMP_MESSAGE] = "1";

		Response.Redirect(CreateDetailUrl(productSaleId));
	}

	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		StringBuilder sbResultUrl = new StringBuilder();
		sbResultUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTSALE_REGISTER);
		sbResultUrl.Append("?").Append(Constants.REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_ID).Append("=").Append(HttpUtility.UrlEncode(m_strProductSaleId));
		sbResultUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_COPY_INSERT);

		Response.Redirect(sbResultUrl.ToString());
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		// 削除
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductSale", "DeleteProductSale"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCTSALE_SHOP_ID, this.LoginOperatorShopId);
			htInput.Add(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID, m_strProductSaleId);

			sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
		}

		// 商品セール価格テンポラリデータ削除
		new ProductSaleService().DeleteProductSalePriceTmpBySaleId(this.LoginOperatorShopId, m_strProductSaleId);

		// 一覧へリダイレクト
		Response.Redirect(CreateListUrl());
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnToList_Click(object sender, EventArgs e)
	{
		// 一覧へリダイレクト
		Response.Redirect(CreateListUrl());
	}

	/// <summary>
	/// 商品追加イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbAddProduct_Click(object sender, EventArgs e)
	{
		// すでにセットされていたら抜ける
		foreach (ProductSalePrice sp in m_lProductSalePrices)
		{
			if ((sp.ProductId == hfAddProductId.Value) && (sp.VId == hfAddVId.Value))
			{
				return;
			}
		}

		// 入力データ更新
		foreach (RepeaterItem ri in rProductList.Items)
		{
			ProductSalePrice psp = (ProductSalePrice)m_lProductSalePrices[ri.ItemIndex];
			psp.SalePrice = ((TextBox)ri.FindControl("tbProductSalePrice")).Text;
			psp.DisplayOrder = ((TextBox)ri.FindControl("tbDisplayOrder")).Text;
		}

		// 追加（オブジェクト操作なのでビューステートの更新は不要）
		m_lProductSalePrices.Add(new ProductSalePrice(hfAddProductId.Value, hfAddVId.Value, hfAddProductName.Value, hfAddProductPrice.Value, hfAddProductSpecialPrice.Value, "", "1"));

		if (m_lProductSalePrices.Count >= Constants.PRODUCTSALE_REGISTER_UPPER_LIMIT)
		{
			var errorMessages = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_UPPER_LIMIT)
				.Replace("@@ 1 @@", Constants.PRODUCTSALE_REGISTER_UPPER_LIMIT.ToString());
			this.Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			this.Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 商品リストデータバインド
		PeoductListDataBind(m_lProductSalePrices);
	}

	/// <summary>
	/// アイテム削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteItem_Click(object sender, EventArgs e)
	{
		// 新しいリスト作成
		List<ProductSalePrice> lNewProductSalePrices = new List<ProductSalePrice>();
		foreach (RepeaterItem ri in rProductList.Items)
		{
			ProductSalePrice psp = m_lProductSalePrices[ri.ItemIndex];
			if (((CheckBox)ri.FindControl("cbProductSelect")).Checked == false)
			{
				lNewProductSalePrices.Add(psp);
			}
		}

		if (m_lProductSalePrices.Count != lNewProductSalePrices.Count)
		{
			// 商品リストデータバインド＆値セット
			PeoductListDataBind(lNewProductSalePrices);

			// ビューステート再設定
			ViewState.Add(VIEWSRATE_KEY_PRODUCT_SALE_PRICES, lNewProductSalePrices);
		}
	}

	/// <summary>
	/// セール情報入力データ取得
	/// </summary>
	/// <param name="strProductSaleKbn"></param>
	/// <param name="strProductSaleId"></param>
	/// <returns></returns>
	private Hashtable GetProductSaleInformationInputData(string strProductSaleKbn, string strProductSaleId)
	{
		Hashtable htInput = new Hashtable();
		htInput.Add(Constants.FIELD_PRODUCTSALE_SHOP_ID, this.LoginOperatorShopId);
		htInput.Add(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN, strProductSaleKbn);
		htInput.Add(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID, strProductSaleId);
		htInput.Add(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_NAME, tbProductSaleName.Text);
		if (strProductSaleKbn == Constants.FLG_PRODUCTSALE_PRODUCTSALE_KBN_CLOSEDMARKET)
		{
			htInput.Add(Constants.FIELD_PRODUCTSALE_CLOSEDMARKET_PASSWORD + "_chk", tbClosedMarketPassword.Text);
			htInput.Add(Constants.FIELD_PRODUCTSALE_CLOSEDMARKET_PASSWORD, tbClosedMarketPassword.Text);
		}
		else
		{
			htInput.Add(Constants.FIELD_PRODUCTSALE_CLOSEDMARKET_PASSWORD + "_chk", "dammy");
			htInput.Add(Constants.FIELD_PRODUCTSALE_CLOSEDMARKET_PASSWORD, "");
		}

		htInput.Add(
			Constants.FIELD_PRODUCTSALE_DATE_BGN,
			ucDisplayPeriod.StartDateTimeString);
		htInput.Add(
			Constants.FIELD_PRODUCTSALE_DATE_END,
			ucDisplayPeriod.EndDateTimeString);
		htInput.Add(Constants.FIELD_PRODUCTSALE_VALID_FLG,
			(cbValidFlg.Checked ? Constants.FLG_PRODUCTSALE_VALID_FLG_VALID : Constants.FLG_PRODUCTSALE_VALID_FLG_INVALID));
		htInput.Add(Constants.FIELD_PRODUCTSALE_LAST_CHANGED, this.LoginOperatorName);

		return htInput;
	}

	/// <summary>
	/// セール価格情報入力データ取得
	/// </summary>
	/// <param name="strProductSaleKbn"></param>
	/// <param name="strProductSaleId"></param>
	/// <returns></returns>
	private List<Hashtable> GetProductSalePriceInformationInputData()
	{
		List<Hashtable> lhtResult = new List<Hashtable>();
		if (this.IsOverThousandProducts)
		{
			foreach (var ri in m_lProductSalePrices)
			{
				lhtResult.Add(ri.GetHashtable());
			}
		}
		else
		{
			foreach (RepeaterItem ri in rProductList.Items)
			{
				ProductSalePrice psp = (ProductSalePrice)m_lProductSalePrices[ri.ItemIndex];
				psp.SalePrice = ((TextBox)ri.FindControl("tbProductSalePrice")).Text;
				psp.DisplayOrder = ((TextBox)ri.FindControl("tbDisplayOrder")).Text;

				lhtResult.Add(psp.GetHashtable());
			}
		}

		return lhtResult;
	}

	/// <summary>
	/// 商品一覧データバインド
	/// </summary>
	/// <param name="lProductSetItems"></param>
	private void PeoductListDataBind(List<ProductSalePrice> lProductSalePrice)
	{
		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		rProductList.DataSource = lProductSalePrice;
		rProductList.DataBind();

		//------------------------------------------------------
		// 各値セット
		//------------------------------------------------------
		foreach (RepeaterItem ri in rProductList.Items)
		{
			((TextBox)ri.FindControl("tbProductSalePrice")).Text = lProductSalePrice[ri.ItemIndex].SalePrice.ToPriceString();
			((TextBox)ri.FindControl("tbDisplayOrder")).Text = lProductSalePrice[ri.ItemIndex].DisplayOrder;
		}

		if (lProductSalePrice.Count == 0)
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}
		else
		{
			trListError.Visible = false;
		}
	}

	/// <summary>
	/// ProductSaleModify.xmlに設定されているColumnの属性名と名称を取得する
	/// </summary>
	/// <returns>ColumnIDとColumnNameのペアをDictionaryに格納して返す</returns>
	protected Dictionary<string, string> GetThisPageColumnNameListFromValidator()
	{
		Dictionary<string, string> dicResult = new Dictionary<string, string>();
		XmlDocument xd = new XmlDocument();
		xd.Load(AppDomain.CurrentDomain.BaseDirectory + "Xml/Validator/ProductSaleModify.xml");
		foreach (XmlNode xn in xd.SelectNodes("/ProductSaleModify/Column"))
		{
			dicResult.Add(xn.Attributes["name"].InnerText, xn.SelectSingleNode("name").InnerText);
		}

		return dicResult;
	}

	/// <summary>
	/// 入力項目チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	protected string CheckInputItems(string strProductSaleKbn, string strProductSaleId)
	{
		StringBuilder sbErrorMessages = new StringBuilder();

		// セール情報 入力チェック
		sbErrorMessages.Append(CheckSaleInformationDatas(strProductSaleKbn, strProductSaleId));
		
		// Check date time
		if (string.IsNullOrEmpty(ucDisplayPeriod.StartDateTimeString) == false)
		{
			if (Validator.CheckDateRange(
				DateTime.Parse(ucDisplayPeriod.HfStartDate.Value),
				DateTime.Parse(ucDisplayPeriod.HfEndDate.Value)) == false)
			{
				sbErrorMessages.Append(
					WebMessages.GetMessages(WebMessages.INPUTCHECK_DATERANGE)).Replace("@@ 1 @@", " 表示期間 ");
			}
		}

		if (this.IsOverThousandProducts == false)
		{
			// セール価格情報 入力チェック
			sbErrorMessages.Append(CheckProductSalePriceInformation());
		}

		return sbErrorMessages.ToString();
	}

	/// <summary>
	/// セール情報 入力チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	protected string CheckSaleInformationDatas(string strProductSaleKbn, string strProductSaleId)
	{
		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		// 開始・終了日付が正しい値で無い場合、Validatorの開催期間の重複チェックSQLクエリで
		// datetime型への変換に失敗し、日付の算術オーバーフローが発生する為、
		// strErrorMessageに開始・終了日付が含まれていない場合にのみ、
		// 開催期間の重複チェックを行う。
		Hashtable htInput = GetProductSaleInformationInputData(strProductSaleKbn, strProductSaleId);

		string strErrorMessages = Validator.Validate("ProductSaleModify", htInput);

		// ページの項目名を取得する
		Dictionary<string, string> dicColumnNames = GetThisPageColumnNameListFromValidator();

		// 開始日、終了日の双方にエラーが無かった場合、再度Validateを実行して開催期間の重複チェックを行う。
		if (strErrorMessages.Contains(dicColumnNames[Constants.FIELD_PRODUCTSALE_DATE_BGN]) == false
		 && strErrorMessages.Contains(dicColumnNames[Constants.FIELD_PRODUCTSALE_DATE_END]) == false)
		{
			htInput.Add(Constants.FIELD_PRODUCTSALE_HOLDING_PERIOD, "dammy");
			strErrorMessages = Validator.Validate("ProductSaleModify", htInput);
			if (DateTime.Parse(ucDisplayPeriod.HfStartDate.Value)
				.CompareTo(DateTime.Parse(ucDisplayPeriod.HfEndDate.Value)) == 1)
			{
				strErrorMessages += (WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SALE_PRODUCT_DATE_ERROR));
			}
		}
		return strErrorMessages;
	}

	/// <summary>
	/// セール価格情報 入力チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	protected string CheckProductSalePriceInformation()
	{
		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		if (rProductList.Items.Count == 0) return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SALE_PRODUCT_NOT_EXIST);
		
		List<ProductSalePrice> lProductSalePrices = new List<ProductSalePrice>();
		StringBuilder sbErrorMessages = new StringBuilder();
		foreach (RepeaterItem ri in rProductList.Items)
		{
			ProductSalePrice psp = (ProductSalePrice)m_lProductSalePrices[ri.ItemIndex];
			psp.SalePrice = ((TextBox)ri.FindControl("tbProductSalePrice")).Text;
			psp.DisplayOrder = ((TextBox)ri.FindControl("tbDisplayOrder")).Text;

			lProductSalePrices.Add(psp);
		}

		foreach (ProductSalePrice psp in lProductSalePrices)
		{
			sbErrorMessages.Append(Validator.Validate("ProductSalePriceRegist", psp.GetHashtable()).Replace("@@ 1 @@", psp.Name));

			// 商品有効性チェック
			sbErrorMessages.Append(CheckValidProduct(Constants.CONST_DEFAULT_SHOP_ID, psp.ProductId));
		}

		return sbErrorMessages.ToString();
	}

	/// <summary>
	/// マスタデータ出力用の検索ハッシュテーブル生成
	/// </summary>
	/// <returns>検索ハッシュテーブル</returns>
	/// <remarks>マスタ出力ユーザコントロールのイベントに割り当てて使う</remarks>
	public Hashtable CreateSearchParams()
	{
		Hashtable inputParam = new Hashtable()
		{
			{Constants.FIELD_PRODUCTSALEPRICE_SHOP_ID, this.LoginOperatorShopId},
			{Constants.FIELD_PRODUCTSALEPRICE_PRODUCTSALE_ID, m_strProductSaleId}
		};

		return inputParam;
	}

	/// <summary>CSVダウンロードリンク表示可否</summary>
	protected bool ShowsCSVDownloadLink
	{
		get { return (ViewState["ShowsCSVDownloadLink "] != null) ? (bool)ViewState["ShowsCSVDownloadLink "] : false; }
		set { ViewState["ShowsCSVDownloadLink "] = value; }
	}
	/// <summary> URL </summary>
	protected string Url { get; set; }
	/// <summary> マスタアップロード用URL </summary>
	protected string MasterUploadUrl { get; set; }
	/// <summary>商品が1000件以上か</summary>
	protected bool IsOverThousandProducts
	{
		get { return (this.ViewState["IsOverThousandProducts"] != null) ? (bool)this.ViewState["IsOverThousandProducts"] : false; }
		set { this.ViewState["IsOverThousandProducts"] = value; }
	}

	///**************************************************************************************
	/// <summary>
	/// 商品セール価格クラス
	/// </summary>
	///**************************************************************************************
	[Serializable]
	public class ProductSalePrice
	{
		string m_strProductId = null;
		string m_strVId = null;
		string m_strName = null;
		string m_strPrice = null;
		string m_strSpecialPrice = null;
		string m_strSalePrice = null;
		string m_strDisplayOrder = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strProductId"></param>
		/// <param name="strVId"></param>
		/// <param name="strName"></param>
		/// <param name="strPrice"></param>
		/// <param name="strSpecialPrice"></param>
		/// <param name="strSalePrice"></param>
		/// <param name="strDisplayOrder"></param>
		public ProductSalePrice(
			string strProductId,
			string strVId,
			string strName,
			string strPrice,
			string strSpecialPrice,
			string strSalePrice,
			string strDisplayOrder
		)
		{
			m_strProductId = strProductId;
			m_strVId = strVId;
			m_strName = strName;
			m_strPrice = strPrice;
			m_strSpecialPrice = strSpecialPrice;
			m_strSalePrice = strSalePrice;
			m_strDisplayOrder = strDisplayOrder;
		}

		/// <summary>
		/// 登録用ハッシュテーブル取得
		/// </summary>
		/// <returns></returns>
		public Hashtable GetHashtable()
		{
			Hashtable htInputProduct = new Hashtable();
			htInputProduct.Add(Constants.FIELD_PRODUCTSALEPRICE_PRODUCT_ID, this.ProductId);
			htInputProduct.Add(Constants.FIELD_PRODUCTSALEPRICE_VARIATION_ID, this.ProductId + this.VId);
			htInputProduct.Add(Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE, this.SalePrice);
			htInputProduct.Add(Constants.FIELD_PRODUCTSALEPRICE_DISPLAY_ORDER, this.DisplayOrder);
			htInputProduct.Add("name", this.Name);	// エラーメッセージ表示用に使用

			return htInputProduct;
		}

		/// <summary>プロパティ：商品ID</summary>
		public string ProductId
		{
			get { return m_strProductId; }
		}
		/// <summary>プロパティ：バリエーションID</summary>
		public string VId
		{
			get { return m_strVId; }
		}
		/// <summary>プロパティ：商品名</summary>
		public string Name
		{
			get { return m_strName; }
		}
		/// <summary>プロパティ：商品価格</summary>
		public string Price
		{
			get { return m_strPrice; }
		}
		/// <summary>プロパティ：特別価格</summary>
		public string SpecialPrice
		{
			get { return m_strSpecialPrice; }
		}
		/// <summary>プロパティ：商品セール価格</summary>
		public string SalePrice
		{
			get { return m_strSalePrice; }
			set { m_strSalePrice = value; }
		}
		/// <summary>プロパティ：表示順</summary>
		public string DisplayOrder
		{
			get { return m_strDisplayOrder; }
			set { m_strDisplayOrder = value; }
		}
	}
}
