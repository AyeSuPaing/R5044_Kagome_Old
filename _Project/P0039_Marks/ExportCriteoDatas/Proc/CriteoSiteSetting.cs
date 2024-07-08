/*
=========================================================================================================
  Module      : Criteo連携用サイト設定(CriteoModelCreator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.Batch.ExportCriteoDatas.Proc
{	
	/// <summary>
	/// Criteo連携用サイト設定
	/// </summary>
	public class CriteoSiteSetting
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CriteoSiteSetting() : this("","","","","")
		{}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="fileName">出力ファイル名</param>
		/// <param name="dirName">出力先ディレクトリ名</param>
		/// <param name="siteRoot">サイトROOTパス</param>
		/// <param name="productPage">商品ページパス</param>
		/// <param name="productImage">商品画像パス</param>
		public CriteoSiteSetting(string fileName, string dirName, string siteRoot, string productPage, string productImage)
		{
			this.ExportFileName = fileName;
			this.ExportDirName = dirName;
			this.SiteRoot = siteRoot;
			this.ProductPagePath = productPage;
			this.ProductImagePath = productImage;
		}

		/// <summary>出力ファイル名</summary>
		public string ExportFileName { get; private set; }
		/// <summary>出力先ディレクトリ名</summary>
		public string ExportDirName { get; private set; }
		/// <summary>サイトROOTパス</summary>
		public string SiteRoot { get; private set; }
		/// <summary>商品ページパス</summary>
		public string ProductPagePath { get; private set; }
		/// <summary>商品画像パス</summary>
		public string ProductImagePath { get; private set; }
		/// <summary>出力先完全ファイルパス</summary>
		public string ExportPath { get { return this.ExportDirName + ExportFileName; } }
	}
}
