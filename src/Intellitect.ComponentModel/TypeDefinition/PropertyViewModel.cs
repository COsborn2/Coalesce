﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AutoMapper.Internal;
using System.ComponentModel;
using Intellitect.ComponentModel.DataAnnotations;
using Intellitect.ComponentModel.Utilities;
using Intellitect.ComponentModel.TypeDefinition.Wrappers;
using Microsoft.CodeAnalysis;

namespace Intellitect.ComponentModel.TypeDefinition
{
    public class PropertyViewModel
    {
        /// <summary>
        /// .net PropertyInfo class that gives reflected information about the property.
        /// </summary>
        internal PropertyWrapper Wrapper { get; }

        internal PropertyViewModel(PropertyWrapper propertyWrapper, ClassViewModel parent)
        {
            Wrapper = propertyWrapper;
            Parent = parent;
        }
        public PropertyViewModel(PropertyInfo propertyInfo, ClassViewModel parent)
        {
            Wrapper = new ReflectionPropertyWrapper(propertyInfo);
            Parent = parent;
        }

        public PropertyViewModel(IPropertySymbol propertySymbol, ClassViewModel parent)
        {
            Wrapper = new SymbolPropertyWrapper(propertySymbol);
            Parent = parent;
        }

        public TypeViewModel Type
        {
            get
            {
                return new TypeViewModel(Wrapper.Type);
            }
        }

        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name { get { return Wrapper.Name; } }

        public string Comment { get { return Wrapper.Comment; } }

        /// <summary>
        /// Name of the type
        /// </summary>
        public string TypeName { get { return Wrapper.Type.Name; } }
        public ClassViewModel Parent { get; }

        /// <summary>
        /// Gets the type name without any collection around it.
        /// </summary>
        public TypeViewModel PureType { get { return new TypeViewModel(Wrapper.Type.PureType); } }

        /// <summary>
        /// Gets the name for the API call.
        /// </summary>
        public string Api
        {
            get
            {
                if (Wrapper.Type.IsGeneric && Wrapper.Type.IsCollection)
                {
                    if (IsManytoManyCollection)
                    {
                        return Object.ApiUrl;
                    }
                }
                return Object.ApiUrl;
            }
        }

        public string JsVariable
        {
            get
            {
                return Name.ToCamelCase();
            }
        }

        /// <summary>
        /// Name of the Valid Value list object in JS in Pascal case.
        /// </summary>
        public string ValidValueListName
        {
            get
            {
                return Name + "ValidValues";
            }
        }



        /// <summary>
        /// Text property name for knockout, for things like enums. PureType+'Text'
        /// </summary>
        public string JsTextPropertyName
        {
            get { return Name.ToCamelCase() + "Text"; }
        }


        /// <summary>
        /// Returns true if the property is class outside the system NameSpace, but is not a string, array, or filedownload
        /// </summary>
        public bool IsPOCO
        {
            get
            {
                return !Wrapper.Type.Namespace.StartsWith("System") &&
                  !Wrapper.Type.IsString &&
                  Wrapper.Type.IsClass &&
                  !Wrapper.Type.IsArray &&
                  !Wrapper.Type.IsCollection &&
                  !IsFileDownload;
            }
        }

        /// <summary>
        /// Returns true if this property is a complex type.
        /// </summary>
        public bool IsComplexType
        {
            get
            {
                if (IsPOCO)
                {
                    return Object.IsComplexType;
                }
                return false;
            }
        }


        /// <summary>
        /// True if this property has a ViewModel.
        /// </summary>
        public bool HasViewModel { get { return Object != null && Object.HasDbSet && !IsInternalUse; } }

        /// <summary>
        /// Gets the ClassViewModel associated with the Object
        /// </summary>
        public ClassViewModel Object
        {
            get
            {
                return Wrapper.Type.PureType.ClassViewModel;
            }
        }

        public bool IsDbSet
        {
            get
            {
                return Type.Name.Contains("DbSet");
            }
        }


