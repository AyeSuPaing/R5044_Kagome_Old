/*
=========================================================================================================
  Module      : デフォルト注文方法サービス (UserDefaultOrderSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.UserDefaultOrderSetting
{
	/// <summary>
	/// デフォルト注文方法サービス
	/// </summary>
	public class UserDefaultOrderSettingService : ServiceBase, IUserDefaultOrderSettingService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public UserDefaultOrderSettingModel Get(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserDefaultOrderSettingRepository(accessor))
			{
				var model = repository.Get(userId);
				return model;
			}
		}
		#endregion

		#region +Insert Or Update 登録または更新
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
		public void InsertOrUpdate(
			string userId,
			string paymentId,
			int? creditCardBranchNo,
			int? shippingNo,
			int? invoiceNo,
			string rakutenCvsType,
			string zeusCvsType,
			string paygentCvsType,
			string lastChanged)
		{
			var isInsert = (Get(userId) == null);
			var userDefaultOrderSetting = new UserDefaultOrderSettingModel
			{
				UserId = userId,
				PaymentId = paymentId,
				CreditBranchNo = creditCardBranchNo,
				UserShippingNo = shippingNo,
				UserInvoiceNo = invoiceNo,
				RakutenCvsType = rakutenCvsType,
				ZeusCvsType = zeusCvsType,
				PaygentCvsType = paygentCvsType,
				LastChanged = lastChanged,
			};
			// デフォルト注文方法情報が存在しない場合は新規登録、存在する場合は更新する。
			if (isInsert)
			{
				Insert(userDefaultOrderSetting);
			}
			else
			{
				Update(userDefaultOrderSetting);
			}
		}
		#endregion

		#region -Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		private void Insert(UserDefaultOrderSettingModel model)
		{
			using (var repository = new UserDefaultOrderSettingRepository())
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region -Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int Update(UserDefaultOrderSettingModel model, SqlAccessor accessor = null)
		{
			using (var repository = new UserDefaultOrderSettingRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion
	}
}
