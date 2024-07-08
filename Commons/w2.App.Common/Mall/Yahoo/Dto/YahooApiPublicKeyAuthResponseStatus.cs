/*
=========================================================================================================
  Module      : Yahoo API 公開鍵認証レスポンスステータス(YahooApiPublicKeyAuthResponseStatus.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Helper.Attribute;

namespace w2.App.Common.Mall.Yahoo.Dto
{
	/// <summary>
	/// Yahoo API 公開鍵認証レスポンスステータス
	/// </summary>
	public enum YahooApiPublicKeyAuthResponseStatus
	{
		/// <summary>公開鍵認証に成功</summary>
		[EnumTextName("authorized")]
		Authorized,
		/// <summary>認証ヘッダー(X-sws-signature)が付与されていない</summary>
		[EnumTextName("none")]
		None,
		/// <summary>指定された公開鍵の有効期限切れ</summary>
		[EnumTextName("expired-key-version")]
		ExpiredKeyVersion,
		/// <summary>認証キーバージョンヘッダー(X-sws-signature-version)が不正</summary>
		[EnumTextName("invalid-key-version")]
		InvalidKeyVersion,
		/// <summary> 指定された公開鍵バージョンが存在しない</summary>
		[EnumTextName("not-found-key-version")]
		NotFoundKeyVersion,
		/// <summary>指定された公開鍵のステータスが不正</summary>
		[EnumTextName("invalid-key-status")]
		InvalidKeyStatus,
		/// <summary>認証ヘッダー(X-sws-signature)に付与されているtimestampが有効期限切れ</summary>
		[EnumTextName("expired-timestamp")]
		ExpiredTimestamp,
		/// <summary>認証ヘッダー(X-sws-signature)に付与されているtimestampの値が不正</summary>
		[EnumTextName("invalid-timestamp")]
		InvalidTimestamp,
		/// <summary>認証ヘッダー(X-sws-signature)に付与されているSellerIdの値が不正</summary>
		[EnumTextName("invalid-sellerid")]
		InvalidSellerId,
		/// <summary>認証ヘッダー(X-sws-signature)のフォーマットが不正</summary>
		[EnumTextName("invalid-authorize-value")]
		InvalidAuthorizeValue,
		/// <summary>公開鍵認証に失敗</summary>
		[EnumTextName("unauthorized")]
		Unauthorized,
	}
}
