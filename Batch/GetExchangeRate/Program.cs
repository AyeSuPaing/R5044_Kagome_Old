/*
=========================================================================================================
  Module      : 為替レート取得バッチ(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Global.Region.Currency;
using w2.Commerce.GetExchangeRate.WebApi;
using w2.Commerce.GetExchangeRate.WebApi.CurrencyConverterWebApi;
using w2.Common.Extensions;
using w2.Common.Logger;
using w2.Domain.ExchangeRate;
using w2.Commerce.GetExchangeRate.WebApi.CurrencyLayer;
using w2.Commerce.GetExchangeRate.WebApi.ExchangeRateApi;

namespace w2.Commerce.GetExchangeRate
{
	/// <summary>
	/// 為替レート取得バッチ
	/// </summary>
	internal class Program
	{
		#region メイン処理
		/// <summary>
		/// メイン処理
		/// </summary>
		/// <param name="args">引数</param>
		private static void Main(string[] args)
		{
			try
			{
				var program = new Program();

				// バッチ起動をイベントログ出力
				AppLogger.WriteInfo("起動");

				program.Exec();

				// バッチ終了をイベントログ出力
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				// エラーイベントログ出力
				AppLogger.WriteError(ex);
			}
		}
		#endregion メイン処理

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Program()
		{
			Initialize();
		}
		#endregion

		#region 初期化
		/// <summary>
		/// 初期化処理
		/// </summary>
		private void Initialize()
		{
			try
			{
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				var appSetting = new ConfigurationSetting(
					Properties.Settings.Default.ConfigFileDirPath,
					ConfigurationSetting.ReadKbn.C000_AppCommon,
					ConfigurationSetting.ReadKbn.C100_BatchCommon,
					ConfigurationSetting.ReadKbn.C200_GetExchangeRate);

				// 為替レート取得バッチの設定読み込み
				Constants.EXCHANGERATE_KBN = (Constants.ExchangeRateKbn?)appSetting.GetAppSetting("GetExchangeRate_Kbn", typeof(Constants.ExchangeRateKbn));
				Constants.EXCHANGERATE_BASEURL = appSetting.GetAppStringSetting("ExchangeRate_BaseUrl");
				Constants.EXCHANGERATE_ACCESSKEY = appSetting.GetAppStringSetting("ExchangeRate_AccessKey");
				Constants.EXCHANGERATE_SRCCURRENCYCODES = appSetting.GetAppStringSetting("ExchangeRate_SrcCurrencyCodes").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				Constants.EXCHANGERATE_DSTCURRENCYCODES = appSetting.GetAppStringSetting("ExchangeRate_DstCurrencyCodes").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				Constants.EXCHANGERATE_SAVEDIRECTORYPATH = appSetting.GetAppStringSetting("ExchangeRate_SaveDirectoryPath");
				Constants.EXCHANGERATE_SAVEFILENAME = appSetting.GetAppStringSetting("ExchangeRate_SaveFileName");
				Constants.EXCHANGERATE_SAVEFILENAME_DATEFORMAT = appSetting.GetAppStringSetting("ExchangeRate_SaveFileName_DateFormat");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}
		#endregion

		#region 実行
		/// <summary>
		/// 実行
		/// </summary>
		private void Exec()
		{
			// 為替レート取得
			var rates = GetExchangeRates();
			// ファイル保存
			WriteExchangeRatesFile(rates);
			// DB更新
			UpdateExchangeRates(rates);
		}
		#endregion

		#region 為替レート取得
		/// <summary>
		/// 為替レート取得
		/// </summary>
		/// <returns>為替レート</returns>
		private IEnumerable<GetExchangeRateResposeModel> GetExchangeRates()
		{
			switch (Constants.EXCHANGERATE_KBN)
			{
				case Constants.ExchangeRateKbn.CurrencyLayer:
					this.CurrencyLayerApi = new CurrencyLayerWebApi(Constants.EXCHANGERATE_BASEURL, Constants.EXCHANGERATE_ACCESSKEY);
					this.ExchangeRates = this.CurrencyLayerApi.GetExchangeRates(Constants.EXCHANGERATE_SRCCURRENCYCODES, Constants.EXCHANGERATE_DSTCURRENCYCODES);
					break;
				case Constants.ExchangeRateKbn.CurrencyConverter:
					this.CurrencyConverterApi = new CurrencyConverterWebApi(Constants.EXCHANGERATE_BASEURL, Constants.EXCHANGERATE_ACCESSKEY);
					this.ExchangeRates = this.CurrencyConverterApi.GetExchangeRates(Constants.EXCHANGERATE_SRCCURRENCYCODES, Constants.EXCHANGERATE_DSTCURRENCYCODES);
					break;
				case Constants.ExchangeRateKbn.ExchangeRate:
					this.ExchangeRateApi = new ExchangeRateWebApi(Constants.EXCHANGERATE_BASEURL, Constants.EXCHANGERATE_ACCESSKEY);
					this.ExchangeRates = this.ExchangeRateApi.GetExchangeRates(Constants.EXCHANGERATE_SRCCURRENCYCODES, Constants.EXCHANGERATE_DSTCURRENCYCODES);
					break;
			}
			
			if(this.ExchangeRates == null) throw new ApplicationException("為替レートAPI区分の設定が正しくありません");

			var failureExchangeRates = this.ExchangeRates.Where(rate => (rate.Success == false)).ToArray();
			if (failureExchangeRates.Any())
			{
				var responseJson = "";
				switch (Constants.EXCHANGERATE_KBN)
				{
					case Constants.ExchangeRateKbn.CurrencyLayer:
						responseJson = this.CurrencyLayerApi.ConvertExchangeRatesToJson(failureExchangeRates);
						break;
					case Constants.ExchangeRateKbn.CurrencyConverter:
						responseJson = this.CurrencyConverterApi.ConvertExchangeRatesToJson(failureExchangeRates);
						break;
					case Constants.ExchangeRateKbn.ExchangeRate:
						responseJson = this.ExchangeRateApi.ConvertExchangeRatesToJson(failureExchangeRates);
						break;
				}
				var errorMessage = string.Format("為替レートの取得に失敗しました。\r\n{0}", responseJson);
				throw new ApplicationException(errorMessage);
			}
			return this.ExchangeRates;
		}

		#endregion

		#region 為替レートファイル保存
		/// <summary>
		/// 為替レートファイル保存
		/// </summary>
		/// <param name="exchangeRates">為替レート</param>
		private void WriteExchangeRatesFile(IEnumerable<GetExchangeRateResposeModel> exchangeRates)
		{
			var saveDirectoryPath = Constants.EXCHANGERATE_SAVEDIRECTORYPATH;
			if (Directory.Exists(saveDirectoryPath) == false) Directory.CreateDirectory(saveDirectoryPath);

			var dateFormat = DateTime.Now.ToString(Constants.EXCHANGERATE_SAVEFILENAME_DATEFORMAT);
			var saveFileName = Constants.EXCHANGERATE_SAVEFILENAME.Replace("@@DATE_FORMAT@@", dateFormat);
			var outputFilePath = Path.Combine(saveDirectoryPath, saveFileName);

			var responseJson = "";
			switch (Constants.EXCHANGERATE_KBN)
			{
				case Constants.ExchangeRateKbn.CurrencyLayer:
					responseJson = this.CurrencyLayerApi.ConvertExchangeRatesToJson(exchangeRates);
					break;
				case Constants.ExchangeRateKbn.CurrencyConverter:
					responseJson = this.CurrencyConverterApi.ConvertExchangeRatesToJson(exchangeRates);
					break;
				case Constants.ExchangeRateKbn.ExchangeRate:
					responseJson = this.ExchangeRateApi.ConvertExchangeRatesToJson(exchangeRates);
					break;
			}
			File.WriteAllText(outputFilePath, responseJson);
		}
		#endregion

		#region 為替レートマスタ更新
		/// <summary>
		/// 為替レートマスタ更新（Truncate → BulkInsert）
		/// </summary>
		/// <param name="exchangeRates">為替レート</param>
		private void UpdateExchangeRates(IEnumerable<GetExchangeRateResposeModel> exchangeRates)
		{
			var allExchangeRates = exchangeRates
				.SelectMany(rate => rate.Quotes)
				.Select(dict => new
				{
					SrcCurrencyCode = dict.Key.Substring(0, 3), // 1～3文字は通貨コード（元）
					DstCurrencyCode = dict.Key.Substring(3, 3), // 3～6文字は通貨コード（先）
					ExchangeRate = decimal.Parse(dict.Value, NumberStyles.Any)
				}
				);

			var service = new ExchangeRateService();
			service.Truncate();
			service.BulkInsert(allExchangeRates.AsDataReader());

			CurrencyManager.UpdateExchangeRateCache();
		}
		#endregion

		/// <summary>為替レートレスポンス</summary>
		private IEnumerable<GetExchangeRateResposeModel> ExchangeRates { get; set; }
		/// <summary>CurrencyLayerWebApi</summary>
		private CurrencyLayerWebApi CurrencyLayerApi { get; set; }
		/// <summary>CurrencyConverterWebApi</summary>
		private CurrencyConverterWebApi CurrencyConverterApi { get; set; }
		/// <summary>ExchangeRateWebApi</summary>
		private ExchangeRateWebApi ExchangeRateApi { get; set; }
	}
}