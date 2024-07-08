using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using w2.Common.Sql;
using w2.Common.Util;
using w2.App.Common.Extensions.Currency;

namespace w2.App.Common.DataExporters
{
	/// <summary>
	/// 出力設定XMLに従って、行を整形するためのクラス
	/// ※ 大塚の変換バッチとはいろいろ異なっているので注意
	/// 
	/// 
	/// </summary>
	public class ExportDataFormatter
	{
		protected XmlNode ConversionTable;
		protected List<ConvertSettingField> ConvertSettings;
		protected Hashtable EnvVariables;
		protected IDataRecord CurrentRecord;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settingXML"></param>
		public ExportDataFormatter(string settingXML)
		{
			XmlDocument setting = new XmlDocument();
			setting.LoadXml(settingXML);

			// 出力設定
			ConversionTable = setting.SelectSingleNode("*/conversion_table");
			ConvertSettings = LoadConvertSettings(setting.SelectSingleNode("*/export_format"));

			// 環境変数
			EnvVariables = new Hashtable();
			InitEnvVariables(EnvVariables);
		}

		// 環境変数初期化
		protected virtual void InitEnvVariables(Hashtable EnvVariables)
		{
			EnvVariables["DATE"] = DateTime.Now;
			EnvVariables["VALUE"] = new object();
		}

		/// <summary>
		/// XMLノードから、変換設定読み込み
		/// </summary>
		/// <param name="xnSetting"></param>
		/// <returns></returns>
		public List<ConvertSettingField> LoadConvertSettings(XmlNode xnSetting)
		{
			const string CONST_STR_FIELD_NAME = "name";
			const string CONST_STR_FIELD_DEFAULT = "default";
			const string CONST_STR_FIELD_DATA_TYPE = "data_type";
			const string CONST_STR_FIELD_DATA_SIZE = "data_size";
			const string CONST_STR_FIELD_CONVERSION_TABLE = "conversion_table";

			List<ConvertSettingField> convertSettings = new List<ConvertSettingField>();

			XmlNodeList xnlItems = xnSetting.SelectNodes("field");
			foreach (XmlNode xnItem in xnlItems)
			{
				ConvertSettingField convertSetting = new ConvertSettingField(
					xnItem.SelectSingleNode(CONST_STR_FIELD_NAME).InnerText,
					xnItem.SelectSingleNode(CONST_STR_FIELD_DATA_TYPE).InnerText,
					Convert.ToInt32(xnItem.SelectSingleNode(CONST_STR_FIELD_DATA_SIZE).InnerText),
					xnItem.SelectSingleNode(CONST_STR_FIELD_DEFAULT).InnerText,
					xnItem.SelectSingleNode(CONST_STR_FIELD_CONVERSION_TABLE).InnerText);

				convertSettings.Add(convertSetting);
			}

			return convertSettings;
		}

		/// <summary>
		/// SQLからきたデータを変換、整形する
		/// </summary>
		/// <param name="dataReader"></param>
		/// <returns></returns>
		public List<string> FormatExportRowData(SqlStatementDataReader dataReader)
		{
			CurrentRecord = dataReader.SqlDataReader;
			return FormatExportRowData();
		}

		/// <summary>
		/// SQLからきたデータを変換、整形する
		/// </summary>
		/// <param name="dataReader"></param>
		/// <returns></returns>
		public List<string> FormatExportRowData()
		{
			List<String> results = new List<String>();

			foreach (ConvertSettingField convertSetting in this.ConvertSettings)
			{
				try
				{
					// 変換対象を取得
					string value = CurrentRecord[convertSetting.FieldName].ToString();

					// 環境変数を更新
					EnvVariables["VALUE"] = value;

					// 空文字列もしくはNULLであれば、default値を評価してセット
					if (string.IsNullOrEmpty(value) || CurrentRecord[convertSetting.FieldName] == DBNull.Value)
					{
						value = Evaluate(convertSetting.Default);
					}

					// 変換テーブル
					if (string.IsNullOrEmpty(convertSetting.ConversionTable) == false)
					{
						value = Evaluate(ConvertValueText(convertSetting, value));
					}

					// 型変換
					if (string.IsNullOrEmpty(value) == false && string.IsNullOrEmpty(convertSetting.DataType) == false)
					{
						value = FormatData(convertSetting, value);
					}

					// サイズがあわない
					if (convertSetting.DataSize > 0 && value.Length > convertSetting.DataSize)
					{
						throw new FormatException("サイズが大きすぎます: " + convertSetting.FieldName + ": " + value);
					}

					results.Add(value);
				}
				catch (IndexOutOfRangeException ex)
				{
					results.Add("ERROR! " + ex.Message + "が検索結果に見つかりませんでした");
				}
				catch (Exception ex)
				{
					results.Add("ERROR! " + ex.Message);
				}
				
			}

			return results;
		}

