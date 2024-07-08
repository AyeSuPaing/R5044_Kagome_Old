/*
=========================================================================================================
  Module      : 決済種別情報確認ページ処理(PaymentConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.RefreshFileManager;
using w2.App.Common.User;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;
using w2.Domain.UserManagementLevel;

public partial class Form_Payment_PaymentConfirm : PaymentPage
{
	protected Hashtable m_htParam = new Hashtable();					// 決済種別情報データバインド用
	protected ArrayList m_alParam = new ArrayList();					// 決済手数料情報データバインド用

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
			if (strActionStatus == Constants.ACTION_STATUS_INSERT ||
				strActionStatus == Constants.ACTION_STATUS_COPY_INSERT ||
				strActionStatus == Constants.ACTION_STATUS_UPDATE
				)
			{
				//------------------------------------------------------
				// 処理区分チェック
				//------------------------------------------------------
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
				m_htParam = (Hashtable)Session[Constants.SESSIONPARAM_KEY_PAYMENT_INFO];
				m_alParam = (ArrayList)m_htParam[Constants.SESSIONPARAM_KEY_PAYMENT_PRICE_INFO];
			}
			// 詳細表示？
			else if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				// 決済種別ID取得
				string strPaymentId = Request[Constants.REQUEST_KEY_PAYMENT_ID];
				DataView dv = null;
				DataRow dr = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatements = new SqlStatement("Payment", "GetPayment"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_PAYMENT_SHOP_ID, this.LoginOperatorShopId);
					htInput.Add(Constants.FIELD_PAYMENT_PAYMENT_ID, strPaymentId);

					DataSet ds = sqlStatements.SelectStatementWithOC(sqlAccessor, htInput);
					dv = ds.Tables["Table"].DefaultView;

					// 該当データが有りの場合
					if (ds.Tables["Table"].DefaultView.Count != 0)
					{
						dr = ds.Tables["Table"].Rows[0];
					}
					// 該当データ無しの場合
					else
					{
						// エラーページへ
						Session[Constants.SESSION_KEY_ERROR_MSG] =
							WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}
				}

				// Hashtabe格納
				foreach (DataColumn dc in dr.Table.Columns)
				{
					m_htParam.Add(dc.ColumnName, dr[dc.ColumnName]);
				}

				// 決済種別手数料情報設定
				for (int iLoop = 0; iLoop < dv.Count; iLoop++)
				{
					Hashtable htPaymentPrice = new Hashtable();
					htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE_NO,
						dv[iLoop][Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE_NO]);	// 枝番
					htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_TGT_PRICE_BGN,
						dv[iLoop][Constants.FIELD_PAYMENTPRICE_TGT_PRICE_BGN]);		// 対象購入金額（以上）
					htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_TGT_PRICE_END,
						dv[iLoop][Constants.FIELD_PAYMENTPRICE_TGT_PRICE_END]);		// 対象購入金額（以下）
					htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE,
						dv[iLoop][Constants.FIELD_PAYMENTPRICE_PAYMENT_PRICE]);		// 決済手数料
					m_alParam.Add(htPaymentPrice);
				}
				// 決済種別手数料情報を格納
				m_htParam.Add(Constants.SESSIONPARAM_KEY_PAYMENT_PRICE_INFO, m_alParam);

				// 翻訳設定情報取得
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					var searchCondition = new NameTranslationSettingSearchCondition
					{
						DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT,
						MasterId1 = strPaymentId,
						MasterId2 = string.Empty,
						MasterId3 = string.Empty,
					};
					this.PaymentTranslationData = new NameTranslationSettingService().GetTranslationSettingsByMasterId(searchCondition);
				}
			}
			// それ以外の場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// 決済手数料表示制御
			if ((string)m_htParam[Constants.FIELD_PAYMENT_PAYMENT_PRICE_KBN] == Constants.FLG_PAYMENT_PAYMENT_PRICE_KBN_SINGULAR)
			{
				trPaymentPrice.Visible = true;
				rPaymentPrice.Visible = false;
			}
			// 決済手数料を分ける
			else if ((string)m_htParam[Constants.FIELD_PAYMENT_PAYMENT_PRICE_KBN] == Constants.FLG_PAYMENT_PAYMENT_PRICE_KBN_PLURAL)
			{
				trPaymentPrice.Visible = false;
				rPaymentPrice.Visible = true;
			}

			// Get User ManagementLevel Name List
			var listLevel = m_htParam[Constants.FIELD_PAYMENT_USER_MANAGEMENT_LEVEL_NOT_USE].ToString().Split(',');
			var userLevelList = new UserManagementLevelService().GetAllList()
				.Where(m => listLevel.Contains(m.UserManagementLevelId))
				.ToArray();

			lbUserManagementLevelNameList.Text = (userLevelList.Length > 0)
				? string.Join(", ", userLevelList.Select(m => m.UserManagementLevelName).ToArray())
				: string.Empty;

			// 利用不可注文者区分項目表示
			// 全てのユーザー、全てのゲストを表示
			var orderOwnerKbnNotUseList = m_htParam[Constants.FIELD_PAYMENT_ORDER_OWNER_KBN_NOT_USE].ToString().Split(',');
			var orderOwnerKbnList = ValueText.GetValueItemArray(Constants.TABLE_ORDEROWNER, Constants.VALUE_TEXT_KEY_ORDEROWNER_USER_KBN_ALL)
				.Where(orderOwnerKbnNotUse => orderOwnerKbnNotUseList.Contains(orderOwnerKbnNotUse.Value))
				.ToArray();

			lOrderOwnerKbnNameList.Text = (orderOwnerKbnList.Length > 0)
				? string.Join(", ", orderOwnerKbnList.Select(m => m.Text).ToArray()) 
				: string.Empty;

			// VIewStateにも格納しておく
			ViewState.Add(Constants.SESSIONPARAM_KEY_PAYMENT_INFO, m_htParam);

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
		if (strActionStatus == Constants.ACTION_STATUS_INSERT ||
			strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
		{
			btnInsertTop.Visible = true;
			btnInsertBottom.Visible = true;
			trConfirm.Visible = true;
		}
		// 更新？
		else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			btnUpdateTop.Visible = true;
			btnUpdateBottom.Visible = true;
			trConfirm.Visible = true;
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
			trPaymentId.Visible = true;
			trDateCreated.Visible = true;
			trDateChanged.Visible = true;
			trLastChanged.Visible = true;
			trDetail.Visible = true;
		}
	}


	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// 決済種別情報をそのままセッションへセット
		Session[Constants.SESSIONPARAM_KEY_PAYMENT_INFO] = ViewState[Constants.SESSIONPARAM_KEY_PAYMENT_INFO];

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PAYMENT_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);

	}


	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, System.EventArgs e)
	{
		// 決済種別情報をそのままセッションへセット
		Session[Constants.SESSIONPARAM_KEY_PAYMENT_INFO] = ViewState[Constants.SESSIONPARAM_KEY_PAYMENT_INFO];

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT;

		// 登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PAYMENT_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COPY_INSERT);

	}


	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		// 変数宣言
		bool blRoolBack = false;
		int iUpdated = 0;

		// 削除
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			// トランザクション開始
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			// 決済種別情報取得
			Hashtable htPayment = (Hashtable)ViewState[Constants.SESSIONPARAM_KEY_PAYMENT_INFO];

			// 決済種別情報削除
			iUpdated = this.DeletePayment(htPayment, sqlAccessor);
			if (iUpdated <= 0)
			{
				blRoolBack = true;
			}

			// 決済種別手数料情報削除
			iUpdated = this.DeletePaymentPrice(htPayment, sqlAccessor);
			if (iUpdated <= 0)
			{
				blRoolBack = true;
			}

			// 削除処理件数が0以下の場合があった場合
			if (blRoolBack)
			{
				// トランザクション取り消し
				sqlAccessor.RollbackTransaction();
			}
			else
			{
				// トランザクション確定
				sqlAccessor.CommitTransaction();
			}
		}

		// フロント系サイトを最新情報へ更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.Payment).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PAYMENT_LIST);
	}


	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 変数宣言
		bool blRoolBack = false;
		int iUpdated = 0;

		// 登録
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			// トランザクション開始
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			// 決済種別情報取得
			Hashtable htPayment = (Hashtable)ViewState[Constants.SESSIONPARAM_KEY_PAYMENT_INFO];

			// 新決済種別ID取得
			string strPaymentId = null;
			using (SqlStatement sqlStatement = new SqlStatement("Payment", "GetNewPaymentId"))
			{
				DataView dv = sqlStatement.SelectSingleStatement(sqlAccessor, htPayment);
				strPaymentId = (string)dv[0][Constants.FIELD_PAYMENT_PAYMENT_ID];
			}
			htPayment[Constants.FIELD_PAYMENT_PAYMENT_ID] = strPaymentId;							// 決済種別ID

			// 決済種別情報登録
			iUpdated = this.InsertPayment(htPayment, sqlAccessor);
			if (iUpdated <= 0)
			{
				blRoolBack = true;
			}

			// 決済種別手数料情報取得
			ArrayList alPaymentPrice = (ArrayList)htPayment[Constants.SESSIONPARAM_KEY_PAYMENT_PRICE_INFO];
			for (int iLoop = 0; iLoop < alPaymentPrice.Count; iLoop++)
			{
				// 決済種別手数料情報設定
				Hashtable htPaymentPrice = (Hashtable)alPaymentPrice[iLoop];
				htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_SHOP_ID, htPayment[Constants.FIELD_PAYMENT_SHOP_ID]);			// 店舗ID
				htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_ID, htPayment[Constants.FIELD_PAYMENT_PAYMENT_ID]);		// 決済種別ID
				htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_LAST_CHANGED, htPayment[Constants.FIELD_PAYMENT_LAST_CHANGED]);	// 最終更新者

				// 決済種別手数料情報登録
				iUpdated = this.InsertPaymentPrice(htPaymentPrice, sqlAccessor);
				if (iUpdated <= 0)
				{
					blRoolBack = true;
				}
			}

			// 削除処理件数が0以下の場合があった場合
			if (blRoolBack)
			{
				// トランザクション取り消し
				sqlAccessor.RollbackTransaction();
			}
			else
			{
				// トランザクション確定
				sqlAccessor.CommitTransaction();
			}
		}

		// フロント系サイトを最新情報へ更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.Payment).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PAYMENT_LIST);
	}


	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateTop_Click(object sender, System.EventArgs e)
	{
		// 変数宣言
		bool blRoolBack = false;
		int iUpdated = 0;

		// 更新
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			// トランザクション開始
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			// 決済種別情報取得
			Hashtable htPayment = (Hashtable)ViewState[Constants.SESSIONPARAM_KEY_PAYMENT_INFO];

			// 決済種別情報更新
			iUpdated = this.UpdatePayment(htPayment, sqlAccessor);
			if (iUpdated <= 0)
			{
				blRoolBack = true;
			}

			// 決済種別手数料情報の削除
			// 決済種別手数料情報の更新処理は、以前の決済種別手数料情報の削除→追加といった処理を行う｡Update文は実行しない
			iUpdated = this.DeletePaymentPrice(htPayment, sqlAccessor);
			if (iUpdated <= 0)
			{
				blRoolBack = true;
			}

			// 決済種別手数料情報取得
			ArrayList alPaymentPrice = (ArrayList)htPayment[Constants.SESSIONPARAM_KEY_PAYMENT_PRICE_INFO];
			for (int iLoop = 0; iLoop < alPaymentPrice.Count; iLoop++)
			{
				// 決済種別手数料情報設定
				Hashtable htPaymentPrice = (Hashtable)alPaymentPrice[iLoop];
				htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_SHOP_ID, htPayment[Constants.FIELD_PAYMENT_SHOP_ID]);			// 店舗ID
				htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_PAYMENT_ID, htPayment[Constants.FIELD_PAYMENT_PAYMENT_ID]);		// 決済種別ID
				htPaymentPrice.Add(Constants.FIELD_PAYMENTPRICE_LAST_CHANGED, htPayment[Constants.FIELD_PAYMENT_LAST_CHANGED]);	// 最終更新者

				// 決済種別手数料情報登録
				iUpdated = this.InsertPaymentPrice(htPaymentPrice, sqlAccessor);
				if (iUpdated <= 0)
				{
					blRoolBack = true;
				}
			}

			// 削除処理件数が0以下の場合があった場合
			if (blRoolBack)
			{
				// トランザクション取り消し
				sqlAccessor.RollbackTransaction();
			}
			else
			{
				// トランザクション確定
				sqlAccessor.CommitTransaction();
			}
		}

		// フロント系サイトを最新情報へ更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.Payment).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PAYMENT_LIST);
	}


	/// <summary>
	/// 決済種別情報のDELETE処理
	/// </summary>
	/// <param name="htPayment">決済種別情報</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <returns>処理件数</returns>
	private int DeletePayment(Hashtable htPayment, SqlAccessor sqlAccessor)
	{
		// 変数宣言
		int iResult = 0;

		// 決済種別情報削除
		using (SqlStatement sqlStatement = new SqlStatement("Payment", "DeletePayment"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PAYMENT_SHOP_ID,
				(string)htPayment[Constants.FIELD_PAYMENT_SHOP_ID]);	// 店舗ID
			htInput.Add(Constants.FIELD_PAYMENT_PAYMENT_ID,
				(string)htPayment[Constants.FIELD_PAYMENT_PAYMENT_ID]);	// 決済種別ID

			iResult = sqlStatement.ExecStatement(sqlAccessor, htInput);
		}

		return iResult;
	}


	/// <summary>
	/// 決済種別手数料情報のDELETE処理
	/// </summary>
	/// <param name="htPayment">決済種別情報</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <returns>処理件数</returns>
	private int DeletePaymentPrice(Hashtable htPayment, SqlAccessor sqlAccessor)
	{
		// 変数宣言
		int iResult = 0;

		// 決済種別手数料情報削除
		using (SqlStatement sqlStatement = new SqlStatement("Payment", "DeletePaymentPrice"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PAYMENT_SHOP_ID,
				(string)htPayment[Constants.FIELD_PAYMENT_SHOP_ID]);	// 店舗ID
			htInput.Add(Constants.FIELD_PAYMENT_PAYMENT_ID,
				(string)htPayment[Constants.FIELD_PAYMENT_PAYMENT_ID]);	// 決済種別ID

			iResult = sqlStatement.ExecStatement(sqlAccessor, htInput);
		}

		return iResult;
	}


	/// <summary>
	/// 決済種別情報のINSERT処理
	/// </summary>
	/// <param name="htPayment">決済種別情報</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <returns>処理件数</returns>
	private int InsertPayment(Hashtable htPayment, SqlAccessor sqlAccessor)
	{
		// 変数宣言
		int iResult = 0;

		// 決済種別情報登録
		using (SqlStatement sqlStatement = new SqlStatement("Payment", "InsertPayment"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PAYMENT_SHOP_ID,
				(string)htPayment[Constants.FIELD_PAYMENT_SHOP_ID]);				// 店舗ID
			htInput.Add(Constants.FIELD_PAYMENT_PAYMENT_ID,
				(string)htPayment[Constants.FIELD_PAYMENT_PAYMENT_ID]);				// 決済種別ID
			htInput.Add(Constants.FIELD_PAYMENT_PAYMENT_NAME,
				(string)htPayment[Constants.FIELD_PAYMENT_PAYMENT_NAME]);			// 決済種別名
			htInput.Add(Constants.FIELD_PAYMENT_PAYMENT_NAME,
				(string)htPayment[Constants.FIELD_PAYMENT_PAYMENT_NAME_MOBILE]);	// モバイル用決済種別名
			htInput.Add(Constants.FIELD_PAYMENT_PAYMENT_PRICE_KBN,
				(string)htPayment[Constants.FIELD_PAYMENT_PAYMENT_PRICE_KBN]);		// 決済手数料区分q
			htInput.Add(Constants.FIELD_PAYMENT_LAST_CHANGED,
				(string)htPayment[Constants.FIELD_PAYMENT_LAST_CHANGED]);			// 最終更新者

			iResult = sqlStatement.ExecStatement(sqlAccessor, htInput);
		}

		return iResult;
	}


	/// <summary>
	/// 決済種別手数料情報のINSERT処理
	/// </summary>
	/// <param name="htPaymentPrice">決済種別手数料情報</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <returns>処理件数</returns>
	private int InsertPaymentPrice(Hashtable htPaymentPrice, SqlAccessor sqlAccessor)
	{
		// 変数宣言
		int iResult = 0;

		// 決済種別手数料情報登録
		using (SqlStatement sqlStatement = new SqlStatement("Payment", "InsertPaymentPrice"))
		{
			iResult = sqlStatement.ExecStatement(sqlAccessor, htPaymentPrice);
		}

		return iResult;
	}


	/// <summary>
	/// 決済種別情報のUPDATE処理
	/// </summary>
	/// <param name="htPayment">決済種別情報</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	/// <returns>処理件数</returns>
	private int UpdatePayment(Hashtable htPayment, SqlAccessor sqlAccessor)
	{
		// 変数宣言
		int iResult = 0;

		// 決済種別情報更新
		using (SqlStatement sqlStatement = new SqlStatement("Payment", "UpdatePayment"))
		{
			iResult = sqlStatement.ExecStatement(sqlAccessor, htPayment);
		}

		return iResult;
	}

	/// <summary>
	/// 有効金額文字列作成
	/// </summary>
	/// <param name="drvPayment"></param>
	/// <returns></returns>
	protected string CreateUsablePriceString(object objUsablePrice)
	{
		return ((objUsablePrice != DBNull.Value) && (objUsablePrice != null)) ? objUsablePrice.ToPriceString(true) : "-";
	}

	/// <summary>決済種別翻訳情報</summary>
	protected NameTranslationSettingModel[] PaymentTranslationData
	{
		get { return (NameTranslationSettingModel[])ViewState["payment_translation_data"]; }
		set { ViewState["payment_translation_data"] = value; }
	}
}

