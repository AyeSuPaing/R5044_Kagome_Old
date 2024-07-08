/*
=========================================================================================================
  Module      : ブランド情報クラス(ProductBrand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using w2.Common.Util;
using w2.Common.Web;
using w2.Common.Sql;

namespace w2.App.Common.Product
{
	///*********************************************************************************************
	/// <summary>
	/// ブランド情報クラス
	/// </summary>
	///*********************************************************************************************
	public class ProductBrandUtility
	{
		/// <summary>情報格納用(永続的に保持・ロックのために実体作成)</summary>
		private static DataView m_dvProductBrand = new DataView();

		/// <summary>表示データ更新日時(永続的に保持)</summary>
		private static DateTime m_dtUpdateDate = new DateTime();

		/// <summary>
		/// ブランド情報数の取得
		/// </summary>
		/// <returns>ブランド数</returns>
		public static int GetProductBrandCount()
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("ProductBrand", "GetProductBrandCount"))
			{
				DataView dvProductBrandList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor);

				if (dvProductBrandList.Count != 0)
				{
					return (int)dvProductBrandList[0]["row_count"];
				}
			}

			return 0;
		}

		/// <summary>
		/// ブランド情報リストの取得
		/// </summary>
		/// <returns>ブランド情報</returns>
		public static DataView GetProductBrandList()
		{
			DataView dvBrand = null;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("ProductBrand", "GetProductBrandList"))
			{
				Hashtable htInput = new Hashtable();

				dvBrand = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			return dvBrand;
		}

		/// <summary>
		/// ブランド情報リスト（ページャー）の取得
		/// </summary>
		/// <returns>ブランド情報</returns>
		public static DataView GetProductBrandListPager(int iPageBgn, int iPageEnd)
		{
			DataView dvBrand = null;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("ProductBrand", "GetProductBrandListPager"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add("bgn_row_num", iPageBgn);
				htInput.Add("end_row_num", iPageEnd);

				dvBrand = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			return dvBrand;
		}

		/// <summary>
		/// ブランドの削除
		/// </summary>
		public static void DeleteBrand(string strBrandId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				using (SqlStatement sqlStatement = new SqlStatement("ProductBrand", "DeleteProductBrand"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_PRODUCTBRAND_BRAND_ID, strBrandId);

					sqlStatement.ExecStatement(sqlAccessor, htInput);
				}
			}
		}

		/// <summary>
		/// デフォルトブランド取得
		/// </summary>
		/// <returns>ブランド情報</returns>
		public static DataView GetDefaultBrand()
		{
			DataView dvBrand = null;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("ProductBrand", "GetProductDefaultBrand"))
			{
				Hashtable htInput = new Hashtable();
				dvBrand = sqlStatement.SelectSingleStatementWithOC(sqlAccessor);
			}

			return dvBrand;
		}

		/// <summary>
		/// デフォルトブランド取得
		/// </summary>
		/// <returns>ブランドID</returns>
		public static string GetDefaultBrandId()
		{
			DataView dvBrand = GetDefaultBrand();

			if (dvBrand.Count != 0)
			{
				return (string)dvBrand[0][Constants.FIELD_PRODUCTBRAND_BRAND_ID];
			}

			return "";
		}

		/// <summary>
		/// デフォルトブランドセット
		/// </summary>
		public static void SetDefaultBrandId(string strBrandId, string strOperatorName)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("ProductBrand", "SetProductDefaultBrand"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_PRODUCTBRAND_BRAND_ID, strBrandId);
				htInput.Add(Constants.FIELD_PRODUCTBRAND_LAST_CHANGED, strOperatorName);

				sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// デフォルトブランドリセット
		/// </summary>
		public static void ResetDefaultBrandId(string strOperatorName)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("ProductBrand", "ResetProductDefaultBrand"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_PRODUCTBRAND_LAST_CHANGED, strOperatorName);

				sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// ブランド情報取得
		/// </summary>
		/// <param name="strBrandId">ブランドID</param>
		/// <returns>ブランド情報</returns>
		public static DataView GetProductBrand(string strBrandId)
		{
			DataView dvBrand = null;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("ProductBrand", "GetProductBrand"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_PRODUCTBRAND_BRAND_ID, strBrandId);

				dvBrand = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			return dvBrand;
			// TODO: モバイルで使われているが、それはキャッシュを利用するべき。
		}

		/// <summary>
		/// ブランド情報取得（キャッシュから）
		/// </summary>
		/// <param name="strBrandId">ブランドID</param>
		/// <returns>ブランド情報</returns>
		public static List<DataRowView> GetBrandDataFromCache(string strBrandId)
		{
			// TODO: Listである必要は無く、DataRowViewを返せば良い
			List<DataRowView> ldrv = new List<DataRowView>();

			//------------------------------------------------------
			// 更新タイミング判定
			//------------------------------------------------------
			lock (m_dvProductBrand)
			{
				if (DateTime.Now >= m_dtUpdateDate)
				{
					m_dvProductBrand = GetProductBrandList();

					// 次回更新は5分後とする
					m_dtUpdateDate = DateTime.Now.AddMinutes(5);
				}
			}

			//------------------------------------------------------
			// ブランドの検索
			//------------------------------------------------------
			foreach (DataRowView drv in m_dvProductBrand)
			{
				if ((string)drv[Constants.FIELD_PRODUCTBRAND_BRAND_ID] == strBrandId)
				{
					ldrv.Add(drv);
					break;
				}
			}

			return ldrv;
		}

		/// <summary>
		/// ブランド情報インサート
		/// </summary>
		public static void InsertBrand(Hashtable htInput, SqlAccessor sqlAccessor)
		{
			using (SqlStatement sqlStatement = new SqlStatement("ProductBrand", "InsertProductBrand"))
			{
				sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// ブランド情報アップデート
		/// </summary>
		public static void UpdateBrand(Hashtable htInput, SqlAccessor sqlAccessor)
		{
			using (SqlStatement sqlStatement = new SqlStatement("ProductBrand", "UpdateProductBrand"))
			{
				sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// ブランド名取得
		/// </summary>
		/// <param name="strBrandId">ブランドID</param>
		/// <returns>ブランド名</returns>
		public static string GetProductBrandName(string strBrandId)
		{
			List<DataRowView> lProduct = GetBrandDataFromCache(strBrandId);

			if (lProduct.Count != 0)
			{
				return (string)lProduct[0][Constants.FIELD_PRODUCTBRAND_BRAND_NAME];
			}

			return "";
		}
	}
}