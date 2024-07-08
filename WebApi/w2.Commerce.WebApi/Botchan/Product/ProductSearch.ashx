<%--
=========================================================================================================
  Module      : 商品検索 Search(ProductSearch.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="Botchan.Product.ProductSearch" %>
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BotchanApi;
using w2.App.Common;
using w2.App.Common.Botchan;
using w2.App.Common.Global.Region;
using w2.App.Common.Order.Botchan;
using w2.Common.Util;

namespace Botchan.Product
{
	/// <summary>
	/// 商品検索
	/// </summary>
	public class ProductSearch : BotchanApiBase, IHttpHandler
	{
		/// <summary>
		/// Process request
		/// </summary>
		/// <param name="context">Http context</param>
		public void ProcessRequest(HttpContext context)
		{
			BotChanApiProcess(context, w2.App.Common.Constants.BOTCHAN_API_NAME_PRODUCT_SEARCH);
		}

		/// <summary>
		/// レスポンス取得
		/// </summary>
		/// <param name="context">httpコンテキスト</param>
		/// <param name="requestContents">リクエスト文字列</param>
		/// <param name="errorList">エラーリスト</param>
		/// <param name="memo">メモ</param>
		/// <returns>レスポンス</returns>
		protected override object GetResponseData(
			HttpContext context,
			string requestContents,
			ref List<BotchanMessageManager.MessagesCode> errorList,
			ref string memo)
		{
			var productSearchRequest = JsonConvert.DeserializeObject<ProductSearchRequest>(requestContents);
			var productIds = productSearchRequest.SearchProductList
				.Select(product => product.ProductId)
				.Take(w2.App.Common.Constants.BOTCHAN_LIMIT_PRODUCT_ID_FOR_SEARCH)
				.ToArray();
			var productSearchResponses = ProductSearchUtility.GetProducts(
				w2.Domain.Constants.CONST_DEFAULT_SHOP_ID,
				productIds,
				ref errorList,
				ref memo);
			return productSearchResponses;
		}

		/// <summary>
		/// BOTCHAN共通チェック
		/// </summary>
		/// <param name="requestContents">リクエスト文字列</param>
		/// <param name="apiName">API名</param>
		/// <returns>エラーリスト</returns>
		protected override List<BotchanMessageManager.MessagesCode> BotChanUtilityValidate(string requestContents, string apiName)
		{
			var productSearchRequest = JsonConvert.DeserializeObject<ProductSearchRequest>(requestContents);
			var validate = new Hashtable { { "AuthText", productSearchRequest.AuthText } };
			var errorList = BotChanUtility.ValidateRequest(validate, apiName);
			return errorList;
		}

		/// <summary>
		/// パラメータバリエーション
		/// </summary>
		/// <param name="requestContents">リクエスト文字列</param>
		/// <returns>エラーリスト</returns>
		protected override Validator.ErrorMessageList ParametersValidate(string requestContents)
		{
			var productSearchRequest = JsonConvert.DeserializeObject<ProductSearchRequest>(requestContents).SearchProductList;

			var errorList = new Validator.ErrorMessageList();
			for (var index = 0; index < productSearchRequest.Length; index++)
			{
				SearchProduct element = productSearchRequest[index];
				var dicParam = new Hashtable
			{
				{Constants.REQUEST_KEY_PRODUCT_ID, element.ProductId}
			};
				var list = Validator.Validate(
					Constants.CHECK_KBN_BOTCHAN_PRODUCT_SEARCH,
					dicParam,
					Constants.GLOBAL_OPTION_ENABLE ? RegionManager.GetInstance().Region.LanguageLocaleId : "",
					"");

				errorList.AddRange(list.Select(error =>
					new KeyValuePair<string, string>(string.Format("[{0}番目の商品]{1}", (index + 1), error.Key),
						error.Value)));
			}

			return errorList;
		}

		/// <summary>Is Reusable</summary>
		public bool IsReusable
		{
			get { return false; }
		}
	}
}