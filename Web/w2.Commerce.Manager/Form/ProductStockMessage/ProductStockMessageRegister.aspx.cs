/*
=========================================================================================================
  Module      : 商品在庫文言情報登録ページ処理(ProductStockMessageRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.ProductStockMessage;

public partial class Form_ProductStockMessage_ProductStockMessageRegister : BasePage
{
	// 定数
	protected const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_NO = "stock_no";
	protected const string FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE = "stock_message";
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

			var param = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_INFO];
			if (param != null && param.Contains(Constants.SETTING_TABLE_NAME))
			{
				this.ProductStockMessageSettingTableName = (string)param[Constants.SETTING_TABLE_NAME];
			}

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
				// 処理無し
			}
			// コピー新規・編集・グローバル設定登録？
			else if ((strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
				|| (strActionStatus == Constants.ACTION_STATUS_UPDATE)
				|| (strActionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT))
			{
				// セッションより決済種別情報取得
				m_htParam = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_INFO];							// 商品在庫文言情報
				m_alParam = (ArrayList)m_htParam[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_DATUM_INFO];					// 商品在庫文言情報(在庫基準)
				ViewState.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID, m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID]);
			}
			// それ以外の場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				InitializeLanguageCodeList(strActionStatus);
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
		this.IsEditable = true;

		// 新規登録？
		if (strActionStatus == Constants.ACTION_STATUS_INSERT)
		{
			trRegister.Visible = true;
			tdProductStockMessageIdEdit.Visible = true;
		}
		// コピー新規登録？
		else if (strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
		{
			trRegister.Visible = true;
			tdProductStockMessageIdEdit.Visible = true;
		}
		// 更新？
		else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			trEdit.Visible = true;
			tdProductStockMessageIdView.Visible = true;
			ddlLanguageCode.Enabled = false;

			if (Constants.GLOBAL_OPTION_ENABLE
				&& (this.ProductStockMessageSettingTableName == Constants.TABLE_PRODUCTSTOCKMESSAGEGLOBAL))
			{
				tbStockMessageName.Visible = false;
				lStockMessageName.Visible = true;
				this.IsEditable = false;
			}
		}
		else if (strActionStatus == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT)
		{
			trEdit.Visible = true;
			tdProductStockMessageIdView.Visible = true;
			tbStockMessageName.Visible = false;
			lStockMessageName.Visible = true;
			this.IsEditable = false;
		}
	}

	/// <summary>
	/// 言語コードリスト初期化
	/// </summary>
	/// <param name="actionStatus">実行アクションステータス</param>
	private void InitializeLanguageCodeList(string actionStatus)
	{
		var wddlLanguageCode = GetWrappedControl<WrappedDropDownList>("ddlLanguageCode");

		// グローバル設定登録時以外は、「指定しない」をドロップダウンリストに表示する
		if (actionStatus != Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT)
		{
			wddlLanguageCode.Items.Add(new ListItem(
				ValueText.GetValueText(Constants.TABLE_PRODUCTSTOCKMESSAGE, Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_CODE, string.Empty)
				, string.Format("{0}/{1}", string.Empty, string.Empty)));
		}
		wddlLanguageCode.Items.AddRange(
			Constants.GLOBAL_CONFIGS.GlobalSettings.Languages
				.Select(c => new ListItem(string.Format("{0}({1})", c.Code, c.LocaleId), string.Format("{0}/{1}", c.Code, c.LocaleId))).ToArray());

		if (actionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			var languageCode = (string)m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_CODE];
			var languageLocaleId = (string)m_htParam[Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_LOCALE_ID];

			wddlLanguageCode.SelectItemByValue(string.Format("{0}/{1}", languageCode, languageLocaleId));
		}
	}

	/// <summary>
	/// 商品在庫文言情報(在庫基準)
	/// </summary>
	/// <remarks>
	/// 入力された商品在庫文言情報(在庫基準)を在庫基準の大きいものから格納
	/// </remarks>
	private ArrayList GetStockDatumArrayList()
	{
		// 変数宣言
		ArrayList alResult = new ArrayList();
		ArrayList alTemp = new ArrayList();
		int iIndex = 0;
		string strNo = null;

		// 入力された商品在庫文言情報(在庫基準)を在庫基準の少ないものから格納する(在庫基準昇順)
		Repeater r = rStockMessage;
		for (int iLoop = 0; iLoop < r.Items.Count; iLoop++)
		{
			Hashtable htInput = new Hashtable();

			strNo = ((int)(iLoop + 1)).ToString();
			htInput.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_NO, strNo);
			htInput.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM, int.Parse(((TextBox)r.Items[iLoop].FindControl("tbStockDatum")).Text));
			htInput.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE, ((TextBox)r.Items[iLoop].FindControl("tbStockMessage")).Text);

			iIndex = 0;
			for (int iLoop2 = 0; iLoop2 < alTemp.Count; iLoop2++)
			{
				Hashtable htTemp = (Hashtable)alTemp[iLoop2];
				if ((int)htInput[FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM] > (int)htTemp[FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM])
				{
					break;
				}
				iIndex++;
			}
			alTemp.Insert(iIndex, htInput);
		}

		// 在庫基準1～xx、在庫在庫文言1～xx
		for (int iLoop = 0; iLoop < alTemp.Count; iLoop++)
		{
			Hashtable htTemp = (Hashtable)alTemp[iLoop];
			Hashtable htInput = new Hashtable();

			strNo = ((int)(iLoop + 1)).ToString();
			htInput.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_NO + strNo, strNo);
			htInput.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM + strNo, htTemp[FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM]);
			htInput.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE + strNo, htTemp[FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE]);
			alResult.Add(htInput);
		}

		return alResult;
	}

	/// <summary>
	/// 在庫基準1～xx、在庫在庫文言1～xxを設定
	/// </summary>
	/// <param name="htStock">商品在庫文言情報</param>
	/// <param name="alDatum">商品在庫文言情報(在庫基準)</param>
	private void SetStockDatum(Hashtable htStock, ArrayList alDatum)
	{
		// 変数宣言
		string strNo = null;

		// デフォルト設定
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM1, DBNull.Value);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE1, String.Empty);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM2, DBNull.Value);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE2, String.Empty);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM3, DBNull.Value);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE3, String.Empty);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM4, DBNull.Value);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE4, String.Empty);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM5, DBNull.Value);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE5, String.Empty);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM6, DBNull.Value);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE6, String.Empty);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM7, DBNull.Value);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE7, String.Empty);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE1, String.Empty);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE2, String.Empty);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE3, String.Empty);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE4, String.Empty);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE5, String.Empty);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE6, String.Empty);
		htStock.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_MOBILE7, String.Empty);

		// 在庫基準データが存在する場合        
		for (int iLoop = 0; iLoop < alDatum.Count; iLoop++)
		{
			Hashtable htTemp = (Hashtable)alDatum[iLoop];
			strNo = ((int)(iLoop + 1)).ToString();
			htStock[FIELD_PRODUCTSTOCKMESSAGE_STOCK_NO + strNo] = strNo;
			htStock[FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM + strNo] = htTemp[FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM + strNo];
			htStock[FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE + strNo] = htTemp[FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE + strNo];
		}
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirmTop_Click(object sender, EventArgs e)
	{
		// 変数宣言
		string strStockMessageId = (string)ViewState[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID];
		string strValidator = String.Empty;
		string strErrorMessagesDatum = String.Empty;

		//------------------------------------------------------
		// パラメタ格納
		//------------------------------------------------------
		Hashtable htInput = new Hashtable();
		ArrayList alInput = new ArrayList();

		//------------------------------------------------------
		// 処理ステータス
		//------------------------------------------------------		
		// 新規・コピー新規
		if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT
			|| (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_COPY_INSERT)
		{
			strValidator = "ProductStockMessageRegist";
			htInput.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID, tbProductStockMessageId.Text);
		}
		// 変更
		else if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_UPDATE)
		{
			strValidator = "ProductStockMessageModify";
			htInput.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID, strStockMessageId);
		}
		else if ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT)
		{
			// グローバル設定登録時にはIDを入力しないため、入力チェックしない(Hashtableに格納しない)
			strValidator = "ProductStockMessageRegist";
		}

		htInput.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_SHOP_ID, this.LoginOperatorShopId);										// 店舗ID
		htInput.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_NAME, tbStockMessageName.Text);	                            // 在庫文言名
		htInput.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF, tbStockMessageDef.Text);                             	// 標準在庫文言
		htInput.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_LAST_CHANGED, this.LoginOperatorName); // 最終更新者

		var languageCode = string.Empty;
		var languageLocaleId = string.Empty;
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			languageCode = ddlLanguageCode.SelectedValue.Split('/')[0];
			languageLocaleId = ddlLanguageCode.SelectedValue.Split('/')[1];
		}
		htInput.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_CODE, languageCode);
		htInput.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_LANGUAGE_LOCALE_ID, languageLocaleId);
		htInput.Add(Constants.SETTING_TABLE_NAME, this.ProductStockMessageSettingTableName);

		// 入力チェック
		string strErrorMessages = Validator.Validate(strValidator, htInput);

		// 入力された商品在庫文言情報(在庫基準)チェック
		Repeater r = rStockMessage;
		for (int iLoop = 0; iLoop < r.Items.Count; iLoop++)
		{
			strErrorMessages += CheckInputStock(iLoop, strValidator);
		}

		if (Constants.GLOBAL_OPTION_ENABLE && ((string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_GLOBAL_SETTING_INSERT))
		{
			// 重複チェック
			if (new ProductStockMessageService().CheckLanguageCodeDupulication(
				this.LoginOperatorShopId,
				strStockMessageId,
				languageCode,
				languageLocaleId) == false)
			{
				strErrorMessages += WebMessages.GetMessages(WebMessages.INPUTCHECK_DUPLICATION)
					.Replace("@@ 1 @@",
					//「言語コード」
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_PRODUCT_STOCK,
							Constants.VALUETEXT_PARAM_PRODUCT_STOCK_MESSAGE_REGISTER,
							Constants.VALUETEXT_PARAM_PRODUCT_STOCK_LANGUAGE_CODE));
			}

			htInput.Add(Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_ID, strStockMessageId);
		}

		if (strErrorMessages != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = strErrorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 商品在庫文言情報(在庫基準)並び替え
		alInput = GetStockDatumArrayList();

		// 在庫基準1～xx、在庫文言1～xx設定
		SetStockDatum(htInput, alInput);

		// パラメタをセッションへ格納
		htInput.Add(Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_DATUM_INFO, alInput);
		Session[Constants.SESSIONPARAM_KEY_PRODUCTSTOCK_INFO] = htInput;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];

		// 商品在庫文言情報確認ページへ遷移			
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCKMESSAGE_CONFIRM +
			"?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS]);
	}

	/// <summary>
	/// 入力された商品在庫文言情報チェック
	/// </summary>
	/// <param name="index">インデックス番号</param>
	/// <param name="validator">使用するバリデータ</param>
	/// <returns>エラーメッセージ</returns>
	protected string CheckInputStock(int index, string validator)
	{
		var rowNo = (index + 1).ToString();
		var errorMessages = new StringBuilder();

		// 在庫基準チェック
		errorMessages.Append(CheckInputStockDatum(((TextBox)rStockMessage.Items[index].FindControl("tbStockDatum")).Text, validator, rowNo));

		// 在庫文言チェック
		errorMessages.Append(CheckInputStockMessage(((TextBox)rStockMessage.Items[index].FindControl("tbStockMessage")).Text, validator, rowNo));

		return errorMessages.ToString();
	}

	/// <summary>
	/// 入力された商品在庫文言情報(在庫基準)チェック
	/// </summary>
	/// <param name="datum">在庫基準</param>
	/// <param name="validator">使用するバリデータ</param>
	/// <param name="rowNo">対象行No</param>
	/// <returns>エラーメッセージ</returns>
	protected string CheckInputStockDatum(string datum, string validator, string rowNo)
	{
		var htStockMessage = new Hashtable
		{
			{FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM, datum},
		};
		var errorMessages = Validator.Validate(validator, htStockMessage);
		if (string.IsNullOrEmpty(errorMessages)) return string.Empty;

		return errorMessages
			.Replace("在庫基準", WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_PRODUCTSTOCKMESSAGE_STOCK_DATUM_INPUT_ERROR))
			.Replace("@@ 1 @@", rowNo);
	}

	/// <summary>
	/// 入力された商品在庫文言情報(在庫文言)チェック
	/// </summary>
	/// <param name="message">在庫文言</param>
	/// <param name="validator">使用するバリデータ</param>
	/// <param name="rowNo">対象行No</param>
	/// <returns>エラーメッセージ</returns>
	protected string CheckInputStockMessage(string message, string validator, string rowNo)
	{
		var htStockMessage = new Hashtable
		{
			{FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE, message},
		};
		var errorMessages = Validator.Validate(validator, htStockMessage);
		if (string.IsNullOrEmpty(errorMessages)) return string.Empty;

		return errorMessages
			.Replace("在庫文言", WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_INPUT_ERROR))
			.Replace("@@ 1 @@", rowNo);
	}

	/// <summary>
	/// 追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertStockMessage_Click(object sender, EventArgs e)
	{
		// 変数宣言
		Hashtable htInput = new Hashtable();
		int iLoop = 0;
		string strNo = null;

		// 入力された商品在庫文言情報を格納
		Repeater r = rStockMessage;
		for (iLoop = 0; iLoop < r.Items.Count; iLoop++)
		{
			Hashtable htStockMessage = new Hashtable();

			strNo = ((int)(iLoop + 1)).ToString();
			htStockMessage.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_NO + strNo, strNo);
			htStockMessage.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM + strNo, ((TextBox)r.Items[iLoop].FindControl("tbStockDatum")).Text);
			htStockMessage.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE + strNo, ((TextBox)r.Items[iLoop].FindControl("tbStockMessage")).Text);

			m_alParam.Add(htStockMessage);
		}
		strNo = ((int)(iLoop + 1)).ToString();

		// 商品在庫文言情報追加
		htInput.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_NO + strNo, strNo);
		htInput.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM + strNo, "");
		htInput.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE + strNo, "");
		m_alParam.Add(htInput);

		// データバインド
		btnInsertStockMessage.DataBind();
		rStockMessage.DataSource = m_alParam;
		rStockMessage.DataBind();
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDeleteStockMessage_Click(object sender, EventArgs e)
	{
		// 変数宣言
		int iNo = 0;

		// 削除ボタンインデックス取得
		int iItemIndex = ((RepeaterItem)((Button)sender).Parent).ItemIndex;

		// 入力された商品在庫文言情報を格納
		Repeater r = rStockMessage;
		for (int iLoop = 0; iLoop < r.Items.Count; iLoop++)
		{
			// 削除対象の商品在庫文言情報以外を格納
			if (iLoop != iItemIndex)
			{
				Hashtable htStockMessage = new Hashtable();

				iNo += 1;
				htStockMessage.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_NO + iNo.ToString(), iNo.ToString());
				htStockMessage.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_DATUM + iNo.ToString(), ((TextBox)r.Items[iLoop].FindControl("tbStockDatum")).Text);
				htStockMessage.Add(FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE + iNo.ToString(), ((TextBox)r.Items[iLoop].FindControl("tbStockMessage")).Text);

				m_alParam.Add(htStockMessage);
			}
		}

		// データバインド
		btnInsertStockMessage.DataBind();
		rStockMessage.DataSource = m_alParam;
		rStockMessage.DataBind();
	}

	/// <summary>
	/// 編集可能か
	/// </summary>
	protected bool IsEditable
	{
		get { return (bool)ViewState["is_editable"]; }
		set { ViewState["is_editable"] = value; }
	}
	/// <summary>商品在庫文言設定設定テーブル名(w2_ProductStockMessage/w2_ProductStockMessageGlobal)</summary>
	private string ProductStockMessageSettingTableName
	{
		get { return (string)ViewState[Constants.SETTING_TABLE_NAME]; }
		set { ViewState[Constants.SETTING_TABLE_NAME] = value; }
	}
}
