﻿using Distancify.Migrations.Litium.SeedBuilder.LitiumGraphQlModel;
using Distancify.Migrations.Litium.Seeds;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distancify.Migrations.Litium.SeedBuilder.Respositories
{
    public abstract class Repository<T, TSeedGenerator>
        where T : GraphQlObject
        where TSeedGenerator : ISeedGenerator<T>
    {
        protected readonly IDictionary<string, TSeedGenerator> Items = new Dictionary<string, TSeedGenerator>();

        public void AddOrMerge(T graphQlItem)
        {
            if (Items.TryGetValue(graphQlItem.Id, out var existing))
            {
                existing.Update(graphQlItem);
            }
            else
            {
                var seed = CreateFrom(graphQlItem);
                Items.Add(graphQlItem.Id, seed);
            }
        }

        

        protected abstract TSeedGenerator CreateFrom(T graphQlItem);


        public int NumberOfItems
        {
            get
            {
                return Items.Count();
            }
        }

        public void WriteMigration(StringBuilder builder)
        {
            foreach(var i in Items.Values)
            {
                i.WriteMigration(builder);
            }
        }
    }
}
