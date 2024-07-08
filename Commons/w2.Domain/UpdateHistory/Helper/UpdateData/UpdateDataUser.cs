/*
=========================================================================================================
  Module      : ユーザー情報更新データクラス (UpdateDataUser.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.UserShipping;
using w2.Domain.Point;
using w2.Domain.TwUserInvoice;

namespace w2.Domain.UpdateHistory.Helper.UpdateData
{
	/// <summary>
	/// ユーザー情報更新データクラス
	/// </summary>
	[XmlRoot("w2_User")]
	public class UpdateDataUser : UpdateDataBase, IUpdateData
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataUser()
			: base()
		{
			this.UserPoint = new UpdateDataUserPoint();
			this.UserCoupons = new UpdateDataUserCoupon[0];
			this.UserExtend = new UpdateDataUserExtend();
			this.UserCreditCards = new UpdateDataUserCreditCard[0];
			this.UserShippings = new UpdateDataUserShipping[0];
			this.UserInvoices = new UpdateDataUserInvoice[0];
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="user">ユーザー</param>
		/// <param name="userShippings">ユーザー配送先リスト</param>
		/// <param name="userPoints">ユーザーポイントリスト</param>
		/// <param name="userCoupons">ユーザークーポンリスト</param>
		/// <param name="userInvoice">User Invoice</param>
		public UpdateDataUser(
			UserModel user,
			UserShippingModel[] userShippings,
			UserPointModel[] userPoints,
			UserCouponDetailInfo[] userCoupons,
			TwUserInvoiceModel[] userInvoice)
			: this()
		{
			// ユーザー
			this.KeyValues = user.CreateUpdateDataList();

			// ユーザーポイント
			if (userPoints != null)
			{
				var keyValues = new List<UpdateDataKeyValue>();
				var userPoint = userPoints.FirstOrDefault(x => x.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP);
				if (userPoint != null)
				{
					userPoint.Point = userPoints
						.Where(x => (x.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP))
						.Sum(point => point.Point);
					// 本ポイントセット
					keyValues.AddRange(userPoint.CreateUpdateDataList());
				}
				// 仮ポイント（ポイントのみ）セット
				var pointTemp = userPoints.Where(x => x.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP).Sum(x => x.Point);
				keyValues.Add(new UpdateDataKeyValue(Constants.FIELD_USERPOINT_POINT + "_temp", pointTemp));
				this.UserPoint.KeyValues = keyValues.ToArray();
			}
			// ユーザークーポン
			if (userCoupons != null)
			{
				this.UserCoupons = userCoupons.Select(coupon => new UpdateDataUserCoupon(coupon)).ToArray();
			}

			// ユーザー拡張項目
			this.UserExtend = new UpdateDataUserExtend(user.UserExtend);

			// ユーザークレジットカード
			this.UserCreditCards = user.UserCreditCards.Select(userCreditCard => new UpdateDataUserCreditCard(userCreditCard)).ToArray();

			// ユーザー配送先
			this.UserShippings = userShippings.Select(shipping => new UpdateDataUserShipping(shipping)).ToArray();

			if (userInvoice != null)
			{
				this.UserInvoices = userInvoice.Select(invoice => new UpdateDataUserInvoice(invoice)).ToArray();
			}
		}
		#endregion

		#region メソッド
		/// <summary>
		///  更新データインスタンス変換（バイナリ⇒モデル）
		/// </summary>
		/// <param name="updateData">更新データ</param>
		/// <returns>更新データインスタンス</returns>
		public static UpdateDataUser Deserialize(byte[] updateData)
		{
			return UpdateDataConverter.Deserialize<UpdateDataUser>(updateData);
		}
		/// <summary>
		/// 更新データXML変換（モデル⇒バイナリ）
		/// </summary>
		/// <returns>シリアライズ化された更新履歴、ハッシュ文字列</returns>
		public Tuple<byte[], string> SerializeAndCreateHashString()
		{
			var result = base.SerializeAndCreateHashString(
				// 履歴に格納しないフィールド
				new[] { Constants.FIELD_USER_DATE_LAST_LOGGEDIN },
				// ハッシュに含めないフィールド
				new[]
				{
					Constants.FIELD_USER_DATE_LAST_LOGGEDIN,
					Constants.FIELD_USER_DATE_CHANGED,
					Constants.FIELD_USER_LAST_CHANGED,
					Constants.FIELD_USERCREDITCARD_REGISTER_ACTION_KBN,
					Constants.FIELD_USERCREDITCARD_REGISTER_STATUS,
					Constants.FIELD_USERCREDITCARD_REGISTER_TARGET_ID,
					Constants.FIELD_USERCREDITCARD_BEFORE_ORDER_STATUS,
				});
			return result;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザーポイント</summary>
		[XmlElement("w2_UserPoint")]
		public UpdateDataUserPoint UserPoint { get; set; }
		/// <summary>ユーザークーポンポイント</summary>
		[XmlElement("w2_UserCoupon")]
		public UpdateDataUserCoupon[] UserCoupons { get; set; }
		/// <summary>ユーザー拡張項目</summary>
		[XmlElement("w2_UserExtend")]
		public UpdateDataUserExtend UserExtend { get; set; }
		/// <summary>ユーザークレジットカードリスト</summary>
		[XmlArray("w2_UserCreditCards")]
		[XmlArrayItem("w2_UserCreditCard")]
		public UpdateDataUserCreditCard[] UserCreditCards { get; set; }
		/// <summary>ユーザー配送先リスト</summary>
		[XmlArray("w2_UserShippings")]
		[XmlArrayItem("w2_UserShipping")]
		public UpdateDataUserShipping[] UserShippings { get; set; }
		/// <summary>Tw User Invoices</summary>
		[XmlArray("w2_TwUserInvoices")]
		[XmlArrayItem("w2_TwUserInvoice")]
		public UpdateDataUserInvoice[] UserInvoices { get; set; }
		#endregion
	}

	/// <summary>
	/// ユーザー情報更新データクラス（ユーザーポイント）
	/// </summary>
	public class UpdateDataUserPoint : UpdateDataBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataUserPoint()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">ユーザーポイント情報モデル</param>
		public UpdateDataUserPoint(UserPointModel model)
			: this()
		{
			this.KeyValues = model.CreateUpdateDataList();
		}
		#endregion
	}

	/// <summary>
	/// ユーザー情報更新データクラス（ユーザークーポン）
	/// </summary>
	public class UpdateDataUserCoupon : UpdateDataBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataUserCoupon()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">ユーザーポイント情報モデル</param>
		public UpdateDataUserCoupon(UserCouponDetailInfo model)
			: this()
		{
			this.KeyValues = model.CreateUpdateDataList();
		}
		#endregion
	}

	/// <summary>
	/// ユーザー情報更新データクラス（ユーザー拡張項目）
	/// </summary>
	public class UpdateDataUserExtend : UpdateDataBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataUserExtend()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">ユーザー拡張項目情報モデル</param>
		public UpdateDataUserExtend(UserExtendModel model)
			: this()
		{
			var keyValues = new List<UpdateDataKeyValue>();
			if (model != null)
			{
				foreach (var column in model.UserExtendColumns)
				{
					keyValues.Add(new UpdateDataKeyValue(column, model.UserExtendDataText[column]));
				}
			}
			this.KeyValues = keyValues.ToArray();
		}
		#endregion
	}

	/// <summary>
	/// ユーザー情報更新データクラス（ユーザー配送先）
	/// </summary>
	public class UpdateDataUserShipping : UpdateDataBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataUserShipping()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">ユーザー配送先情報モデル</param>
		public UpdateDataUserShipping(UserShippingModel model)
			: this()
		{
			this.KeyValues = model.CreateUpdateDataList();
		}
		#endregion
	}

	/// <summary>
	/// Update Data User Invoice
	/// </summary>
	public class UpdateDataUserInvoice : UpdateDataBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateDataUserInvoice()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">ユーザー配送先情報モデル</param>
		public UpdateDataUserInvoice(TwUserInvoiceModel model)
			: this()
		{
			this.KeyValues = model.CreateUpdateDataList();
		}
		#endregion
	}
}