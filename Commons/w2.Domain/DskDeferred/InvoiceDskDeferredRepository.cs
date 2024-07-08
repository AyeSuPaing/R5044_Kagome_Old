/*
=========================================================================================================
  Module      : DSK後払い請求書リポジトリ (InvoiceDskDeferredRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using w2.Common.Sql;
using w2.Domain.InvoiceDskDeferredDetail;

namespace w2.Domain.InvoiceDskDeferred
{
	/// <summary>
	/// DSK後払い請求書リポジトリ
	/// </summary>
	internal class InvoiceDskDeferredRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "InvoiceDskDeferred";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal InvoiceDskDeferredRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal InvoiceDskDeferredRepository(SqlAccessor accessor)
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
		internal InvoiceDskDeferredModel Get(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_INVOICEDSKDEFERRED_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new InvoiceDskDeferredModel(dv[0]);
		}
		#endregion

		#region ~Insert 請求書登録
		/// <summary>
		/// 請求書登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(InvoiceDskDeferredModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~InsertDetail 請求書明細登録
		/// <summary>
		/// 請求書明細登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertDetail(InvoiceDskDeferredDetailModel model)
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
				{ Constants.FIELD_INVOICEDSKDEFERRED_ORDER_ID, orderId },
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
				{ Constants.FIELD_INVOICEDSKDEFERRED_ORDER_ID, orderId },
			};

			return Exec(XML_KEY_NAME, "DeleteDetail", ht);
		}
		#endregion
	}
}
