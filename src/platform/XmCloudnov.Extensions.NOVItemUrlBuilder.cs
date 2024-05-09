// XmCloudnov.Extensions.NOVItemUrlBuilder
using System;
using Sitecore.Data.Items;
using Sitecore.Links.UrlBuilders;

namespace XmCloudSXAStarter
{
	public class NOVItemUrlBuilder : ItemUrlBuilder
	{
		public NOVItemUrlBuilder(DefaultItemUrlBuilderOptions defaultOptions)
			: base(defaultOptions)
		{
		}

		public override string Build(Item item, ItemUrlBuilderOptions options)
		{
			string text = base.Build(item, options);
			if (text != null && text.ToLower().Contains("/products/"))
			{
				int num = text.LastIndexOf('/') + 1;
				string text2 = text.Substring(num, text.Length - num);
				text = text.ToLower().Split(new string[1] { "/products/" }, StringSplitOptions.None)[0] + "/products/" + text2?.ToLower();
			}
			return text?.ToLower();
		}
	}
}