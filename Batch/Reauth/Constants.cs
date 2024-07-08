/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;

namespace w2.Commerce.Reauth
{
	/// <summary>
	/// 定数定義
	/// </summary>
	class Constants : w2.App.Common.Constants
	{
		/// <summary>ZEUSクレジット与信期限切れ(日)</summary>
		public static int CREDIT_ZEUS_AUTH_EXPIRE_DAYS = 62;
		/// <summary>SBPSクレジット与信期限切れ(日)</summary>
		public static int CREDIT_SBPS_AUTH_EXPIRE_DAYS = 0;
		/// <summary>GMOクレジット与信期限切れ(日)</summary>
		public static int CREDIT_GMO_AUTH_EXPIRE_DAYS = 0;
		/// <summary>ヤマトクレジット与信期限切れ(日)</summary>
		public static int CREDIT_YAMATOKWC_AUTH_EXPIRE_DAYS = 0;
		/// <summary>ヤマトコンビニ後払い与信期限切れ(日)</summary>
		public static int CVSDEF_YAMATOKA_AUTH_EXPIRE_DAYS = 89;
		/// <summary>GMO後払い与信期限切れ(日)</summary>
		public static int CVSDEF_GMO_AUTH_EXPIRE_DAYS = 0;
		/// <summary>Atodene後払い与信期限切れ(日)</summary>
		public static int CVSDEF_ATODENE_AUTH_EXPIRE_DAYS = 0;
		/// <summary>DSK後払い与信期限切れ(日)</summary>
		public static int CVSDEF_DSK_AUTH_EXPIRE_DAYS = 0;
		/// <summary>Amazon Pay与信期限切れ(日)</summary>
		public static int AMAZONPAY_AUTH_EXPIRE_DAYS = 0;
		/// <summary>Zcomクレジット与信期限切れ(日)</summary>
		public static int CREDIT_ZCOM__AUTH_EXPIRE_DAYS = 0;
		/// <summary>ベリトランスクレジット与信期限切れ(日)</summary>
		public static int CREDIT_VERITRANS_AUTH_EXPIRE_DAYS = 400;
		/// <summary>ペイジェントクレジット与信期限切れ（日）</summary>
		public static int CREDIT_PAYGENT_AUTH_EXPIRE_DAYS = 59;
		/// <summary>Paidy与信期限切れ(日)</summary>
		public static int PAIDYPAY_AUTH_EXPIRE_DAYS = 0;
		/// <summary>LINEPay与信期限切れ(日)</summary>
		public static int LINEPAY_AUTH_EXPIRE_DAYS = 0;
		/// <summary>NP後払い与信期限切れ(日)</summary>
		public static int NPAFTERPAY_AUTH_EXPIRE_DAYS = 0;
		/// <summary>e-SCOTT与信期限切れ(日)</summary>
		public static int CREDIT_ESCOTT_AUTH_EXPIRE_DAYS = 0;
		/// <summary>スコア後払い与信期限切れ(日)</summary>
		public static int CVSDEF_SCORE_AUTH_EXPIRE_DAYS = 0;
		/// <summary>ベリトランス後払い与信期限切れ(日)</summary>
		public static int CVSDEF_VERITRANS_AUTH_EXPIRE_DAYS = 0;
		/// <summary>再与信対象外決済種別(空の場合は再与信に対応しているすべての決済を対象とする)</summary>
		public static List<string> NOT_TARGET_REAUTH_PAYMENT_IDS = new List<string>();
		/// <summary>Gmo掛け払い与信期限切れ(日)</summary>
		public static int GMOPOST_AUTH_EXPIRE_DAYS = 0;
	}
}
