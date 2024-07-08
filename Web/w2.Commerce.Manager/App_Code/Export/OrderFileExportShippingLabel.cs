/*
=========================================================================================================
  Module      : 送り状発行データ出力モジュール (OrderFileExportShippingLabel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.Common.Logger;
using w2.Domain.DeliveryCompany;
using w2.Domain.Order;
using w2.Domain.Product;

/// <summary>
/// 送り状データ出力モジュール
/// </summary>
public class OrderFileExportShippingLabel
{
	/// <summary>送り状出力設定ファイルの格納フォルダ</summary>
	public const string DIRECTORY_NAME_SHIPPING_LABEL_SETTING = "ShippingLabelSetting";
	/// <summary>現在日時タグの名前</summary>
	public const string CURRENT_DATE_TIME_TAG_NAME = "Now";
	/// <summary>Display name file DSK振り込み用紙用CSV.xml</summary>
	public const string DSK_DEF_DISPLAY_NAME = "DSK振り込み用紙用CSV";
	/// <summary>Display name file 後払い.com.xml</summary>
	public const string ATOBARAICOM_DEF_DISPLAY_NAME = "後払い.com";
	/// <summary>Display name file ベリトランス請求書.xml</summary>
	private const string VERITRANS_DEF_DISPLAY_NAME = "ベリトランス請求書";

	/// <summary>
	/// 送り状設定一覧の取得
	/// </summary>
	/// <returns>送り状設定一覧</returns>
	public List<ShippingLabelExportSetting> GetShippingLabelExportSettingList()
	{
		// 送り状出力設定フォルダから設定一覧を取得
		var path = string.Format("{0}{1}", Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE, DIRECTORY_NAME_SHIPPING_LABEL_SETTING);
		if (Directory.Exists(path) == false) return null;

		var tagrgetFilePaths = Array.FindAll(Directory.GetFiles(path), item => item.ToLower().EndsWith(".xml"));
		var settings = tagrgetFilePaths.Select(ParseSettingFile).Where(s => s != null).ToList();
		return settings;
	}

	/// <summary>
	/// 送り状出力設定ファイルから設定内容読込
	/// </summary>
	/// <param name="filePath">送り状出力設定ファイルのパス</param>
	/// <returns>送り状出力設定</returns>
	private ShippingLabelExportSetting ParseSettingFile(string filePath)
	{
		try
		{
			// XX.○○.xmlファイルはサンプルとして用意しますので、スキップする
			if (Path.GetFileName(filePath).StartsWith(@"XX.")) return null;

			var xml = GetXDocument(filePath);
			var rootName = xml.Root.Name.LocalName;
			CheckValidInput(
				rootName,
				new[] { "OrderDataExportSetting" },
				errorMessage: string.Format("設定ファイルのルート要素は「OrderDataExportSetting」以外の値({0})が対応しません。", rootName));
			var fileSetting = TryGetRequiredElement(xml.Root, "ExportSetting");
			var fieldsSetting = TryGetRequiredElement(xml.Root, "FieldsSetting");
			var contentsSetting = xml.Root.Element("ContentsSetting");

			var setting = new ShippingLabelExportSetting();
			setting.DisplayName = TryGetRequiredElementValue(fileSetting, "DisplayName");
			var formatType = TryGetRequiredElement(fileSetting, "FormatType");
			CheckValidInput(formatType.Value, new[] { "csv", "Excel" }, formatType.Name.LocalName);
			setting.FormatType = formatType.Value;
			var unitType = TryGetRequiredElement(fileSetting, "UnitType");
			CheckValidInput(unitType.Value, new[] { "Order", "AtodeneInvoice", "DskDeferredInvoice", "ScoreInvoice", "VeritransInvoice", "SQL", "OrderItem" }, unitType.Name.LocalName);
			setting.UnitType = unitType.Value;
			setting.FileName = CreateFileName(TryGetRequiredElement(fileSetting, "FileName"));
			setting.ExportHeader = bool.Parse(TryGetRequiredElementValue(fileSetting, "ExportHeader"));
			setting.AlwaysExportQuotation = bool.Parse(TryGetRequiredElementValue(fileSetting, "AlwaysExportQuotation"));
			setting.StringQuotationMark = char.Parse(TryGetRequiredElementValue(fileSetting, "StringQuotationMark"));
			var separator = TryGetRequiredElement(fileSetting, "Separator");
			CheckValidInput(
				separator.Value,
				new[] { ShippingLabelExportSetting.COMMA_STRING, ShippingLabelExportSetting.TAB_STRING },
				separator.Name.LocalName);
			setting.Separator = (ShippingLabelExportSetting.COMMA_STRING == separator.Value) ? ShippingLabelExportSetting.COMMA : ShippingLabelExportSetting.TAB;
			var encoding = TryGetRequiredElement(fileSetting, "Encoding");
			CheckValidInput(encoding.Value, new[] { "Shift_JIS", "UTF-8", "Big5" }, encoding.Name.LocalName);
			setting.FileEncoding = Encoding.GetEncoding(encoding.Value);
			setting.SelectSql = CreateSelectSqlFromSetting(fieldsSetting, setting);
			if (contentsSetting != null)
			{
				setting.ContentsSettingEMS = GetContentSetting(contentsSetting);
			}

			var sqlSetting = xml.Root.Element("SqlSetting");
			setting.SqlExport = (sqlSetting != null) ? TryGetRequiredElementValue(sqlSetting, "SqlExport") : string.Empty;
			return setting;
		}
		catch (Exception ex)
		{
			FileLogger.WriteError(string.Format("設定ファイル({0})が不正です。", filePath), ex);
			return null;
		}
	}

	/// <summary>
	/// 送り状出力設定ファイルの取得
	/// </summary>
	/// <param name="filePath">送り状出力設定ファイルのパス</param>
	/// <returns>送り状Xドキュメント</returns>
	private XDocument GetXDocument(string filePath)
	{
		var tempXml = XDocument.Load(filePath);
		var tempString = new StringBuilder(tempXml.ToString());
		tempString.Replace("@@ KEY_CURRENCY_DIGITS @@", CurrencyManager.DigitsByKeyCurrency.ToString());
		var xml = XDocument.Load(new StringReader(tempString.ToString()));
		return xml;
	}

	/// <summary>
	/// 親要素から必須の子要素を取得
	/// </summary>
	/// <param name="parentElement">親要素</param>
	/// <param name="elementName">必須の子要素の名前</param>
	/// <returns>必須の子要素</returns>
	private XElement TryGetRequiredElement(XElement parentElement, string elementName)
	{
		var element = parentElement.Element(elementName);
		if (element != null) return element;
		throw new Exception(string.Format("{0}タグの直下に必須の{1}タグがありません。", parentElement.Name.LocalName, elementName));
	}

	/// <summary>
	/// 親要素から必須の子要素の値を取得
	/// </summary>
	/// <param name="parentElement">親要素</param>
	/// <param name="elementName">必須の子要素の名前</param>
	/// <returns>必須の子要素の値</returns>
	private string TryGetRequiredElementValue(XElement parentElement, string elementName)
	{
		var element = parentElement.Element(elementName);
		if (element != null) return element.Value;
		throw new Exception(string.Format("{0}タグの直下に必須の{1}タグがありません。", parentElement.Name.LocalName, elementName));
	}

	/// <summary>
	/// 親要素から必須の子要素の一覧を取得
	/// </summary>
	/// <param name="parentElement">親要素</param>
	/// <param name="elementName">必須の子要素の名前</param>
	/// <returns>必須の子要素一覧</returns>
	private IEnumerable<XElement> TryGetRequiredElementList(XElement parentElement, string elementName)
	{
		var elements = parentElement.Elements(elementName);
		if (elements.Any()) return elements;
		throw new Exception(string.Format("{0}タグの直下に必須の{1}タグがありません。", parentElement.Name.LocalName, elementName));
	}

	/// <summary>
	/// 指定要素から必須の属性の値を取得
	/// </summary>
	/// <param name="element">指定要素</param>
	/// <param name="attributeName">必須属性の名前</param>
	/// <returns>必須属性の値</returns>
	private string TryGetRequiredAttributeValue(XElement element, string attributeName)
	{
		var attribute = element.Attribute(attributeName);
		if (attribute != null) return attribute.Value;
		throw new Exception(string.Format("{0}タグに必須の{1}属性がありません。", element.Name.LocalName, attributeName));
	}

	/// <summary>
	/// 設定値は設定可能一覧に属するかどうかチェック
	/// </summary>
	/// <param name="value">設定値</param>
	/// <param name="validInputs">設定可能値の一覧</param>
	/// <param name="elementName">要素・属性の名前</param>
	/// <param name="errorMessage">エラーメッセージ</param>
	private void CheckValidInput(string value, string[] validInputs, string elementName = null, string errorMessage = null)
	{
		if (Array.IndexOf(validInputs, value) != -1) return;
		throw new Exception(errorMessage ?? string.Format(
				"{0}要素・属性に非対応値（{1}）が設定されました。{2}のいずれかに設定してください。",
				elementName,
				value,
				string.Format("「{0}」", string.Join("」、「", validInputs))));
	}

	/// <summary>
	/// FileNameタグから出力ファイルのファイル名を作成
	/// </summary>
	/// <param name="tagFileName">FileNameタグ</param>
	/// <returns>出力ファイルのファイル名</returns>
	private string CreateFileName(XElement tagFileName)
	{
		return string.Join(string.Empty, TryGetRequiredElementList(tagFileName, "Value").Select(CreateFileNameFromValueTag));
	}

	/// <summary>
	/// 出力ファイルのファイル名の設定値を読込
	/// </summary>
	/// <param name="valueTag">FileNameタグの中にあるValueタグ</param>
	/// <returns>出力ファイルのファイル名の一部分</returns>
	private string CreateFileNameFromValueTag(XElement valueTag)
	{
		var type = TryGetRequiredAttributeValue(valueTag, "Type");
		CheckValidInput(
			type,
			new[] { "string", "dateString" },
			errorMessage: string.Format("FileNameタグ中のValueタグのType属性に未対応値({0})が設定されました。「string」、「dateString」のいずれかに設定してください。", type));
		if (type == "string")
		{
			// 固定値
			return valueTag.Value;
		}
		else
		{
			// 現日時
			CheckValidInput(
				valueTag.Value,
				new[] { CURRENT_DATE_TIME_TAG_NAME },
				errorMessage: string.Format("FileName中のType「dateString」は「Now」以外のValue({0})は対応しません。", valueTag.Value));
			return StringUtility.ToDateString(DateTime.Now, TryGetRequiredAttributeValue(valueTag, "Format"));
		}
	}

	/// <summary>
	/// Csv出力内容の設定を読込
	/// </summary>
	/// <param name="tagFieldsSetting">FieldsSettingタグ</param>
	/// <param name="setting">送り状出力設定</param>
	/// <returns>Csv内容出力用Sql文</returns>
	private string CreateSelectSqlFromSetting(XElement tagFieldsSetting, ShippingLabelExportSetting setting)
	{
		return string.Join(",\r\n", TryGetRequiredElementList(tagFieldsSetting, "Field").Select(s => CreateSelectSqlForField(s, setting)));
	}

	/// <summary>
	/// FieldタグからSql文を作成
	/// </summary>
	/// <param name="tagField">Fieldタグ</param>
	/// <param name="setting">送り状の設定ファイル</param>
	/// <returns>Sql文</returns>
	private string CreateSelectSqlForField(XElement tagField, ShippingLabelExportSetting setting)
	{
		// ヘッダー名を取得
		var headerName = TryGetRequiredAttributeValue(tagField, "Name");
		// 送り状出力列の設定
		var columnSetting = new ShippingLabelExportColumn(headerName);

		// CSV列の特別な処理情報を取得
		// 文字の種類変換の設定を取得
		var convertString = tagField.Attribute("ConvertString");
		if (convertString != null)
		{
			CheckValidInput(
				convertString.Value,
				new[] {
					ShippingLabelExportColumn.CONVERT_TO_HANKAKU, 
					ShippingLabelExportColumn.CONVERT_TO_HANKAKU_KATAKANA,
					ShippingLabelExportColumn.CONVERT_TO_ZENKAKU,
					ShippingLabelExportColumn.CONVERT_TO_ZENKAKU_HIRAGANA,
					ShippingLabelExportColumn.CONVERT_TO_ZENKAKU_KATAKANA },
				elementName: convertString.Name.LocalName);
			columnSetting.ConvertString = convertString.Value;
		}
		// 列の値のエスケープ処理設定を取得
		var isCsvEscapeColumn = tagField.Attribute("EscapeCsvColumn");
		if (isCsvEscapeColumn != null) columnSetting.IsEscapeCsvColumn = bool.Parse(isCsvEscapeColumn.Value);
		// バイト長で文字列抽出設定を取得
		var startBytePos = tagField.Attribute("StartBytePosition");
		if (startBytePos != null)
		{
			columnSetting.StartBytePosition = int.Parse(startBytePos.Value);
			columnSetting.ByteLength = int.Parse(TryGetRequiredAttributeValue(tagField, "ByteLength"));
		}
		// B2送り状の配送時間項目の設定を取得
		var isB2ShippingTimeField = tagField.Attribute("IsB2ShippingTimeField");
		if (isB2ShippingTimeField != null) columnSetting.IsB2ShippingTimeField = bool.Parse(isB2ShippingTimeField.Value);

		// 出力しない列かどうかを取得
		var isHiddenField = tagField.Attribute("IsHiddenField");
		if (isHiddenField != null) columnSetting.IsHiddenField = bool.Parse(isHiddenField.Value);

		// 設定を保存
		setting.ColumnSettings.Add(columnSetting);

		// Sql文の作成
		var str = new StringBuilder();
		if (columnSetting.IsB2ShippingTimeField)
		{
			// B2送り状の配送時間の場合
			str.Append(
				CreateSqlFieldColumn(
					TryGetRequiredElement(tagField, ShippingLabelExportSetting.B2_DELIVERY_COMPANY_ID_VALUE),
					string.Format("{0}_{1}", headerName, ShippingLabelExportSetting.B2_DELIVERY_COMPANY_ID_VALUE)));
			str.Append(ShippingLabelExportSetting.COMMA).AppendLine();
			str.Append(
				CreateSqlFieldColumn(
					TryGetRequiredElement(tagField, ShippingLabelExportSetting.B2_SHIPPING_TIME_VALUE),
					string.Format("{0}_{1}", headerName, ShippingLabelExportSetting.B2_SHIPPING_TIME_VALUE)));
		}
		else
		{
			str.Append(CreateSqlFieldColumn(tagField, headerName));
		}
		return str.ToString();
	}

	/// <summary>
	/// Fieldタグ設定から取得データ列のSql文作成
	/// </summary>
	/// <param name="element">Field・B2ShippingIdValue・B2ShippingTimeValueタグ</param>
	/// <param name="columnName">取得データの列名</param>
	/// <returns>Sql文</returns>
	private string CreateSqlFieldColumn(XElement element, string columnName)
	{
		if (element.HasElements == false) throw new Exception(string.Format("{0}タグに必須のタグがありません。", element.Name.LocalName));

		var str = new StringBuilder();
		foreach (var childElement in element.Elements())
		{
			CheckValidInput(childElement.Name.LocalName, new[] { "Substring", "Left", "Right", "Switch", "Value" }, element.Name.LocalName);
			if (childElement != element.Elements().FirstOrDefault()) str.Append(" + ");
			str.Append(CreateSqlForFieldChild(childElement));
		}
		str.AppendFormat(" AS \'{0}'", columnName);
		return str.ToString();
	}

	/// <summary>
	/// Fieldタグの子要素種類に応じる、Sql文を作成
	/// </summary>
	/// <param name="element">Fieldタグの子要素</param>
	/// <returns>Sql文</returns>
	private string CreateSqlForFieldChild(XElement element)
	{
		switch (element.Name.LocalName)
		{
			// 文字列の一部分の出力
			case "Substring":
				return CreateSelectSqlForSubstring(element);

			// 左側から文字列の何文字の出力
			case "Left":
				return CreateSelectSqlForLeft(element);

			// 右側から文字列の何文字の出力
			case "Right":
				return CreateSelectSqlForRight(element);

			// 分岐で値の出力
			case "Switch":
				return CreateSelectSqlForSwitch(element);

			// 固定文字列の出力
			case "Value":
			default:
				return CreateSelectSqlForValue(element);
		}
	}

	/// <summary>
	/// ValueタグからSql文の作成
	/// </summary>
	/// <param name="valueTag">Valueタグ</param>
	/// <returns>Sql文</returns>
	private string CreateSelectSqlForValue(XElement valueTag)
	{
		var sql = string.Empty;
		var val = valueTag.Value;
		switch (TryGetRequiredAttributeValue(valueTag, "Type"))
		{
			// 固定文字列を出力
			case "string":
				// SQLインジェクション防ぐため、シングルクオーテーションエスケープ
				sql = string.Format("'{0}'", val.Replace("'", "''"));
				break;

			// マスタ項目の値を出力
			case "field":
				sql = (val == CURRENT_DATE_TIME_TAG_NAME) ? "GETDATE()" : val;
				break;

			// 日時値を指定フォーマットで出力
			case "dateString":
			default:
				var supportedDateTimeFormatList = new Dictionary<string, int>()
				{
					{"mm/dd/yy", 1},
					{"mm/dd/yyyy", 101},
					{"yy.mm.dd", 2},
					{"yyyy.mm.dd", 102},
					{"dd/mm/yy", 3},
					{"dd/mm/yyyy", 103},
					{"dd.mm.yy", 4},
					{"dd.mm.yyyy", 104},
					{"dd-mm-yy", 5},
					{"dd-mm-yyyy", 105},
					{"mm-dd-yy", 10},
					{"mm-dd-yyyy", 110},
					{"yy/mm/dd", 11},
					{"yyyy/mm/dd", 111},
					{"yymmdd", 12},
					{"yyyymmdd", 112},
					{"hh:mi:ss", 108},
					{"hh:mi:ss:mmm", 114},
					{"yyyy-mm-dd hh:mi:ss", 120},
					{"yyyy-mm-dd hh:mi:ss.mmm", 121},
					{"yyyy-mm-ddThh:mi:ss.mmm", 126},
				};
				var format = TryGetRequiredAttributeValue(valueTag, "Format");
				var plus = (valueTag.Attribute("Plus") != null) ? valueTag.Attribute("Plus").Value : "0";
				CheckValidInput(format, supportedDateTimeFormatList.Keys.ToArray(), elementName: "Format");
				sql = string.Format(
					"CONVERT(nvarchar(30), {0}, {1})",
					(val == "Now")
						? "GETDATE()"
						: (val == "Add")
							? "DATEADD(DAY, " + plus + ", GETDATE())"
							: val, supportedDateTimeFormatList[format]);
				break;
		}

		return sql;
	}

	/// <summary>
	/// SubstringタグからSql文を作成
	/// </summary>
	/// <param name="tagSubstring">Substringタグ</param>
	/// <returns>Sql文</returns>
	private string CreateSelectSqlForSubstring(XElement tagSubstring)
	{
		return string.Format(
			"SUBSTRING({0}, {1}, {2})",
			string.Join(" + ", TryGetRequiredElementList(tagSubstring, "Value").Select(CreateSelectSqlForValue)),
			TryGetRequiredAttributeValue(tagSubstring, "Start"),
			TryGetRequiredAttributeValue(tagSubstring, "Length"));
	}

	/// <summary>
	/// LeftタグからSql文を作成
	/// </summary>
	/// <param name="tagLeft">Leftタグ</param>
	/// <returns>Sql文</returns>
	private string CreateSelectSqlForLeft(XElement tagLeft)
	{
		return string.Format(
			"LEFT({0}, {1})",
			string.Join(" + ", TryGetRequiredElementList(tagLeft, "Value").Select(CreateSelectSqlForValue)),
			TryGetRequiredAttributeValue(tagLeft, "Length"));
	}

	/// <summary>
	/// RightタグからSql文を作成
	/// </summary>
	/// <param name="tagRight">Rightタグ</param>
	/// <returns>Sql文</returns>
	private string CreateSelectSqlForRight(XElement tagRight)
	{
		return string.Format(
			"RIGHT({0}, {1})",
			string.Join(" + ", TryGetRequiredElementList(tagRight, "Value").Select(CreateSelectSqlForValue)),
			TryGetRequiredAttributeValue(tagRight, "Length"));
	}

	/// <summary>
	/// SwitchタグからSql文を作成
	/// </summary>
	/// <param name="tagSwitch">Switchタグ</param>
	/// <returns>Sql文</returns>
	private string CreateSelectSqlForSwitch(XElement tagSwitch)
	{
		return string.Format(
			"CASE{0}{1}{2}{0}END",
			Environment.NewLine,
			string.Join(string.Empty, TryGetRequiredElementList(tagSwitch, "Case").Select(CreateSqlForCaseTag)),
			CreateSqlForElseTag(TryGetRequiredElement(tagSwitch, "Else")));
	}

	/// <summary>
	/// CaseタグからSql文を作成
	/// </summary>
	/// <param name="caseTag">Caseタグ</param>
	/// <returns>Sql文</returns>
	private string CreateSqlForCaseTag(XElement caseTag)
	{
		return string.Format(
			"WHEN {0} THEN {1}{2}",
			TryGetRequiredElementValue(caseTag, "Condition"),
			CreateSelectSqlForValue(TryGetRequiredElement(caseTag, "Value")),
			Environment.NewLine);
	}

	/// <summary>
	/// ElseタグからSql文を作成
	/// </summary>
	/// <param name="elseTag">Elseタグ</param>
	/// <returns>Sql文</returns>
	private string CreateSqlForElseTag(XElement elseTag)
	{
		return string.Format("ELSE {0}", CreateSelectSqlForValue(TryGetRequiredElement(elseTag, "Value")));
	}

	/// <summary>
	/// B2送り状の配送時間を出力
	/// </summary>
	/// <param name="deliveryCompanyId">配送会社ID</param>
	/// <param name="shippingTime">配送時間帯コード</param>
	/// <returns>B2送り状の配送時間</returns>
	private string ConvertB2ShippingTime(string deliveryCompanyId, string shippingTime)
	{
		if (string.IsNullOrEmpty(shippingTime)) return "";

		var shopShippingTimeMessage = GetShippingTimeMessageFromDb(deliveryCompanyId, shippingTime);
		// ヤフー側設定に合わせた場合のB2出力 …「XX:XX～XX:XX」の形式
		if (Regex.IsMatch(shopShippingTimeMessage, @"[^0-9]*[0-9]{1,2}[:|：][0-9]{1,2}～[0-9]{1,2}[:|：][0-9]{1,2}"))
		{
			shopShippingTimeMessage = Regex.Replace(shopShippingTimeMessage, "[:|：][0-9]{1,2}", ""); // 余計な部分を取り去る
			var mc = Regex.Matches(shopShippingTimeMessage, @"[0-9]{1,2}");

			shopShippingTimeMessage = null;
			foreach (Match match in mc)
			{
				int format;
				if (int.TryParse(match.Value, out format)) shopShippingTimeMessage += format.ToString("00");
			}
		}
		// その他、B2出力 …「XX～XX～」などの形式
		else if (Regex.IsMatch(shopShippingTimeMessage, @"[^0-9]*[0-9]{2}[^0-9]+[0-9]{2}[^0-9]"))
		{
			var mc = Regex.Matches(shopShippingTimeMessage, @"[0-9]{2}");
			shopShippingTimeMessage = mc[0].Value + mc[1].Value;
		}
		else if (shopShippingTimeMessage.Contains("午前"))
		{
			// 午前中指定は[0812]
			shopShippingTimeMessage = "0812";
		}
		return shopShippingTimeMessage;
	}

	/// <summary>
	/// DBから配送時間帯文言を取得
	/// </summary>
	/// <param name="deliveryCompanyId">配送会社ID</param>
	/// <param name="shippingTime">配送希望時間帯コード</param>
	/// <returns>配送時間帯文言</returns>
	private string GetShippingTimeMessageFromDb(string deliveryCompanyId, string shippingTime)
	{
		var deliveryCompanyInfo = new DeliveryCompanyService().Get(deliveryCompanyId);

		if (deliveryCompanyInfo == null) return "";

		if (shippingTime == deliveryCompanyInfo.ShippingTimeId1)
		{
			return deliveryCompanyInfo.ShippingTimeMessage1;
		}
		else if (shippingTime == deliveryCompanyInfo.ShippingTimeId2)
		{
			return deliveryCompanyInfo.ShippingTimeMessage2;
		}
		else if (shippingTime == deliveryCompanyInfo.ShippingTimeId3)
		{
			return deliveryCompanyInfo.ShippingTimeMessage3;
		}
		else if (shippingTime == deliveryCompanyInfo.ShippingTimeId4)
		{
			return deliveryCompanyInfo.ShippingTimeMessage4;
		}
		else if (shippingTime == deliveryCompanyInfo.ShippingTimeId5)
		{
			return deliveryCompanyInfo.ShippingTimeMessage5;
		}
		else if (shippingTime == deliveryCompanyInfo.ShippingTimeId6)
		{
			return deliveryCompanyInfo.ShippingTimeMessage6;
		}
		else if (shippingTime == deliveryCompanyInfo.ShippingTimeId7)
		{
			return deliveryCompanyInfo.ShippingTimeMessage7;
		}
		else if (shippingTime == deliveryCompanyInfo.ShippingTimeId8)
		{
			return deliveryCompanyInfo.ShippingTimeMessage8;
		}
		else if (shippingTime == deliveryCompanyInfo.ShippingTimeId9)
		{
			return deliveryCompanyInfo.ShippingTimeMessage9;
		}
		else if (shippingTime == deliveryCompanyInfo.ShippingTimeId10)
		{
			return deliveryCompanyInfo.ShippingTimeMessage10;
		}
		return "時間指定あり";
	}

	/// <summary>
	/// 送り状出力設定によって受注データをCSV形式の文字列で出力
	/// </summary>
	/// <param name="order">受注データ</param>
	/// <param name="setting">送り状出力設定</param>
	/// <returns>Csvファイルの中身</returns>
	public string CreateCsv(DataView order, ShippingLabelExportSetting setting)
	{
		var csvContent = new StringBuilder();
		// Csvヘッダー出力
		if (setting.ExportHeader)
		{
			csvContent.AppendLine(
				string.Format(
					"\"{0}\"",
					string.Join(
						"\",\"",
						setting.ColumnSettings.Where(s => s.IsHiddenField == false).Select(s => s.HeaderName)
							.ToArray())));
		}
			
		// Csvデータ出力
		foreach (DataRowView rowView in order)
		{
			if (setting.IsEMS)
			{
				var orderInfo = new OrderService().Get((string)rowView[Constants.FIELD_ORDER_ORDER_ID]);

				// 21種類以上または日本の場合csv出力しないでログ出力
				if ((string)rowView[setting.ColumnSettings.First(c => c.HeaderName.Contains("国名コード")).HeaderName] == "JP")
				{
					FileLogger.WriteInfo("国コードが日本のためEMS送り状を出力しませんでした");
					continue;
				}
				if (orderInfo.OrderItemCount > 20)
				{
					FileLogger.WriteInfo(
						string.Format(
							"受注ID:{0}は商品明細が{1}件のためEMS送り状を出力できませんでした",
							(string)rowView[Constants.FIELD_ORDER_ORDER_ID],
							orderInfo.OrderItemCount));
					continue;
				}

				OutputColumnValue(setting, csvContent, rowView);

				foreach (var orderItem in orderInfo.Items)
				{
					foreach (var shippingContentsExportColumn in setting.ContentsSettingEMS)
					{
						// 区切り文字を追加
						csvContent.Append(setting.Separator);
						// 内容品の値を追加
						var contentsValue = GetContentsValue(orderInfo, orderItem, shippingContentsExportColumn);
						csvContent.Append(OutputQuotationMarks(setting, contentsValue));
					}
				}
			}
			else
			{
				OutputColumnValue(setting, csvContent, rowView);
			}

			csvContent.AppendLine();
		}
		return csvContent.ToString();
	}

	/// <summary>
	/// 受注情報を送り状出力列ごとにcsv文字列に追加
	/// </summary>
	/// <param name="setting">送り状出力設定</param>
	/// <param name="csvContent">csv文字列</param>
	/// <param name="rowView">受注情報</param>
	private void OutputColumnValue(ShippingLabelExportSetting setting, StringBuilder csvContent, DataRowView rowView)
	{
		for (var i = 0; i < setting.ColumnSettings.Count; i++)
		{
			var colSetting = setting.ColumnSettings[i];

			if (colSetting.IsHiddenField) continue;

			string val = null;

			// 2列目以降の場合、区切り文字を追加
			if (i > 0) csvContent.Append(setting.Separator);

			// B2送り状の配送時間列かどうかにより処理
			val =
				(colSetting.IsB2ShippingTimeField)
					? ConvertB2ShippingTime(
						StringUtility.ToEmpty(rowView[string.Format("{0}_{1}", colSetting.HeaderName, ShippingLabelExportSetting.B2_DELIVERY_COMPANY_ID_VALUE)]),
						StringUtility.ToEmpty(rowView[string.Format("{0}_{1}", colSetting.HeaderName, ShippingLabelExportSetting.B2_SHIPPING_TIME_VALUE)]))
					: StringUtility.ToEmpty(rowView[colSetting.HeaderName]);

			// 文字の種類を変換
			switch (colSetting.ConvertString)
			{
				case ShippingLabelExportColumn.CONVERT_TO_HANKAKU:
					val = StringUtility.ToHankaku(val);
					break;

				case ShippingLabelExportColumn.CONVERT_TO_HANKAKU_KATAKANA:
					val = StringUtility.ToHankakuKatakana(val);
					break;

				case ShippingLabelExportColumn.CONVERT_TO_ZENKAKU:
					val = StringUtility.ToZenkaku(val);
					break;

				case ShippingLabelExportColumn.CONVERT_TO_ZENKAKU_HIRAGANA:
					val = StringUtility.ToZenkakuHiragana(val);
					break;

				case ShippingLabelExportColumn.CONVERT_TO_ZENKAKU_KATAKANA:
					val = StringUtility.ToZenkakuKatakana(val);
					break;

				default:
					// デフォルト値（空）の場合、何も処理しない
					break;
			}

			// 文字列の部分文字列をバイト長で指定した範囲で出力
			if ((colSetting.StartBytePosition != -1) && (colSetting.ByteLength != -1))
			{
				val = StringUtility.GetWithSpecifiedByteLength(val, colSetting.StartBytePosition, colSetting.ByteLength, setting.FileEncoding);
			}

			// 列の値をエスケープして出力
			// ・改行のエスケープ（\r\n → \n、\r → \n）
			// ・ダブルクオートのエスケープ（" → ""）
			if (colSetting.IsEscapeCsvColumn) val = StringUtility.EscapeCsvColumn(val);

			csvContent.Append(OutputQuotationMarks(setting, val));
		}
	}

	/// <summary>
	/// 引用符出力
	/// 例：引用符が「"」、1列目の値は「123」、2列目の値は「abc」、3列目の値は「1,2」の場合、
	///    ・引用符常時出力有無（AlwaysExportQuotation）がtrue の場合、"123","abc","1,2"を出力しますが、
	///    ・引用符常時出力有無（AlwaysExportQuotation）がfalse の場合、123,abc,"1,2"を出力します。（値にカンマ(,)があるときは強制エスケープ）
	/// </summary>
	/// <param name="setting">送り状出力設定</param>
	/// <param name="val">出力する値</param>
	/// <returns>設定によって引用符を付けた文字列</returns>
	private string OutputQuotationMarks(ShippingLabelExportSetting setting, string val)
	{
		var quoted = setting.AlwaysExportQuotation
			|| ((setting.Separator == ShippingLabelExportSetting.COMMA)
				&& val.Contains(ShippingLabelExportSetting.COMMA.ToString()))
				? string.Format("{0}{1}{0}", setting.StringQuotationMark, val)
				: val;
		return quoted;
	}

	/// <summary>
	/// 内容品の値を取得
	/// </summary>
	/// <param name="order">受注</param>
	/// <param name="orderItem">受注明細</param>
	/// <param name="shippingContentsExportColumn">内容品出力設定</param>
	/// <returns>内容品の列の値</returns>
	private string GetContentsValue(OrderModel order, OrderItemModel orderItem, ShippingContentsExportColumn shippingContentsExportColumn)
	{
		var contentVal = string.Empty;
		switch (shippingContentsExportColumn.FieldType)
		{
			case ShippingContentsExportColumn.FIELD:
				contentVal = (orderItem.DataSource[shippingContentsExportColumn.Value] is string)
					? (string)orderItem.DataSource[shippingContentsExportColumn.Value]
					: (orderItem.DataSource[shippingContentsExportColumn.Value] is int)
						? ((int)orderItem.DataSource[shippingContentsExportColumn.Value]).ToString()
						: ((decimal)orderItem.DataSource[shippingContentsExportColumn.Value]).ToPriceDecimal().ToString();
				break;

			case ShippingContentsExportColumn.PRODUCT_FIELD:
				// バリエーションごとの重量取得
				var product = new ProductService().GetProductVariation(
					order.ShopId,
					orderItem.ProductId,
					orderItem.VariationId,
					order.MemberRankId);
				contentVal = product.VariationWeightGram.ToString();
				break;

			case ShippingContentsExportColumn.STRING_FIELD:
				contentVal = shippingContentsExportColumn.Value;
				break;
		}

		return contentVal;
	}

	/// <summary>
	/// 内容品出力内容の設定を読込
	/// </summary>
	/// <param name="contentsSetting">ContentsSettingタグ</param>
	/// <returns>内容品出力設定のリスト</returns>
	private List<ShippingContentsExportColumn> GetContentSetting(XElement contentsSetting)
	{
		var shippingContentsExportColumns = new List<ShippingContentsExportColumn>();
		var fieldList = TryGetRequiredElementList(contentsSetting, "Field");

		foreach (var element in fieldList)
		{
			var attribute = element.Attribute("Type");
			var shippingContentsExportColumn = new ShippingContentsExportColumn
			{
				FieldType = (attribute != null) ? attribute.Value : string.Empty,
				Value = element.Value
			};
			shippingContentsExportColumns.Add(shippingContentsExportColumn);
		}

		return shippingContentsExportColumns;
	}

	/// <summary>
	/// 送り状出力用の設定クラス
	/// </summary>
	public class ShippingLabelExportSetting
	{
		/// <summary>タブ区切り文字</summary>
		public const char TAB = '\t';
		/// <summary>カンマ区切り文字</summary>
		public const char COMMA = ',';
		/// <summary>カンマ区切り設置値</summary>
		public const string COMMA_STRING = "COMMA";
		/// <summary>タブ区切り設置値</summary>
		public const string TAB_STRING = "TAB";
		/// <summary>配送会社ID値のヘッダー名</summary>
		public const string B2_DELIVERY_COMPANY_ID_VALUE = "B2DeliveryCompanyIdValue";
		/// <summary>配送時間帯コード値のヘッダー名</summary>
		public const string B2_SHIPPING_TIME_VALUE = "B2ShippingTimeValue";

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ShippingLabelExportSetting()
		{
			this.ColumnSettings = new List<ShippingLabelExportColumn>();
		}

		/// <summary>
		/// 注文一覧画面用Sqlステートメント名取得
		/// </summary>
		/// <returns>Sqlステートメント名</returns>
		public string GetOrderListPageSqlStatementName()
		{
			var statement = "GetOrderMaster";
			// 条件によって変える
			if (this.UnitType == "AtodeneInvoice")
			{
				statement = "GetOrderMasterAtodeneInvoice";
			}

			// DSK後払い(債権保証なし)
			if (this.DisplayName == DSK_DEF_DISPLAY_NAME)
			{
				statement = "GetOrderItemMasterForDskDef";
			}

			// DSK後払い(債権保証あり)
			if (this.UnitType == "DskDeferredInvoice")
			{
				statement = "GetOrderItemMasterForCreditDskDef";
			}

			if (this.UnitType == "OrderItem")
			{
				statement = "GetOrderItemMaster";
			}

			if (this.DisplayName == ATOBARAICOM_DEF_DISPLAY_NAME)
			{
				statement = "GetOrderMasterWMS";
			}

			if (this.UnitType == "ScoreInvoice")
			{
				statement = "GetOrderItemMasterScoreInvoice";
			}

			if (this.DisplayName == VERITRANS_DEF_DISPLAY_NAME)
			{
				statement = "GetOrderMasterVeritransInvoice";
			}

			return statement;
		}

		/// <summary>
		/// 受注ワークフロー画面用Sqlステートメント名取得
		/// </summary>
		/// <returns>Sqlステートメント名</returns>
		public string GetOrderWorkFlowPageSqlStatementName()
		{
			var statement = "GetOrderWorkflowMaster";
			// 条件によって変える
			if (this.UnitType == "AtodeneInvoice")
			{
				statement = "GetOrderWorkflowMasterAtodeneInvoice";
			}

			if (this.DisplayName == DSK_DEF_DISPLAY_NAME)
			{
				statement = "GetOrderItemWorkflowMasterForDskDef";
			}

			// DSK後払い(債権保証あり)
			if (this.UnitType == "DskDeferredInvoice")
			{
				statement = "GetOrderWorkflowMasterrForCreditDskDefInvoice";
			}

			if (this.UnitType == "OrderItem")
			{
				statement = "GetOrderItemWorkflowMaster";
			}

			if (this.DisplayName == ATOBARAICOM_DEF_DISPLAY_NAME)
			{
				statement = "GetOrderWorkflowMasterWMS";
			}

			if (this.UnitType == "ScoreInvoice")
			{
				statement = "GetOrderWorkflowMasterScoreInvoice";
			}

			if (this.DisplayName == VERITRANS_DEF_DISPLAY_NAME)
			{
				statement = "GetOrderWorkflowMasterVeritransInvoice";
			}

			return statement;
		}

		#region プロパティ
		/// <summary>送り状ダウンロードリンクのテキスト</summary>
		public string DisplayName { get; set; }
		/// <summary>出力ファイル種類（設定可能値：csv）</summary>
		public string FormatType { get; set; }
		/// <summary>出力データ単位（設定可能値：Order、AtodeneInvoice）</summary>
		public string UnitType { get; set; }
		/// <summary>出力ファイル名</summary>
		public string FileName { get; set; }
		/// <summary>ヘッダ行出力有無（設定可能値：true, false）</summary>
		public bool ExportHeader { get; set; }
		/// <summary>
		/// 引用符常時出力有無（設定可能値：true, false）
		/// 例：文字列の引用符は「"」を設定して、1列目の値は「123」、2列目の値は「abc」、3列目の値は「1,2」の場合、
		/// trueの場合、"123","abc","1,2"を出力しますが、
		/// falseの場合、123,abc,"1,2"を出力します。
		/// </summary>
		public bool AlwaysExportQuotation { get; set; }
		/// <summary>文字列の引用符</summary>
		public char StringQuotationMark { get; set; }
		/// <summary>区切り文字（設定可能値：COMMA, TAB）</summary>
		public char Separator { get; set; }
		/// <summary>文字コード（設定可能値：Shift_JIS、 UTF-8、 Big5）</summary>
		public Encoding FileEncoding { get; set; }
		/// <summary>ヘッダー名</summary>
		public List<ShippingLabelExportColumn> ColumnSettings { get; set; }
		/// <summary>Sql文</summary>
		public string SelectSql { get; set; }
		/// <summary>内容品出力設定</summary>
		public List<ShippingContentsExportColumn> ContentsSettingEMS { get; set; }
		/// <summary>EMS送り状出力か</summary>
		public bool IsEMS
		{
			get { return ContentsSettingEMS != null; }
		}
		/// <summary>実行SQL</summary>
		public string SqlExport { get; set; }
		#endregion
	}

	/// <summary>
	/// 送り状出力列の設定クラス
	/// </summary>
	public class ShippingLabelExportColumn
	{
		/// <summary>半角への変換</summary>
		public const string CONVERT_TO_HANKAKU = "ToHankaku";
		/// <summary>半角カタカナへ変換</summary>
		public const string CONVERT_TO_HANKAKU_KATAKANA = "ToHankakuKatakana";
		/// <summary>全角への変換</summary>
		public const string CONVERT_TO_ZENKAKU = "ToZenkaku";
		/// <summary>全角ひらがなへ変換</summary>
		public const string CONVERT_TO_ZENKAKU_HIRAGANA = "ToZenkakuHiragana";
		/// <summary>全角カタカナへ変換</summary>
		public const string CONVERT_TO_ZENKAKU_KATAKANA = "ToZenkakuKatakana";

		/// <summary>
		/// コンストラクタ（初期化）
		/// </summary>
		/// <param name="headerName">出力列のヘッダー名</param>
		public ShippingLabelExportColumn(string headerName)
		{
			this.HeaderName = headerName;
			this.IsEscapeCsvColumn = false;
			this.StartBytePosition = -1;
			this.ByteLength = -1;
			this.ConvertString = string.Empty;
			this.IsB2ShippingTimeField = false;
			this.IsHiddenField = false;
		}

		#region プロパティ
		/// <summary>出力列のヘッダー名</summary>
		public string HeaderName { get; set; }
		/// <summary>エスケープCSV対応かどうか</summary>
		public bool IsEscapeCsvColumn { get; set; }
		/// <summary>バイト長で取得開始位置</summary>
		public int StartBytePosition { get; set; }
		/// <summary>バイト長で取得長さ</summary>
		public int ByteLength { get; set; }
		/// <summary>文字列変換対応かどうか</summary>
		public string ConvertString { get; set; }
		/// <summary>B2送り状の配送時間項目かどうか</summary>
		public bool IsB2ShippingTimeField { get; set; }
		/// <summary>出力しない列かどうか</summary>
		public bool IsHiddenField { get; set; }
		#endregion
	}

	/// <summary>
	/// 内容品出力列の設定クラス(EMSで使用)
	/// </summary>
	public class ShippingContentsExportColumn
	{
		/// <summary>OrderItemのField</summary>
		public const string FIELD = "field";
		/// <summary>ProductのField</summary>
		public const string PRODUCT_FIELD = "product_field";
		/// <summary>文字列</summary>
		public const string STRING_FIELD = "string";

		#region プロパティ
		/// <summary>フィールタイプ</summary>
		public string FieldType { get; set; }
		/// <summary>バリュー</summary>
		public string Value { get; set; }
		#endregion
	}
}
