/*
=========================================================================================================
  Module      : モール設定要素 (MallConfigTip.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using w2.Common.Sql;
using w2.Common.Logger;
using w2.SFTPClientWrapper;

namespace w2.Commerce.MallBatch.StockUpdate.Mall
{
	///**************************************************************************************
	/// <summary>
	/// モール設定の要素
	/// </summary>
	///**************************************************************************************
	public class MallConfigTip
	{
		/// <summary>
		/// 実行用の商品コンバータのパラメータを取得する
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="strMasterKbn">マスター区分</param>
		/// <param name="strActionKbn">アクション区分</param>
		/// <param name="blVariationFlg">バリエーション使用有無</param>
		public List<Hashtable> GetConvertParams(
			SqlAccessor sqlAccessor, 
			string strMasterKbn, 
			string strActionKbn, 
			bool blVariationFlg)
		{
			List<Hashtable> lConvertParams = new List<Hashtable>();
			bool blUseVariation1 = false;
			bool blUseVariation2 = false;

			//------------------------------------------------------
			// 実行用商品コンバータIDを初期化する
			//------------------------------------------------------
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MALLPRDCNV_ADTO_ID, "");
			htInput.Add(Constants.FIELD_MALLPRDCNV_ADTO_ID + "_2", "");
			htInput.Add(Constants.FIELD_MALLPRDCNV_ADTO_ID + "_3", "");

			//------------------------------------------------------
			// 「楽天」の商品コンバータIDを取得する
			//------------------------------------------------------
			if (this.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN)
			{
				// 商品バリエーションなし
				if (blVariationFlg == false)
				{
					// 商品情報
					if (strMasterKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCT)
					{
						// 登録時
						if (strActionKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_INSERT)
						{
							htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID] = this.CnvidRtnNInsItemcsv;
							htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID + "_3"] = this.CnvidRtnItemcatcsv; // カテゴリ登録
						}
						// 更新時
						else if (strActionKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_UPDATE)
						{
							htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID] = this.CnvidRtnNUpdItemcsv;
						}
					}
					// 在庫情報
					else if (strMasterKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCTSTOCK)
					{
						htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID] = this.CnvidRtnNStkItemcsv;
					}
				}
				// 商品バリエーションあり
				else
				{
					// 商品情報
					if (strMasterKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCT)
					{
						// 登録時
						if (strActionKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_INSERT)
						{
							htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID] = this.CnvidRtnVInsItemcsv;
							htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID + "_2"] = this.CnvidRtnVInsSelectcsv;
							blUseVariation2 = true;
							htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID + "_3"] = this.CnvidRtnItemcatcsv; // カテゴリ登録
						}
						// 更新時
						else if (strActionKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_UPDATE)
						{
							htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID] = this.CnvidRtnVUpdItemcsv;
						}
					}
					// 在庫情報
					else if (strMasterKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCTSTOCK)
					{
						htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID] = this.CnvidRtnVStkSelectcsv;
						blUseVariation1 = true;
					}
				}
			}
			//------------------------------------------------------
			// 「ヤフー」の商品コンバータIDを取得する
			//------------------------------------------------------
			else if (this.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO)
			{
				// 商品バリエーションなし
				if (blVariationFlg == false)
				{
					// 商品情報
					if (strMasterKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCT)
					{
						// 登録時
						if (strActionKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_INSERT)
						{
							htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID] = this.CnvidYhoNInsDatacsv;
							htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID + "_2"] = this.CnvidYhoNInsStockcsv;
						}
						// 更新時
						else if (strActionKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_UPDATE)
						{
							htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID] = this.CnvidYhoNUpdDatacsv;
						}
					}
					// 在庫情報
					else if (strMasterKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCTSTOCK)
					{
						htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID] = this.CnvidYhoNStkDatacsv;
					}
				}
				// 商品バリエーションあり
				else
				{
					// 商品情報
					if (strMasterKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCT)
					{
						// 登録時
						if (strActionKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_INSERT)
						{
							htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID] = this.CnvidYhoVInsDatacsv;
							htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID + "_2"] = this.CnvidYhoVInsStockcsv;
							blUseVariation2 = true;
						}
						// 更新時
						else if (strActionKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_UPDATE)
						{
							htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID] = this.CnvidYhoVUpdDatacsv;
						}
					}
					// 在庫情報
					else if (strMasterKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCTSTOCK)
					{
						htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID] = this.CnvidYhoVStkDatacsv;
						blUseVariation1 = true;
					}
				}
			}

			//------------------------------------------------------
			// 実行用商品コンバータ設定情報取得
			//------------------------------------------------------
			DataView dvAdtoParams = new DataView();
			using (SqlStatement sqlStatement = new SqlStatement("Adto", "GetAdto"))
			{
				dvAdtoParams = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
			}

			foreach (DataRowView drvAdtoParams in dvAdtoParams)
			{
				Hashtable htAdtoParams = new Hashtable();
				htAdtoParams.Add(Constants.FIELD_MALLPRDCNV_ADTO_ID, drvAdtoParams[Constants.FIELD_MALLPRDCNV_ADTO_ID].ToString());
				htAdtoParams.Add(Constants.FIELD_MALLPRDCNV_FILENAME, CheckFileName((string)drvAdtoParams[Constants.FIELD_MALLPRDCNV_FILENAME]));
				htAdtoParams.Add(Constants.FIELD_MALLPRDCNV_PATH, (string)drvAdtoParams[Constants.FIELD_MALLPRDCNV_PATH]);
				htAdtoParams.Add(Constants.USE_VARIATION, false);

				// 在庫更新時、バリエーションあり
				if (blUseVariation1)
				{
					if ((string)htAdtoParams[Constants.FIELD_MALLPRDCNV_ADTO_ID] == (string)htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID])
					{
						htAdtoParams[Constants.USE_VARIATION] = true;
					}
				}

				// 商品登録時、バリエーションあり
				if (blUseVariation2)
				{
					if ((string)htAdtoParams[Constants.FIELD_MALLPRDCNV_ADTO_ID] == (string)htInput[Constants.FIELD_MALLPRDCNV_ADTO_ID + "_2"])
					{
						htAdtoParams[Constants.USE_VARIATION] = true;
					}
				}

				if (Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION
					&& (this.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN)
					&& blVariationFlg)
				{
					htAdtoParams[Constants.USE_VARIATION] = true;
				}

				lConvertParams.Add(htAdtoParams);
			}

			return lConvertParams;
		}

		/// <summary>
		/// ファイル名変更要否取得
		/// </summary>
		/// <param name="strFileName">ファイル名</param>
		/// <returns>ファイル名</returns>
		private string CheckFileName(string strFileName)
		{
			if (Regex.Match(strFileName, @"\[.*\]").Success)
			{
				string [] strDates = strFileName.Split("[]".ToCharArray());
				DateTime dateTime = DateTime.Now.AddMinutes(Constants.YAHOO_ADD_MINUTE);

				try
				{
					return strDates[0] + dateTime.ToString(strDates[1]) + strDates[2];
				}
				catch (Exception)
				{
					FileLogger.WriteError("ファイル名のフォーマットが正しくありません。");
					throw;
				}
			}
			return strFileName;
		}

		/// <summary>店舗ＩＤ</summary>
		public string ShopId { get; set; }
		/// <summary>モールＩＤ</summary>
		public string MallId { get; set; }
		/// <summary>モール名</summary>
		public string MallName { get; set; }
		/// <summary>モール区分</summary>
		public string MallKbn { get; set; }
		/// <summary>FTPホスト</summary>
		public string FtpHost { get; set; }
		/// <summary>FTPユーザＩＤ</summary>
		public string FtpUserName { get; set; }
		/// <summary>FTPパスワード</summary>
		public string FtpPassWord { get; set; }
		/// <summary>FTPアップロード先ディレクトリ</summary>
		public string FtpUploadDir { get; set; }
		/// <summary>商品コンバータ：楽天（バリエーションなし・商品登録）</summary>
		public string CnvidRtnNInsItemcsv { get; set; }
		/// <summary>商品コンバータ：楽天（バリエーションなし・商品更新）</summary>
		public string CnvidRtnNUpdItemcsv { get; set; }
		/// <summary>商品コンバータ：楽天（バリエーションなし・在庫更新）</summary>
		public string CnvidRtnNStkItemcsv { get; set; }
		/// <summary>商品コンバータ：楽天（バリエーションあり・商品登録）</summary>
		public string CnvidRtnVInsItemcsv { get; set; }
		/// <summary>商品コンバータ：楽天（バリエーションあり・商品登録（バリエーション・在庫））</summary>
		public string CnvidRtnVInsSelectcsv { get; set; }
		/// <summary>商品コンバータ：楽天（バリエーションあり・商品更新）</summary>
		public string CnvidRtnVUpdItemcsv { get; set; }
		/// <summary>商品コンバータ：楽天（バリエーションあり・在庫更新（バリエーション・在庫））</summary>
		public string CnvidRtnVStkSelectcsv { get; set; }
		/// <summary>商品コンバータ：楽天カテゴリ</summary>
		public string CnvidRtnItemcatcsv { get; set; }
		/// <summary>商品コンバータ：楽天SKU管理IDフォーマット（バリエーションなし）</summary>
		public string RakutenSkuManagementIdOutputFormatForNormal { get; set; }
		/// <summary>商品コンバータ：楽天SKU管理IDフォーマット（バリエーションあり）</summary>
		public string RakutenSkuManagementIdOutputFormatForVariation { get; set; }
		/// <summary>商品コンバータ：Yahoo!（商品登録）</summary>
		public string CnvidYhoNInsDatacsv { get; set; }
		/// <summary>商品コンバータ：Yahoo!（商品登録・在庫）</summary>
		public string CnvidYhoNInsStockcsv { get; set; }
		/// <summary>商品コンバータ：Yahoo!（商品更新）</summary>
		public string CnvidYhoNUpdDatacsv { get; set; }
		/// <summary>商品コンバータ：Yahoo!（在庫更新）</summary>
		public string CnvidYhoNStkDatacsv { get; set; }
		/// <summary>商品コンバータ：Yahoo!（バリエーションあり・商品登録）</summary>
		public string CnvidYhoVInsDatacsv { get; set; }
		/// <summary>商品コンバータ：Yahoo!（バリエーションあり・商品登録（バリエーション・在庫））</summary>
		public string CnvidYhoVInsStockcsv { get; set; }
		/// <summary>商品コンバータ：Yahoo!（バリエーションあり・商品更新）</summary>
		public string CnvidYhoVUpdDatacsv { get; set; }
		/// <summary>商品コンバータ：Yahoo!（バリエーションあり・在庫更新（バリエーション・在庫））</summary>
		public string CnvidYhoVStkDatacsv { get; set; }
		/// <summary>メンテナンス開始日</summary>
		public DateTime? MaintenanceDateFrom { get; set; }
		/// <summary>メンテナンス終了日</summary>
		public DateTime? MaintenanceDateTo { get; set; }
		/// <summary>FTPルートパス</summary>
		public string PathFtpUpload { get; set; }
		/// <summary>SFTPの設定</summary>
		public SFTPSettings SFTPSettings { get; set; }
		/// <summary>SFTPを使うかどうか</summary>
		public bool IsSftp { get; set; }
	}
}
