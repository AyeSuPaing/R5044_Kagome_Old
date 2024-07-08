/*
=========================================================================================================
  Module      : 決済カード連携リポジトリ (UserCreditCardRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.UserCreditCard
{
	/// <summary>
	/// 決済カード連携リポジトリ
	/// </summary>
	public class UserCreditCardRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "UserCreditCard";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserCreditCardRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserCreditCardRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <returns>モデル</returns>
		public UserCreditCardModel Get(string userId, int branchNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCREDITCARD_USER_ID, userId},
				{Constants.FIELD_USERCREDITCARD_BRANCH_NO, branchNo},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new UserCreditCardModel(dv[0]);
		}
		#endregion

		#region GetByUserId ユーザーIDで取得
		/// <summary>
		/// ユーザーIDで取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>モデル</returns>
		public UserCreditCardModel[] GetByUserId(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCREDITCARD_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetByUserId", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserCreditCardModel(drv)).ToArray();
		}
		#endregion

		#region +GetByCooperationId1 連携ID1から取得
		/// <summary>
		/// 連携ID1から取得
		/// </summary>
		/// <param name="cooperationId1">連携ID1</param>
		/// <returns>モデル</returns>
		public UserCreditCardModel GetByCooperationId1(string cooperationId1)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCREDITCARD_COOPERATION_ID, cooperationId1},
			};
			var dv = Get(XML_KEY_NAME, "GetByCooperationId1", ht);
			if (dv.Count == 0) return null;
			return new UserCreditCardModel(dv[0]);
		}
		#endregion

		#region +GetByUserIdAndCooperationId ユーザーIDと連携ID1から取得
		/// <summary>
		/// ユーザーIDと連携ID1から取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="cooperationId">連携ID1</param>
		/// <returns>モデル</returns>
		public UserCreditCardModel[] GetByUserIdAndCooperationId(string userId, string cooperationId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERCREDITCARD_USER_ID, userId },
				{ Constants.FIELD_USERCREDITCARD_COOPERATION_ID, cooperationId },
			};
			var dv = Get(XML_KEY_NAME, "GetByUserIdAndCooperationId", ht);
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new UserCreditCardModel(drv)).ToArray();
		}
		#endregion

		#region +GetUsableOrUnregisterd  利用可能かユーザークレカ未登録のものを取得（管理画面で利用）
		/// <summary>
		///  利用可能かユーザークレカ未登録のものを取得（管理画面で利用）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>ユーザークレジットカード情報</returns>
		public UserCreditCardModel[] GetUsableOrUnregisterd(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCREDITCARD_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetUsableOrUnregisterd", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserCreditCardModel(drv)).ToArray();
		}
		#endregion

		#region +GetUsableOrByBranchno 利用可能か枝番から取得（管理画面で利用）
		/// <summary>
		/// 利用可能か枝番から取得（管理画面で利用）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		/// <returns>ユーザークレジットカード情報</returns>
		public UserCreditCardModel[] GetUsableOrByBranchno(string userId, int branchNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCREDITCARD_USER_ID, userId},
				{Constants.FIELD_USERCREDITCARD_BRANCH_NO, branchNo},
			};
			var dv = Get(XML_KEY_NAME, "GetUsableOrByBranchno", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserCreditCardModel(drv)).ToArray();
		}
		#endregion

		#region +GetUsable 利用可能な物取得
		/// <summary>
		/// 利用可能な物取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>ユーザークレジットカード情報</returns>
		public UserCreditCardModel[] GetUsable(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCREDITCARD_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetUsable", ht);
			return dv.Cast<DataRowView>().Select(drv => new UserCreditCardModel(drv)).ToArray();
		}
		#endregion
		
		#region +GetMaxBranchNo 枝番の最大値取得
		/// <summary>
		/// 枝番の最大値取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>ユーザークレジットカード情報</returns>
		public int GetMaxBranchNo(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCREDITCARD_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetMaxBranchNo", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region +GetDeleteMemberByEScott e-SCOTTの削除対象会員のクレカ取得
		/// <summary>
		/// e-SCOTTの削除対象会員のクレカ取得
		/// </summary>
		/// <param name="userIdList">ユーザーIDリスト</param>
		/// <param name="dateFrom">日付(From)</param>
		/// <param name="dateTo">日付(To)</param>
		/// <returns>ユーザークレジットカード情報</returns>
		public UserCreditCardModel[] GetDeleteMemberByEScott(
			List<string> userIdList,
			DateTime dateFrom,
			DateTime dateTo)
		{
			if (userIdList.Any() == false) return new UserCreditCardModel[0];

			var userIdQuotation = userIdList.Select(ui => "'" + ui + "'");
			var userIdsParam = string.Join(",", userIdQuotation);

			var ht = new Hashtable
			{
				{ "auth_date_from", dateFrom },
				{ "auth_date_to", dateTo },
			};
			var replaceString = new KeyValuePair<string, string>("@@ user_ids @@", userIdsParam);
			var dv = Get(XML_KEY_NAME, "GetDeleteMemberByEScott", ht, null, replaceString);
			return dv.Cast<DataRowView>().Select(drv => new UserCreditCardModel(drv)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(UserCreditCardModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(UserCreditCardModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +UpdateCooperationId2 連携ID2のみ更新
		/// <summary>
		/// 連携ID2のみ更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateCooperationId2(UserCreditCardModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateCooperationId2", model.DataSource);
			return result;
		}
		#endregion

		#region ~UpdateCardDisplayName カード名更新
		/// <summary>
		/// カード更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateCardDisplayName(UserCreditCardModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateCardDisplayName", model.DataSource);
			return result;
		}
		#endregion
		
		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="userId">ユーザID</param>
		/// <param name="branchNo">カード枝番</param>
		public int Delete(string userId, int branchNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERCREDITCARD_USER_ID, userId},
				{Constants.FIELD_USERCREDITCARD_BRANCH_NO, branchNo},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region GetMaxBranchNoByUserIdAndCooperationType
		/// <summary>
		/// Get Max Branch No By User Id And Cooperation Type
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="cooperationType">Cooperation Type</param>
		/// <returns>Max Branch No</returns>
		public int GetMaxBranchNoByUserIdAndCooperationType(
			string userId,
			string cooperationType)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_USERCREDITCARD_USER_ID, userId },
				{ Constants.FIELD_USERCREDITCARD_COOPERATION_TYPE, cooperationType },
			};
			var data = Get(XML_KEY_NAME, "GetMaxBranchNoByUserIdAndCooperationType", input);

			return (int)data[0][0];
		}
		#endregion

		#region +GetUserCreditCardExpiredForPaymentPaidys
		/// <summary>
		/// Get User Credit Card Expired For Payment Paidys
		/// </summary>
		/// <param name="paymentPaidyTokenDeleteLimitDay">Payment Paidy Token Delete Limit Day</param>
		/// <param name="cooperationType">Cooperation Type</param>
		/// <param name="maskString">Mask String</param>
		/// <returns>User Credit Card Expired For Payment Paidys</returns>
		public List<UserCreditCardModel> GetUserCreditCardExpiredForPaymentPaidys(
			int paymentPaidyTokenDeleteLimitDay, 
			string cooperationType,
			string maskString)
		{
			var targetDate = DateTime.Now.AddDays(-1 * paymentPaidyTokenDeleteLimitDay).Date;
			var input = new Hashtable
			{
				{ "target_date", targetDate },
				{ Constants.FIELD_USERCREDITCARD_COOPERATION_TYPE, cooperationType },
				{ "mask_string", maskString },
			};
			var data = Get(XML_KEY_NAME, "GetUserCreditCardExpiredForPaymentPaidys", input);
			var result = data.Cast<DataRowView>()
				.Select(item => new UserCreditCardModel(item))
				.ToList();

			return result;
		}
		#endregion
	}
}
