﻿using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Mod3ASPNET.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Mod3ASPNET.Doc
{
     public class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
     {
          // ==================================================================================================================================================
          //                                                                                                                                                  !
          //                                                                 Fields                                                                           !
          //                                                                                                                                                  !
          // ==================================================================================================================================================
          private readonly IApiVersionDescriptionProvider _provider;





          // ==================================================================================================================================================
          //                                                                                                                                                  !
          //                                                                Constants                                                                         !
          //                                                                                                                                                  !
          // ==================================================================================================================================================





          // ==================================================================================================================================================
          //                                                                                                                                                  !
          //                                                                Properties                                                                        !
          //                                                                                                                                                  !
          // ==================================================================================================================================================





          // ==================================================================================================================================================
          //                                                                                                                                                  !
          //                                                                  Indexers                                                                        !
          //                                                                                                                                                  !
          // ==================================================================================================================================================





          // ==================================================================================================================================================
          //                                                                                                                                                  !
          //                                                                   Events                                                                         !
          //                                                                                                                                                  !
          // ==================================================================================================================================================





          // ==================================================================================================================================================
          //                                                                                                                                                  !
          //                                                                 Constructors                                                                     !
          //                                                                                                                                                  !
          // ==================================================================================================================================================
          public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider p_provider)
          {
               _provider = p_provider;
          }





          // ==================================================================================================================================================
          //                                                                                                                                                  !
          //                                                              Synchronous methods                                                                 !
          //                                                                                                                                                  !
          // ==================================================================================================================================================
          public void Configure(SwaggerGenOptions options)
          {
               foreach (var item in _provider.ApiVersionDescriptions)
               {
                    var info = new OpenApiInfo
                    {
                         Title     = "Mon API ",
                         Version   = item.ApiVersion.ToString()
                    };
                    options.SwaggerDoc(item.GroupName, info);
               }
          }





          // ==================================================================================================================================================
          //                                                                                                                                                  !
          //                                                             Asynchronous methods                                                                 !
          //                                                                                                                                                  !
          // ==================================================================================================================================================





          // ==================================================================================================================================================
          //                                                                                                                                                  !
          //                                                         Standard operators redefined                                                             !
          //                                                                                                                                                  !
          // ==================================================================================================================================================
          // Unary operators ----------------------------------------------------------------------------------------------------------------------------------



          // Binary operators ---------------------------------------------------------------------------------------------------------------------------------



          // Conversion operators -----------------------------------------------------------------------------------------------------------------------------





          // ==================================================================================================================================================
          //                                                                                                                                                  !
          //                                                           Standard methods redefined                                                             !
          //                                                                                                                                                  !
          // ==================================================================================================================================================
          // ToString() ---------------------------------------------------------------------------------------------------------------------------------------



          // Equals() -----------------------------------------------------------------------------------------------------------------------------------------



          // HashCode() ---------------------------------------------------------------------------------------------------------------------------------------





          // ==================================================================================================================================================
          //                                                                                                                                                  !
          //                                                                  Nested types                                                                    !
          //                                                                                                                                                  !
          // ==================================================================================================================================================





          // ==================================================================================================================================================
          //                                                                                                                                                  !
          //                                                                    Finalizer                                                                     !
          //                                                                                                                                                  !
          // ==================================================================================================================================================
          // Unmanaged resources ------------------------------------------------------------------------------------------------------------------------------
     }
}
