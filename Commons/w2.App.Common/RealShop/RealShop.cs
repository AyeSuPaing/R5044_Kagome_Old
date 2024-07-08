/*
=========================================================================================================
  Module      : リアル店舗共通処理クラス(RealShop.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Sql;

namespace w2.App.Common.RealShop
{
	/// <summary>
	/// リアル店舗共通処理クラス
	/// </summary>
	public class RealShop
	{
		/// <summary>
		/// リアル店舗情報取得
		/// </summary>
		/// <returns>リアル店舗情報</returns>
		public static DataView GetRealShopAll()
		{
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("RealShop", "GetRealShopAll"))
			{
				var input = new Hashtable();
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
			}
		}

		/// <summary>
		/// リアル店舗情報取得
		/// </summary>
		/// <param name="realShopId">リアル店舗ID</param>
		/// <returns>リアル店舗情報</returns>
		public static DataView GetRealShopDetail(string realShopId)
		{
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("RealShop", "GetRealShopInfoByRealShopId"))
			{
				var input = new Hashtable
				{
					{ Constants.FIELD_REALSHOP_REAL_SHOP_ID, realShopId },
				};
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
			}
		}

		/// <summary>
		/// リアル店舗情報登録
		/// </summary>
		/// <param name="input">リアル店舗情報</param>
		public static int InsertRealShop(Hashtable input)
		{
			var inserted = 0;
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("RealShop", "InsertRealShop"))
			{
				inserted = sqlStatement.ExecStatementWithOC(sqlAccessor, input);
				if (inserted < 1) throw new Exception("登録0件です");
			}
			return inserted;
		}

		/// <summary>
		/// リアル店舗情報を更新 
		/// </summary>
		/// <param name="input">リアル店舗情報</param>
		public static void UpdateRealShop(Hashtable input)
		{
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("RealShop", "UpdateRealShop"))
			{
				sqlStatement.ExecStatementWithOC(sqlAccessor, input);
			}
		}
	}
}
