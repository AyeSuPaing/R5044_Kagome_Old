/*
=========================================================================================================
  Module      : 特集画像グループ入力クラス(FeatureImageGroupInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Input;
using w2.Domain.FeatureImage;

namespace w2.Cms.Manager.Input
{
	/// <summary>画像検索キャッシュ</summary>
	public enum ImageSearchCache
	{
		/// <summary>最新の情報を登録</summary>
		Register,
		/// <summary>消えていれば登録</summary>
		Restore,
	}

	/// <summary>
	/// 特集画像グループ入力クラス
	/// </summary>
	public class FeatureImageGroupInput : InputBase<FeatureImageGroupModel>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeatureImageGroupInput()
		{
			this.GroupId = 0;
			this.GroupName = string.Empty;
			this.LastChanged = string.Empty;
		}

		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override FeatureImageGroupModel CreateModel()
		{
			return new FeatureImageGroupModel
			{
				GroupName = this.GroupName,
				LastChanged = this.LastChanged
			};
		}

		/// <summary>グループID</summary>
		public long GroupId { get; set; }
		/// <summary>グループ名</summary>
		public string GroupName { get; set; }
		/// <summary>最終更新者</summary>
		public string LastChanged { get; set; }
	}
}
