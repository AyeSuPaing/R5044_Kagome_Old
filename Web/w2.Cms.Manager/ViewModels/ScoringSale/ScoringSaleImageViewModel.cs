/*
=========================================================================================================
  Module      : Scoring Sale Image View Model (ScoringSaleImageViewModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.IO;

namespace w2.Cms.Manager.ViewModels.ScoringSale
{
	/// <summary>
	/// Scoring sale image view model
	/// </summary>
	public class ScoringSaleImageViewModel
	{
		/// <summary>Path scoring image</summary>
		private string _pathScoringImage = string.Empty;

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="pathScoringImage">Path scoring image</param>
		public ScoringSaleImageViewModel(string pathScoringImage)
		{
			_pathScoringImage = pathScoringImage;
		}
		#endregion

		#region Property
		/// <summary>画像パス</summary>
		public string ImagePath
		{
			get
			{
				return string.Format("{0}{1}",
					_pathScoringImage,
					this.FileName);
			}
		}
		/// <summary>ファイルサイズ(KB,MB)</summary>
		public string FileSize { get; set; }
		/// <summary>イメージサイズ(縦×横)</summary>
		public string ImageSize { get; set; }
		/// <summary>更新日</summary>
		public string DataChanged { get; set; }
		/// <summary>ファイル名</summary>
		public string FileName
		{
			get
			{
				return Path.GetFileName(this.ImageSrc);
			}
		}
		/// <summary>画像ソース</</summary>
		public string ImageSrc { get; set; }
		#endregion
	}
}