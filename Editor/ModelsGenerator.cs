using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using UnityCodeGen;
using UnityEngine;

namespace LotW.CherryUI.Editor
{
    [Generator]
    // ReSharper disable once UnusedType.Global
    public class ModelsGenerator : ICodeGenerator
    {
        public void Execute(GeneratorContext context)
        {
            Debug.Log("[Models Code Generator] Start...");
            context.OverrideFolderPath(CodeGenConstants.BasePath);
            
            if (Directory.Exists(Path.Combine(CodeGenConstants.BasePath, CodeGenConstants.ModelsPath)))
            {
                Directory.Delete(Path.Combine(CodeGenConstants.BasePath, CodeGenConstants.ModelsPath), true);
            }

            Directory.CreateDirectory(Path.Combine(CodeGenConstants.BasePath, CodeGenConstants.ModelsPath));
            
            var templates = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.Namespace == CodeGenConstants.TemplateNamespace)
                ;
            
            foreach (var template in templates)
            {
                if (template.IsGenericTypeDefinition)
                {
                    Debug.LogError("[Models Generator] Generating models of Generic types is not supported!!!");
                    continue;
                }
                
                var code = CodeGenConstants.ModelTemplate;
                code = code.Replace(CodeGenConstants.TemplateNamePlaceholder, template.Name);

                var modelName = template.Name.Replace(CodeGenConstants.TemplateSuffix, String.Empty) + CodeGenConstants.ModelSuffix;
                code = code.Replace(CodeGenConstants.ModelNamePlaceholder, modelName);

                var ctor = new StringBuilder();
                var props = new StringBuilder();
                var fields = template.GetFields();

                foreach (var field in fields)
                {
                    var name = field.Name;
                    var typeName = TypeUtils.GetFormattedName(field.FieldType);
                    ctor.Append($"\t\t\tGetters.Add(nameof({name}), new Func<{typeName}>(() => {name}));\n");
                    ctor.Append($"\t\t\tSetters.Add(nameof({name}), new Action<{typeName}>(o => {name} = o));\n");
                    ctor.Append($"\t\t\t{name}Accessor = new Accessor<{typeName}>(this, nameof({name}));");
                    var propHeader =
                        CodeGenConstants.PropertyTemplate.Replace(CodeGenConstants.MemberNamePlaceholder, name);
                    
                    propHeader = propHeader.Replace(CodeGenConstants.TypePlaceholder, typeName);
                    props.Append(propHeader);
                }

                code = code.Replace(CodeGenConstants.ConstructorPlaceholder, ctor.ToString());
                code = code.Replace(CodeGenConstants.PropsPlaceholder, props.ToString());

                var modelFilename = new StringBuilder();
                modelFilename.Append(Path.Combine(CodeGenConstants.ModelsPath, modelName));
                modelFilename.Append(CodeGenConstants.FilenameEnd);
                context.AddCode(modelFilename.ToString(), code);
            }
            
            
            

            Debug.Log("[Models Code Generator] Finished!");
        }
        

    }
}