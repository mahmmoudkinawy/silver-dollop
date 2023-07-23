using System;
using System.Collections.Generic;

namespace CourseLibrary.API.Services;
public class PropertyMapping<TSource, TDestination> : IPropertyMapping
{
    public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
    {
        _mappingDictionary = mappingDictionary ??
            throw new ArgumentNullException(nameof(mappingDictionary));
    }

    public Dictionary<string, PropertyMappingValue> _mappingDictionary { get; private set; }
}
