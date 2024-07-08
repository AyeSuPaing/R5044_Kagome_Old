/*
=========================================================================================================
  Module      : 商品画像情報参照クラス(ProductImage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Net;
using w2.Common.Logger;
using w2.App.Common.Order;
using w2.Common.Web;
using System.Drawing.Imaging;
using w2.Common.Util;
using System.Drawing;
/*
| PC          | Mobile
--------------------------------------
Product   | main | o(S/M/L/LL) | o -
| sub  | o( /M/L/LL) | o -
--------------------------------------
Variation | main | o(S/M/L/LL) | o -
| sub  | -           | - 
--------------------------------------
注1 モバイル画像が存在しない場合はPC画像を参照する
注2 カラム（モバイル画像名）に値が無い場合もPC画像を参照する
*/
namespace w2.App.Common.Product
{
	/// <summary>商品画像タイプ</summary>
	public enum ImageType { Normal, Sub }
	/// <summary>サイトタイプ</summary>
	public enum SiteType { Pc, Mobile }
	/// <summary>商品タイプ</summary>
	public enum ProductType { Product, Variation }

	public class ProductImage
	{
		private const int IMAGE_SIZE_MAX = 1000;

		/// <summary>
		/// 画像ファイル名を取得
		/// </summary>
		/// <param name="info">商品またはバリエーション情報</param>
		/// <param name="productType">商品タイプ</param>
		/// <param name="siteType">サイトタイプ</param>
		/// <param name="footerSizeType">フッター</param>
		/// <param name="imageType">商品画像タイプ</param>
		/// <param name="subNo">サブ画像No</param>
		/// <returns>商品画像名（拡張子付）</returns>
		public static string GetImageFileName(object info, ProductType productType, SiteType siteType, string footerSizeType = "", ImageType imageType = ImageType.Normal, int subNo = 0)
		{
			var result = "";
			if (imageType == ImageType.Normal)
			{
				if (siteType == SiteType.Pc)
				{
					if (productType == ProductType.Product)
					{
						// PC商品メイン
						result = (string)ProductCommon.GetKeyValue(info, Constants.FIELD_PRODUCT_IMAGE_HEAD);
					}
					else
					{
						// PC商品バリエーション

						result = (string)ProductCommon.GetKeyValue(info, Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD);
					}
					result += footerSizeType;
				}
				else
				{
					if (productType == ProductType.Product)
					{
						// モバイル商品メイン
						result = (string)ProductCommon.GetKeyValue(info, Constants.FIELD_PRODUCT_IMAGE_MOBILE);
					}
					else
					{
						// モバイル商品バリエーション
						result = (string)ProductCommon.GetKeyValue(info, Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_MOBILE);
					}
				}
			}
			else
			{
				if (siteType == SiteType.Pc)
				{
					// PCサブ画像
					result = (string)ProductCommon.GetKeyValue(info, Constants.FIELD_PRODUCT_IMAGE_HEAD) + Constants.PRODUCTSUBIMAGE_FOOTER + subNo.ToString(Constants.PRODUCTSUBIMAGE_NUMBERFORMAT) + footerSizeType;
				}
				else
				{
					// モバイルサブ画像 ※モバイルは拡張子含めて商品登録するため、文字列を分ける
					string imageMobile = (string)ProductCommon.GetKeyValue(info, Constants.FIELD_PRODUCT_IMAGE_MOBILE);
					int iPos = imageMobile.LastIndexOf(".");
					var header = ((iPos != -1) ? imageMobile.Substring(0, iPos) : imageMobile);
					var extension = ((iPos != -1) ? imageMobile.Substring(iPos) : "");

					result = header + Constants.PRODUCTSUBIMAGE_FOOTER + subNo.ToString(Constants.PRODUCTSUBIMAGE_NUMBERFORMAT) + extension;
				}
			}

			return result;
		}
		/// <summary>
		/// 画像ファイル名を取得（モバイル）
		/// </summary>
		/// <param name="info">商品またはバリエーション情報</param>
		/// <param name="productType">商品タイプ</param>
		/// <param name="footerSizeType">フッター</param>
		/// <param name="imageType">商品画像タイプ</param>
		/// <param name="subNo">サブ画像No</param>
		/// <returns>商品画像名（拡張子付）</returns>
		/// <remarks>モバイル用にPC/モバイルどちらを参照するかに利用</remarks>
		public static string GetImageFileName(object info, ProductType productType, string footerSizeType = "", ImageType imageType = ImageType.Normal, int subNo = 0)
		{
			bool mobileEnable = GetMobileImageEnable(info, productType, footerSizeType, imageType, subNo);

			return GetImageFileName(
				info,
				productType,
				mobileEnable ? SiteType.Mobile : SiteType.Pc,
				mobileEnable ? "" : footerSizeType,
				imageType,
				subNo);
		}
		/// <summary>
		/// 商品またはバリエーション画像のHTMLイメージタグ作成
		/// </summary>
		/// <param name="info">商品またはバリエーション情報</param>
		/// <param name="productType">商品タイプ</param>
		/// <param name="siteType">サイトタイプ</param>
		/// <param name="footerSizeType">フッター</param>
		/// <param name="imageType">商品画像タイプ</param>
		/// <param name="subNo">サブ画像No</param>
		/// <param name="size">サイズ</param>
		/// <param name="isNowPrinting">NowPrintingを使うか</param>
		/// <returns>IMGタグ</returns>
		public static string GetHtmlImageTag(object info, ProductType productType, SiteType siteType, string footerSizeType = "", ImageType imageType = ImageType.Normal, int subNo = 0, int size = 80, bool isNowPrinting = true)
		{
			return CreateHtmlImageTag(
				GetImageFileName(info, productType, siteType, footerSizeType, imageType, subNo),
				(string)ProductCommon.GetKeyValue(info, Constants.FIELD_PRODUCT_SHOP_ID),
				siteType,
				GetImageDirectoryPath(siteType, imageType),
				Constants.KBN_IMAGEFORMAT_JPG,
				size,
				isNowPrinting);
		}
		/// <summary>
		/// 商品またはバリエーション画像のHTMLイメージタグ作成（モバイル）
		/// </summary>
		/// <param name="info">商品またはバリエーション情報</param>
		/// <param name="productType">商品タイプ</param>
		/// <param name="footerSizeType">フッター</param>
		/// <param name="imageType">商品画像タイプ</param>
		/// <param name="subNo">サブ画像No</param>
		/// <param name="size">サイズ</param>
		/// <param name="isNowPrinting">NowPrintingを使うか</param>
		/// <remarks>モバイル用にPC/モバイルどちらを参照するかに利用</remarks>
		/// <returns>IMGタグ</returns>
		public static string GetHtmlImageTag(object info, ProductType productType, string footerSizeType = "", ImageType imageType = ImageType.Normal, int subNo = 0, int size = 80, bool isNowPrinting = true)
		{
			bool mobileEnable = GetMobileImageEnable(info, productType, footerSizeType, imageType, subNo);
			SiteType siteType = mobileEnable ? SiteType.Mobile : SiteType.Pc;

			return CreateHtmlImageTag(
				GetImageFileName(info, productType, siteType, footerSizeType, imageType, subNo),
				(string)ProductCommon.GetKeyValue(info, Constants.FIELD_PRODUCT_SHOP_ID),
				siteType,
				GetImageDirectoryPath(siteType, imageType),
				Constants.KBN_IMAGEFORMAT_JPG,
				size,
				isNowPrinting);
		}
		/// <summary>
		/// モバイル商品画像ファイルを参照するか否か
		/// </summary>
		/// <param name="info">商品またはバリエーション情報</param>
		/// <param name="productType">商品タイプ</param>
		/// <param name="footerSizeType">フッター</param>
		/// <param name="imageType">商品画像タイプ</param>
		/// <param name="subNo">サブ画像No</param>
		/// <returns>PCサイトにしか存在しない画像の場合PC商品画像ファイル参照</returns>
		public static bool GetMobileImageEnable(object info, ProductType productType, string footerSizeType = "", ImageType imageType = ImageType.Normal, int subNo = 0)
		{
			bool hasMobileImage = CheckImageFileExsits(info, productType, SiteType.Mobile, "", imageType, subNo);
			bool hasPcImage = CheckImageFileExsits(info, productType, SiteType.Pc, footerSizeType, imageType, subNo);
			return (((hasMobileImage == false) && (hasPcImage)) == false);
		}
		/// <summary>
		/// 画像ファイルが存在するかチェック
		/// </summary>
		/// <param name="info">商品またはバリエーション情報</param>
		/// <param name="productType">商品タイプ</param>
		/// <param name="siteType">サイトタイプ</param>
		/// <param name="footerSizeType">フッター</param>
		/// <param name="imageType">商品画像タイプ</param>
		/// <param name="subNo">サブ画像No</param>
		/// <returns>画像存在有無</returns>
		public static bool CheckImageFileExsits(object info, ProductType productType, SiteType siteType, string footerSizeType = "", ImageType imageType = ImageType.Normal, int subNo = 0)
		{
			return CheckImageFileExsits(
				siteType,
				GetImageFileName(info, productType, siteType, footerSizeType, imageType, subNo),
				(string)ProductCommon.GetKeyValue(info, Constants.FIELD_PRODUCT_SHOP_ID),
				GetImageDirectoryPath(siteType, imageType));
		}
		/// <summary>
		/// 画像ファイルが存在するかチェック（モバイル）
		/// </summary>
		/// <param name="info">商品またはバリエーション情報</param>
		/// <param name="productType">商品タイプ</param>
		/// <param name="footerSizeType">フッター</param>
		/// <param name="imageType">商品画像タイプ</param>
		/// <param name="subNo">サブ画像No</param>
		/// <returns>画像存在有無</returns>
		/// <remarks>モバイル用にPC/モバイルどちらを参照するかに利用</remarks>
		public static bool CheckImageFileExsits(object info, ProductType productType, string footerSizeType = "", ImageType imageType = ImageType.Normal, int subNo = 0)
		{
			bool mobileEnable = GetMobileImageEnable(info, productType, footerSizeType, imageType, subNo);
			SiteType siteType = mobileEnable ? SiteType.Mobile : SiteType.Pc;

			return CheckImageFileExsits(
				siteType,
				GetImageFileName(info, productType, siteType, footerSizeType, imageType, subNo),
				(string)ProductCommon.GetKeyValue(info, Constants.FIELD_PRODUCT_SHOP_ID),
				GetImageDirectoryPath(siteType, imageType));
		}
		/// <summary>
		/// 画像ファイルが存在するかチェック
		/// </summary>
		/// <param name="fileName">ファイル名（名称＋サイズ＋拡張子）</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="siteType">サイトタイプ</param>
		/// <param name="dir">ディレクトリ</param>
		/// <returns>TRUE:存在する FALSE:存在しない</returns>
		private static bool CheckImageFileExsits(SiteType siteType, string fileName, string shopId, string dir)
		{
			bool result = false;

			// ファイル存在チェック
			if (fileName.Length == 0) return result;

			// 管理サーバー側のファイルをチェックする（Webサーバーと管理サーバーが同じ場合）
			var urlRootPath = (siteType == SiteType.Pc) ? Constants.PATH_ROOT_FRONT_PC : Constants.PATH_ROOT_FRONT_MOBILE;

			// If RoothPath have an empty value. I will return false
			if (urlRootPath == "") return false;

			if (urlRootPath.StartsWith("/"))
			{
				var physicalRootPath = ((siteType == SiteType.Pc) ? Constants.PHYSICALDIRPATH_FRONT_PC : Constants.PHYSICALDIRPATH_FRONT_MOBILE) + dir + shopId + "/" + fileName;
				result = File.Exists(physicalRootPath);
			}
			// Webサーバー側のファイルをチェックする
			else
			{
				try
				{
					StringBuilder url = new StringBuilder((siteType == SiteType.Pc) ? Constants.URL_FRONT_PC : Constants.URL_FRONT_MOBILE);
					url.Append(dir).Append(shopId).Append("/").Append(fileName);

					// HttpWebRequestインスタンス生成＆レスポンス取得
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(url.ToString()));
					using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
					{
						result = httpWebResponse.ContentType.StartsWith("image/");
					}
				}
				catch (Exception ex)
				{
					AppLogger.WriteError("商品画像のレスポンス取得に失敗しました。:", ex);
					return false;
				}
			}

