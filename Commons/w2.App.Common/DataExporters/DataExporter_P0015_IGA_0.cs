/*
=========================================================================================================
  Module      : 0015_IGA基幹システム連携用データ設定クラス(DataExporter_P0015_IGA_0.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Application : w2.App.Common
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Resources;
using System.Text;
using System.Xml;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.App.Common.DataExporters
{
	/// <summary>
	/// P0015_IGA基幹システム連携用データ設定クラス
	/// </summary>
	public partial class DataExporter_P0015_IGA_0 : DataExporterBase
	{
		private ExportDataFormatter formatter;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DataExporter_P0015_IGA_0(SqlAccessor sqlAccessor)
			: base(sqlAccessor,
				"okurijyo.csv",
				",",
				"",
				Encoding.GetEncoding("Shift-JIS"),
				Constants.ExportKbn.OrderList,
				"*/export/statement",
				ReadSettingXML(),
				"ecat用csv出力")
		{
			formatter = new ExportDataFormatter_P0015_IGA_0(ReadSettingXML());
		}

		/// <summary>
		/// 設定XMLを取得	※webキャッシュを利用
		/// </summary>
		/// <returns></returns>
		private static string ReadSettingXML()
		{
			const string CACHE_KEY_SETTING = "setting";

			string fileName = Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + @"DataExport/P0015_IGA_0.xml";

			System.Web.Caching.Cache cache = System.Web.HttpContext.Current.Cache;
			
			//if (cache[CACHE_KEY_SETTING] == null)
			{
				using (System.IO.StreamReader reader = System.IO.File.OpenText(fileName))
				cache.Add(CACHE_KEY_SETTING,
							reader.ReadToEnd(),
							new System.Web.Caching.CacheDependency(fileName),
							System.Web.Caching.Cache.NoAbsoluteExpiration,
							new TimeSpan(6, 0, 0),
							System.Web.Caching.CacheItemPriority.Normal,
							null);
			}

			return (string)cache[CACHE_KEY_SETTING];
		}

		/// <summary>
		/// 基幹システム連携用単位出力データ取得
		/// </summary>
		/// <param name="ssdrDataReader">SQLデータリーダー</param>
		/// <returns>単位出力データ</returns>
		public override string GetExportUnitData(SqlStatementDataReader ssdrDataReader)
		{
			var rowData = formatter.FormatExportRowData(ssdrDataReader);
			return CreateRowData(rowData);
		}
	}

	/// <summary>
	/// IGA用出力型定義
	/// </summary>
	public class ExportDataFormatter_P0015_IGA_0 : ExportDataFormatter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settingXML"></param>
		public ExportDataFormatter_P0015_IGA_0(string settingXML) : base(settingXML) { }

		/// <summary>
		/// IGA用出力型定義
		/// </summary>
		/// <param name="convertSetting"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected override string FormatData(ConvertSettingField convertSetting, string value)
		{
			switch (convertSetting.DataType)
			{
				// データサイズにあわせて文字列を切り詰める
				case "RigidChar" :
					value = value.Substring(0, convertSetting.DataSize);
					break;

				case "time" :
					DateTime timeForParse;
					if (DateTime.TryParse(value, out timeForParse))
					{
						value = timeForParse.ToString("HHmm");
					}
					// パースに失敗
					else
					{
						throw new FormatException("型変換(time)にてパース失敗: " + convertSetting.FieldName + ": " + value);
					}
					break;

				case "date_iga":
					DateTime dateForParse;
					if (DateTime.TryParse(value, out dateForParse))
					{
						value = dateForParse.ToString("yyyyMMdd");
					}
					// パースに失敗
					else
					{
						throw new FormatException("型変換(date)にてパース失敗: " + convertSetting.FieldName + ": " + value);
					}
					break;

				case "crlfcnv":
					value = value.Replace("[特記事項（任意）]", "").Replace("\r\n", " ");
					break;

				case "num02cnv":
					value = string.Format("{0:D2}", int.Parse(value));
					break;

				case "minuscnv":
					value = "-" + value;
					break;
			}

			// 基本の型定義も利用する
			return base.FormatData(convertSetting, value);
		}
	}
}