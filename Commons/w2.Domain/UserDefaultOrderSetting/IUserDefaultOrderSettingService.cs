/*
=========================================================================================================
  Module      : デフォルト注文方法サービスのインタフェース(IUserDefaultOrderSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.UserDefaultOrderSetting
{
	/// <summary>
	/// デフォルト注文方法サービスのインタフェース
	/// </summary>
	public interface IUserDefaultOrderSettingService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		UserDefaultOrderSettingModel Get(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// 登録または更新
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <param name="creditCardBranchNo">クレジットカード枝番</param>
		/// <param name="shippingNo">配送先枝番</param>
		/// <param name="invoiceNo">Invoice No</param>
		/// <param name="rakutenCvsType">楽天コンビニ前払い支払いコンビニ</param>
		/// <param name="zeusCvsType">Zeusコンビニ前払い支払いコンビニ</param>
		/// <param name="paygentCvsType">Paygentコンビニ前払い支払いコンビニ</param>
		/// <param name="lastChanged">最終更新者</param>
		void InsertOrUpdate(
			string userId,
			string paymentId,
			int? creditCardBranchNo,
			int? shippingNo,
			int? invoiceNo,
			string rakutenCvsType,
			string zeusCvsType,
			string paygentCvsType,
			string lastChanged);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		int Update(UserDefaultOrderSettingModel model, SqlAccessor accessor = null);
	}
}
