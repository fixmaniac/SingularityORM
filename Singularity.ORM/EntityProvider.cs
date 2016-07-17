﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Singularity.ORM.Enum;

namespace Singularity.ORM
{
    /// <summary>
    /// 
    /// </summary>
    [TypeDescriptionProvider(typeof(BusinessTypeDescriptionProvider))]
    public abstract class EntityProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public EntityProvider()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<String, Object> customFieldValues =
                               new Dictionary<String, Object>();

        public Object this[String fieldName]
        {
            get
            {
                Object value = null;
                customFieldValues.TryGetValue(fieldName, out value);
                return value;
            }

            set
            {
                customFieldValues[fieldName] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public virtual ISqlTransaction Transaction
        {
            get
            {
                return (ISqlTransaction)this["CurrentTransaction"];
            }
            set
            {
                this["CurrentTransaction"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public virtual FieldState State
        {
            get
            {
                return this["State"] == null
                    ? FieldState.Unchanged : (FieldState)this["State"];
            }
            set
            {
                this["State"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void BeforeAdded()
        {

        }

        /// <summary>
        /// Handle an adding of new entity instance's object
        /// </summary>
        public virtual void OnAdded()
        {

        }

        /// <summary>
        /// Handle a deleting of entity
        /// </summary>
        public virtual void OnDeleted()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnCommited()
        {

        }

        public virtual EntityRepository Repository
        {
            get
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    class BusinessRow
    {
        public BusinessRow(String name, Type dataType)
        {
            Name = name;
            DataType = dataType;
        }

        public String Name { get; private set; }

        public Type DataType { get; private set; }
    }

    class BusinessTypeDescriptionProvider : TypeDescriptionProvider
    {
        private static TypeDescriptionProvider defaultTypeProvider =
                       TypeDescriptor.GetProvider(typeof(EntityProvider));

        public BusinessTypeDescriptionProvider()
            : base(defaultTypeProvider)
        {
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType,
                                                                object instance)
        {
            ICustomTypeDescriptor defaultDescriptor =
                                  base.GetTypeDescriptor(objectType, instance);

            return instance == null ? defaultDescriptor :
                new BusinessCustomTypeDescriptor(defaultDescriptor, instance);
        }
    }

    class BusinessCustomTypeDescriptor : CustomTypeDescriptor
    {
        public BusinessCustomTypeDescriptor(ICustomTypeDescriptor parent, object instance)
            : base(parent)
        {
            EntityProvider entity = (EntityProvider)instance;

            customFields.AddRange(GenerateCustomFields()
                .Select(f => new CustomFieldPropertyDescriptor(f)).Cast<PropertyDescriptor>());

        }

        private List<PropertyDescriptor> customFields = new List<PropertyDescriptor>();

        public override PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(base.GetProperties()
                .Cast<PropertyDescriptor>().Union(customFields).ToArray());
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(base.GetProperties(attributes)
                .Cast<PropertyDescriptor>().Union(customFields).ToArray());
        }

        internal static IEnumerable<BusinessRow> GenerateCustomFields()
        {
            yield return new BusinessRow("State", typeof(FieldState));
            yield return new BusinessRow("CurrentTransaction", typeof(ISqlTransaction));
        }
    }

    class CustomFieldPropertyDescriptor : PropertyDescriptor
    {
        public BusinessRow CustomField { get; private set; }

        public CustomFieldPropertyDescriptor(BusinessRow customField)
            : base(customField.Name, new Attribute[0])
        {
            CustomField = customField;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get
            {
                return typeof(EntityProvider);
            }
        }

        public override object GetValue(object component)
        {
            EntityProvider entity = (EntityProvider)component;
            return entity[CustomField.Name] ?? (CustomField.DataType.IsValueType ?
                (Object)Activator.CreateInstance(CustomField.DataType) : null);
        }

        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return CustomField.DataType;
            }
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value)
        {
            EntityProvider entity = (EntityProvider)component;
            entity[CustomField.Name] = value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }

}
