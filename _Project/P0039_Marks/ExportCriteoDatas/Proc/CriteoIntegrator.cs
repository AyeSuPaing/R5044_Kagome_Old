/*
=========================================================================================================
  Module      : Criteo連携(CriteoIntegrator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using w2.Common.Logger;
using w2.Common.Sql;

namespace w2.Commerce.Batch.ExportCriteoDatas.Proc
{
	/// <summary>
	/// Criteo連携
	/// </summary>
	public class CriteoIntegrator
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="setting">連携用サイト設定</param>
		public CriteoIntegrator(CriteoIntegrationSetting setting)
		{
			this.Setting = setting;
			this.NumberOf = 0;
		}

		/// <summary>
		/// 連携データ出力処理
		/// </summary>
		public void Export()
		{
			try
			{
				// 出力処理開始
				using (CriteoFileExporter criteoFileExporter = new CriteoFileExporter(this.Setting.setting.ExportPath))
				{
					var products = GetCriteoProducts();

					foreach (var product in products)
					{
						// カウンティング
						this.NumberOf++;
						this.LastProduct = product;

						var criteoModel = CreateCriteoModel(product, this.Setting.setting);

						// 出力処理
						criteoFileExporter.Write(criteoModel);
					}
				}
			}
			catch (IOException ex)
			{
				string message = "ファイル作成時にエラーが発生しました。" + ex.Message;
				throw new Exception(message, ex);
			}
			catch (Exception ex)
			{
				// エラーの場合はファイル削除
				DeleteFile(this.Setting.setting.ExportPath);

				string message;
				if (this.LastProduct == null)
				{
					message = "Criteo連携用ファイル出力開始前に予期せぬエラーが発生しました。";
					FileLogger.WriteError(message, ex);
					throw;
				}
				else
				{
					message = string.Format(
@"{0}件目の出力時にエラーが発生しました。\r\n
該当の商品IDは '{1}' です。
詳細な行の情報は以下
- {2}",
						this.NumberOf,
						this.LastProduct.ProductId,
						HashtableToJSON(this.LastProduct.DataSource));

					FileLogger.WriteError(message, ex);
					throw new CriteoExportException(message, ex);
				}
			}
		}

		/// <summary>
		/// Criteoモデル生成
		/// </summary>
		/// <param name="product">商品モデル</param>
		/// <param name="setting">Criteo連携用サイト設定</param>
		/// <returns>Criteoモデル</returns>
		private CriteoModel CreateCriteoModel(ProductModel product, CriteoSiteSetting setting)
		{
			var creator = new CriteoModelCreator();
			return creator.CreateCriteoModel(product, setting);
		}

		/// <summary>
		/// HashtableをJSONへ変換する
		/// </summary>
		/// <param name="source">Hashtable</param>
		/// <returns>JSON</returns>
		private string HashtableToJSON(Hashtable source)
		{
			List<string> objectList = new List<string>();
			foreach (object key in source.Keys)
				objectList.Add(string.Format("\"{0}\":\"{1}\"", (string)key, source[key].ToString()));

			return string.Format("{{0}}", string.Join(",", objectList.ToArray()));
		}

		/// <summary>
		/// Criteo連携用の商品情報を取得する
		/// </summary>
		/// <returns>Criteoモデルの列挙</returns>
		public IEnumerable<ProductModel> GetCriteoProducts()
		{
			var data = GetCriteoProductData();
			foreach (Hashtable item in data)
			{
				yield return new ProductModel(item);
			}
		}

		/// <summary>
		/// Criteo連携用の商品情報を取得する
		/// </summary>
		/// <returns>Criteo連携用の商品情報の列挙</returns>
		private IEnumerable<Hashtable> GetCriteoProductData()
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement("Product", "GetCriteoProducts"))
			{
				accessor.OpenConnection();

				using (SqlStatementDataReader dataReader = new SqlStatementDataReader(accessor, statement))
				{
					while (dataReader.Read())
					{
						Hashtable row = new Hashtable();
						var range = Enumerable.Range(0, dataReader.FieldCount);

						foreach (int i in range)
						{
							row.Add(dataReader.GetName(i), dataReader[i]);
						}

						yield return row;
					}
				}
			}
		}

		/// <summary>
		/// ファイル削除
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		private void DeleteFile(string filePath)
		{
			FileInfo fileInfo = new FileInfo(filePath);

			if (fileInfo.Exists)
			{
				// ファイルを削除する
				fileInfo.Delete();
			}
		}

		/// <summary>出力先ファイルパス</summary>
		private CriteoIntegrationSetting Setting;
		/// <summary>出力件数</summary>
		public int NumberOf { get; private set; }
		/// <summary>最後に処理した商品データ</summary>
		private ProductModel LastProduct;
	}
}
