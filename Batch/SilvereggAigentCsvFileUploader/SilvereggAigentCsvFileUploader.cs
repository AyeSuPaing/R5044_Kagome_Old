/*
=========================================================================================================
  Module      : シルバーエッグアイジェント商品データ連携バッチ(SilvereggAigentCsvFileUploader.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using w2.App.Common;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Util.Archiver;

namespace w2.Commerce.Batch.SilvereggAigentCsvFileUploader
{
	/// <summary>
	/// メイン処理クラス
	/// </summary>
	class SilvereggAigentCsvFileUploader
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SilvereggAigentCsvFileUploader()
		{
			Iniitialize();
		}

		/// <summary>
		/// 設定初期化
		/// </summary>
		private void Iniitialize()
		{
			Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;
			Constants.CSV_ENCODE = Properties.Settings.Default.CsvEncode;

			var csSetting = new ConfigurationSetting(
				Properties.Settings.Default.ConfigFileDirPath,
				ConfigurationSetting.ReadKbn.C000_AppCommon,
				ConfigurationSetting.ReadKbn.C100_BatchCommon,
				ConfigurationSetting.ReadKbn.C200_SilvereggAigentCsvFileUploader);

			// WEBサイトパスルート
			Constants.PATH_ROOT = Constants.PATH_ROOT_FRONT_PC = csSetting.GetAppStringSetting("Site_RootPath_PCFront");
			// WEBサイトHTTPSプロトコル
			Constants.PROTOCOL_HTTPS = csSetting.GetAppStringSetting("Site_ProtocolHttps");
			// CSVファイル格納ディレクトリ
			Constants.PHYSICALDIRPATH_CSV_FILES = csSetting.GetAppStringSetting("Directory_SilvereggAigentCsvFileUploader_CsvFilePath");
			// 作業ディレクトリ
			Constants.PHYSICALDIRPATH_TEMP = Path.Combine(Constants.PHYSICALDIRPATH_CSV_FILES, DateTime.Today.ToString("yyyyMMdd"));
			// 商品マスタCSVファイル名
			Constants.CSV_FILENAME_PRODUCT = "product_" + Constants.RECOMMEND_SILVEREGG_MERCHANT_ID + ".csv";
			// カテゴリマスタCSVファイル名
			Constants.CSV_FILENAME_CATEGORY = "category_" + Constants.RECOMMEND_SILVEREGG_MERCHANT_ID + ".csv";
			// 商品マスタCSVファイル名
			Constants.CSV_FILENAME_PRODUCT_CAT = "product_cat_" + Constants.RECOMMEND_SILVEREGG_MERCHANT_ID + ".csv";
			// FTP送信先ファイルパス
			Constants.DESTINATION_FILE_PATH = csSetting.GetAppStringSetting("Recommend_Silveregg_Destination_File_Path");

			// バッチ実行結果
			this.IsBatchSuccess = true;
		}

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		static void Main()
		{
			try
			{
				// インスタンス作成
				var uploader = new SilvereggAigentCsvFileUploader();

				// バッチ起動をイベントログ出力
				AppLogger.WriteInfo("起動");

				// レコメンド対象商品をCSV出力
				uploader.CreateCsv();

				// シルバーエッグ社FTPサーバへファイルアップロード
				uploader.UploadCsv();

				// 取込完了後のCSVファイルを圧縮して残す
				uploader.ZipArchiveCsv();

				// 取込結果をメール送信
				SendBatchEndMail(uploader.IsBatchSuccess);

				// バッチ終了をイベントログ出力
				AppLogger.WriteInfo((uploader.IsBatchSuccess) ? "正常終了" : "異常終了");
			}
			catch (Exception ex)
			{
				// エラーイベントログ出力
				AppLogger.WriteError(ex);
				// 取込結果をメール送信
				SendBatchEndMail(false);
			}
		}

		/// <summary>
		/// レコメンド対象商品をCSV出力
		/// </summary>
		private void CreateCsv()
		{
			// 作業ディレクトリ作成
			if (Directory.Exists(Constants.PHYSICALDIRPATH_TEMP) == false) Directory.CreateDirectory(Constants.PHYSICALDIRPATH_TEMP);

			// 商品マスタと商品／カテゴリ紐づけマスタのCSV出力
			CreateProductCsv();

			// 商品カテゴリマスタのCSV出力
			CreateCategoryCsv();
		}

		/// <summary>
		/// シルバーエッグ社FTPサーバへファイルアップロード
		/// </summary>
		private void UploadCsv()
		{
			var fluentFtpUtility = new w2.ExternalAPI.Common.Ftp.FluentFtpUtility
			(
				Constants.RECOMMEND_SILVEREGG_FTP_HOST,
				Constants.RECOMMEND_SILVEREGG_FTP_ID,
				Constants.RECOMMEND_SILVEREGG_FTP_PW,
				false,
				false
			);

			// ３ファイルをアップロード
			this.IsBatchSuccess = false;
			if (fluentFtpUtility.Upload(
				Path.Combine(Constants.PHYSICALDIRPATH_TEMP, Constants.CSV_FILENAME_PRODUCT),
				Path.Combine(Constants.DESTINATION_FILE_PATH, Constants.CSV_FILENAME_PRODUCT)))
			{
				if (fluentFtpUtility.Upload(
					Path.Combine(Constants.PHYSICALDIRPATH_TEMP, Constants.CSV_FILENAME_CATEGORY),
					Path.Combine(Constants.DESTINATION_FILE_PATH, Constants.CSV_FILENAME_CATEGORY)))
				{
					if (fluentFtpUtility.Upload(
						Path.Combine(Constants.PHYSICALDIRPATH_TEMP, Constants.CSV_FILENAME_PRODUCT_CAT),
						Path.Combine(Constants.DESTINATION_FILE_PATH, Constants.CSV_FILENAME_PRODUCT_CAT)))
					{
						this.IsBatchSuccess = true;
					}
				}
			}
		}

		/// <summary>
		/// 取込完了後のCSVファイルを圧縮して残す
		/// </summary>
		private void ZipArchiveCsv()
		{
			var targetDirPaths = Constants.PHYSICALDIRPATH_TEMP;
			var targetRootPath = Constants.PHYSICALDIRPATH_CSV_FILES;
			var zipFilePath = Path.Combine(Constants.PHYSICALDIRPATH_CSV_FILES, DateTime.Today.ToString("yyyyMMdd")) + ".zip";
			new ZipArchiver().CompressFile(targetDirPaths, targetRootPath, zipFilePath);

			// 圧縮元のディレクトリは削除
			Directory.Delete(Constants.PHYSICALDIRPATH_TEMP, true);
		}

		/// <summary>
		/// 商品マスタと商品／カテゴリ紐づけマスタのCSV出力
		/// </summary>
		private void CreateProductCsv()
		{
			try
			{
				// 商品マスタ取得
				var recommendItem = GetRecommendItem();
				if (recommendItem.Count == 0) return;

				// 商品CSV出力
				var productCsv = new StringBuilder();
				var productAndCategoryCsv = new StringBuilder();
				foreach (DataRowView drv in recommendItem)
				{
					// 連携用商品IDの採番
					if (string.IsNullOrEmpty((string)drv[Constants.FIELD_PRODUCT_RECOMMEND_PRODUCT_ID]))
					{
						drv[Constants.FIELD_PRODUCT_RECOMMEND_PRODUCT_ID] = CreateRecommendProductId((string)drv[Constants.FIELD_PRODUCT_PRODUCT_ID]);
					}

					// 商品データ出力
					productCsv.Append(CreateProductArea(drv));

					// 商品／カテゴリ紐づけデータ出力
					productAndCategoryCsv.Append(CreateProductAndCategoryArea(drv));
				}

				// CSVファイル生成
				CreateCsv(productCsv, Constants.CSV_FILENAME_PRODUCT);
				CreateCsv(productAndCategoryCsv, Constants.CSV_FILENAME_PRODUCT_CAT);
			}
			catch (Exception ex)
			{
				// エラーイベントログ出力
				AppLogger.WriteError(ex);
				// バッチ実行エラー
				this.IsBatchSuccess = false;
			}
		}

		/// <summary>
		/// 商品カテゴリマスタCSV出力
		/// </summary>
		private void CreateCategoryCsv()
		{
			try
			{
				// 商品カテゴリマスタ取得
				var recommendCategory = GetRecommendCategory();
				if (recommendCategory.Count == 0) return;

				// 商品カテゴリCSV出力
				var categoryCsv = new StringBuilder();
				foreach (DataRowView drv in recommendCategory)
				{
					// カテゴリデータ出力
					categoryCsv.Append(CreateCategoryArea(drv));
				}

				// CSVファイル生成
				CreateCsv(categoryCsv, Constants.CSV_FILENAME_CATEGORY);
			}
			catch (Exception ex)
			{
				// エラーイベントログ出力
				AppLogger.WriteError(ex);
				// バッチ実行エラー
				this.IsBatchSuccess = false;
			}
		}

		/// <summary>
		/// 商品マスタ取得
		/// </summary>
		/// <returns>レコメンド対象商品</returns>
		private DataView GetRecommendItem()
		{
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("Product", "GetRecommendProduct"))
			{
				var ht = new Hashtable
				{
					{Constants.FIELD_PRODUCT_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID}
				};
				var dv = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, ht);
				return dv;
			}
		}

		/// <summary>
		/// 商品カテゴリマスタ取得
		/// </summary>
		/// <returns>レコメンド対象の商品カテゴリ</returns>
		private DataView GetRecommendCategory()
		{
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("Product", "GetRecommendCategory"))
			{
				var ht = new Hashtable
				{
					{Constants.FIELD_PRODUCTCATEGORY_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID}
				};
				var dv = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, ht);
				return dv;
			}
		}

		/// <summary>
		/// CSVレコード編集（レコメンド対象商品）
		/// </summary>
		/// <param name="drv">レコメンド対象商品</param>
		/// <returns>レコメンド対象商品CSVレコード</returns>
		private string CreateProductArea(DataRowView drv)
		{
			// 価格の編集
			var price = ((string.IsNullOrEmpty(drv[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE].ToString())) 
					? drv[Constants.FIELD_PRODUCT_DISPLAY_PRICE] 
					: drv[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE]).ToPriceString();
			var displayPrice = String.Format("￥{0:#,0}（{1}）", price, TaxCalculationUtility.GetTaxTypeText());

			// 画像URLの編集
			var imageUrl = CreateProductImageUrl(drv);

			// リンク先URLの編集(デコードしないとシルバーエッグ側の文字数チェック引っかかる）
			var detailUrl = HttpUtility.UrlDecode(CreateProductDetailUrl(drv));

			// CSVレコード作成
			var dataColums = new[]
			{
				// アイテムID
				(string)drv[Constants.FIELD_PRODUCT_RECOMMEND_PRODUCT_ID],
				// アイテム名
				(string)drv[Constants.FIELD_PRODUCT_NAME],
				// アイテム説明
				(string)drv[Constants.FIELD_PRODUCT_CATCHCOPY],
				// 表示用価格
				displayPrice,
				// 画像URL PC用
				imageUrl,
				// リンク先URL PC用
				detailUrl,
				// おすすめ可能フラグ
				"1",
				// フィルタ用価格
				price,
				// 画像URL SP用
				imageUrl,
				// リンク先URL SP用
				detailUrl,
				// 住所（未使用項目）
				"",
				// 予備1（未使用項目）
				"",
				// 予備2（未使用項目）
				"",
				// 予備3（未使用項目）
				""
			};
			var result = StringUtility.CreateEscapedCsvString(dataColums) + Constants.CR_LF_CODE;
			return result;
		}

		/// <summary>
		/// CSVレコード編集（レコメンド対象商品カテゴリ）
		/// </summary>
		/// <param name="drv">レコメンド対象商品カテゴリ</param>
		/// <returns>レコメンド対象商品カテゴリCSVレコード</returns>
		private string CreateCategoryArea(DataRowView drv)
		{
			// 階層レベル算出
			var level = drv[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID].ToString().Length / 3;

			// 階層レベルは最大9まで
			if (level <= 9)
			{
				// CSVレコード作成
				var dataColums = new[]
				{
					// カテゴリID
					(string)drv[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID],
					// カテゴリ名
					(string)drv[Constants.FIELD_PRODUCTCATEGORY_NAME],
					// 親カテゴリID
					(level == 1) ? "" : (string)drv[Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID],
					// 階層レベル
					level.ToString()
				};
				var result = StringUtility.CreateEscapedCsvString(dataColums) + Constants.CR_LF_CODE;
				return result;
			}
			return "";
		}

		/// <summary>
		/// CSVレコード編集（アイテム/カテゴリ紐づけ）
		/// </summary>
		/// <param name="drv">レコメンド対象商品</param>
		/// <returns>アイテム/カテゴリ紐づけCSVレコード</returns>
		private string CreateProductAndCategoryArea(DataRowView drv)
		{
			if (string.IsNullOrEmpty((string)drv[Constants.FIELD_PRODUCT_CATEGORY_ID1]) == false)
			{
				// CSVレコード作成
				var dataColums = new[]
				{
					// アイテムID
					(string)drv[Constants.FIELD_PRODUCT_RECOMMEND_PRODUCT_ID],
					// 最下層カテゴリID
					(string)drv[Constants.FIELD_PRODUCT_CATEGORY_ID1]
				};
				var result = StringUtility.CreateEscapedCsvString(dataColums) + Constants.CR_LF_CODE;
				return result;
			}
			return "";
		}

		/// <summary>
		/// CSVファイル生成
		/// </summary>
		/// <param name="data">CSVデータ</param>
		/// <param name="fileName">CSVファイル名</param>
		private void CreateCsv(StringBuilder data, string fileName)
		{
			// CSVファイルパス
			var csvFilePath = Path.Combine(Constants.PHYSICALDIRPATH_TEMP, fileName);

			// CSVファイルを一応削除
			File.Delete(csvFilePath);

			using (var fileStream = new FileStream(csvFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
			using (var streamWriter = new StreamWriter(fileStream, Encoding.GetEncoding(Constants.CSV_ENCODE)))
			{
				streamWriter.Write(data.ToString());
			}
		}

		/// <summary>
		/// 連携用商品IDの採番
		/// </summary>
		/// <param name="product_id">商品ID</param>
		/// <returns>新しく採番された連携用の商品ID</returns>
		private string CreateRecommendProductId(string productId)
		{
			var recommendProductId = NumberingUtility.CreateNewNumber(Constants.CONST_DEFAULT_SHOP_ID, Constants.NUMBER_KEY_RECOMMEND_PRODUCT_ID);

			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("Product", "UpdateRecommendProductId"))
			{
				var ht = new Hashtable
				{
					{Constants.FIELD_PRODUCT_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID},
					{Constants.FIELD_PRODUCT_PRODUCT_ID, productId},
					{Constants.FIELD_PRODUCT_RECOMMEND_PRODUCT_ID, recommendProductId}
				};
				sqlStatement.ExecStatementWithOC(sqlAccessor, ht);
			}

			return recommendProductId.ToString();
		}

		/// <summary>
		/// 商品詳細URL作成
		/// </summary>
		/// <param name="drv">商品情報</param>
		/// <returns>商品詳細URL</returns>
		private string CreateProductDetailUrl(DataRowView drv)
		{
			var url = ProductCommon.CreateProductDetailUrl(
				drv,
				"",
				(string)drv[Constants.FIELD_PRODUCT_CATEGORY_ID1],
				(string)drv[Constants.FIELD_PRODUCT_BRAND_ID1],
				"",
				GetProductBrandName((string)drv[Constants.FIELD_PRODUCT_BRAND_ID1])
			);
			var result = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + url;
			return result;
		}

		/// <summary>
		/// ブランド名取得
		/// </summary>
		/// <param name="brand_id">ブランドID</param>
		/// <returns>ブランド名</returns>
		private string GetProductBrandName(string brandId)
		{
			// 商品ブランドマスタ取得
			DataView productBrand = null;
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("Product", "GetProductBrand"))
			{
				var ht = new Hashtable
				{
					{Constants.FIELD_PRODUCTBRAND_BRAND_ID, brandId}
				};
				productBrand = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, ht);
			}

			var bandName = (productBrand.Count > 0) ? productBrand[0][Constants.FIELD_PRODUCTBRAND_BRAND_NAME].ToString() : "";
			return bandName;
		}

		/// <summary>
		/// 商品画像URL取得
		/// </summary>
		/// <param name="objValue">商品情報</param>
		/// <returns>商品画像URL</returns>
		private string CreateProductImageUrl(DataRowView drv)
		{
			// ファイル名ヘッダ
			var imageFileNameHead = (string)drv[Constants.FIELD_PRODUCT_IMAGE_HEAD];

			// ファイル名フッタ(Mサイズ固定)
			var fileNameFoot = Constants.PRODUCTIMAGE_FOOTER_M;

			// 画像URL作成
			var imageUrl = new StringBuilder();
			var imagePath = new StringBuilder();
			if (imageFileNameHead.Contains(Uri.SchemeDelimiter))
			{
				// 外部画像URLの場合はスキーマをリプレース
				imageUrl.Append(
					imageFileNameHead.Replace(
						imageFileNameHead.Substring(0, imageFileNameHead.IndexOf(Uri.SchemeDelimiter)),
						"https")).Append(fileNameFoot);
			}
			else
			{
				var shopId = (string)drv[Constants.FIELD_PRODUCT_SHOP_ID];

				// 画像URL
				imageUrl.Append(Constants.PATH_ROOT)
					.Append(Constants.PATH_PRODUCTIMAGES)
					.Append(shopId).Append("/")
					.Append(imageFileNameHead)
					.Append(fileNameFoot);

				// 画像物理パス
				imagePath.Append(Constants.PHYSICALDIRPATH_FRONT_PC)
					.Append(Constants.PATH_PRODUCTIMAGES)
					.Append(shopId).Append("/")
					.Append(imageFileNameHead)
					.Append(fileNameFoot);

				// 画像の存在チェック
				if (File.Exists(imagePath.ToString()) == false)
				{
					// 画像無しの場合はNOIMAGE画像
					imageUrl = new StringBuilder();
					imageUrl.Append(Constants.PATH_ROOT)
						.Append(Constants.PATH_PRODUCTIMAGES)
						.Append(Constants.PRODUCTIMAGE_NOIMAGE_HEADER)
						.Append(fileNameFoot);
				}
			}

			var result = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + imageUrl.ToString();
			return result;
		}

		/// <summary>
		/// バッチ終了メール送信
		/// </summary>
		/// <param name="result">バッチの実行結果</param>
		private static void SendBatchEndMail(bool result)
		{
			var ht = new Hashtable()
			{
				{"result", (result) ? "成功" : "失敗"}
			};

			using (var mail = new MailSendUtility(
					Constants.CONST_DEFAULT_SHOP_ID,
					Constants.CONST_MAIL_ID_SILVEREGGAIGENT_FILEUPLOADER_FOR_OPERATOR,
					string.Empty,
					ht))
			{
				if (mail.SendMail() == false)
				{
					FileLogger.WriteError(mail.MailSendException);
				}
			}
		}

		/// <summary> バッチ実行結果 </summary>
		private bool IsBatchSuccess { get; set; }
	}
}
