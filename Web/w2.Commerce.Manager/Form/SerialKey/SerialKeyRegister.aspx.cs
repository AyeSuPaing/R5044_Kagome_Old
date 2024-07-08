/*
=========================================================================================================
  Module      : シリアルキー情報登録ページ処理(SerialKeyRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using w2.App.Common.Product;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.SerialKey;
using w2.Domain.User;

public partial class Form_SerialKey_SerialKeyRegister : SerialKeyPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender">パラメータの説明を記述</param>
	/// <param name="e">パラメータの説明を記述</param>
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
			InitializeComponents(this.ActionStatus);

			//------------------------------------------------------
			// 処理区分チェック
			//------------------------------------------------------
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			//------------------------------------------------------
			// セッションにあれば値セット
			//------------------------------------------------------
			if (Session[Constants.SESSIONPARAM_KEY_SERIALKEY_INFO] != null)
			{
				this.SerialKeyMaster = (Hashtable)Session[Constants.SESSIONPARAM_KEY_SERIALKEY_INFO];
			}

			//------------------------------------------------------
			// 各マスタ取得処理
			//------------------------------------------------------
			switch (this.ActionStatus)
			{
				//------------------------------------------------------
				// 新規登録
				//------------------------------------------------------
				case Constants.ACTION_STATUS_INSERT:

					if (this.SerialKeyMaster == null)
					{
						// 初期値設定
						this.SerialKeyMaster = new Hashtable
						{
							{ Constants.FIELD_SERIALKEY_STATUS, Constants.FLG_SERIALKEY_STATUS_NOT_RESERVED }
						};
					}
					break;

				//------------------------------------------------------
				// コピー新規登録 OR 編集
				//------------------------------------------------------
				case Constants.ACTION_STATUS_COPY_INSERT:
				case Constants.ACTION_STATUS_UPDATE:

					// シリアルキー情報取得・値チェック
					if (this.SerialKeyMaster == null)
					{
						var serialKeyData = new SerialKeyService().Get(this.SerialKey, this.ProductId);
						if (serialKeyData == null)
						{
							Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
							Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
						}
						this.SerialKeyMaster = serialKeyData.DataSource;
						var dateDelivered = StringUtility.ToEmpty(serialKeyData.DateDelivered);
						if (string.IsNullOrEmpty(dateDelivered) == false)
						{
							ucDisplayPeriod.SetStartDate(DateTime.Parse(dateDelivered));
					}
					}

					break;

				default:
					// エラーページへ
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					break;
			}

			if (this.IsBackFromConfirm
				&& (string.IsNullOrEmpty(GetDate(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_DATE_DELIVERED).ToString()) == false))
			{
				var date = StringUtility.ToEmpty(GetDate(this.SerialKeyMaster, Constants.FIELD_SERIALKEY_DATE_DELIVERED));
				ucDisplayPeriod.SetStartDate(DateTime.Parse(date));
				Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = null;
			}

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			DataBind();

			//------------------------------------------------------
			// 初期の状態が”未引当”の時、一部の入力を制御
			//------------------------------------------------------
			ChangeEnabledByDdlStatus();
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	/// <param name="actionStatus"></param>
	private void InitializeComponents(string actionStatus)
	{
		// 新規登録？
		if (actionStatus == Constants.ACTION_STATUS_INSERT)
		{
			trRegister.Visible = true;
			trSerialKeyRegister.Visible = true;
		}
		else if (actionStatus == Constants.ACTION_STATUS_COPY_INSERT)
		{
			trRegister.Visible = true;
			trSerialKeyRegister.Visible = true;
		}
		else if (actionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			trEdit.Visible = true;
			trSerialKeyEdit.Visible = true;
		}
		
		// 状態ドロップダウン作成
		ddlStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_SERIALKEY, Constants.FIELD_SERIALKEY_STATUS));
	}

	#region 画面入力値取得系処理
	/// <summary>
	///  シリアルキーを画面から取得
	/// </summary>
	/// <returns>シリアルキー</returns>
	private string GetSerialKeyFromInput()
	{
		string serialKey = null;

		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				return tbSerialKey.Text.Trim();

			case Constants.ACTION_STATUS_UPDATE:
				return lbSerialKey.Text;
		}
		return serialKey;
	}

	/// <summary>
	///  シリアルキー情報を画面から取得する
	/// </summary>
	/// <returns>シリアルキー情報</returns>
	private Hashtable GetProductFromInput()
	{
		string serialKey = GetSerialKeyFromInput();

		//------------------------------------------------------
		// パラメタ格納
		//------------------------------------------------------
		Hashtable serialKeyObject = new Hashtable();
		// シリアルキー
		serialKeyObject.Add(Constants.FIELD_SERIALKEY_SERIAL_KEY, SerialKeyUtility.EncryptSerialKey(serialKey));
		serialKeyObject.Add(Constants.FIELD_SERIALKEY_SERIAL_KEY + "_chk", serialKey);
		// 商品ID
		serialKeyObject.Add(Constants.FIELD_SERIALKEY_PRODUCT_ID, tbProductId.Text);
		serialKeyObject.Add(Constants.FIELD_SERIALKEY_PRODUCT_ID + "_old", this.ProductId);
		// バリエーションID（商品ID含む）
		serialKeyObject.Add(Constants.FIELD_SERIALKEY_VARIATION_ID, tbProductId.Text + tbVariationId.Text);
		// バリエーションID（商品ID含まない）
		serialKeyObject.Add(Constants.FIELD_PRODUCTVARIATION_V_ID, tbVariationId.Text);
		// 状態
		serialKeyObject.Add(Constants.FIELD_SERIALKEY_STATUS, ddlStatus.SelectedValue);
		// 注文ID
		serialKeyObject.Add(Constants.FIELD_SERIALKEY_ORDER_ID, tbOrderId.Text);
		// 注文商品枝番
		serialKeyObject.Add(Constants.FIELD_SERIALKEY_ORDER_ITEM_NO, tbOrderItemNo.Text);
		// ユーザーID
		serialKeyObject.Add(Constants.FIELD_SERIALKEY_USER_ID, tbUserId.Text);
		// 引渡日
		if (string.IsNullOrEmpty(ucDisplayPeriod.StartDateTimeString))
		{
			serialKeyObject.Add(Constants.FIELD_SERIALKEY_DATE_DELIVERED, null);
		}
		else
		{
			serialKeyObject.Add(
				Constants.FIELD_SERIALKEY_DATE_DELIVERED,
				ucDisplayPeriod.StartDateTimeString);
		}
		// 有効フラグ
		serialKeyObject.Add(Constants.FIELD_SERIALKEY_VALID_FLG, cbValidFlg.Checked ? Constants.FLG_SERIALKEY_VALID_FLG_VALID : Constants.FLG_SERIALKEY_VALID_FLG_INVALID);
		// 回数
		serialKeyObject.Add(Constants.FIELD_SERIALKEY_DOWNLOAD_COUNT, 0);
		serialKeyObject.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);

		return serialKeyObject;
	}
	#endregion

	#region 入力チェック系処理
	/// <summary>
	///  シリアルキー情報の入力チェック
	/// </summary>
	/// <param name="serialKeyObject">シリアルキー情報</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckInputSerialKeyData(Hashtable serialKeyObject)
	{
		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		var errorMessages = new StringBuilder();

		var serialKey = (string)serialKeyObject[Constants.FIELD_SERIALKEY_SERIAL_KEY];
		var productId = (string)serialKeyObject[Constants.FIELD_SERIALKEY_PRODUCT_ID];
		var productIdOld = (string)serialKeyObject[Constants.FIELD_SERIALKEY_PRODUCT_ID + "_old"];
		// 入力チェック＆重複チェック
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				errorMessages.Append(Validator.Validate("SerialKeyRegist", serialKeyObject));

				// 重複チェック
				if (errorMessages.Length == 0)
				{
					var count = new SerialKeyService().CountSerialKeyForDuplicationCheck(serialKey, productId);
					if (count > 0)
					{
						errorMessages.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_DUPLICATION).Replace("@@ 1 @@", ReplaceTag("@@SerialKey.serial_key.name@@")));
					}
				}

				break;

			case Constants.ACTION_STATUS_UPDATE:
				errorMessages.Append(Validator.Validate("SerialKeyModify", serialKeyObject));

				// 重複チェック
				if (errorMessages.Length == 0)
				{
					var count = new SerialKeyService().CountSerialKeyForDuplicationCheck(serialKey, productId, productIdOld);
					if (count > 0)
					{
						errorMessages.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_DUPLICATION).Replace("@@ 1 @@", ReplaceTag("@@SerialKey.product_id.name@@")));
					}
				}
				
				break;
		}

		// 商品チェック
		if (errorMessages.Length == 0)
		{
			var count = new ProductService().CountProductView(
				(string)serialKeyObject[Constants.FIELD_PRODUCT_SHOP_ID],
				(string)serialKeyObject[Constants.FIELD_SERIALKEY_PRODUCT_ID],
				(string)serialKeyObject[Constants.FIELD_SERIALKEY_VARIATION_ID]);
			if (count == 0)
			{
				errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_NOPRODUCT));
			}
		}
		// ユーザチェック
		if ((errorMessages.Length == 0) && ((string)serialKeyObject[Constants.FIELD_SERIALKEY_USER_ID] != ""))
		{
			var count = new UserService().Count((string)serialKeyObject[Constants.FIELD_SERIALKEY_USER_ID]);
			if (count == 0)
			{
				errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_NOUSER));
			}
		}
		// 注文チェック
		if ((errorMessages.Length == 0) && ((string)serialKeyObject[Constants.FIELD_SERIALKEY_ORDER_ID] != ""))
		{
			var count = new OrderService().Count((string)serialKeyObject[Constants.FIELD_SERIALKEY_ORDER_ID]);
			if (count == 0)
			{
				errorMessages.Append(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_ORDER_INFORMATION_WAS_FOUND));
			}
		}

		return errorMessages.ToString();
	}
	#endregion

	#region ボタンクリック系処理
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
		//------------------------------------------------------
		// リダイレクト処理
		//------------------------------------------------------
		this.SerialKeyMaster = (Hashtable)Session[Constants.SESSIONPARAM_KEY_SERIALKEY_INFO];

		switch (this.ActionStatus)
		{
			// 新規
			case Constants.ACTION_STATUS_INSERT:

				// 一覧画面へ
				Response.Redirect(CreateSerialKeyListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_SERIALKEY_SEARCH_INFO]));
				break;

			// 更新・コピー新規
			case Constants.ACTION_STATUS_UPDATE:
			case Constants.ACTION_STATUS_COPY_INSERT:

				// 詳細画面へ
				Response.Redirect(CreateSerialKeyDetailUrl(this.SerialKey, this.ProductId));
				break;
		}
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 値取得・セッションへセット
		//------------------------------------------------------
		Hashtable serialKeyObject = GetProductFromInput();
		
		Session[Constants.SESSIONPARAM_KEY_SERIALKEY_INFO] = serialKeyObject;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = 1;
		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		string errorMessages = CheckInputSerialKeyData(serialKeyObject);
		if (errorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// 画面遷移
		//------------------------------------------------------
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = this.ActionStatus;

		// シリアルキー情報確認ページへ遷移		
		StringBuilder urlRedirect = new StringBuilder();
		urlRedirect.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_SERIALKEY_CONFIRM);
		urlRedirect.Append("?").Append(Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY).Append("=").Append(HttpUtility.UrlEncode(this.SerialKey));
		urlRedirect.Append("&").Append(Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode(this.ProductId));
		urlRedirect.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(this.ActionStatus);
		Response.Redirect(urlRedirect.ToString());
	}
	#endregion

	/// <summary>
	/// 状態ドロップダウンリスト変更時処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlStatus_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		ChangeEnabledByDdlStatus();
	}

	/// <summary>
	/// 状態によって入力を制御
	/// </summary>
	private void ChangeEnabledByDdlStatus()
	{
		// 状態”未引当”が選択された時、一部の項目を入力不可に
		ucDisplayPeriod.Disabled =
			(ddlStatus.SelectedValue == Constants.FLG_SERIALKEY_STATUS_NOT_RESERVED);
		tbOrderId.Enabled = tbOrderItemNo.Enabled = tbUserId.Enabled
				= (ddlStatus.SelectedValue != Constants.FLG_SERIALKEY_STATUS_NOT_RESERVED);
	}

	#region プロパティ
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
	protected Hashtable SerialKeyMaster { get; private set; }

	/// <summary>確認画面から戻ってきたか</summary>
	protected bool IsBackFromConfirm
	{
		get { return (Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] != null); }
	}
	#endregion
}