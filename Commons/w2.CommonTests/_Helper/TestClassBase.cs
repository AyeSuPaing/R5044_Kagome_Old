using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace w2.CommonTests._Helper
{
	/// <summary>
	/// テストクラス基底クラス
	/// </summary>
	public class TestClassBase
	{
		/// <summary>現在のテストの実行についての情報および機能を提供するテスト コンテキストを取得または設定します</summary>
		public TestContext TestContext { get; set; }
	}
}