
using System;
using Distancify.Migrations.Litium;

namespace kidsconcept.Web.Migrations.Production
{
	public class Pages : DevelopmentMigration
	{
		public override void Apply()
		{ 
			WebsiteSeed.Ensure(Guid.Parse("d6864a81-9605-4bf1-bdfb-3a6d29c6e38a"), "AcceleratorWebsite")
				.WithName("en-US", "Kid's Concept")
				.WithName("sv-SE", "Kid's Concept")
				.WithName("en-GB", "Kid's Concept")
				.Commit();

			PageSeed.Ensure(Guid.Parse("857c6bbd-9ef1-42f8-8af3-d2f66dc4a84e"), "Home")
				.WithWebsite(Guid.Parse("d6864a81-9605-4bf1-bdfb-3a6d29c6e38a"))
				.WithParentPage(Guid.Parse("00000000-0000-0000-0000-000000000000"))
				.WithName("en-US", "Home page")
				.WithName("sv-SE", "Hemsida")
				.WithName("en-GB", "Homepage")
				.Commit();


		}
	}
}