/*
=========================================================================================================
  Module      : 配送会社リポジトリ (DeliveryCompanyRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.Domain.DeliveryCompany
{
	/// <summary>
	/// 配送会社リポジトリ
	/// </summary>
	public class DeliveryCompanyRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "DeliveryCompany";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal DeliveryCompanyRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal DeliveryCompanyRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deliveryCompanyId">Delivery company ID</param>
		/// <returns>モデル</returns>
		internal DeliveryCompanyModel Get(string deliveryCompanyId)
		{
			var parameter = new Hashtable
			{
				{Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_ID, deliveryCompanyId},
			};
			var deliveryCompanyList = Get(XML_KEY_NAME, "Get", parameter);

			if (deliveryCompanyList.Count == 0) return null;

			return new DeliveryCompanyModel(deliveryCompanyList[0]);
		}
		#endregion

		#region +Get all
		/// <summary>
		/// Get all
		/// </summary>
		/// <returns>Delivery Companys</returns>
		internal DeliveryCompanyModel[] GetAll()
		{
			var data = Get(XML_KEY_NAME, "GetAll");

			return data.Cast<DataRowView>().Select(item => new DeliveryCompanyModel(item)).ToArray();
		}
		#endregion

		#region +GetByShippingId 配送種別に紐づける配送サービス情報取得
		/// <summary>
		/// 配送種別に紐づける配送サービス情報取得
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>モデル配列</returns>
		internal DeliveryCompanyModel[] GetByShippingId(string shippingId)
		{
			var parameter = new Hashtable
			{
				{Constants.FIELD_SHOPSHIPPING_SHIPPING_ID, shippingId},
			};
			var data = Get(XML_KEY_NAME, "GetByShippingId", parameter);

			return data.Cast<DataRowView>().Select(item => new DeliveryCompanyModel(item)).ToArray();
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
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_ID, shippingId},
				{Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_KBN, shippingKbn},
				{"shipping_time_message", shippingTimeMsg},
			};
			var data = Get(XML_KEY_NAME, "SearchShippingTimeId", ht);
			if (data.Count == 0) return null;
			return StringUtility.ToEmpty(data[0]["shipping_time_id"]);
		}
		#endregion

		#region +GetShippingTimeMatching 配送希望時間帯マッチング文言取得
		/// <summary>
		/// 配送希望時間帯マッチング文言取得
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="shippingKbn">配送区分</param>
		/// <returns> 配送希望時間帯モデル</returns>
		public DeliveryCompanyModel GetShippingTimeMatching(string shippingId, string shippingKbn)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_ID, shippingId},
				{Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_KBN, shippingKbn},
			};
			var data = Get(XML_KEY_NAME, "GetShippingTimeMatching", ht);
			if (data.Count == 0) return null;
			return new DeliveryCompanyModel(data[0]);
		}
		#endregion

		#region ~GetByOrderId 注文IDをもとに配送会社取得
		/// <summary>
		/// 注文IDをもとに配送会社取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデル</returns>
		internal DeliveryCompanyModel GetByOrderId(string orderId)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetByOrderId",
				new Hashtable
				{
					{ Constants.FIELD_ORDER_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID },
					{ Constants.FIELD_ORDER_ORDER_ID, orderId },
				});
			if (dv.Count == 0) return null;
			return new DeliveryCompanyModel(dv[0]);
		}
		#endregion

		#region +Search
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="beginRowNum">開始行</param>
		/// <param name="endRowNum">終了行</param>
		/// <returns>配送会社リスト</returns>
		internal DeliveryCompanyModel[] Search(int beginRowNum, int endRowNum)
		{
			var param = new Hashtable
				{
					{ "bgn_row_num", beginRowNum },
					{ "end_row_num",endRowNum },
				};

			var dv = Get(XML_KEY_NAME, "Search", param);
			return dv.Cast<DataRowView>().Select(drv => new DeliveryCompanyModel(drv)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(DeliveryCompanyModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(DeliveryCompanyModel model)
		{
			return Exec(XML_KEY_NAME, "Update", model.DataSource);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="deliveryCompanyId">Delivery company ID</param>
		internal int Delete(string deliveryCompanyId)
		{
			var parameter = new Hashtable
			{
				{Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_ID, deliveryCompanyId},
			};

			return Exec(XML_KEY_NAME, "Delete", parameter);
		}
		#endregion

		#region +GetDeliveryCompanyNamesByDeliveryCompanyIds
		/// <summary>
		/// Get delivery company names by delivery company ids
		/// </summary>
		/// <param name="deliveryCompanyIds">Delivery company ids</param>
		/// <returns>Delivery company names</returns>
		internal string[] GetDeliveryCompanyNamesByDeliveryCompanyIds(string[] deliveryCompanyIds)
		{
			var replaceKeyValues = new[]
			{
				new KeyValuePair<string, string>(
					"@@ delivery_company_ids @@",
					string.Join(",", deliveryCompanyIds.Select(deliveryCompanyId => string.Format("'{0}'", deliveryCompanyId.Replace("'", "''"))))),
			};
			var dv = Get(XML_KEY_NAME, "GetDeliveryCompanyNamesByDeliveryCompanyIds", replaces: replaceKeyValues);
			var deliveryCompanyNames = dv.Cast<DataRowView>()
				.Select(drv => (string)drv[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_NAME])
				.ToArray();
			return deliveryCompanyNames;
		}
		#endregion
	}
}
