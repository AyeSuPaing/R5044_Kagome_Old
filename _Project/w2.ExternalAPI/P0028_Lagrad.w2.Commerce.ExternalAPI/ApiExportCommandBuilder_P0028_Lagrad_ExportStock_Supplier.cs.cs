using System.Data;
using w2.App.Common.Stock;
using w2.ExternalAPI.Common.Export;

namespace P0028_Lagrad.w2.Commerce.ExternalAPI
{
	/// <summary>
	/// サプライア（店舗）側の在庫Exportコマンド。在庫の絶対数ファイルを作成
	/// </summary>
	public class ApiExportCommandBuilder_P0028_Lagrad_ExportStock_Supplier : ApiExportCommandBuilder
	{
		public override DataTable GetDataTable()
		{
			// コマンドライン引数から対象サプライアIDをとってくる
			string supplier = base.Properties["supplier"];
			int term = int.Parse(base.Properties["term"]);

			// サプライアIDを元に在庫数取得
			// 在庫数が在庫安全基準値以下の場合は在庫数は0として返却される
			StockCommon stockCommon = new StockCommon();
			DataTable dt = stockCommon.GetStockBySupplier(supplier,term);

			return dt;

		}

		/// <summary>
		/// エクスポート
		/// </summary>
		/// <param name="record">対象レコード</param>
		/// <returns>エクスポート値</returns>
		protected override object[] Export(IDataRecord record)
		{
			// そのまま
			return record.ToArray();
		}
	}
}
