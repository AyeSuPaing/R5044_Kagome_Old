/*
=========================================================================================================
  Module      : 商品コンバータ文字列変換クラス(ProductConverter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;

namespace w2.Commerce.MallBatch.Mkadv.Common.Convert
{
	///**************************************************************************************
	/// <summary>
	/// 商品コンバータ文字列変換クラス（クラス名変更：AdConv.cs → ProductConverter.cs）
	/// </summary>
	///**************************************************************************************
	public class ProductConverter
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductConverter()
		{
			// プロパティ初期化
			this.ConvertSet = new Hashtable();
			this.TargetedConvertSets = new List<ConvertSet>();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="iAdtoId">商品コンバータID</param>
		/// <remarks>
		/// 文字列の単純変換規則（固定文字列の置換）をサポートする
		/// </remarks>
		/// =============================================================================================
		public ProductConverter(SqlAccessor sqlAccessor, int iAdtoId) : this()
		{
			// 商品コンバータＩＤ
			this.AdtoId = iAdtoId;

			// 商品コンバータ情報を取得する
			InitProductConverter(sqlAccessor);
		}

		/// <summary>
		/// 商品コンバータ変換データ取得
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <remarks>変換データを取得する</remarks>
		/// =============================================================================================
		private void InitProductConverter(SqlAccessor sqlAccessor)
		{
			DataView dvAdConv = null;
			using (SqlStatement sqlStatement = new SqlStatement("AdConv","GetAdConv"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLPRDCNVRULE_ADTO_ID, this.AdtoId);

				dvAdConv = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
			}

			foreach (DataRowView drvAdConv in dvAdConv)
			{
				if (drvAdConv[Constants.FIELD_MALLPRDCNVRULE_TARGET] == DBNull.Value)
				{
					// 全体設定を保持する（ハッシュテーブル格納：変換前文字をKey、変換後文字をValue）
					this.ConvertSet[(string)drvAdConv[Constants.FIELD_MALLPRDCNVRULE_CONVERTFROM]] = drvAdConv[Constants.FIELD_MALLPRDCNVRULE_CONVERTTO];
				}
				else
				{
					// 個別設定を保持する（ConvertSetクラス格納：Targetをキーに変換前文字、変換後文字を保持する）
					this.TargetedConvertSets.Add(new ConvertSet((int)drvAdConv[Constants.FIELD_MALLPRDCNVRULE_TARGET],
															 (string)drvAdConv[Constants.FIELD_MALLPRDCNVRULE_CONVERTFROM],
															 (string)drvAdConv[Constants.FIELD_MALLPRDCNVRULE_CONVERTTO]));
				}
			}
		}

		/// <summary>
		/// 文字列変換（変換前文字列→変換後文字列）
		/// </summary>
		/// <param name="iTarget">対象出力フィールドID</param>
		/// <param name="strFrom">変換前文字列</param>
		/// <returns>文字列の変換規則に基づき、変換する</returns>
		public string Convert(int iTarget, string strFrom)
		{
			string strRet = strFrom;

			// 個別変換（ConvertSetクラスの保持情報から変換する）
			foreach (ConvertSet convertSet in this.TargetedConvertSets)
			{
				if (convertSet.Target == iTarget)
				{
					strRet = strRet.Replace(convertSet.From, convertSet.To);
				}
			}
			// 全体変換（ハッシュテーブルの保持情報から変換する）
			foreach (string strKey in this.ConvertSet.Keys)
			{
				if (strKey != "")
				{
					strRet = strRet.Replace(strKey, (string)this.ConvertSet[strKey]);
				}
			}

			return strRet;
		}

		/// <summary>商品コンバータID</summary>
		private int AdtoId { get; set; }

		/// <summary>変換テーブル（全体）</summary>
		private Hashtable ConvertSet { get; set; }

		/// <summary>変換テーブル（個別）</summary>
		private List<ConvertSet> TargetedConvertSets { get; set; }
	}
}
