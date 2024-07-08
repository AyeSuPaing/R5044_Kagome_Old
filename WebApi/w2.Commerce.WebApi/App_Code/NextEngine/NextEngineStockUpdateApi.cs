/*
=========================================================================================================
  Module      : ネクストエンジン在庫連携API (NextEngineStockUpdateApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Web;
using w2.App.Common.MallCooperation;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.MallCooperationSetting;
using w2.Domain.ProductStock;
using w2.Domain.ProductStockHistory;

/// <summary>
/// ネクストエンジン在庫連携API
/// </summary>
public class NextEngineStockUpdateApi
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="context">コンテキスト</param>
	public NextEngineStockUpdateApi(HttpContext context)
	{
		this.Context = context;
		this.MallWatchingLogManager = new MallWatchingLogManager();
		this.NextEngineCooperationSettingModels = new MallCooperationSettingService()
			.GetValidByMallKbn(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_NEXT_ENGINE).FirstOrDefault(
				m => (m.ValidFlg == Constants.FLG_MALLCOOPERATIONSETTING_VALID_FLG_VALID));
	}

	/// <summary>
	/// 在庫同期リクエストによる処理
	/// </summary>
	public void ProcessRequestContext()
	{
		if (this.NextEngineCooperationSettingModels == null) return;

		var request = new NextEngineStockUpdateRequest(this.Context.Request);

		if ((request.ChekcRequired() == false)
			|| (request.CheckStoreAccount(this.NextEngineCooperationSettingModels.NextEngineStockStoreAccount) == false)
			|| (request.CheckSig(this.NextEngineCooperationSettingModels.NextEngineStockAuthKey) == false))
		{
			this.Context.Response.Write("Error");
			return;
		}

		/*
		 *※下記条件に合致する商品の場合、在庫数は空で送ります。
・		 * 商品区分「20：受注発注」
		 * 商品区分「10：予約」かつ、予約在庫数「99999」
		 */
		if (string.IsNullOrEmpty(request.Stock))
		{
			ResponseWrite(request, NextEngineStockUpdateResponse.Processed.Success);
			return;
		}

		try
		{
			string productId;
			string productVariationId;
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("NextEngine", "GetProduct"))
			{
				var ht = new Hashtable()
				{
					{ Constants.FIELD_PRODUCT_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID },
					{ Constants.FIELD_PRODUCTVARIATION_VARIATION_ID, request.Code }
				};
				var product = statement.SelectSingleStatementWithOC(accessor, ht);
				if (product.Count != 1)
				{
					this.MallWatchingLogManager.Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_NEXT_ENGINE_API,
						this.NextEngineCooperationSettingModels.MallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
						"該当商品なし",
						this.Context.Request.Url.AbsoluteUri);

					ResponseWrite(request, NextEngineStockUpdateResponse.Processed.Success);
					return;
				}

				productId = (string)product[0][Constants.FIELD_PRODUCT_PRODUCT_ID];
				productVariationId = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_ID];
			}

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var productStockService = new ProductStockService();
				var productStockModel = productStockService.Get(
					Constants.CONST_DEFAULT_SHOP_ID,
					productId,
					productVariationId);
				if (productStockModel != null)
				{
					if (productStockModel.Stock != request.StockNum)
					{
						productStockModel.Stock = request.StockNum;
						productStockModel.LastChanged = this.NextEngineCooperationSettingModels.MallId;
						productStockService.Update(productStockModel, accessor);
						var productHistoryModel = new ProductStockHistoryModel()
						{
							OrderId = "",
							ShopId = Constants.CONST_DEFAULT_SHOP_ID,
							ProductId = productId,
							VariationId = productVariationId,
							ActionStatus = Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_EXTERNAL_API,
							LastChanged = this.NextEngineCooperationSettingModels.MallId,
							AddStock = request.StockNum,
							SyncFlg = false,
							UpdateMemo = string.Format("ネクストエンジン在庫同期 : Code : {0}", request.Code)
						};
						new ProductStockHistoryService().Insert(productHistoryModel, accessor);
					}
					accessor.CommitTransaction();
				}
				else
				{
					accessor.CommitTransaction();
					this.MallWatchingLogManager.Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_NEXT_ENGINE_API,
						this.NextEngineCooperationSettingModels.MallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
						"在庫管理されていません。",
						this.Context.Request.Url.AbsoluteUri);
				}
			}
		}
		catch (Exception e)
		{
			ResponseWrite(request, NextEngineStockUpdateResponse.Processed.SystemError);
			FileLogger.WriteError("ネクストエンジン 在庫連携エラー " + this.Context.Request.Url.AbsoluteUri, e);
			this.MallWatchingLogManager.Insert(
				Constants.FLG_MALLWATCHINGLOG_BATCH_ID_NEXT_ENGINE_API,
				this.NextEngineCooperationSettingModels.MallId,
				Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
				"ネクストエンジン 在庫連携エラー",
				this.Context.Request.Url.AbsoluteUri);
			return;
		}

		ResponseWrite(request, NextEngineStockUpdateResponse.Processed.Success);
	}

	/// <summary>
	/// レスポンス書き込み
	/// </summary>
	/// <param name="request">リクエスト内容</param>
	/// <param name="processed">処理結果</param>
	private void ResponseWrite(NextEngineStockUpdateRequest request, NextEngineStockUpdateResponse.Processed processed)
	{
		var response = new NextEngineStockUpdateResponse();
		var responseText = response.GetResponseText(request, processed);

		this.Context.Response.Clear();
		this.Context.Response.ContentType = "text/xml";
		this.Context.Response.ContentEncoding = NextEngineStockUpdateResponse.ENCODING;
		this.Context.Response.Write(responseText);
	}

	/// <summary>有効なネクストエンジン設定</summary>
	private MallCooperationSettingModel NextEngineCooperationSettingModels { get; set; }
	/// <summary>モール監視ログマネージャー</summary>
	private MallWatchingLogManager MallWatchingLogManager { get; set; }
	/// <summary>HTTPコンテキスト</summary>
	private HttpContext Context { get; set; }
}
