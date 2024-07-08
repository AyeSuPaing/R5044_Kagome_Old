/*
=========================================================================================================
  Module      : ショートURL入力モデル(NewsController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using w2.App.Common.Input;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Domain.ShortUrl;
using w2.Domain.ShortUrl.Helper;
using Validator = w2.Common.Util.Validator;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// ショートURL入力モデル
	/// </summary>
	[Serializable]
	public class ShortUrlInput : InputBase<ShortUrlModel>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ShortUrlInput()
		{
			this.SurlNo = "";
			this.ShopId = "";
			this.ShortUrl = "";
			this.LongUrl = "";
			this.DateCreated = "";
			this.DateChanged = "";
			this.LastChanged = "";
			this.OriginalShortUrl = this.ShortUrl;
			this.OriginalLongUrl = this.OriginalLongUrl;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public ShortUrlInput(ShortUrlModel model)
		{
			this.SurlNo = model.SurlNo.ToString();
			this.ShopId = model.ShopId;
			this.ShortUrl = model.ShortUrl;
			this.LongUrl = model.LongUrl;
			this.DateCreated = model.DateCreated.ToString();
			this.DateChanged = model.DateChanged.ToString();
			this.LastChanged = model.LastChanged;
			this.OriginalShortUrl = this.ShortUrl;
			this.OriginalLongUrl = this.LongUrl;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="result">一覧検索結果</param>
		public ShortUrlInput(ShortUrlListSearchResult result)
		{
			this.SurlNo = result.SurlNo.ToString();
			this.ShopId = result.ShopId;
			this.ShortUrl = result.ShortUrl;
			this.LongUrl = result.LongUrl;
			this.DateCreated = result.DateCreated.ToString();
			this.DateChanged = result.DateChanged.ToString();
			this.LastChanged = result.LastChanged;
			this.OriginalShortUrl = this.ShortUrl;
			this.OriginalLongUrl = this.LongUrl;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <param name="register">登録かどうか</param>
		/// <returns>モデル</returns>
		public ShortUrlModel CreateModel(bool register)
		{
			var model = CreateModel();
			if (register == false) model.SurlNo = long.Parse(this.SurlNo);
			return model;
		}

		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override ShortUrlModel CreateModel()
		{
			var model = new ShortUrlModel
			{
				ShopId = this.ShopId,
				ShortUrl = Regex.Replace(Regex.Replace(this.ShortUrlNoProtocolAndDomain, @"^https://", @"http://"), @"\/+$", @""),
				LongUrl = this.LongUrlNoProtocolAndDomain,
				LastChanged = this.LastChanged,
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="register">登録?</param>
		/// <param name="shortUrlListForCheckDuplication">ショートURLリスト取得（重複チェック用）</param>
		/// <returns>エラーメッセージ</returns>
		public string Validate(bool register, ShortUrlModel[] shortUrlListForCheckDuplication)
		{
			// 入力チェック
			var input = new Hashtable
			{
				{Constants.FIELD_SHORTURL_SHORT_URL, this.ShortUrlWithProtocolAndDomain},
				{Constants.FIELD_SHORTURL_LONG_URL, this.LongUrlWithProtocolAndDomain},
				{Constants.FIELD_SHORTURL_SHORT_URL+"_nec", this.ShortUrl},
				{Constants.FIELD_SHORTURL_LONG_URL+"_nec", this.LongUrl},
			};

			var errorMessage = string.Join("<br />", Validator.Validate(register ? "ShortUrlRegist" : "ShortUrlModify", input)
				.Select(s => s.Value.Replace("@@ 1 @@", "No." + this.SurlNo))
				.ToArray());

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				return errorMessage;
			}

			if (shortUrlListForCheckDuplication.Any(s => (s.ShortUrl == this.ShortUrlNoProtocolAndDomain) && (register || (s.SurlNo != long.Parse(this.SurlNo)))))
			{
				return WebMessages.InputCheckDuplication.Replace("@@ 1 @@", register ? this.ShortUrl : "No." + this.SurlNo + " " + this.ShortUrl) + "<br/>";
			}

			// ショートURL拡張子チェック
			return CheckShortUrlExtenision();
		}

		/// <summary>
		/// ショートURL拡張子チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string CheckShortUrlExtenision()
		{
			var shortUrlParams = this.ShortUrlWithProtocolAndDomain.Split('/');
			var shortUrlFileName = shortUrlParams[shortUrlParams.Length - 1].Split('?')[0];
			if (shortUrlFileName.Contains("."))
			{
				var splitedFileNames = shortUrlFileName.Split('.');
				var ext = splitedFileNames[splitedFileNames.Length - 1];
				if (Constants.SHORTURL_DENY_EXTENSIONS.Contains(ext.ToLower()))
				{
					return WebMessages.ShorturlInputExtensionError.Replace("@@ 1 @@", this.SurlNo).Replace("@@ 2 @@", ext);
				}
			}

			return "";
		}

		/// <summary>
		/// 変更があったかどうか
		/// </summary>
		/// <returns>
		/// True：変更あり
		/// False：変更なし
		/// </returns>
		public bool IsUrlChanged()
		{
			if (this.ShortUrl != this.OriginalShortUrl) return true;
			if (this.LongUrl != this.OriginalLongUrl) return true;
			return false;
		}
		#endregion

		#region プロパティ
		/// <summary>ショートURL NO</summary>
		public string SurlNo
		{
			get { return (string)this.DataSource[Constants.FIELD_SHORTURL_SURL_NO]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_SURL_NO] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHORTURL_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_SHOP_ID] = value; }
		}
		/// <summary>ショートURL</summary>
		public string ShortUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_SHORTURL_SHORT_URL]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_SHORT_URL] = value; }
		}
		/// <summary>ロングURL</summary>
		public string LongUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_SHORTURL_LONG_URL]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_LONG_URL] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_SHORTURL_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SHORTURL_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SHORTURL_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_LAST_CHANGED] = value; }
		}
		/// <summary>ショートURL（プロコトル+ドメイン付）</summary>
		public string ShortUrlWithProtocolAndDomain
		{
			get { return w2.App.Common.ShortUrl.ShortUrl.AddProtocolAndDomain(this.ShortUrl); }
		}
		/// <summary>ロングURL（プロコトル+ドメイン付）</summary>
		public string LongUrlWithProtocolAndDomain
		{
			get { return w2.App.Common.ShortUrl.ShortUrl.AddProtocolAndDomain(this.LongUrl); }
		}
		/// <summary>ショートURL（プロコトル+ドメインなし）</summary>
		public string ShortUrlNoProtocolAndDomain
		{
			get { return w2.App.Common.ShortUrl.ShortUrl.RemoveProtocolAndDomain(this.ShortUrl); }
		}
		/// <summary>ロングURL（プロコトル+ドメインなし）</summary>
		public string LongUrlNoProtocolAndDomain
		{
			get { return w2.App.Common.ShortUrl.ShortUrl.RemoveProtocolAndDomain(this.LongUrl); }
		}
		/// <summary>ショートURL(変更前)</summary>
		public string OriginalShortUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_SHORTURL_SHORT_URL + "_origin"]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_SHORT_URL + "_origin"] = value; }
		}
		/// <summary>ロングURL(変更前)</summary>
		public string OriginalLongUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_SHORTURL_LONG_URL + "_origin"]; }
			set { this.DataSource[Constants.FIELD_SHORTURL_LONG_URL + "_origin"] = value; }
		}
		#endregion
	}
}