/*
=========================================================================================================
  Module      : ページ管理 共通処理 (PageDesignCommon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.IO;

namespace w2.App.Common.Design
{
	/// <summary>
	/// ページ管理 共通処理
	/// </summary>
	public class PageDesignCommon
	{
		/// <summary>
		/// カスタムページリスト取得
		/// </summary>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <param name="targetPage">ターゲットファイル条件</param>
		/// <returns>カスタムページリスト</returns>
		public static List<RealPage> GetCustomPageList(
			DesignCommon.DeviceType deviceType,
			string targetPage = "*" + DesignCommon.PAGE_FILE_EXTENSION)
		{
			var physicalDirPathTargetSite = DesignCommon.GetPhysicalDirPathTargetSite(deviceType);

			// カスタムページ読み取り
			var list = new List<RealPage>();
			if (string.IsNullOrEmpty(targetPage)) return list;

			foreach (var strFilePath in Directory.GetFiles(
				physicalDirPathTargetSite + DesignCommon.CUSTOM_PAGE_DIR_PATH,
				targetPage))
			{
				var fileName = Path.GetFileName(strFilePath);
				var filePath = strFilePath.Replace(physicalDirPathTargetSite, "").Replace(fileName, "");
				var realPage = new RealPage(
					string.Empty,
					physicalDirPathTargetSite,
					filePath,
					fileName,
					Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM);

				list.Add(realPage);
			}
			return list;
		}

		/// <summary>
		/// 標準ページリスト取得
		/// </summary>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>標準ページリスト</returns>
		public static List<RealPage> GetStandardPageSetting(DesignCommon.DeviceType deviceType)
		{
			var resultList = new List<RealPage>();
			foreach (var setting in PageDesignSetting.GetInstance().PageSettingList)
			{
				var fileName = Path.GetFileName(setting.Path);

				if (string.IsNullOrEmpty(fileName)) continue;

				var filePath = setting.Path.Replace(fileName, "");
				var realPage = new RealPage(
					setting.Name,
					DesignCommon.GetPhysicalDirPathTargetSite(deviceType),
					filePath,
					fileName,
					Constants.FLG_PAGEDESIGN_PAGE_TYPE_NORMAL)
				{
					StandardPageSetting = setting
				};

				if ((Constants.FEATUREPAGESETTING_OPTION_ENABLED == false)
					&& (realPage.StandardPageSetting.Option == PageDesignSetting.Option.FeaturePage.ToString())) continue;

				if ((Constants.AMAZON_PAYMENT_OPTION_ENABLED == false)
					&& (realPage.StandardPageSetting.Option == PageDesignSetting.Option.Amazon.ToString())) continue;

				if ((Constants.ORDER_COMBINE_OPTION_ENABLED == false)
					&& (realPage.StandardPageSetting.Option == PageDesignSetting.Option.Combine.ToString())) continue;

				if ((Constants.GIFTORDER_OPTION_ENABLED == false)
					&& (realPage.StandardPageSetting.Option == PageDesignSetting.Option.Gift.ToString())) continue;

				if ((Constants.AWOO_OPTION_ENABLED == false)
					&& (realPage.StandardPageSetting.Option == PageDesignSetting.Option.Awoo.ToString())) continue;

				if (File.Exists(realPage.PhysicalFullPath))
				{
					resultList.Add(realPage);
				}
			}
			return resultList;
		}

		/// <summary>
		/// HTMLページリスト取得
		/// </summary>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <param name="targetPage">ターゲットページ</param>
		/// <returns>標準ページリスト</returns>
		public static List<RealPage> GetHtmlPageList(
			DesignCommon.DeviceType deviceType,
			string targetPage = "*" + DesignCommon.HTML_FILE_EXTENSION)
		{
			var physicalDirPathTargetSite = DesignCommon.GetPhysicalDirPathTargetSite(deviceType);

			// HTMLページ読み取り
			var list = new List<RealPage>();
			if (string.IsNullOrEmpty(physicalDirPathTargetSite)) return list;

			foreach (var strFilePath in Directory.GetFiles(physicalDirPathTargetSite, targetPage))
			{
				var fileName = Path.GetFileName(strFilePath);
				var filePath = strFilePath.Replace(physicalDirPathTargetSite, "").Replace(fileName, "");
				var realPage = new RealPage(
					fileName,
					physicalDirPathTargetSite,
					filePath,
					fileName,
					Constants.FLG_PAGEDESIGN_PAGE_TYPE_HTML);
				list.Add(realPage);
			}
			return list;
		}
	}
}
