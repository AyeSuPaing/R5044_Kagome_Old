using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using w2.App.Common;
using w2.App.Common.Global.Config;
using w2.Common.Util;
using w2.CommonTests._Helper;

namespace w2.App.CommonTests._Helper
{
	/// <summary>
	/// アプリテストクラス基底クラス
	/// </summary>
	[TestClass()]
	[DeploymentItem(@"CustomerResources", "CustomerResources")]
	public class AppTestClassBase : TestClassBase
	{
		/// <summary>
		/// 初期化
		/// </summary>
		[AssemblyInitialize]
		public static void AppAssemblyInitialize(TestContext context)
		{
			MessageManager.MessageProvider = new MessageProviderDummy();
		}

		/// <summary>
		/// 初期化
		/// </summary>
		[TestInitialize]
		public void AppTestInitialize()
		{
			Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE = @"CustomerResources";
			Constants.GLOBAL_CONFIGS = GlobalConfigs.GetInstance();
		}

		/// <summary>
		/// クリーンアップ
		/// </summary>
		[TestCleanup]
		public void AppTestCleanup()
		{
			Constants.GLOBAL_CONFIGS = null;
		}
	}
}
