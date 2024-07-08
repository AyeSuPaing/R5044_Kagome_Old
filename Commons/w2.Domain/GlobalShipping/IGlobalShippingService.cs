/*
=========================================================================================================
  Module      : 海外配送エリア構成サービスのインタフェース(IGlobalShippingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
namespace w2.Domain.GlobalShipping
{
	/// <summary>
	/// 海外配送エリア構成サービスのインタフェース
	/// </summary>
	public interface IGlobalShippingService : IService
	{
		/// <summary>
		/// 配送種別と配送会社で海外配送エリアの送料を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別</param>
		/// <param name="areaId">エリア</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <returns>結果</returns>
		GlobalShippingPostageModel[] GetAreaPostageByShippingDeliveryCompany(
			string shopId,
			string shippingId,
			string areaId,
			string deliveryCompanyId);

		/// <summary>
		/// 住所の情報をもとに海外配送エリアを振り分ける
		/// </summary>
		/// <param name="countryIsoCode">ISO</param>
		/// <param name="addr5">住所5</param>
		/// <param name="addr4">住所4</param>
		/// <param name="addr3">住所3</param>
		/// <param name="addr2">住所2</param>
		/// <param name="zip">郵便番号</param>
		/// <returns>該当するエリア</returns>
		GlobalShippingAreaComponentModel DistributesShippingArea(
			string countryIsoCode,
			string addr5,
			string addr4,
			string addr3,
			string addr2,
			string zip);
	}
}
