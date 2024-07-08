/*
=========================================================================================================
  Module      : 配送リードタイムリポジトリ (DeliveryLeadTimeRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.DeliveryLeadTime
{
	/// <summary>
	/// 配送リードタイムリポジトリ
	/// </summary>
	internal class DeliveryLeadTimeRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "DeliveryLeadTime";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public DeliveryLeadTimeRepository()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DeliveryLeadTimeRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get All取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <returns>モデル</returns>
		public DeliveryLeadTimeModel[] GetAll(string shopId, string deliveryCompanyId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_DELIVERYLEADTIME_SHOP_ID, shopId},
				{Constants.FIELD_DELIVERYLEADTIME_DELIVERY_COMPANY_ID, deliveryCompanyId},
			};
			var data = Get(XML_KEY_NAME, "GetAll", input);

			if (data.Count == 0) return new DeliveryLeadTimeModel[0];

			return data.Cast<DataRowView>().Select(dataRow => new DeliveryLeadTimeModel(dataRow)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public int Insert(DeliveryLeadTimeModel model)
		{
			var inserted = Exec(XML_KEY_NAME, "Insert", model.DataSource);

			return inserted;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(string shopId, string deliveryCompanyId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_DELIVERYLEADTIME_SHOP_ID, shopId },
				{ Constants.FIELD_DELIVERYLEADTIME_DELIVERY_COMPANY_ID, deliveryCompanyId },
			};

			return Exec(XML_KEY_NAME, "Delete", input);
		}
		#endregion
	}
}