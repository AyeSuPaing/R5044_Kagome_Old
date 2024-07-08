/*
=========================================================================================================
  Module      : 配送会社サービスのインタフェース(IUserCreditCardService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.DeliveryCompany
{
	/// <summary>
	///  配送会社サービスのインタフェース
	/// </summary>
	public interface IDeliveryCompanyService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deliveryCompanyId">Delivery company ID</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>モデル</returns>
		DeliveryCompanyModel Get(string deliveryCompanyId, SqlAccessor accessor = null);

		/// <summary>
		/// Get all
		/// </summary>
		/// <returns>Delivery Companys</returns>
		DeliveryCompanyModel[] GetAll();

		/// <summary>
		/// 配送種別に紐づける配送サービス情報取得
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>モデル配列</returns>
		DeliveryCompanyModel[] GetByShippingId(string shippingId);

		/// <summary>
		/// 配送希望時間帯ID検索
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="shippingKbn">配送区分</param>
		/// <param name="shippingTimeMsg">配送希望時間帯文言</param>
		/// <returns>配送希望時間帯ID</returns>
		string SearchShippingTimeId(string shippingId, string shippingKbn, string shippingTimeMsg);

		/// <summary>
		/// 配送希望時間帯マッチング文言取得
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="shippingKbn">配送区分</param>
		/// <returns>配送希望時間帯モデル</returns>
		DeliveryCompanyModel GetShippingTimeMatching(string shippingId, string shippingKbn);

		/// <summary>
		/// 注文IDをもとに配送会社取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		DeliveryCompanyModel GetByOrderId(string orderId, SqlAccessor accessor = null);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="beginRowNum">開始行</param>
		/// <param name="endRowNum">終了行</param>
		/// <returns>配送会社リスト</returns>
		DeliveryCompanyModel[] Search(int beginRowNum, int endRowNum);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">Sql accessor</param>
		void Insert(DeliveryCompanyModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>件数</returns>
		/// <param name="accessor">Sql accessor</param>
		int Update(DeliveryCompanyModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deliveryCompanyId">Delivery company ID</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>件数</returns>
		int Delete(string deliveryCompanyId, SqlAccessor accessor = null);

		/// <summary>
		/// Get delivery company names by delivery company ids
		/// </summary>
		/// <param name="deliveryCompanyIds">Delivery company ids</param>
		/// <returns>Delivery company names</returns>
		string[] GetDeliveryCompanyNamesByDeliveryCompanyIds(string[] deliveryCompanyIds);
	}
}
