/*
=========================================================================================================
  Module      : Amazon注文モデルクラス(AmazonOrderModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using MarketplaceWebServiceOrders.Model;
using w2.Domain.Order;
using w2.Domain.User;

namespace w2.Commerce.Batch.LiaiseAmazonMall.Amazon
{
	/// <summary>
	/// Amazon注文モデルクラス
	/// </summary>
	public class AmazonOrderModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AmazonOrderModel(){}
		#endregion

		#region プロパティ
		/// <summary>Amazon注文情報</summary>
		public Order Order { get; set; }
		/// <summary>Amazon注文商品情報</summary>
		public List<OrderItem> OrderItem { get; set; }
		/// <summary>デフォルト配送会社ID</summary>
		public string DefaultDeliveryCompanyId { get; set; }
		/// <summary>デフォルト会員ランクID</summary>
		public string DefaultMemberRankId { get; set ;}
		/// <summary>ユーザ情報</summary>
		public UserModel User { get; set; }
		/// <summary>ユーザID</summary>
		public string UserId { get; set; }
		/// <summary>ユーザ区分</summary>
		public string UserKbn { get; set; }
		/// <summary>新規ユーザか</summary>
		public bool IsNewUser { get; set; }
		/// <summary>取込済み注文情報</summary>
		public OrderModel ImportedOrder { get; set; }
		/// <summary>削除が必要なユーザID(仮注文取込時に登録した仮ユーザ)</summary>
		public string DeleteNecessaryUserId { get; set; }
		#endregion
	}
}
