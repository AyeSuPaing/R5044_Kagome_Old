/*
=========================================================================================================
  Module      : 商品セット情報確認ページ処理(ProductSetConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;

public partial class Form_ProductSet_ProductSetConfirm : ProductSetPage
{
	private string m_strProductSetId = null;
	public string m_strActionStatus = null;
	protected Hashtable m_htProductSet = null;
	private List<ProductSetItem> m_lProductSetItems = null;

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
			// パラメタ取得
			//------------------------------------------------------
			m_strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, m_strActionStatus);

			m_strProductSetId = (string)Request[Constants.REQUEST_KEY_PRODUCT_SET_ID];
			ViewState.Add(Constants.REQUEST_KEY_PRODUCT_SET_ID, m_strProductSetId);

			// ID・URL表示設定
			trId.Visible = (m_strActionStatus == Constants.ACTION_STATUS_DETAIL);
			trUrl.Visible = (m_strActionStatus == Constants.ACTION_STATUS_DETAIL);

			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponent(m_strActionStatus);

			//------------------------------------------------------
			// データ取得
			//------------------------------------------------------
			switch (m_strActionStatus)
			{
				case Constants.ACTION_STATUS_DETAIL:
					m_htProductSet = GetProductSet(m_strProductSetId);
					m_lProductSetItems = GetProductSetItem(m_strProductSetId);
					break;

				case Constants.ACTION_STATUS_INSERT:
				case Constants.ACTION_STATUS_COPY_INSERT:
				case Constants.ACTION_STATUS_UPDATE:
					m_htProductSet = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
					m_lProductSetItems = (List<ProductSetItem>)Session[Constants.SESSION_KEY_PARAM2];
					break;
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				this.ProductSetTranslationData = GetProductSetTranslationData(m_strProductSetId);
			}

			// ビューステートに保存
			ViewState["ProductSet"] = m_htProductSet;
			ViewState["ProductSetItems"] = m_lProductSetItems;

			//------------------------------------------------------
			// データ表示
			//------------------------------------------------------
			lProductSetId.Text = WebSanitizer.HtmlEncode(m_htProductSet[Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID]);
			lProductSetName.Text = WebSanitizer.HtmlEncode(m_htProductSet[Constants.FIELD_PRODUCTSET_PRODUCT_SET_NAME]);
			lParentMaxMin.Text = CreateMinMaxString(m_htProductSet[Constants.FIELD_PRODUCTSET_PARENT_MIN], m_htProductSet[Constants.FIELD_PRODUCTSET_PARENT_MAX]);
			lChildMaxMin.Text = CreateMinMaxString(m_htProductSet[Constants.FIELD_PRODUCTSET_CHILD_MIN], m_htProductSet[Constants.FIELD_PRODUCTSET_CHILD_MAX]);
			lDescriptionKbn.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTSET, Constants.FIELD_PRODUCTSET_DESCRIPTION_KBN, (string)m_htProductSet[Constants.FIELD_PRODUCTSET_DESCRIPTION_KBN]));
			lMaxSellQuantity.Text = StringUtility.ToEmpty(m_htProductSet[Constants.FIELD_PRODUCTSET_MAX_SELL_QUANTITY]);
			lValidFlg.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_PRODUCTSET, Constants.FIELD_PRODUCTSET_VALID_FLG, (string)m_htProductSet[Constants.FIELD_PRODUCTSET_VALID_FLG]));
			this.PcUrl = Constants.URL_FRONT_PC_SECURE + Constants.PAGE_FRONT_PRODUCTSET_LIST + "?shop=" + this.LoginOperatorShopId + "&psid=" + lProductSetId.Text;

			SetHtmlForPreviewList();

			rProductList.DataSource = m_lProductSetItems;
			rProductList.DataBind();

			//商品が存在するか
			tblProductErrorMessages.Visible = m_lProductSetItems.Any(value => string.IsNullOrEmpty(value.Name));
			//エラーメッセージセット
			if (tblProductErrorMessages.Visible)
			{
				lbProductErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_SET_ITEM_DELETED_ERROR);
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				DataBind();
			}
		}
		else
		{
			m_strActionStatus = (string)ViewState[Constants.REQUEST_KEY_ACTION_STATUS];
			m_strProductSetId = (string)ViewState[Constants.REQUEST_KEY_PRODUCT_SET_ID];
			m_htProductSet = (Hashtable)ViewState["ProductSet"];
			m_lProductSetItems = (List<ProductSetItem>)ViewState["ProductSetItems"];
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	/// <param name="strActionKbn"></param>
	private void InitializeComponent(string strActionKbn)
	{
		// ボタン
		btnEditTop.Visible = btnEditBottom.Visible = (strActionKbn == Constants.ACTION_STATUS_DETAIL);
		btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = (strActionKbn == Constants.ACTION_STATUS_DETAIL);
		btnDeleteTop.Visible = btnDeleteBottom.Visible = (strActionKbn == Constants.ACTION_STATUS_DETAIL);
		btnInsertTop.Visible = btnInsertBottom.Visible = ((strActionKbn == Constants.ACTION_STATUS_INSERT) || (strActionKbn == Constants.ACTION_STATUS_COPY_INSERT));
		btnUpdateTop.Visible = btnUpdateBottom.Visible = (strActionKbn == Constants.ACTION_STATUS_UPDATE);
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, EventArgs e)
	{
		// 編集画面へ遷移
		StringBuilder sbUrl = new StringBuilder(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTSET_REGISTER);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_PRODUCT_SET_ID).Append("=").Append(HttpUtility.UrlEncode(m_strProductSetId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_UPDATE));

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// コピー新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		// 編集画面へ遷移
		StringBuilder sbUrl = new StringBuilder(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTSET_REGISTER);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_PRODUCT_SET_ID).Append("=").Append(HttpUtility.UrlEncode(m_strProductSetId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_COPY_INSERT));

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// 削除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		// 削除
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductSet", "DeleteProductSet"))
		{
			sqlStatement.ExecStatementWithOC(sqlAccessor, m_htProductSet);
		}

		// 一覧画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSET_LIST);
	}

	/// <summary>
	/// 登録する
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		// Vreate new product set ID
		string newProductSetId = NumberingUtility.CreateKeyId(this.LoginOperatorShopId, Constants.NUMBER_KEY_PRODUCT_SET_ID, 8);

		// 登録
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			try
			{
				m_htProductSet[Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID] = newProductSetId;
				using (SqlStatement sqlStatement = new SqlStatement("ProductSet", "InsertProductSet"))
				{
					sqlStatement.ExecStatement(sqlAccessor, m_htProductSet);
				}

				foreach (ProductSetItem psi in m_lProductSetItems)
				{
					using (SqlStatement sqlStatement = new SqlStatement("ProductSet", "InsertProductSetItem"))
					{
						Hashtable htInput = psi.GetHashtable();
						htInput.Add(Constants.FIELD_PRODUCTSETITEM_SHOP_ID, this.LoginOperatorShopId);
						htInput.Add(Constants.FIELD_PRODUCTSETITEM_PRODUCT_SET_ID, newProductSetId);
						htInput.Add(Constants.FIELD_PRODUCTSETITEM_LAST_CHANGED, this.LoginOperatorName);

						sqlStatement.ExecStatement(sqlAccessor, htInput);
					}
				}

				sqlAccessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				sqlAccessor.RollbackTransaction();
				throw ex;
			}
		}

		// 一覧画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSET_LIST);
	}

	/// <summary>
	/// 更新する
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		// 更新
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			try
			{
				using (SqlStatement sqlStatement = new SqlStatement("ProductSet", "UpdateProductSet"))
				{
					sqlStatement.ExecStatement(sqlAccessor, m_htProductSet);
				}

				using (SqlStatement sqlStatement = new SqlStatement("ProductSet", "DelleteProductSetItem"))
				{
					sqlStatement.ExecStatement(sqlAccessor, m_htProductSet);
				}

				foreach (ProductSetItem psi in m_lProductSetItems)
				{
					using (SqlStatement sqlStatement = new SqlStatement("ProductSet", "InsertProductSetItem"))
					{
						Hashtable htInput = psi.GetHashtable();
						htInput.Add(Constants.FIELD_PRODUCTSETITEM_SHOP_ID, this.LoginOperatorShopId);
						htInput.Add(Constants.FIELD_PRODUCTSETITEM_PRODUCT_SET_ID, m_htProductSet[Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID]);
						htInput.Add(Constants.FIELD_PRODUCTSETITEM_LAST_CHANGED, this.LoginOperatorName);

						sqlStatement.ExecStatement(sqlAccessor, htInput);
					}
				}

				// コミット
				sqlAccessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				sqlAccessor.RollbackTransaction();
				throw ex;
			}
		}

		// 一覧画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSET_LIST);
	}

	/// <summary>
	/// 個数制限文字列作成
	/// </summary>
	/// <param name="psiProductSetItem"></param>
	/// <returns></returns>
	protected string CreateMinMaxString(object objMin, object objMax)
	{
		StringBuilder sbResutl = new StringBuilder();
		if (StringUtility.ToEmpty(objMin).Length != 0)
		{
			sbResutl.Append(
				string.Format(
					ReplaceTag("@@DispText.common_message.morethan@@"),
					objMin));
			
			sbResutl.Append(" ");
		}
		if (StringUtility.ToEmpty(objMin).Length + StringUtility.ToEmpty(objMax).Length != 0)
		{
		}
		if (StringUtility.ToEmpty(objMax).Length != 0)
		{
			sbResutl.Append(
				string.Format(
					ReplaceTag("@@DispText.common_message.lessthan@@"),
					objMax));
		}
		return sbResutl.ToString();
	}

	# region -GetProductSetTranslationData 商品セット翻訳情報取得
	/// <summary>
	/// 商品セット翻訳情報取得
	/// </summary>
	/// <param name="productSetId">商品セットID</param>
	/// <returns>商品セット翻訳情報</returns>
	private NameTranslationSettingModel[] GetProductSetTranslationData(string productSetId)
	{
		var searchCondition = new NameTranslationSettingSearchCondition
		{
			DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTSET,
			MasterId1 = productSetId,
		};
		var translationData = new NameTranslationSettingService().GetTranslationSettingsByMasterId(searchCondition);
		return translationData;
	}
	#endregion

	/// <summary>
	/// プレビュー対象HTMLリスト設定
	/// （表示用文言、モバイル用表示文言）
	/// </summary>
	private void SetHtmlForPreviewList()
	{
		this.HtmlForPreviewList = new List<string>
		{ 
			(string)m_htProductSet[Constants.FIELD_PRODUCTSET_DESCRIPTION],
			(string)m_htProductSet[Constants.FIELD_PRODUCTSET_DESCRIPTION_MOBILE],
		};
	}

	/// <summary> PC用URL </summary>
	protected string PcUrl { get; set; }
	/// <summary>商品セット翻訳設定情報</summary>
	protected NameTranslationSettingModel[] ProductSetTranslationData
	{
		get { return (NameTranslationSettingModel[])ViewState["productset_translation_data"]; }
		set { ViewState["productset_translation_data"] = value; }
	}
}
