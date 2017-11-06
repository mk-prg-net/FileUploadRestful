using System;

namespace Server.WebApi.Areas.HelpPage.ModelDescriptions
{
    public class ParameterAnnotation
    {
        public Attribute AnnotationAttribute { get; set; }

        public string Documentation { get; set; }
    }
}