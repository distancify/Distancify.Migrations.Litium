
using Distancify.Migrations.Litium;

namespace Eqquo.Litium.Migrations.Production.Development
{
	public class TestMigration2 : DevelopmentMigration
	{
		public override void Apply()
		{
			
			ChannelSeed.Ensure("wholesale-sweden", "DefaultChannelFieldTemplate")
				.Commit();

			ChannelSeed.Ensure("sweden", "DefaultChannelFieldTemplate")
				.Commit();

			ChannelSeed.Ensure("global", "DefaultChannelFieldTemplate")
				.Commit();

		}
	}
}