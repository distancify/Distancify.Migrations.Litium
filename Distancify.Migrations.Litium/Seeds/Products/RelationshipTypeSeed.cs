using System;
using System.Linq;
using Litium;
using Litium.Products;


namespace Distancify.Migrations.Litium.Seeds.Products
{
    public class RelationshipTypeSeed : ISeed
    {
        private readonly RelationshipType _relationshipType;

        protected RelationshipTypeSeed(RelationshipType relationshipType)
        {
            _relationshipType = relationshipType;
        }

        public void Commit()
        {
            var service = IoC.Resolve<RelationshipTypeService>();

            if (_relationshipType.SystemId == Guid.Empty)
            {
                _relationshipType.SystemId = Guid.NewGuid();
                service.Create(_relationshipType);
                return;
            }

            service.Update(_relationshipType);
        }

        public static RelationshipTypeSeed Ensure(string relationshipType)
        {
            var relationshipTypeClone = IoC.Resolve<RelationshipTypeService>().Get(relationshipType)?.MakeWritableClone() ??
                new RelationshipType()
                {
                    Id = relationshipType,
                    SystemId = Guid.Empty
                };

            return new RelationshipTypeSeed(relationshipTypeClone);
        }

        public RelationshipTypeSeed WithName(string culture, string name)
        {
            if (!_relationshipType.Localizations.Any(l => l.Key.Equals(culture)) ||
                !_relationshipType.Localizations[culture].Name.Equals(name))
            {
                _relationshipType.Localizations[culture].Name = name;
            }

            return this;
        }

        public RelationshipTypeSeed WithBidirectional(bool bidirectional)
        {
            _relationshipType.Bidirectional = bidirectional;

            return this;
        }
    }
}
