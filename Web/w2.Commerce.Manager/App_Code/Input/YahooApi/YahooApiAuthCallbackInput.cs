/*
=========================================================================================================
  Module      : YAHOO API AuthorizationエンドポイントAPIコールバック入力クラス (YahooApiAuthCallbackInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Logger;

/// <summary>
/// YAHOO API AuthorizationエンドポイントAPIコールバック入力
/// </summary>
public class YahooApiAuthCallbackInput
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public YahooApiAuthCallbackInput(){}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="accessToken">アクセストークン</param>
	/// <param name="tokenType">トークンタイプ</param>
	/// <param name="idToken">IDトークン</param>
	/// <param name="code">認可コード</param>
	/// <param name="state">ステート</param>
	/// <param name="error">エラーコード</param>
	/// <param name="errorDescription">エラー詳細</param>
	/// <param name="errorCode">エラー判定用コード</param>
	public YahooApiAuthCallbackInput(
		string accessToken,
		string tokenType,
		string idToken,
		string code,
		string state,
		string error,
		string errorDescription,
		string errorCode)
	{
		this.AccessToken = accessToken;
		this.TokenType = tokenType;
		this.IdToken = idToken;
		this.Code = code;
		this.State = state;
		this.Error = error;
		this.ErrorDescription = errorDescription;
		this.ErrorCode = errorCode;
	}

	/// <summary>
	/// ログ出力
	/// </summary>
	public void WriteLog()
	{
		var log = string.Format(
			"{{ \"yahoo_api_authorization_endpoint\": {{\"access_token\":\"{0}\", \"token_type\":\"{1}\", \"id_token\":\"{2}\", \"code\":\"{3}\", \"state\":\"{4}\" }} }}",
			this.AccessToken,
			this.TokenType,
			this.IdToken,
			this.Code,
			this.State);
		FileLogger.WriteDebug(log);
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <param name="state">ステート</param>
	/// <returns>検証結果</returns>
	public bool Validate(string state)
	{
		if (string.IsNullOrEmpty(this.Error) == false)
		{
			var err = string.Format(
				"AuthorizationエンドポイントAPIの実行に失敗しました。error={0},error_description={1},error_code{2}",
				this.Error,
				this.ErrorDescription,
				this.ErrorCode);
			FileLogger.WriteError(err);
			return false;
		}

		if (string.IsNullOrEmpty(this.Code))
		{
			FileLogger.WriteError("認可コードがありません。管理者へ問い合わせてください。");
			return false;
		}

		if (string.IsNullOrEmpty(state) || this.State != state)
		{
			FileLogger.WriteError("不正なアクセスです。管理者へ問い合わせてください。");
			return false;
		}
		return true;
	}

	/// <summary>アクセストークン</summary>
	public string AccessToken { get; private set; }
	/// <summary>トークンタイプ</summary>
	public string TokenType { get; private set; }
	/// <summary>IDトークン</summary>
	public string IdToken { get; private set; }
	/// <summary>認可コード</summary>
	public string Code { get; private set; }
	/// <summary>ステート</summary>
	public string State { get; private set; }
	/// <summary>エラー</summary>
	public string Error { get; private set; }
	/// <summary>エラー詳細</summary>
	public string ErrorDescription { get; private set; }
	/// <summary>エラー判定用コード</summary>
	public string ErrorCode { get; private set; }
}
