﻿using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace IdentityService.Presentation.Configuration
{
    public class RoutePrefixConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _routePrefix;

        public RoutePrefixConvention(string prefix)
        {
            _routePrefix = new AttributeRouteModel(new Microsoft.AspNetCore.Mvc.RouteAttribute(prefix));
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                foreach (var selector in controller.Selectors)
                {
                    if (selector.AttributeRouteModel != null)
                    {
                        selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                            _routePrefix,
                            selector.AttributeRouteModel);
                    }
                    else
                    {
                        selector.AttributeRouteModel = _routePrefix;
                    }
                }
            }
        }
    }
}
