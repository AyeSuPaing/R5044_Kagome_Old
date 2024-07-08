/*
=========================================================================================================
  Module      : 特集画像管理リポジトリ (FeatureImageRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.FeatureImage
{
	/// <summary>
	/// 特集画像管理リポジトリ
	/// </summary>
	internal class FeatureImageRepository : RepositoryBase
	{
		/// <returns>キー</returns>
		private const string XML_KEY_NAME = "FeatureImage";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal FeatureImageRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal FeatureImageRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetAllGroup 全てグループ取得
		/// <summary>
		/// 全てグループ取得
		/// </summary>
		/// <returns>モデル</returns>
		internal FeatureImageGroupModel[] GetAllGroup()
		{
			var dv = Get(XML_KEY_NAME, "GetAllGroup", new Hashtable());
			return dv.Cast<DataRowView>().Select(item => new FeatureImageGroupModel(item)).ToArray();
		}
		#endregion

		#region +GetGroup グループ取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="featureImageGroupId">特集画像グループID</param>
		/// <returns>モデル</returns>
		internal FeatureImageGroupModel GetGroup(long featureImageGroupId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FEATUREIMAGEGROUP_FEATURE_IMAGE_GROUP_ID, featureImageGroupId},
			};
			var dv = Get(XML_KEY_NAME, "GetGroup", ht);
			if (dv.Count == 0) return null;
			return new FeatureImageGroupModel(dv[0]);
		}
		#endregion

		#region +GetAllImage 全て画像取得
		/// <summary>
		/// 全て画像取得
		/// </summary>
		/// <returns>モデル</returns>
		internal FeatureImageModel[] GetAllImage()
		{
			var dv = Get(XML_KEY_NAME, "GetAllImage");
			return dv.Cast<DataRowView>().Select(item => new FeatureImageModel(item)).ToArray();
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="imageId">画像ID</param>
		/// <returns>モデル</returns>
		internal FeatureImageModel Get(long imageId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FEATUREIMAGE_IMAGE_ID, imageId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new FeatureImageModel(dv[0]);
		}
		#endregion

		#region +GetImage パスで取得
		/// <summary>
		/// パスで取得
		/// </summary>
		/// <param name="dirPath">パス</param>
		/// <param name="fileName">ファイル名</param>
		/// <returns>モデル</returns>
		internal FeatureImageModel GetImage(string dirPath, string fileName)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FEATUREIMAGE_FILE_DIR_PATH, dirPath},
				{Constants.FIELD_FEATUREIMAGE_FILE_NAME, fileName},
			};
			var dv = Get(XML_KEY_NAME, "GetImageByPath", ht);
			if (dv.Count == 0) return null;
			return new FeatureImageModel(dv[0]);
		}
		#endregion

		#region +GetImagesByGroupId グループIDで画像取得
		/// <summary>
		/// グループIDで画像取得
		/// </summary>
		/// <returns>モデル</returns>
		internal FeatureImageModel[] GetImagesByGroupId(long groupId)
		{
			var input = new Hashtable()
			{
				{Constants.FIELD_FEATUREIMAGE_GROUP_ID, groupId}
			};
			var dv = Get(XML_KEY_NAME, "GetImagesByGroupId", input);
			return dv.Cast<DataRowView>().Select(item => new FeatureImageModel(item)).ToArray();
		}
		#endregion

		#region +GetImageCountByFileName ファイル名で画像件数取得
		/// <summary>
		/// ファイル名で画像件数取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>件数</returns>
		internal int GetImageCountByFileName(string fileName)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_FEATUREIMAGE_FILE_NAME, fileName}
			};
			var dv = Get(XML_KEY_NAME, "GetImageCountByFileName", input);
			var result = (int)dv[0][0];
			return result;
		}
		#endregion

		#region +InsertGroup グループ登録
		/// <summary>
		/// グループ登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertGroup(FeatureImageGroupModel model)
		{
			Exec(XML_KEY_NAME, "InsertGroup", model.DataSource);
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>特集画像ID</returns>
		internal int Insert(FeatureImageModel model)
		{
			var result = Get(XML_KEY_NAME, "Insert", model.DataSource);
			return int.Parse(result[0][0].ToString());
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(FeatureImageGroupModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateGroup", model.DataSource);
			return result;
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(FeatureImageModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +DeleteGroup グループ削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="featureImageGroupId">特集画像グループID</param>
		internal int DeleteGroup(long featureImageGroupId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FEATUREIMAGEGROUP_FEATURE_IMAGE_GROUP_ID, featureImageGroupId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteGroup", ht);
			return result;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="imageId">画像ID</param>
		internal int Delete(long imageId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FEATUREIMAGE_IMAGE_ID, imageId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion
	}
}
