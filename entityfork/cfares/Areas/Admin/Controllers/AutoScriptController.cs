using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using cfacore.shared.domain.attributes;
using System.ComponentModel;
using cfacore.domain._base;

namespace cfares.Areas.Admin.Controllers
{
    
    public class AutoScriptController : Controller
    {
        //
        // GET: /Admin/AutoScript/

        public ActionResult Index()
        {
            return View();
        }

        [ActionName("DomainModel.js")]
        #if (!DEBUG)
        [OutputCache(Duration = 600, VaryByParam = "none")]
        #endif
        public ActionResult DomainModel()
        {

            #region settypes

            Type[] types = new Type[]{
                    typeof(cfares.domain._event.Slot),
                    typeof(cfares.domain._event.slot.tours.TourSlot),                      
                    typeof(cfares.domain._event.Occurrence),                 
                    typeof(cfares.domain._event.ResTemplate), 
                    typeof(cfares.domain._event.ReservationType),                 
                    typeof(cfares.domain._event.ResTemplate), 
                    typeof(cfares.domain._event.ticket.TicketMeta),             
                    typeof(cfares.domain._event.ticket.tours.TourTicket),   
                    typeof(cfares.domain._event.Schedule),   
                    
                    typeof(cfares.domain.user.ResUser),                                
                    typeof(cfares.domain.user.ResAdmin), 
                    typeof(cfacore.shared.domain.store.Store),                 
                    typeof(cfacore.shared.domain.user.Address),                                                 
                    typeof(cfares.domain._event.ResEvent),
                    typeof(cfacore.shared.domain.user.AddressCollection), 
                    typeof(cfares.domain._event.slot.SlotCollection), 
                    typeof(cfares.domain._event.occurrence.OccurrenceCollection), 
                    typeof(cfares.domain._event.ReservationType), 
                    typeof(cfares.domain._event.ticket.TicketCollection),                 
                    typeof(cfares.domain.user.ResUserCollection),  
                    typeof(cfacore.shared.domain.store.StoreCollection), 
                    typeof(cfares.domain._event.resevent.ResEventCollection)

                };

            Type[] enums = new Type[]{                
                    typeof(cfares.domain._event.TicketStatus),
                    typeof(cfares.domain._event.slot.CameoType),
                    typeof(cfares.domain._event.SlotStatus),
                    typeof(cfares.domain._event.ResEventStatus),
                    typeof(cfares.domain._event.OccurrenceStatus),
                    typeof(cfacore.shared.domain.media.MediaType),
                    typeof(cfacore.domain.user.UserAccountStatus),
                    typeof(cfares.domain.user.UserOperationRole),
                    typeof(cfacore.domain.store.StoreStatus),
                    typeof(cfacore.domain.store.ConceptCode),
                    
                    typeof(cfacore.domain.store.LocationCode)                
                };

            #endregion

            #region declarations and domainObjects

            List<string> backboneDef = new List<string>();
            List<string> enumDef = new List<string>();

            string backboneTemplate = @"DomainModel['{0}'] = Backbone.Model.extend({{  {1}/*todo here*/}});DomainModel['{0}'].name='{0}';";

            string backboneExtensionTemplate = @"DomainModel['{0}'] = DomainModel.{1}.extend({{  {2}/*todo here*/}});DomainModel['{0}'].name='{0}';";

            string backboneCollectionTemplate = @"DomainModel['{0}'] = Backbone.Collection.extend({{  {1}/*todo here*/}});DomainModel['{0}'].name='{0}';";

            string updateUrlTemplate = @"urlRoot:{0},sync:ModelTools.Sync/*,url:ModelTools.ODataUrl*/";

            string updateUrlInnerTemplate = @"function(method){{   switch(method){{      case 'create':         return '{2}';       case 'update':      return '{1}'; 
    case 'delete':     return '{3}';     case 'read':     default:      return '{0}';   }}   }}";

            string modelIdAttributeTemplate = @"idAttribute:'{0}'";
            string collectionModelTemplate = @"model:DomainModel.{0}";
            //string defaultValueTemplate = @"defaults:{{{0}}}";

            //string childDefaultValueTemplate = @"defaults:($.extend({{}},DomainModel.{1}.prototype.defaults, {{{0}}}))";

            string defaultValueTemplate = @"defaults:function(){{ return {{{0}}}; }}";

            string associatedValueTemplate = @"initialize:ModelTools.InitializeAssociations,associations:{{ {0}  }}";

            string childDefaultValueTemplate = @"defaults:function(){{ return $.extend({{}},DomainModel.{1}.prototype.defaults(), {{{0}}}) }}";

            string defaultValueInnerTemplate = @" {0}:{1}";

            //TODO JARED - CREATE A VALIDATION STRING TEMPLATE (javascript)

            string validationTemplate = @"{0}:{{{1}}}";
            string validationTemplateWrapper = @"validation:{{{0}}}";

            //END TODO JARED

            #endregion


            #region iterate types

            foreach (Type modelType in types)
            {

                #region declarations

                Type type = null;
                Type parentDomainModel = null;

                List<string> backboneModelObjects = new List<string>();

                string className = Regex.Replace(modelType.Name, @"^I", "");
                string modelId = string.Empty;



                bool isCollection = IsGenericList(modelType);



                Type collectionModel = null;
                string collectionModelName = string.Empty;

                #endregion


                #region collectionlogic

                if (isCollection)
                {
                    Type listDomainObject = modelType.GetInterfaces().Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
                    collectionModel = listDomainObject.GetGenericArguments().Single(
                        x => typeof(DomainObject).IsAssignableFrom(x)
                        );
                }
                else
                {
                    parentDomainModel = modelType.BaseType;
                    if (!types.AsEnumerable<Type>().Contains(parentDomainModel))
                        parentDomainModel = null;
                }


                if (isCollection && collectionModel != null)
                {

                    collectionModelName = Regex.Replace(collectionModel.Name, @"^I", "");
                    backboneModelObjects.Add(string.Format(collectionModelTemplate, collectionModelName));

                    type = collectionModel.GetInterfaces().Last();
                }
                else
                    type = modelType;


                if (!isCollection && parentDomainModel == null)
                    modelId = className + "Id";
                else
                    modelId = null;

                #endregion


                #region iterate class attributes

                System.Attribute[] attrs = System.Attribute.GetCustomAttributes(type);  // Reflection.
                foreach (System.Attribute attr in attrs)
                {

                    if (attr is SyncUrlAttribute)
                    {
                        SyncUrlAttribute sAttr = (SyncUrlAttribute)attr;
                        string uriString = sAttr.uri;

                        if (sAttr.uri == sAttr.update && sAttr.uri == sAttr.create)
                        {
                            uriString = "'" + uriString + "'";
                        }
                        else
                        {
                            uriString = string.Format(updateUrlInnerTemplate, sAttr.uri, sAttr.update, sAttr.create, sAttr.delete);
                        }

                        backboneModelObjects.Add(string.Format(updateUrlTemplate, uriString));
                    }
                    if (attr is ModelIdAttribute)
                    {
                        modelId = ((ModelIdAttribute)attr).param;
                    }

                }

                #endregion



                #region iterate properties

                System.Reflection.PropertyInfo[] properties = type.GetProperties();  // Reflection.
                List<string> defaultValues = new List<string>();

                List<string> associatedModels = new List<string>();

                List<string> validations = new List<string>();
                foreach (System.Reflection.PropertyInfo property in properties)
                {


                    object defaultVal = null;
                    string column = null;
                    Type defaultType = null;

                    #region iterate property attribute
                    System.Attribute[] memattrs = System.Attribute.GetCustomAttributes(property);  // Reflection.
                    foreach (System.Attribute attr in memattrs)
                    {
                        if (attr is ClientIgnoreAttribute)
                        {
                            column = null;
                            break;
                        }
                        if (attr is ClientDefaultAttribute)
                        {

                            defaultVal = ((ClientDefaultAttribute)attr).val;

                            defaultType = ((ClientDefaultAttribute)attr).type;
                        }
                        if (attr is System.Data.Linq.Mapping.ColumnAttribute)
                        {
                            column = ((System.Data.Linq.Mapping.ColumnAttribute)attr).Name;
                            if (string.IsNullOrEmpty(column))
                                column = property.Name;
                        }
                        if (attr is System.Data.Linq.Mapping.AssociationAttribute)
                        {
                            /*
                            */
                            if (types.AsEnumerable<Type>().Any(m => property.PropertyType.IsAssignableFrom(m)))
                            {
                                /*
                                 * Converting associated objects to domain models
                                 * */
                                string thisKey = ((System.Data.Linq.Mapping.AssociationAttribute)attr).ThisKey;

                                Type associatedType = types.AsEnumerable<Type>().First(m => property.PropertyType.IsAssignableFrom(m));
                                string associatedName = Regex.Replace(associatedType.Name, @"^I", "");
                                defaultVal = string.Format("(new DomainModel.{0}())", associatedName);

                                associatedModels.Add(string.Format("{0}:{{ThisKey:'{1}',Model:'{2}'}}", property.Name, thisKey, associatedName));
                                defaultType = typeof(int);
                                column = property.Name;
                            }
                            else
                            {
                                column = ((System.Data.Linq.Mapping.AssociationAttribute)attr).ThisKey;
                                if (defaultVal == null)
                                    defaultVal = "''";
                                if (string.IsNullOrEmpty(column))
                                    column = property.Name;
                            }
                        }

                        if (typeof(System.ComponentModel.DataAnnotations.ValidationAttribute).IsAssignableFrom(attr.GetType()))
                        {

                            // TODO JARED - PUT VALIDATORS HERE

                            string validationContent = "";
                            bool useTemplate = true;

                            if (attr is System.ComponentModel.DataAnnotations.RequiredAttribute)
                            {
                                validationContent = "required:true";
                            }
                            else if (attr is System.ComponentModel.DataAnnotations.StringLengthAttribute)
                            {
                                System.ComponentModel.DataAnnotations.StringLengthAttribute strAttr = attr as System.ComponentModel.DataAnnotations.StringLengthAttribute;

                                validationContent = "maxLength:" + strAttr.MaximumLength + ",minLength:" + strAttr.MinimumLength;

                            }
                            else if (attr is DataTypeAttribute)
                            {
                                //DataTypeAttribute DataTypeAttr = attr as System.ComponentModel.DataAnnotations.DataTypeAttribute;
                                //validationContent = "DataType:" + DataTypeAttr.DataType;
                            }
                            else if (attr is DisplayNameAttribute)
                            {
                                //DisplayNameAttribute NameAttribute = attr as System.ComponentModel.DisplayNameAttribute;
                                //validationContent = "name:" + NameAttribute.DisplayName;
                            }
                            else if (attr is RangeAttribute)
                            {
                                RangeAttribute RangeAttr = attr as System.ComponentModel.DataAnnotations.RangeAttribute;
                                validationContent = "range: [" + RangeAttr.Minimum + "," + RangeAttr.Maximum + "]";
                            }
                            else if (attr is UIHintAttribute)
                            {
                                //UIHintAttribute UIAttr = attr as System.ComponentModel.DataAnnotations.UIHintAttribute;
                            }
                            else if (attr is RegularExpressionAttribute)
                            {
                                RegularExpressionAttribute RegAttr = attr as System.ComponentModel.DataAnnotations.RegularExpressionAttribute;
                                validationContent = "pattern: /" + RegAttr.Pattern+"/";
                            }

                            System.ComponentModel.DataAnnotations.ValidationAttribute vAttr = attr as System.ComponentModel.DataAnnotations.ValidationAttribute;

                            if (!string.IsNullOrEmpty(vAttr.ErrorMessage) && !string.IsNullOrEmpty(validationContent))
                            {
                                validationContent = validationContent + ",msg:'" + vAttr.ErrorMessage + "'";
                            }

                            if (useTemplate)
                                validations.Add(string.Format(validationTemplate, property.Name, validationContent));
                            else//use this if you want to have a function be the validation - good for custom stuff
                                validations.Add(string.Format("{0}:{1}", property.Name, validationContent));


                            //END TODO JARED

                        }
                    }
                    #endregion

                    #region property logic

                    if (string.IsNullOrEmpty(column))
                        continue;

                    string typename = property.PropertyType.Name.ToLower();
                    if (defaultVal == null)
                    {

                        switch (typename)
                        {
                            case "string":
                                defaultVal = "";
                                break;
                            case "int":
                            case "int32":
                            case "int16":
                            case "int64":
                            case "enum":
                                defaultVal = "0";
                                break;
                            case "float":
                            case "decimal":
                            case "double":
                                defaultVal = "0.0";
                                break;
                            case "bool":
                            case "boolean":
                                defaultVal = "false";
                                break;
                            case "date":
                            case "datetime":
                                defaultVal = "(new Date())";
                                break;
                            case "object":
                            default:
                                defaultVal = "null";
                                break;
                        }

                    }
                    if (
                        (property.PropertyType == typeof(string) && (string)defaultVal != "''")
                        || defaultType == typeof(string))
                    {
                        defaultVal = "'" + defaultVal + "'";
                    }

                    //render default values
                    defaultValues.Add(string.Format(defaultValueInnerTemplate, column, defaultVal));



                    #endregion
                }
                if (associatedModels.Count > 0)
                    backboneModelObjects.Add(string.Format(associatedValueTemplate, string.Join(",", associatedModels)));


                #endregion

                #region apply to string lists

                if (parentDomainModel != null)
                    backboneModelObjects.Add(string.Format(childDefaultValueTemplate, string.Join(",", defaultValues), Regex.Replace(parentDomainModel.Name, @"^I", "")));
                else
                    backboneModelObjects.Add(string.Format(defaultValueTemplate, string.Join(",", defaultValues)));

                backboneModelObjects.Add(string.Format(validationTemplateWrapper, string.Join(",", validations)));

                System.Reflection.MethodInfo[] methods = type.GetMethods();  // Reflection.
                foreach (System.Reflection.MethodInfo method in methods)
                {

                    System.Attribute[] methattrs = System.Attribute.GetCustomAttributes(method);  // Reflection.
                    foreach (System.Attribute attr in methattrs)
                    {



                    }

                }
                if (!string.IsNullOrEmpty(modelId))
                {
                    backboneModelObjects.Add(string.Format(modelIdAttributeTemplate, modelId));
                }
                else if (parentDomainModel != null)
                {
                    // string parentName = Regex.Replace(parentDomainModel.Name, @"^I", "");
                    // backboneModelObjects.Add(string.Format(modelIdAttributeTemplate.Replace("'",""), string.Format("DomainModel['{0}'].__super__.idAttribute",parentName)));
                }

                string template = isCollection ? backboneCollectionTemplate : backboneTemplate;
                if (parentDomainModel != null)
                {
                    string parentName = Regex.Replace(parentDomainModel.Name, @"^I", "");
                    backboneDef.Add(string.Format(backboneExtensionTemplate, className, parentName, string.Join(",", backboneModelObjects)));
                }
                else
                {
                    backboneDef.Add(string.Format(template, className, string.Join(",", backboneModelObjects)));
                }

                #endregion

            }

            #endregion

            #region iterate enums

            string enumTemplate = @"Enum['{0}'] = [ {1} ];{2}EnumFormat.{0}= function (sourceObject, val, column) {{        return Enum.{0}[val];    }};";
            string enumLookupTemplate = @"Enum['{0}']['{1}'] = {2};";
            foreach (Type enumType in enums)
            {
                List<string> enumValues = new List<string>(Enum.GetNames(enumType));

                List<string> enumLookupValues = new List<string>();

                for (int i = 0; i < enumValues.Count; i++)
                {
                    enumLookupValues.Add(string.Format(enumLookupTemplate, enumType.Name, enumValues[i], i));
                    enumValues[i] = "'" + enumValues[i] + "'";
                }

                enumDef.Add(string.Format(enumTemplate,
                    enumType.Name, string.Join(",", enumValues),
                    string.Join(" ", enumLookupValues)
                    ));
            }

            #endregion


            return JavaScript("if(!window.DomainModel)window.DomainModel={};if(!window.Enums)window.Enum={};if(!window.EnumFormat)window.EnumFormat={};"
                + String.Join("\n", backboneDef) + "\n" + String.Join("\n", enumDef));
        }

        bool IsGenericList(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            foreach (Type @interface in type.GetInterfaces())
            {
                if (@interface.IsGenericType)
                {
                    if (@interface.GetGenericTypeDefinition() == typeof(ICollection<>))
                    {
                        // if needed, you can also return the type used as generic argument
                        return true;
                    }
                }
            }
            return false;
        }



        public string ToDescription(Enum value)
        {
            DescriptionAttribute[] da = (DescriptionAttribute[])(value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false));
            return da.Length > 0 ? da[0].Description : value.ToString();
        }

    }
}
