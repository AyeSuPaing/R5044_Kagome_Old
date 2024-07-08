/*
=========================================================================================================
  Module      : ユーザ電子発票管理情報リポジトリ (TwUserInvoiceRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.TwUserInvoice
{
	/// <summary>
	/// ユーザ電子発票管理情報リポジトリ
	/// </summary>
	internal class TwUserInvoiceRepository : RepositoryBase
	{
		/// <returns>Xml Key Name</returns>
		private const string XML_KEY_NAME = "TwUserInvoice";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TwUserInvoiceRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">Accessor</param>
		public TwUserInvoiceRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="beginRowNum">開始行番号</param>
		/// <param name="endRowNum">終了行番号</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(string userId, int beginRowNum, int endRowNum)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_USERSHIPPING_USER_ID, userId },
				{ "bgn_row_num", beginRowNum },
				{ "end_row_num", endRowNum }
			};
			var data = Get(XML_KEY_NAME, "GetSearchHitCount", input);

			return (int)data[0][0];
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="beginRowNum">開始行番号</param>
		/// <param name="endRowNum">終了行番号</param>
		/// <returns>検索結果列</returns>
		public TwUserInvoiceModel[] Search(string userId, int beginRowNum, int endRowNum)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_USERSHIPPING_USER_ID, userId },
				{ "bgn_row_num", beginRowNum },
				{ "end_row_num", endRowNum }
			};
			var datas = Get(XML_KEY_NAME, "Search", input);
			var result = datas.Cast<DataRowView>().Select(data => new TwUserInvoiceModel(data)).ToArray();

			return result;
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="twInvoiceNo">電子発票管理枝番</param>
		/// <returns>モデル</returns>
		public TwUserInvoiceModel Get(string userId, int twInvoiceNo)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_TWUSERINVOICE_USER_ID, userId },
				{ Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NO, twInvoiceNo }
			};
			var data = Get(XML_KEY_NAME, "Get", input);
			var result = ((data.Count == 0)
				? null
				: new TwUserInvoiceModel(data[0]));

			return result;
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(TwUserInvoiceModel model)
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
		public void Update(TwUserInvoiceModel model)
		{
			Exec(XML_KEY_NAME, "Update", model.DataSource);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="userId">ユーザID</param>
		/// <param name="twInvoiceNo">電子発票管理枝番</param>
		public void Delete(string userId, int twInvoiceNo)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_TWUSERINVOICE_USER_ID, userId },
				{ Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NO, twInvoiceNo }
			};

			Exec(XML_KEY_NAME, "Delete", input);
		}
		#endregion

		#region +Get All User Invoice By User Id
		/// <summary>
		/// Get All User Invoice By User Id
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>All User Invoice By User Id</returns>
		public TwUserInvoiceModel[] GetAllUserInvoiceByUserId(string userId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_TWUSERINVOICE_USER_ID, userId }
			};
			var datas = Get(XML_KEY_NAME, "GetAllUserInvoiceByUserId", input);
			if (datas.Count == 0) return null;

			var result = datas.Cast<DataRowView>().Select(data => new TwUserInvoiceModel(data)).ToArray();

			return result;
		}
		#endregion

		#region +Get New Invoice No
		/// <summary>
		/// Get New Invoice No
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>モデル</returns>
		internal int GetNewInvoiceNo(string userId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_TWUSERINVOICE_USER_ID, userId }
			};
			var data = Get(XML_KEY_NAME, "GetNewInvoiceNo", input);

			return (int)data[0][0];
		}
		#endregion
	}
}
