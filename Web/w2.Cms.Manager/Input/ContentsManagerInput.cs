/*
=========================================================================================================
  Module      : コンテンツマネージャ入力クラス(ContentsManagerInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.IO;
using System.Linq;
using System.Web;
using System.Drawing;
using w2.App.Common;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Codes.Settings;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// コンテンツマネージャ入力クラス
	/// </summary>
	public class ContentsManagerInput
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ContentsManagerInput()
		{
			this.ZipDecompress = true;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="current">選択されたディレクトリ</param>
		/// <returns>エラーメッセージ</returns>
		internal string Validate(string current)
		{
			// ファイル指定なしチェック
			if ((this.UploadContents == null) || (string.IsNullOrEmpty(this.UploadContents.FileName)))
			{
				return "ファイルを指定してください";
			}

			var fileName = this.UploadContents.FileName;

			// アップロード先ファイルのアクセス権なしチェック
			var targetDirPath = Path.GetDirectoryName(Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, current));
			var targetFilePath = Path.Combine(targetDirPath, fileName);

			if (ContentsManagerSetting.GetInstance().RefuseFileList.Any(
				rfl => rfl == targetFilePath.Replace(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, string.Empty).ToLower()))
			{
				return WebMessages.ContentsManagerFileAlreadyExists;
			}

			// 画像フォルダだったら指定サイズの入力チェック
			if ((ContentsManagerWorkerService.IsSelectImages(current) == false) && (ContentsManagerWorkerService.IsSelectSubImages(current) == false)) return string.Empty;
			// Subが選択されてる時はSサイズの入力をチェックしない
			if (((ContentsManagerWorkerService.IsSelectImages(current)) && (CheckResizeImageSize(this.ImageSizeWidthS, this.ImageSizeHeightS) == false))
				|| (CheckResizeImageSize(this.ImageSizeWidthM, this.ImageSizeHeightM) == false)
				|| (CheckResizeImageSize(this.ImageSizeWidthL, this.ImageSizeHeightL) == false)
				|| (CheckResizeImageSize(this.ImageSizeWidthLL, this.ImageSizeHeightLL) == false))
			{
				return WebMessages.ContentsManagerImageSizeError;
			}

			return string.Empty;
		}

		/// <summary>
		/// 画像リサイズ値チェック
		/// </summary>
		/// <param name="imageSizeWidth">指定サイズ(横幅)</param>
		/// <param name="imageSizeHeight">指定サイズ(縦幅)</param>
		/// <returns>チェック結果</returns>
		private bool CheckResizeImageSize(string imageSizeWidth, string imageSizeHeight)
		{
			var checkResult = (CheckResizeImageSize(imageSizeWidth) && CheckResizeImageSize(imageSizeHeight));
			return checkResult;
		}
		/// <summary>
		/// 画像リサイズ値チェック（どちらか片方のみ）
		/// </summary>
		/// <param name="imageSize">指定サイズ</param>
		/// <returns>チェック結果</returns>
		/// <remarks>指定サイズが1以上の数値かどうかをチェックする（空欄はOKとする）</remarks>
		private bool CheckResizeImageSize(string imageSize)
		{
			// 空はOK（もう一方のサイズに拡大縮小するパターンがあるため）
			if (imageSize.Trim() == string.Empty)
			{
				return true;
			}

			// int型に変換できなければNG
			var size = 0;
			if (int.TryParse(imageSize, out size) == false)
			{
				return false;
			}

			// 1以上であればOK
			return (size >= 1);
		}

		/// <summary>アップロードファイル</summary>
		public HttpPostedFileBase UploadContents { get; set; }
		/// <summary>ZIP自動解凍</summary>
		public bool ZipDecompress { get; set; }
		/// <summary>PCサイト商品画像自動リサイズ</summary>
		public bool AutoResize { get; set; }
		/// <summary>PCサイト商品画像自動リサイズS横幅</summary>
		public string ImageSizeWidthS { get; set; }
		/// <summary>PCサイト商品画像自動リサイズM横幅</summary>
		public string ImageSizeWidthM { get; set; }
		/// <summary>PCサイト商品画像自動リサイズL横幅</summary>
		public string ImageSizeWidthL { get; set; }
		/// <summary>PCサイト商品画像自動リサイズLL横幅</summary>
		public string ImageSizeWidthLL { get; set; }
		/// <summary>PCサイト商品画像自動リサイズS縦</summary>
		public string ImageSizeHeightS { get; set; }
		/// <summary>PCサイト商品画像自動リサイズM縦</summary>
		public string ImageSizeHeightM { get; set; }
		/// <summary>PCサイト商品画像自動リサイズL縦</summary>
		public string ImageSizeHeightL { get; set; }
		/// <summary>PCサイト商品画像自動リサイズLL縦</summary>
		public string ImageSizeHeightLL { get; set; }
	}
}