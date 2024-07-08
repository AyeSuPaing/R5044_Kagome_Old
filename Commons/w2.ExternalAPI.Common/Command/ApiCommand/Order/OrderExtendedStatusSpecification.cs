/*
=========================================================================================================
  Module      : 注文拡張ステータス条件クラス (GetOrderShippings.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Text.RegularExpressions;

namespace w2.ExternalAPI.Common.Command.ApiCommand.Order
{
    public class OrderExtendedStatusSpecification
    {
        private string m_Expression;
        private const string FIELD_ORDER_EXTENDED_STATUS_PREFIX = "extend_status";

        /// <summary>
		/// 文字列式をもとにインスタンス生成
        /// </summary>
        /// <param name="expression">文字列式</param>
        /// <returns>インスタンス</returns>
        public static OrderExtendedStatusSpecification GenByString(string expression)
        {
            return new OrderExtendedStatusSpecification {m_Expression = expression};
        }


        /// <summary>
        /// SQLのWhere節を作成
        /// </summary>
		/// <returns>SQLのWhere節</returns>
        public string CreateSqlWhereClause()
        {
            return string.IsNullOrEmpty(m_Expression)
                       ? string.Empty
                       : CreateSqlFromExpression(m_Expression);
        }

		/// <summary>
		/// 文字列に変換
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.CreateSqlWhereClause();
		}

        /// <summary>
		/// SQLのWhere節を作成
        /// </summary>
        /// <param name="expression">文字列式</param>
        /// <returns>SQLのWhere節</returns>
        private string CreateSqlFromExpression(string expression)
        {
            var digits = Regex.Split(expression, @"\D+");

            expression = digits.Where(digit => !string.IsNullOrEmpty(digit))
                .Aggregate(expression, (current, digit) => current.Replace(digit, FIELD_ORDER_EXTENDED_STATUS_PREFIX + digit));

            return " AND (" +
                   expression.Replace("&", " AND ").Replace("|", " OR ").Replace("T", " = '1'").Replace("F", " = '0'") +
                   " ) ";
        }
    }
}
