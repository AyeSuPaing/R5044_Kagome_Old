/*
=========================================================================================================
  Module      : 再与信アクション（何もしない）クラス(DoNothingAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（何もしない）クラス
	/// </summary>
	public class DoNothingAction : BaseReauthAction<DoNothingAction.ReauthActionParams>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DoNothingAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.NoAction, "何もしない", reauthActionParams)
		{
		}

		/// <summary>
		/// 空のアクションを生成
		/// </summary>
		/// <returns>再与信アクション（何もしない）</returns>
		public static DoNothingAction CreateEmpty() => new DoNothingAction(new ReauthActionParams());

		/// <summary>
		/// 何もしない
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			return new ReauthActionResult(true, string.Empty, string.Empty);
		}

		/// <summary>
		/// 再与信アクションパラメタ
		/// </summary>
		public new class ReauthActionParams : IReauthActionParams
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public ReauthActionParams()
			{
			}
		}
	}
}
