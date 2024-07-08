/*
=========================================================================================================
  Module      : オプションポピュラースライダークラス (PopularOptionSlider.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace OptionAppeal
{
	/// <summary>
	/// オプションポピュラースライダークラス
	/// </summary>
	[Serializable]
	public class PopularOptionSlider
	{
		/// <summary>人気スライダーID</summary>
		private const string OPTIONAPPEAL_POPULAR_SLIDER_ID = "SliderId";

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PopularOptionSlider()
		{
			this.SliderId = string.Empty;
			this.ImagePath = string.Empty;
			this.OptionId = string.Empty;
			this.Visible = string.Empty;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sliderId">スライダーID</param>
		/// <param name="imagePath">アイコンパス</param>
		/// <param name="optionId">オプションID</param>
		/// <param name="visible">表示</param>
		public PopularOptionSlider(
			string sliderId,
			string imagePath,
			string optionId,
			string visible)
		{
			this.SliderId = sliderId;
			this.ImagePath = imagePath;
			this.OptionId = optionId;
			this.Visible = visible;
		}

		/// <summary>
		/// 人気オプションスライダー情報変更
		/// </summary>
		/// <param name="basePopularOptionSliderModelList">変更元</param>
		/// <param name="changePopularOptionSliderModelList">変更データ</param>
		/// <returns>人気オプションスライダー</returns>
		public List<PopularOptionSlider> ChangePopularOptionSliders(
			List<PopularOptionSlider> basePopularOptionSliderModelList,
			List<PopularOptionSlider> changePopularOptionSliderModelList)
		{
			foreach (var basePopularOptionSliderModel in basePopularOptionSliderModelList)
			{
				var canPlan = changePopularOptionSliderModelList.FirstOrDefault(p => p.SliderId == basePopularOptionSliderModel.SliderId);
				if (canPlan == null) continue;

				basePopularOptionSliderModel.SliderId = canPlan.SliderId;
				basePopularOptionSliderModel.ImagePath = canPlan.ImagePath;
				basePopularOptionSliderModel.OptionId = canPlan.OptionId;
				basePopularOptionSliderModel.Visible = canPlan.Visible;
			}

			return basePopularOptionSliderModelList;
		}

		/// <summary>スライダーID</summary>
		public string SliderId { get; set; }
		/// <summary>アイコンパス</summary>
		public string ImagePath { get; set; }
		/// <summary>オプションID</summary>
		public string OptionId { get; set; }
		/// <summary>表示</summary>
		public string Visible { get; set; }
	}
}