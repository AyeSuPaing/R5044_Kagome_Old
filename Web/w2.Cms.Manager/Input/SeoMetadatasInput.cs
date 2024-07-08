/*
=========================================================================================================
  Module      : SEO設定入力クラス (SeoMetadatasInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes.Common;
using w2.Domain.SeoMetadatas;
using Constants = w2.App.Common.Constants;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// SEO設定入力クラス
	/// </summary>
	public class SeoMetadatasInput : InputBase<SeoMetadatasModel>
	{
		/// <summary>SEOタグキーリスト</summary>
		private readonly List<string> SEO_TAG_KEY_LIST = new List<string>()
		{
			{ Constants.SEOSETTING_KEY_HTML_TITLE.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_TITLE.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_ROOT_PARENT_CATEGORY_NAME.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PARENT_CATEGORY_NAME.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_CATEGORY_NAME.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_CHILD_CATEGORY_TOP.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_SEO_KEYWORDS.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_BRAND_TITLE.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_BRAND_SEO_KEYWORD.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_ICON1.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_ICON2.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_ICON3.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_ICON4.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_ICON5.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_ICON6.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_ICON7.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_ICON8.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_ICON9.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_ICON10.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_COLOR_KEYWORD.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_PRICE_KEYWORD.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_FREE_WORD_KEYWORD.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_DEFAULT_TEXT.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_NAME.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_NAMES.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_NAME_KANA.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_PRODUCT_TAG.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_VARIATION_NAME1.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_VARIATION_NAME2.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_VARIATION_NAME3.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_SEO_TITLE.Replace("@@ ", "").Replace(" @@", "") },
			{ Constants.SEOSETTING_KEY_SEO_DESCRIPTION.Replace("@@ ", "").Replace(" @@", "") },
		};

		/// <summary>SEOテキストタイプリスト</summary>
		private readonly List<string> SEO_TEXT_TYPE_LIST = new List<string>()
		{
			{ SEO_TEXT_TYPE_TITLE },
			{ SEO_TEXT_TYPE_KEYWORDS },
			{ SEO_TEXT_TYPE_DESCRIPTION },
			{ SEO_TEXT_TYPE_COMMENT },
			{ SEO_TEXT_TYPE_SEOTEXT },
		};

		/// <summary>開始タグ終了タグのリスト</summary>
		private readonly List<string> SEO_TAG_LIST_FOR_START_AND_END = new List<string>()
		{
			{ SEO_TAG_START },
			{ SEO_TAG_END },
		};

		/// <summary>SEOテキストタイプ（タイトル）</summary>
		private const string SEO_TEXT_TYPE_TITLE = "title";
		/// <summary>SEOテキストタイプ（キーワード）</summary>
		private const string SEO_TEXT_TYPE_KEYWORDS = "keywords";
		/// <summary>SEOテキストタイプ（ディスクリプション）</summary>
		private const string SEO_TEXT_TYPE_DESCRIPTION = "description";
		/// <summary>SEOテキストタイプ（コメント）</summary>
		private const string SEO_TEXT_TYPE_COMMENT = "comment";
		/// <summary>SEOテキストタイプ（SEO文言）</summary>
		private const string SEO_TEXT_TYPE_SEOTEXT = "seo_text";
		/// <summary>開始タグ</summary>
		private const string SEO_TAG_START = "set_tag_start";
		/// <summary>終了タグ</summary>
		private const string SEO_TAG_END = "set_tag_end";
		/// <summary>タグと考えられるものを取得する正規表現パターン</summary>
		public const string GET_NEARLY_SEO_TAG_PATTERN = "<[^>]*@[^>]*@([^@>]*?)@[^>]*@[^>]*>";
		/// <summary>タグが正しいかを確認する正規表現パターン </summary>
		public const string SEO_TAG_REGEX_PATTERN = @"</?@@( ?([^@>\s])* ?)@@>";
		/// <summary>開始タグ置換パターン</summary>
		private const string CONVERT_PATTERN_SEO_TAG_START = "<@@((?!@@>).)*@@>";
		/// <summary>終了タグ置換パターン</summary>
		private const string CONVERT_PATTERN_SEO_TAG_END = "</@@((?!@@>).)*@@>";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SeoMetadatasInput()
		{
			this.DataKbn = string.Empty;
			this.HtmlTitle = string.Empty;
			this.MetadataKeywords = string.Empty;
			this.MetadataDesc = string.Empty;
			this.Comment = string.Empty;
			this.LastChanged = string.Empty;
			this.SeoText = string.Empty;
			this.DefaultText = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public SeoMetadatasInput(SeoMetadatasModel model)
			: this()
		{
			if (model == null) return;

			this.DataKbn = model.DataKbn;
			this.HtmlTitle = model.HtmlTitle;
			this.MetadataKeywords = model.MetadataKeywords;
			this.MetadataDesc = model.MetadataDesc;
			this.Comment = model.Comment;
			this.LastChanged = model.LastChanged;
			this.SeoText = model.SeoText;
			this.DefaultText = model.DefaultText;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override SeoMetadatasModel CreateModel()
		{
			var model = new SeoMetadatasModel
			{
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				DataKbn = this.DataKbn,
				HtmlTitle = this.HtmlTitle,
				MetadataKeywords = this.MetadataKeywords,
				MetadataDesc = this.MetadataDesc,
				Comment = this.Comment,
				LastChanged = this.LastChanged,
				SeoText = this.SeoText,
				DefaultText = this.DefaultText,
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="register">登録？</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(bool register)
		{
			var errorMessage = Validator.Validate(
				register ? "SeoMetadatasRegister" : "SeoMetadatasModify",
				this.DataSource);

			// SEO用囲いタグの検証
			foreach (var seoTextType in SEO_TEXT_TYPE_LIST)
			{
				MatchCollection matchSeoTags = Regex.Matches("", "");
				switch (seoTextType)
				{
					case SEO_TEXT_TYPE_TITLE:
						matchSeoTags = GetMatchCollectionForSeoTags(
							this.HtmlTitle,
							GET_NEARLY_SEO_TAG_PATTERN);
						break;

					case SEO_TEXT_TYPE_KEYWORDS:
						matchSeoTags = GetMatchCollectionForSeoTags(
							this.MetadataKeywords,
							GET_NEARLY_SEO_TAG_PATTERN);
						break;

					case SEO_TEXT_TYPE_DESCRIPTION:
						matchSeoTags = GetMatchCollectionForSeoTags(
							this.MetadataDesc,
							GET_NEARLY_SEO_TAG_PATTERN);
						break;

					case SEO_TEXT_TYPE_COMMENT:
						matchSeoTags = GetMatchCollectionForSeoTags(
							this.Comment,
							GET_NEARLY_SEO_TAG_PATTERN);
						break;

					case SEO_TEXT_TYPE_SEOTEXT:
						matchSeoTags = GetMatchCollectionForSeoTags(
							this.Comment,
							GET_NEARLY_SEO_TAG_PATTERN);
						break;
				}
				var matchSeoTagValues = matchSeoTags
					.Cast<Match>()
					.Select(match => new KeyValuePair<string, string>(match.Groups[1].Value, match.Value))
					.ToList();

				errorMessage += ValidateSeoText(matchSeoTagValues, seoTextType);

				var startSeoTagValues = matchSeoTagValues.Where(matchValues => Regex.IsMatch(matchValues.Value, CONVERT_PATTERN_SEO_TAG_START)).ToList();
				var endSeoTagValues = matchSeoTagValues.Where(matchValues => Regex.IsMatch(matchValues.Value, CONVERT_PATTERN_SEO_TAG_END)).ToList();
				if ((startSeoTagValues.Count != endSeoTagValues.Count) && string.IsNullOrEmpty(errorMessage))
				{
					errorMessage += ValidateSeoEnclosure(startSeoTagValues, endSeoTagValues, seoTextType);
				}
			}
			return errorMessage;
		}

		/// <summary>
		/// 囲いタグの条件一致項目を取得する
		/// </summary>
		/// <param name="targetText">ターゲットテキスト</param>
		/// <param name="convertPattern">置換パターン</param>
		/// <returns>エラーメッセージ整形結果</returns>
		public MatchCollection GetMatchCollectionForSeoTags(string targetText, string convertPattern)
		{
			var resultCollection = Regex.Matches(targetText, convertPattern);
			return resultCollection;
		}

		/// <summary>
		/// SEOテキスト検証
		/// </summary>
		/// <param name="matchSeoTagValues">SEO閉じタグの値</param>
		/// <param name="seoTextType">SEOテキストタイプ</param>
		/// <returns>エラーメッセージ</returns>
		public string ValidateSeoText(List<KeyValuePair<string, string>> matchSeoTagValues, string seoTextType)
		{
			var errorMessage = "";
			foreach (var match in matchSeoTagValues)
			{
				var seoMessage = match.Value.Replace("<", "&lt;").Replace(">", "&gt;");
				if ((SEO_TAG_KEY_LIST.Contains(match.Key) && Regex.IsMatch(match.Value, SEO_TAG_REGEX_PATTERN)) == false)
				{
					switch (seoTextType)
					{
						case SEO_TEXT_TYPE_TITLE:
							errorMessage += WebMessages.Get(WebMessages.ERRMSG_MANAGER_SEO_TITLE_TEXT_ERROR)
								.Replace("@@ 1 @@", seoMessage);
							errorMessage = SetBrCodeAtTheEndInCaseOfEmpty(errorMessage);
							break;

						case SEO_TEXT_TYPE_KEYWORDS:
							errorMessage += WebMessages.Get(WebMessages.ERRMSG_MANAGER_SEO_KEYWORDS_TEXT_ERROR)
								.Replace("@@ 1 @@", seoMessage);
							errorMessage = SetBrCodeAtTheEndInCaseOfEmpty(errorMessage);
							break;

						case SEO_TEXT_TYPE_DESCRIPTION:
							errorMessage += WebMessages.Get(WebMessages.ERRMSG_MANAGER_SEO_DESCRIPTION_TEXT_ERROR)
								.Replace("@@ 1 @@", seoMessage);
							errorMessage = SetBrCodeAtTheEndInCaseOfEmpty(errorMessage);
							break;

						case SEO_TEXT_TYPE_COMMENT:
							errorMessage += WebMessages.Get(WebMessages.ERRMSG_MANAGER_SEO_COMMENT_TEXT_ERROR)
								.Replace("@@ 1 @@", seoMessage);
							errorMessage = SetBrCodeAtTheEndInCaseOfEmpty(errorMessage);
							break;
					}
				}
			}

			return errorMessage;
		}

		/// <summary>
		/// SEO囲みタグ検証
		/// </summary>
		/// <param name="startSeoTagValues">SEO開始タグの値</param>
		/// <param name="endSeoTagValues">SEO閉じタグの値</param>
		/// <param name="seoTextType">SEOテキストタイプ</param>
		/// <returns>エラーメッセージ</returns>
		public string ValidateSeoEnclosure(List<KeyValuePair<string, string>> startSeoTagValues, List<KeyValuePair<string, string>> endSeoTagValues, string seoTextType)
		{
			var errorMessage = "";
			foreach (var startTag in startSeoTagValues.ToArray())
			{
				foreach (var endTag in endSeoTagValues.ToArray())
				{
					// 囲みタグのセットがあればセットごと削除
					if (startTag.Key == endTag.Key)
					{
						startSeoTagValues.Remove(startTag);
						endSeoTagValues.Remove(endTag);
						break;
					}
				}
			}
			if (startSeoTagValues.Count > 0)
			{
				errorMessage = GetEncloserErrorMessage(seoTextType, startSeoTagValues, errorMessage);
			}
			if (endSeoTagValues.Count > 0)
			{
				errorMessage = GetEncloserErrorMessage(seoTextType, endSeoTagValues, errorMessage, false);
			}
			return errorMessage;
		}

		/// <summary>
		/// SEO囲みタグエラーメッセージ整形
		/// </summary>
		/// <param name="seoTextType">SEOテキストタイプ</param>
		/// <param name="seoTagValues">SEOタグの値</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <param name="isStartTag">対象が開始タグか</param>
		/// <returns>エラーメッセージ</returns>
		public string GetEncloserErrorMessage(string seoTextType, List<KeyValuePair<string, string>> seoTagValues, string errorMessage, bool isStartTag = true)
		{
			foreach (var seoTag in seoTagValues)
			{
				var seoMessage = seoTag.Value.Replace("<", "&lt;").Replace(">", "&gt;");
				switch (seoTextType)
				{
					case SEO_TEXT_TYPE_TITLE:
						errorMessage += WebMessages.Get(
							(isStartTag)
								? WebMessages.ERRMSG_MANAGER_SEO_TITLE_START_TAG_ENCLOSURE_ERROR
								: WebMessages.ERRMSG_MANAGER_SEO_TITLE_END_TAG_ENCLOSURE_ERROR)
							.Replace("@@ 1 @@", seoMessage);
						errorMessage = SetBrCodeAtTheEndInCaseOfEmpty(errorMessage);
						break;

					case SEO_TEXT_TYPE_KEYWORDS:
						errorMessage += WebMessages.Get(
								(isStartTag)
									? WebMessages.ERRMSG_MANAGER_SEO_KEYWORDS_START_TAG_ENCLOSURE_ERROR
									: WebMessages.ERRMSG_MANAGER_SEO_KEYWORDS_END_TAG_ENCLOSURE_ERROR)
							.Replace("@@ 1 @@", seoMessage);
						errorMessage = SetBrCodeAtTheEndInCaseOfEmpty(errorMessage);
						break;

					case SEO_TEXT_TYPE_DESCRIPTION:
						errorMessage += WebMessages.Get(
								(isStartTag)
									? WebMessages.ERRMSG_MANAGER_SEO_DESCRIPTION_START_TAG_ENCLOSURE_ERROR
									: WebMessages.ERRMSG_MANAGER_SEO_DESCRIPTION_END_TAG_ENCLOSURE_ERROR)
							.Replace("@@ 1 @@", seoMessage);
						errorMessage = SetBrCodeAtTheEndInCaseOfEmpty(errorMessage);
						break;

					case SEO_TEXT_TYPE_COMMENT:
						errorMessage += WebMessages.Get(
								(isStartTag)
									? WebMessages.ERRMSG_MANAGER_SEO_COMMENT_START_TAG_ENCLOSURE_ERROR
									: WebMessages.ERRMSG_MANAGER_SEO_COMMENT_END_TAG_ENCLOSURE_ERROR)
							.Replace("@@ 1 @@", seoMessage);
						errorMessage = SetBrCodeAtTheEndInCaseOfEmpty(errorMessage);
						break;
				}
			}
			return errorMessage;
		}

		/// <summary>
		/// 空文字でない場合終端に改行コードを設定する
		/// </summary>
		/// <param name="target">対象</param>
		/// <returns>結果</returns>
		public string SetBrCodeAtTheEndInCaseOfEmpty(string target)
		{
			if (string.IsNullOrEmpty(target) == false)
			{
				return target + "<br />";
			}
			return target;
		}
		#endregion

		#region プロパティ
		/// <summary>データ区分</summary>
		public string DataKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_DATA_KBN]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_DATA_KBN] = value; }
		}
		/// <summary>タイトル</summary>
		public string HtmlTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_HTML_TITLE]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_HTML_TITLE] = value; }
		}
		/// <summary>キーワード</summary>
		public string MetadataKeywords
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_METADATA_KEYWORDS]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_METADATA_KEYWORDS] = value; }
		}
		/// <summary>ディスクリプション</summary>
		public string MetadataDesc
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_METADATA_DESC]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_METADATA_DESC] = value; }
		}
		/// <summary>コメント</summary>
		public string Comment
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_COMMENT]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_COMMENT] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_LAST_CHANGED] = value; }
		}
		/// <summary>SEO文言</summary>
		public string SeoText
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_SEO_TEXT]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_SEO_TEXT] = value; }
		}
		/// <summary>デフォルト文言</summary>
		public string DefaultText
		{
			get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_DEFAULT_TEXT]; }
			set { this.DataSource[Constants.FIELD_SEOMETADATAS_DEFAULT_TEXT] = value; }
		}
		#endregion
	}
}