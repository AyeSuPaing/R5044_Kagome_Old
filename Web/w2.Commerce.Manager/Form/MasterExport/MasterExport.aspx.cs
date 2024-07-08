/*
=========================================================================================================
  Module      : マスタ情報出力ページ処理(MasterExport.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Config;
using w2.App.Common.MasterExport;
using w2.App.Common.Order;
using w2.Domain.User.Helper;
using w2.Domain.FixedPurchase;
using w2.Domain.MallExhibitsConfig;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.ProductCategory;
using w2.Domain.ProductReview;
using w2.Domain.ProductSale;
using w2.Domain.ProductStock;
using w2.Domain.ProductTaxCategory;
using w2.Domain.RealShop;
using w2.Domain.RealShopProductStock;
using w2.Domain.SerialKey;
using w2.Domain.ShortUrl;
using w2.Domain.StockOrder;
using w2.Domain.User;
using w2.Domain.UserProductArrivalMail;
using w2.Domain.ShopOperator;

/// <summary>
/// MasterExport の概要の説明です。
/// </summary>
public partial class Form_MasterExport_MasterExport : BasePage
{
	#region 定数
	/// <summary>Order id for get data for order combine</summary>
	private const string ORDER_ID_FOR_GET_FIXED_PURCHASE_SHIPPING_DATES = "order_id_for_get_fixed_purchase_shipping_dates";
	#endregion

	/// <summary>出力ファイル種別</summary>
	public enum FileType
	{
		Csv,
		Excel,
	}

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		// マスタ情報出力
		OutPutMaster(this.LoginOperatorShopId);
	}

	/// <summary>
	/// マスタ情報出力
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	private void OutPutMaster(string strShopId)
	{
		//------------------------------------------------------
		// マスタ出力定義情報取得
		//------------------------------------------------------
		// マスタ情報チェック
		Hashtable parameters = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		if (CheckMasterInfo(parameters) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// マスタ区分取得(ワークフローの場合マスタ定義は注文と同一情報を利用するためここで変換)
		string masterKbn = MasterExportSettingUtility.GetMasterKbnException((string)parameters[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN]);
		var settingIndex = (int)parameters[Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID];

		// マスタ出力定義情報取得
		var exportSettings = GetMasterExportSettingDataView(strShopId, masterKbn);
		if (exportSettings.Length <= settingIndex)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTEREXPORTSETTING_NO_DATA);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// マスタデータ取得・出力
		//------------------------------------------------------
		var exportSetting = exportSettings[settingIndex];
		exportSetting.MasterKbn = (string)parameters[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN];

		// HTTPヘッダセット(ファイル名など）
		var fileExtension = (exportSetting.ExportFileType == Constants.FLG_MASTEREXPORTSETTING_EXPORT_FILE_TYPE_CSV)
			? ".csv"
			: Constants.MASTEREXPORT_EXCELFORMAT;
		Response.AppendHeader("Content-Disposition", "attachment; filename=" + masterKbn + DateTime.Now.ToString("yyyyMMdd") + fileExtension);

		if (exportSetting.ExportFileType == Constants.FLG_MASTEREXPORTSETTING_EXPORT_FILE_TYPE_CSV)
		{
			// CSV出力時に文字コードを設定
			Response.ContentEncoding = Encoding.GetEncoding(Constants.MASTEREXPORT_CSV_ENCODE);
		}

		int orderTotalCounts;
		// データ結合マスタが有効かつ、セッションに値が入っている場合のみ出力上限受注件数の判定を行う
		if (Constants.MASTEREXPORT_DATABINDING_OPTION_ENABLE
			&& int.TryParse(StringUtility.ToEmpty(Session[Constants.SESSIONPARAM_KEY_ORDER_TOTAL_COUNT]), out orderTotalCounts))
		{
			// 出力上限受注件数が0の場合は上限無しで出力
			if (Constants.MASTEREXPORT_DATABINDING_OUTPUT_LINES_LIMIT != 0)
			{
				if (((masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING) || (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING_WORKFLOW))
					&& (orderTotalCounts > Constants.MASTEREXPORT_DATABINDING_OUTPUT_LINES_LIMIT))
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTEREXPORT_DATABINDING_OUTPUT_LINES_OVER_ERROR)
						.Replace("@@ 1 @@", Constants.MASTEREXPORT_DATABINDING_OUTPUT_LINES_LIMIT.ToString());
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
			}
		}

		// マスタ情報出力
		OutPutMasterInner(parameters, exportSetting);

		// 出力ストップ
		Response.End();
	}

	/// <summary>
	/// 各マスタ検索情報有無チェック
	/// </summary>
	/// <param name="parameters">各マスタ検索情報</param>
	/// <returns>各マスタ検索情報有無</returns>
	private bool CheckMasterInfo(Hashtable parameters)
	{
		if (parameters == null) return false;

		switch (StringUtility.ToEmpty(parameters[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN]))
		{
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCT:				// 商品マスタ表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTVARIATION: 	// 商品バリエーション表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTPRICE:			// 会員ランク価格
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTTAG:  // 商品タグ表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTEXTEND:		// 商品拡張項目表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSTOCK:		    // 商品在庫表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTCATEGORY:	    // 商品カテゴリ表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER:				// 注文マスタ表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM:			// 注文商品マスタ表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION:	// 注文セットプロモーションマスタ表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER_WORKFLOW:		// 注文マスタ表示（ワークフロー）
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM_WORKFLOW:	// 注文商品マスタ表示（ワークフロー）
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION_WORKFLOW:	// 注文セットプロモーションマスタ表示（ワークフロー）
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER:					// ユーザーマスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERSHIPPING:			// 配送先情報
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDER:			// 発注マスタ表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDERITEM:		// 発注商品マスタ表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_MALLEXHIBITSCONFIG:	// モール出品設定マスタ表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL:	// 入荷通知メールマスタ表示（サマリー）
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL_DETAIL:	// 入荷通知メールマスタ表示（明細）
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTREVIEW:		// 商品レビュー
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSALEPRICE:		// 商品セール価格
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SHORTURL:				// ショートURL
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE:		// 定期購入マスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM:		// 定期購入商品マスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE_WORKFLOW:		// 定期購入マスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM_WORKFLOW:		// 定期購入商品マスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SERIALKEY: // シリアルキーマスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOP: 				// リアル店舗マスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOPPRODUCTSTOCK: 	// リアル店舗商品在庫マスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERINVOICE:			// ユーザー電子発票管理マスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_OPERATOR:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING_WORKFLOW:
				return true;

			default:
				return false;
		}
	}

	/// <summary>
	/// 各マスタ情報取得(CSV形式文字列)
	/// </summary>
	/// <param name="sqlParams">各マスタ検索情報</param>
	/// <param name="setting">マスタ出力定義情報</param>
	/// <returns>各マスタ情報取得(CSV形式文字列)</returns>
	private void OutPutMasterInner(Hashtable sqlParams, MasterExportSettingModel setting)
	{
		// SQLステートメント情報取得
		var outputFieldInfo = GetMasterExportSettingFields(setting.MasterKbn);
		var fieldNamesCsvSplit = StringUtility.SplitCsvLine(setting.Fields);
		
		// 税率毎価格情報項目チェック
		var taxFieldNames = new ProductTaxCategoryService().GetMasterExportSettingFieldNames();
		var taxFields = string.Join(",", taxFieldNames.Select(taxFieldName => string.Format("[{0}]", taxFieldName)));

		// データ結合マスタの場合フィールド名にプレフィックスをつける
		var masterKbn = MasterExportSettingUtility.GetMasterKbnException((string)sqlParams[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN]);
		var baseName = masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING ? Constants.FIELD_ORDER_CONVERTING_NAME : string.Empty;

		var settingTaxFieldNames = fieldNamesCsvSplit
			.Where(name => name.Contains("_by_rate_"))
			.Distinct();
		var illegalTaxFields = settingTaxFieldNames.Where(
			settingTaxFieldName => taxFieldNames.Any(taxFieldName => settingTaxFieldName == baseName + taxFieldName) == false).ToArray();
		if (illegalTaxFields.Any())
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages
				.GetMessages(WebMessages.ERRMSG_MANAGER_CSV_OUTPUT_TAX_FIELD_ERROR)
				.Replace("@@ 1 @@", string.Join("<br>", illegalTaxFields))
				.Replace(
					"@@ 2 @@",
					ValueText.GetValueText(
						Constants.TABLE_MASTEREXPORTSETTING,
						Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN,
						setting.MasterKbn));
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// CSV出力フォーマットとCSVフォーマット設定の不整合発生時はエラーとする
		var missedField = fieldNamesCsvSplit.FirstOrDefault(field => (outputFieldInfo.ContainsKey(field) == false));
		if (missedField != null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CSV_OUTPUT_FIELD_ERROR)
				.Replace("@@ 1 @@", missedField)
				.Replace("@@ 2 @@", ValueText.GetValueText(Constants.TABLE_MASTEREXPORTSETTING, Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN, setting.MasterKbn));
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 取得するフィールド列作成
		var sqlFieldNames = string.Join(
			",",
			fieldNamesCsvSplit.Select(f => outputFieldInfo[f]));
		if (fieldNamesCsvSplit.Any(f => IsGetTargetOfCombineFixedPurchaseData((string)outputFieldInfo[f], setting.MasterKbn)))
			{
			sqlFieldNames += string.Format(
				",{0}.{1} AS {2}",
				Constants.TABLE_ORDER,
				Constants.FIELD_ORDER_ORDER_ID,
				ORDER_ID_FOR_GET_FIXED_PURCHASE_SHIPPING_DATES);
			}

		// 出力
		var fileType = (setting.ExportFileType == Constants.FLG_MASTEREXPORTSETTING_EXPORT_FILE_TYPE_CSV)
			? FileType.Csv
			: FileType.Excel;

		if (Export(setting, sqlParams, sqlFieldNames, Response.OutputStream, fileType) == false)
	{
				// 最大件数オーバーエラー
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTEREXPORT_EXCEL_OVER_CAPACITY);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
	}

	/// <summary>
	/// マスタ出力設定のフィールド群取得
	/// </summary>
	/// <param name="masterKbn">マスタ区分</param>
	/// <returns>ハッシュテーブル</returns>
	private Hashtable GetMasterExportSettingFields(string masterKbn)
	{
		masterKbn = MasterExportSettingUtility.GetMasterKbnException(masterKbn);

		//------------------------------------------------------
		// フィールド名変換（共通）
		//------------------------------------------------------
		Hashtable result = MasterExportSettingUtility.GetMasterExportSettingFields(masterKbn);

		//------------------------------------------------------
		// フィールド名変換（ユーザーマスタの場合)
		//------------------------------------------------------
		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER)
		{
			// ユーザー拡張項目のフィールド名を取得できるよう、動的部分のHashtable作成
			var userExtendSettingList = new UserExtendSettingList(this.LoginOperatorName);
			userExtendSettingList.Items.ForEach(userExtendSetting =>
				result.Add(userExtendSetting.SettingId, Constants.TABLE_USEREXTEND + "." + userExtendSetting.SettingId));

			var userAttributeFields = MasterExportSettingUtility.GetMasterExportSettingFields(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERATTRIBUTE);
			foreach (var key in userAttributeFields.Keys)
			{
				result[key] = userAttributeFields[key];
			}
		}

		//------------------------------------------------------
		// フィールド名変換（商品タグマスタの場合)
		//------------------------------------------------------
		// 名称からフィールド名を取得できるようHashtable作成
		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTTAG)
		{
			foreach (DataRowView drv in GetProductTagSetting())
			{
				result.Add(drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID], "w2_ProductTag." + drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]);
			}
		}

		//------------------------------------------------------
		// フィールド名変換（商品拡張項目マスタの場合)
		//------------------------------------------------------
		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTEXTEND)
		{
			foreach (DataRowView drv in GetProductExtendSetting(this.LoginOperatorShopId))
			{
				// 名称からフィールド名を取得できるようHashtable作成
				result.Add("extend" + ((int)drv[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO]).ToString(), "w2_ProductExtend.extend" + ((int)drv[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO]).ToString());
			}
		}

		//------------------------------------------------------
		// フィールド名変換（モール出品設定項目の場合）
		//------------------------------------------------------
		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_MALLEXHIBITSCONFIG)
		{
			int exhibitsCount = 1;
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLCOOPERATIONSETTING, Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG))
			{
				bool existsMallCooperationSettings = false;
				DataView mallCooperationSettings = MallPage.GetMallCooperationSettingAll(this.LoginOperatorShopId);
				foreach (DataRowView drv in mallCooperationSettings)
				{
					if ((string)drv[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG] == li.Value)
					{
						existsMallCooperationSettings = true;
						break;
					}
				}
				if (existsMallCooperationSettings == false)
				{
					result.Remove(Constants.CONST_MALLEXHIBITSCONFIG_EXHIBITS_FLG + exhibitsCount);
				}

				exhibitsCount++;
			}
		}

		//------------------------------------------------------
		// フィールド名変換（注文系マスタの場合、注文拡張ステータス部分)
		//------------------------------------------------------
		if ((masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER)
			|| (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM)
			|| (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER_WORKFLOW)
			|| (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM_WORKFLOW))
		{
			MasterExportSettingUtility.SetFieldInfoForOrder(result);
		}

		// Extend status and Extend status date for Fixed Purchase and Fixed Purchase item
		if ((masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE) || (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM))
		{
			for (int i = 1; i <= Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; i++)
			{
				var extendName = Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_BASENAME + i.ToString();
				result.Add(extendName, string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, extendName));

				var extendDate = Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE_BASENAME + i.ToString();
				result.Add(extendDate, string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, extendDate));
			}
		}

		if (Constants.ORDER_EXTEND_OPTION_ENABLED
			&& ((masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER)
				|| (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM)))
		{
			foreach (var model in DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels)
			{
				result.Add(model.SettingId, string.Format("{0}.{1}", Constants.TABLE_ORDER, model.SettingId));
			}
		}

		if (Constants.ORDER_EXTEND_OPTION_ENABLED
			&& ((masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE)
				|| (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM)))
		{
			foreach (var model in DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels)
			{
				result.Add(model.SettingId, string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, model.SettingId));
			}
		}

		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING)
		{
			// ユーザー拡張項目のフィールド名を取得できるよう、動的部分のHashtable作成
			var userExtendSettingList = new UserExtendSettingList(this.LoginOperatorName);

			foreach (var userExtendSetting in userExtendSettingList.Items)
			{
				result.Add(
					Constants.FIELD_USER_EXTEND_CONVERTING_NAME + userExtendSetting.SettingId,
					Constants.TABLE_USEREXTEND + "." + userExtendSetting.SettingId + " as "
					+ Constants.FIELD_USER_EXTEND_CONVERTING_NAME + userExtendSetting.SettingId);
			}
			var userAttributeFields = MasterExportSettingUtility.GetMasterExportSettingFields(
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERATTRIBUTE,
				Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING);

			foreach (var key in userAttributeFields.Keys)
			{
				result[key] = userAttributeFields[key];
			}

			// 商品タグマスター
			foreach (DataRowView drv in GetProductTagSetting())
			{
				result.Add(
					Constants.FIELD_PRODUCT_TAG_CONVERTING_NAME + drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID],
					"w2_ProductTag." + drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID] + " as "
					+ Constants.FIELD_PRODUCT_TAG_CONVERTING_NAME + drv[Constants.FIELD_PRODUCTTAGSETTING_TAG_ID]);
			}

			// 商品拡張項目マスター
			foreach (DataRowView drv in GetProductExtendSetting(this.LoginOperatorShopId))
			{
				result.Add(
					Constants.FIELD_PRODUCT_EXTEND_CONVERTING_NAME + "extend" + ((int)drv[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO]),
					"w2_ProductExtend.extend" + ((int)drv[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO]) + " as "
					+ Constants.FIELD_PRODUCT_EXTEND_CONVERTING_NAME + "extend" + ((int)drv[Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NO]));
			}

			MasterExportSettingUtility.SetFieldInfoForOrder(result, masterKbn);

			// Extend status and Extend status date for Fixed Purchase and Fixed Purchase item
			for (var i = 1; i <= Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX; i++)
			{
				var extendName = Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_BASENAME + i;
				result.Add(
					Constants.FIELD_FIXEDPURCHASE_CONVERTING_NAME + extendName,
					string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, extendName) + " as "
					+ Constants.FIELD_FIXEDPURCHASE_CONVERTING_NAME + extendName);

				var extendDate = Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE_BASENAME + i;
				result.Add(
					Constants.FIELD_FIXEDPURCHASE_CONVERTING_NAME + extendDate,
					string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, extendDate) + " as "
					+ Constants.FIELD_FIXEDPURCHASE_CONVERTING_NAME + extendDate);
			}

			foreach (var model in DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels)
			{
				result.Add(Constants.FIELD_ORDER_EXTEND_SETTING_CONVERTING_NAME + model.SettingId,
					string.Format("{0}.{1}", Constants.TABLE_ORDER, model.SettingId) + " as "
					+ Constants.FIELD_ORDER_EXTEND_SETTING_CONVERTING_NAME + model.SettingId);
			}

			foreach (var model in DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels)
			{
				result.Add(Constants.FIELD_FIXEDPURCHASE_CONVERTING_NAME + model.SettingId,
					string.Format("{0}.{1}", Constants.TABLE_FIXEDPURCHASE, model.SettingId) + " as "
					+ Constants.FIELD_FIXEDPURCHASE_CONVERTING_NAME + model.SettingId);
			}
		}
		return result;
	}

	/// <summary>
	/// マスタ出力定義情報データビュー取得
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="masterKbn">マスタ区分</param>
	/// <returns>マスタ出力定義情報リスト</returns>
	private MasterExportSettingModel[] GetMasterExportSettingDataView(string shopId, string masterKbn)
	{
		var result = (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_MALLEXHIBITSCONFIG)
				? GetMasterExportSettingDataViewForMall(shopId, masterKbn)
				: new MasterExportSettingService().GetAllByMaster(shopId, masterKbn);
		return result;
		}

	/// <summary>
	/// マスタ出力定義情報取得(モール用)
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="masterKbn">マスタ区分</param>
	/// <returns>マスタ出力定義情報データビュー</returns>
	/// <remarks>マスタ情報を仮登録し、データビューを取得する</remarks>
	private MasterExportSettingModel[] GetMasterExportSettingDataViewForMall(string shopId, string masterKbn)
	{
		MasterExportSettingModel[] result = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			sqlAccessor.BeginTransaction();

			// モール連携設定取得
			DataView dvMallCooperationSettings = MallPage.GetMallCooperationSettingAll(this.LoginOperatorShopId);

			// フィールド列作成
			string strFields = Constants.FIELD_MALLEXHIBITSCONFIG_SHOP_ID + "," + Constants.FIELD_MALLEXHIBITSCONFIG_PRODUCT_ID;
			int iExhibitsCount = 1;
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLCOOPERATIONSETTING, Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG))
			{
				foreach (DataRowView drv in dvMallCooperationSettings)
				{
					if ((string)drv[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG] == li.Value)
					{
						strFields += "," + Constants.CONST_MALLEXHIBITSCONFIG_EXHIBITS_FLG + iExhibitsCount.ToString();
						break;
					}
				}
				iExhibitsCount++;
			}

			var masterExportSettingService = new MasterExportSettingService();
			masterExportSettingService.Insert(
				new MasterExportSettingModel
			{
					ShopId = shopId,
					MasterKbn = masterKbn,
					Fields = strFields,
					LastChanged = "",
					ExportFileType = Constants.FLG_MASTEREXPORTSETTING_EXPORT_FILE_TYPE_CSV,
					SettingName = "",
				},
				sqlAccessor);

			// マスタ出力定義取得
			result = masterExportSettingService.GetAllByMaster(shopId, masterKbn, sqlAccessor);

			sqlAccessor.RollbackTransaction();
		}
		return result;
	}

	/// <summary>
	/// Is Get Combine Fixed Purchase Data
	/// </summary>
	/// <param name="field">Field</param>
	/// <param name="masterKbn">Master Kbn</param>
	/// <returns>True if has get combine fixed purchase data</returns>
	private bool IsGetTargetOfCombineFixedPurchaseData(string field, string masterKbn)
	{
		masterKbn = MasterExportSettingUtility.GetMasterKbnException(masterKbn);
		var isValidMasterKbn = ((masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER)
			|| (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM));

		return isValidMasterKbn
			&& (field.Contains(Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE)
				|| field.Contains(Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE)
				|| field.Contains(Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN));
		}

	/// <summary>
	/// 実行
	/// </summary>
	/// <param name="setting">出力設定</param>
	/// <param name="sqlParams">検索パラメタ</param>
	/// <param name="sqlFieldNames">フィールド名（カンマ区切り）</param>
	/// <param name="outputStream">出力ストリーム</param>
	/// <param name="type">出力ファイル種別</param>
	public bool Export(MasterExportSettingModel setting, Hashtable sqlParams, string sqlFieldNames, Stream outputStream, FileType type)
	{
		var excelTemplateSetting = new ExcelTemplateSetting(
			Path.Combine(Constants.PHYSICALDIRPATH_COMMERCE_MANAGER, @"Contents\MasterExportExcelTemplate\TemplateBase.xml"),
			Path.Combine(Constants.PHYSICALDIRPATH_COMMERCE_MANAGER, @"Contents\MasterExportExcelTemplate\TemplateElements.xml")
		);

		// 日付フォーマット設定※グローバル対応なしの場合、デフォルトフォーマットで日付を抽出
		var formatDate = (Constants.GLOBAL_OPTION_ENABLE && (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false))
			? GlobalConfigUtil.GetDateTimeFormatText(
				Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecondNoneServerTime)
			: Constants.CONST_SHORTDATETIME_2LETTER_FORMAT;

		var replacePrice = CultureInfo.CreateSpecificCulture(Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId).NumberFormat.CurrencyDecimalSeparator;

		//基軸通貨 小数点以下の有効桁数
		var digitsByKeyCurrency = Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.CurrencyDecimalDigits
			?? CultureInfo.CreateSpecificCulture(Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId).NumberFormat.CurrencyDecimalDigits;

		switch (setting.MasterKbn)
			{
			// ユーザーマスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERSHIPPING:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERINVOICE:
				if (type == FileType.Csv) return new UserService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new UserService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			// 商品マスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCT:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTVARIATION:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTPRICE:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTTAG:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTEXTEND:
				if (type == FileType.Csv) return new ProductService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new ProductService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			// 注文マスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER_WORKFLOW:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM_WORKFLOW:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION_WORKFLOW:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING_WORKFLOW:
				var replacesValue = OrderCommon.GetOrderSearchOrderByStringForOrderItemListAndWorkflow(
					(string)sqlParams["sort_kbn"],
					setting.MasterKbn,
					new OrderService().GetStatementNameInfo(setting.MasterKbn));
				var replacesValueForMultiOrderId = OrderCommon.GetOrderSearchMultiOrderId(sqlParams);
				if (type == FileType.Csv) return new OrderService().ExportToCsv(setting,
					sqlParams,
					sqlFieldNames,
					outputStream,
					formatDate,
					replacesValue,
					replacesValueForMultiOrderId,
					digitsByKeyCurrency,
					replacePrice);
				if (type == FileType.Excel) return new OrderService().ExportToExcel(setting, sqlParams,
					sqlFieldNames,
					excelTemplateSetting,
					outputStream,
					formatDate,
					replacesValue,
					replacesValueForMultiOrderId,
					digitsByKeyCurrency,
					replacePrice);
				break;

			// 定期マスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE_WORKFLOW:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASEITEM_WORKFLOW:
				if (type == FileType.Csv) return new FixedPurchaseService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new FixedPurchaseService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			// 商品在庫表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSTOCK:
				if (type == FileType.Csv) return new ProductStockService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new ProductStockService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			// 商品カテゴリ表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTCATEGORY:
				if (type == FileType.Csv) return new ProductCategoryService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new ProductCategoryService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			// 商品セール価格
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTSALEPRICE:
				if (type == FileType.Csv) return new ProductSaleService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new ProductSaleService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			// ショートURL
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SHORTURL:
				if (type == FileType.Csv) return new ShortUrlService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new ShortUrlService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, replacePrice, digitsByKeyCurrency);
				break;

			// シリアルキー
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SERIALKEY:
				if (type == FileType.Csv) return new SerialKeyService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new SerialKeyService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			// リアル店舗
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOP:
				if (type == FileType.Csv) return new RealShopService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new RealShopService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			// 発注マスタ表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDER:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STOCKORDERITEM:
				if (type == FileType.Csv) return new StockOrderService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new StockOrderService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			// モール出品設定マスタ表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_MALLEXHIBITSCONFIG:
				if (type == FileType.Csv) return new MallExhibitsConfigService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new MallExhibitsConfigService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			// 入荷通知メールマスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL:
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL_DETAIL:
				if (type == FileType.Csv) return new UserProductArrivalMailService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new UserProductArrivalMailService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			// 商品レビュー
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTREVIEW:
				if (type == FileType.Csv) return new ProductReviewService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new ProductReviewService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			// リアル店舗商品在庫
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_REALSHOPPRODUCTSTOCK:
				if (type == FileType.Csv) return new RealShopProductStockService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new RealShopProductStockService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			// オペレータマスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_OPERATOR:
				if (type == FileType.Csv) return new ShopOperatorService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new ShopOperatorService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;
		}
		throw new Exception("未対応のマスタ区分：" + setting.MasterKbn);
	}
}
