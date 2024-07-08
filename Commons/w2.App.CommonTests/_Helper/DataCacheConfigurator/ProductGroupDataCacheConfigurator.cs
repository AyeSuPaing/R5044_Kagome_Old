using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.ProductGroup;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// 商品グループデータキャッシュ設定クラス
	/// </summary>
	internal class ProductGroupDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ProductGroupDataCacheConfigurator(ProductGroupModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// 商品グループデータキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">商品グループデータ</param>
		private void RefreshDataCache(ProductGroupModel[] data)
		{
			var productGroupServiceMock = new Mock<IProductGroupService>();
			productGroupServiceMock.Setup(s => s.GetAllProductGroup()).Returns(data);
			Domain.DomainFacade.Instance.ProductGroupService = productGroupServiceMock.Object;

			DataCacheControllerFacade.GetProductGroupCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new ProductGroupModel[0]);
		}
	}
}