			return result;
		}
		/// <summary>
		/// 画像ディレクトリパス取得
		/// </summary>
		/// <param name="siteType">サイトタイプ</param>
		/// <param name="imageType">商品画像タイプ</param>
		/// <returns>画像ディレクトリパス</returns>
		/// <remarks>サイト（PC,モバイル）と画像（メイン、サブ）に応じたコンテンツからのディレクトリパス</remarks>
		private static string GetImageDirectoryPath(SiteType siteType, ImageType imageType)
		{
			return (siteType == SiteType.Pc) ?
				((imageType == ImageType.Normal) ? Constants.PATH_PRODUCTIMAGES : Constants.PATH_PRODUCTSUBIMAGES) :
				((imageType == ImageType.Normal) ? Constants.PATH_PRODUCTIMAGES_MOBILE : Constants.PATH_PRODUCT_SUB_IMAGES_MOBILE);
		}
		/// <summary>
		/// イメージコンバータURLの作成
		/// </summary>
		/// <param name="imageFileUrl">商品画像URL</param>
		/// <param name="format">拡張子</param>
		/// <param name="width">サイズ</param>
		/// <returns>イメージコンバータ経由で参照可能なURL</returns>
		private static string CreateImageCnvUrl(string imageFileUrl, string format, int width)
		{
			StringBuilder url = new StringBuilder();
			url.Append(Constants.PATH_ROOT + Constants.PAGE_MANAGER_IMAGECONVERTER);
			url.Append("?").Append(Constants.REQUEST_KEY_IMGCNV_FILE).Append("=").Append(HttpUtility.UrlEncode(imageFileUrl));
			url.Append("&").Append(Constants.REQUEST_KEY_IMGCNV_FORMAT).Append("=").Append(HttpUtility.UrlEncode(format));
			url.Append("&").Append(Constants.REQUEST_KEY_IMGCNV_SIZE).Append("=").Append(HttpUtility.UrlEncode(width.ToString()));
			return HtmlSanitizer.UrlAttrHtmlEncode(url.ToString());
		}
		/// <summary>
		/// 商品またはバリエーション画像のHTMLイメージタグ作成
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="siteType">サイトタイプ</param>
		/// <param name="dir"></param>
		/// <param name="format"></param>
		/// <param name="size">サイズ</param>
		/// <param name="isNowPrinting">NowPrintingを使うか</param>
		/// <returns>IMGタグ</returns>
		private static string CreateHtmlImageTag(string fileName, string shopId, SiteType siteType, string dir, string format, int size, bool isNowPrinting)
		{
			//------------------------------------------------------
			// ファイル名が空の時は作成しない
			//------------------------------------------------------
			if (fileName.Length == 0) return "";

			//------------------------------------------------------
			// 条件判断して画像ファイル名などを決定
			//------------------------------------------------------
			StringBuilder imageFileUrl = new StringBuilder();
			// ファイルが存在する？
			if (CheckImageFileExsits(siteType, fileName, shopId, dir))
			{
				imageFileUrl.Append(shopId).Append("/").Append(fileName);
			}
			// NowPrinting画像を使う？
			else if (isNowPrinting)
			{
				imageFileUrl.Append((siteType == SiteType.Pc) ? Constants.PRODUCTIMAGE_NOIMAGE_PC : Constants.PRODUCTIMAGE_NOIMAGE_MOBILE);
			}
			else
			{
				return "";
			}

			//------------------------------------------------------
			// 画像タグ作成
			//------------------------------------------------------
			var rootPath = (siteType == SiteType.Pc) ? Constants.PATH_ROOT_FRONT_PC : Constants.PATH_ROOT_FRONT_MOBILE;
			var path_tmp = rootPath + dir + imageFileUrl.ToString();
			var path = rootPath.StartsWith("/") ? CreateImageCnvUrl(path_tmp, format, size) : path_tmp;
			var alt = path_tmp; // todo ひつようか？
			var imgTag = "<img src=\"{0}\"{1} border=\"0\">";

			return string.Format(imgTag, path, (" alt=\"" + HtmlSanitizer.HtmlEncode(alt) + "\""));
		}

