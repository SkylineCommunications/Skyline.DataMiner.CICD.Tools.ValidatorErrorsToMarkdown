using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Skyline.DataMiner.CICD.Tools.ValidatorErrorsToMarkdown
{
	/// <summary>
	/// Creates an object TocItem.
	/// </summary>
	public class TocItem
	{
		private readonly string path;
		private readonly List<TocItem> childItems;

		/// <summary>
		/// Creates an instance of class <see cref="TocItem"/>.
		/// </summary>
		/// <param name="path"></param>
		public TocItem(string path)
		{
			this.path = path;
			this.childItems = new List<TocItem>();
		}

		/// <summary>
		/// Add Child TocItem. Return added item to allow LINQ behavior.
		/// </summary>
		/// <param name="child">An instance of <see cref="TocItem"/> representing a child in the table of contents.</param>
		/// <returns>An instance of <see cref="TocItem"/> representing the added child in the table of contents.</returns>
		public TocItem With(TocItem child)
		{
			childItems.Add(child);
			return child;
		}

		/// <summary>
		/// Builds a tocfile of an TocItem.
		/// </summary>
		/// <param name="tocContent"></param>
		/// <param name="level"></param>
		public void Build(StringBuilder tocContent, int level = 0)
		{
			string name = path.Split(@"\").Last();
			tocContent.Append(GetSpacesBasedOnLevel(level));
			tocContent.AppendLine($"- name: {name}");
			tocContent.Append(GetSpacesBasedOnLevel(level));
			tocContent.AppendLine($"  items:");
			if (name.StartsWith("Check") || name.StartsWith("CSharp"))
            {
				foreach (var file in Directory.GetFiles(path))
                {
					tocContent.Append(GetSpacesBasedOnLevel(level));
					tocContent.AppendLine($"  - name: {File.ReadLines(file).Skip(6).Take(1).First()[3..]}");
					tocContent.Append(GetSpacesBasedOnLevel(level));
					tocContent.AppendLine($"    topicUid: {file.Split(@"\").Last().Split(".").First()}");
				}				
            }           

			foreach (var item in childItems)
			{
				item.Build(tocContent, level += 1);
				level -= 1;
			}
		}

		private static StringBuilder GetSpacesBasedOnLevel(int level)
		{
			StringBuilder spaces = new();
			for (int i = 0; i < level; i++)
			{
				spaces.Append("  ");
			}

			return spaces;
		}
	}
}
