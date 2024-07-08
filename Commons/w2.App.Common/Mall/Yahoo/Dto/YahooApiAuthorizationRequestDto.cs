/*
=========================================================================================================
  Module      : YAHOO API AuthorizationエンドポイントAPI リクエストDTO クラス(YahooApiAuthorizationRequestDto.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Helper;
using w2.Common.Helper.Attribute;

namespace w2.App.Common.Mall.Yahoo.Dto
{
	/// <summary>
	/// YAHOO API AuthorizationエンドポイントAPI リクエストDTO クラス
	/// </summary>
	internal class YahooApiAuthorizationRequestDto
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseType">返却値のタイプ</param>
		/// <param name="clientId">クライアントID</param>
		/// <param name="redirectUri">リダイレクトURI</param>
		/// <param name="bail">規約に同意しない時の挙動フラグ</param>
		/// <param name="scope">スコープ</param>
		/// <param name="state">ステート</param>
		/// <param name="nonce">ナンス</param>
		/// <param name="display">ページ種類</param>
		/// <param name="prompt">ユーザーに強制させたいアクション</param>
		/// <param name="maxAge">最大認証経過時間</param>
		/// <param name="codeChallenge">code_challenge(認可コード横取り攻撃対策のパラメータ。)</param>
		/// <param name="codeChallengeMethod">code_challenge_method(認可コード横取り攻撃対策のパラメータ)</param>
		public YahooApiAuthorizationRequestDto(
			string responseType,
			string clientId,
			string redirectUri,
			string bail,
			string scope,
			string state,
			string nonce,
			string display,
			string prompt,
			string maxAge,
			string codeChallenge,
			string codeChallengeMethod)
		{
			this.ResponseType = responseType;
			this.ClientId = clientId;
			this.RedirectUri = redirectUri;
			this.Bail = bail;
			this.Scope = scope;
			this.State = state;
			this.Nonce = nonce;
			this.Display = display;
			this.Prompt = prompt;
			this.MaxAge = maxAge;
			this.CodeChallenge = codeChallenge;
			this.CodeChallengeMethod = codeChallengeMethod;
		}

		/// <summary>
		/// AuthorizationエンドポイントAPI用のURL生成
		/// </summary>
		/// <param name="endpoint">エンドポイント</param>
		/// <returns>URL</returns>
		public string GenerateUrl(string endpoint)
		{
			var url = $"{endpoint}?{QueryStringHelper.GenerateQueryString(obj: this)}";
			return url;
		}

		/// <summary>返却値のタイプ</summary>
		[QueryStringVariableName("response_type")]
		public string ResponseType { get; set; }
		/// <summary>クライアントID</summary>
		[QueryStringVariableName("client_id")]
		public string ClientId { get; set; }
		/// <summary>リダイレクトURI</summary>
		[QueryStringVariableName("redirect_uri")]
		public string RedirectUri { get; set; }
		/// <summary>規約に同意しない時の挙動フラグ</summary>
		[QueryStringVariableName("bail")]
		public string Bail { get; set; }
		/// <summary>スコープ</summary>
		[QueryStringVariableName("scope")]
		public string Scope { get; set; }
		/// <summary>ステート</summary>
		[QueryStringVariableName("state")]
		public string State { get; set; }
		/// <summary>ナンス</summary>
		[QueryStringVariableName("nonce")]
		public string Nonce { get; set; }
		/// <summary>ページ種類</summary>
		[QueryStringVariableName("display")]
		public string Display { get; set; }
		/// <summary>ユーザーに強制させたいアクション</summary>
		[QueryStringVariableName("prompt")]
		public string Prompt { get; set; }
		/// <summary>最大認証経過時間</summary>
		[QueryStringVariableName("max_age")]
		public string MaxAge { get; set; }
		/// <summary>code_challenge (認可コード横取り攻撃対策のパラメータ。)</summary>
		[QueryStringVariableName("code_challenge")]
		public string CodeChallenge { get; set; }
		/// <summary>code_challenge_method (認可コード横取り攻撃対策のパラメータ)</summary>
		[QueryStringVariableName("code_challenge_method")]
		public string CodeChallengeMethod { get; set; }
	}
}
