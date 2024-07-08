/*
=========================================================================================================
  Module      : 定期購入継続分析テーブルリポジトリ (FixedPurchaseRepeatAnalysisRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.FixedPurchaseRepeatAnalysis
{
	/// <summary>
	/// 定期購入継続分析テーブルリポジトリ
	/// </summary>
	internal class FixedPurchaseRepeatAnalysisRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "FixedPurchaseRepeatAnalysis";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseRepeatAnalysisRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FixedPurchaseRepeatAnalysisRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetRepeatAnalysisByOrderId 定期継続分析取得(注文ID)
		/// <summary>
		/// 定期継続分析取得(注文ID)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>定期継続分析のモデル</returns>
		public FixedPurchaseRepeatAnalysisModel[] GetRepeatAnalysisByOrderId(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "GetRepeatAnalysisByOrderId", ht);
			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseRepeatAnalysisModel(drv)).ToArray();
		}
		#endregion

		#region + GetRepeatAnalysisByUser 定期継続分析取得(ユーザーID)
		/// <summary>
		/// 定期継続分析取得(ユーザーID)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>購入回数のモデル</returns>
		public FixedPurchaseRepeatAnalysisModel[] GetRepeatAnalysisByUser(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_USER_ID, userId},
			};

			var dv = Get(XML_KEY_NAME, "GetRepeatAnalysisByUser", ht);

			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseRepeatAnalysisModel(drv)).ToArray();
		}
		#endregion

		#region + GetRepeatAnalysisByUserProduct 定期継続分析取得(ユーザーID、商品ID、バリエーションID)
		/// <summary>
		/// 定期継続分析取得(ユーザーID、商品ID、バリエーションID)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <returns>購入回数のモデル</returns>
		public FixedPurchaseRepeatAnalysisModel[] GetRepeatAnalysisByUserProduct(string userId, string productId, string variationId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_USER_ID, userId},
				{Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_PRODUCT_ID, productId},
				{Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_VARIATION_ID, variationId},
			};

			var dv = Get(XML_KEY_NAME, "GetRepeatAnalysisByUserProduct", ht);

			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseRepeatAnalysisModel(drv)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Insert(FixedPurchaseRepeatAnalysisModel model)
		{
			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource);
			return result;
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(FixedPurchaseRepeatAnalysisModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="dataNo">データNo</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(long dataNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_DATA_NO, dataNo},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region +DeleteByFixedPurchaseId 定期台帳IDをもとに削除
		/// <summary>
		/// 定期台帳IDをもとに削除
		/// </summary>
		/// <param name="fixedPurchaseId">定期台帳ID</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteByFixedPurchaseId(string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_FIXED_PURCHASE_ID, fixedPurchaseId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteByFixedPurchaseId", ht);
			return result;
		}
		#endregion
	}
}
