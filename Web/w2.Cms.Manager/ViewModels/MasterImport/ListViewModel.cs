/*
=========================================================================================================
  Module      : マスタインポートビューモデル(ListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.WorkerServices;
using SelectListItem = System.Web.Mvc.SelectListItem;

namespace w2.Cms.Manager.ViewModels.MasterImport
{
	/// <summary>
	/// マスタインポートビューモデル
	/// </summary>
	public class ListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ListViewModel()
		{
			var masterList = new MasterImportWorkerService().GetMasterItemFromXml(
				Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.FILE_XML_MASTER_UPLOAD_SETTING));

			MasterKbnInitialization(masterList);
		}

		/// <summary>
		/// マスタ種別のセット
		/// </summary>
		public void MasterKbnInitialization(List<SelectListItem> masterList)
		{
			this.MasterItems = new List<SelectListItem>();
			foreach (var item in masterList)
			{
				// ショートURL
				if ((Constants.SHORTURL_OPTION_ENABLE == false) && (item.Value == Constants.TABLE_SHORTURL)) 
					continue;
				
				this.MasterItems.Add(item);
			}
		}

		/// <summary>マスタ種別</summary>
		public List<SelectListItem> MasterItems { get; set; }
		/// <summary>マスタ種別</summary>
		public string MasterType { get; set; }
		/// <summary>アップロード可能なファイルの最大サイズ（メガバイト）</summary>
		public int MaxRequestLength
		{
			get
			{
				var httpRuntimeSection = (HttpRuntimeSection)ConfigurationManager.GetSection("system.web/httpRuntime");
				var maxRequestLength = httpRuntimeSection.MaxRequestLength;
				return maxRequestLength / 1024;
			}
		}
		/// <summary>アップロード可能か</summary>
		public bool UploadEnabled { get; set; }
		/// <summary>アップロードファイル</summary>
		public string[] UploadFiles { get; set; }
		/// <summary>アップロードファイルの名前</summary>
		public string[] UploadFileNames { get; set; }
	}
}