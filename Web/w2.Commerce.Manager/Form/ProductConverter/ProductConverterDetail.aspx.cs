/*
=========================================================================================================
  Module      : 商品コンバータ 設定ページ処理(ProductConverterDetail.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using w2.App.Common;
using w2.App.Common.Product;

public partial class Form_ProductConverter_ProductConverterDetail : ProductConverterPage
{
	protected static string XML_MASTEREXPORTSETTING_J_NAME = "jname";					// フィールド名(日本語)
	protected static string XML_MASTEREXPORTSETTING_NAME = "name";						// 英語名
	const string XML_MASTEREXPORTSETTING_ROOT = "ProductConvSetting";					// マスタ出力定義ルート名
	const string FIELD_MALLPRDCNV_SEPARATERTYPE = "separaterType";							// 区切り文字形式

	Hashtable m_htParam = new Hashtable();
	List<ListItem> m_lProductConverterConvertTargets = new List<ListItem>();
	DataView m_dvProductConverterConvert = null;
	DataView m_dvProductConverterColumns = null;
	string m_strAdtoId = null;
	string m_strActionStatus = null;
	string m_shop_id = null;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// リクエスト取得＆ビューステート格納
		//------------------------------------------------------
		m_strActionStatus = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ACTION_STATUS]);
		ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, m_strActionStatus);

		m_strAdtoId = StringUtility.ToEmpty(Request[Constants.FIELD_MALLPRDCNV_ADTO_ID]);
		ViewState.Add(Constants.REQUEST_KEY_ADTO_ID, m_strAdtoId);

		//------------------------------------------------------
		// ログインしているオペレータのshop_idを取得
		//------------------------------------------------------
		m_shop_id = this.LoginOperatorShopId;

		//------------------------------------------------------
		// 画面制御
		//------------------------------------------------------
		InitializeComponents(m_strActionStatus);

		if (!IsPostBack)
		{
			if (Request[Constants.REQUEST_KEY_ADTO_ID] != null)
			{
				//------------------------------------------------------
				// 商品コンバータ情報取得
				//------------------------------------------------------
				m_htParam = GetRowInfoHashtable(GetProductConverterOnce(Request[Constants.REQUEST_KEY_ADTO_ID]), 0);

				//------------------------------------------------------
				// shop_idの整合性チェック
				//------------------------------------------------------
				if ((string)m_htParam[Constants.FIELD_MALLPRDCNV_SHOP_ID] != m_shop_id)
				{
					// エラーページへ
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
			}
			else
			{
				//------------------------------------------------------
				// 空行を取得
				//------------------------------------------------------
				m_htParam = GetRowInfoHashtable(GetProductConverterEmpty(), 0);
				switch ((string)m_htParam[Constants.FIELD_MALLPRDCNV_SEPARATER])
				{
					case ",":
						m_htParam[FIELD_MALLPRDCNV_SEPARATERTYPE] = Constants.FLG_MALLPRDCNVSEPARATERTYPE_COMMA;
						break;

					case "\t":
						m_htParam[FIELD_MALLPRDCNV_SEPARATERTYPE] = Constants.FLG_MALLPRDCNVSEPARATERTYPE_TAB;
						break;

					default:
						m_htParam[FIELD_MALLPRDCNV_SEPARATERTYPE] = Constants.FLG_MALLPRDCNVSEPARATERTYPE_COMMA;
						break;
				}
			}

			//------------------------------------------------------
			// 商品コンバータの文字列変換ルールを取得
			//------------------------------------------------------
			m_dvProductConverterConvert = GetProductConverterConvert(Request[Constants.REQUEST_KEY_ADTO_ID]);

			//------------------------------------------------------
			// 商品コンバータの出力列設定を取得
			//------------------------------------------------------
			m_dvProductConverterColumns = GetProductConverterColumns(Request[Constants.REQUEST_KEY_ADTO_ID]);

			if ((Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_COPY_INSERT)
				|| (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT))
			{
				// コピー新規登録の場合、商品コンバータIDを空にする
				m_htParam[Constants.FIELD_MALLPRDCNV_ADTO_ID] = "";
			}

			//------------------------------------------------------
			// 商品コンバータ詳細表示（データバインドを行う）
			//------------------------------------------------------
			ViewProductConverterDetail();

			// 抽出条件を取得する
			int iExtractionPatternCount = 0;
			foreach (ListItem liExtractPattern in ValueText.GetValueItemList(Constants.TABLE_MALLPRDCNV, Constants.FIELD_MALLPRDCNV_EXTRACTIONPATTERN))
			{
				lbExtractionPattern.Items.Add(liExtractPattern);

				int iExtractValue = 0;
				if (int.TryParse(liExtractPattern.Value, out iExtractValue)
					&& (m_htParam[Constants.FIELD_MALLPRDCNV_EXTRACTIONPATTERN] != null)
					&& (iExtractValue == (int)m_htParam[Constants.FIELD_MALLPRDCNV_EXTRACTIONPATTERN]))
				{
					lbExtractionPattern.SelectedIndex = iExtractionPatternCount;
				}

				iExtractionPatternCount++;
			}
		}
		else
		{
			//------------------------------------------------------
			// 商品コンバータ情報取得
			//------------------------------------------------------
			m_htParam = GetRowInfoHashtable(GetProductConverterOnce(Request[Constants.REQUEST_KEY_ADTO_ID]), 0);

			//------------------------------------------------------
			// shop_idの整合性チェック
			//------------------------------------------------------
			if ((Request[Constants.REQUEST_KEY_ADTO_ID] != null) && ((string)m_htParam[Constants.FIELD_MALLPRDCNV_SHOP_ID] != m_shop_id))
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			if ((Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_COPY_INSERT)
				|| (Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT))
			{
				m_htParam[Constants.FIELD_MALLPRDCNV_ADTO_ID] = "";
			}

			m_strActionStatus = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];
			m_strAdtoId = (string)ViewState[Constants.FIELD_MALLPRDCNV_ADTO_ID];

			// セッションから前回のデータを復元
			m_dvProductConverterConvert = GetDataViewByArray((List<Hashtable>)Session[Constants.SESSION_KEY_ADDETAIL_CONVERT]);
			m_dvProductConverterColumns = GetDataViewByArray((List<Hashtable>)Session[Constants.SESSION_KEY_ADDETAIL_COLUMNS]);

			// セッションに保存されているデータが空だったら作っておく
			if (m_dvProductConverterColumns == null)
			{
				m_dvProductConverterColumns = GetProductConverterColumns("-1");
			}
			if (m_dvProductConverterConvert == null)
			{
				m_dvProductConverterConvert = GetProductConverterConvert("-1");
			}

			//------------------------------------------------------
			// フォームの値をメンバ変数に書き戻す
			//------------------------------------------------------
			if (hfAdtoId.Value != "")
			{
				int iAdtoId;
				if (int.TryParse(hfAdtoId.Value, out iAdtoId))
				{
					m_htParam[Constants.FIELD_MALLPRDCNV_ADTO_ID] = iAdtoId; // 基本設定　ＩＤ（非表示）
				}
			}
			m_htParam[Constants.FIELD_MALLPRDCNV_ADTO_NAME] = tbProductConverterName.Text; // 基本設定　商品コンバータ 設定名
			m_htParam[Constants.FIELD_MALLPRDCNV_PATH] = tbPath.Text; // 基本設定　出力ファイルパス
			m_htParam[Constants.FIELD_MALLPRDCNV_FILENAME] = tbFilename.Text; // 基本設定　出力ファイル名

			// 基本設定　区切り文字
			if (rbProductConverterSeparaterCSV.Checked)
			{
				m_htParam[Constants.FIELD_MALLPRDCNV_SEPARATER] = Constants.FLG_MALLPRDCNVSEPARATERTYPE_COMMA;
				m_htParam[FIELD_MALLPRDCNV_SEPARATERTYPE] = Constants.FLG_MALLPRDCNVSEPARATERTYPE_COMMA;
			}
			else
			{
				m_htParam[Constants.FIELD_MALLPRDCNV_SEPARATER] = Constants.FLG_MALLPRDCNVSEPARATERTYPE_TAB;
				m_htParam[FIELD_MALLPRDCNV_SEPARATERTYPE] = Constants.FLG_MALLPRDCNVSEPARATERTYPE_TAB;
			}

			// 基本設定　文字コード
			if (rbProductConverterCharacterCodeSHIFT.Checked)
			{
				m_htParam[Constants.FIELD_MALLPRDCNV_CHARACTERCODETYPE] = Constants.FLG_MALLPRDCNVCHARACTERCODETYPE_SHIFT;
			}
			else if (rbProductConverterCharacterCodeUTF.Checked)
			{
				m_htParam[Constants.FIELD_MALLPRDCNV_CHARACTERCODETYPE] = Constants.FLG_MALLPRDCNVCHARACTERCODETYPE_UTF;
			}

			// 基本設定　改行コード
			if (rbProductConverterNewLineCR.Checked)
			{
				m_htParam[Constants.FIELD_MALLPRDCNV_NEWLINETYPE] = Constants.FLG_MALLPRDCNV_NEWLINETYPE_CR;
			}
			else if (rbProductConverterNewLineLF.Checked)
			{
				m_htParam[Constants.FIELD_MALLPRDCNV_NEWLINETYPE] = Constants.FLG_MALLPRDCNV_NEWLINETYPE_LF;
			}
			else if (rbProductConverterNewLineCRLF.Checked)
			{
				m_htParam[Constants.FIELD_MALLPRDCNV_NEWLINETYPE] = Constants.FLG_MALLPRDCNV_NEWLINETYPE_CRLF;
			}

			m_htParam[Constants.FIELD_MALLPRDCNV_ISHEADER] = cbIsHeader.Checked; // ヘッダ行出力
			m_htParam[Constants.FIELD_MALLPRDCNV_ISQUOTE] = cbProductConverterIsQuote.Checked; // 文字列の引用符（チェックボックス）
			m_htParam[Constants.FIELD_MALLPRDCNV_QUOTE] = tbQuote.Text; // 文字列の引用符（文字）

			// 基本設定 マスタ種別
			if (rbMasterTypeProduct.Checked)
			{
				m_htParam[Constants.FIELD_MALLPRDCNV_SOURCETABLENAME] = Constants.TABLE_PRODUCT;
			}
			else if (rbMasterTypeProductView.Checked)
			{
				m_htParam[Constants.FIELD_MALLPRDCNV_SOURCETABLENAME] = Constants.TABLE_PRODUCTVIEW;
			}

			// 基本設定　抽出条件
			m_htParam[Constants.FIELD_MALLPRDCNV_EXTRACTIONPATTERN] = lbExtractionPattern.SelectedItem.Value;

			// 出力項目設定
			for (int iLoop = 0; iLoop < rColumns.Items.Count; iLoop++)
			{
				// カラム情報が入っていない場合は、新しい行を追加する（セッション破棄回避対応）
				if (iLoop > m_dvProductConverterColumns.Count - 1)
				{
					m_dvProductConverterColumns.AddNew();
				}

				m_dvProductConverterColumns[iLoop][Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NO] = ((HiddenField)(rColumns.Items[iLoop].FindControl("hfProductConverterColumnNo"))).Value; // 行番号
				m_dvProductConverterColumns[iLoop][Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NAME] = ((TextBox)(rColumns.Items[iLoop].FindControl("tbProductConverterColumnName"))).Text; // 列名
				m_dvProductConverterColumns[iLoop][Constants.FIELD_MALLPRDCNVCOLUMNS_OUTPUT_FORMAT] = ((TextBox)(rColumns.Items[iLoop].FindControl("tbProductConverterColumnFormat"))).Text; // 出力フォーマット
			}

			// 文字列置換設定
			for (int iLoop = 0; iLoop < rConvert.Items.Count; iLoop++)
			{
				// カラム情報が入っていない場合は、新しい行を追加する（セッション破棄回避対応）
				if (iLoop > m_dvProductConverterConvert.Count - 1)
				{
					m_dvProductConverterConvert.AddNew();
				}

				m_dvProductConverterConvert[iLoop][Constants.FIELD_MALLPRDCNVRULE_CONVERTFROM] = ((TextBox)(rConvert.Items[iLoop].FindControl("tbConvertFrom"))).Text; // 変換前
				m_dvProductConverterConvert[iLoop][Constants.FIELD_MALLPRDCNVRULE_CONVERTTO] = ((TextBox)(rConvert.Items[iLoop].FindControl("tbConvertTo"))).Text; // 変換後
				m_dvProductConverterConvert[iLoop][Constants.FIELD_MALLPRDCNVRULE_TARGET] = ((HiddenField)(rConvert.Items[iLoop].FindControl("hfConvertTargetValue"))).Value; // 置換対象設定
			}
		}

		//------------------------------------------------------
		// 各情報を設定
		//------------------------------------------------------
		// 項目表示
		tbProductConverterName.Text = (string)m_htParam[Constants.FIELD_MALLPRDCNV_ADTO_NAME]; // 商品コンバータ 設定名
		hfAdtoId.Value = m_htParam[Constants.FIELD_MALLPRDCNV_ADTO_ID].ToString(); // 商品コンバータID
		tbPath.Text = (string)m_htParam[Constants.FIELD_MALLPRDCNV_PATH]; // 出力ファイルパス
		tbFilename.Text = (string)m_htParam[Constants.FIELD_MALLPRDCNV_FILENAME]; // 出力ファイル名

		// 区切り文字 
		switch ((int)m_htParam[FIELD_MALLPRDCNV_SEPARATERTYPE])
		{
			case Constants.FLG_MALLPRDCNVSEPARATERTYPE_COMMA:
				rbProductConverterSeparaterCSV.Checked = true;
				break;

			case Constants.FLG_MALLPRDCNVSEPARATERTYPE_TAB:
				rbProductConverterSeparaterTSV.Checked = true;
				break;
		}

		// 文字コード
		switch ((string)m_htParam[Constants.FIELD_MALLPRDCNV_CHARACTERCODETYPE])
		{
			case Constants.FLG_MALLPRDCNVCHARACTERCODETYPE_SHIFT:
				rbProductConverterCharacterCodeSHIFT.Checked = true;
				break;

			case Constants.FLG_MALLPRDCNVCHARACTERCODETYPE_UTF:
				rbProductConverterCharacterCodeUTF.Checked = true;
				break;

			default:
				rbProductConverterCharacterCodeSHIFT.Checked = true;
				break;
		}

		// 改行コード
		switch ((string)m_htParam[Constants.FIELD_MALLPRDCNV_NEWLINETYPE])
		{
			case Constants.FLG_MALLPRDCNV_NEWLINETYPE_CR:
				rbProductConverterNewLineCR.Checked = true;
				break;

			case Constants.FLG_MALLPRDCNV_NEWLINETYPE_LF:
				rbProductConverterNewLineLF.Checked = true;
				break;

			case Constants.FLG_MALLPRDCNV_NEWLINETYPE_CRLF:
				rbProductConverterNewLineCRLF.Checked = true;
				break;

			default:
				rbProductConverterNewLineCRLF.Checked = true;
				break;
		}

		cbIsHeader.Checked = (bool)m_htParam[Constants.FIELD_MALLPRDCNV_ISHEADER]; // ヘッダ行出力

		// 文字列の引用符
		cbProductConverterIsQuote.Checked = (bool)m_htParam[Constants.FIELD_MALLPRDCNV_ISQUOTE];
		cbProductConverterIsQuote.Attributes.Add("onclick", "javascript:on_product_converter_export_quote();");
		divProductConverterExportQuote.Style.Add(HtmlTextWriterStyle.Display, (cbProductConverterIsQuote.Checked) ? "inline" : "none");
		tbQuote.Text = (string)m_htParam[Constants.FIELD_MALLPRDCNV_QUOTE];

		// マスタ種別
		switch ((string)m_htParam[Constants.FIELD_MALLPRDCNV_SOURCETABLENAME])
		{
			case Constants.TABLE_PRODUCT:
				rbMasterTypeProduct.Checked = true;
				rbMasterTypeProductView.Checked = false;
				divSourceTableStyleProduct.Style.Add(HtmlTextWriterStyle.Display, "inline");
				divSourceTableStyleProductView.Style.Add(HtmlTextWriterStyle.Display, "none");
				break;

			case Constants.TABLE_PRODUCTVIEW:
				rbMasterTypeProduct.Checked = false;
				rbMasterTypeProductView.Checked = true;
				divSourceTableStyleProduct.Style.Add(HtmlTextWriterStyle.Display, "none");
				divSourceTableStyleProductView.Style.Add(HtmlTextWriterStyle.Display, "inline");
				break;

			default:
				rbMasterTypeProduct.Checked = true;
				rbMasterTypeProductView.Checked = false;
				divSourceTableStyleProduct.Style.Add(HtmlTextWriterStyle.Display, "inline");
				divSourceTableStyleProductView.Style.Add(HtmlTextWriterStyle.Display, "none");
				break;
		}

		// 出力項目設定（▲▼ボタン）
		StringBuilder sbJavascriptForColumnBtn = new StringBuilder();
		sbJavascriptForColumnBtn.Append("javascript:if (document.getElementById('");
		sbJavascriptForColumnBtn.Append(hfProductConverterColumnSelected.ClientID);
		sbJavascriptForColumnBtn.Append("').value==-1){alert(\"");
		sbJavascriptForColumnBtn.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_SELECT_TARGET));
		sbJavascriptForColumnBtn.Append("\");return false;}else{return true;}");
		btnDelColumn.OnClientClick = btnUpColumn.OnClientClick = btnDownColumn.OnClientClick = sbJavascriptForColumnBtn.ToString();

		// 文字列置換設定（▲▼ボタン）
		StringBuilder sbJavascriptForConvertBtn = new StringBuilder();
		sbJavascriptForConvertBtn.Append("javascript:if (document.getElementById('");
		sbJavascriptForConvertBtn.Append(hfProductConverterConvertSelected.ClientID);
		sbJavascriptForConvertBtn.Append("').value==-1){alert(\"");
		sbJavascriptForConvertBtn.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_SELECT_TARGET));
		sbJavascriptForConvertBtn.Append("\");return false;}else{return true;}");
		btnDelConvert.OnClientClick = btnUpConvert.OnClientClick = btnDownConvert.OnClientClick = sbJavascriptForConvertBtn.ToString();
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		// 新規・コピー新規？
		if ((strActionStatus == Constants.ACTION_STATUS_INSERT) || (strActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
		{
			btnInsertTop.Visible = btnInsertBottom.Visible = true;
			btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = false;
			btnDeleteTop.Visible = btnDeleteBottom.Visible = false;
			btnUpdateTop.Visible = btnUpdateBottom.Visible = false;
		}
		// 詳細？
		else
		{
			// Show message when insert/update complete
			if ((Session[Constants.SESSION_KEY_PARAM] != null) && (Session[Constants.SESSION_KEY_PARAM].ToString() == Constants.ACTION_STATUS_COMPLETE))
			{
				divComplete.Visible = true;
				lMessage.Text = Session[Constants.SESSION_KEY_ERROR_MSG].ToString();
				Session[Constants.SESSION_KEY_ERROR_MSG] = null;
				Session[Constants.SESSION_KEY_PARAM] = null;
			}

			btnInsertTop.Visible = btnInsertBottom.Visible = false;
			btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = true;
			btnDeleteTop.Visible = btnDeleteBottom.Visible = true;
			btnUpdateTop.Visible = btnUpdateBottom.Visible = true;
		}
	}

	/// <summary>
	/// コンバートリストを設定する
	/// </summary>
	private void SetConvertList()
	{
		// 置換設定の対象リストを作る
		m_lProductConverterConvertTargets = new List<ListItem>();
		m_lProductConverterConvertTargets.AddRange(
			//「全て」
			ValueText.GetValueItemArray(
				Constants.VALUETEXT_PARAM_PRODUCT_CONVERTER,
				Constants.VALUETEXT_PARAM_PRODUCT_CONVERTER_TARGETS));
		foreach (DataRowView drvProductConverterColumns in m_dvProductConverterColumns)
		{
			m_lProductConverterConvertTargets.Add(new ListItem(
				(string)drvProductConverterColumns[Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NAME],
				drvProductConverterColumns[Constants.FIELD_MALLPRDCNVCOLUMNS_ADCOLUMN_ID].ToString()));
		}
	}

	/// <summary>
	/// 商品コンバータ情報表示(DataGridにDataView(商品コンバータ情報)を設定)
	/// </summary>
	/// <remarks>※データバインドを行う</remarks>
	private void ViewProductConverterDetail()
	{
		//------------------------------------------------------
		// XMLファイル取得
		//------------------------------------------------------
		XmlDocument objXmlDoc = new XmlDocument();
		GetMasterExportSettingXml(objXmlDoc);

		//------------------------------------------------------
		// データソース設定
		//------------------------------------------------------
		rProductColumnNames.DataSource = ChangeProductExtendName(MasterFieldSetting.RemoveMasterFields(GetMasterField(objXmlDoc, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCT), Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCT));
		rProductViewColumnNames.DataSource = ChangeProductExtendName(MasterFieldSetting.RemoveMasterFields(GetMasterField(objXmlDoc, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTVIEW), Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTVIEW));
		rConvert.DataSource = m_dvProductConverterConvert;
		rColumns.DataSource = m_dvProductConverterColumns;

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		DataBind();

		//------------------------------------------------------
		// JavaScript設定（データバインド後に行う）
		//------------------------------------------------------
		// 出力フォーマット設定
		for (int iLoop = 0; iLoop < rColumns.Items.Count; iLoop++)
		{
			// 出力フォーマットにonfocusイベントをセット（列名）
			StringBuilder sbColumnName = new StringBuilder();
			sbColumnName.Append("javascript:listselect_mout1(document.getElementById( 'trProductConverterColumnRow' + document.getElementById('");
			sbColumnName.Append(hfProductConverterColumnSelected.ClientID);
			sbColumnName.Append("').value ) );");
			sbColumnName.Append("document.getElementById('");
			sbColumnName.Append(hfProductConverterColumnSelected.ClientID);
			sbColumnName.Append("').value=");
			sbColumnName.Append(iLoop.ToString());
			sbColumnName.Append(";document.getElementById('");
			sbColumnName.Append(hfProductConverterColumnFocus.ClientID);
			sbColumnName.Append("').value=id;");
			sbColumnName.Append("listselect_mover(document.getElementById('trProductConverterColumnRow");
			sbColumnName.Append(iLoop.ToString());
			sbColumnName.Append("'));");
			((TextBox)(rColumns.Items[iLoop].FindControl("tbProductConverterColumnName"))).Attributes.Add("onfocus", sbColumnName.ToString());

			// 出力フォーマットにonfocusイベントをセット（出力フォーマット名）
			StringBuilder sbColumnFormat = new StringBuilder();
			sbColumnFormat.Append("javascript:listselect_mout1(document.getElementById( 'trProductConverterColumnRow' + document.getElementById('");
			sbColumnFormat.Append(hfProductConverterColumnSelected.ClientID);
			sbColumnFormat.Append("').value ) );");
			sbColumnFormat.Append("document.getElementById('");
			sbColumnFormat.Append(hfProductConverterColumnSelected.ClientID);
			sbColumnFormat.Append("').value=");
			sbColumnFormat.Append(iLoop.ToString());
			sbColumnFormat.Append(";document.getElementById('");
			sbColumnFormat.Append(hfProductConverterColumnFocus.ClientID);
			sbColumnFormat.Append("').value=id;");
			sbColumnFormat.Append("listselect_mover(document.getElementById('trProductConverterColumnRow");
			sbColumnFormat.Append(iLoop.ToString());
			sbColumnFormat.Append("'));");
			((TextBox)(rColumns.Items[iLoop].FindControl("tbProductConverterColumnFormat"))).Attributes.Add("onfocus", sbColumnFormat.ToString());
		}

		// 文字列置換設定
		for (int iLoop = 0; iLoop < rConvert.Items.Count; iLoop++)
		{
			// 変換テーブルにonfocusイベントをセット（変換前）
			StringBuilder sbConvertFrom = new StringBuilder();
			sbConvertFrom.Append("javascript:listselect_mout1(document.getElementById( 'trProductConverterConvertRow' + document.getElementById('");
			sbConvertFrom.Append(hfProductConverterConvertSelected.ClientID);
			sbConvertFrom.Append("').value ) );");
			sbConvertFrom.Append("document.getElementById('");
			sbConvertFrom.Append(hfProductConverterConvertSelected.ClientID);
			sbConvertFrom.Append("').value=");
			sbConvertFrom.Append(iLoop.ToString());
			sbConvertFrom.Append(";");
			sbConvertFrom.Append("listselect_mover(document.getElementById('trProductConverterConvertRow");
			sbConvertFrom.Append(iLoop.ToString());
			sbConvertFrom.Append("'));");
			((TextBox)(rConvert.Items[iLoop].FindControl("tbConvertFrom"))).Attributes.Add("onfocus", sbConvertFrom.ToString());

			// 変換テーブルにonfocusイベントをセット（変換後）
			StringBuilder sbConvertTo = new StringBuilder();
			sbConvertTo.Append("javascript:listselect_mout1(document.getElementById( 'trProductConverterConvertRow' + document.getElementById('");
			sbConvertTo.Append(hfProductConverterConvertSelected.ClientID);
			sbConvertTo.Append("').value ) );");
			sbConvertTo.Append("document.getElementById('");
			sbConvertTo.Append(hfProductConverterConvertSelected.ClientID);
			sbConvertTo.Append("').value=");
			sbConvertTo.Append(iLoop.ToString());
			sbConvertTo.Append(";");
			sbConvertTo.Append("listselect_mover(document.getElementById('trProductConverterConvertRow");
			sbConvertTo.Append(iLoop.ToString());
			sbConvertTo.Append("'));");
			((TextBox)(rConvert.Items[iLoop].FindControl("tbConvertTo"))).Attributes.Add("onfocus", sbConvertTo.ToString());
		}

		//------------------------------------------------------
		// 文字列置換対象一覧ドロップダウン設定（データバインド後に行う）
		//------------------------------------------------------
		// 文字列置換対象リスト作成（メンバ変数へセット）
		SetConvertList();

		// 文字列置換対象一覧ドロップダウン作成
		foreach (RepeaterItem riConvert in rConvert.Items)
		{
			// 変換テーブルのDropDownListに選択肢をセット
			DropDownList ddlConvertTarget = (DropDownList)(riConvert.FindControl("ddlConvertTarget"));

			// 現在の選択をセット
			string strTargetId = null;
			ddlConvertTarget.Items.AddRange(
				//「全て」
				ValueText.GetValueItemArray(
					Constants.VALUETEXT_PARAM_PRODUCT_CONVERTER,
					Constants.VALUETEXT_PARAM_PRODUCT_CONVERTER_TARGETS));

			bool blFirstFlg = true;
			foreach (ListItem li in m_lProductConverterConvertTargets)
			{
				// 最初の行を読み飛ばす（最初の行は「全て」）
				if (blFirstFlg)
				{
					blFirstFlg = false;
					continue;
				}

				// 選択肢をセットする
				ddlConvertTarget.Items.Add(new ListItem(li.Text, li.Value));
				if (li.Value == m_dvProductConverterConvert[riConvert.ItemIndex][Constants.FIELD_MALLPRDCNVRULE_TARGET].ToString())
				{
					strTargetId = li.Value;
				}
			}
			foreach (ListItem li in ddlConvertTarget.Items)
			{
				li.Selected = (li.Value == strTargetId);
			}

			// 隠しフィールドを追加
			HiddenField hfValue = (HiddenField)(riConvert.FindControl("hfConvertTargetValue"));
			hfValue.Value = strTargetId;
			ddlConvertTarget.Attributes.Add("onchange", "javascript:document.getElementById('" + hfValue.ClientID + "').value=value;");
		}

		Session[Constants.SESSION_KEY_PARAM] = (object)m_htParam;
		Session[Constants.SESSION_KEY_ADDETAIL_CONVERT] = (object)GetArrayByDataView(m_dvProductConverterConvert);
		Session[Constants.SESSION_KEY_ADDETAIL_COLUMNS] = (object)GetArrayByDataView(m_dvProductConverterColumns);
	}

	/// <summary>
	///	マスタ項目一覧の拡張項目フィールド名を設定名称に変更する
	/// </summary>
	/// <param name="lMasterFields">マスタ項目一覧</param>
	/// <returns>マスタ項目一覧</returns>
	private List<Hashtable> ChangeProductExtendName(List<Hashtable> lMasterFields)
	{
		List<Hashtable> lResults = new List<Hashtable>(lMasterFields);

		// 拡張項目設定値取得
		DataView dvProductExtendSetting = GetProductExtendSetting(m_shop_id);

		// 拡張項目名称変更処
		int iCount = 0;
		foreach (Hashtable htMasterField in lMasterFields)
		{
			bool blSearch = false;
			foreach (DataRowView drvProductExtendSetting in dvProductExtendSetting)
			{
				if (htMasterField.ContainsValue("extend" + drvProductExtendSetting[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO].ToString()))
				{
					// 拡張項目の名称を設定する
					((Hashtable)lResults[iCount])["jname"] = (string)drvProductExtendSetting[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME];
					blSearch = true;
					break;
				}
			}
			if (blSearch == false)
			{
				// 未使用の拡張項目を一覧から削除する
				if (htMasterField["name"].ToString().IndexOf("extend") != -1)
				{
					lResults.RemoveAt(iCount);
					iCount--;
				}
			}
			iCount++;
		}

		return lResults;
	}

	/// <summary>
	/// ハッシュの配列からDataViewを取得する
	/// </summary>
	/// <param name="lHashs">ハッシュ配列</param>
	/// <returns>表示可能なデータビュー</returns>
	private DataView GetDataViewByArray(List<Hashtable> lHashs)
	{
		DataView dvResult = null;

		if (lHashs.Count > 0)
		{
			DataTable dtTable = new DataTable();

			// スキーマを定義
			foreach (string strKey in lHashs[0].Keys)
			{
				DataColumn dcColumn = new DataColumn(strKey);
				dtTable.Columns.Add(dcColumn);
			}

			// データを作成
			for (int iLoop = 0; iLoop < lHashs.Count; iLoop++)
			{
				DataRow drRow = dtTable.NewRow();
				foreach (string strKey in lHashs[0].Keys)
				{
					drRow[strKey] = lHashs[iLoop][strKey];
				}
				dtTable.Rows.Add(drRow);
			}

			dvResult = new DataView(dtTable);
		}

		return dvResult;
	}

	/// <summary>
	/// DataViewからハッシュの配列を取得する
	/// </summary>
	/// <param name="dvParam">変換対象のDataView</param>
	/// <returns>ハッシュ配列</returns>
	private List<Hashtable> GetArrayByDataView(DataView dvParam)
	{
		List<Hashtable> lResults = new List<Hashtable>();

		for (int iLoop = 0; iLoop < dvParam.Count; iLoop++)
		{
			Hashtable htParam = GetRowInfoHashtable(dvParam, iLoop);
			lResults.Add(htParam);
		}

		return lResults;
	}

	/// <summary>
	/// マスタ出力セッティングファイル定義アレイリスト取得
	/// </summary>
	/// <param name="objXmlDoc">XMLオブジェクト</param>
	/// <param name="strMasterKbn">マスタ区分</param>
	/// <returns>マスタ出力セッティングファイル定義アレイリスト</returns>
	private List<Hashtable> GetMasterField(XmlDocument xmlDocument, string strMasterKbn)
	{
		List<Hashtable> lResults = new List<Hashtable>();

		// パラメタ情報取得
		XmlNode xnSNode = xmlDocument.SelectSingleNode(XML_MASTEREXPORTSETTING_ROOT + "/" + strMasterKbn);

		// <フィールド>
		foreach (XmlNode xnNode in xnSNode.ChildNodes)
		{
			// コメントは対象外
			if (xnNode.NodeType != XmlNodeType.Comment)
			{
				Hashtable htParam = new Hashtable();
				XmlAttributeCollection xacCollection = xnNode.Attributes;

				// <フィールド属性 xx1="" xx2="">
				foreach (XmlAttribute xmlAttribute in xacCollection)
				{
					// xx1,xx2等を格納
					htParam.Add(xmlAttribute.Name, xmlAttribute.Value);
				}

				lResults.Add(htParam);
			}
		}

		// Add Product Tag
		lResults.AddRange(GetProductTagField());

		return lResults;
	}

	/// <summary>
	/// Get product tag fields
	/// </summary>
	/// <returns> Product tag fields</returns>
	private List<Hashtable> GetProductTagField()
	{
		var listField = ProductTagUtility.GetProductTagSetting()
			.Select(tag => new Hashtable
				{
					{ XML_MASTEREXPORTSETTING_J_NAME, tag.TagName },
					{ "field", tag.TagId },
					{ XML_MASTEREXPORTSETTING_NAME, tag.TagId }
				})
			.ToList();
		return listField;
	}

	/// <summary>
	/// マスタ出力セッティングファイル読込
	/// </summary>
	/// <param name="objXmlDoc">XMLオブジェクト</param>
	/// <returns>取込結果</returns>
	private bool GetMasterExportSettingXml(XmlDocument xmlDocument)
	{
		try
		{
			xmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_PRODUCTCONV_SETTING);
		}
		catch
		{
			xmlDocument = null;
		}

		return (xmlDocument != null);
	}

	/// <summary>
	/// 追加ボタンクリック（文字列置換設定）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddConvert_Click(object sender, EventArgs e)
	{
		// 空行を1行追加する
		m_dvProductConverterConvert.AddNew();

		// 画面を再表示する
		ViewProductConverterDetail();
	}

	/// <summary>
	/// 削除 ボタンクリック（文字列置換設定）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelConvert_Click(object sender, EventArgs e)
	{
		// 選択行を削除する
		int iSelected;
		if (int.TryParse(hfProductConverterConvertSelected.Value, out iSelected))
		{
			if (-1 < iSelected)
			{
				m_dvProductConverterConvert.Delete(iSelected);
				hfProductConverterConvertSelected.Value = "-1";
			}
		}

		// 画面を再表示する
		ViewProductConverterDetail();
	}

	/// <summary>
	/// ▲ ボタンクリック（文字列置換設定）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpConvert_Click(object sender, EventArgs e)
	{
		// 選択行と１行上を入れ替える
		int iSelected;
		if (int.TryParse(hfProductConverterConvertSelected.Value, out iSelected))
		{
			// 選択されている行が最上位ではない かつ データ件数が２件以上
			if ((0 < iSelected) && (m_dvProductConverterConvert.Count > 1))
			{
				m_dvProductConverterConvert[iSelected][Constants.FIELD_MALLPRDCNVRULE_CONVERTFROM] = ((TextBox)(rConvert.Items[iSelected - 1].FindControl("tbConvertFrom"))).Text;
				m_dvProductConverterConvert[iSelected][Constants.FIELD_MALLPRDCNVRULE_CONVERTTO] = ((TextBox)(rConvert.Items[iSelected - 1].FindControl("tbConvertTo"))).Text;
				m_dvProductConverterConvert[iSelected - 1][Constants.FIELD_MALLPRDCNVRULE_CONVERTFROM] = ((TextBox)(rConvert.Items[iSelected].FindControl("tbConvertFrom"))).Text;
				m_dvProductConverterConvert[iSelected - 1][Constants.FIELD_MALLPRDCNVRULE_CONVERTTO] = ((TextBox)(rConvert.Items[iSelected].FindControl("tbConvertTo"))).Text;
			}
		}

		// 画面を再表示する
		ViewProductConverterDetail();
	}

	/// <summary>
	/// ▼ ボタンクリック（文字列置換設定）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDownConvert_Click(object sender, EventArgs e)
	{
		// 選択行と１行下を入れ替える
		int iSelected;
		if (int.TryParse(hfProductConverterConvertSelected.Value, out iSelected))
		{
			// 選択されている行が最下位ではない かつ データ件数が２件以上
			if ((m_dvProductConverterConvert.Count > iSelected + 1) && (m_dvProductConverterConvert.Count > 1))
			{
				m_dvProductConverterConvert[iSelected][Constants.FIELD_MALLPRDCNVRULE_CONVERTFROM] = ((TextBox)(rConvert.Items[iSelected + 1].FindControl("tbConvertFrom"))).Text;
				m_dvProductConverterConvert[iSelected][Constants.FIELD_MALLPRDCNVRULE_CONVERTTO] = ((TextBox)(rConvert.Items[iSelected + 1].FindControl("tbConvertTo"))).Text;
				m_dvProductConverterConvert[iSelected + 1][Constants.FIELD_MALLPRDCNVRULE_CONVERTFROM] = ((TextBox)(rConvert.Items[iSelected].FindControl("tbConvertFrom"))).Text;
				m_dvProductConverterConvert[iSelected + 1][Constants.FIELD_MALLPRDCNVRULE_CONVERTTO] = ((TextBox)(rConvert.Items[iSelected].FindControl("tbConvertTo"))).Text;
			}
		}

		// 画面を再表示する
		ViewProductConverterDetail();
	}

	/// <summary>
	/// 追加 ボタンクリック（出力項目設定）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddColumn_Click(object sender, EventArgs e)
	{
		// 仮ID発番が必要
		int iMaxId = 0;
		foreach (DataRowView drvProductConverterColumns in m_dvProductConverterColumns)
		{
			int iTmp;
			if (int.TryParse(drvProductConverterColumns[Constants.FIELD_MALLPRDCNVCOLUMNS_ADCOLUMN_ID].ToString(), out iTmp))
			{
				iMaxId = (iMaxId < iTmp) ? iTmp : iMaxId;
			}
		}
		iMaxId++;

		m_dvProductConverterColumns.AddNew();
		m_dvProductConverterColumns[m_dvProductConverterColumns.Count - 1][Constants.FIELD_MALLPRDCNVCOLUMNS_ADCOLUMN_ID] = iMaxId.ToString();
		m_dvProductConverterColumns[m_dvProductConverterColumns.Count - 1][Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NAME] = "";

		// 画面を再表示する
		ViewProductConverterDetail();
	}

	/// <summary>
	/// 削除 ボタンクリック（出力項目設定）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelColumn_Click(object sender, EventArgs e)
	{
		// 選択行を削除する
		int iSelected;
		if (int.TryParse(hfProductConverterColumnSelected.Value, out iSelected))
		{
			if (-1 < iSelected)
			{
				m_dvProductConverterColumns.Delete(iSelected);
				hfProductConverterColumnSelected.Value = "-1";
			}
		}

		// 画面を再表示する
		ViewProductConverterDetail();
	}

	/// <summary>
	/// ▲ ボタンクリック（出力項目設定）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpColumn_Click(object sender, EventArgs e)
	{
		// 選択行と１行上を入れ替える
		int iSelected;
		if (int.TryParse(hfProductConverterColumnSelected.Value, out iSelected))
		{
			// 選択されている行が最上位ではない かつ データ件数が２件以上
			if ((0 < iSelected) && (m_dvProductConverterColumns.Count > 1))
			{
				m_dvProductConverterColumns[iSelected][Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NAME] = ((TextBox)(rColumns.Items[iSelected - 1].FindControl("tbProductConverterColumnName"))).Text;
				m_dvProductConverterColumns[iSelected][Constants.FIELD_MALLPRDCNVCOLUMNS_OUTPUT_FORMAT] = ((TextBox)(rColumns.Items[iSelected - 1].FindControl("tbProductConverterColumnFormat"))).Text;
				m_dvProductConverterColumns[iSelected - 1][Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NAME] = ((TextBox)(rColumns.Items[iSelected].FindControl("tbProductConverterColumnName"))).Text;
				m_dvProductConverterColumns[iSelected - 1][Constants.FIELD_MALLPRDCNVCOLUMNS_OUTPUT_FORMAT] = ((TextBox)(rColumns.Items[iSelected].FindControl("tbProductConverterColumnFormat"))).Text;
			}
		}

		// 画面の再表示を行う
		ViewProductConverterDetail();
	}

	/// <summary>
	/// ▼ ボタンクリック（出力項目設定）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDownColumn_Click(object sender, EventArgs e)
	{
		// 選択行と１行下を入れ替える
		int iSelected;
		if (int.TryParse(hfProductConverterColumnSelected.Value, out iSelected))
		{
			// 選択されている行が最下位ではない かつ データ件数が２件以上
			if ((m_dvProductConverterColumns.Count > iSelected + 1) && (m_dvProductConverterColumns.Count > 1))
			{
				m_dvProductConverterColumns[iSelected][Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NAME] = ((TextBox)(rColumns.Items[iSelected + 1].FindControl("tbProductConverterColumnName"))).Text;
				m_dvProductConverterColumns[iSelected][Constants.FIELD_MALLPRDCNVCOLUMNS_OUTPUT_FORMAT] = ((TextBox)(rColumns.Items[iSelected + 1].FindControl("tbProductConverterColumnFormat"))).Text;
				m_dvProductConverterColumns[iSelected + 1][Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NAME] = ((TextBox)(rColumns.Items[iSelected].FindControl("tbProductConverterColumnName"))).Text;
				m_dvProductConverterColumns[iSelected + 1][Constants.FIELD_MALLPRDCNVCOLUMNS_OUTPUT_FORMAT] = ((TextBox)(rColumns.Items[iSelected].FindControl("tbProductConverterColumnFormat"))).Text;
			}
		}

		// 画面の再表示を行う
		ViewProductConverterDetail();
	}

	/// <summary>
	/// コピー新規登録する ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTCONVERTER_DETAIL);
		sbResult.Append("?");
		sbResult.Append(Constants.REQUEST_KEY_ADTO_ID).Append("=").Append(HttpUtility.UrlEncode(m_strAdtoId));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_COPY_INSERT));

		Response.Redirect(sbResult.ToString());
	}

	/// <summary>
	/// 削除 ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		if ((m_htParam[Constants.FIELD_MALLPRDCNV_ADTO_ID] != null) && (m_htParam[Constants.FIELD_MALLPRDCNV_ADTO_ID].ToString() != ""))
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MallProductConverter", "DeleteProductConverter"))
			{
				// 削除実行
				int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, m_htParam);
			}
		}

		// 一覧画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTCONVERTER_LIST);
	}

	/// <summary>
	/// 更新する ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void UpdateProductConverter(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 入力チェック＆重複チェック
		//------------------------------------------------------
		StringBuilder sbErrorMessages = new StringBuilder(Validator.Validate("ProductConverterDetail", m_htParam));

		// 出力項目設定
		foreach (RepeaterItem riColumns in rColumns.Items)
		{
			Hashtable htColumns = new Hashtable();
			htColumns[Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NO] = ((HiddenField)(riColumns.FindControl("hfProductConverterColumnNo"))).Value;
			htColumns[Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NAME] = ((TextBox)(riColumns.FindControl("tbProductConverterColumnName"))).Text;
			htColumns[Constants.FIELD_MALLPRDCNVCOLUMNS_OUTPUT_FORMAT] = ((TextBox)(riColumns.FindControl("tbProductConverterColumnFormat"))).Text;

			//------------------------------------------------------
			// 出力フォーマットチェック
			//------------------------------------------------------
			// HACK: ★条件式の正規表現が複雑でメンテナンス性が低いため、仕様・コードを見直す必要あり★
			// (\"[^\"]*\")+を|で他の正規表現と組み合わせると負荷がかかり危険なため、正規表現を複数に分けてチェックする。
			string strCheckFormat = (string)htColumns[Constants.FIELD_MALLPRDCNVCOLUMNS_OUTPUT_FORMAT];
			bool blCheckFlg = false;

			// フィールド名
			if (Regex.IsMatch(strCheckFormat, "^[a-zA-Z0-9_]+($|:LEN[0-9]+$|:LENB[0-9]+$)"))
			{
				blCheckFlg = true;
			}

			// 特殊タグ
			if (Regex.IsMatch(strCheckFormat, "^([a-zA-Z0-9_]+|\\[SP:[a-zA-Z0-9_]+\\])+($|:LEN[0-9]+$|:LENB[0-9]+$)"))
			{
				blCheckFlg = true;
			}

			// 固定文字列
			if (Regex.IsMatch(strCheckFormat, "^(\"[^\"]*\")+($|:LEN[0-9]+$|:LENB[0-9]+$)"))
			{
				blCheckFlg = true;
			}

			// 条件式
			if (Regex.IsMatch(strCheckFormat, "^(\\?=[a-zA-Z0-9_]+(\\((\"[^\\(\\)\"]+\"|[a-zA-Z0-9_\\?<>=,\"!]+|[a-zA-Z0-9_]+|,)+\\))+)+($|:LEN[0-9]+$|:LENB[0-9]+$)"))
			{
				blCheckFlg = true;
			}

			// 特殊タグ（MallStockAlertDispatch用）
			if (Regex.IsMatch(strCheckFormat, "^(\\[SP:MallStockAlertDispatch)(\\([a-zA-Z0-9_]+,[^,]+){1}(,?[^:,]+:[a-zA-Z0-9_ ]+){1,}(\\)\\])$"))
			{
				blCheckFlg = true;
			}

			// その他
			if (blCheckFlg == false)
			{
				// 空文字の場合、エラーとしない
				if (strCheckFormat != "")
				{
					//------------------------------------------------------
					// フィールド名、固定文字列の組み合わせチェック
					//------------------------------------------------------
					// ダブルクォーテーション数が偶数の場合、チェックを続行する
					string[] strSplits = strCheckFormat.Split('"');
					if ((strSplits.Length != 1) && (strSplits.Length % 2 != 0))
					{
						int iStart = (strCheckFormat.IndexOf("\"") == 0) ? 1 : 0;
						int iEnd = (strCheckFormat.LastIndexOf("\"") == strCheckFormat.Length - 1) ? 1 : 0;
						for (int iLoop = iStart; iLoop < strSplits.Length - iEnd; iLoop++)
						{
							// 「フィールド名」「ダブルクォーテーションで囲まれた文字列」をチェックする
							blCheckFlg = Regex.IsMatch(strSplits[iLoop], (iLoop % 2 == 0) ? "^[a-zA-Z0-9_]+($|:LEN[0-9]+$|:LENB[0-9]+$)" : "^([^\"]*)+($|:LEN[0-9]+$|:LENB[0-9]+$)");
							if (blCheckFlg == false)
							{
								break;
							}
						}
					}
				}
				else
				{
					// 空文字はエラーとしない
					blCheckFlg = true;
				}
			}

			//------------------------------------------------------
			// エラーメッセージ出力
			//------------------------------------------------------
			string strErrorMessagesTmp = Validator.Validate("ProductConverterDetail", htColumns);
			strErrorMessagesTmp += ((blCheckFlg == false)
				? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_CATEGORY_INCORRECT_FORMAT)
				: string.Empty);
			if (strErrorMessagesTmp != "")
			{
				sbErrorMessages.Append("[").Append(htColumns[Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NAME]).Append("]");
				sbErrorMessages.Append(strErrorMessagesTmp);
			}
		}

		if (sbErrorMessages.ToString() != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = sbErrorMessages.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// ファイル名日付チェック
		//------------------------------------------------------
		string strFileName = ((string)m_htParam[Constants.FIELD_MALLPRDCNV_FILENAME]);
		try
		{
			Regex regex = new Regex("\\[[^\\[\\]]+\\]");
			Match match = regex.Match(strFileName);
			while (match.Success)
			{
				string strDateTimeString = DateTime.Now.ToString(match.Value);
				strDateTimeString = strDateTimeString.Substring(1, strDateTimeString.Length - 2);

				if (match.Value.Equals(Constants.FIELD_SHOP_SHOP_ID) == false)
				{
					strFileName = regex.Replace(strFileName, strDateTimeString, 1, match.Index);
				}
				match = match.NextMatch();
			}
		}
		catch (FormatException)
		{
			// エラーページへ
			sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_CATEGORY_FILE_NAME_DATE_INCORRECT_FORMAT));
			Session[Constants.SESSION_KEY_ERROR_MSG] = sbErrorMessages.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// ファイル名として許されるか？
		//------------------------------------------------------
		char[] cInvalidchs = Path.GetInvalidFileNameChars();
		foreach (char cStr in cInvalidchs)
		{
			// ファイル名として許可された文字でない場合、エラーページへ
			if (strFileName.LastIndexOf(cStr) != -1)
			{
				sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_CATEGORY_FILE_NAME_INCORRECT_FORMAT));
				Session[Constants.SESSION_KEY_ERROR_MSG] = sbErrorMessages.ToString();
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}

		//------------------------------------------------------
		// コマンド・パラメタ作成
		//------------------------------------------------------
		// コマンド作成
		char[] cDirPaths = ((string)m_htParam[Constants.FIELD_MALLPRDCNV_PATH]).ToCharArray();

		StringBuilder sbExecParam = new StringBuilder();
		sbExecParam.Append(m_htParam[Constants.FIELD_MALLPRDCNV_ADTO_ID]).Append(" "); // 出力対象ＩＤ
		sbExecParam.Append("\"").Append(((string)m_htParam[Constants.FIELD_MALLPRDCNV_PATH]).Replace('/', '\\')); // 出力先パス
		sbExecParam.Append(((cDirPaths.Length > 1) && ((cDirPaths[cDirPaths.Length - 1] != '/') && (cDirPaths[cDirPaths.Length - 1] != '\\')) ? @"\" : ""));
		sbExecParam.Append(m_htParam[Constants.FIELD_MALLPRDCNV_FILENAME]).Append("\" ");
		sbExecParam.Append(m_htParam[Constants.FIELD_MALLPRDCNV_SOURCETABLENAME]); // ソーステーブル名

		//------------------------------------------------------
		// ＤＢを更新する
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			try
			{
				// トランザクションを開始する
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction(IsolationLevel.RepeatableRead);

				// パラメータ取得
				m_htParam[FIELD_MALLPRDCNV_SEPARATERTYPE] = (rbProductConverterSeparaterCSV.Checked) ? Constants.FLG_MALLPRDCNVSEPARATERTYPE_COMMA : Constants.FLG_MALLPRDCNVSEPARATERTYPE_TAB;
				m_htParam[Constants.FIELD_MALLPRDCNV_CHARACTERCODETYPE] = (rbProductConverterCharacterCodeUTF.Checked) ? Constants.FLG_MALLPRDCNVCHARACTERCODETYPE_UTF : Constants.FLG_MALLPRDCNVCHARACTERCODETYPE_SHIFT;
				if (rbProductConverterNewLineCR.Checked)
				{
					m_htParam[Constants.FIELD_MALLPRDCNV_NEWLINETYPE] = Constants.FLG_MALLPRDCNV_NEWLINETYPE_CR;
				}
				else if (rbProductConverterNewLineLF.Checked)
				{
					m_htParam[Constants.FIELD_MALLPRDCNV_NEWLINETYPE] = Constants.FLG_MALLPRDCNV_NEWLINETYPE_LF;
				}
				else
				{
					m_htParam[Constants.FIELD_MALLPRDCNV_NEWLINETYPE] = Constants.FLG_MALLPRDCNV_NEWLINETYPE_CRLF;
				}

				if ((m_strActionStatus == Constants.ACTION_STATUS_INSERT) || (m_strActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
				{
					// w2_MallPrdcnv テーブルを新規挿入、shop_idを追加
					m_htParam[Constants.FIELD_MALLPRDCNV_SHOP_ID] = m_shop_id;

					using (SqlStatement sqlStatement = new SqlStatement("MallProductConverter", "InsertProductConverter"))
					{
						DataView dvProductConverter = sqlStatement.SelectSingleStatement(sqlAccessor, m_htParam);
						m_htParam[Constants.FIELD_MALLPRDCNV_ADTO_ID] = dvProductConverter[0][0];
					}

					m_strAdtoId = StringUtility.ToEmpty(m_htParam[Constants.FIELD_MALLPRDCNV_ADTO_ID]);
				}
				else
				{
					// w2_MallPrdcnv テーブルを更新
					using (SqlStatement sqlStatement = new SqlStatement("MallProductConverter", "UpdateProductConverter"))
					{
						int iResult = sqlStatement.ExecStatement(sqlAccessor, m_htParam);
					}
				}

				// 仮ID－行番号の対比
				Hashtable htOldProductConverterColumnsMap = new Hashtable();
				for (int iLoop = 0; iLoop < m_dvProductConverterColumns.Count; iLoop++)
				{
					int iAdColumnId;
					if (int.TryParse((string)m_dvProductConverterColumns[iLoop][Constants.FIELD_MALLPRDCNVCOLUMNS_ADCOLUMN_ID], out iAdColumnId))
					{
						htOldProductConverterColumnsMap[iAdColumnId] = iLoop;
					}
				}

				// w2_MallPrdcnvColumns テーブル更新、いったん削除した後にすべてinsert
				using (SqlStatement sqlStatement = new SqlStatement("MallProductConverter", "DeleteProductConverterColumns"))
				{
					int iResult = sqlStatement.ExecStatement(sqlAccessor, m_htParam);
				}

				foreach (RepeaterItem riColumns in rColumns.Items)
				{
					using (SqlStatement sqlStatement = new SqlStatement("MallProductConverter", "InsertProductConverterColumns"))
					{
						// 出力フォーマット
						Hashtable htColumns = new Hashtable();
						htColumns[Constants.FIELD_MALLPRDCNV_ADTO_ID] = m_htParam[Constants.FIELD_MALLPRDCNV_ADTO_ID];
						htColumns[Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NO] = ((HiddenField)(riColumns.FindControl("hfProductConverterColumnNo"))).Value;
						htColumns[Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NAME] = ((TextBox)(riColumns.FindControl("tbProductConverterColumnName"))).Text;
						htColumns[Constants.FIELD_MALLPRDCNVCOLUMNS_OUTPUT_FORMAT] = ((TextBox)(riColumns.FindControl("tbProductConverterColumnFormat"))).Text;

						int iResult = sqlStatement.ExecStatement(sqlAccessor, htColumns);
					}
				}

				// w2_MallPrdcnvRule テーブルを更新、いったん削除した後にすべてinsert
				using (SqlStatement sqlStatement = new SqlStatement("MallProductConverter", "DeleteProductConverterConvert"))
				{
					int iResult = sqlStatement.ExecStatement(sqlAccessor, m_htParam);
				}

				foreach (RepeaterItem riConvert in rConvert.Items)
				{
					using (SqlStatement sqlStatement = new SqlStatement("MallProductConverter", "InsertProductConverterConvert"))
					{
						// 文字置換テーブル
						Hashtable htConvert = new Hashtable();
						htConvert[Constants.FIELD_MALLPRDCNV_ADTO_ID] = m_htParam[Constants.FIELD_MALLPRDCNV_ADTO_ID];
						htConvert[Constants.FIELD_MALLPRDCNVRULE_CONVERTFROM] = ((TextBox)(riConvert.FindControl("tbConvertFrom"))).Text;
						htConvert[Constants.FIELD_MALLPRDCNVRULE_CONVERTTO] = ((TextBox)(riConvert.FindControl("tbConvertTo"))).Text;

						HiddenField hfConvertTargetValue = (HiddenField)(riConvert.FindControl("hfConvertTargetValue"));
						int iTarget;
						htConvert[Constants.FIELD_MALLPRDCNVRULE_TARGET] = (int.TryParse(hfConvertTargetValue.Value, out iTarget)) ? htOldProductConverterColumnsMap[iTarget] : null;

						int iResult = sqlStatement.ExecStatement(sqlAccessor, htConvert);
					}
				}

				// コミット
				sqlAccessor.CommitTransaction();
			}
			catch (Exception)
			{
				sqlAccessor.RollbackTransaction();
				throw;
			}
		}

		// Get message to show when complete insert or update
		Session[Constants.SESSION_KEY_PARAM] = Constants.ACTION_STATUS_COMPLETE;
		Session[Constants.SESSION_KEY_ERROR_MSG] = ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE) || (this.ActionStatus == Constants.ACTION_STATUS_DETAIL))
			? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_CONVERTER_UPDATE_SUCCESS)
			: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_CONVERTER_REGIST_SUCCESS);

		// 変更した結果を表示するために詳細ページへリダイレクト
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTCONVERTER_DETAIL);
		sbResult.Append("?");
		sbResult.Append(Constants.REQUEST_KEY_ADTO_ID).Append("=").Append(HttpUtility.UrlEncode(m_strAdtoId));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_DETAIL));

		Response.Redirect(sbResult.ToString());
	}

	/// <summary>
	/// 一覧へ戻る ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		// 一覧へ戻す
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTCONVERTER_LIST);
	}

	/// <summary>
	/// 商品コンバータ基本設定を取得する（１件分の空レコードを取得する）
	/// </summary>
	private DataView GetProductConverterEmpty()
	{
		DataView dvProductConverter = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallProductConverter", "GetProductConverterEmpty"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MALLPRDCNV_ADTO_ID, null);

			dvProductConverter = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		dvProductConverter.ApplyDefaultSort = true;

		return dvProductConverter;
	}

	/// <summary>
	/// 商品コンバータ文字列変換設定を取得する
	/// </summary>
	/// <param name="strAdtoId">商品コンバータID</param>
	private DataView GetProductConverterConvert(string strAdtoId)
	{
		DataView dvProductConverter = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallProductConverter", "GetProductConverterConvert"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MALLPRDCNVRULE_ADTO_ID, strAdtoId);

			dvProductConverter = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		dvProductConverter.Sort = Constants.FIELD_MALLPRDCNVRULE_ADCONV_ID + " ASC";

		return dvProductConverter;
	}

	/// <summary>
	/// 商品コンバータ出力列設定を取得する
	/// </summary>
	/// <param name="strAdtoId">商品コンバータID</param>
	private DataView GetProductConverterColumns(string strAdtoId)
	{
		DataView dvProductConverter = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallProductConverter", "GetProductConverterColumns"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MALLPRDCNVRULE_ADTO_ID, strAdtoId);

			dvProductConverter = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		dvProductConverter.Sort = Constants.FIELD_MALLPRDCNVCOLUMNS_COLUMN_NO + " ASC";

		return dvProductConverter;
	}

	/// <summary>
	/// 曜日取得
	/// </summary>
	/// <param name="dayOfWeek">曜日</param>
	/// <returns>曜日(数字)</returns>
	private int GetDayOfWeek2Int(DayOfWeek dayOfWeek)
	{
		int iResult = 0;

		switch (dayOfWeek)
		{
			case DayOfWeek.Sunday:
				iResult = 1;
				break;

			case DayOfWeek.Monday:
				iResult = 2;
				break;

			case DayOfWeek.Tuesday:
				iResult = 3;
				break;

			case DayOfWeek.Wednesday:
				iResult = 4;
				break;

			case DayOfWeek.Thursday:
				iResult = 5;
				break;

			case DayOfWeek.Friday:
				iResult = 6;
				break;

			case DayOfWeek.Saturday:
				iResult = 7;
				break;
		}

		return iResult;
	}
}
