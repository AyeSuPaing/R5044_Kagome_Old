/*
=========================================================================================================
  Module      : 定期購入配送先情報モデル (FixedPurchaseShippingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Util;

namespace w2.Domain.FixedPurchase
{
	/// <summary>
	/// 定期購入配送先情報モデル
	/// </summary>
	public partial class FixedPurchaseShippingModel
	{
		#region メソッド
		/// <summary>
		/// <para>配送先情報比較</para>
		/// <para>配送先情報が等価であることを調べる</para>
		/// </summary>
		/// <param name="compareTargetModel">比較対象となる配送情報モデル</param>
		/// <param name="isShippingAddrJp">配送先住所が日本か</param>
		/// <returns>配送情報が一致する</returns>
		public bool IsSameAddress(FixedPurchaseShippingModel compareTargetModel, bool isShippingAddrJp)
		{
			var isSame = true;

			isSame &= (this.ShippingName == compareTargetModel.ShippingName);
			isSame &= (this.ShippingName1 == compareTargetModel.ShippingName1);
			isSame &= (this.ShippingName2 == compareTargetModel.ShippingName2);
			isSame &= (this.ShippingZip == compareTargetModel.ShippingZip);
			isSame &= (this.ShippingAddr == compareTargetModel.ShippingAddr);
			isSame &= (this.ShippingAddr2 == compareTargetModel.ShippingAddr2);
			isSame &= (this.ShippingAddr3 == compareTargetModel.ShippingAddr3);
			isSame &= (this.ShippingTel1 == compareTargetModel.ShippingTel1);

			if (isShippingAddrJp)
			{
				isSame &= (this.ShippingNameKana == compareTargetModel.ShippingNameKana);
				isSame &= (this.ShippingNameKana1 == compareTargetModel.ShippingNameKana1);
				isSame &= (this.ShippingNameKana2 == compareTargetModel.ShippingNameKana2);
				isSame &= (this.ShippingAddr1 == compareTargetModel.ShippingAddr1);
			}
			else
			{
				isSame &= (this.ShippingAddr4 == compareTargetModel.ShippingAddr4);
				isSame &= (this.ShippingAddr5 == compareTargetModel.ShippingAddr5);
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				isSame &= (this.ShippingCountryIsoCode == compareTargetModel.ShippingCountryIsoCode);
			}

			return isSame;
		}

		/// <summary>
		/// <para>配送先情報セット</para>
		/// </summary>
		/// <param name="srcModel">定期配送先情報</param>
		public void SetAddress(FixedPurchaseShippingModel srcModel)
		{
			this.ShippingName = srcModel.ShippingName;
			this.ShippingName1 = srcModel.ShippingName1;
			this.ShippingName2 = srcModel.ShippingName2;
			this.ShippingNameKana = srcModel.ShippingNameKana;
			this.ShippingNameKana1 = srcModel.ShippingNameKana1;
			this.ShippingNameKana2 = srcModel.ShippingNameKana2;
			this.ShippingZip = srcModel.ShippingZip;
			this.ShippingCountryIsoCode = srcModel.ShippingCountryIsoCode;
			this.ShippingAddr1 = srcModel.ShippingAddr1;
			this.ShippingAddr2 = srcModel.ShippingAddr2;
			this.ShippingAddr3 = srcModel.ShippingAddr3;
			this.ShippingAddr4 = srcModel.ShippingAddr4;
			this.ShippingAddr5 = srcModel.ShippingAddr5;
			this.ShippingTel1 = srcModel.ShippingTel1;
		}

		/// <summary>
		/// 指定された定期購入配送先枝番とこのオブジェクトの定期購入配送先枝番が一致するかどうか。
		/// </summary>
		/// <param name="shippingNo">定期購入配送先枝番</param>
		/// <returns>一致する場合、True</returns>
		public bool MatchesFixedPurchaseShippingNo(int shippingNo)
		{
			var result = (shippingNo == this.FixedPurchaseShippingNo);
			return result;
		}
		#endregion

		#region プロパティ
		/// <summary>商品リスト</summary>
		public FixedPurchaseItemModel[] Items
		{
			get { return (FixedPurchaseItemModel[])this.DataSource["EX_Items"]; }
			set { this.DataSource["EX_Items"] = value; }
		}
		/// <summary>住所</summary>
		public string ShippingAddr
		{
			get
			{
				return StringUtility.ToEmpty(this.ShippingAddr1)
					+ StringUtility.ToEmpty(this.ShippingAddr2)
					+ StringUtility.ToEmpty(this.ShippingAddr3)
					+ StringUtility.ToEmpty(this.ShippingAddr4)
					+ StringUtility.ToEmpty(this.ShippingAddr5)
					+ (Constants.GLOBAL_OPTION_ENABLE ? StringUtility.ToEmpty(this.ShippingCountryName) : "");
			}
		}
		/// <summary>メール便か</summary>
		public bool IsMail
		{
			get { return this.ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL; }
		}
		/// <summary>郵便番号（ハイフンなし）</summary>
		public string HyphenlessShippingZip
		{
			get { return this.ShippingZip.Replace("-", ""); }
		}
		#endregion
	}
}
