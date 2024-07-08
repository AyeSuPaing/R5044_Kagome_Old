using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace w2.Domain.Tests
{
	[TestClass()]
	public class RepositoryBaseTests
	{
		/// <summary>
		/// リポジトリのリソースファイルが含まれているかチェック
		/// </summary>
		[DataTestMethod()]
		public void CheckResourceTest()
		{
			// リポジトリの名称をすべて取得
			var asm = Assembly.LoadFrom("w2.Domain.dll");
			var types = asm.GetTypes();
			var repositoryNames = types
				.Where(t => t.Name.EndsWith("Repository"))
				.Select(t => t.Name.Substring(0, t.Name.Length - 10));

			// リソース内にXMLが存在するかチェック
			foreach (var repositoryName in repositoryNames)
			{
				var xmlString = CheckResourceFile(repositoryName);

				// _subが存在するかチェック（XMLコメントは除外しないといけない）
				var xmlElements = XDocument.Parse(xmlString).Root.Elements();
				if (xmlElements.Any(e => e.Value.Contains("[[ ")))
				{
					CheckResourceFile(repositoryName + "_Sub");
				}
			}
		}

		/// <summary>
		/// リソースファイルの存在を確認
		/// </summary>
		/// <param name="resourceName">リソース名</param>
		/// <returns>リソースの内容</returns>
		private string CheckResourceFile(string resourceName)
		{
			var resource = w2.Domain.Properties.Resources.ResourceManager.GetString(resourceName);
			resource.Should().NotBeNull("リポジトリXMLが存在しませんでした：" + resourceName);
			return resource;
		}
	}
}
