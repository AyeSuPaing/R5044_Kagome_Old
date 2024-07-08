/*
=========================================================================================================
  Module      : ノベルティ設定モデル (NoveltyModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;

namespace w2.Domain.Novelty
{
	/// <summary>
	/// ノベルティ設定モデル
	/// </summary>
	public partial class NoveltyModel
	{
		#region 列挙体
		/// <summary>ステータス種別</summary>
		public enum StatusType
		{
			/// <summary>準備中</summary>
			Preparing,
			/// <summary>開催中</summary>
			OnGoing,
			/// <summary>一時停止</summary>
			Suspended,
			/// <summary>終了済</summary>
			Finished,
		}
		#endregion

		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>
		/// ノベルティ対象アイテム
		/// </summary>
		public NoveltyTargetItemModel[] TargetItemList
		{
			get { return (NoveltyTargetItemModel[])this.DataSource["TargetItemList"]; }
			set { this.DataSource["TargetItemList"] = value; }
		}
		/// <summary>
		/// ノベルティ付与条件
		/// </summary>
		public NoveltyGrantConditionsModel[] GrantConditionsList
		{
			get { return (NoveltyGrantConditionsModel[])this.DataSource["GrantConditionsList"]; }
			set { this.DataSource["GrantConditionsList"] = value; }
		}
		/// <summary>開催状況</summary>
		public StatusType Status
		{
			get
			{
				// 日付範囲外は準備中・終了
				if (this.DateBegin > DateTime.Now) return StatusType.Preparing;
				if ((this.DateBegin <= DateTime.Now)
					&& ((this.DateEnd.HasValue == false) || (this.DateEnd >= DateTime.Now)))
				{
					return (this.ValidFlg == Constants.FLG_NOVELTY_VALID_FLG_VALID) ? StatusType.OnGoing : StatusType.Suspended;
				}
				return StatusType.Finished; ;
			}
		}
		/// <summary>有効か</summary>
		public bool IsValid
		{
			get { return (this.ValidFlg == Constants.FLG_NOVELTY_VALID_FLG_VALID); }
		}
		/// <summary>
		/// 拡張項目_有効フラグ表示テキスト
		/// </summary>
		public string ValidFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_NOVELTY, Constants.FIELD_NOVELTY_VALID_FLG, this.ValidFlg);
			}
		}
		#endregion
	}
}
