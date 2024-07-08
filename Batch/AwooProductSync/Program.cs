/*
=========================================================================================================
  Module      : Awoo商品連携バッチ(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using w2.App.Common;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order;
using w2.App.Common.SendMail;
using w2.App.Common.Util;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.Product;

namespace w2.Commerce.Batch.AwooProductSync
{
	/// <summary>
	/// プログラム本体
	/// </summary>
	public class Program
	{
		/// <summary>
		/// メイン処理
		/// </summary>
		/// <param name="args">引数</param>
		static void Main(string[] args)
		{
			try
			{
				var program = new Program();
				FileLogger.WriteInfo("起動");

				if (ProcessUtility.ExcecWithProcessMutex(program.Execute) == false)
				{
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}
				FileLogger.WriteInfo("終了");
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Program()
		{
			Initialize();
		}

		/// <summary>
		/// 設定初期化
		/// </summary>
		private void Initialize()
		{
			try
			{
				Constants.APPLICATION_NAME = Properties.Settings.Default.ApplicationName;

				var appSetting = new ConfigurationSetting(
					Properties.Settings.Default.ConfigFileDirPath,
					ConfigurationSetting.ReadKbn.C000_AppCommon,
					ConfigurationSetting.ReadKbn.C100_BatchCommon,
					ConfigurationSetting.ReadKbn.C200_AwooProductSync);

				Constants.PROTOCOL_HTTPS = appSetting.GetAppStringSetting("Site_ProtocolHttps");
				Constants.AWOO_PRODUCT_SYNC_FILE_PATH = appSetting.GetAppStringSetting("Product_Master_File_Path");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("設定ファイルの読み込みに失敗しました。\r\n" + ex);
			}
		}

		/// <summary>
		/// 処理実行
		/// </summary>
		private void Execute()
		{
			if (Constants.AWOO_OPTION_ENABLED == false) return;
			if (Constants.AWOO_PRODUCT_SYNC_FILE_PATH.EndsWith(".csv") == false)
			{
				NotifyError(MessageManager.GetMessages(CommerceMessages.ERRMSG_MANAGER_AWOO_PRODUCT_SYNC_FILE_PATH_NAME_INVALID));
				return;
			}

			try
			{
				var result = CreateProductCsv();
				NotifySuccess(result);
			}
			catch (Exception)
			{
				NotifyError(MessageManager.GetMessages(CommerceMessages.ERRMSG_MANAGER_AWOO_PRODUCT_SYNC_CREATE_DATE_FAILED));
			}
		}

		/// <summary>
		/// 商品連携情報のCSV出力
		/// </summary>
		/// <returns>出力商品点数</returns>
		private int CreateProductCsv()
		{
			try
			{
				// 商品連携情報取得
				var productList = new ProductService().GetProductForAwooProductSync();
				if (productList.Count == 0) return 0;

				// 商品CSVデータ作成
				var productCsv = new StringBuilder();
				productCsv.Append(CreateHeaderArea());
				foreach (DataRowView drv in productList)
				{
					productCsv.Append(CreateProductArea(new ProductData(drv)));
				}

				// CSVファイル生成
				CreateCsv(productCsv, Constants.AWOO_PRODUCT_SYNC_FILE_PATH);
				return productList.Count;
			}
			catch (Exception ex)
			{
				if (File.Exists(Constants.AWOO_PRODUCT_SYNC_FILE_PATH)) File.Delete(Constants.AWOO_PRODUCT_SYNC_FILE_PATH);
				AppLogger.WriteError(ex);
				return 0;
			}
		}

		/// <summary>
		/// CSVヘッダレコード作成
		/// </summary>
		/// <returns>CSVヘッダレコード</returns>
		private string CreateHeaderArea()
		{
			var dataColums = new[]
			{
				"id",
				"title",
				"description",
				"link",
				"image_link",
				"availability",
				"price",
				"sale_price",
				"brand",
				"product_type",
				"product_create_time",
				"favorite",
			};
			var result = StringUtility.CreateEscapedCsvString(dataColums) + "\r\n";
			return result;
		}

		/// <summary>
		/// CSV商品情報レコード作成
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <returns>CSV商品情報レコード作成</returns>
		private string CreateProductArea(ProductData product)
		{
			var description = Regex.Replace(
				product.Outline + product.DescDetail1 + product.DescDetail2 + product.DescDetail3 + product.DescDetail4,
				"<.*?>",
				string.Empty);

			var detailUrl = HttpUtility.UrlDecode(CreateProductDetailUrl(product));

			var imageUrl = CreateProductImageUrl(product);

			var categoryNameList = new[]
			{
				StringUtility.ToEmpty(product.CategoryName1),
				StringUtility.ToEmpty(product.CategoryName2),
				StringUtility.ToEmpty(product.CategoryName3),
				StringUtility.ToEmpty(product.CategoryName4),
				StringUtility.ToEmpty(product.CategoryName5),
			};
			var productType = string.Join(
				",",
				categoryNameList.Where(category => string.IsNullOrEmpty(category) == false));

			var salePrice = product.SalePrice ?? (product.DisplaySpecialPrice ?? product.DisplayPrice);

			// CSVレコード作成
			var dataColums = new[]
			{
				// id
				TrimStringDataByMaxLength(product.ProductId, Constants.AWOO_PRODUCT_SYNC_MAX_LENGTH_PRODUCT_ID),
				// title
				TrimStringDataByMaxLength(product.Name, Constants.AWOO_PRODUCT_SYNC_MAX_LENGTH_TITLE),
				// description
				TrimStringDataByMaxLength(description, Constants.AWOO_PRODUCT_SYNC_MAX_LENGTH_DESCRIPTION),
				// link
				TrimStringDataByMaxLength(detailUrl, Constants.AWOO_PRODUCT_SYNC_MAX_LENGTH_LINK),
				// image_link
				TrimStringDataByMaxLength(imageUrl, Constants.AWOO_PRODUCT_SYNC_MAX_LENGTH_IMAGE_LINK),
				// availability
				"in_stock",
				// price
				String.Format("{0} {1}", product.DisplayPrice.ToPriceString(), Constants.CONST_KEY_CURRENCY_CODE),
				// sale_price
				String.Format("{0} {1}", salePrice.ToPriceString(), Constants.CONST_KEY_CURRENCY_CODE),
				// brand
				TrimStringDataByMaxLength(StringUtility.ToEmpty(product.BrandName), Constants.AWOO_PRODUCT_SYNC_MAX_LENGTH_BRAND),
				// product_type
				TrimStringDataByMaxLength(productType, Constants.AWOO_PRODUCT_SYNC_MAX_LENGTH_PRODUCT_TYPE),
				// product_create_time
				product.SellFrom.ToString("yyyy-MM-ddTHH:mm:ssK"),
				// favorite
				product.FavoriteCount.ToString(),
			};
			var result = StringUtility.CreateEscapedCsvString(dataColums) + "\r\n";
			return result;
		}

		/// <summary>
		/// 商品詳細URL作成
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <returns>商品詳細URL</returns>
		private string CreateProductDetailUrl(ProductData product)
		{
			var url = ProductCommon.CreateProductDetailUrl(
				product,
				"",
				product.CategoryId1,
				product.BrandId,
				"",
				product.BrandName
			);
			var result = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC + url;
			return result;
		}

		/// <summary>
		/// 商品画像URL取得
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <returns>商品画像URL</returns>
		private string CreateProductImageUrl(ProductData product)
		{
			// ファイル名ヘッダ
			var imageFileNameHead = StringUtility.ToEmpty(product.ImageHead);
			var shopId = StringUtility.ToEmpty(product.ShopId);

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
						imageFileNameHead.Substring(
							0,
							imageFileNameHead.IndexOf(Uri.SchemeDelimiter, StringComparison.Ordinal)),
						"https")).Append(fileNameFoot);
			}
			else
			{
				// 画像URL
				imageUrl.Append(Constants.PATH_ROOT_FRONT_PC)
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
					imageUrl.Append(Constants.PATH_ROOT_FRONT_PC)
						.Append(Constants.PATH_PRODUCTIMAGES)
						.Append(Constants.PRODUCTIMAGE_NOIMAGE_HEADER)
						.Append(fileNameFoot);
				}
			}

			var result = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + imageUrl;
			return result;
		}

		/// <summary>
		/// CSVファイル生成
		/// </summary>
		/// <param name="data">CSVデータ</param>
		/// <param name="filePath">CSVファイル名</param>
		private void CreateCsv(StringBuilder data, string filePath)
		{
			// 作業ディレクトリ作成
			var directoryPath = Path.GetDirectoryName(filePath);
			if ((Directory.Exists(directoryPath) == false) && (directoryPath != null))
			{
				Directory.CreateDirectory(directoryPath);
			}

			if (File.Exists(filePath)) File.Delete(filePath);

			using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
			using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
			{
				streamWriter.Write(data.ToString());
			}
		}

		/// <summary>
		/// 文字データを最大文字数以下に整形
		/// </summary>
		/// <param name="str">文字データ</param>
		/// <param name="maxLength">最大文字数</param>
		private string TrimStringDataByMaxLength(string str, int maxLength)
		{
			if (string.IsNullOrEmpty(str)) return str;

			// 全角は2文字判定とするため、Shift_JISエンコードしてバイト数で計算する
			var enc = Encoding.GetEncoding("Shift_JIS");
			if (enc.GetByteCount(str) <= maxLength) return str;

			// 指定バイトと、指定バイト+1分の文字列を取得して比較する
			var data = enc.GetBytes(str);
			var result = enc.GetString(data, 0, maxLength);
			var result2 = enc.GetString(data, 0, maxLength + 1);

			// 指定文字数から1バイトずらして桁が同じならば、全角の途中で終わっているので-1バイトした文字を返す
			if (result.Length == result2.Length)
			{
				return result.Remove(result.Length - 1);
			}
			// 桁が増えるならば、半角終わりなのでそのまま返す
			return result;
		}

		/// <summary>
		/// 結果通知(Error)
		/// </summary>
		/// <param name="message">メッセージ</param>
		private void NotifyError(string message)
		{
			var data = new Hashtable {{"error_message", message}};
			SendMailCommon.SendMailToOperator(Constants.CONST_MAIL_ID_AWOO_PRODUCT_SYNC_FAILED, data);
		}

		/// <summary>
		/// 結果通知(Success)
		/// </summary>
		/// <param name="exportItemCount">出力件数</param>
		private void NotifySuccess(int exportItemCount)
		{
			var data = new Hashtable { { "item_count", exportItemCount } };
			SendMailCommon.SendMailToOperator(Constants.CONST_MAIL_ID_AWOO_PRODUCT_SYNC_SUCCESS, data);
		}
	}
}