        /// <summary>
        /// Returns true if this property is a collection and has the ManyToMany Attribute 
        /// </summary>
        public bool IsManytoManyCollection
        {
            get
            {
                if (Wrapper.Type.IsCollection)
                {
                    if (Wrapper.HasAttribute<ManyToManyAttribute>()) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// True if this property has the ClientValidation Attribute
        /// </summary>
        public bool HasClientValidation
        {
            get
            {
                return Wrapper.HasAttribute<ClientValidationAttribute>();
            }
        }

        /// <summary>
        /// True if the client should save data when there is a ClientValidation error. False is default.
        /// </summary>
        public bool ClientValidationAllowSave
        {
            get
            {
                if (!HasClientValidation) return false;
                var allowSave = Wrapper.GetAttributeValue<ClientValidationAttribute>(nameof(ClientValidationAttribute.AllowSave)) as bool?;
                return allowSave.HasValue && allowSave.Value;
            }
        }

        public string BuiltInDataAnnotationsValidationKnockoutJs
        {
            get
            {
                string validationText = "";
                if (IsRequired)
                {
                    string errorMessage = null;
                    if (Wrapper.HasAttribute<RequiredAttribute>())
                    {
                        errorMessage = Wrapper.GetAttributeObject<RequiredAttribute, string>(nameof(RequiredAttribute.ErrorMessage));
                    }
                    validationText = $"required: {KoValidationOptions("true", errorMessage)}";
                }
                if (MaxLength.HasValue)
                {
                    string errorMessage = null;
                    errorMessage = Wrapper.GetAttributeObject<MaxLengthAttribute, string>(nameof(MaxLengthAttribute.ErrorMessage));
                    validationText = $"maxLength: {KoValidationOptions(MaxLength.Value.ToString(), errorMessage)}";
                }
                if (MinLength.HasValue)
                {
                    string errorMessage = null;
                    errorMessage = Wrapper.GetAttributeObject<MinLengthAttribute, string>(nameof(MinLengthAttribute.ErrorMessage));
                    validationText = $"minLength: {KoValidationOptions(MinLength.Value.ToString(), errorMessage)}";
                }
                if (Range != null)
                {
                    string errorMessage = null;
                    errorMessage = Wrapper.GetAttributeObject<RangeAttribute, string>(nameof(RangeAttribute.ErrorMessage));
                    validationText = $"minLength: {KoValidationOptions(Range.Item1.ToString(), errorMessage)}, maxLength: {KoValidationOptions(Range.Item2.ToString(), errorMessage)}";
                }
                return validationText;
            }
        }

        /// <summary>
        /// Gets the Knockout JS text for the validation.
        /// </summary>
        public string ClientValidationKnockoutJs
        {
            get
            {
                if (!HasClientValidation) { return ""; }

                var isRequired = Wrapper.GetAttributeValue<ClientValidationAttribute>(nameof(ClientValidationAttribute.IsRequired)) as bool?;
                var minValue = Wrapper.GetAttributeValue<ClientValidationAttribute>(nameof(ClientValidationAttribute.MinValue)) as double?;
                var maxValue = Wrapper.GetAttributeValue<ClientValidationAttribute>(nameof(ClientValidationAttribute.MaxValue)) as double?;
                var minLength = Wrapper.GetAttributeValue<ClientValidationAttribute>(nameof(ClientValidationAttribute.MinLength)) as int?;
                var maxLength = Wrapper.GetAttributeValue<ClientValidationAttribute>(nameof(ClientValidationAttribute.MaxLength)) as int?;
                var pattern = Wrapper.GetAttributeObject<ClientValidationAttribute, string>(nameof(ClientValidationAttribute.Pattern));
                var step = Wrapper.GetAttributeValue<ClientValidationAttribute>(nameof(ClientValidationAttribute.Step)) as double?;
                var isEmail = Wrapper.GetAttributeValue<ClientValidationAttribute>(nameof(ClientValidationAttribute.IsEmail)) as bool?;
                var isPhoneUs = Wrapper.GetAttributeValue<ClientValidationAttribute>(nameof(ClientValidationAttribute.IsPhoneUs)) as bool?;
                var equal = Wrapper.GetAttributeObject<ClientValidationAttribute, string>(nameof(ClientValidationAttribute.Equal));
                var notEqual = Wrapper.GetAttributeObject<ClientValidationAttribute, string>(nameof(ClientValidationAttribute.NotEqual));
                var isDate = Wrapper.GetAttributeValue<ClientValidationAttribute>(nameof(ClientValidationAttribute.IsDate)) as bool?;
                var isDateIso = Wrapper.GetAttributeValue<ClientValidationAttribute>(nameof(ClientValidationAttribute.IsDateIso)) as bool?;
                var isNumber = Wrapper.GetAttributeValue<ClientValidationAttribute>(nameof(ClientValidationAttribute.IsNumber)) as bool?;
                var isDigit = Wrapper.GetAttributeValue<ClientValidationAttribute>(nameof(ClientValidationAttribute.IsDigit)) as bool?;
                var customName = Wrapper.GetAttributeObject<ClientValidationAttribute, string>(nameof(ClientValidationAttribute.CustomName));
                var customValue = Wrapper.GetAttributeObject<ClientValidationAttribute, string>(nameof(ClientValidationAttribute.CustomValue));
                var errorMessage = Wrapper.GetAttributeObject<ClientValidationAttribute, string>(nameof(ClientValidationAttribute.ErrorMessage));


                var validations = new List<string>();
                if (isRequired.HasValue && isRequired.Value) validations.Add($"required: {KoValidationOptions("true", errorMessage)}");
                if (minValue.HasValue && minValue.Value != double.MaxValue) validations.Add($"min: {KoValidationOptions(minValue.Value.ToString(), errorMessage)}");
                if (maxValue.HasValue && maxValue.Value != double.MinValue) validations.Add($"max: {KoValidationOptions(maxValue.Value.ToString(), errorMessage)}");
                if (minLength.HasValue && minLength.Value != int.MaxValue) validations.Add($"minLength: {KoValidationOptions(MinLength.Value.ToString(), errorMessage)}");
                if (maxLength.HasValue && maxLength.Value != int.MinValue) validations.Add($"maxLength: {KoValidationOptions(MaxLength.Value.ToString(), errorMessage)}");
                if (pattern != null) validations.Add($"pattern: {KoValidationOptions($"'{pattern}'", errorMessage)}");
                if (step.HasValue) validations.Add($"step: {KoValidationOptions($"{step.Value}", errorMessage)}");
                if (isEmail.HasValue && isEmail.Value) validations.Add($"email: {KoValidationOptions("true", errorMessage)}");
                if (isPhoneUs.HasValue && isPhoneUs.Value) validations.Add($"isPhoneUs: {KoValidationOptions("true", errorMessage)}");
                if (equal != null) validations.Add($"equal: {KoValidationOptions($"{equal}", errorMessage)}");
                if (notEqual != null) validations.Add($"notEqual: {KoValidationOptions($"{notEqual}", errorMessage)}");
                if (isDate.HasValue && isDate.Value) validations.Add($"isDate: {KoValidationOptions("true", errorMessage)}");
                if (isDateIso.HasValue && isDateIso.Value) validations.Add($"isDateISO: {KoValidationOptions("true", errorMessage)}");
                if (isNumber.HasValue && isNumber.Value) validations.Add($"isNumber: {KoValidationOptions("true", errorMessage)}");
                if (isDigit.HasValue && isDigit.Value) validations.Add($"isDigit: {KoValidationOptions("true", errorMessage)}");
                if (!string.IsNullOrWhiteSpace(customName) && !string.IsNullOrWhiteSpace(customValue)) validations.Add($"{customName}: {customValue}");

                return string.Join(", ", validations);
            }
        }

        private string AddErrorMessage(string errorMessage)
        {
            string message = null;

            if (!string.IsNullOrWhiteSpace(errorMessage)) message = $", message: \"{errorMessage}\"";

            return message;
        }

        private string KoValidationOptions(string value, string errorMessage)
        {
            string message = AddErrorMessage(errorMessage);
            if (!string.IsNullOrWhiteSpace(message))
            {
                return $"{{params: {value}, message: \"{errorMessage}\"}}";
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Returns the name of the collection to map as a direct many-to-many collection
        /// </summary>
        public string ManyToManyCollectionName
        {
            get
            {
                return Wrapper.GetAttributeValue<ManyToManyAttribute>(nameof(ManyToManyAttribute.CollectionName)) as string;
            }
        }

        /// <summary>
        /// Property on the other side of the many-to-many relationship.
        /// </summary>
        public PropertyViewModel ManyToManyCollectionProperty
        {
            get
            {
                foreach (var prop in Object.Properties)
                {
                    if (prop.IsPOCO && prop.Object.Name != Parent.Name)
                    {
                        return prop;
                    }
                }
                return null;
            }
        }


        /// <summary>
        /// Returns true if this property has the InternalUse Attribute 
        /// </summary>
        public bool IsInternalUse
        {
            get
            {
                return Wrapper.HasAttribute<InternalUseAttribute>();
            }
        }

        /// <summary>
        /// Returns true if this property has the FileDownload Attribute.
        /// </summary>
        public bool IsFileDownload
        {
            get
            {
                return Wrapper.HasAttribute<FileDownloadAttribute>();
            }
        }


        /// <summary>
        /// Only available on reflected types, not during Roslyn compile.
        /// </summary>
        public PropertyInfo PropertyInfo { get { return Wrapper.PropertyInfo; } }


        /// <summary>
        /// True if the property is read only.
        /// </summary>
        public bool IsReadOnly { get { return !CanWrite && CanRead; } }
        /// <summary>
        /// True if the property can be written.
        /// </summary>
        public bool CanWrite { get { return Wrapper.CanWrite && !IsPrimaryKey && !HasReadOnlyAttribute && !HasReadOnlyApiAttribute; } }
        /// <summary>
        /// True if the property can be read.
        /// </summary>
        public bool CanRead { get { return Wrapper.CanRead; } }


        /// <summary>
        /// True if the property has the DateType(DateOnly) Attribute.
        /// </summary>
        public bool IsDateOnly
        {
            get
            {
                var dateType = Wrapper.GetAttributeValue<DateTypeAttribute, DateTypeAttribute.DateTypes>(nameof(DateTypeAttribute.DateType));
                if (dateType != null)
                {
                    return dateType.Value == DateTypeAttribute.DateTypes.DateOnly;
                }
                return false;
            }
        }

        public string DateFormat
        {
            get
            {
                if (IsDateOnly)
                {
                    return "M/D/YYYY";
                }
                else
                {
                    return "M/D/YYYY h:mm a";
                }
            }
        }

        /// <summary>
        /// Returns the DisplayName Attribute or 
        /// puts a space before every upper class letter aside from the first one.
        /// </summary>
        public string DisplayName
        {
            get
            {
                var displayName = Wrapper.GetAttributeValue<DisplayNameAttribute>(nameof(DisplayNameAttribute.DisplayName)) as string;
                if (displayName != null) return displayName;
                displayName = Wrapper.GetAttributeValue<DisplayAttribute>(nameof(DisplayAttribute.Name)) as string;
                if (displayName != null) return displayName;
                else return Regex.Replace(Name, "[A-Z]", " $0").Trim();
            }
        }

        public string DisplayNameLabel(string labelOverride)
        {
            return labelOverride ?? DisplayName;
        }

        /// <summary>
        /// If true, there is an API controller that is serving this type of data.
        /// </summary>
        public bool HasValidValues
        {
            get { return IsManytoManyCollection || ApiController != null; }
        }

        /// <summary>
        /// If this is an object, the name of the API controller serving this data. Or null if none.
        /// </summary>
        public string ApiController
        {
            get
            {
                if (IsPOCO && !IsComplexType) return Name;
                // TODO: Fix this so it is right. 
                //if (IsGeneric)
                //{
                //    if (MvcHelper.GetApiControllerNames().Contains(ContainedType.Name + "Controller"))
                //    {
                //        return ContainedType.Name + "Controller";
                //    }
                //}
                //if (MvcHelper.GetApiControllerNames().Contains(TypeName + "Controller"))
                //{
                //    return TypeName + "Controller";
                //}
                return null;
            }
        }

        /// <summary>
        /// For the specified area, returns true if the property has a hidden attribute.
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public bool IsHidden(HiddenAttribute.Areas area)
        {
            if (IsId) return true;
            // Check the attribute
            var value = Wrapper.GetAttributeValue<HiddenAttribute, HiddenAttribute.Areas>(nameof(HiddenAttribute.Area));
            if (value != null) return value.Value == area || value.Value == HiddenAttribute.Areas.All;
            return false;
        }


        /// <summary>
        /// For the specified area, returns true if the property has a hidden attribute.
        /// </summary>
        /// <returns></returns>
        public string ListGroup
        {
            get
            {
                return (string)Wrapper.GetAttributeValue<ListGroupAttribute>(nameof(ListGroupAttribute.Group));
            }
        }

        /// <summary>
        /// True if the property has the ReadOnly attribute.
        /// </summary>
        /// <returns></returns>
        public bool HasReadOnlyAttribute
        {
            get
            {
                return Wrapper.HasAttribute<ReadOnlyAttribute>();
            }
        }

        /// <summary>
        /// True if the property has the ReadonlyApi attribute.
        /// </summary>
        /// <returns></returns>
        public bool HasReadOnlyApiAttribute
        {
            get
            {
                return Wrapper.HasAttribute<ReadOnlyApiAttribute>();
            }
        }

        /// <summary>
        /// True if the property has the Required attribute, or if the value type is not nullable.
        /// </summary>
        public bool IsRequired
        {
            get
            {
                var value = Wrapper.HasAttribute<RequiredAttribute>();
                if (value) return true;
                if (IsPrimaryKey) return false;  // Because it will be created by the server.
                // TODO: Figure out how to handle situations where we want to hand back an invalid model because the server side is going to figure things out for us.
                //if (IsId && !IsNullable) return true;
                if (Wrapper.Type.IsCollection) return false;
                return false;
            }
        }

        /// <summary>
        /// Returns the MinLength of the property or null if it doesn't exist.
        /// </summary>
        public int? MinLength
        {
            get
            {
                return (int?)Wrapper.GetAttributeValue<MinLengthAttribute>(nameof(MinLengthAttribute.Length));
            }
        }
        /// <summary>
        /// Returns the MaxLength of the property or null if it doesn't exist.
        /// </summary>
        public int? MaxLength
        {
            get
            {
                return (int?)Wrapper.GetAttributeValue<MaxLengthAttribute>(nameof(MaxLengthAttribute.Length));
            }
        }

        /// <summary>
        /// Returns the range of valid values or null if they don't exist. (min, max)
        /// </summary>
        public Tuple<object, object> Range
        {
            get
            {
                var min = Wrapper.GetAttributeValue<RangeAttribute>(nameof(RangeAttribute.Minimum));
                var max = Wrapper.GetAttributeValue<RangeAttribute>(nameof(RangeAttribute.Maximum));
                if (min != null && max != null) return new Tuple<object, object>(min, max);
                return null;
            }
        }

        /// <summary>
        /// Returns true if this property is marked with the Search attribute.
        /// </summary>
        public bool IsSearch
        {
            get
            {
                return Wrapper.HasAttribute<SearchAttribute>();
            }
        }

        /// <summary>
        /// Now to search for the string when this is a search property.
        /// </summary>
        public SearchAttribute.SearchMethods SearchMethod
        {
            get
            {
                var searchMethod = Wrapper.GetAttributeValue<SearchAttribute, SearchAttribute.SearchMethods>(nameof(SearchAttribute.SearchMethod));
                if (searchMethod != null)
                {
                    return searchMethod.Value;
                }
                return SearchAttribute.SearchMethods.BeginsWith;
            }
        }

        /// <summary>
        /// True if the search term should be split on spaces and evaluated individually with or.
        /// </summary>
        public bool SearchIsSplitOnSpaces
        {
            get
            {
                var isSplitOnSpaces = Wrapper.GetAttributeValue<SearchAttribute, bool>(nameof(SearchAttribute.IsSplitOnSpaces));
                if (isSplitOnSpaces != null)
                {
                    return isSplitOnSpaces.Value;
                }
                return false;
            }
        }


        /// <summary>
        /// Returns true if this property is the field to be used for list text and marked with the ListText Attribute.
        /// </summary>
        public bool IsListText
        {
            get
            {
                return Wrapper.HasAttribute<ListTextAttribute>();
            }
        }

        /// <summary>
        /// Returns true if this property is a primary or foreign key.
        /// </summary>
        public bool IsId
        {
            get { return IsPrimaryKey || IsForeignKey; }
        }


        /// <summary>
        /// Returns true if this is the primary key for this object.
        /// </summary>
        public bool IsPrimaryKey
        {
            get
            {
                if (Wrapper.HasAttribute<KeyAttribute>())
                    return true;
                else if (string.Compare(Name, "Id", StringComparison.InvariantCultureIgnoreCase) == 0)
                    return true;
                else
                    return string.Compare(Name, Parent.Name + "Id", StringComparison.InvariantCultureIgnoreCase) == 0;
            }
        }

        /// <summary>
        /// Returns true if this property's name ends with Id.
        /// </summary>
        public bool IsForeignKey
        {
            // TODO: should this be the same as IsPrimaryKey
            get
            {
                if (Wrapper.HasAttribute<ForeignKeyAttribute>() && !IsPOCO)
                {
                    return true;
                }
                if (this.Name.EndsWith("Id") && !IsPrimaryKey && Parent.PropertyByName(Name.Substring(0, Name.Length - 2)) != null)
                {
                    return true;
                }
                return false;
            }
        }



        /// <summary>
        /// If this is an object, returns the name of the property that holds the ID. 
        /// </summary>
        public string ObjectIdPropertyName
        {
            get
            {
                // Use the foreign key attribute
                var value = (string)Wrapper.GetAttributeValue<ForeignKeyAttribute>(nameof(ForeignKeyAttribute.Name));
                if (value != null) return value;
                // See if this is a one-to-one using the parent's key
                // Look up the other object and check the key
                var vm = ReflectionRepository.GetClassViewModel(PureType.Name);
                if (vm != null)
                {
                    if (vm.IsOneToOne)
                    {
                        return Parent.PrimaryKey.Name;
                    }
                }
                // Look on the Object for the key in case of a commonly keyed one-to-one
                return PureType.Name + "Id";
            }
        }


        /// <summary>
        /// If this is an object, returns the property that holds the ID. 
        /// </summary>
        public PropertyViewModel ObjectIdProperty
        {
            get
            {
                return Parent.PropertyByName(ObjectIdPropertyName);
            }
        }


        public string IdFieldCollection
        {
            get
            {
                if (IsManytoManyCollection)
                {
                    return PureType + "Ids";
                }
                else
                {
                    return PureType + "Id";
                }
            }
        }

        /// <summary>
        /// Gets the name of the property that this ID property points to.
        /// </summary>
        private string IdPropertyObjectPropertyName
        {
            get
            {
                if (IsForeignKey)
                {
                    /// Use the ForeignKey Attribute if it is there.
                    var value = (string)Wrapper.GetAttributeValue<ForeignKeyAttribute>(nameof(ForeignKeyAttribute.Name));
                    if (value != null) return value;
                    // Else, by convention remove the Id at the end.
                    return Name.Substring(0, Name.Length - 2);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the property that is the object reference for this ID property.
        /// </summary>
        public PropertyViewModel IdPropertyObjectProperty
        {
            get
            {
                if (string.IsNullOrEmpty(IdPropertyObjectPropertyName))
                {
                    return null;
                }
                else
                {
                    return Parent.PropertyByName(IdPropertyObjectPropertyName);
                }
            }
        }


        public string EditorOrder
        {
            get
            {
                int order = 10000;
                var value = (int?)Wrapper.GetAttributeValue<DisplayAttribute>(nameof(DisplayAttribute.Order));
                if (value != null) order = value.Value;
                // Format them to be sorted.
                return string.Format("{0:D7}:{1}", order, Name);
            }
        }


        public OrderByInformation DefaultOrderBy
        {
            get
            {
                var order = (int?)Wrapper.GetAttributeValue<DefaultOrderByAttribute>(nameof(DefaultOrderByAttribute.FieldOrder));
                var direction = Wrapper.GetAttributeValue<DefaultOrderByAttribute, DefaultOrderByAttribute.OrderByDirections>(nameof(DefaultOrderByAttribute.OrderByDirection));
                if (order != null && direction != null)
                {
                    return new OrderByInformation() { FieldName = Name, FieldOrder = order.Value, OrderByDirection = direction.Value };
                }
                return null;
            }
        }


        /// <summary>
        /// Returns the URL for the List Editor with the ???Id= query string.
        /// Ex: Adult/index?AdultId=
        /// </summary>
        public string ListEditorUrl
        {
            get
            {
                if (InverseIdProperty == null) { return "Nothing"; }
                return string.Format("{0}/table?{1}=", Object.ApiUrl.Replace("api/", ""), InverseIdProperty.Name);
            }
        }
        /// <summary>
        /// Returns the core URL for the List Editor.
        /// </summary>
        public string ListEditorUrlName
        {
            get { return string.Format("{0}ListUrl", Name); }
        }

        public bool HasViewModelProperty
        {
            get
            {
                if (IsInternalUse) return false;
                if (PureType.Name == "Image") return false;
                if (PureType.Name == "IdentityRole") return false;
                if (PureType.Name == "IdentityUserRole") return false;
                if (PureType.Name == "IdentityUserClaim") return false;
                if (PureType.Name == "IdentityUserLogin") return false;
                return true;
            }
        }

        public bool HasInverseProperty
        {
            get { return Wrapper.HasAttribute<InversePropertyAttribute>(); }
        }

        /// <summary>
        /// For a many to many collection this is the reference to this object from the contained object.
        /// </summary>
        public PropertyViewModel InverseProperty
        {
            get
            {
                var name = Wrapper.GetAttributeObject<InversePropertyAttribute, string>(nameof(InversePropertyAttribute.Property));
                if (name != null)
                {
                    var inverseProperty = Object.PropertyByName(name);
                    return inverseProperty;
                }
                else if (Object != null)
                {
                    return Object.PrimaryKey;
                }
                return null;
            }
        }

        /// <summary>
        /// For a many to many collection this is the ID reference to this object from the contained object.
        /// </summary>
        public PropertyViewModel InverseIdProperty
        {
            get
            {
                var inverseProperty = InverseProperty;
                if (inverseProperty != null)
                {
                    return inverseProperty.ObjectIdProperty;
                }
                return null;
            }
        }


        /// <summary>
        /// Name of the database column
        /// </summary>
        public string ColumnName
        {
            get
            {
                //TODO: Make this more robust
                return Name;
            }
        }

        public override string ToString()
        {
            return $"{Name} : {TypeName}";
        }

        public SecurityInfoProperty SecurityInfo
        {
            get
            {
                var result = new SecurityInfoProperty();

                if (Wrapper.HasAttribute<ReadAttribute>())
                {
                    result.IsRead = true;
                    var roles = (string)Wrapper.GetAttributeValue<ReadAttribute>(nameof(ReadAttribute.Roles));
                    result.ReadRoles = roles;
                }

                if (Wrapper.HasAttribute<EditAttribute>())
                {
                    result.IsEdit = true;
                    var roles = (string)Wrapper.GetAttributeValue<EditAttribute>(nameof(EditAttribute.Roles));
                    result.EditRoles = roles;
                }

                return result;
            }
        }


        /// <summary>
        /// Object is not in the database, but hyndrated via other means.
        /// </summary>
        public bool IsExternal
        {
            get
            {
                return IsPOCO && ObjectIdProperty != null && HasNotMapped;
            }
        }


        /// <summary>
        /// Has the NotMapped attribute.
        /// </summary>
        public bool HasNotMapped
        {
            get
            {
                return Wrapper.HasAttribute<NotMappedAttribute>();
            }
        }

        /// <summary>
        /// If true, this property should be searchable on the URL line. 
        /// </summary>
        public bool IsUrlParameter
        {
            get
            {
                return !IsComplexType && (!Type.IsClass || Type.IsString) && !Type.IsArray && !Type.IsGeneric;
            }
        }

        /// <summary>
        /// List of words already used in the API for other things.
        /// </summary>
        private static string ReservedUrlParameterNames =
            "fields,include,includes,orderby,orderbydescending,page,pagesize,where,listdatasource,case,params,if,this,base";
        /// <summary>
        /// Name of the field to use in the API. If this is in ReservedUrlParameterNames, then my is added to the name.
        /// </summary>
        public string UrlParameterName
        {
            get
            {
                if (ReservedUrlParameterNames.Contains(Name.ToLower()))
                {
                    return "my" + Name;
                }
                return Name.ToCamelCase();
            }
        }
    }


}