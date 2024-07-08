/*
=========================================================================================================
  Module      : 店舗情報クラス(Shop.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;

namespace w2.Commerce.MallBatch.Mkadv.Common
{
	///**************************************************************************************
	/// <summary>
	/// 店舗情報クラス
	/// </summary>
	///**************************************************************************************
	public class Shop
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public Shop()
		{
			// プロパティ初期化
			this.ShopIds = new List<string>();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="iAdtoId">商品コンバータID</param>
		/// <remarks>全ての店舗情報を取得する</remarks>
		public Shop(SqlAccessor sqlAccessor, int iAdtoId) : this()
		{
			InitShop(sqlAccessor, iAdtoId);
		}

		/// <summary>
		/// 全ての店舗情報を取得する
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="iAdtoId">商品コンバータID</param>
		private void InitShop(SqlAccessor sqlAccessor, int iAdtoId)
		{
			DataView dvShopIds = null;
			using (SqlStatement sqlStatement = new SqlStatement("Adto", "GetShopId"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLPRDCNV_ADTO_ID, iAdtoId);

				dvShopIds = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
			}

			this.ShopIds = new List<string>();
			foreach (DataRowView drvShopIds in dvShopIds)
			{
				this.ShopIds.Add((string)drvShopIds[Constants.FIELD_MALLPRDCNV_SHOP_ID]);
			}
		}

		/// <summary>店舗ID</summary>
		public List<string> ShopIds { get; private set; }
	}
}
