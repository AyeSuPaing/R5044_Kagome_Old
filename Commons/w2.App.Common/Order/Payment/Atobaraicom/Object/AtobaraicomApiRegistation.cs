/*
=========================================================================================================
  Module      : 後払いAPI登録 (AtobaraicomApiRegistation.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 後払いAPI登録
	/// </summary>
	public class AtobaraicomApiRegistation
	{
		/// <summary>注文日</summary>
		public DateTime OReceiptOrderDate { get; set; }
		/// <summary>受付事業者ID</summary>
		public string OEnterpriseId { get; set; }
		/// <summary>受付サイトID</summary>
		public string OSiteId { get; set; }
		/// <summary>APIユーザーID</summary>
		public string OApiUserId { get; set; }
		/// <summary>任意注文番号</summary>
		public string OEntOrderId { get; set; }
		/// <summary>備考（メモ）</summary>
		public string OEntNote { get; set; }
		/// <summary>請求金額合計</summary>
		public int OUseAmount { get; set; }
		/// <summary>OEM用任意注文番号</summary>
		public string OOEMOrderId { get; set; }
		/// <summary>郵便番号</summary>
		public string CPostalCode { get; set; }
		/// <summary>住所</summary>
		public string CUnitingAddress { get; set; }
		/// <summary>氏名</summary>
		public string CNameKj { get; set; }
		/// <summary>氏名</summary>
		public string CNameKn { get; set; }
		/// <summary>電話番号</summary>
		public string CPhone { get; set; }
		/// <summary>メールアドレス</summary>
		public string CMailAddress { get; set; }
		/// <summary>職業</summary>
		public string COccupation { get; set; }
		/// <summary>別配送先指定</summary>
		public string OAnotherDeliFlg { get; set; }
		/// <summary>郵便番号</summary>
		public string DPostalCode { get; set; }
		/// <summary>住所</summary>
		public string DUnitingAddress { get; set; }
		/// <summary>氏名</summary>
		public string DDestNameKj { get; set; }
		/// <summary>氏名カナ</summary>
		public string DDestNameKn { get; set; }
		/// <summary>電話番号</summary>
		public string DPhone { get; set; }
		/// <summary>購入品目１</summary>
		public string IItemNameKj { get; set; }
		/// <summary>単価</summary>
		public int IUnitPrice { get; set; }
		/// <summary>数量</summary>
		public int IItemNum { get; set; }
		/// <summary>消費税率</summary>
		public int ITaxRate { get; set; }
		/// <summary>消費税率</summary>
		public int IItemCarriage { get; set; }
		/// <summary>店舗手数料</summary>
		public int IItemCharge { get; set; }
		/// <summary>与信結果情報</summary>
		public string O_RtOrderStatus { get; set; }
		/// <summary>新旧区分</summary>
		public int O_NewSystemFlg { get; set; }
		/// <summary>役務提供予定日</summary>
		public DateTime O_ServicesProvidedDate { get; set; }
		/// <summary>テスト注文区分</summary>
		public string O_TestOrderFlg { get; set; }
		/// <summary>テスト注文自動与信審査区分</summary>
		public string O_TestCreditResult { get; set; }
		/// <summary>加盟店顧客番号</summary>
		public string C_EntCustId { get; set; }
		/// <summary>法人名</summary>
		public string C_CorporateName { get; set; }
		/// <summary>部署名</summary>
		public string C_DivisionName { get; set; }
		/// <summary>担当者名</summary>
		public string C_CpNameKj { get; set; }
		/// <summary>請求書別送</summary>
		public string C_SeparateShipment { get; set; }
		/// <summary>外税額</summary>
		public int I_OutsideTax { get; set; }
	}
}
