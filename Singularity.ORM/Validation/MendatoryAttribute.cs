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
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;


namespace Singularity.ORM.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MendatoryAttribute : BusinessValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            base.ErrorMessage = "Wartość pola jest wymagana.";
            if (value != null
                && value.GetType().IsEnum
                && value.ToString().Equals("empty"))
                return new ValidationResult(ErrorMessage);
            else if (value != null
                && value.GetType() == typeof(String)
                && ((String)value) == String.Empty)
                return new ValidationResult(ErrorMessage);
            else if (value != null
                && value.GetType().IsEnum
                && value.ToString().Equals("0"))
                return new ValidationResult(ErrorMessage);
            else if (value == null)
            {
                return new ValidationResult(ErrorMessage);
            }
            else
                return ValidationResult.Success;
        }
    }
}
