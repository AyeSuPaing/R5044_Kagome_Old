using System;
using System.Linq.Expressions;
using System.Reflection;

namespace w2.CommonTests._Helper
{
	/// <summary>
	/// メンバー情報取得クラス
	/// </summary>
	public class Member
	{
		/// <summary>
		/// メンバー情報を取得します
		/// </summary>
		/// <typeparam name="T">型</typeparam>
		/// <param name="expression">式</param>
		/// <returns>メンバー情報</returns>
		public static MemberInfo Of<T>(Expression<Func<T>> expression)
		{
			return ((MemberExpression)expression.Body).Member;
		}
	}
}