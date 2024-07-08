/*
=========================================================================================================
  Module      : ターゲットリスト登録URLクリエータ (ImportTargetListUrlCreator.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Web;

/// <summary>
/// ターゲットリスト登録URLクリエータ
/// </summary>
public class ImportTargetListUrlCreator
{
	/// <summary>
	/// Create url target list
	/// </summary>
	/// <param name="windowKbn">Window kbn</param>
	/// <param name="productId">Product id</param>
	/// <param name="variationId">Variation id</param>
	/// <param name="fixedPurchaseYear">Fixed purchase year</param>
	/// <param name="fixedPurchaseMonth">Fixed purchase month</param>
	/// <param name="orderMonth">Order month</param>
	/// <param name="title">Title</param>
	/// <param name="pageNumber">Page number</param>
	/// <returns>Url target list</returns>
	public static string CreateUrlTargetList(
		string windowKbn,
		string productId,
		string variationId,
		int fixedPurchaseYear,
		int fixedPurchaseMonth,
		int orderMonth,
		int title,
		int pageNumber)
	{
		var url = new UrlCreator(Constants.PATH_ROOT_MP + Constants.PAGE_MANAGER_FIXEDPURCHASEREPEATANALYSISREPORT_TARGETLIST)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, windowKbn)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, productId)
			.AddParam(Constants.REQUEST_KEY_VARIATION_ID, variationId)
			.AddParam(Constants.REQUEST_KEY_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_FIXEDPURCHASE_ORDER_YEAR, fixedPurchaseYear.ToString())
			.AddParam(Constants.REQUEST_KEY_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_FIXEDPURCHASE_ORDER_MONTH, fixedPurchaseMonth.ToString())
			.AddParam(Constants.REQUEST_KEY_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_ORDER_MONTH, orderMonth.ToString())
			.AddParam(Constants.REQUEST_KEY_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_TITLE, title.ToString())
			.AddParam(Constants.REQUEST_KEY_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_PAGE_NUMBER, pageNumber.ToString())
			.CreateUrl();
		return url;
	}
}