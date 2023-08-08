using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
using System.Web;

namespace ABC.Microservices.Common.Extensions.API.ModelBinders
{
    public class QueryCommandModelBinder : IModelBinder
    {
        private readonly IActionContextAccessor _actionContextAccessor;

        public QueryCommandModelBinder(IActionContextAccessor actionContextAccessor)
        {
            _actionContextAccessor = actionContextAccessor;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Type modelType = bindingContext.ModelType;

            object model = Activator.CreateInstance(modelType);

            foreach (PropertyInfo propertyInfo in modelType.GetProperties())
            {
                foreach (var query in bindingContext.HttpContext.Request.Query)
                {
                    if (query.Key.ToLower() == propertyInfo.Name.ToLower())
                    {
                        if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
                        {
                            try
                            {
                                propertyInfo.SetValue(model, DateTime.ParseExact(query.Value[0], "dd.MM.yyyy", CultureInfo.InvariantCulture));
                            }
                            catch
                            {
                                propertyInfo.SetValue(model, Convert.ChangeType(query.Value[0], propertyInfo.PropertyType), null);
                            }
                        }
                        else
                        {
                            Type type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                            {
                                propertyInfo.SetValue(model, JsonConvert.DeserializeObject(query.Value[0].ToString(), type));
                            }
                            else
                            {
                                propertyInfo.SetValue(model, Convert.ChangeType(query.Value[0], propertyInfo.PropertyType), null);
                            }
                        }

                        break;
                    }
                }

                if (propertyInfo.GetValue(model) == null || propertyInfo.GetValue(model).ToString() == "0")
                {
                    for (int i = 0; i < _actionContextAccessor.ActionContext.RouteData.Values.Keys.Count; i++)
                    {
                        if (_actionContextAccessor.ActionContext.RouteData.Values.Keys.ElementAt(i).ToLower() == propertyInfo.Name.ToLower())
                        {
                            propertyInfo.SetValue(model, Convert.ChangeType(HttpUtility.UrlDecode(_actionContextAccessor.ActionContext.RouteData.Values.Values.ElementAt(i).ToString()),
                                propertyInfo.PropertyType), null);
                            break;
                        }
                    }
                }
            }

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.FromResult(model);
        }
    }
}