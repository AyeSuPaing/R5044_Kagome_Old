/*
=========================================================================================================
  Module      : レポートレンダラ(ReportRenderer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.IO;
using Microsoft.Reporting.WebForms;

namespace w2.Domain.MasterExportSetting.Helper
{
	/// <summary>
	/// レポートレンダラ
	/// </summary>
	public class ReportRenderer
	{
		/// <summary>
		/// レポート書き出し
		/// </summary>
		/// <param name="reportCompositionData">レポート構成データ</param>
		/// <param name="source">データソース</param>
		/// <param name="fileType">ファイルタイプ（EXCEL/PDF）</param>
		/// <param name="outputStream">出力ストリーム</param>
		public void Write(TextReader reportCompositionData, IEnumerable source, string fileType, Stream outputStream)
		{
			using (var lr = new LocalReport())
			{
				lr.LoadReportDefinition(reportCompositionData);
				lr.DataSources.Add(new ReportDataSource("DataSet", source));

				// 作成
				Warning[] warnings;
				string mimeType;
				string fileNameExtension;
				string encoding;
				string[] stream;
				var bytes = lr.Render(fileType, null, PageCountMode.Actual, out mimeType, out encoding, out fileNameExtension, out stream, out warnings);

				// ストリームに書き出し
				outputStream.Write(bytes, 0, bytes.Length);
			}
		}
	}
}