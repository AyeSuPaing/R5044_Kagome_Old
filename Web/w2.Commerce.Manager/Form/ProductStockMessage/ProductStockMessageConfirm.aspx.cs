/*
=========================================================================================================
  Module      : 商品在庫文言情報確認ページ処理(ProductStockMessageConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.ProductStockMessage;

public partial class Form_ProductStockMessage_ProductStockMessageConfirm : BasePage
{
	// 定数
	protected const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_NO = "stock_no";
	protected const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE = "stock_message";
	protected const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE = "stock_message_mobile";
	protected const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM = "stock_datum";
	public const int CONST_USER_PRODUCTSTOCKMESSAGE_STOCK_DATUM = 7;

	protected Hashtable m_htParam = new Hashtable();					// 商品在庫文言情報データバインド用
	protected ArrayList m_alParam = new ArrayList();					// 商品在庫文言情報データバインド用(在庫基準)

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
			string strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, strActionStatus);

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(strActionStatus);

			//------------------------------------------------------
			// 画面設定処理
			//------------------------------------------------------
			// 登録・コピー登録・更新画面確認？
			if (strActionStatus == Constants.ACTION_STATUS_INSERT
				|| strActionStatus == Constants.ACTION_STATUS_COPY_INSERT
				|| strActionStatus == Constants.ACTION_STATUS_UPDATE
				|| strActionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT)
			{
				//------------------------------------------------------
				// 処理区分チェック
				//------------------------------------------------------
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

				// 商品在庫文言情報取得
				m_htParam = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_INFO];
				m_alParam = (ArrayList)m_htParam[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_DATUM_INFO];

				if (m_htParam.Contains(Constants.SETTING_TABLE_NAME))
				{
					this.ProductStockMessageSettingTableName = (string)m_htParam[Constants.SETTING_TABLE_NAME];
				}
			}
			// 詳細表示？
			else if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				// 商品在庫文言ID取得
				string strStockMessageId = Request[Constants.REQUEST_KEY_STOCK_MESSAGE_ID];
				DataView dv = null;

				if (Constants.GLOBAL_OPTION_ENABLE == false)
				{
					dv = GetProductStockMessageDataView(this.LoginOperatorShopId, strStockMessageId);
				}
				else
				{
					var requestLanguageCode = (string)Request[Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE] ?? string.Empty;
					var requestLanguageLocaleId = (string)Request[Constants.REQUEST_KEY_GLOBAL_LANGUAGE_LOCALE_ID] ?? string.Empty;

					if ((string.IsNullOrEmpty(requestLanguageCode) == false)
						&& (string.IsNullOrEmpty(requestLanguageLocaleId) == false))
					{
						dv = new ProductStockMessageService().GetProductStockMessageDataView(
							this.LoginOperatorShopId,
							strStockMessageId,
							requestLanguageCode,
							requestLanguageLocaleId);
					}
					else
					{
						dv = GetProductStockMessageDataView(this.LoginOperatorShopId, strStockMessageId);
					}
					this.ProductStockMessageSettingTableName = Constants.TABLE_PRODUCTSTOCKMESSAGE;

					if (dv.Count == 0)
					{
						dv = new ProductStockMessageService().GetProductStockMessageTranslationSettingDataView(
							this.LoginOperatorShopId,
							strStockMessageId,
							requestLanguageCode,
							requestLanguageLocaleId);
						this.ProductStockMessageSettingTableName = Constants.TABLE_PRODUCTSTOCKMESSAGEGLOBAL;
					}
				}

				// 該当データが有りの場合
				if (dv.Count != 0)
				{
					m_htParam = GetRowInfoHashtable(dv, 0);
					m_alParam = GetStockDatum(m_htParam);
					m_htParam.Add(Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_DATUM_INFO, m_alParam);

					if (Constants.GLOBAL_OPTION_ENABLE)
					{
						InitializeLanguageCodeList();
						m_htParam.Add(Constants.SETTING_TABLE_NAME, this.ProductStockMessageSettingTableName);
					}
				}
				// 該当データ無しの場合
				else
				{
					// エラーページへ
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
			}

			// 商品在庫文言情報をビューステートに格納しておく
			ViewState.Add(Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_INFO, m_htParam);

			DataBind();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		// 新規・コピー新規？
		if (strActionStatus == Constants.ACTION_STATUS_INSERT
			|| strActionStatus == Constants.ACTION_STATUS_COPY_INSERT
			|| strActionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT)
		{
			btnGoBackTop.Visible = true;
			btnGoBackBottom.Visible = true;
			btnInsertTop.Visible = true;
			btnInsertBottom.Visible = true;
			trConfirm.Visible = true;
		}
		// 更新？
		else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			btnGoBackTop.Visible = true;
			btnGoBackBottom.Visible = true;
			btnUpdateTop.Visible = true;
			btnUpdateBottom.Visible = true;
			trConfirm.Visible = true;
		}
		// 詳細
		else if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			btnBackToListTop.Visible = true;
			btnBackToListBottom.Visible = true;
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

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				ddlLanguageCode.Visible = true;
				btnInsertGlobalTop.Visible = true;
				btnInsertGlobalBottom.Visible = true;
			}
		}
	}

	/// <summary>
	/// 商品在庫文言情報取得
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <returns>店舗配送料情報データビュー</returns>
	private DataView GetProductStockMessageDataView(string strShopId, string strStockMessageId)
	{
		// 変数宣言
		DataView dvResult = null;

		// ステートメントからカテゴリデータ取得
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductStockMessage", "GetProductStockMessage"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_SHOP_ID, strShopId);	// 店舗ID
			htInput.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID, strStockMessageId);	// 店舗ID

			// SQL発行
			dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
		return dvResult;
	}

	/// <summary>
	/// 在庫基準情報取得
	/// </summary>
	/// <param name="htStock">商品在庫文言情報取得</param>
	/// <returns></returns>
	private ArrayList GetStockDatum(Hashtable htStock)
	{
		// 変数宣言
		ArrayList alResult = new ArrayList();
		string strNo = null;

		// 在庫基準1～xx
		for (int iLoop = 0; iLoop < CONST_USER_PRODUCTSTOCKMESSAGE_STOCK_DATUM; iLoop++)
		{
			strNo = ((int)(iLoop + 1)).ToString();

			// 在庫基準xxが登録されている場合
			if (StringUtility.ToEmpty(htStock[FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM + strNo]) != "")
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_NO + strNo, strNo);
				htInput.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM + strNo, htStock[FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM + strNo]);
				htInput.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE + strNo, htStock[FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE + strNo]);
				htInput.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE + strNo, htStock[FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE + strNo]);
				alResult.Add(htInput);
			}
		}

		return alResult;
	}

	/// <summary>
	/// 行データハッシュテーブル取得
	/// </summary>
	/// <param name="dv">データビュー</param>
	/// <param name="iRow">行番号</param>
	/// <returns>行データ</returns>
	private Hashtable GetRowInfoHashtable(DataView dv, int iRow)
	{
		Hashtable htResult = new Hashtable();

		// データが存在する場合
		if (dv.Count != 0)
		{
			DataRow dr = dv[iRow].Row;	// 指定行データ取得

			// Hashtabe格納
			foreach (DataColumn dc in dr.Table.Columns)
			{
				htResult.Add(dc.ColumnName, dr[dc.ColumnName]);
			}
		}

		return htResult;
	}

	#region #btnBack_Click 一覧へ戻るボタンクリック
	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBackToListTop_Click(object sender, System.EventArgs e)
	{
		// 在庫文言設定一覧へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCKMESSAGE_LIST);
	}
	#endregion

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEditTop_Click(object sender, EventArgs e)
	{
		// 商品在庫文言情報をそのままセッションへセット
		Session[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_INFO] = ViewState[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_INFO];

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCKMESSAGE_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);
	}

	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsertTop_Click(object sender, EventArgs e)
	{
		// 商品在庫文言情報をそのままセッションへセット
		Session[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_INFO] = ViewState[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_INFO];

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT;

		// 登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCKMESSAGE_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COPY_INSERT);
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteTop_Click(object sender, EventArgs e)
	{
		// 変数宣言
		Hashtable htInput = (Hashtable)ViewState[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_INFO];

		//------------------------------------------------------
		// 商品在庫文言情報削除
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			using (SqlStatement sqlStatement = new SqlStatement("ProductStockMessage", "DeleteProductStockMessage"))
			{
				int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}
		}

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			var productStockMessageId = (string)htInput[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID];
			new ProductStockMessageService().DeleteGlobalSetting(this.LoginOperatorShopId, productStockMessageId);
		}

		// 商品在庫文言情報一覧ページへ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCKMESSAGE_LIST);
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertTop_Click(object sender, EventArgs e)
	{
		// 変数宣言
		Hashtable htInput = (Hashtable)ViewState[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_INFO];
		var actionStatus = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];

		//------------------------------------------------------
		// 商品在庫文言情報登録
		//------------------------------------------------------
		if ((actionStatus == Constants.ACTION_STATUS_INSERT) || (actionStatus == Constants.ACTION_STATUS_COPY_INSERT))
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				using (SqlStatement sqlStatement = new SqlStatement("ProductStockMessage", "InsertProductStockMessage"))
				{
					int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
				}
			}
		}
		else
		{
			var input = new ProductStockMessageInput();
			input.DataSource = htInput;
			new ProductStockMessageService().InsertGlobalSetting(input.CreateGlobalModel());
		}

		// 商品在庫文言情報一覧へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCKMESSAGE_LIST);
	}

	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateTop_Click(object sender, EventArgs e)
	{
		// 変数宣言
		Hashtable htInput = (Hashtable)ViewState[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_INFO];

		//------------------------------------------------------
		// 商品在庫文言情報更新
		//------------------------------------------------------
		if ((Constants.GLOBAL_OPTION_ENABLE == false) || (this.ProductStockMessageSettingTableName == Constants.TABLE_PRODUCTSTOCKMESSAGE))
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				using (SqlStatement sqlStatement = new SqlStatement("ProductStockMessage", "UpdateProductStockMessage"))
				{
					int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
				}
			}
		}
		else
		{
			var input = new ProductStockMessageInput();
			input.DataSource = htInput;
			new ProductStockMessageService().UpdateGlobalSetting(input.CreateGlobalModel());
		}


		// 商品在庫文言情報一覧へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCKMESSAGE_LIST);
	}

	/// <summary>
	/// 他言語コードで登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertGlobal_Click(object sender, EventArgs e)
	{
		// 商品在庫文言情報をそのままセッションへセット
		Session[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_INFO] = ViewState[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_INFO];

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT;

		// グローバル設定登録画面へ
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCKMESSAGE_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT);
		Response.Redirect(urlCreator.CreateUrl());
	}

	/// <summary>
	/// 言語コードリスト初期化
	/// </summary>
	private void InitializeLanguageCodeList()
	{
		var settings = new ProductStockMessageService().GetProductStockMessageLanguageCodeSettings(
				this.LoginOperatorShopId,
				(string)m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID]);

		// 「言語コード/言語ロケールID」の形式でドロップダウンリストを生成する
		var wddlLanguageCode = GetWrappedControl<WrappedDropDownList>("ddlLanguageCode");
		wddlLanguageCode.AddItems(settings.OrderBy(setting => setting.LanguageCode)
			.Select(setting =>
				new ListItem(
					(string.IsNullOrEmpty(setting.LanguageCode) == false)
						? setting.LanguageCode + "(" + setting.LanguageLocaleId + ")"
						: ValueText.GetValueText(Constants.TABLE_PRODUCTSTOCKMESSAGE, Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_CODE, string.Empty)	// 「指定しない」を表示
					, string.Format("{0}/{1}", setting.LanguageCode, setting.LanguageLocaleId))).ToArray());

		var languageCode = (string)m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_CODE] ?? string.Empty;
		var languageLocaleId = (string)m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_LOCALE_ID] ?? string.Empty;
		wddlLanguageCode.SelectItemByValue(string.Format("{0}/{1}", languageCode, languageLocaleId));
	}

	/// <summary>
	/// 言語コードドロップダウンリストチェンジイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlLanguageCode_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var selectedItem = ((DropDownList)sender).SelectedItem;
		var selectedValueSplit = selectedItem.Value.Split('/');

		var languageCode = selectedValueSplit[0];
		var languageLocaleId = selectedValueSplit[1];

		var productStockMessageId =
			(string)((Hashtable)ViewState[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_INFO])[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID];
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCKMESSAGE_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_STOCK_MESSAGE_ID, productStockMessageId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL);

		if ((string.IsNullOrEmpty(languageCode) == false) && (string.IsNullOrEmpty(languageLocaleId) == false))
		{
			urlCreator.AddParam(Constants.REQUEST_KEY_GLOBAL_LANGUAGE_CODE, languageCode)
				.AddParam(Constants.REQUEST_KEY_GLOBAL_LANGUAGE_LOCALE_ID, languageLocaleId);
		}

		Response.Redirect(urlCreator.CreateUrl());
	}

	/// <summary>
	/// 在庫文言があるか
	/// </summary>
	/// <param name="message">在庫文言</param>
	/// <returns>在庫文言</returns>
	protected string ToProductStockMessage(string message)
	{
		return string.IsNullOrEmpty(message) ? "-" : message;
	}

	/// <summary>商品在庫文言設定設定テーブル名(w2_ProductStockMessage/w2_ProductStockMessageGlobal)</summary>
	private string ProductStockMessageSettingTableName
	{
		get { return (string)ViewState[Constants.SETTING_TABLE_NAME]; }
		set { ViewState[Constants.SETTING_TABLE_NAME] = value; }
	}
}
