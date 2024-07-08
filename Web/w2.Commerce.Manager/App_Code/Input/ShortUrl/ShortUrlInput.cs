/*
=========================================================================================================
  Module      : ショートURL入力クラス (ShortUrlInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using w2.App.Common.Input;
using w2.Domain.ShortUrl;
using w2.Domain.ShortUrl.Helper;

/// <summary>
/// ショートURLマスタ入力クラス
/// </summary>
[Serializable]
public class ShortUrlInput : InputBase<ShortUrlModel>
{
	#region コンストラクタ

	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ShortUrlInput()
	{
	}

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="result">検索結果</param>
	public ShortUrlInput(ShortUrlListSearchResult result)
		: this()
	{
		this.SurlNo = result.SurlNo.ToString();
		this.ShopId = result.ShopId;
		this.ShortUrl = result.ShortUrl;
		this.LongUrl = result.LongUrl;
		this.DateCreated = result.DateCreated.ToString();
		this.DateChanged = result.DateChanged.ToString();
		this.LastChanged = result.LastChanged;
	}

	#endregion

	#region メソッド

	/// <summary>
	/// モデル作成
	/// </summary>
	/// <param name="register">登録?</param>
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
				{Constants.FIELD_SHORTURL_LONG_URL, this.LongUrlWithProtocolAndDomain}
			};
		var errorMessage = Validator.Validate(register ? "ShortUrlRegist" : "ShortUrlModify", input).Replace("@@ 1 @@", "No." + this.SurlNo);

		// ショートURL重複チェック
		if (shortUrlListForCheckDuplication
			.Where(s => (s.ShortUrl == this.ShortUrlNoProtocolAndDomain)
					&& (register
						|| (s.SurlNo != long.Parse(this.SurlNo)))).Any())
		{
			errorMessage += WebMessages.GetMessages(WebMessages.INPUTCHECK_DUPLICATION).Replace("@@ 1 @@", "No." + this.SurlNo + "のショートURL") + "<br/>";
		}

		// ショートURL拡張子チェック
		return errorMessage + CheckShortUrlExtenision();
	}

	/// <summary>
	/// ショートURL拡張子チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string CheckShortUrlExtenision()
	{
		var shortUrlParams = this.ShortUrlWithProtocolAndDomain.ToString().Split('/');
		var shortUrlFileName = shortUrlParams[shortUrlParams.Length - 1].Split('?')[0];
		if (shortUrlFileName.Contains("."))
		{
			var splitedFileNames = shortUrlFileName.Split('.');
			var ext = splitedFileNames[splitedFileNames.Length - 1];
			if (Constants.SHORTURL_DENY_EXTENSIONS.Contains(ext.ToLower()))
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHORTURL_INPUT_EXTENSION_ERROR).Replace("@@ 1 @@", this.LineNo).Replace("@@ 2 @@", ext);
			}
		}

		return "";
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

	/// <summary>列番号</summary>
	public string LineNo
	{
		get { return StringUtility.ToEmpty(this.DataSource["LineNo"]); }
		set { this.DataSource["LineNo"] = value; }
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

	#endregion
}