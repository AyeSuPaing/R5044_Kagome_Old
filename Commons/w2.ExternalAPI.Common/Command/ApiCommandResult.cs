/*
=========================================================================================================
  Module      : APIコマンド実行結果クラス(ApiCommandResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

namespace w2.ExternalAPI.Common.Command
{
	/// <summary>
	///	APIコマンド実行結果クラス
	/// </summary>
	/// <remarks>
	/// APIコマンドの実行結果を持つクラス
	/// </remarks>
	public class ApiCommandResult
	{
		#region メンバ変数
		private EnumResultStatus m_enumResultStatus;
		#endregion

		#region コンストラクタ
		/// <summary>コンストラクタ</summary>
		/// <param name="enumResultStatus">実行結果列挙体</param>
		public ApiCommandResult(EnumResultStatus enumResultStatus)
		{
			m_enumResultStatus = enumResultStatus;
		}
		#endregion
		
		#region プロパティ
		public EnumResultStatus ResultStatus
		{	
			get { return m_enumResultStatus; }
			set { m_enumResultStatus = value; }
		}
		#endregion
	}
}