		/// <summary>
		/// イメージコンバータ：画像リサイズ数
		/// </summary>
		/// <param name="size">サイズ</param>
		/// <returns>サイズ</returns>
		private static int GetReSize(string size)
		{
			int reSize = 0;
			int.TryParse(size, out reSize);
			reSize = (reSize > IMAGE_SIZE_MAX) ? IMAGE_SIZE_MAX : reSize;
			reSize = (reSize == 0) ? -1 : reSize;	// あとで画像サイズヘリサイズ
			return reSize;
		}
		/// <summary>
		/// イメージコンバータ：イメージフォーマットタイプ取得
		/// </summary>
		/// <param name="key">フォーマットのキー</param>
		/// <returns>イメージフォーマットタイプ</returns>
		private static ImageFormat GetImageFormat(string key)
		{
			ImageFormat result = null;
			switch (key)
			{
				case Constants.KBN_IMAGEFORMAT_BMP:
					result = ImageFormat.Bmp;
					break;

				case Constants.KBN_IMAGEFORMAT_GIF:
					result = ImageFormat.Gif;
					break;

				case Constants.KBN_IMAGEFORMAT_PNG:
					result = ImageFormat.Png;
					break;

				case Constants.KBN_IMAGEFORMAT_JPG:
				default:
					result = ImageFormat.Jpeg;
					break;
			}
			return result;
		}
		/// <summary>
		/// イメージコンバータ：イメージコンバータフォーマットタイプ取得
		/// </summary>
		/// <param name="key">フォーマットのキー</param>
		/// <returns>イメージコンバータフォーマットタイプ</returns>
		public static string GetImageConverterFormat(string key)
		{
			string result = null;
			switch (key)
			{
				case Constants.KBN_IMAGEFORMAT_BMP:
					result = "image/x-bmp";
					break;

				case Constants.KBN_IMAGEFORMAT_GIF:
					result = "image/gif";
					break;

				case Constants.KBN_IMAGEFORMAT_PNG:
					result = "image/png";
					break;

				case Constants.KBN_IMAGEFORMAT_JPG:
				default:
					result = "image/jpeg";
					break;
			}
			return result;
		}
		/// <summary>
		/// イメージコンバータ：画像コンバート内容をバイト列へ変換して取得
		/// </summary>
		/// <param name="size"></param>
		/// <param name="imageFormat"></param>
		/// <param name="fullPath"></param>
		/// <returns></returns>
		public static byte[] GetConvertToBytes(string size, string imageFormat, string fullPath)
		{
			int reSize = GetReSize(size);
			byte[] bBuffer;
			using (Image imgSrc = Image.FromFile(fullPath, true))
			{
				int width = 0;
				int height = 0;
				if (reSize != -1)
				{
					width = (imgSrc.Size.Width >= imgSrc.Size.Height) ? reSize : (reSize * imgSrc.Size.Width) / imgSrc.Size.Height;
					height = (imgSrc.Size.Height >= imgSrc.Size.Width) ? reSize : (reSize * imgSrc.Size.Height) / imgSrc.Size.Width;
				}
				else
				{
					width = imgSrc.Size.Width;
					height = imgSrc.Size.Height;
				}

				// バイト列へ画像コンバートする
				bBuffer = ImageConvertor.ConvertToBytes(
					imgSrc,
					width,
					height,
					ImageConvertor.SupplementType.None,
					GetImageFormat(imageFormat),
					Constants.PRODUCTIMAGE_ENCODE_QUALITY);
			}
			return bBuffer;
		}

