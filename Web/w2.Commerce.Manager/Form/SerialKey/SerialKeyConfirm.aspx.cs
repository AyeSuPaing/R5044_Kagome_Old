/*
=========================================================================================================
  Module      : シリアルキー情報確認ページ処理(SerialKeyConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using System.Collections;
using System.Data;
using System.Web;
using w2.Domain.SerialKey;

public partial class Form_SerialKey_SerialKeyConfirm : SerialKeyPage
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
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			this.SerialKey = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY]);
			this.ProductId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID]);

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

					// シリアルキー情報取得
					{
						this.SerialKeyMaster = (Hashtable)Session[Constants.SESSIONPARAM_KEY_SERIALKEY_INFO];

						// シリアルキーの整合性チェック
						if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
							 && ((string)Request[Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY] != (string)GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_SERIAL_KEY))
							 && ((string)Request[Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID] != (string)GetKeyValue(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_PRODUCT_ID)))
						{
							Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_POPUP_IRREGULAR_PARAMETER_ERROR);
							Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
						}
					}
					break;

				//------------------------------------------------------
				// 詳細画面表示処理
				//------------------------------------------------------
				case Constants.ACTION_STATUS_DETAIL:
				case Constants.ACTION_STATUS_COMPLETE:

					// セッションクリア
					Session[Constants.SESSIONPARAM_KEY_SERIALKEY_INFO] = null;

					var serialKeyData = new SerialKeyService().Get(this.SerialKey, this.ProductId);
					if (serialKeyData == null)
					{
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}
					serialKeyData.DataSource[Constants.FIELD_PRODUCTVARIATION_V_ID] = serialKeyData.VId;
					this.SerialKeyMaster = serialKeyData.DataSource;
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
		if (this.ActionStatus == Constants.ACTION_STATUS_INSERT ||
			this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
		{
			btnInsertTop.Visible = true;
			btnInsertBottom.Visible = true;
			trConfirm.Visible = true;
		}
		// 更新？
		else if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			btnUpdateTop.Visible = true;
			btnUpdateBottom.Visible = true;
			trConfirm.Visible = true;
		}
		// 登録/更新完了?
		else if (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE)
		{
			lMessage.Text = (string)Session[Constants.SESSION_KEY_ERROR_MSG];
			divComp.Visible = true;
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
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = null;

		// 編集画面へ
		Response.Redirect(CreateSerialKeyRegistUrl(Request[Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY], Request[Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID], Constants.ACTION_STATUS_UPDATE));
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
		Response.Redirect(CreateSerialKeyRegistUrl(Request[Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY], Request[Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID], Constants.ACTION_STATUS_COPY_INSERT));
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		new SerialKeyService().Delete(this.SerialKey, this.ProductId);
		Response.Redirect(CreateSerialKeyListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_SERIALKEY_SEARCH_INFO]));
	}

	/// <summary>
	/// 登録する/更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertUpdate_Click(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// シリアルキー情報取得
		//------------------------------------------------------
		var serialKeyData = new SerialKeyModel((Hashtable)Session[Constants.SESSIONPARAM_KEY_SERIALKEY_INFO]);
		
		// シリアルキーの整合性チェック
		if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
			&& ((string)Request[Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY] != serialKeyData.SerialKey)
			&& ((string)Request[Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID] != serialKeyData.ProductId))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_POPUP_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		//------------------------------------------------------
		// DB登録/更新
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			try
			{
				// トランザクション開始
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				// シリアルキー情報登録
				InsertUpdateSerialKey(this.ActionStatus, serialKeyData, sqlAccessor);

				// トランザクションコミット
				sqlAccessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				// ロールバック
				sqlAccessor.RollbackTransaction();

				// 例外をスローする
				throw ex;
			}
		}

		// 登録・更新完了メッセージをセット
		Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SERIALKEY_REGIST_UPDATE_SUCCESS);

		//------------------------------------------------------
		// 詳細表示画面へ遷移
		//------------------------------------------------------
		StringBuilder urlRedirect = new StringBuilder();
		urlRedirect.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_SERIALKEY_CONFIRM);
		urlRedirect.Append("?").Append(Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY).Append("=").Append(HttpUtility.UrlEncode(serialKeyData.SerialKey));
		urlRedirect.Append("&").Append(Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode(serialKeyData.ProductId));
		urlRedirect.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_COMPLETE);
		Response.Redirect(urlRedirect.ToString());
	}

	/// <summary>
	/// シリアルキー情報登録/更新
	/// </summary>
	/// <param name="actionStatus">アクションステータス</param>
	/// <param name="serialKeyData">シリアルキー情報</param>
	/// <param name="sqlAccessor">SQLアクセサ</param>
	protected void InsertUpdateSerialKey(
		string actionStatus,
		SerialKeyModel serialKeyData,
		SqlAccessor sqlAccessor)
	{
		serialKeyData.LastChanged = this.LoginOperatorName;

		if (actionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			new SerialKeyService().Update(serialKeyData, sqlAccessor);
		}
		else
		{
			new SerialKeyService().Insert(serialKeyData, sqlAccessor);
		}
	}

	/// <summary>
	/// 一覧へ戻るボタン処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBackList_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateSerialKeyListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_SERIALKEY_SEARCH_INFO]));
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
				Response.Redirect(CreateSerialKeyListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_SERIALKEY_SEARCH_INFO]));
				break;

			// 新規、コピー新規、更新
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:

				// 編集画面へ
				Response.Redirect(CreateSerialKeyRegistUrl(this.SerialKey, this.ProductId, this.ActionStatus));
				break;
		}
	}

	/// <summary>シリアルキー</summary>
	protected string SerialKey
	{
		get { return (string)ViewState["SerialKey"]; }
		private set { ViewState["SerialKey"] = value; }
	}
	/// <summary>商品ID</summary>
	protected string ProductId
	{
		get { return (string)ViewState["ProductId"]; }
		private set { ViewState["ProductId"] = value; }
	}
	/// <summary>シリアルキーマスタ</summary>
	protected Hashtable SerialKeyMaster { get; set; }
}

