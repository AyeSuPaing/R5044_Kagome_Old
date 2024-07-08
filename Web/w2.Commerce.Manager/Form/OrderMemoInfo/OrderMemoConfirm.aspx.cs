/*
=========================================================================================================
  Module      : 注文メモ情報確認ページ処理(OrderMemoConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;

public partial class Form_OrderMemoInfo_OrderMemoConfirm : BasePage
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
			// リクエスト取得
			//------------------------------------------------------
			string strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(strActionStatus);

			this.OrderMemoSettingTranslationData = new NameTranslationSettingModel[0];

			//------------------------------------------------------
			// 画面設定処理
			//------------------------------------------------------
			// 登録・コピー登録・更新画面確認？
			if ((strActionStatus == Constants.ACTION_STATUS_INSERT)
				|| (strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
				|| (strActionStatus == Constants.ACTION_STATUS_UPDATE))
			{
				// 処理区分チェック
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
				this.OrderMemoSetting = (Hashtable)Session[Constants.SESSIONPARAM_KEY_ORDER_MEMO_INFO];
				ViewState[Constants.SESSIONPARAM_KEY_ORDER_MEMO_INFO] = (Hashtable)Session[Constants.SESSIONPARAM_KEY_ORDER_MEMO_INFO];
			}
			// 詳細表示？
			else if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatements = new SqlStatement("OrderMemoSetting", "GetOrderMemoSettingFromId"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID, Request[Constants.REQUEST_KEY_ORDER_MEMO_ID]);

					DataView dvOrderMemo = sqlStatements.SelectSingleStatementWithOC(sqlAccessor, htInput);

					// 該当データ無しの場合
					if (dvOrderMemo.Count == 0)
					{
						// エラーページへ
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}
					this.OrderMemoSetting = dvOrderMemo[0];
				}

				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					var orderMemoId = (string)Request[Constants.REQUEST_KEY_ORDER_MEMO_ID];
					this.OrderMemoSettingTranslationData = GetSettingTranslationData(orderMemoId);
					//DataBind();
				}
			}
			// それ以外の場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// データバインド
			DataBind();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		// 新規・コピー新規？
		if ((strActionStatus == Constants.ACTION_STATUS_INSERT)
			|| (strActionStatus == Constants.ACTION_STATUS_COPY_INSERT))
		{
			btnInsertTop.Visible = true;
			btnInsertBottom.Visible = true;
			trDetailTop.Visible = false;
			trConfirmTop.Visible = true;
		}
		// 更新？
		else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			btnUpdateTop.Visible = true;
			btnUpdateBottom.Visible = true;
			trDetailTop.Visible = false;
			trConfirmTop.Visible = true;
		}
		// 詳細
		else if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			btnEditTop.Visible = true;
			btnEditBottom.Visible = true;
			btnCopyInsertTop.Visible = true;
			btnCopyInsertBottom.Visible = true;
			btnDeleteTop.Visible = true;
			btnDeleteBottom.Visible = true;
			trDateCreated.Visible = true;
			trDateChanged.Visible = true;
			trLastChanged.Visible = true;
			trDetailTop.Visible = true;
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

		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_ORDER_MEMO_REGISTER);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_ORDER_MEMO_ID).Append("=").Append(HttpUtility.UrlEncode(Request[Constants.REQUEST_KEY_ORDER_MEMO_ID]));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_UPDATE));

		// 登録画面へ
		Response.Redirect(sbResult.ToString());
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

		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_ORDER_MEMO_REGISTER);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_ORDER_MEMO_ID).Append("=").Append(HttpUtility.UrlEncode(Request[Constants.REQUEST_KEY_ORDER_MEMO_ID]));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_COPY_INSERT));

		// 登録画面へ
		Response.Redirect(sbResult.ToString());
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("OrderMemoSetting", "DeleteOrderMemoSetting"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_ORDERMEMOSETTING_ORDER_MEMO_ID, Request[Constants.REQUEST_KEY_ORDER_MEMO_ID]);
			sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
		}

		// 注文メモ一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_MEMO_LIST);
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("OrderMemoSetting", "InsertOrderMemoSetting"))
		{
			Hashtable htInput = (Hashtable)ViewState[Constants.SESSIONPARAM_KEY_ORDER_MEMO_INFO];
			sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
		}

		// 注文メモ一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_MEMO_LIST);
	}


	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("OrderMemoSetting", "UpdateOrderMemoSetting"))
		{
			Hashtable htInput = (Hashtable)ViewState[Constants.SESSIONPARAM_KEY_ORDER_MEMO_INFO];
			sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
		}

		// 注文メモ一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_MEMO_LIST);
	}

	#region -GetSettingTranslationData 注文メモ設定翻訳情報取得
	/// <summary>
	/// 注文メモ設定翻訳情報取得
	/// </summary>
	/// <param name="orderMemoId">注文メモID</param>
	/// <returns>注文メモ設定翻訳情報</returns>
	private NameTranslationSettingModel[] GetSettingTranslationData(string orderMemoId)
	{
		var searchCondition = new NameTranslationSettingSearchCondition
		{
			DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_ORDERMEMOSETTING,
			MasterId1 = orderMemoId,
			MasterId2 = string.Empty,
			MasterId3 = string.Empty,
		};
		var translationData = new NameTranslationSettingService().GetTranslationSettingsByMasterId(searchCondition);
		return translationData;
	}
	#endregion

	/// <summary>注文メモ設定</summary>
	protected object OrderMemoSetting { get; private set; }
	/// <summary>注文メモ翻訳設定情報</summary>
	protected NameTranslationSettingModel[] OrderMemoSettingTranslationData
	{
		get { return (NameTranslationSettingModel[])ViewState["ordermemosetting_translation_data"]; }
		set { ViewState["ordermemosetting_translation_data"] = value; }
	}
}

