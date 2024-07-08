/*
=========================================================================================================
  Module      : 定期購入情報情報更新データクラス (UpdateDataFixedPurchase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using w2.Domain.UserCreditCard;
using w2.Domain.FixedPurchase.Helper;

namespace w2.Domain.UpdateHistory.Helper.UpdateData
{
	/// <summary>
	/// 定期購入情報更新データクラス
	/// </summary>
	[XmlRoot("w2_FixedPurchase")]
	public class UpdateDataFixedPurchase : UpdateDataBase, IUpdateData
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataFixedPurchase()
			: base ()
		{
			this.Shippings = new UpdateDataFixedPurchaseShipping[0];
			this.UserCreditCard = new UpdateDataUserCreditCard();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">表示用定期購入情報モデル</param>
		/// <param name="userCreditCard">ユーザークレジットカードモデル</param>
		public UpdateDataFixedPurchase(FixedPurchaseContainer model, UserCreditCardModel userCreditCard)
			: this()
		{
			// 定期購入情報
			this.KeyValues = model.CreateUpdateDataList();

			// 定期購入配送先情報
			this.Shippings = model.Shippings.Select(s => new UpdateDataFixedPurchaseShipping(s)).ToArray();

			// ユーザークレジットカード
			if (model.UseUserCreditCard && (userCreditCard != null))
			{
				this.UserCreditCard = new UpdateDataUserCreditCard(userCreditCard);
			}
		}
		#endregion

		#region メソッド
		/// <summary>
		///  更新データインスタンス変換（バイナリ⇒モデル）
		/// </summary>
		/// <param name="updateData">更新データ</param>
		/// <returns>更新データインスタンス</returns>
		public static UpdateDataFixedPurchase Deserialize(byte[] updateData)
		{
			return UpdateDataConverter.Deserialize<UpdateDataFixedPurchase>(updateData);
		}

		/// <summary>
		/// 更新データXML変換（モデル⇒バイナリ）
		/// </summary>
		/// <returns>シリアライズ化された更新履歴、ハッシュ文字列</returns>
		public Tuple<byte[], string> SerializeAndCreateHashString()
		{
			var result = base.SerializeAndCreateHashString(
				// 履歴に格納しないフィールド
				new string[0],
				// ハッシュに含めないフィールド
				new[] { Constants.FIELD_FIXEDPURCHASE_DATE_CHANGED, Constants.FIELD_FIXEDPURCHASE_LAST_CHANGED });
			return result;
		}
		#endregion

		#region プロパティ
		/// <summary>定期購入配送先リスト</summary>
		[XmlArray("w2_FixedPurchaseShippings")]
		[XmlArrayItem("w2_FixedPurchaseShipping")]
		public UpdateDataFixedPurchaseShipping[] Shippings { get; set; }
		/// <summary>ユーザークレジットカード</summary>
		[XmlElement("w2_UserCreditCard")]
		public UpdateDataUserCreditCard UserCreditCard { get; set; }
		#endregion

	}

	/// <summary>
	/// 定期購入情報更新データクラス（定期購入配送先）
	/// </summary>
	public class UpdateDataFixedPurchaseShipping : UpdateDataBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataFixedPurchaseShipping()
			: base()
		{
			this.Items = new UpdateDataFixedPurchaseItem[0];
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">表示用定期購入配送先情報モデル</param>
		public UpdateDataFixedPurchaseShipping(FixedPurchaseShippingContainer model)
			: this()
		{
			this.KeyValues = model.CreateUpdateDataList();

			// 定期購入商品情報
			this.Items = model.Items.Select(i => new UpdateDataFixedPurchaseItem(i)).ToArray();
		}
		#endregion

		#region プロパティ
		/// <summary>定期購入配送先リスト</summary>
		[XmlArray("w2_FixedPurchaseItems")]
		[XmlArrayItem("w2_FixedPurchaseItem")]
		public UpdateDataFixedPurchaseItem[] Items { get; set; }
		#endregion
	}

	/// <summary>
	/// 定期購入情報更新データクラス（定期購入商品）
	/// </summary>
	public class UpdateDataFixedPurchaseItem : UpdateDataBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataFixedPurchaseItem()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">表示用定期購入商品情報モデル</param>
		public UpdateDataFixedPurchaseItem(FixedPurchaseItemContainer model)
			: this()
		{
			this.KeyValues = model.CreateUpdateDataList();
		}
		#endregion

		#region プロパティ
		#endregion
	}
}