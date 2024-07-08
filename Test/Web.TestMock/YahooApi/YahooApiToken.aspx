<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="false" ValidateRequest="False" %>
<%--
=========================================================================================================
  Module      : YAHOO API TokenエンドポイントAPI モッククラス(YahooApiToken.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<script runat="server">
	void Main()
	{
		// リクエスト値の取得
		var queryStringValues = RetrieveRequestValues();

		// エラーを返す
		// NOTE: https://developer.yahoo.co.jp/yconnect/v2/authorization_code/token.html
		if (queryStringValues.ContainsKey("provokes_error"))
		{
			var errorResponseValues = GenerateErrorResponseValues();
			Response.Clear();
			Response.ContentType = "application/x-www-form-urlencoded";
			Response.ContentEncoding = Encoding.UTF8;
			Response.Write(errorResponseValues);
		}

		// リフレッシュトークンを新規取得する場合のレスポンス値生成
		// NOTE: https://developer.yahoo.co.jp/yconnect/v2/authorization_code/token.html
		var grantType = queryStringValues["grant_type"];
		var queryString = "";
		if(grantType == "authorization_code")
		{
			queryString = GenerateResponseValues(
				accessToken: "SlAV32hkKG",
				tokenType: "Bearer",
				refreshToken: "8xLOxBtZp8",
				expiresIn: "3600",
				idToken: "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IjBjYzE3NWI5YzBmMWI2YTgzMWMzOTllMjY5NzcyNjYxIn0.eyJpc3MiOiJodHRwczpcL1wvYXV0aC5sb2dpbi55YWhvby5jby5qcFwveWNvbm5lY3RcL3YyIiwic3ViIjoiU0tBR1FOVlZTVDZYS0c2REFDUlFISFhETUkiLCJhdWQiOlsiZGowMGFpWnBQV013TlhBMlFrdEZXRWx2VENaelBXTnZibk4xYldWeWMyVmpjbVYwSm5nOU4yRS0iXSwiZXhwIjoxNjgwMjU1MDM4LCJpYXQiOjE2Nzc4MzU4MzgsImFtciI6WyJwd2QiXSwibm9uY2UiOiJuLTBTNl9XekEyTWoiLCJjX2hhc2giOiJrQW5ReFdEVGdhSTZOTTVjempoNk1RIn0.o2JJGFuixgD3C9KXhx0wCJykRsjHP5h7pUeRPu4vdbve87dtJsbOlmwTMSYl0DC0-uvqXO1J0GNWIIY1z18fQm3szXWXzespHG9OB2ZWbqDOVHdoGIbBUmaQMR5I5Q50aw6Fqk5gMSpZuXri5ltb7TZyztE_PTZl1uB0wsmNBOuojCq5XZjjRHPhO_R5ZEjwvFfvJiSwViyoQSiYKmhCr7W66qVldO9XjFPpx5VUsCrLDDnPcXpUCHYmRikbQbge2OTiR4SeI7Tjkxe5Xp6IfTWq5NjxAYZDOB__2XbxJ8g3c3toEFgTBdeZoE3LgwQ2t1T1Zmw6b6HU9YuuYsuINA");
		}

		// リフレッシュトークンを更新する場合のレスポンス値生成
		if(grantType == "refresh_token")
		{
			queryString = GenerateResponseValues(accessToken: "SlAV32hkKG", tokenType: "Bearer", expiresIn: "3600");
		}

		// レスポンス返却
		Response.Clear();
		Response.ContentType = "application/x-www-form-urlencoded";
		Response.ContentEncoding = Encoding.UTF8;
		Response.Write(queryString);
	}

	/// <summary>
	/// リクエスト値の取得
	/// </summary>
	/// <returns>リクエスト値</returns>
	private Dictionary<string, string> RetrieveRequestValues()
	{
		var values = new Dictionary<string, string>();
		foreach (var key in Request.Form.AllKeys)
		{
			values.Add(key, Request.Form[key]);
		}
		return values;
	}

	/// <summary>
	/// レスポンス値生成
	/// </summary>
	/// <param name="accessToken">アクセストークン</param>
	/// <param name="tokenType">トークンタイプ</param>
	/// <param name="expiresIn">有効時間</param>
	/// <returns>レスポンス値</returns>
	private string GenerateResponseValues(
		string accessToken,
		string tokenType,
		string expiresIn)
	{
		var resValues = new Dictionary<string, string>
		{
			{ "access_token", accessToken },
			{ "token_type", tokenType },
			{ "expires_in", expiresIn },
		};
		var queryString = Join(resValues);
		return queryString;
	}
	/// <summary>
	/// レスポンス値生成
	/// </summary>
	/// <param name="accessToken">アクセストークン</param>
	/// <param name="tokenType">トークンタイプ</param>
	/// <param name="expiresIn">有効時間</param>
	/// <param name="refreshToken">リフレッシュトークン</param>
	/// <param name="idToken">ID TOKEN</param>
	/// <returns>レスポンス値</returns>
	private string GenerateResponseValues(
		string accessToken,
		string tokenType,
		string refreshToken,
		string expiresIn,
		string idToken)
	{
		var resValues = new Dictionary<string, string>
		{
			{ "access_token", accessToken },
			{ "token_type", tokenType },
			{ "refresh_token", refreshToken },
			{ "expires_in", expiresIn },
			{ "id_token", idToken },
		};
		var queryString = Join(resValues);
		return queryString;
	}

	/// <summary>
	/// エラーレスポンス値生成
	/// </summary>
	/// <returns>エラーレスポンス値</returns>
	private string GenerateErrorResponseValues()
	{
		var resValues = new Dictionary<string, string>
		{
			{ "error", "invalid_request" },
			{ "error_description", "Unsupported response_type value" },
			{ "error_code", "1000" },
		};
		var queryString = Join(resValues);
		return queryString;
	}

	/// <summary>
	/// レスポンス値の結合
	/// </summary>
	/// <param name="resValues">レスポンス値</param>
	/// <returns>結合したレスポンス値</returns>
	private string Join(Dictionary<string, string> resValues)
	{
		var queryString = "#" + string.Join(
			"&",
			resValues.Select(v => string.Format("{0}={1}",v.Key, HttpUtility.UrlEncode(v.Value, Encoding.UTF8))));
		return queryString;
	}
</script>
<% this.Main(); %>
