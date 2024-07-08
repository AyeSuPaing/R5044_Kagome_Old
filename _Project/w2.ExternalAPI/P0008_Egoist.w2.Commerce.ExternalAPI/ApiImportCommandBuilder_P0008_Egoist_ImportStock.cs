/*
=========================================================================================================
  Module      : Egoist 在庫情報入力クラス(ApiImportCommandBuilder_P0008_Egoist_ImportStock.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.IO;
using System.Linq;
using w2.Common.Logger;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Ftp;
using w2.ExternalAPI.Common.Import;

namespace P0008_Egoist.w2.Commerce.ExternalAPI
{
	public class ApiImportCommandBuilder_P0008_Egoist_ImportStock : ApiImportCommandBuilder
	{
		#region #Import インポート処理の実装
		/// <summary>
		/// インポート処理の実装
		/// </summary>
		/// <param name="apiEntry"></param>
		protected override void Import(ApiEntry apiEntry)
		{
			// Get Stock
			int stock;
			if (Int32.TryParse((string)apiEntry.Data[13], out stock) == false) return;
			stock = stock / 100;

			// ■変換内容
			string productCd = (string)apiEntry.Data[8];
			string colorCd = (string)apiEntry.Data[10];
			string sizeCd = ((((string)apiEntry.Data[11]).LastOrDefault() == default(char)) ? string.Empty : ((string)apiEntry.Data[11]).Last().ToString());

			// Get Product
			string variationId = GetVariationId(productCd, colorCd, sizeCd);
			var product = GetProduct(variationId);

			if ((product == null) || ((string)product["VariationCooperationId5"] != string.Empty)) return;

			// 引数生成
			SetStockQuantityArg arg = new SetStockQuantityArg
			{
				ProductID = (string)product["productId"],
				VariationID = variationId,
				Stock = stock + Convert.ToInt32(product["stock"])
			};

			// コマンド実行
			SetStockQuantity cmd = new SetStockQuantity();
			SetStockQuantityResult result = (SetStockQuantityResult)cmd.Do(arg);
		}
		#endregion

		#region
		/// <summary>
		/// 商品取得
		/// </summary>
		/// <param name="variationId">バリエーションID</param>
		/// <returns>商品</returns>
		private DataRow GetProduct(string variationId)
		{
			if (Stocks == null)
			{
				// コマンド生成
				GetStockQuantitiesNow cmd = new GetStockQuantitiesNow();

				// 引数生成
				GetStockQuantitiesNowArg arg = new GetStockQuantitiesNowArg() { };

				// コマンド実行（全在庫情報取得）
				Stocks = ((GetStockQuantitiesNowResult)cmd.Do(arg)).ResultTable;
			}

			return Stocks.AsEnumerable().Where(stock => (string)stock["VariationId"] == variationId).FirstOrDefault();
		}

		/// <summary>
		/// Get the variation identifier.
		/// </summary>
		/// <param name="productCd">商品CD</param>
		/// <param name="colorCd">色CD</param>
		/// <param name="sizeCd">サイズCD</param>
		/// <returns>Variation Id</returns>
		private string GetVariationId(string productCd, string colorCd, string sizeCd)
		{
			if (productCd.Length == 8)
			{
				return string.Format("{0}{1}{2}", productCd, sizeCd, colorCd);
			}
			else if (productCd.Length == 12)
			{
				return string.Format("{0}{2}{1}", productCd, sizeCd, colorCd);
			}
			else
			{
				return productCd;
			}
		}
		#endregion

		#region #ParepareImportFile インポート対象ファイルの準備処理
		/// <summary>
		/// インポート対象ファイルの準備処理
		/// </summary>
		/// <param name="importFilepath">準備予定のファイルパス</param>
		public override void ParepareImportFile(string importFilepath)
		{
			var fluentFtpUtill = new FluentFtpUtility(
				ExternalAPI.Properties.Settings.Default.ftpHost,
				ExternalAPI.Properties.Settings.Default.ftpUserName,
				ExternalAPI.Properties.Settings.Default.ftpUserPassword,
				ExternalAPI.Properties.Settings.Default.ftpUseActive,
				ExternalAPI.Properties.Settings.Default.ftpEnableSsl);

			// ダウンロード先に指定したファイル名と同一のファイルを対象にFTPサーバからダウンロード
			var downloadSource = ExternalAPI.Properties.Settings.Default.ftpDownloadFileDirctory + new FileInfo(importFilepath).Name;
			if (fluentFtpUtill.Download(downloadSource, importFilepath))
			{
				FileLogger.WriteInfo("FTPダウンロードに成功しました。 ダウンロード元:" + downloadSource + " ダウンロード先: " + importFilepath);
			}
			else
			{
				FileLogger.WriteError("FTPダウンロードに失敗しました ダウンロード元:" + downloadSource + " ダウンロード先: " + importFilepath);
				throw new Exception("FTPダウンロードに失敗しました ダウンロード元:" + downloadSource + " ダウンロード先: " + importFilepath);
			}

			// ダウンロード先に対象ファイルが存在する場合のみ、ダウンロード元のファイルを削除
			if (File.Exists(importFilepath) && fluentFtpUtill.Delete(downloadSource))
			{
				FileLogger.WriteInfo("FTPダウンロードに成功したため、元ファイル(" + downloadSource + ")を削除しました ダウンロード先ファイル名 : " + importFilepath);
			}
			else
			{
				FileLogger.WriteError("FTPダウンロードに成功しましたが、元ファイル(" + downloadSource + ")を削除に失敗しました ダウンロード先ファイル名 : " + importFilepath);
				throw new Exception("FTPダウンロードに成功しましたが、元ファイル(" + downloadSource + ")を削除に失敗しました ダウンロード先ファイル名 : " + importFilepath);
			}
		}
		#endregion

		private DataTable Stocks { get; set; }
	}
}
