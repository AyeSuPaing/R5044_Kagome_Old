/*
=========================================================================================================
  Module      : 名称翻訳設定CSV出力フォーマッタクラス (NameTranslationCsvOutputFormatter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.App.Common.Util.Attribute;
using w2.Common.Util;
using w2.Domain.NameTranslationSetting;
namespace w2.App.Common.Global.Translation
{
	/// <summary>
	/// 名称翻訳設定CSV出力フォーマッタクラス
	/// </summary>
	public class NameTranslationCsvOutputFormatter
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public NameTranslationCsvOutputFormatter() {}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public NameTranslationCsvOutputFormatter(NameTranslationSettingModel model)
		{
			this.CsvData = model;
		}
		#endregion

		#region +OutputCsvHeader CSVヘッダ出力
		/// <summary>
		/// CSVヘッダ出力
		/// </summary>
		/// <returns>CSVヘッダ行</returns>
		public string OutputCsvHeader()
		{
			var headers = this.GetType().GetProperties()
				.Where(property => System.Attribute.GetCustomAttribute(property, typeof(CsvMapName)) != null)
				.OrderBy(property => ((CsvMapName)System.Attribute.GetCustomAttribute(property, typeof(CsvMapName))).ColumnIndex)
				.Select(property => string.Format(@"""{0}""", ((CsvMapName)System.Attribute.GetCustomAttribute(property, typeof(CsvMapName))).MapName));
			return string.Join(",", headers.ToArray());
		}
		#endregion

		#region +FormatCsvLine CSV形式出力
		/// <summary>
		/// CSV形式出力
		/// </summary>
		/// <returns>各プロパティの改行を削除したものをカンマ区切りで出力</returns>
		public string FormatCsvLine()
		{
			var records = this.GetType().GetProperties()
				.Where(property => System.Attribute.GetCustomAttribute(property, typeof(CsvMapName)) != null)
				.OrderBy(property => ((CsvMapName)System.Attribute.GetCustomAttribute(property, typeof(CsvMapName))).ColumnIndex)
				.Select(property => string.Format(@"""{0}""", StringUtility.EscapeCsvColumn((string)property.GetValue(this, null))));
			return string.Join(",", records.ToArray());
		}
		#endregion

		#region プロパティ
		/// <summary>対象データ区分</summary>
		[CsvMapName(1, "data_kbn")]
		public string DataKbn { get { return this.CsvData.DataKbn; } }
		/// <summary>翻訳対象項目</summary>
		[CsvMapName(2, "translation_target_column")]
		public string TranslationTargetColumn { get { return this.CsvData.TranslationTargetColumn; } }
		/// <summary>マスタID1</summary>
		[CsvMapName(3, "master_id1")]
		public string MasterId1 { get { return this.CsvData.MasterId1; } }
		/// <summary>マスタID2</summary>
		[CsvMapName(4, "master_id2")]
		public string MasterId2 { get { return this.CsvData.MasterId2; } }
		/// <summary>マスタID3</summary>
		[CsvMapName(5, "master_id3")]
		public string MasterId3 { get { return this.CsvData.MasterId3; } }
		/// <summary>言語コード</summary>
		[CsvMapName(6, "language_code")]
		public string LanguageCode { get { return this.CsvData.LanguageCode; } }
		/// <summary>言語ロケールID</summary>
		[CsvMapName(7, "language_locale_id")]
		public string LanguageLocalId { get { return this.CsvData.LanguageLocaleId; } }
		/// <summary>翻訳前名称</summary>
		[CsvMapName(8, "before_translational_name")]
		public string BeforeTranslationalName { get { return this.CsvData.BeforeTranslationalName; } }
		/// <summary>翻訳後名称</summary>
		[CsvMapName(9, "after_translational_name")]
		public string AfterTranslationalName { get { return this.CsvData.AfterTranslationalName; } }
		/// <summary>表示HTML区分</summary>
		[CsvMapName(10, "display_kbn")]
		public string DisplayKbn { get { return this.CsvData.DisplayKbn; } }

		/// <summary>名称翻訳設定CSVデータ</summary>
		public NameTranslationSettingModel CsvData { get; set; }
		#endregion

	}
}
