/*
=========================================================================================================
  Module      : セットプロモーション商品マスタモデル (SetPromotionItemModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;

namespace w2.Domain.SetPromotion
{
	/// <summary>
	/// セットプロモーション商品マスタモデル
	/// </summary>
	public partial class SetPromotionItemModel
	{
		/// <summary>対象商品</summary>
		public string[] ItemsList
		{
			get { return this.SetpromotionItems.Replace("\r\n", "\n").Split('\n').Where(s => s != "").ToArray(); }
		}
		/// <summary>数量以上フラグが有効か</summary>
		public bool IsOverQuantityFlg
		{
			get { return (this.SetpromotionItemQuantityMoreFlg == Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_QUANTITY_MORE_FLG_VALID); }
		}
	}
}
