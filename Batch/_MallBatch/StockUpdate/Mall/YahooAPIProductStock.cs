/*
=========================================================================================================
  Module      : YahooAPI連携商品在庫情報格納クラス (YahooAPIProductStock.cs)
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

namespace w2.Commerce.MallBatch.StockUpdate.Mall
{
	///**************************************************************************************
	/// <summary>
	/// YahooAPI連携商品在庫情報格納クラス
	/// </summary>
	///**************************************************************************************
	class YahooAPIProductStock
	{
		/// <summary>商品ID(Code)</summary>
		public string ProductId { get; set; }

		/// <summary>バリエーションID(SubCode)</summary>
		public string VariationId { get; set; }

		/// <summary>結果(Processed)</summary>
		public string Processed { get; set; }
	}
}
