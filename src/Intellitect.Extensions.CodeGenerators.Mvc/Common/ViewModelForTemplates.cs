﻿using Intellitect.ComponentModel.TypeDefinition;
using Microsoft.Extensions.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intellitect.Extensions.CodeGenerators.Mvc.Common
{
    public class ViewModelForTemplates
    {
        public IEnumerable<ClassViewModel> Models { get; set; }

        public ModelType ContextInfo { get; set; }

        public string Namespace { get; set; }

        public List<ViewModelForTemplate> ViewModelsForTemplates
        {
            get
            {
                var result = new List<ViewModelForTemplate>();
                foreach (var model in Models)
                {
                    result.Add(new ViewModelForTemplate
                    {
                        ContextInfo = ContextInfo,
                        Model = model,
                        Namespace = Namespace,
                        AreaName = AreaName,
                        ModulePrefix = ModulePrefix
                    });
                }
                return result;
            }
    }

        public string AreaName { get; internal set; }

        public string ModulePrefix { get; set; }
    }

    public class ViewModelForTemplate
    {
        public ClassViewModel Model { get; set; }

        public ModelType ContextInfo { get; set; }

        public string Namespace { get; set; }
        public string AreaName { get; internal set; }
        public string ModulePrefix { get; set; }
    }

}
