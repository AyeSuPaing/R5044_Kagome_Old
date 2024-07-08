using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.User;
using w2.Domain.User.Helper;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// ユーザー拡張設定データキャッシュ設定クラス
	/// </summary>
	internal class UserExtendSettingDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal UserExtendSettingDataCacheConfigurator(UserExtendSettingList data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// ユーザー拡張設定データキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">ユーザー拡張設定データ</param>
		private void RefreshDataCache(UserExtendSettingList data)
		{
			var userServiceMock = new Mock<IUserService>();
			//FIXME:ビルドエラー回避
			//userServiceMock.Setup(s => s.GetUserExtendSettingList()).Returns(data);
			Domain.DomainFacade.Instance.UserService = userServiceMock.Object;

			DataCacheControllerFacade.GetUserExtendSettingCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new UserExtendSettingList());
		}
	}
}
