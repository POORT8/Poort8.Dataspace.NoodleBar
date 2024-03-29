﻿using System.ComponentModel.DataAnnotations;

namespace Poort8.Dataspace.AuthorizationRegistry.Entities;
public class Feature //https://schema.org/Service
{
    [Key]
    public string FeatureId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<FeatureProperty> Properties { get; set; } = new List<FeatureProperty>();

    public Feature(string featureId, string name, string description)
    {
        FeatureId = featureId;
        Name = name;
        Description = description;
    }

    public Feature(string featureId, string name, string description, ICollection<FeatureProperty> properties)
    {
        FeatureId = featureId;
        Name = name;
        Description = description;
        Properties = properties;
    }

    public class FeatureProperty
    {
        [Key]
        public string PropertyId { get; init; } = Guid.NewGuid().ToString();
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsIdentifier { get; set; } = false;

        public FeatureProperty(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public FeatureProperty(string key, string value, bool isIdentifier)
        {
            Key = key;
            Value = value;
            IsIdentifier = isIdentifier;
        }
    }
}