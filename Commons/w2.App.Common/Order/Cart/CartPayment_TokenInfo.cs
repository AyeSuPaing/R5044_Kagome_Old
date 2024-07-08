/*
=========================================================================================================
  Module      : カート決済情報クラス（トークン情報）(CartPayment_TokenInfo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Wrapper;

namespace w2.App.Common.Order
{
	/// <summary>
	/// カート決済情報クラス
	/// </summary>
	public partial class CartPayment
	{
		/// <summary>
		/// クレジットトークン情報
		/// </summary>
		[Serializable]
		public abstract class CreditTokenInfoBase
		{
			/// <summary>
			/// クレジットトークン情報作成
			/// </summary>
			/// <param name="value">hidden文字列</param>
			/// <returns>トークン情報</returns>
			public static CreditTokenInfoBase CreateCreditTokenInfo(string value)
			{
				if (string.IsNullOrEmpty(value)) return null;

				switch (Constants.PAYMENT_CARD_KBN)
				{
					case Constants.PaymentCard.Gmo:
						var tokenInfoGmo = new CreditTokenInfoGmo(value);
						return tokenInfoGmo;

					case Constants.PaymentCard.SBPS:
						var tokenInfoSbps = new CreditTokenInfoSbps(value);
						return tokenInfoSbps;

					case Constants.PaymentCard.Zeus:
						var tokenInfoZeus = new CreditTokenInfoZeus(value);
						return tokenInfoZeus;

					case Constants.PaymentCard.YamatoKwc:
						var tokenInfoYamatoKwc = new CreditTokenInfoYamatoKwc(value);
						return tokenInfoYamatoKwc;

					case Constants.PaymentCard.EScott:
						var tokenInfoEScott = new CreditTokenInfoEScott(value);
						return tokenInfoEScott;

					case Constants.PaymentCard.VeriTrans:
						var tokenInfoVeriTrans = new CreditTokenInfoVeriTrans(value);
						return tokenInfoVeriTrans;

					case Constants.PaymentCard.Rakuten:
						var tokenInfoRakuten = new CreditTokenInfoRakuten(value);
						return tokenInfoRakuten;

					case Constants.PaymentCard.Paygent:
						var tokenInfoPaygent = new CreditTokenInfoPaygent(value);
						return tokenInfoPaygent;
				}
				throw new NotImplementedException("トークン作成処理が組み込まれていません：" + Constants.PAYMENT_CARD_KBN);
			}

			/// <summary>
			/// トークンを有効期限切れにする
			/// </summary>
			/// <returns>トークンhidden値</returns>
			public void SetTokneExpired()
			{
				this.ExpireDate = DateTime.Now.AddYears(-1);
			}

			/// <summary>
			/// 文字列表現へ変換
			/// </summary>
			/// <returns>トークンhidden値</returns>
			public override string ToString()
			{
				return CreateTokenHiddenValue();
			}

			/// <summary>
			/// トークンhidden値作成
			/// </summary>
			/// <returns>トークンhiddenセット用value</returns>
			public abstract string CreateTokenHiddenValue();
			/// <summary>トークン文字列（aspxのhiddenには直接セットしないでください）</summary>
			public string Token { get; protected set; }
			/// <summary>トークンの有効期限（決済時にトークン切れエラーが返却された場合は過去の日付をセット）</summary>
			public virtual DateTime? ExpireDate { get; set; }
			/// <summary>トークンが有効期限切れか</summary>
			public bool IsExpired
			{
				get { return this.ExpireDate.HasValue && (this.ExpireDate.Value < DateTimeWrapper.Instance.Now); }
			}
			/// <summary>カード連携ID1</summary>
			public string CooperationId1 { get; set; }
			/// <summary>カード連携ID2</summary>
			public string CooperationId2 { get; set; }
		}

		/// <summary>
		/// GMOクレジットトークン情報
		/// </summary>
		[Serializable]
		public sealed class CreditTokenInfoGmo : CreditTokenInfoBase
		{
			/// <summary>GMOトークン有効期限日付フォーマット</summary>
			private const string TOKEN_EXPIRE_DATE_FORMAT_GMO = "yyyy-MM-dd-HH-mm-ss";

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="value">hidden文字列</param>
			internal CreditTokenInfoGmo(string value)
			{
				var splitted = (value + " ").Split(' ');
				this.Token = splitted[0];
				DateTime expired;
				this.ExpireDate = DateTime.TryParseExact(
					splitted[1],
					new[] { TOKEN_EXPIRE_DATE_FORMAT_GMO },
					DateTimeFormatInfo.InvariantInfo,
					DateTimeStyles.None,
					out expired)
					? (DateTime?)expired
					: null;
				this.CooperationId1 = "";
				this.CooperationId2 = "";
			}

			/// <summary>
			/// トークンhidden値作成
			/// </summary>
			/// <returns>トークンhiddenセット用value</returns>
			public override string CreateTokenHiddenValue()
			{
				var value = string.Join(
					" ",
					this.Token,
					this.ExpireDate.HasValue ? this.ExpireDate.Value.ToString(TOKEN_EXPIRE_DATE_FORMAT_GMO) : "");
				return value;
			}
		}

		/// <summary>
		/// SBPSクレジットトークン情報
		/// </summary>
		[Serializable]
		public class CreditTokenInfoSbps : CreditTokenInfoBase
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="value">hidden文字列</param>
			internal CreditTokenInfoSbps(string value)
			{
				var splitted = (value + " ").Split(' ');
				this.Token = splitted[0];
				this.TokenKey = splitted[1];
				this.CooperationId1 = "";
				this.CooperationId2 = "";
			}

			/// <summary>
			/// トークンhidden値作成
			/// </summary>
			/// <returns>トークンhiddenセット用value</returns>
			public override string CreateTokenHiddenValue()
			{
				var value = string.Join(" ", this.Token, this.TokenKey);
				return value;
			}

			/// <summary>トークンキー文字列</summary>
			public string TokenKey { get; protected set; }
		}

		/// <summary>
		/// ZEUSクレジットトークン情報
		/// </summary>
		[Serializable]
		public sealed class CreditTokenInfoZeus : CreditTokenInfoBase
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="value">hidden文字列</param>
			internal CreditTokenInfoZeus(string value)
			{
				this.Token = value;
				this.CooperationId1 = "";
				this.CooperationId2 = "";
				this.ExpireDate = DateTime.Now.AddMinutes(55);	// 60分しかもたないので55分で切れることにする
			}

			/// <summary>
			/// トークンhidden値作成
			/// </summary>
			/// <returns>トークンhiddenセット用value</returns>
			public override string CreateTokenHiddenValue()
			{
				var value = string.Join(" ", this.Token);
				return value;
			}
		}

		/// <summary>
		/// ヤマトKWCクレジットトークン情報
		/// </summary>
		[Serializable]
		public sealed class CreditTokenInfoYamatoKwc : CreditTokenInfoBase
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="value">hidden文字列</param>
			internal CreditTokenInfoYamatoKwc(string value)
			{
				var splitted = (value + "  ").Split(' ');

				this.CooperationId1 = splitted[0];
				this.CooperationId2 = splitted[1];
				this.Token = splitted[2];
			}

			/// <summary>
			/// トークンhidden値作成
			/// </summary>
			/// <returns>トークンhiddenセット用value</returns>
			public override string CreateTokenHiddenValue()
			{
				var value = string.Join(" ", this.CooperationId1, this.CooperationId2, this.Token);
				return value;
			}
		}

		/// <summary>
		/// e-SCOTTクレジットトークン情報
		/// </summary>
		[Serializable]
		public class CreditTokenInfoEScott : CreditTokenInfoBase
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="value">hidden文字列</param>
			internal CreditTokenInfoEScott(string value)
			{
				this.Token = value;
				this.CooperationId1 = string.Empty;
				this.CooperationId2 = string.Empty;
			}

			/// <summary>
			/// トークンhidden値作成
			/// </summary>
			/// <returns>トークンhiddenセット用value</returns>
			public override string CreateTokenHiddenValue()
			{
				return this.Token;
			}
		}

		/// <summary>
		/// ベリトランストークン情報
		/// </summary>
		[Serializable]
		public sealed class CreditTokenInfoVeriTrans : CreditTokenInfoBase
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="value">hidden文字列</param>
			internal CreditTokenInfoVeriTrans(string value)
			{
				var spitted = (value + "  ").Split(' ');
				this.Token = spitted[0];
				this.CooperationId1 = spitted[1];
				this.CooperationId2 = spitted[2];
			}

			/// <summary>
			/// トークンhidden値作成
			/// </summary>
			/// <returns>トークンhiddenセット用value</returns>
			public override string CreateTokenHiddenValue()
			{
				var value = string.Join(" ", this.Token, this.CooperationId1, this.CooperationId2);
				return value;
			}
		}

		/// <summary>
		/// 楽天クレカ情報
		/// </summary>
		[Serializable]
		public class CreditTokenInfoRakuten : CreditTokenInfoBase
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="value">hidden文字列</param>
			internal CreditTokenInfoRakuten(string value)
			{
				var splitted = (value + " ").Split(' ');
				this.Token = splitted[0];
				this.TokenKey = splitted[1];
				this.CooperationId1 = splitted[0];
				this.CooperationId2 = string.Empty;
			}

			/// <summary>
			/// トークンhidden値作成
			/// </summary>
			/// <returns>トークンhiddenセット用value</returns>
			public override string CreateTokenHiddenValue()
			{
				var value = string.Join(" ", this.Token, this.TokenKey);
				return value;
			}

			/// <summary>トークンキー文字列</summary>
			public string TokenKey { get; protected set; }
		}

		/// <summary>
		/// ペイジェントクレカ情報
		/// </summary>
		[Serializable]
		public class CreditTokenInfoPaygent : CreditTokenInfoBase
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="value">hidden文字列</param>
			internal CreditTokenInfoPaygent(string value)
			{
				var splitted = (value + " ").Split(' ');
				this.Token = splitted[0];
				this.TokenKey = splitted[1];
				this.CooperationId1 = splitted[0];
				this.CooperationId2 = string.Empty;
			}

			/// <summary>
			/// トークンhidden値作成
			/// </summary>
			/// <returns>トークンhiddenセット用value</returns>
			public override string CreateTokenHiddenValue()
			{
				var value = string.Join(" ", this.Token, this.TokenKey);
				return value;
			}

			/// <summary>トークンキー文字列</summary>
			public string TokenKey { get; protected set; }
		}
	}
}
