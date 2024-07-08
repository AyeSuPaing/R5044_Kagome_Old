/*
=========================================================================================================
  Module      : ツリービューヘルパ(TreeViewHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using w2.Cms.Manager.Codes.Common;

namespace w2.Cms.Manager.Codes.TreeView
{
	/// <summary>
	/// ツリービューヘルパ
	/// </summary>
	public static class TreeViewHelper
	{
		/// <summary>
		/// アイテムのコレクションで再帰的にHtml木を作成
		/// </summary>
		public static TreeView<T> TreeView<T>(this HtmlHelper html, IEnumerable<T> items)
		{
			return new TreeView<T>(html, items);
		}

		/// <summary>
		/// ファイル取得
		/// </summary>
		/// <param name="targetRealPath">ターゲットパス</param>
		/// <returns>ファイル</returns>
		public static string[] GetTreeViewFiles(string targetRealPath)
		{
			var files = Directory.GetFiles(targetRealPath);
			var result = (files.Length <= Constants.TREEVIEW_MAX_VIEW_CONTENT_COUNT)
				? files
				: null;
			return result;
		}

		/// <summary>
		/// 表示件数超過エラーメッセージ取得
		/// </summary>
		/// <param name="files">ファイル</param>
		/// <returns>エラーメッセージ</returns>
		public static string GetTreeViewErrorMessage(string[] files)
		{
			if (files != null) return string.Empty;
			var errorMessage = WebMessages.TreeViewMaxViewContentError.Replace(
				"@@ 1 @@",
				Constants.TREEVIEW_MAX_VIEW_CONTENT_COUNT.ToString());
			return errorMessage;
		}
	}
}