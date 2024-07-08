/*
=========================================================================================================
  Module      : Atodene後払い請求書リポジトリ (AtodeneInvoiceRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.Atodene
{
	/// <summary>
	/// Atodene後払い請求書リポジトリ
	/// </summary>
	internal class AtodeneRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "Atodene";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AtodeneRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AtodeneRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		/// <summary>
		/// 請求書取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>請求書モデル</returns>
		internal InvoiceAtodeneModel Get(string orderId)
		{
			var dv = Get(
				XML_KEY_NAME,
				"Get",
				new Hashtable
				{
					{ Constants.FIELD_INVOICEATODENE_ORDER_ID, orderId }
				});
			if (dv.Count == 0)
			{
				return null;
			}

			var model = new InvoiceAtodeneModel(dv[0]);
			model.Details = dv[0].DataView.Cast<DataRowView>().Select(drv => new InvoiceAtodeneDetailModel(drv)).ToArray();
			return model;
		}

		/// <summary>
		/// 請求書登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>処理件数</returns>
		internal int Insert(InvoiceAtodeneModel model)
		{
			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource);
			return result;
		}

		/// <summary>
		/// 請求書明細登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>処理件数</returns>
		internal int InsertDetail(InvoiceAtodeneDetailModel model)
		{
			var result = Exec(XML_KEY_NAME, "InsertDetail", model.DataSource);
			return result;
		}

		/// <summary>
		/// 請求書削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="orderId">注文ID</param>
		public int Delete(string orderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_INVOICEATODENE_ORDER_ID, orderId },
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}

		/// <summary>
		/// 請求書明細削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="orderId">注文ID</param>
		public int DeleteDetails(string orderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_INVOICEATODENE_ORDER_ID, orderId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteDetails", ht);
			return result;
		}
	}
}
