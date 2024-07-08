/*
=========================================================================================================
  Module      : 取込基底クラス(ImportBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;
using w2.Commerce.Batch.LiaiseAmazonMall.Amazon;

namespace w2.Commerce.Batch.LiaiseAmazonMall.Import
{
	/// <summary>
	/// 取込基底クラス
	/// </summary>
	public abstract class ImportBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="accessor">SQLアクセサ</param>
		protected ImportBase(AmazonOrderModel amazonOrder, string mallId, SqlAccessor accessor)
		{
			this.AmazonOrder = amazonOrder;
			this.MallId = mallId;
			this.Accessor = accessor;
		}

		/// <summary>
		/// 取込
		/// </summary>
		public abstract void Import();

		#region プロパティ
		/// <summary>Amazon注文情報</summary>
		protected AmazonOrderModel AmazonOrder { get; set; }
		/// <summary>モールID</summary>
		protected string MallId { get; set; }
		/// <summary>SQLアクセサ</summary>
		protected SqlAccessor Accessor { get; set; }
		#endregion
	}
}
