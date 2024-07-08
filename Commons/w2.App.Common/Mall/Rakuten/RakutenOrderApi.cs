/*
=========================================================================================================
  Module      : 楽天API連携のラッパークラス (RakutenOrderApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;

namespace w2.App.Common.Mall.Rakuten
{
	/// <summary>
	/// 楽天受注API(WebService)を使用するためのラッパークラス
	/// </summary>
	public class RakutenOrderApi
	{
		/// <summary>楽天受注APIの最大検索可能期間</summary>
		public const int RAKUTEN_ORDER_SEARCH_PERIOD = 63;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mallSetting">モール設定情報</param>
		public RakutenOrderApi(DataRowView mallSetting)
		{
			// 受注API用インスタンス生成
			this.OrderApi = new OrderApiService();
			if (!string.IsNullOrEmpty(ApiUrl))
			{
				this.OrderApi.Url = ApiUrl;
			}

			// 受注API使用認証モデル生成
			this.UserAuth = new userAuthModel();
			// 認証用ユーザー名 ※新規認証では不要だがタグは必要。Nullの場合にタグが出力されないため値を設定する。
			this.UserAuth.userName = (string)mallSetting[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_USER_NAME];
			this.UserAuth.shopUrl = (string)mallSetting[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_SHOP_URL];	// 認証用店舗URL
			var keyBytes = System.Text.Encoding.UTF8.GetBytes(
				string.Format(
					"{0}:{1}",
					(string)mallSetting[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_SERVICE_SECRET],
					(string)mallSetting[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_API_LICENSE_KEY]
				));
			this.UserAuth.authKey = "ESA " + System.Convert.ToBase64String(keyBytes);
		}

		/// <summary>
		/// 注文IDから注文情報取得
		/// </summary>
		/// <param name="orderIdList">注文ID一覧</param>
		/// <returns>受注情報取得レスポンスモデル</returns>
		public getOrderResponseModel GetRakutenOrderInfo(string[] orderIdList)
		{
			// 受注情報の取得条件を設定
			var getOrderRequest = new getOrderRequestModel();
			getOrderRequest.orderNumber = orderIdList;
			getOrderRequest.orderSearchModel = new orderSearchModel();
			getOrderRequest.orderSearchModel.dateType = Constants.RAKUTEN_API_DATE_TYPE_ORDER;						// 期間検索種別
			getOrderRequest.orderSearchModel.dateTypeSpecified = true;												// 期間検索種別有効
			getOrderRequest.orderSearchModel.startDate = DateTime.Now.AddDays((-1) * RAKUTEN_ORDER_SEARCH_PERIOD);	// 期間検索開始日
			getOrderRequest.orderSearchModel.startDateSpecified = true;												// 期間検索開始日有効
			getOrderRequest.orderSearchModel.endDate = DateTime.Now;												// 期間検索終了日：現在
			getOrderRequest.orderSearchModel.endDateSpecified = true;												// 期間検索終了日有効

			// 受注情報を取得
			return this.OrderApi.getOrder(this.UserAuth, getOrderRequest);
		}

		#region プロパティ・メンバ変数
		/// <summary>受注API用インスタンス</summary>
		public OrderApiService OrderApi
		{
			get { return m_orderApi; }
			set { m_orderApi = value; }
		}
		OrderApiService m_orderApi = null;
		/// <summary>受注API使用ユーザー認証</summary>
		public userAuthModel UserAuth
		{
			get { return m_userAuth; }
			set { m_userAuth = value; }
		}
		userAuthModel m_userAuth = null;
		/// <summary>APIのURL（値保持）</summary>
		public static string ApiUrl
		{
			get { return m_apiUrl; }
			set { m_apiUrl = value; }
		}
		private static string m_apiUrl = "";
		#endregion
	}
}