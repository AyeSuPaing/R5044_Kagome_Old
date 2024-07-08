/*
=========================================================================================================
  Module      : 楽天IDConnect連携モック処理(AuthRakutenIDConnectMock.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml.Linq;
using w2.Common.Util;
using w2.Common.Web;

public partial class Auth_Mock_AuthRakutenIDConnectMock : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// モック検証無効の場合は、何も返さない
			if (string.IsNullOrEmpty(Constants.RAKUTEN_ID_CONNECT_MOCK_URL))
			{
				Response.ContentType = "text/javascript";
				Response.Output.Write("");
				Response.End();
			}

			var user = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"_Dev\Rakuten\Xml\ProvideUser.xml")
				.Element("ProvideUser");
			this.FamilyName = user.Element("FamilyName").Value;
			this.FamilyNameKana = user.Element("FamilyNameKana").Value;
			this.GivenName = user.Element("GivenName").Value;
			this.GivenNameKana = user.Element("GivenNameKana").Value;
			this.Nickname = user.Element("Nickname").Value;
			this.Gender = user.Element("Gender").Value;
			this.BirthDate = user.Element("BirthDate").Value;
			this.Email = user.Element("Email").Value;
			this.PhoneNumber = user.Element("PhoneNumber").Value;
			this.Country = user.Element("Country").Value;
			this.PostalCode = user.Element("PostalCode").Value;
			this.Region = user.Element("Region").Value;
			this.Locality = user.Element("Locality").Value;
			this.StreetAddress = user.Element("StreetAddress").Value;
			this.Formatted = this.Region + this.Locality + this.StreetAddress;

			switch (this.Type)
			{
				case "register":
					break;
				case "authorize":
					RedirectAuth();
					break;
				case "tokens":
					OutPutTokens();
					break;
				case "userinfo":
					OutPutUserInfo();
					break;
			}
		}
	}

	/// <summary>
	/// 認証成功時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbOk_OnClick(object sender, EventArgs e)
	{
		RedirectAuth();
	}

	/// <summary>
	/// 認証失敗時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCancel_OnClick(object sender, EventArgs e)
	{
		RedirectAuth(false);
	}

	#region メソッド
	/// <summary>
	/// 楽天IDConnect連携画面へ遷移
	/// </summary>
	/// <param name="isSuccess">認証成功か</param>
	private void RedirectAuth(bool isSuccess = true)
	{
		var url = new UrlCreator("https://localhost/R5044_Kagome.Develop/Web/w2.Commerce.Front/" + Constants.PAGE_FRONT_AUTH_RAKUTEN_ID_CONNECT)
			.AddParam("code", isSuccess ? "781a6302cf1cbe9785a218dca1c6224ac10361135be435c6" : "")
			.AddParam("state", this.State)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// トークン情報出力
	/// </summary>
	private void OutPutTokens()
	{
		var responseString = @"{""access_token"":""baa514dc4c4f44155f8c649aaed419b3af9b9be54a278025"",""id_token"":""eyJhbGciOiJSUzI1NiIsImtpZCI6ImM5YzliMGU0OWQ3YWJlODI0NDM5NzBkYzNlN2YxY2M4Y2ViZGE0MzcifQ.eyJqdGkiOiJQMmt1OGp2WGZnU0lxLXBQZG05QkFRIiwiaXNzIjoiaHR0cHM6Ly9hY2NvdW50cy5pZC5yYWt1dGVuLmNvLmpwLyIsInN1YiI6ImQ4MzNmNjFkLTBiMGEtNGI5Zi1hNzhkLTNmYTM1YzExNDdmOSIsImF1ZCI6IjZiYWZlYTdmLTJjOWItNDZiNC04OGQ2LTA5MjkyYjg5NjY3YSIsImlhdCI6MTUxNDM2Mjc5NSwibm9uY2UiOiIqKioqKioqKiIsIm9wZW5pZF9pZCI6Imh0dHBzOi8vbXlpZC5yYWt1dGVuLmNvLmpwL29wZW5pZC91c2VyL2RDWXpIc2Fsc3pScUhhZ0NyWU0zUDV5amNRPT0iLCJleHAiOjE1MTQzNjMwOTV9.JwbvU5TO7O4tMAgel4yV2HF5y0EvG2lN1nhQluoBnApR-H28_4h-H4p9rFCbEob3K-7DBHpjldoKzzotdUn_q__QpwqSOpWP8rL2vIfJX2gTA4bmQxD6Ejl3igsmBkCWO-LcWjNqiNiOq86U3rbPhRaFc1N7A08E4SCfxVbkUvM_2RTVhpfuvZSG7rS8dnWRqhtDTSX-IcTQACUBvSkoGUyv2j5UEDhh1e1qDg3uo1V96AXY9M6dK2xbuZ7QEqU9tYNsKgMSXzSgbBN2Gi9Y6bCL0zUOCUIPoJ_uMKC5qp9fWmp5gWhtpC6Er9-kKRiP9UnjomKJXHy2ld_FoV7AJw"",""token_type"":""Bearer"",""expires_in"":300}";
		Response.ContentType = "text/javascript";
		Response.Output.Write(responseString);
		Response.End();
	}

	/// <summary>
	/// 楽天会員情報出力
	/// </summary>
	private void OutPutUserInfo()
	{
		// 楽天IDConnect会員登録を確認したいときに変更する
		var openId = "https://myid.rakuten.co.jp/openid/user/dCYzHsalszRqHagCrYM3P5yjcQ==";
		var responseString = @"{""sub"":""d833f61d-0b0a-4b9f-a78d-3fa35c1147f9"
			+ @""",""family_name_kana"":""" + this.FamilyNameKana
			+ @""",""address"":{""country"":""" + this.Country
			+ @""",""street_address"":""" + this.StreetAddress
			+ @""",""formatted"":""" + this.Formatted
			+ @""",""locality"":""" + this.Locality
			+ @""",""region"":""" + this.Region
			+ @""",""postal_code"":""" + this.PostalCode + @"""}"
			+ @",""birthdate"":""" + this.BirthDate
			+ @""",""gender"":""" + this.Gender
			+ @""",""openid_id"":""" + openId
			+ @""",""family_name#ja-Kana-JP"":""" + this.FamilyNameKana
			+ @""",""given_name"":""" + this.GivenName
			+ @""",""nickname"":""" + this.Nickname
			+ @""",""phone_number"":""" + this.PhoneNumber
			+ @""",""given_name_kana"":""" + this.GivenNameKana
			+ @""",""family_name"":""" + this.FamilyName
			+ @""",""given_name#ja-Kana-JP"":""" + this.GivenNameKana
			+ @""",""email"":""" + this.Email + @"""}";
		Response.ContentType = "text/javascript";
		Response.Output.Write(responseString);
		Response.End();
	}
	#endregion

	#region プロパティ
	/// <summary>種別</summary>
	public string Type
	{
		get { return StringUtility.ToEmpty(Request["type"]); }
	}
	/// <summary>状態</summary>
	public string State
	{
		get { return StringUtility.ToEmpty(Request["state"]); }
	}
	/// <summary>氏</summary>
	protected string FamilyName { get; private set; }
	/// <summary>氏（カナ）</summary>
	protected string FamilyNameKana { get; private set; }
	/// <summary>名</summary>
	protected string GivenName { get; private set; }
	/// <summary>名（カナ）</summary>
	protected string GivenNameKana { get; private set; }
	/// <summary>ニックネーム</summary>
	protected string Nickname { get; private set; }
	/// <summary>性別</summary>
	protected string Gender { get; private set; }
	/// <summary>生年月日</summary>
	protected string BirthDate { get; private set; }
	/// <summary>メールアドレス</summary>
	protected string Email { get; private set; }
	/// <summary>電話番号</summary>
	protected string PhoneNumber { get; private set; }
	/// <summary>国</summary>
	protected string Country { get; private set; }
	/// <summary>郵便番号</summary>
	protected string PostalCode { get; private set; }
	/// <summary>都道府県</summary>
	protected string Region { get; private set; }
	/// <summary>群市区</summary>
	protected string Locality { get; private set; }
	/// <summary>郡市区以降の住所</summary>
	protected string StreetAddress { get; private set; }
	/// <summary>住所(全て)</summary>
	protected string Formatted { get; private set; }
	#endregion
}