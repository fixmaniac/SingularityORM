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
using System.Text;
using System.Threading.Tasks;

namespace Singularity.ORM.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class TextMaxLengthAttribute : BusinessValidationAttribute
    {
        public readonly int MaxLength;

        public TextMaxLengthAttribute(int maxLength)
        {
            this.MaxLength = maxLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value.GetType() != typeof(string))
                return ValidationResult.Success;
            base.ErrorMessage = "Zbyt długi ciąg tekstowy.";
            return ((value as string).Length > this.MaxLength)
                     ? new ValidationResult(ErrorMessage)
                     : ValidationResult.Success;

        }
    }
}
