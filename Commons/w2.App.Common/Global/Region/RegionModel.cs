/*
=========================================================================================================
  Module      : リージョンモデルクラス(RegionModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using Newtonsoft.Json;
using w2.App.Common.Global.Config;

namespace w2.App.Common.Global.Region
{
	/// <summary>
	/// リージョンモデル
	/// </summary>
	[Serializable]
	public class RegionModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RegionModel(RegionModel model = null)
		{
			this.CountryIsoCode = (model != null) ? model.CountryIsoCode : string.Empty;
			this.LanguageCode = (model != null) ? model.LanguageCode : string.Empty;
			this.LanguageLocaleId = (model != null) ? model.LanguageLocaleId : string.Empty;
			this.CurrencyCode = (model != null) ? model.CurrencyCode : string.Empty;
			this.CurrencyLocaleId = (model != null) ? model.CurrencyLocaleId : string.Empty;
		}

		/// <summary>
		/// 現在設定されているリージョンのチェック
		/// </summary>
		/// <returns>OK:true, NG:false</returns>
		public bool RegionCheck()
		{
			var result = (GlobalConfigUtil.CheckCountryPossible(this.CountryIsoCode)
				&& GlobalConfigUtil.CheckCurrencyPossible(this.CurrencyCode)
				&& GlobalConfigUtil.CheckLanguagePossible(this.LanguageCode)
				&& GlobalConfigUtil.CheckCurrencyLocaleIdPossible(this.CurrencyCode, this.CurrencyLocaleId)
				&& GlobalConfigUtil.CheckLanguageLocaleIdPossible(this.LanguageCode, this.LanguageLocaleId));

			return result;
		}

		/// <summary>
		/// リージョンモデル内のデータの更新
		/// </summary>
		/// <param name="model">更新内容を含むリージョンモデル</param>
		/// <returns>更新成功:true, 更新失敗:false</returns>
		public bool UpdateRegionModel(RegionModel model)
		{
			// 更新前のバックアップモデル
			var backupModel = new RegionModel(this);

			this.CountryIsoCode = (string.IsNullOrEmpty(model.CountryIsoCode)) ? this.CountryIsoCode : model.CountryIsoCode;
			this.LanguageCode = (string.IsNullOrEmpty(model.LanguageCode)) ? this.LanguageCode : model.LanguageCode;
			this.LanguageLocaleId = (string.IsNullOrEmpty(model.LanguageLocaleId)) ? this.LanguageLocaleId : model.LanguageLocaleId;
			this.CurrencyCode = (string.IsNullOrEmpty(model.CurrencyCode)) ? this.CurrencyCode : model.CurrencyCode;
			this.CurrencyLocaleId = (string.IsNullOrEmpty(model.CurrencyLocaleId)) ? this.CurrencyLocaleId : model.CurrencyLocaleId;

			// リージョンの対応チェック
			var result = RegionCheck();

			// NGの場合はもとに戻す
			if (result == false)
			{
				this.CountryIsoCode = backupModel.CountryIsoCode;
				this.LanguageCode = backupModel.LanguageCode;
				this.LanguageLocaleId = backupModel.LanguageLocaleId;
				this.CurrencyCode = backupModel.CurrencyCode;
				this.CurrencyLocaleId = backupModel.CurrencyLocaleId;
			}

			return result;
		}

		/// <summary>国ISOコード</summary>
		[JsonProperty("cic")]
		public string CountryIsoCode { get; set; }
		/// <summary>言語コード</summary>
		[JsonProperty("lc")]
		public string LanguageCode { get; set; }
		/// <summary>言語ロケールID</summary>
		[JsonProperty("lli")]
		public string LanguageLocaleId { get; set; }
		/// <summary>通貨コード</summary>
		[JsonProperty("cc")]
		public string CurrencyCode { get; set; }
		/// <summary>通貨ロケールID</summary>
		[JsonProperty("cli")]
		public string CurrencyLocaleId { get; set; }
	}
}
