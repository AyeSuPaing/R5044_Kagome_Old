using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace w2.CommonTests._Helper
{
	/// <summary>
	/// <see cref="PrivateObject"/> 拡張クラス
	/// </summary>
	public static class PrivateObjectExtensions
	{
		/// <summary>
		/// プロパティを取得
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">オブジェクト</param>
		/// <param name="propertyName">プロパティ名</param>
		/// <returns>プロパティ</returns>
		public static T GetProperty<T>(this PrivateObject obj, string propertyName)
		{
			return (T)obj.GetProperty(propertyName);
		}
	}
}
