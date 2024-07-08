/*
=========================================================================================================
  Module      : ＆mall在庫引当API (AndmallStockAllocationApi.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using w2.App.Common.MallCooperation;
using w2.App.Common.Option;
using w2.App.Common.Stock;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.AndmallInventoryReserve;
using w2.Domain.MallCooperationSetting;
using w2.Domain.ProductStock;
using w2.Domain.ProductStockHistory;

/// <summary>
/// ＆mall在庫引当API
/// </summary>
public class AndmallStockAllocation
{
	/// <summary>引当モード</summary>
	private const int MODE_RESERVE = 20;
	/// <summary>引当キャンセルモード</summary>
	private const int MODE_CANCEL = 21;
	/// <summary>サイトコード ヘッダー名</summary>
	private const string HEADER_NAME_SITECODE = "X-Scs-Sitecode";
	/// <summary>サイト認証ハッシュ ヘッダー名</summary>
	private const string HEADER_NAME_SIGNATURE = "X-Scs-Signature";

	/// <summary>
	/// コンストラクタ
	/// </summary>
	public AndmallStockAllocation()
	{
		this.ErrorMessageManager = new ErrorMessageManager();
		this.MallWatchingLogManager = new MallWatchingLogManager();
		this.AndmallCooperationSettingModels = new MallCooperationSettingService()
			.GetValidByMallKbn(Constants.CONST_DEFAULT_SHOP_ID, Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_ANDMALL)
			.Where(m => m.ValidFlg == Constants.FLG_MALLCOOPERATIONSETTING_VALID_FLG_VALID).ToArray();
	}

	/// <summary>
	/// ＆mall在庫引当リクエストによる在庫引当処理
	/// </summary>
	/// <param name="context">HTTPコンテキスト</param>
	public void ProcessRequestContext(HttpContext context)
	{
		// 有効な＆mallショップが１つも存在しない場合は処理しない
		if (this.AndmallCooperationSettingModels.Length == 0) return;

		this.Context = context;

		// リクエストボディの読み込み
		var requestContents = GetRequest();
		if (string.IsNullOrEmpty(requestContents) == false)
		{
			FileLogger.WriteInfo(string.Format("リクエスト:{0}", requestContents));
		}
		else
		{
			var resultParamError = new ResultModel();
			SetResultError(resultParamError, ErrorMessageManager.ResultCode.E0001);
			ResultResuponse(resultParamError);
			FileLogger.WriteError("パラメータの不足");
			return;
		}

		// ヘッダー情報の認証
		if (CheckHeader(requestContents,
			this.AndmallCooperationSettingModels.FirstOrDefault().AndmallSiteCode,
			this.AndmallCooperationSettingModels.FirstOrDefault().AndmallSignatureKey) == false) return;

		// リクエストボディのJSONエンコード
		RequestModel requestModel = null;
		try
		{
			requestModel = JsonConvert.DeserializeObject<RequestModel>(requestContents);
			if (requestModel == null) throw new Exception();
		}
		catch (Exception ex)
		{
			var resultJsonError = new ResultModel();
			SetResultError(resultJsonError, ErrorMessageManager.ResultCode.E0003);
			ResultResuponse(resultJsonError);
			FileLogger.WriteError("JSONエンコードエラー", ex);
			return;
		}

		if ((requestModel.TotalRequest == 0) || (requestModel.Requests == null))
		{
			var resultParamError = new ResultModel();
			SetResultError(resultParamError, ErrorMessageManager.ResultCode.E0001);
			ResultResuponse(resultParamError);
			FileLogger.WriteError("処理するリクエストがありません。");
			return;
		}

		// 引当処理の実施
		try
		{
			FileLogger.WriteInfo(string.Format("処理開始 識別コード:{0}", string.Join(",", requestModel.Requests.Select(i => i.IdentificationCode).ToArray())));
			ResultResuponse(Exec(requestModel));
			FileLogger.WriteInfo(string.Format("処理終了 識別コード:{0}", string.Join(",", requestModel.Requests.Select(i => i.IdentificationCode).ToArray())));
		}
		catch (Exception ex)
		{
			var resultExceptionError = new ResultModel();
			SetResultError(resultExceptionError, ErrorMessageManager.ResultCode.E9999);
			ResultResuponse(resultExceptionError);
			FileLogger.WriteError("システムエラー", ex);
			return;
		}
	}

	/// <summary>
	/// 実行
	/// </summary>
	/// <param name="model">リクエストモデル</param>
	/// <returns>レスポンスモデル</returns>
	private ResultModel Exec(RequestModel model)
	{
		ResultModel resultModel = null;
		try
		{
			resultModel = ConvertRequest(model);
			SetResultError(resultModel, ErrorMessageManager.ResultCode.S0000);
		}
		catch (Exception ex)
		{
			var resultParamError = new ResultModel();
			SetResultError(resultParamError, ErrorMessageManager.ResultCode.E0001);
			FileLogger.WriteError("パラメータ設定エラー", ex);
			return resultParamError;
		}

		foreach (Result result in resultModel.Results)
		{
			foreach (ResultItemize itemize in result.Items)
			{
				SetDetailsResultError(itemize, ErrorMessageManager.DetailsResultCode.S0000);

				var mallSetting = this.AndmallCooperationSettingModels.FirstOrDefault(
					m => (m.AndmallBaseStoreCode == itemize.BaseStoreCode)
					&& (m.AndmallTenantCode == itemize.CorporationCode)
					&& (m.AndmallSiteCode == itemize.SiteCode)
					&& (m.AndmallShopNo == result.SqlCode));
				if (mallSetting == null)
				{
					SetResultError(resultModel, ErrorMessageManager.ResultCode.E0030);
					SetDetailsResultError(itemize, ErrorMessageManager.DetailsResultCode.E0010);
					FileLogger.WriteError(string.Format("引当 ショップコードに紐づくモール設定が見つかりません \n識別コード:{0} ショップコード:{1} SKUID:{2}",
						result.IdentificationCode,
						itemize.BaseStoreCode,
						itemize.CsCode));
					continue;
				}

				var modifyModel = new AndmallInventoryReserveService().Get(result.IdentificationCode, itemize.CsCode, itemize.BaseStoreCode);

				switch (result.Mode)
				{
					case MODE_RESERVE:
						if (modifyModel == null)
						{
							ExecModeReserve(result.IdentificationCode, resultModel, itemize, mallSetting);
						}
						else
						{
							ExecModeReserveModify(result.IdentificationCode, resultModel, itemize, modifyModel);
						}
						break;

					case MODE_CANCEL:
						ExecModeCancel(result.IdentificationCode, resultModel, itemize, modifyModel);
						break;

					default:
						SetResultError(resultModel, ErrorMessageManager.ResultCode.E0001);
						break;
				}
			}
		}

		//モール監視ログの出力
		foreach (var mall in this.AndmallCooperationSettingModels)
		{
			var successReserveItems = resultModel.Results
				.Where(r => r.Mode == MODE_RESERVE)
				.SelectMany(r => r.Items)
				.Where(i => (i.BaseStoreCode == mall.AndmallBaseStoreCode) && (i.ProcessedCode == ErrorMessageManager.CodeKeyName(ErrorMessageManager.DetailsResultCode.S0000)))
				.ToArray();
			var successCanselItems = resultModel.Results
				.Where(r => r.Mode == MODE_CANCEL)
				.SelectMany(r => r.Items)
				.Where(i => (i.BaseStoreCode == mall.AndmallBaseStoreCode) && (i.ProcessedCode == ErrorMessageManager.CodeKeyName(ErrorMessageManager.DetailsResultCode.S0000)))
				.ToArray();
			var errorReserveItems = resultModel.Results
				.Where(r => r.Mode == MODE_RESERVE)
				.SelectMany(r => r.Items)
				.Where(i => (i.BaseStoreCode == mall.AndmallBaseStoreCode) && (i.ProcessedCode != ErrorMessageManager.CodeKeyName(ErrorMessageManager.DetailsResultCode.S0000)))
				.ToArray();
			var errorCanselItems = resultModel.Results
				.Where(r => r.Mode == MODE_CANCEL)
				.SelectMany(r => r.Items)
				.Where(i => (i.BaseStoreCode == mall.AndmallBaseStoreCode) && (i.ProcessedCode != ErrorMessageManager.CodeKeyName(ErrorMessageManager.DetailsResultCode.S0000)))
				.ToArray();

			if ((successReserveItems.Length > 0) || (successCanselItems.Length > 0))
			{
				this.MallWatchingLogManager.Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_ANDMALL_STOCK_API,
					mall.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS, string.Format("ショップコード「{0}」の在庫引当に成功", mall.AndmallBaseStoreCode),
					string.Format("在庫引当:\n{0} \n\n在庫引当キャンセル:\n{1}",
						string.Join("\n", successReserveItems.Select(
							i => string.Format("識別コード:{0} SKUID:{1} 商品ID:{2} 引当数量:{3}",
								i.IdentificationCode,
								i.CsCode,
								i.VariationId,
								(i.Quantity * -1).ToString()))),
						string.Join(",", successCanselItems.Select(
							i => string.Format("識別コード:{0} SKUID:{1} 商品ID:{2}",
								i.IdentificationCode,
								i.CsCode,
								i.VariationId)))));
			}

			if ((errorReserveItems.Length > 0) || (errorCanselItems.Length > 0))
			{
				this.MallWatchingLogManager.Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_ANDMALL_STOCK_API,
					mall.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR, string.Format("ショップコード「{0}」の在庫引当に失敗", mall.AndmallBaseStoreCode),
					string.Format("在庫引当:\n{0} \n\n在庫引当キャンセル:\n{1}",
						string.Join("\n", errorReserveItems.Select(
							i => string.Format("エラーコード:{0} メッセージ:{1} 識別コード:{2} SKUID:{3} 商品ID:{4}",
								i.ProcessedCode,
								i.ProcessedMessage,
								i.IdentificationCode,
								i.CsCode,
								i.VariationId))),
						string.Join(",", errorCanselItems.Select(
							i => string.Format("エラーコード:{0} メッセージ:{1} 識別コード:{2} SKUID:{3} 商品ID:{4}",
								i.ProcessedCode,
								i.ProcessedMessage,
								i.IdentificationCode,
								i.CsCode,
								i.VariationId)))));
			}
		}
		return resultModel;
	}

	/// <summary>
	/// 引当変更の処理
	/// </summary>
	/// <param name="identificationCode">識別コード</param>
	/// <param name="resultModel">レスポンス用モデル</param>
	/// <param name="resultItemize">引当変更商品</param>
	/// <param name="modifyModel">前回の引当情報モデル</param>
	private void ExecModeReserveModify(string identificationCode, ResultModel resultModel, ResultItemize resultItemize, AndmallInventoryReserveModel modifyModel)
	{
		if (modifyModel.Status == Constants.FLG_ANDMALLINVENTORYRESERVE_STATUS_CANCEL)
		{
			SetResultError(resultModel, ErrorMessageManager.ResultCode.E0030);
			SetDetailsResultError(resultItemize, ErrorMessageManager.DetailsResultCode.E0012);
			FileLogger.WriteError(string.Format("引当変更 引当情報のステータスが不正です。 \n識別コード:{0} product_id:{1} variation_id:{2}",
				identificationCode,
				modifyModel.ProductId,
				modifyModel.VariationId));
			return;
		}

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var productStockService = new ProductStockService();
			var productStockModel = productStockService.Get(Constants.CONST_DEFAULT_SHOP_ID, modifyModel.ProductId, modifyModel.VariationId, accessor);
			if (productStockModel == null)
			{
				SetResultError(resultModel, ErrorMessageManager.ResultCode.E0030);
				SetDetailsResultError(resultItemize, ErrorMessageManager.DetailsResultCode.E0012);
				FileLogger.WriteError(string.Format("引当変更 在庫情報が見つかりません \n識別コード:{0} product_id:{1} variation_id:{2}",
					identificationCode,
					modifyModel.ProductId,
					modifyModel.VariationId));
				return;
			}

			// ログ用
			resultItemize.VariationId = modifyModel.VariationId;
			resultItemize.IdentificationCode = identificationCode;

			var productUpdateStockModel = productStockService.UpdateProductStockAndGetStock(
				Constants.CONST_DEFAULT_SHOP_ID,
				modifyModel.ProductId, modifyModel.VariationId,
				((modifyModel.Quantity - resultItemize.Quantity) * -1),
				StockCommon.LAST_CHANGED_API,
				accessor);

			var productHistoryModel = new ProductStockHistoryModel()
			{
				OrderId = "",
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				ProductId = modifyModel.ProductId,
				VariationId = modifyModel.VariationId,
				ActionStatus = Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_RESERVED,
				LastChanged = StockCommon.LAST_CHANGED_API,
				AddStock = modifyModel.Quantity - resultItemize.Quantity,
				SyncFlg = false,
				UpdateMemo = string.Format("＆mallによる在庫連携 : 変更による引当調整 : SKUID : {0}", resultItemize.CsCode)
			};
			new ProductStockHistoryService().Insert(productHistoryModel);

			modifyModel.Quantity = resultItemize.Quantity;
			new AndmallInventoryReserveService().Update(modifyModel);

			resultItemize.AllocateQuantity = resultItemize.Quantity;
			resultItemize.AllocatableStock = productUpdateStockModel.Stock;

			FileLogger.WriteInfo(string.Format("在庫引当調整 識別コード:{0} sku_id:{1} product_id:{2} variation_id:{3} 前引当数量:{4} 後引当数量:{5}",
				identificationCode,
				modifyModel.ProductId,
				modifyModel.VariationId,
				resultItemize.CsCode,
				modifyModel.Quantity,
				resultItemize.Quantity));
		}
	}

	/// <summary>
	/// 引当処理
	/// </summary>
	/// <param name="identificationCode">識別コード</param>
	/// <param name="resultModel">レスポンス用モデル</param>
	/// <param name="resultItemize">引当商品</param>
	/// <param name="mallSetting">モール設定</param>
	private void ExecModeReserve(string identificationCode, ResultModel resultModel, ResultItemize resultItemize, MallCooperationSettingModel mallSetting)
	{
		var productId = string.Empty;
		var productVariationId = string.Empty;
		var andmallReservationFlg = string.Empty;

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			using (var statement = new SqlStatement("Andmall", "GetProduct"))
			{
				// 商品設定を指定
				statement.ReplaceStatement("## EXHIBITS_FLG ##",
					(string.IsNullOrEmpty(mallSetting.MallExhibitsConfig) == false)
						? string.Format(" AND {0}.{1} = {2} ",
							Constants.TABLE_MALLEXHIBITSCONFIG,
							MallOptionUtility.GetExhibitsConfigField(mallSetting.MallExhibitsConfig),
							Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)
						: string.Empty);
				// 商品IDの場合の連携カラム名を指定
				statement.ReplaceStatement("## SKU_PRODUCT_ID ##",
					string.Format(" AND {0}.{1} = '{2}'",
						Constants.TABLE_PRODUCT,
						mallSetting.AndmallCooperation,
						resultItemize.CsCode));
				// 商品バリエーションIDの場合のバリエーション連携カラム名を指定
				statement.ReplaceStatement("## SKU_VARIATION_ID ##",
					string.Format(" AND {0}.{1} = '{2}'",
						Constants.TABLE_PRODUCTVARIATION,
						mallSetting.AndmallVariationCooperation, resultItemize.CsCode));
				var product = statement.SelectSingleStatementWithOC(accessor);

				if (product.Count != 1)
				{
					SetResultError(resultModel, ErrorMessageManager.ResultCode.E0030);
					SetDetailsResultError(resultItemize, ErrorMessageManager.DetailsResultCode.E0013);
					FileLogger.WriteError(string.Format("引当 商品情報が見つかりません。 \n識別コード:{0} SKUID:{1} 商品ヒット数:{2}",
						identificationCode,
						resultItemize.CsCode,
						product.Count));
					return;
				}

				productId = (string)product[0][Constants.FIELD_PRODUCT_PRODUCT_ID];
				productVariationId = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_ID];
				andmallReservationFlg = (string)product[0][Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG];
			}

			if (andmallReservationFlg != ((resultItemize.SalesType) ?? Constants.FLG_PRODUCT_ANDMALL_RESERVATION_FLG_COMMON))
			{
				SetResultError(resultModel, ErrorMessageManager.ResultCode.E0030);
				SetDetailsResultError(resultItemize, ErrorMessageManager.DetailsResultCode.E0013);
				FileLogger.WriteError(string.Format("引当 販売方式が異なります \n識別コード:{0} product_id:{1} variation_id:{2} ＆mall側販売方式:{3}",
					identificationCode,
					productId, productVariationId,
					resultItemize.SalesType));
				return;
			}

			// ログ用
			resultItemize.VariationId = productVariationId;
			resultItemize.IdentificationCode = identificationCode;

			var productStockService = new ProductStockService();
			var productStockModel = productStockService.Get(Constants.CONST_DEFAULT_SHOP_ID, productId, productVariationId, accessor);
			if (productStockModel == null)
			{
				SetResultError(resultModel, ErrorMessageManager.ResultCode.E0030);
				SetDetailsResultError(resultItemize, ErrorMessageManager.DetailsResultCode.E0010);
				FileLogger.WriteError(string.Format("引当 在庫情報が見つかりません \n識別コード:{0} product_id:{1} variation_id:{2}",
					identificationCode,
					productId, productVariationId));
				return;
			}

			if (productStockModel.Stock - resultItemize.Quantity < 0)
			{
				SetDetailsResultError(resultItemize, ErrorMessageManager.DetailsResultCode.E0020);
				resultItemize.AllocateQuantity = 0;
				resultItemize.AllocatableStock = productStockModel.Stock;
				FileLogger.WriteError(string.Format("引当 在庫数が0以下になるため処理をスキップします \n識別コード:{0} product_id:{1} variation_id:{2}",
					identificationCode,
					productId, productVariationId));
				return;
			}
			var productUpdateStockModel = productStockService.UpdateProductStockAndGetStock(
				Constants.CONST_DEFAULT_SHOP_ID,
				productId, productVariationId,
				resultItemize.Quantity,
				StockCommon.LAST_CHANGED_API,
				accessor);

			var productHistoryModel = new ProductStockHistoryModel()
			{
				OrderId = "",
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				ProductId = productId,
				VariationId = productVariationId,
				ActionStatus = Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_RESERVED,
				LastChanged = StockCommon.LAST_CHANGED_API,
				AddStock = (resultItemize.Quantity * -1),
				SyncFlg = false,
				UpdateMemo = string.Format("＆mallによる在庫連携 : 引当 : SKUID : {0}", resultItemize.CsCode)
			};
			new ProductStockHistoryService().Insert(productHistoryModel);

			var insertModel = new AndmallInventoryReserveModel()
			{
				IdentificationCode = identificationCode,
				AndmallBaseStoreCode = resultItemize.BaseStoreCode,
				SkuId = resultItemize.CsCode,
				ProductId = productId,
				VariationId = productVariationId,
				Quantity = resultItemize.Quantity,
				Status = Constants.FLG_ANDMALLINVENTORYRESERVE_STATUS_ALLOCATION
			};
			new AndmallInventoryReserveService().Insert(insertModel);

			resultItemize.AllocateQuantity = resultItemize.Quantity;
			resultItemize.AllocatableStock = productUpdateStockModel.Stock;
		}
	}

	/// <summary>
	/// 引当キャンセル
	/// </summary>
	/// <param name="identificationCode">識別コード</param>
	/// <param name="resultModel">レスポンス用モデル</param>
	/// <param name="resultItemize">引当キャンセル商品</param>
	/// <param name="cancelModel">前回の引当情報モデル</param>
	private void ExecModeCancel(string identificationCode, ResultModel resultModel, ResultItemize resultItemize, AndmallInventoryReserveModel cancelModel)
	{
		if (cancelModel == null || cancelModel.Status == Constants.FLG_ANDMALLINVENTORYRESERVE_STATUS_CANCEL)
		{
			SetResultError(resultModel, ErrorMessageManager.ResultCode.E0031);
			SetDetailsResultError(resultItemize, ErrorMessageManager.DetailsResultCode.E0015);
			FileLogger.WriteError(string.Format("キャンセル 引当情報が見つかりません \n識別コード:{0} SKUID:{1}",
				identificationCode,
				resultItemize.CsCode));
			return;
		}
		cancelModel.Status = Constants.FLG_ANDMALLINVENTORYRESERVE_STATUS_CANCEL;
		cancelModel.CancelDate = DateTime.Now;

		// ログ用
		resultItemize.VariationId = cancelModel.VariationId;
		resultItemize.IdentificationCode = identificationCode;

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var productStockService = new ProductStockService();
			var productStockModel = productStockService.Get(Constants.CONST_DEFAULT_SHOP_ID, cancelModel.ProductId, cancelModel.VariationId, accessor);
			if (productStockModel == null)
			{
				SetResultError(resultModel, ErrorMessageManager.ResultCode.E0031);
				SetDetailsResultError(resultItemize, ErrorMessageManager.DetailsResultCode.E0015);
				FileLogger.WriteError(string.Format("キャンセル 在庫情報が見つかりません \n識別コード:{0} product_id:{1} variation_id:{2}",
					identificationCode,
					cancelModel.ProductId,
					cancelModel.VariationId));
				return;
			}

			var productUpdateStockModel = productStockService.UpdateProductStockAndGetStock(
				Constants.CONST_DEFAULT_SHOP_ID,
				cancelModel.ProductId, cancelModel.VariationId,
				(cancelModel.Quantity * -1),
				StockCommon.LAST_CHANGED_API, accessor);

			var productHistoryModel = new ProductStockHistoryModel()
			{
				OrderId = "",
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				ProductId = cancelModel.ProductId,
				VariationId = cancelModel.VariationId,
				ActionStatus = Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_FORWARD_CANCEL,
				LastChanged = StockCommon.LAST_CHANGED_API,
				AddStock = cancelModel.Quantity,
				SyncFlg = false,
				UpdateMemo = string.Format("＆mallによる在庫連携 : 引当キャンセル : SKUID : {0}", resultItemize.CsCode)
			};
			new ProductStockHistoryService().Insert(productHistoryModel);

			new AndmallInventoryReserveService().Update(cancelModel);

			resultItemize.AllocateQuantity = resultItemize.Quantity;
			resultItemize.AllocatableStock = productUpdateStockModel.Stock;
		}
	}

	/// <summary>
	/// リクエストモデルをレスポンス用モデルにコンバート
	/// </summary>
	/// <param name="model">リクエストモデル</param>
	/// <returns>レスポンス用モデル</returns>
	private ResultModel ConvertRequest(RequestModel model)
	{
		var result = new ResultModel()
		{
			TotalRequest = model.TotalRequest,
			Results = model.Requests.Select(r => new Result()
			{
				Number = r.Number,
				Mode = r.Mode,
				IdentificationCode = r.IdentificationCode,
				SqlCode = r.SqlCode,
				UseType = r.UseType,
				Items = r.Items.Select(i => new ResultItemize()
				{
					CorporationCode = i.CorporationCode,
					BaseStoreCode = i.BaseStoreCode,
					SiteCode = i.SiteCode,
					NewIdentificationCode = i.NewIdentificationCode,
					ProductCode = i.ProductCode,
					CsCode = i.CsCode,
					SalesType = i.SalesType,
					Quantity = i.Quantity
				}).ToList()
			}).ToList()
		};
		return result;
	}

	/// <summary>
	/// リクエストの取得
	/// </summary>
	/// <returns>リクエスト</returns>
	private string GetRequest()
	{
		using (var reader = new StreamReader(this.Context.Request.InputStream))
		{
			return reader.ReadToEnd();
		}
	}

	/// <summary>
	/// レスポンスボディの書き込み(application/json)
	/// </summary>
	/// <param name="model">レスポンス用モデル</param>
	private void ResultResuponse(ResultModel model)
	{
		this.Context.Response.ContentType = "application/json";
		this.Context.Response.Write(JsonConvert.SerializeObject(model));
	}

	/// <summary>
	/// 処理結果コードの書き込み
	/// </summary>
	/// <param name="model">レスポンス用モデル</param>
	/// <param name="code">処理結果コード</param>
	private void SetResultError(ResultModel model, ErrorMessageManager.ResultCode code)
	{
		model.ProcessedCode = ErrorMessageManager.CodeKeyName(code);
		model.ProcessedMessage = this.ErrorMessageManager.ResultErrorMessages[code];
	}

	/// <summary>
	/// 明細処理結果コードの書き込み
	/// </summary>
	/// <param name="model">引当商品情報</param>
	/// <param name="code">明細処理結果コード</param>
	private void SetDetailsResultError(ResultItemize model, ErrorMessageManager.DetailsResultCode code)
	{
		model.ProcessedCode = ErrorMessageManager.CodeKeyName(code);
		model.ProcessedMessage = this.ErrorMessageManager.DetailsResultErrorMessages[code];
	}

	/// <summary>
	/// ヘッダー認証
	/// </summary>
	/// <param name="requestBody">リクエストボディ</param>
	/// <param name="siteCode">サイトコード</param>
	/// <param name="sigunatureKey">サイト認証キー</param>
	/// <returns>成功:True 失敗:False</returns>
	private bool CheckHeader(string requestBody, string siteCode, string sigunatureKey)
	{
		var headers = this.Context.Request.Headers;
		if (headers.Get(HEADER_NAME_SITECODE) != siteCode)
		{
			var resultJsonError = new ResultModel();
			SetResultError(resultJsonError, ErrorMessageManager.ResultCode.E0004);
			ResultResuponse(resultJsonError);
			FileLogger.WriteError("サイトコードが不正");
			return false;
		}

		var sigunature = headers.Get(HEADER_NAME_SIGNATURE);
		if (headers.Get(HEADER_NAME_SIGNATURE) != CalculateHashSHA1(requestBody + sigunatureKey))
		{
			var resultJsonError = new ResultModel();
			SetResultError(resultJsonError, ErrorMessageManager.ResultCode.E0002);
			ResultResuponse(resultJsonError);
			FileLogger.WriteError("認証用ハッシュが正しくありません。");
			return false;
		}

		return true;
	}

	/// <summary>
	/// SHA1ハッシュ計算
	/// </summary>
	/// <param name="value">暗号化する文字列</param>
	/// <returns>ハッシュ値</returns>
	private string CalculateHashSHA1(string value)
	{
		var calculator = new SHA1Managed();

		var hash = calculator.ComputeHash(Encoding.UTF8.GetBytes(value));

		var hashString = new StringBuilder();
		foreach (byte b in hash)
		{
			hashString.Append(b.ToString("x2"));
		}
		return hashString.ToString();
	}

	/// <summary>有効な＆mall設定一覧</summary>
	private MallCooperationSettingModel[] AndmallCooperationSettingModels { get; set; }
	/// <summary>エラーメッセージマネージャー</summary>
	private ErrorMessageManager ErrorMessageManager { get; set; }
	/// <summary>モール監視ログマネージャー</summary>
	private MallWatchingLogManager MallWatchingLogManager { get; set; }
	/// <summary>HTTPコンテキスト</summary>
	private HttpContext Context { get; set; }
}