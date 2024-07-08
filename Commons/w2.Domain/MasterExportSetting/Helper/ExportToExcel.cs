/*
=========================================================================================================
  Module      : エクセル出力クラス(ExportToExcel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace w2.Domain.MasterExportSetting.Helper
{
	/// <summary>
	/// エクセルファイル出力用クラス
	/// </summary>
	/// <remarks>Commerce.Manager側のテンプレートを利用します。</remarks>
	public class ExportToExcel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="templateSetting">テンプレート設定</param>
		public ExportToExcel(ExcelTemplateSetting templateSetting)
		{
			this.TemplateSetting = templateSetting;

			// ネームスペースを設定
			var document = XDocument.Load(this.TemplateSetting.TemplatePath, LoadOptions.None);
			this.NamespaceReportDefinition = document.Root.GetDefaultNamespace();
			this.NamespaceReportDesigner = document.Root.GetNamespaceOfPrefix("rd");

			// テンプレート用の要素を設定
			this.TemplateElements = XDocument.Load(this.TemplateSetting.TemplateSettingPath, LoadOptions.None);
		}

		/// <summary>
		/// エクセルファイル作成
		/// </summary>
		/// <param name="source">データソース</param>
		/// <param name="sOutPutStream">出力用ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		public void Create(DataView source, Stream sOutPutStream, string formatDate)
		{
			// テンプレートの読み込み
			var document = XDocument.Load(this.TemplateSetting.TemplatePath, LoadOptions.None);

			document.Root.GetNamespaceOfPrefix("xmlns:rd");

			// データフィールドの追加
			AddDataFieldElements(document, source.Table.Columns);

			// 列の追加
			AddColumnElements(document, source.Table.Columns);

			// 行の追加
			AddRowElements(document, source.Table.Columns, formatDate);

			// ColumnHierarchyに列数分のメンバを追加
			AddColumnHierarchyElements(document, source.Table.Columns);

			// レポートをStreamに書き出し
			new ReportRenderer().Write(new StringReader(document.ToString()), source, this.GetExcelFileType, sOutPutStream);
		}

		/// <summary>
		/// DataField追加
		/// </summary>
		/// <param name="document">XMLドキュメント</param>
		/// <param name="dataColumnCollection">データカラムコレクション</param>
		private void AddDataFieldElements(XDocument document, DataColumnCollection dataColumnCollection)
		{
			// Field要素を追加する場所の選択
			var fields = GetElement(document, this.NamespaceReportDefinition, "Fields");

			// Fieldのテンプレート取得
			var fieldTemplate = new XElement(GetElement(this.TemplateElements, this.NamespaceReportDefinition, "Field"));

			// 列名と列のデータタイプ(System.String等)を設定する
			foreach (DataColumn column in dataColumnCollection)
			{
				// 新しく入れ物を作成する
				var field = new XElement(fieldTemplate);
				var dataField = GetElement(field, this.NamespaceReportDefinition, "DataField");
				var typeName = GetElement(field, this.NamespaceReportDesigner, "TypeName");

				// 値設定
				dataField.Value = column.ColumnName;
				typeName.Value = column.DataType.ToString();
				field.SetAttributeValue("Name", column.ColumnName);

				// 元データに作成したエレメント追加
				fields.Add(field);
			}
		}

		/// <summary>
		/// テーブル列要素設定
		/// </summary>
		/// <param name="document">XMLドキュメント</param>
		/// <param name="dataColumnCollection">データカラムコレクション</param>
		private void AddColumnElements(XDocument document, DataColumnCollection dataColumnCollection)
		{
			// 列を追加する場所の選択
			var tablixColumns = GetElement(document, this.NamespaceReportDefinition, "TablixColumns");

			// Columnのテンプレート読み込み
			var tablixColumn = new XElement(GetElement(this.TemplateElements, this.NamespaceReportDefinition, "TablixColumn"));

			// 値を入れ替える為、TablixColumns以下のノードを削除する
			tablixColumns.RemoveNodes();

			// 列数を設定する
			for (var i = 0; i < dataColumnCollection.Count; i++)
			{
				tablixColumns.Add(new XElement(tablixColumn));
			}
		}

		/// <summary>
		/// テーブル行要素設定
		/// </summary>
		/// <param name="document">XMLドキュメント</param>
		/// <param name="dataColumnCollection">データカラムコレクション</param>
		/// <param name="formatDate">日付形式</param>
		private void AddRowElements(XDocument document, DataColumnCollection dataColumnCollection, string formatDate = "")
		{
			// Cellのテンプレートを取得
			var cellTemplate = new XElement(GetElement(this.TemplateElements, this.NamespaceReportDefinition, "TablixCell"));

			// 行要素取得
			var rowTemplate = GetElement(this.TemplateElements, this.NamespaceReportDefinition, "TablixRow");

			// ヘッダ行用テンプレート取得
			var headerRow = new XElement(rowTemplate);
			var headerCells = GetElement(headerRow, this.NamespaceReportDefinition, "TablixCells");

			// 明細行用テンプレート取得
			var detailRow = new XElement(rowTemplate);
			var detailCells = GetElement(detailRow, this.NamespaceReportDefinition, "TablixCells");

			foreach (DataColumn dcColumn in dataColumnCollection)
			{
				var columnName = dcColumn.ColumnName;
				if (dcColumn.ColumnName == "入庫儲位_良品")
				{
					columnName = "入庫儲位(良品)";
				}

				// ヘッダ行設定
				GetElement(cellTemplate, NamespaceReportDefinition, "Value").Value = columnName;
				GetElement(cellTemplate, this.NamespaceReportDefinition, "Textbox").SetAttributeValue("Name", "head_" + dcColumn.ColumnName);
				headerCells.Add(new XElement(cellTemplate));

				// 明細行設定 ※日付形式が指定されるかつ日付データの場合、日付を変換する
				var fieldValue = "Fields!" + dcColumn.ColumnName + ".Value";
				GetElement(cellTemplate, NamespaceReportDefinition, "Value").Value = "="
					+ (((formatDate != "") && (dcColumn.DataType == typeof(System.DateTime)))
						? "Format(" + fieldValue + ", \"" + formatDate + "\")"
						: fieldValue);

				GetElement(cellTemplate, this.NamespaceReportDefinition, "Textbox").SetAttributeValue("Name", dcColumn.ColumnName);
				detailCells.Add(new XElement(cellTemplate));
			}

			// 各行設定
			GetElement(document, this.NamespaceReportDefinition, "TablixRows").Add(headerRow, detailRow);
		}

		/// <summary>
		/// ColumnHierarchyを設定
		/// </summary>
		/// <param name="document">XMLドキュメント</param>
		/// <param name="dataColumnCollection">データカラムコレクション</param>
		private void AddColumnHierarchyElements(XDocument document, DataColumnCollection dataColumnCollection)
		{
			// 各階層のXML要素の取得
			var xeHierarchy = GetElement(document, this.NamespaceReportDefinition, "TablixColumnHierarchy");
			var xeMembers = GetElement(xeHierarchy, this.NamespaceReportDefinition, "TablixMembers");
			var xeMember = new XElement(GetElement(this.TemplateElements, this.NamespaceReportDefinition, "TablixMember"));

			// 列数分設定する
			for (var i = 0; i < dataColumnCollection.Count; i++)
			{
				xeMembers.Add(xeMember);
			}
		}

		/// <summary>
		/// XML要素取得
		/// </summary>
		/// <param name="container">コンテナ</param>
		/// <param name="nameSpace">XMLネームスペース</param>
		/// <param name="target">要素名</param>
		/// <returns>該当要素</returns>
		private XElement GetElement(XContainer container, XNamespace nameSpace, string target)
		{
			return container.Descendants(nameSpace + target).ElementAt(0);
		}

		/// <summary>エクセスファイルタイプ取得（EXCE or EXCELOPENXML）</summary>
		private string GetExcelFileType
		{
			get
			{
				return (Constants.MASTEREXPORT_EXCELFORMAT == ".xlsx") ? "EXCELOPENXML" : "EXCEL";
			}
		}
		/// <summary>テンプレート設定</summary>
		private ExcelTemplateSetting TemplateSetting { get; set; }
		/// <summary>ネームスペース（デフォルト）</summary>
		private XNamespace NamespaceReportDefinition { get; set; }
		/// <summary>ネームスペース（レポートデザイナ）</summary>
		private XNamespace NamespaceReportDesigner { get; set; }
		/// <summary>テンプレート用要素群</summary>
		private XDocument TemplateElements { get; set; }
	}
}