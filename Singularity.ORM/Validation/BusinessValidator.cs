using System;
using System.Globalization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Singularity.ORM.Validation
{
    public static class BusinessValidator
    {
        internal static PropertyDescriptor GetPropertyDescriptor(Type recordType, string name)
        {
            PropertyInfo PropertyInfo = recordType.GetProperty
                      (name, BindingFlags.Instance | BindingFlags.Public);
            return TypeDescriptor.GetProperties(PropertyInfo.DeclaringType)[PropertyInfo.Name];
        }

        public static bool IsMendatory(PropertyDescriptor descriptor)
        {
            return (descriptor.Attributes.Cast<Attribute>().Any
                (a => a is MendatoryAttribute));
        }


        public static IEnumerable<string> Validate(object o)
        {
            Type type = o.GetType();
            PropertyInfo[] properties = o.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(BusinessValidationAttribute),
                                             inherit: true);
                foreach (var customAttribute in customAttributes)
                {
                    var validationAttribute = (BusinessValidationAttribute)customAttribute;
                    object value = propertyInfo.GetValue
                          (o, BindingFlags.GetProperty, null, null, CultureInfo.InvariantCulture);
                    bool isValid = validationAttribute.IsValid(value);
                    if (!isValid)
                    {
                        yield return String.Format(
                            String.Concat(validationAttribute.ErrorMessage,
                            "Nazwa parametru: {0}"),
                            propertyInfo.Name);
                    }
                }
            }
        }
    }
}