		/// <summary>
		/// Get html image source
		/// </summary>
		/// <param name="fileName">File name</param>
		/// <param name="shopId">Shop id</param>
		/// <param name="siteType">Site type</param>
		/// <param name="imageType">Image type</param>
		/// <param name="isNowPrinting">Is now printing</param>
		/// <returns>A string of html image source</returns>
		public static string GetHtmlImageSource(
			string fileName,
			string shopId,
			SiteType siteType,
			ImageType imageType = ImageType.Normal,
			bool isNowPrinting = true)
		{
			return CreateHtmlImageSource(
				fileName,
				shopId,
				siteType,
				GetImageDirectoryPath(siteType, imageType),
				Constants.KBN_IMAGEFORMAT_JPG,
				isNowPrinting);
		}

		/// <summary>
		/// 商品画像ファイルパスを取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="siteType">サイトタイプ</param>
		/// <param name="imageType">商品画像タイプ</param>
		/// <returns>商品画像ファイルパス</returns>
		public static string GetProductImagePath(
			string fileName,
			string shopId,
			SiteType siteType,
			ImageType imageType = ImageType.Normal)
		{
			if (string.IsNullOrEmpty(fileName)) return string.Empty;

			var imageDirectoryPath = GetImageDirectoryPath(siteType, imageType);

			// 絶対参照商品画像パス（画像存在チェック用）
			var productImageAbsolutePath = string.Format(
				"{0}{1}{2}/{3}",
				Constants.PHYSICALDIRPATH_CONTENTS_ROOT,
				imageDirectoryPath,
				shopId,
				fileName);

			if (File.Exists(productImageAbsolutePath) == false) return string.Empty;

			// 相対参照商品画像パス
			var productImageRelativePath = string.Format(
				"{0}{1}{2}/{3}",
				Constants.PATH_ROOT_FRONT_PC,
				imageDirectoryPath,
				shopId,
				fileName);

			return productImageRelativePath;
		}

