/*
=========================================================================================================
  Module      : モール連携 設定編集ページ処理(MallConfig.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Mall.Yahoo;
using w2.App.Common.Mall.Yahoo.Procedures;
using w2.App.Common.Option;
using w2.Common.Helper;
using w2.Common.Helper.Attribute;
using w2.Common.Web;
using w2.Domain.MallCooperationSetting;

public partial class Form_MallLiaise_MallConfig : MallPage
{
	/// <summary>バリデーションチェック区分</summary>
	private enum CheckKbn
	{
		/// <summary>登録</summary>
		[EnumTextName("MallConfigRegist")]
		MallConfigRegist,
		/// <summary>更新/コピー新規登録</summary>
		[EnumTextName("MallConfig")]
		MallConfig,
	}
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.IsSftpPrivateKeyFileUploaded = false;

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeComponent();

			//------------------------------------------------------
			// 画面設定
			//------------------------------------------------------
			// 新規登録（コピー新規登録）
			if ((Request[Constants.REQUEST_KEY_ACTION_STATUS] == Constants.ACTION_STATUS_INSERT)
				|| (this.MallId == Constants.FLG_USER_MALL_ID_OWN_SITE))
			{
				tbMallId.Enabled = true;
				btnInsertTop.Visible = btnInsertButtom.Visible = true;
				btnDeleteTop.Visible = btnDeleteButtom.Visible = false;
				btnCopyInsertTop.Visible = btnCopyInsertButtom.Visible = false;
				btnUpdateTop.Visible = btnUpdateButtom.Visible = false;
			}
			// 詳細・編集
			else
			{
				tbMallId.Enabled = false;
				btnInsertTop.Visible = btnInsertButtom.Visible = false;
				btnDeleteTop.Visible = btnDeleteButtom.Visible = true;
				btnCopyInsertTop.Visible = btnCopyInsertButtom.Visible = true;
				btnUpdateTop.Visible = btnUpdateButtom.Visible = true;
				trDateCreated.Visible = true;
				trDateChanged.Visible = true;
				trLastChanged.Visible = true;

				// モール連携設定取得
				DataRowView drvMallCooperationSetting = GetMallCooperationSetting(this.LoginOperatorShopId, this.MallId);

				tbMallId.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID];			// モールＩＤ
				SetDropDownValue(ddlMallKbn, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN]);	// モール区分
				tbMallName.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME];		// モール名
				foreach (ListItem li in ddlMallExhibitsConfig.Items)
				{
					li.Selected = (li.Value == (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG]);  // モール出品設定
				}
				tbTgtAddr.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_TGT_MAIL_ADDR];		// 対象メールアドレス
				tbPopServer.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_POP_SERVER];		// 受信ＰＯＰサーバ
				tbPopPort.Text = drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_POP_PORT].ToString();			// 受信ＰＯＰポート
				tbPopUserName.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_POP_USER_NAME];	// 受信ＰＯＰユーザ名
				tbPopPassword.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_POP_PASSWORD];	// 受信ＰＯＰパスワード
				cbPopApop.Checked = ((string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_POP_APOP_FLG] == "1");	// 受信ＡＰＯＰフラグ
				tbFtpHost.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_HOST];			// ＦＴＰホスト名
				tbFtpUserName.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_USER_NAME];	// ＦＴＰユーザ名
				tbFtpPassword.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_PASSWORD];	// ＦＴＰパスワード
				tbFtpUploadDir.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_UPLOAD_DIR];	// ＦＴＰアップロード先ディレクトリ
				tbSftpHost.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_HOST];			// ＳＦＴＰホスト名
				tbSftpUserName.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_USER_NAME];	// ＳＦＴＰユーザ名
				tbSftpPassPhrase.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_PASS_PHRASE];	// ＳＦＴＰパスフレーズ
				tbSftpPort.Text = drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_PORT].ToString(); //ポート番号

				// SFTP秘密鍵ファイルパスの判定
				hfSftpPrivateKeyFilePath.Value = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_PRIVATE_KEY_FILE_PATH];
				if ((string.IsNullOrEmpty(hfSftpPrivateKeyFilePath.Value) == false) && (File.Exists(hfSftpPrivateKeyFilePath.Value)))
				{
					this.IsSftpPrivateKeyFileUploaded = true;
				}

				if (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO)
				{
					tbYahooApiClientId.Text = StringUtility.ToEmpty(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_CLIENT_ID]);
					tbYahooApiClientSecret.Text = StringUtility.ToEmpty(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_CLIENT_SECRET]);
					tbYahooApiSellerId.Text = StringUtility.ToEmpty(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_SELLER_ID]);
					tbYahooApiAccessToken.Text = StringUtility.ToEmpty(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_ACCESS_TOKEN]);
					tbYahooApiAccessTokenExpirationDatetime.Text = StringUtility.ToEmpty(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_ACCESS_TOKEN_EXPIRATION_DATETIME]);
					tbYahooApiRefreshToken.Text = StringUtility.ToEmpty(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_REFRESH_TOKEN]);
					var yahooApiRefreshTokenExpirationDatetime = StringUtility.ToEmpty(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_REFRESH_TOKEN_EXPIRATION_DATETIME]);
					tbYahooApiRefreshTokenExpirationDatetime.Text = yahooApiRefreshTokenExpirationDatetime;
					lTokenExpirationAlert.Text = CheckIfRefreshTokenWillBeExpiredWithinOneDay(yahooApiRefreshTokenExpirationDatetime);
					tbYahooApiPublicKey.Text = StringUtility.ToEmpty(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_PUBLIC_KEY]);
					tbYahooApiPublicKeyVersion.Text = StringUtility.ToEmpty(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_PUBLIC_KEY_VERSION]);
					tbYahooApiPublicKeyAuthorizedDatetime.Text = StringUtility.ToEmpty(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_LAST_PUBLIC_KEY_AUTHORIZED_AT]);
				}
				if (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN)
				{
					MallSmtpServerSettingRakuten msssr = new MallSmtpServerSettingRakuten((string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_OTHER_SETTING]);
					tbSmtpServerName.Text = msssr.SmtpServerName;
					tbSmtpServerPort.Text = msssr.SmtpServerPort;
					tbSmtpServerUserName.Text = msssr.SmtpAuthId;
					tbSmtpServerPassword.Text = msssr.SmtpAuthPassword;
					tbRakutenStoreMailAddress.Text = msssr.RakutenStoreMailAddress;
					tbRakutenApiUserName.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_USER_NAME];
					tbRakutenApiShopUrl.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_SHOP_URL];
					tbRakutenApiServiceSecret.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_SERVICE_SECRET];
					tbRakutenApiLicenseKey.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_LICENSE_KEY];
				}

				if (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_ANDMALL)
				{
					tbAndMallTenantCode.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_TENANT_CODE];
					tbAndMallBaseStoreCode.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_BASE_STORE_CODE];
					tbAndMallShopNo.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_SHOP_NO];
					ddlCooperationId.Items.Clear();
					ddlCooperationId.Items.Add(new ListItem("", ""));
					foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLCOOPERATIONSETTING, Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_COOPERATION))
					{
						ddlCooperationId.Items.Add(li);
					}
					SetDropDownValue(ddlCooperationId, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_COOPERATION]);
					ddlVariationCooperationId.Items.Clear();
					ddlVariationCooperationId.Items.Add(new ListItem("", ""));
					foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLCOOPERATIONSETTING, Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_VARIATION_COOPERATION))
					{
						ddlVariationCooperationId.Items.Add(li);
					}
					SetDropDownValue(ddlVariationCooperationId, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_VARIATION_COOPERATION]);

					tbSiteCode.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_SITE_CODE];
					tbSignatureKey.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_SIGNATURE_KEY];
				}

				switch (ddlMallKbn.SelectedValue)
				{
					// モール区分毎の設定情報を取得する
					case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN:
						SetDropDownValue(ddlCnvidRtnNInsItemcsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_INS_ITEMCSV]);
						SetDropDownValue(ddlCnvidRtnNUpdItemcsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_UPD_ITEMCSV]);
						SetDropDownValue(ddlCnvidRtnNStkItemcsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_STK_ITEMCSV]);
						SetDropDownValue(ddlCnvidRtnVInsItemcsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_ITEMCSV]);
						SetDropDownValue(ddlCnvidRtnVInsSelectcsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_SELECTCSV]);
						SetDropDownValue(ddlCnvidRtnVUpdItemcsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_UPD_ITEMCSV]);
						SetDropDownValue(ddlCnvidRtnVStkSelectcsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_STK_SELECTCSV]);
						SetDropDownValue(ddlCnvidRtnItemCatcsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_ITEMCATCSV]);
						if (Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION)
						{
							tbRakutenSkuManagementIdOutputFormatForNormal.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_SKU_MANAGEMENT_ID_OUTPUT_FORMAT_FOR_NORMAL];
							tbRakutenSkuManagementIdOutputFormatForVariation.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_SKU_MANAGEMENT_ID_OUTPUT_FORMAT_FOR_VARIATION];
						}
						break;

					case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO:
						SetDropDownValue(ddlCnvidYhoAddDatacsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_DATACSV]);
						SetDropDownValue(ddlCnvidYhoAddQuantitycsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_STOCKCSV]);
						SetDropDownValue(ddlCnvidYhoDatacsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_UPD_DATACSV]);
						SetDropDownValue(ddlCnvidYhoQuantitycsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_STK_DATACSV]);
						SetDropDownValue(ddlCnvidYhoVAddDatacsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_DATACSV]);
						SetDropDownValue(ddlCnvidYhoVAddQuantitycsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_STOCKCSV]);
						SetDropDownValue(ddlCnvidYhoVDatacsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_UPD_DATACSV]);
						SetDropDownValue(ddlCnvidYhoVQuantitycsv, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_STK_DATACSV]);
						break;

					case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON:
						tbAmazonMerchantId.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_MERCHANT_ID];
						tbAmazonMarketPlaceId.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_MARKETPLACE_ID];
						tbAwsAccessKeyId.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_AWS_ACCESSKEY_ID];
						tbAmazonSecretKey.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_SECRET_KEY];
						tbMwsAuthToken.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_MWS_AUTHTOKEN];
						cbStockUpdateUseFlg.Checked =
							((string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG] == Constants.FLG_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG_VALID);
						break;

					case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_LOHACO:
						cbLohacoStockUpdateUseFlg.Checked =
							(StringUtility.ToEmpty(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG]) == Constants.FLG_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG_VALID);
						tbLohacoPrivateKey.Text = drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_LOHACO_API_PRIVATE_KEY].ToString();
						break;

					case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_NEXT_ENGINE:
						tbNextEngineStockStoreAccount.Text = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_NEXT_ENGINE_STOCK_STORE_ACCOUNT];
						tbNextEngineStockAuthKey.Text  = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_NEXT_ENGINE_STOCK_AUTH_KEY];
						break;

					case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_FACEBOOK:
						tbFacebookCatalogId.Text = StringUtility.ToEmpty(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_FACEBOOK_CATALOG_ID]);
						tbFacebookAccessToken.Text = StringUtility.ToEmpty(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_FACEBOOK_ACCESS_TOKEN]);
						break;
				}

				// メンテナンス期間
				var maintenanceDateFrom = StringUtility.ToEmpty(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_FROM]);
				var maintenanceDateTo = StringUtility.ToEmpty(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_TO]);

				if (string.IsNullOrEmpty(maintenanceDateFrom) == false)
				{
					ucDateTimePickerPeriod.SetStartDate(DateTime.Parse(maintenanceDateFrom));
				}
				if (string.IsNullOrEmpty(maintenanceDateTo) == false)
				{
					ucDateTimePickerPeriod.SetEndDate(DateTime.Parse(maintenanceDateTo));
				}

				cbVValidFlg.Checked = ((string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_VALID_FLG] == "1");	// 有効フラグ
				lDateCreated.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_DATE_CREATED],
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
				lDateChanged.Text = WebSanitizer.HtmlEncode(
					DateTimeUtility.ToStringForManager(
						drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_DATE_CHANGED],
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
				lLastChanged.Text = WebSanitizer.HtmlEncode(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_LAST_CHANGED]);
			}

			// モール用商品コンバータ設定表示切り替え
			trProductConvRakuten.Visible = (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN);
			trProductConvYahoo.Visible = (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO);

			// 完了メッセージ表示
			if ((bool)this.IsComplete)
			{
				trCompMessage.Visible = true;
				this.IsComplete = false;
			}
		}
	}

	/// <summary>
	/// ドロップダウン値セット
	/// </summary>
	/// <param name="dropDownList">ドロップダウンリスト</param>
	/// <param name="objValue">オブジェクト値</param>
	private void SetDropDownValue(DropDownList dropDownList, object objValue)
	{
		// ドロップダウンリストの値を選択する
		foreach (ListItem li in dropDownList.Items)
		{
			li.Selected = (li.Value == StringUtility.ToEmpty(objValue));
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponent()
	{
		//------------------------------------------------------
		// モール区分ドロップダウン作成
		//------------------------------------------------------
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLCOOPERATIONSETTING, Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN))
		{
			// Remove item of Facebook catalog api cooperation setting when option setting off
			if ((Constants.FACEBOOK_CATALOG_API_COOPERATION_OPTION_ENABLED == false)
				&& (li.Value == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_FACEBOOK)) continue;

			ddlMallKbn.Items.Add(li);
		}

		//------------------------------------------------------
		// モール出品設定ドロップダウン作成
		//------------------------------------------------------
		// モール連携設定情報取得
		DataView dvMallCooperationSetting = GetMallCooperationSettingAll(this.LoginOperatorShopId);

		// 未設定のモール出品設定値を判定して設定可能な一覧を構築する
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLCOOPERATIONSETTING, Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG))
		{
			if (dvMallCooperationSetting.Count != 0)
			{
				bool blMallExhibitsSettingFlg = false;
				foreach (DataRowView drvMallCooperationSetting in dvMallCooperationSetting)
				{
					// 「モール出品設定済み」かつ「自分のモールID」ではない場合、ドロップダウンリスト構築対象外とする
					if (li.Value == ((string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG])
						&& (drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID].ToString() != this.MallId))
					{
						blMallExhibitsSettingFlg = true;
						break;
					}
				}
				if (blMallExhibitsSettingFlg == false)
				{
					ddlMallExhibitsConfig.Items.Add(li);
				}
			}
			else
			{
				ddlMallExhibitsConfig.Items.Add(li);
			}
		}

		//------------------------------------------------------
		// 商品コンバータ設定取得
		//------------------------------------------------------
		// 商品コンバータ設定リスト初期化
		List<DropDownList> lProductConvSettings = new List<DropDownList>();
		lProductConvSettings.Add(ddlCnvidRtnNInsItemcsv);
		lProductConvSettings.Add(ddlCnvidRtnNUpdItemcsv);
		lProductConvSettings.Add(ddlCnvidRtnNStkItemcsv);
		lProductConvSettings.Add(ddlCnvidRtnVInsItemcsv);
		lProductConvSettings.Add(ddlCnvidRtnVInsSelectcsv);
		lProductConvSettings.Add(ddlCnvidRtnVUpdItemcsv);
		lProductConvSettings.Add(ddlCnvidRtnVStkSelectcsv);
		lProductConvSettings.Add(ddlCnvidRtnItemCatcsv);
		lProductConvSettings.Add(ddlCnvidYhoAddDatacsv);
		lProductConvSettings.Add(ddlCnvidYhoAddQuantitycsv);
		lProductConvSettings.Add(ddlCnvidYhoDatacsv);
		lProductConvSettings.Add(ddlCnvidYhoQuantitycsv);
		lProductConvSettings.Add(ddlCnvidYhoVAddDatacsv);
		lProductConvSettings.Add(ddlCnvidYhoVAddQuantitycsv);
		lProductConvSettings.Add(ddlCnvidYhoVDatacsv);
		lProductConvSettings.Add(ddlCnvidYhoVQuantitycsv);

		// 商品コンバータ設定情報取得
		DataView dvProductConvList = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallProductConverter", "GetAllProductConverter"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MALLPRDCNV_SHOP_ID, this.LoginOperatorShopId);

			dvProductConvList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
		// 商品コンバータ設定情報のリストを構築する
		foreach (DropDownList ddlProductConvSetting in lProductConvSettings)
		{
			ddlProductConvSetting.Items.Add("");
			foreach (DataRowView drvProductConvList in dvProductConvList)
			{
				ddlProductConvSetting.Items.Add(new ListItem(
					drvProductConvList[Constants.FIELD_MALLPRDCNV_ADTO_ID].ToString() + " : " + (string)drvProductConvList[Constants.FIELD_MALLPRDCNV_ADTO_NAME],
					drvProductConvList[Constants.FIELD_MALLPRDCNV_ADTO_ID].ToString()));
			}
		}
	}

	/// <summary>
	/// 戻る ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void goList(object sender, EventArgs e)
	{
		// 一覧へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MALL_LIAISE_LIST);
	}

	/// <summary>
	/// リフレッシュトークンの有効期限が切れる24H以下かどうか
	/// </summary>
	/// <param name="yahooApiRefreshTokenExpirationDatetime">リフレッシュトークン有効期限日時</param>
	/// <returns>リフレッシュトークンの有効期限が切れる24H以下かどうか</returns>
	private string CheckIfRefreshTokenWillBeExpiredWithinOneDay(string yahooApiRefreshTokenExpirationDatetime)
	{
		if (string.IsNullOrEmpty(yahooApiRefreshTokenExpirationDatetime)) return "";
		var nextDayAtThisTime = DateTime.Now.AddDays(1);
		var expiresInOneDay = DateTime.Parse(yahooApiRefreshTokenExpirationDatetime) < nextDayAtThisTime;
		var result = expiresInOneDay == false
			? ""
			: WebMessages.GetMessages(WebMessages.ERRMSG_MALL_CONFIG_YAHOO_API_TOKEN_EXPIRATION_ALERT);
		return result;
	}

	/// <summary>
	/// 更新する ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		// Yahooの場合、公開鍵認証情報をチェック
		if (CheckYahooApiPublicKeyInput() == false) return;

		// 画面より設定値を取得し、ＤＢ更新を行う
		UpdateMallCooperationSetting(SetMallCooperationSetting(CheckKbn.MallConfig.ToText()));

		//------------------------------------------------------
		// 登録画面遷移後、完了メッセージ表示
		//------------------------------------------------------
		this.IsComplete = true;
		Response.Redirect(CreateMallConfigUrl(this.MallId));
	}

	/// <summary>
	/// 登録する ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		// Yahooの場合、公開鍵認証情報をチェック
		if (CheckYahooApiPublicKeyInput() == false) return;

		// 画面より設定値を取得し、ＤＢ登録を行う
		using (SqlAccessor sqlAccesser = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallLiaise", "InsertMallCooperation"))
		{
			Hashtable htInput = SetMallCooperationSetting(CheckKbn.MallConfigRegist.ToText());
			sqlStatement.ExecStatementWithOC(sqlAccesser, htInput);
		}

		// モール出品設定リセット
		ResetMallExhibitsConfig();

		//------------------------------------------------------
		// 登録画面遷移後、完了メッセージ表示
		//------------------------------------------------------
		this.IsComplete = true;
		Response.Redirect(CreateMallConfigUrl(tbMallId.Text));
	}

	/// <summary>
	/// 公開鍵認証入力チェック
	/// </summary>
	/// <returns>エラーがなければtrue</returns>
	private bool CheckYahooApiPublicKeyInput()
	{
		// Yahoo!モールではない場合
		if (this.ddlMallKbn.SelectedValue != Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO) return true;

		var strPublicKey = tbYahooApiPublicKey.Text.Trim();
		var strPublicKeyVersion = tbYahooApiPublicKeyVersion.Text.Trim();
		var errorMessage = new YahooApiPublicKeyInput(strPublicKey, strPublicKeyVersion).Validate();

		if (string.IsNullOrEmpty(errorMessage)) return true;

		lPublicKeyAlert.Visible = true;
		lPublicKeyAlert.Text = errorMessage;
		return false;
	}

	/// <summary>
	/// パラメタセット
	/// </summary>
	/// <param name="checkKbn">対象エラーチェック区分</param>
	private Hashtable SetMallCooperationSetting(string checkKbn)
	{
		Hashtable htParam = new Hashtable();

		//------------------------------------------------------
		// 設定値格納
		//------------------------------------------------------
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID] = tbMallId.Text;					// モールＩＤ
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN] = ddlMallKbn.SelectedItem.Value;	// モール区分
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME] = tbMallName.Text;				// モール名
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG] = ddlMallExhibitsConfig.SelectedValue;  // モール出品設定
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_TGT_MAIL_ADDR] = tbTgtAddr.Text;				// 対象メールアドレス
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_POP_SERVER] = tbPopServer.Text;				// 受信ＰＯＰサーバ
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_POP_PORT] = tbPopPort.Text;					// 受信ＰＯＰポート
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_POP_USER_NAME] = tbPopUserName.Text;			// 受信ＰＯＰユーザ名
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_POP_PASSWORD] = tbPopPassword.Text;			// 受信ＰＯＰパスワード
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_POP_APOP_FLG] = (cbPopApop.Checked) ? "1" : "0"; // 受信ＡＰＯＰフラグ
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_HOST] = tbFtpHost.Text;					// ＦＴＰホスト名
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_USER_NAME] = tbFtpUserName.Text;			// ＦＴＰユーザ名
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_PASSWORD] = tbFtpPassword.Text;			// ＦＴＰパスワード
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_UPLOAD_DIR] = tbFtpUploadDir.Text;		// ＦＴＰアップロード先ディレクトリ
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_VALID_FLG] = (cbVValidFlg.Checked) ? "1" : "0";	// 有効フラグ
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_HOST] = tbSftpHost.Text;				// SFTPサーバ接続先
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_PORT] = tbSftpPort.Text;				// SFTPポート
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_USER_NAME] = tbSftpUserName.Text;		// SFTPユーザー名
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_PASS_PHRASE] = tbSftpPassPhrase.Text;	// SFTPパスフレーズ

		// メンテナンス開始日
		if (string.IsNullOrEmpty(ucDateTimePickerPeriod.StartDateTimeString))
		{
			htParam[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_FROM] = null;
		}
		else
		{
			htParam[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_FROM] =
				ucDateTimePickerPeriod.StartDateTimeString;
		}

		// メンテナンス終了日
		if (string.IsNullOrEmpty(ucDateTimePickerPeriod.EndDateTimeString))
		{
			htParam[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_TO] = null;
		}
		else
		{
			htParam[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_TO] =
				ucDateTimePickerPeriod.EndDateTimeString;
		}

		// SMTPサーバ設定Xml設定
		string strSmtpServerSettingXml = null;
		if (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN)
		{
			strSmtpServerSettingXml = new MallSmtpServerSettingRakuten(
				tbSmtpServerName.Text.Trim(),
				tbSmtpServerPort.Text.Trim(),
				tbSmtpServerUserName.Text.Trim(),
				tbSmtpServerPassword.Text.Trim(),
				tbRakutenStoreMailAddress.Text.Trim()).CreateXml();

			// 楽天APIの設定
			htParam[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_USER_NAME] = tbRakutenApiUserName.Text.Trim();
			htParam[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_SHOP_URL] = tbRakutenApiShopUrl.Text.Trim();
			htParam[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_SERVICE_SECRET] = tbRakutenApiServiceSecret.Text.Trim();
			htParam[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_LICENSE_KEY] = tbRakutenApiLicenseKey.Text.Trim();
		}
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_OTHER_SETTING] = StringUtility.ToEmpty(strSmtpServerSettingXml);

		// 全モールのパラメータを用意しておく
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_INS_ITEMCSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_UPD_ITEMCSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_STK_ITEMCSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_ITEMCSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_SELECTCSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_UPD_ITEMCSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_STK_SELECTCSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_ITEMCATCSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_DATACSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_STOCKCSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_UPD_DATACSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_STK_DATACSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_DATACSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_STOCKCSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_UPD_DATACSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_STK_DATACSV] = null;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_MERCHANT_ID] = string.Empty;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_MARKETPLACE_ID] = string.Empty;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_AWS_ACCESSKEY_ID] = string.Empty;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_SECRET_KEY] = string.Empty;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_MWS_AUTHTOKEN] = string.Empty;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG] = Constants.FLG_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG_INVALID;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_NEXT_ENGINE_STOCK_STORE_ACCOUNT] = string.Empty;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_NEXT_ENGINE_STOCK_AUTH_KEY] = string.Empty;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_FACEBOOK_CATALOG_ID] = string.Empty;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_FACEBOOK_ACCESS_TOKEN] = string.Empty;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_CLIENT_ID] = string.Empty;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_CLIENT_SECRET] = string.Empty;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_SELLER_ID] = string.Empty;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_SKU_MANAGEMENT_ID_OUTPUT_FORMAT_FOR_NORMAL] = string.Empty;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_SKU_MANAGEMENT_ID_OUTPUT_FORMAT_FOR_VARIATION] = string.Empty;

		switch (ddlMallKbn.SelectedValue)
		{
			// 楽天パラメータ追加
			case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN:
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_INS_ITEMCSV] = ddlCnvidRtnNInsItemcsv.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_UPD_ITEMCSV] = ddlCnvidRtnNUpdItemcsv.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_STK_ITEMCSV] = ddlCnvidRtnNStkItemcsv.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_ITEMCSV] = ddlCnvidRtnVInsItemcsv.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_SELECTCSV] = ddlCnvidRtnVInsSelectcsv.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_UPD_ITEMCSV] = ddlCnvidRtnVUpdItemcsv.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_STK_SELECTCSV] = ddlCnvidRtnVStkSelectcsv.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_ITEMCATCSV] = ddlCnvidRtnItemCatcsv.SelectedValue;

				// メルアド形式チェックを行うため、別にhtParamに入れる
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_RTN_MAIL_ADDR] = tbRakutenStoreMailAddress.Text.Trim();

				if (Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION)
				{
					htParam[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_SKU_MANAGEMENT_ID_OUTPUT_FORMAT_FOR_NORMAL] = tbRakutenSkuManagementIdOutputFormatForNormal.Text.Trim();
					htParam[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_SKU_MANAGEMENT_ID_OUTPUT_FORMAT_FOR_VARIATION] = tbRakutenSkuManagementIdOutputFormatForVariation.Text.Trim();
				}
				break;

			// YAHOOパラメータ追加
			case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO:
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_DATACSV] = ddlCnvidYhoAddDatacsv.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_STOCKCSV] = ddlCnvidYhoAddQuantitycsv.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_UPD_DATACSV] = ddlCnvidYhoDatacsv.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_STK_DATACSV] = ddlCnvidYhoQuantitycsv.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_DATACSV] = ddlCnvidYhoVAddDatacsv.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_STOCKCSV] = ddlCnvidYhoVAddQuantitycsv.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_UPD_DATACSV] = ddlCnvidYhoVDatacsv.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_STK_DATACSV] = ddlCnvidYhoVQuantitycsv.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_CLIENT_ID] = tbYahooApiClientId.Text.Trim();
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_CLIENT_SECRET] = tbYahooApiClientSecret.Text.Trim();
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_SELLER_ID] = tbYahooApiSellerId.Text.Trim();
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_PUBLIC_KEY] = tbYahooApiPublicKey.Text.Trim();
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_PUBLIC_KEY_VERSION] = tbYahooApiPublicKeyVersion.Text.Trim();
				break;

			// Amazonパラメータ追加
			case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON:
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_MERCHANT_ID] = tbAmazonMerchantId.Text.Trim();
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_MARKETPLACE_ID] = tbAmazonMarketPlaceId.Text.Trim();
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_AWS_ACCESSKEY_ID] = tbAwsAccessKeyId.Text.Trim();
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_SECRET_KEY] = tbAmazonSecretKey.Text.Trim();
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_AMAZON_MWS_AUTHTOKEN] = tbMwsAuthToken.Text.Trim();
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG] =
					(cbStockUpdateUseFlg.Checked) ? Constants.FLG_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG_VALID : Constants.FLG_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG_INVALID;
				break;

			// Lohacoパラメータ追加
			case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_LOHACO:
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG] =
					(cbLohacoStockUpdateUseFlg.Checked) ? Constants.FLG_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG_VALID : Constants.FLG_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG_INVALID;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_LOHACO_API_PRIVATE_KEY] = tbLohacoPrivateKey.Text;
				break;

			// ＆mallパラメータ追加
			case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_ANDMALL:
				// ＆mall連携項目
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_TENANT_CODE] = tbAndMallTenantCode.Text.Trim();
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_BASE_STORE_CODE] = tbAndMallBaseStoreCode.Text.Trim();
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_SHOP_NO] = tbAndMallShopNo.Text.Trim();
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_COOPERATION] = ddlCooperationId.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_VARIATION_COOPERATION] = ddlVariationCooperationId.SelectedValue;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_SITE_CODE] = tbSiteCode.Text.Trim();
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_SIGNATURE_KEY] = tbSignatureKey.Text.Trim();
				break;

			// ネクストエンジンパラメータ追加
			case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_NEXT_ENGINE:
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_NEXT_ENGINE_STOCK_STORE_ACCOUNT] = tbNextEngineStockStoreAccount.Text.Trim();
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_NEXT_ENGINE_STOCK_AUTH_KEY] = tbNextEngineStockAuthKey.Text.Trim();
				break;

			// Facebook
			case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_FACEBOOK:
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_FACEBOOK_CATALOG_ID] = tbFacebookCatalogId.Text.Trim();
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_FACEBOOK_ACCESS_TOKEN] = tbFacebookAccessToken.Text.Trim();
				break;
		}

		// 共通情報格納
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID] = this.LoginOperatorShopId;
		htParam[Constants.FIELD_MALLCOOPERATIONSETTING_LAST_CHANGED] = this.LoginOperatorName;

		//------------------------------------------------------
		// 入力チェック
		//------------------------------------------------------
		StringBuilder sbErrorMessages = new StringBuilder();
		if ((string)htParam[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID] == Constants.FLG_USER_MALL_ID_OWN_SITE)
		{
			//「以外」
			var textDifferent = ValueText.GetValueText(
				Constants.TABLE_MALLCOOPERATIONSETTING,
				Constants.VALUETEXT_PARAM_TEXT_MALL_COOPERATION_SETTING,
				Constants.VALUETEXT_PARAM_DIFFERENT);
			sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_REGEXP)
					.Replace("@@ 1 @@", ReplaceTag("@@DispText.order.MailId@@"))
					.Replace("@@ 2 @@", string.Format(
						"「{0}」{1}",
						StringUtility.ToEmpty(htParam[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID]),
						textDifferent)))
				.Append("<br />");
		}

		// ネクストエンジンは一元管理システムなので登録は有効無効関係なく１つ限定
		if ((checkKbn == CheckKbn.MallConfigRegist.ToText())
			&& ((string)htParam[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN] == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_NEXT_ENGINE)
			&& new MallCooperationSettingService()
				.GetAll(this.LoginOperatorShopId)
				.Any(m => (m.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_NEXT_ENGINE)))
		{
			sbErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MALL_CONFIG_NEXT_ENGINE_KBN_DUPLICATE));
		}

		sbErrorMessages.Append(Validator.Validate(checkKbn, htParam));
		if (sbErrorMessages.Length > 0)
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = sbErrorMessages.ToString();
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		else
		{
			if (ddlMallKbn.SelectedValue != Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN)
			{
				// 楽天APIの設定
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_USER_NAME] = string.Empty;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_SHOP_URL] = string.Empty;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_SERVICE_SECRET] = string.Empty;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_LICENSE_KEY] = string.Empty;
			}

			if (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_ANDMALL)
			{
				// コピー新規登録か？
				var isCopyRegister = ((string.IsNullOrEmpty(this.MallId) == false) && (tbMallId.Text.Equals(this.MallId) == false));
				// SFTP秘密鍵ファイルの設定
				// ファイル設定なしまたは更新の場合以外は、ファイルアップロード処理を行う
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_PRIVATE_KEY_FILE_PATH] =
					((string.IsNullOrEmpty(fSftpPrivateKeyFileUpload.Value) && string.IsNullOrEmpty(hfSftpPrivateKeyFilePath.Value))
						|| ((string.IsNullOrEmpty(hfSftpPrivateKeyFilePath.Value) == false) && (isCopyRegister == false)))
						? hfSftpPrivateKeyFilePath.Value
						: UploadSftpPrivateKeyFile(isCopyRegister);
			}
			else
			{
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_TENANT_CODE] = string.Empty;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_BASE_STORE_CODE] = string.Empty;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_SHOP_NO] = string.Empty;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_COOPERATION] = string.Empty;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_VARIATION_COOPERATION] = string.Empty;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_SITE_CODE] = string.Empty;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_SIGNATURE_KEY] = string.Empty;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_SFTP_PRIVATE_KEY_FILE_PATH] = string.Empty;
			}
			if (ddlMallKbn.SelectedValue != Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_LOHACO)
			{
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_LOHACO_API_PRIVATE_KEY] = string.Empty;
			}

			if (ddlMallKbn.SelectedValue != Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO)
			{
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_PUBLIC_KEY] = string.Empty;
				htParam[Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_PUBLIC_KEY_VERSION] = string.Empty;
			}
		}
		return htParam;
	}

	/// <summary>
	/// コピー新規登録する ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		// 画面より設定値を取得し、ＤＢ更新を行う
		UpdateMallCooperationSetting(SetMallCooperationSetting(CheckKbn.MallConfig.ToText()));

		// モールＩＤをクリアして編集可能にする
		tbMallId.Text = "";
		tbMallId.Enabled = true;
		btnInsertTop.Visible = btnInsertButtom.Visible = true;
		btnDeleteTop.Visible = btnDeleteButtom.Visible = false;
		btnCopyInsertTop.Visible = btnCopyInsertButtom.Visible = false;
		btnUpdateTop.Visible = btnUpdateButtom.Visible = false;
		if (string.IsNullOrEmpty(hfSftpPrivateKeyFilePath.Value) == false) spCopyFileNotice.Visible = true;

		// モール出品設定からコピー元情報を削除する
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLCOOPERATIONSETTING, Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG))
		{
			DataView dvMallCooperationSettings = GetMallCooperationSettingAll(this.LoginOperatorShopId);
			foreach (DataRowView drvMallCooperationSetting in dvMallCooperationSettings)
			{
				if (((string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG] == li.Value)
					&& ((string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID] == this.MallId))
				{
					ddlMallExhibitsConfig.Items.Remove(li);
					break;
				}
			}
		}
		foreach (ListItem li in ddlMallExhibitsConfig.Items)
		{
			li.Selected = (li.Value == "");
		}
	}

	/// <summary>
	/// モール連携設定情報削除処理
	/// </summary>
	/// <param name="strMallId">削除対象のモールＩＤ</param>
	protected void DeleteMallCooperationSetting(string strMallId)
	{
		// モール出品設定リセット
		ResetMallExhibitsConfig();
	}

	/// <summary>
	/// 削除する ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, EventArgs e)
	{
		// モール連携設定情報削除
		using (SqlAccessor sqlAccesser = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallLiaise", "DeleteMallCooperation"))
		{
			Hashtable htInput = new Hashtable();
			htInput[Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID] = this.LoginOperatorShopId;
			htInput[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID] = tbMallId.Text;

			sqlStatement.ExecStatementWithOC(sqlAccesser, htInput);
		}

		// &mallの場合、SFTP秘密鍵ファイルを削除する
		if (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_ANDMALL)
		{
			DeleteSftpPrivateKeyFile(hfSftpPrivateKeyFilePath.Value);
		}

		// 一覧画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_MALL_LIAISE_LIST);
	}

	/// <summary>
	/// モール区分切替
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlMallKbn_SelectedIndexChanged(object sender, EventArgs e)
	{
		trProductConvRakuten.Visible = (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN);
		trProductConvYahoo.Visible = (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO);

		if (ddlMallKbn.SelectedValue == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_ANDMALL)
		{
			ddlCooperationId.Items.Clear();
			ddlCooperationId.Items.Add(new ListItem("", ""));
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLCOOPERATIONSETTING, Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_COOPERATION))
			{
				ddlCooperationId.Items.Add(li);
			}
			ddlVariationCooperationId.Items.Clear();
			ddlVariationCooperationId.Items.Add(new ListItem("", ""));
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLCOOPERATIONSETTING, Constants.FIELD_MALLCOOPERATIONSETTING_ANDMALL_VARIATION_COOPERATION))
			{
				ddlVariationCooperationId.Items.Add(li);
			}
		}
	}

	/// <summary>
	/// SFTP秘密鍵ファイルのアップロード処理
	/// </summary>
	/// <param name="isCopyFile">ファイルコピーするか※コピー新規登録対応</param>
	private string UploadSftpPrivateKeyFile(bool isCopyFile)
	{
		// SFTPファイルのディレクトリ
		var strSftpFileDirectory = Constants.PHYSICALDIRPATH_MALLCOOPERATION_SFTPPRIVATEKEY_FILEPATH;
		// ディレクトリが存在しなければ作成
		if (Directory.Exists(strSftpFileDirectory) == false)
		{
			Directory.CreateDirectory(strSftpFileDirectory);
		}
		// ファイルアップロード処理
		var fileName = ddlMallKbn.SelectedValue + "_" + tbMallId.Text.Trim() + "_" + Constants.MALLCOOPERATION_SFTPPRIVATEKEY_FILENAME;
		var strFilePath = Path.Combine(strSftpFileDirectory, fileName);
		// コピー新規登録の場合、コピー元のファイルをコピーして新しいファイルを作成する
		if (isCopyFile)
		{
			File.Copy(hfSftpPrivateKeyFilePath.Value, strFilePath);
		}
		else
		{
			// ファイル存在チェック
			if (fSftpPrivateKeyFileUpload.PostedFile.InputStream.Length == 0)
			{
				// ファイルなしエラー
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SFTP_PRIVATE_KEY_UPLOAD_FILE_UNFIND);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
			else
			{
				try
				{
					// ファイルアップロード実行
					fSftpPrivateKeyFileUpload.PostedFile.SaveAs(strFilePath);
				}
				catch (System.UnauthorizedAccessException ex)
				{
					// ファイルアップロード権限エラー（ログにも記録）
					AppLogger.WriteError(ex);
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SFTP_PRIVATE_KEY_UPLOAD_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
			}
		}
		return strFilePath;
	}

	/// <summary>
	/// SFTP秘密鍵ファイルの削除処理
	/// </summary>
	/// <param name="filePath">ファイルパス</param>
	private void DeleteSftpPrivateKeyFile(string filePath)
	{
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
	}

	/// <summary>
	/// トークンを取得するボタンを押下イベント発火時処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRefreshYahooApiToken_OnClick(object sender, EventArgs e)
	{
		// まず、Refresh Token でのトークン取得を試みる
		var procedure = new YahooApiTokenProcedure();
		var result = procedure.GetAccessTokenWithRefreshToken(
			mallId: MallId,
			dateTimeToCompare: DateTime.Now,
			forcesAccessTokenRefresh: true);
		switch (result.Result)
		{
			// Refresh Tokenの有効期限が切れている場合は、"認可コード" での取得を行う
			case YahooApiTokenRefreshResultCode.RefreshOnBrowserRequired:
				// Yahoo API "State" コード (CSRF対策) を生成
				var state = YahooApiTokenProcedure.GenerateRandomStateCode();
				SessionManager.YahooApiAntiForgeryStateCode = state;
				SessionManager.YahooApiMallId = this.MallId;
				var url = procedure.GenerateYahooApiAuthorizationUrl(this.MallId, state);
				Response.Redirect(url);
				break;

			// 更新成功した場合、画面描写
			case YahooApiTokenRefreshResultCode.SuccessfullyRefreshed:
				tbYahooApiAccessToken.Text = result.AccessToken;
				tbYahooApiAccessTokenExpirationDatetime.Text = result.AccessTokenExpirationDateTime.ToString();
				break;
			
			// エラーページへ
			case YahooApiTokenRefreshResultCode.FailedToRefresh:
				Session[Constants.SESSION_KEY_ERROR_MSG] = "トークンの取得に失敗しました。管理者へ問い合わせてください。";
				var errUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR).CreateUrl();
				Response.Redirect(errUrl);
				break;

			// 想定しないケース
			// (アクセストークンがまだ有効でもアクセストークン更新するため、何もしないことはない)
			case YahooApiTokenRefreshResultCode.NoNeedToRefreshAccessToken:
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	/// <summary>
	/// トークンを削除するボタンを押下イベント発火時処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDeleteYahooApiToken_OnClick(object sender, EventArgs e)
	{
		// 空更新
		new MallCooperationSettingService().DeleteYahooApiTokenSet(this.LoginOperatorShopId, this.MallId);

		tbYahooApiAccessToken.Text = string.Empty;
		tbYahooApiAccessTokenExpirationDatetime.Text = string.Empty;
		tbYahooApiRefreshToken.Text = string.Empty;
		tbYahooApiRefreshTokenExpirationDatetime.Text = string.Empty;
	}

	/// <summary>
	/// トークンを強制取得するボタンを押下イベント発火時処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbForceRefreshYahooApiToken_OnClick(object sender, EventArgs e)
	{
		var procedure = new YahooApiTokenProcedure();
		var state = YahooApiTokenProcedure.GenerateRandomStateCode();
		SessionManager.YahooApiAntiForgeryStateCode = state;
		SessionManager.YahooApiMallId = this.MallId;

		var url = procedure.GenerateYahooApiAuthorizationUrl(this.MallId, state);
		Response.Redirect(url);
	}

	/// <summary>モールID</summary>
	string MallId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MALL_ID]); }
	}
	/// <summary>完了フラグ</summary>
	bool IsComplete
	{
		get { return ((Session["IsComplete"] != null) && ((bool)Session["IsComplete"])); }
		set { Session["IsComplete"] = value; }
	}
	/// <summary>SFTP秘密鍵ファイルがアップロードされたか</summary>
	protected bool IsSftpPrivateKeyFileUploaded { get; set; }
}
