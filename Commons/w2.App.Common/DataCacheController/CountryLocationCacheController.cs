/*
=========================================================================================================
  Module      : 国ISOコードキャッシュコントローラ(CountryNamesCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.CountryLocation;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 国ISOコードキャッシュコントローラ
	/// </summary>
	public class CountryLocationCacheController : DataCacheControllerBase<CountryLocationCacheController.CountryLocationInfos>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal CountryLocationCacheController() : base(RefreshFileType.CountryLocation)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				var countries = DomainFacade.Instance.CountryLocationService.GetCountryNames();
				var nameIsoCodes = countries.ToDictionary(c => c.CountryName, c => c.CountryIsoCode);
				var isoCodeToNames = countries.ToDictionary(c => c.CountryIsoCode, c => c.CountryName);
				this.CacheData = new CountryLocationInfos(countries, nameIsoCodes, isoCodeToNames);
			}
		}

		/// <summary>
		/// 国ロケーション情報
		/// </summary>
		public class CountryLocationInfos
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="countries">国リスト</param>
			/// <param name="nameIsoCodes">名前→国ISOコードディクショナリ</param>
			/// <param name="isoCodeToNames">国ISOコード→名前ディクショナリ</param>
			public CountryLocationInfos(
				CountryLocationModel[] countries,
				Dictionary<string, string> nameIsoCodes,
				Dictionary<string, string> isoCodeToNames)
			{
				this.Countries = countries;
				this.NameIsoCodes = nameIsoCodes;
				this.IsoCodeToNames = isoCodeToNames;
			}

			/// <summary>国リスト</summary>
			public CountryLocationModel[] Countries { get; set; }
			/// <summary>名前→国ISOコードディクショナリ</summary>
			public Dictionary<string, string> NameIsoCodes { get; set; }
			/// <summary>国ISOコード→名前ディクショナリ</summary>
			public Dictionary<string, string> IsoCodeToNames { get; set; }
		}
	}
}
