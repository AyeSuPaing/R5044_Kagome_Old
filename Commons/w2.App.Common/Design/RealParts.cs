/*
=========================================================================================================
  Module      : 実パーツモデル(RealParts.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;

namespace w2.App.Common.Design
{
	/// <summary>
	/// 実パーツモデル
	/// </summary>
	public class RealParts : RealPage
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RealParts()
		{
			this.PartsTag = new List<PartsTag>();
			this.TemplateTitle = string.Empty;
			this.TemplateFileName = string.Empty;
			this.TemplateFilePath = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="pageTitle">タイトル</param>
		/// <param name="physicalPath">物理パス</param>
		/// <param name="pageDirPath">ページディレクトリパス</param>
		/// <param name="fileName">ファイル名</param>
		/// <param name="pageType">ページタイプ</param>
		/// <param name="templateTitle">テンプレートタイトル</param>
		/// <param name="templateFileName">テンプレートファイル名</param>
		/// <param name="templateFilePath">テンプレートファイルパス</param>
		public RealParts(
			string pageTitle,
			string physicalPath,
			string pageDirPath,
			string fileName,
			string pageType,
			string templateTitle,
			string templateFileName,
			string templateFilePath) : base(pageTitle, physicalPath, pageDirPath, fileName, pageType)
		{
			this.PartsTag = new List<PartsTag>();
			this.TemplateTitle = templateTitle;
			this.TemplateFileName = templateFileName;
			this.TemplateFilePath = templateFilePath;
		}

		/// <summary>テンプレートタイトル</summary>
		public string TemplateTitle { get; private set; }
		/// <summary>テンプレートファイル名</summary>
		public string TemplateFileName { get; private set; }
		/// <summary>テンプレートファイルパス</summary>
		public string TemplateFilePath { get; private set; }
		/// <summary>タグ内容</summary>
		public List<PartsTag> PartsTag { get; set; }
		/// <summary>タグ説明</summary>
		public string Declaration { get; set; }
		/// <summary>標準パーツ: 設定ファイル内容</summary>
		public PartsSetting StandardPartsSetting { get; set; }
		/// <summary>削除許可</summary>
		public override bool PermissionDelete
		{
			get
			{
				return ((this.Existence == ExistStatus.Exist)
					&& (this.PageType == Constants.FLG_PARTSDESIGN_PARTS_TYPE_CUSTOM)
					&& (this.CheckFeatureArea == false));
			}
		}
		/// <summary>コード編集許可</summary>
		public override bool PermissionCodeEdit
		{
			get { return ((this.Existence == ExistStatus.Exist) && (this.CheckFeatureArea == false)); }
		}
		/// <summary>複製許可</summary>
		public override bool PermissionCopy
		{
			get
			{
				return ((this.Existence == ExistStatus.Exist)
					&& (this.PageType == Constants.FLG_PARTSDESIGN_PARTS_TYPE_CUSTOM)
					&& (this.CheckFeatureArea == false));
			}
		}
		/// <summary>特集エリアテンプレートを利用しているか?</summary>
		public bool CheckFeatureArea
		{
			get { return this.TemplateFileName.Contains("900FAT_"); }
		}
		/// <summary>パーツファイル存在するか</summary>
		public bool IsPartFileExists
		{
			get { return (string.IsNullOrEmpty(this.PageDirPath) == false); }
		}
	}

	/// <summary>
	/// タグ内容
	/// </summary>
	public class PartsTag
	{
		/// <summary>タグ内容</summary>
		public string Value { get; set; }
	}
}