/*
=========================================================================================================
  Module      : YahooAPI連携クラス (YahooAPI.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using w2.Common.Logger;
using System.Collections.Generic;
using w2.Common.Util;

namespace w2.Commerce.MallBatch.StockUpdate.Mall
{
	///**************************************************************************************
	/// <summary>
	/// YahooAPI連携クラス
	/// </summary>
	///**************************************************************************************
	class YahooAPI
	{
		/// <summary>
		/// YahooAPI商品在庫情報取得
		/// </summary>
		/// <param name="strUrl">URL</param>
		/// <param name="strStoreId">店舗ID</param>
		/// <param name="strTs">有効期限</param>
		/// <param name="strHashKey">ハッシュキー</param>
		/// <returns>成功有無</returns>
		/// <remarks>YahooAPIを使用して全ての在庫情報を取得する</remarks>
		public static void GetProductStockYahooAPI(
			string strUrl,
			string strStoreId, 
			string strTs,
			string strHashKey
			)
		{
			// プロパティ初期化
			ProductStocks = new List<YahooAPIProductStock>();

			// 全商品在庫情報を取得するまで繰り返す
			int iRequestCount = 0;
			while (true)
			{
				// パラメータ作成
				StringBuilder sbParam = new StringBuilder();
				sbParam.Append("StoreAccount=").Append(strStoreId);
				sbParam.Append("&.ts=").Append(strTs);
				sbParam.Append("&StartIndex=").Append((iRequestCount * 100 + 1).ToString());

				// ハッシュ作成
				string strMD5 = BitConverter.ToString(CreateMD5AsBytes(sbParam.ToString() + strHashKey));
				StringBuilder sbSig = new StringBuilder();
				sbSig.Append("&.sig=").Append(strMD5.Replace("-", "").ToLower());

				// URL作成
				StringBuilder sbUrl = new StringBuilder();
				sbUrl.Append(strUrl).Append("?").Append(sbParam.ToString()).Append(sbSig.ToString());

				// ログ書き出し
				FileLogger.WriteDebug(sbUrl.ToString());

				// レスポンス取得
				string strResponse = GetResponse(sbUrl.ToString(), "EUC-JP");

				// XMLに読み取り
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(new StringReader(strResponse));

				// XMLノードから戻り値判別
				bool blResultXML = false;
				foreach (XmlNode xn1 in xmlDoc.SelectNodes("ShoppingGetStock/ResultSet"))
				{
					foreach (XmlNode xn2 in xn1.SelectNodes("Result"))
					{
						// 商品情報を格納する
						YahooAPIProductStock yahooAPIProductStock = new YahooAPIProductStock();
						yahooAPIProductStock.ProductId = xn2.SelectSingleNode("Code").InnerXml;
						yahooAPIProductStock.VariationId = (xn2.SelectSingleNode("SubCode") != null) ? xn2.SelectSingleNode("SubCode").InnerXml : "";
						yahooAPIProductStock.Processed = xn2.SelectSingleNode("Processed").InnerXml;
						
						// プロパティに格納する
						ProductStocks.Add(yahooAPIProductStock);

						// XML取得に成功
						if (StringUtility.ToEmpty(yahooAPIProductStock.ProductId) != "")
						{
							blResultXML = true;
						}
					}
				}
				if (blResultXML == false)
				{
					break;
				}

				iRequestCount++;
			}
		}

		/// <summary>
		/// YahooAPI在庫チェック
		/// </summary>
		/// <param name="strProductId">商品ID</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <returns>成功有無</returns>
		/// <remarks>前提条件：GetProductStockYahooAPI関数でYahoo!側商品情報を取得していること</remarks>
		public static bool CheckProductStockYahooAPI(
			string strProductId,
			string strVariationId
			)
		{
			foreach (YahooAPIProductStock yahooAPIProductStock in ProductStocks)
			{
				// XML取得に成功したかどうか
				if (yahooAPIProductStock.Processed == "0")
				{
					// 商品IDチェック
					if (strProductId == yahooAPIProductStock.ProductId)
					{
						// バリエーションあり
						if (strVariationId == yahooAPIProductStock.VariationId)
						{
							return true;
						}
						// バリエーションなし
						else if ((strProductId.ToLower() == strVariationId.ToLower()) 
							&& (yahooAPIProductStock.VariationId == ""))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// YahooAPI在庫更新処理
		/// </summary>
		/// <param name="strUrl">URL</param>
		/// <param name="strStoreId">店舗ID</param>
		/// <param name="strCode">店舗コード</param>
		/// <param name="strSubCode">サブコード</param>
		/// <param name="strStock">在庫数</param>
		/// <param name="strTs">有効期限</param>
		/// <param name="strHashKey">ハッシュキー</param>
		/// <returns>成功有無</returns>
		/// <remarks>YahooAPIを使用して在庫を更新する</remarks>
		public static bool UpdateProductStockYahooAPI(
			string strUrl,
			string strStoreId, 
			string strCode, 
			string strSubCode, 
			string strStock, 
			string strTs, 
			string strHashKey)
		{
			bool blIsReturn = false;

			// パラメータ作成
			StringBuilder sbParam = new StringBuilder();
			sbParam.Append("StoreAccount=").Append(strStoreId);
			sbParam.Append("&Code=").Append(strCode);
			if (strSubCode != strCode)
			{
				// バリエーションがある場合のみ追加
				sbParam.Append("&SubCode=").Append(strSubCode);
			}
			sbParam.Append("&Stock=").Append(strStock);
			sbParam.Append("&.ts=").Append(strTs);

			// ハッシュ作成
			string strMD5 = BitConverter.ToString(CreateMD5AsBytes(sbParam.ToString() + strHashKey));
			StringBuilder sbSig = new StringBuilder();
			sbSig.Append("&.sig=").Append(strMD5.Replace("-", "").ToLower());

			// URL作成
			StringBuilder sbUrl = new StringBuilder();
			sbUrl.Append(strUrl).Append("?").Append(sbParam.ToString()).Append(sbSig.ToString());

			// ログ書き出し
			FileLogger.WriteDebug(sbUrl.ToString());

			// レスポンス取得
			string strResponse = GetResponse(sbUrl.ToString(), "EUC-JP");

			// XMLに読み取り
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(new StringReader(strResponse));

			// 実行結果取得
			XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("Processed");
			if (xmlNodeList.Count == 1)
			{
				// 成功なら
				if (xmlNodeList[0].InnerXml == "0")
				{
					blIsReturn = true;
				}
			}

			return blIsReturn;
		}

		/// <summary>
		/// MD5暗号化
		/// </summary>
		/// <param name="strValue">文字列</param>
		private static byte[] CreateMD5AsBytes(string strValue)
		{
			MD5 md5 = new MD5CryptoServiceProvider();
			return md5.ComputeHash(Encoding.ASCII.GetBytes(strValue));
		}

		/// <summary>
		/// レスポンス取得
		/// </summary>
		/// <param name="strUrl">リクエストURL</param>
		/// <param name="strEncode">エンコード</param>
		/// <returns>レスポンス</returns>
		private static string GetResponse(string strUrl, string strEncode)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(strUrl);
			using (Stream stream = httpWebRequest.GetResponse().GetResponseStream())
			using (StreamReader streamReader = new StreamReader(stream, Encoding.GetEncoding(strEncode)))
			{
				return streamReader.ReadToEnd();
			}
		}

		/// <summary>Yahoo!全商品在庫情報</summary>
		public static List<YahooAPIProductStock> ProductStocks { get; private set; }
	}
}
