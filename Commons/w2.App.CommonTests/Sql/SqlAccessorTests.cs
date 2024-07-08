using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data;
using System.Management.Instrumentation;
using w2.Common.Sql;
using w2.CommonTests._Helper;
using w2.Domain;
using w2.Domain.User;

namespace w2.App.CommonTests.Sql
{
	[TestClass()]
	public class SqlAccessorTests : TestClassBase
	{
		/// <summary>
		/// SqlAccessorの生成テスト
		///</summary>
		[DataTestMethod()]
		public void GenerateSqlAccessorTest()
		{
			var mockConnection = new Mock<ISqlConnection>();
			var mockTransaction = new Mock<ISqlTransaction>();
			mockConnection
				.Setup(x => x.BeginTransaction())
				.Returns(mockTransaction.Object);
			mockConnection
				.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>()))
				.Returns(mockTransaction.Object);
			SqlConnectionLocator.SetConnection(mockConnection.Object);
			var userservicemock = new Mock<IUserService>();
			userservicemock
				.Setup(s => s.Get(It.IsAny<string>(), It.IsAny<SqlAccessor>()))
				.Returns(new UserModel()
				{
					UserId = "test001"
				});
			DomainFacade.Instance.UserService = userservicemock.Object;

			using (var accssor = new SqlAccessor())
			{
				accssor.OpenConnection();
				accssor.BeginTransaction();
				DomainFacade.Instance.UserService.Get("N200057442", accssor);
				accssor.CommitTransaction();
			}
			mockConnection.Verify(c =>
				c.Open(),
				Times.Once);
			mockConnection.Verify(c =>
				c.BeginTransaction(It.IsAny<IsolationLevel>()),
				Times.Exactly(2));
			mockTransaction.Verify(t =>
				t.Commit(),
				Times.Once);
		}
	}
}
