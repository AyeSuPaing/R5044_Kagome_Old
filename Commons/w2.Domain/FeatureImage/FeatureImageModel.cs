/*
=========================================================================================================
  Module      : 特集画像管理モデル (FeatureImageModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FeatureImage
{
	/// <summary>
	/// 特集画像管理モデル
	/// </summary>
	[Serializable]
	public partial class FeatureImageModel : ModelBase<FeatureImageModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FeatureImageModel()
		{
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeatureImageModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeatureImageModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>画像ID</summary>
		public long ImageId
		{
			get { return (long)this.DataSource[Constants.FIELD_FEATUREIMAGE_IMAGE_ID]; }
		}
		/// <summary>ファイル名(拡張子含む)</summary>
		public string FileName
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREIMAGE_FILE_NAME]; }
			set { this.DataSource[Constants.FIELD_FEATUREIMAGE_FILE_NAME] = value; }
		}
		/// <summary>ディレクトリパス</summary>
		public string FileDirPath
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREIMAGE_FILE_DIR_PATH]; }
			set { this.DataSource[Constants.FIELD_FEATUREIMAGE_FILE_DIR_PATH] = value; }
		}
		/// <summary>グループ識別ID</summary>
		public long GroupId
		{
			get { return (long)this.DataSource[Constants.FIELD_FEATUREIMAGE_GROUP_ID]; }
			set { this.DataSource[Constants.FIELD_FEATUREIMAGE_GROUP_ID] = value; }
		}
		/// <summary>グループ内画像順序</summary>
		public int ImageSortNumber
		{
			get { return (int)this.DataSource[Constants.FIELD_FEATUREIMAGE_IMAGE_SORT_NUMBER]; }
			set { this.DataSource[Constants.FIELD_FEATUREIMAGE_IMAGE_SORT_NUMBER] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FEATUREIMAGE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FEATUREIMAGE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FEATUREIMAGE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FEATUREIMAGE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREIMAGE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FEATUREIMAGE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
