/*
=========================================================================================================
  Module      : メールエラーアドレスサービス (MailErrorAddrService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Transactions;

namespace w2.Domain.MailErrorAddr
{
	/// <summary>
	/// メールエラーアドレスサービス
	/// </summary>
	public class MailErrorAddrService : ServiceBase
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="mailAddr">対象メールアドレス</param>
		/// <returns>モデル</returns>
		public MailErrorAddrModel Get(string mailAddr)
		{
			using (var repository = new MailErrorAddrRepository())
			{
				var model = repository.Get(mailAddr);
				return model;
			}
		}
		#endregion

		/// <summary>
		/// エラーポイント追加（レコード追加）
		/// </summary>
		/// <param name="mailAddr">対象メールアドレス</param>
		/// <param name="errorPoint">追加エラーポイント</param>
		/// <returns>更新件数</returns>
		public int AddErrorPoint(string mailAddr, int errorPoint)
		{
			using (var repository = new MailErrorAddrRepository())
			{
				var result = repository.Upsert(mailAddr, errorPoint);
				return result;
			}
		}
	}
}