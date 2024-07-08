/*
=========================================================================================================
  Module      : 自動バックアップデザインファイルビューモデル(AutoBackedUpDesignFileViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.IO;
using System.Linq;
using w2.App.Common.Design;
using w2.Cms.Manager.Codes.PageDesign;
using w2.Domain.PageDesign;
using w2.Domain.PartsDesign;

namespace w2.Cms.Manager.ViewModels.Shared
{
	/// <summary>自動バックアップデザインファイルビューモデル</summary>
	public class AutoBackedUpDesignFileViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="id">対応したID</param>
		/// <param name="type">復元タイプ</param>
		public AutoBackedUpDesignFileViewModel(long id, RestoreType type)
		{
			string targetPath;
			if (type == RestoreType.Page)
			{
				var page = new PageDesignService().GetPage(id);
				targetPath = Path.Combine(
					DesignCommon.PhysicalDirPathTargetSitePc,
					page.FileDirPath,
					page.FileName);
			}
			else
			{
				var parts = new PartsDesignService().GetParts(id);
				targetPath = Path.Combine(
					DesignCommon.PhysicalDirPathTargetSitePc,
					parts.FileDirPath,
					parts.FileName);
			}

			this.BackUpDirPath = RestoreUtility.GetBuckUpDirectoryPath(
				targetPath,
				DesignCommon.DeviceType.Pc,
				type);

			this.FileDetas = Directory.Exists(this.BackUpDirPath)
				? Directory.GetFiles(this.BackUpDirPath).Select(path => new FileDeta(path)).OrderByDescending(f=>f.FileName).ToArray()
				: new FileDeta[0];
		}

		/// <summary>バックアップするディレクトリパス</summary>
		public string BackUpDirPath { get; set; }
		/// <summary>ファイル名(複数)</summary>
		public FileDeta[] FileDetas { get; set; }

		/// <summary>
		/// ファイルデータクラス
		/// </summary>
		public class FileDeta
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public FileDeta(string filePath)
			{
				this.FileName = Path.GetFileName(filePath);
				this.FilePath = filePath;
			}

			/// <summary>ファイル名</summary>
			public string FileName { get; set; }
			/// <summary>ファイルパス</summary>
			public string FilePath { get; set; }
		}
	}
}