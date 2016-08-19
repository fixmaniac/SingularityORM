/*
 *  Copyright (c) 2016, Łukasz Ligocki.
 *  All rights reserved.
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at

 *  http://www.apache.org/licenses/LICENSE-2.0

 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */

using System;
using System.Globalization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Singularity.ORM.Enum;

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
            if (!typeof(EntityProvider).IsAssignableFrom(o.GetType())
                || (FieldState)((EntityProvider)o)["State"] == FieldState.Deleted)
                yield break;

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
