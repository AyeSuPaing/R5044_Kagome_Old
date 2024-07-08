/*
=========================================================================================================
  Module      : Real Shop Area Cache Controller (RealShopAreaCacheController.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.IO;
using System.Linq;
using System.Xml.Linq;
using w2.App.Common.RealShop;
using w2.App.Common.RefreshFileManager;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// Real shop area cache controller
	/// </summary>
	public class RealShopAreaCacheController : DataCacheControllerBase<RealShopArea[]>
	{
		/// <summary>Setting file name</summary>
		private const string PROJECT_SETTING_FILE_NAME = "RealShopArea.xml";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal RealShopAreaCacheController()
			: base(RefreshFileType.RealShopArea)
		{
		}

		/// <summary>
		/// Refresh cache data
		/// </summary>
		internal override void RefreshCacheData()
		{
			if (File.Exists(this.SettingFileFullPath) == false) return;

			this.CacheData = XDocument.Load(this.SettingFileFullPath)
				.Element("RealShopAreaList")
				.Elements("RealShopArea")
				.Select(realShopArea => new RealShopArea
				{
					AreaId = realShopArea.Attribute("areaId").Value,
					AreaName = realShopArea.Attribute("areaName").Value,
				})
				.ToArray();
		}

		/// <summary>
		/// Get real shop area list
		/// </summary>
		/// <returns>Real shop area list</returns>
		public RealShopArea[] GetRealShopAreaList()
		{
			return this.CacheData;
		}

		/// <summary>
		/// Get real shop area name
		/// </summary>
		/// <param name="areaId">Area id</param>
		/// <returns>Real shop area name</returns>
		public string GetRealShopAreaName(string areaId)
		{
			var realShop = this.CacheData.FirstOrDefault(item => (item.AreaId == areaId));
			if (realShop == null) return string.Empty;

			return realShop.AreaName;
		}

		/// <summary>Setting file full path</summary>
		private string SettingFileFullPath
		{
			get { return Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC_XML, PROJECT_SETTING_FILE_NAME); }
		}
	}
}
