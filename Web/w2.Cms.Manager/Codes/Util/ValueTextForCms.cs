/*
=========================================================================================================
  Module      : CMS向け値テキストモジュール(ValueTextForCms.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace w2.Cms.Manager.Codes.Util
{
	/// <summary>
	/// CMS向け値テキストモジュールユーティリティ
	/// </summary>
	public class ValueTextForCms : w2.Common.Util.ValueText
	{
		/// <summary>
		/// フィールド値の表示文字列選択リストアイテム配列取得(MVC向け)
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <param name="fieldName">フィールド名</param>
		/// <returns>SelectListItem配列</returns>
		public static SelectListItem[] GetValueSelectListItems(string tableName, string fieldName)
		{
			var result = GetValueKvpArray(tableName, fieldName).Select(
				kvp => new SelectListItem
				{
					Value = kvp.Key,
					Text = kvp.Value,
				}).ToArray();
			return result;
		}

		/// <summary>
		/// Convert value text to array string
		/// </summary>
		/// <param name="tableName">Table name</param>
		/// <param name="fieldName">Field name</param>
		/// <returns>An array string</returns>
		public static string ConvertValueTextToArrayString(string tableName, string fieldName)
		{
			var items = GetValueKvpArray(tableName, fieldName)
				.Select(item =>
					new SelectListItem
					{
						Value = item.Key,
						Text = item.Value,
					});

			if (items.Any() == false) return string.Empty;

			// String format by pattern
			var itemFormat = "{{ text: \"{0}\", value: \"{1}\" }}";
			var itemListString = string.Join(
				",",
				items.Select(item => string.Format(itemFormat, item.Text, item.Value)));

			var result = string.Format("[{0}]", itemListString);
			return result;
		}

		/// <summary>
		/// Convert value text to array string not empty
		/// </summary>
		/// <param name="tableName">Table name</param>
		/// <param name="fieldName">Field name</param>
		/// <returns>An array string not empty</returns>
		public static string ConvertValueTextListToArrayStringNotEmpty(string tableName, string fieldName)
		{
			var items = GetValueKvpArray(tableName, fieldName)
				.Select(item =>
					new SelectListItem
					{
						Value = item.Key,
						Text = item.Value,
					})
				.Where(item => (string.IsNullOrEmpty(item.Value) == false) && (string.IsNullOrEmpty(item.Text) == false));

			if (items.Any() == false) return string.Empty;

			// String format by pattern
			var itemFormat = "{{ text: \"{0}\", value: \"{1}\" }}";
			var itemListString = string.Join(
				",",
				items.Select(item => string.Format(itemFormat, item.Text, item.Value)));

			var result = string.Format("[{0}]", itemListString);
			return result;
		}
	}
}