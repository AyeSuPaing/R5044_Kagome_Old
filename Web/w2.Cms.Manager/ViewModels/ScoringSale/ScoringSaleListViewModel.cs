/*
=========================================================================================================
  Module      : Scoring Sale List View Model(ScoringSaleListViewModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.ParamModels.ScoringSale;

namespace w2.Cms.Manager.ViewModels.ScoringSale
{
	/// <summary>
	/// Scoring sale list view model
	/// </summary>
	public class ScoringSaleListViewModel : ViewModelBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ScoringSaleListViewModel()
		{
			this.ParamModel = new ScoringSaleListParamModel();
			this.Items = new ScoringSaleListItemDetailViewModel[0];
			this.OpenDetailScoringSaleId = string.Empty;
			this.ItemCount = 0;
			this.CanScoringSaleRegister = true;
		}

		/// <summary>Param model</summary>
		public ScoringSaleListParamModel ParamModel { get; set; }
		/// <summary>Items</summary>
		public ScoringSaleListItemDetailViewModel[] Items { get; set; }
		/// <summary>Open detail Scoring sale id</summary>
		public string OpenDetailScoringSaleId { get; set; }
		/// <summary>Item count</summary>
		public int ItemCount { get; set; }
		/// <summary>Can scoring sale register</summary>
		public bool CanScoringSaleRegister { get; set; }
	}

	/// <summary>
	/// Scoring sale list item detail view model
	/// </summary>
	public class ScoringSaleListItemDetailViewModel : ViewModelBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ScoringSaleListItemDetailViewModel()
		{
			this.PublishStatus = Constants.FLG_SCORINGSALE_PUBLISH_STATUS_UNPUBLISHED;
			this.UsePublicRange = false;
		}

		/// <summary>Scoring sale id</summary>
		public string ScoringSaleId { get; set; }
		/// <summary>Scoring sale title</summary>
		public string ScoringSaleTitle { get; set; }
		/// <summary>Page url</summary>
		public string PageUrl { get; set; }
		/// <summary>Date changed 1</summary>
		public string DateChanged1 { get; set; }
		/// <summary>Date changed 2</summary>
		public string DateChanged2 { get; set; }
		/// <summary>View count</summary>
		public long ViewCount { get; set; }
		/// <summary>CVR</summary>
		public string CVR { get; set; }
		/// <summary>Count</summary>
		public long Count { get; set; }
		/// <summary>Price</summary>
		public decimal Price { get; set; }
		/// <summary>Publish status</summary>
		public string PublishStatus { get; set; }
		/// <summary>Design mode</summary>
		public string DesignMode { get; set; }
		/// <summary>Use public range</summary>
		public bool UsePublicRange { get; set; }
		/// <summary>Thumbnail url pc</summary>
		public string ThumbnailUrlPc { get; set; }
		/// <summary>Thumbnail url sp</summary>
		public string ThumbnailUrlSp { get; set; }
		/// <summary>Scoring sale chart view model</summary>
		public ScoringSaleChartViewModel[] ScoringSaleChartViewModel { get; set; }
	}
}