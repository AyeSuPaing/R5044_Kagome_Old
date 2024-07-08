/*
=========================================================================================================
  Module      : 実ページモデル(RealPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;

namespace w2.App.Common.Design
{
	/// <summary>
	/// 実ページモデル
	/// </summary>
	public class RealPage
	{
		/// <summary>ファイルの存在状態</summary>
		public enum ExistStatus
		{
			/// <summary>存在</summary>
			Exist,
			/// <summary>非存在</summary>
			NotExist
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RealPage()
		{
			this.PageTitle = string.Empty;
			this.PhysicalPath = string.Empty;
			this.PageDirPath = string.Empty;
			this.FileName = string.Empty;
			this.PageType = string.Empty;
			this.PhysicalFullPath = string.Empty;
			this.Existence = ExistStatus.NotExist;
			this.CreateDate = new DateTime();
			this.UpdateDate = new DateTime();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="pageTitle">タイトル</param>
		/// <param name="physicalPath">物理パス</param>
		/// <param name="pageDirPath">ページディレクトリパス</param>
		/// <param name="fileName">ファイル名</param>
		/// <param name="pageType">ページタイプ</param>
		public RealPage(string pageTitle, string physicalPath, string pageDirPath, string fileName, string pageType)
		{
			this.PageTitle = pageTitle;
			this.PhysicalPath = physicalPath;
			this.PageDirPath = pageDirPath;
			this.FileName = fileName;
			this.PageType = pageType;
			this.PhysicalFullPath = Path.Combine(this.PhysicalPath, this.PageDirPath, this.FileName);
			this.Existence = File.Exists(this.PhysicalFullPath) ? ExistStatus.Exist : ExistStatus.NotExist;
			this.CreateDate = (this.Existence == ExistStatus.Exist)
				? File.GetCreationTime(this.PhysicalFullPath)
				: new DateTime();
			this.UpdateDate = (this.Existence == ExistStatus.Exist)
				? File.GetLastWriteTime(this.PhysicalFullPath)
				: new DateTime();
		}

		/// <summary>タイトル</summary>
		public string PageTitle { get; set; }
		/// <summary>物理パス</summary>
		public string PhysicalPath { get; private set; }
		/// <summary>物理フルパス</summary>
		public string PhysicalFullPath { get; private set; }
		/// <summary>ページディレクトリパス</summary>
		public string PageDirPath { get; private set; }
		/// <summary>ファイルパス</summary>
		public string FileName { get; private set; }
		/// <summary>ページタイプ</summary>
		public string PageType { get; private set; }
		/// <summary>標準ページ: 設定ファイル内容</summary>
		public PageSetting StandardPageSetting { get; set; }
		/// <summary>ファイル有無</summary>
		public ExistStatus Existence { get; set; }
		/// <summary>作成日付</summary>
		public DateTime CreateDate { get; private set; }
		/// <summary>更新日付</summary>
		public DateTime UpdateDate { get; private set; }
		/// <summary>最終更新者</summary>
		public string LastChange { get; set; }
		/// <summary>削除許可</summary>
		public virtual bool PermissionDelete
		{
			get
			{
				return ((this.Existence == ExistStatus.Exist)
					&& (this.PageType == Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM));
			}
		}
		/// <summary>コード編集許可</summary>
		public virtual bool PermissionCodeEdit
		{
			get
			{
				return ((this.Existence == ExistStatus.Exist)
					&& (this.PageType == Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM));
			}
		}
		/// <summary>複製許可</summary>
		public virtual bool PermissionCopy
		{
			get { return (this.Existence == ExistStatus.Exist); }
		}
	}
}