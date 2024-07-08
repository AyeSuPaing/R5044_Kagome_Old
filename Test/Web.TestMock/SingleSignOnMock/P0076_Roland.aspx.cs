/*
=========================================================================================================
  Module      : シングルサインオンテスト用モック（P0076_Roland用）クラス(P0076_Roland.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// シングルサインオンテスト用モック（P0076_Roland用）
/// </summary>
public partial class P0076_Roland : System.Web.UI.Page
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.NextUrl = this.Context.Request["nurl"];
		this.MId = "mid-275";
		this.TimeStamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
		this.CheckCode = CreateCheckCode(string.Join("^||^", this.NextUrl, this.MId, this.TimeStamp));
		this.Nickname = "ローランドのファンA";
		this.BirthYear = "1992";
		this.BirthMonth = "8";
		this.BirthDay = "4";
		this.Sex = "0";
		this.Pref = "1";
		this.MailAddr = "roland.test275@w2solution.co.jp";
		this.Tel1 = "080";
		this.Tel2 = "1234";
		this.Tel3 = "5678";
	}

	/// <summary>
	/// チェックコードを作成(SHA256)
	/// </summary>
	/// <param name="stringToHash">ハッシュ化する文字列</param>
	/// <returns>SHA256でハッシュ化した文字列</returns>
	protected static string CreateCheckCode(string stringToHash)
	{
		var bytes = Encoding.UTF8.GetBytes(stringToHash);
		var numArray = new SHA256CryptoServiceProvider().ComputeHash(bytes);

		var hashedText = new StringBuilder();
		foreach (var num in numArray)
		{
			hashedText.AppendFormat("{0:X2}", num);
		}
		return hashedText.ToString();
	}

	/// <remarks>ログイン後遷移先URL</remarks>
	public string NextUrl { get; set; }
	/// <summary>メンバID</summary>
	public string MId { get; set; }
	/// <summary>TimeStamp</summary>
	public string TimeStamp { get; set; }
	/// <summary>CheckCode</summary>
	public string CheckCode { get; set; }
	/// <summary>ニックネーム</summary>
	public string Nickname { get; set; }
	/// <summary>誕生年</summary>
	public string BirthYear { get; set; }
	/// <summary>誕生月</summary>
	public string BirthMonth { get; set; }
	/// <summary>誕生日</summary>
	public string BirthDay { get; set; }
	/// <summary>性別</summary>
	public string Sex { get; set; }
	/// <summary>都道府県</summary>
	public string Pref { get; set; }
	/// <summary>メールアドレス</summary>
	public string MailAddr { get; set; }
	/// <summary>電話番号1</summary>
	public string Tel1 { get; set; }
	/// <summary>電話番号2</summary>
	public string Tel2 { get; set; }
	/// <summary>電話番号3</summary>
	public string Tel3 { get; set; }
}