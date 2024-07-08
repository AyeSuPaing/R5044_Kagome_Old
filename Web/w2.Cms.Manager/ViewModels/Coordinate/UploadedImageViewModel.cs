/*
=========================================================================================================
  Module      : アップロード済み画像モデル(UploadedImageViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.IO;
using System.Linq;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.ViewModels.Coordinate
{
	/// <summary>
	/// アップロード済み画像モデル
	/// </summary>
	public class UploadedImageViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <param name="operatorId">オペレータID</param>
		public UploadedImageViewModel(string coordinateId, string operatorId)
		{
			var targetPath = Path.Combine(
				Constants.PHYSICALDIRPATH_FRONT_PC,
				Constants.PATH_TEMP,
				operatorId,
				Constants.PATH_TEMP_COORDINATE,
				coordinateId);
			this.CoordinateId = coordinateId;
			if (Directory.Exists(targetPath))
			{
				var target = Directory.EnumerateFileSystemEntries(targetPath);
				var enumerable = target as string[] ?? target.ToArray();
				this.HasUploadedImage = enumerable.Any();
				this.UploadedImagePathList = new List<string>();
				for (var i = 1; i <= enumerable.Length; i++)
				{
					this.UploadedImagePathList.Add(
						CoordinateWorkerService.GetImage(Constants.COORDINATEIMAGE_PREFIX + coordinateId + i + ".jpg", Path.Combine(Constants.PATH_TEMP, operatorId, "Coordinate", coordinateId)));
				}
			}
		}

		/// <summary>画像がアップロードされているか</summary>
		public bool HasUploadedImage { get; set; }
		/// <summary>メイン画像パスリスト</summary>
		public List<string> UploadedImagePathList { get; set; }
		/// <summary>コーディネートID</summary>
		public string CoordinateId { get; set; }
	}
}