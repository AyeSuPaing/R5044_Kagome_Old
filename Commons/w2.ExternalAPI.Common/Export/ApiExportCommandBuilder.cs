/*
=========================================================================================================
  Module      : エクスポートコマンドビルダ抽象クラス(ApiExportCommandBuilder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data;
using w2.ExternalAPI.Common.Logging;

namespace w2.ExternalAPI.Common.Export
{
	/// <summary>
	/// エクスポートコマンドビルダ基底クラス
	/// </summary>
	public abstract class ApiExportCommandBuilder : IDisposable
	{
		protected const string FIELD_API_EXPORT_ORDER_ID = "OrderId";
		protected const string FIELD_API_EXPORT_PRODUCT_ID = "ProductId";
		protected const string FIELD_API_EXPORT_VARIATION_ID = "VariationId";
		protected const string FIELD_API_EXPORT_PRODUCT_PRICE = "ProductPrice";
		protected const string FIELD_API_EXPORT_PRODUCT_TAX_RATE = "TaxRate";
		protected const string FIELD_API_EXPORT_PRODUCT_TAX_ROUND_TYPE = "TaxRoundType";
		protected const string FIELD_API_EXPORT_PRODUCT_PRICE_ORG = "ProductPriceOrg";
		protected const string FIELD_API_EXPORT_ITEM_QUANTITY = "ItemQuantity";
		protected const string FIELD_API_EXPORT_ORDER_STATUS = "OrderStatus";
		protected const string FIELD_API_EXPORT_RETURN_EXCHANGE_KBN = "ReturnExchangeKbn";
		protected const string FIELD_API_EXPORT_TIMESPAN = "Timespan";

		#region メンバ変数
		protected DataTableReader m_ResultDataTable;
		#endregion

		#region コンストラクタ
		/// <summary>コンストラクタ</summary>
		protected ApiExportCommandBuilder()
		{
			Init();
		}
		#endregion

		#region +GetDataTable 出力データを一括取得
		public abstract DataTable GetDataTable();
		#endregion

		#region #PreDo 実行前処理
		/// <summary>
		/// 実行前処理
		/// </summary>
		public virtual void PreDo() { }
		#endregion

		#region +Do RowResultを加工
		/// <summary>RowResultを加工</summary>
		/// <returns></returns>
		public object[] Do(IDataRecord record)
		{
			PreExport(record);
			CheckApiData(record);

			object[] objects = Export(record);
			CheckRecordData(objects);

			return objects;
		}
		#endregion

		#region #PostDo 実行後処理
		/// <summary>
		/// 実行後処理
		/// </summary>
		public virtual void PostDo() { }
		#endregion

		#region #Init 初期化処理
		/// <summary>初期化処理</summary>
		protected virtual void Init()
		{
			// 起動ログ
			ApiLogger.Write(LogLevel.info, "コマンドビルダー開始:" + GetType().Name, "");
		}
		#endregion

		#region #PreExport 出力前処理
		/// <summary>出力前処理</summary>
		protected virtual void PreExport(IDataRecord record) { }
		#endregion

		#region #Export 出力処理
		/// <summary>出力処理</summary>
		protected abstract object[] Export(IDataRecord record);
		#endregion

		#region #EndExport ファイル生成後
		/// <summary>ファイル生成後</summary>
		/// <param name="filePath">生成したファイルのフルパス</param>
		public virtual void EndExport(string filePath) { }
		#endregion

		#region #PostExport 出力後処理
		/// <summary>出力後処理</summary>
		public virtual void PostExport(object[] objects) { }
		#endregion

		#region #End 終了処理
		/// <summary>終了処理</summary>
		protected virtual void End()
		{
			// 終了ログ
			ApiLogger.Write(LogLevel.info, "コマンドビルダー終了:" + GetType().Name, "");
		}
		#endregion

		#region +Dispose
		/// <summary>
		/// Disposeの実装
		/// </summary>
		public void Dispose()
		{
			End();
		}
		#endregion

		#region #CheckApiData API生データチェック処理
		/// <summary>API生データチェック処理</summary>
		protected virtual void CheckApiData(IDataRecord record) { }
		#endregion

		#region #CheckRecordData Export実行後レコードチェック処理
		/// <summary>Export実行後レコードチェック処理</summary>
		protected virtual void CheckRecordData(object[] objects) { }
		#endregion

		/// <summary>コマンドライン引数指定のプロパティ </summary>
		public BatchProperties Properties { get; set; }
	}

	/// <summary>
	/// IDataRecordを配列に変換する拡張メソッド
	/// </summary>
	public static class IDataRecordExtension
	{
		public static object[] ToArray(this IDataRecord self)
		{
			object[] result = new object[self.FieldCount];
			self.GetValues(result);
			return result;
		}
	}
}
