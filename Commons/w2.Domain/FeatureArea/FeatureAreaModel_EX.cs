/*
=========================================================================================================
  Module      : 特集エリアマスタモデル (FeatureAreaModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.FeatureArea
{
	/// <summary>
	/// 特集エリアマスタモデル
	/// </summary>
	public partial class FeatureAreaModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>バナー</summary>
		public FeatureAreaBannerModel[] Banners { get; set; }
		/// <summary>動作タイプ</summary>
		public string ActionType
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREAREATYPE_ACTION_TYPE]; }
			set { this.DataSource[Constants.FIELD_FEATUREAREATYPE_ACTION_TYPE] = value; }
		}
		/// <summary>パーツの利用状態</summary>
		public string UseType
		{
			get { return (string)this.DataSource[Constants.FIELD_PARTSDESIGN_USE_TYPE]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGN_USE_TYPE] = value; }
		}
		#endregion
	}
}
