/*
=========================================================================================================
 Module      : サイトマップ用ページモデル(SitemapPageModel.cs)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.PageDesign.Helper
{
	/// <summary>
	/// サイトマップ用ページモデル
	/// (複数テーブルから情報取得し一元管理するためのヘルパ、DBには存在しない）
	/// </summary>
	public class SitemapPageModel : ModelBase<SitemapPageModel>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SitemapPageModel()
		{
			this.ManagementTitle = string.Empty;
			this.FileName = string.Empty;
			this.FileDirPath = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SitemapPageModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public SitemapPageModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>管理用タイトル</summary>
		public string ManagementTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_PAGEDESIGN_MANAGEMENT_TITLE]; }
			set { this.DataSource[Constants.FIELD_PAGEDESIGN_MANAGEMENT_TITLE] = value; }
		}
		/// <summary>ファイル名(拡張子含む)</summary>
		public string FileName
		{
			get { return (string)this.DataSource[Constants.FIELD_PAGEDESIGN_FILE_NAME]; }
			set { this.DataSource[Constants.FIELD_PAGEDESIGN_FILE_NAME] = value; }
		}
		/// <summary>ディレクトリパス</summary>
		public string FileDirPath
		{
			get { return (string)this.DataSource[Constants.FIELD_PAGEDESIGN_PC_FILE_DIR_PATH]; }
			set { this.DataSource[Constants.FIELD_PAGEDESIGN_PC_FILE_DIR_PATH] = value; }
		}
		#endregion
	}
}