/*
=========================================================================================================
  Module      : CrossMall用定数定義クラス(CrossMallConstants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.CrossMall
{
	/// <summary>
	/// CrossMall用定数定義クラス
	/// </summary>
	public static class CrossMallConstants
	{
		/// <summary> パラメータキー：アカウント </summary>
		public const string CONST_PARAM_KEY_NAME_ACCOUNT = "account";
		/// <summary> パラメータキー：署名 </summary>
		public const string CONST_PARAM_KEY_NAME_SIGN = "signing";
		/// <summary> パラメータキー：注文番号 </summary>
		public const string CONST_PARAM_KEY_NAME_ORDER_CODE = "order_code";

		/// <summary> 結果のステータスフラグ：失敗 </summary>
		public const string FLG_RESULT_STATUS_ERROR_VALUE = "error";
		/// <summary> 結果のステータスフラグ：成功 </summary>
		public const string FLS_RESULT_STATUS_SUCCESS_VALUE = "success";
	}
}
