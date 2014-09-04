﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm;
using Microsoft.Data.OData;
using Simple.OData.Client.Extensions;

namespace Simple.OData.Client
{
    class RequestWriterV3 : IRequestWriter
    {
        private readonly ISession _session;
        private readonly string _urlBase;
        private readonly IEdmModel _model;

        public RequestWriterV3(ISession session, string urlBase, IEdmModel model)
        {
            _session = session;
            _urlBase = urlBase;
            _model = model;
        }

        public string CreateEntry(string entityTypeNamespace, string entityTypeName,
            IDictionary<string, object> properties,
            IEnumerable<KeyValuePair<string, object>> associationsByValue,
            IEnumerable<KeyValuePair<string, int>> associationsByContentId)
        {
            // TODO: check dispose
            var writerSettings = new ODataMessageWriterSettings() { BaseUri = new Uri(_urlBase) };
            var message = new ODataV3RequestMessage(null, null);
            var messageWriter = new ODataMessageWriter(message, writerSettings, _model);
            var entryWriter = messageWriter.CreateODataEntryWriter();
            var entry = new Microsoft.Data.OData.ODataEntry();
            entry.TypeName = string.Join(".", entityTypeNamespace, entityTypeName);

            var typeProperties = (_model.FindDeclaredType(entry.TypeName) as IEdmEntityType).Properties();
            entry.Properties = properties.Select(x => new ODataProperty()
            {
                Name = typeProperties.Single(y => Utils.NamesAreEqual(y.Name, x.Key, _session.Pluralizer)).Name,
                Value = GetPropertyValue(typeProperties, x.Key, x.Value)
            }).ToList();

            entryWriter.WriteStart(entry);

            if (associationsByValue != null)
            {
                foreach (var association in associationsByValue)
                {
                    if (association.Value != null)
                        WriteLink(entryWriter, entry, association);
                }
            }

            entryWriter.WriteEnd();

            return Utils.StreamToString(message.GetStream());
        }

        private void WriteLink(ODataWriter entryWriter, Microsoft.Data.OData.ODataEntry entry, KeyValuePair<string, object> linkData)
        {
            var navigationProperty = (_model.FindDeclaredType(entry.TypeName) as IEdmEntityType).NavigationProperties()
                .Single(x => Utils.NamesAreEqual(x.Name, linkData.Key, _session.Pluralizer));
            bool isCollection = navigationProperty.Partner.Multiplicity() == EdmMultiplicity.Many;

            IEdmEntityType linkType;
            if (navigationProperty.Type.Definition.TypeKind == EdmTypeKind.Collection)
                linkType = (navigationProperty.Type.Definition as IEdmCollectionType).ElementType.Definition as IEdmEntityType;
            else
                linkType = navigationProperty.Type.Definition as IEdmEntityType;

            entryWriter.WriteStart(new ODataNavigationLink()
            {
                Name = linkData.Key,
                IsCollection = isCollection,
                Url = new Uri("http://schemas.microsoft.com/ado/2007/08/dataservices/related/" + linkType, UriKind.Absolute),
            });

            var linkKey = linkType.DeclaredKey;
            var linkEntry = linkData.Value.ToDictionary();
            var formattedKey = "(" + string.Join(".", linkKey.Select(x => new ValueFormatter().FormatContentValue(linkEntry[x.Name]))) + ")";
            var linkSet = _model.EntityContainers().SelectMany(x => x.EntitySets())
                .Single(x => Utils.NamesAreEqual(x.ElementType.Name, linkType.Name, _session.Pluralizer));
            var link = new ODataEntityReferenceLink { Url = new Uri(linkSet.Name + formattedKey, UriKind.Relative) };
            entryWriter.WriteEntityReferenceLink(link);

            entryWriter.WriteEnd();
        }

        private object GetPropertyValue(IEnumerable<IEdmProperty> properties, string key, object value)
        {
            if (value == null)
                return value;

            var property = properties.Single(x => Utils.NamesAreEqual(x.Name, key, _session.Pluralizer));
            switch (property.Type.TypeKind())
            {
                case EdmTypeKind.Complex:
                    value = new ODataComplexValue()
                    {
                        TypeName = property.Type.FullName(),
                        Properties = (value as IDictionary<string, object>).Select(x => new ODataProperty()
                        {
                            Name = x.Key,
                            Value = GetPropertyValue(property.Type.AsComplex().StructuralProperties(), x.Key, x.Value),
                        }),
                    };
                    break;

                case EdmTypeKind.Collection:
                    value = new ODataCollectionValue()
                    {
                        TypeName = property.Type.FullName(),
                        Items = (value as IEnumerable<object>).Select(x => GetPropertyValue(
                            property.Type.AsCollection().AsStructured().StructuralProperties(), property.Name, x)),
                    };
                    break;

                case EdmTypeKind.Primitive:
                default:
                    return value;
            }
            return value;
        }
    }
}