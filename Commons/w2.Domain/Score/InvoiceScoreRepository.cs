/*
=========================================================================================================
  Module      : スコア後払い請求書リポジトリ (InvoiceScoreRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.Score
{
	/// <summary>
	/// スコア後払い請求書リポジトリ
	/// </summary>
	internal class InvoiceScoreRepository : RepositoryBase
	{
		/// <returns>テーブル名</returns>
		private const string XML_KEY_NAME = "InvoiceScore";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal InvoiceScoreRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal InvoiceScoreRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデル</returns>
		internal InvoiceScoreModel Get(string orderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_INVOICE_SCORE_ORDER_ID, orderId },
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;

			return new InvoiceScoreModel(dv[recordIndex: 0]);
		}
		#endregion

		#region ~Insert 請求書登録
		/// <summary>
		/// 請求書登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(InvoiceScoreModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~InsertDetail 請求書明細登録
		/// <summary>
		/// 請求書明細登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertDetail(InvoiceScoreDetailModel model)
		{
			Exec(XML_KEY_NAME, "InsertDetail", model.DataSource);
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>件数</returns>
		internal int Delete(string orderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_INVOICE_SCORE_ORDER_ID, orderId },
			};

			return Exec(XML_KEY_NAME, "Delete", ht);
		}
		#endregion

		#region ~DeleteDetail 請求書明細削除
		/// <summary>
		/// 請求書明細削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>件数</returns>
		internal int DeleteDetail(string orderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_INVOICE_SCORE_ORDER_ID, orderId },
			};

			return Exec(XML_KEY_NAME, "DeleteDetail", ht);
		}
		#endregion
	}
}