		/// <summary>
		/// Create html image source
		/// </summary>
		/// <param name="fileName">File name</param>
		/// <param name="shopId">Shop id</param>
		/// <param name="siteType">Size type</param>
		/// <param name="directoryPath">Directory path</param>
		/// <param name="format">Format</param>
		/// <param name="isNowPrinting">Is now printing</param>
		/// <returns>A string of html image source</returns>
		private static string CreateHtmlImageSource(
			string fileName,
			string shopId,
			SiteType siteType,
			string directoryPath,
			string format,
			bool isNowPrinting)
		{
			// ファイル名が空の時は作成しない
			if (string.IsNullOrEmpty(fileName)) return string.Empty;

			// 条件判断して画像ファイル名などを決定
			var imageFileUrl = new StringBuilder();

			// ファイルが存在する？
			if (CheckImageFileExsits(siteType, fileName, shopId, directoryPath))
			{
				imageFileUrl.AppendFormat("{0}/{1}", shopId, fileName);
			}
			else if (isNowPrinting)
			{
				imageFileUrl.Append((siteType == SiteType.Pc) ? Constants.PRODUCTIMAGE_NOIMAGE_PC_LL : Constants.PRODUCTIMAGE_NOIMAGE_MOBILE);
			}
			else
			{
				return string.Empty;
			}

			// 画像タグ作成
			var rootPath = (siteType == SiteType.Pc)
				? Constants.PATH_ROOT_FRONT_PC
				: Constants.PATH_ROOT_FRONT_MOBILE;
			var path_tmp = string.Format(
				"{0}{1}{2}",
				rootPath,
				directoryPath,
				imageFileUrl.ToString());
			var path = rootPath.StartsWith("/")
				? CreateImageCnvUrl(path_tmp, format, 0)
				: path_tmp;

			return path;
		}

		/// <summary>
		/// Get image source
		/// </summary>
		/// <param name="imagePath">Image path</param>
		/// <param name="isPrinting">Is printing</param>
		/// <returns>Image source</returns>
		public static string GetImageSource(string imagePath, bool isPrinting = true)
		{
			if (string.IsNullOrEmpty(imagePath) && isPrinting)
			{
				imagePath = Path.Combine(
					Constants.PATH_ROOT_FRONT_PC,
					Constants.PATH_PRODUCTIMAGES,
					Constants.PRODUCTIMAGE_NOIMAGE_PC_LL);
			}
			else if (string.IsNullOrEmpty(imagePath))
			{
				return string.Empty;
			}

			var path = imagePath.StartsWith("/")
				? CreateImageCnvUrl(imagePath, Constants.KBN_IMAGEFORMAT_JPG, 0)
				: imagePath;

			return path;
		}
	}
}
