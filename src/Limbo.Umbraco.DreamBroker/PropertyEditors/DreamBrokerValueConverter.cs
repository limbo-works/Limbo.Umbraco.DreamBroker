using System;
using Limbo.Umbraco.DreamBroker.Models.Videos;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

#pragma warning disable 1591

namespace Limbo.Umbraco.DreamBroker.PropertyEditors {

    /// <summary>
    /// Property value converter for <see cref="DreamBrokerEditor"/>.
    /// </summary>
    public class DreamBrokerValueConverter : PropertyValueConverterBase {

        public override bool IsConverter(IPublishedPropertyType propertyType) {
            return propertyType.EditorAlias == DreamBrokerEditor.EditorAlias;
        }

        public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) {
            return source is string str && str.DetectIsJson() ? JsonUtils.ParseJsonObject(str) : null;
        }

        public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) {
            return DreamBrokerValue.Parse(inter as JObject);
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {
            return typeof(DreamBrokerValue);
        }

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
            return PropertyCacheLevel.Element;
        }

    }

}