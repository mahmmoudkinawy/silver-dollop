using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseLibrary.API.Services;
public class PropertyMappingService
{
    private Dictionary<string, PropertyMappingValue> _authorsPropertyMappings
        = new(StringComparer.OrdinalIgnoreCase)
        {
            {"Id", new PropertyMappingValue(new List<string>(){"Id"}) },
            {"Name", new PropertyMappingValue(new List<string>(){"FirstName", "LastName"}, true) },
            {"Age", new PropertyMappingValue(new List<string>(){"DateOfBirth"}) },
            {"MainCategory", new PropertyMappingValue(new List<string>(){"MainCategory"}) }
        };

    private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

    public PropertyMappingService()
    {
        _propertyMappings.Add(new PropertyMapping<AuthorDto, Author>(_authorsPropertyMappings));
    }

    public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
    {
        var matchingProperty = _propertyMappings
            .OfType<PropertyMapping<TSource, TDestination>>();

        if (matchingProperty.Count() == 1)
        {
            return matchingProperty.First()._mappingDictionary;
        }

        throw new Exception($"Cannot find exact property mapping instance " +
            $"for <{typeof(TSource)},{typeof(TDestination)}>");
    }
}
