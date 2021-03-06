﻿namespace Distancify.Migrations.Litium.SeedBuilder
{
    public class MigrationConfiguration
    {

        /// <summary>
        /// Hostname including protocol for GraphQL server
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Graphql query used to pull data
        /// </summary>
        /// <example>
        /// <![CDATA[
        ///      query{
        ///         channels{
        ///           id
        ///         }
        ///     }
        /// ]]>
        /// </example>
        public string Query { get; set; }

        /// <summary>
        /// Namespace of the migration
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Class name of the migration
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Class the migration should build on
        /// </summary>
        public string BaseMigration { get; set; }

        /// <summary>
        /// Path to where the migration code should be written
        /// </summary>
        public string Output { get; set; }


    }
}
