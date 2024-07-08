/*
=========================================================================================================
  Module      : テスト用API引数クラス(TestApiCommandArg.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

namespace w2.ExternalAPI.Common.Command.ApiCommand.Test
{
	///**************************************************************************************
	/// <summary>
	///	テスト用API引数クラス
	/// </summary>
	/// <remarks>
	/// テスト用の引数実装
	/// </remarks>
	///**************************************************************************************
	public class TestApiCommandArg : ApiCommandArgBase
	{
		public string Para1 { get; set; }
		public string Para2 { get; set; }
		public string Para3 { get; set; }
	}
}
