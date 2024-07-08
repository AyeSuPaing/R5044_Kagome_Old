/*
=========================================================================================================
  Module      : Order History Api Input (OrderHistoryApiInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.Common.Util;

namespace w2.App.Common.CrossPoint.OrderHistory
{
	/// <summary>
	/// 伝票取得入力クラス
	/// </summary>
	public class OrderHistoryApiInput
	{
		/// <summary>取得開始番号</summary>
		private const int DEFAULT_START_NO = 1;
		/// <summary>取得最終番号</summary>
		private const int DEFAULT_END_NO = 100;
		/// <summary>削除済みを含まない</summary>
		private const string EXCLUDE_DELETE_ORDER = "1";
		/// <summary>ショップ区分 実店舗</summary>
		private const string SHOP_DIVISION_REAL_SHOP_ONLY = "1";

		/// <summary>リクエスト種別</summary>
		public enum RequestType
		{
			List,
			Detail,
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="memberId">会員ID</param>
		public OrderHistoryApiInput(string memberId)
		{
			this.MemberId = memberId;
			this.StartDate = DateTime.Now.AddYears(-2);
			this.EndDate = DateTime.Now;
			this.DeleteFlg = EXCLUDE_DELETE_ORDER;
			this.ShopCode = Constants.CROSS_POINT_AUTH_SHOP_CODE;
			this.StartNo = DEFAULT_START_NO;
			this.EndNo = DEFAULT_END_NO;
			this.Condition = string.Empty;
			this.ShopDivision = SHOP_DIVISION_REAL_SHOP_ONLY;
		}

		/// <summary>
		/// パラメータ取得
		/// </summary>
		/// <param name="type">リクエスト種別</param>
		/// <returns>パラメータ</returns>
		public Dictionary<string, string> GetParam(RequestType type)
		{
			var param = new Dictionary<string, string>();
			if (type == RequestType.List)
			{
				param.Add(Constants.CROSS_POINT_PARAM_ORDER_HISTORY_MEMBER_ID, this.MemberId);
				param.Add(Constants.CROSS_POINT_PARAM_ORDER_HISTORY_START_DATE, StringUtility.ToDateString(this.StartDate, "yyyy/MM/dd HH:mm:ss"));
				param.Add(Constants.CROSS_POINT_PARAM_ORDER_HISTORY_END_DATE, StringUtility.ToDateString(this.EndDate, "yyyy/MM/dd HH:mm:ss"));
				param.Add(Constants.CROSS_POINT_PARAM_ORDER_HISTORY_DELETE_FLG, this.DeleteFlg);
				param.Add(Constants.CROSS_POINT_PARAM_ORDER_HISTORY_START_NO, this.StartNo.ToString());
				param.Add(Constants.CROSS_POINT_PARAM_ORDER_HISTORY_END_NO, this.EndNo.ToString());
				param.Add(Constants.CROSS_POINT_PARAM_ORDER_HISTORY_CONDITION, this.Condition);
				param.Add(Constants.CROSS_POINT_PARAM_ORDER_HISTORY_SHOP_DIVISION, this.ShopDivision);
			}
			else if (type == RequestType.Detail)
			{
				param.Add(Constants.CROSS_POINT_PARAM_ORDER_HISTORY_ORDER_ID, this.OrderNo);
			}
			return param;
		}

		/// <summary>会員ID</summary>
		public string MemberId { get; set; }
		/// <summary>注文日時(開始)</summary>
		public DateTime? StartDate { get; set; }
		/// <summary>注文日時(終了)</summary>
		public DateTime? EndDate { get; set; }
		/// <summary>削除フラグ</summary>
		public string DeleteFlg { get; set; }
		/// <summary>受注店舗コード</summary>
		public string ShopCode { get; set; }
		/// <summary>取得開始番号</summary>
		public int StartNo { get; set; }
		/// <summary>取得最終番号</summary>
		public int EndNo { get; set; }
		/// <summary>並び順</summary>
		public string Condition { get; set; }
		/// <summary>ショップ区分</summary>
		public string ShopDivision { get; set; }
		/// <summary>伝票番号</summary>
		public string OrderNo { get; set; }
	}
}