		/// <summary>
		/// 変換テーブル適用
		/// </summary>
		/// <param name="convertSetting"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private string ConvertValueText(ConvertSettingField convertSetting, string value)
		{
			XmlNode replaceNode = ConversionTable.SelectSingleNode(convertSetting.ConversionTable + "/Value[@value='" + value + "']");
			if (replaceNode != null)
			{
				value = replaceNode.Attributes["text"].Value;
			}
			else
			{
				// デフォルト値の取得を試み、なければ例外スロー
				XmlNode defaultNode = ConversionTable.SelectSingleNode(convertSetting.ConversionTable + "/Value[@default='true']");
				if (defaultNode != null)
				{
					value = defaultNode.Attributes["text"].Value;
				}
				else
				{
					throw new Exception(new StringBuilder("変換テーブルが見つかりませんでした。ノード「*/conversion_table/").Append(convertSetting.ConversionTable).Append("」、value：「").Append(value).Append("」が必要です。").ToString());
				}
			}
			return value;
		}

		/// <summary>
		/// 式の評価をおこなう
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected string Evaluate(string value)
		{
			string result = string.Copy(value);

			// 評価対象「${～}」を探索し、それぞれ評価して置換
			var matches = Regex.Matches(result, @"\$\{([\d\w]+)\}");
			foreach(Match match in matches)
			{
				result = result.Replace(match.Groups[0].Value, EvaluateInner(match.Groups[1].Value));
			}

			return result;
		}

		/// <summary>
		/// Checking Format
		/// </summary>
		/// <param name="index">Index</param>
		/// <param name="value">Value</param>
		/// <returns>Checking format</returns>
		public string CheckingFormat(int index, string value)
		{
			var convertSetting = this.ConvertSettings[index];

			try
			{
				// 型変換
				if ((string.IsNullOrEmpty(value) == false) && (string.IsNullOrEmpty(convertSetting.DataType) == false))
				{
					value = FormatData(convertSetting, value);
				}

				// サイズがあわない
				if ((value != null) && (value.Length > convertSetting.DataSize))
				{
					throw new FormatException("サイズが大きすぎます: " + convertSetting.FieldName + ": " + value);
				}
			}
			catch (Exception ex)
			{
				return "ERROR! " + ex.Message;
			}

			return value;
		}

		/// <summary>
		/// 式の評価を行う
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual string EvaluateInner(string value)
		{
			if (EnvVariables.ContainsKey(value))
			{
				return EnvVariables[value].ToString();
			}
			else
			{
				return CurrentRecord[value].ToString();
			}
			// 該当なしのため例外をスロー
			throw new FormatException("式評価にて失敗: " + value);
		}

		/// <summary>
		/// データを整形
		/// </summary>
		/// <param name="convertSetting"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual string FormatData(ConvertSettingField convertSetting, string value)
		{
			switch (convertSetting.DataType)
			{
				case "numeric":
					decimal number = 0;
					if (decimal.TryParse(value, out number))
					{
						value = number.ToPriceString();
					}
					// パースに失敗
					else
					{
						throw new FormatException("型変換(numeric)にてパース失敗: " + convertSetting.FieldName + ": " + value);
					}

					break;

				case "char":	// データサイズと合致するよう半角スペースで補完
					value = value.PadRight(convertSetting.DataSize, ' ');
					break;

				case "varchar":
					// 何もしない
					break;

				case "date":
					DateTime dateForParse;
					if (DateTime.TryParse(value, out dateForParse))
					{
						value = dateForParse.ToString("yyyy-MM-dd");
					}
					// パースに失敗
					else
					{
						throw new FormatException("型変換(date)にてパース失敗: " + convertSetting.FieldName + ": " + value);
					}
					break;

				case "datetime":
					DateTime datetimeForParse;
					if (DateTime.TryParse(value, out datetimeForParse))
					{
						value = datetimeForParse.ToString("yyyy-MM-dd HH:mm:ss");
					}
					// パースに失敗
					else
					{
						throw new FormatException("型変換(datetime)にてパース失敗: " + convertSetting.FieldName + ": " + value);
					}
					break;
			}

			return value;
		}
	}

	/// <summary>
	/// 変換設定を格納するクラス
	/// </summary>
	public class ConvertSettingField
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		/// <param name="strFieldName"></param>
		/// <param name="strDataType"></param>
		/// <param name="iDataSize"></param>
		/// <param name="strDefault"></param>
		/// <param name="strConversionTable"></param>
		public ConvertSettingField(string strFieldName, string strDataType, int iDataSize, string strDefault, string strConversionTable)
		{
			this.FieldName = strFieldName;
			this.DataType = strDataType;
			this.DataSize = iDataSize;
			this.Default = strDefault;
			this.ConversionTable = strConversionTable;
		}

		/// <summary> フィールド名 </summary>
		public string FieldName { get; private set; }
		
		/// <summary> フィールド型 </summary>
		public string DataType { get; private set; }
		
		/// <summary> フィールドサイズ </summary>
		public int DataSize { get; private set; }
		
		/// <summary> デフォルト値 </summary>
		public string Default { get; private set; }
		
		/// <summary> 変換テーブル </summary>
		public string ConversionTable { get; private set; }
	}
}
