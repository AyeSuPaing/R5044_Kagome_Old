/*
=========================================================================================================
  Module      : 配送料情報確認ページ処理(ShippingConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.RefreshFileManager;
using w2.Common.Web;
using w2.Domain.ShopShipping;
using w2.Domain.GlobalShipping;
using w2.App.Common.Web.Page;
using w2.App.Common.ShippingBaseSettings;

public partial class Form_Shipping_ShippingConfirm : ShopShippingPage
{
	/// <summary>
	/// 海外配送料金表マップ
	/// </summary>
	[Serializable]
	public class GlobalPostageMap : GlobalShippingAreaModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GlobalPostageMap() : base() { }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">データソース</param>
		public GlobalPostageMap(DataRowView source) : base(source) { }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">データソース</param>
		public GlobalPostageMap(Hashtable source) : base(source) { }

		/// <summary>
		/// 海外配送料金表
		/// </summary>
		public GlobalShippingPostageModel[] Postage { get; set; }
	}

	protected Hashtable m_htParam = new Hashtable();    // 配送料情報データバインド用

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// リクエスト取得＆ビューステート格納
			var actionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, actionStatus);

			// 画面設定処理
			// 登録・コピー登録・更新画面確認？
			if ((actionStatus == Constants.ACTION_STATUS_INSERT)
				|| (actionStatus == Constants.ACTION_STATUS_COPY_INSERT)
				|| (actionStatus == Constants.ACTION_STATUS_UPDATE))
			{
				// 処理区分チェック
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

				// 配送料・配送料地帯情報の取得
				m_htParam = this.ShippingInfoInSession;
			}
			// 詳細表示？
			else if (actionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				// 配送料設定ID取得
				this.KeepingShippingId = Request[Constants.REQUEST_KEY_SHIPPING_ID];

				DataRow dr = null;

				// 配送料情報取得
				var dv = GetShippingDataView(this.LoginOperatorShopId, this.KeepingShippingId);
				// 該当データが有りの場合
				if (dv.Count != 0)
				{
					dr = dv.Table.Rows[0];
				}
				// 該当データ無しの場合
				else
				{
					// エラーページへ
					RedirectToErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL));
				}
				// Hashtabe格納
				foreach (DataColumn dc in dr.Table.Columns)
				{
					m_htParam.Add(dc.ColumnName, dr[dc.ColumnName]);
				}

				// 配送会社情報取得
				m_htParam[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO]
					= m_htParam[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO_BEFORE]
					= GetDeliveryCompany(this.LoginOperatorShopId, this.KeepingShippingId);


				// 配送料マスタ取得
				var shippingPostages = new ShippingDeliveryPostageService().GetByShippingId(
					this.LoginOperatorShopId,
					this.KeepingShippingId);

				// 配送料地帯情報取得
				var shippingZones = new ShopShippingRepository().GetZoneAll(
					this.LoginOperatorShopId,
					this.KeepingShippingId);
				// 該当データが有りの場合
				if ((shippingPostages.Length > 0) && (shippingZones.Length > 0))
				{
					// 配送料マスタを格納
					m_htParam[Constants.SESSIONPARAM_KEY_SHIPPINGDELIVERYPOSTAGE_INFO] = shippingPostages
						.OrderBy(item => this.DeliveryCompanyList.First(company => (company.DeliveryCompanyId == item.DeliveryCompanyId)).DisplayOrder)
						.ThenBy(item => item.DeliveryCompanyId)
						.ToArray();

					// 配送料地帯情報を格納
					m_htParam[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO] = shippingZones;
				}
				// 該当データ無しの場合
				else
				{
					// エラーページへ
					RedirectToErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL));
				}

				// 本配送種別が設定されている商品が存在する場合、削除できないようにする
				int iProductCount = (int)m_htParam["product_count"];
				if (iProductCount > 0)
				{
					btnDeleteTop.OnClientClick = btnDeleteBottom.OnClientClick = string.Format("javascript:alert('{0}'); return false;",
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PARAM_CONFIRM).Replace("@@ 1 @@", iProductCount.ToString()));
				}
				// 本配送種別に紐づく注文情報が存在する場合も、削除できないようにする
				else if (IsShopShippingIdInUsedForOrders(this.KeepingShippingId))
				{
					btnDeleteTop.OnClientClick = btnDeleteBottom.OnClientClick = string.Format("javascript:alert('{0}'); return false;",
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PARAM_CONFIRM_ALERT));
				}

				// 海外配送料情報を格納
				m_htParam[Constants.SESSIONPARAM_KEY_GLOBALSHIPPING_AREA_POSTAGE] = Constants.GLOBAL_OPTION_ENABLE
					? GetGlobalShipping(this.LoginOperatorShopId, this.KeepingShippingId)
					: new GlobalPostageMap[0];

				// 元の配送種別IDを保持
				this.OriginShippingId = this.KeepingShippingId;
			}
			// それ以外の場合
			else
			{
				// エラーページへ
				RedirectToErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR));
			}

			// 配送拠点表示
			var shippingBaseSettingManager = ShippingBaseSettingsManager.GetInstance();
			this.shippingBaseName = shippingBaseSettingManager.Settings
				.Where(sb => sb.Id == (string)m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_BASE_ID])
				.Select(sb => sb.Name)
				.FirstOrDefault();

			// ViewStateにも格納しておく
			this.ShippingInfoInViewState = m_htParam;

			// 画面制御
			InitializeComponents(actionStatus);

			// データバインド
			DataBind();
		}
	}

	/// <summary>
	/// 海外配送料金表表示制御
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="shippingId">配送種別</param>
	private GlobalPostageMap[] GetGlobalShipping(string shopId, string shippingId)
	{
		// 海外配送エリアの一覧を取得
		var sv = new GlobalShippingService();
		var area = sv.GetValidGlobalShippingArea();
		var areaPostage = area
			.Select(a =>
			{
				var rtn = new GlobalPostageMap(a.DataSource);
				rtn.Postage = sv.GetAreaPostage(shopId, shippingId, a.GlobalShippingAreaId);
				return rtn;
			})
			.ToArray();
		return areaPostage;
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
			trDetailTop.Visible = false;
			trDetailBottom.Visible = false;
			trConfirmTop.Visible = true;
			trConfirmBottom.Visible = true;

		}
		// 更新？
		else if (strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			btnUpdateTop.Visible = true;
			btnUpdateBottom.Visible = true;
			trDetailTop.Visible = false;
			trConfirmBottom.Visible = true;
			trConfirmTop.Visible = true;
			trDetailBottom.Visible = false;

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

		// 配送料の別途見積り利用フラグがONの場合、配送料設定部分を表示しない
		if (this.IsShippingpriceSeparateEstimateUsable)
		{
			trDetailBottom.Visible = false;
			trConfirmBottom.Visible = false;
		}
	}

	/// <summary>
	/// 配送料情報取得
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="strShippingId">配送料設定ID</param>
	/// <returns>配送料情報データビュー</returns>
	private DataView GetShippingDataView(string shopId, string strShippingId)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatements = new SqlStatement("ShopShipping", "GetShopShipping"))
		{
			Hashtable input = new Hashtable();
			input.Add(Constants.FIELD_SHOPSHIPPING_SHOP_ID, shopId);			// 店舗ID
			input.Add(Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, strShippingId);	// 配送料設定ID

			return sqlStatements.SelectSingleStatementWithOC(sqlAccessor, input);
		}
	}

	/// <summary>
	/// 配送会社リスト取得
	/// </summary>
	/// <param name="shopId">ショップID</param>
	/// <param name="shippingId">配送種別ID</param>
	/// <returns>配送会社リスト</returns>
	private ShopShippingCompanyModel[] GetDeliveryCompany(string shopId, string shippingId)
	{
		var service = new ShopShippingService();
		var shippingInfo = service.Get(shopId, shippingId);
		var result = shippingInfo.CompanyList
			.OrderBy(item => this.DeliveryCompanyList.First(company => (company.DeliveryCompanyId == item.DeliveryCompanyId)).DisplayOrder)
			.ThenBy(item => item.DeliveryCompanyId)
			.ToArray();
		return result;
	}

	/// <summary>
	/// データバインド用日付範囲文言作成
	/// </summary>
	/// <param name="strShippingDateSetFlg">配送日設定可能フラグ</param>
	/// <param name="strShippingDateSetBegin">配送日設定可能範囲(開始)</param>
	/// <param name="strShippingDateSetEnd">配送日設定可能範囲（終了）</param>
	/// <param name="businessDaysForShipping">出荷所要営業日数</param>
	/// <returns>配送日設定可能範囲文言取得</returns>
	protected string CreateShippingDate(string strShippingDateSetFlg, string strShippingDateSetBegin, string strShippingDateSetEnd, string businessDaysForShipping)
	{
		string strResult = "-";

		// 配送日設定可能フラグが有効の場合
		if (strShippingDateSetFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID)
		{
			strResult = string.Format(Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
					"@@DispText.shipping_date.settable_date@@",
					Constants.GLOBAL_OPTION_ENABLE
						? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
						: ""),
				businessDaysForShipping,
				strShippingDateSetBegin,
				strShippingDateSetEnd);
		}

		return strResult;
	}

	/// <summary>
	/// データバインド用配送パターン文言作成
	/// </summary>
	/// <param name="fixedPurchaseFlg">定期購入設定可能フラグ</param>
	/// <returns>配送パターン文言</returns>
	protected string CreateFixedPurchasePatternsString(string fixedPurchaseFlg)
	{
		return (fixedPurchaseFlg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID) ? "" : "-";
	}
	/// <summary>
	/// データバインド用配送パターン文言作成
	/// </summary>
	/// <param name="fixedPurchaseFlg">定期購入設定可能フラグ</param>
	/// <param name="kbnFlg">配送パターン区分フラグ</param>
	/// <param name="kbnFlgValid">配送パターン区分有効フラグ</param>
	/// <param name="kbnMessage">配送パターン文言</param>
	/// <returns>配送パターン文言</returns>
	protected string CreateFixedPurchasePatternsString(string fixedPurchaseFlg, string kbnFlg, string kbnFlgValid, string kbnMessage)
	{
		return ((fixedPurchaseFlg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID) && (kbnFlg == kbnFlgValid)) ? "[" + kbnMessage + "]" : "";
	}

	/// <summary>
	/// データバインド用配送パターン文言作成
	/// </summary>
	/// <param name="fixedPurchaseFlg">定期購入設定可能フラグ</param>
	/// <param name="kbn1Flg">配送パターン01可能フラグ</param>
	/// <param name="kbn2Flg">配送パターン02可能フラグ</param>
	/// <param name="kbn1Setting">月間隔選択肢文言</param>
	/// <returns>配送パターン文言</returns>
	protected string CreateKbn1SettingMessage(string fixedPurchaseFlg, string kbn1Flg, string kbn2Flg, string kbn1Setting)
	{
		var endOfMonth = CommonPage.ReplaceTag("@@DispText.fixed_purchase_kbn.endOfMonth@@");

		if (string.IsNullOrEmpty(kbn1Setting))
		{
			return "-";
		}

		return ((fixedPurchaseFlg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID)
			&& ((kbn1Flg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG_VALID)
				|| (kbn2Flg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG_VALID)))
			? kbn1Setting.Replace(Constants.DATE_PARAM_END_OF_MONTH_VALUE, endOfMonth)
			: "-";
	}

	/// <summary>
	/// データバインド用配送パターン文言作成
	/// </summary>
	/// <param name="fixedPurchaseFlg">定期購入設定可能フラグ</param>
	/// <param name="kbn3Flg">配送パターン03可能フラグ</param>
	/// <param name="kbn3Setting">配送間隔選択肢文言</param>
	/// <returns>配送パターン文言</returns>
	protected string CreateKbn3SettingMessage(string fixedPurchaseFlg, string kbn3Flg, string kbn3Setting)
	{
		return (fixedPurchaseFlg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID
			&& kbn3Flg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG_VALID) ? kbn3Setting : "-";
	}

	/// <summary>
	/// データバインド用配送パターン文言作成
	/// </summary>
	/// <param name="fixedPurchaseFlg">定期購入設定可能フラグ</param>
	/// <param name="kbn4Flg">配送パターン04可能フラグ</param>
	/// <param name="kbn4Setting1">週間隔選択肢文言</param>
	/// <returns>配送パターン文言</returns>
	protected string CreateKbn4Setting1Message(string fixedPurchaseFlg, string kbn4Flg, string kbn4Setting1)
	{
		var FixedPurchaseMessage = ((fixedPurchaseFlg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID)
			&& (kbn4Flg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG_VALID)) ? kbn4Setting1 : "-";

		return FixedPurchaseMessage;
	}

	/// <summary>
	/// 配送種別名称取得
	/// </summary>
	/// <param name="fixedPurchaseFlg">定期購入設定可能フラグ</param>
	/// <param name="kbn4Flg">配送パターン04可能フラグ</param>
	/// <param name="kbn4Setting2">選択曜日（連結番号）</param>
	/// <returns>決済種別名称（連結）</returns>
	protected string CreateKbn4Setting2Message(string fixedPurchaseFlg, string kbn4Flg, string kbn4Setting2)
	{
		var kbn4Setting2Param = string.Empty;

		if ((fixedPurchaseFlg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID)
			&& (kbn4Flg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG_VALID))
		{
			var kbn4Setting2List  = kbn4Setting2
				.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(kbn => int.Parse(kbn))
				.ToArray();

			var kbnList = new List<string>();
			foreach (var item in kbn4Setting2List)
			{
				var kbn4SettingDay = ValueText.GetValueText(
					Constants.TABLE_SHOPSHIPPING,
					Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST,
					item);

				kbnList.Add(kbn4SettingDay);
			}
			kbn4Setting2Param = string.Join(",", kbnList);
		}
		else
		{
			kbn4Setting2Param = "-";
		}

		return kbn4Setting2Param;
	}

	/// <summary>
	/// データバインド用配送パターン共通文言作成
	/// </summary>
	/// <param name="fpValidFlg">定期購入設定可能フラグ</param>
	/// <param name="message">配送パターン共通文言</param>
	/// <returns>配送パターン共通文言</returns>
	protected string CreateFixedPurchaseCommonMessage(string fpValidFlg, string message)
	{
		return (fpValidFlg == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID) ? message : "-";
	}

	/// <summary>
	/// 決済種別名称取得
	/// </summary>
	/// <param name="permittedPaymentIds">決済種別リスト</param>
	/// <returns>決済種別名称（連結）</returns>
	protected string CreatePaymentNames(string permittedPaymentIds)
	{
		StringBuilder paymentNames = new StringBuilder();
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("Payment", "GetPaymentNames"))
		{
			Hashtable sqlParam = new Hashtable();
			sqlParam.Add(Constants.FIELD_SHOPSHIPPING_SHOP_ID, this.LoginOperatorShopId);

			string[] paymentIds = permittedPaymentIds.Split(',');

			sqlStatement.Statement = sqlStatement.Statement.Replace("@@ payment_ids @@", "'" + string.Join("','", paymentIds) + "'");
			DataView paymentDatas = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, sqlParam);
			foreach (DataRowView paymentData in paymentDatas)
			{
				paymentNames.Append(paymentNames.ToString() != "" ? "," : "").Append((string)paymentData[Constants.FIELD_PAYMENT_PAYMENT_NAME]);
			}
		}
		return paymentNames.ToString();
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// 配送料情報をそのままセッションへセット
		this.ShippingInfoInSession = this.ShippingInfoInViewState;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);
	}

	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, System.EventArgs e)
	{
		// 配送料情報をそのままセッションへセット
		this.ShippingInfoInSession = this.ShippingInfoInViewState;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT;

		// 登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COPY_INSERT);
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		// 削除
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("ShopShipping", "DeleteShipping"))
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();


			// 配送料情報取得
			var input = this.ShippingInfoInViewState;
			int iResult = statement.ExecStatement(accessor, input);

			var shippingId = (string)input[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID];

			// 配送料マスタ削除
			new ShippingDeliveryPostageService().DeleteByShippingId(this.LoginOperatorShopId, shippingId, accessor);

			// 配送会社削除
			var service = new ShopShippingService();
			service.DeleteCompany(accessor, shippingId);

			// 海外送料も消しちゃう
			var sv = new GlobalShippingService();
			sv.DeleteGlobalShippingPostageByShipping(
				base.LoginOperatorShopId,
				(string)input[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID],
				accessor);

			accessor.CommitTransaction();
		}

		// フロント系サイトを最新情報へ更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.ShopShipping).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_LIST);
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 配送種別情報取得
		var input = this.ShippingInfoInViewState;

		// 登録
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();

			try
			{
				// 配送種別情報登録
				using (var statement = new SqlStatement("ShopShipping", "InsertShipping"))
				{
					statement.ExecStatement(accessor, input);
				}

				// 配送料マスタ登録
				var sv = new ShippingDeliveryPostageService();
				foreach (var shippingPostage in (ShippingDeliveryPostageModel[])input[Constants.SESSIONPARAM_KEY_SHIPPINGDELIVERYPOSTAGE_INFO])
				{
					sv.Insert(shippingPostage, accessor);
				}

				// 配送料地帯情報登録
				var service = new ShopShippingService();
				foreach (var shippingZone in (ShopShippingZoneModel[])input[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO])
				{
					service.InsertShopShippingZone(shippingZone, accessor);
				}

				// 配送会社登録
				service.InsertCompany(
					accessor,
					(string)input[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID],
					(ShopShippingCompanyModel[])input[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO]);

				// トランザクション確定
				accessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				// トランザクションロールバック
				accessor.RollbackTransaction();

				// スローする
				throw ex;
			}
		}

		// フロント系サイトを最新情報へ更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.ShopShipping).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_LIST);
	}

	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		// 配送種別情報取得
		var input = this.ShippingInfoInViewState;
		var shopId = (string)input[Constants.FIELD_SHOPSHIPPING_SHOP_ID];
		var shippingId = (string)input[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID];

		// 更新
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();
			try
			{
				// 配送料情報更新
				using (var statement = new SqlStatement("ShopShipping", "UpdateShipping"))
				{
					statement.ExecStatement(accessor, input);
				}

				// 配送料マスタ更新（全部消してから、登録）
				var sv = new ShippingDeliveryPostageService();
				sv.Inserts(
					shopId,
					shippingId,
					(ShippingDeliveryPostageModel[])input[Constants.SESSIONPARAM_KEY_SHIPPINGDELIVERYPOSTAGE_INFO],
					accessor);

				// 配送料地帯情報更新（全部消してから、登録）
				var sv2 = new ShopShippingService();
				sv2.InsertShopShippingZones(
					shopId,
					shippingId,
					(ShopShippingZoneModel[])input[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO],
					accessor);

				// 配送会社登録
				sv2.InsertCompany(
					accessor,
					(string)input[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID],
					(ShopShippingCompanyModel[])input[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO]);

				// 不整合性の海外配送料削除
				DeleteInconsistencyGlobalShippingPostage(input, shopId, shippingId, accessor);

				// トランザクション確定
				accessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				// トランザクションロールバック
				accessor.RollbackTransaction();

				// スローする
				throw ex;
			}
		}

		// フロント系サイトを最新情報へ更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.ShopShipping).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_LIST);
	}

	/// <summary>
	/// 不整合性の海外配送料削除
	/// </summary>
	/// <param name="input">入力情報</param>
	/// <param name="shopId">店舗ID</param>
	/// <param name="shippingId">配送種別ID</param>
	/// <param name="accessor">アクセサ</param>
	private void DeleteInconsistencyGlobalShippingPostage(
		Hashtable input,
		string shopId,
		string shippingId,
		SqlAccessor accessor)
	{
		// 編集ではない場合、何もない
		if (this.IsActionUpdate == false) return;

		var beforeDeliveryCompanies = GetDistinctCompany(
			(ShopShippingCompanyModel[])input[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO_BEFORE]);
		var currentDeliveryCompanies = GetDistinctCompany(
			(ShopShippingCompanyModel[])input[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO]);

		// 配送種別に紐づく配送会社の変更前後の差分
		var diffDeliveryCompanyId = beforeDeliveryCompanies.Select(before => before.DeliveryCompanyId).ToArray()
			.Except(currentDeliveryCompanies.Select(cur => cur.DeliveryCompanyId).ToArray())
			.ToList();

		// 海外配送料の整合性のため、配送種別に紐づく配送会社の変更前後の差分に応じて、削除
		var sv = new GlobalShippingService();
		diffDeliveryCompanyId.ForEach(
			deliveryCompanyId => sv.DeleteGlobalShippingPostageByShippingDeliveryCompany(
				shopId,
				shippingId,
				deliveryCompanyId,
				accessor));
	}

	/// <summary>
	/// 本配送種別に紐づく注文情報が存在するチェック
	/// </summary>
	/// <param name="shippingId">配送種別ID</param>
	/// <returns>TRUE：存在する　FALSE：存在しない</returns>
	private bool IsShopShippingIdInUsedForOrders(string shippingId)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ShopShipping", "CheckExistOrder"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_SHOPSHIPPING_SHOP_ID, this.LoginOperatorShopId);
			htInput.Add(Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, shippingId);

			DataView dv = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);

			return ((int)dv[0]["count"] > 0);
		}
	}

	/// <summary>
	/// 配送会社取得（宅配便）
	/// </summary>
	/// <param name="companyList">配送会社リスト</param>
	/// <returns>配送会社リスト（宅配便）</returns>
	public ShopShippingCompanyModel[] GetExpressCompany(ShopShippingCompanyModel[] companyList)
	{
		return companyList.Where(i => i.ShippingKbn == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS).ToArray();
	}

	/// <summary>
	/// 配送会社取得（メール便）
	/// </summary>
	/// <param name="companyList">配送会社リスト</param>
	/// <returns>配送会社リスト（メール便）</returns>
	public ShopShippingCompanyModel[] GetMailCompany(ShopShippingCompanyModel[] companyList)
	{
		return companyList.Where(i => i.ShippingKbn == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL).ToArray();
	}

	/// <summary>
	/// 配送会社名取得
	/// </summary>
	/// <param name="companyList">配送会社リスト</param>
	/// <param name="isDeliveryCompanyMailEscalation">メール便配送サービスエスカレーション機能がTRUEか</param>
	/// <returns>配送会社名</returns>
	public string GetDeliveryCompanyString(ShopShippingCompanyModel[] companyList, bool isDeliveryCompanyMailEscalation = false)
	{
		if (isDeliveryCompanyMailEscalation) return GetDeliveryCompanyStringMail(companyList);
		return string.Join(",", companyList.Select(i => string.Format("{0}{1}", this.DeliveryCompanyList.First(company => company.DeliveryCompanyId == i.DeliveryCompanyId).DeliveryCompanyName, (i.DefaultDeliveryCompany == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY_VALID ? "（初期値）" : ""))).ToArray());
	}

	/// <summary>
	/// 配送会社名取得(メール便)
	/// </summary>
	/// <param name="companyList">配送会社リスト</param>
	/// <returns>配送会社名(メール便)</returns>
	public string GetDeliveryCompanyStringMail(ShopShippingCompanyModel[] companyList)
	{
		return string.Join(
			",",
			companyList
				.Select(i => string.Format(
					"{0}{1}{2}",
					this.DeliveryCompanyList
						.First(company => company.DeliveryCompanyId == i.DeliveryCompanyId)
						.DeliveryCompanyName,
					"(" + this.DeliveryCompanyList
						.First(company => company.DeliveryCompanyId == i.DeliveryCompanyId)
						.DeliveryCompanyMailSizeLimit + ")",
					((i.DefaultDeliveryCompany == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY_VALID)
						? "（初期値）"
						: "")))
				.ToArray());
	}

	/// <summary>
	/// 海外用の金額表示制御
	/// </summary>
	/// <param name="val">対象値</param>
	/// <returns>制御した値</returns>
	protected string GlobalPriceDisplayControl(decimal val)
	{
		return val.ToPriceString(true);
	}

	/// <summary>
	/// 配送料金表リピーターイベント
	/// </summary>
	/// <param name="source">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void rShippingPostage_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		if (e.CommandName == "EditGlobalShippingPostage")
		{
			// 海外配送料金表編集画面へ遷移
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_GLOBAL_SHIPPING_POSTAGE_EDIT)
				.AddParam(Constants.REQUEST_KEY_SHIPPING_ID, this.KeepingShippingId)
				.AddParam(Constants.REQUEST_KEY_DELIVERY_COMPANY_ID, e.CommandArgument.ToString())
				.CreateUrl();
			Response.Redirect(url);
		}
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		string url;
		if (this.IsActionDetail)
		{
			// 一覧ページへ遷移
			url = CreateShippingPageUrl(Constants.PAGE_MANAGER_SHIPPING_LIST);
			this.Response.Redirect(url);
		}

		m_htParam = this.ShippingInfoInSession;
		var redirectPage = (this.IsShippingpriceSeparateEstimateUsable || (this.IsShippingCountryAvailableJp == false))
				? Constants.PAGE_MANAGER_SHIPPING_REGISTER
				: Constants.PAGE_MANAGER_SHIPPING_REGISTER2;
		url = CreateShippingPageUrl(redirectPage, this.ActionStatus);
		this.Response.Redirect(url);
	}

	#region プロパティ
	/// <summary>日付範囲設定の利用の有無</summary>
	public bool IsShippingDateUsable
	{
		get { return (string)m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG] == Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_VALID; }
	}
	/// <summary>定期購入の利用の有無</summary>
	public bool IsFixedPurchaseUsable
	{
		get { return (string)m_htParam[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG] == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID; }
	}
	/// <summary>のし設定の利用の有無</summary>
	public bool IsWrappingPaperUsable
	{
		get { return (string)m_htParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG] == Constants.FLG_SHOPSHIPPING_WRAPPING_PAPER_FLG_VALID; }
	}
	/// <summary>包装設定の利用の有無</summary>
	public bool IsWrappingBagUsable
	{
		get { return (string)m_htParam[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG] == Constants.FLG_SHOPSHIPPING_WRAPPING_BAG_FLG_VALID; }
	}
	/// <summary>任意決済種別選択の利用の有無</summary>
	public bool IsPaymentSelectionUsable
	{
		get { return (string)m_htParam[Constants.FIELD_SHOPSHIPPING_PAYMENT_SELECTION_FLG] == Constants.FLG_SHOPSHIPPING_PAYMENT_SELECTION_FLG_VALID; }
	}
	/// <summary>配送料の別途見積もり利用の有無</summary>
	public bool IsShippingpriceSeparateEstimateUsable
	{
		get { return (string)m_htParam[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG] == Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID; }
	}
	/// <summary>配送種別配送会社リスト</summary>
	public ShopShippingCompanyModel[] ShopShippingCompanyList { get { return (ShopShippingCompanyModel[])m_htParam[Constants.SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO]; } }
	/// <summary>配送種別ID（ViewState保持）</summary>
	protected string KeepingShippingId
	{
		get { return StringUtility.ToEmpty(ViewState["shipping_id"]); }
		set { ViewState["shipping_id"] = value; }
	}
	/// <summary>指定した配送サービス件数</summary>
	protected int SelectedDeliveryCompanyCount
	{
		get { return ((ShippingDeliveryPostageModel[])m_htParam[Constants.SESSIONPARAM_KEY_SHIPPINGDELIVERYPOSTAGE_INFO]).Length; }
	}
	/// <summary>配送拠点表示名</summary>
	public string shippingBaseName { get; set; }
	#endregion
}

