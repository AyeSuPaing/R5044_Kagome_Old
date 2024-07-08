/*
=========================================================================================================
  Module      : 商品カラーキャッシュコントローラー(ProductColorCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Linq;
using System.Linq;
using System.IO;
using w2.App.Common.RefreshFileManager;
using w2.App.Common.Product;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 商品カラーキャッシュコントローラー
	/// </summary>
	public class ProductColorCacheController : DataCacheControllerBase<ProductColor[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ProductColorCacheController()
			: base(RefreshFileType.ProductColor)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = XDocument.Load(Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, Constants.FILEPATH_XML_COLORS)).Element("ProductColors").Elements("ProductColor")
				.Select(colorDataXml =>
					new ProductColor
					{
						Id = colorDataXml.Attribute("id").Value,
						Filaname = colorDataXml.Attribute("filename").Value,
						Url = Path.Combine(Path.Combine(Constants.PATH_ROOT_FRONT_PC, Constants.PATH_IMAGES_COLOR), colorDataXml.Attribute("filename").Value),
						DispName = colorDataXml.Attribute("dispname").Value,
					}
				).ToArray();
		}

		/// <summary>
		/// 商品カラーリストを取得
		/// </summary>
		/// <returns>商品カラーリスト</returns>
		public ProductColor[] GetProductColorList()
		{
			return this.CacheData;
		}
	}
}
