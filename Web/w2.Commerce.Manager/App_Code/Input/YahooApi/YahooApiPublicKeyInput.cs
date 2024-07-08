/*
=========================================================================================================
  Module      : Yahoo API公開鍵入力クラス(YahooApiPublicKeyInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common;
/// <summary>
/// Yahoo API公開鍵入力クラス
/// </summary>
public class YahooApiPublicKeyInput
{
	/// <summary>Yahoo API公開鍵フォーマット</summary>
	private const string _PUBLIC_KEY_FORMAT = @"-----BEGIN PUBLIC KEY-----(?:[A-Za-z0-9/+=\s]+)-----END PUBLIC KEY-----";

	/// <summary>
	/// コンストラクタ
	/// </summary>
	public YahooApiPublicKeyInput() { }
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="yahooApiPublicKey">Yahoo API公開鍵</param>
	/// <param name="yahooApiPublicKeyVersion">Yahoo API公開鍵バージョン</param>
	public YahooApiPublicKeyInput(string yahooApiPublicKey, string yahooApiPublicKeyVersion)
	{
		this.YahooApiPublicKey = yahooApiPublicKey;
		this.YahooApiPublicKeyVersion = yahooApiPublicKeyVersion;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>検証結果</returns>
	public string Validate()
	{
		var errorMessages = new StringBuilder();

		var match = Regex.Match(this.YahooApiPublicKey, _PUBLIC_KEY_FORMAT);
		if ((string.IsNullOrEmpty(this.YahooApiPublicKey) == false) && (match.Success == false))
		{
			errorMessages.Append(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_YAHOO_API_PUBLIC_KEY_INPUT_CHECK_FAILED));
		}

		int version;
		if ((string.IsNullOrEmpty(this.YahooApiPublicKeyVersion) == false)
			&& (int.TryParse(YahooApiPublicKeyVersion, out version) == false))
		{
			errorMessages.Append(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_YAHOO_API_PUBLIC_KEY_VERSION_INPUT_CHECK_FAILED));
		}
		
		return errorMessages.ToString();
	}

	/// <summary>Yahoo API公開鍵</summary>
	public string YahooApiPublicKey { get; set; }
	/// <summary>Yahoo API公開鍵バージョン</summary>
	public string YahooApiPublicKeyVersion { get; set; }
}
