/*
=========================================================================================================
  Module      : SQLステートメントタグリプレーサ (SqlStatementTagReplacer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using w2.Common.Helper;
using w2.Common.Helper.Attribute;
using w2.Common.Util;

namespace w2.Common.Sql
{
	/// <summary>
	/// SQLステートメントタグリプレーザ
	/// </summary>
	internal class SqlStatementTagReplacer
	{
		/// <summary>タグ種別</summary>
		private enum ValueTagType
		{
			/// <summary>valタグ</summary>
			[EnumTextName("val")]
			Val,
			/// <summary>notvalタグ</summary>
			[EnumTextName("notval")]
			NotVal
		}

		/// <summary>
		/// タグのリプレース
		/// </summary>
		/// <returns>ステートメント</returns>
		public static string ReplaceTags(string statement, IDictionary input)
		{
			// valタグ
			statement = ReplaceValueTags(statement, input, ValueTagType.Val);
			// notvalタグ
			statement = ReplaceValueTags(statement, input, ValueTagType.NotVal);
			// hasvalタグ
			statement = ReplaceHasValueTags(statement, input, string.Empty);
			// hasnovalタグ
			statement = ReplaceHasNoValueTags(statement, input, string.Empty);

			return statement;
		}

		/// <summary>
		/// hasvalタグのリプレース
		/// 検証文字列と一致したタグの内容を""に置き換える
		/// </summary>
		/// <param name="statement">ステートメント</param>
		/// <param name="input">入力パラメタ</param>
		/// <param name="matchValue">値との一致検証文字列</param>
		/// <returns>リプレース済みステートメント</returns>
		private static string ReplaceHasValueTags(string statement, IDictionary input, string matchValue)
		{
			var empTags = Regex.Matches(statement, @"<@@hasval:(.*?)@@>");
			foreach (Match empTag in empTags)
			{
				var tagName = empTag.Groups[1].Value;
				var beginTag = "<@@hasval:" + tagName + "@@>";
				var endTag = "</@@hasval:" + tagName + "@@>";
				var beginToEnds = Regex.Matches(statement, beginTag + "(.*?)" + endTag, RegexOptions.Singleline);
				var tagNameSplitted = tagName.Split(',');	// カンマ区切りの定義に対応
				foreach (Match beginToEnd in beginToEnds)
				{
					var matchToValue = tagNameSplitted.All(str => (StringUtility.ToEmpty(input[str]) == matchValue));
					statement = statement.Replace(beginToEnd.Value, matchToValue ? "" : beginToEnd.Groups[1].Value);
				}
			}

			return statement;
		}

		/// <summary>
		/// hasnovalタグのリプレース
		/// 検証文字列と不一致したタグの内容を""に置き換える
		/// </summary>
		/// <param name="statement">ステートメント</param>
		/// <param name="input">入力パラメタ</param>
		/// <param name="matchValue">値との一致検証文字列</param>
		/// <returns>リプレース済みステートメント</returns>
		private static string ReplaceHasNoValueTags(string statement, IDictionary input, string matchValue)
		{
			var empTags = Regex.Matches(statement, @"<@@hasnoval:(.*?)@@>");
			foreach (Match empTag in empTags)
			{
				var tagName = empTag.Groups[1].Value;
				var beginTag = "<@@hasnoval:" + tagName + "@@>";
				var endTag = "</@@hasnoval:" + tagName + "@@>";
				var beginToEnds = Regex.Matches(statement, beginTag + "(.*?)" + endTag, RegexOptions.Singleline);
				var tagNameSplitted = tagName.Split(',');	// カンマ区切りの定義に対応
				foreach (Match beginToEnd in beginToEnds)
				{
					var matchToValue = tagNameSplitted.All(str => (StringUtility.ToEmpty(input[str]) != matchValue));
					statement = statement.Replace(beginToEnd.Value, matchToValue ? "" : beginToEnd.Groups[1].Value);
				}
			}

			return statement;
		}

		/// <summary>
		/// valタグ/notvalタグのリプレース
		/// </summary>
		/// <param name="statement">ステートメント</param>
		/// <param name="input">入力パラメタ</param>
		/// <param name="type">タグ種別(val/notval)</param>
		private static string ReplaceValueTags(string statement, IDictionary input, ValueTagType type)
		{
			var tagName = type.ToText();

			while (true)
			{
				var matchedTags = ExtractPairedTagInfo(statement, tagName).ToArray();
				if (matchedTags.Any() == false) break;

				foreach (var matchedTagInfo in matchedTags)
				{
					var paramKey = matchedTagInfo.TagParams[1];
					var paramValues = matchedTagInfo.TagParams[2].Split(',');

					var isMatchedForCondition = (type == ValueTagType.Val)
						? (input.Contains(paramKey) && (input[paramKey] != null) && paramValues.Contains(input[paramKey].ToString()))
						: (input.Contains(paramKey) == false) || (paramValues.Any(p => (p == input[paramKey].ToString())) == false);

					statement = statement.Replace(
						matchedTagInfo.WholeTag,
						isMatchedForCondition
							? matchedTagInfo.TagInner
							: string.Empty);
				}
			}

			return statement;
		}

		/// <summary>
		/// 一対になっているタグを抽出
		/// </summary>
		/// <param name="statement">ステートメント</param>
		/// <param name="tagName">タグ名</param>
		/// <returns>マッチ済タグ</returns>
		private static IEnumerable<MatchedTag> ExtractPairedTagInfo(string statement, string tagName)
		{
			var tags = Regex.Matches(
				statement,
				string.Format(
					@"<@@({0}.*?)@@>(.*?)</@@\1@@>",
					tagName),
				RegexOptions.Singleline);

			foreach (Match tag in tags)
			{
				yield return new MatchedTag
				{
					WholeTag = tag.Value,
					TagParams = tag.Groups[1].Value.Split(':'),
					TagInner = tag.Groups[2].Value
				};
			}
		}

		/// <summary>
		/// マッチ済みタグ
		/// </summary>
		private class MatchedTag
		{
			/// <summary>タグ全体</summary>
			public string WholeTag { get; set; }
			/// <summary>タグのパラメタ(&lt;@@tagName:AAA:BBB@@&gt;の、tagName,AAA,BBBが格納された配列)</summary>
			public string[] TagParams { get; set; }
			/// <summary>タグの中身</summary>
			public string TagInner { get; set; }
		}
	}
}
