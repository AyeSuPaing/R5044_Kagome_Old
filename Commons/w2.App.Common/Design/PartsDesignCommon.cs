/*
=========================================================================================================
  Module      : ページ管理 共通処理 (PartsDesignCommon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using w2.Common.Util;
using w2.Common.Web;

namespace w2.App.Common.Design
{
	/// <summary>
	/// ページ管理 共通処理
	/// </summary>
	public class PartsDesignCommon
	{
		/// <summary>デバイスタイプ</summary>
		public enum DeviceType
		{
			/// <summary>PCデバイス</summary>
			Pc,
			/// <summary>SPデバイス</summary>
			Sp,
		}

		/// <summary>パーツファイル接頭文字</summary>
		public const string PARTS_PREFIX_NAME = "Parts";

		/// <summary>
		/// カスタムパーツディレクトリの作成
		/// </summary>
		public static void CreatePartsDirectory()
		{
			var pcPartsDirPath = Path.Combine(
				DesignCommon.PhysicalDirPathTargetSitePc,
				DesignCommon.CUSTOM_PARTS_DIR_PATH);
			if (Directory.Exists(pcPartsDirPath) == false) Directory.CreateDirectory(pcPartsDirPath);

			var spPartsDirPath = Path.Combine(
				DesignCommon.PhysicalDirPathTargetSiteSp,
				DesignCommon.CUSTOM_PARTS_DIR_PATH);
			if (Directory.Exists(spPartsDirPath) == false) Directory.CreateDirectory(spPartsDirPath);
		}

		/// <summary>
		/// カスタムパーツリスト取得
		/// </summary>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <param name="targetParts">対象パーツ</param>
		/// <returns>カスタムパーツリスト取得</returns>
		public static List<RealParts> GetCustomPartsList(
			DesignCommon.DeviceType deviceType,
			string targetParts = "*" + DesignCommon.PARTS_FILE_EXTENSION)
		{
			var physicalDirPathTargetSite = DesignCommon.GetPhysicalDirPathTargetSite(deviceType);

			var list = new List<RealParts>();
			foreach (var strFilePath in Directory.GetFiles(
				physicalDirPathTargetSite + DesignCommon.CUSTOM_PARTS_DIR_PATH,
				targetParts))
			{
				var partsName = "";
				var partsValue = new string[] { };
				var partsInitial = GetPartsPrefixStandardPartsName(strFilePath);

				foreach (var li in PartsDesignSetting.GetInstance().DesignParts.UseTemplateStandardParts)
				{
					if (li.Value.ToLower().Contains(partsInitial.ToLower()) == false) continue;

					partsName = li.Text;
					partsValue = li.Value.Split(',');
					break;
				}

				if (string.IsNullOrEmpty(partsName) == false)
				{
					var fileName = Path.GetFileName(strFilePath);
					var filePath = strFilePath.Replace(physicalDirPathTargetSite, "").Replace(fileName, "");
					var realParts = new RealParts(
						string.Empty,
						physicalDirPathTargetSite,
						filePath,
						fileName,
						Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM,
						partsName,
						partsValue[0],
						partsValue[1]);
					var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
					realParts.Declaration = "<%@ Register TagPrefix=\"uc\" TagName=\""
						+ HtmlSanitizer.HtmlEncode(fileNameWithoutExtension) + "\" Src=\"~/"
						+ DesignCommon.CUSTOM_PARTS_DIR_PATH.Replace("\\", "/") + "/"
						+ HtmlSanitizer.HtmlEncode(fileNameWithoutExtension) + DesignCommon.PARTS_FILE_EXTENSION
						+ "\" %>";

					realParts.PartsTag.Add(
						new PartsTag
						{
							Value = "<uc:" + fileNameWithoutExtension + " runat=\"server\" />"
						});
					list.Add(realParts);
				}
			}
			return list;
		}

		/// <summary>
		/// 標準パーツリスト取得
		/// </summary>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>標準パーツ</returns>
		public static List<RealParts> GetStandardPartsList(DesignCommon.DeviceType deviceType)
		{
			var resultList = new List<RealParts>();
			foreach (var setting in PartsDesignSetting.GetInstance().DesignParts.PartsSetting)
			{
				if (ValidParts(setting.Path) == false) continue;

				var fileName = Path.GetFileName(setting.Path);
				if (fileName == null) continue;

				var filePath = setting.Path.Replace(fileName, "");
				var realParts = new RealParts(
					setting.Name,
					DesignCommon.GetPhysicalDirPathTargetSite(deviceType),
					filePath,
					fileName,
					Constants.FLG_PARTSDESIGN_PARTS_TYPE_NORMAL,
					"",
					"",
					"")
				{
					Declaration = StringUtility.ToEmpty(setting.Declaration),
					StandardPartsSetting = setting
				};

				realParts.PartsTag.AddRange(
					setting.LayoutParts.Select(
						lp => new PartsTag
						{
							Value = lp.Value
						}).ToList());

				if (File.Exists(realParts.PhysicalFullPath))
				{
					resultList.Add(realParts);
				}
			}
			return resultList;
		}

		/// <summary>
		/// 有効なパーツかどうか？
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <returns>判定結果</returns>
		public static bool ValidParts(string filePath)
		{
			// リアル店舗商品在庫一覧パーツ AND リアル店舗OPがOFFの場合
			if ((filePath.ToLower().Contains("BodyRealShopProductStockList.ascx".ToLower())
					|| filePath.ToLower().Contains("100RSPSL_".ToLower()))
					&& (Constants.REALSHOP_OPTION_ENABLED == false))
			{
				return false;
			}

			// レコメンド表示パーツ AND レコメンド設定OPがOFFの場合
			if ((filePath.ToLower().Contains("RecommendEngine.ascx".ToLower())
					|| filePath.ToLower().Contains("080PRRE_".ToLower())
					|| filePath.ToLower().Contains("090CRRE_".ToLower()))
					&& (Constants.RECOMMEND_OPTION_ENABLED == false))
			{
				return false;
			}

			// 特集ページパーツ AND 特集ページOPがOFFの場合
			if ((filePath.ToLower().Contains("PartsBannerTemplate.ascx".ToLower())
					|| filePath.ToLower().Contains("900FAT_".ToLower()))
					&& (Constants.FEATUREAREASETTING_OPTION_ENABLED == false))
			{
				return false;
			}

			// 頒布会コース一覧パーツ AND 頒布会OPがOFFの場合
			if ((filePath.ToLower().Contains("BodySubscriptionBoxList.ascx".ToLower())
					|| filePath.ToLower().Contains("000SSBL_".ToLower()))
					&& (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED == false))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// ファイル名よりパーツ種別文字 000TMPL_ など(ValueText管理) を取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>パーツ設定文字</returns>
		public static string GetPartsPrefixStandardPartsName(string fileName)
		{
			var partsInitial = StringUtility.ToEmpty(Path.GetFileNameWithoutExtension(fileName))
				.Replace(PARTS_PREFIX_NAME, "");
			partsInitial = (partsInitial.Length >= 3) ? partsInitial.Substring(0, partsInitial.Length - 3) : "";
			return partsInitial;
		}
	}
}