/*
=========================================================================================================
  Module      : Criteoモデル(CriteoModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.Common.Util;

namespace w2.Commerce.Batch.ExportCriteoDatas.Proc
{
	/// <summary>
	/// Criteoモデル
	/// </summary>
	public class CriteoModel
	{
		/// <summary>禁則文字</summary>
		private const string PROHIBITE_CHARS = "<>,①②③④⑤⑥⑦⑧⑨⑩⑪⑫⑬⑭⑮⑯⑰⑱⑲⑳㍉㎜㎝㎞㎎㎏㏄㍉㌔㌢㍍㌘㌧㌃㌶㍑㍗㌍㌦㌣㌫㍊㌻№㏍℡㊤㊥㊦㊧㊨㈱㈲㈹㍾㍽㍼㍻ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩ";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="product">商品モデル</param>
		/// <param name="siteSetting">サイト設定</param>
		public CriteoModel(ProductModel product, CriteoSiteSetting siteSetting)
		{
			this.Product = product;
			this.SiteSetting = siteSetting;
		}

		/// <summary>
		/// 連携用フォーマットで出力
		/// </summary>
		/// <returns>連携用フォーマット</returns>
		public override string ToString()
		{
			return string.Join(",", 
				new string[]
				{
					this.Id,								// 商品ID(必須)
					FormatCriteoString(this.Name),			// 名前(必須)
					this.ProductUrl,						// リンク先URL(必須)
					this.BigImage,							// 画像URL(必須)
					FormatCriteoString(this.CategoryId1),	// カテゴリ名1(必須)
					FormatCriteoString(this.Description),   // 商品の詳細(必須)
					this.Price.ToPriceString(),				// 価格(必須)
					this.Instock,							// 在庫情報
					"",										// 小画像URL
					"",										// 推奨小売価格
					"",										// 値引き
					this.Recommendable,						// リコメンドの可否
					FormatCriteoString(this.CategoryId2),	// カテゴリ名2
					FormatCriteoString(this.CategoryId3)	// カテゴリ名3
				}.Select(item => EscapeString(item)));
		}

		/// <summary>
		/// エスケープ処理をする
		/// </summary>
		/// <param name="value">値</param>
		/// <returns>エスケープ処理済み文字列</returns>
		private string EscapeString(string value)
		{
			return PROHIBITE_CHARS.Aggregate(value, (acc, chr) => acc.Replace(chr, ' '));
		}

		/// <summary>
		/// Criteo用文字列フォーマット 「""」で囲み改行を削除
		/// </summary>
		/// <param name="value">値</param>
		/// <returns>ダブルクォーテーションで囲み改行を削除した文字列</returns>
		private string FormatCriteoString(string value)
		{
			return string.Format("\"{0}\"", value).Replace(Environment.NewLine, "");
		}

		/// <summary>画像フッター</summary>
		protected string ImageFooter { get { return Constants.PRODUCTIMAGE_FOOTER_L; } }

		#region プロパティ
		/// <summary>商品ID</summary>
		public string Id { get { return this.Product.ProductId; } }
		/// <summary>名前</summary>
		public string Name { get { return this.Product.Name; } }
		/// <summary>リンク先URL</summary>
		/// <remarks>URLイメージ：http://ドメイン/Form/Product/ProductDetail.aspx?shop_id=0&pid=product_id&bid=brand_id1 </remarks>
		public string ProductUrl
		{
			get
			{
				return string.Concat(this.SiteSetting.SiteRoot, this.SiteSetting.ProductPagePath,
					"?shop=", Constants.CONST_DEFAULT_SHOP_ID,
					"&pid=", HttpUtility.UrlEncode(this.Product.ProductId),
					"&bid=", HttpUtility.UrlEncode(this.Product.BrandId1));
			}
		}
		/// <summary>画像URL</summary>
		/// <remarks>URLイメージ：http://ドメイン/contents/ProductImages/0/image_head_L.jpg </remarks>
		public string BigImage
		{
			get
			{
				return string.Concat(this.SiteSetting.SiteRoot, this.SiteSetting.ProductImagePath, this.Product.ImageHead, this.ImageFooter);
			}
		}
		/// <summary>カテゴリID1</summary>
		public string CategoryId1 { get { return this.Product.CategoryName1; } }
		/// <summary>カテゴリID2</summary>
		public string CategoryId2 { get { return this.Product.CategoryName2; } }
		/// <summary>カテゴリID3</summary>
		public string CategoryId3 { get { return this.Product.CategoryName3; } }
		/// <summary>商品の詳細</summary>
		public string Description { get { return this.Product.Catchcopy;} }
		/// <summary>価格</summary>
		public string Price { get { return DecimalUtility.DecimalRound (this.Product.DisplayPrice, DecimalUtility.Format.RoundDown).ToString(); } }
		/// <summary>バナー表示可否</summary>
		public string Recommendable { get { return this.Product.Recommendable; } }
		/// <summary>在庫の有無</summary>
		public string Instock { get { return this.Product.Instock; } }
		#endregion

		/// <summary>商品情報</summary>
		protected ProductModel Product;
		/// <summary>サイト設定</summary>
		protected CriteoSiteSetting SiteSetting;
	}
}