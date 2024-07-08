/*
=========================================================================================================
  Module      : デザイン管理 共通操作処理(DesignUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Text;
using System.Web;
using w2.App.Common.Design;
using w2.Cms.Manager.Codes.Common;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.Cms.Manager.Codes.PageDesign
{
	/// <summary>
	/// デザイン管理 共通操作処理
	/// </summary>
	public class DesignUtility
	{
		/// <summary>
		/// ファイル 物理削除
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <returns>エラーメッセージ</returns>
		public static string DeleteFile(string filePath)
		{
			var successFlg = DesignCommon.DeleteFile(filePath);
			var errorMesssage = (successFlg)
				? string.Empty
				: WebMessages.FileDeleteError.Replace("@@ 1 @@", Path.GetFileName(filePath));
			return errorMesssage;
		}

		/// <summary>
		/// ファイル更新
		/// </summary>
		/// <param name="targetFilePath">更新対象のファイルパス</param>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <param name="dateCheck">日付による更新チェックの有無</param>
		/// <param name="isPageRename">ページ名の変更か</param>
		/// <returns>エラーメッセージ</returns>
		public static string UpdateFile(string targetFilePath, string fileTextAll, bool dateCheck = false, bool isPageRename = false)
		{
			if ((File.Exists(targetFilePath)) && ((File.GetAttributes(targetFilePath) & FileAttributes.ReadOnly) != 0))
			{
				return WebMessages.FileReadOnlyError.Replace("@@ 1 @@", Path.GetFileName(targetFilePath));
			}

			// 時刻が空でない場合は画面上でファイルを開いた時間と実ファイルの最終更新時刻を比較する。
			if (dateCheck)
			{
				var updateTime = HttpContext.Current.Session[targetFilePath];
				if ((updateTime != null) && (File.GetLastWriteTime(targetFilePath) > (DateTime)updateTime))
				{
					return WebMessages.FileOldError;
				}
			}

			if ((targetFilePath.EndsWith(PageDesignUtility.PREVIEW_PAGE_FILE_EXTENSION) == false)
				&& (targetFilePath.EndsWith(PartsDesignUtility.PREVIEW_PARTS_FILE_EXTENSION) == false)
				&& (File.Exists(targetFilePath) == false)
				&& (isPageRename == false))
			{
				return string.Empty;
			}

			try
			{
				var encode = File.Exists(targetFilePath) ? StringUtility.GetCode(File.ReadAllBytes(targetFilePath)) : Encoding.UTF8;

				using (var sw = new StreamWriter(targetFilePath, false, encode))
				{
					sw.Write(fileTextAll);
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError("ページ管理 ファイル更新失敗", ex);
				return WebMessages.FileUpdateError.Replace("@@ 1 @@", Path.GetFileName(targetFilePath));
			}
			return string.Empty;
		}

		/// <summary>
		/// 最終更新者更新
		/// </summary>
		/// <param name="targetFilePath">更新対象のファイルパス</param>
		/// <param name="changeOperator">オペレータ名</param>
		/// <returns>エラーメッセージ</returns>
		public static string UpdateLastChanged(string targetFilePath, string changeOperator)
		{
			var fileTextAll = new StringBuilder(DesignCommon.GetFileTextAll(targetFilePath));
			DesignCommon.ReplaceLastChanged(fileTextAll, changeOperator);

			var errorMessage = UpdateFile(targetFilePath, fileTextAll.ToString());
			return errorMessage;
		}
	}
}
