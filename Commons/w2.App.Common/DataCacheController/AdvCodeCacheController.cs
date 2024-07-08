/*
=========================================================================================================
  Module      : 広告コード情報キャッシュコントローラ(AdvCodeCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.AdvCode;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 広告コード情報キャッシュコントローラ
	/// </summary>
	public class AdvCodeCacheController : DataCacheControllerBase<AdvCodeModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AdvCodeCacheController() : base(RefreshFileType.AdvCode)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.AdvCodeService.GetAll();
		}

		/// <summary>
		/// 広告コード 有効なもの全取得
		/// </summary>
		/// <returns>広告コードモデルリスト</returns>
		public AdvCodeModel[] GetAdvCodemodels()
		{
			return this.CacheData.Where(i => i.IsValid).ToArray();
		}

		/// <summary>
		/// 広告コード情報取得
		/// </summary>
		/// <param name="advcode">広告コード</param>
		/// <returns>広告コードモデル</returns>
		public AdvCodeModel GetAdvCodemodel(string advcode)
		{
			return this.CacheData.FirstOrDefault(i => i.IsValid && (i.AdvertisementCode == advcode));
		}
	}
}