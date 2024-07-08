using System;
using System.Reflection;

namespace w2.CommonTests._Helper
{
	/// <summary>
	/// テスト設定クラス
	/// </summary>
	public class TestConfigurator : IDisposable
	{
		/// <summary>フィールド情報</summary>
		private readonly FieldInfo m_fieldInfo;
		/// <summary>フィールド値</summary>
		private readonly object m_fieldValue;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="memberInfo">メンバー情報</param>
		/// <param name="fieldValue">フィールド値</param>
		public TestConfigurator(MemberInfo memberInfo, object fieldValue)
		{
			if (memberInfo.ReflectedType == null) throw new ArgumentException("引数が無効です");

			m_fieldInfo = memberInfo.ReflectedType.GetField(memberInfo.Name);
			m_fieldValue = m_fieldInfo.GetValue(null);
			m_fieldInfo.SetValue(null, fieldValue);
		}

		/// <summary>
		/// Dispose（破棄時に元のフィールド値で復元）
		/// </summary>
		public void Dispose()
		{
			m_fieldInfo.SetValue(null, m_fieldValue);
		}
	}
}