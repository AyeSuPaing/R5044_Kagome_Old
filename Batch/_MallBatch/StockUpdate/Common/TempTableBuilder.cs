/*
=========================================================================================================
  Module      : 連携用一時テーブル作成と情報保持 (TempTableBuilder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================


 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using w2.Common.Util;
using w2.Common.Sql;
using w2.Common.Logger;

namespace w2.Commerce.MallBatch.StockUpdate.Common
{
	///**************************************************************************************
	/// <summary>
	/// 商品コンバータ連携用一時テーブル作成・保持クラス（クラス名変更：TargetDataBuilde.cs → TempTableBuilder.cs）
	/// </summary>
	///**************************************************************************************
	public class TempTableBuilder
	{
		/// <summary>
		/// 一時テーブルを作成し、内容を構築する
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="strLogTableName">ログテーブル名</param>
		/// <param name="strInsertMasterKbn">マスター区分</param>
		/// <param name="strInsertActionKbn">アクション区分</param>
		/// <param name="blUseVariationFlg">バリエーション使用有無</param>
		/// <remarks>商品コンバータで一時テーブルを使用する</remarks>
		public TempTableBuilder(
			SqlAccessor sqlAccessor, 
			string strLogTableName, 
			string strInsertMasterKbn, 
			string strInsertActionKbn, 
			bool blUseVariationFlg)
		{
			//------------------------------------------------------
			// 一時テーブルを作成する
			//------------------------------------------------------
			// 一時テーブル名を取得する
			LogTableName = strLogTableName;
			ConvTableNameRakuten = strLogTableName + "01";
			ConvTableNameYahoo = strLogTableName + "03";
			ConvVTableNameRakuten = strLogTableName + "04";
			ConvVTableNameYahoo = strLogTableName + "06";

			// 一時テーブルを作成
			using (SqlStatement sqlStatement = new SqlStatement("TempTable", "CreateTmpTable"))
			{
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ logtablename @@", LogTableName);
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ convtablenamerakuten @@", ConvTableNameRakuten);
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ convtablenameyahoo @@", ConvTableNameYahoo);
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ convvtablenamerakuten @@", ConvVTableNameRakuten);
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ convvtablenameyahoo @@", ConvVTableNameYahoo);

				sqlStatement.ExecStatement(sqlAccessor);
			}

			//------------------------------------------------------
			// 作業用一時テーブルにログ情報を保持する
			//------------------------------------------------------
			// 作業用ログデータを作業用一時テーブルに移す
			using (SqlStatement sqlStatement = new SqlStatement("TempTable", "InsertToTmpLogTable"))
			{
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ logtablename @@", LogTableName);

				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLCOOPERATIONUPDATELOG_MASTER_KBN, strInsertMasterKbn);
				htInput.Add(Constants.FIELD_MALLCOOPERATIONUPDATELOG_ACTION_KBN, strInsertActionKbn);
				htInput.Add(Constants.FIELD_PRODUCT_USE_VARIATION_FLG, (blUseVariationFlg) ? Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE : Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_UNUSE);

				sqlStatement.ExecStatement(sqlAccessor, htInput);
			}

			// 取り込みログをアクティブ状態にする
			Active(sqlAccessor, strLogTableName);

			//------------------------------------------------------
			// 連携用一時テーブルにログ情報を保持する（作業用一時テーブルより取得する）
			//------------------------------------------------------
			// 連携用データを商品コンバータ一時テーブルに移す(Product単位)
			using (SqlStatement sqlStatement = new SqlStatement("TempTable", "InsertToTmpConvTable"))
			{
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ logtablename @@", LogTableName);
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ convtablenamerakuten @@", ConvTableNameRakuten);
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ convtablenameyahoo @@", ConvTableNameYahoo);

				sqlStatement.ExecStatement(sqlAccessor);
			}

			// 連携用データを商品コンバータ一時テーブル移す(ProductVariation単位)
			using (SqlStatement sqlStatement = new SqlStatement("TempTable", "InsertToVTmpConvTable"))
			{
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ logtablename @@", LogTableName);
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ convvtablenamerakuten @@", ConvVTableNameRakuten);
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ convvtablenameyahoo @@", ConvVTableNameYahoo);
				
				// モール連携ログのマスタ区分が在庫更新の場合、商品バリエーション連携ID3が「s」のものを除外する
				if (strInsertMasterKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCTSTOCK)
				{
					sqlStatement.Statement = sqlStatement.Statement.Replace("@@ WHERE @@", " WHERE w2_ProductVariationView.variation_cooperation_id3 <> 's' AND ##" + LogTableName + ".variation_id = w2_ProductVariationView.variation_id");
				}
				else
				{
					sqlStatement.Statement = sqlStatement.Statement.Replace("@@ WHERE @@", " WHERE 1 = 1");
				}

				sqlStatement.ExecStatement(sqlAccessor);
			}
		}

		/// <summary>
		/// 更新件数取得（更新済みであること前提）
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="strMallKbn">モール区分</param>
		/// <param name="blUseVariation">バリエーション使用有無</param>
		/// <returns>更新件数</returns>
		public int GetUpdateCount(SqlAccessor sqlAccessor, string strMallKbn, string strMallId, bool blUseVariation)
		{
			string strTableName = null;

			// モール区分とバリエーション有無を判断してテーブル名を取得する
			switch (strMallKbn)
			{
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN:
					strTableName = (blUseVariation) ? this.ConvVTableNameRakuten : this.ConvTableNameRakuten;
					break;

				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO:
					strTableName = (blUseVariation) ? this.ConvVTableNameYahoo : this.ConvTableNameYahoo;
					break;

				default:
					strTableName = "";
					break;
			}
			
			// 更新ログ件数を返却する
			using (SqlStatement sqlStatement = new SqlStatement("MallCooperationSetting", "GetCooperationUpdateLogCount"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID, strMallId);

				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ tablename @@", strTableName);

				return (int)sqlStatement.SelectSingleStatement(sqlAccessor, htInput)[0][0];
			}
		}

		/// <summary>
		/// 更新対象のログステータスをアクティブにする
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="strTableName">テーブル名</param>
		public void Active(SqlAccessor sqlAccessor, string strTableName)
		{
			using (SqlStatement sqlStatement = new SqlStatement("MallCooperationSetting", "UpdateCooperationUpdateLogActive"))
			{
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ tablename @@", strTableName);

				sqlStatement.ExecStatement(sqlAccessor);
			}
		}

		/// <summary>
		/// 更新対象のログステータスを完了にする
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="strMallKbn">モール区分</param>
		public void Complete(SqlAccessor sqlAccessor, string strMallKbn, string strMallId)
		{
			string strTableName = null;

			// モール区分を判断してテーブル名を取得する
			switch (strMallKbn)
			{
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN:
					strTableName = this.ConvTableNameRakuten;
					break;

				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO:
					strTableName = this.ConvTableNameYahoo;
					break;

				default:
					strTableName = "";
					break;
			}

			// 更新対象ログを完了状態に更新する
			using (SqlStatement sqlStatement = new SqlStatement("MallCooperationSetting", "UpdateCooperationUpdateLogComplete"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID, strMallId);

				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ tablename @@", strTableName);
				sqlStatement.ExecStatement(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// 更新対象のログステータスを未処理に戻す
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="strMallKbn">モール区分</param>
		/// <param name="strMallId">モールID</param>
		public void RollBack(SqlAccessor sqlAccessor, string strMallKbn, string strMallId)
		{
			string strTableName = null;

			// モール区分を判断してテーブル名を取得する
			switch (strMallKbn)
			{
				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN:
					strTableName = this.ConvTableNameRakuten;
					break;

				case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO:
					strTableName = this.ConvTableNameYahoo;
					break;

				default:
					strTableName = "";
					break;
			}

			// 更新対象ログを未処理状態に更新する
			using (SqlStatement sqlStatement = new SqlStatement("MallCooperationSetting", "UpdateCooperationUpdateLogRollBack"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLCOOPERATIONUPDATELOG_MALL_ID, strMallId);
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ tablename @@", strTableName);

				sqlStatement.ExecStatement(sqlAccessor, htInput);
			}
		}

		/// <summary>一時テーブル名(ログ用)</summary>
		public string LogTableName { get; private set; }

		/// <summary>楽天用一時テーブル名(商品コンバータ用)</summary>
		public string ConvTableNameRakuten { get; private set; }

		/// <summary>Yahoo!用一時テーブル名(商品コンバータ用)</summary>
		public string ConvTableNameYahoo { get; private set; }

		/// <summary>楽天用一時テーブル名(商品コンバータ用)</summary>
		public string ConvVTableNameRakuten { get; private set; }

		/// <summary>Yahoo!用一時テーブル名(商品コンバータ用)</summary>
		public string ConvVTableNameYahoo { get; private set; }
	}
}
