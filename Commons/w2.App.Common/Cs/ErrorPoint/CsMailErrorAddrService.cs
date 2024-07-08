/*
=========================================================================================================
  Module      : メールエラーアドレスサービス(CsMailErrorAddrService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using w2.App.Common.Cs.MailFrom;

namespace w2.App.Common.Cs.ErrorPoint
{
	public class CsMailErrorAddrService
	{
		/// <summary>レポジトリ</summary>
		private CsMailErrorAddrRepository Repository;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository"></param>
		public CsMailErrorAddrService(CsMailErrorAddrRepository repository)
		{
			this.Repository = repository;
		}
		#endregion

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <returns>メールエラーアドレスモデル</returns>
		public CsMailErrorAddrModel Get(string mailAddr)
		{
			DataView dv = this.Repository.Get(mailAddr);
			return (dv.Count == 0) ? null : new CsMailErrorAddrModel(dv[0]);
		}
	}
}
