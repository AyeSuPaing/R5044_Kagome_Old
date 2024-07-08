/*
=========================================================================================================
  Module      : デコメベースページクラス(DecomeBasePage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using w2.Common.Web;

public class DecomeBasePage : BasePage
{
	protected const string TAG_EMOJI_HEAD = "<@@emoji:";
	protected const string TAG_EMOJI_FOOT = "@@>";

	protected const string TAG_DECOME_IMAGE_HEAD = "<@@image:";
	protected const string TAG_DECOME_IMAGE_FOOT = "@@>";

	/// <summary>
	/// タグ毎に処理を行う
	/// </summary>
	/// <param name="strSrcText">タグを含むテキスト</param>
	/// <param name="strTagHead">タグ先頭</param>
	/// <param name="strTagFoot">タグ終端</param>
	/// <param name="action">タグを引数に渡されるデリゲート</param>
	protected static void ForEachTag(string strSrcText, string strTagHead, string strTagFoot, Action<string> aActionForTag)
	{
		foreach (Match mFind in Regex.Matches(strSrcText, strTagHead + "((?!" + strTagFoot + ").)*" + strTagFoot, RegexOptions.Singleline | RegexOptions.IgnoreCase))
		{
			aActionForTag.Invoke(mFind.Value);
		}
	}

	/// <summary>
	/// タグ毎に置換を行う
	/// </summary>
	/// <param name="strSrcText">タグを含むテキスト</param>
	/// <param name="strTagHead">タグ先頭</param>
	/// <param name="strTagFoot">タグ終端</param>
	/// <param name="GetReplaceString">タグを引数に渡される置換処理</param>
	/// <returns>置換結果文字列</returns>
	protected static string ReplaceEachTag(string strSrcText, string strTagHead, string strTagFoot, Func<string, string> GetReplaceString)
	{
		string strReplaceResult = string.Copy(strSrcText);

		ForEachTag(strReplaceResult, strTagHead, strTagFoot, (strMatch) =>
		{
			string strTag = strMatch.Split(':')[1].Replace(strTagFoot, "");
			strReplaceResult = strReplaceResult.Replace(strMatch, GetReplaceString.Invoke(strMatch));
		});

		return strReplaceResult;
	}

	/// <summary>
	/// デコメイメージタグをimgタグに変換する
	/// </summary>
	/// <param name="strSrcTextText">置換対象文字列</param>
	/// <returns>置換結果文字列</returns>
	protected static string ReplaceImageTagToHtml(string strSrcTextText)
	{
		return ReplaceImageTag(strSrcTextText, ((Func<string, string>)((string strMatch) =>
		{
			string strTag = strMatch.Split(':')[1].Replace(TAG_DECOME_IMAGE_FOOT, "");

			return "<img src='" + Constants.MARKETINGPLANNER_DECOME_MOBILEHTMLMAIL_URL + strMatch.Split(':')[1].Replace(TAG_DECOME_IMAGE_FOOT, "") + "' />";
		})));
	}

	/// <summary>
	/// デコメイメージタグ置換（すべてのデコメイメージ文字タグを指定した文字列に置換する）
	/// </summary>
	/// <param name="strSrcTextText"></param>
	/// <param name="GetReplaceFunction"></param>
	/// <returns>置換結果文字列</returns>
	protected static string ReplaceImageTag(string strSrcTextText, string strReplace)
	{
		return ReplaceImageTag(strSrcTextText, (strMatch) => { return strReplace; });
	}
	/// <summary>
	/// デコメイメージタグ置換（タグの中身を引数としてラムダ式にわたし、演算結果で置換する）
	/// </summary>
	/// <param name="strSrcTextText">置換対象文字列</param>
	/// <param name="GetReplaceFunction">引数としてタグの中身を取得し、結果として置換文字列を返すラムダ式</param>
	/// <returns>置換結果文字列</returns>
	protected static string ReplaceImageTag(string strSrcTextText, Func<string, string> GetReplaceString)
	{
		return ReplaceEachTag(strSrcTextText, TAG_DECOME_IMAGE_HEAD, TAG_DECOME_IMAGE_FOOT, GetReplaceString);
	}

	/// <summary>
	/// 絵文字タグをimgタグに変換する
	/// </summary>
	/// <param name="strSrcTextText"></param>
	/// <returns>置換結果文字列</returns>
	protected static string ReplaceEmojiTagToHtml(string strSrcTextText)
	{
		return ReplaceEmojiTag(strSrcTextText, (string strMatch) =>
		{
			string strTag = strMatch.Split(':')[1].Replace(TAG_EMOJI_FOOT, "");

			StringBuilder sbReplacedResult = new StringBuilder();
			sbReplacedResult.Append("<img src='").Append(GetEmojiImageSrcUrl(strTag));
			sbReplacedResult.Append("' Title='").Append(strTag);
			sbReplacedResult.Append("' Alt='").Append(GetEmojiImageSrcUrl(strTag));
			sbReplacedResult.Append("' />");
			return sbReplacedResult.ToString();
		});
	}

	/// <summary>
	/// "<"→"&lt;"のようにサニタイズされた絵文字タグをimgタグに変換する
	/// </summary>
	/// <param name="strSrcTextText"></param>
	/// <returns>置換結果文字列</returns>
	protected static string ReplaceEmojiTagHtmlEncodedToHtml(string strSrcTextText)
	{
		return ReplaceEachTag(strSrcTextText, WebSanitizer.HtmlEncode(TAG_EMOJI_HEAD), WebSanitizer.HtmlEncode(TAG_EMOJI_FOOT), (strMatch) =>
		{
			string strTag = strMatch.Split(':')[1].Replace(WebSanitizer.HtmlEncode(TAG_EMOJI_FOOT), "");

			StringBuilder sbReplacedResult = new StringBuilder();
			sbReplacedResult.Append("<img src='").Append(GetEmojiImageSrcUrl(strTag));
			sbReplacedResult.Append("' Title='").Append(strTag);
			sbReplacedResult.Append("' Alt='").Append(GetEmojiImageSrcUrl(strTag));
			sbReplacedResult.Append("' />");
			return sbReplacedResult.ToString();
		});
	}

	/// <summary>
	/// 絵文字タグ置換（すべての絵文字タグを指定した文字列に置換する）
	/// </summary>
	/// <param name="strSrcTextText"></param>
	/// <param name="GetReplaceFunction"></param>
	/// <returns>置換結果文字列</returns>
	protected static string ReplaceEmojiTag(string strSrcTextText, string strReplace)
	{
		return ReplaceEmojiTag(strSrcTextText, (strMatch) => { return strReplace; });
	}
	/// <summary>
	/// 絵文字タグ置換（タグの中身を引数としてラムダ式にわたし、演算結果で置換する）
	/// </summary>
	/// <param name="strSrcTextText"></param>
	/// <param name="GetReplaceFunction"></param>
	/// <returns>置換結果文字列</returns>
	protected static string ReplaceEmojiTag(string strSrcTextText, Func<string, string> GetReplaceString)
	{
		return ReplaceEachTag(strSrcTextText, TAG_EMOJI_HEAD, TAG_EMOJI_FOOT, GetReplaceString);
	}

	/// <summary>
	/// 絵文字タグ名からimg参照先urlを取得する。該当する画像がなければ、no_emojiを返却
	/// </summary>
	/// <param name="strEmojiTagName">絵文字タグ名</param>
	/// <returns>絵文字画像参照先URL</returns>
	public static string GetEmojiImageSrcUrl(string strEmojiTagName)
	{
		if (File.Exists(Constants.MARKETINGPLANNER_EMOJI_IMAGE_DIRPATH + strEmojiTagName + ".png") == false)
		{
			strEmojiTagName = "no_emoji";
		}

		return Constants.MARKETINGPLANNER_EMOJI_IMAGE_URL + strEmojiTagName + ".png";
	}

	/// <summary>
	/// Bodyタグのbgcolor属性を取得
	/// </summary>
	/// <param name="strSrcText">bodyタグを含む文字列</param>
	/// <returns>#から始まる色指定文字列</returns>
	protected static string GetBodyBgColor(string strSrcText)
	{
		string strBgColor = "#FFFFFF";	// デフォルト背景色は白

		Match match = Regex.Match(strSrcText, @"body\s*[0-9a-z=#""]*s*bgcolor\s*=\s*\""(?<bgcolor>#[0-9a-f]{6})\""", RegexOptions.IgnoreCase);
		if (match.Groups.Count > 1)
		{
			strBgColor = match.Groups["bgcolor"].Value;
		}

		return strBgColor;
	}

	/// <summary>プレビュー対象HTMLリスト</summary>
	protected List<string> HtmlForPreviewList
	{
		get
		{
			return (Session[Constants.SESSION_KEY_HTML_FOR_PREVIEW_LIST] != null)
				? (List<string>)Session[Constants.SESSION_KEY_HTML_FOR_PREVIEW_LIST]
				: null;
		}
		set { Session[Constants.SESSION_KEY_HTML_FOR_PREVIEW_LIST] = value; }
	}
}
