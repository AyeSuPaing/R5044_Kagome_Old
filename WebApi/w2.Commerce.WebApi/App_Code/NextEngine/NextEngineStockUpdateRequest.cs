/*
=========================================================================================================
  Module      : ネクストエンジン在庫連携API リクエスト (NextEngineStockUpdateRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using w2.Common.Helper;
using w2.Common.Helper.Attribute;

/// <summary>
/// ネクストエンジン在庫連携API リクエスト
/// </summary>
public class NextEngineStockUpdateRequest
{
	/// <summary>パラメータ名</summary>
	public enum NextEngineStockUpdateRequestQueryParam
	{
		/// <summary>ストアアカウント</summary>
		[EnumTextName("StoreAccount")] StoreAccount,
		/// <summary>商品コード</summary>
		[EnumTextName("Code")] Code,
		/// <summary>在庫数</summary>
		[EnumTextName("Stock")] Stock,
		/// <summary>日付</summary>
		[EnumTextName("ts")] Ts,
		/// <summary>署名コード</summary>
		[EnumTextName(".sig")] Sig,
	}

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="request">リクエスト内容</param>
	public NextEngineStockUpdateRequest(HttpRequest request)
	{
		this.StoreAccount = request[NextEngineStockUpdateRequestQueryParam.StoreAccount.ToText()];
		this.Code = request[NextEngineStockUpdateRequestQueryParam.Code.ToText()];
		this.Stock = request[NextEngineStockUpdateRequestQueryParam.Stock.ToText()];
		this.Ts = request[NextEngineStockUpdateRequestQueryParam.Ts.ToText()];
		this.Sig = request[NextEngineStockUpdateRequestQueryParam.Sig.ToText()];

		int stockNumTemp;
		if (int.TryParse(this.Stock, out stockNumTemp))
		{
			this.StockNum = stockNumTemp;
		}
	}

	/// <summary>
	/// 必須チェック
	/// </summary>
	/// <returns>結果</returns>
	public bool ChekcRequired()
	{
		var result =
			(string.IsNullOrEmpty(this.StoreAccount) == false)
			&& (string.IsNullOrEmpty(this.Code) == false)
			&& (string.IsNullOrEmpty(this.Ts) == false)
			&& (string.IsNullOrEmpty(this.Sig) == false);

		return result;
	}

	/// <summary>
	/// ストアアカウントチェック
	/// </summary>
	/// <param name="storeAccount">モール設定 ストアアカウント</param>
	/// <returns>結果</returns>
	public bool CheckStoreAccount(string storeAccount)
	{
		var result = (this.StoreAccount == storeAccount);
		return result;
	}

	/// <summary>
	/// 認証情報チェック
	/// </summary>
	/// <param name="key">モール設定 認証キー</param>
	/// <returns>結果</returns>
	public bool CheckSig(string key)
	{
		var parameters = new List<KeyValuePair<string, string>>()
		{
			new KeyValuePair<string, string>(
				NextEngineStockUpdateRequestQueryParam.StoreAccount.ToText(),
				this.StoreAccount),
			new KeyValuePair<string, string>(NextEngineStockUpdateRequestQueryParam.Code.ToText(), this.Code),
			new KeyValuePair<string, string>(NextEngineStockUpdateRequestQueryParam.Stock.ToText(), this.Stock),
			new KeyValuePair<string, string>(NextEngineStockUpdateRequestQueryParam.Ts.ToText(), this.Ts),
		};
		var sig = string.Join("&", parameters.Select(p => string.Format("{0}={1}", p.Key, p.Value)));
		var result = (this.Sig == ConvertMd5Hash(sig + key));
		return result;
	}

	/// <summary>
	/// MD5ハッシュ生成
	/// </summary>
	/// <param name="value">生成元</param>
	/// <returns>結果</returns>
	private string ConvertMd5Hash(string value)
	{
		var byteData = Encoding.ASCII.GetBytes(value);
		var md5 = new MD5CryptoServiceProvider();
		var byteHash = md5.ComputeHash(byteData);
		var hash = new System.Text.StringBuilder();
		foreach (var b in byteHash)
		{
			hash.Append(b.ToString("x2"));
		}
		var result = hash.ToString();
		return result;
	}

	/// <summary>ストアアカウント</summary>
	public string StoreAccount { get; private set; }
	/// <summary>商品コード</summary>
	public string Code { get; private set; }
	/// <summary>在庫数</summary>
	public string Stock { get; private set; }
	/// <summary>在庫数 数値</summary>
	public int StockNum { get; private set; }
	/// <summary>実行日次</summary>
	public string Ts { get; private set; }
	/// <summary>認証情報</summary>
	public string Sig { get; private set; }
}