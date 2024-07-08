/*
=========================================================================================================
  Module      : 郵便番号サービス (ZipcodeService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Transactions;
using w2.Common.Sql;
using w2.Domain.Zipcode.Helper;

namespace w2.Domain.Zipcode
{
	/// <summary>
	/// 郵便番号サービス
	/// </summary>
	public class ZipcodeService : ServiceBase
	{
		#region +GetByZipcode 住所取得
		/// <summary>
		/// 住所取得
		/// </summary>
		/// <param name="zipcode">郵便番号</param>
		/// <returns>住所モデル配列取得</returns>
		public ZipcodeModel[] GetByZipcode(string zipcode)
		{
			using (var repository = new ZipcodeRepository())
			{
				var models = repository.GetByZipcode(zipcode);
				models.ToList().ForEach(m => ZipcodeHelper.RemoveInvalidAddressPart(m));
				return models;
			}
		}
		#endregion
	}
}
