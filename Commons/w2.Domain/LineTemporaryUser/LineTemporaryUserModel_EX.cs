/*
=========================================================================================================
  Module      : LINE仮会員テーブルモデル (LineTemporaryUserModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.LineTemporaryUser
{
	/// <summary>
	/// LINE仮会員テーブルモデル
	/// </summary>
	public partial class LineTemporaryUserModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>本登録済みユーザーか</summary>
		public bool IsRegularUser
		{
			get { return this.RegularUserRegistrationFlag == Constants.FLG_LINETEMPORARYUSER_REGISTRATION_FLAG_VALID; }
		}
		#endregion
	}
}
