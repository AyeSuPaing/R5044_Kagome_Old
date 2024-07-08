/*
=========================================================================================================
  Module      : 後払いAPIすべての変更 (AtobaraicomApiAllModification.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 後払いAPIすべての変更
	/// </summary>
	public class AtobaraicomApiAllModification
	{
		/// <summary>事業者ID</summary>
		public string EnterpriseId { get; set; }
		/// <summary>APIユーザーID</summary>
		public int ApiUserId { get; set; }
		/// <summary>注文ID</summary>
		public string OrderId { get; set; }
		/// <summary>注文日</summary>
		public DateTime O_ReceiptOrderDate { get; set; }
		/// <summary>受付サイトID</summary>
		public string O_SiteId { get; set; }
		/// <summary>任意注文番号</summary>
		public string O_Ent_OrderId { get; set; }
		/// <summary>請求金額合計</summary>
		public int O_UserAmount { get; set; }
		/// <summary>役務提供予定日 </summary>
		public DateTime O_ServicesProvidedDate { get; set; }
		/// <summary>OEM用任意注文番号</summary>
		public string O_OEM_OrderId { get; set; }
		/// <summary>郵便番号</summary>
		public string C_PostalCode { get; set; }
		/// <summary>住所</summary>
		public string C_UnitingAddress { get; set; }
		/// <summary>氏名</summary>
		public string C_NameKj { get; set; }
		/// <summary>氏名カナ</summary>
		public string C_NameKn { get; set; }
		/// <summary>お電話番号</summary>
		public string C_Phone { get; set; }
		/// <summary>メールアドレス</summary>
		public string C_MailAddress { get; set; }
		/// <summary>法人名</summary>
		public string C_CorporateName { get; set; }
		/// <summary>加盟店顧客番号</summary>
		public string C_EntCustId { get; set; }
		/// <summary>部署名</summary>
		public string C_DivisionName { get; set; }
		/// <summary>担当者名</summary>
		public string C_CpNameKj { get; set; }
		/// <summary>請求書別送</summary>
		public string C_SeparateShipment { get; set; }
		/// <summary>請求書別送</summary>
		public string O_AnotherDeliFlg { get; set; }
		/// <summary>請求書別送</summary>
		public string D_PostalCode { get; set; }
		/// <summary>請求書別送</summary>
		public string D_UnitingAddress { get; set; }
		/// <summary>氏名</summary>
		public string D_DestNameKj { get; set; }
		/// <summary>氏名</summary>
		public string D_DestNameKn { get; set; }
		/// <summary>氏名</summary>
		public string D_Phone { get; set; }
		/// <summary>購入品目</summary>
		public string I_ItemNameKj { get; set; }
		/// <summary>単価</summary>
		public int I_UnitPrice { get; set; }
		/// <summary>数量</summary>
		public int I_ItemNum { get; set; }
		/// <summary>消費税率</summary>
		public int I_TaxRate { get; set; }
		/// <summary>商品送料</summary>
		public int I_ItemCarriage { get; set; }
		/// <summary>店舗手数料</summary>
		public int I_OutsideTax { get; set; }
		/// <summary>与信結果情報</summary>
		public string O_RtOrderStatus { get; set; }
	}
}
