/*
=========================================================================================================
  Module      : 注文登録プロパティ(OrderRegistProperties.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order.Register
{
	/// <summary>
	/// 注文登録プロパティ
	/// </summary>
	internal class OrderRegisterProperties
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="orderExecType">注文実行種別</param>
		/// <param name="isUser">ユーザーか（ポイント付与判断など）</param>
		/// <param name="lastChanged">DB最終更新者</param>
		public OrderRegisterProperties(OrderRegisterBase.ExecTypes orderExecType, bool isUser, string lastChanged)
		{
			this.OrderExecType = orderExecType;
			this.IsUser = isUser;
			this.ErrorMessages = new List<string>();
			this.AlertMessages = new List<string>();
			this.ZeusCard3DSecurePaymentOrders = new List<Hashtable>();	// HACK:本当はFrontのみにしたい
			this.GmoCard3DSecurePaymentOrders = new List<Hashtable>();
			this.SuccessOrders = new List<Hashtable>();
			this.SuccessCarts = new List<CartObject>();
			this.LinePayFailedCarts = new List<CartObject>();
			this.LastChanged = lastChanged;
			this.ZcomCard3DSecurePaymentOrders = new List<Hashtable>();
			this.VeriTrans3DSecurePaymentOrders = new List<Hashtable>();
			this.RakutenCard3DSecurePaymentOrders = new List<Hashtable>();
			this.YamatoCard3DSecurePaymentOrders = new List<Hashtable>();
			this.PaygentCard3DSecurePaymentOrders = new List<Hashtable>();
		}

		/// <summary>注文実行区分</summary>
		internal OrderRegisterBase.ExecTypes OrderExecType { get; set; }

		/// <summary>ユーザーか（ポイント付与判断など）</summary>
		internal bool IsUser { get; private set; }
		/// <summary>YamatoKwcクレジットが期限切れか</summary>
		internal bool IsExpiredYamatoKwcCredit { get; set; }
		/// <summary>エラーメッセージ</summary>
		internal List<string> ErrorMessages { get; private set; }
		/// <summary>アラートッセージ</summary>
		internal List<string> AlertMessages { get; private set; }

		/// <summary>ゼウス3Dセキュア注文</summary>
		internal List<Hashtable> ZeusCard3DSecurePaymentOrders { get; set; }
		/// <summary>GMO3Dセキュア注文</summary>
		internal List<Hashtable> GmoCard3DSecurePaymentOrders { get; set; }
		/// <summary>ヤマトKWC3Dセキュア注文</summary>
		internal List<Hashtable> YamatoCard3DSecurePaymentOrders { get; set; }
		/// <summary>ペイジェント3Dセキュア注文</summary>
		internal List<Hashtable> PaygentCard3DSecurePaymentOrders { get; set; }
		/// <summary>成功注文</summary>
		internal List<Hashtable> SuccessOrders { get; set; }
		/// <summary>成功カート</summary>
		internal List<CartObject> SuccessCarts { get; set; }
		/// <summary>LINE Pay failed carts</summary>
		internal List<CartObject> LinePayFailedCarts { get; set; }

		/// <summary>DB更新時の最終更新者</summary>
		internal string LastChanged { get; private set; }
		/// <summary>Zcom card 3DSecure payment orders</summary>
		internal List<Hashtable> ZcomCard3DSecurePaymentOrders { get; set; }
		/// <summary>ベリトランス3Dセキュア注文情報</summary>
		internal List<Hashtable> VeriTrans3DSecurePaymentOrders { get; set; }
		/// <summary>Rakuten 3D Secure Payment Orders</summary>
		internal List<Hashtable> RakutenCard3DSecurePaymentOrders { get; set; }
		/// <summary>管理者向け注文完了メール送信者種別</summary>
		internal Constants.EnabledOrderCompleteEmailSenderType OrderCompleteEmailSenderType { get; set; }
	}
}
