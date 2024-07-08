/*
=========================================================================================================
  Module      : 新着情報マスタモデル (NewsModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text.RegularExpressions;
using w2.Common.Web;

namespace w2.Domain.News
{
	/// <summary>
	/// 新着情報マスタモデル
	/// </summary>
	public partial class NewsModel
	{
		/// <summary>
		///  本文取得(Text,Html判定）
		/// </summary>
		/// <returns>本文</returns>
		public string GetNewsTextHtml()
		{
			// Display HTML
			if (this.NewsTextKbn == Constants.FLG_NEWS_NEWS_TEXT_KBN_HTML) return this.NewsText;

			// Display TEXT
			return HtmlSanitizer.HtmlEncodeChangeToBr(this.NewsText);
		}

		/// <summary>
		/// 本文取得(brタグをスペースに置換）
		/// </summary>
		/// <returns>本文</returns>
		public string GetNewsTextReplaceBr()
		{
			var newsText = GetNewsTextHtml();
			foreach (Match match in Regex.Matches(newsText, "<br.*?>", RegexOptions.IgnoreCase))
			{
				newsText = newsText.Replace(match.Value, " ");
			}

			return newsText;
		}

		#region メソッド
		/// <summary>表示日付（※旧カラム）</summary>
		public DateTime DisplayDate
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_NEWS_DISPLAY_DATE]; }
			set { this.DataSource[Constants.FIELD_NEWS_DISPLAY_DATE] = value; }
		}
		#endregion

		#region プロパティ
		/// <summary>新着情報翻訳前設定情報</summary>
		public NewsBeforeTranslationModel BeforeTranslationData
		{
			get { return (NewsBeforeTranslationModel)this.DataSource["before_translation_data"]; }
			set { this.DataSource["before_translation_data"] = value; }
		}
		#endregion
	}

	/// <summary>
	/// 新着情報翻訳前設定情報モデルクラス
	/// </summary>
	/// <remarks>グローバルOP：ON時、表示名称の切り替えに使用</remarks>
	public class NewsBeforeTranslationModel : ModelBase<NewsBeforeTranslationModel>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public NewsBeforeTranslationModel() { }
		#endregion

		#region プロパティ
		/// <summary>本文区分</summary>
		public string NewsTextKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_KBN]; }
			set { this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_KBN] = value; }
		}
		/// <summary>本文</summary>
		public string NewsText
		{
			get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT]; }
			set { this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT] = value; }
		}
		#endregion
	}
}
