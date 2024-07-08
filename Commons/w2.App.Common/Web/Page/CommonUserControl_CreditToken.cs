/*
=========================================================================================================
  Module      : コモンユーザーコントロール(CommonUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.UI.WebControls;
using w2.App.Common.Order.UserCreditCardCooperationInfos;
using w2.App.Common.Web.WrappedContols;

namespace w2.App.Common.Web.Page
{
	/// <summary>
	/// コモンユーザーコントロール
	/// </summary>
	public partial class CommonUserControl
	{
		/// <summary>
		/// カスタムバリデータdisabled処理（トークン取得用・マスキングした後にポストバックできるようにするため）
		/// </summary>
		/// <param name="wrCartList">カートリストラップ済みリピータ</param>
		protected void DisableCreditInputCustomValidatorForGetCreditToken(WrappedRepeater wrCartList)
		{
			this.Process.DisableCreditInputCustomValidatorForGetCreditToken(wrCartList);
		}
		/// <summary>
		/// クレジットトークン向け カスタムバリデータdisabled処理（マスキングした後にポストバックできるようにするため）
		/// </summary>
		/// <param name="riParent">親カートリピータアイテム（カート外であればnullを指定）</param>
		protected void DisableCreditInputCustomValidatorForGetCreditToken(RepeaterItem riParent = null)
		{
			this.Process.DisableCreditInputCustomValidatorForGetCreditToken(riParent);
		}

		/// <summary>
		/// クレジットトークン向け  カード情報取得JSスクリプト作成（カート内ではない）
		/// </summary>
		/// <returns>
		/// カード情報取得スクリプト
		/// 文字列を返すのでevalで動的実行すれば連想配列でカード情報がとれます
		/// </returns>
		protected virtual string CreateGetCardInfoJsScriptForCreditTokenInner()
		{
			return this.Process.CreateGetCardInfoJsScriptForCreditTokenInner();
		}
		/// <summary>
		/// クレジットトークン向け カード情報取得JSスクリプト作成（カート内ではない）
		/// </summary>
		/// <param name="riCart">カートリピータがある場合のリピータアイテム（カート1と同じ決済利用時のトークン取得用）</param>
		/// <param name="riParent">親カートリピータアイテム（カート外であればnullを指定）</param>
		/// <param name="userCreditCardCooperationInfo">ユーザークレジットカード連携情報</param>
		/// <returns>
		/// カード情報取得スクリプト
		/// 文字列を返すのでevalで動的実行すれば連想配列でカード情報がとれます
		/// </returns>
		protected virtual string CreateGetCardInfoJsScriptForCreditTokenInner(
			UserCreditCardCooperationInfoBase userCreditCardCooperationInfo,
			RepeaterItem riCart = null,
			RepeaterItem riParent = null)
		{
			return this.Process.CreateGetCardInfoJsScriptForCreditTokenInner(
				userCreditCardCooperationInfo,
				riCart,
				riParent);
		}

		/// <summary>
		/// クレジットトークン取得＆フォームセットJSスクリプト作成 内部メソッド
		/// </summary>
		/// <param name="riParent">親リピーターアイテム</param>
		/// <returns>JSスクリプト</returns>
		protected string CreateGetCreditTokenAndSetToFormJsScriptInner(RepeaterItem riParent = null)
		{
			return this.Process.CreateGetCreditTokenAndSetToFormJsScriptInner(riParent);
		}
		/// <summary>
		/// クレジットトークン取得＆フォームセットJSスクリプト作成 内部メソッド
		/// </summary>
		/// <param name="wrCartList">カートリストリピーター</param>
		/// <returns>JSスクリプト</returns>
		protected string CreateGetCreditTokenAndSetToFormJsScriptInner(WrappedRepeater wrCartList)
		{
			return this.Process.CreateGetCreditTokenAndSetToFormJsScriptInner(wrCartList);
		}

		/// <summary>
		/// クレジットトークン向け フォームマスキングJSスクリプト作成 内部メソッド
		/// </summary>
		/// <param name="riParent">親リピーターアイテム</param>
		/// <returns>JSスクリプト</returns>
		protected virtual string CreateMaskFormsForCreditTokenJsScriptInner(RepeaterItem riParent = null)
		{
			return this.Process.CreateMaskFormsForCreditTokenJsScriptInner(riParent);
		}
		/// <summary>
		/// クレジットトークン向け フォームマスキングJSスクリプト作成 内部メソッド
		/// </summary>
		/// <param name="wrCartList">カートリストリピーター</param>
		/// <returns>JSスクリプト</returns>
		protected virtual string CreateMaskFormsForCreditTokenJsScriptInner(WrappedRepeater wrCartList)
		{
			return this.Process.CreateMaskFormsForCreditTokenJsScriptInner(wrCartList);
		}

		/// <summary>
		/// トークンが入力されていたら入力画面を切り替える
		/// </summary>
		/// <param name="wrCartList">カートリストラップ済みリピータ</param>
		protected void SwitchDisplayForCreditTokenInput(WrappedRepeater wrCartList)
		{
			this.Process.SwitchDisplayForCreditTokenInput(wrCartList);
		}
		/// <summary>
		/// クレジットトークンが入力されていたら入力画面を切り替える
		/// </summary>
		/// <param name="riParent">親カートリピータアイテム（カート外であればnullを指定）</param>
		protected void SwitchDisplayForCreditTokenInput(RepeaterItem riParent = null)
		{
			this.Process.SwitchDisplayForCreditTokenInput(riParent);
		}

		/// <summary>
		/// 入力フォームからクレジットトークン情報削除（再入力ボタンで利用）
		/// </summary>
		/// <param name="riParent">親カートリピータアイテム（カート外であればnullを指定）</param>
		/// <param name="inputErrorCssClass">フォーム入力エラーCSSクラス</param>
		/// <param name="restCustomValidate">カスタムばりデータリセットするか</param>
		protected void ResetCreditTokenInfoFromForm(RepeaterItem riParent, string inputErrorCssClass, bool restCustomValidate = true)
		{
			this.Process.ResetCreditTokenInfoFromForm(riParent, inputErrorCssClass, restCustomValidate);
		}

		/// <summary>
		/// クレジットトークンを持っているか（表示切り替えに利用）
		/// </summary>
		/// <param name="riParent">親カートリピータアイテム（カート外であればnullを指定）</param>
		/// <returns>トークンを持っているか</returns>
		protected virtual bool HasCreditToken(RepeaterItem riParent = null)
		{
			return this.Process.HasCreditToken(riParent);
		}
	}
}
