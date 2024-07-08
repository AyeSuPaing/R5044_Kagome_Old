/*
=========================================================================================================
  Module      : 商品グループ設定キャッシュコントローラ(ProductGroupCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.ProductGroup;

namespace w2.App.Common.DataCacheController
{
	public class ProductGroupCacheController : DataCacheControllerBase<ProductGroupModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ProductGroupCacheController()
			: base(RefreshFileType.ProductGroup)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.ProductGroupService.GetAllProductGroup().ToArray();
		}

		/// <summary>
		/// 有効な商品グループ設定取得
		/// </summary>
		/// <returns>有効な商品グループ設定モデル列</returns>
		public ProductGroupModel[] GetApplicableProductGroup()
		{
			// 開始終了時間内、有効フラグがON
			return this.CacheData.Where(productGroup =>
				((productGroup.BeginDate <= DateTime.Now) && ((productGroup.EndDate == null) || (productGroup.EndDate >= DateTime.Now)))
				&& (productGroup.IsValid)).ToArray();
		}

	}
}
