/*
=========================================================================================================
  Module      : User Business Owner Repository(UserBusinessOwnerRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Sql;

namespace w2.Domain.UserBusinessOwner
{
	class UserBusinessOwnerRepository : RepositoryBase
	{
		/// <summary>ユーザーSQLファイル</summary>
		private const string XML_KEY_NAME = "UserBusinessOwner";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserBusinessOwnerRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserBusinessOwnerRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>モデル</returns>
		public UserBusinessOwnerModel GetByUserId(string userId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetByUserId", input);
			if (dv.Count == 0) return null;
			return new UserBusinessOwnerModel(dv[0]);
		}

		/// <summary>
		/// 与信ステータスによるデータ取得
		/// </summary>
		/// <param name="CreditStatus">与信状況</param>
		/// <returns>与信ステータス</returns>
		public UserBusinessOwnerModel[] GetByCreditStatus(string CreditStatus)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_USER_BUSINESS_OWNER_CREDIT_STATUS, CreditStatus},
			};
			var dv = Get(XML_KEY_NAME, "GetByCreditStatus", input);
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new UserBusinessOwnerModel(drv)).ToArray();
		}

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Insert(UserBusinessOwnerModel model)
		{
			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource);
			return result;
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(UserBusinessOwnerModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}

		/// <summary>
		/// 審査中のGMOユーザーリスト取得
		/// </summary>
		/// <returns>ユーザーリスト</returns>
		public UserBusinessOwnerModel[] GetFrameGuaranteeNeedUpdate()
		{
			var dv = Get(XML_KEY_NAME, "GetFrameGuaranteeNeedUpdate");
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new UserBusinessOwnerModel(drv)).ToArray();
		}

		/// <summary>
		/// クレジットステータス更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="status">ステータス</param>
		public int UpdateCreditStatus(string userId, string status)
		{
			var input = new Hashtable
				{
					{Constants.FIELD_USER_BUSINESS_OWNER_CREDIT_STATUS, status},
					{Constants.FIELD_USER_BUSINESS_OWNER_USER_ID, userId},
					{Constants.FIELD_USER_BUSINESS_OWNER_DATE_CHANGED, DateTime.Now}
				};
			var result = Exec(XML_KEY_NAME, "UpdateCreditStatus", input);
			return result;
		}
	}
}