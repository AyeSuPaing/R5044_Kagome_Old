/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 住所クラス(PaymentYamatoKaAddressSplitter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;

namespace w2.App.Common.Order.Payment.YamatoKa.Utils
{
	/// <summary>
	/// ヤマト決済(後払い) 住所クラス
	/// </summary>
	/// <remarks>
	/// ヤマト後払いで送信する住所は住所1（50[byte]）と住所2（50[byte]）があります。
	/// このクラスはw2Commerceでは住所1～4をその2つの住所に分割します。
	/// </remarks>
	public class PaymentYamatoKaAddress
	{
		/// <summary>スペース文字列（分割処理を簡易にするために利用）</summary>
		private static readonly string m_spaces = string.Concat(Enumerable.Repeat("　", 50).ToArray());

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="address1">住所1</param>
		/// <param name="address2">住所2</param>
		/// <param name="address3">住所3</param>
		/// <param name="address4">住所4</param>
		public PaymentYamatoKaAddress(string address1, string address2, string address3, string address4)
		{
			// 住所結合（住所4が数字から始まる可能性を考えてスペースを入れる。）
			this.AddressOrg = address1 + address2 + address3 + "　" + address4;
		}

		/// <summary>分割元</summary>
		public string AddressOrg { get; private set; }
		/// <summary>住所1</summary>
		public string Address1
		{
			get { return (this.AddressOrg + m_spaces).Substring(0, 25).Trim(); }
		}
		/// <summary>住所2</summary>
		public string Address2
		{
			get { return (this.AddressOrg + m_spaces).Substring(25, 25).Trim(); }
		}
	}
}
