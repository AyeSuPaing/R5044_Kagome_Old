/*
=========================================================================================================
  Module      : レコメンド設定モデル (RecommendModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;

namespace w2.Domain.Recommend
{
	/// <summary>
	/// レコメンド設定モデル
	/// </summary>
	public partial class RecommendModel
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
		/// レコメンド適用条件アイテムリスト
		/// </summary>
		public RecommendApplyConditionItemModel[] ApplyConditionItems
		{
			get { return (RecommendApplyConditionItemModel[])this.DataSource["EX_ApplyConditionItems"]; }
			set { this.DataSource["EX_ApplyConditionItems"] = value; }
		}
		/// <summary>
		/// レコメンドアップセル対象アイテム
		/// </summary>
		public RecommendUpsellTargetItemModel UpsellTargetItem
		{
			get { return (RecommendUpsellTargetItemModel)this.DataSource["EX_UpsellTargetItem"]; }
			set { this.DataSource["EX_UpsellTargetItem"] = value; }
		}
		/// <summary>
		/// レコメンドアイテムリスト
		/// </summary>
		public RecommendItemModel[] Items
		{
			get { return (RecommendItemModel[])this.DataSource["EX_Items"]; }
			set { this.DataSource["EX_Items"] = value; }
		}
		/// <summary>
		/// 拡張項目_アップセルか？
		/// </summary>
		public bool IsUpsell
		{
			get { return (this.RecommendKbn == Constants.FLG_RECOMMEND_RECOMMEND_KBN_UP_SELL); }
		}
		/// <summary>
		/// 拡張項目_クロスセルか？
		/// </summary>
		public bool IsCrosssell
		{
			get { return (this.RecommendKbn == Constants.FLG_RECOMMEND_RECOMMEND_KBN_CROSS_SELL); }
		}
		/// <summary>
		/// 拡張項目_レコメンドHTMLか？
		/// </summary>
		public bool IsRecommendHtml
		{
			get { return (this.RecommendKbn == Constants.FLG_RECOMMEND_RECOMMEND_KBN_RECOMMEND_HTML); }
		}
		/// <summary>
		/// 拡張項目_レコメンド区分テキスト
		/// </summary>
		public string RecommendKbnText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_RECOMMEND, Constants.FIELD_RECOMMEND_RECOMMEND_KBN, this.RecommendKbn);
			}
		}
		/// <summary>拡張項目_開催状況</summary>
		public StatusType Status
		{
			get
			{
				// 日付範囲外は準備中・終了
				if (this.DateBegin > DateTime.Now) return StatusType.Preparing;
				if ((this.DateBegin <= DateTime.Now)
					&& ((this.DateEnd.HasValue == false) || (this.DateEnd >= DateTime.Now)))
				{
					return (this.ValidFlg == Constants.FLG_RECOMMEND_VALID_FLG_VALID) ? StatusType.OnGoing : StatusType.Suspended;
				}
				return StatusType.Finished;
			}
		}
		/// <summary>拡張項目_有効か</summary>
		public bool IsValid
		{
			get { return (this.ValidFlg == Constants.FLG_RECOMMEND_VALID_FLG_VALID); }
		}
		/// <summary>
		/// 拡張項目_有効フラグ表示テキスト
		/// </summary>
		public string ValidFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_RECOMMEND, Constants.FIELD_RECOMMEND_VALID_FLG, this.ValidFlg);
			}
		}
		/// <summary>ワンタイム表示フラグ有効か</summary>
		public bool IsOnetime
		{
			get { return (this.OnetimeFlg == Constants.FLG_RECOMMEND_ONETIME_FLG_VALID); }
		}
		/// <summary>注文確認画面で表示可能か</summary>
		public bool CanDisplayOrderConfirmPage
		{
			get { return (this.RecommendDisplayPage == Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_PAGE_ORDER_CONFIRM); }
		}
		/// <summary>注文完了画面で表示可能か</summary>
		public bool CanDisplayOrderCompletePage
		{
			get { return (this.RecommendDisplayPage == Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_PAGE_ORDER_COMPLETE); }
		}
		/// <summary>拡張項目_表示ページテキスト</summary>
		public string RecommendDisplayPageText
		{
			get
			{
				return ValueText.GetValueText(
					Constants.TABLE_RECOMMEND,
					Constants.FIELD_RECOMMEND_RECOMMEND_DISPLAY_PAGE,
					this.RecommendDisplayPage);
			}
		}
		/// <summary>拡張項目_PV数</summary>
		public int PvNumber
		{
			get { return (int)this.DataSource[Constants.FIELD_RECOMMEND_PV_NUMBER]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_PV_NUMBER] = value; }
		}
		/// <summary>拡張項目_CV数</summary>
		public int CvNumber
		{
			get { return (int)this.DataSource[Constants.FIELD_RECOMMEND_CV_NUMBER]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_CV_NUMBER] = value; }
		}
		/// <summary>拡張項目_CV率(%)</summary>
		public int CvPercent
		{
			get { return (this.PvNumber == 0) ? 0 : this.CvNumber * 100 / this.PvNumber; }
		}
		#endregion
	}
}
