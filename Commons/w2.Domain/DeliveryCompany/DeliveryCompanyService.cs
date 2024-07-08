/*
=========================================================================================================
  Module      : 配送会社サービス (DeliveryCompanyService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using w2.Common.Sql;

namespace w2.Domain.DeliveryCompany
{
	/// <summary>
	/// 配送会社サービス
	/// </summary>
	public class DeliveryCompanyService : ServiceBase, IDeliveryCompanyService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deliveryCompanyId">Delivery company ID</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>モデル</returns>
		public DeliveryCompanyModel Get(string deliveryCompanyId, SqlAccessor accessor = null)
		{
			using (var repository = new DeliveryCompanyRepository(accessor))
			{
				return repository.Get(deliveryCompanyId);
			}
		}
		#endregion

		#region +Get all
		/// <summary>
		/// Get all
		/// </summary>
		/// <returns>Delivery Companys</returns>
		public DeliveryCompanyModel[] GetAll()
		{
			using (var repository = new DeliveryCompanyRepository())
			{
				return repository.GetAll();
			}
		}
		#endregion

		#region +GetByShippingId 配送種別に紐づける配送サービス情報取得
		/// <summary>
		/// 配送種別に紐づける配送サービス情報取得
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>モデル配列</returns>
		public DeliveryCompanyModel[] GetByShippingId(string shippingId)
		{
			using (var repository = new DeliveryCompanyRepository())
			{
				return repository.GetByShippingId(shippingId);
			}
		}
		#endregion

		#region +SearchShippingTimeId 配送希望時間帯ID検索
		/// <summary>
		/// 配送希望時間帯ID検索
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="shippingKbn">配送区分</param>
		/// <param name="shippingTimeMsg">配送希望時間帯文言</param>
		/// <returns>配送希望時間帯ID</returns>
		public string SearchShippingTimeId(string shippingId, string shippingKbn, string shippingTimeMsg)
		{
			using (var repository = new DeliveryCompanyRepository())
			{
				return repository.SearchShippingTimeId(shippingId, shippingKbn, shippingTimeMsg);
			}
		}
		#endregion

		#region +GetShippingTimeMatching 配送希望時間帯マッチング文言取得
		/// <summary>
		/// 配送希望時間帯マッチング文言取得
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="shippingKbn">配送区分</param>
		/// <returns>配送希望時間帯モデル</returns>
		public DeliveryCompanyModel GetShippingTimeMatching(string shippingId, string shippingKbn)
		{
			using (var repository = new DeliveryCompanyRepository())
			{
				return repository.GetShippingTimeMatching(shippingId, shippingKbn);
			}
		}
		#endregion

		#region ~GetByOrderId 注文IDをもとに配送会社取得
		/// <summary>
		/// 注文IDをもとに配送会社取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public DeliveryCompanyModel GetByOrderId(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new DeliveryCompanyRepository(accessor))
			{
				var result = repository.GetByOrderId(orderId);
				return result;
			}
		}
		#endregion

		#region +Search
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="beginRowNum">開始行</param>
		/// <param name="endRowNum">終了行</param>
		/// <returns>配送会社リスト</returns>
		public DeliveryCompanyModel[] Search(int beginRowNum, int endRowNum)
		{
			using (var repository = new DeliveryCompanyRepository())
			{
				return repository.Search(beginRowNum, endRowNum);
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">Sql accessor</param>
		public void Insert(DeliveryCompanyModel model, SqlAccessor accessor = null)
		{
			using (var repository = new DeliveryCompanyRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>件数</returns>
		/// <param name="accessor">Sql accessor</param>
		public int Update(DeliveryCompanyModel model, SqlAccessor accessor = null)
		{
			using (var repository = new DeliveryCompanyRepository(accessor))
			{
				return repository.Update(model);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deliveryCompanyId">Delivery company ID</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>件数</returns>
		public int Delete(string deliveryCompanyId, SqlAccessor accessor = null)
		{
			using (var repository = new DeliveryCompanyRepository(accessor))
			{
				return repository.Delete(deliveryCompanyId);
			}
		}
		#endregion

		#region +GetDeliveryCompanyNamesByDeliveryCompanyIds
		/// <summary>
		/// Get delivery company names by delivery company ids
		/// </summary>
		/// <param name="deliveryCompanyIds">Delivery company ids</param>
		/// <returns>Delivery company names</returns>
		public string[] GetDeliveryCompanyNamesByDeliveryCompanyIds(string[] deliveryCompanyIds)
		{
			using (var repository = new DeliveryCompanyRepository())
			{
				var deliveryCompanyNames = repository.GetDeliveryCompanyNamesByDeliveryCompanyIds(deliveryCompanyIds);
				return deliveryCompanyNames;
			}
		}
		#endregion
	}
}
