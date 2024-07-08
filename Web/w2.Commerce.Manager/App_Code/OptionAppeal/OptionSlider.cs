/*
=========================================================================================================
  Module      : オプションスライダークラス (OptionSlider.cs)
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
	/// オプションスライダークラス
	/// </summary>
	[Serializable]
	public class OptionSlider
	{
		/// <summary>スライダーID</summary>
		private const string OPTIONAPPEAL_SLIDER_ID = "SliderId";

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OptionSlider()
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
		public OptionSlider(
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
		/// オプションスライダー情報変更
		/// </summary>
		/// <param name="baseOptionSliderModelList">変更元</param>
		/// <param name="changeOptionSliderModelList">変更データ</param>
		/// <returns>オプションスライダー</returns>
		public List<OptionSlider> ChangeOptionSliders(
			List<OptionSlider> baseOptionSliderModelList,
			List<OptionSlider> changeOptionSliderModelList)
		{
			foreach (var baseOptionSliderModel in baseOptionSliderModelList)
			{
				var canPlan = changeOptionSliderModelList.FirstOrDefault(p => p.SliderId == baseOptionSliderModel.SliderId);
				if (canPlan == null) continue;

				baseOptionSliderModel.SliderId = canPlan.SliderId;
				baseOptionSliderModel.ImagePath = canPlan.ImagePath;
				baseOptionSliderModel.OptionId = canPlan.OptionId;
				baseOptionSliderModel.Visible = canPlan.Visible;
			}

			return baseOptionSliderModelList;
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
