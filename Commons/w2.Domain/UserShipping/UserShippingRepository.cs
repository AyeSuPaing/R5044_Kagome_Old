/*
=========================================================================================================
  Module      : ユーザー配送先情報リポジトリ (UserShippingRepository.cs)
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

namespace w2.Domain.UserShipping
{
	/// <summary>
	/// ユーザー配送先情報リポジトリ
	/// </summary>
	internal class UserShippingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "UserShipping";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal UserShippingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal UserShippingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Search 検索（一覧）
		/// <summary>
		/// 検索（一覧）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="beginRowNum">開始行番号</param>
		/// <param name="endRowNum">終了行番号</param>
		/// <returns>モデル配列</returns>
		internal UserShippingModel[] Search(string userId, int beginRowNum, int endRowNum)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERSHIPPING_USER_ID, userId},
				{"bgn_row_num", beginRowNum},
				{"end_row_num", endRowNum},
			};
			var dv = Get(XML_KEY_NAME, "Search", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserShippingModel(drv)).ToArray();
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
		internal int GetSearchHitCount(string userId, int beginRowNum, int endRowNum)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERSHIPPING_USER_ID, userId},
				{"bgn_row_num", beginRowNum},
				{"end_row_num", endRowNum},
			};
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="shippingNo">配送先枝番</param>
		/// <returns>モデル</returns>
		internal UserShippingModel Get(string userId, int shippingNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERSHIPPING_USER_ID, userId},
				{Constants.FIELD_USERSHIPPING_SHIPPING_NO, shippingNo},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new UserShippingModel(dv[0]);
		}
		#endregion

		#region +GetAll 取得（全て）
		/// <summary>
		/// 取得（全て）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>モデル列</returns>
		internal UserShippingModel[] GetAll(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERSHIPPING_USER_ID, userId}
			};
			var dv = Get(XML_KEY_NAME, "GetAll", ht);
			if (dv.Count == 0) return new UserShippingModel[0];

			return dv.Cast<DataRowView>().Select(drv => new UserShippingModel(drv)).OrderBy(us => us.ShippingNo).ToArray();
		}
		#endregion

		#region +GetNewShippingNo ユーザー配送先の新しい配送番号取得
		/// <summary>
		/// ユーザー配送先の新しい配送番号取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>モデル</returns>
		internal int GetNewShippingNo(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERSHIPPING_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetNewShippingNo", ht);
			return (int)dv[0][0];
		}
		#endregion
		

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(UserShippingModel model)
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
		internal int Update(UserShippingModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="userId">ユーザID</param>
		/// <param name="shippingNo">配送先枝番</param>
		internal int Delete(string userId, int shippingNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERSHIPPING_USER_ID, userId},
				{Constants.FIELD_USERSHIPPING_SHIPPING_NO, shippingNo},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion
	}
}
