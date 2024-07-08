/*
=========================================================================================================
  Module      : レコメンドボタン画像操作クラス(RecommendButtonImageOperator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using w2.Common.Util;

namespace w2.App.Common.Recommend
{
	#region 列挙対
	/// <summary>ボタン画像種別</summary>
	public enum ButtonImageType
	{
		/// <summary>PCレコメンド商品投入ボタン</summary>
		AddItemPc,
		/// <summary>SPレコメンド商品投入ボタン</summary>
		AddItemSp
	}
	#endregion

	/// <summary>
	/// レコメンドボタン画像操作クラス
	/// </summary>
	[Serializable]
	public class RecommendButtonImageOperator
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RecommendButtonImageOperator()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="buttonImageType">画像ボタン種別</param>
		public RecommendButtonImageOperator(ButtonImageType buttonImageType)
		{
			// ボタン画像種別セット
			this.ButtonImageType = buttonImageType;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// ボタン画像ファイル保存
		/// </summary>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="tempRecommendId">一時レコメンドID</param>
		public void SaveRecommendButtonImageFile(string recommendId, string tempRecommendId)
		{
			// 一時保存中のボタン画像ファイルパス取得
			var filePaths = Directory.EnumerateFiles(this.RecommendButtonImageTempPhysicalDirectoryPath, tempRecommendId + "*");
			foreach (var filePath in filePaths)
			{
				// ファイル名の一時レコメンドIDをレコメンドIDに置換
				var fileName = Path.GetFileName(filePath);
				fileName = fileName.Replace(tempRecommendId, recommendId);

				// 既に画像がある場合は削除
				var oldFilePath
					= Directory.EnumerateFiles(this.RecommendButtonImagePhysicalDirectoryPath, Path.GetFileNameWithoutExtension(fileName) + "*")
						.FirstOrDefault();
				if (oldFilePath != null)
				{
					File.Delete(oldFilePath);
				}

				// 一時ディレクトリからファイル移動
				var destFilePath = Path.Combine(this.RecommendButtonImagePhysicalDirectoryPath,fileName);
				File.Copy(filePath, destFilePath, true);
				File.Delete(filePath);
			}
		}

		/// <summary>
		/// ボタン画像ファイル削除
		/// </summary>
		/// <param name="recommendId">レコメンドID</param>
		public void DeleteRecommendButtonImageFile(string recommendId)
		{
			DeleteRecommendButtonImageFileInner(recommendId, false);
		}

		/// <summary>
		/// 一時保存中のボタン画像ファイル削除
		/// </summary>
		/// <param name="tempRecommendId">一時レコメンドID</param>
		public void DeleteTempRecommendButtonImageFile(string tempRecommendId)
		{
			DeleteRecommendButtonImageFileInner(tempRecommendId, true);
		}

		/// <summary>
		/// ボタン画像ファイル削除（内部用）
		/// </summary>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="isTemp">一時保存か？</param>
		private void DeleteRecommendButtonImageFileInner(string recommendId, bool isTemp)
		{
			// ボタン画像ファイル名（拡張子なし）取得
			var fileNameWithoutExtension = GetRecommendButtonImageFileNameWithoutExtension(recommendId);
			// ディレクトリパス取得
			var directoryPath = (isTemp == false)
				? this.RecommendButtonImagePhysicalDirectoryPath
				: this.RecommendButtonImageTempPhysicalDirectoryPath;
			// ボタン画像ファイルを削除
			var filePaths = Directory.EnumerateFiles(directoryPath, fileNameWithoutExtension + "*");
			foreach (var filePath in filePaths)
			{
				File.Delete(filePath);
			}
		}

		/// <summary>
		/// ボタン画像ファイルを一時ディレクトリにコピー
		/// </summary>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="tempRecommendId">一時レコメンドID</param>
		public void CopyTempRecommendButtonImageFile(string recommendId, string tempRecommendId)
		{
			// 一時保存中のボタン画像ファイルパス取得
			var filePaths = Directory.EnumerateFiles(this.RecommendButtonImagePhysicalDirectoryPath, recommendId + "*");
			foreach (var filePath in filePaths)
			{
				// ファイル名のレコメンドIDを一時レコメンドIDに置換
				var fileName = Path.GetFileName(filePath);
				fileName = fileName.Replace(recommendId, tempRecommendId);

				// 一時ディレクトリへファイルコピー
				var destFilePath = Path.Combine(this.RecommendButtonImageTempPhysicalDirectoryPath , fileName);
				File.Copy(filePath, destFilePath, true);

				// 読み取り専用解除
				var fileAttributes = File.GetAttributes(destFilePath);
				var isReadOnly = ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
				if (isReadOnly)
				{
					File.SetAttributes(destFilePath, fileAttributes & ~FileAttributes.ReadOnly);
				}
			}
		}

		/// <summary>
		/// ボタン画像物理ディレクトリパス取得
		/// </summary>
		/// <param name="isTemp">一時保存か？</param>
		private string GetRecommendButtonImagePhysicalDirectoryPath(bool isTemp)
		{
			var path = (isTemp == false)
				? this.RecommendButtonImagePhysicalDirectoryPath
				: this.RecommendButtonImageTempPhysicalDirectoryPath;
			return path;
		}

		/// <summary>
		/// ボタン画像ディレクトリパス取得
		/// </summary>
		/// <param name="isTemp">一時保存か？</param>
		private string GetRecommendButtonImageDirectoryPath(bool isTemp)
		{
			var path = (isTemp == false)
				? this.RecommendButtonImageDirectoryPath
				: this.RecommendButtonImageTempDirectoryPath;
			return path;
		}

		/// <summary>
		/// ボタン画像物理ファイルパス取得
		/// </summary>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="isTemp">一時保存か？</param>
		/// <returns>ボタン画像物理ファイルパス</returns>
		private string GetRecommendButtonImagePhysicalFilePath(string recommendId, bool isTemp = false)
		{
			// 物理ディレクトリパス取得
			var path = GetRecommendButtonImagePhysicalDirectoryPath(isTemp);

			// ファイル検索＆ファイルパスを返す
			var filePath
				= Directory.EnumerateFiles(path, GetRecommendButtonImageFileNameWithoutExtension(recommendId) + "*").FirstOrDefault();
			return StringUtility.ToEmpty(filePath);
		}

		/// <summary>
		/// ボタン画像物理ファイルパス取得
		/// </summary>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="fileExtension">ファイル拡張子</param>
		/// <param name="isTemp">一時保存か？</param>
		/// <returns>ボタン画像物理ファイルパス</returns>
		public string GetRecommendButtonImagePhysicalFilePath(string recommendId, string fileExtension, bool isTemp = false)
		{
			// 物理ディレクトリパス取得
			var path = GetRecommendButtonImagePhysicalDirectoryPath(isTemp);
			// ファイルパスを返す
			var fileName = GetRecommendButtonImageFileName(recommendId, fileExtension);
			return Path.Combine(path, fileName);
		}

		/// <summary>
		/// ボタン画像ファイルパス取得
		/// </summary>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="isTemp">一時保存か？</param>
		/// <returns>ボタン画像物理ファイルパス</returns>
		public string GetRecommendButtonImageFilePath(string recommendId, bool isTemp = false)
		{
			// ファイルが存在しない場合は空文字を返す
			var filePath = GetRecommendButtonImagePhysicalFilePath(recommendId, isTemp);
			if (filePath.Length == 0) return string.Empty;

			var fileExtension = Path.GetExtension(filePath);
			var recommendButtonImageFilePath = GetRecommendButtonImageFilePath(recommendId, fileExtension, isTemp);
			return recommendButtonImageFilePath;
		}

		/// <summary>
		/// ボタン画像ファイルパス取得
		/// </summary>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="fileExtension">ファイル拡張子</param>
		/// <param name="isTemp">一時保存か？</param>
		/// <returns>ボタン画像物理ファイルパス</returns>
		public string GetRecommendButtonImageFilePath(string recommendId, string fileExtension, bool isTemp = false)
		{
			var path = GetRecommendButtonImageDirectoryPath(isTemp);
			var fileName = GetRecommendButtonImageFileName(recommendId, fileExtension);
			return Constants.PATH_ROOT_FRONT_PC + path + fileName;
		}

		/// <summary>
		/// ボタン画像ファイル名取得
		/// </summary>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="fileExtension">ファイル拡張子</param>
		/// <returns>ボタン画像ファイル名</returns>
		public string GetRecommendButtonImageFileName(string recommendId, string fileExtension)
		{
			return GetRecommendButtonImageFileNameWithoutExtension(recommendId) + fileExtension;
		}

		/// <summary>
		/// ボタン画像ファイル名（拡張子なし）取得
		/// </summary>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>ボタン画像ファイル名（拡張子なし）</returns>
		public string GetRecommendButtonImageFileNameWithoutExtension(string recommendId)
		{
			// ファイルフォーマット：レコメンドID_ボタン画像種別
			return string.Format("{0}_{1}",
				recommendId,
				this.ButtonImageType);
		}

		/// <summary>
		/// ボタン画像ディレクトリ作成
		/// </summary>
		public void CreateRecommendButtonImageDirectory()
		{
			// ディレクトリ存在チェック（存在しない場合は作成）
			if (Directory.Exists(this.RecommendButtonImagePhysicalDirectoryPath) == false)
			{
				Directory.CreateDirectory(this.RecommendButtonImagePhysicalDirectoryPath);
			}
			if (Directory.Exists(this.RecommendButtonImageTempPhysicalDirectoryPath) == false)
			{
				Directory.CreateDirectory(this.RecommendButtonImageTempPhysicalDirectoryPath);
			}
		}

		/// <summary>
		/// 古い（作成日が1日以上経った）一時ボタン画像ファイルを削除
		/// </summary>
		public void DeleteOldTempRecommendButtonImageFile()
		{
			// 作成日が1日以上経った一時ファイルを削除
			foreach (var filePath in Directory.EnumerateFiles(this.RecommendButtonImageTempPhysicalDirectoryPath, "*"))
			{
				if (File.GetCreationTime(filePath) < DateTime.Now.AddDays(-1))
				{
					File.Delete(filePath);
				}
			}
		}
		#endregion

		#region プロパティ
		/// <summary>ボタン画像種別</summary>
		private ButtonImageType ButtonImageType { get; set; }
		/// <summary>ボタン画像ディレクトリパス</summary>
		private string RecommendButtonImageDirectoryPath
		{
			get { return Constants.PATH_RECOMMEND_BUTTONIMAGES; }
		}
		/// <summary>ボタン画像物理ディレクトリパス</summary>
		private string RecommendButtonImagePhysicalDirectoryPath
		{
			get { return Constants.PHYSICALDIRPATH_CONTENTS_ROOT + this.RecommendButtonImageDirectoryPath.Replace("/", "\\"); }
		}
		/// <summary>ボタン画像一時ディレクトリパス</summary>
		private string RecommendButtonImageTempDirectoryPath
		{
			get { return this.RecommendButtonImageDirectoryPath + "temp/"; }
		}
		/// <summary>ボタン画像物理一時ディレクトリパス</summary>
		private string RecommendButtonImageTempPhysicalDirectoryPath
		{
			get { return Constants.PHYSICALDIRPATH_CONTENTS_ROOT + this.RecommendButtonImageTempDirectoryPath.Replace("/", "\\"); }
		}
		#endregion
	}
}