/*
=========================================================================================================
  Module      : 特集画像管理サービス (FeatureImageService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.IO;
using w2.Common.Sql;

namespace w2.Domain.FeatureImage
{
	/// <summary>
	/// 特集画像管理サービス
	/// </summary>
	public class FeatureImageService : ServiceBase
	{
		#region +GetAllGroup グループを全て取得
		/// <summary>
		/// グループを全て取得
		/// </summary>
		/// <returns>モデル</returns>
		public FeatureImageGroupModel[] GetAllGroup()
		{
			using (var repository = new FeatureImageRepository())
			{
				var model = repository.GetAllGroup();
				return model;
			}
		}

		#region +GetGroup グループを取得
		/// <summary>
		/// グループを取得
		/// </summary>
		/// <param name="featureImageGroupId">特集画像グループID</param>
		/// <returns>モデル</returns>
		public FeatureImageGroupModel GetGroup(long featureImageGroupId)
		{
			using (var repository = new FeatureImageRepository())
			{
				var model = repository.GetGroup(featureImageGroupId);
				return model;
			}
		}
		#endregion

		/// <summary>
		/// 全ての画像取得
		/// </summary>
		/// <returns>全ての画像</returns>
		public FeatureImageModel[] GetAllImage()
		{
			using (var repository = new FeatureImageRepository())
			{
				var model = repository.GetAllImage();
				return model;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="imageId">画像ID</param>
		/// <returns>モデル</returns>
		public FeatureImageModel Get(long imageId)
		{
			using (var repository = new FeatureImageRepository())
			{
				var model = repository.Get(imageId);
				return model;
			}
		}
		#endregion

		#region +GetImageCountByFileName ファイル名で画像件数取得
		/// <summary>
		/// ファイル名で画像件数取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>件数</returns>
		public int GetImageCountByFileName(string fileName)
		{
			using (var repository = new FeatureImageRepository())
			{
				var count = repository.GetImageCountByFileName(fileName);
				return count;
			}
		}
		#endregion

		#region +InsertGroup グループ登録
		/// <summary>
		/// グループ登録
		/// </summary>
		/// <param name="model">グループ</param>
		public void InsertGroup(FeatureImageGroupModel model)
		{
			using (var repository = new FeatureImageRepository())
			{
				repository.InsertGroup(model);
			}
		}
		#endregion

		#region +Insert
		/// <summary>
		/// 特集画像登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>新規画像ID</returns>
		public int Insert(FeatureImageModel model)
		{
			using (var repository = new FeatureImageRepository())
			{
				var id = repository.Insert(model);
				return id;
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int Update(FeatureImageGroupModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FeatureImageRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +UpdateGroupSort グループ順序更新
		/// <summary>
		/// グループ順序更新
		/// </summary>
		/// <param name="groupIds">グループID</param>
		public void UpdateGroupSort(long[] groupIds)
		{
			using (var repository = new FeatureImageRepository())
			{
				for (var i = 0; i < groupIds.Length; i++)
				{
					var model = repository.GetGroup(groupIds[i]);
					if (model == null) { continue; }

					model.GroupSortNumber = i + 1;
					repository.Update(model);
				}
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int Update(FeatureImageModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FeatureImageRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +InsertUpdateImageSort 画像順序更新
		/// <summary>
		/// 画像順序更新
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="imagePath">画像パス</param>
		public void InsertUpdateImageSort(long groupId, string[] imagePath)
		{
			using (var repository = new FeatureImageRepository())
			{
				for (var i = 0; i < imagePath.Length; i++)
				{
					var fileName = Path.GetFileName(imagePath[i]);
					var dirPath = imagePath[i].Replace(fileName, string.Empty);
					var updateModel = repository.GetImage(dirPath, fileName);
					if (updateModel == null)
					{
						var insertModel = new FeatureImageModel
						{
							FileName = fileName,
							FileDirPath = dirPath,
							GroupId = groupId,
							ImageSortNumber = i + 1
						};
						repository.Insert(insertModel);
					}
					else
					{
						updateModel.GroupId = groupId;
						updateModel.ImageSortNumber = i + 1;
						repository.Update(updateModel);
					}
				}
			}
		}
		#endregion

		#region +UploadFeatureImage グループを指定してアップロード
		/// <summary>
		/// グループを指定してアップロード
		/// </summary>
		/// <param name="featureImageFileName">ファイル名</param>
		/// <param name="fileDirPath">ディレクトリパス</param>
		/// <param name="groupId">グループID</param>
		/// <param name="lastChanged">最終更新者</param>
		public void UploadFeatureImage(string featureImageFileName, string fileDirPath, string groupId, string lastChanged)
		{
			using (var repository = new FeatureImageRepository())
			{
				var images = repository.GetImagesByGroupId(long.Parse(groupId));
				var model = new FeatureImageModel
				{
					FileName = featureImageFileName,
					FileDirPath = fileDirPath,
					GroupId = long.Parse(groupId),
					ImageSortNumber = images.Length + 1,
					LastChanged = lastChanged
				};

				repository.Insert(model);
			}
		}
		#endregion

		#region +Delete グループ削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="featureImageGroupId">特集画像グループID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteGroup(long featureImageGroupId, SqlAccessor accessor = null)
		{
			using (var repository = new FeatureImageRepository(accessor))
			{
				repository.DeleteGroup(featureImageGroupId);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="imageId">画像ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(long imageId, SqlAccessor accessor = null)
		{
			using (var repository = new FeatureImageRepository(accessor))
			{
				repository.Delete(imageId);
			}
		}
		#endregion
	}
}
