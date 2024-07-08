/*
=========================================================================================================
  Module      : Criteoモデルクリエーター(CriteoModelCreator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.Batch.ExportCriteoDatas.Proc
{
	/// <summary>
	/// Criteoモデルのクリエーター
	/// </summary>
	public class CriteoModelCreator
	{
		/// <summary>
		/// Criteo連携用モデル生成
		/// </summary>
		/// <param name="product">商品モデル</param>
		/// <param name="setting">Criteo連携用サイト設定</param>
		/// <returns>Criteo連携用モデル</returns>
		public CriteoModel CreateCriteoModel(ProductModel product, CriteoSiteSetting setting)
		{
			CriteoModel model = new CriteoModel(product, setting);
			return model;
		}
	}
}
